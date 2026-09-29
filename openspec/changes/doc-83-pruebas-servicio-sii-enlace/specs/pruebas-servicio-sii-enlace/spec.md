<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Purpose

Establecer evidencia reproducible y segura de que la importación moderna de anexos SII ENLASE funciona, conserva legacy y puede activarse y revertirse sin contaminar tareas ni ocultar fallos.

## ADDED Requirements

Trazabilidad de decisiones: D-01, D-02, D-03, D-04, D-05, D-06, D-07, D-08, D-09.
### Requirement: RQ-01 Matriz de cobertura trazable
El sistema SHALL mantener un inventario versionado que relacione riesgos y condiciones de cierre con pruebas, escenarios E2E, autorizaciones, controles y evidencia.

#### Scenario: Cobertura incompleta
- **WHEN** falta una prueba o evidencia obligatoria para un riesgo alto o muy alto
- **THEN** la validación falla indicando el riesgo y la referencia ausente

### Requirement: RQ-02 Regresión determinista integral
El sistema SHALL validar localmente capacidad, contexto ENLASE, consulta, preview, preparación, tipología, intención, reconciliación, lista documental, cambio de tarea, asignación explícita, accesibilidad, gate y legacy.

#### Scenario: Suite afectada
- **WHEN** se ejecuta la batería DOC-83
- **THEN** todas las suites registradas pasan sin debilitar validaciones productivas ni duplicar fixtures

### Requirement: RQ-03 Lectura e interfaz sin mutaciones
El sistema SHALL demostrar que los recorridos reales de lectura y UI no modifican tarea, auditoría, documentos, expediente, índices, caché ni asignación.

#### Scenario: Lectura autorizada
- **WHEN** se ejecuta una lectura o recorrido UI ENLASE autorizado
- **THEN** los controles registrados permanecen invariantes y la evidencia confirma preview y contexto originales

#### Scenario: Datos extensos en la lista SII
- **WHEN** naturaleza, noticia, referencia, nombre o tipo exceden el ancho asignado de su columna
- **THEN** cada valor permanece contenido con elipsis y título completo, la columna de acciones conserva un fondo opaco y el desplazamiento ocurre dentro de la tabla sin superponer datos

#### Scenario: Preparación masiva y vista previa navegables
- **WHEN** la selección contiene suficientes documentos para exceder el alto disponible o el usuario abre la vista previa
- **THEN** solo la lista de preparación se desplaza, `Cancelar` y `Crear intención` permanecen visibles, no se duplica el detalle del preflight como “Plan previsto”, y la vista previa reemplaza temporalmente la lista con una acción visible para volver y un visor alineado dentro del alto disponible

### Requirement: RQ-04 Importación idempotente y aislada
El sistema SHALL demostrar que una selección individual o múltiple usa una intención idempotente, persiste únicamente documentos confirmados y no asigna la tarea.

#### Scenario: Ejecución múltiple autorizada
- **WHEN** se importa una selección sobre un recurso descartable autorizado
- **THEN** existe una ejecución efectiva, evidencia física por elemento, ausencia de duplicados y tarea sin transición

#### Scenario: Resultado incierto o repetido
- **WHEN** se pierde una respuesta o se consulta nuevamente la misma intención
- **THEN** el sistema reconcilia antes de reintentar y no crea documentos duplicados

#### Scenario: Tarea devuelta o reabierta
- **WHEN** la tarea tiene un estado activo actual para la ruta ENLASE aunque existan ciclos históricos finalizados
- **THEN** la tarea es operable sin usar el historial como veto

#### Scenario: Reimportación explícita de un antecedente vigente
- **WHEN** la misma `ExternalKey` conserva evidencia física confirmada y el usuario elige `Reimportar`
- **THEN** el preflight exige `ReimportRequested=true`, incorpora `DOCUMENT_REIMPORT_EXPLICIT` al plan y permite crear la nueva intención

#### Scenario: Antecedente eliminado físicamente
- **WHEN** existe historial confirmado pero el documento ya no existe en el gabinete verificable
- **THEN** el elemento vuelve a estado `Disponible` y se importa como recurso normal, sin exigir reimportación

#### Scenario: Dos documentos del mismo tipo
- **WHEN** SII entrega dos `ExternalKey` distintas clasificadas con la misma tipología documental
- **THEN** ambas pueden integrar la misma selección y no se consideran duplicadas entre sí

#### Scenario: Proyección inmediata ENLASE con contrato completo
- **WHEN** el almacenamiento y la reconciliación confirman un documento de la tarea ENLASE visible
- **THEN** la respuesta transporta una proyección tipada con gabinete, identificador, radicado, tipo físico, nombre, tarea, firma e icono, y el adaptador ENLASE acepta los estados autoritativos confirmados `Disponible` y `Completado` para insertarla en `GridView_list_documento_relacion` con `id_rad`/`idd_rad` completos, sin recargar el sistema ni hacer postback del `GridView`

