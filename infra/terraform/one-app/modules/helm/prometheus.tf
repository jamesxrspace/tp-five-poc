resource "aws_prometheus_workspace" "amp" {
  alias = format("%s-PROMETHEUS", var.eks_cluster_name)

  tags = {
    Service     = var.service
    Environment = var.environment
  }
}

data "aws_secretsmanager_secret" "slack_secret" {
  arn = var.slack_secret_arn
}

data "aws_secretsmanager_secret_version" "slack_creds" {
  secret_id = data.aws_secretsmanager_secret.slack_secret.arn
}

resource "kubernetes_namespace" "monitoring" {
  metadata {
    name = var.prometheus_chart_namespace
  }
}

data "aws_iam_policy_document" "prometheus_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.prometheus_chart_namespace}:prometheus"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}

resource "aws_iam_role" "prometheus_role" {
  assume_role_policy = data.aws_iam_policy_document.prometheus_assume_role_policy.json
  name               = format("AmazonEKSPrometheusRole-%s", var.eks_cluster_name)
}

resource "aws_iam_role_policy_attachment" "prometheus_iam_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonPrometheusRemoteWriteAccess"
  role       = aws_iam_role.prometheus_role.name
}

resource "kubernetes_service_account" "prometheus" {
  metadata {
    name      = "prometheus"
    namespace = var.prometheus_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.prometheus_role.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "prometheus"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
  depends_on = [kubernetes_namespace.monitoring]
}

locals {
  slack_creds = jsondecode(
    data.aws_secretsmanager_secret_version.slack_creds.secret_string
  )
}

resource "helm_release" "prometheus" {
  name        = var.prometheus_chart_name
  chart       = var.prometheus_chart_name
  repository  = var.prometheus_chart_repo
  version     = var.prometheus_chart_version
  namespace   = var.prometheus_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/prometheus-values.yaml", {
    PROMETHEUS_SERVICE_ACCOUNT_NAME = kubernetes_service_account.prometheus.metadata[0].name,
    PROMETHEUS_ENDPOINT             = "https://aps-workspaces.${var.aws_region}.amazonaws.com/workspaces/${aws_prometheus_workspace.amp.id}/api/v1/remote_write"
    PROMETHEUS_CLUSTER_NAME         = format("%s-PROMETHEUS", var.eks_cluster_name)
    PROMETHEUS_REGION               = var.aws_region
    SLACK_API_URL                   = local.slack_creds.url
    CRITICAL_CHANNEL                = var.critical_channel
    WARNING_CHANNEL                 = var.warning_channel
    INFO_CHANNEL                    = var.info_channel
    MEMORY_LIMIT                    = var.prometheus_memory_limit
    MEMORY_REQUEST                  = var.prometheus_memory_request
    MAX_METRICS_STORAGE             = var.prometheus_max_metrics_storage
  })]
  depends_on = [kubernetes_service_account.prometheus]
}
