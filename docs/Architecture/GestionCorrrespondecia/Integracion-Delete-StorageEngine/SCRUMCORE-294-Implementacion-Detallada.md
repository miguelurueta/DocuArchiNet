# SCRUMCORE-294 - Implementacion Detallada

## Cambios por archivo

### `DocumentosWorkbench.tsx`

- Consume la accion `eliminar_item`.
- Limpia `activeRowId` y `activeFileUrl` si se borra el documento activo.
- Refresca el listado tras exito.
- Mantiene el flujo `ver_documento` sin regresiones.

### `useGestionRespuestaDocumentosTable.ts`

- Mantiene el action request generico para la tabla.
- Resuelve `idDocumento`, `DocumentId` y `NombreGabinete` desde la fila.
- Conserva compatibilidad con filas legacy donde falte metadata completa.

### `documentosWorkbenchActionMapper.ts`

- Continúa construyendo el request base de la tabla para `eliminar_item`.
- No acopla el mapper a un endpoint nuevo fuera del contrato de la tabla.

### `listaDocumentosRadicados.types.ts`

- Conserva `CanDelete` como metadata opcional.
- Mantiene `ActionId` compatible con `eliminar_item`.

## Contrato operacional

```text
Eliminar documento persistido
  -> actionId: eliminar_item
  -> documentId/idAlmacen resuelto desde la fila
  -> nombreGabinete obligatorio
  -> sourceModule=WORKFLOW
  -> requestId correlacionable
```

## Reglas de UX

- Si `CanDelete=false`, la accion no debe estar disponible visualmente.
- Si el backend responde error de negocio, mostrar `UserMessage`.
- Si el documento activo es eliminado, limpiar el visor antes o al refrescar la tabla.
- No mostrar texto tecnico crudo como mensaje principal.

## Compatibilidad

- Si una fila legacy no trae `CanDelete`, la accion sigue existiendo como fallback contractualmente valido.
- Si el backend responde un envelope generico, la UI no debe romper.
- La tabla y el visor deben seguir operando aunque el delete falle.

## Nota de implementacion

La accion de eliminar aparece dentro de `AppTreeTable` como accion de fila. Este cambio no introduce un boton global nuevo.
