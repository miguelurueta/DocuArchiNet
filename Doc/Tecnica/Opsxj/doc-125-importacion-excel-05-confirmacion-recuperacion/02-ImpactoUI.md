# DOC-125 — Confirmación transaccional y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: implementación y aceptación real5/5 verificadas.

## Superficies UI

La fase entrega backend, no una nueva pantalla o menú. SolicitudConfirmarExcel contiene las referencias y hashes de la carga, con confirmación explícita para REEMPLAZAR. ConsultarResultado recupera estado, conteo y UTC; ConsultarCargaExcel conserva compatibilidad y usa la misma consulta. El bootstrap informa PermiteNuevaCarga/CodigoCapacidad para permitir contexto de consulta aunque los límites actuales cierren nuevas cargas.

## Validacion visual

La interfaz corresponde a fase06. La E2E de servicios reutiliza Web Forms y dos sesiones autenticadas independientes; propietario y cuenta sin permiso se comprueban antes de los efectos. Se recuperaron tres confirmaciones tras agotar el presupuesto del protocolo de prueba, sin repetir cargas. El bloqueo de sesión ASP.NET puede serializar peticiones de un mismo navegador; no se promete progreso en vivo, cancelación al cerrar la pestaña ni pausa por fila. Recuperar por ID antes de un reintento explícito. El protocolo dispone de15min para sus controles SELECT; cada HTTP conserva30s y cero reintentos.
