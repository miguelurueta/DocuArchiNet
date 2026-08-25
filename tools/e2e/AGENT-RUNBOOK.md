# Runbook para agentes — E2E y concurrencia DOC-10 / DOC-11 / DOC-28 / DOC-32

Este runbook permite reutilizar las pruebas ASMX Workflow sin copiar credenciales, modificar datos sin autorización ni reactivar la política legacy.

## Límites obligatorios

- Ejecutar solo contra un ambiente de pruebas autorizado; nunca inferir autorización para producción.
- Recibir URL, cuentas y acceso MySQL de solo lectura mediante secretos del entorno o instrucción expresa. No crear `.env` ni mostrar secretos, cookies o cadenas de conexión.
- Reutilizar `tests/support/authenticated-workflow-session.cjs` para todo login E2E DOC-10/DOC-11/DOC-28/DOC-32. No enviar usuario, grupo, ruta, actividad ni permisos al ASMX salvo el destino que DOC-28 obtiene del preview actual al ejecutar.
- `PreviewEnviarTarea` y `PreviewEnviarUsuario` son de solo lectura. Las consultas de control son una única sentencia `SELECT` con exactamente un parámetro `?` para la tarea.
- No modificar el flujo legacy. Al cierre, `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb` no deben tener cambios.
- No activar, editar ni limitar `WorkflowCentroTrabajoModernActive`, usuarios o grupos: la experiencia moderna es oficial para todo contexto Workflow válido.

## Preparación

```powershell
npm.cmd --prefix tools/e2e install
npm.cmd --prefix tools/e2e run install:browsers
```

Las variables de sesión se entregan por secretos efímeros:

```powershell
$env:DOC10_E2E_BASE_URL = 'https://ambiente-pruebas/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<cuenta-workflow-valida>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '<tarea-activa-autorizada>'
```

Para huellas antes/después, agregar únicamente una cuenta MySQL de lectura y una consulta de auditoría aprobada:

```powershell
$env:DOC10_E2E_MYSQL_URL = 'mysql://usuario_solo_lectura:secreto@host/base'
$env:DOC10_E2E_AUDIT_SQL = 'SELECT COUNT(*) AS total FROM tabla_auditoria WHERE id_tarea = ?'
```

`DOC10_E2E_TASK_STATE_SQL` es opcional y también debe ser un `SELECT` con un único `?`.

## Estado del gate

Al inicio y cierre se comprueba que la configuración permanece apagada y sin alcance. Es un control de integridad, no un mecanismo de habilitación:

```powershell
rg -n "WorkflowCentroTrabajoModern(Active|Users|Groups)" Web.config
```

El resultado esperado es `Active=false` y listas vacías. Si difiere, detener la corrida y solicitar intervención del responsable del ambiente; nunca corregirlo desde la prueba.

## Selección de prueba

