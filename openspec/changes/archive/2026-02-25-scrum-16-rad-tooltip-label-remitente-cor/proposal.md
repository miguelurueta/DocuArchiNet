## Why

RAD-TOOLTIP-LABEL-REMITENTE-COR. Tengo una constante llamada camposPlantilla que contiene objetos con información de campos.En el componente RadicacionForm.tsx existe un campo con atributo data-ident="pl-radicacion-spe-REMITENTE_COR".Necesito que se ubique en la estructura camposPlantilla el registro correspondiente comparando "name_campo": "REMITENTE_COR".Con esa coincidencia se debe implementar,7. Conservar atributos existentes como required, disabled, title y tooltipAyuda.8. Usar el valor del campo title_control para agregar un tooltip (atributo title).9. Usar el valor del campo tooltipAyuda para agregar un span con clase "tooltip-ayuda" junto al label (incluyendo el icono si aplica).

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUM-16.
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
