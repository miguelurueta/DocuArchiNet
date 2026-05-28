# Modelo de requisitos: AppUploadDocumental

## Objetivo

Definir un componente documental que adapte el componente reusable `AppUpload` a la nueva API de almacenamiento documental, conservando la logica funcional util del legacy `FileUploadHandler.js` sin migrar su UI ni sus dependencias antiguas.

## Contexto legacy

El componente legacy `FileUploadHandler.js` permite:

- seleccionar uno o varios archivos;
- configurar extensiones permitidas y tamano maximo desde servicio;
- mostrar archivos pendientes en tabla;
- asignar tipologia documental por archivo;
- sugerir tipologia comparando el nombre del archivo con la lista de tipologias;
- validar tipologia obligatoria;
- subir cada archivo con metadata;
- permitir carga individual por archivo y carga masiva;
- capturar fecha por archivo cuando el proceso lo exige;
- ajustar extensiones permitidas segun modo documental;
- refrescar la interfaz mediante callbacks globales.

La nueva implementacion debe usar React, `AppUpload`, servicios tipados y la API nueva de almacenamiento por chunks.

## Componentes relacionados

```txt
AppUploadDocumental
├─ AppUpload
└─ AppProgressBatch
```

- `AppUpload`: componente existente para seleccion, drag and drop, preview y estado por archivo.
- `AppProgressBatch`: componente generico para procesar multiples archivos de forma secuencial.
- `AppUploadDocumental`: adaptador de negocio documental.

## Alcance

Incluye:

- carga de configuracion de upload desde API;
- carga de tipologias documentales desde API;
- seleccion multiple de documentos;
- tipologia por archivo;
- sugerencia automatica de tipologia por nombre de archivo;
- validacion local con reglas recibidas del backend;
- carga temporal por chunks;
- registro final por archivo en la nueva API;
- accion de guardar archivo individual;
- fecha documental por archivo;
- modos documentales que alteran reglas de extension y metadata;
- callbacks de exito/error para que el modulo consumidor refresque listados.

No incluye:

- reimplementar `AppUpload`;
- migrar modales Bootstrap legacy;
- migrar jQuery;
- usar `FormData` legacy;
- registrar multiples tipologias en un solo request final;
- cambiar contrato backend para TRD por item.

## Decision arquitectonica

La API actual de almacenamiento tiene `trd` a nivel global del request final. Como el legacy permite una tipologia diferente por archivo, la migracion adoptara esta decision:

```txt
Seleccion multiple en frontend
→ procesamiento secuencial por archivo
→ un POST final /api/gestor-documental/almacenamiento por archivo
```

Esto permite enviar el `trd` global con la tipologia especifica de cada archivo sin modificar backend.

## Ubicacion propuesta

```txt
src/modules/almacenamientoDocumental/
├─ components/
│  └─ AppUploadDocumental/
│     ├─ AppUploadDocumental.tsx
│     ├─ AppUploadDocumental.types.ts
│     ├─ AppUploadDocumental.module.css
│     └─ index.ts
├─ services/
│  ├─ almacenamientoDocumentalUpload.service.ts
│  ├─ uploadConfig.service.ts
│  └─ tipoDocumental.service.ts
├─ utils/
│  ├─ storageFile.utils.ts
│  └─ tipoDocumentalSuggestion.utils.ts
└─ types/
   └─ almacenamientoDocumental.types.ts
```

## Contrato propuesto

