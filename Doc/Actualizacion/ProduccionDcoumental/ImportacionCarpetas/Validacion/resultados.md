# DOC-120 — índice cronológico de resultados

## DOC-120 - guardado real, pausa, cierre y retoma aprobados2026-11-19

Ventana08bc33cc7b92403e8dd1ba47adca1a86 PASS_REAL_STORAGE_UI: lote52 pasa80 a84 guardados;2177-2180 producen18341-18344, expediente696/relacion1120. Pausa espera sin siguiente; cancelar salida permite continuar automaticamente; aceptar cierre espera respuesta; recarga/continuacion y nueva recarga sin replay. Tres recepciones/134642 bytes y cuatro materializaciones de datos, sin planes/lotes/expedientes nuevos.623 HTTP/50 SELECT externos de control/1 sesion, dentro de1200/80/1. Promedio HTTP0,98075s, rango0,938-1,022s; excluye retencion de entrega al navegador, UI y controles. RA/DA, conteos e identidades unicas verificados;996 ajenos/valores/origenes y auditoria/configuracion anteriores preservados. Cerrada, admision cerrada y claves eliminadas; no relanzar. Evidencia Validacion/doc120-guardado-real-resultado-20261119.md/JSON/CSV y SHA del estado local; fuentes actuales coinciden. No certifica1000 completos, inicio/creacion/arbol, rollback o descarga fisica;256/267 y once brechas anteriores/manual pendientes. Revision de evidencia sin nuevos efectos, SQL o cambios de producto/DLL/esquema. Apartados inferiores historicos.

## DOC-120 - prioridad de guardado real, pausa y cierre2026-11-19

Run5db2662119bd4d9ab7985548e6cdfea5: consola privada PID27716 termino STOPPED/CONNECTION_INVALID antes de acceso,0 HTTP/0 SELECT/0 sesiones/0 efectos; cerrada y secretos eliminados. Certificado/DSN se validaron en un control combinado, sin retener datos escritos; no se conoce cual de esos datos causo el rechazo. No repetir automaticamente. Ejecutar `run-doc120-storage-ui.cjs --run <nuevo32hex> --authorized-storage-ui` solo para una ventana nueva, nunca repetir este ID. Reutiliza autenticacion, consultas, controles RA/DA y seleccion existentes; Chromium visible/slowMo250. Lote52/propietario141/empresa2, cuatro proximos pendientes:4 ALMACENAR, hasta4 recepciones/96MiB y4 preparaciones de datos si hacen falta;1200 HTTP/80 SELECT/1 sesion/45min. Cero planes/lotes/expedientes nuevos, perfiles, borrados o reanudacion administrativa. Solicitud iniciada espera respuesta; solo se retiene su entrega al navegador para observar controles. Secuencia: pausa A; continuar B/cancelar salida e iniciar C automaticamente; aceptar cierre durante C y esperar respuesta; recargar/continuar D/pausar/recargar sin repetir. Verificar cuatro checkpoints RA/DA, conteos y996 archivos/valores/origenes preservados.68/68 locales,10 nuevos; no equivale a PASS nativo. Evidencia Validacion/doc120-guardado-real-visible-20261119.md/JSON.3.243-3.247 completas en alcance local/preparacion/apertura;256/267, once brechas anteriores conservadas. No relanzar ventanas cerradas ni afirmar QA actual aprobada. Producto/DLL/esquema sin cambios. Apartados inferiores historicos.

## Consulta secundaria conservada; E2E real detenida -2026-11-19

[Diagnostico y limites](doc120-consulta-visible-20261119.md), [fuentes](doc120-consulta-visible-20261119.json). Run97c9d4c3/lote52 STOPPED/DOC119_STOPPED con192 HTTP/10 SELECT/1 sesion, cerrada/secretos eliminados y sin endpoint escritor; sin huella posterior verificada. Defecto UI reproducido: pagina con todos sus archivos guardados elimina Actualizar estado al reemplazar pie por Continuar. Se conserva el mismo boton en cabecera y se clasifica su espera como RESULT_CONTROL_NOT_READY. AUD-08 rojo->verde;44/44 focales y8/8 diagnostico con7 solapados. Amplia223/224; contextual teclado falla una vez y pasa4/4 aislado sin cambios, causa intermitente no certificada/6.4 pendiente. No declarar corrida224/224 ni nueva E2E real aprobada. Assets doc120-consulta-visible-20261119-9;251/262 tareas,108 escenarios,11 pendientes previas. Ventana cerrada no se relanza.

## Parametros de video para E2E visible -2026-11-19

[Parametros y limites](doc120-e2e-parametros-video-20261119.md), [fuentes](doc120-e2e-parametros-video-20261119.json). Lote52 preseleccionado por --lot, solo dentro de catalogo autorizado, sin fallback ni recuperacion de otro lote. Egresos v3 mostrado antes de consulta no sustituye perfil confirmado.41/41 locales,2 nuevos; nueva consola97c9d4c3/PID15952, resultado real pendiente.36a221bf sin proceso activo y estado anterior preservado; sin recuperar secretos.250/261 y11 pendientes previas. Sin cambios del producto/DLL/esquema.

## E2E existente con Chromium visible -2026-11-19

[Preparacion y alcance](doc120-e2e-visible-20261119.md), [fuentes](doc120-e2e-visible-20261119.json). run-doc119-seguimiento.cjs --headed conserva guardias/limites solo lectura y abre Chromium con slowMo300; consentimiento de nueva ventana antes de credenciales. Cinco casos nuevos,39/39 politicas y20/20 compatibilidad (cinco solapados). Consola36a221bf abierta y Chromium iniciado/AUTHENTICATING_OWNER;96 HTTP/0 SELECT/1 sesion al corte, resultado real pendiente.249/260 y once pendientes previas. No confirma, recibe, crea, almacena ni reanuda; no valida pausa durante guardado. Sin cambios del producto/DLL/esquema.

## Auditoria de transiciones - corte local2026-11-19

[Dictamen y matriz de recorridos](doc120-auditoria-transiciones-20261119.md) y [fuentes/resultados](doc120-auditoria-transiciones-20261119.json). Cuatro defectos reproducidos y corregidos: pagina de raices heredada de otro lote, borrador de filtro que reaparecia, ayuda cerrada por acuses y mensajes/progreso de revision copiados a consulta. Siete casos nuevos;223/223 en24 suites finales, con1000 sinteticos. La primera amplia214/223 y el contraste parcial anterior se conservan: ocho expectativas antiguas y un cierre esperado sin aceptar; cinco suites actualizadas38/38 antes de repetir la seleccion completa. Ejecutor tools/e2e/scripts/test-doc120-transitions.cjs; assets doc120-auditoria-transiciones-20261119-8.247/258 tareas,107 escenarios,11 pendientes previas. Cero HTTP/SELECT/autenticacion/efectos reales, DLL o esquema; QA historica cerrada sin replay. No declara ausencia absoluta de errores ni certificacion de negocio real. Cortes inferiores historicos.


