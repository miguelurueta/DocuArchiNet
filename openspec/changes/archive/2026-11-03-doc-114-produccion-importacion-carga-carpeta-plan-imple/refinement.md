<!-- opsxj:refinement version=1 state=approved -->

# DOC-114 — Refinamiento del preflight existente

## Fuente y alcance

- Ticket: `DOC-114` — PRODUCCION-IMPORTACION-CARGA-CARPETA-PLAN-IMPLE
- Cambio OpenSpec: `doc-114-produccion-importacion-carga-carpeta-plan-imple`
- Fuente Jira: `specs/*/jira-context.md`
- Perfil tecnológico inspeccionado: WebForms/VB.NET Framework 4.6.1, ASMX, JavaScript y MySQL mediante infraestructura existente.

Base main 6886677a02c8bf3a379eba5f8e7180d3d81de0fb, con DOC-110/111/112/113 integrados. El servicio solicitado ya existe. Se propone completar evidencia focal PLAN-01/02/03 y corregir exclusivamente defectos reproducidos si aparecen. No hay un defecto funcional nuevo demostrado en esta revisión. No se reconstruye el motor ni se crea otro repositorio, esquema o protocolo fragmentado. Este borrador no autoriza E2E ni cambios de permisos.

## Contexto inspeccionado

- Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb: Abrir, ListarPerfiles, CalcularPlan, Preparar, EvaluarElegibilidad.
- Services/ProduccionDocumental/ImportacionCarpetas/CanonizadorManifiestoImportacion.vb: Canonizar, RutaCanonica, EvaluarAgrupacion.
- Services/ProduccionDocumental/ImportacionCarpetas/ResolvedorReglasPerfilImportacion.vb y CoordinadorProveedorIndices.vb: reglas y fuentes diferidas existentes.
- Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorPerfilImportacionOperativa.vb, ImportacionCarpetasContextAdapter.vb, ImportacionCarpetasRequestGuard.vb y PerfilImportacionCatalogAdapter.vb: publicación autorizada, contexto, cuerpo cerrado y código/ID de tipología.
- Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb: GuardarPlan, ObtenerPlan y ConfirmarPlan; repositorio único de 07-A/07-B.
- DTOs/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasDtos.vb y webservice/WebServiceImportacionCarpetas.asmx.vb: entrada/respuesta y composición operativa.
- tests/ImportacionCarpetasBehaviorTests.cs: PreflightCalculationCases, PlanStorageCases, ConfirmationServiceCases, AuthenticatedRevocationCases, ManifestCases y DeferredValuesCases. Arnés: tests/doc110-perfil-importacion-behavior.test.cjs.
- tools/e2e/package.json, playwright.config.cjs, tests/support/doc111-importacion-carpetas.cjs, authenticated-workflow-session.cjs, doc113-ventana-importacion.cjs; scripts/support/interactive-e2e-console.cjs, doc32-e2e-odbc.cjs; fixtures y AGENT-RUNBOOK.md.
- Guía 00 y Diseno/02-contratos-operacion.md como contratos comunes vigentes. No se abre otra fuente de verdad.

Se leyó código y aserciones; no se ejecutaron arnés, compilación, login, consultas MySQL ni E2E DOC-114. Contraste detallado en [gap-analysis.md](gap-analysis.md).

## Decisiones propuestas para aprobación

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Reutilizar servicio/ASMX y resolver autoridad/destino en servidor en cada llamada. | ServicioPreparacionImportacion; ContextAdapter.ResolverDestino/AutorizarRecursos; composición ASMX. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Conservar manifiesto completo, canonización y cuotas existentes; no añadir fragmentación. | CanonizadorManifiestoImportacion; PrepararPlanImportacionSolicitud; ManifestCases. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Reutilizar resolvedor publicado y diferidos tipados sin leer binarios ni fabricar IDs. | CoordinadorProveedorIndices; ResolvedorReglasPerfilImportacion; PreflightCalculationCases. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Verificar EXPEDIENTE/LOTE/PARCIAL y error global preservando el inventario completo. | EvaluarElegibilidad y casos integrados de Preparar existentes. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Mantener plan protegido, TTL y determinismo; distinguir cambio válido de manipulación. | GuardarPlan/ConfirmarPlan; guard cerrado; PlanStorageCases/ConfirmationServiceCases. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Añadir evidencia focal PLAN-01/02/03 con sesión/consola/DSN compartidos y autorización concreta. | Infraestructura DOC-111/113 existente; suite PLAN focal no encontrada. | D-06 | RQ-06 | Origen: D-06, RQ-06 |

### D-01 — Servicio y autoridad

AbrirImportacion/ListarPerfilesImportacion/PrepararPlan conservan contratos y autoridad servidor. Obtener un perfil no concede crear/cargar ni sustituye permisos de nivel/gabinete. Las pruebas negativas comprueban rechazo sin persistencia de plan ni documento. No llamar al escritor legacy.

