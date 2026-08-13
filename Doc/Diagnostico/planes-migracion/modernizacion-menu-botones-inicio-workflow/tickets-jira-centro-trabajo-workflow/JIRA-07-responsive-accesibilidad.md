# JIRA-07 — Responsive, accesibilidad y resiliencia visual

## Prompt para Jira

**Rol:** Actúa como especialista senior en accesibilidad web, diseño responsive y QA de interfaces empresariales legacy.

Endurece la experiencia del centro de trabajo para escritorio, tablet y móvil, con accesibilidad verificable y sin alterar lógica Workflow.

### Alcance

- Validar 1366, 1024, 768 y 375 px.
- Garantizar foco visible, orden de tabulación lógico, Enter/Espacio en controles nativos y Escape para menús.
- Definir objetivos táctiles de 40 px o más en acciones iconográficas críticas.
- Añadir `aria-label` a iconos sin texto y preservar texto visible en acciones destructivas.
- Validar contraste, truncamiento, lector de contexto y capas de modales/dropdowns.

### Restricciones no negociables

- `title` no es sustituto de nombre accesible.
- No ocultar una acción esencial únicamente por viewport; se reubica en menú accesible.
- No cambiar semántica de controles ASP.NET.

### Entregables técnicos

1. `01-AccesibilidadCentroTrabajo.md`.
2. `02-ResponsiveBreakpoints.md`.
3. Evidencia de teclado y capturas por viewport.

### Criterios de aceptación

- No hay solapamientos, pérdida de foco ni scroll horizontal accidental.
- Todo icono interactivo tiene nombre accesible.
- El lector puede identificar tarea y documento activos.

### Pruebas requeridas

- Tab/Shift+Tab/Enter/Escape; zoom navegador 200 %; móvil 375 px.

### Reversión

Revertir exclusivamente reglas responsive/accesibles nuevas si causan regresión visual.
