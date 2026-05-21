# SCRUMCORE-217 - Pruebas

## Unitarias (obligatorias)

- `[SPEC:APPTREETABLE-217]` `gestionRespuestaDocumentosRequestMapper`
  - Archivo: `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts`
- `[SPEC:APPTREETABLE-217]` `documentosWorkbenchResponseAdapter`
  - Archivo: `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts`
- `[SPEC:APPTREETABLE-217]` `documentosWorkbenchActionMapper`
  - Archivo: `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts`

## Integración UI (obligatorias)

- `[SPEC:APPTREETABLE-217]` `DocumentosWorkbench`
  - Archivo: `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
  - Cubre: render base, rail toggle, variante overlay, wiring selección/acción hacia visor (mock).

## Browser interaction (manual checklist)

- Click principal (fila/label) ejecuta `ver_documento` y actualiza visor.
- Expand/collapse no rompe layout ni selection.
- Menú secundario abre y dispara acción dinámica.
- Error en query muestra estado + retry.
- Responsive intacto: desktop, mobile overlay, tablet/iPad.
- Focus visible y navegación keyboard-friendly.

## E2E (Playwright)

Pendiente: agregar pruebas E2E para flujo real:
- carga real de documentos por tarea
- `ver_documento` actualiza visor
- menú secundario funciona
- responsive intacto

## Regresión

- Verificar que `AppTreeTable` y `AppTable` siguen pasando sus suites existentes (smoke).

## Evidencia de ejecución

Comandos ejecutados (2026-05-21):
- `npm test -- --run src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.test.ts` (PASS)
- `npm test -- --run src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts` (PASS)
- `npm test -- --run src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.test.ts` (PASS)
- `npm test -- --run src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` (PASS)
