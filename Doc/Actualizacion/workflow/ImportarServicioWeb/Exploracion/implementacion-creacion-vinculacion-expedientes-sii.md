# Implementación de creación y vinculación de expedientes SII

## Objetivo

Documentar la exploración arquitectónica del flujo legacy y definir cómo debe incorporarse al flujo moderno de `ImportarServicioWeb` la creación, reutilización, vinculación y reconciliación de expedientes SII.

## Diagnóstico ejecutivo

El flujo legacy contiene una etapa completa de creación y vinculación de expedientes SII que no fue trasladada íntegramente al flujo moderno.

El flujo moderno actual sí:

- consulta las inscripciones e imágenes del SII;
- descarga cada recurso;
- almacena documentos independientes;
- vincula el documento con la tarea Workflow mediante el adaptador legacy;
- mantiene una intención e items con control de estado e idempotencia.

El flujo moderno actual no:

- consulta `util_Estado_Crea_ExpedienteSII`;
- consulta `util_Estado_Multiple_expedienteSII`;
- conserva explícitamente la agrupación completa de inscripciones para coordinar expedientes;
- crea expedientes primarios o secundarios;
- registra la caché de creación de expedientes SII;
- ejecuta explícitamente la vinculación documento-expediente;
- actualiza realmente los índices en la fase `IndicesActualizados`;
- reconcilia la relación documento-expediente.

Por tanto, una ejecución moderna puede finalizar satisfactoriamente y demostrar `documento → tarea`, sin demostrar `documento → expediente SII`.

## Evidencia del flujo moderno actual

### `ExpedientePreparado` no prepara un expediente

`Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb`, clase `PrepareImportExecutionStep`, únicamente:

1. valida que el contenido haya sido descargado;
2. resuelve una extensión confiable;
3. crea una ruta temporal;
4. escribe el contenido en el archivo temporal;
5. libera el arreglo de bytes.

Sin embargo, declara como fase confirmada `FaseImportacionServicio.ExpedientePreparado`.

El nombre correcto de la responsabilidad actual sería `ArchivoTemporalPreparado`. En ese paso no se consulta, crea ni vincula un expediente.

### `IndicesActualizados` es un marcador sin actualización

La clase `PrepareImportIndicesExecutionStep` solamente comprueba que `RutaArchivoPreparado` tenga valor y devuelve éxito. No modifica índices, gabinete, expediente ni base de datos.

No debería confirmar `IndicesActualizados` hasta que exista una operación real y comprobable de actualización.

### La resolución parcial está delegada al almacenamiento legacy

`StoreImportExecutionStep` construye `ComandoAlmacenamientoImportacion` con gabinete, ruta, tarea, tipología y campos SII. Después invoca `IImportDocumentStoragePort.Almacenar`.

`LegacyImportDocumentStorageAdapter` delega en:

```text
ClassAlmacenamiento.AlmacenaDocumentoTareaWorkflow
```

Esa función consulta un expediente vinculante mediante `SolicitaEstructuraExpedienteDocumentoVinculante` y pasa los identificadores encontrados a `Almacenamiento`.

Esta operación puede heredar un expediente existente, pero no sustituye el proceso legacy explícito de creación, caché y vinculación.

## Evidencia del flujo legacy

### Activación por configuración del trámite

`js/workflow/Webworkflow.js`, función `GuardarConstanciaIncripcionSII`, consulta las banderas:

- `util_Estado_Crea_ExpedienteSII`;
- `util_Estado_Multiple_expedienteSII`.

Si la creación está activa, ejecuta el flujo de creación y vinculación. Si no está activa, ejecuta la vinculación con un expediente previamente registrado.

### Decisión existe/no existe

`js/java_general/JSExpediente.js`, función `CreaExpedienteVinculaDocumentoSII`, primero consulta el registro de expediente por matrícula.

```text
¿Existe caché/expediente?
├── No: crea expediente y vincula documentos.
└── Sí: reutiliza el expediente y vincula los documentos pendientes.
```

### Creación efectiva

`Gestion/ClassGaExpediente.vb`, función `CreaExpedienteIntegracionSII`, recibe una lista completa de `CIncripcionSII` y llama `AutoRegistraExpedienteTramite`.

La función obtiene desde SII:

