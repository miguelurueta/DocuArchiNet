# SCRUMCORE-226 - Integración BackEnd

Este ticket **no** modifica backend ni endpoints. Solo consume contratos existentes.

## Resolve visualización

- Endpoint: `POST /api/gestor-documental/documentos/visualizacion/resolve`
- Request: `{ "NombreGabinete": string, "IdDocumento": number }`

Uso en el core:
- `useDocumentViewerOrchestrator()` invoca el resolve a través de `resolveVisualizacionDocumento()`.
- Selección de URL final:
  - Prioridad: `UrlTemporalAbsoluta`
  - Fallback: `UrlTemporal`

### Request/Response (forma real esperada)

Request (JSON):

```json
{ "NombreGabinete": "GABINETE", "IdDocumento": 123 }
```

Response (campos mínimos consumidos por el core):

```json
{
  "IdDocumento": 123,
  "NombreGabinete": "GABINETE",
  "FileName": "document.pdf",
  "ContentType": "application/pdf",
  "Origen": "ORIGINAL",
  "UrlTemporal": "/api/.../download/tok-1",
  "UrlTemporalAbsoluta": "https://.../tok-1",
  "ExpiresAt": "2026-01-01T00:00:00.000Z"
}
```

## Firma electrónica

- Endpoint: `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}`
- Regla: solo se consulta si el documento es PDF.

Uso en el core:
- Se consulta con `idArchivo = IdDocumento` resuelto por `visualizacion/resolve`.
- La consulta **no bloquea** la visualización del documento.
- Si falla, se conserva el documento visible y se consolida `firmaCheckStatus = "failed"` + `isElectronicallySigned = null`.

### Request/Response (forma real esperada)

Request:
- Path param: `{idArchivo} = IdDocumento`
- Query: `nombreGabinete`

Response (campos mínimos consumidos por el core):

```json
{
  "IdArchivo": 123,
  "NombreGabinete": "GABINETE",
  "FirmadoElectronico": true,
  "IdCertificado": 999
}
```
