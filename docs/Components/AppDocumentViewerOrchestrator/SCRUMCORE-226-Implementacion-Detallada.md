# SCRUMCORE-226 - Implementación Detallada

Este documento describe en detalle lo implementado en el repo para `SCRUMCORE-226`.

## Ubicación en el código

Carpeta del core:

- `src/app/Components/UI/AppDocumentViewerOrchestrator/`

Archivos agregados:

- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.types.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.adapter.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.service.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/useDocumentViewerOrchestrator.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/index.ts`

Export agregado al barrel:

- `src/app/Components/UI/index.ts`

## Objetivo técnico del módulo

Entregar un core reusable para visualización documental que consolide resolve + firma + estado runtime con reglas enterprise de:

- Concurrencia (cancelación + stale protection).
- Estabilidad del visor (no perder documento visible ante fallos).
- Seguridad runtime (no persistencia de URLs temporales).

## Hook API

`useDocumentViewerOrchestrator()` expone:

- `visualizarDocumento(input)`
- `documentoActivo`
- `loading`
- `error`
- `reset()`
- `cancelCurrentRequest()`

### Semántica de métodos

- `visualizarDocumento(input)`:
  - Cancela el request anterior (si existe).
  - Ejecuta `visualizacion/resolve`.
  - Consolida `fileUrl`, `contentType`, `isPdf`, `resolveStatus`.
  - Si `isPdf === true`, consulta firma electrónica y consolida `isElectronicallySigned` + `firmaCheckStatus`.
- `cancelCurrentRequest()`:
  - Aborta el request en vuelo mediante `AbortController`.
  - Marca el estado como `cancelled` (sin perder el documento previamente visible).
- `reset()`:
  - Cancela requests en vuelo.
  - Limpia el estado del orquestador (útil para consumidores que quieran resetear el visor).

## Estado runtime: cómo interpretarlo

`documentoActivo` es el **estado consolidado** del documento “actual” del visor (o el último documento estable).

Campos principales:

- `fileUrl`: URL temporal final para el visor. Puede ser `null` si el intento falló y no existe un documento previamente visible.
- `contentType`: `ContentType` devuelto por backend (para detección de PDF).
- `isPdf`: `true` si el documento se interpreta como PDF.
- `isElectronicallySigned`:
  - `true/false` cuando la consulta de firma responde correctamente.
  - `null` cuando no aplica (no PDF) o cuando la consulta falló.
- `resolveStatus`:
  - `idle`: sin intentos.
  - `loading`: se está resolviendo un documento (y el orquestador protege estabilidad).
  - `resolved`: resolve exitoso.
  - `failed`: el intento falló (pero puede mantenerse el documento previamente visible).
  - `cancelled`: request cancelada (por nuevo intento o cancel explícito).
- `firmaCheckStatus`:
  - `not_required`: no PDF.
  - `resolved`: firma consultada y respondida.
  - `failed`: la consulta falló (no se pierde el documento).
- `errors[]`: lista de códigos/mensajes simples consumibles por UI (sin throw no controlado).

## Reglas de negocio críticas (del prompt)

- URL final: `UrlTemporalAbsoluta` tiene prioridad, fallback `UrlTemporal`.
- Firma electrónica: solo para PDF; no bloquear visualización.
- `idArchivo` para firma: `IdDocumento` del resolve.
- Si falla resolve: NO consultar firma.
- Si falla firma: mantener visualización (`fileUrl` estable).

## Reglas de seguridad/runtime

- El orquestador NO persiste `UrlTemporal*` en `localStorage`, `sessionStorage`, `indexedDB` o caches persistentes.
- El orquestador NO loguea ni almacena URLs temporales fuera de memoria.

## Flujo de ejecución (paso a paso)

1. El consumidor llama `visualizarDocumento({ documentId, nombreGabinete, context? })`.
2. El orquestador:
   - Cancela request previa con `AbortController`.
   - Incrementa `requestId` para protegerse de respuestas out-of-order.
3. Llama a `visualizacion/resolve`.
4. Si resolve responde:
   - Selecciona `fileUrl` (`UrlTemporalAbsoluta` > `UrlTemporal`).
   - Calcula `isPdf`.
   - Publica estado con `resolveStatus="resolved"` y `fileUrl` listo para el visor.
5. Si `isPdf=true`, dispara consulta de firma electrónica (side-effect):
   - La visualización NO se bloquea (el visor puede abrir con `fileUrl`).
   - Al resolver firma, actualiza `isElectronicallySigned` y `firmaCheckStatus`.
   - Si falla firma, mantiene `fileUrl` y marca `firmaCheckStatus="failed"`.
6. Si una respuesta llega tarde (stale), se ignora por `requestId`.

## Errores y estabilidad (reglas que deben preservarse)

- Si falla `visualizacion/resolve`:
  - El intento no produce una URL nueva válida (`fileUrl=null` para el intento).
  - Si ya existía un documento visible, se mantiene (no flicker / no pérdida).
- Si falla firma electrónica:
  - Se mantiene el documento visible.
  - `isElectronicallySigned=null` y `firmaCheckStatus="failed"`.

## Ejemplo de integración (pseudocódigo, sin hardcodear lógica de módulo)

```ts
const { visualizarDocumento, documentoActivo, loading, error } = useDocumentViewerOrchestrator();

