# Corte vigente — enlace V2 y driver implementados localmente

Completadas6.9/6.10/D-17/RQ-17:composición de intentos nuevos con revisión de servidor, persistencia V2 y controles existentes; driver compartido admite recuperación con contratos cerrados y sin replay.12 casos nuevos de enlace y12 Node de driver;235/235 Node y145 casos V2 internos incluidos en el harness. Progreso27/37; mismas10 tareas reales/finales abiertas.

Función compartida de almacenamiento, enlace legacy y SII intactos. P1/P2 del plan real resueltos solo en candidato; P3/P4/P5 siguen abiertos. Sin despliegue o ejecución real;1276 HTTP/113 SELECT/9 aperturas sin cambios. Evidencia:[enlace/driver](doc118-enlace-v2-local-20261109.json), [detalle](doc118-enlace-v2-local-20261109.md), [estado del plan](doc118-plan-real-v2-enlace-20261109.md).

## Corte anterior — RV-08 revisión local y plan real preparados

Revisión consolidada de16 requisitos/35 escenarios,21 condiciones de matriz y133 casos V2. Los9 entregables locales V2 quedan documentados; progreso25/35, mismas10 tareas reales/finales abiertas y DOC-118 no listo para archivo. Se reutiliza223/223 Node de RV-07 en e68d9972 con código/pruebas/herramientas sin cambios, comprobados por hashes; no hay una corrida nueva en RV-08.

El plan real propone dos lotes nuevos y límites separados todavía no autorizados. No es ejecutable hasta integrar el escritor V2, extender el driver y revisar el enlace de fallos/calificación del runtime. Sin despliegue/login/HTTP/SELECT reales;1276 HTTP/113 SELECT/9 aperturas sin cambios y41/53/69 protegidos. Evidencia:[revisión V2](doc118-revision-v2-local-20261109.json), [reporte](doc118-revision-v2-local-20261109.md), [plan real](doc118-plan-real-v2-20261109.md).

## Corte anterior — RV-07 matriz local verificada

12 casos nuevos combinan adaptador/repositorio/guardia con barreras explícitas:ejecutor activo, retorno/checkpoint, respuestas perdidas, dos recuperadores, rollback y lector en otro proceso. Cero cambios de producto;133 casos V2 internos y223/223 Node pasan. Progreso24/34, mismas10 tareas reales/finales abiertas; sigue RV-08. SQL/escritor/transacción son dobles, no califica MySQL/IIS ni COORD-02/03 reales.

Dos procesos lectores nuevos cerrados; épocas ajenas, V1 y41/53/69 siguen bloqueados. Consumos1276 HTTP/113 SELECT/9 aperturas sin cambios. Evidencia:[matriz V2](doc118-matriz-v2-local-20261109.json), [detalle](doc118-matriz-v2-local-20261109.md).

## Corte anterior — RV-06 recuperación HTTP implementada

RecuperarDocumentoV2 expone cuatro referencias mediante el guard operativo y composición sin escritor, con autoridad RECONCILIAR y RA/DA fijados. Respuesta de cierre sin invocación separada de confirmación documental.19 casos HTTP nuevos con dispatcher/composición reales compilados, HTTP sintético y guardia real;223/223 Node. Progreso23/33; mismas10 tareas anteriores abiertas. Sin despliegue ni calificación MySQL real; almacenamiento habitual V1.

Consumos1276 HTTP/113 SELECT/9 aperturas sin cambios;41/53/69 protegidos. Evidencia:[recuperación HTTP V2](doc118-recuperacion-http-v2-local-20261109.json), [alcance](doc118-recuperacion-http-v2-local-20261109.md).

## Corte anterior — RV-05 recuperación interna implementada

Cierre negativo V2 limitado y confirmación por IDs observados, sin escritor, bajo guardia y vista comparada hasta commit/rollback.22 casos nuevos con guardia real/SELECT y transacción simulados;223/223 Node. Progreso22/32, mismas10 tareas anteriores abiertas. No acredita MySQL real ni activa HTTP.

Sin sesiones, HTTP de negocio, SELECT reales o efectos nuevos. Consumos1276 HTTP/113 SELECT/9 aperturas,69 protegido y sin backfill. Evidencia:[recuperación V2](doc118-recuperacion-v2-local-20261109.json), [alcance](doc118-recuperacion-v2-local-20261109.md).

