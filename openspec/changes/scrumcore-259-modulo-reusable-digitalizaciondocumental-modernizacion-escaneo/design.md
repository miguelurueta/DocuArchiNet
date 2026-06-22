## Context

SCRUMCORE-259: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- MODERNIZACION-ESCANEO

## Jira Details

> MODERNIZACIÓN DE EXPERIENCIA DE ESCANEO Y PROCESAMIENTO
> CONTEXTO
> La auditoría determinó que el diálogo:
> PaperStream IPEn digitalizaciónPágina XCancelar
> es renderizado por el driver nativo PaperStream IP y disparado por Dynamsoft Web TWAIN mediante AcquireImage().
> Por tanto:
> NO puede personalizarse desde React.
> NO puede modificarse visualmente desde DocuArchi.
> Sin embargo, DocuArchi sí controla completamente:
> Scanner Status
> 
> Preview PDF
> 
> Toolbar
> 
> Miniaturas
> 
> Overlay de carga
> 
> Procesamiento posterior
> 
> OBJETIVO
> Modernizar la experiencia visual controlada por DocuArchi.
> ==================================================
> FASE 1
> AUDITORÍA DE EVENTOS DYNAMSOFT
> Investigar si existen eventos disponibles para:
> Página adquirida
> 
> Página procesada
> 
> Avance de escaneo
> 
> Estado de adquisición
> 
> Determinar si puede obtenerse:
> Página actualTotal de páginas
> durante AcquireImage.
> ==================================================
> FASE 2
> NUEVO OVERLAY DOCUARCHI
> Crear overlay corporativo.
> Diseño:
> 📄 Escaneando documentos
> Página actual
> Barra de progreso
> Estado actual
> Cancelar operación
> ==================================================
> FASE 3
> ESTADOS SOPORTADOS
> Escaneando
> Procesando imágenes
> Aplicando Deskew
> Aplicando Auto Crop
> Aplicando Auto Rotate
> Eliminando páginas en blanco
> Generando PDF
> Preparando documento
> ==================================================
> FASE 4
> ELIMINAR DUPLICIDAD VISUAL
> Actualmente existen:
> Loader Preview
> 
> Indicadores dispersos
> 
> Unificar experiencia.
> Mostrar un único estado visual consistente.
> ==================================================
> FASE 5
> OPTIMIZACIÓN DE VELOCIDAD PERCIBIDA
> Evaluar:
> Render bloqueante
> 
> Actualización de miniaturas
> 
> Regeneración de preview
> 
> Reconstrucción de páginas
> 
> Documentar oportunidades de mejora.
> ==================================================
> FASE 6
> DOCUMENTACIÓN
> Crear:
> docs/Architecture/DigitalizacionDocumental/SCRUMCORE-275-scan-progress-modernization.md
> Incluir:
> Resultado auditoría PaperStream.
> 
> Limitaciones del driver.
> 
> Eventos disponibles.
> 
> Diseño propuesto.
> 
> Mockups.
> 
> Riesgos.
> 
> ==================================================
> VALIDAR
> npx tsc --noEmit
> eslint
> vitest
> IMPLEMENTAR

## Goals / Non-Goals

**Goals**
- Modernizar la experiencia visual controlada por DocuArchi durante escaneo y procesamiento.
- Centralizar los estados de scanner, procesamiento, preview y PDF en un unico overlay.
- Documentar la limitacion del dialogo nativo PaperStream y la auditoria de eventos disponible en el contrato local.

**Non-Goals**
- Personalizar el dialogo nativo PaperStream IP renderizado por el driver.
- Cambiar la configuracion fisica del scanner o el contrato backend de digitalizacion.

## Decisions

1. Se agrega `ScanProgressSnapshot` como contrato de progreso del scanner.
2. `useDigitalizacionScanner` conserva estados gruesos (`scanning`, `generatingPdf`) y expone `progress` para detalle visual.
3. `DynamsoftTwainClient` reporta progreso solo en fases controladas por DocuArchi: adquisicion indeterminada, construccion de paginas, blank-page removal, Deskew, Auto Crop, Auto Rotate, generacion PDF y preparacion final.
4. El overlay vive sobre el panel Preview PDF para evitar loaders duplicados en toolbar, miniaturas o footer.
5. La presentacion final del overlay es minimalista: loader Contasoft, un unico texto visible (`Escaneando documentos`, `Procesando documentos` o `Generando PDF`) y boton de cancelacion cuando aplica.
6. No se exponen barras, porcentajes, paginas actuales/totales ni detalles tecnicos internos en la UI.

## Risks / Trade-offs

- Durante `AcquireImage()` no hay pagina actual/total confiable en el contrato TypeScript local; el overlay muestra avance indeterminado.
- El boton de cancelacion del overlay cancela el flujo DocuArchi; la cancelacion del driver sigue dependiendo de PaperStream cuando el dialogo nativo esta activo.
- Los eventos futuros de Dynamsoft deben conectarse al mismo contrato de progreso para no duplicar UI.

## Migration Plan

1. Extender tipos de scanner con progreso opcional.
2. Emitir progreso desde hook y cliente Dynamsoft sin romper clientes existentes.
3. Renderizar overlay unico en `DigitalizacionDocumentalWorkspace`.
4. Cubrir el contrato con pruebas unitarias focales y documentar la auditoria.

## Open Questions

- Confirmar en QA con hardware PaperStream real si el driver expone eventos runtime adicionales fuera del contrato local.
