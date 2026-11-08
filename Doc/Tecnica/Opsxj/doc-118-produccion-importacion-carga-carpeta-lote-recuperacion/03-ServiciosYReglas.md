# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: preflight autenticado PASS; observador SIN_CALIFICAR y revisión de consola pendiente. Progreso16/27, sin efectos documentales.

## Servicios y reglas

AvanzarLote despacha una fase por recurso; ReservarIntentoDocumental aplica exclusión por lote con transacción y revisión. HayReclamacionDocumental bloquea reservas RESERVADO/EN_CURSO/INCIERTO y marcadores huérfanos; no ofrece takeover por fecha. AlmacenarArchivo reutiliza historial de la misma solicitud antes de una reserva nueva.

SolicitarDetencion conserva llamadas activas y efectos. RegistrarResultadoDocumento llama a ConsolidarEnTransaccion en su transacción exterior: no se omite ese cierre por leer solo el helper de persistencia. Consolidar conserva el inventario original; actividad e incertidumbre no equivalen a detención completada. Si el último efecto requerido queda positivo puede resultar COMPLETADO con la señal conservada.

CerrarReservaSinInicio exige reserva documental probada sin inicio, vista exacta y ausencia de detención; su cierre RECHAZADO no acredita un documento. ReconciliarRecurso verifica INCIERTO con evidencia conocida sin escritor. EN_CURSO por fecha, cierre con señal de detención o identidad insuficiente no habilitan reparación. No se modifica servicio alguno en esta entrega.
