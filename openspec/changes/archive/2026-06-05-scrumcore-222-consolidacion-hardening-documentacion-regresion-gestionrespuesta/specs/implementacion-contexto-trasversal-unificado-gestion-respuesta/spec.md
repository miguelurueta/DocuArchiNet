## Purpose

Definir el contrato enterprise para el contexto documental transversal de
GestionRespuesta, incluyendo datos compartidos, resolución segura de gabinete,
compatibilidad con adjuntos/visor/documentos y documentación técnica del cambio.

## Requirements
### Requirement: Contexto documental transversal de GestionRespuesta
El sistema SHALL mantener `GestionRespuestaDocumentosContext` como estado transversal documental, sin ampliar responsabilidades a UI local.

#### Scenario: Estabilidad de contrato de contexto
- **GIVEN** un consumidor existente usa `files` y `setFiles`
- **WHEN** se ejecuta cualquier validación de regresión del ticket
- **THEN** la firma y comportamiento de `files`/`setFiles` permanecen sin cambios funcionales
- **AND** no se introducen estados de UI no transversales en el proveedor

#### Scenario: Estado transversal consistente
- **GIVEN** cambios de render y recarga normales
- **WHEN** se actualizan estados transversales (`idTareaWf`, `radicado`, `idRespuestaRadicado`, `nombreGabinete`)
- **THEN** el proveedor actualiza solo estado documental y efectos asociados
- **AND** no rompe la integración de adjuntos ni acciones actuales

### Requirement: Resolución idempotente y cancelable de gabinete
El sistema SHALL proteger el estado de render cuando falla la resolución de gabinete o no existe `idTareaWf`.

#### Scenario: Manejo de fallback de gabinete
- **WHEN** falla la resolución de gabinete o no existe `idTareaWf`
- **THEN** se mantiene render estable
- **AND** `gabineteError` reporta un error explícito sin bloquear árbol/visor
- **AND** `gabineteLoading` vuelve a `false` en estado final

### Requirement: Fronteras de arquitectura y consumidores
El sistema SHALL mantener la request backend en `service/hook/context` y prevenir que componentes UI resuelvan gabinete o `casing` backend manualmente.

#### Scenario: Componentes UI no usan axios directo
- **GIVEN** GestionRespuesta, AppVisorEmbedPdf y DocumentosWorkbench requieren datos de gabinete o estado documental compartido
- **WHEN** consumen esos datos
- **THEN** acceden mediante `useGestionRespuestaDocumentos` o props existentes
- **AND** no llaman servicios directos de backend

#### Scenario: Service conserva endpoint backend
- **GIVEN** se implementa soporte de cancelación
- **WHEN** el servicio de gabinete se ejecuta
- **THEN** mantiene el endpoint `/api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`
- **AND** no cambia contrato backend

#### Scenario: Visor y documentos comparten estado sin regresión
- **GIVEN** AppVisorEmbedPdf, DocumentosWorkbench y adjuntos operan dentro del provider
- **WHEN** se carga o recarga gabinete
- **THEN** los flujos existentes siguen renderizando
- **AND** el estado de adjuntos no se pierde

### Requirement: Pruebas, documentación y calidad del cambio
El sistema SHALL entregar pruebas y documentación explícita para SCRUMCORE-220, cubriendo contrato, idempotencia, cancelación, compatibilidad y regresión.

#### Scenario: Pruebas unitarias cubren provider y hook
- **GIVEN** se ejecutan las pruebas afectadas
- **WHEN** se valida el contexto documental
- **THEN** existen casos para estado expuesto, `reloadGabinete`, `gabineteLoading`, `gabineteError` y compatibilidad con `files`/`setFiles`

#### Scenario: Pruebas cubren idempotencia y cancelación
- **GIVEN** se simulan re-renders, `reload` y cambio rápido de `idTareaWf`
- **WHEN** se inspeccionan llamadas al servicio
- **THEN** no hay fetch duplicado automático, `reload` fuerza fetch y respuestas obsoletas no sobrescriben estado

#### Scenario: Pruebas de integración validan consumers
- **GIVEN** se montan consumers del módulo
- **WHEN** leen `nombreGabinete`, `gabineteLoading`, `gabineteError`
- **THEN** reciben datos del contexto sin regresión de visor, documentos ni adjuntos

#### Scenario: Documentación obligatoria generada
- **GIVEN** finaliza la implementación
- **WHEN** se revisa `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/contextounificadovariables/`
- **THEN** existen `SCRUMCORE-220-Arquitectura.md`, `SCRUMCORE-220-Implementacion-Detallada.md`, `SCRUMCORE-220-Integracion-BackEnd.md`, `SCRUMCORE-220-Pruebas.md` y `SCRUMCORE-220-Metadata.md`

#### Scenario: Documentación contiene trazabilidad completa
- **GIVEN** se revisan los documentos de SCRUMCORE-220
- **WHEN** se comparan contra el alcance del ticket
- **THEN** incluyen decisiones, restricciones, diagramas Mermaid, endpoint, idempotencia/cache, cancelación, fallback, pruebas ejecutadas, riesgos, mitigaciones y referencias a código

#### Scenario: Calidad sin regresiones
- **GIVEN** se ejecuta la validación final
- **WHEN** se corren TypeScript/build/tests relevantes y validación OpenSpec
- **THEN** no hay errores TypeScript, warnings runtime nuevos, errores de consola conocidos ni regresiones funcionales en visor/documentos/adjuntos
