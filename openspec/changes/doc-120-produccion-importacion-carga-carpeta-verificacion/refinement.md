<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento — DOC-120

## Fuente y alcance

Fuente: Jira DOC-120, contexto completo en specs/produccion-importacion-carga-carpeta-verificacion/jira-context.md. Continuación solicitada por el usuario después de opsxj:new: autorización de refinamiento local. Base funcional 8c0773d4371aeb35543bf182c0abdd0319cababa, merge PR130 de DOC-119; inspección sobre 60e02ae78841f94d08619ac9d53982d4318871c3. Tecnología legacy-webforms-vb, proyecto .NET Framework4.6.1 y JavaScript legacy. No se ejecutaron autenticación, HTTP de aplicación, SQL real, cargas, permisos, fallos o limpieza.

Aprobación del plan local: después de aplicar el mapa24 en73106588, el usuario indicó «continua». Se registra autorización para la consolidación documental y comprobaciones locales del plan. No concede aceptación integral, prueba real ni presupuesto de capacidad. INT-03 permanece NO_EJECUTADO sin definición/autorización. El diseño y la especificación describen cómo resolverlo, sin inventar objetivos.

## Contexto inspeccionado

- Base y evidencias: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-entrega-20261105.json; doc117-matriz-almacenamiento.md; doc118-integracion-completa-20261110.json; doc119-ui-fuentes-20261112.json. Lectura y comparación, primarios inmutables.
- Índices nuevos: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/matriz-verificacion.md; Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/resultados.md. Ausentes: crear solo estos índices al ejecutar el plan; Markdown, sin Compile de proyecto.
- Inspección de refinamiento: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc120-inspeccion-refinamiento-20261112.json. Nuevo manifiesto saneado de fuentes/evidencias, sin credenciales ni efecto de aceptación real.
- Operación y autoridad: Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb:Consultar/ConsultarConfirmacion/ConsultarPagina/ExportarPagina; Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb:Resolver/ResolverDestino/AutorizarRecursos. Consultar y revisar; sin nuevos contratos ni reconciliación desde lectura.
- Coordinación: Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb; CoordinadorImportacionCarpetas.Valores.vb; CoordinadorImportacionCarpetas.Operaciones.vb; CoordinadorImportacionCarpetas.RecuperacionV2.vb. Conservar fases, revisiones y barreras; corregir solo defecto reproducido.
- Adaptadores: Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb; AdaptadorDocumentoImportacion.vb; ComposicionLimpiezaOriginalImportacion.vb:Crear. Únicos existentes; sin invocar escritor o limpieza por inspección.
- UI definitiva: Gestion/WebFormProducionDocumental.aspx/vb; js/gestion/WebFormProducionDocumental.js; js/gestion/ImportacionCarpetas/ImportacionCarpetas.js/Vista.js; js/jquery.contextMenu.js/css. Contrastar huellas DOC-119 y casos afectados; no otro asistente ni menú.
- Pruebas: tests/ImportacionCarpetasBehaviorTests.cs; tests/doc111-importacion-carpetas-client.test.cjs; tests/doc119-navigation.test.cjs; tests/doc119-context-menu.test.cjs; tests/doc88-produccion-upload-contract.test.cjs; tests/doc117-alm03-individual-ui.test.cjs. Reutilizar arnés y Node/Chromium instalados; fuentes de prueba fuera del proyecto productivo.
- E2E y degradación: tools/e2e/AGENT-RUNBOOK.md; tools/e2e/tests/doc111-degradation.test.cjs; tools/e2e/tests/support/doc119-seguimiento.cjs; tools/e2e/tests/support/doc113-ventana-importacion.cjs. Sin login/runner/puente alternativo; plan y autorización previos a ejecución real.
- Guía y dossier: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/11-prompt-seguimiento-operacion.md; Diseno/02-contratos-operacion.md; Doc/Tecnica/Opsxj/doc-120-produccion-importacion-carga-carpeta-verificacion/. Vincular versión y límites; no duplicar plataforma o máquina de pruebas.

Los índices exigidos por Jira no existen en la base inspeccionada y se justifican como entregables documentales dentro de Validacion. El manifiesto doc120-inspeccion-refinamiento-20261112.json registra la comparación de fuentes y evidencias. La propuesta no crea una plataforma nueva.

