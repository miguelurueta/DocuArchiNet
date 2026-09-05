# Exploración backend de ImportarServicioWeb

## 1. Propósito

Documentar el backend actual de la importación desde servicios externos, usando `INTEGRACIONSII` como primer caso observado, y establecer una arquitectura segura para modernizar consulta, preparación, importación y reconciliación sin convertir SII en el contrato común de todos los proveedores.

Este documento es exclusivamente exploratorio. No autoriza cambios de código, llamadas reales a proveedores, escrituras, migraciones, pruebas E2E, carga ni activación de gates.

## 2. Relación con la exploración funcional

La experiencia, decisiones frontend, progreso, reconciliación visual y transición legacy se documentan en `01-exploracion-modernizacion-importar-servicio-web.md`. El presente documento profundiza exclusivamente en las garantías que deben residir en servidor.

La sincronización normativa entre ambos lados, incluida la propiedad única de ejecución, operaciones, estados, preview, gate y orden cruzado, se mantiene en `../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`.

Los diagramas de clases, casos de uso, secuencias, estados y datos del backend existente se mantienen en `04-radiografia-backend-actual.md`. Ese documento representa el estado observado y no debe confundirse con los diseños objetivo de esta exploración.

La frontera deseada es:

```text
Frontend
├── presenta capacidades del proveedor
├── captura selección y requisitos
├── muestra progreso confirmado
└── representa resultados reconciliados
        │
        ▼
Backend ImportarServicioWeb
├── autoriza y fija contexto
├── resuelve adaptador
├── consulta proveedor
├── crea intención idempotente
├── ejecuta efectos controlados
└── reconcilia documentos
        │
        ▼
Adaptadores
├── INTEGRACIONSII
└── proveedores futuros
```

## 3. Componentes backend observados

| Responsabilidad | Componente actual |
|---|---|
| Resolver integración habilitada | `webservice/WebServiceAdjuntaDocumentoServicioIntegracion.asmx.vb` |
| Validar configuración inicial | `ServiciosIntegracion/ClassAdjuntaDocumentoServicioIntegracion.vb` |
| Catálogo de servicios | `ServiciosIntegracion/Class_ra_ser_servicioIntegracion.vb` |
| Consultar inscripciones SII | `webservice/WebService_integracion_sii.asmx.vb` y `Integracionccv/Class_consultarInformacionSello.vb` |
| Transporte HTTP externo | `Integracionccv/Class_ClassResfull.vb` |
| Crear o vincular expedientes | `webservice/WebServiceGaExpediente.asmx.vb`, `Gestion/ClassGaExpediente.vb` y cachés SII |
| Actualizar índices | `webservice/WebService_integracion_sii.asmx.vb` y `Integracionccv/ClassRaSIICacheActualizaIndice.vb` |
| Guardar constancia | `SeviceGuardaConstanciaInscripcionSII` y `workflow/ClassAlmacenamiento.vb` |
| Registrar caché | `Integracionccv/ClassRaSiiCahcheInscripcion.vb` |
| Construir respuesta para la UI | `estado_respuesta_sello_sii.dato_lista` |

El proyecto compila para .NET Framework 4.6.1. Existen implementaciones asíncronas parciales en `Class_ClassResfull`, pero el recorrido SII activo continúa invocando transporte síncrono.

## 4. Secuencia backend actual

La consulta y la escritura no forman una única operación de servidor. El navegador coordina varios endpoints:

```text
resolver integración y configuración
→ consultar SII
→ consultar caché
→ crear o vincular expediente
→ actualizar índices
→ guardar cada constancia
→ registrar caché si no existía
→ construir filas de documentos en el cliente
```

### 4.1 Consulta

`Class_consultarInformacionSello`:

1. Recupera tarea y permiso desde sesión.
2. Obtiene código de barras y radicado de la tarea.
3. Recupera credenciales técnicas y URL desde configuración.
4. Solicita un token SII.
5. Ejecuta `consultarInformacionSello`.
6. Deserializa y consolida los registros para la tabla.

### 4.2 Importación

El endpoint `SeviceGuardaConstanciaInscripcionSII`:

1. Deserializa una lista y utiliza su primer elemento.
2. Escribe la tipología en `Session("DG_LISTA_CHEQUEO")`.
3. Recupera configuración del trámite.
4. Consulta datos de expediente SII.
5. Invoca `PreAlmacenaConstanciaIsncripcionsSII`.
6. Devuelve `YES` y una cadena delimitada con datos para insertar el documento en la interfaz.

