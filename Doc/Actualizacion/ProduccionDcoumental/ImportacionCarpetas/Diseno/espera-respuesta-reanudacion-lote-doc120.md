# DOC-120 — espera de respuesta y reanudación de lote

Contrato formalizado tras «continua con la implementacion» sobre el mapa3.136–3.158,2026-11-17. Autoriza implementación y validación local; el diagnóstico de5 SELECT del lote50 permanece cerrado y no autoriza levantar su detención ni almacenar sus996 pendientes.

## Espera y secuencia

Recepción multipart y AvanzarLote/fase ALMACENAR usan XHR.timeout=0: no existe corte automático de30 segundos del cliente. La solicitud permanece pendiente hasta onload/onerror/onabort; un fallo de IIS/red sigue siendo posible y no se convierte en éxito. Consultas y demás operaciones conservan su timeout. El100% de bytes representa transporte; todavía debe recibirse el acuse. Hay una sola mutación en vuelo. Pausar espera esa solicitud y no inicia otra; cerrar cancela la espera e invalida callbacks, sin prometer detener o revertir el servidor.

## Control general

`WebServiceImportacionCarpetas.asmx/ReanudarLote`, POST JSON autenticado bajo las guardas operativas existentes y X-Importacion-Carpetas-CSRF. Solicitud: `{solicitud:{loteId,revisionEsperada}}`; IDs decimales y revisión positiva inferior al límite operativo. Sin IDs ficticios de raíz/archivo, tokens, valores de perfil ni instrucciones SQL.

Respuesta exitosa: envelope público existente, datos `{loteId,estado,revision,detencionSolicitada:false}`. La revisión debe ser exactamente la esperada+1. Cualquier respuesta perdida/inválida o lectura posterior fallida conserva la referencia, impide encadenamiento y requiere consulta/nueva decisión. No hay retry automático. El control no recibe PDF, no crea expedientes, no materializa índices ni invoca el almacenamiento.

La capacidad aditiva `IReanudacionLoteImportacionRepository` se implementa en el repositorio existente sin romper consumidores del puerto IControlLoteImportacionRepository. La composición reautoriza propietario, empresa, destino, origen y permisos actuales. El servicio no utiliza la versión publicada actual del perfil: conserva el snapshot del lote confirmado.

## Elegibilidad y transacción

Solo es reanudable un lote DETENIDO con detencion_solicitada=1, sin intentos RESERVADO/EN_CURSO/INCIERTO, actividad o marcadores no resueltos en raíces/archivos. Una inconsistencia exige diagnóstico; el tiempo no acredita liberación. Estados COMPLETADO/FALLIDO no se reanudan. Retomar creación y recuperar un documento siguen siendo operaciones distintas.

Orden de bloqueo: lote, raíces, archivos e intentos, igual que la consolidación existente. Bajo la misma transacción se carga el inventario/plan/evidencia, se compara revisión y se vuelve a evaluar elegibilidad. Se deriva RECIBIENDO/LISTO/EN_CURSO/PARCIAL o resultado final a partir del inventario; no se fuerza EN_CURSO. La consolidación ordinaria mantiene la detención; solamente el cálculo de reanudación validado permite salir de ella. Se conservan las firmas previas de evaluación para consumidores existentes.

Una actualización condicionada a DETENIDO/señal1/revisión esperada establece estado/señal0/revisión+1; cambios de estado agregado de raíces se incluyen en la misma transacción. Se registra REANUDAR_LOTE/DETENCION_LEVANTADA en ra_integracion_evento. El fracaso de cualquier CAS o del evento revierte la transacción; una solicitud antigua jamás levanta una detención nueva. No se modifican estados individuales, recibos, intentos, identidades, valores efectivos ni resultados de documentos. No se agregan tablas o columnas.

## Consulta y cliente

La consulta usa el mismo predicado y expone REANUDAR_LOTE en accionesPermitidas junto con motivoBloqueo cerrado. La pista no sustituye la autoridad transaccional. SOLICITAR_REANUDACION conserva su significado previo para retomar una creación reservada. Servidor anterior sin REANUDAR_LOTE: consulta disponible y explicación de estado, sin prometer continuación.

El botón Reanudar importación constituye consentimiento para ese control y su continuación: una llamada, acuse validado, lectura coherente y luego coordinador existente solo si SOLICITAR_AVANCE sigue disponible. Consulta, exportación, reapertura, recuperación de acuse y carga de página no inician efectos. Respuesta perdida no inicia la cola. Pausa local ofrece Continuar; detención ofrece Reanudar solo con capacidad; actividad/incertidumbre ofrecen consulta/recuperación permitida. Lote/plan/destino/perfil e inventario se conservan y los confirmados se saltan por ID/estado, nunca por posición de fila.

## Medición y entrega

Medir etapas desde las fronteras del importador, con tiempo/categoría/referencia públicos y sin PDF, SQL, valores, tokens o conexiones. Separar la línea base de1/100/1.000 archivos en laboratorio de tiempos reales. La validación global repetida es una hipótesis de costo; una propuesta de optimización debe preservar integridad, permisos, retenciones y revisión antes de cualquier cambio de política.

El despliegue debe incluir DLL y recursos cliente compatibles; publicar solo el cliente no habilita reanudación del servidor anterior. Un retorno al servidor anterior conserva la capacidad de consultar los datos y el historial nuevo, pero deja de ofrecer reanudación general. Ningún despliegue o corrida real se acredita por compilar/probar localmente. Sin cambios a Web.config, escritor compartido, SII, formatos, limpieza o workers.
