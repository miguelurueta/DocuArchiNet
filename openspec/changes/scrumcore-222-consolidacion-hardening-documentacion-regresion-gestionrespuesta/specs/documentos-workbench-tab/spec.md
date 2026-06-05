## MODIFIED Requirements

### Requirement: Workbench estable bajo recarga de estado transversal
`DocumentosWorkbench` SHALL mantener layout y estado de interacción cuando cambian estados de gabinete (`loading/error/reload`) sin romper árbol ni panel PDF.

#### Scenario: Layout estable durante recarga de gabinete
- **WHEN** `gabineteLoading` cambia a `true` y posteriormente a `false` en una sesión activa
- **THEN** el layout del workbench (rail y visor) no colapsa ni pierde posición visual esperada
- **AND** la sesión de selección de documento visible no se reinicia por recargas de gabinete.

### Requirement: Toggle y estado responsive consistentes
- **WHEN** el usuario interactúa con el toggle de rail durante transiciones de contexto
- **THEN** no se pierde focus y el estado visual por breakpoint se mantiene.

### Requirement: Estado de error no bloquea flujo general
- **WHEN** `gabineteError` está presente
- **THEN** el árbol permanece operativo para acciones no dependientes de gabinete
- **AND** las acciones dependientes muestran error funcional sin romper interacción general.
