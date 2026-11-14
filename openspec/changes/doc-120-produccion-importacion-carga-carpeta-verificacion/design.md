<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## Context

Fuente completa: specs/produccion-importacion-carga-carpeta-verificacion/jira-context.md. Base funcional main 8c0773d4371aeb35543bf182c0abdd0319cababa, merge PR130 de DOC-119. Inspección sobre 60e02ae78841f94d08619ac9d53982d4318871c3, rama feature/DOC-120, carpeta principal. El proyecto declara .NET Framework4.6.1 (GestionDocumental-Docuarchi.net.vbproj:16), WebForms VB y JavaScript legacy. Esta revisión no cambia framework ni fuentes de producto.

## Goals / Non-Goals

**Goals**: matriz única con vigencia por componente; consumo correcto de EXP/ALM/COORD/SEG; regresión focal de la interfaz definitiva; resolución justificada INT-02 y preparación medible INT-03; cierre sin brechas críticas ocultas.

**Non-Goals**: rehacer seguimiento, repetir cada carga, nueva arquitectura/plataforma, SII o limpieza reales sin autorización, modificar almacenamiento compartido/esquema/formatos o activar jobs/cuotas/aislamiento XML adicional.

## PRECONDICIONES DE RUTAS

| Responsabilidad | Rutas/símbolos inspeccionados | Tratamiento |
| --- | --- | --- |
| Base y evidencias | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-entrega-20261105.json; doc117-matriz-almacenamiento.md; doc118-integracion-completa-20261110.json; doc119-ui-fuentes-20261112.json | Lectura y comparación, primarios inmutables |
| Índices nuevos | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/matriz-verificacion.md; Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/resultados.md | Ausentes: crear solo estos índices al ejecutar el plan; Markdown, sin Compile de proyecto |
| Consolidación verificable | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-consolidacion-20261112.json | Manifiesto de dependencias/huellas y decisiones de reutilización de los índices; datos saneados, sin Compile ni plataforma nueva |
| Inspección de refinamiento | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-inspeccion-refinamiento-20261112.json | Nuevo manifiesto saneado de fuentes/evidencias, sin credenciales ni efecto de aceptación real |
| Operación y autoridad | Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb:Consultar/ConsultarConfirmacion/ConsultarPagina/ExportarPagina; Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb:Resolver/ResolverDestino/AutorizarRecursos | Consultar y revisar; sin nuevos contratos ni reconciliación desde lectura |
| Coordinación | Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb; CoordinadorImportacionCarpetas.Valores.vb; CoordinadorImportacionCarpetas.Operaciones.vb; CoordinadorImportacionCarpetas.RecuperacionV2.vb | Conservar fases, revisiones y barreras; corregir solo defecto reproducido |
| Adaptadores | Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb; AdaptadorDocumentoImportacion.vb; ComposicionLimpiezaOriginalImportacion.vb:Crear | Únicos existentes; sin invocar escritor o limpieza por inspección |
| UI definitiva | Gestion/WebFormProducionDocumental.aspx/vb; js/gestion/WebFormProducionDocumental.js; js/gestion/ImportacionCarpetas/ImportacionCarpetas.js/Vista.js; js/jquery.contextMenu.js/css | Contrastar huellas DOC-119 y casos afectados; no otro asistente ni menú |
| Pruebas | tests/ImportacionCarpetasBehaviorTests.cs; tests/doc111-importacion-carpetas-client.test.cjs; tests/doc119-navigation.test.cjs; tests/doc119-context-menu.test.cjs; tests/doc88-produccion-upload-contract.test.cjs; tests/doc117-alm03-individual-ui.test.cjs | Reutilizar arnés y Node/Chromium instalados; fuentes de prueba fuera del proyecto productivo |
| E2E y degradación | tools/e2e/AGENT-RUNBOOK.md; tools/e2e/tests/doc111-degradation.test.cjs; tools/e2e/tests/support/doc119-seguimiento.cjs; tools/e2e/tests/support/doc113-ventana-importacion.cjs | Sin login/runner/puente alternativo; plan y autorización previos a ejecución real |
| Guía y dossier | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/11-prompt-seguimiento-operacion.md; Diseno/02-contratos-operacion.md; Doc/Tecnica/Opsxj/doc-120-produccion-importacion-carga-carpeta-verificacion/ | Vincular versión y límites; no duplicar plataforma o máquina de pruebas |

