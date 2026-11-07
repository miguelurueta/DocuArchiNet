# DOC-117 — Estado vigente de consolidación local, 2026-11-06

| Caso vigente | Evidencia propia | Alcance |
| --- | --- | --- |
| ALM-01 PASS real | doc117-alm01-pass-20261105.md/.json | Valores finales, originales e incorporación; no repetir |
| ALM-02 PASS real de laboratorio | doc117-alm02-interno-pass-20261106.md/.json | Ventanas A/B; intento53 EN_CURSO/público INCIERTO preservado |
| ALM-03C PASS real de laboratorio | doc117-alm03c-coexistencia-pass-20261106.md/.json | Tres documentos, solapamiento99,7347ms, originales/preservación e incorporación física; original restaurado/reabierto |


## Resultado vigente — ALM-03C verificado

PASS real con variante de laboratorio: A16225/23323/intento63, individual16226/23324 y B16227/23325/intento64. Solapamiento interno99,7347ms, tres incorporaciones físicas y preservación posterior verificadas. Lote23 COMPLETADO, raíces37/38 COMPLETADA; consolidación interna propia de ALMACENAR, sin operación extra. DLL original restaurada/reabierta, dos respaldos nuevos y cuatro históricos idénticos,912 entradas originales y staging de lotes22/23 intactos.

Contador339+104+50+88=**581/659**; los cierres solo SELECT mantienen el mismo presupuesto. Fallo inicial de reapertura INPUT_REQUIRED y error posterior del control EN_CURSO conservados por SHA y resueltos mediante comprobaciones independientes, sin repetir documentos. La lectura incompleta de PersistirResultadoDocumento omitía ConsolidarEnTransaccion en su llamador; las notas anteriores de ausencia de consolidación quedan corregidas. Regresión final365/365 y OpenSpec estricto PASS. Informe/certificado doc117-alm03c-coexistencia-pass-20261106.md/.json.

Trazabilidad D-06/RQ-06/17.1 y D-08/RQ-08/18.1 completadas al consolidar esta evidencia. No se acredita concurrencia XML/SII, commits simultáneos o rendimiento ordinario. Casos antiguos e incertidumbres41/53 preservados, sin replay/reparación/reconciliación/limpieza. El checklist local termina18/18; publicación, revisión, sincronización/archivo OpenSpec y transición Jira son operaciones posteriores separadas.


## Cronología anterior

Los apartados siguientes conservan los estados anteriores, sin sustituir la evidencia vigente anterior.


16/18 tareas completas tras ALM-02 interno real autorizado. Cambio activo; no listo para cierre o archivo. Los apartados posteriores conservan la cronología de los avances previos; el último resultado real prevalece para el estado vigente.

| Trazabilidad | Evidencia | Estado |
| --- | --- | --- |
| D-01 / RQ-01 / 1.1–2.1 | `source-review.json`, `evidence-review.md` del cambio y matriz DOC-117 | Procedencia histórica y local separada; no se traslada un PASS a otro binario. |
| D-02 / RQ-02 / 3.1–4.1 | `doc117-local-20261105.md/.json`; regresión .NET repetida el 2026-11-06 | PASS local de original/copia y replay con escritor doble. |
| D-03 / RQ-03 / 5.1–6.1,15.1 | `doc117-alm01-pass-20261105.md/.json` | PASS real ALM-01, planes 35/36, lotes 19/20; no repetir. |
| D-04 / RQ-04 / 7.1–9.1,16.1 | Regresión .NET y `doc117-alm02-interno-pass-20261106.md/.json` | PASS real de A/B en variante de laboratorio: A/intento 52 confirmado y replay sin otro escritor; B/intento 53 EN_CURSO/público INCIERTO sin repetición. Runtime original restaurado y reabierto, recursos previos preservados. |
| D-05 / RQ-05 / 10.1 | `ReconciliationCases`, regresión .NET | PASS local; EN_CURSO y prueba insuficiente no habilitan reparación ni nuevo escritor. |
| D-06 / RQ-06 / 11.1,17.1 | `doc117-alm03-propuesta.md`, `doc117-alm03-setup-pass-20261106.md/.json` | Preparación real PASS, lote 22/expedientes 666–667; coexistencia NO_EJECUTADO. Faltan guardas activas, candidata/ejecutor revisados y autorización de instalación/almacenamiento. |
| D-07 / RQ-07 / 12.1–13.1 | `DocumentAdapterCases` / `DocumentCatalogCases`, regresión .NET | PASS local de rechazos; formatos sin ampliación. |
| D-08 / RQ-08 / 14.1,18.1 | Preflight, ALM-02 y matriz DOC-117, OpenSpec estricto | Verificación estructural PASS; consolidación final pendiente de ALM-03 o aceptación explícita de alcance reducido. |

