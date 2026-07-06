# SCRUMCORE-287 - Flujo API Y Estados

## Resumen

Este documento separa el flujo en tres capas:

1. configuracion upload;
2. seleccion y validacion en UI;
3. transferencia y almacenamiento documental.

La regla principal es fail-closed: si la configuracion no se puede resolver desde backend, el usuario no debe seleccionar archivos.

## Flujo API

### 1. Resolver Configuracion De Upload

Request:

```http
GET /api/gestor-documental/configuracion-upload?nameProceso=CORRESPO
Authorization: Bearer <token>
```

Response esperada:

```json
{
  "success": true,
  "message": "YES",
  "data": [
    {
      "IdConfigUploadGestion": 3,
      "ExtensionUpload": ".PDF,.DOC,.DOCX,.ZIP,.XLS,.XLSX",
      "LengUpload": 600000000,
      "NameProceso": "CORRESPO",
      "EstadoProceso": 1
    }
  ],
  "meta": null,
  "errors": []
}
```

Normalizacion:

```txt
ExtensionUpload -> allowedExtensions -> accept
LengUpload -> maxSizeBytes
```

### 2. Resolver Tipologias Workflow

Request:

```http
GET /api/gestor-documental/tipologias-documentales?Contexto=WORKFLOW&IdTareaWf={idTareaWf}&IdRutaWf={idRutaWf}
Authorization: Bearer <token>
```

Uso:

- poblar dropdown por archivo;
- permitir metadata independiente por adjunto;
- bloquear/validar antes de almacenar si falta tipologia obligatoria.

### 3. Inicializar Upload Temporal

Request:

```http
POST /api/gestor-documental/almacenamiento/upload-temporal/init
Authorization: Bearer <token>
Content-Type: application/json
```

Payload conceptual:

```json
{
  "NombreOriginal": "archivo.pdf",
  "TamanoBytes": 869185622,
  "Extension": ".pdf",
  "HashSha256Esperado": null,
  "NumeroChunks": 208
}
```

Response esperada:

```json
{
  "success": true,
  "data": {
    "RutaTemporalId": "usr_136_x",
    "ArchivoTemporalId": "af_x.pdf",
    "ChunkSizeBytes": 10485760,
    "Estado": "IN_PROGRESS"
  }
}
```

### 4. Subir Chunks

Request:

```http
PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
Authorization: Bearer <token>
Content-Type: application/octet-stream
X-Total-Chunks: {totalChunks}
```

Reglas frontend:

- usar `ChunkSizeBytes` sugerido por backend;
- aplicar `storageOptions.maxChunkSizeBytes` si el flujo lo define;
- para Gestion Respuesta se usa tope tecnico de 4 MB;
- reintentar solo errores transitorios sin response (`ERR_NETWORK`, timeout);
- no reintentar aborts ni errores funcionales backend.

### 5. Validar Estado Temporal

Request:

```http
GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
Authorization: Bearer <token>
```

Uso:

- confirmar bytes recibidos;
- detectar chunks pendientes;
- evitar `complete` cuando el temporal aun no esta listo.

### 6. Completar Temporal

Request:

```http
POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
Authorization: Bearer <token>
```

### 7. Registrar Documento

Request:

```http
POST /api/gestor-documental/almacenamiento
Authorization: Bearer <token>
Content-Type: application/json
```

Payload conceptual para Gestion Respuesta:

```json
{
  "NombreGabinete": "CORRESPO",
  "RutaTemporalId": "usr_136_x",
  "NombreDocumento": "Anexo workflow respuesta 2500466700035",
  "RequestId": "documental-x",
  "Documentos": [
    {
      "IdDocumento": "wf-anexo-documental-x",
      "ArchivoTemporalId": "af_x.pdf",
      "NombreOriginal": "archivo.pdf",
      "Extension": ".pdf"
    }
  ],
  "Inventario": {
    "Radicado": "2500466700035",
    "IdUsuarioGestion": 136,
    "IdEmpresa": 2,
    "FechaElaboracion": "2026-07-06"
  },
  "Trd": {
    "IdTipoDocumento": 72,
    "NombreTipoDocumento": "Anexos Oficios"
  },
  "Workflow": {
    "IdTareaWorkflow": 933,
    "IdRutaWorkflow": 9
  },
  "AnexoRespuesta": {
    "IdRespuestaRadicado": 640,
    "NombreArchivo": "archivo.pdf",
    "TipoAdjunto": "respuesta",
    "Observacion": "Anexo cargado desde workflow"
  }
}
```

