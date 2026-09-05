# Radiografía del backend actual de ImportarServicioWeb

## 1. Propósito y alcance

Representar, con nombres y relaciones observados en el repositorio, el funcionamiento backend actual de **Cargar desde servicio** para `INTEGRACIONSII`.

Esta radiografía describe el sistema existente; no representa la arquitectura objetivo. Incluye al navegador porque actualmente actúa como orquestador de varios endpoints backend independientes.

No se ejecutaron servicios, escrituras ni E2E. Los diagramas provienen de análisis estático. Cuando una consecuencia depende de datos, configuración o procedimientos no inspeccionados en ejecución, se marca como no demostrada.

Cada diagrama también está disponible como archivo Mermaid independiente en `DiagramasBackendActual/`.

## 2. Convenciones

| Notación | Significado |
|---|---|
| Flecha continua | Invocación observada |
| Flecha de retorno | Resultado consumido por el llamador |
| `YES` | Convención textual de éxito existente |
| Sesión | Lectura o escritura de `HttpContext.Current.Session` |
| Caché SII | Tablas auxiliares de inscripción, expediente, vínculo o índices |
| No demostrado | No puede confirmarse únicamente con el recorrido estático |

## 3. Diagrama de clases y dependencias actuales

```mermaid
classDiagram
    direction LR

    class WebworkflowJS {
      +ActivaServicioIntegracionAdjuntaDocumentosSistemasExternos()
      +ActivaGuardarMultiplexConstanciasInscription()
      +ActivaGuardarContasnciaInscripcion(element)
      +GuardarConstanciaIncripcionSII(tipo)
      +ServiceRESTactualizaIndiceDocumentosSII(...)
      +ServiceRESTGuardaConstanciaInscripcionSII(...)
      +ServiceRESTregistraCacheInscripcionRadicadoSII(...)
    }

    class JSexpdiente {
      +LoadJServiceExpediente()
      +VinculaDocumentosExpedienteSII(...)
      +CreaExpedienteVinculaDocumentoSII(...)
    }

    class JSProgresBar {
      +LoadJSProgresBar()
      +ServiceGuardaSelloSII
      +ServiceVinculaDocumentoSII
      +ServiceRegistraExpeidenteSIIVincula
    }

    class WebServiceAdjuntaDocumentoServicioIntegracion {
      +ServiceAdjuntaDocumentoServicioIntegracion(parameter)
      +ServiceAdjuntaDocumentoServicioIntegracionEnlace(parameter)
    }

    class ClassAdjuntaDocumentoServicioIntegracion {
      +ActivaAdjuntaDocumentoServicioIntegracion(tarea, ruta, ...)
    }

    class Class_ra_ser_servicioIntegracion {
      +SolicitaEstructuraServicioIntegracion(id, estructura)
      +SolicitaDatosCamposIndiceGabineteIntegracion(...)
    }

    class WebService_integracion_sii {
      +ServiceSolicitaListaConstanciaInscripcionSII(codigo)
      +ServiceSolicitaEstructuraCacheInscripcionRadicado(recibo)
      +ServiceActualizaIndiceDocumentosSII(inscripciones, tramite, recibo)
      +SeviceGuardaConstanciaInscripcionSII(inscripcion, tipologia, tramite)
      +ServiceRegistraCacheInscripcionRadicadoSII(inscripciones, tramite)
      +ServiceSolicitaRegistroExpedienteMatricula(inscripciones, tramite)
      +ServiceSolicitaCahcheVinculacionSII(inscripciones)
      +ServiceRegistraCahcheVinculacionSII(cache)
      +ServiceActualizaEstadoVinculacionDocumentoSII(expediente)
    }

    class WebServiceGaExpediente {
      +ServiceCreaExpedienteIntegracionSII(tramite, inscripciones)
      +ServiceSolicitaDocumentosVinculacionUnicoExpedienteSII(...)
      +ServiceSolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII(...)
      +ServiceVinculaDocumentoExpediente(documento)
    }

    class Class_consultarInformacionSello {
      +SolicitaListaConstanciasIncripcionSII(...)
      +Lista_inscripciones_radicado_sii(...)
      +ConSultarInformacionSello(radicado, respuesta)
      +ConsolidaListaConstanciasIscripcionesSII(...)
    }

    class Class_ClassResfull {
      +Solicitar_token_general(...)
      +GetResponse(...)
      +GetResponse_GET(...)
      +GetResponse_POST(...)
      -GetResponseAsync(...)
      +GetResponse_POST_Async(...)
    }

    class ClassGaExpediente {
      +CreaExpedienteIntegracionSII(...)
      +VinculaDocumentoExpediente(...)
      +SolicitaEstructuraExpedienteDocumentoVinculante(...)
    }

    class ClassRaSIiCacheExpediente {
      +SolicitaRegistroExpedienteMatricula(...)
      +RegistraCacheCreacionExpedienteSII(...)
      +SolicitaCacheCreacionExpedienteSII(...)
      +ActualizaEstadoVinculacionDocumentoSII(...)
    }

    class ClassRaSiiCacheVinculacion {
      +SolicitaCahcheVinculacionSII(...)
      +RegistraCahcheVinculacionSII(...)
    }

    class ClassRaSIICacheActualizaIndice {
      +ActualizaIndiceDocumentosSII(...)
      +InsertarCacheIndiceSII(...)
      +SolicitaCacheIndiceSIIRadicado(...)
    }

    class ClassRaSiiCahcheInscripcion {
      +SolicitaEstructuraCacheInscripcionRadicado(...)
      +RegistraCacheInscripcionRadicadoSII(...)
    }

    class ClassAlmacenamiento {
      +PreAlmacenaConstanciaIsncripcionsSII(...)
      +AlmacenaDocumentoTareaWorkflow(...)
    }

    class Class_ItexShare {
      +ItexConstanciaIsncripcionSII(...)
    }

    class SessionASPNet {
      +ID_TAREA_SELECCIONDA
      +WF_RUTAWORKFLOW
      +Id_Ruta_Workflow
      +DG_LISTA_CHEQUEO
      +GA_IDUSUARIOGESTION
      +ADJUNTAR_IMAGENES_PREDETERMINADA
    }

    class ServicioSII {
      +solicitarToken
      +consultarInformacionSello
      +recursoConstancia
    }

    WebworkflowJS --> WebServiceAdjuntaDocumentoServicioIntegracion
    WebworkflowJS --> WebService_integracion_sii
    WebworkflowJS --> JSexpdiente
    WebworkflowJS --> JSProgresBar
    JSexpdiente --> WebServiceGaExpediente
    JSexpdiente --> WebService_integracion_sii
    JSexpdiente --> JSProgresBar
    JSProgresBar --> WebServiceGaExpediente
    JSProgresBar --> WebService_integracion_sii

    WebServiceAdjuntaDocumentoServicioIntegracion --> ClassAdjuntaDocumentoServicioIntegracion
    ClassAdjuntaDocumentoServicioIntegracion --> Class_ra_ser_servicioIntegracion
    WebServiceAdjuntaDocumentoServicioIntegracion --> SessionASPNet
    ClassAdjuntaDocumentoServicioIntegracion --> SessionASPNet

    WebService_integracion_sii --> Class_consultarInformacionSello
    Class_consultarInformacionSello --> Class_ClassResfull
    Class_ClassResfull --> ServicioSII
    WebService_integracion_sii --> ClassRaSIICacheActualizaIndice
    WebService_integracion_sii --> ClassRaSiiCahcheInscripcion
    WebService_integracion_sii --> ClassRaSIiCacheExpediente
    WebService_integracion_sii --> ClassRaSiiCacheVinculacion
    WebService_integracion_sii --> ClassAlmacenamiento
    WebService_integracion_sii --> SessionASPNet

    WebServiceGaExpediente --> ClassGaExpediente
    WebServiceGaExpediente --> SessionASPNet
    ClassGaExpediente --> ClassRaSIiCacheExpediente
    ClassAlmacenamiento --> Class_ItexShare
    ClassAlmacenamiento --> ClassGaExpediente
    ClassAlmacenamiento --> ClassRaSiiCahcheInscripcion
    ClassAlmacenamiento --> SessionASPNet
```

