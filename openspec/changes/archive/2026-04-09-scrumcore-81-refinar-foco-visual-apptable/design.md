## Context

`AppTable` ya implementa `rowClickAffordance` para expresar que una celda o fila tiene una accion primaria observable del consumidor. Para soportar teclado, la implementacion actual deja `suppressCellFocus = false` cuando ese contrato esta activo.

Ese enfoque resolvio el soporte de `Enter`, pero tambien dejo visible la decoracion por defecto de AG Grid sobre la celda enfocada. En la UI eso se percibe como un estado adicional parecido a seleccion de celda, que compite con la seleccion real de fila y con el hover navegable.

El ajuste debe hacerse dentro del shared component. No corresponde mover la responsabilidad a `GestionCorrespondencia` ni a otro modulo consumidor, porque el problema nace del contrato reusable y de la integracion con AG Grid.

## Goals / Non-Goals

**Goals:**
- Mantener foco funcional del grid para teclado y accesibilidad.
- Conservar `Enter` como disparador de la accion primaria ya definida por `rowClickAffordance`.
- Eliminar la percepcion visual de doble seleccion cuando una celda navegable recibe foco.
- Encapsular la solucion dentro de `AppTable` sin introducir nuevos props.
- Preservar exclusion de columnas de acciones, seleccion y controles interactivos internos.

**Non-Goals:**
- Rediseñar la navegacion funcional de `AppTable`.
- Cambiar el contrato de `onCellClicked`, `onRowClicked` o `onActionTriggered`.
- Alterar el comportamiento de `presentationMode="cards"` salvo para verificar ausencia de regresion.
- Introducir estilos globales de AG Grid fuera del scope del shared component.

## Decisions

### 1. Mantener foco funcional y desacoplar solo la capa visual

Se mantendra `suppressCellFocus = false` cuando `rowClickAffordance` este activo. Esa parte ya demostro ser necesaria para soportar teclado con `Enter` en el grid.

La correccion se aplicara exclusivamente sobre la representacion visual del foco (`ag-cell-focus` y estados relacionados), no sobre la existencia del foco.

**Por que esta opcion:**
- preserva accesibilidad y teclado sin hacks
- evita rediseñar el flujo funcional ya validado
- mantiene el comportamiento consistente entre tablas que usen el contrato reusable

**Alternativa descartada:**
- volver a `suppressCellFocus = true`
  - descartada porque reabre el riesgo de perder operabilidad por teclado

### 2. Usar override CSS scoped activado por la clase raiz del affordance

La solucion visual se apoyara en una clase raiz ya existente o reusable del grid cuando `rowClickAffordance` este activo. Sobre ese scope se redefinira la decoracion visual del foco de AG Grid para que deje de verse como una seleccion adicional.

**Por que esta opcion:**
- evita overrides globales
- respeta el alcance opt-in del contrato reusable
- mantiene la correccion localizada al shared component

**Alternativa descartada:**
- estilos globales sobre `.ag-cell-focus`
  - descartada porque afectaria tablas sin `rowClickAffordance`

### 3. Mantener jerarquia clara entre hover, foco y seleccion

La jerarquia visual buscada sera:
- seleccion de fila: estado principal y mas fuerte
- hover navegable: affordance sutil
- foco de celda: estado tecnico, visible solo de forma no dominante o neutralizada visualmente

Esto implica que el foco ya no debe competir con el color o borde de seleccion de fila.

**Por que esta opcion:**
- reduce ambiguedad en la UI
- alinea el comportamiento con la expectativa del usuario: click abre detalle, seleccion indica eleccion persistente

### 4. No afectar foco visible de controles interactivos internos

El override debe apuntar al foco de celda del grid, no al foco interno de botones, links, inputs o menus renderizados dentro de la celda.

**Por que esta opcion:**
- esos controles necesitan conservar su accesibilidad y su propio estado visible
- el problema actual esta en la celda contenedora, no en los elementos interactivos

## Risks / Trade-offs

- **[Riesgo]** Un override CSS demasiado amplio puede degradar otras tablas de AG Grid.  
  **Mitigacion:** aplicar el override solo bajo el scope visual activado por `rowClickAffordance`.

- **[Riesgo]** Neutralizar demasiado el foco podria reducir claridad para usuarios de teclado.  
  **Mitigacion:** mantener el foco funcional y validar `Enter`, `Tab` y el comportamiento observable con tests focales.

- **[Riesgo]** El estilo de AG Grid puede combinar varias clases de foco/seleccion que no se resuelven con una sola regla.  
  **Mitigacion:** revisar `ag-cell-focus` y estados relacionados en runtime antes del ajuste final, y cubrir el caso con pruebas del renderer.

- **[Trade-off]** El foco quedara menos visible que antes en tablas navegables.  
  **Mitigacion:** conservar hover navegable y seleccion de fila como estados principales de comunicacion visual.
