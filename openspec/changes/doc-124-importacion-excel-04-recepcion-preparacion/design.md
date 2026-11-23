<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14 -->
## Context

DOC124 implementa fase04 después de autorización/plantillas DOC122 y lector DOC123. Base integrada87d155b7, rama feature/DOC-124; Web Forms/VB, net461, MySQL5.1.37 y conector actual. Este refinamiento no cambia producto, SQL o despliegue.

Los generadores ya produjeron artefactos iniciales; se sustituyen sus marcadores con decisiones contrastadas. La referencia a cuenta dedicada del prompt original queda superada por la decisión posterior del usuario en DOC122: usar conexión/cuenta DA del módulo. No introducir otra versión de MySql.Data para probar.

## Goals / Non-Goals

Recibir un XLS/XLSX privado con identidad estable, validar contenido completo/metadatos/unicidad y persistir RECIBIDA/PREPARADA. Preparar no altera datos ni esquema permanente del destino. Entregar vista previa/diagnósticos para confirmación futura.

Confirmar, crear tabla de negocio, INSERT/DELETE destino, reemplazar, activar fuentes/proveedores, UI y ejecución por bloques con pausa/reanudación quedan fuera. No reutilizar autorizaciones E2E cerradas ni modificar EGRESOS/CONTRATOS. Las correcciones de instalación siguen siendo SQL manual.

## Inventario previo y división de responsabilidades

| Ruta exacta | Estado / responsabilidad |
|---|---|
| Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb | Existente; añadir ejecución, política, snapshot y resultado de preparación sin alterar modelos/huella previos. |
| DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb | Existente; solicitudes de inicio/recepción/preparación/consulta, respuestas tipadas sin filas del navegador. |
| Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb | Existente, solo JSON; añadir vía multipart con mismo propósito CSRF. |
| Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb | Existente; exponer fábrica autorizada para repositorio de cargas sin devolver cadena al cliente. |
| Infrastructure/ProduccionDocumental/ImportacionExcel/ReferenciaRecepcionExcel.vb | Nuevo; referencia previa protegida con MachineKey, propósito distinto de streaming/CSRF. |
| Infrastructure/ProduccionDocumental/ImportacionExcel/AlmacenArchivoExcel.vb | Nuevo; almacenamiento Excel privado, archivo completo/retención y copia/hash acotados. |
| Infrastructure/ProduccionDocumental/ImportacionExcel/PoliticaDestinoCargaExcelAdapter.vb | Nuevo; consulta de aislamiento desde recursos RA existentes con autoridad propia de cargas. |
| Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlCargaExcelRepository.vb | Nuevo; ejecuciones propias y transiciones transaccionales. Separar metadatos/temporales en archivos parciales si mejora revisión. |
| Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPreparacionExcelValidator.vb | Nuevo; metadatos/índices, comparación temporal y ciclo de conexión. No escribe datos destino. |
| Services/ProduccionDocumental/ImportacionExcel/ServicioRecepcionExcel.vb | Nuevo; coordinar referencia, archivo y registro RECIBIDA, sin copiar coordinador documental completo. |
| Services/ProduccionDocumental/ImportacionExcel/ServicioPreparacionExcel.vb | Nuevo; lectura DOC123, validación, snapshot y publicación PREPARADA. |
| Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb | Existente; reutilizar sin duplicar conversiones. |
| Infrastructure/ProduccionDocumental/ImportacionExcel/LectorArchivoExcel.vb | Existente; añadir inspección de formato/hojas para recepción sin exigir datos válidos; preservar contrato DOC123. |
| webservice/WebServiceImportacionExcel.asmx; webservice/WebServiceImportacionExcel.asmx.vb | Existentes; añadir IniciarRecepcionExcel, Preparar, ConsultarCargaExcel y composición. |
| generic_control/ImportacionExcelUpload.ashx; generic_control/ImportacionExcelUpload.ashx.vb | Nuevos; receptor multipart/sesión, sobre tipado y no-cache. |
| GestionDocumental-Docuarchi.net.vbproj | Registrar Compile/Content de cada archivo nuevo; conservar framework y referencias vigentes. |
| generic_control/ImportacionCarpetasUpload.ashx.vb; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AlmacenStagingImportacion.vb | Antecedentes reutilizados por patrón: guardas, retención, fallos. No cambiar PDF, perfiles o estados documentales para cargar Excel. |
| Infrastructure/Documentos/Streaming/DocumentoStreamingReferencia.vb | Patrón MachineKey ya presente; no transportar rutas del cliente ni reutilizar su propósito de documento. |
| Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlFuenteImportacionRepository.vb | Antecedente de consulta RA/política CONEXION-COLUMNA; no fingir permisos administrativos para invocarlo. |
| tests/ImportacionExcelBehaviorTests.cs; tests/ImportacionExcelLecturaTests.cs; tests/doc122-importacion-excel-behavior.test.cjs | Existentes; mismo ejecutable/harness. Puede añadirse archivo C# de preparación al mismo ejecutable por legibilidad. |
| tools/e2e/tests/importacion-excel.spec.cjs; tools/e2e/scripts/run-doc110-profile-session.cjs | Existentes; extensión DOC124, autenticación y consola privadas existentes. |
| Doc/Actualizacion/Importacion/Sql/11-preflight-doc124.sql | Nuevo durante implementación: exclusivamente SELECT de instalación, índices, identidad y capacidades observables. |
| Doc/Actualizacion/Importacion/Diseno/04-recepcion-preparacion-doc124.md; Doc/Actualizacion/Importacion/Validacion/ | Contrato vigente y evidencia, con diferencias entre local/MySQL/E2E. |

