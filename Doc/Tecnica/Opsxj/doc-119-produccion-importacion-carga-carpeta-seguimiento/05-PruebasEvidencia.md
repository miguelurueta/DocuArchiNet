# DOC-119 — evidencia de validación

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Base del primer corte local: 94ca9277ce234b337b73bdda9bbd881e037b3047, con cambios locales descritos.

Estado vigente del 2026-11-12: **SEG-01 PASS_READONLY integrado**, tarea2.4 completa; **7/24 tareas**. Corrida ccb25ec9, lote29/plan45: filtros, segunda página, CSV, vacío, privacidad con Carolina136 y huellas idénticas;384HTTP/29SELECT/2sesiones. SEG-02 detenido en precondición de rama original, antes de reselección y sin huella posterior; no se abre2.7. [Evidencia saneada](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-seg01-aceptado-20261112.json) y [detalle/corrección de recuperación](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-seg01-aceptado-recuperacion-pendiente-20261112.md).

Antecedente histórico: corrida08e8608d, privacidad PASS y SEG-01 PASS_READONLY_WITH_LIMITS por falta de segunda página; SEG-02 omitido por decisión0. [Evidencia previa](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-privacidad-20261112.json). Los cortes siguientes anteriores a ccb25ec9 conservan sus estados históricos.

Actualización posterior: el usuario autorizó expresamente la preparación acotada de datos y confirmó operar sin limpieza. Se completan1.2 y2.6; **6/24 tareas**. SEG-03 no ejecutado; originales/partes/historial se conservan. El ejecutor específico y su transporte pasaron9/9 pruebas de límites; `--check` comprobó las27 copias locales sin red. Estas pruebas no acreditan todavía efectos reales ni SEG-01/02: la ejecución autenticada es el siguiente paso.

La preparación autenticada4ffe5447 se detuvo posteriormente en PrepararPlan con400/SOLICITUD_INVALIDA,186HTTP/3SELECT/1sesión; no envió confirmación, recepción ni creación. La validación offline del DTO/manifiesto con el ensamblado local pasó; la causa del rechazo del pipeline sigue pendiente del diagnóstico SELECT. Las ventanas SEG-01/02 no comenzaron. Transporte con clasificación cerrada de errores:10/10 pruebas locales. [Detalle y límites](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-preparacion-400-20261112.md). Progreso6/24; no abrir2.7.

Diagnóstico posterior completado:4SELECT/0HTTP/0sesiones adicionales, sin planes recientes del alcance; cerrado y credenciales eliminadas. El perfil40 deriva una referencia inválida001 del PDF anterior y un código común DOC119 de las raíces anteriores. Preparación local revisión2 corregida y comprobada con reglas publicadas/ensamblado actual:27 PDF resuelven referencia79, dos códigos distintos y fechas válidas. La reselección distingue copias por nombre+ruta. Sin nueva ejecución real ni aceptación inferida; no cubre raíces físicas homónimas. [Revisión2](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-datos-revision2-20261112.md).

## Evidencia requerida

Inspección posterior `eb09fc5981be453b936b2bf1714c43a3`: cerrada con 5 SELECT/0 HTTP/0 sesiones, credenciales eliminadas. Los 24 lotes propios inspeccionados tienen máximo3 archivos frente a página25; los cinco lotes con recepción pendiente no tienen raíces confirmadas para SEG-02 ni originales coincidentes en el alcance local examinado. Se preparó únicamente en disco local un inventario de27 PDF; los efectos de aplicación siguen pendientes de autorización. [Propuesta de datos y alcance](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-datos-aceptacion-propuesta-20261112.md). SEG-03 tiene una propuesta separada de operar sin limpieza, pendiente de decisión expresa; consumo observado57 archivos/24.371 bytes. Progreso4/24; compuerta2.7 cerrada.

1. Corte previo de refinamiento sobre d7dbf201: 16/16 pruebas cliente PASS, sin cambios de producto en aquel corte.
2. Reproducción DOC-119: dos pruebas nuevas fallan por filtro inexistente antes de cambiar producto.
3. Corrección de filtros/CSV: `node --test tests/doc111-importacion-carpetas-client.test.cjs tests/doc110-perfil-importacion-behavior.test.cjs`: 20/20 PASS (18 cliente y dos del arnés Framework/DDL).
4. Después de añadir Enter sin submit implícito: `node --test tests/doc111-importacion-carpetas-client.test.cjs`: 18/18 PASS. Backend no se modificó y no se repitió; no se suman estas corridas como casos distintos.
5. Auditoría de refinamiento y OpenSpec estricto PASS; comprobación de sintaxis JS y diff sin errores.

