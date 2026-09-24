# Exploración técnica: modernización de la importación de anexos SII en actividades ENLASE

## 1. Propósito

Este documento registra la exploración arquitectónica del flujo que permite adjuntar documentos provenientes del SII mientras se selecciona una tarea cuya actividad es de tipo `ENLASE`.

El objetivo futuro es modernizar esta operación reutilizando la plataforma construida para la importación moderna desde servicios externos, sin confundirla con la importación de constancias e inscripciones. Aunque ambas integraciones pertenecen al SII, consultan operaciones diferentes y tienen reglas de negocio, contexto y persistencia distintos.

Este documento no constituye una implementación. Antes de modificar código debe iniciarse el ticket correspondiente mediante `opsxj:new`, con requisitos, diseño, tareas y criterios de aceptación revisados dentro del flujo OPSXJ.

## 2. Conclusión arquitectónica

La solución debe registrar una nueva **capacidad de negocio** dentro del proveedor existente `INTEGRACIONSII`:

```text
Proveedor:          INTEGRACIONSII
Credenciales:       compartidas
Contrato seguridad: compartido
Capacidad actual:   CONSTANCIAS_INSCRIPCION
Capacidad nueva:    ANEXOS_RADICADO_ENLASE
```

No se recomienda crear otro proveedor, duplicar credenciales ni agregar una segunda fila en `ra_ser_servicioIntegracion`. La nueva operación utiliza el mismo sistema externo, la misma autenticación y el mismo contrato de seguridad.

Se deben separar, sin embargo:

- Los endpoints consumidos.
- Los contratos de solicitud y respuesta.
- El mapper del contrato externo al modelo interno.
- Las validaciones de contexto.
- Las reglas de preparación y persistencia.
- La telemetría por operación.
- Las pruebas funcionales y E2E.

## 3. Flujo legacy identificado

La actividad se encuentra registrada con el literal histórico `ENLASE`. Al seleccionar una tarea de este tipo, el sistema no la asigna inmediatamente. Primero abre un administrador de documentos que permite completar los documentos requeridos y posteriormente confirmar la asignación.

Flujo observado:

```text
Seleccionar tarea ENLASE
        |
        v
Guardar contexto temporal en sesión
        |
        v
Abrir popup de enlace de documentos
        |
        v
Consultar documentos relacionados
        |
        v
Opcionalmente consultar anexos en SII
        |
        v
Seleccionar anexo y tipología
        |
        v
Descargar y almacenar inmediatamente
        |
        v
Actualizar lista de documentos del popup
        |
        v
Validar documentos obligatorios
        |
        v
Asignar la tarea
```

La selección de la tarea y la asignación son dos fases distintas. La modernización no debe asignar automáticamente la tarea después de importar un documento.

## 4. Componentes legacy relevantes

### 4.1 Selección de tarea

`workflow/Classselecciotarea.vb` identifica las actividades `ENLASE`, establece el contexto temporal y abre el modal de administración de documentos.

Entre los valores mantenidos en sesión se encuentran:

- `ID_TAREA_SELECCIONDA_ENLACE`
- `SELECCIONTEMPORAL`
- `WF_TAGSELECCION_EMERGENTE`
- `WF_RUTAWORKFLOW`
- valores auxiliares `DG_*`

### 4.2 Interfaz

`workflow/Webworkflow.aspx` contiene el panel de enlace de documentos, la lista de documentos relacionados, el visor y la acción final de asignación.

`js/workflow/Webworkflow.js` controla la activación de la integración SII, la consulta de anexos y el almacenamiento del anexo seleccionado.

### 4.3 Activación de la integración

La operación comienza desde:

```text
ServiceAdjuntaDocumentoServicioIntegracionEnlace
```

Este servicio valida el permiso legacy, la tarea seleccionada, la ruta, el trámite, el tipo de documento entrante y la configuración de la integración.

