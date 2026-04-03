## Context

`SCRUMCORE-36` continúa sobre tres piezas ya implementadas en la línea dinámica de `AppTable`:

- Fase de contratos y adapters dinámicos, que normaliza `CellActions` del backend a `AppGridCellAction`
- Fase de action layer, que ya expone `useDynamicUiTableActions`, `payload builder`, `guard`, `behavior resolver`, `presentation resolver` y `executeAction`
- Fase de integración de `workflowInboxgestion` en `gestionCorrespondencia`, que ya renderiza `AppTable` con datos reales

El estado actual del sistema es consistente pero incompleto desde el punto de vista visual. El backend devuelve metadata de acciones para la columna `acciones`, esa metadata se conserva en `AppGridColumn.actions`, pero el adapter final a `ColDef` deja la celda vacía porque todavía no existe un `cellRenderer` reusable dentro de `AppTable`.

La decisión central de esta fase es cerrar ese gap sin romper la arquitectura ya creada:

- `AppTable` sigue siendo el renderer visual único
- la action layer sigue siendo la fuente única de ejecución y disponibilidad
- `gestionCorrespondencia` no recibe lógica especial para pintar ni ejecutar acciones

## Goals / Non-Goals

**Goals:**

- Renderizar acciones dinámicas dentro de columnas `isActionColumn` sin crear un grid paralelo
- Reutilizar la action layer existente para disponibilidad, payload y ejecución
- Soportar al menos `Presentation = icon_button` con layout inline
- Mantener el orden de acciones recibido desde backend
- Mantener la solución reusable para futuras tablas dinámicas que usen `AppTable`

**Non-Goals:**

- No implementar toolbar actions, bulk actions ni menús complejos en esta fase
- No ejecutar navegación real, modales ni descargas
- No mover lógica al módulo `gestionCorrespondencia`
- No redefinir el contrato del backend ni la normalización ya resuelta en fases previas
- No prometer una semántica final de dominio para `client_event`

## Decisions

### 1. El renderer vive dentro de `AppTable` y se inyecta desde el adapter final de columnas

Se agregará un renderer reusable en `src/app/Components/UI/AppTable/renderers/AppTableActionCellRenderer.tsx` y el punto de integración seguirá siendo `appGridToAppTableColumns.ts`.

Rationale:

- el problema es una responsabilidad visual del componente base, no del módulo
- `appGridToAppTableColumns.ts` ya detecta `isActionColumn`, por lo que es el lugar natural para asignar `cellRenderer`
- evita que cada pantalla tenga que duplicar la misma traducción `AppGridColumn.actions -> renderer`

Alternativa descartada:

- renderizar acciones en `gestionCorrespondencia`: rompe reutilización y acopla una capability transversal a un módulo concreto

### 2. El renderer usa params compatibles con AG Grid, no un contrato inventado

El renderer se diseñará sobre `ICellRendererParams<AppTableRow>` extendido con metadata inyectada vía `cellRendererParams`, por ejemplo:

- `appGridColumn`
- `actions`
- `userClaims?`

Rationale:

- respeta la forma real en que AG Grid entrega contexto al renderer
- evita crear una API ficticia que luego requiera más adapters intermedios
- mantiene compatibilidad con el contrato actual de `ColDef`

Alternativa descartada:

- definir props artificiales tipo `{ row, column, actions }`: simplifica en papel, pero no representa el punto real de integración con AG Grid

### 3. El contexto mínimo de ejecución se construye con datos realmente disponibles

El renderer debe construir `DynamicUiActionContext` usando:

- `row` derivado de `params.data`
- `columnKey` derivado de `appGridColumn.field`
- `userClaims?` si se inyectan desde una capa superior

`selectedRows` queda como soporte opcional y solo se incorporará si puede derivarse de forma segura desde el grid o inyectarse explícitamente. No se asumirá como disponible por defecto.

Rationale:

- evita inventar fuentes de datos que hoy no existen en la integración visual final
- permite cumplir el caso actual de cell actions sin abrir trabajo extra de selección múltiple

