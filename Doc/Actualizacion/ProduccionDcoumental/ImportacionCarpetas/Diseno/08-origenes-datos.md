# Orígenes de datos — restricción confirmada

El usuario confirmó que las tablas del sistema no residen todas en una base. La coincidencia de servidor/esquema observada entre dos conexiones configuradas en una inspección no acredita una topología única para todos los módulos o ambientes.

## Evidencia por consumidor

| Datos / operación | Origen observado | Evidencia / límite |
|---|---|---|
| Catálogo de empresas y configuración de módulos del login | `OdbcServicesGestor` | `GestorModuleSesion.Initialize`; enrutamiento, no empresa documental del usuario por equivalencia de ID. |
| Empresa del usuario y permisos de Producción | Conexión `RA_` | `ClassAdmonEmpresa.Retorna_id_empresa_usuario_gestion` y `Class_remit_dest_interno_perfil_produccion` usan `Dbase_Conction_Mysql_RA`. |
| Organigrama y niveles de Producción | Conexión `RA_` | Consumidores inspeccionados `Class_registro_organigrama` / `Class_ra_pro_niveles`. |
| Configuración de gabinete consultada por `Solicita_estructura_configuracion_gabinete` | `RA_` | `Docuarchi/Class_system1.vb`, alrededor de 56. No demuestra equivalencia con el gabinete DA. |
| ID de gabinete solicitado por `SolicitaIdGabineteDocuarchi` | `DA_` | Mismo archivo, alrededor de 295; consulta `system1` usando `Dbase_Conction_Mysql_DA`. |
| Conexión DA | Parámetros de sesión `DA_*` | `Defaul/conect.vb`, alrededor de 251. Separada de RA y de la conexión sin prefijo. |
| Tres tablas nuevas de perfil | Ubicación propuesta: mismo origen RA de empresa/usuario | Decisión de persistencia local de DOC-110; tablas aún no aplicadas. |

No se ha cerrado el mapa completo de índices, tipologías, permisos de gabinete y almacenamiento. Caracterizar cada método realmente consumido; no deducir origen por el nombre del archivo/clase o por una tabla homónima.

## Reglas para implementación y SQL

1. Repositorio del perfil usa RA. Cada adaptador de catálogo usa la conexión comprobada de su operación. No convertir el adaptador de catálogo en un JOIN universal sobre RA.
2. Las FK del SQL 01 solo vinculan las tres tablas nuevas, co-localizadas. No agregar FK a gabinete, usuario, empresa u otros catálogos ni suponer transacción distribuida.
3. El ALTER de permisos corresponde al origen donde el lector RA encuentra `remit_dest_interno_perfil_produccion`. Confirmar destino antes de aplicar cada bloque; si el despliegue difiere del mapa observado, detener y revisar enrutamiento/script. No crear copias de tablas legacy para satisfacer el script.
4. `gabinete_id` aislado no identifica un recurso entre bases. Antes de cerrar su contrato operativo, documentar si corresponde al gabinete RA, al DA o a una referencia resuelta por relación existente. No asumir IDs iguales ni usar nombre como equivalencia automática.
5. Resolver el origen mediante configuración de módulos autorizada del servidor. El cliente no suministra host, base, credenciales ni SQL. No persistir secretos en la configuración del perfil.
6. Si la identidad persistida actual no permite distinguir el origen o detectar un cambio de enrutamiento, revisar el contrato y SQL antes de implementar selección/almacenamiento; no inventar aquí una columna o tabla de correspondencias.
7. La publicación transaccional solo cubre las tablas propias en RA. Lecturas de otros catálogos requieren validación y revalidación en su origen; no se promete una instantánea atómica global.

## Decisión vigente — 2026-10-20

El usuario confirmó DA. El adaptador consulta gabinete, índices y permisos en DA; clasificación, tipología, responsable, nivel y persistencia propia en RA. Las observaciones anteriores son antecedentes de la decisión, no impedimentos pendientes.

La configuración JSON de cada versión conserva OrigenGabinete=DA y HuellaOrigenGabinete (SHA-256 de identidad de servidor/base obtenida mediante SELECT). No contiene secretos ni requiere nuevas columnas. Los cambios de origen bloquean publicación/selección y requieren revisión administrativa; la huella no garantiza continuidad ante clonaciones o restauraciones.

SQL aplicado manualmente por el usuario. Verificación real de esquema mediante SELECT: PASS, evidencia ../Validacion/doc110-verificacion-esquema-runtime.json. Metadatos: ../Validacion/doc110-catalogos-metadatos.json. Pruebas aisladas distinguen RA/DA, índices ajenos y cambio de origen. Validación real de bases separadas e indisponibilidad pendiente. No se acredita almacenamiento ni transacción distribuida.
