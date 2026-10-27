# DOC-110: instalación de proveedores y transición de fuentes a RA

Guía operativa de 14.19/15.17, revisada el 2026-10-26. Es una entrega documental; instalar, conceder permisos y desplegar siguen a cargo del equipo. No acredita ejecución de los comandos SQL siguientes ni aceptación del catálogo de fuentes.

## Destino y orden

RA es el origen documental del módulo, resuelto en servidor con `ModuleSessionConnectionStringResolver` y prefijo `RA_`. Allí residen perfiles, proveedores, fuentes y sus historiales. DA conserva tablas/vistas de negocio, incluida `imp_02_egresos`; los scripts siguientes no se aplican en DA. Confirmar el destino sin copiar cadenas de conexión a evidencias.

| Estado previo comprobado | Aplicación manual necesaria |
|---|---|
| Instalación nueva sin perfiles | [SQL 01](01-perfil-importacion-mysql51.sql), después 04 proveedores, 05, 06 y 07. |
| Perfiles incrementales ya instalados; sin proveedores | [04 proveedores](04-proveedores-indices-mysql51.sql), [05 ciclo](05-ciclo-proveedores-mysql51.sql), [06 permiso](06-permiso-proveedores-mysql51.sql), [07 fuentes](07-fuentes-autorizadas-mysql51.sql). |
| Proveedores creados con 04; sin ciclo administrativo | Solo 05, 06 y 07 faltantes, previa comparación física. |
| 04/05/06 completos; sin catálogo de fuentes | Solo 07. |
| Instalación parcial | Comparar cada objeto/columna/índice; completar únicamente sentencias faltantes compatibles. No repetir un script entero. |

`04-activar-identificadores-perfil-mysql51.sql` es otro archivo: pertenece a la migración de identidades del perfil, no instala proveedores. Para perfiles antiguos seguir [procedimiento 03](03-aplicacion-migracion-identidades.md) antes de instalar consumidores incrementales. No convertir IDs ni publicaciones por inferencia de esta guía.

Scripts de una sola aplicación, MySQL 5.1/InnoDB, INT UNSIGNED AUTO_INCREMENT, ASCII para identidades/JSON, UTF8 para texto visible, fechas UTC escritas por aplicación. Sin JSON nativo, utf8mb4, CHECK como garantía ni migraciones al arrancar. 05 deja registros preexistentes como BORRADOR; no equivale a publicarlos. 06/07 añaden permisos con default 0; no asignan capacidades ni autorizan fuentes.

## Comprobaciones de solo lectura

El equipo selecciona la RA correcta en su cliente. Estos SELECT son controles estructurales y agregados, no exponen credenciales, documentos ni filas DA. Ejecutar controles de datos solo cuando existan sus tablas.

```sql
SELECT VERSION() AS version_mysql, DATABASE() AS base_actual;

SELECT TABLE_NAME, ENGINE, TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('ra_integracion_perfil','ra_integracion_proveedor','ra_integracion_proveedor_version',
  'ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial')
ORDER BY TABLE_NAME;

SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE, COLUMN_DEFAULT,
       EXTRA, CHARACTER_SET_NAME, COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE() AND
 (TABLE_NAME IN ('ra_integracion_proveedor','ra_integracion_proveedor_version',
                'ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial')
  OR (TABLE_NAME='remit_dest_interno_perfil_produccion' AND COLUMN_NAME IN
      ('ADMINISTRAR_PERFILES_IMPORTACION','ADMINISTRAR_PROVEEDORES_IMPORTACION','ADMINISTRAR_FUENTES_IMPORTACION')))
ORDER BY TABLE_NAME, ORDINAL_POSITION;

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN
 ('ra_integracion_proveedor','ra_integracion_proveedor_version',
  'ra_integracion_fuente','ra_integracion_fuente_columna','ra_integracion_fuente_historial')
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

SELECT TABLE_NAME, CONSTRAINT_NAME, COLUMN_NAME, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA=DATABASE() AND REFERENCED_TABLE_NAME IS NOT NULL AND TABLE_NAME IN
 ('ra_integracion_proveedor_version','ra_integracion_fuente_columna','ra_integracion_fuente_historial')
ORDER BY TABLE_NAME, CONSTRAINT_NAME;

SELECT COUNT(*) AS versiones_proveedor_huerfanas
FROM ra_integracion_proveedor_version v LEFT JOIN ra_integracion_proveedor p
 ON p.proveedor_id=v.proveedor_id WHERE p.proveedor_id IS NULL;

SELECT COUNT(*) AS columnas_fuente_huerfanas
FROM ra_integracion_fuente_columna c LEFT JOIN ra_integracion_fuente f
 ON f.fuente_id=c.fuente_id WHERE f.fuente_id IS NULL;

SELECT COUNT(*) AS historiales_fuente_huerfanos
FROM ra_integracion_fuente_historial h LEFT JOIN ra_integracion_fuente f
 ON f.fuente_id=h.fuente_id WHERE f.fuente_id IS NULL;

SELECT COUNT(*) AS fuentes_sin_historial_de_revision_actual
FROM ra_integracion_fuente f LEFT JOIN ra_integracion_fuente_historial h
 ON h.fuente_id=f.fuente_id AND h.revision=f.revision WHERE h.historial_id IS NULL;
```