## Decisions

### D-01 — Base y vigencia por componente (RQ-01)

Fijar el merge de DOC-119 como base funcional. Cada caso conserva SHA/huellas de sus componentes, prueba, ambiente y resultado originales; la identidad de huellas permite reutilización local, no una ejecución real nueva. Un cambio invalida únicamente los casos dependientes; el mapa explicita esa dependencia.

Evidencia: Gestion/WebFormProducionDocumental.aspx; js/gestion/ImportacionCarpetas/ImportacionCarpetas.js; doc119-ui-fuentes-20261112.json.

Criterio y compatibilidad: Una fila con código distinto exige evaluar impacto; igualdad de fuentes no renueva autenticación, datos o runtime.

### D-02 — Matriz e índices únicos (RQ-02)

Crear matriz-verificacion.md y resultados.md en Validacion, ausentes en la base inspeccionada. La matriz relaciona requisito/caso, componente/símbolo, prueba, SHA/huella, ambiente, evidencia, resultado, vigencia y límites. Resultados mantiene cronología y enlaza fuentes primarias sin sobrescribirlas.

Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/; doc111-matriz-final-20261102.md.

Criterio y compatibilidad: Cada caso se clasifica REAL_HISTORICO, LOCAL_HISTORICO_VIGENTE, LOCAL_DOC120, ESTATICO o NO_EJECUTADO; el resultado primario permanece separado de esa clasificación.

### D-03 — Consumo de EXP/ALM/COORD/SEG (RQ-03)

Usar resultados efectivos y sus cortes finales. DOC-116 resolvió su ventana interna mediante variante de laboratorio; DOC-117 acredita ALM-01/02/03C con límites; DOC-118 distingue archivo administrativo de cierre técnico38/38; DOC-119 conserva SEG-01 real y SEG-02 con cobertura mixta aceptada. No sumar suites de cortes distintos ni reproducir efectos inciertos retenidos.

Evidencia: doc116-entrega-20261105.json; doc117-matriz-almacenamiento.md; doc118-integracion-completa-20261110.json; doc119-recuperacion-verificada-20261112.json.

Criterio y compatibilidad: Resolver contradicciones editoriales por fuente primaria, fecha y alcance; preservar intentos41/53, documentos e historial. Los observadores de laboratorio no se atribuyen al runtime ordinario.

### D-04 — Interfaz definitiva y regresión focal (RQ-04)

Corrección de fidelidad solicitada tras2642e604: tomar el HTML Prototipo/propuesta-importacion-guiada-doc120.html como referencia directa de estilos, distribución y controles, conservando datos/operaciones reales. Barra Consultar importación / Consultar último lote siempre visible, incluso durante scroll; último lote sin referencia aparece deshabilitado con explicación. Consulta secundaria preserva la preparación en memoria y permite volver; durante consulta solo la reselección explícita autoriza Files, nunca el inventario original por coincidencia de idCliente. Al volver a un lote confirmado se relee antes de habilitar operaciones. Cierre informa sobre la preparación original conservada. Cabecera contextual, pasos circulares, selección con filas, contadores, estados, filtros y configuración inferior, panel de acción por recurso y pie separado reproducen la maqueta. Ver acciones selecciona un recurso sin efectos y el botón de operación se conserva único; guards de accionesRecurso permanecen autoritativos. No se copian datos ficticios, controles de simulación, persistencia ni automatismos del HTML.

Ampliación autorizada2026-11-13: reorganizar el asistente existente en Seleccionar (perfil y carpetas juntos), Revisar y Seguimiento. Reutilizar modelo, eventos, selector, inventario, accionesRecurso y servicios actuales. Configuración y filtros se despliegan sin eliminar consultas, páginas independientes ni exportación. La siguiente acción se destaca con su recurso y las otras permanecen en sus filas; cada clic conserva una sola operación. Actualizar estado muestra fecha/contador solamente después de completar lecturas válidas. El envío expresa cantidad de archivos elegibles de la página, sin confundir bytes de transporte con documentos almacenados. Foco, borradores de filtros y paneles abiertos se conservan durante renderizados. CSS limitado al asistente; barra superior/búsqueda y backend sin cambios.

