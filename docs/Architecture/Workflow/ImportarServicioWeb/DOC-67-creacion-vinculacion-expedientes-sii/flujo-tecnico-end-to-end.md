# Flujo técnico end-to-end — Importación SII y vinculación de expedientes

## Alcance

Este documento describe el recorrido productivo de `ExecuteImportIntent` en DOC-67, desde el cliente Workflow hasta la respuesta ASMX. Incluye las clases y métodos reales, validaciones de contexto, reglas de negocio, repositorios, funciones legacy conservadas y salidas de error.

> La frontera es ASMX, no REST. Los errores funcionales se devuelven normalmente dentro de `Error.Codigo` con HTTP `200 OK`. Un HTTP `500` queda reservado para fallos de transporte, serialización o infraestructura que escapen del manejo del método.

## Vista sencilla del flujo completo

```text
┌───────────────────────────────┐
│ Usuario selecciona una tarea │
│ e inicia Importar desde SII  │
└───────────────┬───────────────┘
                ▼
┌─────────────────────────────────────────┐
│ Validaciones iniciales                  │
│ • usuario autenticado y autorizado      │
│ • tarea seleccionada, asignada y activa │
│ • radicado y código de barras válidos   │
│ • configuración del trámite habilitada  │
└───────────────┬─────────────────────────┘
                ▼
┌─────────────────────────────────────────┐
│ Consulta SII                            │
│ • obtiene inscripciones y documentos    │
│ • normaliza la identidad de matrícula   │
│ • elimina letras y ceros a la izquierda │
└───────────────┬─────────────────────────┘
                ▼
┌─────────────────────────────────────────┐
│ Crea la intención persistente           │
│ • garantiza idempotencia                │
│ • registra items e inscripciones        │
└───────────────┬─────────────────────────┘
                ▼
       ┌─────────────────────────┐
       │ ¿Existe expediente por │
       │ identidad y gabinete?  │
       └────────────┬────────────┘
               SÍ  │  NO
          ┌────────┴──────────┐
          ▼                   ▼
┌──────────────────┐  ┌─────────────────────────┐
│ Reutiliza el     │  │ ¿Creación habilitada y │
│ expediente; no   │  │ datos suficientes?     │
│ crea duplicado.  │  └────────────┬────────────┘
└─────────┬────────┘           SÍ  │  NO
          │               ┌────────┴────────┐
          │               ▼                 ▼
          │       ┌───────────────┐  ┌────────────────┐
          │       │ Crea y       │  │ Falla cerrado: │
          │       │ verifica el  │  │ expediente no  │
          │       │ expediente.  │  │ resoluble.     │
          │       └───────┬──────┘  └────────────────┘
          └───────────────┘
                          ▼
┌─────────────────────────────────────────┐
│ Vincula el radicado actual              │
│ Un radicado anterior en caché no es     │
│ conflicto si el expediente coincide.    │
└───────────────┬─────────────────────────┘
                ▼
┌─────────────────────────────────────────┐
│ Incorpora y relaciona documentos        │
│ • almacena el documento                 │
│ • vincula documento y expediente        │
│ • conserva metadatos de incorporación   │
│ • en documentos relacionados actualiza  │
│   solo matrícula, NIT y razón social    │
│ • adapta tipo y longitud al gabinete    │
└───────────────┬─────────────────────────┘
                ▼
┌─────────────────────────────────────────┐
│ Confirma efectos físicos                │
│ almacenamiento + relación + índices +   │
│ caché + documentos relacionados         │
└───────────────┬─────────────────────────┘
                ▼
       ┌────────────────────────┐
       │ ¿Todos los efectos se │
       │ confirmaron?          │
       └────────────┬───────────┘
               SÍ  │  NO
          ┌────────┴──────────┐
          ▼                   ▼
┌──────────────────┐  ┌──────────────────────┐
│ Intención        │  │ Parcial, conflicto o│
│ completada       │  │ reconciliación      │
│                  │  │ requerida           │
└──────────────────┘  └──────────────────────┘
```

