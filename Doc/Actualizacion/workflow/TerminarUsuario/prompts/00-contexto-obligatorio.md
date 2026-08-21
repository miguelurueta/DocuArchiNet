# Prompt base obligatorio — Modernización de "Enviar a usuario"

Adjuntar este prompt al inicio de cada etapa de implementación de esta carpeta.

## USO DE ESTE ARCHIVO

Este archivo **solo establece contexto, límites y criterios comunes**. Por sí solo no autoriza ni ordena implementar código, crear o modificar un cambio OpenSpec, ejecutar pruebas, generar paquetes documentales ni avanzar a una etapa posterior.

- Si se entrega únicamente `00-contexto-obligatorio.md`, leerlo y responder que se requiere el prompt numerado de la etapa a ejecutar.
- Ejecutar una etapa únicamente cuando la solicitud incluya de forma expresa uno de los prompts `01` a `04`; aplicar solo esa etapa.
- No inferir una etapa a partir del objetivo global ni interpretar “implementar todo” como permiso para recorrer automáticamente los prompts. La secuencia debe ser solicitada o confirmada etapa por etapa.
- Una instrucción del usuario que contradiga esta regla —por ejemplo, “no implementar código”— prevalece y detiene cualquier cambio de producto, aunque se hubiera indicado una etapa previamente.

## CONTROL POR JIRA

Jira es la única fuente de estado, dependencias, aprobaciones y cierre de esta modernización. Cada ticket fijo debe enlazar exactamente uno de los prompts de esta carpeta.

- Antes de actuar, comprobar que el ticket Jira actual referencia el archivo de etapa correcto y que sus predecesores están aprobados o cerrados.
- Ejecutar solo el prompt asociado al ticket actual. No crear tareas, propuestas ni artefactos OpenSpec paralelos y no avanzar al ticket sucesor.
- El resultado de la etapa debe indicar ticket actual, evidencia producida, archivos modificados, verificaciones y bloqueos para que Jira pueda desbloquear la siguiente etapa.
- Si el ticket no identifica con claridad el prompt, sus predecesores o la autorización aplicable, detenerse y solicitar la corrección del ticket; no inferirlos del objetivo global.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior de .NET Framework, VB.NET, ASP.NET Web Forms, MySQL y JavaScript legado. Trabaja incrementalmente, conserva compatibilidad y no amplíes el alcance sin documentar la decisión y obtener la aprobación necesaria.

## OBJETIVO

Modernizar únicamente el comando **Enviar a usuario** del Centro de trabajo en `workflow/Webworkflow.aspx`.

La operación moderna recibe y ejecuta con `IdTarea`, `IdUsuarioWorkflowDestino`, `IdActividadDestino` y `TokenVersion`. Debe terminar la tarea mediante `ClassWorkflow.Terminar_Tarea_Workflow`, sin conector y sin reasignar respuesta.

## ALCANCE

- Implementar solo la etapa indicada por el prompt complementario en `prompts/`.
- Usar `Doc/Actualizacion/workflow/TerminarUsuario/00-exploracion-arquitectura-envio-usuario.md` como decisión arquitectónica vigente.
- Reutilizar componentes transversales seguros: contexto autenticado, token de versión, `GET_LOCK`, auditoría, confirmación y actualización de presentación.
- Reutilizar `WorkflowModernPresentationBootstrap` y `WebServiceWorkflowModern.asmx`; es una operación adicional dentro del mismo límite moderno.
- La experiencia moderna es la ruta oficial para todo usuario con contexto Workflow válido; no depende de listas piloto, usuarios, grupos ni configuración de habilitación.

## RESTRICCIONES CRÍTICAS

