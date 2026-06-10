# Spec - SCRUMCORE-226: AppDocumentViewerOrchestrator

## Alcance

Implementar un núcleo reusable (sin UI) llamado `AppDocumentViewerOrchestrator` que consolida el estado de visualización documental para consumo por `AppVisorEmbedPdf`, incluyendo resolve de URL y consulta de firma electrónica solo para PDF.

## Reglas obligatorias (guardrails)

- **No cambiar backend** ni endpoints.
- **No** depender de módulos específicos (`DocumentosWorkbench`, `AppTreeTable`, etc.).
- **No** invocar `action/ver_documento`.
- **No** introducir lógica visual ni de permisos.
- **No** persistir URLs temporales (localStorage/sessionStorage/caches persistentes).
- TypeScript estricto, **sin `any`**.
- Si falla resolve o firma, el documento previamente visible **no se pierde**.

## Contratos

### Entrada (canónica)

- `documentId: number`
- `nombreGabinete: string`
- `context?` opcional (tiene solo propósito de trazabilidad futura, sin lógica actual obligatoria).

### Salida (estado runtime)

El orquestador SHALL exponer un estado consolidado con:

- `fileUrl: string | null`
- `contentType: string | null`
- `isPdf: boolean`
- `isElectronicallySigned: boolean | null`
- `resolveStatus: "idle" | "loading" | "resolved" | "failed" | "cancelled"`
- `firmaCheckStatus: "not_required" | "resolved" | "failed"`
- `errors: string[]`

### Contrato del hook reusable (del prompt)

El sistema SHALL exponer `useDocumentViewerOrchestrator()` que provee:

- `visualizarDocumento(input)` (dispara resolve + firma si aplica)
- `documentoActivo` (estado consolidado del documento actualmente visible/activo)
- `loading` (derivado de `resolveStatus === "loading"` y/o firma en progreso si se decide exponerlo)
- `error` (derivado de `errors[]` o del último error relevante)
- `reset()` (vuelve a estado `idle` sin romper estabilidad del documento visible si el consumidor así lo decide)
- `cancelCurrentRequest()` (cancela requests en vuelo y marca estado como `cancelled` o stale)

## Integraciones (backend)

### Resolve visualización

`POST /api/gestor-documental/documentos/visualizacion/resolve`

Request:
- `NombreGabinete: string`
- `IdDocumento: number`

Response (mínimo esperado):
- `IdDocumento: number`
- `NombreGabinete: string`
- `FileName: string`
- `ContentType: string`
- `UrlTemporal: string`
- `UrlTemporalAbsoluta: string | null`
- `ExpiresAt: string`

### Firma electrónica

`GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}`

Response (mínimo esperado):
- `FirmadoElectronico: boolean`

## Requirements

### Requirement R1: Resolve y selección de URL

El sistema SHALL resolver la visualización y seleccionar la URL final para el visor.

#### Scenario R1.1: Prioridad de URL absoluta
- **GIVEN** un documento `{documentId, nombreGabinete}`
- **WHEN** `visualizacion/resolve` responde con `UrlTemporalAbsoluta`
- **THEN** `fileUrl` MUST ser `UrlTemporalAbsoluta`

#### Scenario R1.2: Fallback a URL relativa/temporal
- **GIVEN** un documento `{documentId, nombreGabinete}`
- **WHEN** `visualizacion/resolve` responde `UrlTemporalAbsoluta = null` y `UrlTemporal` no vacío
- **THEN** `fileUrl` MUST ser `UrlTemporal`

#### Scenario R1.3: Resolve falla
- **GIVEN** existe un documento previamente visible
- **WHEN** falla `visualizacion/resolve` para un nuevo intento
- **THEN** `resolveStatus = "failed"`
- **AND** el intento fallido MUST producir `fileUrl = null` (no hay URL nueva válida)
- **AND** el **documento previamente visible** MUST mantenerse (estabilidad del visor)
- **AND** NO se debe consultar firma electrónica

### Requirement R2: Detección de PDF

El sistema SHALL detectar si el documento resuelto es PDF.

#### Scenario R2.1: PDF por ContentType
- **WHEN** `ContentType` indica PDF
- **THEN** `isPdf = true`

#### Scenario R2.2: No PDF
- **WHEN** `ContentType` no indica PDF
- **THEN** `isPdf = false`

### Requirement R3: Firma electrónica solo para PDF

El sistema SHALL consultar firma electrónica únicamente cuando el documento es PDF.

#### Scenario R3.1: PDF => consulta firma
- **GIVEN** `isPdf = true` y `resolveStatus = "resolved"`
- **WHEN** se completa el resolve
- **THEN** se consulta `firma-electronica` usando el `IdDocumento` resuelto como `idArchivo`

#### Scenario R3.2: No PDF => no consulta firma
- **GIVEN** `isPdf = false`
- **THEN** `firmaCheckStatus = "not_required"`
- **AND** NO se llama el endpoint de firma

#### Scenario R3.3: Firma falla sin perder visor
- **GIVEN** `fileUrl` ya resuelta y visible
- **WHEN** falla `firma-electronica`
- **THEN** `firmaCheckStatus = "failed"`
- **AND** `isElectronicallySigned = null`
- **AND** el documento visible MUST mantenerse

### Requirement R4: Concurrencia (anti-race)

El sistema SHALL protegerse contra múltiples visualizaciones concurrentes, cancelando requests previas e ignorando respuestas stale.

#### Scenario R4.1: Cancelación de request anterior
- **GIVEN** una visualización en progreso
- **WHEN** el consumidor llama `visualizarDocumento()` con otro documento
- **THEN** el request anterior MUST ser cancelado o marcado como stale

#### Scenario R4.2: Ignorar respuesta stale
- **GIVEN** dos requests A (antiguo) y B (nuevo)
- **WHEN** A responde después de B
- **THEN** el estado consolidado MUST reflejar B y MUST ignorar A

## Observabilidad

- El orquestador SHOULD agregar errores legibles a `errors[]` (sin lanzar excepciones no controladas).
- El orquestador SHOULD exponer un método para cancelar la request actual.

## Documentación (del ticket)

El sistema SHALL entregar documentación mínima del componente en `docs/Components/AppDocumentViewerOrchestrator/` con artefactos ligados a `SCRUMCORE-226` (arquitectura, integración backend, pruebas, metadata).
