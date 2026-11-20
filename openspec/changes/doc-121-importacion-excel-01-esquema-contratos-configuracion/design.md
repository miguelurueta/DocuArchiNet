<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Context

DOC-121 entrega la fase 01 contractual de importación XLS/XLSX a `imp_02_`. Lectura, endpoints y UI pertenecen a fases 02–07. Perfil legacy-webforms-vb, Web Forms/VB.NET, Framework 4.6.1 y MySQL 5.1.37 histórico. RA/DA son destinos lógicos que pueden ser bases distintas.

## Goals / Non-Goals

Inventario, contrato de datos/request-response/configuración, SQL manual y revisión estática. Flujo acordado: archivo completo, preparación integral, confirmación transaccional y resultado por ID. Fuera de alcance: CSV, pausa por fila, subida reanudable por bytes, workers, upsert, omitir duplicados, cambios SII/carpetas y aplicar instalación automáticamente.

## Decisions

### D-01 — Catálogos y compatibilidad (RQ-01)

Conservar los tres catálogos `imp01_*`, sus PK/FK, nombres y orden históricos. Agregar al final de `imp01_plantillaimp` CAMPO_UNICO VARCHAR(64) ASCII nullable y REVISION_CARGA INT UNSIGNED NOT NULL DEFAULT 1. Plantillas sin clave siguen siendo legibles para SII; requieren adecuación antes de escribir desde web. No modificar mapeos ni opciones históricas durante instalación.

Inventario y contratos en `Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md`. Los consumidores inspeccionados seleccionan columnas explícitas. Los metadatos del 11 de noviembre no son comprobación actual; no reducir BENEFICIARIO a su longitud histórica después de la ampliación informada por el usuario.

### D-02 — Instalación incremental manual (RQ-02)

Paquete `Doc/Actualizacion/Importacion/Sql/`: 00 preflight SELECT en RA y DA; 01 permiso RA; 02 extensión DA; 03 ejecución DA; 04 adecuación de clave por tabla; 05 concesiones DA; 06 postflight SELECT. DDL de una sola aplicación, después de comprobar ausencia/compatibilidad; no ocultar incompatibilidades mediante IF NOT EXISTS. Un paso ya aplicado se verifica y omite. Un esquema parcial requiere otro archivo revisado.

04 genera una propuesta de ALTER desde metadatos reales, preservando tipo/charset/collation. El usuario la guarda como nuevo SQL específico y la revisa antes de aplicar; controles de datos, aislamiento e índices son precondiciones independientes. 05 es una plantilla administrativa a completar localmente con base/cuenta/host existentes; no contiene contraseñas. Instalación inicial PENDIENTE_EJECUCION_MANUAL. Confirmación y SELECT posterior se acreditan aparte.

### D-03 — Registro operativo (RQ-03)

Solo una tabla nueva `imp01_carga_ejecucion`, InnoDB, en la DA del destino; ID CHAR(36) ASCII binario generado por servidor, sin FK hacia legacy/otras bases. Usuario/ámbito VARCHAR(128) ASCII; nombre original UTF8 descriptivo, referencia privada de 120 caracteres, bytes/hash BINARY(32). Configuración canónica ASCII escapada LONGTEXT, huellas BINARY(32) y revisión de plantilla. Preparación puede ser NULL en RECIBIDA; no usar CHECK ignorados, JSON nativo o triggers.

RECIBIDA/PREPARADA/CONFIRMADA/FALLIDA son estados validados por aplicación. PREPARADA exige configuración y huellas; CONFIRMADA exige conteos/fecha y commit coherentes. FALLIDA solo tras fallo conocido y rollback comprobado. Un acuse perdido no degrada el commit. Índices ámbito/usuario/fecha, plantilla/fecha y ámbito/destino/estado. Resultado histórico independiente de la configuración futura.

### D-04 — Autoridad y configuración (RQ-04)

Permiso RA ADMINISTRAR_CARGAS_IMPORTACION TINYINT UNSIGNED NOT NULL DEFAULT 0, sin habilitar usuarios en instalación. La fase 02 reconstruye usuario/empresa/perfil de permisos RA; no exige administrar perfiles ni reutiliza su resolvedor como autorización del cargador. Protección de solicitud con propósito propio: sesión/ámbito/origen/tipo/tamaño. Reutilizar patrones existentes sin compartir automáticamente su token.