### D-02 — Inventario observable y completo

La solicitud contiene nivelRef/versionId/raices/archivos. El canonizador valida correlaciones, selecciones, duplicados, rutas, cuotas, raíces pobladas y colisiones, y ordena una copia sin leer contenido. Homónimos se diferencian por identidad. Sin campos ni operaciones de fragmentación en el contrato vigente, no se introduce ensamblaje. Un inventario incompleto significa inconsistencias observables: no se promete detectar un archivo local que nunca fue declarado.

### D-03 — Valores resueltos y límites

Dos formatos, ceros iniciales, fechas y longitud funcional ya tienen aserciones locales. El catálogo distingue código de tipología de ID y produce TIPOLOGIA_NO_UNIVOCA. Las fuentes TRAS_EXPEDIENTE conservan valor nulo y descriptor tipado; se materializan posteriormente, fuera de esta capacidad. El preflight solo valida metadatos: no acredita formato real, huella de contenido ni recepción.

### D-04 — Política publicada

COMPLETO_OBLIGATORIO/EXPEDIENTE permite A elegible conservando B bloqueada; COMPLETO_OBLIGATORIO/LOTE bloquea por error impeditivo individual; PARCIAL_PERMITIDO/NO_APLICA mantiene rechazados y permite unidades/archivos válidos. Error global impeditivo bloquea siempre. PuedeConfirmar es elegibilidad, no recepción completa ni permiso para efectos. PLAN-02 debe probarlo en servidor real sin ejecutar barreras documentales.

### D-05 — Integridad del plan

CalcularPlan no persiste. Preparar incorpora reloj UTC/expiración y delega GuardarPlan; el repositorio asigna identidad/revisión/huella y no actualiza planes confirmados al preparar otra vez. Determinismo se compara sobre contexto/configuración/manifiesto/resultado canónicos, excluyendo IDs y tiempos documentados.

Cambiar a otra publicación compatible es una nueva preparación legítima. Manipulación significa referencias ajenas/incompatibles, campos de autoridad inyectados o revisión/huella discordantes contra un plan protegido. PLAN-03 no altera filas por SQL. Si incluye confirmación negativa para probar huella/revisión, requiere autorización concreta y solicitud deliberadamente discordante; nunca confirma un plan válido. Revocar permisos exige recurso/cuenta y mecanismo autorizado con restauración comprobada; no se habilitan ni cambian permisos por inferencia.

### D-06 — Evidencia focal y reutilización

No lanzar runRec para probar planes: crea/recibe documentos. Reutilizar openSession, authenticated-workflow-session, perfil base cerrado DOC-111, interactive-e2e-console, DSN/SELECT y reporteros existentes. Extender controles de forma compatible cuando falte una consulta; no copiar suites ni introducir otro login, .env o configuración.

Rutas propuestas, todavía no creadas: tools/e2e/tests/doc114-plan-importacion.spec.cjs, tools/e2e/tests/support/doc114-plan-importacion.cjs y tools/e2e/scripts/run-doc114-plan-importacion.cjs. Registrar solo el comando necesario en package.json. El selector será fino y su frontera de operaciones se probará antes de autenticar.

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Contexto/apertura/preparación autorizados por servidor. | Nivel ajeno o permiso revocado rechazan sin nuevo plan ni documento. | Mismo ASMX/autoridad; sin escritores. |
| RQ-02 | Manifiesto acotado con identidades preservadas. | Rutas/IDs/cuotas incoherentes rechazan; homónimos/subcarpetas respetan perfil. | Sin fragmentación nueva. |
| RQ-03 | Valores/incidencias/diferidos fieles a reglas publicadas. | Dos formatos y errores por archivo; ceros y diferidos tipados sin ID ficticio. | Resolvedor y materialización existentes. |
| RQ-04 | Elegibilidad por ámbito y error global. | A válida/B inválida en tres políticas; error global bloquea todos sin retirar B. | PuedeConfirmar no equivale a recepción. |
| RQ-05 | Plan protegido y resultado determinista. | Referencias/autoridad/huella alteradas rechazan; versión compatible prepara otro plan legítimo. | Repositorio único y TTL/revisión vigentes. |
| RQ-06 | PLAN-01/02/03 reproducibles y evidenciados. | Sin autorización no autentica; ejecución aprobada registra SHA/HTTP/SELECT sin efectos documentales. | Infraestructura compartida y recursos autorizados por caso. |

## Desglose propuesto para design/spec/tasks

