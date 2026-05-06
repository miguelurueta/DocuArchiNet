# SCRUMCORE-204 — Objetivo general

## Qué se implementa

Actualización enterprise del componente `AppVisorEmbedPdf` para incluir:
- Toolbar desacoplada (presentacional)
- Zoom In / Zoom Out / Reset Zoom

## Problema que resuelve

Habilitar control de zoom de manera estándar y mantenible, usando el plugin oficial de EmbedPDF, sin filtrar lógica del engine/plugins hacia módulos consumidores.

## Alcance funcional

- Registrar `@embedpdf/plugin-zoom` vía `createPluginRegistration(ZoomPluginPackage)`.
- Integrar toolbar interna con API desacoplada (`AppPdfToolbarProps`).
- Mantener virtualización y lazy rendering existentes.

## Objetivo arquitectónico

- Mantener toda la lógica EmbedPDF encapsulada dentro de `AppVisorEmbedPdf`.
- Evitar rerenders masivos mediante memoización de toolbar (`React.memo`).

## Resultado esperado

- Toolbar renderiza correctamente y zoom funciona (plugin oficial).
- Sin warnings TS/React (Rules of Hooks).
- Workbench no recibe lógica de zoom/plugin.

