# Runbook para agentes — E2E y concurrencia DOC-10 / DOC-11 / DOC-28 / DOC-32 / DOC-33 / DOC-44

## DOC-120 - seguimiento visible sobre E2E existente2026-11-19

Reutilizar `run-doc119-seguimiento.cjs --run <nuevo32hex> --authorized-plan --headed`: Chromium visible/slowMo300, nueva autorizacion SI en consola antes de cuentas y controles solo lectura existentes.600 HTTP/2 sesiones por SEG;80 SELECT en SEG-01 y60 en SEG-02. No confirma planes, recibe, crea, almacena, reanuda ni borra. Puede comprobar consulta/filtros/CSV/privacidad/recuperacion, pero no pausa durante guardado. Consola36a221bf iniciada; resultado pendiente, no ejecutar otra ni reutilizar QA cerrada. Evidencia Validacion/doc120-e2e-visible-20261119.md/JSON;39/39 politicas y20/20 compatibilidad con cinco solapados. Apartados inferiores historicos.

## DOC-120 - auditoria de transiciones2026-11-19

AUDCTX01..04/tareas3.235..3.238: cuatro defectos de contexto corregidos; consulta con modelo propio, paginas reiniciadas, borradores ligados a filtros del contexto y ayuda estable ante acuses.223/223 en24 suites locales,7 nuevos; incluye1000 sinteticos. Comando reutilizable `node tools/e2e/scripts/test-doc120-transitions.cjs` (`--list` sin ejecutar); no pide claves ni usa runners reales. Primera amplia214/223 conservada, cinco suites antiguas ajustadas al consentimiento/etiquetas/ultimo lote y38/38 antes de corrida completa.247/258 y11 pendientes previas; assets doc120-auditoria-transiciones-20261119-8. Evidencia Validacion/doc120-auditoria-transiciones-20261119.md/JSON. Cero negocio real, autenticacion/HTTP/SELECT nuevos, DLL o esquema; mantener QA anterior cerrada. Apartados inferiores historicos.

## DOC-120 - consentimiento del lote consultado, corte2026-11-19

CTX-EXIT01..03/tareas3.231..3.234: el lote visible pendiente tiene prioridad sobre preparacion guardada vacia/completada; revision/otro lote guardado se advierten aparte. Cancelar no pausa; aceptar espera acuse sin otro efecto.53/53 Chromium sinteticos,5 nuevos; rojo0/2 y focal5/5, texto1/1 solapado,1280/768/390.243/254 y11 pendientes previas; assets doc120-salida-consulta-20261119-7. Evidencia Validacion/doc120-salida-consulta-20261119.md/JSON. Cero HTTP/SELECT/autenticacion/efectos reales, DLL o esquema. Sin operaciones reales sobre52 ni reapertura de QA cerrada. Aceptacion visual actual pendiente; apartados inferiores historicos.

## DOC-120 - salida de revision antes de pausa, corte actual2026-11-19

REV-EXIT01..03/tareas3.227..3.230: consentimiento del modulo antes de pausar preparacion/confirmacion/lote. Cancelar no pausa, aceptar espera, forzado invalida. Una continuacion principal/ticket vigente; perfil/inventario distintos invalidan; pausa intencional es estado y contador incorpora ultima parte confirmada. Avisos separan seleccion/metadatos/confirmacion incierta y limites tras recarga.81 casos unicos locales (63/65 +18/18 focal/core, dos reparados y16 distintos),6 nuevos;1/1 primario solapado y1000 sinteticos con presupuestos DOM/memoria.239/250 y11 pendientes previas; assets doc120-revision-salida-20261119-6 incluyen segmentador. Evidencia Validacion/doc120-revision-salida-20261119.md/JSON. Cero HTTP/SELECT/autenticacion/efectos reales, DLL o esquema. No relanzar50/51 ni ventanas cerradas; aceptacion visual actual pendiente. Otras confirmaciones legacy y permiso de carpeta del navegador no cambian. Apartados inferiores historicos.


## DOC-120 - salida nombrada como pausa, corte actual2026-11-19

CTL-10/tareas3.224..3.226: Pausar y salir visible/accesible y en consentimiento durante actividad; Cerrar en reposo/pausa/completado. Misma espera/confirmacion/uncertumbre sin cambiar negocio.42 casos existentes verificados (34/42 +10/10 focales, dos solapados),0 nuevos y sin repetir1000.235/246 y11 pendientes previas; assets doc120-pausar-salir-20261119-5. Evidencia Validacion/doc120-pausar-salir-20261119.md/JSON. Cero HTTP/SELECT/autenticacion/efectos reales, DLL o esquema. QA historica cerrada; aceptacion visual actual pendiente. No relanzar50/51 ni ventanas cerradas; apartados inferiores historicos.


## DOC-120 - mensajes y aviso compacto, corte actual2026-11-19

MNS-01..03/tareas3.220..3.223: mensaje y render inicial de continuar/pausar inmediatos, progreso normal agrupado, diagnostico incierto conservado. Aviso local resumido con detalle de ruta/fase/referencia; limites/scroll/foco del dialogo y botones originales.59/59 locales (5 nuevos),1/1 solapado de ultimo boton;1552x912/768x600/390x600. No se repite1000 ni se mide SQL/almacenamiento nativo.232/243 y11 pendientes previas; assets doc120-mensajes-vigentes-20261119-4. Evidencia Validacion/doc120-mensajes-vigentes-20261119.md/JSON. Video usuario muestra recuperacion22 a99 guardados, sin auditoria SQL nueva. Cero autenticacion/HTTP/SELECT/efectos reales, DLL o esquema. No relanzar50/51 ni ventanas anteriores; aceptacion visual actual pendiente. Apartados inferiores historicos.


## DOC-120 - recuperacion local visible, corte actual2026-11-19

ORI-01..03/tareas3.216..3.219: ante PENDIENTE sin File, aviso distingue seleccion local y temporal recibido; Vincular carpeta/PDF pausa antes de selector existente y no envia. Pausar/cancelar mantiene accion visible, validacion contextual/metadatos omite RECIBIDO y Continuar es explicito. Temporal RECIBIDO ausente no ofrece restauracion local.56/56 locales,6 nuevos;6/6 focales solapados. Sintetico1000:10 existentes preservados,990 vinculados sin envio y990 procesados una vez,0 nuevas confirmaciones/expedientes. Capturas390/768/1280, assets doc120-recuperacion-originales-20261119-3;228/239 y11 pendientes previas. Evidencia Validacion/doc120-recuperacion-originales-20261119.md/JSON. Sin autenticacion/HTTP/SELECT/efectos reales, DLL o esquema. No relanzar50/51 ni ventanas previas; aceptacion manual actual pendiente. Apartados inferiores historicos.


## DOC-120 - consentimiento de cierre, corte actual2026-11-19

CTL-07..09/tareas3.212..3.215: dialogo previo para lote confirmado pendiente; cancelar/Escape conserva ejecucion, aceptar espera solicitud/lectura y no inicia siguiente efecto. Mientras decide puede avanzar, informado. Completado/forzado no preguntan. Driver SEG-02 acepta nuevo aviso si existe y espera cierre real antes de recargar.78 casos unicos locales (73/78 +11/11 focales, seis solapados),11 nuevos; capturas390/768/1280. Cero HTTP/SELECT/autenticacion/efectos reales; no se compila/sustituye DLL ni cambia esquema. DLL observada B0B5D0D3 distinta de QA8B27A942, sin certificarla.224/235;11 pendientes previas. Assets doc120-consentimiento-cierre-20261119-2. Evidencia Validacion/doc120-consentimiento-cierre-20261119.md/JSON. No relanzar lotes50/51 ni ventanas previas para verificar este ajuste; aceptacion manual actual pendiente. Apartados inferiores historicos.


## DOC-120 - controles y cierre cliente actualizados2026-11-19

CTL-01..06 implementados localmente: X/Escape voluntarios esperan acuse sin abortar; una transicion de navegacion; pausa de admision manual/metadatos y aviso por resultado incierto. Desmontaje forzado conserva invalidacion/cancelacion local.22 nuevos casos,142 unicos cubiertos (141/142 inicial +8/8 focal tras conservar redaccion habitual, siete repetidos), flujos sinteticos de1000/DOM/memoria y consulta/CSV/arbol. Sin nueva autenticacion/HTTP/SELECT/efectos, DLL o esquema. Fuente Validacion/doc120-controles-cierre-20261119.md/JSON, assets doc120-controles-cierre-20261119-1;220/231 y11 pendientes. QA5f7af8f5 pertenece al cliente anterior y sigue cerrada. Videos del usuario muestran50 completado1000 en esa version; no reimportar120/121 ni reiniciar su ventana para verificar el cierre nuevo. Otra QA real necesita alcance/cuentas/recursos/presupuesto nuevos. Los apartados inferiores son historicos.

## DOC-120 - QA real aprobada con limites2026-11-19

5f7af8f5354a4fca5e2eb5c7a53b4b4d PASS_QA_WITH_LIMITS:758 HTTP/22 SELECT externos/3 sesiones,4 ALMACENAR sobre120/121 (2 positivos/2 rechazos esperados), documentos16284/16285 y lote50 pasa23 a25. Lote29 conserva16282/16283, B51 pendiente y arbol con un nodo logico/etiqueta por codigo. Perdida de entrega tras checkpoint recuperada sin replay, carrera HTTP con un ganador, revision antigua rechazada y consulta/CSV ajena denegados en29/50. Huellas25/998 preservadas; estados anteriores intactos, ventana cerrada y secretos eliminados. No relanzar120/121 ni reabrir ventanas cerradas; la siguiente operacion requiere estado/alcance concretos. Fuente Validacion/doc120-qa-recuperacion-concurrencia-20261119.md/JSON/CSV.3.196/3.202/3.203 completas en alcance;212/223. No certifica rollback nativo3.187, inicio UI completo3.87, preparacion/capacidad total o aceptacion administrativa. El usuario puede hacer ahora su prueba visual;50 muestra25 guardados/975 pendientes, sin operaciones nuevas del agente por esta revision.

## DOC-120 - QA parcial con efectos, continuacion especifica2026-11-18

Run95b22a23 cerrado STOPPED:582 HTTP/12 SELECT/1 sesion,1 recibo677 bytes y1 expediente694, documentos16282/16283. No volver a lanzar el perfil inicial: el corpus29 ya cambio. Continuacion exclusiva --resume-after-ui revisa ese estado intacto y verifica identidades/huellas antes de HTTP; lote29 solo lectura, cero confirmaciones/recibos/creaciones. Solo archivos120/121 del50 para cuatro solicitudes maximas/dos documentos esperados; nuevo presupuesto y acumulados en Validacion/doc120-qa-fallos-plan-20261118.md requieren aprobacion concreta. Fallo generico no localiza subpaso despues de pausa; no atribuirlo al escritor. Evidencia doc120-qa-parcial-20261118.json.12/12 locales, DLL sin cambios; recuperacion final, carrera y privacidad pendientes.

## DOC-120 — diez siguientes medidos y ventana cerrada2026-11-18

Solicitud expresa de diez siguientes del lote50: perfil `--authorized-ten` del ejecutor existente, ventana nueva3d30f47d6bec4e33b6e532ffa8ed78cb enlazada a c6f62c44 aprobada. FINISHED_TEN,118 HTTP/48 SELECT externos/1 sesión/10 ALMACENAR;110–119 confirmados RA/DA en documentos16270–16279, mismo expediente693/gabinete10.13→23 guardados y huella de990 ajenos preservada. Media0,969465s, mediana0,934637s, p95 muestral1,140969s, rango0,897519–1,225048s; sin degradación creciente observada entre mitades.14/14 locales, modo3 conserva límite3. Sin cambio de DLL/esquema, recepción/creación/reanudación/borrado. Cerrado y secretos eliminados, estado/ventana anterior intactos. Fuente `Validacion/doc120-lote50-diez-20261118.md`/JSON/CSV. No reabrir ni ampliar automáticamente la ventana;50 o1.000 necesitan alcance/aprobación propios. No certifica sellado en frío, PDF grandes, concurrencia o capacidad total.

## DOC-120 — corrección focal instalada y tres guardados aprobados2026-11-18

DLL/PDB8f71ec7e instaladas con respaldo tras aprobación (DLL8b27a942). Nueva ventana aprobada c6f62c443bf64e039878754736ceb1b4 FINISHED_THREE:104 HTTP/20 SELECT externos/1 sesión/3 ALMACENAR.107/108/109 confirmados RA/DA en documentos16267/16268/16269 y expediente693;10→13 guardados, huella de997 ajenos preservada. HTTP1,352934/0,913335/0,908159s, promedio1,058143s frente79,432043s de la referencia histórica. Control focal ya existente, no mide sellado en frío ni capacidad completa. Cerrado, secretos eliminados, ventana42318523 intacta; no repetir automáticamente. Fuente `Validacion/doc120-lote50-correccion-medida-20261118.md`/JSON/CSV.3.198 aprobada en ese alcance;3.187/3.196 y once brechas previas conservadas. Los apartados inferiores mantienen sus cortes históricos.

## DOC-120 — NullReference focal reproducida sin escritor2026-11-18

Diagnóstico puro9f11f7ba:7 SELECT,1.000 archivos/1.000 pruebas válidas,0 HTTP/efectos, cerrado y secretos eliminados. Auxiliar nativo59c48d91 falla por vista32/64 del DSN antes de SQL; corregido x64. ad60fe52 llega a NATIVE_LOT y falla un guard del auxiliar, límite superior9 SELECT, sin reproducir el fallo de aplicación. No confundir estos estados con una causa certificada del500. Inspección y reproducción sin conexión sobre DLL instalada b3303603 confirman NullReferenceException en CargarBarreraFocal al agregar la raíz a una lista Nothing. Corrección local inicia las dos colecciones mediante constructor del repositorio;20 Node+2 wrappers Framework/DDL aprobados, Compile aislado0 errores. Fuente `Validacion/doc120-preflight-puro-20261118.md`/JSON. Candidata8b27a942 con respaldo, instalación y nueva ventana real pendientes; no repetir107 por estos diagnósticos ni declarar3.198 aprobada.

