# Implementación de creación y vinculación de expedientes SII

## Objetivo

Documentar la exploración arquitectónica del flujo legacy y definir cómo debe incorporarse al flujo moderno de `ImportarServicioWeb` la resolución, creación, reutilización, vinculación, actualización de índices y reconciliación de expedientes SII.

La creación del expediente no se considera completa cuando únicamente existe un registro de expediente. Se considera completa cuando todos los documentos encontrados en el gabinete mediante `ENLASE = radicado SII` están vinculados exactamente al expediente que les corresponde, tienen actualizados `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA`, forman parte del índice electrónico SQL y del XML, y una reconciliación independiente puede demostrar esos efectos.

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

La caché legacy agrava la brecha en ejecuciones posteriores: un registro global por radicado puede impedir que se vuelvan a consultar y vincular documentos añadidos después. El flujo requerido debe ser incremental por documento y conservar el expediente ya resuelto.

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

## Flujo preliminar sustituido

> Este diagrama conserva el razonamiento inicial. Fue sustituido por el flujo consolidado siguiente porque ubicaba el plan de vinculación antes de almacenar los nuevos documentos y no modelaba la caché incremental por `IdImagen`.

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

## Flujo consolidado vigente

El flujo conserva la entrada moderna y agrega coordinación a nivel de intención. La operación obligatoria es resolver los expedientes: reutilizarlos cuando existen o crearlos cuando faltan. Después de almacenar los items seleccionados se consulta nuevamente el universo documental mediante la única relación disponible: `NombreGabinete + ENLASE`.

```text
┌─────────────────────────────────────────────────────────────────┐
│ 1. CONSULTAR Y SELECCIONAR SII                             │
│ Validar contexto y conservar inscripción → imágenes.        │
└────────────────────────────────┬────────────────────────────────┘
                               ▼
┌────────────────────────────────────────────────────────────────┐
│ 2. PREFLIGHT Y CREAR/RECUPERAR INTENCIÓN                 │
│ Validar tarea, trámite, tipologías y claves; persistir lote.│
└────────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ 3. RESOLVER EXPEDIENTES SII — OBLIGATORIO                │
│ Resolver configuración, gabinete, campos únicos y roles.    │
│ Buscar por matrícula normalizada + gabinete.                 │
└────────────────────────────────┬────────────────────────────────┘
                               ▼
                   ┌────────────────────┐
                   │ ¿Existe y es válido?│
                   └─────────┬──────────┘
                       SÍ   │   NO
             ┌───────────┴───────────┐
             ▼                       ▼
      Verificar y reutilizar      Crear expediente(s)
             │                       │
             │                       ▼
             │               Verificar y cachear
             └───────────┬───────────┘
                       ▼
┌───────────────────────────────────────────────────────────────┐
│ 4. PLANIFICAR EXPEDIENTES                                 │
│ Único: todos al mismo. Múltiple: primario/secundarios por│
│ tipologías configuradas. Un solo destino por documento.      │
└────────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ 5. ALMACENAR ITEMS SII SELECCIONADOS                      │
│ Descargar, validar, preparar, almacenar y obtener IdImagen.│
└────────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ 6. CONSULTAR UNIVERSO DOCUMENTAL ACTUAL                    │
│ Única fuente: NombreGabinete WHERE ENLASE = RadicadoSII. │
│ Incluye documentos previos y nuevos; deduplica por IdImagen.│
└────────────────────────────────┬────────────────────────────────┘
                               ▼
┌──────────────────────────────────────────────────────────────┐
│ 7. PROCESAR CADA DOCUMENTO                                │
│ Caché: UNIQUE(IdTarea, IdImagen, NombreGabinete).         │
│ Verificar siempre la relación física con IdExpediente.     │
└────────────────────────────────┬────────────────────────────────┘
                               ▼
         ┌──────────────┼─────────────┐
         ▼             ▼             ▼
      Ausente          Correcta       Conflictiva/duplicada
         │             │             │
         ▼             ▼             ▼
      Vincular       No revincular      Detener item
         │             │
         └──────┼─────┘
                   ▼
┌───────────────────────────────────────────────────────────────┐
│ 8. VERIFICAR, CACHEAR Y ACTUALIZAR ÍNDICES              │
│ Cachear solo relación confirmada. Actualizar NITCEDULA,     │
│ RAZONSOCIAL y MATRICULA. Confirmar índice SQL y XML.      │
└────────────────────────────────┬────────────────────────────────┘
                               ▼
┌───────────────────────────────────────────────────────────────┐
│ 9. RECONCILIAR                                              │
│ Documento + expediente + relación + caché + índices + XML.│
└────────────────────────────────┬────────────────────────────────┘
                               ▼
          Completada / Parcial / ResultadoIncierto
```

