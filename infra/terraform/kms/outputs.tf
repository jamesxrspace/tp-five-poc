output "developer_kms_arn" {
  value = aws_kms_key.developer_sops_key.arn
}

output "devops_kms_arn" {
  value = aws_kms_key.devops_sops_key.arn
}

output "sagemaker_kms_arn" {
  value = aws_kms_key.sagemaker_key.arn
}
