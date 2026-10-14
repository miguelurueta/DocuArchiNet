# PROCESAMIENTO-SERVIDOR

- Ticket: DOC-104
- Cambio OpenSpec: doc-104-procesamiento-servidor
- Clasificacion: cross_cutting

## Objetivo

El guardado del grafo deja de reutilizar temporales por usuario y deja de copiar directamente sobre el PDF original. Cada operación usa GUID, lock por documento, imagen preparada propia, PDF staged adyacente, backup y validación anterior/posterior al reemplazo.

La transparencia se procesa con `Bitmap.LockBits`; se eliminó la falsa asincronía y no se usa `GetPixel/SetPixel`. El resultado visual legacy mantiene tamaño efectivo 90 x 90. Cambiar ese tamaño queda fuera de DOC-104.

## Flujo

```text
Cliente DOC-103
  -> WebMethod valida payload
  -> valida certificado antes de escribir
  -> resuelve PDF y adquiere lock por documento
  -> crea workspace GUID
  -> carga y valida imagen
  -> aplica transparencia opcional con LockBits
  -> PdfStamper escribe staged.pdf
  -> reabre y valida staged.pdf
  -> File.Replace(staged, original, backup)
  -> reabre original; restaura backup si falla
  -> clasifica RANGE/STREAM sobre el archivo persistido
  -> responde URL vigente con cache-buster
  -> PDF.js refresca getPage(i) en modo STREAM
  -> libera recursos y limpia artefactos propios
```

## Alcance y compatibilidad

- Se conservan endpoint ASMX, arreglo `Class_itext_anotate` y colección `service_itex`.
- No cambia almacenamiento, esquema, iTextSharp ni autorización de rutas.
- No se introduce linealización; perder `/Linearized` selecciona `stream`.
- Reversa de despliegue: restaurar el commit. No existe migración de datos.
- Reversa de operación: `File.Replace` conserva backup hasta validar el original reemplazado.
