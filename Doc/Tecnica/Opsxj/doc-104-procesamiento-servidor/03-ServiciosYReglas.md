# PROCESAMIENTO-SERVIDOR — SERVICIOS Y REGLAS

- Ticket: DOC-104
- Cambio OpenSpec: doc-104-procesamiento-servidor
- Clasificacion: cross_cutting

## Servicios y reglas

## `WebService_itext.service_anotate_pdf_image`

1. Deserializa exactamente un `Class_itext_anotate`.
2. Normaliza números con cultura invariante y rechaza NaN/infinito.
3. Admite rotación 0/90/180/270, transparencia 0/1 y tolerancia 0..255.
4. Exige exactamente una fuente de imagen: archivo o base64.
5. Consulta el certificado antes de crear archivos o adquirir lock.
6. Convierte excepciones en mensajes públicos cerrados.
7. Si la auditoría falla después del commit, conserva éxito documental con advertencia.

## `Class_ItexShare.add_anotate_image_pdf`

- Base64: máximo 10 MiB decodificados.
- Bitmap: máximo 4096 x 4096.
- Lock: archivo lateral abierto con `FileShare.None` por documento.
- Imágenes: workspace `Temp_Image/upload_file/<usuario>/<GUID>/`.
- PDF staged/backup: junto al original para evitar cruce de volumen.
- Página: `1..PdfReader.NumberOfPages` antes de crear stamper.
- Staged: longitud positiva, lectura válida, igual número de páginas y página objetivo existente.
- Commit: `File.Replace`; no existe fallback `File.Copy(..., True)`.
- Recuperación: si el resultado reemplazado no valida, se restaura backup.
- Clasificación: `EstrategiaCargaPdf.Seleccionar` solo después del commit.

## `Class_transparent.Transparent`

Convierte a `Format32bppArgb`, recorre el buffer respetando stride y cambia únicamente el alfa a cero cuando cada diferencia RGB es estrictamente menor que la tolerancia. Los demás canales y píxeles se preservan.
