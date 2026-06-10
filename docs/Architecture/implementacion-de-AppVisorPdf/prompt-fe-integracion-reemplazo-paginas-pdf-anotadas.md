# PROMPT ARQUITECTÓNICO — Ticket FE

SCRUM-249 — Integración React para Reemplazo de Páginas PDF Anotadas desde AppVisorEmbedPdf  
(ENTERPRISE FINAL — AppVisorEmbedPdf + Exportación por Página + Upload Temporal Chunks + API real `/paginas-anotadas`)

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ROL ESPERADO

Actúa como Arquitecto de Software Senior Frontend React + TypeScript especialista en:

- React.
- TypeScript estricto.
- EmbedPDF / Pdfium Engine.
- `@embedpdf/plugin-annotation`.
- `@embedpdf/plugin-export`.
- encapsulación de visores PDF.
- servicios API con Axios.
- upload temporal por chunks.
- manejo de archivos grandes.
- cancelación con `AbortController`.
- patrón latest-wins.
- integración con backend ASP.NET Core.
- pruebas unitarias e integración FE.
- observabilidad controlada.
- documentación técnica enterprise.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CONTEXTO DEL PROYECTO

Repositorio FE:

Este repositorio React. Usar rutas relativas al workspace.

Visor PDF:

`src/app/Components/UI/AppVisorEmbedPdf`

Componente consumidor:

`src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`

Fuente de verdad para esta implementación FE:

- Este prompt es autocontenido para el frontend.
- No depender de rutas locales del repositorio backend.
- El contrato HTTP que debe implementar React está descrito en las secciones `SERVICIO HTTP — TIPOS`, `UPLOAD TEMPORAL`, `REEMPLAZO PÁGINAS PDF ANOTADAS` y `FLUJO DE ORQUESTACIÓN`.
- Si hay duda de contrato, pedir confirmación al owner backend o consultar Swagger/OpenAPI publicado para el ambiente objetivo.
- No importar código, DTOs ni tipos desde backend.

Referencias backend opcionales para trazabilidad:

- API: `https://github.com/miguelurueta/DocuArchi.Api.git`
- DTOs: `https://github.com/miguelurueta/MiApp.DTOs.git`
- Services: `https://github.com/miguelurueta/MiApp.Services.git`
- Repository: `https://github.com/miguelurueta/MiApp.Repository.git`
- Documentación/core: `https://github.com/miguelurueta/DocuArchiCore.git`

Reglas para estas referencias:

- No clonar ni leer estos repos como requisito normal de implementación FE.
- Usarlos solo para auditoría de contrato, revisión técnica o resolución de dudas.
- Si Swagger/OpenAPI publicado contradice estos repos o este prompt, escalar la discrepancia antes de implementar.

Endpoint objetivo:

`POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`

Endpoints backend existentes a reutilizar:

- `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`
- `PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- `GET /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status`
- `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
- `DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`

Ya existe en `AppVisorEmbedPdf`:

- `AnnotationLayer`.
- `useAnnotation(documentId)`.
- `useAnnotationCapability()`.
- `useExport(documentId)`.
- `annotationCap.provides.commit()`.
- `exportApi.provides.saveAsCopy(documentId)`.
- `annotation.state.pages`.
- toolbar presentacional `AppPdfToolbar`.
- API imperativa actual `load/reset/cancelCurrentLoad`.
- permisos efectivos `ViewerEffectivePermissions`.
- flujo de firma/anotaciones como `STAMP`/`INK`.

Ya existe en `DocumentosWorkbench`:

- `visorRef`.
- `documentViewer.documentoActivo`.
- `activeFileUrl`.
- `activeRowId`.
- `idTareaWf`.
- `documentosTable.getWorkbenchContext?.()`.
- flujo `visualizarDocumento`.
- control de loading/toast/error.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CORRECCIÓN FUNCIONAL CRÍTICA

La implementación FE **NO debe enviar imágenes**.

La implementación FE **NO debe rasterizar páginas a PNG/JPEG**.

La implementación FE debe enviar al backend **páginas PDF anotadas**, donde cada archivo temporal es un PDF de una sola página.

Definición exacta:

- Detectar páginas con anotaciones en el visor.
- Materializar anotaciones con `commit()`.
- Exportar o construir PDFs de una sola página para cada página anotada.
- Subir cada PDF de una sola página al upload temporal existente.
- Enviar al endpoint `/paginas-anotadas` la lista `{ pageNumber, rutaTemporalId, archivoTemporalId, contentType: "application/pdf", hashSha256Esperado }`.
- El backend reemplaza páginas PDF completas, no imágenes internas.

Corrección de contrato SCRUM-249:

