# Revisión de compatibilidad MySQL 5.1 — SQL 01

Resultado: **sin incompatibilidades identificadas en revisión estática y metadatos del servidor MySQL 5.1.37-community**. No se ejecutó ni preparó el DDL; no es una prueba real de creación/ALTER. No fue necesario modificar el archivo.

Actualización posterior (2026-10-20): el usuario confirmó que aplicó manualmente el SQL. El JSON enlazado se renovó a las 14:43:29 UTC y ahora confirma las tres tablas InnoDB/ascii_bin, dos nombres de FK y `ADMINISTRAR_PERFILES_IMPORTACION` como `tinyint(3) unsigned NOT NULL DEFAULT 0`. “DDL not executed” en la evidencia delimita la auditoría SELECT del agente, no niega la aplicación realizada por el usuario. La tabla de diagnóstico y sus menciones a objetos ausentes describen la revisión previa. No volver a ejecutar el script en el destino inspeccionado.

Archivo: `Sql/01-perfil-importacion-mysql51.sql`.

SHA-256 revisado: `a7dae051804e4be98de72e15beb5a600e87fc4397110730b2327ef235c479afb`.

Evidencia SELECT de 2026-10-20: [metadatos saneados](doc110-mysql51-compatibilidad.json). Los controles están en esta evidencia, no se reincorporaron al SQL simplificado aprobado.

## Diagnóstico por componente

| Elemento revisado | Resultado |
|---|---|
| Tres CREATE TABLE y un ALTER TABLE ADD COLUMN | Sintaxis correspondiente a MySQL 5.1; no cláusulas de versiones posteriores. |
| Motor | InnoDB disponible y DEFAULT en servidor consultado; las tres tablas nuevas lo especifican. |
| Tipos | CHAR, VARCHAR, TEXT, LONGTEXT, BINARY(32), TINYINT/SMALLINT/INT UNSIGNED y DATETIME compatibles. No JSON nativo, utf8mb4 ni columnas generadas. |
| Defaults | Literal de modo en VARCHAR y 0 en nuevo permiso; ningún default de TEXT/LONGTEXT ni DATETIME CURRENT_TIMESTAMP. |
| Charset/collation | Servidor confirma ascii_bin/ascii y utf8_bin/utf8. MAXLEN observado: 1 y 3 bytes. |
| PK y FK | IDs relacionados son CHAR(32) ASCII/ascii_bin NOT NULL en ambos lados. Referencias apuntan a PK; índices hijos comienzan por columna FK. |
| Acciones referenciales | ON DELETE RESTRICT / ON UPDATE RESTRICT. Orden de creación: perfil → versión → regla. |
| Ámbito de relaciones | Dos FK únicamente entre tablas propias; ninguna hacia gabinete/catálogos externos. No FK circular desde cabecera a versión. |
| Nombres | Identificadores inspeccionados dentro del límite de 64 caracteres; máximo observado de tablas/índices/constraints: 39. Nombres FK libres en esquema consultado. |
| Columnas largas | TEXT/LONGTEXT sin índices; no se requieren prefijos de índice sobre ellas. Definiciones de fila acotadas, sin columnas VARCHAR desmesuradas. |
| ALTER de permisos | Tabla existente InnoDB/latin1_swedish_ci. Nueva columna numérica no cambia charset del resto. Campo nuevo ausente; NOT NULL DEFAULT 0 no concede automáticamente capacidades. |
| Modo estricto | Servidor: STRICT_TRANS_TABLES, NO_AUTO_CREATE_USER, NO_ENGINE_SUBSTITUTION. No se modifica. La aplicación debe enviar campos NOT NULL sin default al insertar; esto no es un error del CREATE. |
| Repetición | No es script idempotente: CREATE/ADD de objetos ya existentes fallaría. En esta inspección no existen los objetos nuevos. No añadir IF NOT EXISTS ni suprimir errores por inferencia. |

La compatibilidad de sintaxis, reglas de FK y tipos se contrastó con el [manual oficial MySQL 5.1](https://downloads.mysql.com/docs/refman-5.1-en.pdf), especialmente CREATE TABLE, ALTER TABLE, tipos y restricciones InnoDB. Para claves de texto relacionadas exige charset/collation compatibles e índices adecuados; el script los declara.

## Longitud declarada de claves

Cálculos sobre las columnas del SQL, con ASCII de un byte confirmado por el servidor. No representan tamaño total del árbol B-tree.

| Tabla / clave | Bytes declarados |
|---|---:|
| Perfil PK | 32 |
| Perfil UQ ámbito/código | 128 + 64 = 192 |
| Perfil listado ámbito/activo/perfil | 128 + 1 + 32 = 161 |
| Versión PK | 32 |
| Versión UQ perfil/número | 32 + 4 = 36 |
| Versión índice perfil/estado/número | 32 + 40 + 4 = 76 |
| Regla PK | 32 |
| Regla UQ versión/tipo/referencia | 32 + 40 + 128 = 200 |
| Regla índice versión/orden | 32 + 4 = 36 |

Incluso la clave compuesta mayor queda por debajo del umbral histórico de 767 bytes documentado para restricciones de claves InnoDB, sin depender de configuraciones de índices largos de versiones posteriores. Referencia de época: [MySQL Bug #53475, límite en MySQL 5.1](https://bugs.mysql.com/bug.php?id=53475).

## Límites del resultado

El usuario reportó la aplicación; el agente no observó resultados individuales de las sentencias, permisos CREATE/ALTER, bloqueo/duración del ALTER ni escritura desde la aplicación. La comprobación posterior no compara todas las columnas, índices ni destinos/acciones FK. El agente solo consultó metadatos; implementación, autorización externa y pruebas funcionales siguen pendientes.
