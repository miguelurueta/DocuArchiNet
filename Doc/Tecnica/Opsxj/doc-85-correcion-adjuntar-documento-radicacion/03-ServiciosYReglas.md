# CORRECION-ADJUNTAR-DOCUMENTO-RADICACION

- Ticket: DOC-85
- Cambio OpenSpec: doc-85-correcion-adjuntar-documento-radicacion
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`ServicioAdjuntoRadicacion` coordina un `IContextoAdjuntoRadicacionRepository`, compara el radicado informativo y entrega un contexto inmutable a la preparación. El repository filtra por estado y usuario con parámetros. El constructor explícito no consulta `DAT_ADIC_TAR`. Detalle y diagrama: paquete documental canónico DOC-85.
