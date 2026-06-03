## Why

IMPLEMENTACION-CONTEXTO-TRASVERSAL-UNIFICADO-GESTION-RESPUESTA. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-220.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> PROMPT ARQUITECTÓNICO — Contexto transversal unificado de GestionRespuesta
> Rol esperado
> Arquitecto de software senior frontend  (React 19, TypeScript estricto, React Context, state orchestration, Clean Architecture, testing enterprise)
> Objetivo
> Refactorizar el contexto actual de GestionRespuesta para centralizar y propagar estado transversal compartido relacionado con:
> idTareaWf
> 
> radicado
> 
> idRespuestaRadicado
> 
> nombreGabinete
> 
> files/setFiles
> 
> Garantizando:
> compatibilidad con consumidores actuales
> 
> desacoplamiento del módulo
> 
> manejo seguro de carga de gabinete
> 
> estabilidad del estado compartido
> 
> ausencia de regresiones en adjuntos y visor
> 
> IMPORTANTE
> Este contexto NO debe convertirse en un “god context” del módulo.
> Debe limitarse exclusivamente a:
> datos transversales compartidos
> 
> estado documental
> 
> información requerida por visor/adjuntos/documentos
> 
> NO debe absorber:
> lógica de negocio
> 
> estados locales de formularios
> 
> estado UI no transversal
> 
> orchestration pesada del módulo
> 
> Dependencia
> GestionRespuesta
> 
> flujo de estructura por tarea
> 
> solicitaGabineteRadicadoWorkflow.service.ts
> 
> flujo actual de adjuntos
> 
> AppVisorEmbedPdf
> 
> DocumentosWorkbench
> 
> Contexto existente
> Actualmente:
> GestionRespuestaDocumentosContext expone:
> files
> 
> setFiles
> 
> Pero no centraliza:
> idTareaWf
> 
> radicado
> 
> idRespuestaRadicado
> 
> nombreGabinete
> 
> Esto provoca:
> fetches dispersos
> 
> posibles duplicaciones
> 
> dificultad de compartir contexto transversal
> 
> acoplamiento potencial entre componentes
> 
> Estado actual
> El nombre de gabinete aún no está centralizado ni cacheado a nivel contextual, y diferentes consumidores podrían terminar resolviendo datos similares de forma redundante.
> Ubicación esperada
> Context:src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
> Hook:src/modules/gestionCorrespondencia/hooks/useGestionRespuestaDocumentos.ts
> Page:src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
> Service:src/modules/gestionCorrespondencia/services/solicitaGabineteRadicadoWorkflow.service.ts
> Tests:src/modules/gestionCorrespondencia/tests/*
> Restricciones obligatorias
> NO usar anyNO romper flujo actual de adjuntosNO usar axios directo en componentesNO convertir el contexto en estado global del móduloNO introducir lógica de negocio en el providerNO duplicar fetches de gabineteNO generar re-fetch innecesario
> Regla arquitectónica obligatoria
> GestionRespuestaDocumentosContext debe funcionar como contexto transversal documental del módulo y NO como contenedor global de estado.
> Esto implica:
> El provider centraliza únicamente datos compartidos relevantes
> 
> El fetch de gabinete vive en service/hook
> 
> El contexto expone estado ya normalizado
> 
> Los consumidores NO llaman directamente al service
> 
> El contexto NO absorbe estados UI ajenos
> 
> Contrato esperado
> GestionRespuestaDocumentosContext debe exponer:
> idTareaWf?: number
> 
> radicado?: string
> 
> idRespuestaRadicado?: string | number
> 
> nombreGabinete?: string
> 
> gabineteLoading: boolean
> 
> gabineteError?: string
> 
> reloadGabinete: () => Promise<void>
> 
> files
> 
> setFiles
> 
> Contrato de source of truth
> GestionRespuesta:
> provee:
> idTareaWf
> 
> radicado
> 
> idRespuestaRadicado
> 
> Context:
> resuelve:
> nombreGabinete
> 
> Service:
> ejecuta request backend
> 
> Reglas de implementación obligatorias
> Extender contexto
> 
> Agregar:
> idTareaWf
> 
> radicado
> 
> idRespuestaRadicado
> 
> nombreGabinete
> 
> gabineteLoading
> 
> gabineteError
> 
> reloadGabinete
> 
> Provider
> 
> Debe recibir:
> idTareaWf
> 
> radicado
> 
> idRespuestaRadicado
> 
> Desde:
> GestionRespuesta
> 
> Resolución de gabinete
> 
> Debe cargar:GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete
> Reglas:
> cargar una sola vez por idTareaWf
> 
> no duplicar requests
> 
> soportar reload explícito
> 
> soportar abort/cancelación
> 
> Error handling
> 
> error NO rompe render
> 
> gabineteError visible para consumers
> 
> fallback seguro cuando no existe gabinete
> 
> Compatibilidad
> 
> mantener files/setFiles
> 
> mantener API del hook compatible cuando sea posible
> 
> si cambia API:
> migración controlada
> 
> backward-compatible
> 
> Reglas de idempotencia
> nombreGabinete debe resolverse de forma idempotente:
> no re-fetch si idTareaWf no cambia
> 
> cancelar request anterior si cambia rápido
> 
> evitar race conditions
> 
> Reglas de memoización
> reloadGabinete debe ser:
> estable
> 
> memoizado
> 
> seguro frente a re-render
> 
> Reglas de interacción
> consumers leen datos solo desde hook/context
> 
> components NO llaman service directamente
> 
> visor/documentos pueden compartir estado de gabinete
> 
> Reglas de migración segura
> no romper flujo de adjuntos
> 
> no romper visor PDF
> 
> no romper DocumentosWorkbench
> 
> no romper consumers actuales
> 
> mantener render estable
> 
> Accesibilidad y UX
> gabineteLoading debe permitir feedback visual
> 
> gabineteError no debe bloquear UX
> 
> no generar flicker visual
> 
> no provocar renders vacíos inesperados
> 
> Riesgos a evitar
> god context
> 
> doble fetch
> 
> race conditions
> 
> stale gabinete state
> 
> re-fetch innecesario
> 
> coupling entre consumers
> 
> pérdida de estado de adjuntos
> 
> memory leaks por requests no cancelados
> 
> Pruebas unitarias obligatorias
> provider expone estado correctamente
> 
> reloadGabinete funciona
> 
> gabineteLoading cambia correctamente
> 
> gabineteError se maneja correctamente
> 
> idempotencia por idTareaWf
> 
> cancelación segura de requests
> 
> Pruebas de integración UI obligatorias
> consumers reciben nombreGabinete correctamente
> 
> visor/documentos siguen funcionando
> 
> files/setFiles siguen operativos
> 
> error no rompe render
> 
> Pruebas de interacción en navegador obligatorias
> reloadGabinete actualiza contexto
> 
> cambio rápido de idTareaWf no rompe estado
> 
> no hay loading infinito
> 
> no hay re-render masivo
> 
> Pruebas E2E obligatorias
> GestionRespuesta comparte gabinete correctamente
> 
> visor/documentos usan contexto unificado
> 
> flujo adjuntos sigue estable
> 
> reload funciona correctamente
> 
> Pruebas QT / calidad
> sin errores build
> 
> sin warnings TS/lint
> 
> sin errores consola
> 
> sin regresiones visuales
> 
> sin memory leaks
> 
> Criterios de aceptación
> Contexto expone:
> idTareaWf
> 
> radicado
> 
> idRespuestaRadicado
> 
> nombreGabinete
> 
> nombreGabinete se resuelve una sola vez por idTareaWf
> 
> files/setFiles siguen funcionando
> 
> reloadGabinete funciona
> 
> no hay regresiones
> 
> tests pasan
> 
> Documentación obligatoria
> Ruta:
> docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/contextounificadovariables
> Archivos obligatorios:
> SCRUMCORE-[XX]-Arquitectura.md
> 
> Debe incluir:
> SCRUM-[ID] - Arquitectura
> 1. Resumen arquitectónico
> objetivo técnico
> 
> decisiones
> 
> restricciones
> 
> 2. Vista estática
> Capas:
> context
> 
> hooks
> 
> services
> 
> pages
> 
> types
> 
> 3. Diagramas de clases
> Mermaid classDiagram:
> Context
> 
> Provider
> 
> Hook
> 
> Service
> 
> Consumers
> 
> 4. Diagramas de secuencia
> Mermaid sequenceDiagram:
> mount provider
> 
> resolve gabinete
> 
> reloadGabinete
> 
> cambio idTareaWf
> 
> 5. Diagramas de estados
> stateDiagram-v2:
> idle
> 
> loading
> 
> ready
> 
> error
> 
> 6. ADRs resumidas
> contexto transversal
> 
> evitar god context
> 
> idempotencia gabinete
> 
> 7. Riesgos técnicos y mitigaciones
> 8. Trazabilidad a código
> SCRUMCORE-[XX]-Implementacion-Detallada.md
> 
> Debe incluir:
> context actualizado
> 
> hooks
> 
> services
> 
> wiring GestionRespuesta
> 
> estrategia idempotencia
> 
> cancelación requests
> 
> Capas:
> context
> 
> hooks
> 
> services
> 
> pages
> 
> SCRUM-[XX]-Integracion-BackEnd.md
> 
> Debe incluir:
> endpoint gabinete
> 
> request
> 
> response
> 
> errores
> 
> retry
> 
> fallback
> 
> integración FE-BE
> 
> SCRUM-[XX]-Pruebas.md
> 
> Debe incluir:
> unitarias
> 
> integración
> 
> browser interaction
> 
> E2E
> 
> regresión
> 
> matriz de cobertura
> 
> SCRUM-[ID]-Metadata.md
> 
> Debe incluir:
> ticket
> 
> autor
> 
> fecha
> 
> versión
> 
> control de cambios
> 
> referencias cruzadas
> 
> Entregables
> Código actualizado
> 
> Tests ajustados/agregados
> 
> Documentación técnica generada
> 
> Estrategia de cache/idempotencia documentada
> 
> Resultado de pruebas ejecutadas
> 
> Instrucción final
> Refactorizar GestionRespuestaDocumentosContext como contexto transversal documental desacoplado, centralizando únicamente estado compartido relevante, resolviendo nombreGabinete de forma segura e idempotente y preservando compatibilidad total con visor, adjuntos y consumidores actuales.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CONTEXTO, IMPLEMENTACION, UNIFICADO, VARIABLE

## Capabilities

### New Capabilities
- `implementacion-contexto-trasversal-unificado-gestion-respuesta`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
