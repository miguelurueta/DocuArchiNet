# DOC-124 — Recepción y preparación de Excel

Implementación del 23 de noviembre de 2026 sobre DOC122/DOC123. Estados durables
`RECIBIDA` y `PREPARADA`. E2E real del archivo Órdenes aprobada1/1 en IIS local,
con despliegue verificado y controles SELECT; la matriz completa de modos sigue
pendiente. No se crea tabla de negocio ni se confirman/importan órdenes.

## Recorrido de servicios

Todas las operaciones requieren sesión activa, permiso propio de cargas, empresa
y DA/módulo vigentes. Conservan el token `X-Importacion-Excel-CSRF` de DOC122 y
respuestas sin caché. La conexión funcional es la DA del módulo; no se añade otra
conexión en Web.config, cuenta o versión del conector.

| Operación | Entrada y resultado |
|---|---|
| ASMX `IniciarRecepcionExcel` | `ConexionId`, `PlantillaId`, `RevisionEsperada`, `HuellaEsperada`. Devuelve `IdCarga` UUID, referencia protegida y vencimiento UTC de 30 minutos. No crea fila ni archivo. |
| `generic_control/ImportacionExcelUpload.ashx` | POST multipart: un archivo `archivo` y campos únicos `idCarga`, `referenciaRecepcion`, `conexionId`, `plantillaId`. Devuelve estado, nombre descriptivo, bytes, hash y hojas observadas. |
| ASMX `Preparar` | `ConexionId`, `PlantillaId`, `IdCarga`, revisión/huella esperadas, `Hoja`, `Modo`; `PreparacionHashEsperada` para cambiar una preparación vigente. Devuelve estado, huellas, clave, etapas, totales y muestras. |
| ASMX `ConsultarCargaExcel` | `ConexionId`, `IdCarga`. Sólo lee la ejecución propia; no prepara, repara o confirma. |

La guarda verifica autoridad, origen, CSRF y cabeceras antes de `Form/Files`.
JSON conserva su máximo de 262144 bytes. Multipart admite bytes efectivos del
archivo más 262144 de metadatos; el archivo sigue limitado a 10 MB decimales y
10000 filas por defecto. Rechaza campos repetidos/adicionales, archivo extra,
vacío, nombre incompatible, tamaño inconsistente y formato real distinto.
ASP.NET puede bufferizar el cuerpo: la copia al almacenamiento es acotada, sin
promesa de reanudación por bytes o pausa durante el parseo.

## Identidad, almacenamiento y recuperación

`ReferenciaRecepcionExcel.vb` aplica MachineKey con propósito propio
`ImportacionExcelRecepcion/DOC124-v1`; vincula ID, propietario, empresa, módulo,
conexión, plantilla/revisión/huella y vencimiento. Puede usarse entre sesiones
independientes del mismo propietario, con CSRF válido de cada sesión.

El servidor genera `App_Data/ImportacionExcel/<empresa>/<modulo>/<id>/contenido.bin`.
El nombre original es descriptivo, nunca parte de la ruta. Comprueba componentes
sin reparse, crea `.part` exclusivo, cuenta bytes/hash, inspecciona XLS/XLSX y
publica sin sobrescribir. `App_Data` queda excluido de Git y protegido por ASP.NET;
el ambiente debe conservar ACL privadas de escritura. El archivo se retiene con
lectura que impide escritura/borrado mientras se publica RECIBIDA o se prepara.

Un único GET_LOCK por conexión, derivado de identidad física DA+ID y de 64
caracteres, serializa la operación sin esperar. Copia/lectura no mantienen una
transacción abierta; INSERT/UPDATE de ejecución usan transacciones cortas,
`FOR UPDATE` y condiciones de propietario, módulo, estado y archivo.

| Situación | Comportamiento |
|---|---|
| Acuse perdido tras recepción | Consultar el mismo ID conocido antes de enviar bytes. |
| Fila no visible | `RESULTADO_INCIERTO`; no demuestra ausencia de escritor. |
| Escritor de carga propia activo | `OPERACION_EN_CURSO`; no tomarlo por timeout. |
| Reintento explícito, mismo ID y bytes | Verifica el archivo íntegro y reutiliza/completa la recepción propia. |
| Bytes distintos o archivo alterado | Rechaza; conserva el completo anterior. |
| Fallo conocido antes de publicar archivo | Puede retirar únicamente el `.part` que creó esta operación. |
| Commit incierto | Conserva archivo/identidad; no degrada un ganador PREPARADA/CONFIRMADA. |

