# LIBERACION-DEVOLVER-TAREA-ACTIVIDAD

- Ticket: DOC-35
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Clasificacion: cross_cutting

## Contratos e integraciones

No se agrega ni modifica endpoint, handler, payload, esquema o autenticación. PreviewDevolverActividad y EjecutarDevolverActividad conservan la terna mínima de tarea, conector y token, y reconstruyen el contexto de Ruta o Flujo en servidor.

La matriz de liberación no contiene secretos ni datos de conexión. La gestión de despliegue es una dependencia operativa externa: solo restaura un paquete previamente aprobado, no cambia contratos ni revierte transiciones confirmadas.