## DOC-120 — HTTP500 antes de nueva reserva2026-11-18

Usuario «lanza»: run42318523fd4e432a84a2473952e3e59d,100 HTTP/8 SELECT/1 sesión, un ALMACENAR107 con HTTP500/ERROR_INTERNO en1121,0625ms;0 acuses confirmados y cero solicitudes108/109. Referencia a6720c307b20449ca695c0b318e69ee4. Cerrado, secretos eliminados, ventana anterior intacta. Diagnóstico posterior be3cbe81 con5 SELECT/0 HTTP confirma RA EN_CURSO/revisión3057,10 completados/990 pendientes y cero actividad/incertidumbre; no certifica DA/disco ni la excepción exacta. Fuente `Validacion/doc120-lote50-http500-20261118.md`/JSON. No reintentar107 ni modificar estados para forzar medición. Reproducir lecturas/validaciones sin escritor o mejorar evidencia técnica antes de otra ventana con efectos.3.198 sigue pendiente.

## DOC-120 — nueva versión instalada, medición sin despachos2026-11-18

Solicitud de reutilizar la medición de tres archivos del lote50. Run9dd8a1e963154c338fb8885c10ce8180 revisa la ventana23e18a40 sin modificarla. Índice09 aplicado manualmente y verificado por SELECT. Tras «continua» y aprobación de instalación, DLL/PDB del commitb72f2dd6 instaladas con respaldo verificado (DLL b3303603). Cerró antes del login con5 SELECT/0 HTTP/0 sesiones/0 ALMACENAR, secretos eliminados; código genérico no certifica causa. Diagnóstico futuro distingue entrada de cuenta/clave y arranque del navegador sin conservar excepciones privadas. Evidencia `Validacion/doc120-lote50-medicion-reutilizada-20261118.md`/JSON;11/11 locales. No hay tiempos nuevos ni aceptación de3.198/3.187/3.196. No reabrir ni borrar estados cerrados; revisar consumo/resultados y autorización antes de otra ventana. Los apartados inferiores conservan el estado histórico de su corte.

## DOC-120 - procesamiento independiente, corte local2026-11-18

Implementacion local y pruebas sinteticas terminadas; script09 preparado, sin aplicar indice ni instalar DLL/assets. [Contrato focal](../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/contrato-focal-implementado-doc120.md) y [ventana nueva propuesta](../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-independientes-plan-real-20261118.md). La ventana anterior23e18a40 sigue cerrada. No reutilizarla, no borrar originales recibidos para simular faltantes y no interpretar "implementa" como autorizacion de operaciones reales. Nuevas pruebas MySQL/IIS requieren recursos, topes y aprobacion especificos. El ejecutor anterior conserva sus limites; la ampliacion del parser de tiempos no lo habilita para decisiones o nuevos efectos.


## DOC-120 — tres almacenamientos medidos y ventana cerrada2026-11-17

Autorización «ejecuta la prueba»: DLL/PDB instrumentadas instaladas con respaldo,1 POST anónimo401; run23e18a403aae4dd4b455adc86ab787ad FINISHED_THREE.104 HTTP/14 SELECT de control/1 sesión/3 almacenamientos. Archivos104/105/106 confirmados en documentos16264/16265/16266 y expediente693;7→10 guardados, huella de997 ajenos preservada. Promedio HTTP79,432s, metadatos72,321% y writer0,082% del coordinador. Fuente `Validacion/doc120-medicion-tres-20261117.md`/JSON/CSV. Cierre y secretos eliminados; el proceso Node terminó. No reabrir la consola ni eludir window.json para guardar otros tres; una continuación requiere revisar consumo/resultados y autorización específica. La DLL detallada sigue instalada; la captura por cabecera es optativa, sin optimización ni cambio de política. Los apartados inferiores conservan el estado de preparación/histórico de su corte.

## DOC-120 — medición solicitada de tres archivos,2026-11-17

Usuario solicita prueba real de tiempos de tres archivos del lote50. [Ventana concreta](../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-medicion-tres-plan-real-20261117.md): propietario141/empresa2/nivel223;3 ALMACENAR de originales ya recibidos y con valores fijados,600 HTTP/60 SELECT de control/1 sesión/45min para nuevas operaciones. Cero preparación, recepción, creación, materialización, reanudación, reconciliación, SII o borrado. Señal de detención, actividad o incertidumbre detienen; no liberar por SQL ni inferir abandono.

Instalación de DLL instrumentada pendiente de aprobación separada; no iniciar con la DLL anterior porque falta la captura Server-Timing. `measure-doc120-three-files.cjs --run <32hex> --authorized-three --binary <64hex-instrumentado>` exige consola privada, sella binario/runner y bloquea una segunda ventana sin revisar la anterior. No usar safePost genérico para ALMACENAR: su timeout30s no sirve para esta medición; el ejecutor espera respuesta con timeout0 y maxRetries0. Los controles son SELECT y el resto de operaciones no están permitidas. No copiar credenciales/cookies/cuerpos, ni convertir esta ventana de tres archivos en carga completa o aceptación final.

## DOC-120 — espera y reanudación general, implementación local2026-11-17

3.136–3.158 implementan espera de acuse en recepción/ALMACENAR y ReanudarLote con revisión/evento transaccionales; prueba de carrera/rollback solo en un modelo local. Fuentes y límites: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-espera-reanudacion-20261117.md`. No hay nuevo despliegue ni operaciones reales en esta entrega. La autorización cerrada del diagnóstico inferior no habilita reanudar el lote50.

Ventana adicional preparada en `Validacion/doc120-reanudacion-plan-real-20261117.md`: propone un control y un solo documento existente, cero recepciones/creaciones,600 HTTP/120 SELECT de control/2 sesiones, máximo45min y64MiB de crecimiento. Requiere aprobación específica de recursos/topes, versión desplegada compatible y runner revisado con guardia de un almacenamiento/pausa al iniciarlo; no lanzar un ejecutor previo que carezca de ese control. Las carreras reales/inyección de fallo MySQL necesitan lote y ventana de laboratorio propios. La ventana de1.000 PDF anterior no se amplía por esta preparación.

## DOC-120 — diagnóstico real lote50, corte2026-11-17

Usuario «autorizado» habilitó como máximo5 SELECT RA/0 HTTP/0 cambios para certificar detención del lote50. Run8803b6f4f2c44b028bc279fbe42f4250 cerrado:5/5 completados, credenciales eliminadas; DETENIDO/señal1/revisión3026, evento de detención3544, inventario1.000 recibido/4 completado/996 NO_INICIADO con valores, creación confirmada y0 intentos activos/inciertos. Lecturas antes/después estables. Fuente `Validacion/doc120-detencion-lote50-20261117.md`/JSON. No repetir automáticamente, no convertir esta ventana en permiso de ReanudarLote/almacenamiento/SQL correctivo ni inspección de DA/disco. `inspect-doc120-stopped-lot.cjs` está fijado a lote50/propietario141/empresa2 y conserva solo estados/referencias/conteos;8/8 locales. La brecha de reanudación general sigue pendiente de implementación.

## DOC-120 — diagnóstico visible de parada, corte2026-11-17

Tras video PARADA, cliente actualizado con causa pública/timeout real, contexto y posición del recurso, último avance confirmado y bloqueo publicado antes de la consulta posterior.89/89 locales con módulos/Chromium y transporte sintético;4 GET técnicos estáticos200 coinciden con fuentes principales.0 sesiones autenticadas/SELECT/efectos reales. No repetir lote50/archivo98 por el video ni asumir su guardado final. Fuente `Validacion/doc120-parada-diagnostico-20261117.md` y JSON. Sin cambios a tiempo de espera, esquema, DLL, integridad, almacenamiento, SII o permisos. Las otras ventanas, incluida ampliación de BENEFICIARIO detenida antes de escribir, conservan su alcance separado.

## DOC-120 — ajuste autorizado de BENEFICIARIO, corte2026-11-17

Usuario «hazlo por mi» autoriza medir y alinear índice17 de Egresos. Medición0bca49d4 cerrada con9 SELECT: CONTABIL.BENEFICIARIO60/regla60, origen250/máximo77/230 excedentes en25.153 filas. Credenciales eliminadas, sin textos de negocio guardados. Intento d605ad7e cerrado en login con96 HTTP/1 sesión/0 SELECT de control/0 mutaciones y secretos eliminados; corregida la omisión de fuente opcional y landing, con guardia de POST cerrada. Continuación fa226614 cerrada en PUBLICATION_CHANGED tras98 HTTP/1 sesión/0 SELECT de control/0 mutaciones; secretos eliminados. Refrescar la medición/observar publicación y revisiones vigentes antes de otra aplicación. No repetir automáticamente. Límites de la aplicación preparada: máximo600 HTTP/1 sesión/12 SELECT de control,1 ALTER de ese campo a250,1 UPDATE acotado del metadato17 y una versión nueva de Egresos. Reutiliza autenticación/safePost existentes; cuenta administradora y cuenta MySQL con permisos específicos en consola, nunca del Web.config. Ver `Validacion/doc120-egresos-longitud-20261117.md`. Excepción D-09 solo para ese campo/metadato; sin permisos, recortes, origen, otros campos, planes/lotes/documentos ni actualización de snapshots. El DDL tiene commit implícito: conservar efectos parciales y detener sin replay ni shrink automático. No marcar aplicado por scripts/pruebas locales o mientras falten entradas; no heredar esta autorización para importar1.000 PDF.

## DOC-120 — relectura de plan sellado, corte posterior2026-11-17

`beae0afc` corrige snapshot original guardado frente a snapshot canónico de la huella, y lee planes2 anteriores normalizando solo el orden en memoria. Reproducción local con cabecera persistida/1.000 archivos y rechazo de valor alterado;18/18 locales y Build. DLL/PDB instalados preservando una recompilación intermedia;1 POST JSON anónimo401/SESION_REQUERIDA,0 SELECT/efectos. No repetir preparación ni consultar datos reales sin la ventana pertinente. Referencia62232125f0f64fdfb1537b2251288a35 reportada por usuario; lectura autenticada del caso pendiente. Retorno recomendado vigente `beae0afc`, sustituye `af1ff073`; fuente `Validacion/doc120-lectura-plan-sellado-20261117.md`. Reinicio ASP.NET puede perder sesión InProc/referencias; no prometer continuar el borrador tras recarga sin reconstruir su correspondencia. Sin cambios de SQL, perfiles, límites, SII o escritores de negocio.

## DOC-120 — esquema y despliegue, corte posterior2026-11-17

El usuario informó aplicación manual de SQL08 y continuó la verificación/despliegue local. Run25b3cd1989384452b39a70282782d9b9 cerrado:10 SELECT de metadatos completados, esquema anterior/nuevo aceptados, credenciales eliminadas. DLL/PDB instalados en IIS local y3 HTTP técnicos sin autenticación/efectos: WSDL500 preexistente comprobado con DLL anterior/nueva, JSON de nueva consulta401/SESION_REQUERIDA. No repetir SQL ni esta ventana automáticamente. Evidencia `Validacion/doc120-despliegue-esquema-20261117.md`. **Retorno mínimo vigente `af1ff073`**, sustituye `d5bae464` por la corrección del detector de índices padre. No inferir autorización de importación, almacenamiento, carga o fallo/rollback transaccional;3.118/3.126/3.127 y5.2 continúan pendientes. Los cortes inferiores conservan su alcance histórico.

## DOC-120 — preparación segmentada, corte local2026-11-17

Contrato08 implementado localmente: partes de metadatos, plan2, consulta focal y reselección de carpeta.200 casos únicos y compilaciones/lectores A→B/retorno compatibles, sin autenticación nueva, SQL real, despliegue o recepción. Evidencia `Validacion/doc120-segmentada-20261117.md`. `check-doc120-plan-compatibility.cjs --reader d5bae4640082f7a53fcd5522891715e7d876b0dc` solo compila en `.tmp/` y prueba archivos sintéticos.

Ventana de1.000 PDF preparada en `Validacion/doc120-capacidad-plan-real-20261117.md`: **NO AUTORIZADA NI EJECUTADA**. SQL08 y DLL sin aplicar; retorno mínimo compatible `d5bae464`, conservando tablas/planes2. No instalar binarios anteriores que desconozcan2 cuando haya esos planes. La prueba real propone un lote/una raíz/1.000 documentos y continuación con pausa/recarga; exige autorización específica y revisión del ejecutor adaptado. El runner DOC119 y su allowlist antigua no habilitan estas operaciones ni almacenamiento. No heredar las ventanas históricas de27 archivos ni ejecutar rollback/fallos/DDL por inferencia.3.118 conserva pendiente el ensayo transaccional MySQL.

## DOC-120 — inicio único, corte local2026-11-15

Confirmar e importar coordina las fases existentes con acuse/lectura antes del siguiente efecto; pausa/cierre invalidan la continuación y consultar/exportar no reanuda. Continuar importación usa los IDs/estado del mismo lote y pide originales faltantes. Códigos repetidos permitidos, snapshots históricos intactos y política configurable futura todavía fuera de alcance.207/207 locales y compilación aislada;106/112 tareas,40 escenarios. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-coordinacion-20261115.md` y JSON compañero. Sin despliegue/activación IIS ni nueva ejecución autenticada o de negocio real.

Ventana concreta preparada en `Validacion/doc120-coordinacion-plan-real-20261115.md`: falta autorización específica de ambiente, cuentas, recursos y efectos;3.86/3.87 pendientes. Los permisos de corridas anteriores no autorizan esta prueba ni27 almacenamientos. No usar el runner DOC119 de preparación como sustituto, porque su permiso excluía almacenar documentos. No ejecutar carga, DDL, limpieza, SII ni modificar cuentas por inferencia. Las secciones siguientes conservan su fecha y alcance histórico.

## DOC-120 — reanudación permanente, corte local2026-11-14

