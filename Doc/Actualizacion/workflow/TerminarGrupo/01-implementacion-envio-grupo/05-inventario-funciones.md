# Inventario técnico de funciones — DOC-15

Inventario de las funciones operativas incorporadas o reutilizadas por **Enviar a grupo**. Se excluyen constructores, propiedades, DTOs, normalizadores triviales y utilidades de presentación sin decisión funcional. Las rutas son relativas a la raíz del repositorio.

## Funciones implementadas

| Función / elemento | Objetivo | Ruta física | Descripción |
| --- | --- | --- | --- |
| `PreviewEnviarGrupo(idTarea)` | Consultar destinos antes de enviar | `webservice/WebServiceWorkflowModern.asmx.vb` | Endpoint ASMX de solo lectura. Obtiene el contexto autenticado, crea el servicio de grupo y entrega destinos o un bloqueo funcional. |
| `EjecutarEnvioGrupo(idTarea, idActividadDestino, tokenVersion)` | Ejecutar el envío directo a una actividad | `webservice/WebServiceWorkflowModern.asmx.vb` | Endpoint ASMX mutante. No acepta identidad, permisos ni `IdConector` desde el navegador; convierte fallos inesperados en una respuesta pública segura. |
| `AsegurarContextoEnvioGrupo(prepararEjecucion)` | Preparar un contexto confiable para la operación de grupo | `webservice/WorkflowPreviewSessionContextGate.vb` | Reutiliza la validación de sesión y agrega el permiso efectivo `Cambio_Ruta`, obtenido del servidor antes de preview o ejecución. |
| `Previsualizar(contexto, idTarea)` | Resolver destinos disponibles | `Services/Workflow/Terminar/ServicioEnvioGrupoTarea.vb` | Valida gate, contexto, permiso, tarea y estado; consulta solo destinos de la ruta y produce el token de versión. |
| `Ejecutar(contexto, solicitud)` | Orquestar una única transición de grupo | `Services/Workflow/Terminar/ServicioEnvioGrupoTarea.vb` | Valida solicitud, toma el lock, relee la tarea, resuelve el destino, revisa requisitos, invoca el adaptador legacy y deja auditoría sanitizada. |
| `ValidarSolicitud(solicitud)` | Rechazar entradas incompletas o inválidas | `Services/Workflow/Terminar/ValidadorEnvioGrupoTarea.vb` | Comprueba identificador de tarea, actividad destino y token antes de acceder a repositorios o al motor legacy. |
| `ObtenerDestinos(contexto, tarea)` | Listar actividades destino válidas | `Infrastructure/Repositories/Workflow/MySqlEnvioGrupoRepository.vb` | Repositorio de lectura que valida ruta, flujo y actividad, y consulta actividades de la misma ruta. |
| `ResolverDestino(contexto, tarea, idActividadDestino)` | Revalidar el destino seleccionado | `Infrastructure/Repositories/Workflow/MySqlEnvioGrupoRepository.vb` | Comprueba, durante la ejecución, que la actividad sigue disponible y pertenece a la ruta de la tarea. |
| `ValidarEstado(...)` y `LeerDestinos(...)` | Proteger las consultas de destino | `Infrastructure/Repositories/Workflow/MySqlEnvioGrupoRepository.vb` | Funciones internas que separan la comprobación de estado de la lectura SQL; la operación de preview no escribe datos. |
| `Evaluar(contexto, tarea, destino)` | Verificar requisitos legacy | `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioGrupoRequisitosAdapter.vb` | Adapta la revisión de aprobaciones para bloquear un envío pendiente, sin ejecutar reasignación de respuesta/radicado. |
| `Ejecutar(contexto, tarea, destino)` | Invocar el motor legado de forma aislada | `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioGrupoExecutorAdapter.vb` | Única frontera que llama `ClassWorkflow.Terminar_Tarea_Workflow`; utiliza envío directo y normaliza bloqueos, advertencias y errores. |
| `RegistrarAuditoria(...)` y `CrearTareaAuditoria(...)` | Trazar el resultado sin datos sensibles | `Services/Workflow/Terminar/ServicioEnvioGrupoTarea.vb` | Construyen y registran la auditoría `ASMX_ENVIO_GRUPO`, con correlación, resultado y código funcional; las advertencias de auditoría no repiten la transición. |
| `MapearEjecucion(...)`, `MapearDestinos(...)` y `MapearRequisitos(...)` | Mantener el contrato público separado del dominio | `Services/Workflow/Terminar/ServicioEnvioGrupoTarea.vb` | Transforman modelos internos en DTOs de ASMX sin filtrar excepciones ni detalles de infraestructura. |
| `RegisterWorkflowEnvioGrupoModernScript()` | Cargar la UI moderna de destinos | `workflow/Webworkflow.aspx.vb` | Registra una única vez el script de selección de destino cuando la experiencia moderna está habilitada. |
| `RegisterWorkflowEnvioGrupoConfirmationIntegrationScript()` | Cargar la confirmación de envío | `workflow/Webworkflow.aspx.vb` | Registra una única vez el módulo que confirma y ejecuta el envío de grupo. |
| `RegisterWorkflowEnvioGrupoModernBootstrap()` | Entregar el identificador de tarea a la UI | `workflow/Webworkflow.aspx.vb` | Inicializa los módulos cliente sin alterar el postback Web Forms cuando el gate está inactivo. |
| `solicitarPrevisualizacion(idTarea)` | Consultar el preview desde el navegador | `js/workflow/workflow-group-send-ui.js` | Realiza `POST` same-origin a `PreviewEnviarGrupo`, desempaqueta la respuesta ASMX y normaliza el contrato. |
| `crearDetalleSeleccion(...)` | Propagar destino y token seleccionados | `js/workflow/workflow-group-send-ui.js` | Construye el detalle del evento `workflow:group-destination-selected` con tarea, destino, contexto visible y token. |
| `inicializar()` y `aplicarEnvioExitoso(...)` | Gestionar modal, foco y refresco local | `js/workflow/workflow-group-send-ui.js` | Intercepta el trigger moderno, soporta Escape/Tab/foco, descarta previews obsoletos y limpia la selección tras éxito. |
| `normalizeSelection(...)` | Validar el contexto recibido por la confirmación | `js/workflow/workflow-group-send-confirmation.js` | Exige tarea, destino y token antes de permitir una llamada mutante. |
| `executeSend(context)` | Invocar la ejecución desde la confirmación | `js/workflow/workflow-group-send-confirmation.js` | Realiza `POST` same-origin a `EjecutarEnvioGrupo` con el payload mínimo permitido. |
| `normalizeResult(...)`, `openFromSelection(...)` e `initialize()` | Mostrar resultado y coordinar la confirmación | `js/workflow/workflow-group-send-confirmation.js` | Clasifican éxito, bloqueo o error reintentable; abren la confirmación compartida y notifican solo el resultado vigente. |

