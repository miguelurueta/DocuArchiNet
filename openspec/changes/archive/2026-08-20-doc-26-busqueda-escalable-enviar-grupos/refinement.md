<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-26-busqueda-escalable-enviar-grupos

## Fuente y alcance

- Ticket: DOC-26 — BUSQUEDA-ESCALABLE-ENVIAR-GRUPOS.
- Cambio OpenSpec: doc-26-busqueda-escalable-enviar-grupos.
- Perfil tecnológico: .NET Framework, ASP.NET Web Forms, VB.NET, MySQL y JavaScript legado accesible.
- Alcance: búsqueda paginada de destinos para el modal moderno de Enviar a grupo. La selección y la ejecución conservan IdActividadDestino.

No se crean gates, fuentes de autorización, despliegues paralelos ni cambios de esquema. Con el gate inactivo permanece el postback Web Forms existente; Continuar flujo mantiene sus endpoints y su payload con IdConector.

## Contexto inspeccionado

- webservice/WebServiceWorkflowModern.asmx.vb expone PreviewEnviarGrupo(idTarea) y EjecutarEnvioGrupo(idTarea, idActividadDestino, tokenVersion), y compone ServicioEnvioGrupoTarea sin invocar el motor legacy desde el ASMX.
- Infrastructure/Repositories/Workflow/MySqlEnvioGrupoRepository.vb valida contexto, Cambio_Ruta, tarea, ruta y flujo, y hoy carga todos los destinos de la ruta mediante LISTADO_ACTIVIDADES_WORKFLOW con LEFT JOIN a grupos_workflow.
- El LEFT JOIN actual puede producir varias filas de la misma actividad cuando tiene varios grupos asociados; la actividad, no el grupo, es el identificador seleccionable.
- js/workflow/workflow-group-send-ui.js presenta la lista completa del preview en tabla y tarjetas; ya dispone de estados, aria-live, Escape, trampa de foco, reintento y descarte de respuestas de preview obsoletas.
- workflow/Webworkflow.aspx mantiene un modal moderno aislado y workflow/Webworkflow.aspx.vb registra sus scripts solo tras el gate existente.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Se añade BuscarDestinosEnvioGrupo como operación ASMX moderna de lectura; PreviewEnviarGrupo conserva su firma y entrega una primera página acotada, mientras EjecutarEnvioGrupo no cambia de firma ni semántica. | WebServiceWorkflowModern.asmx.vb; ServicioEnvioGrupoTarea | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | El contrato de búsqueda y el preview acotado devuelven idTarea, tokenVersion, página normalizada, tamaño aplicado, tieneMas, destinos sanitizados y error público. La búsqueda recibe idTarea, termino, pagina y tamanoPagina. | DTOs de EnvioGrupo y WebServiceWorkflowModern.asmx.vb | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | El servidor normaliza término vacío o de 2 a 80 caracteres, página a un mínimo de 1 y tamaño a 1..50; un término no vacío de un carácter o de más de 80 produce bloqueo funcional seguro. | ServicioEnvioGrupoTarea y validador específico | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Cada búsqueda reevalúa sesión, gate existente, Cambio_Ruta, tarea activa, ruta y flujo aplicable antes de revelar resultados; toda la ruta de búsqueda usa únicamente SELECT parametrizados. | WorkflowPreviewSessionContextGate.vb; MySqlEnvioGrupoRepository.vb | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | La consulta devuelve una actividad por IdActividadDestino, filtra por actividad o grupo asociado y resume grupos como nombre único o cantidad; un grupo nunca es una opción seleccionable independiente. | LISTADO_ACTIVIDADES_WORKFLOW y grupos_workflow | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Se recupera tamanoPagina más uno para calcular tieneMas sin COUNT por pulsación; no se crean índices ni se modifica el esquema sin una migración y decisión posterior aprobadas. | MySqlEnvioGrupoRepository.vb | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | La UI usa demora de 300 ms, mínimo de dos caracteres para iniciar búsqueda, cancelación o descarte de respuestas obsoletas, primera página al limpiar y selección invalidada ante filtro o página nuevos. | workflow-group-send-ui.js; workflow-group-send-confirmation.js | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | La ejecución conserva exactamente { idTarea, idActividadDestino, tokenVersion }, su relectura dentro del lock y sus revalidaciones; se conservan accesibilidad, fallback legacy y aislamiento de Continuar flujo. | ServicioEnvioGrupoTarea; WorkflowLegacyEnvioGrupoExecutorAdapter.vb | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo y compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El cliente llama solo BuscarDestinosEnvioGrupo para filtros remotos. | Cuando se escribe un término válido, entonces se solicita una página de la operación nueva; PreviewEnviarGrupo conserva la carga inicial y EjecutarEnvioGrupo no recibe parámetros nuevos. | Evita cambiar consumidores existentes. |
| RQ-02 | La respuesta no contiene sesión, SQL, permisos, secretos ni IdConector. | Cuando la solicitud es válida, entonces devuelve solamente los campos del contrato y un error normalizado si no puede continuar. | Impide filtración de datos internos. |
| RQ-03 | Los límites se aplican antes de Infrastructure y se devuelven en la respuesta. | Cuando pagina o tamanoPagina están fuera de rango, entonces se normalizan; cuando término tiene longitud inválida, entonces se entrega un código funcional seguro. | Evita consultas ilimitadas o ambiguas. |
| RQ-04 | Un usuario no autorizado o una tarea fuera de su contexto no recibe destinos. | Dado gate inactivo, Cambio_Ruta denegado, tarea inactiva, ruta cerrada o flujo inválido, cuando busca, entonces recibe bloqueo sin filas y sin escritura. | La búsqueda no concede autorización de ejecución. |
| RQ-05 | Cada actividad aparece una vez y puede encontrarse por nombre de actividad o grupo. | Dada una actividad con varios grupos, cuando el término coincide con cualquiera, entonces retorna una sola opción con resumen acotado de grupos. | Evita duplicidad y selección ambigua. |
| RQ-06 | Las rutas extensas devuelven como máximo el tamaño aplicado y tienen indicador de continuación. | Dada una lista de más de una página, cuando se consulta una página, entonces tieneMas deriva de una fila adicional sin COUNT por pulsación. | El rendimiento se verifica con plan de consulta antes de cualquier índice. |
| RQ-07 | Tabla, tarjetas y estado anunciado muestran el mismo conjunto vigente. | Dada una respuesta lenta, cambio de término, limpieza, página o reintento, entonces una respuesta anterior no reemplaza los resultados actuales y una selección anterior no se puede confirmar. | Conserva teclado, foco, Escape y uso móvil. |
| RQ-08 | Buscar no debilita la operación mutante ni el fallback. | Dado token vencido, destino retirado o concurrencia, cuando se ejecuta, entonces EjecutarEnvioGrupo conserva el bloqueo; con gate inactivo continúa el postback legacy y Continuar flujo conserva IdConector. | No hay regresión de contratos ni de seguridad. |

## Resultado del refinamiento

La implementación comienza con el contrato, el repositorio de búsqueda y las pruebas focales. No se ejecutará E2E autenticado, carga ni activación del gate sin autorización explícita.
