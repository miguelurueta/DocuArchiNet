# Pruebas locales DOC-105

## Corrección 13.1 — propiedad de caché, 2026-10-18

Prueba roja/verde real sobre VB compilado mediante `node --test tests/document-streaming.test.cjs`: antes, `LimpiarExpirados` borraba el fixture ajeno foreign.pdf; después conserva PDF ajenos, nombres hash sin marca, marcas legacy/vacías/incorrectas, archivos recientes, subcarpetas, temporales de generación y marcas bloqueadas. Elimina derivado y preview propios vencidos junto a sus marcas. Verifica que acceso no crea propiedad, publicación sí, no sobrescribe marca ajena y una generación activa en el proceso se conserva hasta quedar inactiva. Solo archivos sintéticos en directorio temporal exclusivo; no se usó Ghostscript, documentos reales ni credenciales.

Resultado: streaming compilado 2/2; `test:workflow:pdf-viewer-performance:policy` 74/74 (incluye contrato de publicación después de File.Move y rechazo de reparse points); `test:doc105:pdf-cross-module:policy` 70/70; `test:doc104:server-processing:policy` 34/34; `test:doc105:toolbar-dom` 1/1. MSBuild Debug termina con código 0 y advertencias existentes. La protección de reparse points se verificó por contrato estático, no se ejecutó una prueba de junction real. No sustituye E2E autenticada, ensayo operativo de rollback ni revisión global de cierre.

## Preflight del lanzador Migración convertido — 2026-10-18

