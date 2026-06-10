# SCRUMCORE-239 - Validation

## Implementacion

- Se creo `src/modules/digitalizacion/` como modulo reusable desacoplado.
- Se definieron tipos estrictos para `DigitalizacionContext`, `DigitalizacionResult`, errores funcionales, scanner, metadata y operacion.
- Se agrego validacion contractual runtime para contexto nulo, modo invalido, `nombreGabinete` vacio y `idDocumentoDestino` requerido en modo `adjuntar`.
- Se implemento `useDigitalizacionDocumentalState` con estado separado por contexto, scanner, metadata y operacion.
- Se agrego proteccion anti-stale por generacion y reset completo ante cambio de contexto.
- Se implemento `DigitalizacionDocumentalModal` como shell operativo sin scanner/backend real.

## Pruebas ejecutadas

```powershell
npx eslint src/modules/digitalizacion --ext .ts,.tsx
npx vitest run src/modules/digitalizacion
```

Resultado focal:

- ESLint focal: OK.
- Vitest focal: 3 archivos, 14 pruebas OK.

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
