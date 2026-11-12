# DOC-119 — plan de aceptación para concretar y autorizar

Preparación de tarea 1.2, D-10/RQ-10. El usuario indicó «autorizado» en respuesta al plan presentado: se autorizan las ventanas de lectura SEG-01/02 descritas abajo en el ambiente local. Cuentas y recursos se concretan por el operador en la consola enmascarada; no se deducen de permisos históricos ni se guardan credenciales. La tarea no se completa hasta fijar esas referencias.

## Destino propuesto y datos que faltan

Ambiente de estas ventanas autorizadas: `https://localhost/GestionDocumental-Docuarchi.net/`. Se autentica la cuenta propietaria y se muestran hasta 30 importaciones propias por fecha UTC, destino, estado y cantidades. El operador elige una opción de la lista; no necesita conocer IDs internos. La consulta se limita a usuario/empresa y facultades actuales, sin deducirlos de permisos/sesiones históricas. La cuenta ajena autorizada con permiso de importar se captura al llegar a la prueba de privacidad. Una excepción para certificado local solo se activa si el operador elige expresamente SI en la consola; de lo contrario se valida TLS normalmente.

SEG-01 necesita lote del propietario con estados mixtos y suficientes filas para más de una página según política vigente; al menos un filtro con coincidencias y uno sin ellas. Lote5 histórico PARCIAL es una referencia candidata para revisar por SELECT si el responsable la elige; no se presupone propietario, disponibilidad o adecuación actuales. Lotes27/28 y restantes recursos protegidos se conservan; este plan no los modifica ni reejecuta.

SEG-02 necesita un lote existente con recepción por completar, archivo local correspondiente autorizado para reselección y rama original visible/cargada; además otro nivel seleccionable y caso homónimo. Si no existe recurso adecuado, su creación/carga requiere propuesta y autorización separadas. No fabricar datos, modificar estados ni repetir efectos inciertos para construir el escenario.

## Ventanas propuestas

| Ventana | Acciones autorizadas | Techo autorizado por ventana | Criterio de aceptación |
| --- | --- | --- | --- |
| SEG-01 | Dos sesiones: propietario y cuenta ajena; ConsultarLote, ConsultarRecursosLote, ExportarPaginaLote y descarga desde vista | 2 aperturas de sesión, 600 HTTP de aplicación, 80 SELECT; sin efectos documentales | Filtros/páginas/CSV coinciden, totales originales preservados, fórmulas neutralizadas, cuenta ajena sin filas/CSV y huellas de negocio invariantes |
| SEG-02 lectura/UI | Recarga, ConsultarConfirmacionLote si corresponde, consulta del lote, reselección local sin envío y actualización deduplicada de rama | 2 aperturas de sesión, 600 HTTP de aplicación, 60 SELECT; cero ConfirmarPlan/AvanzarLote/ReconciliarRecurso/upload | Destino del lote y selección conservados, referencias aisladas, homónimos identificados por IDs y ausencia de confirmación repetida |
| SEG-02 recepción, solo si resulta necesaria | Enviar únicamente el recurso pendiente y contenido fijados después de revisar lectura/UI | No concedido; requiere IDs, PDF/hash/tamaño y presupuesto de un envío acordados aparte | Verificación servidor por contenido/autoridad sin duplicación; ningún almacenamiento o efecto adicional implícito |

Los techos propuestos cuentan reintentos y fallos de sesión; al agotarlos se detiene sin reiniciar contador. La sesión genera los registros normales de login/apertura existentes; invariancia se evalúa sobre datos/auditoría de negocio de los recursos, no se promete ausencia absoluta de registros de acceso.

## Controles y reutilización de herramientas

Reutilizar sesión autenticada y puente ODBC `queryRows` existentes. Los DSN RA/DA y credenciales SELECT se capturan en consola antes del descubrimiento parametrizado; no se admiten cadenas de conexión o secretos en el perfil. Huellas antes/después de lote/raíces/archivos/intentos y relaciones documentales pertinentes; sin UPDATE/DELETE, FOR UPDATE ni funciones de espera.

`run-doc111-importacion-carpetas.cjs` predeterminado ejecuta REC con efectos. `--diagnose` solo diagnostica acceso y no acredita seguimiento; `--verify-lot` está restringido a lote COMPLETADO y no acredita filtro/CSV/negativo/recarga incompleta. Los modos de recuperación pueden cerrar reservas, reconciliar y continuar almacenamiento: quedan fuera de las ventanas de lectura.

