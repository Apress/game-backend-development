variable "api_management_name" {
  description = "Name of the API Management service"
}

variable "shared_resource_group_name" {
  description = "Shared resource group name"
}

resource "azurerm_api_management_api" "chat" {
  name                  = "chat-api"
  resource_group_name   = var.shared_resource_group_name
  api_management_name   = var.api_management_name
  revision              = "1"
  path                  = "chat"
  display_name          = "chat API"
  protocols             = ["https"]
  subscription_required = false
}


resource "azurerm_api_management_api_operation" "chat-get" {
  operation_id        = "subscribe"
  api_name            = azurerm_api_management_api.chat.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "GET subscribe"
  method              = "GET"
  url_template        = "/Subscribe"
}


resource "azurerm_api_management_backend" "chat" {
  name                = "chat-azure-functions-backend"
  resource_group_name = var.shared_resource_group_name
  api_management_name = var.api_management_name
  protocol            = "http"
  url                 = "https://${azurerm_function_app.chat.default_hostname}/api/"
  resource_id         = "https://management.azure.com/${azurerm_function_app.chat.id}"
  credentials {
    header = {
      x-functions-key = "${data.azurerm_function_app_host_keys.chat.default_function_key}"
    }
  }
}


resource "azurerm_api_management_api_operation" "chat-translate" {
  operation_id        = "translate"
  api_name            = azurerm_api_management_api.chat.name
  api_management_name = var.api_management_name
  resource_group_name = var.shared_resource_group_name
  display_name        = "POST translate"
  method              = "POST"
  url_template        = "/Translate"
}


resource "azurerm_api_management_backend" "chat-translate" {
  name                = "translator-backend"
  resource_group_name = var.shared_resource_group_name
  api_management_name = var.api_management_name
  protocol            = "http"
  url                 = azurerm_cognitive_account.chat.endpoint
  credentials {
    header = {
      Ocp-Apim-Subscription-Key    = "${azurerm_cognitive_account.chat.primary_access_key}"
      Ocp-Apim-Subscription-Region = "westeurope"
      Content-Type                 = "application/json"
    }
    query = {
      "api-version" = "3.0"
    }
  }
}


resource "azurerm_api_management_api_operation_policy" "chat-translate" {
  api_name            = azurerm_api_management_api_operation.chat-translate.api_name
  api_management_name = azurerm_api_management_api_operation.chat-translate.api_management_name
  resource_group_name = azurerm_api_management_api_operation.chat-translate.resource_group_name
  operation_id        = azurerm_api_management_api_operation.chat-translate.operation_id

  xml_content = <<XML
<policies>
  <inbound>
    <set-backend-service id="apim-policy" backend-id="translator-backend" />
    <validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Unauthorized. Access token is missing or invalid.">
        <openid-config url="https://gamebackend2022.b2clogin.com/gamebackend2022.onmicrosoft.com/v2.0/.well-known/openid-configuration?p=B2C_1_SignInSignUp" />
        <audiences>
            <audience>e4d1e07d-81ff-45b8-b849-91bef09b5bbd</audience>
        </audiences>
        <issuers>
            <issuer>https://gamebackend2022.b2clogin.com/fe46bdde-805c-4a42-83c6-871e0f4278e7/v2.0/</issuer>
        </issuers>
    </validate-jwt>
    <set-header name="Authorization" exists-action="delete" />
  </inbound>
</policies>
XML

}

resource "azurerm_api_management_api_operation_policy" "chat-get" {
  api_name            = azurerm_api_management_api_operation.chat-get.api_name
  api_management_name = azurerm_api_management_api_operation.chat-get.api_management_name
  resource_group_name = azurerm_api_management_api_operation.chat-get.resource_group_name
  operation_id        = azurerm_api_management_api_operation.chat-get.operation_id

  xml_content = <<XML
<policies>
  <inbound>
    <set-backend-service id="apim-policy" backend-id="chat-azure-functions-backend" />
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

