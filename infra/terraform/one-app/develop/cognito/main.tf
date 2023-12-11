locals {
  envs = ["dev"]
}

module "cognito" {
  for_each = toset(local.envs)

  source      = "../../modules/cognito"
  environment = each.key
}