| Objetivo | Comando | Requisitos adicionales | Resultado esperado |
| --- | --- | --- | --- |
| Borde sin sesión | `npm.cmd --prefix tools/e2e run test:anonymous` | URL y tarea. | Contexto anónimo rechazado, sin destinos. |
| Sesión Gestión → Workflow | `npm.cmd --prefix tools/e2e run test:session` | Cuenta Workflow válida. | Contexto resuelto sin bloqueo de despliegue. |
| Dos contextos oficiales | `npm.cmd --prefix tools/e2e run test:contexts` | Agregar segunda cuenta Workflow válida. | Ambas sesiones resuelven contexto; la disponibilidad es una regla de negocio. |
| E2E funcional completa | `npm.cmd --prefix tools/e2e run test:e2e` | MySQL de solo lectura y auditoría. | Destinos o bloqueo funcional esperado; huellas iguales antes/después. |
| Concurrencia ASMX | `npm.cmd --prefix tools/e2e run test:load` | Autorización específica de carga y MySQL de solo lectura. | Métricas y huellas iguales antes/después. |
| DOC-28 anónimo | `npm.cmd --prefix tools/e2e run test:doc28:anonymous` | URL autorizada. | Contexto rechazado, sin destinos usuario. |
| DOC-28 validación | `npm.cmd --prefix tools/e2e run test:doc28:validation` | Cuenta Gestión con `CAMBIO_USUARIO`. | Parámetro inválido bloqueado sin transición. |
| DOC-28 preview | `npm.cmd --prefix tools/e2e run test:doc28:preview` | Ambiente/cuenta autorizados, tarea activa y MySQL solo lectura. | Preview usuario sin cambios de estado/auditoría. |
| DOC-28 ejecución | `npm.cmd --prefix tools/e2e run test:doc28:execute` | Autorización explícita, tarea descartable, MySQL solo lectura y `DOC28_E2E_EXECUTION_AUTHORIZED=true`. | Resultado y huellas esperadas, sin tocar gate/legacy. |
| DOC-28 concurrencia | `npm.cmd --prefix tools/e2e run test:doc28:concurrency` | Autorización doble, tarea descartable y MySQL solo lectura. | Dos solicitudes: una `completada` y un bloqueo seguro; estado/auditoría cambian. |
| DOC-29 bloqueo UI | `npm.cmd --prefix tools/e2e run test:doc29:user-send-lock` | Autorización explícita doble, tarea descartable activa en la sesión, MySQL solo lectura, `DOC28_E2E_EXECUTION_AUTHORIZED=true` y `DOC29_E2E_UI_LOCK_AUTHORIZED=true`. | Un solo envío real; X, cancelar, fondo, Escape, API y recarga quedan bloqueados mientras se retiene la respuesta, y se valida el cambio de estado/auditoría. |
| DOC-32 preview | `npm.cmd --prefix tools/e2e run test:doc32:preview` | El comando solicita ambiente/cuenta autorizados, tarea descartable, MySQL solo lectura y presupuesto. | Estado y auditoría sin cambios; evidencia saneada. |
| DOC-32 ejecución | `npm.cmd --prefix tools/e2e run test:doc32:execute` | El comando solicita autorización explícita, primera tarea descartable, MySQL solo lectura y presupuesto. | Ejecución E2E oficial: una devolución real desde conector/token del preview actual. |
| DOC-32 concurrencia | `npm.cmd --prefix tools/e2e run test:doc32:concurrency` | El comando solicita autorizaciones explícitas, segunda tarea descartable, MySQL solo lectura y presupuesto. | Ejecución E2E oficial: dos solicitudes, una transición y un bloqueo seguro. |

## Cierre de cada corrida

1. Conservar solo evidencia resumida sin secretos ni cuerpos de respuesta.
2. Registrar ambiente, tipo de tarea, resultado y huellas cuando la evidencia pertenezca al cambio en curso.
3. Confirmar que el gate siguió apagado y las listas vacías con el comando anterior.
4. Ejecutar `git diff --name-only -- workflow/Webworkflow.aspx workflow/Webworkflow.aspx.vb`; no debe producir salida.
5. No detener procesos residuales sin autorización explícita.

## DOC-11 mutante

`EjecutarEnvioTarea` cambia la tarea. Solo puede ejecutarse con autorización explícita para una tarea descartable, cuenta válida, conector y token obtenidos del preview actual, además de consultas de estado y auditoría `SELECT` con un parámetro `?` y MySQL de solo lectura.

Las pruebas anónima y de validación no cambian estado. Ejecución y concurrencia requieren además `DOC11_E2E_EXECUTION_AUTHORIZED=true`; después se confirma que la configuración del gate continuó apagada y el flujo legacy no cambió.

## DOC-28 envío a usuario

`PreviewEnviarUsuario` recibe siempre tarea, filtro, cursor y tamaño de página. En el preview completo DOC-28 se usan `DOC28_E2E_TASK_STATE_SQL` y `DOC28_E2E_AUDIT_SQL`; ambas son obligatorias y el harness rechaza cualquier instrucción distinta de un `SELECT` con un único `?`.

`EjecutarEnvioUsuario` es mutante. Además de los límites generales, exige autorización explícita para el ambiente y cuentas, una tarea descartable, `DOC28_E2E_EXECUTION_AUTHORIZED=true`, resultado esperado y acceso MySQL solo lectura. El test obtiene usuario destino, actividad y token desde el preview de la misma sesión; nunca acepte ni configure esos valores por variables de entorno.

