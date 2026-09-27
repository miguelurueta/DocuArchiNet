# DOC-83 — Pruebas, gate y compatibilidad ENLASE

DOC-83 valida transversalmente `ANEXOS_RADICADO_ENLASE` sin cambiar lógica productiva. Reutiliza las corridas autorizadas de DOC-80, DOC-81 y DOC-82, añade acceso negativo y registra una etapa independiente para accionar la asignación legacy con control autoritativo.

## Resultado implementado

- Manifiesto: `tools/e2e/validation/doc83-sii-enlase-closure-matrix.json`.
- Validador: `tools/e2e/scripts/support/doc83-sii-enlase-closure-matrix.cjs`.
- Prueba positiva y negativas controladas: `tools/e2e/tests/doc83-sii-enlase-closure-matrix.test.cjs`.
- Regresión consolidada: `npm.cmd --prefix tools/e2e run test:doc83:regression`.
- Escenarios reutilizados: `import-sii-enlase-read`, `import-sii-enlase-execution` e `import-sii-enlase-ui`.
- Escenarios DOC-83: `import-sii-enlase-anonymous` e `import-sii-enlase-assignment`.

## Estado de cierre

La cobertura determinística y la evidencia histórica real están diferenciadas. DOC-83 confirmó los dos caminos de asignación: `BLOCKED` mantuvo la tarea sin cambios cuando faltaba el documento obligatorio y, después de importar de forma autorizada `Recibo De Caja`, `ASSIGNED` produjo la transición esperada. Importación y asignación conservaron autorizaciones y reservas E2E independientes.

`tools/e2e/AGENT-RUNBOOK.md` documenta el nuevo escenario, sus autorizaciones reutilizadas y la interpretación cerrada de `BLOCKED`/`ASSIGNED`.

## Documentos

- `01-INVENTARIO-Y-ALCANCE.md`: componentes y comportamiento preservado.
- `02-MATRIZ-PRUEBAS-Y-RIESGOS.md`: riesgos, cobertura y brechas reales.
- `03-GATE-ROLLOUT-ROLLBACK.md`: seguridad, alternancia y recuperación.
- `04-EVIDENCIA-Y-LIMITACIONES.md`: comandos, resultados y límites probatorios.
