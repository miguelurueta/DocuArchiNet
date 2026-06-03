## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira + contexto real de `AppVisorEmbedPdf`.
- [x] 1.2 Corregir naming del componente: `AppGuideTour` (no `AppAppguidetour`).
- [x] 1.3 Definir controles reales del tour segun toolbar existente: thumbnails, zoom out, zoom level, zoom in, reset zoom, rotate left/right, signature, lock/unlock, delete signature, print, export y ayuda.
- [x] 1.4 Ajustar `design.md` con arquitectura, contratos, decisiones, riesgos y diagramas Mermaid.
- [x] 1.5 Ajustar `spec.md` con requisitos verificables y escenarios concretos.

## 2. Dependencia y estructura base

- [x] 2.1 Agregar dependencia `driver.js`.
- [x] 2.2 Crear estructura `src/app/Components/UI/AppGuideTour/`.
- [x] 2.3 Crear exports publicos en `index.ts`.
- [x] 2.4 Definir tipos publicos en `AppGuideTour.types.ts`.
- [x] 2.5 Definir constantes/eventos en `AppGuideTour.constants.ts`.
- [x] 2.6 Crear `AppGuideTour.adapter.ts` como contrato/puerto interno entre hook/componente y driver concreto.

## 3. Adapter, hook y componente reusable

- [x] 3.1 Implementar `drivers/DriverJsAdapter.ts` encapsulando Driver.js.
- [x] 3.2 Implementar servicio de normalizacion/filtrado de steps (`AppGuideTour.service.ts`).
- [x] 3.3 Implementar `hooks/useAppGuideTour.ts` con state machine minima.
- [x] 3.4 Implementar `AppGuideTour.tsx`.
- [x] 3.5 Implementar `providers/AppGuideTourProvider.tsx` solo si se necesita contexto compartido; si no aplica, documentar decision y omitir.
- [x] 3.6 Asegurar cleanup/destruccion de Driver.js en unmount.
- [x] 3.7 Asegurar que consumers no importan `driver.js` directamente.
- [x] 3.8 Asegurar que no se introduce Driver.js dentro de plugins, hooks existentes, reducers existentes ni logica PDF del visor.
- [x] 3.9 Mantener TypeScript estricto sin `any` nuevo en AppGuideTour/integracion.
- [x] 3.10 Validar ausencia de dependencia circular entre `AppGuideTour` y `AppVisorEmbedPdf`.

## 4. Integracion AppVisorEmbedPdf

- [x] 4.1 Agregar `data-guide-tour-id` estables a controles reales de `AppPdfToolbar`.
- [x] 4.2 Extender `AppPdfToolbarProps` con props opcionales de guia sin romper consumers.
- [x] 4.3 Agregar boton de ayuda accesible en toolbar usando estilo existente.
- [x] 4.4 Crear configuracion de steps para los botones reales de `AppPdfToolbar`: thumbnails, zoom out, zoom level, zoom in, reset zoom, rotate left/right, signature, lock/unlock, delete signature, print, export y ayuda.
- [x] 4.5 Integrar `useAppGuideTour`/`AppGuideTour` en `AppVisorEmbedPdf` sin tocar logica PDF.
- [x] 4.6 Agregar targets para overlays visibles relevantes: paginacion y scroll-to-top si existen.
- [x] 4.7 Verificar que zoom, rotate, print, export, firma, anotaciones, thumbnails y scroll no cambian comportamiento.
- [x] 4.8 Confirmar que el boton de ayuda es visible en desktop y mobile sin alterar layout.
- [x] 4.9 Confirmar que Search, Fit Width y Fit Page quedan fuera del tour porque no existen como botones actuales de `AppPdfToolbar`; no crear esos controles en este ticket.

## 5. Accesibilidad, observabilidad y performance

- [x] 5.1 Validar `aria-label`, `aria-describedby` cuando aplique, title/tooltip y foco visible del boton de ayuda.
- [x] 5.2 Validar cierre con Escape y cleanup de foco cuando sea posible.
- [x] 5.3 Emitir eventos `guide_started`, `guide_completed`, `guide_cancelled`, `guide_step_changed`, `guide_error`.
- [x] 5.4 Garantizar que eventos no incluyan URLs, tokens, nombres de archivo, contenido PDF ni identificadores documentales sensibles.
- [x] 5.5 Memoizar steps y evitar recreacion innecesaria del driver.
- [x] 5.6 Filtrar targets faltantes al iniciar el tour.
- [x] 5.7 Validar que el tour es screen-reader friendly en el alcance permitido por Driver.js.
- [x] 5.8 Confirmar que iniciar/detener el tour no provoca reload ni rerenders costosos del visor PDF.

## 6. Pruebas unitarias e integracion

- [x] 6.1 Unit: `AppGuideTour` renderiza sin auto-start por defecto.
- [x] 6.2 Unit: `useAppGuideTour` registra steps y ejecuta `start()`, `stop()`, `refresh()` y cleanup.
- [x] 6.3 Unit: `DriverJsAdapter` mapea steps a Driver.js y destruye instancia.
- [x] 6.4 Unit: servicio filtra steps sin target DOM.
- [x] 6.5 Integration: `AppPdfToolbar` muestra boton ayuda cuando recibe `onStartGuideTour`.
- [x] 6.6 Integration: click/keyboard en ayuda inicia tour.
- [x] 6.7 Integration: recorrido completo y cierre correcto con driver mockeado.
- [x] 6.8 Regression: tests existentes de zoom, thumbnails, rotate, print/export, download y signature siguen pasando.

## 7. Playwright

- [x] 7.1 Crear smoke Playwright para boton ayuda visible.
- [x] 7.2 Validar tooltip/title accesible.
- [x] 7.3 Validar apertura del tour.
- [x] 7.4 Validar navegacion siguiente/anterior.
- [x] 7.5 Validar finalizacion/cancelacion del tour.
- [x] 7.6 Validar responsive desktop/tablet/mobile.

## 8. Documentacion enterprise

- [x] 8.1 Crear `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Arquitectura.md`.
- [x] 8.2 Crear `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Implementacion-Detallada.md`.
- [x] 8.3 Crear `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Pruebas.md`.
- [x] 8.4 Crear `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Metadata.md`.
- [x] 8.5 Incluir trazabilidad: AppGuideTour, DriverJsAdapter, useAppGuideTour, HelpButton, Playwright.
- [x] 8.6 Incluir diagramas Mermaid obligatorios en arquitectura: classDiagram, sequenceDiagram y stateDiagram-v2.
- [x] 8.7 Incluir tabla de metadata con elemento, archivo, evidencia y estado.

## 9. Verificacion y cierre

- [x] 9.1 Ejecutar tests unitarios/integracion afectados y registrar evidencia.
- [x] 9.2 Ejecutar Playwright afectado y registrar evidencia.
- [x] 9.3 Ejecutar build o registrar deuda TypeScript no relacionada si falla.
- [x] 9.4 Validar OpenSpec (`spec:validate` si aplica).
- [ ] 9.5 Preparar `opsxj:archive`, PR y cierre Jira post-merge.
