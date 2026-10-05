<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Purpose

Define el comportamiento seguro y verificable para consultar y registrar recibos SII en rutas Workflow, preservando la compatibilidad de los formularios relacionados. La capacidad cubre canonización, contexto autoritativo, autorización, persistencia idempotente y evidencia de cierre.

## ADDED Requirements

### Requirement: RQ-01 Contrato de los formularios de registro

Los formularios MUST permitir validar y serializar sus controles de ruta, flujo y flujo SII sin depender de atributos ausentes y sin modificar el validador compartido. Implementa D-01.

#### Scenario: Validación del formulario completo

- **WHEN** el usuario pulsa Aceptar con los seis valores válidos
- **THEN** el validador recorre exactamente `recibo`, `codigo_barras`, `matricula`, `rscocial`, `id_tramite` e `id_actividad` sin excepción y entrega un único comando de registro

#### Scenario: Registro para flujo

- **WHEN** el usuario pulsa Aceptar en Registrar tarea para flujo
- **THEN** los ocho controles de `conten_registro_flujo` incluyen `atrib_campo_beetwen="0"` y el validador termina sin desreferenciar un atributo ausente

#### Scenario: Registro RUE o virtual

- **WHEN** el usuario pulsa Aceptar en un registro RUE o virtual
- **THEN** los ocho controles de `conten_registro_flujo_tarea_sii` incluyen `atrib_campo_beetwen="0"` y conservan los demás atributos, nombres y eventos existentes

### Requirement: RQ-02 Recibo canónico determinista

La operación de ruta MUST aceptar únicamente prefijo `S` o `R` y un consecutivo de uno a nueve dígitos, produciendo prefijo más nueve dígitos. Implementa D-02.

#### Scenario: Entrada válida e idempotente

- **WHEN** se canoniza un consecutivo válido con espacios laterales o un recibo ya canónico
- **THEN** se obtiene el mismo valor canónico al aplicar la función una o más veces

#### Scenario: Entrada ambigua o inválida

- **WHEN** el valor está vacío, contiene letras internas, prefijo contradictorio, guion, caracteres no numéricos o más de nueve dígitos
- **THEN** el cliente muestra error saneado y no consulta ni registra

### Requirement: RQ-03 Contexto consultado vigente y catálogos inequívocos

El cliente MUST conservar una instantánea privada de la última consulta válida y fallar cerrado ante estados incompletos o modificados. Implementa D-03.

#### Scenario: Edición posterior a la consulta

- **WHEN** el usuario cambia el prefijo o recibo después de una consulta exitosa
- **THEN** se invalidan la instantánea, código de barras, matrícula, razón social, trámite y actividad, y Aceptar no llama al servicio mutante

#### Scenario: Respuesta incompleta

- **WHEN** `data.d` está vacío, contiene más de un resultado, carece de radicado o razón social, o retorna error funcional
- **THEN** la promesa termina controladamente, la interfaz recupera su estado y no queda contexto vigente

#### Scenario: Matrícula y subtipo opcionales

- **WHEN** SII retorna una consulta válida con matrícula o subtipo de trámite vacíos
- **THEN** la matrícula se conserva vacía y, cuando falta el subtipo, cliente y servidor usan el tipo de trámite del recibo para autoseleccionar y validar una única coincidencia del catálogo

#### Scenario: Trámite no inequívoco

- **WHEN** el tipo efectivo SII —subtipo del radicado o, si está vacío, tipo del recibo— tiene cero o más de una coincidencia normalizada en el catálogo
- **THEN** no se selecciona el primer trámite, se invalida la consulta y se informa el problema

#### Scenario: Actividad no seleccionada

- **WHEN** la actividad es vacía, `0`, `-1` o no pertenece al catálogo retornado
- **THEN** el registro se bloquea en cliente y servidor sin escritura

### Requirement: RQ-04 Autorización y contexto autoritativo en servidor

El endpoint mutante MUST derivar identidad y datos registrables desde sesión, SII y catálogos server-side, no desde campos visuales. Implementa D-04.

#### Scenario: Sesión o permiso ausente

- **WHEN** se invoca el endpoint sin usuario Workflow válido o sin `UTIL_SII_REGISTRO_TAREA_RUTA`
- **THEN** se retorna rechazo saneado y no se abre una transacción de escritura

#### Scenario: Petición manipulada

