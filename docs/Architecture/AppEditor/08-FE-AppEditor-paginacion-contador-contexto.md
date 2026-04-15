# Ticket 03 FE

## Titulo

Contador de pagina actual en `AppEditor`

## Objetivo

Mostrar `Pagina X de Y` basado en cursor o scroll.

## Regla arquitectonica

- `application` -> `usePageContext`
- `presentation` -> contador
- `infrastructure` -> `coordsAtPos` + DOM

## Estrategia

Prioridad:

1. cursor (si hay foco)
2. scroll (fallback)

## Calculo

```text
pageIndex = floor(offset / pageContentHeight) + 1
```

## Cursor

```ts
editor.view.coordsAtPos(selection.from)
```

## UI

- esquina inferior derecha
- discreto

## Performance

- debounce scroll
- evitar `setState` innecesario

## Resultado

- contador estable
- no rompe editor