La maqueta corrige C01 (padre confirmado visible para índices/almacenamiento), C02 (CSV13 columnas/coma/escape) y C03 (página solicitada vacía sin ajuste automático; capacidad de página simulada explícita). Se mantienen sus límites de simulación: no acredita autenticación, persistencia real, políticas del servidor ni reconciliación real. La auditoría previa se conserva como corte histórico.

Mapear UI01–05 al menú superior/contextual y asistente resultantes de DOC-119. Cubrir seleccionado/pulsado, homónimos, selección inválida, autoridad denegada/revocada, referencia/último lote/confirmación sin acuse, recarga/reapertura/UpdatePanel, árbol, foco y viewport. Reutilizar fuentes vigentes y ejecutar solo casos afectados o brechas documentadas.

Evidencia: js/gestion/WebFormProducionDocumental.js:Doc119NivelPulsado/Doc111ActualizarRama; tests/doc119-navigation.test.cjs; tests/doc111-importacion-carpetas-client.test.cjs.

Criterio y compatibilidad: La UI final de DOC-119 es LOCAL con servicios sintéticos. El estado real SEG-02 conserva sus límites; no acredita inserción nueva ni raíces físicas homónimas observadas en IIS.

Ampliación solicitada por el usuario para3.8: alinear exclusivamente la barra superior existente y su búsqueda. Se conserva el DOM de acciones, IDs, onclick/onkeypress, permisos y controles WebForms. La distribución usa navbar-expand-xl y estilos aislados a #menu_var: cabeceras indivisibles, búsqueda flexible y colapso Bootstrap existente; en pantallas estrechas el panel tiene altura acotada y desplazamiento interno, conservando opciones/submenús y sus eventos. Un observador de altura llama al auto_zise existente sin modificar su lógica, peticiones o selección. Los dos recursos se incluyen como Content del proyecto. Se compara el inventario y resultado de36 manejadores originales con dobles de fronteras de negocio, más búsqueda/Enter/restauración, foco, montaje dinámico y postback sintético. La prueba local no certifica una sesión IIS autenticada.

### D-05 — Integridad, incertidumbre y consumidores (RQ-05)

Video194059, corrección posterior a12942b6: el recurso activo que bloquea el lote se muestra antes que un archivo sin acción. Conservar fase/recurso y códigos públicos de la última solicitud no confirmada; aceptar operacionId solo con formato público32hex, sin mensajes arbitrarios del servidor. Presentar CREANDO/EN_CURSO como ejecución sin resultado confirmado y mantener la reclamación; no equipararla a INCIERTO ni habilitar reconciliación por timeout. Una consulta no demuestra que el efecto terminó ni reemplaza su evidencia. Aviso fuera del scroll o dentro del panel siempre visible, con próximo paso permitido y referencia de diagnóstico. Creación confirmada conduce a la acción de índices/almacenamiento del archivo, sin encadenar peticiones; final solo por estado/identidades del servidor. El diagnóstico real del lote39 exige ventana separada de hasta5 SELECT, sin HTTP ni efectos; resultados/rutas locales excluidos y secretos efímeros. Causa de fallo del checkpoint no se infiere del video.

Corrección autorizada DOC120-REC01, tarea3.9: el video recorrido-error muestra Crear expediente antes de recibir el PDF. La consulta de raíces proyecta `recepcionListaParaCrear`, calculada con EvaluarBarreraCreacion y los metadatos completos del ámbito confirmado, sin filtros de archivos ni dependencia de su página visible. Las raíces pendientes de una página se agrupan en dos SELECT de metadatos más una lectura del plan, dentro de la transacción de consulta; EXPEDIENTE/PARCIAL se evalúan por identidad de raíz y LOTE sobre todo el lote. La pista interna se excluye del JSON persistido y por defecto es falsa. No verifica disco ni reserva intentos: la operación mantiene su comprobación inmediata de autoridad, revisión, barrera y staging. El cliente exige true estricto, muestra la instrucción de recepción y sitúa Enviar pendientes antes de filtros; mantiene incertidumbre, detención y acciones explícitas. Sin cambios de esquema, perfil Egresos, SII, almacenamiento ni efectos sobre el lote31.

Relacionar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, valores diferidos y fecha estable, revocación, stop e incertidumbre con casos del arnés existente. Comprobar contrato del plugin en sus26 registros/22archivos y carga individual pertinente. Un callback sintético no demuestra la acción de negocio real de cada módulo.

