# fix-sellos-sii Specification

## Purpose

Garantizar que los sellos y constancias importados desde SII se proyecten de inmediato como documentos Workflow completos y operables, sin recargar la lista ni alterar el recorrido protegido de ENLASE.

## Requirements

### Requirement: RQ-01 Proyección Workflow completa desde el almacenamiento confirmado

Decisión de origen: D-01.

El sistema SHALL construir una proyección Workflow independiente de ENLASE después de almacenar correctamente un sello o constancia, sin consultas adicionales.

#### Scenario: Ocho campos completos

- **WHEN** `AlmacenaDocumentoTareaWorkflow` retorna `YES`, un `IdDocumento` válido y `EstructuraDatosImagen`
- **THEN** la proyección contiene gabinete, documento, radicado, tipo físico, tipología, tarea, estado de firma e icono
- **AND** utiliza `DBT` o, si está vacío, `extension` como tipo físico
- **AND** conserva la tipología seleccionada en lugar de fabricar `RAD-<radicado>`

#### Scenario: Proyección incompleta

- **WHEN** falta un campo obligatorio o la identidad no coincide
- **THEN** el sistema no publica una proyección Workflow utilizable
- **AND** no fabrica valores parciales ni consulta nuevamente servidor o base de datos

### Requirement: RQ-02 Propagación efímera y reconciliación segura

Decisión de origen: D-02.

El sistema SHALL propagar `WorkflowProjection` mediante tipos propios desde el almacenamiento hasta la respuesta moderna, sin persistir cadenas delimitadas.

#### Scenario: Ejecución confirmada

- **WHEN** un ítem alcanza un estado confirmado con documento y tarea válidos
- **THEN** `ImportItemResultDto.WorkflowProjection` contiene la proyección correspondiente

#### Scenario: Reemplazo por snapshot autoritativo

- **WHEN** la reconciliación sustituye el ítem de ejecución por un snapshot persistido
- **THEN** conserva la proyección efímera solo si coinciden capacidad Workflow, `ClientItemId`, `ExternalKey`, `DocumentId` y `TaskId`
- **AND** descarta la proyección ante cualquier diferencia o campo obligatorio vacío

### Requirement: RQ-03 Inserción JavaScript Workflow completa y deduplicada

Decisión de origen: D-03.

El sistema SHALL traducir la proyección Workflow al contrato histórico únicamente en un adaptador cliente exclusivo.

#### Scenario: Inserción válida

- **WHEN** el resultado está confirmado, corresponde a la tarea visible y contiene ocho campos válidos
- **THEN** el adaptador llama una sola vez `insert_row_documento_relacionado(legacyData, "wf", 1)`
- **AND** la fila creada en `GridView_list_documento_relacion_wf` contiene `id_wf` e `idd_wf` completos
- **AND** los valores visibles están escapados y no contienen el delimitador `|`

#### Scenario: Documento duplicado o lote múltiple

- **WHEN** el GridView ya contiene el `DocumentId`
- **THEN** el adaptador considera satisfecha la proyección sin insertar otra fila
- **AND** un lote inserta cada documento confirmado una sola vez en el orden recibido

#### Scenario: Tarea o proyección inválida

- **WHEN** `TaskId` no coincide con la tarea visible o falta un campo obligatorio
- **THEN** no se inserta ninguna fila parcial

### Requirement: RQ-04 Cierre sin recarga y fallo cerrado

Decisión de origen: D-04.

El sistema SHALL cerrar el modal únicamente después de comprobar todas las filas confirmadas en el GridView correcto.

#### Scenario: Proyección comprobada

- **WHEN** todos los documentos confirmados fueron insertados o ya existían en la lista Workflow
- **THEN** el modal puede cerrarse
- **AND** no ocurre postback, `DataBind`, `PageRequestManager`, clic de actualización ni recarga parcial/completa

#### Scenario: Proyección no comprobable

- **WHEN** alguna fila no puede construirse o comprobarse
- **THEN** el modal permanece abierto
- **AND** informa que el documento fue importado pero no pudo proyectarse de forma segura

### Requirement: RQ-05 Integración y versión de recursos

Decisión de origen: D-05.

El sistema SHALL registrar y cargar el adaptador Workflow antes de la UI y renovar la versión pública de cada script modificado.

#### Scenario: Registro del módulo

- **WHEN** se renderiza `Webworkflow.aspx`
- **THEN** el módulo Workflow está incluido en el proyecto y cargado antes de `importar-servicio-web-ui.js`
- **AND** las versiones registradas no reutilizan una versión previamente desplegada

### Requirement: RQ-06 Invariante ENLASE y evidencia local

Decisión de origen: D-06.

El sistema SHALL preservar íntegramente el recorrido ENLASE y demostrar la corrección mediante pruebas determinísticas.

#### Scenario: Aislamiento de capacidades

- **WHEN** la capacidad es `ANEXOS_RADICADO_ENLASE`
- **THEN** se utiliza exclusivamente `EnlaseProjection` y destino `rad`
- **AND** no se utiliza `WorkflowProjection`

#### Scenario: Sellos Workflow

- **WHEN** la capacidad no es ENLASE
- **THEN** se utiliza exclusivamente `WorkflowProjection` y destino `wf`
- **AND** no se utiliza `EnlaseProjection`

#### Scenario: Validación local

- **WHEN** se ejecutan las suites focales y de regresión
- **THEN** pasan sin autenticación, red ni mutación externa
- **AND** cualquier E2E no autorizada se documenta como no ejecutada, nunca como evidencia simulada

### Requirement: RQ-07 Tipología predeterminada para constancias

Decisión de origen: D-07.

El sistema SHALL proponer automáticamente la tipología autorizada equivalente a `Constancia de Inscripción` durante la preparación de sellos.

#### Scenario: Coincidencia inequívoca

- **WHEN** el catálogo contiene una única tipología cuyo nombre corresponde a Constancia de Inscripción
- **THEN** esa tipología queda seleccionada para todos los documentos preparados
- **AND** conserva el identificador y nombre exactos del catálogo
- **AND** tiene prioridad sobre otra tipología marcada como obligatoria

#### Scenario: Variante ortográfica

- **WHEN** el nombre difiere solo por mayúsculas, tildes, separadores o hasta dos ediciones por palabra conceptual
- **THEN** se reconoce como equivalente, incluyendo variantes como `Contancia de Inscrpcion`

#### Scenario: Coincidencia ambigua

- **WHEN** dos o más tipologías autorizadas parecen equivalentes a Constancia de Inscripción
- **THEN** el sistema no predetermina ninguna
- **AND** mantiene la selección manual del usuario
