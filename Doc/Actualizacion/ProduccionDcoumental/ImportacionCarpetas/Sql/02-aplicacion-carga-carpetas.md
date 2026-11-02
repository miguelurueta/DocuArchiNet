# DOC-111: aplicación manual del esquema operativo

Corte vigente 2026-10-30: antes y durante la aceptación autorizada se verificó de nuevo el destino histórico mediante `MySqlImportacionCarpetasRepository.VerificarEsquema`, solo SELECT: PASS. El usuario autorizó específicamente ambiente/cuenta/recursos DOC-111; la corrida ya tiene plan 4/lote 2 y efectos parciales, por lo que las observaciones inferiores de tablas vacías son históricas. No se reaplicó SQL ni se ejecutó DDL. [Evidencia de aceptación parcial y compatibilidad](../Validacion/doc111-lote2-formato-nombre-20261030.md). Se acredita 13.11 sin afirmar aceptación completa.

Entregable: `02-carga-carpetas-mysql51.sql`. Estado al 2026-10-27: **aplicado manualmente por el usuario y estructura verificada mediante SELECT** en el ambiente configurado. Las seis tablas están vacías y ningún registro tiene IMPORTAR_CARPETAS habilitado. [Evidencia posterior a la aplicación](../Validacion/doc111-esquema-instalado-20261027.md). El agente no ejecutó DDL ni concedió permisos. Este procedimiento se conserva para instalaciones nuevas; no volver a ejecutar el archivo en el ambiente ya instalado.

## Revisión de compatibilidad MySQL 5.1

Revisión inicial del 2026-10-27: **compatible en revisión de sintaxis y estructura**. Se contrastó el archivo con el manual oficial 5.1 y se completó una inspección de metadatos en MySQL 5.1.37-community. Posteriormente el usuario lo aplicó manualmente y se verificó la estructura instalada con el repositorio, como indica la evidencia anterior.

- Seis tablas InnoDB, con IDs INT UNSIGNED AUTO_INCREMENT y PK, tipos BIGINT UNSIGNED/BINARY/LONGTEXT/DATETIME disponibles en 5.1. Los LONGTEXT no tienen DEFAULT ni se indexan. Los DATETIME no usan valores predeterminados mediante expresiones.
- Charset ASCII y collation ascii_bin explícitos; documentos Unicode mediante JSON escapado. Los 23 índices declarados, incluidos PK y únicos, se revisaron; la mayor clave compuesta ocupa 320 bytes de datos y no depende de ampliar límites de InnoDB.
- Diez FK con tamaño/signo compatibles y padres creados antes que hijos. Todas usan RESTRICT. Los identificadores SQL están dentro de 64 caracteres. No hay JSON nativo, utf8mb4, columnas generadas, índices funcionales ni sintaxis de migración reciente.
- `ALTER TABLE ... ADD COLUMN ... TINYINT UNSIGNED NOT NULL DEFAULT 0` es compatible con 5.1. La instalación es manual y el script no concede permisos.

La inspección SELECT confirma InnoDB disponible como motor predeterminado, ascii_bin presente y la dependencia `ra_integracion_perfil_version.version_id` como PK única de una columna INT UNSIGNED. Antes de la aplicación se comprobaron libres los nombres de destino. La inspección posterior confirma las seis tablas, la columna de permiso y los diez constraints ya instalados. Evidencia saneada actual: `tools/e2e/artifacts/doc111-schema-preflight.json` y `tools/e2e/artifacts/doc111-installed-schema.json`. Repetir los controles de precondición para otra instalación o si cambia el ambiente.

Fuentes oficiales: [sintaxis CREATE/ALTER de MySQL 5.1](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/sql-syntax.html), [InnoDB y restricciones de claves foráneas](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/storage-engines.html), [tipos de datos y restricciones de TEXT](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/data-types.html).

## Precondiciones

