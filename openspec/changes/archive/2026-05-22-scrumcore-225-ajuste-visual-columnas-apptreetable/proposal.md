# SCRUMCORE-225 — Proposal: Ajuste visual de columnas (AppTreeTable / Workbench)

## Why
En el `DocumentosWorkbench`, el listado basado en `AppTreeTable` puede renderizar más de dos columnas o dejar una columna secundaria con sizing inconsistente (poca visibilidad), afectando la legibilidad y la experiencia de usuario en un panel de trabajo crítico. Se requiere un ajuste visual enterprise, acotado y trazable, sin cambiar comportamiento funcional ni contratos backend.

## What Changes
- Aplicar una regla **visual** (no funcional) para que el listado de documentos del Workbench renderice **exactamente 2 columnas** (primaria + secundaria) con sizing consistente.
- Ajustar el “sizing preset” de esas 2 columnas (flex/minWidth) para evitar que la columna secundaria quede invisibilizada por el layout.
- Mantener intactos: selección, click primario, menú/acciones backend-driven (`onActionTriggered`), integración visor PDF, y compatibilidad SCRUM-205/SCRUM-209.
- Agregar pruebas automatizadas para regresión visual/funcional (incluye Playwright) enfocadas en Workbench.
- Documentar el cambio en ruta obligatoria: `docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnas/` (con archivos `SCRUMCORE-225-*`).

## Capabilities
### New Capabilities
- `apptreetable-ajuste-visual-columnas-workbench`
  - Reglas UI/UX para renderizar 2 columnas visibles en Workbench.
  - Reglas de selección de columnas basada en configuración backend-driven (sin hardcodear columnas).
  - Reglas de sizing enterprise (flex/minWidth/truncado) y comportamiento responsive.

### Modified Capabilities
- (vacío)

## Impact
- **Código**: cambios acotados al pipeline de columnas backend-driven (Dynamic UI → AG Grid columns) y/o su adaptación para el contexto de Workbench (sin modificar contratos backend).
- **UI/UX**: mejora de visibilidad y consistencia del listado (2 columnas), preservando layout visor + rail y comportamiento actual.
- **Testing**: nuevas pruebas unitarias e integración, y tests Playwright de regresión visual/funcional para Workbench.
- **Docs**: documentación enterprise obligatoria en `docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnas/` con trazabilidad a archivos tocados, decisiones, riesgos, mitigaciones y evidencia de pruebas.

