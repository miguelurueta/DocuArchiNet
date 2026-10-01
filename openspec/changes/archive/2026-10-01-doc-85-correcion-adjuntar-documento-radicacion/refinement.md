<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-85-correcion-adjuntar-documento-radicacion

## Fuente y alcance

- Ticket: `DOC-85` — CORRECION-ADJUNTAR-DOCUMENTO-RADICACION.
- Cambio OpenSpec: `doc-85-correcion-adjuntar-documento-radicacion`.
- Fuente Jira: `specs/correcion-adjuntar-documento-radicacion/jira-context.md`.
- Perfil tecnológico verificado: ASP.NET Web Forms, VB.NET sobre .NET Framework 4.6.1, JavaScript legacy y MySQL.
- Guía arquitectónica oficial: `Doc/Actualizacion/RadicacionSimplificada/Adjunta/Exploracion/01-exploracion-consolidada-contexto-radicado-adjunto.md`.

El alcance se limita a corregir la pérdida del radicado autoritativo en la rama `ADJUNTARADICACION` y la proyección de su nueva fila revelada por la E2E. No se modifica el transporte compartido ni los recorridos legacy de otros eventos; la bifurcación JavaScript queda delimitada por el evento de Radicación.

## Contexto inspeccionado

- `workflow/ClassAlmacenamiento.vb`: `UploadSaveFile`, `PreAlmacenaDocumentosRadicacion`, `AlmacenaDocumentosRadicacion` y `Almacenamiento`.
- `Docuarchi/ClassDaGabinete.vb`: `SolicitaDatosCamposIndiceGabinete` y consulta de plantilla.
- `workflow/Class_DAT_ADIC_TAR.vb`: `SolicitaRadicadoTareaWorkflow` puede retornar `YES` dejando el radicado vacío.
- `radicador/Class_ra_rad_estados_modulo_radicacion.vb`: el registro seleccionado contiene `consecutivo_radicado`.
- `generic_control/FileUploadHandler_.ashx.vb`: la rama actual ya envía los doce argumentos y mapea `stru_datos_image_lista` a `uploadFiles`.
- `generic_control/FileUploadHandler.js` y `js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js`: el cliente ya transporta el identificador de estado; la E2E comprobó que la rama compartida requería insertar, no actualizar, la fila de `ADJUNTARADICACION`.
- Implementación legacy de referencia: la identidad del radicado se obtiene del registro de estado y se entrega explícitamente al almacenamiento, sin redescubrirla desde `DAT_ADIC_TAR`.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Resolver en servidor el radicado autoritativo desde `IdRegistroEstadoRadicacion` y `ra_rad_estados_modulo_radicacion.consecutivo_radicado`; el valor del navegador solo detecta discrepancias. | `radicador/Class_ra_rad_estados_modulo_radicacion.vb`; recorrido `ADJUNTARADICACION` en `workflow/ClassAlmacenamiento.vb` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Materializar una sola vez un `ContextoAdjuntoRadicacion` inmutable mediante repository parametrizado y coordinarlo desde `ServicioAdjuntoRadicacion`. | Nuevos componentes bajo `Modelo`, `Infrastructure/Repositories` y `Services/RadicacionSimplificada/Adjuntos` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Construir plantilla e índices con el radicado ya validado, sin llamar de nuevo `SolicitaRadicadoTareaWorkflow`, y reutilizar el almacenamiento existente. | `Docuarchi/ClassDaGabinete.vb`; `workflow/ClassAlmacenamiento.vb`; `workflow/Class_DAT_ADIC_TAR.vb` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Separar `UploadSaveFile` en sobrecargas inequívocas de diez y doce argumentos y conservar el recorrido legacy para cualquier evento distinto de `ADJUNTARADICACION`. | Llamadas y firmas de `UploadSaveFile` en `workflow/ClassAlmacenamiento.vb` y handler existente | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Mantener sin cambios el handler y el JavaScript propio del módulo; limitar la inserción nueva en el JavaScript compartido a `ADJUNTARADICACION`, preservando el recorrido legacy en `else` y sin recarga. | `generic_control/FileUploadHandler_.ashx.vb`, `generic_control/FileUploadHandler.js`, `js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Demostrar aislamiento con pruebas locales, compilación y E2E real positiva/negativa; la E2E solo se ejecuta con autorización y mantiene el gate en `false` con listas vacías. | `tests`, `tools/e2e`, `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Componer el repository exclusivamente con el resolver de sesión, la factoría de Radicación y el ejecutor ADO.NET compartidos; no mantener una fábrica privada. | `Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb`, `Infrastructure/Shared/Data/WorkflowModuleConnectionFactory.vb`, `Infrastructure/Shared/Data/AdoNetDataInfrastructure.vb` y `workflow/ClassAlmacenamiento.vb` | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El documento usa el `consecutivo_radicado` persistido aunque `DAT_ADIC_TAR` no entregue radicado. | GIVEN un registro autorizado con radicado y una tarea sin ese dato, WHEN se adjunta, THEN plantilla, índices, almacenamiento y respuesta usan el valor del registro. | Se rechazan identificador inválido, registro ajeno, radicado autoritativo vacío y discrepancia del valor informativo. |
| RQ-02 | Cada carga lógica resuelve un único contexto tipado, autorizado e inmutable mediante SQL parametrizado. | WHEN el servicio recibe la solicitud, THEN consulta una vez el registro y entrega el mismo contexto durante el recorrido completo. | No se usa `Session`, estado estático ni controles Web Forms como autoridad. |
| RQ-03 | La preparación específica no redescubre el radicado desde la tarea. | WHEN se construyen plantilla e índices, THEN el constructor recibe un radicado obligatorio y no referencia `SolicitaRadicadoTareaWorkflow`. | `AlmacenaDocumentosRadicacion` y `Almacenamiento` conservan su contrato. |
| RQ-04 | Las llamadas de diez argumentos siguen el flujo legacy y la llamada de doce argumentos entra exclusivamente al servicio nuevo. | WHEN compila cada consumidor existente, THEN la resolución de sobrecarga es inequívoca y no hay argumentos opcionales del contexto nuevo. | No cambian `GESTION_RESPUESTA`, `WORKFLOWSELECCION`, `PRODUCCION`, `ENLACE_RADICADO`, SII ni sellos. |
| RQ-05 | El control compartido conserva contrato HTTP y comportamiento visual. | WHEN finaliza una carga válida, THEN `uploadFiles` mantiene sus campos y la fila aparece mediante inserción JavaScript sin postback, `DataBind` ni recarga. | Handler y JavaScript del módulo quedan intactos; la bifurcación compartida solo admite `ADJUNTARADICACION` y conserva el resto en `else`. |
| RQ-06 | La corrección cuenta con evidencia reproducible local y E2E real controlada. | WHEN se valida el cambio, THEN aprueban pruebas focales, regresión, compilación y los escenarios E2E autorizados positivo y negativo. | Sin autorización o recurso descartable se registra bloqueo operacional y no se declara el ticket cerrado. |
| RQ-07 | La ruta productiva reutiliza la infraestructura transversal existente. | WHEN se compone el servicio, THEN `ClassAlmacenamiento` usa `ModuleSessionConnectionStringResolver` e inyecta `RadicacionModuleConnectionFactory` y `AdoNetDataExecutor`. | El repository no conoce `HttpContext`, credenciales ni una fábrica de conexión privada. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` está desarrollada en `design.md`, reflejada en `spec.md` y vinculada a tareas mediante `Origen: D-XX, RQ-XX`.
2. Toda tarea con checkbox conserva un origen válido, incluidas validación y documentación.
3. El refinamiento no autoriza por sí mismo la ejecución de E2E autenticada ni mutaciones sobre ambientes externos.
4. No se introduce feature gate ni se modifica `WorkflowCentroTrabajoModernActive`; su estado final debe ser `false` con usuarios y grupos vacíos.

## Resultado del refinamiento

- Estado: aprobado para planificación e implementación controlada.
- La ejecución de E2E real queda sujeta a autorización explícita del ambiente, cuenta, registro descartable, fixture, tipología y consultas `SELECT` de evidencia.
- Sincronización: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-85 --sync`.
