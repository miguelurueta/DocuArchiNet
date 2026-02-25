## Why

El formulario de radicación necesita autocompletar un campo específico (ANEXOS_COR) para reducir errores de digitación y acelerar el diligenciamiento. Actualmente ese campo se muestra como input simple y no consume la API de autocompletado.

## What Changes

- Se agrega un autocompletado para el campo `ANEXOS_COR` detectado en `camposPlantilla`.
- La consulta usa `/api/PlantillaRadicado/solicitaAutoCompleteCampos` con `tbl_control` tomado de `tbl_control` del campo y `name_campo` del campo actual.
- Se conserva el render existente (required, disabled, title, tooltip) y se estandariza para reutilizar el componente en campos similares.

## Capabilities

### New Capabilities
- 

### Modified Capabilities
- `autocomplete-campos-plantilla`: Extiende el autocompletado para soportar el campo `ANEXOS_COR` con parámetros dinámicos y render reutilizable.

## Impact

- UI del formulario `RadicacionForm` y componentes de campos dinámicos.
- Servicios de API (axios) para autocompletado.
- Tests de comportamiento de autocompletado.
