# Prompt 05 — Progreso real y resultados parciales

Integra la presentación del recorrido moderno con `js/java_general/JSProgresBar.js` conservando todos sus consumidores existentes. Depende de `ExecuteImportIntent` y `GetImportIntent` publicados por el Prompt backend 04.

## Objetivo

Presentar fases globales y resultados por elemento confirmados por el orquestador backend, sin convertir el navegador en un segundo ejecutor ni inventar porcentajes.

## Rutas canónicas de implementación

```txt
js/workflow/importar-servicio-web/
├── importar-servicio-web-progress-adapter.js
└── importar-servicio-web-progress-view.js

Tests/
├── importar-servicio-web-progress-adapter.test.cjs
├── importar-servicio-web-progress-state-mapping.test.cjs
└── importar-servicio-web-progress-legacy-regression.test.cjs
```

- `progress-adapter.js` consume `GetImportIntent` mediante `importar-servicio-web-api.js` y aplica el mapa contractual; no ejecuta mutaciones.
- `progress-view.js` integra presentación sin modificar `js/java_general/JSProgresBar.js` ni sus consumidores.
- No crear `JSProgresBarSII`, una copia de la barra, temporizadores de progreso o handlers inline.
- Registrar módulos nuevos en el `.vbproj`; estilos adicionales permanecen en `Styles/importar-servicio-web-modern.css`.

## Ruta documental obligatoria

```txt
docs/modulos/workflow/importar-servicio-web/SCRUMCORE-000-progreso-resultados-parciales/
```

Sustituir `SCRUMCORE-000` por el ticket real; crear el paquete canónico y `Diagramas/` únicamente allí.

## Implementa

- `ImportarServicioWebProgressAdapter` con estado estructurado por clave externa, fase backend, estado visible y mensaje seguro.
- Adaptación de eventos confirmados por `GetImportIntent`; inicio, elemento iniciado, progreso, resultado, decisión y finalización son notificaciones, no órdenes mutadoras.
- Aplicación exacta del mapeo de estados definido en `../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`.
- En múltiple, **Continuar con las demás** y **Detener importación** ante `CTRLRETURN`.
- Resumen con guardadas, omitidas, fallidas y no procesadas.

## Restricciones

- No copies ni especialices `JSProgresBar` para SII.
- En el recorrido moderno, `JSProgresBar` no invoca guardado, expediente, índices, caché ni almacenamiento por elemento; el único ejecutor es `ImportServiceOrchestrator`.
- No interpretes `YES`, `CTRL`, `CTRLRETURN` ni `dato_lista`; esa traducción pertenece exclusivamente al adaptador backend.
- No cambies orden, retornos, `estado_control`, pausas, cancelación o selección por `name_service` de los consumidores legacy.
- No modifiques `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` ni sus consumidores.
- No ofrezcas **Reintentar fallidos** en esta entrega.
- No anuncies éxito total cuando exista cualquier resultado distinto de Guardada.

## Aceptación

- Individual y múltiple usan el mismo orquestador backend con colecciones de diferente cardinalidad.
- Los códigos legacy no llegan al frontend moderno ni aparecen en la interfaz.
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
Leer F01–F04, B04, el mapa normativo, `js/java_general/JSProgresBar.js` y sus consumidores identificados. La barra y consumidores existentes son referencia de compatibilidad y no se modifican.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, con propiedad de ejecución, mapeo de estados, decisiones, regresión legacy y diagramas.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

Agregar regla para [CONTRACT_DETAIL_REQUIRED]: Props, callbacks, eventos, request/response, payloads o tipos documentados.

Agregar regla para [ANTI_REGRESSION_DETAIL_REQUIRED]: Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.

Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.
