# DOC-127 — Preparación de recursos para capacidad

**19/33 tareas;4.1 sigue abierta.** Base `4349a40259ba869a5b6c6502ac82736d0ea26a09`. Esta entrega prepara archivos manuales y18 borradores de ventanas; no aplica SQL, autentica ni importa datos. Conserva la evidencia local anterior.

## Hallazgo y decisión

El inventario nuevo de **11 SELECT** confirmó un UNIQUE completo sobre `imp01_plantillaimp.NOMBRE_PLANTILLA`. Crear una segunda plantilla XLS con el mismo nombre de tabla que58/59 produciría conflicto. Se conserva ese índice y la configuración anterior: XLSX reutiliza tablas/plantillas DOC126; XLS usa dos fixtures propios con el esquema DOC126; CREAR usa seis destinos nuevos, inicialmente ausentes.

Las plantillas57/58/59 conservan revisión1 y seis campos CLAVE/VALOR VARCHAR(250), No/No/No/0. Las políticas anteriores8/9 siguen activas/revisión2. Los conteos de agregar/reemplazar permanecen1002/1000. La cuenta141 mantiene sus cinco permisos;194 sólo cargas y196 ninguno. Son observaciones de preparación, no autorización de nuevas ventanas.

## Archivos y aplicación manual

| Orden | Archivo | Destino | Efecto previsto |
| --- | --- | --- | --- |
| 1 | [30-fixtures-xls-da-doc127.sql](../Sql/30-fixtures-xls-da-doc127.sql) | DA11/ref6 | Dos tablas XLS vacías con el esquema e índices de fixture ya probados. Los seis CREAR siguen ausentes. |
| 2 | [31-fuentes-xls-ra-doc127.sql](../Sql/31-fuentes-xls-ra-doc127.sql) | RA12 | Dos fuentes CONEXION de empresa2, cuatro columnas e historiales ALTA/ACTIVAR. Sin proveedores ni permisos nuevos. |
| 3 | [32-plantillas-capacidad-da-doc127.sql](../Sql/32-plantillas-capacidad-da-doc127.sql) | DA11/ref6 | Ocho cabeceras distintas y16 campos: dos XLS y seis CREAR. No escribe datos de negocio. |
| 4 | [33-verificacion-recursos-doc127.sql](../Sql/33-verificacion-recursos-doc127.sql) | Bloques separados DA/RA | SELECT desde conexión nueva, después de confirmar aplicación/COMMIT. |

SQL30: comprobar ausencia de ambas tablas antes del primer CREATE. Cada DDL confirma implícitamente; si sólo se crea una, conservarla y solicitar una corrección nueva. No repetir el lote completo ni borrar objetos.

SQL31: completar empresa2 y actor autorizado141 (LUZ.AGUILERA, confirmado activo/con permiso de fuentes en este corte), o el actor real revisado. Los valores públicos siguen NULL. Ante LISTO_PARA_COMMIT y cero errores, ejecutar COMMIT manual en esa misma sesión; otro resultado exige ROLLBACK. Conservar IDs de las dos fuentes.

SQL32: completar empresa_confirmada=2, modulo_confirmado=11, conexion=6 y preflight_confirmado=1 **sólo después de confirmar los pasos anteriores**. Debe informar LISTO_PARA_COMMIT y8/16/8 sin avisos. COMMIT manual en la misma sesión; conservar los ocho IDs. Un error o resultado distinto exige ROLLBACK. Los locks del catálogo se liberan al terminar la transacción.

El usuario confirma archivo, destino y aplicación/COMMIT; después se verifican los resultados por SELECT. Todos los archivos modificadores permanecen **PENDIENTE_EJECUCION_MANUAL**. SQL33 es lectura, no prueba de aplicación por existir en el repo. [EXPLAIN29](../Sql/29-explain-proveedor-doc127.sql) sigue independiente y pendiente.

## Ventanas de capacidad propuestas

Una combinación por ventana, máximo una recepción nueva y una confirmación efectiva más su replay controlado. Orden propuesto: XLSX y después XLS; tamaños1000/5000/10000, con CREAR/AGREGAR/REEMPLAZAR. **No constituye autorización de18 escrituras.**

