## Why

SCRUMCORE-247 alinea la integracion frontend de Dynamsoft con el servicio local ya instalado en Windows. La sandbox de `AppDigitalizador` ya carga licencia, JS, CSS y runtime, pero el SDK anterior `dwt@18.5.0` queda desalineado con:

```txt
Servicio instalado: 1.9.3.1028
TWAIN Module: 19.3.2.0306
```

La desalineacion produce el mensaje del runtime:

```txt
Please update your document scanning service
```

## What Changes

- Actualizar la carga CDN de Dynamsoft de `dwt@18.5.0` a `dwt@19.3.2`.
- Mantener `DynamsoftTwainClient` y su contrato actual sin reescribir la arquitectura.
- Mantener carga explicita de CSS requerida por DWT:
  - `src/dynamsoft.webtwain.css`
  - `src/dynamsoft.webtwain.viewer.css`
- Exponer constantes de diagnostico para version SDK, servicio esperado, modulo TWAIN esperado e instalador.
- Mantener compatibilidad con `AppDigitalizador`, `DigitalizacionDocumentalWorkspace` y `DigitalizacionDocumentalModal`.

## Non-Goals

- No agregar funcionalidades nuevas de scanner.
- No migrar a otro modelo de API Dynamsoft.
- No cambiar el contrato reusable de `AppDigitalizador`.
- No modificar backend.

## Impact

- Cambio acotado a `src/modules/digitalizacion/infrastructure/dynamsoft`.
- `package.json` no cambia porque el SDK se carga por CDN, no como dependencia npm instalada.
- Requiere validacion manual con scanner fisico en `/__sandbox/app-digitalizador`.