1. Contar con autorización específica del ambiente y una ventana de mantenimiento para la tabla de permisos. Identificar el esquema documental por la configuración servidor y seleccionar ese esquema con el cliente administrativo. El archivo no contiene `USE` ni nombres físicos de ambientes.
2. Conservar respaldo recuperable y estructura de la tabla de permisos y de la dependencia DOC-110. Comprobar que DOC-110 esté aplicado y que `ra_integracion_perfil_version.version_id` sea PK `INT UNSIGNED NOT NULL AUTO_INCREMENT` en InnoDB. No alterar las tablas documentales ni SII para acomodar este script.
3. Consultar los metadatos siguientes con la misma conexión que aplicará el DDL. Detenerse si falta la dependencia, si alguna tabla destino existe, si existe `IMPORTAR_CARPETAS`, o si los tipos/motor no coinciden. No usar `IF NOT EXISTS` para ocultar una instalación parcial. Verificar también que ningún nombre de constraint del script exista ya en el esquema.

```sql
SELECT VERSION() AS version_servidor;

SELECT TABLE_NAME, ENGINE, TABLE_COLLATION
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('ra_integracion_perfil_version',
    'remit_dest_interno_perfil_produccion', 'ra_integracion_plan',
    'ra_integracion_lote', 'ra_integracion_raiz', 'ra_integracion_archivo',
    'ra_integracion_intento', 'ra_integracion_evento');

SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE,
       COLUMN_DEFAULT, EXTRA, CHARACTER_SET_NAME, COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND ((TABLE_NAME = 'ra_integracion_perfil_version' AND COLUMN_NAME = 'version_id')
    OR (TABLE_NAME = 'remit_dest_interno_perfil_produccion'
      AND COLUMN_NAME = 'IMPORTAR_CARPETAS'));

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'ra_integracion_perfil_version'
ORDER BY INDEX_NAME, SEQ_IN_INDEX;

SELECT CONSTRAINT_NAME, TABLE_NAME, CONSTRAINT_TYPE
FROM information_schema.TABLE_CONSTRAINTS
WHERE CONSTRAINT_SCHEMA = DATABASE()
  AND CONSTRAINT_NAME LIKE 'fk\_integracion\_%';
```

La inspección inicial del 2026-10-27 observó MySQL 5.1.37-community, ambos padres InnoDB, versión `int(10) unsigned` incremental no nula, seis nombres operativos libres y permiso ausente. Esa observación antecede a la aplicación; los informes actuales ya muestran el esquema instalado. Esta observación no reemplaza las comprobaciones de la ventana de otra aplicación. Los índices de la dependencia deben verificarse nuevamente con la consulta anterior.

## Aplicación

Ejecutar en orden: plan, lote, raíz, archivo, intento, evento y extensión de permisos. Interrumpir en el primer error y conservar qué sentencias finalizaron. No usar una opción que continúe ante errores. MySQL 5.1 efectúa commit implícito de DDL; `ROLLBACK` no revierte una creación ni un `ALTER TABLE`. El cambio de permisos puede bloquear la tabla durante el `ALTER`.

Todos los documentos persistidos usan LONGTEXT ASCII/ascii_bin con JSON de Unicode escapado; la aplicación debe producir y comprobar el esquema cerrado. Las claves de entidades son INT UNSIGNED incrementales; los bytes son BIGINT UNSIGNED y su transporte HTTP es decimal en cadena. Los hashes son BINARY(32), no su representación hexadecimal de 64 caracteres. No configurar truncamiento silencioso como aceptación de entradas fuera de rango.

Los índices de texto usan ASCII y su mayor clave compuesta es de 320 bytes de datos, dentro del límite InnoDB de 767 bytes. No hay índices sobre LONGTEXT, JSON nativo, `utf8mb4`, columnas generadas ni dependencias de CHECK. Las referencias físicas usan INT UNSIGNED con `ON DELETE RESTRICT ON UPDATE RESTRICT`; no hay cascadas ni FK a SII. `intento_activo_id` y `recurso_id` son referencias lógicas que se validan dentro del lote. Las FK independientes de intento no prueban pertenencia al mismo lote.

