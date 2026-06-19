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
- **AND** el handle expone un grip interno de puntos verticales
- **AND** el handle incluye tooltip nativo para explicar que se puede arrastrar
- **AND** cada panel tiene tamanos iniciales y minimos definidos

#### Scenario: Feedback visual del handle
- **GIVEN** el modo paralelo esta activo
- **WHEN** el usuario pasa el mouse, enfoca o activa el separador
- **THEN** el contenedor del handle mantiene apariencia limpia y transparente
- **AND** el grip interno cambia a un estado azul sutil
- **AND** el sistema mantiene cursor de redimensionamiento
- **AND** no se bloquea el resize provisto por `react-resizable-panels`

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

### Requirement: Asistente IA flotante del workbench
El sistema SHALL exponer un boton flotante de IA persistente en el workbench de Gestion Correspondencia sin requerir extension de navegador.

#### Scenario: Boton visible en ambos tabs
- **GIVEN** el usuario visualiza `GestionRespuesta`
- **WHEN** esta en el tab `Gestion` o en el tab `Documentos`
- **THEN** el sistema muestra un boton flotante azul abajo a la derecha
- **AND** el boton muestra icono de robot y texto `IA` cuando el chat esta cerrado
- **AND** el boton no bloquea la navegacion del workbench ni los controles del tab activo

#### Scenario: Apertura y cierre del chat
- **GIVEN** el boton flotante de IA esta visible
- **WHEN** el usuario hace click en el boton
- **THEN** el sistema abre un panel tipo chat
- **AND** la apertura se anima desde el boton flotante
- **AND** el boton muestra una X visible mientras el chat esta abierto
- **WHEN** el usuario cierra el chat desde el boton flotante o desde el header
- **THEN** el panel se anima de regreso hacia el boton antes de desmontarse

#### Scenario: Escritura y envio de mensaje
- **GIVEN** el chat de IA esta abierto
- **WHEN** el usuario escribe en el input del chat
- **THEN** el input permite escribir caracteres consecutivos sin perder foco
- **AND** el AppEditor no intercepta las teclas del input
- **WHEN** el usuario presiona Enter
- **THEN** el sistema envia el mensaje
- **AND** limpia el input
- **AND** conserva el foco en el input

#### Scenario: Limpieza del input
- **GIVEN** el chat de IA esta abierto
- **AND** el input contiene texto
- **WHEN** el usuario hace click en la X interna del input
- **THEN** el sistema limpia el texto
- **AND** mantiene el foco en el input
- **AND** no cierra el chat

#### Scenario: Alcance local sin backend
- **GIVEN** el chat de IA esta abierto
- **WHEN** el usuario envia un mensaje
- **THEN** el sistema agrega el mensaje al log local
- **AND** agrega una respuesta placeholder local
- **AND** no invoca endpoints, servicios backend, modelos IA, streaming ni persistencia

### Requirement: No solapamiento con indicadores del AppEditor
El sistema SHALL evitar que el boton de IA interrumpa visualmente el indicador flotante de palabras/caracteres del AppEditor.

#### Scenario: Indicador desplazado
- **GIVEN** el AppEditor muestra el indicador de palabras/caracteres
- **AND** el boton flotante de IA esta visible
- **WHEN** ambos elementos comparten la zona inferior del workbench
- **THEN** el indicador de palabras/caracteres se ubica mas hacia la izquierda
- **AND** el boton IA permanece abajo a la derecha
- **AND** no se modifica la logica de conteo de palabras, conteo de caracteres ni paginacion visual
