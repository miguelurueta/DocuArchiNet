# BACKEND-CONTRATOS-NOTAS

- Ticket: DOC-40
- Cambio OpenSpec: doc-40-backend-contratos-notas
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

La fundación define seis contratos internos: listar y contar por `idTarea`; crear con `idTarea`, contenido e identificador de intención; consultar con `idTarea` e `idNota`; actualizar y eliminar con `idTarea`, `idNota` y versión. Sus respuestas son tipadas y devuelven datos autorizados o un código funcional seguro. La ruta Workflow no forma parte del payload: el gate obtiene `IdRutaWorkflow` y el puerto obtiene el `IdRuta` de la tarea autorizada.

No se publica ASMX, handler ni payload de navegador en DOC-40. Si una fase posterior expone transporte, este adaptará la sesión autenticada al gate y enviará solicitudes tipadas al servicio; la identidad, autor, grupo y permiso no se aceptarán desde el cliente. No hay cambio de autenticación, esquema, migración, integración externa ni compatibilidad pública que desplegar ahora.

El acceso de infraestructura adoptará el patrón Workflow con parámetros ADO.NET. Si una consulta necesita metadatos de ruta, parte de `rutas_workflow` mediante `@idRuta`; los nombres técnicos derivados solo proceden de esa resolución confiable y superan validación de identificador. `Class_anotacion_tarea` continúa como integración legacy aislada y no participa en el contrato moderno.

La siguiente integración está diseñada en `Prompt/02-lectura-listado-y-contador.md`: expone solamente listar, consultar y contar en un cambio propio, reutiliza estos contratos y sitúa cualquier ASMX en `webservice/`. El listado usa página inicial 50, máximo 100, cursor ligado al contexto y orden estable; el contador es `COUNT(*)` y se refresca por evento o cambio de tarea. Su E2E se integra al mismo cambio, reutiliza `tools/e2e` y requiere autorización expresa de ambiente, cuentas y tarea descartable. DOC-40 no crea endpoint ni E2E.

Una exposición posterior de escritura requiere primero el preflight de almacenamiento descrito en `03-ServiciosYReglas.md`. Los contratos ya contienen `Version` como cadena para el ETag SHA-256 e `IdSolicitudCliente` para el UUID idempotente; el cliente no aporta autor, actividad, versión calculada, ruta ni datos de auditoría.
