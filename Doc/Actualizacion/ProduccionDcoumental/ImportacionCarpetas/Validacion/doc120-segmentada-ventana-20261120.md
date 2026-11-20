# DOC-120 — ventana aprobada de persistencia segmentada

Usuario: «aprobado continua», tras confirmar reutilización de E2E y corpus `D:\Pruebas\Importador\Egresos_20261113_092045_39126bb4`. Fecha: 2026-11-20. Origen D-05/RQ-05, D-08/RQ-08 y D-10/RQ-10. Esta autorización sustituye la espera de identificación de recursos del plan local; no concede carga completa, cambios de perfil, esquema, permisos, DLL ni efectos documentales.

Ventana nueva: `92158d88c6d94d328c5af9f2cb562d3d`. Recursos de la configuración E2E DOC-115 existente: propietario 141, empresa 2, nivel 7, perfil 24 y gabinete 10, DSN registrado de pruebas MembershipUsers. Se lee la publicación vigente del perfil; si no está activo o no corresponde al recurso, se detiene sin activarlo ni elegir otro. Destino MySQL ligado a la huella del escenario existente. Ninguna clave se obtiene de archivos o procesos.

Se usan nombres y tamaños de dos PDF de la subcarpeta ya conocida de 1.000 PDF; total seleccionado **89.570 bytes**. No se copian, alteran o envían sus contenidos. El manifiesto privado excluido de Git conserva rutas, nombres y SHA-256 de ambos, la aprobación y las huellas de **145 fuentes/dependencias**. Check local: `PASS_NOT_EXECUTED`.

## Ejecutor y límites

Se extienden los ejecutores existentes, sin crear otro ejecutor: `run-doc115-recepcion-importacion.cjs --repository-segmented --session <manifiesto>` invoca `run-doc111-persistence.ps1 -Mode Doc120Segmented`, que reutiliza `support/doc115-repository-integration.ps1`. Conserva las entradas previas y su autorización independiente. El rollback usa el núcleo común DOC-111/115; la ampliación específica cubre partes y sellado.

- Máximo una preparación, un plan esquema 2, un lote, una raíz y dos archivos de inventario. Tres partes; fragmentos de validación y sellado producidos por el código existente.
- Tres fallos previstos de confirmación: antes del evento, después del evento y después de actualizar el plan, dentro de la transacción. Después, confirmación concurrente y recuperación del acuse.
- Máximo dos operaciones simultáneas y 48 llamadas del arnés. Hasta 64 SELECT de control del arnés y 96 del envoltorio, contados por separado; los SELECT internos del repositorio no se confunden con ellos. Cada etapa admite nuevas operaciones hasta 15 minutos y deja terminar las ya iniciadas.
- Cero HTTP de aplicación, recepciones PDF, creación de expedientes, guardado documental, SII, reconciliación, limpieza, reanudación administrativa o DDL. No interviene en lotes existentes.
- Huellas antes/después de registros preexistentes, perfiles, versiones, reglas, permisos y conteos documentales. Incluye preparaciones previas y sus partes/resultados. Recursos nuevos conservados; sin DELETE ni restauración.
- Journal de un solo uso y consola con cuatro entradas privadas: cuenta/clave SELECT y cuenta/clave de escritura de laboratorio. No se vuelve a pedir la aprobación de negocio ya concedida. Un fallo termina la ventana, sin retry automático. Los secretos no se registran ni imprimen; se eliminan al terminar.

El fixture de repositorio utiliza una raíz nueva identificada por la ventana y normaliza la marca local de fecha a cero. Sus bytes se calculan desde los dos metadatos seleccionados; la guarda permite hasta 2 MiB por archivo. No se atribuye a ese fixture evaluación del proveedor, sesión HTTP, selección UI o envío binario.

## Comprobación previa y resultado

**31/31 pruebas Node locales** en las tres suites existentes `doc110-perfil-importacion-behavior`, `doc115-07a-runner` y `doc115-recepcion-contracts`. Incluyen compilación VB/C#, ejecución Framework sin conexión, 16 comprobaciones internas del arnés y tres casos nuevos del envoltorio: autorización, check sin prompts y limpieza/no repetición después de fallo. Los 16 internos no se suman como otras 16 pruebas Node. Los modos anteriores conservan sus pruebas.

Estado inicial: ejecutor verificado y ventana aprobada; credenciales todavía no introducidas, cero accesos reales al preparar este registro. El estado efectivo se conserva en el journal local `tools/e2e/artifacts/doc120-segmented-92158d88c6d94d328c5af9f2cb562d3d.json` y el estado de la consola junto al manifiesto excluido. La apertura de consola no acredita PASS ni completa 3.118.

3.118, 3.187, aceptación y demás tareas mantienen su criterio original hasta observar el resultado. La aprobación de esta ventana no completa la autorización de capacidad de 1.000 documentos de 5.2 ni permite repetir automáticamente una corrida detenida.

## Primera ventana detenida y corrección del adaptador

La ejecución `92158d88c6d94d328c5af9f2cb562d3d` concluyó **STOPPED_REVIEW_REQUIRED**, fase REGISTERED_CONNECTION, código DOC115_AUTHORIZED_DATABASE_CHANGED. Completó 29 SELECT del envoltorio y confirmó el perfil 24 activo/publicación 38 y el nivel 7; no llegó al arnés mutante ni registró preparaciones, planes o lotes. La consola quedó STOPPED y credentialsCleared=true. El journal cerrado y sus fuentes se conservan intactos.

El adaptador construía el destino MySQL desde las propiedades DataSource/Database de la conexión ODBC. La inspección posterior de únicamente SERVER y DATABASE del DSN registrado, sin conexión ni claves, coincide con la huella aprobada. La corrección utiliza exclusivamente esos dos campos, valida la misma huella y rechaza cualquier definición discordante del DSN; no cambia el destino ni relaja la guarda. 30/30 casos del adaptador/contratos pasan, incluido un caso nuevo de resolución sin acceso y rechazo de destinos ausentes/ajenos. Los dos casos Framework anteriores conservan su resultado propio.

Continuación preparada `1a31695b3e0c4a9c9f559a205d2dcf3c`: mismos recursos y efectos, nuevos ID/huellas y consola de un solo uso. Check PASS_NOT_EXECUTED. Liga el journal anterior por SHA-256 y exige su parada anterior a mutaciones y claves eliminadas; arranca con los **29 SELECT ya consumidos**, conservando el máximo acumulado de 96 del envoltorio y 64 nativos. No reabre ni repite la ventana detenida. La consola nueva se abre mediante aprobación explícita de esa apertura; no se declara ejecución o PASS al prepararla.
