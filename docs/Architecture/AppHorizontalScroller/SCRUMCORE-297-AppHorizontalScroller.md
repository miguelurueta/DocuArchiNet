# SCRUMCORE-297 AppHorizontalScroller

## Objetivo

Crear `AppHorizontalScroller` como primitive UI reutilizable para renderizar contenido en una fila horizontal responsive con scroll en X. El componente permite construir rails/banners horizontales para accesos rapidos, tarjetas resumidas, colecciones compactas o futuros listados documentales sin acoplarse a reglas de negocio.

## Alcance

- Componente compartido en `src/app/Components/UI/AppHorizontalScroller/`.
- Implementacion con React 19, TypeScript y CSS Modules.
- API tipada para densidad, separacion, ancho minimo/maximo de items, scroll snap y edge fade.
- Region accesible con `role="region"` y `aria-label`.
- Pruebas unitarias con React Testing Library.

## Mapa De Archivos

```txt
src/app/Components/UI/AppHorizontalScroller/
├── AppHorizontalScroller.tsx
├── AppHorizontalScroller.module.css
├── AppHorizontalScroller.test.tsx
└── index.ts

src/app/Components/UI/index.ts
└── export * from "./AppHorizontalScroller";

docs/Architecture/AppHorizontalScroller/
└── SCRUMCORE-297-AppHorizontalScroller.md
```

Responsabilidades:

| Archivo | Responsabilidad |
|---|---|
| `AppHorizontalScroller.tsx` | API publica, normalizacion de dimensiones, composicion DOM y clases. |
| `AppHorizontalScroller.module.css` | Layout horizontal, overflow, densidad, gap, snap y edge fade. |
| `AppHorizontalScroller.test.tsx` | Contrato de render, accesibilidad, variantes, dimensiones, snap y defensas. |
| `index.ts` | Export del componente y tipos publicos. |
| `src/app/Components/UI/index.ts` | Barrel compartido para consumidores que importan desde UI. |

## No Objetivos

- No consumir APIs internas o externas.
- No usar `axios`, `fetch`, servicios HTTP ni hooks de dominio.
- No integrar `GestionCorrespondencia`.
- No implementar SCRUM-162.
- No crear cards documentales, visor, descarga, busqueda, filtros, paginacion, virtualizacion ni botones prev/next.
- No modificar `AppTable` ni `AppTreeTable`.
- No agregar dependencias nuevas.

## Arquitectura

### Composicion General

```txt
Consumidor de dominio
  - obtiene datos
  - maneja loading/error/empty
  - renderiza items
  - define acciones
        |
        | children
        v
AppHorizontalScroller
  - role region + aria-label
  - layout horizontal
  - overflow-x
  - gap/density
  - item min/max width
  - scroll snap opcional
  - edge fade no bloqueante
```

`AppHorizontalScroller` no conoce endpoints, estados remotos, DTOs ni reglas funcionales. El consumidor es responsable de obtener datos y componer los hijos.

### Estructura DOM

```txt
AppHorizontalScroller
└── div.root
    └── div.viewport
        ├── role="region"
        ├── aria-label={ariaLabel}
        ├── data-testid={testId}
        └── div.content
            ├── style custom properties
            └── children
```

### Flujo De Layout

```txt
Props
  ├── density ───────────────> class densityCompact|densityComfortable
  ├── gap ───────────────────> class gapXS|gapSM|gapMD|gapLG
  ├── scrollSnap ────────────> snap + snapStart|snapCenter
  ├── edgeFade ──────────────> root edgeFade pseudo-elements
  ├── itemMinWidth ──────────> --app-horizontal-scroller-item-min-width
  └── itemMaxWidth ──────────> --app-horizontal-scroller-item-max-width
                                      |
                                      v
                              .content > * stable width
```

## API De Props

```ts
export type AppHorizontalScrollerDensity = "compact" | "comfortable";
export type AppHorizontalScrollerGap = "xs" | "sm" | "md" | "lg";
export type AppHorizontalScrollerSnap = "none" | "start" | "center";

export interface AppHorizontalScrollerProps {
  children: React.ReactNode;
  ariaLabel: string;
  className?: string;
  viewportClassName?: string;
  contentClassName?: string;
  density?: AppHorizontalScrollerDensity;
  gap?: AppHorizontalScrollerGap;
  itemMinWidth?: number | string;
  itemMaxWidth?: number | string;
  scrollSnap?: AppHorizontalScrollerSnap;
  edgeFade?: boolean;
  testId?: string;
}
```

