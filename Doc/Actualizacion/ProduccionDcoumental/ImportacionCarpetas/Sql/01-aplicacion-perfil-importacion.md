Estado al cierre, 2026-10-27: SQL 01 usa claves incrementales para instalaciones nuevas. La [migración del destino GUID](../Validacion/doc110-correccion-incrementales.md) y la [instalación nueva aislada autorizada](../Validacion/doc110-instalacion-nueva-aislada-20261027.md) cuentan con evidencia independiente. La instalación nueva fue verificada en doc110_instalacion_prueba; no repetir SQL 01 en un destino existente. Las referencias inferiores a instalación o pruebas por realizar corresponden al procedimiento histórico inicial, no a pendientes actuales. El archivo SQL conserva exclusivamente DDL por instrucción expresa del usuario.

# Aplicación manual y recuperación — SQL 01

Estado histórico del esquema GUID (2026-10-20): el usuario confirmó la aplicación manual del SQL. La comprobación SELECT posterior confirma las tres tablas propias InnoDB, los dos nombres de FK y la columna del permiso NOT NULL DEFAULT 0 en `docuarchi`, servidor MySQL 5.1.37-community. Evidencia: [metadatos posteriores a la aplicación](../Validacion/doc110-mysql51-compatibilidad.json). No volver a ejecutar el archivo en este destino.

**Cambio solicitado por el usuario:** SQL 01 contiene únicamente tres CREATE TABLE y un ALTER TABLE. Se retiraron consultas SELECT, bloques de validación e instrucciones de revisión del archivo ejecutable. Las referencias a bloques A/B/C/D y controles de la versión anterior que aparezcan en documentación histórica no describen el archivo vigente. No reintroducirlos por aplicación automática de prompts anteriores.

El DDL fue aplicado por el usuario, no por el agente. La comprobación no acredita equivalencia exhaustiva de columnas/índices/acciones FK, concesión del permiso ni funcionamiento de la aplicación. El refinamiento completo y las pruebas funcionales conservan su estado pendiente.

## Diccionario físico y decisiones de materialización

Nombres actualizados por instrucción del usuario: `ra_integracion_perfil`, `ra_integracion_perfil_version`, `ra_integracion_perfil_regla`. SQL, FK, índices y consultas de control sincronizados. Una comprobación SELECT posterior confirmó que estos tres nombres están libres en el destino inspeccionado (`evidencia-nombres-ra.json` de DOC-110). Las evidencias anteriores con nombres sin prefijo se conservan como históricas; no son objetos que se deban crear o renombrar.

La sección 2 de `Diseno/01-modelo-datos.md` es la fuente de las tres tablas. El script especifica cada columna, nulabilidad, PK, índices y dos FK propias. No incluye seis tablas operativas ni FK a catálogos legacy.

| Detalle materializado | Motivo / comprobación |
|---|---|
| INT UNSIGNED para IDs/referencias; ASCII/ascii_bin para documentos escapados | Tres PK AUTO_INCREMENT. JSON sin bytes Unicode directos; validación de contenido en aplicación. |
| UTF8/utf8_bin para nombres/descripción | UTF8 de tres bytes del diseño, collation explícita sin imponer equivalencia lingüística. No sustituir caracteres no representables. |
| `activo` sin default implícito | Servicio debe proporcionar el estado expresamente. Disponibilidad efectiva exige publicación válida y permisos. |
| `revision`, `numero`, `esquema_config`, fechas y actores sin defaults artificiales | Servicio debe proporcionarlos expresamente; no usar reloj/defaults de BD para simular contrato. |
| UQ ámbito/código: 192 bytes ASCII; UQ versión/destino: 200 bytes ASCII | Longitudes declaradas inferiores al límite histórico InnoDB de 767 bytes por clave; no se indexan LONGTEXT. |
| Referencia publicada sin FK circular | Validar pertenencia de versión al perfil dentro de la transacción, como permite el modelo común. |
| Nuevos estados/políticas en VARCHAR y configuración LONGTEXT | Dominio cerrado validado por servicio. No habilita valores arbitrarios ni resuelve decisiones operativas abiertas. |
| Permiso nuevo TINYINT UNSIGNED NOT NULL DEFAULT 0 | Solo 1 concede; otros valores deniegan. La tabla existente usa columnas int para otros permisos: no cambiarlas. |

