# SCRUMCORE-256 / SCRUMCORE-267 - Vista avanzada de miniaturas

## Contexto

El panel de miniaturas del workspace de digitalizacion documental necesita soportar varios modos de visualizacion sin romper seleccion, drag and drop ni reordenamiento.

## Implementacion

- Se agrega una accion `Vista` como dropdown icon-only en la barra del preview, junto a navegacion de pagina y acciones de rotacion.
- El dropdown permite elegir `1x1`, `2x2`, `3x3`, `4x4`, `5x5` y `6x6`.
- El modo activo se expone como `data-view-mode` en la lista de miniaturas y CSS controla las columnas.
- Los botones de miniatura conservan su key por `page.id`, sus handlers de seleccion y los handlers de drag/drop existentes.
- Para lotes mayores a 100 paginas se marca `data-virtualized="true"` y se activa `content-visibility: auto` por miniatura.

## Rendimiento

- No se duplica ni transforma la coleccion `scanner.pages` para cambiar de vista.
- El cambio de modo solo actualiza estado local de presentacion.
- `content-visibility: auto` delega al navegador evitar trabajo de layout/paint fuera de viewport en lotes grandes.

## Validacion

- Pruebas RTL cubren seleccion de modos desde el dropdown, reordenamiento en cuadrícula y activacion del atributo de virtualizacion al superar 100 paginas.