Los campos de preparación son nulos en la primera RECIBIDA; nunca se insertan
bytes cero o hash ficticio. El catálogo de hojas se devuelve durante recepción;
no tiene una columna durable propia en el esquema03. La consulta no relee el libro
para reconstruirlo. Una preparación inválida persiste sólo su código principal;
su detalle se recalcula mediante Preparar explícito si se perdió la respuesta.

## Esquema, aislamiento y claves

Para AGREGAR/REEMPLAZAR exige destino InnoDB, política activa RA para DA/tabla/
empresa y revisión. CONEXION requiere exclusividad configurada sin políticas
activas contradictorias en ese catálogo. COLUMNA exige la misma columna empresarial
en las políticas compartidas, de texto/entero, requerida y mapeada. Comprueba cada
valor empresarial mediante MySQL; no completa mapeos ni confía en el ámbito del
Excel. Política ausente/ambigua devuelve `POLITICA_DESTINO_NO_DEFINIDA`.

Lee atributos físicos e índices por nombre exacto de base/tabla, incluyendo
unsigned, precisión, charset/collation, nulabilidad y defaults. Exige UNIQUE completo
de clave o empresa+clave, sin prefijos ni columnas adicionales. No toma una PK
técnica como identidad de negocio. CREAR exige nombre `imp_02_` ausente y proyecta
tipos permitidos, charset/collation de servidor e índice de hasta 767 bytes; no crea
tabla, activa fuente o reserva el nombre. Conserva longitudes físicas existentes.

Reutiliza `ServicioLecturaExcel`: si devuelve incidencias o lectura incompleta,
su lista de filas se descarta y no se ejecuta la etapa de duplicados. Con lectura
válida, crea dos temporales propias sobre la conexión del lock, con definiciones
de clave idénticas al destino. Parametriza valores, conserva DECIMAL65 como cadena
canónica y acota lotes a 128 parámetros y al menor de 64 KiB o medio
`@@max_allowed_packet`. Agrupa la primera temporal hacia la segunda y las une
sin autojoin de una temporal, compatible con MySQL5.1.

AGREGAR rechaza duplicados internos y claves existentes; REEMPLAZAR admite
coincidencias existentes del ámbito pero rechaza internos; CREAR exige unicidad
interna y nombre libre. Preparar sólo lee datos de negocio. Distingue códigos y
filas físicas sin devolver valores de otra empresa.

Activa SQL estricto de sesión, rechaza advertencias de conversión y finalmente
retira temporales, restaura `sql_mode` y libera el lock. Si la limpieza no se
acredita, usa ClearPool dirigido a la conexión afectada antes de Dispose. La
primitiva existe en la DLL local MySql.Data 6.2.3.0; no cambia versión ni usa
ClearAllPools. Los dobles verifican el camino de descarte, no una reutilización
real del pool de un servidor.

## Publicación y límites de la evidencia

Archivo retenido verificado antes/después; relectura final de autoridad, plantilla,
revisión/huella, esquema y política. Cualquier cambio observado impide publicar.
`configuracion_doc` contiene snapshot físico ordenado, huella de plantilla v1,
hoja/modo, clave, política, identidad física/lógica, empresa/módulo y límites.
Su hash es distinto de la huella v1. `esquema_hash` incluye atributos e índices/
componentes ordenados con NULL explícito y JSON ASCII/UTF8 sin BOM. Hay vector
independiente Node→VB en fixtures. No se guardan filas normalizadas en sesión/DB.

La respuesta contiene hasta25 filas y25 incidencias, totales, truncamiento y
etapas ejecutadas. PREPARADA no congela escritores, DDL o datos externos; fase05
debe volver a validar antes de confirmar. No se promete importación, reemplazo,
fuente de índices activada, pantalla o rendimiento integral en esta fase.

La evidencia [local](../Validacion/doc124-validacion-local-20261123.md) distingue
pruebas sintéticas de MySQL/HTTP real. SQL11 queda entregado para ejecución manual;
no se prevé una nueva migración de las22 columnas. Diferencias requieren otro SQL
específico antes de ejecutar o verificar. No se alteran EGRESOS/CONTRATOS.

