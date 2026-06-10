# PROMPT ARQUITECTONICO - upload-storage-client

## Rol esperado

Arquitecto frontend senior

React 19, TypeScript estricto, integracion API enterprise, Axios, contratos backend, upload por chunks, cancelacion, guards runtime, idempotencia, testing de servicios.

## Objetivo

Implementar el cliente tecnico de almacenamiento documental para la nueva API:

```txt
init -> chunks -> complete -> almacenar
```

El cliente debe ser independiente de UI y reusable por:

- `AppUploadDocumental`;
- flujos futuros de carga documental;
- procesos batch que requieran almacenamiento temporal;
- potenciales wrappers especializados.

## IMPORTANTE

Este ticket NO debe:

- crear componentes React;
- renderizar modales;
- manejar tipologias en UI;
- decidir layout;
- usar `FormData` legacy;
- usar `XMLHttpRequest`;
- llamar `.ashx`;
- usar jQuery;
- inventar endpoints;
- modificar backend;
- introducir `any` nuevo;
- ocultar errores de contrato.

Este ticket SI debe:

- usar `clienteApi`;
- mapear contratos reales de backend;
- subir chunks como bytes crudos;
- reportar progreso;
- soportar cancelacion con `AbortSignal`;
- llamar `DELETE upload-temporal` cuando aplique;
- validar shape de respuestas;
- construir payload final por archivo;
- preservar el resultado backend necesario para que capas superiores construyan eventos de registro en interfaz;
- emitir errores tipados;
- no persistir URLs temporales;
- no loguear payload sensible;
- tener pruebas unitarias de servicio.

## Dependencias

- `src/api/Clienteaxios.ts`
- Axios con soporte `signal`.
- TypeScript estricto.
- API backend de almacenamiento documental.

## Contrato backend obligatorio

No inferir contratos manualmente. Leer y reflejar los DTOs reales desde:

```txt
D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\GestorDocumental\AlmacenamientoDocumental\AlmacenamientoDocumentalController.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\TemporaryUpload\StorageUploadInitRequestDto.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\TemporaryUpload\StorageUploadInitResponseDto.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\AlmacenarDocumentoRequest.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\AlmacenarDocumentoResponse.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\DocumentoEntradaDto.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\TrdStorageDto.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\ExpedienteStorageDto.cs
D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\WorkflowStorageDto.cs
```

Endpoints obligatorios:

```txt
POST /api/gestor-documental/almacenamiento/upload-temporal/init
PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
POST /api/gestor-documental/almacenamiento
```

## Ubicacion esperada

Servicios:

```txt
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
```

Tipos:

```txt
src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts
```

Utils:

```txt
src/modules/almacenamientoDocumental/utils/storageFile.utils.ts
```

Tests:

```txt
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
src/modules/almacenamientoDocumental/utils/storageFile.utils.test.ts
```

## Estructura de archivos obligatoria

Crear o completar:

```txt
src/modules/almacenamientoDocumental/
├─ services/
│  └─ almacenamientoDocumentalUpload.service.ts
├─ types/
│  └─ almacenamientoDocumental.types.ts
└─ utils/
   └─ storageFile.utils.ts
```

Tests:

```txt
src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.test.ts
src/modules/almacenamientoDocumental/utils/storageFile.utils.test.ts
```

## Tipos frontend obligatorios

Modelar los DTOs backend con nombres frontend claros:

