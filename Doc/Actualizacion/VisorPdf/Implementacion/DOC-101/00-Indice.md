# DOC-101 — Arranque y carga diferida del visor PDF

## Objetivo y alcance

Reducir el trabajo anterior al primer canvas eliminando dependencias duplicadas o ejecutadas en un contexto incorrecto y trasladando capacidades opcionales a su primer uso. Se conservan WebForms, PDF.js 3.4.120 y las firmas públicas de `PDFjsViewer`; no se modifica descarga, persistencia, permisos, auditoría ni el planificador DOC-100.

## Componentes

- Página y coordinación: `pdfjs/pdf_anotate_view/index.html` y `script.js`.
- Visor legacy: `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`.
- Contratos y runner saneado: `tools/e2e/`.
- Recursos existentes: jQuery UI, contextMenu, Wacom, Print.js, Fabric y Prettify.

## Documentos

1. `01-Arquitectura.md`
2. `02-FlujoIntegracion.md`
3. `03-ContratoUploadYMapping.md`
4. `04-EstadosErroresYAntiregresion.md`
5. `05-PruebasEvidencia.md`
6. `06-Diagramas.md`
7. `07-Metadata.md`
