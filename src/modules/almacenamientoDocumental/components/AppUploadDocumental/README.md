# AppUploadDocumental

Componente documental especializado para seleccion multiple, metadata por archivo, upload tecnico por chunks y registro final individual usando la API nueva de almacenamiento documental.

## Objetivo

`AppUploadDocumental` conserva la semantica util del flujo legacy de carga documental sin migrar DOM manual, jQuery, WebForms, Bootstrap, `.ashx`, `XMLHttpRequest` ni `FormData` legacy. La seleccion se delega en `AppUploadBatchView`/`AppUpload`, el proceso batch en `AppProgressBatch` y el almacenamiento en `almacenamientoDocumentalUpload.service`.

## Props principales

- `proceso`: clave del proceso consumidor.
- `context`: contexto funcional; `nombreGabinete` es obligatorio.
- `loadConfig`: loader obligatorio de reglas de upload.
- `loadTiposDocumentales`: loader obligatorio de tipologias.
- `tipologiaObligatoria`: fuerza tipologia por archivo.
- `requiereFechaCarga` y `fechaCargaObligatoria`: controlan fecha documental por archivo.
- `allowSingleFileStore`: habilita guardar individual.
- `validationMode`: `reject` o `queue-with-error`.
- `onStored`: recibe resultado almacenado por archivo.
- `onInterfaceRegistration`: recibe eventos tipados para refresco de interfaz.
- `onBatchComplete`: recibe resumen del lote.
- `onError`: recibe errores controlados como `unknown`.

## Ejemplo embebido

```tsx
<AppUploadDocumental
  proceso="radicacion"
  context={{ nombreGabinete: "Gestion", idExpediente: 15 }}
  loadConfig={loadUploadConfig}
  loadTiposDocumentales={loadTiposDocumentales}
  onStored={(result) => refreshRow(result)}
/>
```

## Ejemplo modal/controlado

```tsx
<AppUploadDocumental
  embedded={false}
  open={open}
  proceso="workflow"
  context={{ nombreGabinete: "Workflow", idTareaWorkflow: 7 }}
  loadConfig={loadUploadConfig}
  loadTiposDocumentales={loadTiposDocumentales}
  onClose={() => setOpen(false)}
  onBatchComplete={(summary) => setSummary(summary)}
/>
```

## Loaders requeridos

No hay endpoint canonico de configuracion/tipologias confirmado en el repo. Por eso `loadConfig` y `loadTiposDocumentales` son obligatorios y son la fuente de verdad para:

- extensiones permitidas;
- `accept`;
- tamano maximo;
- modo multiple;
- tipologia requerida;
- fecha requerida;
- tamano preferido de chunk;
- catalogo de tipos documentales.

Los adaptadores `uploadConfig.service.ts` y `tipoDocumental.service.ts` solo normalizan contratos; no inventan endpoints ni llaman `clienteApi`.

## Flujo de upload

1. Validar `context.nombreGabinete`.
2. Cargar config y tipologias.
3. Habilitar seleccion.
4. Normalizar archivo, extension y metadata.
5. Validar extension/tamano con config.
6. Sugerir tipologia por nombre si aplica.
7. Validar tipologia/fecha antes de guardar.
8. Ejecutar `uploadAndStoreOneDocument`.
9. Storage client ejecuta `init -> chunks -> complete -> almacenar`.
10. Construir eventos de interfaz con `buildUploadDocumentalInterfaceRegistration`.
11. Emitir `onStored`, `onInterfaceRegistration` y `onBatchComplete`.

## Matriz FE/BE

