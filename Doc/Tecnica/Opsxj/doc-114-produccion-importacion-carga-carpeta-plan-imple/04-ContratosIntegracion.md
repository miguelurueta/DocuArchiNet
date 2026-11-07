# DOC-114 — Preparación focal de planes

- Ticket: DOC-114
- Cambio OpenSpec: doc-114-produccion-importacion-carga-carpeta-plan-imple
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

PrepararPlan mantiene nivelRef/versionId/raices/archivos y devuelve planId/revision/planHash/expiraUtc/nivelRef/snapshot/resultado. Actor/ámbito y valores se resuelven en servidor. Campos desconocidos rechazados por PerfilImportacionJson.Leer emiten CONFIGURACION_INVALIDA/HTTP400; nivel/versión ajenos y permiso revocado emiten ACCESO_DENEGADO. El fixture se ajustó al contrato existente, sin cambiar el producto.

Los recursos de sesión son datos de prueba cerrados sobre perfiles base DOC-111, sin secretos. Selector, consola, proceso, sesión y DSN son compartidos. Las referencias/revisión/huellas se conservan antes de validar; recuperación por lectura mantiene planes11/22 sin repetir preparación. La suite distingue resumedPlans de solicitudes nuevas, guarda pruebas ID/huella y diagnóstico de rechazo sin mensajes privados. El diario propio de permiso quedó RESTORED. [Informe consolidado](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-final-report-20261103.md).
