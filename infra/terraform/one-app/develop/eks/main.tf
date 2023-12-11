###################
# EKS
###################
module "eks" {
  source                         = "terraform-aws-modules/eks/aws"
  version                        = "19.16"
  cluster_name                   = data.terraform_remote_state.network.outputs.eks_cluster_name
  cluster_endpoint_public_access = true
  cluster_version                = "1.27"
  cluster_enabled_log_types      = ["api", "audit", "authenticator", "controllerManager", "scheduler"]
  cluster_addons = {
    coredns = {
      most_recent                 = true
      resolve_conflicts_on_create = "OVERWRITE"
      resolve_conflicts_on_update = "PRESERVE"
    }
    kube-proxy = {
      most_recent                 = true
      resolve_conflicts_on_create = "OVERWRITE"
      resolve_conflicts_on_update = "PRESERVE"
    }
    vpc-cni = {
      most_recent                 = true
      resolve_conflicts_on_create = "OVERWRITE"
      resolve_conflicts_on_update = "PRESERVE"
    }
  }

  subnet_ids = data.terraform_remote_state.network.outputs.eks_private_subnets
  vpc_id     = data.terraform_remote_state.network.outputs.vpc_id

  manage_aws_auth_configmap = true
  kms_key_administrators    = ["arn:aws:iam::047401700492:user/denny.wang", "arn:aws:iam::047401700492:user/pu.chen"]
  # map developer & admin ARNs as kubernetes Users
  aws_auth_users = [
    { "groups" : ["system:masters"], "userarn" : "arn:aws:iam::047401700492:user/denny.wang", "username" : "denny.wang" },
    { "groups" : ["system:masters"], "userarn" : "arn:aws:iam::047401700492:user/pu.chen", "username" : "pu.chen" },
  ]
  # extend the limitations of the security group, so that Lens can capture prometheus metrics
  cluster_security_group_additional_rules = {
    egress_nodes_ephemeral_ports_tcp = {
      description                = "To node 1025-65535"
      protocol                   = "tcp"
      from_port                  = 1025
      to_port                    = 65535
      type                       = "egress"
      source_node_security_group = true
    }
    cluster_egress_https = {
      description                = "Node all egress"
      protocol                   = "tcp"
      from_port                  = 443
      to_port                    = 443
      type                       = "egress"
      source_node_security_group = true
    }
  }

