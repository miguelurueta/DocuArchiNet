## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- HERRAMIENTAS-DYNAMSOFT. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-249.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.
- Se completa la auditoria tecnica sin cambios funcionales, segun restriccion del ticket.
- Se documenta el estado actual de orientacion, duplex, ADF, configuracion `AcquireImage` y capacidades Dynamsoft.
- Se incorpora auditoria adicional de toolbar, panel derecho, configuracion de escaneo y layout objetivo.
- Se propone estrategia para modo DocuArchi y modo Driver Scanner sin modificar codigo.
- Actualizacion de alcance: despues de la auditoria, el usuario autorizo implementar el toolbar, panel derecho y layout recomendados.
- Se reemplaza el panel Metadata por Configuracion de Escaneo dentro del workspace inline.
- Se exponen opciones controladas de ADF, duplex, color y resolucion, y modo Driver Scanner con UI nativa.

## Resultado de auditoria

- Entregable principal: `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-249-dynamsoft-capabilities-audit.md`.
- La auditoria inicial no modifico comportamiento de escaneo, preview, PDF, toolbar ni rotacion.
- Causa raiz duplex: el flujo actual no envia `duplex: true`; por tanto `IfDuplexEnabled` queda en `false`.
- Orientacion: no hay rotacion CSS detectada en el workspace; la decision requiere validar `PAGE_DIMENSIONS`, `THUMBNAIL_DIMENSIONS` y `PREVIEW_DIMENSIONS` con scanner fisico.
- UX propuesta: reemplazar Metadata por panel lateral de Configuracion de Escaneo, mantener toolbar limpio y ampliar protagonismo del Preview PDF.

## Resultado de implementacion

- `DigitalizacionDocumentalWorkspace` mantiene el layout inline reutilizable.
- El toolbar superior queda enfocado en comandos: escanear, rotar izquierda/derecha, eliminar, limpiar y generar PDF.
- La columna derecha queda como Configuracion de Escaneo con selector de scanner, modo DocuArchi y modo Driver Scanner.
- `DynamsoftTwainClient.scan()` recibe `IfShowUI`, `IfFeederEnabled`, `IfDuplexEnabled`, `Resolution` y `PixelType` desde las opciones del workspace.
- El preview PDF gana mayor proporcion horizontal al retirar Metadata del panel derecho.

## Jira Details

> ESCANEO Y CAPACIDADES DYNAMSOFT
> CONTEXTO
> La integración actual ya permite:
> Inicializar Dynamsoft correctamente.
> 
> Detectar scanners.
> 
> Seleccionar scanner.
> 
> Escanear documentos.
> 
> Mostrar miniaturas.
> 
> Generar PDF.
> 
> Actualmente se utiliza un Fujitsu fi-7160.
> OBJETIVO
> Realizar una auditoría técnica completa de las capacidades reales disponibles en:
> Dynamsoft Web TWAIN.
> 
> Fujitsu fi-7160.
> 
> Flujo actual de digitalización.
> 
> IMPORTANTE
> NO IMPLEMENTAR.
> NO MODIFICAR CÓDIGO.
> NO CAMBIAR COMPORTAMIENTO.
> SOLO DIAGNÓSTICO TÉCNICO.
> AUDITORÍA DE ORIENTACIÓN DE PÁGINAS
> 
> Problema observado:
> Algunos documentos y miniaturas se visualizan horizontalmente cuando visualmente deberían verse verticales.
> Investigar:
> Dimensiones originales capturadas.
> 
> Orientación original de las imágenes.
> 
> Rotaciones aplicadas.
> 
> Transformaciones aplicadas.
> 
> Generación de miniaturas.
> 
> Generación de preview.
> 
> Determinar:
> Si el problema viene del scanner.
> 
> Si el problema viene de Dynamsoft.
> 
> Si el problema viene de CSS.
> 
> Si el problema viene del preview.
> 
> Si el problema viene del proceso de generación de miniaturas.
> 
> Entregable:
> Causa raíz exacta.
> 
> Punto exacto del código involucrado.
> 
> Corrección recomendada.
> 
> AUDITORÍA DE ESCANEO DÚPLEX
> 
> Problema observado:
> El fi-7160 soporta escaneo por ambas caras, pero actualmente solo se captura una cara.
> Investigar:
> Configuración actual de AcquireImage.
> 
> IfDuplexEnabled.
> 
> IfFeederEnabled.
> 
> IfShowUI.
> 
> AutoFeed.
> 
> Duplex.
> 
> Flatbed.
> 
> Determinar:
> Si actualmente se usa simplex.
> 
> Si actualmente se usa duplex.
> 
> Si el scanner reporta soporte duplex.
> 
> Si el código habilita duplex.
> 
> Si Dynamsoft detecta la capacidad.
> 
> Entregable:
> Configuración actual.
> 
> Configuración recomendada.
> 
> Evidencia técnica.
> 
> AUDITORÍA DE CAPACIDADES DYNAMSOFT
> 
> Investigar soporte disponible para:
> Escaneo:
> Show Scanner UI
> 
> Use ADF
> 
> Duplex
> 
> Flatbed
> 
> Color
> 
> Gray
> 
> B&W
> 
> Resolution
> 
> Brightness
> 
> Contrast
> 
> Procesamiento:
> Auto Rotate
> 
> Deskew
> 
> Auto Crop
> 
> Blank Page Detection
> 
> Blank Page Removal
> 
> Border Removal
> 
> Visualización:
> Zoom In
> 
> Zoom Out
> 
> Fit Width
> 
> Fit Page
> 
> Rotate Left
> 
> Rotate Right
> 
> Documentos:
> Reordenar páginas
> 
> Duplicar páginas
> 
> Eliminar páginas
> 
> Exportar PDF
> 
> Exportar TIFF
> 
> Exportar JPG
> 
> Exportar PNG
> 
> Determinar:
> Qué ya existe.
> 
> Qué no existe.
> 
> Qué requiere desarrollo.
> 
> Qué soporta el fi-7160.
> 
> AUDITORÍA DEL FLUJO DE ESCANEO ACTUAL
> 
> Documentar:
> Scanner→ Selección→ Configuración→ AcquireImage→ Captura→ Miniatura→ Preview→ PDF
> Identificar:
> Configuración TWAIN utilizada actualmente.
> 
> Capacidades desaprovechadas.
> 
> Restricciones actuales.
> 
> ENTREGABLE FINAL
> Capacidades reales de Dynamsoft.
> 
> Capacidades reales del Fujitsu fi-7160.
> 
> Estado actual del soporte dúplex.
> 
> Estado actual del soporte ADF.
> 
> Estado actual de orientación.
> 
> Funcionalidades disponibles para exponer en UI.
> 
> Funcionalidades que requieren desarrollo.
> 
> Recomendaciones técnicas.
> 
> Validaciones:
> npx tsc --noEmit
> 
> eslint
> 
> vitest
> 
> NO IMPLEMENTAR.SOLO AUDITORÍA TÉCNICA.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, DYNAMSOFT, HERRAMIENTAS, MODULOS, REUSABLE

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-herramientas-dynamsoft`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
