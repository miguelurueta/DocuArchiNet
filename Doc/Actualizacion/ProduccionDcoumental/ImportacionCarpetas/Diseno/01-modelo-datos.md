# Diseño 01 — Modelo de datos propuesto

Estado: diseño técnico para las entregas 02 y 07-A. No es DDL aplicado ni aprobación de las decisiones funcionales pendientes. Leer [decisiones y pendientes](04-decisiones-y-pendientes.md) antes de implementarlo.

## 1. Evidencia y convenciones

La evidencia `Exploracion/02-evidencia-esquema-perfil.json` corresponde a MySQL 5.1.37 y muestra collations mixtas: `expediente_archivo` usa latin1_swedish_ci; `detalle_gabienete`, utf8_general_ci. `system1.id_gabinete` y `detalle_gabienete.id_detalle_gabinete` son INT UNSIGNED. `expediente_archivo.ID_EXPEDIENTE` es INT UNSIGNED y su PK es compuesta con `CODIGO_UNICO`. No crear una FK solo a ID_EXPEDIENTE suponiendo que constituye toda su identidad. No se inspeccionaron nuevamente datos operativos.

Decisiones técnicas propuestas:

- Todas las tablas nuevas InnoDB. Nombres siguientes son nuevos y deben verificarse libres antes de entregar el SQL.
- `ID`: INT UNSIGNED; solo las PK usan AUTO_INCREMENT de MySQL. Referencias del mismo tipo sin autoincremento. Transporte como cadena decimal canónica entre 1 y 4294967295; ausencia opcional null, nunca cero. No es una secuencia por empresa ni garantiza números sin huecos. La autorización se verifica por ámbito en cada operación.
- `UID`: CHAR(32) ASCII/ascii_bin reservado para referencias opacas de staging y tokens técnicos; no para PK ni FK de entidades.
- `HASH`: BINARY(32), SHA-256 calculado en aplicación; no depender de funciones SQL de otra versión. Las comparaciones de identidad de petición verifican también su representación canónica cuando corresponda, no solo la huella.
- `CODE`: VARCHAR(40) CHARACTER SET ascii COLLATE ascii_bin para estados/modos cerrados. Sin ENUM ni CHECK como única validación: Service valida transiciones.
- `DOC`: LONGTEXT CHARACTER SET ascii COLLATE ascii_bin con documento JSON de esquema versionado, serialización determinista y Unicode escapado. No es JSON nativo MySQL. Rechazar claves repetidas, propiedades no permitidas, números fuera de rango y tamaño/profundidad excedidos. No se admiten expresiones ejecutables ni nombres SQL libres. Es una representación de configuración/evidencia, no un catálogo paralelo.
- Textos cortos visibles nuevos: UTF8 de MySQL 5.1 (tres bytes) con validación estricta, sin reemplazo silencioso de caracteres. Nombres/rutas originales completos se conservan en DOC escapado; su conversión a destinos latin1/utf8 legacy se valida antes de ejecutar. Si un nombre no es representable en el destino, reportar incidencia: no quitar caracteres silenciosamente.
- DATETIME contiene UTC explícito desde aplicación. No asumir fracciones de segundo ni múltiples defaults automáticos de TIMESTAMP. Revisiones INT UNSIGNED, incrementadas en CAS; desbordamiento se rechaza.
- Salvo marcado `NULL`, los campos son NOT NULL. `creado_utc`, `actualizado_utc` y `revision` se incluyen donde se indica; no se usan para sustituir identificadores o control de exclusión.
- Las referencias legacy se guardan como referencias lógicas verificadas: `ambito_ref`, `usuario_ref`, `nivel_ref` CHAR/VARCHAR(128) ASCII con formato canónico validado del identificador real. No inventar IDs ni permitir consulta libre a un esquema indicado por el cliente. Gabinete usa el INT UNSIGNED observado. Mapeo de ámbito/identidad debe concretarse contra autenticación real antes de SQL final.
- FK físicas solo entre tablas nuevas compatibles en el mismo esquema: ON DELETE RESTRICT, sin borrado en cascada de historial. Referencias a catálogos legacy se validan en servicios; no modificar sus índices para imponer FK nuevas. Un borrador incompleto permite referencias NULL, pero nunca valores inválidos.

