# SCRUMCORE-272 - Guia de uso upload-storage-client

Fecha: 2026-06-26

## Proposito

Esta guia explica como consumir el cliente tecnico de almacenamiento documental implementado en:

```txt
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
```

El cliente no es un componente React. Es un servicio reusable para `AppUploadDocumental`, flujos futuros de carga documental, procesos batch y wrappers especializados.

## Import principal

```ts
import {
  uploadAndStoreOneDocument,
  type UploadStorageProgress,
} from "@/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service";
```

Si el proyecto no tiene alias `@`, usar ruta relativa desde el consumidor.

## Uso recomendado para UI

`AppUploadDocumental` debe delegar solo la operacion tecnica al cliente. La UI sigue siendo responsable de:

- seleccionar archivos;
- validar reglas visuales o de negocio propias de pantalla;
- decidir mensajes al usuario;
- construir eventos visuales;
- mapear progreso a barras, listas o estados;
- interpretar `rawBackendResult` si el backend devuelve datos adicionales.

Ejemplo conceptual:

```ts
const controller = new AbortController();

const result = await uploadAndStoreOneDocument({
  fileUid: "file-001",
  file,
  signal: controller.signal,
  initialChunkSizeBytes: 4 * 1024 * 1024,
  request: {
    nombreGabinete: "Gestion",
    nombreDocumento: file.name,
    requestId: crypto.randomUUID(),
    camposIndexacion: [
      {
        nombreCampo: "NumeroRadicado",
        valor: "RAD-001",
        esObligatorio: true,
      },
    ],
    trd: {
      idTipoDocumento: 10,
      nombreTipoDocumento: "Contrato",
    },
    expediente: {
      idExpediente: 20,
      idTipoExpediente: 3,
    },
    workflow: {
      idTareaWorkflow: 100,
      idRutaWorkflow: 5,
    },
    documento: {
      numeroPaginas: 12,
    },
  },
  onProgress: (progress: UploadStorageProgress) => {
    // La UI decide como representar este evento.
    updateFileProgress(progress.fileUid, progress);
  },
});

const almacen = result.response;
const backendExtra = result.rawBackendResult;
```

## Flujo interno que ejecuta el cliente

Una llamada a `uploadAndStoreOneDocument` ejecuta:

```txt
1. POST upload-temporal/init
2. PUT chunk 0..N con bytes crudos
3. POST upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
4. POST /api/gestor-documental/almacenamiento
```

La llamada final de almacenamiento se hace por archivo y contiene un solo `DocumentoEntrada`.

## Progreso

El callback `onProgress` recibe:

```ts
type UploadStorageProgress = {
  fileUid: string;
  phase: "initializing" | "uploading" | "completing" | "storing";
  chunkIndex?: number;
  totalChunks?: number;
  loadedBytes?: number;
  totalBytes?: number;
  percent: number;
};
```

Reglas para consumidores:

- `phase` indica la etapa tecnica.
- `percent` siempre queda entre 0 y 100.
- `chunkIndex` es base cero.
- `loadedBytes` y `totalBytes` estan disponibles durante upload por chunks.
- La UI no debe asumir que `uploading 100` equivale a documento almacenado; despues vienen `completing` y `storing`.

## Cancelacion

La cancelacion debe hacerse con `AbortController`:

```ts
const controller = new AbortController();

const promise = uploadAndStoreOneDocument({
  fileUid: "file-001",
  file,
  signal: controller.signal,
  request,
});

controller.abort();

await promise;
```

Si la cancelacion ocurre despues de `init`, el cliente intenta:

```txt
DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
```

Si ese `DELETE` falla, el error principal sigue siendo `storage_aborted` y el fallo de cleanup queda en `details.cancelWarning`.

## Manejo de errores

El servicio propaga `AlmacenamientoDocumentalUploadError`:

```ts
try {
  await uploadAndStoreOneDocument(input);
} catch (error) {
  if (error instanceof AlmacenamientoDocumentalUploadError) {
    switch (error.code) {
      case "storage_aborted":
        break;
      case "storage_contract_error":
        break;
      case "storage_chunk_error":
        break;
      case "storage_complete_error":
        break;
      case "storage_store_error":
        break;
    }
  }
}
```

Codigos disponibles:

```txt
storage_contract_error
storage_init_error
storage_chunk_error
storage_status_error
storage_complete_error
storage_cancel_error
storage_store_error
storage_aborted
```

Reglas para UI:

- No mostrar `details` completos al usuario final si pueden contener payload tecnico.
- Usar `code` y `phase` para mensajes controlados.
- Loguear solo metadatos seguros si se requiere observabilidad.

## Uso avanzado por pasos

Wrappers especializados pueden usar la API granular:

```ts
const temporal = await initTemporaryUpload(initRequest, signal);

await uploadTemporaryChunk({
  rutaTemporalId: temporal.rutaTemporalId,
  archivoTemporalId: temporal.archivoTemporalId,
  chunkIndex: 0,
  totalChunks: 1,
  chunk,
  signal,
});

await completeTemporaryUpload({
  rutaTemporalId: temporal.rutaTemporalId,
  archivoTemporalId: temporal.archivoTemporalId,
  signal,
});

const response = await almacenarDocumento(request, signal);
```

Este modo debe usarse solo cuando el consumidor necesita controlar explicitamente cada fase.

## Contratos de entrada clave

`request` en `uploadAndStoreOneDocument` debe incluir los campos finales excepto `rutaTemporalId` y `documentos`, porque esos los construye el cliente:

```ts
type UploadOneDocumentInput["request"] =
  Omit<AlmacenarDocumentoRequest, "rutaTemporalId" | "documentos"> & {
    documento?: Partial<DocumentoEntrada>;
  };
```

El cliente completa:

```txt
rutaTemporalId
documentos[0].archivoTemporalId
documentos[0].nombreOriginal
documentos[0].extension
```

## Lo que el consumidor no debe hacer

- No llamar `.ashx`.
- No usar `FormData` para chunks.
- No usar `XMLHttpRequest`.
- No duplicar el slicing de archivo si usa `uploadAndStoreOneDocument`.
- No persistir `File` en stores globales.
- No interpretar `rawBackendResult` dentro del cliente.
- No acoplar este servicio a componentes visuales.

## Resultado esperado

```ts
type UploadOneDocumentResult = {
  temporal: StorageUploadInitResponse;
  response: AlmacenarDocumentoResponse;
  rawBackendResult?: unknown;
};
```

`response` es el contrato normalizado para almacenamiento.

`rawBackendResult` conserva campos adicionales del backend para que `AppUploadDocumental` construya eventos visuales o de registro sin que el cliente tecnico conozca UI.
