# Resumen técnico

- Ticket: DOC-55
- Cambio OpenSpec: doc-55-adaptador-sii-asmx
- Clasificacion: cross_cutting

## Objetivo

DOC-55 incorpora `INTEGRACIONSII` como cliente asíncrono explícito del núcleo ImportarServicioWeb. La solución es aditiva: conserva el registro síncrono, los endpoints históricos y la caja negra de almacenamiento.

## Alcance y compatibilidad

El alcance comprende el adaptador SII, el ASMX moderno, la mediación de preview, pruebas sin red y documentación. Las superficies legacy permanecen intactas.

## Componentes

| Componente | Responsabilidad |
| --- | --- |
| `SiiExternalImportProviderClient` | Consumir SII mediante el transporte HTTP común. |
| `SiiImportContractMapper` | Normalizar items, preview y comandos documentales. |
| `SiiImportProvider` | Exponer capacidades, consulta, preview y descarga asíncronos. |
| `SiiPreviewResponseFactory` | Autorizar y sanear los metadatos temporales. |
| `SiiLegacyResultAdapter` | Traducir exclusivamente los códigos históricos. |
| `WebServiceImportarServicioWebModern` | Validar gate/contexto, delegar y serializar. |

## Propiedades de diseño

- Resolución exacta de `INTEGRACIONSII`, sin fallback.
- `Await` de extremo a extremo, sin `.Result`, `.Wait()` ni `GetAwaiter().GetResult()`.
- Preview sin rutas físicas, tokens ni excepciones internas.
- Persistencia coordinada por el orquestador y el puerto común.
- Gate desactivado por defecto y rutas legacy intactas.
