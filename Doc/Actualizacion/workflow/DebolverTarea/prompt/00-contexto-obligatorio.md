# Contexto interno — Modernización de devolver a actividad anterior

Este archivo permanece versionado en el repositorio. No se adjunta, copia ni carga en Jira: al ejecutar una etapa, el agente lo lee desde esta carpeta junto con el prompt numerado enlazado por el ticket.

## USO DE ESTE ARCHIVO

Este archivo establece contexto, límites y criterios comunes. Por sí solo no autoriza implementar código, ejecutar pruebas, crear trabajo paralelo, generar paquetes documentales ni avanzar a una etapa posterior.

- Ejecutar una etapa únicamente cuando la solicitud incluya de forma expresa uno de los prompts `01` a `04`.
- No inferir una etapa a partir del objetivo global ni recorrer los prompts automáticamente.
- Una instrucción del usuario que detenga implementación o pruebas prevalece sobre la etapa indicada.

## CONTROL POR JIRA

Jira es la única fuente de estado, dependencias, aprobaciones y cierre.

- Cada ticket debe enlazar solo el prompt numerado de su etapa y tener sus predecesores aprobados o cerrados.
- No crear tareas, propuestas ni artefactos de planificación paralelos.
- El resultado debe informar ticket, evidencia, archivos modificados, verificaciones y bloqueos.
- Si el ticket no identifica etapa, predecesores o autorización, detenerse y solicitar corrección.

## OBJETIVO

Modernizar únicamente **Devolver → Elegir actividad anterior** del Centro de trabajo en `workflow/Webworkflow.aspx`.

La operación devuelve una tarea activa a un predecesor válido de su Ruta o Flujo. El navegador envía solamente `IdTarea`, `IdConector` y `TokenVersion`; el servidor resuelve el tipo de contexto desde la tarea y reconstruye/revalida el destino mediante el conector entrante. En Ruta, `IdConector` representa exclusivamente `actividades_disponibles_envio.id_actividades_disponibles_envio`; en Flujo representa exclusivamente el identificador de conector de Flujo. Nunca se intercambian ni se confían como autorización.

## FUENTES DE DECISIÓN

- `Doc/Actualizacion/workflow/DebolverTarea/Exploracion/01-exploracion-modernizacion-actividad-anterior.md`.
- `Doc/Actualizacion/workflow/DebolverTarea/Exploracion/02-modelo-requerimientos-devolver-actividad-anterior.md`.

## ALCANCE

- Reutilizar contexto autenticado, token de versión, `GET_LOCK`, auditoría, confirmación y actualización moderna de presentación.
- Implementar preview de solo lectura, búsqueda paginada, confirmación y ejecución moderna de devolución.
- Soportar Ruta y Flujo con repositorios que validen su semántica específica detrás de un contrato público uniforme.
- La experiencia moderna es la única ruta de esta operación para todo usuario con contexto Workflow válido; no depende de pilotos, listas de usuarios/grupos ni configuración de habilitación. Su registro de presentación no evalúa `WorkflowCentroTrabajoModernActive` y no cambia el gate de las otras operaciones.

## RESTRICCIONES CRÍTICAS

- El conector debe ser entrante al origen real de la tarea; no reutilizar la semántica de conectores salientes de Continuar flujo. En Ruta se valida la arista completa: identificador de configuración, actividad origen, actividad siguiente, Ruta de la tarea y actividad actual; en Flujo se valida la arista y el Flujo/actividad actuales.
- No confiar en identificadores, actividad, usuario, grupo, Ruta ni Flujo enviados por el navegador.
- El preview ejecuta exclusivamente `SELECT` parametrizados y no modifica tarea, estado, auditoría, eventos ni datos de negocio.
- No modificar contratos o comportamiento de Continuar flujo, Enviar a usuario, Enviar a grupo ni Usuario anterior.
- No crear banderas de habilitación, rutas UI alternativas, postbacks, `GridView`, `UpdatePanel`, `ModalPopupExtender` ni campos ocultos para autorizar o ejecutar.
- La operación no consulta, valida, bloquea, crea, actualiza, reasigna ni audita respuestas, radicados o confirmaciones. Los componentes nuevos no referencian `Classgestionrespuesta`, `Verifica_respuesta_*` ni `Reasigna_respuesta_envia_tarea_usuario`.
- La política de notificación y eventos dinámicos de `Terminar_Tarea_Workflow` debe estar aprobada en Exploración antes de backend. El adaptador y los componentes nuevos no pueden construir ni invocar componentes de respuestas; una construcción transitoria interna del motor legacy es tolerable solo si los parámetros aprobados impiden ejecutar sus métodos y una prueba focal lo demuestra.
- El ASMX no manipula `Page`, controles Web Forms ni handlers. La mutación final ocurre mediante un adaptador específico a `Terminar_Tarea_Workflow`, con `Page = Nothing` y actualización de interfaz legacy desactivada.
- No ejecutar E2E autenticada, carga ni una tarea real sin autorización explícita de ambiente y cuentas de prueba.
- No imprimir ni guardar credenciales, cookies ni cadenas de conexión.

## CRITERIOS DE ACEPTACIÓN

- Ruta y Flujo devuelven solo predecesores válidos y autorizados del contexto actual.
- Un conector manipulado, ajeno o retirado se bloquea antes de cambiar la tarea.
- Un token vencido, lock ocupado o solicitud concurrente no provoca una segunda transición. El lock es exclusivo por `IdTarea`; token y conector se revalidan dentro del lock.
- Éxito, bloqueo y error devuelven mensajes funcionales sin detalles técnicos.
- La bandeja conserva su layout y restablece su desplazamiento horizontal después del éxito.
- Ninguna capa de la capacidad contiene tratamiento de respuestas.

## PRUEBAS Y COMPILACIÓN

- Las etapas de implementación ejecutan pruebas focales y la compilación disponible.
- La verificación transversal, QA manual y consolidación de evidencia pertenecen a la etapa 03.
- Registrar comando, resultado, cobertura y limitaciones reproducibles.

## DOCUMENTACIÓN TÉCNICA

- Actualizar las fuentes de decisión cuando cambie un contrato o requisito.
- La implementación documenta `00-indice.md`, `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` bajo `Doc/Actualizacion/workflow/DebolverTarea/01-implementacion-devolver-actividad-anterior/`.
- Registrar archivos modificados, supuestos y riesgos residuales.

## ENTREGABLE FINAL

Entregar cambios, archivos, pruebas y compilación, documentación actualizada y riesgos o decisiones pendientes. No avanzar de etapa si falta un criterio verificable o una aprobación funcional.
