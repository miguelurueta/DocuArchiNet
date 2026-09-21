# DOC-73 — Servicios y reglas

- Ticket: DOC-73
- Cambio: doc-73-adaptador-interfaz-integracion-sii
- Impacto: cross_cutting

## Servicios y reglas

El adaptador llama `resolveCapabilities` y luego exactamente una vez `queryItems`. El mapper no interpreta `ExternalKey`. Los filtros y páginas no producen red. Solo items no importados con acción IMPORT/IMPORTAR entran en selección. Los errores se traducen a códigos seguros sin registrar contenido sensible.

