data "azurerm_resources" "agones" {
  resource_group_name = azurerm_kubernetes_cluster.agones.node_resource_group
  type                = "Microsoft.Network/networkSecurityGroups"
}

resource "azurerm_network_security_rule" "gameserver" {
  name                        = "gameserver"
  priority                    = 100
  direction                   = "Inbound"
  access                      = "Allow"
  protocol                    = "Udp"
  source_port_range           = "*"
  destination_port_range      = "7000-8000"
  source_address_prefix       = "*"
  destination_address_prefix  = "*"
  resource_group_name         = azurerm_kubernetes_cluster.agones.node_resource_group
  network_security_group_name = data.azurerm_resources.agones.resources.0.name
  # network_security_group_name = "aks-agentpool-55978144-nsg"
}







