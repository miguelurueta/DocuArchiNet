# Ticket 04 FE

## Titulo

Agregar salto de pagina manual en `AppEditor`

## Objetivo

Insertar `PageBreak` persistido usando extension Tiptap.

## Nodo obligatorio

- `type: block`
- `atom: true`
- `selectable: true`
- `isolating: true`

## HTML

```html
<div data-page-break="true"></div>
```

## Render

- linea horizontal
- separacion visual
- no editable

## Comando

```ts
editor.commands.insertPageBreak()
```

## Reglas

- no multiples consecutivos
- insertar en posicion valida

## Cursor

- antes y despues funcional
- no bloquear escritura

## Integracion paginacion

- `PageBreak = limite duro`
- reiniciar calculo despues del salto

## Parsing

- detectar `data-page-break`
- mapear a nodo

## Resultado

- salto persistido
- compatible con visual paginado
