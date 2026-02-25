
## Why

Los campos con autocompletado requieren lógica uniforme de consulta a la API y manejo de errores, hoy dispersa o inexistente. Centralizarlo en un componente reutilizable reduce inconsistencias, mejora la UX y facilita mantenimiento.

## What Changes

- Crear un componente React que filtre `camposPlantilla` por `campo_tip = 1` y `ComportamientoCampo = "AUTOCOMPLETE"` y renderice autocompletados dentro de `<Card data-ident="pl-radicacion-card-spe">`.
- Integrar consumo de `/api/PlantillaRadicado/solicitaAutoCompleteCampos` con parámetros dinámicos por campo y manejo centralizado de errores en Axios.
- Soportar accesibilidad, estados de carga, i18n de textos visibles y eventos `onChange`, `onBlur`, `onFocus`.
- Documentar extensión para nuevos endpoints/validaciones.

## Capabilities

### New Capabilities
- `autocomplete-campos-plantilla`: Autocompletado dinámico de campos de plantilla con consulta API y manejo robusto de errores.

### Modified Capabilities
- (none)

## Impact

- Nuevo componente reutilizable en `src/modules/radicacion/components`.
- Nuevas utilidades/hook de consumo de API en `src/modules/radicacion/services` o `src/modules/radicacion/hooks`.
- Tests de autocompletado con Vitest + Testing Library.