Regla principal: se resuelve primero por identidad normalizada y gabinete. Si
el expediente existe, se reutiliza y se vincula el nuevo radicado; solo se crea
cuando no existe y la configuración lo permite. Una operación no se considera
completa hasta confirmar sus efectos físicos.

## 1. Cliente → Controller

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ CLIENTE WORKFLOW                                                            │
│                                                                             │
│ POST /webservice/WebServiceImportarServicioWebModern.asmx/ExecuteImportIntent│
│ Content-Type: application/json                                              │
│                                                                             │
│ ExecuteImportIntentRequestDto:                                              │
│ • SchemaVersion                                                            │
│ • OperationId                                                              │
│ • CorrelationId                                                            │
│ • TaskId                                                                   │
│ • ProviderId = "SII"                                                       │
│ • IntentId                                                                 │
│ • VersionToken                                                             │
│ • StopRequested                                                            │
└──────────────────────────────────┬──────────────────────────────────────────┘
                                   │
                                   ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ CONTROLLER ASMX                                                             │
│                                                                             │
│ Clase: WebServiceImportarServicioWebModern                                  │
│ Método: ExecuteImportIntent(request)                                        │
│                                                                             │
│ 1. FeatureEnabled()                                                         │
│ 2. ValidRequest(request)                                                    │
│ 3. TryBuildImportContext(request, context, session)                         │
│ 4. Compose(session, ProviderId, context)                                    │
│ 5. composition.Orchestrator.Execute(context, request)                       │
│ 6. composition.Reconciliation.ProjectExecutionResult(context, execution)   │
└─────────────────────────────────────────────────────────────────────────────┘
```

`ValidRequest` exige:

- `request IsNot Nothing`.
- `TaskId > 0`.
- `OperationId` y `CorrelationId` no vacíos.
- `ProviderId` igual a `SII`, sin distinguir mayúsculas.

```text
                    ┌────────────────────────┐
                    │ ¿Gate moderno activo?  │
                    └───────────┬────────────┘
                         SÍ     │     NO
                 ┌──────────────┴──────────────┐
                 ▼                             ▼
           Validar request              HTTP 200
                                        Error.Codigo = FEATURE_DISABLED
                 │
                 ▼
                    ┌────────────────────────┐
                    │ ¿Request estructural?  │
                    └───────────┬────────────┘
                         SÍ     │     NO
                 ┌──────────────┴──────────────┐
                 ▼                             ▼
           Validar sesión               HTTP 200
                                        Error.Codigo = INVALID_REQUEST
```

## 2. Seguridad y contexto confiable

`TryBuildImportContext` toma la autoridad exclusivamente de `HttpContext.Session`:

- `Id_Usuario_Workflow`.
- `Id_Grupo_Workflow`.
- `Id_Ruta_Workflow`.
- `Login_Usuario_Workfow`.
- `ID_TAREA_SELECCIONDA`.
- `DG_ID_TRAMITE`; si falta, se resuelve mediante `Classselecciotarea.Solicita_id_tipo_tramite_tarea_workflow`.
- Conexiones Workflow, Docuarchi y Radicación suministradas por el contexto del servidor.

Reglas centrales:

```text
┌─────────────────────────────┐
│ TaskId enviado por cliente  │
└──────────────┬──────────────┘
               ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ Es una referencia, no autoridad. Debe coincidir exactamente con             │
│ Session("ID_TAREA_SELECCIONDA").                                            │
└──────────────┬──────────────────────────────────────────────────────────────┘
               ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ WorkflowPreviewSessionContextGate.AsegurarContexto() calcula usuario, grupo,│