Antes de este endpoint, el cliente ya pudo haber creado o vinculado expedientes y actualizado índices. Después, el cliente puede registrar el caché. No se identificó una transacción que abarque esa secuencia completa.

## 5. Contexto y sesión

El backend utiliza, entre otros:

- `ID_TAREA_SELECCIONDA`;
- `ID_TAREA_SELECCIONDA_ENLACE`;
- `WF_RUTAWORKFLOW`;
- `Id_Ruta_Workflow`;
- `DG_LISTA_CHEQUEO`;
- `GA_IDUSUARIOGESTION`;
- `ADJUNTAR_IMAGENES_PREDETERMINADA`;
- rutas temporales almacenadas en sesión.

### Hallazgo B-01: identidad de tarea mutable

Varios endpoints mutadores vuelven a leer la tarea desde sesión en lugar de recibir y validar un contexto inmutable. Otra pestaña puede cambiar esa selección durante una operación larga.

### Hallazgo B-02: tipología compartida en sesión

El guardado escribe `DG_LISTA_CHEQUEO`. Dos operaciones concurrentes o intercaladas dentro de la misma sesión pueden sobrescribir el valor esperado.

### Hallazgo B-03: bloqueo de sesión

Aunque las llamadas externas se vuelvan asíncronas, las solicitudes que escriben sesión ASP.NET pueden serializarse por sesión. Por ello, `async` no garantiza concurrencia efectiva mientras cada endpoint conserve acceso de escritura a sesión.

### Decisión recomendada

Capturar antes del primer `Await` un contexto validado y pasar explícitamente tarea, ruta, usuario, proveedor y tipología a las capas internas. La sesión podrá participar en la autorización inicial, pero no sustituir ni cambiar el destino de la operación.

## 6. Autorización

La apertura inicial valida permiso, tarea y configuración mediante `ClassAdjuntaDocumentoServicioIntegracion`. No se demostró que todos los endpoints posteriores repitan el mismo conjunto de controles.

Cada operación deberá validar en servidor:

```text
usuario autenticado
+ permiso vigente
+ tarea existente y operable
+ ruta y trámite correspondientes
+ proveedor habilitado
+ intención perteneciente al usuario y tarea
+ elemento externo incluido en la intención
+ tipología y destino permitidos
```

La autorización inicial del modal no puede reutilizarse como autorización implícita para endpoints mutadores independientes.

## 7. Asincronía

### 7.1 Situación actual

`Class_ClassResfull.GetResponse` utiliza `WebRequest.GetResponse()` y `GetRequestStream()`, por lo que bloquea un hilo mientras espera token y respuesta externa.

El mismo archivo contiene:

- `GetResponseAsync` basado en `HttpWebRequest`;
- `GetResponse_POST_Async` basado en `HttpClient`;
- un `HttpClientSingleton`;
- versiones comentadas que intentan adaptar async a llamadas síncronas mediante `Task.Run(...).Result` o `GetResult()`.

El recorrido activo no usa esas variantes.

### 7.2 Dónde aplicar async

Async aporta valor en I/O externo real:

- token del proveedor;
- consulta de elementos;
- descarga de constancias o anexos;
- obtención de recursos para vista previa;
- futuras llamadas HTTP de adaptadores.

No debe simularse asincronía con `Task.Run` para:

- generación PDF;
- librerías de archivos síncronas;
- reglas de negocio síncronas;
- wrappers de base de datos sin API async;
- operaciones CPU-bound.

### 7.3 Regla arquitectónica

```text
I/O HTTP realmente asíncrono
→ Await hasta una frontera asíncrona compatible
→ mutaciones internas secuenciales
→ reconciliación
```

No utilizar `.Result`, `.Wait()`, `GetAwaiter().GetResult()` ni `Task.Run` para cerrar artificialmente una cadena asíncrona dentro de ASP.NET.

### 7.4 Async no implica paralelismo

La primera modernización mantendrá el procesamiento de elementos en secuencia. Ejecutar varias inscripciones simultáneamente elevaría el riesgo sobre sesión, temporales, expedientes, índices, caché y orden de resultados.

### 7.5 Complejidad estimada

| Alcance | Complejidad | Riesgo |
|---|---:|---:|
| Cliente HTTP tipado y asíncrono | 5/10 | Medio |
| Consulta SII asíncrona de extremo a extremo | 6/10 | Medio |
| Descarga externa y cancelación | 6/10 | Medio-alto |
| Mantener ASMX y propagar async hasta la frontera | 8/10 | Alto |
| Volver asíncrona toda la persistencia | 9/10 | Alto |
| Procesar inscripciones en paralelo | 9/10 | Crítico inicialmente |

