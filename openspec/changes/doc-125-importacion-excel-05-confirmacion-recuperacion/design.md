<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14,D-15 -->
## Context

DOC125 parte de DOC124 integrado en main (merge 423f0c2387f4965ee4c7313cb0e3fa1d0d148820). La carga persiste archivo y configuración privados, snapshots v1 y estados RECIBIDA/PREPARADA; el repositorio ya dispone de GET_LOCK por ID y CAS. La tabla operativa tiene 22 columnas, incluidas filas_confirmadas y confirmado_utc que aún no se leen en los modelos. ConsultarCargaExcel se compone hoy con límites de lector: se corregirá esa dependencia para consultar historia aunque cambien límites.

La ejecución real DOC124 preparó seis casos y conservó 625 contratos. Su autorización de destinos de sólo lectura no habilita las escrituras de esta fase. Los 20000 configurados sustituyen para este ambiente el valor inicial de 10000 del prompt, que sigue siendo el predeterminado del lector.

## Goals / Non-Goals

**Goals:** confirmar una sola vez por ID/contenido; filas y resultado atómicos; rollback íntegro; CREAR recuperable con procedencia; aislar por destino y preservar ámbito; contratos y pruebas revisables antes de operar.

**Non-Goals:** pausa durable a mitad de filas, commits parciales, CSV, omisión/upsert, menús o interfaz fase06, edición de SII/perfiles, publicación automática de fuentes nuevas, migraciones automáticas, garantía frente a DDL administrativo o garantías de capacidad sin medición.

## PRECONDICIONES DE RUTAS

Inventario contrastado con la carpeta principal antes de implementar. Las rutas nuevas son entregables; no existen aún. Se mantiene la división por responsabilidad de DOC124 y se amplían sus harness.

