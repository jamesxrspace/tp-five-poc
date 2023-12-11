resource "helm_release" "metrics_server" {
  name        = var.metrics_server_chart_name
  chart       = var.metrics_server_chart_name
  repository  = var.metrics_server_chart_repo
  version     = var.metrics_server_chart_version
  namespace   = var.metrics_server_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/metrics-server-values.yaml", {})]
}
