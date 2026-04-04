# PROMPT ARQUITECTÓNICO
Corregir parpadeo de `AppTable` en primera paginación `server mode`

## Rol esperado

Arquitecto de software senior y desarrollador frontend React  
(React 19 + TypeScript estricto + arquitectura enterprise)

## Objetivo

Eliminar el parpadeo visible que ocurre en la primera paginación de `GestionCorrespondencia` y dejar la infraestructura shared preparada para que otras implementaciones de `AppTable` en `server mode` no sufran el mismo problema.

## Problema actual

- al cambiar por primera vez de página en `server mode`, la tabla muestra un flash visual
- el problema se reproduce en `GestionCorrespondencia`
- el resto de implementaciones actuales no lo exponen de la misma manera, pero la causa vive en la capa shared

## Causa identificada

La causa principal está en `useDynamicUiTableQuery`.

Durante el primer cambio a una página no cacheada:

- cambia el `queryKey`
- `useQuery` entra en refetch
- mientras no llega `query.data` de la nueva key, el hook entrega fallback vacío
- eso produce temporalmente:
  - `rows = []`
  - `columns = []`
  - `total = 0`
- `AppTable` recibe un estado transitorio vacío y el grid entra en un cambio visual brusco

## Diagnóstico técnico

La corrección principal no debe recaer únicamente sobre el host ni únicamente sobre el renderer.

### Regla clave

- la capa de consulta shared debe conservar los datos previos durante el refetch de una nueva página server
- `AppTable` puede endurecer el render como protección complementaria, pero no sustituye la corrección del data flow

## Alcance

- conservar los datos previos durante refetch en `useDynamicUiTableQuery`
- validar que `rows`, `columns` y `total` no caigan temporalmente a vacío durante la transición
- evaluar endurecimiento adicional de `AppTable` cuando:
  - `loading = true`
  - ya existen filas renderizadas
- dejar pruebas de no regresión

## No alcance

- no rediseñar `AppTable`
- no cambiar backend
- no mezclar este ticket con búsqueda avanzada o filtros de dominio
- no tocar `client mode`

## Archivos esperados

- `src/app/Components/UI/AppTable/hooks/useDynamicUiTableQuery.ts`
- `src/app/Components/UI/AppTable/tests/useDynamicUiTableQuery.test.ts`

Opcional si se implementa endurecimiento visual adicional:

- `src/app/Components/UI/AppTable/AppTable.tsx`
- tests asociados de `AppTable`

## Reglas de implementación

### 1. Query shared

Durante el refetch de una nueva página en `server mode`:

- se deben conservar los datos previos hasta recibir la nueva respuesta
- no se debe entregar fallback vacío si ya existe una respuesta válida anterior

Implementación esperada:

- usar `placeholderData: (previousData) => previousData`
- o una estrategia equivalente que preserve el resultado previo

### 2. Comportamiento esperado durante transición

Mientras llega la nueva página:

- la tabla puede seguir mostrando la página anterior
- el estado `loading` puede mantenerse activo
- `total` debe mantenerse estable
- no debe aparecer un empty state artificial

### 3. Endurecimiento opcional en `AppTable`

Si se decide complementar la corrección en `AppTable`, aplicar esta regla:

- si `loading = true` y ya existen filas visibles:
  - no degradar la experiencia mostrando un overlay agresivo o vaciando la percepción del grid

Esto es complementario y no reemplaza la preservación de datos previos en el hook shared.

## Riesgos a evitar

- corregir solo el renderer y no la capa de datos
- volver a entregar `rows = []` durante refetch
- romper empty state real
- romper first load sin datos
- romper `client mode`
- introducir comportamiento especial acoplado solo a `GestionCorrespondencia`

## Pruebas obligatorias

- conserva filas previas mientras llega una nueva página
- conserva `total` previo durante la transición
- no cae temporalmente a `rows = []` en el primer refetch de una página no cacheada
- al llegar la nueva respuesta:
  - actualiza filas
  - actualiza paginación
- empty state real sigue funcionando
- `GestionCorrespondencia` deja de presentar parpadeo en la primera paginación server

## Criterios de aceptación

- desaparece el parpadeo de la primera paginación en `GestionCorrespondencia`
- la solución queda reusable para otras tablas en `server mode`
- no se rompen empty state, loading state ni `client mode`
- la corrección queda cubierta por pruebas automáticas

## Decisión arquitectónica

La corrección principal debe vivir en la capa shared de query.

Conclusión:

- cambiar solo `AppTable` no garantiza resolver la causa
- preservar datos previos en `useDynamicUiTableQuery` sí corrige el origen del problema
- una mejora adicional en `AppTable` es opcional y complementaria
