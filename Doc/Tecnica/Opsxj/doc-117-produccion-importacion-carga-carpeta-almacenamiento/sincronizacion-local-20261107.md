# DOC-117 — Integración en el repositorio principal, 2026-11-07

Por instrucción expresa del usuario, la carpeta principal `D:/imagenesda/DocuachiNet/DocuArchiNet` tiene prioridad y concentra la entrega. La rama de trabajo es `sync/local-20261107`. Los worktrees permanecen como auxiliares y antecedentes; no son el destino final de este trabajo.

## Conservación e integración

Antes de integrar se respaldaron los 1022 archivos modificados o nuevos observados: 916 de código, pruebas y documentación, y 106 de datos generados. El inventario con SHA-256 y el respaldo están en `.tmp/primary-sync-20261107` de la carpeta principal. El commit `10a90155cb11c565ba18cecb101b6b4bee650eb1` conserva los cambios locales versionables anteriores a la integración.

La integración reúne ese snapshot, la historia de `origin/main` fijada en `9a1bce2be9cfd640184269c0e02d6ef94ff181f6` y la entrega DOC-117 `31a24d06031d960e753482829c988b46ef24faab`. Se revisaron 26 conflictos. El código ordinario de aplicación coincide con la entrega integrada; se conservaron dos avances específicos de la carpeta principal: el modo `Doc111Creation` del inspector de requisitos y la reutilización del soporte común de restauración en `doc116-runtime-lease.cjs`. Se incorporaron las pruebas portables de DOC-117 y las mejoras previas de DOC-113/DOC-114, sin sustituir esos avances locales.

El informe arquitectónico local `doc116-120-revision-arquitectonica-20261104.md` y el reporte de revisión local también quedan versionados. Los otros worktrees no presentan fuentes de negocio pendientes: DOC-111 conserva seis logs locales y DOC-115 un reporte generado; no se elimina su contenido.

Los datos generados se conservan físicamente. `.gitignore` excluye App_Data, temporales de Gestión/Radicación y DIRECTROY. El único archivo generado modificado que ya estaba en Git, `Temp_Gestion/141/IMPRESION/temp_141.pdf`, se retira únicamente del índice: sus bytes locales permanecen. No se versionan credenciales, staging ni documentos generados del usuario.

## Verificación desde la carpeta principal

- `npm.cmd --prefix tools/e2e run test:doc117:local`: 454/454 PASS, 49 archivos, sin omisiones. Log `.tmp/primary-sync-20261107/local-regression.tap`.
- `node --test tests/doc116-runtime-lease.test.cjs tests/doc111-importacion-carpetas-client.test.cjs`: 25/25 PASS, sin omisiones. Log `.tmp/primary-sync-20261107/local-priority-regression.tap`. La UI usa Chromium local con respuestas sintéticas, sin autenticación o tráfico HTTP.
- `openspec.cmd validate doc-117-produccion-importacion-carga-carpeta-almacenamiento --strict`: PASS.

Son dos grupos disjuntos ejecutados en esta integración: 479 pruebas locales en total. No se suman corridas históricas. Las pruebas .NET usan fuentes y compiladores reales con almacenamiento/persistencia simulados; no se repiten cargas reales, consultas de control o instalación de DLL. Los resultados reales históricos de ALM-01/02/03C mantienen sus certificados, hashes y límites.

`AGENTS.md` deja registrada la prioridad del repositorio principal. `package.json` expone desde esta carpeta `npm.cmd run opsxj:status -- DOC-117` y `npm.cmd run test:doc117:local`.

## Identidad y sincronización

Las compuertas se vuelven a registrar en la carpeta principal contra el SHA del commit de integración, sin reutilizar la validación de DOC-105 o la del worktree. La bitácora local `.opsxj/runs/DOC-117.json` y `git rev-parse HEAD` identifican el resultado. Una modificación posterior del SHA exige renovar esa validación.

La sincronización remota de `sync/local-20261107` se comprueba comparando el HEAD local con su rama en origin y el contador de commits exclusivos de cada lado. El snapshot local y los dos antecedentes integrados deben permanecer como ancestros. Este paso no archiva OpenSpec ni modifica Jira.
