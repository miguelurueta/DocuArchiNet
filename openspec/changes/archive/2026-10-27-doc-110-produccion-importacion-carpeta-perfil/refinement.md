<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-110-produccion-importacion-carpeta-perfil

## Aprobación vigente

El usuario aprobó explícitamente el refinamiento completo el 2026-10-20, en respuesta a la solicitud de aprobación integral de DOC-110. Estado vigente: `approved`. Las menciones históricas a `draft`, ausencia de aprobación y fallo de `refinement:approved` que siguen documentan revisiones anteriores y quedan superadas por esta aprobación.

La aprobación comprende el diseño, contratos y tareas del perfil. No acredita implementación, concesión del permiso externo ni pruebas funcionales; tampoco autoriza por sí sola E2E autenticadas o carga en un ambiente. Se conserva el alcance documental de esta entrega. El SQL fue aplicado manualmente por el usuario y su comprobación SELECT está referenciada más abajo.

## Fuente y alcance

Resultado posterior del rastreo: A1 resuelto en diseño según [diagnóstico de empresa](diagnostico-dominio-empresa.md). Perfil y empresa se resuelven en el mismo origen RA; no usar conexión sin prefijo ni sustituir GA_IDEMPRESA por RA_ID_EMPRESA. Se consultaron metadatos/conteos mediante SELECT, sin sesión autenticada ni escrituras. Esta resolución no aprueba el refinamiento completo ni resuelve A3/asignación administrativa.

Ampliación documental: [contrato de ámbito y permiso](contrato-ambito-permiso.md). Define fuente de empresa, rechazo de cero, mapeo físico propuesto y revocación efectiva en la siguiente solicitud protegida. A1 define el origen RA y exige verificarlo al implementar; asignación administrativa corresponde a otra aplicación, según confirmación del usuario; contrato externo entregado. No acredita implementación.

- Ticket: `DOC-110` — PRODUCCION-IMPORTACION-CARPETA-PERFIL
- Cambio OpenSpec: `doc-110-produccion-importacion-carpeta-perfil`
- Fuente Jira: `specs/*/jira-context.md`
- Perfil tecnológico confirmado: legacy-webforms-vb; WebForms/VB.NET 4.6.1, JavaScript, ASMX y MySQL 5.1. No introducir MVC/Core/React/EF.

