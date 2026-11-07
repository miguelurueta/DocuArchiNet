# DOC-116 EXP-02: variante de resultado descartado en cliente

Estado: composición local lista; ejecución real no autorizada todavía. La autorización anterior se limita a EXP-01. Se reutilizan preparación, cliente DOC-111, autenticación, consola privada, consultas SELECT y ejecutor de creación DOC-116. No se duplica el escritor productivo.

Recursos propuestos: nivel 7, versión 38, gabinete 10, contexto 141/2, mismo ambiente y certificado de pruebas. Nombres DOC116-20261105-EXP02-01/02. Un plan nuevo, una confirmación/un lote, dos PDF de 677 bytes en staging. Un expediente nuevo para la primera raíz; segunda raíz sin crear. Dos solicitudes de creación: primera confirmación seguida del descarte de resultado en cliente y replay idéntico tras verificar IDs/checkpoint por SELECT. Cero documentos definitivos, cambios de permisos, perfiles o configuración. Conservar todos los recursos anteriores, incluido lote 16 y expedientes 657/658. Ventana sin otras importaciones/creaciones sobre niveles observados.

Salida obligatoria: caseComplete=false, internalFailure=NO_EJECUTADO, responseObservation=CLIENT_RESULT_DISCARDED. Esta variante no acredita caída del servidor, pérdida de respuesta HTTP ni fallo postlegacy/precheckpoint. El caso EXP-02 completo sigue pendiente de composición interna autenticada.

Se reutilizan los archivos inicialmente denominados doc116-exp01-setup.cjs, doc116-exp01-authorized.spec.cjs y run-doc116-exp01-authorized.cjs, ahora con selección cerrada EXP-01/EXP-02, presupuesto independiente y filtro exacto por escenario. Conservadas las referencias históricas de EXP-01. La comprobación de identidad usa por lectura el lote HTTP 16 y compara destino vigente antes de consultar.

Validación local: 85/85 PASS; descubrimiento aislado de ambas pruebas sin ejecución real; presupuesto ampliado de EXP-02 rechazado. Se mantienen bloqueo ante incertidumbre y registro de IDs antes de otras verificaciones.

Propuesta exacta: doc116-exp02-recursos-propuestos-20261105.json. No se ha abierto consola ni realizado login/SQL/escrituras reales para esta variante.
