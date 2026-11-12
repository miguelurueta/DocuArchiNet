# DOC-119 — implementación y verificación de navegación definitiva

Fecha:2026-11-12. Carpeta principal, rama `feature/DOC-119`. Base previa a esta implementación:b6f40d3b56ab385ab432ebee971929d610c58952. Las huellas SHA-256 normalizadas de las fuentes y pruebas exactas están en `doc119-ui-fuentes-20261112.json`; el commit que incorpora este informe identifica la entrega. No se atribuyen los cambios nuevos al SHA base.

## Resultado y procedencia

Implementación local de las24 tareas terminada y verificada. SEG-01 integrado ccb25ec9 y SEG-0229623218 conservan sus resultados originales, ambiente y SHA en sus respectivos JSON. El usuario respondió «aceptado» a la cobertura mixta de SEG-02 antes de empezar UI. El runtime sigue PASS_READONLY_WITH_LIMITS: inserción nueva y raíces físicas homónimas no fueron observadas en IIS y conservan cobertura local. SEG-03 resuelto expresamente como operación sin limpieza; no se ejecutó borrado. La medición57archivos/24.371bytes pertenece al corte previo a la preparación posterior.

La UI final se verificó en Windows/Chromium local con fuentes reales y respuestas sintéticas, sin autenticación, IIS, MySQL ni efectos de negocio reales. No se inició otra consola ni se reutilizaron secretos. No hay nueva aceptación autenticada de esta UI ni despliegue. Las limitaciones aceptadas de SEG-02 no se eliminan por estas pruebas.

## Cambios entregados

- Menú Importación disponible al importador sin administrar perfiles; operaciones y administración separadas, con permisos servidor existentes y sin duplicación al reinicializar o reemplazar hosts.
- Menú superior toma el nivel seleccionado. Contextual captura explícitamente el nivel pulsado y ofrece «Importar carpetas aquí…» y consulta. Nombres iguales no sustituyen IDs; expediente/documento/fondo vacío no provocan fallback. Una ventana abierta mantiene su destino y ruta autorizada.
- El mismo asistente aloja consulta manual sin selección, último lote y verificación de confirmación pendiente. Conserva avisos visibles, pistas aisladas por contexto y bloqueo de incertidumbre, sin reconfirmación automática. Se retira la franja permanente.
- El plugin compartido mide el menú visible, limita altura/anchura y permite scroll, apertura Shift+F10/ContextMenu, flechas/Home/End, Enter/Espacio, Escape y retorno del foco. Conserva el primer argumento jQuery del callback y agrega captura opcional del invocador.
- Recursos modificados versionados en el ASPX. Los helpers E2E existentes abren el nuevo menú; no se añade login, endpoint, DTO ni coordinador.

## Verificación ejecutada

| Comprobación | Resultado | Alcance |
| --- | --- | --- |
| Batería cliente/editor/navegación/contextual/datos/políticas/cuentas |99/99 PASS,0omitidas | `tests/doc111-importacion-carpetas-client.test.cjs`, `doc110-perfil-importacion-editor.test.cjs`, `doc119-navigation.test.cjs`, `doc119-context-menu.test.cjs`, `doc119-acceptance-data.test.cjs`; políticas y cuentas DOC-119 existentes |
| Navegación después de precisar el rótulo «aquí…» |6/6 PASS | Repetición afectada; no se suma como casos adicionales |
| Carga individual y contratos compartidos |33/33 PASS | `tests/doc88-produccion-upload-contract.test.cjs`, `doc90-file-upload-shared-consumers.test.cjs`, `doc117-alm03-individual-ui.test.cjs`; controles estáticos/dobles existentes, sin carga real |
| Compilación Framework |0errores,308advertencias,6,60s | MSBuild Debug, BuildProjectReferences=false; advertencias existentes del proyecto |
| Visual | Consulta móvil390×640 revisada | Captura local excluida `.tmp/doc119-consulta-ui-20261112.png`; sin desbordamiento del diálogo |
| Fuentes protegidas | Sin delta en esta reorganización | SII/Integracionccv, almacenamiento, Web.config, esquema, servicios, permisos y limpieza fuera del diff de producto UI |

