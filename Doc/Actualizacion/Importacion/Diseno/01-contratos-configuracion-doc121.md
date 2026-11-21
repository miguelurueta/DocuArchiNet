# DOC-121 — esquema, contratos y configuración de importación Excel

Fecha: 2026-11-20. Cambio `doc-121-importacion-excel-01-esquema-contratos-configuracion`, fase 01 de 7. Documento contractual; no afirma endpoints disponibles ni instalación. Fuente: [exploración aprobada](../Exploracion/carga-excel-csv-tablas-imp-02.md) y [prompt 01](../Exploracion/01-esquema-contratos-y-configuracion.md). El usuario eligió conservar el flujo actual de los prompts y ejecutar manualmente los SQL.

Alcance vigente aclarado por el usuario: EGRESOS es solo de pruebas. Su adecuación o depuración no es requisito para entregar esta fase; se conserva intacta. Una tabla elegida como destino de carga debe cumplir la clave requerida/única cuando vaya a utilizarse. El aprovisionamiento y prueba de cuenta/host/DA se concretan en las fases que implementan la conexión, a partir del contrato y la plantilla 05 entregados aquí.

## Inventario y compatibilidad — D-01/RQ-01

| Elemento | Evidencia inspeccionada | Regla de compatibilidad |
|---|---|---|
| imp01_xmlconexion | Exploración del 11/11: 9 columnas, PK ID_XMLCONEXION, nombre único. | No cambiar esquema ni exponer sus credenciales históricas. Su ID no autoriza por sí mismo escribir en una DA. |
| imp01_plantillaimp | Class_imp01_plantillaimp.Solicita_identificacion_plantilla_externa_x_nombe consulta ID_PLANTILLAIMP por NOMBRE_PLANTILLA en DA. | Preservar 10 columnas históricas, PK/FK y nombre; agregar CAMPO_UNICO/REVISION_CARGA al final. |
| imp01_campos_plantilla | Class_imp01_campos_plantilla consulta CAMPO_DESTINO/CAMPO_FUENTE por IMP01_PLANTILLAIMP_ID_PLANTILLAIMP. | No cambiar nombres, ortografía o mapeos existentes durante instalación. |
| Plantilla externa/SII | WebServicePlantillaExterna.Service_solicita_estructura_campos_dynamic_polantilla_externa_rue_SII y Class_Integracion_SII. | Conservar consultas actuales y formatos de sus respuestas. |
| Conexiones | Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.Resolve. | Autoridad RA y destino DA por sesión/configuración; nunca valores libres del navegador. |
| Permisos | PerfilImportacionContextAdapter.Resolver consulta pertenencia empresarial y permiso de perfiles. | Reutilizar patrón con permiso propio; el cargador no debe exigir administrar perfiles. |
| Índices | MySqlProveedorDatosIndices.ConsultarDatos y PerfilImportacionCatalogAdapter.ValorCompatible. | Columna/tipo reales, clave parametrizada, filtro empresarial, LIMIT 2; validar longitud/rango/charset. |
| Fuentes | RestriccionTablasFuente.Permite y catálogo RA de fuentes/proveedores. | Prefijo imp_02 y autorización de lectura vigentes; el permiso de carga es independiente. |
| Programa anterior | preproyectoimport/ClassCargarDatos.Crea_tabla_Destino; ClassPlantillas.Crear_Plantilla. | NOMBRE_PLANTILLA ya puede contener imp_02_; no agregarlo dos veces. IMP_02_ID_CLAVE es una PK técnica opcional. |

Cabecera histórica, en su orden observado: ID_PLANTILLAIMP, IMP01_XMLCONEXION_ID_XMLCONEXION, NOMBRE_PLANTILLA, CONEXION_FUENTE, LIBRO_EXCEL, RUTA_ARCHIVO_FUENTE, ELIMINA_TABLA, ELIMINA_INSERTA_RECORD, INSERTA_RECORD, ESTADO_PLANTILLA.

Mapeos históricos: ID_CAMPOS_PLANTILLA, IMP01_PLANTILLAIMP_ID_PLANTILLAIMP, CAMPO_FUENTE, CAMPO_DESTINO, TIPO_CAMPO, REMPLAZA_COMA, REMPLAZA_PUNTO, OPCION_SPLITER, CARACTER_SPLITER, NUMERO_MATRIZ_EXPLITER. Las transformaciones y su numeración se conservan como metadatos; su lectura exacta se acredita en fase 03.

