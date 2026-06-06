# PROMPT ARQUITECTONICO ENTERPRISE - SCRUM-249

## Integracion React para Reemplazo de Paginas PDF Anotadas desde AppVisorEmbedPdf

**Version:** Enterprise Final alineada al proyecto actual DocuArchiCore.react  
**Fecha base:** 2026-06-06  
**Repositorio objetivo:** DocuArchiCore.react  
**Scope:** Frontend React + TypeScript  
**Componente frontera EmbedPDF:** `src/app/Components/UI/AppVisorEmbedPdf`  
**Componente consumidor principal:** `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`  
**Endpoint parcial objetivo:** `POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`

## 1. Rol esperado

Actua como Arquitecto de Software Senior Frontend React + TypeScript especialista en React, TypeScript estricto, Vite, Vitest, EmbedPDF/Pdfium Engine, `@embedpdf/plugin-annotation`, `@embedpdf/plugin-export`, `@embedpdf/plugin-print`, encapsulacion enterprise de visores PDF, servicios API con `clienteApi` y Axios centralizado, upload temporal por chunks, manejo de archivos grandes, `Blob`, `ArrayBuffer`, `Uint8Array`, `slice()`, cancelacion con `AbortController`, patron latest-wins, integracion con backend ASP.NET Core, contratos HTTP tipo `AppResponses<T>`, pruebas unitarias e integracion FE, observabilidad controlada con `window.__DV_DEBUG__` y documentacion tecnica enterprise.

## 2. Contexto real del proyecto actual

Este prompt aplica al repositorio React actual `DocuArchiCore.react`.

Usar rutas relativas al workspace. No depender de rutas locales absolutas salvo para documentacion generada.

Ruta principal del visor:

`src/app/Components/UI/AppVisorEmbedPdf`

Archivos actuales relevantes:

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.types.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/types/AppVisorEmbedPdfProps.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.ts`

El visor actual ya contiene:

- `AnnotationLayer`.
- `useAnnotation(documentId)`.
- `useAnnotationCapability()`.
- `useExport(documentId)`.
- `annotationCap.provides.commit()`.
- `exportApi.provides.saveAsCopy()`.
- `annotation.state.pages`.
- toolbar presentacional `AppPdfToolbar`.
- API imperativa actual `load/reset/cancelCurrentLoad`.
- permisos efectivos `ViewerEffectivePermissions`.
- flujo de firma/anotaciones como `STAMP`/`INK`.
- `waitPdfTask` y `waitPdfTaskVoid`.
- debug local con `window.__DV_DEBUG__`.
- politica fail-closed para permisos del visor.

Ruta del consumidor principal:

`src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

El Workbench actual ya contiene:

- `visorRef`.
- `documentViewer.documentoActivo`.
- `activeFileUrl`.
- `activeRowId`.
- `idTareaWf`.
- `documentosTable.getWorkbenchContext?.()`.
- flujo `visualizarDocumento`.
- control de loading/toast/error.
- `viewerKind` para PDF/imagen.
- `isElectronicallySigned` en contexto activo.
- `visorRef.current?.cancelCurrentLoad()`.
- flujo managed `visorRef.current?.load({...})` activo con `nombre_modulo: "gestioncorrespondencia"`.

Importante:

El prompt original mencionaba una brecha donde `load()` estaba comentado. En el proyecto actual esa brecha ya fue parcialmente resuelta: `DocumentosWorkbench` si llama `visorRef.current?.load(...)` para PDFs. Por tanto, cualquier implementacion futura debe preservar este modo gestionado y no volver a una integracion exclusivamente por `fileUrl` directo.

El proyecto ya tiene dependencias EmbedPDF, Axios, Vitest, Testing Library, `pdfjs-dist` y `react-pdf`.

El proyecto no tiene actualmente `pdf-lib` en `package.json`.

Decision obligatoria:

Para extraer PDFs de una sola pagina desde el PDF anotado completo, se requiere una libreria frontend segura que permita cargar PDF, copiar paginas y guardar un nuevo PDF. Si no existe una dependencia ya aprobada en el proyecto, evaluar agregar `pdf-lib` mediante decision tecnica explicita.

No implementar extraccion por pagina si no existe mecanismo real sin rasterizacion.

## 3. Fuente de verdad del contrato FE

Este prompt es autocontenido para frontend.

No importar codigo, DTOs ni tipos desde backend.

Referencias backend opcionales para auditoria:

- API: `https://github.com/miguelurueta/DocuArchi.Api.git`
- DTOs: `https://github.com/miguelurueta/MiApp.DTOs.git`
- Services: `https://github.com/miguelurueta/MiApp.Services.git`
- Repository: `https://github.com/miguelurueta/MiApp.Repository.git`
- Documentacion/core: `https://github.com/miguelurueta/DocuArchiCore.git`

Reglas:

- No clonar ni leer estos repos como requisito normal de implementacion FE.
- Usarlos solo para auditoria de contrato, revision tecnica o resolucion de dudas.
- Si Swagger/OpenAPI publicado contradice este prompt, escalar discrepancia antes de implementar.
- Si backend confirma contrato distinto, actualizar primero `design/spec/tasks` antes de tocar codigo.

## 4. Endpoint objetivo y endpoints existentes

Endpoint objetivo parcial:

```http
POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas
Authorization: Bearer {jwt}
Content-Type: application/json
```

Upload temporal existente:

```http
POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init
PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
GET /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
```

Cliente HTTP obligatorio:

Usar el cliente centralizado del proyecto: `clienteApi`.

No usar `axios` directo en nuevos servicios salvo que el patron actual del proyecto lo exija de forma demostrable.

Regla enterprise preferida:

- HTTP nuevo en service dedicado.
- `clienteApi` como frontera HTTP.
- `AbortSignal` propagado.
- envelope backend desempaquetado y validado.
- errores de dominio normalizados.

## 5. Correccion funcional critica

La implementacion FE no debe enviar imagenes.

La implementacion FE no debe rasterizar paginas a PNG/JPEG.

La implementacion FE debe enviar paginas PDF anotadas, donde cada archivo temporal es un PDF de una sola pagina.

Definicion exacta:

1. Detectar paginas con anotaciones en el visor.
2. Materializar anotaciones con `annotationCap.provides.commit()`.
3. Exportar PDF anotado completo con `exportApi.provides.saveAsCopy()`.
4. Extraer un PDF de una sola pagina por cada pagina anotada.
5. Subir cada PDF de una sola pagina al upload temporal existente.
6. Enviar al endpoint `/paginas-anotadas` la lista `{ PageNumber, RutaTemporalId, ArchivoTemporalId, ContentType: "application/pdf", HashSha256Esperado }`.
7. El backend reemplaza paginas PDF completas, no imagenes internas.

Correccion de contrato SCRUM-249:

- El upload temporal actual crea un `RutaTemporalId` por cada `init`.
- Para reemplazo multipagina, cada item de `Paginas` debe enviar su propio `RutaTemporalId`.
- `RutaTemporalId` raiz existe solo como fallback compatible para clientes de una sola ruta.
- No forzar una ruta temporal comun en frontend.
- El backend retorna y espera `AppResponses<T>`.
- El servicio FE debe desempaquetar `data` y propagar `errors`.
- El endpoint acepta `OriginalPdfPassword` opcional para PDF original protegido.
- El FE solo puede conservar `OriginalPdfPassword` en memoria.
- El endpoint acepta validacion anti-desfase opcional con `SourceDocumentHashSha256`, `SourceDocumentVersion`, `SourcePageWidth`, `SourcePageHeight`, `SourcePageRotation` y `SourcePageFingerprintSha256`.
- El backend abre el PDF original, reemplaza solo las paginas indicadas y genera el PDF final con iText/iText7.
- El PDF anotado de una pagina puede tener tamano u orientacion diferente al original.
- El backend conserva tamano/orientacion de la pagina original y ajusta el contenido anotado dentro de esa caja.
- `SourcePageWidth`, `SourcePageHeight`, `SourcePageRotation` y `SourcePageFingerprintSha256` son metadata de validacion anti-desfase, no instrucciones para definir el tamano final de la pagina.

