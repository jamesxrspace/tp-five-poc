data "aws_caller_identity" "current" {}

data "aws_secretsmanager_secret_version" "creds" {
  secret_id = var.github_app_cred_arn
}

locals {
  github_app_cred = jsondecode(
    data.aws_secretsmanager_secret_version.creds.secret_string
  )
}

data "terraform_remote_state" "network" {
  backend = "s3"
  config = {
    bucket = var.network_state_backend_bucket_name
    key    = var.network_state_backend_key
    region = var.network_state_backend_region
  }
}

data "terraform_remote_state" "iam" {
  backend = "s3"
  config = {
    bucket = var.iam_state_backend_bucket_name
    key    = var.iam_state_backend_key
    region = var.iam_state_backend_region
  }
}
