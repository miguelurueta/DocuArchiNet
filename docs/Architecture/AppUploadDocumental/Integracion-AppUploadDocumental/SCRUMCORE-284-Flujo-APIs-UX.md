# SCRUMCORE-284 - Flujo, APIs y UX operativa

## Flujo funcional desde usuario

1. Usuario abre Gestion Respuesta.
2. En tab Gestion hace click en `Adjuntar documentos`.
3. Se abre `GestionRespuestaUploadDocumentalModal`.
4. Se renderiza `GestionRespuestaUploadDocumental`.
5. Se resuelve contexto:
   - gabinete;
   - radicado;
   - respuesta de radicado;
   - tarea workflow;
   - ruta workflow;
   - usuario gestion;
   - empresa;
   - fecha elaboracion.
6. `AppUploadDocumental` carga configuracion y tipologias.
7. Usuario agrega archivos.
8. Cada archivo queda en cola con metadata independiente.
9. Usuario selecciona tipologia por archivo o deja el campo vacio si se esta validando backend.
10. Usuario ejecuta `Guardar` o `Guardar todo`.
11. Por cada archivo se ejecuta StorageEngineV2.
12. Si backend confirma `AnexoRespuesta.Created`, se refresca Workbench y se cierra modal.
13. El documento aparece en el listado del Workbench si backend lo retorna en la recarga.
14. El usuario puede abrirlo en el visor oficial del tab Documentos.

## Flujo tecnico por archivo

```txt
File seleccionado
-> init upload temporal
-> subir chunks raw bytes
-> consultar status
-> complete upload temporal
-> POST almacenamiento final
-> normalizar respuesta
-> validar AnexoRespuesta.Created
-> refreshDocumentos
-> remount AppTreeTable
-> close modal
```

## APIs consumidas

### Tipologias workflow

```http
GET /api/gestor-documental/tipologias-documentales?Contexto=WORKFLOW&IdTareaWf={idTareaWf}&IdRutaWf={idRutaWf}
```

Contrato esperado:

```ts
{
  success: boolean;
  message: string;
  data: Array<{
    Id: number;
    Descripcion: string;
  }>;
  meta?: {
    Status?: string;
    RequestId?: string;
    Total?: number;
  };
  errors?: unknown[];
}
```

### Upload temporal init

```http
POST /api/gestor-documental/almacenamiento/upload-temporal/init
Content-Type: application/json
```

Payload:

```json
{
  "NombreOriginal": "archivo.pdf",
  "TamanoBytes": 48216,
  "Extension": ".pdf",
  "HashSha256Esperado": null,
  "NumeroChunks": 1
}
```

### Chunk upload

```http
PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
Content-Type: application/octet-stream
X-Total-Chunks: {totalChunks}
```

Reglas:

- `chunkIndex` inicia en cero.
- Se envia solo el `Blob` del chunk.
- No se usa `FormData`.
- El ultimo chunk puede ser menor.

### Status temporal

```http
GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
```

El frontend valida que no existan chunks pendientes antes de `complete` cuando la opcion esta habilitada.

### Complete temporal

```http
POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
```

### Almacenamiento final

```http
POST /api/gestor-documental/almacenamiento
Content-Type: application/json
```

Payload clave:

```json
{
  "NombreGabinete": "CORRESPO",
  "RutaTemporalId": "usr_...",
  "NombreDocumento": "Anexo workflow respuesta 2500466700035",
  "RequestId": "documental-...",
  "Documentos": [
    {
      "IdDocumento": "wf-anexo-documental-...",
      "ArchivoTemporalId": "af_....pdf",
      "NombreOriginal": "DIG00008872.PDF",
      "Extension": ".pdf"
    }
  ],
  "Inventario": {
    "IdUsuarioGestion": 136,
    "IdEmpresa": 2,
    "Radicado": "2500466700035",
    "FechaElaboracion": "2026-07-02"
  },
  "Trd": {
    "IdTipoDocumento": 72,
    "NombreTipoDocumento": "Anexos Oficios"
  },
  "Workflow": {
    "IdTareaWorkflow": 933,
    "IdRutaWorkflow": 9
  },
  "CabinetIndexSeed": {
    "SourceModule": "RADICACION",
    "ProviderKey": "RADICACION",
    "Version": "1.0.0",
    "Payload": {
      "ModoResolucion": "RespuestaRadicado",
      "ProveedorExterno": null,
      "RadicadoExterno": null,
      "MatriculaSII": null
    }
  },
  "AnexoRespuesta": {
    "IdRespuestaRadicado": 640,
    "NombreArchivo": "DIG00008872.PDF",
    "TipoAdjunto": "respuesta",
    "Observacion": "Anexo cargado desde workflow"
  }
}
```