### 4.4 Consulta de anexos SII

La consulta se realiza mediante:

```text
WebService_integracion_sii.asmx/
ServiceSolicitaArchivosAnexosrelacionadosRadicadoSII
```

`Integracionccv/ClassListaAnexosSII.vb` reconstruye desde la sesión:

- Identificación de la tarea.
- Código de barras.
- Recibo o radicado SII.
- Gabinete.

Después invoca la consulta SII y transforma la colección `imagenes` en una lista de anexos.

### 4.5 Persistencia

El almacenamiento se ejecuta mediante:

```text
WebService_integracion_sii.asmx/SeviceGuardaDocumentoAnexoSII
```

Este servicio delega en:

```text
ClassAlmacenamiento.PreAlmacenaDocumentoAnexosEnlaceIntegracionSII
```

La operación descarga el recurso externo, prepara campos de índice y utiliza el almacenamiento documental legacy. Después devuelve una cadena delimitada que el JavaScript inserta en la lista de documentos relacionados.

## 5. Diferencias frente a la importación moderna de constancias

| Aspecto | Constancias e inscripciones | Anexos SII de ENLASE |
|---|---|---|
| Momento | Tarea seleccionada o gestionada | Antes de asignar la tarea |
| Fuente | Constancias o inscripciones | Imágenes/anexos del radicado |
| Contexto principal | Código de barras y tarea | Tarea, recibo, código de barras, ruta y gabinete |
| Persistencia | Intención moderna y ejecución controlada | Persistencia inmediata legacy |
| Tipología | Clasificación del documento importado | Tipología válida para el flujo y checklist |
| Finalización | Resultado de importación | Actualización documental y posterior asignación |
| Duplicidad | Identidad de inscripción/recurso | Identidad estable del anexo y existencia física |

Por estas diferencias no se debe redirigir simplemente el enlace legacy hacia la pantalla moderna de constancias.

## 6. Registro del servicio derivado

### 6.1 Estructura lógica

```text
INTEGRACIONSII
|
+-- Autenticación compartida
|   +-- código de empresa
|   +-- usuario de servicio
|   +-- contraseña
|   +-- obtención/renovación de token
|
+-- CONSTANCIAS_INSCRIPCION
|   +-- consultar
|   +-- vista previa
|   +-- descargar
|
+-- ANEXOS_RADICADO_ENLASE
    +-- consultar
    +-- vista previa
    +-- descargar
    +-- importar antes de asignar
```

### 6.2 Registro moderno

El registro actual rechaza identidades canónicas duplicadas. Por ello debe registrarse una sola implementación con `ProviderId = INTEGRACIONSII`, compuesta internamente por adaptadores operacionales.

Diseño conceptual:

```text
RegistroClientesProveedoresImportacion
                |
                v
        SiiImportProvider
          ProviderId:
        INTEGRACIONSII
                |
        +-------+-------+
        |               |
        v               v
SiiConstanciasClient  SiiAnexosEnlaseClient
```

La capacidad solicitada determina el adaptador, pero el servidor debe comprobar que dicha capacidad sea compatible con el contexto real de la tarea.

### 6.3 Base de datos

Se conserva una sola fila activa:

```text
ra_ser_servicioIntegracion.NombreServicio = INTEGRACIONSII
```

No se propone registrar `INTEGRACIONSII2` ni `INTEGRACIONSII_ANEXOS` como nuevos proveedores.

### 6.4 Telemetría

Los intentos se diferencian por la columna o dimensión de operación:

```text
INTEGRACIONSII / QUERY_CONSTANCIAS
INTEGRACIONSII / PREVIEW_CONSTANCIA
INTEGRACIONSII / DOWNLOAD_CONSTANCIA
INTEGRACIONSII / QUERY_ANEXOS_ENLASE
INTEGRACIONSII / PREVIEW_ANEXO_ENLASE
INTEGRACIONSII / DOWNLOAD_ANEXO_ENLASE
```