`run-doc113-ventana-importacion.cjs` cubre los casos UI-01/02/03 de DOC-113; UI-02 prepara planes y puede confirmar si se solicita. No es un selector SEG-01/02. Antes de una corrida DOC-119 hay que revisar/adaptar el recorrido de seguimiento con las mismas utilidades y una lista cerrada de operaciones permitidas; no ejecutar otro escenario para simular aceptación.

## Detención y evidencia

Detener ante autoridad/identidad/destino discordantes, operación fuera de la lista, mutación inesperada, respuesta no interpretable, control SELECT fallido o presupuesto agotado. No reintentar automáticamente ni convertir incertidumbre en permiso de recepción/almacenamiento. No cambiar permisos para probar revocación sin autorización específica.

Conservar solo resultado saneado, SHA/hashes del cliente servido y ejecutor, ambiente, IDs autorizados, contadores, códigos y huellas. Capturar secretos por consola compartida y eliminarlos al cerrar; nunca guardarlos o imprimirlos. No borrar lotes, temporales o evidencia al terminar.

La selección concreta de importación/nivel alterno/PDF se realiza después de autenticar al propietario y consultar sus recursos existentes. Esa sesión y los SELECT de descubrimiento consumen los mismos presupuestos SEG-01/02, sin reiniciarlos; la sesión propietaria se reutiliza en SEG-01. Hasta elegir el recurso, la guardia no permite operaciones de importación. La autorización actual no permite crear recursos si no existen casos adecuados ni habilita recepción: la tercera ventana sigue sin concederse. La reorganización UI de la sección 3 permanece detrás de aceptación integrada SEG-01/02; preparar el selector o pasar pruebas locales no abre esa compuerta. SEG-03 requiere decisión propia y no habilita borrados.

## Selector preparado

`node tools/e2e/scripts/run-doc119-seguimiento.cjs --check` comprueba el código sin red, sesiones ni credenciales. La corrida usa `--run <runId-32-hex> --authorized-plan` en consola visible; cuentas, claves y ruta local del PDF se enmascaran. El selector no consume perfiles con secretos ni configura otro login.

Reutiliza openSession/consulta de diagnóstico, queryRows, createSelectControls, selectLevel y captura de evidencia existentes. La guardia bloquea cualquier ASMX fuera de la lista, todo ASHX y postbacks de negocio; en SEG-02 solo permite Button_inline_trevie con título de nivel original/alterno elegido. Cuenta todos los HTTP de aplicación permitidos, incluidos recursos estáticos y pruebas de origen servido; cuenta SELECT y sesiones antes de invocarlos y conserva consumos en `.tmp/doc119-seguimiento/<runId>/state.json`, excluido de Git. No reutilizar un runId ni reiniciar el presupuesto tras un fallo.

Para aceptar SEG-01 completo se necesita un filtro cuyo resultado tenga segunda página real bajo el tamaño vigente; lote5 es solo candidato y puede ser insuficiente. Si faltan filas para paginar, se pueden revisar filtros, CSV de la primera página filtrada, vacío, cuenta ajena y huellas, registrando pagination=false y PASS_READONLY_WITH_LIMITS; no se fabrica éxito ni se acepta SEG-01 completo. SEG-02 ofrece las importaciones con archivos por recibir y muestra el nombre/ruta relativa del archivo esperado; necesita además raíz confirmada, rama original cargada y archivo correspondiente. Se puede omitir con opción 0 y queda NO_EJECUTADO, incluso si SEG-01 falla después. Los destinos alternos se eligen por nombre entre los actualmente autorizados. Homónimos o nuevas relaciones no observados se informan como límite, no como aceptación completa. Reselección solo acredita metadatos/identidad local; bytes/hash servidor no se verifican sin una recepción aparte.

## Continuación manual solicitada — 2026-11-12

La corrida `d09ca8a7cdc547cca79fb53d37af26af` se detuvo por guardia antes del postback de login: 1 apertura de sesión, 1 HTTP de aplicación, 0 SELECT y sin casos aceptados. Un diagnóstico posterior sin credenciales consumió 1 GET de aplicación y confirmó que la primera solicitud bloqueada era la hoja opcional de Google Fonts. La guardia ahora omite esa referencia exacta mediante aborto local, sin enviar tráfico externo ni ampliar las operaciones de negocio permitidas.

El usuario solicitó expresamente «continua con la prueba real» después de la primera corrida. Esa instrucción se toma como continuación manual tras corregir el ejecutor, con una nueva ventana de los mismos topes SEG-01/02 y runId distinto. No se borra ni reinicia la corrida fallida; se conservan sus consumos y el diagnóstico, y no se configura retry automático. Los secretos anteriores fueron eliminados y deben capturarse nuevamente por consola. Este intento no autoriza recepción, carga ni limpieza.

