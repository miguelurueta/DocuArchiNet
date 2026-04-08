# PROMPT ARQUITECTONICO Ticket 11 FE

# Adoptar affordance navegable de AppTable en GestionCorrespondencia

## Rol esperado

Arquitecto de software senior frontend (React, componentes reutilizables, tablas enterprise, adopcion de design system, separacion de capas).

## Objetivo

Adoptar en `GestionCorrespondencia` la nueva capacidad reusable de `AppTable` para expresar affordance visual de navegacion, reemplazando la solucion CSS local actualmente usada en el modulo.

La solucion debe permitir:

- activar la affordance visual desde el contrato shared de `AppTable`
- mantener la navegacion actual de `GestionCorrespondencia`
- eliminar el CSS local duplicado que hoy resuelve el cursor navegable

## Dependencia

Este ticket depende de que exista el contrato reusable definido en:

- `10-FE-Agregar-affordance-navegable-reutilizable-AppTable.md`

## Contexto existente

- Pantalla objetivo:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- Estilos actuales del modulo:
  - `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`
- Componente shared:
  - `src/app/Components/UI/AppTable/AppTable.tsx`
  - `src/app/Components/UI/AppTable/AppTable.types.ts`

## Estado actual

Hoy `GestionCorrespondencia` usa una solucion local:

- `gridClassName={styles.navigableGrid}`
- reglas CSS del modulo para aplicar `cursor: pointer` a celdas navegables

Eso funciona, pero ya no debe mantenerse si `AppTable` expone un contrato reusable para la misma affordance.

## Ubicacion esperada

```txt
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css
src/modules/gestionCorrespondencia/tests/*
```

## Restricciones obligatorias

- no volver a implementar affordance visual manual en CSS del modulo si ya existe el contrato shared
- no mover la logica de navegacion fuera de `GestionCorrespondencia`
- no acoplar `AppTable` al dominio del modulo
- no romper el menu de acciones ni la seleccion de filas
- no usar `any`

## Regla arquitectonica obligatoria

Una vez exista la capacidad reusable en `AppTable`, `GestionCorrespondencia` debe consumirla como modulo cliente y eliminar la solucion CSS local equivalente.

Esto implica:

- `AppTable` resuelve la affordance visual
- `GestionCorrespondencia` solo activa la prop
- `GestionCorrespondencia` conserva callbacks:
  - `onCellClicked`
  - `onActionTriggered`

## Contrato esperado

Ejemplo recomendado:

```tsx
<AppTable
  ...
  rowClickAffordance
  onCellClicked={handleTableCellClick}
  onActionTriggered={handleTableAction}
/>
```

Si el contrato definitivo reusable usa otro nombre, por ejemplo `navigableCells`, la adopcion debe alinearse a ese contrato y no reintroducir CSS local redundante.

## Reglas de implementacion obligatorias

1. Reemplazar en `GestionCorrespondencia.tsx` el uso de `gridClassName={styles.navigableGrid}` por la nueva prop reusable de `AppTable`.
2. Mantener intacta la logica de navegacion actual de `GestionCorrespondencia`.
3. Eliminar del CSS del modulo las reglas dedicadas exclusivamente a la affordance navegable local.
4. Verificar que la columna `acciones` sigue sin comportarse como superficie navegable.
5. Verificar que la columna de seleccion sigue sin comportarse como superficie navegable.
6. No cambiar el contrato de rutas ni el flujo `respuesta/:id`.

## Riesgos a evitar

- dejar activa la prop reusable y tambien el CSS local, generando duplicacion
- reintroducir navegacion al hacer click en la columna `acciones`
- afectar el cursor de controles internos del menu contextual
- perder la affordance visual sobre celdas de datos navegables

## Pruebas unitarias obligatorias

- `GestionCorrespondencia` activa la nueva prop reusable de `AppTable`
- `GestionCorrespondencia` ya no depende de `gridClassName={styles.navigableGrid}` para ese caso
- la navegacion por celda de datos sigue funcionando
- la columna `acciones` no dispara navegacion por click de celda
- el menu contextual sigue funcionando

## Pruebas QT / calidad

- el usuario sigue viendo cursor navegable sobre celdas de datos
- el usuario no ve affordance navegable sobre la columna `acciones`
- el usuario no pierde el menu contextual
- la navegacion a `respuesta/:id` sigue funcionando
- no queda CSS local redundante para este comportamiento

## Criterios de aceptacion

- `GestionCorrespondencia` adopta la affordance reusable de `AppTable`
- se elimina la solucion local equivalente del modulo
- la navegacion actual no se rompe
- la columna `acciones` mantiene su comportamiento contextual sin activar navegacion accidental
- el modulo queda alineado con el contrato shared y sin duplicacion visual