Esto permite medir la disponibilidad del servicio de anexos de manera independiente sin duplicar la identidad del proveedor.

## 7. Arquitectura propuesta

```text
+-----------------------------+
| Cliente Webworkflow         |
| Modal moderno ENLASE        |
+--------------+--------------+
               |
               | DTO seguro
               v
+-----------------------------+
| Controller / ASMX moderno   |
| ImportarAnexosEnlase        |
+--------------+--------------+
               |
               | comando tipado
               v
+-----------------------------+
| Service                     |
| ImportarAnexosEnlaseService|
+------+----------------+-----+
       |                |
       v                v
+-------------+  +--------------------+
| Provider SII|  | Repositories       |
| Anexos      |  | Workflow/Documentos|
+------+------+  +----------+---------+
       |                    |
       v                    v
+-------------+  +--------------------+
| Servicio SII|  | BD y repositorio   |
| de anexos   |  | documental         |
+-------------+  +--------------------+
```

### 7.1 Cliente

Responsabilidades:

- Abrir el modal moderno desde el flujo `ENLASE`.
- Mostrar anexos, selección múltiple y selección total.
- Presentar vista previa segura.
- Permitir asignar o confirmar tipología.
- Preparar y ejecutar una intención.
- Mostrar el resultado por documento.
- Actualizar la lista de documentos relacionados.
- Mantener visible el error cuando la operación falle.

### 7.2 Controller o servicio ASMX moderno

Operaciones sugeridas:

- `ResolveCapabilities`
- `QueryItems`
- `GetPreview`
- `PrepareImport`
- `ExecuteImportIntent`
- `GetImportIntent`
- `ReconcileImportIntent`
- `ValidateAssignment`

Debe validar estructura, sesión y correlación, y devolver códigos seguros. No debe contener SQL ni reglas detalladas de almacenamiento.

### 7.3 Service

Responsabilidades:

- Reconstruir el contexto desde fuentes autoritativas.
- Validar que la actividad sea `ENLASE`.
- Confirmar que la tarea continúe disponible para preasignación.
- Validar la configuración SII del trámite.
- Orquestar consulta, preparación, ejecución y reconciliación.
- Aplicar idempotencia.
- Consolidar resultados individuales.
- Mantener separadas la importación y la asignación de la tarea.

### 7.4 Provider SII de anexos

`SiiAnexosEnlaseClient` utilizaría la autenticación y el transporte compartidos, pero tendría endpoints, modelos, mapper y validadores propios.

No debe conocer:

- Variables de sesión HTTP.
- Consultas SQL.
- Reglas de asignación de Workflow.
- Detalles del almacenamiento documental.

### 7.5 Repositories

Puertos esperados:

- `WorkflowContextRepository`
- `DocumentTypeRepository`
- `ImportIntentRepository`
- `DocumentRepository`
- `ImportItemStatusRepository`
- `RelatedDocumentRepository`
- `AuditRepository`
- `AssignmentRepository`

## 8. Contexto seguro

El navegador debe enviar únicamente identidades mínimas, por ejemplo `taskId`, `providerId`, `capability`, `operationId` y `correlationId`.

El servidor debe reconstruir y verificar:

- Usuario y grupo.
- Tarea y actividad.
- Ruta y trámite.
- Tipo de actividad `ENLASE`.
- Código de barras.
- Recibo o radicado SII.
- Gabinete.
- Configuración de la integración.
- Estado de preasignación.

La sesión legacy puede utilizarse inicialmente como dato de compatibilidad, pero no debe ser la única fuente autoritativa después de abrir el modal.

## 9. Flujo técnico objetivo

