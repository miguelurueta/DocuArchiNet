## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-AJUSTE -ZOOM. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-268.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> NORMALIZACIÓN DEL ZOOM DEL PREVIEW Y ELIMINACIÓN DE SALTOS VISUALES
> CONTEXTO
> Actualmente el preview de imágenes utiliza dos mecanismos distintos de visualización:
> fitPage
> 
> fitWidth
> 
> custom (previewZoom)
> 
> El problema actual es que al presionar:
> Zoom +
> o
> Zoom -
> mientras el usuario está en:
> fitPage
> o
> fitWidth
> el sistema ejecuta simultáneamente:
> setPreviewFitMode("custom")
> y
> setPreviewZoom(...)
> Esto genera un cambio brusco de estrategia de layout y un cambio de zoom en el mismo evento.
> El usuario percibe un salto visual muy grande aunque internamente previewZoom solo aumente 25%.
> ==================================================
> PROBLEMA IDENTIFICADO
> Actualmente:
> fitPage↓
> Zoom +
> ↓
> custom 125%
> Esto provoca:
> Cambio de layout.
> 
> Cambio de zoom.
> 
> en la misma interacción.
> ==================================================
> OBJETIVO
> Eliminar completamente los saltos visuales al salir de:
> fitPage
> fitWidth
> hacia
> custom
> ==================================================
> COMPORTAMIENTO ESPERADO
> FIT PAGE
> ↓
> Zoom +
> ↓
> Calcular primero el equivalente visual real.
> ↓
> Entrar a custom usando el zoom equivalente.
> ↓
> Aplicar únicamente el incremento de PREVIEW_ZOOM_STEP.
> ==================================================
> EJEMPLO
> Si visualmente:
> fitPage
> equivale a:
> 83%
> Entonces:
> Zoom +
> ↓
> custom 108%
> NO:
> custom 125%
> ==================================================
> FIT WIDTH
> ↓
> Zoom +
> ↓
> Determinar el tamaño visual real mostrado.
> ↓
> Convertirlo a porcentaje equivalente.
> ↓
> Entrar a custom desde ese valor.
> ==================================================
> NO RESETEAR
> No ejecutar:
> setPreviewZoom(100)
> durante la transición.
> ==================================================
> NUEVA REGLA
> Antes de cambiar a:
> custom
> calcular:
> equivalentZoom
> utilizando medidas reales del DOM.
> ==================================================
> MEDICIÓN
> Utilizar:
> getBoundingClientRect()
> sobre:
> .previewPageSurface
> .previewViewport
> ==================================================
> CALCULAR
> equivalentZoom
> basado en:
> Ancho renderizado actual
> vs
> Ancho base de la página
> ==================================================
> FLUJO NUEVO
> fitPage
> ↓
> Zoom +
> ↓
> equivalentZoom
> ↓
> custom
> ↓
> equivalentZoom + PREVIEW_ZOOM_STEP
> ==================================================
> fitPage
> ↓
> Zoom -
> ↓
> equivalentZoom
> ↓
> custom
> ↓
> equivalentZoom - PREVIEW_ZOOM_STEP
> ==================================================
> fitWidth
> ↓
> Zoom +
> ↓
> equivalentZoom
> ↓
> custom
> ↓
> equivalentZoom + PREVIEW_ZOOM_STEP
> ==================================================
> fitWidth
> ↓
> Zoom -
> ↓
> equivalentZoom
> ↓
> custom
> ↓
> equivalentZoom - PREVIEW_ZOOM_STEP
> ==================================================
> INDICADOR VISUAL DE ZOOM
> Agregar indicador permanente en toolbar.
> Ejemplo:
> [-] 125% [+]
> ==================================================
> MOSTRAR
> Modo actual:
> Fit Page
> Fit Width
> Custom
> ==================================================
> EJEMPLO
> Fit Page | 100%
> Custom | 125%
> Fit Width | 100%
> ==================================================
> OBJETIVO UX
> El usuario debe comprender:
> Qué modo está activo.
> 
> Qué porcentaje real está viendo.
> 
> Qué ocurrirá al aumentar o disminuir zoom.
> 
> ==================================================
> NO MODIFICAR
> MIN_PREVIEW_ZOOM
> MAX_PREVIEW_ZOOM
> PREVIEW_ZOOM_STEP
> ==================================================
> MANTENER
> const MIN_PREVIEW_ZOOM = 50;const MAX_PREVIEW_ZOOM = 200;const PREVIEW_ZOOM_STEP = 25;
> ==================================================
> COMPATIBILIDAD
> Validar:
> ✓ Preview normal
> ✓ Pantalla completa
> ✓ Organizador de páginas
> ✓ Navegación flotante
> ✓ Miniaturas visibles
> ✓ Miniaturas ocultas
> ✓ Documento de 1 página
> ✓ Documento de 100 páginas
> ==================================================
> DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-295-preview-zoom-normalization.md
> Documentar:
> Problema identificado.
> 
> Causa raíz.
> 
> Estrategia de cálculo.
> 
> Equivalencia fitPage/fitWidth/custom.
> 
> Casos de prueba.
> 
> Evidencia técnica.
> 
> ==================================================
> VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> IMPLEMENTAR

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: AJUSTE, DIGITALIZACIONDOCUMENTAL, MODULOS, REUSABLE, ZOOM

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-ajuste-zoom`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
