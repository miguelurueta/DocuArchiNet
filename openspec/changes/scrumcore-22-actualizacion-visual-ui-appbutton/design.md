# Design

## Context

`SCRUMCORE-22` pide un ajuste visual concreto sobre la pantalla `GestionCorrespondencia`: agregar un nuevo botón `Actualizar` dentro del `AppToolbar`, junto a las acciones actuales, usando el wrapper enterprise `AppButton`. El botón no debe introducir lógica de negocio real, solo una acción placeholder coherente con el sistema visual.

Además, el ticket exige que `.page` use `background-color: white`, manteniendo la estructura actual de la página y el comportamiento responsive ya existente.

## Goals / Non-Goals

### Goals

- Agregar el botón `Actualizar` con `AppButton` dentro del grupo actual de acciones del toolbar.
- Mantener el orden visual y el wrap responsive sin romper la composición actual.
- Usar `UndoOutlined` como icono izquierdo.
- Aplicar `background-color: white` en `.page`.

### Non-Goals

- Cambiar la lógica interna de `AppToolbar`.
- Introducir una acción real de recarga de datos.
- Modificar otros botones existentes más allá de convivir con el nuevo.
- Cambiar el contrato de `AppButton`.

## Decisions

### 1. El botón se implementará localmente en `GestionCorrespondencia.tsx`

El ticket apunta a una vista concreta y no a una extensión del componente `AppToolbar`. Por eso la decisión es agregar el nuevo botón dentro de `actionContent` en `GestionCorrespondencia.tsx`, junto a `Exportar` y `Abrir respuesta contextual`.

### 2. `handleRefresh` será un placeholder sin lógica de negocio

La especificación pide explícitamente no implementar lógica real. La acción se representará con un callback local simple, por ejemplo un `console.log`, para dejar el punto de integración listo sin acoplarlo a APIs o estado de negocio.

### 3. El estilo de fondo se limita a `.page`

El requerimiento de `background-color: white` se aplica sobre `.page`, que ya es el wrapper principal de la vista. Así se respeta el alcance indicado sin tocar componentes compartidos.

## Risks / Trade-offs

- Añadir una nueva acción puede compactar más el grupo de botones en mobile; se acepta porque el toolbar ya soporta wrap.
- El callback placeholder no produce efecto funcional real; eso es consistente con el ticket y evita sobre-implementar.
