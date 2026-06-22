## Context

SCRUMCORE-259: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- MODERNIZACION-ESCANEO

## Jira Details

> MODERNIZACIÓN DE EXPERIENCIA DE ESCANEO Y PROCESAMIENTO
> CONTEXTO
> La auditoría determinó que el diálogo:
> PaperStream IPEn digitalizaciónPágina XCancelar
> es renderizado por el driver nativo PaperStream IP y disparado por Dynamsoft Web TWAIN mediante AcquireImage().
> Por tanto:
> NO puede personalizarse desde React.
> NO puede modificarse visualmente desde DocuArchi.
> Sin embargo, DocuArchi sí controla completamente:
> Scanner Status
> 
> Preview PDF
> 
> Toolbar
> 
> Miniaturas
> 
> Overlay de carga
> 
> Procesamiento posterior
> 
> OBJETIVO
> Modernizar la experiencia visual controlada por DocuArchi.
> ==================================================
> FASE 1
> AUDITORÍA DE EVENTOS DYNAMSOFT
> Investigar si existen eventos disponibles para:
> Página adquirida
> 
> Página procesada
> 
> Avance de escaneo
> 
> Estado de adquisición
> 
> Determinar si puede obtenerse:
> Página actualTotal de páginas
> durante AcquireImage.
> ==================================================
> FASE 2
> NUEVO OVERLAY DOCUARCHI
> Crear overlay corporativo.
> Diseño:
> 📄 Escaneando documentos
> Página actual
> Barra de progreso
> Estado actual
> Cancelar operación
> ==================================================
> FASE 3
> ESTADOS SOPORTADOS
> Escaneando
> Procesando imágenes
> Aplicando Deskew
> Aplicando Auto Crop
> Aplicando Auto Rotate
> Eliminando páginas en blanco
> Generando PDF
> Preparando documento
> ==================================================
> FASE 4
> ELIMINAR DUPLICIDAD VISUAL
> Actualmente existen:
> Loader Preview
> 
> Indicadores dispersos
> 
> Unificar experiencia.
> Mostrar un único estado visual consistente.
> ==================================================
> FASE 5
> OPTIMIZACIÓN DE VELOCIDAD PERCIBIDA
> Evaluar:
> Render bloqueante
> 
> Actualización de miniaturas
> 
> Regeneración de preview
> 
> Reconstrucción de páginas
> 
> Documentar oportunidades de mejora.
> ==================================================
> FASE 6
> DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-275-scan-progress-modernization.md
> Incluir:
> Resultado auditoría PaperStream.
> 
> Limitaciones del driver.
> 
> Eventos disponibles.
> 
> Diseño propuesto.
> 
> Mockups.
> 
> Riesgos.
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

1. TBD

## Risks / Trade-offs

- TBD

## Migration Plan

1. TBD

## Open Questions

- TBD