`IMPORTAR_CARPETAS` queda en cero para todos los registros existentes. El archivo no concede facultades a ninguna cuenta. Configurar límites operativos, autorizar cuentas y habilitar el flujo son pasos posteriores, con sus propias comprobaciones. La existencia del esquema no habilita la carga.

## Controles posteriores de solo lectura

Volver a consultar TABLES y COLUMNS como arriba. Deben aparecer exactamente las seis tablas nuevas InnoDB y la columna `IMPORTAR_CARPETAS TINYINT UNSIGNED NOT NULL DEFAULT 0`. Comparar **todas** las columnas, nulabilidad, charset/collation e índices con el DDL, no solo el número de tablas:

```sql
SELECT TABLE_NAME, ORDINAL_POSITION, COLUMN_NAME, COLUMN_TYPE,
       IS_NULLABLE, COLUMN_DEFAULT, EXTRA, CHARACTER_SET_NAME, COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('ra_integracion_plan', 'ra_integracion_lote',
    'ra_integracion_raiz', 'ra_integracion_archivo', 'ra_integracion_intento', 'ra_integracion_evento')
ORDER BY TABLE_NAME, ORDINAL_POSITION;

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME, SUB_PART
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME IN ('ra_integracion_plan', 'ra_integracion_lote',
    'ra_integracion_raiz', 'ra_integracion_archivo', 'ra_integracion_intento', 'ra_integracion_evento')
ORDER BY TABLE_NAME, INDEX_NAME, SEQ_IN_INDEX;

SELECT k.TABLE_NAME, k.COLUMN_NAME, k.REFERENCED_TABLE_NAME,
       k.REFERENCED_COLUMN_NAME, r.UPDATE_RULE, r.DELETE_RULE
FROM information_schema.KEY_COLUMN_USAGE k
JOIN information_schema.REFERENTIAL_CONSTRAINTS r
  ON r.CONSTRAINT_SCHEMA = k.CONSTRAINT_SCHEMA
 AND r.CONSTRAINT_NAME = k.CONSTRAINT_NAME
WHERE k.TABLE_SCHEMA = DATABASE()
  AND k.TABLE_NAME IN ('ra_integracion_plan', 'ra_integracion_lote',
    'ra_integracion_raiz', 'ra_integracion_archivo', 'ra_integracion_intento', 'ra_integracion_evento')
ORDER BY k.TABLE_NAME, k.CONSTRAINT_NAME;

SELECT 'plan' AS entidad, COUNT(*) AS filas FROM ra_integracion_plan
UNION ALL SELECT 'lote', COUNT(*) FROM ra_integracion_lote
UNION ALL SELECT 'raiz', COUNT(*) FROM ra_integracion_raiz
UNION ALL SELECT 'archivo', COUNT(*) FROM ra_integracion_archivo
UNION ALL SELECT 'intento', COUNT(*) FROM ra_integracion_intento
UNION ALL SELECT 'evento', COUNT(*) FROM ra_integracion_evento;

SELECT COUNT(*) AS permisos_fuera_de_cero
FROM remit_dest_interno_perfil_produccion
WHERE IMPORTAR_CARPETAS IS NULL OR IMPORTAR_CARPETAS <> 0;
```

Antes de cualquier prueba operativa, las seis cuentas y `permisos_fuera_de_cero` deben ser cero. Guardar evidencia saneada de metadatos y resultados. Los controles de unicidad, carrera, rollback de inventario e idempotencia requieren después pruebas de integración autorizadas; la revisión del archivo no las acredita.

## Interrupción o esquema parcial

Mantener la capacidad operativa deshabilitada. Obtener nuevamente metadatos y determinar el prefijo de sentencias efectivamente aplicado; contrastar cada objeto con el archivo. Si son idénticos y permanecen vacíos, preparar y revisar un script de continuación que contenga únicamente las sentencias faltantes, bajo autorización del operador. No volver a ejecutar íntegramente ni ignorar duplicados.

