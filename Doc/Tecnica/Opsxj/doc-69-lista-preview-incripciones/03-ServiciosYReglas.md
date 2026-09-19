# LISTA-PREVIEW-INCRIPCIONES

- Ticket: DOC-69
- Cambio OpenSpec: doc-69-lista-preview-incripciones
- Clasificacion: cross_cutting (Transversal)
## Servicios y reglas

`ImportPreviewDescriptorService` genera Base64URL criptográfico y persiste solo SHA-256. `ImportPreviewDescriptorRepository` crea, consulta, reclama y consume snapshots mediante SQL parametrizado. `ImportPreviewContentService` aplica autoridad, expiración, MIME, tamaño, disposición y nombre seguro. `ImportPreviewCompositionFactory` comparte la composición entre ASMX y handler.

HEAD consulta metadatos sin BLOB, consumo ni SII. GET reclama atómicamente `Disponible -> Reclamado`, transmite por bloques y finaliza `Consumido`; dos GET concurrentes no pueden entregar dos veces. Ausente, alterado, vencido, ajeno o consumido devuelve 404 vacío; una falla interna devuelve 503 sin detalles.
