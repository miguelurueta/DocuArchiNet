## Context

SCRUMCORE-247: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- ACTUALIZACION-SDK

## Jira Details

> SCRUMCORE-247
> Alineación SDK Dynamsoft 19.3.2
> Contexto
> La sandbox AppDigitalizador ya funciona.
> Se validó:
> licencia;
> 
> carga JS;
> 
> carga CSS;
> 
> runtime;
> 
> servicio local.
> 
> Error detectado:
> "Please update your document scanning service"
> Diagnóstico confirmado:
> Frontend:dwt@18.5.0
> Servicio instalado:1.9.3.1028
> TWAIN Module:19.3.2.0306
> Existe desalineación de versiones.
> Objetivo
> Actualizar la integración frontend para utilizar la misma familia instalada en Windows:
> dwt@19.3.2
> No modificar lógica funcional.
> No reescribir adapter.
> No cambiar arquitectura AppDigitalizador.
> Analizar y ajustar
> package.json
> 
> dynamsoft.constants.ts
> 
> loadDynamsoftScripts.ts
> 
> DynamsoftTwainClient.ts
> 
> tests asociados
> 
> Validaciones obligatorias
> Verificar que continúan funcionando:
> runtime.ProductKey
> 
> runtime.ResourcesPath
> 
> runtime.Load()
> 
> runtime.GetWebTwain()
> 
> SourceCount
> 
> GetSourceNameItems()
> 
> SelectSourceByIndex()
> 
> OpenSource()
> 
> AcquireImage()
> 
> CloseSource()
> 
> Rotate()
> 
> RemoveImage()
> 
> RemoveAllImages()
> 
> ConvertToBlob("application/pdf")
> 
> CSS
> Validar rutas reales para 19.3.2:
> dynamsoft.webtwain.css
> 
> dynamsoft.webtwain.viewer.css
> 
> Actualizar ResourcesPath si es necesario.
> Entregables
> Archivos modificados.
> 
> Cambios realizados.
> 
> Riesgos encontrados.
> 
> Resultado de pruebas.
> 
> Confirmar compatibilidad con AppDigitalizador.
> 
> Confirmar compatibilidad con DigitalizacionDocumentalWorkspace.
> 
> Confirmar compatibilidad con DigitalizacionDocumentalModal.
> 
> Pruebas finales
> Abrir:
> /__sandbox/app-digitalizador
> Validar:
> desaparición del mensaje "Please update your document scanning service";
> 
> listado de scanners;
> 
> selección de scanner;
> 
> escaneo real;
> 
> miniaturas;
> 
> preview;
> 
> generación PDF.
> 
> No implementar funcionalidades nuevas.Solo alinear SDK y servicio a la versión 19.3.2.

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
