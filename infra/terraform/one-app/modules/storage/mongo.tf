resource "mongodbatlas_project" "project" {
  name                              = var.service
  org_id                            = var.org_id
  is_extended_storage_sizes_enabled = true
}

resource "mongodbatlas_privatelink_endpoint" "pe" {
  project_id    = mongodbatlas_project.project.id
  provider_name = "AWS"
  region        = var.aws_region
}

resource "aws_vpc_endpoint" "vpc_endpoint" {
  vpc_id             = var.vpc_id
  service_name       = mongodbatlas_privatelink_endpoint.pe.endpoint_service_name
  vpc_endpoint_type  = "Interface"
  subnet_ids         = var.eks_private_subnets
  security_group_ids = [var.eks_node_security_group_id]
}

resource "mongodbatlas_privatelink_endpoint_service" "pe_service" {
  project_id          = mongodbatlas_privatelink_endpoint.pe.project_id
  private_link_id     = mongodbatlas_privatelink_endpoint.pe.id
  endpoint_service_id = aws_vpc_endpoint.vpc_endpoint.id
  provider_name       = "AWS"
}

resource "mongodbatlas_cluster" "cluster" {
  project_id   = mongodbatlas_project.project.id
  name         = format("%s-%s", var.service, var.environment)
  cluster_type = "REPLICASET"
  replication_specs {
    num_shards = 1
    regions_config {
      region_name     = upper(replace(var.aws_region, "-", "_"))
      electable_nodes = 3
      priority        = 7
      read_only_nodes = 0
    }
  }
  provider_name               = var.provider_name
  provider_region_name        = upper(replace(var.aws_region, "-", "_"))
  provider_instance_size_name = var.provider_instance_size_name
  provider_volume_type        = var.provider_volume_type
  cloud_backup                = true
  lifecycle {
    ignore_changes = [
      disk_size_gb,
    ]
  }
  depends_on = [mongodbatlas_privatelink_endpoint_service.pe_service]
}

resource "mongodbatlas_database_user" "user" {
  username           = var.mongodb_username
  password           = var.mongodb_password
  project_id         = mongodbatlas_project.project.id
  auth_database_name = "admin"
  roles {
    role_name     = "readWrite"
    database_name = var.mongodb_dbname
  }
  roles {
    role_name     = "readAnyDatabase"
    database_name = "admin"
  }
  scopes {
    name = format("%s-%s", var.service, var.environment)
    type = "CLUSTER"
  }
}
