# PROMPT ARQUITECTÓNICO
Implementar `AppTableExport` para `currentPage` y `selectedRows`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Implementar la primera version funcional de `AppTableExport` resolviendo exportacion local de:

- pagina actual
- registros seleccionados

sin dependencia de backend.

## Problema actual

No existe una pieza reusable que permita exportar lo visible o lo seleccionado desde cualquier tabla basada en `AppTable`.

## Objetivo funcional

Permitir exportar datasets locales pequenos o visibles usando columnas y filas ya disponibles en frontend.

## Alcance

- crear componente o hook `AppTableExport`
- soportar `currentPage`
- soportar `selectedRows`
- integrar generacion local de archivo para formatos soportados en esta fase
- dejar preparado el componente para exponerse mediante `AppDropdown`

## No alcance

- no implementar `allMatching`
- no conectar backend
- no migrar todos los modulos
- no resolver PDF complejo si requiere layout avanzado

## Reglas funcionales

### `currentPage`
- usa las filas visibles de la tabla

### `selectedRows`
- usa seleccion actual
- si no hay filas seleccionadas:
  - ocultar opcion
  - o dejarla disabled con semantica clara

## Reglas tecnicas

- reutilizar metadata de columnas visibles
- no exportar acciones puramente visuales si no tienen valor de dato
- mantener separada la logica de serializacion del trigger visual
- la implementacion grafica debe contemplar un trigger reusable basado en `AppDropdown`
- aunque la integracion visual completa se consolide en un ticket posterior, este ticket debe dejar compatible la estructura del menu de descarga
- la salida exportada debe poder incorporar encabezado ejecutivo con metadata del reporte
- la imagen corporativa debe salir de un asset del repo, no de una ruta local ad hoc
- la imagen corporativa debe insertarse dentro del archivo generado y no referenciarse como URL

Regla por formato en esta fase:

- `xlsx`
  - debe considerarse apto para encabezado ejecutivo con logo incrustado
- `pdf`
  - si se soporta en esta fase, debe seguir la misma regla de encabezado ejecutivo
- `csv`
  - puede limitarse a exportacion plana sin logo, manteniendo metadata institucional por nombre de archivo o estrategia textual compatible

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTableExport.tsx`
- helpers de serializacion local si aplica
- pruebas de `AppTableExport`

## Riesgos a evitar

- exportar columnas de acciones como si fueran datos
- depender de una pantalla concreta
- leer estado interno de AG Grid de forma acoplada
- mezclar UI del menu con logica de transformacion de archivo

## Pruebas obligatorias

- exportar `currentPage`
- exportar `selectedRows`
- comportamiento cuando no hay seleccion
- consistencia con columnas visibles

## Criterios de aceptación

- existe `AppTableExport` reusable
- exporta pagina actual
- exporta seleccionados
- no depende de backend
- no depende de un modulo concreto
- queda listo para integrarse mediante `AppDropdown` como patron visual del menu de descarga
- soporta encabezado de reporte profesional cuando el formato lo permita

## Conclusión

Este ticket valida la pieza reusable con casos locales antes de entrar a exportacion total server-side.