Alternativa descartada:

- forzar `selectedRows` siempre en el renderer: introduce una dependencia no resuelta en la integración actual

### 4. La disponibilidad se resuelve con una regla explícita de visibilidad y habilitación

La semántica de render será:

- `isVisible = false` -> no renderizar la acción
- `isVisible = true` y `isEnabled = false` -> renderizar deshabilitada
- `isVisible = true` y `isEnabled = true` -> renderizar habilitada

Rationale:

- evita que distintas pantallas interpreten de forma diferente el mismo resultado del guard
- mantiene trazabilidad clara entre metadata, guard y estado visual

Alternativa descartada:

- permitir que cada renderer decida entre ocultar o deshabilitar sin regla fija: genera inconsistencias visuales

### 5. La action layer se reutiliza completa, pero `behavior` solo se clasifica

El flujo de click será:

1. `evaluateActionAvailability`
2. `buildActionPayload`
3. `executeAction`

El `behavior resolver` y el `presentation resolver` se usarán para clasificar metadata y decidir representabilidad mínima, pero no para ejecutar navegación, abrir modales ni descargar archivos.

Rationale:

- mantiene centralizada la lógica crítica ya implementada
- evita duplicar semántica de negocio en el renderer
- deja la fase preparada para futuras extensiones sin sobreprometer comportamiento final

Alternativa descartada:

- ejecutar `navigate`, `modal` o `download` desde el renderer: rompe el desacoplamiento y adelanta fases no incluidas

### 6. El soporte mínimo visual se limita a `icon_button`, con fallback neutro para lo no soportado

La fase debe soportar explícitamente `Presentation = icon_button`. Cualquier presentación no soportada debe ignorarse o renderizar un placeholder neutro sin romper la celda.

Rationale:

- cubre el caso real actual de `workflowInboxgestion`
- evita bloquear la entrega por tratar de completar todo el sistema de presentaciones
- mantiene extensibilidad futura

Alternativa descartada:

- intentar soportar desde ya `menu_item`, toolbar y menús jerárquicos: amplía el alcance de forma innecesaria

## Risks / Trade-offs

- [No existe una fuente explícita de `userClaims` en el renderer final] -> Mitigación: permitir inyección opcional y documentar que la disponibilidad por claims depende de esa provisión
- [`selectedRows` no está resuelto en la UI actual] -> Mitigación: dejarlo fuera del alcance mínimo y soportarlo solo si puede obtenerse de forma explícita
- [La acción `client_event` quede visualmente activa pero sin efecto final de negocio] -> Mitigación: reutilizar `executeAction` y documentar que `behavior` solo se clasifica en esta fase
- [El renderer agregue demasiada lógica al componente base] -> Mitigación: mantener renderer delgado y delegar disponibilidad/payload/ejecución a la action layer existente
- [Presentaciones no soportadas rompan la celda] -> Mitigación: fallback neutro e ignorar acciones no representables

## Migration Plan

1. Crear `AppTableActionCellRenderer.tsx` dentro de `AppTable/renderers`
2. Ajustar `appGridToAppTableColumns.ts` para inyectar `cellRenderer` y `cellRendererParams` en columnas `isActionColumn`
3. Reutilizar `useDynamicUiTableActions` dentro del renderer para disponibilidad, payload y ejecución
4. Agregar pruebas del renderer y pruebas de integración con el hook de acciones
5. Verificar que `GestionCorrespondencia` muestre contenido real en la columna `acciones` sin cambiar su wiring de pantalla

Rollback:

- revertir el nuevo renderer y el ajuste del adapter de columnas
- volver al comportamiento actual de columna vacía sin afectar query, action layer ni integración de pantalla

## Open Questions

- De dónde se obtendrán finalmente los `userClaims` para el renderer si se quiere disponibilidad completa por claims en todas las pantallas
- Si `selectedRows` debe resolverse en esta fase o quedar para una fase posterior vinculada a acciones masivas
- Si el fallback visual para presentaciones no soportadas debe ser silencioso o mostrar un placeholder explícito de UI