## Corte anterior — RV-04 persistencia implementada y validada localmente

Callbacks V2 en el repositorio existente; acuses después de Operar/commit, gate único, diario concordante y checkpoint/evento/consolidación juntos.20 casos SQL nuevos integrados con el adaptador sobre doble transaccional;223/223 Node y DDL ampliado válido. Progreso21/31; mismas10 tareas anteriores abiertas. No acredita MySQL real ni activa composición pública o recuperación.

Sin sesiones, HTTP de negocio, SELECT o efectos documentales nuevos. Consumo1276 HTTP/113 SELECT/9 aperturas e intento69 protegido, sin backfill. Evidencia:[persistencia V2](doc118-persistencia-v2-local-20261108.json), [alcance](doc118-persistencia-v2-local-20261108.md).

## Corte anterior — RV-03 implementada como recorrido interno

Retención antes del inicio V2, acuses concordantes y observación de IDs en finally, sin fallback V1.20 casos nuevos del adaptador con staging real y dobles explícitos de persistencia/escritor;223/223 Node actuales. Progreso20/30 por nueva6.3 completada; las10 tareas anteriores siguen abiertas. Composición pública/MySQL y recuperación permanecen pendientes.

No hubo sesiones, HTTP de negocio, SELECT o efectos documentales nuevos. Consumo1276 HTTP/113 SELECT/9 aperturas e intento69 protegido, sin replay. Sigue RV-04, persistencia y diario transaccional. Evidencia:[adaptador V2](doc118-adaptador-v2-local-20261108.json), [alcance](doc118-adaptador-v2-local-20261108.md).

## Corte anterior — RV-02-B implementada y validada localmente

Puerto de retención/guardia V2 en el proveedor existente, sin activar recuperación.21 casos nuevos en el harness,3 procesos sintéticos cerrados;223/223 Node actuales. Otra época del proveedor —incluido reinicio— permanece bloqueada. Progreso19/29 tras añadir y completar6.2; las10 tareas anteriores siguen pendientes.

No hubo sesiones, HTTP de negocio, SELECT o efectos nuevos. Consumo1276 HTTP/113 SELECT/9 aperturas e intento69 protegido, sin backfill. Sigue RV-03, retención desde el adaptador. Evidencia:[puerto V2](doc118-puerto-staging-v2-local-20261108.json), [alcance](doc118-puerto-staging-v2-local-20261108.md).

## Corte anterior — RV-01 implementada como contrato interno

La tarea nueva6.1/D-11/RQ-11 fija documentos V2 de inicio, autorización y retorno, sin conectarlos al flujo.19 casos V2 pasan dentro del harness existente; regresión18/18 y herramientas205/205. El avance es18/28 por una tarea nueva completada; las10 tareas reales/finales anteriores permanecen abiertas. Leer JSON válido no prueba procedencia durable ni habilita invocación/recuperación.

No hubo nuevas sesiones, HTTP de negocio, SELECT o efectos documentales. Intento69 protegido e inelegible; consumo real1276 HTTP/113 SELECT/9 aperturas. Siguiente:RV-02-B, puerto del proveedor. Evidencia:[contratos V2](doc118-contratos-v2-local-20261108.json), [alcance](doc118-contratos-v2-local-20261108.md).

## Corte anterior — RV-02-A validada localmente

La comprobación de primitivas de staging pasó13/13 pruebas con6 procesos sintéticos y limpieza confirmada. Acredita exclusión local en NTFS e identifica la necesidad de conservar el handle y aplicar fencing durable; no implementa ni califica el proveedor de producción. Regresión conjunta205/205. RV-02-B continúa pendiente. Contrato y progreso activos:17/27.

No hubo nuevas sesiones, HTTP, SELECT o efectos de negocio. El intento69 sigue protegido y los consumos permanecen1276 HTTP/113 SELECT/9 aperturas. Evidencia:[prueba de staging](doc118-staging-lock-probe-20261108.json), [alcance y límites](doc118-staging-lock-probe-20261108.md).

## Corte anterior — ausencia física actual y contención acreditada

