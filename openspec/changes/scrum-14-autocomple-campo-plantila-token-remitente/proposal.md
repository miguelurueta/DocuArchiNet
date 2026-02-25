## Why

AUTOCOMPLE-CAMPO-PLANTILA-TOKEN-REMITENTE. Tengo una constante llamada camposPlantilla que contiene objetos con información de campos. En el componente RadicacionForm.tsx existe un campo con atributo data-ident="pl-radicacion-spe-REMITENTE_COR". Necesito que se ubique en la estructura camposPlantilla el registro correspondiente comparando "name_campo": "REMITENTE_COR". Con esa coincidencia se debe implementar un autocompletado que consulte la API /api/PlantillaRadicado/autoCompleteTercero.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUM-14.
- Se incluye el resumen y descripcion del ticket como contexto inicial.
- Se guarda la propuesta en el arbol de cambios de OpenSpec.

## Capabilities

### New Capabilities
- `jira-proposal-generator`: Generacion automatica de propuestas OpenSpec desde Jira.

### Modified Capabilities
- 

## Impact

- Nuevo script de generacion en `scripts/`.
- Nuevo archivo `openspec/changes/<issueKey>/proposal.md`.
