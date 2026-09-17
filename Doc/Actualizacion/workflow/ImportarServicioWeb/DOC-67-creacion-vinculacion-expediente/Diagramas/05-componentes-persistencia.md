# Componentes y persistencia

Fuentes: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`, `Infrastructure/Repositories/Workflow/ImportarServicioWeb/`.

Referencias CODE: `IImportIntentRepository`; `MySqlImportIntentRepository`; `IImportExpedientConfigurationRepository`; `MySqlImportExpedientConfigurationRepository`; `IImportReconciliationRepository`; `MySqlImportReconciliationRepository`.

```mermaid
classDiagram
  class IImportIntentRepository
  class MySqlImportIntentRepository
  class IImportExpedientConfigurationRepository
  class MySqlImportExpedientConfigurationRepository
  class IImportReconciliationRepository
  class MySqlImportReconciliationRepository
  IImportIntentRepository <|.. MySqlImportIntentRepository
  IImportExpedientConfigurationRepository <|.. MySqlImportExpedientConfigurationRepository
  IImportReconciliationRepository <|.. MySqlImportReconciliationRepository
  MySqlImportIntentRepository --> EXT_MySQL_Workflow
  MySqlImportReconciliationRepository --> EXT_MySQL_DocuArchi
```
