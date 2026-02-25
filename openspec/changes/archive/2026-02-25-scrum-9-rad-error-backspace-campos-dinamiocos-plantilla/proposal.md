## Why

En el formulario de radicacion, los campos de plantilla se renderizan dinamicamente desde `camposPlantilla`. Al borrar contenido con la tecla `Backspace` en esos campos, se produce un error en consola y se degrada la experiencia de captura.

## What Changes

- Se corrige el manejo de eventos/estado para evitar errores al usar `Backspace` en campos dinamicos de plantilla.
- Se preserva el comportamiento de edicion normal (incluyendo borrado y valor vacio) sin romper el render actual.
- Se agregan pruebas de comportamiento para cubrir regresion del caso de `Backspace`.

## Capabilities

### New Capabilities
- `radicacion-campos-dinamicos-backspace-fix`: correccion del error al borrar contenido con `Backspace` en campos dinamicos.

### Modified Capabilities
- `campos-dinamicos-plantilla`: robustecer control de entrada y manejo de estado frente a valores vacios/borrado.

## Impact

- Cambios en componentes/hooks de renderizado dinamico en `src/modules/radicacion/`.
- Nuevas pruebas de no regresion para evitar reaparicion del error.