#### Scenario: Proyección efímera preservada por reconciliación
- **WHEN** `ProjectExecutionResult` sustituye el resultado de ejecución por el snapshot autoritativo ENLASE
- **THEN** conserva la proyección efímera únicamente si capacidad, `ClientItemId`, `ExternalKey`, documento, tarea y campos visuales coinciden con el ítem confirmado; cualquier diferencia elimina la proyección

#### Scenario: Proyección sin DBT en importación sin tipología
- **WHEN** el almacenamiento confirmado no retorna DBT porque el trámite permite importar sin tipología, pero sí retorna la extensión física del archivo
- **THEN** la proyección usa esa extensión como descriptor físico autoritativo, sin inferir tipología ni realizar una segunda consulta

#### Scenario: Proyección ENLASE incompleta
- **WHEN** falta cualquier dato obligatorio o la proyección no coincide con el documento y la tarea confirmados
- **THEN** la interfaz no crea una fila parcial, mantiene visible el resultado y no invoca interacciones heredadas con gabinete vacío


### Requirement: RQ-05 Gate reversible y legacy disponible
El sistema SHALL usar el gate existente, publicarlo activo globalmente en despliegues `Release`, sin audiencias de usuario o grupo, y conservar el recorrido legacy cuando el gate esté apagado en la configuración base reversible.

#### Scenario: Activación oficial transversal
- **WHEN** se aplica la transformación `Web.Release.config`
- **THEN** `WorkflowCentroTrabajoModernActive` queda en `true`, usuarios y grupos permanecen vacíos y toda sesión Workflow válida puede usar la capacidad sin filtros por identidad o grupo

#### Scenario: Finalización normal o fallida
- **WHEN** termina o se interrumpe cualquier corrida que habilitó temporalmente el gate
- **THEN** el gate queda en `false`, usuarios y grupos vacíos y la integridad legacy coincide con la línea base

#### Scenario: Aceptación visual manual controlada
- **WHEN** un responsable autoriza ambiente, gate, ejecución y recurso descartable para comprobar la proyección inmediata ENLASE
- **THEN** el runner abre una sesión autenticada visible, espera que WebForms quede estable, abre la ventana moderna y exige una lista SII importable antes de tomar la línea base; luego limita la inspección a diez minutos, exige una llamada real al inserter y una fila nueva sin recarga ni postback, valida una interacción sin error, captura los controles registrados y restaura el gate en `finally`
#### Scenario: Gate apagado
- **WHEN** la funcionalidad moderna no está habilitada
- **THEN** no se registra el bootstrap moderno y los handlers legacy continúan disponibles

### Requirement: RQ-06 Autorización y acceso negativo
El sistema SHALL fallar cerrado ante sesión ausente, contexto no ENLASE, capacidad incompatible, tarea distinta, autorización incompleta o acceso directo no permitido.

#### Scenario: Solicitud no autorizada
- **WHEN** falta cualquiera de las condiciones autoritativas
- **THEN** no se consulta ni persiste contenido externo y se devuelve únicamente un código seguro

### Requirement: RQ-07 Asignación separada y revalidada
El sistema SHALL mantener la asignación como acción explícita posterior y delegar la revalidación final a `Buttonaceptar_Click`.

#### Scenario: Documentos obligatorios faltantes
- **WHEN** el usuario intenta asignar y la validación autoritativa no devuelve `YES`
- **THEN** la tarea permanece sin asignar y se informa el requisito pendiente

#### Scenario: Importación terminada
- **WHEN** una importación completa finaliza
- **THEN** no se dispara automáticamente la asignación ni se afirma preventivamente que los requisitos están completos

### Requirement: RQ-08 Evidencia saneada y controles seguros
El sistema SHALL producir evidencia mínima sin secretos y ejecutar únicamente controles `SELECT` registrados mediante una cuenta de solo lectura.

#### Scenario: Evidencia generada
- **WHEN** una corrida termina con éxito o error
- **THEN** la evidencia conserva códigos, conteos, huellas y restauración, y rechaza credenciales, cookies, tokens, conexiones y cuerpos del proveedor

### Requirement: RQ-09 Cierre OPSXJ condicionado
El sistema SHALL impedir el cierre completo mientras falte una prueba obligatoria, autorización, recurso descartable, restauración, compatibilidad legacy o evidencia trazable.

#### Scenario: E2E no autorizada o bloqueada
- **WHEN** no existe autorización expresa o una dependencia externa impide la corrida
- **THEN** se registra el bloqueo sin declarar validación completa ni sustituir la integración real por mocks

#### Scenario: Matriz satisfecha
- **WHEN** pruebas, build, E2E aplicables, gate, legacy, persistencia, idempotencia, tarea, asignación, evidencia y documentación cumplen la matriz
- **THEN** DOC-83 puede continuar por validación, archivo, publicación y cierre exclusivamente mediante OPSXJ
