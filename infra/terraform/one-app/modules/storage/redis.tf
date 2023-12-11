locals {
  eks_cluster_name                   = format("%s-%s", var.service, var.environment)
  elastic_cache_private_subnets_CIDR = [for zone_name in local.azs : cidrsubnet(var.network_block, var.private_subnet_prefix_extension, index(local.azs, zone_name) + var.private_zone_offset)]
  azs                                = slice(var.eks_availability_zones, 0, var.number_of_azs)
}

###################
# Elastic Cache
###################
resource "aws_elasticache_subnet_group" "redis_cluster" {
  name       = format("%s-redis-subnet-group", local.eks_cluster_name)
  subnet_ids = aws_subnet.redis_private_subnet.*.id
}

resource "aws_elasticache_replication_group" "redis_cluster" {
  replication_group_id = format("%s-redis", local.eks_cluster_name)
  description          = format("%s-redis", local.eks_cluster_name)
  engine               = "redis"

  # Redis settings
  engine_version              = var.redis_engine_version
  port                        = 6379
  parameter_group_name        = var.redis_parameter_group_name
  node_type                   = var.redis_node_type
  num_cache_clusters          = var.redis_number_cache_clusters
  automatic_failover_enabled  = var.redis_automatic_failover_enabled
  subnet_group_name           = aws_elasticache_subnet_group.redis_cluster.name
  preferred_cache_cluster_azs = local.azs
  multi_az_enabled            = var.redis_multi_az_enabled

  # Security
  security_group_ids         = [var.eks_node_security_group_id]
  at_rest_encryption_enabled = false
  transit_encryption_enabled = false

  # Backup
  snapshot_window          = var.redis_snapshot_window
  snapshot_retention_limit = var.redis_snapshot_retention_limit

  # Maintenance
  maintenance_window = var.redis_maintenance_window

  tags = {
    Service     = var.service
    Environment = var.environment
  }
}

########################
# Elastic Cache Subnet
########################

resource "aws_subnet" "redis_private_subnet" {
  count                           = length(local.azs)
  vpc_id                          = var.vpc_id
  cidr_block                      = element(local.elastic_cache_private_subnets_CIDR, count.index)
  availability_zone               = element(local.azs, count.index)
  map_public_ip_on_launch         = false
  assign_ipv6_address_on_creation = false

  tags = {
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-redis-private-subnet-${element(local.azs, count.index)}", local.eks_cluster_name)
  }
}