### Lectura arquitectónica

- El navegador es el orquestador de la transacción funcional.
- Hay dos superficies ASMX principales: integración SII y expedientes.
- La tarea, ruta, tipología y usuario se recuperan repetidamente desde sesión.
- El transporte externo, las reglas SII y el almacenamiento comparten dependencias concretas.
- `ClassAlmacenamiento` funciona como infraestructura común, pero contiene una rama específica para el caso `SII`.
- No existe una clase servidor que represente una intención completa de importación.

## 4. Diagrama de casos de uso actuales

Mermaid no posee una notación nativa de casos de uso; se representa mediante actores y capacidades observadas.

```mermaid
flowchart LR
    U[Usuario Workflow]
    S[Sesión ASP.NET]
    P[Proveedor externo SII]
    DB[(Base de datos)]
    FS[(Almacenamiento documental)]

    subgraph ISW[Importar desde servicio - comportamiento actual]
      UC1([Resolver servicio configurado])
      UC2([Consultar inscripciones])
      UC3([Visualizar o descargar constancia])
      UC4([Consultar caché del radicado])
      UC5([Crear expediente])
      UC6([Vincular documentos al expediente])
      UC7([Actualizar índices])
      UC8([Guardar una constancia])
      UC9([Guardar todas las constancias])
      UC10([Registrar caché SII])
      UC11([Agregar documento a la lista de la tarea])
    end

    U --> UC1
    U --> UC2
    U --> UC3
    U --> UC8
    U --> UC9

    UC1 --> S
    UC2 --> S
    UC2 --> P
    UC3 --> P
    UC8 --> UC4
    UC9 --> UC4
    UC8 --> UC5
    UC8 --> UC6
    UC8 --> UC7
    UC8 --> FS
    UC9 --> UC5
    UC9 --> UC6
    UC9 --> UC7
    UC9 --> FS
    UC9 --> UC10
    UC5 --> DB
    UC6 --> DB
    UC7 --> DB
    UC10 --> DB
    FS --> UC11
```

