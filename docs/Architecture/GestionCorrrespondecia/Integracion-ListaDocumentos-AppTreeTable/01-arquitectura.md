# SCRUMCORE-295 - Arquitectura

## Resumen

`SCRUMCORE-295` formaliza el listado de documentos radicados en `DocumentosWorkbench`, usando `AppTreeTable` como renderer y el endpoint `POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/query` como fuente de datos.

La implementacion final es una lista completa sin paginacion interactiva. El frontend solicita todos los documentos principales con `EnablePagination=false`, mantiene `DocumentRelationScope=documentsOnly` y filtra localmente sobre el dataset recibido.

## Objetivos

- Mostrar todos los documentos base del radicado sin perder filas por paginacion.
- Evitar mezclar anexos de respuesta en la lista principal.
- Mantener la busqueda funcional sin depender de la semantica backend de `Search`.
- Preservar `AppTreeTable` como renderer reusable.
- Mantener `AppTableQueryWrapper` compatible con consumidores existentes.
- Documentar contrato, flujo, riesgos, validacion y diagramas bajo el SCRUM ID.

## No Objetivos

- No crear pantalla nueva.
- No modificar la ruta del endpoint.
- No mover reglas documentales a `AppTable` ni `AppTreeTable`.
- No inferir anexos por nombre, extension o label.
- No persistir payloads completos con radicado, tokens, contenido documental ni paths fisicos.

## Decisiones Arquitectonicas

| Decision | Justificacion |
|---|---|
| Cargar lista completa con `EnablePagination=false` | Evita que filas queden fuera por pagina y habilita busqueda local confiable. |
| Usar `documentsOnly` como scope base | Mantiene la lista principal sin anexos de respuesta. |
| Ocultar paginacion solo en `DocumentosWorkbench` | El usuario pidio listar todas las filas; otros consumidores conservan paginacion. |
| Agregar `showPagination` a `AppTableQueryWrapper` | Es un cambio compatible: default `true`, opt-out por pantalla. |
| Buscar localmente | Como la UI tiene todas las filas, evita que backend `Search` oculte resultados esperados. |
| Mantener `AppTreeTable` sin reglas de negocio | Protege reusabilidad y evita acoplamiento con radicado/gabinete/scope. |

## Responsabilidades Por Archivo

| Archivo | Responsabilidad |
|---|---|
| `DocumentosWorkbench.tsx` | Composicion del visor, listado, wrapper de busqueda y `AppTreeTable`. |
| `DocumentosWorkbench.module.css` | Layout scoped, input compacto y superficie sin paginacion visible. |
| `useGestionRespuestaDocumentosTable.ts` | Query state, carga completa, filtro local, scope, acciones y totales. |
| `gestionRespuestaDocumentosRequestMapper.ts` | Payload compatible hacia `ListaDocumentosRadicados/query`. |
| `documentosWorkbenchResponseAdapter.ts` | Adaptacion de rows, columns, metadata, pagination y total. |
| `AppTableQueryWrapper.tsx` | Wrapper compartido con `showPagination` compatible hacia atras. |

## Limites Del Cambio

- El hook documental decide `EnablePagination=false`.
- El mapper conserva defaults compatibles para otros posibles usos.
- El servicio sigue siendo transporte; no contiene reglas de UI.
- `AppTreeTable` solo recibe `load`, `loadChildren`, columnas y callbacks.
- `AppTable` no conoce `DocumentRelationScope`, `Radicado`, `NombreGabinete`, anexos, offsets ni limits.

## Riesgos Y Mitigaciones

| Riesgo | Mitigacion |
|---|---|
| Payload grande al traer todo | Aceptado para esta pantalla por requerimiento funcional de listar todo. |
| Backend `Search` no coincide con lo visible | Se envia `Search=""` y se filtra localmente. |
| Otros consumidores pierden paginacion | `showPagination` default `true`; solo el workbench pasa `false`. |
| Totales inconsistentes durante busqueda | Con busqueda activa se usa total filtrado local. |
| Cambio accidental en `AppTreeTable` | No se modifico su logica interna ni contrato de negocio. |