- **WHEN** el navegador envía código de barras, matrícula o razón social distintos a la reconsulta SII
- **THEN** esos campos se ignoran, se reconstruye el contexto autoritativo y no se persiste una mezcla de recibos

#### Scenario: Identificadores inválidos

- **WHEN** trámite o actividad no son positivos o no existen en sus catálogos permitidos
- **THEN** el servidor rechaza antes de escribir

#### Scenario: Error interno

- **WHEN** SII, catálogo o persistencia falla
- **THEN** la respuesta pública no contiene SQL, cadena de conexión, ruta física ni traza

### Requirement: RQ-05 Persistencia parametrizada, localmente atómica e idempotente entre bases

La operación MUST confirmar tarea y evento outbox en una única transacción Workflow, materializar idempotentemente la relación en Docuarchi y serializar intentos concurrentes del mismo recibo. Implementa D-05.

#### Scenario: Registro exitoso

- **WHEN** el comando autorizado y revalidado se ejecuta
- **THEN** registro público, inicio, datos adicionales, estado, log aplicable y evento outbox quedan confirmados en una transacción Workflow parametrizada; luego la relación queda confirmada idempotentemente en Docuarchi y el evento se marca completado

#### Scenario: Falla intermedia

- **WHEN** falla una escritura antes del commit Workflow
- **THEN** la transacción local revierte y no queda tarea ni evento parcial

#### Scenario: Docuarchi no disponible después del alta

- **WHEN** la tarea y su evento fueron confirmados pero la relación no puede materializarse
- **THEN** el evento permanece reintentable, la respuesta es `REGISTERED_RELATION_PENDING` y ningún reintento crea una segunda tarea

#### Scenario: Reconciliación idempotente

- **WHEN** se reintenta un evento cuya relación ya existe para el mismo expediente
- **THEN** se confirma el evento sin insertar una segunda relación; si apunta a otro expediente queda en revisión sin sobrescribir datos

#### Scenario: Solicitudes concurrentes

- **WHEN** dos solicitudes intentan registrar simultáneamente la misma combinación de ruta y recibo
- **THEN** como máximo una confirma y la otra recibe un resultado funcional de duplicado

#### Scenario: Recibo histórico o registrado por otro consumidor

- **WHEN** `F_W_E_REGISTROPUBLICO` ya contiene el recibo aunque no exista evento DOC-87 en el outbox
- **THEN** el servidor responde `ALREADY_REGISTERED`, no crea tarea, estado ni evento, y la interfaz informa que el recibo ya estaba registrado sin presentar un alta exitosa

#### Scenario: Reintento con evento existente

- **WHEN** el outbox ya contiene el recibo y se reintenta para reconciliar su relación documental
- **THEN** no se crea otra tarea y la respuesta conserva semántica de recibo ya registrado, diferenciando si la relación continúa pendiente

#### Scenario: Identificador dinámico no permitido

- **WHEN** el nombre de ruta no satisface el patrón cerrado esperado
- **THEN** no se construye ni ejecuta SQL dinámico

### Requirement: RQ-06 Experiencia, compatibilidad y evidencia de cierre

La corrección MUST entregar confirmación visible, evitar doble envío y demostrar que los módulos protegidos no cambiaron. Implementa D-06.

#### Scenario: Confirmación de éxito

- **WHEN** el servidor confirma el registro
- **THEN** la interfaz muestra una confirmación inequívoca, limpia el estado una sola vez y no navega, recarga ni hace postback

#### Scenario: Recursos actualizados

- **WHEN** se despliega DOC-87
- **THEN** los JavaScript modificados se solicitan con una versión de caché nueva

#### Scenario: Verificación local

- **WHEN** se ejecutan pruebas focales, regresión protegida, compilación y `git diff --check`
- **THEN** todas aprueban sin cambios en importación SII, Radicación Simplificada, adjuntos o `FileUploadHandler_.ashx`

#### Scenario: E2E real sin autorización

- **WHEN** faltan autorización explícita, cuenta, ambiente o recibo descartable
- **THEN** la E2E no se ejecuta y el cambio conserva un bloqueo de cierre documentado

#### Scenario: E2E real autorizada

- **WHEN** están autorizados el ambiente, la cuenta y los datos descartables
- **THEN** la infraestructura existente de `tools/e2e` demuestra el alta única y los rechazos sin escritura mediante consultas de control solo `SELECT`, y deja el gate en `false` con usuarios y grupos vacíos
