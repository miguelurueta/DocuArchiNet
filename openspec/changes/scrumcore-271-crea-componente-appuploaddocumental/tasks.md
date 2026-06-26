## 1. Refinement

- [x] 1.1 Leer Jira context y referencias obligatorias de AppUploadDocumental.
- [x] 1.2 Revisar APIs reales de `AppUpload`, `AppUploadBatchView`, `AppProgressBatch` y storage client SCRUMCORE-272.
- [x] 1.3 Corregir proposal: capability `crea-componente-appuploaddocumental`, ubicacion real y dependencia con SCRUMCORE-272.
- [x] 1.4 Reescribir design con decisiones, arquitectura, riesgos y plan implementable.
- [x] 1.5 Reescribir spec como requisitos verificables.
- [x] 1.6 Validar OpenSpec estricto antes de implementar.
- [ ] 1.7 Verificar disponibilidad de DTOs/backend externos indicados en el prompt y documentar evidencia o bloqueo si no son accesibles.

## 2. Scaffold

- [ ] 2.1 Crear `src/modules/almacenamientoDocumental/components/AppUploadDocumental/`.
- [ ] 2.2 Crear `AppUploadDocumental.tsx`, `AppUploadDocumental.types.ts`, `AppUploadDocumental.module.css`, `README.md` e `index.ts`.
- [ ] 2.3 Crear hooks `useAppUploadDocumentalState.ts` y `useAppUploadDocumentalActions.ts`.
- [ ] 2.4 Crear/actualizar tests focales del componente y hooks.
- [ ] 2.5 Verificar que no se crean componentes fuera del modulo esperado.

## 3. Contracts

- [ ] 3.1 Definir `UploadDocumentalProcessKey`.
- [ ] 3.2 Definir `UploadDocumentalContext`.
- [ ] 3.3 Definir `UploadDocumentalConfig`.
- [ ] 3.4 Definir `TipoDocumentalOption`.
- [ ] 3.5 Definir `UploadDocumentalFileMetadata`.
- [ ] 3.6 Definir `UploadDocumentalInterfaceRegistration`.
- [ ] 3.7 Definir `AlmacenarDocumentoStoredResult`.
- [ ] 3.8 Definir `UploadDocumentalBatchSummary`.
- [ ] 3.9 Definir `AppUploadDocumentalProps` con `loadConfig` y `loadTiposDocumentales` obligatorios.
- [ ] 3.10 Garantizar cero `any` nuevo; usar `unknown` para shapes no modelados.

## 4. Services and Mappers

- [ ] 4.1 Crear `uploadDocumentalInterfaceRegistration.mapper.ts`.
- [ ] 4.2 Implementar `buildUploadDocumentalInterfaceRegistration`.
- [ ] 4.3 Mapear variantes conocidas: production, related, workflow, migration, counters, traffic-light, dropdown, version, table import.
- [ ] 4.4 Implementar fallback `{ kind: "raw" }` solo cuando haya dato util.
- [ ] 4.5 Agregar tests aislados del mapper.
- [ ] 4.6 Crear contratos/adaptadores `uploadConfig.service.ts` y `tipoDocumental.service.ts` solo si no inventan endpoints.
- [ ] 4.7 Verificar que `clienteApi` no se importe desde componente/hooks.

## 5. Utils

- [ ] 5.1 Crear `tipoDocumentalSuggestion.utils.ts`.
- [ ] 5.2 Implementar normalizacion/tokenizacion de nombres.
- [ ] 5.3 Implementar score y umbral configurable.
- [ ] 5.4 Garantizar que la sugerencia no sobreescribe seleccion manual.
- [ ] 5.5 Implementar/ubicar validacion de fecha `yyyy-MM-dd`, fecha real y no futura.
- [ ] 5.6 Implementar helpers para construir metadata/payload final por archivo si aplica.
- [ ] 5.7 Agregar tests de sugerencia, fecha y payload.
- [ ] 5.8 Reutilizar `storageFile.utils` existente de SCRUMCORE-272 para extension/chunks cuando aplique, sin duplicar logica.

## 6. State Hook

- [ ] 6.1 Cargar config al montar/cambiar `proceso`, `context.nombreGabinete` o `modoDocumento`.
- [ ] 6.2 Cargar tipologias al montar/cambiar `proceso` o `context`.
- [ ] 6.3 Deshabilitar seleccion si config falla o falta `nombreGabinete`.
- [ ] 6.4 Mantener cola por `uid` con metadata independiente.
- [ ] 6.5 Soportar seleccion activa y preview.
- [ ] 6.6 Soportar eliminar archivo y limpiar todos.
- [ ] 6.7 Revocar object URLs al remover/limpiar/desmontar.
- [ ] 6.8 Aplicar politica anti-stale con `operationId` o token equivalente.

## 7. Validation Behavior

- [ ] 7.1 Aplicar `accept`, extensiones y `maxSizeBytes` desde config.
- [ ] 7.2 Soportar `validationMode="reject"`.
- [ ] 7.3 Soportar `validationMode="queue-with-error"`.
- [ ] 7.4 Validar tipologia obligatoria.
- [ ] 7.5 Validar fecha requerida e invalida.
- [ ] 7.6 Bloquear guardar por archivo cuando metadata del archivo es invalida.
- [ ] 7.7 Mostrar errores accionables por fila.

## 8. Component UI