El primer agregado dio98PASS/1fallo por una espera del título antiguo «Resultados persistidos» después de cambiar el encabezado. Se sustituyó esa espera por encabezado accesible y estado RECIBIDO; el caso afectado pasó y se repitió la batería completa99/99. No se ocultó ni saltó el caso. Logs en `.tmp/doc119-ui-tests-final.log`, `.tmp/doc119-navigation-final.log`, `.tmp/doc119-ui-individual-regression.log` y `.tmp/doc119-ui-build.log`, excluidos de Git.

Reproducción previa del contextual: menú de952px en viewport390×260, posicióny=-707; teclado sin apertura. Tras el cambio caben los límites y se alcanza la última acción. Inventario `doc119-contextuales-inventario-20261112.json`:26 registros en22archivos. La prueba recorre cada selector/nombre con callback observado y conserva el contenedor/recurso invocador, además de cierre y reemplazo sin duplicados. No ejecuta las operaciones de negocio reales de los26 consumidores.

## Revisión OpenSpec: completitud, corrección y coherencia

| Dimensión | Resultado |
| --- | --- |
| Completitud |24/24 tareas;10/10 requisitos con evidencia identificada |
| Corrección | Autoridad servidor/contratos existentes; regresión afectada y alcances de prueba explícitos |
| Coherencia | D-01–D-10 respetadas; compuerta aceptada antes de UI, sin cambio de semántica de negocio |

| Requisito/decisión | Implementación y escenarios verificados |
| --- | --- |
| RQ-01/D-01 | Matriz de procedencia y aceptación mixta, informes SEG-01/02 con SHA/ambiente; este informe diferencia servicios sintéticos y corridas reales |
| RQ-02/D-02 | Cliente/vista transportan filtros y solicitud CSV efectiva; pruebas DOC-111 y SEG-01 real cubren totales, página filtrada, vacío, privacidad y huellas |
| RQ-03/D-03 | Referencias aisladas, consulta de confirmación sin replay, reselección por IDs y Doc111ActualizarRama; DOC-111 cubre homónimos/inserción, SEG-02 real cubre recarga/reselección/relación única y selección |
| RQ-04/D-04 | Resolución expresa `doc119-seg03-propuesta-sin-limpieza-20261112.md`; composición y datos conservados, sin borrar |
| RQ-05/D-05 | RenderImportacionCarpetas/asegurarMenu/PerfilImportacionEditor; pruebas navegación1–2 y editor cubren importador independiente y separación/reemplazo |
| RQ-06/D-06 | Doc119NivelPulsado/Doc119InvocarImportacion y abrir con nivel explícito; navegación1/3/4/5 cubre seleccionado/pulsado, homónimos, excluidos, consulta destino original y denegación sintética |
| RQ-07/D-07 | montar/sincronizar/consulta/verificación y vista única; navegación4/6 y DOC-111 cubren referencia, recarga, incertidumbre, aislamiento y reapertura/foco |
| RQ-08/D-08 | jquery.contextMenu JS/CSS;4 pruebas Chromium cubren viewport, teclado/foco, captura/reemplazo y contrato de26 registros |
| RQ-09/D-09 | Aceptación previa registrada; matriz UI local y contratos individuales; guía11 y contratos02 actualizados con lectura/efectos, incertidumbre, cierre/detención y limpieza |
| RQ-10/D-10 | Sin nuevas operaciones reales en este corte; diff acotado y pruebas políticas de presupuestos/secretos/efectos. No modifica recepción, integridad, PDF/TIFF ni almacenamiento |

Sin hallazgos críticos de implementación. Dos advertencias de cobertura: la UI reorganizada y las acciones de negocio de los otros consumidores no tienen una nueva ejecución autenticada; inserción/raíces físicas homónimas de SEG-02 conservan cobertura local expresamente aceptada. Para ampliar evidencia integrada deberá usarse un plan autorizado específico, sin repetir efectos inciertos ni recrear el lote29. Estas advertencias no se presentan como pruebas reales aprobadas ni se trasladan como defectos reproducidos a DOC-120.

OpenSpec estricto, auditoría de refinamiento, sintaxis y diff se verifican al registrar la entrega. El gobierno OPSXJ se vincula al SHA final en sus registros locales excluidos; este informe no equivale a archivo, cierre Jira, publicación ni integración a main.
