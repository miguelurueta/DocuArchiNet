## Context

`AppTable` ya soporta tablas dinámicas y existe un query layer compartido alrededor de `useDynamicUiTableQuery`, pero el estado de consulta todavía puede quedar fragmentado por pantalla. Para la siguiente fase de paginación híbrida y búsqueda server, el frontend necesita una única fuente de verdad para `page`, `pageSize`, `search`, `searchType`, `structuredFilters`, `sortField` y `sortDir`.

El backend de `workflowInboxgestion` ya expone un contrato suficiente para esa consulta unificada: recibe paginación, `Search`, `SearchType`, `StructuredFilters` y sort; además devuelve `Pagination.Total` real. Por eso este cambio no debe inventar un segundo modelo de filtros ni dejar serialización manual por módulo.

La principal restricción es mantener el estado reusable en la capa compartida de `AppTable`, sin acoplarlo a `GestionCorrespondencia` ni a un wrapper visual específico. El `query state` debe servir tanto para futuras tablas server-mode como para integraciones locales que quieran reutilizar reglas de reset y serialización.

## Goals / Non-Goals

**Goals:**
- Centralizar un contrato tipado de `AppTableQueryState` dentro de la infraestructura compartida de `AppTable`.
- Encapsular en helpers puros las reglas de reset de página y actualización del estado.
- Centralizar la serialización hacia el request backend para evitar mappers ad hoc por pantalla.
- Representar completamente el contrato backend real de filtros estructurados, incluyendo operadores como `between`, `isNull` e `isNotNull`.
- Dejar una API reusable consumible por hooks de módulo y wrappers de composición UI.

**Non-Goals:**
- No diseñar todavía la UI final de filtros avanzados ni de toolbar.
- No meter `refresh` dentro del `query state`.
- No acoplar el estado a `GestionCorrespondencia` ni a un endpoint concreto.
- No reemplazar validaciones profundas de backend con lógica funcional en frontend.
- No introducir un modelo paralelo de consulta dentro de componentes visuales.

## Decisions

### 1. El estado de consulta vive en la capa compartida de `AppTable`

`AppTableQueryState` se modelará en `src/app/Components/UI/AppTable`, junto con sus tipos, helpers y hook. La decisión evita que cada módulo vuelva a definir su propia forma de paginación o filtros y alinea el cambio con la arquitectura ya fijada para `AppTable`, `AppTableQueryWrapper` y los futuros modos `client/server/none`.

**Alternativas consideradas**
- Definir el estado solo dentro de `GestionCorrespondencia`: se descarta porque duplicaría lógica al migrar otros módulos.
- Resolverlo únicamente en `useDynamicUiTableQuery`: se descarta porque ese hook quedaría mezclando estado reusable con transporte de datos.

### 2. El contrato de filtros debe ser semánticamente compatible con backend

El estado interno seguirá en camelCase, pero `AppTableStructuredFilter` debe poder representar los operadores y valores que el backend ya soporta. Por eso se adopta un contrato explícito con `value`, `valueFrom` y `valueTo`, en vez de un `unknown[]` genérico.

Esto reduce ambigüedad para filtros `between` y evita transformaciones débiles al serializar el request.

**Alternativas consideradas**
- Usar un contrato libre basado en `Record<string, unknown>`: se descarta porque degrada TypeScript estricto y no representa bien `between`.
- Usar exactamente el DTO backend en frontend: se descarta para mantener una capa de adaptación limpia y no acoplar el naming interno a PascalCase.

### 3. Las reglas de reset se encapsulan en `updateAppTableQueryState`

Los cambios efectivos en `search`, `searchType`, `structuredFilters`, `sortField`, `sortDir` y `pageSize` deben forzar `page = 1`. Esa lógica vivirá en un helper puro compartido, no en la UI ni en hooks de pantalla. El helper comparará por valor efectivo, no solo por referencia, para evitar resets espurios cuando el contenido no cambie realmente.

**Alternativas consideradas**
- Resetear página manualmente desde cada pantalla: se descarta porque rompe consistencia y genera regresiones.
- Hacer que el hook administre reglas implícitas internas sin helper reutilizable: se descarta porque dificulta pruebas unitarias y reutilización.

### 4. La serialización hacia backend se centraliza en un único mapper reusable

`serializeAppTableQueryState` será la única puerta de salida desde el estado compartido hacia el request del backend. El state puede mantenerse en camelCase (`page`, `pageSize`, `search`, etc.), pero el mapper debe producir el shape compatible con el contrato real del endpoint consumidor.

La serialización centralizada evita diferencias entre pantallas sobre nombres de campos, filtros vacíos y operadores estructurados.

**Alternativas consideradas**
- Permitir que cada módulo serialice su request: se descarta porque ya fue identificado como fuente de inconsistencias.
- Acoplar directamente `AppTableQueryState` al payload backend final: se descarta para conservar la separación entre estado reusable y transporte.

### 5. `useAppTableQueryState` será un wrapper ligero sobre los helpers

El hook compartido expondrá el estado inicial y una forma estable de actualizarlo reutilizando `getDefaultAppTableQueryState` y `updateAppTableQueryState`. Su responsabilidad es de ergonomía, no de negocio: no debe ejecutar queries, no debe refrescar backend y no debe guardar estado paralelo fuera del propio `query state`.

**Alternativas consideradas**
- Resolver todo con funciones sin hook: se descarta porque el consumo en componentes React sería más verboso.
- Hacer un hook opinado que también serialice o consulte backend: se descarta para no mezclar capas.

## Risks / Trade-offs

- [Risk] La comparación profunda de `structuredFilters` puede introducir complejidad o costos innecesarios.  
  Mitigation: limitar la comparación al shape del contrato tipado y mantener helpers pequeños y testeables.

- [Risk] El backend evolucione `SearchType` o nuevos operadores de filtros antes de que frontend se actualice.  
  Mitigation: dejar explícito el contrato actual (`1 | 2 | 3` y operadores soportados) y ajustar el mapper como punto único de cambio futuro.

- [Risk] Módulos existentes sigan serializando el request manualmente aunque el estado compartido exista.  
  Mitigation: usar este cambio como base para los tickets siguientes y mover la integración real al flujo reusable en `SCRUMCORE-42` y `SCRUMCORE-43`.

- [Risk] Se termine mezclando lógica de refresh o fetch dentro del `query state`.  
  Mitigation: documentar y probar que `refresh` no pertenece al state y que `onRefresh` no altera la estructura del query state.

## Migration Plan

1. Crear los tipos compartidos `AppTableSearchType`, `AppTableStructuredFilter` y `AppTableQueryState` en la capa `AppTable`.
2. Implementar `getDefaultAppTableQueryState`, `updateAppTableQueryState` y `serializeAppTableQueryState` como helpers puros con pruebas unitarias.
3. Implementar `useAppTableQueryState` como wrapper ligero de consumo React.
4. Ajustar el query layer compartido para poder consumir el state serializado sin introducir serialización duplicada.
5. Validar este cambio de forma aislada antes de que `AppTableQueryWrapper`, los modos de paginación y la integración de búsqueda server se apoyen en él.

Rollback: al ser un cambio de infraestructura compartida no invasiva, puede revertirse retirando los nuevos tipos y helpers mientras los módulos existentes sigan usando su wiring actual.

## Open Questions

- Si `searchType` debe permanecer cerrado a `1 | 2 | 3` en frontend o conviene encapsularlo además en constantes exportadas para evitar números mágicos en módulos consumidores.
- Si la capa de serialización debe devolver un `Record<string, unknown>` genérico o un tipo intermedio más estricto antes del mapper final de cada endpoint.
