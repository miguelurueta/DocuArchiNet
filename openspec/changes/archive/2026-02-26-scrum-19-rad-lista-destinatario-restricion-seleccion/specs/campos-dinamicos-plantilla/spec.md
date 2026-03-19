## ADDED Requirements

### Requirement: Consulta de estructura de restriccion al seleccionar tramite
El sistema SHALL capturar el `idValue` seleccionado en el campo `Descripcion_Documento` (`data-ident="pl-radicacion-spe-Descripcion_Documento"`) y consultar `GET /api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion` enviando el identificador seleccionado como parametro.

#### Scenario: Seleccion de tramite con idValue valido
- **WHEN** el usuario selecciona una opcion de tramite con `idValue` valido
- **THEN** el sistema invoca `GET /api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion` con el `idValue` seleccionado

### Requirement: Manejo de estado cuando no hay idValue
El sistema SHALL evitar consultas al endpoint de restriccion cuando el tramite seleccionado no tenga `idValue` valido, y mantener estado seguro en el formulario.

#### Scenario: Seleccion sin idValue
- **WHEN** el usuario selecciona un tramite sin `idValue` o limpia la seleccion
- **THEN** el sistema no consulta el endpoint de restriccion y limpia estado derivado asociado a restriccion

### Requirement: Integracion sin regresion del flujo de formulario
El sistema SHALL mantener el comportamiento existente del formulario de radicacion mientras integra la consulta de estructura de restriccion desde `Descripcion_Documento`.

#### Scenario: Convivencia con flujos existentes
- **WHEN** se dispara la consulta de estructura de restriccion por seleccion de tramite
- **THEN** los demas controles dependientes del tramite continúan operando sin regresion funcional

### Requirement: Despliegue y seleccion de lista para Destinatario_Cor
El sistema SHALL desplegar la lista de resultados de autocompletado para `Destinatario_Cor` y permitir seleccionar un item cuando el usuario digita texto y la API retorna coincidencias.

#### Scenario: Usuario digita y selecciona destinatario desde lista
- **WHEN** el usuario digita en `Destinatario_Cor` y la API `/api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion` retorna opciones
- **THEN** el sistema despliega la lista de opciones sin filtrar localmente resultados del backend
- **AND** al seleccionar una opcion, el item queda seleccionado en el control tokenizado