## Funciones y componentes reutilizados

| Función / elemento | Objetivo | Ruta física | Descripción |
| --- | --- | --- | --- |
| `AsegurarContexto()` | Construir el contexto autenticado de preview | `webservice/WorkflowPreviewSessionContextGate.vb` | Función existente reutilizada por `AsegurarContextoEnvioGrupo`; valida sesión y datos de módulo en servidor. |
| `AsegurarContextoEjecucion()` | Endurecer el contexto de operación mutante | `webservice/WorkflowPreviewSessionContextGate.vb` | Función existente reutilizada antes de ejecutar; evita delegar identidad o autorización al cliente. |
| `Evaluar(contexto)` | Resolver el gate único de Workflow moderno | `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb` | Evalúa `WorkflowCentroTrabajoModernActive`, alcance, exclusiones y fallback. Se comparte con la modernización existente; no se creó una bandera de grupo. |
| `ObtenerTarea(contexto, idTarea)` | Releer la tarea desde el origen de verdad | `Infrastructure/Repositories/Workflow/MySqlWorkflowPreviewRepositories.vb` | Repositorio existente reutilizado en preview y dentro del lock para detectar tarea o token obsoletos. |
| `Adquirir(contexto, idTarea, tokenVersion)` | Evitar ejecuciones concurrentes | `Infrastructure/Workflow/Terminar/MySqlTransicionConcurrencyGuard.vb` | Toma `GET_LOCK` por tarea y versión; su lease libera el lock al finalizar. |
| `Registrar(auditoria)` | Persistir trazabilidad de Workflow | `Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb` | Adaptador existente que inserta el evento sanitizado en el registro histórico, sin exponer errores técnicos al usuario. |
| `Terminar_Tarea_Workflow(...)` | Realizar la transición de negocio | `Infrastructure/Workflow/Terminar/WorkflowLegacyEnvioGrupoExecutorAdapter.vb` | Motor legado reutilizado únicamente a través del nuevo adaptador; su definición no está disponible como fuente rastreable en este repositorio y el adaptador concentra la llamada que cambia tarea y estado. |
| `WorkflowCentroTrabajoModernActive` | Conservar el fallback Web Forms | `workflow/Webworkflow.aspx.vb` | Propiedad y gate existentes reutilizados para mostrar UI/ASMX modernos solo cuando corresponde; inactivo conserva `ImageButtonEnviaActividad` y postback legado. |
| `ConfirmationDialog` | Confirmación accesible compartida | Módulo cliente existente, invocado desde `js/workflow/workflow-group-send-confirmation.js` | Reutiliza el diálogo de confirmación para no duplicar comportamiento de foco, cancelación y envío controlado. |

## Contratos que conectan las funciones

| Contrato | Objetivo | Ruta física | Descripción |
| --- | --- | --- | --- |
| `IEnvioGrupoDestinosRepository` | Aislar lectura y resolución de destinos | `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` | Declara `ObtenerDestinos` y `ResolverDestino`; su implementación es de solo lectura. |
| `IEnvioGrupoRequisitosRepository` | Aislar requisitos previos | `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` | Declara la evaluación de aprobaciones antes de invocar el motor legacy. |
| `IEnvioGrupoLegacyExecutor` | Aislar la mutación legacy | `Modelo/Workflow/Terminar/WorkflowModernInterfaces.vb` | Declara la única llamada de ejecución directa, sin `Page`, `Session` ni `IdConector`. |
| Solicitud, destinos y resultados de grupo | Estabilizar la frontera ASMX | `Modelo/Workflow/Terminar/WorkflowModernModels.vb` y `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` | Modelos internos y DTOs para tarea, actividad destino, token, bloqueos, advertencias y resultado público. |

## Lectura de mantenimiento

Para cambiar comportamiento de negocio, empezar en `ServicioEnvioGrupoTarea.Ejecutar`; para cambiar seguridad de acceso, revisar `AsegurarContextoEnvioGrupo` y el gate compartido; para cambiar consultas, hacerlo mediante `MySqlEnvioGrupoRepository`; y para modificar la UI, mantener el fallback legado cuando el gate esté inactivo. No activar el gate ni ejecutar E2E autenticado como parte de una actualización documental.