Retomar creación está implementado como fase explícita de AvanzarLote, con preflight, fencing del token y conservación de la misma reserva.108/108 locales y compilación; evidencia doc120-retomar-creacion-permanente-20261114.json. Este corte no autoriza ni ejecuta recuperación autenticada: lote39/intento96/recibo95 siguen sin operaciones nuevas;8 intentos SELECT anteriores conservados. No editar token/estado/stop por SQL manual ni repetir preparación/recepción. La ventana de600 HTTP/2 sesiones/60 SELECT y una creación sigue siendo una propuesta que exige autorización propia. Los cortes posteriores en este archivo son antecedentes.

## DOC-119 — seguimiento de lectura autorizado

Estado vigente2026-11-12: el usuario aceptó expresamente la cobertura mixta preparada de SEG-02; tareas2.5/2.7 completas antes de UI. La reorganización final tiene regresión local99/99 y carga individual/contratos33/33, sin nueva sesión autenticada. Accesos en menú Importación, consulta manual/último lote/verificación dentro del asistente y contextual del nivel pulsado. Ver `Validacion/doc119-ui-verificacion-20261112.md` y guía11. Corte real29623218 sobre74083c36: recuperación FINISHED_WITH_LIMITS,281HTTP/27SELECT/1sesión, sin fallo; mantiene límites de inserción nueva y raíces físicas homónimas, complementados localmente por decisión expresa. No repetir automáticamente pruebas pasadas ni recrear lote29/plan45; la aceptación no amplía efectos.

Antecedente histórico eb9311ca (superado por29623218): TreeView WebForms emite enlace de imagen tNi y texto tN con el mismo título. Producto/observador identifican solo nodos lógicos tN; no confundir enlaces con duplicados reales ni ocultar dos nodos lógicos distintos. El ASPX versiona el JS del árbol y ambos ejecutores cotejan su hash servido. En errores de árbol conservar únicamente el diagnóstico cerrado de disponibilidad/conteos y selección/expansión; jamás DOM, títulos, cabeceras o secretos. La corrida anterior280HTTP/19SELECT/1sesión quedó cerrada y la repetición29623218 ya fue revisada; no iniciar otra por este antecedente. Detalle `doc119-recuperacion-arbol-20261112.md`.

Estado posterior ccb25ec9: SEG-01 aceptado, lote29/plan45. No repetir la preparación de accc9ae9: sus dos intentos confirmados (RECEPCION y CREAR_EXPEDIENTE) son los esperados, sin documentos almacenados. `run-doc119-datos-aceptacion.cjs --run <32-hex-nuevo> --authorized-plan --resume-readonly accc9ae9f3344d7a96123ed7cd986a79` conserva los efectos y solo los verifica/consulta. Para continuar únicamente recuperación, usar `--recovery-only ccb25ec9d37a4da083caccfbe80001b1`: verifica SEG-01 aprobado y su preparación original, no repite SEG-01 ni pide Carolina,600HTTP/60SELECT/2sesiones. Guardias de lectura bloquean efectos. Las huellas incorporan metadatos origen_doc/bytes_recibidos mediante SELECT acotado por lote/archivo_id; presupuestos no se reinician dentro de una ventana. La rama original no disponible se registra como límite y no impide consultar/reseleccionar sin enviar; no acredita inserción/deduplicación no observada. Nueva ventana manual expresa después del fallo; detalle en `doc119-seg01-aceptado-recuperacion-pendiente-20261112.md`.

Ampliación expresa del usuario 2026-11-12: `doc119-datos-aceptacion-propuesta-20261112.md` autoriza preparar un lote de27 archivos/dos raíces, recibir únicamente un PDF de677 bytes y crear únicamente un expediente/relación en destino7, para propietario141/empresa2; perfil26/version40/gabinete10. `run-doc119-datos-aceptacion.cjs --run <32-hex-nuevo> --authorized-plan` usa una consola enmascarada y conserva el consumo SETUP400HTTP/40SELECT/1sesión. El transporte específico coteja manifiesto, confirmación, lote, raíz y bytes; rechaza replay de efectos, ALMACENAR y otros efectos. Revisa elegibilidad/valores antes de confirmar; ante fallo conserva referencias y se detiene sin repetir. Después abre ventanas independientes SEG-01/02 con los límites de lectura existentes, Carolina136 y originales locales ya preparados. No ejecutar el runner REC completo como sustituto: incluye almacenamiento no autorizado. `--check` no usa red. La composición de limpieza permanece deshabilitada; el usuario confirmó conservar los datos, sin borrado ni cuota/plazo nuevos.

Preparación4ffe5447 cerrada por400 en PrepararPlan; diagnóstico posterior4SELECT/0HTTP confirma ausencia de planes recientes y referencia de tipología001 inválida en el inventario anterior. La revisión2 `doc119-datos-revision2-20261112.md` usa códigos distintos y PDF DOC119-79.pdf en rutas distintas, con originales locales verificados; registra su revisión/hash de manifiesto. No cubre raíces físicas homónimas y no permite dar por aceptado SEG-02 completo. Correlacionar la copia local por nombre+ruta, nunca por el primer nombre igual. La revisión2 no se ejecuta automáticamente tras el fallo; requiere nueva ventana manual expresamente instruida.