Si hay objetos incompatibles, filas inesperadas o permisos distintos de cero, detenerse y conservar evidencia y respaldos. No renombrar, borrar, truncar, modificar permisos ni restaurar automáticamente. La recuperación del respaldo o una corrección de esquema exige un procedimiento concreto aprobado para ese ambiente. Esta entrega no contiene un rollback destructivo.

## Controles pendientes de compatibilidad de creación (solo SELECT)

Preparados el 2026-10-28. El subconjunto de motor/modo SQL, columnas relevantes de expediente, índices, cardinalidad de ruta y clasificación se ejecutó mediante el inspector existente, reutilizando el destino de prueba fijado por DOC-111 y los parámetros del perfil 24/versión 38. Ver `Validacion/doc111-compatibilidad-creacion-select-20261028.md`; no se ejecutó literalmente todo el bloque manual siguiente. La evidencia histórica del 2026-10-19 no sustituye ese contraste actual. En otras ejecuciones, los parámetros `@empresa`, `@instrumento` y `@area` deben enlazarse desde recursos autorizados mediante el cliente de consultas; no son variables inicializadas por este documento. No ejecutar un bloque parametrizado sin enlazar sus referencias.

```sql
SELECT DATABASE() AS base_actual, VERSION() AS version_motor,
       @@SESSION.sql_mode AS modo_sql;

SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE,
       COLUMN_DEFAULT, CHARACTER_SET_NAME, COLLATION_NAME
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE()
  AND ((TABLE_NAME='expediente_archivo' AND COLUMN_NAME IN
       ('ID_EXPEDIENTE','ID_EMPRESA_EXPEDIENTE','ID_USUARIO_GESTION',
        'VOLUMEN_EXPEDIENTE','estado_expediente_electronico'))
    OR (TABLE_NAME='registro_organigrama' AND COLUMN_NAME IN
       ('ID_ORGANIGRAMA','NOMBRE_ORGANIGRAMA','EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA')))
ORDER BY TABLE_NAME,ORDINAL_POSITION;

SELECT TABLE_NAME, INDEX_NAME, NON_UNIQUE, SEQ_IN_INDEX, COLUMN_NAME
FROM information_schema.STATISTICS
WHERE TABLE_SCHEMA=DATABASE()
  AND TABLE_NAME IN ('registro_organigrama','areas_depart_radicacion',
      'ra_de_fondo_documental','ra_de_tipos_ciclos_archivo','ra_ruta_expediente')
ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX;

SELECT COUNT(*) AS configuraciones_ruta_expediente FROM ra_ruta_expediente;

SELECT COUNT(*) AS nombres_organigrama_ambiguos
FROM (
  SELECT NOMBRE_ORGANIGRAMA
  FROM registro_organigrama
  WHERE EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=@empresa
  GROUP BY NOMBRE_ORGANIGRAMA
  HAVING COUNT(*)>1
) grupos;

SELECT i.id_instrumento,i.registro_organigrama_ID_ORGANIGRAMA AS organigrama,
       a.Codigo_Area AS area
FROM ra_registro_instrumento_archivistico i
JOIN registro_organigrama o ON o.ID_ORGANIGRAMA=i.registro_organigrama_ID_ORGANIGRAMA
JOIN areas_depart_radicacion a ON a.Registro_Organigrama_Id_Organigrama=o.ID_ORGANIGRAMA
WHERE o.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=@empresa
  AND i.id_instrumento=@instrumento AND a.Codigo_Area=@area
LIMIT 2;
```

Estos controles no insertan expedientes, no modifican modo SQL/configuración, no abren rutas ni imprimen credenciales/cadenas de conexión. Sus resultados no habilitan por sí solos el adaptador: quedan pendientes la correspondencia por IDs, estabilidad de las búsquedas legacy y correlación/verificación de efectos. No corregir automáticamente datos, nombres, índices, volumen o modo SQL para forzar la llamada. Conservar evidencia saneada y separar metadatos actuales de la exploración histórica.
