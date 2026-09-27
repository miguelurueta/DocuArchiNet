<!-- opsxj:refinement-traceability version=1 artifact=proposal decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Why

DOC-80, DOC-81 y DOC-82 entregaron consulta/preview, ejecución/reconciliación e interfaz ENLASE, pero el cierre exige demostrar de forma conjunta autorización, reversibilidad, compatibilidad legacy, ausencia de contaminación entre tareas y restauración del ambiente. Las evidencias históricas son insumos trazables; no sustituyen los casos transversales faltantes ni autorizan nuevas corridas reales.

## What Changes

- Consolidar un inventario ejecutable de riesgos, escenarios locales, integración y E2E para `ANEXOS_RADICADO_ENLASE`.
- Reutilizar los escenarios `import-sii-enlase-read`, `import-sii-enlase-execution` e `import-sii-enlase-ui`, sin crear login, transporte, perfil secreto ni sistema de evidencia paralelo.
- Incorporar únicamente cobertura transversal faltante para gate apagado, acceso rechazado, legacy, saneamiento, idempotencia, asignación explícita y restauración.
- Separar estrictamente lectura no mutadora, importación mutadora y asignación mutadora, cada una con autorización y recurso propios.
- Mantener `WorkflowCentroTrabajoModernActive` como alternancia existente; no crear otro gate ni retirar legacy en DOC-83.
- Documentar matriz de cierre, evidencia real reutilizada, limitaciones, rollout y rollback en la ruta canónica del ticket.
- No modificar lógica productiva salvo que una prueba reproducible revele una contradicción material y el hallazgo se refine antes de corregirse.

## Capabilities

### New Capabilities
- `pruebas-servicio-sii-enlace`: Gobierno verificable de pruebas, evidencia, gate, compatibilidad y cierre para la capacidad moderna de anexos SII ENLASE.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta pruebas Node, registro/perfiles/runner E2E existentes, validadores deterministas, documentación y evidencia OPSXJ.
- Reutiliza código productivo de DOC-80/81/82 sin ampliar contratos ni cambiar persistencia o asignación.
- Toda E2E autenticada continúa condicionada a autorización explícita y secretos efímeros por TTY.