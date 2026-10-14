# DOC-102 — implementación del streamer HTTP Range

DOC-102 cambia la apertura enriquecida del visor PDF de una descarga completa en memoria a lectura directa mediante URL autorizada. El navegador no recibe la ruta física; recibe `doc_ref`, una referencia protegida, temporal y ligada a la sesión. El transporte se selecciona sin cambiar almacenamiento: Range para PDF grandes linearizados y streaming secuencial para los demás.

## Qué mejora

- El visor deja de precargar explícitamente el archivo y solicita rangos; algunos PDF no optimizados pueden requerir bytes distribuidos por todo el documento para resolver la primera página.
- PDF.js puede pedir segmentos adicionales según scroll, zoom, rotación o navegación.
- La memoria del servidor permanece acotada a un buffer de 64 KiB por copia, no al tamaño del PDF.
- Revalidación privada permite reutilizar contenido sin exponerlo como caché pública.
- Los PDF no linearizados evitan la cascada de rangos que terminó descargando el archivo completo: reciben un único `200` transmitido por bloques.

Esto no significa que cada scroll siempre produzca una descarga: PDF.js reutiliza bytes ya disponibles y solicita rangos adicionales cuando su parser los requiere. Tampoco se descarga proactivamente todo el documento desde `script.js`; `disableAutoFetch` impide la precarga automática. Sin embargo, la estructura interna de un PDF puede obligar al parser a consultar todo el archivo antes del primer canvas. La E2E del documento de 34 páginas observó este último caso y, correctamente, dejó la compuerta en `FAILED`.

El guardado y pegado de grafo permanece separado del transporte: continúa usando `urimage_format` y el web service iText existente. Después de guardar, solo la página afectada se vuelve a leer mediante streaming completo compatible; no se reinicia el visor ni se cambian coordenadas, payload o permisos.

## Componentes

- `DocumentoStreamingReferencia`: emisión y resolución fail-closed de `doc_ref`.
- `RangoHttpDocumento`: parser de un único intervalo.
- `CondicionesHttpDocumento`: ETag, fechas y precondiciones.
- `EscritorStreamingDocumento`: copia exacta con buffer fijo.
- `Handler_image_wf`: orquestación HTTP y adaptador legacy.
- `script.js`: URL directa para `pdfjsLib.getDocument`.

## Seguridad y rollback

Amenazas cubiertas: manipulación/reutilización del token, fuga de ruta, traversal, rangos malformados, caché compartida, mensajes internos y bloqueo exclusivo del archivo. El camino autorizado usa sesión de solo lectura y no toca persistencia.

Rollback: revertir conjuntamente emisores, `script.js`, handler, componentes `Infrastructure`, registro del proyecto y pruebas. No se requiere rollback de base de datos. No debe revertirse solo el handler mientras los emisores sigan produciendo `doc_ref`, porque rompería la apertura enriquecida.

## Validación

Las pruebas focales, la política integral del visor y MSBuild pasan. La E2E adaptativa terminó `PASSED`: eligió `stream`, realizó un único `200` sin rangos por iteración y redujo la mediana de primer canvas de 1.123 ms a 624 ms para el PDF aprobado. Scroll y zoom continuaron operativos, con una tarea larga agregada. Consultar `Doc/Tecnica/Opsxj/doc-102-streamer-http-rangos/05-PruebasEvidencia.md`.

## Prueba de concepto de linearización

Sin modificar almacenamiento ni documentos del sistema, `tools/e2e/scripts/run-doc102-linearization-poc.cjs` construye en temporal un PDF sintético de 168 páginas desde el fixture público `help/help_searh_public_document.pdf`, genera una variante Fast Web View mediante Ghostscript y sirve ambas variantes por HTTP Range al PDF.js incluido en el repositorio. La prueba usa un perfil local controlado de aproximadamente 26,21 Mbps y elimina los derivados en `finally`.

La corrida reproducible confirmó igual cantidad de páginas y la misma huella visual de la primera página. La cobertura única necesaria hasta el primer canvas bajó de 305.840 bytes (28 %) a 105.180 bytes (9,12 %), una reducción del 65,6 %. En este fixture pequeño no mejoraron los bytes físicos enviados ni el tiempo: hubo solapamiento entre el `200` inicial y tres rangos, y el tiempo pasó de 488 a 536 ms. El resultado demuestra que Fast Web View habilita acceso parcial, no que garantice por sí solo menor latencia o tráfico.

Ghostscript no se incorpora al runtime ni al despliegue. Una decisión productiva posterior debe validar licenciamiento/redistribución, PDF cifrado, firmas digitales, formularios, anotaciones, metadatos, equivalencia visual completa y si una reescritura posterior con iText conserva o destruye la linearización.
