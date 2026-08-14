<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## Context

DOC-10 implementa el Prompt 02 después de la fundación entregada por DOC-9. Se necesita un endpoint ASMX paralelo que previsualice destinos de una tarea sin modificar el comportamiento vigente de `workflow/Webworkflow.aspx` ni ejecutar la terminación legacy.

## Goals / Non-Goals

**Goals**

- Exponer `PreviewEnviarTarea(idTarea)` como contrato JSON de solo lectura.
- Resolver sesión, autorización, habilitación, tarea, flujo/ruta y destinos en servidor con capas tipadas.
- Implementar y ejecutar una E2E automatizada real contra el ASMX desplegado.

**Non-Goals**

- D-01 / RQ-01: terminar tareas, cambiar estado, enviar correo, firmar, disparar eventos dinámicos o modificar el flujo Web Forms existente.
- D-05 / RQ-05: aceptar usuario, grupo, ruta, actividad o permisos desde JavaScript.
- D-03 / RQ-03: crear un repositorio genérico o copiar clases/SQL legacy a Presentation.

## Decisions

### D-01 — Preview paralelo sin ejecución legacy

`WebServiceWorkflowModern.asmx` se limita a previsualizar. `ServicioTransicionTarea.Previsualizar` no compone `IWorkflowLegacyExecutor` ni usa `WorkflowLegacyExecutorAdapter`; tampoco puede invocar `Terminar_Tarea_Workflow`, `Cambia_Estado`, `PRETERMINARACTIVIAD` o `TERMINARACTIVIDAD`. El flujo actual continúa siendo dueño de la escritura, notificación y transacción. RQ-01 verifica ausencia de efectos secundarios.

### D-02 — Contexto de servidor y gate fail-closed

El borde ASMX construye `ContextoModuloWorkflow` a partir de la sesión autenticada y valida sus valores antes de llamar Application. `IWorkflowModernFeatureGate` se evalúa antes de recuperar la tarea; cualquier estado distinto de activo devuelve `WORKFLOW_MODERN_INACTIVE`, sin destinos ni fallback. RQ-02 impide que una invocación directa revele datos fuera del piloto.

### D-03 — Repositorios tipados de lectura por Workflow

`ITareaWorkflowRepository`, `ITransicionFlujoRepository` e `ITransicionRutaRepository` se implementan en `Infrastructure/Repositories/Workflow/`. Reciben `ContextoModuloWorkflow`, usan conexión, comandos y parámetros de `Infrastructure/Shared/Data`, y devuelven modelos tipados. Cada consulta limita datos a tarea activa y autorizada, origen, grupo y flujo/ruta reales. No leen sesión, devuelven `DataSet` ni construyen HTML. RQ-03 protege la autorización y el límite de dominio.

### D-04 — Orquestación y contrato seguro de preview

`ServicioTransicionTarea.Previsualizar` valida contexto y tarea, determina `FLUJO` o `RUTA`, pide destinos al repositorio apropiado y mapea una `PrevisualizacionTransicionDto`. El DTO contiene solo contexto seguro, destinos, destinatario/grupo cuando aplique, requisitos, notificación, token de versión y bloqueo funcional. Tipos desconocidos, tarea no disponible, ruta no disponible, conectores inconsistentes y colecciones vacías se representan mediante códigos y mensajes visibles no sensibles. RQ-04 define el contrato observable.

### D-05 — ASMX de presentación mínimo y seguro

Se crean `webservice/WebServiceWorkflowModern.asmx` y su code-behind con `ScriptService` y `WebMethod(EnableSession:=True)`. El método público recibe exclusivamente `idTarea`, compone feature gate, validadores y repositorios de lectura, y traduce errores inesperados a un bloqueo seguro. Los archivos se incluyen en `GestionDocumental-Docuarchi.net.vbproj`. RQ-05 conserva el aislamiento de Presentation.

### D-06 — Evidencia de integración real

Además de compilación y verificaciones focales, la entrega incorpora una E2E automatizada y ejecutada contra un ambiente de prueba con ASMX desplegado. La prueba usa un usuario autorizado y otro no autorizado, valida respuesta y ausencia de mutaciones antes/después. QA manual y documentos complementan la evidencia, pero no sustituyen la E2E. RQ-06 cubre el riesgo de integración de hospedaje, sesión y datos.

### D-07 — Bootstrap validado de contexto Workflow desde Gestión

El ASMX incorpora un `WorkflowPreviewSessionContextGate` de Presentation, distinto de `IWorkflowModernFeatureGate`. Para cada invocación desde una sesión Gestión autenticada, el gate vuelve a validar la relación de servidor `remit_dest_interno.Relacion_Workflow` y su login asociado, resuelve usuario, grupo y ruta mediante la consulta legacy estrictamente de lectura, y guarda exclusivamente esas claves de contexto en la sesión existente. No recibe credenciales ni identificadores del cliente, no llama `InicializaSesionModuloWorkflow`, no compila scripts y no registra auditoría. Si falta la sesión Gestión, la relación o la configuración de módulo Workflow, entrega un contexto inválido y el endpoint permanece fail-closed.