## Decisiones para revisión

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks del mapa |
| --- | --- | --- | --- | --- | --- |
| D-01 | Fijar el merge de DOC-119 como base funcional. Cada caso conserva SHA/huellas de sus componentes, prueba, ambiente y resultado originales; la identidad de huellas permite reutilización local, no una ejecución real nueva. Un cambio invalida únicamente los casos dependientes; el mapa explicita esa dependencia. | Gestion/WebFormProducionDocumental.aspx; js/gestion/ImportacionCarpetas/ImportacionCarpetas.js; doc119-ui-fuentes-20261112.json | D-01 | RQ-01 | Origen: D-01, RQ-01; 1.1/1.3 |
| D-02 | Crear matriz-verificacion.md y resultados.md en Validacion, ausentes en la base inspeccionada. La matriz relaciona requisito/caso, componente/símbolo, prueba, SHA/huella, ambiente, evidencia, resultado, vigencia y límites. Resultados mantiene cronología y enlaza fuentes primarias sin sobrescribirlas. | Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/; doc111-matriz-final-20261102.md | D-02 | RQ-02 | Origen: D-02, RQ-02; 2.1/2.2 |
| D-03 | Usar resultados efectivos y sus cortes finales. DOC-116 resolvió su ventana interna mediante variante de laboratorio; DOC-117 acredita ALM-01/02/03C con límites; DOC-118 distingue archivo administrativo de cierre técnico38/38; DOC-119 conserva SEG-01 real y SEG-02 con cobertura mixta aceptada. No sumar suites de cortes distintos ni reproducir efectos inciertos retenidos. | doc116-entrega-20261105.json; doc117-matriz-almacenamiento.md; doc118-integracion-completa-20261110.json; doc119-recuperacion-verificada-20261112.json | D-03 | RQ-03 | Origen: D-03, RQ-03; 1.2/2.3 |
| D-04 | Mapear UI01–05 al menú superior/contextual y asistente resultantes de DOC-119. Cubrir seleccionado/pulsado, homónimos, selección inválida, autoridad denegada/revocada, referencia/último lote/confirmación sin acuse, recarga/reapertura/UpdatePanel, árbol, foco y viewport. Reutilizar fuentes vigentes y ejecutar solo casos afectados o brechas documentadas. | js/gestion/WebFormProducionDocumental.js:Doc119NivelPulsado/Doc111ActualizarRama; tests/doc119-navigation.test.cjs; tests/doc111-importacion-carpetas-client.test.cjs | D-04 | RQ-04 | Origen: D-04, RQ-04; 3.1/3.2/3.3/6.3 |
| D-05 | Relacionar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, valores diferidos y fecha estable, revocación, stop e incertidumbre con casos del arnés existente. Comprobar contrato del plugin en sus26 registros/22archivos y carga individual pertinente. Un callback sintético no demuestra la acción de negocio real de cada módulo. | Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb; CoordinadorImportacionCarpetas.Valores.vb; tests/ImportacionCarpetasBehaviorTests.cs; tests/doc119-context-menu.test.cjs | D-05 | RQ-05 | Origen: D-05, RQ-05; 3.4/3.5 |
| D-06 | Conservar el caso real histórico de ausencia temporal de ra_integracion_evento con restauración y carga individual, sin extrapolar a cada incompatibilidad. Contrastar los verificadores/esquema y el guard de indisponibilidad actuales. Exigir variante nueva solo si un requisito concreto y una diferencia reproducida la hacen necesaria; registrar NO_NECESARIA con justificación en los demás casos. | Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/; tools/e2e/tests/doc111-degradation.test.cjs; doc111-degradacion-individual-pass-20261102.md | D-06 | RQ-06 | Origen: D-06, RQ-06; 4.1/4.2/4.3 |
| D-07 | Antes de medir acordar objetivo, cantidad/distribución/tamaño de archivos y raíces, navegadores/versiones, concurrencia, duración, ambiente, recursos, disco temporal/libre y umbrales de tiempo/memoria. Incluir topes HTTP/SELECT/sesiones/bytes/efectos y criterio de parada. Sin esos acuerdos, INT-03 queda NO_EJECUTADO por falta de definición/autorización. | Modelo/ProduccionDocumental/ImportacionCarpetas/; tools/e2e/scripts/run-doc111-importacion-carpetas.cjs; doc119-datos-aceptacion-propuesta-20261112.md | D-07 | RQ-07 | Origen: D-07, RQ-07; 5.1/5.3 |
| D-08 | La continuación autoriza refinamiento/inspección y preparación local. Antes de ejecutar cada ventana real exigir ambiente, cuentas, recursos, operaciones, presupuestos y recuperación revisados. Reutilizar autenticación, runners y queryRows existentes; controles RA/DA exclusivamente SELECT y secretos efímeros. Autorizaciones DOC110–119 no se heredan. | tools/e2e/AGENT-RUNBOOK.md; tools/e2e/tests/support/doc119-seguimiento.cjs; tools/e2e/tests/support/doc113-ventana-importacion.cjs | D-08 | RQ-08 | Origen: D-08, RQ-08; 5.2/6.6 |
| D-09 | Conservar SII/Integracionccv, AlmacenamientoDocumentoProduccionDocumental, Web.config, esquema, semántica de recepción/efectos y PDF/rechazo TIFF. Mantener decisión expresa SEG-03 de operar sin limpieza y datos retenidos. No añadir jobs, cuotas, formatos, endpoints o aislamiento XML adicional. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb:Crear; doc119-seg03-propuesta-sin-limpieza-20261112.md | D-09 | RQ-09 | Origen: D-09, RQ-09; 2.4/6.1 |
| D-10 | Modificar producto solo ante una brecha reproducible vinculada a requisito, causa y prueba de regresión. Registrar cada no ejecutado con motivo, responsable e impacto operativo. Autoridad/destino/integridad/recuperación con fallo crítico bloquean salida. Límites necesitan aceptación expresa; archivo/CI/checklists no sustituyen aceptación integral. | Validacion/matriz-verificacion.md; Validacion/resultados.md; Doc/Tecnica/Opsxj/doc-120-produccion-importacion-carga-carpeta-verificacion/; tools/opsxj/scripts/lib/legacyGovernanceService.js | D-10 | RQ-10 | Origen: D-10, RQ-10; 6.2/6.4/6.5 |

