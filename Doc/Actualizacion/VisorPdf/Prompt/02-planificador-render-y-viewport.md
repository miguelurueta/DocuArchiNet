# Prompt 02 — Planificador de render, cancelación, scroll y zoom

## Contexto de ejecución OPSXJ

Trabaja exclusivamente en el ticket OPSXJ recibido. No ejecutes `opsxj:new`. Verifica primero los artefactos e implementación reales del Prompt 01 y crea el OpenSpec autónomo de esta Jira.

## Rol esperado

Actúa como especialista senior en JavaScript, PDF.js, rendimiento del navegador y coordinación asíncrona.

## Fuentes obligatorias

- `../Exploracion/01-diagnostico-rendimiento-visor-pdf-grafos.md`.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`, especialmente `_visiblePages`, `loadPages`, `_loadingTask`, `_renderPage`, `_cleanPage`, `setZoom` y eventos de scroll.
- `pdfjs/pdf_anotate_view/script.js`.
- Evidencia y pruebas del Prompt 01.

## Objetivo

Eliminar renderizados duplicados u obsoletos y reducir el bloqueo del hilo principal durante scroll y zoom, conservando el resultado visual, página activa, anotaciones y contratos públicos.

## Diseño obligatorio

Documenta en `design.md` una máquina de estados explícita:

```text
idle -> queued -> rendering -> rendered
                 |              |
                 +-> cancelled <-+
