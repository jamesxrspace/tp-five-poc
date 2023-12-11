variable "aws_oidc_url" {
  type = string
}

variable "aws_oidc_arn" {
  type = string
}

variable "eks_cluster_name" {
  type = string
}

variable "aws_region" {
  type = string
}

variable "alb_ingress_controller_chart_name" {
  type        = string
  description = "ALB Ingress Controller Helm chart name."
}

variable "alb_ingress_controller_chart_repo" {
  type        = string
  description = "ALB Ingress Controller Helm repository name."
}

variable "alb_ingress_controller_chart_version" {
  type        = string
  description = "ALB Ingress Controller Helm chart version."
}

variable "alb_ingress_controller_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy ALB Ingress Controller Helm chart."
}

variable "efs_arn" {
  type = string
}

variable "efs_id" {
  type = string
}

variable "aws_efs_csi_driver_chart_name" {
  type        = string
  description = "AWS For EFS CSI Driver Helm chart name."
}

variable "aws_efs_csi_driver_chart_repo" {
  type        = string
  description = "AWS For EFS CSI Driver Helm repository name."
}

variable "aws_efs_csi_driver_chart_version" {
  type        = string
  description = "AWS For EFS CSI Driver Helm chart version."
}

variable "aws_efs_csi_driver_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy AWS For EFS CSI Driver Helm chart."
}

variable "aws_ebs_csi_driver_chart_name" {
  type        = string
  description = "AWS For EBS CSI Driver Helm chart name."
}

variable "aws_ebs_csi_driver_chart_repo" {
  type        = string
  description = "AWS For EBS CSI Driver Helm repository name."
}

variable "aws_ebs_csi_driver_chart_version" {
  type        = string
  description = "AWS For EBS CSI Driver Helm chart version."
}

variable "aws_ebs_csi_driver_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy AWS For EBS CSI Driver Helm chart."
}

variable "cluster_autoscaler_chart_name" {
  type        = string
  description = "Cluster Autoscaler Helm chart name."
}

variable "cluster_autoscaler_chart_repo" {
  type        = string
  description = "Cluster Autoscaler Helm repository name."
}

variable "cluster_autoscaler_chart_version" {
  type        = string
  description = "Cluster Autoscaler Helm chart version."
}

variable "cluster_autoscaler_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Cluster Autoscaler Helm chart."
}

variable "slack_secret_arn" {
  type = string
}

variable "info_channel" {
  type = string
}

variable "warning_channel" {
  type = string
}

variable "critical_channel" {
  type = string
}

variable "prometheus_max_metrics_storage" {
  type = string
}

variable "prometheus_chart_name" {
  type        = string
  description = "Prometheus Helm chart name."
}

variable "prometheus_chart_repo" {
  type        = string
  description = "Prometheus Helm repository name."
}

variable "prometheus_chart_version" {
  type        = string
  description = "Prometheus Helm chart version."
}

variable "prometheus_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Prometheus Helm chart."
}

variable "prometheus_memory_request" {
  type = string
}

variable "prometheus_memory_limit" {
  type = string
}

variable "environment" {
  type = string
}

variable "service" {
  type = string
}

variable "istio_chart_repo" {
  type        = string
  description = "Istio Helm repository name."
}

variable "istio_chart_version" {
  type        = string
  description = "Istio Helm chart version."
}

variable "istio_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Istio Helm chart."
}

variable "istio_base_chart_name" {
  type        = string
  description = "Istio Base Helm chart name."
}

variable "istiod_chart_name" {
  type        = string
  description = "Istiod Helm chart name."
}

variable "istio_ingress_gateway_chart_name" {
  type        = string
  description = "Istio ingress gateway Helm chart name."
}

variable "metrics_server_chart_name" {
  type        = string
  description = "Metrics server Helm chart name."
}

variable "metrics_server_chart_repo" {
  type        = string
  description = "Metrics server Helm repository name."
}

variable "metrics_server_chart_version" {
  type        = string
  description = "Metrics server Helm chart version."
}

variable "metrics_server_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Metrics server Helm chart."
}

variable "external_alb_ssl_cert" {
  type = string
}

variable "external_alb_host" {
  type = string
}

variable "developer_kms_policy_arn" {
  type = string
}

