<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Context

`Handler_image_wf.ashx.vb` recibe `rut_image`, normaliza `|` a `/`, obtiene la extensión desde esa ruta y delega en `ClassRaEnvioCorrespondencia.ReadFile`. Ese método cambia atributos del archivo, reserva un `Byte()` del tamaño total y lo llena antes de que el handler use `BinaryWrite` para PDF. El handler implementa `IRequiresSessionState`, aunque no escribe sesión, por lo que solicitudes concurrentes de rangos quedarían serializadas.

El arranque de `pdfjs/pdf_anotate_view/script.js` reconstruye la URL del handler y ejecuta `fetch`, `blob`, `FileReader` y `Uint8Array` antes de llamar a `loadDocument`. `PDFjsViewer.loadDocument` ya pasa su argumento a `pdfjsLib.getDocument`, por lo que admite una URL sin cambiar la API interna. Los constructores enriquecidos actuales se encuentran en `Docuarchi/ClassVisualisaDocumento.vb`, `workflow/ClassWorflowVisor.vb` y `Gestion/ClassGaProducionDocumental.vb`; además existen consumidores simples de `rut_image` para descargas, formatos no PDF, versiones, migración y visores externos.

## Goals / Non-Goals

**Goals**

- Reducir tiempo y bytes hasta el primer canvas permitiendo que PDF.js negocie streaming y rangos.
- Evitar materializar el documento completo en memoria del servidor o del navegador para abrir el visor enriquecido.
- Hacer que la autoridad del flujo nuevo sea una referencia emitida por servidor, no una ruta libre.
- Cumplir semántica HTTP observable para `GET`, `HEAD`, Range y revalidación privada.
- Mantener las acciones de anotación, firma, impresión y descarga, y aislar el adaptador legacy.

**Non-Goals**

- Agregar persistencia, tablas, almacenamiento externo, CDN o un framework nuevo.
- Cambiar el guardado de grafos o los permisos funcionales del visor.
- Migrar en esta entrega cada consumidor simple de `rut_image`; su deuda queda delimitada y comprobada por regresión.
- Habilitar caché pública o acceso sin la sesión/autorización que originó la referencia.

## Decisions

### D-01 — Referencia opaca emitida por servidor

Se añadirá un componente VB.NET bajo `Infrastructure/Documentos/Streaming/` que cree y resuelva una referencia protegida con `MachineKey`. El payload versionado incluirá ruta canónica, expiración UTC, identidad de sesión y metadatos mínimos de presentación. La referencia se codificará para URL y solo podrá resolverse en la misma sesión antes de expirar. La protección autenticada evita modificar ruta, identidad o vencimiento; una referencia inválida, ajena o expirada producirá un error genérico sin revelar la causa ni la ruta. <!-- D-01; RQ-01 -->

Los tres constructores enriquecidos emitirán `doc_ref` después de haber resuelto el documento mediante sus reglas actuales. El navegador nunca construirá ni enviará una ruta física nueva para activar streaming. No se inventan tablas ni una segunda fuente de autorización.

### D-02 — Semántica determinista de método y rango

El handler aceptará `GET` y `HEAD`. Para una referencia autorizada sin `Range` responderá `200`; para un único rango válido `bytes=start-end`, `bytes=start-` o `bytes=-suffix` responderá `206`. Rechazará rangos múltiples, sintaxis inválida, sufijo cero, inicio fuera del archivo o fin anterior al inicio con `416` y `Content-Range: bytes */<length>`. El fin superior se limitará al último byte. `HEAD` devolverá los encabezados de la representación completa y ningún cuerpo. <!-- D-02; RQ-02 -->

### D-03 — Caché privada y validadores ligados a la referencia

Las respuestas autorizadas usarán `Cache-Control: private, no-cache, must-revalidate`, `Vary: Cookie`, `ETag` fuerte opaco y `Last-Modified` normalizado a segundos. El ETag se derivará de la referencia autorizada y metadatos del archivo, sin exponer la ruta. `If-None-Match` tendrá precedencia sobre `If-Modified-Since` para devolver `304` en solicitudes completas. `If-Range` solo conservará el rango cuando coincida el ETag fuerte o un HTTP-date válido que no sea anterior a la modificación; de otro modo se enviará `200` completo. <!-- D-03; RQ-03 -->

La URL opaca, el enlace a sesión y la caché privada impiden que una respuesta validada para un usuario se reutilice como autorización para otro.

### D-04 — Streaming acotado y cierre seguro