Los documentos de validación están bajo `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/`. La matriz se actualizó para quitar afirmaciones vigentes contradictorias sobre ALM-01, conservando los reportes históricos. No se modificó la evidencia anterior ni se repitió su carga.

Límites: no se ejecutaron pruebas autenticadas, SELECT reales, cargas, instalación de DLL, alteración de permisos/configuración ni limpieza durante este avance. No se confirma revisión OpenSpec/OPSXJ para un SHA nuevo, no se archiva ni se cierra Jira. La candidata local desactivada no está lista para instalación.

## Avance posterior: preflight de lectura autorizado

Tras el avance local, el usuario autorizó expresamente las ocho consultas del preflight ALM-02. Resultado real PASS, 8/8 SELECT; ver `doc117-alm02-preflight-pass-20261106.md/.json` en Validacion. La declaración anterior de ausencia de SELECT reales corresponde únicamente al avance local previo. No hubo login de aplicación, efectos ni cambios de DLL. La lectura confirma publicación, receta de código/nombre, ausencia de los nombres propuestos e índices; no acredita permisos, tipología, ventanas internas ni restauración. El avance continúa en 15/18 y no se archiva el cambio.

## Preparación posterior: restauración y alcance del intento

Ensayo local aislado PASS con original/candidata copiados a `.tmp`, guardián independiente y dueño terminado sin restauración propia; ambos respaldos y archivo restaurado idénticos al original. Evidencia `doc117-restauracion-local-20261106.md/.json` en Validacion. No hubo nuevas consultas, autenticación o activación real. Las 16 pruebas iniciales DOC-116/DOC-117 pasaron tras extraer la mecánica de archivos compartida conservando gates independientes.

La propuesta editorial `doc117-alm02-ajuste-alcance-propuesto-20261106.md` resuelve la imposibilidad de compilar un ID que el coordinador asigna dentro de `AvanzarLote`; mantiene la fijación exacta antes del escritor. Está sin aplicar, a la espera de decisión editorial. El gate LIVE sigue exigiendo el alcance cerrado y su autorización propia. La ejecución interna real de 16.1 permanece pendiente.

## Aplicación posterior de la aprobación editorial

El usuario aprobó el ajuste mediante «aprobado» el 2026-11-06. D-04 y el soporte local ya aplican el protocolo de fijación previa al escritor; la declaración anterior de espera editorial es histórica. Evidencia `doc117-fijacion-intento-local-20261106.md/.json`: comando real inmutable, igualdad de huella con el coordinador, rechazo de registros ajenos/alterados y carrera, candidata desactivada compilada y restauración aislada con guardián independiente. El gate LIVE exige protocolo, recursos y huellas, y autorizaciones propias. No hubo nuevos SELECT, autenticación, cargas o instalación. Progreso 15/18; pendiente la preparación autorizada de recursos y la candidata real de 16.1.

Regresión conjunta final 48/48 PASS; OpenSpec estricto PASS. La preparación real está implementada/probada pero no ejecutada. `doc117-alm02-setup-listo-20261106.md` define el siguiente alcance revisable y `doc117-alm02-setup-local-20261106.json` liga propuesta, soporte, calculador y presupuesto. La aprobación editorial y el preflight anterior no conceden los efectos nuevos de esa preparación. Se conservan las tres tareas abiertas.

## Preparación real autorizada después

El usuario autorizó específicamente la preparación, completada con PASS: plan 37/lote 21, expedientes 664/665, dos recepciones/materializaciones, 100/160 SELECT acumulados y cero almacenamiento/cambios de DLL. La corrección local de comparación de reglas permitió continuar el mismo plan sin crear otro. Evidencia `doc117-alm02-setup-pass-20261106.md/.json`. Regresión actual 51/51 PASS, OpenSpec estricto PASS. Los estados de espera anteriores son históricos.

La candidata de esas identidades se compiló fuera de bin y sus bytes se restauraron en un sitio temporal mediante guardián independiente. No está instalada y las ventanas internas reales siguen pendientes; no se traslada ese ensayo al runtime del sitio. La preparación no concede instalación/almacenamiento/inyección. 15/18 tareas completas, sin archivo ni cierre.

## Ejecutor interno listo para autorización posterior

Soporte local cerrado para las dos ventanas del lote 21: fijación de intento, marca durable de entrada al escritor, verificación de A antes del replay y de B, prueba física por IDs sin promover EN_CURSO, y restauración final incluso si falla el cierre de sesión. Nueva candidata `85824b25…` compilada fuera de bin, 912 entradas originales inalteradas y ensayo de restauración de esos bytes PASS en sitio temporal con guardián independiente. Regresión ampliada 134/134 PASS y OpenSpec estricto PASS.

