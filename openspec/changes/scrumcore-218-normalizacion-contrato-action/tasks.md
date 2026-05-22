## 1. Normalizacion de respuesta en adapter

- [x] 1.1 Ajustar `pickDynamicUiTable` en `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.ts` para soportar `data.Config` y `data` directo como `DynamicUiTableDto`.
- [x] 1.2 Mantener la inferencia de columnas existente y validar que no se rompa el mapeo actual de `flatDocuments`.
- [x] 1.3 Preservar acciones de fila priorizando `CellActions` + `MenuActions` y usando `RowActions` como fallback legacy.

## 2. Ajustes del hook y contrato de action

- [x] 2.1 Actualizar `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentosTable.ts` para sincronizar `tableIdRef` con `model.tableId` cuando venga en respuesta.
- [x] 2.2 Mantener fallback `InboxListaDocumentosRadicado` cuando la respuesta no provea `tableId`.
- [x] 2.3 Verificar que `load()` deje `tableColumns` y `columns` listos para que `AppTreeTable` renderice acciones.
- [x] 2.4 Normalizar request de `action` para enviar `TableId` efectivo + `RowId` + payload de documento con prioridad `IdDocumento` sobre `DocumentId` y conservacion de `NombreGabinete`.
- [x] 2.5 Confirmar que `query` mantenga `IncludeConfig: true` en el root del request.

## 3. Cobertura de pruebas y evidencia

- [x] 3.1 Ajustar `src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts` para cubrir shape `data.Config` y shape `data` directo.
- [x] 3.2 Agregar caso de preservacion de columna principal y acciones en `flatDocuments` (incluyendo `CellActions`/`MenuActions` con fallback `RowActions`).
- [x] 3.3 Ejecutar `npm.cmd test -- src/modules/gestionCorrespondencia/adapters/documentosWorkbenchResponseAdapter.test.ts` y documentar resultado (pass/fail).
- [x] 3.4 Preparar resumen de entrega con diff de archivos, explicacion de disparo correcto de `onActionTriggered` y confirmacion explicita de no cambios en backend/rutas.