La auditoría propia de DOC-28 se registra en `log_usuario`, con `Mecanismo=ASMX_ENVIO_USUARIO` dentro de `Valor_Log`. La consulta `DOC28_E2E_AUDIT_SQL` debe ser un único `SELECT` con un parámetro `?` que filtre ese mecanismo y `Tarea=<parámetro>`; no use `wf_log_estados_workflow` como sustituto.

`test:doc28:concurrency` es una carrera fija de exactamente dos solicitudes mutantes y no una prueba de carga. Requiere además `DOC28_E2E_CONCURRENCY_AUTHORIZED=true`, autorización explícita de concurrencia, tarea descartable y las dos consultas MySQL de lectura. Debe producir una sola respuesta `completada`, un único bloqueo entre `WORKFLOW_TRANSITION_IN_PROGRESS`, `WORKFLOW_VERSION_CONFLICT` o `WORKFLOW_TASK_UNAVAILABLE`, y cambios de estado/auditoría. No admita niveles configurables, usuarios virtuales ni carga masiva.

`test:doc29:user-send-lock` es una única transición UI DOC-29, no una carrera ni una prueba de carga. Intercepta la respuesta después de que el servidor atendió el POST para mantener el diálogo en estado `enviando`; durante ese intervalo verifica controles deshabilitados, fondo, Escape, API y el guardia `beforeunload`. Exige una tarea descartable seleccionada activamente en la sesión y las dos banderas de autorización. La evidencia saneada solo conserva conteos, banderas y huellas.

La evidencia DOC-28 solo puede guardar códigos, conteos, banderas de cambio y huellas; no guarde destinos, token, cookies, secretos, cadena de conexión ni cuerpos de respuesta. Al cierre, ejecute el control del gate y la comprobación de páginas legacy de la sección anterior.

## DOC-32 — Devolver actividad anterior

DOC-32 usa exclusivamente `tests/support/authenticated-workflow-session.cjs` para la sesión Gestión; no cree un login, cookies ni `.env` alternativos. Antes de ejecutar cualquier comando autenticado, obtenga autorización explícita para el ambiente y la cuenta Workflow. Antes de ejecución o concurrencia, obtenga además autorización explícita para cada tarea descartable.

Los comandos DOC-32 solicitan los datos desde la consola interactiva y los pasan solo a los procesos de la corrida. Las contraseñas Workflow y MySQL se capturan ocultas; no se deben cargar manualmente, persistir con `setx` ni pegar en el chat. El destino de controles es el DSN ODBC no sensible `workflowconta`; nunca se acepta una URL ni una cadena de conexión. Sin TTY, el comando se detiene antes de iniciar sesión o abrir un navegador.

### Corrida unificada reutilizable

Para reutilizar una configuración no sensible entre DOC y ambientes, copie la plantilla `profiles/workflow-e2e.profile.example.json` a una ubicación local externa como `C:\cert\contet.txt`, conserve el contenido como JSON y ajuste únicamente URL, `ignoreHttpsErrors`, módulo, nombre de DSN ODBC, tareas descartables, `previewActivityNames`, `executionActivityName`, `executionFinalActivityName`, `concurrencyActivityName`, consultas `SELECT` y presupuestos. `previewActivityNames` declara la lista exacta y sin repetidos que debe devolver el preview; el harness exige que no haya más páginas ni destinos distintos. `executionActivityName` debe ser una sola actividad de esa lista; el harness deriva el conector y el token desde el preview vigente y falla cerrada si no hay una coincidencia única. `executionFinalActivityName` es el nombre de la actividad activa después de ejecutar (`NOMBRE_ACTIVIDAD`), no el grupo ni el usuario destino; puede diferir de la etiqueta del preview solo cuando la ruta crea otra actividad efectiva. `concurrencyActivityName` obliga a la carrera a elegir una única actividad de su preview vigente en vez de depender del primer resultado. `ignoreHttpsErrors` debe ser booleano y permanecer en `false` salvo que el ambiente autorizado use un certificado autofirmado. El perfil nunca puede incluir cuenta Workflow, contraseña, cookies, token, autorización, URL MySQL ni cadena de conexión.

