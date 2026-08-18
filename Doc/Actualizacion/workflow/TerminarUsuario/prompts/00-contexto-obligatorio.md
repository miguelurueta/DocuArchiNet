# Prompt base obligatorio — Modernización de "Enviar a usuario"

Adjuntar este prompt al inicio de cada etapa de implementación de esta carpeta.

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior de .NET Framework, VB.NET, ASP.NET Web Forms, MySQL y JavaScript legado. Trabaja incrementalmente, conserva compatibilidad y no amplíes el alcance sin documentar la decisión y obtener la aprobación necesaria.

## OBJETIVO

Modernizar únicamente el comando **Enviar a usuario** del Centro de trabajo en `workflow/Webworkflow.aspx`.

La operación moderna recibe y ejecuta con `IdTarea`, `IdUsuarioWorkflowDestino`, `IdActividadDestino` y `TokenVersion`. Debe terminar la tarea mediante `ClassWorkflow.Terminar_Tarea_Workflow`, sin conector y sin reasignar respuesta.

## ALCANCE

- Implementar solo la etapa indicada por el prompt complementario en `prompts/`.
- Usar `Doc/Actualizacion/workflow/TerminarUsuario/00-exploracion-arquitectura-envio-usuario.md` como decisión arquitectónica vigente.
- Reutilizar componentes transversales seguros: contexto autenticado, gate, token de versión, `GET_LOCK`, auditoría, confirmación y actualización de presentación.
- Reutilizar `IWorkflowModernFeatureGate`, `WorkflowModernPresentationBootstrap` y `WebServiceWorkflowModern.asmx`; es una operación adicional dentro del mismo límite moderno.
- Mantener el fallback Web Forms cuando la experiencia moderna esté inhabilitada.

## RESTRICCIONES CRÍTICAS

- `Enviar a usuario` es envío directo; el destino es el par `IdUsuarioWorkflowDestino` + `IdActividadDestino`, nunca un `IdConector`.
- No crear conectores ficticios ni relajar la regla `IdConector > 0` de **Continuar flujo**.
- No crear un segundo appSetting, bandera, fuente de evaluación ni gate. Toda habilitación usa la fuente existente y falla cerrada.
- No modificar contratos, endpoints, destinos ni comportamiento de `PreviewEnviarTarea`, `EjecutarEnvioTarea` o `ServicioTransicionTarea`.
- No usar el destino recibido del navegador como autorización: revalidar en servidor permiso, tarea, token, ruta/flujo y relación usuario–actividad–ruta.
- El preview solo puede ejecutar `SELECT`; no modifica tarea, estado, auditoría, eventos ni respuesta.
- La operación moderna **no debe invocar** `After_envio_usuario_workflow`, `Reasigna_respuesta_envia_tarea_usuario` ni ninguna reasignación de respuesta.
- Si el estado de respuesta requiere confirmación o radicado, bloquear con código y mensaje público. No intentar terminar la tarea ni actualizar la respuesta.
- El ASMX no puede manipular `Page`, `GridView`, `UpdatePanel`, `ModalPopupExtender` ni handlers Web Forms. La mutación final ocurre solo mediante un adaptador específico a `Terminar_Tarea_Workflow`.
- No ejecutar E2E autenticado, carga ni activar gate sin autorización explícita de ambiente y cuentas de prueba.
- Ninguna etapa cambia la configuración de habilitación. Si una prueba autorizada la modificara, restaurar `WorkflowCentroTrabajoModernActive=false` y listas de usuarios/grupos vacías antes de terminar.
- No imprimir ni guardar credenciales, cookies ni cadenas de conexión.
- Mantener los cambios acotados. No refactorizar componentes no relacionados.

## CRITERIOS DE ACEPTACIÓN

- La nueva solicitud contiene `IdUsuarioWorkflowDestino` e `IdActividadDestino`, sin `IdConector`.
- Con gate inactivo, el botón conserva el postback y modal legacy de Enviar a usuario.
- Con gate activo, preview y ejecución validan `CAMBIO_USUARIO`, tarea activa, token, ruta/flujo abierto, usuario activo, actividad destino, pertenencia a ruta y `UTIL_ASIGNA_TAREA=1`.
- Si la respuesta está pendiente, el resultado es bloqueo funcional y no existe reasignación.
- Una ejecución concurrente o con token vencido no produce una segunda transición.
- La auditoría sanitizada usa un mecanismo distinguible `ASMX_ENVIO_USUARIO`.
- Continuar flujo conserva endpoints, payload `IdConector`, validaciones y pruebas actuales sin regresión.
- Los errores públicos no exponen SQL, Session, credenciales ni excepciones internas.

## PRUEBAS OBLIGATORIAS

- Agregar o actualizar pruebas automatizadas de contratos y JavaScript del área afectada.
- Ejecutar pruebas unitarias/CJS afectadas y reportar comando, resultado y archivos cubiertos.
- Ejecutar la compilación MSBuild del proyecto afectado cuando esté disponible; si no puede compilarse localmente, documentar causa y verificación manual reproducible.
- Cubrir, cuando corresponda: permiso denegado, tarea no disponible, ruta/flujo cerrado, usuario inactivo o fuera de ruta, `UTIL_ASIGNA_TAREA=0`, respuesta pendiente, token vencido, concurrencia, fallback y no regresión de Continuar flujo.
- No sustituir estas evidencias por E2E autenticado o carga no autorizados.

## DOCUMENTACIÓN TÉCNICA

- Actualizar exploración u OpenSpec aplicable cuando cambie una decisión, contrato o requisito.
- Cada etapa crea o actualiza exclusivamente su paquete bajo `Doc/Actualizacion/workflow/TerminarUsuario/<NN>-<slug>/` con `00-indice.md`, `01-arquitectura.md`, `02-contrato.md`, `03-flujo-y-seguridad.md`, `04-pruebas-y-evidencia.md` y `Diagramas/` cuando corresponda.
- Documentar endpoints, payloads, códigos de bloqueo, mecanismo de auditoría, gate y rollback cuando se introduzcan.
- Registrar archivos modificados, supuestos y riesgos residuales en el resultado de cada etapa.

## ENTREGABLE FINAL

Entregar una respuesta breve y verificable con:

1. Cambios implementados y relación con el objetivo.
2. Archivos modificados.
3. Pruebas y compilación ejecutadas, con resultados.
4. Documentación actualizada.
5. Riesgos, limitaciones o decisiones pendientes.

No continuar a una etapa posterior si la actual no cumple los criterios o requiere una decisión funcional.

