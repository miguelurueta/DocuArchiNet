# Ticket 01 FE

## Titulo

Introducir modo de paginacion visual open source en `AppEditor`

## Rol

Desarrollador Frontend Senior especializado en:

- React 19 + TypeScript estricto
- Tiptap / ProseMirror
- Clean Architecture
- CSS avanzado tipo documento
- Testing con Vitest

## Objetivo

Extender `AppEditor` para soportar un modo de visualizacion paginada
(`visual`), sin fragmentar el documento internamente.

## Regla arquitectonica

- `presentation` -> layout tipo hoja
- `application` -> estado editor
- `infrastructure` -> Tiptap

Prohibido:

- logica de paginacion en Tiptap
- modificar estructura del documento

## API

```tsx
<AppEditor
  paginationMode="visual"
  pageFormat="A4"
  pageOrientation="portrait"
  pageMargins={{ top: 96, right: 72, bottom: 96, left: 72 }}
/>
```

## Estrategia visual

Estructura:

- `editorWrapper`
- `canvas`
- `sheet`
- `content`

## Dimensiones base

A4:

- `width: 794px`
- `height: 1123px`

## Scroll

- scroll en `canvas`
- hoja sin scroll interno

## Performance

- sin calculos en `keypress`
- layout solo con CSS

## Resultado

- hoja centrada
- documento continuo
- sin romper editor
