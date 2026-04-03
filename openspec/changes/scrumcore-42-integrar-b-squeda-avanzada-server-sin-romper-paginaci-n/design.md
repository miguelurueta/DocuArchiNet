## Context

Después de `SCRUMCORE-39`, `40` y `41`, el frontend ya tiene:

- un `AppTableQueryState` reusable
- un `AppTableQueryWrapper` para la composición UI
- un `AppTable` con `paginationMode`

Lo que todavía falta es conectar la consulta server real para que búsqueda simple, filtros estructurados, sort, `page` y `pageSize` viajen juntos al backend sin que cada pantalla serialice su propio request.

El backend de `workflowInboxgestion` ya está listo para esta fase: acepta `Page`, `PageSize`, `Search`, `SearchType`, `StructuredFilters`, `SortField` y `SortDir`, y devuelve `Pagination.Total` real. Por eso esta fase ya no debe rediseñar el backend; debe usar el contrato existente y centralizar el mapping frontend.

La restricción principal es evitar dos clases de regresión:

- que el frontend mezcle quick filter local con búsqueda server
- que distintos hooks o módulos serialicen requests distintos para la misma consulta

## Goals / Non-Goals

**Goals:**
- Integrar `AppTableQueryState` al flujo real de consulta server.
- Centralizar el mapper desde el state reusable hacia el request backend.
- Garantizar que `search`, `searchType`, `structuredFilters`, `sortField`, `sortDir`, `page` y `pageSize` salgan del mismo estado.
- Mantener `Pagination.Total` como valor de backend, sin recálculo local.
- Dejar la infraestructura lista para que `GestionCorrespondencia` adopte `server mode` en la siguiente fase.

**Non-Goals:**
- No diseñar todavía la UI final de filtros avanzados de dominio.
- No rediseñar backend ni su contrato de request.
- No mezclar quick filter local con server mode.
- No migrar aún otros módulos fuera del primero que ya está planeado.

## Decisions

### 1. `AppTableQueryState` sigue siendo la única fuente de verdad

La búsqueda server, incluyendo búsqueda simple y filtros estructurados, saldrá exclusivamente de `AppTableQueryState`. Ningún hook de módulo ni pantalla podrá volver a mantener copias paralelas de `search`, `structuredFilters` o sort.

Esto alinea la implementación con la arquitectura maestra y con la lógica de reset ya encapsulada en `updateAppTableQueryState`.

**Alternativas consideradas**
- Permitir que cada módulo agregue su propio estado y luego lo fusione: se descarta porque reintroduce duplicación.
- Resolver parte del state en el wrapper visual: se descarta porque el wrapper no debe serializar requests.

### 2. El mapper al request backend debe ser único y explícito

El frontend mantendrá el state interno en camelCase, pero la salida hacia backend debe producir explícitamente:

- `Page`
- `PageSize`
- `Search`
- `SearchType`
- `StructuredFilters`
- `SortField`
- `SortDir`

La decisión evita transformaciones manuales por pantalla y deja un solo punto de cambio si el endpoint evoluciona.

**Alternativas consideradas**
- Seguir usando campos camelCase hasta el servicio HTTP: se descarta porque ya existe un contrato backend concreto.
- Permitir que cada endpoint decida su mapper: se descarta porque abre divergencias.

### 3. `quickFilterText` no participa del request server

Con `paginationMode = "server"`, la búsqueda local del grid debe quedar fuera del request. El total y las filas visibles deben corresponder únicamente a la consulta backend serializada desde `AppTableQueryState`.

**Alternativas consideradas**
- Aplicar quick filter local además del request server: se descarta porque rompe coherencia entre rows y total.

### 4. El query layer dinámico será el punto de integración principal

La integración debe ocurrir en los hooks y mappers compartidos alrededor de `useDynamicUiTableQuery`, no en `AppTableQueryWrapper` ni en una pantalla específica. Esa capa ya conoce:

- request input
- respuesta paginada
- total backend

Por eso es el lugar correcto para hacer que el flujo server consuma el state reusable sin romper contratos existentes.

**Alternativas consideradas**
- Resolver el mapping en el hook del módulo final: se descarta porque lo necesitarán más pantallas.

## Risks / Trade-offs

- [Risk] Romper consumidores actuales de `useDynamicUiTableQuery` al endurecer el request.  
  Mitigation: mantener compatibilidad progresiva con el contrato actual y agregar cobertura específica.

- [Risk] Que `StructuredFilters` llegue con shape correcto pero no se serialice con el naming requerido por backend.  
  Mitigation: centralizar el mapper y probar específicamente el shape final del request.

- [Risk] Mezclar `quickFilterText` con búsqueda server en la primera pantalla integrada.  
  Mitigation: dejar explícito que el request server ignora esa prop y cubrirlo en pruebas.

- [Risk] Introducir validaciones funcionales profundas en frontend.  
  Mitigation: limitarse a validación estructural mínima y delegar whitelist/sanitización al backend.

## Migration Plan

1. Revisar el contrato actual de `useDynamicUiTableQuery` y los request mappers disponibles.
2. Crear o ajustar un helper único que transforme `AppTableQueryState` al request backend real.
3. Integrar ese helper en el query layer compartido, preservando compatibilidad hacia atrás donde haga falta.
4. Cubrir serialización, paginación, sort y filtros estructurados con pruebas de hook/integración.
5. Dejar esta capa lista para que `SCRUMCORE-43` migre `GestionCorrespondencia` sin volver a serializar manualmente.

Rollback: al estar concentrado en el query layer y mappers, puede revertirse sin desmontar `AppTableQueryState` ni `AppTableQueryWrapper`.

## Open Questions

- Si conviene introducir un tipo intermedio explícito para el request backend o si basta con el mapper que devuelva el shape final del endpoint.
- Si algunos filtros adicionales del módulo real, como `category`, deberán integrarse en la siguiente fase encima de este flujo base o mantenerse aparte.