La división evita ampliar el receptor de PDF con estado Excel o construir otra plataforma de pruebas. Los nuevos nombres pueden dividirse conservando responsabilidades y registro previo del ajuste. No crear ServicioConfirmacionExcel/JS/CSS en esta fase.

## Decisions

### D-01 / RQ-01 — Autoridad y transporte propios

Verificar en cada operación usuario activo, empresa, permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1, DA/módulo vigentes y CSRF propio. Extender guarda multipart sin debilitar POST JSON ni exigir permisos de perfiles/fuentes.

Evidencia inspeccionada: ImportacionExcelContextAdapter.Resolver; ImportacionExcelRequestGuard.Validar/ValidarTransporte; ImportacionCarpetasRequestGuard.LeerMultipart.

### D-02 / RQ-02 — Archivo privado e inmutable

Contar bytes y SHA256 durante copia acotada a archivo .part exclusivo, validar XLS/XLSX real y publicar completo sin sobrescribir. Referencias/rutas generadas por servidor bajo almacenamiento protegido; RECIBIDA exige archivo completo verificable.

Evidencia inspeccionada: AlmacenStagingImportacion.Recibir/ConservarVerificado; LectorArchivoExcel; Sql/03-ejecucion-da-doc121.sql.

### D-03 / RQ-03 — ID previo y recuperación del acuse

Emitir ID de carga UUID y referencia protegida antes del multipart, sin insertar metadatos ficticios. Repetición explícita del mismo ID/contenido reutiliza recepción propia; conflicto de contenido o escritor activo no se reintenta automáticamente.

Evidencia inspeccionada: DocumentoStreamingReferencia.Emitir/TryResolver (MachineKey); ServicioRecepcionImportacion.Recibir; PK imp01_carga_ejecucion.id_carga.

### D-04 / RQ-04 — Destino y aislamiento de servidor

Resolver DA por recursos del módulo sin otra conexión/conector. Para tabla existente exigir política empresarial explícita de servidor; CONEXION exclusiva o COLUMNA con empresa+clave. No inferir exclusividad por prefijo/nombre de columna ni autoridad desde Excel.