MaxFilasDatos=10000 y MaxArchivoMB=10; MB=1000000 bytes. Enteros positivos, independientes y de servidor, visibles al operador. Comprobar bytes al recibir, filas al preparar y vigencia antes de confirmar. Vinculación protegida ámbito/DA/ID_XMLCONEXION/cuenta dedicada; el cliente no escoge host/base/credenciales. Privilegios acordados SELECT/INSERT/UPDATE/DELETE/CREATE/ALTER/INDEX/CREATE TEMPORARY TABLES sobre esa DA, sin DROP/globales/GRANT OPTION. La concesión de base alcanza técnicamente todas sus tablas; la aplicación limita destinos elegibles.

### D-05 — Clave y fuente de índices (RQ-05)

Tablas nuevas `imp_02_<nombre>` e identificadores ASCII válidos de hasta 64 caracteres. PK técnica histórica opcional. Clave de negocio requerida, sin NULL/vacíos y UNIQUE completo, no un prefijo. Fuente exclusiva: clave; compartida: empresa+clave según autorización. Duplicados comparados con tipo/collation MySQL reales. No reducir longitudes, limpiar datos o cambiar aislamiento en instalación.

Tipos nuevos VARCHAR(250), INT, DECIMAL(18,2), DATE según selección; existentes conservan capacidad real. Contrastar bytes de índice, nulabilidad/defaults/comentarios e índices equivalentes. El generador no acredita datos. Después de confirmar datos, registrar/activar fuente y publicar proveedor por servicios existentes; no conceder autorizaciones mediante semillas SQL.

### D-06 — HTTP y preparación (RQ-06)

Contrato en `Diseno/01-contratos-configuracion-doc121.md`: contexto, listar/consultar/guardar plantilla, recibir, preparar, confirmar y consultar resultado. Son operaciones previstas, no endpoints disponibles. IDs/referencias de servidor, modos CREAR/AGREGAR/REEMPLAZAR; archivo multipart y resto JSON acotado. Encabezado en primera fila; contar filas de datos no vacías de la hoja elegida.

Preparar valida archivo completo, mapeos, tipos/rangos/charset, nulabilidad, índice, duplicados y modo; muestra acotada y resumen completo. Solo publica PREPARADA sin incidencias, fijando archivo/configuración/esquema. Efectos limitados al staging, registro y objetos temporales; no cambiar filas destino. Cambios posteriores, incluso de plantilla antigua sin incremento de revisión, producen conflicto antes de escribir. No transportar el conjunto de filas desde el navegador como autoridad de confirmación.

### D-07 — Confirmación y recuperación (RQ-07)

Confirmar reautoriza y verifica identidad/contenido; si ya está CONFIRMADA devuelve el histórico. Para la ejecución sin confirmar, bloqueo cooperativo por DA física+tabla, transacción InnoDB y bloqueo/relectura de registro/configuración. Releer una confirmación concurrente ganadora antes de escribir. El bloqueo se comparte entre ámbitos sobre la misma tabla; UNIQUE protege frente a escritores externos.

AGREGAR rechaza conflictos e inserta por parámetros en agrupaciones acotadas. REEMPLAZAR usa DELETE filtrado+INSERT en una transacción, conservando estructura. CONFIRMADA y datos se confirman juntos. Fallo conocido: rollback completo y registro seguro posterior de FALLIDA sin sobrescribir otro ganador. Incertidumbre: consulta por ID, sin retry automático o liberación por tiempo.

CREAR es DDL separado, con comentario `ImportacionExcel:<idCarga>` y huella para verificar tabla propia ante acuse perdido. CREATE no tiene rollback DML: puede quedar tabla propia vacía; un nombre ajeno es conflicto, nunca DROP/recreación automática. Agrupaciones INSERT no son commits parciales ni puntos de pausa. Cerrar navegador no autoriza repetir; consulta y confirmación son distintas.

### D-08 — Evidencia y entrega (RQ-08)

Revisión estática SQL/MySQL 5.1, contratos y trazabilidad; no simular aplicación. E2E/build de producto no aplican a documentos/SQL sin compilables. Usar herramientas existentes de gobierno y matriz con SHA/hashes/comandos/resultado/límites. La fase contractual puede entregarse con instalación sin ejecutar; las fases dependientes requieren confirmación y SELECT posterior. Aceptación de capacidad/concurrencia/rollback corresponde a fases ejecutables.

## Risks / Trade-offs

Los prompts elegidos no agregan versionado de datos de fuente ni bloqueo de reemplazo por importaciones activas. Una versión de perfil no congela las filas de la fuente; la regresión posterior debe registrar el comportamiento de valores fijados/diferidos sin prometer otra política. No se cambia Web.config ni se instalan librerías aquí.

La reversión de código conserva objetos aditivos y no reactiva automáticamente permisos. Cualquier reversión estructural exige otro SQL revisado; no entregar DROP o limpieza como desinstalación automática. El operador determina respaldo/ventana de sus DDL y concesiones.
