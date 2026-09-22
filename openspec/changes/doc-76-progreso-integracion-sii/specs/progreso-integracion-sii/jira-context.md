# Jira Context - DOC-76

## Summary

PROGRESO-INTEGRACION-SII

## Description

> # Prompt 05 — Espera global y resultados por elemento
> 
> Implementa la presentación del recorrido moderno dentro de los módulos y estilos propios del feature. `js/java_general/JSProgresBar.js` y todos sus consumidores legacy permanecen intactos y no son una dependencia de esta pantalla. Depende de `ExecuteImportIntent` y `GetImportIntent` publicados por el Prompt backend 04.
> 
> ## Objetivo
> 
> Presentar una espera global durante la única ejecución síncrona y, al finalizar, los resultados por elemento confirmados por el orquestador backend, sin convertir el navegador en un segundo ejecutor ni inventar porcentajes.
> 
> ## Rutas canónicas de implementación
> 
> ```txt
> js/workflow/importar-servicio-web/
> ├── importar-servicio-web-progress-adapter.js
> └── importar-servicio-web-progress-view.js
> 
> Tests/
> ├── importar-servicio-web-progress-adapter.test.cjs
> ├── importar-servicio-web-progress-state-mapping.test.cjs
> └── importar-servicio-web-progress-legacy-regression.test.cjs
> ```
> 
> - `progress-adapter.js` consume la respuesta de `ExecuteImportIntent` y puede confirmar el snapshot final mediante `GetImportIntent`; no ejecuta mutaciones.
> - `progress-view.js` presenta un indicador indeterminado accesible mediante HTML/CSS del feature, sin importar, instanciar, adaptar ni invocar `js/java_general/JSProgresBar.js`.
> - No crear `JSProgresBarSII`, copiar la barra, inventar porcentajes, usar temporizadores de progreso ni agregar handlers inline.
> - Registrar módulos nuevos en el `.vbproj`; estilos adicionales permanecen en `Styles/importar-servicio-web-modern.css`.
> 
> ## Ruta documental obligatoria
> 
> ```txt
> Doc/Actualizacion/workflow/ImportarServicioWeb/SCRUMCORE-000-progreso-resultados-parciales/
> ```
> 
> Sustituir `SCRUMCORE-000` por el ticket real; crear el paquete canónico y `Diagramas/` únicamente allí.
> 
> ## Implementa
> 
> - `ImportarServicioWebProgressAdapter` con estado estructurado por clave externa, fase backend, estado visible y mensaje seguro.
> - Una sola llamada a `ExecuteImportIntent` por intención, tanto para uno como para varios elementos.
> - Espera global indeterminada mientras la llamada está pendiente, sin porcentaje ni avance individual simulado.
> - Al finalizar, adaptación de `response.Items` y presentación de un resultado independiente por elemento.
> - Aplicación exacta del mapeo de estados definido en `../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`.
> - Resumen con guardadas, omitidas, fallidas y no procesadas.
> - No realizar polling durante la llamada síncrona; `GetImportIntent` se reserva para timeout, pérdida de respuesta o reapertura autorizada.
> 
> ## Restricciones
> 
> - No copies, especialices, importes ni invoques `JSProgresBar` desde el feature moderno. Es infraestructura exclusivamente legacy.
> - En el recorrido moderno, el único ejecutor es `ImportServiceOrchestrator`; el indicador local solo refleja solicitud pendiente y respuesta final.
> - No interpretes `YES`, `CTRL`, `CTRLRETURN` ni `dato_lista`; esa traducción pertenece exclusivamente al adaptador backend.
> - No realices una llamada por inscripción, no consultes nuevamente SII por elemento y no fragmentes la intención para simular progreso.
> - No cambies orden, retornos, `estado_control`, pausas, cancelación o selección por `name_service` de los consumidores legacy.
> - No modifiques `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` ni sus consumidores.
> - No ofrezcas **Reintentar fallidos** en esta entrega.
> - No anuncies éxito total cuando exista cualquier resultado distinto de Guardada.
> - Una vez iniciada `ExecuteImportIntent`, la primera entrega no ofrece cancelación en curso; cerrar el modal no cancela ni revierte la operación.
> 
> ## Aceptación
> 
> - Individual y múltiple usan una sola intención y una sola invocación al mismo orquestador backend con colecciones de diferente cardinalidad.
> - La espera termina únicamente con respuesta o fallo de transporte; después se muestran todos los resultados disponibles.
> - Los códigos legacy no llegan al frontend moderno ni aparecen en la interfaz.
> - Existen pruebas de invariancia que demuestran que `JSProgresBar` y sus consumidores legacy no fueron modificados ni invocados por el feature moderno.
> 
> ## Correcciones opsxj:prompt-review
> 
> Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.
> 
> ## Rol esperado
> Definir el rol tecnico esperado para ejecutar el ticket.
> 
> ## Objetivo
> Describir el objetivo funcional y tecnico verificable.
> 
> ## Restricciones criticas
> - No introducir cambios fuera del alcance declarado.
> - No romper comportamiento existente ni contratos publicos.
> 
> ## Criterios de aceptacion
> - El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.
> 
> ## Contexto obligatorio
> Leer F01–F04, B04, el mapa normativo, `js/java_general/JSProgresBar.js` y sus consumidores identificados. Inspeccionarlos únicamente para demostrar aislamiento e invariancia; no convertirlos en dependencia del feature moderno.
> 
> ## Pruebas obligatorias
> Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.
> 
> ## Documentacion tecnica
> Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, con propiedad de ejecución, mapeo de estados, decisiones, regresión legacy y diagramas.
> 
> ## Entregable final
> Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.
> 
> Agregar regla para [CONTRACT_DETAIL_REQUIRED]: Props, callbacks, eventos, request/response, payloads o tipos documentados.
> 
> Agregar regla para [ANTI_REGRESSION_DETAIL_REQUIRED]: Reglas explicitas de no romper, preservar, no llamar o no usar workarounds.
> 
> Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.
> 
> Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: INTEGRACION, INTERFAZ, PROGRESO
