## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-DESKEW-MANUAL. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-266.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> DESKEW MANUAL POSTERIOR AL ESCANEO
> CONTEXTO
> Actualmente existe la opción:
> Deskew
> dentro de la configuración de captura.
> Esta funcionalidad se ejecuta únicamente durante el proceso de escaneo.
> Sin embargo, pueden existir páginas que:
> No fueron corregidas correctamente.
> 
> Fueron importadas desde imágenes.
> 
> Fueron agregadas posteriormente.
> 
> Presentan inclinación residual.
> 
> Se requiere una herramienta manual para corregir inclinación después de capturada la página.
> ==================================================
> OBJETIVO
> Permitir ejecutar Deskew manual sobre páginas ya existentes.
> ==================================================
> TOOLBAR
> Agregar botón:
> Deskew
> Ubicación:
> Junto a:
> Rotar izquierda
> Rotar derecha
> ==================================================
> TOOLTIP
> "Corregir inclinación de la página"
> ==================================================
> COMPORTAMIENTO
> Página activa
> ↓
> Deskew
> ↓
> Procesar página
> ↓
> Actualizar resultado
> ==================================================
> SELECCIÓN SIMPLE
> Si existe una única página activa:
> Aplicar Deskew únicamente a dicha página.
> ==================================================
> SELECCIÓN MÚLTIPLE
> Si existen varias páginas seleccionadas:
> Aplicar Deskew a todas las páginas seleccionadas.
> ==================================================
> ACTUALIZAR
> Después de procesar:
> ✓ Preview
> ✓ Miniatura
> ✓ Organizador
> ✓ Navegación
> ==================================================
> COMPATIBILIDAD
> Debe funcionar con:
> ✓ Escaneos nuevos
> ✓ Imágenes importadas
> ✓ PDF importados
> ✓ Páginas duplicadas
> ✓ Páginas insertadas
> ✓ Páginas reemplazadas
> ==================================================
> INDICADOR VISUAL
> Mientras se ejecuta:
> Mostrar overlay corporativo.
> Mensaje:
> Corrigiendo inclinación...
> ==================================================
> ERRORES
> Si la página ya está correctamente alineada:
> No generar error.
> Mantener página actual.
> ==================================================
> ARQUITECTURA
> Reutilizar la misma lógica Deskew utilizada actualmente durante la captura.
> NO implementar una segunda versión.
> NO duplicar algoritmos.
> ==================================================
> DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-293-manual-deskew.md
> Documentar:
> Flujo.
> 
> Compatibilidad.
> 
> Reutilización del motor existente.
> 
> Casos de uso.
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
- Labels: DESKEW, DIGITALIZACIONDOCUMENTAL, MANUAL, MODULOS, REUSABLE

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-deskew-manual`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
