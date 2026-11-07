# DOC-116 EXP-01: corrección de la referencia de acceso

La consulta web del lote 11 devolvió 403/ACCESO_DENEGADO antes de cualquier preparación. Este lote pertenece al ensayo de repositorio DOC-115 07-A, que utiliza RutaCanonica y SelloDestino sintéticos. ServicioConsultaImportacion.MismoDestino exige ambos valores vigentes, por lo que no sirve como prueba de identidad web. El error fue seleccionarlo en el ejecutor de pruebas.

Se cambia únicamente la referencia de lectura al lote 9, creado por el flujo HTTP de RECIB-03. Primero se abre el nivel 7 y se comparan el contexto persistido y el destino real; después se consulta el lote protegido. No se relajan permisos ni se modifica ningún lote anterior.

Autorización EXP-01 conservada: un plan, un lote, dos PDF recibidos, dos expedientes y cuatro solicitudes de creación/replay. Nombres y presupuestos sin cambios. Todos los intentos anteriores se detuvieron antes de preparar.

Verificación: 83/83 pruebas locales PASS, incluida una regresión que rechaza el contexto sintético antes de la consulta protegida y de toda escritura. La aceptación real EXP-01 sigue pendiente.
