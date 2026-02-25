## Why

Se requiere soportar de forma consistente los campos de tipo seleccion en la plantilla de radicacion, reutilizando la estructura y estilos ya definidos para los campos dinamicos. Esto asegura una experiencia uniforme y habilita configuraciones declarativas desde `camposPlantilla`.

## What Changes

- Renderizar campos con `ComportamientoCampo = "SELECCION"` y `campo_tip = 1` dentro del `Card` de radicacion.
- Aplicar atributos y reglas de validacion (required, disabled, maxLength, type/pattern, data-api-method, data-group) igual que en los campos existentes.
- Mantener la estructura y estilos de los campos tipo `AUTOCOMPLETE` para consistencia visual y accesibilidad.

## Capabilities

### New Capabilities

- `campos-plantilla-select`: Renderizado dinamico de campos `SELECCION` en plantilla de radicacion con reglas de validacion y atributos declarativos.

### Modified Capabilities

- `campos-dinamicos-plantilla`: Se amplian los requisitos para soportar `SELECCION` con la misma estructura/estilos y reglas de atributos.

## Impact

- UI de radicacion (componentes de renderizado de campos de plantilla).
- Estilos compartidos de campos dinamicos.
- Tests de UI para la plantilla de radicacion.
