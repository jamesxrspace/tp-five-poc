data "terraform_remote_state" "network" {
  backend = "s3"
  config = {
    bucket = local.state_backend_bucket_name
    key    = "one-app/develop/network/terraform.tfstate"
    region = local.aws_region
  }
}

data "terraform_remote_state" "eks" {
  backend = "s3"
  config = {
    bucket = local.state_backend_bucket_name
    key    = "one-app/develop/eks/terraform.tfstate"
    region = local.aws_region
  }
}

data "aws_secretsmanager_secret" "mongodbatlas_api_secret" {
  arn = local.mongodbatlas_api_secret_arn
}

data "aws_secretsmanager_secret_version" "mongodbatlas_api_creds" {
  secret_id = data.aws_secretsmanager_secret.mongodbatlas_api_secret.arn
}

data "aws_secretsmanager_secret" "mongodb_user_secret" {
  arn = local.mongodb_user_secret_arn
}

data "aws_secretsmanager_secret_version" "mongodb_user_creds" {
  secret_id = data.aws_secretsmanager_secret.mongodb_user_secret.arn
}

data "aws_secretsmanager_secret" "postgres_user_secret" {
  arn = local.postgres_user_secret_arn
}

data "aws_secretsmanager_secret_version" "postgres_user_creds" {
  secret_id = data.aws_secretsmanager_secret.postgres_user_secret.arn
}
