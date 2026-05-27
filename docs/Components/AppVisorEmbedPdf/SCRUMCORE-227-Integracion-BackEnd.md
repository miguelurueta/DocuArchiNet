# SCRUMCORE-227 — AppVisorEmbedPdf.load() (Integración BackEnd)

## Restricción
- Backend NO modificado.
- Endpoints NO modificados.

## Endpoint consumido (permisos visor PDF)
**GET**
`/api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`

### Response esperado
```json
{
  "CodigoImplementacion": "string",
  "IdUsuario": 0,
  "Permissions": { "pdf.signature.add": true },
  "Sources": { "pdf.signature.add": "policy" },
  "GeneratedAt": "2026-05-26T00:00:00Z"
}
```

## Notas de contrato
- `codigoImpl` se resuelve vía mapping centralizado `resolveCodigoImplementacion(nombre_modulo)`.
- Si el mapping falla:
  - `permissionStatus="failed"`
  - `permissionsEffective` en modo fail-closed para edición
  - la visualización NO se bloquea.

