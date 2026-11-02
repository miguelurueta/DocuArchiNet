<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12 -->
# DOC-111 — Plan atómico de implementación

Corte de cierre 2026-11-02: **130/130**. 13.18: revisión de matriz/contratos y conformidad del usuario con resultados y límites registradas; OpenSpec estricto y gobierno comprobados, sin omitir controles mediante entorno. Evidencia `Validacion/doc111-cierre-tecnico-20261102.md` y `doc111-gobierno-final-20261102.json`. Alcance 04–12 completado; SII real y limpieza de originales no ejecutados, host desatendido opcional fuera de alcance. Los recuentos y pendientes inferiores son históricos.

Corte vigente matriz 2026-11-02: **129/130**. Se cierra 13.17: matriz de capacidades/RQ-01–12, nivel real/local/NO_EJECUTADO, huellas por sesión, recursos, restauración y límites en `Validacion/doc111-matriz-final-20261102.md`. 13.18 permanece abierta por revisión OpenSpec persistida y QA manual final; no se infiere aprobación de gobierno a partir de los PASS E2E.

Corte vigente posterior degradación 2026-11-02: **128/130**. Sesión 26 PASS: indisponibilidad DOC-111 por tabla operativa ausente, carga individual real de un PDF en expediente 467 (documento 16212/inventario 23301), restauración exacta de ra_integracion_evento y lotes 2–5 sin cambios. 31 archivos protegidos idénticos a HEAD; SII real NO_EJECUTADO, preservación estática separada. Se cierran 13.15/13.16. Evidencia `Validacion/doc111-degradacion-individual-pass-20261102.md`. Matriz y revisión/gobierno finales pendientes; los recuentos inferiores son históricos.

Corte vigente posterior PARCIAL 2026-11-02: **126/130**. REC-01/02 PARCIAL pasó en sesión 25: perfil 25/versión 39, plan 7/lote 5, dos expedientes/documentos, rechazado y total original de tres archivos conservados, índices y repetición/reapertura sin cambios. Se cierra 13.14. Evidencia `Validacion/doc111-rec-parcial-pass-20261102.md`. Los tres modos cuentan con aceptación real propia; siguen pendientes 13.15–13.18. Los recuentos inferiores son históricos.

Corte vigente posterior 2026-11-02: **125/130**. REC-01/02 por LOTE pasó íntegro en sesión 22: plan 6/lote 4, expedientes 652/653, documentos 16208/16209, barrera global, índices y repetición/reapertura sin cambios. Se cierra 13.13. Evidencia `Validacion/doc111-rec-lote-pass-20261102.md`. Los lotes 2 y 3 se conservan; PARCIAL, degradación, carga individual y revisión final permanecen pendientes. Los recuentos inferiores son históricos.

Corte vigente posterior 2026-11-01: **124/130**. REC-01/02 por EXPEDIENTE pasó íntegro en sesión 18: plan 5/lote 3, dos expedientes/documentos, barrera, índices y repetición/reapertura sin cambios; fuentes contrastadas con la ejecución. Se cierra 13.12. Evidencia `Validacion/doc111-rec-expediente-pass-20261101.md`. LOTE y PARCIAL no heredan este PASS; las demás aceptaciones/revisión siguen pendientes. Los recuentos inferiores son históricos.

Corte vigente 2026-11-01: **123/130**. Se cierran 5.9/6.6 con HTTP/sesión sintéticos, autoridad vigente doble y servicios/guards reales reutilizados; no se revocaron permisos ni ejecutaron efectos del ambiente. El lote 2 está completado y sus índices tienen aprobación manual del usuario. REC íntegros, degradación/carga individual reales y revisión final siguen pendientes. Evidencia `Validacion/doc111-flujo-autorizacion-compatibilidad-20261101.md`; los recuentos inferiores son históricos.

Corte 2026-11-01, extensión de recuperación de 10.8/10.11: cierre explícito de reserva ALMACENAR sin inicio, CAS y diario bajo el mutex existente; fachada autenticada y continuación del mismo lote mediante el runner existente. Pruebas locales y compilación en `Validacion/doc111-reserva-sin-inicio-20261101.md`. No completa las nueve tareas de aceptación pendientes ni declara recuperado el lote 2 antes de ejecutar y comprobar la acción real.

Estado: refinamiento autorizado para continuar el flujo el 2026-10-27; implementación progresiva desde el directorio principal. Este desglose conserva el diseño/especificación y solo se marca implementación con su evidencia propia. Cada tarea tiene un entregable principal, un área exacta y una comprobación observable. S/M/L expresan complejidad relativa, no duración.

Las rutas son relativas al repositorio y los archivos nuevos son los propuestos en Diseno/design.md. Los puertos/métodos descritos expresan responsabilidades del diseño, no APIs o campos adicionales. Respetar propietarios 04–12; no crear 13 ni duplicar repositorios, SQL, login o runner. Las tareas de efectos siguen condicionadas a caracterización y autorización real específica. Una verificación local con dobles no acredita integración real.

Las dependencias son por ID de este plan, no por el número de un prompt. Registrar Compile/Content de cada archivo productivo al incorporarlo; la tarea final de proyecto comprueba el conjunto. Mantener la regresión de la capacidad afectada antes de completar su tarea. La referencia «Sustituye» conserva los IDs anteriores para revisar la descomposición; no debe usarse como dependencia nueva.

## 1. Refinamiento y contratos

- [x] 1.1 Inventariar código y dependencia DOC-110, documentar propietarios y contratos comunes; revisión estática realizada en 6313f91f. Origen: D-12, RQ-12.

- [x] 1.2 Redactar diseño y escenarios del conjunto 04–12, límites técnicos y condiciones operativas; sin aprobación ni efectos. Origen: D-12, RQ-12.

- [x] 1.3 [S] Obtener confirmación explícita del refinamiento antes de implementación funcional. Área/archivos: `openspec/changes/doc-111-produccion-importacion-carga-carpeta/refinement.md`. Origen: D-12, RQ-12. Verificación: el usuario indicó «continua con el flujo» después de ajustar tareas y priorizar el repositorio local; se registra como autorización para continuar la implementación, sin autorización adicional de efectos reales. Depende de: 1.1, 1.2. Sustituye: 1.3.

## 2. Contratos, tipos y arnés compartidos

Propietario de la guía 00: 04 / 07-A / arnés 12. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 2.1 [S] Definir el contexto operativo sin representar al importador como administrador. Área/archivos: `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasModels.vb`. Origen: D-02, RQ-02. Verificación: El modelo distingue identidad, ámbito, destino y facultad operativa sin exigir PuedeAdministrar. Depende de: 1.3. Sustituye: 2.3, 3.3.

- [x] 2.2 [S] Definir identidades y metadatos del manifiesto sin contenido binario. Área/archivos: `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasModels.vb`. Origen: D-03, RQ-03. Verificación: Dos raíces homónimas admiten correlaciones distintas y los bytes se transportan sin pérdida de precisión. Depende de: 1.3. Sustituye: 3.3, 4.1.

- [x] 2.3 [M] Definir la representación del plan y su snapshot de versión/reglas. Área/archivos: `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasModels.vb`. Origen: D-04, RQ-04. Verificación: La representación conserva nivel, publicación, política y fuentes diferidas sin un ID de expediente ficticio. Depende de: 2.1, 2.2. Sustituye: 3.3, 4.4.

- [x] 2.4 [S] Representar por separado estados de recepción, efecto, raíz, intento y lote. Área/archivos: `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasModels.vb`. Origen: D-09, RQ-09. Verificación: Cada dimensión corresponde al documento 03 y no confunde RECIBIDO con COMPLETADO. Depende de: 1.3. Sustituye: 3.5.

- [x] 2.5 [M] Definir los puertos operativos que implementará un único repositorio. Área/archivos: `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasInterfaces.vb`. Origen: D-05, RQ-05. Verificación: Preparación, confirmación, recepción, reclamación y consulta consumen contratos del mismo repositorio, sin otra familia de tablas. Depende de: 2.3, 2.4. Sustituye: 3.3, 3.4.

- [x] 2.6 [S] Definir los puertos internos de staging y efectos documentales. Área/archivos: `Modelo/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasInterfaces.vb`. Origen: D-07, RQ-07. Verificación: Los puertos reciben contexto explícito y devuelven resultado/evidencia verificables, sin controles WebForms ni endpoints propios. Depende de: 2.1, 2.4. Sustituye: 3.3, 6.2, 6.4.

- [x] 2.7 [S] Definir solicitudes y respuestas de apertura y preparación. Área/archivos: `DTOs/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasDtos.vb`. Origen: D-01, RQ-01. Verificación: Los DTO corresponden a AbrirImportacion/PrepararPlan del contrato 02 y no aceptan identidad autenticada o ruta física cliente. Depende de: 2.1, 2.2, 2.3. Sustituye: 4.3, 4.4.

- [x] 2.8 [S] Definir solicitudes de confirmación y recepción sobre referencias persistidas. Área/archivos: `DTOs/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasDtos.vb`. Origen: D-05, RQ-05. Verificación: Los DTO permiten comprobar plan/revisión/huella/idempotencia sin recibir un plan autoritativo editado por el cliente. Depende de: 2.3, 2.4. Sustituye: 5.1, 5.3.

