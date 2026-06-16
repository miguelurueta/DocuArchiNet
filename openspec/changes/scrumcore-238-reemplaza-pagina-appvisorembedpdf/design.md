## Context

SCRUMCORE-238 implementa en frontend el flujo de reemplazo de paginas PDF anotadas desde `AppVisorEmbedPdf`.

El backend no espera imagenes ni un PDF completo para reemplazar el documento. El frontend debe enviar un PDF anotado de una sola pagina por cada pagina modificada, subir cada archivo por upload temporal y llamar a:

`POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`

El backend abre el PDF original, reemplaza las paginas indicadas y genera el PDF final con iText/iText7.

## Current State

- `AppVisorEmbedPdf` ya encapsula EmbedPDF y expone API imperativa `load`, `reset` y `cancelCurrentLoad`.
- El visor ya usa `useAnnotation`, `useAnnotationCapability`, `useExport`, `annotation.state.pages`, `annotationCap.provides.commit()` y `exportApi.provides.saveAsCopy()`.
- `DocumentosWorkbench` ya integra el visor en modo gestionado mediante `visorRef.current?.load(...)`.
- `DocumentosWorkbench` ya conoce `documentViewer.documentoActivo`, `viewerKind`, `nombreGabinete`, `documentId`, `isElectronicallySigned` y cancelacion del visor.
- Los servicios HTTP del proyecto usan `clienteApi` desde `src/api/Clienteaxios`.
- No existe actualmente `pdf-lib` en `package.json`.

## Goals

- Agregar un flujo end-to-end para guardar anotaciones reemplazando paginas PDF completas.
- Mantener `AppVisorEmbedPdf` como unica frontera con EmbedPDF.
- Mantener `DocumentosWorkbench` como orquestador de negocio documental.
- Usar un service dedicado con `clienteApi` para upload temporal y reemplazo final.
- Enviar solo PDFs anotados de una pagina, nunca imagenes, canvas, Base64 ni el PDF completo.
- Manejar cancelacion, latest-wins, limpieza best-effort de temporales y errores de dominio.
- Bloquear documentos firmados electronicamente.
- Preservar exportacion, impresion, firma, imagenes y carga gestionada actual.

## Non-Goals

- No implementar cambios backend.
- No importar DTOs desde repos backend.
- No reemplazar documentos completos.
- No usar rasterizacion ni `pdfjs-dist` para producir el contrato de reemplazo.
- No persistir `OriginalPdfPassword`.
- No inventar metadata anti-desfase si no existe fuente real en frontend.

## Architecture

### Visor

`AppVisorEmbedPdf` debe agregar una API imperativa para exportar paginas anotadas sin filtrar detalles de EmbedPDF:

```ts
type AppVisorAnnotatedPdfPage = {
  pageNumber: number;
  fileName: string;
  blob: Blob;
  sizeBytes: number;
  hashSha256?: string;
  sourcePageWidth?: number;
  sourcePageHeight?: number;
  sourcePageRotation?: number;
  sourcePageFingerprintSha256?: string;
};

type AppVisorAnnotatedPdfPagesExportResult = {
  hasAnnotations: boolean;
  annotatedPages: number[];
  pages: AppVisorAnnotatedPdfPage[];
};
```

El metodo del ref debe:

1. Leer paginas anotadas desde `annotation.state.pages`.
2. Convertir indices internos base 0 a `PageNumber` base 1.
3. Ejecutar `annotationCap.provides.commit()`.
4. Ejecutar `exportApi.provides.saveAsCopy()`.
5. Extraer un PDF de una pagina por pagina anotada.
6. Retornar `Blob` tipo `application/pdf`.

El visor no debe llamar APIs, conocer workflow ni exponer tipos `@embedpdf/*`.

### Extraccion PDF

La extraccion debe usar un mecanismo PDF real, sin rasterizacion.

Decision pendiente:

- Si se aprueba `pdf-lib`, agregarla con documentacion de licencia, peso e impacto bundle, preferiblemente con import dinamico.
- Si no se aprueba una dependencia frontend o no hay mecanismo real disponible, bloquear la fase de extraccion y documentar el bloqueo. No sustituir por imagenes.

### Servicio HTTP

Crear un service dedicado bajo `src/app/Components/UI/AppVisorEmbedPdf/services/` para:

- `initUploadTemporalPdfAnotado`
- `uploadTemporalChunk`
- `completeUploadTemporal`
- `cancelUploadTemporal`
- `reemplazarPaginasPdfAnotadas`

Reglas:

- Usar `clienteApi`.
- Propagar `AbortSignal`.
- Desempaquetar `AppResponses<T>`.
- Preservar errores `Field` y `Message`.
- Enviar chunks como body binario puro (`Blob`, `File` o `ArrayBuffer`).
- Enviar `Content-Type: application/octet-stream` y `X-Total-Chunks`.
- No setear manualmente `Content-Length` desde browser; es header restringido.
- No usar `FormData` salvo cambio explicito de contrato.

