## Purpose

Definir el contrato enterprise para el contexto documental transversal de
`GestionRespuesta`, incluyendo datos compartidos, resolucion segura de gabinete,
compatibilidad con adjuntos/visor/documentos y documentacion tecnica del cambio.

## Requirements

### Requirement: Contexto documental transversal de GestionRespuesta
El sistema SHALL exponer un contexto documental transversal acotado para `GestionRespuesta` con `idTareaWf`, `radicado`, `idRespuestaRadicado`, `nombreGabinete`, `gabineteLoading`, `gabineteError`, `reloadGabinete`, `files` y `setFiles`.

#### Scenario: Provider expone identificadores y adjuntos compartidos
- **GIVEN** `GestionRespuestaDocumentosProvider` recibe `idTareaWf`, `radicado` e `idRespuestaRadicado`
- **WHEN** un consumer invoca `useGestionRespuestaDocumentos`
- **THEN** el hook retorna esos valores junto con `files`, `setFiles` y `available: true`

#### Scenario: Compatibilidad con consumers actuales
- **GIVEN** un consumer actual solo usa `files` y `setFiles`
- **WHEN** se actualiza el contexto con el nuevo contrato
- **THEN** el consumer sigue funcionando sin cambiar su logica de adjuntos

#### Scenario: Hook fuera del provider conserva fallback seguro
- **GIVEN** `useGestionRespuestaDocumentos` se invoca fuera del provider
- **WHEN** el hook resuelve su estado
- **THEN** retorna `available: false`, `files: []`, `setFiles` no-op, `gabineteLoading: false`, `gabineteError: undefined`, `nombreGabinete: undefined` y `reloadGabinete` como promesa resuelta

#### Scenario: El contexto no se convierte en god context
- **GIVEN** se agregan nuevos datos al contexto
- **WHEN** se revisa el contrato publico
- **THEN** solo contiene datos documentales compartidos y no incluye estados locales de formularios, flags visuales locales ni reglas de negocio

### Requirement: Resolucion idempotente y cancelable de gabinete
El sistema SHALL resolver `nombreGabinete` desde `GET /api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete` una sola vez por `idTareaWf`, con reload explicito, cancelacion segura y sin requests duplicados.

#### Scenario: Carga automatica una sola vez por idTareaWf
- **GIVEN** el provider recibe `idTareaWf` valido
- **WHEN** se monta o re-renderiza sin cambiar `idTareaWf`
- **THEN** ejecuta como maximo una request automatica de gabinete para ese id

#### Scenario: Sin idTareaWf valido no se consulta backend
- **GIVEN** el provider no recibe `idTareaWf` o recibe un valor no finito
- **WHEN** se monta
- **THEN** no llama el service de gabinete y expone `nombreGabinete: undefined`

#### Scenario: Response backend normaliza NombreGabinete
- **GIVEN** el service retorna un response con `data.NombreGabinete`
- **WHEN** el provider procesa el resultado
- **THEN** el contexto expone `nombreGabinete` con ese valor normalizado

#### Scenario: Fallback seguro cuando no hay gabinete
- **GIVEN** el backend retorna `data: null` o sin `NombreGabinete`
- **WHEN** el provider procesa el resultado
- **THEN** `nombreGabinete` queda `undefined` y el render no se rompe

#### Scenario: reloadGabinete fuerza recarga explicita
- **GIVEN** `idTareaWf` ya fue cargado
- **WHEN** un consumer llama `reloadGabinete`
- **THEN** el provider ejecuta una nueva request para el id actual y actualiza `nombreGabinete`

#### Scenario: Cambio rapido de idTareaWf cancela request anterior
- **GIVEN** existe una request de gabinete en curso para un `idTareaWf`
- **WHEN** el provider recibe un `idTareaWf` diferente antes de completar la request
- **THEN** la request anterior se cancela o queda invalidada y no puede sobrescribir el estado del nuevo id

#### Scenario: Error backend no rompe render
- **GIVEN** el service de gabinete falla
- **WHEN** el provider captura el error
- **THEN** expone `gabineteError` como string, `gabineteLoading: false`, mantiene render estable y no lanza error durante render

