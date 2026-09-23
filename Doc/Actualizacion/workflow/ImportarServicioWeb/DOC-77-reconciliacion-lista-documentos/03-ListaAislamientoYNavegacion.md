# Lista, aislamiento y navegación

- Ticket: DOC-77
- Cambio OpenSpec: doc-77-reconciliacion-lista-documentos
- Clasificacion: cross_cutting

## Actualización final

El adaptador recorre `items` una vez. Para cada item exige simultáneamente:

1. Estado `Disponible`.
2. `DocumentId` numérico positivo.
3. `TaskId` igual a la tarea actualmente visible.
4. `DocumentId` no procesado previamente en el lote.

La tarea se lee al sincronizar, no al iniciar la operación. Así, un cambio de selección durante la respuesta no contamina la nueva tarea.

## Compatibilidad visual

El DTO moderno no contiene `dato_lista`; por diseño no se fabrica. Si la integración inyecta una proyección segura, el adaptador puede delegar cada documento confirmado. En la composición productiva actual se usa el fallback autoritativo de recarga completa, que vuelve a renderizar la lista desde servidor.

## Ver documento importado

La vista de resultados crea la acción únicamente para un `DocumentId` positivo. La apertura vuelve a validar que el documento exista en `GridView_list_documento_relacion_wf` y que la selección renderizada pertenezca a la tarea visible. La navegación existente conserva scroll/foco y limpia el estado transitorio mediante el flujo de preview y cierre.

No se crean una segunda lista ni una ruta paralela de autorización.
