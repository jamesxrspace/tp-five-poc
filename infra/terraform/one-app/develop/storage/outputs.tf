output "redis_endpoint" {
  value = module.storage.redis_endpoint
}

output "mongodb_endpoint" {
  value = module.storage.connection_string
}

output "s3_backend_bucket_name" {
  value = module.storage.s3_backend_bucket_name
}

output "s3_backend_tmp_bucket_name" {
  value = module.storage.s3_backend_tmp_bucket_name
}

output "s3_cms_bucket_name" {
  value = module.storage.s3_cms_bucket_name
}

output "efs_arn" {
  value = module.storage.efs_arn
}

output "efs_id" {
  value = module.storage.efs_id
}

output "cloudfront_backend_arn" {
  value = module.storage.cloudfront_backend_arn
}

output "postgres_address" {
  value = module.storage.postgres_address
}

output "postgres_port" {
  value = module.storage.postgres_port
}
