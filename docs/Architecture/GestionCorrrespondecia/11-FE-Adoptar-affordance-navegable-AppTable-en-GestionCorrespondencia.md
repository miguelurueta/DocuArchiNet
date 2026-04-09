# PROMPT ARQUITECTONICO Ticket 11 FE

# Adoptar affordance navegable de AppTable en GestionCorrespondencia

## Rol esperado

Arquitecto de software senior frontend
(React, componentes reutilizables, tablas enterprise, adopcion de design system, accesibilidad, separacion de capas)

## Objetivo

Adoptar en `GestionCorrespondencia` la capacidad reusable de `AppTable` para expresar affordance visual de navegacion, reemplazando completamente la solucion CSS local actualmente usada en el modulo.

La solucion debe:

- activar la affordance visual desde el contrato shared de `AppTable`
- mantener intacta la navegacion actual del modulo
- eliminar cualquier implementacion CSS local equivalente
- mantener soporte de interaccion por teclado (`Enter`)
- garantizar consistencia visual y funcional

## Dependencia

Este ticket depende de:

```txt
docs/Architecture/AppTableArchitecture/10-FE-Agregar-affordance-navegable-reutilizable-AppTable.md
```

## Contexto existente

- Pantalla objetivo:
  - `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- Estilos actuales:
  - `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`
- Componente shared:
  - `src/app/Components/UI/AppTable/AppTable.tsx`
  - `src/app/Components/UI/AppTable/AppTable.types.ts`

## Estado actual

Actualmente el modulo implementa affordance mediante:

```tsx
gridClassName={styles.navigableGrid}
```

y reglas CSS locales para aplicar `cursor: pointer`.

Esto debe eliminarse en favor del contrato reusable.

## Ubicacion esperada

```txt
src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx
src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css
src/modules/gestionCorrespondencia/tests/*
```

## Restricciones obligatorias

- NO reimplementar affordance visual en CSS del modulo
- NO mantener simultaneamente CSS local y prop reusable
- NO mover la logica de navegacion fuera de `GestionCorrespondencia`
- NO acoplar `AppTable` al dominio del modulo
- NO romper seleccion de filas ni menu de acciones
- NO usar `any`

## Regla arquitectonica obligatoria

Una vez exista la capacidad reusable en `AppTable`, el modulo debe consumirla como cliente y eliminar cualquier implementacion equivalente local.

Esto implica:

- `AppTable` resuelve affordance visual y soporte de teclado
- `GestionCorrespondencia` conserva la navegacion
- eliminacion total de duplicacion

## Contrato esperado

```tsx
<AppTable
  ...
  rowClickAffordance
  onCellClicked={handleTableCellClick}
  onActionTriggered={handleTableAction}
/>
```

Si el contrato final usa otro nombre (ej: `navigableCells`), debe adoptarse ese contrato.

## Reglas de implementacion obligatorias

1. Reemplazar `gridClassName={styles.navigableGrid}` por la prop reusable de `AppTable`.
2. Mantener intacta la logica de navegacion actual.
3. Eliminar del CSS del modulo:
   - clases usadas para cursor pointer
   - hover de celdas navegables
   - cualquier regla relacionada con affordance
4. Eliminar tambien clases residuales no utilizadas relacionadas con este patron.
5. Validar que:
   - columna `acciones` no adquiere affordance navegable
   - columna de seleccion no adquiere affordance
6. No modificar rutas ni flujo `respuesta/:id`.
7. Validar que la affordance reusable cubre completamente el comportamiento antes de eliminar CSS.
8. No implementar manejo de `Enter` en el modulo; solo adoptar y validar el comportamiento provisto por `AppTable`.

## Reglas de migracion segura

```txt
La eliminacion del CSS local solo debe realizarse despues de validar que la prop reusable reproduce completamente el comportamiento visual esperado.
```

## Reglas de consistencia visual

```txt
La experiencia visual final debe ser equivalente o superior a la implementacion anterior.
```

## Reglas de interaccion

- mantener click de celda funcionando
- mantener navegacion actual
- mantener comportamiento de acciones contextuales
- no interferir con elementos interactivos dentro de la celda

## Accesibilidad y teclado

Debe validarse que:

```txt
Enter sigue ejecutando la accion de fila esperada
```

y que:

- no se rompe navegacion por teclado
- no se rompe accesibilidad existente

## Riesgos a evitar

- coexistencia de CSS local y prop reusable
- perdida de affordance visual
- affordance aplicada incorrectamente en columna `acciones`
- romper interaccion de botones dentro de celdas
- romper navegacion por teclado
- regresiones visuales

## Pruebas unitarias obligatorias

- `GestionCorrespondencia` usa `rowClickAffordance`
- no se usa `gridClassName={styles.navigableGrid}`
- no existe CSS activo relacionado con affordance local
- navegacion sigue funcionando
- columna `acciones` no navega
- seleccion de filas no se altera
- interaccion por teclado (`Enter`) funciona correctamente a traves de `AppTable`
- no existe doble aplicacion de affordance

## Pruebas QT / calidad

- cursor navegable visible en celdas correctas
- cursor no visible en columna `acciones`
- menu contextual funciona correctamente
- navegacion a `respuesta/:id` sigue funcionando
- no existe CSS duplicado
- comportamiento consistente con otros modulos que adopten el patron
- interaccion con teclado funcional

## Criterios de aceptacion

- `GestionCorrespondencia` adopta el contrato reusable
- se elimina completamente la implementacion local equivalente
- no existe duplicacion visual ni tecnica
- la navegacion no se rompe
- accesibilidad y teclado funcionan correctamente
- experiencia visual consistente o mejorada
- el modulo queda alineado con el design system

## Instruccion final

Antes de implementar:

- revisar `GestionCorrespondencia.tsx`
- revisar implementacion del Ticket 10
- identificar CSS local relacionado con affordance

Luego:

- activar prop reusable
- eliminar CSS local
- validar comportamiento completo

Finalmente reportar:

- cambios realizados
- CSS eliminado
- validacion visual
- pruebas ejecutadas
- confirmacion de ausencia de duplicacion
