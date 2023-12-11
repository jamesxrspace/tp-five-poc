data "aws_iam_policy_document" "tempo_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.tempo_chart_namespace}:tempo"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}

resource "aws_iam_role" "tempo" {
  assume_role_policy = data.aws_iam_policy_document.tempo_assume_role_policy.json
  name               = format("TempoRole-%s", var.eks_cluster_name)
}

resource "aws_s3_bucket" "tempo_s3" {
  bucket = format("%s-tempo-s3", var.eks_cluster_name)

  tags = {
    Service     = var.service
    Environment = var.environment
  }
}


resource "aws_iam_policy" "tempo" {
  name        = "TempoIAMPolicy-${var.eks_cluster_name}"
  path        = "/"
  description = "${var.eks_cluster_name} Tempo IAM Policy"

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:PutObject",
        "s3:GetObject",
        "s3:ListBucket",
        "s3:DeleteObject",
        "s3:GetObjectTagging",
        "s3:PutObjectTagging"
      ],
      "Resource": [
          "arn:aws:s3:::${aws_s3_bucket.tempo_s3.id}",
          "arn:aws:s3:::${aws_s3_bucket.tempo_s3.id}/*"
      ]
    }
  ]
}

EOT
}

resource "aws_iam_role_policy_attachment" "tempo_iam_policy" {
  policy_arn = aws_iam_policy.tempo.arn
  role       = aws_iam_role.tempo.name
}

resource "kubernetes_service_account" "tempo" {
  metadata {
    name      = "tempo"
    namespace = var.tempo_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.tempo.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "tempo"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}

resource "helm_release" "tempo" {
  name        = var.tempo_chart_name
  chart       = var.tempo_chart_name
  repository  = var.tempo_chart_repo
  version     = var.tempo_chart_version
  namespace   = var.tempo_chart_namespace
  timeout     = 900
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/tempo-distributed-values.yaml", {
    S3_BUCKET_NAME      = aws_s3_bucket.tempo_s3.id,
    SERVICEACCOUNT_NAME = kubernetes_service_account.tempo.metadata[0].name,
    AWS_REGION          = var.aws_region
  })]

  depends_on = [kubernetes_service_account.tempo]
}