### Condiciones actuales por caso de uso

| Caso | Condición observada |
|---|---|
| Resolver servicio | Tarea seleccionada, permiso, ruta, trámite y servicio activo |
| Consultar | Proveedor igual a `INTEGRACIONSII` en el despacho cliente |
| Guardar individual | El caché del radicado debe existir; de lo contrario el cliente detiene |
| Guardar todas | Recupera la colección completa y puede registrar el caché al final |
| Crear/vincular expediente | Depende de `util_Estado_Crea_ExpedienteSII` y cachés previos |
| Actualizar índices | Se realiza antes de almacenar la constancia |
| Agregar a la lista | Se construye desde `dato_lista` después de un guardado exitoso |

## 5. Secuencia actual de consulta

```mermaid
sequenceDiagram
    autonumber
    actor U as Usuario
    participant JS as Webworkflow.js
    participant WSA as WebServiceAdjuntaDocumentoServicioIntegracion
    participant CFG as ClassAdjuntaDocumentoServicioIntegracion
    participant CAT as Class_ra_ser_servicioIntegracion
    participant WSS as WebService_integracion_sii
    participant CIS as Class_consultarInformacionSello
    participant HTTP as Class_ClassResfull
    participant EXT as Servicio SII
    participant SES as Session ASP.NET

    U->>JS: Cargar desde servicio
    JS->>WSA: ServiceAdjuntaDocumentoServicioIntegracion(0)
    WSA->>SES: leer ID_TAREA_SELECCIONDA, ruta y permiso
    WSA->>CFG: ActivaAdjuntaDocumentoServicioIntegracion(...)
    CFG->>SES: validar ADJUNTAR_IMAGENES_PREDETERMINADA
    CFG->>CAT: SolicitaEstructuraServicioIntegracion(id)
    CAT-->>CFG: NombreServicio e id
    CFG-->>WSA: YES + CTipoDocEntrante
    WSA-->>JS: NameService, IdServicioIntegracion, configuración

    alt NameService = INTEGRACIONSII
      JS->>WSS: ServiceSolicitaListaConstanciaInscripcionSII("")
      WSS->>CIS: SolicitaListaConstanciasIncripcionSII(...)
      CIS->>SES: leer tarea y permiso
      CIS->>CIS: obtener código de barras y radicado de la tarea
      CIS->>HTTP: Solicitar_token_general(...)
      HTTP->>EXT: POST solicitarToken (síncrono)
      EXT-->>HTTP: token o error
      HTTP-->>CIS: token deserializado
      CIS->>HTTP: GetResponse(... consultarInformacionSello)
      HTTP->>EXT: POST consultarInformacionSello (síncrono)
      EXT-->>HTTP: inscripciones o error
      HTTP-->>CIS: respuesta textual
      CIS->>CIS: deserializar, sanear parcialmente y consolidar filas
      CIS-->>WSS: columnas y filas Bootstrap
      WSS-->>JS: Error_result + field_table_boot + row_table_boot
      JS-->>U: mostrar modal y tabla
    else servicio vacío
      JS-->>U: advertencia de configuración
    else proveedor distinto
      Note over JS: No se observó otro adaptador en este despacho
    end
```

