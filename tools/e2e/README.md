# E2E DOC-10 con Playwright

Esta suite separa la llamada directa anónima del ASMX de la E2E autenticada. No crea cabeceras de bypass ni acepta identidad, grupo, ruta o permisos desde el cliente.

Para agentes que vayan a ejecutar una prueba real, leer primero [AGENT-RUNBOOK.md](AGENT-RUNBOOK.md). Contiene los límites de seguridad, el orden de pruebas y la restauración obligatoria del gate.

## Instalación local

```powershell
npm.cmd --prefix tools/e2e install
npm.cmd --prefix tools/e2e run install:browsers
```

Si el agente ya tiene Edge o Chrome administrado, se puede evitar la descarga indicando `DOC10_E2E_BROWSER_CHANNEL=msedge` o la ruta explícita en `DOC10_E2E_BROWSER_PATH`.

## Comprobación directa sin login

La prueba anónima solo verifica que el ASMX rechaza una solicitud sin sesión con `WORKFLOW_CONTEXT_INVALID` y sin destinos:

```powershell
$env:DOC10_E2E_BASE_URL = 'http://localhost/GestionDocumental-Docuarchi.net/'
$env:DOC10_E2E_TASK_ID = '1'
$env:DOC10_E2E_BROWSER_CHANNEL = 'msedge'
npm.cmd --prefix tools/e2e run test:anonymous
```

## E2E real autenticada

Defina secretos solo en el almacén seguro de CI o en las variables de la sesión. No versionar archivos `.env`, cookies, capturas ni cadenas de conexión.

```powershell
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<piloto>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_UNAUTHORIZED_USER = '<no-piloto>'
$env:DOC10_E2E_UNAUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '123'
$env:DOC10_E2E_MYSQL_URL = 'mysql://usuario_solo_lectura:secreto@host/base'
$env:DOC10_E2E_AUDIT_SQL = 'SELECT COUNT(*) AS total FROM tabla_auditoria WHERE id_tarea = ?'
npm.cmd --prefix tools/e2e run test:e2e
```

`DOC10_E2E_TASK_STATE_SQL` es opcional y debe ser un único `SELECT` con exactamente un parámetro `?`. La consulta de auditoría también es obligatoria, de solo lectura y debe tener el mismo parámetro. La evidencia segura se guarda por defecto en `tools/e2e/artifacts/doc10-preview-e2e.json`, ruta ignorada por Git. Si se configura `DOC10_E2E_EVIDENCE_PATH` con una ruta relativa, esta se resuelve desde la raíz del repositorio.

Si la tarea elegida está autorizada pero el preview debe devolver un bloqueo funcional conocido, configure opcionalmente `DOC10_E2E_AUTHORIZED_EXPECTED_CODE` con ese código. La prueba exigirá ese bloqueo y cero destinos, pero mantendrá las comparaciones antes/después de estado y auditoría.

## Verificación de sesión Gestión → Workflow

Para diagnosticar autenticación y bootstrap de sesión antes de una E2E completa, use solo el usuario piloto. La prueba hace el postback Web Forms y confirma que el ASMX ya no responde `WORKFLOW_CONTEXT_INVALID`; no escribe en Workflow ni requiere conexión MySQL de auditoría.

```powershell
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<piloto>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '123'
npm.cmd --prefix tools/e2e run test:session
```

Los comandos fallan antes de iniciar Playwright cuando falta una variable obligatoria; una E2E omitida no puede quedar aprobada en CI.

## Carga autenticada: 20 y 30 sesiones

`test:load` crea una sesión Gestión independiente por usuario virtual y, cuando todas están autenticadas, mide el POST del ASMX. El login se dosifica con cinco sesiones en paralelo de forma predeterminada; el ASMX recibe después todas las solicitudes del nivel a la vez. Por defecto ejecuta 20 y 30 sesiones, con una solicitud por sesión. Así se mide la concurrencia del preview y no la serialización de varias llamadas sobre una misma sesión ASP.NET.

Usa las mismas variables seguras de la E2E completa, excepto las del usuario no piloto. Las consultas de estado y auditoría siguen siendo obligatorias y de solo lectura. La evidencia guarda solo métricas, códigos funcionales públicos y huellas; no guarda credenciales, cookies, cadenas de conexión ni cuerpos de respuesta.

```powershell
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<piloto>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '123'
$env:DOC10_E2E_MYSQL_URL = 'mysql://usuario_solo_lectura:secreto@host/base'
$env:DOC10_E2E_AUDIT_SQL = 'SELECT COUNT(*) AS total FROM tabla_auditoria WHERE id_tarea = ?'
$env:DOC10_LOAD_CONCURRENCIES = '20,30' # opcional; valor por defecto
$env:DOC10_LOAD_LOGIN_CONCURRENCY = '5' # opcional; solo regula el bootstrap de sesiones
$env:DOC10_LOAD_EVIDENCE_PATH = 'Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/evidencias/qa-preview-load.json'
npm.cmd --prefix tools/e2e run test:load
```

`DOC10_LOAD_MAX_FAILURE_PERCENT` acepta como máximo porcentaje de fallos y su valor por defecto es `0`. `DOC10_LOAD_MAX_P95_MS` es opcional: permite que CI aplique un objetivo de latencia acordado sin fijarlo en el código. La prueba no cambia la implementación a asíncrona; sus métricas sirven para decidirlo con evidencia.

## Verificación de autorización de piloto

Esta prueba no consulta MySQL ni escribe evidencia. Requiere que el gate esté activo y acotado al piloto: confirma que el piloto supera el gate (aunque la tarea puede devolver un bloqueo funcional propio) y que otro usuario autenticado recibe `WORKFLOW_MODERN_INACTIVE` sin destinos. Para exigir destinos se necesita una tarea activa y disponible. Es el paso previo a `test:e2e`, que además verifica estado y auditoría antes/después.

```powershell
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<piloto>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_UNAUTHORIZED_USER = '<no-piloto>'
$env:DOC10_E2E_UNAUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '123'
npm.cmd --prefix tools/e2e run test:authorization
```
