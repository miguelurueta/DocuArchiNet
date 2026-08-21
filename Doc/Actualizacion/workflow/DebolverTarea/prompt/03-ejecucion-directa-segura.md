# 03 — Ejecución directa segura

## ROL ESPERADO

Actúa como desarrollador senior de casos de uso Workflow, concurrencia MySQL y encapsulación de motores existentes.

## OBJETIVO

Implementar el adaptador exclusivo de devolución, `ServicioDevolucionActividadTarea`, auditoría y `EjecutarDevolverActividad`.

## CONTEXTO OBLIGATORIO

- Requiere 02 aprobado y contratos/preview focales en verde.
- Leer `00-contexto-obligatorio.md` y evidencia de 02.
- La salida habilita 04 solo si ejecución, lock y revalidaciones pasan sin E2E ni tarea real.

## REQUISITOS POSITIVOS

- Aceptar solo `{ IdTarea, IdConector, TokenVersion }` en el endpoint de ejecución.
- Dentro de `GET_LOCK`, releer y validar contexto, permiso de devolución, tarea, token, Ruta o Flujo, y conector entrante real.
- Reconstituir el destino desde el conector validado y no desde atributos o campos del cliente.
- Crear puerto y adaptador exclusivos que invoquen `Terminar_Tarea_Workflow` con `Page = Nothing`, sin handlers Web Forms.
- Normalizar éxito, bloqueo, error reintentable y advertencias; auditar sin datos sensibles con un mecanismo distinguible de devolución moderna.

## RESTRICCIONES CRÍTICAS

- No invocar postbacks, `ModalPopupExtender`, `GridView`, `UpdatePanel`, handlers Web Forms ni ejecutores de otras operaciones.
- No alterar `ServicioTransicionTarea`, `ServicioEnvioGrupoTarea`, `EjecutarEnvioTarea`, `EjecutarEnvioGrupo` ni Usuario anterior.
- No consultar, bloquear, reasignar, actualizar ni auditar respuestas; no referenciar sus componentes.
- Un token vencido, permiso retirado, conector inválido o lock ocupado bloquea antes del motor.

## REGLAS DE ANTIRREGRESIÓN

- Una falla de auditoría no revierte una transición ya confirmada.
- Dos solicitudes concurrentes no devuelven dos veces la tarea.
- Continuar flujo conserva su contrato de conectores salientes.

## CRITERIOS DE ACEPTACIÓN

- Existe un único punto mutante de devolución y el navegador no autoriza el destino.
- La ejecución usa exclusivamente un conector entrante revalidado.
- Los errores públicos no filtran SQL, sesión ni excepciones.

## PRUEBAS OBLIGATORIAS

Cubrir solicitud inválida, permiso, tarea inactiva, token vencido, conector retirado o manipulado, Ruta/Flujo inconsistente, lock ocupado, concurrencia, advertencia, auditoría fallida, éxito simulado y ausencia de referencias a respuestas. Ejecutar MSBuild y pruebas focales; no E2E mutante sin autorización.

## DOCUMENTACIÓN TÉCNICA

Actualizar contrato, flujo de seguridad y evidencia con el punto mutante, parámetros, auditoría, estados y reglas preservadas.

## ENTREGABLE FINAL

Reportar ticket, archivos, pruebas, compilación, trazabilidad sanitizada y confirmación de no regresión. No implementar UI ni cambiar configuración de ambiente.
