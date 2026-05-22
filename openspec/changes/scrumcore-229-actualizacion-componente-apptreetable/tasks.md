## 1. Refinement (antes de implementar)

- [x] 1.1 Confirmar **scope root** del Workbench (preferido: `data-testid="documentos-workbench"`; fallback: wrapper class del módulo)
- [x] 1.2 Identificar archivo de estilos actual del Workbench (reusar si existe; si no, crear CSS Module nuevo)
- [x] 1.3 Definir tokens/variables de color permitidos (Design System) para hover/selected/focus (sin redefinir fuentes globales)

## 2. Implementación (CSS-only, sin tocar AppTable)

- [x] 2.1 Crear/editar **CSS custom** scoped del Workbench (CSS-only / CSS Modules) según prompt JIRA
- [x] 2.2 Definir selectores objetivo (solo dentro del scope):
  - headers: `.ag-header`, `.ag-header-cell`, `.ag-header-cell-label`
  - rows/cells: `.ag-row`, `.ag-cell`
  - action cell: `.app-table-action-cell` (si existe)
  - selection col (opcional): `.ag-Grid-SelectionColumn` (suavizar; no ocultar por defecto)
- [x] 2.3 Header limpio: reducir bordes, mejorar tipografía semibold y spacing (sin “caja” fuerte)
- [x] 2.4 Rows: hover sutil (sin parpadeos) + separador leve (`border-bottom` sutil o `box-shadow` muy leve; sin marcos)
- [x] 2.5 Selected state: resaltar fila seleccionada usando `aria-selected="true"` (background + outline discreto; contraste OK)
- [x] 2.6 Focus visible: estilo de foco en celdas navegables (teclado) sin focus traps
- [x] 2.7 Action cell: look minimalista del botón de acciones (sin tocar renderer)
- [x] 2.8 Performance MUST: evitar `:has()`, evitar selectores profundos/globales (`*`), no animar `width/height/top/left`, preferir variables/estilos estáticos
- [x] 2.9 Verificación: se mantienen 2 columnas funcionales cuando backend las provee (Documento + Acciones) sin tocar sizing
- [x] 2.10 Verificación: no se cambió el ancho de columnas (no tocar `minWidth`/`flex`/`width` desde TS/JS)
- [x] 2.11 Verificación: no hay cambios en `src/app/Components/UI/AppTable/**` (MUST)
- [x] 2.12 Verificación: no hay cambios funcionales en `AppTreeTable` (handlers/hooks/data flow) (MUST)
- [x] 2.13 Smoke: no warnings/errores en consola y no regresiones visuales fuera de Workbench (checklist)

## 3. Playwright (regresión)

- [x] 3.1 Test: Workbench muestra 2 headers visibles (Documento + Acciones cuando existan)
- [x] 3.2 Test: foco visible al navegar con teclado
- [x] 3.3 Test: selected + hover aplican estilo sin romper layout
- [x] 3.4 Snapshot/asserter visual: usar snapshot si ya existe baseline; si no, asserts de estilo/atributos

## 4. Documentación enterprise (ruta obligatoria)

Ruta: `docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/`

- [x] 4.1 `SCRUMCORE-229-Arquitectura.md` (Mermaid + alcance/no alcance + riesgos)
- [x] 4.2 `SCRUMCORE-229-Implementacion-Detallada.md` (rutas reales, **lista exacta de selectores**, variables/colores, decisiones UX, y “qué NO se tocó”)
- [x] 4.3 `SCRUMCORE-229-Pruebas.md` (Playwright ejecutadas vs pendientes + comandos)
- [x] 4.4 `SCRUMCORE-229-Metadata.md` (traza: commits/PR/merge/opsxj)

## 5. Cierre

- [x] 5.1 `npm.cmd test` (suite focal o la recomendada por el módulo)
- [x] 5.2 `npm.cmd run opsxj:validate` (si existe en el repo) o `spec:validate`
- [ ] 5.3 Commit/push/PR siguiendo flujo opsxj (sin mezclar con otros tickets)