## Contrato, aceptación y compatibilidad por decisión

### D-01 / RQ-01 — Base y vigencia por componente

Fijar el merge de DOC-119 como base funcional. Cada caso conserva SHA/huellas de sus componentes, prueba, ambiente y resultado originales; la identidad de huellas permite reutilización local, no una ejecución real nueva. Un cambio invalida únicamente los casos dependientes; el mapa explicita esa dependencia.

Criterio de aceptación: Una fila con código distinto exige evaluar impacto; igualdad de fuentes no renueva autenticación, datos o runtime.

Entregable local: tareas 1.1/1.3 del mapa aplicado. La evidencia es Gestion/WebFormProducionDocumental.aspx; js/gestion/ImportacionCarpetas/ImportacionCarpetas.js; doc119-ui-fuentes-20261112.json; no modifica autorización o semántica de negocio.

### D-02 / RQ-02 — Matriz e índices únicos

Crear matriz-verificacion.md y resultados.md en Validacion, ausentes en la base inspeccionada. La matriz relaciona requisito/caso, componente/símbolo, prueba, SHA/huella, ambiente, evidencia, resultado, vigencia y límites. Resultados mantiene cronología y enlaza fuentes primarias sin sobrescribirlas.

Criterio de aceptación: Cada caso se clasifica REAL_HISTORICO, LOCAL_HISTORICO_VIGENTE, LOCAL_DOC120, ESTATICO o NO_EJECUTADO; el resultado primario permanece separado de esa clasificación.

Entregable local: tareas 2.1/2.2 del mapa aplicado. La evidencia es Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/; doc111-matriz-final-20261102.md; no modifica autorización o semántica de negocio.

### D-03 / RQ-03 — Consumo de EXP/ALM/COORD/SEG

