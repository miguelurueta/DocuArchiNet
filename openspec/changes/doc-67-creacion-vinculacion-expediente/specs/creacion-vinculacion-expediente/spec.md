<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14 -->
## Purpose

Garantizar que cada importación SII resuelva, cree, vincule, indexe y reconcilie sus expedientes y documentos de forma secuencial, idempotente, recuperable y compatible con el recorrido legacy.

## ADDED Requirements

### Requirement: RQ-01 Resolución obligatoria del expediente
Origen de diseño: D-01.
El sistema SHALL resolver y verificar todos los expedientes requeridos antes de almacenar documentos de una intención.

#### Scenario: Reutilización de expediente válido
- **WHEN** la identidad configurada corresponde exactamente a un expediente físico existente del gabinete
- **THEN** el sistema reutiliza ese expediente y no crea otro

#### Scenario: Creación de expediente faltante
- **WHEN** la configuración exige expediente y no existe uno físicamente válido
- **THEN** el sistema crea exactamente uno, verifica su identidad y persiste el resultado antes de almacenar

#### Scenario: Expediente obligatorio irresoluble
- **WHEN** la configuración, identidad, creación o verificación del expediente no puede resolverse
- **THEN** el sistema no almacena documentos ni declara completada la intención y retorna un código seguro

### Requirement: RQ-02 Conservación del agregado de inscripción
Origen de diseño: D-02.
El sistema SHALL conservar la relación entre cada inscripción SII, sus datos autoritativos y sus documentos durante planificación, ejecución, recuperación y reconciliación.

#### Scenario: Inscripción con varios documentos
- **WHEN** una inscripción contiene más de un documento seleccionable
- **THEN** todos conservan la misma clave de inscripción y sus datos de libro, registro, matrícula o proponente y sujeto

#### Scenario: Reanudación de intención
- **WHEN** una intención parcial se recupera después de una interrupción
- **THEN** el sistema reconstruye la asociación inscripción-documentos desde persistencia sin depender del cliente

### Requirement: RQ-03 Planificación única y múltiple inequívoca
Origen de diseño: D-03.
El sistema SHALL soportar expediente único y múltiples expedientes con roles primario y secundarios, asignando exactamente un destino a cada documento.

#### Scenario: Configuración de expediente único
- **WHEN** el trámite está configurado para expediente único
- **THEN** cada documento aplicable queda planificado hacia el mismo expediente verificado

#### Scenario: Configuración de primario y secundarios
- **WHEN** el trámite habilita múltiples expedientes y existen reglas de tipología
- **THEN** el sistema crea o reutiliza primario y secundarios y asigna cada documento según su regla

#### Scenario: Destino ausente o ambiguo
- **WHEN** una tipología no determina destino o determina más de uno
- **THEN** el documento no se vincula ni completa y la inconsistencia queda representada explícitamente

### Requirement: RQ-04 Identidad consistente del expediente
Origen de diseño: D-04.
El sistema SHALL localizar expedientes mediante matrícula normalizada y gabinete, y SHALL confirmar su identidad física con todos los campos únicos configurados vigentes.

#### Scenario: Normalización por gabinete
- **WHEN** se resuelve un expediente MERCANTIL, ESAL o RUP
- **THEN** el sistema aplica respectivamente matrícula mercantil, matrícula sin el prefijo `S0` o número de proponente

#### Scenario: Validación de campos únicos dinámicos
- **WHEN** el trámite configura campos con `estado_unico=1`
- **THEN** el expediente solo se acepta si todos esos campos coinciden, sin usar una lista fija universal

#### Scenario: Recuperación de expediente secundario
- **WHEN** se recupera un expediente secundario creado anteriormente
- **THEN** se usa la misma normalización e identidad física empleada durante su creación

### Requirement: RQ-05 Descubrimiento del universo documental por ENLASE
Origen de diseño: D-05.
Después del almacenamiento, el sistema SHALL descubrir el universo documental vigente usando `NombreGabinete + ENLASE = RadicadoSII` y SHALL deduplicarlo por `IdImagen`.

#### Scenario: Documentos previos y recién almacenados
- **WHEN** el gabinete contiene documentos anteriores y documentos almacenados en la ejecución actual con el mismo `ENLASE`
- **THEN** todos forman parte del plan físico y de la reconciliación exactamente una vez

#### Scenario: Única evidencia de pertenencia disponible
- **WHEN** se construye el universo documental del radicado
- **THEN** el sistema no exige otra relación física con la tarea distinta de la consulta por `ENLASE`

#### Scenario: Consulta del universo fallida
- **WHEN** no puede demostrarse el resultado de la consulta por gabinete y `ENLASE`
- **THEN** la intención no continúa a vinculación ni finalización y reporta un resultado seguro recuperable

