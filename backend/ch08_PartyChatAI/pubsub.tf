resource "azurerm_web_pubsub" "chat" {
  name                = "chat-webpubsub"
  location            = azurerm_resource_group.chat.location
  resource_group_name = azurerm_resource_group.chat.name

  sku      = "Free_F1"
  capacity = 1

  public_network_access_enabled = true

  live_trace {
    enabled                   = true
    messaging_logs_enabled    = true
    connectivity_logs_enabled = true
  }
}

resource "azurerm_web_pubsub_hub" "chat" {
  name          = "chat"
  web_pubsub_id = azurerm_web_pubsub.chat.id
  event_handler {
    url_template       = "https://chat-azure-functions.azurewebsites.net/runtime/webhooks/webpubsub?code=X"
    user_event_pattern = "*"
  }

  anonymous_connections_enabled = true

  depends_on = [
    azurerm_web_pubsub.chat
  ]
}
