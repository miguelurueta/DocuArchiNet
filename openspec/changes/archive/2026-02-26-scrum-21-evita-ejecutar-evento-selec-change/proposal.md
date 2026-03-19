## Why

EVITA-EJECUTAR-EVENTO-SELEC-CHANGE. Evitar consumo de API en primer render por eventos change (RadicacionForm.tsx)

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUM-21.
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
