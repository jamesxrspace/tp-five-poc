output "vpc_id" {
  value = aws_vpc.vpc.id
}

output "eks_private_subnets" {
  value = aws_subnet.eks_private_subnet.*.id
}

output "eks_public_subnets" {
  value = aws_subnet.eks_public_subnet.*.id
}

output "eks_cluster_name" {
  value = local.eks_cluster_name
}

output "eks_availability_zones" {
  value = local.azs
}

output "eks_private_subnet_CIDR" {
  value = local.eks_private_subnets_CIDR
}

output "network_block" {
  value = var.main_network_block
}
