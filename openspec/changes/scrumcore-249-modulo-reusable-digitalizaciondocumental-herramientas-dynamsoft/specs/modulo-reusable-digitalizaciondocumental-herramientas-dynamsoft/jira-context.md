# Jira Context - SCRUMCORE-249

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- HERRAMIENTAS-DYNAMSOFT

## Description

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

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, DYNAMSOFT, HERRAMIENTAS, MODULOS, REUSABLE
