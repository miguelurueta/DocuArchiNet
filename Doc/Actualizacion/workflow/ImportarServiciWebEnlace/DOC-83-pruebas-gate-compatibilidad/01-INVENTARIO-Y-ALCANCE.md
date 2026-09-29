# Inventario y alcance

## Código productivo verificado

| Tipo | Archivo | Símbolo | Responsabilidad comprobada |
|---|---|---|---|
| Gate | `Infrastructure/Workflow/ImportarServicioWeb/ImportarServicioWebFeatureGate.vb` | `ImportarServicioWebFeatureGate.EstaHabilitado(ContextoModuloWorkflow) As Boolean` | Exige contexto Workflow válido y `WorkflowCentroTrabajoModernActive=true`; no filtra por usuario o grupo. |
| Configuración | `Web.config` | claves `WorkflowCentroTrabajoModernActive`, `WorkflowCentroTrabajoModernUsers`, `WorkflowCentroTrabajoModernGroups` | Línea base segura: `false`, cadena vacía y cadena vacía. |
| Bootstrap | `workflow/Webworkflow.aspx.vb` | `WorkflowCentroTrabajoModernActive`, registro de scripts de importación | Publica la interfaz moderna bajo el gate y distingue ENLASE mediante `ANEXOS_RADICADO_ENLASE`. |
| Asignación | `workflow/Webworkflow.aspx.vb` | `Buttonaceptar_Click(Object, EventArgs)` | Conserva la asignación explícita y llama `ClassWorkflowDigitalizacion.Verfica_existencia_tipo_documental_obligatorio_digitalizado`; cualquier resultado distinto de `YES` detiene la asignación. |
| Adaptador UI | `js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-adapter.js` | adaptador ENLASE | Fija la capacidad y delega presentación/transporte compartidos. |
| Plataforma E2E | `tools/e2e/scripts/support/workflow-e2e-platform-registry.cjs` | `SCENARIO_REGISTRY`, `CONTROL_REGISTRY` | Registra escenarios, siete controles de importación y el control de asignación de solo lectura. |
| Seguridad E2E | `tools/e2e/scripts/support/workflow-e2e-platform.cjs` | `requiredAuthorizationsFor`, `captureLegacyIntegrityBaseline`, `assertPlatformIntegrity`, `createSafeEvidence`, `executePlatformRun` | Autoriza por etapa, sanea evidencia, compara legacy y exige restauración del gate. |
| Proyección de almacenamiento | `Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyEnlaseImportDocumentStorageAdapter.vb` | `Almacenar(ComandoAlmacenamientoImportacion)`, `CrearProyeccion(...)` | Convierte la `stru_datos_image_lista` ya devuelta por el almacenamiento exitoso en una proyección tipada; usa `DBT` o, si el flujo sin tipología no lo llena, la `extension` física retornada. No realiza otra consulta. |
| Contrato público | `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb` | `ImportEnlaseDocumentProjectionDto`, `ImportItemResultDto.EnlaseProjection` | Transporta gabinete, documento, radicado, tipo físico, nombre, tarea, firma e icono sin exponer `dato_lista`. |
| Adaptador de lista | `js/workflow/importar-servicio-web/importar-servicio-web-document-list-adapter.js` | `createLegacyGridAppender(options)` | Valida identidad y campos obligatorios; para ENLASE traduce al contrato de ocho campos y llama el destino `rad` sin postback. |

## Escenarios reutilizados

| Escenario | Origen | Etapa | Mutación | Autorizaciones base | Autorizaciones de etapa |
|---|---|---|---|---|---|
| `import-sii-enlase-anonymous` | DOC-83 | `anonymous` | No | `environment`, `gate` | TLS local solo si el perfil lo solicita; sin sesión, tarea, controles ni secretos. |
| `import-sii-enlase-read` | DOC-80 | `read` | No | `environment`, `gate` | TLS local solo si el perfil lo solicita. |
| `import-sii-enlase-execution` | DOC-81 | `execution` | Sí | `environment`, `gate` | `execution`, `discardable-resource`; TLS local si aplica. |
| `import-sii-enlase-ui` | DOC-82 | `read` | No | `environment`, `gate` | TLS local solo si el perfil lo solicita. |
| `import-sii-enlase-assignment` | DOC-83 | `assignment` | Sí | `environment`, `gate` | `execution`, `discardable-resource`; TLS local si aplica. |
| `import-sii-enlase-manual-visual` | DOC-83 | `execution` | Sí | `environment`, `gate` | `execution`, `discardable-resource`; navegador visible, lista SII verificada, línea base no invasiva, inserción `rad` observada, dos confirmaciones y máximo diez minutos. |

Los escenarios autenticados de consulta/importación, incluida la aceptación visual mutadora, usan los siete controles `import-*`. La asignación usa únicamente `workflow-assignment-state`, un `SELECT` registrado sobre `estados_tarea_workflow`. El perfil no puede proporcionar SQL.

## Fuera de alcance

- No se creó `ValidateAssignment` ni se simuló habilitación preventiva.
- No se añadió gate, proveedor SII, login, archivo `.env`, proyecto Playwright ni sistema de evidencia paralelo.
- La única lógica productiva añadida en DOC-83 es la proyección inmediata ENLASE desde un resultado ya confirmado; no cambia persistencia, esquema de base de datos ni el comportamiento de `GridView_list_documento_relacion_wf`.
- No se ejecutó una corrida real nueva sin autorización vigente.
