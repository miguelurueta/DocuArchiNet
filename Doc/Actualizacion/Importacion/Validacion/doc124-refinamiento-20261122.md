# DOC-124 — Revisión de refinamiento

Fecha:2026-11-22. Revisor técnico:Codex. Rama:feature/DOC-124. Base local:b5f43dcb, sobre DOC123 integrado87d155b7. Este corte solo entrega planificación; no acredita producto, instalación ni ejecución real.

## Resultado

`openspec.cmd validate doc-124-importacion-excel-04-recepcion-preparacion --strict`:PASS.

`npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-124 --sync`:PASS,15/15 comprobaciones. Sincronización de trazabilidad sin sustituir el contenido del diseño/especificación/tareas.

Matriz:14 decisiones D-01..D-14,14 requisitos RQ-01..RQ-14 y31 escenarios.23 tareas (5S/18M/0L): una de refinamiento completa y22 de producto/verificación abiertas. Cada tarea tiene área, origen y verificación; no se marcan pruebas futuras como realizadas.

## Decisiones relevantes

Reutilizar conexión DA del módulo, conector y permisos propios DOC122, lector/fixtures DOC123 y harness/E2E existentes. La recepción necesita ID conocido antes del multipart para consultar un acuse perdido. El registro de22 columnas exige bytes/hash reales; no se insertan placeholders de recepción incompleta. Separa archivo publicado de commit conocido/incierto y conserva evidencia propia antes de cualquier recuperación explícita.

Preparación no modifica datos destino: valida archivo completo, esquema/índice UNIQUE íntegro y política empresarial explícita. Igualdad de claves se contrasta con temporales tipadas de MySQL; no se acredita por comparación ordinal. Se restaura sesión/limpian temporales antes de devolver conexión al pool; la implementación verificará descarte seguro con el conector actual, sin cambiarlo.

La huella de plantilla v1 se conserva dentro de un snapshot efectivo con hoja/modo, aislamiento y esquema. Vista previa25 filas/incidencias con totales observados y etapas no ejecutadas, sin afirmar validación completa cuando el lector rechaza datos. Repetir/preparar no equivale a confirmar ni retomar una migración por fila.

## SQL y evidencia real

No se prevé migración permanente nueva a partir de la revisión del contrato03. La implementación entregará11-preflight-doc124.sql de SELECT; cualquier diferencia permanente se entrega en otro archivo para ejecución manual y posterior verificación SELECT tras confirmación. Los scripts históricos no se reaplican por defecto.

E2E real sí aplica al recorrido de servicios ASHX/ASMX/MySQL/archivo de esta fase. Se exige alcance DOC124 propio, ambiente/cuentas/recursos descartables autorizados y controles SELECT del destino sin mutación. No se hereda la autorización cerrada de DOC122 ni se cambia esta prueba a no aplicable por falta de UI. La tarea real queda abierta y no bloquea trabajo local independiente.

## Límites de este corte

Se inspeccionaron código, scripts versionados, límites HTTP seleccionados y fuentes primarias del motor; no se leyeron secretos en evidencia. No se ejecutó SQL, autenticación, upload, preparación, importación, build o prueba de producto. Sin modificaciones a EGRESOS/CONTRATOS, MySql.Data, Web.config o DLL desplegada. No se solicita autorización de E2E antes de tener implementación y alcance revisables.

## Artefactos

- [Refinamiento](../../../../openspec/changes/doc-124-importacion-excel-04-recepcion-preparacion/refinement.md)
- [Diseño](../../../../openspec/changes/doc-124-importacion-excel-04-recepcion-preparacion/design.md)
- [Tareas](../../../../openspec/changes/doc-124-importacion-excel-04-recepcion-preparacion/tasks.md)

Contexto Jira original conservado en specs/importacion-excel-04-recepcion-preparacion/jira-context.md dentro del cambio. Las decisiones posteriores sobre conexión y conector tienen precedencia frente al antecedente de cuenta dedicada.
