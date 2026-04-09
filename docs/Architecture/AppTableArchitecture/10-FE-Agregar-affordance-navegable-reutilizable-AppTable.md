# PROMPT ARQUITECTONICO Ticket 10 FE

# Agregar affordance navegable reutilizable en AppTable (con soporte de teclado)

## Rol esperado

Arquitecto de software senior frontend
(React, design systems, componentes reutilizables, tablas enterprise, UX affordance, accesibilidad, keyboard navigation, separacion de capas)

## Objetivo

Incorporar en `AppTable` un mecanismo reusable, accesible y opt-in para expresar visualmente y funcionalmente que una fila o sus celdas son navegables, sin acoplar el componente shared a un modulo especifico ni asumir que todas las tablas navegan al hacer click.

La solucion debe:

- exponer una API declarativa (`rowClickAffordance`)
- aplicar affordance visual consistente
- mantener la navegacion real en el consumidor
- soportar interaccion por teclado (`Enter`) para accesibilidad
- no interferir con acciones, seleccion ni componentes interactivos internos

## Contexto existente

- Componente reusable:
  - `src/app/Components/UI/AppTable/AppTable.tsx`
  - `src/app/Components/UI/AppTable/AppTable.types.ts`
- Renderer grid:
  - `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
- Estilos base:
  - `src/app/Components/UI/AppTable/AppTable.module.css`
- Implementacion actual local:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
  - `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`

## Problema actual

Actualmente la affordance de navegacion se resuelve con CSS local por modulo.

Esto genera:

- duplicacion de estilos
- inconsistencias UX
- ausencia de contrato reusable en `AppTable`
- dificultad para escalar el patron a otros modulos

## Ubicacion esperada

```txt
src/app/Components/UI/AppTable/AppTable.tsx
src/app/Components/UI/AppTable/AppTable.types.ts
src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx
src/app/Components/UI/AppTable/AppTable.module.css
src/app/Components/UI/AppTable/tests/*
src/modules/*/tests/*
```

## Restricciones obligatorias

- NO asumir que todas las tablas son navegables
- NO forzar `cursor: pointer` por defecto
- NO acoplar `AppTable` a `GestionCorrespondencia`
- NO mezclar affordance visual con logica de `navigate`
- NO romper columnas especiales:
  - `acciones`
  - seleccion
  - celdas no navegables
- NO interferir con elementos interactivos internos (botones, links, inputs)
- NO usar `any`

## Regla arquitectonica obligatoria

La affordance de navegacion debe ser:

- declarativa
- opt-in
- reusable
- desacoplada de navegacion real

Esto implica:

- `AppTable` expone una prop
- el modulo consumidor decide activarla
- `AppTable` solo resuelve UX (visual + interaccion)
- la navegacion real vive en callbacks del consumidor

## API del componente (OBLIGATORIO)

```ts
rowClickAffordance?: boolean; // default false
```

Semantica:

- indica que existe una accion primaria de interaccion sobre la fila/celda
- comunica affordance visual y habilita interaccion por teclado
- NO ejecuta navegacion automaticamente

## Reglas de implementacion obligatorias

### 1. Activacion opt-in

- `rowClickAffordance` debe ser `false` por defecto
- solo aplica cuando el consumidor lo activa explicitamente

### 2. Alcance de la affordance

```txt
La affordance debe aplicarse a celdas de datos (no al contenedor global de fila),
respetando el comportamiento estándar de AG Grid.
```

Excluir:

- columna de acciones (`app-table-action-cell`)
- columna de seleccion
- celdas explicitamente no navegables

### 3. Comportamiento visual (OBLIGATORIO)

La affordance debe incluir como minimo:

- `cursor: pointer`
- estado hover consistente (ej: highlight ligero)
- transicion suave opcional
- no modificar layout ni estructura

Regla:

```txt
El estilo debe ser consistente con el design system y no introducir variaciones arbitrarias por tabla.
```

### 4. Implementacion tecnica

Debe implementarse mediante:

```txt
cellClass / cellClassRules en AppTableGridRenderer
```

- NO manipular DOM directamente
- NO aplicar estilos desde el modulo consumidor

### 5. Elementos interactivos internos

Si una celda contiene:

- botones
- links
- inputs

Entonces:

```txt
- no sobreescribir su cursor
- no interferir con sus eventos
- no aplicar affordance sobre el elemento interno
```

### 6. Eventos (NO INTERFERENCIA)

La affordance:

- NO debe interceptar clicks
- NO debe alterar `onRowClicked`, `onCellClicked`, `onActionTriggered`
- NO debe cambiar bubbling existente

### 7. Compatibilidad con seleccion

```txt
La affordance no debe alterar el comportamiento de selección de filas.
```

## Accesibilidad y teclado (OBLIGATORIO)

### 8. Navegacion por teclado

Cuando `rowClickAffordance = true`:

- la fila o celda debe ser accesible por teclado
- debe responder a:

```txt
Enter  ejecutar acción primaria equivalente a click de fila
```

### Reglas

- no duplicar logica de navegacion
- debe reutilizar el mismo flujo que `onRowClicked`
- no romper navegacion por teclado del grid

### 9. Accesibilidad basica

- elementos deben ser focusables si aplica
- no romper navegacion con Tab
- no interferir con accesibilidad existente de AG Grid

## Riesgos a evitar

- aplicar `pointer` en celdas con controles internos
- dar falsa senal UX en tablas no navegables
- duplicar CSS en modulos
- mezclar affordance con navegacion real
- romper seleccion de filas
- afectar modo cards
- interceptar eventos existentes
- romper navegacion por teclado

## Recomendaciones de compatibilidad

- `Enter` debe disparar la accion primaria solo sobre celdas navegables de datos
- no debe activarse en:
  - `app-table-action-cell`
  - `ag-Grid-SelectionColumn`
  - celdas con `button`, `a`, `input`, `textarea`, `select`, `[role="button"]`
- la navegacion o accion primaria debe seguir saliendo del callback del consumidor, no del componente shared
- `GestionCorrespondencia` podra adoptar este contrato en el ticket dependiente `11-FE`

## Pruebas unitarias obligatorias

- `AppTable` no aplica affordance por defecto
- `AppTable` aplica affordance cuando la prop esta activa
- columnas de acciones no reciben affordance
- columna de seleccion no recibe affordance
- no se afectan eventos existentes (`onRowClicked`, etc.)
- elementos interactivos internos mantienen comportamiento
- `Enter` dispara accion equivalente a click de fila
- no se rompe seleccion de filas

## Pruebas de accesibilidad

- la tabla sigue siendo navegable con teclado
- `Enter` funciona solo cuando corresponde
- no se rompe navegacion por Tab
- no se generan focus traps

## Pruebas QT / calidad

- tabla sin affordance  cursor normal
- tabla con affordance  cursor y hover correctos
- acciones siguen funcionando
- seleccion sigue funcionando
- keyboard navigation funcional
- comportamiento consistente entre modulos

## Criterios de aceptacion

- `AppTable` expone `rowClickAffordance`
- comportamiento es opt-in
- no hay acoplamiento a modulos
- no se rompe seleccion ni acciones
- navegacion real sigue en consumidor
- soporte de teclado implementado correctamente
- estilo consistente reusable
- listo para eliminar CSS duplicado en modulos

## Instruccion final

Antes de implementar:

- revisar `AppTableGridRenderer`
- revisar contrato de eventos
- validar comportamiento actual de keyboard navigation en AG Grid

Luego:

- implementar affordance visual reusable
- implementar soporte de teclado (`Enter`)
- asegurar no interferencia con eventos
- mantener tipado estricto

Finalmente reportar:

- decisiones de diseño
- implementación técnica (`cellClassRules`)
- validación de accesibilidad
- pruebas ejecutadas
- impacto en módulos existentes
