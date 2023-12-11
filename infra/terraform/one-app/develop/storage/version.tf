terraform {
  required_version = "~> 1.5"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = ">= 5.0.0"
    }
    mongodbatlas = {
      source  = "mongodb/mongodbatlas"
      version = ">= 1.11.0"
    }
  }
  backend "s3" {
    bucket = "xrspace-terraform-backend"
    key    = "one-app/develop/storage/terraform.tfstate"
    region = "ap-southeast-1"
  }
}
