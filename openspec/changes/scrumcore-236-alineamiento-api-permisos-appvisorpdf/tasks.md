# Tasks: SCRUMCORE-236 - Alineamiento API permisos AppVisorPDF

## 1. Refinement y alcance

- [x] 1.1 Consolidar alcance desde Jira y diagnostico de codigo actual.
- [x] 1.2 Reemplazar propuesta generica por propuesta enterprise accionable.
- [x] 1.3 Refinar design con decisiones, responsabilidades, riesgos y no-goals.
- [x] 1.4 Refinar spec con requirements verificables.
- [x] 1.5 Refinar tasks con plan ejecutable antes de publish.

## 2. Implementacion funcional

- [x] 2.1 Actualizar `AppVisorEmbedPdf.service.ts` para consumir envelope `{ success, message, data, meta, errors }`.
- [x] 2.2 Retornar `envelope.data` desde `fetchMisPermisosVisorPdf`.
- [x] 2.3 Rechazar contrato invalido cuando falte `success=true` o `data.Permissions`.
- [x] 2.4 Mantener `AbortSignal` en la consulta de permisos.
- [x] 2.5 Agregar/ajustar tipos de permisos documentados en `AppVisorEmbedPdf.permissions.ts`.
- [x] 2.6 Confirmar `resolveCodigoImplementacion("gestioncorrespondencia") -> "gestion_correspondencia"`.
- [x] 2.7 Actualizar mapping de permisos backend:
  - [x] `pdf.print` -> `allowPrint`
  - [x] `pdf.download` -> `allowExport`
  - [x] `pdf.annotate.signature.place` -> `allowSignaturePlacement`
  - [x] `pdf.annotate.signature.delete` -> `allowSignatureDelete`
  - [x] `pdf.annotate.signature.lock/unlock` -> `allowSignatureLockToggle`
  - [x] `pdf.annotate.*` de firma -> `allowAnnotationEdit`
- [x] 2.8 Mantener fail-closed para errores o permisos vacios.
- [x] 2.9 Mantener debug log de permisos solo bajo `window.__DV_DEBUG__` o retirarlo si no se requiere.
- [x] 2.10 Revisar `DocumentosWorkbench.tsx` para confirmar que no consulta permisos ni envia `idUsuario`.
- [x] 2.11 Preservar metadata del contrato (`CodigoImplementacion`, `IdUsuario`, `Sources`, `GeneratedAt`, `success`, `message`, `meta`, `errors`) en tipos, logs/debug o documentacion sin cambiar el payload operativo del visor.
- [x] 2.12 Confirmar que `pdf.view`, `pdf.zoom` y `pdf.rotate` quedan documentados pero no conectados a nuevas capacidades UI en esta iteracion.

## 3. Documentacion enterprise

- [x] 3.1 Crear `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Metadata.md`.
- [x] 3.2 Crear `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Arquitectura.md`.
- [x] 3.3 Crear `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Contrato-API.md`.
- [x] 3.4 Crear `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Implementacion-Detallada.md`.
- [x] 3.5 Crear `docs/Architecture/AlineamientoContratoApiPermisosVisor/SCRUMCORE-236-Pruebas.md`.
- [x] 3.6 Crear `docs/Architecture/AlineamientoContratoApiPermisosVisor/PROMPT-SCRUMCORE-236-AlineamientoContratoApiPermisosVisor.md`.

## 4. Pruebas

- [x] 4.1 Crear/actualizar `AppVisorEmbedPdf.service.test.ts`.
- [x] 4.2 Crear/actualizar `AppVisorEmbedPdf.permissions.test.ts`.
- [x] 4.3 Validar tests existentes de `AppVisorEmbedPdf`.
- [x] 4.4 Ejecutar vitest focalizado de service, permissions, visor y toolbar.
- [x] 4.5 Ejecutar lint focalizado de service y permissions.
- [x] 4.6 Registrar evidencia en documentacion enterprise.
- [x] 4.7 Validar que el service no acepta silenciosamente `Permissions` en raiz salvo fallback transitorio documentado explicitamente.
- [x] 4.8 Validar que el endpoint `mis-permisos` no recibe `idUsuario` en path, query ni body.

## 5. Validacion manual / QA

- [ ] 5.1 Activar `window.__DV_DEBUG__ = true`.
- [ ] 5.2 Abrir documento desde Gestion Correspondencia.
- [ ] 5.3 Confirmar request a `/implementaciones/gestion_correspondencia/mis-permisos`.
- [ ] 5.4 Confirmar log con `response`, `raw` y `effective`.
- [ ] 5.5 Confirmar que `pdf.download=true` habilita export/download.
- [ ] 5.6 Confirmar que `pdf.print=true` habilita print.
- [ ] 5.7 Confirmar fail-closed si API falla o devuelve contrato invalido.

## 6. Cierre

- [x] 6.1 Validar OpenSpec.
- [x] 6.2 Commit y push de implementacion.
- [ ] 6.3 Crear PR.
- [ ] 6.4 Registrar PR en Jira.
- [ ] 6.5 Archivar change despues del merge y evidencia final.

## 7. Registro de ejecucion realizada

- [x] 7.1 Implementacion de service alineado al envelope oficial `{ success, message, data, meta, errors }`.
- [x] 7.2 Implementacion de tipos documentados para permisos oficiales del visor PDF.
- [x] 7.3 Implementacion de mapping frontend con claves oficiales `pdf.download`, `pdf.print` y `pdf.annotate.signature.*`.
- [x] 7.4 Implementacion de fail-closed ante permisos vacios, API fallida o contrato invalido.
- [x] 7.5 Implementacion de debug log protegido por `window.__DV_DEBUG__`.
- [x] 7.6 Confirmacion de que `DocumentosWorkbench` no envia `idUsuario` ni consume permisos directamente.
- [x] 7.7 Creacion de documentacion enterprise completa en `docs/Architecture/AlineamientoContratoApiPermisosVisor/`.
- [x] 7.8 Creacion de pruebas unitarias para service de permisos.
- [x] 7.9 Creacion de pruebas unitarias para resolver/mapping de permisos.
- [x] 7.10 Ejecucion de tests focalizados nuevos: `12/12 passed`.
- [x] 7.11 Ejecucion de regresion focalizada de visor/toolbar: `20/20 passed`.
- [x] 7.12 Ejecucion de lint focalizado: passed.
- [x] 7.13 Ejecucion de OpenSpec strict validate: passed.
- [ ] 7.14 Pendiente QA manual con backend real y `window.__DV_DEBUG__ = true`.
- [ ] 7.15 Pendiente cierre Git/Jira: commit, push, PR, registro Jira y archive post-merge.