## 8. Transporte HTTP

### Hallazgo B-04: contratos inconsistentes

Las implementaciones existentes difieren en formato: el método síncrono envía JSON, mientras variantes asíncronas utilizan formulario URL-encoded. Cambiar de implementación sin prueba de contrato puede alterar la integración SII.

### Hallazgo B-05: configuración compartida mutable

El singleton de `HttpClient` cambia `Timeout` dentro de la operación. La configuración común debe establecerse una vez o administrarse mediante clientes por proveedor; no debe mutarse concurrentemente.

### Hallazgo B-06: validación de certificados

El transporte instala callbacks globales que aceptan certificados sin validación. Esta conducta no debe trasladarse al cliente moderno. La política TLS y de certificados debe ser segura, explícita y verificable por ambiente.

### Contrato recomendado

Cada adaptador utilizará un cliente tipado equivalente a:

```text
IExternalImportProviderClient
├── GetCapabilitiesAsync(context, cancellationToken)
├── QueryAsync(context, criteria, cancellationToken)
├── GetPreviewAsync(context, externalKey, cancellationToken)
└── FetchDocumentAsync(context, externalKey, cancellationToken)
```

El cliente declarará timeout por operación, tipos de contenido, serialización, límites de tamaño y traducción segura de fallos.

## 9. Transacciones y orden de efectos

Se observaron transacciones locales dentro de componentes de almacenamiento y cachés, pero no una unidad transaccional que abarque proveedor, expediente, índices, documento y caché.

La transacción distribuida no es viable sobre un servicio externo y almacenamiento documental heterogéneo. La solución deberá usar una intención persistida y una máquina de estados, no prometer atomicidad global inexistente.

### Estados conceptuales de intención

```text
Creada
→ Validada
→ RecursoObtenido
→ ExpedientePreparado
→ DocumentoAlmacenado
→ ÍndicesActualizados
→ CachéActualizado
→ Reconciliada
→ Completada
```

Estados alternos:

```text
RequiereDecision
FallidaAntesDePersistir
ResultadoIncierto
Parcial
Detenida
```

No se debe afirmar que una operación falló sin persistir cuando no puede demostrarse en qué fase terminó.

## 10. Idempotencia

No se identificó una clave idempotente explícita por intención y elemento. El caché SII se consulta por `RadicadoSII`, lo cual representa contexto del trámite, pero no necesariamente una inscripción individual.

La identidad debe dividirse en dos niveles:

```text
intención
├── idOperacion
├── tarea
├── usuario
├── proveedor
└── selección y requisitos

elemento
├── proveedor
├── claveExternaCanónica
├── tarea destino
└── idIntención
```

Para SII debe validarse si la clave externa canónica requiere código de barras o radicado, libro y registro, y si existe un identificador oficial más estable.

La restricción de unicidad debe residir en servidor y, cuando sea posible, en almacenamiento persistente. Una verificación previa en cliente no evita carreras.

## 11. Persistencia y caché

### Hallazgo B-07: SQL concatenado

`ClassRaSiiCahcheInscripcion` construye consultas e inserciones concatenando valores funcionales. La modernización debe usar parámetros y no trasladar esta práctica a contratos genéricos.

### Hallazgo B-08: primer elemento como contexto

El registro de caché recibe una colección y toma el primer elemento para construir el registro general. Debe definirse explícitamente si el caché pertenece al radicado, a la intención o a cada elemento.

### Hallazgo B-09: acoplamiento de cardinalidad

El cliente usa la importación masiva para inicializar contexto requerido por la individual. El backend objetivo ofrecerá un preflight o preparación explícita independiente del número de elementos seleccionados.

## 12. Almacenamiento documental

`PreAlmacenaConstanciaIsncripcionsSII` descarga o genera el PDF, construye campos de gabinete y llama a `AlmacenaDocumentoTareaWorkflow`.

`AlmacenaDocumentoTareaWorkflow` ya es una infraestructura compartida, pero contiene comportamiento condicionado por `NombreCaso = "SII"`. Esto indica una frontera incompleta entre reglas de proveedor y almacenamiento común.

La modernización se implementará en paralelo y no reemplazará el código vigente. `AlmacenaDocumentoTareaWorkflow(...)` se reutilizará como una caja negra compartida: estos prompts no autorizan cambiar su firma, lógica interna, efectos ni consumidores actuales. El adaptador nuevo deberá resolver externamente cualquier traducción entre el contrato moderno y los argumentos que esta función ya espera.

