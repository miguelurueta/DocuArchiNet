## Why

Hoy los campos de plantilla no se renderizan de forma consistente ni declarativa, lo que impide reutilizar metadatos (tipo, validaciones, obligatoriedad, accesibilidad) y aumenta el esfuerzo de mantenimiento. Necesitamos un componente que materialice dinámicamente la plantilla para garantizar reglas uniformes y extensibles.

## What Changes

- Crear un componente React que renderice dinámicamente campos de `camposPlantilla` con reglas de comportamiento, validación y accesibilidad.
- Incluir soporte de atributos de UI (label, tooltip, data-ident, data-api-method, data-group) e internacionalización de textos.
- Exponer eventos `onChange`, `onBlur`, `onFocus` para lógica adicional.

## Capabilities

### New Capabilities
- `campos-dinamicos-plantilla`: Renderizado dinámico de campos de plantilla con reglas de validación, accesibilidad e i18n.

### Modified Capabilities
- (none)

## Impact

- Nuevo componente en módulo de radicación (p.ej., `src/modules/radicacion/components`).
- Ajustes en UI donde se use `camposPlantilla` para renderizar campos dinámicos.
- Nuevos tests de rendering dinámico y validaciones (Vitest + Testing Library).
