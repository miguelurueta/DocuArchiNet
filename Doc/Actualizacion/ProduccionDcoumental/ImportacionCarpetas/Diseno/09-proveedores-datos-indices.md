# AMP-01 — Proveedores de datos para índices

Contrato documental de DOC-110, tareas 14.1 y 14.18, 2026-10-21. Define la extensión solicitada; no acredita código, tablas creadas ni consulta real de un proveedor. Corrección de alcance del usuario: la tabla fuente es configurable; una tabla real solo condiciona la aceptación integrada de 14.17, no el desarrollo del mecanismo.

## Alcance y evidencia

Primera entrega: un contrato de proveedores y un adaptador común de tablas/vistas configurables por proveedor, exclusivamente para INDICE_GABINETE. Una API concreta será otra entrega. No se cambia SII, la creación de expedientes ni el almacenamiento documental.

El resolvedor actual admite CONSTANTE, CARPETA, ARCHIVO y EXPEDIENTE y es puro. ServicioPerfilImportacion.Ensayar coordina resolución y ValidarValores del adaptador de catálogos; este último verifica el destino DA. Las reglas se persisten en regla_doc LONGTEXT, con serialización cerrada y huella del perfil. Estos límites se conservan.

RegistroProveedoresImportacion y RegistroClientesProveedoresImportacion del módulo Workflow son registros en memoria de instancias, no tablas de configuración versionada. SiiExternalImportProviderClient interpreta contratos específicos SII. Ninguno constituye una tabla reutilizable para este catálogo. El transporte HTTP genérico es candidato a evaluación futura, sin modificar sus consumidores actuales.

La búsqueda en código VB/SQL de orden de compra, orden de trabajo, tabla_origen y consulta_sql no identificó una fuente de negocio para el caso del usuario. El nombre ilustrativo del archivo no permite deducir tabla, base ni clave. Esta evidencia de repositorio no demuestra inexistencia de tablas en bases externas.

## D-08 — Consulta separada de la transformación

El perfil configura una fuente PROVEEDOR para una regla de índice. Su configuración referencia un proveedor y una versión concreta, una clave de consulta y una propiedad retornada. Una misma respuesta puede alimentar varias reglas; cada índice conserva una sola asignación. El nombre de la propiedad lógica es independiente del nombre físico de columna.

El proveedor registra un contrato cerrado de entrada y salida. La primera entrega admite una clave escalar; claves compuestas y consultas encadenadas quedan fuera hasta definir sus contratos. La clave puede provenir de CONSTANTE, CARPETA o ARCHIVO utilizando extracción completa/segmento y tratamiento de extensión actuales. Puede provenir de una propiedad permitida de EXPEDIENTE, respetando TRAS_EXPEDIENTE: si el dato todavía no existe, se registra dependencia diferida y no se consulta con un ID ficticio. No se admite que una consulta dependa de otra consulta ni de un índice de destino.

Las claves de texto conservan ceros iniciales. No se convierten identificaciones a número por apariencia. Tipo y longitud se verifican contra el contrato del proveedor antes de consultar. El tipo del índice se aplica al valor retornado, no impone el tipo de la clave de búsqueda.

Recorrido: contexto autenticado → validar versión/proveedor/clave → consultar adaptador → resolver propiedad → convertir y validar valor → ValidarValores contra gabinete DA → devolver ensayo por campo. El resultado del proveedor se construye en servidor; no se acepta como autoridad un diccionario enviado por el navegador. El resolvedor recibe valores y estados, sin conexiones ni HTTP.

Dentro de un ensayo se comparte respuesta por contexto autorizado, proveedor, versión y clave canónica. No hay caché global entre usuarios/empresas ni entre ensayos. Una lectura debe devolver como máximo dos coincidencias para distinguir unicidad; nunca seleccionar arbitrariamente la primera. Los límites de tiempo se fijan en configuración administrada del servidor, no en el cliente.

## Resultados y ausencias

| Situación | Comportamiento contractual |
|---|---|
| Una coincidencia y propiedad válida | Convertir y validar contra el índice de destino. |
| Ninguna coincidencia | Incidencia de ausencia por regla afectada; no inventar 0 ni cadena vacía. |
| Varias coincidencias | Incidencia de ambigüedad; no elegir por orden accidental. |
| Propiedad no declarada | Configuración inválida al guardar/publicar. |
| Propiedad declarada con NULL/vacío | Incidencia de valor ausente; mantener semántica actual del resolvedor. |
| Error de conexión o timeout | Fallo técnico diferenciado, sin mostrar conexión o SQL. |
| Dato de expediente aún inexistente | Dependencia diferida, no ensayo exitoso de un dato inventado. |

No se incorpora fallback silencioso ni omisión de reglas opcionales en este incremento. La política de importación determina si la incidencia bloquea expediente/lote o excluye el archivo en modo parcial. Opcional no significa sustituir un error del origen por un valor válido. Guardar/publicar valida el contrato y las referencias, no exige encontrar todas las claves que se usarán en el futuro ni garantiza disponibilidad futura del origen.

## Autorización y topología

Empresa y usuario proceden del contexto servidor vigente. El proveedor es de un ámbito explícito y debe autorizarse tanto al listar como al consultar. Una versión o propiedad conocida no confiere acceso a datos de otro ámbito. Desactivación impide nuevas consultas/selecciones, pero conserva lectura histórica del perfil.