| Ruta | Situación | Responsabilidad |
| --- | --- | --- |
| `Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb` | existente | Extender resultado y contratos internos de confirmación sin romper las sesiones DOC124. |
| `DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb` | existente | SolicitudConfirmarExcel, resultado seguro y consulta de resultado. |
| `Services/ProduccionDocumental/ImportacionExcel/ServicioConfirmacionExcel.vb` | nuevo | Orquestar autoridad, replay, revalidación y confirmación. |
| `Services/ProduccionDocumental/ImportacionExcel/ReglasCargaExcel.vb` | existente | Comparar identidad/snapshots y proyectar resultado seguro. |
| `Services/ProduccionDocumental/ImportacionExcel/ServicioPreparacionExcel.vb` | referencia | Conservar Documento v1 y sus cargas; no reabrir ventanas previas. |
| `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb` | reutilización | Lectura completa y normalización DOC123. |
| `Services/ProduccionDocumental/ImportacionExcel/ServicioRecepcionExcel.vb` | existente | Mantener consulta y recepción compatibles al extender resultado. |
| `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlCargaExcelRepository.vb` | existente | Lectura de resultados, sesión de confirmación, locks y transacción/CAS. |
| `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlConfirmacionExcelWriter.vb` | nuevo | DML parametrizado y resultado atómico sobre conexión/tx del repositorio. |
| `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlEstructuraExcel.vb` | nuevo | Inspección de seguridad, proyección CREATE, procedencia y recuperación. |
| `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPreparacionExcelValidator.vb` | existente | Reutilizar comparación/SQL estricto; admitir DbTransaction explícita sin alterar la huella v1. |
| `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb` | existente | Exponer lectura bloqueada de cabecera/campos en conexión y tx compartidas. |
| `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/SqlCargaExcel.vb` | reutilización | Comandos/identificadores/parámetros. |
| `Infrastructure/ProduccionDocumental/ImportacionExcel/ComposicionCargaExcel.vb` | existente | Componer confirmación y posponer límites/lector para consulta y replay histórico. |
| `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb` | existente | Resolver DA y límites por operación, sin conexión nueva en Web.config. |
| `Infrastructure/ProduccionDocumental/ImportacionExcel/PoliticaDestinoCargaExcelAdapter.vb` | reutilización | Política CONEXION/COLUMNA explícita. |
| `Infrastructure/ProduccionDocumental/ImportacionExcel/AlmacenArchivoExcel.vb` | reutilización | Retener archivo privado y comprobar integridad. |
| `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb` | reutilización | Sesión/CSRF y transporte. |
| `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelContextAdapter.vb` | reutilización | Autoridad vigente. |
| `webservice/WebServiceImportacionExcel.asmx.vb` | existente | Métodos Confirmar y ConsultarResultado; conservar métodos actuales. |
| `generic_control/ImportacionExcelUpload.ashx.vb` | regresión | Sin protocolo nuevo de recepción. |
| `GestionDocumental-Docuarchi.net.vbproj` | existente | Registrar servicios y repositorios nuevos. |
| `Web.config` | referencia | Conservar MaxFilasDatos=20000, conexiones y límites vigentes. |
| `tests/ImportacionExcelConfirmacionTests.cs` | nuevo | Pruebas dentro del harness compilado existente. |
| `tests/doc122-importacion-excel-behavior.test.cjs` | existente | Compilar fuentes nuevas y ejecutar suite nativa ampliada. |
| `tests/ImportacionExcelBehaviorTests.cs` | existente | Punto de entrada y contratos/regresión existentes. |
| `tests/ImportacionExcelPreparacionTests.cs` | reutilización | Dobles/snapshots/preparación y regresión. |
| `tests/doc125-e2e-reuse.test.cjs` | nuevo | Guardas locales del nuevo scope en el mismo runner. |
| `tools/e2e/tests/importacion-excel.spec.cjs` | existente | Ampliar el recorrido real de confirmación. |
| `tools/e2e/scripts/run-doc110-profile-session.cjs` | existente | Nuevo modo explícito en la consola privada existente. |
| `tools/e2e/tests/support/doc125-excel-scope.cjs` | nuevo | Scope exclusivo de confirmación que reutiliza utilidades DOC124. |
| `Doc/Actualizacion/Importacion/Sql/18-preflight-doc125.sql` | nuevo | Archivo de diagnóstico exclusivamente SELECT. |
| `Doc/Actualizacion/Importacion/Sql/README.md` | existente | Guía manual y diferencia instalación/operación. |
| `Doc/Actualizacion/Importacion/Diseno/05-confirmacion-recuperacion-doc125.md` | nuevo | Contrato/diseño de fase, enlazado a OpenSpec. |
| `Doc/Actualizacion/Importacion/Validacion/doc125-refinamiento-20261123.md` | nuevo | Resultado de la revisión y sus límites. |
| `Doc/Tecnica/Opsxj/doc-125-importacion-excel-05-confirmacion-recuperacion/` | existente | Cinco documentos obligatorios de gobierno. |

## Decisions

### D-01 / RQ-01 — Autoridad y petición de confirmación

Confirmar recibe ConexionId, IdCarga, ArchivoHash, ConfiguracionHash, EsquemaHash y ConfirmacionReemplazo. Sesión, permiso vigente, empresa, módulo y conexión resueltos por los adaptadores actuales gobiernan la operación. Validar UUID, hashes y propiedad antes de revelar estado; no aceptar tabla, SQL, filas ni credenciales del navegador.

Evidencia inspeccionada: `ImportacionExcelRequestGuard.Validar`, `ReglasCargaExcel.Propietario`, `ServicioRecepcionExcel.Consultar` y DTOs existentes.

### D-02 / RQ-02 — Snapshots y validación completa

Reutilizar ConfiguracionDoc v1 sin reescribir cargas DOC124. Verificar su hash, archivo privado retenido, lectura completa, hoja, modo, plantilla completa, revisión, identidad física, política y esquema. Las cargas por ejecutar contrastan los límites actuales con los registrados; cualquier cambio exige nueva preparación o nueva carga según su estado. Bloquear y releer cabecera y campos en la transacción de escritura; comprobar de nuevo autoridad y política antes del commit.