  node_security_group_additional_rules = {
    ingress_self_all = {
      description = "Node to node all ports/protocols"
      protocol    = "-1"
      from_port   = 0
      to_port     = 0
      type        = "ingress"
      self        = true
    }
    ingress_cluster_to_node_all_traffic = {
      description                   = "Cluster API to Nodegroup all traffic"
      protocol                      = "-1"
      from_port                     = 0
      to_port                       = 0
      type                          = "ingress"
      source_cluster_security_group = true
    }
  }
  # Managed Node Groups
  eks_managed_node_group_defaults = {
    iam_role_additional_policies = { additional = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore" }
    capacity_type                = "SPOT"
    ami_type                     = "AL2_x86_64"
    block_device_mappings = {
      xvda = {
        device_name = "/dev/xvda"
        ebs = {
          volume_size           = 100
          volume_type           = "gp3"
          delete_on_termination = true
        }
      }
    }
    update_config = {
      max_unavailable_percentage = 50
    }
  }

  eks_managed_node_groups = {
    app_general = {
      desired_size = 2
      max_size     = 10
      min_size     = 2

      instance_types = ["t3.small", "t3a.small"]
      labels = {
        Service     = "application"
        Type        = "general"
        Environment = local.environment
      }
    }
    monitor_memory = {
      desired_size   = 3
      max_size       = 50
      min_size       = 3
      instance_types = ["t3.medium", "t3a.medium"]

      labels = {
        Service     = "monitor"
        Type        = "memory"
        Environment = local.environment
      }
    }
    core_general = {
      desired_size   = 3
      max_size       = 50
      min_size       = 3
      instance_types = ["t3.medium", "t3a.medium"]

      labels = {
        Service     = "core-component"
        Type        = "general"
        Environment = local.environment
      }
    }
  }
  tags = {
    Environment = local.environment
    Service     = local.service
  }
}

module "helm" {
  source                                  = "../../modules/helm"
  eks_cluster_name                        = data.terraform_remote_state.network.outputs.eks_cluster_name
  aws_region                              = local.aws_region
  aws_oidc_url                            = module.eks.oidc_provider
  aws_oidc_arn                            = module.eks.oidc_provider_arn
  alb_ingress_controller_chart_name       = "aws-load-balancer-controller"
  alb_ingress_controller_chart_repo       = "https://aws.github.io/eks-charts"
  alb_ingress_controller_chart_version    = "1.6.0"
  alb_ingress_controller_chart_namespace  = "kube-system"
  aws_ebs_csi_driver_chart_name           = "aws-ebs-csi-driver"
  aws_ebs_csi_driver_chart_repo           = "https://kubernetes-sigs.github.io/aws-ebs-csi-driver"
  aws_ebs_csi_driver_chart_version        = "2.21.0"
  aws_ebs_csi_driver_chart_namespace      = "kube-system"
  aws_efs_csi_driver_chart_name           = "aws-efs-csi-driver"
  aws_efs_csi_driver_chart_repo           = "https://kubernetes-sigs.github.io/aws-efs-csi-driver"
  aws_efs_csi_driver_chart_version        = "2.4.9"
  aws_efs_csi_driver_chart_namespace      = "kube-system"
  efs_arn                                 = data.terraform_remote_state.storage.outputs.efs_arn
  efs_id                                  = data.terraform_remote_state.storage.outputs.efs_id
  cluster_autoscaler_chart_name           = "cluster-autoscaler"
  cluster_autoscaler_chart_repo           = "https://kubernetes.github.io/autoscaler"
  cluster_autoscaler_chart_version        = "9.29.1"
  cluster_autoscaler_chart_namespace      = "kube-system"
  slack_secret_arn                        = "arn:aws:secretsmanager:ap-southeast-1:047401700492:secret:slack_webhook_url-e9JNTR"
  info_channel                            = "#feed-eks-stage"
  warning_channel                         = "#feed-eks-stage"
  critical_channel                        = "#feed-eks-stage"
  prometheus_chart_name                   = "kube-prometheus-stack"
  prometheus_chart_repo                   = "https://prometheus-community.github.io/helm-charts"
  prometheus_chart_version                = "48.3.1"
  prometheus_chart_namespace              = "monitoring"
  prometheus_memory_limit                 = "4Gi"
  prometheus_memory_request               = "1Gi"
  prometheus_max_metrics_storage          = "10GB"
  environment                             = local.environment
  service                                 = local.service
  istio_chart_namespace                   = "istio-system"
  istio_chart_repo                        = "https://istio-release.storage.googleapis.com/charts"
  istio_chart_version                     = "1.18.2"
  istio_base_chart_name                   = "base"
  metrics_server_chart_name               = "metrics-server"
  metrics_server_chart_repo               = "https://kubernetes-sigs.github.io/metrics-server"
  metrics_server_chart_version            = "3.11.0"
  metrics_server_chart_namespace          = "monitoring"
  istiod_chart_name                       = "istiod"
  istio_ingress_gateway_chart_name        = "gateway"
  external_alb_ssl_cert                   = "arn:aws:acm:ap-southeast-1:047401700492:certificate/20273f91-c724-40e8-80d1-f4dad871b3fe"
  external_alb_host                       = "*.dev.xrspace.io"
  developer_kms_policy_arn                = data.terraform_remote_state.iam.outputs.developer_kms_policy_arn
  github_oauth_argocd_secret_arn          = "arn:aws:secretsmanager:ap-southeast-1:047401700492:secret:github-oauth-argo-YhWo3C"
  argocd_chart_name                       = "argo-cd"
  argocd_chart_repo                       = "https://argoproj.github.io/argo-helm"
  argocd_chart_version                    = "5.43.4"
  argocd_chart_namespace                  = "kube-system"
  argocd_image_updater_chart_name         = "argocd-image-updater"
  argocd_image_updater_chart_version      = "0.9.1"
  account_id                              = data.aws_caller_identity.current.account_id
  helm_secret_version                     = "4.5.0"
  vals_version                            = "0.24.0"
  sops_version                            = "3.7.3"
  kubectl_version                         = "1.28.0"
  argocd_host                             = "argocd.dev.xrspace.io"
  github_oauth_grafana_secret_arn         = "arn:aws:secretsmanager:ap-southeast-1:047401700492:secret:github-oauth-grafana-iWenPI"
  grafana_chart_name                      = "grafana"
  grafana_chart_repo                      = "https://grafana.github.io/helm-charts"
  grafana_chart_version                   = "6.59.1"
  grafana_chart_namespace                 = "monitoring"
  grafana_host                            = "grafana.dev.xrspace.io"
  promtail_chart_name                     = "promtail"
  promtail_chart_repo                     = "https://grafana.github.io/helm-charts"
  promtail_chart_version                  = "6.15.3"
  promtail_chart_namespace                = "kube-system"
  loki_chart_name                         = "loki"
  loki_chart_repo                         = "https://grafana.github.io/helm-charts"
  loki_chart_version                      = "5.20.0"
  loki_chart_namespace                    = "monitoring"
  tempo_chart_name                        = "tempo-distributed"
  tempo_chart_repo                        = "https://grafana.github.io/helm-charts"
  tempo_chart_version                     = "1.6.2"
  tempo_chart_namespace                   = "monitoring"
  opentelemetry_collector_chart_name      = "opentelemetry-collector"
  opentelemetry_collector_chart_repo      = "https://open-telemetry.github.io/opentelemetry-helm-charts"
  opentelemetry_collector_chart_version   = "0.68.0"
  opentelemetry_collector_chart_namespace = "monitoring"
  dagster_chart_name                      = "dagster"
  dagster_chart_repo                      = "https://dagster-io.github.io/helm"
  dagster_chart_version                   = "1.5.9"
  dagster_chart_namespace                 = "dagster"
  dagster_host                            = "dagster.dev.xrspace.io"
  github_oauth_dagster_secret_arn         = "arn:aws:secretsmanager:ap-southeast-1:047401700492:secret:github-oauth-dagster-mgbFpc"
  oauth2_proxy_chart_name                 = "oauth2-proxy"
  oauth2_proxy_chart_repo                 = "https://oauth2-proxy.github.io/manifests"
  oauth2_proxy_chart_version              = "6.19.1"
  oauth2_proxy_chart_namespace            = "oauth2-proxy"
  oauth2_proxy_host                       = "oauth2-proxy.dev.xrspace.io"
  postgres_address                        = data.terraform_remote_state.storage.outputs.postgres_address
  postgres_port                           = data.terraform_remote_state.storage.outputs.postgres_port
  postgres_db_name                        = "xrspace"
  postgres_username                       = local.postgres_user_creds.username
  postgres_password                       = local.postgres_user_creds.password
  postgres_engine_version                 = "15.5"
}

module "namespace" {
  source     = "../../modules/namespace"
  namespaces = ["one-app-develop"]
}

module "sa" {
  source             = "../../modules/sa"
  eks_oidc_url       = module.eks.oidc_provider
  eks_oidc_arn       = module.eks.oidc_provider_arn
  eks_cluster_name   = data.terraform_remote_state.network.outputs.eks_cluster_name
  s3_bucket_name     = data.terraform_remote_state.storage.outputs.s3_backend_bucket_name
  s3_tmp_bucket_name = data.terraform_remote_state.storage.outputs.s3_backend_tmp_bucket_name
  s3_cms_bucket_name = data.terraform_remote_state.storage.outputs.s3_cms_bucket_name
  namespaces         = ["one-app-develop"]
}