- `Enviar a usuario` es envío directo; el destino es el par `IdUsuarioWorkflowDestino` + `IdActividadDestino`, nunca un `IdConector`.
- No crear conectores ficticios ni relajar la regla `IdConector > 0` de **Continuar flujo**.
- No crear appSettings, banderas ni fuentes de evaluación para condicionar la experiencia moderna.
- No modificar contratos, endpoints, destinos ni comportamiento de `PreviewEnviarTarea`, `EjecutarEnvioTarea` o `ServicioTransicionTarea`.
- No usar el destino recibido del navegador como autorización: revalidar en servidor permiso, tarea, token, ruta/flujo y relación usuario–actividad–ruta.
- El preview solo puede ejecutar `SELECT`; no modifica tarea, estado, auditoría, eventos ni respuesta.
- La operación moderna **no debe invocar** `After_envio_usuario_workflow`, `Reasigna_respuesta_envia_tarea_usuario` ni ninguna reasignación de respuesta.
- Si el estado de respuesta requiere confirmación o radicado, bloquear con código y mensaje público. No intentar terminar la tarea ni actualizar la respuesta.
- El ASMX no puede manipular `Page`, `GridView`, `UpdatePanel`, `ModalPopupExtender` ni handlers Web Forms. La mutación final ocurre solo mediante un adaptador específico a `Terminar_Tarea_Workflow`.
- No ejecutar E2E autenticado ni carga sin autorización explícita de ambiente y cuentas de prueba.
- Ninguna etapa cambia configuración de ambiente fuera de su alcance autorizado.
- No imprimir ni guardar credenciales, cookies ni cadenas de conexión.
- Mantener los cambios acotados. No refactorizar componentes no relacionados.

## CRITERIOS DE ACEPTACIÓN

- La nueva solicitud contiene `IdUsuarioWorkflowDestino` e `IdActividadDestino`, sin `IdConector`.
- Para todo usuario con contexto Workflow válido, preview y ejecución validan `CAMBIO_USUARIO`, tarea activa, token, ruta/flujo abierto, usuario activo, actividad destino, pertenencia a ruta y `UTIL_ASIGNA_TAREA=1`.
- El comando usa exclusivamente la experiencia moderna; no existe una ruta alternativa de postback o modal Web Forms para Enviar a usuario.
- Si la respuesta está pendiente, el resultado es bloqueo funcional y no existe reasignación.
- Una ejecución concurrente o con token vencido no produce una segunda transición.
- La auditoría sanitizada usa un mecanismo distinguible `ASMX_ENVIO_USUARIO`.
- Continuar flujo conserva endpoints, payload `IdConector`, validaciones y pruebas actuales sin regresión.
- Los errores públicos no exponen SQL, Session, credenciales ni excepciones internas.

## PRUEBAS Y COMPILACIÓN

- Las etapas de implementación ejecutan únicamente pruebas focales del área cambiada y la compilación disponible.
- La verificación transversal, QA manual y consolidación de evidencia pertenecen exclusivamente a la etapa 03.
- Registrar comando, resultado, cobertura y limitaciones reproducibles; no sustituir evidencias por E2E autenticado o carga no autorizados.

## DOCUMENTACIÓN TÉCNICA

- Actualizar la exploración cuando cambie una decisión, contrato o requisito; no crear OpenSpec para este flujo gestionado en Jira.
- Todas las etapas actualizan exclusivamente el paquete documental único `Doc/Actualizacion/workflow/TerminarUsuario/01-implementacion-envio-usuario/`, con `00-indice.md`, `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md`, `04-pruebas-y-evidencia.md` y `Diagramas/` cuando corresponda.
- Documentar endpoints, payloads, códigos de bloqueo, mecanismo de auditoría y la experiencia moderna oficial cuando se introduzcan.
- Registrar archivos modificados, supuestos y riesgos residuales en el resultado de cada etapa.

## ENTREGABLE FINAL

Entregar una respuesta breve y verificable con:

1. Cambios implementados y relación con el objetivo.
2. Archivos modificados.
3. Pruebas y compilación ejecutadas, con resultados.
4. Documentación actualizada.
5. Riesgos, limitaciones o decisiones pendientes.

No continuar a una etapa posterior si la actual no cumple los criterios o requiere una decisión funcional.
