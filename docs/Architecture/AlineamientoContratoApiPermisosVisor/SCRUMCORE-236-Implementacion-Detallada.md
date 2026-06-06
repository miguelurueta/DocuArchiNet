# SCRUMCORE-236 - Implementacion Detallada

## Cambios por archivo

### `AppVisorEmbedPdf.service.ts`

- Define envelope API local.
- Consume `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`.
- Valida `success=true`.
- Valida existencia de `data.Permissions`.
- Retorna `envelope.data`.
- Mantiene `AbortSignal`.

### `AppVisorEmbedPdf.permissions.ts`

- Agrega `VisorPdfPermissionCode`.
- Tipifica `Permissions` con codigos oficiales.
- Resuelve `gestioncorrespondencia -> gestion_correspondencia`.
- Mapea claves oficiales:
  - `pdf.print`
  - `pdf.download`
  - `pdf.annotate.signature.place`
  - `pdf.annotate.signature.delete`
  - `pdf.annotate.signature.lock`
  - `pdf.annotate.signature.unlock`

### `AppVisorEmbedPdf.tsx`

- Mantiene flujo `load`.
- Usa el payload `data` retornado por el service.
- Mantiene debug log bajo `window.__DV_DEBUG__`.

## Antes

```text
res.data.Permissions
pdf.export
pdf.signature.add
pdf.annotation.edit
```

## Despues

```text
res.data.data.Permissions
pdf.download
pdf.annotate.signature.place
pdf.annotate.signature.delete
```

## Fail-closed

Si la API falla, `success=false`, o falta `data.Permissions`, el visor bloquea acciones sensibles.

## Compatibilidad futura

Otros modulos pueden agregar mapping en `resolveCodigoImplementacion` sin tocar `AppTreeTable` ni duplicar permisos.