- El upload temporal actual crea un `RutaTemporalId` por cada `init`.
- Para reemplazo multipágina, **cada item de `Paginas` debe enviar su propio `RutaTemporalId`**.
- `RutaTemporalId` raíz existe solo como fallback compatible para clientes de una sola ruta.
- No forzar una ruta temporal común en frontend.
- El backend retorna y espera `AppResponses<T>`; el servicio FE debe desempaquetar `data` y propagar `errors`.
- El endpoint acepta `OriginalPdfPassword` opcional para PDF original protegido; el FE solo puede conservarlo en memoria.
- El endpoint acepta validación anti-desfase opcional con `SourceDocumentHashSha256`, `SourceDocumentVersion`, `SourcePageWidth`, `SourcePageHeight`, `SourcePageRotation` y `SourcePageFingerprintSha256`.

Actualizaciones necesarias para alinear React con la API:

- Cambiar tipos FE para incluir `AppResponses<T>` y desempaquetar `data`.
- Cambiar `UploadPageResult` y `Paginas[]` para transportar `RutaTemporalId` por página.
- Eliminar validaciones FE que exijan que todos los uploads compartan el mismo `rutaTemporalId`.
- Mantener `RutaTemporalId` raíz solo como fallback compatible, usando preferiblemente el de la primera página.
- Agregar campos opcionales de seguridad: `OriginalPdfPassword`, `SourceDocumentHashSha256` y `SourceDocumentVersion`.
- Agregar metadata anti-desfase por página: `SourcePageWidth`, `SourcePageHeight`, `SourcePageRotation`, `SourcePageFingerprintSha256`.
- Resolver `ModuloRegistro` desde el contexto consumidor del visor; para `DocumentosWorkbench` en flujo de tareas, usar `WORKFLOW`.
- Manejar errores `AppResponses.errors[]`, especialmente `originalPdfPassword` y campos anti-desfase.
- Ajustar pruebas para multipágina con rutas temporales diferentes.
- Prohibir persistencia/logging de `OriginalPdfPassword`.

Prohibido:

- enviar `image/png`.
- enviar `image/jpeg`.
- usar canvas como contrato de integración.
- usar `pdfjs-dist` para rasterizar.
- enviar el PDF completo si el endpoint parcial está disponible.
- exponer `@embedpdf/*` fuera del visor.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## OBJETIVO

Implementar en este repositorio React un flujo end-to-end para:

- detectar páginas PDF con anotaciones.
- exportar páginas PDF anotadas de una sola página.
- subir esas páginas por chunks usando el upload temporal existente.
- llamar a la nueva API de reemplazo de páginas PDF anotadas.
- manejar progreso, errores, cancelación y latest-wins.
- refrescar el documento visible tras reemplazo exitoso.
- bloquear reemplazo si el documento está firmado electrónicamente.
- no romper exportación/impresión/firma existentes.
- no romper visualización de imágenes.
- dejar pruebas y documentación.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DECISIÓN ARQUITECTÓNICA PRINCIPAL

`AppVisorEmbedPdf` debe encapsular toda la lógica relacionada con EmbedPDF:

- conocer `documentId`.
- conocer `annotation.state.pages`.
- ejecutar `commit()`.
- exportar páginas PDF anotadas.

`DocumentosWorkbench` debe orquestar negocio:

- tomar contexto documental.
- llamar al método imperativo del visor.
- subir temporales.
- llamar API `/paginas-anotadas`.
- mostrar UX/progreso.
- reabrir/refrescar el documento.

El servicio API debe encapsular HTTP:

- init upload.
- chunk upload.
- complete.
- cancel best-effort.
- replace pages.

`AppPdfToolbar` debe seguir siendo presentacional:

- recibe callbacks.
- recibe flags.
- no conoce HTTP.
- no conoce EmbedPDF.
- no conoce `clienteApi`.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ARCHIVOS ESPERADOS

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

Opcional si se requiere separar utilidades puras:

- `src/app/Components/UI/AppVisorEmbedPdf/utils/pdfPageAnnotations.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/utils/pdfPageAnnotations.test.ts`

No crear servicios HTTP dentro de `gestionCorrespondencia` salvo que se justifique por dependencia fuerte de workflow. Preferir servicio neutral del visor para reutilización por otros módulos.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## API IMPERATIVA DEL VISOR

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

- `annotatedPages` usa numeración humana base 1.
- `annotation.state.pages` usa pageIndex base 0.
- Si no hay anotaciones, retornar `hasAnnotations: false`, `annotatedPages: []`, `pages: []`.
- No descargar archivos.
- No subir a backend.
- No exponer `documentId`.
- No exponer tipos de EmbedPDF.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DETECCIÓN DE PÁGINAS ANOTADAS

Usar exclusivamente:

`annotation.state.pages`

Regla:

