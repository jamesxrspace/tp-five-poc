resource "aws_iam_policy" "developer_kms_policy" {
  name        = "DeveloperKMSPolicy"
  path        = "/"
  description = "Developer Access KMS IAM Policy"

  policy = <<EOT
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Action": [
        "kms:Decrypt",
        "kms:DescribeKey",
        "kms:Encrypt",
        "kms:GenerateDataKey",
        "kms:GetKeyPolicy",
        "kms:GetKeyRotationStatus",
        "kms:GetParametersForImport",
        "kms:GetPublicKey",
        "kms:ReEncryptFrom",
        "kms:ReEncryptTo",
        "kms:Sign",
        "kms:Verify"
      ],
      "Effect": "Allow",
      "Resource": "${data.terraform_remote_state.kms.outputs.developer_kms_arn}"
    },
    {
      "Effect": "Allow",
      "Action": [
        "s3:Get*",
        "s3:List*"
      ],
      "Resource": [
        "arn:aws:s3:::one-app-develop*",
        "arn:aws:s3:::one-app-develop*/*"
      ]
    }
  ]
}

EOT
}

resource "aws_iam_group_policy_attachment" "developer_kms_policy" {
  policy_arn = aws_iam_policy.developer_kms_policy.arn
  group      = var.developer_iam_group_name
}
