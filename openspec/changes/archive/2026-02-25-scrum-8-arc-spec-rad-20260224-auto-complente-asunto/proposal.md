## Why

ARC-SPEC-RAD-20260224:auto-complente-asunto. Tengo una constante llamada camposPlantilla que contiene objetos con información de campos.En el componente RadicacionForm.tsx existe un campo con atributo data-ident="pl-radicacion-spe-ASUNTO".Necesito que se ubique en la estructura camposPlantilla el registro correspondiente comparando "name_campo": "ASUNTO".Con esa coincidencia se debe implementar un autocompletado que consulte la API /api/PlantillaRadicado/solicitaAutoCompleteCampos.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUM-8.
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
