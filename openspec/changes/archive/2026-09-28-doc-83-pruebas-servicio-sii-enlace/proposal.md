<!-- opsxj:refinement-traceability version=1 artifact=proposal decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Why

DOC-80, DOC-81 y DOC-82 entregaron consulta/preview, ejecución/reconciliación e interfaz ENLASE, pero el cierre exige demostrar de forma conjunta autorización, reversibilidad, compatibilidad legacy, ausencia de contaminación entre tareas y restauración del ambiente. Las evidencias históricas son insumos trazables; no sustituyen los casos transversales faltantes ni autorizan nuevas corridas reales.

## What Changes

- Consolidar un inventario ejecutable de riesgos, escenarios locales, integración y E2E para `ANEXOS_RADICADO_ENLASE`.
- Reutilizar los escenarios `import-sii-enlase-read`, `import-sii-enlase-execution` e `import-sii-enlase-ui`, sin crear login, transporte, perfil secreto ni sistema de evidencia paralelo.
- Incorporar únicamente cobertura transversal faltante para gate apagado, acceso rechazado, legacy, saneamiento, idempotencia, asignación explícita y restauración.
- Separar estrictamente lectura no mutadora, importación mutadora y asignación mutadora, cada una con autorización y recurso propios.
- Mantener `WorkflowCentroTrabajoModernActive` como alternancia existente; no crear otro gate ni retirar legacy en DOC-83.
- Publicar la configuración `Release` con el gate activo para toda sesión Workflow válida, sin listas de usuarios ni grupos; conservar la configuración base local/E2E apagada para pruebas reversibles.
- Incorporar una aceptación visual manual registrada dentro del runner, con navegador visible, autorización mutadora, tarea descartable, límite de diez minutos, controles antes/después y restauración en `finally`; no permitir activación manual aislada del gate.
- Documentar matriz de cierre, evidencia real reutilizada, limitaciones, rollout y rollback en la ruta canónica del ticket.
- Corregir la contradicción funcional hallada manualmente: una tarea devuelta con estado activo vuelve a ser operable, un recurso físico ausente vuelve a estar disponible y un antecedente físicamente vigente ofrece reimportación explícita en vez de un bloqueo definitivo.
- Corregir la proyección inmediata ENLASE sin postback: propagar desde el almacenamiento confirmado un DTO tipado con el contrato visual autoritativo y usar un adaptador exclusivo para `GridView_list_documento_relacion`, sin reutilizar la estructura de `GridView_list_documento_relacion_wf`.
- No modificar lógica productiva salvo que una prueba reproducible revele una contradicción material y el hallazgo se refine antes de corregirse.

## Capabilities

### New Capabilities
- `pruebas-servicio-sii-enlace`: Gobierno verificable de pruebas, evidencia, gate, compatibilidad y cierre para la capacidad moderna de anexos SII ENLASE.

### Modified Capabilities
- Ninguna.

## Impact

- Afecta pruebas Node, registro/perfiles/runner E2E existentes, validadores deterministas, documentación y evidencia OPSXJ.
- Reutiliza código productivo de DOC-80/81/82 y amplía de forma compatible la selección con `ReimportRequested`; no cambia asignación ni elimina documentos existentes.
- Toda E2E autenticada continúa condicionada a autorización explícita y secretos efímeros por TTY.
