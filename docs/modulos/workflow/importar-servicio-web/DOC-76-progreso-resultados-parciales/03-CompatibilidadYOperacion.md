# Compatibilidad y operación

`GetImportIntent` se habilita únicamente para `timeout`, pérdida de respuesta (`lost-response`) o reapertura autorizada (`authorized-reopen`). Cada acción realiza una sola consulta; no existe polling.

Los módulos modernos no importan, copian ni invocan `JSProgresBar`, no interpretan `YES`, `CTRL`, `CTRLRETURN` o `dato_lista`, y no modifican almacenamiento ni consumidores legacy. Con el gate apagado, el recorrido legacy permanece intacto.

La espera usa `role="status"`, `aria-live="polite"` y una animación CSS indeterminada decorativa. No tiene porcentaje; `prefers-reduced-motion` desactiva el movimiento. No se ofrecen reintento de fallidos ni cancelación en curso.

La reversión retira los dos módulos, su registro, el panel y estilos aditivos. No hay migraciones de datos.
