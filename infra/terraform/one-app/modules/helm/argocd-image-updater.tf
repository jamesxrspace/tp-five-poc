data "aws_iam_policy_document" "argocd_image_updater_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.alb_ingress_controller_chart_namespace}:argocd-image-updater"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}

resource "aws_iam_role" "argocd_image_updater" {
  assume_role_policy = data.aws_iam_policy_document.argocd_image_updater_assume_role_policy.json
  name               = format("ArgoCDImageUpdaterRole-%s", var.eks_cluster_name)
}

resource "aws_iam_role_policy_attachment" "argocd_image_updater_iam_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly"
  role       = aws_iam_role.argocd_image_updater.name
}

resource "kubernetes_service_account" "argocd_image_updater" {
  metadata {
    name      = "argocd-image-updater"
    namespace = var.argocd_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.argocd_image_updater.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "argocd-image-updater"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}

resource "helm_release" "argocd_image_updater" {
  name        = var.argocd_image_updater_chart_name
  chart       = var.argocd_image_updater_chart_name
  repository  = var.argocd_chart_repo
  version     = var.argocd_image_updater_chart_version
  namespace   = var.argocd_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/argocd-image-updater-values.yaml", {
    AWS_REGION          = var.aws_region,
    SERVICEACCOUNT_NAME = kubernetes_service_account.argocd_image_updater.metadata[0].name,
    ACCOUNT_ID          = var.account_id,
  })]

  depends_on = [kubernetes_service_account.argocd_image_updater]
}
