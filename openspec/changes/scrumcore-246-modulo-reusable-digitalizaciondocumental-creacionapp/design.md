## Context

`DigitalizacionDocumentalWorkspace` es el componente inline funcional del modulo `src/modules/digitalizacion`. Usa `useDigitalizacionScanner`, `useDigitalizacionDocumentalState` y `useDigitalizacionOperationOrchestrator`, y conserva scanner, miniaturas, preview, metadata, generacion PDF y operaciones API.

SCRUMCORE-246 agrega una fachada corporativa para que modulos como CapDocument, Correspondencia, Workflow, Ventanilla, Archivo Central, PQRS, Contratos y Produccion Documental no consuman directamente esa infraestructura.

## Current Architecture

```txt
Modulo consumidor
  -> DigitalizacionDocumentalWorkspace
     -> hooks digitalizacion
     -> DigitalizacionScannerClient
     -> DynamsoftTwainClient
     -> Dynamsoft
```

## Final Architecture

```txt
Modulo consumidor
  -> AppDigitalizador
     -> AppDigitalizadorProvider/defaults
     -> DigitalizacionDocumentalWorkspace
        -> hooks digitalizacion
        -> DigitalizacionScannerClient
        -> DynamsoftTwainClient
        -> Dynamsoft
```

## Decisions

1. `AppDigitalizador` es inline y no usa `AppModal`.
2. `DigitalizacionDocumentalModal` sigue existiendo para flujos que requieran overlay.
3. La API publica minima es:

```tsx
<AppDigitalizador
  context={context}
  onCompleted={handleCompleted}
/>
```

4. `scannerClient`, `apiClient`, `dynamsoft` y `licenciaDynamsoft` quedan como overrides avanzados para pruebas, sandbox o configuracion corporativa.
5. El provider permite definir defaults compartidos sin repetir configuracion en cada modulo.
6. La licencia real puede llegar por prop, por provider o por variable `VITE_DYNAMSOFT_LICENSE_KEY` en la sandbox.

## File Tree

```txt
src/app/Components/UI/AppDigitalizador/
├─ AppDigitalizador.context.ts
├─ AppDigitalizador.module.css
├─ AppDigitalizador.tsx
├─ AppDigitalizador.types.ts
├─ AppDigitalizadorProvider.tsx
├─ hooks/
│  └─ useAppDigitalizadorScannerClient.ts
├─ index.ts
└─ tests/
   └─ AppDigitalizador.test.tsx
```

## Sandbox

Ruta:

```txt
/__sandbox/app-digitalizador
```

Archivo:

```txt
src/app/pages/AppDigitalizadorSandboxPage.tsx
```

La sandbox monta exclusivamente `<AppDigitalizador />`.

## Risks / Pendientes

- Pruebas con scanner fisico requieren licencia Dynamsoft real y runtime instalado.
- Las operaciones de metadata, upload, crear documento y adjuntar documento dependen de disponibilidad backend.
- Los bloqueos backend auditados en SCRUMCORE-239 siguen siendo fuente de verdad para pendientes funcionales no frontend.
