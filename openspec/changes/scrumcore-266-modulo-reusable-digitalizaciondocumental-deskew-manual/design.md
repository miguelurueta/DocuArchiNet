## Context

SCRUMCORE-266: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-DESKEW-MANUAL

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

## Goals / Non-Goals

**Goals**
- Exponer Deskew manual en el toolbar de pagina y en el organizador.
- Aplicar Deskew a la pagina activa o a la seleccion multiple, segun el estado actual.
- Reutilizar la misma integracion de Deskew usada durante el procesamiento automatico.
- Refrescar preview, miniaturas, organizador, navegacion e invalidar el PDF generado.
- Mostrar overlay corporativo durante la correccion.

**Non-Goals**
- Implementar un algoritmo propio de deskew en frontend.
- Cambiar la configuracion de Deskew automatico durante captura.
- Modificar contratos backend de guardado o adjunto.

## Decisions

1. Se agrega `deskewPage(pageId)` al contrato `DigitalizacionScannerClient` para mantener la accion reusable desde workspace, modal y `AppDigitalizador`.
2. `DynamsoftTwainClient.deskewPage` reutiliza la entrada `deskew` de `automaticProcessingFeatures`; no duplica nombres de metodos ni algoritmos.
3. Cuando el runtime no expone una API nativa compatible, el cliente mantiene las paginas actuales y registra el resultado como `unsupported`, igual que el procesamiento automatico.
4. El hook `useDigitalizacionScanner` usa el estado `processingPage` y el stage `applyingDeskew` para activar el overlay "Corrigiendo inclinacion".
5. Las operaciones masivas de Deskew se ejecutan secuencialmente en orden visual para evitar que respuestas concurrentes sobrescriban el estado de paginas.

## Risks / Trade-offs

- El resultado final depende de las capacidades nativas expuestas por Dynamsoft Web TWAIN en la estacion del usuario.
- En runtimes sin Deskew nativo, la accion no modifica la imagen; se mantiene como no destructiva para cumplir el requisito de no fallar cuando no hay correccion aplicable.
- El procesamiento multiple reutiliza llamadas por pagina para conservar el contrato existente de pagina estable; esto prioriza consistencia sobre paralelismo.

## Migration Plan

1. Extender contrato, hook y cliente Dynamsoft con `deskewPage`.
2. Agregar accion UI junto a rotacion y en el toolbar del organizador.
3. Actualizar mocks y pruebas de hook, cliente y `AppDigitalizador`.
4. Documentar arquitectura en `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-293-manual-deskew.md`.

## Open Questions

- Ninguna abierta para implementacion frontend.
