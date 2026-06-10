## Why

El ticket `SCRUMCORE-222` cierra el ciclo de hardening después de:

- `SCRUMCORE-219`: normalización y tipado de `idRespuestaRadicado`
- `SCRUMCORE-220`: contexto transversal documental en `GestionRespuesta`
- `SCRUMCORE-221`: consumo centralizado de gabinete desde contexto en el flujo de documentos

Aunque estos cambios están operativos, no hay todavía una validación formal de cierre de estabilidad. Existe riesgo de regresiones silenciosas entre contexto, árbol documental y visor (estado de gabinete, re-renders, estados de carga/errores) sin una evidencia unificada de calidad.

Este ticket no introduce nuevas funcionalidades. Se limita a consolidar estabilidad, pruebas, trazabilidad y documentación enterprise.

## What Changes

- Validar y consolidar la estabilidad de extremo a extremo de `GestionRespuesta` tras los refactors previos.
- Confirmar que el estado transversal permanece estable:
  - `idTareaWf`
  - `radicado`
  - `idRespuestaRadicado`
  - `nombreGabinete`
  - `gabineteLoading`
  - `gabineteError`
  - `files`
  - `setFiles`
- Ejecutar regresión funcional sin cambiar endpoints, contratos backend ni UI de producto.
- Reforzar la trazabilidad entre:
  - normalización de estructura por tarea,
  - contexto transversal,
  - consumo de documentos y acciones del árbol (`SCRUM-205`),
  - flujo de visor/adjuntos.
- Generar documentación enterprise completa con evidencias de pruebas ejecutadas, pendientes y riesgos residuals.

## Scope

- No cambiar endpoints.
- No modificar contratos backend.
- No introducir lógica de negocio nueva.
- No alterar UI pública.

## Capabilities

### New Capability

- `consolidacion-dureza-regresion-gestionrespuesta`: validación de estabilidad transversal y evidencias de regresión.

### Modified Capabilities

- `implementacion-contexto-trasversal-unificado-gestion-respuesta`: endurecimiento de estabilidad del proveedor de contexto, sin ampliar responsabilidades.
- `actualiza-lista-documentos-radicado-atualiza-parametro-gabinete-desde-contexto`: confirmación de dependencia única del contexto para estado de gabinete.
- `documentos-workbench-tab`: verificación de estabilidad de layout y estado operativo en recargas.

## Impact

- Módulos objetivo:
  - `src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx`
  - `src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts`
  - `src/modules/gestionCorrespondencia/hooks/useListaDocumentosRadicadosTreeTable.ts`
  - `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
  - `src/modules/gestionCorrespondencia/components/documentosWorkbench/*`
  - `src/modules/gestionCorrespondencia/adapters/*`
  - `src/modules/gestionCorrespondencia/types/gestionRespuestaEstructura.types.ts`
  - `src/modules/gestionCorrespondencia/tests/*`
- Documentación objetivo:
  - `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/consolidaciondocumentacionregresion`

## Acceptance Criteria

- Sin regresiones visibles de `GestionRespuesta` (estructura, documentos, visor, adjuntos).
- `GestionRespuestaDocumentosContext` conserva contrato con consumidores y sin estado UI no transversal.
- `useListaDocumentosRadicadosTreeTable` no realiza request local de gabinete.
- Errores de gabinete no rompen render del árbol ni del visor.
- Cobertura de pruebas por tipo documentada con resultados ejecutados/pendientes.
- Pruebas de regresión sin warnings nuevos y sin errores de consola reproducibles.
