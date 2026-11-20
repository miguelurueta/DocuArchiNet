# DOC-120 — preparación del laboratorio de persistencia segmentada

Fecha: 2026-11-20. Base: `b25d07a1f91fe8eed0d4a1814540060bdd9b319e`, carpeta principal y rama `feature/DOC-120`. Origen: D-05/RQ-05, D-08/RQ-08 y D-10/RQ-10. Avance parcial de **3.118**, sin cambiar su casilla. Estado: **ARNÉS LOCAL PREPARADO; MYSQL NO EJECUTADO**. Se mantienen 259/270 tareas completas y once abiertas.

## Preparación implementada

`tests/ImportacionCarpetasSegmentedRepositoryIntegrationTests.cs` añade el punto explícito `RunDoc120SegmentedRepositoryIntegration`. Reutiliza el repositorio productivo, el constructor interno de observación y el coordinador de dos operaciones del arnés DOC-115. Se compila mediante el arnés Framework existente; no cambia clases de la aplicación ni añade endpoints, DDL, dependencias o instalación de DLL.

Indicación posterior del usuario: reutilizar las E2E existentes. Se elimina la copia del ensayo de rollback en el helper nuevo: `RunConfirmationRollbackCases` es ahora el mismo núcleo utilizado por `RunRollbackIntegration` de DOC-111/115 y por la preparación segmentada de DOC-120. Conserva los tres puntos, SQL, comprobaciones transaccionales y revisión de ausencia de publicación; los conteos proceden del fixture existente (dos raíces en DOC-115, una en este caso). Cada llamador conserva autorización, conexión y presupuesto. La confirmación de replay del lote previo sigue en el recorrido DOC-111/115.

| Infraestructura E2E existente | Reutilización |
| --- | --- |
| `run-doc111-persistence.ps1` y `support/doc115-repository-integration.ps1` | Entrada de persistencia, compilación, journal, huellas y controles de conservación; adaptar únicamente el escenario y sus recursos. No crear un ejecutor paralelo ni heredar sus permisos o IDs. |
| `ImportacionCarpetasBehaviorTests.cs:RunRollbackIntegration` | Ensayo real de rollback compartido, con los mismos observadores y SQL. |
| `run-doc120-qa-failures.cjs` | Recorridos ya existentes `lostResponse` y `concurrency`, autenticación y SELECT de control. Sus resultados históricos conservan su alcance y ventanas cerradas; no repetirlos para sustituir el rollback nativo faltante. |
| `run-doc120-storage-ui.cjs` | Pausa, cierre y continuación ya probados; reutilizar su evidencia y recorrido para la aceptación UI pertinente. |

La ampliación específica sigue siendo el transporte persistente de partes y su sellado. No se reescriben autenticación, captura privada, navegación UI, pérdida de respuesta, pausa/cierre o concurrencia que ya existen.

La ejecución habitual del arnés solo llama `Doc120SegmentedRepositoryGuardCases`: **16 comprobaciones internas sin conexión**, incluidas autorización, alcance, destino, errores sanitizados antes de acceso, preparación pura del inventario, reconstrucción de partes fuera de orden y rechazo de la llamada 49 antes de ejecutar su delegado. No llama al recorrido MySQL.

La entrada real necesita autorización explícita, conexión privada proporcionada por el operador, plan sintético acotado, ID nuevo de 32 hexadecimales, huella SHA-256 de `servidor|base` y un registrador de fases/IDs. No obtiene claves de archivos, configuración, procesos o ventanas anteriores. Comprueba destino antes de conectar y usa `Pooling=false`, con timeout de conexión de cinco segundos. Un fallo inesperado devuelve un código cerrado sin adjuntar la excepción privada.

El plan de entrada es esquema 1 y se clona con el serializador acotado existente. Se valida de forma pura antes de abrir conexiones; después las transacciones del repositorio revalidan la publicación. El ensayo convierte su preparación sellada a esquema 2. El clon exclusivo de esquema 2 no se usa para la entrada inicial de esquema 1.

## Recorrido MySQL preparado, todavía sin acreditar

| Caso | Control que ejecutará el arnés |
| --- | --- |
| Inicio y acuse perdido | Una preparación nueva; repetir la cabecera converge y cambiar su huella rechaza. Encontrar una preparación anterior con el mismo ID de ejecución detiene la entrada, sin retomarla. |
| Partes faltantes y expiración | No permite sellar un borrador incompleto. Un reloj de prueba adelantado rechaza escritura vencida; no modifica fechas en BD. |
| Orden, duplicado y conflicto | Registra raíz y después archivos en orden 2→1; repetir la parte 2 conserva revisión, contenido distinto rechaza. Dos operaciones con la misma parte 1 convergen en una revisión y tres posiciones únicas. |
| Validación y CAS | No sella partes sin validar; rechaza validación con revisión antigua y después publica los tres resultados de parte con un sello de prueba consistente. |
| Sellado concurrente y recuperación | Dos solicitudes y una consulta de recuperación convergen en un plan sellado; se relee el inventario esquema 2 y se compara su huella. |
| Confirmación vencida | El reloj de prueba adelantado rechaza antes de publicar el lote. |
| Rollback de confirmación | Tres casos expresos en ANTES_EVENTO, EVENTO_INSERTADO y PLAN_CONFIRMADO. Dentro de la transacción comprueba lote, raíz, archivos, evento y revisión provisionales; después del fallo esperado comprueba cero lotes publicados y plan REVISABLE/revisión 1/huella conservados. |
| Dos confirmaciones y acuse perdido | Claves distintas sobre el mismo plan producen un solo lote. Repetir la confirmación recupera su identidad. Comprueba una raíz, dos archivos pendientes, un evento de confirmación y cero intentos. Correlaciona IDs cliente, raíz, bytes y documento original completo de cada archivo con el inventario sintético. |