variable "github_oauth_argocd_secret_arn" {
  type = string
}

variable "argocd_chart_name" {
  type        = string
  description = "ArgoCD Helm chart name."
}

variable "argocd_chart_repo" {
  type        = string
  description = "ArgoCD Helm repository name."
}

variable "argocd_chart_version" {
  type        = string
  description = "ArgoCD Helm chart version."
}

variable "argocd_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy ArgoCD Helm chart."
}

variable "helm_secret_version" {
  type = string
}

variable "vals_version" {
  type = string
}

variable "sops_version" {
  type = string
}

variable "kubectl_version" {
  type = string
}

variable "argocd_host" {
  type = string
}

variable "account_id" {
  type = string
}

variable "argocd_image_updater_chart_name" {
  type = string
}

variable "argocd_image_updater_chart_version" {
  type = string
}

variable "github_oauth_grafana_secret_arn" {
  type = string
}

variable "github_oauth_dagster_secret_arn" {
  type = string
}

variable "grafana_chart_name" {
  type        = string
  description = "Grafana Helm chart name."
}

variable "grafana_chart_repo" {
  type        = string
  description = "Grafana Helm repository name."
}

variable "grafana_chart_version" {
  type        = string
  description = "Grafana Helm chart version."
}

variable "grafana_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Grafana Helm chart."
}

variable "grafana_host" {
  type = string
}

variable "promtail_chart_name" {
  type        = string
  description = "Promtail Helm chart name."
}

variable "promtail_chart_repo" {
  type        = string
  description = "Promtail Helm repository name."
}

variable "promtail_chart_version" {
  type        = string
  description = "Promtail Helm chart version."
}

variable "promtail_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Promtail Helm chart."
}

variable "loki_chart_name" {
  type        = string
  description = "Loki Helm chart name."
}

variable "loki_chart_repo" {
  type        = string
  description = "Loki Helm repository name."
}

variable "loki_chart_version" {
  type        = string
  description = "Loki Helm chart version."
}

variable "loki_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Loki Helm chart."
}

variable "tempo_chart_name" {
  type        = string
  description = "Tempo Helm chart name."
}

variable "tempo_chart_repo" {
  type        = string
  description = "Tempo Helm repository name."
}

variable "tempo_chart_version" {
  type        = string
  description = "Tempo Helm chart version."
}

variable "tempo_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Tempo Helm chart."
}

variable "opentelemetry_collector_chart_name" {
  type        = string
  description = "Opentelemetry Collector Helm chart name."
}

variable "opentelemetry_collector_chart_repo" {
  type        = string
  description = "Opentelemetry Collector Helm repository name."
}

variable "opentelemetry_collector_chart_version" {
  type        = string
  description = "Opentelemetry Collector Helm chart version."
}

variable "opentelemetry_collector_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Opentelemetry Collector Helm chart."
}

variable "dagster_host" {
  type = string
}

variable "dagster_chart_name" {
  type        = string
  description = "Dagster Helm chart name."
}

variable "dagster_chart_repo" {
  type        = string
  description = "Dagster Helm repository name."
}

variable "dagster_chart_version" {
  type        = string
  description = "Dagster Helm chart version."
}

variable "dagster_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Dagster Helm chart."
}

variable "oauth2_proxy_host" {
  type = string
}

variable "oauth2_proxy_chart_name" {
  type        = string
  description = "Oauth2 proxy Helm chart name."
}

variable "oauth2_proxy_chart_repo" {
  type        = string
  description = "Oauth2 proxy Helm repository name."
}

variable "oauth2_proxy_chart_version" {
  type        = string
  description = "Oauth2 proxy Helm chart version."
}

variable "oauth2_proxy_chart_namespace" {
  type        = string
  description = "Kubernetes namespace to deploy Oauth2 proxy Helm chart."
}

variable "postgres_address" {
  type        = string
  description = "Postgres host address."
}

variable "postgres_port" {
  type        = string
  description = "Postgres port."
}

variable "postgres_db_name" {
  type        = string
  description = "Postgres db name."
}

variable "postgres_username" {
  type        = string
  description = "Postgres username."
}

variable "postgres_password" {
  type        = string
  description = "Postgres password."
}

variable "postgres_engine_version" {
  type        = string
  description = "Postgres engine version."
}