- [x] 2.9 [M] Reutilizar la capacidad del transporte/JSON y vigencia de sesión existentes, conservando reglas funcionales del perfil publicado. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/LimitesImportacionCarpetas.vb`. Origen: D-03, RQ-03. Verificación: Sin appSettings Doc111 se permite componer la capacidad con una sesión válida; transporte/sesión inválidos y archivos que exceden el perfil se rechazan. No hay cuota de maqueta ni habilitación adicional en Web.config. Corrección explícita del usuario 2026-10-30. Depende de: 1.3. Sustituye: 4.2.

- [x] 2.10 [M] Incorporar los casos operativos al arnés Framework existente sin sustituir DOC-110. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs`, `tests/PerfilImportacionBehaviorTestRunner.cs`, `tests/doc110-perfil-importacion-behavior.test.cjs`. Origen: D-12, RQ-12. Verificación: El arnés ejecuta los casos nuevos y mantiene las familias administrativas existentes con las referencias Framework del proyecto. Depende de: 2.5, 2.6, 2.4. Sustituye: 9.2.

## 3. Esquema operativo y procedimiento manual

Propietario de la guía 00: 07-A. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 3.1 [S] Contrastar tipos, collations y referencias físicas con la evidencia de esquema disponible. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/01-modelo-datos.md`. Origen: D-05, RQ-05. Verificación: Quedan identificadas las referencias comprobadas y las que requieren metadatos autorizados antes de cerrar el DDL. Depende de: 1.3. Sustituye: 3.1.

- [x] 3.2 [M] Materializar la entidad de plan y sus restricciones del modelo común. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`. Origen: D-05, RQ-05. Verificación: La definición soporta contexto/snapshot/huella/revisión y expiración sin JSON nativo ni sintaxis posterior a MySQL 5.1. Depende de: 3.1, 2.3. Sustituye: 3.2.

- [x] 3.3 [M] Materializar la entidad de lote con unicidad por plan y clave de confirmación. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`. Origen: D-05, RQ-05. Verificación: Las restricciones expresan un lote por plan y por contexto/clave, conservando totales originales y política efectiva. Depende de: 3.2. Sustituye: 3.2.

- [x] 3.4 [S] Materializar la entidad de raíz y su identidad dentro del lote. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`. Origen: D-05, RQ-05. Verificación: La unicidad no fusiona selecciones por nombre y las referencias propias usan tipos compatibles. Depende de: 3.3. Sustituye: 3.2.

- [x] 3.5 [M] Materializar la entidad de archivo y sus referencias de recepción/efecto. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`. Origen: D-05, RQ-05. Verificación: La entidad conserva ruta/hash, bytes previstos/recibidos y estados separados con unicidad dentro de la raíz. Depende de: 3.4, 2.4. Sustituye: 3.2.

- [x] 3.6 [S] Materializar la entidad de intento y las referencias de reclamación. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`. Origen: D-09, RQ-09. Verificación: El esquema soporta fase/recurso/token/evidencia sin atribuir fencing del escritor legacy a un índice. Depende de: 3.5. Sustituye: 3.2, 3.5.

- [x] 3.7 [S] Materializar el historial de eventos de operación. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`. Origen: D-05, RQ-05. Verificación: Eventos, actor y referencias quedan auditables con índices de consulta y sin tablas SII. Depende de: 3.6. Sustituye: 3.2.

- [x] 3.8 [S] Entregar la extensión de IMPORTAR_CARPETAS con defecto denegado. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-carga-carpetas-mysql51.sql`. Origen: D-02, RQ-02. Verificación: El cambio de permisos reutiliza la infraestructura existente con default 0 y no contiene concesiones a cuentas. Depende de: 3.1. Sustituye: 2.2.

- [x] 3.9 [M] Documentar aplicación manual, controles SELECT y recuperación del SQL operativo. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-aplicacion-carga-carpetas.md`. Origen: D-05, RQ-05. Verificación: El procedimiento detecta esquema parcial/incompatible, ordena dependencias y no promete rollback transaccional de DDL. Depende de: 3.7, 3.8. Sustituye: 3.2.

## 4. Persistencia del plan y confirmación

Propietario de la guía 00: 07-A. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 4.1 [M] Comprobar capacidad del esquema operativo antes de escribir. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`. Origen: D-05, RQ-05. Verificación: Una estructura ausente o incompatible devuelve indisponibilidad sin emitir DDL. Depende de: 2.5, 3.9. Sustituye: 3.4, 9.5.

- [x] 4.2 [M] Persistir snapshots y manifiestos mediante documentos cerrados y huellas canónicas. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`, `Services/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionJson.vb`. Origen: D-05, RQ-05. Verificación: Un round-trip conserva IDs/valores y detecta alteración del documento; se consume el codec compatible sin relajar esquemas administrativos. Depende de: 4.1, 2.3. Sustituye: 3.3.

- [x] 4.3 [M] Insertar una preparación completa con identidad incremental. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`. Origen: D-04, RQ-04. Verificación: La identidad se recupera en la misma conexión y el plan conserva contexto, configuración y resultado calculado. Depende de: 4.2. Sustituye: 3.4, 4.5.

- [x] 4.4 [S] Obtener un plan filtrado por identidad y ámbito autorizados. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`. Origen: D-04, RQ-04. Verificación: Un plan ajeno no se devuelve ni puede utilizarse como entrada de confirmación. Depende de: 4.3. Sustituye: 3.4, 4.5.

- [x] 4.5 [S] Registrar el evento operativo dentro de la transacción que modifica el recurso. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`. Origen: D-05, RQ-05. Verificación: Un fallo al registrar el evento revierte el cambio propio asociado. Depende de: 4.1. Sustituye: 3.5.

- [x] 4.6 [M] Persistir lote, raíces y archivos y confirmar el plan en una sola transacción. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`. Origen: D-05, RQ-05. Verificación: Un fallo intermedio no deja un lote o inventario parcial confirmado. Depende de: 4.4, 4.5, 2.8. Sustituye: 3.4.

- [x] 4.7 [M] Resolver confirmaciones repetidas mediante plan, contexto, clave y huella. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`. Origen: D-05, RQ-05. Verificación: Dos confirmaciones iguales convergen al mismo lote; la misma clave con contenido diferente produce conflicto. Depende de: 4.6. Sustituye: 3.4, 5.1.

- [x] 4.8 [M] Verificar la carrera de confirmación con dos clientes independientes. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-05, RQ-05. Verificación: La prueba de integración controla un único lote/inventario y revisión tras la carrera; una BD real exige autorización propia. Depende de: 4.7. Sustituye: 3.4, 9.4.

## 5. Autoridad operativa y guard de transporte

Propietario de la guía 00: 06 / 07-B. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 5.1 [M] Resolver usuario activo y empresa del mismo origen documental RA. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb`. Origen: D-02, RQ-02. Verificación: Usuario inactivo, relación ambigua o empresa inválida impiden obtener contexto operativo. Depende de: 2.1. Sustituye: 2.1, 2.3.

- [x] 5.2 [S] Consultar IMPORTAR_CARPETAS vigente en cada solicitud operativa. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb`. Origen: D-02, RQ-02. Verificación: Permiso ausente/nulo/0 o revocado deniega aunque la sesión conserve un valor anterior. Depende de: 5.1, 3.8. Sustituye: 2.2, 2.3.

- [x] 5.3 [M] Autorizar nivel, gabinete y capacidad documental requerida por la acción. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb`, `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb`. Origen: D-02, RQ-02. Verificación: La facultad de importar sola no permite operar sobre un nivel o gabinete no autorizado. Depende de: 5.2. Sustituye: 2.1, 2.3.

- [x] 5.4 [M] Resolver ruta canónica y sello de contexto del nivel. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb`. Origen: D-01, RQ-01. Verificación: Mover el nivel o cambiar su contexto relevante invalida la revisión sin redirigir el destino. Depende de: 5.3. Sustituye: 2.1.

- [x] 5.5 [S] Emitir un token operativo vinculado a sesión, identidad, empresa y propósito. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasRequestGuard.vb`. Origen: D-02, RQ-02. Verificación: Un token administrativo o de otro contexto no sustituye el token operativo y no exige PuedeAdministrar. Depende de: 5.1. Sustituye: 2.3.

- [x] 5.6 [M] Validar sesión, token, origen y forma de transporte JSON antes del servicio. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasRequestGuard.vb`. Origen: D-02, RQ-02. Verificación: Una solicitud JSON inválida se rechaza antes de invocar preparación/confirmación. Depende de: 5.5, 2.9. Sustituye: 2.3.

- [x] 5.7 [M] Validar autoridad y protección de la recepción multipart. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasRequestGuard.vb`. Origen: D-02, RQ-02. Verificación: Una carga con token ajeno/ausente se rechaza antes de crear una parte de staging. Depende de: 5.5, 2.9. Sustituye: 2.3, 5.3.

- [x] 5.8 [M] Cubrir la separación entre importar y administrar con la matriz de facultades. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-02, RQ-02. Verificación: Solo importador puede usar la vía operativa autorizada; solo administrador no obtiene permiso de ejecución y no se alteran mutaciones administrativas. Depende de: 5.6, 5.7, 5.3. Sustituye: 2.3, 2.4, 9.4.

- [x] 5.9 [M] Cubrir la revocación de la facultad operativa en una sesión ya autenticada. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-02, RQ-02. Verificación: La siguiente solicitud protegida queda denegada sin efectos aunque la sesión conserve el permiso anterior. Depende de: 5.6, 5.7, 5.3. Sustituye: 2.3, 2.4, 9.4.

## 6. Consumo operativo del perfil DOC-110

Propietario de la guía 00: 06. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 6.1 [S] Definir el contrato de lectura por propósito sin ampliar mutaciones administrativas. Área/archivos: `Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionInterfaces.vb`. Origen: D-02, RQ-02. Verificación: El puerto operativo permite pedir una publicación compatible y no expone guardar/publicar perfiles. Depende de: 2.1. Sustituye: 2.4.

- [x] 6.2 [M] Habilitar la lectura de la publicación para un contexto operativo autorizado. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb`. Origen: D-02, RQ-02. Verificación: Un importador válido obtiene la publicación solicitada y un borrador/referencia ajena se rechazan sin simular administración. Depende de: 6.1, 5.3. Sustituye: 2.4.

