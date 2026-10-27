# DOC-110 — AMP-02: catálogo de fuentes autorizado en RA

Fecha: 2026-10-23. Contrato de implementación de 15.1–15.4; no acredita funcionalidad desplegada. Extiende D-09/RQ-12 y sustituye, cuando se implemente 15.9, la autoridad de Web.config descrita en el contrato 10.

## Responsabilidades

DA contiene los datos de negocio. RA conserva el catálogo de fuentes, sus columnas permitidas y su historial. El proveedor sigue guardando cómo buscar y qué propiedades devolver en sus tablas actuales. Crear un proveedor nunca crea una autorización implícita. No alterar imp_02_egresos, SII, sus consumidores ni el resolvedor de conexiones compartido.

Identidad de fuente: empresa autenticada + alias lógico RA/DA + nombre físico exacto de tabla/vista. Alias y credenciales se resuelven en servidor mediante ModuleSessionConnectionStringResolver. No recibir empresa, base, servidor, credenciales ni SQL como autoridad del cliente. Tabla y conexión son inmutables tras el alta; para otra identidad se crea otra fuente. El resto se modifica con revisión esperada.

## Autorización y descubrimiento inicial

Agregar ADMINISTRAR_FUENTES_IMPORTACION a remit_dest_interno_perfil_produccion: TINYINT UNSIGNED NOT NULL DEFAULT 0. Se asigna/revoca mediante la administración externa ya utilizada; no concederlo en la migración ni inferirlo de GA_MANAGER_CONFIGURACION.

El acceso al administrador de fuentes requiere ADMINISTRAR_PERFILES_IMPORTACION y ADMINISTRAR_FUENTES_IMPORTACION. Crear proveedores conserva el permiso ADMINISTRAR_PROVEEDORES_IMPORTACION. Una persona puede poseer las tres capacidades, pero ninguna se deduce de otra. Comprobar usuario activo, empresa, fila única y valor exacto 1 en servidor, mediante el patrón existente de PerfilImportacionContextAdapter. Los lectores de perfiles no requieren el permiso de fuentes para usar una fuente ya autorizada.

El nuevo descubrimiento administrativo, exclusivo para quien autoriza fuentes, consulta information_schema.TABLES/COLUMNS de DATABASE() en la conexión de sesión seleccionada, con paginación de 25, filtros parametrizados y timeout de cinco segundos. Ofrece tablas/vistas con identificadores admitidos y columnas compatibles, sin consultar filas de negocio. No depende de la lista ya autorizada: esto resuelve el alta inicial. No expone otras bases ni concede permisos MySQL. El descubrimiento ordinario de proveedores conserva el filtro por catálogo autorizado.

Autorizar una fuente permite que los usuarios habilitados para perfiles/proveedores consulten las columnas autorizadas mediante esos casos de uso. La interfaz debe explicar esta consecuencia antes de guardar una nueva autorización o ampliar columnas; no es una simple etiqueta visual.

## Aislamiento empresarial

Dos modalidades: COLUMNA y CONEXION. En COLUMNA, la fuente guarda la columna de empresa autorizada; no basta con dejar que cada proveedor elija cualquier columna. La versión del proveedor debe coincidir con esa columna y la consulta aplica @empresa. La columna debe pertenecer al conjunto autorizado y representar el mismo dominio empresarial del contexto RA.

En CONEXION, el administrador declara que la tabla/vista pertenece exclusivamente a la empresa por la conexión del módulo; columna_empresa se guarda vacía. La versión del proveedor también debe omitirla. La declaración es configuración de autorización, no un dato que el proveedor pueda concederse. Para imp_02_egresos corresponde CONEXION por confirmación del usuario. No agregar EMPRESA a la tabla.

## Diccionario físico

Tres tablas nuevas, InnoDB, en la misma RA de ra_integracion_proveedor. IDs INT UNSIGNED AUTO_INCREMENT. Referencias a empresa/usuario VARCHAR(128) ASCII siguen el contrato actual de identidad; no crear FK hacia otras bases ni asumir tipos legacy.

| Tabla | Columnas y reglas |
|---|---|
| ra_integracion_fuente | fuente_id PK; ambito_ref varchar(128); conexion_ref varchar(64); tabla_nombre varchar(64); modo_aislamiento varchar(16); columna_empresa varchar(64), vacía en CONEXION; activo tinyint unsigned default 0; revision int unsigned default 1; creado_por/actualizado_por varchar(128), creado_utc/actualizado_utc datetime. UNIQUE ámbito/conexión/tabla; índice ámbito/activo/id. |
| ra_integracion_fuente_columna | columna_id PK incremental; fuente_id FK; nombre varchar(64); UNIQUE fuente/nombre. Hasta 128 columnas permitidas por fuente. Tipos/longitudes se contrastan en el origen, no se duplican como autoridad en esta tabla. |
| ra_integracion_fuente_historial | historial_id PK incremental; fuente_id FK; revision int unsigned; accion varchar(24); actor_ref varchar(128); fecha_utc datetime; motivo varchar(500) UTF8; estado_doc LONGTEXT ASCII. UNIQUE fuente/revisión. Snapshot JSON canónico de la autorización resultante, incluyendo columnas; nunca datos de negocio, claves consultadas o credenciales. |

