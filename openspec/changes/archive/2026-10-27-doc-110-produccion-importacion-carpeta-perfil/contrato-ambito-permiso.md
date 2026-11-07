# DOC-110 — Contrato de ámbito y permiso

Fecha: 2026-10-20. Desarrollo documental de D-05/RQ-05. Los elementos nuevos son propuestas, no implementación ni esquema desplegado.

## Evidencia y diagnóstico

| Evidencia | Diagnóstico |
|---|---|
| `Gestion/ClassAdmonEmpresa.vb`, `Retorna_id_empresa_usuario_gestion`: relaciona `remit_dest_interno.Empresa_Gestion_Documental_id_empresa` con `empresa_gestion_documental.ID_EMPRESA`. | La empresa tiene identidad de negocio, independiente de razón social o nombre de BD. |
| La función devuelve `YES` con empresa 0 cuando no encuentra filas. | Exigir relación vigente e ID positivo; el texto de éxito no basta. Aplicar el control en el adaptador nuevo sin cambiar la función compartida. |
| `Defaul/ClassGestorSesion.vb`: finalización de Gestión carga permisos y empresa; también existen rutas de consulta pública. | Variables de sesión aisladas no acreditan autenticación administrativa. Verificar contexto autenticado vigente y rechazar consulta pública. |
| `Gestion/Class_remit_dest_interno_perfil_produccion.vb`: lee permisos por usuario y toma la primera fila. | Reutilizar infraestructura; el control nuevo debe rechazar resultados ambiguos sin alterar consumidores anteriores. |
| Búsqueda de la tabla en fuentes versionadas VB/ASPX/SQL: lector, llamadas de sesión y consulta de Radicación. | No se identificó escritor o pantalla de asignación. No afirmar que una pantalla concreta ya soporta el permiso. |
| `02-evidencia-esquema-perfil.json`: tabla de permisos en `candidateNames`. | Es lista de candidatos, no evidencia de columnas/tipos/unicidad. Verificar estructura antes del DDL. |

## Contrato de ámbito

Precisión multibase: empresa/usuario y persistencia propia comparten RA; otros catálogos no necesariamente. Las referencias a «catálogos» en este contrato se limitan al catálogo empresarial que emite el ámbito, no a gabinete/índices/almacenamiento. Ver documento común `Diseno/08-origenes-datos.md`.

El adaptador obtiene identidad autenticada y conexión del módulo desde servidor. Resuelve la relación vigente usuario → empresa y exige exactamente una relación válida, ID positivo y coherencia con el contexto de sesión. Ausencia, ambigüedad o discrepancia rechazan; no corregir silenciosamente ni aceptar empresa enviada por cliente.

El ámbito lógico propuesto es el ID de empresa en decimal canónico positivo, sin ceros iniciales. Aplicarlo a lectura, escritura, unicidad de código, catálogos y autorización del recurso.

Ese ID es local al catálogo que lo emite. Antes de cerrar A1/DDL, verificar que el repositorio de perfiles y los catálogos comparten ese dominio de identidad. `ModuleSessionConnectionStringResolver` resuelve conexión, pero no demuestra esa equivalencia. Si se centralizan perfiles de catálogos independientes con IDs repetidos, diseñar previamente una identidad estable de origen autorizada; no usar credenciales o nombre de BD ni inventar ahora una plataforma multiorigen.

Actualización tras rastreo y SELECT: [diagnóstico del dominio](diagnostico-dominio-empresa.md). A1 queda resuelto en diseño: repositorio de perfiles en el mismo origen RA que usuario/empresa y catálogos documentales; resolvedor con prefijo explícito `RA_`. El ID de empresa es local a ese origen. La verificación de despliegue permanece como criterio de implementación, no como decisión de negocio abierta.

## Mapeo físico propuesto

Verificación física completada mediante SELECT el 2026-10-20: [evidencia de permisos](evidencia-esquema-permiso.json). La tabla es InnoDB, la relación con usuario tiene índice no único y los datos actuales no presentan duplicados. El campo nuevo está ausente. Se entrega su definición en SQL 01 y procedimiento manual; no está aplicado. Esto completa la revisión física de 1.2, no localiza el escritor administrativo de 1.8.

| Elemento | DOC-110 | Entrega posterior |
|---|---|---|
| Capacidad/columna propuesta | `ADMINISTRAR_PERFILES_IMPORTACION` | `IMPORTAR_CARPETAS` |
| Tabla candidata existente | `remit_dest_interno_perfil_produccion` | La misma, previa verificación |
| Clave de sesión propuesta | `GA_ADMINISTRAR_PERFILES_IMPORTACION` | `GA_IMPORTAR_CARPETAS` |
| Tipo propuesto | `TINYINT UNSIGNED NOT NULL DEFAULT 0` | Igual, en su entrega |
| Concesión válida | Solo valor numérico exacto 1 | Misma regla |

