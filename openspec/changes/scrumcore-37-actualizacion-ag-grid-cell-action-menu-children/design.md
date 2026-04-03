## Context

`SCRUMCORE-37` continúa sobre el estado dejado por `SCRUMCORE-36`:

- la columna `acciones` ya renderiza un trigger visual dentro de `AppTable`
- el trigger ya puede abrir un `Dropdown` cuando la acción principal viene como `client_event` con `behaviorConfig.menuItems`
- la action layer compartida ya existe y sigue siendo la única vía de ejecución

Sin embargo, el comportamiento actual del menú sigue incompleto. Hoy el renderer solo puede humanizar ids como texto porque la resolución real de `menuItems` contra acciones completas todavía no existe en frontend. El nuevo contrato backend agregará `MenuActions`, `Children` e `IsDivider`, y esta fase debe consumirlo sin abrir lógica específica de negocio ni degradar el comportamiento existente del `AppDropdown` compartido.

La decisión principal de esta fase es mover la resolución del menú a la capa compartida de `AppTable`, preservando esta secuencia:

`DynamicUiTableDto -> modelo interno dinámico -> renderer de acción -> AppDropdown`

## Goals / Non-Goals

**Goals:**

- Modelar y preservar `MenuActions` desde el DTO backend hasta la capa de render de `AppTable`
- Resolver `BehaviorConfig.menuItems` contra `MenuActions` por `ActionId`
- Mapear acciones resueltas a `AppDropdownItem[]`
- Soportar `Children` de forma recursiva
- Soportar `IsDivider` como separador visual reutilizable en `AppDropdown`
- Mantener la action layer existente como única vía de ejecución para items válidos

**Non-Goals:**

- No cambiar el contrato backend dentro de esta fase frontend
- No introducir lógica de dominio de `gestionCorrespondencia`
- No rediseñar `AppDropdown` completo fuera de lo necesario para `divider`
- No crear una infraestructura paralela de menú fuera de `AppDropdown`
- No romper las acciones directas ya soportadas en `SCRUMCORE-36`

## Decisions

### 1. `MenuActions` se modela tanto en DTO como en modelo interno compartido

El frontend no debe limitarse a leer `MenuActions` en `DynamicUiTableDto`; debe preservarlo hasta el modelo interno consumido por el renderer. Esto implica extender:

- `DynamicUiTableDto`
- `AppDataTableAgGrid`
- la salida de `useDynamicUiTableQuery` o la capa que hoy transporta metadata necesaria al renderer

Rationale:

- si `MenuActions` se pierde en la normalización intermedia, el renderer vuelve a quedar forzado a inventar labels o estructuras
- la resolución del menú es concern de la tabla compartida, no del módulo consumidor

Alternativa descartada:

- resolver `menuItems` directamente contra el DTO raw en la pantalla consumidora: rompe la separación lograda en fases previas

### 2. La resolución `menuItems -> MenuActions -> AppDropdownItem[]` vive en la capa compartida de `AppTable`

La lógica para resolver ids de menú y construir items del dropdown debe vivir junto al renderer de `AppTable`, no en `gestionCorrespondencia`.

Rationale:

- la columna `acciones` es una capability transversal
- cualquier otra pantalla dinámica debería reutilizar exactamente la misma traducción
- evita duplicar código de resolución por tabla o módulo

Alternativa descartada:

- dejar que cada módulo traduzca `MenuActions`: acopla la semántica del contrato backend a features específicas

### 3. `AppDropdown` se extiende de forma mínima para soportar divisores

El componente compartido ya soporta `children`, `disabled`, `icon` y `onSelect`, pero no expone explícitamente un `type: "divider"` en su contrato público. Esta fase debe ampliar el shape de `AppDropdownItem` lo justo para soportar separadores visuales sin rediseñar el componente.

Rationale:

