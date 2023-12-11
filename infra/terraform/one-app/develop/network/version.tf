terraform {
  required_version = "~> 1.5"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = ">= 5.0.0"
    }
  }
  backend "s3" {
    bucket = "xrspace-terraform-backend"
    key    = "one-app/develop/network/terraform.tfstate"
    region = "ap-southeast-1"
  }
}