Las dos pruebas nuevas usan Chromium/cliente/vista reales y servicios sintéticos; verifican filtros/página/CSV, limpieza, separación de filtros de carpetas/archivos, denegación sin exportación obsoleta, Enter sin postback y viewport de 390 px. El CSV sintético verifica transporte/selección; CeldaCsv productivo se comprueba en el arnés backend existente. No atribuir el CSV simulado a una ejecución real del servicio.

Informes: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-procedencia-20261111.md` y `doc119-filtros-local-20261111.md` en el mismo directorio. Los antecedentes DOC-111/116/117/118 conservan su código/runtime/ambiente y no se reetiquetan como DOC-119.

## QA/E2E WebForms — corte histórico anterior a la ejecución real

No ejecutados por DOC-119: autenticación real, HTTP de aplicación, SELECT/MySQL reales, recepción/carga, almacenamiento, inyección de fallos, cambios de permisos, limpieza o despliegue. El plan de aceptación preparado aún requiere ambiente/cuentas/lotes y autorización específica.

Tareas verificadas: 1.1, 2.1, 2.2, 2.3; 4/24. SEG-01/02 siguen sin aceptación integrada; 1.2 abierta para concretar plan. SEG-03 sigue condicionado. No se reorganizan accesos antes de compuerta 2.7, no se solicita cierre ni se marca evidencia manual como aprobada.

## Preparación posterior al «autorizado» del usuario

Ventanas de lectura del plan local autorizadas; cuentas/recursos se capturan por consola enmascarada antes de actuar. Selector específico preparado con sesión/SQL existentes y límites persistentes, sin habilitar efectos. `doc119-selector-lectura-20261111.md` detalla fuentes y límites.

Regresión nueva: **34/34 PASS** (21 cliente/selector y 13 políticas), incluyendo los tres recorridos del selector con servicios simulados; `--check`, sintaxis Node, OpenSpec estricto y refinamiento PASS. No acredita todavía una ejecución real. Progreso permanece 4/24 hasta revisar preflight y resultados con referencias concretas. Estado de ejecución se conserva solo en .tmp, excluido de Git.

## Selección comprensible — 2026-11-12

El selector ahora muestra importaciones existentes de la cuenta autenticada por fecha, destino y estado, sin exigir IDs internos. Descubrimiento parametrizado y limitado a usuario/empresa/facultades actuales; sesión y consultas cuentan dentro de los presupuestos existentes. La captura anterior se reemplazó antes de cualquier sesión/HTTP/SELECT, conservando su estado local.

Regresión actual: **39/39 PASS** (21 cliente/selector y 18 políticas), sin sumar corridas anteriores. Incluye selección por opción, guardia sin recurso elegido, descubrimiento acotado, cuenta ambigua y archivo pendiente. Sigue sin acreditar aceptación integrada real; avance 4/24. Detalle en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-seleccion-importaciones-20261112.md`.

## Primer intento real y corrección del ejecutor — 2026-11-12

Corrida `d09ca8a7cdc547cca79fb53d37af26af`: STOPPED, guardia `DOC119_HTTP_SCOPE_DENIED`, error exterior `DOC111_LOGIN_TRANSPORT_FAILED`. Consumo: 1 sesión, 1 HTTP, 0 SELECT; postback no observado, sin importación elegida ni aceptación SEG-01/02. Secretos eliminados al cerrar. Diagnóstico posterior sin credenciales: 1 GET, 0 sesiones/SELECT; primera solicitud bloqueada `GOOGLE_FONT_STYLESHEET`. Ambos registros se conservan excluidos de Git.

Corrección: abortar localmente la hoja opcional exacta de Google Fonts sin enviar tráfico externo ni detener la guardia; el resto de los límites sigue vigente. Fallos de guardia se informan con su código propio. Regresión **40/40 PASS** (21 cliente/selector, 19 políticas), incluida omisión sin consumo de red y rechazo de POST a esa misma URL. No se suma a los cortes anteriores. El usuario pidió continuar la prueba real; se prepara nueva ventana manual conforme al plan, sin retry automático ni aceptación inferida. Progreso sigue 4/24.

