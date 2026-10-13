# PLANIFICADOR-VIEWPORT

- Ticket: DOC-100
- Cambio OpenSpec: doc-100-planificador-viewport
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

Se conservan `setZoom`, `loadPages`, `scrollToPage`, `rotate`, callbacks e IDs de canvas. No cambian endpoint, payload, autenticación o esquema. El fallo saneado admite solo `code`, `source`, `type`, `fileBase` y `line`. El resultado admite `PASSED`, `PASSED_WITH_OBSERVATIONS` y `FAILED`; este último establece código de salida no cero.