│ login, ruta y conexiones autorizadas.                                       │
└──────────────┬──────────────────────────────────────────────────────────────┘
               │
       ┌───────┴────────┐
       ▼                ▼
  Coincide          No coincide
       │                │
       ▼                ▼
  Continuar         TASK_CONTEXT_MISMATCH / SESSION_* / FORBIDDEN
```

Los adaptadores legacy vuelven a comprobar que la tarea y la ruta de sesión coincidan con `ContextoImportacionServicio`. El cliente nunca controla usuario, grupo, trámite, gabinete, expediente, conexiones o permisos.

## 3. Composición productiva

`WebServiceImportarServicioWebModern.Compose` crea:

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ CONEXIONES                                                                  │
│                                                                             │
│ WorkflowModuleConnectionFactory  → intención, plan y estados                │
│ DocuarchiModuleConnectionFactory → documentos, índices y telemetría         │
│ RadicacionModuleConnectionFactory→ tipo documental autorizado               │
├─────────────────────────────────────────────────────────────────────────────┤
│ SERVICIOS                                                                   │
│                                                                             │
│ ValidadorContextoImportacion                                                │
│ ServicioIntencionImportacion                                                │
│ ImportServiceOrchestrator                                                   │
│ ImportExpedientCoordinator                                                  │
│ ImportRelatedDocumentCoordinator                                            │
│ ServicioReconciliacionImportacion                                           │
├─────────────────────────────────────────────────────────────────────────────┤
│ REPOSITORIOS                                                                │
│                                                                             │
│ MySqlImportIntentRepository                                                 │
│ MySqlSiiImportAuthorizationRepository                                       │
│ MySqlImportExpedientConfigurationRepository                                 │
│ PhysicalImportExpedientRepository                                           │
│ LegacyImportExpedientCacheRepository                                        │
│ MySqlImportRelatedDocumentRepository                                        │
│ MySqlImportDocumentLinkCacheRepository                                      │
│ MySqlImportStorageMetadataRepository                                        │
│ MySqlImportDocumentTypeResolver                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 4. Autorización de negocio

`ValidadorContextoImportacion.Validar(contexto)` delega en `MySqlSiiImportAuthorizationRepository`:

- `UsuarioAutenticado`.
- `PermisoVigente`: consulta `PERMISOS_USUARIO_WORKFLOW.ADJUNTAR_IMAGENES_PREDETERMINADA`.
- `TareaOperable`: tarea asignada al usuario, seleccionada, activa, sin fecha de fin y `ESTADO_TAREA=0`.
- `RutaCoincide`.
- `TramiteCoincide`.
- `ProveedorHabilitado`.

No existe bifurcación manager/no-manager: el alcance se obtiene de la asignación activa de Workflow, el permiso, la ruta, el trámite y el contexto persistido.

```text
                 ┌───────────────────────┐
                 │ ¿Contexto autorizado? │
                 └───────────┬───────────┘
                       SÍ    │    NO
              ┌──────────────┴──────────────┐
              ▼                             ▼
     Consultar intención              HTTP 200 + Error.Codigo:
                                     INVALID_CONTEXT
                                     FORBIDDEN
                                     TASK_NOT_OPERABLE
                                     ROUTE_MISMATCH
                                     PROCEDURE_MISMATCH
                                     PROVIDER_NOT_SUPPORTED
```

## 5. Intención, versión y concurrencia

`ImportServiceOrchestrator.Execute` llama `MySqlImportIntentRepository.Obtener(contexto, IntentId)`. La lectura queda filtrada por usuario, tarea e intención.

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ VALIDACIONES DE INTENCIÓN                                                   │
│                                                                             │
│ • La intención existe.                                                      │
│ • Su ContextoOriginal coincide con usuario, grupo, login, tarea, ruta,      │
│   trámite y proveedor actuales.                                             │
│ • request.VersionToken coincide con el token persistido.                    │
└───────────────────────────────┬─────────────────────────────────────────────┘
                                │
            ┌───────────────────┼─────────────────────┐
            ▼                   ▼                     ▼
      No existe           Contexto cambió       Versión cambió
            │                   │                     │
            ▼                   ▼                     ▼
   INTENT_NOT_FOUND   PERSISTED_CONTEXT_MISMATCH  VERSION_CONFLICT
```

