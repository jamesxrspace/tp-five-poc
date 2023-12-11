# get current aws key in .env or ~/.aws/credentials
data "aws_caller_identity" "current" {}

# get all available AZs in our region
data "aws_availability_zones" "available_azs" {}

data "terraform_remote_state" "network" {
  backend = "s3"
  config = {
    bucket = local.state_backend_bucket_name
    key    = "one-app/develop/network/terraform.tfstate"
    region = local.aws_region
  }
}

data "terraform_remote_state" "iam" {
  backend = "s3"
  config = {
    bucket = local.state_backend_bucket_name
    key    = "iam/terraform.tfstate"
    region = local.aws_region
  }
}

data "terraform_remote_state" "storage" {
  backend = "s3"
  config = {
    bucket = local.state_backend_bucket_name
    key    = "one-app/develop/storage/terraform.tfstate"
    region = local.aws_region
  }
}

data "aws_secretsmanager_secret" "postgres_user_secret" {
  arn = local.postgres_user_secret_arn
}

data "aws_secretsmanager_secret_version" "postgres_user_creds" {
  secret_id = data.aws_secretsmanager_secret.postgres_user_secret.arn
}
