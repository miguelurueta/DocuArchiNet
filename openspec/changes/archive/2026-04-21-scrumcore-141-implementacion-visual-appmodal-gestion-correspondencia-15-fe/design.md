# Design

## Context

`SCRUMCORE-141` cubre pruebas unitarias/UI del flujo de reasignacion en Gestion Correspondencia:
- componente `ReasignarRespuestaModal`
- integracion de apertura/cierre desde `Reasignar Tramite`

Referencias:
- `docs/Architecture/GestionCorrrespondecia/15-FE-Pruebas-AppModal-Reasignar-Respuesta.md`
- `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/`

## Scope

Incluye:
- pruebas del modal (render, callbacks, tags, nota, acciones)
- prueba de integracion UI desde accion de fila hasta cierre por cancelar
- evidencia de ejecucion de suite focalizada

No incluye:
- backend, servicios ni persistencia
- cambios de comportamiento del modal o la tabla fuera de testing
- modificaciones de AppTable base

## Design Decisions

### 1) Pruebas por comportamiento observable

Las pruebas validan elementos visibles y callbacks, evitando acoplarse a detalles internos de implementacion.

### 2) Integracion sin tocar la tabla base

La integracion se valida desde el punto donde llega `onActionTriggered` en la pagina de Gestion Correspondencia, con mocks existentes del AppTable.

### 3) Cobertura minima obligatoria alineada al ticket

Se cubren explicitamente:
- abrir/cerrar modal
- render y eliminacion de tags
- render de nota
- botones funcionales
- apertura desde `Reasignar Tramite` y cierre por `Cancelar`

## Technical Approach

- Archivo de pruebas de modal:
  - `src/modules/gestionCorrespondencia/components/modalReasignarRespuesta/ReasignarRespuestaModal.test.tsx`
- Archivo de pruebas de integracion de pagina:
  - `src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
- Ejecucion focalizada:
  - Vitest por archivo para confirmar flujo FE15 en verde

