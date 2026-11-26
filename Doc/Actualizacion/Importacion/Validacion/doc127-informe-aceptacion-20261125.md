# DOC-127 — Informe de aceptación y capacidad con restricciones

Actualización de autoridad2026-11-26: **1/1 E2E real y seis rechazos PASS;29/33 tareas**. [Resultado y clasificación](doc127-autoridad-real-20261126.md). Se completa5.1: permisos, CSRF, referencias y lectura reales; preparación18/18 reutilizada y36 cambios de autoridad sintéticos identificados.38 tablas/configuración,78 perfiles y41 cargas conservadas. Cero importaciones/guardados/cambios de permisos; ventana consumida. La matriz de capacidad conserva18/18 PASS. Pendientes2.4,5.2,7.1,7.5; los cortes28/33 y anteriores son históricos.

Siguiente ejecución preparada: [autoridad y lectura](doc127-ventana-autoridad-20261126.md), pendiente de autorización para cuentas194/196 en IIS local GESTOR. Seis rechazos y comprobaciones de lectura con cero cargas, guardados o cambios de permisos;38 tablas/78 perfiles/41 cargas protegidas. La preparación no es evidencia real adicional ni completa tareas. Avance vigente28/33 y capacidad18/18.

Actualización 2026-11-26: **XLSX10000,3/3 modos reales PASS;28/33 tareas**. [Resultado y mediciones](doc127-capacidad-xlsx10000-20261126.md). Se completa6.6, además de6.1/6.2/6.3/6.4/6.5 ya acreditadas. Matriz de capacidad18/18 completa; los cortes27/33 y anteriores inferiores son históricos.


Actualización 2026-11-26: **XLSX5000,3/3 modos reales PASS;27/33 tareas**. [Resultado y mediciones](doc127-capacidad-xlsx5000-20261126.md). Se completa6.5, además de6.1/6.2/6.3/6.4 ya acreditadas. Los cortes26/33 y anteriores inferiores son históricos.


Actualización 2026-11-26: **XLSX1000,3/3 modos reales PASS;26/33 tareas**. [Resultado y mediciones](doc127-capacidad-xlsx1000-20261126.md). Se completa6.4, además de6.1/6.2/6.3 ya acreditadas. Los cortes25/33 y anteriores inferiores son históricos.


Actualización 2026-11-26: **XLS10000,3/3 modos reales PASS;25/33 tareas**. [Resultado y mediciones](doc127-capacidad-xls10000-20261126.md). Se completa6.3, además de6.1 y6.2 ya acreditadas. Los cortes24/33,23/33 y22/33 inferiores son históricos.


Actualización 2026-11-26: **XLS5000,3/3 modos reales PASS;24/33 tareas**. [Resultado y mediciones](doc127-capacidad-xls5000-20261126.md). Se completa6.2, además de6.1 ya acreditada. Los cortes23/33 y22/33 inferiores son históricos.


Estado actual: **23/33 tareas**. XLS1000 pasó sus tres modos reales y se completó6.1; [resultados, tiempos y memoria](doc127-capacidad-xls1000-20261125.md). La aceptación integral continúa pendiente. Los párrafos de corte inicial y lectura conservan sus fechas y cifras históricas.

Corte inicial de evaluación: `23464d6b07155fc8e4e6afa88e81db5ef5c97cdf`. Estado **ACEPTACIÓN INTEGRAL BLOQUEADA**, cambio activo. La consolidación inicial reunió resultados previos con su SHA, sin nuevas pruebas reales en ese corte. **22/33 tareas**: se completaron la consolidación6.7 y el informe7.4 por documentar resultados y vacíos explícitos. Las seis tareas de ejecución de capacidad y las de aceptación integral siguen abiertas.

