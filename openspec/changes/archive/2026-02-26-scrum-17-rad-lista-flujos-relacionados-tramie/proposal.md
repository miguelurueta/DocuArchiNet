## Why

RAD-LISTA-FLUJOS-RELACIONADOS-TRAMIE. Tengo una constante llamada camposPlantilla que contiene objetos con información de campos.  En el componente RadicacionForm.tsx existe un campo con atributo data-ident="pl-radicacion-spe-Descripcion_Documento".  Necesito que captures el evento selecion chage y tomes el valor del campo idValue del campo, con este valor    consume  la API /api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite y enviale ese parametro.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUM-17.
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
