# SCRUMCORE-207 — Spec (Print + Export)

## Alcance

Agregar soporte de:

- Print (plugin oficial EmbedPDF)
- Export/Download (plugin oficial EmbedPDF)

## Fuera de alcance

- Toolbar avanzada, menús, shortcuts
- Lógica custom de impresión/descarga
- Cambios en Workbench

## Requerimientos

### Dependencias

- Agregar deps:
  - `@embedpdf/plugin-print`
  - `@embedpdf/plugin-export`

### Registro obligatorio

Registrar en `src/app/Components/UI/AppVisorEmbedPdf/plugins/pluginRegistration.ts`:

- `createPluginRegistration(PrintPluginPackage)`
- `createPluginRegistration(ExportPluginPackage)`

Usar **packages React**:

- `@embedpdf/plugin-print/react`
- `@embedpdf/plugin-export/react`

### UI

En `src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.tsx`:

- Botón Print a la derecha
- Botón Export a la derecha
- `title` + `aria-label`
- Mantener `React.memo(AppPdfToolbar)`

### Encapsulamiento

- Workbench no cambia.
- No exponer engine/plugins en props públicas.

## Testing mínimo

Actualizar `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`:

- Render toolbar incluye botones `Print` y `Export`
- Click ejecuta `provides` cuando existe
- No crashea cuando `provides` es `null`

## Documentación enterprise

Actualizar/crear bajo:

`docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`

- `SCRUM-SCRUMCORE-207-Metadata.md`
- `SCRUM-SCRUMCORE-207-Objetivo-General.md`
- `SCRUM-SCRUMCORE-207-Responsabilidades-del-Componente.md`
- `SCRUM-SCRUMCORE-207-Arquitectura-Tecnica.md`
- `SCRUM-SCRUMCORE-207-Informacion-Tecnica-del-Componente.md`
- `SCRUM-SCRUMCORE-207-APIs-Utilizadas.md`
- `SCRUM-SCRUMCORE-207-Comportamiento-del-Componente.md`
- `SCRUM-SCRUMCORE-207-Testing-Enterprise.md`
- `SCRUM-SCRUMCORE-207-Evidencias-Tecnicas.md`

