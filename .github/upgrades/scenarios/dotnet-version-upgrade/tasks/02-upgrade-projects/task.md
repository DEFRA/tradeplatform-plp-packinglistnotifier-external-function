# 02-upgrade-projects: Upgrade all projects to net10.0

Update all four projects simultaneously:

- Change `<TargetFramework>` from `net8.0` to `net10.0` in all `.csproj` files
- Replace `Microsoft.NET.Sdk.Functions` (4.4.1) with `Microsoft.Azure.Functions.Worker.Sdk` (2.0.7), `Microsoft.Azure.Functions.Worker` (2.52.0), and `Microsoft.Azure.Functions.Worker.Extensions.Http` (3.3.0) in the main function project
- Remove `Microsoft.Azure.WebJobs.Extensions.ServiceBus` (already included in framework reference)
- Enable Azure Functions V2 model and Application Insights as required by `AzureFunctionsUpgrade.0002`
- Address binary-incompatible APIs (`Api.0001`) in the main project
- Address source-incompatible APIs (`Api.0002`) and behavioral changes (`Api.0003`) in the test project
- Replace deprecated `xunit` (2.9.0) in both test projects with a non-deprecated alternative

**Done when**: All `.csproj` files target `net10.0`; solution restores and builds with 0 errors; no deprecated or incompatible packages remain.
