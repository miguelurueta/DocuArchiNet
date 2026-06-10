# SCRUMCORE-240 - Validation

## Implementacion

- Se agrego la infraestructura `src/modules/digitalizacion/infrastructure/dynamsoft`.
- Se definio `DigitalizacionScannerClient` como contrato estable para aislar la UI del SDK.
- Se implemento `DynamsoftTwainClient` como adapter PDF-only.
- Se agrego loader idempotente `loadDynamsoftScripts`.
- Se agregaron errores funcionales tipados para runtime, licencia, scanner, concurrencia, PDF y stale operations.
- Se agrego `useDigitalizacionScanner` para orquestar estado `idle | initializing | ready | scanning | generatingPdf | error`.
- Se exporto la API publica desde `src/modules/digitalizacion/index.ts`.

## Pruebas ejecutadas

```powershell
npx eslint src/modules/digitalizacion --ext .ts,.tsx
npx vitest run src/modules/digitalizacion
```

Resultado focal:

- ESLint focal: OK.
- Vitest focal: 6 archivos, 30 pruebas OK.

## Build global

```powershell
npm run build
```

Resultado: falla por errores preexistentes fuera del alcance en:

- `src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx`

Detalle:

- `Property 'dropdownProps' does not exist on type 'IntrinsicAttributes & AppDropdownProps'.`

## OpenSpec

`openspec status` y `openspec instructions apply` no pudieron ejecutarse porque `openspec` no esta instalado en PATH dentro de esta sesion.