Las transiciones usan actualización optimista por estado y versión anterior. `workflow_import_intent`, `workflow_import_intent_item` y `workflow_import_intent_transition` se actualizan transaccionalmente mediante SQL parametrizado.

## 6. Resolución y creación del expediente

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ ImportExpedientCoordinator.Resolver(contexto, intención)                    │
│                                                                             │
│ 1. MySqlImportExpedientConfigurationRepository.Obtener(contexto)            │
│ 2. Asigna gabinete, radicado, ordinal y rol a cada inscripción.             │
│ 3. LegacySiiExpedientSubjectResolver.Resolver(...)                          │
│ 4. Busca cache verificada.                                                  │
│ 5. Busca físicamente; si está ausente, crea.                                │
│ 6. Verifica la postcondición física.                                        │
│ 7. Registra cache solo después de verificar.                                │
│ 8. ImportExpedientPlan.Construir(...)                                       │
└─────────────────────────────────────────────────────────────────────────────┘
```

El rol se calcula así:

- Configuración de expediente único: `Unico`.
- Configuración múltiple, primera inscripción: `Primario`.
- Configuración múltiple, restantes: `Secundario`.

```text
                 ┌──────────────────────────────┐
                 │ ¿Existe cache de expediente? │
                 └──────────────┬───────────────┘
                         SÍ     │     NO
                 ┌──────────────┴───────────────┐
                 ▼                              ▼
 LegacyImportExpedientCacheRepository   PhysicalImportExpedientRepository
 .Obtener(...)                          .Buscar(...)
                 │                              │
                 ▼                              ▼
 .Verificar(idExpediente)              LegacyPhysicalExpedientGateway
                                       .Localizar(...)
                                                │
                                                ▼
                               ┌────────────────────────────┐
                               │ ¿Resultado físico único?   │
                               └──────────────┬─────────────┘
                                      SÍ     │     NO
                               ┌──────────────┴──────────────┐
                               ▼                             ▼
                         Verificar identidad        Ausente: crear
                                                   Múltiple: conflicto
```

La creación usa la función legacy conservada:

```text
ClassGaExpediente.AutoRegistraExpedienteTramite(
    IdTramite,
    CIncripcionSII,
    IdTarea,
    0,
    ByRef idExpediente,
    ByRef nombreExpediente)
```

`PhysicalImportExpedientRepository.Crear` no confía únicamente en el retorno `YES`. Verifica inmediatamente mediante:

- `ClassGaExpediente.SolicitaDatosEstructuraExpediente`.
- `ClassGaExpediente.SolicitaGabineteProducionExpediente`.
- Comparación de gabinete, identidad normalizada y campos configurados.

Si la respuesta de creación se pierde, vuelve a localizar antes de permitir otro intento. Los resultados posibles son `Confirmado`, `Conflicto`, `Ausente`, `Fallido` o `ResultadoIncierto`.

## 7. Descarga y almacenamiento

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ PASOS SECUENCIALES POR ITEM                                                 │
│                                                                             │
│ DownloadImportExecutionStep.Ejecutar                                        │
│   → resuelve proveedor SII                                                  │
│   → DownloadAsync(ExternalKey, CorrelationId, IntentId, ClientItemId, ...)   │
│   → obtiene metadatos SII                                                   │
│   → rechaza respuesta vacía                                                 │
│                                                                             │
│ PrepareImportExecutionStep.Ejecutar                                         │
│   → permite PDF, TIFF, PNG y JPEG                                           │
│   → determina la extensión desde metadatos confiables                       │
│   → crea archivo temporal aleatorio                                         │
│                                                                             │
│ PrepareImportIndicesExecutionStep.Ejecutar                                  │
│   → exige archivo preparado                                                 │
│                                                                             │
│ StoreImportExecutionStep.Ejecutar                                           │
│   → MySqlImportStorageMetadataRepository.Resolver                           │
│   → MySqlImportDocumentTypeResolver.Resolver                                │
│   → Class_DAT_ADIC_TAR.SolicitaReciboCodigoBarrasSII                        │
│   → LegacyImportDocumentStorageAdapter.Almacenar                            │
│   → elimina el temporal en finally                                          │
│                                                                             │
│ CompleteImportExecutionStep(CacheActualizado)                               │
│   → aún no declara la intención Completada                                  │
└─────────────────────────────────────────────────────────────────────────────┘
```

