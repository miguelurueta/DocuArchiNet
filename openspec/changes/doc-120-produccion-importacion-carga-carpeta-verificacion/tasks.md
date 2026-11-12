<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
# Tareas — DOC-120

Mapa aplicado a este archivo activo desde plan-tareas-propuesto.md por la indicación del usuario sobre tasks.md. Sustituye las ocho entradas genéricas iniciales.

Progreso: **21/25 tareas verificadas en el alcance documental/local**. Complejidad:12 S,13 M,0 L. Se añade4.4 por la brecha DOC120-SUP01 reproducida durante INT-02. Cada tarea declara área, origen D/RQ y verificación. El usuario indicó «continua» sobre73106588: plan aprobado para consolidación y comprobaciones locales, sin aceptación operativa ni autorización de pruebas reales.

Dependencias: refinamiento aprobado para trabajo local; seleccionar regresión por vigencia y brechas; definir y autorizar cada ventana real antes de usar ambiente/cuentas/recursos. INT-03 requiere objetivo, umbrales y presupuesto explícitos; sin ellos se registra NO_EJECUTADO según D-07/RQ-07. Las autorizaciones de DOC110–119 no se heredan.

## 1. Procedencia y vigencia por componente

- [x] 1.1 [S] Fijar la base funcional y las huellas actuales de componentes. Área/archivos: doc120-inspeccion-refinamiento-20261112.json; doc119-ui-fuentes-20261112.json. Origen: D-01, RQ-01. Verificación: Cada componente tiene SHA-256 LF y contraste con8c0773d4 y con la evidencia DOC-119.
- [x] 1.2 [M] Inventariar los resultados efectivos EXP/ALM/COORD/SEG. Área/archivos: Validacion/doc116-entrega-20261105.json; doc117-matriz-almacenamiento.md; doc118-integracion-completa-20261110.json; doc119-seg01-aceptado-20261112.json. Origen: D-03, RQ-03. Verificación: Cada familia conserva fuente primaria, SHA/ambiente, resultado y limitación; no se suman cortes.
- [x] 1.3 [M] Relacionar cada caso con los componentes que condicionan su vigencia. Área/archivos: Validacion/matriz-verificacion.md; manifiestos de fuentes por ejecución. Origen: D-01, RQ-01. Verificación: Un diff cambia únicamente los casos dependientes y nunca convierte una ejecución histórica en nueva.

## 2. Matriz e índices de evidencia

- [x] 2.1 [M] Crear la matriz única con las columnas de procedencia y alcance. Área/archivos: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/matriz-verificacion.md. Origen: D-02, RQ-02. Verificación: Cada requisito/caso enlaza componente, prueba, código, ambiente, fuente, resultado, vigencia y límite.
- [x] 2.2 [S] Crear el índice cronológico de resultados sin sustituir fuentes. Área/archivos: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/resultados.md. Origen: D-02, RQ-02. Verificación: Cada corte tiene fecha/identidad y enlace; original STOPPED/NO_EJECUTADO sigue accesible.
- [x] 2.3 [S] Resolver las contradicciones de estado entre cortes históricos. Área/archivos: doc116-verificacion-final-20261105.json; doc118-resultados-integracion.md; matriz-verificacion.md. Origen: D-03, RQ-03. Verificación: La matriz distingue la resolución interna DOC-116 y38/38 técnico DOC-118 de sus bloqueos o archivo administrativo previos.
- [x] 2.4 [S] Conservar la decisión SEG-03 y los límites de SEG-02 en los índices. Área/archivos: doc119-seg03-propuesta-sin-limpieza-20261112.md; doc119-aceptacion-cobertura-mixta-propuesta-20261112.md. Origen: D-09, RQ-09. Verificación: No se anuncia borrado real, disco actual ni inserción/homónimos reales no observados.

## 3. Regresión focal de la versión definitiva

