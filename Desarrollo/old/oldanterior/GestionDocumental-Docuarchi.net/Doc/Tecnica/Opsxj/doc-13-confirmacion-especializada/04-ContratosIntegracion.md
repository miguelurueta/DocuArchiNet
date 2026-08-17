# CONFIRMACION-ESPECIALIZADA

- Ticket: DOC-13
- Cambio OpenSpec: doc-13-confirmacion-especializada
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

El adaptador de navegador invoca `POST ../webservice/WebServiceWorkflowModern.asmx/EjecutarEnvioTarea`. El payload JSON contiene `idTarea`, `idConector` y `tokenVersion`, obtenidos de la selección que entregó el preview. La sesión HTTP del usuario se mantiene en el servicio ASMX; el cliente no agrega credenciales ni cabeceras de autenticación propias.

La respuesta se normaliza desde el envoltorio ASMX `d` a un resultado con éxito, mensaje funcional, código de bloqueo, advertencias, posibilidad de reintento y versión. Los valores ausentes se tratan de forma defensiva y no se renderizan como datos inventados.

No se altera el esquema de base de datos ni se publica un endpoint nuevo. La compatibilidad se preserva porque el flujo legacy continúa activo cuando el gate moderno está deshabilitado y porque la ejecución reutiliza el endpoint de transición existente.