Actualización de lectura: **1/1 E2E real PASS**, corte `986f0f1cc9974dd81343e3f9227a264d53bba13c`, doc127-1, cuentas194/196. Consulta/Escape y aislamiento de form1 acreditados;32 tablas y configuración conservadas.78 perfiles y cinco resultados previos coinciden con la línea base fresca; se registra diferencia de huella histórica anterior al login. [Evidencia vigente](doc127-lectura-real-20261125.md). La ventana está consumida; capacidad y aceptación integral siguen pendientes. La procedencia inferior conserva el corte inicial de consolidación.

Actualización de capacidad: **XLS1000,3/3 modos reales PASS;23/33 tareas**. [Resultado vigente](doc127-capacidad-xls1000-20261125.md). Se completa6.1; las cifras22/33 inferiores corresponden al corte inicial. Los resultados y tiempos/memoria de estas tres celdas sustituyen sus vacíos iniciales; las demás mantienen su estado.

## Procedencia y vigencia

Ambiente de referencia: Windows local; .NET Framework 4.6.1; IIS HTTPS/GESTOR empresa2 DA11/RA12 cuando la evidencia es real; MySQL5.1.37/MySql.Data6.2.3. Normalización de fuentes UTF8/LF; huellas binarias para runtime. El JSON adjunto contiene SHA256, fecha original, SHA evaluado, fuentes comparadas y relaciones caso/evidencia.

| Evidencia | Tipo | SHA original | Archivo |
| --- | --- | --- | --- |
| LOCAL | LOCAL_SINTETICA_BUILD_Y_SELECT_PREVIO | 10f307112c6239faa8180a021a8064eccc25b538 | [doc127-implementacion-local-20261125.json](doc127-implementacion-local-20261125.json) |
| INTEGRACION | LOCAL_SINTETICA_Y_SELECT_PREVIO | 17bdf09e4f46efb43cbe4771d4de99ee8a308628 | [doc127-integracion-20261125.json](doc127-integracion-20261125.json) |
| RECURSOS | APLICACION_DELEGADA_PREVIA_VERIFICADA_SELECT | 6428e62ca6c9cb4a650d96adff5f2eceddb3663b | [doc127-aplicacion-recursos-20261125.json](doc127-aplicacion-recursos-20261125.json) |
| DOC125 | REAL_HISTORICA_REUTILIZADA_LIMITADA | 7a4247c96639097b75f887183dd0a2343458d25b | [doc125-ejecucion-seguimiento-20261124.json](doc125-ejecucion-seguimiento-20261124.json) |
| DOC126_FUNCIONAL | REAL_HISTORICA_REUTILIZADA_LIMITADA | 0a3117662d78f69c1314989593abd4e0796671d2 | [doc126-e2e-funcional-real-20261124.json](doc126-e2e-funcional-real-20261124.json) |
| DOC126_ASISTENTE | REAL_HISTORICA_REUTILIZADA_LIMITADA | 5046842b05bad4ec1e8b85a8c9d8f09df9f04e92 | [doc126-e2e-asistente-real-20261125.json](doc126-e2e-asistente-real-20261125.json) |
| DOC126_RECUPERACION | REAL_HISTORICA_REUTILIZADA_LIMITADA | 12ae034eca355c9d301250644908c7f210b35693 | [doc126-e2e-rollback-concurrencia-20261124.json](doc126-e2e-rollback-concurrencia-20261124.json) |
| DOC126_LECTURA | REAL_HISTORICA_REUTILIZADA_LIMITADA | a2d6f19237d8eeaa7e1c6e1e1d300ea96fbee653 | [doc126-e2e-permisos-real-20261125.json](doc126-e2e-permisos-real-20261125.json) |
| DOC127_LECTURA | REAL_ACTUAL_LECTURA_SIN_CARGAS | 986f0f1cc9974dd81343e3f9227a264d53bba13c | [doc127-lectura-real-20261125.json](doc127-lectura-real-20261125.json) |
| DOC127_XLS1000_REAL | REAL_CAPACITY_XLS1000_THREE_MODES | c2277806aa5869521bdd43041561002431417d7d | [doc127-capacidad-xls1000-20261125.json](doc127-capacidad-xls1000-20261125.json) |
| DOC127_XLS5000_REAL | REAL_CAPACITY_XLS5000_THREE_MODES | 365330139fed82672367bccc6903845b3f331619 | [doc127-capacidad-xls5000-20261126.json](doc127-capacidad-xls5000-20261126.json) |
| DOC127_XLS10000_REAL | REAL_CAPACITY_XLS10000_THREE_MODES | 2cc794bb8795121a3c568de82af57a2ae1229976 | [doc127-capacidad-xls10000-20261126.json](doc127-capacidad-xls10000-20261126.json) |
| DOC127_XLSX1000_REAL | REAL_CAPACITY_XLSX1000_THREE_MODES | a4bb16a917f2ec2a6fd16f24583058a794e4f6ac | [doc127-capacidad-xlsx1000-20261126.json](doc127-capacidad-xlsx1000-20261126.json) |
| DOC127_XLSX5000_REAL | REAL_CAPACITY_XLSX5000_THREE_MODES | 96dfc85e3a78ecba9653eaaa7a6a55f0723b0d39 | [doc127-capacidad-xlsx5000-20261126.json](doc127-capacidad-xlsx5000-20261126.json) |
| DOC127_XLSX10000_REAL | REAL_CAPACITY_XLSX10000_THREE_MODES | 86074721005fd4b9c6b3d8a6347e5add474b6649 | [doc127-capacidad-xlsx10000-20261126.json](doc127-capacidad-xlsx10000-20261126.json) |

