## Why

RAD-LLENAR-LISTA-DESTINATARIO-RESTRINGIDA. RadicacionForm.tsx renderiza controles (fijos y dinámicos) basados en la constante camposPlantilla y otros estados internos.Existe un campo con data-ident="pl-radicacion-spe-Descripcion_Documento" que ya tiene un evento de selección/change que consume una API.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUM-22.
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
