# Saga de documentos relacionados

Fuentes: `Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb`, `Infrastructure/Workflow/ImportarServicioWeb/Expedients/DocumentExpedientRelationAdapter.vb`, `Infrastructure/Workflow/ImportarServicioWeb/Expedients/SiiDocumentIndexAdapter.vb`.

Referencias CODE: `ImportRelatedDocumentCoordinator.Procesar(ContextoImportacionServicio,IntencionImportacionServicio,PlanExpedienteImportacion,String,String):PlanDocumentosRelacionadosImportacion`; `DocumentExpedientRelationAdapter.Vincular(ContextoImportacionServicio,DocumentoRelacionadoImportacion):ResultadoEfectoExpedienteImportacion`; `SiiDocumentIndexAdapter.Actualizar(ContextoImportacionServicio,DocumentoRelacionadoImportacion,InscripcionImportacion):ResultadoEfectoExpedienteImportacion`; `SiiDocumentIndexAdapter.Verificar(ContextoImportacionServicio,DocumentoRelacionadoImportacion):EvidenciaIndiceElectronicoImportacion`.

```mermaid
sequenceDiagram
  participant R as CODE:ImportRelatedDocumentCoordinator
  participant V as CODE:DocumentExpedientRelationAdapter
  participant I as CODE:SiiDocumentIndexAdapter
  loop cada IdImagen único por ENLASE
    R->>V: Vincular(contexto, documento)
    alt relación conflictiva o no confirmada
      V-->>R: ResultadoEfectoExpedienteImportacion(Fallido)
    else relación confirmada
      R->>I: Actualizar(contexto, documento, inscripcion)
      R->>I: Verificar(contexto, documento)
      I-->>R: SQL y XML independientes
    end
  end
```
