# DOC-123 — Revisión del refinamiento

Fecha:2026-11-22. Revisor técnico:Codex. Rama:feature/DOC-123. Base de esta revisión:f07455f0fe17b5ebb463772669ac447e2636cf44, sobre cierre DOC122 integrado en main36b10cc0. Documento de planificación, sin código de producto.

## Resultado

`npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-123 --sync`:PASS,15/15 comprobaciones. Sincroniza cabeceras de trazabilidad sin reemplazar las decisiones de design/spec/tasks.

`openspec.cmd validate doc-123-importacion-excel-03-lectura-validacion-xls-xlsx --strict`:PASS. `git diff --check`:PASS.

Comprobación local de matriz:12 decisiones D-01..D-12,12 requisitos RQ-01..RQ-12,33 escenarios;21 tareas (6S/15M/0L), una completada por este refinamiento y20 abiertas. Sin orígenes, áreas o verificaciones ausentes. La tarea de aceptación futura verifica producto/evidencia; no se considera cumplida con la validación del plan.

## Hallazgos de arquitectura

Se conserva DA del módulo y cuenta compartida de DOC122, huella v1, límites10000 filas/10000000 bytes, consumidores de SII y FastExcel. DOC123 entrega lector interno sin UI, endpoint o importación MySQL. Las transformaciones explícitas se recuperan del antecedente; no se reproducen eliminación de apóstrofes/n, conversión de barras, salto de primera fila o comprobación invertida de índice split.

El código oficial de ExcelDataReader3.6.0 devuelve null ante ciertos errores XLS/XLSX. La primera tarea de producto caracteriza errores, NULL y fórmulas en ambos formatos; la integración queda condicionada a distinguirlos con estrategia documentada/probada. Este hallazgo no está resuelto por el refinamiento ni por elegir el paquete. Fuentes versionadas y criterios detallados en design.md.

## Límites de evidencia

No se implementó lector, restauraron paquetes, compilaron cambios de producto ni ejecutaron pruebas de lectura. No se ejecutó autenticación, E2E real, SQL, carga, despliegue o modificación de EGRESOS/CONTRATOS. El plan reutiliza harness DOC122/DOC110 y conserva tools/e2e para fases con recorrido; las ventanas reales anteriores permanecen cerradas. Esta evidencia acredita revisión documental y trazabilidad, no aceptación del lector.

## Artefactos

- [Refinamiento](../../../../openspec/changes/doc-123-importacion-excel-03-lectura-validacion-xls-xlsx/refinement.md)
- [Diseño](../../../../openspec/changes/doc-123-importacion-excel-03-lectura-validacion-xls-xlsx/design.md)
- [Tareas](../../../../openspec/changes/doc-123-importacion-excel-03-lectura-validacion-xls-xlsx/tasks.md)

El contexto Jira original se conserva en specs/importacion-excel-03-lectura-validacion-xls-xlsx/jira-context.md dentro del cambio; la planificación vigente identifica explícitamente las decisiones posteriores de DOC122 que sustituyen la cuenta dedicada inicial.
