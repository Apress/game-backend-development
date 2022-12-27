variable "api_management_name" {
  description = "Name of the API Management service"
}

variable "shared_resource_group_name" {
  description = "Shared resource group name"
}

resource "azurerm_api_management_api" "economy" {
  name                  = "economy-api"
  resource_group_name   = var.shared_resource_group_name
  api_management_name   = var.api_management_name
  revision              = "1"
  path                  = "economy"
  display_name          = "Economy API"
  protocols             = ["https"]
  subscription_required = false
}

resource "azurerm_api_management_api_operation" "economy-post-catalog" {
  operation_id        = "economy-post-catalog"
  api_name            = azurerm_api_management_api.economy.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "POST Catalog"
  method              = "POST"
  url_template        = "/Catalog"
}

resource "azurerm_api_management_api_operation" "economy-get-catalog" {
  operation_id        = "economy-get-catalog"
  api_name            = azurerm_api_management_api.economy.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "GET Catalog"
  method              = "GET"
  url_template        = "/Catalog"
}

resource "azurerm_api_management_api_operation" "economy-delete-catalog" {
  operation_id        = "economy-delete-catalog"
  api_name            = azurerm_api_management_api.economy.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "DELETE Catalog"
  method              = "DELETE"
  url_template        = "/Catalog"
}

resource "azurerm_api_management_api_operation" "economy-post-inventory" {
  operation_id        = "economy-post-inventory"
  api_name            = azurerm_api_management_api.economy.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "POST Inventory"
  method              = "POST"
  url_template        = "/Inventory"
}

resource "azurerm_api_management_api_operation" "economy-get-inventory" {
  operation_id        = "economy-get-inventory"
  api_name            = azurerm_api_management_api.economy.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "GET Inventory"
  method              = "GET"
  url_template        = "/Inventory"
}

resource "azurerm_api_management_api_operation" "economy-post-virtualcurrency" {
  operation_id        = "economy-post-virtualcurrency"
  api_name            = azurerm_api_management_api.economy.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "POST VirtualCurrency"
  method              = "POST"
  url_template        = "/VirtualCurrency"
}

resource "azurerm_api_management_api_operation" "economy-get-virtualcurrency" {
  operation_id        = "economy-get-virtualcurrency"
  api_name            = azurerm_api_management_api.economy.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "GET VirtualCurrency"
  method              = "GET"
  url_template        = "/VirtualCurrency"
}


resource "azurerm_api_management_backend" "economy" {
  name                = "economy-azure-functions-backend"
  resource_group_name = var.shared_resource_group_name
  api_management_name = var.api_management_name
  protocol            = "http"
  url                 = "https://${azurerm_function_app.economy.default_hostname}/api/"
  resource_id         = "https://management.azure.com/${azurerm_function_app.economy.id}"
  credentials {
    header = {
      x-functions-key = "${data.azurerm_function_app_host_keys.economy.default_function_key}"
    }
  }
}

resource "azurerm_api_management_api_policy" "economy" {
  api_name            = azurerm_api_management_api.economy.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name

  xml_content = <<XML
<policies>
  <inbound>
    <set-backend-service id="apim-policy" backend-id="economy-azure-functions-backend" />
    <validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Unauthorized. Access token is missing or invalid.">
        <openid-config url="https://gamebackend2022.b2clogin.com/gamebackend2022.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_SignInSignUp" />
        <audiences>
            <audience>e4d1e07d-81ff-45b8-b849-91bef09b5bbd</audience>
        </audiences>
        <issuers>
            <issuer>https://gamebackend2022.b2clogin.com/fe46bdde-805c-4a42-83c6-871e0f4278e7/v2.0/</issuer>
        </issuers>
    </validate-jwt>
  </inbound>
</policies>
XML

}