El selector `node tools/e2e/scripts/run-doc119-seguimiento.cjs --check` valida preparación sin red ni secretos. Una corrida requiere plan explícito y consola visible con `--run <32-hex> --authorized-plan`; no reutilizar un runId. El plan DOC-119 vigente está en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc119-plan-aceptacion-20261111.md`.

Reutiliza consola, openSession, queryRows y controles de lectura existentes. Captura cuentas/claves y ruta de reselección enmascaradas; no usar chat, `.env`, archivos de sesión o credenciales de Web.config. URL cerrada al localhost autorizado. Certificado local solo se acepta si el operador lo elige expresamente. Modo lectura no autoriza recepción, creación, almacenamiento, permisos, fallos inyectados o limpieza.

Primero autentica al propietario y lista hasta 30 importaciones propias por fecha UTC, destino, estado y cantidades, con usuario/empresa y facultades actuales. El operador elige opciones visibles, sin ingresar IDs internos. La sesión de descubrimiento se reutiliza y sus HTTP/SELECT consumen el mismo presupuesto, sin reset. Hasta resolver la selección no se permiten ASMX de importación. Recuperación ofrece archivos por recibir y destinos alternos por nombre; permite omitir el caso si falta el archivo original. La segunda cuenta se pide al llegar a la comprobación de privacidad.

La guardia permite únicamente los ASMX de lectura del lote/plan/nivel elegidos y postback de selección de nivel en SEG-02; bloquea todos los ASHX y botones de negocio. Consumos persistentes por ventana, ampliación autorizada expresamente el 2026-11-12: SEG-01 600 HTTP/80 SELECT/2 sesiones; SEG-02 600 HTTP/60 SELECT/2 sesiones. Una violación/fallo detiene sin retry. Estado saneado en `.tmp/doc119-seguimiento/<runId>/state.json`, excluido de Git; registra revisión/topes de autorización y jamás contiene formularios, CSV, cuentas, claves, cookies o tokens.

La referencia exacta a la hoja de fuente Google del login se aborta localmente sin enviarla, sin consumir HTTP de aplicación y sin detener la guardia. No es una autorización de acceso externo: otras solicitudes externas y cualquier método distinto de GET siguen deteniendo la corrida. Tras un fallo no reiniciar automáticamente: una nueva ventana manual debe tener una instrucción expresa del usuario y conservar los consumos/resultados de la anterior.

El acceso normal incluye POST `WebServiceLoginSegundoFactor.asmx/ObtenerEstado` con JSON exactamente `{}`, contado como HTTP de aplicación; no se permiten Verificar/Reenviar/Cancelar ni campos adicionales por esa excepción. No interpretar la consulta de estado como autenticación completada. La navegación al destino fijo `Defaul/WebFormInicioDocuarchiGestion.aspx`, sin parámetros y solo GET/HEAD, se resuelve localmente con HTML vacío: conserva la cookie del login real y evita ejecutar inicialización/servicios del panel de inicio; el helper compartido abre después Producción Documental real. No se sustituye ninguna respuesta del módulo ni de sus APIs de seguimiento. El estado registra `homeSkipped`.

Producción Documental referencia seis dependencias públicas versionadas en CDN. Solo se permiten GET de script/stylesheet a las seis URLs exactas de `STATIC_DEPENDENCIES` en el helper DOC-119, coincidentes con el ASPX; se eliminan Cookie, Authorization, Proxy-Authorization, Referer y Origin antes de enviarlos. Cada GET consume el presupuesto HTTP existente y registra solo su código fijo. Otras URLs, métodos, parámetros o tipos de recurso siguen bloqueados. La referencia Google Fonts sigue omitida localmente. No se habilitan servicios del panel de inicio ni endpoints de negocio adicionales.

El diagnóstico de guardia conserva únicamente categorías/métodos/operaciones de una lista fija, sin URL, parámetros, formularios ni valores privados. Antes de volver a solicitar secretos tras un bloqueo de carga, comprobar la página de acceso y sus dependencias públicas sin enviarlos.

Preflight debe acreditar recursos adecuados. Falta de segunda página, recepción por completar, raíz confirmada, rama disponible o PDF correcto no permite simular aceptación ni crear recursos por inferencia. No marcar SEG-01/02 aprobados por `--check` o pruebas locales. Un resultado con límites conserva esos límites; la reorganización UI espera aceptación integrada documentada.

La sesión local caduca a los 10 minutos de inactividad. El ejecutor avisa al mostrar elecciones y rechaza reutilizar la sesión propietaria después de un margen de 8 minutos, antes de los controles SELECT y de usarla; no cambia Web.config ni hace keepalive/relogin automático. Un rechazo de ConsultarLote al esperar recursos se informa directamente (401 como SESSION_REQUIRED, acceso denegado como RESOURCE_ACCESS_DENIED), sin esperar recursos que el cliente no solicitará ni conservar el cuerpo.

Con pocos archivos, SEG-01 puede comprobar filtros, CSV de la primera página filtrada, vacío, cuenta ajena y huellas. Si no existe segunda página, registra `pagination=false`, `INSUFFICIENT_EXISTING_ROWS` y `PASS_READONLY_WITH_LIMITS`, sin aceptar SEG-01 completo ni abrir la compuerta UI. Omitir recuperación con 0 queda registrado como NOT_EXECUTED incluso si SEG-01 falla después.

Para una nueva consola manual se puede usar `--reuse-selection <runId-cerrado>` después de `--authorized-plan`: solo reutiliza el ID de importación elegido y la decisión de omitir recuperación, de una corrida local cerrada con esta revisión de autorización. No restaura claves, sesión, resultados, huellas ni consumos; los anteriores permanecen intactos. La importación debe aparecer entre los recursos de la cuenta recién autenticada y superar de nuevo los controles de autoridad/propiedad antes de consultarla. No se usa con una recuperación anterior que requiera PDF.

Cuando el usuario solicita identificar una segunda cuenta, `inspect-doc119-second-accounts.cjs --run <32-hex> --source-run <runId-principal>` reutiliza consola y queryRows para **dos SELECT como máximo**, cero HTTP y cero aperturas de autenticación de aplicación. Resuelve propietario/empresa desde el lote de la corrida autorizada y lista hasta 30 cuentas activas de esa empresa con IMPORTAR_CARPETAS=1, identidad/perfil únicos y distintas del propietario; excluye consulta pública. Captura solo DSN y acceso MySQL de lectura por consola enmascarada, sin recuperar secretos de Web.config ni del proceso principal. No concede permisos ni prueba claves de aplicación.

La consulta auxiliar conserva su propio consumo vinculado a la corrida principal; contabilizarlo aparte y sumarlo al informar lecturas totales del trabajo. `state.json` no conserva cuentas/claves; el inventario solicitado de IDs y logins, sin contraseñas ni otros campos personales, queda en `.tmp/doc119-account-lookup/<runId>/account-candidates.json`, excluido de Git. Las candidatas verificadas por SELECT todavía necesitan credenciales conocidas por el operador y autenticación real; no presentar un antecedente histórico como permiso vigente.

Antes de la segunda sesión, el ejecutor principal reutiliza ese mismo inventario mediante dos SELECT dentro de sus 80 permitidos. Si hay una candidata, usa directamente el login canónico de la base; si hay varias, el operador elige una opción de la lista verificada. Ya no solicita escribir el nombre de la segunda cuenta. Muestra al operador el identificador elegido y pide solo su contraseña, enmascarada; no registra el login ni la contraseña en el diario principal, únicamente userRef/companyRef y CURRENT_RA_SELECT. Sin candidatas detiene antes de abrir otra sesión. ACCOUNT_NOT_FOUND sigue siendo fallo de autenticación, no denegación por propiedad ni aprobación del caso negativo; no cambia contraseñas ni hace retry automático.

Este runbook permite reutilizar las pruebas ASMX Workflow sin copiar credenciales, modificar datos sin autorización ni reactivar la política legacy.

## Límites obligatorios

### DOC-120: diagnóstico de creación sin confirmación

`node tools/e2e/scripts/inspect-doc120-final-state.cjs --lot <id> --root <id> --authorized` prepara una ventana específica de máximo5 SELECT en RA, cero HTTP/sesiones de aplicación y cero efectos. Exigir autorización explícita del ambiente/DSN, lote y raíz antes de ejecutarla; `--authorized` declara esa autorización, no la sustituye ni hereda otra corrida. El operador introduce DSN y credenciales MySQL exclusivamente SELECT en consola interactiva enmascarada. No recuperar secretos de Web.config, cookies o procesos anteriores. Sin autorización no lanzar ni consultar.

Reutiliza queryRows y el guard de SELECT. Limita la lectura a lote/raíz/archivos/intentos/eventos autorizados, elimina JSON/mensajes privados y conserva solo referencias, estados y fechas necesarios en `.tmp/doc120-final-state/<runId>/`, excluido de Git. STOPPED no se reintenta automáticamente. No libera intentos, reconcilia, crea ni reenvía; CREANDO/EN_CURSO y la antigüedad no permiten inferir ausencia del efecto. Un diagnóstico terminado no equivale a recuperación del lote ni a una prueba E2E de importación.

Corrección del reporte2026-11-14: conserva únicamente categorías cerradas del puente como `DOC120_DIAGNOSTIC_DB_AUTHENTICATION_FAILED`, `DSN_UNAVAILABLE`, `COLUMN_UNAVAILABLE` o sus otras categorías aprobadas, nunca el mensaje del controlador. `query` identifica la lectura del libro fijo; `select` contabiliza intentos antes de abrir ODBC y `completedSelect` las respuestas de filas recibidas. Un intento no demuestra que el servidor haya ejecutado SQL. La corrida64c70664 quedó STOPPED con1 intento contabilizado y error genérico; no se puede reconstruir retrospectivamente su causa. Otra ventana necesita instrucción expresa y conserva ese consumo, sin restaurar credenciales.

Diagnóstico933961f9,2026-11-14, autorizado expresamente después de STOPPED: FINISHED con5 SELECT concluidos,0 HTTP/0 sesiones;6 intentos presupuestados acumulados con64c70664. Lote39/carpeta60 conserva intento96 RESERVADO, sin checkpoint de inicio, y señal de detención posterior. El diagnóstico no libera intentos ni prueba ausencia del efecto;3.27 acredita estado persistido, no recuperación. Ventana cerrada y credenciales eliminadas. No abrir más lecturas o efectos por inferencia; evidencia saneada doc120-diagnostico-lote39-20261114.json.

Auxiliar de prerrequisitos DOC-120: `inspect-doc120-creation-prerequisites.cjs --lot 39 --root 60 --attempt 96 --authorized`, preparación3.29 local23/23. Nueva ventana propuesta, todavía sin autorización: hasta30 SELECT RA/DA incluyendo metadatos y catálogos;0 HTTP/sesiones/efectos,6 intentos anteriores conservados (máximo36 acumulados). Reutiliza las clases reales con delegado a queryRows y lista cerrada de tablas; compila solo el auxiliar excluido. Snapshot/valores/token viajan efímeros por IPC, no se guardan. La proyección previa a la detención es contrafactual: no acredita la sesión/conexión legacy ni la transacción real, no libera/reconcilia ni crea. Credenciales de lectura enmascaradas y huellas de DLL/auxiliar antes/después. Ver `Validacion/doc120-prerrequisitos-plan-20261114.md`; STOPPED sin retry automático.

Ventana704fde28 autorizada por «continua», cerrada STOPPED:1 SELECT de metadatos concluido, rechazo CONFIGURACION_INVALIDA en COMMAND antes de catálogos,0 HTTP/sesiones/efectos, huellas estables y secretos eliminados.7 intentos acumulados; no repetir automáticamente. Refinamiento local26/26 prepara `--command-only --authorized` (opciones finales tras --attempt 96): nueva ventana de **1 SELECT RA**,0 catálogos/HTTP/efectos, máximo8 acumulados. Solicitud de autorización sin respuesta al registrar este corte. El diagnóstico del guard reutiliza validadores reales y conserva solo campos/códigos cerrados y conteos; no valores ni snapshot/huellas privadas. Ver `Validacion/doc120-comando-detalle-plan-20261114.md`.

Ventanaf23baf65 autorizada expresamente, cerrada STOPPED tras1 SELECT RA: ROOT_FIELD_RULE/FECHA_INICIAL/LONGITUD_EXCEDIDA, valor canónico de10 caracteres.8 intentos acumulados,0 HTTP/sesiones/efectos; secretos eliminados. ValidarValorResuelto corregido sobre copia y29/29 locales, sin nueva lectura real. El lote39/reserva96 no se modifica; no repetir ahora el selector ni crear otro lote. La propuesta doc120-recuperacion-reserva96-propuesta-20261114.md no está implementada ni autoriza efectos. Cualquier reanudación necesita mecanismo revisado y ventana explícita; no liberar ni alterar tokens/stop por SQL manual.

No activar, editar ni limitar el gate fuera del runner aprobado y de una autorización explícita.

- Ejecutar solo contra un ambiente de pruebas autorizado; nunca inferir autorización para producción.
- Recibir URL, cuentas y acceso MySQL de solo lectura mediante secretos del entorno o instrucción expresa. No crear `.env` ni mostrar secretos, cookies o cadenas de conexión.
- Reutilizar `tests/support/authenticated-workflow-session.cjs` para todo login E2E DOC-10/DOC-11/DOC-28/DOC-32/Notas. No enviar usuario, grupo, ruta, actividad ni permisos al ASMX salvo el destino que DOC-28 obtiene del preview actual al ejecutar.
- `PreviewEnviarTarea` y `PreviewEnviarUsuario` son de solo lectura. Las consultas de control son una única sentencia `SELECT` con exactamente un parámetro `?` para la tarea.
- Las suites de preview no modifican el flujo legacy. Un cambio de modernización aprobado puede ajustar `workflow/`, pero debe conservar la semántica del fallback y demostrar ausencia de doble operación.
- La experiencia moderna es oficial y no tiene gate configurable ni listas de audiencia. Los runners no pueden reintroducir esas claves; cualquier ajuste temporal permitido debe restaurar exactamente `Web.config` en `finally`.

## Preparación

```powershell
npm.cmd --prefix tools/e2e install
npm.cmd --prefix tools/e2e run install:browsers
```

Las variables de sesión se entregan por secretos efímeros:

```powershell
$env:DOC10_E2E_BASE_URL = 'https://ambiente-pruebas/app/'
$env:DOC10_E2E_MODULE = 'GESTOR'
$env:DOC10_E2E_AUTHORIZED_USER = '<cuenta-workflow-valida>'
$env:DOC10_E2E_AUTHORIZED_PASSWORD = '<secreto>'
$env:DOC10_E2E_TASK_ID = '<tarea-activa-autorizada>'
```

Para huellas antes/después, agregar únicamente una cuenta MySQL de lectura y una consulta de auditoría aprobada:

```powershell
$env:DOC10_E2E_MYSQL_URL = 'mysql://usuario_solo_lectura:secreto@host/base'
$env:DOC10_E2E_AUDIT_SQL = 'SELECT COUNT(*) AS total FROM tabla_auditoria WHERE id_tarea = ?'
```

`DOC10_E2E_TASK_STATE_SQL` es opcional y también debe ser un `SELECT` con un único `?`.

## Estado del gate

Al inicio y cierre se comprueba que la configuración permanece apagada y sin alcance. Es un control de integridad, no un mecanismo de habilitación:

```powershell
rg -n "WorkflowCentroTrabajoModern(Active|Users|Groups)" Web.config
```

El resultado esperado es `Active=false` y listas vacías. Si difiere, detener la corrida y solicitar intervención del responsable del ambiente; nunca corregirlo desde la prueba.

## Selección de prueba

| Objetivo | Comando | Requisitos adicionales | Resultado esperado |
| --- | --- | --- | --- |
| Borde sin sesión | `npm.cmd --prefix tools/e2e run test:anonymous` | URL y tarea. | Contexto anónimo rechazado, sin destinos. |
| Sesión Gestión → Workflow | `npm.cmd --prefix tools/e2e run test:session` | Cuenta Workflow válida. | Contexto resuelto sin bloqueo de despliegue. |
| Dos contextos oficiales | `npm.cmd --prefix tools/e2e run test:contexts` | Agregar segunda cuenta Workflow válida. | Ambas sesiones resuelven contexto; la disponibilidad es una regla de negocio. |
| E2E funcional completa | `npm.cmd --prefix tools/e2e run test:e2e` | MySQL de solo lectura y auditoría. | Destinos o bloqueo funcional esperado; huellas iguales antes/después. |
| Concurrencia ASMX | `npm.cmd --prefix tools/e2e run test:load` | Autorización específica de carga y MySQL de solo lectura. | Métricas y huellas iguales antes/después. |
| DOC-28 anónimo | `npm.cmd --prefix tools/e2e run test:doc28:anonymous` | URL autorizada. | Contexto rechazado, sin destinos usuario. |
| DOC-28 validación | `npm.cmd --prefix tools/e2e run test:doc28:validation` | Cuenta Gestión con `CAMBIO_USUARIO`. | Parámetro inválido bloqueado sin transición. |
| DOC-28 preview | `npm.cmd --prefix tools/e2e run test:doc28:preview` | Ambiente/cuenta autorizados, tarea activa y MySQL solo lectura. | Preview usuario sin cambios de estado/auditoría. |
| DOC-28 ejecución | `npm.cmd --prefix tools/e2e run test:doc28:execute` | Autorización explícita, tarea descartable, MySQL solo lectura y `DOC28_E2E_EXECUTION_AUTHORIZED=true`. | Resultado y huellas esperadas, sin tocar gate/legacy. |
| DOC-28 concurrencia | `npm.cmd --prefix tools/e2e run test:doc28:concurrency` | Autorización doble, tarea descartable y MySQL solo lectura. | Dos solicitudes: una `completada` y un bloqueo seguro; estado/auditoría cambian. |
| DOC-29 bloqueo UI | `npm.cmd --prefix tools/e2e run test:doc29:user-send-lock` | Autorización explícita doble, tarea descartable activa en la sesión, MySQL solo lectura, `DOC28_E2E_EXECUTION_AUTHORIZED=true` y `DOC29_E2E_UI_LOCK_AUTHORIZED=true`. | Un solo envío real; X, cancelar, fondo, Escape, API y recarga quedan bloqueados mientras se retiene la respuesta, y se valida el cambio de estado/auditoría. |
| DOC-32 preview | `npm.cmd --prefix tools/e2e run test:doc32:preview` | El comando solicita ambiente/cuenta autorizados, tarea descartable, MySQL solo lectura y presupuesto. | Estado y auditoría sin cambios; evidencia saneada. |
| DOC-32 ejecución | `npm.cmd --prefix tools/e2e run test:doc32:execute` | El comando solicita autorización explícita, primera tarea descartable, MySQL solo lectura y presupuesto. | Ejecución E2E oficial: una devolución real desde conector/token del preview actual. |
| DOC-32 concurrencia | `npm.cmd --prefix tools/e2e run test:doc32:concurrency` | El comando solicita autorizaciones explícitas, segunda tarea descartable, MySQL solo lectura y presupuesto. | Ejecución E2E oficial: dos solicitudes, una transición y un bloqueo seguro. |
| DOC-33 preview UI | `test:workflow:run -- --doc doc33 --profile <perfil> --stages preview --authorize environment` | Ambiente/cuenta autorizados, tarea UI seleccionada y ODBC solo lectura. | El modal moderno representa el preview sin alterar estado ni auditoría. |
| DOC-33 devolución UI | `test:workflow:run -- --doc doc33 --profile <perfil> --stages preview,execution --authorize environment,execution` | Autorización explícita, tarea descartable seleccionada y ODBC solo lectura. | Una devolución real iniciada por el modal moderno; valida estado, auditoría y actividad final. |
| DOC-33 bloqueo UI | `test:workflow:run -- --doc doc33 --profile <perfil> --stages ui-lock --authorize environment,ui_lock` | Autorización explícita y una segunda tarea descartable seleccionada. | Retiene la respuesta de ejecución y bloquea cancelar, cerrar, Escape, modal y abandono hasta recibirla. |
| Notas anónimo | `npm.cmd --prefix tools/e2e run test:notes:anonymous` | URL autorizada. | Validación contractual: contexto rechazado, sin notas ni información interna. |
| Notas lectura | `npm.cmd --prefix tools/e2e run test:notes:read` | Ambiente/cuenta autorizados, tarea con nota visible y MySQL solo lectura. | Validación contractual: listado/consulta sin cambios de estado o auditoría. |
| Notas escritura | `npm.cmd --prefix tools/e2e run test:notes:write` | Autorización explícita, tarea descartable y MySQL solo lectura. | Validación contractual: crear idempotente, editar/conflicto/eliminar. |
| Notas concurrencia | `npm.cmd --prefix tools/e2e run test:notes:concurrency` | Autorización doble, tarea descartable distinta, nota semilla propia y MySQL solo lectura. | Validación contractual: exactamente dos actualizaciones, una efectiva y un bloqueo seguro. |
| DOC-43/44/45 UI Notas | `npm.cmd --prefix tools/e2e run test:doc44:workflow-notes` | Ambiente, cuenta y tarea descartable expresamente autorizados; datos para lecturas negativas y una nota ajena perteneciente a la misma tarea. | La presentación moderna es única; una nota propia extensa valida lectura completa y una nota ajena valida ausencia de acciones y rechazo `NotOwner`, sin modificar el gate. |
| DOC-95 Login 2FA ASMX | `npm.cmd --prefix tools/e2e run test:doc95:second-factor -- --profile <perfil>` | Autorización nueva para ambiente, cuenta, buzón, challenge, carrera exacta y datos descartables. | Postback real crea el reto, estado saneado pendiente y exactamente una de dos verificaciones del mismo OTP completa el login. |

## Cierre de cada corrida

1. Conservar solo evidencia resumida sin secretos ni cuerpos de respuesta.
2. Registrar ambiente, tipo de tarea, resultado y huellas cuando la evidencia pertenezca al cambio en curso.
3. Confirmar que el gate siguió apagado y las listas vacías con el comando anterior.
4. Ejecutar `git diff --name-only -- workflow/Webworkflow.aspx workflow/Webworkflow.aspx.vb`; solo DOC con retiro o modernización explícitamente trazada puede contener esas rutas. Cualquier cambio no declarado detiene la corrida.
5. No detener procesos residuales sin autorización explícita.

## DOC-56 importación SII

DOC-56 reutiliza `test:workflow:platform`; no tiene login, `.env`, conexión ni configuración Playwright propios. Sus perfiles de ejemplo `doc56-import-sii-*.profile.example.json` deben copiarse dentro de `tools/e2e/profiles/` con otro nombre y completar únicamente ambiente, DSN, tarea descartable, radicado y presupuestos no sensibles.

La lectura exige `--authorize environment,gate`; la ejecución exige `--authorize environment,gate,execution,discardable-resource`; la carrera fija de dos solicitudes exige `--authorize environment,gate,execution,concurrency,discardable-resource`. Cada valor se confirma nuevamente por TTY. El runner captura allí las cuentas, habilita temporalmente el gate local y restaura en `finally` el contenido original antes de validar gate apagado, alcance vacío y páginas legacy intactas.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-read --profile <perfil.json> --authorize environment,gate
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-execution --profile <perfil.json> --authorize environment,gate,execution,discardable-resource
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-concurrency --profile <perfil.json> --authorize environment,gate,execution,concurrency,discardable-resource
```