Evidencia inspeccionada: ImportacionExcelConexionAdapter.Resolver; MySqlFuenteImportacionRepository.Cabecera/CargarPoliticaEmpresa; PoliticaFuentesProveedor; MySqlProveedorDatosIndices.ConsultarDatos.

### D-05 / RQ-05 — Lectura completa y límites heredados

Usar ServicioLecturaExcel con límites positivos del servidor,10000 filas por defecto y20000 configuradas por solicitud del usuario2026-11-23, con10MB decimal, conservando coordenadas/tipos. Ninguna muestra o prefijo acredita validez; errores detienen publicación PREPARADA y mantienen RECIBIDA.

Evidencia inspeccionada: ServicioLecturaExcel.Leer; ResultadoLecturaExcel.Valido/LecturaCompleta; ImportacionExcelConexionAdapter.Limite.

### D-06 / RQ-06 — Índice único y esquema físico

En destinos existentes verificar InnoDB, tipos/longitudes/charset/nulabilidad actuales y UNIQUE de clave completa, o empresa+clave completa. Rechazar índice de prefijo/no único/con columnas adicionales; CREAR solo proyecta estructura y comprueba nombre disponible.

Evidencia inspeccionada: MySqlPlantillaExcelRepository.LeerDestino; ServicioPlantillasExcel.ValidarDestino/TipoCreable; DOC121 D-05 y snapshot de esquema.

### D-07 / RQ-07 — Duplicados con semántica MySQL

Comparar claves mediante temporales de la misma conexión con tipo, charset/collation y unsigned/precisión exactos. Distinguir duplicados internos y claves existentes al AGREGAR; REEMPLAZAR admite coincidencias del ámbito sustituido, CREAR exige unicidad interna.

Evidencia inspeccionada: MySqlProveedorDatosIndices.ConsultarDatos; PerfilImportacionCatalogAdapter.ValorCompatible; manual MySQL5.1 CREATE TEMPORARY TABLE/GROUP BY.

### D-08 / RQ-08 — Snapshot efectivo y detección de cambios

Persistir documento efectivo canónico y hashes de archivo/configuración/esquema, revisión, hoja, modo, aislamiento y filas previstas. Mantener huella v1 de plantilla dentro del documento; relectura final de autoridad/plantilla/esquema invalida cambios observados.

Evidencia inspeccionada: ServicioPlantillasExcel.Huella; DOC121 snapshot v1; imp01_carga_ejecucion.configuracion_doc/configuracion_hash/esquema_hash.

### D-09 / RQ-09 — Estados y concurrencia por carga

Serializar recepción/preparación del mismo ID y publicar estado con transacción InnoDB y relectura propia; sin transacción abierta durante copia/parseo. RECIBIDA→PREPARADA solo tras validar. Fallo conocido no sobrescribe ganador y acuse incierto exige consultar.

Evidencia inspeccionada: ServicioRecepcionImportacion.Recibir; imp01_carga_ejecucion PK/estado; patrones FOR UPDATE de MySqlPlantillaExcelRepository.Guardar.

### D-10 / RQ-10 — Vista previa y consulta acotadas

Preparar devuelve hasta25 filas/25 incidencias con totales observados y truncamiento. ConsultarCargaExcel es lectura propia, nunca prepara/confirma. No devolver rutas, SQL, secretos o recursos ajenos; ninguna tabla destino se modifica.

Evidencia inspeccionada: DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb; WebServiceImportacionExcel.Ejecutar; DOC121 contrato request/response.

### D-11 / RQ-11 — Temporales, pool y recursos recuperables

Liberar temporales y lock y restaurar sql_mode de sesión en Finally; si limpieza no se acredita, descartar conexión del pool. No modificar modo global, cuenta o versión MySql.Data; un lock vencido por tiempo no autoriza tomar un escritor.

