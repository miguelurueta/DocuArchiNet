## Context

`SCRUMCORE-67` formaliza el ticket `02-FE` para integrar `AppInputSearch` dentro de `AppToolbar.actionContent` en `GestionCorrespondencia`, usando el estado de consulta existente de la tabla y manteniendo deshabilitado el buscador propio de `AppTableQueryWrapper`.

La rama base ya contiene la implementacion funcional proveniente de `SCRUMCORE-66`:

- `GestionCorrespondencia.tsx` renderiza `AppInputSearch` dentro de `AppToolbar.actionContent`.
- El valor del buscador viene de `table.queryState.search`.
- El cambio de texto llama a `table.onQueryChange({ search })`.
- `AppTableQueryWrapper` se mantiene con `showSearch={false}` para evitar buscadores duplicados.
- `GestionCorrespondencia.module.css` contiene `toolbarSearch` como clase local de layout.
- Las pruebas de `GestionCorrespondencia` verifican un unico buscador visible con el nombre accesible `Buscar tareas workflow`, que el buscador del wrapper no se renderiza y que escribir actualiza `onQueryChange`.

Por ese motivo, este cambio debe tratarse como una consolidacion y verificacion de la integracion, no como una nueva implementacion paralela.

## Goals / Non-Goals

**Goals:**

- Validar que `GestionCorrespondencia` conserve un unico buscador visible dentro del `AppToolbar`.
- Mantener `AppInputSearch` conectado exclusivamente a `table.queryState.search` y `table.onQueryChange`.
- Mantener `AppTableQueryWrapper` con `showSearch={false}`.
- Preservar acciones existentes del toolbar, exportacion y paginacion.
- Mantener `toolbarSearch` como estilo local limitado a layout, ancho, flex y separacion.
- Alinear los artefactos OpenSpec con la capability real `gestion-correspondencia`.

**Non-Goals:**

- No modificar el contrato core de `AppInputSearch`; ese trabajo ya corresponde a la capability `app-input-search`.
- No implementar autocomplete ni consumo de endpoints desde la pantalla.
- No mover logica de request, backend o `SearchType` a `GestionCorrespondencia.tsx`.
- No cambiar contratos de `AppTable`, exportacion, seleccion o paginacion.
- No introducir estilos globales ni selectores de Ant Design fuera del CSS module del modulo.
- No corregir el problema backend de sintaxis SQL `LIKE`; ese ajuste pertenece a un ticket BE separado.

## Decisions

1. Usar `gestion-correspondencia` como capability modificada.

   El proposal generado inicialmente sugiere una capability nueva `integracion-appinputsearch-apptoolbar`, pero el comportamiento pertenece al modulo existente `gestion-correspondencia`. La fase de specs debe corregir el delta para modificar esa capability y evitar duplicar especificaciones funcionales.

2. Mantener la pantalla como capa de composicion.

   `GestionCorrespondencia.tsx` solo debe componer `AppToolbar`, `AppInputSearch` y `AppTableQueryWrapper`. La pagina no debe importar servicios, endpoints ni construir requests. La salida del buscador se limita a `table.onQueryChange({ search })`.

3. Evitar buscadores duplicados.

   El buscador del toolbar es el unico buscador visible para la pantalla. `AppTableQueryWrapper` permanece con `showSearch={false}` para evitar dos inputs conectados al mismo estado y para preservar una experiencia consistente.

4. Mantener estilos locales de layout.

   `toolbarSearch` puede controlar ancho, flex y comportamiento responsive, pero no debe alterar foco, semantica, estados o accesibilidad internos de `AppInputSearch`.

5. Enfocar implementacion futura en verificacion.

   Dado que el comportamiento ya existe, las tareas deben priorizar inspeccion, ajustes minimos si aparece alguna brecha, pruebas focales y validacion OpenSpec. No se debe reescribir el componente ni duplicar wiring.

## Risks / Trade-offs

- Capability duplicada en OpenSpec -> Corregir proposal/specs para usar `gestion-correspondencia` en lugar de crear `integracion-appinputsearch-apptoolbar`.
- Reimplementar un comportamiento ya mergeado -> Tratar el ticket como consolidacion y revisar antes de editar codigo.
- Dos buscadores visibles en la pantalla -> Mantener `showSearch={false}` en `AppTableQueryWrapper` y cubrirlo con pruebas.
- Acoplar la pagina a backend -> Restringir `GestionCorrespondencia.tsx` a `queryState` y `onQueryChange`.
- Romper acciones existentes del toolbar -> Mantener refresh, respuesta contextual, exportacion y paginacion sin cambios de contrato.
- Confundir el error backend `LIKE` con este ticket frontend -> Mantener ese ajuste fuera de alcance y documentarlo como dependencia BE separada si vuelve a aparecer.
