# DOC-110: comprobación de guard HTTP del catálogo de fuentes

Estado vigente: **HTTP autenticado PASS, 17 rechazos comprobados**, 1 passed (17.9 s), 2026-10-27. La evidencia de preparación y NO_EJECUTADO siguientes se conserva como historial; resultado final al final. 15.8 completa; 15.14/7.8 mantienen criterios restantes abiertos.

Fecha: 2026-10-27. Avance de 15.8/15.14 y 7.8, D-05/D-09 y RQ-05/RQ-12. HTTP real nuevo pendiente.

## Escenario preparado

Modo --source-guards del runner y suite de sesión existentes. Reutiliza ambiente/cuenta, login compartido, token efímero, contexto anónimo separado y safePost sin reintentos. Exige reporte PASS de la aceptación de fuentes para la tabla elegida y el ID técnico de ese fixture. Lee la fuente y su historial; exige inactiva y revisión mayor que uno. No concede/revoca permisos ni requiere otra cuenta.

Para CrearFuente/GuardarFuente/ActivarFuente/RevocarFuente prueba token ausente, token inválido y contexto sin sesión aunque lleve token autenticado. Exige respectivamente 403 SOLICITUD_NO_VERIFICADA y 401 SESION_REQUERIDA, ok=false y datos=null. Después de cada rechazo compara documento completo e historial contra el estado inicial.

Como protección adicional del escenario, las tres operaciones de modificación siempre envían revisión antigua y el alta conserva FuenteId/revisión no inicial: aun con un fallo de CSRF, no se envía una solicitud de dominio válida que deba escribir. Con token válido se exige 409 CONFLICTO_REVISION en las modificaciones y 400 SOLICITUD_INVALIDA en CrearFuente. Estos controles diferencian el rechazo de transporte del rechazo de dominio. No se afirma que el caso de alta use una configuración nueva completa: el alta válida ya fue aceptada por UI en 15.16.

Un último intento con token válido añade UsuarioRef/AmbitoRef al contrato de GuardarFuente. Exige 400 CONFIGURACION_INVALIDA por contrato cerrado y documento/historial intactos; los campos del navegador no se usan como autoridad servidor. Total: **17 rechazos** con lectura posterior. Ningún intento repite automáticamente una mutación. El soporte no consulta filas DA.

## Validación local

```powershell
node --test tools/e2e/tests/doc110-source-guards.test.cjs tests/doc110-perfil-importacion-behavior.test.cjs
```

**3 PASS**, incluido arnés .NET Framework. Dos pruebas del soporte verifican la matriz completa, revisión antigua/ID preservado, lecturas posteriores y detección de efectos, historial alterado, código equivocado, éxito inesperado o datos filtrados. Son dobles explícitos, no prueba de 17 rechazos reales IIS.

Sintaxis Node PASS para soporte, runner y suite de sesión. OpenSpec estricto y diff de espacios comprobados. No se cambia código productivo, SQL, permisos ni configuración global.

## Corrida real pendiente

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --source-guards --source-table imp_02_creditos --headed
```

Solicita solo credenciales por consola. El fixture de fuente permanece revocado; no hace falta activar nada ni preparar otra tabla. Reporte saneado `doc110-source-guards-result.json`, con estado, conteo y fechas UTC, sin payload, claves de negocio, cookies, tokens ni credenciales.

Estado **NO_EJECUTADO** hasta recibir resultado. Las tareas 15.8/15.14/7.8 siguen abiertas. No acredita sesión expirada temporalmente, token de otra sesión autenticada, revocación externa de permiso, aislamiento empresarial ni concurrencia de fuentes. El contexto anónimo se identifica como ausencia de sesión, sin llamarlo expiración real.

## Resultado autenticado aprobado, 2026-10-27

El usuario aporta **1 passed (17.9 s)**, caso **16.6 s**, ejecutando --source-guards --source-table imp_02_creditos --headed. El reporte local confirma PASS y checks=17; inicio `2026-10-27T12:13:23.650Z`, fin `2026-10-27T12:13:24.162Z`.

CrearFuente/GuardarFuente/ActivarFuente/RevocarFuente rechazan token ausente/inválido con 403 SOLICITUD_NO_VERIFICADA y contexto sin sesión con 401 SESION_REQUERIDA. Con token válido, el alta con ID/revisión no inicial retorna 400 SOLICITUD_INVALIDA; las tres modificaciones con revisión antigua retornan 409 CONFLICTO_REVISION. La identidad/ámbito inyectados en GuardarFuente retornan 400 CONFIGURACION_INVALIDA por esquema cerrado. En cada caso se exigen ok=false y datos=null, y se releen fuente e historial íntegros para compararlos contra el estado inicial.

Fuente confirmada inactiva, sin cambios a revisión, columnas o auditoría. No se crea otra fuente, no se activan consumidores ni se modifica DA, permisos o configuración. El caso de alta es deliberadamente inválido en dominio; la creación válida desde UI quedó acreditada previamente en 15.16.

El conjunto completa 15.8: operaciones HTTP integradas y positivas del catálogo en 15.16, autoridad/contexto/contrato cerrado del servidor, guard y revisión observados aquí, más arnés .NET de denegación sin facultad antes de I/O (repositorio, servicio y descubrimiento de metadatos). La denegación de la facultad separada se distingue como prueba local; esta corrida no usa otra cuenta autenticada sin permiso de fuentes ni altera su concesión para probarla.

Se mantienen pendientes en sus tareas las pruebas reales de aislamiento empresarial, concurrencia de fuentes, revocación externa de permisos y expiración temporal de sesión. Los resultados de contexto anónimo no se atribuyen a expiración, ni estos conflictos secuenciales a una carrera entre conexiones. No se ejecutaron SELECT adicionales en esta sesión; estado persistido observado por lecturas ASMX.