Evidencia inspeccionada: `ServicioPreparacionExcel.Documento`, `ReglasCargaExcel.Plantilla`, `AlmacenArchivoExcel.Retener`, `ServicioLecturaExcel.Leer` y `MySqlPlantillaExcelRepository.Leer`.

### D-03 / RQ-03 — Unidad transaccional durable

Usar una conexión DA para la transacción InnoDB de filas y resultado, con FOR UPDATE de la ejecución seguido de cabecera y campos. Incorporar filas_confirmadas y confirmado_utc ya existentes a modelo, SELECT y DTO seguro. Persistir CONFIRMADA con conteo exacto, hashes originales y UTC dentro del mismo commit que las filas. La publicación usa identidad, estado y ConfiguracionHash esperados; no usar el Publicar de preparación para confirmar.

Evidencia inspeccionada: `MySqlCargaExcelRepository.Leer/Publicar`, `MySqlPlantillaExcelRepository.Leer/Guardar` y SQL03 de DOC121 (22 columnas).

### D-04 / RQ-04 — Bloqueos compatibles con MySQL 5.1

Conservar el GET_LOCK por ID de DOC124 en la conexión de datos y adquirir el bloqueo por destino en una segunda conexión del mismo recurso DA. Orden fijo: carga, destino, ejecución FOR UPDATE, plantilla. GET_LOCK con espera cero; nombre de destino derivado del servidor/puerto físicos y base/tabla canónicas según lower_case_table_names, sin usuario, empresa, plantilla ni alias de conexión. Esta canonización de lock no modifica IdentidadFisica de los snapshots v1. Verificar ambas identidades físicas y posesión antes de efectos/commit; cada conexión libera solamente su bloqueo y se descarta si no queda limpia. En MySQL 5.1 un segundo GET_LOCK en la misma conexión libera el primero.

Evidencia inspeccionada: `MySqlCargaExcelRepository.Sesion.New/Dispose` y `MySqlPreparacionExcelValidator.Identidad`; documentación oficial MySQL sobre GET_LOCK anterior a 5.7.5.

### D-05 / RQ-05 — Escritura tipada y acotada

Reutilizar valores normalizados y SqlCargaExcel. Insertar sólo columnas mapeadas con identificadores validados y valores parametrizados, incluyendo DBNull, fechas y números canónicos sin pérdida de precisión. Agrupar hasta 128 parámetros y presupuesto de bytes conservador min(65536, max_allowed_packet/2); una fila que no cabe se rechaza antes del DELETE/INSERT. Mantener sql_mode estricto en la conexión DA y considerar advertencias de conversión como fallo; restaurar modo y temporales al terminar.

Evidencia inspeccionada: `MySqlPreparacionExcelValidator.Insertar/ActivarModoEstricto/Cerrar`, `SqlCargaExcel.Cmd/Identificador` y `ValorNormalizadoExcel`.

### D-06 / RQ-06 — Agregar y carrera de clave

AGREGAR repite dentro de la transacción la comparación tipada/collation contra el destino y mantiene el UNIQUE completo. No usar IGNORE, REPLACE, ON DUPLICATE KEY UPDATE ni omisión de filas. Una colisión con cualquier restricción única, incluso introducida por otro escritor entre validación e INSERT, revierte todas las filas del intento.

Evidencia inspeccionada: `MySqlPreparacionExcelValidator.Validar/Claves` e índices completos acreditados en DOC124.

### D-07 / RQ-07 — Reemplazar dentro del ámbito

REEMPLAZAR valida ConfirmacionReemplazo=True, archivo íntegro y política explícita. CONEXION permite DELETE del contenido autorizado de la tabla exclusiva; COLUMNA parametriza empresa tanto en DELETE como en la validación de todas las filas de entrada. DELETE e INSERT están en la misma transacción, sin TRUNCATE ni recrear índices. Unicidad de la entrada se exige; coincidencias con claves sustituidas son válidas.

Evidencia inspeccionada: `PoliticaDestinoCargaExcelAdapter.Seleccionar`, `MySqlPreparacionExcelValidator.Claves/Validar` y modo histórico 0/1/0.

