# SCRUMCORE-271 - Implementacion detallada

## Archivos creados

```txt
src/modules/almacenamientoDocumental/components/AppUploadDocumental/
├─ AppUploadDocumental.tsx
├─ AppUploadDocumental.types.ts
├─ AppUploadDocumental.module.css
├─ AppUploadDocumental.test.tsx
├─ README.md
├─ index.ts
└─ hooks/
   ├─ useAppUploadDocumentalActions.ts
   ├─ useAppUploadDocumentalActions.test.ts
   ├─ useAppUploadDocumentalState.ts
   └─ useAppUploadDocumentalState.test.ts

src/modules/almacenamientoDocumental/services/
├─ tipoDocumental.service.ts
├─ uploadConfig.service.ts
├─ uploadDocumentalInterfaceRegistration.mapper.ts
└─ uploadDocumentalInterfaceRegistration.mapper.test.ts

src/modules/almacenamientoDocumental/utils/
├─ tipoDocumentalSuggestion.utils.ts
└─ tipoDocumentalSuggestion.utils.test.ts
```

## Contratos

`AppUploadDocumental.types.ts` define:

- `UploadDocumentalProcessKey`
- `UploadDocumentalContext`
- `UploadDocumentalConfig`
- `TipoDocumentalOption`
- `UploadDocumentalFileMetadata`
- `UploadDocumentalInterfaceRegistration`
- `AlmacenarDocumentoStoredResult`
- `UploadDocumentalBatchSummary`
- `AppUploadDocumentalProps`

Los shapes no modelados usan `unknown`. No se introdujo `any` productivo.

## Estado

`useAppUploadDocumentalState` administra:

- carga de config;
- carga de tipologias;
- seleccion deshabilitada fail-safe;
- cola por `uid`;
- metadata independiente;
- validacion `reject` / `queue-with-error`;
- tipologia requerida;
- fecha requerida/valida;
- seleccion activa;
- eliminar/limpiar;
- resumen de estados;
- `operationId` anti-stale.

## Acciones

`useAppUploadDocumentalActions` administra:

- guardar individual;
- preparar lote;
- `processBatchItem` para `AppProgressBatch`;
- construccion de request final por archivo;
- mapping de progreso storage a estado visual;
- callbacks `onStored`, `onInterfaceRegistration`, `onBatchComplete`, `onError`;
- cancelacion con `AbortController`;
- limpieza ante cambio de `operationId`;
- retry desde una nueva corrida.

## Request final por archivo

Cada archivo genera:

```txt
UploadAndStoreOneDocumentInput.request
  nombreGabinete
  nombreDocumento
  requestId
  camposIndexacion
  trd
  expediente
  workflow
  numeroPaginasDeclaradas
  documento
```

La extension se obtiene con `normalizeFileExtension` de SCRUMCORE-272.

## Mapper de interfaz

`buildUploadDocumentalInterfaceRegistration` recibe:

- resultado normalizado;
- `rawBackendResult`;
- contexto;
- metadata;
- proceso;
- modo documental.

Puede emitir:

- `production-document-row`
- `related-document-row`
- `workflow-document-row`
- `migration-preview`
- `page-counter`
- `traffic-light`
- `dropdown-option`
- `document-version-row`
- `table-import-result`
- `raw`

No llama funciones globales, no muta DOM y no concatena datos con `|`.

## Utils

`tipoDocumentalSuggestion.utils.ts` implementa:

- normalizacion Unicode;
- mayusculas;
- remocion de caracteres no alfanumericos;
- tokenizacion;
- filtro por longitud minima;
- scoring;
- umbral configurable;
- preservacion de seleccion manual;
- validacion de fecha `yyyy-MM-dd`, real y no futura.

## Seguridad y legacy

Confirmado en codigo productivo del modulo:

- sin jQuery;
- sin WebForms;
- sin Bootstrap manual;
- sin `.ashx`;
- sin `XMLHttpRequest`;
- sin `FormData` legacy;
- sin `fetch` directo;
- sin `clienteApi` en componente/hooks;
- sin logs de bytes, tokens o payload sensible.
