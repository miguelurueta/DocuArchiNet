# PROMPT ARQUITECTÓNICO
Estandarizar tipografía de registros en `AppTable` con estilo tipo Gmail

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Ajustar la tipografía visual de los registros de `AppTable` para que tenga una lectura más limpia, compacta y consistente, con una referencia visual tipo Gmail:

- fuente sans limpia
- tamaño de texto controlado
- peso regular
- line-height compacta y legible

## Problema actual

Hoy la tipografía de filas en `AppTable` no está suficientemente estandarizada a nivel visual.
Eso puede generar:

- inconsistencia entre headers y filas
- densidad visual poco controlada
- sensación menos refinada en tablas tipo inbox o listado

## Objetivo funcional

Definir un estándar tipográfico reusable para:

- filas del grid
- headers del grid
- cards, cuando `presentationMode="cards"` aplique

## Alcance

- ajustar `font-family`
- ajustar `font-size`
- ajustar `font-weight`
- ajustar `line-height`
- mantener buena legibilidad en tabla y cards
- preservar consistencia con el resto del sistema visual

## No alcance

- no rediseñar `AppTable`
- no cambiar backend
- no mezclar este ticket con click, selección o foco
- no copiar Gmail de forma literal

## Reglas de implementación

- definir tipografía como estándar shared de `AppTable`
- no hardcodear estilos solo para una pantalla
- diferenciar:
  - texto de filas
  - texto de header
- mantener compatibilidad con `presentationMode="table"` y `presentationMode="cards"`

## Dirección visual recomendada

Referencia aproximada:

- filas:
  - `font-size: 13px` o `14px`
  - `font-weight: 400`
  - `line-height: 1.35` o `1.4`
- headers:
  - un poco más densos o con peso moderado
  - sin exagerar contraste

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTable.module.css`
- `src/app/Components/UI/AppTable/renderers/AppTableGridRenderer.tsx`
- `src/app/Components/UI/AppTable/renderers/AppTableCardRenderer.tsx`
- configuración visual AG Grid si aplica
- tests visuales o de clase si existen

## Riesgos a evitar

- romper consistencia con otros componentes
- usar una fuente demasiado distinta al sistema actual
- apretar demasiado el texto y perder legibilidad
- cambiar solo una pantalla en vez del componente shared

## Pruebas obligatorias

- tabla mantiene legibilidad en desktop
- cards mantienen legibilidad
- headers y rows quedan visualmente consistentes
- no se rompen layout ni alturas de fila

## Criterios de aceptación

- `AppTable` queda con una tipografía shared más limpia y consistente
- el look se aproxima a un listado tipo Gmail
- la solución aplica al componente reusable, no solo a una pantalla
- no se rompen tabla ni cards

## Conclusión

Este trabajo debe ir en un ticket separado de `SCRUMCORE-48`.

Separación correcta:

- `SCRUMCORE-48`: click, selección y foco de celda
- nuevo ticket: tipografía visual de `AppTable`
