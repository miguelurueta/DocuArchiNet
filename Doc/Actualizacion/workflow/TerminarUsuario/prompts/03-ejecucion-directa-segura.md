# 03 — Ejecución directa segura

## ROL ESPERADO

Actúa como desarrollador senior de casos de uso Workflow, concurrencia MySQL y encapsulación de motores legacy.

## OBJETIVO

Implementar juntos el adaptador exclusivo a `Terminar_Tarea_Workflow`, `ServicioEnvioUsuarioTarea`, auditoría y `EjecutarEnvioUsuario`, cerrando la dependencia entre servicio y adaptador en un único ticket.

## CONTEXTO OBLIGATORIO

- Requiere 02 aprobado, contratos y preview focales en verde.
- Leer `00-contexto-obligatorio.md`, evidencia de 02 y la exploración arquitectónica.
- La salida habilita 04 solo si ejecución, lock y revalidaciones pasan sin E2E ni tarea real.

## REQUISITOS POSITIVOS

- Aceptar solo `{ IdTarea, IdUsuarioWorkflowDestino, IdActividadDestino, TokenVersion }` en `EjecutarEnvioUsuario` del ASMX existente.
- Dentro de `GET_LOCK`, releer y validar contexto, `CAMBIO_USUARIO`, tarea, token, ruta/flujo, respuesta `YES`, usuario activo, relación usuario–actividad–ruta, `UTIL_ASIGNA_TAREA` y notificación.
- Crear puerto y adaptador exclusivos; solo este adaptador invoca una vez `ClassWorkflow.Terminar_Tarea_Workflow` con `Page = Nothing`, sin conector.
- Normalizar éxito, bloqueo, error reintentable y advertencias; auditar con `ASMX_ENVIO_USUARIO` sin datos sensibles.

## RESTRICCIONES CRÍTICAS

- No invocar `After_envio_usuario_workflow`, `Reasigna_respuesta_envia_tarea_usuario`, `Cambia_Estado`, handlers Web Forms, batch de Pendientes ni ejecutor por conector.
- No alterar `ServicioTransicionTarea`, `EjecutarEnvioTarea`, `IWorkflowLegacyExecutor`, `IdConector`, UI ni configuración.
- Respuesta pendiente, token vencido o destino retirado bloquean antes del motor legacy.

## REGLAS DE ANTIRREGRESIÓN

- Continuar flujo conserva su contrato con conector y no recibe código, listeners ni puertos de envío a usuario.
- Una falla de auditoría no revierte una transición ya confirmada; no se duplica transición bajo concurrencia.

## CRITERIOS DE ACEPTACIÓN

- Existe un único punto mutante directo y el navegador no autoriza el destino.
- Dos solicitudes concurrentes no terminan dos veces la tarea y los errores públicos no filtran SQL, sesión ni excepciones.

## PRUEBAS OBLIGATORIAS

Cubrir solicitud inválida, permiso, respuesta pendiente, token, destino retirado, usuario inactivo, `UTIL_ASIGNA_TAREA=0`, lock ocupado, advertencia correo/evento, auditoría fallida y éxito simulado. Ejecutar MSBuild y pruebas focales; no E2E mutante sin autorización explícita.

## DOCUMENTACIÓN TÉCNICA

Actualizar `02-contrato.md`, `03-flujo-y-seguridad.md` y `04-pruebas-y-evidencia.md` con el punto mutante, parámetros, auditoría, estados/error y reglas legacy preservadas.

## ENTREGABLE FINAL

Reportar ticket, archivos, pruebas, compilación, trazabilidad sanitizada y confirmación de no regresión de Continuar flujo. No implementar UI ni cambiar configuración de ambiente.