Evidencia en Validacion: `doc117-alm02-interno-listo-20261106.md`, `doc117-alm02-interno-local-20261106.json` y propuesta cerrada `doc117-alm02-interno-propuesto-20261106.json`. La candidata nueva sustituye a la histórica para la próxima propuesta; no se modifican sus reportes anteriores. Falta autorización específica para instalar temporalmente, inyectar fallos y ejecutar hasta tres ALMACENAR/dos documentos/240 SELECT. No se ha creado esa concesión ni se han ejecutado autenticaciones, consultas reales, instalación o almacenamiento durante este avance. B quedaría intencionalmente EN_CURSO/publicación INCIERTO, sin replay/reconciliación/reparación. Progreso 15/18, tareas 16.1–18.1 abiertas.

## ALM-02 interno real autorizado después — PASS

Autorización específica del usuario y corrida `8acc6c159c162668302fb82905560118`, finalizada `2026-11-06T12:10:09.600Z`. Tres solicitudes ALMACENAR, dos entradas al escritor, dos documentos nuevos y 191/240 SELECT. A/intento 52 confirmó documento 16222/inventario 23320 y su replay permaneció invariable; B/intento 53 conservó EN_CURSO con resultado público INCIERTO y documento 16223/inventario 23321 físicos, sin repetición/reconciliación/reparación. Lote 21 aún EN_CURSO.

Originales íntegros, valores/relaciones/incorporaciones verificados y recursos anteriores sin cambios. Candidata temporal retirada; DLL original y ambos respaldos con SHA `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`, sesión reabierta con original y controles finales PASS. Evidencia `doc117-alm02-interno-pass-20261106.md/.json`. Se completa 16.1: 16/18. Los apartados de espera anteriores son históricos; faltan ALM-03 y consolidación final, sin archivo ni cierre Jira.

## Revisión local posterior de observabilidad ALM-03

Analizador de evidencia exterior con 23/23 PASS; mantiene `writerBodyOverlapVerified=false` aunque se solapen las llamadas. Revisión de las dos rutas directas y ensayo únicamente sintético de VSDiagnostics: captura obtenida/cerrada, sin extracción por método/hilo/recurso acreditada. Sin IIS, autenticación, SELECT real, efectos o cambios de producto en este avance. Se conservaron DLL original y las 912 entradas originales de compilación/recursos.

Propuesta editorial concreta `doc117-alm03-observacion-propuesta-20261106.md` y evidencia `doc117-alm03-observabilidad-local-20261106.json` en Validacion. La excepción de observación dentro del escritor, limitada a copia de laboratorio, sigue sin aprobar/aplicar. Se mantiene el requisito de solapamiento interno; no se realiza una carga que no pueda acreditarlo. Tareas 17.1 y 18.1 abiertas, progreso 16/18.

## Excepción aprobada después y prototipo interno local

El usuario aprobó mediante «aprobado» el desarrollo local de la observación interna. Se actualizó D-06/D-08 y el escenario RQ-06. Candidata desactivada compilada fuera de bin y núcleo/envoltura .NET probados con escritor sintético: retornos/ByRef/errores conservados, marcas alteradas o incompletas sin aceptación, sesiones comparadas solo en memoria, sin espera entre escritores en el observador. Regresión conjunta 184/184 PASS. Guardián independiente restauró los bytes de esa candidata en un sitio temporal tras salida del propietario; respaldos/ACL verificados. Las 912 entradas productivas y la DLL instalada siguen intactas.

Evidencia `doc117-alm03-metodo-local-20261106.md/.json`. La entrada de la candidata rechaza todo almacenamiento y no existe binding activo de llamadores o recursos reales cerrados. Faltan preflight/preparación autorizados, guardas de comando/contexto/argumentos y ejecutor/candidata específica antes de solicitar ejecución real. Sin login, SELECT real, recursos de negocio o instalación durante este avance. ALM-03 de aplicación NO_EJECUTADO y 17.1/18.1 abiertas, 16/18; sin cierre/archivo.

## Lectura autorizada y preparación revisable posterior

La continuación autorizada se acotó a doce SELECT reales, sin login de aplicación, cargas o cambios de DLL. El preflight detuvo su octavo control al exigir al expediente individual el nivel 7; los cuatro controles diagnósticos siguientes identificaron nivel 8/relación 1072 para expediente 467 y tipología 50 compatible. No se modificó esa relación y se observaron 41/52/53 en sus estados esperados. `doc117-alm03-destino-lectura-20261106.md/.json` no convierte ese diagnóstico en PASS del preflight o ALM-03.