Se conservan las15 huellas de integración (17bdf09e4f46efb43cbe4771d4de99ee8a308628):141/141 pruebas locales de11 suites. De79 fuentes del corte local anterior,73 coinciden y6 difieren: README SQL/runbook actualizados, página/Vista/UI con la corrección form1 y suite E2E ampliada. Las cuatro diferencias de producto/pruebas están incluidas en la evidencia posterior de integración. DLL, conector, Web.config, API y CSS conservan su huella binaria; Vista cambió conforme a esa corrección.

Build aislado previo net461:0 errores/308 advertencias. Harness previo:4616 checks nativos;19 pruebas SII y lector de18 libros PASS. No se repiten suites con fuentes idénticas para esta entrega documental ni se atribuye un nuevo build global a HEAD. La aceptación histórica conserva runtime/assets/archivos y límites propios; no certifica los cambios posteriores de interfaz.

## Matriz de requisitos

Cada ID remite a la tabla de procedencia con su SHA/ambiente/tipo; el JSON conserva el caso y prueba concretos. LOCAL mezcla build, fixtures sintéticos y SELECT previos según su caso: no equivale a E2E real.

| RQ / caso | Prueba y evidencia | Resultado | Límite |
| --- | --- | --- | --- |
| RQ-01: Manifiesto y comparacion por fuentes | DOC127_SOURCE_AND_EVIDENCE_AUDIT; LOCAL, INTEGRACION | LOCAL_PASS | La huella de una fuente no transfiere toda la aceptacion de un runtime diferente. |
| RQ-02: Runner DOC110/scopes DOC125–127 | tests/doc127-e2e-reuse.test.cjs; LOCAL, INTEGRACION, DOC127_LECTURA | LOCAL_PASS_REAL_READ_PASS_CAPACITY_PENDING | Lectura DOC127 autorizada y consumida; capacidad pendiente de ventanas propias. Sin instalar/sembrar/limpiar desde E2E. |
| RQ-03: Autoridad/CSRF/preparación | DOC127_AUTORIDAD_REAL;18 preparaciones reales;36 cambios sintéticos | REAL_AUTHORITY_READ_PASS_PREPARATION_REUSED | Seis rechazos y lectura194/196 PASS;38 tablas/configuración,78 perfiles y41 cargas conservadas. Revocación física no ejecutada; nivel sintético identificado. [Detalle](doc127-autoridad-real-20261126.md) |
| RQ-04: Instalacion compatible y flujo manual | DOC127_SELECT_CONTRACT; Sql/README.md; LOCAL, RECURSOS | SELECT_PASS_ENTREGA_SQL | Controles actuales equivalentes, no ejecucion manual integra de28; metadatos sujetos a visibilidad. |
| RQ-05: Plantillas y tres modos por formato | DOC122–125_NATIVE; DOC127_LOCAL_READER; DOC126 reales; LOCAL, DOC126_FUNCIONAL, DOC126_ASISTENTE | LOCAL_PASS_REAL_PARCIAL | 18 libros locales no equivalen a18 importaciones. XLS real por modo sin completar. |
| RQ-06: Claves/collation/esquema/politica | DOC124_NATIVE; DOC125_NATIVE; DOC126 rollback/carrera; LOCAL, DOC126_RECUPERACION | LOCAL_PASS_HISTORICA_REUTILIZADA | Escenarios adversos de motor/FK/trigger y politica COLUMNA tienen cobertura sintetica. |
| RQ-07: Replay/CREATE/rollback/carrera | DOC125_NATIVE; DOC126 rollback/carrera; transporte DOC127; LOCAL, DOC125, DOC126_RECUPERACION | LOCAL_PASS_HISTORICA_REUTILIZADA | Acuse ignorado no acredita corte fisico COMMIT/red. No se repiten las carreras anteriores. |
| RQ-08: Filas/bytes/cambio de limites | ImportacionExcelLecturaTests.Boundaries/Reports; ConfirmacionTests.Rejections; LOCAL | LOCAL_PASS | Fronteras grandes y cambios de configuracion son locales; IIS permanece intacto. |
| RQ-09: Seis celdas y medicion por modo | DOC127_LOCAL_READER; DOC125 tiempos historicos; LOCAL, DOC125, RECURSOS, DOC127_XLS1000_REAL, DOC127_XLS5000_REAL, DOC127_XLS10000_REAL, DOC127_XLSX1000_REAL, DOC127_XLSX5000_REAL, DOC127_XLSX10000_REAL | REAL_CAPACITY_MATRIX_18_PASS_SHARED_SAMPLING | 18/18 combinaciones XLS/XLSX de1000/5000/10000 filas y tres modos con resultado real. Memoria IIS compartida y muestreada, sin asignacion exclusiva ni picos garantizados; no extrapolar20000. |
| RQ-10: Consumidor por clave y regresion | harness DOC110;19 tests SII;29-explain-proveedor-doc127.sql; LOCAL | LOCAL_PASS_EXPLAIN_PENDIENTE | Plan manual no aportado; importar tabla no publica proveedor/fuente. |
| RQ-11: UI/cliente/semaforo/foco | tests/doc126-importacion-excel-ui.test.cjs; client; INTEGRACION, DOC126_LECTURA, DOC127_LECTURA | REAL_DOC127_1_READ_AND_FORM_ISOLATION_PASS | Lectura/consulta/Escape y aislamiento DOM doc127-1 reales; no se envio un postback del gestor ni se ejecutaron escrituras o capacidad. |
| RQ-12: Build aislado y regresion actual | MSBuild net461; logs locales por SHA; LOCAL, INTEGRACION | LOCAL_PASS | Cero despliegues; el binario compilado aislado no sustituye el instalado. |
| RQ-13: Instalacion/operacion/recuperacion | Sql/README.md; Operacion-Excel.md; LOCAL, RECURSOS | DOCUMENTADO | La carga no ofrece pausa por fila; cualquier cambio SQL sigue el procedimiento manual. |
| RQ-14: Matriz y gates finales | Este corte y tareas OpenSpec; LOCAL, INTEGRACION | INFORME_PUBLICADO_ACEPTACION_BLOQUEADA | Capacidad/autorizacion/EXPLAIN y aceptacion integral impiden declarar cierre. |