// 1) El módulo consumidor decide el input canónico:
// const input = { documentId, nombreGabinete, context?: ... }

await visualizarDocumento(input);

// 2) El visor consume el estado consolidado:
// <AppVisorEmbedPdf fileUrl={documentoActivo?.fileUrl} />
// UI decide cómo mostrar loading/error, sin reimplementar resolve/firma.
```

## Concurrencia

- Cancela requests previas.
- Usa `requestId` incremental para ignorar respuestas stale (out-of-order).

## Estabilidad

- Si un nuevo intento falla, el documento previamente visible se mantiene (sin flicker / sin pérdida).

## Service (integración HTTP)

Archivo:
- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.service.ts`

Implementado:
- `resolveVisualizacionDocumento({ request, signal })` llama `POST /api/gestor-documental/documentos/visualizacion/resolve` usando `clienteApi`.
- `fetchFirmaElectronica({ idArchivo, nombreGabinete, signal })` llama el endpoint de firma electrónica (solo cuando corresponde, desde el hook).
- Se soporta cancelación usando `AbortSignal` (axios `ERR_CANCELED` → `AbortError`).

### Propagación del mensaje del backend (decisión UX vigente)

**Decisión:** el frontend **no reescribe** los mensajes de negocio/validación del backend. Si el backend responde con un mensaje humano (por ejemplo: `No existe carpeta física del documento`), ese texto se **propaga** al consumidor para que se muestre tal cual en la UI.

### Por qué lo maneja el orquestador (y no el módulo consumidor)

Este comportamiento se implementa en el orquestador porque el error ocurre **dentro del flujo de integración** que el orquestador encapsula (`visualizacion/resolve` y, si aplica, `firma-electronica`). En términos arquitectónicos, el orquestador es el **único punto** que debe conocer y orquestar:

- Cómo se invoca `visualizacion/resolve` y cómo se interpreta su respuesta.
- Cómo se normalizan fallos HTTP a un estado runtime consistente (sin excepciones no controladas).
- Cómo se preserva la **estabilidad del visor**: si un nuevo intento falla, no se pierde el documento previamente visible.
- Cómo se evita duplicación: sin esta normalización central, cada módulo consumidor tendría que parsear el `400` y decidir qué mensaje mostrar, produciendo divergencias y manejo desigual de errores.

Por diseño, el módulo consumidor solo decide **cuándo** llamar `visualizarDocumento()` y con qué contrato canónico; el orquestador decide **cómo** resolver y cómo consolidar el estado y errores para consumo de UI.