Evidencia inspeccionada: ImportacionExcelConexionAdapter.ConexionVigente; AlmacenStagingImportacion; manual MySQL5.1 temporales y locks de conexión.

### D-12 / RQ-12 — SQL manual y compatibilidad

Reutilizar las22 columnas de ejecución y scripts históricos sin reaplicarlos. Entregar preflight SELECT nuevo; diferencias permanentes requieren otro SQL manual y confirmación antes de SELECT posterior. No modificar EGRESOS/CONTRATOS ni consumidores SII/carpetas.

Evidencia inspeccionada: Sql/03-ejecucion-da-doc121.sql; Sql/README.md; AGENTS.md; DOC123 cerrado87d155b7.

### D-13 / RQ-13 — Pruebas locales reutilizadas

Extender el ejecutable/harness DOC122/DOC110 y fixtures DOC123 con fallos entre filesystem/DB, rollback propio, modos, hashes, límites y concurrencia. Build aislado y evidencia distinguen dobles sintéticos de MySQL real.

Evidencia inspeccionada: tests/ImportacionExcelBehaviorTests.cs; tests/ImportacionExcelLecturaTests.cs; tests/doc122-importacion-excel-behavior.test.cjs.

### D-14 / RQ-14 — E2E real de recepción y preparación

Extender tools/e2e existente para ASHX/ASMX reales, sin infraestructura paralela ni heredar autorización DOC122. Reutilizar las tablas consumidas y los controles existentes, conservando los destinos mediante SELECT y huellas completas. Acreditar con autorización específica ámbito/cuentas/archivos y ejecuciones de prueba: acceso, tres modos, dos sesiones y destino sin mutación; no exigir tablas nuevas o declarar descartables las existentes.

Evidencia inspeccionada: tools/e2e/tests/importacion-excel.spec.cjs; run-doc110-profile-session.cjs; AGENTS.md; tools/e2e/AGENT-RUNBOOK.md.


## Recepción coherente y recuperación — D-01 / D-02 / D-03

IniciarRecepcionExcel recibe conexión/plantilla/revisión/huella esperadas, verifica autoridad y devuelve UUID servidor más referencia protegida de recepción. No inserta ejecución ni crea archivo. El cliente conoce el ID antes de enviar bytes y puede consultar aunque pierda el acuse. La referencia se protege con MachineKey del entorno existente, propósito propio/versionado, vinculada a usuario/empresa/módulo/conexión/plantilla/ID y vencimiento de30 minutos; no concede autoridad ni sustituye CSRF. No requiere clave nueva en Web.config. Un cambio de clave del entorno puede invalidarla; no se resuelve desactivando protección. La consulta de una fila propia ya registrada depende de autoridad vigente y no del token inicial vencido.

ASHX admite POST multipart, exactamente un archivo y campos idCarga, referenciaRecepcion, conexionId, plantillaId. Validar autoridad/token/origen/Content-Length positivo y límite de cuerpo antes de Form/Files; boundary válido, campos únicos y sin otros campos. La cabecera X-Importacion-Excel-CSRF conserva su propósito. JSON mantiene262144bytes. El cuerpo multipart admite máximo de archivo efectivo más262144bytes de metadatos; esa holgura no aumenta el límite de bytes del archivo. Contar contenido real y rechazar archivo vacío, tamaño inconsistente o exceso.

Inspección local solo de límites HTTP, sin revelar configuración: Web.config actual maxRequestLength102400000KB y maxAllowedContentLength300000000bytes, mayores que10MB. No elevar esos límites ni executionTimeout. La aplicación impone su límite propio; Request.Files puede haber sido bufferizado por ASP.NET. Se promete copia acotada a staging, no streaming de red sin buffer ni reanudación por bytes.

