module "singlevm" {
  source = "./ch01_GettingStarted"
}

module "auth" {
  source = "./ch02_PlayerAuthentication"
}

module "agones" {
  source = "./ch03_DedicatedGameServer"
}

module "matchmaking" {
  source                     = "./ch04_Matchmaking"
  api_management_name        = module.shared.api_management_name
  shared_resource_group_name = module.shared.shared_resource_group_name
  app_service_plan_id        = module.shared.app_service_plan_id
  storage_account_name       = module.shared.storage_account_name
  storage_account_access_key = module.shared.storage_account_access_key
}

module "shared" {
  source = "./ch04_Matchmaking/shared"
}

module "leaderboard" {
  source                     = "./ch05_Leaderboards"
  api_management_name        = module.shared.api_management_name
  shared_resource_group_name = module.shared.shared_resource_group_name
  app_service_plan_id        = module.shared.app_service_plan_id
  storage_account_name       = module.shared.storage_account_name
  storage_account_access_key = module.shared.storage_account_access_key
}

module "economy" {
  source                     = "./ch06_Economy"
  api_management_name        = module.shared.api_management_name
  shared_resource_group_name = module.shared.shared_resource_group_name
  app_service_plan_id        = module.shared.app_service_plan_id
  storage_account_name       = module.shared.storage_account_name
  storage_account_access_key = module.shared.storage_account_access_key
}


module "gameanalytics" {
  source                     = "./ch07_GameAnalytics"
  api_management_name        = module.shared.api_management_name
  shared_resource_group_name = module.shared.shared_resource_group_name
  app_service_plan_id        = module.shared.app_service_plan_id
  storage_account_name       = module.shared.storage_account_name
  storage_account_access_key = module.shared.storage_account_access_key
}

module "chat" {
  source                     = "./ch08_PartyChatAI"
  api_management_name        = module.shared.api_management_name
  shared_resource_group_name = module.shared.shared_resource_group_name
  app_service_plan_id        = module.shared.app_service_plan_id
  storage_account_name       = module.shared.storage_account_name
  storage_account_access_key = module.shared.storage_account_access_key
}

module "cloudscript" {
  source                     = "./ch09_CloudScriptAzureFunctions"
  app_service_plan_id        = module.shared.app_service_plan_id
  storage_account_name       = module.shared.storage_account_name
  storage_account_access_key = module.shared.storage_account_access_key
}