```ts
export type UploadDocumentalProcessKey = string;

export type UploadDocumentalContext = {
  nombreGabinete: string;
  idExpediente?: number;
  idTipoExpediente?: number;
  idUnidadConservacion?: number;
  idClaseDocumento?: number;
  idTareaWorkflow?: number;
  idRutaWorkflow?: number;
  idRespuesta?: number;
  tipoAdjunta?: number;
  estadoAdjunto?: number;
  estadoRelacionado?: number;
  numeroDocumentoRelacionado?: number;
  idImagen?: number;
  nameModulo?: string;
  camposIndexacion?: Array<{
    nombreCampo: string;
    valor?: string;
    esObligatorio?: boolean;
  }>;
};

export type TipoDocumentalOption = {
  idTipoDocumento: number;
  nombreTipoDocumento: string;
};

export type UploadDocumentalFileMetadata = {
  uid: string;
  idTipoDocumento?: number;
  nombreTipoDocumento?: string;
  numeroPaginas?: number;
  fechaCarga?: string;
  error?: string;
};

export type AppUploadDocumentalProps = {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
  tipologiaObligatoria?: boolean;
  autoSuggestTipologia?: boolean;
  requiereFechaCarga?: boolean;
  allowSingleFileStore?: boolean;
  validationMode?: "reject" | "queue-with-error";
  modoDocumento?: "default" | "adjunto-radicado" | "relacionado-radicado" | "formato-respuesta" | "documento-libre-respuesta";
  onStored?: (result: AlmacenarDocumentoStoredResult) => void;
  onBatchComplete?: (summary: UploadDocumentalBatchSummary) => void;
  onError?: (error: unknown) => void;
};
```

## API nueva de almacenamiento

Base:

```txt
/api/gestor-documental/almacenamiento
```

### Inicializar upload temporal

```txt
POST /api/gestor-documental/almacenamiento/upload-temporal/init
```

Body:

```json
{
  "nombreOriginal": "documento.pdf",
  "tamanoBytes": 123456,
  "extension": ".pdf",
  "hashSha256Esperado": null,
  "numeroChunks": 4
}
```

Respuesta relevante:

```json
{
  "rutaTemporalId": "...",
  "archivoTemporalId": "...",
  "chunkSizeBytes": 1048576,
  "estado": "..."
}
```

### Subir chunk

```txt
PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
Content-Type: application/octet-stream
X-Total-Chunks: {totalChunks}
```

### Completar upload temporal

```txt
POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
```

### Cancelar upload temporal

```txt
DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
```

### Registrar documento final

```txt
POST /api/gestor-documental/almacenamiento
```

El registro final se ejecuta una vez por archivo.

## Requisitos funcionales

### RF-UD-01 Carga de configuracion desde API

El componente debe consultar configuracion de carga antes de habilitar seleccion de archivos.

Debe obtener:

- extensiones permitidas;
- tamano maximo por archivo;
- si permite seleccion multiple, si aplica;
- si requiere tipologia;
- si requiere fecha por archivo;
- reglas adicionales del proceso, si existen.

Equivalencia legacy:

```txt
Service_parameter_upload
CONTENT_ESTENSION_PERMITIDA
CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD
CONTENT_SELECT_FILE_UPLOAD
```

### RF-UD-02 Validacion local con configuracion backend

El componente debe configurar `AppUpload.accept` y `AppUpload.maxSize` con los valores recibidos desde la API.

La validacion local es preventiva; backend mantiene la autoridad final.

El componente debe soportar dos politicas:

- `reject`: el archivo invalido no entra a la cola;
- `queue-with-error`: el archivo aparece en la lista con error y no puede guardarse.

La politica `queue-with-error` conserva el comportamiento de la variante dinamica legacy, donde un archivo grande puede mostrarse con advertencia y bloquear su guardado.

### RF-UD-03 Seleccion multiple

El componente debe permitir seleccionar multiples archivos cuando la configuracion lo permita.

Debe soportar:

- input file;
- drag and drop;
- eliminar archivo;
- limpiar lista;
- preview cuando `AppUpload` lo soporte.

### RF-UD-03A Acciones por archivo

El componente debe permitir ejecutar acciones sobre un archivo individual:

- eliminar;
- previsualizar, si esta habilitado;
- guardar/subir individualmente, si `allowSingleFileStore` esta habilitado.

Equivalencia legacy:

```txt
_EventEnviarArchivoServer
```

### RF-UD-04 Carga de tipologias documentales

El componente debe consultar las tipologias documentales segun el proceso y contexto.

Debe cargar opciones antes o durante la seleccion, segun UX final, pero antes de guardar.

### RF-UD-05 Tipologia por archivo

Cada archivo seleccionado debe tener su propia metadata de tipologia:

```ts
{
  idTipoDocumento?: number;
  nombreTipoDocumento?: string;
}
```

El usuario debe poder cambiar la tipologia por archivo.

### RF-UD-05A Metadata documental heredada

El contexto debe poder transportar metadata equivalente a los campos legacy:

