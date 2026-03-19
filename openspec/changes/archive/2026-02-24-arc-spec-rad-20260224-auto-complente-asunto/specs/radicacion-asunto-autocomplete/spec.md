## ADDED Requirements

### Requirement: Autocompletado de ASUNTO desde plantilla
El sistema SHALL habilitar autocompletado para el campo `ASUNTO` en radicacion cuando exista el registro correspondiente en `camposPlantilla` (`name_campo = "ASUNTO"`).

#### Scenario: Resolucion de campo ASUNTO en metadata
- **WHEN** `camposPlantilla` incluye un item con `name_campo = "ASUNTO"`
- **THEN** el formulario usa ese item para configurar label, atributos y comportamiento del campo `data-ident="pl-radicacion-spe-ASUNTO"`

### Requirement: Consulta de sugerencias para ASUNTO
El sistema SHALL consultar la API `/api/PlantillaRadicado/solicitaAutoCompleteCampos` para obtener sugerencias del campo `ASUNTO` en funcion del texto digitado por el usuario.

#### Scenario: Consulta exitosa de autocompletado
- **WHEN** el usuario escribe en el campo `ASUNTO` y la API responde con resultados
- **THEN** el sistema muestra una lista de sugerencias seleccionables asociadas al texto ingresado

### Requirement: Degradacion segura ante error de autocompletado
El sistema SHALL mantener el ingreso manual de `ASUNTO` cuando la API de autocompletado falle o no retorne datos.

#### Scenario: Falla de API de autocompletado
- **WHEN** la API `/api/PlantillaRadicado/solicitaAutoCompleteCampos` responde error
- **THEN** el sistema mantiene el campo editable, informa error amigable y no bloquea el formulario
