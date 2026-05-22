# SCRUMCORE-223 - Implementacion Detallada

## Scope tecnico

Implementacion enfocada en `DocumentosWorkbench` para garantizar header persistente durante scroll vertical de lista documental, sin modificar backend ni comportamiento global de `AppTable`.

## Layout strategy

- `DocumentosWorkbench` pasa `tableLayoutMode="fill"` a `AppTreeTable`.
- `AppTreeTable` reexpone `tableLayoutMode` y `tableDomLayout` como props opcionales.
- `AppTreeTable` conserva defaults actuales:
  - `tableLayoutMode = "content"`
  - `tableDomLayout = "autoHeight"`

Resultado:
- Solo el flujo documental usa scroll interno.
- Otros consumidores de `AppTreeTable` siguen igual.

## Scroll strategy

- `listSurface` en `DocumentosWorkbench` deja de tener scroll externo vertical.
- El scroll vertical queda en el viewport interno del grid (`AG Grid`) cuando `layoutMode="fill"`.
- Header de columnas queda siempre visible respecto al viewport del grid.

## Sizing strategy

- `listSurface` usa `display: flex`, `height: 100%`, `min-height: 0`, `overflow: hidden`.
- `AppTreeTable` agrega clase `rootFill` para ocupar alto disponible (`flex:1`, `height:100%`, `min-height:0`) cuando aplica modo `fill`.
- Se preserva el layout de rail + visor ya existente.

## Compatibilidad AppTreeTable / AppTable

- `AppTreeTable` ahora acepta:
  - `tableLayoutMode?: AppTableLayoutMode`
  - `tableDomLayout?: AppTableDomLayout`
- `AppTreeTable` delega estas props a `AppTable`.
- No se altera API existente: cambios additive-only y backward-compatible.

## Comportamiento responsive

- No se alteran reglas de `overlay` mobile/tablet.
- No se modifican reglas de `AppCollapseRail`.
- No se altera `AppVisorEmbedPdf`.

## Separacion de responsabilidades

- `DocumentosWorkbench`: controla layout y estrategia local de scroll.
- `AppTreeTable`: wrapper funcional con pasarela de configuracion.
- `AppTable`: mantiene semantica global.
- CSS local (`DocumentosWorkbench.module.css`): controla overflow/sizing del panel.

## Archivos impactados

- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.module.css`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.tsx`
- `src/app/Components/UI/AppTreeTable/types.ts`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.module.css`
- `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx`
- `src/app/Components/UI/AppTreeTable/AppTreeTable.test.tsx`

## Impacto global

- Sin cambios globales en estrategia de `AppTable`.
- Sin cambios backend, endpoints ni contratos.
- Sin cambios de logica de seleccion, documento activo o acciones.