## Salida desde lote consultado - corte local2026-11-19

[Dictamen](doc120-salida-consulta-20261119.md) y [fuentes/resultados](doc120-salida-consulta-20261119.json). Video200257: preparacion guardada vacia ocultaba consentimiento del lote visible. Ahora se prioriza el lote actual pendiente y se advierte aparte revision/otro lote guardado; cancelar conserva ejecucion y aceptar espera acuse. CTX-EXIT01-03/tareas3.231-3.234, cinco casos nuevos; reproduccion roja0/2, focal5/5 y regresion53/53; texto1/1 solapado. Cabecera visible/scroll a1280-768-390.243/254 tareas,103 escenarios,11 pendientes previas. Assets doc120-salida-consulta-20261119-7; cero autenticacion/HTTP/SELECT/efectos reales, DLL o esquema. Aceptacion visual actual pendiente; sin operaciones sobre52 ni replay de ventanas cerradas. Cortes inferiores historicos.


## Salida de revision antes de pausar - corte local2026-11-19

[Dictamen](doc120-revision-salida-20261119.md) y [fuentes/resultados](doc120-revision-salida-20261119.json). Video191801 cancela aviso nativo y queda pausado600/1000. Consentimiento del modulo ahora precede pausa para preparacion/confirmacion/lote; cancelar conserva ejecucion, aceptar espera lo iniciado. Una continuacion principal sobre ticket vigente, cambios invalidan preparacion y contador incluye ultima parte confirmada.81 unicos locales (63/65 amplios +18/18 focal/core, con dos reparados y16 distintos);6 nuevos y1/1 estilo primario solapado.1000 sinteticos/DOM/memoria aprobados, sin capacidad real nueva.239/250 tareas,100 escenarios,11 pendientes previas. Assets doc120-revision-salida-20261119-6 incluyen segmentador. Sin autenticacion/HTTP/SELECT/efectos reales, DLL o esquema; aceptacion visual actual pendiente. Cortes inferiores historicos.


## Etiqueta Pausar y salir - corte local2026-11-19

[Dictamen](doc120-pausar-salir-20261119.md) y [fuentes/resultados](doc120-pausar-salir-20261119.json). Aprobacion expresa: salida activa y consentimiento muestran Pausar y salir visible/accesible; reposo/pausa/completado conserva Cerrar. Mismo manejador y espera, sin cambios de negocio.42 casos existentes locales verificados (34/42 +10/10 focales, dos solapados),0 nuevos; cuatro fases/teclado/390-768-1280, incertidumbre y mensajes conservados.235/246 tareas,97 escenarios,11 pendientes previas. Assets doc120-pausar-salir-20261119-5; sin QA real/HTTP/SELECT/efectos, DLL o esquema. Aceptacion visual actual pendiente; cortes inferiores historicos.


## Mensajes vigentes y aviso compacto - corte local2026-11-19

[Dictamen](doc120-mensajes-vigentes-20261119.md) y [fuentes/resultados](doc120-mensajes-vigentes-20261119.json). Video103048 muestra recuperacion/avance22 a99 guardados, aviso alto y franja de pausa previa al continuar. Se actualizan mensaje/render inmediato solo inicio/pausa; error/incertidumbre conservados. Aviso local resumido, detalle desplegable sin operaciones y limites/scroll de dialogo.59/59 locales,5 nuevos;1/1 focal solapado del ultimo boton. Geometria1552x912/768x600/390x600; no se repite1000.232/243 tareas,96 escenarios,11 pendientes previas. Assets doc120-mensajes-vigentes-20261119-4. Sin nueva QA real/HTTP/SELECT/efectos, DLL o esquema; aceptacion visual actual pendiente. Cortes inferiores historicos.


## Recuperacion visible de originales - corte local2026-11-19

[Dictamen](doc120-recuperacion-originales-20261119.md) y [fuentes/resultados](doc120-recuperacion-originales-20261119.json). Video092932: lote51 muestra10 recibidos/guardados y990 pendientes; falta seleccion local, no se acredita perdida de temporal recibido. Aviso local ofrece carpeta/PDF, pausa antes del selector y seleccion no envia; cancelar/pausar/modificado conserva recuperacion visible. Temporal RECIBIDO mantiene camino anterior sin reemplazo local.56/56 casos locales,6 nuevos;6/6 focales solapados tras ajustar texto/asercion. Sintetico1000 vincula990 sin efectos y continua990 una vez, conservando10 IDs,0 nuevas confirmaciones/expedientes.228/239 tareas,93 escenarios,11 pendientes previas. Assets doc120-recuperacion-originales-20261119-3. Sin nueva QA real/HTTP/SELECT/efectos, DLL o esquema; aceptacion manual actual pendiente. Cortes inferiores historicos.


## Consentimiento de cierre - corte local2026-11-19

[Dictamen](doc120-consentimiento-cierre-20261119.md) y [fuentes/resultados](doc120-consentimiento-cierre-20261119.json). Video090901 muestra salida sin pregunta del lote51; ahora X/Escape sobre lote confirmado pendiente piden consentimiento antes de pausar. Cancelar no cambia ejecucion; aceptar espera respuesta/lectura y no admite siguiente fase.78 casos unicos locales verificados:73/78 amplios +11/11 focales tras adaptar cinco fixtures (seis solapados);11 nuevos. Foco/viewport390/768/1280.224/235 tareas,90 escenarios,11 pendientes previas. Assets doc120-consentimiento-cierre-20261119-2. Sin nueva QA real, HTTP/SELECT/efectos, compilacion/sustitucion DLL o esquema; hash instalado observado distinto del historico, sin certificar esa DLL. Aceptacion manual actual pendiente. Apartados inferiores historicos.


## Controles y cierre seguro - implementación local2026-11-19

[Dictamen](doc120-controles-cierre-20261119.md) y [JSON de fuentes/resultados](doc120-controles-cierre-20261119.json), D-04/RQ-04, D-05/RQ-05, D-08/RQ-08, D-10/RQ-10. CTL-01–06/tareas3.204–3.211: política compartida, una intención, X/Escape tras acuse, pausa de admisión manual, metadatos en curso, respuesta perdida y desmontaje forzado diferenciado.22 pruebas nuevas;142 casos únicos cubiertos mediante141/142 iniciales y8/8 focales tras corregir redacción, con siete solapados. Corridas de1.000 archivos sintéticos/DOM/memoria y consultas/CSV/árbol pasan; no son otra QA real.

220/231 tareas locales/documentales verificadas,87 escenarios y11 pendientes previas. Sin nueva autenticación/HTTP/SELECT/efectos reales, DLL o esquema. Assets de API/vista/controlador versionados doc120-controles-cierre-20261119-1; aceptación visual posterior pendiente. La QA5f7af8f5 conserva fuentes anteriores y no acredita este nuevo cierre; los videos del usuario muestran la culminación previa de1.000 archivos sin nueva auditoría SQL del agente. Los apartados inferiores conservan sus cortes y límites históricos.

