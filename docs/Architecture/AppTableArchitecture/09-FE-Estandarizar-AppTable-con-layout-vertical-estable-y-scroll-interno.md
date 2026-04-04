# PROMPT ARQUITECTÓNICO
Estandarizar `AppTable` con layout vertical estable y scroll interno

## Rol esperado

Arquitecto de software senior y desarrollador frontend React  
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Extender `AppTable` para soportar un modo estándar de layout vertical estable, de forma que la tabla pueda ocupar el espacio restante del contenedor y usar scroll interno vertical, evitando crecer o encogerse cuando cambia el número de registros por página.

La solución debe permitir dos estrategias oficiales:

- `content`
- `fill`

## Problema actual

Hoy `AppTable` opera por defecto con comportamiento equivalente a `autoHeight`, lo que hace que:

- la altura de la tabla dependa de la cantidad de filas renderizadas
- al cambiar `pageSize` (`10 -> 20 -> 50`) el componente cambie visualmente de tamaño
- en pantallas con layout compuesto, ese crecimiento o contracción genera inestabilidad

Esto es especialmente problemático en pantallas que ya contienen otros bloques verticales, por ejemplo:

- `AppToolbar`
- paginación
- filtros
- footer
- drawers o paneles secundarios

## Contexto arquitectónico

En módulos como `GestionCorrespondencia`, el contenedor general no está formado solo por la tabla.  
El alto disponible para `AppTable` depende del espacio ya ocupado por:

- toolbar superior
- controles de consulta/paginación
- footer u otros bloques opcionales

Por eso la solución no debe limitarse a “dar una altura fija” manual por pantalla.  
Debe existir un patrón reusable que permita a `AppTable` ocupar el espacio restante del layout.

## Objetivo funcional

`AppTable` debe poder operar oficialmente en dos modos:

### `content`
- la tabla crece según su contenido
- equivalente al comportamiento actual con `autoHeight`

### `fill`
- la tabla ocupa el espacio vertical restante del contenedor
- usa scroll vertical interno
- mantiene altura visual estable aunque cambie `pageSize`

## Restricciones

- no romper usos actuales de `AppTable`
- no cambiar backend
- no acoplar la solución a `GestionCorrespondencia`
- no usar medición manual con JavaScript como primera opción
- priorizar solución basada en layout CSS + configuración AG Grid
- no mezclar esta mejora con el fix de preserve data/refetch

## No alcance

- no rediseñar la UX completa de tablas
- no cambiar `AppTableQueryWrapper` de forma ad hoc para una sola pantalla
- no introducir lógica de negocio
- no resolver aquí temas de búsqueda, filtros o server query

## Contrato esperado

En `AppTable` agregar una prop estándar, por ejemplo:

```ts
layoutMode?: "content" | "fill"
```

o equivalente semántico, siempre que quede claro que existen dos estrategias oficiales de layout.

## Reglas de implementación

### 1. `layoutMode="content"`

- debe mantener el comportamiento actual
- la tabla crece según su contenido
- no debe romper pantallas existentes

### 2. `layoutMode="fill"`

- `AppTable` debe ocupar la altura restante del contenedor
- debe usar `domLayout="normal"`
- debe renderizar scroll vertical interno del grid
- el cambio de `pageSize` no debe alterar la altura visual total de la tabla

### 3. Contenedor padre compatible

Para que `fill` funcione correctamente, el contenedor padre debe usar un patrón de distribución vertical de espacio, por ejemplo:

```txt
auto
auto
minmax(0, 1fr)
auto
```

o equivalente con `flex`.

Esto significa:

- header/toolbar ocupan su alto natural
- paginación/controles ocupan su alto natural
- tabla ocupa el resto
- footer ocupa su alto natural

### 4. Evitar cálculos manuales por defecto

La solución preferida es:

- layout CSS
- altura restante por `grid` o `flex`
- `AppTable` configurado para llenar el espacio

No usar de entrada:

- `ResizeObserver`
- mediciones manuales de header + footer
- cálculos imperativos por pantalla

Solo considerar esas técnicas si la solución basada en layout no cubre un caso real.

## Responsabilidad por capas

### `AppTable`
- soportar el modo reusable `fill`
- configurar AG Grid correctamente para scroll interno

### contenedor de tabla
- distribuir el layout vertical
- permitir que la tabla herede la altura restante

### pantalla consumidora
- elegir si necesita `content` o `fill`
- no implementar hacks manuales de altura

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTable.tsx`
- `src/app/Components/UI/AppTable/AppTable.types.ts`
- `src/app/Components/UI/AppTable/hooks/useAgGridBaseConfig.ts`
- `src/app/Components/UI/AppTable/AppTable.module.css`
- tests asociados
- documentación de arquitectura si aplica

## Riesgos a evitar

- cambiar globalmente el comportamiento actual y romper pantallas existentes
- introducir una altura rígida no reusable
- depender de valores hardcodeados por módulo
- usar JavaScript para medir layout cuando CSS puede resolverlo
- mezclar esta mejora con problemas de refetch o preserve data

## Pruebas obligatorias

Cubrir mínimo:

- `layoutMode="content"` preserva comportamiento actual
- `layoutMode="fill"` usa scroll vertical interno
- en `fill`, al cambiar `pageSize`, la altura visible de la tabla se mantiene estable
- no se rompen overlays
- no se rompen tablas existentes que no usan `fill`

## Criterios de aceptación

- `AppTable` soporta oficialmente un modo de altura estable reusable
- `layoutMode="fill"` mantiene altura visual fija
- el grid usa scroll interno vertical
- `layoutMode="content"` conserva compatibilidad hacia atrás
- el patrón queda reusable para pantallas con toolbar, paginación y footer

## Decisión arquitectónica recomendada

Este trabajo debe ir en un ticket separado del fix de parpadeo.

Separación correcta:

- `SCRUMCORE-44`
  - corrige la transición de datos durante refetch
- nuevo ticket
  - corrige estabilidad vertical y scroll interno reusable

## Instrucción final

Antes de implementar:

- validar cómo `AppTable` usa hoy `domLayout`
- validar `useAgGridBaseConfig`
- validar el layout actual del contenedor donde se quiera aplicar `fill`

Luego:

- implementar con TypeScript estricto
- preservar compatibilidad actual
- dejar el nuevo comportamiento como estándar opt-in

Finalmente reportar:

- contrato final elegido
- estrategia de layout adoptada
- impacto en `domLayout`
- compatibilidad preservada
- pantallas candidatas para adopción gradual
