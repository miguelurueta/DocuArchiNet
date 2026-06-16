# tabs-workbench-gestion-correspondencia Specification

## ADDED Requirements

### Requirement: Modo normal por defecto
El sistema SHALL conservar el comportamiento actual de tabs en Gestion Correspondencia como modo inicial.

#### Scenario: Render inicial en tabs
- **GIVEN** el usuario abre `GestionRespuesta`
- **WHEN** no ha activado la vista paralela
- **THEN** el sistema muestra los tabs normales `Gestion` y `Documentos`
- **AND** el contenido se renderiza mediante `AppTabs`
- **AND** no se modifica la logica de negocio de `GestionRespuestaMainTabContent` ni `DocumentosWorkbench`

### Requirement: Toggle opt-in de vista paralela
El sistema SHALL exponer un control visible para alternar entre vista normal y vista paralela.

#### Scenario: Activar vista paralela
- **GIVEN** el usuario esta en modo normal
- **WHEN** hace click en `Vista paralela`
- **THEN** el modo cambia a `parallel`
- **AND** el boton expone `aria-pressed="true"`
- **AND** el texto del control cambia a `Vista normal`

#### Scenario: Desactivar vista paralela
- **GIVEN** el usuario esta en modo paralelo
- **WHEN** hace click en `Vista normal`
- **THEN** el modo vuelve a `tabs`
- **AND** el boton expone `aria-pressed="false"`
- **AND** el texto del control cambia a `Vista paralela`

### Requirement: Render simultaneo de Gestion y Documentos
El sistema SHALL mostrar `Gestion` y `Documentos` simultaneamente cuando la vista paralela esta activa.

#### Scenario: Paneles simultaneos
- **GIVEN** el modo paralelo esta activo
- **WHEN** se renderiza el Workbench
- **THEN** el panel `Gestion` contiene `GestionRespuestaMainTabContent`
- **AND** el panel `Documentos` contiene `DocumentosWorkbench`
- **AND** ambos paneles son visibles al mismo tiempo
- **AND** no se crean multiples instancias de `DocumentosWorkbench`

### Requirement: Layout redimensionable con react-resizable-panels
El sistema SHALL usar `react-resizable-panels` para el layout paralelo.

#### Scenario: PanelGroup horizontal
- **GIVEN** el modo paralelo esta activo en desktop o tablet ancho
- **WHEN** se renderiza el layout paralelo
- **THEN** se usa `PanelGroup` con direccion horizontal
- **AND** se renderizan dos `Panel`
- **AND** se renderiza un `PanelResizeHandle` visible y operable
- **AND** cada panel tiene tamanos iniciales y minimos definidos

### Requirement: Provider compartido y no duplicacion de negocio
El sistema SHALL mantener un unico `GestionRespuestaDocumentosProvider` alrededor del modo normal y paralelo.

#### Scenario: Provider comun
- **GIVEN** `GestionRespuesta` renderiza el Workbench
- **WHEN** el usuario alterna entre modo normal y paralelo
- **THEN** `GestionRespuestaDocumentosProvider` sigue envolviendo ambos modos
- **AND** no se crean providers independientes por panel
- **AND** no se duplican services ni hooks por decisiones de layout

### Requirement: Responsive conservador
El sistema SHALL evitar una vista paralela horizontal cuando el ancho disponible degrade la experiencia.

#### Scenario: Ancho reducido
- **GIVEN** el viewport tiene ancho reducido
- **WHEN** el usuario usa el Workbench
- **THEN** el sistema mantiene tabs normales o degrada la vista paralela a una experiencia usable
- **AND** no fuerza dos columnas horizontales inutilizables
- **AND** la decision responsive queda documentada

### Requirement: Accesibilidad del modo paralelo
El sistema SHALL mantener controles y paneles accesibles.

#### Scenario: Control accesible
- **GIVEN** el boton de vista paralela esta disponible
- **WHEN** se inspecciona el control
- **THEN** es operable por teclado
- **AND** tiene texto visible o nombre accesible claro
- **AND** usa `aria-pressed` para comunicar el estado

#### Scenario: Paneles con nombre accesible
- **GIVEN** el modo paralelo esta activo
- **WHEN** se inspeccionan los paneles
- **THEN** el panel de gestion tiene nombre accesible `Gestion`
- **AND** el panel de documentos tiene nombre accesible `Documentos`
- **AND** el divisor tiene foco visible o comportamiento accesible provisto por la libreria

### Requirement: Documentacion enterprise
El sistema SHALL documentar la implementacion de SCRUMCORE-251 en la ruta enterprise definida.

#### Scenario: Archivos documentales
- **WHEN** se cierre la implementacion
- **THEN** existen documentos en `docs/Architecture/GestionCorrrespondecia/Tabs-React-Resizable-Panels/`
- **AND** incluyen arquitectura, implementacion detallada, pruebas y metadata
- **AND** registran riesgos residuales, pruebas ejecutadas, commits y PR
