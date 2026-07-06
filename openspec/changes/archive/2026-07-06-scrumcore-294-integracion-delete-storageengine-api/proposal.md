# Why

SCRUMCORE-294 convierte el contrato Jira `INTEGRACION-DELETE-STORAGEENGINE-API` en un flujo de producto enterprise para borrar documentos persistidos desde Gestion Respuesta. El objetivo es cerrar el ciclo de borrado con trazabilidad, severidad controlada y sin mostrar detalles tecnicos crudos al usuario.

# What Changes

- Se integra el delete persistido en el `DocumentosWorkbench` sin romper `ver_documento`, la seleccion de filas ni el refresh del listado.
- Se trata `eliminar_item` como la accion funcional existente del workbench y se conserva compatibilidad con filas legacy.
- Se normaliza el contrato de borrado con `idAlmacen`, `nombreGabinete`, `sourceModule=WORKFLOW` para esta pantalla y correlacion por `X-Request-Id` cuando exista.
- Se aplica la precedencia estricta de mensajes: `errors[0].UserMessage` -> `errors[0].Message` -> `message` -> fallback local.
- Se refresca la lista y se limpia el documento activo si la fila eliminada era la seleccionada en el visor.

# Jira Details

## Contract Summary

- Ticket: `SCRUMCORE-294`
- Summary: `INTEGRACION-DELETE-STORAGEENGINE-API`
- Endpoint: `DELETE /api/gestor-documental/eliminar-documento/{idAlmacen:long}`
- Query required: `nombreGabinete`, `sourceModule`
- Source module for this screen: `WORKFLOW`
- Claims required: `defaulalias`, `usuarioid`
- Request id: `X-Request-Id` recommended; backend may generate `meta.requestId`

## Error Semantics

- `errors[0].UserMessage` is the primary user-facing message.
- `errors[0].Message` is secondary fallback only.
- `message` is technical fallback only.
- The UI must not render raw paths, SQL, tokens or stack traces as primary copy.
- `meta.requestId` or `meta.RequestId` must be preserved for support and diagnostics.

## UI Impact

- `DocumentosWorkbench` is the orchestration point for delete from the document rail.
- The delete flow must tolerate backend business, authorization, not found and technical failures without breaking the table state.
- The workbench should use `CanDelete` only as a guardrail when it is present; the backend remains the source of truth.

# Impact

- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`
- `src/modules/gestionCorrespondencia/types/listaDocumentosRadicados.types.ts`
- Tests around delete success, blocked delete, unsupported delete and active-row cleanup
