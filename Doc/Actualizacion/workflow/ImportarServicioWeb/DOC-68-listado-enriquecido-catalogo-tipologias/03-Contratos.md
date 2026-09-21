# Contratos

El envelope conserva `SchemaVersion=1.0`. Los objetos añadidos usan versión 1.1:

- `ImportDocumentTypeDto`: `DocumentTypeId`, `Name`, `Required`, `SortOrder`.
- `ImportItemMetadataDto`: `Code`, `Label`, `Value`.
- `ExternalItemDto`: `PresentationSchemaVersion`, `Metadata`, `ImportStatus`, `AllowedActions`.

Estados: `Disponible`, `Importado`, `ConNovedad`. Acciones cerradas: `Preview`, `Import`, `View`, `Review`.

`QueryItems` considera importable exclusivamente un anexo SII con `tipoanexo=505` (sello de inscripción). Otros anexos del mismo registro, como `518` (soporte de notificación SIPREF), no se publican. La misma validación se repite al resolver el `ExternalKey`, evitando que un cliente omita el filtro mediante una clave fabricada.
