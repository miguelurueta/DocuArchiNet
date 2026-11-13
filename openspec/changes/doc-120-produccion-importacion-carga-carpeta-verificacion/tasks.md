<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
# Tareas — DOC-120

Mapa aplicado a este archivo activo desde plan-tareas-propuesto.md por la indicación del usuario sobre tasks.md. Sustituye las ocho entradas genéricas iniciales.

Progreso: **24/28 tareas verificadas en el alcance documental/local**. Complejidad:12 S,16 M,0 L. Se añaden3.6/3.7 por correcciones de interfaz,3.8 por la solicitud expresa de alinear únicamente la barra superior y4.4 por la brecha DOC120-SUP01 reproducida durante INT-02. Cada tarea declara área, origen D/RQ y verificación. El usuario indicó «continua» sobre73106588: plan aprobado para consolidación y comprobaciones locales, sin aceptación operativa ni autorización de pruebas reales.

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

- [x] 3.6 [M] Corregir la franja amarilla de carga al abrir la importación. Área/archivos: ImportacionCarpetas.js/css; Gestion/WebFormProducionDocumental.aspx; tests/doc119-navigation.test.cjs; Validacion/doc120-apertura-20261113.json. Origen: D-04, RQ-04. Relación adicional: D-10, RQ-10. Verificación: reproducción previa del desplazamiento41px; apertura demorada sin franja ni movimiento, error real y confirmación sin acuse visibles, indicador accesible y recursos versionados;41/41 locales con respuestas sintéticas, sin operación real.

- [x] 3.7 [M] Unificar el menú de Importación con la barra existente. Área/archivos: ImportacionCarpetas.js/css; PerfilImportacionEditor.js; Gestion/WebFormProducionDocumental.aspx; tests/doc119-navigation.test.cjs. Origen: D-04, RQ-04. Relación adicional: D-10, RQ-10. Verificación: colores calculados iguales a Archivos e iconos en cabecera/opciones; permisos, referencias y reinicialización conservados;55/55 pruebas locales con CSS/fuente reales y servicios sintéticos. Evidencia: Validacion/doc120-menu-20261113.json.