- alinear el contrato frontend con `IsDivider` del backend
- evitar inventar representaciones ad hoc dentro del renderer
- conservar compatibilidad con consumidores actuales de `AppDropdown`

Alternativa descartada:

- renderizar divisores manualmente fuera de `AppDropdown`: rompe la reutilización y duplica la semántica del menú

### 4. `Children` se resuelve recursivamente y no usa `menuItems`

Los `Children` deben venir completamente resueltos desde backend y mapearse de forma recursiva a `children` en `AppDropdownItem`. No se debe mezclar la resolución por ids de `menuItems` con la jerarquía declarada por `Children`.

Rationale:

- refleja fielmente el contrato backend actualizado
- evita una doble estrategia de resolución para submenús
- simplifica la recursividad del mapper

Alternativa descartada:

- permitir `menuItems` dentro de `Children`: vuelve ambigua la resolución y complica innecesariamente el mapper

### 5. Los divisores y los items inválidos nunca pasan por la action layer

Solo las acciones resueltas y ejecutables deben reutilizar:

- guard
- payload builder
- resolvers
- `executeAction`

Los divisores (`IsDivider = true`) y los ids no resueltos deben transformarse a output visual o ignorarse sin ejecutar nada.

Rationale:

- protege la action layer de entradas que no representan acciones reales
- mantiene clara la frontera entre estructura visual y semántica ejecutable

Alternativa descartada:

- dejar que el renderer intente ejecutar cualquier item renderizado: introduciría errores silenciosos y semántica incorrecta

### 6. El fallback debe ser silencioso y controlado

Si `MenuActions` no existe, viene vacío o falta alguna resolución individual, el render no debe romperse. El dropdown solo debe abrirse si la resolución produce al menos un item válido. Si no hay items válidos, debe mantenerse un fallback estable sin error fatal.

Rationale:

- protege compatibilidad hacia atrás
- evita que respuestas parciales rompan la tabla
- reduce coupling entre rollout backend y frontend

Alternativa descartada:

- lanzar error o dejar celdas rotas cuando falte resolución: no es aceptable en una tabla compartida

## Risks / Trade-offs

- [El contrato backend puede llegar parcial y no traer `MenuActions`] -> Mitigación: fallback silencioso y preservación de acción directa cuando no haya items válidos
- [`AppDropdown` puede verse afectado para otros consumidores] -> Mitigación: extender su contrato de forma mínima y con tests de regresión
- [La recursividad de `Children` puede introducir estructuras inválidas] -> Mitigación: ignorar ramas inconsistentes y mantener mapeo puro sin mutaciones
- [Los divisores pueden mezclarse con acciones ejecutables por error backend] -> Mitigación: dar prioridad semántica a `IsDivider` y descartar ejecución
- [La metadata se puede perder antes del renderer] -> Mitigación: propagar `MenuActions` explícitamente por los tipos y adapters intermedios

## Migration Plan

1. Extender tipos compartidos para soportar `MenuActions`, `Children` e `IsDivider`
2. Ajustar mappers y adapters para preservar `MenuActions` en el modelo interno
3. Implementar la resolución de menú en la capa compartida del renderer
4. Extender `AppDropdown` para soportar divisores
5. Agregar pruebas de resolución, recursividad, divisores y regresión
6. Validar que `GestionCorrespondencia` siga renderizando la columna `acciones` sin lógica adicional

Rollback:

- revertir la resolución de `MenuActions` y la ampliación de `AppDropdown`
- volver al comportamiento actual de dropdown basado en ids humanizados o fallback visual, sin tocar query ni action layer

## Open Questions

- Si el backend enviará `MenuActions` siempre que exista `menuItems`, o si habrá respuestas mixtas durante la transición
- Cómo se quiere representar visualmente un dropdown sin items válidos: trigger visible sin apertura o fallback visual silencioso
- Si `IsDivider` debe coexistir con otros metadatos opcionales ignorables, o si se tratará como shape estrictamente exclusivo desde backend
