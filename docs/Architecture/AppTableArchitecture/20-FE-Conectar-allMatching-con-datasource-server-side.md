# PROMPT ARQUITECTÓNICO
Conectar `allMatching` con datasource server-side

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + integracion con backend)

## Objetivo

Conectar `AppTableExport` con la estrategia backend definida para exportar todos los resultados de la consulta activa.

## Problema actual

Aun teniendo contrato reusable y backend definido, falta enlazar el modo `allMatching` desde frontend sin acoplarlo a una sola pantalla.

## Objetivo funcional

Permitir que una tabla server-side exporte todos los resultados usando el `queryState` actual y un datasource o adapter inyectable.

## Alcance

- soportar `getAllMatchingRows`
- o soportar una accion server de exportacion directa
- propagar `queryState`
- mantener componente reusable

## No alcance

- no redefinir contratos base
- no migrar todos los modulos
- no mover backend al componente

## Reglas funcionales

- `allMatching` debe respetar filtros, sort y busqueda actual
- no debe usar solo `rows` visibles
- debe mostrar estado de carga si el proceso tarda
- ese estado de carga no debe activar `Skeleton Screen` de la tabla
- debe propagar metadata del reporte para construir encabezado ejecutivo cuando aplique

## Reglas tecnicas

- la capacidad debe ser opt-in
- si no existe datasource server-side:
  - `allMatching` no aparece
  - o queda deshabilitado explicitamente
- no acoplar la implementacion a `GestionCorrespondencia`

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTableExport.tsx`
- adapters o hooks de exportacion si aplica
- pruebas de integracion con datasource async

## Riesgos a evitar

- asumir datos completos en `rows`
- mezclar `allLoaded` con `allMatching`
- acoplar hooks del modulo al componente reusable

## Pruebas obligatorias

- `allMatching` usa datasource async
- se propaga `queryState`
- comportamiento cuando falla exportacion
- comportamiento cuando no existe capacidad server-side
- exportacion async mantiene visible la tabla actual
- exportacion async refleja loading en el trigger o controles de exportacion
- metadata del reporte llega al flujo server-side

## Criterios de aceptación

- `allMatching` funciona en tablas server-side
- la integracion sigue siendo reusable
- la semantica de consulta activa se preserva

## Conclusión

Este ticket cierra la brecha entre arquitectura reusable y exportacion total real.