1. Inventariar en el diseño común rutas reutilizadas y las tres rutas E2E propuestas. D-01/D-06, RQ-01/RQ-06.
2. Ejecutar el arnés existente y mapear aserciones de preflight/protección. Añadir regresión solo ante un caso concreto sin cubrir. D-01 a D-05, RQ-01 a RQ-05.
3. Definir fixtures PLAN-01: dos formatos, válido y errores de tipología/nombre/requerido/conversión con diferidos observables. D-03, RQ-03.
4. Definir matriz PLAN-02: A válida/B inválida y error global en tres políticas. D-02/D-04, RQ-02/RQ-04.
5. Definir PLAN-03: campos inyectados, referencias ajenas, versión incompatible y revocación autorizada; separar cambio compatible. D-01/D-05, RQ-01/RQ-05.
6. Implementar suite/soporte y selector fino tras inventariar helper por helper, con controles para impedir confirmación válida/recepción/avance. D-06, RQ-06.
7. Verificar política de autorización y regresión de consumidores compartidos sin autenticar. D-06, RQ-06.
8. Ejecutar regresión local y MSBuild Framework aislado, sin despliegue. D-01/D-05, RQ-01/RQ-05.
9. Concretar autorización y recursos PLAN-01/02/03; ejecutar únicamente dentro de ese alcance. D-06, RQ-06.
10. Consolidar estados, SHA, HTTP y SELECT por caso; conservar NO_EJECUTADO/FALLIDO si falta dependencia. D-06, RQ-06.

Estos entregables se desarrollan en design/spec y en dieciséis tareas atómicas sin completar. El desglose no acredita ejecución ni aceptación.

## Dependencias y reversa

Falta ejecutar localmente las aserciones leídas y diseñar perfiles/fixtures exactos para servidor real. Las versiones históricas 37/38/39 no acreditan por sí solas dos formatos ni autorización de revocación. Codex es responsable de caracterización/diseño/suite; el responsable del ambiente debe autorizar recursos y cambios/restauración de permisos cuando correspondan. No hay autorización E2E específica DOC-114 en este corte.

No se proponen cambios en almacenamiento/SII, tablas, cuotas globales o sistema de permisos. Si aparece una brecha funcional, documentar reproducción y corregir el componente existente antes de ampliar el alcance. La reversa se limita al diff DOC-114, sin borrar recursos conservados.

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.
3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.
4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.

## Resultado del refinamiento

- Estado: borrador concreto de seis decisiones y seis requisitos. Sin implementación ni aceptación funcional atribuida a la inspección.
- Corte alineado: design/spec/tasks desarrollan D-01 a D-06 y RQ-01 a RQ-06; dieciséis tareas sin completar. Falta aceptación editorial y autorización concreta del runtime antes de las fases dependientes.
- El marcador draft permanece hasta revisión/aceptación explícita. No se hereda la autorización DOC-113.

## Implementación local autorizada por continuación

La instrucción de continuar permitió implementar el corte local: 12/16 tareas, suite/soporte/selector y extensión del puente compartido sin ejecutar permisos. Regresión compartida 62/62 y driver 53/53 PASS, arnés Framework 2/2 y build aislado previamente PASS con backend sin cambios. Tres casos descubiertos y omitidos sin autorización. La publicación y aprobación editorial siguen separadas; marcador draft conservado. Recursos y corridas PLAN reales no se han ejecutado. Los estados de propuestas/no creación anteriores son cortes históricos. Informe actual: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-plan-preflight.md.

## Resultado técnico integrado DOC-114 — 2026-11-03

Los cortes propuestos/locales anteriores son históricos. PLAN-01/02/03 tienen evidencia real PASS; permiso operativo restaurado y planes11–23 conservados. El preflight de producto se reutilizó sin cambios. Se implementaron únicamente suite/soporte/selector/fixtures, la extensión autorizada del puente y helpers acotados para recursos de prueba. Ajustes del E2E: correlación de empresa documental por inventario, null/omisión conforme a atributos, orden canónico de publicación y código CONFIGURACION_INVALIDA del validador existente.

Cobertura: seis requisitos/diecisiete escenarios con procedencia real/local explícita. Homónimos/subcarpetas/cuotas/determinismo se acreditan por el arnés Framework existente; no se renombran como E2E. Límites: cuerpo original de plan11 no retenido y positivo por código no disponible; lectura/huellas y negativo ID/código conservan evidencia propia. Informe canónico: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-final-report-20261103.md. La aprobación editorial de este refinamiento permanece draft; las autorizaciones de runtime no la sustituyen.

## Aprobación editorial — 2026-11-03

El usuario respondió «continua» a la solicitud directa de aprobar el informe consolidado y continuar con publicación. Aprobación vinculada a Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc114-final-report-20261103.md SHA256 63571253664acd4d2026827bdef91b204fcd3d3efd65a6e1a833fe6b336785f8. Los estados draft previos son cortes históricos. Se conservan las 16 tareas verificadas, PLAN-01/02/03 reales PASS y permiso restaurado; no se autorizan efectos nuevos del runtime por este cierre.
