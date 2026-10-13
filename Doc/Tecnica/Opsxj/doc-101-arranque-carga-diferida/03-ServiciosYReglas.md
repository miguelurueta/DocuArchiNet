# ARRANQUE-CARGA-DIFERIDA

- Ticket: DOC-101
- Cambio OpenSpec: doc-101-arranque-carga-diferida
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

No se modifica VB.NET/C# ni se crean servicios. La consulta existente `service_solicita_datos_configuracion_itex_stamp_user` pasa de ejecutarse al abrir a ejecutarse una vez antes de firma/grafo y se memoiza durante la sesión del iframe.

Wacom e impresión comprueban permisos antes de descargar recursos. Un fallo opcional alerta, conserva el PDF y habilita reintento. El registro normal de visualización existente no cambia.
