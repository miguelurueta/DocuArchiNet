# SCRUMCORE-246 AppDigitalizador

## Resultado

Se crea el componente corporativo reutilizable:

```txt
src/app/Components/UI/AppDigitalizador/AppDigitalizador.tsx
```

Import publico:

```tsx
import { AppDigitalizador } from "@/app/Components/UI/AppDigitalizador";
```

Uso minimo:

```tsx
<AppDigitalizador
  context={context}
  onCompleted={handleCompleted}
/>
```

## API publica

Props requeridas:

- `context: DigitalizacionContext | null`
- `onCompleted: (result: DigitalizacionResult) => void`

Props opcionales:

- `active`
- `modulo`
- `apiClient`
- `scannerClient`
- `dynamsoft`
- `licenciaDynamsoft`
- `className`
- `onCancel`
- `onError`

## Arquitectura final

```txt
Modulo consumidor
  -> AppDigitalizador
     -> AppDigitalizadorProvider/defaults
     -> DigitalizacionDocumentalWorkspace
        -> useDigitalizacionScanner
        -> useDigitalizacionDocumentalState
        -> useDigitalizacionOperationOrchestrator
        -> DynamsoftTwainClient
```

`AppDigitalizador` no usa `AppModal`. Se renderiza inline y puede ser montado en paneles como `CapDocument.centerPanel`.

## Sandbox

Ruta:

```txt
/__sandbox/app-digitalizador
```

Archivo:

```txt
src/app/pages/AppDigitalizadorSandboxPage.tsx
```

La sandbox usa `VITE_DYNAMSOFT_LICENSE_KEY` para probar licencia real si esta configurada.

## Compatibilidad

- `DigitalizacionDocumentalModal` sigue funcionando para overlays.
- `DigitalizacionDocumentalWorkspace` sigue siendo la base funcional inline.
- `DynamsoftTwainClient`, hooks, servicios y contratos existentes no fueron duplicados.

## Validacion

- `npx eslint src/app/Components/UI/AppDigitalizador src/app/pages/AppDigitalizadorSandboxPage.tsx src/modules/digitalizacion --ext .ts,.tsx`: PASS.
- `npx tsc --noEmit`: PASS.
- `npx vitest run src/modules/digitalizacion`: PASS, 50 tests.
- `npx vitest run src/app/Components/UI/AppDigitalizador`: PASS, 3 tests.

## Pendientes

- Validacion manual con scanner fisico y licencia real en la sandbox.
- Confirmar disponibilidad backend para metadata, upload temporal, crear documento y adjuntar digitalizacion segun la matriz SCRUMCORE-239.
