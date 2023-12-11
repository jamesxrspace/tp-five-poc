output "auth_pool_id" {
  description = "The PoolID of the AWS Cognito Auth User Pool"
  value       = resource.aws_cognito_user_pool.auth_pool.id
}

output "auth_client_id" {
  description = "The ClientID of the AWS Cognito Auth User Pool Client"
  value       = resource.aws_cognito_user_pool_client.auth_client.id
}
