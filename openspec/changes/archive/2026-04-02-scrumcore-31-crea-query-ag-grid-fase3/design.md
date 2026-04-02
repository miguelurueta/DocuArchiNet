## Context

`SCRUMCORE-31` introduce la fase de consulta para `AppTable` después de que `SCRUMCORE-30` dejó lista la capa de contratos y adapters `DynamicUiTableDto -> AppDataTableAgGrid`. El backend ya entrega una respuesta dinámica y homogénea basada en `ApiResponse<DynamicUiTableDto | null>`, mientras que el frontend todavía no tiene una capa estándar para solicitar esa tabla, aplicar ACL al request y exponer el resultado a React Query sin acoplar la implementación a un módulo funcional concreto.

El cambio debe vivir dentro de `src/app/Components/UI/AppTable/` porque su objetivo es transversal. El ticket fija restricciones duras: usar `clienteApi`, usar React Query solo dentro del hook, no modificar adapters de fase 1B, no modificar `AppTable` base, no ejecutar acciones ni resolver lógica de negocio de dominio, y mantener el contrato de respuesta fiel al backend.

## Goals / Non-Goals

**Goals:**
- Definir un contrato de entrada genérico `DynamicTableQueryInput` para que cada dominio construya su request sin filtrar la respuesta.
- Implementar un servicio reutilizable sobre `clienteApi` que retorne exactamente `Promise<ApiResponse<DynamicUiTableDto | null>>`.
- Implementar un hook `useDynamicUiTableQuery` que concentre React Query, traduzca el DTO con los adapters existentes y exponga un resultado consumible por `AppTable`.
- Estandarizar estados vacíos, errores y paginación sin introducir dependencias al dominio.
- Dejar pruebas y documentación que expliquen el patrón para futuras integraciones.

**Non-Goals:**
- No modificar `dynamicUiToAgGridColumns`, `dynamicUiToAgGridRows` ni `dynamicUiActionMapper`.
- No renderizar pantallas, no conectar módulos concretos y no disparar `RowActions` o `CellActions`.
- No definir ACL sobre la respuesta ni enriquecer el payload del backend con reglas de negocio.
- No mover React Query fuera del hook ni convertir el servicio en una capa stateful.
- No cerrar todavía la última milla entre `AppDataTableAgGrid` y el contrato visual de `AppTable` (`ColDef<T>[]` + filas planas), porque esa adaptación no forma parte de esta fase.

## Decisions

### 1. ACL y variabilidad del dominio solo viven en el request

Se introduce `DynamicTableQueryInput` como contrato interno mínimo y `RequestMapper<TRequest>` como mecanismo obligatorio para proyectar ese input al request real de cada endpoint. La capa transversal no conocerá claims, ids extra, filtros de dominio o nombres finales del payload; solo sabrá invocar un mapper que produce `TRequest`.

Esta decisión evita acoplar `AppTable` a tickets o módulos específicos y cumple la restricción del ticket: ACL sí en request, no en response. La alternativa de modelar un request DTO único y cerrado se descarta porque obligaría a incorporar campos de dominio en una capa transversal y haría que cada nuevo backend rompiera el contrato común.

### 2. El servicio HTTP es delgado y preserva el contrato del backend

`dynamicUiTable.service.ts` debe exponer `getDynamicTable<TRequest>(request: TRequest): Promise<ApiResponse<DynamicUiTableDto | null>>` y delegar el transporte a `clienteApi`. La responsabilidad del servicio es únicamente enviar el request, devolver el shape del backend y reutilizar el manejo transversal ya existente de Axios e interceptores.

La capa de servicio no mapeará columnas, filas ni errores de UI. Esa separación permite probar por un lado el cumplimiento del contrato HTTP y por otro la transformación a estado visual. La alternativa de devolver directamente `AppDataTableAgGrid` desde el servicio se descarta porque mezclaría transporte y presentación y rompería la reutilización de los adapters de fase 1B.

### 3. React Query queda encapsulado en un hook adaptador

`useDynamicUiTableQuery.ts` será el único punto con React Query. El hook recibirá `input`, `requestMapper` y `queryFn`, generará la query key exacta definida por el ticket y, una vez reciba `ApiResponse<DynamicUiTableDto | null>`, convertirá el DTO a `AppDataTableAgGrid` con los adapters existentes.