## DOC-11 mutante

`EjecutarEnvioTarea` cambia la tarea. Solo puede ejecutarse con autorización explícita para una tarea descartable, cuenta válida, conector y token obtenidos del preview actual, además de consultas de estado y auditoría `SELECT` con un parámetro `?` y MySQL de solo lectura.

Las pruebas anónima y de validación no cambian estado. Ejecución y concurrencia requieren además `DOC11_E2E_EXECUTION_AUTHORIZED=true`; después se confirma que la configuración del gate continuó apagada y el flujo legacy no cambió.

## DOC-28 envío a usuario

`PreviewEnviarUsuario` recibe siempre tarea, filtro, cursor y tamaño de página. En el preview completo DOC-28 se usan `DOC28_E2E_TASK_STATE_SQL` y `DOC28_E2E_AUDIT_SQL`; ambas son obligatorias y el harness rechaza cualquier instrucción distinta de un `SELECT` con un único `?`.

`EjecutarEnvioUsuario` es mutante. Además de los límites generales, exige autorización explícita para el ambiente y cuentas, una tarea descartable, `DOC28_E2E_EXECUTION_AUTHORIZED=true`, resultado esperado y acceso MySQL solo lectura. El test obtiene usuario destino, actividad y token desde el preview de la misma sesión; nunca acepte ni configure esos valores por variables de entorno.

La auditoría propia de DOC-28 se registra en `log_usuario`, con `Mecanismo=ASMX_ENVIO_USUARIO` dentro de `Valor_Log`. La consulta `DOC28_E2E_AUDIT_SQL` debe ser un único `SELECT` con un parámetro `?` que filtre ese mecanismo y `Tarea=<parámetro>`; no use `wf_log_estados_workflow` como sustituto.

`test:doc28:concurrency` es una carrera fija de exactamente dos solicitudes mutantes y no una prueba de carga. Requiere además `DOC28_E2E_CONCURRENCY_AUTHORIZED=true`, autorización explícita de concurrencia, tarea descartable y las dos consultas MySQL de lectura. Debe producir una sola respuesta `completada`, un único bloqueo entre `WORKFLOW_TRANSITION_IN_PROGRESS`, `WORKFLOW_VERSION_CONFLICT` o `WORKFLOW_TASK_UNAVAILABLE`, y cambios de estado/auditoría. No admita niveles configurables, usuarios virtuales ni carga masiva.

`test:doc29:user-send-lock` es una única transición UI DOC-29, no una carrera ni una prueba de carga. Intercepta la respuesta después de que el servidor atendió el POST para mantener el diálogo en estado `enviando`; durante ese intervalo verifica controles deshabilitados, fondo, Escape, API y el guardia `beforeunload`. Exige una tarea descartable seleccionada activamente en la sesión y las dos banderas de autorización. La evidencia saneada solo conserva conteos, banderas y huellas.

La evidencia DOC-28 solo puede guardar códigos, conteos, banderas de cambio y huellas; no guarde destinos, token, cookies, secretos, cadena de conexión ni cuerpos de respuesta. Al cierre, ejecute el control del gate y la comprobación de páginas legacy de la sección anterior.

## DOC-32 — Devolver actividad anterior

DOC-32 usa exclusivamente `tests/support/authenticated-workflow-session.cjs` para la sesión Gestión; no cree un login, cookies ni `.env` alternativos. Antes de ejecutar cualquier comando autenticado, obtenga autorización explícita para el ambiente y la cuenta Workflow. Antes de ejecución o concurrencia, obtenga además autorización explícita para cada tarea descartable.

Los comandos DOC-32 solicitan los datos desde la consola interactiva y los pasan solo a los procesos de la corrida. Las contraseñas Workflow y MySQL se capturan ocultas; no se deben cargar manualmente, persistir con `setx` ni pegar en el chat. El destino de controles es el DSN ODBC no sensible `workflowconta`; nunca se acepta una URL ni una cadena de conexión. Sin TTY, el comando se detiene antes de iniciar sesión o abrir un navegador.

### Corrida unificada reutilizable

Para reutilizar una configuración no sensible entre DOC y ambientes, copie la plantilla `profiles/workflow-e2e.profile.example.json` a una ubicación local externa como `C:\cert\contet.txt`, conserve el contenido como JSON y ajuste únicamente URL, `ignoreHttpsErrors`, módulo, nombre de DSN ODBC, tareas descartables, `previewActivityNames`, `executionActivityName`, `executionFinalActivityName`, `concurrencyActivityName`, consultas `SELECT` y presupuestos. `previewActivityNames` declara la lista exacta y sin repetidos que debe devolver el preview; el harness exige que no haya más páginas ni destinos distintos. `executionActivityName` debe ser una sola actividad de esa lista; el harness deriva el conector y el token desde el preview vigente y falla cerrada si no hay una coincidencia única. `executionFinalActivityName` es el nombre de la actividad activa después de ejecutar (`NOMBRE_ACTIVIDAD`), no el grupo ni el usuario destino; puede diferir de la etiqueta del preview solo cuando la ruta crea otra actividad efectiva. `concurrencyActivityName` obliga a la carrera a elegir una única actividad de su preview vigente en vez de depender del primer resultado. `ignoreHttpsErrors` debe ser booleano y permanecer en `false` salvo que el ambiente autorizado use un certificado autofirmado. El perfil nunca puede incluir cuenta Workflow, contraseña, cookies, token, autorización, URL MySQL ni cadena de conexión.

Con autorizaciones explícitas del responsable para ambiente, primera tarea y segunda tarea, ejecute una sola secuencia DOC-32:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:run -- --doc doc32 --profile C:\cert\contet.txt --authorize environment,execution,concurrency
```

El iniciador valida el perfil, el gate, las consultas, las autorizaciones y el DSN ODBC antes de pedir una sola vez la cuenta Workflow y su contraseña oculta, además del usuario y contraseña MySQL de solo lectura. El perfil conserva solo el nombre del DSN; nunca se solicita, imprime ni persiste una URL o cadena de conexión. Después ejecuta preview, devolución y carrera en orden; si una etapa falla, no ejecuta las posteriores. Los secretos desaparecen al terminar y los controles de cierre se aplican aun cuando la secuencia se interrumpe.

Si las tareas descartables deben ser seleccionadas manualmente antes de cada operación, el responsable puede dividir la secuencia sin guardar autorizaciones ni secretos: `--stages preview,execution --authorize environment,execution` para la primera tarea, y después `--stages concurrency --authorize environment,execution,concurrency` para una segunda tarea ya seleccionada. Cada invocación solicita los secretos una sola vez y conserva los mismos controles de cierre.

### Recursos reutilizables para cualquier E2E

Una etapa mutante debe declarar un contrato de recurso registrado, no un comando ni una consulta escrita en el perfil. El contrato define el descriptor no sensible, el preflight de solo lectura, el alcance de reserva y si una transición consume el recurso. La plataforma reserva el recurso antes de iniciar la etapa mutante y lo libera al fallar o lo marca consumido al completar; solo un preflight que observe una nueva generación preparada puede habilitarlo de nuevo.

La reserva `local` protege corridas en este espacio de trabajo. Para un recurso compartido entre equipos se requiere un coordinador compartido registrado; si falta, la corrida falla cerrada. No implemente restauraciones de datos de negocio en el núcleo: un adaptador puede declarar una restauración segura y autorizada, o el responsable prepara otro recurso. Para registrar un escenario no Workflow, añada su adaptador y pruebas de política; no herede tareas, actividades ni consultas DOC-32.

`DOC32_E2E_TASK_STATE_SQL` y `DOC32_E2E_AUDIT_SQL` deben ser cada una una sola sentencia `SELECT` con exactamente un parámetro `?`. La cuenta MySQL es solo de lectura. La consulta de auditoría debe apuntar al registro adicional `ASMX_DEVOLVER_ACTIVIDAD`; no sustituirlo por el log histórico del motor.

## DOC-33 — Devolución por interfaz moderna y bloqueo de respuesta

DOC-33 reutiliza la sesión autenticada y el ODBC de solo lectura de DOC-32, pero opera el modal oficial de **Elegir actividad anterior**. No agrega login, cookies, `.env`, URL de conexión ni atajos de backend. El perfil contiene solo configuración no sensible y declara dos tareas distintas: una para la devolución UI y otra exclusivamente para el escenario de bloqueo.

Para derivar un perfil DOC-33 desde un perfil **JSON no sensible** DOC-32 existente sin copiar secretos, cree una vez el archivo persistente y editable:

```powershell
node tools/e2e/scripts/create-doc33-workflow-ui-profile.cjs --source C:\cert\doc32-e2e.profile.json --destination C:\cert\doc33-e2e-852.profile.json --execution-task 852 --lock-task 843
```

El archivo fuente debe seguir `profiles/workflow-e2e.profile.example.json` y no puede contener cuentas ni contraseñas. Un archivo heredado `clave=valor` con credenciales no es un perfil E2E y se rechaza antes de abrir el navegador. La primera tarea queda como `uiExecutionTaskId=852`. La segunda es una reserva para una futura E2E de bloqueo y debe reemplazarse por una tarea descartable diferente, preparada y seleccionada por la cuenta autorizada. No ejecute ambas etapas sobre la misma tarea ni use una tarea ya devuelta.

Después de autorización explícita del ambiente, la cuenta y la tarea 852, la devolución UI se inicia con una sola corrida:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:run -- --doc doc33 --profile C:\cert\doc33-e2e-852.profile.json --stages preview,execution --authorize environment,execution
```

La etapa `ui-lock` se ejecuta únicamente después de preparar la segunda tarea declarada y de obtener autorización explícita para ella:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:run -- --doc doc33 --profile C:\cert\doc33-e2e-852.profile.json --stages ui-lock --authorize environment,ui_lock
```

La prueba intercepta la respuesta **después** de enviar el único POST al servidor. Desde el navegador, esto reproduce una ejecución cuya respuesta sigue pendiente: la confirmación, el fondo, Escape, el cierre API, el cierre del modal y `beforeunload` deben permanecer bloqueados. Al liberar la respuesta, verifica una transición, estado/auditoría y actividad final por ODBC. La evidencia conserva solo conteos, banderas, códigos funcionales, latencias y huellas; no destinos, actividades, tokens, cuerpos, cookies ni credenciales.

Primero, el preview real no mutante:

```powershell
npm.cmd --prefix tools/e2e run test:doc32:preview
```

Después, con autorización de la primera tarea descartable, ejecute la transición oficial. El comando pedirá confirmación literal `SI`:

```powershell
npm.cmd --prefix tools/e2e run test:doc32:execute
```

Finalmente, la segunda tarea descartable debe ser distinta y nueva. La carrera no es una prueba de carga ni admite usuarios virtuales o niveles configurables:

```powershell
npm.cmd --prefix tools/e2e run test:doc32:concurrency
```

El harness toma `IdConector` y token exclusivamente del preview actual. La evidencia solo conserva códigos, conteos, banderas, latencias y huellas. Al cierre, comprobar el gate en `false` con usuarios/grupos vacíos y ejecutar `git diff --name-only -- workflow/Webworkflow.aspx workflow/Webworkflow.aspx.vb`; no debe haber salida.

### Registrar otro DOC

Cada DOC adicional debe declarar en el registro común su lista estricta de campos de perfil no sensibles, las etapas ordenadas, las autorizaciones de cada etapa, el mapeo de variables efímeras y las pruebas propietarias que se invocan. El perfil no puede seleccionar scripts ni comandos. Antes de habilitarlo, agregue pruebas de perfil, autorización, orden, limpieza de secretos y evidencia saneada; conserve las pruebas específicas del DOC sin alterar sus semánticas.

## Notas Workflow

Las pruebas de Notas usan exclusivamente `tests/support/authenticated-workflow-session.cjs` y los contratos modernos de Notas con `idTarea` explícito. DOC-44 posee la regresión E2E exclusiva del consumidor de Notas; DOC-32 conserva su transición de devolución de actividad. No se permite usar `Session("ID_TAREA_SELECCIONDA")`, endpoints legacy, login alterno, `.env` ni valores de usuario, grupo, permiso o autor entregados por el cliente.

Los comandos `test:notes:*` precargan desde valores no sensibles la raíz local de Gestión, módulo y ambiente `GESTOR` (se pueden sustituir por variables efímeras no sensibles del mismo nombre) y solicitan directamente en una consola interactiva solo los datos de cuenta, las confirmaciones y las tareas. La contraseña Workflow y la contraseña MySQL se capturan ocultas; el control de base de datos reutiliza exclusivamente el DSN ODBC no sensible `workflowconta` y solicita el usuario MySQL de solo lectura. No se acepta URL ni cadena de conexión MySQL, ni se deben cargar secretos manualmente, persistir con `setx` ni pegar en el chat. Sin TTY, el comando se detiene antes de iniciar sesión o abrir un navegador.

Por defecto TLS se valida. Solo con autorización explícita para un certificado autofirmado local puede establecerse temporalmente `NOTES_E2E_IGNORE_HTTPS_ERRORS=true` en el proceso de la corrida; el navegador y el cliente HTTP ASMX comparten entonces esa excepción, conservando la sesión en memoria y sin persistir certificados ni secretos.

Antes de cualquier modo autenticado, el responsable debe contar con autorización del ambiente, cuenta válida, acceso MySQL de solo lectura y las tareas aprobadas. El iniciador los solicitará en esa consola, incluidos los presupuestos; pedirá confirmación literal `SI` para ambiente, escritura y concurrencia antes de asignar las banderas correspondientes.

`NOTES_E2E_SERVICE_PATH` es opcional y, si se omite, la suite usa `webservice/WebServiceWorkflowNotesModern.asmx`. La suite fija `NOTES_E2E_ODBC_DSN=workflowconta` y registra sus dos controles no sensibles: el estado toma metadatos de `ANOTACION_TAREA` sin `DATO_ANOTACION`, y la auditoría toma metadatos de `wf_log_workflow` sin `datos_operacion`; ambas sentencias son un único `SELECT` con exactamente un parámetro `?`. La cuenta MySQL del DSN es solo de lectura.

Primero ejecute el borde anónimo y la lectura real no mutante. Cada comando preguntará solo los valores necesarios. La lectura usa una tarea con al menos una nota visible y debe conservar las huellas de estado y auditoría:

```powershell
npm.cmd --prefix tools/e2e run test:notes:anonymous
npm.cmd --prefix tools/e2e run test:notes:read
```

Escritura y concurrencia son mutantes. Requieren autorización explícita del ambiente y de cada tarea descartable, solicitadas por el iniciador. La concurrencia usa una nota semilla propiedad de la cuenta autorizada y ejecuta exactamente dos actualizaciones con la misma versión; no es una prueba de carga:

```powershell
npm.cmd --prefix tools/e2e run test:notes:write
npm.cmd --prefix tools/e2e run test:notes:concurrency
```

La regresión DOC-44 agrega lecturas negativas sobre tarea ajena/inactiva y nota cruzada, además de un CRUD sobre una tarea descartable. Requiere tres confirmaciones independientes y restaura el gate aunque falle:

```powershell
npm.cmd --prefix tools/e2e run inspect:doc44:test-data
npm.cmd --prefix tools/e2e run test:doc44:workflow-notes
```

La evidencia de Notas solo conserva modo, códigos, conteos, latencias, banderas y huellas. No guarda el contenido de notas, credenciales, cookies, tokens, usuarios, destinos, cadenas de conexión ni cuerpos HTTP. Al cierre aplique todos los controles de la sección "Cierre de cada corrida": gate apagado, listas vacías y fallback legacy disponible.

## DOC-81 importación de anexos ENLASE

DOC-81 reutiliza `test:workflow:platform` con el escenario `import-sii-enlase-execution`. Exige ambiente, gate temporal, ejecución y recurso descartable; cada autorización se confirma por TTY. El perfil no contiene secretos y la tarea debe estar seleccionable por el contexto oficial `ENLASE` (`HiddenIdFlujo`).

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-enlase-execution --profile <perfil-runtime.json> --authorize environment,gate,execution,discardable-resource
```

