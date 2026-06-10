# SCRUMCORE-206 — Objetivo general (AppVisorEmbedPdf)

## Objetivo
Extender `AppVisorEmbedPdf` con rotación (plugin oficial EmbedPDF) manteniendo el visor estable. Se incorporan ajustes de UX para evitar “jump” en zoom bajo ciertas rotaciones.

## Alcance implementado
- Rotación izquierda/derecha vía `@embedpdf/plugin-rotate` (sin exponer engine al consumidor).
- Zoom estable garantizado: el zoom se mantiene habilitado solo cuando `rotation === 0` (evita jump en 90°/180°/270°).
- Toolbar presentacional con iconografía Ant Design (`@ant-design/icons`).
- FAB “Ir arriba” tipo WhatsApp (aparece al scrollear y redirige arriba).

## Fuera de alcance
- Resolver “jump” de zoom en 90°/270° vía parches profundos de plugins (se optó por guardrail UX).

Extender `AppVisorEmbedPdf` para soportar rotación usando exclusivamente el plugin oficial `@embedpdf/plugin-rotate`, reutilizando el toolbar existente y manteniendo virtualización/render.