## Segundo intento y preflight de acceso — 2026-11-12

Corrida `42a55db64044453c8c6cbac63109702a`: STOPPED, 93 HTTP/1 apertura de autenticación/0 SELECT; sin aceptación SEG-01/02. Diagnóstico sin credenciales: mismo bloqueo POST XHR al iniciar la página, 93 HTTP/0 aperturas de autenticación/0 SELECT. La consulta automática ObtenerEstado del cliente 2FA faltaba en la guardia. Se permite solo JSON vacío para esa operación y GET/HEAD al destino local fijo del login; ninguna mutación 2FA se habilita. Se conserva observación de login al priorizar errores de guardia y se añade diagnóstico por categorías fijas sin datos privados.

Preflight real corregido sin credenciales: **PASS_LOGIN_PAGE_ONLY**, HTTP 200, controles presentes, una consulta ObtenerEstado y ninguna violación, con 95 HTTP/0 aperturas de autenticación/0 SELECT. Huella SHA-256 del helper de ese preflight: `53fccc5e80b84a04bea291b683afcf823f6934dbd37a825bce572ef93cda1e73`. Estado en `.tmp/doc119-seguimiento/42a55db64044453c8c6cbac63109702a/login-status-preflight.json`, excluido de Git. No acredita autenticación ni consulta de importaciones.

El presupuesto HTTP original resulta insuficiente para el recorrido completo. El plan propone 600 HTTP por ventana con el resto de límites intactos; pendiente de autorización expresa, sin cambiar todavía los topes del código ni iniciar una nueva corrida. Regresión local actual: **42/42 PASS** (21 cliente/selector y 21 políticas), sin sumar cortes anteriores. Progreso 4/24.

## Ampliación aprobada — 2026-11-12

El usuario autorizó expresamente nueva corrida con 600 HTTP por ventana, 2 sesiones y 80/60 SELECT, sin ampliar efectos. Se configuran los nuevos topes y se registra su revisión en cada estado de ejecución. Los registros previos permanecen intactos. Política del ejecutor: **21/21 PASS**, incluida detención al agotar presupuesto; sintaxis Node, `--check`, diff y OpenSpec estricto PASS. No se repite la suite cliente sin cambios ni se suman corridas previas. No se atribuye resultado real a esta autorización. Progreso 4/24 hasta revisar la ejecución.

## Acceso posterior a ampliación — 2026-11-12

Corrida `c10eeb796fd147ee9742712916622cd2`: STOPPED por servicio POST fuera de alcance durante la página de inicio; 137 HTTP, 1 apertura de autenticación, 0 SELECT. Postback 200/COMPLETE, campos MATCHED y cuerpo BODY_NOT_RETAINED; Producción no observada, sin importación elegida ni huellas de negocio. Secretos eliminados al cerrar. No acredita autenticación al módulo, invariancia documental o SEG-01/02.

Corrección del ejecutor: conservar login compartido/cookie real y resolver localmente solo la página intermedia fija de inicio; abrir después Producción real. Se verifican las seis dependencias CDN exactas del ASPX y se cargan sin encabezados privados, contabilizadas en el mismo presupuesto. Se mantiene rechazo de servicios adicionales, efectos y variantes de las URLs. `access.homeSkipped` y códigos fijos de dependencias se conservan en el estado. El ASPX se incluye en evidencia de código local.

Preflight externo sin credenciales: las seis dependencias responden 200, con SHA-256 en `.tmp/doc119-seguimiento/c10eeb796fd147ee9742712916622cd2/static-preflight-network.json`; 6 HTTP, 0 aperturas de autenticación/SELECT. Se conserva el fallo previo de transporte del sandbox en otro archivo (1 intento contabilizado). Son lecturas públicas estáticas, sin aceptación funcional.

Regresión: **45/45 PASS** (21 cliente/selector y 24 políticas). Una nueva prueba Chromium en modo offline recorre el login compartido, espera la navegación intermedia, conserva la cookie sintética y abre la página sintética de Producción sin ejecutar el panel; otras dos comprueban dependencias exactas, eliminación de encabezados privados, conteo y variantes denegadas. Sintaxis Node, `--check`, diff y OpenSpec estricto PASS. Estas pruebas no se atribuyen al ambiente real.

