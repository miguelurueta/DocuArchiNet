## Why

RAD-CREA-CONSTANTE-RESTRICION-DESTINATARIO. En el componente RadicacionForm.tsx necesito crear una constante llamada CDeRelacionEstadoRetriccionDto con la siguiente estructura:

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUM-18.
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
