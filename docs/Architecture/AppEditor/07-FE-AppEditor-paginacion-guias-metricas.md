# Ticket 02 FE

## Titulo

Medir contenido y dibujar guias visuales de pagina

## Objetivo

Calcular altura del contenido y dibujar guias sin fragmentar documento.

## Regla arquitectonica

- `application` -> calculo metricas (`usePaginationMetrics`)
- `presentation` -> overlay guias
- `infrastructure` -> acceso DOM

## Estrategia de medicion

- usar `scrollHeight` de `.ProseMirror`
- ejecutar en `rAF` o `useLayoutEffect`

## Calculo

```text
pageContentHeight = pageHeight - (top + bottom)
totalPages = ceil(contentHeight / pageContentHeight)
```

## Render guias

- overlay absoluto
- fuera de `ProseMirror`
- `pointer-events: none`

## Performance

- debounce `16–50ms`
- no medir cada `keypress`

## Resultado

- guias visibles por pagina
- sin romper editor