Evidencia física de permisos: `openspec/changes/doc-110-produccion-importacion-carpeta-perfil/evidencia-esquema-permiso.json`, 2026-10-20. Tabla InnoDB/latin1, PK `id_perfil_produccion`; índice de usuario no único. 78 filas, 78 usuarios distintos, cero grupos duplicados en esa inspección. El permiso nuevo no existía y los tres nombres de tablas estaban libres. Es evidencia del ambiente consultado, no garantía de otros despliegues.

## Secuencia

La inspección previa de `Exploracion/09-mapa-bases-verificado.md` identificó el destino RA y documentó la ausencia inicial de las tablas. Ese estado fue superado por la aplicación manual confirmada. La secuencia siguiente queda como referencia para otros despliegues; no es una instrucción de reaplicación.

La aplicación usa varias bases: consultar `Diseno/08-origenes-datos.md`. Identificar destino por bloque; no suponer que la base seleccionada contiene todos los catálogos. El script no crea objetos en DA ni define equivalencia de gabinetes RA/DA. El contrato operativo de `gabinete_id` debe caracterizarse antes de aceptar definitivamente ese vínculo.

1. Identificar la base documental RA mediante el contexto de módulos autorizado. No elegir por nombre supuesto ni por la conexión sin prefijo. Confirmar respaldo y procedimiento de cambio del equipo.
2. Confirmar que el destino corresponde a la instalación prevista. El SQL vigente no contiene bloques de consulta ni validación.
3. En un destino nuevo, ejecutar los CREATE en orden: perfil, versión, regla. Registrar resultado por sentencia. Si ya existe alguna tabla, aplicar la recuperación de abajo antes de continuar.
4. Aplicar el ALTER únicamente cuando la columna esté ausente, en la ventana definida por el equipo. No ejecutar ADD otra vez.
5. Conservar evidencia de resultados por sentencia. SQL 01 no introduce usuarios ni asignaciones de permisos.
6. Registrar esquema/fecha/resultados de forma saneada. La aplicación del DDL no sustituye compilación, pruebas de repositorio, concurrencia ni E2E autorizadas.

El archivo vigente no contiene comentarios PAUSA ni bloques A/B/C/D. No usar una opción de cliente que continúe ante errores.

## Repetición y recuperación parcial

No se usa `CREATE TABLE IF NOT EXISTS`: ocultaría estructura incompatible. No hay procedimientos, SQL dinámico, DROP, DELETE, UPDATE o concesiones de permisos.

| Situación | Acción manual |
|---|---|
| Falló primer CREATE | Conservar error saneado; revisar precondición/definición antes de reintentar. |
| Existe perfil y falta versión/regla | Comparar definición completa de perfil; solo si coincide, ejecutar sentencias ausentes en orden. |
| Existe una tabla con estructura diferente | Detener ese bloque y preparar ajuste específico revisable. No borrar/recrear ni dar por válida su existencia. |
| Tablas completas y ALTER falló | Conservar tablas creadas; revisar causa del ALTER. La aplicación debe degradar la capacidad nueva sin romper login. |
| Columna de permiso ya compatible | Omitir ADD; conservar asignaciones existentes. No reponer valores a cero. |
| FK/índices distintos | No declarar instalación completa. Corregir mediante entrega revisada, sin modificar SII. |
| Reversión de la aplicación | Deshabilitar entrada nueva según procedimiento autorizado y conservar datos/historial. No eliminar tablas/columna como rollback automático. |

DDL provoca commits implícitos: no prometer reversión global usando una transacción. La incertidumbre sobre una sentencia se resuelve inspeccionando metadatos antes de reenviarla.

## Validación realizada y por realizar

Realizada: contraste documental con modelo común y metadatos de permisos; sintaxis revisada para evitar características posteriores a MySQL 5.1. No equivale a ejecutar el script.

Por realizar en ambiente autorizado: aplicar sobre instalación limpia y parcial, comparar esquema, verificar defecto cero, probar lectura/guardado/publicación y concurrencia mediante implementación real. No ejecutar DDL en la base consultada como parte de esta investigación.
