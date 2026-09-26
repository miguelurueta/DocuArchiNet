<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-81-reconciliacion-persistencia-servicio-enlace-sii

## Fuente y alcance

- Ticket: `DOC-81` — RECONCILIACION-PERSISTENCIA-SERVICIO-ENLACE-SII.
- Cambio OPSXJ/OpenSpec: `doc-81-reconciliacion-persistencia-servicio-enlace-sii`.
- Perfil tecnológico: ASP.NET Web Forms/ASMX en VB.NET, servicios y repositorios modernos de `ImportarServicioWeb`, persistencia MySQL y almacenamiento documental legacy.
- Alcance: preparación, intención, ejecución, persistencia y reconciliación de anexos SII para la capacidad `ANEXOS_RADICADO_ENLASE` publicada por DOC-80.
- Fuera de alcance: asignar/cerrar la tarea, modificar la importación de constancias o crear una segunda ruta de almacenamiento documental.

## Contexto inspeccionado

- Contrato moderno: `webservice/WebServiceImportarServicioWebModern.asmx.vb`, `DTOs/Workflow/ImportarServicioWeb/`, `Modelo/Workflow/ImportarServicioWeb/` y `Services/Workflow/ImportarServicioWeb/`.
- Persistencia e idempotencia: `MySqlImportIntentRepository`, `MySqlImportIntentConcurrencyGuard`, `MySqlImportReconciliationRepository` y `MySqlImportDocumentTypeResolver`, bajo `Infrastructure/Repositories/Workflow/ImportarServicioWeb/`.
- Ruta legacy de anexos: `ClassAlmacenamiento.PreAlmacenaDocumentoAnexosEnlaceIntegracionSII` y `WebService_integracion_sii.SeviceGuardaDocumentoAnexoSII`.
- Evidencia funcional base: `Doc/Actualizacion/workflow/ImportarServiciWebEnlace/exploracion/modernizacion-importacion-anexos-sii-enlase.md` y el contrato de lectura/capacidad de DOC-80.
- Comportamiento que se preserva: campos documentales `CODBARRAS` y `ENLASE`, resolución MERCANTIL/ESAL/RUP, relación con la tarea y actualización de la imagen principal cuando corresponda. La asignación permanece fuera del importador.
- Hallazgo: la función legacy obtiene la tarea desde `ID_TAREA_SELECCIONDA_ENLACE`, descarga a una ruta temporal de sesión, llama `AlmacenaDocumentoTareaWorkflow`, actualiza `DAT_ADIC_TAR` cuando no existe imagen y retorna texto o `YES`. Ese retorno no basta como confirmación autoritativa y debe quedar detrás de un adaptador con verificación posterior.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Extender el contrato moderno de colección para preparar uno o varios anexos ENLASE con catálogo autorizado; la tipología predeterminada solo se aplica cuando la resolución es única. | `ServicioPreflightImportacion.Preflight`; `MySqlImportDocumentTypeResolver.Resolver`; DTOs de preflight e intención | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Crear o reutilizar una sola intención para toda la selección; su huella incluye contexto inmutable, proveedor, capacidad e identidades externas, y la ejecución queda serializada. | `ServicioIntencionImportacion.Crear`; `IImportIntentRepository`; `MySqlImportIntentConcurrencyGuard` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Revalidar en servidor usuario, tarea, actividad/ruta, trámite, proveedor y capacidad ENLASE antes del primer efecto y bloquear si el contexto cambió. | `ValidadorContextoImportacion.Validar`; `WorkflowImportSessionContextFactory`; endpoints de `WebServiceImportarServicioWebModern` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Encapsular `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII` en el puerto moderno de almacenamiento, traduciendo retornos legacy a un resultado estructurado y sin copiar su cuerpo ni llamarlo por HTTP interno. | `IImportDocumentStoragePort`; `ImportDocumentStorageStep.Ejecutar`; `ClassAlmacenamiento.PreAlmacenaDocumentoAnexosEnlaceIntegracionSII` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Un anexo solo queda confirmado cuando coinciden identidad externa, registro lógico, relación con la tarea y existencia física; un registro sin archivo se clasifica recuperable y no bloquea una nueva ejecución controlada. | `IImportReconciliationRepository`; `MySqlImportReconciliationRepository`; `ServicioReconciliacionImportacion`; `ImportItemResultMapper` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Persistir resultados por elemento y agregar el estado de la intención sin ocultar fallos; un resultado incierto no se reintenta automáticamente y se resuelve mediante reconciliación autorizada. | `ImportServiceOrchestrator.Execute`; `ImportIntentStateMachine.Intentar`; `ServicioReconciliacionImportacion.ReconcileImportIntent` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Mantener invariantes: no asignar tarea, no crear efectos de constancias, no exponer secretos/rutas/excepciones y validar con pruebas focales; E2E mutadora solo con autorización y datos descartables. | `ImportServiceOrchestrator`; capacidad existente de constancias; `tools/e2e/AGENT-RUNBOOK.md` | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Preflight acepta selección individual o múltiple bajo el mismo contrato y devuelve tipologías autorizadas. | Sin tipología o con predeterminado ambiguo bloquea sin escritura; una coincidencia única permite continuar. | No alterar catálogo ni preparación de constancias. |
| RQ-02 | Una selección produce una intención; repetición o concurrencia no duplica documentos. | La misma clave/huella reutiliza intención y dos solicitudes concurrentes producen una ejecución efectiva o respuesta idempotente. | La URL temporal nunca participa en la clave. |
| RQ-03 | El servidor rechaza contexto ENLASE inválido o cambiado antes de descargar/persistir. | Cualquier diferencia de tarea, ruta, trámite, proveedor o capacidad devuelve error seguro y cero efectos. | No confiar en campos libres del navegador. |
| RQ-04 | Cada anexo se descarga, valida y almacena por un adaptador moderno que reutiliza la única función legacy. | Fallo previo a persistencia queda `Fallido`; retorno legacy se traduce en backend y no implica éxito por sí solo. | Preservar campos y efectos legacy caracterizados. |
| RQ-05 | La confirmación exige evidencia lógica y física y conserva el ID interno autorizado. | Registro y archivo presentes ⇒ importado/idempotente; registro sin archivo ⇒ recuperable; evidencia contradictoria ⇒ incierto/inconsistente. | Evitar falsos duplicados y reimportación destructiva. |
| RQ-06 | La respuesta diferencia éxito, omisión idempotente, fallo, parcial e incierto por elemento. | Si algún elemento falla, queda incierto o no se procesa, la intención no anuncia éxito total; el incierto requiere reconciliación explícita. | No reintentar automáticamente después de un posible efecto. |
| RQ-07 | La tarea sigue sin asignarse y los flujos de constancias/legacy conservan su contrato. | Suites focales y caracterización cubren preparación, concurrencia, storage, reconciliación y antirregresión; E2E real respeta autorización. | Rollback: retirar composición ENLASE moderna sin modificar la función legacy. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` está desarrollada en `design.md`, reflejada en `spec.md` y vinculada en `tasks.md` mediante `Origen: D-XX, RQ-XX`.
2. La implementación debe registrar toda desviación descubierta entre el contrato DOC-80 y la caracterización legacy antes de modificar diseño o código.
3. Las consultas E2E de control serán solo `SELECT`; credenciales, cookies, rutas físicas y excepciones crudas no formarán parte de la evidencia.

## Resultado del refinamiento

- Estado: aprobado para implementación.
- La aprobación habilita el trabajo técnico, no autoriza por sí sola una corrida E2E mutadora ni la activación del gate.
- Comando de verificación: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-81 --sync`.