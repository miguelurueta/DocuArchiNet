# Ticket 02 BE

## Titulo

Exponer total real y conteo filtrado en `workflowInboxgestion`

## Objetivo

Implementar paginacion servidor real en el endpoint:

`POST /api/workflowInboxgestion/inboxgestion`

de forma que:

`DynamicUiTableDto.Pagination.Total`

represente el total real de registros de la consulta filtrada y no únicamente el numero de filas de la pagina actual.

## Problema actual

- `Pagination.Total` se construye con `rows.Count`
- no existe conteo real de la consulta filtrada
- el frontend no puede representar rangos reales tipo:
  - `1-25 de 320`
- rompe UX tipo Gmail

## Alcance

- implementar cálculo de total real de la consulta filtrada
- propagar el total desde repository/query layer hasta:
  - `DynamicUiTableDto.Pagination.Total`
- reutilizar la lógica existente del `QueryBuilder`
- preservar comportamiento actual de:
  - `Search`
  - `SearchType`
  - `StructuredFilters`
  - `SortField`
  - `SortDir`
  - `Page`
  - `PageSize`

## No alcance

- no cambiar contrato público del endpoint
- no rediseñar búsqueda avanzada
- no modificar frontend
- no cambiar lógica funcional existente de filtros
- no introducir SQL dinámico inseguro
- no rediseñar por completo la infraestructura de `QueryOptions`

## Dependencias

- Ticket 01 BE completado
  - claims reales
  - eliminación de hardcodes
  - empty state estructurado

## Flujo arquitectónico (obligatorio)

- Controller
- Service
- Repository
- QueryBuilder
- DapperCrudEngine
- Base de datos

## Regla arquitectónica crítica

La query de datos y la query de conteo deben construirse a partir de la misma lógica de filtros del `QueryBuilder`.

No se permite:

- duplicar lógica de filtros
- construir una query de conteo con reglas funcionales distintas
- omitir filtros en la query de conteo

## Reglas de implementación (obligatorias)

1. `Pagination.Total` debe representar el total real de la consulta filtrada.

2. El total debe respetar exactamente:
   - filtros base del workflow
   - `Search`
   - `SearchType`
   - `StructuredFilters`
   - joins requeridos por filtros
   - condiciones de seguridad existentes

3. El total debe calcularse mediante una query de tipo:
   - `COUNT(1)` o equivalente funcional seguro

4. La query de conteo debe:
   - reutilizar exactamente la misma lógica de filtros que la query principal
   - no incluir `ORDER BY`
   - no incluir columnas innecesarias
   - no alterar joins requeridos por filtros

5. El `QueryBuilder` debe exponer una construcción separada para datos y otra para conteo, o una construcción compartida con variantes derivadas, siempre que ambas reutilicen la misma lógica de filtros.

6. No se permite:
   - usar `rows.Count` como total
   - traer todos los registros para contar
   - ejecutar lógica de conteo fuera del flujo repository/query builder

7. `Page` y `PageSize`:
   - no deben afectar el cálculo del total
   - solo afectan la query de datos

8. Caso de página fuera de rango:
   - si `Page > totalPages`
   - retornar `Rows = []`
   - mantener `Total` correcto
   - no lanzar error

9. Empty state:
   - `Rows = []`
   - `Pagination.Total = 0`
   - estructura completa de `DynamicUiTableDto` intacta

## Reglas de seguridad

- no introducir SQL inseguro nuevo
- respetar validaciones existentes de columnas y metadata
- no aceptar columnas fuera de whitelist
- `StructuredFilters` debe mantenerse sanitizado
- cualquier mejora de parametrización debe respetar la arquitectura actual y no convertir este ticket en una refactorización completa del query layer

## Riesgos a evitar (obligatorio)

- contar sin aplicar `StructuredFilters`
- contar sin aplicar `Search`
- contar sin aplicar filtros base del workflow
- duplicar lógica entre query de datos y query de conteo
- incluir `ORDER BY` en la query de conteo
- degradar performance por `COUNT` mal implementado
- romper `offset/limit` actual

## Pruebas obligatorias

### Unit Tests

- total real sin filtros
- total real con `Search`
- total real con `StructuredFilters`
- total real con combinación de filtros + sort
- `Page` y `PageSize` consistentes
- página fuera de rango -> `Rows = []` + `Total` correcto
- empty state -> `Pagination.Total = 0`

### Integration Tests

- validación contra base de datos real si el entorno de pruebas lo permite
- coincidencia entre:
  - `COUNT`
  - cantidad real de registros filtrados

## Criterios de aceptación

- `Pagination.Total` ya no usa `rows.Count`
- el total corresponde a la consulta filtrada real
- no se rompe el contrato `DynamicUiTableDto`
- no se rompe búsqueda, filtros ni ordenamiento
- la solución es consistente con el `QueryBuilder` existente
- no hay duplicación de lógica funcional
- el endpoint queda preparado para soportar paginación servidor tipo Gmail en frontend

## Restricciones

- no romper arquitectura actual del proyecto
- no mover lógica al controller
- no modificar contratos existentes
- no introducir SQL concatenado inseguro nuevo
- no inventar filtros o joins no existentes
- mantener consistencia con `DapperCrudEngine` + `QueryOptions`