## QA real aprobada con límites -2026-11-19

**PASS_QA_WITH_LIMITS** en5f7af8f5354a4fca5e2eb5c7a53b4b4d: recuperación/árbol sin replay, original local faltante por decisión, pérdida de entrega tras checkpoint, dos sesiones HTTP con un ganador, revisión antigua y privacidad de ambos lotes. [Resultado y límites](doc120-qa-recuperacion-concurrencia-20261119.md), [JSON](doc120-qa-recuperacion-concurrencia-20261119.json), [CSV](doc120-qa-recuperacion-concurrencia-20261119.csv).758 HTTP/22 SELECT externos/3 sesiones;120/121→16284/16285, mismo expediente693,23→25 guardados. Se conservan16282/16283 del29, B51 pendiente, una relación lógica con etiqueta por código y huellas25/998. Media HTTP de los dos últimos1,570348s; casos de QA con respuesta suprimida/carrera, no benchmark pareado.

3.196/3.202/3.203 completas en alcance acotado:212/223,11 pendientes.3.87 conserva inicio UI completo/aceptación manual;3.187 rollback nativo y otros escenarios; capacidad/segmentación/publicación/aceptación restantes sin certificar. Estados previos intactos, credenciales eliminadas y cero ejecuciones nuevas en esta revisión. DLL8b27a942 conservada;35/35 regresiones locales del ejecutor/árbol/mensajes en el corte previo. Los apartados inferiores son cortes históricos con sus propias versiones y límites.

Implementacion local2026-11-18: [evidencia y limites](doc120-independientes-20261118.md), [contrato focal](../Diseno/contrato-focal-implementado-doc120.md), script09 manual sin aplicar.25 tareas locales verificadas;204/217,13 pendientes (once anteriores y3.187/3.196);27 S/190 M,81 escenarios.258 pruebas Node unicas aprobadas tras repeticion focal documentada,2 wrappers Framework/DDL incluidos sin sumar internos, compilacion aislada0 errores. Sin desplegar DLL/assets, SQL aplicado o nueva prueba real.

Flujo vigente: [resumen de guardado en texto](../Diseno/flujo-tecnico-importacion-carpetas-doc120.txt), anexo/manifiesto regenerados para124 fuentes. El sello inicial y la auditoria final son separados del costo normal por archivo; no se promete un tiempo IIS o numero total de SELECT antes de medir.

## Tres archivos medidos en ejecución real —2026-11-17

Usuario «ejecuta la prueba» autorizó instalar la instrumentación y medir tres guardados del lote50. Run23e18a40 **FINISHED_THREE**,3 confirmados y verificados,7→10 documentos y997 archivos preservados. [Tiempos/controles reales](doc120-medicion-tres-20261117.md), JSON y CSV:93,177/74,219/70,900s HTTP, writer0,152/0,023/0,020s; lectura/reconstrucción de metadatos72,321% del coordinador. DLL instrumentada instalada con respaldo;105 HTTP totales/14 SELECT de control/1 sesión. Sin optimizar el recorrido; secretos eliminados y ventana cerrada.3.169 completa,178/189; once pendientes previas conservadas.

## Medición de tres documentos — preparación2026-11-17

Solicitud real del usuario delimitada en [plan de tres archivos](doc120-medicion-tres-plan-real-20261117.md). Captura Server-Timing por solicitud y runner con guardias preparados;8/8 locales y compilación aislada. Estado **PREPARADO; NO EJECUTADO**: requiere aprobación de instalación y credenciales privadas. No hay tiempos reales nuevos ni escrituras de esta ventana;3.169 sigue abierta. No se aplicó la optimización propuesta.

## Consulta del último lote — corrección2026-11-17

Video170531 muestra el acceso deshabilitado sin ID local, incluso eligiendo Egresosv3. [Causa y resultado local](doc120-consulta-ultimo-20261117.md)/JSON:70/70 Node únicas, compilación aislada y consulta de último lote propio sin perfil/referencia guardada. Búsqueda y lectura no ejecutan importación. Nueva DLL todavía requiere despliegue autorizado; los once pendientes previos continúan abiertos.

## Espera y reanudación general — implementación local2026-11-17

[Evidencia local](doc120-espera-reanudacion-20261117.md) y JSON compañero documentan3.136–3.158: espera del acuse, pausa/cierre sin duplicados, reanudación explícita con revisión/evento, recuperación visual del inventario y medición por etapa. Compilación aislada y pruebas locales; sin instalación de DLL, HTTP/SELECT reales, ni recuperación del lote50. El modelo transaccional no prueba rollback MySQL real. [Propuesta de optimización](../Diseno/medicion-optimizacion-almacenamiento-doc120.md) conserva integridad; [ventana preparada](doc120-reanudacion-plan-real-20261117.md) todavía sin ejecutar. Los cortes inferiores mantienen sus resultados y autorizaciones originales.

## Detención persistente del lote50 — 2026-11-17, diagnóstico real posterior

Ventana autorizada8803b6f4:5/5 SELECT RA,0 HTTP/sesiones/efectos; cerrada y secretos eliminados. DETENIDO/señal1/revisión3026;1.000 recibidos,1 creación confirmada,4 documentos completados y996 sin iniciar almacenamiento, todos con valores efectivos.0 intentos activos/inciertos y0 marcadores. Detención3544 registrada bajo cuenta propietaria a14:58:35 Bogotá; revisión/último evento estables. [Fuente y límites](doc120-detencion-lote50-20261117.md), JSON compañero.3.135 verificada;144/155 tareas. No se levantó la detención ni se reanudó/importó; no se inspeccionaron documentos físicos o DA.

## Parada identificada por archivo — 2026-11-17, corte posterior

Solicitud de mensaje claro tras video PARADA:3.132–3.134 completas con89/89 locales. El panel conserva causa/espera real del cliente, archivo/ruta/posición, último avance confirmado y siguiente acción. Publica la parada antes de esperar la consulta posterior; distingue consulta fallida, intento activo y guardado confirmado sin replay ni continuación automática.4 GET estáticos200 coinciden con las fuentes principales;0 autenticaciones/SELECT/efectos. [Evidencia y límites](doc120-parada-diagnostico-20261117.md), JSON compañero.143/154 tareas; capacidad real y pendientes anteriores continúan abiertos.

## Longitud de BENEFICIARIO — 2026-11-17, medición y ajuste preparado

