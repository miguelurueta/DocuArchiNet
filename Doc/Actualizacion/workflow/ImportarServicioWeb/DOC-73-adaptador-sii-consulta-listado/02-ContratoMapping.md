# Contrato y mapping

| Origen | Destino |
|---|---|
| `ExternalItemDto.ExternalKey` | `externalKey` opaco |
| `DisplayName` | `displayName` |
| `ContentType` | `contentType` |
| `ImportStatus` | estado normalizado |
| `AllowedActions` | cálculo de importabilidad |
| metadatos BOOK/LIBRO | libro |
| INSCRIPTION/INSCRIPCION | inscripción |
| DATE/FECHA | fecha |
| NATURE/NATURALEZA/ACT/ACTO | naturaleza o acto |
| NEWS/NOTICIA | noticia |
| REFERENCE/REFERENCIA | referencia |

Una respuesta sin `Items`, o un item sin identidad/título, produce `SII_QUERY_RESPONSE_INVALID`. Nunca se descompone `ExternalKey`.

