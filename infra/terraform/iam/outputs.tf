output "github_action_access_keys" {
  value     = aws_iam_access_key.github_action.*
  sensitive = true
}

output "developer_iam_group_name" {
  value = var.developer_iam_group_name
}

output "developer_kms_policy_arn" {
  value = aws_iam_policy.developer_kms_policy.arn
}

output "github_action_policy_arn" {
  value = aws_iam_policy.github_action_policy.arn
}