Identificadores físicos ASCII con comparación exacta ascii_bin; el servicio rechaza columnas duplicadas ignorando mayúsculas para conservar validaciones actuales. Historial serializado mediante PerfilImportacionJson, con límite de 262144 caracteres; motivo obligatorio de 1–500 caracteres. Revisiones/estados/límites se validan en servicio, no mediante CHECK ignorados por motores antiguos. No JSON nativo, DATETIME con fracciones, triggers ni valores default para LONGTEXT. Fechas UTC suministradas por aplicación.

## Transacciones y revocación

Alta: validar metadatos y autorización, insertar cabecera inicialmente inactiva, columnas e historial ALTA/revisión 1 en una sola transacción. Activación es explícita y exige contrato completo y vigente. Edición, ACTIVAR y REVOCAR bloquean cabecera con FOR UPDATE, comparan revisión y escriben estado/columnas e historial juntos. Si falla auditoría, rollback de toda la mutación. Identidad inmutable; sin borrado físico de fuentes/historial. El reemplazo transaccional de columnas debe conservar coherencia con el snapshot histórico.

Una petición usa una lectura consistente del catálogo por empresa, compartida entre validación y consultas de esa petición; no cargarlo por cada índice. No introducir caché entre peticiones en esta entrega. Una revocación confirmada rige para nuevas peticiones; no cancela una consulta ya iniciada. Documentar esta frontera, sin prometer revocación retroactiva. Fallo de RA deniega capacidad, nunca equivale a acceso total ni permite recurrir al archivo.

Las referencias existentes de proveedores se resuelven por ámbito/conexión/tabla: no alterar sus contratos publicados ni hashes para agregar fuente_id. Fuente ausente/inactiva, columna revocada o aislamiento incompatible impiden validar/publicar/consultar, conservando las publicaciones y sus referencias para diagnóstico. Historial propio de autorización no reemplaza historial de versiones de proveedor.

## Reutilización de auditoría: evidencia del repositorio

- Gestion/ClassGaExpediente.vb escribe ra_log_expediente con ID_UNIDAD y semántica de expediente: no es un catálogo de autorizaciones.
- Infrastructure/Workflow/Terminar/WorkflowLegacyAuditoriaAdapter.vb exige usuario Workflow y tarea; abre conexión propia a log_usuario, fuera de la transacción RA propuesta.
- Services/Workflow/ImportarServicioWeb/ImportExecutionSteps.vb contiene auditoría específica de transiciones de importación. No modificar esa integración para este catálogo.
- Las tablas de proveedor/version conservan contrato/publicación, pero no cada modificación de una autorización independiente.

Por estas restricciones se define historial propio transaccional; se reutilizan serialización, contexto, patrones de repositorio y guard CSRF existentes. La revisión no afirma que todo el repositorio carezca de otros mecanismos de auditoría.

## Transición y recuperación

Procedimiento operativo y controles de instalación consolidados el 2026-10-26: [guía SQL 04–07](../Sql/04-07-instalacion-proveedores-fuentes.md). Distingue el contrato de la autoridad RA ya compuesta en el código y de la aceptación pendiente del catálogo; conserva los estados históricos documentados abajo.

SQL nuevo es estructural y manual, sin INSERT de fuentes ni concesión de permisos. Instalación incremental tras 04/05/06: ejecutar 07-fuentes-autorizadas-mysql51.sql en RA. No repetir scripts anteriores. Los datos reales siguen en DA.

Al implementar 15.9, RA será la única autoridad. No mezclar listas ni mantener fallback a Doc110.Proveedores.FuentesAutorizadas. Antes de activar el cambio, inventariar entradas del archivo y registrarlas por interfaz con responsable/motivo; catálogo vacío significa acceso denegado. No activar el nuevo consumidor hasta completar el registro necesario en el ambiente. En el entorno observado no había entradas configuradas; verificar otra vez al instalar.

Una reversión de código no debe reactivar entradas antiguas de Web.config. Ante un problema, detener la capacidad y corregir/recuperar el catálogo preservando historial; no restaurar automáticamente una política anterior. Despliegue al servidor principal a cargo del usuario.

## Aceptación

Probar ambas modalidades, columna empresarial manipulada, permiso insuficiente, referencias a otra empresa, revocación en petición posterior, conflicto concurrente con un ganador e historial atómico. E2E reutiliza tools/e2e y el editor existente: registrar fuente desde UI, activarla, crear proveedor y ensayar un perfil, sin semillas SQL para eludir el formulario. Distinguir pruebas simuladas de aceptación real. CONSECUTIVO sigue siendo texto, con LIMIT 2 y AMBIGUO si hay duplicados; el formato de FECHA permanece en 14.33.

Actualizacion de seguridad: contrato 12-restriccion-tablas-fuentes.md restringe el descubrimiento inicial y el uso al prefijo literal imp_02 o excepciones exactas configuradas por servidor. Esta regla sustituye la oferta general de tablas/vistas descrita anteriormente; los permisos no levantan este limite.
