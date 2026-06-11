## Context

Antes de `SCRUMCORE-245`, el unico componente React exportado para digitalizacion visual era `DigitalizacionDocumentalModal`. Ese componente contenia simultaneamente:

- estado documental;
- inicializacion de scanner;
- toolbar;
- miniaturas;
- preview;
- metadata;
- generacion PDF;
- submit crear/adjuntar;
- wrapper `AppModal`.

Eso hacia imposible usarlo como digitalizador corporativo inline en un layout como:

```txt
CapDocument
+- centerPanel
   +- DigitalizacionDocumentalWorkspace
```

El arbol real anterior siempre era:

```txt
CapDocument
+- centerPanel
   +- DigitalizacionDocumentalModal
      +- AppModal
         +- contenido digitalizacion
```

## Decisions

1. `DigitalizacionDocumentalWorkspace` es el componente final reutilizable para embebido inline.
2. `DigitalizacionDocumentalModal` queda como wrapper de compatibilidad para consumidores que aun requieren overlay.
3. La UI React sigue sin acceder a `DWObject`; todo pasa por `DigitalizacionScannerClient`.
4. La salida final se mantiene PDF-only.
5. Las reglas de metadata, crear documento, adjuntar y validacion final siguen delegadas a APIs modernas.
6. La trazabilidad legacy queda documentada en `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-239-legacy-traceability.md`.

## Component Architecture

```txt
src/modules/digitalizacion/
+- components/
   +- DigitalizacionDocumentalWorkspace/
      +- DigitalizacionDocumentalWorkspace.tsx
      +- DigitalizacionDocumentalWorkspace.module.css
      +- digitalizacionWorkspace.helpers.ts
      +- index.ts
   +- DigitalizacionDocumentalModal/
      +- DigitalizacionDocumentalModal.tsx
      +- index.ts
```

Runtime inline:

```txt
CapDocument
+- leftPanel 70%
   +- DigitalizacionDocumentalWorkspace
      +- useDigitalizacionDocumentalState
      +- useDigitalizacionScanner
      +- useDigitalizacionOperationOrchestrator
      +- DigitalizacionScannerClient
```

Runtime modal:

```txt
Consumer
+- DigitalizacionDocumentalModal
   +- AppModal
      +- DigitalizacionDocumentalWorkspace
```

## Public API

Inline:

```tsx
import { DigitalizacionDocumentalWorkspace } from "src/modules/digitalizacion";
```

Modal:

```tsx
import { DigitalizacionDocumentalModal } from "src/modules/digitalizacion";
```

## Risks / Trade-offs

- `DigitalizacionDocumentalWorkspace` conserva las capacidades actuales: scanner, miniaturas, preview, metadata, PDF y orquestacion API. No agrega crop, deskew ni descarte automatico de paginas en blanco; esas funciones quedan trazadas como pendientes si negocio las confirma.
- Las APIs backend de validacion final y merge PDF siguen siendo fuente de verdad; el frontend no puede completar reglas de firmado/bloqueado/radicado sin backend.
- Las rutas `D:\imagenesda\...` no estuvieron disponibles. Se usaron copias locales del legacy para la trazabilidad inicial.

## Validation Plan

1. Ejecutar lint focal de `src/modules/digitalizacion`.
2. Ejecutar Vitest focal de `src/modules/digitalizacion`.
3. Ejecutar `npm run build` y documentar si falla por errores ajenos al alcance.