## 6. Secuencia actual de guardado individual

```mermaid
sequenceDiagram
    autonumber
    actor U as Usuario
    participant JS as Webworkflow.js
    participant WSS as WebService_integracion_sii
    participant JSE as JSExpediente.js
    participant WSE as WebServiceGaExpediente
    participant EXP as ClassGaExpediente
    participant IDX as ClassRaSIICacheActualizaIndice
    participant ALM as ClassAlmacenamiento
    participant PDF as Class_ItexShare
    participant DB as Base de datos y cachés
    participant FS as Almacenamiento documental
    participant SES as Session ASP.NET

    U->>JS: Importar una inscripción
    JS->>WSS: ServiceSolicitaEstructuraCacheInscripcionRadicado(radicado)
    WSS->>DB: SELECT caché por RadicadoSII
    DB-->>WSS: caché o vacío

    alt caché inexistente
      WSS-->>JS: YES + CahcheInscripcion = null
      JS-->>U: recomendar Guardar todas
      Note over JS,U: El recorrido individual termina sin guardar
    else caché existente
      JS-->>U: solicitar tipología
      U->>JS: confirmar tipología
      JS->>JSE: JSExpdiente(opción, colección de 1)

      alt configuración crea expediente
        JSE->>WSS: ServiceSolicitaRegistroExpedienteMatricula(...)
        WSS->>DB: consultar caché de expediente
        alt expediente no registrado en caché
          JSE->>WSE: ServiceCreaExpedienteIntegracionSII(...)
          WSE->>SES: leer tarea y ruta
          WSE->>EXP: CreaExpedienteIntegracionSII(...)
          EXP->>DB: crear expediente y preparar vínculos
          DB-->>EXP: resultado
          EXP-->>WSE: ClassExpedienteVincula
          WSE-->>JSE: documentos por vincular
        else expediente conocido
          JSE->>WSS: ServiceSolicitaCahcheVinculacionSII(...)
          WSS->>DB: consultar caché de vínculo
        end
      else solo vinculación
        JSE->>WSS: ServiceSolicitaRegistroExpedienteMatricula(...)
        WSS->>DB: consultar caché de expediente
        JSE->>WSS: ServiceSolicitaCahcheVinculacionSII(...)
        WSS->>DB: consultar caché de vínculo
      end

      opt vínculo aún no registrado
        JSE->>WSE: solicitar documentos para vincular
        WSE-->>JSE: ClsssStructureVinculaDocumento[]
        JSE->>WSE: ServiceVinculaDocumentoExpediente por JSProgresBar
        WSE->>EXP: VinculaDocumentoExpediente(...)
        EXP->>DB: registrar vínculo
        JSE->>WSS: ServiceRegistraCahcheVinculacionSII(...)
        WSS->>DB: insertar caché de vínculo
      end

      JS->>WSS: ServiceActualizaIndiceDocumentosSII(...)
      WSS->>IDX: ActualizaIndiceDocumentosSII(...)
      IDX->>DB: actualizar índices e insertar caché de índices
      IDX-->>WSS: YES o texto de error
      WSS-->>JS: AppError

      JS->>WSS: SeviceGuardaConstanciaInscripcionSII(...)
      WSS->>SES: escribir DG_LISTA_CHEQUEO y leer tarea/ruta/usuario
      WSS->>ALM: PreAlmacenaConstanciaIsncripcionsSII(...)
      ALM->>DB: consultar trámite, gabinete, caché y expediente
      ALM->>PDF: obtener o generar constancia PDF
      PDF-->>ALM: ruta temporal o error
      ALM->>FS: AlmacenaDocumentoTareaWorkflow(...)
      FS-->>ALM: id de imagen y datos de lista
      ALM-->>WSS: YES o texto de error
      WSS-->>JS: error_gestion + dato_lista delimitado
      JS-->>U: insertar documento en lista de la tarea
    end
```