Contrastar columnas, longitudes, nulabilidad, índices únicos, PK/FK y reglas RESTRICT con los DDL completos 04/05/07; presencia de tabla no basta. Verificar también tipos/defaults exactos de los permisos. Los conteos de huérfanos y de revisiones sin historial deben ser cero. Si hay diferencias, detener la habilitación y diagnosticar; no corregir con borrados ni historial inventado. Comparar conteos y lecturas de publicaciones previas antes/después; para JSON/huellas usar el codec de la aplicación, no funciones JSON de MySQL posteriores a 5.1.

Guardar evidencia saneada con fecha UTC, ambiente lógico, base destino confirmada por el responsable, commit y cambios locales probados, archivos aplicados, resultado y limitaciones. La base local histórica no demuestra instalación de otro servidor.

## Transición de autoridad y permisos

Antes de activar el consumidor nuevo, el responsable inventaría las entradas antiguas de `Doc110.Proveedores.FuentesAutorizadas`, si existen, en un registro restringido. Conserva empresa, alias, tabla, columnas y modalidad, responsable y decisión de aprobar/rechazar cada autorización. No copia credenciales ni un Web.config completo a la evidencia. En el ambiente observado el 2026-10-23 no había entradas; no extrapolarlo a otros destinos.

Registrar por **Administrar fuentes** las autorizaciones vigentes aprobadas, con motivo y columnas mínimas, inicialmente inactivas; comprobar metadatos y activar explícitamente. No importar automáticamente listas del archivo ni sembrar fuentes con INSERT. El equipo prepara esquema/código/permisos durante una ventana controlada; hasta completar el registro mantiene la capacidad sin uso operativo. Un catálogo RA vacío deniega acceso y permite al administrador facultado registrar la primera fuente.

| Capacidad | Asignación y alcance |
|---|---|
| ADMINISTRAR_PERFILES_IMPORTACION | Requisito del contexto administrativo. |
| ADMINISTRAR_PROVEEDORES_IMPORTACION | Adicional para crear/configurar/publicar proveedores. |
| ADMINISTRAR_FUENTES_IMPORTACION | Adicional para descubrir candidatos y registrar/activar/revocar fuentes. No se deduce del permiso de proveedores. |

Asignar/revocar en la aplicación administrativa externa, bajo su autorización; el editor no concede permisos. No habilitar por GA_MANAGER_CONFIGURACION ni asignar masivamente. Confirmar concesión/revocación con cuenta y ambiente autorizados; esa aceptación externa sigue en 9.6.

El código actual compone metadatos, administración y consultas de proveedores desde `WebServiceImportacionCarpetas.PoliticaActual`, que llama a `MySqlFuenteImportacionRepository.CargarPolitica`. La política procede de RA; no se combina con la lista antigua ni hay fallback a Web.config. En índices se comparte mediante Lazy dentro de la petición. Fallo de lectura no concede acceso; revocación afecta nuevas peticiones y no cancela retroactivamente consultas iniciadas.

