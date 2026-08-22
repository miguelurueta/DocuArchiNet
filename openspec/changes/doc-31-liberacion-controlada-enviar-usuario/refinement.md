<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento aprobado — DOC-31 Liberación controlada de Enviar a usuario

## Fuente y alcance

- Ticket: `DOC-31` — Liberación controlada de Enviar a usuario.
- Cambio OpenSpec: `doc-31-liberacion-controlada-enviar-usuario`.
- Fuente funcional: `specs/liberacion-controlada-enviar-usuario/jira-context.md` y el dictamen DOC-30.
- Perfil tecnológico: ASP.NET Web Forms, VB.NET, ASMX y JavaScript legado; la actividad es documental y operativa.

DOC-31 prepara una decisión, matriz y runbook. No despliega, no edita configuración, no ejecuta E2E/carga ni utiliza secretos. La información disponible no nombra un ambiente de despliegue, ventana aprobada ni responsables nominales; esa ausencia es una condición de la decisión, no una autorización implícita.

## Contexto inspeccionado

- `Doc/Actualizacion/workflow/TerminarUsuario/03-verificacion-transversal-doc-30/`: dictamen técnico apto para solicitar aprobación operativa, compilación y CJS aprobados.
- PR #23 mergeado en `main` con SHA `43d42045beea0984c1b63193e66d12f6a49e5e1c`: referencia integrada de versión.
- `Doc/Actualizacion/workflow/TerminarUsuario/01-implementacion-envio-usuario/` y `02-documentacion-tecnica-doc-29/`: contratos de usuario, aislamiento y reversión por paquete.
- `tools/e2e/AGENT-RUNBOOK.md`: controles de solo lectura, gate inactivo y prohibición de almacenar secretos.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código o proceso | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | DOC-31 decide solicitar aprobación operativa y no desplegar. | Dictamen DOC-30 y ausencia de autorización de ambiente en Jira. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | La versión integrada se identifica por el merge `43d42045beea0984c1b63193e66d12f6a49e5e1c`. | PR #23 y rama `main`. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | La matriz declara cero ambientes elegibles hasta una solicitud aprobada por ambiente. | Ticket DOC-31 y matriz documental. | D-03 | RQ-02 | Origen: D-03, RQ-02 |
| D-04 | El runbook admite solo evidencia documental y `SELECT` autorizados. | `tools/e2e/AGENT-RUNBOOK.md` y documentación DOC-31. | D-04 | RQ-03 | Origen: D-04, RQ-03 |
| D-05 | La reversión restaura el paquete previo para intentos nuevos sin modificar transiciones confirmadas. | Arquitectura DOC-29 y runbook DOC-31. | D-05 | RQ-03 | Origen: D-05, RQ-03 |
| D-06 | La operación conserva la ruta moderna y el contrato `IdConector` de Continuar flujo. | Contratos y pruebas DOC-28/DOC-29. | D-06 | RQ-04 | Origen: D-06, RQ-04 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Existe una decisión única de liberación. | Sin autorización de ambiente, el resultado es solicitar aprobación operativa. | Impide despliegue accidental. |
| RQ-02 | La matriz no infiere ambientes ni responsables. | Cada ambiente solo es elegible cuando su solicitud identifica autorización, ventana, roles y evidencia. | Aísla autorizaciones entre ambientes. |
| RQ-03 | El runbook conserva controles de solo lectura y reversión segura. | Verifica versión y estado sanitizado; aborta o revierte el paquete por gestión aprobada. | No altera tareas ni respuestas confirmadas. |
| RQ-04 | La operación conserva compatibilidad funcional. | Usuario permanece moderno y Continuar flujo conserva `IdConector`. | Evita fallback legacy y regresión de transiciones. |

## Reglas de trazabilidad

1. D-01 a D-06 aparecen en `design.md`, `spec.md` y tareas con `Origen: D-XX, RQ-XX`.
2. Cada tarea tiene un área concreta, complejidad y una verificación independiente.
3. Las actividades operativas solo se ejecutan tras autorización explícita del ambiente y nunca se infieren de DOC-30.

## Resultado del refinamiento

- Estado: aprobado para construir documentación de liberación y validar su consistencia.
- Decisión vigente: solicitar aprobación operativa; ningún despliegue está autorizado.
