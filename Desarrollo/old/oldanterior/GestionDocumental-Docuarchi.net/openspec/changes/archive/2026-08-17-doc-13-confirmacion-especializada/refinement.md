<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - DOC-13: confirmación especializada

## Fuente y alcance

- Ticket: `DOC-13` — CONFIRMACION-ESPECIALIZADA.
- Cambio OpenSpec: `doc-13-confirmacion-especializada`.
- Perfil técnico observado: ASP.NET Web Forms .NET Framework 4.6.1, VB.NET, ASMX, JavaScript progresivo y CSS aislado.
- Alcance aprobado: componente de confirmación reutilizable de Presentation y adaptador de Workflow que consume el preview ya seleccionado y el endpoint `EjecutarEnvioTarea` existente.
- Fuera de alcance: nuevos endpoints, expansión de DTOs, cambios en `Terminar_Tarea_Workflow`, `Cambia_Estado`, autorización, validaciones de negocio, repositorios, SQL, Session, migraciones o bibliotecas UI.

## Contexto inspeccionado

- `js/workflow/workflow-transition-ui.js` consulta `PreviewEnviarTarea`, conserva el `tokenVersion` y emite `workflow:destination-selected`; en DOC-12 aún no ejecuta una transición.
- `workflow/Webworkflow.aspx` contiene el enlace legacy, su modal y el host del modal moderno. `workflow/Webworkflow.aspx.vb` registra los recursos modernos solo cuando `WorkflowModernPresentationBootstrap` permite la solicitud.
- `webservice/WebServiceWorkflowModern.asmx.vb` ya expone `EjecutarEnvioTarea(idTarea, idConector, tokenVersion)` y delega a `ServicioTransicionTarea.Ejecutar`.
- `ServicioTransicionTarea.Ejecutar` revalida feature gate, token, tarea, destino, requisitos, concurrencia y autorización legacy mediante adaptadores; `ResultadoTransicionDto` ya publica éxito, bloqueo, advertencias, destino, versión y referencia.
- El preview real entrega radicado, tipo de decisión, grupo actual, destino seleccionado y token. No entrega trámite; `ActividadOrigen` es un identificador no legible y `Requisitos` se calcula durante la ejecución.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | La confirmación muestra solamente datos del preview que el usuario pueda interpretar: radicado, tipo, grupo actual y datos del destino. Omite trámite, actividad origen no legible y requisitos o advertencias no entregados por preview; no los infiere ni amplía backend. | `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb`; `Services/Workflow/Terminar/ServicioTransicionTarea.vb`; `js/workflow/workflow-transition-ui.js` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | `ConfirmationDialog` se implementa una sola vez en `js/java_general/ConfirmationDialog.js`, con CSS propio y API global `open(config)`/`close()`, sin referencias a Workflow, HTML legacy, Session, controles ocultos ni textos del caso de uso. | `js/workflow/workflow-transition-ui.js`; `workflow/Webworkflow.aspx` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Un adaptador en `js/workflow/workflow-transition-confirmation-integration.js` transforma la selección y la respuesta ASMX a la configuración genérica; la lista moderna debe publicar el resumen normalizado del preview requerido por el adaptador. | `js/workflow/workflow-transition-ui.js`; `webservice/WebServiceWorkflowModern.asmx.vb` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Solo el adaptador invoca asíncronamente `EjecutarEnvioTarea` con `{ idTarea, idConector, tokenVersion }`; mientras la solicitud está en curso, el componente previene doble envío, bloquea cierre, reemplazo y cancelación, y solicita confirmación nativa ante navegación. | `js/java_general/ConfirmationDialog.js`; `js/workflow/workflow-transition-confirmation-integration.js`; `webservice/WebServiceWorkflowModern.asmx.vb`; `Services/Workflow/Terminar/ServicioTransicionTarea.vb` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | El adaptador entrega el éxito correlacionado a `WorkflowTransitionPagePresentation.applySuccess`, un callback de Presentation basado en atributos `data-*`. Solo entonces retira la fila si aún existe, restablece la lista, limpia visor y contexto, oculta acciones de la selección, ajusta contador y anuncia éxito durante seis segundos; bloqueo o error no ejecutan ese callback. | `workflow/ClassListandoTareas.vb`; `workflow/Webworkflow.aspx`; `js/workflow/workflow-transition-page-presentation.js` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | La experiencia sigue condicionada por `WorkflowModernPresentationBootstrap` y el ASMX revalida `IWorkflowModernFeatureGate`; desactivar la bandera devuelve al flujo legacy sin fallback automático ni cambio de estado. | `workflow/WorkflowModernPresentationBootstrap.vb`; `Infrastructure/Workflow/Terminar/ConfiguracionWorkflowModernFeatureGate.vb`; `workflow/Webworkflow.aspx.vb` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | La entrega incluye pruebas JavaScript focales, build real, QA manual accesible y responsive, y el paquete documental obligatorio bajo `Doc/Actualizacion/workflow/Terminar/05-confirmacion-especializada/`. | `tests/workflow-transition-ui.test.cjs`; `GestionDocumental-Docuarchi.net.vbproj`; `Doc/Actualizacion/workflow/Terminar/05-confirmacion-especializada.md` | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El resumen de confirmación solo representa información proveniente del preview normalizado. | Cuando el preview no publica trámite, nombre legible de actividad origen, requisitos o advertencias, el diálogo los omite y no consulta DOM legacy, Session ni valores derivados. | No se amplía Application por contexto de presentación; evita información inventada o engañosa. |
| RQ-02 | El diálogo reutilizable puede abrirse con un `config` simulado ajeno a Workflow. | Al suministrar título, etiquetas, campos, aviso, contexto opaco y callbacks genéricos, el diálogo no requiere rutas, selectores, globals ni vocabulario de Workflow. | Un único componente evita duplicación y no acopla otros módulos a Web Forms. |
| RQ-03 | El adaptador convierte la selección validada y la respuesta del ASMX entre ambos contratos sin trasladar reglas críticas al navegador. | Al recibir `workflow:destination-selected`, el adaptador forma `summaryFields`, `executionContext`, texto contextual y callbacks; solo él conoce los textos de envío. | La lista y el componente quedan separados; no se llama a botones invisibles ni a métodos legacy. |
| RQ-04 | Existe como máximo un envío útil para la combinación de tarea, conector y token actualmente confirmada, y la persona conserva visibilidad de su resultado. | Durante `enviando`, acciones, `X`, Cancelar, fondo, Escape, cierre programático y una nueva apertura no cierran el diálogo; la navegación solicita la confirmación nativa del navegador. | El servidor conserva el guard de concurrencia y vuelve a validar versión y destino; si la pestaña se cierra pese al aviso, la lista se vuelve a consultar desde servidor. |
| RQ-05 | La interfaz cambia solo después de un resultado funcional exitoso y correlacionado. | Ante éxito se cierra la confirmación y el callback actualiza solo el contexto afectado; ante bloqueo o error se conserva el contexto, se muestra mensaje seguro y se habilita recuperación conforme a `EsReintentable`. | Evita perder tareas por respuestas técnicas, bloqueos o resultados obsoletos. |
| RQ-06 | El camino legacy permanece disponible cuando el piloto moderno no está activo. | Si falta o es falso el bootstrap, no se cargan ni se enlazan recursos de confirmación; si el endpoint devuelve `WORKFLOW_MODERN_INACTIVE`, se muestra bloqueo sin ejecutar fallback automático. | Rollback inmediato por configuración, sin migración de datos ni alteración del motor legacy. |
| RQ-07 | Las pruebas y la documentación permiten repetir y auditar la entrega. | Pruebas focales cubren contratos, estados, doble envío y correlación; QA registra cancelación, éxito, bloqueo, error, cierre en vuelo, teclado, ARIA, contraste y móvil/escritorio. | La prueba E2E autenticada se ejecuta solo con ambiente y credenciales autorizadas; se registra la limitación si no existe. |

## Decisión de alcance registrada

Por confirmación de alcance, DOC-13 no agrega trámite, una actividad origen legible, requisitos ni advertencias al preview. El adaptador recibe valores vacíos para los campos opcionales y el diálogo no los representa. La ejecución conserva el contrato existente: la evaluación de requisitos, autorización y transición ocurre exclusivamente en servidor. La actualización visual posterior usa el callback de página basado en `data-*`; no accede a controles ocultos ni invoca funciones legacy.

## Resultado del refinamiento

Estado: aprobado para diseñar y planificar. La matriz D/RQ está completa; el desglose de `tasks.md` se propone por separado con tareas atómicas antes de iniciar código.
