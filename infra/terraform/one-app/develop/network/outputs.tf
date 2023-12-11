output "vpc_id" {
  value = module.network.vpc_id
}

output "eks_private_subnets" {
  value = module.network.eks_private_subnets
}

output "eks_public_subnets" {
  value = module.network.eks_public_subnets
}

output "eks_cluster_name" {
  value = module.network.eks_cluster_name
}

output "eks_availability_zones" {
  value = module.network.eks_availability_zones
}

output "eks_private_subnet_CIDR" {
  value = module.network.eks_private_subnet_CIDR
}

output "network_block" {
  value = module.network.network_block
}
