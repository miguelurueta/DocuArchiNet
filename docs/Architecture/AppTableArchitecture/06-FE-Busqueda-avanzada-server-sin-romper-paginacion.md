# Ticket 06 FE

## Titulo

Integrar busqueda avanzada server sin romper paginacion

## Objetivo

Integrar la búsqueda server al flujo real de consulta usando `AppTableQueryState` como única fuente de verdad, sin romper:

- `page`
- `pageSize`
- `total`
- `sortField`
- `sortDir`

La búsqueda server incluye tanto búsqueda simple como búsqueda avanzada, y debe serializarse de forma consistente hacia el backend.

## Problema actual

- la búsqueda avanzada puede quedar aislada del estado compartido
- eso rompe paginación, total visible y navegación
- distintos hooks o pantallas podrían serializar requests de forma distinta

## Alcance

- integrar `StructuredFilters`
- integrar `SearchType`
- integrar `Search`
- integrar `SortField`
- integrar `SortDir`
- garantizar reset correcto de `page`
- centralizar serialización del request server

## No alcance

- no rediseñar backend
- no crear filtros visuales de dominio muy específicos
- no migrar aún otras pantallas fuera de la primera integración
- no mezclar quick filter local con búsqueda server
- no diseñar en este ticket la UI final de filtros avanzados; este ticket resuelve integración de estado y request

## Dependencias

- Ticket 02 BE completado
- Ticket 03 FE completado
- Ticket 05 FE completado

## Archivos frontend esperados

- hooks de tabla dinámica
- adapters / request mappers
- integración opcional con wrapper reusable, sin mover ahí la lógica de serialización
- tests de hooks e integración

## Regla arquitectónica obligatoria

`AppTableQueryState` es la única fuente de verdad para:

- búsqueda simple
- búsqueda avanzada
- paginación
- sort

La serialización al request backend debe pasar por un único helper o mapper reusable.

## Definición de búsqueda server

La búsqueda server se compone de:

- `Search`
- `SearchType`
- `StructuredFilters`

Todos estos campos deben derivarse exclusivamente del `AppTableQueryState`.

## Reglas de implementación

- todos los cambios de filtros deben pasar por `AppTableQueryState`
- cambiar `Search` resetea `page = 1`
- cambiar `SearchType` resetea `page = 1`
- cambiar `StructuredFilters` resetea `page = 1`
- cambiar `SortField` o `SortDir` resetea `page = 1`
- `pageSize` se conserva salvo cambio explícito
- `refresh` no altera filtros, sort ni paginación
- el total mostrado debe venir del backend ya filtrado
- en `paginationMode = "server"`, `quickFilterText` no participa del request
- no recalcular total localmente
- cuando `structuredFilters` cambie efectivamente a `[]`, se debe resetear `page = 1`
- cuando `search` cambie efectivamente a `""`, se debe resetear `page = 1`

## Contrato mínimo del request mapper

Debe serializar, como mínimo, un shape compatible con el endpoint backend real, incluyendo:

- `Page`
- `PageSize`
- `Search`
- `SearchType`
- `StructuredFilters`
- `SortField`
- `SortDir`

El state interno puede mantenerse en camelCase, pero el mapper debe producir el shape requerido por el backend.

## Validación frontend

- frontend puede aplicar validación estructural mínima del state
- frontend no debe reemplazar validaciones funcionales o de seguridad del backend
- la validación de operadores, whitelist y sanitización profunda sigue siendo responsabilidad backend

## Riesgos a evitar

- duplicar filtros en estado local y estado backend
- mezclar quick filter local con búsqueda avanzada server
- recalcular total localmente en server mode
- serializar el request de forma distinta por pantalla
- mantener estados paralelos para filtros

## Pruebas obligatorias

- búsqueda simple con paginación activa
- búsqueda avanzada con paginación activa
- cambio de sort con paginación activa
- reset de `page`
- total consistente con la consulta
- limpiar búsqueda simple resetea `page`
- `structuredFilters = []` resetea `page`
- `refresh` no altera el query state
- `quickFilterText` no afecta server mode
- serialización compatible con el request backend real

## Criterios de aceptación

- la búsqueda server usa el request real del backend
- toda la consulta sale desde `AppTableQueryState`
- no se rompe paginación
- no se rompe total
- no se rompe sort
- la serialización queda centralizada y reusable
- frontend no duplica validaciones profundas que ya pertenecen al backend

## Instrucción final

Antes de implementar:

- validar contrato backend real del endpoint
- validar `AppTableQueryState`
- validar hooks actuales de consulta dinámica
- validar request mappers existentes

Luego:

- implementar con TypeScript estricto
- mantener separación de capas
- mantener la serialización centralizada

Finalmente reportar:

- decisiones de diseño
- estrategia de serialización
- reglas de reset aplicadas
- cómo se evita duplicación en pantallas
- compatibilidad con backend real y con futuras tablas server mode
