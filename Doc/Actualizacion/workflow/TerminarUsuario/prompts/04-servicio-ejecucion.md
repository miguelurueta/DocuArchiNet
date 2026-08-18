# 04 — Ejecución segura de envío a usuario

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior de casos de uso Workflow, concurrencia MySQL y encapsulación de motores legacy.

## OBJETIVO

Implementar `ServicioEnvioUsuarioTarea` y `EjecutarEnvioUsuario` en el ASMX moderno existente. La operación termina directamente la tarea en el usuario destino y reutiliza controles transversales sin introducir reasignación de respuesta.

## RESTRICCIONES CRÍTICAS

- Lee y aplica `prompts/00-contexto-obligatorio.md`.
- No modificar lógica ni validación de `ServicioTransicionTarea` o `EjecutarEnvioTarea`.
- No usar `IdConector` ni crear uno artificial; el request usa usuario y actividad destino.
- Application no usa `Page`, Session, `GridView`, `UpdatePanel` ni `ModalPopupExtender`.
- No duplicar `Terminar_Tarea_Workflow`, `Cambia_Estado`, firma, expediente, balanceo, correo ni eventos dinámicos.
- No llamar `After_envio_usuario_workflow` ni métodos de reasignación de respuesta.
- Si la respuesta está pendiente, bloquear antes del motor legacy y no producir cambios.
- Reutilizar el mismo gate y ASMX; no crear ni modificar configuración de habilitación.

## REQUISITOS POSITIVOS

1. Validar contexto, gate, `CAMBIO_USUARIO`, `IdTarea`, `IdUsuarioWorkflowDestino`, `IdActividadDestino` y token.
2. Adquirir el `GET_LOCK` existente por tarea y versión.
3. Dentro del lock, releer tarea, comparar token y revalidar permiso, apertura de ruta/flujo, respuesta en `YES`, usuario activo, `UTIL_ASIGNA_TAREA`, actividad y pertenencia a ruta.
4. Resolver la notificación desde el usuario destino en servidor.
5. Delegar únicamente al adaptador legacy directo de la etapa 05 y mapear éxito, bloqueo, error reintentable y advertencias a DTO público.
6. Registrar auditoría sanitizada con mecanismo `ASMX_ENVIO_USUARIO`, sin SQL, Session, token, documentos ni credenciales.
7. Exponer `EjecutarEnvioUsuario(idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion)` sin alterar endpoints existentes.

## CRITERIOS DE ACEPTACIÓN

- Dos solicitudes simultáneas no producen dos transiciones.
- Token vencido, permiso cambiado, respuesta pendiente o destino retirado bloquean antes del motor legacy.
- Una falla de auditoría produce advertencia segura y no revierte una transición confirmada.
- `EjecutarEnvioTarea` y la transición por conector continúan sin cambios.

## PRUEBAS OBLIGATORIAS

Agregar y ejecutar pruebas de solicitud inválida, gate/permiso bloqueados, respuesta pendiente, token vencido, destino fuera de ruta, usuario inactivo, `UTIL_ASIGNA_TAREA=0`, concurrencia, error reintentable, advertencia de auditoría y éxito. Ejecutar MSBuild y pruebas focales; no ejecutar E2E mutante sin autorización y tarea descartable.

## DOCUMENTACIÓN TÉCNICA

Crear o actualizar `Doc/Actualizacion/workflow/TerminarUsuario/04-servicio-ejecucion/` con componentes, secuencia bajo lock, contrato de ejecución, códigos, auditoría, exclusión explícita de reasignación y evidencia de pruebas.

## ENTREGABLE FINAL

Entregar servicio, composición ASMX, contratos, pruebas, compilación, documentación y listado de reglas legacy preservadas. No implementar UI hasta que la ejecución pase sus pruebas.