## 7. Secuencia actual de guardado múltiple

```mermaid
sequenceDiagram
    autonumber
    actor U as Usuario
    participant JS as Webworkflow.js
    participant JSE as JSExpediente.js
    participant PB as JSProgresBar.js
    participant WSE as WebServiceGaExpediente
    participant WSS as WebService_integracion_sii
    participant DB as Expedientes, índices y cachés
    participant ALM as ClassAlmacenamiento
    participant FS as Almacenamiento documental

    U->>JS: Guardar todas las inscripciones
    JS->>WSS: Service_solicita_registros_sellos_sii(0)
    WSS-->>JS: colección CIncripcionSII
    JS->>WSS: consultar caché del radicado
    WSS-->>JS: caché o vacío
    JS-->>U: solicitar tipología
    U->>JS: confirmar

    JS->>JSE: crear o vincular expediente(s)
    JSE->>WSS: consultar cachés de expediente y vínculo
    JSE->>WSE: crear expediente o solicitar documentos para vínculo
    WSE->>DB: mutaciones de expediente/vínculo
    JSE->>PB: vincular documentos secuencialmente
    PB->>WSE: ServiceVinculaDocumentoExpediente por elemento
    JSE->>WSS: registrar caché de vínculo si aplica

    JS->>WSS: ServiceActualizaIndiceDocumentosSII(colección)
    WSS->>DB: actualizar índices antes del almacenamiento

    JS->>PB: JSProgresBarBoot(ServiceGuardaSelloSII, colección)
    loop una inscripción por iteración
      PB->>WSS: SeviceGuardaConstanciaInscripcionSII(elemento)
      WSS->>ALM: preparar constancia
      ALM->>FS: almacenar documento
      FS-->>ALM: resultado
      ALM-->>WSS: YES o error
      WSS-->>PB: error_gestion + dato_lista
      PB-->>JS: YES, CTRL, CTRLRETURN u otro resultado
    end

    alt caché de inscripción inicialmente vacío
      JS->>WSS: ServiceRegistraCacheInscripcionRadicadoSII(colección)
      WSS->>DB: INSERT usando datos del primer elemento
    end
    JS-->>U: cerrar modales o mostrar error
```

### Fronteras transaccionales observadas

```text
[expediente/vínculos]   transacciones locales posibles
          |
          | llamada HTTP independiente
          v
[índices]               operación y caché propios
          |
          | llamada HTTP independiente
          v
[cada documento]        almacenamiento por elemento
          |
          | llamada HTTP independiente
          v
[caché inscripción]     inserción posterior
```

No se identificó commit o rollback común entre estos bloques.

## 8. Diagrama de estados actuales

El sistema no persiste una máquina de estados de importación. El siguiente diagrama reconstruye los estados observables derivados del control de flujo y sus retornos.

