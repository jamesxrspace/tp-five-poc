data "aws_iam_policy_document" "ebs_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.aws_ebs_csi_driver_chart_namespace}:ebs-csi-controller-sa"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}


resource "aws_iam_role" "ebs_csi_driver" {
  assume_role_policy = data.aws_iam_policy_document.ebs_assume_role_policy.json
  name               = format("AmazonEKSEBSCSIDriverRole-%s", var.eks_cluster_name)
}

resource "aws_iam_role_policy_attachment" "ebs_csi_driver_policy" {
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEBSCSIDriverPolicy"
  role       = aws_iam_role.ebs_csi_driver.name
}

resource "kubernetes_service_account" "ebs_csi_controller" {
  automount_service_account_token = true
  metadata {
    name      = "ebs-csi-controller-sa"
    namespace = var.aws_ebs_csi_driver_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.ebs_csi_driver.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "aws-ebs-csi-driver"
      "app.kubernetes.io/component"  = "controller"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}

# deploy ebs csi driver
resource "helm_release" "aws_ebs_csi_driver" {
  name        = var.aws_ebs_csi_driver_chart_name
  chart       = var.aws_ebs_csi_driver_chart_name
  repository  = var.aws_ebs_csi_driver_chart_repo
  version     = var.aws_ebs_csi_driver_chart_version
  namespace   = var.aws_ebs_csi_driver_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/ebs-csi-driver-values.yaml", {
    SERVICEACCOUNT_NAME = kubernetes_service_account.ebs_csi_controller.metadata[0].name,
  })]

  depends_on = [kubernetes_service_account.ebs_csi_controller]
}

resource "kubernetes_storage_class" "ebs_sc" {
  metadata {
    name = "ebs-sc"
  }
  reclaim_policy      = "Retain"
  storage_provisioner = "ebs.csi.aws.com"
  volume_binding_mode = "WaitForFirstConsumer"
}
