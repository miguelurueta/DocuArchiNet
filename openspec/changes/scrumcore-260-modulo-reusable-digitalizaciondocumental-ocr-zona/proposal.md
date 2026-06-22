## Why

SCRUMCORE-260 solicita OCR por zona para extraer texto desde una region seleccionada dentro de una pagina digitalizada. La seleccion de area ya existe por SCRUMCORE-269 y debe reutilizarse.

El ticket tambien establece una restriccion explicita: no implementar OCR funcional hasta auditar licencia, disponibilidad de APIs, idiomas y rendimiento. Por eso este cambio formaliza auditoria y diseno tecnico.

## What Changes

- Se documenta el estado actual de Dynamsoft Web TWAIN 19.3.2 frente a OCR.
- Se confirma que el contrato local no expone API OCR tipada ni dependencias OCR.
- Se define una arquitectura futura basada en `DigitalizacionOcrClient`.
- Se establece que `OCR Zona` debe reutilizar `PageCropSelection` y el `pageId` activo.
- Se documentan flujo UX futuro, drawer de resultado, acciones y riesgos.
- No se agrega boton, drawer, dependencia OCR ni ejecucion OCR real en esta fase.

## Jira Details

> OCR POR ZONA (EXTRACCION DE TEXTO DESDE AREA SELECCIONADA)
> CONTEXTO
> El modulo de digitalizacion permitira seleccionar una region especifica dentro de una pagina mediante la funcionalidad de Seleccion de Area.
> Se requiere aprovechar dicha seleccion para ejecutar OCR unicamente sobre la region seleccionada.
> OBJETIVO
> Permitir que el usuario seleccione una zona especifica de una pagina y extraiga unicamente el texto contenido en dicha region.
> DEPENDENCIA
> Requiere SCRUMCORE-269 Seleccion de Area + Recorte Manual.
> La seleccion existente debe reutilizarse.
> NO crear un segundo mecanismo de seleccion.
> TOOLBAR
> Agregar futuramente OCR Zona con tooltip Extraer texto de la seleccion.
> RESULTADO
> Mostrar modal lateral o drawer con Texto extraido, Copiar, Insertar en metadato y Cerrar.
> NO IMPLEMENTAR AUN
> Antes de desarrollar: auditar licencia actual de Dynamsoft, disponibilidad OCR, APIs disponibles, idiomas soportados y rendimiento esperado.
> DOCUMENTACION
> Crear docs/Architecture/DigitalizacionDocumental/SCRUMCORE-277-ocr-zona.md.
> SOLO AUDITORIA Y DISENO TECNICO.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DIGITALIZACIONDOCUMENTAL, MODULOS, OCR, REUSABLE, ZONA

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-ocr-zona`: auditoria y diseno tecnico para OCR por zona.

### Modified Capabilities
- `modulo-reusable-digitalizaciondocumental-modernizacion-escaneo`: reutiliza la seleccion visual existente solo como dependencia documentada, sin cambio funcional.

## Impact

- Nuevo documento: `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-277-ocr-zona.md`.
- OpenSpec refinado para bloquear implementacion funcional hasta confirmar capacidades OCR.
- Sin impacto runtime en scanner, toolbar, preview, PaperStream, Dynamsoft ni generacion PDF.
