# DOC-117 — Entrega reconciliada, 2026-11-07

Estado posterior por instrucción del usuario: la entrega se integra y valida en la carpeta principal, que es la fuente de verdad. Este informe conserva el paso previo del worktree; el resultado vigente está en `sincronizacion-local-20261107.md`.

El trabajo final de DOC-117 se reúne en el worktree `.tmp/doc117-work`, rama `feature/DOC-117`, cuyo HEAD anterior era `04688b86f3446f739d0198ecc37ad39bec0ff022`. La carpeta principal permanece en `feature/DOC-105`, HEAD `b2e93b726f221868cf2b82d43200a8e568871117`; sus archivos locales se conservaron.

## Procedencia y selección

Se compararon 258 archivos de DOC-117 y sus dependencias explícitas: cambio OpenSpec, documentación propia, pruebas, fixtures y soporte de laboratorio. La comparación recorrió también imports locales estáticos y el inventario de la regresión final histórica. Se transfirieron 204 archivos con diferencias de contenido o de representación; cada sobrescritura conservó previamente los bytes de destino en `.tmp/doc117-reconciliation-20261107/backup` de la carpeta principal. El plan y la constancia de aplicación están junto a ese respaldo. La transferencia no eliminó archivos.

Se preservaron las versiones más recientes de DOC-113/DOC-114 presentes en la rama: `doc110-permission-revocation.cjs`, `doc111-partial-profile.cjs` y el runbook. Los 89 scripts npm previos se conservan con sus valores; únicamente se añade `test:doc117:local`. Los cambios compartidos incorporados pertenecen a la observación de carga individual y la comprobación por IDs conocidos de DOC-117.

Las nueve fuentes documentales de `source-review.json` mantienen sus huellas normalizadas. La modificación del test C# es la ampliación ya documentada de conservación de bytes y rechazo TIFF. El proyecto de aplicación, su escritor ordinario y los archivos de negocio quedan fuera del diff de esta entrega. No se transfirieron staging, PDFs generados de usuarios, credenciales, configuraciones locales ni DLL instaladas.

## Evidencia y pruebas

Las 63 referencias de `sourceEvidence` y `finalReadbackReviewedFiles` del certificado final ALM-03C se verificaron por SHA-256 contra sus originales en la carpeta principal. Las referencias públicas y fuentes transferidas coinciden también en el worktree. Los artefactos primarios ignorados de `.tmp` y `tools/e2e/artifacts` permanecen en su ubicación histórica; sus certificados públicos conservan las rutas y hashes originales. No se editó ni trasladó una autorización consumida.

La primera regresión en el worktree produjo 429 PASS y 24 fallos por dependencia de recursos históricos privados o rutas de la instalación original. Esas dependencias de pruebas se sustituyeron por una historia sintética en memoria, con scopes vinculados a la ubicación del checkout. Los loaders reales siguen verificando hashes de candidato, fuentes y 912 entradas sintéticas; se añadió comprobación de rechazo de bytes y fuentes alterados. Los fixtures nativos existentes continúan compilando y ejecutando código .NET real con almacenamiento y persistencia simulados. Los dos casos que validan concesiones de preparación usan descriptores sintéticos y la calculadora simulada; no afirman una nueva validación de la instalación histórica completa. Las fuentes de los runners no se relajaron para admitir el traslado.

Resultado final: **454/454 PASS**, 49 archivos, cero fallos, cancelaciones u omisiones. Comando reproducible desde este worktree:

```powershell
npm.cmd --prefix tools/e2e run test:doc117:local
```

Inventario versionado: `tools/e2e/fixtures/doc117-local-tests.json`. Log local: `.tmp/doc117-local-reconciled-20261107.tap`. Requiere Node y los compiladores/dependencias .NET ya usados por el arnés del repositorio. No inicia navegador, autenticación, consultas reales, cargas ni instalación de variantes. El control estructurado de los JSON seleccionados no encontró campos sensibles con valores persistidos; las evidencias públicas se mantienen saneadas.

## Revisión OpenSpec y compuertas

| Dimensión | Resultado |
| --- | --- |
| Completitud | 18/18 tareas; RQ-01–RQ-08 y D-01–D-08 conservados |
| Corrección | Regresión local y evidencia histórica ALM-01/02/03C diferenciadas |
| Coherencia | Sin sustitución del escritor; variantes de laboratorio separadas y mejoras anteriores preservadas |

ALM-03C conserva el alcance real histórico: documentos 16225/16226/16227, inventarios 23323/23324/23325, solapamiento interno de 99,7347 ms y 581/659 SELECT acumulados. No acredita concurrencia XML/SII, commits simultáneos o rendimiento ordinario. Las cargas y lecturas reales no se repitieron durante esta reconciliación. Sin hallazgos críticos en el alcance revisado.

`verificacion-20261107.md` describe la revisión anterior efectuada sobre archivos locales de la carpeta principal y no acredita el commit de esta entrega. Las compuertas asociadas al HEAD de DOC-105 se retiran mediante eventos adicionales de la bitácora, conservando su historial. Las nuevas compuertas se ejecutan en `feature/DOC-117` después de crear el commit local, sin copiar bitácoras entre worktrees. La identidad efectiva de la validación consta en `.opsxj/runs/DOC-117.json` de este worktree y en su HEAD; debe renovarse al cambiar el SHA.

Esta entrega prepara revisión y archivo posteriores. El commit local y la validación no publican la rama, crean un PR, archivan el cambio ni modifican Jira.
