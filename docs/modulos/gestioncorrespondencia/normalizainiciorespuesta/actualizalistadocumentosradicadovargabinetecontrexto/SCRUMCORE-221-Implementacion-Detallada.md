# SCRUMCORE-221 - Implementacion Detallada

## Archivos modificados

- `src/modules/gestionCorrespondencia/hooks/useListaDocumentosRadicadosTreeTable.ts`
- `src/modules/gestionCorrespondencia/tests/useListaDocumentosRadicadosTreeTable.test.tsx`

## Cambio aplicado en hook

En `useListaDocumentosRadicadosTreeTable` se eliminó la resolución local de gabinete:

- Se quitó `getSolicitaGabinetePorTareaWorkflow` del hook.
- Se consumió `useGestionRespuestaDocumentos`.
- Se tomó estado contextual:
  - `nombreGabinete`
  - `gabineteLoading`
  - `gabineteError`
  - `available` (legacy del hook de contexto para trazabilidad interna)

### Antes

- El hook construía flujo de documentos consultando localmente el gabinete.
- Se podía generar consulta duplicada del mismo gabinete cuando el documento era usado desde `DocumentosWorkbench`.

### Después

- El hook depende de un único origen: contexto transversal de `GestionRespuesta`.
- `load`, `loadChildren` y `onSelectRow` validan:
  - si `gabineteLoading` -> bloquean con mensaje funcional.
  - si `gabineteError` -> retornan error funcional.
  - si no existe `nombreGabinete` -> error controlado.
- El contrato público del hook (`columns`, `load`, `loadChildren`, `onSelectRow`) permanece intacto.
- Payload de acción se conserva: `ActionId = "ver_documento"` con `NombreGabinete`, `IdDocumento` / `DocumentId`.

## Reglas de mapeo / fallback

- `buildInitialQuery` ahora recibe `nombreGabinete` desde contexto y lo inyecta solo si existe.
- Si no hay gabinete disponible:
  - `load` retorna `{ ok: false, message: "NombreGabinete requerido" }`.
  - `loadChildren` retorna `{ ok: false, message: "NombreGabinete requerido" }`.
  - `onSelectRow` lanza error funcional con el mismo criterio.

## Dependencias por capa

- `hooks`: consume `useGestionRespuestaDocumentos`.
- `services`: sin invocaciones nuevas de gabinete desde este hook.
- `context`: fuente única de verdad para estado de gabinete.
- `components`: `DocumentosWorkbench` conserva wire sin cambios funcionales.

## Estrategia de idempotencia

No se introduce estado local adicional para gabinete en el hook.

La idempotencia del gabinete se resuelve en el contexto:

- `loadGabinete` en provider controla recarga sólo por cambio de `idTareaWf`.
- Se evita refetch innecesario si no cambió contexto de tarea (SCRUMCORE-220).

## Cancelación y seguridad de request

Aunque este hook no ejecuta llamadas de gabinete, sigue protegido contra fallas por estados:

- no ejecuta query/acción si `gabineteLoading` está activo;
- no usa datos de gabinete parcial incompletos;
- evita ejecutar acciones dependientes sin validación de contexto completo.

## Compatibilidad

- No se alteró el contrato de AppTreeTable usado por el resto del módulo.
- No se modificó endpoint de listados/acciones ni servicio de adjuntos.
- No se introdujeron cambios de UI.
- No se tocó el comportamiento de carga de documentos salvo en dependencia de estado contextual existente.