### Requirement: RQ-06 Reejecución incremental y sellos corregidos
Origen de diseño: D-06.
El sistema SHALL conservar efectos confirmados y procesar únicamente documentos nuevos o pendientes en ejecuciones posteriores del mismo radicado.

#### Scenario: Repetición sin documentos nuevos
- **WHEN** se repite una intención o radicado cuyo universo no cambió
- **THEN** no se duplican expedientes, documentos, relaciones, cachés ni índices

#### Scenario: Nuevo sello en segundo intento
- **WHEN** una ejecución posterior descubre un `IdImagen` nuevo
- **THEN** reutiliza los expedientes confirmados y procesa el nuevo documento sin repetir los anteriores

#### Scenario: Sello corregido
- **WHEN** el nuevo documento corrige un sello almacenado previamente
- **THEN** el sistema conserva ambos documentos y reconcilia cada uno sin anular ni sustituir el anterior

### Requirement: RQ-07 Caché documental persistente y verificable
Origen de diseño: D-07.
El sistema MUST mantener una sola entrada de caché por combinación de tarea, imagen y gabinete, conservando el expediente esperado y el radicado SII.

#### Scenario: Registro posterior a verificación
- **WHEN** la relación física apunta al expediente esperado
- **THEN** el sistema registra o confirma la caché documental con su fecha de verificación

#### Scenario: Caché presente y relación ausente
- **WHEN** existe caché pero la relación física no existe
- **THEN** el sistema no acepta la caché como prueba, representa la discrepancia y aplica la recuperación autorizada

#### Scenario: Caché conflictiva
- **WHEN** la misma tarea, imagen y gabinete están cacheados hacia otro expediente
- **THEN** el sistema no sobrescribe la entrada ni vincula silenciosamente y retorna un conflicto seguro

#### Scenario: Escritura concurrente de caché
- **WHEN** dos ejecuciones intentan registrar simultáneamente la misma identidad documental
- **THEN** la persistencia conserva una sola entrada consistente

### Requirement: RQ-08 Vinculación física exacta
Origen de diseño: D-08.
El sistema SHALL vincular cada documento exactamente al expediente esperado y SHALL verificar físicamente el resultado antes de confirmarlo.

#### Scenario: Relación correcta existente
- **WHEN** el documento ya está relacionado una vez con el expediente esperado
- **THEN** el sistema conserva la relación y no invoca nuevamente el efecto mutador

#### Scenario: Relación ausente
- **WHEN** no existe relación para el documento
- **THEN** el sistema la crea una vez y confirma mediante lectura que apunta al expediente esperado

#### Scenario: Relación duplicada o cruzada
- **WHEN** existen varias relaciones o una apunta a otro expediente
- **THEN** el sistema detiene el documento, no lo declara disponible y retorna el código seguro correspondiente

#### Scenario: Respuesta mutadora ambigua
- **WHEN** la operación de vinculación retorna éxito textual pero la postcondición física no se demuestra
- **THEN** el sistema no confirma el vínculo y pasa a reconciliación o resultado incierto

### Requirement: RQ-09 Índices documentales y electrónicos completos
Origen de diseño: D-09.
El sistema SHALL actualizar `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` con las reglas del gabinete y SHALL verificar independientemente el índice electrónico SQL y el archivo XML.

#### Scenario: Índices completos
- **WHEN** documento, sujeto y expediente están resueltos
- **THEN** los tres índices documentales corresponden a sus valores autoritativos y SQL y XML representan el documento

#### Scenario: SQL correcto y XML ausente
- **WHEN** el índice SQL está confirmado pero el XML falta o está desactualizado
- **THEN** el sistema no confirma la fase de índices ni completa el documento

#### Scenario: Reintento de índices
- **WHEN** la vinculación está confirmada y falla únicamente la indexación
- **THEN** el reintento actualiza o verifica índices sin recrear expediente, documento o relación

### Requirement: RQ-10 Saga persistente y recuperación
Origen de diseño: D-10.
El sistema SHALL persistir el estado de cada efecto confirmado y SHALL continuar un reintento desde la última postcondición demostrada.

#### Scenario: Expediente creado y documento fallido
- **WHEN** el expediente fue verificado pero el almacenamiento no finalizó
- **THEN** el sistema conserva el expediente y reintenta desde el documento

#### Scenario: Documento almacenado y vínculo fallido
- **WHEN** existe un `IdImagen` confirmado pero la relación no se confirmó
- **THEN** el sistema no almacena nuevamente y reanuda desde la vinculación

