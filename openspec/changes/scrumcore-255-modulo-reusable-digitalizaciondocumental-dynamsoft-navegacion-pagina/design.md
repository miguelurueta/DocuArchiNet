## Context

SCRUMCORE-255: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- DYNAMSOFT-NAVEGACION-PAGINA

## Jira Details

> NAVEGACIÓN RÁPIDA ENTRE PÁGINAS
> OBJETIVO
> Permitir navegar rápidamente a una página específica.
> ==================================================FASE 1
> Agregar control:
> [ Página ] [ Ir ]
> Ejemplo:
> 5
> ↓
> Ir
> ↓
> Página 5
> ==================================================FASE 2
> Atajo:
> CTRL + G
> ==================================================FASE 3
> Scroll automático.
> Selección automática.
> Highlight temporal.
> ==================================================DOCUMENTAR
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-266-page-navigation.md
> ==================================================RENDIMIENTO
> No recorrer DOM completo.
> No re-renderizar todas las miniaturas.
> IMPLEMENTAR.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. La navegacion rapida se implementa en `DigitalizacionDocumentalWorkspace`, porque ahi viven la toolbar, la seleccion de pagina y la lista de miniaturas.
2. El control usa un input numerico con boton `Ir`; `Ctrl+G` solo enfoca el input para no disparar navegacion accidental.
3. El scroll automatico usa refs por `page.id` en un `Map`, evitando recorrer el DOM completo.
4. El highlight temporal usa estado local (`highlightedPageId`) y `data-highlighted`, sin mutar la coleccion `scanner.pages`.

## Risks / Trade-offs

- Si las miniaturas estan colapsadas, la pagina se selecciona y el preview cambia, pero no se fuerza expandir el panel para respetar la preferencia del usuario.
- El timeout de highlight es visual y no participa en persistencia ni en datos del scanner.

## Migration Plan

1. Agregar control de navegacion y shortcut en el workspace.
2. Mantener refs por miniatura y estilos para highlight temporal.
3. Cubrir el flujo con pruebas RTL y documentar la arquitectura.

## Open Questions

- Ninguna pendiente para la implementacion inicial.
