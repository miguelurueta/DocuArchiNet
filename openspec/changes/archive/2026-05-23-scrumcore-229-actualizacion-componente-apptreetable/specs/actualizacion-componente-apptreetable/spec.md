## ADDED Requirements

### Requirement: Ajuste visual CSS-only (Workbench)
El sistema SHALL mejorar el aspecto visual del listado renderizado por `AppTreeTable` en el Workbench usando **solo CSS** (o CSS Modules), manteniendo comportamiento funcional.

#### Scenario: Scoping estricto (no-regresión cross-screen)
- **GIVEN** el listado se renderiza dentro de Workbench (`data-testid="documentos-workbench"`)
- **WHEN** se aplican estilos del ticket
- **THEN** los estilos solo afectan elementos dentro de ese contenedor (sin impactar otras pantallas que usan `AppTreeTable/AppTable`)

#### Scenario: 2 columnas funcionales visibles (sin cambiar sizing)
- **GIVEN** backend/Dynamic UI provee 2 columnas funcionales (Documento + Acciones)
- **WHEN** se renderiza el listado en Workbench
- **THEN** se mantienen las 2 columnas visibles
- **AND** no se modifica el sizing de columnas desde código (no tocar `minWidth`/`flex`/`width`)

#### Scenario: Look & feel moderno/enterprise
- **WHEN** el usuario visualiza el listado
- **THEN** el header se ve liviano (sin bordes pesados)
- **AND** las filas tienen hover sutil
- **AND** la fila seleccionada se distingue claramente (respetando `aria-selected`)
- **AND** el estilo es consistente con un UI enterprise limpio

#### Scenario: Accesibilidad visual (focus visible)
- **WHEN** el usuario navega con teclado por celdas navegables
- **THEN** existe foco visible (outline/ring) sin “focus trap”

#### Scenario: Performance
- **WHEN** se interactúa con scroll/hover/selección
- **THEN** no se introducen selectores globales pesados ni animaciones que disparen reflow constante

### Requirement: Restricciones MUST
El sistema SHALL cumplir:
- NO modificar `src/app/Components/UI/AppTable/**`
- NO modificar lógica funcional de `AppTreeTable`
- NO cambiar contratos backend/DTOs
- NO introducir dependencias pesadas

### Requirement: Pruebas Playwright
El sistema SHALL incluir pruebas Playwright de regresión para Workbench.

#### Scenario: Headers visibles
- **WHEN** Workbench carga con Documento + Acciones
- **THEN** existen 2 `columnheader` visibles correspondientes a ambas columnas

#### Scenario: Focus visible
- **WHEN** se navega con teclado
- **THEN** el foco visible se mantiene

#### Scenario: Selected/Hover
- **WHEN** se selecciona una fila y se hace hover
- **THEN** el estilo se aplica sin romper layout
