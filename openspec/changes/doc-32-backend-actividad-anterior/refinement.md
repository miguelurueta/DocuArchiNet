<!-- opsxj:refinement version=1 state=draft -->

# Refinamiento - doc-32-backend-actividad-anterior

## Fuente y alcance

- Ticket: `DOC-32` — BACKEND-ACTIVIDAD-ANTERIOR
- Cambio OpenSpec: `doc-32-backend-actividad-anterior`
- Fuente Jira: `specs/*/jira-context.md`
- Perfil tecnologico: no definido; no introducir reglas de framework hasta identificar la tecnologia afectada.

Este artefacto es la compuerta entre el ticket y la implementacion. No se aprueba por generacion automatica: una persona responsable debe confirmar alcance, decisiones, compatibilidad y evidencia de codigo.

## Contexto inspeccionado

- [PENDIENTE: rutas, clases, handlers, scripts y datos legacy inspeccionados]
- [PENDIENTE: comportamiento actual y compatibilidad que se debe preservar]

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | [PENDIENTE: decision concreta] | [PENDIENTE: ruta y simbolo] | D-01 | RQ-01 | Origen: D-01, RQ-01 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | [PENDIENTE: resultado] | [PENDIENTE: WHEN/THEN] | [PENDIENTE: regresion y rollback] |

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.
3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.
4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.

## Resultado del refinamiento

- Estado: borrador. Sustituya el marcador inicial por el estado `approved` despues de completar y revisar la matriz.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- <ISSUE-KEY> --sync`.
