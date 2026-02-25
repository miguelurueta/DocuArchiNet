## Why

El campo `RE_flujo_trabajo` en `RadicacionForm` debe respetar los metadatos de plantilla (title y tooltip) para mantener consistencia con otros campos dinámicos y mejorar la ayuda contextual al usuario.

## What Changes

- Se busca el registro de `camposPlantilla` con `name_campo = "RE_flujo_trabajo"` para derivar atributos de UI.
- Se conserva `required` y `disabled` existentes y se añade `title` desde `title_control` y tooltip desde `tooltipAyuda` junto al label.

## Capabilities

### New Capabilities
- 

### Modified Capabilities
- `campos-dinamicos-plantilla`: Ajusta el render de un campo dinámico específico para aplicar metadatos de plantilla (title/tooltip) en `RE_flujo_trabajo`.

## Impact

- UI del formulario `RadicacionForm`.
- Tests de comportamiento del render de campos dinámicos.
