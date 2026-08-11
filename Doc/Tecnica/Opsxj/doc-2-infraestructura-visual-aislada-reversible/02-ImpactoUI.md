# DOC-2 — Contrato CSS e impacto visual

## Encapsulamiento y tokens

Toda regla comienza con `.workflow-centro-trabajo-moderno`. Los tokens son `--ctw-navy`, `--ctw-blue`, `--ctw-ink`, `--ctw-muted`, `--ctw-line`, `--ctw-pale`, `--ctw-bg`, `--ctw-danger`, `--ctw-warning`, `--ctw-radius` y `--ctw-control-height`.

Componentes entregados: `.ctw-btn`, `.ctw-btn--primary`, `.ctw-btn--danger`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__trigger`, `.ctw-menu__panel`, `.ctw-menu__item`, `.ctw-badge`, `.ctw-action-bar`, `.ctw-document-bar`, `.ctw-panel`, `.ctw-pane-head` y `.ctw-document-row--selected`.

## Capas y alcance

- `ctw-layer-layout`: panel y jerarquía base sin cambiar tamaño, posición o visibilidad.
- `ctw-layer-actions`: barra `#menucab` y sus dropdowns existentes.
- `ctw-layer-documents`: cabecera y fila activa de documentos relacionados.
- `ctw-layer-a11y`: foco visible, estado deshabilitado y botón de icono de 40px en móvil.

Los popovers usan `z-index: 1100`, por encima del contenido local sin alterar visor o modales legacy. El breakpoint es `767px`; ningún selector habilita una acción que el servidor haya ocultado.

## Evidencia visual pendiente

QA debe comparar modo apagado/encendido en 1366, 1024, 768 y 375 px, incluyendo hover, foco, deshabilitado, menú abierto y documento seleccionado. Requiere ambiente, piloto y datos de Workflow controlados.
