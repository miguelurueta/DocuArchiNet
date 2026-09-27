# Inventario y alcance

## Código productivo verificado, no modificado

| Tipo | Archivo | Símbolo | Responsabilidad comprobada |
|---|---|---|---|
| Gate | `Infrastructure/Workflow/ImportarServicioWeb/ImportarServicioWebFeatureGate.vb` | `ImportarServicioWebFeatureGate.EstaHabilitado(ContextoModuloWorkflow) As Boolean` | Exige contexto Workflow válido y `WorkflowCentroTrabajoModernActive=true`; no filtra por usuario o grupo. |
| Configuración | `Web.config` | claves `WorkflowCentroTrabajoModernActive`, `WorkflowCentroTrabajoModernUsers`, `WorkflowCentroTrabajoModernGroups` | Línea base segura: `false`, cadena vacía y cadena vacía. |
| Bootstrap | `workflow/Webworkflow.aspx.vb` | `WorkflowCentroTrabajoModernActive`, registro de scripts de importación | Publica la interfaz moderna bajo el gate y distingue ENLASE mediante `ANEXOS_RADICADO_ENLASE`. |
| Asignación | `workflow/Webworkflow.aspx.vb` | `Buttonaceptar_Click(Object, EventArgs)` | Conserva la asignación explícita y llama `ClassWorkflowDigitalizacion.Verfica_existencia_tipo_documental_obligatorio_digitalizado`; cualquier resultado distinto de `YES` detiene la asignación. |
| Adaptador UI | `js/workflow/importar-servicio-web/enlase/importar-servicio-web-enlase-adapter.js` | adaptador ENLASE | Fija la capacidad y delega presentación/transporte compartidos. |
| Plataforma E2E | `tools/e2e/scripts/support/workflow-e2e-platform-registry.cjs` | `SCENARIO_REGISTRY`, `CONTROL_REGISTRY` | Registra escenarios, siete controles de importación y el control de asignación de solo lectura. |
| Seguridad E2E | `tools/e2e/scripts/support/workflow-e2e-platform.cjs` | `requiredAuthorizationsFor`, `captureLegacyIntegrityBaseline`, `assertPlatformIntegrity`, `createSafeEvidence`, `executePlatformRun` | Autoriza por etapa, sanea evidencia, compara legacy y exige restauración del gate. |

## Escenarios reutilizados

| Escenario | Origen | Etapa | Mutación | Autorizaciones base | Autorizaciones de etapa |
|---|---|---|---|---|---|
| `import-sii-enlase-anonymous` | DOC-83 | `anonymous` | No | `environment`, `gate` | TLS local solo si el perfil lo solicita; sin sesión, tarea, controles ni secretos. |
| `import-sii-enlase-read` | DOC-80 | `read` | No | `environment`, `gate` | TLS local solo si el perfil lo solicita. |
| `import-sii-enlase-execution` | DOC-81 | `execution` | Sí | `environment`, `gate` | `execution`, `discardable-resource`; TLS local si aplica. |
| `import-sii-enlase-ui` | DOC-82 | `read` | No | `environment`, `gate` | TLS local solo si el perfil lo solicita. |
| `import-sii-enlase-assignment` | DOC-83 | `assignment` | Sí | `environment`, `gate` | `execution`, `discardable-resource`; TLS local si aplica. |

Los tres escenarios autenticados de consulta/importación usan los siete controles `import-*`. La asignación usa únicamente `workflow-assignment-state`, un `SELECT` registrado sobre `estados_tarea_workflow`. El perfil no puede proporcionar SQL.

## Fuera de alcance

- No se creó `ValidateAssignment` ni se simuló habilitación preventiva.
- No se añadió gate, proveedor SII, login, archivo `.env`, proyecto Playwright ni sistema de evidencia paralelo.
- No se cambió lógica productiva, esquema de base de datos ni comportamiento legacy.
- No se ejecutó una corrida real nueva sin autorización vigente.