La composición del ASMX crea una factoría de conexión MySQL desde el snapshot de configuración de módulo ya contenido en la sesión autenticada. La factoría no conoce `HttpContext`; los repositorios continúan sin leer sesión y reciben esa dependencia por constructor. Esto evita que `MyDbContext` desvíe la lectura al catálogo Gestión.

### D-08 — Campos de asignación no son un bloqueo del preview

`TIPO_RUTA_ABIERTA_CERRADA` y `TIPO_ABIERTA_CERRADA_ACTIVIDAD` describen si el usuario puede asignar libremente a grupos o usuarios. No representan el estado de envío de una tarea. Por tanto, `MySqlTransicionFlujoRepository.ObtenerDestinos` no los consulta ni devuelve `WORKFLOW_FLOW_CLOSED` a partir de ellos: filtra únicamente conectores salientes del flujo, actividad de origen y usuario Workflow resueltos en servidor. DOC-10 sigue siendo solo lectura; la validación definitiva de cualquier envío continúa fuera de este endpoint.

### D-09 — Estado documental de ruta en su catálogo propietario

`tipo_doc_entrante` pertenece a Docuarchi, no al módulo Workflow. `WorkflowPreviewSessionContextGate` obtiene además el snapshot Docuarchi ya creado por el login Gestión, y el ASMX compone `DocuarchiModuleConnectionFactory` sin exponerlo al cliente. `MySqlTransicionRutaRepository` recibe ambas dependencias: Workflow para la tarea, la configuración de ruta y los destinos; Docuarchi exclusivamente para `estado_ruta_open_close`. Si el snapshot documental no está disponible, la ruta retorna su bloqueo funcional sin intentar consultar esa tabla en Workflow. Las factorías no conocen `HttpContext` y el repositorio no lee `Session`.

### D-10 — Carga autenticada antes de una decisión de asincronía

`tools/e2e/scripts/run-doc10-concurrency.cjs` crea una sesión Gestión aislada por usuario virtual con el login real, y después mide solo el POST al ASMX. El login se dosifica con paralelismo 5 por defecto y, una vez autenticadas, todas las sesiones llaman al ASMX simultáneamente. Así separa el costo del login de la concurrencia del endpoint. Los niveles por defecto son 20 y 30, con una solicitud por sesión para evitar que el bloqueo de sesión del mismo usuario distorsione la prueba. Cada nivel consulta huellas de estado y auditoría antes/después mediante `SELECT` parametrizados y guarda un resumen seguro de p50/p95/p99, fallos y mutación. El ejecutor no cambia el ASMX síncrono ni afirma que la asincronía sea necesaria: esa decisión queda sustentada por los resultados, los límites configurables y el monitoreo del servidor.

## Flujo de preview

1. El cliente envía únicamente `idTarea` a `PreviewEnviarTarea`.
2. El ASMX valida la sesión Gestión y su relación Workflow; resuelve y establece solo las claves de contexto relacionadas.
3. El ASMX crea las factorías snapshot de Workflow y, para rutas, de Docuarchi ya resueltas en la sesión.
4. Application evalúa el feature gate; si no está activo retorna bloqueo sin resolver datos.
5. El repositorio de tarea valida actividad y autorización y obtiene radicado, actividad origen, decisión y token.
6. Application delega a repositorio de flujo o ruta según la decisión real y filtra destinos permitidos; la ruta lee su estado en Docuarchi y sus destinos en Workflow; en flujo no interpreta campos de libertad de asignación como cierre.
7. Application devuelve DTO JSON o bloqueo funcional; el endpoint no escribe nada.

## Risks / Trade-offs

- Las consultas legacy mezclan controles y estado; replicar solo la lectura exige documentar fuente y filtros antes de codificar cada repositorio.
- La E2E requiere ambiente desplegado, cuentas y tareas controladas, pero es obligatoria porque las verificaciones locales no cubren sesión, ASMX ni configuración real.
- El feature gate puede bloquear a usuarios configurados incorrectamente; ese comportamiento fail-closed es intencional y reversible desde la configuración existente.
- La prueba de carga usa varias sesiones del piloto cuando no existe una cohorte de cuentas equivalente; mide concurrencia de aplicación y base de datos, pero no sustituye una prueba de permisos distribuidos ni métricas de infraestructura del servidor.

## Migration Plan

1. Implementar y verificar el preview manteniendo el gate inactivo por defecto.
2. Ejecutar E2E y QA con el piloto autorizado, sin modificar la tarea de prueba.
3. Mantener el endpoint paralelo; la ejecución moderna pertenece al Prompt 03 y no se incorpora en DOC-10.

## Open Questions

No hay decisiones de alcance abiertas. Los nombres exactos de tablas y columnas se documentarán junto a cada repositorio a partir de las fuentes legacy inspeccionadas, sin incluir SQL en el contrato público.
