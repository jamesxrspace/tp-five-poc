# get all available AZs in our region
data "aws_availability_zones" "available_azs" {
  state = "available"
}

locals {
  azs = slice(data.aws_availability_zones.available_azs.names, 0, var.number_of_azs)
  # this loop will create a one-line list as ["10.1.176.0/20", "10.1.192.0/20", "10.1.208.0/20", ...]
  # with a length depending on how many Zones are available
  ci_runner_private_subnets_CIDR = [for zone_name in local.azs : cidrsubnet(var.main_network_block, var.private_subnet_prefix_extension, index(local.azs, zone_name) + var.private_zone_offset)]
  # this loop will create a one-line list as ["10.1.160.0/24", "10.1.161.0/24", "10.1.162.0/24", ...]
  # with a length depending on how many Zones are available
  ci_runner_public_subnets_CIDR = [for zone_name in local.azs : cidrsubnet(var.main_network_block, var.public_subnet_prefix_extension, index(local.azs, zone_name) + var.public_zone_offset)]
}
