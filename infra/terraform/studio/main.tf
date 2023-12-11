resource "aws_s3_bucket" "training_data" {
  bucket = "studio-training-data"

  tags = {
    Service = var.service
  }
}

resource "aws_s3_bucket_cors_configuration" "training_data_cors_config" {
  bucket = aws_s3_bucket.training_data.id

  cors_rule {
    allowed_headers = ["*"]
    allowed_methods = ["GET"]
    allowed_origins = ["*"]
    max_age_seconds = 3000
  }
}

resource "aws_sns_topic" "edge_server_sns" {
  name = "edge-server-result"
}

resource "aws_sns_topic_subscription" "edge_server_sns_subscribe" {
  endpoint  = var.sns_backend_subscribe_path
  protocol  = "https"
  topic_arn = aws_sns_topic.edge_server_sns.arn
}

data "aws_iam_policy_document" "assume_role" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["sagemaker.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "sagemaker_role" {
  name               = "AmazonSageMakerExecutionRole-${data.terraform_remote_state.network.outputs.eks_cluster_name}"
  assume_role_policy = data.aws_iam_policy_document.assume_role.json
}

resource "aws_iam_policy" "sagemaker_policy" {
  name        = "SageMakerExecutionIAMPolicy-${data.terraform_remote_state.network.outputs.eks_cluster_name}"
  path        = "/"
  description = "${data.terraform_remote_state.network.outputs.eks_cluster_name} SageMaker IAM Policy"

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "sagemaker:*"
      ],
      "Resource": ["*"]
    },
    {
      "Effect": "Allow",
      "Action": [
        "sns:*"
      ],
      "Resource": ["${aws_sns_topic.edge_server_sns.arn}"]
    },
    {
      "Effect": "Allow",
      "Action": [
        "application-autoscaling:DeleteScalingPolicy",
        "application-autoscaling:DeleteScheduledAction",
        "application-autoscaling:DeregisterScalableTarget",
        "application-autoscaling:DescribeScalableTargets",
        "application-autoscaling:DescribeScalingActivities",
        "application-autoscaling:DescribeScalingPolicies",
        "application-autoscaling:DescribeScheduledActions",
        "application-autoscaling:PutScalingPolicy",
        "application-autoscaling:PutScheduledAction",
        "application-autoscaling:RegisterScalableTarget",
        "aws-marketplace:ViewSubscriptions",
        "cloudformation:GetTemplateSummary",
        "cloudwatch:DeleteAlarms",
        "cloudwatch:DescribeAlarms",
        "cloudwatch:GetMetricData",
        "cloudwatch:GetMetricStatistics",
        "cloudwatch:ListMetrics",
        "cloudwatch:PutMetricAlarm",
        "cloudwatch:PutMetricData",
        "codecommit:BatchGetRepositories",
        "codecommit:CreateRepository",
        "codecommit:GetRepository",
        "codecommit:List*",
        "cognito-idp:AdminAddUserToGroup",
        "cognito-idp:AdminCreateUser",
        "cognito-idp:AdminDeleteUser",
        "cognito-idp:AdminDisableUser",
        "cognito-idp:AdminEnableUser",
        "cognito-idp:AdminRemoveUserFromGroup",
        "cognito-idp:CreateGroup",
        "cognito-idp:CreateUserPool",
        "cognito-idp:CreateUserPoolClient",
        "cognito-idp:CreateUserPoolDomain",
        "cognito-idp:DescribeUserPool",
        "cognito-idp:DescribeUserPoolClient",
        "cognito-idp:List*",
        "cognito-idp:UpdateUserPool",
        "cognito-idp:UpdateUserPoolClient",
        "ec2:CreateNetworkInterface",
        "ec2:CreateNetworkInterfacePermission",
        "ec2:CreateVpcEndpoint",
        "ec2:DeleteNetworkInterface",
        "ec2:DeleteNetworkInterfacePermission",
        "ec2:DescribeDhcpOptions",
        "ec2:DescribeNetworkInterfaces",
        "ec2:DescribeRouteTables",
        "ec2:DescribeSecurityGroups",
        "ec2:DescribeSubnets",
        "ec2:DescribeVpcEndpoints",
        "ec2:DescribeVpcs",
        "ecr:BatchCheckLayerAvailability",
        "ecr:BatchGetImage",
        "ecr:CreateRepository",
        "ecr:Describe*",
        "ecr:GetAuthorizationToken",
        "ecr:GetDownloadUrlForLayer",
        "ecr:StartImageScan",
        "elastic-inference:Connect",
        "elasticfilesystem:DescribeFileSystems",
        "elasticfilesystem:DescribeMountTargets",
        "fsx:DescribeFileSystems",
        "glue:CreateJob",
        "glue:DeleteJob",
        "glue:GetJob*",
        "glue:GetTable*",
        "glue:GetWorkflowRun",
        "glue:ResetJobBookmark",
        "glue:StartJobRun",
        "glue:StartWorkflowRun",
        "glue:UpdateJob",
        "groundtruthlabeling:*",
        "iam:ListRoles",
        "kms:DescribeKey",
        "kms:ListAliases",
        "lambda:ListFunctions",
        "logs:CreateLogDelivery",
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:DeleteLogDelivery",
        "logs:Describe*",
        "logs:GetLogDelivery",
        "logs:GetLogEvents",
        "logs:ListLogDeliveries",
        "logs:PutLogEvents",
        "logs:PutResourcePolicy",
        "logs:UpdateLogDelivery",
        "robomaker:CreateSimulationApplication",
        "robomaker:DescribeSimulationApplication",
        "robomaker:DeleteSimulationApplication",
        "robomaker:CreateSimulationJob",
        "robomaker:DescribeSimulationJob",
        "robomaker:CancelSimulationJob",
        "secretsmanager:ListSecrets",
        "servicecatalog:Describe*",
        "servicecatalog:List*",
        "servicecatalog:ScanProvisionedProducts",
        "servicecatalog:SearchProducts",
        "servicecatalog:SearchProvisionedProducts",
        "sns:ListTopics",
        "tag:GetResources"
      ],
      "Resource": ["*"]
    },
    {
      "Effect": "Allow",
      "Action": [
        "s3:PutObject",
        "s3:GetObject",
        "s3:DeleteObject"
      ],
      "Resource": [
        "arn:aws:s3:::${data.terraform_remote_state.storage.outputs.s3_backend_bucket_name}",
        "arn:aws:s3:::${data.terraform_remote_state.storage.outputs.s3_backend_bucket_name}/*"
      ]
    },
    {
      "Effect": "Allow",
      "Action": [
        "kms:Decrypt",
        "kms:Encrypt",
        "kms:GenerateDataKey*"
      ],
      "Resource": [
        "${data.terraform_remote_state.kms.outputs.sagemaker_kms_arn}"
      ]
    }
  ]
}

