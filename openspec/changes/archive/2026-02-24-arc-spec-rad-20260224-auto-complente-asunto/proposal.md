## Why

El campo `ASUNTO` del formulario de radicacion necesita autocompletado dinamico basado en la metadata de `camposPlantilla`. Hoy el campo existe en UI (`data-ident="pl-radicacion-spe-ASUNTO"`), pero no consulta la API declarada para sugerencias y obliga digitacion manual.

## What Changes

- Se identifica en `camposPlantilla` el registro con `name_campo = "ASUNTO"` para configurar el autocompletado.
- Se implementa autocompletado para `ASUNTO` en `RadicacionForm.tsx` usando `/api/PlantillaRadicado/solicitaAutoCompleteCampos`.
- Se manejan estados de carga/error y seleccion de sugerencias sin romper el flujo actual de radicacion.

## Capabilities

### New Capabilities
- `radicacion-asunto-autocomplete`: Sugerencias dinamicas para el campo ASUNTO en radicacion.

### Modified Capabilities
- `campos-dinamicos-plantilla`: Extiende el renderer de plantilla para habilitar autocompletado en ASUNTO.

## Impact

- Cambios en renderer/formulario de radicacion y hook asociado de campos dinamicos.
- Nuevas pruebas de comportamiento para escenario de autocompletado de ASUNTO.