El adaptador debe entregar un comando documental normalizado:

```text
ImportDocumentCommand
├── operationId
├── taskId
├── providerId
├── externalKey
├── documentTypeId
├── contentDescriptor
├── targetCabinet
├── normalizedIndexes[]
└── providerMetadata
```

El almacenamiento común no debe interpretar matrícula, libro, registro ni códigos SII. El adaptador transforma esos datos antes de cruzar la frontera.

## 13. Contratos de resultado

El contrato actual combina `error_gestion = "YES"` con `dato_lista`, una cadena delimitada por `|`. Esto dificulta versionado, validación y evolución multiproveedor.

El contrato objetivo será estructurado:

```text
ImportItemResult
├── schemaVersion
├── operationId
├── providerId
├── externalKey
├── status
├── code
├── safeMessage
├── retryable
├── persistenceKnown
├── reachedPhase
├── document
│   ├── id
│   ├── cabinet
│   ├── documentType
│   └── taskId
└── correlationId
```

Durante la transición, el adaptador traducirá este resultado hacia o desde `YES`, `CTRL`, `CTRLRETURN` y `dato_lista` sin exponer esos detalles al núcleo.

## 14. Reconciliación

La confirmación de una llamada de guardado no basta para reconstruir el estado después de timeout, recarga o cambio de tarea. Se requiere una consulta de reconciliación por intención y por elemento.

La fuente de verdad deberá componer:

- intención y fases persistidas;
- tarea original;
- identidad externa;
- documento almacenado;
- relación con la tarea;
- expediente y vínculos aplicables;
- índices requeridos;
- caché del adaptador.

Solo un documento confirmado y relacionado con la tarea original podrá incorporarse a la lista de documentos.

## 15. Arquitectura multiproveedor backend

```text
ImportServiceOrchestrator
├── Authorize
├── ResolveProvider
├── Preflight
├── CreateIntent
├── ExecuteSequentially
└── Reconcile
        │
        ├── IExternalImportProvider
        │   ├── SiiImportProvider
        │   └── proveedores futuros
        │
        └── servicios comunes
            ├── almacenamiento documental
            ├── tareas y permisos
            ├── expedientes
            ├── índices
            └── auditoría
```

El registro de proveedores se resolverá mediante la identidad configurada. Un proveedor desconocido producirá un resultado explícito de no soportado; nunca utilizará SII como fallback.

## 16. Compatibilidad ASMX

La migración no debe obligar a reemplazar todos los ASMX en una sola entrega.

La primera implementación será aditiva y coexistirá con los ASMX y la coreografía actuales bajo un gate reversible. Ningún endpoint moderno sustituirá ni redirigirá silenciosamente una ruta existente.

Estrategia recomendada:

1. Extraer contratos y lógica a clases independientes de `HttpContext`.
2. Crear clientes externos asíncronos por proveedor.
3. Introducir un orquestador que reciba contexto explícito.
4. Mantener ASMX como adaptadores de compatibilidad mientras el frontend legacy los necesite.
5. Incorporar endpoints modernos asíncronos cuando la frontera ASMX no permita propagar `Task` de forma segura.
6. Retirar las rutas antiguas únicamente después de regresión y E2E autorizada.

No se debe llamar una API async desde ASMX mediante espera bloqueante para declarar artificialmente que el recorrido fue modernizado.

## 17. Timeout, cancelación y recuperación

Cada llamada externa debe aceptar cancelación y timeout, pero la cancelación del navegador no implica rollback.

Clasificación mínima:

| Situación | Resultado backend |
|---|---|
| Cancelación antes de enviar | Detenida sin efecto externo conocido |
| Timeout consultando | Consulta fallida, sin mutación local |
| Timeout descargando antes de persistir | Fallida antes de persistir, si puede demostrarse |
| Pérdida de respuesta durante escritura | Resultado incierto; reconciliar |
| Detención después de elementos completados | Parcial; no revertir confirmados |

Un reintento solo será permitido cuando el servidor confirme `retryable = true`, conozca la fase alcanzada y reutilice la misma identidad idempotente.

## 18. Seguridad

