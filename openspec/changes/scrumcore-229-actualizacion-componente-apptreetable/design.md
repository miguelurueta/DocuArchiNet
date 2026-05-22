## Context

SCRUMCORE-229: ACTUALIZACION-COMPONENTE-APPTREETABLE

Este ticket define un **ajuste visual CSS-only** para modernizar el listado renderizado por `AppTreeTable` en el Workbench, sin tocar `AppTable` ni cambiar comportamiento funcional.

## Goals / Non-Goals

**Goals**
- Modernizar el look & feel (clean/enterprise) del listado de documentos en Workbench usando CSS **scopeado**.
- Mantener el **ancho actual** de columnas: no cambiar sizing desde código (`minWidth`/`flex`/`width`).
- Mantener performance (sin selectores pesados ni reflows innecesarios).
- Mantener accesibilidad visual (focus visible, selected state claro).
- Validar no-regresión con Playwright.

**Non-Goals**
- Cambios en `AppTable` (código, adapters, renderers, props, sizing, columnas).
- Cambios funcionales en `AppTreeTable` (handlers, data flow).
- Cambios en el contrato backend/DTOs.
- Rediseño estructural del Workbench (layout/responsive).

## Decisions

1. **Scoping CSS**
   - Regla: aplicar estilos únicamente bajo el contenedor `data-testid="documentos-workbench"` (o wrapper equivalente del Workbench).
   - Motivo: evitar leakage y regresiones en otras pantallas que usan `AppTreeTable/AppTable`.

2. **CSS Modules en el módulo**
   - Implementación preferida: crear/editar un archivo de estilos del módulo `documentosWorkbench` y usar selectores globales *solo dentro del scope* (por ejemplo `:global(.ag-row)`).

3. **No tocar sizing de columnas**
   - Regla: no modificar ni generar lógica de columnas; solo cambiar estilos visuales (padding, background, border, hover, focus, typography).

4. **Selectores de bajo costo**
   - Evitar `:has()`, selectores profundos y reglas globales.
   - No animar propiedades que disparen layout en scroll (width/height/top/left).

## Risks / Trade-offs

- **CSS leakage**: mitigación con scoping por `data-testid`.
- **Coste de render** por selectores: mitigación con selectores cortos y específicos.
- **Diferencias por theme/tokens**: mitigación usando variables existentes cuando estén disponibles y fallback conservador.

## Migration Plan

1. Crear hoja de estilos scoped para Workbench.
2. Aplicar estilos por capas: header → rows → selected/hover → focus visible → action cell.
3. Agregar Playwright para:
   - 2 headers visibles (Documento + Acciones cuando existan)
   - focus visible en navegación
   - selected/hover no rompen layout
4. Smoke manual rápido (Workbench y 1 pantalla adicional que use AppTreeTable) para verificar no-regresión.

## Open Questions

- ¿Existe ya un archivo de estilos del Workbench para extender, o se crea uno nuevo?
- ¿Qué tokens/variables del Design System se recomiendan para hover/selected?