Prohibido:

- enviar `image/png`.
- enviar `image/jpeg`.
- usar canvas como contrato de integracion.
- usar `pdfjs-dist` para rasterizar.
- enviar el PDF completo si el endpoint parcial esta disponible.
- exponer `@embedpdf/*` fuera del visor.
- importar DTOs backend.
- persistir contrasenas PDF.
- loguear contrasenas PDF.

## 6. Objetivo

Implementar en este repositorio React un flujo end-to-end para detectar paginas PDF con anotaciones, exportar paginas PDF anotadas de una sola pagina, subir esas paginas por chunks usando el upload temporal existente, llamar a la nueva API de reemplazo de paginas PDF anotadas, manejar progreso, errores, cancelacion y latest-wins, refrescar el documento visible tras reemplazo exitoso, bloquear reemplazo si el documento esta firmado electronicamente, no romper exportacion actual, impresion actual, firma actual ni visualizacion de imagenes, y dejar pruebas/documentacion enterprise completas.

## 7. Decision arquitectonica principal

`AppVisorEmbedPdf` debe encapsular toda la logica relacionada con EmbedPDF:

- conocer `documentId` interno del engine.
- conocer `annotation.state.pages`.
- ejecutar `commit()`.
- ejecutar `saveAsCopy()`.
- exportar paginas PDF anotadas.
- no subir archivos a backend.
- no llamar endpoints de reemplazo.
- no conocer reglas de negocio de workflow.
- no exponer tipos `@embedpdf/*` hacia consumidores.

`DocumentosWorkbench` debe orquestar negocio:

- tomar contexto documental.
- validar documento activo.
- validar si es PDF.
- validar firma electronica.
- llamar metodo imperativo del visor.
- llamar servicio de upload temporal.
- llamar API `/paginas-anotadas`.
- mostrar UX/progreso.
- reabrir/refrescar documento.
- preservar seleccion activa si falla.

El servicio API debe encapsular HTTP:

- init upload.
- chunk upload.
- complete.
- cancel best-effort.
- replace pages.
- desempaquetado `AppResponses<T>`.
- normalizacion PascalCase/camelCase si aplica.
- errores de dominio.

`AppPdfToolbar` debe seguir siendo presentacional:

- recibe callbacks.
- recibe flags.
- recibe progreso si aplica.
- no conoce HTTP.
- no conoce EmbedPDF.
- no conoce `clienteApi`.
- no conoce workflow.

## 8. Archivos esperados

Modificar:

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.types.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/types/AppVisorEmbedPdfProps.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- tests existentes de `AppVisorEmbedPdf`
- tests existentes de `DocumentosWorkbench`

Crear:

- `src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.types.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service.test.ts`

Crear opcionalmente:

- `src/app/Components/UI/AppVisorEmbedPdf/utils/pdfPageAnnotations.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/utils/pdfPageAnnotations.test.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/utils/pdfSinglePageExtraction.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/utils/pdfSinglePageExtraction.test.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/utils/hashSha256.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/utils/hashSha256.test.ts`

No crear servicios HTTP dentro de `gestionCorrespondencia` salvo dependencia fuerte y justificada de workflow.

Preferir servicio neutral dentro del visor para reutilizacion por otros modulos.

## 9. API imperativa del visor

Actualizar:

`src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.types.ts`

Tipos nuevos:

```ts
export type AppVisorAnnotatedPdfPage = {
  pageNumber: number;
  fileName: string;
  blob: Blob;
  sizeBytes: number;
  hashSha256?: string;
};

export type AppVisorAnnotatedPdfPagesExportResult = {
  hasAnnotations: boolean;
  annotatedPages: number[];
  pages: AppVisorAnnotatedPdfPage[];
  totalSizeBytes: number;
};
```

Extender ref:

```ts
export type AppVisorEmbedPdfRef = {
  load(input: AppVisorLoadInput): Promise<AppVisorLoadResult>;
  reset(): void;
  cancelCurrentLoad(): void;
  exportAnnotatedPdfPages(): Promise<AppVisorAnnotatedPdfPagesExportResult>;
};
```

Reglas:

- `annotatedPages` usa numeracion humana base 1.
- `annotation.state.pages` usa pageIndex base 0.
- Si no hay anotaciones, retornar `hasAnnotations: false`, `annotatedPages: []`, `pages: []`.
- No descargar archivos.
- No subir a backend.
- No exponer `documentId`.
- No exponer tipos de EmbedPDF.
- No modificar print/export/firma existentes.
- No hacer side effects de negocio.

## 10. Deteccion de paginas anotadas

Usar exclusivamente `annotation.state.pages`.

Funcion pura recomendada:

```ts
export function getAnnotatedPageNumbers(pagesState: Record<string, unknown>): number[] {
  return Array.from(
    new Set(
      Object.entries(pagesState ?? {})
        .filter(([, ids]) => Array.isArray(ids) && ids.length > 0)
        .map(([pageIndex]) => Number(pageIndex) + 1)
        .filter((value) => Number.isFinite(value) && value > 0),
    ),
  ).sort((a, b) => a - b);
}
```

No usar DOM, canvas, thumbnails, seleccion de texto, inferencias visuales ni screenshots.

## 11. Exportacion de paginas PDF anotadas

Objetivo:

Obtener un PDF de una sola pagina por cada pagina anotada.

Flujo obligatorio dentro del visor:

1. Validar documento activo.
2. Validar que `annotationCap.provides?.commit` existe.
3. Validar que `exportApi.provides?.saveAsCopy` existe.
4. Calcular `annotatedPages` desde `annotation.state.pages`.
5. Si no hay anotaciones, retornar resultado vacio.
6. Ejecutar commit:

```ts
await waitPdfTaskVoid(annotationCap.provides.commit());
```

7. Obtener PDF anotado completo:

```ts
const buffer = await waitPdfTask<ArrayBuffer | Uint8Array<ArrayBufferLike>>(
  exportApi.provides.saveAsCopy(),
);
```

Importante:

En el proyecto actual `saveAsCopy()` se usa sin parametro. No usar `saveAsCopy(documentId)` salvo que una version futura del adapter lo requiera y se valide en codigo.

8. Convertir bytes a `BlobPart` usando util existente o equivalente seguro:

```ts
function toPdfBlobPart(buffer: ArrayBuffer | Uint8Array<ArrayBufferLike>): BlobPart {
  if (buffer instanceof ArrayBuffer) return buffer;

  const source = new Uint8Array(buffer.buffer, buffer.byteOffset, buffer.byteLength);
  const copy = new Uint8Array(source.byteLength);
  copy.set(source);
  return copy.buffer;
}
```

9. A partir del PDF anotado completo, extraer un PDF de una sola pagina por cada `pageNumber`.

Decision de extraccion:

Primero inspeccionar dependencias existentes.

