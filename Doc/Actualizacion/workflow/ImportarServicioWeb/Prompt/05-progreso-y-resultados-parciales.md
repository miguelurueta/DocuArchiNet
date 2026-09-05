# Prompt 05 — Progreso real y resultados parciales

Integra el recorrido moderno con `js/java_general/JSProgresBar.js` conservando todos sus consumidores existentes.

## Objetivo

Presentar fases globales y resultado por elemento mediante un adaptador, sin duplicar el ejecutor compartido ni inventar porcentajes.

## Implementa

- `ImportarServicioWebProgressAdapter` con estado por clave externa, estado normalizado, código legacy y mensaje seguro.
- Callbacks genéricos, opcionales y retrocompatibles solo si son indispensables: inicio, elemento iniciado, progreso, resultado, decisión y finalización.
- Traducción: `YES` a Guardada; `CTRL` a Omitida o No procesada según causa; `CTRLRETURN` a Requiere decisión; otros valores a Fallida conservando la detención vigente.
- En múltiple, **Continuar con las demás** y **Detener importación** ante `CTRLRETURN`.
- Resumen con guardadas, omitidas, fallidas y no procesadas.

## Restricciones

- No copies ni especialices `JSProgresBar` para SII.
- No cambies orden, retornos, `estado_control`, pausas, cancelación o selección por `name_service` sin prueba de compatibilidad.
- No ofrezcas **Reintentar fallidos** en esta entrega.
- No anuncies éxito total cuando exista cualquier resultado distinto de Guardada.

## Aceptación

- Individual y múltiple usan el mismo ejecutor con colecciones de diferente cardinalidad.
- Los códigos internos no aparecen en la interfaz.
- Existen pruebas de regresión de los consumidores compartidos afectados.

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

Agregar regla para [CONTRACT_DETAIL_REQUIRED]: Props, callbacks, eventos, request/response, payloads o tipos documentados.

Agregar regla para [ANTI_REGRESSION_DETAIL_REQUIRED]: Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.
