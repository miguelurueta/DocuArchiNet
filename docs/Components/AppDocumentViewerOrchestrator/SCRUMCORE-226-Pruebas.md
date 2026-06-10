# SCRUMCORE-226 - Pruebas

Este ticket agrega pruebas unitarias del core del orquestador (adapter + hook).

## Unit tests (core)

- Prioridad `UrlTemporalAbsoluta` y fallback `UrlTemporal`.
- Detección PDF por `ContentType` + fallback por `FileName`.
- PDF consulta firma; no PDF no consulta firma.
- Fallo de resolve / fallo de firma sin perder documento previamente visible.
- Respuestas stale ignoradas.
 - Cancelación de request anterior en visualizaciones consecutivas.

## Cómo correrlas

Ejecutar solo las pruebas del orquestador:

```bash
npm test -- AppDocumentViewerOrchestrator
```

## Evidencias de pruebas ejecutadas

Durante este cambio se ejecutó al menos:

- `npm test -- AppDocumentViewerOrchestrator`

Nota: el reporte detallado queda en la salida del comando en el entorno local/CI.

## Ubicación de tests agregados

- `src/app/Components/UI/AppDocumentViewerOrchestrator/tests/AppDocumentViewerOrchestrator.adapter.test.ts`
  - Valida URL y detección PDF (funciones puras).
- `src/app/Components/UI/AppDocumentViewerOrchestrator/tests/useDocumentViewerOrchestrator.test.tsx`
  - Valida:
    - no PDF => no consulta firma
    - PDF => consulta firma y llena `isElectronicallySigned`
    - resolve falla => mantiene documento previo
    - stale responses ignoradas
    - cancelación (AbortSignal) del request anterior
