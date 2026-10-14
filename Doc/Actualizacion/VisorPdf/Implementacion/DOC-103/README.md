# DOC-103 — Transacción frontend del pegado de grafo

El guardado del grafo ahora es una única transacción esperable. El overlay permanece visible hasta que terminan imagen, POST, validación funcional y refresco de la página afectada. Clics repetidos comparten la operación pendiente y generan como máximo un POST.

## Secuencia

```text
Usuario -> validar overlay/página/imagen
        -> bloquear Guardar/Cerrar
        -> cargar imagen
        -> POST ASMX (30 s, cancelable)
        -> normalizar respuesta
           -> commit: refrescar una página -> retirar overlay
           -> warning: refrescar -> retirar -> aviso fijo
           -> fallo/incierto: conservar overlay -> aviso fijo
        -> finally: liberar single-flight y controles
```

Se preservan ASMX, DTO, iText, almacenamiento, temporales, firma, Wacom, descarga e impresión. El refresco sigue usando el modo `stream` de DOC-102 y no cambia página activa, zoom, scroll ni URL documental.

La matriz local está automatizada bajo `tools/e2e`. La prueba real queda bloqueada hasta autorización específica y PDF descartable; consulte `tools/e2e/AGENT-RUNBOOK.md`.
