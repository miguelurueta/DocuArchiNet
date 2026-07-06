# SCRUMCORE-294 - Metadata

- Ticket: `SCRUMCORE-294`
- Nombre: `INTEGRACION-DELETE-STORAGEENGINE-API`
- Tipo: Integracion frontend / contrato delete persistido
- Fecha: `2026-07-06`
- Modulo: `Gestion Correspondencia`
- Submodulo: `Gestion Respuesta`
- Alcance: `Frontend`
- Estado: `Refinamiento listo para implementacion`
- Endpoint principal: `DELETE /api/gestor-documental/eliminar-documento/{idAlmacen:long}`

## Archivos impactados

- `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchActionMapper.ts`
- `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts`
- `src/modules/gestionCorrespondencia/services/listaDocumentosRadicados.service.ts`
- `src/modules/gestionCorrespondencia/types/listaDocumentosRadicados.types.ts`
- Tests focales de workbench, mapping y delete

## Decisiones

- La accion funcional de borrado entra por `eliminar_item`.
- `DocumentosWorkbench` orquesta el flujo y mantiene el estado del visor.
- `CanDelete` es guardrail visual, no autoridad final.
- `sourceModule` esperado para esta pantalla: `WORKFLOW`.
- La precedencia de errores es estricta: `UserMessage -> Message -> message -> fallback local`.
- `meta.requestId` o `meta.RequestId` se conserva para soporte y trazabilidad.

## Estado esperado

- Implementacion: servicio delete, conexion con accion de fila, refresh del listado y cleanup del documento activo.
- Documentacion: metadata, arquitectura, contrato API, implementacion detallada, pruebas y evidencias.
- QA: validar success, business block, forbidden, not found y error tecnico.

## Riesgos

- El backend puede exponer `CanDelete` de forma parcial o en algunas vistas no exponerlo.
- `DocumentId`, `IdDocumento` e `idAlmacen` pueden divergir entre filas legacy y contractuales.
- El envelope puede llegar generico; el frontend no debe depender de un shape delete-specific para mostrar la UX.
