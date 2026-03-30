# boton-actualizar-toolbar-gestion-correspondencia Specification

## Purpose

Definir la incorporación del botón `Actualizar` en el `AppToolbar` de `GestionCorrespondencia`.

## Requirements

### Requirement: La toolbar debe exponer un botón Actualizar

La vista `GestionCorrespondencia` MUST renderizar un nuevo `AppButton` llamado `Actualizar` dentro del grupo de acciones del toolbar.

#### Scenario: Render del botón

- **GIVEN** la página principal de `GestionCorrespondencia`
- **WHEN** el toolbar se renderiza
- **THEN** aparece un botón con el texto `Actualizar`
- **AND** usa `variant="ghost"`
- **AND** usa `size="sm"`
- **AND** usa `UndoOutlined` como icono izquierdo

### Requirement: El nuevo botón no debe romper la composición responsive

La incorporación del botón MUST respetar el layout actual del toolbar en desktop, tablet y mobile.

#### Scenario: Integración visual

- **GIVEN** el grupo de acciones del `AppToolbar`
- **WHEN** se agrega el botón `Actualizar`
- **THEN** el grupo conserva su wrap actual
- **AND** no introduce overflow horizontal
- **AND** mantiene coherencia visual con los demás botones

### Requirement: La página debe usar fondo blanco

La clase `.page` en `GestionCorrespondencia.module.css` MUST usar `background-color: white`.

#### Scenario: Estilo base de la página

- **GIVEN** la clase `.page`
- **WHEN** se aplica `SCRUMCORE-22`
- **THEN** el contenedor principal usa fondo blanco