Los controles SQL permanecen registrados y son únicamente `SELECT`. La corrida confirma intención idempotente, documento con evidencia lógica/física, ausencia de efectos de expediente y preservación del estado de la tarea. En `finally` restaura el gate a `false`, con usuarios y grupos vacíos, y conserva evidencia saneada.

## DOC-83 asignación explícita ENLASE

DOC-83 reutiliza la misma sesión, TTY, ciclo de recursos y restauración con `import-sii-enlase-assignment`. Es una etapa mutadora independiente de la importación y requiere otra tarea descartable. No llama al servicio SII: acciona `enlase-assign-action`, deja que `Buttonaceptar_Click` revalide los documentos obligatorios y compara un `SELECT` registrado sobre `estados_tarea_workflow`.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-enlase-assignment --profile <perfil-runtime.json> --authorize environment,gate,execution,discardable-resource
```

`assignmentResult=BLOCKED` solo es válido con el control intacto y un aviso funcional observado. `assignmentResult=ASSIGNED` exige cambio en el control. La evidencia no guarda el texto del aviso ni identidades. Cualquiera de los caminos consume conservadoramente el recurso para impedir una repetición accidental; una validación exitosa posterior necesita otra tarea descartable y una autorización nueva.
## DOC-83 aceptación visual ENLASE

La reintroducción manual de `WorkflowCentroTrabajoModernActive` está prohibida. Para comprobar que una importación aparece inmediatamente en `GridView_list_documento_relacion` sin recargar, use exclusivamente `import-sii-enlase-manual-visual`.

La etapa es mutadora: exige autorización de ambiente, gate, ejecución y recurso descartable, cuenta Workflow, MySQL de solo lectura y una tarea ENLASE operable. El navegador visible pertenece al runner; no abra otra sesión ni cambie de tarea. El plazo total es de diez minutos.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-enlase-manual-visual --profile <perfil-runtime.json> --authorize environment,gate,execution,discardable-resource
```

El runner espera que WebForms termine su actualización, abre la ventana moderna y solo muestra el primer hito cuando la lista SII contiene al menos una acción `Importar/Reimportar`; no retira ni oculta documentos existentes. Use esa acción sin recargar y confirme `SI` únicamente cuando aparezca una fila nueva. El runner exige una llamada real al inserter con destino `rad`, ausencia de postback y los ocho campos de `idd_rad`. En el segundo hito, abra una vez el documento agregado y confirme `SI` únicamente si no apareció error. No asigne, elimine, reemplace ni cambie tipología durante esta corrida.

El runner compara los siete controles de importación, consume la reserva solo si termina correctamente y restaura en `finally` el gate a `false`, con usuarios y grupos vacíos. Timeout, respuesta distinta de `SI`, cierre del navegador, recarga, fila ausente, contrato incompleto, diálogo de error o huellas inesperadas producen un fallo saneado.

## DOC-83 revisión visual de la tabla SII

Para revisar textos extensos, preparación masiva y preview sin importar documentos, use `import-sii-enlase-layout-review`. Es una etapa de lectura: abre un navegador visible, exige que las celdas contengan el texto con elipsis, que Acciones permanezca fija y opaca, que el desplazamiento sea interno, que el footer de preparación permanezca visible y que el preview sustituya la lista con retorno explícito. Cada confirmación manual solo se solicita después de sus controles automáticos.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario import-sii-enlase-layout-review --profile <perfil-runtime.json> --authorize environment,gate
```

La corrida no llama `CreateImportIntent` ni `ExecuteImportIntent`, compara como invariantes los siete controles y restaura el gate en `finally`. El runner selecciona y abre la preparación, la cancela, abre un preview y vuelve a la lista. No pulse manualmente Preparar, Crear intención, Importar ni Reimportar durante esta revisión; limite su interacción a responder las tres confirmaciones de consola después de observar cada hito.

## DOC-87 — Registro de recibo SII para ruta

DOC-87 reutiliza `test:workflow:platform` mediante `registro-ruta-sii-execution`. No usa el spec Playwright aislado como recorrido oficial. Copie el perfil de ejemplo dentro de `tools/e2e/profiles/` con otro nombre y configure únicamente URL, módulo, ambiente, los dos DSN no sensibles, recibo SII descartable, actividad y presupuesto.

La plataforma solicita por TTY la cuenta Workflow, contraseña y la cuenta MySQL de solo lectura. Reserva el recibo mediante una huella local, valida por `SELECT` el outbox/registro público y la relación Docuarchi, bloquea recibo editado y actividad `0`, ejecuta una sola alta desde la UI y reintenta el mismo comando para probar idempotencia. No activa ningún gate.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario registro-ruta-sii-execution --profile <perfil-runtime.json> --authorize environment,execution,discardable-resource
```

La evidencia conserva exclusivamente códigos, conteos, huellas y eventos de reserva. El recibo, datos SII, credenciales, cookies y cadenas de conexión no se escriben en artefactos.

## DOC-88 — Carga en Producción Documental

DOC-88 reutiliza `test:workflow:platform` mediante `production-document-upload-preview`. Copie `doc88-production-document-upload.profile.example.json` dentro de `tools/e2e/profiles/` con otro nombre y configure únicamente URL, módulo, ambiente, DSN no sensible, identificador y etiqueta exacta del expediente de prueba, tipología, PDF del directorio `fixtures` y presupuesto. El perfil no admite rutas externas, SQL ni secretos.

La plataforma solicita por TTY la cuenta de aplicación, contraseña y la cuenta de lectura. Antes de abrir el navegador exige confirmaciones separadas para ambiente, cuenta y archivo. Ejecuta únicamente un control `SELECT` parametrizado sobre `registro_producion_documental`; no reserva un recurso mutante.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario production-document-upload-preview --profile <perfil-runtime.json> --authorize environment,account,discardable-file
```

La corrida comprueba rechazo sin selección, contexto correspondiente al expediente autorizado, apertura del cargador, preparación local del PDF y selección exacta de la tipología. No activa `Guardar`. Una ruta defensiva aborta cualquier solicitud inesperada al handler antes de que llegue al servidor y detiene la corrida. El control de persistencia debe permanecer sin cambios.

La evidencia conserva solo códigos, conteos, cambio de huellas y eventos de reserva. No persiste expediente, nombre del archivo, contenido, tipología, credenciales, cookies ni cadenas de conexión.

La carga positiva real se ejecuta en un escenario separado y requiere además autorizaciones de ejecución y recurso descartable. Activa `Guardar` una sola vez, valida la respuesta del handler, la proyección visual y el cambio del control `SELECT`. No reintenta si el almacenamiento es rechazado.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario production-document-upload-execution --profile <perfil-runtime-ejecucion.json> --authorize environment,account,discardable-file,execution,discardable-resource
```

## DOC-89 — Guardado desde escáner en Enlace de documentos

DOC-89 reutiliza `test:workflow:platform` mediante `scanner-link-overlay-execution`. Copie `doc89-scanner-link-overlay.profile.example.json` dentro de `tools/e2e/profiles/` con otro nombre y configure únicamente URL, módulo, ambiente, DSN ODBC no sensible, tarea descartable y presupuesto. No agregue tipología, contenido documental, SQL ni secretos al perfil.

La etapa abre un navegador visible. Después de seleccionar la tarea autorizada, siga el recorrido Enlace de documentos → digitalización → Guardar como y haga doble clic una sola vez sobre `Aceptar`. La plataforma observa exactamente un `ButtonAlmacenar`, un async postback, un indicador compacto sin `.overlay_`, una sola fila nueva, ausencia de navegación y conservación del visor y buffer. El control registrado `workflow-assignment-state` es un `SELECT` parametrizado y debe permanecer sin cambios.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:platform -- --scenario scanner-link-overlay-execution --profile <perfil-runtime.json> --authorize environment,account,execution,discardable-resource
```

La ejecución exige autorización expresa del ambiente, cuenta, tarea y documento digitalizado descartables. La evidencia conserva únicamente códigos, conteos y huellas; nunca credenciales, cookies, contenido o cadenas de conexión.

## DOC-90 — Adjunto en Radicación Entrante clásica

DOC-90 tiene un runner interactivo propio porque debe seleccionar primero la plantilla autorizada de Radicación Entrante en el menú oficial y luego recorrer la selección clásica de pendientes antes de abrir el cargador. Copie `doc90-radicacion-classic-attachment.profile.example.json` dentro de `tools/e2e/profiles/` con otro nombre y configure únicamente URL, módulo, radicado descartable pendiente, tipología, PDF del directorio `fixtures`, conexión MySQL no sensible y dos consultas de control. El `SELECT` de contexto recibe el radicado autorizado y debe devolver estado, estado pendiente, plantilla y nombre de plantilla con los alias del ejemplo. Las consultas deben ser un único `SELECT` con exactamente un parámetro `?`.

El runner solicita por TTY la cuenta de aplicación, contraseña y cuenta MySQL de solo lectura. También exige cuatro confirmaciones independientes: ambiente, cuenta, carga real y recurso descartable. Selecciona el registro por el flujo real de Radicación Entrante, comprueba el evento `ADJUNTARADICACION_CLASICA`, confirma que el multipart no incluye `id_registro_estado_radicacion` ni `radicado_radicacion`, y exige una sola persistencia y una sola fila visible sin postback.

```powershell
npm.cmd --prefix tools/e2e run test:doc90:radicacion-classic-attachment -- --profile tools/e2e/profiles/<perfil-runtime.json>
```

La matriz funcional debe completarse también con la no regresión de Radicación Simplificada, usando una autorización y un recurso descartable independientes:

```powershell
npm.cmd --prefix tools/e2e run test:radicacion-simple:attachment -- --profile tools/e2e/profiles/<perfil-doc85-runtime.json>
```

Ninguno de estos comandos se ejecuta sin autorización explícita. La evidencia DOC-90 conserva únicamente fecha, huella del recurso, banderas y conteos; no guarda radicado, credenciales, cookies, consultas, cadenas de conexión ni contenido documental.

## Medición de rendimiento del visor PDF en Workflow

La medición reutiliza la sesión autenticada y el recorrido oficial de Workflow. No llama directamente al handler con una ruta construida por la prueba. Copie `workflow-pdf-viewer-performance.profile.example.json` dentro de `tools/e2e/profiles/` con otro nombre terminado en `.runtime.json` y configure URL, módulo, tarea, ordinal del documento PDF y parámetros de medición. No agregue rutas físicas, identificadores documentales, consultas ni secretos.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance -- --profile profiles/<perfil-runtime.json>
```

El runner selecciona la tarea mediante `seleccion_tarea_wf`, abre el documento mediante `vis_doc_selecion_wf` y exige que `ifrm_visor_` navegue al visor oficial `pdf_anotate_view`. Mide carga inicial, recargas controladas, primera página, transferencia saneada, canvases, tareas largas, scroll y zoom. No invoca `service_anotate_pdf_image`, no pulsa el control de persistencia y no modifica el PDF.

La apertura normal registra la visualización mediante la lógica existente de la aplicación. Por ello se requieren confirmaciones separadas para ambiente, cuenta/tarea y registro de visualización. La evidencia omite URL, query string, ruta física, cuenta, tarea, documento, cookies y credenciales. Si el objetivo posterior es medir el guardado de un grafo, debe utilizarse otro escenario, un PDF descartable y una autorización explícita de modificación.

## DOC-103 — Guardado transaccional de grafo

El modo `double` valida localmente la matriz transaccional sin login, red ni escritura documental. Usa el DOM, `Image`, jQuery y ASMX controlados por el arnés Node existente:

```powershell
npm.cmd --prefix tools/e2e run test:doc103:graph-save -- --mode double
```

