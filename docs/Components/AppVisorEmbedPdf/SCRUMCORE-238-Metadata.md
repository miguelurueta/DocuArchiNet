# SCRUMCORE-238 - Metadata

- **Ticket**: SCRUMCORE-238
- **Nombre**: Estabilidad enterprise del visor PDF en firma, PDFs grandes y PDFs protegidos
- **Fecha**: 2026-06-12 (America/Bogota)
- **Autor**: Equipo Frontend (cambio realizado con Codex CLI)
- **Rama**: `feature/SCRUMCORE-238`
- **Commit funcional validado**: `c340d6d` (`c340d6df6dd47fdbf2ef4b6789ba21984c70c4f9`)
- **Ultimo commit documental previo**: `212eeb9` (`212eeb9f7855c336174b8dfddbbd252e9fb18949`)
- **Ultimo commit publicado**: `11088d4` (`11088d4ad302d0f33c033ba629da1ba3e6f084eb`)
- **PR funcional base**: `#288` - fusionado en `main`
- **PR de sincronizacion AppEditor**: `#289` - `https://github.com/miguelurueta/DocuArchiCore.react/pull/289`
- **Tipo**: Bugfix / hardening de concurrencia, lifecycle y seguridad de password en memoria
- **Backend**: NO modificado
- **Endpoints**: NO modificados
- **Persistencia de password**: NO (`localStorage`, `sessionStorage`, IndexedDB, cache persistente, logs o telemetria)

## Archivos modificados

- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.tsx`
- `docs/Components/AppVisorEmbedPdf/SCRUMCORE-238-Implementacion-Detallada.md`
- `docs/Components/AppVisorEmbedPdf/SCRUMCORE-238-Metadata.md`

## Problemas corregidos

### 1. Prompt falso en PDF grande no protegido

Caso:

- PDF grande no protegido, aproximadamente 869 MB.
- Abre correctamente.
- Se firma correctamente.
- Al navegar a otro documento y volver al PDF grande aparecia prompt de contrasena.

Diagnostico:

- El PDF llegaba como `application/pdf`.
- `hasPassword: false`.
- No habia password real.
- Habia doble `load()` para el mismo documento/fuente.
- La segunda apertura podia cerrar el documento que la primera apertura aun estaba estabilizando en PDFium/EmbedPDF.

Solucion:

- Deduplicacion de cargas identicas en curso dentro de `AppVisorEmbedPdf`.
- Reutilizacion de la misma promesa de carga cuando `attemptId + documentKey + url + isElectronicallySigned` coinciden.
- Limpieza de carga en curso al cancelar.

Log esperado:

```txt
[DV][visor] load() duplicate in-flight reused
```

### 2. Password validada se limpiaba durante firma en PDF protegido

Caso:

- PDF protegido con contrasena.
- Usuario ingresa password.
- `retry:ok` confirma password valida.
- Al realizar firma/exportacion, reaparece prompt.
- El request final salia con `OriginalPdfPassword: false`.
- Backend respondia `400 Validation` porque el PDF original protegido no recibia password.

Diagnostico:

- La password validada se perdia en memoria por re-render.
- Los callbacks enviados a `EmbedPdfDocumentHost` estaban inline.
- El effect del host dependia de esos callbacks.
- Al cambiar la identidad de callbacks, React ejecutaba cleanup.
- El cleanup limpiaba `validatedPdfPasswordRef` y `originalPdfPasswordRef`.

Solucion:

- Estabilizacion de callbacks con `useCallback`:
  - `handleExportAnnotatedPdfPagesReady`
  - `handleMarkAnnotatedPagesPersistedReady`
  - `handleOriginalPdfPasswordChange`
- El host ya no limpia password por re-render de firma/export/upload.
- La password sigue limpiandose al cambiar de documento/fuente o desmontar.

Log seguro agregado:

```txt
[DV][password][memory] { hasPassword: true|false }
```

### 3. Upload de paginas anotadas grandes

Caso:

- PDF grande o pagina anotada generada con tamano mayor al tolerado por el canal/proxy/backend.
- El upload podia fallar por `ERR_CONNECTION_RESET` o `Network Error` al enviar un chunk grande.

Diagnostico:

- El PDF anotado de una sola pagina podia pesar mas de 1 MB.
- El backend podia devolver `ChunkSizeBytes` de 1 MB.
- La infraestructura real se comportaba mejor con chunks mas pequenos.

Solucion:

- Se limito el tamano efectivo del chunk frontend a `768 KB`.
- Se mantiene el contrato binario puro.
- Se sigue respetando un chunk menor si backend lo retorna.

Log:

```txt
[DV][reemplazo-paginas][chunk]
```

### 4. Bloqueo de navegacion durante reemplazo

Caso:

- Mientras se guardaba la firma/reemplazo, el usuario podia seleccionar otro documento.
- Esto podia interferir con temporales, cancelaciones, carga de visor y estado visible.

Solucion:

- Se bloquea la navegacion del listado y del rail durante `isReplacingAnnotatedPages`.
- Se registra intento bloqueado con:

```txt
[DV][reemplazo-paginas][navigation-blocked]
```

Componentes cubiertos:

- `AppTreeTable`
- `AppCollapseRail`
- seleccion de documentos del workbench

### 5. Firma persistida sin reload post-exito

Caso:

- Recargar el PDF despues de firmar un documento grande o protegido podia reabrir el engine, consumir mucha memoria o disparar prompt de password.

Solucion:

- Despues del reemplazo exitoso no se fuerza reload inmediato.
- El visor marca la firma como persistida visualmente.
- Las firmas ya guardadas dejan de ser editables/removibles.
- El usuario puede agregar nuevas firmas posteriormente.

Logs:

```txt
[DV][firma][persisted:start]
[DV][firma][persisted:done]
```

### 6. Mensaje de exito simplificado

El mensaje visible al usuario queda como:

```txt
Documento firmado correctamente.
```

No se muestra `RequestId` en el toast principal para evitar ruido visual. La referencia tecnica queda disponible para soporte si se requiere revisar logs/respuestas.

## Seguridad

- `OriginalPdfPassword` sigue viviendo solo en memoria volatil.
- No se persiste en navegador.
- No se registra el valor en consola.
- El log nuevo solo muestra booleano `hasPassword`.
- El request final solo envia `OriginalPdfPassword` cuando existe una password validada.
- PDFs sin contrasena siguen enviando `OriginalPdfPassword: false`.

## Contrato de backend

Sin cambios.

El frontend sigue consumiendo:

- `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/init`
- `PUT /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}`
- `POST /api/gestor-documental/documentos/reemplazopdf/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete`
- `POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas`

## Evidencia tecnica esperada

PDF protegido correcto:

```txt
[DV][password][retry:ok] hasValidatedPassword: true
[DV][password][memory] hasPassword: true
[DV][firma][replace-export:start]
[DV][reemplazo-paginas][final-request] OriginalPdfPassword: true
```

PDF grande no protegido correcto:

```txt
[DV][password][open-attempt] hasPassword: false
[DV][visor] load() duplicate in-flight reused
```

No debe aparecer prompt de contrasena falso para PDF no protegido.

## Pruebas ejecutadas

```powershell
npx.cmd tsc --noEmit --pretty false
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx --reporter verbose
npm.cmd run build
```

Resultado:

- TypeScript: OK.
- Vitest visor: OK, 22 tests.
- Build productivo: OK.

## Riesgos residuales

- Si PDFium reporta `PdfErrorCode.Password` y `encrypted: true`, el documento realmente puede estar protegido o el PDF generado puede estar cifrado.
- Si el backend devuelve un PDF corrupto/incompleto despues del reemplazo, el visor puede fallar por `OPEN_FAILED`; ese caso debe diagnosticarse con `blobSize`, `contentType`, `encrypted` y logs del backend.
- PDFs extremadamente grandes pueden seguir alcanzando limites de memoria del navegador o del engine; este ticket evita duplicar aperturas y reducir presion, pero no elimina limites fisicos del runtime.
- PDFs con metadata/rotacion efectiva siguen presentando un pendiente visual: la pagina puede verse correctamente autoajustada por PDFium/EmbedPDF, pero la firma colocada con el plugin oficial puede aparecer rotada. Este pendiente no tiene parche frontend activo.

## Actualizacion 2026-06-16 - Diagnostico backend PDF con metadata de rotacion

Estado del repositorio al cierre parcial:

- Rama: `feature/SCRUMCORE-238`.
- HEAD: `c340d6d` - `fix: preserve validated PDF password during signing`.
- Working tree previo a esta actualizacion de metadata: limpio.
- No quedan aplicados los intentos frontend de correccion de firma rotada sobre PDFs con metadata de rotacion.
- Se revirtieron los ajustes experimentales que modificaban defaults de herramientas, flags `noRotate`, renderers custom o geometria del visor porque no resolvieron la rotacion visual de la firma.

Diagnostico entregado para backend:

- El frontend genera y sube correctamente el PDF anotado temporal de una sola pagina.
- El flujo de chunks y el `POST /api/gestor-documental/documentos/reemplazopdf/paginas-anotadas` responden exitosamente.
- El problema observado ocurre en PDFs cuyo original llega con geometria interna portrait, pero PDFium/EmbedPDF lo presenta visualmente como landscape por metadata/rotacion efectiva.
- Ejemplo real antes de reemplazar:
  - `width: 612`
  - `height: 792`
  - `rotatedWidth: 792`
  - `rotatedHeight: 612`
  - `slotLooksRotated: true`
  - `rotationRaw: 0`
  - `rotationSteps: 0`
- Despues del reemplazo fisico por backend, el PDF resultante puede volver como:
  - `rotatedWidth: 612`
  - `rotatedHeight: 792`
  - `slotLooksRotated: false`
- Esto indica que el reemplazo backend con iText/iText7 no esta preservando la rotacion/metadata efectiva de la pagina original.

Revision solicitada a backend:

- Leer y preservar la rotacion real de la pagina original (`GetRotation()` o equivalente).
- Preservar `/Rotate`, `MediaBox`, `CropBox` y orientacion efectiva de la pagina reemplazada.
- Insertar/adaptar el contenido del PDF anotado de una pagina dentro de la caja original sin normalizar ni perder la orientacion visual previa.
- Garantizar que el PDF final conserve la misma orientacion con la que PDFium presentaba el documento antes del reemplazo.

Conclusion actual:

- El problema de perdida de metadata/orientacion despues de consumir la API no queda clasificado como error de upload frontend.
- El frontend mantiene pendiente una decision tecnica separada para la colocacion visual de firma en paginas con metadata rotada, pero no se deja ningun parche experimental activo.

## Actualizacion 2026-06-16 - Consolidado de alcance funcional

Estado funcional documentado:

- El visor mantiene carga gestionada desde `DocumentosWorkbench`; no se regreso a una integracion simple basada solo en `fileUrl`.
- La operacion de firma/reemplazo usa `AppVisorEmbedPdf` como frontera con EmbedPDF/PDFium.
- `DocumentosWorkbench` orquesta validaciones, upload temporal, request final, progreso, bloqueo de navegacion y limpieza best-effort.
- El servicio HTTP dedicado usa `clienteApi`, `AbortSignal` y envelope `AppResponses<T>`.
- El upload envia chunks binarios puros, sin Base64 y sin `FormData`.
- El frontend no intenta setear manualmente `Content-Length`.
- Cada PDF anotado temporal corresponde a una sola pagina.
- El request final envia cada pagina con su propio `RutaTemporalId` y `ArchivoTemporalId`.
- `OriginalPdfPassword` vive solo en memoria volatil y solo viaja en el request final cuando existe password validada.
- El guardado bloquea documentos firmados electronicamente.
- El guardado muestra loader/progreso desde el inicio de la operacion.
- El listado y el rail se bloquean durante el reemplazo para evitar navegacion concurrente.
- Despues del exito, el visor marca las firmas como persistidas sin recargar inmediatamente el PDF pesado.
- El mensaje visible de exito es simple: `Documento firmado correctamente.`

Pruebas y validaciones asociadas:

- Tests unitarios del servicio de reemplazo: init, chunk, complete, cancel, final request, envelope y errores.
- Tests de `DocumentosWorkbench`: chunks, password final, documento firmado, reemplazo exitoso y persistencia posterior.
- Tests de `AppVisorEmbedPdf`: password en memoria, prompts falsos y comportamiento de visor.
- Build productivo ejecutado en validaciones previas: `npm.cmd run build`.

Pendiente tecnico explicito:

- La firma rotada en PDFs con metadata de autoajuste sigue abierta.
- El caso tambien se observa con PDFs que EmbedPDF/PDFium ya presenta correctamente, pero cuyo plugin de firma calcula la colocacion en una geometria diferente a la visual.
- No se debe considerar resuelto hasta tener una solucion aislada que no afecte PDFs normales ni el centrado actual del visor.
- Las alternativas experimentales probadas fueron revertidas.

## Actualizacion 2026-06-16 - Sincronizacion AppEditor desde main

Motivo:

- La rama `feature/SCRUMCORE-238` ya tenia fusionado el PR funcional base `#288` en `main`.
- Posteriormente se requirio traer a esta rama la version actual de `AppEditor` disponible en `origin/main`.
- El objetivo fue mantener los cambios de `AppVisorEmbedPdf`, firma, reemplazo de paginas anotadas y documentacion SCRUM, pero incorporar la actualizacion de `AppEditor`.

