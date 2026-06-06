# SCRUMCORE-236 - Contrato API Permisos Visor PDF

## Endpoint usuario autenticado

```http
GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos
Authorization: Bearer {jwt}
```

## Claims requeridos

- `defaulalias`
- `usuarioid`

## Valores `codigoImpl`

- `workflow`
- `gestion_correspondencia`

## Response OK

```json
{
  "success": true,
  "message": "OK",
  "data": {
    "CodigoImplementacion": "gestion_correspondencia",
    "IdUsuario": 205,
    "Permissions": {
      "pdf.view": true,
      "pdf.print": false,
      "pdf.download": false,
      "pdf.annotate.open_signature_modal": false,
      "pdf.annotate.signature.draw": false,
      "pdf.annotate.signature.upload": false,
      "pdf.annotate.signature.personal": false,
      "pdf.annotate.signature.place": false,
      "pdf.annotate.signature.delete": false,
      "pdf.annotate.signature.lock": false,
      "pdf.annotate.signature.unlock": false,
      "pdf.rotate": false,
      "pdf.zoom": true
    },
    "Sources": {
      "pdf.view": "perfil_activo",
      "pdf.zoom": "perfil_activo"
    },
    "GeneratedAt": "2026-05-20T14:40:00Z"
  },
  "meta": {
    "Status": "success",
    "Total": 13
  },
  "errors": []
}
```

## Response error

```json
{
  "success": false,
  "message": "No cuenta con permisos administrativos",
  "data": {},
  "meta": {
    "Status": "validation"
  },
  "errors": [
    {
      "Type": "Validation",
      "Field": "authorization",
      "Message": "No cuenta con permisos administrativos"
    }
  ]
}
```

## Interpretacion FE

| Codigo backend | Uso frontend |
|---|---|
| `pdf.print` | `allowPrint` |
| `pdf.download` | `allowExport` |
| `pdf.annotate.signature.place` | `allowSignaturePlacement` |
| `pdf.annotate.signature.delete` | `allowSignatureDelete` |
| `pdf.annotate.signature.lock` | `allowSignatureLockToggle` |
| `pdf.annotate.signature.unlock` | `allowSignatureLockToggle` |
| `pdf.annotate.open_signature_modal` | contribuye a `allowAnnotationEdit` |
| `pdf.annotate.signature.draw` | contribuye a `allowAnnotationEdit` |
| `pdf.annotate.signature.upload` | contribuye a `allowAnnotationEdit` |
| `pdf.annotate.signature.personal` | contribuye a `allowAnnotationEdit` |
| `pdf.view` | documentado, no conectado a UI nueva |
| `pdf.rotate` | documentado, no conectado a UI nueva |
| `pdf.zoom` | documentado, no conectado a UI nueva |

## Reglas

- No enviar `idUsuario` en `mis-permisos`.
- No enviar `codiperfil`.
- No usar endpoints admin desde el visor normal.
- Si el envelope no contiene `success=true` y `data.Permissions`, el frontend aplica fail-closed.