- [ ] 8.1 Renderizar `AppUploadBatchView` con titulo, resumen, toolbar, lista, metadata, preview y footer.
- [ ] 8.2 Usar `renderMetadata` para tipologia y fecha.
- [ ] 8.3 Usar `AppInputSelect`/wrapper existente para tipologia.
- [ ] 8.4 Usar `AppInput`/wrapper existente para fecha si aplica.
- [ ] 8.5 Mostrar acciones por fila: ver, eliminar, guardar individual cuando aplique.
- [ ] 8.6 Mostrar guardar todos y eliminar todos.
- [ ] 8.7 Garantizar layout responsive sin tabla DOM manual, hero, gradientes ni cards decorativas.
- [ ] 8.8 Agregar `aria-label`/nombres accesibles en acciones.
- [ ] 8.9 Usar `AppButton` o wrapper existente equivalente para acciones globales y por fila.

## 9. Storage Actions

- [ ] 9.1 Implementar guardar individual con `uploadAndStoreOneDocument`.
- [ ] 9.2 Implementar guardar todos con `AppProgressBatch`.
- [ ] 9.3 Procesar archivos secuencialmente.
- [ ] 9.4 Construir request final por archivo con `trd`, `expediente`, `workflow`, `camposIndexacion` y `documento`.
- [ ] 9.5 Mapear progreso storage a estados/fases visuales.
- [ ] 9.6 Emitir `onStored` con metadata y response normalizada.
- [ ] 9.7 Emitir `onInterfaceRegistration` cuando mapper genere eventos.
- [ ] 9.8 Emitir `onBatchComplete` con resumen.
- [ ] 9.9 Emitir `onError` con errores controlados.
- [ ] 9.10 Soportar cancelacion con `AbortController`.
- [ ] 9.11 Soportar retry desde estado error/cancelled.

## 10. Legacy Exclusion and Security

- [ ] 10.1 Verificar que no se usa jQuery.
- [ ] 10.2 Verificar que no se usa Bootstrap manual/WebForms.
- [ ] 10.3 Verificar que no se usa `.ashx`.
- [ ] 10.4 Verificar que no se usa `XMLHttpRequest`.
- [ ] 10.5 Verificar que no se usa `FormData` legacy para upload.
- [ ] 10.6 Verificar que no se usa `fetch` directo ni `clienteApi` en componente/hooks.
- [ ] 10.7 Verificar que no se loguean tokens, bytes ni payload sensible.
- [ ] 10.8 Confirmar que backend no fue modificado y que endpoints de almacenamiento existentes no fueron cambiados.
- [ ] 10.9 Verificar que extensiones, tamano maximo y tipologias no quedan hardcodeados como fuente final.

## 11. Tests

- [ ] 11.1 Tests de carga de config y tipologias.
- [ ] 11.2 Tests de fallo de config y seleccion deshabilitada.
- [ ] 11.3 Tests de `accept`, extension y max size desde config.
- [ ] 11.4 Tests de seleccion multiple.
- [ ] 11.5 Tests de `reject` y `queue-with-error`.
- [ ] 11.6 Tests de metadata independiente por archivo.
- [ ] 11.7 Tests de sugerencia de tipologia y override manual.
- [ ] 11.8 Tests de tipologia obligatoria.
- [ ] 11.9 Tests de fecha requerida/futura/invalida.
- [ ] 11.10 Tests de eliminar archivo, limpiar todos, seleccionar preview.
- [ ] 11.11 Tests de guardar individual procesa un solo archivo.
- [ ] 11.12 Tests de guardar todos procesa secuencialmente.
- [ ] 11.13 Tests de multiples archivos generan multiples POST finales.
- [ ] 11.14 Tests de `onStored`, `onInterfaceRegistration`, `onBatchComplete`, `onError`.
- [ ] 11.15 Tests de cancelacion durante chunks.
- [ ] 11.16 Tests de retry.
- [ ] 11.17 Tests de respuesta stale ignorada.
- [ ] 11.18 Tests de ausencia de legacy/prohibidos en codigo productivo.
- [ ] 11.19 Verificacion navegador/manual o Playwright: seleccionar 5 archivos, contador correcto y preview PDF.
- [ ] 11.20 Verificacion navegador/manual o Playwright: cambiar tipologia/fecha por archivo, eliminar uno y limpiar todos.
- [ ] 11.21 Verificacion navegador/manual o Playwright: guardar individual, guardar todos, invalido por extension/tamano y retry tras error simulado.

## 12. Documentation

- [ ] 12.1 Crear README enterprise del componente.
- [ ] 12.2 Documentar props y contratos.
- [ ] 12.3 Documentar ejemplo embebido.
- [ ] 12.4 Documentar ejemplo modal/controlado.
- [ ] 12.5 Documentar loaders requeridos.
- [ ] 12.6 Documentar flujo `init -> chunks -> complete -> almacenar`.
- [ ] 12.7 Documentar matriz FE/BE campo a campo.
- [ ] 12.8 Documentar politica de tipologia por archivo y fecha.
- [ ] 12.9 Documentar contrato de retorno para registro de interfaz.
- [ ] 12.10 Documentar errores, cancelacion, retry y limites conocidos.

## 13. Verification and Publish Readiness

- [ ] 13.1 Ejecutar `npx.cmd openspec validate scrumcore-271-crea-componente-appuploaddocumental --strict`.
- [ ] 13.2 Ejecutar suite focal SCRUMCORE-271.
- [ ] 13.3 Ejecutar lint/TypeScript focal o documentar deuda no relacionada.
- [ ] 13.4 Ejecutar busqueda de prohibidos (`any`, `.ashx`, `XMLHttpRequest`, `FormData`, jQuery, direct `clienteApi` en UI).
- [ ] 13.5 Revisar `git diff --stat`.
- [ ] 13.6 Commit de refinamiento OpenSpec antes de implementacion/publish.
- [ ] 13.7 Registrar evidencia de pruebas navegador/manuales o documentar deuda explicita si el entorno no permite ejecutarlas.
