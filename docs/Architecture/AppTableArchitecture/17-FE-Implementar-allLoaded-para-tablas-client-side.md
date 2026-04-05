# PROMPT ARQUITECTÓNICO
Implementar `allLoaded` para tablas client-side

## Rol esperado

Arquitecto de software senior y desarrollador frontend React
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Extender `AppTableExport` para soportar exportacion de todos los registros ya cargados en memoria en tablas client-side.

## Problema actual

Aunque una tabla use paginacion local, el sistema aun no distingue entre:

- pagina actual
- dataset completo cargado en front

## Objetivo funcional

Permitir exportar el conjunto completo de datos locales sin depender del page actual del grid.

## Alcance

- soportar modo `allLoaded`
- habilitarlo solo cuando el datasource lo provea
- diferenciarlo claramente de `allMatching`

## No alcance

- no implementar carga total desde backend
- no iterar paginas server-side desde navegador
- no redefinir query state

## Reglas funcionales

### `allLoaded`
- exporta todo lo cargado en memoria
- aplica a tablas sin paginacion server-side
- no debe mostrarse como equivalente a “todos los resultados del sistema”

## Reglas tecnicas

- usar `getAllLoadedRows`
- no derivar `allLoaded` desde `currentPage`
- no ofrecer esta opcion si el datasource no la implementa

## Archivos esperados

- `src/app/Components/UI/AppTable/AppTableExport.tsx`
- helpers asociados
- pruebas unitarias e integracion

## Riesgos a evitar

- confundir `allLoaded` con `allMatching`
- mostrar la opcion en tablas server-side sin soporte real
- duplicar logica de disponibilidad

## Pruebas obligatorias

- `allLoaded` funciona con datasets locales
- `allLoaded` no aparece sin datasource
- `allLoaded` no reemplaza semantica de `allMatching`

## Criterios de aceptación

- exportacion completa local disponible en tablas client-side
- opcion visible solo cuando aplica
- semantica clara y reusable

## Conclusión

Este ticket completa la capa local de exportacion y prepara el salto a la exportacion total server-side.
