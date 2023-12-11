resource "helm_release" "promtail" {
  name        = var.promtail_chart_name
  chart       = var.promtail_chart_name
  repository  = var.promtail_chart_repo
  version     = var.promtail_chart_version
  namespace   = var.promtail_chart_namespace
  max_history = 3
  atomic      = true

  values = [templatefile("${path.module}/templates/promtail-values.yaml", {})]
}
