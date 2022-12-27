resource "azurerm_eventhub_namespace" "gameanalytics" {
  name                = "GameAnalyticsEventHubNamespace"
  location            = azurerm_resource_group.gameanalytics.location
  resource_group_name = azurerm_resource_group.gameanalytics.name
  sku                 = "Standard"
  capacity            = 1
}

resource "azurerm_eventhub" "gameanalytics" {
  name                = "GameAnalyticsEventHub"
  namespace_name      = azurerm_eventhub_namespace.gameanalytics.name
  resource_group_name = azurerm_resource_group.gameanalytics.name
  partition_count     = 2
  message_retention   = 1

  capture_description {
    enabled             = true
    encoding            = "Avro"
    skip_empty_archives = true
    interval_in_seconds = 60

    destination {
      name                = "EventHubArchive.AzureBlockBlob"
      archive_name_format = "{Namespace}/{EventHub}/{PartitionId}/{Year}/{Month}/{Day}/{Hour}/{Minute}/{Second}"
      blob_container_name = azurerm_storage_container.gameanalytics.name
      storage_account_id  = azurerm_storage_account.gameanalytics.id
    }
  }
}

resource "azurerm_eventhub_authorization_rule" "gameanalytics" {
  name                = "MyAuthRule"
  namespace_name      = azurerm_eventhub_namespace.gameanalytics.name
  eventhub_name       = azurerm_eventhub.gameanalytics.name
  resource_group_name = azurerm_resource_group.gameanalytics.name
  listen              = false
  send                = true
  manage              = false
}