## Corrección de acceso y propuesta inicial de presupuesto

La corrida `42a55db64044453c8c6cbac63109702a` se detuvo con 93 HTTP, 1 apertura de autenticación y 0 SELECT. Un diagnóstico sin credenciales reprodujo el bloqueo durante un POST XHR al iniciar la página. El cliente 2FA llama automáticamente a ObtenerEstado con JSON vacío; el ejecutor ahora contempla exclusivamente esa consulta y la página local fija de destino de login. No contempla operaciones de verificación/reenvío/cancelación ni amplía APIs de importación.

Preflight real sin credenciales sobre la guardia corregida: página HTTP 200, controles presentes, 1 consulta ObtenerEstado, 95 HTTP de aplicación, 0 aperturas de autenticación/SELECT y ninguna violación. No acredita login ni SEG-01/02.

La carga de esa página ya supera los 80 HTTP de SEG-02 y deja solo 25 de los 120 HTTP de SEG-01 para Producción Documental y la segunda cuenta. Por ello se propuso **una nueva ventana manual con 600 HTTP por SEG-01 y 600 por SEG-02**, contando todos los recursos estáticos, postbacks, recargas y consultas como antes. Se conservan 80/60 SELECT y 2 aperturas de autenticación por ventana, sin concurrencia, efectos, recepción ni limpieza. En ese corte los nuevos techos no estaban autorizados ni configurados. La corrida fallida y sus dos diagnósticos permanecen registrados y no se reinician.

## Ampliación autorizada — 2026-11-12

El usuario respondió «autorizado» a la solicitud expresa de nueva corrida con **600 HTTP por prueba**, 2 sesiones y 80/60 SELECT, solo lectura y sin cargas ni limpieza. Se aplica esa ampliación en el ejecutor y en la tabla vigente de este plan. Cada nueva corrida conserva `authorizationRevision=20261112-http600` y los topes exactos en su estado local; no modifica los registros ni consumos de corridas anteriores. Los datos de acceso se capturan nuevamente en consola enmascarada porque se eliminaron al finalizar las corridas fallidas. La aprobación de presupuesto no acredita aceptación SEG-01/02 ni completa tareas pendientes por sí sola.

## Recorrido de acceso acotado

La corrida `c10eeb796fd147ee9742712916622cd2` terminó STOPPED con 137 HTTP, 1 apertura de autenticación y 0 SELECT. El postback respondió 200 con campos coincidentes; la guardia detuvo un POST de otro servicio durante la carga del panel de inicio. No se tomaron huellas de negocio ni se acredita aceptación o invariancia de recursos.

El ejecutor ahora resuelve localmente con HTML vacío el GET/HEAD sin parámetros del destino de inicio fijo, conservando la cookie que emitió el login real. El helper compartido abre después Producción Documental real. Así no se ejecutan el panel de inicio, su inicialización de temporales/firma ni sus servicios. No se sustituyen la página de Producción, respuestas de seguimiento o CSV.

Para cargar el cliente real se contemplan sus seis dependencias públicas versionadas, referenciadas en el ASPX: Popper 1.11.0, Bootstrap Table 1.23.2 (CSS/JS/export) y TableExport 1.29.0 (JS/jsPDF). Solo GET y tipo script/stylesheet a las URLs exactas enumeradas en `STATIC_DEPENDENCIES`; sin Cookie/Authorization/Proxy-Authorization/Referer/Origin. Cada solicitud consume los 600 HTTP autorizados. No se habilitan otros orígenes dinámicos, métodos o APIs. Se preservan códigos de dependencias en evidencia, sin cuerpos ni valores privados.

Preflight público de las seis dependencias: 6 HTTP, todas 200 y huellas SHA-256 conservadas localmente; sin autenticación ni SELECT. La sonda inicial dentro del sandbox falló al primer intento de transporte y se conserva por separado. No acredita el módulo autenticado. Una nueva consola queda bajo control del operador; no se relanza automáticamente ninguna corrida fallida ni se restauran credenciales eliminadas.

## Vigencia de sesión y selección conservada — 2026-11-12

