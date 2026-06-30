## Context

SCRUMCORE-271 crea `AppUploadDocumental`, una UI documental enterprise sobre:

- `AppUpload` para seleccion y drag/drop;
- `AppUploadBatchView` para lista, preview, toolbar, metadata y layout;
- `AppProgressBatch` para procesamiento secuencial;
- `almacenamientoDocumentalUpload.service` para upload tecnico por chunks y almacenamiento final.

SCRUMCORE-272 ya entrego el cliente tecnico de almacenamiento en `src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts`, por lo que este ticket no debe reimplementar esa capa ni llamar `clienteApi` desde React.

## Goals / Non-Goals

**Goals**

- Implementar la vista documental final y reusable.
- Cargar configuracion y tipologias por loaders obligatorios.
- Validar archivos con reglas backend/config.
- Mantener metadata por archivo: tipologia, fecha, error, warning, sugerencia.
- Procesar multiples archivos secuencialmente, con un POST final por archivo.
- Emitir resultados tipados para consumidores sin mutar DOM.
- Cubrir mapper de registro visual y sugerencia de tipologia con tests aislados.
- Documentar uso enterprise del componente.

**Non-Goals**

- No copiar HTML legacy.
- No reimplementar `AppUpload`, `AppUploadBatchView` ni `AppProgressBatch`.
- No usar `.ashx`, `XMLHttpRequest`, `FormData` legacy, jQuery, Bootstrap manual o WebForms.
- No inventar endpoints de configuracion/tipologias.
- No cambiar endpoints de almacenamiento ni backend.
- No introducir `any`.
- No resolver integracion definitiva de cada modulo consumidor; se entrega contrato tipado para que cada modulo actualice su UI.

## Existing Building Blocks

### AppUploadBatchView

`AppUploadBatchViewProps<TMetadata>` ya soporta:

- `files`;
- `accept`, `maxSize`, `multiple`, `drag`;
- `onFilesSelected`, `onRemoveFile`, `onSaveFile`, `onSaveAll`, `onClearAll`;
- `renderMetadata`;
- `renderPreview`;
- `renderFooterExtra`;
- estados como `queued`, `ready`, `uploading`, `completing`, `storing`, `done`, `warning`, `error`, `cancelled`.

### AppProgressBatch

`AppProgressBatch` ya soporta:

- procesamiento secuencial;
- `AbortSignal`;
- `setCurrentLabel`, `setItemProgress`, `setPhase`;
- resultados `success`, `warning`, `skipped`, `controlled-error`, `fatal-error`;
- `onComplete`, `onCancel`, `onError`.

### Storage Client

`uploadAndStoreOneDocument` ya entrega:

- progreso `initializing`, `uploading`, `completing`, `storing`;
- cancelacion via `AbortSignal`;
- `temporal`;
- `response`;
- `rawBackendResult`.

## Decisions

1. **Ubicacion modular**
   - El componente vive en `src/modules/almacenamientoDocumental/components/AppUploadDocumental/`.
   - Los tipos especificos de UI viven junto al componente.
   - Los tipos compartidos de almacenamiento se consumen desde `src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts`.

2. **Loaders obligatorios**
   - `loadConfig` y `loadTiposDocumentales` son props obligatorias.
   - `uploadConfig.service.ts` y `tipoDocumental.service.ts` pueden existir como adaptadores, pero no deben inventar endpoints.
   - Si no hay endpoint canonico, los loaders del consumidor son la unica fuente de verdad.

3. **Estado por archivo**
   - El estado se maneja por `uid`.
   - `File` permanece en memoria runtime del componente/hook.
   - Metadata, errores, warning, sugerencia y estado visual se actualizan por archivo sin contaminar otros items.

4. **Tipologia por archivo**
   - La API final modela `trd` a nivel request.
   - Para soportar tipologias diferentes, el componente procesa de forma secuencial y construye un request final por archivo.
   - No se manda un request final con multiples documentos de tipologias distintas.

5. **Fecha documental**
   - `fechaCarga` se renderiza como metadata por archivo cuando `requiereFechaCarga` o config lo exige.
   - Se valida `yyyy-MM-dd`, fecha real, no futura y obligatoriedad.
   - Se mapea a `camposIndexacion` con un nombre documentado si no existe campo backend canonico.

6. **Sugerencia de tipologia**
   - `tipoDocumentalSuggestion.utils.ts` es pura y testeable.
   - La sugerencia no sobreescribe seleccion manual.
   - El score queda expuesto como `suggestionConfidence`.