Usar resultados efectivos y sus cortes finales. DOC-116 resolvió su ventana interna mediante variante de laboratorio; DOC-117 acredita ALM-01/02/03C con límites; DOC-118 distingue archivo administrativo de cierre técnico38/38; DOC-119 conserva SEG-01 real y SEG-02 con cobertura mixta aceptada. No sumar suites de cortes distintos ni reproducir efectos inciertos retenidos.

Criterio de aceptación: Resolver contradicciones editoriales por fuente primaria, fecha y alcance; preservar intentos41/53, documentos e historial. Los observadores de laboratorio no se atribuyen al runtime ordinario.

Entregable local: tareas 1.2/2.3 del mapa aplicado. La evidencia es doc116-entrega-20261105.json; doc117-matriz-almacenamiento.md; doc118-integracion-completa-20261110.json; doc119-recuperacion-verificada-20261112.json; no modifica autorización o semántica de negocio.

### D-04 / RQ-04 — Interfaz definitiva y regresión focal

Mapear UI01–05 al menú superior/contextual y asistente resultantes de DOC-119. Cubrir seleccionado/pulsado, homónimos, selección inválida, autoridad denegada/revocada, referencia/último lote/confirmación sin acuse, recarga/reapertura/UpdatePanel, árbol, foco y viewport. Reutilizar fuentes vigentes y ejecutar solo casos afectados o brechas documentadas.

Criterio de aceptación: La UI final de DOC-119 es LOCAL con servicios sintéticos. El estado real SEG-02 conserva sus límites; no acredita inserción nueva ni raíces físicas homónimas observadas en IIS.

Entregable local: tareas 3.1/3.2/3.3/6.3 del mapa aplicado. La evidencia es js/gestion/WebFormProducionDocumental.js:Doc119NivelPulsado/Doc111ActualizarRama; tests/doc119-navigation.test.cjs; tests/doc111-importacion-carpetas-client.test.cjs; no modifica autorización o semántica de negocio.

### D-05 / RQ-05 — Integridad, incertidumbre y consumidores

Relacionar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, valores diferidos y fecha estable, revocación, stop e incertidumbre con casos del arnés existente. Comprobar contrato del plugin en sus26 registros/22archivos y carga individual pertinente. Un callback sintético no demuestra la acción de negocio real de cada módulo.

Criterio de aceptación: Consultar/exportar no procesa ni reconcilia; RECIBIDO no equivale a DOCUMENTO_ALMACENADO. Stop no revierte efectos y una salida INCIERTO no autoriza replay.

Entregable local: tareas 3.4/3.5 del mapa aplicado. La evidencia es Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Barreras.vb; CoordinadorImportacionCarpetas.Valores.vb; tests/ImportacionCarpetasBehaviorTests.cs; tests/doc119-context-menu.test.cjs; no modifica autorización o semántica de negocio.

### D-06 / RQ-06 — INT-02 degradación acotada

Conservar el caso real histórico de ausencia temporal de ra_integracion_evento con restauración y carga individual, sin extrapolar a cada incompatibilidad. Contrastar los verificadores/esquema y el guard de indisponibilidad actuales. Exigir variante nueva solo si un requisito concreto y una diferencia reproducida la hacen necesaria; registrar NO_NECESARIA con justificación en los demás casos.

Criterio de aceptación: DDL previo manual se conserva. No reinstalar migraciones ni degradar la base para refrescar un antecedente; cualquier DDL/fallo real necesita aprobación específica y restauración revisada.

Entregable local: tareas 4.1/4.2/4.3/4.4 del mapa aplicado. La evidencia es Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/; tools/e2e/tests/doc111-degradation.test.cjs; doc111-degradacion-individual-pass-20261102.md; no modifica autorización o semántica de negocio.

### D-07 / RQ-07 — INT-03 capacidad con presupuesto definido

Antes de medir acordar objetivo, cantidad/distribución/tamaño de archivos y raíces, navegadores/versiones, concurrencia, duración, ambiente, recursos, disco temporal/libre y umbrales de tiempo/memoria. Incluir topes HTTP/SELECT/sesiones/bytes/efectos y criterio de parada. Sin esos acuerdos, INT-03 queda NO_EJECUTADO por falta de definición/autorización.