3.129 verificada con9 SELECT:25.153 filas, máximo77,230 valores mayores a60; origen250 frente a CONTABIL.BENEFICIARIO/regla60. Ajuste a250 preparado,14/14 locales. Intento d605ad7e cerrado en login con96 HTTP/1 sesión; guardia de presentación corregida. Continuación fa226614 autenticó y se detuvo por PUBLICATION_CHANGED antes de escribir:98 HTTP/1 sesión/0 SELECT de control/0 mutaciones. Ambas cerradas, secretos eliminados.3.130/3.131 pendientes de observar publicación/revisiones actuales; no se amplió ni publicó por estas corridas. [Evidencia y límites](doc120-egresos-longitud-20261117.md), JSON compañero. No se ejecutaron importaciones.

## Relectura de plan sellado — 2026-11-17, corte posterior

Reporte de fallo tras1.000/1.000 revisados, referencia62232125f0f64fdfb1537b2251288a35: se reproduce localmente inconsistencia entre snapshot original persistido y huella del snapshot canónico. `beae0afc` corrige escritura y lectura compatible sin actualizar datos.18/18 locales del corte, Build completo y DLL/PDB instalados;1 HTTP anónimo401,0 SELECT/efectos. [Fuente, huellas y límites](doc120-lectura-plan-sellado-20261117.md), JSON compañero. La instancia real reportada no se leyó con cuenta autenticada; confirmación pendiente. Retorno recomendado vigente `beae0afc`;139/148 tareas y aceptación de1.000 PDF pendientes.

## SQL manual verificado y DLL desplegada — 2026-11-17, corte posterior

Por confirmación del SQL manual y continuación expresa:10 SELECT reales de metadatos,3 tablas compatibles y DLL/PDB actualizados solo en el sitio local. Corrección del falso rechazo de índices padre reproducida antes y validada después;11/11 locales y Build completo. Retorno mínimo `af1ff073`; la recomendación anterior `d5bae464` queda sustituida. Dos GET WSDL500 reproducen un defecto SOAP anterior; la nueva consulta JSON anónima devuelve401/SESION_REQUERIDA, sin datos. Consumo técnico3 HTTP/10 SELECT/0 sesiones autenticadas/0 efectos. [Fuentes y límites](doc120-despliegue-esquema-20261117.md), JSON compañero.139/148 tareas; aceptación de1.000 PDF y rollback/carrera MySQL pendientes.

## Preparación segmentada, lectura focal y recuperación masiva — 2026-11-17

Implementación local del contrato08 aprobado:1.000 metadatos en partes automáticas, un plan/lote, lecturas focales y vinculación de carpeta para pendientes. **200 casos locales únicos**:199 del corte principal y1 caso nuevo de referencia, con reejecución15/15 de cliente y2/2 wrappers sin sumarlos otra vez. Dos compilaciones completas y compatibilidad B→A→B de planes1/2; retorno mínimo `d5bae464`. Node:3.044 lecturas/3.001 efectos sintéticos/6.665ms; Chromium:3.047 lecturas/766 nodos/p95=7,3ms. [Evidencia, fuentes y límites](doc120-segmentada-20261117.md), JSON compañero y [ventana real propuesta](doc120-capacidad-plan-real-20261117.md).

**139/148 tareas**,53 escenarios.3.118 queda abierta por fallo/rollback y carrera real MySQL no ensayados;3.126/3.127 por carga/continuación sin autorización. Las seis pendientes anteriores conservan sus condiciones. Cero HTTP de aplicación, SELECT o efectos reales; SQL08/DLL sin aplicar a IIS. Los cortes siguientes son históricos y no describen el estado vigente del contrato08.

## Línea base de capacidad y contrato propuesto — 2026-11-16

3.98 verificada con inventario/API/motor reales y transporte/efectos sintéticos: cuerpo de1.000 metadatos172.409 caracteres; preflight sin XHR; casos de1/100 archivos completan y1.000 se bloquea con COORDINACION_LIMITE tras232 recepciones sintéticas y10.000 lecturas modeladas. No se alcanza creación/almacenamiento. [Resultado, reproducción y límites](doc120-capacidad-base-20261116.md), JSON compañero y [contrato propuesto](../Diseno/08-preparacion-segmentada-capacidad.md).111/148 tareas,41 escenarios vigentes;12 casos nuevos aún propuestos.3.97 requiere aprobación del contrato antes de producto. Cero HTTP/SELECT/efectos reales, migraciones o activación IIS. Cortes siguientes históricos.

## Mensaje de preparación grande — 2026-11-16

3.95/3.96 verificadas: límite efectivo como metadato, preflight del cuerpo sin POST al excederlo, aviso de cantidad/tamaño/máximo y señales ASMX/HTTP413 cerradas; errores desconocidos conservan incertidumbre y privacidad. **122/122 locales**,10 nuevas e incluidos2 wrappers Framework/DDL sin sumar internos; compilación aislada0 errores. **110/116 tareas**,41 escenarios. [Fuentes y límites](doc120-mensaje-tamano-20261116.md) y JSON compañero. Sin activar IIS, ampliar capacidad o ejecutar importaciones reales. Seis tareas finales pendientes. Cortes siguientes históricos.

## Selección y revisión comprensibles — video121039

3.93/3.94 implementadas: explicación del permiso nativo sin otro diálogo, resumen único, fichas con código/nombre completos y referencia del plan en detalles. Incidencias, datos previstos y operaciones conservados. **110/110 locales**,6 nuevas y104 superpuestas al corte previo; no se suman a los207 históricos. Sin backend nuevo, HTTP/SELECT/efectos reales ni activación IIS. **108/114 tareas**,40 escenarios. [Evidencia y límites](doc120-revision-claridad-20261115.md) y JSON compañero.3.86/3.87/5.2/6.4/6.5/6.6 permanecen abiertas. Cortes siguientes históricos.

## Inicio único y códigos repetidos — 2026-11-15

3.72–3.85/3.88–3.92 verificadas localmente: Confirmar e importar coordina el lote completo, con acuse y lectura por fase, pausa, recuperación explícita y acceso por ID. Se retiran las restricciones de códigos repetidos conservando comprobaciones técnicas, permisos, integridad e idempotencia. **207/207 pruebas Node únicas**, incluidos2 wrappers Framework/DDL sin sumar internos; compilación aislada0 errores. **106/112 tareas**,40 escenarios. [Detalle y límites](doc120-coordinacion-20261115.md), JSON compañero y [ventana real propuesta](doc120-coordinacion-plan-real-20261115.md).3.86/3.87 pendientes de autorización/ejecución específica, además de5.2/6.4/6.5/6.6. Sin activar IIS, HTTP/SELECT/efectos reales ni aceptación integral. Los cortes siguientes conservan su fecha y alcance.

## Etiqueta por código — 2026-11-15

Petición «debe ser codigo unico»:3.69–3.71 verificadas localmente. Nuevas creaciones guardan código como alias; consulta y proyección del árbol separan código/nombre y conservan identidad por ID, búsqueda y expansión. **75/75 pruebas locales únicas**, wrappers Framework/DDL incluidos sin sumar casos internos; compilación aislada0 errores. Sin migrar filas históricas, cambiar políticas de coincidencia, activar IIS ni ejecutar efectos reales. Avance**87/91**,37 escenarios; abiertas5.2/6.4/6.5/6.6. [Detalle, fuentes y límites](doc120-etiqueta-codigo-20261115.md), con huellas en el JSON compañero. No renueva los resultados históricos siguientes ni concede aceptación integral.

