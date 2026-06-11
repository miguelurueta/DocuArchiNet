# SCRUMCORE-247 Dynamsoft 19.3.2

## Resultado

La integracion frontend de Dynamsoft queda alineada a la familia instalada en Windows:

```txt
SDK frontend: dwt@19.3.2
Servicio esperado: 1.9.3.1028
TWAIN Module esperado: 19.3.2
```

## URLs

```txt
https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dynamsoft.webtwain.min.js
https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/src/dynamsoft.webtwain.css
https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/src/dynamsoft.webtwain.viewer.css
https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dist/DynamicWebTWAINServiceSetup.msi
```

## Archivos Modificados

- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.constants.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/loadDynamsoftScripts.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.errors.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/index.ts`
- `src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts`
- `src/modules/digitalizacion/tests/loadDynamsoftScripts.test.ts`

## Compatibilidad

No se modifica la arquitectura ni el contrato publico de:

- `AppDigitalizador`
- `DigitalizacionDocumentalWorkspace`
- `DigitalizacionDocumentalModal`
- `DynamsoftTwainClient`

Las APIs actuales del adapter siguen siendo las mismas:

```txt
runtime.ProductKey
runtime.ResourcesPath
runtime.Load()
runtime.GetWebTwain()
SourceCount
GetSourceNameItems()
SelectSourceByIndex()
OpenSource()
AcquireImage()
CloseSource()
Rotate()
RemoveImage()
RemoveAllImages()
ConvertToBlob("application/pdf")
```

## Validacion

- `npx tsc --noEmit`: PASS.
- `npx vitest run src/modules/digitalizacion`: PASS, 53 tests.
- `npx vitest run src/app/Components/UI/AppDigitalizador`: PASS, 3 tests.

## Validacion Manual Pendiente

Abrir:

```txt
/__sandbox/app-digitalizador
```

Confirmar:

- no aparece `Please update your document scanning service`;
- se listan scanners;
- se puede escanear;
- se muestran miniaturas y preview;
- se genera PDF.