Antecedente físico del 11/11: MySQL 5.1.37, tres catálogos y dos tablas ejemplo InnoDB/latin1_swedish_ci, sin triggers en ese conjunto. No es una lectura actual. El usuario informó una ampliación posterior de BENEFICIARIO y del perfil; ningún SQL de esta entrega restablece su longitud anterior. En el corte contractual del 20/11 no se consultó el servidor. El [corte de verificación del 21/11](../Validacion/doc121-verificacion-instalacion-20261121.md) acredita por separado la estructura base instalada en una base física local y conserva las limitaciones de correspondencia RA/DA.

## Paquete de instalación — D-02/RQ-02

Ver [guía SQL](../Sql/README.md). DDL, índices y concesiones se entregan para el usuario/DBA; agente, tests y arranque no los aplican. Después de confirmación se revisa exclusivamente SELECT. Estado inicial PENDIENTE_EJECUCION_MANUAL; un archivo generado no acredita ejecución. RA/DA se seleccionan en la conexión administrativa, no se hardcodea un nombre físico en el paquete.

## Esquema de ejecución — D-03/RQ-03

Una sola tabla nueva: imp01_carga_ejecucion en la DA transaccional del destino. El [CREATE](../Sql/03-ejecucion-da-doc121.sql) es el diccionario físico normativo, con 22 columnas. Sin FK a catálogos legacy o bases distintas; id_plantilla es referencia lógica validada por servidor. Los tres índices secundarios soportan historial por propietario, plantilla y destino.

| Estado persistido | Invariantes |
|---|---|
| RECIBIDA | id_carga UUID canónico de 36 caracteres generado por servidor; ámbito/usuario/plantilla/destino autorizados, archivo privado completo, bytes y SHA-256. Modo, snapshot y hashes de preparación pueden ser NULL. |
| PREPARADA | Modo, revisión, configuración canónica y sus hashes no NULL; archivo/esquema coherentes, filas previstas válidas y cero incidencias bloqueantes. |
| CONFIRMADA | Datos y resultado confirmados juntos; filas_confirmadas/confirmado_utc coherentes con la ejecución. Snapshot histórico inmutable. |
| FALLIDA | Fallo conocido registrado sin ocultar un commit; no usar para una desconexión con resultado todavía desconocido. |

Una operación en curso no se interpreta como FALLIDA por antigüedad. La actividad o incertidumbre del cliente se informa con códigos de resultado, sin inventar estados de pausa. Fecha UTC por aplicación, hashes de 32 bytes en BINARY; transporte de hashes como 64 caracteres hexadecimales en minúsculas. Configuración JSON con escape ASCII en LONGTEXT. No guardar excepciones, SQL, conexiones o contenido completo de las filas en el diagnóstico.

Transiciones: RECIBIDA pasa a PREPARADA al validar; una validación rechazada conserva RECIBIDA. Antes de efectos se puede preparar otra vez el mismo archivo, reemplazando su preparación solo tras validar autoridad y contenido. PREPARADA pasa a CONFIRMADA con el commit, o a FALLIDA después de un rollback conocido. Una FALLIDA conocida puede volver a prepararse mediante acción explícita, revalidación completa y comprobación de cualquier estructura propia de CREATE. CONFIRMADA es inmutable y solo permite consultar su histórico. Publicar una preparación debe bloquear/releer el registro y rechazar una confirmación ganadora; no sobrescribirlo con el resultado tardío de otra solicitud. Nunca recuperar por esta ruta una operación activa o incierta.

## Configuración y autoridad — D-04/RQ-04

| Clave lógica de servidor | Valor inicial / regla |
|---|---|
| MaxFilasDatos | 10000, entero positivo; filas no vacías de la hoja elegida, sin encabezado. |
| MaxArchivoMB | 10, entero positivo; 1 MB=1000000 bytes, máximo inicial 10000000 bytes. |
| Vinculación de destino | Ámbito RA, DA autorizada y IDs de imp01_xmlconexion elegibles resueltos en servidor. |
| Cuenta DA del módulo — revisión DOC122 | Resolver sesión DA_ mediante ModuleSessionConnectionStringResolver, desde gestor_modulos. Sin cuenta adicional obligatoria; secreto fuera de Git, HTML, DTO y evidencias. |
| Almacenamiento privado | Directorio fuera de contenido público, con referencia relativa generada por servidor y ACL apropiada. |

