<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->

## Context

`WebService_itext.service_anotate_pdf_image(...)` deserializa un arreglo `Class_itext_anotate`, consulta el certificado y delega la mutación en `Class_ItexShare.add_anotate_image_pdf(...)`. La implementación actual usa nombres temporales derivados solo del usuario (`usuario.png`, `usuario_1.png`, `_<documento>.pdf` y `usuario.pdf`), sobrescribe el original con `File.Copy(..., True)` y genera un PDF adicional de una página mediante `Create_file_pagina(...)`.

`Class_transparent.TransparentAsync(...)` es síncrono. Su recorrido descendente no declara `Step -1`, por lo que la tolerancia no se aplica como expresa el código; corregirlo con `GetPixel/SetPixel` agravaría el costo. En el estampado, `ScaleAbsolute(anotate_width, anotate_heigth)` es reemplazado inmediatamente por `ScaleAbsolute(90, 90)`, de modo que 90 x 90 es el resultado visual legacy que esta optimización debe preservar.

DOC-103 ya garantiza un POST único, espera la respuesta ASMX y refresca antes de retirar el overlay. Su `_updatePage(i, document)` consume `stream`, pero abre siempre `getPage(1)` porque el servidor devuelve un PDF temporal de una página. DOC-102 clasifica el PDF real con `EstrategiaCargaPdf.Seleccionar(...)`: solo un archivo de al menos 1 MiB con `/Linearized` en los primeros 4 KiB usa `range`; cualquier otro resultado usa `stream`.

## Goals / Non-Goals

**Goals**

- Aislar cada operación y evitar contaminación entre usuarios, documentos o solicitudes concurrentes.
- Preservar certificado, página, coordenadas y resultado visual 90 x 90 mientras se vuelve determinista la liberación de recursos.
- Construir y validar un PDF completo antes de reemplazar el original con backup recuperable.
- Eliminar la extracción adicional de una página y refrescar la página afectada desde el documento vigente.
- Clasificar la estructura persistida y sanear errores y auditoría.

**Non-Goals**

- Cambiar iTextSharp, almacenamiento, esquema, colas, DTO público de entrada o firma del WebMethod.
- Agregar Ghostscript, linealización productiva o exigir que iTextSharp conserve `/Linearized`.
- Corregir en esta Jira el tamaño histórico 90 x 90 o rediseñar todas las transformaciones geométricas.
- Reemplazar la autorización legacy de rutas o migrar otros consumidores de `rut_image`.

## Decisions

### D-01 — Frontera ASMX y barrera de certificado

Se conservarán `service_anotate_pdf_image(ByVal anotate As Object) As IEnumerable(Of service_itex)` y `Class_itext_anotate`. El WebMethod exigirá exactamente un elemento, validará presencia y tipos antes de delegar y mantendrá `Solicita_registro_certificado_archivo_imagen_gabinete(...)` antes de crear directorios, decodificar base64 o abrir el PDF. Todo rechazo usará mensajes públicos cerrados, nunca `ex.Message`. <!-- D-01; RQ-01 -->

### D-02 — Validación acotada antes de iText

Página, canvas, coordenadas, tamaño, escala y tolerancia se convertirán explícitamente con cultura invariante y deberán ser finitos y positivos donde corresponda. La rotación admitirá solo 0, 90, 180 o 270; transparencia solo 0 o 1; tolerancia, 0 a 255. El base64 decodificado se limitará a 10 MiB y la imagen a 4096 x 4096. El límite superior de página se verificará inmediatamente después de abrir `PdfReader` y antes de crear `PdfStamper`. <!-- D-02; RQ-02 -->

### D-03 — Exclusión por documento y temporales por operación

Cada invocación generará `operationId = Guid.NewGuid().ToString("N")`. Un archivo lateral adyacente al PDF, abierto con `FileShare.None`, dará exclusión entre procesos que comparten almacenamiento: operaciones sobre el mismo documento fallarán de forma controlada si ya está en actualización; documentos distintos seguirán en paralelo. Imágenes intermedias vivirán bajo `Temp_Image/upload_file/<usuario>/<operationId>/`; staged y backup vivirán junto al original para que el reemplazo no cruce volúmenes. Solo se eliminarán rutas propiedad de esa operación. <!-- D-03; RQ-03 -->

### D-04 — Transparencia síncrona mediante LockBits

`TransparentAsync` dejará de existir en el camino productivo. `Class_transparent.Transparent(...)` recorrerá un `Bitmap` en formato compatible mediante `LockBits`, respetará stride y canal alfa, y pondrá alfa cero únicamente cuando las tres diferencias RGB sean estrictamente menores que la tolerancia. Los píxeles no coincidentes conservarán RGB y alfa. No se utilizará `GetPixel/SetPixel`. <!-- D-04; RQ-04 -->

### D-05 — Estampado compatible y recursos deterministas

`add_anotate_image_pdf(...)` conservará el cálculo legacy de página rotada y el tamaño efectivo 90 x 90; no reinterpretará en esta Jira el ancho/alto recibido. `Bitmap`, `PdfReader`, `PdfStamper`, `FileStream` e imágenes administradas se cerrarán mediante `Using` o `Finally` en éxito, rechazo y excepción. La página deberá estar dentro de `1..NumberOfPages` antes de solicitar `GetOverContent`. <!-- D-05; RQ-05 -->

### D-06 — Staging, validación y reemplazo recuperable