Defaults:

```ts
density = "comfortable";
gap = "md";
scrollSnap = "none";
edgeFade = false;
```

### Tabla De Props

| Prop | Tipo | Default | Obligatoria | Uso |
|---|---|---:|---|---|
| `children` | `ReactNode` | N/A | Si | Contenido que el consumidor quiere renderizar horizontalmente. |
| `ariaLabel` | `string` | N/A | Si | Nombre accesible de la region scrolleable. |
| `className` | `string` | `undefined` | No | Clase adicional para el root. |
| `viewportClassName` | `string` | `undefined` | No | Clase adicional para el viewport scrolleable. |
| `contentClassName` | `string` | `undefined` | No | Clase adicional para el contenedor flex de items. |
| `density` | `"compact" \| "comfortable"` | `comfortable` | No | Controla padding vertical. |
| `gap` | `"xs" \| "sm" \| "md" \| "lg"` | `md` | No | Controla separacion entre hijos directos. |
| `itemMinWidth` | `number \| string` | `undefined` | No | Define ancho minimo de hijos directos. Number se convierte a px. |
| `itemMaxWidth` | `number \| string` | `undefined` | No | Define ancho maximo de hijos directos. Number se convierte a px. |
| `scrollSnap` | `"none" \| "start" \| "center"` | `none` | No | Activa snap horizontal de proximidad. |
| `edgeFade` | `boolean` | `false` | No | Activa fade visual en bordes sin bloquear interaccion. |
| `testId` | `string` | `undefined` | No | Se aplica como `data-testid` en el viewport. |

### Normalizacion De Dimensiones

| Entrada | Resultado |
|---|---|
| `220` | `"220px"` |
| `"14rem"` | `"14rem"` |
| `"min(18rem, 80vw)"` | `"min(18rem, 80vw)"` |
| `""` | Ignorado |
| `" "` | Ignorado |
| `0` | Ignorado |
| `-1` | Ignorado |
| `Number.NaN` | Ignorado |
| `Number.POSITIVE_INFINITY` | Ignorado |
| `"-1px"` | Ignorado |
| `"-10rem"` | Ignorado |

## Contrato Visual

- El root ocupa `width: 100%` y no genera overflow horizontal de pagina.
- El viewport usa scroll horizontal nativo.
- El content usa una fila flex horizontal sin wrap.
- Los hijos directos usan `flex: 0 0 auto`.
- `itemMinWidth` y `itemMaxWidth` se aplican por custom properties.
- `edgeFade` es solo una ayuda visual y no comunica informacion critica.

## Reglas Responsive

- Mobile, tablet y desktop usan el mismo scroll horizontal nativo.
- El componente usa `max-width: 100%` y `min-width: 0` para convivir dentro de layouts flex.
- `-webkit-overflow-scrolling: touch` mejora la interaccion en mobile.
- No se fuerza altura fija global; la altura depende del contenido.
- Validacion visual minima: renderizar una coleccion de items con `itemMinWidth` en anchos mobile, tablet y desktop y confirmar que el scroll queda contenido dentro del componente.

### Matriz Responsive

| Viewport | Regla esperada |
|---|---|
| Mobile | El root ocupa `100%`; el overflow queda dentro del viewport del componente; touch scroll habilitado. |
| Tablet | Los items mantienen ancho estable y el scroll horizontal no afecta el layout padre. |
| Desktop | El rail usa scroll nativo cuando el contenido supera el ancho disponible. |

### Reglas CSS Responsive Clave

```css
.root {
  max-width: 100%;
  min-width: 0;
  overflow: hidden;
  width: 100%;
}

.viewport {
  max-width: 100%;
  min-width: 0;
  overflow-x: auto;
  overflow-y: hidden;
  -webkit-overflow-scrolling: touch;
}

.content {
  display: flex;
  flex-flow: row nowrap;
  width: max-content;
}
```

## Accesibilidad

