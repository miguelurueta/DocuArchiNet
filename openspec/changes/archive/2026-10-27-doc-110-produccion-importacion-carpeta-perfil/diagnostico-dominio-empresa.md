# DOC-110 — Origen del dominio de empresa

Investigación 2026-10-20: código y consultas SELECT sobre conexiones configuradas. Sin login/E2E, escrituras SQL o cambios funcionales. Evidencia saneada: [metadatos y conteos](evidencia-dominio-empresa.json).

## Recorrido comprobado

| Paso | Fuente exacta | Resultado |
|---|---|---|
| 1. Abrir catálogo de módulos | `Defaul/GestorModuleSesion.vb:59`, `Initialize` | Usa conexión configurada `OdbcServicesGestor`. |
| 2. Inicializar conexiones | Mismo archivo:73, `InicializaconexionesModulos` | Resuelve módulo y llama `AsignaAtributosConexionModuloRadicacion("GESTOR DOCUMENTAL", nombre_empresa, conn)`. |
| 3. Resolver destino documental | Mismo archivo:871–903 | Une `empresa_gestion_documental` con `gestor_modulos`; carga `RA_ID_MODULO`, `RA_ID_EMPRESA` y parámetros `RA_` del módulo documental. |
| 4. Abrir base documental | `Defaul/conect.vb:38–51`, `Dbase_Conction_Mysql_RA.Returna_Conexion_Mysql` | Usa `RA_DB_NAME_MODULO`, `RA_IP_SERVER_MODULO` y demás configuración RA. |
| 5. Resolver empresa del usuario | `Gestion/ClassAdmonEmpresa.vb:82–113` | En esa conexión RA une usuario de `remit_dest_interno` con `empresa_gestion_documental`; obtiene ID de empresa. |
| 6. Guardar contexto de negocio | `Defaul/ClassGestorSesion.vb:2698` | Asigna el resultado a `GA_IDEMPRESA` durante finalización de Gestión. |
| 7. Consumir estructura documental | `Gestion/Class_registro_organigrama.vb:11–18`; `Gestion/Class_ra_pro_niveles.vb:51` | Usa la conexión RA para organigrama y niveles. |

El prefijo RA tiene aquí un nombre histórico: también es la conexión de Gestión Documental. No significa que el perfil deba ubicarse en una base arbitraria de Radicación.

## Tres identidades diferentes

| Dato | Dominio | Uso en el perfil |
|---|---|---|
| `RA_ID_EMPRESA` | Empresa del catálogo que configura el módulo. | Contexto de enrutamiento; no sustituye la empresa del usuario. |
| `GA_IDEMPRESA` | Empresa documental del usuario dentro de la base RA seleccionada. | Fuente de ámbito, revalidada con relación vigente usuario/empresa. |
| `ID_EMPRESA` y conexión sin prefijo | Contexto de otro módulo según inicialización. En Gestión se inicializa también Workflow por defecto. | No usar como sustituto automático del dominio documental. |

No exigir igualdad numérica entre RA_ID_EMPRESA y GA_IDEMPRESA como si fuera una FK probada entre catálogos. La coherencia exigida es entre el usuario, la empresa resuelta en RA, el contexto autenticado y el recurso solicitado.

## Resultado de base de datos

- `OdbcServicesGestor` y `OdbcServices` reportan igual servidor/esquema en esta inspección. La comparación se hizo en memoria sin publicar identificadores de infraestructura.
- Ambas fuentes contienen empresa, usuarios, permisos, organigrama y niveles.
- En ambas lecturas: 78 usuarios, cero empresas nulas/cero y cero referencias positivas huérfanas. Los conteos no prueban integridad futura ni autorización de una cuenta.
- Hay un módulo GESTOR DOCUMENTAL activo; su nombre de base coincide con el esquema consultado. Esto por sí solo no demuestra que su servidor destino sea el mismo.
- `empresa_gestion_documental.ID_EMPRESA` es PK `int(10) unsigned`; `remit_dest_interno.id_Remit_Dest_Int` también. La referencia de empresa del usuario es `int(11)`, sin clave indicada en la columna consultada.
- La referencia de usuario en la tabla de permisos aparece como `MUL`: este dato no acredita unicidad. No asumir una sola fila sin verificar.
- No se encontraron `integracion_perfil` ni las dos columnas de permisos nuevas en las consultas acotadas. No se aplicó DDL.

## Decisión para cerrar A1 en diseño

El repositorio de perfiles debe usar el mismo origen RA que resuelve empresa, usuario y estructura documental. Crear las tablas propias en ese origen, mediante SQL manual, sin centralizarlas en un catálogo distinto. Al usar `ModuleSessionConnectionStringResolver.Resolve`, pasar explícitamente `sessionPrefix = "RA_"`; omitirlo puede seleccionar Workflow en este flujo.

Dentro de ese origen, el ámbito es el ID positivo de empresa documental en decimal canónico. La unicidad ámbito/código queda local a esa base. El servicio recibe contexto servidor; nunca acepta conexión o empresa autoritativa del cliente. No guardar host, credenciales o nombre de BD como ámbito.

La comprobación de usuario/empresa vigente y el repositorio deben compartir la conexión/origen autorizado por solicitud. Rechazar empresa cero, relación ambigua, contexto incoherente y recurso ajeno. La función legacy que devuelve YES con cero no cambia: el adaptador nuevo aplica el rechazo.

**A1 resuelto como contrato de arquitectura.** No queda una decisión de negocio sobre qué empresa usar. En implementación se verificará la selección RA y el despliegue manual del esquema allí; la inspección no acredita una sesión autenticada ni todos los ambientes. Centralización entre bases independientes sería ampliación de alcance y exigiría otro diseño de identidad.
