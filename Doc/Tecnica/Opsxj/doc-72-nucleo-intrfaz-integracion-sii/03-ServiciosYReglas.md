# NUCLEO-INTRFAZ-INTEGRACION-SII

- Ticket: DOC-72
- Cambio OpenSpec: doc-72-nucleo-intrfaz-integracion-sii
- Clasificación: cross_cutting

## Servicios y reglas

`importar-servicio-web-api.js` encapsula ocho operaciones ASMX. El registro normaliza identidades y falla cerrado sin seleccionar SII. Core admite únicamente transiciones declaradas y comparte una sola promesa de ejecución por intención. UI coordina presentación y solicitudes, sin implementar fases mutadoras ni AJAX directo.

Los códigos seguros distinguen proveedor no configurado, no migrado y no soportado. Una respuesta sin envelope `d` o con JSON inválido se rechaza como contrato inválido.
