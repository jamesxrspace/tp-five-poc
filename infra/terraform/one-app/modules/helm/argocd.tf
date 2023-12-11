data "aws_iam_policy_document" "argocd_repo_server_assume_role_policy" {
  statement {
    actions = ["sts:AssumeRoleWithWebIdentity"]
    effect  = "Allow"

    condition {
      test     = "StringEquals"
      variable = "${replace(var.aws_oidc_url, "https://", "")}:sub"
      values   = ["system:serviceaccount:${var.argocd_chart_namespace}:argocd-repo-server"]
    }

    principals {
      identifiers = [var.aws_oidc_arn]
      type        = "Federated"
    }
  }
}

data "aws_secretsmanager_secret" "github_oauth_argocd_secret" {
  arn = var.github_oauth_argocd_secret_arn
}

data "aws_secretsmanager_secret_version" "github_oauth_argocd_creds" {
  secret_id = data.aws_secretsmanager_secret.github_oauth_argocd_secret.arn
}

resource "aws_iam_role" "argocd_repo_server" {
  assume_role_policy = data.aws_iam_policy_document.argocd_repo_server_assume_role_policy.json
  name               = format("ArgoCDRepoServerRole-%s", var.eks_cluster_name)
}

resource "aws_iam_role_policy_attachment" "argocd_repo_server_iam_policy" {
  policy_arn = var.developer_kms_policy_arn
  role       = aws_iam_role.argocd_repo_server.name
}

resource "kubernetes_service_account" "argocd_repo_server" {
  metadata {
    name      = "argocd-repo-server"
    namespace = var.argocd_chart_namespace
    annotations = {
      "eks.amazonaws.com/role-arn" = aws_iam_role.argocd_repo_server.arn
    }
    labels = {
      "app.kubernetes.io/name"       = "argocd-repo-server"
      "app.kubernetes.io/managed-by" = "terraform"
    }
  }
}

locals {
  github_oauth_argocd_creds = jsondecode(
    data.aws_secretsmanager_secret_version.github_oauth_argocd_creds.secret_string
  )
}

resource "helm_release" "argocd" {
  name        = var.argocd_chart_name
  chart       = var.argocd_chart_name
  repository  = var.argocd_chart_repo
  version     = var.argocd_chart_version
  namespace   = var.argocd_chart_namespace
  timeout     = 900
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/argocd-values.yaml", {
    CLIENT_ID                          = local.github_oauth_argocd_creds.id,
    CLIENT_SECRET                      = local.github_oauth_argocd_creds.secret,
    HELM_SECRETS_VERSION               = var.helm_secret_version,
    VALS_VERSION                       = var.vals_version,
    SOPS_VERSION                       = var.sops_version,
    KUBECTL_VERSION                    = var.kubectl_version,
    ARGOCD_REPO_SERVER_SERVICE_ACCOUNT = kubernetes_service_account.argocd_repo_server.metadata[0].name,
    ARGOCD_HOST                        = var.argocd_host
  })]

  depends_on = [kubernetes_service_account.argocd_repo_server]
}

resource "kubernetes_manifest" "argocd_gateway" {
  manifest = {
    "apiVersion" = "networking.istio.io/v1alpha3"
    "kind"       = "Gateway"
    "metadata" = {
      "name"      = "argocd-gateway"
      "namespace" = var.argocd_chart_namespace
    }
    "spec" = {
      "selector" = {
        "istio" = "gateway"
      }
      "servers" = [
        {
          "hosts" = [
            var.argocd_host,
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
  depends_on = [helm_release.argocd]
}

resource "kubernetes_manifest" "argocd_vs" {
  manifest = {
    "apiVersion" = "networking.istio.io/v1alpha3"
    "kind"       = "VirtualService"
    "metadata" = {
      "name"      = "argocd-vs"
      "namespace" = var.argocd_chart_namespace
    }
    "spec" = {
      "gateways" = [
        "argocd-gateway",
      ]
      "hosts" = [
        var.argocd_host,
      ]
      "http" = [
        {
          "route" = [
            {
              "destination" = {
                "host" = "argo-cd-argocd-server"
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
  depends_on = [kubernetes_manifest.argocd_gateway]
}

