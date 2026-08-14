# Inventario técnico DOC-10

El inventario cubre las funciones y rutas introducidas o reutilizadas directamente por el preview. No es un inventario completo del sistema legacy.

| Ruta | Capa | Clase / función | Entrada | Responsabilidad y datos | Dependencia legacy permitida |
| --- | --- | --- | --- | --- | --- |
| `webservice/WebServiceWorkflowModern.asmx` | Presentation | Directiva ASMX | — | Publica la clase paralela. | Ninguna. |
| `webservice/WebServiceWorkflowModern.asmx.vb` | Presentation | `PreviewEnviarTarea(idTarea As Long)` | Solo `idTarea` | Asegura contexto, compone repositorios con las conexiones Workflow y Docuarchi resueltas y devuelve DTO JSON. | Gate de sesión y snapshots de módulo. |
| `webservice/WorkflowPreviewSessionContextGate.vb` | Presentation | `AsegurarContexto()` | `HttpContext.Current.Session` | Para Gestión, valida en cada llamada la relación y establece usuario, grupo, ruta y login Workflow relacionados; obtiene snapshots Workflow/Docuarchi. No acepta datos del cliente. | `ClassGestionDocumental.SolicitaDatosUsuarioGestionLogin` y `InicioWorkflow.SolicitaIdUsuarIdRutaGrupoWorkflow`, ambos de lectura. |
| Mismo archivo | Presentation | `CrearCadenaConexion(prefijoSesion)` | Snapshot de sesión del módulo | Construye en memoria la cadena Workflow o Docuarchi para inyectarla; nunca la serializa. | Configuración ya resuelta por `gestor_modulos`. |
| Mismo archivo | Presentation | `CrearRespuestaSegura(idTarea)` | Tarea | Convierte una excepción no controlada en bloqueo funcional. | Ninguna. |
| `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | Application | Constructor de cinco dependencias | Repositorios, gate, validador | Composición de preview sin ejecutor legacy. | Ninguna. |
| Mismo archivo | Application | `EvaluarHabilitacion(contexto)` | Contexto | Valida contexto y evalúa gate. | `IWorkflowModernFeatureGate`. |
| Mismo archivo | Application | `Previsualizar(contexto, idTarea)` | Contexto, tarea | Gate → tarea → flujo/ruta → `PrevisualizacionTransicionDto`. | Puertos de lectura solamente. |
| Mismo archivo | Application | `ProveedorTransicionesFlujo.Obtener` | Contexto, tarea | Mapea dominio de flujo a DTO seguro. | `ITransicionFlujoRepository`. |
| Mismo archivo | Application | `ProveedorTransicionesRuta.Obtener` | Contexto, tarea | Mapea dominio de ruta a DTO seguro. | `ITransicionRutaRepository`. |
| `Services/Workflow/Terminar/ValidadorTransicionTarea.vb` | Application | `ValidarContexto` / `ValidarSolicitud` | Contexto o solicitud | Produce códigos funcionales para entradas inválidas. | Ninguna. |
| `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` | Contrato | `PrevisualizacionTransicionDto` y relacionados | — | Define JSON serializable, destino, contexto y error seguro. | Ninguna. |
| `Modelo/Workflow/Terminar/WorkflowModernModels.vb` | Domain | `ContextoModuloWorkflow`, `TareaWorkflow`, `ResultadoDestinosTransicion` | — | Modelos internos de autorización, tarea y destinos. | Ninguna. |
| `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` | Domain | `ITareaWorkflowRepository`, `ITransicionFlujoRepository`, `ITransicionRutaRepository` | Contexto, tarea | Define puertos de lectura. | Ninguna. |
| `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb` | Infrastructure | `MySqlTareaWorkflowRepository.ObtenerTarea` | Contexto, tarea | Recupera tarea seleccionada, activa y autorizada; radicado, ruta, actividad, flujo y token. | Esquema MySQL existente, no clases legacy. |
| Mismo archivo | Infrastructure | `MySqlTransicionFlujoRepository.ObtenerDestinos` | Contexto, tarea | Lista conectores salientes autorizados del origen real; no interpreta campos de libertad de asignación como estado de envío. | Tablas Workflow existentes. |
| Mismo archivo | Infrastructure | `MySqlTransicionRutaRepository.ObtenerDestinos` | Contexto, tarea | Valida ruta/trámite; lee `tipo_doc_entrante.estado_ruta_open_close` en Docuarchi y filtra actividad origen, grupo, ruta y destino en Workflow. | Tablas Docuarchi y Workflow existentes. |
| `Infrastructure/Shared/Data/ModuleDataContracts.vb` | Shared Data | `IDataExecutor.ExecuteReader(Of T)` | SQL, parámetros, proyector | Contrato de lectura tipada reutilizable. | Ninguna. |
| `Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb` | Shared Data | `AdoNetDataExecutor.ExecuteReader(Of T)` | Conexión, parámetros, proyector | Ejecuta lector ADO.NET y materializa modelo tipado. | ADO.NET. |
| `Infrastructure/Shared/Data/ModuleConnectionFactory.vb` | Shared Data | `ModuleConnectionFactory.CreateOpenConnection` | Contexto | Abre conexión `MyDbContext` solo tras validar contexto. | Configuración de conexión. |
| `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb` | Shared Data | `WorkflowModuleConnectionFactory` / `DocuarchiModuleConnectionFactory` | Contexto, snapshot de conexión | Abren la conexión inyectada por Presentation; no conocen `HttpContext` ni `Session`. | MySQL ADO.NET. |
| `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` | Infrastructure | `Evaluar(contexto)` | Contexto | Evalúa activación, usuarios/grupos y exclusiones. | `Web.config`. |
| `tools/validation/Verify-Doc10Preview.ps1` | Validación | Verificaciones estáticas/focales | Ensamblado y fuente | Valida contrato, escenarios y ausencia de escritura. | Ninguna. |
| `tools/e2e/Invoke-Doc10PreviewE2E.ps1` | E2E | `Invoke-Doc10PreviewE2E` | URL, usuarios, tarea, SELECTs | Ejecuta prueba desplegada y compara estado/auditoría antes/después. | `gestor.aspx` para autenticación controlada. |
| `tools/e2e/tests/doc10-preview.spec.cjs` | E2E | `@anonymous`, `@session`, `@authorization`, `@full` | Variables de entorno seguras | Ejecuta ASMX sin sesión, bootstrap, autorización y huellas antes/después. Acepta bloqueo funcional esperado para una tarea no disponible. | `gestor.aspx`, ASMX y MySQL de solo lectura. |
| `tools/e2e/scripts/assert-e2e-config.cjs` | E2E | Validación de modos | Variables requeridas | Rechaza configuraciones incompletas antes de abrir navegador o conectar MySQL. | Ninguna. |
| `tools/e2e/scripts/run-doc10-concurrency.cjs` | Rendimiento | `test:load` | Sesiones piloto, tarea y `SELECT` | Autentica sesiones separadas y mide el ASMX a 20/30 concurrentes; conserva p50/p95/p99, fallos y huellas seguras. | `gestor.aspx`, ASMX y MySQL de solo lectura. |
| `AGENTS.md` | Guía de agentes | Entrada E2E DOC-10 | — | Obliga a leer el runbook antes de una prueba autenticada y fija el cierre del gate. | Ninguna. |
| `tools/e2e/AGENT-RUNBOOK.md` | Guía de agentes | E2E y carga | Secretos en sesión, tarea y `SELECT` | Describe preparación, modos, carga, evidencia y recuperación sin secretos ni bypass. | `gestor.aspx`, ASMX y MySQL de solo lectura. |

## Rutas de configuración y contrato

| Recurso | Ruta / clave | Uso |
| --- | --- | --- |
| Endpoint | `/webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea` | POST JSON del frontend. |
| Gate principal | `WorkflowCentroTrabajoModernActive` | Apagado por defecto; activa el preview en piloto. |
| Alcance piloto | `WorkflowCentroTrabajoModernUsers`, `WorkflowCentroTrabajoModernGroups` | Lista permitida de usuario o grupo. |
| Exclusiones | `WorkflowCentroTrabajoModernExcludedUsers`, `WorkflowCentroTrabajoModernExcludedGroups` | Prevalecen sobre la activación. |
| Conexiones | Snapshots Workflow y Docuarchi en sesión | El ASMX entrega Workflow a tarea/destinos y Docuarchi a `tipo_doc_entrante`; `MyDbContext` queda solo en la composición fail-closed y no abre conexión. |
| Evidencia E2E | `Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/evidencias/` | Solo resultados resumidos, sin secretos. |

Los únicos archivos legacy preservados expresamente son `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`. No se llaman `ClassWorkflow.Terminar_Tarea_Workflow`, `ClassWorkflow.Cambia_Estado`, eventos dinámicos, correo ni firma desde las rutas inventariadas.
