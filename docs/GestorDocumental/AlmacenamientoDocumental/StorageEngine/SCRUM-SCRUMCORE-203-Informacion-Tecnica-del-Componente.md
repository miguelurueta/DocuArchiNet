# SCRUMCORE-203 — Información técnica del componente

## Identificación

- Nombre: `AppVisorEmbedPdf`
- Ruta: `src/app/Components/UI/AppVisorEmbedPdf/`

## Naming conventions

- Folder: `AppVisorEmbedPdf/`
- Entry: `AppVisorEmbedPdf.tsx`
- Export: `index.ts`
- CSS Modules: `*.module.css`

## Props públicas

```ts
export interface AppVisorEmbedPdfProps {
  fileUrl?: string;
  className?: string;
  style?: React.CSSProperties;
}
```

## Interfaces / Types

- Source of truth: `src/app/Components/UI/AppVisorEmbedPdf/types/`

## Configuración soportada

- `fileUrl`: ruta local (Vite) o URL externa accesible por el browser.
- Demo PDF:
  - Default: definido en `src/app/Components/UI/AppVisorEmbedPdf/hooks/useDemoPdfUrl.ts`
  - Override env: `VITE_EMBEDPDF_DEMO_PDF="/demo/archivo.pdf"`

## Responsive strategy

- El visor ocupa el contenedor disponible (width/height 100%).
- El layout final depende del consumer (quien define el espacio).

## Compatibilidad Design System

- Estilos via CSS Modules.
- Colores/fondo/bordes deben alinearse a tokens/estética del proyecto (sin cambiar lógica EmbedPDF).

## Dependencias utilizadas

- EmbedPDF + plugins base (encapsulados dentro del componente).

## Workers / WASM

- Pdfium Engine (detalle exacto depende de EmbedPDF/Pdfium).
- Documentar cualquier ajuste adicional si se introduce (fuera de alcance en este ticket).

## Lazy loading / Virtualización

- Virtualización: `Scroller` (`@embedpdf/plugin-scroll`).
- Lazy rendering: `RenderLayer` (`@embedpdf/plugin-render`).

## Memoization / Rendering strategy

- Evitar renders innecesarios.
- Evitar condicionar el orden de hooks (Rules of Hooks).