El handler abrirá `FileStream` con lectura y compartición compatibles y copiará únicamente el intervalo seleccionado mediante un buffer fijo. No llamará a `ReadFile`, no cambiará atributos, no creará un `Byte()` del tamaño del documento y no usará `BinaryWrite`. Establecerá `Content-Length`, `Content-Range` cuando aplique, `Accept-Ranges: bytes`, MIME explícito y `Content-Disposition` con nombre saneado. Cerrará stream y buffer mediante `Using`, comprobará desconexión y finalizará con `CompleteRequest`, sin `Response.End` ni mensajes de excepción públicos. <!-- D-04; RQ-04 -->

### D-05 — Sesión de solo lectura para concurrencia

`Handler_image_wf` cambiará de `IRequiresSessionState` a `IReadOnlySessionState`. La emisión de la referencia ocurre en la página que ya tiene autorización y puede escribir su propia respuesta; el handler solo compara la identidad de sesión incluida y no muta sesión, tarea, auditoría o documento. Esto permite que PDF.js emita rangos concurrentes sin el bloqueo exclusivo del handler actual. <!-- D-05; RQ-05 -->

### D-06 — PDF.js consume URL con estrategia adaptativa

`script.js` construirá una única URL autorizada a partir de `ash` y `doc_ref`, la pasará a `load_pdf_viwer_anotation` y eliminará del arranque `fetch`, `Blob`, `FileReader` y la copia a `Uint8Array`. El servidor inspeccionará como máximo 4 KiB del encabezado, sin modificar el archivo: un PDF de al menos 1 MiB con diccionario `/Linearized` usará rangos de 256 KiB con `disableStream` y `disableAutoFetch`; un PDF pequeño, no linearizado o indeterminado usará una sola respuesta `200` con `disableRange`, conservando streaming progresivo. La selección viajará como una sugerencia de rendimiento sin autoridad documental; `doc_ref` seguirá siendo la única autoridad del flujo nuevo. <!-- D-06; RQ-06 -->

El pegado de grafo conservará `url_document`/`urimage_format`, el web service iText, su payload y las coordenadas actuales. `_updatePage` abrirá la URL de salida mediante streaming completo para refrescar únicamente la página afectada, evitando exigir Range a una URL devuelta por el contrato legacy. Las entradas binarias existentes conservarán su contrato.

### D-07 — Adaptador legacy delimitado

`rut_image` permanecerá temporalmente para consumidores simples inventariados, incluidos visores de descarga, formatos no PDF, versiones/migración y ramas `LIBRE_PDF`. Ese camino también se transmitirá por bloques para retirar `ReadFile/BinaryWrite`, pero no anunciará Range ni validadores nuevos y no podrá activar el contrato `doc_ref`. Los constructores enriquecidos migrados no volverán a emitir `rut_image` como autoridad de apertura. La deuda de autorización de consumidores simples queda explícita y no se amplía. <!-- D-07; RQ-07 -->

### D-08 — Validación como unidad de entrega

