# E2E DOC-10 con Playwright

Esta suite verifica el ASMX `PreviewEnviarTarea` con sesiones Gestión reales y de solo lectura. La política moderna es oficial para todo contexto Workflow válido: las pruebas no habilitan, limitan ni validan pilotos, usuarios, grupos o gates.

Antes de una corrida real, leer [AGENT-RUNBOOK.md](AGENT-RUNBOOK.md). Los secretos solo se reciben desde el almacén de secretos o variables efímeras de proceso; nunca se versionan `.env`, cookies, capturas ni cadenas de conexión.

## Instalación local

```powershell
npm.cmd --prefix tools/e2e install
npm.cmd --prefix tools/e2e run install:browsers
```

Se puede usar `DOC10_E2E_BROWSER_CHANNEL=msedge` cuando Edge está administrado localmente.

## Pruebas de Preview

### Borde sin sesión

```powershell
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_TASK_ID = '123'
npm.cmd --prefix tools/e2e run test:anonymous
```

La respuesta debe rechazar el contexto anónimo y no incluir destinos.

### Sesión Gestión y contexto oficial

```powershell
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<cuenta-workflow-valida>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '123'
npm.cmd --prefix tools/e2e run test:session
```

La sesión usa `tests/support/authenticated-workflow-session.cjs`; ninguna suite debe copiar los selectores ni el postback de `gestor.aspx`.

### Dos contextos Workflow válidos

```powershell
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<cuenta-principal-valida>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_SECONDARY_USER = '<segunda-cuenta-valida>'
$env:DOC10_E2E_SECONDARY_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '123'
npm.cmd --prefix tools/e2e run test:contexts
```

Ambas cuentas deben resolver contexto Workflow. Que una tarea no esté asignada a la segunda cuenta es un resultado funcional y no una restricción de despliegue.

### E2E completa sin mutación

```powershell
$env:DOC10_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<cuenta-workflow-valida>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '123'
$env:DOC10_E2E_MYSQL_URL = 'mysql://usuario_solo_lectura:secreto@host/base'
$env:DOC10_E2E_AUDIT_SQL = 'SELECT COUNT(*) AS total FROM tabla_auditoria WHERE id_tarea = ?'
npm.cmd --prefix tools/e2e run test:e2e
```

La prueba ejecuta únicamente `SELECT` antes/después y guarda evidencia resumida sin secretos bajo `tools/e2e/artifacts/`. Si se espera un bloqueo funcional de una tarea conocida, use `DOC10_E2E_AUTHORIZED_EXPECTED_CODE`; no lo use para ocultar un fallo inesperado.

## Carga de Preview

`test:load` requiere autorización explícita de carga. Usa una cuenta con contexto Workflow válido, las mismas consultas de solo lectura y una sesión independiente por usuario virtual. No cambia el gate ni el estado de la tarea.

## DOC-11

Las pruebas de `EjecutarEnvioTarea` anónima y de validación no cambian estado. Ejecución y concurrencia sí requieren una tarea descartable nueva, autorización explícita, token/conector vigentes y consultas `SELECT` de control. Consulte el runbook antes de invocarlas.
