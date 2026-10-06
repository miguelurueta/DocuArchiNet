<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-88-actualizacion-carga-documentos-produccion-documental

## Fuente y alcance

- Ticket: `DOC-88` — ACTUALIZACION-CARGA-DOCUMENTOS-PRODUCCION-DOCUMENTAL.
- Cambio OpenSpec: `doc-88-actualizacion-carga-documentos-produccion-documental`.
- Fuente Jira: `specs/actualizacion-carga-documentos-produccion-documental/jira-context.md`.
- Perfil tecnológico comprobado: ASP.NET Web Forms y VB.NET sobre .NET Framework 4.6.1, JavaScript legacy, MySQL y almacenamiento documental Docuarchi.
- Diagnóstico canónico: `Doc/Actualizacion/ProduccionDcoumental/CargaDocumento/00-diagnostico-carga-documentos.md`.
- Prompt aprobado: `Doc/Actualizacion/ProduccionDcoumental/CargaDocumento/01-prompt-implementacion-correccion-carga-documentos.md`.

El alcance es corregir de forma aislada el error de referencia nula en la carga `PRODUCCION`, después de caracterizar su frontera exacta. No se autoriza una reescritura global del cargador ni cambios de semántica para otros eventos.

## Contexto inspeccionado

- `js/gestion/WebFormProducionDocumental.js`: preparación del modal, selección de expediente y tipología, envío y manejo de errores del cliente.
- `webservice/WebServiceProducion.asmx.vb`: `ServiceSolicitaCargarDocumentoExpediente`, `Service_parameter_upload` y respuestas que actualmente pueden exponer `ex.Message`.
- `generic_control/FileUploadHandler_.ashx.vb`: entrada compartida de carga y rama `evento_adjunta=PRODUCCION`.
- `generic_control/FileUploadHandler.js`: proyección del resultado; la rama de producción usa una propiedad ajena o inexistente para construir la fila.
- `workflow/ClassAlmacenamiento.vb`: `UploadSaveFile`, `PreAlmacenaDocumentoProduccion` y `AlmacenamientoDocumentoProduccionDocumental`; este último confía en `YES` antes de consumir `EsctructuraExpediente(0)`.
- Clases de Gestión y Docuarchi enumeradas en el diagnóstico para expediente, gabinete, relaciones, fechas y configuración de carga.
- Compatibilidad obligatoria: contratos HTTP y `UploadFilesResult`, DOC-85/`ADJUNTARADICACION`, Workflow, SII, ENLASE, versiones, digitalización, PQRS y demás consumidores del handler compartido.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Caracterizar los cuatro límites del recorrido y corregir solo la primera frontera que reproduzca la referencia nula; si no se reproduce, proteger los consumidores inseguros demostrados sin atribuir una causa falsa. | `WebServiceProducion.asmx.vb`; `FileUploadHandler_.ashx.vb`; `ClassAlmacenamiento.vb` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Resolver y validar en servidor el contexto autorizado de producción; ninguna salida textual `YES` habilita el acceso a referencias o índices sin comprobar materialización, cantidad e identidad. | `ServiceSolicitaCargarDocumentoExpediente`; `PreAlmacenaDocumentoProduccion`; `AlmacenamientoDocumentoProduccionDocumental` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Mantener el cargador compartido compatible y delimitar cualquier cambio por `evento_adjunta=PRODUCCION`, preservando firmas, campos y ramas de los demás eventos. | `FileUploadHandler_.ashx.vb`; `FileUploadHandler.js`; `UploadFilesResult` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Separar persistencia de proyección visual, corregir el contrato de la fila de producción y no repetir una escritura por una falla posterior de interfaz. | `_RegistraArchivoInterfaz`; `insert_row_producion_documental`; `AlmacenamientoDocumentoProduccionDocumental` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Devolver códigos funcionales saneados y corregir a `ex.message` los `Catch` del recorrido; el cliente no recibe `ex.Message`, rutas ni datos sensibles. | `Service_parameter_upload`; `ProcessRequest`; `WebFormProducionDocumental.js`; `FileUploadHandler.js` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Usar el resultado existente de `Almacenamiento` como autoridad: error confirmado termina el flujo; éxito proyecta sus datos; resultado incierto o falla visual no repite automáticamente la escritura. No agregar consulta de existencia, tabla, columna ni token persistente. | `UploadSaveFile`; `PreAlmacenaDocumentoProduccion`; `_RegistraArchivoInterfaz` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Exigir caracterización automatizada, compilación, regresión compartida y E2E autorizada con infraestructura existente y evidencia saneada antes del cierre. | `tests`; `tools/e2e`; `AGENTS.md`; `tools/e2e/AGENT-RUNBOOK.md` | D-07 | RQ-07 | Origen: D-07, RQ-07 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | La frontera exacta queda reproducida o caracterizada y el escenario original deja de emitir `NullReferenceException`. | WHEN se ejecuta cada límite con contexto válido e incompleto THEN se identifica la etapa y se obtiene éxito o rechazo funcional determinista. | No ocultar incertidumbre con un `Try/Catch`; reversa por retiro del cambio focal. |
| RQ-02 | Ninguna referencia, arreglo o lista se consume sin validación local. | WHEN un resolver retorna `YES` con salida nula, vacía o de otra identidad THEN el sistema rechaza antes de `(0)`, `.Item(0)`, `.Length`, `.Count` o una propiedad. | Preservar el recorrido válido y la autorización existente. |
| RQ-03 | Los consumidores distintos de `PRODUCCION` conservan su contrato. | WHEN se ejecutan `ADJUNTARADICACION`, Workflow, SII, ENLASE, versiones y adjuntos THEN firmas, payloads y resultados permanecen invariantes. | Cualquier cambio compartido debe tener guardia explícita y prueba de inventario. |
| RQ-04 | Persistencia e interfaz tienen resultados independientes. | WHEN el documento se persiste y falla la fila visual THEN no se vuelve a almacenar y se informa `PRODUCCION_CARGA_PROYECCION_FALLIDA` o recuperación equivalente. | Evita duplicación por confundir fallo visual con fallo transaccional. |
| RQ-05 | Los errores públicos son funcionales y saneados. | WHEN falla preparación, validación, almacenamiento, confirmación o proyección THEN la respuesta contiene un código de etapa sin excepción interna ni secretos. | La traza técnica conserva diagnóstico sin datos sensibles. |
| RQ-06 | La corrección no repite automáticamente el almacenamiento ni consulta existencia. | WHEN `Almacenamiento` falla THEN termina; WHEN retorna `YES` THEN usa sus datos; WHEN la respuesta es incierta o falla la proyección THEN informa el estado sin una segunda escritura. El doble clic se bloquea mientras la solicitud está activa. | Preserva el componente compartido y evita introducir persistencia transversal; no promete idempotencia fuerte entre solicitudes independientes. |
| RQ-07 | La corrección demuestra no regresión y cierre seguro. | WHEN concluye la implementación THEN aprueban pruebas focales, compartidas, compilación y OpenSpec; la E2E real solo se ejecuta con autorización explícita. | Sin autorización E2E se registra bloqueo explícito; no se aceptan mocks ni evidencia ficticia para cerrar. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` está desarrollada en `design.md`, reflejada en `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox conserva su origen, incluidas validación, documentación y reversa.
3. Las reglas específicas de Web Forms, VB.NET y JavaScript se aplican porque corresponden al código inspeccionado.
4. No se implementa código hasta aprobar esta matriz y caracterizar el fallo con una prueba o arnés aislado.

## Resultado del refinamiento

- Estado: aprobado para sincronización de trazabilidad.
- Siguiente compuerta: ejecutar `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-88 --sync` y revisar el resultado antes de implementar.
