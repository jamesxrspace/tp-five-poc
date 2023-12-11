resource "kubernetes_ingress_v1" "external_alb" {
  metadata {
    name      = "external-alb"
    namespace = var.istio_chart_namespace
    labels = {
      "app.kubernetes.io/name"       = "external-alb"
      "app.kubernetes.io/managed-by" = "terraform"
    }

    annotations = {
      "alb.ingress.kubernetes.io/load-balancer-attributes" = "deletion_protection.enabled=true"
      "alb.ingress.kubernetes.io/load-balancer-name"       = "${var.eks_cluster_name}-external-alb"
      "alb.ingress.kubernetes.io/actions.ssl-redirect" = jsonencode(
        {
          RedirectConfig = {
            Port       = "443"
            Protocol   = "HTTPS"
            StatusCode = "HTTP_301"
          }
          Type = "redirect"
        }
      )
      "alb.ingress.kubernetes.io/certificate-arn"  = var.external_alb_ssl_cert
      "alb.ingress.kubernetes.io/healthcheck-path" = "/healthz/ready"
      "alb.ingress.kubernetes.io/healthcheck-port" = "15021"
      "alb.ingress.kubernetes.io/listen-ports" = jsonencode(
        [
          {
            HTTP = 80
          },
          {
            HTTPS = 443
          },
        ]
      )
      "alb.ingress.kubernetes.io/scheme"      = "internet-facing"
      "alb.ingress.kubernetes.io/target-type" = "ip"
      "kubernetes.io/ingress.class"           = "alb"
    }
  }

  spec {
    rule {
      host = var.external_alb_host
      http {
        path {
          path      = "/"
          path_type = "Prefix"
          backend {
            service {
              name = "ssl-redirect"
              port {
                name = "use-annotation"
              }
            }
          }

        }

        path {
          path      = "/"
          path_type = "Prefix"
          backend {
            service {
              name = var.istio_ingress_gateway_chart_name
              port {
                number = 80
              }
            }
          }
        }
      }
    }
  }
}