La corrida `cb66698a07b644cf972012a2c738d5b1` accedió a Producción con cliente verificado, encontró 24 importaciones y recibió la elección del operador: opción 20 (lote5, tres archivos/dos carpetas) y omisión de recuperación con 0. Entre mostrar las importaciones y recibir la selección transcurrieron unos 64 minutos. Web.config mantiene sessionState timeout=10; IIS registró ConsultarLote POST 401 a las 14:31:43 UTC. El ejecutor esperaba ConsultarRecursosLote y terminó con DOC119_STOPPED por timeout, sin identificar el rechazo previo.

Se corrige el ejecutor para detectar ese rechazo y verificar la vigencia local con margen de 8 minutos, sin modificar la sesión del producto, renovar automáticamente o hacer keepalive. Tras omitir recuperación, ese estado se conserva aunque luego falle SEG-01. Se permite repetir los pasos de lectura con la elección previa mediante --reuse-selection, validando nueva cuenta/autoridad/propiedad; no se reutilizan secretos, sesión, consumos, resultados o huellas. La consola manual conserva la elección 20/0 para reducir esperas, dentro de los topes 600 HTTP/2 sesiones/80-60 SELECT ya autorizados. Falta de segunda página se conserva como limitación, sin aceptación completa ni creación de recursos.

## Resultado parcial y cuenta necesaria

La corrida `4b9c2748b0b54fc1a41e2a91b8c61447` comprobó consulta, filtros, CSV filtrado y filtro vacío con el propietario; paginación no acreditada por falta de filas y recuperación omitida. La segunda cuenta recibió 403 al acceder al importador (DOC111_IMPORT_ACCESS_DENIED), antes de consultar/exportar el lote ajeno. Se cerró con 375 HTTP, 9 SELECT y 2 sesiones, sin controles posteriores ni aceptación completa. El registro y consumos permanecen intactos. Para una nueva ejecución manual hace falta otra cuenta de prueba habilitada para importar, distinta de la propietaria; no se infiere que este rechazo inicial cubra el negativo por propiedad y no se cambian permisos para conseguir el escenario.

## Consulta de segunda cuenta solicitada — 2026-11-12

El usuario pidió «revisa por mi cual es esa segunda cuenta» mientras la corrida `f6e3fc02cdd940a0bef47a152a38ae67` esperaba la segunda cuenta, con 194 HTTP, 9 SELECT y 1 sesión. Se prepara un inventario acotado solicitado expresamente: máximo 2 SELECT auxiliares, sin HTTP/login de aplicación, reutilizando queryRows y la consola enmascarada. Se buscan cuentas activas de la empresa del lote, distintas del propietario, con IMPORTAR_CARPETAS=1 y filas de identidad/perfil únicas. No se consultan contraseñas ni se modifican permisos. Los nombres históricos no acreditan elegibilidad actual.

La prueba principal permanece en espera y la consulta auxiliar tiene diario/consumo propio enlazado; sus lecturas se suman al informar el total, sin reiniciar el contador principal. Solo se volverán a pedir por consola las credenciales MySQL de lectura, no las de aplicación; no se extraen del proceso principal ni de Web.config. El resultado de elegibilidad no acredita por sí solo el caso de privacidad.

Resultado real del inventario `31f399c43f264242af8e7edeffc3cb7a`: **NO_ELIGIBLE_SECOND_ACCOUNT**, 2 SELECT, 0 HTTP y 0 aperturas de autenticación de aplicación. Lote5, propietario141, empresa2: cero cuentas candidatas distintas del propietario que cumplan simultáneamente identidad activa/única, misma empresa, perfil único e IMPORTAR_CARPETAS=1. No se consultaron contraseñas ni se concedieron permisos. Inspector cerrado y claves MySQL eliminadas. La prueba principal sigue WAITING_SECOND_ACCOUNT con 194 HTTP/9 SELECT/1 sesión; consumo de lecturas conjunto de este corte: 11 SELECT. El negativo por propietario y los controles posteriores continúan pendientes; haría falta disponer de otra cuenta elegible, mediante una actuación separada del inventario de lectura.

## Activación expresamente autorizada de carolina.cruz

El usuario solicitó «activa el usuario carolina.cruz para poder hacer la prueba». Autoriza una actuación administrativa separada de la ventana E2E de lectura: resolver identidad única actual, verificar empresa2 del lote5 y exclusión del propietario141, activar Estado_Usuario solo si está en 0 y habilitar IMPORTAR_CARPETAS solo si está en 0. Si no existe fila de permisos, solo se admite crearla con todos los demás valores por defecto denegados (0/NULL) y clave automática; se detiene ante esquema o identidad incompatibles. No se modifica contraseña, identidad, empresa, otros permisos ni recursos documentales.

