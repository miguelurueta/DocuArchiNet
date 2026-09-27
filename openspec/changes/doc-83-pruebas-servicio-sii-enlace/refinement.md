<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-83-pruebas-servicio-sii-enlace

## Fuente y alcance

- Ticket: `DOC-83` — PRUEBAS-SERVICIO-SII-ENLACE
- Cambio: `doc-83-pruebas-servicio-sii-enlace`
- Perfil: WebForms VB.NET, JavaScript/CSS, Node test runner y plataforma Playwright/E2E propia.
- Alcance: pruebas, validadores, evidencia, gate, compatibilidad, rollout/rollback y cierre. No agrega funcionalidad productiva salvo refinamiento posterior ante un defecto reproducible.

## Contexto inspeccionado

- `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` completos.
- Exploración `Doc/Actualizacion/workflow/ImportarServiciWebEnlace/exploracion/modernizacion-importacion-anexos-sii-enlase.md`.
- Registro y plataforma: `tools/e2e/scripts/support/workflow-e2e-platform-registry.cjs`, `workflow-e2e-platform.cjs`, `run-workflow-e2e-platform.cjs` y adaptador de importación.
- Escenarios y perfiles DOC-80, DOC-81 y DOC-82 en `tools/e2e/tests/` y `tools/e2e/profiles/`.
- Suites ENLASE, gate, legacy, seguridad, persistencia, reconciliación, UI y documentación bajo `tests/`.
- Gate real en `Web.config`, `Infrastructure/Workflow/ImportarServicioWeb/ImportarServicioWebFeatureGate.vb`, `workflow/ImportarServicioWebPreview.ashx.vb` y `workflow/Webworkflow.aspx.vb`.
- Evidencia y documentación técnica de DOC-80/81/82.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
|---|---|---|---|---|---|
| D-01 | DOC-83 es cierre por pruebas; no cuarta implementación funcional | proposal DOC-83 y entregables DOC-80/81/82 | D-01 | RQ-02 | Origen: D-01, RQ-02 |
| D-02 | Matriz única relaciona riesgo, prueba, autorización, control y evidencia | `tools/e2e/scripts/support/workflow-e2e-platform-registry.cjs` | D-02 | RQ-01 | Origen: D-02, RQ-01 |
| D-03 | Se reutilizan read, UI y execution; solo se cubren brechas | escenarios `import-sii-enlase-*` | D-03 | RQ-03, RQ-04 | Origen: D-03, RQ-03 |
| D-04 | Lectura, importación y asignación tienen autorizaciones/recursos independientes | `requiredAuthorizationsFor`, contratos de recurso | D-04 | RQ-03, RQ-04, RQ-07 | Origen: D-04, RQ-07 |
| D-05 | Se conserva el gate global y rollback legacy; no gate nuevo | `Web.config`, `ImportarServicioWebFeatureGate.vb`, `assertPlatformIntegrity` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Evidencia mínima y controles SELECT; secretos prohibidos | `createSafeEvidence`, validación de perfiles | D-06 | RQ-08 | Origen: D-06, RQ-08 |
| D-07 | Idempotencia, existencia física, deduplicación y tarea original son criterios de cierre | adaptador DOC-81 y suites de reconciliación/lista | D-07 | RQ-04 | Origen: D-07, RQ-04 |
| D-08 | Importar no asigna; autoridad final permanece en `Buttonaceptar_Click` | prueba ENLASE assignment y code-behind | D-08 | RQ-07 | Origen: D-08, RQ-07 |
| D-09 | Autorización/evidencia/restauración faltante bloquea el cierre | runner `finally`, runbook y gobierno OPSXJ | D-09 | RQ-09 | Origen: D-09, RQ-09 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio | Riesgo/compatibilidad |
|---|---|---|---|
| RQ-01 | Inventario completo y legible por máquina | Una referencia faltante hace fallar la validación | Evita cobertura declarativa incompleta |
| RQ-02 | Regresión determinista integral | Suites afectadas pasan sin copiar fixtures ni debilitar producción | Protege constancias y legacy |
| RQ-03 | Read/UI no mutan | Siete controles permanecen invariantes y contexto/foco se conservan | Evita contaminación de tarea |
| RQ-04 | Importación idempotente/aislada | Una intención/ejecución, evidencia física, sin duplicado ni transición | Riesgo alto de persistencia incierta |
| RQ-05 | Gate y legacy reversibles | `finally` restaura false/vacíos y baseline legacy | Riesgo alto de activación residual |
| RQ-06 | Accesos inválidos fallan cerrados | Sin sesión/contexto/capacidad/autorización no hay consulta ni escritura | Seguridad backend, no solo UI |
| RQ-07 | Asignación separada y revalidada | Faltante bloquea; importar no dispara postback | Riesgo muy alto de asignación indebida |
| RQ-08 | Evidencia saneada | Solo códigos/conteos/huellas; controles SELECT | Evita exposición y falsa persistencia |
| RQ-09 | Cierre condicionado | Falta real se registra como bloqueo, no como PASS simulado | Integridad OPSXJ y operativa |

## Riesgos altos y muy altos

- Almacenamiento legacy y evidencia física: cubiertos por contrato, ejecución descartable y reconciliación.
- Duplicación tras respuesta perdida: cubierto por huella idempotente, consulta/reconciliación y ausencia de reintento ciego.
- Cambio de tarea: guard frontend y controles por TaskId original.
- Gate global: línea base antes/después y restauración en `finally`.
- Validación de asignación: `Buttonaceptar_Click` es autoridad; no se simula endpoint inexistente.
- Dependencia externa: indisponibilidad del SII produce bloqueo explícito y conserva evidencia del fallo.

## Resultado del refinamiento

- Estado: aprobado.
- No se autoriza todavía ninguna E2E real, activación de gate, uso de cuenta o mutación.
- Siguiente paso: ejecutar `opsxj:refine -- DOC-83`, implementar la matriz y solicitar autorización únicamente al alcanzar una corrida real concreta.