# Flujo de integración

1. El usuario abre el documento mediante el flujo WebForms existente.
2. PDF.js entrega el documento y `forceViewerInitialization` crea los esqueletos de página.
3. El observer informa páginas intersectadas; se agregan las adyacentes configuradas.
4. `loadPages` crea como máximo un trabajo por página y generación.
5. `_loadingTask` obtiene y renderiza una página a la vez.
6. `_renderPage` instala el canvas solo si la generación y necesidad siguen vigentes.
7. Scroll agrupa eventos; zoom o rotación invalidan trabajo viejo y preservan el scroll proporcional.
8. El runner mide el recorrido, clasifica eventos sin datos libres y deriva el resultado.

No hay requests, responses, batch ni escritura backend nuevos. La apertura conserva el registro normal ya documentado; DOC-100 no agrega mutaciones.
