# 02 — Autorización y preview de historial

## ROL ESPERADO

Actúa como desarrollador senior VB.NET de capas Domain, Application e Infrastructure para Workflow.

## OBJETIVO

Implementar el corte de servidor: autorización de devolución y `PreviewDevolverUsuarioAnterior` de solo lectura sobre el historial de la tarea.

## CONTEXTO OBLIGATORIO

- Requiere 01 aprobado y lectura de `00-contexto-obligatorio.md`.
- La salida habilita 03; no habilita ejecución, UI ni liberación.

## REQUISITOS POSITIVOS

- Crear DTOs, modelos, puertos y códigos públicos exclusivos de devolución a usuario anterior.
- Exponer `PreviewDevolverUsuarioAnterior(idTarea)` en el ASMX moderno existente.
- Validar contexto, tarea activa, permiso de devolución, Ruta o Flujo y accesibilidad antes de devolver resultados.
- Consultar el historial de la misma tarea con `SELECT` parametrizado y obtener únicamente el registro inmediatamente anterior.
- Validar que el registro anterior tenga usuario positivo, usuario elegible y actividad/Ruta/Flujo consistentes.
- Devolver datos mínimos para confirmación: actividad, usuario resumido, token de versión y bloqueo funcional.

## RESTRICCIONES CRÍTICAS

- No ejecutar motor, lock, auditoría, eventos ni cambios de tarea o datos de negocio.
- No crear búsqueda, paginación, selector de actividades, banderas de habilitación ni lista de destinos en cliente.
- No modificar endpoints de Devolver a actividad anterior, Enviar a usuario, Enviar a grupo ni Continuar flujo.
- No tratar respuestas en ningún caso.
- No ejecutar E2E autenticada ni carga sin autorización explícita.

## REGLAS DE ANTIRREGRESIÓN

- El preview no revela registros de otras tareas ni destinos alternativos.
- Historial ausente, grupo, usuario retirado o auto-devolución devuelven resultado público seguro y sin escritura.

## CRITERIOS DE ACEPTACIÓN

- El preview devuelve cero o un destino histórico de usuario; nunca un grupo o actividad alternativa.
- Tarea inválida, permiso ausente, Ruta/Flujo inconsistente o historial inválido devuelven bloqueo funcional estable.
- La respuesta no incluye datos de respuesta, SQL, sesión, HTML ni controles Web Forms.

## PRUEBAS OBLIGATORIAS

Agregar pruebas focales para permiso, preview sin escritura, historial válido, historial ausente, grupo, usuario retirado, auto-devolución y `SELECT` parametrizado. Ejecutar compilación disponible y pruebas afectadas; no E2E.

## DOCUMENTACIÓN TÉCNICA

Actualizar arquitectura, contrato, flujo de seguridad y evidencia del paquete documental de DevolverUsuarioAnterior.

## ENTREGABLE FINAL

Reportar ticket, archivos, pruebas, compilación, evidencia de no escritura y riesgos. No implementar adaptador, ejecución ni UI.