#### Scenario: Efecto de persistencia desconocido
- **WHEN** una respuesta se pierde y el efecto no puede demostrarse
- **THEN** el sistema marca `ResultadoIncierto` y reconcilia antes de repetir una mutación

#### Scenario: Resultados mixtos
- **WHEN** algunos documentos están reconciliados y otros permanecen fallidos o inciertos
- **THEN** conserva los confirmados y representa la intención como `Parcial`

### Requirement: RQ-11 Migración trazable de funciones legacy
Origen de diseño: D-11.
Cada función legacy identificada SHALL tener una disposición explícita y evidencia de equivalencia antes de que su responsabilidad se considere migrada.

#### Scenario: Responsabilidad migrada directamente
- **WHEN** una consulta o regla determinista se traslada al flujo moderno
- **THEN** existe trazabilidad de entradas, efectos, destino moderno, caracterización y prueba equivalente

#### Scenario: Mutación conservada temporalmente
- **WHEN** una operación física legacy todavía no puede migrarse con seguridad
- **THEN** queda aislada detrás de un contrato moderno con precondición, postcondición, idempotencia y reconciliación

#### Scenario: Función omitida o reemplazada
- **WHEN** una función legacy deja de invocarse en el flujo moderno
- **THEN** se demuestra qué responsabilidad la reemplaza y que ningún efecto requerido fue eliminado

#### Scenario: Reimplementación sin trazabilidad
- **WHEN** una solución duplica SQL o reglas sin correspondencia demostrada con el comportamiento existente
- **THEN** la implementación no satisface el requisito de migración

### Requirement: RQ-12 Evidencia E2E reutilizando DOC-56
Origen de diseño: D-12.
La validación E2E SHALL ampliar los activos DOC-56 existentes y SHALL reportar individualmente las 18 aserciones obligatorias como `passed`, `failed` o `blocked`.

#### Scenario: Cobertura distribuida existente
- **WHEN** se preparan pruebas de ejecución, reintento, recuperación y concurrencia
- **THEN** se reutilizan sus escenarios, runner, adaptador, perfiles, autenticación, gate, controles y reporte existentes

#### Scenario: Infraestructura paralela propuesta
- **WHEN** una aserción nueva requiere información adicional
- **THEN** solo se agregan campos, controles de lectura o verificadores faltantes y no se crea otro runner o escenario monolítico

#### Scenario: Ejecución sin autorización
- **WHEN** no existe autorización explícita para ambiente, cuenta, mutación y recurso descartable
- **THEN** la E2E autenticada o mutadora no se ejecuta y se reporta bloqueada

#### Scenario: Evidencia incompleta
- **WHEN** alguna aserción está fallida o bloqueada
- **THEN** la implementación no se declara cerrada

### Requirement: RQ-13 Contexto autoritativo e inmutable
Origen de diseño: D-13.
El sistema SHALL resolver en servidor y persistir el contexto de tarea, ruta, gabinete, empresa y actor; frontend y sesión no SHALL determinar el expediente.

#### Scenario: Ejecución y reintento
- **WHEN** se ejecuta o reanuda una intención
- **THEN** las decisiones usan el contexto persistido y la sesión únicamente confirma que el actor sigue autorizado

#### Scenario: Dependencia legacy de sesión
- **WHEN** un adaptador transitorio requiere un valor de sesión
- **THEN** el valor se contrasta con el contexto inmutable y una discrepancia bloquea la operación

#### Scenario: Autoridad enviada por cliente
- **WHEN** el cliente envía un expediente, gabinete o destino no autoritativo
- **THEN** el sistema no lo usa para decidir la asignación

### Requirement: RQ-14 Convivencia y gate seguro
Origen de diseño: D-14.
La implementación SHALL ser aditiva y SHALL conservar intacto el recorrido legacy cuando el gate moderno esté apagado.

#### Scenario: Gate apagado
- **WHEN** el gate moderno no aplica al usuario o está en `false`
- **THEN** el recorrido legacy mantiene su comportamiento y no se ejecuta ningún efecto moderno duplicado

#### Scenario: Contratos y estados extendidos
- **WHEN** se agregan estados o datos persistidos
- **THEN** DTO, fixtures, mapeos y documentación se versionan sin romper consumidores existentes

#### Scenario: Restauración del gate en pruebas
- **WHEN** finaliza una corrida autorizada, tanto en éxito como en fallo
- **THEN** el gate queda en `false` y las listas de usuarios y grupos quedan vacías

#### Scenario: Protección de información
- **WHEN** ocurre un error o se conserva evidencia
- **THEN** no se exponen SQL, rutas físicas, cuerpos SII, datos personales innecesarios, cookies, credenciales ni excepciones internas