**Dónde se implementa (source of truth):**

- `src/app/Components/UI/AppDocumentViewerOrchestrator/useDocumentViewerOrchestrator.ts`
  - En el `catch` del flujo `resolveVisualizacionDocumento(...)` el hook intenta extraer un mensaje del error HTTP (Axios):
    - Prioridad 1: `response.data.errors[0].Message`
    - Prioridad 2 (fallback): `response.data.message`
  - Si existe `apiMessage`:
    - `state.error` se establece en `apiMessage` (mensaje listo para UI).
    - `documentoActivo.errors[]` incluye `apiMessage` y, adicionalmente, conserva el código interno `RESOLVE_FAILED` como respaldo técnico.

**Motivación:**

- Evitar duplicación de copy de errores (backend y frontend).
- Mantener trazabilidad del mensaje real que explica por qué el documento no es resoluble.
- El consumidor puede mostrar el mensaje en:
  - un toast global (notificaciones del proyecto), o
  - un banner inline en el módulo,
  sin que el orquestador dicte UI.

**Implicaciones:**

- Si el backend cambia el texto del mensaje, el frontend lo mostrará actualizado sin despliegues.
- El mensaje puede variar en redacción; si en el futuro se requiere estandarizar UX, se recomienda introducir `code` estable en backend y un diccionario centralizado en frontend (ver sección de Deuda técnica).

**Ejemplo real (caso storage sin carpeta física):**

Backend (`POST /api/gestor-documental/documentos/visualizacion/resolve`) responde `400`:

```json
{
  "success": false,
  "message": "Error de validacion",
  "data": null,
  "meta": { "Status": "validation" },
  "errors": [
    { "Type": "Validation", "Message": "No existe carpeta física del documento", "Field": "storage" }
  ]
}
```

Orquestador (estado consolidado) deja disponible el mensaje para UI:

```ts
{
  resolveStatus: "failed",
  error: "No existe carpeta física del documento",
  documentoActivo: {
    // ...documentId/nombreGabinete previos (no se pierde el documento visible)
    errors: ["No existe carpeta física del documento", "RESOLVE_FAILED"]
  }
}
```

## Adapter (mapeo/decisiones puras)

Archivo:
- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.adapter.ts`

Funciones clave:
- `pickResolvedFileUrl(dto)` implementa la regla: `UrlTemporalAbsoluta` > `UrlTemporal`.
- `isPdfFromContentType(contentType, fileName?)` detecta PDF por `ContentType` y usa fallback por extensión `FileName`.
- Builders para crear el estado runtime sin acoplar UI.

## Compatibilidad legacy vs nueva arquitectura

Este core implementa las reglas del prompt evitando dependencias a `fileUrl/url legacy` y evitando que módulos consumidores repliquen lógica. Si existe consumo legacy, la migración esperada es:

- Antes: consumidor resuelve URL/estado por su cuenta y pasa `fileUrl` al visor.
- Después: consumidor llama `visualizarDocumento()` y pasa `documentoActivo.fileUrl` al visor.

## Performance y memoización

- `visualizarDocumento`, `reset`, `cancelCurrentRequest` se exponen memoizados.
- Se reduce trabajo evitando múltiples resolves simultáneos mediante cancelación.

## Accesibilidad y UX (responsabilidad de consumidores)

El orquestador no renderiza UI. Sin embargo, define señales consistentes para UX:

- `loading`: consumidores lo usan para mostrar estado perceptible.
- `error` / `errors[]`: consumidores lo usan para mensajes visibles sin romper el visor.
- Estabilidad: evita flicker/pérdida del documento en fallos de nuevas visualizaciones.

## Descarga autenticada (blob) para `download/{token}`

Para evitar fallos `401/403` al abrir URLs protegidas (por ejemplo `visualizacion/download/{token}`) sin `Authorization` header, el orquestador descarga el archivo con el cliente HTTP autenticado del proyecto y entrega al visor un `blob:` URL en memoria:

- Service: `downloadVisualizacionBlob({ fileUrl, signal })` usa `clienteApi.get(..., { responseType: "blob" })` (con `withCredentials` y Bearer si aplica por interceptores).
- Normalización robusta de URL:
  - Si `fileUrl` es absoluta (p.ej. `http://localhost/.../download/{token}`), se normaliza a ruta relativa (`pathname + search`) para que `clienteApi` aplique `baseURL`, cookies e interceptores.
  - Guardrail anti-duplicación: si el `baseURL` ya contiene un prefijo de path (p.ej. `/DocuArchiApi`) y el `pathname` también lo trae, el service recorta el prefijo para evitar URLs como `/DocuArchiApi/DocuArchiApi/api/...`.
