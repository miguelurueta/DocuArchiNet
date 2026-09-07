# Autorización e inconsistencias

- Ticket: DOC-54
- Cambio OpenSpec: doc-54-reconciliacion-lista-documetos
- Clasificacion: cross_cutting

La autoridad se reconstruye con el usuario y la tarea del contexto servidor; el navegador no decide propietario, tarea o proveedor. Una intención inexistente y una no autorizada comparten `IMPORT_INTENT_UNAVAILABLE`, evitando enumeración de datos.

Una relación ausente devuelve `DOCUMENT_RELATION_MISSING`; una cardinalidad múltiple, `DOCUMENT_RELATION_DUPLICATED`; y una tarea distinta, `DOCUMENT_TASK_MISMATCH`. Ninguno expone datos del documento como disponible. La correlación persistida se conserva para soporte y los confirmados equivalentes se deduplican por tarea/documento.
