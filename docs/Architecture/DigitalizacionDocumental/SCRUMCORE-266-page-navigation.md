# SCRUMCORE-255 / SCRUMCORE-266 - Navegacion rapida entre paginas

## Contexto

El workspace de digitalizacion documental requiere saltar a una pagina especifica sin recorrer todo el DOM ni forzar re-render de todas las miniaturas.

## Implementacion

- Se agrega un control compacto en la toolbar: `Pagina` + entrada numerica + accion `Ir`.
- `Ctrl+G` enfoca el campo de pagina para navegacion por teclado.
- La navegacion resuelve la pagina por indice desde `scanner.pages`, selecciona su `page.id` y reutiliza el render existente del preview.
- El scroll automatico usa un `Map<pageId, HTMLButtonElement>` mantenido por refs de React, evitando busquedas globales sobre el DOM.
- El highlight temporal se controla con estado `highlightedPageId` y un atributo `data-highlighted`, limpiado por timeout.

## Rendimiento

- No se ejecutan consultas globales como `querySelectorAll` para localizar miniaturas.
- No se duplica la coleccion de paginas ni se reordena el estado para navegar.
- La seleccion cambia solo el `selectedPageId` y el `highlightedPageId`; las miniaturas conservan keys estables por `page.id`.

## Validacion

- Pruebas RTL cubren navegacion por numero de pagina, seleccion, scroll por ref, highlight temporal y foco con `Ctrl+G`.
