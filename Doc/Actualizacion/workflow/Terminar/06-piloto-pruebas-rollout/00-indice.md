# DOC-14 — Piloto de despliegue controlado

Estado: modo oficial local activado con aprobación explícita. Fecha de activación: 2026-08-18T19:06:53Z.

El repositorio usa `WorkflowCentroTrabajoModernActive=true` y `WorkflowCentroTrabajoModernOfficialMode=true`. Las listas de usuarios y grupos quedan vacías de forma intencional: el modo oficial habilita únicamente contextos Workflow válidos no excluidos. No se registran personas, grupos ni datos de ambiente en este paquete.

## Alcance y aprobación previa

| Campo obligatorio | Valor antes de la activación | Responsable | Evidencia requerida | Acción si falta |
| --- | --- | --- | --- | --- |
| Modo oficial | `true`; listas de piloto vacías | Responsable funcional | Aprobación explícita | Rollback |
| Inicio UTC | `2026-08-18T19:06:53Z` | Operación | Cambio aprobado | Rollback |
| Motivo | Habilitación oficial aprobada | Operación | Aprobación explícita | Rollback |
| Responsable | Operación | Líder técnico | Aprobación explícita | Rollback |
| Correlación | Debe registrarse en el sistema de cambios | Operación | Registro de rollout | Rollback |
| Umbrales | Los de la siguiente tabla | Funcional y técnico | Acta de aprobación | No promover |

## Umbrales y dependencia preservada

| Métrica o control | Umbral de promoción | Acción ante falla | Dependencia legacy preservada |
| --- | --- | --- | --- |
| Transición duplicada, pérdida de contexto, filtración, autorización o rollback | 0 eventos | Rollback; promoción bloqueada | Motor legacy y sus reglas |
| Éxito moderno | Al menos 100 intentos y ≥ 99 % | Mantener piloto o rollback | `Terminar_Tarea_Workflow` |
| Errores técnicos modernos | ≤ 1 % | Investigar; no ampliar alcance | Manejo legacy de errores |
| Bloqueos | No superar la línea base en más de 2 puntos porcentuales | Revisar gate/reglas | Autorización legacy |
| Duración p95 | No superar línea base en más de 20 % | No promover | Ejecutor legacy |
| Auditoría | 100 % de intentos con registro o advertencia segura | Corregir adaptador; no promover | `log_usuario` existente |

Los porcentajes se calculan por canal con `tools/validation/Get-Doc14PilotReport.ps1`. Sin línea base, volumen mínimo, responsables y aprobación explícita, el estado es `PENDIENTE_APROBACION`.

## Entregables

- [Arquitectura](01-arquitectura.md)
- [Contrato operativo](02-contrato.md)
- [Flujo, rollback y seguridad](03-flujo-y-seguridad.md)
- [Pruebas, matriz y evidencia](04-pruebas-y-evidencia.md)
- [Diagramas](Diagramas/01-gate-y-rollback.md)

## Decisión de promoción

La activación oficial fue aprobada por el responsable del cambio. Solo el responsable funcional y el líder técnico pueden mantenerla o ampliarla después de revisar las métricas agregadas, la matriz QA y la evidencia de rollback. La decisión debe registrar fecha UTC y correlación fuera de este repositorio si contiene identidades.