Preflight final del login real sin credenciales, con el helper corregido: HTTP 200, controles presentes, una consulta ObtenerEstado y ninguna violación; 95 HTTP/0 aperturas de autenticación/0 SELECT. Huella del helper: `2ae98826010297371dc4941a13382b42cdb157d9a3c6afbf07936a1335c0a01e`; registro `login-corrected-preflight.json` en el directorio local de la corrida. Sigue sin acreditar acceso autenticado a Producción o SEG-01/02. Progreso 4/24; nueva consola identificada por runId y bajo control manual del operador para capturar credenciales.

## Consulta tras espera prolongada — 2026-11-12

Corrida `cb66698a07b644cf972012a2c738d5b1`, ejecutor b3d10adec57a0b67bb82b68ee5095e468955cc43: acceso a Producción y cliente servido verificados; 24 importaciones propias encontradas. El operador eligió opción 20/lote5 y omitió recuperación con 0. Preflight de tres archivos/dos carpetas, huella previa `0e2efb282feeb3cfcdccb1efc999552e0499c96abf58ea3825b77c67fd1a40bd`. Resultado STOPPED/DOC119_STOPPED; 185 HTTP, 9 SELECT y 1 sesión, sin resultados completos. Secretos eliminados. Sin huella posterior, CSV, negativo de cuenta ajena o aceptación SEG-01/02.

Causa confirmada: el listado se mostró a las 13:26:23 UTC y la selección llegó a las 14:30:23 UTC. sessionState timeout=10 minutos, sin cambiar configuración. IIS registra POST ConsultarLote 401 a las 14:31:43 UTC. El ejecutor esperaba recursos que el cliente no solicitaría después de ese rechazo y ocultaba el timeout como error genérico. Omitir recuperación no causó el fallo.

Corrección: clasificar el rechazo previo, controlar la edad de la sesión antes de SELECT/reutilización, guardar omisión de SEG-02 antes de ejecutar SEG-01 y permitir evidencia parcial de filtros/CSV/vacío/negativo/huellas con pagination=false si no hay segunda página. La compuerta sigue cerrada sin aceptación completa. --reuse-selection conserva únicamente la elección explícita de una corrida cerrada, con recursos revalidados bajo la cuenta nueva, sin restaurar secretos ni consumos. Pruebas locales nuevas cubren 401, rechazo 200, CSV filtrado de primera página con paginación pendiente, espera de sesión y selección reutilizada; no se atribuyen a aceptación real.

Verificación: **51/51 PASS** (24 cliente/selector y 27 políticas), sintaxis Node, --check, diff y OpenSpec estricto PASS; Web.config y producto sin cambios. `ownerChecks` conserva el resultado del recorrido propietario como pendiente del negativo/huellas posteriores, sin aceptación anticipada si la segunda cuenta falta o falla. La corrida anterior permanece cerrada, sin reautenticación ni retry automáticos. Progreso 4/24; nueva consola manual conserva selección 20/0 dentro del presupuesto aprobado.

## Recorrido propietario real y límite de segunda cuenta — 2026-11-12

Corrida `4b9c2748b0b54fc1a41e2a91b8c61447`, ejecutor e05df936d75d9525a435ca35369910ff7e2291f3; selección conservada desde la corrida anterior, revalidada con cuenta/empresa/facultades actuales: lote5 y recuperación omitida. **Recorrido propietario real comprobado**: consulta de tres archivos, filtros, descarga CSV de una fila de la página filtrada y filtro vacío. La segunda página no existe para el filtro/datos disponibles: pagination=false, INSUFFICIENT_EXISTING_ROWS. No se crearon recursos ni se alteró el tamaño de página del producto para simular paginación.

La segunda cuenta completó postback y apertura de Producción con HTTP 200, pero no dispuso del control de importación; la sonda AbrirImportacion recibió 403. Resultado **STOPPED/DOC111_IMPORT_ACCESS_DENIED**, con 375 HTTP, 9 SELECT y 2 sesiones. No alcanzó los casos de denegación por lote/CSV de runSeg01Other. Este rechazo al importador no se convierte en aceptación de aislamiento entre propietarios.

`ownerChecks` conserva los resultados como PENDING_NEGATIVE_AND_AFTER_CONTROLS; SEG-02 consta NOT_EXECUTED_RESOURCE_NOT_SELECTED. Sin controles SELECT posteriores, no se acredita invariancia de negocio ni aceptación completa SEG-01. Corrida cerrada y claves eliminadas; registros excluidos de Git, sin cuerpos CSV ni cuentas en este informe. Se necesitan una cuenta ajena habilitada para importar y una nueva ejecución manual para completar el negativo y los controles posteriores, conservando las limitaciones de paginación/recuperación. Progreso **4/24**; compuerta UI cerrada.

