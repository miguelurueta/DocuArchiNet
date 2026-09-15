## Why

CREACION-VINCULACION-EXPEDIENTE. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-67.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # Implementar creación, vinculación e indexación de expedientes SII
> 
> ## Fuente normativa obligatoria
> 
> Antes de analizar, estimar o implementar este ticket, leer completo el siguiente prompt del repositorio:
> 
> ```text
> Doc/Actualizacion/workflow/ImportarServicioWeb/PromptBackend/08-creacion-vinculacion-expedientes-sii.md
> ```
> 
> Consultar también la exploración arquitectónica que lo sustenta:
> 
> ```text
> Doc/Actualizacion/workflow/ImportarServicioWeb/Exploracion/implementacion-creacion-vinculacion-expedientes-sii.md
> ```
> 
> Versión consolidada en `main`: commit `d26e9c3a`, PR `#66`.
> 
> El prompt completo del repositorio es la especificación normativa. Este texto de Jira es únicamente una ficha ejecutiva y no lo sustituye. Si existe contradicción, omisión o diferencia de detalle, prevalece el prompt versionado en el repositorio. Está prohibido implementar basándose solamente en este resumen.
> 
> ## Objetivo
> 
> Completar el flujo backend de importación SII para que, de manera secuencial e idempotente:
> 
> 1. resuelva o cree obligatoriamente los expedientes requeridos;
> 2. almacene los items SII seleccionados;
> 3. consulte el universo documental mediante `NombreGabinete + ENLASE = RadicadoSII`;
> 4. asigne cada `IdImagen` a exactamente un expediente único, primario o secundario;
> 5. vincule únicamente relaciones ausentes;
> 6. registre caché persistente por documento;
> 7. actualice `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` según el gabinete;
> 8. confirme el índice electrónico SQL y su archivo XML;
> 9. reconcilie todos los efectos antes de completar la intención.
> 
> El servicio no se considera multiplex. Los documentos deben procesarse uno por uno, conservando los efectos ya confirmados ante reintentos.
> 
> ## Reglas funcionales innegociables
> 
> - Crear o reutilizar expediente es obligatorio. Si no puede resolverse, no almacenar ni completar.
> - `ENLASE` es la única forma disponible de descubrir los documentos relacionados con el radicado. No exigir otra relación física con la tarea.
> - Un sello corregido no sustituye ni elimina el anterior; ambos deben conservarse y reconciliarse.
> - Cubrir expediente único y múltiples expedientes, incluidos primario y secundarios.
> - Localizar el expediente SII con matrícula normalizada y gabinete; verificar su identidad física mediante todos los campos configurados con `estado_unico=1`.
> - La caché documental debe identificar `IdTarea + IdImagen + NombreGabinete`, guardar el expediente esperado y el radicado, y contar con protección única persistente.
> - La caché no es fuente de verdad: antes de vincular o confirmar se debe consultar la relación física.
> - No completar mientras SQL, XML, vínculo, expediente, caché o índices no estén reconciliados.
> 
> ## Migración obligatoria de funciones existentes
> 
> La implementación debe partir del comportamiento comprobado de las funciones legacy. No se autoriza una reimplementación paralela desde cero.
> 
> Caracterizar y definir individualmente el destino moderno de:
> 
> - `SolicitaEstructuraTramite`
> - `SolicitaRegistroExpedienteMatricula`
> - `SolicitaCacheCreacionExpedienteSII`
> - `SolicitaEstructuraExpedienteSII`
> - `AutoRegistraExpedienteTramite`
> - `CreaExpedienteIntegracionSII`
> - `SolicitaListaImagenesGabineteEnlace`
> - `SolicitaDocumentosTareaWorkflowVinculacionUnicoExpedientesSII`
> - `SolicitaDocumentosTareaWorkflowVinculacionMultipleExpedientesSII`
> - `VinculaDocumentoExpediente`
> - `RegistraCacheCreacionExpedienteSII`
> - `RegistraCahcheVinculacionSII`
> - `ActualizaIndiceDocumentosSII`
> - `ActualizaIndiceDocumentoCacheExpediente`
> - `ActualizaIndiceDocumentoIntegracionSII`
> 
> Para cada función registrar:
> 
> ```text
> archivo/líneas y llamadores
> → entradas explícitas e implícitas
> → consultas, tablas, archivos y efectos
> → regla reutilizada y defecto conocido
> → migración directa | adaptador transitorio | reemplazo controlado
> → componente moderno responsable
> → prueba de caracterización y prueba equivalente
> → estado final
> ```
> 
> No aceptar agrupaciones genéricas como “se reutilizó la lógica legacy”. Los efectos físicos complejos pueden permanecer temporalmente detrás de puertos, pero requieren precondición, postcondición, idempotencia y reconciliación. Un retorno `YES` no demuestra por sí solo una vinculación correcta.
> 
> ## Compatibilidad y restricciones
> 
> - Implementación aditiva bajo el gate vigente.
> - Con el gate apagado debe conservarse íntegramente el recorrido legacy.
> - No modificar `workflow/ClassAlmacenamiento.vb` ni `AlmacenaDocumentoTareaWorkflow(...)`.
> - No modificar ni redirigir consumidores de las funciones legacy protegidas.
> - No invocar ASMX legacy mediante HTTP interno.
> - No usar frontend o sesión como autoridad del expediente.
> - No paralelizar documentos en esta primera versión.
> - No introducir DDL productivo sin migración versionada, rollback y autorización.
> - Mantener .NET Framework 4.6.1 y compatibilidad con el proyecto VB existente.
> 
> ## E2E: reutilización obligatoria
> 
> Leer `tools/e2e/AGENT-RUNBOOK.md` antes de preparar o ejecutar pruebas autenticadas.
> 
> Reutilizar y ampliar exclusivamente la plataforma DOC-56 existente:
> 
> - `import-sii-execution`
> - `import-sii-retry`
> - `import-sii-recovery`
> - `import-sii-concurrency`
> - `scripts/adapters/importar-servicio-web-e2e-adapter.cjs`
> - `doc56-import-sii-execution.profile.example.json`
> - `doc56-import-sii-multidocument.profile.example.json`
> - registro, controles, autenticación, gate, TLS, recursos y evidencia de `workflow-e2e-platform`
> 
> No crear el escenario `import-sii-expedient-execution`, un runner, login, adaptador general, transporte ASMX, manejo de gate o generador de reportes paralelo. Agregar solamente campos de perfil, controles `SELECT` y verificadores que realmente falten.
> 
> Durante el desarrollo ejecutar primero pruebas Node focales. Reservar las E2E autenticadas y mutadoras para la validación final expresamente autorizada. No ejecutar toda la matriz si una evidencia existente ya demuestra la misma propiedad.
> 
> No ejecutar E2E real, carga ni activar el gate sin autorización explícita para ambiente, cuentas y recursos descartables. Todas las consultas de control deben ser `SELECT`. El gate debe quedar siempre en:
> 
> ```text
> WorkflowCentroTrabajoModernActive=false
> WorkflowCentroTrabajoModernUsers=
> WorkflowCentroTrabajoModernGroups=
> ```
> 
> ## Cierre obligatorio
> 
> No cerrar el ticket hasta demostrar:
> 
> - expediente existente reutilizado y expediente faltante creado idempotentemente;
> - caso único y caso primario/secundarios;
> - procesamiento secuencial de todos los documentos esperados;
> - relación física correcta de cada `IdImagen` con un solo expediente;
> - segundo intento sin duplicados y nuevo `IdImagen` procesado incrementalmente;
> - sello anterior conservado cuando llega uno corregido;
> - caché por documento consistente con la relación física;
> - `NITCEDULA`, `RAZONSOCIAL`, `MATRICULA`, índice SQL y XML verificados;
> - recuperación de fallos parciales sin repetir efectos confirmados;
> - trazabilidad completa de todas las funciones legacy enumeradas;
> - regresión legacy intacta con gate apagado;
> - pruebas focales, integración, compilación y E2E autorizada con evidencia saneada;
> - gate restaurado incluso ante fallo.
> 
> Entregar código, migraciones, matriz función–destino–prueba, documentación, diagramas, resultados reales y riesgos residuales como una sola unidad. Cualquier comprobación obligatoria pendiente, fallida o bloqueada impide declarar completa la implementación.

## Jira Metadata

- Tipo: Epic
- Prioridad: Medium
- Labels: CREAR, EXPEDIENTE, VINCULAR

## Capabilities

### New Capabilities
- `creacion-vinculacion-expediente`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

