# DOC-73 — Contratos

- Ticket: DOC-73
- Cambio OpenSpec: doc-73-adaptador-interfaz-integracion-sii
- Clasificacion: cross_cutting

## Contratos e integraciones

Entrada: contexto DOC-72 con tarea y proveedor. Capacidades: `ResolveCapabilitiesResponseDto`, incluido catálogo `DocumentTypes`. Consulta: `QueryItemsResponseDto.Items` de tipo `ExternalItemDto`. La identidad canónica es `INTEGRACIONSII`; cualquier proveedor distinto permanece fuera del adaptador.
