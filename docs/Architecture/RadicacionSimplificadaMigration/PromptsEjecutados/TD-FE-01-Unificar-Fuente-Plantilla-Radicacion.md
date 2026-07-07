# TD-FE-01 - Unificar Fuente De Plantilla De Radicacion

## Que Hace

Centraliza la carga de la plantilla de radicacion en el boundary del modulo. Evita que componentes internos disparen consultas paralelas o construyan modelos distintos.

## Casos De Uso Cubiertos

- Al entrar a radicacion, el modulo carga una sola vez la estructura de plantilla.
- `RadicacionPage`, `RadicacionTabs` y `RadicacionForm` reciben la misma fuente de datos.
- Si la plantilla falla, el error se maneja antes de montar el formulario.

## Arquitectura Implementada

```text
RadicacionRoutePage
  -> useCamposPlantilla()
  -> mapCamposPlantillaToPlantillaRadicado()
  -> RadicacionPage
  -> RadicacionTabs
  -> RadicacionForm
```

## Archivos Principales

- `src/modules/radicacion/pages/RadicacionRoutePage.tsx`
- `src/modules/radicacion/pages/RadicacionPage.tsx`
- `src/modules/radicacion/hooks/RadicacionTabs.tsx`
- `src/modules/radicacion/components/RadicacionForm.tsx`

## Si Falla, Revisar

- Que no se haya reintroducido `useCamposPlantilla()` dentro de `RadicacionForm`.
- Que `camposPlantilla` y `plantilla` vengan desde `RadicacionRoutePage`.
- Que el mapper de plantilla siga soportando respuesta vacia con `EMPTY_PLANTILLA_RADICADO`.

## Restricciones

- No duplicar llamadas de plantilla.
- No crear estado local alterno para la estructura del formulario.

