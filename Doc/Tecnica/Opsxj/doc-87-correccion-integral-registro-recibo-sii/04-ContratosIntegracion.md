# CORRECCION-INTEGRAL-REGISTRO-RECIBO-SII

- Ticket: DOC-87
- Cambio OpenSpec: doc-87-correccion-integral-registro-recibo-sii
- Clasificacion: cross_cutting (Transversal)
## Contratos e integraciones

- Endpoint: `WebServiceWorkflow.asmx/Service_registro_tarea_ruta_sii`.
- Entrada canónica: `parameter.recibo`, `parameter.id_tramite`, `parameter.id_actividad`; el arreglo legacy solo se acepta en el adaptador.
- Contexto SII autoritativo: conserva `Class_parram_consultarRecibo` y `Class_parram_consultarRadicado`; el tipo efectivo prioriza `subtipotramite` y usa `tipotramite` como respaldo.
- Salidas de alta: `YES` o `REGISTERED_RELATION_PENDING`. Duplicados: `ALREADY_REGISTERED` o `ALREADY_REGISTERED_RELATION_PENDING`; nunca se presentan como alta exitosa.
- Autenticación: usuario derivado de sesión y permiso `UTIL_SII_REGISTRO_TAREA_RUTA` consultado server-side.
- Esquema: únicamente `workflow_registro_ruta_sii_outbox`. `F_W_E_REGISTROPUBLICO` conserva su esquema y se consulta parametrizadamente antes del registro.
- Consistencia: tarea + outbox atómicos; relación Docuarchi eventual e idempotente. Reintentar el recibo no duplica tarea.