```

Define identidad de trabajo por página, generación de zoom/rotación, deduplicación y política de cancelación. Una página no puede tener dos `RenderTask` efectivos de la misma generación.

## Implementación obligatoria

1. Corregir `_loadingTask()` para mantener el estado ocupado hasta que la promesa de render finalice o falle.
2. Hacer que `_renderPage()` y la cadena llamadora retornen promesas verificables.
3. Deduplicar páginas pendientes y eliminar entradas inválidas.
4. Conservar y cancelar `RenderTask` de PDF.js cuando una página deja de ser necesaria o cambia la generación de zoom/rotación.
5. Evitar que una finalización antigua reemplace el canvas de una generación nueva.
6. Sustituir el escaneo costoso de todas las páginas por observación de viewport o una estrategia equivalente demostrablemente acotada; conservar páginas adyacentes configurables.
7. Aplicar debounce/coalescencia a scroll y acciones rápidas de zoom sin volver lenta la primera interacción.
8. Separar lecturas y escrituras de layout y limitar trabajo por frame.
9. Mantener página activa, posición de scroll, grafos locales y rotación.

## Restricciones y antirregresión

- No cambiar el handler, el WebMethod, iTextSharp ni la persistencia.
- No cambiar identificadores DOM, botones o firmas públicas consumidas por `script.js`.
- No eliminar render diferido ni renderizar todas las páginas para simplificar.
- No usar esperas fijas como mecanismo de sincronización.
- No introducir workers o frameworks adicionales sin demostrar necesidad y compatibilidad.

## Pruebas obligatorias

- Cola vacía, una página, duplicados, múltiples páginas y error de `getPage`/render.
- Cancelación por salida de viewport, zoom rápido y rotación.
- Finalización tardía de una generación obsoleta.
- Conservación de página activa y scroll.
- E2E de lectura autorizada con el PDF pesado y comparación contra línea base.
- Presupuesto inicial: reducir materialmente los 3.548 ms de tareas largas del scroll y la tarea de 435 ms del zoom. El porcentaje final debe justificarse con evidencia, no fijarse ocultando eventos.

## Criterios de aceptación

- No hay renderizados efectivos duplicados por página/generación.
- La cola permanece ocupada hasta finalizar y recupera estado tras error/cancelación.
- Scroll y zoom mejoran respecto a la línea base sin alterar la imagen ni los grafos.
- El error de una página no bloquea permanentemente las demás.

## Documentación y entrega

Documenta estado, invariantes, firmas, pruebas y métricas en `Doc/Actualizacion/VisorPdf/Implementacion/<JIRA>/`. Entrega diff focalizado, comandos, resultados y pendientes reales.

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
Ejecutar pruebas Node focales, validación sintáctica JavaScript y E2E con Playwright; ejecutar MSBuild únicamente si se toca código compilado y registrar comandos y resultados.

## Documentacion tecnica
Actualizar el paquete documental canonico del ticket.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

## Reglas de ubicacion de codigo
- Mantener el planificador y estado del visor en `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js` y la coordinación UI existente en `pdfjs/pdf_anotate_view/script.js`.
- Mantener las pruebas y mediciones en `tools/e2e/`; no crear otro proyecto Playwright ni una aplicación frontend paralela.
- Antes de crear archivos de producción, demostrar que la responsabilidad no pertenece a uno de los módulos existentes.

Agregar regla para [CONTRACT_DETAIL_REQUIRED]: Props, callbacks, eventos, request/response, payloads o tipos documentados.

Exigir validación sintáctica JavaScript, pruebas Node focales y, si se modifica código compilado, MSBuild; registrar el resultado.

Exigir pruebas focales con la infraestructura Node existente y dobles controlados de PDF.js/DOM cuando aplique; no introducir Vitest ni Testing Library solo para esta tarea.

Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o `.env` paralelos.

Antes de autenticar, exigir lectura de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.

Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo `SELECT` y conservar evidencia saneada.

Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Ruta documental obligatoria
La documentacion debe quedar en una ruta canonica segun el contexto:

Usar `Doc/Actualizacion/VisorPdf/Implementacion/<JIRA>/`, sustituyendo `<JIRA>` por la clave OPSXJ real recibida. No inventar un identificador distinto.

## Paquete documental minimo
Generar como minimo:

```txt
00-Indice.md
01-Arquitectura.md
02-FlujoIntegracion.md
03-ContratoUploadYMapping.md
04-EstadosErroresYAntiregresion.md
05-PruebasEvidencia.md
06-Diagramas.md
07-Metadata.md
```

00-Indice.md debe incluir objetivo, alcance, componentes, hooks/adapters/servicios, modulos, dependencias y listado documental.

01-Arquitectura.md debe explicar decisiones arquitectonicas, reutilizacion, responsabilidades, desacople, alternativas descartadas, componentes de presentacion, contenedores, servicios, adapters, mappers, hooks e infraestructura.

02-FlujoIntegracion.md debe cubrir usuario, renderizado, carga de datos, requests, backend, responses, estado, interfaz UI y batch/lote si aplica.

03-ContratoUploadYMapping.md debe documentar props, contexto, DTOs, request, response, modelos, transformacion/mapping, deduplicacion, metadata y frontera frontend/backend.

04-EstadosErroresYAntiregresion.md debe cubrir estado inicial, carga/loading, exito, errores, datos incompletos, estados parciales, respuestas invalidas, antirregresion, remount, refresh, recargas silenciosas, duplicacion, logica heredada y soluciones temporales.

05-PruebasEvidencia.md debe listar pruebas unitarias, integracion, manuales, comandos, resultados, limitaciones, riesgos y evidencia.

06-Diagramas.md debe incluir componentes, secuencia, flujo principal, flujo alterno, casos de uso, estados y Mermaid o formato estructurado legible.

07-Metadata.md debe consolidar la clave OPSXJ real, rama, fecha, estado, archivos modificados, prompt, dependencias, riesgos y deuda técnica.

Crear carpeta `Diagramas/` dentro del paquete documental para diagramas individuales.

## Tabla de funciones creadas o modificadas
| Funcion | Ruta | Ubicacion | Parametros | Responsabilidad |
| --- | --- | --- | --- | --- |
| `<nombre>` | `<path>` | `<componente/hook/service/adapter>` | `<params>` | `<responsabilidad>` |
