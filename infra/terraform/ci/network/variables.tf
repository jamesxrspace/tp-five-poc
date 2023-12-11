variable "main_network_block" {
  type        = string
  description = "Base CIDR block to be used in our VPC."
}

variable "public_subnet_prefix_extension" {
  type        = number
  description = "CIDR block bits extension to calculate CIDR blocks of each public subnetwork."
}

variable "private_subnet_prefix_extension" {
  type        = number
  description = "CIDR block bits extension to calculate CIDR blocks of each private subnetwork."
}

variable "public_zone_offset" {
  type        = number
  description = "CIDR block bits extension offset to calculate EKS public subnets, avoiding collisions with existing subnets."
}

variable "private_zone_offset" {
  type        = number
  description = "CIDR block bits extension offset to calculate EKS private subnets, avoiding collisions with existing subnets."
}
variable "aws_region" {
  type = string
}

variable "number_of_azs" {
  type = number
}

variable "service" {
  type = string
}
