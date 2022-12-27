output "shared_resource_group_name" {
  value = azurerm_resource_group.shared.name
}

resource "azurerm_resource_group" "shared" {
  name     = "shared-resources"
  location = "West Europe"
}