Las claves son contrato; su lectura efectiva se implementa en fase 02, sin agregar entradas a Web.config en DOC-121. Una configuración presente inválida deniega capacidad; no convierte valores cero/negativos en ilimitados. Si falta un límite, se usa su valor inicial positivo. Mostrar los límites efectivos y coordinar IIS/ASP.NET con multipart, sin aumentos globales silenciosos.

Cada operación verifica sesión, usuario activo, empresa RA y ADMINISTRAR_CARGAS_IMPORTACION vigente. El permiso se instala con 0; no habilita cuentas. El usuario con solo ese permiso puede cargar; fuentes/proveedores conservan sus permisos propios. El cliente no envía usuario/empresa como autoridad ni host/base/credenciales. Toda referencia se resuelve otra vez contra el ámbito.

Revisión autorizada en DOC122 al pedir reutilizar las conexiones del repositorio: sustituye la cuenta separada inicial por la cuenta del módulo DA. No exige una conexión Excel en Web.config. La revisión del contrato está en el delta de DOC122 y no altera el archivo histórico de DOC121. Facultades requeridas para el módulo completo: SELECT, INSERT, UPDATE, DELETE, CREATE, ALTER, INDEX, CREATE TEMPORARY TABLES. La cuenta compartida puede tener otras facultades; este ajuste no afirma ausencia de DROP/GRANT OPTION ni cambia sus concesiones. La concesión sobre la base alcanza técnicamente todas sus tablas. El módulo valida prefijo, plantilla, destino e identidad empresarial; no se presenta como aislamiento físico por prefijo.

## Clave y tipos — D-05/RQ-05

CAMPO_UNICO corresponde a un CAMPO_DESTINO real y a la columna física exacta. NULL en plantilla histórica permite lectura legacy, pero bloquea preparación de escritura hasta adecuación. Clave requerida, NOT NULL y UNIQUE completo. Si hay columna empresarial autorizada, ambos componentes son requeridos y la identidad es empresa+clave. No utilizar UNIQUE de prefijo, índice no único ni PK técnica como sustitutos automáticos.

Validar nulos/vacíos/duplicados usando la comparación del destino MySQL; una comparación JavaScript/.NET no acredita la unicidad real. Preservar tipos, unsigned, precisión, escala, charset, collation, defaults, comentarios y longitudes actuales. Para tipos nuevos se contemplan VARCHAR(250), INT, DECIMAL(18,2), DATE; verificar capacidad de índice completa antes de CREATE. No reducir BENEFICIARIO ni ensanchar columnas sin otro SQL explícito.

Fuente nueva: después de confirmar datos, registrar y activar fuente y publicar proveedor por administración vigente. Las fuentes compartidas y exclusivas mantienen su política existente. Tabla cargada no equivale a fuente habilitada.

## Request/response — D-06/RQ-06

Operaciones previstas en WebServiceImportacionExcel.asmx; receptor ImportacionExcelUpload.ashx. No se crean esos endpoints en DOC-121. Para JSON, sobre `{solicitud: ...}` y respuesta siguiendo RespuestaPerfilImportacion: ok, codigo, mensaje saneado, operacionId, datos, incidencias. UUID de carga se define en DTO propio; IDs legacy se transportan como cadenas decimales positivas dentro de su rango físico, sin convertir las identidades RA de carpetas a UUID.

Protección de solicitud propia del cargador, vinculada a sesión/usuario/ámbito y validada en cada mutación. Cabecera prevista X-Importacion-Excel-CSRF, con propósito distinto al de perfiles. Configuración/contexto inicial expone su token únicamente al cliente autenticado y marca respuesta no-cache/no-store; nunca lo incluye en evidencia. POST JSON para ASMX y multipart para recepción, sin aceptar rutas físicas del cliente.