Campos SII preparados para el gabinete:

`CODBARRAS`, `ENLASE`, `MATRICULA`, `RAZONSOCIAL`, `NITCEDULA`, `LIBRO`, `INSCRIPCION`, `RECIBOCAJA`, `FECHAINSCRIP`/`FECHAREGISTR`, `ACTO` y `DESCRIACTO`/`DESCRIPCIONA`.

Los números se normalizan, los textos se limitan según el gabinete y la fecha `yyyyMMdd` se convierte a `yyyy-MM-dd`.

## 8. Descubrimiento y vinculación de documentos relacionados

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ ImportRelatedDocumentCoordinator.Procesar(...)                              │
│                                                                             │
│ MySqlImportRelatedDocumentRepository.ObtenerPorEnlace(                      │
│     contexto, nombreGabinete, radicadoSii)                                  │
│                                                                             │
│ SELECT ID, ENLASE                                                           │
│ FROM `<gabinete-validado>`                                                  │
│ WHERE ENLASE=@radicado                                                      │
│ ORDER BY ID                                                                 │
│                                                                             │
│ El resultado incluye documentos previos y nuevos; IdImagen es la identidad. │
└───────────────────────────────┬─────────────────────────────────────────────┘
                                ▼
                    ┌──────────────────────────┐
                    │ Por cada IdImagen        │
                    └─────────────┬────────────┘
                                  ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│ 1. MySqlImportRelatedDocumentRepository.Persistir                          │
│    Registra el plan físico en workflow_import_related_document.             │
├─────────────────────────────────────────────────────────────────────────────┤
│ 2. DocumentExpedientRelationAdapter.Vincular                               │
│    Gateway: LegacyDocumentExpedientPhysicalGateway                          │
│                                                                             │
│    Mutación legacy conservada:                                              │
│    ClassGaExpediente.VinculaDocumentoExpediente(...)                        │
│                                                                             │
│    Postcheck autoritativo:                                                  │
│    ClassDaGabinete.Solicita_id_expediente_imagen_gabinete(...)              │
├─────────────────────────────────────────────────────────────────────────────┤
│ 3. MySqlImportDocumentLinkCacheRepository.RegistrarVerificado              │
│    Solo registra una relación físicamente confirmada.                       │
├─────────────────────────────────────────────────────────────────────────────┤
│ 4. SiiDocumentIndexAdapter.Actualizar                                       │
│    Gateway: LegacySiiDocumentIndexPhysicalGateway                           │
│    Actualiza NITCEDULA, RAZONSOCIAL y MATRICULA.                            │
├─────────────────────────────────────────────────────────────────────────────┤
│ 5. SiiDocumentIndexAdapter.Verificar                                        │
│    Comprueba independientemente campos, registro SQL e índice XML.          │
├─────────────────────────────────────────────────────────────────────────────┤
│ 6. MySqlImportRelatedDocumentRepository.Persistir                          │
│    Conserva estados de relación, cache, índices y reconciliación.           │
└───────────────────────────────┬─────────────────────────────────────────────┘
                                │
                 ┌──────────────┴───────────────┐
                 ▼                              ▼
       Todos confirmados               Algún efecto no confirmado
                 │                              │
                 ▼                              ▼
       Items → Completada             RELATED_DOCUMENTS_NOT_CONFIRMED
                                      Conflicto o ResultadoIncierto