Se implementó y probó el soporte ALM-03 de preparación separado, preservando el código/grants históricos de ALM-02. Verifica los dos niveles/destinos antes de crear un plan/lote y dos recepciones/expedientes/materializaciones, con cero documentos o cambios de runtime y presupuesto nuevo de 160 SELECT. Regresión conjunta 221/221 PASS y OpenSpec estricto PASS. `doc117-alm03-setup-listo-20261106.md` y `doc117-alm03-setup-local-20261106.json` presentan el alcance concreto pendiente de autorización; no se ha ejecutado ni concedido esa preparación. Falta luego cerrar guardas/candidata/ejecutor de coexistencia. 16/18, sin archivo/cierre.

## Preparación ALM-03 real autorizada después — PASS

El usuario autorizó específicamente mediante «autoriza» la preparación propuesta. Corrida `57219fc625b867b54d8d282b6b1eb0a2`: un plan 38/lote 22, raíces 35/36, archivos 36/37 y expedientes 666/667 (relaciones 1091/1092). Dos recepciones/materializaciones y 80/160 SELECT adicionales. Cero documentos, ALMACENAR, carga individual o cambio de DLL. Se verificó sin modificar el destino 467/nivel 8/relación 1072; originales, valores y recursos anteriores conservados. Se mantuvieron las 912 entradas originales y la DLL instalada.

Evidencia `doc117-alm03-setup-pass-20261106.md/.json`. Alcance real y solicitudes futuras sellados privadamente, sin enviarlas ni predecir intentos. Faltan guardas activas, candidata/ejecutor revisados y permiso específico para coexistencia; no se declara solapamiento. La preparación no se repite. Progreso 16/18, 17.1/18.1 abiertas; los estados de espera anteriores son históricos.

## Coexistencia ALM-03 completa localmente y pendiente de autorización real

Se completaron guardas de los dos llamadores y ejecutor para lote 22 (archivos 36/37, expedientes 666/667, relaciones 1091/1092, nivel 7) e individual 467/1072/nivel 8/tipología 50. La identidad de sesión se compara exclusivamente en memoria. El executor prepara la UI individual oficial, despacha A/individual una sola vez, espera ambos resultados y conserva sus IDs antes de validaciones posteriores. B solo se despacha tras correlacionar marcas/respuestas e integridad de ambos. Sin retry ante incertidumbre o serialización; controles finales tras restauración/reapertura con DLL original.

Candidata cerrada `142e0d0bba0b6bcfd66933158ac163623784a2d0556c1a0f244ec92396b32e12` compilada sin errores fuera de bin. Ensayo de sus bytes en sitio temporal PASS: guardián independiente restauró tras salida del propietario, ambas copias idénticas y ACL verificadas. Regresión final 238/238 PASS, validación de propuesta/código/candidata/recursos y 912 entradas mediante `--check` PASS, OpenSpec estricto PASS. Todas las entradas originales de compilación/recursos y la DLL real conservadas. Sin autenticación, SELECT real, efecto documental o instalación durante este avance.