Evidencia: Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb; CoordinadorImportacionCarpetas.Valores.vb; tests/ImportacionCarpetasBehaviorTests.cs; tests/doc119-context-menu.test.cjs.

Criterio y compatibilidad: Consultar/exportar no procesa ni reconcilia; RECIBIDO no equivale a DOCUMENTO_ALMACENADO. Stop no revierte efectos y una salida INCIERTO no autoriza replay.

### D-06 — INT-02 degradación acotada (RQ-06)

Conservar el caso real histórico de ausencia temporal de ra_integracion_evento con restauración y carga individual, sin extrapolar a cada incompatibilidad. Contrastar los verificadores/esquema y el guard de indisponibilidad actuales. Exigir variante nueva solo si un requisito concreto y una diferencia reproducida la hacen necesaria; registrar NO_NECESARIA con justificación en los demás casos.

Evidencia: Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/; tools/e2e/tests/doc111-degradation.test.cjs; doc111-degradacion-individual-pass-20261102.md.

Criterio y compatibilidad: DDL previo manual se conserva. No reinstalar migraciones ni degradar la base para refrescar un antecedente; cualquier DDL/fallo real necesita aprobación específica y restauración revisada.

Corrección focal DOC120-SUP01: el auxiliar tools/e2e/tests/support/doc111-degradation.cjs pulsaba el botón dentro del menú cerrado. Reproducido en Chromium con cliente actual y respuesta503 sintética; ahora abre el menú antes del click. La prueba se añade al archivo de políticas existente; tarea4.4 D-06/RQ-06. No se ejecuta DDL ni una nueva degradación real.

### D-07 — INT-03 capacidad con presupuesto definido (RQ-07)

Antes de medir acordar objetivo, cantidad/distribución/tamaño de archivos y raíces, navegadores/versiones, concurrencia, duración, ambiente, recursos, disco temporal/libre y umbrales de tiempo/memoria. Incluir topes HTTP/SELECT/sesiones/bytes/efectos y criterio de parada. Sin esos acuerdos, INT-03 queda NO_EJECUTADO por falta de definición/autorización.

Evidencia: Modelo/ProduccionDocumental/ImportacionCarpetas/; tools/e2e/scripts/run-doc111-importacion-carpetas.cjs; doc119-datos-aceptacion-propuesta-20261112.md.

Criterio y compatibilidad: No inferir capacidad máxima de lotes pequeños, máximo de página, pruebas de contratos o solapamientos de laboratorio. Responsable: solicitante y operador del ambiente; efecto: sin garantía de capacidad ni cierre integral por inferencia.

### D-08 — Autorización separada y herramientas existentes (RQ-08)

La continuación autoriza refinamiento/inspección y preparación local. Antes de ejecutar cada ventana real exigir ambiente, cuentas, recursos, operaciones, presupuestos y recuperación revisados. Reutilizar autenticación, runners y queryRows existentes; controles RA/DA exclusivamente SELECT y secretos efímeros. Autorizaciones DOC110–119 no se heredan.

Evidencia: tools/e2e/AGENT-RUNBOOK.md; tools/e2e/tests/support/doc119-seguimiento.cjs; tools/e2e/tests/support/doc113-ventana-importacion.cjs.

Criterio y compatibilidad: Lectura, recepción, creación, almacenamiento, fallos, DDL y permisos son alcances distintos. Un STOPPED no es PASS; no reintentar automáticamente ni extraer credenciales de archivos/procesos.

Diagnóstico de prerrequisitos posterior a933961f9: reutilizar ComandoCreacionExpedienteImportacion y PerfilImportacionCatalogAdapter con un delegado SELECT a queryRows. El proceso auxiliar no compone escritores ni repositorios mutantes. Metadatos, tokens y valores se transmiten solo por IPC y se eliminan al cerrar; evidencia limitada a etapa/código/conteos/huellas. Una copia local proyecta exclusivamente la situación previa a la detención y EsNuevaReserva para validar el comando; se declara contrafactual, nunca autoridad, lease o permiso para repetir. La sesión HTTP y la transacción real de inicio no se prueban mediante este auxiliar. Ventana nueva de hasta30 SELECT en RA/DA, cero HTTP/efectos, requiere autorización propia; los6 intentos anteriores se conservan. Aun con catálogos compatibles, no se libera el intento96 ni se declara recuperado. Si no se reproduce una causa concreta se conserva esa incertidumbre.

