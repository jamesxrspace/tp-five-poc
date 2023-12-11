resource "aws_s3_bucket" "s3_backend" {
  bucket = format("%s-s3", local.eks_cluster_name)

  tags = {
    Service     = var.service
    Environment = var.environment
  }
}

resource "aws_s3_bucket_public_access_block" "s3_backend" {
  bucket                  = aws_s3_bucket.s3_backend.id
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

resource "aws_cloudfront_origin_access_control" "backend_oac" {
  name                              = format("%s-s3-backend-oac", local.eks_cluster_name)
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

resource "aws_cloudfront_distribution" "backend_dist" {
  enabled = true
  origin {
    origin_id                = aws_s3_bucket.s3_backend.id
    domain_name              = aws_s3_bucket.s3_backend.bucket_regional_domain_name
    origin_access_control_id = aws_cloudfront_origin_access_control.backend_oac.id
  }
  default_cache_behavior {
    allowed_methods        = ["GET", "HEAD", "OPTIONS"]
    cached_methods         = ["GET", "HEAD", "OPTIONS"]
    target_origin_id       = aws_s3_bucket.s3_backend.id
    viewer_protocol_policy = "redirect-to-https" # other options - https only, http
    forwarded_values {
      headers      = []
      query_string = true
      cookies {
        forward = "all"
      }
    }
  }
  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }
  viewer_certificate {
    cloudfront_default_certificate = true
  }
}

data "aws_iam_policy_document" "s3_backend_iam_policy_document" {
  statement {
    actions   = ["s3:GetObject"]
    resources = ["${aws_s3_bucket.s3_backend.arn}/*"]
    principals {
      type        = "Service"
      identifiers = ["cloudfront.amazonaws.com"]
    }
    condition {
      test     = "StringEquals"
      variable = "aws:SourceArn"
      values   = [aws_cloudfront_distribution.backend_dist.arn]
    }
  }
}

resource "aws_s3_bucket_policy" "s3_backend_policy" {
  bucket = aws_s3_bucket.s3_backend.id
  policy = data.aws_iam_policy_document.s3_backend_iam_policy_document.json
}


resource "aws_s3_bucket" "s3_backend_tmp" {
  bucket = format("%s-s3-tmp", local.eks_cluster_name)

  tags = {
    Service     = var.service
    Environment = var.environment
  }
}

resource "aws_s3_bucket_lifecycle_configuration" "s3_tmp" {
  bucket = aws_s3_bucket.s3_backend_tmp.id

  rule {
    id     = "tmp"
    status = "Enabled"
    expiration {
      days = 1
    }
  }
}

resource "aws_s3_bucket" "s3_cms" {
  bucket = format("%s-s3-cms", local.eks_cluster_name)

  tags = {
    Service     = var.service
    Environment = var.environment
  }
}

resource "aws_s3_bucket_public_access_block" "s3_cms" {
  bucket                  = aws_s3_bucket.s3_cms.id
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

resource "aws_cloudfront_origin_access_control" "cms_oac" {
  name                              = format("%s-s3-cms-oac", local.eks_cluster_name)
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

resource "aws_cloudfront_distribution" "cms_dist" {
  enabled             = true
  aliases             = [var.cms_domain_name]
  default_root_object = "index.html"
  origin {
    origin_id                = aws_s3_bucket.s3_cms.id
    domain_name              = aws_s3_bucket.s3_cms.bucket_regional_domain_name
    origin_access_control_id = aws_cloudfront_origin_access_control.cms_oac.id
    origin_path              = "/static"
  }
  default_cache_behavior {
    allowed_methods        = ["GET", "HEAD", "OPTIONS", "PUT", "POST", "PATCH", "DELETE"]
    cached_methods         = ["GET", "HEAD", "OPTIONS"]
    target_origin_id       = aws_s3_bucket.s3_cms.id
    viewer_protocol_policy = "redirect-to-https" # other options - https only, http
    forwarded_values {
      headers      = []
      query_string = true
      cookies {
        forward = "all"
      }
    }
  }
  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }
  viewer_certificate {
    acm_certificate_arn      = var.cms_acm_arn
    ssl_support_method       = "sni-only"
    minimum_protocol_version = "TLSv1.2_2021"
  }
  custom_error_response {
    error_caching_min_ttl = 1
    error_code            = 403
    response_code         = 200
    response_page_path    = "/index.html"
  }
}

data "aws_iam_policy_document" "s3_cms_iam_policy_document" {
  statement {
    actions   = ["s3:GetObject"]
    resources = ["${aws_s3_bucket.s3_cms.arn}/static/*"]
    principals {
      type        = "Service"
      identifiers = ["cloudfront.amazonaws.com"]
    }
    condition {
      test     = "StringEquals"
      variable = "aws:SourceArn"
      values   = [aws_cloudfront_distribution.cms_dist.arn]
    }
  }
}

resource "aws_s3_bucket_policy" "s3_cms_policy" {
  bucket = aws_s3_bucket.s3_cms.id
  policy = data.aws_iam_policy_document.s3_cms_iam_policy_document.json
}
