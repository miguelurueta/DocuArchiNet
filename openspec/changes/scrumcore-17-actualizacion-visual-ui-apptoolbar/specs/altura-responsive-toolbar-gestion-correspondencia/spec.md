## ADDED Requirements

### Requirement: El AppToolbar de GestionCorrespondencia SHALL ajustar su altura automaticamente al contenido
El sistema SHALL permitir que el `AppToolbar` consumido por `GestionCorrespondencia` tenga altura automatica y crecimiento vertical dinamico cuando las acciones visibles ocupen una o mas lineas.

#### Scenario: Toolbar compacta en una sola linea
- **WHEN** el toolbar dispone de ancho suficiente para mantener sus acciones en una sola linea
- **THEN** el contenedor SHALL mantener una altura compacta automatica sin espacios sobrantes innecesarios

#### Scenario: Toolbar crece cuando las acciones hacen wrap
- **WHEN** las acciones del toolbar pasan a dos o mas lineas por falta de ancho horizontal
- **THEN** el contenedor SHALL aumentar su altura automaticamente para contener todo el contenido visible

### Requirement: El contenedor del toolbar SHALL permitir wrap visible sin clipping
El sistema SHALL configurar el contenedor visual del toolbar para soportar `flex-wrap`, `overflow: visible` y alineacion compatible con contenido multilinea, evitando cortes visuales o scroll innecesario.

#### Scenario: Tablet permite wrap sin corte
- **WHEN** la vista se renderiza en un ancho de tablet donde las acciones hacen wrap parcial
- **THEN** el toolbar SHALL mantener visibles todos los controles sin clipping ni contenido oculto

#### Scenario: Mobile permite apilamiento sin limite fijo
- **WHEN** la vista se renderiza en mobile y las acciones se apilan o distribuyen en varias lineas
- **THEN** el toolbar SHALL crecer sin depender de una altura fija

#### Scenario: Breakpoint de 1100px no infla la altura artificialmente
- **WHEN** el `AppToolbar` entra en el breakpoint donde cambia a layout de columna alrededor de `1100px`
- **THEN** las regiones internas del componente SHALL dejar de reservar altura artificial y el contenedor SHALL ajustarse al contenido real

### Requirement: El ajuste SHALL preservar la composicion existente de acciones
El sistema SHALL mantener intactos `AppDropdown`, `AppButton`, la estructura JSX actual y el flujo de navegacion del modulo, limitando este cambio al comportamiento visual del contenedor del toolbar.

#### Scenario: Exportar y Abrir respuesta contextual permanecen sin cambios funcionales
- **WHEN** el usuario interactua con la toolbar despues del ajuste visual
- **THEN** las acciones `Exportar` y `Abrir respuesta contextual` SHALL seguir disponibles y funcionando como antes del cambio

### Requirement: El contenedor padre del modulo SHALL no bloquear el crecimiento vertical del toolbar
El sistema SHALL configurar el wrapper principal de `GestionCorrespondencia` de forma que no impida la expansion vertical del toolbar al adaptarse al contenido.

#### Scenario: El layout del modulo permite expansion del toolbar
- **WHEN** el toolbar necesita crecer verticalmente por wrap de acciones
- **THEN** el contenedor padre del modulo SHALL permitir esa expansion sin forzar recorte ni colapso de la caja visual
