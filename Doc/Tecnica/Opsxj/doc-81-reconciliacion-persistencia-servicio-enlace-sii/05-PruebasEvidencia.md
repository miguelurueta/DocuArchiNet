# RECONCILIACION-PERSISTENCIA-SERVICIO-ENLACE-SII

- Ticket: DOC-81
- Cambio OpenSpec: doc-81-reconciliacion-persistencia-servicio-enlace-sii
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

- unit — PASS, 2026-09-26: suites de importación, políticas E2E, OPSXJ, documentación y firmas Roslyn aprobadas; las correcciones de operabilidad, preparación ENLASE y evidencia cuentan con pruebas focales adicionales.
- build — PASS, 2026-09-26: MSBuild del proyecto, 0 errores.
- OpenSpec — PASS, 2026-09-26: validación estricta y refinement DOC-81 aprobados.
- manual_qa — PASS MULTIDOCUMENTO, 2026-09-26: `import-sii-enlase-execution` terminó correctamente sobre el recurso descartable autorizado 220587 con `sampleSize=2`; verificó 7 controles, una intención idempotente, una ejecución y dos documentos con evidencia física confirmada `2/2`.
- diagnóstico — CONFIRMADO, 2026-09-26: SII respondió `ANNEX_SUCCESS`; el esquema fue desplegado manualmente y las corridas posteriores superaron consulta y preflight. Se corrigieron además la operabilidad previa a asignación ENLASE y la separación de metadatos de constancias.
- seguridad operacional — PASS: sin secretos; gate restaurado a `false`, usuarios y grupos vacíos; controles SQL exclusivamente `SELECT`.

## QA/E2E WebForms

La infraestructura real se ejecuta mediante:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-enlase-execution --profile <perfil-runtime.json> --authorize environment,gate,execution,discardable-resource
```

El runner solicita secretos por TTY, usa controles `SELECT`, selecciona el contexto oficial ENLASE, valida intención idempotente, evidencia física, ausencia de expediente y restaura el gate en `finally`.

La ejecución multidocumento sobre 220587 terminó correctamente con dos anexos disponibles dentro de una sola intención. Confirmó creación idempotente `1/1`, ejecución única `1/1`, evidencia física `2/2`, ausencia de efectos de expediente `0/0`, tarea sin transición y saneamiento. Cambiaron únicamente intención, elementos y auditoría; relación, expediente, índices y caché permanecieron sin cambios. El gate terminó en `false`, con usuarios y grupos vacíos.