- [x] 6.3 [M] Componer las consultas de catálogo con autoridad operativa sin ensanchar la vía administrativa. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb`. Origen: D-02, RQ-02. Verificación: La validación operativa conserva pertenencia/compatibilidad del catálogo y el comportamiento administrativo previo. Depende de: 6.1, 5.3. Sustituye: 2.4.

- [x] 6.4 [M] Separar la autoridad por propósito del motor compatible de consulta de proveedor. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorDatosIndices.vb`, `PoliticaFuentesProveedor.vb`, `MySqlProveedorIndicesRepository.vb`, `MySqlFuenteImportacionRepository.vb`, `CoordinadorProveedorIndices.vb`, `ProveedorIndicesModels.vb` en sus áreas existentes. Origen: D-02, RQ-02. Verificación: El importador autorizado consume la versión permitida sin omitir política RA, estado o contexto. Depende de: 6.1, 5.3. Sustituye: 2.4.

- [x] 6.5 [M] Componer selección publicada, compatibilidad de nivel y snapshot operativo. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorPerfilImportacionOperativa.vb`. Origen: D-04, RQ-04. Verificación: El resultado fija la versión pedida compatible sin sustituirla por la última ni conceder acceso administrativo. Depende de: 6.2, 6.3, 6.4, 5.4. Sustituye: 2.4, 4.3.

- [x] 6.6 [M] Verificar que la vía operativa no altera contratos administrativos DOC-110. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-02, RQ-02. Verificación: Las mismas entradas administrativas conservan sus autorizaciones y rechazos al añadir la lectura operativa. Depende de: 6.5, 5.8, 5.9. Sustituye: 2.4, 9.2.

## 7. Inventario local y preparación autorizada

Propietario de la guía 00: 05 / 06. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 7.1 [M] Conservar selección, raíz, archivo y referencia File en una colección independiente del DOM. Área/archivos: `js/gestion/ImportacionCarpetas/InventarioImportacion.js`. Origen: D-03, RQ-03. Verificación: Raíces homónimas y archivos con el mismo nombre conservan identidades distintas al renderizar o quitar filas. Depende de: 2.2. Sustituye: 4.1.

- [x] 7.2 [S] Agregar una raíz por apertura del selector y detectar capacidad de selección de directorios. Área/archivos: `js/gestion/ImportacionCarpetas/InventarioImportacion.js`. Origen: D-03, RQ-03. Verificación: Agregar carpeta repetidamente conserva selecciones anteriores y la ausencia de capacidad se informa sin simular archivos. Depende de: 7.1. Sustituye: 4.1.

- [x] 7.3 [S] Invalidar la preparación cuando cambia el contenido del inventario. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`. Origen: D-03, RQ-03. Verificación: Agregar/quitar entradas después de revisar impide confirmar el plan anterior. Depende de: 7.1, 2.3. Sustituye: 4.1, 4.5.

- [x] 7.4 [M] Canonizar rutas relativas y detectar traversal, rutas absolutas y colisiones. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CanonizadorManifiestoImportacion.vb`. Origen: D-03, RQ-03. Verificación: Una ruta peligrosa o una colisión normalizada se rechazan antes de construir una ruta física. Depende de: 2.2, 2.9. Sustituye: 4.2.

- [x] 7.5 [M] Comprobar completitud, pertenencia de entradas y cuotas del manifiesto completo. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CanonizadorManifiestoImportacion.vb`. Origen: D-03, RQ-03. Verificación: No se acepta un manifiesto incompleto, con correlaciones ambiguas o que exceda la política servidor. Depende de: 7.4. Sustituye: 4.2.

- [x] 7.6 [S] Evaluar subcarpetas y archivos sueltos conforme al perfil elegido. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CanonizadorManifiestoImportacion.vb`. Origen: D-03, RQ-03. Verificación: La estrategia no implementada se rechaza y no se infieren carpetas vacías a partir de la lista de archivos. Depende de: 7.5, 6.5. Sustituye: 4.1, 4.2.

- [x] 7.7 [M] Resolver apertura con destino fijo, ruta y perfiles operativos compatibles. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb`. Origen: D-01, RQ-01. Verificación: Abrir desde un nivel no elegible falla sin crear plan o efectos; la ruta mostrada procede del servidor. Depende de: 5.4, 6.5, 2.7, 2.9. Sustituye: 4.3.

- [x] 7.8 [M] Añadir el dispatcher operativo a la fachada existente sin usar Ejecutar administrativo. Área/archivos: `webservice/WebServiceImportacionCarpetas.asmx.vb`. Origen: D-02, RQ-02. Verificación: La ruta operativa resuelve su contexto/guard y no requiere ni asigna facultad de administración. Depende de: 5.6, 5.3. Sustituye: 2.4, 4.3.

- [x] 7.9 [S] Exponer AbrirImportacion delegando en el servicio de apertura. Área/archivos: `webservice/WebServiceImportacionCarpetas.asmx.vb`. Origen: D-01, RQ-01. Verificación: El sobre devuelve el contexto validado del nivel solicitado sin aceptar usuario/empresa del payload. Depende de: 7.8, 7.7. Sustituye: 4.3.

- [x] 7.10 [M] Resolver en preflight las reglas del perfil con metadatos del inventario. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb`. Origen: D-04, RQ-04. Verificación: Dos patrones configurados producen sus valores previstos y conservan códigos textuales con ceros. Depende de: 6.5, 7.6. Sustituye: 4.4.

- [x] 7.11 [M] Resolver fuentes de proveedor solo cuando su clave ya es materializable. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb`. Origen: D-04, RQ-04. Verificación: Una consulta autorizada se fija con su versión/procedencia; una clave futura no produce una consulta ficticia. Depende de: 7.10, 6.4. Sustituye: 4.4.

- [x] 7.12 [S] Representar valores dependientes del expediente como fuentes diferidas tipadas. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb`. Origen: D-04, RQ-04. Verificación: El plan conserva la dependencia sin fabricar ID y sin invocar el adaptador de creación. Depende de: 7.10, 7.11. Sustituye: 4.4.

- [x] 7.13 [M] Calcular unidades elegibles y puedeConfirmar separando errores globales y de elemento. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb`. Origen: D-04, RQ-04. Verificación: Un error global bloquea el plan; en parcial una incidencia individual no bloquea otra unidad elegible. Depende de: 7.12. Sustituye: 4.5.

- [x] 7.14 [M] Guardar el plan calculado con snapshot y huella y devolver su referencia. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPreparacionImportacion.vb`. Origen: D-04, RQ-04. Verificación: El resultado recargado coincide con el cálculo servidor y no contiene expedientes o documentos creados. Depende de: 4.3, 7.13. Sustituye: 4.5.

- [x] 7.15 [S] Exponer PrepararPlan con entrada cerrada y límites antes de delegar. Área/archivos: `webservice/WebServiceImportacionCarpetas.asmx.vb`. Origen: D-04, RQ-04. Verificación: Un payload manipulado o incompleto no genera un plan confirmable. Depende de: 7.8, 7.14. Sustituye: 4.4, 4.5.

## 8. Confirmación y recepción de contenido

Propietario de la guía 00: 07-B. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 8.1 [M] Revalidar plan, publicación, destino y autoridad antes de confirmar. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb`. Origen: D-05, RQ-05. Verificación: Un plan expirado/ajeno, una referencia no vigente o un destino cambiado no generan otro lote. Depende de: 4.4, 6.5, 5.4, 4.7. Sustituye: 5.1.

- [x] 8.2 [S] Exponer ConfirmarPlan usando la revisión y huella persistidas. Área/archivos: `webservice/WebServiceImportacionCarpetas.asmx.vb`. Origen: D-05, RQ-05. Verificación: Repetir la misma confirmación conserva lote/IDs; alterar contenido con la misma clave devuelve conflicto. Depende de: 7.8, 8.1, 2.8. Sustituye: 5.1.

- [x] 8.3 [M] Generar referencias y rutas privadas de staging con contención comprobada. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb`. Origen: D-06, RQ-06. Verificación: Una entrada cliente no determina el nombre físico ni permite salir de la raíz privada. Depende de: 2.6, 2.9. Sustituye: 5.2.

- [x] 8.4 [M] Recibir el stream a una parte contabilizando bytes y hash reales. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb`. Origen: D-06, RQ-06. Verificación: Un contenido que supera la cuota se detiene sin leer el lote completo en memoria ni confirmar recepción. Depende de: 8.3. Sustituye: 5.2.

- [x] 8.5 [M] Validar extensión y formato de los bytes recibidos con validadores compatibles. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb`. Origen: D-06, RQ-06. Verificación: MIME/accept del cliente no bastan para aceptar contenido inválido; solo se admiten formatos caracterizados. Depende de: 8.4. Sustituye: 5.2.

- [x] 8.6 [M] Comprobar lote, archivo esperado y estado antes de recibir contenido. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb`. Origen: D-06, RQ-06. Verificación: Una referencia ajena o un archivo no previsto se rechazan antes de escribir staging. Depende de: 5.3, 8.2, 2.8. Sustituye: 5.3.

- [x] 8.7 [M] Confirmar la recepción verificada con bytes, huella y evento. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`. Origen: D-06, RQ-06. Verificación: El estado RECIBIDO solo se confirma con evidencia de la parte válida y no cambia a COMPLETADO documental. Depende de: 4.5, 8.5, 8.6. Sustituye: 5.3, 5.4.

- [x] 8.8 [M] Resolver reenvío de recepción por identidad y contenido verificado. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb`. Origen: D-06, RQ-06. Verificación: Contenido compatible devuelve el resultado recibido; contenido diferente se rechaza sin sustituir el original. Depende de: 8.7. Sustituye: 5.3.