## Capacidad por formato, tamaño y modo

P/C son milisegundos HTTP de preparación/confirmación. Los tres pares numéricos pertenecen a DOC125, con su SHA de la tabla; incluyen transporte/metadatos y no son tiempo SQL puro. Cada celda corresponde a tres casos distintos. Las referencias de plantilla son planificación verificada previamente, no autorización vigente. Bytes y SHA de cada libro DOC127 figuran en el JSON; los archivos históricos DOC125 son otros.

| Formato / filas | Modo | Plantilla prevista | Bytes libro DOC127 | Lector local previo | Real DOC127 P/C/memoria | Histórico DOC125 P/C |
| --- | --- | --- | --- | --- | --- | --- |
| XLS / 1000 | CREAR | 63 r1 | 273920 | PASS | REAL_PASS; P/C 159.0/11000.8 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLS / 5000 | CREAR | 64 r1 | 1362432 | PASS | REAL_PASS; P/C 359.1/12877.2 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLS / 10000 | CREAR | 65 r1 | 2763776 | PASS | REAL_PASS; P/C 653.0/12084.3 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLS / 1000 | AGREGAR | 61 r1 | 282112 | PASS | REAL_PASS; P/C 308.9/11431.6 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLS / 5000 | AGREGAR | 61 r1 | 1402880 | PASS | REAL_PASS; P/C 519.4/12665.2 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLS / 10000 | AGREGAR | 61 r1 | 2844160 | PASS | REAL_PASS; P/C 1046.1/13187.9 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLS / 1000 | REEMPLAZAR | 62 r1 | 293888 | PASS | REAL_PASS; P/C 183.3/11255.2 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLS / 5000 | REEMPLAZAR | 62 r1 | 1463296 | PASS | REAL_PASS; P/C 385.6/11236.8 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLS / 10000 | REEMPLAZAR | 62 r1 | 2964992 | PASS | REAL_PASS; P/C 729.1/12284.7 ms; memoria compartida en evidencia | Sin ejecución histórica de esta celda |
| XLSX / 1000 | CREAR | 66 r1 | 16356 | PASS | REAL_PASS; P/C 164.3/11573.2 ms; memoria compartida en evidencia | Evidencia histórica XLSX1000 conservada por separado |
| XLSX / 5000 | CREAR | 67 r1 | 73869 | PASS | REAL_PASS; P/C 485.0/11835.5 ms; memoria compartida en evidencia | Antecedentes conservados por separado |
| XLSX / 10000 | CREAR | 68 r1 | 145383 | PASS | REAL_PASS; P/C 1031.7/12838.9 ms; memoria compartida en evidencia | Antecedentes conservados por separado |
| XLSX / 1000 | AGREGAR | 58 r1 | 16359 | PASS | REAL_PASS; P/C 272.0/11261.1 ms; memoria compartida en evidencia | Evidencia histórica XLSX1000 conservada por separado |
| XLSX / 5000 | AGREGAR | 58 r1 | 73872 | PASS | REAL_PASS; P/C 716.5/11828.0 ms; memoria compartida en evidencia | Antecedentes conservados por separado |
| XLSX / 10000 | AGREGAR | 58 r1 | 147007 | PASS | REAL_PASS; P/C 1127.2/12898.7 ms; memoria compartida en evidencia | Antecedentes conservados por separado |
| XLSX / 1000 | REEMPLAZAR | 59 r1 | 16632 | PASS | REAL_PASS; P/C 193.8/10869.6 ms; memoria compartida en evidencia | Evidencia histórica XLSX1000 conservada por separado |
| XLSX / 5000 | REEMPLAZAR | 59 r1 | 75297 | PASS | REAL_PASS; P/C 685.4/12554.5 ms; memoria compartida en evidencia | Antecedentes conservados por separado |
| XLSX / 10000 | REEMPLAZAR | 59 r1 | 146809 | PASS | REAL_PASS; P/C 1034.0/12969.2 ms; memoria compartida en evidencia | Antecedentes conservados por separado |

