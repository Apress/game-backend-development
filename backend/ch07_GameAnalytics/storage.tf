resource "azurerm_storage_account" "gameanalytics" {
  name                     = "gameanalyticssa"
  resource_group_name      = azurerm_resource_group.gameanalytics.name
  location                 = azurerm_resource_group.gameanalytics.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_storage_container" "gameanalytics" {
  name                  = "eventhubarchive"
  storage_account_name  = azurerm_storage_account.gameanalytics.name
  container_access_type = "private"
}
