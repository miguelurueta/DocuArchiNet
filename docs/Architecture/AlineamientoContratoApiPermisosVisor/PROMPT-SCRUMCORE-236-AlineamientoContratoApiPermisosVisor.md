# PROMPT SCRUMCORE-236 - Alineamiento Contrato API Permisos Visor

Implementar el alineamiento frontend del visor PDF con el contrato oficial:

`GET /api/gestor-documental/permisos-visorpdf/implementaciones/{codigoImpl}/mis-permisos`

El service debe leer `response.data.data.Permissions`, no `Permissions` en raiz.

El mapper debe usar:

- `pdf.print`
- `pdf.download`
- `pdf.annotate.open_signature_modal`
- `pdf.annotate.signature.draw`
- `pdf.annotate.signature.upload`
- `pdf.annotate.signature.personal`
- `pdf.annotate.signature.place`
- `pdf.annotate.signature.delete`
- `pdf.annotate.signature.lock`
- `pdf.annotate.signature.unlock`

Mantener:

- `nombre_modulo: "gestioncorrespondencia"`
- `codigoImpl: "gestion_correspondencia"`
- `idUsuario` resuelto por backend desde JWT
- fail-closed para acciones sensibles
- no endpoints admin
- no permisos en `AppTreeTable`
- no policy en `DocumentosWorkbench`

Documentar arquitectura, contrato, implementacion y pruebas en:

`docs/Architecture/AlineamientoContratoApiPermisosVisor/`
