<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->

## Context

La ruta productiva del visor carga `pdf_viewer_anotate.js`; `pdfannotate.js` está comentado. Al guardar, `event_drag_anotate(...)` llama `save_anotate_firma(...)`, que registra `Image.onload` pero retorna `"YES"` inmediatamente. El llamador elimina entonces el overlay antes de iniciar o confirmar `service_anotate_pdf_image(...)`. El transporte AJAX no retorna una promesa, no tiene timeout y filtra respuestas crudas. El WebMethod devuelve una colección `service_itex`; DOC-103 conserva esa frontera y corrige únicamente la transacción cliente.

## Goals / Non-Goals

**Goals**

- Hacer esperable el recorrido validación → imagen → POST → respuesta → refresco.
- Garantizar single-flight por documento, página y overlay.
- Conservar el overlay y permitir recuperación explícita ante fallo o resultado incierto.
- Refrescar solo la página persistida sin perder página activa, zoom o scroll.
- Entregar mensajes públicos saneados y evidencia reproducible.

**Non-Goals**

- Cambiar `WebService_itext.asmx.vb`, `Class_itext_anotate`, iTextSharp, temporales, auditoría o almacenamiento.
- Resolver colisiones servidor entre pestañas; corresponde a DOC-104.
- Reactivar o mantener en paralelo `pdfannotate.js`.
- Cambiar firma, Wacom, impresión, descarga o permisos del visor.

## Flujo técnico aprobado

```text
Usuario pulsa guardar
  -> event_drag_anotate valida overlay, canvas, página e imagen
  -> calcula clave documento:página:overlay
  -> ¿existe operación pendiente?
       sí -> no crea otro POST; conserva la promesa vigente
       no -> deshabilita guardar/cerrar y registra la operación
  -> save_anotate_firma espera Image.onload/Image.onerror
  -> construye el payload legacy sin alterar campos
  -> service_anotate_pdf_image inicia un único POST con timeout de 30 s
  -> normaliza data.d
       rejected/invalid_response -> aviso público; conserva overlay
       uncertain/timeout/abort -> aviso de resultado incierto; conserva overlay; sin retry
       committed -> espera _updatePage(página, url_salida)
       committed_with_warning -> misma actualización + advertencia pública de auditoría
  -> ¿refresco completo?
       sí -> retira únicamente el overlay guardado
       no -> conserva overlay y reporta fallo de refresco
  -> finally elimina registro single-flight y rehabilita controles si aún existen
```

## Decisions

### D-01 — Un solo propietario activo

La implementación se limita a `pdf_viewer_anotate.js`. Una prueba de política comprobará que `index.html` lo carga y mantiene comentado `pdfannotate.js`. No se copia la solución a código muerto.

### D-02 — Promesa autoritativa de extremo a extremo

`event_drag_anotate`, `save_anotate_firma` y `service_anotate_pdf_image` retornarán/esperarán una promesa real. El resultado de la transacción solo será exitoso después de respuesta funcional válida y refresco esperado. Los errores síncronos y `Image.onerror` rechazan la misma cadena.

### D-03 — Single-flight localizado

Un `Map` de operaciones pendientes usará una clave estable compuesta por documento, página y `parent_id`. La primera invocación posee el POST; invocaciones repetidas no crean transporte adicional. Guardar y cerrar del overlay se deshabilitan durante el vuelo y se restauran en `finally`. Otros overlays y navegación continúan operativos.

### D-04 — Timeout y cancelación sin reintento

El transporte jQuery conservará su `jqXHR`, configurará 30 000 ms y expondrá cancelación. `pagehide` abortará solicitudes pendientes. Timeout, status 0, aborto posterior al envío o 5xx se tratan conservadoramente como resultado incierto: no se reintenta de forma automática ni se afirma que el servidor no escribió.

### D-05 — Normalización cerrada del ASMX

Una función pura validará `data.d` como colección no vacía y el primer objeto como contrato. `error_sistema === "YES"` más `url_salida` no vacía produce `committed`; `error_log !== "YES"` produce `committed_with_warning`. Error funcional, forma inválida y fallos de transporte generan categorías distintas. Nunca se considera suficiente HTTP 200.

### D-06 — Mensajes públicos

Las categorías se traducen a códigos internos saneados y textos fijos mediante `PdfViwerAlert`. No se interpola `error_sistema`, `error_log`, `responseText`, `errorThrown` ni `ex.message` en DOM o consola. Los detalles técnicos solo se validan en dobles locales sin datos reales.

### D-07 — Refresco parcial esperado

`_updatePage` mantendrá `_createLoadingSource(document, "stream")` y esperará `loadingTask.promise`, `getPage(1)`, `_renderPage(...)` y `_add_anotate_page(...)`. No invocará carga completa. El overlay se retira después de esa resolución, sin reasignar página activa, zoom o scroll.

### D-08 — Frontera servidor intacta

El payload, URL ASMX y DTO permanecen iguales. El estado `committed_with_warning` reconoce que el PDF ya fue mutado aunque la auditoría posterior falle; se muestra advertencia y se evita duplicar el grafo. Cualquier cambio de iText, temporales o semántica del WebMethod se documenta como dependencia DOC-104.

### D-09 — Estrategia de prueba y autorización

Las pruebas Node existentes cargarán el archivo activo con DOM/jQuery controlados y un doble ASMX sin escritura. Cubrirán matriz de transporte, single-flight, lifecycle y refresco. La E2E real tendrá runner dentro de `tools/e2e`, leerá el runbook y se bloqueará antes de autenticar si faltan autorizaciones de ambiente, cuenta y PDF descartable; controles antes/después serán exclusivamente `SELECT`.

## Riesgos y mitigaciones

- **Resultado incierto:** conservar overlay y no reintentar automáticamente; informar que debe verificarse el documento.
- **Commit con fallo de log:** retirar overlay tras refresco porque repetir duplicaría el grafo; mostrar advertencia genérica.
- **Elemento eliminado durante el vuelo:** `finally` verifica existencia antes de rehabilitar controles.
- **Refresco fallido después del commit:** conservar overlay puede mostrar una copia visual superpuesta; el mensaje obliga a recargar/verificar antes de reintentar.
- **Dependencia de jQuery:** se reutiliza la instalada; no se agrega `fetch`, Axios ni otra biblioteca.

## Migration Plan

1. Introducir normalizador, códigos y registro single-flight en el archivo activo.
2. Convertir transporte, carga de imagen y evento a promesas esperables.
3. Hacer esperable `_updatePage` conservando el contrato DOC-102.
4. Ejecutar matriz Node y E2E con doble sin escritura.
5. Ejecutar E2E mutadora solo tras autorización específica y PDF descartable.
6. Rollback: revertir únicamente cambios DOC-103 en `pdf_viewer_anotate.js` y su infraestructura de prueba; no existe migración de datos ni cambio servidor.

## Open Questions

No quedan decisiones bloqueantes para implementar. La optimización y atomicidad física del servidor permanecen explícitamente en DOC-104.
