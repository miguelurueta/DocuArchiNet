# SCRUMCORE-253 Procesamiento automatico de imagenes

## Auditoria tecnica

Fecha: 2026-06-17

SDK configurado:

- `DYNAMSOFT_SDK_VERSION = "19.3.2"`
- `DYNAMSOFT_EXPECTED_SERVICE_VERSION = "1.9.3.1028"`
- `DYNAMSOFT_EXPECTED_TWAIN_MODULE_VERSION = "19.3.2"`
- Carga por CDN: `https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist`

Hallazgo importante: el paquete `node_modules/dwt` no existe en el repositorio. La integracion actual usa el runtime CDN de Dynamsoft Web TWAIN y los contratos locales definidos en:

- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.constants.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`

La busqueda local no encontro un contrato tipado confirmado para `Deskew`, `AutoCrop` o `AutoRotate`. Por eso no se activa procesamiento destructivo ciego desde React.

## Implementacion

Se agrego la seccion lateral `Procesamiento automatico` en `DigitalizacionDocumentalWorkspace` con:

- `Deskew`
- `Auto Crop`
- `Auto Rotate`

Todas las opciones quedan desactivadas por defecto y persisten solo durante la vida de la sesion React del workspace.

El contrato `ScanOptions` ahora incluye:

```ts
automaticProcessing?: {
  deskew?: boolean;
  autoCrop?: boolean;
  autoRotate?: boolean;
};
```

El flujo es:

```txt
DigitalizacionDocumentalWorkspace
  -> useDigitalizacionScanner.scan(options)
  -> DynamsoftTwainClient.scan(options)
  -> applyAutomaticProcessing()
```

## Estrategia de ejecucion

`DynamsoftTwainClient` conserva la responsabilidad tecnica. Si el objeto WebTwain expone una API nativa candidata, se ejecuta por pagina y se reconstruyen las paginas afectadas para refrescar miniaturas, preview y PDF.

Metodos candidatos:

| Opcion | Metodos candidatos |
| --- | --- |
| Deskew | `Deskew`, `deskew`, `DeskewImage`, `AutoDeskew` |
| Auto Crop | `AutoCrop`, `autoCrop`, `AutoCropImage` |
| Auto Rotate | `AutoRotate`, `autoRotate`, `AutoRotateImage` |

Si el metodo no existe en el runtime, el procesamiento queda controlado como `unsupported` y la captura no falla.

## Metricas

Se registran tiempos por capacidad:

- `DESKEW_TIME`
- `AUTOCROP_TIME`
- `AUTOROTATE_TIME`

Cada log incluye:

- `durationMs`
- `pageCount`
- `status`: `applied`, `unsupported` o `failed`
- `message` cuando aplica

## Estado funcional

| Funcionalidad | Estado |
| --- | --- |
| UI de Deskew | Implementado |
| UI de Auto Crop | Implementado |
| UI de Auto Rotate | Implementado |
| Flags en `ScanOptions` | Implementado |
| Delegacion al adapter | Implementado |
| Ejecucion nativa si existe API runtime | Implementado |
| Manejo de API no soportada | Implementado |
| Miniaturas/preview tras procesamiento nativo | Implementado |
| PDF invalidado tras rotacion/manual scan | Implementado |
| Procesamiento propio por canvas | No implementado por riesgo documental |

## Limitaciones

- Sin un contrato oficial local del SDK no se puede confirmar que la licencia habilite Deskew, Auto Crop o Auto Rotate.
- Auto Crop puede recortar contenido documental si se implementa con heuristicas propias sin validacion fisica.
- Auto Rotate puede fallar en documentos con poco texto, cedulas o paginas mixtas.
- La implementacion no agrega librerias de procesamiento de imagen externas.

## Validacion manual recomendada

1. Abrir `/__sandbox/app-digitalizador`.
2. Seleccionar scanner fisico.
3. Confirmar que `Deskew`, `Auto Crop` y `Auto Rotate` aparecen apagados por defecto.
4. Activar una opcion a la vez.
5. Escanear 1 pagina.
6. Revisar consola:
   - Si el runtime soporta la API: `status: "applied"`.
   - Si no la soporta: `status: "unsupported"`.
7. Validar miniatura y preview.
8. Generar PDF y confirmar que respeta el estado de las paginas.

## Archivos modificados

- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx`
- `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/index.ts`
- `src/modules/digitalizacion/index.ts`
- `src/modules/digitalizacion/tests/DynamsoftTwainClient.test.ts`
- `src/modules/digitalizacion/tests/useDigitalizacionScanner.test.tsx`
- `src/modules/digitalizacion/components/DigitalizacionDocumentalModal/DigitalizacionDocumentalModal.test.tsx`
- `src/app/Components/UI/AppDigitalizador/tests/AppDigitalizador.test.tsx`

## Validaciones ejecutadas

```txt
npx tsc --noEmit
Resultado: OK

npx eslint src/modules/digitalizacion src/app/Components/UI/AppDigitalizador
Resultado: OK

npx vitest run src/modules/digitalizacion src/app/Components/UI/AppDigitalizador
Resultado: OK
Nota: el primer intento en sandbox fallo por acceso denegado al cargar vite.config.ts; se reejecuto con permisos elevados y paso.

npm run spec:validate
Resultado: OK
```