### Comportamiento incremental

En cada nueva ejecución se reutilizan los expedientes confirmados y se consulta de nuevo el gabinete por `ENLASE`. Un documento con caché se verifica y no se revincula; un `IdImagen` nuevo se vincula, se cachea y se indexa. Un sello corregido se conserva junto con el anterior y se procesa como documento adicional.

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
 Verificar y         Crear, verificar
 reutilizar          y cachear
       └──────────┬──────────┘
                  ▼
       Almacenar items seleccionados
                  │
                  ▼
       Consultar todos los documentos
       por gabinete + ENLASE
                  │
                  ▼
       Vincular solo relaciones ausentes
       y cachear por tarea + imagen + gabinete
                  │
                  ▼
       Actualizar NITCEDULA, RAZONSOCIAL,
       MATRICULA, índice SQL y XML
                  │
                  ▼
           Reconciliar el conjunto
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

La resolución de expedientes debe ser una operación coordinada a nivel de intención o de inscripción, no un paso aislado por imagen. Antes del almacenamiento se persiste el plan de expedientes; después del almacenamiento se consulta el universo actual por `ENLASE` y se genera el plan físico `IdImagen → expediente`.

Las fases actuales deben corregirse:

- `ExpedientePreparado` debe ejecutar resolución/creación real o cambiar de nombre a `ArchivoTemporalPreparado`.
- `IndicesActualizados` debe ejecutar la actualización real antes de confirmar éxito.
- `CacheActualizado` debe registrar la caché documental por `IdTarea + IdImagen + NombreGabinete` únicamente después de verificar la relación física.
- `Completada` solo debe alcanzarse después de reconciliar documento, expediente, índices, caché e índice XML.

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
10. reconciliación posterior de resultado incierto;
11. segundo intento con el mismo radicado y un nuevo `IdImagen`;
12. sello corregido conservando el sello anterior;
13. caché documental existente con relación física ausente;
14. índice XML ausente o desactualizado impide completar.

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

### Funciones legacy y estrategia de migración

| Responsabilidad | Función legacy | Recomendación |
|---|---|---|
| Configuración del trámite | `SolicitaEstructuraTramite` | Migrar a repositorio moderno |
| Buscar expediente por matrícula | `SolicitaRegistroExpedienteMatricula` y `SolicitaCacheCreacionExpedienteSII` | Migrar; la caché solo localiza y el expediente real debe verificarse |
| Obtener datos SII | `SolicitaEstructuraExpedienteSII` | Reutilizar temporalmente |
| Crear expediente | `AutoRegistraExpedienteTramite` | Adaptador transitorio; migrar progresivamente sus escrituras |
| Coordinar creación única/múltiple | `CreaExpedienteIntegracionSII` | Migrar sus reglas al coordinador; no invocar el ASMX |
| Consultar documentos por `ENLASE` | `SolicitaListaImagenesGabineteEnlace` | Migrar a consulta parametrizada; `ENLASE` es la única pertenencia disponible |
| Planificar expediente único/múltiple | `SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII` y `SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII` | Migrar reglas; no reutilizar directamente |
| Vincular documento | `VinculaDocumentoExpediente` | Adaptador transitorio con precheck y postcheck; migrar progresivamente |
| Registrar caché de creación | `RegistraCacheCreacionExpedienteSII` | Encapsular para compatibilidad; no usar como fuente de verdad |
| Registrar caché de vinculación | `RegistraCahcheVinculacionSII` | Sustituir el control global por caché moderna por tarea, imagen y gabinete |
| Actualizar índices SII | `ActualizaIndiceDocumentosSII`, `ActualizaIndiceDocumentoCacheExpediente` y `ActualizaIndiceDocumentoIntegracionSII` | Migrar; actualizar `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` |
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
matrícula normalizada + gabinete → localización SII del expediente
campos configurados estado_unico=1 → identidad física del expediente
IdTarea + IdImagen + NombreGabinete → caché moderna de vinculación
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
    └── RegistrarCreacion(...)

IImportRelatedDocumentRepository
    └── BuscarPorEnlace(gabinete, radicadoSii)

IImportDocumentLinkCacheRepository
    ├── Buscar(taskId, imageId, gabinete)
    └── RegistrarVerificada(taskId, imageId, gabinete, expedientId, radicadoSii)

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
    ├── workflow_import_intent_item
          ├── inscription_key
          ├── document_id
          ├── expedient_id
          ├── storage_status
          ├── relation_status
          ├── index_status
          └── cache_status
    │
    └── workflow_import_related_document
          ├── task_id
          ├── image_id
          ├── cabinet_name
          ├── sii_radicado
          ├── expected_expedient_id
          ├── relation_status
          ├── index_status
          ├── xml_index_status
          └── reconciliation_status