## Preparación del inventario de segunda cuenta solicitado

Corrida principal actual `f6e3fc02cdd940a0bef47a152a38ae67`: esperando segunda cuenta con recorrido propietario conservado localmente, 194 HTTP/9 SELECT/1 sesión. El usuario solicitó identificar una cuenta elegible. Inspector preparado con máximo dos SELECT vinculados al lote actual; excluye propietario, empresa ajena, usuario inactivo, consulta pública y permisos/identidades ambiguos. Proyecta solo IDs y login de candidatas; no consulta claves ni cambia permisos.

Verificación local del inspector: **3/3 PASS**, sintaxis Node y exclusión Git del directorio de resultados comprobadas. Casos: origen/runId, consultas parametrizadas y proyección mínima, falta de candidatas, discordancia de empresa/propiedad/permiso/estado e identidad inválida. El ejecutor principal y el producto no cambian en este corte; no se repiten ni suman sus 51 pruebas anteriores. Inspección real aún pendiente de acceso MySQL en consola auxiliar; ningún nombre se atribuye todavía a permiso vigente. Progreso 4/24.

## Inventario real de segunda cuenta — 2026-11-12

Inspector f55a2c1097e6023b55be8eb28c78a4a08be782b4, corrida `31f399c43f264242af8e7edeffc3cb7a` vinculada a `f6e3fc02cdd940a0bef47a152a38ae67`: **NO_ELIGIBLE_SECOND_ACCOUNT**, 2 SELECT/0 HTTP/0 aperturas de autenticación de aplicación. Lote5, propietario141 y empresa2 resueltos desde datos actuales. Cero candidatas distintas del propietario con identidad activa/única, perfil único y permiso IMPORTAR_CARPETAS=1 en la misma empresa. No se deduce vigencia desde los nombres históricos ni se afirma que exista otra cuenta utilizable.

Consulta cerrada y credenciales MySQL eliminadas. Diario e inventario mínimo excluidos de Git en `.tmp/doc119-account-lookup/31f399c43f264242af8e7edeffc3cb7a/`; ningún password/cookie/token/cadena de conexión consultado o almacenado como dato de usuario. Principal permanece WAITING_SECOND_ACCOUNT con 194 HTTP/9 SELECT/1 sesión; total conjunto de lecturas de este corte **11 SELECT**. No se ejecutó login con candidatas, se cambiaron permisos o se completó el negativo/huellas posteriores. Progreso **4/24**, compuerta UI cerrada.

## Activación autorizada de carolina.cruz — 2026-11-12

El usuario solicitó expresamente activar carolina.cruz para la prueba. Ejecutor puntual y diario en `.tmp/doc119-admin-carolina/dabc96fa42cf490bbdbf9f2048d08950/`, excluidos de Git. Alcance cerrado a la identidad única, empresa del lote5 y exclusión del propietario; cuenta MySQL de escritura por consola enmascarada, sin recuperar credenciales de Web.config/procesos. Sintaxis JS/PowerShell y comprobaciones locales de identidad/hash PASS, sin atribuirles una prueba de BD.

Resultado real **VERIFIED**, commit confirmado y validado por SELECT en nueva conexión: usuario136/empresa2, carolina.cruz; Estado_Usuario=1 conservado e IMPORTAR_CARPETAS **0→1**. Una UPDATE, una fila afectada, cero INSERT, nueve SELECT administrativos. Perfil existente; todas sus demás columnas conservaron SHA-256 `c4ef650571fed041318371e09d15ac6ce2ecd2cf8f5f4f8c758ea028648d2b47`. No se concedieron otros permisos, cambiaron claves o modificaron recursos documentales. Cambio persistente solicitado por el usuario, sin restauración automática inferida. Diario cerrado y credenciales eliminadas a las16:13:05UTC.

