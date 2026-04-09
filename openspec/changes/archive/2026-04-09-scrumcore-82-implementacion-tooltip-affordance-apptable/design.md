## Context

`AppTable` ya soporta `rowClickAffordance` para comunicar que una celda o registro tiene una accion primaria resuelta por el consumidor. Hoy esa comunicacion se apoya en cursor, hover y foco funcional, pero no ofrece una pista textual contextual reusable.

El ticket busca agregar esa capa de affordance textual sin acoplar `AppTable` a ningun dominio y sin introducir regresiones de performance en AG Grid. La principal complejidad no esta en cards, sino en el modo `table`, donde envolver cada celda navegable con un componente de tooltip pesado puede degradar render y churn de hover.

## Goals / Non-Goals

**Goals:**
- Exponer un contrato reusable y opt-in para tooltip de affordance navegable.
- Mantener `rowClickAffordance` como requisito funcional de base para activar la pista textual.
- Cubrir grid y cards sin alterar la navegacion real.
- Excluir acciones, seleccion y controles interactivos internos.
- Controlar el costo de render en AG Grid.

**Non-Goals:**
- Rediseñar el flujo funcional de `rowClickAffordance`.
- Mover la navegacion a `AppTable`.
- Introducir texto de dominio hardcodeado en el shared component.
- Cambiar el contrato de `onCellClicked`, `onRowClicked` o `onActionTriggered`.

## Decisions

### 1. El contrato sera opcional y dependiente de `rowClickAffordance`

Se agregara una prop reusable tipo `rowClickTooltip?: string`.

Su efecto existira solo cuando `rowClickAffordance` este activo. Si el consumidor informa texto sin affordance navegable, `AppTable` no asumira navegabilidad.

**Por que esta opcion:**
- mantiene semantica clara
- evita tooltips accidentales en tablas no navegables
- conserva el desacople respecto al dominio

### 2. Cards usaran wrapper directo y grid una estrategia liviana

En `presentationMode="cards"` la superficie navegable es mas gruesa y de menor cardinalidad, por lo que un wrapper directo con la primitiva de tooltip del design system es razonable.

En modo `table`, la implementacion debe evitar un wrapper React costoso por cada celda navegable. La integracion debe ser liviana y condicionada a superficies validas, manteniendo el tooltip fuera de acciones, seleccion y controles internos.

**Por que esta opcion:**
- cards tienen menor riesgo de costo por render
- AG Grid exige mayor cuidado en cantidad de nodos y wrappers
- separa correctamente el problema de UX del costo de render

### 3. La pista textual sigue siendo presentacional

El tooltip no resuelve navegacion. Solo describe la accion primaria esperada. El mensaje viene del consumidor y `AppTable` se limita a presentarlo en superficies elegibles.

**Por que esta opcion:**
- evita acoplamiento a modulos
- mantiene el componente reusable y declarativo

### 4. Las exclusiones se resuelven reutilizando la semantica actual de affordance

La elegibilidad del tooltip debe apoyarse en la misma logica base ya usada para superficies navegables:
- sin columna `acciones`
- sin columna de seleccion
- sin controles interactivos internos

**Por que esta opcion:**
- reduce divergencia entre affordance visual y affordance textual
- baja riesgo de inconsistencias entre grid y cards

## Risks / Trade-offs

- **[Riesgo]** Instanciar tooltips pesados por celda puede degradar rendimiento en tablas grandes.  
  **Mitigacion:** definir una estrategia liviana para grid y reservar wrappers directos para cards o superficies puntuales.

- **[Riesgo]** El tooltip puede introducir ruido visual si aparece sobre demasiadas superficies.  
  **Mitigacion:** activacion opt-in, delay de aparicion y exclusiones estrictas.

- **[Riesgo]** Controles internos pueden entrar en conflicto con la capa del tooltip.  
  **Mitigacion:** excluir botones, links, inputs y menus de la superficie navegable del tooltip.

- **[Trade-off]** La solucion de grid puede ser mas limitada que la de cards para proteger performance.  
  **Mitigacion:** priorizar consistencia funcional y costo de render antes que simetria exacta de implementacion.
