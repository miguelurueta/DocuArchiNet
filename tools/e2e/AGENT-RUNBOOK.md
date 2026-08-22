# Runbook para agentes — E2E y concurrencia DOC-10 / DOC-11 / DOC-28

Este runbook permite reutilizar las pruebas ASMX Workflow sin copiar credenciales, modificar datos sin autorización ni reactivar la política legacy.

## Límites obligatorios

- Ejecutar solo contra un ambiente de pruebas autorizado; nunca inferir autorización para producción.
- Recibir URL, cuentas y acceso MySQL de solo lectura mediante secretos del entorno o instrucción expresa. No crear `.env` ni mostrar secretos, cookies o cadenas de conexión.
- Reutilizar `tests/support/authenticated-workflow-session.cjs` para todo login E2E DOC-10/DOC-11/DOC-28. No enviar usuario, grupo, ruta, actividad ni permisos al ASMX salvo el destino que DOC-28 obtiene del preview actual al ejecutar.
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
