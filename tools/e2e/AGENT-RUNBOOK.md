# Runbook para agentes — E2E y concurrencia DOC-10 / DOC-11

Este runbook permite reutilizar las pruebas del ASMX `PreviewEnviarTarea` sin copiar credenciales, modificar datos Workflow ni reactivar la política legacy.

## Límites obligatorios

- Ejecutar solo contra un ambiente de pruebas autorizado; nunca inferir autorización para producción.
- Recibir URL, cuentas y acceso MySQL de solo lectura mediante secretos del entorno o instrucción expresa. No crear `.env` ni mostrar secretos, cookies o cadenas de conexión.
- Reutilizar `tests/support/authenticated-workflow-session.cjs` para todo login E2E DOC-10. No enviar usuario, grupo, ruta, actividad ni permisos al ASMX.
- `PreviewEnviarTarea` es de solo lectura. Las consultas de control son una única sentencia `SELECT` con exactamente un parámetro `?` para la tarea.
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

## Cierre de cada corrida

1. Conservar solo evidencia resumida sin secretos ni cuerpos de respuesta.
2. Registrar ambiente, tipo de tarea, resultado y huellas cuando la evidencia pertenezca al cambio en curso.
3. Confirmar que el gate siguió apagado y las listas vacías con el comando anterior.
4. Ejecutar `git diff --name-only -- workflow/Webworkflow.aspx workflow/Webworkflow.aspx.vb`; no debe producir salida.
5. No detener procesos residuales sin autorización explícita.

## DOC-11 mutante

`EjecutarEnvioTarea` cambia la tarea. Solo puede ejecutarse con autorización explícita para una tarea descartable, cuenta válida, conector y token obtenidos del preview actual, además de consultas de estado y auditoría `SELECT` con un parámetro `?` y MySQL de solo lectura.

Las pruebas anónima y de validación no cambian estado. Ejecución y concurrencia requieren además `DOC11_E2E_EXECUTION_AUTHORIZED=true`; después se confirma que la configuración del gate continuó apagada y el flujo legacy no cambió.
