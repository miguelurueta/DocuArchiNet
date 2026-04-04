## ADDED Requirements

### Requirement: `AppTable` soporta layout reusable `content | fill`

`AppTable` MUST exponer un modo estándar de layout vertical reutilizable.

#### Scenario: `content` preserva el comportamiento actual

- **WHEN** una pantalla renderiza `AppTable` sin `layoutMode`
- **THEN** el componente conserva el layout actual basado en contenido
- **AND** no rompe implementaciones existentes

#### Scenario: `fill` usa alto estable

- **WHEN** una pantalla renderiza `AppTable` con `layoutMode="fill"`
- **THEN** el componente usa `domLayout="normal"`
- **AND** ocupa el alto restante del contenedor
- **AND** usa scroll vertical interno

### Requirement: El contenedor debe ser compatible con layout restante

Las pantallas que consuman `layoutMode="fill"` MUST proporcionar un contenedor con layout compatible para ceder el alto restante a la tabla.

#### Scenario: pantalla con toolbar y paginación

- **WHEN** una pantalla contiene toolbar, paginación y tabla principal
- **THEN** el layout distribuye los bloques superiores con su alto natural
- **AND** la tabla ocupa el resto del espacio disponible
- **AND** cambiar `pageSize` no altera la altura visible total de la tabla
