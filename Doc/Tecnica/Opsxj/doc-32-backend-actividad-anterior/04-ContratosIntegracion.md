# BACKEND-ACTIVIDAD-ANTERIOR

- Ticket: DOC-32
- Cambio OpenSpec: doc-32-backend-actividad-anterior
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

- `PreviewDevolverActividad(idTarea, termino?, cursor?, tamanoPagina?)` y `EjecutarDevolverActividad({ IdTarea, IdConector, TokenVersion })` se publican en el ASMX moderno con sesión habilitada.
- La sesión autenticada se vuelve a validar en servidor. El cliente no puede aportar actividad, usuario, grupo, Ruta, Flujo ni tipo de contexto.
- `IdConector` mantiene semánticas separadas: en Ruta identifica la arista autorizada de Ruta; en Flujo identifica el conector entrante del flujo. Nunca se interpreta entre contextos.
- No hay cambios de esquema, migraciones ni configuración. Las integraciones y contratos de envío existentes permanecen sin cambios.
- La respuesta contiene datos públicos mínimos, destino resumido cuando corresponde, paginación y token de versión; los errores funcionales no exponen detalles internos.
