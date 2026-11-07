# DOC-117 — 03-ServiciosYReglas

- Ticket: DOC-117
- Cambio OpenSpec: doc-117-produccion-importacion-carga-carpeta-almacenamiento
- Clasificacion: cross_cutting

## Servicios y reglas

El comando documental fija contexto, expediente, archivo, valores efectivos, intento y SHA de solicitud; el adaptador verifica staging y crea una copia consumible sin perder el original de importación. El escritor compartido se invoca una vez por recurso. Los IDs documento/inventario y la incorporación deben concordar antes de confirmar.

RegistrarResultadoDocumento ejecuta PersistirResultadoDocumento y posteriormente ConsolidarEnTransaccion dentro de su transacción exterior: el cierre completo deja loteCOMPLETADO y raícesCOMPLETADA. Exigir EN_CURSO después de confirmar ambos archivos era un error del control de laboratorio, corregido en el verificador final independiente, sin reparar datos ni invocar otra operación.

ALM-02 conserva intento53 EN_CURSO con resultado público INCIERTO; eso no habilita replay, reconciliación o reparación. Las guardas canónicas nuevas fijan la representación exacta del catálogo y mantienen igualdad de argumentos entre preparación/entrada. La observación interna solo pertenece a una copia de laboratorio autorizada, no al producto ordinario.
