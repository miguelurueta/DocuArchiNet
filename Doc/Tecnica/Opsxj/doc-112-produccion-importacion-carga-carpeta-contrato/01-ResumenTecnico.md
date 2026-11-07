# DOC-112 — Entrega documental de contratos

- Ticket: DOC-112
- Cambio OpenSpec: doc-112-produccion-importacion-carga-carpeta-contrato
- Clasificacion: cross_cutting (Transversal)

## Objetivo

Consolidar los contratos de importación por carpetas ya implementados por DOC-110/111. El plan inicial se redujo de 52 revisiones a doce diferencias documentales concretas; las doce están aplicadas. Se corrigieron inventario, modelo físico, operaciones públicas, referencias de snapshot/valores, estado de coordinación, aceptación de adaptadores y pendientes históricos.

## Alcance y compatibilidad

Los contratos completos permanecen en [Diseno/](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/design.md); este dossier es un índice de entrega. No modifica VB/JS, DTO, SQL ejecutable, Web.config, permisos, SII, almacenamiento legacy ni herramientas E2E. No ejecuta login, consultas autenticadas, DDL, carga o MSBuild. Los recursos y las aceptaciones de DOC-111 se conservan con sus límites.

La reversa consiste en revertir exclusivamente el diff documental de DOC-112; no hay migración o rollback del producto. Base integrada de referencia main 40c5f55e; los cambios locales se describen por archivos/huellas, no solo por HEAD. [Acta de revisión](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc112-contratos-revision.md) y [plan completo](../../../../openspec/changes/archive/2026-11-02-doc-112-produccion-importacion-carga-carpeta-contrato/tasks.md).