- `tipo_adjunta`;
- `id_respuesta`;
- `estado_adjunto`;
- `estado_relacionado`;
- `numero_documento_relacionado`;
- `gabinete`;
- `id_imagen`;
- `id_expediente`;
- `name_modulo`.

La implementacion debe mapear solo los campos compatibles con la nueva API y exponer los demas al modulo consumidor mediante callbacks o extensiones de payload.

### RF-UD-06 Tipologia obligatoria

Cuando `tipologiaObligatoria` sea verdadero, el componente debe bloquear la carga de cualquier archivo sin tipologia seleccionada.

Equivalencia legacy:

```txt
Debe seleccionar una tipologia para cada archivo cargado.
```

### RF-UD-07 Sugerencia automatica de tipologia

Cuando `autoSuggestTipologia` sea verdadero, el componente debe sugerir una tipologia con base en el nombre del archivo.

Regla base:

- convertir nombre de archivo a mayusculas;
- quitar extension;
- separar por palabras;
- comparar contra `nombreTipoDocumento`;
- elegir la opcion con mayor coincidencia;
- permitir override manual.

La comparacion debe soportar coincidencia flexible por subcadenas con longitud minima configurable. El legacy usa una longitud minima de 4 caracteres.

### RF-UD-07A Modos documentales y extension efectiva

El componente debe poder ajustar la extension efectiva segun el modo documental seleccionado.

Casos legacy relevantes:

- `adjunto_doc_visor` con anexo radicado: restringe a `.TIF`;
- `adjunto_doc_visor` relacionado: usa extensiones permitidas generales;
- `adjunto_doc_respuesta` con formato: restringe a `.DOCX`;
- `adjunto_doc_respuesta` libre: usa extensiones permitidas generales.

Estas reglas deben venir preferiblemente desde la API de configuracion. Si se modelan en frontend, deben quedar aisladas en una utilidad testeable.

### RF-UD-07B Fecha por archivo

Cuando `requiereFechaCarga` sea verdadero, cada archivo debe permitir capturar una fecha en formato `yyyy-MM-dd`.

La fecha:

- es opcional salvo que el proceso la marque obligatoria;
- no debe permitir anos futuros;
- debe validar mes, dia y longitud;
- debe mapearse a `fechaCarga` en la metadata del archivo;
- debe enviarse en `camposIndexacion`, metadata extendida o contrato backend disponible.

Equivalencia legacy:

```txt
CargaFecha
FechaCarga
element_date_{rowId}
```

### RF-UD-08 Procesamiento secuencial por archivo

Cuando el usuario inicia carga de multiples archivos, el componente debe procesar uno por uno.

Debe apoyarse en `AppProgressBatch` para:

- progreso global;
- cancelacion;
- errores controlados;
- resumen final.

El componente debe evitar cerrar/desmontar el flujo mientras exista una carga activa sin pasar por cancelacion.

### RF-UD-09 Upload temporal por chunks

Por cada archivo, el servicio debe:

1. llamar `upload-temporal/init`;
2. calcular chunks usando `chunkSizeBytes` de la respuesta;
3. subir cada chunk como bytes crudos;
4. reportar progreso;
5. llamar `complete`.

### RF-UD-10 Registro final por archivo

Por cada archivo completado, el servicio debe llamar:

```txt
POST /api/gestor-documental/almacenamiento
```

El body debe incluir un solo item en `documentos[]` y `trd` con la tipologia del archivo actual.

Si el archivo tiene fecha o metadata adicional no representada directamente en `AlmacenarDocumentoRequest`, el componente debe enviarla por el canal acordado para el modulo consumidor, por ejemplo `camposIndexacion`.

### RF-UD-11 Payload final por archivo

El request final debe mapear:

```txt
context.nombreGabinete
→ nombreGabinete

rutaTemporalId
→ rutaTemporalId

file.name
→ nombreDocumento

crypto.randomUUID()
→ requestId

archivoTemporalId
→ documentos[0].archivoTemporalId

metadata.idTipoDocumento
→ trd.idTipoDocumento

metadata.nombreTipoDocumento
→ trd.nombreTipoDocumento

context.idExpediente
→ expediente.idExpediente

context.idTareaWorkflow
→ workflow.idTareaWorkflow

metadata.fechaCarga
→ camposIndexacion o campo backend acordado

context.idImagen
→ callback/contexto del modulo consumidor si la API final no lo soporta
```

