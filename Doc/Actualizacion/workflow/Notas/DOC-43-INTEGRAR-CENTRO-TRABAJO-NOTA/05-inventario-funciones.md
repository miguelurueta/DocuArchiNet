# Inventario de implementación

| Archivo | Responsabilidad |
|---|---|
| `workflow/Webworkflow.aspx` | Lista cronológica, estados, editor accesible y fallback legacy |
| `workflow/Webworkflow.aspx.vb` | Gate, exclusión entre consumidores y bootstrap con tarea explícita |
| `workflow/Webworkflow.aspx.designer.vb` | Declaración del panel moderno |
| `js/workflow/Webworkflow.js` | Adaptador único, render seguro y CRUD |
| `Styles/workflow-notes-modern.css` | Composición encapsulada y responsive |
| `Infrastructure/Repositories/Workflow/MySqlNotasWorkflowRepository.vb` | Proyección completa del contenido listado |
| `tools/e2e/tests/doc43-notes-ui-policy.test.cjs` | Políticas y regresiones focales |
| `tools/e2e/tests/doc43-notes-ui.spec.cjs` | CRUD UI autenticado |
| `tools/e2e/scripts/run-doc43-notes-ui-interactive.cjs` | Autorización TTY y restauración segura del gate |
