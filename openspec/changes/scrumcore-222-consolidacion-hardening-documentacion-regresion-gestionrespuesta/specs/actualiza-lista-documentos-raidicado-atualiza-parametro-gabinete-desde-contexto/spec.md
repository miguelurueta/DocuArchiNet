## MODIFIED Requirements

### Requirement: Fuente única de gabinete por contexto
`useListaDocumentosRadicadosTreeTable` SHALL depender exclusivamente del contexto transversal para estado de gabinete y no ejecutar requests de gabinete locales.

#### Scenario: Eliminación de fetch local
- **WHEN** el hook procesa acciones y consultas documentales
- **THEN** no se invoca `getSolicitaGabinetePorTareaWorkflow`
- **AND** consume `nombreGabinete`, `gabineteLoading`, `gabineteError` de `useGestionRespuestaDocumentos`

#### Scenario: Carga de árbol durante transiciones de gabinete
- **GIVEN** existe data previamente cargada de documentos
- **WHEN** `gabineteLoading` = `true`
- **THEN** la grilla conserva visibilidad y `load/loadChildren` mantienen su contrato actual

#### Scenario: Acción ver_documento robusta
- **WHEN** `gabineteError` está activo o falta `nombreGabinete`
- **THEN** la acción `ver_documento` retorna error funcional controlado
- **AND** no modifica el flujo base del visor existente.

### Requirement: No regresión en query/actions contract
- **WHEN** se ejecutan los casos de `load`, `loadChildren`, `rows`, `error` y `loading`
- **THEN** la forma de retorno y payloads permanecen compatibles con implementaciones existentes.