```

La caché documental usa `UNIQUE(task_id, image_id, cabinet_name)` y conserva además el expediente esperado y el radicado. No sustituye la relación física: una entrada cacheada siempre debe contrastarse con el estado real. El modelo permite que varias imágenes pertenezcan a una inscripción, que varias inscripciones compartan o no un expediente y que una segunda ejecución descubra nuevos `IdImagen`.

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
ItemsSiiAlmacenados
  ↓
UniversoDocumentalConsultado
  ↓
VinculacionesProcesadas
  ↓
IndicesYXmlActualizados
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
1. Consultar SII y conservar inscripciones e imágenes.
2. Validar selección y tipologías y crear o recuperar la intención.
3. Consultar configuración del trámite y campos `estado_unico=1`.
4. Buscar cada expediente por matrícula normalizada y gabinete y verificarlo físicamente.
5. Crear únicamente los expedientes ausentes, incluidos primario y secundarios, verificarlos y registrar la caché de creación.
6. Persistir el plan de expedientes resuelto en la intención.
7. Descargar y almacenar los items SII seleccionados y obtener cada `IdImagen`.
8. Consultar nuevamente todos los documentos del gabinete mediante `ENLASE = radicado SII`.
9. Asignar un expediente único a cada documento según modo único o reglas de tipología para primario/secundarios.
10. Por documento, consultar la caché moderna y verificar la relación física.
11. Vincular únicamente relaciones ausentes; detener relaciones cruzadas o duplicadas.
12. Registrar la caché documental solo después de confirmar la relación correcta.
13. Actualizar `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` según gabinete.
14. Confirmar el índice electrónico SQL y actualizar obligatoriamente el XML.
15. Reconciliar todos los efectos y completar solo cuando todo el universo documental sea consistente.
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
fue descubierto en el gabinete por ENLASE = radicado SII
        AND
expediente relacionado = expediente esperado
        AND
no existe relación con otro expediente
        AND
NITCEDULA, RAZONSOCIAL y MATRICULA corresponden al gabinete
        AND
caché apunta al mismo expediente
        AND
índice electrónico SQL y XML son consistentes
```

La reconciliación y la E2E actuales solo comprueban existencia documental y relación con la tarea; no prueban expediente, índices ni caché.

### Riesgos heredados

- La coordinación está distribuida entre JavaScript y varios ASMX.
- Existen dependencias fuertes de `HttpContext.Session`.
- Creación y vinculación son operaciones separadas.
- No existe atomicidad global.
- La lógica cambia entre expediente único y múltiple.
- Algunas decisiones utilizan exclusivamente `CIncripcionSII(0)`.
- Las tablas legacy no tienen restricciones únicas que garanticen la idempotencia requerida.
- En el caso múltiple, la tipología puede determinar el expediente destino, no solamente la inscripción.
- La actualización del XML no participa en la transacción SQL y exige reconciliación.

### Decisiones funcionales consolidadas

1. La única forma disponible de descubrir los documentos relacionados es `NombreGabinete + ENLASE = radicado SII`; no existe otra relación física demostrable con la tarea.
2. Un sello corregido se conserva junto con el anterior. El nuevo sello obtiene otro `IdImagen` y se procesa como documento adicional.
3. La localización SII del expediente usa matrícula normalizada más gabinete. La identidad física se obtiene dinámicamente de los campos del auto-registro marcados `estado_unico=1`; no existe una lista fija universal.
4. Las tablas legacy no tienen restricciones únicas aplicables a esta idempotencia. La caché moderna debe imponer `UNIQUE(task_id, image_id, cabinet_name)`.
5. Los índices SII obligatorios son `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA`:
   - `MERCANTIL`: matrícula mercantil;
   - `ESAL`: matrícula normalizada sin `S0`;
   - `RUP`: número de proponente.
6. El índice electrónico SQL y el archivo XML forman parte obligatoria de la condición de finalización.
7. El alcance incluye expediente único y expedientes múltiples con primario y secundarios.
8. Cuando el expediente obligatorio no pueda resolverse o crearse, la intención no puede continuar con el almacenamiento ni declararse completada.

### Verificaciones técnicas previas a implementación

1. Caracterizar con pruebas las escrituras de `AutoRegistraExpedienteTramite` y `VinculaDocumentoExpediente`, incluido el XML.
2. Definir la migración SQL de la caché documental con su restricción única moderna.
3. Corregir en el diseño moderno la diferencia legacy entre la matrícula usada al crear y la usada al recuperar expedientes secundarios.
4. Precisar la consulta de reconciliación para índice SQL y archivo XML sin depender del retorno `YES` de la operación legacy.
