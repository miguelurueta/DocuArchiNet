# SCRUMCORE-233 — Pruebas

## 0) Objetivo de pruebas

Validar que la solución:
- elimina el “bloqueo” bajo clicks rápidos (no más rechazo por `maxDocuments=10`),
- mantiene comportamiento correcto ante cancelación (sin errores visibles),
- evita prompts falsos de contraseña en fallos genéricos.

## 1) Unit tests (visor)

Archivo:
- `src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`

Casos cubiertos (mínimo):
- `[SPEC:SCRUMCORE-233] no muestra password prompt en OPEN_FAILED`:
  - asegura que el prompt de contraseña no aparece cuando falla `openDocumentUrl` por causas no relacionadas a password.

Ejecución (evidencia):
- `npm test -- --run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx`
  - Resultado: ✅ pasa.
  - Nota: se observaron warnings preexistentes de React Testing Library (`act`) y medidas NaN; no se abordaron para no ampliar el alcance.

## 2) Prueba manual (navegador) — checklist

Precondición:
- Activar logs temporales: `window.__DV_DEBUG__ = true`

Escenario A — clicks rápidos (stress):
- Click repetido alternando documentos A/B/C.
- Esperado:
  - no aparece `Maximum number of documents (10) reached`;
  - el documento visible cambia al último click;
  - no hay prompt de contraseña (salvo que realmente exista password).

Escenario B — cancelación:
- Click en documento A y antes de cargar click en B.
- Esperado:
  - el intento A se cancela silenciosamente;
  - B termina cargando (latest‑wins).

Escenario B2 — overlay/skeleton (documento pesado):
- Click en un documento grande (p. ej. cientos de MB).
- Esperado:
  - aparece overlay/skeleton del visor tras ~100ms (sin flicker en docs pequeños);
  - el overlay se quita cuando el engine confirma “ready” (documento usable);
  - si se cancela y se clickea otro documento, el overlay no queda pegado indefinidamente.

Escenario C — action falla:
- Forzar falla o demora de `ver_documento`.
- Esperado:
  - toast con causa explícita o timeout de 10s (“no respondió”).

## 3) Señales de regresión a vigilar

- En consola: `openDocumentUrl failed` con `reason.message` distinto a `maxDocuments` (requiere análisis específico).
- En consola: `GET blob:... net::ERR_FILE_NOT_FOUND` (síntoma de lifecycle blob/engine timing; mitigado, pero si persiste bajo documentos grandes requiere coordinación handshake→revoke).

## 4) E2E Playwright (harness determinístico del visor)

El harness `/__playwright/embedpdf` se ajustó para cargar un PDF fixture sin login (legacy `fileUrl`), permitiendo correr specs del visor de forma determinística.

Ejecuciones (evidencia):
- `npm run test:e2e -- playwright/appvisorEmbedPdfZoom.spec.ts` → ✅ passed
- `npm run test:e2e -- playwright/appvisorEmbedPdfRotate.spec.ts playwright/appvisorEmbedPdfThumbnails.spec.ts` → ✅ passed
- `npm run test:e2e -- playwright/appvisorEmbedPdfZoom.spec.ts playwright/appvisorEmbedPdfRotate.spec.ts playwright/appvisorEmbedPdfThumbnails.spec.ts` → ✅ 3 passed
