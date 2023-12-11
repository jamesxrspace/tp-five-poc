resource "aws_security_group" "sg_efs" {
  description = format("%s-efs-sg", local.eks_cluster_name)
  name        = format("%s-efs-sg", local.eks_cluster_name)
  vpc_id      = var.vpc_id

  ingress {
    from_port       = 2049
    to_port         = 2049
    protocol        = "tcp"
    security_groups = [var.eks_node_security_group_id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_efs_file_system" "efs" {
  creation_token = format("%s-efs", local.eks_cluster_name)

  tags = {
    Name        = format("%s-efs", local.eks_cluster_name)
    Service     = var.service
    Environment = var.environment
  }
}

resource "aws_efs_mount_target" "mount_target" {
  count = length(var.eks_private_subnets)

  file_system_id  = aws_efs_file_system.efs.id
  subnet_id       = element(var.eks_private_subnets[*], count.index)
  security_groups = [aws_security_group.sg_efs.id]
}
