<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-79-pruebas-retiro-gate

## Fuente y alcance

- Ticket: `DOC-79` — PRUEBAS-RETIRO-GATE
- Cambio OpenSpec: `doc-79-pruebas-retiro-gate`
- Perfil: ASP.NET Web Forms/VB.NET, JavaScript CommonJS, Node test runner y plataforma E2E compartida.
- Alcance: pruebas, validación local, ocultamiento reversible bajo gate, inventario legacy y evidencia; no eliminación de código legacy.

## Contexto inspeccionado

- `workflow/Webworkflow.aspx(.vb)`: árbol visual y handlers legacy de importación SII, bootstrap moderno y exposición del gate.
- `webservice/WebServiceImportarServicioWebModern.asmx.vb`: ocho operaciones modernas y validación inicial de `WorkflowCentroTrabajoModernActive`.
- `tests/importar-servicio-web-*.test.cjs`: contratos, gate, regresión legacy, almacenamiento, ejecución, progreso, reconciliación y contexto.
- `tools/e2e/tests/importar-servicio-web-modern.spec.cjs` y plataforma compartida: lectura, ejecución, recuperación, retry, concurrencia, controles SELECT y restauración del gate.
- `web.config`: gate activo por autorización posterior a la certificación y listas de audiencia vacías; la funcionalidad aplica a todos los usuarios autenticados del módulo.
- Ruta documental vigente: `Doc/Actualizacion/workflow/ImportarServicioWeb/`; no se recreará `docs/`.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Consolidar validación frontend local, determinista y sin red, reutilizando suites existentes. | `tests/importar-servicio-web-*.test.cjs`; `tools/validation/` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Probar el alcance completo del gate en las ocho operaciones antes de dependencias o efectos. | `webservice/WebServiceImportarServicioWebModern.asmx.vb`; `web.config` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Mantener una sola entrada/handler moderno, ocultar inicialmente el árbol legacy bajo gate y conservar el fallback al apagarlo. | `workflow/Webworkflow.aspx(.vb)`; módulos `js/workflow/importar-servicio-web/` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Verificar una ejecución por intención, espera global real, proyección completa y ausencia de duplicados. | UI, adaptadores de progreso/reconciliación y orquestador | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Reutilizar exclusivamente `tools/e2e` y evidencia vigente; una corrida real requiere autorización explícita y restauración final. | runbook, runner, registry y suite moderna | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Inventariar referencias legacy y posponer cualquier retiro físico a otro cambio expresamente autorizado. | controles WebForms, handlers, ASMX, `JSProgresBar`, `ClassAlmacenamiento` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Mantener documentación y evidencia saneada solo en la ruta canónica vigente. | `Doc/Actualizacion/workflow/ImportarServicioWeb/` | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Un comando local valida arquitectura UI, gate, legacy e invariancia sin red. | Falla con salida no cero ante cualquier contrato roto. | No duplicar suites ni infraestructura. |
| RQ-02 | Cada endpoint rechaza acceso sin sesión válida o con gate apagado antes de resolver proveedor o efectos. | Gate apagado o sesión inválida produce error seguro y cero mutaciones; gate activo admite cualquier sesión válida. | No duplicar autorización con listas de usuario o grupo. |
| RQ-03 | Gate activo muestra una sola entrada moderna; gate apagado conserva legacy. | No coexisten dos handlers efectivos para la misma acción. | El markup y handlers legacy no se eliminan. |
| RQ-04 | Una intención produce una llamada de ejecución y todos sus documentos confirmados se proyectan una vez. | La espera no inventa progreso y la reconciliación no duplica filas. | Preservar resultados parciales y recuperación. |
| RQ-05 | E2E usa plataforma, perfiles, controles y saneamiento existentes. | Toda corrida autorizada termina con gate falso y audiencias vacías, incluso al fallar. | Sin autorización se limita a validación local. |
| RQ-06 | Existe inventario de referencias y criterio explícito de retiro futuro. | Ningún control se declara removible mientras conserve referencias o carezca de evidencia. | Rollback inmediato mediante gate/fallback. |
| RQ-07 | Documentación y evidencia residen exclusivamente bajo la ruta canónica DOC-79. | No se crea `docs/` ni se persisten secretos. | Evidencias solo saneadas. |

## Reglas de trazabilidad obligatorias

1. Cada decisión D-XX se refleja en design, spec y al menos una tarea.
2. Cada tarea declara área, complejidad, origen y verificación.
3. E2E real, gate o cuentas requieren autorización explícita; las consultas de control son solo SELECT.
4. El estado final del gate es `false`, con usuarios y grupos vacíos.

## Resultado del refinamiento

- Estado: aprobado por el usuario el 2026-09-23.
- Alcance listo para planificación atómica e implementación.