Almacenamiento bajo App_Data/ImportacionExcel separado de carpetas, protegido contra acceso HTTP y fuera de Git. Subrutas por empresa/módulo/ID y referencia servidor, nombre original solo descriptivo (máximo255 compatible con utf8 físico; rechazar caracteres no representables). Canonicalizar raíz y negar traversal/reparse en componentes, con ACL de escritura apropiada; creación exclusiva y publicación sin overwrite. Comprobar formato real/hojas con lector común antes de registrar. Un libro de formato válido con incidencias de datos puede recibirse: su validación de filas pertenece a Preparar.

Nunca insertar bytes=0/hash de ceros para simular recepción. Tras publicar el archivo completo, reautorizar, INSERT transaccional RECIBIDA con bytes/hash reales y32 bytes de hash binario; campos de preparación nulos, filas_confirmadas0. Conexión_ref canónica DA:<moduloId>:<conexionHistoricaId>, sin credenciales y menor de64. Ambito/usuario provienen de RA; tabla_destino de plantilla. Esa referencia se distingue del literal DA que usa el catálogo de fuentes.

Mantener archivo completo si INSERT/commit es incierto; no borrar por timeout ni crear otra carga. Con ID propio, inspección de fila/archivo/actividad puede informar RECIBIDA, OPERACION_EN_CURSO, RESULTADO_INCIERTO o referencia no disponible. Consulta no repara. Un retry solo explícito sobre el mismo ID, autorización/referencia válida, ausencia acreditada de escritor y contenido íntegro idéntico; puede completar el registro de archivo propio o devolver la recepción anterior. Contenido distinto rechaza. Parciales de fallo conocido pueden eliminarse solo si son propios, no referenciados y sin escritor; la limpieza general no pertenece a esta fase.

## Estado, transacción e identidad — D-08 / D-09 / D-10

Estados persistidos de esta fase: RECIBIDA y PREPARADA. Inicio y copia son actividad transitoria, no estados falsos en tabla. El esquema03 tiene22 columnas suficientes; no se planifica otra migración permanente.

Recepción/preparación serializan el mismo ID mediante un lock de operación de conexión, nombre derivado de identidad física DA+ID, acotado a64caracteres, adquisición no bloqueante. No usar tiempo transcurrido o lease como permiso para tomar un escritor. Preparar no bloquea la tabla destino durante lectura. Se usa un único lock nombrado a la vez por conexión; no combinar locks de carga/destino en esa conexión de MySQL5.1. Fase05 definirá el bloqueo de confirmación por destino.

La copia/parseo no mantienen una transacción InnoDB abierta. Publicar RECIBIDA o PREPARADA utiliza transacción corta, fila propia FOR UPDATE y relectura de estado/archivo/contexto; condición de propietario/módulo/conexión obligatoria. Estado CONFIRMADA de fase posterior no se sobrescribe. Una segunda preparación ya equivalente puede devolver resultado vigente solo tras revalidar sus huellas; intención distinta requiere hash esperado y preparación explícita.

Fallo conocido de validación conserva RECIBIDA y codigo_incidencia; limpia en una transición propia los campos preparados que pudieran quedar obsoletos. Un fallo o commit incierto no puede degradar un ganador PREPARADA/CONFIRMADA; consulta obligatoria. No añadir historial o reporte de incidencias persistente inexistente:03 solo guarda código principal en RECIBIDA. Si se pierde una respuesta inválida, la consulta devuelve ese código; el detalle se recalcula con Preparar explícito, no como efecto de consultar.

## Aislamiento y esquema — D-04 / D-06

DA/empresa/módulo y conexión histórica se vuelven a resolver por los recursos de DOC122. No seleccionar credenciales de imp01_xmlconexion ni añadir cuenta/conexión protegida aparte. La cuenta compartida conserva sus facultades; esta fase no afirma mínimos privilegios físicos por prefijo.

