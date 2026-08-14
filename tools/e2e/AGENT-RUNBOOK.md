# Runbook para agentes — E2E y concurrencia DOC-10

Este documento permite reutilizar las pruebas del ASMX `PreviewEnviarTarea` sin copiar credenciales ni modificar datos Workflow.

## Límites obligatorios

- Ejecutar contra un ambiente de pruebas autorizado; nunca asumir autorización para producción.
- Recibir cuentas, URL y acceso MySQL de solo lectura únicamente por secretos del entorno o una instrucción explícita del responsable. No crear `.env`, no escribir ni mostrar valores de variables de entorno sensibles.
- No enviar al ASMX usuario, grupo, ruta, actividad ni permisos: los scripts usan el login real de `gestor.aspx` y después llaman `PreviewEnviarTarea(idTarea)`.
- El ASMX debe mantenerse de solo lectura. Las consultas de tarea y auditoría aceptadas por los scripts son una sola sentencia `SELECT` con un parámetro `?` para la tarea.
- No modificar el flujo legacy. Al finalizar verificar que `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb` no tengan cambios.

## Preparación

Desde la raíz del repositorio:

```powershell
npm.cmd --prefix tools/e2e install
npm.cmd --prefix tools/e2e run install:browsers
```

Si ya existe Microsoft Edge administrado, usar `DOC10_E2E_BROWSER_CHANNEL=msedge`. Las variables comunes se entregan por el almacén de secretos del agente o la sesión actual:

```powershell
$env:DOC10_E2E_BASE_URL = 'https://ambiente-pruebas/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<piloto>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '<tarea-activa-autorizada>'
```

Para las verificaciones con huellas, agregar un usuario MySQL de solo lectura y una consulta de auditoría aprobada:

```powershell
$env:DOC10_E2E_MYSQL_URL = 'mysql://usuario_solo_lectura:secreto@host/base'
$env:DOC10_E2E_AUDIT_SQL = 'SELECT COUNT(*) AS total FROM tabla_auditoria WHERE id_tarea = ?'
```

`DOC10_E2E_TASK_STATE_SQL` es opcional. Si se usa, también debe ser un único `SELECT` con un solo `?`.

## Gate temporal

El gate normal es:

```text
WorkflowCentroTrabajoModernActive=false
WorkflowCentroTrabajoModernUsers=
WorkflowCentroTrabajoModernGroups=
```

Solo tras autorización explícita, habilitarlo temporalmente y limitarlo al piloto de prueba. Usar una edición reversible, ejecutar la prueba y restaurar inmediatamente los tres valores anteriores, incluso si la prueba falla. Confirmar la restauración con:

```powershell
rg -n "WorkflowCentroTrabajoModern(Active|Users|Groups)" Web.config
```

## Selección de prueba

| Objetivo | Comando | Requisitos adicionales | Resultado esperado |
| --- | --- | --- | --- |
| Borde sin sesión | `npm.cmd --prefix tools/e2e run test:anonymous` | Solo URL y tarea. | `WORKFLOW_CONTEXT_INVALID`, sin destinos. |
| Diagnosticar login Gestión → Workflow | `npm.cmd --prefix tools/e2e run test:session` | Piloto y gate no necesario. | No retorna `WORKFLOW_CONTEXT_INVALID`. |
| Gate piloto/no piloto | `npm.cmd --prefix tools/e2e run test:authorization` | Agregar usuario y clave no piloto; gate limitado al piloto. | Piloto supera el gate; no piloto recibe `WORKFLOW_MODERN_INACTIVE`. |
| E2E funcional completa | `npm.cmd --prefix tools/e2e run test:e2e` | No piloto, MySQL de solo lectura y auditoría. | Destinos o bloqueo esperado; huellas iguales antes/después. |
| Concurrencia ASMX | `npm.cmd --prefix tools/e2e run test:load` | MySQL de solo lectura y gate limitado al piloto. | Métricas por nivel y huellas iguales antes/después. |

Para `test:e2e`, definir además `DOC10_E2E_UNAUTHORIZED_USER` y `DOC10_E2E_UNAUTHORIZED_PASSWORD`. Si una tarea autorizada debe bloquearse por una razón conocida, usar `DOC10_E2E_AUTHORIZED_EXPECTED_CODE`; no usarlo para ocultar un fallo inesperado.

## Carga de 20 y 30 sesiones

`test:load` crea sesiones Gestión independientes y luego dispara el ASMX simultáneamente. El login se dosifica para no confundir un cuello de botella de Web Forms con la latencia del endpoint.

```powershell
$env:DOC10_LOAD_CONCURRENCIES = '20,30'       # predeterminado
$env:DOC10_LOAD_LOGIN_CONCURRENCY = '5'       # solo bootstrap de login
$env:DOC10_LOAD_LOGIN_TIMEOUT_MS = '30000'    # opcional
$env:DOC10_LOAD_REQUESTS_PER_SESSION = '1'    # predeterminado
$env:DOC10_LOAD_MAX_FAILURE_PERCENT = '0'     # predeterminado
$env:DOC10_LOAD_MAX_P95_MS = '<objetivo>'     # opcional, solo si fue acordado
$env:DOC10_LOAD_EVIDENCE_PATH = 'Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/evidencias/qa-preview-load.json'
npm.cmd --prefix tools/e2e run test:load
```

La evidencia incluye sesiones autenticadas/fallidas, solicitudes exitosas/fallidas, p50/p95/p99, códigos públicos de error y huellas de estado/auditoría. No contiene secretos, cookies ni cuerpos de respuesta. `LOGIN_TIMEOUT` indica un problema de bootstrap Web Forms; se reporta por separado de una falla del ASMX. No convertir el endpoint a asíncrono solo por ese código: primero revisar IIS, pool MySQL, CPU, memoria y la repetición en un ambiente representativo.

## Cierre de cada corrida

1. Conservar la evidencia resumida en `Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/evidencias/` si no incluye secretos.
2. Registrar ambiente, tipo de tarea (`FLUJO` o `RUTA`), resultado y huellas en `04-pruebas-y-evidencia.md`.
3. Restaurar el gate apagado y confirmar sus valores.
4. Ejecutar `git diff --name-only -- workflow/Webworkflow.aspx workflow/Webworkflow.aspx.vb`; el resultado debe estar vacío.
5. Si hubo procesos de navegador o prueba residuales, detenerlos solo con autorización explícita y volver a comprobar el gate.

La checklist para la aprobación humana está en [../../Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/07-checklist-qa-manual.md](../../Doc/Actualizacion/workflow/Terminar/02-preview-ruta-flujo/07-checklist-qa-manual.md). La referencia humana ampliada está en [README.md](README.md); este runbook es la entrada operativa para agentes.
