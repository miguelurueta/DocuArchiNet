# SCRUMCORE-264 - Capture Management

## Objetivo

Agregar operaciones avanzadas de captura al digitalizador documental para construir o corregir documentos sin reiniciar todo el flujo:

- Nuevo
- Reemplazar
- Insertar antes
- Insertar despues
- Agregar

## Modelo

La operacion se modela como `CaptureOperation` en el contrato del scanner:

- `NEW`: la captura actual reemplaza el documento en construccion.
- `REPLACE`: las paginas recien capturadas sustituyen la pagina activa.
- `INSERT_BEFORE`: las paginas recien capturadas se ubican antes de la pagina activa.
- `INSERT_AFTER`: las paginas recien capturadas se ubican despues de la pagina activa.
- `APPEND`: las paginas recien capturadas se agregan al final.

El contrato vive en `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts` y viaja dentro de `ScanOptions.captureOperation`.

## Arquitectura

`DigitalizacionDocumentalWorkspace` conserva una unica funcion de adquisicion que arma las opciones de scanner actuales: dispositivo, color, duplex, ADF, resolucion y procesamiento automatico. Los botones de toolbar solo agregan la intencion de captura.

`DynamsoftTwainClient.scan()` mantiene la adquisicion nativa de Dynamsoft sin cambiar el flujo del driver. Despues de adquirir, identifica las paginas nuevas comparando el lote previo con el buffer resultante y resuelve el orden visual/PDF segun `CaptureOperation`.

`generatePdf()` ya genera el PDF usando los indices de `this.pages`, por lo que el orden resuelto se respeta sin mover fisicamente imagenes en el buffer de Dynamsoft.

## UX

Las acciones de captura aparecen en la toolbar principal:

1. Escanear / Nuevo documento
2. Reemplazar
3. Insertar
4. Agregar

`Reemplazar` e `Insertar` requieren pagina activa. `Agregar` esta disponible cuando hay scanner seleccionado.

## Boton Inteligente Escanear / Nuevo Documento

SCRUMCORE-265 unifica `Escanear` y `Nuevo` en un unico boton contextual para reducir ruido visual en la toolbar.

- Con documento vacio (`pages.length === 0`), el boton muestra `Escanear`, usa tooltip `Iniciar captura documental` e inicia la captura sin confirmacion.
- Con documento en construccion (`pages.length > 0`), el mismo boton cambia a `Nuevo documento`, usa tooltip `Descartar documento actual e iniciar uno nuevo` y reutiliza la confirmacion existente de la operacion `NEW`.
- Si el usuario cancela la confirmacion, se conserva el estado actual: paginas, miniaturas, seleccion, preview, PDF generado y navegacion.
- Si el usuario continua, se limpia el documento actual, se resetean seleccion/crop/navegacion/PDF temporal y se inicia una captura `NEW`.

## Compatibilidad

Las operaciones mantienen compatibilidad con:

- organizador de paginas
- drag and drop
- seleccion multiple
- crop manual
- rotacion
- duplicacion
- navegacion flotante
- zoom y ajustes de preview
- pantalla completa

## Riesgos

La eliminacion de paginas en blanco se ejecuta dentro del flujo de scanner existente. Cuando se usa junto con operaciones sobre documentos ya capturados, debe verificarse con lotes reales de scanner para confirmar que no descarte paginas existentes por falsos positivos.

## Validacion

Cobertura agregada:

- `DynamsoftTwainClient.test.ts`: append, replace, insert before/after y orden de indices PDF.
- `DigitalizacionDocumentalModal.test.tsx`: exposicion de toolbar y envio de `captureOperation`.