Criterio de aceptación: No inferir capacidad máxima de lotes pequeños, máximo de página, pruebas de contratos o solapamientos de laboratorio. Responsable: solicitante y operador del ambiente; efecto: sin garantía de capacidad ni cierre integral por inferencia.

Entregable local: tareas 5.1/5.3 del mapa aplicado. La evidencia es Modelo/ProduccionDocumental/ImportacionCarpetas/; tools/e2e/scripts/run-doc111-importacion-carpetas.cjs; doc119-datos-aceptacion-propuesta-20261112.md; no modifica autorización o semántica de negocio.

### D-08 / RQ-08 — Autorización separada y herramientas existentes

La continuación autoriza refinamiento/inspección y preparación local. Antes de ejecutar cada ventana real exigir ambiente, cuentas, recursos, operaciones, presupuestos y recuperación revisados. Reutilizar autenticación, runners y queryRows existentes; controles RA/DA exclusivamente SELECT y secretos efímeros. Autorizaciones DOC110–119 no se heredan.

Criterio de aceptación: Lectura, recepción, creación, almacenamiento, fallos, DDL y permisos son alcances distintos. Un STOPPED no es PASS; no reintentar automáticamente ni extraer credenciales de archivos/procesos.

Entregable local: tareas 5.2/6.6 del mapa aplicado. La evidencia es tools/e2e/AGENT-RUNBOOK.md; tools/e2e/tests/support/doc119-seguimiento.cjs; tools/e2e/tests/support/doc113-ventana-importacion.cjs; no modifica autorización o semántica de negocio.

### D-09 / RQ-09 — Fuentes y decisiones protegidas

Conservar SII/Integracionccv, AlmacenamientoDocumentoProduccionDocumental, Web.config, esquema, semántica de recepción/efectos y PDF/rechazo TIFF. Mantener decisión expresa SEG-03 de operar sin limpieza y datos retenidos. No añadir jobs, cuotas, formatos, endpoints o aislamiento XML adicional.

Criterio de aceptación: SII real y limpieza real siguen NO_EJECUTADO. La medición57archivos/24.371bytes es histórica previa a nuevos datos; no representa disco actual ni capacidad disponible.

Entregable local: tareas 2.4/6.1 del mapa aplicado. La evidencia es Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComposicionLimpiezaOriginalImportacion.vb:Crear; doc119-seg03-propuesta-sin-limpieza-20261112.md; no modifica autorización o semántica de negocio.

### D-10 / RQ-10 — Defectos reproducidos y criterio de cierre

Modificar producto solo ante una brecha reproducible vinculada a requisito, causa y prueba de regresión. Registrar cada no ejecutado con motivo, responsable e impacto operativo. Autoridad/destino/integridad/recuperación con fallo crítico bloquean salida. Límites necesitan aceptación expresa; archivo/CI/checklists no sustituyen aceptación integral.

Criterio de aceptación: Una diferencia de huella por sí sola no prueba defecto ni obliga a repetir efectos. La revisión final identifica SHA/ambiente y resolución; publicación, archivo, merge y cierre se ejecutan posteriormente con sus comprobaciones.

Entregable local: tareas 6.2/6.4/6.5 del mapa aplicado. La evidencia es Validacion/matriz-verificacion.md; Validacion/resultados.md; Doc/Tecnica/Opsxj/doc-120-produccion-importacion-carga-carpeta-verificacion/; tools/opsxj/scripts/lib/legacyGovernanceService.js; no modifica autorización o semántica de negocio.

## Requisitos verificables