## Primer expediente del nivel — corrección2026-11-14

El usuario reporta que recuperó el lote y terminó la importación, con expediente ausente del árbol. Reproducción local: Doc111ActualizarRama exige un contenedor que WebForms omite en niveles vacíos y el asistente ignora el fallo. Se reutiliza el insertor del primer hijo, se verifica la relación y se conserva selección/expansión existente, búsqueda y contador. Si no se puede mostrar, hay aviso y Actualizar reintenta solo la proyección. **74/74 pruebas locales únicas**, incluidos cinco focales superpuestos y expansor nativo System.Web; sin operaciones reales ni delta backend. Avance**57/61**,36 escenarios. La actualización en IIS queda sin observar; abiertas5.2/6.4/6.5/6.6. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-primer-expediente-arbol-20261114.json`; instrucciones `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-primer-expediente-arbol-20261114.md`. Cortes siguientes históricos.


## Reanudación permanente implementada — 2026-11-14

**Retomar creación** queda incorporado al seguimiento de cualquier importación elegible, sin configuración por cuenta. La consulta comprueba una reserva de creación activa, sin inicio ni evidencia de resultado y con diario coherente. Confirmar vuelve a validar permisos, destino, catálogos, recepción y staging; rota la reclamación del mismo intento bajo bloqueos/CAS y conserva su identidad, hash, inicio, recibo, valores y snapshot. Solo levanta la detención observada con consentimiento; una revisión nueva o intento ya iniciado se rechazan.

La interfaz mantiene consulta/último lote, filtros, CSV y árbol. Cancelar no envía; confirmar solicita una fase. Tras la creación confirmada aparece Preparar índices; no recibe ni almacena automáticamente. Una respuesta perdida exige consultar y no produce replay. **108/108 pruebas locales únicas**, incluidos dos wrappers Framework/DDL cuyos casos internos no se suman, y compilación local0 errores; Chromium sintético390/1280px. Las pruebas de SQL/transacciones/escritor usan dobles y staging temporal real; no prueban concurrencia en MySQL real ni sesión autenticada de IIS.

Avance**55/59**,35 escenarios. El lote39 no se recuperó en este corte:0 HTTP de aplicación,0 SELECT de negocio,0 sesiones y0 efectos reales nuevos. Las8 lecturas intentadas previas se conservan. Abiertas5.2,6.4,6.5,6.6 y aceptación integral pendiente. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-retomar-creacion-permanente-20261114.json`; detalle `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-retomar-creacion-permanente-20261114.md`. Los cortes siguientes son antecedentes históricos y no se renuevan sus resultados.


## Fecha canónica corregida — diagnósticof23baf65,2026-11-14

La ventana autorizada de1 SELECT RA terminó STOPPED reproduciendo **ROOT_FIELD_RULE / FECHA_INICIAL / LONGITUD_EXCEDIDA**: fuente/tipo coinciden y el valor resuelto ocupa10 caracteres. El validador resuelto conservaba el límite menor de la entrada original.8 intentos acumulados,7 lecturas concluidas conocidas y1 anterior de resultado desconocido;0 HTTP/sesiones/efectos y credenciales eliminadas. No se guardó el valor real ni se presume la longitud exacta de la regla.

Corrección acotada en ValidarValorResuelto: sobre copia, FECHA se valida como yyyy-MM-dd con longitud10. La entrada original sigue respetando formato/límite del perfil; fechas inválidas/no canónicas y tipos no fecha siguen rechazados. No cambian reglas publicadas, snapshot, permisos, barreras ni reserva96. Reproducción fallida antes en la DLL anterior con el mismo detalle del caso real; **29/29 pruebas locales** después, incluidos1 wrapper Framework y1 DDL cuyos casos internos no se suman, y compilación local0 errores. La DLL compilada acepta el caso sintético compacto sin mutar recursos.

3.30/3.31 completadas en diagnóstico de causa, corrección y propuesta de recuperación; avance**47/51**. **El lote39 no fue recuperado ni probado de nuevo con efectos.** La reserva y detención permanecen; la acción de retomar una reserva requiere implementación revisada y autorización propia. Propuesta técnica, todavía no disponible: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-recuperacion-reserva96-propuesta-20261114.md`. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-fecha-canonica-corregida-20261114.json`. Sin aceptación integral; los cortes siguientes conservan su alcance histórico.


## Rechazo del comando confirmado — ventana704fde28,2026-11-14

Ventana autorizada por «continua» sobre9d64595a: **STOPPED**,1 SELECT de metadatos concluido,0 HTTP/sesiones/efectos y huellas estables; credenciales eliminadas. El comando real rechaza la copia contrafactual con **CONFIGURACION_INVALIDA en COMMAND**, antes de consultar catálogos. Esto acota el problema a snapshot/reglas/valores de creación; no identifica aún el predicado ni acredita la excepción histórica.7 intentos acumulados (6 lecturas concluidas conocidas y1 de resultado anterior desconocido). La reserva96 permanece intacta.

Refinamiento local: clasificación del guard posterior al rechazo real, campos/códigos de lista cerrada y modo `--command-only` de **una SELECT RA**, sin catálogos. **26/26 pruebas locales**,14 focales; sin cambios de producto ni nueva lectura real. Propuesta de una lectura adicional, máximo8 acumulados, pendiente de autorización; no se ejecuta automáticamente. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-rechazo-comando-20261114.json`; plan `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-comando-detalle-plan-20261114.md`. Avance**45/51**;3.30/3.31 siguen abiertas y no hay recuperación ni aceptación integral. Los cortes siguientes son históricos.


## Preparación del diagnóstico de prerrequisitos — 2026-11-14

3.29 completada; se añaden3.30/3.31 para diagnóstico real específico y corrección de causa comprobada. Auxiliar de lectura reutiliza comando/catálogo reales y queryRows; distingue etapas, protege SQL/tablas/parámetros, conserva presupuesto y elimina credenciales. La copia contrafactual previa a la detención no demuestra sesión, conexión legacy ni transacción, ni permite liberar/repetir96. **23/23 pruebas locales**, sin leer negocio ni modificar producto. Plan de hasta30 SELECT RA/DA,0 HTTP/efectos;6 intentos previos conservados, máximo36 acumulados. Autorización solicitada sin respuesta; no se ejecuta. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-prerrequisitos-preparacion-20261114.json`; plan `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-prerrequisitos-plan-20261114.md`. Avance**45/51**; abiertas3.30,3.31,5.2,6.4,6.5,6.6. El lote real permanece sin recuperación; cortes siguientes históricos.


## Estado real del lote39 — diagnóstico933961f9,2026-11-14

