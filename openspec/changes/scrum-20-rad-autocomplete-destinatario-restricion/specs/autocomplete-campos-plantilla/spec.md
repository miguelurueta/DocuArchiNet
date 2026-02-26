## ADDED Requirements

### Requirement: Autocompletado token de Destinatario con restriccion
El sistema SHALL renderizar `Destinatario_Cor` como control tipo token en modo multiple usando la estructura de `ant-select` existente y mantener los atributos declarativos `required`, `disabled`, `title` y `tooltipAyuda` provenientes de `camposPlantilla`.

#### Scenario: Render del control token con metadata
- **WHEN** el formulario resuelve el campo `name_campo = "Destinatario_Cor"` en `camposPlantilla`
- **THEN** el control se renderiza como token multiple con `data-ident` de destinatario y conserva `required`, `disabled`, `title` y `tooltipAyuda`

### Requirement: Consulta de autocomplete con contrato restringido
El sistema SHALL consultar `POST /api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion` con el payload `{ idScript, nombreCampo, valueCampo }`, donde `idScript` SHALL obtenerse de `camposPlantilla.id_escript` del campo actual y `nombreCampo` SHALL corresponder al nombre del campo destinatario.

#### Scenario: Payload correcto para consulta de destinatario
- **WHEN** el usuario escribe texto en el input del token `Destinatario_Cor`
- **THEN** el request envía `{ idScript: <id_escript del campo>, nombreCampo: "Destinatario_Cor", valueCampo: <texto digitado> }`

### Requirement: Gestion de items y seleccion de tokens
El sistema SHALL poblar la lista de opciones con la respuesta de API, permitir la seleccion de tokens y limpiar las opciones de sugerencia cuando el valor de busqueda sea vacio sin perder estabilidad del control.

#### Scenario: Carga de opciones y seleccion token
- **WHEN** la API retorna resultados validos para `Destinatario_Cor`
- **THEN** el control muestra opciones seleccionables y al seleccionar agrega el token correspondiente

#### Scenario: Limpieza de items al vaciar busqueda
- **WHEN** el valor del input de busqueda queda vacio
- **THEN** las opciones de sugerencia se limpian y el control no lanza errores de renderizado

### Requirement: Manejo centralizado de errores de autocompletado
El sistema SHALL manejar errores de la API mediante la capa centralizada de Axios/hooks y mantener el flujo del formulario sin colapsar el control de tokens.

#### Scenario: Error de API controlado
- **WHEN** la consulta de autocompletado falla por error HTTP o red
- **THEN** el error se procesa por la capa centralizada y el control permanece operativo sin romper el formulario
