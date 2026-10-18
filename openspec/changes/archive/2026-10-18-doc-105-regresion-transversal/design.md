## Contexto

DOC-105 no introduce un tercer modo de carga ni reemplaza el almacenamiento actual. Cierra transversalmente DOC-100 a DOC-104 demostrando, con el mismo PDF y evidencia saneada, que `STREAM` y `RANGE` conservan permisos y comportamiento en todos los consumidores reales.

La implementación productiva ya converge en `workflow/Handler_image_wf.ashx.vb`; los constructores enriquecidos están en `Docuarchi/ClassVisualisaDocumento.vb`, `workflow/ClassWorflowVisor.vb` y `Gestion/ClassGaProducionDocumental.vb`. El visor PDF.js aplica la política en `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js` y la selección servidor está en `Infrastructure/Documentos/Streaming/EstrategiaCargaPdf.vb`.

## Flujo técnico bajo prueba

```text
Consumidor WebForms
  -> constructor enriquecido o URL simple
  -> Handler_image_wf.ashx
  -> autorización y resolución del documento
  -> EstrategiaCargaPdf
       |-- PDF linealizado >= 1 MiB -> RANGE (206)
       |-- derivado linealizado disponible -> RANGE (206)
       `-- primera carga/no configurado/error -> STREAM original (200)
  -> PDF.js o descarga directa
  -> métricas comunes + resultado por consumidor