## 2. Configuración — SQL 01, propietario prompt 02

Materialización documental: [SQL 01](../Sql/01-perfil-importacion-mysql51.sql) y [procedimiento/diccionario físico](../Sql/01-aplicacion-perfil-importacion.md). Preparados para revisión; no aplicados. Ámbito DOC-110: empresa documental del usuario en el origen RA. Se verificaron nombres libres y esquema de permisos mediante SELECT el 2026-10-20, sin acreditar otros ambientes.

### `ra_integracion_perfil`

| Columna | Tipo | Semántica |
|---|---|---|
| perfil_id | ID PK | Identidad estable. |
| ambito_ref | VARCHAR(128) ASCII | Ámbito autorizado de configuración. |
| codigo | VARCHAR(64) ASCII | Código estable, normalizado, no nombre visible. |
| nombre | VARCHAR(160) UTF8 | Identificación administrativa del perfil. |
| descripcion | TEXT UTF8 NULL | Ayuda de uso. |
| activo | TINYINT UNSIGNED | 0/1; desactivar no borra versiones. |
| version_publicada_id | ID NULL | Referencia a versión publicada habitual. |
| creado_por, actualizado_por | VARCHAR(128) ASCII | Identidades canónicas de auditoría. |
| creado_utc, actualizado_utc | DATETIME | Auditoría. |
| revision | INT UNSIGNED | Concurrencia de edición administrativa. |

UQ `(ambito_ref,codigo)`. Índice de listado `(ambito_ref,activo,perfil_id)`. La referencia a versión publicada se valida con pertenencia al perfil en transacción; para evitar ciclo DDL puede mantenerse lógica, sin FK física en la cabecera. La FK versión→perfil sí es física.

### `ra_integracion_perfil_version`

| Columna | Tipo | Semántica |
|---|---|---|
| version_id | ID PK | Versión estable. |
| perfil_id | ID FK | Perfil propietario. |
| numero | INT UNSIGNED | Número por perfil, asignado transaccionalmente. |
| estado | CODE | BORRADOR o PUBLICADA. No borrar publicación histórica. |
| revision | INT UNSIGNED | CAS de borrador/publicación. |
| nombre_visible | VARCHAR(160) UTF8 | Nombre efectivo de esta versión, independiente de posteriores ediciones de cabecera. |
| gabinete_id | INT UNSIGNED NULL | Destino; NULL solo en borrador. |
| modo_integridad | CODE | COMPLETO_OBLIGATORIO predeterminado; PARCIAL_PERMITIDO explícito. |
| ambito_integridad | CODE NULL | Decisión aprobada: EXPEDIENTE o LOTE, seleccionable obligatoriamente en modo completo. En parcial: NO_APLICA. NULL solo en borrador incompleto; impide publicar. |
| estrategia_carpetas | CODE NULL | Primera modalidad: RAIZ_EXPEDIENTE. |
| politica_subcarpetas | CODE NULL | Opciones implementadas explícitas; inclusión o rechazo. |
| esquema_config | SMALLINT UNSIGNED | Versión del contrato DOC. |
| configuracion_doc | DOC | Clasificación, fuentes del expediente, restricciones, política de código/coincidencias y descripción. Sin nivel fijo que reemplace el de apertura. |
| config_hash | HASH NULL | Obligatoria al publicar, calculada sobre configuración y reglas ordenadas. |
| creado_por, publicado_por | VARCHAR(128) ASCII; segundo NULL | Auditoría. |
| creado_utc, publicado_utc | DATETIME; segundo NULL | Auditoría. |

