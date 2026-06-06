# SCRUMCORE-236 - Metadata

- Ticket: SCRUMCORE-236
- Nombre: ALINEAMIENTO-API-PERMISOS-APPVISORPDF
- Tipo: Alineamiento de contrato API / permisos frontend
- Fecha: 2026-06-06
- Modulo: `AppVisorEmbedPdf`
- Endpoint principal: `GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`

## Archivos impactados

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.test.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.test.ts`

## Decisiones

- `codigoImpl` para Gestion Correspondencia: `gestion_correspondencia`.
- `idUsuario` no se envia desde frontend en `mis-permisos`.
- El service consume el envelope oficial y retorna `data`.
- El mapper usa codigos backend documentados.
- `pdf.view`, `pdf.zoom` y `pdf.rotate` quedan documentados sin ampliar UI en esta iteracion.

## Estado esperado

- Implementacion: service, mapping, debug log y tests focalizados.
- Documentacion: contrato, arquitectura, implementacion y pruebas.
- QA: validar en navegador con `window.__DV_DEBUG__ = true`.

## Riesgos

- Backend puede devolver `Permissions` vacio si el usuario no tiene perfil o defaults.
- Ambientes antiguos con `Permissions` en raiz fallaran por contrato invalido.
- `pdf.view`, `pdf.zoom`, `pdf.rotate` no alteran UI en este ticket.
