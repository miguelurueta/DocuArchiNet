# PROMPT ARQUITECTONICO Ticket 03 FE

# Conectar busqueda LIKE simple en GestionCorrespondencia

## Rol esperado

Arquitecto de software senior frontend (React, data fetching, contratos API, integracion con backend).

## Objetivo

Asegurar que la busqueda simple escrita desde `AppInputSearch` en `GestionCorrespondencia` llegue al backend con `SearchType = 2`, para activar la busqueda global tipo `LIKE` ya soportada por `WorkflowInboxQueryBuilder`.

## Contexto existente

- Documento tecnico:
  - `docs/Architecture/GestionCorrrespondecia/WorkflowInbox-Busqueda-Autocomplete-Architecture.md`
- Hook de tabla:
  - `src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts`
- Mapper de request:
  - `src/modules/gestionCorrespondencia/adapters/gestionCorrespondenciaTableRequestMapper.ts`
- Mapper shared actual:
  - `src/app/Components/UI/AppTable/utils/dynamicUiTableRequestMapper.ts`
- Endpoint backend:
  - `POST /api/workflowInboxgestion/inboxgestion`
- Builder backend:
  - `WorkflowInboxQueryBuilder.ApplyLikeSearch`

## Ubicacion esperada

```txt
src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts
src/modules/gestionCorrespondencia/adapters/gestionCorrespondenciaTableRequestMapper.ts
src/modules/gestionCorrespondencia/tests/*
```

## Restricciones obligatorias

- no modificar `AppInputSearch` para conocer `SearchType`
- no hardcodear SQL ni nombres de columnas en frontend
- no cambiar el endpoint de listado
- no romper busqueda avanzada `SearchType = 3`
- no modificar autorizacion ni claims
- no deshabilitar paginacion
- no duplicar logica de mapeo fuera del mapper del modulo
- no modificar el mapper shared de `AppTable` si el cambio solo aplica a `GestionCorrespondencia`

## Regla arquitectonica obligatoria

La decision de `SearchType` debe centralizarse en el request mapper del modulo.

El hook reutiliza ese mapper y no debe reimplementar la logica.

Implementacion recomendada:

- `mapGestionCorrespondenciaTableRequest` debe envolver `mapDynamicUiServerTableRequest`
- la normalizacion de `SearchType` especifica del workflow inbox debe vivir en `gestionCorrespondenciaTableRequestMapper.ts`
- `mapDynamicUiServerTableRequest` debe permanecer generico para evitar efectos colaterales en otras tablas

## Contrato obligatorio

Cuando `queryState.search` tenga texto efectivo de busqueda simple, el request debe incluir:

```ts
{
  Search: queryState.search,
  SearchType: 2
}
```

### Definicion de texto efectivo

Texto efectivo significa:

```txt
queryState.search.trim().length > 0
```

Cuando no haya texto efectivo:

- no debe enviarse `SearchType = 2`
- `Search` se omite o se envia vacio segun el contrato ya existente del modulo
- esa decision debe ser consistente y centralizada en el mapper

## Regla de precedencia obligatoria

- si el flujo avanzado define explicitamente `SearchType = 3`, ese valor tiene prioridad
- `SearchType = 2` solo aplica cuando existe busqueda simple efectiva y no hay override explicito de busqueda avanzada
- si se recibe otro `SearchType` explicito, el mapper debe conservarlo o documentar de forma expresa por que lo normaliza

Regla recomendada:

```ts
if (input.searchType === 3) return 3;
if (input.search?.trim()) return 2;
return input.searchType;
```

## Reglas de implementacion obligatorias

1. Resolver `SearchType = 2` en el request mapper de `GestionCorrespondencia`, no en `AppInputSearch`.
2. Mantener compatibilidad con filtros estructurados existentes.
3. Mantener compatibilidad con paginacion server-side.
4. Resetear o conservar pagina segun el patron existente de `onQueryChange` en `AppTableQueryWrapper`.
5. No cambiar `SearchType = 3` cuando la busqueda avanzada lo use explicitamente.
6. Centralizar el mapeo para evitar que cada pantalla arme requests manualmente.
7. Preservar en el mapper, como minimo:
   - `page` -> `Page`
   - `pageSize` -> `PageSize`
   - `search` -> `Search`
   - `searchType` -> `SearchType`
   - `structuredFilters` -> `StructuredFilters`
   - `sortField` -> `SortField`
   - `sortDir` -> `SortDir` normalizado a `"ASC" | "DESC"`
8. Mantener `includeConfig` / `IncludeConfig` cuando aplique.
9. Asegurar que `getAllMatchingRows` y exportacion backend usen el mismo mapper o una regla equivalente documentada.

## Riesgos a evitar

- que `Search` llegue con `SearchType = 1` y no filtre
- romper busqueda avanzada
- disparar `LIKE` con texto vacio o solo espacios
- acoplar UI a detalles backend
- duplicar mapeo de request en pagina o hook
- cambiar paginacion o exportacion por accidente
- modificar el mapper shared y afectar otros modulos
- divergir entre request de tabla, `allMatching` y exportacion

## Pruebas unitarias obligatorias

- cuando hay texto simple efectivo, el mapper envia `SearchType = 2`
- cuando no hay texto efectivo, no fuerza `LIKE` innecesario
- si existe `SearchType = 3`, lo conserva para busqueda avanzada
- si existe `SearchType` explicito distinto de `3`, conserva o documenta el comportamiento esperado
- `onQueryChange({ search })` actualiza el request esperado
- paginacion y page size se preservan
- filtros estructurados existentes se preservan
- `sortField` y `sortDir` se preservan como `SortField` y `SortDir`
- `IncludeConfig` se preserva cuando aplique
- `getAllMatchingRows` mantiene `Search` y `SearchType` esperados
- exportacion backend mantiene `Search` y `SearchType` esperados si aplica

## Pruebas QT / calidad

- usuario escribe una palabra y la tabla se consulta filtrada
- usuario limpia el texto y la tabla vuelve a consulta sin filtro textual
- usuario pagina despues de buscar y mantiene el filtro activo
- no se observan requests con `SearchType = 1` cuando hay busqueda simple efectiva
- `allMatching` y exportacion no pierden el filtro activo

## Criterios de aceptacion

- la busqueda simple de `GestionCorrespondencia` usa `SearchType = 2`
- el backend puede activar `ApplyLikeSearch`
- `AppInputSearch` permanece presentacional
- no se rompe busqueda avanzada ni filtros existentes
- pruebas de hook/mapper cubren el contrato
- el cambio se limita al mapper del modulo o justifica cualquier modificacion shared

