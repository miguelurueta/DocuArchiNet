# Catálogo de proveedores configurables — AMP-01

Contrato corregido por instrucción del usuario el 2026-10-22: administración de proveedores desde interfaz, dentro de DOC-110. SQL 04 ya aplicado manualmente y verificado por SELECT; el ciclo administrativo completo aún no está implementado.

## Conexiones y autorización

ModuleSessionConnectionStringResolver.Resolve ya materializa conexiones de módulo RA_/DA_ desde configuración servidor. Se reutiliza su resultado en la composición; no se modifica este componente compartido ni se acepta un prefijo de sesión del navegador. El adaptador recibe un resolvedor de conexión lógica construido por servidor. Una conexión externa adicional requiere registro administrativo servidor, no credenciales dentro del perfil. Alias desconocido o sin cadena disponible se rechaza.

La creación y modificación del proveedor se realizan desde una interfaz administrativa de esta entrega, sin scripts de registro. La pantalla permite seleccionar conexión lógica autorizada, tabla/vista y columnas obtenidas por metadatos. No permite escribir SQL. La lista de fuentes que el administrador puede ofrecer debe autorizarse en servidor con una política independiente del borrador: poder crear un proveedor no autoriza cualquier tabla accesible a la cuenta de conexión. La tarea 14.20 debe concretar permisos y catálogo permitido; no reutilizar automáticamente ADMINISTRAR_PERFILES_IMPORTACION como permiso de descubrimiento arbitrario.

Antes de consultar datos, se verifica ámbito/proveedor/version/disponibilidad y metadatos information_schema.COLUMNS en DATABASE() del origen resuelto. Tabla/vista y columnas son identificadores simples ASCII (letras, dígitos y guion bajo, primer carácter no numérico, máximo 64), delimitados con backticks después de validarlos. No se admiten calificadores de base, joins, expresiones ni SQL libre. Esta primera implementación soporta MySQL; otras tecnologías requieren otro adaptador bajo el mismo puerto.

Cada configuración exige columna de empresa con comparación parametrizada contra AmbitoRef. No se crea un modo SIN_FILTRO que permita acceso transversal. Fuentes sin esa columna pueden exponerse mediante una vista administrada que entregue el ámbito correcto; debe comprobarse la equivalencia de identidad, no suponer que un ID de empresa externo es igual a RA. Un aislamiento alternativo requiere ampliar explícitamente el contrato. La ausencia de una vista válida no bloquea otras configuraciones.

Primera consulta: una clave escalar, hasta 128 propiedades, tipo de clave TEXTO/ENTERO/DECIMAL/FECHA. Metadatos deben coincidir con tipos declarados; columnas text/blob, tipos no soportados y varchar/char de más de 4096 caracteres se rechazan en esta entrega para limitar lectura. Tiempo máximo de comando 5 segundos, SELECT LIMIT 2. Una fila es resultado, cero ausencia y dos ambigüedad. No se muta la fuente. Fallos técnicos se devuelven saneados.

## Reutilización y diccionario

Los registros Workflow existentes son diccionarios en memoria de clientes/proveedores, sin identidad empresarial ni versiones de configuración en BD. No se reutilizan sus tablas o contratos específicos SII. Se agregan dos tablas propias en RA, junto al perfil; nunca en la base fuente por deducción.

| Tabla | Campos y garantías |
|---|---|
| ra_integracion_proveedor | proveedor_id incremental, ambito_ref, codigo único por ámbito, nombre, activo por defecto 0, creado_por, creado_utc. La activación es administrativa; no carga filas ficticias. |
| ra_integracion_proveedor_version | version_id incremental, proveedor_id FK restrictiva, numero único por proveedor, tipo TABLA, conexion_ref lógica, tabla_nombre, clave_columna, clave_tipo, empresa_columna, propiedades_doc LONGTEXT con mapa lógico→columna/tipo, esquema_config=1, creado_por y creado_utc. |

