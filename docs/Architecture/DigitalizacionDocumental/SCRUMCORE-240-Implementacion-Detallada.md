# SCRUMCORE-240 - Implementacion detallada

## Archivos principales

- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/loadDynamsoftScripts.ts`
- `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts`

## Adapter

`DynamsoftTwainClient` implementa `DigitalizacionScannerClient` y concentra la interaccion con Dynamsoft:

- inicializa runtime y licencia;
- lista scanners;
- selecciona scanner por `deviceId`;
- ejecuta scan;
- rota/remueve paginas por `pageId`;
- limpia buffer;
- genera PDF con MIME `application/pdf`;
- libera runtime con `dispose`.

## Validacion runtime

El adapter valida:

- `deviceId` no vacio;
- `resolutionDpi` entre 75 y 600;
- `colorMode` dentro de `color | grayscale | blackWhite`;
- scanner seleccionado antes de scan;
- paginas existentes antes de PDF;
- PDF no vacio, extension `.pdf` y MIME `application/pdf`.

## Anti-stale

Cada operacion captura una generacion interna. `dispose` incrementa la generacion, limpia estado y evita que respuestas tardias muten el adapter o el hook.
