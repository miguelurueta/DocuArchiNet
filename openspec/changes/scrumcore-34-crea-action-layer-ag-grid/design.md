## Context

`SCRUMCORE-34` extiende la línea dinámica de `AppTable` después de dos fases previas ya implementadas: Fase 1B estabilizó contratos y adapters para tablas dinámicas; Fase 2 agregó el query layer con `clienteApi` y React Query. En el estado actual, la metadata de acciones ya no vive solo como `UiActionDto` crudo del backend: la fase 1B la normaliza a `AppGridCellAction` dentro de `AppDataTableAgGrid`, preservando `behavior`, `presentation`, `request`, `claims`, `rules`, `payload` y `metadata`.

El cambio buscado en esta fase no es visual. Debe agregar una capa reusable para ejecutar acciones dinámicas sin contaminar el grid base, sin reimplementar la normalización ya resuelta y sin acoplarse a un módulo funcional. Además, el repo ya consolidó esta capability dentro de `src/app/Components/UI/AppTable/`, por lo que mover Fase 3 a `src/features/dynamic-ui-table/` partiría la misma arquitectura en dos lugares.

## Goals / Non-Goals

**Goals:**
- Implementar una capa de ejecución de acciones dinámica compatible con `AppTable`, Fase 1B y Fase 2.
- Centralizar mutaciones HTTP de acciones sobre `clienteApi`, con endpoint default e inyectable.
- Exponer helpers puros para resolver `behavior`, `presentation`, payload y disponibilidad.
- Orquestar esas piezas desde un hook con React Query, sin lógica UI ni dominio.
- Preparar la integración futura de toolbar, row actions, cell actions y bulk actions.

**Non-Goals:**
- No modificar `AppTable.tsx` ni renderizar botones, menús, modales o navegación.
- No duplicar la capa de normalización de acciones ya resuelta por `dynamicUiActionMapper.ts`.
- No mover la capability a `src/features/dynamic-ui-table/`.
- No evaluar reglas inseguras o semánticamente ambiguas que solo backend debe resolver.
- No cerrar todavía la integración visual final con contenedores o componentes de acción; esa queda para la fase siguiente.

## Decisions

### 1. La fase continúa dentro de `AppTable`, no en `src/features`

La nueva capa debe vivir en `src/app/Components/UI/AppTable/` junto a `types`, `services`, `hooks`, `utils` y `tests` ya existentes. Esto preserva continuidad con Fase 1B y Fase 2, evita fragmentar la capability y mantiene un único punto de verdad para contratos, adapters, query y actions.

Alternativa descartada: crear `src/features/dynamic-ui-table/`. Esa ruta introduce una frontera artificial que el repo actual no usa para esta capability y obliga a duplicar imports, contratos y reglas arquitectónicas.

### 2. La unidad principal de trabajo es la acción normalizada del frontend

El action layer debe operar principalmente sobre `AppGridCellAction`, no exclusivamente sobre `UiActionDto`. La fase 1B ya convirtió el backend a un modelo frontend estable con `actionId`, `behavior`, `presentation`, `request`, `requiredClaimsAny`, `requiredClaimsAll`, `claimKey`, `rules`, `payload` y `metadata`.

La capa nueva puede tolerar compatibilidad opcional con `UiActionDto`, pero no debe depender de ella como input principal. Así evita repetir la normalización, reduce acoplamiento al shape remoto y se alinea con lo que hoy devuelve `useDynamicUiTableQuery`.

### 3. El contexto de acciones debe incluir claims de usuario

`DynamicUiActionContext` debe incluir `userClaims?` además de `row`, `selectedRows`, `columnKey` y `tableId`. Sin claims en contexto, el guard no puede evaluar correctamente `RequiredClaimsAny`, `RequiredClaimsAll` y `ClaimKey`, que ya forman parte del modelo normalizado de acciones.

Alternativa descartada: resolver claims desde estado global o hooks de dominio. Eso acoplaría una capa transversal a infraestructura o módulos concretos y volvería opaco el contrato del guard.

### 4. El servicio de acciones replica el patrón reusable del query layer

