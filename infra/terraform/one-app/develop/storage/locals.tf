locals {
  aws_region                  = "ap-southeast-1"
  state_backend_bucket_name   = "xrspace-terraform-backend"
  environment                 = "develop"
  service                     = "one-app"
  mongodbatlas_api_secret_arn = "arn:aws:secretsmanager:ap-southeast-1:047401700492:secret:mongo-atlas-api-key-4zkRgG"
  mongodb_user_secret_arn     = "arn:aws:secretsmanager:ap-southeast-1:047401700492:secret:mongodb-stage-secret-m8bGrx"
  mongodb_user_creds = jsondecode(
    data.aws_secretsmanager_secret_version.mongodb_user_creds.secret_string
  )
  postgres_user_secret_arn = "arn:aws:secretsmanager:ap-southeast-1:047401700492:secret:postgres-stage-secret-ySapDK"
  postgres_user_creds = jsondecode(
    data.aws_secretsmanager_secret_version.postgres_user_creds.secret_string
  )
}
