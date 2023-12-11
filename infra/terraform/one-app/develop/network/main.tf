locals {
  aws_region = "ap-southeast-1"
}

module "network" {
  source                          = "../../modules/network"
  single_nat_gateway              = true
  environment                     = "develop"
  main_network_block              = "10.1.0.0/16"
  public_subnet_prefix_extension  = 8
  private_subnet_prefix_extension = 4
  public_zone_offset              = 176
  private_zone_offset             = 12
  aws_region                      = local.aws_region
  number_of_azs                   = 3
  service                         = "one-app"
}
