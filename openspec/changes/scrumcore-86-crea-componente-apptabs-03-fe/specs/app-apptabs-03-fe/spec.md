## ADDED Requirements

### Requirement: Sincronizacion con router
El componente SHALL sincronizar la tab activa con el URL cuando `syncWithRouter` este habilitado, soportando path segment y query param `?tab=`.

#### Scenario: Resolver tab desde query param
- **WHEN** el URL contiene `?tab=<key>` y `syncWithRouter` es `true`
- **THEN** el componente SHALL activar la tab con ese `key`

#### Scenario: Resolver tab desde path segment
- **WHEN** no existe `?tab=` y el ultimo segmento del path coincide con un `key`
- **THEN** el componente SHALL activar la tab correspondiente

#### Scenario: Fallback de tab invalida
- **WHEN** el `key` del URL no existe en `items`
- **THEN** el componente SHALL activar el primer tab habilitado

#### Scenario: Conflicto activeKey vs router
- **WHEN** `activeKey` esta definido y `syncWithRouter` es `true`
- **THEN** el router SHALL dominar sobre `activeKey`

#### Scenario: Diferenciar sync inicial vs cambios de ruta
- **WHEN** el componente monta con `syncWithRouter` activo
- **THEN** el componente SHALL aplicar la tab del URL sin disparar cambios innecesarios

### Requirement: Lazy rendering con cache
El componente SHALL renderizar contenido solo al activar una tab cuando `lazy` este habilitado y SHALL cachear contenido activado.

#### Scenario: Lazy render inicial
- **WHEN** `lazy` es `true` y una tab no esta activa
- **THEN** su contenido SHALL no renderizarse hasta que se active

#### Scenario: Cache de contenido
- **WHEN** una tab fue activada previamente
- **THEN** su contenido SHALL mantenerse sin remount al volver a activarla

### Requirement: Telemetria de visibilidad
El componente SHALL ejecutar `onTabVisible(key)` cuando una tab se vuelve visible.

#### Scenario: Evento de visibilidad
- **WHEN** el usuario activa una tab
- **THEN** el componente SHALL llamar `onTabVisible` con el `key` visible

### Requirement: Documentacion profesional
El componente SHALL documentar `syncWithRouter`, `lazy` y `onTabVisible` con ejemplos en README.

#### Scenario: README actualizado
- **WHEN** el README es consultado
- **THEN** SHALL incluir descripcion, props y ejemplos de las funciones avanzadas