### D-09 — Fuentes y decisiones protegidas (RQ-09)

Conservar SII/Integracionccv, AlmacenamientoDocumentoProduccionDocumental, Web.config, esquema, semántica de recepción/efectos y PDF/rechazo TIFF. Mantener decisión expresa SEG-03 de operar sin limpieza y datos retenidos. No añadir jobs, cuotas, formatos, endpoints o aislamiento XML adicional.

Evidencia: Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb:Crear; doc119-seg03-propuesta-sin-limpieza-20261112.md.

Criterio y compatibilidad: SII real y limpieza real siguen NO_EJECUTADO. La medición57archivos/24.371bytes es histórica previa a nuevos datos; no representa disco actual ni capacidad disponible.

### D-10 — Defectos reproducidos y criterio de cierre (RQ-10)

Modificar producto solo ante una brecha reproducible vinculada a requisito, causa y prueba de regresión. Registrar cada no ejecutado con motivo, responsable e impacto operativo. Autoridad/destino/integridad/recuperación con fallo crítico bloquean salida. Límites necesitan aceptación expresa; archivo/CI/checklists no sustituyen aceptación integral.

Evidencia: Validacion/matriz-verificacion.md; Validacion/resultados.md; Doc/Tecnica/Opsxj/doc-120-produccion-importacion-carga-carpeta-verificacion/; tools/opsxj/scripts/lib/legacyGovernanceService.js.

Criterio y compatibilidad: Una diferencia de huella por sí sola no prueba defecto ni obliga a repetir efectos. La revisión final identifica SHA/ambiente y resolución; publicación, archivo, merge y cierre se ejecutan posteriormente con sus comprobaciones.

## Risks / Trade-offs

- El cierre de un ticket no renueva todas sus pruebas. La matriz conserva código/runtime/datos de origen.
- Los callbacks sintéticos y el solapamiento de laboratorio no demuestran otros flujos reales ni capacidad máxima.
- Huella distinta requiere análisis; repetir una operación INCIERTO podría duplicar documentos.
- Sin contrato INT-03 no hay medición ni umbral que aprobar. Cada no ejecutado tiene responsable e impacto sobre operación.
- Contadores y páginas de seguimiento no representan snapshot global ni rendimiento del lote completo.

## Migration Plan

Sin migración de datos o despliegue. Primero revisar artefactos y aplicar mapa atómico; después crear índices/relaciones de vigencia, seleccionar regresión por impacto y preparar ventanas reales solo si son necesarias. Producto cambia únicamente tras reproducción. Registrar SHA/ambiente y revisar límites antes de archivo/merge/cierre. Reversa documental restaura índices/guía sin borrar planes, lotes, originales o historial ni repetir efectos.

## Decisiones operativas externas

INT-03 requiere del solicitante y operador: objetivo/umbrales, corpus, navegadores, concurrencia, disco, ambiente/cuentas/recursos, presupuestos y autorización específica. No se fijan cifras por inferencia. La aceptación de límites DOC-119 se conserva como antecedente; no equivale a aceptación integral de DOC-120. La continuación posterior a73106588 aprueba consolidación documental y comprobaciones locales, sin nuevas ventanas reales.

## Ajuste focal autorizado2026-11-13

Solicitud «corrige» sobre el aviso amarillo de apertura del video: D-04/D-10 y RQ-04/RQ-10 amparan la corrección de producto reproducida41px y su regresión. Tarea3.6 y Validacion/doc120-apertura-20261113.json fijan fuentes y41/41 casos locales. Se conserva el aviso de confirmación sin acuse y errores reales. La frase inicial sin cambios de producto corresponde a la inspección original; este corte sí modifica JS/CSS y versiones de recursos ASPX. No cambia contratos, backend, fuentes protegidas ni autorización de pruebas reales.

Refinamiento tras704fde28: rechazo de configuración en COMMAND anterior a catálogos. El modo `--command-only` limita una nueva ventana autorizada a1 SELECT RA y termina sin catálogos;7 intentos previos conservados. Clasificar el guard después del rechazo real mediante validadores/huella existentes y comparaciones diagnósticas del mismo comando; informar exclusivamente campos/códigos cerrados y conteos/booleanos. No cambiar producto, reinterpretar el rechazo como permiso ni afirmar causa histórica por inferencia.
