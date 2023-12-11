data "aws_caller_identity" "current" {}

data "terraform_remote_state" "kms" {
  backend = "s3"
  config = {
    bucket = var.kms_state_backend_bucket_name
    key    = var.kms_state_backend_key
    region = var.kms_state_backend_region
  }
}