- [x] 3.1 [S] Determinar los casos UI que requieren regresión en la versión definitiva. Área/archivos: doc119-ui-fuentes-20261112.json; tests/doc119-navigation.test.cjs; tests/doc111-importacion-carpetas-client.test.cjs. Origen: D-04, RQ-04. Verificación: Selección de casos derivada de huellas y brechas; cada exclusión por reutilización está razonada.
- [x] 3.2 [M] Verificar destino y autoridad de ambos accesos con los casos afectados. Área/archivos: Doc119NivelPulsado/Doc119InvocarImportacion; tests/doc119-navigation.test.cjs. Origen: D-04, RQ-04. Verificación: SeleccionadoA/pulsadoB, homónimos, nodos excluidos y denegación quedan clasificados local/real sin extrapolación.
- [x] 3.3 [M] Verificar consulta y recuperación del asistente con los casos afectados. Área/archivos: ImportacionCarpetas.js/Vista.js; tests/doc111-importacion-carpetas-client.test.cjs; tools/e2e/tests/support/doc119-seguimiento.cjs. Origen: D-04, RQ-04. Verificación: Referencia, último lote, confirmación sin acuse, recarga y rama conservan identidad sin replay; alcance y foco quedan registrados.
- [x] 3.4 [M] Acreditar el contrato de consumidores del contextual y la carga individual pertinente. Área/archivos: doc119-contextuales-inventario-20261112.json; tests/doc119-context-menu.test.cjs; tests/doc88-produccion-upload-contract.test.cjs; tests/doc117-alm03-individual-ui.test.cjs. Origen: D-05, RQ-05. Verificación: Se registra contrato de26 invocadores y alcance de la carga individual; no se afirman26 flujos reales.
- [x] 3.5 [M] Consolidar la cobertura de integridad, valores, revocación, stop e incertidumbre. Área/archivos: tests/ImportacionCarpetasBehaviorTests.cs; CoordinadorImportacionCarpetas.Barreras/Valores/RecuperacionV2.vb; evidencias DOC114–118. Origen: D-05, RQ-05. Verificación: Matriz completa/parcial y fecha estable tienen caso/fuente; incertidumbres retenidas no se reenvían.

## 4. Degradación acotada — INT-02

- [x] 4.1 [S] Delimitar el antecedente de degradación y su compatibilidad actual. Área/archivos: doc111-degradacion-individual-pass-20261102.md; tools/e2e/tests/doc111-degradation.test.cjs; repositorio MySQL vigente. Origen: D-06, RQ-06. Verificación: Se identifica exactamente ra_integracion_evento ausente y restauración; no se extrapola a otro fallo de esquema.
- [x] 4.2 [M] Contrastar la respuesta de indisponibilidad y el aislamiento del flujo individual. Área/archivos: ImportacionCarpetasContextAdapter.vb; AdaptadorPerfilImportacionOperativa.vb; pruebas de degradación existentes. Origen: D-06, RQ-06. Verificación: La revisión identifica contrato/casos vigentes y cualquier brecha concreta sin ejecutar DDL.
- [x] 4.3 [S] Resolver si INT-02 exige una variante adicional. Área/archivos: Validacion/matriz-verificacion.md; Validacion/resultados.md. Origen: D-06, RQ-06. Verificación: Resultado NO_NECESARIA con requisito/justificación, o plan de variante concreta con defecto, restauración y autorización propia; nunca cobertura genérica.

- [x] 4.4 [M] Corregir el acceso del auxiliar de degradación al menú operativo moderno. Área/archivos: tools/e2e/tests/support/doc111-degradation.cjs; tools/e2e/tests/doc111-degradation.test.cjs. Origen: D-06, RQ-06. Verificación: DOC120-SUP01 reproduce Timeout con botón oculto antes del cambio; después4/4 políticas/UI pasan con cliente y Chromium locales y respuesta503 sintética, sin DDL ni autenticación.

## 5. Capacidad y autorización — INT-03

- [x] 5.1 [M] Preparar el contrato medible de capacidad INT-03. Área/archivos: Validacion/resultados.md; plan específico enlazado a runners existentes. Origen: D-07, RQ-07. Verificación: Quedan campos explícitos de archivos, tamaños, navegadores, concurrencia, disco, tiempos/memoria, topes y criterio de parada sin inventar valores aprobados.
- [ ] 5.2 [S] Obtener la autorización específica del plan real antes de ejecutarlo. Área/archivos: tools/e2e/AGENT-RUNBOOK.md; plan de ventana concreta. Origen: D-08, RQ-08. Verificación: El responsable confirma ambiente/cuentas/recursos/efectos/recuperación/topes; no se usan autorizaciones anteriores como sustituto.
- [x] 5.3 [M] Registrar la medición autorizada de capacidad o su no ejecución justificada. Área/archivos: Validacion/resultados.md; runner ya aprobado para esa ventana. Origen: D-07, RQ-07. Verificación: Resultado conserva SHA, ambiente y métricas contra umbrales; sin definición/autorización consta NO_EJECUTADO con responsable e impacto sobre operación.