La tabla anterior describe la base instalada por SQL 04, no el esquema final del ciclo administrativo. Los borradores se pueden editar con control de revisión. Las versiones publicadas son inmutables; cambiar tabla/clave/columna/conexión requiere crear otro borrador de versión y publicarlo. El repositorio debe implementar estas mutaciones mediante servicio, transacciones locales y autorización; la lectura existente por sí sola no cubre esta capacidad. La aplicación exige esquema cerrado y límites de propiedades_doc; MySQL 5.1 no aporta JSON ni CHECK efectivos. El catálogo no almacena contraseñas ni consultas SQL. La regla existente almacena referencia a proveedor/version y propiedad lógica; no se crea otra tabla de mapeos al gabinete.

## Instalación

SQL 04-proveedores-indices-mysql51.sql: ejecutar una vez en la base RA, con las tablas propias aún ausentes. Para instalación nueva ejecutar primero SQL 01 y después SQL 04; para instalación de DOC-110 existente aplicar solo SQL 04. Contiene únicamente CREATE TABLE; no búsquedas de existencia, DROP, seeds ni migración de datos de negocio. No alterar el archivo SQL 01 ya aplicado por el usuario.

El SQL se limita a instalar/actualizar estructura y permisos; no registra proveedores operativos. La configuración inicial se crea, valida y publica desde la interfaz. Deshabilitar proveedor conserva historial; no borrar versiones referenciadas. SQL 04 ya fue aplicado: no volver a ejecutarlo ni modificarlo como sustituto de una migración. La tarea 14.22 entregará un script incremental MySQL 5.1 para aplicación manual exclusiva del usuario, con tratamiento explícito de filas previas sin publicarlas automáticamente.

## Ciclo administrativo obligatorio

Administrar proveedores → crear borrador → seleccionar conexión autorizada → seleccionar tabla/vista → elegir clave y columna de empresa → mapear columnas a propiedades lógicas → validar metadatos → probar clave de ejemplo → guardar → publicar versión → seleccionar desde reglas del perfil.

La prueba de consulta no guarda ni publica por sí sola. Publicación exige contrato válido, revisión vigente y autorización; el resultado de una consulta de ejemplo no garantiza todas las claves futuras. No exigir que exista una fila de ejemplo para guardar un borrador. Distinguir validación estructural de ausencia/ambigüedad/indisponibilidad de datos.

La interfaz debe permitir listado/búsqueda, edición de borrador, consulta de historial, creación de versión desde publicación y cambio de disponibilidad. Publicado se muestra de solo lectura. Catálogos dependientes se invalidan al cambiar conexión o tabla; descartar respuestas antiguas y proteger cambios sin guardar. Códigos y nombres no sustituyen la identidad incremental.

La selección y el ensayo desde perfiles solo admiten versiones PUBLICADAS habilitadas del ámbito correcto. Revisar Listar y Obtener actuales: filtrar únicamente activo=1 resulta insuficiente al incorporar borradores. El servidor rechaza una versión BORRADOR aunque el cliente envíe su ID directamente. Los perfiles ya publicados mantienen referencia a una versión concreta y su huella.

El diccionario debe concretar estado, revisión, responsables/fechas de cambio/publicación y evidencia de publicación antes del ALTER. La instalación ya existente no demuestra esos campos ni permisos. No inventar concesiones ni promover filas previas silenciosamente. El historial de versiones conserva contratos previos.

## Aceptación

La E2E existente debe crear el proveedor desde UI, validar/probar un origen autorizado, guardar, publicar, seleccionarlo en un perfil y ensayar sus índices. Debe probar segunda versión, inmutabilidad de la primera, conflicto de revisión, revocación/no autorización y catálogo vacío. No insertar un proveedor por SQL para eludir la prueba del formulario. Fixtures de transporte solo acreditan UI aislada, no aceptación real.
## Diccionario del ciclo — 14.21, 2026-10-22

