locals {
  aws_region                = "ap-southeast-1"
  state_backend_bucket_name = "xrspace-terraform-backend"
  environment               = "develop"
  service                   = "one-app"
  postgres_user_secret_arn  = "arn:aws:secretsmanager:ap-southeast-1:047401700492:secret:postgres-stage-secret-ySapDK"
  postgres_user_creds = jsondecode(
    data.aws_secretsmanager_secret_version.postgres_user_creds.secret_string
  )
}
