variable "aws_region" {
  type = string
}

variable "service" {
  type = string
}

variable "state_backend_bucket_name" {
  type = string
}

variable "network_state_backend_key" {
  type = string
}

variable "eks_state_backend_key" {
  type = string
}

variable "storage_state_backend_key" {
  type = string
}

variable "edge_server_image_tag" {
  type = string
}

variable "edge_server_model_data_s3_url" {
  type = string
}

variable "edge_server_endpoint_name" {
  type = string
}

variable "edge_server_variant_name" {
  type = string
}

variable "instance_type" {
  type    = string
  default = "ml.g4dn.xlarge"
}

variable "sns_backend_subscribe_path" {
  type    = string
  default = "https://backend.dev.xrspace.io/api/v1/aigc/sns_endpoint/motion"
}

variable "kms_state_backend_bucket_name" {
  type = string
}

variable "kms_state_backend_key" {
  type = string
}
