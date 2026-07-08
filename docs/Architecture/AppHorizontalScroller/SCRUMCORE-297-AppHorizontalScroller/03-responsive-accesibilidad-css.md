# 03 Responsive, Accesibilidad Y CSS

## Reglas Responsive

- Mobile, tablet y desktop usan el mismo scroll horizontal nativo.
- El componente usa `max-width: 100%` y `min-width: 0` para convivir dentro de layouts flex.
- `-webkit-overflow-scrolling: touch` mejora la interacción en mobile.
- No se fuerza altura fija global; la altura depende del contenido.
- Validación visual mínima: renderizar una colección de items con `itemMinWidth` en anchos mobile, tablet y desktop y confirmar que el scroll queda contenido dentro del componente.

## Matriz Responsive

| Viewport | Regla esperada |
|---|---|
| Mobile | El root ocupa `100%`; el overflow queda dentro del viewport del componente; touch scroll habilitado. |
| Tablet | Los items mantienen ancho estable y el scroll horizontal no afecta el layout padre. |
| Desktop | El rail usa scroll nativo cuando el contenido supera el ancho disponible. |

## Reglas CSS Responsive Clave

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
- El componente funciona con botones, links, cards, elementos estáticos y componentes de dominio.
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

## Scroll Snap

| Valor | Resultado |
|---|---|
| `none` | No aplica snap. |
| `start` | Aplica `scroll-snap-type: x proximity` y `scroll-snap-align: start` a hijos directos. |
| `center` | Aplica `scroll-snap-type: x proximity` y `scroll-snap-align: center` a hijos directos. |

## Edge Fade

El edge fade se implementa con pseudo-elementos del root:

```txt
.edgeFade::before
.edgeFade::after
```

Ambos usan `pointer-events: none`, por lo que no bloquean botones, links, inputs, selección de texto ni interacción de hijos.
