# Jira Context - SCRUMCORE-267

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- ELIMINAR-REDUNDANCIA

## Description

> ELIMINAR RESUMEN REDUNDANTE DE CONFIGURACIÓN DE ESCANEO
> CONTEXTO
> Actualmente el panel "Configuración de Escaneo" muestra dos veces la misma información:
> Controles reales:
> ADF
> 
> Duplex
> 
> Eliminar páginas en blanco
> 
> Deskew
> 
> Auto Crop
> 
> Auto Rotate
> 
> Color
> 
> Resolución
> 
> Resumen visual inferior:
> ADF si
> 
> Duplex si
> 
> Blancas si
> 
> Deskew si
> 
> Crop si
> 
> AutoRot si
> 
> Color
> 
> 600 dpi
> 
> Este resumen duplica información ya visible en los controles.
> OBJETIVO
> Eliminar completamente el bloque de chips/resumen ubicado al final del panel de configuración.
> REQUISITOS
> Eliminar renderizado.
> 
> Eliminar estilos asociados que queden sin uso.
> 
> Eliminar lógica de construcción del resumen.
> 
> Mantener intacta la configuración funcional.
> 
> NO MODIFICAR
> Checkboxes.
> 
> Selectores.
> 
> Resolución.
> 
> Color.
> 
> Configuración de captura.
> 
> RESULTADO ESPERADO
> El panel finaliza inmediatamente después del selector de resolución y demás configuraciones sin mostrar chips de resumen.
> VALIDAR
> tsc
> 
> eslint
> 
> vitest
> 
> DOCUMENTAR
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-294-remove-scan-summary.md

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, ELIMINAR, MODULOS, REDUNDANCIA, REUSABLE