- No exponer credenciales, tokens, cookies, cadenas de conexión, rutas físicas ni respuestas externas crudas.
- Validar certificados y TLS sin callbacks globales permisivos.
- Parametrizar consultas SQL.
- Sanear mensajes antes de enviarlos al cliente.
- Aplicar límites de tamaño y tipo de contenido antes de almacenar.
- Validar que el documento recuperado corresponda al proveedor, intención y elemento autorizados.
- Registrar correlación y auditoría sin datos secretos.
- No confiar en URL, proveedor, tarea, gabinete o tipología enviados por el navegador sin revalidación.

## 19. Estrategia de pruebas backend

### Focales sin red

- Registro y resolución de proveedores.
- Serialización y normalización de contratos.
- Traducción de códigos legacy.
- Timeout, cancelación y clasificación de errores.
- Captura inmutable del contexto.
- Máquina de estados de intención.
- Idempotencia y carreras sobre el mismo elemento.
- SQL parametrizado y validaciones de entrada.

### Integración local

- Servidor HTTP simulado para token, consulta, descarga, timeout y respuesta inválida.
- Repositorios o almacenamiento temporal aislado.
- Fallo inyectado antes y después de cada fase persistente.
- Reconciliación de documento, tarea, expediente, índices y caché.
- Compatibilidad de lectura y respuesta con los adaptadores ASMX existentes.

### E2E

La E2E real requiere autorización explícita. Antes de una prueba autenticada de `PreviewEnviarTarea` se debe leer `tools/e2e/AGENT-RUNBOOK.md`. Las consultas de control serán exclusivamente `SELECT`; no se imprimirán secretos; y cualquier gate autorizado deberá restaurarse a su estado seguro al finalizar.

## 20. Decisiones recomendadas

1. Async obligatorio para I/O HTTP nuevo, propagado sin bloqueos síncronos hasta una frontera compatible.
2. Mutaciones secuenciales en la primera modernización; no paralelizar elementos.
3. Contexto explícito e inmutable; reducir dependencias de sesión antes de ampliar concurrencia.
4. Intención persistida y máquina de estados en lugar de atomicidad global ficticia.
5. Contratos tipados y versionados; compatibilidad legacy confinada a adaptadores.
6. Núcleo multiproveedor; SII no será fallback ni contaminará almacenamiento común.
7. Reconciliación obligatoria para resultados inciertos y actualización de documentos.
8. Seguridad de transporte y SQL como parte de la modernización, no como mejora opcional posterior.
9. `ImportServiceOrchestrator` como único ejecutor moderno; `JSProgresBar` conserva su ejecución legacy y actúa solo como presentación en la ruta moderna.

## 21. Preguntas abiertas

1. ¿La versión y configuración actual de ASMX permite propagar `Task` de forma confiable o se requieren endpoints paralelos?
2. ¿Qué proveedores están configurados realmente además de `INTEGRACIONSII`?
3. ¿Cuál es la identidad externa canónica de una inscripción SII?
4. ¿Qué tablas y archivos modifica exactamente cada fase?
5. ¿Qué transacciones locales incluyen la relación documento-tarea y cuáles quedan fuera?
6. ¿Puede reordenarse actualización de índices después del almacenamiento confirmado?
7. ¿Qué compensaciones son funcionalmente válidas para expediente, índices y caché?
8. ¿Qué auditoría existe actualmente y qué nueva auditoría requiere la intención?
9. ¿Qué límites de timeout, tamaño y cantidad aplica cada proveedor?
10. ¿Qué endpoints pueden operar sin escribir sesión y cuáles requieren adaptación?

## 22. Prompts backend derivados

Después de validar las preguntas abiertas, la implementación se divide en los prompts ejecutables de [`../PromptBackend/`](../PromptBackend/README.md):

1. [Contratos, contexto y registro multiproveedor](../PromptBackend/01-contratos-contexto-registro-multiproveedor.md).
2. [Clientes HTTP asíncronos, timeout, cancelación y seguridad](../PromptBackend/02-clientes-http-asincronos-seguridad.md).
3. [Preflight, intención persistida e idempotencia](../PromptBackend/03-preflight-intencion-idempotencia.md).
4. [Orquestación secuencial, estados y compensación](../PromptBackend/04-orquestacion-secuencial-estados-compensacion.md).
5. [Reconciliación y actualización de la lista de documentos](../PromptBackend/05-reconciliacion-lista-documentos.md).
6. [Adaptador SII y compatibilidad ASMX](../PromptBackend/06-adaptador-sii-compatibilidad-asmx.md).
7. [Pruebas backend, integración local y evidencia autorizada](../PromptBackend/07-pruebas-backend-evidencia.md).