El modo `real` es mutador. Antes de solicitar cuenta o contraseña exige, por separado, autorización del ambiente, de la cuenta y confirmación de que el PDF seleccionado es descartable. Copie `profiles/doc103-graph-save.profile.example.json` con sufijo `.runtime.json`; no incluya secretos, rutas físicas ni identificadores documentales. `documentStateSql` debe ser una sola sentencia `SELECT`, contener exactamente un parámetro `?` y ejecutarse contra el único DSN `workflowdocument` del perfil con la cuenta ODBC de solo lectura capturada en la consola. La huella de tarea debe permanecer igual: el efecto documental se comprueba mediante un único POST confirmado, refresco parcial y retiro del overlay. No se consulta `logdocuarchi` ni se introduce otro origen.

```powershell
npm.cmd --prefix tools/e2e run test:doc103:graph-save -- --mode real --profile profiles/<perfil-runtime.json>
```

La corrida real crea un overlay con la firma configurada para la cuenta, acepta una sola confirmación, observa exactamente un POST a `service_anotate_pdf_image` y espera el retiro del overlay después del refresco parcial. Verifica que página, zoom y scroll no cambien, y compara huellas `SELECT` antes/después. Si falta una autorización, el documento no es descartable, no existe permiso/firma o fallan los controles de solo lectura, debe detenerse sin autenticar o sin mutar, según la fase. La evidencia ignorada conserva únicamente conteos, estados normalizados y banderas; nunca URL, query, cuenta, tarea, documento, ruta, imagen, cookies, tokens, credenciales ni resultados SQL.

## DOC-105 — Matriz transversal del visor PDF

DOC-105 reutiliza la sesión autenticada, selección Workflow, medición PDF y guardado DOC-103 existentes. No crea login, configuración Playwright ni `.env` paralelos. Copie `doc105-pdf-cross-module.profile.example.json` con sufijo `.runtime.json`; complete únicamente consumidores autorizados, selectores/rutas relativas conocidas y la matriz de red. No incluya secretos ni rutas físicas.

La matriz de lectura exige confirmaciones separadas para ambiente, cuenta/consumidores y registros normales de visualización. Captura la cuenta y contraseña por TTY, crea una sesión autorizada, aplica por CDP perfiles de 10/20/50/100 Mbps y ejecuta carga fría y caliente. Un consumidor deshabilitado o inaccesible queda `BLOCKED`; nunca se convierte en aprobación transversal. Gestión de Correspondencia conserva su ventana nativa de solo lectura (`anula_visor_espress = 1`): allí se valida el iframe hacia `Handler_image_wf.ashx`, la autorización y el transporte, mientras canvas, overlay y scroll/zoom de PDF.js son `NOT_APPLICABLE`.

```powershell
npm.cmd --prefix tools/e2e run test:doc105:pdf-cross-module -- --profile profiles/<perfil-runtime.json>
```

Para conservar exactamente la tarea y PDF de una medición Workflow anterior sin copiar sus identificadores, se admite `--workflow-profile profiles/<perfil-rendimiento-runtime.json>`. Este modo habilita únicamente Workflow y registra las otras cuatro clases como `BLOCKED`; cada una requiere después un perfil DOC-105 con su contexto autorizado. No habilita consumidores por inferencia.

Para diagnóstico acotado puede agregarse `--network remote-10|remote-20|remote-50|remote-100`. La corrida sigue ejecutando frío y caliente y no sustituye la matriz completa.

Para diagnosticar el scroll horizontal, `--consumer production --diagnose-horizontal-layout` reutiliza autenticación y recorrido de Producción, abre una sola vez y mide cuatro niveles mediante el botón Acercar. Rechaza mutaciones y otros consumidores; aborta el servicio de pegado de grafo. Usa viewport 1500x880 y no emula redes ni ejecuta la matriz fría/caliente. Conserva únicamente números/booleanos de zoom, anchos, alturas, desplazamiento y recorte inferior a través de los iframes en `artifacts/doc105-horizontal-layout.json`. `FAILED` señala desbordamiento sin desplazamiento efectivo o borde inferior recortado; sin desbordamiento queda `INCONCLUSIVE`. Un gutter cero puede ser barra superpuesta del navegador y no demuestra por sí mismo ausencia. No sustituye una prueba visual ni la matriz global. Cuenta y contraseña se capturan exclusivamente por TTY.

Migración admite `--migration-selection profiles/<seleccion>.runtime.json`, incompatible con los otros selectores operativos. Campos: `cabinetName`, `searchField`, `searchValue`, `documentPrefix`. Abre el menú oficial y selecciona solo una coincidencia exacta del campo y única del prefijo. La opción de panel de migración consulta original y destino existentes; el runner no pulsa convertir/reemplazar/guardar/eliminar. Exige destino existente en `Iframe_visor_pdf` y mide solo sus respuestas. Sin destino no aprueba. Los datos de selección quedan en perfil ignorado y las credenciales solo en TTY.

Versiones de Radicación entrante admite `--versions-selection profiles/<seleccion>.runtime.json`, incompatible con `--docuarchi-selection`. Campos cerrados: `templateName`, `radicado` y `documentTitle`. Usa la cuenta a la que ya está asignado el radicado. El runner abre la plantilla exacta por el menú, verifica el radicado asignado y no pulsa asignación de pendientes. Abre el historial del documento exacto y visualiza una versión existente; no adjunta, restaura, reemplaza ni elimina. Un contexto diferente detiene la prueba. Este modo habilita únicamente `versions` y deshabilita mutaciones.

En selección de Migración, `view` es opcional: `converted` conserva la prueba de destino existente y `source` reproduce el visor del original. Ambos usan el menú lateral y el campo primario de búsqueda del gabinete. `source` registra `DOC105_MIGRATION_SOURCE_PREVIEW` y nunca sirve como evidencia de aprobación del destino convertido ni de versiones. No se modifica el documento en ninguno de esos modos.

Para la selección Docuarchi use `--docuarchi-selection profiles/<seleccion>.runtime.json`, con `cabinetName`, `searchField` y `searchValue` autorizados. La coincidencia debe ser exacta y única. El campo opcional `simpleClass` acepta solo `docuarchiExternal` (predeterminado) o `docuarchiDownload`. Descarga exige un documento no PDF que presente el botón oficial; espera finalización, cuenta bytes y contrasta HTTP 200/Content-Length. No guarda contenido en evidencia ni abre Word; elimina únicamente el temporal de esa descarga administrado por Playwright. Todas las otras clases y mutaciones permanecen deshabilitadas.

Cuando las cuentas autorizadas difieren por módulo, use `--consumer workflow|correspondence|production|approvalExternal|simpleHandler`. La corrida habilita exclusivamente esa clase y conserva las demás como `BLOCKED`; no combine esta opción con `--enable-inventoried-consumers`. Cada módulo solicita su propia cuenta por TTY y sus resultados se consolidan después sin compartir credenciales.

Si el perfil base procede de Workflow pero el consumidor pertenece a Gestión, declare el valor público del selector de login mediante `--module GESTOR`. Esta opción solo cambia el módulo elegido en el formulario para esa corrida; no persiste cuentas, no altera el perfil runtime y no habilita permisos inexistentes.

El guardado es opcional y permanece deshabilitado por defecto. Para habilitarlo, `mutation.consumer` debe ser `workflow` y `mutation.profile` debe señalar un perfil DOC-103 runtime dentro de `profiles/`. La consola solicita otra autorización para cuenta/tarea y PDF descartable, además de la cuenta ODBC de solo lectura. El runner delega en la protección DOC-103 y exige exactamente un POST; no permite mutación en Correspondencia, Producción, externo ni handler simple.

La evidencia conserva consumidor categórico, perfil de red, caché, estado, HTTP, modo, tiempos, conteos y bytes agregados. No guarda URLs, queries, IDs, títulos, rutas, payloads, cookies, tokens, credenciales o conexiones. `RANGE` solo se considera parcial con `206`, total conocido y unión de bytes previa al primer canvas menor que el documento; `STREAM 200` nunca se presenta como parcial.

## DOC-95 — Orquestación Login 2FA por ASMX

DOC-95 reutiliza el formulario `gestor.aspx`, `tests/support/authenticated-workflow-session.cjs`, la consola TTY y Playwright ya instalados. No crea login, configuración, buzón automatizado, navegador ni `.env` paralelos. El perfil contiene únicamente URL, módulo con 2FA activo, política TLS y timeout; no admite cuentas, contraseñas, OTP, challenge ni conexiones.

La ejecución crea y consume datos reales. Antes de abrir el navegador solicita confirmaciones independientes para ambiente, cuenta, acceso al buzón, creación/consumo del challenge, carrera exacta de dos verificaciones y datos descartables. La cuenta y contraseña se capturan en el TTY; después del postback, el responsable consulta el buzón autorizado e ingresa el OTP de seis dígitos de forma oculta. No pegue secretos en el perfil, variables persistentes, chat o artefactos.

```powershell
npm.cmd --prefix tools/e2e run test:doc95:second-factor -- --profile profiles/<perfil-runtime.json>
```

El escenario llama `ObtenerEstado` y exige estado `PENDIENTE`, destino enmascarado y vigencia positiva. Luego envía exactamente dos llamadas `Verificar` con el mismo OTP: una debe completar y devolver una ruta local; la otra debe fallar cerrada. No usa consultas SQL. Si en el futuro se agregan controles de base de datos, deberán ser exclusivamente `SELECT` y usar una cuenta de solo lectura. La evidencia solo imprime banderas y el conteo de ganadores; nunca OTP, identidad, destino, cookie, challenge, ruta, cuerpo HTTP o excepciones.

Para estabilidad de arranque existe `--scenario stability`. El runner no recicla IIS: el responsable debe autorizar y ejecutar el reciclado por un canal separado. Inmediatamente después, el escenario solicita una autorización adicional y realiza tres sesiones nuevas secuenciales; cada una crea un challenge y correo nuevos, solicita su OTP oculto, completa el login, comprueba la cookie y cierra el contexto antes de la siguiente. No lo ejecute con un solo OTP reutilizado.

```powershell
npm.cmd --prefix tools/e2e run test:doc95:second-factor -- --scenario stability --profile profiles/<perfil-runtime.json>
```

El modo siguiente valida registro y perfil sin TTY, login, navegador, correo ni escritura real; no sustituye la E2E autorizada:

```powershell
npm.cmd --prefix tools/e2e run test:doc95:second-factor -- --check --profile profiles/doc95-login-second-factor.profile.example.json
```

## DOC-111 — Importación por carpetas REC-01/02

La prueba autorizada de degradación usa `--scenario LOTE --degradation-profile <perfil-carga-individual.json>` como selector técnico, sin ejecutar REC ni crear otro lote. Es excluyente con publicación/diagnóstico/recuperación. Solo admite nivel 7 y el recurso de pruebas expediente 467/tipología 50/PDF doc111-carga-individual.pdf de 677 bytes, conforme a su aprobación específica. Reutiliza el puente de esquema DOC-110 con `-Operational`: únicamente ra_integracion_evento ↔ doc111_rec_evento_20261102, snapshot previo, restauración en finally y registro doc111-operational-schema-recovery.json. Nunca iniciar sin autorización concreta de esa alteración y del PDF individual; evitar otras importaciones durante la ventana. Recuperación explícita mediante el mismo puente `-Operational -Recover` si el cierre no acredita restauración. Los controles SELECT siguen usando la cuenta de lectura; DDL usa el puente administrativo autorizado, sin otorgar permisos al usuario SELECT. Marcador separado DOC111_DEGRADATION_RESULT exige indisponibilidad, carga/incorporación, restauración e invariancia de lotes 2–5; no acredita SII real.

Para preparar una publicación PARCIAL separada, el selector admite `--scenario PARCIAL --partial-profile <propuesta-aprobada.json>`. Requiere aprobación específica para crear/publicar el perfil y los recursos REC, además de las autorizaciones habituales. La propuesta incluye el perfil/versión de origen, configuración/reglas copiadas con PARCIAL_PERMITIDO/NO_APLICA y los mismos nivel/nombres del runtime. El helper contrasta la publicación de origen antes de crear; usa el token administrativo DOC-110 existente para CrearPerfil/GuardarBorrador/PublicarVersion/CambiarDisponibilidad y pasa al REC el ID de versión devuelto por el servidor. No cambia el perfil de origen, no hace SQL directo y no reintenta mutaciones. `DOC111_PROFILE_REFERENCE` conserva exclusivamente IDs del nuevo perfil/versión para revisar un fallo intermedio; no acredita publicación o REC por sí mismo. Un fallo detiene la corrida: no repetir el selector para recuperar.

Para un lote ya COMPLETADO, `--scenario LOTE --verify-lot <lote>` solo inicia sesión, consulta, verifica por SELECT y reabre. Efectos y recursos descartables permanecen sin autorización en el proceso hijo; el driver bloquea cualquier fase, reconciliación o recepción. Marcador `DOC111_COMPLETED_VERIFICATION_RESULT`, sin acreditar replay de comandos mutantes ni REC completo. Las columnas de distintas tablas deben usar alias diferentes incluso ignorando mayúsculas: el diccionario de filas PowerShell no distingue `FORMATO` de `formato`; usar `inventory_format` e `index_format`.

Para un documento ya almacenado con IDs persistidos y resultado INCIERTO, `--scenario LOTE --resume-lot <lote> --reconcile-attempt <intento>` verifica/reconcilia ese intento mediante el coordinador existente y conserva documento/inventario/expediente. Es excluyente con `--close-attempt`; nunca cierra una reserva iniciada ni reenvía el primer almacenamiento. Solo continúa el segundo archivo NO_INICIADO después de comprobar la reconciliación y su repetición sin cambios. Resultado saneado `DOC111_RECONCILE_RESUME_RESULT`, distinto de REC completo. Los controles exigen nombre original en inventario/índice y respetan la visibilidad de NOMBRE del gabinete: vacío/NULL solo si no se materializa; un nombre distinto, metadatos ambiguos o visibilidad inválida bloquean.