## Recorrido real y matriz pendiente

Reutiliza `run-doc110-profile-session.cjs --excel-prepare` y
`importacion-excel.spec.cjs`. El scope DOC124 debe identificar ambiente, empresa,
usuario, módulo y al menos cinco casos: CREAR/AGREGAR/REEMPLAZAR válidos, duplicado
interno y clave existente. Cada caso fija plantilla/revisión/huella, tabla
existente conservada o intención CREAR sin tabla, archivo local íntegro/hoja, filas y códigos esperados. Un caso válido
de 8000–10000 filas se usa para contención desde dos sesiones independientes; si
no observa un ganador y OPERACION_EN_CURSO, no acredita concurrencia.

La E2E reutiliza el control SELECT ODBC existente mediante `controlPrefix`, con
DSN/cuenta vigentes ingresados sólo en consola privada cuando no estén disponibles;
no añade una cuenta ni configuración web. Comprueba identidad propia en ejecución
y huellas de filas/esquema/índices del destino antes/después. No instala, siembra,
guarda plantillas o limpia tablas. La cuenta restringida acredita rechazo.

Corrección por instrucción del usuario2026-11-23: reutilizar las tablas ya consumidas
por las E2E, incluyendo EGRESOS/CRÉDITOS; se retira la exclusión fija por nombre de
EGRESOS/CONTRATOS. `destinationReadOnly=true` permite conservar una tabla existente
sin declararla descartable. Metadatos mediante queryRows y datos mediante
queryFingerprint DOC32, con orden físico completo y sin muestras/LIMIT. Así una
fuente con más de10000 filas puede comprobarse sin elevar el límite del Excel.
El inspector DOC110 incorpora Doc124Excel para reutilizar su conexión de módulo
y descubrir fuentes/proveedores/catálogos existentes, únicamente por SELECT.

La autorización «lanza la E2E» permitió ejecutar el caso Órdenes con cuentas/perfil
existentes y desplegar la DLL verificada para ese recorrido. Los demás casos de la
matriz siguen sujetos a destinos/configuración compatibles y alcance autorizado;
no se reabren las ventanas cerradas DOC122 ni la del archivo aprobado.

## Órdenes2025 y límite configurado20000

Solicitud posterior del usuario2026-11-23: crear la plantilla y aumentar a20000 filas.
Se añade `ImportacionExcel.MaxFilasDatos=20000` en Web.config, reutilizando el mecanismo
de configuración existente; el valor predeterminado del lector aislado sigue10000.
El alcance E2E admite hasta20000 y conserva10 MB decimales. La nueva plantilla27,
`imp_02_ordenes_2025`, usa conexión6, Hoja1 y clave textual NROORDEN; siete campos
incluyen las dos fechas DATE. Es intención CREAR, sin crear aún la tabla física.

El archivo original contiene un título en fila1 y encabezados en fila2: las órdenes
reales son12759, no12760; el conteo anterior incluía esa fila de encabezados. Se genera
una copia local compatible, quitando únicamente el título y ajustando referencias,
sin cambiar valores, formato o el original. Lector DOC123 sobre esa copia:12759 filas
completas,0 incidencias con máximo20000. Es validación local, no E2E real de servicios
ni confirmación/importación. SQL13 y evidencia específica documentan el alta autorizada.

## E2E real del archivo Órdenes

Por instrucción «lanza la E2E» se amplía el mismo runner con `--excel-file` y la misma
suite con `@doc124-file`. Scope `escenario=archivo`: exactamente un caso CREAR,
máximo20000 y12759 filas. El scope matriz conserva sus tres modos/cinco casos mínimos;
los escenarios no pueden usarse indistintamente para acreditar toda la fase.

El recorrido real aprobó1/1 con recuperación/replay, CSRF, cuenta restringida y dos
sesiones independientes/un ganador. La carga quedó PREPARADA con12759 previstas,
0 confirmadas y archivo verificado.32 SELECT externos explícitos acreditan identidad,
configuración efectiva, lock libre y conservación de tablas/plantillas/mapeos.
Se desplegó DLL verificada con dependencias y respaldo; no se crearon tablas ni
importaron órdenes. Ver Validacion/doc124-e2e-ordenes-resultado-20261123.md/JSON.
La matriz completa y revisión final siguen pendientes; el caso no cierra DOC124.