```mermaid
stateDiagram-v2
    [*] --> SinTarea
    SinTarea --> Bloqueada: tarea 0 o -1
    SinTarea --> ResolviendoServicio: tarea seleccionada

    ResolviendoServicio --> Bloqueada: sin permiso
    ResolviendoServicio --> SinConfiguracion: ruta, trámite o servicio inválido
    ResolviendoServicio --> ConsultandoSII: INTEGRACIONSII
    ResolviendoServicio --> SinAdaptadorObservado: otro NameService

    ConsultandoSII --> ErrorConsulta: token, transporte, deserialización o mensaje SII
    ConsultandoSII --> SinRegistros: colección vacía
    ConsultandoSII --> RegistrosDisponibles: filas consolidadas

    RegistrosDisponibles --> VisualizandoConstancia: abrir recurso externo
    VisualizandoConstancia --> RegistrosDisponibles: cerrar o volver
    RegistrosDisponibles --> ConsultandoCache: guardar una o todas

    ConsultandoCache --> RequiereGuardarTodas: individual y caché inexistente
    RequiereGuardarTodas --> RegistrosDisponibles
    ConsultandoCache --> SeleccionandoTipologia: caché existente o modo múltiple
    SeleccionandoTipologia --> RegistrosDisponibles: cancelar
    SeleccionandoTipologia --> PreparandoExpediente: confirmar

    PreparandoExpediente --> ErrorParcial: fallo al crear o consultar expediente
    PreparandoExpediente --> VinculandoDocumentos: documentos pendientes de vínculo
    PreparandoExpediente --> ActualizandoIndices: sin vínculos pendientes
    VinculandoDocumentos --> ErrorParcial: retorno distinto de YES
    VinculandoDocumentos --> ActualizandoIndices: vínculo finalizado
    ActualizandoIndices --> ErrorParcial: AppError distinto de YES
    ActualizandoIndices --> Almacenando: índices actualizados

    Almacenando --> DocumentoGuardado: YES
    Almacenando --> OmitidoControlado: CTRL
    Almacenando --> RequiereDecisionLegacy: CTRLRETURN
    Almacenando --> ErrorParcial: otro resultado
    DocumentoGuardado --> Almacenando: quedan elementos
    OmitidoControlado --> Almacenando: quedan elementos
    RequiereDecisionLegacy --> Almacenando: continuar según control actual

    DocumentoGuardado --> RegistrandoCache: último elemento y caché vacío
    DocumentoGuardado --> Completada: caché ya existente
    RegistrandoCache --> ErrorParcial: inserción fallida
    RegistrandoCache --> Completada: YES

    Completada --> [*]
    ErrorConsulta --> [*]
    SinRegistros --> [*]
    SinConfiguracion --> [*]
    SinAdaptadorObservado --> [*]
    Bloqueada --> [*]
    ErrorParcial --> [*]
```

### Limitación crítica del estado actual

Los estados `ErrorParcial`, `DocumentoGuardado` o `Completada` viven principalmente en memoria del navegador y en retornos textuales. No existe una intención persistida que permita consultar directamente la fase alcanzada después de perder la respuesta.

## 9. Diagrama de datos y estado compartido

```mermaid
flowchart TB
    UI[Webworkflow.js]

    subgraph Browser[Estado global del navegador]
      CI[CIncripcionSII array]
      CS[CacheInscripcion array]
      MS[MULTIPLE_SII]
      CFG[IListCservicioIntegracionAdjuntaDocumento]
      JSR[_JSEexpedienteResult]
    end

    subgraph Session[Session ASP.NET]
      T[ID_TAREA_SELECCIONDA]
      R[WF_RUTAWORKFLOW / Id_Ruta_Workflow]
      U[GA_IDUSUARIOGESTION]
      TP[DG_LISTA_CHEQUEO]
      PM[ADJUNTAR_IMAGENES_PREDETERMINADA]
      TMP[Rutas temporales]
    end

    subgraph Persistence[Persistencia]
      CE[(Caché expediente SII)]
      CV[(Caché vinculación SII)]
      CX[(Caché índices SII)]
      CI2[(ra_sii_cahche_inscripcion)]
      EX[(Expedientes y vínculos)]
      DOC[(Documentos e índices)]
      FILE[(Archivos documentales)]
    end

    UI --> CI
    UI --> CS
    UI --> MS
    UI --> CFG
    UI --> JSR
    UI --> Session
    Session --> CE
    Session --> CV
    Session --> CX
    Session --> CI2
    Session --> EX
    Session --> DOC
    Session --> FILE
```

No existe una entidad persistente observada que una todos estos estados mediante un único `operationId`.

## 10. Diagrama de retorno actual hacia la lista de documentos

