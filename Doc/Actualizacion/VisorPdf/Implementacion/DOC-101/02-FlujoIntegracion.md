# Flujo de integración

## Lectura

1. WebForms abre el iframe oficial.
2. El navegador carga el núcleo y configura `workerSrc`.
3. `PDFjsViewer.loadDocument` usa el planificador DOC-100 y produce el primer canvas.
4. No se solicitan grafo, Wacom, impresión, Fabric ni Prettify.

## Primer uso de una capacidad

1. El handler comprueba primero el permiso cuando aplica.
2. Deshabilita temporalmente el botón que inició la operación.
3. Solicita la capacidad al cargador memoizado.
4. El cargador agrega estilos y scripts en orden y comprueba el símbolo final.
5. La acción original continúa; solicitudes concurrentes comparten la misma promesa.
6. El control se habilita de nuevo tanto en éxito como en fallo.

El primer clic derecho sobre un canvas carga grafo, obtiene la configuración de sello, registra los menús de los canvases existentes y repite el evento. Los canvases posteriores usan el registro legacy al renderizarse.

La impresión y Wacom conservan una segunda verificación dentro de `PDFjsViewer`, de modo que una llamada directa fuera de la toolbar también es segura. Ningún flujo nuevo escribe documento, estado de tarea o auditoría.