- matrícula;
- proponente;
- identificación;
- razón social;
- propietario;
- radicado;
- código de barras.

### Expediente único y múltiples expedientes

Si `util_Estado_Multiple_expedienteSII = 1`, recorre todas las inscripciones y puede crear un expediente primario y expedientes secundarios. La distribución de documentos utiliza las tipologías configuradas para expedientes secundarios.

Si la bandera no está activa, crea o reutiliza un único expediente para el conjunto aplicable.

### Caché e idempotencia funcional

Después de crear un expediente, el legacy ejecuta `RegistraCacheCreacionExpedienteSII` con:

- código de barras;
- radicado SII;
- matrícula;
- identificación;
- razón social;
- ID del expediente;
- gabinete;
- condición primario/secundario.

### Vinculación de documentos

El legacy construye elementos `ClsssStructureVinculaDocumento` con:

- gabinete;
- ID de expediente;
- ID de imagen/documento;
- ID de tarea;
- radicado.

Posteriormente ejecuta la vinculación y registra su caché.

## Brechas funcionales

| Capacidad legacy | Estado moderno actual |
|---|---|
| Leer configuración de creación | No implementado |
| Leer configuración de expedientes múltiples | No implementado |
| Preservar inscripciones como agregado | Parcial; se aplanan a imágenes |
| Buscar expediente/caché antes de crear | No implementado |
| Crear expediente | No implementado |
| Crear primario y secundarios | No implementado |
| Registrar caché de creación | No implementado |
| Vincular explícitamente documento-expediente | No implementado en el orquestador moderno |
| Actualizar índices SII | Fase presente, lógica ausente |
| Reconciliar documento-expediente | No implementado |

## Flujo propuesto

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│ CLIENTE / FRONTEND                                                           │
│                                                                              │
│ QueryItems({ taskId, providerId, codigoBarras })                             │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ API / ASMX MODERNO                                                           │
│                                                                              │
│ • valida sesión y contexto                                                   │
│ • obtiene usuario, grupo, ruta, trámite y tarea desde servidor               │
│ • valida tarea activa/asignada                                               │
│ • valida proveedor INTEGRACIONSII                                            │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                      ┌─────────────┴─────────────┐
                      ▼                           ▼
               Contexto válido              Contexto inválido
                      │                           │
                      ▼                           ▼
                Consultar SII                  Rechazar
                      │
                      ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ CONSULTA Y NORMALIZACIÓN SII                                                 │
│                                                                              │
│ • solicita token en servidor                                                 │
│ • ejecuta consultarInformacionSello                                          │
│ • conserva inscripciones y documentos                                        │
│ • crea una ExternalKey por imagen                                            │
│ • conserva matrícula, proponente, libro, registro y propietario              │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ FRONTEND                                                                     │
│                                                                              │
│ Muestra los documentos y permite seleccionar uno, varios o todos.            │
│ Envía los seleccionados juntos mediante Items[].                             │
│ El frontend no decide permisos, gabinete ni expediente.                      │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ PREFLIGHT                                                                    │
│                                                                              │
│ • valida items y claves únicas                                               │
│ • valida tarea destino                                                       │
│ • resuelve tipología documental                                              │
│ • valida tipología contra trámite                                            │
│ • genera ContextFingerprint                                                  │
│ • no produce efectos persistentes                                            │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ CREAR INTENCIÓN                                                              │
│                                                                              │
│ • una cabecera de intención                                                  │
│ • un item por documento                                                      │
│ • conserva agrupación por inscripción                                        │
│ • huella idempotente del lote                                                │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ COORDINADOR DE EXPEDIENTES SII                                               │
│                                                                              │
│ Obtiene en backend:                                                          │
│ • trámite, ruta y tarea                                                      │
│ • gabinete                                                                   │
│ • util_Estado_Crea_ExpedienteSII                                             │
│ • util_Estado_Multiple_expedienteSII                                         │
│ • reglas de expedientes secundarios                                          │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
                  ┌─────────────────────────────────┐
                  │ ¿El trámite crea expediente SII?│
                  └────────────────┬────────────────┘
                              SÍ   │   NO
                   ┌───────────────┴────────────────┐
                   ▼                                ▼
       Buscar expediente/caché             Buscar expediente existente
                   │                                │
                   ▼                                ▼
          ┌─────────────────┐              ┌────────────────────┐
          │ ¿Existe?        │              │ ¿Existe?           │
          └────────┬────────┘              └─────────┬──────────┘
              SÍ   │   NO                       SÍ   │   NO
          ┌────────┴──────────┐              ┌───────┴──────────┐
          ▼                   ▼              ▼                  ▼
       Reutilizar       Extraer datos SII  Reutilizar      Detener o aplicar
       expediente              │            expediente      regla contractual
          │                    ▼              │
          │        AutoRegistraExpediente     │
          │                    │              │
          │                    ▼              │
          │          Registrar caché SII      │
          └────────────────────┴──────────────┘
                               │
                               ▼
             ┌─────────────────────────────────────┐
             │ ¿Maneja múltiples expedientes?      │
             └──────────────────┬──────────────────┘
                           SÍ   │   NO
              ┌────────────────┴─────────────────┐
              ▼                                  ▼
   Agrupar inscripciones por            Asignar todos los documentos
   matrícula/propietario/tipología      al expediente único
              │                                  │
              ▼                                  │
   Crear/reutilizar primario y                    │
   secundarios                                    │
              └────────────────┬─────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ PLAN DE VINCULACIÓN                                                         │
