# DOC-111: cuarta sesión detenida durante REC

## Decisión posterior del usuario

Corrección posterior implementada y desplegada: [sin dependencia de appSettings](doc111-correccion-sin-appsettings-20261030.md). El código ya no conserva el bloqueo descrito abajo; la evidencia inferior es histórica.

El usuario rechazó la solución de habilitar la carga mediante nuevas claves obligatorias en Web.config. La propuesta XML queda **descartada y no debe aplicarse**. Web.config permanece sin cambios. El diagnóstico inferior describe la dependencia de la implementación actual, no un requisito aprobado ni una solución vigente.

La revisión encontró que `ConfiguracionPerfilImportacion` ya persiste `Extensiones` y `MaximoBytesArchivo`, y el editor existente permite modificar este último. No se encontró en ese modelo una política completa de cuotas, TTL, retención y presupuestos técnicos. Antes de cambiar el lector hay que resolver esa ubicación y semántica con el usuario, reutilizando perfil/editor/repositorio y evitando sustituir la dependencia rechazada por valores inventados. No se cambió la implementación, el perfil publicado ni la BD durante esta revisión; tampoco se lanzó otra E2E.

## Evidencia de la sesión

Sesión 04, 21:32:30–21:33:59 UTC: `READONLY_RA`, `READONLY_DA` y apertura de sesión finalizaron sin excepción; el driver entró en REC. Terminó con `DOC111_OPERATION_NOT_CONFIRMED`, sin referencias de plan/lote y sin resultado PASS. Ese error se produce al recibir una respuesta JSON válida cuyo HTTP no es 200 o cuyo `ok` es falso. No identifica por sí solo la operación o el código público original.

El flujo previo a la primera referencia solo ejecuta `AbrirImportacion` y `PrepararPlan`; no alcanzó confirmación con referencia registrada. SELECT posterior, 21:35:33 UTC: cero planes/lotes nuevos para cuenta 141/empresa 2/versión 38 y cero coincidencias de los expedientes propuestos. No se repitió la carga.

## Bloqueo de configuración encontrado

Inspección restringida al appSettings local, sin mostrar conexiones: faltan las diez claves de límites generales, presupuestos PDF y consulta enumeradas en [propuesta local](doc111-appsettings-prueba-lote.xml). La fachada llama a `LimitesImportacionCarpetas.DesdeConfiguracion` antes del guard y la acción. Cuando faltan los siete límites generales en la configuración efectiva, devuelve `CAPACIDAD_NO_DISPONIBLE`; las claves PDF/consulta se exigen en etapas posteriores. El diseño deliberadamente no incluye valores habilitantes predeterminados.

La inspección de configuración efectiva mediante `WebConfigurationManager.OpenWebConfiguration` no completó y se interrumpió; no se obtuvo con ella evidencia adicional sobre configuración heredada. La ausencia local constituye un bloqueo comprobado de la preparación, pero la sesión no conservó la respuesta HTTP exacta y no se declara reconstruida. Se mejoró el diagnóstico futuro para conservar únicamente operación permitida, HTTP y código público de una lista cerrada, sin cuerpo, mensaje, token ni cookies. Suite focal del driver: 20/20 PASS.

La propuesta XML limita la prueba a dos raíces, dos PDF de hasta 1 MiB cada uno, 2 MiB por lote, manifiesto de 64 KiB, plan de 15 minutos, retención de staging de siete días, PDF de hasta 100 páginas/10000 objetos y páginas de consulta de 25. El tipo `LimitesImportacionCarpetas` de la DLL desplegada aceptó la política propuesta en memoria, sin HTTP ni efectos. Son valores propuestos para esta prueba; no se aplicaron ni se modificó Web.config. La limpieza sigue deshabilitada. Aplicar configuración reiniciará la aplicación IIS; antes de otra corrida debe verificarse la política resultante y la apertura operativa, sin repetir una carga a ciegas.
