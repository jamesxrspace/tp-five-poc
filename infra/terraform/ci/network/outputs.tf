output "aws_region" {
  value = var.aws_region
}

output "vpc_id" {
  value = aws_vpc.vpc.id
}

output "ci_runner_private_subnets" {
  value = aws_subnet.ci_runner_private_subnet.*.id
}

output "ci_runner_public_subnets" {
  value = aws_subnet.ci_runner_public_subnet.*.id
}

output "ci_runner_availability_zones" {
  value = local.azs
}

output "ci_runner_private_subnet_CIDR" {
  value = local.ci_runner_private_subnets_CIDR
}

output "network_block" {
  value = var.main_network_block
}
