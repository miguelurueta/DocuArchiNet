# Prompt 01 — Observabilidad y corrección del error JavaScript del visor PDF

## Contexto de ejecución OPSXJ

Este prompt se ejecuta dentro de un ticket OPSXJ ya creado y revisado. No ejecutes `opsxj:new`, no abras otro ticket y no crees un cambio OpenSpec paralelo. Genera o continúa los artefactos Spec-Driven de esta Jira antes de modificar código.

## Rol esperado

Actúa como arquitecto senior y especialista en JavaScript, Playwright, PDF.js y ASP.NET WebForms. Trabaja con evidencia reproducible y no atribuyas causas a librerías sin capturar el fallo real.

## Fuentes obligatorias

Lee completamente:

- `../Exploracion/01-diagnostico-rendimiento-visor-pdf-grafos.md`.
- `pdfjs/pdf_anotate_view/index.html`.
- `pdfjs/pdf_anotate_view/script.js`.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`.
- `tools/e2e/scripts/run-workflow-pdf-viewer-performance-interactive.cjs`.
- `tools/e2e/tests/workflow-pdf-viewer-performance-policy.test.cjs`.
- `tools/e2e/AGENT-RUNBOOK.md`.

La línea base autoritativa actual es: PDF de 34 páginas y 33.589.755 bytes, primera carga 2.902 ms, mediana 878 ms, scroll 3.902 ms con 3.548 ms en tareas largas, zoom 779 ms con 435 ms bloqueantes y un error JavaScript por iteración.

## Objetivo

Clasificar y corregir el error JavaScript repetible observado durante cada carga y consolidar una medición reproducible que diferencie red, arranque, render, scroll y zoom sin revelar información documental.

## Precondiciones de rutas y diseño

En `design.md`, inventaría antes de editar:

- listeners y puntos de inicialización activos del visor;
- scripts realmente cargados y su orden;
- símbolos globales y léxicos consumidos;
- eventos `error`, `unhandledrejection`, respuestas HTTP y fases medidas;
- archivos exactos de `tools/e2e` y `pdfjs/pdf_anotate_view` afectados.

No crees un servicio backend, DTO, repositorio o tabla para telemetría. Esta tarea usa artefactos locales saneados y pruebas existentes.

## Implementación obligatoria

1. Extender el runner para clasificar errores mediante código estable, tipo, archivo base y línea, eliminando URL, query string, rutas, mensajes libres y datos del documento.
2. Distinguir errores del frame principal, visor, worker y recursos opcionales.
3. Identificar con evidencia el error observado en las tres iteraciones y corregir su causa mínima en el archivo propietario.
4. Conservar hitos visibles de selección, primer canvas, scroll, zoom y recargas.
5. Separar `PASSED`, `PASSED_WITH_OBSERVATIONS` y fallo funcional; una medición completada con errores no debe declararse silenciosamente limpia.
6. Mantener el artefacto JSON sin tarea, usuario, título documental, URL, ruta, cookie o contenido.

## Restricciones y antirregresión

- No cambiar todavía la cola, streaming, dependencias o guardado de grafos salvo lo indispensable para corregir el error probado.
- No añadir logs productivos con excepciones crudas.
- No depender de `window.pdfViewer`: el visor declara `let pdfViewer` y la prueba debe respetar el alcance real.
- No guardar capturas del documento ni trazas que incluyan su contenido.
- No convertir errores en `catch` vacíos para hacer pasar la prueba.

## Pruebas obligatorias

- Contratos unitarios del perfil, sanitización, clasificación y estados finales.
- Prueba que demuestre que ningún artefacto contiene URL, query, ruta, tarea, documento o credencial.
- Ejecución de `test:workflow:pdf-viewer-performance:policy`.
- E2E de lectura sobre PDF autorizado, solo con autorización vigente, comparando tres iteraciones con la línea base.
- Si la E2E no se autoriza, documentar el bloqueo y no afirmar que el error desapareció en ambiente real.

## Criterios de aceptación

- El error repetible tiene código, origen y causa demostrados.
- La corrección mínima elimina el error sin degradar carga, scroll o zoom.
- La evidencia distingue medición válida con observaciones de una corrida limpia.
- La suite contractual y las regresiones del visor terminan sin fallos.

## Documentación técnica

Actualizar la exploración y crear evidencia específica en `Doc/Actualizacion/VisorPdf/Implementacion/<JIRA>/`, incluyendo causa, símbolos modificados, métricas antes/después, comandos y limitaciones.

## Entregable final

Entrega código, pruebas, evidencia saneada y trazabilidad D/RQ/tareas. No cierres el OpenSpec si el error solo fue ocultado o si no puede reproducirse y explicarse.

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

## Precondiciones de rutas
- Inventariar en `design.md` los archivos exactos y las funciones JavaScript que se modificarán.
- La instrumentación del visor debe permanecer en `pdfjs/pdf_anotate_view/` y la automatización en `tools/e2e/`; no crear capas, DTOs, repositorios ni servicios que este diagnóstico no necesita.
- Si el error conduce a una corrección de servidor, verificar primero la ruta real y limitar el cambio al handler o ASMX existente que origina el fallo.
- No crear un visor, arnés Playwright, configuración ni mecanismo de autenticación paralelos.

Agregar regla para [CONTRACT_DETAIL_REQUIRED]: Props, callbacks, eventos, request/response, payloads o tipos documentados.

Agregar regla para [FLOW_DETAIL_REQUIRED]: Flujo paso a paso, secuencia o comportamiento esperado.

Agregar regla para [EXISTING_DOC_UPDATE_REQUIRED]: Ubicar documentacion existente, actualizarla o documentar la ausencia/ruta requerida.

Ejecutar la validación sintáctica JavaScript, las pruebas Node focales de `tools/e2e` y la compilación MSBuild del proyecto solo si se modifica código compilado; registrar comandos, resultados y bloqueos reales.

Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o `.env` paralelos.

Antes de autenticar, exigir lectura de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.

Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo `SELECT` y conservar evidencia saneada.

Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.
