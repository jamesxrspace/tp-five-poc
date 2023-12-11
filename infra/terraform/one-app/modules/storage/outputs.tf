output "redis_endpoint" {
  value = aws_elasticache_replication_group.redis_cluster.primary_endpoint_address
}

locals {
  private_endpoints = flatten([for cs in mongodbatlas_cluster.cluster.connection_strings : cs.private_endpoint])

  connection_strings = [
    for pe in local.private_endpoints : pe.srv_connection_string
    if contains([for e in pe.endpoints : e.endpoint_id], aws_vpc_endpoint.vpc_endpoint.id)
  ]
}

output "connection_string" {
  value = length(local.connection_strings) > 0 ? local.connection_strings[0] : ""
}

output "s3_backend_bucket_name" {
  value = aws_s3_bucket.s3_backend.id
}

output "s3_backend_tmp_bucket_name" {
  value = aws_s3_bucket.s3_backend_tmp.id
}

output "s3_cms_bucket_name" {
  value = aws_s3_bucket.s3_cms.id
}

output "efs_arn" {
  value = aws_efs_file_system.efs.arn
}

output "efs_id" {
  value = aws_efs_file_system.efs.id
}

output "cloudfront_backend_arn" {
  value = aws_cloudfront_distribution.backend_dist.arn
}

output "postgres_address" {
  value = aws_db_instance.postgres_db.address
}

output "postgres_port" {
  value = aws_db_instance.postgres_db.port
}