Si no existe libreria frontend segura para manipular PDF por paginas, evaluar agregar `pdf-lib`.

Decision obligatoria antes de implementar:

- Si se aprueba `pdf-lib`, documentar licencia, peso e impacto bundle.
- Si no se aprueba `pdf-lib`, bloquear esta parte y escalar alternativa tecnica.
- No implementar extraccion por imagen.
- No usar `pdfjs-dist` para rasterizar.
- No usar canvas.

Requisito tecnico de la libreria:

- cargar PDF desde `ArrayBuffer` o `Uint8Array`.
- copiar una pagina concreta a un documento nuevo.
- guardar PDF resultante como bytes.
- no rasterizar.

Pseudocodigo con libreria tipo `pdf-lib`:

```ts
const source = await PDFDocument.load(pdfBytes);
for (const pageNumber of annotatedPages) {
  const pageIndex = pageNumber - 1;
  if (pageIndex < 0 || pageIndex >= source.getPageCount()) {
    throw new Error(`Pagina anotada fuera de rango: ${pageNumber}.`);
  }

  const single = await PDFDocument.create();
  const [copied] = await single.copyPages(source, [pageIndex]);
  single.addPage(copied);
  const singleBytes = await single.save();
  const blob = new Blob([singleBytes], { type: "application/pdf" });
}
```

Validaciones:

- `pageNumber - 1` debe estar dentro del rango del PDF exportado.
- cada blob generado debe tener `type = "application/pdf"`.
- cada blob generado debe tener `size > 0`.
- cada archivo debe tener una pagina por construccion.
- si falla una pagina, fallar toda la operacion con error controlado.

Nombre sugerido:

```ts
`annotated-page-${documentId}-${pageNumber}.pdf`
```

No descargar archivos.

## 12. Hash SHA-256 en frontend

Calcular hash por cada PDF de pagina usando WebCrypto:

```ts
export async function sha256Hex(blob: Blob): Promise<string | undefined> {
  if (typeof crypto === "undefined" || !crypto.subtle) {
    return undefined;
  }

  const buffer = await blob.arrayBuffer();
  const digest = await crypto.subtle.digest("SHA-256", buffer);
  return Array.from(new Uint8Array(digest))
    .map((b) => b.toString(16).padStart(2, "0"))
    .join("");
}
```

Reglas:

- Si `crypto.subtle` no esta disponible, permitir envio sin hash si backend lo trata como opcional.
- No bloquear navegadores por falta de WebCrypto.
- Registrar debug controlado si falta WebCrypto.
- No convertir a base64.
- No guardar hash en localStorage.
- No guardar hash en sessionStorage.

## 13. Validacion anti-desfase y PDF protegido

Tipos FE opcionales:

```ts
export type AppVisorSourceDocumentSnapshot = {
  sourceDocumentHashSha256?: string;
  sourceDocumentVersion?: string;
  pages?: SourcePageSnapshot[];
};

export type SourcePageSnapshot = {
  pageNumber: number;
  sourcePageWidth?: number;
  sourcePageHeight?: number;
  sourcePageRotation?: number;
  sourcePageFingerprintSha256?: string;
};
```

Regla de implementacion:

Estos campos son opcionales y solo deben enviarse si el frontend tiene fuente real y confiable.

No inventar `SourceDocumentHashSha256`, `SourceDocumentVersion`, `SourcePageWidth`, `SourcePageHeight`, `SourcePageRotation` ni `SourcePageFingerprintSha256`.

Si el FE puede calcular `sourceDocumentHashSha256` del PDF original renderizado, enviarlo.

Si el visor expone geometria de pagina, conservar por pagina `sourcePageWidth`, `sourcePageHeight` y `sourcePageRotation`.

Si se calcula fingerprint de pagina, usar SHA-256 sobre una cadena estable equivalente a:

```text
width|height|rotation
```

La normalizacion debe coincidir con backend. Si no esta confirmada, no enviar fingerprint.

Si backend responde validacion por `sourceDocumentHashSha256`, `sourcePageWidth`, `sourcePageHeight`, `sourcePageRotation` o `sourcePageFingerprintSha256`, mostrar:

```text
El documento cambio desde que fue abierto. Recarga el PDF antes de guardar las anotaciones.
```

No ocultar documento visible. No resetear seleccion.

`OriginalPdfPassword` aplica solo al PDF original almacenado, no a los temporales anotados.

Reglas:

- Enviarlo solo si el usuario ya lo ingreso para abrir/anotar el PDF.
- Mantenerlo solo en memoria durante la sesion de visualizacion.
- No persistirlo en storage.
- No loguearlo.
- No enviarlo a telemetria.
- No incluirlo en debug logs.
- Si backend responde error en `originalPdfPassword`, solicitar de nuevo la contrasena o abortar guardado sin limpiar documento visible.

## 14. Servicio HTTP - tipos

Crear:

`src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.types.ts`

```ts
export type UploadTemporalInitRequest = {
  NombreOriginal: string;
  TamanoBytes: number;
  Extension: ".pdf" | ".PDF";
  HashSha256Esperado?: string;
  NumeroChunks: number;
};

export type UploadTemporalInitResponse = {
  RutaTemporalId: string;
  ArchivoTemporalId: string;
  ChunkSizeBytes: number;
  Estado: string;
};

export type UploadTemporalChunkResponse = {
  chunkIndex: number;
};

export type UploadTemporalStatusResponse = {
  Estado: "IN_PROGRESS" | "COMPLETED" | "CANCELLED" | string;
  ChunksRecibidos: number;
  ChunksPendientes: number;
  TamanoRecibidoBytes: number;
};

export type UploadTemporalCompleteResponse = {
  Estado: "COMPLETED" | string;
};

export type UploadTemporalCancelResponse = {
  Estado: "CANCELLED" | string;
};

export type UploadPageResult = {
  pageNumber: number;
  rutaTemporalId: string;
  archivoTemporalId: string;
  hashSha256?: string;
  sizeBytes: number;
};

export type AppResponses<T> = {
  success: boolean;
  message: string;
  data: T | null;
  meta?: {
    Status?: string;
    status?: string;
    Total?: number;
    total?: number;
  };
  errors?: Array<{
    Type?: string;
    type?: string;
    Field?: string;
    field?: string;
    Message?: string;
    message?: string;
  }>;
};

export type SourcePageSnapshot = {
  pageNumber: number;
  sourcePageWidth?: number;
  sourcePageHeight?: number;
  sourcePageRotation?: number;
  sourcePageFingerprintSha256?: string;
};

export type ReemplazarPaginaPdfAnotadaTemporalDto = {
  PageNumber: number;
  RutaTemporalId: string;
  ArchivoTemporalId: string;
  ContentType?: "application/pdf";
  HashSha256Esperado?: string | null;
  SourcePageWidth?: number;
  SourcePageHeight?: number;
  SourcePageRotation?: number;
  SourcePageFingerprintSha256?: string;
};

export type ReemplazarPaginasPdfAnotadasRequest = {
  NombreGabinete: string;
  IdDocumento: number;
  RutaTemporalId?: string;
  OriginalPdfPassword?: string;
  SourceDocumentHashSha256?: string;
  SourceDocumentVersion?: string;
  Paginas: ReemplazarPaginaPdfAnotadaTemporalDto[];
  Motivo?: string;
  DescOp?: "AGREGA GRAFO PDF" | "AGREGAR GRAFO MANUSCRITO";
  ModuloRegistro?: "DOCUARCHI" | "PRODUCCION" | "WORKFLOW" | string;
  Radicado?: string;
  IdTareaWorkflow?: number;
  IdRutaWorkflow?: number;
  TipologiaDocumental?: string;
};

export type ReemplazarPaginasPdfAnotadasResponse = {
  IdDocumento: number;
  NombreGabinete: string;
  PaginasReemplazadas: number[];
  RutaArchivoFinal: string;
  RutaRespaldo: string;
  TamanoAnteriorBytes: number;
  TamanoNuevoBytes: number;
  HashAnteriorSha256: string;
  HashNuevoSha256: string;
  RequestId: string;
};
```