### D-08 / RQ-08 — Crear con procedencia recuperable

CREAR tiene una fase estructural anterior a la transacción DML: InnoDB, tipos proyectados, charset/collation autorizado, clave NOT NULL y UNIQUE completo. Incluir en el mismo CREATE TABLE un comentario versionado con UUID y EsquemaHash preparado; no usar IF NOT EXISTS ni apropiarse de tablas existentes. Comparar estructura materializada con la proyección normalizando sólo representaciones equivalentes de MySQL (p. ej., ancho de visualización INT y NUMERIC/DECIMAL), sin ignorar precisión, longitud, nullabilidad, índices, charset o collation. Conservar el hash v1 original. Una tabla propia compatible y vacía permite reintento; una tabla ajena, alterada o con filas no explicadas produce conflicto. No DROP, TRUNCATE ni limpieza automática.

Evidencia inspeccionada: `MySqlPreparacionExcelValidator.Proyectar/HuellaEsquema`, `ServicioPreparacionExcel.Documento` y reglas de creación de DOC121.

### D-09 / RQ-09 — Fallos conocidos e incertidumbre

Después de rollback conocido registrar FALLIDA con código acotado mediante otra transacción corta y CAS de identidad/estado/hash, conservando archivo y snapshots. Permitir reintento explícito del mismo ID FALLIDA sólo con la misma intención íntegra y revalidación actual; si cambió la configuración se requiere una carga nueva. Un error de COMMIT, corte de conexión o fallo de limpieza después de publicar devuelve RESULTADO_INCIERTO: no marcar FALLIDA, no degradar CONFIRMADA y no repetir automáticamente. Si tampoco puede persistirse un fallo, conservar resultado incierto.

Evidencia inspeccionada: `MySqlCargaExcelRepository.Publicar/Rechazar/Dispose`, estados DOC121 y consulta filtrada DOC124.

### D-10 / RQ-10 — Resultado histórico y consulta sin efectos

Agregar ConsultarResultado y ampliar ConsultarCargaExcel para leer filas_confirmadas/confirmado_utc. Tras comprobar identidad y contenido de la petición, CONFIRMADA se devuelve desde persistencia antes de leer plantilla, límites, archivo o esquema actuales. Refactorizar la composición para resolver límites/lectores sólo cuando una carga por ejecutar los necesita. La consulta no adquiere GET_LOCK ni escribe; sólo SELECT y, para CREAR sin confirmar, inspección de comentario/estructura propia. No exponer connection IDs, rutas privadas ni documentos internos; ausencia o estado no concluyente mantiene RESULTADO_INCIERTO.

Evidencia inspeccionada: `ComposicionCargaExcel.New`, `ServicioRecepcionExcel.Consultar`, `ReglasCargaExcel.Estado` y `MySqlCargaExcelRepository.Consultar`.

### D-11 / RQ-11 — Elegibilidad física antes de escribir

La confirmación acredita InnoDB en tablas de ejecución/configuración/destino, tabla base, clave completa no nula y esquema compatible. Inspeccionar triggers y claves foráneas entrantes/salientes tanto del destino como del registro de ejecución: cualquier trigger o FK de las tablas que se escriben bloquea la confirmación en esta fase. Comprobar visibilidad suficiente de metadatos, incluido privilegio TRIGGER efectivo; un resultado vacío sin ese privilegio no demuestra ausencia. La inspección de relaciones debe cubrir también referencias desde otras tablas, no sólo las visibles por permisos parciales. Si no puede acreditarse la inspección, cerrar la capacidad y entregar la adecuación de permisos como SQL manual específico. Extender inspección de confirmación sin cambiar la huella v1 de preparación. No autorizar mantenimiento DDL concurrente; el módulo no ofrece exclusión frente a administradores que lo ignoren.

Evidencia inspeccionada: `MySqlPreparacionExcelValidator.ValidarEsquema/HuellaEsquema`, `SqlCargaExcel.Identificador` y metadatos information_schema.

### D-12 / RQ-12 — Límites y recuperación de ejecución

