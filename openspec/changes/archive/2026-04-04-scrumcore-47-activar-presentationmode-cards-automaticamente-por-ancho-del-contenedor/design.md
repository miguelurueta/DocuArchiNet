## Contexto

`AppTable` ya soporta `presentationMode: "table" | "cards"`, pero hoy el modo `cards` solo se activa si una pantalla lo solicita explícitamente. Eso deja la infraestructura lista, pero no habilita comportamiento responsive reusable por sí solo.

La siguiente evolución correcta es permitir que `AppTable` seleccione automáticamente entre tabla y cards según el ancho disponible del contenedor.

## Objetivos

- Activar `cards` automáticamente cuando el contenedor sea estrecho.
- Mantener la posibilidad de forzar `table` o `cards` manualmente.
- Evitar que la decisión dependa de rutas o nombres de pantalla.

## Decisiones

### 1. La fuente principal de verdad es el ancho del contenedor

La activación responsive debe depender del ancho del contenedor de `AppTable`, no del nombre del módulo.

### 2. El override manual mantiene prioridad

Si `presentationMode` viene explícito:

- ese valor manda

Solo cuando `presentationMode` no venga definido debe entrar la lógica responsive.

### 3. La medición debe ser reusable

La técnica recomendada es usar `ResizeObserver` sobre el contenedor del componente para decidir cuándo cambiar a `cards`.

### 4. El contrato debe ser simple

Se propone una configuración como:

```ts
responsivePresentation?: {
  enabled?: boolean
  cardsBelow?: number
}
```

Con eso el componente puede decidir:

- si el ancho del contenedor `< cardsBelow` -> `cards`
- en otro caso -> `table`