La auditoría SELECT de lote24/archivo40/expediente670 encontró0 registros en gabinete/inventario/índice. Intento69 sigue EN_CURSO y reclamado, sin checkpoint; no se libera ni repite. Archivo41 NO_INICIADO y sin documento físico. Snapshot estable. Acumulado1276 HTTP/113 SELECT/9 aperturas. Se acredita4.6 como contención, sin resolver69:17/27.

La liberación del depurador pasó en un proceso sintético propio, con peer en ejecución y cierre confirmado; no califica legacy/ASMX. Regresión192/192. Detalle:[auditoría y probe](doc118-auditoria-fisica-liberacion-20261108.md). La decisión de alcance para evaluar recuperación EN_CURSO está propuesta, sin aplicar.

## Cortes anteriores

# Corte vigente — intento69 activo, sin replay permitido

La continuación autorizada recuperó el lote24 y alcanzó RUN02; el observador falló durante liberación. Auditoría SELECT posterior:archivo40/intento69 EN_CURSO con reclamación activa y sin resultado durable; archivo41 NO_INICIADO y sin documento físico. Snapshot estable. Consumo1276 HTTP/95 SELECT/9 aperturas;0 aperturas disponibles. El efecto físico A permanece sin confirmar. C03 no iniciado. Limpieza del runner/observador/auditor confirmada. Corrección candidata local del observador y regresión187/187; no calificada en runtime. Progreso16/27.

Detalle:[ventana y auditoría](doc118-ventana-activa-auditoria-20261108.md). Evidencia:[parada](doc118-active-window-stop-20261108.json), [auditoría](doc118-select-audit-20261108.json).

## Cortes anteriores

# Ampliación autorizada para recuperar el lote24

El usuario confirmó con «continua» la propuesta de9 aperturas/1500 HTTP acumulados (1425/75), conservando recursos y consumos. La petición HEAD de disponibilidad pasó HTTP200 y elevó el acumulado de995 a996; siguen40 SELECT y7 aperturas. El worker31784 tiene observador externo ATTACHED con sourceBound=true. La consola local enmascarada está abierta, pendiente de captura; este corte no registra otro login ni solicitudes documentales. La recuperación del lote24 y la calificación Freeze/Thaw siguen pendientes. Regresión177/177; progreso16/27. Evidencia de disponibilidad: `doc118-prepared-recovery-readiness-20261108.json`.

## Cortes previos

# Corte actual — lote24 conservado, antes del primer almacenamiento

La autorización1200 HTTP/7 aperturas fue aplicada tras «confirmado». PREFLIGHT y PREPARE02 pasaron; RUN02 se detuvo antes de despachar ALMACENAR. Lote24:dos PDFs recibidos/materializados y expedientes670/671;0 documentos almacenados solicitados por esta corrida. Consumo995 HTTP/40 SELECT/7 aperturas, sin capacidad de otra apertura. Finally y limpieza del observador confirmados. ARM/RESUME/CLOSE posterior pasó sin escritor ni efectos; Freeze/Thaw y progreso del peer aún no calificados. Progreso16/27.

Detalle y límites: [preparación y recuperación](doc118-preparacion-recuperacion-20261108.md), [corte saneado](doc118-prepared-stop-20261108.json), [ARM externo](doc118-observer-arm-20261108.json). Regresión local177/177. Recuperación implementada por lecturas del lote existente; propuesta9 aperturas/1500 HTTP pendiente, sin aplicar.

## Cortes anteriores con su alcance original

# DOC-118 — preflight autenticado y bloqueo del observador

El preflight de dos sesiones pasó con publicación38/gabinete10 y17 SELECT. Consumo acumulado699 HTTP C02,0 C03 y5 aperturas;0 fases documentales y0 efectos. El worker26244 dejó de estar activo por causa no determinada. La consola terminó sin reconocer la orden CLOSE ni el finally; no quedan consola/shell ni hijos directos detectados. Las instancias privadas de depuración ya no están presentes y los dos IDE originales quedaron sin depuración activa, con12 breakpoints del usuario conservados. La observación interna sigue SIN_CALIFICAR. Evidencia: doc118-authenticated-preflight-20261107.json.

## Autorización y consumo

