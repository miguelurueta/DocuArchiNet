# Design

## Summary

`AppTable` necesita exponer una affordance navegable reusable para escenarios donde una fila o celda de datos representa una accion primaria, sin asumir navegacion por defecto ni acoplarse a un modulo especifico. La solucion debe ser opt-in, visible solo en superficies navegables y compatible con la navegacion por teclado del grid.

## Decisions

### 1. Contrato shared opt-in

Se agrega al contrato de `AppTable` una prop booleana:

```ts
rowClickAffordance?: boolean;
```

Semantica:

- `false` por defecto
- comunica que la tabla tiene una accion primaria sobre fila o celdas de datos
- habilita affordance visual reusable
- habilita soporte de teclado con `Enter`
- no ejecuta navegacion automaticamente

La navegacion real sigue perteneciendo al consumidor a traves de callbacks existentes como `onCellClicked` y `onRowClicked`.

### 2. Alcance de la affordance

La affordance se aplica solo a celdas de datos del modo tabla de AG Grid.

Se excluyen:

- columna de acciones (`app-table-action-cell`)
- columna de seleccion (`ag-Grid-SelectionColumn`)
- celdas explicitamente no navegables

Esto evita dar una senal UX incorrecta sobre superficies que ya tienen su propio patron interactivo.

### 3. Implementacion en el grid layer

La implementacion debe vivir en `AppTableGridRenderer` usando `cellClass` o `cellClassRules`.

No se debe:

- manipular DOM manualmente
- inyectar CSS desde el modulo consumidor
- cambiar bubbling de eventos existentes

Esto mantiene la capacidad dentro del shared component y evita duplicacion entre modulos.

### 4. Estilo reusable

Los estilos van en `AppTable.module.css` y deben cubrir:

- `cursor: pointer`
- hover ligero consistente con el design system
- transicion suave

El estilo debe vivir en una clase reusable del grid y no depender de un modulo concreto.

### 5. Controles internos

Si una celda contiene controles internos como botones, links o inputs:

- el elemento interno conserva su comportamiento propio
- la affordance no debe sobreescribir su cursor
- la affordance no debe interceptar sus eventos

La clase se aplica a la celda, no al control interno.

### 6. Soporte de teclado

Cuando `rowClickAffordance` este activo, el grid debe responder a `Enter` sobre una celda navegable del mismo modo observable que la accion primaria del consumidor.

Reglas:

- no duplicar logica de navegacion dentro de `AppTable`
- reutilizar el flujo actual del callback del consumidor
- no activarse en acciones, seleccion o controles interactivos internos
- no romper la navegacion propia de AG Grid

### 7. Sin adopcion de modulo en este ticket

Este ticket implementa la capacidad shared en `AppTable`.

La adopcion en `GestionCorrespondencia` queda fuera de este alcance y se resuelve en el ticket dependiente `11-FE`, donde se reemplazara `gridClassName={styles.navigableGrid}` por la prop reusable y se eliminara el CSS local.

## Affected Areas

- `src/app/Components/UI/AppTable/AppTable.tsx`
- `src/app/Components/UI/AppTable/AppTable.types.ts`
- `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
- `src/app/Components/UI/AppTable/AppTable.module.css`
- `src/app/Components/UI/AppTable/tests/*`

## Verification Strategy

- verificar que la affordance no aplica por defecto
- verificar que la affordance aplica solo con `rowClickAffordance`
- verificar exclusiones para `acciones` y seleccion
- verificar no interferencia con callbacks existentes
- verificar que `Enter` dispara la accion primaria esperada sin hardcodear navegacion
- verificar que controles internos mantienen su comportamiento