**Diagnóstico de estado completado; recuperación sin resolver.** Ventana autorizada expresamente sobre50803073:5 SELECT concluidos,0 HTTP/0 sesiones, credenciales eliminadas. Con el intento64c70664 anterior:6 intentos presupuestados y5 lecturas concluidas conocidas; no se reconstruye el resultado fallido anterior. Lote39/plan57 EN_CURSO, carpeta60 CREANDO e intento96 **RESERVADO**, sin evidencia CREACION_PREPARADA ni identidad de expediente registrada. Archivo86 RECIBIDO/NO_INICIADO, recepción95 CONFIRMADA. La detención se solicitó después (00:43:02 UTC frente a la reserva00:39:26), por lo que no explica el rechazo inicial.

El código actual registra EN_CURSO antes del escritor; la lectura orienta a comprobaciones previas o a la transacción de inicio. **No identifica la validación fallida ni demuestra ausencia del expediente en tablas de negocio.** No se liberó el intento, reconcilió, recibió, creó ni almacenó. Los estados activos no autorizan replay por tiempo. Tarea3.27 completada por diagnóstico válido de estado; avance**44/48**. La brecha crítica de recuperación sigue bloqueando6.4/aceptación integral. Cualquier lectura adicional o recuperación real necesita alcance y autorización específicos. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-diagnostico-lote39-20261114.json`; filas saneadas y ejecución permanecen excluidas de Git. Los cortes siguientes conservan su alcance histórico.


## Diagnóstico detenido y corrección del reporte — 2026-11-14

Ventana autorizada64c70664, sobre3010eef7: lote39/carpeta60, STOPPED en el primer intento contabilizado,0 HTTP/0 sesiones y credenciales eliminadas. No hay archivo de resultados; el contador anterior no distingue apertura fallida de SQL ejecutado. La causa del lote y la existencia del expediente siguen desconocidas. El ejecutor descartaba las categorías seguras DOC111_SELECT del puente: corrección local conserva solo la lista cerrada y agrega lectura actual/intentos/respuestas concluidas. **12/12 pruebas locales**, sin consultar datos de negocio ni repetir la corrida real. Evidencia: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-diagnostico-reporte-20261114.json`. El corte79/79 de UI permanece histórico; esta corrección solo cambia el auxiliar. Avance43/48;3.27 sigue abierta. Otra ventana de hasta5 SELECT exige instrucción expresa; se conserva1 intento previo (máximo6 acumulado), sin HTTP ni efectos. Los cortes siguientes conservan su alcance histórico.


## Flujo final tras creación sin confirmación — corte2026-11-14

Video194059: lote39/plan57, carpeta60 y archivo86. La recepción se muestra confirmada, pero la creación deja la carpeta CREANDO y el lote EN_CURSO durante las consultas de la grabación. La columna Expediente pendiente **no demuestra ausencia del expediente real**; no se conoce su resultado sin diagnóstico.

Corrección local3.24/3.25/3.26/3.28: contexto de operación conservado, aviso fijo con referencias, recurso bloqueante priorizado y **Revisar bloqueo** como acción principal local tras una consulta sin confirmación. Actualizar no reanuda; CREANDO no permite reconciliar por timeout. Una verificación válida sin resolución mantiene el bloqueo con explicación. Creación/índices/almacenamiento y fin solo por confirmaciones, sin encadenamiento. **79/79 Node locales**, incluidos2 wrappers Framework/DDL; compilación0 errores/1 advertencia. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-flujo-final-20261114.json.

**3.27 pendiente**: diagnóstico preparado de5 SELECT,0 HTTP,0 efectos; requiere autorización y credenciales efímeras. No ejecutado ni recuperado el lote39; causa interna todavía no comprobada. Avance**43/48**; pendientes3.27,5.2,6.4,6.5,6.6. La UI local aprobada no concede aceptación integral ni cierra este resultado real. Los cortes siguientes permanecen históricos.


## Fidelidad a la maqueta — corte vigente2026-11-13

Corrección solicitada tras2642e604: la reorganización anterior no igualaba el aspecto ni mantenía accesos de consulta dentro de todas las etapas. Tareas3.19–3.23 completadas: barra Consultar importación / Consultar último lote y pie siempre visibles, diseño de controles trasladado de la maqueta, selección de recurso sin efectos y consulta secundaria con preparación conservada. Un idCliente coincidente de otro lote no habilita Files del inventario original. Las operaciones mantienen sus guardas y validaciones.

**116 propiedades visuales contrastadas** y capturas390/768/1280px con CSS real, Bootstrap y servicios sintéticos; **71/71 pruebas Node**, incluidos2 wrappers Framework/DDL (no se suman casos internos), compilación Debug incremental0 errores/1 advertencia. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-fidelidad-maqueta-20261113.json. Avance**39/43**, con5.2,6.4,6.5,6.6 abiertas. No hay nueva sesión IIS autenticada ni aceptación integral. El marco modal y los controles operativos adicionales se conservan; la maqueta sigue siendo una página independiente con datos de ejemplo. Los cortes siguientes son históricos.


## Asistente guiado — corte vigente2026-11-13

Tareas3.10–3.18 completadas: **Seleccionar → Revisar → Seguimiento** sobre el asistente real. Perfil/carpetas juntos; una acción existente destacada con recurso, sin duplicación ni fases automáticas. Configuración/filtros desplegables con borradores y foco; consultas por referencia/último lote/plan pendiente, páginas independientes y CSV conservados. Actualizar estado muestra fecha/contador al completar lecturas válidas; envío muestra cantidad elegible de la página y archivo actual/total de cola, diferenciando transporte, recepción y almacenamiento. C01/C02/C03 de la maqueta corregidas; auditoría anterior y sus límites se conservan como antecedentes.

**Validación local:68/68 Node**, incluidos2 wrappers Framework/DDL cuyos casos internos no se suman; Chromium con servicios sintéticos y compilación Debug incremental0 errores/1 advertencia. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-asistente-guiado-20261113.json. Avance**34/38**; siguen abiertas5.2,6.4,6.5 y6.6. Sin ejecución IIS autenticada nueva ni aceptación integral. Los cortes siguientes mantienen su SHA, conteos y ámbito histórico.


## Recepción previa a creación — tarea3.9,2026-11-13

DOC120-REC01: el video de Egresos reproduce creación ofrecida con PDF pendiente en lote31/plan49; cinco SELECT de diagnóstico confirman ausencia de intentos. La consulta proyecta disponibilidad calculada con la barrera existente sobre el ámbito completo EXPEDIENTE/LOTE/PARCIAL, independiente de filtros o páginas. Una lectura de plan y dos SELECT agrupados de metadatos por página; sin reservas ni efectos. El cliente exige true estricto, explica el requisito y muestra Enviar pendientes antes de filtros. La operación conserva su validación inmediata de autoridad, revisión y staging.

