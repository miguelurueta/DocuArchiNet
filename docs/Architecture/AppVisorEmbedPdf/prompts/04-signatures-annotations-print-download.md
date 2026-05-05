# Prompt 04 — Signatures + Annotations + Print/Download enterprise

Objetivo: completar capacidades enterprise y persistencia.

## Alcance

- Signatures:
  - firma predeterminada desde BD
  - gestión de firmas (`AppPdfSignatureManager`)
  - persistencia en JSON
- Annotations:
  - persistencia en JSON
  - layers separados (annotations/signatures)
- Print/Download:
  - política por permisos/capabilities
  - auditoría/telemetría si aplica

## Criterios de aceptación

- Se pueden guardar/cargar anotaciones y firmas (JSON) sin perder estado.
- Print/Download respetan permisos del documento.

