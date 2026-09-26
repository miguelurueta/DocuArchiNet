# Inventario técnico y trazabilidad

## API

| Tipo | Ruta | Clase | Nombre exacto | Entrada | Retorno | Seguridad/relación |
|---|---|---|---|---|---|---|
| WebMethod | `webservice/WebServiceImportarServicioWebModern.asmx.vb` | `WebServiceImportarServicioWebModern` | `PreflightImport` | `PreflightImportRequestDto` | `PreflightImportResponseDto` | Gate, sesión y contexto servidor |
| WebMethod | misma | misma | `CreateImportIntent` | `CreateImportIntentRequestDto` | `CreateImportIntentResponseDto` | Referencias SII sobrescritas por servidor |
| WebMethod | misma | misma | `ExecuteImportIntent` | `ExecuteImportIntentRequestDto` | `ExecuteImportIntentResponseDto` | Guard, versión y revalidación |
| WebMethod | misma | misma | `GetImportIntent` | `GetImportIntentRequestDto` | `GetImportIntentResponseDto` | Contexto e intención autorizados |
| WebMethod | misma | misma | `ReconcileImportIntent` | `ReconcileImportIntentRequestDto` | `ReconcileImportIntentResponseDto` | Filtro opcional por `ExternalKey` |

## Servicios, puertos y repositorios

| Tipo | Ruta | Clase/interfaz | Método | Responsabilidad |
|---|---|---|---|---|
| Servicio | `Services/.../ServicioPreflightImportacion.vb` | `ServicioPreflightImportacion` | `Preflight` | Selección, catálogo, duplicados y bloqueo de documento vigente |
| Servicio | `Services/.../ServicioIntencionImportacion.vb` | `ServicioIntencionImportacion` | `Crear`, `Canonical` | Intención única y huella |
| Servicio | `Services/.../ImportServiceOrchestrator.vb` | `ImportServiceOrchestrator` | `Execute` | Guard, transición y pasos por elemento |
| Paso | `Services/.../ImportExecutionSteps.vb` | `DownloadImportExecutionStep` | `Ejecutar` | Descarga SII moderna |
| Paso | misma | `StoreImportExecutionStep` | `Ejecutar` | Construye comando y llama puerto de almacenamiento |
| Paso | misma | `VerifyStoredImportExecutionStep` | `Ejecutar` | Verificación lógica/física |
| Adaptador | `Infrastructure/.../Storage/LegacyEnlaseImportDocumentStorageAdapter.vb` | `LegacyEnlaseImportDocumentStorageAdapter` | `Almacenar` | Única llamada legacy ENLASE |
| Proveedor | `Infrastructure/.../Sii/SiiImportProvider.vb` | `SiiImportProvider` | `DownloadAsync` | Bifurca por capacidad |
| Cliente | `Infrastructure/.../Sii/SiiExternalImportProviderClient.vb` | `SiiExternalImportProviderClient` | `DownloadAnnexAsync` | Reconsulta, resuelve y descarga segura |
| Repositorio | `Infrastructure/Repositories/.../MySqlImportIntentRepository.vb` | `MySqlImportIntentRepository` | interfaces de intención | Contexto, capacidad, identidad, tipología y estados |
| Repositorio | `Infrastructure/Repositories/.../MySqlImportItemStatusRepository.vb` | `MySqlImportItemStatusRepository` | `ObtenerLote` | Bloqueo de duplicado válido y recuperación de huérfano |
| Repositorio | `Infrastructure/Repositories/.../MySqlImportReconciliationRepository.vb` | `MySqlImportReconciliationRepository` | `Obtener`, `ObtenerItem` | Snapshot autorizado y evidencia física |
| Servicio | `Services/.../ServicioReconciliacionImportacion.vb` | `ServicioReconciliacionImportacion` | `ReconcileImportIntent` | Proyección y agregado seguro |

`Services/...` significa `Services/Workflow/ImportarServicioWeb`; `Infrastructure/...` significa `Infrastructure/Workflow/ImportarServicioWeb`; `Infrastructure/Repositories/...` significa `Infrastructure/Repositories/Workflow/ImportarServicioWeb`.

## Persistencia moderna

Las migraciones `Sql/001-add-intent-capability.sql` y `Sql/002-rollback-intent-capability.sql` agregan/revierten `capability` y `provider_reference` en el esquema moderno. La identidad por elemento se conserva en las tablas de intención ya existentes. La verificación lee `workflow_import_intent`, `workflow_import_item`, `logdocuarchi` y el gabinete documental; no altera datos durante reconciliación.

## Reutilización

- Directa: preflight, intención, guard, máquina de estados, provider registry, transporte HTTP seguro, repositorios modernos y mapper de resultados.
- Extendida: DTO/modelo con capacidad/referencia/evidencia; repositorio de intención; reconciliación física; pipeline con paso `VerifyStoredImportExecutionStep`.
- Adaptada: `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII`, con parámetro opcional `RutaArchivoPreparada` para evitar una segunda descarga.
- Excluida: asignación/cierre de tarea, coordinadores de expediente y efectos de constancias.

## Diagramas

| ID | Archivo | Vista |
|---|---|---|
| DOC81-D01 | `Diagramas/01-componentes.puml` | Componentes y dependencias |
| DOC81-D02 | `Diagramas/02-ejecucion-secuencia.puml` | Ejecución completa y alternativas |
| DOC81-D03 | `Diagramas/03-estados.puml` | Máquina de estados |
| DOC81-D04 | `Diagramas/04-reconciliacion-actividad.puml` | Decisión lógica/física |