Las18 ventanas nuevas conservan PENDIENTE_AUTORIZACION_Y_PREFLIGHT_DE_VENTANA. Sus conteos previos/esperados se recalculan antes de cada ejecución; no son una observación actual. Las plantillas61–68 y fuentes10/11 se aplicaron por delegación expresa y fueron verificadas; no reaplicar SQL30–32. La lectura separada ya terminó PASS y no autoriza cargas.

Memoria histórica DOC125:104 muestras entre12:52:09.981Z y12:55:38.267Z, máximo RSS agregado279072768bytes/privados581500928bytes;102 muestras entre14:31:39.331Z y14:35:03.612Z, RSS237957120bytes/privados596037632bytes. Sin baseline por carga y con cobertura parcial; no asignar estos máximos a un modo. El monitor DOC127 prevé baseline e intervalo solicitado2s; todavía no aporta mediciones durante una carga real.

Fronteras: configuración vigente20000 filas/10000000bytes; fallback lector10000. El harness acredita10000/10001 y20000/20001 según configuración,10000000/10000001 bytes con documentos reconocidos y cambios entre preparar/confirmar. Son casos locales; no certifican fronteras HTTP ni20000 filas confirmadas en IIS. DOC1248000/12759 sólo preparó; DOC1251000 confirmadas no se extrapola a5000/10000/20000.

