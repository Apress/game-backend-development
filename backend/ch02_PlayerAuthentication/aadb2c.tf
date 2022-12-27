resource "azurerm_resource_provider_registration" "auth" {
  name = "Microsoft.AzureActiveDirectory"
}

resource "azurerm_aadb2c_directory" "auth" {
  country_code            = "DE"
  data_residency_location = "Europe"
  display_name            = "mygamedirectory"
  domain_name             = "mygamedirectory.onmicrosoft.com"
  resource_group_name     = azurerm_resource_group.auth.name
  sku_name                = "PremiumP1"
}


