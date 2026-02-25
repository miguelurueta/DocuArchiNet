## Why

RAD-ELIMINA-CAMPO-ESTATICO-MEDIO-RECEPCION. En el componente RadicacionForm.tsx se renderizan dinámicamente y también algunos campos fijos.Necesito que se elimine el campo fijo cuyo atributo data-ident="pl-radicacion-spe-Medio-recep".

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUM-10.
- Se incluye el resumen y descripcion del ticket como contexto inicial.
- Se guarda la propuesta en el arbol de cambios de OpenSpec.

## Capabilities

### New Capabilities
- N/A

### Modified Capabilities
- `campos-dinamicos-plantilla`: eliminar render estatico duplicado de "Medio de Recepcion" y consolidar el dato en metadata dinamica de plantilla.

## Impact

- Nuevo script de generacion en `scripts/`.
- Nuevo archivo `openspec/changes/<issueKey>/proposal.md`.
