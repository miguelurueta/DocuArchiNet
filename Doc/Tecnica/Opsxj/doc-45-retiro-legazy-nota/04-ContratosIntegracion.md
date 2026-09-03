# RETIRO-LEGAZY-NOTA — Contratos e integraciones

- Ticket: DOC-45
- Cambio OpenSpec: doc-45-retiro-legazy-nota
- Clasificacion: cross_cutting

## Contratos e integraciones

- Se conserva `WebServiceWorkflow.asmx` y `WebFormAnotacion.aspx` para Radicación y Correspondencia.
- El DTO moderno agrega la capacidad booleana `PuedeGestionar`; el cliente no suministra identidad para calcularla.
- `NotOwner` es un código funcional saneado para mutaciones no autorizadas por propiedad.
- El cliente se integra con `PageRequestManager.endRequest` y relee `Hidden_id_tarea_selecionada` tras postbacks parciales.
- No hay nuevo login, arnés Playwright, `.env`, endpoint, tabla ni migración.
- El rollback debe revertir coordinadamente backend, markup, JavaScript, CSS y pruebas; no debe restaurar una doble presentación.