Conservar ImportacionExcel.MaxFilasDatos=20000 y MaxArchivoMB=10 (10000000 bytes), con predeterminado del lector 10000. Releer la configuración al confirmar una carga sin ejecutar; no usar valores HTTP. Una carga tiene una sola transacción DML aunque use comandos agrupados: no existe pausa durable a mitad de filas ni commits por lote. Antes de confirmar puede abandonarse conservando PREPARADA; después de un corte se consulta el mismo ID. Medir tiempos de etapas y memoria sobre volúmenes autorizados sin transformar una prueba local o el mayor archivo previo de 12759 filas en garantía de capacidad.

Evidencia inspeccionada: `ImportacionExcelConexionAdapter.Limite`, `Web.config`, `ServicioLecturaExcel.Leer` y aceptación documentada DOC124.

### D-13 / RQ-13 — Instalación y SQL manual

Reutilizar las 22 columnas de imp01_carga_ejecucion y las configuraciones DOC121–124. Entregar SQL18 sólo SELECT como preflight y guía por RA/DA. Si el diagnóstico descubre una adecuación necesaria, crear un archivo nuevo numerado con precondiciones/efectos/SELECT posteriores; nunca reeditar scripts ya aplicados ni instalarlos desde tests/arranque. Registrar PENDIENTE_EJECUCION_MANUAL hasta confirmación del usuario. El CREATE operativo solicitado por Confirmar es funcional; el alta de plantillas/fuentes/permisos de instalación se entrega como SQL manual.

Evidencia inspeccionada: `Doc/Actualizacion/Importacion/Sql/03-ejecucion-da-doc121.sql`, SQL15–17 aplicados y `Sql/README.md`.

### D-14 / RQ-14 — E2E existente y autorización de efectos

Extender importacion-excel.spec.cjs, run-doc110-profile-session.cjs, controles SELECT DOC32 y harness nativo DOC122, sin otra autenticación/conexión web. Añadir --excel-confirm y scope DOC125 específico con ambiente, cuentas, tabla descartable, modos, archivos, hashes, máximos y efectos permitidos; nunca convertir destinationReadOnly de DOC124 en permiso de escritura. Las cargas/CONTRATOS/Órdenes reales anteriores y sus resultados se conservan. Preparar escenarios de rollback/carrera con dobles locales y ejecutores nativos existentes; no añadir endpoints de inyección de fallos ni instalación/siembra/limpieza automática. El escritor externo de la carrera es una operación de prueba explícitamente autorizada y acotada, con datos sintéticos conservados.

Evidencia inspeccionada: `tools/e2e/tests/importacion-excel.spec.cjs`, runner DOC110, `doc124-excel-scope.cjs`, `tests/ImportacionExcelPreparacionTests.cs` y runbook.

### D-15 / RQ-15 — Compatibilidad y entrega verificable

Conservar net461, MySQL5.1.37, MySql.Data6.2.3, lector DOC123 y resolución RA/DA vigente. Registrar archivos nuevos de compilación en vbproj y ampliar el mismo harness con pruebas de confirmación, dobles ADO independientes y regresiones DOC122–124/carpetas. Actualizar documentos de la fase y evidencia por SHA. La interfaz y registro administrativo de fuentes nuevas siguen en sus fases; no modificar SII, perfiles de carpetas, menús ni proveedores para implementar esta confirmación.

Evidencia inspeccionada: `GestionDocumental-Docuarchi.net.vbproj`, pruebas DOC122–124, recursos del módulo y contratos DOC121.

## Contratos y secuencia

`SolicitudConfirmarExcel`: ConexionId, IdCarga, ArchivoHash, ConfiguracionHash, EsquemaHash, ConfirmacionReemplazo. El propietario/tabla/modo se obtienen del registro. `SolicitudConsultaCargaExcel` sigue conteniendo sólo ConexionId e IdCarga. `EstadoCargaExcel` incorpora FilasConfirmadas y ConfirmadoUtc; el resultado de consulta puede añadir estado estructural seguro para CREAR (no creada, propia compatible, ajena/incompatible o no acreditada). Ninguna respuesta expone archivo_ref, configuracion_doc, secretos o identificadores de conexión MySQL.

