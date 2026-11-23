# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación local verificada, aceptación real abierta.

## Superficies UI

La fase entrega backend, no una nueva pantalla o menú. SolicitudConfirmarExcel contiene las referencias y hashes de la carga, con confirmación explícita para REEMPLAZAR. ConsultarResultado recupera estado, conteo y UTC; ConsultarCargaExcel conserva compatibilidad y usa la misma consulta. El bootstrap informa PermiteNuevaCarga/CodigoCapacidad para permitir contexto de consulta aunque los límites actuales cierren nuevas cargas.

## Validacion visual

No aplica una validación visual nueva: la interfaz corresponde a fase06. La E2E de servicios usa la suite existente y dos sesiones independientes; su código está preparado, sin autenticación real en esta entrega. El bloqueo de sesión ASP.NET puede serializar peticiones de un mismo navegador; no se promete progreso en vivo, cancelación al cerrar la pestaña ni pausa por fila. Recuperar por ID antes de un reintento explícito.
