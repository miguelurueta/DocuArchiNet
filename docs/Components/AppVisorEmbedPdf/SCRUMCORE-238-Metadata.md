# SCRUMCORE-238 - Metadata

- **Ticket**: SCRUMCORE-238
- **Nombre**: Estabilidad enterprise del visor PDF en firma, PDFs grandes y PDFs protegidos
- **Fecha**: 2026-06-12 (America/Bogota)
- **Autor**: Equipo Frontend (cambio realizado con Codex CLI)
- **Rama**: `feature/SCRUMCORE-238`
- **Commit actual validado**: `c340d6d` (`c340d6df6dd47fdbf2ef4b6789ba21984c70c4f9`)
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
