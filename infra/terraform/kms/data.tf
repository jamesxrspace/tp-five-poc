data "aws_caller_identity" "current" {}

data "aws_iam_group" "developer" {
  group_name = data.terraform_remote_state.iam.outputs.developer_iam_group_name
}

data "terraform_remote_state" "iam" {
  backend = "s3"
  config = {
    bucket = var.iam_state_backend_bucket_name
    key    = var.iam_state_backend_key
    region = var.iam_state_backend_region
  }
}

data "terraform_remote_state" "storage" {
  backend = "s3"
  config = {
    bucket = var.storage_state_backend_bucket_name
    key    = var.storage_state_backend_key
    region = var.storage_state_backend_region
  }
}