- Runtime: el hook crea `URL.createObjectURL(blob)` y lo expone como `documentoActivo.fileUrl` (formato `blob:`) para `AppVisorEmbedPdf`.
- Cleanup/Leak prevention:
  - En cancelación (`cancelCurrentRequest`) se revoca el `blob:` URL (`URL.revokeObjectURL(...)`).
  - En visualizaciones consecutivas se revoca el `blob:` anterior antes de crear uno nuevo.

### Control de loading y estabilidad del visor (blob lifecycle)

Para evitar “falsos positivos” de documento protegido/dañado bajo clicks rápidos (race conditions) y para soportar documentos grandes:

- El orquestador **no revoca inmediatamente** el `blob:` URL previo cuando llega un nuevo documento.
- En su lugar, programa la revocación de forma diferida (guardrail) para reducir el riesgo de invalidar el blob mientras el motor del visor aún lo está leyendo.
- Esto preserva la regla: *si falla/cancela una nueva visualización, el documento previamente visible no se pierde*.

### Motivación (incidente real)

En ambiente local se observó:

- `visualizacion/resolve` retornaba `200` con `UrlTemporalAbsoluta` apuntando a `http://localhost/...` (sin puerto).
- `AppVisorEmbedPdf` intentaba abrir el `download/{token}` directo (sin header `Authorization`) y el backend respondía `401`.
- El visor interpretaba el response no-PDF como documento “dañado/protegido” (password prompt/errores).

La descarga como `Blob` mediante `clienteApi` elimina la dependencia de cookies/origin del request directo y hace el flujo estable en dev/QA/prod.

## Trazabilidad (archivo -> símbolo -> test)

- `src/app/Components/UI/AppDocumentViewerOrchestrator/AppDocumentViewerOrchestrator.adapter.ts`
  - `pickResolvedFileUrl` -> `src/app/Components/UI/AppDocumentViewerOrchestrator/tests/AppDocumentViewerOrchestrator.adapter.test.ts`
  - `isPdfFromContentType` -> `src/app/Components/UI/AppDocumentViewerOrchestrator/tests/AppDocumentViewerOrchestrator.adapter.test.ts`
- `src/app/Components/UI/AppDocumentViewerOrchestrator/useDocumentViewerOrchestrator.ts`
  - `useDocumentViewerOrchestrator` -> `src/app/Components/UI/AppDocumentViewerOrchestrator/tests/useDocumentViewerOrchestrator.test.tsx`

## Casos pendientes / no implementados

- Integración real con el consumidor final (módulo que obtiene `{documentId, nombreGabinete}`) para cerrar el flujo end-to-end.
- Tests E2E Playwright del flujo completo (requiere integración real + fixtures).

## Deuda técnica / Recomendaciones futuras

- Definir contrato de errores más granular (códigos/enum) y mapping a mensajes de UI uniforme.
- Agregar métricas/observabilidad (sin loguear URLs) si el producto requiere trazabilidad runtime.
- Extender tests para escenarios de red lenta/timeouts y comportamiento de reintento (si se decide soportarlo).