Huellas de ejecutor: activate.cjs `7f5cf0c60e1722a53a8d63e74e054baab4c5547819d1af6cba52abe575cde300`; activate.ps1 `17faf5653edde958f05bde0b52a1abc5d95ec5a39e3d9f0d1007b4cd07d381ab`. La actuación administrativa no se presenta como consulta de control de solo lectura ni como aceptación E2E. Principal f6e3fc02 seguía en WAITING_SECOND_ACCOUNT, con propietario comprobado, 194 HTTP/9 SELECT/1 sesión; inventario previo2 SELECT y administración9 SELECT se registran aparte (20 SELECT conjuntos en ese corte y una escritura autorizada). SEG-01 completo/SEG-02 y compuerta UI siguen pendientes; 4/24.

## Fallo de identificación al autenticar segunda cuenta

Resultado final de f6e3fc02cdd940a0bef47a152a38ae67: STOPPED/DOC111_LOGIN_ACCOUNT_NOT_FOUND; 291 HTTP/9 SELECT/2 sesiones, guardia sin violación y claves eliminadas. Postback200/COMPLETE, cuerpo AVAILABLE, campos MATCHED y sin apertura de Producción para la segunda cuenta. ownerChecks conserva consulta/filtros/CSV/vacío como pendientes del negativo/controles posteriores, pagination=false; SEG-02 omitido. No se identifica retrospectivamente el nombre capturado ni se da por demostrado un error de escritura. El código público significa usuario no encontrado en el módulo; no acredita una contraseña incorrecta o revocación del permiso habilitado.

Corrección del ejecutor: reutilizar findSecondAccounts antes de la segunda sesión y usar el login canónico de una candidata actual; única se selecciona directamente, múltiples requieren elegir una opción. Se pide solo contraseña, sin recuperación/reset ni nuevas escrituras. Dos SELECT adicionales dentro del techo80; sin candidatas no se abre sesión. Estado registra activeRole y secondAccount con IDs/origen, sin login ni clave. El nuevo helper se incluye en evidencia de código.

Verificación local: **32/32 PASS** (27 políticas y cinco de inventario/selección), sintaxis y --check PASS. Nuevos casos acreditan nombre canónico sin captura manual, selección entre candidatas actuales, rechazo de selección ajena y falta de candidatas antes de login. Cliente/producto sin cambios; no se repiten sus pruebas anteriores ni se suman cortes. Permanece 4/24, sin nuevo PASS integrado hasta ejecución manual revisada.

## Privacidad real comprobada y cierre con límites — 2026-11-12

Corrida `08e8608d0dd94331864ab958830f2808`, código `4677ae05d1a59f6813eaff47a1a0a44f1bc822df`, ambiente localhost del plan, lote5, segunda cuenta136/empresa2 resuelta por SELECT vigente. Finalizó a las **19:04:13 UTC**, **FINISHED_WITH_LIMITS**, sin failure ni violación de guardia; sesiones cerradas y claves eliminadas.

**Privacidad PASS**: la cuenta ajena habilitada no pudo consultar el lote ni exportar su CSV (otherAccountDenied/otherCsvDenied=true; el helper exige ACCESO_DENEGADO y ausencia de datos de lote). El propietario comprobó consulta, filtros, descarga real de una fila de la página filtrada y filtro vacío sobre tres archivos originales. Controles RA y DA de expedientes conocidos invariantes: antes/después `0e2efb282feeb3cfcdccb1efc999552e0499c96abf58ea3825b77c67fd1a40bd`; noBusinessMutation=true, daScope=KNOWN_EXPEDITIONS. Esto no afirma invariancia de todos los datos del servidor ni ausencia de registros normales de acceso.

Consumo de esta ventana: **378/600 HTTP, 18/80 SELECT y 2/2 sesiones**, incluidos inventario vigente de segunda cuenta, dos autenticaciones y controles posteriores. La activación administrativa anterior de Carolina conserva su autorización, diario y contadores separados; no se reetiqueta como lectura de esta corrida. Los intentos anteriores no se suman como casos nuevos ni se borran.

SEG-01 queda **PASS_READONLY_WITH_LIMITS**: pagination=false/INSUFFICIENT_EXISTING_ROWS, sin segunda página acreditada; no se cambia política/tamaño de página ni se fabrican filas. SEG-02 queda **NOT_EXECUTED_RESOURCE_NOT_SELECTED** por omisión0. Tareas2.4/2.5 y compuerta2.7 permanecen pendientes de la aceptación restante; 4/24. Evidencia exportada por lista cerrada de campos a doc119-privacidad-20261112.json, sin nombres de cuenta, claves, cookies, tokens, cuerpos HTTP o CSV; diario original excluido en .tmp.
