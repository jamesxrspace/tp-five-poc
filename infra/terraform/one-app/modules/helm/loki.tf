data "aws_iam_policy_document" "loki_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.loki_chart_namespace}:loki"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}

resource "aws_iam_role" "loki" {
  assume_role_policy = data.aws_iam_policy_document.loki_assume_role_policy.json
  name               = format("LokiRole-%s", var.eks_cluster_name)
}

resource "aws_s3_bucket" "loki_s3" {
  bucket = format("%s-loki-s3", var.eks_cluster_name)

  tags = {
    Service     = var.service
    Environment = var.environment
  }
}


resource "aws_iam_policy" "loki" {
  name        = "LokiIAMPolicy-${var.eks_cluster_name}"
  path        = "/"
  description = "${var.eks_cluster_name} Loki IAM Policy"

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:ListBucket",
        "s3:PutObject",
        "s3:GetObject",
        "s3:DeleteObject"
      ],
      "Resource": [
          "arn:aws:s3:::${aws_s3_bucket.loki_s3.id}",
          "arn:aws:s3:::${aws_s3_bucket.loki_s3.id}/*"
      ]
    }
  ]
}

EOT
}

resource "aws_iam_role_policy_attachment" "loki_iam_policy" {
  policy_arn = aws_iam_policy.loki.arn
  role       = aws_iam_role.loki.name
}

resource "kubernetes_service_account" "loki" {
  metadata {
    name      = "loki"
    namespace = var.loki_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.loki.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "loki"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}

resource "helm_release" "loki" {
  name        = var.loki_chart_name
  chart       = var.loki_chart_name
  repository  = var.loki_chart_repo
  version     = var.loki_chart_version
  namespace   = var.loki_chart_namespace
  timeout     = 900
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/loki-values.yaml", {
    S3_BUCKET_NAME      = aws_s3_bucket.loki_s3.id,
    SERVICEACCOUNT_NAME = kubernetes_service_account.loki.metadata[0].name,
    AWS_REGION          = var.aws_region
  })]

  depends_on = [kubernetes_service_account.loki]
}
