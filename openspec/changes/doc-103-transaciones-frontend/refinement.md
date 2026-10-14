<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-103-transaciones-frontend

## Fuente y alcance

- Ticket: `DOC-103` — TRANSACIONES-FRONTEND.
- Cambio OpenSpec: `doc-103-transaciones-frontend`.
- Fuentes normativas: Jira, Prompt 05, exploración consolidada y contratos implementados por DOC-100 a DOC-102.
- Perfil tecnológico confirmado: JavaScript legacy en visor WebForms, jQuery AJAX, PDF.js y WebMethod ASMX existente.
- Alcance: transacción cliente de guardado del grafo. No modifica iTextSharp, temporales, persistencia, DTO ni firma del WebMethod.

## Contexto inspeccionado

- `pdfjs/pdf_anotate_view/index.html:183-184`: carga `pdf_viewer_anotate.js`; `pdfannotate.js` permanece comentado y no es la ruta productiva.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:1289-1399`: `event_drag_anotate(...)` interpreta el retorno inmediato de `save_anotate_firma(...)` y elimina el overlay antes de `Image.onload` y del POST.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:1508-1569`: `service_anotate_pdf_image(...)` no retorna el `jqXHR`, no declara timeout, mezcla `data.d` escalar/colección y expone `responseText`/excepciones mediante `alert`.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:252-277`: `PdfViwerAlert(...)` es el componente de aviso ya utilizado por el visor.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:1015-1022`: `_updatePage(...)` obtiene la página actualizada, pero no espera `_renderPage(...)` ni `_add_anotate_page(...)` antes de resolver.
- `webservice/WebService_itext.asmx.vb`, `service_anotate_pdf_image(...)`: el WebMethod retorna `IEnumerable(Of service_itex)`; éxito funcional exige `error_sistema = "YES"` y `url_salida`, mientras `error_log` puede representar una mutación confirmada con advertencia de auditoría.
- `itextshare/Class_ItexShare.vb`, `Class_itext_anotate`: el payload existente contiene documento, página, imagen, geometría, identificadores, rotación y transparencia; DOC-103 no cambia ese contrato.
- `tools/e2e/tests/workflow-pdf-viewer-streaming-policy.test.cjs`: DOC-102 ya protege el refresco de una sola página mediante transporte `stream`.
- `tools/e2e/AGENT-RUNBOOK.md`: toda E2E autenticada y mutadora requiere autorización vigente, PDF descartable, secretos efímeros y controles de verificación solo `SELECT`.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Modificar exclusivamente la ruta activa `pdf_viewer_anotate.js`; conservar `pdfannotate.js` inactivo y sin duplicar la corrección. | `index.html:183-184`; ambas implementaciones localizadas | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Modelar el guardado como una única promesa que abarca validación, carga de imagen, POST, normalización y refresco; retirar el overlay solo tras resultado autoritativo confirmado. | `event_drag_anotate`, `save_anotate_firma`, retorno anticipado `"YES"` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Mantener un registro single-flight por documento, página y overlay; bloquear controles de ese overlay mientras la operación está en curso y liberar siempre en `finally`. | atributos `parent_id`/`event_drag`; ausencia de registro vigente | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Envolver el `jqXHR` en una promesa cancelable con timeout explícito de 30 s; abortar solicitudes activas en `pagehide`/descarga del visor sin reintento automático. | `$.ajax` sin `timeout` ni retorno; no existe ciclo de cancelación del guardado | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Normalizar la colección ASMX en un resultado cerrado: `committed`, `committed_with_warning`, `rejected`, `invalid_response` o `uncertain`; HTTP 200 no implica éxito. | `IEnumerable(Of service_itex)` y campos `error_sistema`, `url_salida`, `error_log` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Traducir fallos a códigos y mensajes públicos mediante `PdfViwerAlert`; nunca mostrar `responseText`, payload, ruta, usuario o excepción. | ramas AJAX con `alert(...)` y `xception.responseText` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Hacer que `_updatePage(...)` espere descarga, render y actualización de caché de la página afectada; conservar página activa, zoom, scroll y transporte `stream`. | `_updatePage` actual y política DOC-102 | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Mantener sin cambios el WebMethod, DTO, algoritmo iText, temporales, firma, Wacom, descarga e impresión; una advertencia de log posterior no autoriza repetir una mutación ya confirmada. | frontera ASMX/iText y permisos actuales | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Validar con doble ASMX sin escritura y E2E real separada, autorizada y descartable; una operación debe producir una solicitud y un efecto verificable. | infraestructura `tools/e2e` y runbook | D-09 | RQ-09 | Origen: D-09, RQ-09 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Existe un único propietario productivo del guardado. | La página carga `pdf_viewer_anotate.js` y no ejecuta `pdfannotate.js`. | Evita corregir código muerto o activar dependencias retiradas. |
| RQ-02 | El overlay solo desaparece después de persistencia y refresco confirmados. | Error de validación, imagen, red, servidor o refresco conserva el overlay. | Evita pérdida visual y falso éxito. |
| RQ-03 | Doble clic/eventos concurrentes producen como máximo un POST por clave. | Mientras existe una promesa en curso, los controles quedan deshabilitados y llamadas repetidas reutilizan/rechazan sin nuevo POST. | No pretende resolver concurrencia servidor entre pestañas; esa deuda pertenece a DOC-104. |
| RQ-04 | Timeout y cierre terminan de forma controlada. | A los 30 s o en `pagehide`, se aborta el `jqXHR`, se libera el bloqueo y no hay reintento automático. | Un timeout después de enviar es resultado incierto; el mensaje no afirma rollback. |
| RQ-05 | Solo una respuesta ASMX completa confirma éxito. | Colección de un elemento, `error_sistema = YES` y `url_salida` válida permiten refrescar; cualquier otra forma se rechaza o clasifica incierta. | `error_log <> YES` significa commit con advertencia y nunca dispara repetición automática. |
| RQ-06 | Los mensajes son accionables y saneados. | 401/403, 404, 500, parser, timeout, aborto y respuesta inválida usan códigos públicos sin datos crudos. | Preserva privacidad y evita XSS mediante contenido servidor no confiable. |
| RQ-07 | Solo cambia la página afectada y el estado de navegación se conserva. | `_updatePage` resuelve después del render/caché; página activa, zoom y scroll no cambian. | No se reinicializa ni redescarga el PDF completo. |
| RQ-08 | Contratos fuera del cliente permanecen compatibles. | No hay diff en ASMX, DTO, iText, firma, Wacom, descarga o impresión salvo que una evidencia nueva obligue a reabrir el diseño. | MSBuild solo es obligatorio si finalmente cambia código compilado. |
| RQ-09 | La evidencia demuestra atomicidad cliente y efecto único. | Dobles cubren la matriz completa; E2E real requiere autorizaciones y PDF descartable, con controles antes/después solo `SELECT`. | Sin autorización se registra bloqueo, no se simula éxito. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` está desarrollada en `design.md`, reflejada en `spec.md` y vinculada a tareas mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea declara complejidad, área concreta y una verificación observable.
3. No se crean módulos, endpoints, DTO, framework de pruebas ni arnés E2E paralelos.
4. Código, pruebas, E2E autorizada, documentación y evidencia saneada forman una sola entrega.

## Resultado del refinamiento

- Estado: aprobado tras inspección de la ruta JavaScript activa, contrato ASMX, refresco DOC-102 e infraestructura E2E.
- Alcance bloqueado: transacción cliente; cualquier cambio iText/temporales se deriva a DOC-104.
- Comando de sincronización: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-103 --sync`.
