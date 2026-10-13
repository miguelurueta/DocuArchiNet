# Estados, errores y antirregresión

## Estados

```text
idle -> queued -> rendering -> rendered
                 |              |
                 +-> cancelled <-+
```

Una cancelación esperada no se trata como fallo de página. Un error real de `getPage` o render muestra el contenido de error configurado y el trabajador continúa.

El runner produce:

- `PASSED`: todos los hitos y cero errores.
- `PASSED_WITH_OBSERVATIONS`: hitos completos y solo recurso opcional conocido.
- `FAILED`: hito ausente, error requerido o categoría desconocida; código de salida distinto de cero.

## Antirregresión

Se preservan `setZoom`, `loadPages`, `scrollToPage`, `rotate`, IDs DOM, callbacks, página activa, scroll proporcional, capas de anotación, firma e impresión. `script.js` no requirió cambios. El fallback mantiene el comportamiento cuando no existe `IntersectionObserver`.

Rollback: revertir los JavaScript, pruebas y documentación de DOC-100; no existe migración de datos ni configuración.
