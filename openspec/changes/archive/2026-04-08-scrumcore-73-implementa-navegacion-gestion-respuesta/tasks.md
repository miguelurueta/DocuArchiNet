## 1. Contrato reusable de acciones en AppTable

- [x] 1.1 Extender el flujo de acciones de `AppTable` o `AppTableActionCellRenderer` con un callback opcional para `client_event` que entregue `actionId`, `row` y `columnKey`.
- [x] 1.2 Verificar que el nuevo callback no acople el renderer shared a rutas, modulos o llamadas a `navigate`.
- [x] 1.3 Verificar que las acciones `api_call` mantienen su comportamiento actual sin pasar por la nueva ruta de navegacion.

## 2. Navegacion del modulo GestionCorrespondencia

- [x] 2.1 Actualizar `GestionCorrespondencia.tsx` para escuchar la accion contextual relevante de la fila y navegar con `row.id`.
- [x] 2.2 Eliminar el boton `Abrir respuesta contextual` del toolbar.
- [x] 2.3 Validar que la navegacion solo ocurra cuando exista `row.id` valido.

## 3. Routing y shell persistente

- [x] 3.1 Cambiar la ruta del modulo para soportar `respuesta/:id`.
- [x] 3.2 Ajustar `GestionCorrespondenciaRoute` y el flujo de shell para resolver deep links con parametro sin desmontar la bandeja principal.
- [x] 3.3 Mantener `GestionRespuesta` desacoplada del router y sin logica de navegacion propia.

## 4. Pruebas y validacion

- [x] 4.1 Actualizar pruebas de `AppTableActionCellRenderer` para cubrir el callback reusable de `client_event`.
- [x] 4.2 Actualizar pruebas de `GestionCorrespondencia` para verificar que ya no existe el boton de toolbar y que la accion de fila dispara navegacion.
- [x] 4.3 Actualizar pruebas de routing del modulo para `respuesta/:id`, deep link y retorno visible.
- [x] 4.4 Ejecutar pruebas focales de `AppTable` y `gestionCorrespondencia`.
- [x] 4.5 Validar `openspec validate scrumcore-73-implementa-navegacion-gestion-respuesta --strict`.