```

## 9. Persistencia

| Tabla | Responsabilidad |
|---|---|
| `workflow_import_intent` | Cabecera, contexto confiable, estado y versión. |
| `workflow_import_intent_item` | Estado por item, documento, expediente y efectos. |
| `workflow_import_intent_requirement` | Requisitos del preflight. |
| `workflow_import_intent_transition` | Auditoría de cada transición. |
| `workflow_import_inscription` | Agregado de inscripciones y expediente asignado. |
| `workflow_import_related_document` | Diario por cada `IdImagen` descubierto. |
| `workflow_import_document_link_cache` | Cache verificada con unicidad tarea–imagen–gabinete. |

Las operaciones usan parámetros. El único identificador dinámico es el nombre del gabinete, previamente validado y resuelto desde configuración del servidor.

## 10. Respuesta

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ HTTP 200 OK                                                                 │
│                                                                             │
│ {                                                                           │
│   "SchemaVersion": "1.0",                                                  │
│   "OperationId": "...",                                                    │
│   "CorrelationId": "...",                                                  │
│   "IntentId": "...",                                                       │
│   "Accepted": true,                                                        │
│   "Status": "Completada | Parcial | Detenida | ResultadoIncierto",         │
│   "VersionToken": "...",                                                   │
│   "Items": [                                                               │
│     {                                                                       │
│       "ClientItemId": "...",                                               │
│       "ExternalKey": "...",                                                │
│       "Status": "Completada",                                              │
│       "DocumentId": 9630,                                                  │
│       "ErrorCode": null,                                                    │
│       "PersistenceKnown": true,                                            │
│       "Retryable": false,                                                   │
│       "CorrelationId": "..."                                               │
│     }                                                                       │
│   ],                                                                        │
│   "Error": null                                                            │
│ }                                                                           │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 11. Contrato de errores efectivo

| Condición | HTTP actual | Código funcional |
|---|---:|---|
| Gate desactivado | 200 | `FEATURE_DISABLED` |
| Solicitud incompleta | 200 | `INVALID_REQUEST` |
| Sesión inválida | 200 | `FORBIDDEN` o `SESSION_*` |
| Tarea diferente de la seleccionada | 200 | `TASK_CONTEXT_MISMATCH` |
| Tarea no operable | 200 | `TASK_NOT_OPERABLE` |
| Intención inexistente | 200 | `INTENT_NOT_FOUND` |
| Contexto persistido diferente | 200 | `PERSISTED_CONTEXT_MISMATCH` |
| Conflicto de versión | 200 | `VERSION_CONFLICT` |
| Expediente no resoluble | 200 | `EXPEDIENT_DESTINATION_UNRESOLVED` |
| Plan no persistido | 200 | `EXPEDIENT_PLAN_NOT_PERSISTED` |
| Documento no almacenado | 200 | Código dentro del item |
| Relaciones o índices no confirmados | 200 | `RELATED_DOCUMENTS_NOT_CONFIRMED` |
| Excepción controlada | 200 | `IMPORT_UNAVAILABLE` |
| Fallo ASMX/IIS no controlado | 500 | Error de transporte |

## 12. Archivos principales

- `webservice/WebServiceImportarServicioWebModern.asmx.vb`.
- `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`.
- `Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb`.
- `Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb`.
- `Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb`.
- `Infrastructure/Repositories/Workflow/ImportarServicioWeb/PhysicalImportExpedientRepository.vb`.
- `Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyPhysicalExpedientGateway.vb`.
- `Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyDocumentExpedientPhysicalGateway.vb`.
- `Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacySiiDocumentIndexPhysicalGateway.vb`.
