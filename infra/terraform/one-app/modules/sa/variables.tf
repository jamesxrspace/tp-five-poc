variable "eks_oidc_url" {
  type = string
}

variable "eks_oidc_arn" {
  type = string
}

variable "eks_cluster_name" {
  type = string
}

variable "s3_bucket_name" {
  type = string
}

variable "s3_tmp_bucket_name" {
  type = string
}

variable "s3_cms_bucket_name" {
  type = string
}

variable "namespaces" {
  type = list(string)
}
