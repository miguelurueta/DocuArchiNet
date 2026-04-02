## Context

`AppTable` ya existe como base reusable y presentacional. El ticket `SCRUMCORE-30` no pide conectarlo a un modulo ni a una API, sino construir la capa intermedia que reciba el contrato dinamico real del backend y lo traduzca a un modelo interno estable (`AppDataTableAgGrid`) listo para futuras fases.

El payload real del backend incluye particularidades que la fase debe absorber: propiedades en PascalCase, metadata de filtros (`FilterType`, `AgGridFilterType`), orden de columnas (`Order`), acciones anidadas en `CellActions[].Action`, `Pagination`, `Sorting` con `SortField`/`SortDir` y `meta` a nivel de respuesta o tabla.

La restriccion central del ticket es mantener pureza y separacion de capas: sin React, sin HTTP, sin React Query, sin side effects y sin acoplamiento a modulos de producto.

## Goals / Non-Goals

**Goals:**
- Definir contratos frontend tolerantes a PascalCase/camelCase para el payload dinamico real.
- Adaptar columnas, filas, acciones y metadata a un modelo interno `AppDataTableAgGrid`.
- Preservar metadata util del backend sin trasladar el shape crudo a capas visuales.
- Cubrir el comportamiento con pruebas unitarias puras sobre adapters y normalizacion.

**Non-Goals:**
- Modificar `AppTable`, introducir props nuevas o cambiar su contrato publico.
- Conectar endpoints, `clienteApi`, React Query o contenedores React.
- Ejecutar acciones, navegar o interpretar logica de negocio de un modulo especifico.
- Implementar renderers visuales finales de acciones o estados.

## Decisions

- **Separacion de contratos remotos e internos**: se modelan DTOs backend (`DynamicUiTableDto`, `UiColumnDto`, `UiRowDto`, `UiActionDto`) y contratos internos (`AppGridColumn`, `AppGridRow`, `AppGridCellAction`, `AppDataTableAgGrid`) como tipos distintos. Alternativa descartada: reutilizar el mismo shape del backend en toda la UI.

- **Normalizacion tolerante a payload real**: se aceptan claves `PascalCase` y `camelCase`, y se soportan particularidades reales como `CellActions[].Action`, `SortField`, `SortDir` y `meta`. Alternativa descartada: imponer un contrato frontend mas estricto que el backend actual.

- **Adapters puros por responsabilidad**: columnas, filas y acciones se mapean en funciones separadas y un ensamblador final compone `AppDataTableAgGrid`. Alternativa descartada: un unico mapper monolitico.

- **Orden y visibilidad resueltos en la capa de adaptacion**: `Visible=false` excluye la columna del resultado y `Order` define la secuencia final. Alternativa descartada: delegar ese orden a la capa visual.

- **Metadata preservada, no ejecutada**: las acciones mantienen `behavior`, `presentation`, `request`, claims y metadata, pero no ejecutan nada en esta fase. Alternativa descartada: mezclar adaptacion con comportamiento.

## Risks / Trade-offs

- [El backend entrega acciones anidadas en `CellActions.Action`] -> Mitigacion: desanidar en el mapper sin alterar el shape de salida.
- [El contrato real mezcla convenciones de nombres] -> Mitigacion: lectura tolerante de aliases y pruebas con payload real.
- [Se pierda metadata util de filtros u orden] -> Mitigacion: incluir `Order`, `FilterType`, `AgGridFilterType` y `FilterOptions` en el modelo interno.
- [Se intente ampliar el alcance hacia HTTP o React] -> Mitigacion: mantener la fase limitada a tipos, adapters, tests y documentacion.

## Migration Plan

- La fase no requiere migracion ni despliegue especial.
- Rollback: eliminar tipos, adapters, tests y documentacion agregados en `AppTable`, sin tocar el componente base.

## Open Questions

- Confirmar si `ToolbarActions` y `BulkActions` usaran el mismo mapper en la siguiente fase o si requieren diferencias de salida.
- Definir si el ensamblador final debe vivir en `adapters/` o separarse a `assemblers/` cuando crezca el contrato.
