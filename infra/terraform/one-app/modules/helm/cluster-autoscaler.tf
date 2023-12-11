resource "aws_iam_policy" "cluster_autoscaler_policy" {
  name        = "EKSClusterAutoscalerIAMPolicy-${var.eks_cluster_name}"
  path        = "/"
  description = "${var.eks_cluster_name} EKS Cluster Autoscaler IAM Policy"

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "autoscaling:DescribeAutoScalingGroups",
        "autoscaling:DescribeAutoScalingInstances",
        "autoscaling:DescribeLaunchConfigurations",
        "autoscaling:DescribeScalingActivities",
        "autoscaling:DescribeTags",
        "ec2:DescribeInstanceTypes",
        "ec2:DescribeLaunchTemplateVersions"
      ],
      "Resource": ["*"]
    },
    {
      "Effect": "Allow",
      "Action": [
        "autoscaling:SetDesiredCapacity",
        "autoscaling:TerminateInstanceInAutoScalingGroup",
        "ec2:DescribeImages",
        "ec2:GetInstanceTypesFromInstanceRequirements",
        "eks:DescribeNodegroup"
      ],
      "Resource": ["*"]
    }
  ]
}

EOT
}

data "aws_iam_policy_document" "cluster_autoscaler_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.cluster_autoscaler_chart_namespace}:cluster-autoscaler"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}

resource "aws_iam_role" "cluster_autoscaler_role" {
  assume_role_policy = data.aws_iam_policy_document.cluster_autoscaler_role_policy.json
  name               = format("AmazonEKSClusterAutoscalerRole-%s", var.eks_cluster_name)
}

resource "aws_iam_role_policy_attachment" "cluster_autoscaler_iam_policy" {
  policy_arn = aws_iam_policy.cluster_autoscaler_policy.arn
  role       = aws_iam_role.cluster_autoscaler_role.name
}

resource "kubernetes_service_account" "cluster_autoscaler" {
  metadata {
    name      = "cluster-autoscaler"
    namespace = var.cluster_autoscaler_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.cluster_autoscaler_role.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "cluster-autoscaler"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}

resource "helm_release" "cluster_autoscaler" {
  name        = var.cluster_autoscaler_chart_name
  chart       = var.cluster_autoscaler_chart_name
  repository  = var.cluster_autoscaler_chart_repo
  version     = var.cluster_autoscaler_chart_version
  namespace   = var.cluster_autoscaler_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/cluster-autoscaler-values.yaml", {
    CLUSTER_NAME        = var.eks_cluster_name,
    AWS_REGION          = var.aws_region,
    SERVICEACCOUNT_NAME = kubernetes_service_account.cluster_autoscaler.metadata[0].name,
  })]

  depends_on = [kubernetes_service_account.cluster_autoscaler]
}
