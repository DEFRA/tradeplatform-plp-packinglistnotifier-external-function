# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade all 4 projects from net8.0 to net10.0
**Scope**: Small solution — 2 source projects + 2 test projects, Azure Functions isolated worker

### Selected Strategy
**All-At-Once** — All projects upgraded simultaneously in a single operation.
**Rationale**: 4 projects, all on net8.0, clear dependency structure. Issues are predominantly TFM bumps, package updates, and known API fixes.

---

## Tasks

### 01-prerequisites: Validate SDK and environment

Verify that the .NET 10 SDK is installed and any `global.json` is compatible with the upgrade target before making changes.

**Done when**: .NET 10 SDK confirmed present; `global.json` (if present) does not pin a conflicting SDK version.

---

### 02-upgrade-projects: Upgrade all projects to net10.0

Update all four projects simultaneously:

- Change `<TargetFramework>` from `net8.0` to `net10.0` in all `.csproj` files
- Replace `Microsoft.NET.Sdk.Functions` (4.4.1) with `Microsoft.Azure.Functions.Worker.Sdk` (2.0.7), `Microsoft.Azure.Functions.Worker` (2.52.0), and `Microsoft.Azure.Functions.Worker.Extensions.Http` (3.3.0) in the main function project
- Remove `Microsoft.Azure.WebJobs.Extensions.ServiceBus` (already included in framework reference)
- Enable Azure Functions V2 model and Application Insights as required by `AzureFunctionsUpgrade.0002`
- Address binary-incompatible APIs (`Api.0001`) in the main project
- Address source-incompatible APIs (`Api.0002`) and behavioral changes (`Api.0003`) in the test project
- Replace deprecated `xunit` (2.9.0) in both test projects with a non-deprecated alternative

**Done when**: All `.csproj` files target `net10.0`; solution restores and builds with 0 errors; no deprecated or incompatible packages remain.

---

### 03-tests: Run tests and resolve failures

Execute the full test suite after the upgrade completes. Investigate and fix any test failures introduced by behavioral changes (`Api.0003`) or API incompatibilities addressed in the prior task.

**Done when**: All tests pass with 0 failures.

---

### 04-commit: Commit the upgrade

Stage and commit all changes to the `net10upgrade` branch with a descriptive message summarising the upgrade.

**Done when**: All changes committed on `net10upgrade` branch.
