## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- SECCION-AREA-RECORTE-MANUAL. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-257.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> SELECCIÓN DE ÁREA Y RECORTE MANUAL
> CONTEXTO
> Actualmente el módulo permite:
> ✓ Escanear documentos✓ Visualizar Preview PDF✓ Organizar páginas✓ Rotar páginas✓ Eliminar páginas✓ Reordenar páginas✓ Zoom
> Se requiere incorporar una herramienta de selección visual que permita posteriormente ejecutar acciones sobre una región específica de una página.
> IMPORTANTE
> Esta funcionalidad será la base futura para:
> Recorte manual
> 
> OCR por zona
> 
> Firmas
> 
> Sellos
> 
> Anotaciones
> 
> Redacción de datos sensibles
> 
> ==================================================FASE 1AUDITORÍA
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-269-selection-crop.md
> Documentar:
> Arquitectura actual del Preview PDF.
> 
> Eventos de mouse disponibles.
> 
> Sistema de coordenadas.
> 
> Escalado actual del visor.
> 
> Zoom actual.
> 
> Determinar:
> Cómo almacenar una selección independientemente del zoom.
> ==================================================FASE 2NUEVO BOTÓN TOOLBAR
> Agregar:
> ✂ Seleccionar área
> Tooltip:
> "Seleccionar área"
> Estados:
> INACTIVO
> ACTIVO
> Cuando está activo:
> El cursor cambia.
> Se habilita modo selección.
> ==================================================FASE 3SELECCIÓN VISUAL
> Permitir:
> mousedown
> ↓
> drag
> ↓
> mouseup
> Mostrar rectángulo visual.
> Ejemplo:
> ┌──────────────────────┐│                      ││   ┌─────────────┐    ││   │ Selección   │    ││   └─────────────┘    ││                      │└──────────────────────┘
> ==================================================FASE 4PANEL DE ACCIONES
> Una vez exista selección:
> Mostrar:
> ✂ Recortar
> ↺ Reiniciar selección
> ✕ Cancelar
> ==================================================FASE 5RECORTE
> Al pulsar:
> ✂ Recortar
> Aplicar crop únicamente a:
> Página seleccionada.
> Actualizar:
> ✓ Preview✓ Miniatura✓ Organizador de páginas✓ PDF pendiente
> ==================================================FASE 6ZOOM
> La selección debe funcionar correctamente con:
> Zoom -
> Zoom +
> Fit Width
> Fit Page
> Pantalla completa
> ==================================================FASE 7PERSISTENCIA
> Mientras no se aplique:
> La selección puede modificarse.
> La selección puede moverse.
> La selección puede cancelarse.
> ==================================================FASE 8RENDIMIENTO
> NO regenerar documento completo.
> NO regenerar todas las miniaturas.
> NO reconstruir scanner.pages completo.
> Actualizar únicamente:
> La página afectada.
> ==================================================FASE 9PREPARACIÓN FUTURA
> Diseñar el modelo para soportar posteriormente:
> OCR por selección.
> Firmas.
> Sellos.
> Anotaciones.
> Ocultamiento de información.
> La selección debe almacenarse como:
> xywidthheight
> respecto a dimensiones reales de la página.
> NO respecto al zoom actual.
> ==================================================VALIDACIONES
> Página vertical.
> Página horizontal.
> Documento de identidad.
> A4.
> Zoom 50%.
> Zoom 100%.
> Zoom 200%.
> Pantalla completa.
> ==================================================DOCUMENTACIÓN
> Actualizar:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-269-selection-crop.md
> Incluir:
> Arquitectura.
> 
> Flujo.
> 
> Modelo de coordenadas.
> 
> Riesgos.
> 
> Evidencia visual.
> 
> ==================================================VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> IMPLEMENTAR.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: AREA, DIGITALIZACIONDOCUMENTAL, DYNAMSOFT, MANUAL, MODULOS, RECORTE, REUSABLE, SECCION

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-seccion-area-recorte-manual`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