│                                                                              │
│ Por documento determina:                                                     │
│ • ExternalKey y ClientItemId                                                 │
│ • inscripción origen                                                        │
│ • tipología                                                                  │
│ • ID de expediente destino                                                   │
│ • gabinete, tarea y radicado                                                 │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ EJECUCIÓN POR DOCUMENTO                                                      │
│                                                                              │
│ 1. Descargar recurso.                                                        │
│ 2. Validar formato, host, tamaño y contenido.                                │
│ 3. Preparar archivo temporal.                                                │
│ 4. Resolver campos y tipología.                                              │
│ 5. Almacenar documento.                                                      │
│ 6. Obtener DocumentId.                                                       │
│ 7. Vincular DocumentId con expediente y tarea.                               │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ ACTUALIZACIÓN REAL DE ÍNDICES                                                │
│                                                                              │
│ Actualiza matrícula/proponente, identificación, razón social, libro,         │
│ inscripción, fecha, acto, expediente y campos específicos del gabinete.      │
│ Solo entonces confirma IndicesActualizados.                                  │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ CACHÉS SII                                                                   │
│                                                                              │
│ • caché de creación de expediente                                            │
│ • caché de vinculación                                                       │
│ • caché de inscripción/radicado                                              │
│ • operación idempotente                                                      │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                                    ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ RECONCILIACIÓN                                                               │
│                                                                              │
│ Verifica por item:                                                           │
│ • exactamente un documento                                                   │
│ • tarea correcta                                                             │
│ • expediente esperado                                                        │
│ • ausencia de relaciones duplicadas o cruzadas                               │
│ • índices SII                                                                │
│ • caché apuntando al mismo expediente                                        │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │
                     ┌──────────────┼──────────────┐
                     ▼              ▼              ▼
                Completada        Parcial      ResultadoIncierto
```

## Regla central

```text
Datos enviados o inferidos por frontend
                │
                ▼
No constituyen autoridad ni determinan expediente
                │
                ▼
Backend resuelve configuración del trámite y datos SII
                │
                ▼
Busca expediente de forma idempotente
                │
       ┌────────┴────────┐
       ▼                 ▼
    Existe            No existe
       │                 │
       ▼                 ▼
   Reutilizar       ¿Creación autorizada?
                         │
                  ┌──────┴──────┐
                  ▼             ▼
                 Sí             No
                  │             │
                  ▼             ▼
           Crear y cachear   Detener/aplicar regla
                  │
                  ▼
           Vincular documentos
                  │
                  ▼
           Reconciliar relación
```

## Estados y códigos recomendados

```text
Solicitud inválida
    └── INVALID_REQUEST

Sesión o tarea no autorizada
    └── FORBIDDEN / SESSION_CONTEXT_UNAVAILABLE

Sin inscripciones o imágenes SII
    └── SII_INSCRIPTIONS_EMPTY / SII_IMAGES_EMPTY

Tipología no permitida
    └── DOCUMENT_TYPE_NOT_ALLOWED_FOR_PROCEDURE

