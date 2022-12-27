
output "app_service_plan_id" {
  value = azurerm_app_service_plan.shared.id
}

output "storage_account_name" {
  value = azurerm_storage_account.shared.name
}

output "storage_account_access_key" {
  value = azurerm_storage_account.shared.primary_access_key
}

resource "azurerm_storage_account" "shared" {
  name                     = "sharedfunctionsappsa"
  resource_group_name      = azurerm_resource_group.shared.name
  location                 = azurerm_resource_group.shared.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "shared" {
  name                = "shared-azure-functions-service-plan"
  location            = azurerm_resource_group.shared.location
  resource_group_name = azurerm_resource_group.shared.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}
