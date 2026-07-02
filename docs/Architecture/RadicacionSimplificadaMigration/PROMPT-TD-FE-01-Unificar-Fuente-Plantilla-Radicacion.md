# PROMPT DE DEUDA TECNICA - Frontend Radicacion
# TD-FE-01 - Unificar fuente de plantilla y eliminar doble carga

## Objetivo

Corregir la deuda donde `RadicacionRoutePage` carga la plantilla, pero `RadicacionPage` la descarta y `RadicacionForm` vuelve a cargar campos con `useCamposPlantilla`.

Regla:

```txt
La plantilla de radicacion debe tener una sola fuente de verdad por montaje del modulo.
```

## Evidencia Actual

```txt
src/modules/radicacion/pages/RadicacionRoutePage.tsx
  carga useCamposPlantilla()
  mapCamposPlantillaToPlantillaRadicado(data)
  pasa plantilla a RadicacionPage

src/modules/radicacion/pages/RadicacionPage.tsx
  recibe plantilla
  ejecuta void plantilla
  renderiza TabsDocu sin props

src/modules/radicacion/components/RadicacionForm.tsx
  vuelve a ejecutar useCamposPlantilla()
```

## Alcance

- `RadicacionPage` debe usar realmente `plantilla`.
- `RadicacionTabs` debe recibir `plantilla` o contexto equivalente.
- `RadicacionForm` no debe volver a cargar plantilla si ya fue cargada por la ruta.
- `useCamposPlantilla` debe quedar en el boundary de ruta o en un provider unico.
- Eliminar `console.log` en `useCamposPlantilla`.

## Diseño Esperado

Opcion recomendada:

```txt
RadicacionRoutePage
  -> carga plantilla
  -> RadicacionPage plantilla={plantilla}
      -> RadicacionTabs plantilla={plantilla}
          -> RadicacionForm plantilla={plantilla}
```

Si se crea provider:

```txt
RadicacionPlantillaProvider
  - plantilla
  - camposPlantilla
  - isLoading
  - error
```

No usar ambas estrategias al tiempo.

## Criterios de Aceptacion

- `RadicacionPage` no descarta `plantilla`.
- `RadicacionForm` no llama `useCamposPlantilla` si recibe plantilla/campos desde arriba.
- No hay doble request a `/api/PlantillaRadicado/listaPlantilla`.
- Se elimina `console.log(data)` de `useCamposPlantilla`.
- Tests actualizados para validar que `RadicacionPage` propaga plantilla.
- Tests actualizados para validar que `RadicacionForm` renderiza desde props/contexto.

## Fuera de Alcance

- registrar radicacion contra backend;
- pendientes;
- documentos;
- refactor grande del formulario.
