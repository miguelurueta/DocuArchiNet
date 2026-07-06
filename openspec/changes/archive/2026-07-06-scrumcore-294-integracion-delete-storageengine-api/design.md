# Context

SCRUMCORE-294 cierra el gap entre el workbench documental de Gestion Respuesta y el contrato de borrado persistido del StorageEngine. El repositorio ya tiene las piezas necesarias para resolverlo sin crear una superficie nueva:

- el rail de documentos en `DocumentosWorkbench`
- la accion genericamente modelada como `eliminar_item`
- `CanDelete` en la metadata de fila
- refresh del listado y estado activo del visor

El refinamiento debe usar esas piezas y no inventar un segundo mecanismo de borrado.

# Goals / Non-Goals

## Goals

- Permitir el borrado de documentos persistidos desde el rail documental.
- Enviar el contrato enterprise con `idAlmacen`, `nombreGabinete`, `sourceModule=WORKFLOW` y correlacion por request id cuando exista.
- Resolver mensajes y severidades con la precedencia y clasificacion del prompt Jira.
- Mantener el listado, el visor y la accion `ver_documento` estables durante y despues del delete.

## Non-Goals

- No tocar el lifecycle de upload temporal.
- No modificar el PDF viewer, la anotacion ni el reemplazo de paginas.
- No cambiar el contrato backend fuera de lo descrito en el ticket.

# Decisions

1. `DocumentosWorkbench` orquesta el delete.
   El rail ya posee acciones por fila, seleccion y estado del visor. Centralizar el delete ahi evita duplicar logica.

2. `eliminar_item` sigue siendo la entrada funcional.
   El workbench no necesita un action id nuevo si el contrato de tabla ya expone `eliminar_item`.

3. `CanDelete` es una señal de UI, no la autoridad final.
   Si existe, debe ocultar o deshabilitar la accion. Si no existe, la UI puede seguir el path legacy y delegar la decision final al backend.

4. El `sourceModule` de esta pantalla es `WORKFLOW`.
   Gestion Respuesta pertenece al contexto workflow y el contrato Jira explicita que `DELETE_WORKFLOW_BLOCKED` depende de ese discriminante.

5. La precedencia de error es estricta.
   `errors[0].UserMessage` -> `errors[0].Message` -> `message` -> fallback local seguro.

6. El éxito debe limpiar el estado activo de forma determinista.
   Si la fila borrada era la activa, el visor debe desanclarse y el listado debe refrescarse.

7. El mapper de acciones se mantiene generico.
   `buildListaDocumentosRadicadosActionRequest` sigue construyendo el payload base de la tabla y no se acopla a un endpoint especifico de borrado.

# Risks / Trade-offs

- `DocumentId`, `IdDocumento` e `idAlmacen` pueden no coincidir en todas las vistas. La implementacion debe mantener la cadena de fallback existente.
- El backend puede responder con un envelope generico. La UI debe tolerar success sin depender de un shape delete-specific.
- Los errores de delete pueden ser de negocio, autorizacion, no encontrado o tecnico. La UI debe mapear severidad, no solo texto.

# Migration Plan

1. Agregar o adaptar el servicio de borrado persistido.
2. Conectar `eliminar_item` del workbench al flujo de delete.
3. Respetar `CanDelete` cuando exista y mantener compatibilidad con filas legacy.
4. Refrescar la lista y limpiar el documento activo si la fila eliminada estaba abierta.
5. Cubrir success, bloqueo de negocio, autorizacion, no encontrado y cleanup del estado activo con pruebas.
6. Validar el cambio con OpenSpec y publicar.
