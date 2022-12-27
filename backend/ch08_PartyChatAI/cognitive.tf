resource "azurerm_cognitive_account" "chat" {
  name                = "cognitive-account"
  location            = azurerm_resource_group.chat.location
  resource_group_name = azurerm_resource_group.chat.name
  kind                = "TextTranslation"

  sku_name = "F0"
}