El constructor JSON de `PoliticaFuentesProveedor` sirve para materializar la lectura y los fixtures; no indica que se siga leyendo appSettings como autorización. `Doc110.Fuentes.TablasAdicionales` permanece como **límite adicional de nombres** del servidor: no sustituye una autorización RA activa. Sin excepciones, solo prefijo literal `imp_02`, sensible a mayúsculas; la excepción es por alias/nombre exacto. Ver [contrato de restricción](../Diseno/12-restriccion-tablas-fuentes.md).

En COLUMNA, el proveedor debe usar exactamente la columna empresarial autorizada y consulta con el ámbito servidor. En CONEXION, autorización explícita de exclusividad y columna empresarial vacía; para `imp_02_egresos` esa es la modalidad acordada. Autorizar solo las columnas necesarias, incluidas clave y propiedades. Referencias publicadas conservan IDs, contrato y huella; no se reescriben para agregar fuente_id.

## Recuperación y resultados inciertos

Si falta `ra_integracion_fuente_historial`, el alta/edición debe fallar antes de la primera escritura con `ESQUEMA_FUENTES_INCOMPLETO`. Comprobar las tres tablas y la columna de permiso; completar únicamente el DDL faltante en RA tras revisión del equipo. No repetir 07 completo sobre instalación parcial.

DDL MySQL 5.1 puede confirmar parcialmente por commits implícitos. Respaldar con el procedimiento del equipo antes de aplicarlo; no prometer rollback transaccional del esquema. No eliminar tablas, columnas, versiones, fuentes ni historial para reintentar instalación.

Ante fallo funcional, retirar el uso de la capacidad y corregir el catálogo/esquema preservando datos. No revertir a un consumidor que lea la lista antigua y reactive autorizaciones revocadas. Cualquier restauración de respaldo debe conciliar revisiones/revocaciones posteriores antes de habilitar; no activar automáticamente fuentes recuperadas. La recuperación preferida mantiene el código que exige RA y deniega si RA no está disponible.

Revocar una fuente detiene nuevas consultas y conserva proveedores/perfiles publicados e historial. No equivale a desactivar el proveedor ni el perfil; cada estado tiene su propia revisión. Ante timeout o pérdida de respuesta, consultar estado e historial por UI antes de una nueva escritura. No repetir una mutación automáticamente; si no se puede confirmar resultado, mantenerlo incierto hasta recuperar lectura autorizada.

## Evidencia y aceptación

| Evidencia | Resultado y límite |
|---|---|
| [Esquema 07, 2026-10-23](../Validacion/doc110-amp02-sql07-verificacion.md) | SELECT histórico: fuente/columna y permiso presentes, historial ausente. Falta confirmar esquema completo actual; no asumir que sigue igual ni que fue corregido. |
| [Conexión AMP-02](../Validacion/doc110-amp02-conexion-ui.md) | Build y componentes/interfaz con dobles PASS; no acredita transacciones reales del catálogo. |
| [Revalidación, 2026-10-26](../Validacion/doc110-revalidacion-proveedor-20261026.md) | 8 pruebas locales PASS y E2E autorizado 1 passed (23.0 s): proveedor visual, ensayo real, perfil versionado, desactivación/revalidación e historial conservado. No registra ni revoca fuentes desde UI. |

Repetición del recorrido de proveedores con ambiente/cuenta autorizados y clave unívoca, sin secretos en argumentos:

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --provider-profile --headed
```

La aceptación pendiente de fuentes exige esquema completo, facultad asignada y autorización específica para registrar/editar/activar/revocar una fuente descartable sin afectar consumidores existentes. Debe recorrer UI fuente → proveedor → perfil → ensayo, comprobar revisiones/historial y efectos de revocación, además de aislamiento/concurrencia y lecturas SELECT. La autorización previa del recorrido de proveedores no concede esas mutaciones del catálogo. No marcar 15.16 ni las tareas de persistencia real por el PASS de proveedores.

Para regresión local reutilizar el arnés .NET y las suites de cliente/editores existentes. Las pruebas DOC-88/SII estáticas complementan la revisión, no acreditan carga individual real. Esta guía y sus controles no ejecutan importaciones, no modifican fuentes DA ni despliegan al servidor principal.
