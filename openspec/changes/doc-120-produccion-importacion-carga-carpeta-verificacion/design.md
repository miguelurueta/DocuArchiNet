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

Mapear UI01–05 al menú superior/contextual y asistente resultantes de DOC-119. Cubrir seleccionado/pulsado, homónimos, selección inválida, autoridad denegada/revocada, referencia/último lote/confirmación sin acuse, recarga/reapertura/UpdatePanel, árbol, foco y viewport. Reutilizar fuentes vigentes y ejecutar solo casos afectados o brechas documentadas.

Evidencia: js/gestion/WebFormProducionDocumental.js:Doc119NivelPulsado/Doc111ActualizarRama; tests/doc119-navigation.test.cjs; tests/doc111-importacion-carpetas-client.test.cjs.

Criterio y compatibilidad: La UI final de DOC-119 es LOCAL con servicios sintéticos. El estado real SEG-02 conserva sus límites; no acredita inserción nueva ni raíces físicas homónimas observadas en IIS.

### D-05 — Integridad, incertidumbre y consumidores (RQ-05)

Relacionar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, valores diferidos y fecha estable, revocación, stop e incertidumbre con casos del arnés existente. Comprobar contrato del plugin en sus26 registros/22archivos y carga individual pertinente. Un callback sintético no demuestra la acción de negocio real de cada módulo.

Evidencia: Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb; CoordinadorImportacionCarpetas.Valores.vb; tests/ImportacionCarpetasBehaviorTests.cs; tests/doc119-context-menu.test.cjs.

Criterio y compatibilidad: Consultar/exportar no procesa ni reconcilia; RECIBIDO no equivale a DOCUMENTO_ALMACENADO. Stop no revierte efectos y una salida INCIERTO no autoriza replay.

### D-06 — INT-02 degradación acotada (RQ-06)

Conservar el caso real histórico de ausencia temporal de ra_integracion_evento con restauración y carga individual, sin extrapolar a cada incompatibilidad. Contrastar los verificadores/esquema y el guard de indisponibilidad actuales. Exigir variante nueva solo si un requisito concreto y una diferencia reproducida la hacen necesaria; registrar NO_NECESARIA con justificación en los demás casos.

Evidencia: Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/; tools/e2e/tests/doc111-degradation.test.cjs; doc111-degradacion-individual-pass-20261102.md.

Criterio y compatibilidad: DDL previo manual se conserva. No reinstalar migraciones ni degradar la base para refrescar un antecedente; cualquier DDL/fallo real necesita aprobación específica y restauración revisada.

### D-07 — INT-03 capacidad con presupuesto definido (RQ-07)

Antes de medir acordar objetivo, cantidad/distribución/tamaño de archivos y raíces, navegadores/versiones, concurrencia, duración, ambiente, recursos, disco temporal/libre y umbrales de tiempo/memoria. Incluir topes HTTP/SELECT/sesiones/bytes/efectos y criterio de parada. Sin esos acuerdos, INT-03 queda NO_EJECUTADO por falta de definición/autorización.

Evidencia: Modelo/ProduccionDocumental/ImportacionCarpetas/; tools/e2e/scripts/run-doc111-importacion-carpetas.cjs; doc119-datos-aceptacion-propuesta-20261112.md.

Criterio y compatibilidad: No inferir capacidad máxima de lotes pequeños, máximo de página, pruebas de contratos o solapamientos de laboratorio. Responsable: solicitante y operador del ambiente; efecto: sin garantía de capacidad ni cierre integral por inferencia.

### D-08 — Autorización separada y herramientas existentes (RQ-08)

La continuación autoriza refinamiento/inspección y preparación local. Antes de ejecutar cada ventana real exigir ambiente, cuentas, recursos, operaciones, presupuestos y recuperación revisados. Reutilizar autenticación, runners y queryRows existentes; controles RA/DA exclusivamente SELECT y secretos efímeros. Autorizaciones DOC110–119 no se heredan.

Evidencia: tools/e2e/AGENT-RUNBOOK.md; tools/e2e/tests/support/doc119-seguimiento.cjs; tools/e2e/tests/support/doc113-ventana-importacion.cjs.

Criterio y compatibilidad: Lectura, recepción, creación, almacenamiento, fallos, DDL y permisos son alcances distintos. Un STOPPED no es PASS; no reintentar automáticamente ni extraer credenciales de archivos/procesos.

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

INT-03 requiere del solicitante y operador: objetivo/umbrales, corpus, navegadores, concurrencia, disco, ambiente/cuentas/recursos, presupuestos y autorización específica. No se fijan cifras por inferencia. La aceptación de límites DOC-119 se conserva como antecedente; no equivale a aceptación integral de DOC-120. La autorización actual comprende refinamiento local.
