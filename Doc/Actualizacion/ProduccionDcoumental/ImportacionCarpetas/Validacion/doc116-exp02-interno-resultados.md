# DOC-116 EXP02 interno — aceptación autenticada de laboratorio

PASS revisado el 2026-11-05. Ambas ventanas internas se ejecutaron con sesión HTTP real 141/2, llamadas legacy y repositorio reales, bajo la variante de laboratorio expresamente autorizada. Reutilizado plan 34/lote 18/nivel 215/versión 38 y sus dos originales.

| Raíz | Intento | Expediente / relación | Ventana y resultado |
| --- | --- | --- | --- |
| 29 | 40 | 660 / 1085 | Excepción al devolver checkpoint ya confirmado; coordinador recuperó histórico; replay mismos IDs y cero efectos adicionales |
| 30 | 41 | 661 / 1086 | Excepción después del legacy y antes del checkpoint; público INCIERTO, intento persistido EN_CURSO; no se repitió ni reconcilió la creación |

Tres solicitudes en orden 29,29,30 y dos expedientes nuevos. Cero planes/lotes/recepciones adicionales, documentos definitivos y cambios de permisos. Las marcas privadas correlacionan lote/raíz/intento/IDs y punto exacto del fallo; no hay atribución por nombre. Verificados por SELECT relación, nivel, actor, empresa, clasificación, Archivo Gestión y ausencia de documentos definitivos.

La marca de raíz 30 contiene estado CONFIRMADO del resultado legacy verificado en memoria; no implica checkpoint persistido. El intento 41 queda deliberadamente pendiente de verificación. No repetir AvanzarLote para esa raíz, no recrear el expediente y no escribir un checkpoint manualmente. Cualquier recuperación adicional necesita procedimiento revisado y autorización aparte. El caso exigía reconciliar solo evidencia suficiente o conservar incertidumbre sin duplicar; se acreditó esta segunda salida.

## DLL original preservada y restaurada

DLL original/restaurada: 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5. DLL de laboratorio utilizada: e26242b901043394bd1dc38063becfe399cd093738cfd10b7ebca24785d8944a. Diario final RESTORED y reapertura/consulta con archivo original restaurado verificadas durante la ejecución. Esta revisión confirmó además igualdad byte por byte entre DLL de IIS y ambas copias:

- `.tmp\doc116-internal-runtime\original.dll` (9627648 bytes, SHA256 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5).
- `.tmp\doc116-internal-reserve\original.dll` (9627648 bytes, SHA256 4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5).

Los respaldos se conservan; no se limpiaron los originales, staging ni marcas. La candidata no está activa.

## Procedencia y límites

Checkpoint saneado: `tools/e2e/artifacts/doc116-internal-lote18.json`, SHA256 b8b5b74a5d4684fb743328d083db9581aec2aa936a8e1a124ec89f28bcfe483c. Código revisado: d3b8db793b211686db40cd3495df9ecc520254c2aaac48caf3081789643f5fa8. Marcas y hashes están en `doc116-exp02-interno-resultados-20261105.json`; los dos fuentes instrumentados exactos se conservan en AnexosDOC116 como test-only, fuera de los Compile del proyecto normal. No se presentan como código instalado en producción. Pruebas locales previas: 110/110 PASS.

Se acredita excepción interna después del commit y antes de devolver su resultado; no se afirma pérdida de paquetes de red. El fallo previo a checkpoint conserva los IDs fiables en evidencia privada pero no fabrica un checkpoint SQL. Los recursos previos fuera del lote objetivo conservaron sus seis huellas; el lote 18 sí cambió conforme al presupuesto.

La brecha funcional DOC116-VERIFY-01 queda resuelta para las ventanas autorizadas. Siguen pendientes los pasos formales de entrega/revisión de cierre/publicación; no se marca Jira Listo ni se archiva automáticamente.
