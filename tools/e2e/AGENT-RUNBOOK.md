# Runbook para agentes — E2E y concurrencia DOC-10 / DOC-11 / DOC-28 / DOC-32 / DOC-33 / DOC-44

Este runbook permite reutilizar las pruebas ASMX Workflow sin copiar credenciales, modificar datos sin autorización ni reactivar la política legacy.

## Límites obligatorios

- Ejecutar solo contra un ambiente de pruebas autorizado; nunca inferir autorización para producción.
- Recibir URL, cuentas y acceso MySQL de solo lectura mediante secretos del entorno o instrucción expresa. No crear `.env` ni mostrar secretos, cookies o cadenas de conexión.
- Reutilizar `tests/support/authenticated-workflow-session.cjs` para todo login E2E DOC-10/DOC-11/DOC-28/DOC-32/Notas. No enviar usuario, grupo, ruta, actividad ni permisos al ASMX salvo el destino que DOC-28 obtiene del preview actual al ejecutar.
- `PreviewEnviarTarea` y `PreviewEnviarUsuario` son de solo lectura. Las consultas de control son una única sentencia `SELECT` con exactamente un parámetro `?` para la tarea.
- Las suites de preview no modifican el flujo legacy. Un cambio de modernización aprobado puede ajustar `workflow/`, pero debe conservar la semántica del fallback y demostrar ausencia de doble operación.
- La entrega mantiene `WorkflowCentroTrabajoModernActive=false` con usuarios y grupos vacíos. Solo un runner aprobado, con autorización literal del ambiente, mutación y gate, puede habilitarlo durante una corrida y debe restaurar exactamente la configuración segura en `finally`.

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
| DOC-33 preview UI | `test:workflow:run -- --doc doc33 --profile <perfil> --stages preview --authorize environment` | Ambiente/cuenta autorizados, tarea UI seleccionada y ODBC solo lectura. | El modal moderno representa el preview sin alterar estado ni auditoría. |
| DOC-33 devolución UI | `test:workflow:run -- --doc doc33 --profile <perfil> --stages preview,execution --authorize environment,execution` | Autorización explícita, tarea descartable seleccionada y ODBC solo lectura. | Una devolución real iniciada por el modal moderno; valida estado, auditoría y actividad final. |
| DOC-33 bloqueo UI | `test:workflow:run -- --doc doc33 --profile <perfil> --stages ui-lock --authorize environment,ui_lock` | Autorización explícita y una segunda tarea descartable seleccionada. | Retiene la respuesta de ejecución y bloquea cancelar, cerrar, Escape, modal y abandono hasta recibirla. |
| Notas anónimo | `npm.cmd --prefix tools/e2e run test:notes:anonymous` | URL autorizada. | Validación contractual: contexto rechazado, sin notas ni información interna. |
| Notas lectura | `npm.cmd --prefix tools/e2e run test:notes:read` | Ambiente/cuenta autorizados, tarea con nota visible y MySQL solo lectura. | Validación contractual: listado/consulta sin cambios de estado o auditoría. |
| Notas escritura | `npm.cmd --prefix tools/e2e run test:notes:write` | Autorización explícita, tarea descartable y MySQL solo lectura. | Validación contractual: crear idempotente, editar/conflicto/eliminar. |
| Notas concurrencia | `npm.cmd --prefix tools/e2e run test:notes:concurrency` | Autorización doble, tarea descartable distinta, nota semilla propia y MySQL solo lectura. | Validación contractual: exactamente dos actualizaciones, una efectiva y un bloqueo seguro. |
| DOC-43/44/45 UI Notas | `npm.cmd --prefix tools/e2e run test:doc44:workflow-notes` | Ambiente, cuenta y tarea descartable expresamente autorizados; datos para lecturas negativas y una nota ajena perteneciente a la misma tarea. | La presentación moderna es única; una nota propia extensa valida lectura completa y una nota ajena valida ausencia de acciones y rechazo `NotOwner`, sin modificar el gate. |

## Cierre de cada corrida

1. Conservar solo evidencia resumida sin secretos ni cuerpos de respuesta.
2. Registrar ambiente, tipo de tarea, resultado y huellas cuando la evidencia pertenezca al cambio en curso.
3. Confirmar que el gate siguió apagado y las listas vacías con el comando anterior.
4. Ejecutar `git diff --name-only -- workflow/Webworkflow.aspx workflow/Webworkflow.aspx.vb`; solo DOC con retiro o modernización explícitamente trazada puede contener esas rutas. Cualquier cambio no declarado detiene la corrida.
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

## DOC-33 — Devolución por interfaz moderna y bloqueo de respuesta

DOC-33 reutiliza la sesión autenticada y el ODBC de solo lectura de DOC-32, pero opera el modal oficial de **Elegir actividad anterior**. No agrega login, cookies, `.env`, URL de conexión ni atajos de backend. El perfil contiene solo configuración no sensible y declara dos tareas distintas: una para la devolución UI y otra exclusivamente para el escenario de bloqueo.

Para derivar un perfil DOC-33 desde un perfil **JSON no sensible** DOC-32 existente sin copiar secretos, cree una vez el archivo persistente y editable:

```powershell
node tools/e2e/scripts/create-doc33-workflow-ui-profile.cjs --source C:\cert\doc32-e2e.profile.json --destination C:\cert\doc33-e2e-852.profile.json --execution-task 852 --lock-task 843
```

