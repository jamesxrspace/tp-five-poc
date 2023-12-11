data "aws_iam_policy_document" "backend_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringLike"
      variable = "${replace(var.eks_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:*:backend"]
    }

    principals {
      identifiers = [var.eks_oidc_arn]
      type        = "Federated"
    }
  }
}

resource "aws_iam_policy" "backend_policy" {
  name        = format("BackendPolicy-%s", var.eks_cluster_name)
  path        = "/"
  description = format("Backend Service Account IAM Policy In EKS %s", var.eks_cluster_name)

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:ListBucket"
      ],
      "Resource": [
          "arn:aws:s3:::${var.s3_bucket_name}",
          "arn:aws:s3:::${var.s3_tmp_bucket_name}",
          "arn:aws:s3:::${var.s3_cms_bucket_name}"
      ]
    },
    {
      "Effect": "Allow",
      "Action": [
        "sagemaker:InvokeEndpoint",
        "sagemaker:InvokeEndpointAsync"
      ],
      "Resource": ["*"]
    },
    {
      "Effect": "Allow",
      "Action": [
        "sns:Subscribe",
        "sns:ConfirmSubscription"
      ],
      "Resource": ["*"]
    },
    {
      "Effect": "Allow",
      "Action": [
        "s3:GetObject",
        "s3:DeleteObject",
        "s3:PutObject"
      ],
      "Resource": [
          "arn:aws:s3:::${var.s3_bucket_name}/*",
          "arn:aws:s3:::${var.s3_tmp_bucket_name}/*",
          "arn:aws:s3:::${var.s3_cms_bucket_name}/*"
      ]
    }
  ]
}
EOT
}

resource "aws_iam_role" "backend_role" {
  assume_role_policy = data.aws_iam_policy_document.backend_role_policy.json
  name               = format("BackendRole-%s", var.eks_cluster_name)
}

resource "aws_iam_role_policy_attachment" "backend_iam_policy" {
  policy_arn = aws_iam_policy.backend_policy.arn
  role       = aws_iam_role.backend_role.name
}

resource "kubernetes_service_account" "backend" {
  count = length(var.namespaces)

  metadata {
    name      = "backend"
    namespace = element(var.namespaces, count.index)
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.backend_role.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "backend"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}