UQ `(perfil_id,numero)`; índice `(perfil_id,estado,numero)`. Estado, cabecera, revisión y pertenencia se bloquean/comprueban al publicar. Una publicación no modifica otra versión. El DOC usa un esquema cerrado: referencias de clasificación separadas por concepto, fuentes del código/fecha/responsable, políticas y límites. Los tipos/relaciones de catálogo los verifica el adaptador, no el cliente.

### `ra_integracion_perfil_regla`

| Columna | Tipo | Semántica |
|---|---|---|
| regla_id, version_id | ID PK; ID FK | Identidad y versión. |
| destino_tipo | CODE | CAMPO_EXPEDIENTE, INDICE_GABINETE o TIPOLOGIA. |
| destino_ref | VARCHAR(128) ASCII | Referencia registrada del campo; no expresión SQL. |
| fuente_tipo | CODE | CONSTANTE, CARPETA, ARCHIVO o EXPEDIENTE. |
| etapa_resolucion | CODE | PREFLIGHT o TRAS_EXPEDIENTE. |
| obligatoria | TINYINT UNSIGNED | 0/1. |
| orden | INT UNSIGNED | Orden de evaluación estable, no posición en UI. |
| regla_doc | DOC | Extracción, conversión y alternativas ordenadas si se habilitan. |

UQ `(version_id,destino_tipo,destino_ref)`: una asignación final por destino. Si hay fuentes alternativas, se expresan dentro de una regla con precedencia validada, no como filas contradictorias. Constantes del perfil y reglas no escriben dos veces el mismo destino: el compilador de configuración rechaza ese conflicto. Referencias índice pertenecen al gabinete de la versión. Índice `(version_id,orden)`.

## 3. Preparación y ejecución — SQL 02, propietario prompt 07-A

### `integracion_plan`

| Columna | Tipo | Semántica |
|---|---|---|
| plan_id | ID PK | Preparación asociada al contexto de apertura. |
| ambito_ref, usuario_ref, nivel_ref | VARCHAR(128) ASCII | Contexto validado, nunca autoridad libre del request. |
| version_id | ID FK | Versión publicada elegida. |
| estado | CODE | REVISABLE, BLOQUEADO, CONFIRMADO, EXPIRADO. |
| revision | INT UNSIGNED | Revisión entregada a confirmación. |
| esquema_doc | SMALLINT UNSIGNED | Contrato del plan. |
| contexto_doc, config_doc, manifiesto_doc, resultado_doc | DOC | Ruta canónica/revisión de destino, configuración efectiva, inventario original, errores/elegibilidad/valores diferidos. |
| plan_hash | HASH | Huella de representación canónica relevante. |
| creado_utc, expira_utc | DATETIME | Ventana de revisión configurada; no TTL inventado. |

Índice `(ambito_ref,usuario_ref,creado_utc)`. Cada preparación cambia la revisión o crea otro plan; el contrato 02 de diseño decide aquí nueva preparación = nuevo plan_id para simplificar trazabilidad. No editar un plan CONFIRMADO. La ventana de expiración y límites del manifiesto son parámetros de operación pendientes; sin configuración no aceptar payload ilimitado.

### `integracion_lote`

| Columna | Tipo | Semántica |
|---|---|---|
| lote_id | ID PK | Identidad de ejecución. |
| plan_id | ID FK UNIQUE | Un lote por plan confirmado. |
| ambito_ref, solicitante_ref, nivel_ref | VARCHAR(128) ASCII | Contexto de origen fijado. |
| clave_confirmacion | VARCHAR(64) ASCII | Clave de idempotencia dentro del solicitante/ámbito. |
| solicitud_hash | HASH | Igual clave/diferente solicitud produce conflicto. |
| version_id | ID FK | Versión fija del perfil. |
| modo_integridad, ambito_integridad | CODE | Copia efectiva; ámbito no puede ser NULL al confirmar. |
| contexto_doc, config_doc | DOC | Snapshots con esquema y configuración incluyendo reglas. |
| estado | CODE | Agregado operativo según diseño 03. |
| detencion_solicitada | TINYINT UNSIGNED | Señal, no prueba de detención. |
| total_raices, total_archivos | INT UNSIGNED | Totales originales congelados. |
| total_bytes | BIGINT UNSIGNED | Bytes previstos con límite de aplicación y representación segura en JSON. |
| revision | INT UNSIGNED | CAS, sin sustituir exclusión de efectos. |
| creado_utc, actualizado_utc | DATETIME | Auditoría. |