El archivo fuente debe seguir `profiles/workflow-e2e.profile.example.json` y no puede contener cuentas ni contraseñas. Un archivo heredado `clave=valor` con credenciales no es un perfil E2E y se rechaza antes de abrir el navegador. La primera tarea queda como `uiExecutionTaskId=852`. La segunda es una reserva para una futura E2E de bloqueo y debe reemplazarse por una tarea descartable diferente, preparada y seleccionada por la cuenta autorizada. No ejecute ambas etapas sobre la misma tarea ni use una tarea ya devuelta.

Después de autorización explícita del ambiente, la cuenta y la tarea 852, la devolución UI se inicia con una sola corrida:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:run -- --doc doc33 --profile C:\cert\doc33-e2e-852.profile.json --stages preview,execution --authorize environment,execution
```

La etapa `ui-lock` se ejecuta únicamente después de preparar la segunda tarea declarada y de obtener autorización explícita para ella:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:run -- --doc doc33 --profile C:\cert\doc33-e2e-852.profile.json --stages ui-lock --authorize environment,ui_lock
```

La prueba intercepta la respuesta **después** de enviar el único POST al servidor. Desde el navegador, esto reproduce una ejecución cuya respuesta sigue pendiente: la confirmación, el fondo, Escape, el cierre API, el cierre del modal y `beforeunload` deben permanecer bloqueados. Al liberar la respuesta, verifica una transición, estado/auditoría y actividad final por ODBC. La evidencia conserva solo conteos, banderas, códigos funcionales, latencias y huellas; no destinos, actividades, tokens, cuerpos, cookies ni credenciales.

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

## Notas Workflow

Las pruebas de Notas usan exclusivamente `tests/support/authenticated-workflow-session.cjs` y los contratos modernos de Notas con `idTarea` explícito. DOC-44 posee la regresión E2E exclusiva del consumidor de Notas; DOC-32 conserva su transición de devolución de actividad. No se permite usar `Session("ID_TAREA_SELECCIONDA")`, endpoints legacy, login alterno, `.env` ni valores de usuario, grupo, permiso o autor entregados por el cliente.

Los comandos `test:notes:*` precargan desde valores no sensibles la raíz local de Gestión, módulo y ambiente `GESTOR` (se pueden sustituir por variables efímeras no sensibles del mismo nombre) y solicitan directamente en una consola interactiva solo los datos de cuenta, las confirmaciones y las tareas. La contraseña Workflow y la contraseña MySQL se capturan ocultas; el control de base de datos reutiliza exclusivamente el DSN ODBC no sensible `workflowconta` y solicita el usuario MySQL de solo lectura. No se acepta URL ni cadena de conexión MySQL, ni se deben cargar secretos manualmente, persistir con `setx` ni pegar en el chat. Sin TTY, el comando se detiene antes de iniciar sesión o abrir un navegador.

Por defecto TLS se valida. Solo con autorización explícita para un certificado autofirmado local puede establecerse temporalmente `NOTES_E2E_IGNORE_HTTPS_ERRORS=true` en el proceso de la corrida; el navegador y el cliente HTTP ASMX comparten entonces esa excepción, conservando la sesión en memoria y sin persistir certificados ni secretos.

Antes de cualquier modo autenticado, el responsable debe contar con autorización del ambiente, cuenta válida, acceso MySQL de solo lectura y las tareas aprobadas. El iniciador los solicitará en esa consola, incluidos los presupuestos; pedirá confirmación literal `SI` para ambiente, escritura y concurrencia antes de asignar las banderas correspondientes.

`NOTES_E2E_SERVICE_PATH` es opcional y, si se omite, la suite usa `webservice/WebServiceWorkflowNotesModern.asmx`. La suite fija `NOTES_E2E_ODBC_DSN=workflowconta` y registra sus dos controles no sensibles: el estado toma metadatos de `ANOTACION_TAREA` sin `DATO_ANOTACION`, y la auditoría toma metadatos de `wf_log_workflow` sin `datos_operacion`; ambas sentencias son un único `SELECT` con exactamente un parámetro `?`. La cuenta MySQL del DSN es solo de lectura.

Primero ejecute el borde anónimo y la lectura real no mutante. Cada comando preguntará solo los valores necesarios. La lectura usa una tarea con al menos una nota visible y debe conservar las huellas de estado y auditoría:

```powershell
npm.cmd --prefix tools/e2e run test:notes:anonymous
npm.cmd --prefix tools/e2e run test:notes:read
```

Escritura y concurrencia son mutantes. Requieren autorización explícita del ambiente y de cada tarea descartable, solicitadas por el iniciador. La concurrencia usa una nota semilla propiedad de la cuenta autorizada y ejecuta exactamente dos actualizaciones con la misma versión; no es una prueba de carga:

```powershell
npm.cmd --prefix tools/e2e run test:notes:write
npm.cmd --prefix tools/e2e run test:notes:concurrency
```

La regresión DOC-44 agrega lecturas negativas sobre tarea ajena/inactiva y nota cruzada, además de un CRUD sobre una tarea descartable. Requiere tres confirmaciones independientes y restaura el gate aunque falle:

```powershell
npm.cmd --prefix tools/e2e run inspect:doc44:test-data
npm.cmd --prefix tools/e2e run test:doc44:workflow-notes
```

La evidencia de Notas solo conserva modo, códigos, conteos, latencias, banderas y huellas. No guarda el contenido de notas, credenciales, cookies, tokens, usuarios, destinos, cadenas de conexión ni cuerpos HTTP. Al cierre aplique todos los controles de la sección "Cierre de cada corrida": gate apagado, listas vacías y fallback legacy disponible.
