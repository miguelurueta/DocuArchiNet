# SCRUMCORE-204 — Responsabilidades del componente

## Componente

- Nombre: `AppVisorEmbedPdf`
- Ruta: `src/app/Components/UI/AppVisorEmbedPdf/`

## Nuevas responsabilidades (esta fase)

- Registrar plugin oficial: `@embedpdf/plugin-zoom`.
- Exponer controles de zoom vía toolbar interna desacoplada.
- Mantener sincronización zoom ↔ viewport usando capacidades nativas del plugin.

## Qué encapsula

- Engine, plugins, capabilities y estado de zoom (interno al visor).

## Qué NO debe hacer

- No exponer engine/plugins/capabilities al consumer.
- No implementar zoom “manual” fuera del plugin oficial.

## Responsabilidades del consumer

- Ninguna relacionada a zoom: el consumer solo monta `<AppVisorEmbedPdf />`.

