## ADDED Requirements

### Requirement: Seleccion Visual De Area
El sistema SHALL permitir activar un modo de seleccion de area desde el toolbar del Preview PDF.

#### Scenario: Activar seleccion
- **WHEN** existen paginas escaneadas
- **AND** el usuario pulsa `Seleccionar area`
- **THEN** el boton cambia a estado activo
- **AND** el cursor del preview permite seleccionar un area rectangular

#### Scenario: Dibujar rectangulo
- **WHEN** el modo de seleccion esta activo
- **AND** el usuario hace pointer down, drag y pointer up sobre la pagina visible
- **THEN** el sistema muestra un rectangulo visual sobre esa pagina
- **AND** la seleccion se guarda como `x`, `y`, `width`, `height` en coordenadas reales de pagina
- **AND** la seleccion no depende del zoom actual

### Requirement: Acciones De Seleccion
El sistema SHALL mostrar acciones contextuales cuando exista una seleccion valida.

#### Scenario: Acciones disponibles
- **WHEN** existe una seleccion valida en la pagina activa
- **THEN** el sistema muestra acciones para recortar, reiniciar seleccion y cancelar

#### Scenario: Reiniciar o cancelar
- **WHEN** el usuario reinicia o cancela la seleccion
- **THEN** el rectangulo se elimina
- **AND** no se modifica `scanner.pages`

### Requirement: Recorte Manual Por Pagina
El sistema SHALL aplicar recorte manual unicamente sobre la pagina seleccionada.

#### Scenario: Aplicar recorte
- **WHEN** existe una seleccion valida
- **AND** el usuario pulsa `Recortar`
- **THEN** el sistema llama `cropPage(pageId, selection)`
- **AND** el cliente Dynamsoft usa `DWT.Crop` sobre el indice de esa pagina
- **AND** se refresca solo la pagina afectada
- **AND** el PDF pendiente se invalida para regeneracion posterior

#### Scenario: Runtime sin Crop
- **WHEN** el runtime Dynamsoft no expone `Crop`
- **THEN** el sistema muestra error controlado
- **AND** no modifica el lote actual

### Requirement: No Regresion Del Preview
El sistema SHALL mantener preview, miniaturas, organizador, scanner, zoom y configuracion montados.

#### Scenario: Convivencia con zoom y organizador
- **WHEN** el usuario usa zoom, fit width, fit page o pantalla completa
- **THEN** la seleccion visual sigue alineada con la imagen visible
- **AND** las coordenadas siguen expresadas contra dimensiones reales
- **AND** abrir el organizador limpia el modo de seleccion sin desmontar el preview