Con autorizaciones explícitas del responsable para ambiente, primera tarea y segunda tarea, ejecute una sola secuencia DOC-32:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:run -- --doc doc32 --profile C:\cert\contet.txt --authorize environment,execution,concurrency
```

El iniciador valida el perfil, el gate, las consultas, las autorizaciones y el DSN ODBC antes de pedir una sola vez la cuenta Workflow y su contraseña oculta, además del usuario y contraseña MySQL de solo lectura. El perfil conserva solo el nombre del DSN; nunca se solicita, imprime ni persiste una URL o cadena de conexión. Después ejecuta preview, devolución y carrera en orden; si una etapa falla, no ejecuta las posteriores. Los secretos desaparecen al terminar y los controles de cierre se aplican aun cuando la secuencia se interrumpe.

Si las tareas descartables deben ser seleccionadas manualmente antes de cada operación, el responsable puede dividir la secuencia sin guardar autorizaciones ni secretos: `--stages preview,execution --authorize environment,execution` para la primera tarea, y después `--stages concurrency --authorize environment,execution,concurrency` para una segunda tarea ya seleccionada. Cada invocación solicita los secretos una sola vez y conserva los mismos controles de cierre.

### Recursos reutilizables para cualquier E2E

Una etapa mutante debe declarar un contrato de recurso registrado, no un comando ni una consulta escrita en el perfil. El contrato define el descriptor no sensible, el preflight de solo lectura, el alcance de reserva y si una transición consume el recurso. La plataforma reserva el recurso antes de iniciar la etapa mutante y lo libera al fallar o lo marca consumido al completar; solo un preflight que observe una nueva generación preparada puede habilitarlo de nuevo.

La reserva `local` protege corridas en este espacio de trabajo. Para un recurso compartido entre equipos se requiere un coordinador compartido registrado; si falta, la corrida falla cerrada. No implemente restauraciones de datos de negocio en el núcleo: un adaptador puede declarar una restauración segura y autorizada, o el responsable prepara otro recurso. Para registrar un escenario no Workflow, añada su adaptador y pruebas de política; no herede tareas, actividades ni consultas DOC-32.

`DOC32_E2E_TASK_STATE_SQL` y `DOC32_E2E_AUDIT_SQL` deben ser cada una una sola sentencia `SELECT` con exactamente un parámetro `?`. La cuenta MySQL es solo de lectura. La consulta de auditoría debe apuntar al registro adicional `ASMX_DEVOLVER_ACTIVIDAD`; no sustituirlo por el log histórico del motor.

Primero, el preview real no mutante:

```powershell
npm.cmd --prefix tools/e2e run test:doc32:preview
```

Después, con autorización de la primera tarea descartable, ejecute la transición oficial. El comando pedirá confirmación literal `SI`:

```powershell
npm.cmd --prefix tools/e2e run test:doc32:execute
```

Finalmente, la segunda tarea descartable debe ser distinta y nueva. La carrera no es una prueba de carga ni admite usuarios virtuales o niveles configurables:

```powershell
npm.cmd --prefix tools/e2e run test:doc32:concurrency
```

El harness toma `IdConector` y token exclusivamente del preview actual. La evidencia solo conserva códigos, conteos, banderas, latencias y huellas. Al cierre, comprobar el gate en `false` con usuarios/grupos vacíos y ejecutar `git diff --name-only -- workflow/Webworkflow.aspx workflow/Webworkflow.aspx.vb`; no debe haber salida.

### Registrar otro DOC

Cada DOC adicional debe declarar en el registro común su lista estricta de campos de perfil no sensibles, las etapas ordenadas, las autorizaciones de cada etapa, el mapeo de variables efímeras y las pruebas propietarias que se invocan. El perfil no puede seleccionar scripts ni comandos. Antes de habilitarlo, agregue pruebas de perfil, autorización, orden, limpieza de secretos y evidencia saneada; conserve las pruebas específicas del DOC sin alterar sus semánticas.
