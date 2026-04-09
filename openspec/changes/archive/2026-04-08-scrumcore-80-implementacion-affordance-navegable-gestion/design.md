# Design

## Summary

`SCRUMCORE-80` no implementa una nueva capacidad shared. Su responsabilidad es adoptar en `GestionCorrespondencia` el contrato reusable de `AppTable` ya introducido por `SCRUMCORE-79`, eliminando el CSS local que resolvia el mismo patron y preservando el comportamiento actual de navegacion del modulo.

## Decisions

### 1. El shared component ya resuelve la affordance

`AppTable` ya expone una prop opt-in para affordance navegable y soporte de teclado:

```ts
rowClickAffordance?: boolean;
```

Por lo tanto, `GestionCorrespondencia` debe comportarse como consumidor del contrato y no como segundo punto de implementacion visual.

### 2. La navegacion permanece en el modulo

La adopcion de `rowClickAffordance` no mueve la navegacion al shared component.

`GestionCorrespondencia` conserva:

- `handleTableCellClick`
- `handleTableAction`
- el flujo `navigate(\`respuesta/:id\`)`

Esto mantiene la separacion correcta entre:

- affordance visual y teclado: `AppTable`
- decision de dominio y routing: `GestionCorrespondencia`

### 3. Eliminacion completa del CSS local equivalente

El CSS actual del modulo:

- `gridClassName={styles.navigableGrid}`
- reglas `.navigableGrid ...`

debe eliminarse una vez activada la prop reusable del shared component.

No debe quedar:

- doble affordance
- duplicacion visual
- reglas muertas o residuales del mismo patron

### 4. Alcance funcional sin cambios de UX

La experiencia observable debe seguir siendo la misma o mejor:

- celdas de datos navegables
- columna `acciones` sin affordance navegable
- columna de seleccion sin affordance navegable
- menu contextual sin regresiones
- `Enter` funcionando mediante el shared component

### 5. Sin cambios de rutas ni layout

Este ticket no cambia:

- `respuesta/:id`
- shell persistente del modulo
- action layer de `AppTable`
- comportamiento de seleccion

Solo adopta el contrato reusable y elimina la implementacion local equivalente.

## Affected Areas

- `src/modules/gestionCorrespondencia/pages/GestionCorrespondencia.tsx`
- `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`
- `src/modules/gestionCorrespondencia/tests/*`

## Verification Strategy

- verificar que `GestionCorrespondencia` usa `rowClickAffordance`
- verificar que deja de usar `gridClassName={styles.navigableGrid}`
- verificar que se elimina el CSS local equivalente
- verificar que la navegacion por celda de datos sigue funcionando
- verificar que la columna `acciones` no navega por click de celda
- verificar que el menu contextual sigue intacto
- verificar que `Enter` sigue funcionando a traves de `AppTable`
