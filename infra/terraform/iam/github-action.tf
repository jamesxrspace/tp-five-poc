resource "aws_iam_user" "github_action" {
  name = "github-action"
}

resource "aws_iam_access_key" "github_action" {
  user = aws_iam_user.github_action.name
}

resource "aws_iam_policy" "github_action_policy" {
  name        = "GithubActionPolicy"
  path        = "/"
  description = "IAM policy for self-hosted github runner."

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": [
        "ec2:Describe*",
        "ec2:TerminateInstances",
        "ec2:StartInstances",
        "ec2:StopInstances",
        "ec2:RunInstances",
        "ec2:CreateVolume",
        "ec2:DeleteVolume",
        "ec2:AttachVolume",
        "ec2:CreateImage",
        "ec2:CreateSnapshot",
        "ec2:DeleteSnapshot",
        "ec2:CreateKeypair",
        "ec2:DeleteKeypair",
        "ec2:CreateSecurityGroup",
        "ec2:DeleteSecurityGroup",
        "ec2:AuthorizeSecurityGroupIngress",
        "ec2:CreateTags",
        "ec2:ModifyInstanceAttribute",
        "s3:PutObject",
        "s3:GetObject",
        "s3:DeleteObject",
        "s3:List*",
        "s3:Get*"
      ],
      "Effect": "Allow",
      "Resource": "*"
    },
    {
      "Effect" : "Allow",
      "Action" : "sagemaker:*",
      "Resource" : "*"
    },
    {
      "Effect" : "Allow",
      "Action" : "sns:*",
      "Resource" : "*"
    },
    {
      "Effect" : "Allow",
      "Action" : [
        "iam:CreatePolicy",
        "iam:DeletePolicy",
        "iam:CreateRole",
        "iam:DeleteRole",
        "iam:AttachRolePolicy",
        "iam:DetachRolePolicy",
        "iam:GetRole",
        "iam:GetPolicy",
        "iam:ListRolePolicies",
        "iam:GetPolicyVersion",
        "iam:ListAttachedRolePolicies",
        "iam:PassRole"
      ],
      "Resource" : "*"
    },
    {
      "Sid" : "SSMPutParameterPermission",
      "Effect" : "Allow",
      "Action" : "ssm:PutParameter",
      "Resource" : "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/github-action-runners/ci-runner/*"
    },
    {
      "Effect" : "Allow",
      "Action" : [
        "ecr:GetDownloadUrlForLayer",
        "ecr:BatchGetImage",
        "ecr:BatchCheckLayerAvailability",
        "ecr:PutImage",
        "ecr:InitiateLayerUpload",
        "ecr:UploadLayerPart",
        "ecr:CompleteLayerUpload",
        "ecr:DescribeImages"
      ],
      "Resource" : "arn:aws:ecr:${var.aws_region}:${data.aws_caller_identity.current.account_id}:repository/*"
    },
    {
      "Effect" : "Allow",
      "Action" : "ecr:GetAuthorizationToken",
      "Resource" : "*"
    },
    {
      "Effect" : "Allow",
      "Action" : [
        "cloudwatch:PutMetricData",
        "cloudwatch:GetMetricStatistics",
        "cloudwatch:ListMetrics"
      ],
      "Resource" : "*"
    },
    {
      "Effect" : "Allow",
      "Action" : [
        "lambda:InvokeFunction"
      ],
      "Resource" : "*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "application-autoscaling:DescribeScalableTargets",
        "application-autoscaling:DescribeScalingPolicies",
        "application-autoscaling:PutScalingPolicy",
        "application-autoscaling:DeleteScalingPolicy",
        "application-autoscaling:RegisterScalableTarget",
        "application-autoscaling:DeregisterScalableTarget",
        "application-autoscaling:ListTagsForResource",
        "application-autoscaling:AddTagsToResource",
        "application-autoscaling:RemoveTagsFromResource"
      ],
      "Resource": "*"
    },
    {
      "Effect": "Allow",
      "Action": [
        "cloudwatch:PutMetricAlarm",
        "cloudwatch:DeleteAlarms",
        "cloudwatch:DescribeAlarms"
      ],
      "Resource": "*" 
    }
  ]
}

EOT
}

resource "aws_iam_user_policy_attachment" "github_action_sops" {
  user       = aws_iam_user.github_action.name
  policy_arn = aws_iam_policy.developer_kms_policy.arn
}

resource "aws_iam_user_policy_attachment" "github_action" {
  user       = aws_iam_user.github_action.name
  policy_arn = aws_iam_policy.github_action_policy.arn
}
