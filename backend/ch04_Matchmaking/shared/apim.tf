output "api_management_name" {
  value = azurerm_api_management.shared.name
}

resource "azurerm_api_management" "shared" {
  name                = "shared-apim"
  location            = azurerm_resource_group.shared.location
  resource_group_name = azurerm_resource_group.shared.name
  publisher_name      = "gamebackend.dev"
  publisher_email     = "mail@gamebackend.dev"
  sku_name            = "Consumption_0"
}

