# E2E Workflow DOC-10, DOC-11 y DOC-28 con Playwright

Esta suite verifica el ASMX `PreviewEnviarTarea` con sesiones Gestión reales y de solo lectura. La política moderna es oficial para todo contexto Workflow válido: las pruebas no habilitan, limitan ni validan pilotos, usuarios, grupos o gates.

Antes de una corrida real, leer [AGENT-RUNBOOK.md](AGENT-RUNBOOK.md). Los secretos solo se reciben de forma efímera durante la corrida; nunca se versionan `.env`, cookies, capturas ni cadenas de conexión.

## Instalación local

```powershell
npm.cmd --prefix tools/e2e install
npm.cmd --prefix tools/e2e run install:browsers
```

Se puede usar `DOC10_E2E_BROWSER_CHANNEL=msedge` cuando Edge está administrado localmente. DOC-28 usa sus propias variables `DOC28_E2E_BROWSER_CHANNEL` o `DOC28_E2E_BROWSER_PATH`.

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

## DOC-28 — envío a usuario

DOC-28 prueba los contratos `PreviewEnviarUsuario` y `EjecutarEnvioUsuario`. No reutiliza las variables ni los resultados DOC-10/11: el destino y token de ejecución se toman exclusivamente del preview actual para evitar datos vencidos o configurados a mano.

Antes de toda corrida, lea [AGENT-RUNBOOK.md](AGENT-RUNBOOK.md). Las credenciales, la URL y la conexión de solo lectura se reciben únicamente mediante secretos efímeros de proceso. No guarde `.env`, cookies, trazas, capturas ni evidencias con cuerpos de respuesta.

### Borde anónimo

```powershell
$env:DOC28_E2E_BASE_URL = 'https://qa.example/app/'
npm.cmd --prefix tools/e2e run test:doc28:anonymous
```

El ASMX debe devolver `WORKFLOW_CONTEXT_INVALID` y una lista de destinos vacía.

### Validación con sesión Gestión

```powershell
$env:DOC28_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC28_E2E_MODULE = 'GESTOR'
$env:DOC28_E2E_AUTHORIZED_USER = '<cuenta-con-CAMBIO_USUARIO>'
$env:DOC28_E2E_AUTHORIZED_PASSWORD = '<secreto-efimero>'
npm.cmd --prefix tools/e2e run test:doc28:validation
```

La cuenta debe poseer `CAMBIO_USUARIO`. La prueba envía una tarea inválida y espera `WORKFLOW_TASK_INVALID`, sin transición.

### Preview E2E de solo lectura

Esta corrida requiere un ambiente de pruebas autorizado, una tarea activa y una cuenta MySQL solo lectura. Ambas consultas de control deben ser una única sentencia `SELECT` con exactamente un parámetro `?` para la tarea.

```powershell
$env:DOC28_E2E_BASE_URL = 'https://qa.example/app/'
$env:DOC28_E2E_MODULE = 'GESTOR'
$env:DOC28_E2E_AUTHORIZED_USER = '<cuenta-con-CAMBIO_USUARIO>'
$env:DOC28_E2E_AUTHORIZED_PASSWORD = '<secreto-efimero>'
$env:DOC28_E2E_TASK_ID = '<tarea-activa-autorizada>'
$env:DOC28_E2E_PAGE_SIZE = '1'
$env:DOC28_E2E_EXPECT_PAGINATION = 'true'
$env:DOC28_E2E_MYSQL_URL = 'mysql://usuario_solo_lectura:secreto@host/base'
$env:DOC28_E2E_TASK_STATE_SQL = 'SELECT ID_ESTADO FROM estados_tarea_workflow WHERE INICIO_TAREAS_WORKFLOW_ID_TAREA = ?'
$env:DOC28_E2E_AUDIT_SQL = "SELECT Usuario_Workflow_idU_suario, Fecha_Inicio_Seccion, Valor_Log FROM log_usuario WHERE INSTR(Valor_Log, CONCAT('Tarea=', ?, CHAR(59))) > 0 AND INSTR(Valor_Log, 'Mecanismo=ASMX_ENVIO_USUARIO') > 0 ORDER BY Fecha_Inicio_Seccion"
npm.cmd --prefix tools/e2e run test:doc28:preview
```

La tarea debe tener al menos dos destinos cuando `DOC28_E2E_EXPECT_PAGINATION=true`. El resultado compara las huellas de estado y auditoría antes/después; deben ser iguales. Para un bloqueo funcional conocido, defina `DOC28_E2E_PREVIEW_EXPECTED_CODE` y no use esa variable para ocultar fallos inesperados.

