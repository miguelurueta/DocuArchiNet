# Liberación y operación controlada

## Precondiciones

- Aplicar manualmente el DDL aprobado después de inspecciones `SELECT` del esquema objetivo.
- Confirmar `workflow_notas_version` en InnoDB y el preflight del repositorio.
- Mantener `WorkflowCentroTrabajoModernActive=false` con usuarios y grupos vacíos.
- Ejecutar E2E solo con ambiente, cuenta y tareas descartables autorizados.

## Decisión

DOC-42 queda técnicamente listo para revisión. No habilita consumidores ni UI y no implica publicación o activación operativa.

## Reversión

Retirar el despliegue moderno mediante el procedimiento aprobado, sin alterar notas existentes. El `DROP TABLE workflow_notas_version` solo procede con autorización y si el inventario confirma que fue introducida por esa corrida; no degradar motores InnoDB ni borrar datos de negocio.