iTextSharp escribirá solo `.<nombre>.<operationId>.staged.pdf`. Cerrado el stamper, el staged se reabrirá para comprobar longitud positiva, lectura válida, igual número de páginas y existencia de la página objetivo. Después se ejecutará `File.Replace(staged, original, backup)` en el mismo directorio. El original se reabrirá tras el reemplazo; si esa validación falla se restaurará el backup. Si `File.Replace` no está soportado, la operación fallará cerrada y no caerá en una copia destructiva. <!-- D-06; RQ-06 -->

### D-07 — Refresco del documento vigente sin PDF de una página

`Create_file_pagina(...)` saldrá del flujo de guardado. El servidor devolverá en `url_salida` la URL de lectura legacy recibida con un parámetro opaco de invalidación de caché, sin exponer ruta física. `_updatePage(i, document)` mantendrá `_createLoadingSource(document, "stream")` pero solicitará `getPage(i)`. Así se conserva el contrato `pageUrl` de DOC-103, se evita el temporal público `{usuario}.pdf` y solo se renderiza la página afectada. <!-- D-07; RQ-07 -->

### D-08 — Clasificación posterior, desacoplada de la mutación

Solo después de confirmar el reemplazo se ejecutará `EstrategiaCargaPdf.Seleccionar(original)`. La medición registrará `range` o `stream` sin ruta ni contenido. La respuesta de refresco seguirá forzando `stream`; la clasificación documenta la siguiente apertura y no convierte la mutación en un mecanismo de transporte. No se ejecutará linealización externa. <!-- D-08; RQ-08 -->

### D-09 — Errores y auditoría saneados

Se eliminarán de `datos_add_grafo` `anotate_file_src`, `pdf_file_src`, base64 y excepciones. La auditoría conservará identificadores funcionales ya autorizados, página, tipo de operación, resultado y modo de carga. Un fallo anterior al reemplazo devuelve rechazo; un fallo de auditoría posterior conserva `error_sistema = "YES"` y `error_log <> "YES"`, que DOC-103 interpreta como `committed_with_warning` sin reintento. <!-- D-09; RQ-09 -->

### D-10 — Caracterización y validación como unidad de entrega

Antes de refactorizar se capturarán huellas de fixtures descartables para página normal/rotada, imagen de archivo/base64, transparencia, límites, firma y `/Linearized`. Las pruebas cubrirán fallos por etapa, concurrencia sobre igual/distinto documento, recursos liberados, equivalencia visual y tiempo por fase. La E2E real reutilizará `tools/e2e`, requerirá las autorizaciones del runbook y usará controles exclusivamente `SELECT`; código, pruebas, documentación y evidencia pertenecen al mismo cambio. <!-- D-10; RQ-10 -->

## Processing Flow

1. El cliente DOC-103 envía un único POST y conserva el overlay.
2. El WebMethod valida cardinalidad/tipos y consulta el certificado sin escribir.
3. `Class_ItexShare` resuelve la ruta existente y adquiere el lock por documento.
4. La operación crea exclusivamente sus rutas con GUID y carga archivo o base64.
5. `Class_transparent` transforma la imagen con `LockBits` si aplica.
6. `PdfReader` valida página; `PdfStamper` escribe el staged con el grafo 90 x 90.
7. Todos los handles se cierran; el staged se reabre y valida.
8. `File.Replace` conserva backup, reemplaza y permite restauración comprobada.
9. El PDF persistido se clasifica `range|stream`; la auditoría registra solo campos permitidos.
10. La respuesta entrega la URL vigente con cache-buster; `_updatePage` abre `getPage(i)` en modo stream.
11. `Finally` elimina directorio de operación, staged/backup residuales y libera el lock.

## Risks / Trade-offs

- El lock lateral coordina procesos que comparten el mismo sistema de archivos, pero no un almacenamiento replicado sin locks compartidos; ante conflicto se rechaza en vez de arriesgar pérdida de actualización.
- `File.Replace` puede no estar disponible en algún proveedor de archivos. Fallar sin tocar el original prioriza integridad sobre disponibilidad.
- Mantener 90 x 90 preserva compatibilidad pero deja sin efecto visual el ancho/alto enviado; corregirlo requiere caracterización funcional separada.
- El refresco del PDF completo en modo stream puede transferir más bytes que el PDF de una página, pero elimina una segunda reescritura iText y un temporal compartido. La E2E medirá este intercambio.
- Aplicar correctamente la tolerancia puede cambiar píxeles que antes no cambiaban por el defecto del bucle. La comparación de fixtures deberá demostrar el resultado aprobado antes de desplegar.
- El ambiente de cierre no tiene un PDF firmado descartable. El responsable aceptó el riesgo residual de no ejecutar ese único escenario real; se conserva la barrera previa en código, la prueba de orden contractual y un runner `signed` estricto para validación futura. No se registra el escenario como ejecutado.

## Migration Plan

1. Crear caracterización y pruebas de política que fallen con las colisiones y contratos actuales.
2. Incorporar validación, lock, workspace y transparencia sin conectar aún el reemplazo.
3. Implementar staging/validación/reemplazo y ejecutar fallos controlados sobre copias descartables.
4. Cambiar el refresco coordinadamente en backend y `_updatePage`, manteniendo el contrato ASMX de colección.
5. Ejecutar pruebas focales, MSBuild, regresión DOC-102/DOC-103 y E2E autorizada.
6. Rollback: restaurar código anterior desde Git; no se requiere migración de datos. Cualquier backup de una operación fallida se restaura antes de responder y se documenta sin revelar su ruta.