| Frontend | Backend storage |
| --- | --- |
| `context.nombreGabinete` | `AlmacenarDocumentoRequest.nombreGabinete` |
| `file.name` | `nombreDocumento`, `documentos[0].nombreOriginal` |
| `normalizeFileExtension(file.name)` | `documentos[0].extension` |
| `metadata.idTipoDocumento` | `trd.idTipoDocumento` |
| `metadata.nombreTipoDocumento` | `trd.nombreTipoDocumento` |
| `context.idExpediente` | `expediente.idExpediente` |
| `context.idTipoExpediente` | `expediente.idTipoExpediente` |
| `context.idTareaWorkflow` | `workflow.idTareaWorkflow` |
| `context.idRutaWorkflow` | `workflow.idRutaWorkflow` |
| `metadata.fechaCarga` | `camposIndexacion["fechaCarga"]` |
| `metadata.numeroPaginas` | `numeroPaginasDeclaradas`, `documentos[0].numeroPaginas` |
| `createStorageRequestId("documental")` | `requestId` |
| `uploadAndStoreOneDocument.temporal.rutaTemporalId` | construido por storage client |
| `uploadAndStoreOneDocument.temporal.archivoTemporalId` | construido por storage client |

Los DTOs backend externos bajo `D:\imagenesda\...` no estuvieron accesibles desde este workspace durante la implementacion. La evidencia local disponible es el contrato validado por SCRUMCORE-272 en `almacenamientoDocumental.types.ts` y `almacenamientoDocumentalUpload.service.ts`.

## Tipologia por archivo

La API final tiene `trd` a nivel request. Para soportar tipologias independientes, la UI procesa secuencialmente y genera un `POST /api/gestor-documental/almacenamiento` por archivo. No se agrupan documentos con tipologias diferentes en un mismo request final.

## Fecha documental

Cuando `requiereFechaCarga` es verdadero, cada fila muestra fecha. Si `fechaCargaObligatoria` es verdadero, el archivo no se puede guardar sin fecha valida `yyyy-MM-dd`, real y con ano no futuro. La fecha se envia como campo de indexacion `fechaCarga` hasta que exista campo backend canonico distinto.

## Retorno para registro de interfaz

El componente no llama funciones globales legacy ni concatena campos con `|`. El mapper retorna eventos discriminados como:

- `production-document-row`;
- `related-document-row`;
- `workflow-document-row`;
- `migration-preview`;
- `page-counter`;
- `traffic-light`;
- `dropdown-option`;
- `document-version-row`;
- `table-import-result`;
- `raw`.

`onStored` recibe el resultado normalizado y puede incluir `interfaceRegistration`. `onInterfaceRegistration` emite el mismo arreglo como canal especializado para el modulo consumidor.

## Errores, cancelacion y retry

- Si falla config, se deshabilita seleccion y guardado.
- Si falla tipologias y la tipologia es obligatoria, el archivo no puede guardarse.
- Archivos invalidos se rechazan o se encolan con error segun `validationMode`.
- Errores de `init`, chunk, complete o store marcan solo el archivo afectado.
- Cancelar aborta el `AbortSignal`; el storage client intenta limpieza temporal cuando aplica.
- Retry reinicia desde `init` y no reusa ids temporales previos.

## Limites conocidos

- No se implementan endpoints default de configuracion/tipologias porque no hay contrato canonico confirmado.
- La normalizacion de eventos visuales depende de campos presentes en `rawBackendResult`; si el backend no trae suficiente informacion, se emite evento contextual o `raw`.
- La verificacion navegador/manual debe ejecutarse en un entorno con fixtures reales de PDF, imagen y errores simulados.

## Evidencia de verificacion

- TypeScript focal: `npx.cmd tsc -p tsconfig.app.json --noEmit`.
- Tests focales: `npx.cmd vitest run src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.test.tsx src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.test.ts src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.test.ts src/modules/almacenamientoDocumental/services/uploadDocumentalInterfaceRegistration.mapper.test.ts src/modules/almacenamientoDocumental/utils/tipoDocumentalSuggestion.utils.test.ts --environment jsdom`.
- OpenSpec: `npx.cmd openspec validate scrumcore-271-crea-componente-appuploaddocumental --strict`.
- Busqueda de prohibidos: no hay uso productivo de `.ashx`, `XMLHttpRequest`, `FormData`, jQuery, `clienteApi` directo ni `any` nuevo en el modulo.
- Deuda explicita: no se ejecuto flujo navegador/manual de cinco archivos porque este repo no contiene una pantalla consumidora montada con loaders reales, fixtures PDF/imagen y simuladores de error/tamano para `AppUploadDocumental`.
