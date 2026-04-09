# PROMPT ARQUITECTONICO Ticket 26 FE

# Refinar foco visual de celda navegable en AppTable

## Rol esperado

Arquitecto de software senior frontend
(React 19, TypeScript estricto, AG Grid, design systems, accesibilidad, componentes reutilizables desacoplados, UX states, Clean Architecture)

## Objetivo

Refinar la representacion visual del foco de celda en `AppTable` cuando `rowClickAffordance` esta activo, garantizando que el grid mantenga foco funcional para navegacion por teclado (incluyendo `Enter`), pero eliminando la percepcion visual de seleccion de celda que actualmente introduce AG Grid.

La solucion debe preservar accesibilidad, mantener intacto el contrato reusable del componente y evitar conflictos visuales con seleccion de fila y affordance navegable.

## Dependencia

No depende de otros tickets funcionales, pero requiere que el contrato de `rowClickAffordance` ya este implementado en `AppTable`.

## Contexto existente

- Componente shared:
  - `src/app/Components/UI/AppTable/AppTable.tsx`
  - `src/app/Components/UI/AppTable/AppTable.types.ts`
- Renderer:
  - `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
- Hook base:
  - `src/app/Components/UI/AppTable/hooks/useAgGridBaseConfig.ts`
- Estilos:
  - `src/app/Components/UI/AppTable/AppTable.module.css`

El componente ya soporta navegacion mediante `rowClickAffordance` y mantiene foco funcional para permitir interaccion con teclado.

## Estado actual

Para soportar navegacion con `Enter`, `AppTable` mantiene `suppressCellFocus` en `false`.

Esto permite foco funcional en el grid, pero tambien activa el estilo visual por defecto de AG Grid sobre la celda enfocada.

Ese estilo visual:

- se percibe como una seleccion de celda
- compite con la seleccion de fila
- rompe la claridad del patron de affordance navegable

## Ubicacion esperada

```txt
src/app/Components/UI/AppTable/AppTable.module.css
src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx
src/app/Components/UI/AppTable/hooks/useAgGridBaseConfig.ts
src/app/Components/UI/AppTable/tests/*
```

## Restricciones obligatorias

- NO usar `any`
- NO eliminar foco funcional del grid
- NO usar `suppressCellFocus = true` si rompe navegacion con teclado
- NO usar hacks como blur, timers o manipulacion directa del DOM
- NO acoplar la solucion a modulos consumidores
- NO romper seleccion de filas
- NO romper columna de acciones
- NO romper columna de seleccion
- NO aplicar overrides globales de AG Grid fuera del scope de `AppTable`
- NO eliminar foco visual de elementos interactivos internos (botones, links, inputs)

## Regla arquitectonica obligatoria

El foco del grid debe mantenerse como estado funcional para accesibilidad y teclado, pero su representacion visual debe desacoplarse de la semantica de seleccion.

Esto implica:

- `AppTable` controla la capa visual del foco
- AG Grid mantiene la capacidad tecnica del foco
- el foco no puede interpretarse como una segunda seleccion

## Contrato esperado

El comportamiento se activa unicamente cuando el contrato reusable esta activo:

```tsx
<AppTable
  ...
  rowClickAffordance
/>
```

No se deben introducir nuevas props si no son estrictamente necesarias.

## Reglas de implementacion obligatorias

### 1. Preservar foco funcional

El grid debe mantener foco real para soportar `Enter` y navegacion por teclado.

No se debe eliminar ni simular el foco.

### 2. Introducir scope visual controlado

El renderer debe aplicar o reutilizar una clase raiz condicional cuando `rowClickAffordance` este activo.

Esa clase sera la unica responsable de activar el override visual del foco.

### 3. Refinar unicamente la representacion visual

La solucion debe atacar el estilo visual de AG Grid (`ag-cell-focus` y estados relacionados), no su comportamiento.

El foco debe seguir existiendo, pero dejar de ser visualmente dominante.

### 4. Separacion de estados visuales

Debe mantenerse una jerarquia clara:

- seleccion de fila como estado principal
- hover como affordance
- foco como estado tecnico no dominante

El foco no debe verse como una segunda seleccion.

### 5. Proteccion de elementos interactivos

Si una celda contiene elementos interactivos:

- deben conservar su foco visible
- no deben perder accesibilidad
- no deben ser afectados por el override del foco de celda

### 6. Alcance controlado

El ajuste solo aplica cuando `rowClickAffordance` esta activo.

No debe afectar tablas que no usen este contrato.

## Reglas de migracion segura

La modificacion visual debe implementarse sin alterar el comportamiento actual del grid.

Cualquier override CSS debe validarse primero en conjunto con navegacion por teclado antes de aplicarse de forma definitiva.

## Reglas de consistencia visual

La experiencia final debe:

- eliminar la percepcion de doble seleccion
- mantener claridad visual entre estados
- ser consistente con el design system del proyecto

## Reglas de interaccion

Mantener:

- click de celda navegable
- navegacion actual
- exclusion de columnas de acciones y seleccion
- comportamiento de elementos interactivos dentro de la celda

## Accesibilidad y teclado

Debe garantizarse que:

- `Enter` sigue ejecutando la accion primaria
- `Tab` sigue funcionando correctamente
- no se generan focus traps
- el foco sigue siendo detectable para tecnologias asistivas

## Riesgos a evitar

- ocultar completamente el foco sin alternativa accesible
- romper `Enter`
- confundir foco con seleccion
- afectar estilos globales de AG Grid
- aplicar overrides demasiado amplios
- romper interaccion de botones o inputs dentro de celdas

## Pruebas unitarias obligatorias

- `rowClickAffordance` mantiene navegacion con `Enter`
- `onCellClicked` no cambia su contrato
- columnas de acciones y seleccion siguen excluidas
- el renderer aplica correctamente el scope visual
- no se pierde foco funcional del grid

## Pruebas QT / calidad

- click en celda navegable no muestra borde tipo seleccion
- seleccion de fila sigue siendo el estado visual dominante
- hover navegable sigue funcionando
- `Enter` sigue funcionando
- columnas de acciones funcionan correctamente
- no hay regresiones visuales en otras tablas

## Criterios de aceptacion

- el foco funcional se mantiene
- `Enter` funciona correctamente
- el foco visual de celda deja de percibirse como seleccion
- no existe doble estado visual
- la solucion esta encapsulada en `AppTable`
- no hay acoplamiento a modulos consumidores
- no se rompen acciones, seleccion ni accesibilidad
- la experiencia visual es consistente o mejorada

## Instruccion final

Antes de implementar:

- revisar como AG Grid aplica `ag-cell-focus` y estados relacionados
- revisar implementacion actual de `rowClickAffordance`
- confirmar dependencia real del foco para `Enter`

Luego:

- aplicar override visual controlado y scoped
- validar accesibilidad y comportamiento
- no alterar el contrato del componente

Finalmente reportar:

- decision de diseño tomada
- estrategia visual aplicada
- validacion de accesibilidad
- pruebas ejecutadas
- impacto en componentes consumidores
