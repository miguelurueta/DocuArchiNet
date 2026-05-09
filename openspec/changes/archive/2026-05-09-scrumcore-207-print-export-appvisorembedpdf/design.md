# SCRUMCORE-207 — Design (AppVisorEmbedPdf: Print + Export)

## Objetivo

Extender `AppVisorEmbedPdf` agregando acciones **Print** y **Export/Download** usando plugins oficiales de EmbedPDF:

- `@embedpdf/plugin-print`
- `@embedpdf/plugin-export`

## Principios de arquitectura (obligatorio)

- `DocumentosWorkbench` NO conoce:
  - engine Pdfium/EmbedPDF
  - plugins EmbedPDF
  - estados internos del visor
- Toda lógica EmbedPDF permanece encapsulada en `AppVisorEmbedPdf`.
- `AppPdfToolbar` es 100% presentacional:
  - no importa hooks/plugins
  - solo recibe props y emite callbacks
- Sin wrappers innecesarios.

## Diseño UI

- Botones `Print` y `Export` ubicados en la **derecha** del toolbar.
- Iconografía: `@ant-design/icons` (ya usada por el toolbar).
- Tooltips (atributo `title`) y `aria-label` obligatorios.

## Integración técnica

### Registro de plugins

Registrar vía `createPluginRegistration(...)` en `plugins/pluginRegistration.ts`:

- `PrintPluginPackage` (React package)
- `ExportPluginPackage` (React package)

### Ejecución

- Obtener `activeDocumentId`.
- Instanciar hooks oficiales:
  - `usePrint(activeDocumentId)`
  - `useExport(activeDocumentId)`
- Exponer callbacks memoizados:
  - `onPrint() -> print.provides?.print()`
  - `onExport() -> export.provides?.download()`

### Resiliencia

- Si `provides` es `null`, el botón no debe crashear:
  - handler con guard clause
  - opcional: `disabled` cuando no hay provides

## Diagrama (Mermaid)

```mermaid
flowchart LR
  T[AppPdfToolbar] -->|onPrint| V[AppVisorEmbedPdf]
  T -->|onExport| V
  V --> P[usePrint(documentId)]
  V --> E[useExport(documentId)]
  P --> PP[PrintPluginPackage]
  E --> EP[ExportPluginPackage]
```

