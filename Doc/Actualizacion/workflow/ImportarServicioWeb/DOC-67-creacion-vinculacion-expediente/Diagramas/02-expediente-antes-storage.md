# Resolución de expediente

Fuentes: `Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb`, `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb`.

Referencias CODE: `ImportExpedientCoordinator.Resolver(ContextoImportacionServicio,IntencionImportacionServicio):PlanExpedienteImportacion`; `MySqlImportIntentRepository.PersistirPlanExpedientes(ContextoImportacionServicio,IntencionImportacionServicio,PlanExpedienteImportacion):Boolean`.

```mermaid
flowchart TD
  A[CODE:ImportExpedientCoordinator.Resolver] --> B{CONCEPT:¿identidad única?}
  B -- no --> E[CONCEPT:EXPEDIENT_IDENTITY_CONFLICT]
  B -- sí --> C{CONCEPT:¿expediente existe?}
  C -- sí --> D[CONCEPT:Reutilizar y verificar]
  C -- no --> F{CONCEPT:¿creación habilitada?}
  F -- no --> G[CONCEPT:EXPEDIENT_CREATION_DISABLED]
  F -- sí --> H[CONCEPT:Crear y postverificar]
  D --> P[CODE:MySqlImportIntentRepository.PersistirPlanExpedientes]
  H --> P
```
