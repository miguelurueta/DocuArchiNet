# Pruebas, evidencia y riesgos

- Ticket: DOC-32
- Cambio OpenSpec: `doc-32-backend-actividad-anterior`
- Clasificación: `cross_cutting`

## Verificación local

Ejecutado el 2026-08-24 desde la raíz del repositorio:

```powershell
node --test tools/e2e/tests/e2e-test-resource-lifecycle.test.cjs tools/e2e/tests/workflow-e2e-orchestrator.test.cjs tools/e2e/tests/doc32-return-activity-policy.test.cjs tests/workflow-return-activity.test.cjs
openspec.cmd validate doc-32-backend-actividad-anterior --strict
```

Resultado: 44 pruebas Node aprobadas y validación OpenSpec estricta aprobada. La suite cubre contratos exclusivos, permiso fail-closed, Ruta/Flujo, `SELECT` parametrizado, término, límite, orden, cursor, token, conector manipulado, lock por tarea, concurrencia, adaptador, eventos, notificación, auditoría, aislamiento, perfil reutilizable y políticas de evidencia.

La compilación de la solución incorpora los archivos VB de DOC-32 y fue verificada durante la implementación. Las advertencias heredadas de ensamblados .NET Framework no se consideran evidencia de un error atribuible a DOC-32 cuando MSBuild termina sin errores.

## E2E protegida autorizada

La E2E real se ejecutó el 2026-08-24 con ambiente, cuenta Workflow, dos tareas descartables, presupuestos de latencia y controles de lectura explícitamente autorizados. La configuración se obtuvo de un perfil local externo al repositorio y las credenciales se introdujeron de forma efímera en TTY; no se registraron en código, evidencia ni documentación.

| Etapa | Comprobación | Resultado |
| --- | --- | --- |
| Preview | Huellas de estado y auditoría antes/después mediante dos controles `SELECT` parametrizados. | Aprobado; el preview no mutó tarea, estado ni auditoría. |
| Ejecución | Conector y token derivados del preview vigente; transición y actividad final verificadas por control de solo lectura. | Aprobado; una única transición confirmada. |
| Concurrencia | Dos solicitudes simultáneas sobre una segunda tarea descartable. | Aprobado; una de dos solicitudes produjo transición efectiva y la otra quedó bloqueada de forma controlada. |

La evidencia saneada reside en `tools/e2e/artifacts/doc32-return-activity-preview.json`, `doc32-return-activity-execution.json` y `doc32-return-activity-concurrency.json`. Conserva únicamente códigos, conteos, banderas, latencias y huellas; no conserva destino, token, usuario, cookies, secretos, cadena de conexión ni cuerpos de respuesta.

## Controles operativos y límites

Antes de una nueva corrida autenticada debe leerse `tools/e2e/AGENT-RUNBOOK.md`. Se requiere autorización explícita, perfil local modificable, tarea descartable preparada para cada etapa y presupuestos de latencia. Las consultas de control siguen siendo un único `SELECT` con exactamente un parámetro para la tarea; la suite no escribe configuración ni datos directamente.

Al cierre de la corrida ejecutada se verificó que el gate permanecía en `false`, usuarios y grupos vacíos y sin cambios en las páginas legacy. Esta evidencia no autoriza despliegue, activación de gate, carga ni futuras transiciones reales.

## Riesgos residuales y relevo

- Una nueva E2E requiere volver a preparar tareas descartables en el estado y asignación adecuados; las tareas consumidas no se reutilizan como prueba de transición.
- La semántica Ruta/Flujo depende del esquema Workflow vigente; cambios de base de datos deben volver a validar columnas y consultas de control aprobadas.
- DOC-32 no incluye interfaz ni validación visual. La etapa siguiente debe definir accesibilidad, selección, confirmación y su evidencia sin alterar este contrato.
