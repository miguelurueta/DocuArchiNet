# SCRUMCORE-297 AppHorizontalScroller

## Objetivo

Crear `AppHorizontalScroller` como primitive UI reutilizable para renderizar contenido en una fila horizontal responsive con scroll en X. El componente permite construir rails/banners horizontales para accesos rapidos, tarjetas resumidas, colecciones compactas o futuros listados documentales sin acoplarse a reglas de negocio.

## Alcance

- Componente compartido en `src/app/Components/UI/AppHorizontalScroller/`.
- Implementacion con React 19, TypeScript y CSS Modules.
- API tipada para densidad, separacion, ancho minimo/maximo de items, scroll snap y edge fade.
- Region accesible con `role="region"` y `aria-label`.
- Pruebas unitarias con React Testing Library.

## No Objetivos

- No consumir APIs internas o externas.
- No usar `axios`, `fetch`, servicios HTTP ni hooks de dominio.
- No integrar `GestionCorrespondencia`.
- No implementar SCRUM-162.
- No crear cards documentales, visor, descarga, busqueda, filtros, paginacion, virtualizacion ni botones prev/next.
- No modificar `AppTable` ni `AppTreeTable`.
- No agregar dependencias nuevas.

## Arquitectura

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

## Estrategia De Pruebas

- Render y accesibilidad: `children`, `role`, `aria-label`, `testId`.
- Variantes: densidades y gaps.
- Dimensiones: numeros a px, strings no vacios y valores invalidos ignorados.
- Composicion: no mutar ni clonar hijos.
- Scroll snap: `none`, `start`, `center`.
- Edge fade: clase visual y regla no bloqueante en CSS.
- Render defensivo: `children={null}`.
- Auditoria de acoplamiento: sin `axios`, `fetch`, servicios, negocio, `AppTable` ni `AppTreeTable`.

## Evidencia De Validacion

- Tests focalizados: `npm.cmd run test -- src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx`.
- Lint focalizado: `node_modules\.bin\eslint.cmd src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.tsx src/app/Components/UI/AppHorizontalScroller/AppHorizontalScroller.test.tsx`.
- Auditoria de acoplamiento: busqueda sin resultados para `axios`, `fetch(`, servicios, hooks de dominio, `AppTable`, `AppTreeTable` y `GestionCorrespondencia` dentro de `src/app/Components/UI/AppHorizontalScroller`.
- CSS auditado: `scroll-snap-type: x proximity` y `pointer-events: none` para edge fade.
- Validacion responsive minima: el contrato visual se verifico contra CSS del componente (`max-width: 100%`, `min-width: 0`, `overflow-x: auto`, `-webkit-overflow-scrolling: touch`, fila flex sin wrap y anchos por custom properties). No se genero screenshot porque este ticket no integra un consumidor de pantalla.

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