```text
Usuario selecciona tarea ENLASE
        |
        v
Servidor valida contexto de preasignación
        |
        v
Resolver capacidad ANEXOS_RADICADO_ENLASE
        |
        v
Consultar anexos en el servicio SII específico
        |
        v
Normalizar identidades y estados
        |
        v
Verificar registro lógico y existencia física
        |
        +---- documento existente ----> IMPORTADO
        |
        +---- documento ausente ------> DISPONIBLE
        |
        v
Mostrar tabla, selección y vista previa
        |
        v
Validar tipologías y preparar intención
        |
        v
Confirmar importación
        |
        v
Descargar, validar y almacenar cada recurso
        |
        v
Registrar identidad externa y auditoría
        |
        v
Actualizar documentos relacionados
        |
        v
Revalidar documentos obligatorios
        |
        +---- incompletos ----> impedir asignación
        |
        +---- completos ------> habilitar asignación
```

## 10. Decisiones y reglas críticas

1. Importar no equivale a asignar la tarea.
2. La capacidad solo es válida en el contexto de preasignación `ENLASE`.
3. La identidad de un anexo debe ser estable y provenir del SII.
4. La idempotencia debe incluir al menos tarea, proveedor, capacidad e identidad externa.
5. Un registro lógico no demuestra que el documento siga existiendo físicamente.
6. Si el archivo fue eliminado del repositorio, el anexo puede volver a quedar disponible.
7. La tipología predeterminada solo puede aplicarse cuando la configuración sea inequívoca.
8. Un resultado incierto debe reconciliarse antes de reintentar para evitar duplicados.
9. El modal debe permanecer abierto cuando exista un error que el usuario necesite revisar.
10. La asignación debe revalidar contexto y documentos obligatorios, aunque la importación haya terminado correctamente.

## 11. Riesgos identificados

- Dependencia extensa de variables de sesión legacy.
- Respuestas legacy basadas en cadenas delimitadas y textos libres.
- Descarga y persistencia acopladas en una sola función.
- Posibles duplicados cuando la persistencia se completa pero se pierde la respuesta.
- Falta de identidad externa explícita o estable para algunos anexos.
- Diferencias entre el registro lógico y la existencia física del documento.
- Cambios en la tarea mientras el modal permanece abierto.
- Aplicación errónea de la tipología predeterminada.
- Confusión entre código de barras, recibo y radicado SII.
- Riesgo de habilitar la asignación con documentos obligatorios incompletos.

## 12. Información que debe confirmarse antes de implementar

- URL y operación exactas para consultar anexos.
- URL y operación exactas para descargar o previsualizar el recurso.
- Contrato real de solicitud y respuesta.
- Campo que identifica inequívocamente cada anexo.
- Semántica de versiones o modificaciones del anexo.
- Tamaño y formatos admitidos.
- Reglas de tipología documental por ruta y trámite.
- Comportamiento requerido cuando hay varios anexos.
- Reglas exactas de documentos obligatorios antes de asignar.
- Política ante documentos previamente importados y posteriormente eliminados.
- Compatibilidad requerida con el popup legacy durante la transición.

## 13. Pruebas recomendadas

### 13.1 Unitarias

- Despacho correcto por capacidad.
- Rechazo de capacidad incompatible con la actividad.
- Mapeo del contrato SII de anexos.
- Idempotencia e identidad externa.
- Aplicación segura de tipología predeterminada.
- Normalización de errores externos.

### 13.2 Integración

- Reconstrucción del contexto de tarea.
- Consulta del catálogo documental.
- Creación y recuperación de intención.
- Almacenamiento y verificación física.
- Actualización de documentos relacionados.
- Telemetría separada por operación.

### 13.3 E2E

- Tarea `ENLASE` con un anexo.
- Tarea con múltiples anexos y selección total.
- Vista previa.
- Tipología predeterminada y corrección manual.
- Importación completa, parcial y fallida.
- Resultado incierto y reconciliación.
- Documento eliminado físicamente que vuelve a estar disponible.
- Bloqueo de asignación por documentos obligatorios.
- Asignación exitosa después de completar documentos.
- Comportamiento adaptable del modal y las tablas.

