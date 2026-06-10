# SCRUM-242 Metadata

## Ticket

- Jira: SCRUMCORE-242
- Titulo: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-INTREGRACION-APIS
- Fecha: 2026-06-10
- Version: FE-04

## Cambio

Se implemento la capa API frontend reutilizable de DigitalizacionDocumental con servicios, hooks, upload temporal, validacion contractual, stale protection y anti doble submit.

## Archivos Principales

- `src/modules/digitalizacion/types/digitalizacionApi.types.ts`
- `src/modules/digitalizacion/services/digitalizacionApiClient.ts`
- `src/modules/digitalizacion/services/*digitalizacion*.api.ts`
- `src/modules/digitalizacion/hooks/useDigitalizacionApiOperation.ts`
- `src/modules/digitalizacion/hooks/useUploadTemporalPdf.ts`
- `src/modules/digitalizacion/tests/digitalizacionApi.services.test.ts`
- `src/modules/digitalizacion/tests/useDigitalizacionApiOperation.test.tsx`

## Referencias

- SCRUMCORE-239: contrato inicial del modulo.
- SCRUMCORE-240: scanner/Dynamsoft adapter.
- SCRUMCORE-241: workbench grafico.
- `docs/Architecture/DigitalizacionDocumental/01-BE-Contratos-API-Digitalizacion.md`.