| # | Formato | Modo | Filas | Destino | Plantilla | Filas previas previstas |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | XLSX | CREAR | 1000 | imp_02_doc127_e2e_crear_xlsx_1000 | Tras SQL32 | 0 |
| 2 | XLSX | AGREGAR | 1000 | imp_02_doc126_e2e_agregar | 58 | 1002 |
| 3 | XLSX | REEMPLAZAR | 1000 | imp_02_doc126_e2e_reemplazar | 59 | 1000 |
| 4 | XLSX | CREAR | 5000 | imp_02_doc127_e2e_crear_xlsx_5000 | Tras SQL32 | 0 |
| 5 | XLSX | AGREGAR | 5000 | imp_02_doc126_e2e_agregar | 58 | 2002 |
| 6 | XLSX | REEMPLAZAR | 5000 | imp_02_doc126_e2e_reemplazar | 59 | 1000 |
| 7 | XLSX | CREAR | 10000 | imp_02_doc127_e2e_crear_xlsx_10000 | Tras SQL32 | 0 |
| 8 | XLSX | AGREGAR | 10000 | imp_02_doc126_e2e_agregar | 58 | 7002 |
| 9 | XLSX | REEMPLAZAR | 10000 | imp_02_doc126_e2e_reemplazar | 59 | 5000 |
| 10 | XLS | CREAR | 1000 | imp_02_doc127_e2e_crear_xls_1000 | Tras SQL32 | 0 |
| 11 | XLS | AGREGAR | 1000 | imp_02_doc127_e2e_agregar_xls | Tras SQL32 | 0 |
| 12 | XLS | REEMPLAZAR | 1000 | imp_02_doc127_e2e_reemplazar_xls | Tras SQL32 | 0 |
| 13 | XLS | CREAR | 5000 | imp_02_doc127_e2e_crear_xls_5000 | Tras SQL32 | 0 |
| 14 | XLS | AGREGAR | 5000 | imp_02_doc127_e2e_agregar_xls | Tras SQL32 | 1000 |
| 15 | XLS | REEMPLAZAR | 5000 | imp_02_doc127_e2e_reemplazar_xls | Tras SQL32 | 1000 |
| 16 | XLS | CREAR | 10000 | imp_02_doc127_e2e_crear_xls_10000 | Tras SQL32 | 0 |
| 17 | XLS | AGREGAR | 10000 | imp_02_doc127_e2e_agregar_xls | Tras SQL32 | 6000 |
| 18 | XLS | REEMPLAZAR | 10000 | imp_02_doc127_e2e_reemplazar_xls | Tras SQL32 | 5000 |

Los checkpoint anteriores suponen que todos los recorridos previos concluyen según lo previsto. Se reemplazan por conteos y huellas SELECT nuevos antes de cada autorización. XLS empieza con cero filas; esa condición se informa en las mediciones de reemplazo. Los seis CREATE escriben sólo destinos nuevos. Todos los archivos sintéticos conservan los hashes comprobados por el lector local; no se regeneran ni se presentan como importaciones realizadas.

Los18 borradores privados tienen referencias aún nulas, sin fechas ni autorización, y el validador existente rechaza los18 antes de autenticar. Se guardan en `.tmp/doc127-preparacion/`, excluido de Git. No contienen credenciales. La propuesta usa como candidata la cuenta141, empresa2/DA11, HTTPS local, sujeto a autorización específica posterior.

## Verificación de esta entrega

Revisión estructural de destinos SQL, ausencia de COMMIT automático y ocho nombres distintos frente al UNIQUE; contraste con los metadatos actuales. 59 huellas de fuentes funcionales/pruebas del corte anterior coinciden. No se modificó producto, harness, DLL ni configuración; no se repitieron build o pruebas funcionales ya aprobadas. OpenSpec/refinamiento y formato se verifican por separado.

Quedan pendientes aplicación manual confirmada, SELECT posterior, cierre de recursos4.1, autorización/ejecución real y revisión del EXPLAIN. La preparación conserva el avance19/33 y no atribuye aprobación humana global.