La ampliación1000 HTTP acumulados (925/75) y5 aperturas fue autorizada por el usuario y aplicada conservando425 HTTP/3 aperturas anteriores. La pareja nueva autenticó sin retries y consultó lote23 desde contextos independientes. El preflight comprobó publicación, gabinete, propietario y obtuvo baselines de lotes18/21 mediante17 SELECT de control. No se realizó una comparación posterior ni hubo solicitudes mutantes a esos lotes.

| Concepto | Consumido | Disponible |
| --- | --- | --- |
| HTTP C02 |699 de925;3 ConsultarLote acumuladas |226 |
| HTTP C03 |0 de75 |75 |
| HTTP global |699 de1000 |301 |
| Aperturas/intentos asignados |5 de5 |0 |
| SELECT de control |17 de240 |223 |
| Planes/lotes/raíces/PDFs/expedientes/documentos nuevos |0 | Límites originales |
| Detenciones y congelaciones |0 | Sin ejecutar |

La política de omisión evitó223 subrecursos antes de despacho. No son reembolsos de los425 HTTP anteriores ni evidencia de QA visual. La pareja consumió272 HTTP nuevos. Los archivos de entrada siguen locales e ignorados y no se conservaron datos de la captura en los informes.

## Observador externo

La lectura CodeView/MSF confirma GUID y edad coincidentes entre DLL/PDB ordinarios en disco, conservando sus hashes iniciales. Se corrigió el lector local para el mapa de directorio que abarca varios bloques de512 bytes. Esto no verifica el módulo cargado.

El IDE del repositorio tenía12 breakpoints habilitados. Se usaron instancias privadas vacías para no adoptar esa configuración. Una instancia adjuntó al worker26244 y resolvió un breakpoint False dentro del cuerpo ordinario; se cerró antes de efectos. Otra inicialización falló en BIND_SOURCE y no reconoció su limpieza; la inspección posterior no encontró la instancia privada ni depuración en los IDE originales. La última inicialización cerró en FIND_TARGET_PROCESS; la lectura de IIS confirmó que ya no había worker. No se determinó la causa de su salida; no atribuirla al idle, al depurador ni a un reciclado sin evidencia.

No hubo ARM documental, solicitud ALMACENAR ni Freeze/Thaw. El controlador y el adaptador permanecen candidatos: compilar, adjuntar y resolver un breakpoint no acreditan que otro cliente responda mientras un hilo está retenido. No se modifica almacenamiento compartido, SII, fuente VB, DLL/PDB, configuración o permisos de IIS.

## Salida de consola y aceptación

El último estado del proceso es PREFLIGHT_PASS. Se envió CLOSE con secuencia2, que quedó sin consumir; el proceso y su shell ya no existían al verificarlos y no se observaron hijos directos. El informe no afirma cierre protocolario de contextos ni borrado explícito por finally. La ausencia de la consola y del worker obliga a revisar una nueva ejecución; los contadores no se reinician y ya no quedan aperturas autorizadas.

Progreso16/27. COORD02/03 específicos permanecen NO_EJECUTADOS. En ese corte la CLI solo implementaba PREFLIGHT; la continuación local posterior implementa las siete fases. Regresión local:130 pruebas (12 preflight,31 enlace,16 coordinación,63 drivers,4 símbolos,4 adaptador externo), separada de la evidencia real. El controlador C# compila contra el SDK instalado; no tiene calificación de retención de hilo en runtime.


## Continuación local y comprobación del observador

Las siete fases del ejecutor están implementadas y verificadas localmente. El observador real pasó disponibilidad, breakpoint de fuente, INSPECT y CLOSE con ack de limpieza y salida0 en sesión supervisada. No se ejecutaron ARM/Freeze/Thaw, fases documentales ni nuevas sesiones autenticadas. Consumo acumulado699 HTTP C02,0 C03,17 SELECT y5 aperturas;0 aperturas disponibles. Progreso16/27: COORD02/03 siguen NO_EJECUTADOS para aceptación.

El detalle de la implementación está en doc118-ejecutor-fases-local-20261108.md. La consola previa sigue sin ack de finally; el controlador nuevo sí reconoció su cierre tras corregir el guardado atómico. La disponibilidad se comprobó sin autenticación. La calificación de retención y las ventanas de negocio siguen pendientes. En ese corte la propuesta de1200 HTTP/7 aperturas no estaba aplicada; la sección inicial registra su autorización y consumo posteriores.
