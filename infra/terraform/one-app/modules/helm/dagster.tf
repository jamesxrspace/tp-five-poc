resource "kubernetes_namespace" "dagster" {
  metadata {
    name = var.dagster_chart_namespace
  }
}

resource "kubernetes_config_map" "dagster_workspace" {
  metadata {
    name      = "dagster-workspace"
    namespace = var.dagster_chart_namespace
  }

  data = {
    "workspace.yaml" = <<EOT
load_from:
  - grpc_server:
      location_name: "reports"
      host: "dagster-code-location"
      port: 3030
EOT
  }
}

resource "helm_release" "dagster" {
  name        = var.dagster_chart_name
  chart       = var.dagster_chart_name
  repository  = var.dagster_chart_repo
  version     = var.dagster_chart_version
  namespace   = var.dagster_chart_namespace
  timeout     = 900
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/dagster-system-values.yaml", {
    POSTGRES_HOST           = var.postgres_address,
    POSTGRES_USER           = var.postgres_username,
    POSTGRES_PASSWORD       = var.postgres_password,
    POSTGRES_DB             = var.postgres_db_name,
    POSTGRES_PORT           = var.postgres_port,
    POSTGRES_ENGINE_VERSION = var.postgres_engine_version,
  })]

  depends_on = [kubernetes_namespace.dagster]
}

data "aws_secretsmanager_secret" "github_oauth_dagster_secret" {
  arn = var.github_oauth_dagster_secret_arn
}

data "aws_secretsmanager_secret_version" "github_oauth_dagster_creds" {
  secret_id = data.aws_secretsmanager_secret.github_oauth_dagster_secret.arn
}

locals {
  github_oauth_dagster_creds = jsondecode(
    data.aws_secretsmanager_secret_version.github_oauth_dagster_creds.secret_string
  )
}

resource "helm_release" "oauth2-proxy" {
  name        = var.oauth2_proxy_chart_name
  chart       = var.oauth2_proxy_chart_name
  repository  = var.oauth2_proxy_chart_repo
  version     = var.oauth2_proxy_chart_version
  namespace   = var.dagster_chart_namespace
  timeout     = 900
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/oauth2-proxy-values.yaml", {
    CLIENT_ID     = local.github_oauth_dagster_creds.client_id,
    CLIENT_SECRET = local.github_oauth_dagster_creds.client_secret,
    COOKIE_NAME   = "xrspace_dagster",
    COOKIE_SECRET = local.github_oauth_dagster_creds.cookie_secret,
  })]

  depends_on = [kubernetes_namespace.dagster]
}

resource "kubernetes_manifest" "dagster_gateway" {
  manifest = {
    "apiVersion" = "networking.istio.io/v1alpha3"
    "kind"       = "Gateway"
    "metadata" = {
      "name"      = "dagster-gateway"
      "namespace" = var.dagster_chart_namespace
    }
    "spec" = {
      "selector" = {
        "istio" = "gateway"
      }
      "servers" = [
        {
          "hosts" = [
            var.dagster_host,
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

resource "kubernetes_manifest" "dagster_vs" {
  manifest = {
    "apiVersion" = "networking.istio.io/v1alpha3"
    "kind"       = "VirtualService"
    "metadata" = {
      "name"      = "dagster-vs"
      "namespace" = var.dagster_chart_namespace
    }
    "spec" = {
      "gateways" = [
        "dagster-gateway",
      ]
      "hosts" = [
        var.dagster_host,
      ]
      "http" = [
        {
          "match" = [
            {
              "uri" = {
                "prefix" = "/oauth2"
              },
            }
          ],
          "route" = [
            {
              "destination" = {
                "host" = "oauth2-proxy"
                "port" = {
                  "number" = 80
                }
              }
            },
          ]
        },
        {
          "match" = [
            {
              "uri" = {
                "prefix" = "/"
              },
            }
          ],
          "route" = [
            {
              "destination" = {
                "host" = "dagster-dagster-webserver"
                "port" = {
                  "number" = 80
                }
              }
            },
          ]
        }
      ]
    }
  }
}