Para un destino existente, resolver política explícita en catálogo RA de fuentes por DA/tabla/empresa y revisión. Leer esa política desde adaptador con autoridad propia de cargas, sin fabricar PuedeAdministrar/PuedeAdministrarFuentes ni exigir esos permisos al cargador. CONEXION acredita exclusividad configurada; COLUMNA exige columna empresarial de texto/entero, NOT NULL, mapeada y con cada valor igual a la empresa autorizada según comparación del destino. Excel propone un valor, nunca el ámbito. No rellenar o crear mapeos empresariales silenciosamente.

Fuente inexistente/inactiva/ambigua o política contradictoria para destino existente da POLITICA_DESTINO_NO_DEFINIDA/ESQUEMA_INCOMPATIBLE. No inferir exclusividad por ausencia de columna ni por imp_02_. La política debe prepararse por administración vigente cuando haga falta, sin conceder su permiso al usuario de carga. Fuente cargable y proveedor publicado conservan autorizaciones distintas.

CREAR admite nombre imp_02_ ausente y ámbito DA autorizado; proyecta intención de tabla propia del usuario/empresa, con columnas/charset/collation explícitos del servidor y tipos permitidos DOC121. Preparar no reserva el nombre ni crea/activa fuente; fase05 hará CREATE con procedencia identificable. No declara que la DA física sea exclusiva por poder crear un nombre nuevo.

Metadatos: information_schema.TABLES/COLUMNS/STATISTICS más identidad física del servidor/base. Usar capacidad física actual, no encoger BENEFICIARIO o cambiar FECHA VARCHAR. Índice equivalente: NON_UNIQUE0, SUB_PART nulo en cada componente, columnas exactas y requeridas (clave, o conjunto empresa+clave). Conservar orden físico en hash; orden de los dos componentes puede variar si su conjunto es equivalente. Rechazar índices con otras columnas que permitan repetir la identidad, índices de prefijo/no únicos y PK técnica usada como sustituto sin validación. CREAR comprueba capacidad del índice completo de la estructura proyectada sin ejecutar DDL.

## Lectura, temporales y semántica de claves — D-05 / D-07 / D-11

Abrir original con retención de lectura exclusiva frente a escritura/borrado, verificar bytes/hash antes y después y usar ServicioLecturaExcel. No tomar filas del navegador ni una lista guardada en sesión. El resultado DOC123 válido es condición para comparar claves. Si el lector devuelve incidencias, su Filas está vacía: informar sus totales observados y etapa de duplicados no ejecutada, no inventar cero duplicados o una validación completa.

Para resultado válido, temporales solo en la conexión DA autorizada, con fila física y componentes de identidad definidos exactamente con tipo/unsigned/precisión/escala/charset/collation del destino. Valores parametrizados; DECIMAL65 se pasa como cadena canónica para conversión estricta del servidor, sin Decimal CLR. Identificadores salen de metadatos validados y se escapan; no interpolar cliente, nombres de archivo o SQL libre.

Insertar en grupos acotados por parámetros/bytes y @@max_allowed_packet observado, sin aumentar variables globales. Validar modo SQL estricto de sesión y rechazar conversión/advertencia que afecte datos. Agrupar identidad con comparación MySQL; conservar números físicos de las filas implicadas. No usar HashSet ordinal/JavaScript para acreditar igualdad real.

MySQL5.1 no permite referir dos veces la misma temporal en ciertas consultas. Se permiten dos temporales propias: filas y grupos duplicados, con definiciones explícitas compatibles. Agrupar desde la primera hacia la segunda y hacer join entre ellas sin autojoin de la misma temporal. Para claves existentes, join de temporal de filas contra destino filtrado por empresa. AGREGAR rechaza coincidencia; REEMPLAZAR acepta claves que serán sustituidas dentro del ámbito, pero rechaza duplicados internos; CREAR rechaza duplicados internos/nombre ocupado. Se devuelven códigos y filas, no valores de otra empresa.

