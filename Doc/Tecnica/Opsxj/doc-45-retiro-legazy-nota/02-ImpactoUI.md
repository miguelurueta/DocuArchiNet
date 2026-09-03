# RETIRO-LEGAZY-NOTA — Impacto UI

- Ticket: DOC-45
- Cambio OpenSpec: doc-45-retiro-legazy-nota
- Clasificacion: cross_cutting

## Superficies UI

- Se retiran del Centro de Trabajo los controles y handlers legacy de Notas.
- El acceso moderno visible muestra `Notas N` o `Nueva nota 0`.
- La gestión aparece como modal superpuesto con lista, editor, visor de solo lectura y confirmador auxiliar.
- El modal conserva tamaño y usa scroll interno; los mensajes exitosos son transitorios.
- Notas ajenas no muestran `Editar` ni `Eliminar`.
- Se preservan color y contraste de las acciones Ver documentos, Detalle, Retomar, Asignar y del índice; `Asignar` conserva su estado verde en tareas todavía no tomadas.
- El acceso continúa operativo después de que el `UpdatePanel` reemplace la barra.

## Validacion visual

La validación autorizada confirmó el modal moderno, el visor de solo lectura, la creación desde el estado vacío y el contraste de las acciones de tareas no asignadas.
