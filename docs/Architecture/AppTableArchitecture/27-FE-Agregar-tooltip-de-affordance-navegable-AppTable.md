# PROMPT ARQUITECTONICO Ticket 27 FE

# Agregar tooltip de affordance navegable en AppTable

## Rol esperado

Arquitecto de software senior frontend
(React 19, TypeScript estricto, AG Grid, design systems, accesibilidad, componentes reutilizables desacoplados, UX microinteractions, Clean Architecture)

## Objetivo

Incorporar en `AppTable` una pista textual reusable y opt-in para superficies navegables, de forma que cuando `rowClickAffordance` este activo el usuario reciba una senal contextual adicional al pasar el cursor o enfocar una celda o registro navegable.

La solucion debe:

- ser reusable y desacoplada de modulos consumidores
- activarse solo por contrato explicito
- convivir con `rowClickAffordance`
- no interferir con seleccion, acciones ni controles interactivos
- ser compatible con grid y cards

## Contexto existente

- Componente shared:
  - `src/app/Components/UI/AppTable/AppTable.tsx`
  - `src/app/Components/UI/AppTable/AppTable.types.ts`
- Renderers:
  - `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
  - `src/app/Components/UI/AppTable/renderers/AppTableCardRenderer.tsx`
- Estilos:
  - `src/app/Components/UI/AppTable/AppTable.module.css`

`AppTable` ya soporta:

- `rowClickAffordance`
- navegacion por teclado con `Enter`
- affordance visual reusable
- exclusiones de acciones, seleccion y controles internos

## Problema actual

Hoy `AppTable` comunica navegabilidad principalmente por:

- cursor
- hover
- foco funcional

Eso cubre la base, pero no ofrece una pista textual explicita al usuario sobre la accion esperada al interactuar con una celda o registro navegable.

## Ubicacion esperada

```txt
src/app/Components/UI/AppTable/AppTable.tsx
src/app/Components/UI/AppTable/AppTable.types.ts
src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx
src/app/Components/UI/AppTable/renderers/AppTableCardRenderer.tsx
src/app/Components/UI/AppTable/AppTable.module.css
src/app/Components/UI/AppTable/tests/*
```

## Restricciones obligatorias

- NO usar `any`
- NO acoplar la solucion a `GestionCorrespondencia`
- NO hardcodear textos de dominio en `AppTable`
- NO forzar tooltip por defecto en todas las tablas
- NO romper interaccion de columnas `acciones`
- NO romper columna de seleccion
- NO interferir con botones, links, inputs o menus internos
- NO introducir una experiencia ruidosa o redundante con tooltips permanentes

## Regla arquitectonica obligatoria

El tooltip debe ser:

- opt-in
- reusable
- presentacional
- desacoplado de la accion real

Esto implica:

- `AppTable` expone un contrato declarativo
- el consumidor decide activarlo y define el mensaje
- `AppTable` solo renderiza la pista textual
- la navegacion o accion primaria sigue viviendo en callbacks del consumidor

## Contrato esperado

La recomendacion base es:

```ts
rowClickTooltip?: string;
```

Uso esperado:

```tsx
<AppTable
  ...
  rowClickAffordance
  rowClickTooltip="Abrir detalle"
/>
```

Semantica:

- solo tiene efecto cuando `rowClickAffordance` esta activo
- expresa la accion primaria esperada
- no ejecuta navegacion automaticamente

## Reglas de implementacion obligatorias

### 1. Activacion opt-in

- `rowClickTooltip` debe ser opcional
- si no se informa, no debe renderizarse tooltip alguno

### 2. Dependencia de affordance

```txt
El tooltip solo debe activarse cuando rowClickAffordance este activo.
```

Si existe `rowClickTooltip` sin `rowClickAffordance`, el shared component no debe asumir navegabilidad.

### 3. Superficies cubiertas

La solucion debe contemplar:

- celdas navegables del grid
- cards navegables en `presentationMode="cards"`

### 4. Exclusiones obligatorias

No debe mostrarse tooltip sobre:

- columna `acciones`
- columna de seleccion
- superficies no navegables
- controles interactivos internos

### 5. Implementacion desacoplada

El tooltip debe implementarse dentro del shared component usando las primitivas UI ya presentes en el proyecto o una capa minima consistente.

No debe requerir CSS o wrappers desde el modulo consumidor.

En `presentationMode="cards"` puede usarse un wrapper directo sobre la superficie navegable si el costo es acotado.

En modo `table`, la implementacion debe evitar un wrapper React pesado por cada celda del grid si eso incrementa costo de render o churn innecesario.

### 6. Eventos

El tooltip:

- NO debe interceptar click
- NO debe alterar bubbling
- NO debe romper `onCellClicked`, `onRowClicked` ni `onActionTriggered`

### 7. Accesibilidad

La pista textual debe ser compatible con:

- hover
- foco si aplica
- lectores de pantalla cuando corresponda

Sin generar ruido innecesario ni duplicar labels existentes.

## Reglas de consistencia visual

La experiencia debe:

- ser sobria
- no competir con el contenido principal
- alinearse con el design system existente
- evitar tooltips masivos simultaneos o parpadeantes

## Riesgos a evitar

- tooltip en superficies no navegables
- tooltip sobre acciones o seleccion
- acoplamiento a un texto de dominio especifico
- exceso de ruido visual
- conflicto con accesibilidad de controles internos
- comportamiento inconsistente entre grid y cards
- degradacion de performance por instanciar tooltips pesados en demasiadas celdas

## Pruebas unitarias obligatorias

- no se renderiza tooltip por defecto
- tooltip solo se activa con `rowClickAffordance`
- columnas de acciones y seleccion no muestran tooltip navegable
- cards navegables pueden mostrar tooltip cuando aplica
- no se altera el contrato de eventos existente
- controles interactivos internos no quedan envueltos por el tooltip de navegacion

## Pruebas QT / calidad

- hover sobre celda navegable muestra pista textual
- hover sobre card navegable muestra pista textual
- columna `acciones` no muestra tooltip de navegacion
- seleccion no muestra tooltip de navegacion
- no hay regresiones visuales ni funcionales

## Criterios de aceptacion

- `AppTable` expone un contrato reusable para tooltip de affordance
- el comportamiento es opt-in
- no hay acoplamiento a modulos consumidores
- grid y cards quedan cubiertos
- acciones, seleccion y controles internos quedan excluidos
- no se rompen eventos ni accesibilidad

## Instruccion final

Antes de implementar:

- revisar como hoy se materializa `rowClickAffordance` en grid y cards
- validar si el proyecto ya tiene una primitiva de tooltip reusable adecuada
- confirmar el mejor punto de integracion para no envolver superficies incorrectas
- definir una estrategia liviana para grid que no requiera una instancia costosa por cada celda navegable

Luego:

- agregar el contrato reusable
- integrar el tooltip en superficies navegables validas
- excluir acciones, seleccion y controles internos
- validar la estrategia de performance en `table` y `cards`
- validar comportamiento y accesibilidad

Finalmente reportar:

- contrato elegido
- estrategia de render del tooltip
- exclusiones aplicadas
- pruebas ejecutadas
- impacto en consumidores
