# Accesibilidad implementada

## Semántica

El contenedor usa `role="dialog"`, `aria-modal="true"` y `aria-labelledby="importar-servicio-web-title"`. El estado usa `role="status" aria-live="polite"`; los resultados son una lista con nombre accesible. El botón cerrar tiene nombre explícito.

## Foco y teclado

- Al abrir, el foco pasa a `importar-servicio-web-close`.
- `Tab` y `Shift+Tab` ciclan entre controles visibles obtenidos por selector y `getClientRects`.
- Si no hay controles, el diálogo recibe foco.
- `Escape`, clic en backdrop o botón cerrar llama `close(control)`.
- Al cerrar, el foco vuelve a `ctw-document-action-service` cuando soporta `focus()`.

## Estados visuales

El CSS proporciona foco de tres píxeles, color específico para errores, scroll en el cuerpo, backdrop y modo móvil a `100dvh`. El estado `ejecutando` anuncia espera global; no incluye barra, porcentaje ni progreso por elemento.

## Límites de validación

Las pruebas comprueban estructura, selectores y funciones. No sustituyen auditoría con lector de pantalla, contraste computado en navegador, zoom, navegación táctil ni comportamiento tras postback real; esos puntos permanecen pendientes de QA autorizada.
