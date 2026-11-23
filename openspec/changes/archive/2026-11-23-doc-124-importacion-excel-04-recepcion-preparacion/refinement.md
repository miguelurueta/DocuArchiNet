<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento DOC-124

## Fuente y alcance

Revisión técnica Codex2026-11-22 sobre solicitud «continua» después de iniciar DOC124. Se concreta el refinamiento y se sustituye el plan genérico con tareas trazables; no se implementa producto en esta entrega. Perfil legacy-webforms-vb, base DOC123 integrado87d155b7. Approved expresa revisión técnica del plan, sin certificar ejecución, SQL instalado o autorización E2E.

## Contexto inspeccionado

- WebServiceImportacionExcel: cuatro operaciones JSON y composición autorizada DOC122; guarda actual solo admite application/json/262144bytes.
- ImportacionCarpetasUpload e ImportacionCarpetasRequestGuard.LeerMultipart: autoridad y token antes de Form/Files, un archivo, campos únicos y límites. Reutilizar patrón sin solicitar permiso de carpetas ni aceptar PDF.
- AlmacenStagingImportacion: .part exclusivo, publicación/retención, referencias privadas y comprobación de enlaces; su implementación actual requiere lote/archivo documental y valida PDF, por lo que no se reutiliza como almacén Excel sin adaptación.
- Sql/03-ejecucion-da-doc121.sql:22 columnas, bytes/hash no nulos, estados por aplicación e índices de propietario/destino. Sin columna para una recepción incompleta; no registrar bytes/hash ficticios.
- DocumentoStreamingReferencia.Emitir/TryResolver: patrón MachineKey.Protect existente para referencias con propósito y vencimiento, sin nuevos secretos.
- ImportacionExcelConexionAdapter: DA/módulo de sesión, misma empresa y fábrica revalidada; no cuenta nueva o cambio MySql.Data.
- MySqlFuenteImportacionRepository y PoliticaFuentesProveedor: aislamiento explícito CONEXION/COLUMNA; los guardas administrativos exigen permisos distintos que no se deben fingir al consultar política para cargas.
- MySqlPlantillaExcelRepository.LeerDestino: metadatos de columnas, pero falta modelo de índices/aislamiento para preparar; ServicioPlantillasExcel.Huella conserva snapshot v1.
- ServicioLecturaExcel y ResultadoLecturaExcel: límites/tipos/errores precisos; salida inválida elimina filas y muestra interna de incidencias1000. La preparación no puede inventar duplicados sobre filas que el lector rechazó.
- Harness DOC122/DOC110, fixtures DOC123 y E2E importacion-excel.spec.cjs: reutilizables. El skip global DOC122 se debe separar por suite al añadir alcance DOC124; no heredar su autorización.
- Manual MySQL5.1 y reglas SQL del repositorio revisadas; temporales son por conexión y requieren limpieza explícita aun ante rollback.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
|---|---|---|---|---|---|
| D-01 | Verificar en cada operación usuario activo, empresa, permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1, DA/módulo vigentes y CSRF propio. Extender guarda multipart sin debilitar POST JSON ni exigir permisos de perfiles/fuentes. | ImportacionExcelContextAdapter.Resolver; ImportacionExcelRequestGuard.Validar/ValidarTransporte; ImportacionCarpetasRequestGuard.LeerMultipart | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Contar bytes y SHA256 durante copia acotada a archivo .part exclusivo, validar XLS/XLSX real y publicar completo sin sobrescribir. Referencias/rutas generadas por servidor bajo almacenamiento protegido; RECIBIDA exige archivo completo verificable. | AlmacenStagingImportacion.Recibir/ConservarVerificado; LectorArchivoExcel; Sql/03-ejecucion-da-doc121.sql | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Emitir ID de carga UUID y referencia protegida antes del multipart, sin insertar metadatos ficticios. Repetición explícita del mismo ID/contenido reutiliza recepción propia; conflicto de contenido o escritor activo no se reintenta automáticamente. | DocumentoStreamingReferencia.Emitir/TryResolver (MachineKey); ServicioRecepcionImportacion.Recibir; PK imp01_carga_ejecucion.id_carga | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Resolver DA por recursos del módulo sin otra conexión/conector. Para tabla existente exigir política empresarial explícita de servidor; CONEXION exclusiva o COLUMNA con empresa+clave. No inferir exclusividad por prefijo/nombre de columna ni autoridad desde Excel. | ImportacionExcelConexionAdapter.Resolver; MySqlFuenteImportacionRepository.Cabecera/CargarPoliticaEmpresa; PoliticaFuentesProveedor; MySqlProveedorDatosIndices.ConsultarDatos | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Usar ServicioLecturaExcel con límites positivos del servidor,10000 filas por defecto y20000 configuradas por solicitud del usuario2026-11-23, con10MB decimal, conservando coordenadas/tipos. Ninguna muestra o prefijo acredita validez; errores detienen publicación PREPARADA y mantienen RECIBIDA. | ServicioLecturaExcel.Leer; ResultadoLecturaExcel.Valido/LecturaCompleta; ImportacionExcelConexionAdapter.Limite | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | En destinos existentes verificar InnoDB, tipos/longitudes/charset/nulabilidad actuales y UNIQUE de clave completa, o empresa+clave completa. Rechazar índice de prefijo/no único/con columnas adicionales; CREAR solo proyecta estructura y comprueba nombre disponible. | MySqlPlantillaExcelRepository.LeerDestino; ServicioPlantillasExcel.ValidarDestino/TipoCreable; DOC121 D-05 y snapshot de esquema | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Comparar claves mediante temporales de la misma conexión con tipo, charset/collation y unsigned/precisión exactos. Distinguir duplicados internos y claves existentes al AGREGAR; REEMPLAZAR admite coincidencias del ámbito sustituido, CREAR exige unicidad interna. | MySqlProveedorDatosIndices.ConsultarDatos; PerfilImportacionCatalogAdapter.ValorCompatible; manual MySQL5.1 CREATE TEMPORARY TABLE/GROUP BY | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Persistir documento efectivo canónico y hashes de archivo/configuración/esquema, revisión, hoja, modo, aislamiento y filas previstas. Mantener huella v1 de plantilla dentro del documento; relectura final de autoridad/plantilla/esquema invalida cambios observados. | ServicioPlantillasExcel.Huella; DOC121 snapshot v1; imp01_carga_ejecucion.configuracion_doc/configuracion_hash/esquema_hash | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Serializar recepción/preparación del mismo ID y publicar estado con transacción InnoDB y relectura propia; sin transacción abierta durante copia/parseo. RECIBIDA→PREPARADA solo tras validar. Fallo conocido no sobrescribe ganador y acuse incierto exige consultar. | ServicioRecepcionImportacion.Recibir; imp01_carga_ejecucion PK/estado; patrones FOR UPDATE de MySqlPlantillaExcelRepository.Guardar | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Preparar devuelve hasta25 filas/25 incidencias con totales observados y truncamiento. ConsultarCargaExcel es lectura propia, nunca prepara/confirma. No devolver rutas, SQL, secretos o recursos ajenos; ninguna tabla destino se modifica. | DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb; WebServiceImportacionExcel.Ejecutar; DOC121 contrato request/response | D-10 | RQ-10 | Origen: D-10, RQ-10 |
| D-11 | Liberar temporales y lock y restaurar sql_mode de sesión en Finally; si limpieza no se acredita, descartar conexión del pool. No modificar modo global, cuenta o versión MySql.Data; un lock vencido por tiempo no autoriza tomar un escritor. | ImportacionExcelConexionAdapter.ConexionVigente; AlmacenStagingImportacion; manual MySQL5.1 temporales y locks de conexión | D-11 | RQ-11 | Origen: D-11, RQ-11 |
| D-12 | Reutilizar las22 columnas de ejecución y scripts históricos sin reaplicarlos. Entregar preflight SELECT nuevo; diferencias permanentes requieren otro SQL manual y confirmación antes de SELECT posterior. No modificar EGRESOS/CONTRATOS ni consumidores SII/carpetas. | Sql/03-ejecucion-da-doc121.sql; Sql/README.md; AGENTS.md; DOC123 cerrado87d155b7 | D-12 | RQ-12 | Origen: D-12, RQ-12 |
| D-13 | Extender el ejecutable/harness DOC122/DOC110 y fixtures DOC123 con fallos entre filesystem/DB, rollback propio, modos, hashes, límites y concurrencia. Build aislado y evidencia distinguen dobles sintéticos de MySQL real. | tests/ImportacionExcelBehaviorTests.cs; tests/ImportacionExcelLecturaTests.cs; tests/doc122-importacion-excel-behavior.test.cjs | D-13 | RQ-13 | Origen: D-13, RQ-13 |
| D-14 | Extender tools/e2e existente para ASHX/ASMX reales, sin infraestructura paralela ni heredar autorización DOC122. Reutilizar las tablas consumidas y los controles existentes, conservando los destinos mediante SELECT y huellas completas. Acreditar con autorización específica ámbito/cuentas/archivos y ejecuciones de prueba: acceso, tres modos, dos sesiones y destino sin mutación; no exigir tablas nuevas o declarar descartables las existentes. | tools/e2e/tests/importacion-excel.spec.cjs; run-doc110-profile-session.cjs; AGENTS.md; tools/e2e/AGENT-RUNBOOK.md | D-14 | RQ-14 | Origen: D-14, RQ-14 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
|---|---|---|---|
| RQ-01 | Autoridad y transporte propios | Permiso propio y revocación; Multipart forjado | D-01; controles y límites en design/spec/tasks |
| RQ-02 | Archivo privado e inmutable | Archivo completo verificado; Fallo durante recepción; Formato disfrazado y ruta hostil | D-02; controles y límites en design/spec/tasks |
| RQ-03 | ID previo y recuperación del acuse | Acuse perdido; Replay y escritor activo | D-03; controles y límites en design/spec/tasks |
| RQ-04 | Destino y aislamiento de servidor | Política de tabla compartida; Política ausente o contradictoria; Nuevo destino | D-04; controles y límites en design/spec/tasks |
| RQ-05 | Lectura completa y límites heredados | Fronteras de archivo y filas; Lector inválido | D-05; controles y límites en design/spec/tasks |
| RQ-06 | Índice único y esquema físico | Clave física válida; Índice insuficiente; Cambio concurrente de estructura | D-06; controles y límites en design/spec/tasks |
| RQ-07 | Duplicados con semántica MySQL | Igualdad del destino; Tres modos | D-07; controles y límites en design/spec/tasks |
| RQ-08 | Snapshot efectivo y detección de cambios | Editor antiguo y cambios de hoja; Archivo o política alterados | D-08; controles y límites en design/spec/tasks |
| RQ-09 | Estados y concurrencia por carga | Publicación ganadora; Interrupción y reintento | D-09; controles y límites en design/spec/tasks |
| RQ-10 | Vista previa y consulta acotadas | Muestra y totales; Consulta propia sin efectos | D-10; controles y límites en design/spec/tasks |
| RQ-11 | Temporales, pool y recursos recuperables | Limpieza y reutilización; Cancelación sin toma de escritor | D-11; controles y límites en design/spec/tasks |
| RQ-12 | SQL manual y compatibilidad | Esquema ya instalado; Diferencia de instalación | D-12; controles y límites en design/spec/tasks |
| RQ-13 | Pruebas locales reutilizadas | Fallos reproducibles; Regresión y build | D-13; controles y límites en design/spec/tasks |
| RQ-14 | E2E real de recepción y preparación | Recorrido real autorizado; Concurrencia independiente y gate | D-14; controles y límites en design/spec/tasks |

## Resultado del refinamiento

Catorce decisiones/requisitos trazados y escenarios verificables. Se reaprovechan22 columnas existentes; una necesidad permanente nueva se entrega en otro SQL manual, sin cambiar los históricos. Preflight y E2E real tendrán evidencia posterior: esta revisión solo acredita diseño/plan.

El alcance de recepción incluye ID previo a bytes y consulta del mismo ID; no promete pausa/reanudación por fila, retry automático o congelar escritores externos. Preparar no escribe tablas destino. La ejecución real requiere autorización específica posterior de ambiente/cuentas/recursos descartables y no queda aprobada por esta solicitud.
