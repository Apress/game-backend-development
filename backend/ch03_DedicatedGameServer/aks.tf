resource "azurerm_kubernetes_cluster" "agones" {
  name                = "agones-cluster"
  location            = azurerm_resource_group.agones.location
  resource_group_name = azurerm_resource_group.agones.name
  dns_prefix          = "agones"

  default_node_pool {
    name                  = "default"
    node_count            = 1
    vm_size               = "Standard_B2s"
    enable_node_public_ip = true
  }

  service_principal {
    client_id     = azuread_application.agones.application_id
    client_secret = azuread_service_principal_password.agones.value
  }

  depends_on = [
    azuread_application.agones,
    azuread_service_principal.agones,
    azuread_service_principal_password.agones
  ]
}
