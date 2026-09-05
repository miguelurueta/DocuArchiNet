# Prompt 07 — Protección del contexto de tarea y recuperación

Implementa las defensas de interfaz y los contratos requeridos para que una importación nunca cambie silenciosamente de tarea.

## Objetivo

Vincular toda ejecución a una intención inmutable y reconciliar siempre contra su tarea original.

## Implementa

- Preflight inmediatamente anterior al primer efecto.
- Intención con operación, tarea, ruta, usuario autenticado, proveedor, identidades externas y fecha de inicio.
- Bloqueo de selección/búsqueda de tareas, continuar flujo, devolver, cerrar y demás acciones incompatibles durante escrituras.
- **Detener importación** con explicación de que no revierte efectos ya confirmados.
- Conflicto `TASK_CONTEXT_CHANGED` si la sesión deja de coincidir mientras los endpoints legacy aún dependen de ella.
- Consulta y recuperación de la intención después de recarga, cierre forzado o pérdida de conexión.
- Actualización de documentos solo cuando la vista corresponde a la tarea original.

## Restricciones

- `beforeunload` es solo advertencia, no garantía.
- La sesión no puede sustituir la tarea ligada a la intención.
- Cada endpoint mutador debe volver a validar usuario, tarea, ruta, proveedor e identidad.
- Si falta el contrato backend, no presentes la protección UX como garantía de integridad.

## Aceptación

- Las pruebas cubren cambio de tarea en la misma pestaña y en otra pestaña simulada.
- El conflicto detiene los pendientes y conserva los resultados anteriores.
- La recuperación muestra estado verificable y no reintenta ciegamente.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Listar archivos, modulos, servicios, hooks, adapters y documentacion que deben leerse antes de implementar.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar el paquete documental canonico del ticket.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.
