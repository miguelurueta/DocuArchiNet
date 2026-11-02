# DOC-111: controles registrados en lugar de URLs MySQL

Se corrigió la solicitud de conexiones completas que el usuario reportó en el TTY. La segunda sesión terminó en `READONLY_RA` con `DOC111_REC_FAILED`, antes del login HTTP. Ese código genérico no permite atribuir el fallo exacto a una entrada; no se conservaron las entradas privadas.

## Cambio

El perfil declara ahora `odbcDsn` y `docuarchiOdbcDsn`, como los escenarios existentes. La inspección de solo lectura del catálogo y del registro ODBC confirmó que **MembershipUsers** coincide con los destinos RA y DA del ambiente histórico; ambos DSN del perfil runtime local usan ese nombre. No se imprimieron servidor, base ni credenciales. Los DSN ilustrativos del ejemplo no se usan como destinos reales.

La consola solicita cuenta y contraseña de aplicación, y cuenta y contraseña MySQL exclusivamente de lectura. No solicita URLs MySQL. No toma automáticamente las credenciales privilegiadas del sitio como cuenta de controles.

La suite reutiliza `doc32-e2e-odbc.cjs` y el script PowerShell existente, extendidos con `queryRows`/modo `Rows` para los SELECT parametrizados que requieren los controles DOC-111. El modo predeterminado de huellas conserva el parámetro de tarea. Se rechazan SQL mutador, sentencias múltiples, bloqueos, funciones de espera y parámetros discordantes. Las filas se mantienen en memoria mediante IPC, sin evidencia de filas o mensajes de conexión. Los errores conservan únicamente etapa y código fijo; no se transforman en PASS. No hay login ni transporte de base de datos paralelos.

## Verificación

### Corrección posterior del puente PowerShell

La tercera sesión volvió a detenerse en READONLY_RA con `DOC111_SELECT_FAILED`, antes del login HTTP, sin referencias ni resultado REC. Se reprodujo un defecto del modo Rows en PowerShell 5: `@(ConvertFrom-Json ...)` producía un array exterior cuyo primer elemento era otro array; la validación lo rechazaba como parámetro no escalar. Se corrigió asignando directamente el array deserializado. No se atribuye este fallo a las credenciales introducidas por el usuario.

Una prueba técnica con el puente productivo corregido y un SELECT de literal pasó apertura, enlace del parámetro y materialización de fila. Usó únicamente en memoria la conexión configurada del ambiente para diagnosticar el mecanismo; no se usó como sustituto de la cuenta exclusivamente SELECT del operador ni como evidencia de aceptación REC. No se imprimieron filas o secretos. La validación con la cuenta de lectura del operador sigue pendiente.

Se añadieron códigos cerrados para distinguir entrada, apertura, autenticación, DSN/controlador, consulta y lectura, sin mensajes originales de conexión. Nueva prueba con PowerShell real y DSN deliberadamente inexistente comprueba que arrays vacíos, un escalar, null y varios parámetros llegan a la etapa de apertura y no fallan en deserialización, sin conexión de red. Suite ampliada: **49/49 PASS** con regresiones DOC-32/36/37/Notas. El fallo previo había pasado pruebas con dobles; esta prueba ejecuta ahora el script real y cubre ese límite.

- 47/47 pruebas Node PASS: driver DOC-111, controles ODBC nuevos y regresiones de consumidores DOC-32/36/37/Notas.
- Parseo PowerShell y sintaxis de la suite: PASS.
- Perfil runtime LOTE, `--check`: PASS, `REAL_EXECUTION=false`.
- Listado Playwright: tres escenarios, sin ejecución real.
- SELECT posterior, 21:27:09 UTC: cero planes/lotes nuevos de la cuenta y versión previstas; cero coincidencias de nombres/códigos de los dos expedientes propuestos.

La conexión ODBC real con la cuenta de lectura suministrada por el operador no se da por probada con los dobles. La aplicación desplegada no cambió; solo se corrigieron las herramientas de prueba. Los manifiestos históricos no identifican estas nuevas versiones del driver: la suite registrará su huella actual si la aceptación termina correctamente. No se repitieron mutaciones ni se marca aceptación REC en este documento.