### RF-UD-12 Cancelacion de upload

Si el usuario cancela mientras un archivo tiene upload temporal iniciado, el servicio debe intentar llamar:

```txt
DELETE /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}
```

### RF-UD-13 Resultado por archivo

Al almacenar un archivo correctamente, el componente debe emitir `onStored` con:

- `idAlmacen`;
- `idRegistroProduccionDocumental`;
- `nombreArchivoFinal`;
- `requestId`.

El callback debe recibir tambien el archivo local y su metadata para que el consumidor pueda actualizar tablas, contadores o visores sin depender de funciones globales.

### RF-UD-13A Retorno para registro en interfaz

El legacy no retorna un unico tipo de dato para registrar el resultado en pantalla. Segun `funcion_name`, el resultado puede alimentar distintos destinos visuales:

- fila de produccion documental;
- fila de documento relacionado;
- enlace de workflow;
- visor de migracion mediante URL;
- contador de paginas;
- imagen de semaforo;
- opcion de dropdown de anexos;
- fila de version documental;
- actualizacion de icono/estado de firma;
- tabla de importacion RUE/SII o virtual SII.

La nueva implementacion NO debe mutar DOM ni despachar por callbacks string. Debe emitir eventos tipados para que el modulo consumidor actualice su propia interfaz.

Contrato recomendado:

```ts
export type UploadDocumentalInterfaceRegistration =
  | {
      kind: "production-document-row";
      idRegistro: number;
      idImagen?: number;
      nombreArchivo: string;
      fecha?: string;
      tipoDocumental?: string;
      nombreGabinete?: string;
      alias?: string;
      estadoFirmaDigital?: string;
      iconName?: string;
    }
  | {
      kind: "related-document-row" | "workflow-document-row";
      nombreGabinete?: string;
      idImagen?: number;
      radicado?: string;
      tipoDocumental?: string;
      nombreTipoDocumental?: string;
      idTareaWorkflow?: number;
      estadoFirmaDigital?: string;
      iconName?: string;
    }
  | {
      kind: "migration-preview";
      url: string;
      idRegistro?: number;
    }
  | {
      kind: "page-counter";
      contadorPaginas: number;
    }
  | {
      kind: "traffic-light";
      urlImagenSemaforo: string;
    }
  | {
      kind: "dropdown-option";
      text: string;
      value: string | number;
      target?: "respuesta" | "pqrs" | "anexo";
    }
  | {
      kind: "document-version-row";
      idImagen?: number;
      idVersionDocumento?: number;
      idRegistroVersion?: number;
      tipoDocumento?: string;
      estadoFirmaDigital?: string;
      iconName?: string;
      dbt?: number;
      fechaRegistroVersion?: string;
    }
  | {
      kind: "table-import-result";
      rowTable: unknown;
      fieldTable: unknown;
      source: "rue-sii" | "virtual-sii";
    }
  | {
      kind: "raw";
      raw: unknown;
    };
```

`onStored` debe poder incluir `interfaceRegistration?: UploadDocumentalInterfaceRegistration[]`.

Si el backend nuevo no retorna todos estos campos, el componente debe:

- mapear los campos disponibles;
- conservar `raw` solo como dato opaco para el consumidor;
- no inventar datos;
- no concatenar strings con separador `|`;
- no llamar funciones globales como `insert_row_producion_documental`, `insert_row_documento_relacionado` o `insert_new_versio_document`.

### RF-UD-14 Resultado batch

Al finalizar el lote, el componente debe emitir `onBatchComplete` para que el modulo consumidor pueda refrescar listados.

### RF-UD-15 Errores

El componente debe reportar errores mediante `onError` y reflejarlos visualmente en el archivo correspondiente.

Debe distinguir al menos:

- error de configuracion;
- error de tipologias;
- error de validacion local;
- error de chunk;
- error de complete;
- error de registro final;
- cancelacion.

### RF-UD-16 Fases visibles de upload

El componente debe reportar fases de proceso por archivo:

- `validating`;
- `initializing`;
- `uploading`;
- `completing`;
- `storing`;
- `done`;
- `error`;
- `cancelled`.

Equivalencia legacy:

