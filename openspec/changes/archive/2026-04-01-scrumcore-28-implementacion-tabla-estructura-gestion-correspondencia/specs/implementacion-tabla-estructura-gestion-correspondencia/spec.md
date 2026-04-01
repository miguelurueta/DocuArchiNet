## ADDED Requirements

### Requirement: Filtros en fila superior
El sistema MUST renderizar una fila de filtros con AppInput search y AppInput select de categoria dentro de AppContent.

#### Scenario: Render de filtros
- **WHEN** el usuario visualiza GestionCorrespondencia
- **THEN** se muestran los filtros search y categoria en la primera fila

### Requirement: Paginacion con selector
El sistema SHALL renderizar la fila de paginacion con el total de registros y un selector de pageSize.

#### Scenario: Render de paginacion
- **WHEN** se renderiza la vista de tabla
- **THEN** se muestra el total y el selector de paginacion

### Requirement: Tabla con AppTable
El sistema MUST renderizar AppTable con filas mock y columnas basicas en la tercera fila.

#### Scenario: Render de tabla
- **WHEN** se carga la vista
- **THEN** se muestra AppTable con datos de prueba

### Requirement: Responsive
El sistema SHALL adaptar filtros y paginacion para mobile sin overflow.

#### Scenario: Mobile
- **WHEN** la pantalla es menor a 768px
- **THEN** filtros se apilan y la paginacion se muestra en columna
