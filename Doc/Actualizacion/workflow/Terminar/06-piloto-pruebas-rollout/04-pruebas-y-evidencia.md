# Pruebas, matriz y evidencia — DOC-14

## Evidencia local ejecutada

| Fecha | Comando | Resultado |
| --- | --- | --- |
| 2026-08-18 | `node --test tests/workflow-modern-feature-gate.test.cjs` | Aprobado: gate, bootstrap, ASMX, telemetría, reporte y rollback |
| 2026-08-18 | `msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m` | Aprobado: 0 errores; el proyecto conserva advertencias históricas a gestionar por separado |
| 2026-08-18 | `Verify-Doc14PilotGate.ps1` sobre copia temporal del ensamblado | Aprobado: inactivo, alcance vacío, metadatos, exclusión, usuario, grupo y rollback |
| 2026-08-18 | `Verify-Doc14Telemetry.ps1` sobre dobles en memoria | Aprobado: éxito, bloqueo, error y falla de auditoría |
| 2026-08-18 | `Verify-Doc11Transition.ps1` sobre el ensamblado local | Aprobado sin modificar datos |
| 2026-08-18 | `Get-Doc14PilotReport.ps1` con ejemplo sanitizado | Aprobado: dos canales y seis eventos agregados |
| 2026-08-18 | `npm.cmd --prefix tools/e2e run test:doc11:anonymous` contra localhost | Aprobado: `EjecutarEnvioTarea` sin sesión queda bloqueado; no modifica datos |
| 2026-08-18 | `npm.cmd --prefix tools/e2e run test:doc11:validation` contra localhost | Aprobado: sesión autenticada y parámetros inválidos reciben bloqueo funcional; no modifica datos |
| 2026-08-18 | `npm.cmd --prefix tools/e2e run test:session` contra localhost, tarea 557 | Aprobado: la sesión autenticada resuelve el contexto en `PreviewEnviarTarea`; solo lectura |
| 2026-08-18 | `npm.cmd --prefix tools/e2e run test:doc11:execute` contra localhost, tarea 557 | Aprobado: bloqueo esperado `WORKFLOW_REQUIREMENT_NOT_MET`; el estado de la tarea no cambió y se auditó el intento |

Se ejecutaron los E2E no mutantes indicados, incluido el preview autenticado de la tarea 557, y un único E2E mutante controlado cuyo resultado fue el bloqueo funcional esperado. No se ejecutó carga, QA visual completa ni un envío exitoso de la tarea 557. Antes de cualquier prueba autenticada debe leerse `tools/e2e/AGENT-RUNBOOK.md`; cada corrida restaura el gate en `false`, con modo oficial y listas de alcance vacíos.

## Estado de configuración canónica

Desde el 2026-08-18, esta raíz queda configurada para habilitación oficial: `WorkflowCentroTrabajoModernActive=true`, `WorkflowCentroTrabajoModernOfficialMode=true` y usuarios/grupos de alcance vacíos. Los demás ambientes deben recibir este mismo `Web.config` mediante su despliegue controlado; no basta con cambiar la copia local.

## Matriz QA pendiente de autorización

| Caso | Cobertura | Resultado esperado |
| --- | --- | --- |
| Gate apagado | Apertura, preview y ejecución | Legacy en apertura; ASMX bloqueado |
| Usuario/grupo incluido y exclusión | Alcance y precedencia | Solo el alcance aprobado obtiene UI moderna |
| Ruta y flujo | Destinos y confirmación | Mismo resultado funcional que legacy |
| Requisitos, firma, expediente y correo | Reglas preservadas | Bloqueo o ejecución legacy aplicable |
| Conector inválido y doble clic | Errores y concurrencia | Bloqueo seguro; ninguna duplicación |
| Rollback durante bloqueo | Retorno a legacy | Sin cambio de estado ni pérdida de contexto |
| Resoluciones | 1366x768, 1024x768, 768x1024, 375x812 | Controles utilizables, foco visible y mensajes legibles |
| Accesibilidad | Teclado, foco, lector y contraste | Sin bloqueo de navegación ni pérdida de foco |

La matriz es una guía reproducible, no evidencia de ejecución. Debe registrar solo referencias permitidas, sin credenciales, cookies, conexiones ni datos personales innecesarios.

## Cierre de promoción

La promoción requiere los umbrales de [00-indice.md](00-indice.md), evidencia de rollback, métricas por canal y aprobación explícita funcional/técnica. Cualquier evento crítico bloquea la promoción y obliga a mantener o restaurar legacy.