Response esperada:

```json
{
  "success": true,
  "data": {
    "Documento": {
      "IdAlmacen": 9963,
      "IdRegistroProduccionDocumental": 22222,
      "NombreArchivoFinal": "DIG00009963.pdf"
    },
    "AnexoRespuesta": {
      "Created": true,
      "IdRespuestaRadicado": 640
    },
    "Workflow": {
      "LogInserted": true
    }
  }
}
```

## Estados De Archivo

| Estado | Significado | Acciones |
| --- | --- | --- |
| `ready` | Archivo listo para validar/guardar. | Guardar uno, Guardar todo, eliminar. |
| `warning` | Archivo usable con advertencia. | Guardar uno, Guardar todo, eliminar. |
| `error` | Archivo con error local o funcional. | Corregir metadata, eliminar. |
| `uploading` | Subiendo chunks. | Cancelar archivo. |
| `completing` | Cerrando temporal. | Cancelar si el request sigue activo. |
| `storing` | Registrando documento final. | Cancelar si el request sigue activo. |
| `done` | Almacenado. | Se remueve de cola. |
| `cancelled` | Cancelado por usuario o abortado. | Reintentar con Guardar todo o eliminar. |
| `removed` | Marcado para salida visual. | Sin acciones. |

## Estados Globales De Lote

| Campo | Uso |
| --- | --- |
| `stored` | Cantidad de archivos confirmados como almacenados. |
| `failed` | Cantidad de archivos con error funcional o validacion. |
| `skipped` | Cantidad de archivos no procesados por cancelacion global o prevalidacion. |
| `cancelled` | Cantidad de aborts de usuario. |
| `remainingFiles` | Cantidad real de archivos que siguen en cola despues de remover almacenados. |

## Politica De Cierre

El modal de Gestion Respuesta cierra solo si:

```txt
stored > 0
failed == 0
skipped == 0
cancelled == 0
remainingFiles == 0
```

Si `remainingFiles > 0`, el modal permanece abierto aunque haya almacenado uno o varios archivos.

## Politica De Cancelacion

### Cancelacion Unitaria

- Aborta solo el archivo activo.
- No afecta otros archivos.
- Deja el archivo en `cancelled`.
- El archivo queda reintentable.

### Cancelacion Global

- Aborta todos los controllers activos.
- Detiene el loop del lote inline.
- Conserva archivos no iniciados en cola.
- No elimina documentos ya almacenados.
- Mantiene abierto el modal si queda algo pendiente.

## Politica De Retry De Chunks

Se reintenta cuando:

- no existe `response` HTTP;
- `code` es `ERR_NETWORK`, `ECONNABORTED` o `ETIMEDOUT`;
- el mensaje contiene `Network Error` o `timeout`;
- el `AbortSignal` no esta abortado.

No se reintenta cuando:

- el usuario cancela;
- existe `response.data`;
- backend responde validacion o negocio;
- el error es `AbortError` o `ERR_CANCELED`.

## Riesgos Y Observaciones

- Si backend devuelve configuracion con `ExtensionUpload=null` o `LengUpload=null`, el frontend bloquea seleccion.
- Si IIS/proxy corta PUTs grandes, el retry de chunks reduce fallos transitorios, pero no reemplaza ajustes backend de timeout o request body.
- Si backend no refleja de inmediato en el arbol, el frontend solo puede refrescar; la consistencia final depende de la API de consulta.
- Si el token expira durante una subida grande, el upload puede abortarse y debe reintentarse despues de renovar sesion.