Se añadirán pruebas focales del parser/rendimiento HTTP y políticas estáticas de integración; se ejecutarán MSBuild y OpenSpec estricto. La E2E real reutilizará exclusivamente `tools/e2e`, después de leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`, y requerirá autorización expresa de ambiente, cuenta y documento. La evidencia saneada registrará estados, encabezados, bytes transferidos y tiempo a primer canvas sin secretos ni rutas. Código, runner y evidencia forman una sola entrega. <!-- D-08; RQ-08 -->

### D-09 — Linearización evaluada solo como derivado temporal

La viabilidad se medirá con Ghostscript `pdfwrite -dFastWebView=true`, disponible en la estación de prueba, sobre una copia sintética construida desde un fixture público del repositorio. Un servidor HTTP local con soporte de rango entregará la línea base y la variante al mismo `pdfjs/build/pdf.js` usado por la aplicación. El runner medirá bytes, solicitudes y tiempo hasta renderizar la primera página, comparará cantidad de páginas y huella del canvas, y eliminará todos los temporales en `finally`. No se autenticará, no se abrirán documentos del sistema, no se sustituirá el original y Ghostscript no se agregará al runtime ni al despliegue de DOC-102. <!-- D-09; RQ-09 -->

El resultado es evidencia de factibilidad, no autorización para reescribir PDFs productivos. Una adopción posterior deberá validar licenciamiento, documentos cifrados, firmas digitales, formularios, anotaciones y el efecto de cada reescritura iText sobre la linearización.

## HTTP Flow

1. Una clase de visor resuelve el documento con el flujo legacy ya autorizado.
2. El servidor emite una referencia protegida, expirable y ligada a la sesión, y la incluye como `doc_ref` en la URL del visor.
3. El emisor inspecciona el encabezado y añade `pdf_load_mode=range|stream`; `script.js` entrega `Handler_image_wf.ashx?doc_ref=...` directamente a PDF.js.
4. El handler valida método, sesión, referencia, expiración y existencia antes de abrir el archivo.
5. El handler evalúa condiciones de caché y luego `Range`/`If-Range` con reglas deterministas.
6. El cuerpo completo o parcial se copia desde `FileStream` con memoria acotada; `HEAD`, `304` y `416` no envían cuerpo.
7. PDF.js solicita rangos para PDF linearizados o consume una respuesta secuencial para los demás; ambos caminos usan streaming acotado del servidor.
8. Acciones opcionales continúan usando sus permisos y contratos vigentes; ninguna respuesta del streamer modifica estado.
9. El experimento aislado genera y sirve copias temporales fuera del flujo anterior; al terminar elimina las copias y no cambia la referencia documental ni el almacenamiento.

## Risks / Trade-offs

- `MachineKey` debe ser coherente entre nodos del mismo despliegue; una configuración distinta invalidará referencias al cambiar de nodo. La validación de despliegue deberá comprobarlo sin registrar claves.
- Mantener `rut_image` evita romper consumidores simples, pero conserva una deuda de autorización preexistente. Se reduce su superficie al impedir que reciba las capacidades nuevas y se documenta para una migración posterior.
- `private, no-cache` permite almacenamiento privado con revalidación; aumenta solicitudes respecto de una caché `max-age`, pero evita servir documentos privados sin revalidar.
- Un rango muy pequeño genera más solicitudes. PDF.js controlará su tamaño estándar; el servidor mantendrá buffer fijo y validación de límites.
- El cliente puede desconectarse durante la copia. El cierre determinista evita retener handles y el error no se expondrá como fallo de negocio.
- `disableAutoFetch` evita la precarga especulativa, pero no puede impedir que el parser solicite todos los bytes necesarios para resolver una página. La E2E final del documento aprobado confirmó el contrato Range y aun así consumió sus 33.589.755 bytes codificados antes del primer canvas; garantizar carga parcial exige optimización o linearización documental fuera de este alcance.
- La detección de `/Linearized` es deliberadamente conservadora. Un falso negativo usa streaming completo compatible; un parámetro `pdf_load_mode` alterado solo afecta rendimiento y nunca concede acceso porque el handler valida `doc_ref`.
- La E2E adaptativa del documento aprobado eligió streaming secuencial, redujo la mediana de 1.123 ms a 624 ms y sustituyó 61 rangos por una respuesta `200` por iteración. El beneficio observado es latencia/sobrecarga HTTP; el volumen total permanece igual porque el PDF no está linearizado.
- Ghostscript puede reescribir fuentes, formularios, anotaciones, metadatos o estructuras avanzadas; Fast Web View es incompatible con PDF cifrado y no preserva firmas criptográficas después de una reescritura. Por eso la prueba no altera el archivo fuente y no constituye todavía una solución productiva.

## Migration Plan

1. Incorporar y probar referencia, parser, validadores y copia acotada sin conectar el visor.
2. Migrar los tres constructores enriquecidos para emitir `doc_ref` y registrar el archivo nuevo en el proyecto.
3. Cambiar `script.js` a URL directa y selección adaptativa manteniendo `urimage_format` para acciones legacy y streaming completo para el refresco posterior al grafo.
4. Ejecutar pruebas focales, MSBuild y regresión de consumidores simples.
5. Con autorización explícita, ejecutar E2E y comparar bytes/primer canvas con la evidencia DOC-101 vigente.
6. Rollback: restaurar en los tres constructores el parámetro `rut_image` y la precarga del cliente; el adaptador legacy permanece disponible durante esta entrega.

## Open Questions

- La validación E2E deberá confirmar si el balanceador del ambiente mantiene `MachineKey` compartida y afinidad de sesión; si no, la referencia fallará cerrada y se documentará como bloqueo de despliegue.
- La migración definitiva de consumidores simples y la eliminación de `rut_image` requieren un ticket posterior porque abarcan flujos públicos, externos y de formatos no PDF con autoridades distintas.