Script 05-ciclo-proveedores-mysql51.sql preparado, NO aplicado. Proveedor: revision INT UNSIGNED default 1, actualizado_por y actualizado_utc opcionales para filas históricas. Versión: estado BORRADOR por defecto, revision default 1, actualizado_por/utc, publicado_por/utc, contrato_hash BINARY(32). Las filas previas quedan BORRADOR; no se les inventa autor de publicación. Los nombres físicos pueden mantenerse vacíos en borrador según columnas existentes NOT NULL; no autoriza publicarlos incompletos. propiedades_doc usa [] para borrador sin propiedades.

Crear proveedor produce cabecera inactiva y primer borrador en una transacción. Guardar requiere revisión esperada y BORRADOR; aumenta revisión y registra autor/UTC. Publicar requiere revisión guardada, autorización y validación del contrato; registra PUBLICADA, autor/UTC y SHA-256 del JSON canónico del contrato (tipo, conexión lógica, tabla, clave/tipo, columna empresa y propiedades), excluyendo campos mutables de auditoría/disponibilidad. Nunca modifica una publicación previa. Crear versión clona contrato en nuevo BORRADOR con número obtenido bajo bloqueo de cabecera y revisión esperada. Activación/desactivación afecta cabecera con revisión esperada sin modificar versiones. Las mutaciones persisten auditoría en su misma transacción RA; no hay transacción distribuida.

Lectores de perfiles deben exigir PUBLICADA además de activo. Los filtros y la comprobación del coordinador ya fueron implementados y probados de forma aislada; necesitan SQL 05 antes de probar su consulta real. No se desplegó DLL. La administración visual/mutaciones aún no está implementada. La definición final del permiso y el alcance de fuentes se consultó al usuario y sigue pendiente: no activar descubrimiento ni mutaciones hasta resolver 14.20.
SQL 05 aplicado manualmente por el usuario y verificado el 2026-10-22 mediante SELECT. Evidencia: Validacion/doc110-amp01-ciclo-esquema.json bajo raíz funcional. PASS: diez columnas del ciclo (tipo, nulabilidad y defaults; revision unsigned y hash binary(32)), índice proveedor_id/estado/numero, tablas InnoDB e IDs incrementales. Catálogo sin proveedores/versiones; Repository con filtro publicado devuelve vacío y rechaza referencia inexistente. No hubo DDL ni escrituras del agente. Supera el pendiente de aplicación del SQL 05; no acredita administración visual, publicaciones reales ni permisos. La decisión de 14.20 sobre permiso y alcance de fuentes continúa pendiente.
## Política implementada — continuación restrictiva, 2026-10-22

ADMINISTRAR_PROVEEDORES_IMPORTACION es una capacidad adicional dentro del administrador de perfiles existente: el acceso actual requiere permiso de perfiles y, para descubrimiento/administración de fuentes, permiso específico de proveedores. La aplicación externa vigente asigna/revoca; SQL 06 solo agrega columna default 0 y no concede privilegios. AutorizarProveedores consulta el permiso vigente con usuario/empresa/estado, rechaza nulos, valores distintos de 1 y duplicados. Ausencia de columna afecta solo esta capacidad nueva.

La política independiente se obtiene de appSettings Doc110.Proveedores.FuentesAutorizadas (JSON cerrado, máximo 256 entradas). Cada entrada contiene AmbitoRef, ConexionRef (RA o DA), Tabla y Columnas; no contiene credenciales. Ausencia/vacío equivale a lista vacía, no acceso total. Esto es configuración de seguridad del servidor, no el registro funcional del proveedor: el futuro formulario permitirá crear, mapear y publicar proveedores dentro de ese alcance sin SQL. La configuración de seguridad no se entrega al navegador como autoridad ni puede ser editada por el borrador. No se modificó Web.config ni se habilitaron tablas reales en esta etapa.

