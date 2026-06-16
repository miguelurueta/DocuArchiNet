## Why

REEMPLAZA-PAGINA-APPVISOREMBEDPDF. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-238.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # SCRUM-249 Integracion Frontend Reemplazo Paginas PDF Anotadas
> 
> ## 1. Objetivo Frontend
> 
> Permitir que el frontend detecte paginas anotadas en `AppVisorEmbedPdf`, genere uno o varios PDFs anotados de una sola pagina, los suba por chunks y luego ejecute el reemplazo fisico de paginas especificas del PDF almacenado en gabinete.
> 
> Este flujo no reemplaza el documento completo enviado por el frontend. El backend abre el PDF original, reemplaza solo las paginas indicadas y genera un PDF final usando iText/iText7.
> 
> Repositorio objetivo: `DocuArchiCore.react`
> 
> Scope: Frontend React + TypeScript.
> 
> Componentes frontera:
> 
> - Visor PDF: `src/app/Components/UI/AppVisorEmbedPdf`
> - Consumidor principal: `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
> - Toolbar: `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`
> 
> Cliente HTTP obligatorio:
> 
> - Usar `clienteApi` y los patrones existentes del proyecto.
> - No usar `fetch` ni `axios` directo en codigo productivo nuevo salvo que una restriccion tecnica documentada del upload binario lo exija.
> - Propagar `AbortSignal`.
> - Desempaquetar `AppResponses<T>`.
> - Normalizar errores de dominio con `Field`, `Message`, `RequestId` cuando aplique.
> 
> Responsabilidades:
> 
> - `AppVisorEmbedPdf`: unica frontera con EmbedPDF. Detecta paginas anotadas, ejecuta `commit()`, exporta copia anotada y produce blobs PDF de una sola pagina. No llama APIs.
> - `DocumentosWorkbench`: orquesta negocio documental. Valida documento activo, PDF, permisos, firma electronica, contexto workflow, progreso, cancelacion y refresco.
> - Servicio HTTP dedicado: inicializa upload temporal, sube chunks, completa, cancela y llama `paginas-anotadas`.
> - `AppPdfToolbar`: presentacional. Recibe callbacks, flags y progreso; no conoce HTTP, workflow ni EmbedPDF.
> 
> Estado actual que debe preservarse:
> 
> - El visor ya usa `AnnotationLayer`, `useAnnotation(documentId)`, `useAnnotationCapability()`, `useExport(documentId)`, `annotationCap.provides.commit()`, `exportApi.provides.saveAsCopy()`, `annotation.state.pages`, API imperativa `load/reset/cancelCurrentLoad`, permisos efectivos y debug con `window.__DV_DEBUG__`.
> - `DocumentosWorkbench` ya usa carga gestionada con `visorRef.current?.load(...)` para PDFs. No volver a una integracion basada solo en `fileUrl`.
> - No romper exportacion, impresion, firma, visualizacion de imagenes ni carga gestionada actual.
> 
> Modo de implementacion obligatorio:
> 
> 1. Implementar primero contrato HTTP y tests del servicio.
> 2. Implementar despues exportacion de PDFs anotados de una sola pagina.
> 3. Integrar luego `DocumentosWorkbench` y toolbar.
> 4. Cerrar con hardening, QA y documentacion.
> 
> Si una fase depende de una decision tecnica no resuelta, bloquear esa fase y documentar la razon. No sustituir con imagenes, canvas, Base64, rasterizacion ni PDF completo.
> 
> ## 2. Seguridad
> 
> - Header obligatorio en todas las APIs: `Authorization: Bearer {jwt}`.
> - Claims minimos en JWT:
>   - `defaulalias`
>   - `usuarioid`
> - Si el PDF original requiere contrasena, el frontend puede enviar `OriginalPdfPassword` solo en el request final de reemplazo.
> - No guardar `OriginalPdfPassword` en `localStorage`, `sessionStorage`, IndexedDB, logs del navegador, telemetria ni estado global persistente.
> - Mantener `OriginalPdfPassword` solo en memoria volatil mientras el documento este abierto.
> - Limpiar `OriginalPdfPassword` en `reset`, cambio de documento, cancelacion, cierre de visor o desmontaje.
> - No mostrar rutas fisicas de gabinete a usuarios finales si la politica del producto no lo permite.
> - No loguear blobs completos, passwords, JWT ni respuestas con datos sensibles.
> 
> ## 3. APIs del modulo Reemplazo Paginas PDF Anotadas
> 
> - `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`
> - `PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
> - `GET /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
> - `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
> - `DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`
> - `POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`
> 
> Todos los responses usan envelope:
> 
> ```ts
> type AppResponses<T> = {
>   success: boolean;
>   message: string;
>   data: T | null;
>   meta?: { Status?: string };
>   errors?: Array<{
>     Type?: string;
>     Field?: string;
>     Message?: string;
>   }>;
> };
> ```
> 
> Reglas de envelope:
> 
> - Si HTTP no es 2xx, lanzar error de dominio.
> - Si `success !== true`, lanzar error de dominio.
> - Si `data` viene `null` en endpoint que requiere datos, tratarlo como error contractual.
> - Preservar `Field` para casos como `originalPdfPassword`.
> 
> ## 4. Contratos por API
> 
> ### 4.1 Init upload temporal
> 
> `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`
> 
> Request por cada PDF anotado de una sola pagina:
> 
> ```json
> {
>   "NombreOriginal": "DIG00015416-PAGINA-2-ANOTADA.PDF",
>   "TamanoBytes": 251004,
>   "Extension": ".PDF",
>   "HashSha256Esperado": "opcionalesha256",
>   "NumeroChunks": 1
> }
> ```
> 
> Response OK:
> 
> ```json
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "RutaTemporalId": "usr_141_18be1f9f81524358bdf6a78e7f25f2dc",
>     "ArchivoTemporalId": "af_0d22cb08fb6b4f16b3916f6759089f49.pdf",
>     "ChunkSizeBytes": 1048576,
>     "Estado": "IN_PROGRESS"
>   },
>   "errors": []
> }
> ```
> 
> Reglas frontend:
> 
> - Llamar `init` una vez por cada PDF anotado de una sola pagina.
> - Normalizar extension a `.PDF`.
> - Guardar `RutaTemporalId` + `ArchivoTemporalId` por pagina.
> - Usar `DELETE` solo cuando el usuario abandona el flujo antes del reemplazo final.
> - Si `paginas-anotadas` responde exitosamente, no llamar `DELETE`: el backend ya consumio y elimino los temporales usados.
> - El upload temporal genera un `RutaTemporalId` nuevo por cada `init`. Por eso, para reemplazar varias paginas en una sola llamada, cada item de `Paginas` debe enviar el `RutaTemporalId` que recibio al subir su PDF anotado.
> 
> ### 4.2 Upload chunk
> 
> `PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
> 
> Headers de contrato:
> 
> - `Content-Type: application/octet-stream`
> - `Content-Length: {bytesChunk}`
> - `X-Total-Chunks: {totalChunks}`
> 
> Body:
> 
> - Binario puro del chunk del PDF anotado.
> - No JSON.
> - No Base64.
> - No `FormData`, salvo cambio explicito del contrato backend.
> 
> Response OK:
> 
> ```json
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "chunkIndex": 0
>   },
>   "errors": []
> }
> ```
> 
> Nota frontend obligatoria:
> 
> - `Content-Length` es un header restringido en browsers. React, fetch y Axios no deben setearlo manualmente.
> - En browser, enviar `Blob`, `File` o `ArrayBuffer` como body crudo para que el runtime calcule `Content-Length`.
> - En tests unitarios no exigir que el codigo setee manualmente `Content-Length`; validar `Content-Type`, `X-Total-Chunks`, URL, `chunkIndex` y body binario.
> - Si QA real demuestra que backend no recibe `Content-Length` desde navegador, escalar contrato backend/frontend. No resolver con hacks desde React.
> 
> ### 4.3 Status upload temporal
> 
> `GET /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
> 
> Response OK:
> 
> ```json
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "Estado": "IN_PROGRESS",
>     "ChunksRecibidos": 1,
>     "ChunksPendientes": 0,
>     "TamanoRecibidoBytes": 251004
>   },
>   "errors": []
> }
> ```
> 
> ### 4.4 Complete upload temporal
> 
> `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
> 
> Response OK:
> 
> ```json
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "Estado": "COMPLETED"
>   },
>   "errors": []
> }
> ```
> 
> Antes de llamar `paginas-anotadas`, cada temporal debe estar `COMPLETED`.
> 
> ### 4.5 Cancel upload temporal
> 
> `DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`
> 
> Response OK:
> 
> ```json
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "Estado": "CANCELLED"
>   },
>   "errors": []
> }
> ```
> 
> La cancelacion debe ser best-effort. Si un temporal ya fue consumido o no existe, no bloquear la limpieza local del flujo.
> 
> ### 4.6 Reemplazo final de paginas PDF anotadas
> 
> `POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`
> 
> Request:
> 
> ```json
> {
>   "NombreGabinete": "contabil",
>   "IdDocumento": 15416,
>   "RutaTemporalId": "usr_141_18be1f9f81524358bdf6a78e7f25f2dc",
>   "OriginalPdfPassword": "solo-si-el-pdf-original-esta-protegido",
>   "Paginas": [
>     {
>       "PageNumber": 2,
>       "RutaTemporalId": "usr_141_page2",
>       "ArchivoTemporalId": "af_0d22cb08fb6b4f16b3916f6759089f49.pdf",
>       "ContentType": "application/pdf",
>       "HashSha256Esperado": "opcionalesha256"
>     },
>     {
>       "PageNumber": 5,
>       "RutaTemporalId": "usr_141_page5",
>       "ArchivoTemporalId": "af_91bbcb08fb6b4f16b3916f6759089a1.pdf",
>       "ContentType": "application/pdf",
>       "HashSha256Esperado": "opcionalesha256"
>     }
>   ],
>   "Motivo": "Actualizacion de grafo PDF en paginas anotadas",
>   "DescOp": "AGREGA GRAFO PDF",
>   "ModuloRegistro": "DOCUARCHI",
>   "Radicado": "2600466700019",
>   "IdTareaWorkflow": 12873,
>   "IdRutaWorkflow": 45,
>   "TipologiaDocumental": "FACTURA"
> }
> ```
> 
> `OriginalPdfPassword` es opcional. Debe enviarse unicamente cuando el usuario ya ingreso la contrasena para visualizar/anotar un PDF original protegido. El backend la usa para abrir el PDF original con iText durante esta operacion y no la persiste.
> 
> Si se envia `OriginalPdfPassword` y es valida, el PDF final queda nuevamente protegido con esa clave. El frontend no debe asumir que se conservan permisos internos exactos del cifrado original; la garantia funcional es que el documento final requiere password.
> 
> El PDF anotado de una pagina puede tener un tamano u orientacion diferente al original. La API conserva el tamano/orientacion de la pagina original y ajusta el contenido anotado dentro de esa caja, por lo que el frontend no necesita calcular el `page size` final.
> 
> `RutaTemporalId` raiz existe solo como fallback compatible. En clientes nuevos y reemplazo multipagina, enviar siempre `RutaTemporalId` dentro de cada item de `Paginas`.
> 
> Response OK:
> 
> ```json
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "IdDocumento": 15416,
>     "NombreGabinete": "contabil",
>     "PaginasReemplazadas": [2, 5],
>     "RutaArchivoFinal": "D:/imagenes/discos/CONTABIL7/00093/DIG00015416.PDF",
>     "RutaRespaldo": "D:/temp/storage-temp/replacement-versions/contabil/15416/20260603101530123/f5db29fb18b94b27878db78f6743aa52/DIG00015416.PDF",
>     "TamanoAnteriorBytes": 248112,
>     "TamanoNuevoBytes": 251004,
>     "HashAnteriorSha256": "...",
>     "HashNuevoSha256": "...",
>     "RequestId": "f5db29fb18b94b27878db78f6743aa52"
>   },
>   "meta": {
>     "Status": "success"
>   },
>   "errors": []
> }
> ```
> 
> ### 4.7 Validacion anti-desfase
> 
> Existe riesgo de desfase si el usuario anota una pagina renderizada por el frontend y, antes del reemplazo final, el PDF fisico del backend cambia.
> 
> El endpoint acepta metadata opcional de la version renderizada:
> 
> - `SourceDocumentHashSha256`
> - `SourceDocumentVersion`
> - `SourcePageWidth`
> - `SourcePageHeight`
> - `SourcePageRotation`
> - `SourcePageFingerprintSha256`
> 
> Reglas frontend:
> 
> - Enviar metadata anti-desfase solo si existe fuente real y confiable.
> - No inventar hash, version, geometria ni fingerprint.
> - Si el backend o flujo de descarga no exponen esta metadata, no enviar esos campos y documentar pendiente.
> - Si backend rechaza por desfase, pedir al usuario recargar el documento y repetir la anotacion sobre la version actual.
> - No enviar capturas o imagenes base para comparacion pixel a pixel como validacion principal.
> 
> Ejemplo de item de pagina con metadata anti-desfase:
> 
> ```json
> {
>   "PageNumber": 2,
>   "RutaTemporalId": "usr_141_page2",
>   "ArchivoTemporalId": "af_0d22cb08fb6b4f16b3916f6759089f49.pdf",
>   "ContentType": "application/pdf",
>   "HashSha256Esperado": "hash-del-temporal-anotado",
>   "SourcePageWidth": 612,
>   "SourcePageHeight": 792,
>   "SourcePageRotation": 0,
>   "SourcePageFingerprintSha256": "opcional-huella-de-la-pagina-original-renderizada"
> }
> ```
> 
> ## 5. Paso a paso de consumo frontend
> 
> 1. Identificar documento de gabinete, contexto workflow y paginas anotadas que se van a reemplazar.
> 2. Bloquear si no hay documento activo, si no es PDF, si faltan permisos, si el documento esta firmado electronicamente o si no hay paginas anotadas.
> 3. Conservar en memoria hash/version/metadata de pagina solo cuando exista fuente confiable para validacion anti-desfase.
> 4. Ejecutar `annotationCap.provides.commit()` para materializar anotaciones.
> 5. Exportar PDF anotado completo con `exportApi.provides.saveAsCopy()`.
> 6. Extraer un PDF anotado independiente de una sola pagina por cada pagina modificada.
> 7. Verificar que cada PDF generado tiene `type = "application/pdf"`, `size > 0` y exactamente una pagina.
> 8. Para cada PDF anotado, calcular `TamanoBytes`, `NumeroChunks` y opcionalmente `HashSha256Esperado`.
> 9. Para cada PDF anotado, llamar `init` y guardar `RutaTemporalId` + `ArchivoTemporalId`.
> 10. Dividir cada PDF en chunks con `blob.slice(start, end)` usando `ChunkSizeBytes` devuelto por backend.
> 11. Enviar cada chunk con `PUT .../chunk/{chunkIndex}`, body binario puro y `X-Total-Chunks`.
> 12. Al terminar chunks de cada archivo, llamar `complete`.
> 13. Validar que cada temporal quede en `COMPLETED`.
> 14. Si el visor requirio contrasena para abrir el PDF original, mantenerla solo en memoria y enviarla como `OriginalPdfPassword` en el request final.
> 15. Llamar `POST /reemplazopdf/paginas-anotadas` enviando la lista `Paginas`, cada una con su propio `RutaTemporalId` y metadata anti-desfase cuando aplique.
> 16. Mostrar al usuario un resultado funcional: paginas reemplazadas y `RequestId`.
> 17. Refrescar el documento visible con el patron existente de `DocumentosWorkbench`.
> 18. Si el usuario cancela antes del reemplazo final, invocar `DELETE` para cada temporal creado.
> 19. Si el reemplazo final fue exitoso, no invocar `DELETE`.
> 
> Reglas de concurrencia:
> 
> - Usar `AbortController` por operacion.
> - Aplicar patron latest-wins: incrementar secuencia antes de iniciar y verificarla despues de cada `await` critico.
> - Si cambia el documento activo durante la operacion, abortar y limpiar temporales creados best-effort.
> - Si falla upload o reemplazo, conservar el documento visible actual y mostrar error funcional.
> 
> Extraccion PDF:
> 
> - No usar `pdfjs-dist` para rasterizar.
> - No usar canvas como contrato.
> - Si se requiere `pdf-lib`, documentar licencia, peso, impacto bundle y preferir import dinamico.
> - Si no existe mecanismo real para extraer PDFs de una pagina sin rasterizacion, bloquear implementacion y escalar decision tecnica.
> 
> ## 6. Campos de auditoria `logdocuarchi`
> 
> Campos recomendados cuando aplica flujo/radicacion:
> 
> - `DescOp`
> - `ModuloRegistro`
> - `Radicado`
> - `IdTareaWorkflow`
> - `IdRutaWorkflow`
> - `TipologiaDocumental`
> - `Motivo`
> 
> Defaults backend si no se envian:
> 
> - `DescOp`: `AGREGA GRAFO PDF`
> - `ModuloRegistro`: `DOCUARCHI`
> - `IdTareaWorkflow`: `0`
> - `IdRutaWorkflow`: `0`
> - `TipologiaDocumental`: se toma de `TIPODOCUMENTO` del gabinete cuando aplica
> 
> El backend registra ademas en `logdocuarchi.CAMPOS`:
> 
> - paginas reemplazadas
> - ids temporales
> - ruta preparada
> - ruta de respaldo
> - hashes
> - tamanos
> - modo `REEMPLAZO_PAGINAS_PDF_ANOTADAS`
> - total de paginas del PDF original
> - `RequestId`
> - `passwordOriginalSuministrado`: booleano. Nunca se registra el valor de `OriginalPdfPassword`.
> - `pdfFinalReencriptado`: booleano. Nunca se registra el valor de `OriginalPdfPassword`.
> 
> ## 7. Errores esperados
> 
> - `400 Validation`: falta `defaulalias`, usuario invalido, request incompleto, paginas duplicadas, pagina menor o igual a cero.
> - `400 Validation`: archivo temporal no existe, estado no `COMPLETED`, extension no PDF, hash SHA-256 no coincide.
> - `400 Validation`: PDF temporal no contiene exactamente una pagina.
> - `400 Validation`: pagina fuera del rango del PDF original.
> - `400 Validation`: documento de gabinete no existe o esta firmado.
> - `400 Validation`: PDF original protegido sin `OriginalPdfPassword` o con contrasena invalida. El error llega en `Field = originalPdfPassword`.
> - `400 Validation`: anti-desfase cuando el hash o metadata de pagina enviada por frontend no coincide con el PDF fisico actual.
> - `500 Error`: fallo no controlado en preparacion PDF, reemplazo fisico o auditoria.
> 
> Manejo frontend:
> 
> - Mostrar mensajes funcionales, no trazas tecnicas.
> - Preservar `Field` para enfocar accion correctiva.
> - Guardar `RequestId` cuando venga en respuesta de exito o error.
> - No ocultar el documento visible ante fallo.
> - Ante `originalPdfPassword`, pedir contrasena de nuevo o abortar sin persistirla.
> - Ante anti-desfase, pedir recargar documento antes de reintentar.
> 
> ## 8. Checklist de depuracion frontend
> 
> - JWT vigente con claims `defaulalias` y `usuarioid`.
> - `clienteApi` agrega `Authorization` correctamente.
> - Cada `ArchivoTemporalId` pertenece al `RutaTemporalId` enviado en el item de `Paginas`.
> - Chunks enviados con `application/octet-stream` y `X-Total-Chunks`.
> - Body de chunk enviado como binario puro.
> - No se intenta setear manualmente `Content-Length` desde browser.
> - `complete` ejecutado para cada PDF anotado antes del `POST /paginas-anotadas`.
> - Cada PDF anotado contiene exactamente una pagina.
> - `PageNumber` usa numeracion 1-based, no 0-based.
> - No enviar paginas duplicadas.
> - No enviar imagenes como reemplazo: el temporal debe ser PDF.
> - No enviar el PDF completo al endpoint parcial.
> - No guardar `OriginalPdfPassword`.
> - Si la validacion anti-desfase esta activa, reenviar metadata real recibida/calculada al renderizar.
> - Ante rechazo anti-desfase, recargar documento antes de reintentar.
> - Guardar `RequestId` de error o exito para soporte.
> 
> Criterios de aceptacion:
> 
> - El frontend no envia imagenes, canvas, Base64, PNG ni JPEG.
> - Cada temporal representa un PDF anotado de exactamente una pagina.
> - Cada pagina enviada incluye su propio `RutaTemporalId`.
> - El flujo bloquea documentos firmados electronicamente.
> - El flujo usa `clienteApi`, `AbortSignal` y `AppResponses<T>`.
> - El flujo soporta cancelacion y latest-wins.
> - `OriginalPdfPassword` solo vive en memoria y solo viaja en request final cuando aplica.
> - Si hay metadata anti-desfase real, se envia; si no, no se inventa.
> - El reemplazo exitoso refresca el documento visible.
> - No se rompen exportacion, impresion, firma, visualizacion de imagenes ni carga gestionada actual.
> - Hay pruebas unitarias e integracion enfocadas en contrato, chunks, single-page PDF, errores y orquestacion.
> 
> Pruebas obligatorias:
> 
> - Servicio: `init`, chunk binario, `complete`, `cancel`, `paginas-anotadas`, envelope y errores.
> - Utilidades: paginas anotadas, numeracion 1-based, SHA-256, extraccion single-page PDF.
> - Workbench: validaciones, exito, error, cancelacion, latest-wins, refresh y documento firmado.
> - Regresion: exportacion, impresion, firma, imagenes y carga gestionada.
> 
> Comandos sugeridos:
> 
> ```powershell
> npm run typecheck
> npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
> npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service.test.ts
> npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/utils/pdfPageAnnotations.test.ts
> npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/utils/pdfSinglePageExtraction.test.ts
> npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/utils/hashSha256.test.ts
> npx.cmd vitest run src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.test.tsx
> ```
> 
> ## 9. Ejemplo real de integracion frontend
> 
> Regla para esta seccion:
> 
> - Los bloques de codigo de `9.x` son pseudocodigo contractual para explicar secuencia, payloads y responsabilidades.
> - No copiar estos ejemplos literalmente al codigo productivo.
> - La implementacion real debe usar nombres, rutas, tipos, helpers, interceptores y patrones existentes del repositorio.
> - Toda llamada HTTP productiva debe pasar por `clienteApi` o por un wrapper del proyecto que preserve autenticacion, interceptores, cancelacion y manejo centralizado de errores.
> - Si algun ejemplo contradice codigo real existente del proyecto, prevalece el codigo real y se debe adaptar el servicio manteniendo el contrato API.
> 
> ### 9.1 Escenario real multipagina
> 
> El usuario abre el documento `IdDocumento = 15416` del gabinete `contabil`, anota las paginas 2 y 5 en el visor PDF y el frontend genera dos PDFs independientes:
> 
> | Pagina original | Archivo generado por frontend | Regla |
> |---|---|---|
> | 2 | `DIG00015416-page-2-annotated.pdf` | Debe contener exactamente una pagina. |
> | 5 | `DIG00015416-page-5-annotated.pdf` | Debe contener exactamente una pagina. |
> 
> El frontend sube cada archivo por upload temporal y luego llama `paginas-anotadas` en una sola transaccion logica.
> 
> ### 9.2 Tipos sugeridos TypeScript
> 
> ```ts
> type StorageUploadInitRequest = {
>   NombreOriginal: string;
>   TamanoBytes: number;
>   Extension: ".PDF";
>   HashSha256Esperado?: string | null;
>   NumeroChunks: number;
> };
> 
> type StorageUploadInitResponseDto = {
>   RutaTemporalId: string;
>   ArchivoTemporalId: string;
>   ChunkSizeBytes: number;
>   Estado: "IN_PROGRESS" | "COMPLETED" | "CANCELLED";
> };
> 
> type StorageUploadStatusResponseDto = {
>   Estado: "IN_PROGRESS" | "COMPLETED" | "CANCELLED";
>   ChunksRecibidos: number;
>   ChunksPendientes: number;
>   TamanoRecibidoBytes: number;
> };
> 
> type ReemplazarPaginasPdfAnotadasResponse = {
>   IdDocumento: number;
>   NombreGabinete: string;
>   PaginasReemplazadas: number[];
>   RutaArchivoFinal: string;
>   RutaRespaldo: string;
>   TamanoAnteriorBytes: number;
>   TamanoNuevoBytes: number;
>   HashAnteriorSha256: string;
>   HashNuevoSha256: string;
>   RequestId: string;
> };
> 
> type ReemplazarPaginasPdfAnotadasRequest = {
>   NombreGabinete: string;
>   IdDocumento: number;
>   RutaTemporalId?: string;
>   OriginalPdfPassword?: string;
>   SourceDocumentHashSha256?: string;
>   SourceDocumentVersion?: string;
>   Paginas: Array<{
>     PageNumber: number;
>     RutaTemporalId: string;
>     ArchivoTemporalId: string;
>     ContentType: "application/pdf";
>     HashSha256Esperado?: string | null;
>     SourcePageWidth?: number;
>     SourcePageHeight?: number;
>     SourcePageRotation?: number;
>     SourcePageFingerprintSha256?: string;
>   }>;
>   Motivo?: string;
>   DescOp?: string;
>   ModuloRegistro?: "DOCUARCHI" | "PRODUCCION" | "WORKFLOW";
>   Radicado?: string;
>   IdTareaWorkflow?: number;
>   IdRutaWorkflow?: number;
>   TipologiaDocumental?: string;
> };
> ```
> 
> ### 9.3 Cliente HTTP base
> 
> El codigo productivo debe usar `clienteApi`. Este pseudocodigo solo define la forma esperada de desempaquetar `AppResponses<T>` y no debe copiarse literalmente:
> 
> ```ts
> async function unwrapAppResponse<T>(
>   promise: Promise<{ data: AppResponses<T> }>
> ): Promise<T> {
>   const response = await promise;
>   const body = response.data;
> 
>   if (!body.success || body.data == null) {
>     const firstError = body.errors?.[0];
>     throw new Error(
>       `${body.message}: ${firstError?.Field ?? ""} ${firstError?.Message ?? ""}`.trim()
>     );
>   }
> 
>   return body.data;
> }
> ```
> 
> Servicio esperado:
> 
> ```ts
> async function initUploadTemporalPdfAnotado(
>   request: StorageUploadInitRequest,
>   options?: { signal?: AbortSignal }
> ): Promise<StorageUploadInitResponseDto>;
> 
> async function uploadTemporalChunk(
>   params: {
>     rutaTemporalId: string;
>     archivoTemporalId: string;
>     chunkIndex: number;
>     totalChunks: number;
>     chunk: Blob;
>   },
>   options?: { signal?: AbortSignal }
> ): Promise<void>;
> 
> async function completeUploadTemporal(
>   params: { rutaTemporalId: string; archivoTemporalId: string },
>   options?: { signal?: AbortSignal }
> ): Promise<void>;
> 
> async function cancelUploadTemporal(
>   params: { rutaTemporalId: string; archivoTemporalId: string },
>   options?: { signal?: AbortSignal }
> ): Promise<void>;
> 
> async function reemplazarPaginasPdfAnotadas(
>   request: ReemplazarPaginasPdfAnotadasRequest,
>   options?: { signal?: AbortSignal }
> ): Promise<ReemplazarPaginasPdfAnotadasResponse>;
> ```
> 
> ### 9.4 Upload de un PDF anotado de una pagina
> 
> ```ts
> async function uploadAnnotatedSinglePagePdf(file: Blob, pageNumber: number) {
>   const hashSha256 = await calcularSha256(file);
>   const init = await initUploadTemporalPdfAnotado({
>     NombreOriginal: `DIG00015416-PAGINA-${pageNumber}-ANOTADA.PDF`,
>     TamanoBytes: file.size,
>     Extension: ".PDF",
>     HashSha256Esperado: hashSha256,
>     NumeroChunks: 1
>   });
> 
>   await uploadTemporalChunk({
>     rutaTemporalId: init.RutaTemporalId,
>     archivoTemporalId: init.ArchivoTemporalId,
>     chunkIndex: 0,
>     totalChunks: 1,
>     chunk: file
>   });
> 
>   await completeUploadTemporal({
>     rutaTemporalId: init.RutaTemporalId,
>     archivoTemporalId: init.ArchivoTemporalId
>   });
> 
>   return { upload: init, hashSha256 };
> }
> ```
> 
> Para archivos grandes, dividir `file.slice(start, end)` y enviar `chunkIndex` incremental con `X-Total-Chunks`.
> 
> ### 9.5 Reemplazo final de dos paginas
> 
> ```ts
> async function replaceAnnotatedPagesExample(page2Pdf: Blob, page5Pdf: Blob) {
>   const page2Upload = await uploadAnnotatedSinglePagePdf(page2Pdf, 2);
>   const page5Upload = await uploadAnnotatedSinglePagePdf(page5Pdf, 5);
> 
>   const result = await reemplazarPaginasPdfAnotadas({
>     NombreGabinete: "contabil",
>     IdDocumento: 15416,
>     RutaTemporalId: page2Upload.upload.RutaTemporalId,
>     OriginalPdfPassword: undefined,
>     Paginas: [
>       {
>         PageNumber: 2,
>         RutaTemporalId: page2Upload.upload.RutaTemporalId,
>         ArchivoTemporalId: page2Upload.upload.ArchivoTemporalId,
>         ContentType: "application/pdf",
>         HashSha256Esperado: page2Upload.hashSha256
>       },
>       {
>         PageNumber: 5,
>         RutaTemporalId: page5Upload.upload.RutaTemporalId,
>         ArchivoTemporalId: page5Upload.upload.ArchivoTemporalId,
>         ContentType: "application/pdf",
>         HashSha256Esperado: page5Upload.hashSha256
>       }
>     ],
>     Motivo: "Actualizacion de grafo PDF desde visor",
>     DescOp: "AGREGA GRAFO PDF",
>     ModuloRegistro: "DOCUARCHI",
>     Radicado: "2600466700019",
>     IdTareaWorkflow: 12873,
>     IdRutaWorkflow: 45,
>     TipologiaDocumental: "FACTURA"
>   });
> 
>   return {
>     paginas: result.PaginasReemplazadas,
>     requestId: result.RequestId,
>     rutaFinal: result.RutaArchivoFinal
>   };
> }
> ```
> 
> ### 9.6 Respuesta esperada para mostrar en UI
> 
> ```json
> {
>   "paginas": [2, 5],
>   "requestId": "f5db29fb18b94b27878db78f6743aa52",
>   "rutaFinal": "D:/imagenes/discos/CONTABIL7/00093/DIG00015416.PDF"
> }
> ```
> 
> Recomendacion UI:
> 
> - Mostrar mensaje funcional: `Paginas 2 y 5 actualizadas correctamente`.
> - Guardar `RequestId` en el historial visible para soporte.
> - No mostrar rutas fisicas a usuarios finales si la politica de seguridad del producto no lo permite.
> 
> ### 9.7 Compatibilidad
> 
> Para clientes antiguos o pruebas de una sola pagina, el backend acepta `RutaTemporalId` a nivel raiz como fallback. Para clientes nuevos y reemplazo multipagina, enviar siempre `RutaTemporalId` dentro de cada item de `Paginas`.
> 
> ### 9.8 QA manual esperado
> 
> - Abrir PDF normal, anotar una pagina, guardar y verificar reemplazo.
> - Abrir PDF normal, anotar dos o mas paginas, guardar y verificar reemplazo en una sola operacion logica.
> - Cancelar durante upload y verificar limpieza best-effort de temporales.
> - Cambiar de documento durante upload y verificar latest-wins.
> - Intentar guardar documento firmado electronicamente y verificar bloqueo.
> - Probar PDF protegido con password valida.
> - Probar PDF protegido sin password o con password invalida y verificar error en `originalPdfPassword`.
> - Simular rechazo anti-desfase y verificar que UI pide recargar.
> - Confirmar que no se muestran rutas fisicas a usuarios finales cuando politica lo prohibe.
> - Confirmar que exportacion, impresion, firma, visualizacion de imagenes y carga gestionada siguen funcionando.
> 
> ### 9.9 Instruccion final
> 
> Implementar solo con APIs y datos reales del proyecto. No inventar contexto documental, metadata anti-desfase, passwords ni endpoints.
> 
> La solucion debe priorizar contrato correcto, seguridad de password, PDFs reales de una sola pagina, limpieza de temporales, cancelacion, latest-wins y regresion cero sobre el visor actual.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: APPVISOREMBEDPDF, PAGINA, REEMPLAZA

## Capabilities

### New Capabilities
- `reemplaza-pagina-appvisorembedpdf`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
