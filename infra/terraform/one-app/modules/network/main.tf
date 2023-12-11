############################################
# VPC
############################################
resource "aws_vpc" "vpc" {
  cidr_block = var.main_network_block
  # Required for EKS. Enable DNS hostnames in the VPC.
  enable_dns_hostnames = true
  # Required for EKS. Enable DNS support in the VPC.
  enable_dns_support = true
  instance_tenancy   = "default"

  tags = {
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-vpc", local.eks_cluster_name)
  }
}

############################################
# Internet Gateway
############################################
resource "aws_internet_gateway" "internet_gateway" {
  vpc_id = aws_vpc.vpc.id

  tags = {
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-igw", local.eks_cluster_name)
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
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-public-rtb", local.eks_cluster_name)
  }
}

############################################
# EKS Subnet
############################################
resource "aws_subnet" "eks_private_subnet" {
  count = length(local.azs)

  vpc_id                          = aws_vpc.vpc.id
  cidr_block                      = element(flatten(local.eks_private_subnets_CIDR), count.index)
  availability_zone               = element(local.azs, count.index)
  map_public_ip_on_launch         = false
  assign_ipv6_address_on_creation = false

  tags = {
    Service                                           = var.service
    Environment                                       = var.environment
    Name                                              = format("%s-private-subnet-${element(local.azs, count.index)}", local.eks_cluster_name)
    "kubernetes.io/cluster/${local.eks_cluster_name}" = "shared"
    "kubernetes.io/role/internal-elb"                 = "1"
  }
}

resource "aws_subnet" "eks_public_subnet" {
  count = length(local.azs)

  vpc_id                          = aws_vpc.vpc.id
  cidr_block                      = element(flatten(local.eks_public_subnets_CIDR), count.index)
  availability_zone               = element(local.azs, count.index)
  map_public_ip_on_launch         = true
  assign_ipv6_address_on_creation = false

  tags = {
    Service                                           = var.service
    Environment                                       = var.environment
    Name                                              = format("%s-public-subnet-${element(local.azs, count.index)}", local.eks_cluster_name)
    "kubernetes.io/cluster/${local.eks_cluster_name}" = "shared"
    "kubernetes.io/role/elb"                          = "1"
  }
}

############################################
# EKS NAT Gateway Elastic IP
############################################
resource "aws_eip" "eks_nat_eip" {
  count      = var.single_nat_gateway ? 1 : length(local.azs)
  depends_on = [aws_internet_gateway.internet_gateway]

  tags = {
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-nat-gateway-eip-${element(local.azs, count.index)}", local.eks_cluster_name)
  }
}

############################################
# EKS NAT Gateway
############################################
resource "aws_nat_gateway" "eks_nat_gateway" {
  count         = var.single_nat_gateway ? 1 : length(local.azs)
  allocation_id = element(aws_eip.eks_nat_eip.*.id, count.index)
  subnet_id     = element(aws_subnet.eks_public_subnet.*.id, count.index)

  tags = {
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-nat-gateway-${element(local.azs, count.index)}", local.eks_cluster_name)
  }
}

############################################
# EKS Private Route Table
############################################
resource "aws_route_table" "eks_private_rtb" {
  count  = var.single_nat_gateway ? 1 : length(local.azs)
  vpc_id = aws_vpc.vpc.id

  route {
    cidr_block     = "0.0.0.0/0"
    nat_gateway_id = element(aws_nat_gateway.eks_nat_gateway.*.id, count.index)
  }

  tags = {
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-private-rtb-${element(local.azs, count.index)}", local.eks_cluster_name)
  }
}

############################################
# EKS Public Route Table Association
############################################
resource "aws_route_table_association" "eks_public_rts" {
  count = length(local.azs)

  subnet_id = element(aws_subnet.eks_public_subnet.*.id, count.index)

  route_table_id = aws_route_table.public_rtb.id
}

############################################
# EKS Private Route Table Association
############################################
resource "aws_route_table_association" "eks_private_rts" {
  count = length(local.azs)

  subnet_id = element(aws_subnet.eks_private_subnet.*.id, count.index)

  route_table_id = element(aws_route_table.eks_private_rtb.*.id, var.single_nat_gateway ? 0 : count.index)
}

############################################
# EKS VPC Endpoint for S3
############################################
resource "aws_vpc_endpoint" "s3_endpoint" {
  vpc_id       = aws_vpc.vpc.id
  service_name = "com.amazonaws.${var.aws_region}.s3"

  route_table_ids = aws_route_table.eks_private_rtb.*.id

  tags = {
    Service     = var.service
    Environment = var.environment
    Name        = format("%s-s3-endpoint", local.eks_cluster_name)
  }
}