- El viewport principal usa `role="region"`.
- `ariaLabel` es obligatorio por TypeScript y se asigna como `aria-label`.
- El componente no agrega `tabIndex` al viewport.
- El componente no intercepta teclado.
- El foco visible queda delegado a los hijos interactivos.
- El componente funciona con botones, links, cards, elementos estaticos y componentes de dominio.
- `children={null}` no rompe render.

## Reglas CSS

- CSS Modules exclusivamente.
- Sin estilos globales.
- Sin paleta de negocio hardcodeada.
- Sin cards internas ni cards anidadas.
- Sin `position: fixed`.
- Uso de `box-sizing: border-box`, `max-width: 100%` y `min-width: 0`.
- Scroll horizontal con `overflow-x: auto`.
- Scroll vertical controlado con `overflow-y: hidden`.
- `scroll-snap-type: x proximity` solo cuando se pide snap.
- No se usa `mandatory`.
- Edge fade implementado con pseudo-elementos y `pointer-events: none`.

## Decisiones Tecnicas

1. Componente puro sin estado ni efectos.
   - Mantiene el primitive enfocado en layout.

2. Custom properties para dimensiones.
   - Evita mutar o clonar `children`.

3. Region accesible nombrada.
   - Un area scrolleable debe tener nombre navegable por tecnologias asistivas.

4. Sin dependencia de Ant Design.
   - El componente es un primitive bajo; los consumidores pueden renderizar componentes Ant Design dentro.

5. Snap con `proximity`.
   - Evita experiencia rigida en listas largas.

## Restricciones

- No importar `axios`.
- No llamar `fetch`.
- No importar servicios HTTP.
- No importar hooks o modulos de negocio.
- No depender de `AppTable` ni `AppTreeTable`.
- No agregar dependencias nuevas.
- No usar `dangerouslySetInnerHTML`.

## Ejemplo De Uso

### Import Directo Del Componente

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";
```

### Import Desde Barrel UI

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI";
```

### Rail Basico

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";

export function ExampleRail() {
  return (
    <AppHorizontalScroller ariaLabel="Listado horizontal de acciones">
      <button type="button">Crear</button>
      <button type="button">Editar</button>
      <button type="button">Exportar</button>
    </AppHorizontalScroller>
  );
}
```

### Rail Compacto Con Snap

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";

export function ExampleRail() {
  return (
    <AppHorizontalScroller
      ariaLabel="Listado horizontal de elementos"
      density="compact"
      gap="sm"
      itemMinWidth={220}
      itemMaxWidth={280}
      scrollSnap="start"
      edgeFade
    >
      <button type="button">Elemento 1</button>
      <button type="button">Elemento 2</button>
      <button type="button">Elemento 3</button>
    </AppHorizontalScroller>
  );
}
```

### Rail Con Items De Dominio

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";

type SummaryItem = {
  id: string;
  title: string;
  description: string;
};

export function SummaryRail({ items }: { items: SummaryItem[] }) {
  return (
    <AppHorizontalScroller
      ariaLabel="Resumen horizontal"
      density="compact"
      gap="sm"
      itemMinWidth={220}
      itemMaxWidth={320}
      edgeFade
    >
      {items.map((item) => (
        <article key={item.id}>
          <strong>{item.title}</strong>
          <p>{item.description}</p>
        </article>
      ))}
    </AppHorizontalScroller>
  );
}
```

Este ejemplo ilustra composicion; el componente de dominio sigue siendo responsable de obtener datos, manejar estados y definir acciones.

## Integracion Futura Con SCRUM-162

```txt
GestionRespuestaAdjuntosRespuestaRail
  - obtiene idRespuestaRadicado desde contexto
  - llama service/hook SCRUM-162
  - maneja loading/empty/error/success
  - renderiza tarjetas de adjuntos
        |
        | children
        v
AppHorizontalScroller
  - layout horizontal
  - accesibilidad base
  - scroll en X
