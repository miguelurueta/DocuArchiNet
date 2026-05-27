# Tasks - SCRUMCORE-227 (Enterprise, alineado al prompt)

## Mental model (ultra corto)

- 1 handler unificado
- 1 contrato canónico (`DocumentResolveRequest`)
- 1 orquestador (`AppDocumentViewerOrchestrator`)
- 1 visor consumidor (`AppVisorEmbedPdf`)
- 0 lógica duplicada
- concurrencia controlada
- estado estable del documento
- tests E2E (Playwright) obligatorios

## 0. Guardrails (no negociables)

- [x] 0.1 NO cambiar backend / NO cambiar endpoints.
- [x] 0.2 NO duplicar lógica de resolve/firma en `DocumentosWorkbench`.
- [x] 0.3 NO tocar lógica interna de `AppVisorEmbedPdf` (permisos/policy).
- [x] 0.4 NO romper Dynamic UI / `AppTreeTable` / selección múltiple / documento activo.
- [x] 0.5 NO usar `any`.

## 1. Centralizar evento de apertura de documento

- [x] 1.1 Unificar `row_click` y `menu_action` en un único flujo.
- [x] 1.2 Crear una única función handler orquestadora (memoizada) en `DocumentosWorkbench`.
- [x] 1.3 Garantizar 0 lógica duplicada (no “copias” del flujo en handlers separados).

## 2. Resolver contrato canónico de documento (source of truth)

- [x] 2.1 Consumir `DocumentResolveRequest` desde `action/ver_documento`:
  - `IdDocumento`
  - `NombreGabinete`
- [x] 2.2 Validar que `DocumentResolveRequest` es la única fuente de verdad (no usar DTO directo de tabla/row).
- [x] 2.3 Si falla `action/ver_documento` (`success=false` o error) → NO abrir/actualizar visor.

## 3. Integrar AppDocumentViewerOrchestrator (consumo real)

- [x] 3.1 Invocar `visualizarDocumento()` con:
  - `documentId` y `nombreGabinete` (derivados de `DocumentResolveRequest`)
  - `context` runtime (si está disponible): `idTareaWorkflow`, `radicado`, `grafo`
- [x] 3.2 No reconstruir payloads alternos; no inferir valores fuera del contrato canónico.

## 4. Gestionar estado del documento activo (Workbench)

- [x] 4.1 Mantener y actualizar:
  - `activeRowId`
  - `activeFileUrl`
  - `documentContext` (documentId, nombreGabinete, isPdf, isElectronicallySigned, firmaCheckStatus)
- [x] 4.2 Evitar pérdida de estado ante updates (si el nuevo intento falla, conservar el previo).

## 5. Integrar AppVisorEmbedPdf (solo consumo)

- [x] 5.1 Pasar `fileUrl={activeFileUrl}` al visor.
- [x] 5.2 Usar `isPdf` / `isElectronicallySigned` / `firmaCheckStatus` solo como estado runtime del workbench (sin permisos/policy).
- [x] 5.3 Confirmar explícitamente: NO agregar lógica de permisos aquí.

## 6. Manejo de concurrencia

- [x] 6.1 Evitar múltiples visualizaciones simultáneas: delegar cancelación/anti-race al orquestador.
- [x] 6.2 Cancelar requests previos si existe nuevo click/acción (comportamiento observable).
- [x] 6.3 Ignorar respuestas stale (out-of-order): el documento activo no debe “retroceder”.

## 7. Preservación de estabilidad del visor

- [x] 7.1 Si falla resolve/firma → mantener documento anterior (sin flicker).
- [x] 7.2 No resetear UI innecesariamente (no perder scroll/contexto/foco si ya había documento visible).

## 8. Mantener selección múltiple intacta

- [x] 8.1 No modificar estado de selección de tabla.
- [x] 8.2 Separar claramente:
  - selección UI (AppTreeTable)
  - documento activo (visor)

## 9. Manejo de errores controlado

- [x] 9.1 `action/ver_documento` falla → NO llamar `visualizarDocumento()` y mostrar error sin romper UI.
- [x] 9.2 resolve/firma falla → mantener documento previo y mostrar error (sin romper visor).

## 10. Optimización performance

- [x] 10.1 Memoizar handlers (`row_click`, `menu_action`, handler orquestador).
- [x] 10.2 Evitar re-renders completos de tabla/workbench.
- [x] 10.3 Evitar recalcular resolvers innecesarios (dependencias estables).

## 11. Integración limpia con capas existentes

- [x] 11.1 Workbench solo como consumidor del core (sin lógica de negocio de resolve/firma).
- [x] 11.2 No introducir coupling a DTO backend fuera del adapter dedicado.

## 12. Testing obligatorio

- [x] 12.1 Unit tests:
  - handler unificado (row_click y menu_action entran por el mismo flujo)
  - mapping de `DocumentResolveRequest` → input `visualizarDocumento`
- [x] 12.2 Integration tests (UI):
  - flujo row_click → action → orquestador → visor
  - flujo menu_action → action → orquestador → visor
  - selección múltiple no se rompe
- [ ] 12.3 E2E (Playwright) obligatorio:
  - click fila abre documento
  - menú abre documento
  - selección múltiple no se rompe
  - resolve/firma error no pierde documento previo (si hay fixture/mocks para simularlo)

## 13. Validación de contratos

- [x] 13.1 Asegurar que `DocumentResolveRequest` es el único input válido para el orquestador en este flujo.
- [x] 13.2 Confirmar que no se usa DTO directo de tabla para derive de `IdDocumento/NombreGabinete`.
- [x] 13.3 Confirmar que no se rompe el contrato backend (solo consumo).

## 14. Documentación técnica obligatoria

- [x] 14.1 Documentar arquitectura del flujo (convergencia handlers, contratos, estados).
- [x] 14.2 Documentar implementación por capas (hooks/adapters/services/tests).
- [x] 14.3 Documentar integración backend (solo consumo).
- [x] 14.4 Documentar pruebas (incluyendo Playwright) y evidencias de ejecución.
- [x] 14.5 Documentar metadata Jira (ticket/autor/fecha) y archivos tocados.

Ruta obligatoria (según prompt):

- `docs/modulos/gestioncorrespondencia/implenetacionverdocumento`