EOT
}

resource "aws_iam_role_policy_attachment" "sagemaker" {
  role       = aws_iam_role.sagemaker_role.name
  policy_arn = aws_iam_policy.sagemaker_policy.arn
}

resource "aws_sagemaker_model" "model" {
  name                     = format("%s-edge-server-%s", data.terraform_remote_state.network.outputs.eks_cluster_name, var.edge_server_image_tag)
  execution_role_arn       = aws_iam_role.sagemaker_role.arn
  enable_network_isolation = false

  primary_container {
    image          = "${data.aws_caller_identity.current.account_id}.dkr.ecr.${var.aws_region}.amazonaws.com/tp-five/one-ai/edge-server:${var.edge_server_image_tag}"
    model_data_url = var.edge_server_model_data_s3_url
  }
  vpc_config {
    security_group_ids = [data.terraform_remote_state.eks.outputs.node_security_group_id]
    subnets            = data.terraform_remote_state.network.outputs.eks_private_subnets
  }
  tags = {
    Service = var.service
  }
}

resource "aws_sagemaker_endpoint_configuration" "endpoint_config" {
  name = format("%s-edge-server-endpoint-config", data.terraform_remote_state.network.outputs.eks_cluster_name)

  production_variants {
    variant_name           = var.edge_server_variant_name
    model_name             = aws_sagemaker_model.model.name
    initial_instance_count = 1
    instance_type          = var.instance_type
    initial_variant_weight = 1
  }
  async_inference_config {
    output_config {
      s3_output_path  = "s3://${data.terraform_remote_state.storage.outputs.s3_backend_bucket_name}/edge-server/output"
      s3_failure_path = "s3://${data.terraform_remote_state.storage.outputs.s3_backend_bucket_name}/edge-server/output"
      notification_config {
        error_topic   = aws_sns_topic.edge_server_sns.id
        success_topic = aws_sns_topic.edge_server_sns.id
      }
      kms_key_id = data.terraform_remote_state.kms.outputs.sagemaker_kms_arn
    }
  }
  depends_on = [aws_sagemaker_model.model]
}

resource "aws_sagemaker_endpoint" "endpoint" {
  name                 = var.edge_server_endpoint_name
  endpoint_config_name = aws_sagemaker_endpoint_configuration.endpoint_config.name
  depends_on           = [aws_sagemaker_endpoint_configuration.endpoint_config]
}

resource "aws_appautoscaling_target" "sagemaker_target" {
  max_capacity       = 10
  min_capacity       = 1
  resource_id        = format("endpoint/%s/variant/%s", var.edge_server_endpoint_name, var.edge_server_variant_name)
  scalable_dimension = "sagemaker:variant:DesiredInstanceCount"
  service_namespace  = "sagemaker"
  depends_on         = [aws_sagemaker_endpoint_configuration.endpoint_config, aws_sagemaker_endpoint.endpoint]
}

resource "aws_appautoscaling_policy" "autoscale_policy_sagemaker" {
  name               = "SageMakerEndpointInvocationScalingPolicy"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.sagemaker_target.resource_id
  scalable_dimension = aws_appautoscaling_target.sagemaker_target.scalable_dimension
  service_namespace  = aws_appautoscaling_target.sagemaker_target.service_namespace

  target_tracking_scaling_policy_configuration {
    customized_metric_specification {
      metric_name = "ApproximateBacklogSizePerInstance"
      namespace   = "AWS/SageMaker"

      dimensions {
        name  = "EndpointName"
        value = var.edge_server_endpoint_name
      }

      statistic = "Maximum"
    }
    target_value       = 2
    scale_in_cooldown  = 180
    scale_out_cooldown = 60
  }
  depends_on = [aws_appautoscaling_target.sagemaker_target, aws_sagemaker_endpoint_configuration.endpoint_config]
}
