## Context

SCRUMCORE-256: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- VISTA-CUADRICULA

## Jira Details

> VISTA AVANZADA DE MINIATURAS
> OBJETIVO
> Permitir múltiples modos de visualización.
> ==================================================MODOS
> Lista
> 2x2
> 4x4
> 6x6
> ==================================================COMPORTAMIENTO
> Mantener:
> Drag & Drop.
> 
> Selección.
> 
> Checkboxes.
> 
> Reordenamiento.
> 
> ==================================================UI
> Botón:
> ⊞ Vista
> ==================================================DOCUMENTAR
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-267-thumbnail-grid.md
> ==================================================RENDIMIENTO
> Virtualización si supera 100 páginas.
> IMPLEMENTAR.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. La vista avanzada se implementa en `DigitalizacionDocumentalWorkspace`, porque ahi viven el panel de miniaturas, la seleccion y el reordenamiento.
2. El control `Vista` se agrega como `AppDropdown` icon-only en la barra del preview, en la misma linea de `Ir a pagina` y acciones de rotacion.
3. Los modos de vista se modelan como estado local de presentacion: `grid1`, `grid2`, `grid3`, `grid4`, `grid5` y `grid6`.
4. El layout se controla por CSS usando `data-view-mode`, sin transformar `scanner.pages`.
5. Para mas de 100 paginas se activa `data-virtualized="true"` y CSS `content-visibility: auto`.
6. El organizador de paginas se implementa como overlay absoluto dentro del panel de preview, controlado por `showPageOrganizer`.
7. El organizador usa directamente `scanner.pages`; solo mantiene un `Set` de ids seleccionados para acciones masivas y no crea una coleccion paralela de paginas.
8. El boton `Organizar paginas` vive en la toolbar principal y el boton `Cerrar organizacion` vive en la esquina superior derecha del overlay.

## Risks / Trade-offs

- `content-visibility` depende de soporte del navegador; si no aplica, la lista sigue funcionando con render completo.
- No se cambia la semantica de seleccion: la pagina activa sigue siendo un unico `selectedPageId`.
- El overlay agrega una segunda superficie de interaccion sobre los mismos handlers; las pruebas deben cubrir que preview y miniaturas siguen montados.

## Migration Plan

1. Agregar estado y dropdown `Vista` en la barra del preview.
2. Agregar estilos responsive para lista y cuadriculas.
3. Mantener handlers existentes de drag/drop, seleccion y reordenamiento.
4. Agregar pruebas RTL y documentacion tecnica.
5. Agregar overlay de organizacion sobre preview sin regenerar paginas ni solicitar imagenes a Dynamsoft.

## Open Questions

- Ninguna pendiente para la implementacion inicial.