Reglas:

- Consumir siempre wrapper `AppResponses<T>`.
- Si `success !== true`, lanzar error de dominio usando `message` y primer `errors[]`.
- Si `data` es `null` en respuesta exitosa de endpoint que requiere datos, tratarlo como error contractual.
- Crear adaptadores tolerantes que lean PascalCase y camelCase si backend o mocks varian.
- Para reemplazo multipagina, `ReemplazarPaginaPdfAnotadaTemporalDto.RutaTemporalId` es obligatorio en cada item.
- `ReemplazarPaginasPdfAnotadasRequest.RutaTemporalId` puede enviarse como fallback con el primer `RutaTemporalId`.
- El campo raiz `RutaTemporalId` no debe sustituir el `RutaTemporalId` por pagina.

## 15. Servicio HTTP - envelope y errores

Crear utilidades internas del service:

```ts
function getErrorText(error?: {
  Type?: string;
  type?: string;
  Field?: string;
  field?: string;
  Message?: string;
  message?: string;
}): string | null {
  const field = error?.Field ?? error?.field;
  const message = error?.Message ?? error?.message;
  if (field && message) return `${field}: ${message}`;
  if (message) return message;
  return null;
}

function unwrapAppResponse<T>(envelope: AppResponses<T>, fallbackMessage: string): T {
  const firstError = Array.isArray(envelope?.errors) ? getErrorText(envelope.errors[0]) : null;
  if (!envelope?.success) {
    throw new Error(firstError || envelope?.message || fallbackMessage);
  }
  if (envelope.data == null) {
    throw new Error(`${fallbackMessage}: respuesta sin data.`);
  }
  return envelope.data;
}
```

Reglas:

- No ocultar `errors[]`.
- No perder `Field`/`field` porque se requiere para `originalPdfPassword` y anti-desfase.
- Mantener `AbortSignal` en todas las operaciones.
- Si Axios cancela, propagar cancelacion como cancelacion.
- No convertir errores de cancelacion en toast visible.

## 16. Servicio HTTP - upload temporal por pagina

Crear:

`src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service.ts`

Funcion:

```ts
export async function uploadAnnotatedPdfPageTemporal(params: {
  pageNumber: number;
  blob: Blob;
  fileName: string;
  hashSha256?: string;
  signal?: AbortSignal;
  onProgress?: (progress: {
    pageNumber: number;
    uploadedBytes: number;
    totalBytes: number;
    chunkIndex: number;
    totalChunks: number;
    percent: number;
  }) => void;
}): Promise<UploadPageResult>;
```

Flujo:

1. Validar `blob.type === "application/pdf"`.
2. Validar `blob.size > 0`.
3. Calcular `NumeroChunks` con chunk preliminar backend default documentado: 1 MB (`1048576`).
4. Mantener el chunk preliminar configurable para cambios futuros del backend.
4. Llamar init:

```http
POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init
```

Body:

```json
{
  "NombreOriginal": "annotated-page-doc-2.pdf",
  "TamanoBytes": 12345,
  "Extension": ".PDF",
  "HashSha256Esperado": "...",
  "NumeroChunks": 1
}
```

5. Leer `ChunkSizeBytes` real retornado por backend.
6. Recalcular `totalChunks` con `ChunkSizeBytes`.
7. Si total recalculado difiere del `NumeroChunks` enviado en init, abortar con error claro antes de subir chunks.

Motivo:

`StorageUploadPolicy` valida que `X-Total-Chunks` coincida con `NumeroChunks` persistido durante `init`. Si backend retorna otro chunk size y cambia el total, subir chunks produciria inconsistencia.

8. Subir chunks:

```http
PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
```

Headers:

```ts
{
  "Content-Type": "application/octet-stream",
  "X-Total-Chunks": String(totalChunks),
}
```

Body:

```ts
blob.slice(start, end)
```

Notas:

- Backend exige `Content-Length`.
- En navegador no fijar manualmente `Content-Length`; XHR/fetch lo calcula desde `Blob`.
- En pruebas fuera de navegador, mocks deben simular body tipo Blob.

9. Completar:

```http
POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
```

10. Validar que `complete` retorna `Estado: "COMPLETED"`.
11. Consultar status cuando se requiera confirmacion explicita:

```http
GET /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
```