| ID | Resultado observable | Escenario | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Base y vigencia por componente | WHEN se contrasta una evidencia con la versión final, THEN se comparan sus componentes y huellas, conservando SHA/runtime/ambiente de origen | Una fila con código distinto exige evaluar impacto; igualdad de fuentes no renueva autenticación, datos o runtime. |
| RQ-02 | Matriz e índices únicos | WHEN se incorpora un caso a la matriz, THEN figuran requisito/caso/componente/prueba/SHA/ambiente/evidencia/resultado/vigencia/límite | Cada caso se clasifica REAL_HISTORICO, LOCAL_HISTORICO_VIGENTE, LOCAL_DOC120, ESTATICO o NO_EJECUTADO; el resultado primario permanece separado de esa clasificación. |
| RQ-03 | Consumo de EXP/ALM/COORD/SEG | WHEN un informe contiene bloqueos históricos y una resolución posterior, THEN se identifica el corte efectivo con su fuente primaria y variante | Resolver contradicciones editoriales por fuente primaria, fecha y alcance; preservar intentos41/53, documentos e historial. Los observadores de laboratorio no se atribuyen al runtime ordinario. |
| RQ-04 | Interfaz definitiva y regresión focal | WHEN se valida acceso superior o contextual en la UI definitiva, THEN se conserva seleccionado frente a pulsado, ruta autorizada y destino fijo | La UI final de DOC-119 es LOCAL con servicios sintéticos. El estado real SEG-02 conserva sus límites; no acredita inserción nueva ni raíces físicas homónimas observadas en IIS. |
| RQ-05 | Integridad, incertidumbre y consumidores | WHEN se evalúan escenarios completos/parciales, valores diferidos y fecha estable, THEN cada regla tiene caso y evidencia de las barreras/revisiones existentes | Consultar/exportar no procesa ni reconcilia; RECIBIDO no equivale a DOCUMENTO_ALMACENADO. Stop no revierte efectos y una salida INCIERTO no autoriza replay. |
| RQ-06 | INT-02 degradación acotada | WHEN se reutiliza la ausencia de ra_integracion_evento, THEN se conserva alcance de indisponibilidad/restauración/carga individual | DDL previo manual se conserva. No reinstalar migraciones ni degradar la base para refrescar un antecedente; cualquier DDL/fallo real necesita aprobación específica y restauración revisada. |
| RQ-07 | INT-03 capacidad con presupuesto definido | WHEN se propone medir INT-03, THEN existen objetivo, corpus, navegadores/versiones, concurrencia, disco, umbrales y topes de ejecución revisados | No inferir capacidad máxima de lotes pequeños, máximo de página, pruebas de contratos o solapamientos de laboratorio. Responsable: solicitante y operador del ambiente; efecto: sin garantía de capacidad ni cierre integral por inferencia. |
| RQ-08 | Autorización separada y herramientas existentes | WHEN se pretende autenticar, consultar BD, cargar o inyectar fallos, THEN se exige autorización de ambiente/cuentas/recursos/operaciones/presupuestos/recuperación | Lectura, recepción, creación, almacenamiento, fallos, DDL y permisos son alcances distintos. Un STOPPED no es PASS; no reintentar automáticamente ni extraer credenciales de archivos/procesos. |
| RQ-09 | Fuentes y decisiones protegidas | WHEN se consolida o corrige una brecha de DOC-120, THEN se preservan SII/almacenamiento compartido/configuración/esquema y PDF con rechazo TIFF | SII real y limpieza real siguen NO_EJECUTADO. La medición57archivos/24.371bytes es histórica previa a nuevos datos; no representa disco actual ni capacidad disponible. |
| RQ-10 | Defectos reproducidos y criterio de cierre | WHEN se detecta un defecto de autoridad, destino, integridad o recuperación, THEN se registra causa/caso/corrección/regresión y se bloquea salida mientras sea crítico | Una diferencia de huella por sí sola no prueba defecto ni obliga a repetir efectos. La revisión final identifica SHA/ambiente y resolución; publicación, archivo, merge y cierre se ejecutan posteriormente con sus comprobaciones. |

## Trazabilidad y aplicación

Video194059,2026-11-14: fallo reproducido en presentación posterior a la creación sin confirmación del lote39. Tareas3.24–3.28 cubren D-04/RQ-04 (aviso visible y detalle del bloqueo), D-05/RQ-05 (contexto, autoridad de verificación, continuación/final confirmado) y D-08/RQ-08/D-10/RQ-10 (diagnóstico real específico y evidencia). EN_CURSO/CREANDO no se convierten en INCIERTO por timeout ni autorizan replay. El diagnóstico real de5 SELECT requiere autorización explícita antes de ejecutarse; la corrección local no demuestra recuperación del lote real ni causa interna del checkpoint. El usuario solo aportó el video; no se hereda otra ventana real.48 tareas y29 escenarios; aceptación integral sigue abierta.

