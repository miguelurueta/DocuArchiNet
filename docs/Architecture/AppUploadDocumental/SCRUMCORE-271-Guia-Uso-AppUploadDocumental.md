# SCRUMCORE-271 - Guia de uso AppUploadDocumental

## Uso embebido

```tsx
<AppUploadDocumental
  proceso="radicacion"
  context={{
    nombreGabinete: "Gestion",
    idExpediente: 15,
    idTipoExpediente: 3,
  }}
  loadConfig={loadUploadConfig}
  loadTiposDocumentales={loadTiposDocumentales}
  onStored={(result) => {
    refrescarDocumento(result);
  }}
/>
```

## Uso modal/controlado

```tsx
<AppUploadDocumental
  embedded={false}
  open={open}
  proceso="workflow"
  context={{
    nombreGabinete: "Workflow",
    idTareaWorkflow: 7,
    idRutaWorkflow: 2,
  }}
  loadConfig={loadUploadConfig}
  loadTiposDocumentales={loadTiposDocumentales}
  onClose={() => setOpen(false)}
  onBatchComplete={(summary) => setSummary(summary)}
/>
```

## Loaders

Los loaders son obligatorios:

```ts
type LoadConfig = (input: {
  proceso: string;
  context: UploadDocumentalContext;
  modoDocumento?: AppUploadDocumentalModoDocumento;
}) => Promise<UploadDocumentalConfig>;

type LoadTiposDocumentales = (input: {
  proceso: string;
  context: UploadDocumentalContext;
}) => Promise<TipoDocumentalOption[]>;
```

`loadConfig` debe retornar reglas reales del proceso:

```ts
{
  accept: ".pdf,.png",
  allowedExtensions: [".pdf", ".png"],
  maxSizeBytes: 10485760,
  multiple: true,
  requiereTipologia: true,
  requiereFechaCarga: true,
  fechaCargaObligatoria: true,
  validationMode: "queue-with-error",
  preferredChunkSizeBytes: 4194304
}
```

## Politica de tipologia

- La tipologia vive en metadata del archivo.
- Si es requerida, bloquea guardar el archivo hasta seleccionar una opcion valida.
- La sugerencia por nombre no bloquea.
- La sugerencia no sobreescribe seleccion manual.
- Como `trd` vive a nivel request backend, el componente guarda cada archivo con un request final independiente.

## Politica de fecha

- Se renderiza cuando config o props exigen fecha.
- Formato requerido: `yyyy-MM-dd`.
- Debe ser una fecha real.
- El ano no puede ser futuro.
- Si es obligatoria, bloquea guardar el archivo.
- Se envia como `camposIndexacion` con `nombreCampo: "fechaCarga"` hasta que exista campo canonico backend distinto.

## Eventos de salida

`onStored` recibe:

```ts
{
  fileUid: string;
  fileName: string;
  idAlmacen: number;
  idRegistroProduccionDocumental: number;
  nombreArchivoFinal: string;
  requestId: string;
  metadata: UploadDocumentalFileMetadata;
  interfaceRegistration?: UploadDocumentalInterfaceRegistration[];
  rawBackendResult?: unknown;
}
```

`onInterfaceRegistration` recibe eventos discriminados para que el modulo consumidor refresque su UI sin callbacks string legacy.

`onBatchComplete` recibe:

```ts
{
  total: number;
  stored: number;
  failed: number;
  skipped: number;
  cancelled: number;
  results: AlmacenarDocumentoStoredResult[];
}
```

## Errores y retry

- Error de config: deshabilita seleccion.
- Error de tipologias: no permite guardar si tipologia es requerida.
- Archivo invalido: se rechaza o encola con error segun `validationMode`.
- Error storage: marca solo el archivo afectado.
- Retry usa metadata actual y reinicia desde `init`.
- Cancelacion aborta request activo mediante `AbortSignal`.

## Integracion recomendada

El consumidor debe:

- proveer loaders reales;
- mapear `onStored` a refresco de datos;
- mapear `onInterfaceRegistration` a actualizacion visual si aplica;
- no depender de funciones globales legacy;
- no interpretar strings concatenados.