Response esperado:

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "Estado": "COMPLETED",
    "ChunksRecibidos": 1,
    "ChunksPendientes": 0,
    "TamanoRecibidoBytes": 251004
  },
  "errors": []
}
```

12. Antes de llamar `/paginas-anotadas`, cada temporal debe estar `COMPLETED` por respuesta de complete o status.
13. Retornar:

```ts
{
  pageNumber,
  rutaTemporalId,
  archivoTemporalId,
  hashSha256,
  sizeBytes: blob.size,
}
```

Regla clave:

Cada init puede retornar un `RutaTemporalId` distinto. Conservar el par `{ rutaTemporalId, archivoTemporalId }` por pagina.

No exigir ruta comun.

Extension:

- El contrato backend documenta `.PDF`.
- El FE puede aceptar `.pdf` internamente, pero debe normalizar a `.PDF` si el backend lo requiere.
- Las pruebas deben cubrir `.PDF` como contrato oficial.

## 17. Servicio HTTP - subida de todas las paginas

Funcion:

```ts
export async function uploadAnnotatedPdfPagesTemporal(params: {
  pages: Array<{
    pageNumber: number;
    blob: Blob;
    fileName: string;
    hashSha256?: string;
  }>;
  signal?: AbortSignal;
  onProgress?: (progress: {
    uploadedPages: number;
    totalPages: number;
    currentPageNumber: number;
    uploadedBytes: number;
    totalBytes: number;
    percent: number;
  }) => void;
}): Promise<{
  rutaTemporalId?: string;
  pages: UploadPageResult[];
}>;
```

Reglas:

- Subir secuencialmente para reducir presion de red/memoria.
- Si se paraleliza en el futuro, maximo configurable y documentado.
- No validar que todas las paginas retornan mismo `rutaTemporalId`.
- Retornar `rutaTemporalId` raiz opcional con el primer `rutaTemporalId` solo como fallback compatible.
- Validar que cada pagina tiene su propio `rutaTemporalId` y `archivoTemporalId`.
- Si falla una pagina despues de init, cancelar todos los temporales creados best-effort.
- No ocultar el error original.
- Si aborta la operacion, cancelar temporales creados best-effort.

## 18. Servicio HTTP - cancelacion temporal

Funcion:

```ts
export async function cancelUploadTemporal(params: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  signal?: AbortSignal;
}): Promise<void>;
```

Endpoint:

```http
DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
```

Uso:

- best-effort si falla chunk.
- best-effort si falla complete.
- best-effort si el usuario abandona el flujo antes del reemplazo final.
- best-effort al abortar operacion.
- best-effort si falla `paginas-anotadas` antes de un reemplazo exitoso.

No mostrar error de cancelacion al usuario si ya existe error principal.

Regla contractual:

- Si `paginas-anotadas` responde exitosamente, no invocar `DELETE`.
- Cuando el reemplazo final es exitoso, el backend ya consumio y elimino los temporales usados por el request.
- `DELETE` aplica para cancelacion, abort, abandono del usuario o error antes de un replace exitoso.

## 19. Servicio HTTP - reemplazo paginas PDF anotadas

Funcion:

```ts
export async function replaceAnnotatedPdfPages(params: {
  nombreGabinete: string;
  idDocumento: number;
  rutaTemporalId?: string;
  originalPdfPassword?: string;
  sourceDocumentHashSha256?: string;
  sourceDocumentVersion?: string;
  paginas: Array<{
    pageNumber: number;
    rutaTemporalId: string;
    archivoTemporalId: string;
    hashSha256?: string;
    sourcePageWidth?: number;
    sourcePageHeight?: number;
    sourcePageRotation?: number;
    sourcePageFingerprintSha256?: string;
  }>;
  motivo?: string;
  descOp?: "AGREGA GRAFO PDF" | "AGREGAR GRAFO MANUSCRITO";
  moduloRegistro?: string;
  radicado?: string;
  idTareaWorkflow?: number;
  idRutaWorkflow?: number;
  tipologiaDocumental?: string;
  signal?: AbortSignal;
}): Promise<ReemplazarPaginasPdfAnotadasResponse>;
```

Endpoint:

```http
POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas
```

Body:

```ts
{
  NombreGabinete: nombreGabinete,
  IdDocumento: idDocumento,
  RutaTemporalId: rutaTemporalId ?? paginas[0]?.rutaTemporalId,
  OriginalPdfPassword: originalPdfPassword,
  SourceDocumentHashSha256: sourceDocumentHashSha256,
  SourceDocumentVersion: sourceDocumentVersion,
  Paginas: paginas.map((p) => ({
    PageNumber: p.pageNumber,
    RutaTemporalId: p.rutaTemporalId,
    ArchivoTemporalId: p.archivoTemporalId,
    ContentType: "application/pdf",
    HashSha256Esperado: p.hashSha256,
    SourcePageWidth: p.sourcePageWidth,
    SourcePageHeight: p.sourcePageHeight,
    SourcePageRotation: p.sourcePageRotation,
    SourcePageFingerprintSha256: p.sourcePageFingerprintSha256,
  })),
  Motivo: motivo,
  DescOp: descOp ?? "AGREGA GRAFO PDF",
  ModuloRegistro: moduloRegistro,
  Radicado: radicado,
  IdTareaWorkflow: idTareaWorkflow,
  IdRutaWorkflow: idRutaWorkflow,
  TipologiaDocumental: tipologiaDocumental,
}
```

Reglas:

- No enviar strings vacios.
- No enviar `IdTareaWorkflow` negativo.
- No enviar `IdRutaWorkflow` negativo.
- `DescOp` por defecto: `AGREGA GRAFO PDF`.
- `ModuloRegistro` debe venir del contexto consumidor.
- Para `DocumentosWorkbench` ejecutado desde flujo/tarea, enviar `WORKFLOW`.
- Usar `WORKFLOW` solo cuando el documento proviene efectivamente de flujo/tarea.
- Si no hay contexto workflow, omitir `ModuloRegistro` o usar `DOCUARCHI` segun politica funcional.
- Para otros modulos, enviar valor valido del origen funcional: `DOCUARCHI`, `PRODUCCION`, `WORKFLOW` u otro normalizado por backend.
- Si consumidor no puede resolverlo, omitirlo y dejar backend aplicar default.
- No inventar `GESTION_CORRESPONDENCIA` como modulo de auditoria.
- Enviar `RutaTemporalId` por pagina.
- El `RutaTemporalId` raiz es fallback compatible.
- Enviar `OriginalPdfPassword` solo si existe en memoria por apertura real del PDF protegido.
- No guardar ni loguear `OriginalPdfPassword`.
- Si se dispone de hash/version/geometria/fingerprint real, enviarlo.
- Propagar errores backend `AppResponses`.
- Si `errors[0].Field`/`field` es `originalPdfPassword`, mostrar flujo de contrasena invalida/requerida.

## 20. Integracion en AppVisorEmbedPdf

Extender props en:

`src/app/Components/UI/AppVisorEmbedPdf/types/AppVisorEmbedPdfProps.ts`

Props nuevas recomendadas:

```ts
export interface AppVisorEmbedPdfProps {
  fileUrl?: string;
  loading?: boolean;
  className?: string;
  style?: React.CSSProperties;
  onEmptyDocumentHintRequest?: () => void;