Revisión concreta en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03-coexistencia-lista-20261106.md`, propuesta `doc117-alm03-coexistencia-propuesto-20261106.json` y certificado `doc117-alm03-coexistencia-local-20261106.json`. No se ha concedido ni creado autorización de coexistencia. La solicitud pendiente especifica ambiente/cuentas, tres cargas/documentos, 320 SELECT nuevos y candidata temporal durante máximo 600 segundos con doble respaldo/guardián. Cero planes/lotes/recepciones/expedientes nuevos. El permiso de setup no se amplía. ALM-03 real NO_EJECUTADO, 17.1/18.1 abiertas, progreso 16/18; la aceptación exige evidencia real de solapamiento e integridad/restauración, sin inferirla de los dobles. Conservar también 41/53 y todos los recursos previos.

## ALM-03 autorizado y ejecutado parcialmente — NO_ACREDITADO

El usuario concedió mediante «autorizado» el alcance concreto de coexistencia. Primer preflight 350ac90a29f77f9cc46b6b070f610680 detenido a 32 SELECT, cero cargas/runtime, por una exigencia incorrecta del control local: mapeo predeterminado no obligatorio en la ruta individual. Tres SELECT diagnósticos identificaron ausencia de mapeos predeterminados y de fecha, 21 campos visibles y seis estándar. Corrección local según la ruta oficial, regresión 246/246 y pruebas enfocadas 17/17, sin sumar; `doc117-alm03-control-opcional-20261106.md` y certificado local.

Continuación derivada 149655bd16ab32f957ddae7d44192109 bajo el mismo permiso, recursos/candidata y presupuesto acumulado inicial de 35. A/individual se solicitaron una vez; la pareja falló con `DOC117_ALM03_PAIR_RESULT_UNCERTAIN`. Solo A tiene entrada/salida interna SUCCESS y IDs observados 16224/23322/intento 58/expediente 666/relación 1091. No hay entrada/salida individual, resumen completo ni despacho B. La causa detallada individual no quedó retenida; no se atribuye una causa por inferencia ni se reenvía la carga. La falta de pareja no acredita serialización.

DLL original restaurada en finally, ambos respaldos byte a byte idénticos y 912 entradas originales intactas. No hubo reapertura autenticada con original tras la interrupción del recorrido normal. Acumulado 93/320 SELECT, incluidos los 35 anteriores. La comprobación posterior solo SELECT está preparada y espera dos credenciales MySQL en consola; no se afirma aún checkpoint durable de A, ausencia documental individual, estado durable final de B o preservación posterior de datos.

Evidencia `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03-detenido-20261106.md/.json`, fuentes de corrida/diagnóstico/lectura vinculadas con SHA. **16/18, 17.1/18.1 abiertas; ALM-03 NO_ACREDITADO**, sin cierre/archivo, XML/SII, repetición, reconciliación o reparación. Conservar documento, temporal individual, originales, marcadores y respaldos. Las entradas anteriores de autorización pendiente son históricas y no describen el permiso actual.

## Comprobación final solo SELECT terminada — estado vigente

El usuario informó salida 0 y se verificó el artefacto persistido `tools/e2e/artifacts/doc117-alm03-post-stop-149655bd16ab32f957ddae7d44192109.json`: `READ_VERIFICATION_COMPLETE`, 53 consultas propias y **146/320 SELECT acumulados**. Sin login de aplicación, almacenamiento adicional o instalación durante esta fase. A/intento 58 está CONFIRMADO; documento 16224/inventario 23322 con incorporación única, índice EEE e identidades 666/1091 verificados físicamente. Ambos originales de staging intactos. No hay documento individual del caso; conteos 9/11/11, clasificación, documentos previos y configuración/valores esperados de seis campos estándar sin cambios. B/archivo 37 NO_INICIADO, sin intento ALMACENAR. Inventario global 8813 → 8814 (+1).

Las huellas de planes y lotes anteriores y sus recursos coinciden con el preflight; se comprobaron expresamente 41 EN_CURSO, 52 CONFIRMADO y 53 EN_CURSO. DLL original, ambos respaldos idénticos, 912 entradas originales y temporal individual conservados. La publicación independiente revalidó SHA del artefacto fuente, contador acumulado, etapas de lectura, referencias, preservación y marcadores, además de hashes/bytes locales. Informe vigente `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03-detenido-verificado-20261106.md/.json`; el informe anterior conserva la etapa histórica de espera.

**ALM-03 NO_ACREDITADO por pareja incompleta; progreso 16/18, 17.1/18.1 abiertas.** No hay marcas individuales o resumen completo y la causa detallada individual no quedó retenida; la lectura no recupera esa observación ni permite afirmar serialización. No se repiten cargas ni se reparan/reconcilian efectos. La siguiente prueba necesita diagnóstico previo y otro caso; este lote y todos sus recursos se conservan. Sin cierre o archivo, ni aceptación XML/SII.

## Pérdida del código individual reproducida y corregida localmente

La continuación solicitada trabajó exclusivamente en soporte JavaScript de pruebas. El helper UI conserva su error específico en `error.code`, pero el coordinador de pareja anterior lo descartaba al lanzar `PAIR_RESULT_UNCERTAIN`. Se reprodujo con el helper real y dobles de respuesta, y se corrigió mediante resultados separados y diagnósticos cerrados de fase/HTTP/conteos/servidor/IDs/proyección. Mensajes, stack, causa, cuerpos, cookies, rutas y tokens quedan fuera del registro. La evidencia de error no enmascara el rechazo original; se espera al otro envío y no se repite almacenamiento.

Se corrigió un rechazo de promesa sin manejar cuando Guardar fallaba antes de esperar la respuesta y se fortaleció la validación de referencias UInt32 escalares antes del callback de IDs. Regresión final **270/270 PASS** y OpenSpec estricto PASS. Se verificaron iguales las 912 entradas originales, la DLL instalada y respaldos, candidata histórica y artefactos de la corrida/lectura anteriores. No hubo login, SELECT real, carga o instalación en este avance. Las autorizaciones previas se rechazan por la huella de código nueva y no se creó otra concesión.

Informe/certificado `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03-diagnostico-codigos-local-20261106.md/.json`. La causa histórica individual continúa sin determinar y no se declara corregida una falla de negocio o guarda nativa. El próximo caso deberá conservar su diagnóstico desde el primer intento; un código público genérico del catch legacy puede requerir observación acotada antes del escritor. La evidencia real permanece **146/320 SELECT, A CONFIRMADO 16224/23322/intento 58, individual sin documento, B NO_INICIADO y runtime original restaurado**. 16/18, tareas 17.1/18.1 abiertas, sin cierre/archivo o aceptación de concurrencia/XML/SII.


## Diagnóstico previo al escritor preparado localmente — pendiente de permiso específico

Se preparó una candidata separada que evalúa la guarda individual original y registra solo códigos cerrados y 17 booleanos; termina antes del escritor tanto al aceptar como al rechazar. El cliente exige una cabecera de preparación vinculada al alcance antes de subir el PDF. No se relaja ninguna comparación original ni se atribuye todavía la causa histórica. Ocho variantes nativas, controles UI/permisos/incertidumbre y regresión conjunta final **286/286 PASS**; compilación completa sin errores y ensayo aislado con los mismos bytes de candidata PASS mediante guardián independiente, doble respaldo y ACL. Las 912 entradas originales, DLL instalada, respaldos y evidencia histórica permanecen intactos. Ningún login, SELECT real, carga o instalación real durante este avance.

Revisión `doc117-alm03-guard-diagnostic-listo-20261106.md`, propuesta `doc117-alm03-guard-diagnostic-propuesto-20261106.json` y certificado `doc117-alm03-guard-diagnostic-local-20261106.json`, en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/`. Alcance propuesto: localhost, usuario 141/empresa 2, expediente 467/nivel 8/relación 1072/tipología 50, un PDF temporal distinto de 677 bytes, cero documentos/ALMACENAR/importaciones/recursos nuevos, hasta 160 SELECT adicionales desde 146 (máximo 306 acumulados) y DLL diagnóstica temporal por 600 segundos. Ningún retry, replay, reconciliación, reparación o limpieza. Requiere permiso específico nuevo; todas las banderas públicas de ejecución/instalación/subida siguen false y no se creó otra concesión real. La causa y la concurrencia real siguen sin acreditar; el caso detenido conserva A 16224/23322/intento 58 confirmado, individual sin documento y B sin iniciar. **16/18, 17.1/18.1 abiertas**, sin cierre/archivo.