UQ `(ambito_ref,solicitante_ref,clave_confirmacion)`; sus caracteres ASCII mantienen este índice dentro de 767 bytes sin indexar textos largos. Índices `(ambito_ref,solicitante_ref,creado_utc)` y `(estado,actualizado_utc)`. Confirmación inserta lote/raíces/archivos y marca plan en una transacción; restricciones únicas resuelven carreras. Los totales de resultados se derivan de archivos, no se reemplaza el inventario original.

### `integracion_raiz`

| Columna | Tipo | Semántica |
|---|---|---|
| raiz_id, lote_id | ID PK; ID FK | Identidad persistida y lote. |
| cliente_raiz_id | VARCHAR(64) ASCII | Correlación de la selección local. |
| datos_doc | DOC | Nombre original, propuesta de expediente y fuentes diferidas. |
| estado | CODE | Fase de raíz según diseño 03. |
| identidad_expediente_doc | DOC NULL | Esquema lógico resuelto por despliegue, ID/código real y relación al nivel; no nombre como identidad. |
| valores_efectivos_doc | DOC NULL | Valores concretados tras creación, incluida fecha fijada. |
| intento_activo_id | ID NULL | Reclamación de efecto; referencia lógica a intento para evitar ciclo DDL. |
| revision | INT UNSIGNED | CAS. |
| creado_utc, actualizado_utc | DATETIME | Auditoría. |

UQ `(lote_id,cliente_raiz_id)`, índice `(lote_id,estado)`. Copiar el nombre no autoriza reutilizar un expediente de otro lote. Referencias documentales tienen esquema de identidad tipado en el adaptador; no pueden señalar una base arbitraria.

### `integracion_archivo`

| Columna | Tipo | Semántica |
|---|---|---|
| archivo_id, raiz_id | ID PK; ID FK | Identidad y raíz. Lote se deriva por raíz. |
| cliente_archivo_id | VARCHAR(64) ASCII | Correlación cliente; UQ por raíz. |
| origen_doc | DOC | Nombre original, ruta relativa exacta/normalizada y fecha local informativa. |
| ruta_hash | HASH | SHA-256 de ruta normalizada dentro de raíz. |
| bytes_previstos | BIGINT UNSIGNED | Metadato, no prueba de integridad. |
| bytes_recibidos | BIGINT UNSIGNED NULL | Conteo verificado. |
| contenido_hash | HASH NULL | Huella de contenido recibido. |
| staging_ref | UID NULL | Referencia opaca, nunca ruta de usuario. |
| estado_recepcion, estado_efecto | CODE | Dimensiones separadas; ver diseño 03. |
| plan_valores_doc | DOC | Valores de preflight y fuentes diferidas. |
| valores_efectivos_doc | DOC NULL | Materialización validada antes del efecto. |
| resultado_doc | DOC NULL | IDs reales, evidencia y fase pendiente, sin secretos. |
| intento_activo_id | ID NULL | Reclamación. |
| revision | INT UNSIGNED | CAS. |
| creado_utc, actualizado_utc | DATETIME | Auditoría. |

UQ `(raiz_id,cliente_archivo_id)` y `(raiz_id,ruta_hash)`. Comparar ruta canónica completa ante repetición/hash coincidente; colisión desigual bloquea, no fusiona. Normalización no aplica case folding global de archivos: distinguir rutas originales y detectar alias peligrosos según contrato de recepción, sin perder correspondencia local. Índice `(raiz_id,estado_recepcion,estado_efecto)`.

