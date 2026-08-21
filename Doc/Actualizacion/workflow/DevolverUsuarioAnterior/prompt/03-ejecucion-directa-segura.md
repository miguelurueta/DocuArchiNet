# 03 — Ejecución directa segura

## ROL ESPERADO

Actúa como desarrollador senior de casos de uso Workflow, concurrencia MySQL y encapsulación de motores existentes.

## OBJETIVO

Implementar el adaptador exclusivo de devolución a usuario anterior, `ServicioDevolucionUsuarioAnteriorTarea`, auditoría y `EjecutarDevolverUsuarioAnterior`.

## CONTEXTO OBLIGATORIO

- Requiere 02 aprobado y preview focal en verde.
- Leer `00-contexto-obligatorio.md` y evidencia de 02.
- La salida habilita 04 solo si ejecución, lock y revalidaciones pasan sin E2E ni tarea real.

## REQUISITOS POSITIVOS

- Aceptar solo `{ IdTarea, TokenVersion }` en el endpoint de ejecución.
- Dentro de `GET_LOCK`, releer y validar contexto, permiso de devolución, tarea, token, Ruta o Flujo y registro histórico anterior.
- Obtener usuario y actividad destino exclusivamente del historial revalidado en servidor.
- Validar que el usuario histórico sea positivo, elegible y distinto del usuario Workflow autenticado real.
- Crear puerto y adaptador exclusivos que invoquen `Terminar_Tarea_Workflow` con `Page = Nothing`, sin handlers Web Forms.
- Normalizar éxito, bloqueo, error reintentable y advertencias; auditar sin datos sensibles con un mecanismo distinguible de devolución moderna.

## RESTRICCIONES CRÍTICAS

- No invocar postbacks, `ModalPopupExtender`, `GridView`, `UpdatePanel`, handlers Web Forms ni ejecutores de otras operaciones.
- No alterar servicios de Devolver a actividad anterior, Continuar flujo, Enviar a usuario, Enviar a grupo ni Usuario anterior legado antes de su sustitución aprobada.
- No recibir o resolver destinos alternativos, grupos o conectores entrantes/salientes.
- No consultar, bloquear, reasignar, actualizar ni auditar respuestas; no referenciar sus componentes.
- No usar `Id_Ruta_Workflow` para validar auto-devolución.
- Un token vencido, historial ausente, usuario no válido, permiso retirado o lock ocupado bloquea antes del motor.

## REGLAS DE ANTIRREGRESIÓN

- Una falla de auditoría no revierte una transición ya confirmada.
- Dos solicitudes concurrentes no devuelven dos veces la tarea.
- La operación no abre ni llama a Devolver a actividad anterior.

## CRITERIOS DE ACEPTACIÓN

- Existe un único punto mutante y el navegador no autoriza el destino.
- La validación de auto-devolución compara el usuario histórico con el usuario Workflow autenticado.
- Los errores públicos no filtran SQL, sesión ni excepciones.

## PRUEBAS OBLIGATORIAS

Cubrir solicitud inválida, permiso, tarea inactiva, historial ausente, historial de grupo, usuario retirado, auto-devolución, token vencido, lock ocupado, concurrencia, advertencia, auditoría fallida, éxito simulado y ausencia de referencias a respuestas. Ejecutar MSBuild y pruebas focales; no E2E mutante sin autorización.

## DOCUMENTACIÓN TÉCNICA

Actualizar contrato, flujo de seguridad y evidencia con punto mutante, parámetros, auditoría, estados y corrección de auto-devolución.

## ENTREGABLE FINAL

Reportar ticket, archivos, pruebas, compilación, trazabilidad sanitizada y confirmación de no regresión. No implementar UI ni cambiar configuración de ambiente.
