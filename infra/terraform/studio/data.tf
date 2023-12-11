data "aws_caller_identity" "current" {}

data "terraform_remote_state" "network" {
  backend = "s3"
  config = {
    bucket = var.state_backend_bucket_name
    key    = var.network_state_backend_key
    region = var.aws_region
  }
}

data "terraform_remote_state" "storage" {
  backend = "s3"
  config = {
    bucket = var.state_backend_bucket_name
    key    = var.storage_state_backend_key
    region = var.aws_region
  }
}

data "terraform_remote_state" "eks" {
  backend = "s3"
  config = {
    bucket = var.state_backend_bucket_name
    key    = var.eks_state_backend_key
    region = var.aws_region
  }
}

data "terraform_remote_state" "kms" {
  backend = "s3"
  config = {
    bucket = var.kms_state_backend_bucket_name
    key    = var.kms_state_backend_key
    region = var.aws_region
  }
}