El hook expondrá una API estable orientada al consumo por componentes: `rows`, `columns`, `total`, `pagination`, `loading`, `error`, `isEmpty` y `refetch`, con `rawResponse` opcional para casos de observabilidad o depuración. La alternativa de entregar el objeto de React Query completo se descarta porque filtraría detalles de infraestructura a consumidores que solo necesitan datos normalizados.

La salida del hook se mantendrá en el modelo intermedio `AppGridRow[]` y `AppGridColumn[]`, no en `ColDef[]`. Esto es coherente con la fase 1B ya implementada, pero implica que la integración visual final con `AppTable` requerirá una capa posterior que aplane filas y construya `ColDef<T>` sin alterar el componente base.

### 4. `data = null` se trata como vacío exitoso

Cuando el backend retorne `success = true` y `data = null`, el hook no generará error. En su lugar devolverá columnas y filas vacías, `total = 0`, `isEmpty = true` y conservará la metadata del response cuando exista.

Esta decisión sigue el contrato explícito del ticket y evita que los consumidores diferencien entre “sin datos” y “falló la consulta” usando heurísticas. La alternativa de lanzar error ante `data = null` se descarta porque forzaría a tratar una respuesta válida como excepción.

### 5. Los errores se normalizan en el hook, no en los adapters

El hook tratará dos fuentes de error: respuestas con `success = false` y errores de transporte arrojados por `clienteApi`/Axios. En ambos casos expondrá `error: Error | null` siguiendo los patrones ya usados en el proyecto, sin alterar los adapters ni introducir tipos de error de negocio en la capa transversal.

La decisión mantiene a los adapters puros y deterministas. También evita que el servicio dependa de React Query o de objetos de estado. La alternativa de encapsular errores dentro de `rows`/`columns` se descarta porque degrada la semántica del consumo y dificulta las pruebas.

## Risks / Trade-offs

- [El endpoint concreto puede variar entre módulos] -> La firma del hook acepta `queryFn` inyectable y un `RequestMapper<TRequest>` para desacoplar transporte y forma del request.
- [El backend mezcla convenciones PascalCase/camelCase] -> Se preserva `ApiResponse<DynamicUiTableDto | null>` y la normalización queda en los contratos ya creados en fase 1B.
- [React Query puede filtrar detalles de caché a consumidores] -> El hook expone una superficie reducida y estable, manteniendo la key exacta del ticket para evitar divergencias.
- [Errores de transporte y `success=false` pueden terminar tratados distinto] -> Las pruebas del hook deben cubrir ambos caminos y validar una única salida `Error | null`.
- [Futuras pantallas pueden pedir más metadata de la query] -> Se deja `rawResponse` opcional para extensión sin cambiar el contrato principal del hook.
- [El resultado del hook no entra directo a `AppTable`] -> Documentar explícitamente que esta fase termina en `AppDataTableAgGrid` y deja la adaptación a `ColDef[]`/filas planas para una fase posterior.

## Migration Plan

1. Crear `dynamicUiTableQuery.types.ts` con `DynamicTableQueryInput`, `RequestMapper<TRequest>` y los tipos públicos del hook.
2. Implementar `dynamicUiTable.service.ts` sobre `clienteApi`, manteniendo la firma genérica y el contrato de respuesta intacto.
3. Implementar `useDynamicUiTableQuery.ts` usando React Query y los adapters de fase 1B para materializar `rows`, `columns`, `total` y estados.
4. Agregar pruebas unitarias del servicio y del hook, incluyendo éxito con datos, éxito con `data = null`, `success=false` y error de transporte.
5. Documentar el patrón en `docs/Components/AppTable/Query.md` con énfasis en request ACL, response fiel al backend y límites de la fase, incluyendo el gap actual respecto al contrato visual de `AppTable`.

Rollback: al ser una capa nueva y transversal, revertir implica eliminar los archivos creados en `AppTable` fase 3 y restaurar la documentación OpenSpec asociada. No hay migraciones persistentes ni cambios de datos.

## Open Questions

- Qué endpoint exacto y método HTTP usará el servicio base para la primera integración real; el diseño asume que la capa puede aceptar una implementación de `queryFn` mientras ese detalle se fija en código.
- Si la metadata `rawResponse` debe incluirse siempre en el retorno del hook o mantenerse opcional para no ampliar innecesariamente la API pública.
- Si el proyecto ya tiene una utilidad común para convertir respuestas `success=false` en instancias `Error`; si existe, la implementación debe reutilizarla para no duplicar criterios.
