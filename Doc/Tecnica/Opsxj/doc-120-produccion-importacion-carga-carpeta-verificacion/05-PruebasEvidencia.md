# DOC-120 — PruebasEvidencia

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


## Corte de recepción — DOC120-REC01,2026-11-13

Tarea3.9: disponibilidad de creación por consulta, reutilizando la barrera de recepción y el ámbito completo, con dos SELECT de metadatos agrupados por página y lectura del plan. Se añade el booleano público recepcionListaParaCrear (false por defecto), sin endpoint ni esquema nuevo; pista interna excluida de JSON persistido. Cliente exige true, explica la recepción y acerca el envío. Sin cambios de creación/almacenamiento/SII/perfil Egresos ni efectos sobre lote31. Regresión local64/64 Node (incluye2 wrappers Framework/DDL) y compilación0 errores. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-recepcion-creacion-20261113.json. Avance25/29; cuatro tareas de autorización/aceptación/gobierno/entrega abiertas. Sin nueva prueba IIS autenticada; los apartados siguientes son cortes históricos y sus afirmaciones sin delta/DTO no describen esta corrección.

- Ticket: DOC-120
- Cambio OpenSpec: doc-120-produccion-importacion-carga-carpeta-verificacion
- Clasificacion: cross_cutting

## Barra superior — implementación3.8,2026-11-13

Avance24/28. DOC120-UI08: barra y búsqueda adaptables con acciones/IDs originales, sin cambios de rutas de negocio; baseline funcional36 acciones y regresión60/60 locales con dobles de negocio/postback. Altura ajustada al desplegar, foco y montaje dinámico conservados. CSS/JS aislados, versionados e incluidos como Content; prueba de10 combinaciones de ancho/texto y desplegables largos. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-barra-20261113.json. Sin IIS autenticado ni efectos reales; siguen cuatro tareas abiertas. Los cortes siguientes son antecedentes.

## Menú — corte posterior2026-11-13

DOC120-UI07: iconos de cabecera/opciones y colores coherentes con Archivos, preservando permisos, referencias y carga sin desplazamiento.55/55 pruebas locales con CSS/fuente reales y servicios sintéticos; sin IIS autenticado. Avance23/27, sin aceptación integral. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-menu-20261113.json. Los cortes siguientes son antecedentes.

## Corte vigente2026-11-13

Avance22/26. Corrección DOC120-UI06: carga inicial con indicador neutro accesible en el menú, sin franja amarilla ni desplazamiento41px; errores y confirmación sin acuse siguen visibles. Cambian JS/CSS y las URLs ASPX de esos recursos. Regresión focal41/41 en Chromium/cliente local con respuestas sintéticas (28 cliente,9 navegación,4 degradación); sin IIS autenticado ni cambios de negocio. Huellas nuevas: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-apertura-20261113.json. Las afirmaciones15/15/sin delta siguientes corresponden al corte histórico2026-11-12. Se conservan las cuatro tareas abiertas y los límites de capacidad/aceptación.

## Antecedente2026-11-12

## Evidencia requerida

Corte: consolidación local21/25, sin aceptación integral. Índices: Validacion/matriz-verificacion.md y resultados.md. Manifiesto: doc120-consolidacion-20261112.json, con32 casos,25 fuentes originales,11 componentes,15/15 huellas UI coincidentes,114 entradas del arnés y huellas de logs. La inspección inicial doc120-inspeccion-refinamiento-20261112.json se conserva como corte anterior.

Plan local aprobado sobre73106588 y opsxj:refine -- DOC-120 --sync PASS15/15. El mapa activo tiene25 tareas (24 originales más4.4 por reproducción del auxiliar). UI/contratos99/33 de DOC119 se reutilizan por fuentes iguales; no son ejecuciones DOC120.

Regresión nueva:5/5 inicial (2 wrappers Framework/DDL y3 políticas de degradación); reproducción del caso nuevo1fallo por botón oculto antes del arreglo; después4/4 (3 políticas repetidas y1 Chromium nuevo). Son6 Node únicos, no9 ni los casos internos Framework sumados. Logs locales excluidos: .tmp/doc120-backend-degradation-local.log, doc120-degradation-before-fix.log y doc120-degradation-after-fix.log. No se repite Framework después del cambio Node porque sus entradas permanecen intactas.

Se resuelven contradicciones de cortes DOC116/117/118 y estado global/caso de SEG01; no se reescriben JSON primarios. El cierre técnico de un caso no borra incertidumbres41/53 ni convierte laboratorio en runtime ordinario.

## QA/E2E WebForms

Ninguna nueva autenticación, HTTP de aplicación, SELECT real, recepción, creación, almacenamiento, DDL, permiso, fallo real o limpieza en DOC120. La prueba Chromium devuelve503 sintético local. El fichero DLL comparado es un artefacto de disco, no prueba de ensamblado cargado en IIS.

Abiertas5.2 (autorización real),6.4 (aceptación explícita de límites),6.5 (gobierno final) y6.6 (entrega final).5.3 consta NO_EJECUTADO con motivo/responsable/impacto, no como medición aprobada. INT03 sin contrato/autorización no concede garantía de capacidad. SII/limpieza y UI definitiva autenticada conservan su límite; no hay archivo ni Jira Listo por inferencia.