- [x] 8.9 [M] Crear el handler multipart propio que valida y delega en recepción. Área/archivos: `generic_control/ImportacionCarpetasUpload.ashx`, `generic_control/ImportacionCarpetasUpload.ashx.vb`. Origen: D-06, RQ-06. Verificación: El handler aplica guard operativo y devuelve el sobre de recepción sin coordinar creación o almacenamiento. Depende de: 5.7, 8.8. Sustituye: 5.3.

- [x] 8.10 [M] Reconciliar discordancias de parte privada y fila sin repetir ciegamente la recepción. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioRecepcionImportacion.vb`. Origen: D-06, RQ-06. Verificación: Los fallos antes/después del registro conservan evidencia y no declaran recepción válida por la existencia de una fila aislada. Depende de: 8.7, 8.3. Sustituye: 5.4.

- [x] 8.11 [S] Producir una copia consumible sin entregar el único staging retenido. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb`. Origen: D-07, RQ-07. Verificación: Eliminar la copia consumible no elimina el contenido conservado para recuperación. Depende de: 8.3, 8.5. Sustituye: 5.4, 6.4.

## 9. Adaptación de efectos documentales

Propietario de la guía 00: 08 / 09. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 9.1 [M] Documentar la llamada de creación y su evidencia de relación al nivel. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/15-integracion-carga-doc111.md`, `Gestion/ClassGaExpediente.vb`. Origen: D-07, RQ-07. Verificación: El informe identifica parámetros, autoridad, efectos y relación devuelta de Registrar_Expediente_Conservacion sin afirmar idempotencia no demostrada. Depende de: 1.3. Sustituye: 6.1.

- [x] 9.2 [M] Determinar si puede correlacionarse inequívocamente una creación después de un crash. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/15-integracion-carga-doc111.md`. Origen: D-07, RQ-07. Verificación: La decisión aporta evidencia de correlación o declara que esa ventana debe quedar INCIERTA, sin búsqueda por nombre como prueba. Depende de: 9.1. Sustituye: 6.1.

- [x] 9.3 [M] Invocar creación mediante un comando explícito del contexto de la raíz. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb`, parcial `.Legacy.vb`. Origen: D-07, RQ-07. Verificación: El adaptador usa el nivel fijado y no controles WebForms o selección mutable como destino. Implementación/binding compilados y núcleo probado con dobles; invocación real y coordinación integral pendientes de aceptación. Depende de: 2.6, 9.2, 5.3. Sustituye: 6.2.

- [x] 9.4 [M] Comprobar expediente y relación devueltos contra el destino confirmado. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb`, `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Verificacion.vb`. Origen: D-07, RQ-07. Verificación: Un ID con relación equivocada no se devuelve como creación confirmada. Lector por IDs/campos y fallos probados con SELECT sintéticos; sin certificación de XML o efectos reales. Depende de: 9.3, 5.4. Sustituye: 6.2.

- [x] 9.5 [M] Documentar sesión, temporales y efectos existentes de la operación de almacenamiento. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/15-integracion-carga-doc111.md`, `workflow/ClassAlmacenamiento.vb`. Origen: D-07, RQ-07. Verificación: La caracterización identifica File.Name, eliminación de temporales e incorporación/índice ya efectuados. Depende de: 1.3. Sustituye: 6.3.

- [x] 9.6 [M] Registrar la decisión sobre aislamiento frente a otras cargas. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/15-integracion-carga-doc111.md`. Origen: D-07, RQ-07. Decisión explícita del usuario, 2026-10-30: descartar resolver el aislamiento del XML posterior al commit como requisito de integración, manteniendo intacta la función existente. Sustituye el bloqueo previo por exclusión externa no demostrada; no acredita una solución de aislamiento. Se conservan exclusión propia DOC-111, autoridad, verificación e incertidumbre. Depende de: 9.5. Sustituye: 6.3.

- [x] 9.7 [M] Caracterizar preservación del nombre y extensión al consumir una copia de trabajo. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/15-integracion-carga-doc111.md`, `AdaptadorDocumentoImportacion.vb` y catálogo compartido `.Documento.vb`. Origen: D-07, RQ-07. Verificación: Copia PDF con nombre Unicode/ceros y original protegido probados localmente; binding fija NOMBRE y FileInfo.Name, lector contrasta nombres/formato persistidos con SELECT sintéticos. Almacenamiento físico real pendiente de aceptación autorizada. Depende de: 9.5, 8.11. Sustituye: 6.3.

- [x] 9.8 [M] Determinar el soporte verificable del recorrido Multi-TIFF. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/15-integracion-carga-doc111.md`. Origen: D-07, RQ-07. Verificación: La evidencia permite anunciar el formato caracterizado o declarar que no se admite en esta entrega, sin fingir éxito. Depende de: 9.5, 8.11. Sustituye: 6.3.

- [x] 9.9 [M] Invocar almacenamiento con copia consumible e índices efectivos explícitos. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb`, parcial `.Legacy.vb`. Origen: D-07, RQ-07. Verificación: Binding real compilado, llamada única con modo cero, índices sin conflictos y copia comprobada; núcleo probado con PDF local y escritor/SQL dobles. Función compartida/SII intactos, sin invocación real ni endpoint nuevo. Depende de: 2.6, 9.6, 9.7, 9.8, 9.4. Sustituye: 6.4.

- [x] 9.10 [M] Verificar documento, gabinete e incorporación al expediente esperado. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorDocumentoImportacion.vb`, catálogo compartido `.Documento.vb`. Origen: D-07, RQ-07. Verificación: SELECT por IDs comprueba destino RA y documento/inventario/índice en DA, gabinete, actor/empresa, tipología/clase, nombres, formato e índices efectivos; escenarios sintéticos de ausencia, duplicado y discrepancia PASS. No repite incorporación; SQL/archivo/XML reales pendientes de aceptación. Depende de: 9.9. Sustituye: 6.4.

- [x] 9.11 [M] Materializar y validar los valores diferidos después de confirmar expediente. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Valores.vb`. Origen: D-04, RQ-04. Verificación: Un valor incompatible conserva el expediente creado y bloquea la completitud de la unidad. Fase interna compuesta, probada con proveedores/SQL dobles y build local; HTTP y efectos reales pendientes. Depende de: 9.4, 7.12. Sustituye: 6.5.

- [x] 9.12 [S] Conservar valores efectivos y procedencia para los intentos siguientes. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Valores.vb` y lector principal. Origen: D-04, RQ-04. Verificación: Un reintento con valores confirmados no reconsulta el proveedor ni recalcula silenciosamente esos valores. CAS, repetición y distinción NULL/[] probados con dobles; transacción/carrera MySQL real no ejecutadas. Depende de: 9.11, 4.2. Sustituye: 6.5.

## 10. Coordinación, barreras y recuperación

Propietario de la guía 00: 10. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 10.1 [M] Validar las transiciones operativas contra las dimensiones del documento 03. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.vb`. Origen: D-09, RQ-09. Verificación: Una transición no enumerada se rechaza sin modificar el recurso. Depende de: 2.4. Sustituye: 3.5.

- [x] 10.2 [M] Reservar un intento activo por recurso bajo bloqueo y revisión. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`, parcial `MySqlImportacionCarpetasRepository.Control.vb`. Origen: D-09, RQ-09. Verificación: Dos reclamantes no obtienen permiso para iniciar el mismo efecto. Depende de: 3.6, 4.5, 10.1. Sustituye: 3.5, 7.1.

- [x] 10.3 [M] Ejecutar una única fase documental fuera de la transacción de reclamación. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Creacion.vb` y `.Documento.vb`. Origen: D-08, RQ-08. Verificación: Una llamada procesa una creación o un almacenamiento, con reserva/relectura/autoridad, efecto fuera de transacción y consulta del checkpoint; sin bucle de efectos. Pruebas locales con PDF y dobles SQL/escritor; composición real compilada, sin ejecución HTTP/SQL/legacy real. Depende de: 10.2, 9.10, 9.12. Sustituye: 7.1.

- [x] 10.4 [M] Aplicar la barrera COMPLETO_OBLIGATORIO por EXPEDIENTE. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb`, `.Staging.vb` y parciales del repositorio. Origen: D-08, RQ-08. Verificación: Creación/almacenamiento comparten inventario y recepción; solo avanza una raíz con todos sus originales comprobados. Almacenamiento retiene los originales de la unidad y revalida metadatos al reservar/iniciar. Pruebas locales; bloqueo/SQL real pendientes de aceptación. Depende de: 10.3, 8.7. Sustituye: 7.2.

- [x] 10.5 [M] Aplicar la barrera COMPLETO_OBLIGATORIO por LOTE. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb`, `.Staging.vb` y parciales del repositorio. Origen: D-08, RQ-08. Verificación: Una recepción faltante/rechazada/incierta en otra raíz impide iniciar almacenamiento; un documento ya completado conserva su prueba de recepción sin repetirse. Metadatos entre raíces y retención física de unidad probados localmente con dobles explícitos; sin efectos reales. Depende de: 10.3, 8.7. Sustituye: 7.2.

- [x] 10.6 [M] Seleccionar elementos elegibles en PARCIAL_PERMITIDO sin alterar inventario original. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Documento.vb`, `.Barreras.vb` y `.Staging.vb`. Origen: D-08, RQ-08. Verificación: Solo se acepta un archivo elegible según el plan confirmado y recibido/comprobado; otro rechazado permanece visible sin reducir totales. NO_APLICA conservado, un escritor por llamada y pruebas locales con PDF/dobles; sin efectos reales. Depende de: 10.3, 7.13. Sustituye: 7.2.