## Diagnóstico autorizado detenido antes de controles y runtime — estado vigente

El usuario autorizó expresamente el diagnóstico separado; corrida `188de3a60bbbc41d97cd4ccf3fa04266`, terminada con salida 2 el 2026-11-06 a las 19:58:42 UTC. `STOPPED_REVIEW_REQUIRED`, cero SELECT nuevos y **146 acumulados**, historial vacío, DLL original verificada; no existen lease/diario/reserva ni PDF temporal nuevo de esta corrida. No hubo activación de candidata, envío individual o repetición. El preflight inicial falló con `DOC117_ALM03_PROBE_STOPPED` / `UNCLASSIFIED_FAILURE`; no se retuvo el código específico, no se afirma credencial incorrecta o sesión establecida y no se identifica la causa. El último estado de negocio sigue siendo el de la lectura previa de 146 SELECT; esta corrida no lo releyó.

Se corrigió localmente el catch para conservar códigos cerrados DOC111/DOC116, observación saneada del helper y etapas de apertura/selección/lectura/baseline. Regresión conjunta posterior **289/289 PASS**, OpenSpec estricto y --check PASS; 912 entradas originales, DLL, respaldos, candidata nativa y evidencia histórica intactos. La concesión terminada no se modificó y se rechaza con el código nuevo; no hay segunda autenticación ni otra concesión creada. Informe/certificado `doc117-alm03-guard-diagnostic-detenido-20261106.md/.json`. El diagnóstico nativo no se alcanzó; pendiente clasificar y resolver la preparación antes de cualquier nueva ejecución. **16/18, 17.1/18.1 abiertas; ALM-03 NO_ACREDITADO**, sin cierre/archivo ni crédito de concurrencia/XML/SII.


## Diagnóstico real completo sin escritor — rechazo de descripción confirmado

La preparación aislada `7b23741c6623372c66e79ba854fa6f37` terminó PREPARATION_VERIFIED: sesión, nivel 7 y lote 22 accesibles, cero controles SELECT/efectos/runtime. El postback y Producción dieron HTTP 200; el cuerpo no retenido por Chrome no impidió la sesión y no identifica el fallo anterior. Continuación derivada del permiso original aún sin consumir `00ee73257399269edc67a1d474e4e3a7`: DIAGNOSTIC_COMPLETE_NO_WRITER, salida 0 a las 20:21:21 UTC del 2026-11-06, una subida temporal, cero documentos/escritor/importaciones, 104 SELECT adicionales (52+52), **250 acumulados** dentro del techo 306. Guardar ese acumulado para cualquier fase futura; no reiniciar presupuesto.

La guarda de laboratorio reprodujo DOC117_ALM03_GUARD_INDIVIDUAL_ARGUMENTS_CHANGED: descriptionExact=false y descriptionEquivalent=true, las otras 15 comprobaciones verdaderas. El literal Comprobante de Egreso bajo Option Compare Binary no coincide exactamente, aunque sí tras normalizar Unicode/espacios/mayúsculas. No se conserva el valor recibido y no se distingue cuál transformación es necesaria. WriterInvoked=false; el marcador UI WRITER_BLOCKED es el terminal esperado. No se afirma recuperar el error histórico ni atribuirlo al escritor productivo.