Operacion realizada:

- Se ejecuto `git fetch` para traer referencias remotas sin modificar archivos locales.
- Se confirmo que `origin/main` contenia cambios de `AppEditor` ausentes en `feature/SCRUMCORE-238`.
- Se aplico de forma selectiva solo la carpeta:

```txt
src/app/Components/UI/AppEditor/
```

- No se hizo `merge origin/main` completo para evitar traer cambios ajenos al alcance.
- Se verifico que los cambios locales resultantes estuvieran restringidos a:

```txt
src/app/Components/UI/AppEditor/
src/app/Components/UI/AppDropdown/AppDropdown.tsx
```

Ajuste adicional requerido:

- El `AppEditorToolbar` actualizado usa la prop `dropdownProps` sobre `AppDropdown`.
- La rama actual no tenia esa prop tipada en el componente comun.
- Se agrego compatibilidad minima en:

```txt
src/app/Components/UI/AppDropdown/AppDropdown.tsx
```

- `AppDropdown` ahora acepta `dropdownProps` y los reenvia al componente `Dropdown` de Ant Design.
- Se preserva que `AppDropdown` controle internamente:
  - `children`
  - `menu`
  - `trigger`
  - `open`
  - `onOpenChange`
  - `placement`
- Esto evita romper consumidores existentes y permite que `AppEditorToolbar` compile con la version actualizada.

Commit publicado:

```txt
11088d4 sync AppEditor updates from main
```

PR abierto:

```txt
#289 Sync AppEditor updates into SCRUMCORE-238
https://github.com/miguelurueta/DocuArchiCore.react/pull/289
```

Validacion ejecutada:

```powershell
npm.cmd run build
```

Resultado:

- Build productivo: OK.
- Advertencia Vite por chunks grandes: presente, no bloqueante.
- No se modificaron archivos de `AppVisorEmbedPdf` en esta sincronizacion.
- No se altero el flujo de firma/reemplazo ya validado.
