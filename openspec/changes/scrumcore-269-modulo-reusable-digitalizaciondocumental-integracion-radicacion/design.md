## Context

SCRUMCORE-269: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-INTEGRACION-RADICACION

## Jira Details

> INTEGRACIÓN DEFINITIVA DE APPDIGITALIZADOR EN WORKFLOW DE RADICACIÓN
> OBJETIVO
> Reemplazar el contenido placeholder actual de Captura de Documentos por AppDigitalizador completo, reutilizando exactamente la implementación validada en Sandbox.
> NO reconstruir funcionalidades.
> NO duplicar lógica.
> NO crear una segunda versión del digitalizador.
> La integración debe reutilizar el mismo AppDigitalizador ya probado.
> ====================================================================
> ARQUITECTURA OBLIGATORIA
> La pantalla debe quedar compuesta por:
> Toolbar superior de Captura
> 
> Toolbar principal AppDigitalizador
> 
> AppTreeTable lateral
> 
> Preview PDF central
> 
> Miniaturas
> 
> Configuración de escaneo
> 
> ====================================================================
> DISTRIBUCIÓN
> ┌──────────────────────────────────────────────┐│ Toolbar Captura │└──────────────────────────────────────────────┘
> ┌──────────────────────────────────────────────┐│ Toolbar AppDigitalizador │└──────────────────────────────────────────────┘
> ┌───────────┬───────────────────────┬──────────┐│ │ │ ││TreeTable │ Preview PDF │Miniaturas││ │ │ │└───────────┴───────────────────────┴──────────┘
> ====================================================================
> COMPORTAMIENTO CRÍTICO
> APPTREETABLE DEBE FUNCIONAR COMO GMAIL
> Esto es obligatorio.
> NO se debe desmontar el visor.
> NO se debe reconstruir el visor.
> NO se debe perder estado.
> NO se debe reinicializar PDF.
> NO se debe reinicializar scanner.
> NO se debe regenerar preview.
> NO se debe perder zoom.
> NO se debe perder página actual.
> NO se debe perder selección.
> ====================================================================
> COMPORTAMIENTO ESPERADO
> Estado inicial:
> ┌───────────┬───────────────────────┬──────────┐│TreeTable │ Preview PDF │Miniaturas│└───────────┴───────────────────────┴──────────┘
> Usuario colapsa TreeTable:
> ┌───────────────────────┬──────────┐│ Preview PDF │Miniaturas│└───────────────────────┴──────────┘
> Usuario vuelve a abrir TreeTable:
> ┌───────────┬───────────────────────┬──────────┐│TreeTable │ Preview PDF │Miniaturas│└───────────┴───────────────────────┴──────────┘
> SIN RECONSTRUIR NADA.
> ====================================================================
> PROHIBIDO
> NO usar:
> {treeVisible ? (
> ) : null}
> NO usar:
> {showPreview ? (
> ) : null}
> NO usar:
> {showScanner ? (
> ) : null}
> NO usar render condicional destructivo.
> ====================================================================
> IMPLEMENTACIÓN REQUERIDA
> Mantener montados permanentemente:
> AppTreeTable
> 
> Preview PDF
> 
> Scanner Workspace
> 
> Miniaturas
> 
> Configuración
> 
> Usar únicamente:
> CSSvisibilityopacitywidthflex-basistransform
> o mecanismos equivalentes.
> ====================================================================
> PATRÓN OBLIGATORIO
> Similar al patrón ya implementado en:
> CapDocument
> DigitalizacionWorkspace
> PdfViewerWorkspace
> donde ambas capas permanecen montadas.
> ====================================================================
> TOOLBAR SUPERIOR
> Mantener:
> [Escanear/Nuevo][Insertar ▼][Agregar ▼][Reemplazar][Configuración]
> Ubicación:
> Primer toolbar.
> ====================================================================
> TOOLBAR DEL VISOR
> Mantener todos los controles actuales del Sandbox:
> Rotar izquierda
> Rotar derecha
> Seleccionar área
> Eliminar
> Duplicar
> Deskew
> Zoom -
> Zoom +
> Fit Width
> Fit Page
> Pantalla completa
> Organizar páginas
> Navegación
> Página actual
> Búsqueda de página
> Ubicación:
> Segundo toolbar.
> ====================================================================
> MINIATURAS
> Mantener exactamente la implementación validada en Sandbox.
> Debe conservar:
> selección múltiple
> 
> seleccionar todo
> 
> drag & drop
> 
> navegación
> 
> organización
> 
> ====================================================================
> CONFIGURACIÓN
> No ocupar espacio permanente.
> Implementar panel colapsable.
> Debe abrirse sin reconstruir:
> preview
> 
> scanner
> 
> miniaturas
> 
> ====================================================================
> VALIDACIONES OBLIGATORIAS
> Abrir documento.
> Cambiar zoom.
> Ir a página 20.
> Ocultar AppTreeTable.
> Validar:
> mismo zoom
> 
> misma página
> 
> mismo documento
> 
> misma selección
> 
> Mostrar AppTreeTable nuevamente.
> Validar:
> no hubo remount
> 
> no hubo reinicialización
> 
> ====================================================================
> VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> ====================================================================
> DOCUMENTAR
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-299-radicacion-integration.md
> Incluir:
> arquitectura final
> 
> estrategia tipo Gmail
> 
> preservación de estado
> 
> preservación de componentes montados
> 
> validaciones realizadas
> 
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