1. Validar transporte/CSRF, autoridad, identidad y propiedad. Si CONFIRMADA y contenido coincide, devolver el resultado persistido sin resolver límites, plantilla, archivo ni esquema actuales.
2. Para PREPARADA/FALLIDA: adquirir sesión ID; adquirir bloqueo de destino en otra conexión con la misma identidad física; releer estado. Si ahora CONFIRMADA, resolver replay. RECIBIDA no es confirmable.
3. Verificar snapshots v1, límites actuales, plantilla completa, archivo retenido y lectura completa; inspeccionar elegibilidad física, política, claves y presupuesto de parámetros antes de modificar datos. Todo acceso bajo transacción debe usar su DbTransaction, incluidos inspector y validación temporal.
4. CREAR: verificar ausencia/procedencia; realizar DDL independiente con comentario `docuarchi-excel/v1/<UUID>/<EsquemaHash>`; inspeccionar estructura propia y exigir ausencia de filas inexplicadas. No mantener transacción de filas o locks FOR UPDATE a través de CREATE; sus commits implícitos se reconocen. Revalidar después del DDL.
5. Abrir transacción de datos; bloquear ejecución, cabecera y campos; releer y validar snapshots/autoridad/política y esquema. Repetir la comprobación de claves contra el destino con la transacción actual. Ejecutar DML por modo y publicar CONFIRMADA/conteo/UTC mediante CAS en esa misma transacción.
6. Verificar integridad del archivo retenido y autoridad/política vigentes antes del commit. Ante rollback conocido registrar FALLIDA en tx corta sin sustituir una confirmación concurrente. Ante commit incierto conservar ID y pedir consulta; nunca replay automático.
7. Liberar temporales, modo y ambos locks por conexión. Si una limpieza no queda acreditada descartar esa conexión/pool específico; no afirmar resultado fallido después de un commit exitoso.

## Matriz de estados

| Estado observado | Confirmar misma identidad/contenido | Consulta |
| --- | --- | --- |
| RECIBIDA | Rechazar: requiere preparación completa. | Estado sin efectos. |
| PREPARADA | Revalidar y ejecutar con locks; si otra sesión terminó, resultado histórico. | Estado durable; actividad/estructura sólo si se acredita por SELECT. |
| FALLIDA | Reintento explícito íntegro con snapshots originales y validación vigente; cambios de intención requieren carga nueva. | Fallo saneado y estructura propia superviviente, si procede. |
| CONFIRMADA | Resultado histórico, cero escrituras y cero relecturas de archivo. | Resultado histórico y conteo/UTC. |
| Ausencia, actividad o commit incierto | No inferir permiso de repetir; conservar ID y consultar. | Respuesta no concluyente sin reparar ni filtrar cargas ajenas. |

No se persiste PAUSADA ni una posición de fila. El modo ya confirmado es histórico aunque otro proceso cambie después los datos. La plantilla y la política RA se revalidan en los límites de la operación; RA y permisos externos no participan de una transacción distribuida con DA.

Los métodos ASMX existentes habilitan sesión ASP.NET y pueden serializar peticiones
de una misma sesión. La consulta no promete progreso en vivo desde esa misma
sesión mientras Confirmar siga ejecutándose. La concurrencia de aceptación usa
sesiones independientes; después de terminar o cortar la petición, la recuperación
se hace por el ID durable. No se incorpora un trabajo en segundo plano, un handler
paralelo ni una excepción de autenticación para ocultar esa limitación. Un timeout
HTTP tampoco acredita por sí solo que la transacción haya revertido.

## Risks / Trade-offs

