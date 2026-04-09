## Context

`SCRUMCORE-68` formaliza el ticket `03-FE` para conectar la busqueda simple de `GestionCorrespondencia` con el contrato backend `SearchType = 2`, que activa la busqueda global tipo `LIKE` en Workflow Inbox.

La rama base ya contiene parte importante del comportamiento esperado:

- `GestionCorrespondencia` usa `AppInputSearch` como componente presentacional y solo propaga texto a `table.onQueryChange({ search })`.
- `useGestionCorrespondenciaTable` usa `mapGestionCorrespondenciaTableRequest` como `requestMapper`.
- `getAllMatchingRows` y `getBackendExportFile` tambien usan `mapGestionCorrespondenciaTableRequest`, evitando divergencia entre tabla, exportacion y consulta de todos los resultados.
- `mapGestionCorrespondenciaTableRequest` envuelve `mapDynamicUiServerTableRequest` y resuelve `SearchType` de forma especifica para el modulo:
  - preserva `SearchType = 3` para busqueda avanzada;
  - usa `SearchType = 2` cuando `search.trim()` tiene texto efectivo;
  - conserva el valor recibido cuando no hay texto efectivo.
- El mapper compartido de `AppTable` permanece generico.

Por este estado, el cambio debe enfocarse en consolidar y verificar el contrato del mapper del modulo, no en mover logica hacia `AppInputSearch`, la pagina o el hook.

## Goals / Non-Goals

**Goals:**

- Asegurar que la busqueda simple efectiva de `GestionCorrespondencia` llegue al backend con `SearchType = 2`.
- Preservar `SearchType = 3` para busqueda avanzada.
- Evitar `SearchType = 2` cuando `search` esta vacio o contiene solo espacios.
- Mantener la decision de `SearchType` centralizada en `gestionCorrespondenciaTableRequestMapper.ts`.
- Mantener `mapDynamicUiServerTableRequest` como mapper compartido generico.
- Verificar que tabla, `getAllMatchingRows` y exportacion backend usen el mismo mapper del modulo.
- Preservar paginacion, ordenamiento, `IncludeConfig` y filtros estructurados.

**Non-Goals:**

- No modificar `AppInputSearch` para conocer `SearchType`.
- No cambiar el endpoint de listado `POST /api/workflowInboxgestion/inboxgestion`.
- No hardcodear SQL, columnas o reglas de backend en frontend.
- No modificar autorizacion, claims o configuracion JWT.
- No resolver en este ticket el problema backend de sintaxis `LIKE ESCAPE`; ese ajuste pertenece a un ticket BE separado.
- No introducir autocomplete ni endpoint de sugerencias.
- No cambiar contratos publicos de `AppTable`, exportacion o paginacion.

## Decisions

1. Usar `gestion-correspondencia` como capability modificada.

   El proposal generado sugiere una capability nueva derivada del nombre Jira (`conectar-busqueda-simple-gestioncorrespo`), pero el comportamiento pertenece al modulo existente `gestion-correspondencia`. La fase de specs debe corregir el proposal para modificar esa capability y evitar una spec duplicada.

2. Resolver `SearchType` en el mapper del modulo.

   La decision de enviar `SearchType = 2` depende del contrato workflow inbox, no de la UI ni del mapper compartido. Por eso debe vivir en `mapGestionCorrespondenciaTableRequest`, envolviendo `mapDynamicUiServerTableRequest`.

3. Mantener precedencia de busqueda avanzada.

   Si `searchType === 3`, el mapper debe conservar `3` aunque exista texto en `search`. Esto evita romper flujos avanzados que ya expresan una semantica distinta.

4. Definir texto efectivo con `trim()`.

   `SearchType = 2` solo aplica cuando `input.search?.trim()` produce texto. Asi se evita disparar `LIKE` con strings vacios o espacios, y se conserva el contrato actual donde `Search` se omite cuando no hay valor efectivo.

5. Reutilizar el mismo mapper en tabla y exportacion.

   `useGestionCorrespondenciaTable`, `getAllMatchingRows` y `getBackendExportFile` deben pasar por `mapGestionCorrespondenciaTableRequest`. Esto evita que una exportacion o consulta de todos los resultados pierda el filtro textual activo.

6. Tratar la implementacion actual como base valida.

   Dado que el codigo ya muestra la arquitectura esperada, las tareas deben priorizar verificacion, ampliacion de pruebas si falta algun caso y validacion OpenSpec. Los cambios de codigo funcional deben ser minimos y solo si aparece una brecha real.

## Risks / Trade-offs

- Capability duplicada en OpenSpec -> Corregir proposal/specs para usar `gestion-correspondencia`.
- `Search` con texto pero `SearchType` indefinido -> Cubrir el mapper para garantizar `SearchType = 2`.
- Romper busqueda avanzada -> Mantener prueba para `searchType = 3`.
- Activar `LIKE` con texto vacio -> Validar `search.trim().length > 0`.
- Afectar otras tablas -> No tocar `mapDynamicUiServerTableRequest`.
- Divergencia entre tabla y exportacion -> Verificar que `getAllMatchingRows` y `getBackendExportFile` usen el mapper del modulo.
- Error backend de SQL `LIKE` -> Mantener fuera de alcance del frontend y documentarlo como dependencia BE si vuelve a aparecer.