- [x] 10.7 [M] Persistir resultado confirmado o INCIERTO con su evidencia de intento. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb` y parciales `.ResultadoCreacion.vb`, `.Documento.vb`, `.Consolidacion.vb`. Origen: D-09, RQ-09. Verificación: Checkpoint y consolidación en la misma transacción; completitud exige inventario original, recepción/valores y pruebas terminales concordantes de creación/documento. Incertidumbre, actividad y rechazos permanecen visibles, sin replay ni eliminación de originales. Núcleos y CAS con dobles locales, composición compilada; SQL/rollback/efectos reales pendientes de aceptación. Depende de: 10.3, 4.5. Sustituye: 7.3.

- [x] 10.8 [M] Resolver una fase incierta únicamente con correlación y autoridad comprobadas. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.vb`. Origen: D-09, RQ-09. Verificación: Sin prueba inequívoca se conserva INCIERTO; una reclamación vencida no inicia otro efecto. Depende de: 10.7, 9.2, 9.6. Sustituye: 7.3.

- [x] 10.9 [S] Registrar detención de nuevos efectos sin borrar resultados confirmados. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.vb` y repositorio `.Control.vb`/`.Consolidacion.vb`. Origen: D-09, RQ-09. Verificación: La señal impide nuevos writers y conserva intentos/resultados/IDs. Con llamada activa continúa EN_CURSO, con incertidumbre PENDIENTE_VERIFICACION; DETENIDO exige ausencia de actividad. Un último efecto positivo puede completar conservando la señal. Casos locales con PDF/SQL-writer dobles; sin HTTP/efectos reales. Depende de: 10.2, 10.7. Sustituye: 7.4.

- [x] 10.10 [M] Revalidar autoridad antes de cada efecto conservando el snapshot del lote. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.vb`, `.Creacion.vb`, `.Documento.vb` y bindings de adaptadores. Origen: D-09, RQ-09. Verificación: Identidad/destino/recursos vigentes antes de reservar/invocar y conexiones legacy fijadas; revocación tras reserva impide writer. Las fases usan plan/snapshot confirmados sin seleccionar otra publicación. Composición compilada y pruebas locales de revocación/destino; sesión autenticada real permanece pendiente en 5.9. Depende de: 5.3, 10.3, 10.9. Sustituye: 7.4.

- [x] 10.11 [S] Exponer avance, detención y reconciliación delegando en el coordinador. Área/archivos: `webservice/WebServiceImportacionCarpetas.asmx.vb`. Origen: D-09, RQ-09. Verificación: Las operaciones requieren el recurso/revisión autorizados y no exponen los adaptadores como endpoints independientes. Depende de: 7.8, 10.4, 10.5, 10.6, 10.8, 10.10. Sustituye: 7.1, 7.3, 7.4.

## 11. Consulta y mantenimiento de la operación

Propietario de la guía 00: 11. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 11.1 [M] Obtener el estado del lote bajo propiedad y ámbito autorizados. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb`. Origen: D-10, RQ-10. Verificación: Consultar no avanza, reconcilia ni modifica los recursos del lote. Depende de: 4.1, 5.3, 2.4. Sustituye: 8.3.

- [x] 11.2 [M] Consultar raíces y archivos mediante páginas estables y filtros acotados. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.vb`. Origen: D-10, RQ-10. Verificación: Las páginas conservan totales originales y no excluyen rechazados, cancelados o inciertos para simular éxito. Depende de: 11.1, 3.5. Sustituye: 8.3.

- [x] 11.3 [S] Calcular acciones disponibles a partir de estado y autoridad actuales. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb`. Origen: D-10, RQ-10. Verificación: El cliente no habilita reconciliación o avance solo por una etiqueta de estado. Depende de: 11.1, 10.1. Sustituye: 8.3.

- [x] 11.4 [S] Exportar resultados autorizados protegiendo celdas contra fórmulas. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb`. Origen: D-10, RQ-10. Verificación: El CSV conserva resultados autorizados sin ejecutar acciones ni exponer staging o secretos. Depende de: 11.2. Sustituye: 8.3.

- [x] 11.5 [S] Exponer consultas y exportación usando el servicio de lectura. Área/archivos: `webservice/WebServiceImportacionCarpetas.asmx.vb`. Origen: D-10, RQ-10. Verificación: La fachada devuelve páginas/resultados del recurso autorizado sin ejecutar fases documentales. Depende de: 7.8, 11.3, 11.4. Sustituye: 8.3.

- [x] 11.6 [S] Listar candidatos de limpieza sin borrar cuando falta una política habilitante. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb`. Origen: D-10, RQ-10. Verificación: El diagnóstico en seco preserva staging e historial de cualquier recurso incierto. Depende de: 11.2, 8.3. Sustituye: 8.5.

- [x] 11.7 [M] Aplicar limpieza únicamente a candidatos autorizados que cumplen la retención configurada. Área/archivos: `Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb`. Origen: D-10, RQ-10. Verificación: Una referencia aún necesaria para recuperación no se elimina aunque haya vencido otra retención. Depende de: 11.6, 2.9, 10.8. Sustituye: 8.5.

## 12. Interfaz, transporte y árbol de Producción

Propietario de la guía 00: 05 / 11. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 12.1 [S] Añadir el contenedor propio de importación a Producción sin duplicar formulario o árbol. Área/archivos: `Gestion/WebFormProducionDocumental.aspx`, `Gestion/WebFormProducionDocumental.aspx.vb`. Origen: D-11, RQ-11. Verificación: El contenedor se integra con el nivel existente y no reutiliza el estado global de la carga individual. Depende de: 7.9. Sustituye: 8.1.

- [x] 12.2 [S] Conectar el disparador Importar carpetas con el nivel de origen. Área/archivos: `js/gestion/WebFormProducionDocumental.js`, `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`. Origen: D-11, RQ-11. Verificación: Sin destino válido no se abre la ventana y ActivaCargaArchivos conserva su recorrido individual. Depende de: 12.1. Sustituye: 8.1.

- [x] 12.3 [M] Renderizar las etapas del recorrido con destino de lectura y texto seguro. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js`. Origen: D-11, RQ-11. Verificación: La vista muestra Perfil/Carpetas/Revisión/Resultados sin selector de nivel, catálogos DEMO o simulación productiva. Depende de: 12.1, 7.1. Sustituye: 8.1.

- [x] 12.4 [S] Encapsular estilos y foco del diálogo de importación. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetas.css`. Origen: D-11, RQ-11. Verificación: La ventana no desborda en los viewports del recorrido y sus estilos no alteran los administradores existentes. Depende de: 12.3. Sustituye: 8.1.

- [x] 12.5 [M] Procesar el sobre JSON y errores contractuales sin reenvío automático. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js`. Origen: D-11, RQ-11. Verificación: HTML, timeout o respuesta no interpretable finalizan la solicitud sin declarar éxito ni repetir una mutación. Depende de: 7.8. Sustituye: 8.2.

- [x] 12.6 [M] Enviar archivos con concurrencia acotada y progreso de transferencia real. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js`. Origen: D-11, RQ-11. Verificación: La cola no lanza transferencias sin límite y no presenta bytes enviados como incorporación confirmada. Depende de: 12.5, 8.9, 7.2. Sustituye: 8.2.

- [x] 12.7 [M] Conectar revisión y confirmación al plan calculado por servidor. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`. Origen: D-11, RQ-11. Verificación: Cambiar perfil conserva nivel e invalida plan; confirmar usa la revisión persistida y puedeConfirmar servidor. Depende de: 12.3, 12.5, 7.15, 8.2, 7.3. Sustituye: 8.2.

- [x] 12.8 [S] Representar incidencias y unidades elegibles sin bloquear todo el modo parcial. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js`. Origen: D-11, RQ-11. Verificación: Un error individual no oculta unidades elegibles ni reduce el total original. Depende de: 12.7, 7.13. Sustituye: 8.2.

- [x] 12.9 [M] Inicializar y liberar eventos/recursos de la ventana tras cierre y UpdatePanel. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`. Origen: D-11, RQ-11. Verificación: Abrir/cerrar/reabrir no duplica peticiones o handlers ni conserva object URLs innecesarias. Depende de: 12.2, 12.3, 12.5. Sustituye: 8.1, 8.4.

