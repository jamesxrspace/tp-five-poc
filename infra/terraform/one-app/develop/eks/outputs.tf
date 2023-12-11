output "node_security_group_id" {
  description = "Security group id of the node shared security group."
  value       = module.eks.node_security_group_id
}