Baseline antes/después idéntico, inventario 8814→8814, A/intento 58 confirmado 16224/23322 e índice EEE, B/37 sin iniciar, staging/individual/recursos anteriores conservados. DLL original restaurada, doble respaldo byte a byte, reapertura original y finally verificados; 912 entradas originales y evidencia histórica intactas. Temporal nuevo y anterior, diarios y respaldos conservados; sin replay, reconciliación, reparación o limpieza. Informe/certificado vigente doc117-alm03-guard-diagnostic-verificado-20261106.md/.json. Pendiente corregir el vínculo de descripción canónica en guarda/controles y diseñar otro caso de coexistencia con alcance propio; este diagnóstico no prueba concurrencia o serialización. **16/18, 17.1/18.1 abiertas**, sin cierre/archivo/XML/SII.


## Descripción canónica corregida localmente, sin nueva ejecución real

Se implementó una variante de guarda que vincula por SHA-256 UTF-8 la descripción exacta del catálogo de tipología 50/expediente 467/empresa 2, en lugar del literal mixto. Normalización solo para validar el nombre humano aprobado; no reescribe argumentos ni relaja la comparación exacta preparación/entrada. UI con vínculo exacto antes de Guardar y controles canónicos que revalidan el catálogo y los valores físicos exactos; deriva de metadatos y rechazos anteriores preservados. Guarda/controles históricos conservan su camino de evidencia; la variante nueva se usa separadamente.

Compilación completa desactivada sin errores y regresión conjunta final **310/310 PASS**, incluyendo guarda .NET, representaciones de catálogo, negativos de identidad/selección/archivo/mutación y UI/controles. Se actualizó --check para comprobar evidencia existente byte a byte en vez de suponer que no existe. DLL instalada, cuatro respaldos, 912 entradas originales, candidata nativa y artefactos históricos intactos. Ningún login, SELECT real, PDF, instalación o concesión real nueva. Candidata local sin Site/Run/recursos/binding canónico reales, escritor/importaciones desactivados. Informe/certificado doc117-alm03-descripcion-canonica-local-20261106.md/.json.

Evidencia real vigente: **250 SELECT acumulados**, A 16224/23322/intento 58 confirmado, B sin iniciar; no repetir. La representación real del catálogo aún no se leyó/fijó con el protocolo nuevo. Para el siguiente caso faltan recursos, alcance/presupuesto y binding revisados propios; no reutilizar ni reiniciar los permisos consumidos. **16/18, 17.1/18.1 abiertas**, sin acreditar almacenamiento/coexistencia real posterior ni cierre/archivo.


## Preparación ALM-03C lista localmente — ampliación de presupuesto pendiente

Implementado y probado el ejecutor de preparación de un caso nuevo con nombres DOC117-ALM03C, niveles 7/8, mismos destinos autorizables y protocolo de alcance V2 con descripción canónica exacta del catálogo. Un plan/lote, dos recepciones PDF de 677 bytes, dos expedientes/materializaciones, cero documentos/ALMACENAR/carga individual/DLL. Rechaza IDs históricos y planes/lotes dentro del inventario preservado antes de recibir; compara catálogo, inventario global y documentos individuales antes/después, conserva todos los recursos y sella comandos futuros sin enviarlos. Los fallos conservan etapa, códigos/HTTP públicos cerrados e IDs; sin retry/resume/replay/reconciliación/reparación/limpieza.

Regresión final **333/333 PASS**, --check y OpenSpec estricto PASS; calculadora .NET copiada/verificada fuera de bin. DLL instalada, cuatro respaldos, 912 entradas originales, candidatas y evidencia histórica intactas. Ningún login, SELECT real o efecto nuevo. Propuesta pública aún sin conceder y ninguna autorización real creada. doc117-alm03c-preparacion-lista-20261106.md, doc117-alm03c-recursos-propuestos-20261106.json y doc117-alm03c-setup-local-20261106.json.

**250 SELECT acumulados**: se solicita ampliar expresamente el techo histórico **320→410**, máximo 160 adicionales desde 250; no se ejecuta con el saldo de 70 ni se reinicia contador. La recepción/creación de recursos y la ampliación requieren permiso específico. La ejecución documental/coexistencia y runtime quedan para otra revisión con IDs/binding reales y otro permiso/presupuesto. Estado real conservado: A 16224/23322/intento 58 confirmado, B/37 sin iniciar. **16/18, 17.1/18.1 abiertas**, sin cierre/archivo o nuevo crédito de concurrencia/XML/SII.


