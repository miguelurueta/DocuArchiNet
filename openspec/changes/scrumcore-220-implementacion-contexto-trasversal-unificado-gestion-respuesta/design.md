## Context

SCRUMCORE-220 refina `GestionRespuestaDocumentosContext` para convertirlo en el contexto transversal documental de `GestionRespuesta`.

Estado actual identificado en codigo:

- `GestionRespuestaDocumentosContext.tsx` solo expone `files` y `setFiles`.
- `useGestionRespuestaDocumentos.ts` mantiene compatibilidad fuera de provider con `available: false`.
- `GestionRespuesta.tsx` crea el provider sin props y deriva solo `idTareaWf`.
- `solicitaGabineteRadicadoWorkflow.service.ts` ya centraliza el endpoint de gabinete por tarea.

El cambio debe centralizar datos compartidos por visor, adjuntos y documentos sin absorber logica de negocio ni estados locales de formularios.

## Goals / Non-Goals

**Goals**

- Extender el contexto documental para exponer `idTareaWf`, `radicado`, `idRespuestaRadicado`, `nombreGabinete`, `gabineteLoading`, `gabineteError`, `reloadGabinete`, `files` y `setFiles`.
- Resolver `nombreGabinete` una sola vez por `idTareaWf`, con `reloadGabinete` explicito para refresco manual.
- Evitar requests duplicados, race conditions y memory leaks mediante cancelacion y control de secuencia.
- Mantener compatibilidad con consumidores actuales de `files`, `setFiles` y `available`.
- Mantener la resolucion backend fuera de componentes UI y sin axios directo en componentes.
- Documentar arquitectura, implementacion, integracion backend, pruebas y metadata en la ruta exigida por el ticket.

**Non-Goals**

- No modificar UI visual.
- No cambiar endpoints backend ni contratos backend.
- No introducir logica de negocio en el provider.
- No convertir el contexto en estado global del modulo.
- No mover estados locales de formularios al contexto.
- No alterar funcionalmente visor PDF, adjuntos ni `DocumentosWorkbench`.

## Decisions

1. **Contexto documental acotado**

   `GestionRespuestaDocumentosContext` sera un contexto transversal documental. Su contrato queda limitado a identificadores compartidos, estado de gabinete y estado de adjuntos (`files/setFiles`).

2. **Source of truth en `GestionRespuesta`**

   `GestionRespuesta` seguira siendo la entrada de pagina y proveera al provider `idTareaWf`, `radicado` e `idRespuestaRadicado` cuando esten disponibles en el flujo de estructura por tarea. El contexto no parsea rutas ni resuelve reglas de negocio.

3. **Service como unica capa HTTP**

   La request de gabinete seguira en `solicitaGabineteRadicadoWorkflow.service.ts`. Si se requiere cancelacion, el service aceptara una opcion tipada de `AbortSignal` sin cambiar el endpoint ni exponer axios a componentes.

4. **Provider como orquestador liviano de ciclo de vida**

   El provider puede coordinar `loading/error/ready` del gabinete porque ese estado es transversal documental. No debe contener decisiones de negocio, transformaciones de dominio ajenas ni estado UI no compartido.

5. **Normalizacion de gabinete en contexto**

   El contexto expone `nombreGabinete?: string`. Si el backend no devuelve gabinete valido, el valor queda `undefined` y el render no se rompe.

6. **Idempotencia por `idTareaWf`**

   Se mantiene una referencia del ultimo `idTareaWf` resuelto. Si el id no cambia, el provider no vuelve a cargar automaticamente. `reloadGabinete` fuerza una nueva carga para el id actual.

7. **Cancelacion y proteccion contra stale state**

   Al cambiar rapido `idTareaWf` o desmontar el provider, se aborta la request anterior. Ademas, se usa un guard de secuencia/request id para impedir que una respuesta antigua sobrescriba el estado actual.

8. **API backward-compatible**

   `useGestionRespuestaDocumentos` conserva `files`, `setFiles` y `available`. Fuera del provider retorna valores seguros, `gabineteLoading: false`, `gabineteError: undefined`, `nombreGabinete: undefined` y `reloadGabinete` resuelto sin efectos.

9. **Memoizacion estable**

   `reloadGabinete` se implementa con identidad estable frente a re-render. El value del context se memoiza para evitar renders masivos por referencias nuevas innecesarias.

10. **Documentacion como entregable**

    La documentacion tecnica se generara en `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/contextounificadovariables/` con los cinco archivos obligatorios de SCRUMCORE-220.

## Architecture

Capas afectadas:

- `pages`: recibe/deriva datos fuente y los entrega al provider.
- `context`: aloja estado documental transversal y ciclo de vida liviano de gabinete.
- `hooks`: expone acceso seguro al contexto para consumidores.
- `services`: ejecuta HTTP hacia gabinete por tarea con soporte de cancelacion.
- `types`: mantiene contratos tipados del contexto y del response de gabinete.
- `tests`: valida provider, hook, service integration y no regresion de consumers.

Flujo esperado:

1. `GestionRespuesta` obtiene `idTareaWf` y, desde el flujo de estructura por tarea, `radicado` e `idRespuestaRadicado` cuando existan.
2. `GestionRespuestaDocumentosProvider` recibe esos valores como props.
3. Si `idTareaWf` es valido y no fue cargado, el provider solicita gabinete al service.
4. El service llama `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`.
5. El provider normaliza `NombreGabinete` hacia `nombreGabinete`.
6. Consumers acceden solo mediante `useGestionRespuestaDocumentos`.

## Risks / Trade-offs

- **Riesgo: god context.** Mitigacion: contrato limitado a datos documentales compartidos; prohibido agregar formularios, flags visuales locales o logica de negocio.
- **Riesgo: doble fetch.** Mitigacion: cache por `idTareaWf` y guard de request en curso.
- **Riesgo: stale gabinete state.** Mitigacion: `AbortController` y request sequence guard.
- **Riesgo: ruptura de adjuntos.** Mitigacion: conservar `files/setFiles` y pruebas de compatibilidad.
- **Riesgo: re-render masivo.** Mitigacion: `useMemo`, `useCallback` y dependencias estables.
- **Riesgo: error backend bloquee UX.** Mitigacion: error queda en `gabineteError`, sin throw durante render.

## Migration Plan

1. Extender tipos del contexto y provider props sin remover `files/setFiles`.
2. Actualizar service para aceptar cancelacion tipada sin cambiar endpoint.
3. Implementar carga idempotente de gabinete en provider.
4. Actualizar hook manteniendo fallback backward-compatible.
5. Cablear `GestionRespuesta` para proveer `idTareaWf`, `radicado` e `idRespuestaRadicado`.
6. Revisar consumers para que lean datos compartidos desde el hook/context y no llamen service directamente.
7. Agregar pruebas unitarias/integracion de provider, hook, idempotencia, cancelacion y regresion de `files/setFiles`.
8. Generar documentacion tecnica obligatoria y metadata.
9. Ejecutar validaciones OpenSpec, TypeScript/test suite afectada y registrar evidencia.

## Open Questions

- Confirmar durante implementacion desde que punto exacto del flujo de estructura por tarea se entrega `radicado` e `idRespuestaRadicado` a `GestionRespuesta`, manteniendo `GestionRespuesta` como source of truth y evitando resolver casing/backend en consumidores.