```mermaid
sequenceDiagram
    participant ALM as AlmacenaDocumentoTareaWorkflow
    participant WSS as SeviceGuardaConstanciaInscripcionSII
    participant AJAX as ServiceRESTGuardaConstanciaInscripcionSII
    participant UI as Lista de documentos Workflow

    ALM-->>WSS: IdImgenAlmacenada + stru_datos_image_lista
    WSS->>WSS: concatenar gabinete|id_imagen|radicado|tipología|...|id_tarea|firma|icono
    WSS-->>AJAX: error_gestion = YES + dato_lista
    AJAX->>AJAX: separar dato_lista y validar respuesta
    AJAX->>UI: insert_row_documento_relacionado(...)
```

### Consecuencia

La lista se actualiza a partir de una cadena construida durante la misma respuesta, no mediante una consulta posterior de reconciliación. Si la respuesta se pierde después de almacenar, el documento puede existir sin que la interfaz lo incorpore hasta un refresco alternativo.

## 11. Puntos de fallo por fase

| Fase | Efecto que puede existir | Estado posterior demostrable actualmente |
|---|---|---|
| Token o consulta | Ninguna mutación local esperada | Error textual |
| Crear expediente | Expediente y caché parcial | No hay intención consolidada |
| Vincular documentos | Algunos vínculos completados | Resultado por iteración en cliente |
| Actualizar índices | Índices y caché de índices | `YES` o texto libre |
| Obtener/generar PDF | Archivo temporal | Limpieza y colisión requieren validación |
| Almacenar documento | Documento y relación con tarea | `dato_lista` si llega la respuesta |
| Registrar caché final | Caché basado en primer elemento | `YES` o texto libre |
| Pérdida de respuesta | Cualquier efecto anterior | Resultado incierto, sin consulta por intención |

## 12. Invariantes actuales y no demostradas

### Comprobadas en el flujo

- La consulta principal valida tarea y permiso antes de invocar SII.
- El navegador ejecuta expediente, índices, almacenamiento y caché en ese orden.
- El guardado múltiple almacena elementos secuencialmente mediante `JSProgresBar`.
- El guardado individual llama directamente al endpoint de almacenamiento.
- El documento exitoso devuelve un identificador y metadatos para la lista.
- El caché de inscripción se consulta por radicado y su registro usa el primer elemento.

### No demostradas por análisis estático

- Unicidad persistente por inscripción.
- Autorización completa repetida en todos los endpoints mutadores.
- Atomicidad entre expediente, índices, documento y caché.
- Compensación cuando falla una fase posterior.
- Limpieza garantizada del temporal en todos los resultados.
- Reconciliación después de timeout o cierre del navegador.
- Seguridad de las URL externas durante todo su ciclo de vida.
- Compatibilidad real de otros proveedores con este recorrido.

## 13. Fuentes principales de la radiografía

- `js/workflow/Webworkflow.js`
- `js/java_general/JSExpediente.js`
- `js/java_general/JSProgresBar.js`
- `webservice/WebServiceAdjuntaDocumentoServicioIntegracion.asmx.vb`
- `webservice/WebService_integracion_sii.asmx.vb`
- `webservice/WebServiceGaExpediente.asmx.vb`
- `ServiciosIntegracion/ClassAdjuntaDocumentoServicioIntegracion.vb`
- `ServiciosIntegracion/Class_ra_ser_servicioIntegracion.vb`
- `Integracionccv/Class_consultarInformacionSello.vb`
- `Integracionccv/Class_ClassResfull.vb`
- `Integracionccv/ClassRaSIiCacheExpediente.vb`
- `Integracionccv/ClassRaSIICacheActualizaIndice.vb`
- `Integracionccv/ClassRaSiiCahcheInscripcion.vb`
- `workflow/ClassAlmacenamiento.vb`
- `Gestion/ClassGaExpediente.vb`
- `itextshare/Class_ItexShare.vb`

## 14. Conclusión de la radiografía

El backend actual no funciona como una única operación de importación. Funciona como una coreografía cliente de endpoints independientes, unidos por estado global del navegador, sesión ASP.NET, cachés SII y convenciones textuales.

La modernización backend deberá preservar los efectos funcionales demostrados, pero trasladar la autoridad de la operación a un orquestador servidor con contexto explícito, intención persistida, resultados estructurados y reconciliación. Esta conclusión describe el problema arquitectónico; no autoriza todavía su implementación.
