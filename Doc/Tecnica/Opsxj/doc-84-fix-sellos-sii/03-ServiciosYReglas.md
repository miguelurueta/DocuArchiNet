# FIX-SELLOS-SII

- Ticket: DOC-84
- Cambio OpenSpec: doc-84-fix-sellos-sii
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`LegacyImportDocumentStorageAdapter` crea `ProyeccionDocumentoWorkflowImportacion` únicamente tras almacenamiento exitoso. Los pasos y el orquestador la publican solo con documento confirmado. La reconciliación la conserva únicamente si coinciden capacidad no ENLASE, cliente, identidad externa, documento y tarea, y si todos los campos visibles están completos. Ante una proyección incompleta se conserva el documento importado, se rechaza la inserción parcial y el modal queda abierto.