- [x] 3.8 [M] Alinear y adaptar la distribución de la barra superior de Producción Documental mostrada en barra-superior.png. Área/archivos: Gestion/WebFormProducionDocumental.aspx (#menu_var, #navbarNavDropdown y búsqueda); estilos limitados a #menu_var; js/gestion/WebFormProducionDocumental.js (cálculo de altura si resulta afectado); js/gestion/ImportacionCarpetas/ImportacionCarpetas.js (montaje del menú dinámico); tests/doc119-navigation.test.cjs. Origen: D-04, RQ-04. Relación adicional: D-10, RQ-10. Verificación: en Chromium local, a390/768/1024/1280/1554px y con texto aumentado, icono/texto/flecha de cada cabecera permanecen juntos y alineados; la distribución o colapso mantiene accesibles Expedientes, Cargar, Descargar, Documentos, Compartir, Archivos, Importación y búsqueda sin desbordamiento horizontal ni superposición; se conservan identificadores, manejadores, permisos, búsqueda con Enter, foco/teclado y acceso tras reinicialización/postback, sin duplicar acciones y con alturas del árbol/listado coherentes con la barra, sin recortes. Registrar comparación visual y regresión local de paridad de los manejadores, con servicios sintéticos y sin ejecutar efectos de negocio. Alcance exclusivo de la franja adjunta: conservar contenido y comportamiento de los desplegables; no rediseñar submenús, contextual, árbol, listado, asistentes, backend ni otros módulos. Estado: implementada tras la indicación «implementa»; regresión local60/60 y evidencia Validacion/doc120-barra-20261113.json.

Condición obligatoria de aceptación de3.8, ratificada por el usuario: el ajuste modifica únicamente la presentación/distribución de la barra y no debe afectar el funcionamiento de ningún menú ni de la búsqueda. Conservar opciones/submenús, destinos, selección/contexto, permisos, identificadores y manejadores originales; búsqueda por clic y Enter y restauración de búsqueda mantienen sus controles y resultados. No cambiar lógica de negocio, peticiones, parámetros ni eventos de postback; no clonar controles ni registrar manejadores duplicados. Comparar la paridad funcional antes/después, incluyendo Importación dinámica y reinicialización/postback: cada acción invoca una sola vez su destino original y buscar/restaurar conserva término y selección según el contrato vigente. Si se observa cualquier regresión funcional en menús o búsqueda,3.8 no se acepta aunque la alineación sea correcta. Condición comprobada en el alcance local con fronteras de negocio sintéticas y60/60 pruebas; no acredita una ejecución IIS autenticada.

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

Diez decisiones D-01–D-10 y diez requisitos RQ-01–RQ-10 cubiertos por28 tareas (mapa inicial24 más correcciones focales4.4,3.6,3.7 y tarea de barra superior3.8). Las diez decisiones y veinte escenarios originales se conservan; se añade a RQ-04 un escenario de barra adaptable sin regresión funcional (21 escenarios). Los resultados de origen mantienen SHA/ambiente/clasificación y sus límites real/local/estático. No se autorizan pruebas reales, cargas, cambios de permisos, DDL, fallos ni limpieza mediante este mapa.

## Evidencia del corte local2026-11-12

matriz-verificacion.md y resultados.md consolidan32 casos/clasificaciones con25 fuentes primarias y dependencias de11 componentes. doc120-consolidacion-20261112.json fija15/15 huellas UI,114 entradas del arnés, resultados locales y diff de producto vacío. La regresión local inicial5/5 contiene2 wrappers Framework/DDL y3 políticas; después4/4 incluye esas3 políticas y un nuevo caso Chromium, sin sumarlos como9 pruebas distintas. UI99/33 de DOC119 se reutiliza por huellas/diff, sin nueva sesión real.

5.3 se verifica como NO_EJECUTADO con motivo/responsable/impacto, no como medición aprobada. Permanecen abiertas5.2 (autorización de ventana real),6.4 (decisión de aceptación de límites),6.5 (gobierno de entrega) y6.6 (entrega final). No hay carga/DDL/permisos/fallos/limpieza reales nuevos. La aprobación del plan local no cierra la aceptación integral.

## Corrección local2026-11-13

Tarea3.6 completada; avance22/26. doc120-apertura-20261113.json fija las fuentes cambiadas y41/41 pruebas locales actuales. La igualdad15/15 anterior y el diff de producto vacío son históricos; las fuentes de producto JS/CSS y sus URLs ASPX cambian solo por este defecto reproducido. Sin nueva ejecución real ni aceptación integral; permanecen las mismas cuatro tareas abiertas.

Corte posterior de menú: tarea3.7 completada;23/27. Iconos/colores y regresión55/55 fijados en doc120-menu-20261113.json. Las cuatro tareas de autorización/aceptación/gobierno/entrega integral siguen abiertas.

## Incorporación de la barra superior2026-11-13

El usuario acota la solicitud a la imagen adjunta barra-superior.png y pide registrarla como tarea de DOC-120. Se incorpora3.8 sin implementar código ni ampliar la modernización a los desplegables u otras superficies. En el corte de planificación:23/28; abiertas3.8,5.2,6.4,6.5 y6.6. Los cortes anteriores y sus conteos/resultados se conservan como antecedentes; el análisis local de distribución no constituye aprobación de la futura implementación.

## Implementación de3.8 — 2026-11-13

Solicitud «implementa»: barra alineada y adaptable, acciones/búsqueda originales conservadas y altura ligada al panel. Baseline funcional36 rutas aprobado; distribución original falla; regresión final60/60 local con servicios/fronteras sintéticos. Recursos incluidos como Content y versionados; evidencia doc120-barra-20261113.json. Avance24/28; permanecen abiertas5.2,6.4,6.5 y6.6. Sin nueva sesión IIS autenticada, cambios de permisos o efectos de negocio.