Se prepara ejecutor puntual/diario excluidos en `.tmp/doc119-admin-carolina/dabc96fa42cf490bbdbf9f2048d08950/`, con cuenta MySQL de escritura capturada en consola enmascarada, mismo DSN autorizado y sin recuperar secretos de procesos/Web.config. Requiere tablas InnoDB, transacción, comparación de estado previo, máximo dos escrituras de una fila, control de otras columnas y comprobación en una conexión nueva tras commit. No repite una escritura incierta ni restaura el permiso concedido por inferencia: la activación solicitada queda persistente. Controles administrativos SELECT (máximo16) y escrituras se contabilizan aparte; no amplían los topes de la prueba principal.

Preparación local: sintaxis JS/PowerShell correcta, identidad distinta de propietario/empresa ajena/login distinto rechazadas, hash de otras columnas estable al cambiar solo IMPORTAR_CARPETAS y sensible a cambios administrativos. Estas verificaciones no acreditan todavía activación real; el diario distinguirá commit/verificación/rollback o incertidumbre.

Activación real **VERIFIED** el 2026-11-12 a las 16:13:05 UTC: carolina.cruz corresponde al usuario136/empresa2, distinto del propietario141. Estado_Usuario ya era1 y se conserva; IMPORTAR_CARPETAS pasó0→1 mediante una única UPDATE de una fila de permisos existente, sin INSERT. Hash de todas las demás columnas de esa fila antes/después idéntico: `c4ef650571fed041318371e09d15ac6ce2ecd2cf8f5f4f8c758ea028648d2b47`. Commit y SELECT en nueva conexión confirmados. Nueve SELECT administrativos, una escritura, claves eliminadas al cerrar. La activación queda persistente por solicitud del usuario; no se modifican contraseña, otros permisos o documentos. La prueba principal seguía esperando la segunda cuenta y puede usar carolina.cruz; la activación no acredita por sí sola el negativo/huellas de SEG-01.

## Nombre de segunda cuenta validado antes de autenticar

La corrida f6e3fc02 terminó STOPPED/DOC111_LOGIN_ACCOUNT_NOT_FOUND, con 291 HTTP, 9 SELECT y 2 sesiones. El postback de la segunda cuenta respondió200, cuerpo disponible y campos enviados coincidentes con la captura; no se abrió Producción para esa cuenta. El nombre introducido no se conservó, por lo que no se atribuye el fallo a un error de escritura confirmado ni a contraseña/permisos. La activación administrativa136/empresa2 conserva su verificación independiente.

Para evitar una nueva captura ambigua de identidad, el ejecutor consulta candidatas actuales mediante los dos SELECT del inventario dentro del presupuesto principal, selecciona la única o solicita elegir una opción cuando haya varias, y utiliza el login canónico retornado por la base. Solo se captura la contraseña de aplicación de la cuenta identificada. Se revalidan actividad, empresa, perfil/identidad únicos, IMPORTAR_CARPETAS=1 y exclusión del propietario antes de abrir la segunda sesión; sin candidatas no se intenta login. El diario registra solo el ID/empresa/origen de selección, sin nombres o claves. No se cambian datos de autenticación ni se renueva automáticamente la corrida fallida. La nueva consola manual conserva importación5 y recuperación omitida dentro de los límites autorizados.

## Resultado real de privacidad — cerrado con límites

La corrida `08e8608d0dd94331864ab958830f2808` finalizó FINISHED_WITH_LIMITS a las19:04:13UTC del 2026-11-12. SEG-01: consulta/filtros/CSV/vacío del propietario comprobados; segunda cuenta136/empresa2 con lote y CSV ajenos denegados. Privacidad PASS y huellas de negocio RA/DA conocidas iguales antes/después: `0e2efb282feeb3cfcdccb1efc999552e0499c96abf58ea3825b77c67fd1a40bd`. Consumo378 HTTP/18 SELECT/2 sesiones, dentro de600/80/2; sin fallo ni violación de guardia, corrida cerrada y credenciales eliminadas.

Se conserva PASS_READONLY_WITH_LIMITS: tres archivos originales no alcanzaron para segunda página del filtro; CSV verificado de primera página filtrada, una fila. Recuperación continúa NOT_EXECUTED_RESOURCE_NOT_SELECTED por decisión0. La privacidad acreditada no sustituye paginación ni SEG-02, no habilita cargas/limpieza o reorganización UI y no transforma el cambio completo en aprobado. Informe `doc119-privacidad-20261112.json` en este directorio. La activación de Carolina es una actuación administrativa anterior, autorizada y contabilizada por separado.
