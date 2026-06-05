# SCRUMCORE-222 - Implementacion Detallada

## Resumen de implementación

SCRUMCORE-222 no introduce comportamiento funcional nuevo. Cierra consolidación y hardening de la cadena transversal:

- Contexto documental (`context`)
- Hooks de consulta y acciones documentales (`hooks`)
- Integración de árbol + visor + adjuntos (`components`)
- Verificación de regresión técnica y documentación enterprise.

## Cambios realizados por capa

### Context

#### `src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx`

- Se mantienen expuestos: `idTareaWf`, `radicado`, `idRespuestaRadicado`, `nombreGabinete`, `gabineteLoading`, `gabineteError`, `reloadGabinete`, `files`, `setFiles`.
- Carga de gabinete:
  - Se centraliza con `getSolicitaGabinetePorTareaWorkflow`.
  - Se previenen peticiones obsoletas con `AbortController`.
  - Se controla stale response con `requestSeqRef`.
  - Se evita fetch redundante cuando `idTareaWf` no cambia (`loadedTaskRef`), salvo `reloadGabinete`.
- Normalización y fallback:
  - `nombreGabinete` vacío/`undefined` se normaliza a `undefined`.
  - Si `EstadoExistenciaRadicado` = `"NO"` se publica error funcional y no se fuerza dato corrupto.
  - Errores HTTP/catastrofismo no rompen render del árbol.

### Hooks

#### `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts`

- `useGestionRespuestaDocumentos` continua exponiendo estado de contexto con compatibilidad.
- Mantiene semántica `available` para componentes legacy sin consumir props adicionales.
- No agrega fetch ni lógica de negocio.

#### `src/modules/gestionCorrespondencia/hooks/useListaDocumentosRadicadosTreeTable.ts`

- Se retira resolución local de gabinete.
- Consume solo:
  - `nombreGabinete`
  - `gabineteLoading`
  - `gabineteError`
  - `reloadGabinete`
- Se conserva contrato público:
  - `load`
  - `loadChildren`
  - `loading`
  - `error`
  - `rows`
  - `actions`
- `ver_documento` valida estado de gabinete:
  - loading ⇒ acción bloqueada con mensaje controlado (no rompe UI).
  - error ⇒ error controlado con fallback.
  - gabinete válido ⇒ acción ejecuta con el payload esperado.

### Servicios

#### `src/modules/gestionCorrespondencia/services/solicitaGabinetePorTareaWorkflow.service.ts`

- Se conserva como punto único de consulta de gabinete.
- Sin cambios en endpoint.

### Componentes

#### `src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx`
- Continúa consumiendo `useListaDocumentosRadicadosTreeTable`.
- Mantiene wire de árbol/visor y comportamiento previo.

#### `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx` (alineación indirecta)
- Mantiene fuente de datos normalizados (de flujos previos) hacia `GestionRespuestaDocumentosProvider`.
- No cambia UI.

## Integraciones validadas

### Integración `context` → `hook documental`

- Verificación de que el hook documental no llama `getSolicitaGabinetePorTareaWorkflow` directamente.
- Verificación de que la acción depende de `nombreGabinete` del contexto.

### Integración `context` → `adjuntos`

- Se preservan `files` y `setFiles` como estado transversal sin modificación semántica.

### Integración `tree table` → `visor`

- La carga de documentos (`load`, `loadChildren`) conserva payload y contratos de query/action.
- Se mantiene ruta de acción para abrir documento con `AppVisorEmbedPdf`.

## Estrategia de estabilidad aplicada

- Idempotencia por tarea:
  - No recargar gabinete si `idTareaWf` no cambia.
- Cancelación:
  - Se aborta request anterior al iniciar nuevo request.
- Compatibilidad:
  - No rompió contratos de `load/loadChildren`.
  - No se movieron firmas públicas del API interno de documentos.
- Modo degradado:
  - Estado de gabinete con error no produce caída de árbol/visor.

## Matriz de cambios por archivo

- `GestionRespuestaDocumentosContext.tsx`: hardening de carga/idempotencia/fallback de gabinete.
- `useGestionRespuestaDocumentos.ts`: acceso estable al contexto con compatibilidad.
- `useListaDocumentosRadicadosTreeTable.ts`: consumo único de estado transversal.
- `useListaDocumentosRadicadosTreeTable.test.tsx`: pruebas de fallback/loading/error/acción.
- Tests relacionados de `useGestionRespuestaDocumentos`, `useGestionRespuestaDocumentosTable`, `DocumentosWorkbench`: actualizados para cubrir el comportamiento final.

## Estado de riesgos

- No se detectaron regresiones funcionales nuevas en código tocado.
- Riesgo residual: validación E2E navegable completa aún por disponibilidad de entorno.