Preparar solo lee la tabla destino. Sus escrituras funcionales son archivo privado, fila de ejecución y temporales propias. CREATE TEMPORARY/INSERT temporal/DROP TEMPORARY no son instalación de esquema de negocio, pero sí efectos de aplicación que requieren alcance real autorizado al probarlos. No crear índices con CREATE INDEX fuera de la definición temporal ni mezclar DDL permanente con la publicación transaccional.

En Finally: rollback propio si procede, DROP TEMPORARY explícito de nombres propios, restaurar sql_mode original y RELEASE_LOCK antes de devolver conexión al pool. Rollback no garantiza que desaparezca una temporal. Si no se acredita limpieza/restauración/liberación, descartar conexión/pool correspondiente mediante capacidad del conector vigente antes de Dispose; no ClearAllPools o cambio de conector. Las primitivas de descarte deben verificarse contra la DLL local durante implementación.

## Snapshot y vista previa — D-08 / D-10

Mantener ServicioPlantillasExcel.Huella v1 sin cambiar su formato. Dentro de configuracion_doc persistir documento efectivo versionado con snapshot físico de plantilla y su hash, intención hoja/modo, clave/aislamiento/revisión de política, identidad física DA+módulo/conexión y límites efectivos. configuracion_hash es hash del documento efectivo; no se confunde con la huella de plantilla. Filas previstas provienen de lectura completa; archivo_hash permanece inmutable.

Esquema_hash canónico de motor, columnas usadas y requeridas con atributos físicos, índices y componentes/SEQ_IN_INDEX/NON_UNIQUE/SUB_PART, identidad proyectada si CREAR. Orden ordinal, NULL explícitos, números y escape ASCII/UTF8 sin BOM conforme DOC121; añadir vector sintético independiente. Comparar plantilla/hash/esquema/política antes y al final de preparación. Diferencia observada invalida. No afirmar que PREPARADA congela escritores externos, DDL o datos de proveedores; confirmar en fase05 revalidará.

Respuesta: ID/estado, archivo descriptivo/bytes/hash, plantilla/revisión y hashes efectivos, hoja/modo/destino/clave, filas observadas/previstas, hasta25 filas de muestra y25 incidencias con totales/códigos/coordenadas/truncamiento. Si una etapa no pudo ejecutarse, señalar alcance incompleto. La muestra1000 del lector no altera sus totales; se reduce a25 para transporte. No guardar filas/archivo en sesión ni exponer ruta, excepción SQL, credenciales, token en evidencia o datos masivos.

Preparar primera vez recibe ID, conexión, revisión/huella de plantilla esperadas, hoja y modo. Para cambiar una preparación vigente exige hash efectivo esperado; ConsultarCargaExcel permite recuperarlo como lectura propia. Cada consulta reautoriza, filtra propietario y nunca prepara, confirma, repara o reintenta. ASP.NET ASMX con sesión puede serializar peticiones de una misma sesión; no presentarlas como prueba de escritores concurrentes ni prometer progreso simultáneo en la UI.

## SQL, pruebas y límites del refinamiento — D-12 / D-13 / D-14

Entregar11-preflight-doc124.sql de SELECT y guía que distinga RA/DA, tabla de22 columnas, columnas/índices destino y capacidades observables. Los archivos01–10 históricos no se reescriben. Cualquier diferencia permanente se entrega en otro archivo manual, con PENDIENTE_EJECUCION_MANUAL hasta confirmación; no instalar por startup/test ni simular schema compatible. La confirmación histórica de scripts no acredita una DA nueva de sesión. Sin ejecutar consultas reales en este refinamiento.

Pruebas locales reutilizan ejecutable y compilador DOC122/DOC110, fixtures/lector DOC123; dobles de filesystem y ADO.NET para fallos entre .part/publicación/INSERT/commit, replay, propiedad, límites, CAS, hashes y limpieza. Build con salidas aisladas, sin sustituir DLL de IIS. Verificar fronteras, política COLUMNA/exclusiva, UNIQUE incompleto, mayúsculas/acentos/espacios/números y tres modos; la igualdad de collation definitiva necesita MySQL real.

