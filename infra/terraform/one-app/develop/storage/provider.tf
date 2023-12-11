provider "aws" {
  region = local.aws_region
}

locals {
  mongodbatlas_api_creds = jsondecode(
    data.aws_secretsmanager_secret_version.mongodbatlas_api_creds.secret_string
  )
}

provider "mongodbatlas" {
  public_key  = local.mongodbatlas_api_creds.public
  private_key = local.mongodbatlas_api_creds.private
}
