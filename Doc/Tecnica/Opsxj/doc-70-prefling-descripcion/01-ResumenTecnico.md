# PREFLING-DESCRIPCION

- Ticket: DOC-70
- Cambio OpenSpec: doc-70-prefling-descripcion
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Extender el preflight de importación SII para informar, por cada elemento seleccionado, el destino lógico, la tipología confirmada, los requisitos y los efectos previstos antes de crear una intención. La respuesta permanece segura: no expone identidades físicas ni ejecuta mutaciones o llamadas adicionales a SII.

## Alcance y compatibilidad

- Backend afectado: DTOs y contratos de importación, `ServicioPreflightImportacion`, `ServicioIntencionImportacion`, el constructor del plan, el repositorio de configuración y el servicio ASMX moderno.
- Pruebas afectadas: suites unitarias/estructurales del preflight y el adaptador E2E existente.
- No se modificaron páginas WebForms ni el comportamiento síncrono de `ExecuteImportIntent`.
- La reversa consiste en retirar los campos aditivos y el constructor del plan; el contrato previo permanece compatible para consumidores que ignoran los campos nuevos.
