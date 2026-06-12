# SCRUMCORE-238 - Metadata

- **Ticket**: SCRUMCORE-238
- **Nombre**: Estabilidad enterprise del visor PDF en firma, PDFs grandes y PDFs protegidos
- **Fecha**: 2026-06-12 (America/Bogota)
- **Autor**: Equipo Frontend (cambio realizado con Codex CLI)
- **Rama**: `feature/SCRUMCORE-238`
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
