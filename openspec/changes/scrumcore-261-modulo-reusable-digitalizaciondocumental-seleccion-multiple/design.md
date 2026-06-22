## Context

SCRUMCORE-261: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- SELECCIÓN -MULTIPLE

## Jira Details

> SELECCIÓN MÚLTIPLE DE PÁGINAS Y ACCIONES MASIVAS
> CONTEXTO
> Actualmente el módulo permite:
> ✓ Escanear documentos✓ Visualizar miniaturas✓ Organizar páginas✓ Rotar páginas✓ Eliminar páginas✓ Reordenar páginas✓ Recorte manual✓ Zoom
> Sin embargo, todas las acciones se ejecutan sobre una única página.
> OBJETIVO
> Permitir seleccionar múltiples páginas y ejecutar operaciones masivas.
> ==================================================
> SELECCIÓN MÚLTIPLE
> Agregar modo de selección múltiple.
> Cada miniatura debe permitir:
> ☑ Seleccionar
> ☐ Deseleccionar
> ==================================================
> COMPORTAMIENTO
> Click normal:
> Selecciona página activa.
> CTRL + Click
> o
> Checkbox
> ↓
> Agrega página a selección.
> ==================================================
> ESTADO VISUAL
> Mostrar cantidad seleccionada.
> Ejemplo:
> 3 páginas seleccionadas
> ==================================================
> TOOLBAR
> Cuando exista selección múltiple mostrar:
> ↶ Rotar Izquierda
> ↷ Rotar Derecha
> 🗑 Eliminar
> ✂ Aplicar Crop
> 🧹 Limpiar selección
> ==================================================
> ROTACIÓN MASIVA
> Aplicar:
> 90°
> 270°
> a todas las páginas seleccionadas.
> ==================================================
> ELIMINACIÓN MASIVA
> Eliminar todas las páginas seleccionadas.
> Solicitar confirmación.
> ==================================================
> CROP MASIVO
> Si existe una selección válida:
> Permitir aplicar el mismo recorte a todas las páginas seleccionadas.
> Preparar arquitectura aunque inicialmente quede deshabilitado.
> ==================================================
> ORGANIZADOR DE PÁGINAS
> Mantener compatibilidad con:
> 2x2
> 3x3
> 4x4
> 5x5
> 6x6
> Drag & Drop
> ==================================================
> SELECCIONAR TODO
> Agregar acción:
> Seleccionar Todo
> Deseleccionar Todo
> ==================================================
> INDICADORES
> Mostrar:
> Página 1Página 2Página 3
> seleccionadas visualmente.
> ==================================================
> ARQUITECTURA
> Crear modelo:
> SelectedPageIds
> Centralizar estado.
> Evitar duplicidad.
> ==================================================
> RENDIMIENTO
> Validar:
> 10 páginas
> 50 páginas
> 100 páginas
> 300 páginas
> Evitar re-render masivo innecesario.
> ==================================================
> DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-280-multi-page-selection.md
> Incluir:
> Flujo UX.
> 
> Estados.
> 
> Arquitectura.
> 
> Riesgos.
> 
> Casos de prueba.
> 
> ==================================================
> VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> IMPLEMENTAR

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. La seleccion multiple se centraliza en `selectedPageIds`, dejando `selectedPageId` solo como pagina activa de preview.
2. El panel de miniaturas y el organizador reutilizan la misma seleccion para evitar estados paralelos.
3. Click normal conserva el comportamiento existente de pagina activa; Ctrl+click/Cmd+click alterna seleccion.
4. Los checkboxes por miniatura permiten seleccionar sin depender del teclado.
5. La rotacion y eliminacion masiva reutilizan los botones existentes de la toolbar unica y el contrato `rotatePage`/`removePage` por pagina.
6. No se crea toolbar secundaria ni botones duplicados para seleccion multiple.
7. Crop masivo queda como capacidad futura documentada; no se agrega boton duplicado en el preview.
8. La eliminacion masiva solicita confirmacion antes de ejecutar operaciones destructivas.

## Risks / Trade-offs

- Operaciones masivas sobre 100+ paginas ejecutan llamadas por pagina; una API batch puede ser necesaria si el rendimiento real no es suficiente.
- Crop masivo requiere validar paginas con tamanos/orientaciones diferentes antes de habilitarse.
- `window.confirm` resuelve la confirmacion minima; puede reemplazarse por modal corporativo en un ticket de UX.
- El estado `Set` debe depurarse cuando cambie `scanner.pages` para no conservar IDs eliminados.

## Migration Plan

1. Reemplazar seleccion aislada del organizador por `selectedPageIds`.
2. Agregar checkboxes y Ctrl+click en miniaturas.
3. Mostrar badge contextual cuando haya seleccion.
4. Reutilizar la seleccion central en el organizador 2x2 a 6x6.
5. Hacer que los botones existentes de rotar/eliminar apliquen sobre `selectedPageIds` cuando exista seleccion.
6. Documentar arquitectura y casos de prueba.

## Open Questions

- Definir en un ticket futuro si crop masivo debe aplicar la misma region solo a paginas con dimensiones compatibles.
- Definir si operaciones masivas deben exponerse en el scanner client como batch.