- D-01: Autorización vigente incluso para resultados históricos; las respuestas no filtran datos de cargas ajenas.
- D-02: La edición histórica sin incremento de revisión se detecta por huella completa; lectura previa no acredita validez futura.
- D-03: El almacén de ejecución, las plantillas y el destino deben acreditar InnoDB y la misma DA transaccional.
- D-04: Los bloqueos cooperativos no excluyen escritores externos ni mantenimiento DDL; el índice permanente sigue siendo obligatorio.
- D-05: Agrupación limita comandos; no divide el commit ni transforma números grandes en Decimal CLR.
- D-06: GET_LOCK sólo serializa participantes del módulo; UNIQUE protege frente a otro escritor.
- D-07: La política CONEXION no se infiere del prefijo; COLUMNA conserva las otras empresas.
- D-08: CREATE TABLE tiene commit implícito; su estructura puede sobrevivir a un fallo de filas y debe informarse sin prometer rollback DDL.
- D-09: Un estado sin confirmar observado durante actividad no demuestra rollback; conservar registros y archivos para recuperación.
- D-10: El resultado describe la ejecución de aquel momento, no promete que la tabla siga conteniendo hoy las mismas filas.
- D-11: No se ajustan índices, triggers, motores o relaciones de tablas históricas automáticamente.
- D-12: La continuidad es por resultado durable o reintento íntegro conocido; no por posición parcial ni cancelación garantizada al cerrar la pestaña.
- D-13: Entregar un archivo no acredita aplicación; permisos de pruebas anteriores no autorizan nuevas instalaciones o modificaciones.
- D-14: Las ventanas DOC124 cerradas y la autorización de preparar no autorizan importar o reemplazar los 625 contratos existentes.
- D-15: Las fuentes existentes pueden reflejar los datos autorizados nuevos; compatibilidad no significa congelar su contenido histórico.
- Dos conexiones por confirmación activa consumen dos recursos del mismo pool. La espera cooperativa es cero y los comandos siguen acotados; no se cambia globalmente el pool o los timeouts para ocultar una prueba lenta.
- UNIQUE garantiza identidad frente a otros escritores. No garantiza que un escritor externo sin el lock cooperativo deje de insertar claves distintas durante un reemplazo; el alcance operativo debe controlar esos escritores si exige exclusividad total.
- Los locks del módulo y la revalidación no bloquean DDL administrativo ajeno ni convierten MySQL5.1 en un servidor con capacidades posteriores. Se exige ausencia de mantenimiento estructural concurrente; no se introducen LOCK TABLES ni otras sentencias que comprometan la unidad DML.
- Las E2E anteriores ya están cerradas y sus cargas/datos se conservan. La nueva matriz usa destinos descartables previamente autorizados, nunca el borrado de CONTRATOS por inferencia.

## Migration Plan

1. Entregar SQL18 SELECT y guía; contrastar estructuras/índices/motores y efectos relacionados con confirmación de ejecución manual. No reaplicar SQL03/15/16/17. La cuenta/privilegios se verifican en la ventana autorizada; cualquier adecuación se entrega en otro SQL numerado.
2. Implementar y compilar contra net461/MySql.Data vigente; ampliar pruebas nativas y runner existente. Instalar configuración de fixture sólo mediante archivos revisados y confirmación del usuario.
3. Autorizar por separado ambiente, cuentas, destinos descartables y efectos de la E2E escritora. Registrar cada caso, respuesta y controles SELECT; conservar los efectos y resultados sin limpieza automática.
4. Reversión de código: volver a la versión previa, detener nuevas confirmaciones y conservar registros/archivos/tablas propias. No borrar estructuras supervivientes, datos confirmados, snapshots o incidencias como parte de la reversión. Cualquier corrección de datos tiene archivo/alcance propios.

## Fuentes técnicas contrastadas

- [MySQL: GET_LOCK anterior a 5.7.5 admite un solo lock por conexión](https://dev.mysql.com/doc/relnotes/mysql/5.7/en/news-5-7-5.html).
- [MySQL5.1: DDL y commits implícitos](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/sql-syntax.html#implicit-commit).
- [MySQL5.1: visibilidad de TRIGGERS y privilegio TRIGGER](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/information-schema.html#triggers-table).

La revisión técnica de Codex concreta el prompt vigente; no representa aprobación humana simulada ni nuevas autorizaciones de SQL, despliegue o E2E real.
