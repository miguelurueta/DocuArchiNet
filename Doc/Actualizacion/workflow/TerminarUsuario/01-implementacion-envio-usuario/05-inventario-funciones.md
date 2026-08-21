# Inventario técnico de funciones — DOC-28

Inventario de funciones incorporadas o reutilizadas por **Enviar a usuario**. Se excluyen constructores, DTOs y normalizadores triviales sin decisión de negocio. Las rutas son relativas a la raíz del repositorio.

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Clasificación: cross_cutting

## Funciones implementadas

| Función / elemento | Objetivo | Ruta física | Descripción |
| --- | --- | --- | --- |
| `PreviewEnviarUsuario(...)` | Consultar destinos antes de enviar | `webservice/WebServiceWorkflowModern.asmx.vb` | Endpoint ASMX de solo lectura; crea el servicio exclusivo y devuelve destinos paginados o bloqueo funcional. |
| `EjecutarEnvioUsuario(...)` | Ejecutar envío directo | `webservice/WebServiceWorkflowModern.asmx.vb` | Endpoint mutante; no recibe identidad, permiso ni `IdConector` del navegador y devuelve únicamente resultado público. |
| `AsegurarContextoEnvioUsuario(prepararEjecucion)` | Crear contexto seguro | `webservice/WorkflowPreviewSessionContextGate.vb` | Reutiliza la validación de sesión y calcula `PuedeCambioUsuario` con el índice 18 de permisos. |
| `Previsualizar(contexto, solicitud)` | Resolver una página de destinos | `Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb` | Valida solicitud, contexto, permiso y tarea; no toma lock ni llama auditoría o motor. |
| `Ejecutar(contexto, solicitud)` | Orquestar una transición única | `Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb` | Toma el lock, relee permiso/tarea/token/destino/requisitos, invoca el adaptador y registra trazabilidad. |
| `NormalizarPreview(...)` y `ValidarEjecucion(...)` | Rechazar entradas inseguras | `Services/Workflow/Terminar/ValidadorEnvioUsuarioTarea.vb` | Limita tamaño, filtro, cursor, identificadores y token antes de acceder a infraestructura. |
| `BuscarDestinos(...)` | Buscar destinos autorizados | `Infrastructure/Repositories/Workflow/MySqlEnvioUsuarioRepository.vb` | Valida estado y realiza `SELECT` parametrizado, orden estable, cursor protegido y límite en servidor. |
| `ResolverDestino(...)` | Revalidar el destino | `Infrastructure/Repositories/Workflow/MySqlEnvioUsuarioRepository.vb` | Comprueba dentro de la ejecución que usuario, actividad y relación con la ruta continúan vigentes. |
| `ValidarEstado(...)` y `ValidarRutaAbierta(...)` | Proteger la lectura y resolución | `Infrastructure/Repositories/Workflow/MySqlEnvioUsuarioRepository.vb` | Verifican tarea, ruta, flujo y actividad de flujo antes de consultar usuarios destino. |
| `Evaluar(contexto, tarea, destino)` | Aplicar política de respuesta | `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioRequisitosAdapter.vb` | Solo permite `Verifica_respuesta_radicado_sin_respuesta = YES`; no reasigna respuesta. |
| `TieneCambioUsuario(contexto)` | Reautorizar permiso en servidor | `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioAutorizacionAdapter.vb` | Consulta de nuevo el permiso legacy antes de alcanzar el motor. |
| `Ejecutar(contexto, tarea, destino)` | Cruzar la frontera mutante | `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioUsuarioExecutorAdapter.vb` | Única llamada a `Terminar_Tarea_Workflow`; usa `Page = Nothing`, conector cero y normaliza advertencias. |
| `RegistrarAuditoria(...)` | Trazar resultado sanitizado | `Services/Workflow/Terminar/ServicioEnvioUsuarioTarea.vb` | Registra `ASMX_ENVIO_USUARIO`; ante falla conserva el resultado y agrega advertencia. |

## Funciones y componentes reutilizados

| Función / elemento | Objetivo | Ruta física | Descripción |
| --- | --- | --- | --- |
| `AsegurarContexto()` y `AsegurarContextoEjecucion()` | Construir contexto autenticado | `webservice/WorkflowPreviewSessionContextGate.vb` | Base existente que valida sesión y conexiones de módulo para preview y ejecución. |
| `ObtenerTarea(contexto, idTarea)` | Releer estado de la tarea | `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb` | Repositorio reutilizado para detectar tarea no disponible o token obsoleto. |
| `Adquirir(contexto, idTarea, tokenVersion)` | Evitar doble ejecución | `Infrastructure/Workflow/Terminar/MySqlTransicionConcurrencyGuard.vb` | Implementa `GET_LOCK` y libera el lease en todos los caminos. |
| `Registrar(auditoria)` | Persistir trazabilidad histórica | `Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb` | Conserva el log sanitizado y normaliza el mecanismo `ASMX_ENVIO_USUARIO`. |
| `Terminar_Tarea_Workflow(...)` | Realizar transición de negocio | `workflow/ClassWorkflow.vb` | Motor legacy llamado exclusivamente mediante el adaptador nuevo. |
| `ValidadorTransicionTarea` | Validar contexto común | `Services/Workflow/Terminar/ValidadorTransicionTarea.vb` | Reutilizado sin alterar contratos por conector. |

## Contratos que conectan las funciones

| Contrato | Objetivo | Ruta física | Descripción |
| --- | --- | --- | --- |
| `SolicitudPreviewEnvioUsuario` y `SolicitudEnvioUsuarioWorkflow` | Expresar intención de preview y ejecución | `Modelo/Workflow/Terminar/WorkflowModernModels.vb` | Solicitudes exclusivas sin `IdConector`, `Page` ni `Session`. |
| `DestinoEnvioUsuarioWorkflow` y resultados internos | Aislar dominio de ASMX | `Modelo/Workflow/Terminar/WorkflowModernModels.vb` | Conservan usuario, actividad, notificación, requisitos y bloqueos internos. |
| `IEnvioUsuarioBusquedaRepository` e `IEnvioUsuarioEjecucionRepository` | Separar lectura y revalidación | `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` | Definen búsqueda paginada y resolución directa. |
| `IEnvioUsuarioRequisitosRepository`, `IEnvioUsuarioAutorizacionRepository` e `IEnvioUsuarioLegacyExecutor` | Aislar política, permiso y mutación | `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` | Evitan reutilizar el ejecutor por conector. |
| DTOs de usuario | Estabilizar la frontera pública | `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` | Publican destino, preview, resultado y códigos funcionales seguros. |

## Lectura de mantenimiento

Para cambiar la orquestación, empezar en `ServicioEnvioUsuarioTarea.Ejecutar`; para acceso y permisos, revisar `AsegurarContextoEnvioUsuario` y `WorkflowLegacyEnvioUsuarioAutorizacionAdapter`; para consultas, `MySqlEnvioUsuarioRepository`; para la mutación, únicamente `WorkflowLegacyEnvioUsuarioExecutorAdapter`. La futura UI debe pertenecer a la etapa 02 y no debe activar gates ni ejecutar E2E autenticado por una actualización documental.