## Preparación ALM-03C real aprobada y completada — estado vigente

El usuario aprobó expresamente un plan/lote nuevo, dos PDFs de 677 bytes y dos expedientes/materializaciones, con cero documentos/carga individual/DLL, y amplió el techo 320→410 desde 250. Primera corrida b83e24a81417007ea8f92c54d3c624b1 detenida a tres SELECT antes de efectos por error del control: COUNT de toda la tabla comparado contra 8814, que correspondía a usuario 141/empresa 2. Corrección para usar los mismos filtros, conteos saneados durables y continuación solo con prueba de cero efectos y acarreo de las tres lecturas. Regresión **336/336 PASS**; no se afirma cambio de datos por el rechazo inicial. Las menciones previas a inventario global 8814 se aclaran como conteo de actor/empresa, sin reescribir artefactos históricos.

Continuación 30d7b62134015710dbb8c455c8723de6 PASS, salida 0 a las 22:27:58 UTC del 2026-11-06: plan 39/lote 23, raíces 37/38, archivos 38/39, expedientes 668/669, relaciones 1093/1094; creación confirmada en intentos 61/62. Dos recepciones/materializaciones, cero documentos/ALMACENAR/carga individual/runtime. Canonical binding exacto del catálogo fijado y revalidado en alcance V2; dos comandos futuros sellados privadamente y no enviados. **89/160 SELECT de la fase (3+86), 339/410 acumulados**; ningún reinicio de contador. Inventario actor141/empresa2 8814→8814 y documentos individuales 11→11 con huellas conservadas; preservación previa y originales verificados. Auditoría local independiente de originales de lotes22/23, DLL, cuatro respaldos, 912 entradas originales y evidencia histórica PASS; sin temporal individual nuevo en servidor.

Informe/certificado doc117-alm03c-setup-pass-20261106.md/.json. Alcance run71a43d1962ba3285b3801c4f2080934c, SHA9221008ec1566b27b7ee765e7ed8b770a2ba1b12bf42778eb5b40281d7d2ea19. No repetir la preparación, ni enviar comandos anteriores o nuevos sin su gate propio. Falta adaptar candidata/ejecutor al caso V2, compilar/ensayar y revisar otro permiso de documentos/runtime y presupuesto desde339; el saldo71 no los autoriza. **16/18, 17.1/18.1 abiertas**, sin crédito de coexistencia/serialización, cierre/archivo/XML/SII.


## Coexistencia ALM-03C lista localmente — ejecución pendiente de permiso propio

Adaptación al alcance canónico V2 del lote23 y expedientes668/669 completada. Candidata cerrada dc45234f… compilada fuera de bin sin errores y ensayo de restauración de sus mismos bytes PASS con guardián independiente, doble respaldo y ACL. Ejecutor revisado con cabecera de alcance antes de seleccionar PDF, descripción exacta del catálogo, pareja A/individual en sesiones independientes, B solo después de sus controles, resultados separados y registro nativo cerrado de rechazos. Los resultados parciales siguen al cierre/restauración y comprobación SELECT sin reenviar; no acreditan negocio o concurrencia.

Regresión final **355/355 PASS**, 41 archivos, --check y OpenSpec estricto PASS. Auditoría local de las 912 entradas originales, DLL instalada, cuatro respaldos, originales de staging de lotes22/23 y evidencia histórica PASS. La ruta ALMACENAR mantiene lote EN_CURSO y raíces EXPEDIENTE_CONFIRMADO aun al confirmar los documentos: el control verifica ese checkpoint y no añade consolidación ni otra mutación. Ningún login, SELECT real, documento, temporal individual del caso o activación del runtime real durante este avance.

Revisión doc117-alm03c-coexistencia-lista-20261106.md, propuesta doc117-alm03c-coexistencia-propuesto-20261106.json y certificado doc117-alm03c-coexistencia-local-20261106.json. Propuesta concreta: máximo tres documentos/solicitudes, candidata temporal600s y hasta320 SELECT nuevos desde339, ampliando expresamente techo410→659. Sin nuevos planes/lotes/recepciones/expedientes/materializaciones, retry/replay/reparación/reconciliación/limpieza. Serialización real implica NO_ACREDITADO sin otro intento. Banderas públicas false, ninguna concesión real de coexistencia creada.

La aprobación de setup cubrió cero documentos y cero cambios de DLL: no concede este alcance documental/runtime ni la ampliación. El acumulado real permanece **339/410** y la coexistencia nueva **NO_EJECUTADA**; A antiguo16224/23322/intento58 confirmado y B/archivo37 sin iniciar se conservan sin repetir. **16/18, 17.1/18.1 abiertas**, sin aceptación de concurrencia/XML/SII, cierre o archivo.
