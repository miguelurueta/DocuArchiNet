## Context

SCRUMCORE-248: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- DIAGNOSTICO-CORRECCION-IMAGENES

## Jira Details

> FASE 1 - DIAGNÓSTICO Y CORRECCIÓN DE VISUALIZACIÓN DE PÁGINAS ESCANEADAS
> CONTEXTO
> La integración con Dynamsoft ya funciona:
> Detecta scanners.
> 
> Permite seleccionar scanner.
> 
> Escanea correctamente.
> 
> El contador de páginas aumenta.
> 
> El estado indica páginas capturadas.
> 
> Problema actual:
> Las miniaturas muestran placeholders.
> 
> El preview principal muestra placeholders.
> 
> No se visualizan las imágenes reales escaneadas.
> 
> OBJETIVO
> Determinar la causa raíz exacta por la que las imágenes capturadas no se muestran y corregirla.
> IMPORTANTE
> NO asumir soluciones.
> Primero diagnosticar.
> Luego corregir.
> NO modificar:
> Toolbar.
> 
> PDF.
> 
> Rotación.
> 
> Eliminación.
> 
> Scanner.
> 
> Dynamsoft initialization.
> 
> Licencias.
> 
> Flujo de captura.
> 
> INVESTIGACIÓN OBLIGATORIA
> Determinar exactamente:
> Qué devuelve Dynamsoft después de AcquireImage().
> 
> Cómo se construye cada objeto Page.
> 
> Qué propiedades contiene cada página:
> Blob
> 
> Base64
> 
> DataURL
> 
> Canvas
> 
> ImageIndex
> 
> Thumbnail
> 
> Preview
> 
> Cualquier otra referencia visual.
> 
> En qué archivo se crean las páginas.
> 
> En qué archivo se almacenan.
> 
> En qué archivo se transforman.
> 
> En qué archivo se renderizan.
> 
> CONFIRMAR CON EVIDENCIA
> Identificar cuál de estos escenarios ocurre:
> A)La imagen existe pero nunca se renderiza.
> B)La imagen se pierde durante una transformación.
> C)La imagen nunca se genera.
> D)La imagen existe pero el componente usa placeholders en lugar de la imagen real.
> LOGS TEMPORALES
> Agregar:
> PAGE_CAPTUREDPAGE_OBJECTPAGE_STATEPAGE_IMAGE_DATAPAGE_THUMBNAIL_RENDERPAGE_PREVIEW_RENDER
> Los logs deben mostrar el objeto completo de la página y la información visual disponible.
> CORRECCIÓN
> Una vez encontrada la causa raíz:
> Corregir miniaturas.
> 
> Corregir preview principal.
> 
> Mostrar imágenes reales capturadas.
> 
> Mantener selección actual.
> 
> Mantener scroll actual.
> 
> Mantener placeholders únicamente cuando no existan páginas.
> 
> ENTREGABLE
> Diagnóstico técnico.
> 
> Causa raíz exacta.
> 
> Flujo completo:
> 
> AcquireImage→ Construcción Page→ Estado→ Render miniatura→ Render preview
> Archivos involucrados.
> 
> Código modificado.
> 
> Logs agregados.
> 
> Validaciones ejecutadas:
> 
> npx tsc --noEmiteslintvitest
> No implementar cambios visuales adicionales.No rediseñar la interfaz.No tocar toolbar.
> El único objetivo de esta fase es entender por qué no se muestran las imágenes escaneadas y dejar miniaturas + preview funcionando correctamente.

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
