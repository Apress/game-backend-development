resource "azurerm_postgresql_flexible_server" "shared" {
  name                   = "sharedpostgresqlserver"
  resource_group_name    = azurerm_resource_group.shared.name
  location               = azurerm_resource_group.shared.location
  version                = "12"
  administrator_login    = "sharedadmin"
  administrator_password = ""
  sku_name               = "B_Standard_B1ms"
  storage_mb             = 32768
  zone                   = 2

}

resource "azurerm_postgresql_flexible_server_database" "shared" {
  name      = "shared-postgresql-database"
  server_id = azurerm_postgresql_flexible_server.shared.id
}