```ts
function getAnnotatedPageNumbers(pagesState: Record<string, unknown>): number[] {
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

No usar:

- DOM.
- canvas.
- thumbnails.
- selección de texto.
- inferencias visuales.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## EXPORTACIÓN DE PÁGINAS PDF ANOTADAS

Objetivo:

Obtener un PDF de una sola página por cada página anotada.

Flujo obligatorio dentro del visor:

1. Validar que hay documento activo.
2. Calcular `annotatedPages`.
3. Si no hay anotaciones, retornar resultado vacío.
4. Ejecutar:

```ts
await waitPdfTask<void>(annotationCap.provides.commit());
```

5. Obtener PDF anotado completo con:

```ts
const buffer = await waitPdfTask<ArrayBuffer | Uint8Array>(
  exportApi.provides.saveAsCopy(documentId) as any,
);
```

6. A partir del PDF anotado completo, extraer un PDF de una sola página por cada `pageNumber`.

Implementación de extracción:

- Primero inspeccionar dependencias existentes.
- Si existe librería frontend segura para manipular PDF por páginas, reutilizarla.
- Si no existe, evaluar agregar `pdf-lib` solo si el equipo aprueba dependencia/licencia.
- No usar `pdfjs-dist` para rasterizar.
- No convertir a imagen.

Requisito técnico de la librería:

- cargar PDF desde `ArrayBuffer`.
- copiar una página concreta a un documento nuevo.
- guardar PDF resultante como bytes.

Pseudocódigo con librería tipo `pdf-lib`:

```ts
const source = await PDFDocument.load(pdfBytes);
for (const pageNumber of annotatedPages) {
  const single = await PDFDocument.create();
  const [copied] = await single.copyPages(source, [pageNumber - 1]);
  single.addPage(copied);
  const singleBytes = await single.save();
  const blob = new Blob([singleBytes], { type: "application/pdf" });
}
```

Validaciones:

- `pageNumber - 1` debe estar dentro del rango del PDF exportado.
- cada blob generado debe tener `type = "application/pdf"`.
- cada archivo debe tener una página, por construcción.
- si falla una página, fallar toda la operación con error controlado.

Nombres sugeridos:

`annotated-page-${documentId}-${pageNumber}.pdf`

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## HASH SHA-256 EN FRONTEND

Calcular hash por cada PDF de página usando WebCrypto:

```ts
async function sha256Hex(blob: Blob): Promise<string> {
  const buffer = await blob.arrayBuffer();
  const digest = await crypto.subtle.digest("SHA-256", buffer);
  return Array.from(new Uint8Array(digest))
    .map((b) => b.toString(16).padStart(2, "0"))
    .join("");
}
```

Reglas:

- Si `crypto.subtle` no está disponible, permitir envío sin hash si backend lo trata como opcional.
- No bloquear navegadores por falta de WebCrypto, pero registrar debug.
- No convertir a base64.
- No guardar hash en localStorage.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## VALIDACIÓN ANTI-DESFASE Y PDF PROTEGIDO

El backend SCRUM-249 permite validar que el PDF anotado corresponde a la misma versión que el usuario visualizó antes del reemplazo.

Campos opcionales a capturar mientras el visor carga/renderiza el documento:

```ts
export type AppVisorSourceDocumentSnapshot = {
  sourceDocumentHashSha256?: string;
  sourceDocumentVersion?: string;
  pages?: SourcePageSnapshot[];
};
```

Reglas:

- Si el FE puede calcular `sourceDocumentHashSha256` del PDF original renderizado, enviarlo en el request final.
- Si el visor expone geometría de página, conservar por página `sourcePageWidth`, `sourcePageHeight` y `sourcePageRotation`.
- Si se calcula fingerprint de página, usar SHA-256 sobre una cadena estable equivalente a `width|height|rotation`, normalizada igual al backend.
- No usar capturas, canvas ni comparación pixel a pixel como contrato anti-desfase.
- Si backend responde validación por `sourceDocumentHashSha256`, `sourcePageWidth`, `sourcePageHeight`, `sourcePageRotation` o `sourcePageFingerprintSha256`, mostrar mensaje de recarga: `El documento cambió desde que fue abierto. Recarga el PDF antes de guardar las anotaciones.`

PDF original protegido:

- `OriginalPdfPassword` aplica solo al PDF original almacenado, no a los temporales anotados.
- Enviarlo únicamente si el usuario ya lo ingresó para abrir/anotar el PDF.
- Mantenerlo solo en memoria durante la sesión de visualización.
- No persistirlo en storage del navegador.
- No loguearlo ni enviarlo a telemetría.
- Si backend responde validación en `originalPdfPassword`, solicitar de nuevo la contraseña o abortar el guardado sin limpiar el documento visible.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## SERVICIO HTTP — TIPOS

Crear:

`src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.types.ts`

```ts
export type UploadTemporalInitRequest = {
  NombreOriginal: string;
  TamanoBytes: number;
  Extension: ".pdf";
  HashSha256Esperado?: string;
  NumeroChunks: number;
};

