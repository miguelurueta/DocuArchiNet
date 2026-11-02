# DOC-111: contratos y entrega del esquema

Trabajo realizado en el directorio principal `D:/imagenesda/DocuachiNet/DocuArchiNet`, el 2026-10-27. Alcance: tareas 2.1–2.10 y 3.1–3.9 del plan atómico. El refinamiento quedó autorizado en 1.3. Progreso de este corte: **22/130**, incluyendo las tres tareas de refinamiento. La carga por carpetas todavía no está habilitada.

## Contratos implementados

`ImportacionCarpetasModels.vb` separa contexto operativo, manifiesto, snapshot, plan, lote, raíz, archivo, intento y evento. Reutiliza reglas/valores diferidos DOC-110; no presupone un expediente creado. Los estados de recepción y efecto son dimensiones independientes. El catálogo de estados valida pertenencia; las transiciones y barreras aún están pendientes.

`ImportacionCarpetasInterfaces.vb` define un único repositorio operativo y puertos internos de staging/efectos. Los DTO de apertura, preparación, confirmación y recepción aceptan referencias cliente acotadas y no autoridad, snapshots o rutas físicas. Los bytes se transportan como cadenas decimales para conservar precisión BIGINT UNSIGNED.

`LimitesImportacionCarpetas.vb` exige límites y duraciones positivos definidos por servidor. Una política ausente o inválida produce `CAPACIDAD_NO_DISPONIBLE`. No se han conectado aún estos límites a configuración ni a endpoints. Los cuatro archivos VB se registraron en el proyecto existente.

## Evidencia local

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS. Compila y ejecuta el arnés .NET Framework existente con las familias DOC-110 y los casos DOC-111. Comprueba límites ausentes/inválidos, precisión de bytes, raíces homónimas, snapshot serializado independiente, valores diferidos, separación de estados y rechazo de propiedades JSON autoritativas, duplicadas o de tipo incorrecto. No consulta BD ni HTTP.
- Build Debug del proyecto `GestionDocumental-Docuarchi.net.vbproj` con MSBuild, `DeployOnBuild=false` y salidas ` .tmp/doc111-build/bin` / `.tmp/doc111-build/obj`: PASS. No se reemplaza el binario de la aplicación en ejecución.
- `tools/e2e/artifacts/doc111-schema-preflight.json`: inspección SELECT de metadatos. MySQL 5.1.37-community, padre de versión compatible, seis tablas operativas ausentes y permiso ausente. No contiene cadenas de conexión ni credenciales.
- `tools/e2e/artifacts/doc111-ddl-review.json`: revisión estructural local del archivo SQL. Seis CREATE TABLE, un ALTER de permiso, diez FK con tipos compatibles y RESTRICT, 23 índices contando PK y claves compuestas de hasta 320 bytes de datos. Esta revisión no interpreta SQL en el servidor MySQL ni acredita una instalación.

## Esquema entregado

Corrección solicitada por el usuario el 2026-10-27: las seis tablas operativas llevan el prefijo `ra_`: `ra_integracion_plan`, `ra_integracion_lote`, `ra_integracion_raiz`, `ra_integracion_archivo`, `ra_integracion_intento` y `ra_integracion_evento`. SQL, referencias FK, consultas de control y diseño se actualizaron conjuntamente. Se repitió la inspección de metadatos para estos nombres: todos ausentes. La revisión estructural y OpenSpec estricto volvieron a pasar. No se crearon ni renombraron tablas en el servidor.

Revisión adicional pedida para MySQL 5.1: sin incompatibilidades detectadas en sintaxis/estructura, contrastada con documentación oficial. La inspección SELECT ampliada confirmó PK de versión compatible, soporte InnoDB/ascii_bin y ausencia de los diez nombres de constraints. El detalle está en `Sql/02-aplicacion-carga-carpetas.md`. Esto no acredita haber ejecutado el script en MySQL.

`Sql/02-carga-carpetas-mysql51.sql` materializa las seis entidades del modelo común con InnoDB, IDs incrementales, documentos ASCII, hashes binarios, estados separados y restricciones de identidad e idempotencia. `IMPORTAR_CARPETAS` queda denegado por defecto y no se conceden permisos a cuentas. No hay DDL de SII.

`Sql/02-aplicacion-carga-carpetas.md` contiene precondiciones, orden, controles SELECT, detección de esquema parcial y recuperación revisada por operador. Explica el commit implícito de DDL y evita una promesa de rollback transaccional. Los índices/PK de la dependencia y demás metadatos se vuelven a contrastar en la ventana de aplicación.

**No ejecutado:** aplicación del DDL, integración transaccional, confirmaciones concurrentes, staging real, creación de expedientes, almacenamiento, UI operativa o REC-01/02. La evidencia DOC-110 previa no cierra estas verificaciones DOC-111. Continúa el bloque 4 de persistencia; los controles reales requieren su ambiente y autorización específicos.