## 6. Integridad de la entrega y cierre

- [x] 6.1 [S] Auditar el diff de las fuentes y decisiones protegidas. Área/archivos: Integracionccv/; Web.config; almacenamiento compartido; esquema; ComposicionLimpiezaOriginalImportacion.vb. Origen: D-09, RQ-09. Verificación: No hay cambios fuera de alcance, limpieza/jobs/cuotas nuevos o soporte TIFF; SII real conserva su clasificación.
- [x] 6.2 [M] Clasificar las brechas críticas en el registro de aceptación. Área/archivos: Validacion/matriz-verificacion.md; Validacion/resultados.md. Origen: D-10, RQ-10. Verificación: Cada brecha tiene reproducción, causa y componente; antes de corregir se descompone en tarea concreta de corrección/regresión, o consta ausencia de brecha reproducida.
- [x] 6.3 [S] Vincular la guía operativa a la versión definitiva verificada. Área/archivos: 11-prompt-seguimiento-operacion.md; Diseno/02-contratos-operacion.md; Validacion/resultados.md. Origen: D-04, RQ-04. Verificación: Consulta/efectos/stop/incertidumbre y accesos corresponden al SHA aceptado, sin promesas de replay o limpieza.
- [ ] 6.4 [M] Revisar la matriz de aceptación y los límites con efecto operativo. Área/archivos: Validacion/matriz-verificacion.md; Validacion/resultados.md. Origen: D-10, RQ-10. Verificación: No queda fallo crítico de autoridad/destino/integridad/recuperación; cada límite/no ejecutado tiene motivo, responsable y decisión de salida explícita.
- [ ] 6.5 [S] Consolidar documentación, OpenSpec y gobierno del SHA de entrega. Área/archivos: Doc/Tecnica/Opsxj/doc-120-produccion-importacion-carga-carpeta-verificacion/; artefactos OpenSpec y registros OPSXJ. Origen: D-10, RQ-10. Verificación: Validación estricta y gobierno sobre código/evidencia reales; un documento generado o CI no concede aceptación integral.
- [ ] 6.6 [S] Registrar la entrega y sincronización de la carpeta principal. Área/archivos: Rama feature/DOC-120; Git y estado OPSXJ. Origen: D-08, RQ-08. Verificación: Commit/revisión/PR mantienen SHA y límites; archivo y cierre no anteceden a la aceptación y al merge verificado.

## Cobertura y límites

Diez decisiones D-01–D-10 y diez requisitos RQ-01–RQ-10 cubiertos por25 tareas (mapa inicial24 más corrección focal4.4). El diseño y los20 escenarios de la especificación conservan su contenido. Los resultados de origen mantienen SHA/ambiente/clasificación y sus límites real/local/estático. No se autorizan pruebas reales, cargas, cambios de permisos, DDL, fallos ni limpieza mediante este mapa.

## Evidencia de este corte local

matriz-verificacion.md y resultados.md consolidan32 casos/clasificaciones con25 fuentes primarias y dependencias de11 componentes. doc120-consolidacion-20261112.json fija15/15 huellas UI,114 entradas del arnés, resultados locales y diff de producto vacío. La regresión local inicial5/5 contiene2 wrappers Framework/DDL y3 políticas; después4/4 incluye esas3 políticas y un nuevo caso Chromium, sin sumarlos como9 pruebas distintas. UI99/33 de DOC119 se reutiliza por huellas/diff, sin nueva sesión real.

5.3 se verifica como NO_EJECUTADO con motivo/responsable/impacto, no como medición aprobada. Permanecen abiertas5.2 (autorización de ventana real),6.4 (decisión de aceptación de límites),6.5 (gobierno de entrega) y6.6 (entrega final). No hay carga/DDL/permisos/fallos/limpieza reales nuevos. La aprobación del plan local no cierra la aceptación integral.