export type UploadTemporalInitResponse = {
  RutaTemporalId: string;
  ArchivoTemporalId: string;
  ChunkSizeBytes: number;
  Estado: string;
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

- Consumir siempre el wrapper `AppResponses<T>`.
- Si `success !== true`, lanzar error de dominio usando `message` y el primer `errors[]`.
- Si `data` es `null` en respuesta exitosa de un endpoint que requiere datos, tratarlo como error contractual.
- Crear adaptadores tolerantes que lean PascalCase y camelCase porque los ejemplos de documentación muestran ambos estilos.
- Para reemplazo multipágina, `ReemplazarPaginaPdfAnotadaTemporalDto.RutaTemporalId` es obligatorio en cada item.
- `ReemplazarPaginasPdfAnotadasRequest.RutaTemporalId` puede enviarse como fallback con el primer `RutaTemporalId`, pero no debe sustituir el `RutaTemporalId` por página.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## SERVICIO HTTP — UPLOAD TEMPORAL POR PÁGINA

Crear:

`src/app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service.ts`

Función:

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

1. Calcular `NumeroChunks` con chunk preliminar igual al valor backend por defecto actual: 10 MB (`10485760`).
2. Llamar:

`POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`

Body:

```json
{
  "NombreOriginal": "annotated-page-doc-2.pdf",
  "TamanoBytes": 12345,
  "Extension": ".pdf",
  "HashSha256Esperado": "...",
  "NumeroChunks": 1
}
```

3. Leer `ChunkSizeBytes` real.
4. Recalcular `totalChunks` con `ChunkSizeBytes`.
5. Si el total recalculado difiere del enviado en init:
   - abortar con error claro antes de subir chunks.
   - motivo: `StorageUploadPolicy` valida que `X-Total-Chunks` coincida con `NumeroChunks` persistido en metadata durante `init`.
   - si backend en runtime retorna otro tamaño, abortar con error claro para evitar inconsistencias de `X-Total-Chunks`.
6. Subir chunks:

`PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`

Headers:

```ts
{
  "Content-Type": "application/octet-stream",
  "X-Total-Chunks": String(totalChunks)
}
```

Nota:

- El backend exige `Content-Length`.
- En navegador no fijar manualmente `Content-Length`; `fetch`/XHR lo calcula desde `blob.slice(start, end)`.
- Si se ejecutan pruebas fuera del navegador, asegurar que el cliente envíe `Content-Length`.

Body:

`blob.slice(start, end)`

7. Completar:

`POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`

8. Retornar:

```ts
{
  pageNumber,
  rutaTemporalId,
  archivoTemporalId,
  hashSha256,
  sizeBytes: blob.size
}
```

Regla importante:

La API backend real permite `RutaTemporalId` por página. El endpoint `init` crea un `RutaTemporalId` nuevo por archivo, por lo que el FE debe conservar el par `{ rutaTemporalId, archivoTemporalId }` retornado para cada PDF anotado de una sola página.

No exigir ni simular un `RutaTemporalId` común para todas las páginas. En el request final, enviar `RutaTemporalId` dentro de cada item de `Paginas`. El campo raíz `RutaTemporalId` solo se conserva como fallback compatible y puede tomar el `rutaTemporalId` de la primera página subida.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## SERVICIO HTTP — SUBIDA DE TODAS LAS PÁGINAS

Función:

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

- Subir secuencialmente para reducir presión de red/memoria.
- Si se paraleliza en el futuro, máximo configurable y documentado.
- No validar que todas las páginas retornan el mismo `rutaTemporalId`; el contrato real espera rutas por página.
- Retornar `rutaTemporalId` raíz opcional con el primer `rutaTemporalId` solo como fallback compatible para el request final.
- Validar que cada página tiene su propio `rutaTemporalId` y `archivoTemporalId`.
- Si falla una página después de init, cancelar todos los temporales creados best-effort.
- No ocultar el error original.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## SERVICIO HTTP — CANCELACIÓN TEMPORAL

Función:

```ts
export async function cancelUploadTemporal(params: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  signal?: AbortSignal;
}): Promise<void>;
```

Endpoint:

`DELETE /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}`

Uso:

- best-effort si falla chunk.
- best-effort si falla complete.
- best-effort si falla replace después de subir páginas.
- best-effort al abortar operación.

No mostrar error de cancelación al usuario si ya existe error principal.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## SERVICIO HTTP — REEMPLAZO PÁGINAS PDF ANOTADAS

Función:

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

`POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`

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

- No enviar strings vacíos.
- No enviar `IdTareaWorkflow` negativo.
- No enviar `IdRutaWorkflow` negativo.
- `DescOp` por defecto: `AGREGA GRAFO PDF`.
- `ModuloRegistro` debe venir del contexto consumidor.
- Para `DocumentosWorkbench` ejecutado desde flujo/tarea, enviar `WORKFLOW`.
- Para otros módulos, enviar el valor válido que corresponda al origen funcional (`DOCUARCHI`, `PRODUCCION`, `WORKFLOW` u otro normalizado por backend).
- Si el consumidor no puede resolverlo, omitirlo y dejar que backend aplique su default; no inventar `GESTION_CORRESPONDENCIA`.
- No inventar `DescOp`.
- Enviar `RutaTemporalId` por página; el `RutaTemporalId` raíz es fallback compatible.
- Enviar `OriginalPdfPassword` solo si el documento original protegido ya requirió contraseña en el visor.
- No guardar `OriginalPdfPassword` en `localStorage`, `sessionStorage`, logs ni telemetría.
- Si se dispone del hash del PDF renderizado o metadata de páginas, enviar validación anti-desfase.
- Propagar errores backend `AppResponses`; si `errors[0].Field`/`field` es `originalPdfPassword`, mostrar flujo de contraseña inválida o requerida.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## INTEGRACIÓN EN DOCUMENTOSWORKBENCH

Agregar acción de negocio:

`Guardar anotaciones`

Ubicación recomendada:

- Dentro de la toolbar del visor si se extiende `AppPdfToolbar`.
- O como botón externo sobre el viewer si se quiere evitar acoplar toolbar a reemplazo backend.

Recomendación de menor brecha:

Extender toolbar con callback presentacional y pasarlo desde `AppVisorEmbedPdf`, pero la orquestación real debe vivir en `DocumentosWorkbench`.

Para evitar que el visor conozca backend, usar una prop:

```ts
onRequestSaveAnnotatedPages?: () => void;
isSaveAnnotatedPagesDisabled?: boolean;
isSavingAnnotatedPages?: boolean;
saveAnnotatedPagesProgress?: number | null;
```

`AppVisorEmbedPdf` solo reenvía props a `AppPdfToolbar`.

`DocumentosWorkbench` implementa:

```ts
const handleSaveAnnotatedPages = useCallback(async () => {
  // export -> upload -> replace -> refresh
}, [...]);
```

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## FLUJO DE ORQUESTACIÓN EN DOCUMENTOSWORKBENCH

1. Validar documento activo:
   - existe `documentViewer.documentoActivo`.
   - `viewerKind === "pdf"`.
   - `isPdf === true`.
   - `fileUrl` existe.
   - `documentId > 0`.
   - `nombreGabinete` no vacío.

2. Validar firma electrónica:
   - si `isElectronicallySigned === true`, bloquear con mensaje:
     `No se permite reemplazar páginas de un documento firmado digitalmente.`

3. Validar ref:
   - `visorRef.current?.exportAnnotatedPdfPages` existe.

4. Iniciar latest-wins:
   - abortar operación anterior.
   - incrementar `replaceSeqRef`.
   - crear `AbortController`.

5. Exportar páginas:

```ts
const exportResult = await visorRef.current.exportAnnotatedPdfPages();
```

6. Si `!exportResult.hasAnnotations`:
   - toast info `No hay anotaciones para guardar.`
   - no llamar backend.

7. Subir páginas:

```ts
const upload = await uploadAnnotatedPdfPagesTemporal({
  pages: exportResult.pages,
  signal,
  onProgress,
});
```

8. Llamar API:

```ts
await replaceAnnotatedPdfPages({
  nombreGabinete: doc.nombreGabinete,
  idDocumento: doc.documentId,
  rutaTemporalId: upload.rutaTemporalId, // fallback compatible; cada pagina lleva su propia ruta
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
  signal,
});
```

Regla de contexto:

- `WORKFLOW` aplica para `DocumentosWorkbench` cuando el documento proviene de una tarea/ruta de workflow.
- Si el mismo visor se implementa en otro módulo, el contenedor debe pasar su propio `moduloRegistro` válido.
- `AppVisorEmbedPdf` y el servicio HTTP no deben hardcodear un módulo de auditoría global.

9. Mostrar success:

`Páginas anotadas guardadas correctamente.`

10. Refrescar documento:

- llamar `documentViewer.visualizarDocumento` con el mismo `documentId`, `nombreGabinete`, nuevo `attemptId` y mismo `documentKey`.
- mantener `activeRowId`.
- activar loading del visor mientras se descarga el nuevo blob.

11. Limpiar estado:

- `isSavingAnnotatedPages = false`.
- `progress = null`.
- abort ref null.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## ESTADO UI EN DOCUMENTOSWORKBENCH

Agregar:

```ts
const replaceSeqRef = useRef(0);
const replaceAbortRef = useRef<AbortController | null>(null);
const [isSavingAnnotatedPages, setIsSavingAnnotatedPages] = useState(false);
const [annotatedPagesProgress, setAnnotatedPagesProgress] = useState<number | null>(null);
```

Estados visibles:

- preparando páginas.
- subiendo páginas.
- reemplazando documento.
- completado.
- error.

Mensajes:

- Sin anotaciones: `No hay anotaciones para guardar.`
- Firmado electrónico: `No se permite reemplazar páginas de un documento firmado digitalmente.`
- Preparando: `Preparando páginas anotadas...`
- Subiendo: `Subiendo páginas anotadas...`
- Reemplazando: `Reemplazando páginas del documento...`
- Success: `Páginas anotadas guardadas correctamente.`
- Error: `No fue posible guardar las páginas anotadas.`

No ocultar el documento visible si falla.

No resetear selección si falla.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CANCELACIÓN Y LATEST-WINS

Implementar:

```ts
replaceAbortRef.current?.abort();
replaceSeqRef.current += 1;
const seq = replaceSeqRef.current;
const abortController = new AbortController();
replaceAbortRef.current = abortController;
```

Después de cada `await`:

```ts
if (seq !== replaceSeqRef.current) return;
if (abortController.signal.aborted) return;
```

Al desmontar:

- abortar operación en curso.
- cancelar temporales creados best-effort si hay tracking.

Si error es cancelación:

- no mostrar toast error.
- no detener documento activo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## PERMISOS

Revisar permisos actuales:

`ViewerEffectivePermissions` incluye:

- `allowAnnotationEdit`
- `allowExport`
- `allowPrint`
- firma add/delete/lock

Regla:

El botón `Guardar anotaciones` debe deshabilitarse si:

- `!permissionsEffective.allowAnnotationEdit`, o
- `!permissionsEffective.allowExport`, o
- documento firmado electrónicamente.

Si `DocumentosWorkbench` no recibe permisos efectivos porque el modo `load()` gestionado está comentado, documentar brecha y decidir:

- reactivar flujo gestionado `visorRef.current.load(...)`, o
- exponer callback desde visor con permisos calculados, o
- mantener botón visible solo por estado documental y confiar en backend.

Recomendación:

Reactivar o corregir modo gestionado del visor para que permisos backend apliquen también a esta acción.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## BRECHA EXISTENTE A RESOLVER

En `DocumentosWorkbench.tsx` existe un bloque comentado que llamaba:

`visorRef.current?.load({...})`

Actualmente el visor se monta con `fileUrl` directo.

Impacto:

- permisos gestionados por `fetchMisPermisosVisorPdf` pueden no aplicarse.
- contexto `idImagen/nombreGabinete/idTareaWorkflow/radicado/nombre_modulo` no fluye al visor.

Regla para esta implementación:

Antes de habilitar guardado de páginas anotadas, decidir y documentar:

1. Reactivar `load()` gestionado con guardrail anti-duplicados, o
2. Mantener `fileUrl` directo y mover toda validación de permisos al Workbench/backend.

Preferencia enterprise:

Reactivar modo gestionado de forma controlada porque el visor ya tiene tipos `AppVisorLoadInput` con `idImagen`, `nombreGabinete`, `idTareaWorkflow`, `radicado`, `nombre_modulo`.

No dejar el botón de guardado ignorando permisos reales sin documentarlo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## MANEJO DE DOCUMENTOS PESADOS

Obligatorio:

- usar `Blob`.
- usar `slice()`.
- no base64.
- subir secuencialmente páginas.
- liberar referencias al terminar.
- no crear copias innecesarias del PDF completo.
- mostrar progreso.
- permitir cancelación.

Riesgo:

Para extraer páginas PDF, primero se usa `saveAsCopy(documentId)` y eso genera un PDF anotado completo en memoria.

Mitigación:

- Documentar que EmbedPDF no expone hoy exportación directa por página.
- Evitar rasterización.
- Evitar base64.
- Subir solo páginas extraídas al backend.

Si la memoria se vuelve problema:

- abrir ticket técnico para soporte de exportación por rango/página desde engine o backend.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## OBSERVABILIDAD FE

Usar patrón local:

`window.__DV_DEBUG__`

Logs debug:

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

Campos:

- `attemptId`
- `documentId`
- `nombreGabinete`
- `annotatedPages`
- `pageNumber`
- `blobSize`
- `hashSha256`
- `rutaTemporalId` solo si no contiene datos sensibles
- `archivoTemporalId`
- `percent`
- `durationMs`

Prohibido loguear:

- bytes del PDF.
- URLs temporales con token.
- Authorization.
- contenido del documento.
- rutas físicas.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## VALIDACIONES FE

Antes de exportar:

- documento activo existe.
- PDF activo existe.
- no firmado electrónicamente.
- ref del visor listo.

Después de exportar:

- `pages.length > 0`.
- cada `pageNumber > 0`.
- sin duplicados.
- cada blob `type === "application/pdf"`.
- cada blob `size > 0`.

Antes de replace:

- todos los uploads completados.
- cada página subida tiene `rutaTemporalId` y `archivoTemporalId`.
- no exigir que todas compartan `rutaTemporalId`; el contrato real acepta rutas temporales por página.
- cada página tiene `archivoTemporalId`.
- `nombreGabinete` no vacío.
- `idDocumento > 0`.
- si se envía `OriginalPdfPassword`, proviene solo de memoria.
- si se envía metadata anti-desfase, corresponde a la versión renderizada actual.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## PRUEBAS OBLIGATORIAS

Unitarias `AppVisorEmbedPdf`:

- `exportAnnotatedPdfPages` retorna vacío sin anotaciones.
- convierte pageIndex base 0 a pageNumber base 1.
- deduplica y ordena páginas.
- ejecuta `commit()` antes de `saveAsCopy()`.
- llama extracción de página por cada página anotada.
- retorna blobs `application/pdf`.
- no descarga archivos.
- falla controlado si `annotationCap.provides` no existe.
- falla controlado si `exportApi.provides` no existe.

Unitarias utilidades:

- `getAnnotatedPageNumbers` con estado vacío.
- `getAnnotatedPageNumbers` con páginas duplicadas/no numéricas.
- `sha256Hex` produce hash esperado para blob conocido.
- extractor de páginas valida rango.

Unitarias servicio upload:

- init usa endpoint correcto.
- init envía extensión `.pdf`.
- chunk usa `application/octet-stream`.
- chunk envía `X-Total-Chunks`.
- complete se llama después del último chunk.
- progress se calcula correctamente.
- cancel se llama best-effort ante error.
- abort cancela sin ocultar error original.
- no usa base64.

Unitarias servicio replace:

- POST usa `/paginas-anotadas`.
- body contiene `NombreGabinete`, `IdDocumento`, `RutaTemporalId`, `Paginas`.
- cada item de `Paginas` contiene su propio `RutaTemporalId`.
- `Paginas.ContentType` es `application/pdf`.
- `DescOp` default es `AGREGA GRAFO PDF`.
- `ModuloRegistro` se resuelve desde el contexto consumidor.
- `DocumentosWorkbench` en contexto de workflow envía `WORKFLOW`.
- otro consumidor puede enviar otro módulo válido sin cambiar el visor.
- no envía IDs workflow negativos.
- envía `OriginalPdfPassword` solo cuando existe en memoria.
- no persiste ni loguea `OriginalPdfPassword`.
- envía metadata anti-desfase cuando está disponible.
- maneja error `originalPdfPassword`.
- maneja error de anti-desfase solicitando recarga.
- adapta respuesta PascalCase/camelCase si aplica.

Unitarias `DocumentosWorkbench`:

- botón deshabilitado sin documento activo.
- botón deshabilitado si no es PDF.
- bloquea firmado electrónicamente.
- sin anotaciones muestra toast y no llama backend.
- con anotaciones ejecuta export -> upload -> replace -> refresh.
- error de upload conserva documento visible.
- error de replace conserva documento visible.
- cancelación no muestra error.
- success reabre documento.

Integración FE:

- flujo completo con dos páginas anotadas.
- PDF pesado simulado con varios chunks.
- falla init.
- falla chunk intermedio.
- falla complete.
- falla replace final.
- reemplazo multipágina con `RutaTemporalId` diferente por página.
- PDF original protegido con contraseña válida e inválida.
- rechazo anti-desfase por hash o geometría de página.
- retry posterior funciona.

QT / Calidad:

- no imports `@embedpdf/*` en `DocumentosWorkbench`.
- no `axios` directo fuera del service.
- no base64.
- no `pdfjs-dist` para este flujo.
- no canvas/rasterización.
- no rutas físicas.
- no password PDF en storage/logs/telemetría.
- no se rompe `viewerKind=image`.
- no se rompe print/export actual.

Regresión:

- export/download actual sigue funcionando.
- print sigue funcionando.
- firma sigue funcionando.
- eliminar firma sigue funcionando.
- bloqueo/desbloqueo de firma sigue funcionando.
- carga de documentos sigue latest-wins.
- `cancelCurrentLoad` sigue operativo.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## DOCUMENTACIÓN OBLIGATORIA

Ruta:

`docs/Architecture/implementacion-de-AppVisorPdf/`

Crear o actualizar:

- `SCRUM-[ID]-FE-Reemplazo-Paginas-PDF-Anotadas-Arquitectura.md`
- `SCRUM-[ID]-FE-Reemplazo-Paginas-PDF-Anotadas-Contrato-API.md`
- `SCRUM-[ID]-FE-Reemplazo-Paginas-PDF-Anotadas-Implementacion.md`
- `SCRUM-[ID]-FE-Reemplazo-Paginas-PDF-Anotadas-Pruebas.md`
- `SCRUM-[ID]-FE-Reemplazo-Paginas-PDF-Anotadas-Observabilidad.md`
- `SCRUM-[ID]-FE-Reemplazo-Paginas-PDF-Anotadas-Seguridad.md`
- `SCRUM-[ID]-FE-Reemplazo-Paginas-PDF-Anotadas-Runbook.md`

Debe incluir:

- flujo export páginas PDF.
- por qué no se envían imágenes.
- contrato `/paginas-anotadas`.
- upload temporal por página.
- cancelación.
- latest-wins.
- permisos.
- brecha del `load()` gestionado.
- pruebas.
- riesgos.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## CRITERIOS DE ACEPTACIÓN

- El visor detecta páginas anotadas.
- El visor exporta PDFs de una sola página para cada página anotada.
- No se generan imágenes.
- No se usa rasterización.
- No se envía PDF completo al endpoint parcial.
- Cada página se sube como `application/pdf`.
- Se usa upload temporal existente.
- Se llama `/paginas-anotadas` con `RutaTemporalId` por página y lista de páginas.
- El `RutaTemporalId` raíz se usa solo como fallback compatible.
- Se maneja `OriginalPdfPassword` solo en memoria cuando el PDF original protegido lo requiere.
- Se envía validación anti-desfase cuando el FE tiene hash/version/geometría/fingerprint.
- Se bloquea documento firmado electrónicamente.
- Se maneja progreso.
- Se maneja cancelación.
- Se maneja latest-wins.
- Si no hay anotaciones, no se llama backend.
- Si falla, el documento visible se conserva.
- En success se refresca el documento.
- No se filtra EmbedPDF a `DocumentosWorkbench`.
- No hay HTTP en toolbar.
- No hay base64.
- No hay canvas.
- Pruebas completas.
- Documentación completa.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RESTRICCIONES

No enviar imágenes.

No reemplazar imágenes internas del PDF.

No rasterizar.

No usar canvas como contrato.

No usar `pdfjs-dist` para este flujo.

No base64.

No rutas físicas desde frontend.

No llamadas API desde `AppPdfToolbar`.

No imports `@embedpdf/*` fuera del visor.

No romper reemplazo total existente.

No romper export actual.

No romper print.

No romper firma.

No romper visualización de imágenes.

No ignorar documento firmado electrónicamente.

No forzar `RutaTemporalId` común; usar el contrato SCRUM-249 real con `RutaTemporalId` por página.

No persistir `OriginalPdfPassword`.

No loguear `OriginalPdfPassword`.

No usar capturas ni canvas como validación anti-desfase.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## RIESGOS Y MITIGACIONES

Riesgo:

El implementador asume una ruta temporal común y falla en reemplazo multipágina.

Mitigación:

Usar el contrato backend SCRUM-249 real: cada `init` crea una ruta temporal por archivo y cada item de `Paginas` envía su propio `RutaTemporalId`. El campo raíz se conserva únicamente como fallback compatible.

Riesgo:

El PDF original cambia entre visualización y guardado de anotaciones.

Mitigación:

Enviar `SourceDocumentHashSha256` y metadata/fingerprint por página cuando estén disponibles; ante rechazo backend, recargar el PDF antes de reintentar.

Riesgo:

PDF original protegido por contraseña.

Mitigación:

Mantener `OriginalPdfPassword` solo en memoria y enviarlo únicamente en el request final cuando el visor lo requirió. Si backend rechaza `originalPdfPassword`, pedir contraseña de nuevo o abortar sin modificar el documento visible.

Riesgo:

EmbedPDF solo exporta PDF completo.

Mitigación:

Exportar completo una vez, extraer páginas PDF sin rasterizar y subir solo páginas.

Riesgo:

Dependencia frontend para manipular PDF no existe.

Mitigación:

Revisar dependencias. Si se requiere `pdf-lib`, documentar licencia, peso e impacto antes de agregar.

Riesgo:

Documento firmado electrónicamente.

Mitigación:

Bloquear en FE y mantener validación backend como fuente final.

Riesgo:

Operación larga.

Mitigación:

Progreso, cancelación, latest-wins y no ocultar documento visible.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

## INSTRUCCIÓN FINAL

Implementar en React el flujo enterprise para guardar anotaciones reemplazando únicamente páginas PDF completas, alineado con la API backend `/paginas-anotadas`.

La solución debe:

- usar `AppVisorEmbedPdf` como única frontera con EmbedPDF.
- exportar páginas PDF anotadas, no imágenes.
- subir PDFs de una página por upload temporal chunked.
- llamar al endpoint parcial con contrato exacto.
- enviar `RutaTemporalId` por cada página, no una ruta común artificial.
- manejar `OriginalPdfPassword` y validación anti-desfase según contrato SCRUM-249.
- bloquear documentos firmados electrónicamente.
- manejar progreso, cancelación, latest-wins, errores y refresh.
- mantener pruebas y documentación completas.

No dejar brechas de integración con la API.
