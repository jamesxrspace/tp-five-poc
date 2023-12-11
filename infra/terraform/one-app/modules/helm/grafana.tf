data "aws_iam_policy_document" "grafana_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.grafana_chart_namespace}:grafana"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}

data "aws_secretsmanager_secret" "github_oauth_grafana_secret" {
  arn = var.github_oauth_grafana_secret_arn
}

data "aws_secretsmanager_secret_version" "github_oauth_grafana_creds" {
  secret_id = data.aws_secretsmanager_secret.github_oauth_grafana_secret.arn
}

resource "aws_iam_role" "grafana" {
  assume_role_policy = data.aws_iam_policy_document.grafana_assume_role_policy.json
  name               = format("GrafanaRole-%s", var.eks_cluster_name)
}

resource "aws_iam_role_policy_attachment" "grafana_amp_iam_policy" {
  policy_arn = "arn:aws:iam::aws:policy/AmazonPrometheusQueryAccess"
  role       = aws_iam_role.grafana.name
}

resource "aws_iam_policy" "cloud_watch_metrics" {
  name        = "AWSCloudWatchMetricsIAMPolicy-${var.eks_cluster_name}"
  path        = "/"
  description = "${var.eks_cluster_name} AWS CloudWatch Metrics IAM Policy"

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "AllowReadingMetricsFromCloudWatch",
      "Effect": "Allow",
      "Action": [
        "cloudwatch:DescribeAlarmsForMetric",
        "cloudwatch:DescribeAlarmHistory",
        "cloudwatch:DescribeAlarms",
        "cloudwatch:ListMetrics",
        "cloudwatch:GetMetricData",
        "cloudwatch:GetInsightRuleReport"
      ],
      "Resource": "*"
    },
    {
      "Sid": "AllowReadingTagsInstancesRegionsFromEC2",
      "Effect": "Allow",
      "Action": ["ec2:DescribeTags", "ec2:DescribeInstances", "ec2:DescribeRegions"],
      "Resource": "*"
    },
    {
      "Sid": "AllowReadingResourcesForTags",
      "Effect": "Allow",
      "Action": "tag:GetResources",
      "Resource": "*"
    }
  ]
}

EOT
}

resource "aws_iam_role_policy_attachment" "grafana_cloudwatch_iam_policy" {
  policy_arn = aws_iam_policy.cloud_watch_metrics.arn
  role       = aws_iam_role.grafana.name
}

resource "kubernetes_service_account" "grafana" {
  metadata {
    name      = "grafana"
    namespace = var.grafana_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.grafana.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "grafana"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}

locals {
  github_oauth_grafana_creds = jsondecode(
    data.aws_secretsmanager_secret_version.github_oauth_grafana_creds.secret_string
  )
}

resource "helm_release" "grafana" {
  name        = var.grafana_chart_name
  chart       = var.grafana_chart_name
  repository  = var.grafana_chart_repo
  version     = var.grafana_chart_version
  namespace   = var.grafana_chart_namespace
  timeout     = 900
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/grafana-values.yaml", {
    CLIENT_ID           = local.github_oauth_grafana_creds.id,
    CLIENT_SECRET       = local.github_oauth_grafana_creds.secret,
    SERVICEACCOUNT_NAME = kubernetes_service_account.grafana.metadata[0].name,
    GRAFANA_HOST        = format("https://%s", var.grafana_host)
  })]

  depends_on = [kubernetes_service_account.grafana]
}

resource "kubernetes_manifest" "grafana_gateway" {
  manifest = {
    "apiVersion" = "networking.istio.io/v1alpha3"
    "kind"       = "Gateway"
    "metadata" = {
      "name"      = "grafana-gateway"
      "namespace" = var.grafana_chart_namespace
    }
    "spec" = {
      "selector" = {
        "istio" = "gateway"
      }
      "servers" = [
        {
          "hosts" = [
            var.grafana_host,
          ]
          "port" = {
            "name"     = "http"
            "number"   = 80
            "protocol" = "HTTP"
          }
        },
      ]
    }
  }
}

resource "kubernetes_manifest" "grafana_vs" {
  manifest = {
    "apiVersion" = "networking.istio.io/v1alpha3"
    "kind"       = "VirtualService"
    "metadata" = {
      "name"      = "grafana-vs"
      "namespace" = var.grafana_chart_namespace
    }
    "spec" = {
      "gateways" = [
        "grafana-gateway",
      ]
      "hosts" = [
        var.grafana_host,
      ]
      "http" = [
        {
          "route" = [
            {
              "destination" = {
                "host" = "grafana"
                "port" = {
                  "number" = 80
                }
              }
            },
          ]
        },
      ]
    }
  }
}
