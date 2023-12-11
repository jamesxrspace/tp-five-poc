resource "kubernetes_namespace" "istio" {
  metadata {
    name = var.istio_chart_namespace
  }
}

resource "helm_release" "istio_base" {
  name        = var.istio_base_chart_name
  chart       = var.istio_base_chart_name
  repository  = var.istio_chart_repo
  version     = var.istio_chart_version
  namespace   = var.istio_chart_namespace
  max_history = 3
  atomic      = true

  depends_on = [kubernetes_namespace.istio]
}

resource "helm_release" "istiod" {
  name        = var.istiod_chart_name
  chart       = var.istiod_chart_name
  repository  = var.istio_chart_repo
  version     = var.istio_chart_version
  namespace   = var.istio_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/istiod-values.yaml", {})]

  depends_on = [helm_release.istio_base]
}

resource "helm_release" "istio_ingress_gateway" {
  name        = var.istio_ingress_gateway_chart_name
  chart       = var.istio_ingress_gateway_chart_name
  repository  = var.istio_chart_repo
  version     = var.istio_chart_version
  namespace   = var.istio_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/istio-ingress-gateway-values.yaml", {})]

  depends_on = [helm_release.istiod]
}

resource "kubernetes_manifest" "envoyfilter" {
  manifest = {
    "apiVersion" = "networking.istio.io/v1alpha3"
    "kind"       = "EnvoyFilter"
    "metadata" = {
      "name"      = "default"
      "namespace" = var.istio_chart_namespace
    }
    "spec" = {
      "configPatches" = [
        {
          "applyTo" = "NETWORK_FILTER"
          "match" = {
            "context" = "ANY"
            "listener" = {
              "filterChain" = {
                "filter" = {
                  "name" = "envoy.filters.network.http_connection_manager"
                }
              }
            }
          }
          "patch" = {
            "operation" = "MERGE"
            "value" = {
              "typed_config" = {
                "@type"                        = "type.googleapis.com/envoy.extensions.filters.network.http_connection_manager.v3.HttpConnectionManager"
                "preserve_external_request_id" = true
              }
            }
          }
        },
      ]
    }
  }
}

# Custom auth policy to the dagster host
resource "kubernetes_manifest" "dagster_auth_policy" {
  manifest = {
    apiVersion = "security.istio.io/v1beta1"
    kind       = "AuthorizationPolicy"
    metadata = {
      name      = "auth-policy"
      namespace = var.istio_chart_namespace
    }
    spec = {
      action = "CUSTOM"
      provider = {
        name = "oauth2-proxy"
      }
      rules = [
        {
          to = [
            {
              operation = {
                hosts = [var.dagster_host]
              }
            }
          ]
        }
      ]
    }
  }
}