Configuración de expediente ausente
    └── EXPEDIENT_CONFIGURATION_UNAVAILABLE

Expediente existente
    └── EXPEDIENT_REUSED

Expediente creado
    └── EXPEDIENT_CREATED

Expediente requerido, pero no disponible
    └── EXPEDIENT_REQUIRED

Documento almacenado y vinculado
    └── AVAILABLE

Relación ausente, duplicada o cruzada
    └── EXPEDIENT_RELATION_MISSING
    └── EXPEDIENT_RELATION_DUPLICATE
    └── EXPEDIENT_RELATION_CONFLICT

Fallo anterior a persistencia
    └── FAILED_BEFORE_PERSISTENCE

Persistencia desconocida
    └── RESULTADO_INCIERTO

Lote reconciliado completamente
    └── COMPLETADA

Lote con resultados mixtos
    └── PARCIAL
```

## Recomendación arquitectónica

La creación de expedientes debe ser una operación coordinada a nivel de intención o de inscripción, no un paso aislado por imagen. El modelo debe preservar la relación `inscripción → documentos` y generar antes del almacenamiento un plan explícito `documento → expediente`.

Las fases actuales deben corregirse:

- `ExpedientePreparado` debe ejecutar resolución/creación real o cambiar de nombre a `ArchivoTemporalPreparado`.
- `IndicesActualizados` debe ejecutar la actualización real antes de confirmar éxito.
- `CacheActualizado` debe registrar y verificar las cachés SII antes de confirmar éxito.
- `Completada` solo debe alcanzarse después de reconciliar documento, tarea, expediente, índices y caché.

## Cobertura E2E requerida

La E2E debe incluir al menos:

1. expediente existente reutilizado;
2. expediente único creado;
3. expediente primario y secundarios creados;
4. varios documentos vinculados en una sola intención;
5. ejecución repetida sin duplicar expediente ni relación;
6. relación ausente detectada;
7. relación duplicada detectada;
8. documento relacionado con otro expediente detectado;
9. fallo después de crear expediente y antes de almacenar documento;
10. reconciliación posterior de resultado incierto.

## Diagnóstico técnico de posible implementación

La implementación es viable reutilizando el núcleo legacy, pero no debe incorporarse como otro `IImportExecutionStep` ejecutado ciegamente por documento. La creación del expediente es una operación agregada sobre la intención y sus inscripciones.

### Forma real de la orquestación legacy

El flujo legacy no ejecuta una única operación atómica. Implementa una saga coordinada desde JavaScript:

```text
Consultar configuración
        ↓
Buscar expediente/caché
        ↓
Crear o reutilizar expediente
        ↓
Vincular documentos existentes
        ↓
Actualizar índices
        ↓
Guardar constancias
        ↓