  onRequestSaveAnnotatedPages?: () => void;
  isSaveAnnotatedPagesDisabled?: boolean;
  isSavingAnnotatedPages?: boolean;
  saveAnnotatedPagesProgress?: number | null;
  onPermissionsResolved?: (permissions: ViewerEffectivePermissions) => void;
}
```

`AppVisorEmbedPdf` ya calcula `managedPermissionsEffective`.

Reglas:

- El visor debe poder bloquear visualmente la accion si no tiene permisos.
- El Workbench puede recibir permisos efectivos via `onPermissionsResolved` si necesita decidir estado de negocio.
- No duplicar mapping de permisos en `DocumentosWorkbench`.
- No consultar permisos desde `AppPdfToolbar`.

Dentro de `useImperativeHandle`, agregar `exportAnnotatedPdfPages`.

Reglas del metodo:

- Usar `annotation.state.pages`.
- Usar `annotationCap.provides.commit()`.
- Usar `exportApi.provides.saveAsCopy()` sin parametro.
- Extraer paginas con dependencia aprobada.
- Calcular hash por pagina si `crypto.subtle` existe.
- Retornar estructura neutral sin tipos EmbedPDF.

## 21. Integracion en AppPdfToolbar

Agregar accion presentacional:

```ts
onSaveAnnotatedPages?: () => void;
isSaveAnnotatedPagesDisabled?: boolean;
isSavingAnnotatedPages?: boolean;
saveAnnotatedPagesProgress?: number | null;
```

Reglas:

- Mostrar boton `Guardar anotaciones` o icono equivalente.
- No llamar HTTP.
- No acceder a `visorRef`.
- No importar services.
- No importar `@embedpdf/*`.
- Deshabilitar cuando `isSaveAnnotatedPagesDisabled` sea true.
- Mostrar estado/progreso si `isSavingAnnotatedPages` es true.

## 22. Integracion en DocumentosWorkbench

Agregar accion de negocio:

```ts
const handleSaveAnnotatedPages = useCallback(async () => {
  // export -> upload -> replace -> refresh
}, [...]);
```

Agregar estado:

```ts
const replaceSeqRef = useRef(0);
const replaceAbortRef = useRef<AbortController | null>(null);
const [isSavingAnnotatedPages, setIsSavingAnnotatedPages] = useState(false);
const [annotatedPagesProgress, setAnnotatedPagesProgress] = useState<number | null>(null);
const [viewerEffectivePermissions, setViewerEffectivePermissions] = useState<ViewerEffectivePermissions | null>(null);
```

Validaciones previas:

- existe `documentViewer.documentoActivo`.
- `viewerKind === "pdf"`.
- `isPdf === true`.
- `fileUrl` existe.
- `documentId > 0`.
- `nombreGabinete` no vacio.
- `visorRef.current?.exportAnnotatedPdfPages` existe.
- si `isElectronicallySigned === true`, bloquear.

Mensaje para firmado:

```text
No se permite reemplazar paginas de un documento firmado digitalmente.
```

El boton `Guardar anotaciones` debe deshabilitarse si:

- `!viewerEffectivePermissions?.allowAnnotationEdit`, o
- `!viewerEffectivePermissions?.allowExport`, o
- documento firmado electronicamente, o
- no hay documento activo, o
- no es PDF, o
- operacion en curso.

Si en una iteracion inicial no se puede exponer `viewerEffectivePermissions` al Workbench, documentar brecha y no habilitar el boton sin control. La preferencia enterprise es exponer permisos calculados desde el visor mediante callback.

Flujo obligatorio:

1. Abortar operacion anterior.
2. Incrementar secuencia latest-wins.
3. Crear `AbortController`.
4. Marcar estado `isSavingAnnotatedPages`.
5. Exportar paginas con `visorRef.current.exportAnnotatedPdfPages()`.
6. Si no hay anotaciones, mostrar `No hay anotaciones para guardar.` y no llamar backend.
7. Subir paginas con `uploadAnnotatedPdfPagesTemporal`.
8. Llamar `replaceAnnotatedPdfPages`.
9. Mostrar success `Paginas anotadas guardadas correctamente.`.
10. Mostrar mensaje funcional con paginas reemplazadas cuando backend retorne `PaginasReemplazadas`.
11. Conservar `RequestId` para soporte.
12. No mostrar rutas fisicas a usuarios finales si la politica de seguridad del producto lo restringe.
13. Refrescar documento con `documentViewer.visualizarDocumento`.
14. Mantener `activeRowId`.
15. Limpiar estado.

Ejemplo de replace:

```ts
await replaceAnnotatedPdfPages({
  nombreGabinete: doc.nombreGabinete,
  idDocumento: doc.documentId,
  rutaTemporalId: upload.rutaTemporalId,
  originalPdfPassword: documentViewer.originalPdfPassword,
  sourceDocumentHashSha256: documentViewer.sourceDocumentHashSha256,
  sourceDocumentVersion: documentViewer.sourceDocumentVersion,
  paginas: upload.pages.map((p) => ({
    pageNumber: p.pageNumber,
    rutaTemporalId: p.rutaTemporalId,
    archivoTemporalId: p.archivoTemporalId,
    hashSha256: p.hashSha256,
    ...sourcePageSnapshotByPageNumber[p.pageNumber],
  })),
  motivo: "Anotaciones agregadas desde visor PDF",
  descOp: "AGREGA GRAFO PDF",
  moduloRegistro: resolveModuloRegistroFromWorkbenchContext() ?? "WORKFLOW",
  radicado,
  idTareaWorkflow,
  idRutaWorkflow,
  tipologiaDocumental,
  signal: abortController.signal,
});
```

Importante:

`documentViewer.originalPdfPassword`, `sourceDocumentHashSha256`, `sourceDocumentVersion` y `sourcePageSnapshotByPageNumber` solo deben usarse si existen realmente en el proyecto. Si no existen, no inventarlos y documentar pendiente.

Latest-wins despues de cada `await`:

```ts
if (seq !== replaceSeqRef.current) return;
if (abortController.signal.aborted) return;
```

Al desmontar:

- abortar operacion en curso.
- cancelar temporales creados best-effort si hay tracking.

Si error es cancelacion:

- no mostrar toast error.
- no detener documento activo.

## 23. UX y mensajes

Estados visibles:

- preparando paginas.
- subiendo paginas.
- reemplazando documento.
- completado.
- error.

Mensajes:

- Sin anotaciones: `No hay anotaciones para guardar.`
- Firmado electronico: `No se permite reemplazar paginas de un documento firmado digitalmente.`
- Preparando: `Preparando paginas anotadas...`
- Subiendo: `Subiendo paginas anotadas...`
- Reemplazando: `Reemplazando paginas del documento...`
- Success: `Paginas anotadas guardadas correctamente.`
- Success con paginas: `Paginas {lista} actualizadas correctamente.`
- Error generico: `No fue posible guardar las paginas anotadas.`
- Anti-desfase: `El documento cambio desde que fue abierto. Recarga el PDF antes de guardar las anotaciones.`

Reglas:

- No ocultar documento visible si falla.
- No resetear seleccion si falla.
- No limpiar documento activo si falla.
- No mostrar error si fue cancelacion.
- Mostrar errores backend utiles cuando sean seguros.
- Guardar o exponer `RequestId` para soporte si backend lo retorna.
- No mostrar `RutaArchivoFinal` ni `RutaRespaldo` a usuarios finales si la politica de seguridad restringe rutas fisicas.

## 24. Manejo de documentos pesados

Obligatorio:

- usar `Blob`.
- usar `slice()`.
- no base64.
- subir secuencialmente paginas.
- liberar referencias al terminar.
- no crear copias innecesarias del PDF completo.
- mostrar progreso.
- permitir cancelacion.

Riesgo:

Para extraer paginas PDF, primero se usa `saveAsCopy()` y eso genera un PDF anotado completo en memoria.

Mitigacion:

- Documentar que EmbedPDF no expone hoy exportacion directa por pagina en este proyecto.
- Evitar rasterizacion.
- Evitar base64.
- Subir solo paginas extraidas al backend.

Si la memoria se vuelve problema:

- abrir ticket tecnico para soporte de exportacion por rango/pagina desde engine o backend.

## 25. Observabilidad FE

Usar patron local:

```ts
window.__DV_DEBUG__
```

Logs debug permitidos:

- `[DV][annotated-pages] start`
- `[DV][annotated-pages] pages detected`
- `[DV][annotated-pages] commit ok`
- `[DV][annotated-pages] full annotated pdf exported`
- `[DV][annotated-pages] single page pdf created`
- `[DV][replace-pages] upload start`
- `[DV][replace-pages] upload page ok`
- `[DV][replace-pages] replace start`
- `[DV][replace-pages] replace ok`
- `[DV][replace-pages] refresh start`
- `[DV][replace-pages] failed`
- `[DV][replace-pages] cancelled`

Campos permitidos:

- `attemptId`.
- `documentId`.
- `nombreGabinete`.
- `annotatedPages`.
- `pageNumber`.
- `blobSize`.
- `hashSha256`.
- `rutaTemporalId` solo si no contiene datos sensibles.
- `archivoTemporalId`.
- `percent`.
- `durationMs`.

Prohibido loguear:

- bytes del PDF.
- URLs temporales con token.
- Authorization.
- contenido del documento.
- rutas fisicas.
- `OriginalPdfPassword`.
- payload completo si contiene datos sensibles.

## 26. Validaciones FE

Antes de exportar:

- documento activo existe.
- PDF activo existe.
- no firmado electronicamente.
- ref del visor listo.
- permisos permiten anotacion y export.

Despues de exportar:

- `pages.length > 0` si `hasAnnotations === true`.
- cada `pageNumber > 0`.
- sin duplicados.
- cada blob `type === "application/pdf"`.
- cada blob `size > 0`.

Antes de replace:

- todos los uploads completados.
- cada pagina subida tiene `rutaTemporalId`.
- cada pagina subida tiene `archivoTemporalId`.
- cada temporal esta `COMPLETED` antes del replace final.
- no exigir que todas compartan `rutaTemporalId`.
- `nombreGabinete` no vacio.
- `idDocumento > 0`.
- si se envia `OriginalPdfPassword`, proviene solo de memoria.
- si se envia metadata anti-desfase, corresponde a la version renderizada actual.

## 27. Pruebas obligatorias

Unitarias `AppVisorEmbedPdf`:

- `exportAnnotatedPdfPages` retorna vacio sin anotaciones.
- convierte pageIndex base 0 a pageNumber base 1.
- deduplica y ordena paginas.
- ejecuta `commit()` antes de `saveAsCopy()`.
- usa `saveAsCopy()` sin parametro en el proyecto actual.
- llama extraccion de pagina por cada pagina anotada.
- retorna blobs `application/pdf`.
- no descarga archivos.
- falla controlado si `annotationCap.provides` no existe.
- falla controlado si `exportApi.provides` no existe.
- no expone tipos EmbedPDF en el resultado.

Unitarias utilidades:

- `getAnnotatedPageNumbers` con estado vacio.
- `getAnnotatedPageNumbers` con paginas duplicadas/no numericas.
- `sha256Hex` produce hash esperado para blob conocido.
- `sha256Hex` retorna `undefined` si `crypto.subtle` no existe.
- extractor de paginas valida rango.
- extractor de paginas genera `application/pdf`.

Unitarias servicio upload:

- init usa endpoint correcto.
- init envia extension `.pdf`.
- init envia extension `.PDF` segun contrato oficial o normaliza correctamente desde `.pdf`.
- init envia `NumeroChunks` calculado.
- init calcula `NumeroChunks` con default documentado `1048576` bytes salvo configuracion backend distinta.
- chunk usa `application/octet-stream`.
- chunk envia `X-Total-Chunks`.
- complete se llama despues del ultimo chunk.
- complete valida `Estado: "COMPLETED"`.
- status puede validar `Estado: "COMPLETED"` antes del replace final.
- progress se calcula correctamente.
- cancel se llama best-effort ante error.
- abort cancela sin ocultar error original.
- no usa base64.
- no exige `RutaTemporalId` comun.
- multipagina acepta rutas temporales diferentes.
- no invoca `DELETE` despues de success de `/paginas-anotadas`.

Unitarias servicio replace:

- POST usa `/paginas-anotadas`.
- body contiene `NombreGabinete`, `IdDocumento`, `RutaTemporalId`, `Paginas`.
- cada item de `Paginas` contiene su propio `RutaTemporalId`.
- cada item de `Paginas` contiene `ArchivoTemporalId`.
- `Paginas.ContentType` es `application/pdf`.
- `DescOp` default es `AGREGA GRAFO PDF`.
- `ModuloRegistro` se resuelve desde contexto consumidor.
- `DocumentosWorkbench` en contexto workflow envia `WORKFLOW`.
- otro consumidor puede enviar otro modulo valido sin cambiar visor.
- no envia IDs workflow negativos.
- envia `OriginalPdfPassword` solo cuando existe en memoria.
- no persiste ni loguea `OriginalPdfPassword`.
- envia metadata anti-desfase cuando esta disponible.
- trata `SourcePageWidth`, `SourcePageHeight`, `SourcePageRotation` y `SourcePageFingerprintSha256` como validacion anti-desfase, no como definicion de tamano final.
- maneja error `originalPdfPassword`.
- maneja error anti-desfase solicitando recarga.
- adapta respuesta PascalCase/camelCase si aplica.
- desempaqueta `AppResponses<T>`.
- lanza error si `success !== true`.
- lanza error contractual si `data` requerido viene null.

Unitarias `DocumentosWorkbench`:

- boton deshabilitado sin documento activo.
- boton deshabilitado si no es PDF.
- boton deshabilitado sin permisos.
- bloquea firmado electronicamente.
- sin anotaciones muestra toast y no llama backend.
- con anotaciones ejecuta export -> upload -> replace -> refresh.
- error de upload conserva documento visible.
- error de replace conserva documento visible.
- cancelacion no muestra error.
- success reabre documento.
- mantiene `activeRowId`.
- no rompe `viewerKind=image`.

Integracion FE:

- flujo completo con dos paginas anotadas.
- PDF pesado simulado con varios chunks.
- falla init.
- falla chunk intermedio.
- falla complete.
- falla replace final.
- reemplazo multipagina con `RutaTemporalId` diferente por pagina.
- PDF original protegido con contrasena valida e invalida, solo si existe flujo real.
- rechazo anti-desfase por hash o geometria, solo si existe metadata real.
- retry posterior funciona.

Calidad / restricciones verificables:

- no imports `@embedpdf/*` en `DocumentosWorkbench`.
- no `axios` directo fuera del service nuevo.
- no base64.
- no `pdfjs-dist` para este flujo.
- no canvas/rasterizacion.
- no rutas fisicas.
- no password PDF en storage/logs/telemetria.
- no se rompe `viewerKind=image`.
- no se rompe print/export actual.

Regresion:

- export/download actual sigue funcionando.
- print sigue funcionando.
- firma sigue funcionando.
- eliminar firma sigue funcionando.
- bloqueo/desbloqueo de firma sigue funcionando.
- carga de documentos sigue latest-wins.
- `cancelCurrentLoad` sigue operativo.
- permisos SCRUMCORE-236 siguen aplicando.

## 28. Documentacion obligatoria

Ruta:

`docs/Architecture/implementacion-de-AppVisorPdf/`

Crear o actualizar:

- `SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas-Metadata.md`
- `SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas-Arquitectura.md`
- `SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas-Contrato-API.md`
- `SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas-Implementacion.md`
- `SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas-Pruebas.md`
- `SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas-Observabilidad.md`
- `SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas-Seguridad.md`
- `SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas-Runbook.md`
- `PROMPT-SCRUM-249-FE-Reemplazo-Paginas-PDF-Anotadas.md`

Debe incluir:

- flujo export paginas PDF.
- por que no se envian imagenes.
- contrato `/paginas-anotadas`.
- upload temporal por pagina.
- cancelacion.
- latest-wins.
- permisos.
- integracion managed `load()` actual.
- decision sobre `pdf-lib` o alternativa.
- pruebas.
- riesgos.
- rollback.
- QA manual.

## 29. Criterios de aceptacion

- El visor detecta paginas anotadas.
- El visor exporta PDFs de una sola pagina para cada pagina anotada.
- No se generan imagenes.
- No se usa rasterizacion.
- No se envia PDF completo al endpoint parcial.
- Cada pagina se sube como `application/pdf`.
- Se usa upload temporal existente.
- Cada upload temporal se completa y queda `COMPLETED` antes del reemplazo final.
- Se llama `/paginas-anotadas` con `RutaTemporalId` por pagina y lista de paginas.
- El `RutaTemporalId` raiz se usa solo como fallback compatible.
- Se maneja `OriginalPdfPassword` solo en memoria cuando el PDF original protegido lo requiere y existe fuente real.
- Se envia validacion anti-desfase cuando el FE tiene hash/version/geometria/fingerprint real.
- No se usa metadata anti-desfase para definir tamano final; el backend conserva caja/orientacion original con iText/iText7.
- Se bloquea documento firmado electronicamente.
- Se maneja progreso.
- Se maneja cancelacion.
- Se maneja latest-wins.
- Si no hay anotaciones, no se llama backend.
- Si falla, el documento visible se conserva.
- En success se refresca el documento.
- No se filtra EmbedPDF a `DocumentosWorkbench`.
- No hay HTTP en toolbar.
- No hay base64.
- No hay canvas.
- No se rompe `viewerKind=image`.
- No se rompe print/export/firma.
- Pruebas completas.
- Documentacion completa.

## 30. Restricciones

No enviar imagenes.

No reemplazar imagenes internas del PDF.

No rasterizar.

No usar canvas como contrato.

No usar `pdfjs-dist` para este flujo.

No base64.

No rutas fisicas desde frontend.

No llamadas API desde `AppPdfToolbar`.

No imports `@embedpdf/*` fuera del visor.

No romper reemplazo total existente.

No romper export actual.

No romper print.

No romper firma.

No romper visualizacion de imagenes.

No ignorar documento firmado electronicamente.

No forzar `RutaTemporalId` comun.

No invocar `DELETE` despues de success de `/paginas-anotadas`.

No persistir `OriginalPdfPassword`.

No loguear `OriginalPdfPassword`.

No usar capturas ni canvas como validacion anti-desfase.

No inventar metadatos anti-desfase.

No usar `saveAsCopy(documentId)` en el proyecto actual salvo validacion tecnica nueva.

## 31. Riesgos y mitigaciones

Riesgo: ruta temporal comun artificial.

Mitigacion: usar contrato real SCRUM-249. Cada `init` crea ruta temporal por archivo y cada item de `Paginas` envia su propio `RutaTemporalId`. Campo raiz solo fallback compatible.

Riesgo: PDF original cambia entre visualizacion y guardado.

Mitigacion: enviar `SourceDocumentHashSha256` y metadata/fingerprint por pagina solo cuando esten disponibles. Ante rechazo backend, recargar PDF antes de reintentar.

Riesgo: PDF original protegido por contrasena.

Mitigacion: mantener `OriginalPdfPassword` solo en memoria y enviarlo unicamente cuando el visor lo requirio. Si backend rechaza `originalPdfPassword`, pedir contrasena de nuevo o abortar sin modificar documento visible.

Riesgo: EmbedPDF solo exporta PDF completo.

Mitigacion: exportar completo una vez, extraer paginas PDF sin rasterizar y subir solo paginas. Documentar limitacion y abrir ticket si se requiere exportacion por rango/pagina.

Riesgo: dependencia frontend para manipular PDF no existe.

Mitigacion: evaluar `pdf-lib`. Documentar licencia, peso e impacto. Sin aprobacion, bloquear implementacion de extraccion y no sustituir por imagenes.

Riesgo: documento firmado electronicamente.

Mitigacion: bloquear en FE y mantener validacion backend como fuente final.

Riesgo: operacion larga.

Mitigacion: progreso, cancelacion, latest-wins, subida secuencial y no ocultar documento visible.

Riesgo: permisos no propagados al Workbench.

Mitigacion: exponer permisos efectivos desde visor con `onPermissionsResolved` o mantener accion deshabilitada hasta resolver permisos. No duplicar reglas en Workbench.

## 32. Recomendacion de implementacion por fases

Aunque el objetivo final es end-to-end, para reducir riesgo enterprise se recomienda dividir en fases o sub-SCRUMs.

Fase 1 - API imperativa y deteccion:

- Agregar tipos `AppVisorAnnotatedPdfPage`.
- Agregar `exportAnnotatedPdfPages` al ref.
- Implementar `getAnnotatedPageNumbers`.
- Probar deteccion, dedupe, orden y resultado vacio.

Fase 2 - Export y extraccion PDF:

- Ejecutar `commit()`.
- Ejecutar `saveAsCopy()`.
- Aprobar/agregar `pdf-lib` o alternativa.
- Extraer PDF de una pagina por anotacion.
- Calcular hash SHA-256.
- Probar blobs `application/pdf`.

Fase 3 - Servicios HTTP:

- Crear tipos y service.
- Implementar upload chunked por pagina.
- Implementar status upload temporal cuando se requiera confirmar `COMPLETED`.
- Implementar cancel best-effort.
- Implementar replace `/paginas-anotadas`.
- Probar `AppResponses<T>`, rutas por pagina y errores.

Fase 4 - Integracion Workbench/Toolbar:

- Agregar boton presentacional.
- Orquestar export -> upload -> replace -> refresh.
- Integrar permisos.
- Integrar latest-wins/cancelacion.
- Probar UI y errores.

Fase 5 - Hardening:

- Password PDF si hay fuente real.
- Anti-desfase si hay metadata real.
- Observabilidad.
- QA manual.
- Documentacion final.

## 33. Comandos de validacion sugeridos

Ajustar rutas segun archivos finales:

```powershell
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service.test.ts
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/utils/pdfPageAnnotations.test.ts
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/utils/pdfSinglePageExtraction.test.ts
npx.cmd vitest run src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.test.tsx
npx.cmd eslint src/app/Components/UI/AppVisorEmbedPdf src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx
```

Si se agrega dependencia nueva:

```powershell
npm.cmd install pdf-lib
npm.cmd run build
```

Agregar dependencia requiere aprobacion tecnica/licencia y debe quedar documentado.

## 34. QA manual esperado

1. Activar debug:

```js
window.__DV_DEBUG__ = true
```

2. Abrir documento PDF desde Gestion Correspondencia.
3. Confirmar que el visor carga por modo managed `load()`.
4. Crear anotaciones en una pagina.
5. Guardar anotaciones.
6. Confirmar deteccion de pagina anotada.
7. Confirmar commit.
8. Confirmar export de PDF anotado completo.
9. Confirmar extraccion de PDF de una pagina.
10. Confirmar upload como `application/pdf`.
11. Confirmar `complete` con `Estado: "COMPLETED"` o status `COMPLETED`.
12. Confirmar llamada a `/paginas-anotadas`.
13. Confirmar `Paginas[0].RutaTemporalId`.
14. Confirmar `Paginas[0].ArchivoTemporalId`.
15. Confirmar `Paginas[0].ContentType === "application/pdf"`.
16. Confirmar `RequestId`.
17. Confirmar paginas reemplazadas en UI o evidencia de soporte.
18. Confirmar success y refresh.
19. Confirmar que no se invoca `DELETE` despues de success de `/paginas-anotadas`.
20. Repetir con dos paginas anotadas y validar rutas temporales distintas si backend las retorna.
21. Probar sin anotaciones.
22. Probar documento firmado electronicamente.
23. Probar fallo de upload.
24. Probar fallo de replace.
25. Confirmar que imagenes siguen visualizando sin romper.
26. Confirmar print/export/firma existentes.

## 35. Instruccion final

Implementar en React el flujo enterprise para guardar anotaciones reemplazando unicamente paginas PDF completas, alineado con la API backend `/paginas-anotadas`.

La solucion debe:

- usar `AppVisorEmbedPdf` como unica frontera con EmbedPDF.
- usar `DocumentosWorkbench` como orquestador de negocio.
- usar `AppPdfToolbar` solo como presentacional.
- usar `clienteApi` en services.
- exportar paginas PDF anotadas, no imagenes.
- subir PDFs de una pagina por upload temporal chunked.
- llamar al endpoint parcial con contrato exacto.
- enviar `RutaTemporalId` por cada pagina, no una ruta comun artificial.
- usar `RutaTemporalId` raiz solo como fallback compatible.
- manejar `OriginalPdfPassword` solo si existe fuente real y solo en memoria.
- manejar validacion anti-desfase solo si existe metadata real.
- bloquear documentos firmados electronicamente.
- manejar progreso.
- manejar cancelacion.
- manejar latest-wins.
- manejar errores.
- refrescar documento en success.
- mantener pruebas completas.
- mantener documentacion completa.

No dejar brechas de integracion con la API.

No implementar atajos con imagenes, canvas, base64 o rasterizacion.