## Bloqueos y siguiente acción

| Bloqueo / tareas | Impacto | Acción |
| --- | --- | --- |
| B-01: 2.4 | Rendimiento de lectura por clave sin plan verificable. | El usuario ejecuta manualmente Sql/29-explain-proveedor-doc127.sql en el destino indicado y aporta key/possible_keys/type/rows/Extra y metadatos; revisar sin ejecutar EXPLAIN desde el agente. |
| B-02: 7.1 | Autoridad/lectura vigentes aprobadas; queda el corte integral tras los cruces de plantillas pendientes. | Conservar la ventana consumida y la clasificación de cobertura; cerrar hallazgos tras5.2, sin repetir cargas ni cambiar permisos. |
| B-03: 5.2 | La matriz18/18 de capacidad tiene resultados reales y mediciones. Los cruces restantes de plantillas y su aceptación funcional siguen pendientes. | Conservar las dieciocho cargas actuales y sus scopes consumidos; completar sólo los cruces funcionales pendientes de plantillas mediante un alcance revisable, sin repetir capacidad. |
| B-04: 7.5 | Faltan tareas y aceptacion; no procede archivo ni cierre Jira. | Mantener el cambio activo. Evaluar gates sobre HEAD registrado despues de resolver dependencias; los controles documentales no sustituyen aceptacion real. |

Hallazgo form1 corregido en17bdf09e, con81/81 focales y posterior integración141/141; la lectura posterior acredita aislamiento DOM real doc127-1 sin postback del gestor.7.1 permanece abierta para el corte integral. Revocación física, política COLUMNA real y cortes físicos COMMIT/red conservan sus limitaciones; acuse ignorado y fallos sintéticos no los sustituyen.

Guías vigentes: [instalación/estados SQL](../Sql/README.md), [operación y recuperación por ID](../Operacion-Excel.md) y [ventana de lectura ejecutada](doc127-lectura-propuesta-20261125.md). EXPLAIN29 sigue pendiente de ejecución manual y aportación del plan. No hay aprobación humana global, archivo ni cierre Jira. El corte inicial de consolidación no ejecutó SQL ni sesiones. La lectura posterior abrió dos sesiones y usó sólo SELECT de control, sin importaciones, cambios de permisos o despliegues.

## Validación de esta entrega

OpenSpec estricto PASS y refinamiento15/15 PASS. OPSXJ68/72 controles PASS en el corte inicial: se corrigió el marcador Clasificacion en los cinco documentos técnicos; quedan abiertos11 tareas, revisión persistida por SHA, evidencia unit vinculada al SHA vigente y manual_qa. No se crea evidencia ficticia para superar las compuertas. La salida local queda en .tmp/ y el resumen en el JSON. Revalidar sobre el HEAD registrado antes de una entrega de cierre. `git diff --check` PASS. La actualización posterior ejecutó1/1 E2E real de lectura y revalidó OpenSpec/refinamiento; no repitió build o pruebas de producto ajenas al alcance.
