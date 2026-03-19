## Why

El campo "Fecha Límite Respuesta" hoy no aprovecha los metadatos de plantilla (`title_control` y `tooltipAyuda`) como ya ocurre en otros campos de clasificación del trámite. Esto genera una experiencia inconsistente y reduce la ayuda contextual para el usuario al diligenciar la radicación.

## What Changes

- Usar metadatos de `camposPlantilla` para el campo `FECHALIMITERESPUESTA` en el formulario de radicación.
- Renderizar el label con `title_control` y mostrar icono de ayuda cuando exista `tooltipAyuda`.
- Mantener el comportamiento actual del control de fecha y sus reglas existentes (`required`/`disabled` cuando apliquen por plantilla).
- Alinear accesibilidad declarativa (`aria-describedby`) con el patrón ya usado en campos `Descripcion_Documento` y `RE_flujo_trabajo`.

## Capabilities

### New Capabilities
- Ninguna.

### Modified Capabilities
- `campos-dinamicos-plantilla`: se amplían los requisitos para incluir el render de metadatos (title/tooltip) en `FECHALIMITERESPUESTA`.

## Impact

- Afecta el módulo de radicación, principalmente el formulario y sus tests de comportamiento.
- No cambia contratos de API ni dependencias externas.
- Requiere actualización de spec delta en `openspec/changes/tooltip-re-fecha-limite-respuesta/specs/` y evidencia de pruebas asociadas.
