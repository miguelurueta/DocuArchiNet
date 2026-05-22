## Why

ACTUALIZACION-COMPONENTE-APPTREETABLE. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-229.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> PROMPT ENTERPRISE — SCRUMCORE-[ID] Ajuste Visual (CSS-only) de AppTreeTable en Workbench (sin tocar AppTable)
>   Rol esperado  Arquitecto Frontend Senior (UI Enterprise, React 19, TypeScript strict, Design System, UX/UI consistency)
>   Objetivo  Mejorar el aspecto visual del listado renderizado por AppTreeTable en el Workbench usando solo CSS (o CSS Modules),  logrando un look moderno/limpio/enterprise (sin bordes pesados), manteniendo:
> exactamente 2 columnas funcionales visibles (Documento + Acciones) cuando existan
> 
> el ancho actual de columnas (no cambiar minWidth/flex/width desde código)
> 
> performance estable (sin reflows costosos ni selectores pesados)
> 
>   Restricciones obligatorias (MUST)
> NO modificar AppTable (código, adapters, renderers, props, sizing, columnas).
> 
> NO modificar AppTreeTable lógica funcional (handlers, hooks, data flow).
> 
> NO cambiar contratos backend/DTOs.
> 
> NO introducir dependencias pesadas.
> 
> NO usar any.
> 
> Mantener TypeScript strict.
> 
> Cambios CSS-only y scopeado a Workbench (no estilos globales que afecten otras tablas).
> 
>   Alcance
> Estilos del listado (headers, rows, hover, focus, acciones, spacing, tipografía).
> 
> Estados visuales: loading / empty / error (solo estilos si ya existen; no re-implementar lógica).
> 
> Accesibilidad visual: focus visible, contraste, estados aria-selected.
> 
>   No alcance
> Cambios en columnas (cantidad, orden, sizing).
> 
> Cambios en funcionalidad (select, actions, click, keyboard behavior).
> 
> Refactors de componentes.
> 
>   Ubicación objetivo (scope)  Debe aplicarse únicamente en Workbench:
> src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsxy/o sus estilos asociados (sin tocar AppTable).
> 
>   Recomendación de implementación (CSS-only, sin tocar AppTable)
> Definir un “scope root” exclusivo del Workbench:
> Usar contenedor existente: data-testid="documentos-workbench" (preferido)
> 
> o agregar una clase wrapper en Workbench (si ya existe un wrapper de estilos del módulo).
> 
> Aplicar estilos sobre clases de AG Grid / AppTable solo dentro del scope:
> Headers: .ag-header, .ag-header-cell, .ag-header-cell-label
> 
> Rows/cells: .ag-row, .ag-cell
> 
> Action cell: .app-table-action-cell (si existe)
> 
> Selection column: .ag-Grid-SelectionColumn (solo si se quiere suavizar visualmente, sin ocultarla por defecto)
> 
> Look & feel requerido (enterprise limpio)
> 
> Fondo: limpio, sin bordes marcados.
> 
> Separadores: usar border-bottom sutil (1px) o box-shadow muy leve, no marcos.
> 
> Hover: suave, sin parpadeos.
> 
> Selected row: resaltado discreto (background + outline), respetando aria-selected="true".
> 
> Header: tipografía semibold, sin “caja” fuerte; sticky si ya existe.
> 
> Actions: botón de acciones minimalista, alineación consistente.
> 
> Tipografía: consistente con Design System; no redefinir fuentes globales.
> 
> Performance (MUST)
> 
> Evitar selectores profundos/caros.
> 
> Evitar :has() y selectores globales.
> 
> No animar propiedades que afecten layout (width/height/top/left) en scroll.
> 
> Preferir variables y estilos estáticos.
> 
>   Criterios de aceptación
> No hay cambios en AppTable (git diff no muestra modificaciones bajo src/app/Components/UI/AppTable/).
> 
> En Workbench, el listado se ve moderno/limpio/enterprise:
> headers más livianos
> 
> filas con hover y selección claros
> 
> focus visible al navegar con teclado
> 
> El ancho de columnas se mantiene (sin ajustes de sizing en JS/TS).
> 
> Se siguen viendo 2 columnas funcionales cuando el backend las provee (Documento + Acciones).
> 
> No hay warnings/errores en consola.
> 
> Sin regresiones visuales en otras pantallas (scope correcto).
> 
>   Pruebas requeridas
> Unit: NO obligatorias si es solo CSS.
> 
> Playwright (obligatorio):
> Workbench renderiza 2 headers visibles (Documento + Acciones) cuando existan.
> 
> Navegación teclado: focus visible en una celda navegable.
> 
> Hover/selected: estilo aplicado sin romper layout.
> 
> Snapshot/asserter visual básico (si el repo ya tiene baseline; si no, asserts de estilo/atributos).
> 
>   Documentación enterprise obligatoria (ruta)  Crear carpeta:
> docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/
> 
>   Archivos obligatorios (usar SCRUMCORE-[ID]):
> docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/SCRUMCORE-[ID]-Arquitectura.mdDebe incluir:
> 
> Objetivo técnico del cambio CSS-only
> 
> Restricciones MUST (sin tocar AppTable)
> 
> Alcance/no alcance
> 
> Estrategia de scoping (por data-testid o clase wrapper)
> 
> Mermaid:
> classDiagram (Workbench -> AppTreeTable -> AppTable; CSS solo afecta render output)
> 
> sequenceDiagram (render -> CSS aplica)
> 
> stateDiagram-v2 (hover/selected/focus)
> 
> docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/SCRUMCORE-[ID]-Implementacion-Detallada.mdDebe incluir:
> 
> Archivos de estilos creados/modificados (rutas reales)
> 
> Lista exacta de selectores usados (dentro del scope)
> 
> Variables/colores usados (si aplica)
> 
> Decisiones de UX (hover/selected/focus)
> 
> Qué NO se tocó (AppTable/AppTreeTable logic)
> 
> docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/SCRUMCORE-[ID]-Pruebas.mdDebe incluir:
> 
> Playwright tests ejecutados vs pendientes
> 
> Evidencias (comandos)
> 
> Riesgos residuales (tema de CSS specificity / scoping)
> 
> docs/modulos/gestioncorrespondencia/AppTreeTable/AjusteVisualColumnasCss/SCRUMCORE-[ID]-Metadata.mdDebe incluir:
> 
> Ticket, autor, fecha, versión
> 
> Historial de cambios
> 
> Referencias cruzadas a los docs anteriores
> 
> Link PR cuando exista
> 
>   Entregables
> CSS scoped aplicado al Workbench (sin tocar AppTable)
> 
> 1+ tests Playwright de regresión visual/funcional
> 
> Documentación enterprise completa en la ruta indicada
> 
> Lista de archivos modificados
> 
>   Instrucción final  Implementar un refresh visual moderno/limpio/enterprise del listado AppTreeTable en Workbench usando solo CSS scoped  (idealmente por data-testid="documentos-workbench"), sin modificar AppTable, preservando el ancho actual de columnas y  garantizando accesibilidad/performance.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: ACTUALIZACION, APPTREETABLE, COMPONENTE

## Capabilities

### New Capabilities
- `actualizacion-componente-apptreetable`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
