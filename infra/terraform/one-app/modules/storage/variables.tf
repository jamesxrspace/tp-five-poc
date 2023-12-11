variable "number_of_azs" {
  type = number
}

variable "eks_availability_zones" {
  type = list(any)
}

variable "network_block" {
  type = string
}

variable "private_subnet_prefix_extension" {
  type = number
}

variable "private_zone_offset" {
  type = number
}

variable "environment" {
  type = string
}

variable "service" {
  type = string
}

variable "redis_node_type" {
  type = string
}

variable "redis_number_cache_clusters" {
  type = number
}

variable "redis_engine_version" {
  type = string
}

variable "redis_parameter_group_name" {
  type = string
}

variable "redis_automatic_failover_enabled" {
  type    = bool
  default = true
}

variable "redis_multi_az_enabled" {
  type    = bool
  default = true
}

variable "redis_snapshot_window" {
  type = string
}

variable "redis_snapshot_retention_limit" {
  type = number
}

variable "redis_maintenance_window" {
  type = string
}

variable "vpc_id" {
  type = string
}

variable "eks_private_subnet_CIDR" {
  type = list(any)
}

variable "eks_node_security_group_id" {
  type = string
}

variable "aws_region" {
  type = string
}

variable "org_id" {
  type = string
}

variable "provider_name" {
  type    = string
  default = "AWS"
}

variable "provider_volume_type" {
  type    = string
  default = "STANDARD"
}

variable "provider_instance_size_name" {
  type    = string
  default = "M10"
}

variable "mongodb_dbname" {
  type = string
}

variable "mongodb_username" {
  type = string
}

variable "mongodb_password" {
  type = string
}

variable "eks_private_subnets" {
  type = list(any)
}

variable "cms_acm_arn" {
  type = string
}

variable "cms_domain_name" {
  type = string
}

variable "postgres_instance_class" {
  type = string
}

variable "postgres_engine_version" {
  type = string
}

variable "postgres_username" {
  type = string
}

variable "postgres_password" {
  type = string
}

variable "postgres_maintenance_window" {
  type = string
}

variable "postgres_snapshot_identifier" {
  type = string
}

variable "postgres_db_name" {
  type = string
}

variable "postgres_private_zone_offset" {
  type = number
}

variable "postgres_backup_retention_limit" {
  type = number
}
