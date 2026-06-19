## ADDED Requirements

### Requirement: Toolbar Agrupado Por Contexto
El sistema SHALL organizar el toolbar del Preview PDF en grupos funcionales.

#### Scenario: Grupos visibles semanticamente
- **WHEN** el usuario visualiza el Preview PDF
- **THEN** el toolbar contiene los grupos `Edicion`, `Visualizacion`, `Organizacion` y `Navegacion`
- **AND** los grupos aparecen en ese orden

### Requirement: Acciones Por Grupo
El sistema SHALL mantener las acciones existentes dentro del grupo correcto.

#### Scenario: Organizacion
- **WHEN** existen paginas
- **THEN** el grupo `Organizacion` contiene `Organizar paginas`

#### Scenario: Navegacion
- **WHEN** existen paginas
- **THEN** el grupo `Navegacion` contiene el campo `Pagina` y el boton `Buscar pagina`

#### Scenario: Edicion
- **WHEN** existe pagina seleccionada
- **THEN** el grupo `Edicion` contiene `Rotar izquierda`, `Rotar derecha`, `Seleccionar area`, `Eliminar pagina` y `Limpiar documento`

#### Scenario: Visualizacion
- **WHEN** existe pagina seleccionada
- **THEN** el grupo `Visualizacion` contiene `Reducir zoom`, `Aumentar zoom`, `Ajustar ancho`, `Ajustar pagina` y `Pantalla completa`

### Requirement: Botones Icono Y Tooltip
El sistema SHALL evitar texto permanente dentro de botones del toolbar.

#### Scenario: Botones sin texto permanente
- **WHEN** se renderiza el toolbar
- **THEN** los comandos se presentan como `AppButton` con icono y tooltip
- **AND** el texto descriptivo queda en `aria-label`/tooltip

### Requirement: Estados Existentes
El sistema SHALL mantener las reglas de habilitado/deshabilitado existentes.

#### Scenario: Sin pagina seleccionada
- **WHEN** no existe pagina seleccionada
- **THEN** rotar, seleccionar area, eliminar, zoom y fit se mantienen deshabilitados

#### Scenario: No regresion funcional
- **WHEN** el usuario usa organizar paginas, buscar pagina, rotar, seleccionar area, eliminar, limpiar, zoom, fit o pantalla completa
- **THEN** el comportamiento funcional se mantiene respecto a la implementacion previa