7. **Registro de interfaz**
   - `AppUploadDocumental` no interpreta `funcion_name`, no concatena con `|` y no muta DOM.
   - `uploadDocumentalInterfaceRegistration.mapper.ts` recibe resultado normalizado, `rawBackendResult`, contexto y metadata.
   - Devuelve `UploadDocumentalInterfaceRegistration[]` o fallback `raw` cuando hay dato util no modelable.

8. **Cancelacion y anti-stale**
   - Cada corrida usa `AbortController`.
   - Cambios de `proceso`, `context.nombreGabinete`, `modoDocumento` o desmontaje abortan operaciones activas.
   - Un `operationId` evita aplicar resultados stale y evita emitir `onStored` para contexto obsoleto.

9. **Retry**
   - Retry reutiliza el mismo archivo y metadata actual.
   - Si hubo temporal previo cancelado/fallido, una nueva corrida debe iniciar desde `init`.
   - No se reusa `rutaTemporalId` fallida.

10. **UX**
    - UI operacional, densa y escaneable.
    - Sin hero, cards decorativas, gradientes, tabla DOM manual ni contenido explicativo dentro de la app.
    - Acciones por fila con iconos, `aria-label` y tooltips si el componente local lo permite.

## Architecture

```txt
AppUploadDocumental.tsx
  -> useAppUploadDocumentalState
  -> useAppUploadDocumentalActions
  -> AppUploadBatchView
       -> AppUpload
       -> renderMetadata(tipologia/fecha/error)
       -> renderPreview
  -> AppProgressBatch
  -> uploadAndStoreOneDocument
  -> buildUploadDocumentalInterfaceRegistration
```

## Data Flow

1. Montaje valida `context.nombreGabinete`.
2. Carga config con `loadConfig`.
3. Carga tipologias con `loadTiposDocumentales`.
4. Habilita seleccion solo si config y tipologias requeridas estan listas.
5. Al seleccionar archivos, normaliza `uid`, extension, tamano y metadata inicial.
6. Valida extension/tamano segun config.
7. Aplica `reject` o `queue-with-error`.
8. Sugiere tipologia si aplica.
9. Usuario ajusta tipologia/fecha.
10. Guardar individual o todos valida metadata final.
11. `AppProgressBatch` procesa archivos validos secuencialmente.
12. `uploadAndStoreOneDocument` ejecuta storage.
13. El resultado se normaliza a `AlmacenarDocumentoStoredResult`.
14. El mapper construye eventos `UploadDocumentalInterfaceRegistration[]`.
15. Se emiten `onStored`, `onInterfaceRegistration`, `onBatchComplete` y `onError` segun corresponda.

## Risks / Trade-offs

- **Endpoints de config/tipologias no confirmados**: se exige loader obligatorio para evitar inventar endpoints.
- **Backend externo no disponible desde workspace**: el mapper debe ser tolerante y preservar `rawBackendResult`.
- **Campos legacy heterogeneos**: se modelan eventos discriminados, no callbacks string.
- **Suite completa inestable por tests previos**: la entrega debe incluir suite focal de SCRUMCORE-271 y documentar cualquier fallo no relacionado.
- **Object URLs**: preview debe revocar URLs al remover, limpiar o desmontar.
- **Doble fuente de validacion**: frontend valida preventivamente; backend sigue siendo autoridad final.

## Migration Plan

1. Revisar referencias de arquitectura y componentes existentes.
2. Crear contratos TypeScript de `AppUploadDocumental`.
3. Crear utils de sugerencia, fecha y payload si faltan.
4. Crear mapper de registro de interfaz.
5. Implementar hooks de estado/acciones.
6. Implementar componente con `AppUploadBatchView`.
7. Integrar `AppProgressBatch` para guardar todos.
8. Integrar `uploadAndStoreOneDocument` para guardar uno o todos.
9. Agregar README enterprise del componente.
10. Agregar tests unitarios, integracion y, si el repo lo permite, pruebas navegador focales.

## Open Questions

- No hay endpoint canonico confirmado para configuracion de upload.
- No hay endpoint canonico confirmado para tipologias documentales.
- No hay evidencia local de DTO backend adicional para eventos de registro visual.
- Si un consumidor necesita comportamiento modal, debe componerse fuera del componente o con `open/onClose`, sin recrear Bootstrap legacy.