- [x] 12.10 [M] Recuperar un lote por referencia y pedir reselección solo de bytes no recibidos. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`. Origen: D-11, RQ-11. Verificación: Cerrar/recargar no cambia destino o IDs y no promete acceso automático al disco del usuario. Depende de: 12.9, 11.5, 12.6. Sustituye: 8.4.

- [x] 12.11 [M] Actualizar la rama de origen usando relaciones documentales confirmadas. Área/archivos: `js/gestion/WebFormProducionDocumental.js`. Origen: D-11, RQ-11. Verificación: La actualización no duplica nodos, reconstruye el árbol o cambia la selección actual; una rama no visible no invalida el resultado. Depende de: 9.4, 12.10. Sustituye: 8.4.

## 13. Pruebas, integración y aceptación progresiva

Propietario de la guía 00: 12. Ejecutar después de las dependencias indicadas en cada tarea.

- [x] 13.1 [M] Cubrir identidades del inventario y archivos homónimos con comportamiento de cliente. Área/archivos: `tests/doc111-importacion-carpetas-client.test.cjs`. Origen: D-12, RQ-12. Verificación: La prueba falla si dos raíces/archivos homónimos pierden su identidad al modificar o renderizar la colección. Depende de: 12.7, 12.10, 12.5. Sustituye: 9.1, 9.2.

- [x] 13.2 [M] Cubrir la invalidación de una preparación ante cambios de inventario o perfil. Área/archivos: `tests/doc111-importacion-carpetas-client.test.cjs`. Origen: D-12, RQ-12. Verificación: El cliente no confirma el plan anterior después de cambiar una entrada que afecta su cálculo. Depende de: 12.7, 12.10, 12.5. Sustituye: 9.1, 9.2.

- [x] 13.3 [M] Cubrir errores y resultado incierto sin replay automático del transporte cliente. Área/archivos: `tests/doc111-importacion-carpetas-client.test.cjs`. Origen: D-12, RQ-12. Verificación: Timeout/HTML/respuesta inválida liberan la espera y no vuelven a emitir la mutación. Depende de: 12.7, 12.10, 12.5. Sustituye: 9.1, 9.2.

- [x] 13.4 [M] Construir drivers por capacidad consumiendo la sesión y utilidades E2E existentes. Área/archivos: `tools/e2e/tests/support/doc111-importacion-carpetas.cjs`. Origen: D-12, RQ-12. Verificación: Los drivers reutilizan autenticación/consola y no introducen otra sesión, .env o configuración Playwright. Depende de: 7.15, 8.9, 10.11, 11.5. Sustituye: 9.1.

- [x] 13.5 [M] Agrupar REC-01/02 a partir de los mismos casos especializados de capacidad. Área/archivos: `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs`. Origen: D-12, RQ-12. Verificación: La suite verifica dos raíces, destino y repetición/reapertura sin crear una segunda suite que duplique los recorridos. Depende de: 13.4, 10.4, 10.5, 10.6, 12.11. Sustituye: 9.1, 9.4.

- [x] 13.6 [S] Registrar el selector de escenarios DOC-111 en el paquete E2E existente. Área/archivos: `tools/e2e/scripts/run-doc111-importacion-carpetas.cjs`, `tools/e2e/package.json`. Origen: D-12, RQ-12. Verificación: El comando delega en mecanismos compartidos y exige la autorización correspondiente antes de escenarios con efectos. Depende de: 13.5. Sustituye: 9.1.

- [x] 13.7 [S] Comprobar la sintaxis de cada script productivo y selector DOC-111 implementado. Área/archivos: `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`, `js/gestion/ImportacionCarpetas/InventarioImportacion.js`, `js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js`, `js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js`, `tools/e2e/scripts/run-doc111-importacion-carpetas.cjs`. Origen: D-12, RQ-12. Verificación: node --check termina con salida 0 para cada archivo implementado, sin afirmar pruebas de comportamiento por ese resultado. Depende de: 12.7, 12.10, 13.6. Sustituye: 9.2.

- [x] 13.8 [S] Registrar en el vbproj solo los componentes productivos implementados. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-12, RQ-12. Verificación: Todos los archivos VB/ASMX/ASHX/JS/CSS implementados quedan incluidos sin fixtures, dependencias o secretos. Depende de: 10.11, 11.5, 8.9, 12.11. Sustituye: 9.2.

- [x] 13.9 [M] Compilar el proyecto Framework con salida e intermedios separados. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-12, RQ-12. Verificación: MSBuild de la configuración real termina correctamente sin desplegar ni reemplazar la DLL del sitio. Depende de: 13.8, 2.10. Sustituye: 9.2.

- [x] 13.10 [S] Registrar ambiente, cuenta y recursos descartables autorizados para la aceptación DOC-111. Área/archivos: `Doc/Tecnica/Opsxj/doc-111-produccion-importacion-carga-carpeta/05-PruebasEvidencia.md`. Origen: D-12, RQ-12. Verificación: La evidencia de autorización corresponde a los efectos y recursos de DOC-111; no se infiere de corridas anteriores DOC-110. Depende de: 1.3. Sustituye: 9.3.

- [x] 13.11 [S] Contrastar el esquema operativo aplicado manualmente antes de la aceptación real. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/02-aplicacion-carga-carpetas.md`. Origen: D-12, RQ-12. Verificación: Los controles SELECT confirman compatibilidad del destino autorizado sin aplicar SQL por disponibilidad de conexión. Depende de: 3.9, 13.10. Sustituye: 9.3.

- [x] 13.12 [M] Ejecutar REC-01/02 autorizados con integridad completa por EXPEDIENTE. Área/archivos: `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs`, `Doc/Tecnica/Opsxj/doc-111-produccion-importacion-carga-carpeta/05-PruebasEvidencia.md`. Origen: D-12, RQ-12. Verificación: Dos raíces conservan destino/índices y la raíz incompleta no avanza; repetir/reabrir conserva IDs y se contrasta por SELECT. Depende de: 13.6, 13.9, 13.11, 13.1, 13.2, 13.3. Sustituye: 9.4.

- [x] 13.13 [M] Ejecutar REC-01/02 autorizados con integridad completa por LOTE. Área/archivos: `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs`, `Doc/Tecnica/Opsxj/doc-111-produccion-importacion-carga-carpeta/05-PruebasEvidencia.md`. Origen: D-12, RQ-12. Verificación: Ninguna raíz inicia efectos antes de la barrera global y la repetición conserva los mismos IDs/relaciones comprobados por SELECT. Depende de: 13.6, 13.9, 13.11, 13.1, 13.2, 13.3. Sustituye: 9.4.

- [x] 13.14 [M] Ejecutar REC-01/02 autorizados con PARCIAL_PERMITIDO. Área/archivos: `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs`, `Doc/Tecnica/Opsxj/doc-111-produccion-importacion-carga-carpeta/05-PruebasEvidencia.md`. Origen: D-12, RQ-12. Verificación: Avanzan elementos elegibles con NO_APLICA, se conservan rechazados/totales y la recuperación no duplica efectos confirmados. Depende de: 13.6, 13.9, 13.11, 13.1, 13.2, 13.3. Sustituye: 9.4.

- [x] 13.15 [M] Verificar degradación de la capacidad operativa ante esquema ausente o incompatible. Área/archivos: `Doc/Tecnica/Opsxj/doc-111-produccion-importacion-carga-carpeta/05-PruebasEvidencia.md`. Origen: D-12, RQ-12. Verificación: Solo la importación nueva queda indisponible y Producción/carga individual siguen operativas; cualquier alteración real requiere autorización concreta. Depende de: 4.1, 12.1, 13.10. Sustituye: 9.5.

- [x] 13.16 [M] Contrastar carga individual y preservación de los consumidores SII. Área/archivos: `tools/e2e/tests/doc88-production-document-upload-policy.test.cjs`, `Doc/Tecnica/Opsxj/doc-111-produccion-importacion-carga-carpeta/05-PruebasEvidencia.md`. Origen: D-12, RQ-12. Verificación: La evidencia separa revisión estática de carga individual real autorizada y no atribuye una importación SII no ejecutada. Depende de: 13.9, 13.10. Sustituye: 9.5.

- [x] 13.17 [M] Consolidar matriz de capacidades, SHA, resultados y límites para revisión final. Área/archivos: `Doc/Tecnica/Opsxj/doc-111-produccion-importacion-carga-carpeta/05-PruebasEvidencia.md`. Origen: D-12, RQ-12. Verificación: Cada capacidad indica real/local/NO_EJECUTADO y causa, con referencias verificables sin secretos. Depende de: 13.12, 13.13, 13.14, 13.15, 13.16. Sustituye: 9.6.

- [x] 13.18 [S] Validar coherencia final de OpenSpec, gobierno y contratos consumidores. Área/archivos: `openspec/changes/doc-111-produccion-importacion-carga-carpeta/tasks.md`. Origen: D-12, RQ-12. Verificación: OpenSpec estricto y auditoría OPSXJ pasan con evidencia requerida; 13 no se declara implementado por completar 04–12. Depende de: 13.17. Sustituye: 9.6.

