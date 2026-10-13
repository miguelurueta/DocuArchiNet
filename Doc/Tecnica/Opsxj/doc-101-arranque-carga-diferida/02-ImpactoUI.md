# ARRANQUE-CARGA-DIFERIDA

- Ticket: DOC-101
- Cambio OpenSpec: doc-101-arranque-carga-diferida
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- Iframe oficial del visor, toolbar, dropdown de parámetros, canvas y menú contextual.
- Los botones opcionales se deshabilitan solo durante su primera carga y se restauran en `finally`.
- Los IDs, clases de icono, eventos, foco y layout no cambian.
- El primer clic contextual carga su plugin y vuelve a emitir el evento sobre el mismo canvas.

## Validacion visual

La matriz estática está automatizada. La validación visual real de dropdown, iconos y capacidades queda pendiente de autorización E2E; no se adjunta una captura simulada.
