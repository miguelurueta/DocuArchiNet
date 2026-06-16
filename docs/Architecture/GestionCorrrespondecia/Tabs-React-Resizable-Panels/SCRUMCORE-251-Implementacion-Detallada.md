# SCRUMCORE-251 - Implementacion Detallada

## Archivos creados

- `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.tsx`
- `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.module.css`
- `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/index.ts`
- `src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.test.tsx`
- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.test.tsx`

## Archivos modificados

- `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- `src/modules/gestionCorrespondencia/style/GestionRespuesta.module.css`
- `package.json`
- `package-lock.json`

## Dependencia

Se agrego `react-resizable-panels`.

La version instalada expone:

- `Group`
- `Panel`
- `Separator`

El componente importa:

```ts
import {
  Group as PanelGroup,
  Panel,
  Separator as PanelResizeHandle,
} from "react-resizable-panels";
```

## Componente nuevo

`GestionWorkbenchParallelTabs` es presentacional:

- Recibe `gestion`, `documentos` y `className`.
- Renderiza un `PanelGroup` horizontal.
- Renderiza dos `Panel` con `defaultSize={50}` y `minSize={35}`.
- Renderiza un `PanelResizeHandle` con affordance visual.
- No importa services.
- No ejecuta queries.
- No conoce reglas de negocio.

## Integracion

La integracion vive en `GestionRespuesta.tsx`, donde ya se construyen los tabs `Gestion` y `Documentos`.

Se agrego:

- `GestionWorkbenchLayoutMode = "tabs" | "parallel"`.
- Estado local `layoutMode`.
- Hook local `useCanUseParallelLayout`.
- Switch opt-in con `aria-pressed`.
- Estado visual explicito con `data-layout-state="inactive"` y `data-layout-state="active"`.
- Fallback a tabs normales cuando el viewport es menor a `901px`.

## Provider compartido

`GestionRespuestaDocumentosProvider` se mantiene como wrapper comun de modo normal y modo paralelo. Esto evita crear providers independientes para `Gestion` y `Documentos`.

## CSS

`GestionRespuesta.module.css` ahora define:

- shell flex vertical;
- header de layout;
- body con `min-height: 0`.

`GestionWorkbenchParallelTabs.module.css` define:

- alto completo;
- paneles con overflow controlado;
- divisor con hover/focus;
- fallback CSS en ancho reducido.

## Accesibilidad

- El switch expone `aria-pressed`.
- El switch tiene estado inactivo gris enterprise y estado activo verde sutil.
- Los paneles exponen `aria-label="Gestion"` y `aria-label="Documentos"`.
- El divisor expone `aria-label="Redimensionar paneles"`.
- El divisor tiene estado visual de foco.

## Compatibilidad

Modo normal conserva `AppTabs` como antes. El cambio no modifica `DocumentosWorkbench`, `GestionRespuestaMainTabContent`, AppEditor, visor PDF, firma ni servicios de reemplazo.