Avance parcial adicional de 9.4 (2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-clasificacion-expediente-local-20261029.md`. El lector existente contrasta fondo/instrumento/área/serie/subserie/tipo/unidad persistidos, conserva alcance y cardinalidad y exige NULL para subserie ausente. Arnés Framework con registros sintéticos y build Debug PASS; no acredita causalidad, XML ni creación real. 9.3/9.4 siguen abiertas; 95/130 sin incremento. Funciones legacy y almacenamiento intactos.

Avance parcial de 9.3 (2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-comando-creacion-local-20261029.md`. Comando interno inmutable desde lote/raíz/reserva nuevos, contexto fijo y valores ya resueltos. Reutiliza validación/codec/huella y conversión canónica del resolvedor sin reconsultar fuentes. Guards, copias y conservación de valores probados en Framework; build Debug PASS. No invoca legacy ni habilita efectos; quedan composición de catálogos/conexión/argumentos, autoridad/barreras y comprobación de resultado. 9.3/9.4 abiertas; 95/130 sin incremento.

Avance parcial de 9.3 (catálogos, 2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-catalogos-creacion-local-20261029.md`. Reutiliza el catálogo compartido y su autoridad de nivel/gabinete/origen; añade nombres por IDs y comprobación inversa bajo el alcance legacy, responsable por ID interno resuelto y ausencias explícitas. Framework con SELECT sintéticos y build Debug PASS. Falta decidir origen del ciclo de archivo y componer/invocar/verificar la creación; 9.3/9.4 abiertas, 95/130. Sin efectos ni modificaciones de almacenamiento.

Avance parcial de 9.3 (compatibilidad legacy, 2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-compatibilidad-legacy-local-20261029.md`. El comando y catálogo existentes rechazan desbordamientos Integer y textos incompatibles con SQL concatenado sin cambiar los valores; la preparación comprueba entradas antes de leer y nombres después. Framework con registros sintéticos y build Debug PASS. No invoca legacy ni elige ciclo de archivo; 9.3/9.4 abiertas, 95/130. Funciones legacy y almacenamiento intactos.

Avance parcial de 9.3 (contexto/conexión, 2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-contexto-legacy-local-20261029.md`. El ContextAdapter existente exige Current, compara privadamente la conexión efectiva legacy con el resolvedor compartido y reutiliza autoridad vigente de creación, sello y gabinete. Framework con Session/SELECT sintéticos y build Debug PASS; sin mutación de Session, exposición de cadenas o efectos. Falta ciclo/composición/invocación/verificación; 9.3/9.4 abiertas, 95/130. Almacenamiento y funciones legacy intactos.

Decisión resuelta de 9.3 (ciclo, 2026-10-29): el usuario eligió «Archivo Gestión», reutilizando Producción. `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-ciclo-archivo-gestion-20261029.md`. Comando/catálogo exigen fila única por nombre exacto e ID canónico/Integer sin ID fijo. SELECT real en destino existente devolvió una fila, ID 4; pruebas Framework y build Debug PASS. No se crea ni almacena; quedan composición/invocación, barreras y resultado/correlación. 9.3/9.4 abiertas, 95/130. Esta decisión sustituye el pendiente de política de los cortes anteriores.

Evidencia local de 9.3/9.4 (2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-adaptador-creacion-local-20261029.md`. Puerto estrecho de creación recibe lote persistido; comando/catálogos/autoridad, transición durable RESERVADO→EN_CURSO y binding con argumentos explícitos a Conservación. Resultado incierto conserva IDs parciales y ciclo sin replay o mensajes privados. Verificación SELECT por IDs exactos contrasta alcance/clasificación y valores/ciclo/gabinete/responsable. Núcleo y lectores con dobles/registros sintéticos PASS; binding real compilado, no invocado. CAS/rollback MySQL, XML, persistencia final de fase (10.7), barreras/coordinación y aceptación real siguen pendientes. 97/130; sin efectos documentales ni cambios de almacenamiento/SII.

Avance parcial de 10.7 (creación, 2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-resultado-creacion-local-20261029.md`. Checkpoint transaccional del mismo repositorio para intento/raíz/lote/evento; idempotencia exacta, señal de detención preservada y marcador/IDs inciertos sin replay. Adaptador registra la salida y conserva incertidumbre ante pérdida de respuesta. Corregido enlace SQL del inicio a evidencia_doc con regresión contra DDL. Framework/ejecutor SQL sintético y build Debug PASS; no MySQL/efectos reales. 10.7 continúa abierta para las otras fases y coordinación; 97/130.

Avance parcial de 10.4–10.6 (creación, 2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-barreras-creacion-local-20261029.md`. Barrera pura de recepción por EXPEDIENTE/LOTE/parcial contra inventario original; lector acotado del repositorio, consulta previa y revalidación bajo bloqueo antes de iniciar creación. Reutiliza elegibilidad del plan sin recalcular fuentes o reducir totales. Framework/SELECT sintéticos y build Debug PASS. Sin comprobación física, coordinación integral o aplicación a fases posteriores; tareas abiertas, 97/130. Sin efectos reales ni cambios de almacenamiento.

Avance parcial de coordinación/barreras de creación (2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-staging-previo-creacion-local-20261029.md`. VerificarRetenido reutiliza hash/PDF/contención existentes; vista coherente con revisiones y paso interno que valida archivos físicos/autoridad antes de una reserva, con selección parcial sin reducir inventario. PDFs locales y reserva doble, Framework/build Debug PASS. Verificaciones transitorias, sin exclusión durable o coordinador integral/endpoint; 10.3/10.4–10.6 abiertas y 97/130. Sin efectos o SQL reales ni cambios de almacenamiento legacy.

Avance parcial de 10.3–10.7 (fase de creación, 2026-10-29): `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-fase-creacion-local-20261029.md`. Coordinador compone identidad/destino, huella histórica, barrera/staging/reserva, relectura y un creador; callback obligatorio conserva originales con handles de lectura durante efecto/verificación/checkpoint y libera en Finally. Repetición/commit con respuesta perdida se consultan sin nuevo escritor; detención/revocación en vuelo bloquean inicio. PDFs locales y núcleos reales con dobles de SQL/escritor, build Debug PASS. Composición real compilada, sin endpoint/SQL o efectos reales; faltan fases restantes y aceptación integral. 97/130, almacenamiento/SII intactos.

## Correspondencia con las tareas abiertas anteriores

Se conservan literalmente los dos trabajos completos y la aprobación continúa en 1.3. Esta tabla permite contrastar cada tarea sustituida con su desglose, manteniendo la cobertura de su decisión/requisito original:

| Tarea anterior | Tareas actuales |
|---|---|
| 2.1 | 5.1, 5.3, 5.4 |
| 2.2 | 3.8, 5.2 |
| 2.3 | 2.1, 5.1, 5.2, 5.3, 5.5, 5.6, 5.7, 5.8, 5.9 |
| 2.4 | 5.8, 5.9, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6, 7.8 |
| 3.1 | 3.1 |
| 3.2 | 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.9 |
| 3.3 | 2.1, 2.2, 2.3, 2.5, 2.6, 4.2 |
| 3.4 | 2.5, 4.1, 4.3, 4.4, 4.6, 4.7, 4.8 |
| 3.5 | 2.4, 3.6, 4.5, 10.1, 10.2 |
| 4.1 | 2.2, 7.1, 7.2, 7.3, 7.6 |
| 4.2 | 2.9, 7.4, 7.5, 7.6 |
| 4.3 | 2.7, 6.5, 7.7, 7.8, 7.9 |
| 4.4 | 2.3, 2.7, 7.10, 7.11, 7.12, 7.15 |
| 4.5 | 4.3, 4.4, 7.3, 7.13, 7.14, 7.15 |
| 5.1 | 2.8, 4.7, 8.1, 8.2 |
| 5.2 | 8.3, 8.4, 8.5 |
| 5.3 | 2.8, 5.7, 8.6, 8.7, 8.8, 8.9 |
| 5.4 | 8.7, 8.10, 8.11 |
| 6.1 | 9.1, 9.2 |
| 6.2 | 2.6, 9.3, 9.4 |
| 6.3 | 9.5, 9.6, 9.7, 9.8 |
| 6.4 | 2.6, 8.11, 9.9, 9.10 |
| 6.5 | 9.11, 9.12 |
| 7.1 | 10.2, 10.3, 10.11 |
| 7.2 | 10.4, 10.5, 10.6 |
| 7.3 | 10.7, 10.8, 10.11 |
| 7.4 | 10.9, 10.10, 10.11 |
| 8.1 | 12.1, 12.2, 12.3, 12.4, 12.9 |
| 8.2 | 12.5, 12.6, 12.7, 12.8 |
| 8.3 | 11.1, 11.2, 11.3, 11.4, 11.5 |
| 8.4 | 12.9, 12.10, 12.11 |
| 8.5 | 11.6, 11.7 |
| 9.1 | 13.1, 13.2, 13.3, 13.4, 13.5, 13.6 |
| 9.2 | 2.10, 6.6, 13.1, 13.2, 13.3, 13.7, 13.8, 13.9 |
| 9.3 | 13.10, 13.11 |
| 9.4 | 4.8, 5.8, 5.9, 13.5, 13.12, 13.13, 13.14 |
| 9.5 | 4.1, 13.15, 13.16 |
| 9.6 | 13.17, 13.18 |

Evidencia del corte 2026-10-27, tareas 2.1–2.10 y 3.1–3.9: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-contratos-esquema-20261027.md`. Esquema entregado y revisado, no aplicado; integración real pendiente.

Evidencia de 4.1 y SQL aplicado manualmente por el usuario: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-esquema-instalado-20261027.md. Verificacion real SELECT, sin escrituras del agente.

Evidencia de 4.2 y codigo de 4.3–4.7: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-persistencia-plan-lote-20261027.md. Pruebas locales PASS; verificaciones DML, transaccion y carrera MySQL pendientes; 4.3–4.8 permanecen abiertas.

Evidencia real autorizada de 4.3, 4.4, 4.7 y 4.8: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-persistencia-real-20261027.md. Perfil 24 restaurado inactivo; 4.5 y 4.6 abiertas por fallo intermedio/rollback no ejecutado.

Evidencia de 4.5, 4.6, 5.1 y 5.2: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-rollback-autoridad-20261027.md. Rollback real en tres puntos, filas preservadas, perfil 24 inactivo/revision 9; autoridad local y lectura RA real sin modificar permisos.

Evidencia local de 5.3–5.8: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-recursos-guard-20261027.md. Recursos, sello, guard JSON/multipart y matriz de facultades; no HTTP/E2E ni cambios de permisos. 5.9 permanece pendiente.

Evidencia de 6.1–6.5: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-lectura-operativa-20261027.md. Regresion local DOC-110 y lecturas reales de publicaciones 38/37; perfil 24 inactivo/revision 11; datos operativos preservados. 5.9/6.6 e integracion HTTP siguen pendientes.

Evidencia local de 7.7–7.12 y caracterización estática de 9.1–9.2: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-apertura-preflight-20261028.md. Apertura/dispatcher y cálculo de metadatos probados localmente; 7.13–7.15 siguen pendientes de identidad/coincidencias, integración y persistencia del plan del servicio. Sin HTTP/E2E real ni efectos documentales.

Evidencia local de 7.13–7.15 y 8.1–8.2: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-plan-confirmacion-20261028.md. Preparación/confirmación conectadas al repositorio único; coincidencias preventivas parametrizadas, sin reutilización de candidatos aún habilitada. Composición, round-trip local y ASMX en HttpContext sintético PASS. MySQL/HTTP reales del servicio y recepción de bytes pendientes, sin efectos documentales en este corte.

Evidencia local de 8.3–8.5 y 8.11: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-staging-privado-20261028.md. Puerto de staging privado, stream acotado, PDF validado y copia consumible independiente; pruebas Framework y build Debug PASS con archivos sintéticos locales. 8.6–8.10 siguen pendientes de composición, persistencia y handler. Sin recepción HTTP/MySQL real ni efectos documentales.

Evidencia local de 8.6–8.10: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-recepcion-integrada-local-20261028.md. Servicio, repositorio único y handler de recepción conectados; pertenencia/estado, replay por contenido y reconciliación probados con staging real y persistencia doble explícita. Build Debug PASS. Transacciones/rollback/carrera MySQL de recepción y pipeline HTTP reales NO_EJECUTADO; no se heredan autorizaciones previas ni se habilitan reclamaciones/efectos documentales de 10.2.

Evidencia local de 10.1 y caracterización estática de 9.5, 9.6, 9.8: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-transiciones-aislamiento-20261028.md. Tabla cerrada de seis dimensiones y guardas de recepción; build Debug y 16 pruebas locales PASS. Adaptador de almacenamiento deshabilitado por aislamiento externo no demostrado; TIFF/Multi-TIFF no soportado. 9.7 y ejecución/verificación documental siguen pendientes. Sin HTTP/E2E, SQL o efectos documentales reales en este corte.

Evidencia local de 10.2 y avance parcial de 10.9: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-reserva-detencion-local-20261028.md. Reserva documental transaccional en el repositorio único, replay sin nueva adquisición y señal de detención autorizada. Guards y carreras en doble explícito PASS; no se acredita bloqueo/rollback MySQL real. 10.9 sigue abierta por su dependencia de 10.7 y la aceptación con resultados documentales reales; 10.3/10.11 no se habilitan. La función de almacenamiento permanece intacta.

Evidencia local de 11.1–11.5: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-consulta-exportacion-local-20261028.md. Lectura autorizada, paginas por ID con filtros cerrados/totales originales y CSV de pagina protegido; ASMX reutiliza el dispatcher operativo. Acciones actuales CONSULTAR/EXPORTAR_PAGINA; no se anuncian capacidades de ejecucion pendientes. Politica explicita Doc111.Consulta.MaximoPagina requerida sin modificar Web.config. Pruebas con registros sinteticos/doble de lectura/HttpContext sintetico y build Debug PASS; MySQL y HTTP/IIS reales NO_EJECUTADO. Almacenamiento intacto; 11.6–11.7, interfaz y efectos siguen pendientes.

Evidencia local de 12.1–12.9: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-interfaz-local-20261028.md. Contenedor/disparador propios, etapas del prototipo, guard/transportes operativos, cola de una transferencia, revision/confirmacion servidor y resultados de pagina. Chromium local con servicios sinteticos, seleccion de carpeta sintetica, foco/390 px y confirmacion incierta PASS; 7 pruebas cliente nuevas/existentes y 29 regresiones locales PASS. Build Debug PASS. No HTTP/IIS/BD autenticados ni efectos reales; 12.10 sigue pendiente de persistencia tras recarga, reseleccion vinculada y recuperacion de confirmacion desconocida; 12.11 depende de adaptadores. Almacenamiento intacto.

Evidencia local de 12.10 y 13.1–13.3: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-recuperacion-local-20261028.md. Consulta de confirmacion por plan sin mutaciones, referencias solo de IDs en sessionStorage bajo contexto opaco, consulta manual por lote y reseleccion explicita por archivo pendiente. Chromium con recarga real, almacenamiento nativo y servicios interceptados localmente PASS; no se reconfirma ni se asocian homonimos por nombre. 10 pruebas cliente y 11 regresiones de politica PASS; arnes Framework y build Debug PASS. Sin autenticacion/SQL/HTTP del sistema ni efectos reales. Almacenamiento intacto; 12.11 y aceptacion integral siguen pendientes.

Evidencia local de 11.6: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-retencion-diagnostico-local-20261028.md. Diagnóstico interno paginado, solo lectura y sin endpoint/borrado; no accede al disco ni modifica registros/eventos. Ausencia de retención, incertidumbre, actividad, fecha inválida/futura, cambios de revisión o evidencia incompleta conservan los recursos. Un candidato solo requiere revisión posterior; eliminación siempre deshabilitada. Arnés Framework y build Debug PASS. No SQL/HTTP real ni limpieza ejecutados; 11.7 sigue pendiente de política/autorización y reconciliación documental.

Avance parcial de 9.4 y revisión de compatibilidad de 9.3: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-verificacion-expediente-local-20261028.md. Lector estructural por IDs conocidos en el mismo repositorio; comprobación de relación/nivel/empresa/creador/permiso y cardinalidad con registros sintéticos. No atribuye causalidad ni invoca creación. Ambas tareas siguen abiertas por la llamada compatible, clasificación completa y correlación; no se incrementa progreso. Contraste SELECT posterior reutilizando inspector/configuración/perfil existentes: Validacion/doc111-compatibilidad-creacion-select-20261028.md dentro del mismo directorio documental. DEFAULT actual de volumen 1, una ruta y una coincidencia de clasificación; sin creación, activación o almacenamiento.

Corte local 2026-10-30, reconciliacion (10.8): coordinador por intento/revision, comandos de verificacion sin permiso de ejecucion, lector con IDs persistidos y mismo repositorio para CAS/consolidacion transaccional. No vuelve a invocar creacion/almacenamiento ni libera reclamaciones antiguas sin prueba. Conserva detencion e incertidumbres ajenas; respuesta perdida se consulta sin repetir efectos. Evidencia local: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-reconciliacion-local-20261030.md. Framework y build completos PASS con SELECT/SQL/writers dobles declarados; sin efectos/MySQL/HTTP reales. 110/130; 10.11 HTTP sigue pendiente. La exclusion adicional del XML frente a otras cargas permanece descartada por el usuario.

Corte local 2026-10-30, fachada HTTP (10.11): AvanzarLote, SolicitarDetencion y ReconciliarRecurso reutilizan guard/dispatcher operativo y coordinadores existentes. Una fase cerrada por recurso/revision, sin encadenar efectos ni endpoints de adaptador. Proyeccion publica de IDs/estado; incertidumbre 409 y rechazo de valores 422, sin evidencia privada. Consulta publica la referencia numerica del intento activo para recuperar respuestas perdidas, sin token. Binding productivo separado y compilado; arnes con HTTP sintetico, coordinadores reales y escritores/SQL/proveedores dobles. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-http-fases-local-20261030.md. 111/130; conexion del cliente/actualizacion de rama y aceptacion real pendientes. Sin E2E autenticado, MySQL o efectos reales; almacenamiento legacy intacto. La exclusion adicional del XML frente a otras cargas permanece descartada por el usuario.

Corte local 2026-10-30, cliente/rama (12.11, 13.8 y 13.9): la UI conecta fases explicitas, detencion y verificacion a las operaciones existentes sin encadenar ni repetir efectos. La consulta anuncia solicitudes restrictivas y proyecta NOMBRE confirmado/valores fijados. La rama se agrega por relacion/nivel/expediente con create_trenode_treview existente: seleccion/expansion conservadas, homonimos distintos, consultas sin duplicados y rama ausente/contexto cambiado sin invalidar resultados. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-cliente-rama-local-20261030.md. 13 pruebas cliente Chromium/locales y 21 regresiones locales PASS; arnes Framework y build Debug separado PASS; 87 componentes registrados, fixtures DOC111 excluidas. 114/130. Drivers/selector REC, aceptacion real autorizada y limpieza con politica siguen pendientes; sin HTTP/BD/E2E del sistema ni efectos reales. Almacenamiento legacy intacto; exclusion adicional del XML descartada por el usuario.

Corte local 2026-10-30, drivers/selector REC (13.4-13.7): un recorrido REC-01/02 reutilizado por EXPEDIENTE, LOTE y PARCIAL en el paquete Playwright existente, con helper de sesion/transporte y consola TTY compartidos. Comprueba barrera antes de completar recepcion, dos raices, valores esperados antes de almacenamiento, SELECT de destino/clasificacion/Archivo Gestion/documento/inventario/indice/campos y repeticion/reapertura sin cambiar IDs o eventos. Selector --check sin efectos; corrida real requiere autorizaciones concretas y secretos solo en TTY/entorno efimero. Sin retries/redirecciones, referencias saneadas de recuperacion y resumen cerrado; un skip o codigo cero sin resultado completo no aprueba. 43 pruebas locales conjuntas PASS y tres casos listados, sin ejecutar REC autenticados ni conexiones reales. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-rec-drivers-local-20261030.md. 118/130; aceptacion real y limpieza con politica siguen pendientes. Almacenamiento legacy/SII intactos, sin nueva config o plataforma; exclusion adicional del XML descartada por el usuario.

Corte local 2026-10-30, limpieza controlada (11.7): capacidad interna sin endpoint/job, deshabilitada por defecto y con retencion servidor positiva explicita. Solo lote completo con inventario/checkpoints concordantes y sin actividad/incertidumbre; fecha mas reciente de lote/finalizacion de intentos y SELECT actuales de documento/relaciones/valores. Reutiliza staging privado y registro PREPARAR_LIMPIEZA confirmado antes de retirar un .bin exacto; segunda transaccion revalida y registra conclusion, sin prometer atomicidad disco/MySQL. Respuesta perdida conserva diario para recuperar ausencia autorizada; nunca repite efecto documental ni borra metadatos/partes/historial. Revocacion, politica ampliada/deshabilitada, evidencia cambiante y original alterado/retenido impiden retiro. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-limpieza-local-20261030.md. Arnes Framework con PDF sintetico y SQL dobles, build Debug separado y coherencia PASS. 119/130; sin habilitar politica operativa ni limpiar datos del sistema, sin MySQL/HTTP/E2E reales. Funciones legacy/SII intactas; exclusion adicional del XML descartada por el usuario.
