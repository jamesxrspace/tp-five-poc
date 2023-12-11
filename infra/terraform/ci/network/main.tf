############################################
# VPC
############################################
resource "aws_vpc" "vpc" {
  cidr_block = var.main_network_block
  # Enable DNS hostnames in the VPC.
  enable_dns_hostnames = true
  # Enable DNS support in the VPC.
  enable_dns_support = true
  instance_tenancy   = "default"

  tags = {
    Service = var.service
    Name    = format("%s-vpc", var.service)
  }
}

############################################
# Internet Gateway
############################################
resource "aws_internet_gateway" "internet_gateway" {
  vpc_id = aws_vpc.vpc.id

  tags = {
    Service = var.service
    Name    = format("%s-igw", var.service)
  }
}

############################################
# Public Route Table
############################################
resource "aws_route_table" "public_rtb" {
  vpc_id = aws_vpc.vpc.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.internet_gateway.id
  }

  tags = {
    Service = var.service
    Name    = format("%s-public-rtb", var.service)
  }
}

############################################
# CI Runner Subnet
############################################
resource "aws_subnet" "ci_runner_private_subnet" {
  count = length(local.azs)

  vpc_id                          = aws_vpc.vpc.id
  cidr_block                      = element(flatten(local.ci_runner_private_subnets_CIDR), count.index)
  availability_zone               = element(local.azs, count.index)
  map_public_ip_on_launch         = false
  assign_ipv6_address_on_creation = false

  tags = {
    Service = var.service
    Name    = format("%s-private-subnet-${element(local.azs, count.index)}", var.service)
  }
}

resource "aws_subnet" "ci_runner_public_subnet" {
  count = length(local.azs)

  vpc_id                          = aws_vpc.vpc.id
  cidr_block                      = element(flatten(local.ci_runner_public_subnets_CIDR), count.index)
  availability_zone               = element(local.azs, count.index)
  map_public_ip_on_launch         = true
  assign_ipv6_address_on_creation = false

  tags = {
    Service = var.service
    Name    = format("%s-public-subnet-${element(local.azs, count.index)}", var.service)
  }
}

############################################
# CI Runner NAT Gateway Elastic IP
############################################
resource "aws_eip" "ci_runner_nat_eip" {
  depends_on = [aws_internet_gateway.internet_gateway]
}

############################################
# CI Runner NAT Gateway
############################################
resource "aws_nat_gateway" "ci_runner_nat_gateway" {
  allocation_id = aws_eip.ci_runner_nat_eip.id
  subnet_id     = element(aws_subnet.ci_runner_public_subnet.*.id, 0)

  tags = {
    Service = var.service
    Name    = format("%s-nat-gateway-${element(local.azs, 0)}", var.service)
  }
}

############################################
# CI Runner Private Route Table
############################################
resource "aws_route_table" "ci_runner_private_rtb" {
  vpc_id = aws_vpc.vpc.id

  route {
    cidr_block     = "0.0.0.0/0"
    nat_gateway_id = aws_nat_gateway.ci_runner_nat_gateway.id
  }

  tags = {
    Service = var.service
    Name    = format("%s-private-rtb-${element(local.azs, 0)}", var.service)
  }
}

############################################
# CI Runner Public Route Table Association
############################################
resource "aws_route_table_association" "ci_runner_public_rts" {
  count = length(local.azs)

  subnet_id = element(aws_subnet.ci_runner_public_subnet.*.id, count.index)

  route_table_id = aws_route_table.public_rtb.id
}

############################################
# CI Runner Private Route Table Association
############################################
resource "aws_route_table_association" "ci_runner_private_rts" {
  count = length(local.azs)

  subnet_id = element(aws_subnet.ci_runner_private_subnet.*.id, count.index)

  route_table_id = aws_route_table.ci_runner_private_rtb.id
}

############################################
# CI Runner VPC Endpoint for S3
############################################
resource "aws_vpc_endpoint" "s3_endpoint" {
  vpc_id       = aws_vpc.vpc.id
  service_name = "com.amazonaws.${var.aws_region}.s3"

  route_table_ids = [aws_route_table.ci_runner_private_rtb.id]

  tags = {
    Service = var.service
    Name    = format("%s-s3-endpoint", var.service)
  }
}