```ts
export type StorageUploadInitRequest = {
  nombreOriginal: string;
  tamanoBytes: number;
  extension: string;
  hashSha256Esperado?: string | null;
  numeroChunks: number;
};

export type StorageUploadInitResponse = {
  rutaTemporalId: string;
  archivoTemporalId: string;
  chunkSizeBytes: number;
  estado: string;
};

export type DocumentoEntrada = {
  idDocumento?: number | null;
  archivoTemporalId: string;
  nombreOriginal: string;
  extension: string;
  numeroPaginas?: number | null;
};

export type AlmacenarDocumentoRequest = {
  nombreGabinete: string;
  rutaTemporalId: string;
  nombreDocumento: string;
  requestId: string;
  documentos: DocumentoEntrada[];
  camposIndexacion?: Array<{
    nombreCampo: string;
    valor?: string | null;
    esObligatorio?: boolean | null;
  }> | null;
  inventario?: unknown;
  trd?: {
    idTipoDocumento?: number | null;
    nombreTipoDocumento?: string | null;
  } | null;
  expediente?: {
    idExpediente?: number | null;
    idTipoExpediente?: number | null;
  } | null;
  workflow?: {
    idTareaWorkflow?: number | null;
    idRutaWorkflow?: number | null;
  } | null;
  fullText?: string | null;
  numeroPaginasDeclaradas?: number | null;
};

export type AlmacenarDocumentoResponse = {
  idAlmacen: number;
  idRegistroProduccionDocumental: number;
  nombreArchivoFinal: string;
  requestId: string;
};
```

Si los DTOs reales tienen campos adicionales o nombres distintos, ajustar estos tipos al contrato real y documentar la evidencia.

No usar `any`. Para campos aun no modelados, usar `unknown` y guards.

## API publica del servicio

Exponer funciones puras o de servicio:

```ts
export type UploadStorageProgress = {
  fileUid: string;
  phase: "initializing" | "uploading" | "completing" | "storing";
  chunkIndex?: number;
  totalChunks?: number;
  loadedBytes?: number;
  totalBytes?: number;
  percent: number;
};

export type UploadOneDocumentInput = {
  fileUid: string;
  file: File;
  request: Omit<AlmacenarDocumentoRequest, "rutaTemporalId" | "documentos"> & {
    documento?: Partial<DocumentoEntrada>;
  };
  initialChunkSizeBytes?: number;
  signal?: AbortSignal;
  onProgress?: (progress: UploadStorageProgress) => void;
};

export type UploadOneDocumentResult = {
  temporal: StorageUploadInitResponse;
  response: AlmacenarDocumentoResponse;
  rawBackendResult?: unknown;
};

export async function initTemporaryUpload(
  request: StorageUploadInitRequest,
  signal?: AbortSignal
): Promise<StorageUploadInitResponse>;

export async function uploadTemporaryChunk(input: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  chunkIndex: number;
  totalChunks: number;
  chunk: Blob;
  signal?: AbortSignal;
}): Promise<void>;

export async function completeTemporaryUpload(input: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  signal?: AbortSignal;
}): Promise<void>;

export async function cancelTemporaryUpload(input: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  signal?: AbortSignal;
}): Promise<void>;

export async function almacenarDocumento(
  request: AlmacenarDocumentoRequest,
  signal?: AbortSignal
): Promise<AlmacenarDocumentoResponse>;

export async function uploadAndStoreOneDocument(
  input: UploadOneDocumentInput
): Promise<UploadOneDocumentResult>;
```

## Regla de retorno para interfaz

El cliente tecnico no debe decidir como actualizar la interfaz. Sin embargo, debe preservar suficiente informacion para que `AppUploadDocumental` pueda construir eventos tipados de registro visual.

Reglas:

- `almacenarDocumento` debe retornar la respuesta normalizada `AlmacenarDocumentoResponse`.
- Si el backend devuelve campos adicionales relevantes, conservarlos como `rawBackendResult?: unknown`.
- No concatenar campos con separador `|`.
- No transformar respuestas segun nombres legacy como `funcion_name`.
- No llamar callbacks de interfaz.
- No importar tipos de `AppUploadDocumental`.
- No descartar campos backend que no esten modelados si pueden ser necesarios para el consumidor.

La interpretacion del retorno vive en `AppUploadDocumental`, no en este cliente.

## Reglas de chunks obligatorias

- `init` requiere `numeroChunks`; calcularlo con `initialChunkSizeBytes`.
- Si backend responde `chunkSizeBytes` diferente, recalcular `totalChunks` real antes de subir chunks.
- Subir cada chunk con `Content-Type: application/octet-stream`.
- Enviar header `X-Total-Chunks`.
- `chunkIndex` debe respetar el contrato backend real. Si el backend espera base cero, usar base cero; si espera base uno, documentarlo y testearlo.
- No usar `FormData` para chunks.
- No cargar el archivo completo en memoria si `Blob.slice` permite partirlo.

