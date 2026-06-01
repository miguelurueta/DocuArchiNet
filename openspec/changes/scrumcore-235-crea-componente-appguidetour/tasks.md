## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira + contexto real de `AppVisorEmbedPdf`.
- [x] 1.2 Corregir naming del componente: `AppGuideTour` (no `AppAppguidetour`).
- [x] 1.3 Definir controles reales del tour segun toolbar existente; no inventar Search/Fit Width/Fit Page si no existen en la UI actual.
- [x] 1.4 Ajustar `design.md` con arquitectura, contratos, decisiones, riesgos y diagramas Mermaid.
- [x] 1.5 Ajustar `spec.md` con requisitos verificables y escenarios concretos.

## 2. Dependencia y estructura base

- [ ] 2.1 Agregar dependencia `driver.js`.
- [ ] 2.2 Crear estructura `src/app/Components/UI/AppGuideTour/`.
- [ ] 2.3 Crear exports publicos en `index.ts`.
- [ ] 2.4 Definir tipos publicos en `AppGuideTour.types.ts`.
- [ ] 2.5 Definir constantes/eventos en `AppGuideTour.constants.ts`.
- [ ] 2.6 Crear `AppGuideTour.adapter.ts` como contrato/puerto interno entre hook/componente y driver concreto.

## 3. Adapter, hook y componente reusable

- [ ] 3.1 Implementar `drivers/DriverJsAdapter.ts` encapsulando Driver.js.
- [ ] 3.2 Implementar servicio de normalizacion/filtrado de steps (`AppGuideTour.service.ts`).
- [ ] 3.3 Implementar `hooks/useAppGuideTour.ts` con state machine minima.
- [ ] 3.4 Implementar `AppGuideTour.tsx`.
- [ ] 3.5 Implementar `providers/AppGuideTourProvider.tsx` solo si se necesita contexto compartido; si no aplica, documentar decision y omitir.
- [ ] 3.6 Asegurar cleanup/destruccion de Driver.js en unmount.
- [ ] 3.7 Asegurar que consumers no importan `driver.js` directamente.
- [ ] 3.8 Asegurar que no se introduce Driver.js dentro de plugins, hooks existentes, reducers existentes ni logica PDF del visor.
- [ ] 3.9 Mantener TypeScript estricto sin `any` nuevo en AppGuideTour/integracion.
- [ ] 3.10 Validar ausencia de dependencia circular entre `AppGuideTour` y `AppVisorEmbedPdf`.

## 4. Integracion AppVisorEmbedPdf

- [ ] 4.1 Agregar `data-guide-tour-id` estables a controles reales de `AppPdfToolbar`.
- [ ] 4.2 Extender `AppPdfToolbarProps` con props opcionales de guia sin romper consumers.
- [ ] 4.3 Agregar boton de ayuda accesible en toolbar usando estilo existente.
- [ ] 4.4 Crear configuracion de steps para `AppVisorEmbedPdf`.
- [ ] 4.5 Integrar `useAppGuideTour`/`AppGuideTour` en `AppVisorEmbedPdf` sin tocar logica PDF.
- [ ] 4.6 Agregar targets para overlays visibles relevantes: paginacion y scroll-to-top si existen.
- [ ] 4.7 Verificar que zoom, rotate, print, export, firma, anotaciones, thumbnails y scroll no cambian comportamiento.
- [ ] 4.8 Confirmar que el boton de ayuda es visible en desktop y mobile sin alterar layout.
- [ ] 4.9 Confirmar que el tour solo cubre controles visibles/reales; Search, Fit Width y Fit Page no se agregan si no existen en la toolbar actual.

## 5. Accesibilidad, observabilidad y performance

- [ ] 5.1 Validar `aria-label`, `aria-describedby` cuando aplique, title/tooltip y foco visible del boton de ayuda.
- [ ] 5.2 Validar cierre con Escape y cleanup de foco cuando sea posible.
- [ ] 5.3 Emitir eventos `guide_started`, `guide_completed`, `guide_cancelled`, `guide_step_changed`, `guide_error`.
- [ ] 5.4 Garantizar que eventos no incluyan URLs, tokens, nombres de archivo, contenido PDF ni identificadores documentales sensibles.
- [ ] 5.5 Memoizar steps y evitar recreacion innecesaria del driver.
- [ ] 5.6 Filtrar targets faltantes al iniciar el tour.
- [ ] 5.7 Validar que el tour es screen-reader friendly en el alcance permitido por Driver.js.
- [ ] 5.8 Confirmar que iniciar/detener el tour no provoca reload ni rerenders costosos del visor PDF.

## 6. Pruebas unitarias e integracion

- [ ] 6.1 Unit: `AppGuideTour` renderiza sin auto-start por defecto.
- [ ] 6.2 Unit: `useAppGuideTour` registra steps y ejecuta `start()`, `stop()`, `refresh()` y cleanup.
- [ ] 6.3 Unit: `DriverJsAdapter` mapea steps a Driver.js y destruye instancia.
- [ ] 6.4 Unit: servicio filtra steps sin target DOM.
- [ ] 6.5 Integration: `AppPdfToolbar` muestra boton ayuda cuando recibe `onStartGuideTour`.
- [ ] 6.6 Integration: click/keyboard en ayuda inicia tour.
- [ ] 6.7 Integration: recorrido completo y cierre correcto con driver mockeado.
- [ ] 6.8 Regression: tests existentes de zoom, thumbnails, rotate, print/export, download y signature siguen pasando.

## 7. Playwright

- [ ] 7.1 Crear smoke Playwright para boton ayuda visible.
- [ ] 7.2 Validar tooltip/title accesible.
- [ ] 7.3 Validar apertura del tour.
- [ ] 7.4 Validar navegacion siguiente/anterior.
- [ ] 7.5 Validar finalizacion/cancelacion del tour.
- [ ] 7.6 Validar responsive desktop/tablet/mobile.

## 8. Documentacion enterprise

- [ ] 8.1 Crear `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Arquitectura.md`.
- [ ] 8.2 Crear `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Implementacion-Detallada.md`.
- [ ] 8.3 Crear `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Pruebas.md`.
- [ ] 8.4 Crear `docs/Components/AppGuideTour/GuiaVisorPDF/SCRUMCORE-235-Metadata.md`.
- [ ] 8.5 Incluir trazabilidad: AppGuideTour, DriverJsAdapter, useAppGuideTour, HelpButton, Playwright.
- [ ] 8.6 Incluir diagramas Mermaid obligatorios en arquitectura: classDiagram, sequenceDiagram y stateDiagram-v2.
- [ ] 8.7 Incluir tabla de metadata con elemento, archivo, evidencia y estado.

## 9. Verificacion y cierre

- [ ] 9.1 Ejecutar tests unitarios/integracion afectados y registrar evidencia.
- [ ] 9.2 Ejecutar Playwright afectado y registrar evidencia.
- [ ] 9.3 Ejecutar build o registrar deuda TypeScript no relacionada si falla.
- [ ] 9.4 Validar OpenSpec (`spec:validate` si aplica).
- [ ] 9.5 Preparar `opsxj:archive`, PR y cierre Jira post-merge.
