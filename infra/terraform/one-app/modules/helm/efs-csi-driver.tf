data "aws_iam_policy_document" "efs_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.aws_efs_csi_driver_chart_namespace}:efs-csi-controller-sa"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}


resource "aws_iam_role" "efs_csi_driver" {
  assume_role_policy = data.aws_iam_policy_document.efs_assume_role_policy.json
  name               = format("AmazonEKSEFSCSIDriverRole-%s", var.eks_cluster_name)
}

resource "aws_iam_role_policy_attachment" "efs_csi_driver_policy" {
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEFSCSIDriverPolicy"
  role       = aws_iam_role.efs_csi_driver.name
}

resource "kubernetes_service_account" "efs_csi_controller" {
  automount_service_account_token = true
  metadata {
    name      = "efs-csi-controller-sa"
    namespace = var.aws_efs_csi_driver_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.efs_csi_driver.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "aws-efs-csi-driver"
      "app.kubernetes.io/component"  = "controller"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}

# deploy efs csi driver
resource "helm_release" "aws_efs_csi_driver" {
  name        = var.aws_efs_csi_driver_chart_name
  chart       = var.aws_efs_csi_driver_chart_name
  repository  = var.aws_efs_csi_driver_chart_repo
  version     = var.aws_efs_csi_driver_chart_version
  namespace   = var.aws_efs_csi_driver_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/efs-csi-driver-values.yaml", {
    SERVICEACCOUNT_NAME = kubernetes_service_account.efs_csi_controller.metadata[0].name,
  })]

  depends_on = [kubernetes_service_account.efs_csi_controller]
}

resource "kubernetes_storage_class" "efs_sc" {
  metadata {
    name = "efs-sc"
  }
  reclaim_policy      = "Retain"
  storage_provisioner = "efs.csi.aws.com"
  mount_options       = ["tls"]
  parameters = {
    provisioningMode = "efs-ap"
    fileSystemId     = var.efs_id
    directoryPerms   = "700"
    uid              = "65534" # POSIX user Id to be applied for Access Point root directory creation.
    gid              = "65534" # POSIX group Id to be applied for Access Point root directory creation.
    basePath         = "/dynamic_provisioning"
  }
}
