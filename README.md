# tradeplatform-plp-packinglistnotifier-external-function

> a.k.a `Defra.Trade.Events.IDCOMS.PLNotifier`

# Setup

To run this function, you will need a `.\src\Defra.Trade.Events.IDCOMS.PLNotifier\local.settings.json` file. The file will need the following settings:

```jsonc 
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ServiceBus:ConnectionString": "<secret>", 
    "ConfigurationServer:ConnectionString": "<secret>",
    "ConfigurationServer:TenantId": "<secret>"
  }
}
```

Secrets reference can be found here: https://dev.azure.com/defragovuk/DEFRA-TRADE-APIS/_wiki/wikis/DEFRA-TRADE-APIS.wiki/26086
