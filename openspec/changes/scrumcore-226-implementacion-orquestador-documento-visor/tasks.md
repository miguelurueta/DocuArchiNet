# Tasks - SCRUMCORE-226 (Enterprise)

## Contexto del prompt (qué se entrega)

Este ticket entrega exclusivamente la plataforma reusable `AppDocumentViewerOrchestrator` (sin UI) para que múltiples módulos puedan:

- Resolver visualización documental (`visualizacion/resolve`).
- Resolver URL final del visor (`UrlTemporalAbsoluta` > `UrlTemporal`).
- Detectar si el documento es PDF.
- Consultar firma electrónica solo para PDF (`firma-electronica`) y sin bloquear la visualización.
- Consolidar estado documental runtime consumible por `AppVisorEmbedPdf`.

Responsabilidades explícitas del orquestador:
- resolve documental
- selección URL
- consulta firma
- consolidación de estado runtime

Queda explícitamente fuera (NO):
- permisos UI / toolbar permissions / edición / anotaciones
- `action/ver_documento`
- dependencia de módulos específicos (p.ej. `DocumentosWorkbench`, `AppTreeTable`)
- persistencia de URLs temporales en storage/caches
- cambios de backend/endpoints

## 0. Guardrails (no negociables)

- [ ] 0.1 Confirmar y mantener: NO cambios de backend (código, endpoints, contratos).
- [ ] 0.2 Confirmar y mantener: NO UI / permisos / toolbar / edición / anotaciones dentro del orquestador.
- [ ] 0.3 Confirmar y mantener: NO dependencias a módulos específicos (`DocumentosWorkbench`, `AppTreeTable`, etc.).
- [ ] 0.4 Confirmar y mantener: NO `action/ver_documento`.
- [ ] 0.5 Confirmar y mantener: NO persistencia de URLs temporales (localStorage / sessionStorage / caches persistentes).
- [ ] 0.6 Confirmar y mantener: TypeScript estricto y NO `any`.
- [ ] 0.7 Confirmar “source of truth”: el orquestador recibe `{ documentId, nombreGabinete }` (+ `context?` opcional solo trazabilidad) y no infiere ni reconstruye payloads desde DTOs de UI/rows.

## 1. Refinamiento (antes de publish)

- [ ] 1.1 Confirmar contrato de entrada del core:
  - `{ documentId: number, nombreGabinete: string, context?: { idTareaWorkflow?: number, radicado?: string, grafo?: object } }`
  - `context` se usa solo para trazabilidad (sin lógica funcional obligatoria).
- [ ] 1.2 Confirmar contrato de salida (runtime state) y nombres exactos del prompt:
  - `fileUrl`, `contentType`, `isPdf`, `isElectronicallySigned`, `firmaCheckStatus`, `resolveStatus`, `errors`.
- [ ] 1.3 Confirmar que `AppVisorEmbedPdf` consumirá únicamente el estado consolidado (sin duplicar resolve/firma en consumidores).
- [ ] 1.4 Confirmar la ruta/estructura destino:
  - `src/app/Components/UI/AppDocumentViewerOrchestrator/`
  - `AppDocumentViewerOrchestrator.types.ts`, `.service.ts`, `.adapter.ts`, `useDocumentViewerOrchestrator.ts`, `index.ts`, `tests/`.
- [ ] 1.5 Confirmar cliente HTTP estándar y patrón enterprise para cancelación + timeouts.
- [ ] 1.6 Confirmar contratos backend sin cambios:
  - `POST /api/gestor-documental/documentos/visualizacion/resolve`
  - `GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}`
- [ ] 1.7 Definir semántica enterprise de estados:
  - `resolveStatus`: `idle | loading | resolved | failed | cancelled`
  - `firmaCheckStatus`: `not_required | resolved | failed`
  - Reglas de estabilidad: no perder documento previo en fallas.
- [ ] 1.8 Alinear contrato del hook reusable:
  - `useDocumentViewerOrchestrator()` expone `visualizarDocumento`, `documentoActivo`, `loading`, `error`, `reset`, `cancelCurrentRequest`.

## 2. Implementación (core reusable)

- [x] 2.1 Crear `AppDocumentViewerOrchestrator.types.ts`.
- [x] 2.2 Implementar `AppDocumentViewerOrchestrator.service.ts`.
- [x] 2.3 Implementar `AppDocumentViewerOrchestrator.adapter.ts`.
- [x] 2.4 Implementar `useDocumentViewerOrchestrator.ts`.
- [x] 2.5 Exportar API pública en `index.ts`.

## 3. Integración mínima (sin UI nueva)

- [ ] 3.1 Integrar `AppVisorEmbedPdf` para consumir `documentoActivo`/estado consolidado.
- [ ] 3.2 Validar que no se incorporó lógica de permisos/toolbar en el core.
- [ ] 3.3 Validar que el módulo consumidor es quien obtiene `documentId/nombreGabinete` y cualquier metadata contextual (no el orquestador).

## 4. Pruebas (enterprise)

- [x] 4.1 Unit tests core (contract + comportamiento):
  - [x] URL: prioridad `UrlTemporalAbsoluta`, fallback `UrlTemporal`.
  - [x] Detección PDF por `ContentType` + fallback por `FileName`.
  - [x] PDF => consulta firma; no PDF => `firmaCheckStatus = not_required` y no llama firma.
  - [x] firma falla => `isElectronicallySigned = null` y mantiene visualización.
  - [x] resolve falla => `resolveStatus = failed`, NO consulta firma y mantiene documento previo.
  - [x] cancelación: requests previos cancelados (test explícito).
  - [x] stale responses ignoradas (out-of-order).
- [ ] 4.2 Integración UI (si aplica en el repo):
  - loading perceptible, error visible, documento activo estable (sin flicker).
- [ ] 4.3 E2E Playwright (si hay fixtures/mocks):
  - PDF firmado / no firmado
  - resolve error / firma error
  - clicks rápidos múltiples documentos (concurrencia/cancelación).
- [x] 4.4 Seguridad: verificación de no persistencia (búsqueda/inspección) de `UrlTemporal*` en storage/caches.

## 5. Calidad / Cierre

- [ ] 5.1 Ejecutar `npm test` y `npm run lint` (y `npm run test:e2e` si aplica).
- [x] 5.2 Ejecutar `npm run spec:validate`.
- [x] 5.3 Crear/actualizar documentación obligatoria en `docs/Components/AppDocumentViewerOrchestrator/`:
  - `SCRUMCORE-226-Arquitectura.md`
  - `SCRUMCORE-226-Implementacion-Detallada.md`
  - `SCRUMCORE-226-Integracion-BackEnd.md`
  - `SCRUMCORE-226-Pruebas.md`
  - `SCRUMCORE-226-Metadata.md`

## 6. Checklist de aceptación (del prompt)

- [ ] 6.1 `visualizacion/resolve` funciona correctamente (URL final resuelta, PDF detectado, estado runtime actualizado).
- [ ] 6.2 Solo PDF consulta firma; no PDF marca `firmaCheckStatus = not_required`.
- [ ] 6.3 Documento previo se mantiene ante errores de resolve o firma (sin flicker / sin pérdida).
- [ ] 6.4 URLs temporales NO se persisten (storage/caches).
- [ ] 6.5 Hook reusable desacoplado funcionando (`visualizarDocumento`, `documentoActivo`, `loading`, `error`, `reset`, `cancelCurrentRequest`).
- [ ] 6.6 Tests pasan (unit/integration/e2e según aplique) y calidad OK (lint/build, consola limpia, sin leaks evidentes).