Las pruebas autenticadas deben seguir el runbook E2E del repositorio y requerir autorización expresa. Al finalizar, el gate temporal debe restaurarse a su estado seguro.

## 14. Próximo paso recomendado

Iniciar cada entrega mediante el flujo propio `opsxj:new -- <TICKET>`. No se debe crear ni administrar un cambio OpenSpec manual para esta modernización.

Los prompts de implementación están definidos en `../Prompt/` y cubren:

1. Capacidad, consulta y preview de anexos SII.
2. Preparación, persistencia y reconciliación.
3. Interfaz moderna e integración con la asignación.
4. Pruebas, gate, compatibilidad y cierre OPSXJ.

La implementación debe comenzar únicamente cuando los contratos externos y las decisiones abiertas de este documento estén confirmados.

## 15. Matriz de prompts y pruebas E2E reales

| Prompt | Intención principal | ¿E2E real? | Cobertura E2E recomendada | Reutilización obligatoria |
|---|---|---:|---|---|
| **01 — Capacidad, consulta y preview** | Registrar `ANEXOS_RADICADO_ENLASE` dentro de `INTEGRACIONSII`; consultar anexos y ofrecer preview seguro sin mutaciones. | **Sí, de lectura** | Tarea `ENLASE` válida; cero, uno y múltiples anexos; consulta al SII real; preview; descriptor vencido; rechazo por contexto inválido; demostrar `sinCambios=SI`. | Extender `tools/e2e`, su autenticación, perfiles runtime, validadores de integridad y evidencia saneada. |
| **02 — Preparación, persistencia y reconciliación** | Preparar uno o varios anexos, crear una intención idempotente, almacenarlos y reconciliar resultados. | **Sí, mutadora** | Tipología válida; importación individual y múltiple; documento visible; repetición sin duplicado; resultado parcial; pérdida de respuesta; recuperación; existencia física; tarea no asignada automáticamente. | Reutilizar escenarios de ejecución y recuperación de `ImportarServicioWeb`, repositorios de evidencia y consultas de control `SELECT`. Requiere datos descartables y autorización mutadora expresa. |
| **03 — Interfaz moderna e integración con asignación** | Integrar modal moderno, selección total, preview, tipologías, resultados, cierre controlado y habilitación de `Asignar`. | **Sí, funcional UI** | Flujo completo en navegador; múltiples filas; seleccionar y deseleccionar todas; tipología predeterminada; preview; modal adaptable; scroll; error visible; cierre; refresco documental; bloqueo y habilitación de asignación; cambio de tarea. | Ampliar la especificación Playwright existente. No crear otro login, proyecto, configuración, `.env` ni arnés. Reutilizar datos y evidencia de los Prompts 01 y 02. |
| **04 — Pruebas, gate, compatibilidad y cierre** | Consolidar validaciones, gate, autorización, regresión legacy, restauración, rollout y cierre OPSXJ. | **Sí, obligatoria** | Gate activado y desactivado; acceso directo autorizado y no autorizado; lectura sin mutación; ejecución autorizada; concurrencia; regresión de constancias y legacy; integridad del proveedor; restauración final del ambiente. | Orquestar las pruebas creadas en los tres prompts anteriores. No repetir corridas sin necesidad ni crear perfiles paralelos. |

### 15.1 Secuencia de ejecución

```text
Prompt 01
└── E2E real de lectura y preview

Prompt 02
└── E2E real mutadora y recuperación

Prompt 03
└── E2E real del recorrido completo de interfaz

Prompt 04
└── Reutiliza y consolida las evidencias anteriores
    + gate
    + autorización
    + regresión
    + restauración
```

Todos los prompts ameritan pruebas E2E reales porque atraviesan límites que las pruebas aisladas no pueden demostrar: sesión WebForms, tarea `ENLASE`, servicio SII, almacenamiento documental, actualización visual y gate. El Prompt 04 debe reutilizar las corridas anteriores y ejecutar solamente los casos transversales faltantes.