| Operación | Entrada principal | Salida / efecto |
|---|---|---|
| AbrirCargaExcel | Solicitud vacía; autoridad de sesión. | Límites efectivos y referencias/nombres de conexiones elegibles; contexto de protección. No escribe datos destino. |
| ListarPlantillas | termino, pagina positiva, tamanoPagina acotado (25 inicial). | Plantillas elegibles por ámbito/conexión, IDs y nombres; sin credenciales. |
| ConsultarPlantilla | plantillaId. | Cabecera y mapeos por nombre, revisionCarga y huella de configuración; destino y clave. |
| GuardarPlantilla | plantillaId opcional, revisionEsperada para edición, conexionId, nombre/tabla, hoja, formato XLS/XLSX, campoUnico y mapeos. | ID/revisión/huella; cabecera y campos se guardan juntos. No adecua índices de una tabla histórica automáticamente. |
| RecibirArchivo (ASHX) | plantillaId y exactamente un archivo multipart XLS/XLSX. | ID de carga, estado RECIBIDA, nombre descriptivo, bytes y hash; hojas reconocibles. Archivo guardado bajo referencia privada. |
| Preparar | idCarga, revisionPlantillaEsperada, hoja y modo CREAR/AGREGAR/REEMPLAZAR. | PREPARADA solo si archivo completo es válido; destino, clave, filas previstas/afectadas, huellas y muestra acotada. Con incidencias conserva RECIBIDA y devuelve diagnóstico. |
| Confirmar | idCarga, archivoHash, configuracionHash, esquemaHash y confirmacionReemplazo explícita si corresponde. | Resultado CONFIRMADA y filas efectivas; o rechazo/conflicto/actividad conocida sin duplicar efectos. No recibe filas desde el navegador. |
| ConsultarResultado | idCarga. | Estado persistido y resultado propio del usuario/ámbito; consulta sin insertar, confirmar o reanudar. |

Mapeos: campoFuente, campoDestino, tipoCampo y transformaciones explícitas de los nombres históricos. La clave pertenece a la lista de destinos. NOMBRE_PLANTILLA conserva el nombre físico validado; no duplicar el prefijo si ya existe. LIBRO_EXCEL conserva la hoja y CONEXION_FUENTE el marcador 8.0 xls o 12.0 xlsx. Plantillas web usan RUTA_ARCHIVO_FUENTE relativa generada de hasta 120 caracteres, por ejemplo web/<id>.xlsx; cada carga retiene su propia referencia inmutable. Plantillas históricas conservan su ruta como metadato, no como ruta de recepción elegida por cliente.

Banderas por defecto: crear/agregar 0/0/1 y reemplazar 0/1/0 para ELIMINA_TABLA/ELIMINA_INSERTA_RECORD/INSERTA_RECORD. Modo explícito por ejecución prevalece como intención registrada; una combinación histórica incoherente produce diagnóstico, nunca DROP por ELIMINA_TABLA.

Primera fila es encabezado; mantener posiciones vacías intermedias y números físicos, primera/última fila. Preservar ceros y apóstrofes; DATE tipado o yyyy-MM-dd/dd/MM/yyyy; una columna FECHA VARCHAR sigue siendo texto. No repetir pérdidas de apóstrofes/letras o primera fila del importador antiguo. Lectura completa antes de escribir; límite de filas comprobado durante lectura.

Vista previa inicial de hasta 25 filas e incidencias de muestra hasta 25, con total completo y truncamiento indicado. Esto acota respuestas, no limita la validación ni permite ignorar incidencias restantes. No se incorpora un motor de paginación/reanudación por fila en esta fase.

Snapshot canónico versión 1: ID/revisión y campos efectivos de plantilla, destino autorizado, formato/hoja, modo, clave/aislamiento, mapeos y transformaciones. Conservar nombres físicos de los campos históricos como claves del snapshot. Ordenar claves de objetos por comparación ordinal; mapeos por CAMPO_DESTINO y luego CAMPO_FUENTE, sin depender del orden de lectura SQL; rechazar destinos duplicados. Representar explícitamente NULL; enteros decimales sin ceros iniciales y booleanos JSON; cadenas sin normalizar su contenido. JSON sin espacios o saltos, escape ASCII con hexadecimal minúsculo, UTF8 sin BOM. SHA-256 sobre esos bytes. El esquema usa la misma canonización: motor, columnas usadas con tipo/nulabilidad/default/charset/collation/extra, ordenadas por nombre físico; índices por nombre y componentes por SEQ_IN_INDEX, incluyendo NON_UNIQUE y SUB_PART. Las fases implementadoras acreditan vectores y persistencia de este contrato antes de usar las huellas; la huella nunca sustituye autorización.