Tarea 7.5.5: el lanzador local ignorado `.tmp/run-doc105-migration-converted.ps1` entregaba tools/e2e/profiles/... al validador que exige profiles/*.runtime.json. Se cambia el directorio de trabajo a tools/e2e relativo a PSScriptRoot, junto con las rutas del runner y ambos perfiles. Antes de una futura ejecución conserva copia del artefacto transversal existente (incluida evidencia SOURCE_PREVIEW); no borra ni reinterpreta resultados previos.

Preflight sin login ni red: rechazo del formato antiguo reproducido con el validador real, argumentos corregidos válidos y ambos perfiles cargados por los loaders existentes. Solo simpleHandler/migration habilitado, view=converted, mutaciones deshabilitadas y remote-100 disponible. Parser PowerShell sin errores. No se imprimen datos de selección ni secretos. Sin cambios productivos ni nueva E2E: MSBuild no aplica y 7.5.5 continúa abierta. Un posterior BLOCKED global puede coexistir con PASSED focal; revisar las filas de Migración, no inferir su fallo solo del código 2.

## Cierre focal 11.12 y 11.13 — 2026-10-18

Verificada evidencia local `tools/e2e/artifacts/workflow-pdf-viewer-performance.json`, esquema 5, capturada 2026-10-18T12:33:40.576Z: PASSED, tres muestras, 34 páginas, tiempos 2.266/350/347 ms (mediana 350). Cada snapshot conserva 390.850 bytes completados, correspondientes al preview, frente al principal de 9.646.444 bytes. Todas las muestras pasan transporte; los hitos de selección, primera página, recargas, scroll, zoom y feedback son verdaderos, sin errores clasificados. Cierra ambas tareas junto con las pruebas locales y compilación registradas abajo. Las referencias inferiores a repetición pendiente son históricas y quedan superadas por esta corrida, no por aprobación retrospectiva de los FAILED anteriores.

No se lanzó otra TTY ni se modificó producto durante esta consolidación. El archivo runtime permanece ignorado y aquí solo se conservan métricas saneadas. Esta aceptación no demuestra primera apertura de 350 ms (fue 2.266 ms), mejora uniforme, presupuesto remoto, impresión, guardado ni rollback. DOC-105 sigue sin cierre transversal.

## Tarea 11.13 — Prioridad de preview autorizada (2026-10-18)

Implementado en script/HTML, revisión toolbar-13. Arranque principal posterior a render de preview y dos requestAnimationFrame (oportunidad de pintura, no garantía física). Límite de 3 segundos para toda la operación: fallo, timeout o ausencia retorna al principal; timeout invalida callbacks, retira preview y cancela render/loadingTask sin esperar destrucción. Preview exitoso conserva canvas, contador y telemetría; no reinicia overlay ni habilita toolbar antes del render principal. No cambia permisos, chunkSize, almacenamiento ni operaciones de grafo/descarga/impresión.

Validación local: visor 73/73, DOC-105 70/70, DOM Chrome 1/1, sintaxis y OpenSpec estricto correctos; MSBuild Debug código 0 con advertencia existente MSB3247. Las pruebas de prioridad ejecutan la función productiva con reloj/promesas controlados y cubren éxito, error, ausencia y timeout en carga/render/pintura, además de eventos tardíos. No equivalen a E2E autenticada. Pendiente repetición Workflow: 11.12 y 11.13 siguen abiertas y la evidencia FAILED anterior se conserva. Reversión focal: restaurar función de preview, coordinación de arranque y revisión HTML de forma conjunta; no restaurar documentos ni configuración de negocio.

## Tarea 11.12 — Medidor focal Workflow con preview (2026-10-18)

Repetición posterior: FAILED, mediana 411 ms, 34 páginas; apertura, interactividad y feedback correctos. Snapshot de recursos completados: 10.037.294 / 6.891.566 / 10.037.294 bytes; tamaño principal 9.646.444, preview 390.850. Transporte falla en las muestras 1 y 3. Los bytes codificados no son una medida exclusiva de red ni una unión de rangos. No se altera el resultado ni se cierra 11.12.

Diagnóstico de arranque: `script.js` invoca `loadFirstPagePreview(first_page_url)` sin esperar su promesa e inmediatamente `load_pdf_viwer_anotation(...)`. Comprobación local ejecutando ese bloque real con promesa de preview pendiente confirma `PREVIEW_STARTED -> PRINCIPAL_STARTED` antes de resolver el preview. Esta prueba verifica concurrencia de inicio, no causalidad exclusiva ni rendimiento remoto. Duración agregada del handler en la repetición: 153/217/162 ms; la red rápida permite completar recursos antes del render. No demuestra que mostrar el preview dependa del principal.

Decisión propuesta, pendiente: priorizar render y oportunidad de pintura del preview antes de iniciar el principal, con espera acotada y fallback inmediato ante error o ausencia de preview. Es necesario definir ese límite y evitar que progreso tardío del preview reinicie el estado del principal. No aplicar un simple `await` ilimitado ni cambiar chunkSize, permisos o descarga para forzar aprobación. No se modificó código productivo ni se lanzó otra TTY durante este diagnóstico.

La corrida reportada conserva FAILED: tres primeros canvases, mediana 419 ms, 34 páginas, scroll/zoom y feedback aprobados, transporte rechazado. El medidor comparaba progreso completo de la representación de una página con su propio total y agregaba tráfico posterior del principal.

Corrección limitada al runner existente: snapshot inmutable al primer canvas (incluido data-ready del preview), bytes de recursos completados separados del agregado posterior, fuente explícita y comparación del preview con tamaño principal conocido y respuestas 206. Preview sin snapshot/bytes, tamaño desconocido, ausencia de 206 o recursos completados que ya alcanzan el total no aprueban. El principal completo tampoco aprueba. STREAM no se etiqueta parcial. Se espera el visor principal antes de interacciones y se conserva el tiempo anterior a esa espera.

Límite: los bytes del preview describen esa representación, no todos los bytes en vuelo del principal. Resource Timing cuenta recursos completados; no constituye unión de rangos ni contabilidad física de solicitudes aún abiertas. Este diagnóstico focal no sustituye la matriz transversal ni acredita por sí solo sus presupuestos remotos.

Validación local: `test:workflow:pdf-viewer-performance:policy` 68/68; `test:doc105:pdf-cross-module:policy` 70/70. Regresión ejecutable de snapshot inmutable y casos positivos/negativos del transporte. Sin cambios productivos en esta tarea: MSBuild no aplica. Pendiente repetición E2E autorizada; 11.12 permanece abierta y la evidencia anterior no se modifica para promoverla a PASSED.

## Aceptaciones consolidadas — 2026-10-18

Revisión posterior de la grabación de las 18:56 del 2026-10-16 (89,1 s) resuelve el pendiente de reapertura de 12.10/12.11: overlay con página girada a 62 s, grafo sin controles a 68 s; recarga de la aplicación a 76 s, navegación del árbol a 80 s y documento reabierto a 84–88,8 s. Se conservan tres grafos en posiciones relativas reconocibles, tamaños aparentes y orientación correspondiente al giro; el contenido subyacente sigue visible. Las pruebas locales previas cubren conversión geométrica y configuración. Aceptación manual focal, sin medición subpíxel, verificación de todas las tolerancias, auditoría ni conteo POST. No se ejecutaron escrituras nuevas. Las menciones inferiores a falta de reapertura quedan superadas por esta grabación, no por los videos anteriores.

Revisión retrospectiva adicional: 12.4 completa con grabación de carga del 2026-10-16 (48,73 s): a 2/3 s placeholders y overlay con controles atenuados; a 9,7 s página y contador visibles con controles aún atenuados; a 16 s descarga en preparación y a 29,1 s zoom utilizado. Secuencia visual complementada por pruebas locales, no tiempos instrumentados ni revisión de cada fotograma.

12.12 completa con grabación de scroll del 2026-10-16 (87,13 s): a 17,4 s grafo manipulable; a 34,7 s documento desplazado; inserciones y avance de páginas a 52,1/69,5/86,8 s conservan cabecera y X. No se observó rotación en las muestras; aceptación limitada al recorrido y pruebas DOM existentes.

Grabación de transparencia del 2026-10-16 (35,93 s): grafo editable a 14,3 s y resultado sin barra a 35,6 s deja ver texto subyacente, pero cambia tamaño frente al overlay. Evidencia histórica anterior al ajuste geométrico, no fallo nuevo de la revisión vigente. No muestra cierre/reapertura posterior; 12.10/12.11 permanecen abiertas. Sin nuevas ejecuciones ni capturas documentales versionadas.

12.14: confirmación explícita del usuario posterior al ajuste de posición inicial/controles: grafo funcional. Aceptación manual acotada a esa tarea, no a persistencia de geometría/transparencia ni a auditoría de POST.

12.15: video final revisado, duración 40,9 s. Aviso y botón visualmente deshabilitado a 8,1 y 16,2 s; diálogo abierto y aviso detrás a 24,4 y 32,5 s; vista previa visible a 35 s; cancelación entre 38 y 39 s retira aviso y restaura apariencia habilitada hasta el final. No se observó un nuevo clic después de cancelar ni se midió tráfico en el video. Junto con las pruebas locales anteriores, confirma el ajuste focal de espera/cancelación; no acredita impresión física, menor latencia ni todos los navegadores. Tarea 12.15 completa. Las referencias a confirmación pendiente en su historia inferior quedan superadas por esta revisión.

Consolidación exclusivamente documental: no se repitieron pruebas reales ni compilación. Estado completo y pendientes vigentes en [errores-pendientes.md](errores-pendientes.md).

## Preparación de impresión — tarea 12.15

Revisión toolbar-12: el video posterior muestra aviso al segundo 1, retirado al 7,5, diálogo entre 17 y 20, vista previa aún cargando al 30 y visible al 37,5. La primera corrección no cumple la espera completa. onLoadingEnd deja de finalizar la promesa; solo cierre del diálogo, fallback incompatible o error libera el control. Se prueban microtareas tras fin de carga sin liberar, duplicado durante transición, cancelación y callback tardío anterior sin desbloquear la siguiente operación. Suite visor 65/65 y DOM Chrome 1/1 aprobadas. Pendiente repetición real incluida cancelación: PrintJS usa foco/mouseover como heurística de cierre, no confirmación física. Sin impresión real ni mejora de latencia afirmada. El párrafo siguiente describe la revisión anterior, sustituida en este punto.

La ventana finalmente apareció según el usuario: espera prolongada, no bloqueo permanente demostrado. El flujo conserva copia temporal servidor y descarga completa de PrintJS antes del diálogo; no reutiliza automáticamente los rangos de visualización. Se corrigen falta de aviso, listener solo en icono y ausencia de promesa retornada desde el motor. Ahora el botón completo/teclado muestra “Preparando impresión… Espere, por favor.”, bloquea duplicados durante carga de capacidad, servicio y PrintJS, y se libera al terminar o fallar. El aviso se retira sin afirmar impresión física. Error saneado; onLoadingEnd se difiere una microtarea para priorizar onError de la biblioteca. Permisos y operación Imprime intactos. Recursos toolbar-11.

Validación local sin autenticación ni impresora: suite de visor 65/65; DOM Chrome 1/1 incluyendo borde, icono, Enter, Espacio y bloqueo duplicado; guardado/servidor 34/34; sintaxis JS, MSBuild y OpenSpec estricto correctos. Pruebas controladas no acreditan impresión real. Pendiente confirmar visualmente con PDF pesado; tarea 12.15 permanece abierta. No se afirma mejora del tiempo de preparación. Revertir este ajuste requiere restaurar conjuntamente coordinador, motor, HTML y estilos de la revisión previa.

## Posición inicial y controles del grafo — tarea 12.14

El video muestra cambio de zona de lectura al insertar y acciones poco distinguibles sobre contenido denso. Causas: posición calculada desde altura del contenedor sin scroll de página, y opacity 0.5 aplicada al formulario completo. Se centra el formulario en la intersección visible página/viewport, convirtiendo coordenadas a la página; no se desplaza la lectura desde toolbar. La carga de imagen actualiza dimensiones y centra solo antes de interacción; un pointerdown impide recentrado tardío y un nodo retirado se ignora. Coordenadas explícitas conservan revelado interno. La imagen usa opacity 0.65 solo para previsualización; controles opacos de contraste alto, Guardar con texto y botones nativos accesibles. Sin cambios de transparencia servidor, permisos ni contrato de guardado. Recursos toolbar-10.

Validación local: guardado/servidor 34/34, visor 57/57, MSBuild Debug código 0, sintaxis y OpenSpec válidos. Chrome DOM comprueba scroll vertical/horizontal sin cambio, centrado con cuatro escalas CSS (0.5/1/1.25/2), carga real de imagen sintética y controles opacos. Se verifica además con carga controlada tardía que una firma manipulada no se reposiciona y una retirada no se recrea. Adaptadores de plugins son dobles locales, no prueban arrastre/redimensionado reales ni escritura documental. Pendiente validación visual: Ctrl+F5, abrir PDF, desplazarse a otra zona, insertar y comprobar posición/contraste; no es necesario guardar para esa comprobación. Rollback: restaurar coordinadamente motor, estilos y revisión de recursos de esta tarea.

## Diagnóstico horizontal de Producción — tarea 12.13

Aceptación posterior recibida y contrastada con artefacto local: PASSED, código 0. Zoom 1.25/1.5 sin desbordamiento; 2/3 alcanzan rangos 49/661 px. En los cuatro zooms bottomVisible y ancestorBottomVisible son true, recorte de viewport ancestral cero. El iframe pasó de 825 a 793 px, reservando 32 px de cabecera. Se completa 12.13 exclusivamente para Producción, sin afirmar aprobación de la matriz global.

E2E autorizada inicial: a zoom 1.25/1.5 no había desbordamiento; a 2/3 el rango fue 49/661 px y scrollLeft alcanzó 48.8/660.8 px, con gutter de 9 px. El borde cabía en el viewport interior. El resultado global focal fue FAILED. Se descarta como medida fiable el recorte de ancestros de 901.15 px: la primera instrumentación trataba todos los ancestros overflow:hidden como recortadores incluso de descendientes posicionados. Se sustituye por hit testing del borde inferior proyectado por cada iframe, considerando escala y borde, y se añade regresión con modal fixed bajo ancestro estático de altura cero.

Causa productiva localizada: auto_zise_popup_visor_externo asignaba la misma altura al modal, cuerpo e iframe, sin descontar cabecera. Corrección limitada a Producción: wrapper flex interno con cabecera no contraíble, cuerpo min-height:0 y UpdatePanel/iframe al 100% restante, iframe block sin borde. El JS solo calcula altura del modal; revisión de recurso doc105-layout-1. No modifica el visor compartido ni la geometría de firmas. Prueba DOM ejecuta fragmento ASPX adaptado a HTML y función real de tamaño: falla antes y pasa después en alturas 880/600/1000 y apertura inicialmente oculta. Repetición autenticada pendiente. Rollback: revertir conjuntamente el fragmento del modal, su regla UpdatePanel, función de tamaño y revisión del script de Producción; no revierte documentos.

Se agrega `--diagnose-horizontal-layout` al runner existente, con Producción exclusiva, mutación deshabilitada, sesión y selección oficiales. Una apertura y cuatro niveles de zoom mediante botón; viewport 1500x880. Calcula clientWidth/scrollWidth, rango y extremo alcanzado con scrollLeft (restaurado), gutter, borde inferior y recorte por ancestros/iframes. Solo números y booleanos; no capturas ni documentos. Sin red emulada ni matriz completa. Se aborta el endpoint de persistencia de grafos como defensa adicional.

Validación tras corrección: DOC-105 70/70, visor 57/57, MSBuild Debug código 0, sintaxis JS y OpenSpec estricto válidos. La suite incluye política de consumidor/mutación, DOM Chrome con iframe recortado, ancestro estático no recortador, conservación del scrollLeft y dimensionamiento real del modal. Resultado autenticado posterior al ajuste pendiente de la TTY autorizada; la modificación productiva se limita al modal de Producción.

## Modal estable al insertar grafo — tarea 12.12

La revisión visual posterior detectó que, al insertar otro grafo, desaparecen la cabecera blanca y el cierre del modal aunque el PDF continúa visible. Esto invalida una conclusión de recorrido completamente correcto: la mejora geométrica no cubría el contenedor anfitrión.

Causa reproducida localmente: `_Add_interface_anotate` llamaba a `scrollIntoView()` cuando el grafo quedaba fuera del viewport. En Chrome, con iframe dentro de modal recortable, esa llamada desplaza también los ancestros; la cabecera pasó de y=8 a y=-72. La misma prueba con el ajuste conserva posición y cierre. Se usan exclusivamente scrollTop/scrollLeft del contenedor del visor, con desplazamiento mínimo por eje; se comprueba nuevamente al cargar la imagen y se ignoran nodos retirados. Sin acceso al documento padre, sin cambios CSS globales, sin alterar guardado, permisos, transparencia ni coordenadas del grafo. Recursos coordinados toolbar-9.

Validación local: `npm.cmd --prefix tools/e2e run test:doc104:server-processing:policy` 34/34 y `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy` 57/57. MSBuild Debug código 0. La prueba DOM ejecuta el método real de inserción sobre HTML sintético, con adaptadores vacíos de plugins de arrastre/redimensionado: comprueba inserciones repetidas, ambos ejes, cabecera inmóvil, grafo visible sin scroll adicional y nodo retirado. No es una E2E autenticada ni modifica documentos reales.

Pendiente de aceptación: recargar aplicación con Ctrl+F5 y reabrir visor; desplazarse por la página, insertar un grafo, repetir con la página girada y comprobar que la cabecera y la X siguen accesibles. Basta insertar/cancelar para verificar este fallo, sin guardar un documento. No se requiere repetir toda la matriz. Rollback localizado: restaurar el método de inserción y la revisión coordinada HTML/JS; no modifica grafos ya persistidos.

## Geometría del grafo — tarea 12.11

Causa localizada: el cliente enviaba tamaño real pero el servidor imponía ScaleAbsolute(90,90); además restaba altura CSS sin convertir a coordenadas PDF y usaba una razón inversa para zoom reducido. El cliente medía posición del contenedor, no de la imagen. El video con transparencia habilitada confirma que quitar blanco funciona en ese recorrido, pero el grafo crece hacia arriba al guardar; no prueba reapertura ni POST único.

Corrección: getBoundingClientRect de imagen y canvas para posición/tamaño, sin barra ni borde exterior; matriz afín pura GeometriaGrafoPdf para convertir las cuatro esquinas y orientación. Integración usa intersección CropBox/MediaBox y RotateContents=False porque este visor llama getViewport con rotación explícita, no acumulada al /Rotate. No se modificaron transparencia, fuentes de firma, permisos, auditoría, bloqueo transaccional ni reemplazo seguro. No se reparan grafos anteriores. Revisión coordinada toolbar-8.

Validación: `npm.cmd --prefix tools/e2e run test:doc104:server-processing:policy` 33/33, incluyendo helper VB compilado con 48 combinaciones (cuatro zooms, cuatro giros y tres orígenes), rechazo de dimensiones inválidas, y Chrome DOM con scroll, escalado CSS y DPR 2. La prueba DOM captura el contrato sin llamar al servicio; no representa un guardado documental real. `test:workflow:pdf-viewer-performance:policy` 57/57, sintaxis JS, OpenSpec estricto y MSBuild Debug código 0.

Pendiente: confirmar en un PDF descartable que la posición y dimensiones antes/después de guardar coinciden y permanecen al cerrar/reabrir. No se ejecutó E2E autenticada ni se alteró documento real. Rollback de este ajuste requiere restaurar coordinadamente cliente, integración servidor y registro/helper de geometría; no revierte anotaciones ya persistidas.

## Configuración de fondo transparente — tarea 12.10

El video del grafo muestra fondo blanco superpuesto al texto tras guardar y una posible diferencia de tamaño/posición, aún no corregida ni medida. La configuración real de la cuenta no es visible: no se afirma que el video pruebe una falla del algoritmo de transparencia. El servidor ya elimina colores cercanos al blanco cuando la opción está habilitada y conserva la imagen fuente. Sin configuración de usuario el valor existente es deshabilitado, tolerancia 10; no se cambió ese predeterminado.

Defecto localizado del cliente: la actualización reemplazaba el arreglo de configuración por una respuesta sin catálogo, mientras la promesa de carga conservaba el arreglo anterior; reabrir mostraba opciones viejas y acumulaba duplicados. Ahora se actualizan los dos valores sobre el objeto compartido solo ante confirmación exitosa y se limpia el selector al abrir. Fallo o respuesta inválida conserva estado anterior y solicita verificar mediante reapertura del visor, sin mostrar errores crudos. La etiqueta explica tolerancia, no porcentaje, y advierte sobre pérdida de trazos claros. No se alteraron procesamiento servidor, coordenadas, contratos de guardado ni fuentes de firma. Recursos toolbar-7.

Pruebas: `test:doc103:graph-save:policy` 19/19 (incluye actualización, reapertura, catálogo único, rechazo saneado y regresión single-flight); `test:workflow:pdf-viewer-performance:policy` 57/57; sintaxis del motor, OpenSpec estricto y MSBuild Debug código 0. Pruebas locales sin sesión, servicio real ni documentos reales. No demuestran resultado gráfico final ni persistencia real de esta configuración.

Comprobación pendiente: Ctrl+F5, desplegable del visor → Parámetros del grafo → Quitar fondo blanco al guardar el grafo, tolerancia inicial 10, Aceptar; reabrir parámetros y confirmar elección, guardar una vez en PDF descartable, cerrar/reabrir y comprobar trazos y líneas subyacentes. La vista previa puede conservar blanco: el procesamiento ocurre al guardar. No aplicar sobre grafos ya grabados ni asumir corrección del cambio de tamaño observado.

## Retiro de confirmación de descarga — tarea 12.9

Video revisado: la confirmación de entrega al navegador permanece sobre el documento mientras se continúa la lectura. La causa es ausencia de retiro del aviso final, no evidencia de una preparación pendiente. El visor ahora oculta y limpia esa confirmación a los cinco segundos. Al iniciar otra descarga cancela e invalida el temporizador anterior, incluso frente a callbacks obsoletos; la espera permanece hasta resolver la preparación. Avisos de error y permiso se conservan. No se modifica el servicio ni se afirma finalización en disco. Recursos coordinados toolbar-6.

Validación local: `test:workflow:pdf-viewer-performance:policy` 57/57, incluyendo temporizador y carreras; `test:doc105:toolbar-dom` 1/1 en Chrome con retiro real del aviso y botón habilitado; `node --check pdfjs/pdf_anotate_view/script.js`, OpenSpec estricto y MSBuild Debug código 0. No se ejecutó matriz autenticada ni descarga documental real. Confirmación visual en ambiente pendiente; este ajuste no cierra los pendientes globales de DOC-105.

## Descarga — tarea 12.8

Video revisado muestra entrega de descarga y aviso del navegador sobre múltiples descargas, sin espera visible en el visor. Se añadió mensaje accesible no modal y bloqueo nativo del botón completo durante preparación. La coordinación comparte una promesa pendiente; `_DonwloadPage` retorna la promesa del servicio conservando permiso, endpoint, payload y operación existentes. Respuesta inválida o fallo de transporte producen aviso saneado y liberan botón, sin reintento automático. Advertencia de auditoría no se imprime cruda. Recursos toolbar-5.

El bloqueo cubre preparación y entrega del enlace al navegador: no observa guardado en disco, no altera restricciones de descargas múltiples y no añade fetch completo del PDF. Se conserva el helper común `dowload_file`. Mensaje final: «Descarga enviada al navegador. Revise sus descargas».

Pruebas locales: visor 56/56; DOM Chrome 1/1 (botón completo, estado visible, duplicados bloqueados y liberación); grafos 18/18; MSBuild Debug código 0; sintaxis JS y OpenSpec estricto válidos. Ninguna prueba autenticó, descargó documentos reales ni llamó al servicio de auditoría. Confirmación operativa pendiente.

## Rotación — tarea 12.7

Botones de giro conectados al contenedor nativo completo, nombre accesible y teclado, manteniendo IDs interiores y bloqueo de carga. `rotate` conserva número de página como ancla durante giros consecutivos; inicialización aplica zoom y posiciona inicio de esa página antes de calcular visibles. Se elimina la restauración proporcional al documento completo. Un identificador de inicialización descarta respuestas antiguas sin confundirlas con cambios de generación de render. Los rechazos se presentan por el mecanismo existente de error.

No se promete mantener la coordenada exacta del papel ni se elimina en este ajuste la reconstrucción de contenedores. Recursos coordinados toolbar-4. Pruebas: `test:doc105:toolbar-dom` 1/1 con borde/icono/Enter/Espacio de ambos giros y zoom; `test:workflow:pdf-viewer-performance:policy` 53/53, incluyendo ancla de página 12 y finalización inversa de giros; `test:doc103:graph-save:policy` 18/18. La política de propietario único acepta query de versión sin permitir dos scripts. MSBuild Debug código 0; comprobación sintáctica JS y OpenSpec estricto válidos.

Son pruebas locales, no confirmación del PDF real ni medición de latencia. Revisión visual final pendiente en 12.4, sin repetir matriz ni realizar escrituras documentales.

## Corrección área de zoom — tarea 12.6

El listener estaba en el icono interior: el botón y su activación por teclado no recibían la acción. Se trasladaron solamente los listeners de zoom a `closest("button")`, manteniendo IDs interiores, comprobación de disponibilidad, permisos y planificador. Los dos botones tienen nombre accesible y `type=button`; recursos coordinados con revisión `doc105-toolbar-3`.

`npm.cmd --prefix tools/e2e run test:doc105:toolbar-dom` ejecuta una prueba DOM local en Chrome, sin autenticación, documentos ni tráfico externo. Usa HTML y handlers productivos y un receptor de llamadas de zoom; no mide render PDF. Antes de corregir, solo una de cuatro activaciones llegó al receptor; después pasan borde, icono, Enter y Espacio para ambos botones, exactamente una llamada cada vez, y cero durante carga/error. Resultado 1/1. Regresión `test:workflow:pdf-viewer-performance:policy`: 52/52. MSBuild Debug código 0; JavaScript comprobado con `node --check`.

La confirmación visual con PDF pesado sigue pendiente en 12.4. No se declara corregida ni medida la latencia de renderizado a partir de estas pruebas.

## Seguimiento 2026-10-16 — tarea 12.5

Video posterior confirma barra bloqueada incluso con contador y navegación entre páginas operativos. Las pruebas anteriores no acreditaban el recorrido real del usuario. Se identificaron referencias sin versión a CSS y ambos scripts del visor, compatibles con mezcla de HTML nuevo y coordinación anterior en caché; hipótesis, no causa confirmada por tráfico del ambiente. Se añadió revisión común `doc105-toolbar-2` a esos tres recursos para invalidar sus URLs anteriores sin tocar caché/autorización de documentos.

Suite del visor: 52/52, incluyendo ejecución de los callbacks productivos de metadatos, preparación y render mediante unidad local, además del contrato de revisión común. MSBuild Debug código 0 y OpenSpec estricto válido. La unidad no reproduce ni aprueba el ambiente autenticado. Tarea 12.4 continúa pendiente de repetir apertura después de recargar el HTML; no se asegura resolución del bloqueo real sin esa comprobación.

## Actualización 2026-10-16 — tareas 12.1 a 12.3

Regresión observada en video: el preview muestra primera página mientras los dos campos del contador siguen vacíos. Causa: valores iniciales vacíos y actualización exclusiva por cambio de página activa. Corrección: placeholders accesibles, página 1 desde preview, total real desde `onDocumentMetadata`, y fieldset deshabilitado hasta primer render principal. El error posterior al preview permanece visible y bloquea herramientas; las capacidades diferidas revisan disponibilidad antes y después de cargar. Los bloqueos individuales y permisos existentes no se alteran. Preview y visor comparten escala inicial 1.25; no se añadió `getData()` ni espera de descarga completa.

- `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy`: PASSED, 50/50 (incluye 8 pruebas de estados de carga).
- `npm.cmd --prefix tools/e2e run test:doc105:pdf-cross-module:policy`: PASSED, 67/67.
- `npm.cmd --prefix tools/e2e run test:doc103:graph-save:policy`: PASSED, 18/18.
- `node --check` para `script.js` y `pdf_viewer_anotate.js`: código 0.
- `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /nologo /v:quiet`: código 0, advertencias existentes de ensamblados.
- `openspec.cmd validate doc-105-regresion-transversal --strict`: válido.
- Comprobación DOM local en Chrome sin autenticación/documentos: bloqueo nativo inicial, preview bloqueado, preservación de botón individual deshabilitado y bloqueo ante error, PASSED. No sustituye la E2E real.

Tarea 12.4 pendiente: confirmar visualmente el PDF pesado en ambiente autorizado. No se repitió la matriz completa y no se promueven sus bloqueos previos a éxito.

## Historial anterior

Fecha de última ejecución: 2026-10-15. Estas pruebas no autenticaron contra un ambiente, no abrieron documentos reales y no realizaron mutaciones.

| Comando | Resultado | Alcance |
| --- | --- | --- |
| `npm.cmd --prefix tools/e2e run test:doc105:pdf-cross-module:policy` | `PASSED`, 47/47 | Perfil cerrado, navegación automática de Producción, presupuestos útiles, transporte de la vista previa, contratos, red, caché y evidencia saneada |
| `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-loading-state:policy` | `PASSED`, 4/4 | Accesibilidad, progreso real/indeterminado, primer canvas, error y ausencia de descarga ansiosa |
| `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy` | `PASSED`, 45/45 | Regresión DOC-100/DOC-101/DOC-102, vista previa de primera página, carga visible y compatibilidad del runner reusable |
| `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-streaming:policy` | `PASSED`, 14/14 | Referencias, eficacia estructural, vista previa en primera apertura, derivado temporal, handler y consumidores |
| `npm.cmd --prefix tools/e2e run test:doc103:graph-save:policy` | `PASSED`, 18/18 | Single-flight, un POST lógico, SELECT y evidencia saneada |
| `npm.cmd --prefix tools/e2e run test:doc104:server-processing:policy` | `PASSED`, 29/29 | Transacción PDF, temporales, clasificación y regresión DOC-103 |

## Compilación

Corrección Word en Producción, tarea 7.5.2: `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-streaming:policy` pasó 15/15, incluyendo exclusión de formatos no PDF de la rama express y conservación del fallback de descarga. `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /nologo /v:quiet` terminó con código 0 y advertencias existentes. Videos del usuario del 2026-10-16 confirman descarga DOCX completada y apertura en Word, sin PDF.js ni error. Comparación SHA-256 de solo lectura: la copia abierta y la recién descargada son idénticas (91.272 bytes); no es una comparación contra el original del servidor. Tarea 7.5.2 completa; la matriz HTTP automatizada sigue pendiente. No se incluyen capturas, hashes ni identificadores documentales en la evidencia versionable.

Tarea 7.5.3, validación local del 2026-10-16: `npm.cmd --prefix tools/e2e run test:doc105:pdf-cross-module:policy` pasó 57/57. Incluye selección explícita `docuarchiDownload`, rechazo de clases ajenas, finalización de descarga, conteo de bytes, HTTP/longitud, timeout, archivo vacío y limpieza del temporal propio. Son pruebas locales; no acreditan ejecución autenticada. Sin cambios productivos adicionales, no se requiere otra compilación MSBuild en esta etapa.

`GestionDocumental-Docuarchi.net.vbproj`: `PASSED` con código 0 y 0 errores después de integrar la vista previa de una página, la validación estructural y los tres emisores. La compilación conserva advertencias históricas no relacionadas.

## Corrección de primera página útil

La primera apertura genera de forma acotada una representación temporal de una sola página y la entrega mediante una referencia de sesión separada. El visor muestra ese canvas únicamente después de terminar su render y lo retira cuando llega el primer canvas del documento completo. La generación del derivado completo continúa en segundo plano; el original, `urimage_format` y el almacenamiento no cambian.

La política rechaza un derivado por `/Linearized` nominal cuando `/L` no coincide con el tamaño o `/E` supera 32 MiB o el 50 % del documento. Cada red exige además tiempo, bytes y solicitudes máximos antes del primer canvas. La medición comienza al ejecutar la acción de abrir el documento, no al entrar al módulo.

## Ejecución autenticada

- Workflow: `PASSED` en ocho celdas, 10/20/50/100 Mbps y caché fría/caliente.
- Corrección focal Workflow `remote-100`: `PASSED` en frío/caliente, `RANGE`, HTTP 206 y parcialidad real antes del primer canvas.
- Validación del estado visible de carga: `PASSED` en tres iteraciones; indicador observado antes del canvas, progreso real determinado/indeterminado y ocultamiento posterior al render.
- Producción `remote-100`: `PASSED` en frío/caliente con navegación automática, 1.929/1.174 ms hasta la vista previa, 1.629.367 bytes, 6 solicitudes, HTTP 206 y `RANGE`.
- Producción, matriz final: `PASSED` en ocho celdas 10/20/50/100 Mbps, fría/caliente, con 1.479–4.284 ms, 1.629.367 bytes físicos y 5–6 solicitudes antes del primer canvas.
- Otros formatos Workflow: `PASSED` en `remote-100` frío/caliente; selección automática de tarea/documento, HTTP 200, `STREAM`, una solicitud por celda, capacidades PDF.js `NOT_APPLICABLE` y cero mutaciones.
- Resultado transversal: `BLOCKED` por consumidores no autorizados en la corrida aislada; no representa un fallo de Producción.
- Mutaciones: `NOT_APPLICABLE`; no se guardó grafo.

Los adaptadores ya están implementados y cubiertos localmente: Correspondencia selecciona contexto y documento y valida el handler nativo sin exigir PDF.js; Producción abre ancestros colapsados, selecciona cada nivel y expediente, y usa su grilla y botón propios; aprobación espera el postback, atraviesa el iframe contenedor autenticado y reconoce visor enriquecido, handler o clásico; el handler simple valida transporte sin exigir PDF.js ni scroll/zoom. Las E2E autorizadas de Workflow, Producción y Aprobación/externo pasaron las ocho celdas; Correspondencia pasó `remote-100` frío/caliente.

## Estado visible de carga

El iframe incluye un overlay desde el HTML inicial, con spinner, texto accesible y barra alimentada por `loadingTask.onProgress`. Si PDF.js no conoce el total, la barra permanece indeterminada; si lo conoce, muestra un porcentaje real acotado. El overlay se oculta únicamente después de `onPageRender`, y los eventos tardíos no lo reabren. La llamada de arranque a `pdf.getData()` fue retirada porque su resultado no se usaba y podía forzar transferencia adicional.

La E2E autenticada de solo lectura terminó `PASSED`: mediana del primer canvas de 630 ms en tres iteraciones, 34 páginas, una tarea larga, modo `RANGE`, política de transporte aprobada, scroll y zoom completos. Las tres muestras observaron el overlay antes del canvas, estados determinado e indeterminado, retiro después del canvas y ningún estado de error. La evidencia no conserva cuenta, tarea, documento, URL ni credenciales.

## Pendientes

Revisión 7.5.7: suite DOC-105 67/67 tras comprobar que las dos referencias legacy de Preview externo Gestión se sobrescriben antes de render. Clasificación NOT_APPLICABLE acotada a esas rutas. Sin cambios productivos ni E2E adicional; MSBuild no requerido por este ajuste de prueba/documentación.

Resultado posterior de 7.5.6: E2E autenticada remote-100 fría/caliente PASSED para el original desde Migración. HTTP 200, STREAM, 48.691 bytes declarados y una solicitud por celda; sin errores clasificados. Menú, gabinete, búsqueda por campo y documento seleccionados automáticamente. SOURCE_PREVIEW no sustituye la validación pendiente del destino convertido (7.5.5). Sin nuevas escrituras documentales ni cambios productivos.

Migración (7.5.5): adaptador de consulta implementado y política DOC-105 64/64. Comprueba gabinete exacto, matrícula exacta, prefijo documental único y existencia de representación migrada. Correlaciona la respuesta HTTP del destino para no aprobarlo con los bytes del original. No ejecuta conversión ni reemplazo. Perfil operativo ignorado, sin cuenta ni contraseña. Pendiente E2E fría/caliente; sin cambios productivos adicionales.

Versiones (7.5.4) completa: la suite `test:doc105:pdf-cross-module:policy` pasa 61/61. La ejecución autenticada posterior pasó `remote-100` fría/caliente: HTTP 200, `STREAM`, 182.480 bytes declarados, una solicitud por celda y sin errores clasificados. Selección automática de plantilla, comprobación del radicado previamente asignado, documento exacto e historial; no asigna ni restaura/escribe versiones. Guardado de grafo `NOT_APPLICABLE`, POST de grafo=0. Sin cambios productivos adicionales; resultado global todavía `BLOCKED`.

El adaptador `docuarchiExternal` incorpora selección de gabinete, consulta general y comprobación exacta y única de la columna autorizada antes de abrir el visor externo. La suite DOC-105 pasa 53/53. La E2E `remote-100` fría/caliente pasó: HTTP 200, `STREAM`, 278.960 bytes, una solicitud por celda y sin errores clasificados; guardado `NOT_APPLICABLE`, POST=0. La subtarea 7.5.1 queda completa. Ese tamaño no acredita rendimiento de PDF pesado. Los datos de selección se reciben mediante `--docuarchi-selection profiles/<seleccion>.runtime.json` con campos `cabinetName`, `searchField` y `searchValue`, sin secretos.

- Descargas Docuarchi, tarea 7.5.3: E2E autorizada `remote-100` fría/caliente `PASSED`, HTTP 200, `STREAM`, 91.272 bytes completos y una solicitud por celda. Sin errores clasificados; guardado `NOT_APPLICABLE`, POST de grafo=0. Este resultado posterior cierra el pendiente HTTP registrado arriba; no cambia la cobertura global a `PASSED`.
- Perfiles 10/20/50 Mbps de Correspondencia; matriz de Migración y resolución de aplicabilidad de Preview externo Gestión.
- Único guardado de grafo sobre tarea/PDF descartables.
- Ensayo de rollback y smoke test posterior.
