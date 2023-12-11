output "webhook" {
  value = module.gh_runner.webhook.endpoint
}

output "ci_runner_cache_s3_arn" {
  value = aws_s3_bucket.ci_runner_cache.arn
}