```txt
Cargando...
Guardando...
```

### RF-UD-17 Conteo de archivos

El componente debe exponer visualmente o por estado el conteo de archivos seleccionados y pendientes.

Equivalencia legacy:

```txt
N Archivo(s) Cargado(s)
```

## Requisitos no funcionales

### RNF-UD-01 Sin UI legacy

No se debe migrar:

- tabla HTML manual;
- modales Bootstrap;
- jQuery;
- `$find`;
- variables globales;
- callbacks por string.

### RNF-UD-02 Reutilizacion de AppUpload

El componente debe envolver `AppUpload`; no debe duplicar su funcionalidad base.

### RNF-UD-03 Servicios aislados

La comunicacion HTTP debe vivir en servicios separados:

- `uploadConfig.service.ts`;
- `tipoDocumental.service.ts`;
- `almacenamientoDocumentalUpload.service.ts`.

### RNF-UD-04 Utilidades puras

La logica de:

- extraer extension;
- calcular chunks;
- sugerir tipologia;
- validar fecha;
- resolver extension efectiva por modo documental;
- construir payload final;

debe estar en funciones puras testeables.

### RNF-UD-05 Seguridad

Debe usar `clienteApi` autenticado. La API requiere `[Authorize]` y claims como `usuarioid` y `defaulalias`.

### RNF-UD-06 Idempotencia

Cada registro final debe enviar un `requestId` unico.

### RNF-UD-07 Observabilidad

Debe conservar eventos suficientes para diagnosticar:

- inicio de upload;
- chunk fallido;
- cancelacion;
- complete fallido;
- registro final fallido;
- resultado final por archivo.

## Equivalencia legacy

```txt
LoadFilePERSON
→ AppUploadDocumental

JSProgresBar
→ AppProgressBatch

CONTENT_ESTENSION_PERMITIDA
→ config.accept

CONTENT_MAXIMO_TAMANO_FILE_BYTE_UPLOAD
→ config.maxSize

CONTENT_ITEM_ROW_TIPO
→ tipoDocumentalOptions

element_input_{rowId}
→ metadataPorArchivo[uid].idTipoDocumento

_BuscaCoinsidenciaEstructura
→ suggestTipoDocumentalFromFileName

_EnviaArchivoServidor
→ uploadTemporalPorChunks + almacenarDocumento

funcion_name
→ onStored / onBatchComplete

CargaFecha / FechaCarga
→ requiereFechaCarga / metadata.fechaCarga

estado_adjunto / estado_relacionado / numero_documento_relacionado
→ UploadDocumentalContext y payload/callback segun modulo

upload_file_config_aceptar
→ resolver extension efectiva desde configuracion y modo documental

_EventEnviarArchivoServer
→ guardar archivo individual
```

## Criterios de aceptacion

- Dado un proceso documental, cuando abre el componente, entonces consulta configuracion de extensiones y tamano antes de habilitar carga.
- Dado un archivo con extension no permitida por backend, cuando se selecciona, entonces no entra a la cola.
- Dado multiples archivos, cuando se agregan, entonces cada uno tiene metadata independiente de tipologia.
- Dado un archivo invalido y `validationMode=queue-with-error`, cuando se selecciona, entonces aparece con error y no se puede guardar.
- Dado un nombre de archivo similar a una tipologia, cuando se agrega, entonces se preselecciona la mejor coincidencia.
- Dado `tipologiaObligatoria=true`, cuando un archivo no tiene tipologia, entonces la carga no inicia.
- Dado `requiereFechaCarga=true`, cuando un archivo tiene fecha invalida, entonces la carga no inicia para ese archivo.
- Dado un modo documental que restringe extension, cuando cambia el modo, entonces se recalcula el `accept` efectivo.
- Dado multiples archivos validos, cuando inicia la carga, entonces se procesa un archivo por vez.
- Dado un archivo individual, cuando el usuario ejecuta guardar, entonces solo se procesa ese archivo.
- Dado un archivo procesado, cuando termina el upload temporal, entonces se registra con un POST final individual.
- Dado un batch exitoso, cuando finaliza, entonces se emite `onBatchComplete` para refrescar listados.
- Dado una cancelacion, cuando existe upload temporal activo, entonces se intenta cancelar en backend.