Registrar caché de inscripción
```

El flujo moderno actual está organizado por imagen:

```text
descargar → preparar archivo → marcador índices
→ almacenar → marcador caché → completar
```

Por esta diferencia no es suficiente completar los pasos vacíos: se necesita coordinación a nivel de intención.

### Componentes legacy reutilizables

| Responsabilidad | Función legacy | Recomendación |
|---|---|---|
| Configuración del trámite | `SolicitaEstructuraTramite` | Encapsular en repositorio moderno |
| Buscar expediente por matrícula | `SolicitaRegistroExpedienteMatricula` y caché | Reutilizar mediante puerto |
| Obtener datos SII | `SolicitaEstructuraExpedienteSII` | Reutilizar temporalmente |
| Crear expediente | `AutoRegistraExpedienteTramite` | Encapsular en adaptador |
| Coordinar creación única/múltiple | `CreaExpedienteIntegracionSII` | Reutilizar la lógica, no el ASMX por HTTP |
| Vincular documento | `VinculaDocumentoExpediente` | Encapsular como relación idempotente |
| Registrar caché de creación | `RegistraCacheCreacionExpedienteSII` | Encapsular en repositorio |
| Registrar caché de vinculación | `RegistraCahcheVinculacionSII` | Encapsular en repositorio |
| Actualizar índices | `ActualizaIndiceDocumentoCacheExpediente` | Ejecutar realmente |
| Resolver expediente durante almacenamiento | `SolicitaEstructuraExpedienteDocumentoVinculante` | Mantener como compatibilidad |

El flujo moderno no debe llamar los ASMX legacy mediante HTTP interno. Debe utilizar adaptadores que encapsulen directamente las clases existentes.

### Problemas para una integración directa

#### Dependencia de sesión HTTP

El legacy consume valores globales de sesión como:

```text
ID_TAREA_SELECCIONDA
WF_RUTAWORKFLOW
Id_Ruta_Workflow
GA_IDEMPRESA
GA_IDUSUARIOGESTION
Login_Usuario_Workfow
```

Incluso cuando una función recibe `IdTareaWorkflow`, algunas ramas vuelven a consultar `ID_TAREA_SELECCIONDA`. La lógica moderna no puede depender de que el estado global de sesión coincida accidentalmente con la intención persistida.

La integración debe pasar un contexto explícito e inmutable o encapsular cuidadosamente las dependencias de sesión durante la transición.

#### Pérdida de la agregación funcional

El mapper moderno aplana:

```text
inscripciones[] → imagenes[] → ExternalItemDto[]
```

Esta estructura es útil para selección, pero no conserva formalmente:

```text
Inscripción
├── matrícula
├── proponente
├── propietario
├── condición primaria/secundaria
└── documentos[]
```

La creación de expedientes múltiples necesita este agregado. No debe reconstruirse únicamente desde `ExternalKey`.

#### Persistencia insuficiente de la intención

El item moderno conserva documento, tarea y estado, pero no persiste:

- `inscription_key`;
- `expedient_id`;
- `expedient_role`;
- `expedient_status`;
- `relation_status`;
- `index_status`;
- `cache_status`.

Sin estos datos no puede retomarse de manera segura una ejecución parcialmente completada.

#### Idempotencia incompleta

La intención protege el lote, pero no protege directamente estas identidades:

```text
matrícula + gabinete + trámite → expediente
documento + expediente → relación
radicado + expediente → caché
```

Todo reintento debe consultar primero el efecto real. Una respuesta perdida nunca debe provocar un segundo expediente o una segunda relación.

#### Ausencia de transacción global

La creación, el almacenamiento físico, la vinculación, los índices y las cachés afectan componentes diferentes. No es viable cubrir toda la operación con una sola transacción SQL.

Debe utilizarse una saga persistente:

```text
efecto confirmado
       ↓
estado persistido
       ↓
el reintento continúa desde el último efecto conocido
```

### Diseño recomendado

#### Coordinador de expedientes por intención

Agregar un servicio conceptual `ImportExpedientCoordinator` con estas responsabilidades:

1. cargar todas las inscripciones seleccionadas;
2. consultar la configuración del trámite;
3. determinar expediente único o múltiple;
4. buscar expedientes y cachés existentes;
5. crear únicamente los expedientes faltantes;
6. persistir el plan de asignación;
7. entregar a cada item su `ExpedientId`.

Este coordinador no debe descargar ni almacenar archivos.

#### Puertos propuestos

```text
IImportExpedientConfigurationRepository
    └── ResolverConfiguracion(contexto)

ISiiExpedientSubjectResolver
    └── ResolverDatos(inscripcion, gabinete)

IImportExpedientRepository
    ├── Buscar(...)
    ├── Crear(...)
    └── Obtener(...)

IImportDocumentExpedientRelationPort
    ├── BuscarRelacion(documentId)
    └── Vincular(documentId, expedientId, taskId)

IImportExpedientCacheRepository
    ├── BuscarCreacion(...)
    ├── RegistrarCreacion(...)
    ├── BuscarVinculacion(...)
    └── RegistrarVinculacion(...)

IImportDocumentIndexUpdater
    └── Actualizar(documentId, metadatosSii, expediente)
```

Las primeras implementaciones pueden delegar en clases legacy, manteniendo los puertos libres de `HttpContext` y ASMX.

### Modelo persistente recomendado

```text
workflow_import_intent
    │
    ├── workflow_import_inscription
    │     ├── inscription_key
    │     ├── libro / registro
    │     ├── matricula / proponente
    │     ├── expedient_id
    │     ├── expedient_role
    │     ├── expedient_status
    │     └── cache_status
    │
    └── workflow_import_intent_item
          ├── inscription_key
          ├── document_id
          ├── expedient_id
          ├── storage_status
          ├── relation_status
          ├── index_status
          └── cache_status
