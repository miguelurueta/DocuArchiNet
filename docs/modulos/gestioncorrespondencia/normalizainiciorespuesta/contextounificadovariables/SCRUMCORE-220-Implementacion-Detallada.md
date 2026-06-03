# SCRUMCORE-220 - Implementacion Detallada

## Archivos modificados

- `GestionCorrespondenciaRoute.tsx`
- `GestionRespuesta.tsx`
- `GestionRespuestaDocumentosContext.tsx`
- `useGestionRespuestaDocumentos.ts`
- `useGestionRespuestaDocumentosTable.ts`
- `solicitaGabineteRadicadoWorkflow.service.ts`
- `useGestionRespuestaDocumentos.test.tsx`
- `useGestionRespuestaDocumentosTable.test.tsx`

## Context actualizado

`GestionRespuestaDocumentosState` expone:

- `idTareaWf?: number`
- `radicado?: string`
- `idRespuestaRadicado?: string | number`
- `nombreGabinete?: string`
- `gabineteLoading: boolean`
- `gabineteError?: string`
- `reloadGabinete: () => Promise<void>`
- `files: AppUploadFile[]`
- `setFiles: (files: AppUploadFile[]) => void`

El provider recibe `idTareaWf`, `radicado` e `idRespuestaRadicado` desde `GestionRespuesta`.

## Hooks

`useGestionRespuestaDocumentos`:

- Mantiene `available`.
- Conserva fallback fuera de provider.
- Retorna no-op seguro para `setFiles`.
- Retorna `reloadGabinete` seguro como promesa resuelta fuera del provider.

`useGestionRespuestaDocumentosTable`:

- Deja de llamar `getSolicitaGabinetePorTareaWorkflow`.
- Lee `nombreGabinete`, `radicado`, `gabineteLoading` y `gabineteError` desde `useGestionRespuestaDocumentos`.
- Mantiene su API publica: `load`, `loadChildren`, acciones, counters y `getWorkbenchContext`.

## Services

`getSolicitaGabinetePorTareaWorkflow` y `getSolicitaGabinetePorRadicadoWorkflow` aceptan:

```ts
type GabineteWorkflowRequestOptions = {
  signal?: AbortSignal;
};
```

Esto permite cancelacion sin cambiar endpoint ni response backend.

## Wiring GestionRespuesta

`GestionCorrespondenciaRoute` obtiene `Radicado` e `idRespuestaRadicado` desde `estrucTuraRespuesta`, resultado normalizado de `useEstructuraRespuestaIdTarea`.

`GestionRespuesta` recibe:

- `idTareaWf`
- `radicado`
- `idRespuestaRadicado`

y los entrega a `GestionRespuestaDocumentosProvider`.

## Estrategia de idempotencia

El provider mantiene:

- `loadedTaskRef`: ultimo `idTareaWf` cargado correctamente.
- `idTareaWfRef`: id actual disponible para `reloadGabinete`.
- `requestSeqRef`: secuencia activa para ignorar responses stale.
- `abortRef`: controller de request activa.

Reglas:

- Si `idTareaWf` no es valido, no se consulta backend.
- Si `idTareaWf` no cambia y no es reload, no se refetchea.
- `reloadGabinete` fuerza nueva carga del id actual.

## Cancelacion de requests

Cada nueva carga aborta la anterior mediante `AbortController`.

Si una response llega tarde:

- se compara con `requestSeqRef`;
- si no coincide, se ignora;
- no sobrescribe `nombreGabinete`, `gabineteError` ni `gabineteLoading`.

## Fallback strategy

- Sin `idTareaWf`: `nombreGabinete: undefined`, `gabineteLoading: false`, `gabineteError: undefined`.
- Sin `NombreGabinete`: `nombreGabinete: undefined`.
- Error backend: `gabineteError` con mensaje seguro.
- `EstadoExistenciaRadicado = NO`: `gabineteError` funcional sin exponer ese estado en el contrato publico.

## Compatibilidad

- `files/setFiles` no cambiaron de semantica.
- `GestionRespuestaMainTabContent` sigue usando `files/setFiles`.
- `DocumentosWorkbench` conserva `idTareaWf` y su hook mantiene API.
- No se agregaron estados UI locales al contexto.
- No se modifico el layout ni estilos.