Verificar esquema real antes de generar ALTER; no duplicar infraestructura ni heredar permisos generales. MySQL 5.1 no garantiza dominio booleano por ese tipo: la aplicación rechaza otros valores.

Cada solicitud protegida del perfil consulta el permiso vigente en servidor junto con empresa/recurso. No depende exclusivamente de la copia de sesión. Revocación confirmada impide la siguiente solicitud; no promete interrumpir una transacción iniciada. Error de consulta, ausencia, nulo, valor inválido o filas ambiguas deniega. Este comportamiento se limita al adaptador nuevo; no cambia la vigencia de permisos legacy ni SII.

La inicialización y el cargador compartido deben poner la clave nueva en cero antes de resolverla y comprobar existencia de columna antes de leerla. Si el DDL aún no está aplicado, conservar login y capacidades previas; deshabilitar solo perfiles.

## Dependencia de asignación

El usuario confirmó que la administración está en otra aplicación fuera del repo. Ver `contrato-administracion-externa.md`: DOC-110 entrega columna/lectura/autorización; la aplicación externa asigna/revoca. No crear UI o endpoint local de concesión. La aceptación conjunta queda como dependencia de habilitación. Aplicar DDL manual crea la columna en cero, no asigna capacidades a cuentas.

## Criterios de pruebas futuras

| Caso | Resultado esperado |
|---|---|
| Resolución YES con empresa cero | Rechazo sin persistencia. |
| Empresa manipulada, sesión incoherente o recurso ajeno | Rechazo sin revelar datos. |
| Mismo ID en catálogos distintos | No asumir equivalencia sin dominio verificado. |
| Permiso revocado, inválido o consulta fallida | Denegación en siguiente solicitud protegida. |
| Columna nueva ausente | Login/permisos anteriores preservados; perfiles deshabilitados. |
| Solo permiso general u operativo | No permite administrar perfiles. |

No se ejecutó SQL, se asignaron permisos ni se realizaron E2E.

## Política implementada — continuación restrictiva, 2026-10-22

ADMINISTRAR_PROVEEDORES_IMPORTACION es una capacidad adicional dentro del administrador de perfiles existente: el acceso actual requiere permiso de perfiles y, para descubrimiento/administración de fuentes, permiso específico de proveedores. La aplicación externa vigente asigna/revoca; SQL 06 solo agrega columna default 0 y no concede privilegios. AutorizarProveedores consulta el permiso vigente con usuario/empresa/estado, rechaza nulos, valores distintos de 1 y duplicados. Ausencia de columna afecta solo esta capacidad nueva.

La política independiente se obtiene de appSettings Doc110.Proveedores.FuentesAutorizadas (JSON cerrado, máximo 256 entradas). Cada entrada contiene AmbitoRef, ConexionRef (RA o DA), Tabla y Columnas; no contiene credenciales. Ausencia/vacío equivale a lista vacía, no acceso total. Esto es configuración de seguridad del servidor, no el registro funcional del proveedor: el futuro formulario permitirá crear, mapear y publicar proveedores dentro de ese alcance sin SQL. La configuración de seguridad no se entrega al navegador como autoridad ni puede ser editada por el borrador. No se modificó Web.config ni se habilitaron tablas reales en esta etapa.

ConsultarMetadatosProveedor reutiliza Ejecutar y CSRF existente y vuelve a comprobar el permiso específico. Solicitud: tipo CONEXIONES/TABLAS/COLUMNAS, conexionRef, tabla, pagina desde 1; máximo 25 elementos por página. Conexiones/tablas proceden de la lista autorizada; columnas se contrastan con information_schema.COLUMNS del origen seleccionado y se filtran. Tabla permitida que ya no existe devolverá columnas vacías y no podrá constituir contrato válido. No se exponen credenciales ni se aceptan prefijos de sesión/SQL del cliente. Un permiso de perfil por sí solo no descubre fuentes.

El adaptador de consulta exige política autorizante: ausencia de política deniega. La composición ASMX la aplica a cada consulta, también tras una revocación de alcance servidor. No basta con que un proveedor persistido nombre una tabla. Permisos/alcance no garantizan por sí solos que empresa_columna represente el mismo dominio RA; esa validación permanece obligatoria.