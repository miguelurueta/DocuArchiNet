# PROMPT ARQUITECTÓNICO
Configurar click de fila, selección única/múltiple y foco de celda en `AppTable`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Extender `AppTable` para soportar de forma clara y reusable:

- evento configurable de click sobre registro
- selección de fila única o múltiple
- control explícito del foco o selección visual de celda

La solución debe permitir que al hacer click en una fila:

- la fila pueda seguir seleccionándose
- se pueda disparar un evento de fila
- la celda no quede visualmente marcada por defecto

## Problema actual

Hoy el comportamiento de AG Grid mezcla varias cosas en la misma interacción:

- click en fila o celda
- selección de fila
- foco visual de celda
- posible evento de click del registro

Eso genera una experiencia ambigua:

- la fila se selecciona
- la celda también parece quedar activa o marcada
- no está claro qué parte del comportamiento es realmente configurable

## Objetivo funcional

`AppTable` debe separar explícitamente tres dimensiones distintas:

### 1. Evento de click sobre fila

Permitir un callback reusable cuando el usuario hace click en un registro.

Ejemplo conceptual:

```ts
onRowClicked?: (row: T) => void
```

### 2. Selección de fila

Permitir configuración de selección:

- única
- múltiple

Ejemplo conceptual:

```ts
rowSelection?: "single" | "multiple"
```

### 3. Foco visual de celda

Permitir controlar si una celda queda visualmente activa al hacer click.

Ejemplo conceptual:

```ts
suppressCellFocus?: boolean
```

Default recomendado:

```ts
suppressCellFocus = true
```

## Resultado esperado

Al hacer click sobre una fila:

- la fila puede seguir seleccionándose
- el evento de fila puede ejecutarse
- la celda no debe quedar visualmente marcada por defecto

## Alcance

- mantener soporte de selección única y múltiple
- mantener evento reusable de click de fila
- agregar control explícito del foco o selección visual de celda
- dejar comportamiento default más limpio para pantallas tipo listado

## No alcance

- no rediseñar `AppTable`
- no cambiar backend
- no alterar paginación
- no mezclar con renderer cards
- no eliminar selección de fila por click si la pantalla la necesita

## Reglas de implementación

### 1. Evento de fila

- `AppTable` debe seguir permitiendo un callback de click sobre registro
- este evento no debe depender del foco visual de celda

### 2. Selección de fila

- `rowSelection="single"` debe seguir funcionando
- `rowSelection="multiple"` debe seguir funcionando
- la selección de fila no debe perderse por introducir el cambio de foco de celda

### 3. Foco de celda

- debe poder desactivarse explícitamente
- por default debe quedar desactivado para la mayoría de pantallas tipo listado
- si una pantalla necesita comportamiento completo de AG Grid a nivel celda, debe poder habilitarlo

## Contrato esperado

El contrato final debe distinguir, al menos, estas piezas:

```ts
rowSelection?: "single" | "multiple"
onRowClicked?: (row: T) => void
suppressCellFocus?: boolean
```

## Comportamientos esperados

### Caso A. Selección múltiple

- click selecciona la fila
- puede disparar `onRowClicked`
- no deja celda marcada visualmente por defecto

### Caso B. Selección única

- click selecciona solo una fila
- puede disparar `onRowClicked`
- no deja celda marcada visualmente por defecto

### Caso C. Pantalla especializada

- puede habilitar foco de celda si realmente lo necesita

## Archivos frontend esperados

- `src/app/Components/UI/AppTable/AppTable.types.ts`
- `src/app/Components/UI/AppTable/hooks/useAgGridBaseConfig.ts`
- `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
- tests de `AppTable`

## Riesgos a evitar

- romper selección de fila actual
- romper callbacks de click
- mezclar foco de celda con selección de fila
- introducir comportamiento distinto por pantalla sin contrato shared
- dejar AG Grid con defaults poco claros

## Pruebas obligatorias

- click dispara `onRowClicked`
- selección única sigue funcionando
- selección múltiple sigue funcionando
- `suppressCellFocus=true` evita foco visual de celda por default
- `suppressCellFocus=false` permite comportamiento estándar si se requiere

## Criterios de aceptación

- `AppTable` soporta click de fila reusable
- `AppTable` mantiene selección única y múltiple
- la celda no queda marcada visualmente por defecto
- el foco de celda queda configurable
- el contrato queda claro y reusable para otras pantallas

## Conclusión arquitectónica

No se debe quitar la selección de fila por click.
Lo que debe quedar configurable es el foco o selección visual de celda.

Separación correcta:

- click de fila
- selección de fila
- foco visual de celda
