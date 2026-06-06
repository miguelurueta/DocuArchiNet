# SCRUMCORE-231 - Pruebas

## 1) Estrategia de pruebas
- Validar remount por parsedId en la ruta de detalle.
- Validar que no persistan estados locales entre tareas.
- Mantener hardening de lifecycle sin tocar contratos funcionales.
- Ejecutar regresion trasversal en lo posible en esta iteracion, y dejar trazabilidad de lo no ejecutable por entorno.

## 2) Pruebas unitarias ejecutadas
- `vitest` en `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
  - Remount de detalle al cambiar `:id`.
  - Reinicio de estado local de detalle entre tareas.
  - Navegacion rapida consecutiva `924 -> 925 -> 926`.
  - Casos de bloqueo/carga/empty por estado de estructura.
  - **Estado:** Ejecutada y exitosa (1 archivo, 15 tests, 0 fallos).
- `vitest` en `src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx` (3 tests, 3 passed)
- `vitest` en `src/modules/gestionCorrespondencia/tests/DocumentosWorkbench.test.tsx` (6 tests, 6 passed)
- `vitest` en `src/modules/gestionCorrespondencia/tests/useGestionRespuestaDocumentos.test.tsx`
- `vitest` en `src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx`
- `vitest` en `src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx`
- `vitest` en `src/modules/gestionCorrespondencia/tests/gestionCorrespondenciaTableRequestMapper.test.ts`
- `vitest` en `src/modules/gestionCorrespondencia/tests/solicitaEstructuraRespuestaIdTarea.service.test.ts`
- `vitest` en `src/modules/gestionCorrespondencia/tests/workflowInboxAutocomplete.service.test.ts`

## 3) Pruebas de lint ejecutadas
- `npx eslint src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
- `npx eslint src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondenciaRoutePage.test.tsx`
- **Estado:** Ejecutadas y exitosas (sin hallazgos).

## 4) Integracion y estado de cierre
- Componente `GestionCorrespondenciaRoute` mantiene key remount `gestion-respuesta-${parsedId}` en el contenedor de detalle.
- Se conserva el contrato de guardas (`hasDetail`, `hasValidId`, `loading`, `blocked`, detalle disabled).
- Integracion documentada con `GestionRespuesta` a traves de `detailContent` + props de contexto (`idTareaWf`, `radicado`, `idRespuestaRadicado`).
- Ejecutada adicionalmente validacion de componentes relacionados (`DocumentosWorkbench`, route page y hooks documentales).
- Pendiente a completar:
  - Validacion completa desde bandeja + tabs + visor + arbol en QA/browser.
  - Validacion de adjuntos/reload/responsive en entorno real.

## 5) QT / Calidad y pendientes
- Intento de corrida completa de `src/modules/gestionCorrespondencia/tests/*` detecto fallos preexistentes no asociados al cambio 231:
  - `GestionRespuestaMainTabContent.test.tsx`: `elementFromPoint` no implementado (limpieza de entorno jsdom para tiptap/prosemirror).
  - `GestionCorrespondencia.test.tsx`: timeout en test de exportacion y aserciones de navegacion especifica.
- No se ejecutaron e2e automáticos por falta de variables PLAYWRIGHT en entorno local (`PLAYWRIGHT_*` no definidas).
- Riesgo residual: regresiones visuales y de rendimiento en flujo completo (tree + visor + adjuntos).

## 6) Regresion
- Parcialmente validado: no hay regresion en ruta remount.
- Pendiente:
  - Regresion funcional completa de `AppTable`, `AppTreeTable`, `AppVisorEmbedPdf` en cambio de tarea.
  - Integracion completa `SCRUM-205` y estabilidad de vista compartida.
  - Integridad de adjuntos y recarga en cambio de id.

## 7) Matriz de cobertura (estado)
| Capa | Cobertura | Evidencia | Estado |
|---|---|---|---|
| Routing (`parsedId` key/remount) | `detailPanelKey` por id | `GestionCorrespondenciaRoute.tsx` + `GestionCorrespondenciaRoute.spec.test.tsx` | ? |
| Estado local de detalle | Reset de estado simulado entre tareas | `GestionCorrespondenciaRoute.spec.test.tsx` | ? |
| Providers/contextos asociados | Contenedor de detalle remounted | `GestionCorrespondenciaRoute.tsx` + test de `detailContent` | ? |
| Navegacion rapida | Transiciones sucesivas `/924 -> /925 -> /926` | `GestionCorrespondenciaRoute.spec.test.tsx` | ? |
| Tree/visor/adjuntos | Integracion parcial en tests unitarios y de workbench | `DocumentosWorkbench.test.tsx`, `useListaDocumentosRadicadosTreeTable.test.tsx`, `GestionRespuestaMainTabContent` (si aplica) | ?? |

## 8) Defectos y riesgos residuales
- Riesgo de state async en condiciones de red extrema (requiere validacion con entorno real).
- Riesgo visual/regresivo por interaccion multi-componente (`DocumentosWorkbench`, `AppTreeTable`, `AppVisorEmbedPdf`) pendiente de QA/e2e.
- Riesgo residual por memoria y carrera de request si cambia rapidiss de id sin cancelacion explicita en componentes no migrados de remount.

## 9) Conclusiones de calidad
- Objetivo del ticket (aislamiento por remount de detalle al cambiar tarea): implementado y verificado en suite de ruta.
- Cierre operational requiere completar regresion transversal + e2e con datos de QA y luego actualizar estado F (Jira+archive).