Es una prueba directa de repositorio con plan/valores preconstruidos y sello de catálogo de prueba. No acredita resolución operativa de catálogos/proveedores, sesión, permisos HTTP, dos pestañas reales, selección UI o funcionamiento de escritores documentales. El rechazo de los fallos inyectados es parte del caso aprobado; cualquier otro fallo termina el recorrido y exige revisión, sin retry automático ni limpieza.

## Recursos y presupuestos propuestos

| Dimensión | Límite del recorrido |
| --- | --- |
| Datos sintéticos | Una raíz `DOC120-<runId>-SEG`, IDs r1/s1, archivos a1/a2, nombres 01.pdf y 02.pdf; siete bytes declarados cada uno, sin abrir ni transmitir PDF. |
| Recursos confirmados nuevos | Máximo una preparación, un plan, un lote, una raíz y dos archivos de inventario. Tres partes y resultados correspondientes; los fragmentos finales usan el fragmentador existente. |
| Efectos documentales | Cero recepciones, expedientes, documentos almacenados, llamadas SII, reanudaciones, reconciliaciones o borrados. |
| Concurrencia | Hasta dos operaciones de repositorio; cada operación abre una conexión física sin pooling. No mantiene transacciones abiertas entre fases. |
| Admisión | Máximo 48 llamadas de repositorio y 64 SELECT externos de control, incluidos los observadores. Las consultas internas del repositorio y sus metadatos no se cuentan como SELECT externos. |
| Duración | Hasta 15 minutos de admisión. Las operaciones admitidas terminan; un plazo vencido impide admitir la siguiente. Cada SELECT de control tiene timeout de diez segundos. |
| Fallos previstos | Tres confirmaciones revertidas en puntos diferentes, seguidas por el caso de confirmación concurrente. Se conservan huecos de autoincremento y todos los recursos que queden confirmados. |
| Conservación | Sin DELETE, ALTER, restauración, reducción de capacidad o edición de checkpoints. Un resultado parcial conserva recursos, fases e IDs y termina para revisión. |

Estos límites acotan el arnés y **no son una autorización** ni un presupuesto heredado de una corrida anterior. No se dirige a los lotes reales 39, 50, 51 o 52. La tarea 3.187 conserva su ensayo independiente; este recorrido no prueba reserva documental, escritor B activo o cierre de V2.

## Información y ejecutor que faltan antes de autorizar

1. Identificar instancia/base **de laboratorio**, cuenta MySQL y usuario/empresa/nivel/perfil/versión existentes y autorizados para el fixture. El operador debe confirmar aislamiento, esquema compatible y permisos mínimos; no se presumen a partir del localhost de corridas previas.
2. Preparar en almacenamiento local excluido de Git el plan sintético y un manifiesto con esos IDs, huella del destino, cuenta, fuentes, ID nuevo y límites. Sellar sus hashes y revisarlos antes de conectar. Ningún archivo contiene claves o conexiones.
3. Adaptar la consola privada existente como envoltorio de esta entrada: comprobar manifiesto/fixture, consentimiento de la ventana, captura efímera de la clave, journal de un solo uso y rechazo de una ventana ya iniciada/cerrada. No reutilizar el ejecutor DOC-115 sin adaptar: conserva otros recursos y otra autorización.
4. Definir en ese envoltorio los SELECT de procedencia/autoridad y preservación de datos preexistentes con su presupuesto propio, fuera de los 64 controles internos del arnés. Revisar huellas y contabilizar ese preflight por separado.
5. Presentar el manifiesto y ejecutor final para autorización específica de ambiente/cuentas/recursos/fallos/topes. Solo después ejecutar y registrar PASS o STOPPED con SHA, fases, IDs y consumos efectivos.

Se pidió únicamente la identificación del laboratorio, sin contraseñas, mientras se preparaba el código local. Al corte no hay recursos operativos concretados ni aprobación solicitada para una ventana indefinida. **3.118 no se completa por compilación o guardas locales.**

## Verificación de esta preparación

Comando: `node --test tests/doc110-perfil-importacion-behavior.test.cjs`. El archivo incorpora la nueva parcial C# a la compilación Framework, exige la salida de las 16 comprobaciones nuevas y conserva las suites existentes y la comprobación del DDL. Son dos pruebas Node envolventes; los casos internos no se suman como pruebas Node adicionales. No se suman repeticiones de compilación de los cortes intermedios.

Resultado final: **2/2 pruebas Node aprobadas**, compilación VB/C# y ejecución Framework sin conexión, con las 16 comprobaciones internas nuevas. OpenSpec estricto y controles de refinamiento revisados después de actualizar este registro. No hubo HTTP de aplicación, autenticación, SELECT reales, DDL, efectos de negocio o sustitución de DLL.
