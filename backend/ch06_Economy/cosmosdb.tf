resource "azurerm_cosmosdb_account" "economy" {
  name = "mygame-cosmos-db"
  # location            = azurerm_resource_group.economy.location
  location            = "eastus"
  resource_group_name = azurerm_resource_group.economy.name
  offer_type          = "Standard"
  kind                = "MongoDB"
  enable_free_tier    = true

  capabilities {
    name = "MongoDBv3.4"
  }

  capabilities {
    name = "EnableMongo"
  }

  consistency_policy {
    consistency_level = "Eventual"
  }

  geo_location {
    # location          = azurerm_resource_group.economy.location
    location          = "eastus"
    failover_priority = 0
  }
}


resource "azurerm_cosmosdb_mongo_database" "economy" {
  name                = "mygame-cosmos-mongo-db"
  resource_group_name = azurerm_resource_group.economy.name
  account_name        = azurerm_cosmosdb_account.economy.name
}