### Workbench

`DocumentosWorkbench` debe coordinar:

1. Validar documento activo, PDF, permisos y documento no firmado.
2. Invocar exportacion del visor.
3. Subir cada pagina anotada por upload temporal.
4. Completar cada temporal.
5. Llamar `paginas-anotadas` con `RutaTemporalId` por cada item de `Paginas`.
6. Refrescar el documento visible en success.
7. Limpiar temporales best-effort si se cancela o falla antes del reemplazo final.

Debe mantener latest-wins por secuencia o mecanismo equivalente: si cambia el documento activo durante el flujo, abortar la operacion anterior y evitar aplicar resultados obsoletos.

### Toolbar

`AppPdfToolbar` debe seguir siendo presentacional:

- Recibe callback de guardar paginas anotadas.
- Recibe flags de habilitado/deshabilitado.
- Recibe estado de progreso si aplica.
- No conoce `clienteApi`, endpoints, workflow ni EmbedPDF.

## API Contract

Endpoints:

- `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`
- `PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- `GET /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
- `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
- `DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`
- `POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`

Reglas criticas:

- Cada `init` crea un `RutaTemporalId`.
- En reemplazo multipagina, cada item de `Paginas` debe enviar su propio `RutaTemporalId`.
- `RutaTemporalId` raiz existe solo como fallback compatible.
- `PageNumber` es base 1.
- Cada temporal debe estar `COMPLETED` antes del reemplazo final.
- El backend consume y elimina temporales usados cuando `paginas-anotadas` responde exitosamente; no invocar `DELETE` despues de success.

## Security

- `Authorization: Bearer {jwt}` lo maneja `clienteApi`.
- Claims esperados en JWT: `defaulalias` y `usuarioid`.
- `OriginalPdfPassword` solo puede vivir en memoria volatil y viajar en el request final cuando el PDF original protegido lo requiera.
- Limpiar password en reset, cambio de documento, cancelacion, cierre de visor o desmontaje.
- No registrar password, JWT, blobs completos ni rutas fisicas sensibles.

## Error Handling

Errores esperados:

- Validacion de request incompleto, paginas duplicadas o `PageNumber <= 0`.
- Temporal inexistente, no `COMPLETED`, extension no PDF o hash inconsistente.
- PDF temporal sin exactamente una pagina.
- Pagina fuera de rango.
- Documento firmado electronicamente.
- PDF original protegido sin password o password invalida (`Field = originalPdfPassword`).
- Rechazo anti-desfase.
- Fallo no controlado de preparacion, reemplazo o auditoria.

UI:

- Mostrar mensajes funcionales.
- Conservar documento visible ante fallo.
- No mostrar error por cancelacion explicita.
- Preservar `RequestId` para soporte si viene en response.

## Observability

Usar el patron existente `window.__DV_DEBUG__` para logs no sensibles.

Permitido:

- intento, documento, paginas, tamanos, progreso, duracion, `RequestId`.

Prohibido:

- bytes del PDF, password, JWT, rutas fisicas sensibles, payload completo si contiene datos sensibles.

## Risks / Trade-offs

- **Dependencia PDF frontend:** se requiere mecanismo real para extraer paginas. Mitigar con decision explicita sobre `pdf-lib` o bloqueo documentado.
- **Memoria:** `saveAsCopy()` exporta PDF anotado completo antes de extraer paginas. Mitigar evitando Base64, copias innecesarias y liberando referencias.
- **Content-Length:** browser no permite setearlo manualmente. Mitigar enviando body binario crudo y validando en QA real.
- **Cambio de documento durante upload:** mitigar con `AbortController`, latest-wins y limpieza best-effort.
- **Anti-desfase:** solo enviar metadata si existe fuente real. Si no, documentar pendiente.
- **Password:** mitigar con memoria volatil y limpieza estricta.

## Migration Plan

1. Crear tipos y service HTTP con tests de contrato.
2. Agregar utilidades de paginas anotadas, hash y extraccion single-page PDF.
3. Extender API imperativa del visor con tests.
4. Integrar boton/callback presentacional en toolbar.
5. Orquestar flujo en `DocumentosWorkbench`.
6. Agregar pruebas de integracion y regresion.
7. Documentar decision de dependencia PDF, QA y limitaciones.

## Open Questions

- Se aprobara `pdf-lib` para extraccion de paginas PDF en frontend?
- El flujo actual expone `OriginalPdfPassword` de forma recuperable solo en memoria desde el visor?
- El backend/frontend actual expone metadata confiable para anti-desfase?
- Cual es el origen exacto de `IdRutaWorkflow` y `TipologiaDocumental` en `DocumentosWorkbench` para este flujo?