Corrección de fidelidad posterior a2642e604, solicitada expresamente por el usuario: «no quedo identico a la maqueta», «siempre este visible CONSULTAR importacion Consultar ultimo lote» y «dejalo igual». Se añaden3.19–3.23, vinculadas a D-04/RQ-04 (estilo y accesos), D-05/RQ-05 (acciones y aislamiento de Files durante consulta) y D-10/RQ-10 (comparación y evidencia). La barra de consultas y el pie quedan fuera del contenido desplazable; el diseño reutiliza los estilos del HTML con prefijo del asistente. Comparación116 propiedades y71/71 locales;43 tareas,27 escenarios. Se conservan los datos y controles operativos que la maqueta no simula, el marco modal y las cuatro tareas de aceptación/autorización integral abiertas.

Ampliación expresamente autorizada2026-11-13: «agrega las tareas al task y realiza la implmentacion». El contraste de la maqueta registra C01/C02/C03;3.10–3.12 corrigen sus diferencias sin relajar el servidor.3.13–3.17 reúnen perfil/carpetas, destacan acciones existentes, despliegan filtros/configuración y conservan consulta/recuperación/foco, con conteos de envío y actualización visible.3.18 registra regresión y huellas. Origen D-04/RQ-04 (presentación y consulta), D-05/RQ-05 (acciones/barreras) y D-10/RQ-10 (evidencia/aceptación). El diseño y RQ-04 incorporan tres escenarios;25 escenarios y38 tareas en total. Se trata de una ampliación de presentación autorizada, no de nueva arquitectura, automatización de fases o edición PDF. Las cuatro tareas de aceptación/autorización integral permanecen abiertas.

10 decisiones/10 requisitos/20 escenarios. Durante INT-02 se reprodujo DOC120-SUP01 en el auxiliar de pruebas: botón oculto tras el nuevo menú. Se añade tarea4.4 D-06/RQ-06, corregida y verificada localmente; plan activo25 tareas, sin cambio de producto ni nueva ventana real. Mapa24 inicial aplicado a tasks.md en73106588; plan-tareas-propuesto.md conserva la procedencia. El usuario continuó sobre esa entrega: aprobación para trabajo local, sin efectos reales nuevos. Auditar y sincronizar con opsxj:refine -- DOC-120 --sync; la aprobación del plan no se contabiliza como ejecución de sus24 tareas.

## Condiciones operativas de salida

Solicitante y operador deben definir y autorizar INT-03 antes de medir. Responsable del ambiente revisa cada ventana real necesaria. SII real/limpieza real NO_EJECUTADO; SEG-02 mantiene cobertura mixta histórica aceptada y UI final local. Los fallos críticos bloquean la salida; cada limitación necesita decisión explícita sobre su impacto, sin aceptación de DOC-120 por herencia.

## Ajuste focal autorizado2026-11-13

Solicitud «corrige» sobre el aviso amarillo de apertura del video: D-04/D-10 y RQ-04/RQ-10 amparan la corrección de producto reproducida41px y su regresión. Tarea3.6 y Validacion/doc120-apertura-20261113.json fijan fuentes y41/41 casos locales. Se conserva el aviso de confirmación sin acuse y errores reales. La frase inicial sin cambios de producto corresponde a la inspección original; este corte sí modifica JS/CSS y versiones de recursos ASPX. No cambia contratos, backend, fuentes protegidas ni autorización de pruebas reales.

Diagnóstico64c70664 autorizado posteriormente por «continua»: STOPPED,1 intento presupuestado, sin filas conservadas. Corrección local del auxiliar dentro de3.27 conserva códigos seguros de una lista cerrada, lectura actual y contadores distintos;12/12 locales, sin ampliar el libro SQL. No se repite la ventana ni se conoce la causa de negocio. Se conserva43/48,48 tareas/29 escenarios y aceptación abierta; otra ventana requiere instrucción expresa, hasta5 SELECT adicionales al intento anterior,0 HTTP/efectos. Fuente: Validacion/doc120-diagnostico-reporte-20261114.json.
