# Design - SCRUMCORE-226

## Contexto

Ticket: `SCRUMCORE-226` — **IMPLEMENTACION-ORQUESTADOR-DOCUMENTO-VISOR**

Construir un núcleo reusable (sin UI) llamado `AppDocumentViewerOrchestrator` para orquestar:

- Resolve de visualización documental (`visualizacion/resolve`)
- Selección de URL final (preferir `UrlTemporalAbsoluta`, fallback `UrlTemporal`)
- Detección de PDF
- Consulta de firma electrónica **solo si es PDF**
- Consolidación de estado runtime para consumo por `AppVisorEmbedPdf`

El objetivo es crear la plataforma reusable; la UI/permissions pertenecen al módulo consumidor.

## Objetivos / No-objetivos

**Objetivos**
- Evitar duplicación de lógica de visualización en módulos consumidores.
- Evitar *race conditions* / respuestas *stale* y mantener estabilidad del visor.
- Definir contratos estrictos (TypeScript estricto, sin `any`).

**No-objetivos**
- No introducir lógica visual (UI), permisos, ni toolbars.
- No cambiar backend ni endpoints.
- No depender de módulos específicos (p.ej. `DocumentosWorkbench`, `AppTreeTable`).
- No invocar `action/ver_documento`.
- No persistir URLs temporales (local/session storage, caches persistentes).
- No reconstruir payloads ni inferir datos a partir de DTOs de UI (rows/metadata). El consumidor aporta el contrato canónico y el orquestador orquesta.

## Ubicación esperada (código)

`src/app/Components/UI/AppDocumentViewerOrchestrator/`

- `AppDocumentViewerOrchestrator.types.ts`
- `AppDocumentViewerOrchestrator.adapter.ts`
- `AppDocumentViewerOrchestrator.service.ts`
- `useDocumentViewerOrchestrator.ts`
- `index.ts`
- `tests/`

## Diseño propuesto

### Contratos (source of truth)

Entrada canónica mínima:

```ts
{ documentId: number; nombreGabinete: string }
```

Entrada extendida (solo trazabilidad futura, opcional):

```ts
{
  documentId: number;
  nombreGabinete: string;
  context?: { idTareaWorkflow?: number; radicado?: string; grafo?: object };
}
```

Salida consolidada (state runtime):

```ts
{
  documentId: number;
  nombreGabinete: string;
  fileUrl: string | null;
  contentType: string | null;
  isPdf: boolean;
  isElectronicallySigned: boolean | null;
  firmaCheckStatus: "not_required" | "resolved" | "failed";
  resolveStatus: "idle" | "loading" | "resolved" | "failed" | "cancelled";
  errors: string[];
}
```

### Regla “source of truth” (del prompt)

- El orquestador recibe **únicamente** `{ documentId, nombreGabinete }` como contrato canónico.
- La obtención de `DocumentResolveRequest`, `action/ver_documento` o metadata de filas pertenece al módulo consumidor.
- El orquestador **no** debe inferir datos, reconstruir payloads alternos, ni depender de DTOs de UI.

### Endpoints (contrato backend)

Resolve visualización:

`POST /api/gestor-documental/documentos/visualizacion/resolve`

Request:

```json
{ "NombreGabinete": "string", "IdDocumento": 123 }
```

Firma electrónica:

`GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}`

### Flujos

**Flujo principal**
1. El consumidor invoca `visualizarDocumento({ documentId, nombreGabinete, context? })`.
2. Se cancela cualquier request previa y se crea un `requestId` para ignorar respuestas stale.
3. Se llama a `visualizacion/resolve`.
4. Se determina `fileUrl` (prioridad `UrlTemporalAbsoluta`, fallback `UrlTemporal`). No se persiste.
5. Se detecta PDF por `ContentType` (fallback por extensión `FileName` si aplica).
6. Si no es PDF: `firmaCheckStatus = "not_required"` y termina.
7. Si es PDF: se consulta firma electrónica sin bloquear la visualización.

**Regla de estabilidad**
- Si falla `resolve`: no consultar firma. El intento fallido no produce un `fileUrl` nuevo (`fileUrl = null` para el intento).
- Si falla firma: mantener visualización; `isElectronicallySigned = null` y `firmaCheckStatus = "failed"`.
- Nunca “vaciar” el documento previamente visible por un fallo en una nueva visualización (no pérdida de estabilidad del visor).

### Concurrencia (anti-race)

- Cancelación: `AbortController` por request.
- Ignorar stale: `requestId` incremental + validación al resolver.
- Evitar flicker: actualizar `documentoActivo` solo cuando el resolve es exitoso (y mantener el anterior si el nuevo falla).

## Decisiones

1. Orquestador = core reusable sin UI; consumidores solo aportan `{ documentId, nombreGabinete }`.
2. Firma electrónica es un *side effect* solo para PDF y no bloquea el resolve.
3. URLs temporales viven en memoria únicamente.

## Riesgos / trade-offs

- Diferencias en `ContentType` requieren fallback robusto de detección PDF.
- El manejo “no perder documento previo” necesita reducer/estado cuidadoso.
- Clicks rápidos: respuestas out-of-order deben ser ignoradas (test obligatorio).

## Plan de migración

1. Introducir `AppDocumentViewerOrchestrator` + tests unitarios del core.
2. Integrar `AppVisorEmbedPdf` para consumir estado consolidado (sin tocar permisos/UI).
3. Migrar módulos consumidores gradualmente a `visualizarDocumento()`.

## Preguntas abiertas

- ¿Qué wrapper HTTP se usa hoy para requests y cancelación (axios interceptors / fetch wrapper)?
- ¿Cuáles módulos serán primeros consumidores (para asegurar API correcta desde el inicio)?
