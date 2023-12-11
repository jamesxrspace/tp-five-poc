resource "helm_release" "opentelemetry_collector" {
  name        = var.opentelemetry_collector_chart_name
  chart       = var.opentelemetry_collector_chart_name
  repository  = var.opentelemetry_collector_chart_repo
  version     = var.opentelemetry_collector_chart_version
  namespace   = var.opentelemetry_collector_chart_namespace
  timeout     = 900
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/opentelemetry-collector-values.yaml", {})]
}