## Regla de registro final por archivo

Por decision arquitectonica:

```txt
Seleccion multiple frontend
-> procesamiento secuencial por archivo
-> un POST final /api/gestor-documental/almacenamiento por archivo
```

El cliente debe soportar un solo documento por llamada final cuando lo use `AppUploadDocumental`.

## Guards runtime obligatorios

Validar respuestas:

- `rutaTemporalId` string no vacio;
- `archivoTemporalId` string no vacio;
- `chunkSizeBytes` numero positivo;
- `idAlmacen` numero valido;
- `idRegistroProduccionDocumental` numero valido;
- `nombreArchivoFinal` string no vacio;
- `requestId` string no vacio.

Si la respuesta viene envuelta en `success/data`, usar el wrapper real del proyecto. No asumir shape sin leer patrones existentes.

## Manejo de errores obligatorio

Clasificar errores:

```txt
storage_contract_error
storage_init_error
storage_chunk_error
storage_complete_error
storage_cancel_error
storage_store_error
storage_aborted
```

Reglas:

- si falla `init`, no intentar chunks;
- si falla un chunk, abortar archivo y propagar error;
- si falla `complete`, no llamar almacenamiento final;
- si falla almacenamiento final, devolver error con contexto suficiente;
- si hay ids temporales y se cancela, intentar `DELETE`;
- si `DELETE` falla despues de abortar, reportarlo como warning si la operacion principal ya fue cancelada.

## Seguridad

- usar `clienteApi`;
- no loguear tokens;
- no loguear bytes de archivo;
- no persistir URLs temporales;
- no guardar `File` en storage global;
- no exponer payload sensible en errores visibles.

## Pruebas unitarias obligatorias

- calcula extension normalizada;
- calcula chunks iniciales;
- recalcula chunks con `chunkSizeBytes` backend;
- llama `init` con payload esperado;
- llama `PUT chunk` con bytes crudos;
- envia `X-Total-Chunks`;
- llama `complete` despues de todos los chunks;
- llama `POST almacenamiento` despues de `complete`;
- no llama `POST almacenamiento` si falla chunk;
- no llama `POST almacenamiento` si falla complete;
- llama `DELETE` al cancelar con ids temporales;
- valida shape invalido de `init`;
- valida shape invalido de almacenamiento final;
- propaga `AbortError` o equivalente tipado.

## Pruebas de integracion obligatorias

- `uploadAndStoreOneDocument` ejecuta `init -> chunks -> complete -> store`;
- progreso reporta fases correctas;
- cancelacion durante chunks aborta llamadas posteriores;
- respuesta final retorna temporal y almacenamiento.
- respuesta final conserva `rawBackendResult` cuando el backend trae campos adicionales.

## Criterios de aceptacion

- Servicio sin UI implementado.
- API nueva de almacenamiento usada con endpoints correctos.
- Chunks enviados como `application/octet-stream`.
- Registro final por archivo soportado.
- Resultado backend preservado para normalizacion de interfaz en capas superiores.
- Cancelacion temporal implementada.
- Guards runtime implementados.
- Sin `any` nuevo.
- Tests cubren flujo feliz, errores y cancelacion.

## Entrega esperada

- Diff de servicios, tipos y utilidades.
- Matriz campo a campo con evidencia backend.
- Evidencia de tests ejecutados.
- Confirmacion explicita:
  - backend no modificado;
  - endpoints no modificados;
  - no se uso `.ashx`;
  - no se uso `FormData` legacy;
  - cliente queda reusable y sin UI.

## Instruccion final

Implementar el cliente tecnico de almacenamiento documental usando `clienteApi`, contratos backend reales, upload temporal por chunks, complete, cancelacion y registro final por archivo, con guards runtime, errores tipados, progreso por fase y pruebas completas, sin dependencias de UI ni legado.
