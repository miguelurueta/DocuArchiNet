# Estados y errores

- Ticket: DOC-53
- Cambio OpenSpec: doc-53-orquestacion-estados
- Clasificacion: cross_cutting

El recorrido normal es `Creada → Validada → RecursoObtenido → ExpedientePreparado → IndicesActualizados → DocumentoAlmacenado → CacheActualizado → Completada`. Las alternativas son `FallidaAntesDePersistir`, `Parcial`, `ResultadoIncierto`, `RequiereDecision`, `Reconciliada` y `Detenida`.

`VERSION_CONFLICT` impide repetir efectos con una instantánea obsoleta. Un resultado incierto nunca es reintentable automáticamente. Los mensajes externos se reducen a códigos seguros; no se propagan respuestas legacy, secretos ni payloads del proveedor.

Rollback: retirar los cinco componentes modernos y sus entradas de proyecto. No hay DDL DOC-53.