**64/64 pruebas Node locales**, incluidos2 wrappers Framework/DDL sin sumar sus casos internos:2 focales de recepción,5 barra,28 cliente,11 navegación,12 editor y4 degradación. Chromium y SQL/HTTP sintéticos, también390px; compilación del proyecto local con0 errores. [Huellas y resultados](doc120-recepcion-creacion-20261113.json). Avance25/29; abiertas5.2,6.4,6.5 y6.6. Sin nueva sesión IIS autenticada ni repetición de recepción/creación/almacenamiento del lote31; aceptación integral pendiente. Los cortes siguientes son históricos; sus huellas anteriores de consulta/UI/arnés no describen esta corrección.

## Barra superior adaptable — implementación3.8,2026-11-13

DOC120-UI08: barra existente con cabeceras indivisibles y búsqueda flexible; colapso Bootstrap bajo1200px y panel estrecho con desplazamiento interno. Se conservan controles,36 manejadores y funciones de enrutamiento originales. Un observador de altura llama a auto_zise sin cambiar su cálculo ni la lógica de negocio; recursos versionados e incluidos en el proyecto.

**60/60 pruebas locales**:5 barra,28 cliente,11 navegación,12 editor y4 degradación. La misma comprobación funcional pasa sobre el HTML original y el nuevo (36 rutas en escritorio/móvil, clic/Enter/restauración); el original falla alineación. Cobertura de10 combinaciones de ancho/texto, foco, altura, Importación y postback sintético. [Huellas y resultados](doc120-barra-20261113.json). Avance24/28; abiertas5.2,6.4,6.5 y6.6. Sin servicios reales, SELECT ni efectos de negocio; no sustituye QA IIS autenticada. Los cortes siguientes permanecen históricos.

## Menú de Importación — corte posterior2026-11-13

DOC120-UI07: cabecera y opciones con iconos; textos del mismo color que Archivos y cabecera con icono azul. Las etiquetas dinámicas de consulta/lote mantienen sus iconos al sincronizar y recargar. Se conservan las opciones según permisos y el indicador de carga sin desplazamiento. Recursos versionados doc120-menu-20261113-2.

**55/55 pruebas locales**:28 cliente,11 navegación,12 editor de perfiles y4 degradación. Dos casos nuevos comparan colores calculados contra Archivos y verifican iconos, permisos y referencias. CSS Bootstrap/de página y fuente de iconos locales; respuestas sintéticas, sin autenticación IIS ni operaciones reales. [Huellas y resultados](doc120-menu-20261113.json). Avance23/27; las cuatro tareas abiertas permanecen. Los siguientes cortes conservan sus versiones históricas.

## Corrección de apertura — 2026-11-13

DOC120-UI06 (D-04/D-10, RQ-04/RQ-10), solicitada con «corrige» tras revisar el video: la carga inicial usaba el aviso amarillo y desplazaba el contenido41px. Ahora muestra un indicador neutro en el menú y conserva el mensaje accesible sin ocupar una franja. Errores, indisponibilidad y confirmación sin acuse conservan sus avisos. CSS/JS usan versión doc120-apertura-20261113-1.

Regresión local nueva: **41/41** (28 cliente,9 navegación,4 degradación), con3 casos nuevos de apertura demorada autorizada/denegada y verificación con aviso conservado. Respuestas sintéticas interceptadas en Chromium; ninguna autenticación, HTTP de aplicación, SELECT ni carga real. [Evidencia y huellas](doc120-apertura-20261113.json). Avance22/26; abiertas5.2,6.4,6.5 y6.6.

El manifiesto del2026-11-12 permanece histórico: sus15/15 huellas y diff de producto vacío no describen esta corrección. La vigencia UI se reevalúa con las huellas nuevas; backend, plugin contextual y carga individual no cambian. La prueba local actual no renueva SEG01/02 ni acredita una sesión IIS.

## Corte histórico de consolidación — 2026-11-12

Estado: **consolidación y regresión local; aceptación integral sin completar**. Base funcional8c0773d4, inspección 7310658894305bb203a489f2fb7b9077767dbe2c. [Matriz única](matriz-verificacion.md) y [manifiesto de huellas/primarios](doc120-consolidacion-20261112.json). Ningún resultado real anterior se atribuye a esta versión como corrida nueva.

## Cortes históricos consumidos

| Corte | Caso y fuente | Resultado y límite conservados |
| --- | --- | --- |
| 2026-11-01/02 | [doc111-matriz-final-20261102.md](doc111-matriz-final-20261102.md) | REC completos/parciales, degradación de una tabla y carga individual; sin SII ni limpieza reales. |
| 2026-11-03 | [doc113-verificacion-final-20261103.json](doc113-verificacion-final-20261103.json) | UI01–03 histórica anterior a la reorganización; no sustituye UI definitiva. |
| 2026-11-03 | [doc114-final-manifest-20261103.json](doc114-final-manifest-20261103.json) | PLAN01–03 y restauración; conteos del corte original. |
| 2026-11-04 | [doc115-verificacion-final-20261104.json](doc115-verificacion-final-20261104.json) | RECIB01–03/REPO07A;93Node/2Framework originales, no corrida DOC120. |
| 2026-11-05 | [doc116-entrega-20261105.json](doc116-entrega-20261105.json) | EXP01/02 cliente/02 interno/03 con sus primarios; incertidumbre41 conservada. |
| 2026-11-05/06 | [doc117-matriz-almacenamiento.md](doc117-matriz-almacenamiento.md) | ALM01/02/03C con cortes y laboratorios propios; incertidumbre53 y fallos previos conservados. |
| 2026-11-10 | [doc118-resultados-integracion.md](doc118-resultados-integracion.md) | Archivo administrativo anterior y cierre técnico posterior separados. |
| 2026-11-10 | [doc118-integracion-completa-20261110.json](doc118-integracion-completa-20261110.json) | COORD02/03 y38/38; laboratorio/continuación focal, sin reescribir motor detenido. |
| 2026-11-12 | [doc119-seg01-aceptado-20261112.json](doc119-seg01-aceptado-20261112.json) | SEG01 PASS de la corrida ccb25ec9; STOPPED global sigue por otro segmento. |
| 2026-11-12 | [doc119-recuperacion-verificada-20261112.json](doc119-recuperacion-verificada-20261112.json) | SEG02 real con límites281HTTP/27SELECT/1sesión; sin inserción nueva ni raíces físicas homónimas. |
| 2026-11-12 | [doc119-aceptacion-cobertura-mixta-propuesta-20261112.md](doc119-aceptacion-cobertura-mixta-propuesta-20261112.md) | Aceptación mixta explícita de DOC119, sin modificar runtime PASS_READONLY_WITH_LIMITS. |
| 2026-11-12 | [doc119-ui-verificacion-20261112.md](doc119-ui-verificacion-20261112.md) | UI definitiva local99/99, contratos33/33 y build0errores; sin nueva UI IIS autenticada. |
| 2026-11-12 | [doc119-archivo-20261112.md](doc119-archivo-20261112.md) | Archivo/spec sync previos a merge PR130; main8c0773d4 confirmado en el flujo. |

