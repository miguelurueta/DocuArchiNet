## Context

SCRUMCORE-254: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- PANELES-COLAPSABLES

## Jira Details

> PANELES COLAPSABLES PARA DIGITALIZACIÓN DOCUMENTAL
> CONTEXTO
> Actualmente el módulo utiliza tres áreas:
> Miniaturas.
> 
> Preview PDF.
> 
> Configuración de escaneo.
> 
> Se requiere permitir que el usuario oculte paneles para maximizar el espacio útil del documento.
> OBJETIVO
> Permitir:
> ✓ Ocultar Miniaturas.✓ Mostrar Miniaturas.
> ✓ Ocultar Configuración.✓ Mostrar Configuración.
> Manteniendo el Preview PDF como área central dinámica.
> ==================================================FASE 1 - AUDITORÍA
> Documentar en:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-265-collapsible-panels.md
> Analizar:
> Layout actual.
> 
> Grid actual.
> 
> Dependencias.
> 
> Riesgos.
> 
> ==================================================FASE 2 - MINIATURAS
> Agregar botón:
> ☰ Miniaturas
> Estados:
> Visible.
> 
> Oculto.
> 
> El panel debe:
> Contraerse a 0.
> 
> Mantener Drag & Drop.
> 
> Mantener selección.
> 
> Mantener scroll.
> 
> ==================================================FASE 3 - CONFIGURACIÓN
> Agregar botón:
> ⚙ Configuración
> Estados:
> Visible.
> 
> Oculto.
> 
> ==================================================FASE 4 - PREVIEW RESPONSIVO
> Cuando un panel se oculta:
> Preview debe expandirse automáticamente.
> Cuando ambos se ocultan:
> Preview debe ocupar el ancho disponible.
> ==================================================FASE 5 - PERSISTENCIA
> Persistir:
> showThumbnailsshowConfiguration
> usando localStorage.
> ==================================================RENDIMIENTO
> No generar:
> Re-render completo.
> 
> Re-carga de scanner.
> 
> Re-carga de miniaturas.
> 
> ==================================================VALIDACIONES
> tsceslintvitest
> IMPLEMENTAR.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. TBD

## Risks / Trade-offs

- TBD

## Migration Plan

1. TBD

## Open Questions

- TBD
