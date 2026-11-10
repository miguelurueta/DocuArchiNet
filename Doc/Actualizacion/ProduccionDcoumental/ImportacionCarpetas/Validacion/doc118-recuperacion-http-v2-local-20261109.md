# DOC-118 — recuperación HTTP V2, RV-06

Implementada6.6/D-16/RQ-16 tras «continua» posterior a RV-05 y al siguiente paso de integración HTTP. El mapa se amplió antes del código. Progreso23/33:las mismas10 tareas reales/finales anteriores siguen abiertas. RV-07/RV-08 son los dos entregables restantes del plan V2; esta evidencia no los cierra.

El ASMX existente expone POST RecuperarDocumentoV2 con loteId, intentoId, revisionLoteEsperada y revisionArchivoEsperada. Reutiliza EjecutarEntradaOperativa y el guard de Gestión:autoridad vigente, CSRF/origen, formato/límites y JSON cerrado sin coerciones, duplicados o propiedades adicionales. Se usa el cuerpo comprobado, aunque el objeto entregado por ASMX tenga otras referencias. La validación de referencias precede a la fábrica.

La composición real de reconciliación añade una fábrica sin escritor:repositorio, catálogo y guardia de RV-05, autoridad RECONCILIAR y orígenes RA/DA capturados solo en memoria. Comprueba HttpContext y esos orígenes al crear el enlace, antes de resolver identidad/destino y en cada reautorización, incluida la persistencia. No admite pruebas, proveedor, rutas, tokens ni IDs documentales del cliente.

La respuesta es una proyección propia. Un cierre de control responde HTTP200, ok=true, estado RECHAZADO y código EJECUCION_CERRADA_SIN_INVOCACION, cierreSinInvocacion=true y documentoConfirmado=false, sin documento/inventario. El ok indica que se resolvió el control; no afirma documento almacenado ni rechazo de recepción. La rama positiva devuelve DOCUMENTO_CONFIRMADO y únicamente los IDs observados/verificados. No expone evidencia interna, valores, tokens, rutas o mensajes de excepciones inesperadas. Ambas ramas conservan historial sin nuevas escrituras al repetir; se exige autoridad actual también al consultar ese historial.

Verificación ejecutada:

-19 casos HTTP nuevos dentro del arnés .NET Framework existente:contrato de cuatro referencias, POST/sesión, dispatcher productivo, negativo/positivo, cuerpo frente al objeto ASMX, CSRF/origen/método/formato, sesión pública/ausente, permiso revocado, JSON falso/duplicado/coerciones, referencias incompletas, propietario/ámbito/destino, permisos RECONCILIAR/gabinete, revisiones, V1/69/gate, retención activa/otra época, replay con otra reclamación, respuesta perdida, privacidad, RA/DA/HttpContext fijados y capacidad ausente sin fallback.
- Se compilan el dispatcher y la composición productivos; el dispatcher alcanza la fábrica real de recuperación. Solo las fábricas de escritores legacy se sustituyen en ese enlace del arnés. Las ejecuciones exitosas usan coordinador/guardia reales y repositorio/SELECT/transacción simulados. No es una prueba IIS/ASMX alojada ni acredita MySQL real.
-18/18 Node de comportamiento/cliente y205/205 herramientas:223 pruebas disjuntas. Los19 casos nuevos y los102 casos V2 previos están incluidos en el arnés; no se suman al contador Node. Se amplió además la matriz global de revocación a este endpoint.
-12 de14 fuentes históricas siguen idénticas;2 deltas de pruebas autorizados. Los10 informes históricos no cambian. De16 fuentes de evaluación,15 siguen idénticas y permanece el único delta de comando documentado en RV-05.

La operación HTTP se compone en código local, sin despliegue. El almacenamiento habitual conserva V1 y no genera intentos V2 por este cambio. CerrarReservaSinInicio/ReconciliarRecurso mantienen sus contratos. V1/69, gate sin retorno, proveedor no acreditado, reinicio u otro proceso/nodo permanecen bloqueados. No se añade cliente/UI, recuperación automática, reset general, escritor ni backfill.

Sin sesiones, HTTP de negocio, SELECT reales, efectos, DLL/PDB, configuración, permisos, workers, migraciones o limpieza reales. Consumos1276 HTTP/113 SELECT/9 aperturas sin cambios;41/53/69 protegidos. Evidencia:[JSON local](doc118-recuperacion-http-v2-local-20261109.json).
