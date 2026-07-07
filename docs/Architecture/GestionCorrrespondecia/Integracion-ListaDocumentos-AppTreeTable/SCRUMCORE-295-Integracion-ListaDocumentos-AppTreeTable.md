# SCRUMCORE-295 - Integracion Lista Documentos AppTreeTable

## Objetivo

Documentar la integracion enterprise del listado de documentos radicados en `DocumentosWorkbench` y `AppTreeTable`, alineada al contrato del backend para scope, paginacion y totales.

## Alcance

- El listado base del workbench carga con `DocumentRelationScope=documentsOnly`.
- La carga inicial y la expansion normal de arbol usan `EnablePagination=true`.
- La recarga completa posterior a una mutacion usa `EnablePagination=false` de forma explicita.
- El frontend consume `meta.total` y `data.pagination.total` como fuente de verdad.
- El mensaje de restriccion de delete se expone solo como toast temporal.

## Contrato De Request

Campos relevantes:

- `DocumentRelationScope`
- `EnablePagination`
- `Page`
- `PageSize`
- `NombreGabinete`
- `CampoRadicado`
- `Radicado`

Reglas:

- `DocumentRelationScope` omitido equivale a `documentsOnly`.
- `EnablePagination=true` es el comportamiento por defecto para la lista principal.
- `EnablePagination=false` se reserva para refresh completo cuando el flujo necesita conservar el universo completo de filas.
- El backend valida scope y paginacion; el frontend no recalcula totales ni decide el universo.

## Flujo UI

1. Carga inicial.
   - `EnablePagination=true`
   - `Page=1`
   - `PageSize=25`

2. Refresh despues de mutacion.
   - `EnablePagination=false`
   - Se usa cuando el flujo requiere volver a consultar el conjunto completo.

3. Cambio de pagina.
   - Se conserva el contexto.
   - Solo cambia `Page`.

## Totales Y Paginacion

- Prioridad: `meta.total`.
- Fallback: `data.pagination.total`.
- Ultimo fallback: `rows.length` solo si el backend no informa total.

## Validacion Y Error Handling

- Errores de validacion no deben reintentar con otro scope.
- El delete deshabilitado por backend se comunica con `toast.warning`.
- No se mantiene alerta inline persistente para esta restriccion.

## Compatibilidad

- El cambio no altera `AppTable` ni `AppTreeTable` como primitives globales.
- El alcance queda localizado en `gestionCorrespondencia`.
- Consumidores legacy que no envian scope continúan en `documentsOnly`.

## Evidencia Tecnica

- Request mapper: `src/modules/gestionCorrespondencia/adapters/gestionRespuestaDocumentosRequestMapper.ts`
- Workbench hook: `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- Tree hook: `src/modules/gestionCorrespondencia/hooks/useListaDocumentosRadicadosTreeTable.ts`
- Response adapter: `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`