#### Scenario: reloadGabinete es estable
- **GIVEN** el provider re-renderiza por cambios de estado no relacionados
- **WHEN** un consumer compara la referencia de `reloadGabinete`
- **THEN** la funcion mantiene identidad estable mientras no cambie su dependencia funcional necesaria

### Requirement: Fronteras de arquitectura y consumidores
El sistema SHALL mantener la request backend en service/hook/context y SHALL impedir que componentes UI resuelvan gabinete o casing backend manualmente.

#### Scenario: Componentes UI no usan axios directo
- **GIVEN** se revisan `GestionRespuesta`, `AppVisorEmbedPdf`, `DocumentosWorkbench` y componentes de adjuntos
- **WHEN** requieren datos de gabinete o estado documental compartido
- **THEN** acceden mediante `useGestionRespuestaDocumentos` o props existentes y no llaman axios directamente

#### Scenario: Service conserva endpoint backend
- **GIVEN** se implementa soporte de cancelacion
- **WHEN** el service de gabinete se actualiza
- **THEN** mantiene el endpoint `/api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete` y no cambia el contrato backend

#### Scenario: Visor y documentos comparten estado sin regresion
- **GIVEN** `AppVisorEmbedPdf`, `DocumentosWorkbench` y adjuntos operan dentro del provider
- **WHEN** se carga o recarga gabinete
- **THEN** los flujos existentes siguen renderizando y el estado de adjuntos no se pierde

### Requirement: Pruebas, documentacion y calidad del cambio
El sistema SHALL entregar pruebas y documentacion explicita para SCRUMCORE-220, cubriendo contrato, idempotencia, cancelacion, compatibilidad y regresion.

#### Scenario: Pruebas unitarias cubren provider y hook
- **GIVEN** se ejecutan las pruebas afectadas
- **WHEN** se valida el contexto documental
- **THEN** existen casos para estado expuesto, `reloadGabinete`, `gabineteLoading`, `gabineteError`, fallback fuera del provider y compatibilidad `files/setFiles`

#### Scenario: Pruebas cubren idempotencia y cancelacion
- **GIVEN** se simulan re-renders, reload y cambio rapido de `idTareaWf`
- **WHEN** se inspeccionan llamadas al service
- **THEN** no hay fetch duplicado automatico, reload fuerza fetch y respuestas stale no sobrescriben estado

#### Scenario: Pruebas de integracion validan consumers
- **GIVEN** se montan consumers representativos del modulo
- **WHEN** leen `nombreGabinete`, adjuntos y estado documental
- **THEN** reciben datos desde el contexto y no presentan regresion de visor, documentos ni adjuntos

#### Scenario: Documentacion obligatoria generada
- **GIVEN** finaliza la implementacion
- **WHEN** se revisa `docs/modulos/gestioncorrespondencia/normalizainiciorespuesta/contextounificadovariables/`
- **THEN** existen `SCRUMCORE-220-Arquitectura.md`, `SCRUMCORE-220-Implementacion-Detallada.md`, `SCRUMCORE-220-Integracion-BackEnd.md`, `SCRUMCORE-220-Pruebas.md` y `SCRUMCORE-220-Metadata.md`

#### Scenario: Documentacion contiene trazabilidad completa
- **GIVEN** se revisan los documentos de SCRUMCORE-220
- **WHEN** se comparan contra el alcance del ticket
- **THEN** incluyen decisiones, restricciones, diagramas Mermaid, endpoint, estrategia de idempotencia/cache, cancelacion, fallback, pruebas ejecutadas, riesgos, mitigaciones y referencias a codigo

#### Scenario: Calidad sin regresiones
- **GIVEN** se ejecuta la validacion final
- **WHEN** se corren TypeScript/build/tests relevantes y validacion OpenSpec
- **THEN** no hay errores TypeScript, no hay warnings runtime nuevos, no hay errores consola conocidos y no hay regresiones funcionales en visor/documentos/adjuntos