Recuperación explícita: `run-doc111-importacion-carpetas.cjs --profile <perfil> --scenario LOTE --resume-lot <lote> --close-attempt <intento>` reutiliza consola/sesión/SELECT existentes. Solo admite la reserva ALMACENAR sin inicio, dos expedientes ya confirmados y ambos PDFs recibidos; verifica ausencia de documentos antes de solicitar el cierre transaccional y continúa esos mismos archivos con claves nuevas. No prepara/confirma otro lote, no recibe otra vez ni crea expedientes. Un rechazo o respuesta perdida detiene el recorrido sin retry. La evidencia se emite como `DOC111_RESUME_RESULT`, nunca REC PASS; no vuelve a acreditar la barrera inicial ni las fases de otra corrida. Credenciales solo por consola enmascarada y controles con cuenta exclusivamente SELECT.

El selector y suite DOC-111 reutilizan este paquete, configuración Playwright, consola, helper de sesión y puente ODBC de controles existente. Preparar el código no autoriza ejecutar efectos. Cada corrida real requiere ambiente/cuenta/nivel/perfil/nombres descartables aprobados y controles RA/DA con cuentas exclusivamente SELECT. El perfil declara `odbcDsn` (RA) y `docuarchiOdbcDsn` (DA), además de selección/expectativas, sin conexiones ni secretos. La consola captura la cuenta/contraseña de aplicación y una cuenta/contraseña MySQL de solo lectura con acceso a ambos DSN, enmascaradas, sin `.env` o archivos de secretos. Nunca solicita URLs MySQL ni cadenas de conexión. Si no existe esa cuenta de lectura, detener antes del login; no sustituirla automáticamente por las credenciales del sitio.

Los controles DOC-111 necesitan resultados en memoria y varios parámetros posicionales. Usan el modo `Rows` del puente `query-doc32-odbc-fingerprint.ps1` mediante `queryRows` del helper compartido. Este modo admite únicamente SELECT, verifica el número de parámetros y rechaza múltiples sentencias, escrituras, bloqueos y funciones de espera. Las filas se transmiten por IPC al proceso de pruebas; no se imprimen ni guardan como evidencia. El modo predeterminado de huellas y sus consumidores previos mantienen el contrato de un parámetro.

Diagnóstico de acceso sin carga: agregar `--diagnose` al selector con el perfil y escenario correspondientes. Confirma ambiente/cuenta/TLS y captura únicamente cuenta/contraseña de aplicación; no pide secretos MySQL, no conecta controles ODBC y retorna antes de `runRec`. Reutiliza el mismo helper de sesión. Si falta el control de importación, usa la consulta de lectura `AbrirImportacion` para clasificar sesión ausente, permiso denegado o capacidad no disponible, sin inventar token ni saltar el guard. Puede producir los registros normales de apertura/login existentes. `DOC111_DIAGNOSTIC_RESULT` no acredita REC ni concede efectos; sus autorizaciones de efectos/descartables/SELECT se fuerzan a false. No combinar `--diagnose` con `--check`.

El consumidor DOC-111 usa los observadores del helper compartido para esperar el postback y clasificar únicamente mensajes públicos reconocidos del login. Conserva solo códigos cerrados de credenciales rechazadas, cuenta ausente/bloqueada, entrada requerida, segundo factor, servicio o postback no disponible. Un mensaje desconocido del diálogo se convierte en rechazo genérico; no se guarda texto del diálogo, cuerpo HTTP, credenciales o cookies. No se afirma contraseña incorrecta por un 401 posterior sin ese mensaje explícito. No hay reintentos de autenticación ni bypass de MFA y los redirects no se leen como cuerpos normales.

Si el observador no puede terminar/leer el postback, esa ausencia de diagnóstico no decide la autenticación: la consulta protegida/control/token siguen determinando si la sesión existe. Rechazos explícitos, HTTP de error y segundo factor sí detienen el flujo. La observación conserva exclusivamente estados HTTP, etapa FINISH/BODY, categorías cerradas (target cerrado, navegación abortada, cuerpo no retenido, timeout o sin clasificar) y MATCHED/MISSING/DIFFERENT para los campos enviados, comparados solo en memoria con la entrada efímera. Nunca contiene formularios, nombres, contraseñas, viewstate, cuerpos o texto de excepción. Un 401 sigue rechazado aunque la lectura del postback haya fallado.

Validación local sin login, navegador, conexión ni escrituras servidor:

```powershell
npm.cmd --prefix tools/e2e run test:doc111:rec -- --check --profile profiles/doc111-importacion-carpetas.profile.example.json --scenario EXPEDIENTE
```

El ejemplo tiene URL e IDs ilustrativos. Para una corrida autorizada, preparar un perfil runtime propio a partir del ejemplo con un nivel y versión publicada existentes, dos nombres/códigos descartables y expectativas de tipología/índices reales. Cada raíz debe tener al menos un índice de gabinete esperado. No poner credenciales o conexiones en ese JSON. `--check` valida forma y PDF, sin acreditar recursos/permisos de un ambiente.

Comando de la corrida real, exclusivamente después de la autorización concreta:

```powershell
npm.cmd --prefix tools/e2e run test:doc111:rec -- --profile profiles/<perfil>.runtime.json --scenario EXPEDIENTE
```

Los otros selectores son `LOTE` y `PARCIAL`; el modo debe coincidir con el perfil publicado y el archivo runtime. Uno por corrida, sin retries. La consola exige autorizar confirmación/recepción y creación/almacenamiento/incorporación de dos documentos en dos expedientes; en parcial conserva un tercer original rechazado. Contrasta la barrera y los resultados con SELECT, repite comandos ya confirmados y reabre el lote sin alterar IDs/relaciones/eventos. No modifica perfil, esquema, permisos, configuración o SII.

Ante un error, no volver a ejecutar el selector para «recuperar»: crea una preparación nueva. Conservar `DOC111_REC_REFERENCE` (solo IDs de plan/lote) y consultar primero la operación existente. La suite no reenvía una mutación incierta ni fuerza éxito. La salida saneada solo conserva referencias numéricas y resumen cerrado; no se guardan capturas, trazas, video, payloads ni filas de negocio. Recursos/staging se conservan para revisión; no hay borrado automático.

Implementación local y límites: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-rec-drivers-local-20261030.md`. Su validación con dobles/listado no acredita aceptación real ni autoriza otra corrida.

## DOC-113 — UI focal de importación por carpetas

Reutiliza el perfil base cerrado DOC-111 y la misma consola/sesión/DSN; no hereda autorizaciones previas. Revisar primero sin autenticar:

```powershell
npm.cmd --prefix tools/e2e run test:doc113:ui -- --profile <perfil-base.json> --scenario UI-01 --alternate-level <nivel-B> --alternate-version <publicacion-B> --check
```

Sin --check, se requiere consola interactiva y confirmación SI completa de ambiente/cuenta/controles SELECT. Usuario de aplicación visible solo en consola; contraseñas enmascaradas. UI-02 autoriza dos preparaciones con los nombres del perfil y conserva recursos; --confirm exige aprobación adicional y solo confirma el segundo plan, sin recepción o avance documental. UI-03 exige nivel B y autorización del rechazo real de una apertura con nivel inválido 0. No se pide URL MySQL ni se crean archivos de secretos. UI-01 exige nivel B/publicación B distintos de A. Los IDs alternativos son parámetros no secretos validados.

La suite compara SHA256 del JavaScript servido con el candidato antes de actuar. Una versión distinta detiene el caso. El selector suprime la salida no saneada del proceso y conserva referencias de planes/lote antes de fallar; no repetir tras un resultado desconocido sin consultar estado. Solo se captura el diálogo en Producción, sin login, cookies o trazas. No se reintroducen flags o listas de activación. E2E sin autorización queda NO_EJECUTADO, aunque pruebas locales y discovery pasen.

## DOC-114 — Preparación focal PLAN-01/02/03

Comando del selector fino, primero en modo local sin autenticación:

```powershell
npm.cmd --prefix tools/e2e run test:doc114:plan -- --session profiles/<recursos-revisados>.runtime.json --scenario PLAN-01 --check
```

El archivo es un conjunto cerrado de datos de prueba, no configuración de plataforma ni secretos: version=1, scenario, identity {userRef,ambitoRef}, profiles (uno a ocho perfiles base completos validados con validateProfile DOC-111 sin campos adicionales), cases y referencias negativas foreignLevel/invalidVersion cuando se usan. Cada case contiene key, índice profile, request con el DTO PrepararPlan real y expected {confirmable,eligible,incidences,values} o rejectCodes, nunca ambos. Incidences usan code/root/file y values usan root/file/type/ref/value/deferred. La lista de casos requerida está en fixtures/doc114-plan-cases.json; PLAN-03 requiere además baseline antes de revoked-permission, con la misma solicitud válida, y runtimeRoot absoluto de la aplicación servida. El conjunto compacto se limita a 28 KB para el transporte por entorno del proceso compartido, sin ampliar cuotas del producto. Los fixtures describen candidatos sintéticos: contrastar nombres, tipos, reglas y publicaciones reales antes de autorizar una preparación. No generan perfiles ni conceden permisos.

Sin --check, la consola compartida exige ambiente/cuenta/SELECT, inventarios/publicaciones descartables y el número exacto de intentos enumerados. Conserva planes; nunca confirma, recibe o avanza. Usuario visible solo en consola, contraseñas enmascaradas; RA/DA usan DSN registrados, sin URLs MySQL o .env nuevos. El reporte de resultado distingue intentos, planes persistidos y rechazos. Se conserva cada planId antes de otras aserciones. No repetir después de incertidumbre sin consultar referencias por lectura. Los casos no autorizados quedan NO_EJECUTADO.

PLAN-03 incorpora revocación/restauración de IMPORTAR_CARPETAS, exclusivamente para la cuenta de pruebas ya acotada por el puente (LUZ.AGUILERA) y el ambiente localhost existente. Requiere autorización separada DOC114_AUTH_REVOCATION y mantiene los demás campos, incluido ADMINISTRAR_PERFILES_IMPORTACION. El puente DOC-110 conserva su modo administrativo por defecto; la opción nueva -OperationalImport usa runtimeRoot, un plan verificado (recién preparado o conservado y revalidado por lectura) y su hash como prueba de que el origen administrativo coincide con la base operativa, además de actor/empresa. No copiar conexiones ni modificar Web.config. Recuperación propia: tools/e2e/artifacts/doc114-operational-permission-recovery.json dentro de runtimeRoot. Evitar corridas concurrentes o cambios de permiso sobre esa cuenta durante la ventana autorizada.

La revocación se restaura en finally y al cerrar el puente. El resultado no pasa si falta restauración o cambian otros campos. Ante recuperación pendiente, revisar el diario y usar el mismo puente con -OperationalImport -Recover -RuntimeRoot <ruta>, con autorización específica y cuenta operativa validada en el entorno; no repetir la suite. Nunca ejecutar el modo administrativo como sustituto de la revocación operativa. Los controles RA/DA siguen exclusivamente SELECT; la mutación de permiso utiliza únicamente el mecanismo administrativo existente expresamente autorizado, sin otorgar permisos a la cuenta de lectura.

Las pruebas locales del puente cargan funciones y cláusula REVOKE mediante AST con proveedor en memoria, sin ejecutar conexión/SQL/configuración real. No acreditan revocación productiva. La suite real desactiva capturas/trazas/video y emite solo referencias y resúmenes cerrados. Las huellas cubren el código/pruebas explícitos del candidato: no afirman despliegue de una DLL por conocer HEAD. Registrar procedencia del backend servido antes de la aceptación real.

En todos los escenarios el archivo de recursos incluye runtimeRoot absoluto: se lee el hash de la DLL de esa raíz antes/después y se compara con el capturado por el selector. No se despliega ni se atribuye a HEAD esa DLL. Cada corrida tiene un identificador aleatorio y escribe de inmediato sus planId en artifacts/doc114-<identificador>.json; no se guardan solicitudes, credenciales o filas privadas. profiles/doc114-plan-session.profile.example.json muestra PLAN-01 con referencias ilustrativas, y solo sirve a --check hasta revisar y autorizar recursos reales. El archivo de recursos compacto, sus claves y resultados esperados forman parte del alcance que aprueba el operador.

### Continuación de un plan ya verificado DOC-114

Tras una parada no repetir PrepararPlan. Auditar por SELECT referencias, hash, contexto/manifiesto/resultados, versión y presupuesto consumido. El invocador de consola puede llamar al selector compartido `main(args, {resumePlan: {caseKey, planId, planHash}})` para el primer caso únicamente: valid-format-a de PLAN-01 o baseline de PLAN-03. El selector valida ese descriptor, lo transmite efímeramente y resta un intento nuevo; la suite relee el plan por SQL y conserva resumedPlans=1 separado de attempts/preparations. No omite los otros casos ni autoriza un replay por tener un ID. Nuevos intentos que excedan el presupuesto necesitan alcance específico.

Los fallos de rechazo conservan solo operación/HTTP/código conocido/forma del sobre; nunca cuerpos o mensajes privados. Cada plan verificado registra ID/huella. La comparación persistencia/ASMX normaliza únicamente Proveedor y Procedencia nulos conforme al modelo. La comparación de snapshot con publicación ordena reglas por DestinoTipo/DestinoRef, manteniendo Orden/IDs/huella/contenido; el array de la respuesta persistida mantiene su comprobación propia. Los campos inyectados son rechazados por el validador actual con CONFIGURACION_INVALIDA: comprobar código exacto, no aceptar cualquier 4xx. Las fases de permiso se registran antes/después de REVOKE/RESTORE; el resultado requiere restauración/reapertura y cierre del puente comprobados.

## DOC-117 — Regresión local sin ambiente real

`npm.cmd --prefix tools/e2e run test:doc117:local` ejecuta únicamente el inventario versionado de pruebas locales del almacenamiento y sus dependencias compartidas. Usa historia sintética para revisar loaders/concesiones y compila las guardas en directorios temporales; no autentica, consulta una base real, instala una DLL ni crea documentos. Los artefactos de aceptación histórica siguen separados. Una corrida real exige revisar su propuesta concreta y obtener autorización específica del ambiente, cuentas, recursos, efectos y presupuesto; este comando local no la concede.
