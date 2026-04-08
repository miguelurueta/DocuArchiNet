# PROMPT ARQUITECTONICO Ticket 10 FE

# Agregar affordance navegable reutilizable en AppTable

## Rol esperado

Arquitecto de software senior frontend (React, design systems, componentes reutilizables, tablas enterprise, UX affordance, separacion de capas).

## Objetivo

Incorporar en `AppTable` un mecanismo reusable y opt-in para expresar visualmente que una fila o sus celdas son navegables, sin acoplar el componente shared a un modulo especifico ni asumir que todas las tablas navegan al hacer click.

La solucion debe permitir una API explicita como:

- `navigableCells`
- o `rowClickAffordance`

para que los modulos consumidores activen la affordance visual solo cuando realmente exista un flujo de navegacion asociado.

## Contexto existente

- Componente reusable:
  - `src/app/Components/UI/AppTable/AppTable.tsx`
  - `src/app/Components/UI/AppTable/AppTable.types.ts`
- Renderer grid:
  - `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
- Estilos base:
  - `src/app/Components/UI/AppTable/AppTable.module.css`
- Implementacion puntual actual:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
  - `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`

## Problema actual

Hoy la affordance de cursor navegable se resolvio localmente en `GestionCorrespondencia` mediante estilos del modulo.

Eso cumple el caso puntual, pero deja abierta una necesidad reusable:

- otros modulos pueden querer el mismo patron
- `AppTable` no ofrece una forma declarativa de expresar affordance de navegacion
- repetir CSS modulo por modulo introduce duplicacion y riesgo de inconsistencias

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

- no asumir que todas las tablas son navegables
- no forzar `cursor: pointer` por defecto en todas las celdas
- no acoplar `AppTable` a `GestionCorrespondencia`
- no mezclar affordance visual con logica de `navigate`
- no romper columnas especiales:
  - `acciones`
  - seleccion
  - celdas no navegables
- no usar `any`

## Regla arquitectonica obligatoria

La affordance visual de navegacion debe ser un contrato explicito y opt-in del componente reusable.

Esto implica:

- `AppTable` expone una prop declarativa
- el modulo consumidor decide activarla
- `AppTable` solo resuelve estilo/comportamiento visual
- la navegacion real sigue viviendo en callbacks del consumidor

## Opciones de diseno validas

### Opcion A

```ts
navigableCells?: boolean
```

Semantica:

- si es `true`, `AppTable` aplica affordance visual a celdas de datos navegables
- excluye internamente columnas de accion y seleccion

### Opcion B

```ts
rowClickAffordance?: boolean
```

Semantica:

- expresa que existe una accion de click sobre fila/celda y que la UI debe comunicarlo

## Recomendacion arquitectonica

Preferir `rowClickAffordance?: boolean`.

Razon:

- comunica mejor la intencion UX
- no promete que todas las celdas son navegables como entidad separada
- queda alineado con `onRowClicked` y `onCellClicked` como contratos ya existentes

## Reglas de implementacion obligatorias

1. Agregar una prop explicita reusable en `AppTable`.
2. Mantener esa prop como opt-in y con default `false`.
3. Aplicar affordance visual solo cuando la prop este activa.
4. Excluir de la affordance:
   - `app-table-action-cell`
   - `ag-Grid-SelectionColumn`
5. Mantener el cambio en la capa shared, no en un modulo especifico.
6. No inferir affordance solo porque exista `onRowClicked`.
7. No disparar navegacion automaticamente desde `AppTable`.
8. Permitir que modulos como `GestionCorrespondencia` dejen de usar CSS local una vez adopten la prop reusable.

## Riesgos a evitar

- aplicar `pointer` a controles interactivos dentro de celdas no navegables
- dar una falsa senal visual de navegacion en tablas que no navegan
- duplicar la logica entre CSS local del modulo y CSS base de `AppTable`
- mezclar affordance visual con la semantica de accion contextual
- afectar negativamente el modo cards si no corresponde

## Pruebas unitarias obligatorias

- `AppTable` no aplica affordance navegable por defecto
- `AppTable` aplica affordance cuando la nueva prop esta activa
- la columna `acciones` no recibe affordance navegable
- la columna de seleccion no recibe affordance navegable
- el cambio no rompe `onRowClicked`, `onCellClicked` ni `onActionTriggered`
- `GestionCorrespondencia` puede usar la prop reusable y dejar de depender de CSS local si se migra en una fase posterior

## Pruebas QT / calidad

- tabla sin affordance activa mantiene cursor normal
- tabla con affordance activa muestra cursor esperado en celdas navegables
- menu de acciones sigue funcionando sin senal visual equivocada
- seleccion de filas sigue funcionando sin aparentar navegacion
- el patron es reutilizable por otros modulos sin estilos duplicados

## Criterios de aceptacion

- `AppTable` expone una prop reusable para affordance de navegacion
- el comportamiento es opt-in
- no se acopla la tabla a ningun modulo especifico
- no se rompe el menu de acciones ni la seleccion
- la navegacion real sigue resuelta por el consumidor
- el cambio queda listo para reemplazar estilos locales duplicados en modulos futuros