```

Regla: SCRUM-162 no debe modificar `AppHorizontalScroller` para agregar conocimiento de documentos. Debe componerlo desde un componente de dominio.

## Estrategia De Pruebas

- Render y accesibilidad: `children`, `role`, `aria-label`, `testId`.
- Variantes: densidades y gaps.
- Dimensiones: numeros a px, strings no vacios y valores invalidos ignorados.
- Dimensiones invalidas: strings vacios, strings negativos, cero, negativos numericos, `NaN` e infinitos ignorados.
- Composicion: no mutar ni clonar hijos.
- Scroll snap: `none`, `start`, `center`.
- Edge fade: clase visual y regla no bloqueante en CSS.
- Render defensivo: `children={null}`.
- Auditoria de acoplamiento: sin `axios`, `fetch`, servicios, negocio, `AppTable` ni `AppTreeTable`.

## Evidencia De Validacion

- Tests focalizados: `npm.cmd run test -- src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx` (`14 passed`).
- Lint focalizado: `node_modules\.bin\eslint.cmd src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx`.
- Auditoria de acoplamiento: busqueda sin resultados para `axios`, `fetch(`, servicios, hooks de dominio, `AppTable`, `AppTreeTable` y `GestionCorrespondencia` dentro de `src/app/Components/UI/AppHorizontalScroller`.
- CSS auditado: `scroll-snap-type: x proximity` y `pointer-events: none` para edge fade.
- Validacion responsive minima: el contrato visual se verifico contra CSS del componente (`max-width: 100%`, `min-width: 0`, `overflow-x: auto`, `overflow-y: hidden`, `-webkit-overflow-scrolling: touch`, fila flex sin wrap, `width: max-content` y anchos por custom properties). No se genero screenshot porque este ticket no integra un consumidor de pantalla.

Comandos usados:

```powershell
npm.cmd run test -- src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx
node_modules\.bin\eslint.cmd src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx
npm.cmd run spec:validate
git diff --check
rg "axios|fetch\(|services|hooks|AppTable|AppTreeTable|GestionCorrespondencia" src\app\Components\UI\AppHorizontalScroller
```

## Build

`npm.cmd run build` falla por un error TypeScript preexistente fuera del alcance:

```txt
src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx(8,3): error TS2724: "../../../almacenamientoDocumental/components/AppUploadDocumental" has no exported member named 'UploadDocumentalStoredContext'. Did you mean 'UploadDocumentalContext'?
```

El error pertenece a `gestionCorrespondencia`/`AppUploadDocumental` y no a `AppHorizontalScroller`.

## Riesgos Y Mitigaciones

- Scrollbars nativos varian por navegador.
  - Mitigacion: evitar custom scrollbars y apoyarse en comportamiento nativo.

- Edge fade podria bloquear interaccion si se implementa como overlay interactivo.
  - Mitigacion: `pointer-events: none`.

- Consumidores pueden esperar botones de navegacion.
  - Mitigacion: documentar prev/next como fuera de alcance para esta version.

- El ancho aplica a hijos directos.
  - Mitigacion: documentar el contrato visual y probar custom properties.

- Un consumidor puede pasar CSS string arbitrario para dimensiones.
  - Mitigacion: se ignoran strings vacios y strings que empiezan por `-`; valores CSS avanzados no negativos se permiten para mantener flexibilidad del primitive.

- No hay screenshot responsive automatizado.
  - Mitigacion: la validacion responsive queda documentada por contrato CSS y tests de clases base; la validacion visual real corresponde al primer consumidor de pantalla.

## Relacion Con Futuros Consumidores

Los consumidores deben encargarse de datos, estados y acciones. El primitive solo recibe `children` y configura el layout.

## Relacion Futura Con SCRUM-162

SCRUM-162 podra crear un componente de dominio, por ejemplo `GestionRespuestaAdjuntosRespuestaRail`, que consuma su endpoint y renderice tarjetas dentro de `AppHorizontalScroller`.

`AppHorizontalScroller` no debe conocer:

- `idRespuestaRadicado`
- endpoint SCRUM-162
- DTOs documentales
- estados de carga de adjuntos
- reglas de seguridad documental

## Checklist De Aceptacion

- [x] Componente UI compartido creado.
- [x] API tipada y defaults definidos.
- [x] Scroll horizontal nativo.
- [x] Region accesible con `aria-label`.
- [x] Densidades y gaps.
- [x] Dimensiones por custom properties.
- [x] Scroll snap opcional con proximity.
- [x] Edge fade no bloqueante.
- [x] Tests unitarios.
- [x] Sin consumo HTTP.
- [x] Sin acoplamiento a dominio.
- [x] Sin cambios en `AppTable` ni `AppTreeTable`.
- [x] Documentacion enterprise con SCRUM ID, diagramas, mapa de archivos, API, ejemplos y validaciones.