```

Esto permite que varias imágenes pertenezcan a una inscripción y que varias inscripciones compartan o no un expediente.

### Máquina de estados propuesta

Las fases modernas deberían redefinirse de la siguiente manera:

```text
Creada
  ↓
Validada
  ↓
ExpedientesPlanificados
  ↓
ExpedientesResueltos
  ↓
RecursoObtenido
  ↓
ArchivoTemporalPreparado
  ↓
DocumentoAlmacenado
  ↓
DocumentoVinculado
  ↓
IndicesActualizados
  ↓
CacheActualizado
  ↓
Reconciliada
  ↓
Completada
```

Ejemplo de intención con varios expedientes:

```text
Intención
   ├── Inscripción A → expediente primario
   │      ├── documento 1
   │      └── documento 2
   │
   └── Inscripción B → expediente secundario
          └── documento 3
```

### Orden de ejecución recomendado

`AutoRegistraExpedienteTramite` busca documentos relacionados con el radicado antes de crear el expediente. Esta dependencia debe validarse antes de alterar el orden legacy.

La secuencia de compatibilidad propuesta es:

```text
1. Consultar SII y conservar inscripciones.
2. Validar selección y tipologías.
3. Consultar configuración del trámite.
4. Buscar expediente y caché.
5. Crear el expediente si corresponde y está ausente.
6. Persistir el expediente resuelto en la intención.
7. Descargar y almacenar cada documento.
8. Vincular cada DocumentId al expediente planificado.
9. Actualizar índices.
10. Registrar caché de vinculación e inscripción.
11. Reconciliar todos los efectos.
```

Si la creación legacy requiere documentos previos del trámite, el preflight debe comprobar esa condición antes de iniciar la saga.

### Política de fallos y recuperación

| Punto de fallo | Tratamiento recomendado |
|---|---|
| Antes de crear expediente | Reintentable sin efectos |
| Expediente creado, pero respuesta perdida | Buscar por caché o identidad antes de crear nuevamente |
| Expediente creado y documento no almacenado | Conservar expediente y reintentar documento |
| Documento almacenado y vinculación fallida | No almacenar nuevamente; reintentar vinculación |
| Vinculación realizada e índices fallidos | Reintentar únicamente índices |
| Registro de caché fallido | Reintentar caché sin recrear expediente |
| Persistencia desconocida | `ResultadoIncierto` y reconciliación obligatoria |

### Reconciliación requerida

Para cada documento debe comprobarse:

```text
DocumentId existe exactamente una vez
        AND
tarea relacionada = tarea esperada
        AND
expediente relacionado = expediente esperado
        AND
no existe relación con otro expediente
        AND
índices corresponden a la inscripción
        AND
caché apunta al mismo expediente
```

La reconciliación y la E2E actuales solo comprueban existencia documental y relación con la tarea; no prueban expediente, índices ni caché.

### Riesgos heredados

- La coordinación está distribuida entre JavaScript y varios ASMX.
- Existen dependencias fuertes de `HttpContext.Session`.
- Creación y vinculación son operaciones separadas.
- No existe atomicidad global.
- La lógica cambia entre expediente único y múltiple.
- Algunas decisiones utilizan exclusivamente `CIncripcionSII(0)`.
- La caché actúa como control de repetición, pero se deben verificar sus restricciones únicas reales.
- En el caso múltiple, la tipología puede determinar el expediente destino, no solamente la inscripción.

### Decisiones pendientes antes del prompt definitivo

1. Identificar todas las tablas y escrituras ejecutadas por `AutoRegistraExpedienteTramite`, `VinculaDocumentoExpediente` y las cachés.
2. Confirmar las restricciones únicas existentes para expediente, caché y relación.
3. Determinar si la importación debe fallar cuando el trámite exige expediente y este no puede crearse.
4. Confirmar si los sellos importados también deben vincularse al expediente o si el legacy únicamente vincula documentos previos del trámite.

Estas comprobaciones son necesarias para producir un prompt de implementación cerrado que no introduzca duplicados, estados falsamente completados o dependencias accidentales de sesión.
