<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->

# Diseño técnico — DOC-11: transición segura

## Objetivo

Agregar EjecutarEnvioTarea al ASMX moderno existente. El navegador solo solicita una transición; el servidor reconstruye el contexto, autoriza el destino, serializa la operación y delega el cambio efectivo al motor legacy.

## Límites preservados

- No se modifica workflow/Webworkflow.aspx ni workflow/Webworkflow.aspx.vb.
- No se modifica workflow/ClassWorkflow.vb ni se reimplementa Terminar_Tarea_Workflow o Cambia_Estado.
- JavaScript, ASMX, Application y repositorios no llaman Terminar_Tarea_Workflow ni Cambia_Estado.
- WorkflowLegacyExecutorAdapter es el único adaptador que puede invocar Terminar_Tarea_Workflow; Cambia_Estado sigue siendo una llamada interna de ese método legacy.
- No se abre una transacción nueva para cambiar estados Workflow.

## Flujo de componentes

1. WebServiceWorkflowModern.ASMX recibe idTarea, idConector y tokenVersion con sesión habilitada.
2. WorkflowPreviewSessionContextGate crea el contexto de ejecución completo desde la sesión Gestión ya autenticada, incluida la inicialización de permisos que requiere el motor legacy.
3. ServicioTransicionTarea aplica feature gate, valida forma de solicitud y relee la tarea asignada al usuario actual.
4. El guard de concurrencia obtiene un bloqueo de MySQL por tarea y versión; dentro de él se releen tarea y destino.
5. El repositorio de ejecución resuelve para RUTA o FLUJO los argumentos autorizados del conector, no el DTO mostrado en preview.
6. El servicio valida requisitos y llama EjecutorTransicionTarea.
7. WorkflowLegacyExecutorAdapter ejecuta la única llamada a Terminar_Tarea_Workflow, sin actualizar controles Web Forms y conservando los eventos y efectos legacy.
8. El servicio normaliza el resultado, registra auditoría y devuelve ResultadoTransicionDto.

## Decisiones

### D-01 — Un endpoint, composición segura

El método se agrega a webservice/WebServiceWorkflowModern.asmx.vb con EnableSession y ResponseFormat Json. El ASMX no recibe datos de usuario, ruta, grupo, actividad destino, conexión ni permisos. Ante una excepción devuelve ResultadoTransicionDto con código WORKFLOW_TRANSITION_UNAVAILABLE y no la excepción.

### D-02 — Contexto de ejecución completo

El gate de sesión existente tendrá una ruta explícita para ejecución. Comprueba la sesión Gestión, relación Gestión-Workflow, usuario, grupo, conexión Workflow y permisos mediante `SolicitaPermisosUsuarioWorkflow`; resuelve el nombre de ruta y conserva `SESIONCOMPILAR` y los eventos legacy ya preparados por el login Gestión. Solo si la sesión está incompleta prepara esos eventos con `InicioWorkflow.CompilaScriptUsuario`; no invoca `InicializaSesionModuloWorkflow`, porque esa rutina registra login y compone estado propio de la página Web Forms. Si falta un dato, limpia únicamente el contexto Workflow creado y retorna un contexto inválido. Preview sigue siendo de lectura y no invoca esta preparación.

### D-03 — Datos del navegador sin autoridad

La solicitud solo identifica la intención. ValidadorTransicionTarea exige idTarea positivo, idConector positivo y token no vacío. ITareaWorkflowRepository relee la tarea seleccionada del usuario actual y su token. Un destino de ejecución tipado contiene solo datos que salen de consultas servidoras: tipo, actividad real destino, usuario destino, flujo, actividad de flujo destino, usuario origen, actividad origen, conector y bandera de correo.

### D-04 — Resolución separada de RUTA y FLUJO

ITransicionEjecucionRepository resuelve el conector contra la tarea actual.

- RUTA valida grupo, ruta y actividad origen y recupera el conector de actividades_disponibles_envio, actividad siguiente y correo.
- FLUJO valida flujo, nodo/actividad origen y usuario/grupo fuente, recupera tanto la actividad real de listado_actividades_workflow como los identificadores de wf_registro_actividaes_flujos_trabajo requeridos por el motor.