El servicio HTTP debe ser delgado, usar `clienteApi`, devolver `ApiResponse<unknown>` y soportar tres formas: endpoint default, endpoint explícito por invocación y factory ligada a endpoint. Esta decisión replica lo aprendido en Fase 2, donde un endpoint fijo resultó demasiado rígido para una capability transversal.

Alternativa descartada: hardcodear un único endpoint o esconder el endpoint dentro del hook. Eso limita la reutilización y mezcla concerns de transporte con concerns de orquestación.

### 5. El hook solo orquesta; los utils permanecen puros

`useDynamicUiTableActions` debe ser el único punto con React Query. El hook compone:
- action service
- payload builder
- action guard
- behavior resolver
- presentation resolver

Los cuatro últimos deben ser funciones puras y deterministas. De este modo:
- el hook maneja estado de mutación y errores
- los utils se prueban aislados
- la lógica no queda atada a React ni al grid visual

### 6. `behavior` y `presentation` se resuelven como strings extensibles

Los resolvers deben clasificar valores conocidos como `api_call`, `navigate`, `modal`, `download`, `emit`, `custom`, `client_event`, `button`, `menu_item`, `icon`, `icon_button`, pero siempre retornar `kind`, `rawValue`, `isKnown` y `config?` sin usar enums cerrados ni ejecutar el comportamiento.

Alternativa descartada: enums rígidos. Eso chocaría con el principio contract-first del backend dinámico y obligaría a cambiar frontend por cada nuevo valor.

### 7. El guard solo evalúa reglas seguras de frontend

La evaluación de disponibilidad debe cubrir claims y reglas interpretables sin ambigüedad en frontend. Si una regla no es segura o no tiene semántica clara del lado cliente, el guard no debe “adivinar” su resultado. Debe documentar el límite y devolver razones en la salida.

Esta decisión evita duplicar lógica crítica del backend y mantiene la capa segura frente a reglas más expresivas que puedan aparecer luego.

## Risks / Trade-offs

- [Se mezclan acciones backend crudas y acciones ya normalizadas] -> Mitigación: declarar explícitamente `AppGridCellAction` como unidad principal y solo tolerar compatibilidad opcional con `UiActionDto`.
- [La ubicación en `src/features` rompa continuidad de la capability] -> Mitigación: mantener toda la fase bajo `src/app/Components/UI/AppTable/`.
- [Falta de claims en contexto impida evaluar guards] -> Mitigación: incluir `userClaims?` en `DynamicUiActionContext`.
- [Un endpoint único limite reutilización] -> Mitigación: servicio con endpoint default e inyectable, más factory por endpoint.
- [El frontend intente resolver reglas no seguras] -> Mitigación: limitar el guard a reglas interpretables y documentar los límites.
- [La fase se desvíe hacia UI final] -> Mitigación: mantener como non-goal explícito la integración visual y reservarla para la siguiente fase.

## Migration Plan

1. Crear `dynamicUiTableAction.types.ts` con contratos corregidos y compatibles con `AppGridCellAction`.
2. Implementar `dynamicUiAction.service.ts` sobre `clienteApi` con endpoint default, endpoint inyectable y factory por endpoint.
3. Implementar `dynamicUiActionPayloadBuilder.ts`, `dynamicUiActionGuard.ts`, `dynamicUiActionBehaviorResolver.ts` y `dynamicUiActionPresentationResolver.ts` como funciones puras.
4. Implementar `useDynamicUiTableActions.ts` como capa de orquestación con React Query.
5. Agregar pruebas unitarias de service, hook y utils.
6. Documentar cómo esta fase se conecta con Fase 1B y Fase 2 y por qué no toca el grid base.

Rollback: al ser una capa nueva y transversal, revertir implica eliminar los archivos agregados en `AppTable` Fase 3 y restaurar la documentación asociada. No hay migraciones persistentes ni cambios de datos.

## Open Questions

- Qué endpoint real o conjunto de endpoints se usarán para la primera ejecución de acciones; el diseño asume que puede existir un default, pero no fuerza unicidad.
- Qué subset de `Rules` es considerado “seguro” de evaluar en frontend en esta etapa y cuál debe quedar explícitamente fuera.
- Si el hook debe aceptar solo `AppGridCellAction` en su API pública o mantener compatibilidad adicional con `UiActionDto` por transición.
