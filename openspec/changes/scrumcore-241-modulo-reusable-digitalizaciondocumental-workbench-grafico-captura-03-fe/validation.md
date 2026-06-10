# Validation SCRUMCORE-241

## Scope

Implementacion del workbench grafico de `DigitalizacionDocumentalModal` para captura documental FE-03:

- Header contextual con modo, gabinete, radicado y documento destino.
- Toolbar de scanner con selector, escaneo, retry, limpiar, rotar, eliminar y generar PDF.
- Panel de miniaturas con contador y seleccion.
- Preview estable de pagina/PDF.
- Panel de metadata compatible con estado documental actual.
- Footer operacional con motivo de bloqueo y estado de operacion.

## Evidence

- `npx eslint src/modules/digitalizacion --ext .ts,.tsx`: PASS.
- `npx vitest run src/modules/digitalizacion`: PASS, 6 test files, 32 tests.
- `npm run build`: BLOCKED por errores TypeScript existentes en `src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx` lineas 1887 y 1923 (`dropdownProps` no existe en `AppDropdownProps`).

## Notes

- Vitest requiere ejecucion fuera del sandbox para que Vite lea `vite.config.ts`.
- La validacion `openspec validate` no pudo ejecutarse porque el comando `openspec` no esta instalado o no esta disponible en PATH.
- La UI no crea `object URLs`; cualquier thumbnail recibido se trata como salida del adapter de scanner.