## Comprobaciones nuevas de DOC120

- Plan local aprobado por «continua» sobre73106588; opsxj:refine -- DOC-120 --sync PASS15/15. No aprobación de efectos reales.
- Comparación15/15 huellas DOC119 y diff de componentes/pruebas UI vacío: se reutiliza evidencia local vigente, no se reejecuta99/33 por cambiar de ticket.
- Regresión focal inicial:5/5 Node (2 wrappers Framework/DDL y3 políticas de degradación), archivo excluido .tmp/doc120-backend-degradation-local.log. Framework usa disco temporal y dobles SQL/HTTP; ninguna conexión real.
- SUP01: caso nuevo Chromium/cliente real local reproduce Timeout del botón oculto antes del arreglo (.tmp/doc120-degradation-before-fix.log,1fallo). Después del arreglo,4/4 políticas/UI (.tmp/doc120-degradation-after-fix.log). Tres políticas se repiten; no sumar5+4 como9 casos distintos.
- Solo cambia el auxiliar E2E y su prueba; producto, esquema, configuración, permisos y SII sin delta desde8c0773d4. No se ejecuta el runner de degradación real, puente administrativo o DDL.
- Fuentes, logs y resultados primarios enlazados quedan ligados por SHA-256 en doc120-consolidacion-20261112.json. Los JSON históricos no se sobrescriben.

## Contrato de medición INT-03 — NO_EJECUTADO

El solicitante y operador deben definir y autorizar los siguientes campos antes de medir. Esta tabla concreta la información que falta; no elige números por inferencia ni convierte límites técnicos del transporte en capacidad demostrada.

| Campo | Estado / decisión necesaria |
| --- | --- |
| Objetivo | NO_DEFINIDO: volumen normal/pico y resultado que se quiere acreditar. |
| Corpus | NO_DEFINIDO: raíces/archivos, tamaños y distribución; conservar PDF/rechazo TIFF. |
| Navegadores | NO_DEFINIDO: producto/versión; no inferir otros navegadores desde Chromium local. |
| Concurrencia | NO_DEFINIDA: sesiones/importadores simultáneos y fases incluidas. |
| Ambiente y origen | NO_AUTORIZADO para INT03: URL/aplicación, servidor/BD y versión/binarios que se observarán. Localhost de pruebas anteriores es antecedente. |
| Cuentas y recursos | NO_SELECCIONADOS: actor/ámbito/perfiles/destinos/recursos propios y ajenos; secretos solo en consola de ejecución autorizada. |
| Disco temporal/libre | SIN_MEDICION_ACTUAL: definir corpus esperado, reserva libre y consumo permitido; no usar57archivos/24.371bytes históricos como actual. |
| Tiempo y memoria | SIN_UMBRALES: definir duración máxima, percentil/medida por fase, pico de memoria y tolerancia. |
| Topes | SIN_APROBAR: HTTP/SELECT/sesiones/bytes/recibidos/creaciones/documentos; límite por ventana y acumulado. |
| Efectos | NO_AUTORIZADOS: distinguir lectura, preparación, confirmación, recepción, creación, almacenamiento y preservación; sin limpieza ni replay incierto. |
| Parada y recuperación | Acordar códigos/umbrales de detención, preservación, revisión de resultados inciertos y restauración si se autoriza variante. |
| Instrumento | Reutilizar runners, sesiones/SELECT y herramientas existentes; seleccionar el modo después de fijar objetivo y plan, sin plataforma nueva. |
| Decisión solicitada | Definir prueba acotada o mantener NO_EJECUTADO y revisar ese límite al cierre; sin respuesta no hay autorización ni aceptación. |

No se infiere capacidad máxima de MaximoBytesLote=ULong.MaxValue, de un lote de27PDF, un máximo de página o un solapamiento de laboratorio. LimitesImportacionCarpetas deriva restricciones de JSON/transporte/sesión, sin cupo acumulado de negocio nuevo. Sin contrato aprobado no hay medición ni cierre integral por inferencia.

## Estado de aceptación

Los límites de SII real, limpieza real y UI final autenticada se detallan con responsables/impacto en la matriz. No hay carga ni ventana real DOC120 autorizada en este corte. La autorización de capacidad y la revisión final de límites siguen abiertas. Los resultados locales y el avance documental no equivalen a Jira Listo, archivo ni aprobación de salida operativa.

## Mensajes de las cinco fases — implementación local2026-11-14

Preparación, confirmación, recepción, creación/reanudación e índices/almacenamiento muestran causas verificadas y acciones vigentes, con contrato público conservado. El catálogo cerrado cubre148 códigos, incluidos los118 auditados. El mapa clasifica577 textos y684 sitios, conservando etiquetas y excluyendo administración/legacy/V2 sin ampliación. Un acuse confirmado no se pierde por una consulta o proyección fallida; la cola conserva confirmados, archivo actual y no intentados. Sin replay ni cambios de autoridad, reservas, política de nombres o escritores compartidos. Avisos accesibles con Tab en390/768/1280px. **119/119 pruebas locales únicas**, incluidos19 nuevos casos Node y wrappers Framework/DDL; casos internos no sumados. Compilación0 errores. Avance**82/86**,36 escenarios; siguen abiertas5.2/6.4/6.5/6.6. Fuentes, comandos, logs y huellas en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-mensajes-20261114.json`; mapa compañero `doc120-mensajes-mapa-20261114.json`. No hubo nuevas operaciones reales ni comprobación del runtime o destino de trazas de IIS.

La nueva evidencia es LOCAL_DOC120; los resultados históricos conservan sus fuentes, SHA y límites. No acredita locking/commit MySQL real, sesión autenticada, capacidad máxima ni aceptación operativa.

## Claridad del código existente — corrección2026-11-14

Captura error.png y confirmación del usuario de que el mensaje no es claro:3.67/3.68 verificadas localmente. El aviso indica que el código existe en la misma empresa y área y que el perfil bloquea crear otro igual. Presenta el código ya disponible de una única raíz/valor del plan y la acción de revisar datos/regla y preparar otra revisión. Campo legible, sin prefijo técnico duplicado; ningún ID/nombre del expediente coincidente se consulta ni publica. Las consultas, política y contrato no cambian.56/56 pruebas locales únicas incluyen7 nuevas variantes de dato ausente/ajeno/ambiguo/diferido y HTML como texto; Framework/DDL incluidos sin sumar sus casos internos ni los119 del corte anterior. Compilación0 errores; fuentes, log y captura en Validacion/doc120-coincidencia-mensaje-20261114.json.84/88 tareas,17 S/71 M,36 escenarios. Sin operaciones reales; abiertas5.2/6.4/6.5/6.6 y aceptación integral.