### `integracion_intento`

| Columna | Tipo | Semántica |
|---|---|---|
| intento_id, lote_id | ID PK; ID FK | Identidad y lote. |
| raiz_id, archivo_id | ID NULL FK | Exactamente el recurso correspondiente a fase; validación Service. |
| fase | CODE | RECEPCION, CREAR_EXPEDIENTE, ALMACENAR, INCORPORAR o VERIFICAR. |
| estado | CODE | RESERVADO, EN_CURSO, CONFIRMADO, RECHAZADO o INCIERTO. |
| ejecutor_ref | VARCHAR(128) ASCII | Identidad de instancia/operador, no cookie ni credencial. |
| token_reclamacion | UID | Identidad de compare-and-swap. No implica fencing en legacy. |
| solicitud_hash | HASH | Comando efectivo. |
| inicio_utc, ultimo_contacto_utc | DATETIME | Diagnóstico. |
| fin_utc | DATETIME NULL | No equivale a tiempo de respuesta HTTP. |
| evidencia_doc | DOC NULL | Resultado/fallo conocido o fundamentos de reconciliación. |

Índice `(lote_id,estado,inicio_utc)` y por recurso/fase. La exclusión se consigue actualizando intento_activo_id del recurso bajo bloqueo de fila; un índice de historial no garantiza por sí mismo un único efecto activo. No mantener la transacción MySQL abierta mientras corre almacenamiento legacy en otras conexiones.

### `integracion_evento`

| Columna | Tipo | Semántica |
|---|---|---|
| evento_id, lote_id | ID PK; ID FK | Identidad y lote. |
| intento_id | ID NULL FK | Correlación. |
| recurso_tipo, accion, codigo | CODE | Evento estructurado. |
| recurso_id | ID NULL | Referencia lógica según recurso_tipo. |
| actor_ref | VARCHAR(128) ASCII | Actor autenticado/técnico. |
| ocurrido_utc | DATETIME | UTC. |
| detalle_doc | DOC | Cambios/evidencia saneados; sin contenidos binarios ni secretos. |

Índice `(lote_id,ocurrido_utc,evento_id)`. Append-only por servicios; no UI que edite auditoría. Retención pendiente; no borrado automático de historial por cerrar ventana.

## 4. Relaciones, orden y control de integridad

```text
perfil → versión → reglas
             ↓
           plan → lote → raíz → archivo
                    ↓      ↘      ↙
                  evento ← intento
```

No existen FK nuevas a tablas SII ni lotes en su esquema. El prompt 02 entrega SQL 01 con tablas perfil/versión/regla; 07-A entrega SQL 02 con plan/lote/raíz/archivo/intento/evento. Crear primero padres; las referencias de puntero activo/publicación se mantienen lógicas verificadas para evitar ciclos y comprobar pertenencia. FKs al lote y recurso en intentos no prueban misma pertenencia: el repositorio debe resolverlo dentro de la transacción.

## 5. Límites y aceptación del modelo

El modelo está propuesto para permitir diseño paralelo, no declara listas cerradas de campos legacy ya verificadas para todos los ambientes. Antes del SQL final: resolver mapeo ámbito/usuario/nivel, contratos DOC cerrados, validación de los ámbitos aprobados, longitudes de catálogos, tamaños configurados de JSON, calendario UTC y retención. Publicar requiere referencias completas y reglas soportadas; almacenamiento debe validar representabilidad en catálogos legacy.

Pruebas obligatorias: CAS de publicación, confirmación doble, misma clave/distinto hash/payload, plan expirado, FK/recurso cruzado, rollback de confirmación, nombre Unicode no representable, ruta larga, colisión de identidad y lectura de snapshot tras publicar otra versión. No ejecutar DDL operativo para demostrarlo sin autorización de ambiente.