Un conector encontrado fuera de este contexto produce WORKFLOW_CONNECTOR_UNAVAILABLE. Esta capa no recibe Session y usa IModuleConnectionFactory.

### D-05 — Único límite legacy

WorkflowLegacyExecutorAdapter recibe el contexto y el destino de ejecución ya autorizados. Es el único punto nuevo que crea ClassWorkflow y llama Terminar_Tarea_Workflow. Pasa Page=Nothing y activa_actualizacion_paramtros_interface=0 para que el servicio no manipule controles, cachés ni lista de tareas. Mantiene activa_evento_dinamicos=1, por lo que PRETERMINARACTIVIAD puede bloquear y TERMINARACTIVIDAD se conserva después del cambio. Correo, auditoría legacy y Cambia_Estado permanecen bajo la llamada legacy.

### D-06 — Requisitos previos y mensajes

Antes de la llamada efectiva, se conservan las verificaciones existentes de respuesta/confirmación, aprobaciones y autorizaciones. Firma, expediente, copia documental y balanceo se validan por las reglas actuales del motor legacy y se convierten a bloqueo funcional si impiden continuar. Ningún mensaje original de excepción, SQL o credencial pasa al DTO.

### D-07 — Concurrencia sin segunda transacción

ITransicionConcurrencyGuard mantiene una conexión MySQL y usa GET_LOCK con una clave determinista de tarea y versión. Mientras está retenido se vuelve a leer tarea y destino. Si no se adquiere, se devuelve WORKFLOW_TRANSITION_IN_PROGRESS; si el primer envío consumió la tarea, el segundo recibe WORKFLOW_VERSION_CONFLICT. Al liberar se ejecuta RELEASE_LOCK. El guard no escribe estados ni crea una transacción de negocio.

### D-08 — Resultado, auditoría e idempotencia

ResultadoTransicionDto se completa con Exito, EstadoFinal, MensajeFuncional, CodigoBloqueo, Advertencias, ActividadDestino, Destino, TokenVersion, ReferenciaAuditoria y EsReintentable. Los textos de resultado legacy se clasifican en éxito, rechazo funcional o indisponibilidad sin exponerse directamente. La auditoría registra usuario, tarea, origen, destino, mecanismo, fecha y resultado; la referencia se devuelve solo si no revela datos internos.

### D-09 — Verificación

Las pruebas unitarias cubren validación, mapeo, bloqueos y adaptación del resultado. Las integraciones prueban resolución de RUTA/FLUJO y el guard. QA/E2E ejercitan éxito, bloqueo y concurrencia. Una E2E de ejecución requiere tarea y cuentas descartables explícitamente autorizadas; nunca se usa una tarea real de operación solo para validar.

### D-10 — Piloto y reversa

El gate WorkflowCentroTrabajoModernActive se mantiene false por defecto. El piloto se limita a usuarios/grupos de configuración y se revierte poniendo el gate en false, sin revertir estados ni alterar el camino Web Forms.

## Alternativas descartadas

| Alternativa | Motivo de descarte |
| --- | --- |
| Llamar Terminar_Tarea_Workflow desde ASMX o ServicioTransicionTarea | Rompe el límite único y mezcla Presentation/Application con el núcleo legacy. |
| Usar el DestinoTransicionDto de preview como orden de ejecución | El DTO no contiene todos los identificadores de FLUJO ni es una autorización vigente. |
| Crear una transacción nueva que actualice estados_tarea_workflow | Duplicaría Cambia_Estado y podría dejar los dos caminos inconsistentes. |
| Lock en memoria del proceso | No protege instalaciones con más de una instancia Web Forms. |
| Ejecutar sin inicializar permisos de sesión | El motor legacy usa valores de sesión además del usuario, grupo y ruta. |

## Riesgos y mitigaciones

| Riesgo | Mitigación |
| --- | --- |
| Reglas legacy con efectos colaterales no visibles | Conservar el motor, eventos y correo; probar primero en un piloto aislado. |
| El adaptador recibe texto legacy no apto para JSON | Normalizar y no retornar el texto original. |
| Lock abandonado por error | Usar Using/Finally para RELEASE_LOCK y cierre de conexión. |
| Una tarea cambia entre preview y envío | Token, releída dentro del lock y conflicto controlado. |
