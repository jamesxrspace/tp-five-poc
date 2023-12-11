output "auth_pool_id" {
  description = "The PoolID of the AWS Cognito Auth User Pool"

  value = values(module.cognito).*.auth_pool_id
}

output "auth_client_id" {
  description = "The ClientID of the AWS Cognito Auth User Pool Client"
  value       = values(module.cognito).*.auth_client_id
}
