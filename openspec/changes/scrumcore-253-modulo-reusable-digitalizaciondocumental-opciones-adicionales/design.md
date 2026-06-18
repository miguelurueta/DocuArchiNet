## Context

SCRUMCORE-253: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- OPCIONES- ADICIOANLES

El digitalizador ya es reusable y cuenta con captura, preview, miniaturas, PDF, Drag & Drop, blank page removal y toolbar corporativo. Esta fase agrega procesamiento automatico de calidad documental.

## Goals

- Auditar capacidades reales de Dynamsoft Web TWAIN 19.3.2 antes de activar procesamiento destructivo.
- Agregar opciones de Deskew, Auto Crop y Auto Rotate en el panel lateral.
- Mantener las opciones actuales de ADF, Duplex, Blank Page Removal, Color y Resolucion.
- Procesar solo paginas afectadas.
- Reflejar cambios en miniaturas, preview y PDF final.
- Medir `DESKEW_TIME`, `AUTOCROP_TIME` y `AUTOROTATE_TIME`.
- No romper escaneo simplex, duplex, ADF, blank page removal, Drag & Drop, rotacion manual ni generacion PDF.

## Non-Goals

- No cambiar la arquitectura principal del digitalizador.
- No reemplazar Dynamsoft.
- No modificar endpoints backend.
- No cambiar el contrato de generacion PDF salvo lo estrictamente necesario para respetar paginas procesadas.
- No activar capacidades no soportadas por el SDK/runtime real.

## Proposed Approach

1. Auditar APIs disponibles en el runtime actual.
2. Modelar configuracion local de procesamiento automatico en el workspace.
3. Extender `ScanOptions` con flags opcionales.
4. Delegar procesamiento al scanner client.
5. Si Dynamsoft ofrece API nativa, usarla de forma aislada por pagina.
6. Si no existe API nativa para alguna capacidad, documentarla como no implementable sin procesamiento propio o libreria adicional.
7. Actualizar solo la pagina afectada y regenerar su thumbnail/preview.
8. Invalidar PDF cuando una pagina cambie.

## Decisions

1. La fuente de verdad tecnica sera el adapter `DynamsoftTwainClient`, no los componentes React.
2. El panel lateral concentra las opciones de procesamiento automatico.
3. Las opciones se mantienen desactivadas por defecto.
4. Las opciones son persistentes solo durante la sesion.
5. El procesamiento automatico no debe bloquear la captura completa si una capacidad no existe; debe fallar de forma controlada y documentada.
6. El preview y las miniaturas deben reflejar el resultado procesado.
7. La rotacion manual existente se conserva como accion independiente.

## Risks / Trade-offs

- Dynamsoft Web TWAIN podria no exponer Deskew, Auto Crop o Auto Rotate en el runtime usado por el proyecto.
- Algunas capacidades podrian requerir licencias adicionales.
- Auto Crop puede recortar contenido documental si la sensibilidad no se valida con documentos reales.
- Auto Rotate puede fallar con cedulas o documentos con poco texto.
- Procesamiento propio con canvas puede degradar calidad o rendimiento si se aplica a resolucion completa.
- Procesar grandes lotes puede afectar tiempo de captura si se hace sincronamente.

## Validation Plan

- Validar escaneo simplex.
- Validar escaneo duplex.
- Validar ADF.
- Validar Blank Page Removal.
- Validar Drag & Drop.
- Validar rotacion manual.
- Validar generacion PDF.
- Validar miniaturas y preview despues de procesar.
- Validar documentos inclinados 1, 5 y 10 grados.
- Validar carta, oficio, cedula y documentos pequenos.
- Validar documentos girados 90 y 180 grados.
- Ejecutar:
  - `npx tsc --noEmit`.
  - `npx eslint src/modules/digitalizacion src/app/Components/UI/AppDigitalizador`.
  - `npx vitest run src/modules/digitalizacion`.

## Open Questions

- Que APIs exactas expone DWT 19.3.2 para Deskew, Auto Crop y Auto Rotate en este runtime.
- Si la licencia actual habilita procesamiento avanzado de imagen.
- Si se debe incluir una libreria propia para procesamiento cuando Dynamsoft no cubra una capacidad.
- Que umbrales son aceptables para Auto Crop sin recortar contenido documental.