| Código contractual | Significado |
|---|---|
| OK | Operación completada con respuesta coherente. |
| SESION_REQUERIDA / ACCESO_DENEGADO / SOLICITUD_NO_VERIFICADA | Sesión, autoridad o protección inválidas. |
| CAPACIDAD_NO_DISPONIBLE | Configuración o esquema requeridos no verificables. |
| SOLICITUD_INVALIDA / REFERENCIA_NO_DISPONIBLE | Entrada o referencia propia no válida, sin revelar recursos ajenos. |
| ARCHIVO_INVALIDO / LIMITE_ARCHIVO_EXCEDIDO / LIMITE_FILAS_EXCEDIDO | Contenido/formato o límites incumplidos. |
| DATOS_INVALIDOS / CLAVE_DUPLICADA / ESQUEMA_INCOMPATIBLE | Incidencias de filas, unicidad o estructura; no confirmar. |
| CONFLICTO_REVISION / PREPARACION_INVALIDADA / DESTINO_EXISTENTE | Revisión, contenido preparado o procedencia del CREATE incompatibles. |
| OPERACION_EN_CURSO / RESULTADO_INCIERTO | Consultar el mismo ID antes de decidir; sin retry automático. |
| CARGA_FALLIDA | Fallo conocido con efectos revertidos; no equivale a desconexión. |

Incidencia: codigo, fila física opcional, columna física opcional, mensaje operativo saneado; no incluir SQL, valores masivos o excepciones. Las fases de transporte adaptan el envelope ASMX y status HTTP siguiendo los guardas existentes; una respuesta HTTP por sí sola no prueba el commit.

## Transacción y recuperación — D-07/RQ-07

1. Reautorizar y verificar que ID/contenido pertenecen al usuario/ámbito. Si CONFIRMADA, devolver su histórico aun si después cambiaron plantilla/esquema/límites.
2. Para una carga no confirmada, bloquear cooperativamente la DA física+tabla, iniciar transacción en la conexión dedicada, bloquear y releer ejecución/configuración y comprobar otro ganador. Revalidar archivo/snapshot/esquema/autoridad/límites antes de escribir.
3. AGREGAR: rechazar cualquier duplicado interno o existente. REEMPLAZAR: aceptar claves del conjunto sustituido, pero exigir archivo internamente único. DELETE filtrado e INSERT parametrizados acotados comparten transacción; nunca TRUNCATE o DROP.
4. Datos y CONFIRMADA/conteos/fecha hacen commit juntos. Fallo conocido revierte la operación completa; registrar FALLIDA de forma segura sin sobrescribir un ganador. Liberar el bloqueo cooperativo al terminar la conexión/operación conocida.
5. CREATE es fase estructural separada. Tabla nueva identificada por comentario ImportacionExcel:<idCarga> y huella de estructura; respuesta perdida exige verificar ambas. Tabla ajena: conflicto. Tabla propia vacía: resultado recuperable, sin borrar/recrear automáticamente.
6. Una pérdida de respuesta conserva el ID y exige consulta. Sin evidencia del resultado, no repetir por tiempo transcurrido. Los INSERT agrupados no son commits parciales ni puntos de pausa. No se promete continuar desde determinada fila o bytes recibidos.

## Límites y aceptación posterior — D-08/RQ-08

No hay endpoints, carga real, nuevo permiso concedido o instalación acreditados por esta entrega. Sin cambios compilables, build y E2E de producto no aplican. La evidencia de gobierno/contratos es local; se diferencia de MySQL y recorrido de usuario.

Las fases posteriores acreditan XLS/XLSX, 1000/5000/10000 filas, fronteras exactas y excesos, memoria/tiempos, rollback, dos confirmaciones, acuse perdido, CREATE y regresión SII/carpetas. Reutilizan tools/e2e y autorización específica; no instalan SQL en tests. Los prompts actuales no fijan versión de filas fuente ni bloqueo de reemplazo por lotes activos; la versión del perfil no congela los datos consultados por valores diferidos. No se añade aquí esa política ni se declara resuelta mediante estos contratos.

Referencia primaria para DDL y privilegios: [manual MySQL 5.1, sintaxis SQL](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/sql-syntax.html). CREATE/ALTER y concesiones se revisan como operaciones estructurales separadas de la transacción de datos; su generación no acredita ejecución.