La tabla fuente puede estar en una base distinta de RA y DA. El servidor resuelve una referencia lógica de conexión autorizada; el perfil no almacena contraseñas, cadena de conexión, SQL ni URL. El adaptador usa columnas/tablas previamente autorizadas y parámetros para valores. No se confunde la base del proveedor con el gabinete destino ni se promete transacción distribuida.

14.2 define el mecanismo de selección de conexión lógica, tabla/vista, clave, tipos, columnas de salida y filtro de empresa o aislamiento equivalente. Cada configuración se valida contra los metadatos de su origen antes de habilitarla; no se fija una tabla en el código. Si el origen no expresa empresa, se necesita un mapeo explícito de autorización; no omitir el filtro por comodidad.

## Persistencia y compatibilidad

Diccionario 14.3 concretado en [10-catalogo-proveedores-tabla](10-catalogo-proveedores-tabla.md): ra_integracion_proveedor y ra_integracion_proveedor_version en el origen RA de los perfiles. IDs incrementales; no duplicar los mapeos existentes de reglas. La versión publicada del perfil referencia la versión exacta del proveedor; un cambio de consulta crea otra versión y nunca sustituye silenciosamente el contrato publicado.

El contrato 10 exige administrar proveedores desde interfaz: borrador, revisión, validación, publicación e historial. SQL 04 ya fue aplicado manualmente; su esquema debe ampliarse para ese ciclo mediante migración separada. El editor de perfiles seleccionará versiones publicadas, sin edición de SQL ni scripts de registro. El esquema de proveedores y el adaptador genérico pueden desarrollarse sin una tabla de negocio elegida. No registrar como productiva una fuente ficticia; fixtures se limitan a pruebas. MySQL 5.1, LONGTEXT con esquema validado por aplicación y prefijo ra_; sin cambios en tablas SII.

La extensión del modelo no puede alterar huellas de publicaciones previas solo por agregar propiedades serializadas nulas. 14.8 debe demostrar compatibilidad con un fixture anterior y una estrategia explícita de serialización; no recalcular huellas antiguas para esconder una incompatibilidad. Los cuatro tipos de fuente anteriores mantienen su forma y comportamiento.

## Entrega al plan y al ejecutor — tarea 14.18

El prompt 06 (validación y plan) es consumidor de los valores resueltos. El plan debe conservar por archivo/índice: valor canónico, destino, proveedor/version, clave consultada y momento de resolución; registrar solamente propiedades utilizadas, no la respuesta completa con datos innecesarios. Esto es procedencia, no autorización, y requiere protección del plan por ámbito.

El ejecutor utiliza los valores confirmados del plan. Un reintento de almacenamiento no vuelve a consultar automáticamente el origen: podría obtener valores distintos. Actualizar datos requiere construir/revalidar una nueva revisión del plan y obtener confirmación antes de sustituir valores aprobados. La revalidación de permisos sigue siendo necesaria; conservar un valor no elude una revocación.

Las dependencias TRAS_EXPEDIENTE conservan su etapa y deben resolverse antes del almacenamiento del documento. El plan distingue valores aprobados de dependencias aún pendientes; no presenta una dependencia como índice confirmado. Las barreras de integridad del motor, incluido el riesgo de expediente ya creado si la resolución posterior falla, permanecen responsabilidad de sus prompts, sin prometer rollback global.

Integridad completa EXPEDIENTE: errores del origen bloquean la unidad correspondiente según fase. Integridad completa LOTE: bloquean el conjunto conforme a sus barreras. Parcial: errores identifican archivos no elegibles y se informa el resultado; no se declara éxito de un archivo sin sus valores válidos. DOC-110 ensaya el perfil, no ejecuta estas barreras.

## Condición pendiente

La solicitud anterior de una tabla concreta deja de bloquear diseño, SQL propio y adaptador configurable. Solo la prueba real 14.17 necesita uno o más orígenes de prueba autorizados. La tarea 14.2 está documentada en el contrato 10 y define el mecanismo y la validación de metadatos, sin exigir una tabla única de negocio. Pueden verificarse múltiples configuraciones con fixtures sin atribuirles aceptación real.

## Configuración de cada proveedor de tabla — precisión del usuario

El administrador registra una conexión lógica de la lista autorizada, tabla o vista, columna de búsqueda, propiedades de salida con sus columnas/tipos y política de aislamiento por empresa. La regla del perfil únicamente selecciona proveedor/version, extracción de clave y propiedad de salida. No contiene nombres físicos ni credenciales.

Un mismo adaptador ejecuta todas las configuraciones admitidas, sin clases ni ramas por nombre de tabla. Los identificadores físicos no pueden parametrizarse como valores SQL: se validan contra metadatos y lista autorizada, se delimitan correctamente y se rechazan expresiones, joins y SQL libre. Las claves y filtros se transmiten como parámetros. El ámbito del usuario no se sustituye por un valor editable del perfil. Si la estructura cambia, se informa incompatibilidad; no se cambia el mapeo automáticamente.

Cambiar conexión lógica, tabla/vista, clave, columnas o aislamiento genera una versión nueva del proveedor; las versiones referenciadas conservan su contrato. Elegir una tabla existente por configuración no modifica esa tabla. La administración debe respetar permisos existentes y quedar definida en 14.3, sin asumir que ocultar controles autoriza consultas.

La aceptación del adaptador requiere al menos dos configuraciones con nombres físicos y claves diferentes usando el mismo código, además de rechazo de tabla/columna no autorizada e identificadores manipulados. No se habilita SQL arbitrario ni se exige reprogramar para agregar una tabla compatible.