## Contexto

`AppTable` mantiene hoy un comportamiento equivalente a `autoHeight`, por lo que crece o se contrae según la cantidad de filas renderizadas. En server pagination eso vuelve inestable la interfaz cuando cambia `pageSize` de `10 -> 20 -> 50`, especialmente en pantallas con toolbar, paginación y otros bloques que ya consumen parte del alto disponible.

## Objetivos

- Estandarizar dos estrategias oficiales de layout para `AppTable`: `content` y `fill`.
- Permitir que `fill` ocupe el alto restante del contenedor y use scroll vertical interno.
- Preservar compatibilidad hacia atrás para implementaciones que hoy dependen de `autoHeight`.

## Decisiones

### 1. `AppTable` expone `layoutMode`

Se introduce `layoutMode?: "content" | "fill"` como contrato reusable del componente.

- `content` conserva el comportamiento actual.
- `fill` fuerza `domLayout="normal"` y activa el layout necesario para ocupar el alto disponible.

### 2. `fill` no mide manualmente toolbar y footer

La solución principal no usa JavaScript para sumar alturas. El contenedor padre debe distribuir el layout vertical con CSS (`flex` o `grid`) y cederle a la tabla el espacio restante.

### 3. El wrapper y la pantalla deben ser compatibles con `fill`

`AppTableQueryWrapper` y la pantalla consumidora deben exponer un contenedor con `min-height: 0` y un tramo flexible para que la tabla pueda heredar el alto restante sin romper el resto del layout.