Respuesta exitosa observada:

```ts
{
  Documento: {
    IdAlmacen: 9930,
    IdRegistroProduccionDocumental: 22189,
    NombreArchivoFinal: "DIG00009930.pdf"
  },
  AnexoRespuesta: {
    Created: true,
    IdAlmacen: 9930,
    IdAnexoRespuesta: 122,
    IdRespuestaRadicado: 640,
    NombreArchivo: "DIG00008872.PDF",
    NombreGabinete: "CORRESPO"
  },
  Indice: {
    ProviderKey: "RADICACION",
    Resolved: true,
    SourceTrace: "radicacion_legacy_index"
  },
  Workflow: {
    IdRutaWorkflow: 9,
    IdTareaWorkflow: 933,
    LogInserted: true
  }
}
```

### Acciones Workbench

```http
POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action
```

Request de eliminacion observado:

```ts
{
  TableId: "InboxListaDocumentosRadicado",
  ViewMode: "flatDocuments",
  ActionId: "eliminar_item",
  RowId: "doc-9931",
  ParentRowId: null,
  NodeType: "documento",
  Payload: {
    DocumentId: 9931,
    NombreGabinete: "CORRESPO"
  }
}
```

Respuesta observada:

```ts
{
  success: true,
  message: "OK",
  data: {
    AffectedRowId: "doc-9931",
    Operation: "deleted",
    RequiresReloadNode: true,
    Row: null,
    DocumentResolveRequest: null
  }
}
```

## Comportamiento UX por accion

### Guardar individual

- No abre `AppProgressBatch`.
- Muestra progreso en la fila.
- Si almacena correctamente, dispara `onStored`.
- Si `AnexoRespuesta.Created=true`, se refresca Workbench y se cierra modal.

### Guardar todo

- Abre `AppProgressBatch`.
- Procesa archivos secuencialmente.
- Cada archivo usa el mismo flujo de storage.
- Si el backend rechaza por tipologia/TRD, se marca error visual sin mensaje duplicado de tipologia.
- Si hay exito, se refresca y cierra modal por confirmacion backend.

### Sin tipologia

Decision temporal actual:

- El frontend no bloquea.
- El mapper envia `trd: null`.
- El backend responde la regla real.
- Se loguea request/response en consola para diagnostico.
- UI no muestra mensaje tecnico de tipologia si se detecta que corresponde a TRD/tipologia.

Backend observado:

```txt
Cabinet index seed is invalid: StorageTrd requerido
```

### Archivo pesado

Estado actual:

- Hay limite frontend local de 25 MB.
- El cliente tecnico soporta chunks.
- Para permitir archivos pesados de forma enterprise, debe consumirse `configuracion-upload` y mapear `LengUpload`.

## Diagnostico temporal retirado

Durante la validacion local se usaron trazas de consola para confirmar el flujo completo. No quedan activas en runtime al cierre del ticket.

### Upload documental exitoso

```txt
[GestionRespuestaUploadDocumental] stored result
[GestionRespuestaUploadDocumental] refreshDocumentos triggered
```

### Missing typology / TRD

```txt
[almacenamientoDocumentalUpload] almacenarDocumento missing typology request
[almacenamientoDocumentalUpload] almacenarDocumento missing typology response
[almacenamientoDocumentalUpload] almacenarDocumento backend validation
```

### Workbench action

```txt
[DocumentosWorkbench] action triggered
[useGestionRespuestaDocumentosTable] performAction input
[useGestionRespuestaDocumentosTable] action request
[useGestionRespuestaDocumentosTable] performAction response
```

La evidencia funcional queda en esta documentacion. El codigo productivo conserva solo el manejo de errores normalizado.

## Decisiones de diseno

- No insertar localmente filas nuevas en `AppTreeTable`; el backend es fuente de verdad.
- Cerrar modal solo cuando backend confirma `AnexoRespuesta.Created=true`.
- Remontar `AppTreeTable` por `key` para reflejar cambios reales.
- Mantener `AppUploadBatchView` sin conocimiento de dominio documental.
- Mantener `GestionRespuestaUploadDocumental` como adapter del modulo.
- No bloquear tipologia en frontend durante diagnostico backend.
- Suprimir mensaje tecnico visible de tipologia/TRD mientras se valida contrato.
