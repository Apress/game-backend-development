variable "api_management_name" {
  description = "Name of the API Management service"
}

variable "shared_resource_group_name" {
  description = "Shared resource group name"
}

resource "azurerm_api_management_api" "leaderboard" {
  name                  = "leaderboard-api"
  resource_group_name   = var.shared_resource_group_name
  api_management_name   = var.api_management_name
  revision              = "1"
  path                  = "leaderboard"
  display_name          = "Leaderboard API"
  protocols             = ["https"]
  subscription_required = false
}

resource "azurerm_api_management_api_operation" "leaderboard-post" {
  operation_id        = "leaderboard-post"
  api_name            = azurerm_api_management_api.leaderboard.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "POST Leaderboard"
  method              = "POST"
  url_template        = "/Leaderboard"
}

resource "azurerm_api_management_api_operation" "leaderboard-get" {
  operation_id        = "leaderboard-get-ticket"
  api_name            = azurerm_api_management_api.leaderboard.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "GET Leaderboard"
  method              = "GET"
  url_template        = "/Leaderboard"
}


resource "azurerm_api_management_backend" "leaderboard" {
  name                = "leaderboard-azure-functions-backend"
  resource_group_name = var.shared_resource_group_name
  api_management_name = var.api_management_name
  protocol            = "http"
  url                 = "https://${azurerm_function_app.leaderboard.default_hostname}/api/"
  resource_id         = "https://management.azure.com/${azurerm_function_app.leaderboard.id}"
  credentials {
    header = {
      x-functions-key = "${data.azurerm_function_app_host_keys.leaderboard.default_function_key}"
    }
  }
}

resource "azurerm_api_management_api_policy" "leaderboard" {
  api_name            = azurerm_api_management_api.leaderboard.name
  api_management_name = azurerm_api_management_api.leaderboard.api_management_name
  resource_group_name = azurerm_api_management_api.leaderboard.resource_group_name

  xml_content = <<XML
<policies>
  <inbound>
    <set-backend-service id="apim-policy" backend-id="leaderboard-azure-functions-backend" />

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
  depends_on = [
    azurerm_api_management_backend.leaderboard
  ]
}