### 15.2 Reglas operativas

- Toda corrida real requiere autorización explícita para ambiente, cuentas y datos.
- Antes de autenticar se deben leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`.
- Se debe reutilizar exclusivamente la infraestructura de `tools/e2e`.
- No se pueden crear login, proyecto Playwright, configuración, `.env`, arnés o sistema de evidencia paralelos.
- No deben exponerse credenciales, cookies, tokens ni cadenas de conexión.
- Las verificaciones de control deben ser únicamente `SELECT`.
- Las pruebas de lectura deben demostrar ausencia de mutaciones.
- Las pruebas mutadoras requieren datos descartables y autorización expresa.
- El gate y su alcance de usuarios y grupos deben restaurarse y verificarse al terminar.
- La evidencia debe quedar saneada y asociada al ticket OPSXJ correspondiente.

## 16. Matriz de elementos a reutilizar

| Elemento existente | Ubicación principal | Reutilización prevista | Prompt | Adaptación necesaria | Riesgo |
|---|---|---|---:|---|---|
| Registro de proveedores | `Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb` | Conservar `INTEGRACIONSII` como proveedor único | 01 | Resolver la nueva capacidad sin registrar otro `ProviderId` | Medio |
| Proveedor SII moderno | `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb` | Compartir autenticación y despachar por capacidad | 01 | Incorporar `ANEXOS_RADICADO_ENLASE` sin afectar constancias | Alto |
| Cliente HTTP SII | `SiiExternalImportProviderClient.vb` | Reutilizar transporte, timeout, cancelación y seguridad | 01 | Extraer o compartir autenticación; agregar cliente específico de anexos | Medio |
| Autenticación SII | Configuración legacy utilizada por `ResolveProvider` | Reutilizar empresa, usuario, contraseña y token | 01 | Evitar solicitudes duplicadas de token y separar endpoints | Alto |
| Transporte HTTP seguro | `Infrastructure/Workflow/ImportarServicioWeb/Http/` | Reutilizar factory, validación HTTP y mapeo de errores | 01 | Configurar operaciones y hosts autorizados para anexos | Medio |
| Contratos de proveedor | `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb` | Reutilizar interfaces de consulta, preview y descarga | 01 | Añadir capacidad o contexto operacional sin romper contratos actuales | Alto |
| Modelos de elementos externos | `ImportarServicioWebModels.vb` | Representar cada anexo como elemento externo | 01 | Agregar metadatos específicos sin contaminar el núcleo con campos SII | Medio |
| Contexto moderno | `ContextoImportacionServicio` | Reutilizar usuario, grupo, tarea, ruta, trámite y proveedor | 01–02 | Incorporar contexto de preasignación `ENLASE` y capacidad | Alto |
| Telemetría externa | `MySqlExternalServiceTelemetryRepository.vb` | Mantener el proveedor y diferenciar por operación | 01 | Registrar `QUERY/PREVIEW/DOWNLOAD_ANEXOS_ENLASE` | Bajo |
| Preview mediado | `Infrastructure/Workflow/ImportarServicioWeb/Preview/` | Reutilizar descriptor temporal, autorización y expiración | 01 | Resolver contenido mediante identidad del anexo | Medio |
| Streaming seguro | Handler moderno de preview y descarga | Reutilizar entrega de bytes sin JSON/base64 | 01 | Admitir formatos y referencias del servicio de anexos | Medio |
| Consulta legacy de anexos | `Integracionccv/ClassListaAnexosSII.vb` | Usar como fuente de caracterización funcional | 01 | Extraer contrato sin conservar dependencia directa de sesión | Alto |
| Servicio legacy SII | `webservice/WebService_integracion_sii.asmx.vb` | Conservar durante transición y usar para comparación | 01–04 | No consumir su respuesta Bootstrap desde la interfaz moderna | Medio |
| Catálogo de tipologías | Repositorios modernos de `ImportarServicioWeb` | Reutilizar catálogo autorizado por tarea y trámite | 02 | Resolver equivalencia con checklist y tipo documental de `ENLASE` | Alto |
| Servicio de preflight | Servicios modernos de preflight | Validar requisitos antes de cualquier escritura | 02 | Crear plan específico sin efectos de expedientes innecesarios | Medio |
| Repositorio de intenciones | `MySqlImportIntentRepository.vb` | Crear o recuperar una intención para toda la selección | 02 | Incorporar capacidad y contexto de preasignación a la huella | Alto |
| Control de concurrencia | `MySqlImportIntentConcurrencyGuard` | Evitar ejecuciones duplicadas | 02 | Ajustar clave para tarea, capacidad y anexos seleccionados | Alto |
| Máquina de estados | `FaseImportacionServicio` y orquestador moderno | Reutilizar estados confirmados, fallidos, parciales e inciertos | 02 | Evitar fases exclusivas de expedientes cuando no apliquen | Medio |
| Orquestador moderno | `ImportServiceOrchestrator` | Mantener un único ejecutor backend | 02 | Incorporar una estrategia de ejecución específica para anexos | Alto |
| Descarga moderna | `IExternalImportProviderClient.DownloadAsync` | Descargar el anexo seleccionado | 02 | Resolver la referencia sin confiar en URLs del navegador | Medio |
| Almacenamiento legacy | `ClassAlmacenamiento.PreAlmacenaDocumentoAnexosEnlaceIntegracionSII` | Encapsularlo detrás de un adaptador | 02 | Proporcionar contexto explícito y traducir retornos legacy | **Muy alto** |
| Almacenamiento documental común | `AlmacenaDocumentoTareaWorkflow(...)` | Reutilización indirecta mediante el adaptador legacy | 02 | No modificar su firma ni sus consumidores | **Muy alto** |
| Verificación física | Repositorios modernos de estado documental | Confirmar que el documento continúa existiendo | 02 | Relacionar identidad externa, registro lógico y archivo físico | Alto |
| Reconciliación | `ServicioReconciliacionImportacion` y repositorios asociados | Recuperar resultados inciertos o respuestas perdidas | 02 | Consultar por tarea, capacidad e identidad del anexo | Alto |
| Auditoría de transiciones | Auditoría moderna de importación | Registrar fases y resultados saneados | 02 | Añadir operaciones de anexos sin datos sensibles | Bajo |
| Núcleo frontend moderno | `js/workflow/importar-servicio-web/` | Reutilizar estados, API, UI y registro de adaptadores | 03 | Crear adaptador `ENLASE`, no otra aplicación completa | Medio |
| Cliente API moderno | `importar-servicio-web-api.js` | Centralizar todas las llamadas modernas | 03 | Incorporar capacidad y contexto requerido por anexos | Bajo |
| Registro frontend de adaptadores | `importar-servicio-web-provider-registry.js` | Resolver comportamiento por proveedor y capacidad | 03 | Evitar registrar dos adaptadores con identidad conflictiva | Medio |
| Modal y estilos modernos | `workflow/Webworkflow.aspx` y `Styles/importar-servicio-web-modern.css` | Reutilizar layout, accesibilidad y comportamiento adaptable | 03 | Integrarlo dentro del flujo previo a asignación | Medio |
| Tabla y selección múltiple | Módulos modernos de listado y preparación | Reutilizar selección individual y total | 03 | Respetar estados importables de anexos | Bajo |
| Vista previa frontend | `importar-servicio-web-preview.js` | Reutilizar panel, foco, estados y fallback | 03 | Consumir descriptores de anexos | Bajo |
| Preparación frontend | `importar-servicio-web-preparation.js` | Reutilizar flujo individual/múltiple y tipologías | 03 | Presentar requisitos propios de `ENLASE` | Medio |
| Resultados frontend | Adaptador y vista de progreso modernos | Reutilizar espera global y resultados por elemento | 03 | No utilizar `JSProgresBar` ni progreso ficticio | Bajo |
| Lista documental | `importar-servicio-web-document-list-adapter.js` | Incorporar documentos confirmados sin duplicarlos | 03 | Actualizar la lista del popup `ENLASE`, no otra tarea | Alto |
| Protección de contexto | `importar-servicio-web-task-context-guard.js` | Evitar cambios silenciosos de tarea | 03 | Considerar estado de preasignación y popup abierto | Alto |
| Recuperación frontend | `importar-servicio-web-recovery.js` | Consultar intención después de pérdida de conexión | 03 | Recuperar siempre contra la tarea original | Medio |
| Validación de asignación | Flujo `Buttonaceptar_Click` y validadores de documentos obligatorios | Mantener la segunda fase de asignación | 03 | Crear un puente moderno que revalide antes de habilitar | **Muy alto** |
| Gate moderno | `WorkflowCentroTrabajoModernActive` | Alternancia reversible durante transición | 04 | Confirmar si admite alcance independiente para `ENLASE` | Alto |
| Infraestructura de pruebas focales | `Tests/importar-servicio-web-*.test.cjs` | Reutilizar harness, convenciones y fixtures | 01–04 | Añadir fixtures y casos de anexos sin copiar suites | Bajo |
| Fixtures compartidos | `Tests/Fixtures/Workflow/ImportarServicioWeb/` | Modelar consulta, preview, intención y resultado | 01–04 | Crear subcontratos versionados para anexos | Bajo |
| Validación local | `tools/validation/` | Reutilizar verificadores deterministas sin red | 04 | Incorporar arquitectura y regresión `ENLASE` | Bajo |
| Plataforma E2E | `tools/e2e/` | Reutilizar login, perfiles, escenarios, evidencia y saneamiento | 04 | Agregar escenarios de lectura, ejecución, recuperación y UI | Medio |
| Runbook E2E | `tools/e2e/AGENT-RUNBOOK.md` | Mantener autorizaciones y restauración segura | 04 | Ninguna adaptación funcional; cumplimiento obligatorio | Bajo |
| Evidencia técnica | Paquetes DOC-56 y DOC-79 | Reutilizar estructura de evidencia y controles | 04 | Crear evidencia propia del ticket sin copiar resultados | Bajo |
| Rollout y rollback | Gate y recorrido legacy existentes | Mantener transición reversible | 04 | Documentar criterios para ocultar o retirar legacy | Alto |

### 16.1 Clasificación de reutilización

| Tipo | Significado | Ejemplos |
|---|---|---|
| **Reutilización directa** | Se consume sin cambiar su responsabilidad | Transporte HTTP, telemetría, cliente API, estilos y E2E |
| **Reutilización por extensión** | Se agrega una capacidad conservando contratos existentes | Proveedor SII, contexto, preview y UI |
| **Reutilización mediante adaptador** | Se encapsula una función legacy detrás de un contrato moderno | Almacenamiento y actualización de documentos |
| **Solo caracterización** | Se estudia para preservar reglas, pero no se expone al diseño moderno | ASMX legacy, sesión y respuestas Bootstrap |
| **Conservación temporal** | Permanece disponible para rollback | Popup, endpoints y handlers legacy |

### 16.2 Componentes críticos

Los elementos de mayor riesgo son:

- El almacenamiento legacy.
- La verificación de existencia física.
- La reconciliación de resultados inciertos.
- La validación anterior a la asignación.

Estos componentes justifican separar la preparación e idempotencia de la persistencia y reconciliación en unidades de implementación distintas, aunque permanezcan agrupados bajo una planificación OPSXJ común.