```

DOC-105 modifica inicialmente solo `tools/e2e`, artefactos OpenSpec y documentación. Una corrección productiva queda autorizada únicamente si una regresión reproducible señala un archivo concreto; después debe repetirse la celda afectada y su regresión focal.

## Decisiones

### D-01 — Inventario cerrado de consumidores

La matriz se construirá desde llamadas reales, no desde nombres de módulos. Incluirá: Workflow principal y Gestión de Correspondencia vía `Visualiza_documento_workflow_visor`; Producción Documental vía `ClassGaProducionDocumental`; aprobación/externo autenticado vía `ClassWorflowVisor`; y consumidores simples de `Handler_image_wf.ashx` en Docuarchi, Gestión, versiones, migración y otros formatos. La llamada de Correspondencia se clasifica por su comportamiento efectivo: pasa `anula_visor_espress = 1` y usa el handler dentro del visor nativo de solo lectura, no PDF.js. Cada fila tendrá ruta de entrada, tipo de visor, capacidad esperada y estado `PASSED`, `FAILED`, `BLOCKED` o `NOT_APPLICABLE`.

### D-02 — Un orquestador DOC-105 sobre la infraestructura E2E existente

Se añadirá `tools/e2e/scripts/run-doc105-pdf-cross-module-interactive.cjs`, sin login, configuración Playwright ni `.env` paralelos. Reutilizará la sesión autenticada, validadores, selección y saneamiento existentes. El perfil público de ejemplo será no sensible y el perfil real permanecerá ignorado por Git.

### D-03 — Adaptadores de navegación separados de la medición

El orquestador tendrá un registro cerrado de adaptadores: `workflow`, `correspondence`, `production`, `approvalExternal` y `simpleHandler`. Cada adaptador solo abre/selecciona su consumidor y declara capacidades. Correspondencia entra por `WebForm_interface_gestion_tramite.aspx`, selecciona el contexto y luego el PDF, pero valida el `src` del iframe y la respuesta de `Handler_image_wf.ashx`; no espera canvas ni controles PDF.js porque la ventana es deliberadamente de solo lectura y anula el visor express. Producción entra por `WebFormProducionDocumental.aspx`, selecciona una fila documental y usa su botón propio; aprobación entra por `WebFormGaconsultaclasificacion.aspx` y atraviesa el iframe contenedor antes del iframe PDF; el handler simple conserva la entrada WebForms que estableció la sesión y no presupone PDF.js. Un medidor compartido captura lienzo inicial, `domInteractive`, tareas largas, scroll y zoom cuando PDF.js aplica, o transporte HTTP cuando el consumidor usa directamente el handler, evitando cinco implementaciones distintas de métricas.

### D-04 — Perfiles remotos reproducibles y caché controlada

La emulación usará CDP de Chromium antes de abrir el documento, con perfiles de bajada equivalentes a 10, 20, 50 y 100 Mbps y latencia explícita en el perfil. Una corrida fría crea contexto nuevo; una caliente conserva contexto, sesión y caché privada autorizada. No se compararán navegador, PDF, ambiente o perfil de red diferentes como si fueran un antes/después válido.

### D-05 — Bytes únicos y físicos se calculan por separado

Para respuestas `206`, los intervalos de `Content-Range` se normalizarán y su unión dará bytes únicos. Los bytes físicos serán la suma declarada/transferida observada, incluyendo solapamientos. Solo se afirmará carga parcial cuando exista `206`, longitud total conocida y la unión recibida antes del primer lienzo sea menor al documento. Un `200 STREAM` nunca se etiquetará como descarga parcial aunque el navegador reporte menos bytes transferidos.

### D-06 — Cobertura honesta y evidencia saneada

Cada celda conservará estado y razón. La matriz global solo será `PASSED` si todas las filas obligatorias ejecutables pasan; cualquier consumidor no ejecutado quedará `BLOCKED` y prohibirá afirmar cobertura total. La evidencia no contendrá URL, ruta, título, identificador, payload, cookie, token, credencial ni cadena de conexión.

### D-07 — Una sola mutación controlada

Solo Workflow ejecutará guardado de grafo, exactamente una vez, sobre PDF/tarea descartable y con autorización específica. Se reutilizará el escenario protegido de DOC-104, se verificará un solo POST y se permitirán exclusivamente consultas `SELECT` sobre `workflowdocument`. Los demás consumidores serán de lectura.

### D-08 — Presupuestos derivados de evidencia comparable

El informe separará local/remoto y frío/caliente. Publicará mediana de primer lienzo, `domInteractive`, tareas largas, bytes únicos/físicos y bloques máximos de scroll/zoom. Los 3.548 ms de scroll y 435 ms de zoom son umbrales históricos de regresión, no metas inventadas; el presupuesto final se fijará después de la matriz con el mismo PDF y condiciones declaradas.

### D-09 — Despliegue gradual sin gate legacy

DOC-105 entregará orden de despliegue, observación y reversión por archivos de DOC-100 a DOC-104. No reintroducirá `WorkflowCentroTrabajoModernActive`, usuarios o grupos como activación. No activará producción sin autorización adicional y probará que el procedimiento de rollback restaura el comportamiento previo en un ambiente autorizado.

### D-10 — Regla de corrección localizada

Ante una regresión, la matriz se detiene para esa celda, se registra la causa y se corrige únicamente el límite responsable. Toda corrección exige prueba focal, MSBuild de los proyectos afectados y repetición de la celda. No se aceptan mocks, resultados fabricados ni cambios amplios preventivos.

### D-11 — Derivado linealizado temporal sin cambiar almacenamiento

Corrección 13.1: la limpieza automática exige nombre de 64 hexadecimales con sufijo `.pdf` o `.first.pdf` y marcador `.use` versionado ligado al nombre. Solo una publicación exitosa crea propiedad; acceder a caché legacy no la adopta. Se excluyen rutas con reparse points, subdirectorios, temporales de generación, marcas ilegibles y claves en preparación en el proceso actual. Los archivos sin propiedad comprobable se conservan, incluso si son antiguos. La invalidación de una caché existente tampoco borra archivos sin esa propiedad. Esta marca no sustituye ACL: el directorio debe ser dedicado y no escribible por usuarios ajenos; no se garantiza exclusión distribuida entre procesos IIS.

La matriz autorizada demostró que el PDF real de 33.592.820 bytes usa `STREAM` y que su primer canvas crece de 4.341–6.612 ms a 100 Mbps hasta 29.183–32.618 ms a 10 Mbps, transfiriendo el archivo completo en cada carga. También rechazó forzar `RANGE` sobre el original no linealizado: PDF.js realizó 63 solicitudes, cubrió prácticamente los 33.592.820 bytes y empeoró el primer canvas a 8.003/5.828 ms.

Para un PDF válido de al menos 1 MiB no linealizado, el servidor podrá crear en segundo plano un derivado linealizado temporal y cacheado. La primera apertura conserva `STREAM` del original y nunca espera el proceso; una apertura posterior usa `RANGE` únicamente si el derivado existe y su cabecera valida `/Linearized`. La configuración es global y administrativa, no por usuario o grupo. Ejecutable ausente, desactivación, timeout, salida inválida o cualquier error retornan de forma cerrada al original.

`doc_ref` autoriza solamente la ruta de visualización preparada. `urimage_format` conserva la ruta original para pegado de grafo, firma, coordenadas, descarga y persistencia. El original no se reescribe ni se sustituye. La adopción en un ambiente exige revisar licencia y distribución del linealizador configurado. La decisión solo se acepta si el mismo PDF produce 206, bytes únicos antes del primer canvas menores al total y mejora el tiempo remoto.

### D-12 — Estado de carga visible hasta el primer canvas

El iframe mostrará desde su HTML inicial un overlay accesible sobre el área documental, sin cubrir la barra de herramientas. PDF.js alimentará una barra determinada cuando `onProgress` entregue `loaded` y `total`; sin total conocido conservará animación indeterminada. Al completar transferencia cambiará el texto a preparación de la primera página y el overlay se ocultará únicamente desde `onPageRender`, después de insertar un canvas real. Un fallo mantiene el panel con mensaje estable y sin porcentaje engañoso.

El indicador no agregará sondeos, solicitudes ni temporizadores de progreso ficticio. `onDocumentReady` dejará de invocar `pdf.getData()` porque su resultado no se utiliza y puede forzar descarga adicional, especialmente con `RANGE`. Scroll, zoom, navegación y pegado de grafo conservan sus contratos actuales.

### D-13 — Primera página útil, no parcialidad nominal

Alcance temporal: el límite de 3 segundos de 11.13 comienza en el cargador de preview del navegador. No incluye `GenerarVistaPrevia`, invocada sincrónicamente por `PreparadorVisualizacionPdf.Preparar` cuando hace falta preparar esa página, ni la navegación previa. La generación del derivado completo sí se encola en segundo plano. No se afirma que la apertura completa ni toda preparación del servidor estén acotadas a 3 segundos; tampoco se modifica ese comportamiento mediante esta aclaración documental.

Ajuste autorizado de arranque (11.13): cuando existe preview, se prioriza su render y una oportunidad de pintura antes del principal. Un límite de 3 segundos cubre carga, render y frames suspendidos; error/ausencia continúa inmediatamente. Al vencer se invalida el trabajo y se cancelan recursos sin esperar su destrucción. Callbacks tardíos no publican progreso ni canvas. Preview exitoso conserva estado/contador/telemetría al arrancar el principal, manteniendo controles bloqueados hasta su render. El límite es una cota de fallback, no una mejora de latencia demostrada.

La E2E de Producción Documental sobre un PDF pesado observó `RANGE` y HTTP 206, pero necesitó 231.079.058 de 289.799.314 bytes (79,74 %), 223 solicitudes y 48.774/56.662 ms antes del primer canvas. Ese resultado es una regresión aunque la unión de intervalos sea matemáticamente menor al total.

DOC-105 separará tres condiciones: contrato HTTP válido, parcialidad nominal y primera página útil. Una celda enriquecida solo podrá quedar `PASSED` cuando cumpla presupuestos explícitos y cerrados para tiempo, bytes únicos antes del primer canvas y cantidad de solicitudes; faltar un presupuesto aplicable será `BLOCKED`, no éxito implícito. La evidencia conservará únicamente valores saneados y una categoría de fallo.

La preparación productiva no aceptará un derivado solo por encontrar `/Linearized` en la cabecera. Deberá caracterizar la sección inicial declarada por el diccionario de linearización y rechazar como ineficaz un derivado cuya primera sección exceda el límite administrativo. El rechazo no se presentará como corrección: el comportamiento de primera página deberá resolverse mediante una representación de visualización que permita renderizar la página inicial dentro del presupuesto, sin sustituir ni reescribir el original. La selección definitiva se validará con el mismo PDF pesado de Producción antes de cerrar la tarea.

### D-14 — Contador estable y disponibilidad independiente del preview

La confirmación de entrega al navegador se retira a los cinco segundos y limpia su texto. Una nueva descarga cancela e invalida el temporizador anterior; la espera no tiene caducidad artificial y los avisos de error o permiso se conservan. El temporizador solo controla presentación, no la operación ni el guardado en disco.

Descarga muestra estado accesible y bloqueo exclusivo del botón durante la promesa de preparación del servicio. Llamadas repetidas comparten esa promesa. Se conserva permiso y payload de negocio; error de transporte o respuesta inválida se sanea, sin reintento automático. Al entregar el enlace al navegador se informa esa entrega, no finalización en disco. No se precarga el PDF mediante fetch ni se modifica el helper compartido de descarga.

Rotación usa el botón nativo completo y conserva como ancla el número de página: tras reconstruir posiciona el inicio de esa página antes de calcular páginas visibles, sin porcentaje global ni promesa de conservar la coordenada exacta dentro del papel. Una generación obsoleta no publica estructuras; giros consecutivos conservan el ancla mientras la reconstrucción esté pendiente.

Los controles de zoom registran el listener en el botón nativo contenedor, no en su icono. Se conservan IDs interiores por compatibilidad; se añade nombre accesible al botón y `type=button`. El clic del icono propaga una sola vez al botón; Enter/Espacio usan activación nativa. No se modifica el planificador ni se confunde corrección del área de clic con mejora medida de renderizado.

El video revisado muestra primera página visible con ambos campos vacíos antes de aparecer el total. El HTML inicia con `… / …`; el preview solo conoce página 1 y nunca aporta su total de una página como total documental. Los metadatos del PDF principal actualizan el total sin esperar descarga completa. Un fieldset nativo bloquea la toolbar hasta render principal y ante error, sin modificar permisos ni bloqueos individuales de capacidades. Las acciones JavaScript también comprueban disponibilidad. El preview usa el mismo zoom inicial del visor principal. La comprobación visual posterior se registra pendiente separadamente de las pruebas locales.

## Contratos de evidencia

### D-20 — Preparación de impresión observable

El botón nativo completo inicia una única promesa compartida con aviso accesible inmediato y aria-busy. Ni la respuesta servidor ni onLoadingEnd liberan el control: Chromium puede seguir preparando su diálogo. Se espera onPrintDialogClose (también cancelación), error saneado o fallback onIncompatibleBrowser que abre otra pestaña. Resolución diferida una microtarea prioriza errores síncronos. La biblioteca detecta cierre por recuperación de foco en Chrome/Firefox y mouseover en otros navegadores: es una heurística, no prueba de impresión física, y requiere confirmación real incluida cancelación. No cambia endpoint, operación Imprime, permisos, copia servidor ni velocidad. Sin temporizador que permita duplicados durante una preparación lenta.

### D-19 — Grafo en zona visible y controles opacos

La inserción desde la toolbar usa la intersección entre página activa y viewport documental para centrar el formulario completo en la zona visible, convirtiendo a coordenadas locales de página. No cambia scroll ni la geometría de guardado. Se recalcula al conocer dimensiones de imagen solo si el usuario no ha comenzado a interactuar; una imagen tardía no reposiciona un grafo manipulado o retirado. La ruta con coordenadas explícitas conserva su posición y revelado interno. La opacidad de previsualización se limita a la imagen; barra opaca de alto contraste, botones nativos y etiqueta Guardar visible. No altera transparencia servidor, permisos ni persistencia.

### D-18 — Altura disponible del visor en Producción

Producción asignaba al modal, cuerpo e iframe la misma altura de ventana menos cinco píxeles, omitiendo la cabecera. Se mantiene el tamaño y propietario del modal, pero un contenedor flex interno reserva cabecera y borde; cuerpo con min-height:0 e UpdatePanel/iframe al 100% del espacio restante. No cambia el visor compartido, zoom, firmas, permisos ni otros módulos. La medición del borde inferior usa hit testing por cada iframe y escala geométrica: no infiere clipping de todo ancestro overflow:hidden, pues los descendientes posicionados pueden escapar de ancestros estáticos. El diagnóstico inicial de 901,15 px no es una medida fiable del recorte.

### D-17 — Inserción de grafo sin desplazar el modal anfitrión

La inserción no usará scrollIntoView, porque atraviesa ancestros y puede desplazar el modal que contiene el iframe, ocultando su cabecera y cierre. El visor revelará la anotación mediante scrollTop/scrollLeft exclusivamente de su contenedor documental, con el mínimo desplazamiento necesario en cada eje. Repetirá la comprobación al cargar la imagen, sin cambiar coordenadas del grafo ni acceder al documento padre. La prueba DOM local reproduce el fallo con iframe y modal recortable; no sustituye la confirmación en el módulo real.

### D-16 — Rectángulo visual del grafo convertido al espacio PDF

Se sustituye el tamaño legacy fijo 90x90 y la resta de píxeles a puntos por una matriz afín calculada sobre el rectángulo visible (intersección CropBox/MediaBox). El cliente mide imagen y canvas con getBoundingClientRect, excluye la barra y el borde del contenedor y conserva las medidas fraccionarias. El contrato existente conserva `anotate_y` como distancia desde abajo al borde superior de la imagen. La conversión aplica la rotación explícita 0/90/180/270 usada por este visor en getViewport; no suma /Rotate porque el visor lo reemplaza explícitamente. PdfStamper.RotateContents=False evita una segunda transformación. Se preservan imagen fuente, transparencia, transacción, permisos y auditoría. No se reparan grafos ya incorporados. La validación local de geometría no sustituye la prueba real descartable.

### D-15 — Transparencia explícita y configuración coherente

Se reutiliza la eliminación de blanco del servidor existente, sin cambiar su algoritmo, valores predeterminados ni imagen original. La opción permanece voluntaria. La respuesta de actualización no reemplaza el catálogo ni el objeto de configuración compartido con la promesa de carga; solo una confirmación exitosa aplica los valores elegidos. Reabrir el diálogo limpia sus opciones y refleja el último estado confirmado. La tolerancia se rotula como umbral de color, no porcentaje, advirtiendo que valores altos eliminan trazos claros. El video no demuestra la configuración de la cuenta: no se atribuye su fondo blanco exclusivamente a este defecto. Geometría al guardar queda fuera de este ajuste.

Cada muestra contendrá únicamente: consumidor, perfil de red, estado de caché, modo `RANGE|STREAM`, códigos HTTP, longitud total, bytes únicos, bytes físicos, tiempos agregados, conteo de tareas largas, conteo de solicitudes y resultado. Los valores que puedan identificar documento, cuenta o ambiente se transformarán en categorías o booleanos.

## Riesgos y mitigaciones

- Algunos consumidores pueden no estar disponibles para la cuenta autorizada: se registran `BLOCKED`, nunca `PASSED` por inferencia.
- Un PDF no linealizado grande puede requerir rangos dispersos o terminar cubriendo gran parte del archivo: la E2E mide bytes únicos/físicos y revierte la política si no reduce tiempo remoto.
- La caché puede ocultar transferencia: frío y caliente se ejecutan y reportan por separado.
- Firma/capacidad de tableta puede carecer de dispositivo o fixture: queda observación o bloqueo explícito; no se sustituye con simulación.
- Una mutación real puede duplicarse por reintento: el arnés cancela una segunda confirmación y falla si observa más de un POST.

## Migración y rollback

1. Validar políticas y pruebas locales de DOC-100 a DOC-104.
2. Ejecutar la matriz de solo lectura con autorización vigente.
3. Ejecutar una única mutación descartable con autorización separada.
4. Derivar presupuestos y registrar pendientes reales.
5. Ensayar rollback de archivos/configuración en ambiente autorizado y repetir un smoke test.
6. Promover gradualmente solo con autorización adicional; ante error JavaScript, permisos, doble solicitud o degradación de presupuesto, revertir y conservar evidencia saneada.

## Preguntas abiertas

No hay decisiones arquitectónicas abiertas. Ambiente, cuentas, tareas y documentos concretos son datos operativos efímeros que deben autorizarse antes de cada E2E real y no forman parte del repositorio.
