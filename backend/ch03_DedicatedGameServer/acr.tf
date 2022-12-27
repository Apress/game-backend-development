resource "azurerm_container_registry" "agones" {
  name                = "agonesacr"
  resource_group_name = "agones-resources"
  location            = azurerm_resource_group.agones.location
  sku                 = "Basic"
  admin_enabled       = true
}

resource "azuread_application" "agones" {
  display_name = "agones"
}

resource "azuread_service_principal" "agones" {
  application_id = azuread_application.agones.application_id
}

resource "azuread_service_principal_password" "agones" {
  service_principal_id = azuread_service_principal.agones.id
}

resource "azurerm_role_assignment" "agones" {
  scope                = azurerm_container_registry.agones.id
  role_definition_name = "AcrPull"
  principal_id         = azuread_service_principal.agones.object_id
}
