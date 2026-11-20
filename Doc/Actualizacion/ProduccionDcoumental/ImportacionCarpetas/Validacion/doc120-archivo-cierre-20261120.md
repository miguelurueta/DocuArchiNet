# DOC-120 — OpenSpec archivado y Jira cerrado

Continuación autorizada tras aceptar los límites y entregar la implementación en el [PR131](https://github.com/miguelurueta/DocuArchiNet/pull/131), merge `3fe73d6f`. Jira pasó de **En curso** a **Listo** mediante la transición 41; la lectura posterior confirmó la categoría `done` el 2026-11-20T22:08:53.584Z. No se enviaron comentarios de issue. El [registro JSON](doc120-archivo-cierre-20261120.json) conserva los datos de comprobación.

## Archivo integrado

El [PR132](https://github.com/miguelurueta/DocuArchiNet/pull/132) integró el archivo mediante `86e3fd61e5e3c72eab113da6d04af3388117372b`. El árbol integrado coincide con el revisado `73b4d816380d1b2a7aa1f3e2177b4a2461bf256b`; [CI](https://github.com/miguelurueta/DocuArchiNet/actions/runs/44825467841) terminó con 37 de 37 pasos aprobados. La carpeta principal quedó en `main` y se comprobó el mismo SHA en el remoto antes de cerrar Jira.

Se conservan los 17 archivos y las 270 tareas completas en `openspec/changes/archive/2026-11-20-doc-120-produccion-importacion-carga-carpeta-verificacion/`. El directorio activo ya no existe. La [especificación principal](../../../../../openspec/specs/produccion-importacion-carga-carpeta-verificacion/spec.md) contiene los 15 requisitos y 114 escenarios del delta, comparados bloque por bloque y resolviendo sus enlaces. Se preservó Purpose; el contexto Jira permanece en el archivo como contexto.

Se repararon los enlaces Markdown afectados. Los JSON históricos conservan sus rutas y cortes originales. La primera validación detectó un encabezado auxiliar que cortaba Requirements: el movimiento provisional se revirtió, se corrigió el formato y se validó la sincronización antes del archivo definitivo. Ninguna versión parcial se publicó.

## Verificación y alcance

Validación estricta global: **91/91**. Refinement archivado: **15/15**. Gobierno sobre el merge archivado: **72/72**, sin tareas pendientes. Las **195 entradas** de producto y pruebas mantienen sus hashes; se reutilizan los resultados existentes de 226 pruebas locales y las evidencias E2E con sus alcances documentados. El registro final documental vuelve a vincular la revisión al SHA comprometido en los archivos locales excluidos de Git.

El cierre conserva los [límites aceptados](doc120-aceptacion-limites-final-20261120.md): no añade certificación SII/limpieza, máximo de capacidad o SLA. Esta operación administrativa no ejecutó nuevas cargas reales, despliegues ni limpieza de datos. Los estados «archivo/cierre no ejecutados» de documentos anteriores corresponden a sus cortes históricos.
