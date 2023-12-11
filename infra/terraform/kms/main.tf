resource "aws_kms_key" "developer_sops_key" {
  description             = "SOPS KMS Key for Developers"
  deletion_window_in_days = 7
  enable_key_rotation     = true
}

resource "aws_kms_key" "devops_sops_key" {
  description             = "SOPS KMS Key for DevOps"
  deletion_window_in_days = 7
  enable_key_rotation     = true
}

resource "aws_kms_key" "sagemaker_key" {
  description             = "KMS Key for SageMaker"
  deletion_window_in_days = 7
  policy                  = data.aws_iam_policy_document.sagemaker_kms_policy.json
  enable_key_rotation     = true
}

data "aws_iam_policy_document" "sagemaker_kms_policy" {
  statement {
    actions   = ["kms:Decrypt", "kms:Encrypt", "kms:GenerateDataKey*"]
    effect    = "Allow"
    resources = ["*"]
    principals {
      type        = "Service"
      identifiers = ["cloudfront.amazonaws.com"]
    }
    condition {
      test     = "StringEquals"
      variable = "aws:SourceArn"
      values   = [data.terraform_remote_state.storage.outputs.cloudfront_backend_arn]
    }
  }

  # This statement ensures that the root account can manage the key
  statement {
    actions   = ["kms:*"]
    effect    = "Allow"
    resources = ["*"]
    principals {
      type        = "AWS"
      identifiers = ["arn:aws:iam::${data.aws_caller_identity.current.account_id}:root"]
    }
  }
}
