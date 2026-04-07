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
  - modulo frontend que transforma `queryState` hacia `WorkflowInboxApiRequestDto`
- Endpoint backend:
  - `POST /api/workflowInboxgestion/inboxgestion`
- Builder backend:
  - `WorkflowInboxQueryBuilder.ApplyLikeSearch`

## Ubicacion esperada

```txt
src/modules/gestionCorrespondencia/hooks/useGestionCorrespondenciaTable.ts
src/modules/gestionCorrespondencia/**/mappers/*
src/modules/gestionCorrespondencia/tests/*
```

## Restricciones obligatorias

- no modificar `AppInputSearch` para conocer `SearchType`
- no hardcodear SQL ni nombres de columnas en frontend
- no cambiar el endpoint de listado
- no romper busqueda avanzada `SearchType = 3`
- no modificar autorizacion ni claims
- no deshabilitar paginacion

## Contrato obligatorio

Cuando `queryState.search` tenga texto efectivo de busqueda simple, el request debe incluir:

```ts
{
  Search: queryState.search,
  SearchType: 2
}
```

Cuando no haya texto, el request puede omitir `Search` o enviarlo vacio segun el contrato existente, pero no debe forzar una busqueda `LIKE` innecesaria.

## Reglas de implementacion obligatorias

1. Resolver `SearchType = 2` en hook o mapper de `GestionCorrespondencia`, no en `AppInputSearch`.
2. Mantener compatibilidad con filtros estructurados existentes.
3. Mantener compatibilidad con paginacion server-side.
4. Resetear o conservar pagina segun el patron existente de `onQueryChange` en `AppTableQueryWrapper`.
5. No cambiar `SearchType = 3` cuando el flujo de busqueda avanzada lo use explicitamente.
6. Centralizar el mapeo para evitar que cada pantalla arme requests manualmente.

## Riesgos a evitar

- que `Search` llegue con `SearchType = 1` y no filtre
- romper busqueda avanzada
- disparar `LIKE` con texto vacio
- acoplar UI a detalles backend
- duplicar mapeo de request en pagina
- cambiar paginacion o exportacion por accidente

## Pruebas unitarias obligatorias

- cuando hay texto simple, el mapper envia `SearchType = 2`
- cuando no hay texto, no fuerza `LIKE` innecesario
- si existe `SearchType = 3`, lo conserva para busqueda avanzada
- `onQueryChange({ search })` actualiza el request esperado
- paginacion y page size se preservan
- filtros estructurados existentes se preservan

## Pruebas QT / calidad

- usuario escribe una palabra y la tabla se consulta filtrada
- usuario limpia el texto y la tabla vuelve a consulta sin filtro textual
- usuario pagina despues de buscar y mantiene el filtro activo
- exportacion `allMatching` respeta el filtro activo si aplica
- no se observan requests con `SearchType = 1` cuando hay busqueda simple

## Criterios de aceptacion

- la busqueda simple de `GestionCorrespondencia` usa `SearchType = 2`
- el backend puede activar `ApplyLikeSearch`
- `AppInputSearch` permanece presentacional
- no se rompe busqueda avanzada ni filtros existentes
- pruebas de hook/mapper cubren el contrato

