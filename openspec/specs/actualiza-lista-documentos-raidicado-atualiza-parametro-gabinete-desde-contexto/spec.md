# actualiza-lista-documentos-raidicado-atualiza-parametro-gabinete-desde-contexto Specification

## Purpose
Cotejar y validar el refactor del caso de uso SCRUMCORE-221, garantizando que `useListaDocumentosRadicadosTreeTable` consuma `nombreGabinete` desde el contexto transversal y elimine la resolución local de gabinete.

## Requirements
### Requirement: Actualiza lista documentos y usa gabinete desde contexto
El sistema SHALL implementar el alcance definido para SCRUMCORE-221 y validar que `useListaDocumentosRadicadosTreeTable` ya no resuelve gabinete localmente.

#### Scenario: Fuente única de gabinete por contexto
- **WHEN** se ejecuta el caso de uso principal del ticket
- **THEN** la fuente de gabinete proviene solo de `useGestionRespuestaDocumentos`
- **AND** `useListaDocumentosRadicadosTreeTable` no invoca `getSolicitaGabinetePorTareaWorkflow`

#### Scenario: Carga de árbol durante transiciones de gabinete
- **GIVEN** existe data previamente cargada de documentos
- **WHEN** `gabineteLoading` cambia a `true` y luego a `false`
- **THEN** la grilla conserva visibilidad
- **AND** `load`/`loadChildren` mantienen su contrato actual

### Requirement: Detalle funcional Jira
El sistema SHALL mantener las reglas del ticket y preservar el contrato de query/actions sin regresión funcional.

#### Scenario: No regresión en contrato de load/query
- **WHEN** se ejecutan `load`, `loadChildren`, `rows`, `error` y `loading`
- **THEN** la forma de retorno, payloads y nombres de campos permanecen compatibles con la implementación existente

#### Scenario: Acción `ver_documento` robusta ante errores de gabinete
- **WHEN** `gabineteError` está activo o falta `nombreGabinete`
- **THEN** la acción `ver_documento` retorna un error funcional controlado
- **AND** no rompe el flujo base del visor ni la navegación de `AppVisorEmbedPdf`
