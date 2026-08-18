# CONFIRMACION-ESPECIALIZADA

- Ticket: DOC-13
- Cambio OpenSpec: doc-13-confirmacion-especializada
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- Unit: `node --test tests/workflow-transition-ui.test.cjs tests/confirmation-dialog.test.cjs tests/workflow-transition-confirmation-integration.test.cjs tests/workflow-transition-page-presentation.test.cjs`; 20 pruebas aprobadas y 0 fallos el 2026-08-17. Cubre normalización, limpieza tras éxito, bloqueo de doble envío, foco, Escape, Tab, avisos y mensaje seguro de red.
- Manual QA: recorrido autorizado el 2026-08-17 con el piloto temporal. Se verificaron cancelar, éxito, bloqueo visual seguro, error técnico seguro, concurrencia, accesibilidad y vistas 390 × 894 y 321 × 568. El gate se restauró desactivado al finalizar.

## QA/E2E WebForms

No se ejecutó E2E autenticada: los scripts disponibles cubren contratos DOC-10 y DOC-11, y una corrida DOC-13 requeriría ambiente, cuenta y tarea de prueba autorizados. La evidencia manual no incluye credenciales, cookies, URL interna, radicados reales ni resultados de negocio. El detalle reproducible, límites y reversa se encuentran en `Doc/Actualizacion/workflow/Terminar/05-confirmacion-especializada/04-pruebas-y-evidencia.md`.
