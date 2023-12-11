locals {
  rds_azs                  = slice(var.eks_availability_zones, 0, var.number_of_azs)
  rds_private_subnets_CIDR = [for zone_name in local.rds_azs : cidrsubnet(var.network_block, var.private_subnet_prefix_extension, index(local.rds_azs, zone_name) + var.postgres_private_zone_offset)]
}

###################
# RDS
###################
resource "aws_db_subnet_group" "postgres_db" {
  name       = format("%s-postgres-subnet-group", local.eks_cluster_name)
  subnet_ids = aws_subnet.postgres_private_subnet.*.id
}

resource "aws_db_instance" "postgres_db" {
  identifier = format("%s-postgres", local.eks_cluster_name)

  # Postgres settings
  engine               = "postgres"
  instance_class       = var.postgres_instance_class
  allocated_storage    = 10
  engine_version       = var.postgres_engine_version
  db_name              = var.postgres_db_name
  username             = var.postgres_username
  password             = var.postgres_password
  db_subnet_group_name = aws_db_subnet_group.postgres_db.name

  # Security
  vpc_security_group_ids = [var.eks_node_security_group_id]

  # Maintenance
  maintenance_window = var.postgres_maintenance_window

  # Backup
  backup_retention_period = var.postgres_backup_retention_limit

  tags = {
    Service     = var.service
    Environment = var.environment
  }
}

########################
# RDS Subnet
########################

resource "aws_subnet" "postgres_private_subnet" {
  count                           = length(local.rds_azs)
  vpc_id                          = var.vpc_id
  cidr_block                      = element(local.rds_private_subnets_CIDR, count.index)
  availability_zone               = element(local.rds_azs, count.index)
  map_public_ip_on_launch         = false
  assign_ipv6_address_on_creation = false

  tags = {
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-postgres-private-subnet-${element(local.rds_azs, count.index)}", local.eks_cluster_name)
  }
}
