# AppEditorPdf

`AppEditorPdf` es el alias canonico de editor PDF en la capa shared UI.

## Ubicacion

- `src/app/Components/UI/AppEditorPdf/`

## Decision de arquitectura

- Reutiliza `AppEditor` como engine shared para evitar duplicacion.
- Mantiene contrato tipado compatible para modo controlado/no controlado.
- Permite evolucion incremental por tickets posteriores sin acoplar dominio.

