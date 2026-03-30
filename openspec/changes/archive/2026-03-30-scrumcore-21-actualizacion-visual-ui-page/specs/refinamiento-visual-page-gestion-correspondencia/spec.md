# refinamiento-visual-page-gestion-correspondencia Specification

## Purpose

Definir el refinamiento visual del contenedor `.page` en `GestionCorrespondencia`.

## Requirements

### Requirement: La pagina debe exponer un wrapper visual enterprise

La clase `.page` MUST aplicar un contenedor visual consistente para la pantalla principal de `GestionCorrespondencia`.

#### Scenario: Estilos base aplicados

- **GIVEN** la clase `.page` en `GestionCorrespondencia.module.css`
- **WHEN** se implementa `SCRUMCORE-21`
- **THEN** `.page` usa `gap: 2px`
- **AND** `.page` usa `border-radius: 24px`
- **AND** `.page` usa `background-color: #f5f5f5`

### Requirement: El ajuste visual no debe romper el layout actual

El refinamiento de `.page` MUST conservar la estructura actual entre `AppToolbar` y `AppContent`.

#### Scenario: Integración estructural preservada

- **GIVEN** la ruta principal de `GestionCorrespondencia`
- **WHEN** la página se renderiza tras el ajuste
- **THEN** `AppToolbar` sigue renderizando arriba
- **AND** `AppContent` sigue renderizando debajo
- **AND** la estructura del layout no se rompe

### Requirement: El gap debe ser consistente en todos los breakpoints

La separación definida en `.page` MUST mantenerse consistente en desktop, tablet y mobile.

#### Scenario: Responsive consistente

- **GIVEN** la ruta principal de `GestionCorrespondencia`
- **WHEN** el viewport cambia entre desktop, tablet y mobile
- **THEN** `.page` mantiene `gap: 2px`
- **AND** el resto de reglas críticas del layout siguen funcionando

### Requirement: Las acciones de la toolbar deben alinearse al nuevo tratamiento visual

La página MUST renderizar sus acciones principales con `variant="ghost"` y la variante `ghost` SHOULD usar texto negro como color base.

#### Scenario: Botones de GestionCorrespondencia actualizados

- **GIVEN** la página principal de `GestionCorrespondencia`
- **WHEN** se renderiza tras `SCRUMCORE-21`
- **THEN** el trigger `Exportar` usa `variant="ghost"`
- **AND** el botón `Abrir respuesta contextual` usa `variant="ghost"`
- **AND** `.variantGhost` en `AppButton.module.css` usa `color: black`