E2E DOC124 sí aplica al recorrido de servicios ASMX/ASHX, aunque la pantalla se implementará después. Extender importacion-excel.spec.cjs y runner/consola/perfil existentes con modo/alcance DOC124 separado, p.ej. --excel-prepare. Mover skip DOC122 a su propia suite; no autorizar DOC124 por una variable DOC122/DOC110. Reutilizar autenticación y safe request, con secretos efímeros y sin grabaciones/cookies/token en reportes.

Preparar código/E2E y alcance revisable antes de solicitar ejecución. La autorización real debe identificar ambiente, cuentas, plantillas/archivos y efectos permitidos (emisión, archivos, ejecuciones, temporales y guardado de plantilla solo si se incluye). Instalación/índices/fixtures permanentes se ejecutan manualmente por usuario; tests no siembran/limpian/reparan bases. AGREGAR/REEMPLAZAR reutilizan destinos existentes ya consumidos y autorizados, CREAR solo una intención de nombre ausente; preparar en ambos conserva filas/esquema mediante SELECT de control. Instrucción del usuario2026-11-23: no exigir tablas nuevas o descartables; se permite EGRESOS/CRÉDITOS y los demás destinos explícitos en lectura sin modificarlos. No elegir una fuente por defecto ni presentar un rechazo de esquema como PREPARADA válida.

Para concurrencia usar dos sesiones autenticadas independientes y confirmar contención MySQL/ganador, no solo serialización ASP.NET. Cargas/pruebas anteriores cerradas no se reabren. Si falta autorización o instalación necesaria, registrar el bloqueo de E2E real y continuar implementación local independiente; no archivar marcando la tarea real como simulada o no aplicable.

## Riesgos y rollback

Implementación local2026-11-23: se añaden también ComposicionCargaExcel.vb, ReglasCargaExcel.vb y SqlCargaExcel.vb para composición autorizada, reglas compartidas y comandos parametrizados. Se conserva el reparto de responsabilidades y se registran Compile/Content. La lectura de políticas activas de la misma tabla niega una exclusividad CONEXION contradictoria entre ámbitos; no devuelve políticas ajenas. Preparar acredita propiedad antes de adquirir lock para no revelar actividad de IDs ajenos. SQL11 queda entregado, sin ejecución real. Evidencia y contrato operativo en Doc/Actualizacion/Importacion/Diseno/04-recepcion-preparacion-doc124.md y Validacion/doc124-validacion-local-20261123.md. E2E real/entrega final siguen pendientes.

No hay transacción distribuida filesystem/MySQL: publicación incierta conserva evidencia y usa ID previo. No se promete pausa por fila/byte, reanudación automática o reemplazo en esta fase. Capacidad de10MB/20000 configuradas (10000 por defecto del lector) controla trabajo admitido, no acredita tiempo/memoria completos; medir fase07 con lector nuevo. El usuario autorizó elevar el máximo a20000 y se preparó una copia privada compatible de su archivo, sin incorporarlo a fixtures de Git. La E2E real del2026-11-23 acreditó12759 filas en CREAR hasta PREPARADA; no completa la matriz de tres modos.

Rollback futuro del despliegue no elimina archivos/filas existentes; preservar RECIBIDA/PREPARADA consultables para versión compatible y no borrar efectos inciertos. Fallos se resuelven con recuperación explícita de la misma identidad, sin tomar escritores por timeout. Cualquier retirada de referencias/Compile mantiene DOC122/DOC123. Refinamiento completo no acredita implementación, aplicación SQL ni E2E.

Referencias primarias revisadas: [temporales y transacciones MySQL5.1](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/sql-syntax.html#create-table) y [funciones de bloqueo MySQL5.1](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/functions.html#function_get-lock). Se usan como soporte de límites de conexión; las decisiones de producto anteriores se contrastan con el código del repositorio.
