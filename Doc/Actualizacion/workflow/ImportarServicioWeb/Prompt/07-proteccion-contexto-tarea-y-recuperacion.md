# Prompt 07 — Protección del contexto de tarea y recuperación

Implementa las defensas de interfaz y los contratos requeridos para que una importación nunca cambie silenciosamente de tarea.

Depende de los contratos publicados por B01, B03, B04 y B05. El frontend representa conflictos y recuperación; el backend conserva la autoridad sobre intención, tarea y ejecución.

## Objetivo

Vincular toda ejecución a una intención inmutable y reconciliar siempre contra su tarea original.

## Rutas canónicas de implementación

```txt
js/workflow/importar-servicio-web/
├── importar-servicio-web-task-context-guard.js
└── importar-servicio-web-recovery.js

Tests/
├── importar-servicio-web-task-context-guard.test.cjs
├── importar-servicio-web-recovery.test.cjs
└── importar-servicio-web-multi-tab-context.test.cjs
```

- El guard se integra mediante eventos públicos del núcleo y la página; no sobrescribe selectores o handlers globales.
- `recovery.js` consulta la intención por `importar-servicio-web-api.js`; no persiste autoridad en `localStorage` ni reconstruye contexto desde sesión cliente.
- Los cambios aditivos de atributos/estado de controles se realizan en `workflow/Webworkflow.aspx` y módulos del feature.
- No modificar endpoints, almacenamiento, scripts globales o acciones Workflow existentes.

## Ruta documental obligatoria

```txt
docs/modulos/workflow/importar-servicio-web/SCRUMCORE-000-proteccion-contexto-recuperacion/
```

Sustituir `SCRUMCORE-000` por el ticket real; crear el paquete canónico y `Diagramas/` exclusivamente allí.

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
- El frontend no ejecuta mutaciones por elemento ni reconstruye una intención a partir de la sesión.
- No modifiques `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` o endpoints legacy.
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
Leer F01–F06, B01/B03/B04/B05, `workflow/Webworkflow.aspx`, módulos modernos de transición y eventos actuales de selección/búsqueda de tareas. Inspeccionar sin modificar los handlers legacy.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, con matriz de acciones bloqueadas, cambio de tarea/pestaña, recuperación, estados y pruebas.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.
