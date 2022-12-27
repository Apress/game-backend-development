variable "app_service_plan_id" {
  description = "AppService Plan ID"
}

variable "storage_account_name" {
  description = "Storage account name"
}

variable "storage_account_access_key" {
  description = "Storage account access key"
}

resource "azurerm_function_app" "cloudscript" {
  name                       = "cloudscript-azure-functions"
  location                   = azurerm_resource_group.cloudscript.location
  resource_group_name        = azurerm_resource_group.cloudscript.name
  app_service_plan_id        = var.app_service_plan_id
  storage_account_name       = var.storage_account_name
  storage_account_access_key = var.storage_account_access_key
  version                    = "~4"
}