### Ejecución sobre tarea descartable

`EjecutarEnvioUsuario` cambia Workflow. Solo se permite después de autorización explícita para el ambiente, la cuenta y una tarea descartable; la ejecución no se activa con los comandos de preview.

Use las mismas variables del preview y agregue:

```powershell
$env:DOC28_E2E_EXECUTION_AUTHORIZED = 'true'
$env:DOC28_E2E_EXPECTED_OUTCOME = 'success'
npm.cmd --prefix tools/e2e run test:doc28:execute
```

Para un escenario que debe bloquearse, use `DOC28_E2E_EXPECTED_OUTCOME='blocked'` y `DOC28_E2E_EXPECTED_CODE='<codigo-funcional-esperado>'`. El harness obtiene el usuario destino, actividad y token del preview recién realizado; nunca los recibe por variables de entorno.

### Concurrencia controlada de dos solicitudes

`test:doc28:concurrency` no es una prueba de carga: ejecuta exactamente dos solicitudes simultáneas de `EjecutarEnvioUsuario` sobre el mismo payload derivado de un preview vigente. Requiere una autorización adicional y una tarea descartable distinta de cualquier caso que deba conservarse.

Use las variables de preview y agregue:

```powershell
$env:DOC28_E2E_EXECUTION_AUTHORIZED = 'true'
$env:DOC28_E2E_CONCURRENCY_AUTHORIZED = 'true'
npm.cmd --prefix tools/e2e run test:doc28:concurrency
```

El runner crea dos sesiones autenticadas, espera una única ganadora con estado `completada` y una perdedora con `WORKFLOW_TRANSITION_IN_PROGRESS`, `WORKFLOW_VERSION_CONFLICT` o `WORKFLOW_TASK_UNAVAILABLE`. Deben cambiar las huellas de estado y auditoría. No acepta niveles configurables, usuarios virtuales ni un comando de carga masiva.

Para DOC-28, la consulta de auditoría debe leer `log_usuario` y filtrar `Mecanismo=ASMX_ENVIO_USUARIO` y la tarea con su único parámetro `?`, como en el ejemplo anterior. No use `wf_log_estados_workflow`: registra el motor histórico, pero no la auditoría adicional del endpoint.

La evidencia se escribe en `tools/e2e/artifacts/` y solo conserva endpoint, resultado, códigos, conteos y huellas. Antes y después de cualquier corrida real, compruebe que el gate sigue en `false` y que sus listas están vacías, y confirme que las páginas legacy no cambiaron según el runbook.

## Orquestador reutilizable de Workflow

`test:workflow:run` ejecuta una secuencia configurada por DOC con un único perfil no sensible y una única captura interactiva de secretos. DOC-32 es el primer consumidor. Consulte el runbook para crear el perfil externo, obtener autorización y ejecutar la secuencia; el perfil puede contener solo el nombre de un DSN ODBC, nunca cuentas, contraseñas ni URL MySQL. `previewActivityNames` obliga a que el preview devuelva exactamente las actividades declaradas. Para una devolución con varios resultados, `executionActivityName` fija la única actividad elegida; el conector y token siguen siendo derivados del preview vigente. `executionFinalActivityName` comprueba por ODBC el nombre de la actividad activa (`listado_actividades_workflow.NOMBRE_ACTIVIDAD`) tras la transición: no es el nombre del grupo ni del usuario asignado. Puede diferir de esa etiqueta de selección solo si la transición crea otra actividad efectiva. `concurrencyActivityName` fija de la misma forma la actividad de la carrera y evita depender del orden del preview.

### Ciclo reutilizable de recursos E2E

Las etapas mutantes usan un contrato registrado de recursos: preflight de solo lectura, reserva exclusiva, ejecución y cierre. El núcleo no conoce tareas, actividades ni consultas Workflow; cada DOC o tipo de prueba aporta un adaptador registrado y un descriptor no sensible. Un perfil no puede escoger proveedor, SQL, scripts, rutas ni comandos.

La reserva local evita que dos corridas desde el mismo espacio de trabajo reutilicen el recurso. Un ambiente que necesite exclusión entre equipos debe registrar un coordinador compartido; si no existe, la prueba mutante falla antes de iniciar. Tras una transición efectiva el recurso queda consumido hasta que el preflight observa una nueva generación preparada por el responsable del ambiente. La plataforma nunca intenta revertir datos de negocio de forma genérica.
