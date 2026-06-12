## Context

SCRUMCORE-249: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- HERRAMIENTAS-DYNAMSOFT

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

## Goals / Non-Goals

**Goals**
- Auditar el estado real de capacidades Dynamsoft y del flujo actual de digitalizacion.
- Identificar origen probable de problemas de orientacion y duplex sin modificar comportamiento.
- Documentar brechas entre capacidades disponibles, contrato tecnico y UI actual.
- Auditar toolbar/layout actual y proponer panel derecho de configuracion de escaneo.
- Evaluar modo DocuArchi vs modo Driver Scanner.
- Implementar el rediseno autorizado del toolbar, panel derecho y layout.

**Non-Goals**
- No modificar arquitectura del digitalizador reutilizable.
- No cambiar generacion PDF, upload temporal, metadata backend ni seleccion de scanner.
- No cambiar comportamiento interno de Dynamsoft, PDF ni rotacion.
- No implementar capacidades avanzadas adicionales como auto-rotate, deskew, blank-page removal o persistencia de preferencias.

## Decisions

1. SCRUMCORE-249 se inicio como auditoria tecnica y posteriormente fue autorizado para implementacion UI controlada.
2. El entregable principal queda documentado en `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-249-dynamsoft-capabilities-audit.md`.
3. La configuracion de `AcquireImage` queda controlada desde el panel derecho:
   - `IfShowUI: true` solo en modo Driver Scanner
   - `IfFeederEnabled` segun ADF en modo DocuArchi
   - `IfDuplexEnabled` segun Duplex en modo DocuArchi
   - `Resolution: 200` por defecto
   - `PixelType: color` por defecto
4. El duplex queda desactivado por defecto para no alterar flujos simplex, pero ahora puede habilitarse desde UI.
5. La orientacion no se corrige automaticamente porque debe confirmarse con evidencia real de `PAGE_DIMENSIONS`, `THUMBNAIL_DIMENSIONS` y `PREVIEW_DIMENSIONS` en scanner fisico.
6. El panel Metadata fue reemplazado por un panel compacto de Configuracion de Escaneo, manteniendo la columna derecha.
7. El toolbar contiene acciones de comando, no configuraciones avanzadas.
8. El modo Driver Scanner usa `IfShowUI: true`, sujeto a validacion fisica con PaperStream IP.
9. El preview PDF recibe mayor proporcion de la grilla para convertirse en el area principal de trabajo.

## Risks / Trade-offs

- Activar duplex por defecto puede duplicar paginas en flujos que esperan simplex.
- Auto-rotar sin evidencia puede dañar documentos que intencionalmente fueron capturados horizontales.
- Exponer muchas capacidades Dynamsoft sin normalizar contrato puede acoplar la UI al SDK.
- Los logs diagnosticos actuales son utiles para auditoria, pero deben estabilizarse antes de produccion.
- Mover metadata fuera del panel derecho puede afectar flujos que dependan de verla durante captura.
- Usar UI nativa PaperStream puede introducir comportamiento no deterministico desde DocuArchi.

## Migration Plan

1. Mantener estado actual para captura basica.
2. Validar en entorno fisico los logs de orientacion y duplex.
3. En una fase posterior, crear configuracion UI controlada para duplex/color/DPI/ADF.
4. Modelar capacidades avanzadas con contrato propio antes de exponerlas en UI.
5. Validar fisicamente que ADF, duplex, color, DPI y modo Driver Scanner producen el resultado esperado con Fujitsu fi-7160.
6. Definir persistencia futura de configuraciones por usuario o modulo.

## Open Questions

- Confirmar con scanner fisico si `PAGE_DIMENSIONS` llega portrait o landscape en documentos que se ven horizontales.
- Confirmar si el driver PaperStream reporta capacidades duplex/ADF mediante propiedades disponibles en DWT.
- Definir si duplex debe ser opcion manual, default por contexto o configuracion por modulo.
- Definir si Metadata pasa a footer, panel colapsable o paso de confirmacion.
- Definir si configuraciones de escaneo se persisten por usuario, modulo o sesion.