ConsultarMetadatosProveedor reutiliza Ejecutar y CSRF existente y vuelve a comprobar el permiso específico. Solicitud: tipo CONEXIONES/TABLAS/COLUMNAS, conexionRef, tabla, pagina desde 1; máximo 25 elementos por página. Conexiones/tablas proceden de la lista autorizada; columnas se contrastan con information_schema.COLUMNS del origen seleccionado y se filtran. Tabla permitida que ya no existe devolverá columnas vacías y no podrá constituir contrato válido. No se exponen credenciales ni se aceptan prefijos de sesión/SQL del cliente. Un permiso de perfil por sí solo no descubre fuentes.

El adaptador de consulta exige política autorizante: ausencia de política deniega. La composición ASMX la aplica a cada consulta, también tras una revocación de alcance servidor. No basta con que un proveedor persistido nombre una tabla. Permisos/alcance no garantizan por sí solos que empresa_columna represente el mismo dominio RA; esa validación permanece obligatoria.

## AMP-01: fuente exclusiva de empresa por conexion (2026-10-22)

Decision del usuario: los modulos pertenecen a una sola empresa y su propia base; DA.imp_02_egresos no mezcla organizaciones. Conservar tabla y datos sin agregar EMPRESA. La clave es CONSECUTIVO (texto); CODIGOTIPO, TIPO, CONSECUTIVO, FECHA, NIT y BENEFICIARIO son varchar(250). No asumir unicidad: LIMIT 2 y resultado AMBIGUO permanecen.

Se incorpora ExclusivaEmpresa (booleano, default false) en FuenteProveedorAutorizada, exclusivamente en la politica del servidor. Su identidad sigue siendo AmbitoRef + ConexionRef + Tabla. Solo una entrada autorizada para el ambito actual con ExclusivaEmpresa=true permite ColumnaEmpresa vacia. El cliente no puede declarar esa autorizacion: el contrato del proveedor no incorpora el booleano. La conexion sigue resuelta en servidor desde la sesion RA/DA. La administracion valida esta condicion al validar/publicar y el adaptador la comprueba antes de abrir conexiones. Si existe columna, conserva el filtro parametrizado por ambito, incluso en fuente exclusiva.

Sin migracion SQL: empresa_columna VARCHAR(64) NOT NULL admite cadena vacia; el repositorio ya la guarda y recupera. No cambiar scripts 04/05/06, ni tabla fuente, ni SII. Publicaciones existentes conservan contrato y huella. Una politica sin el atributo mantiene la exigencia anterior de columna.

Configuracion propuesta para la fuente (pendiente del AmbitoRef real de la empresa; no copiar el marcador como ID):
```json
[{"AmbitoRef":"<ID_EMPRESA_REAL>","ConexionRef":"DA","Tabla":"imp_02_egresos","ExclusivaEmpresa":true,"Columnas":["CODIGOTIPO","TIPO","CONSECUTIVO","FECHA","NIT","BENEFICIARIO"]}]
```
La configuracion corresponde a appSettings Doc110.Proveedores.FuentesAutorizadas. No se modifico Web.config ni se creo un proveedor en BD: el alta funcional se realiza por interfaz una vez autorizado el origen y disponible el permiso.

FECHA se obtiene como texto, por ejemplo 23/01/2006 0:00:00. El formato identificado es dd/MM/yyyy H:mm:ss; su conversion a fecha requiere ampliar el conjunto permitido de formatos del resolvedor y del editor con pruebas. Esta entrega de aislamiento no afirma haber implementado esa conversion.

Verificacion: build aislado PASS; node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc110-proveedor-admin-editor.test.cjs: 5 PASS. Casos nuevos: autorizacion exclusiva explicita, default denegado, ambito ajeno, copia de politica sin mutacion, consulta sin parametro empresa, ceros iniciales, duplicados ambiguos y filtro anterior preservado. Pruebas con dobles; sin DDL, consultas de negocio reales ni despliegue.


Actualizacion AMP-02: el contrato 11-administracion-fuentes-da.md define la migracion de autorizaciones hacia RA y su interfaz. SQL 07 preparado sin aplicar. Este cambio de diseno no cambia aun el runtime, que conserva Web.config hasta 15.9.
