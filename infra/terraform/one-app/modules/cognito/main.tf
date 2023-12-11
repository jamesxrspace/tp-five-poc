############################################
# AWS Cognito User Pool                   ##
############################################
resource "aws_cognito_user_pool" "auth_pool" {
  name = "tpfive_auth_pool_${var.environment}"

  account_recovery_setting {
    recovery_mechanism {
      name     = "verified_email"
      priority = 1
    }

    recovery_mechanism {
      name     = "verified_phone_number"
      priority = 2
    }
  }

  password_policy {
    minimum_length    = 8
    require_lowercase = true
    require_numbers   = true
  }

  username_configuration {
    case_sensitive = false
  }
}

resource "aws_cognito_user_pool_client" "auth_client" {
  name = "tpfive_auth_user_client_${var.environment}"

  user_pool_id = resource.aws_cognito_user_pool.auth_pool.id
}
