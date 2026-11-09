# DOC-118 — coordinación y recuperación

- Ticket: DOC-118
- Cambio OpenSpec: doc-118-produccion-importacion-carga-carpeta-lote-recuperacion
- Clasificación: cross_cutting
- Estado: revisión local V2 consolidada en RV-08;25/35 tareas, aceptación real y calificación del runtime abiertas.

## Servicios y reglas

RV-08 consolida los contratos D-11..D-16:inicio/gate/retorno V2, retención previa, mismo repositorio/transacción, recuperación sin escritor y entrada HTTP cerrada. La rama negativa exige PREPARADO sin invocación y expediente sin huellas; la positiva verifica IDs observados completos. Guardia y comparación de vista hasta commit/rollback; historial autorizado no escribe.133 casos V2 internos y matriz21 condiciones de RV-07, sin calificarlos como MySQL real. La composición habitual sigue V1; no se integra ni despliega en esta revisión.

AvanzarLote despacha una fase por recurso; ReservarIntentoDocumental aplica exclusión por lote con transacción y revisión. HayReclamacionDocumental bloquea reservas RESERVADO/EN_CURSO/INCIERTO y marcadores huérfanos; no ofrece takeover por fecha. AlmacenarArchivo reutiliza historial de la misma solicitud antes de una reserva nueva.

SolicitarDetencion conserva llamadas activas y efectos. RegistrarResultadoDocumento llama a ConsolidarEnTransaccion en su transacción exterior: no se omite ese cierre por leer solo el helper de persistencia. Consolidar conserva el inventario original; actividad e incertidumbre no equivalen a detención completada. Si el último efecto requerido queda positivo puede resultar COMPLETADO con la señal conservada.

CerrarReservaSinInicio exige reserva documental probada sin inicio, vista exacta y ausencia de detención; su cierre RECHAZADO no acredita un documento. ReconciliarRecurso verifica INCIERTO con evidencia conocida sin escritor. EN_CURSO por fecha, cierre con señal de detención o identidad insuficiente no habilitan reparación. No se modifica servicio alguno en esta entrega.