DOC-110 corresponde exclusivamente al prompt 02: administración, borradores, publicación inmutable, reglas y selección autorizada del perfil. Fuera de alcance: carpetas reales, transferencia, expedientes, almacenamiento, incorporación, lotes y worker. No crear tablas operativas para completar el perfil. Referencia común: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/`.

Este artefacto es la compuerta entre el ticket y la implementacion. No se aprueba por generacion automatica: una persona responsable debe confirmar alcance, decisiones, compatibilidad y evidencia de codigo.

## Contexto inspeccionado

- `webservice/WebServiceProducion.asmx.vb`: `ServiceSolicitaCargarDocumentoExpediente`, `ServiceSolicitaListaTipologiasExpediente`, WebMethod con sesión. Patrón de transporte y catálogo; sesión no demuestra permisos administrativos ni CSRF.
- `Gestion/ClassGaProducionDocumental.vb`, `SolicitaCargarDocumentoExpediente`, y `js/gestion/WebFormProducionDocumental.js`, `ActivaCargaArchivos`: recorrido individual y permisos existentes que deben preservarse.
- `GestionDocumental-Docuarchi.net.vbproj`: registrar archivos y recursos compatibles con el proyecto WebForms existente.
- Diseño común `01-modelo-datos.md`: tres tablas propuestas de configuración (`ra_integracion_perfil`, `ra_integracion_perfil_version`, `ra_integracion_perfil_regla`), revisión/UNICIDAD y referencias legacy. No acredita tablas desplegadas. Las seis tablas operativas pertenecen a otras entregas.
- Diseño común `04-decisiones-y-pendientes.md`: P02/P03/P08/P09 conservan condiciones sobre identidad/permisos, políticas, seguridad y entorno. Ambos ámbitos de integridad ya están aprobados (P01 resuelto).
- `tools/e2e/package.json`, `playwright.config.cjs` y `tests/support/authenticated-workflow-session.cjs`: reutilizar infraestructura; verificar compatibilidad con Producción sin inventar tareas Workflow.
- La extracción de Jira pierde algunos nombres de tecnología; el prompt local 02 y el código permiten identificar el stack. No se alteró la captura original. El rastreo posterior sí consultó metadatos y conteos mediante SELECT, según las evidencias saneadas del cambio.
- Revisión ampliada: `Class_remit_dest_interno_perfil_produccion.AsignaPermisosPerfilUsuarioGestion` carga capacidades administrativas existentes, incluida `GA_MANAGER_CONFIGURACION`; `ClassGestorSesion` integra esa carga. Hay controles administrativos en menú y servidor. Ver `evidencia-autorizacion-solicitudes.md`: se corrige la exploración incompleta anterior; no hace falta un sistema de permisos nuevo.

## Decisiones de alcance confirmadas; desarrollo técnico por completar

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Configuración del perfil sin efectos documentales; SII y carga individual intactos. | Prompt 02; entradas Producción inspeccionadas. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Borrador con revisión; publicación atómica e inmutable y versiones anteriores preservadas. | Modelo común: ra_integracion_perfil_version y UQ perfil/numero. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Reglas declarativas por campo, fuentes constantes/carpeta/nombre/herencia, sin SQL ni expresiones ejecutables. | Modelo común: ra_integracion_perfil_regla; prompt 02. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Completo predeterminado con EXPEDIENTE/LOTE obligatorio al publicar; parcial explícito con NO_APLICA. | Diseño común D07/P01; aprobación previa del usuario. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Identidad/ámbito servidor; permisos específicos separados ADMINISTRAR_PERFILES_IMPORTACION e IMPORTAR_CARPETAS, default 0; selección conserva nivel. | ServiceSolicitaCargarDocumentoExpediente; prompt 02 y guía 00. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | SQL manual MySQL 5.1; referencias legacy verificadas y sin modificar catálogos/SII. | Modelo común y evidencia de esquema guardada. | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | E2E PER-01/02/03 sobre infraestructura existente; sin login/configuración paralelos. | tools/e2e y sección E2E del prompt 02. | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Proveedores versionados para índices, con adaptador común, autorización servidor y resultado diferenciado de ausencia/ambigüedad. | Extensión solicitada por el usuario y contratos Diseno/09-proveedores-datos-indices.md y Diseno/10-catalogo-proveedores-tabla.md. | D-08 | RQ-11 | Origen: D-08, RQ-11 |
| D-09 | Administración visual de proveedores y autorizaciones persistentes de fuentes RA; facultades separadas, revisión/historial y límite literal imp_02 con excepciones exactas. | Correcciones y ampliaciones solicitadas por el usuario; contratos Diseno/10, 11 y 12, implementación y aceptación documentadas. | D-09 | RQ-12 | Origen: D-09, RQ-12 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Perfil independiente de importación. | WHEN se prueba una regla THEN devuelve valores/incidencias sin crear documentos ni expedientes. | Esquema nuevo ausente no rompe Producción/carga individual; no autorregistro ni tareas ficticias. |
| RQ-02 | Versiones persistidas, inmutables y concurrencia controlada. | WHEN dos ediciones usan igual revisión THEN una se rechaza; nueva publicación conserva versión anterior. | Estado, referencia publicada y pertenencia comprobados en transacción. |
| RQ-03 | Configuración determinista validada. | WHEN se prueban dos formatos THEN se obtienen campos previstos; requerido ausente, tipología inválida o campo ajeno bloquea publicación. | Conservar ceros iniciales y fuentes diferidas tipadas, sin IDs ficticios. |
| RQ-04 | Política explícita por versión. | WHEN completo carece de ámbito THEN rechazar publicación; EXPEDIENTE/LOTE persisten y parcial normaliza NO_APLICA. | Configurar no significa implementar barreras de ejecución. |
| RQ-05 | Autorización por recurso. | WHEN usuario ajeno consulta/edita/publica o manipula nivel THEN rechazar en servidor; selección válida conserva nivel. | Permiso de perfil no concede creación/carga; proteger mutaciones contra CSRF. |
| RQ-06 | DDL revisable de configuración. | WHEN el equipo revisa SQL THEN dispone de tablas/índices propios, orden, pre/postcontroles y recuperación parcial. | Sin JSON nativo, utf8mb4, CHECK efectivo ni rollback global de DDL; no autoaplicar. |
| RQ-07 | Evidencia de capacidad real. | WHEN PER-01–03 se ejecutan autorizados THEN UI/HTTP y observaciones SELECT verifican publicación, persistencia y permisos. | Mocks/static PASS no sustituyen E2E. Registrar ambiente, SHA, casos y pendientes sin secretos. |
| RQ-08 | Identidades incrementales y migración verificable. | WHEN se crea o migra configuración THEN IDs, referencias, JSON y huellas conservan integridad comprobable. | INT UNSIGNED y String decimal canónico; migración e instalación nueva con evidencias distintas. |
| RQ-09 | Búsqueda autorizada por código/nombre. | WHEN se escribe o limpia el término THEN sugerencias vigentes y selección por teclado conservan ámbito y nivel. | Consultas acotadas, texto seguro y descarte de respuestas anteriores. |
| RQ-10 | Detalle guiado con protección de cambios. | WHEN cambia una referencia padre o se navega con edición local THEN se invalidan dependencias y se exige decidir sobre cambios. | Etiquetas autorizadas fuera de página, publicación de configuración guardada y recuperación sin replay. |
| RQ-11 | Consulta de proveedores por versión explícita. | WHEN una regla resuelve una clave THEN el servidor aplica la fuente autorizada y distingue resultado único, ausente o ambiguo. | Sin SQL cliente, referencias publicadas inmutables y sin consulta con clave futura ficticia. |
| RQ-12 | Administración de proveedores y fuentes. | WHEN se publica proveedor o se modifica/revoca fuente THEN se exigen facultad/revisión, transacción e historial y se conservan consumidores publicados. | Política vigente RA sin fallback, descubrimiento por permiso de fuentes y restricción literal de tablas independiente. |

## Reglas de trazabilidad obligatorias

1. Cada decision `D-XX` debe estar desarrollada en `design.md`, reflejada en al menos un requirement/scenario de `spec.md` y vinculada a una tarea mediante `Origen: D-XX, RQ-XX`.
2. Cada tarea con checkbox debe conservar su origen. Las tareas de validacion, rollout y documentacion tambien deben indicar la decision o requisito que verifican.
3. Las reglas de frontend, WebForms, Node u otro framework solo se agregan cuando el perfil tecnologico y el codigo afectado las justifican.
4. El estado solo puede cambiar a `approved` cuando no haya marcadores pendientes, las decisiones sean especificas y la matriz sea completa.

## Política aprobada

ADMINISTRAR_PERFILES_IMPORTACION y IMPORTAR_CARPETAS son capacidades separadas, deshabilitadas por defecto y comprobadas en servidor mediante infraestructura existente. DOC-110 entrega la administrativa; la entrega de carga implementa la operativa. Ninguna sustituye permisos de nivel/gabinete/creación/carga. Cambios de esquema se entregan como SQL MySQL 5.1 manual, sin conceder permisos automáticamente. Aprobación del usuario: 2026-10-20. Esto resuelve A2, no autoriza pruebas reales ni aprueba automáticamente el refinamiento completo.

## Resultado del refinamiento

Estado actualizado el 2026-10-20: el usuario confirmó la aplicación manual del SQL 01. La evidencia `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc110-mysql51-compatibilidad.json`, renovada a las 14:43:29 UTC mediante SELECT, confirma tres tablas propias InnoDB, dos nombres de FK y la columna del permiso NOT NULL DEFAULT 0 en el destino RA inspeccionado. Las menciones posteriores a objetos ausentes o DDL no aplicado describen inspecciones anteriores. No se acredita equivalencia exhaustiva del esquema, implementación funcional, asignación externa ni E2E. Este hecho no cambia el estado draft del refinamiento.

Aprobación específica posterior del usuario: SQL 01 simplificado, con tres CREATE TABLE de nombres ra_ y un ALTER del permiso, sin validaciones en el archivo. Se completa 2.2 como entrega documental aprobada. No autoriza ejecutar DDL, implementar servicios o E2E, ni se interpreta como aprobación del refinamiento completo.

Búsqueda directa por conexiones completada: `evidencia-mapa-bases.json` y mapa funcional `Exploracion/09-mapa-bases-verificado.md`. ODBC y módulos activos inspeccionados; RA/DA seleccionan docuarchi, Workflow bases diferentes. Once tablas legacy presentes y tres nuevas ausentes. MyDbContext no acreditado por fallo de inspección. Sin DDL ni datos modificados; precontroles SQL se mantienen para aplicación manual.

Corrección posterior a la revisión final: el usuario confirmó topología multibase, corroborada por consumidores RA/DA de `Class_system1`. Ver contrato común 08. La aceptación final del vínculo de gabinete y SQL queda condicionada a caracterizar origen/equivalencia, sin inferirlo de ID o nombre. La revisión final anterior no acredita ese punto. A1 permanece acotado a empresa/usuario y tablas propias en RA.

Revisión consolidada: [diagnóstico documental final](revision-final-documental.md). Trazabilidad 1.7 comprobada; prompts 02/06/08/10 y OpenSpec pasan controles estáticos. El paquete está preparado para revisión final responsable, sin declarar código implementado ni aprobar automáticamente el refinamiento.

Tareas 1.5/1.6 completadas documentalmente en `Diseno/07-contrato-http-seguridad-perfil.md`: métodos/DTO, permisos por propósito, revisiones separadas, guard local y recuperación. A3 definido técnicamente; implementarlo y probarlo sigue abierto (5.6/5.7/7.8). No acredita protección existente ni aprueba el refinamiento completo.


Avance independiente: tareas 1.2/2.1/2.2/2.3/3.8 con entregables documentales preparados según `Sql/01-perfil-importacion-mysql51.sql` y su procedimiento. Esquema de permisos contrastado con SELECT y registrado en `evidencia-esquema-permiso.json`. No se aplicó DDL. Las decisiones de coincidencias/desactivación fueron aprobadas explícitamente el 2026-10-20; ver contrato común 06. Se cierran 1.9/1.10 documentalmente, no se aprueba el refinamiento completo.

Tarea documental 1.3 desarrollada en `Diseno/05-contrato-fuentes-expediente.md` de la documentación funcional y enlazada desde D-03. Contiene fuentes, tipos, obligatoriedad, etapas y criterios de prueba; se sincronizan prompts 02/08 y escenarios RQ-03. Su completitud documental no aprueba el contrato ni implementa pruebas. Coincidencias y T11 se desarrollan en contrato 06 aprobado; P03 conserva caracterización técnica del adaptador antes de habilitar efectos.

Investigación de `GA_MANAGER_CONFIGURACION`: ver `diagnostico-ga-manager-configuracion.md`. Es dato de perfil cargado a sesión, sin consumidor de autorización localizado en el código actual/histórico examinado. No equivale a una política de administración ya probada; no habilitar por inferencia del nombre. Se identificó además el origen de negocio de `GA_IDEMPRESA`, que sustenta el ámbito RA documentado, rechazando empresa cero.

Revisión ampliada el 2026-10-20. `design.md` desarrolla D-01–07, contratos conceptuales, persistencia, responsabilidades e inventario propuesto de archivos. `spec.md` desarrolla siete requisitos; `tasks.md` conserva tareas con origen y verificación y distingue entregables documentales de implementación. La fuente Jira permanece en `jira-context.md`; no se presenta el texto importado como diseño implementado.

Validación: OpenSpec estricto PASS. OPSXJ pasa decisiones/requisitos y trazabilidad en design/spec/tasks; falla únicamente `refinement:approved`. Esto no elimina la verificación de implementación A1 ni la condición técnica A3 del diseño.

Revisión final y condiciones de implementación:

- P02 / A1–A2: A2 RESUELTO por aprobación del usuario: permiso administrativo específico para DOC-110 y permiso operativo separado para la entrega de carga, ambos default 0; no reutilizar GA_MANAGER_CONFIGURACION. Mapeo físico documentado y SQL preparado; asignación externa confirmada. Implementación y aceptación conjunta siguen abiertas.
- P03: fuentes en contrato 05 y políticas aprobadas en 06. Caracterizar identidad/comparación legacy antes de habilitar reutilización operativa; no inventar valores globales. T11 resuelto sin suspender lotes por desactivar perfil.
- P08 / A3: sesión, errores, esquema ausente y archivos propuestos documentados. Búsqueda ampliada sin validador CSRF explícito identificado para el nuevo ASMX; no prueba vulnerabilidad ni regresión. La extensión acotada se define en contrato 07; implementarla y verificarla antes de habilitar mutaciones. Contratos y reglas puras pueden avanzar.
- D-01–07 y RQ-01–07 ya se desarrollaron en design/spec/tasks y su trazabilidad pasa. Revisar los contratos propuestos y comprobar los contratos A1/A3 documentados antes de aprobar; no cambiar el marcador solo para obtener PASS.
- P09: entorno/cuentas/datos descartables autorizados y SQL aplicado por el equipo son necesarios para pruebas reales, no para continuar diseño o tests locales.

No se implementó código funcional ni se ejecutó DDL o E2E. SQL 01 sí está generado para revisión y aplicación manual. El marcador draft sigue siendo el estado real; approved en el manifiesto expresa la condición exigida, no una aprobación ya obtenida.

- Estado vigente: aprobado por el usuario el 2026-10-20; historial de revisiones conservado.
- Comando: `npm.cmd --prefix tools/opsxj run opsxj:refine -- <ISSUE-KEY> --sync`.

Actualización por confirmación del usuario: administración de permisos fuera del repo. Tareas 1.8/3.10 cerradas como delimitación/contrato documental; 9.6 mantiene aceptación conjunta externa abierta. No se modifica automáticamente el estado draft ni se declara capacidad desplegada.
