resource "random_id" "random" {
  byte_length = 20
}

locals {
  multi_runner_config = { for c in fileset("${path.module}/templates", "*.yaml") : trimsuffix(c, ".yaml") => yamldecode(templatefile("${path.module}/templates/${c}", { AMI_OWNERS = data.aws_caller_identity.current.account_id, RUNNER_IAM_ROLE_MANAGED_POLICY_ARNS = data.terraform_remote_state.iam.outputs.github_action_policy_arn })) }
}

module "gh_runner" {
  source  = "philips-labs/github-runner/aws//modules/multi-runner"
  version = "4.5.0"
  #  Alternative to loading runner configuration from Yaml files is using static configuration:
  multi_runner_config               = local.multi_runner_config
  aws_region                        = data.terraform_remote_state.network.outputs.aws_region
  vpc_id                            = data.terraform_remote_state.network.outputs.vpc_id
  subnet_ids                        = data.terraform_remote_state.network.outputs.ci_runner_private_subnets
  runners_scale_up_lambda_timeout   = 60
  runners_scale_down_lambda_timeout = 60
  prefix                            = "ci-runner"
  logging_retention_in_days         = 7
  tags = {
    Service = "ci-runner"
  }
  github_app = {
    key_base64     = local.github_app_cred.pem
    id             = var.github_app_id
    webhook_secret = random_id.random.hex
  }

  webhook_lambda_zip                = "../lambdas-download/webhook.zip"
  runner_binaries_syncer_lambda_zip = "../lambdas-download/runner-binaries-syncer.zip"
  runners_lambda_zip                = "../lambdas-download/runners.zip"

  # Enable debug logging for the lambda functions
  log_level = "debug"

  cloudwatch_config = jsonencode({
    "agent" : {
      "metrics_collection_interval" : 5
    }
    "metrics" : {
      "metrics_collected" : {
        "mem" : {
          "measurement" : [
            "mem_used_percent"
          ],
          "metrics_collection_interval" : 60
        }
      },
      "append_dimensions" : {
        "InstanceId" : "$${aws:InstanceId}"
      }
    }
  })
}

resource "aws_s3_bucket" "ci_runner_cache" {
  bucket = "xrspace-ci-runner-cache"

  tags = {
    Service = "ci-runner"
  }
}

resource "aws_s3_bucket_lifecycle_configuration" "ci_runner_cache" {
  bucket = aws_s3_bucket.ci_runner_cache.id

  rule {
    id     = "cache"
    status = "Enabled"
    expiration {
      days = 90
    }
  }
}

resource "aws_dynamodb_table" "unity_license_server_dynamodb_table" {
  name         = "unity-license-server"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "id"

  attribute {
    name = "id"
    type = "S"
  }

  tags = {
    Name    = "unity-license-server-db"
    Service = "ci-runner"
  }
}

resource "aws_iam_policy" "unity_license_server_lambda" {
  name        = "UnityLicenseServerLambdaPolicy"
  path        = "/"
  description = "IAM Policy for Unity License Server Lambda Function"

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "dynamodb:BatchGetItem",
        "dynamodb:GetItem",
        "dynamodb:Query",
        "dynamodb:Scan",
        "dynamodb:BatchWriteItem",
        "dynamodb:PutItem",
        "dynamodb:UpdateItem"
      ],
      "Resource": "${aws_dynamodb_table.unity_license_server_dynamodb_table.arn}"
    },
    {
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogStream",
        "logs:PutLogEvents"
      ],
      "Resource": "arn:aws:logs:${data.terraform_remote_state.network.outputs.aws_region}:${data.aws_caller_identity.current.account_id}:*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogGroup"
      ],
      "Resource": "*"
    }
  ]
}

EOT
}

resource "aws_iam_role" "unity_license_server_lambda_role" {
  name = "UnityLicenseServerLambdaRole"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "lambda.amazonaws.com"
        }
        Action = "sts:AssumeRole"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "unity_license_server_lambda_policy_attachment" {
  role       = aws_iam_role.unity_license_server_lambda_role.name
  policy_arn = aws_iam_policy.unity_license_server_lambda.arn
}

resource "aws_lambda_function" "unity_license_server" {
  function_name = "license-server"
  role          = aws_iam_role.unity_license_server_lambda_role.arn
  handler       = "bootstrap"
  filename      = "${path.module}/../../../../one-utility/license-server/server/bootstrap.zip"
  runtime       = "provided.al2"
  memory_size   = 128

  environment {
    variables = {
      TABLE_NAME = aws_dynamodb_table.unity_license_server_dynamodb_table.name
    }
  }

}
