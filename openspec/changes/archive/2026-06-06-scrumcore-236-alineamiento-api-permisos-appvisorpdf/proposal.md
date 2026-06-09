## Why

`AppVisorEmbedPdf` ya consume el endpoint de permisos del visor PDF, pero el frontend no esta alineado con el contrato oficial de la API.

El problema principal es doble:

- El service espera `Permissions` en la raiz de la respuesta, mientras la API entrega un envelope con `data.Permissions`.
- El mapper de permisos usa codigos antiguos o no documentados (`pdf.export`, `pdf.signature.add`, `pdf.annotation.edit`) mientras el backend documenta codigos como `pdf.download`, `pdf.annotate.signature.place`, `pdf.annotate.signature.delete`, `pdf.zoom`, entre otros.

Esto provoca que una respuesta valida del backend pueda terminar como `raw: {}` o como permisos efectivos en `false`, bloqueando acciones del visor aunque el usuario tenga permisos configurados.

## What Changes

- Alinear `AppVisorEmbedPdf.service.ts` con el envelope real de la API:
  - `success`
  - `message`
  - `data`
  - `meta`
  - `errors`
- Retornar desde el service el payload real `data`, no el envelope completo.
- Mantener `AbortSignal` en requests de permisos.
- Mantener `codigoImpl = gestion_correspondencia` para el flujo de Gestion Correspondencia.
- No enviar `idUsuario` desde frontend en `mis-permisos`; backend lo resuelve desde el JWT.
- Actualizar el mapping frontend para usar los codigos reales del contrato:
  - `pdf.print`
  - `pdf.download`
  - `pdf.annotate.signature.place`
  - `pdf.annotate.signature.delete`
  - `pdf.annotate.signature.lock`
  - `pdf.annotate.signature.unlock`
  - `pdf.annotate.open_signature_modal`
  - `pdf.annotate.signature.draw`
  - `pdf.annotate.signature.upload`
  - `pdf.annotate.signature.personal`
- Documentar `pdf.view`, `pdf.zoom` y `pdf.rotate` como permisos backend disponibles, pero no ampliar `ViewerEffectivePermissions` en esta iteracion salvo requerimiento explicito.
- Conservar fail-closed para acciones sensibles cuando la API falle o el contrato sea invalido.
- Generar documentacion enterprise en `docs/Architecture/AlineamientoContratoApiPermisosVisor/`.

## Capabilities

### New Capabilities

- `alineamiento-api-permisos-appvisorpdf`: alineamiento del consumo frontend de permisos del visor PDF con el contrato oficial de backend.

### Modified Capabilities

- `appvisor-embed-pdf-permissions`: consumo, tipado y mapping de permisos efectivos del visor.
- `gestion-correspondencia-documentos-workbench`: mantiene el flujo managed del visor sin asumir responsabilidad de permisos.

## Impact

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts`
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx` solo revision, sin mover permisos ahi.
- Tests nuevos o ajustados para service y mapping de permisos.
- Documentacion enterprise nueva:
  - `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Metadata.md`
  - `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Arquitectura.md`
  - `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Contrato-API.md`
  - `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Implementacion-Detallada.md`
  - `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Pruebas.md`
  - `docs/Architecture/AlineamientoContratoApiPermisosVisor/PROMPT-SCRUMCORE-236-AlineamientoContratoApiPermisosVisor.md`

## Non-Goals

- No implementar pantalla admin de permisos.
- No consumir endpoints admin desde el visor normal.
- No enviar `idUsuario`, `codiperfil` ni overrides desde frontend en el flujo `mis-permisos`.
- No mover logica de permisos a `AppTreeTable`.
- No mover policy a `DocumentosWorkbench`.
- No ampliar controles visuales para `pdf.view`, `pdf.zoom` o `pdf.rotate` salvo soporte existente o requerimiento explicito.
