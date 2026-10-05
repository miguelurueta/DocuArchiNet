# CORRECCION-INTEGRAL-REGISTRO-RECIBO-SII

- Ticket: DOC-87
- Cambio OpenSpec: doc-87-correccion-integral-registro-recibo-sii
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`Service_registro_tarea_ruta_sii` conserva URL y compatibilidad de entrada, pero adapta a `{recibo,id_tramite,id_actividad}`. `ServicioRegistroTareaRutaSii` valida sesión, permiso, formato y reconsulta SII. La consulta autoritativa conserva recibo y radicado para validar el trámite con `subtipotramite` o, si falta, con `tipotramite`. Los campos derivados del navegador se ignoran.

`MySqlRegistroTareaRutaSiiRepository` resuelve el destino `REGISTROPUBLICO`, parametriza valores, valida el identificador de tabla y serializa por ruta/recibo. Bajo el bloqueo comprueba tanto el outbox como `F_W_E_REGISTROPUBLICO`; así un registro histórico sin evento también retorna `ALREADY_REGISTERED`. La regla no modifica el esquema: consulta la existencia del recibo antes de insertar. Solo un recibo nuevo confirma tarea + outbox en una transacción. `MySqlRelacionRutaSiiGateway` usa la conexión Docuarchi, bloqueo por recibo y nunca sobrescribe una relación incompatible. Las excepciones no cruzan el ASMX.
