# Exploración técnica: rendimiento del visor PDF con grafos

## Objetivo

Diagnosticar, a partir de la implementación existente, las causas de lentitud del visor PDF que permite insertar firmas o estampas denominadas **grafos**.

Esta exploración es exclusivamente diagnóstica. No modifica la lógica del visor, los servicios web ni la persistencia de documentos.

## Alcance revisado

### Cliente

- `pdfjs/pdf_anotate_view/index.html`
- `pdfjs/pdf_anotate_view/script.js`
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js`
- `pdfjs/pdf_anotate_view/pdfannotate.js`
- `pdfjs/pdf_anotate_view/extend_fabric.js`
- Recursos JavaScript de PDF.js, jQuery UI, Fabric, Wacom, Font Awesome y Print.js cargados por el visor.

### Servidor

- `workflow/ClassWorflowVisor.vb`
- `Docuarchi/ClassVisualisaDocumento.vb`
- `Gestion/ClassGaProducionDocumental.vb`
- `workflow/Handler_image_wf.ashx.vb`
- `radicador/ClassRaEnvioCorrespondencia.vb`
- `webservice/WebService_itext.asmx.vb`
- `itextshare/Class_ItexShare.vb`
- `workflow/Class_transparent.vb`

## Aclaración funcional

En esta implementación, un **grafo** no es una estructura de nodos y aristas. Es una imagen de firma o estampa que el usuario posiciona sobre una página y que posteriormente se incrusta físicamente en el PDF mediante iTextSharp.

## Flujo implementado

```text
Página WebForms
    |
    | construye URL del iframe y permisos
    v
pdfjs/pdf_anotate_view/index.html
    |
    | fetch del archivo completo
    v
Handler_image_wf.ashx
    |
    | lee el PDF completo en Byte()
    v
Blob -> FileReader -> ArrayBuffer -> Uint8Array
    |
    v
PDFjsViewer.loadDocument(...)
    |
    +--> crea esqueletos DOM para todas las páginas
    +--> calcula páginas visibles
    +--> renderiza canvas de páginas visibles y adyacentes
    |
    v
Usuario posiciona el grafo
    |
    v
service_anotate_pdf_image(...)
    |
    +--> valida certificado
    +--> procesa transparencia de imagen
    +--> abre el PDF completo con PdfReader
    +--> genera otro PDF completo con PdfStamper
    +--> extrae la página modificada a un PDF temporal
    +--> reemplaza el PDF original
    +--> registra auditoría
    |
    v
El navegador descarga y renderiza nuevamente la página modificada
```

## Diagnóstico ejecutivo

| Severidad | Hallazgo | Impacto observable |
|---|---|---|
| Crítica | El PDF se carga completamente en servidor y navegador | Apertura lenta, alto consumo de memoria y ausencia de visualización progresiva |
| Crítica | La cola de renderizado libera su bloqueo antes de terminar | Renderizados duplicados o concurrentes, CPU alta y congelamientos intermitentes |
| Crítica | Cada grafo reescribe el PDF completo | El tiempo de guardado crece con el tamaño total del documento |
| Alta | El scroll inspecciona geométricamente todas las páginas | Pausas proporcionales al número de páginas |
| Alta | La página carga dependencias duplicadas y no diferidas | Inicio lento incluso con documentos pequeños |
| Alta | El acceso al PDF y los WebMethods participan en el bloqueo de sesión ASP.NET | Una operación pesada puede hacer esperar otras solicitudes del usuario |
| Media | Zoom y rotación recrean canvas sin cancelar renderizados obsoletos | Picos de CPU y memoria durante interacción rápida |
| Media | El guardado no expone una promesa ni bloquea doble envío | Mala percepción de rendimiento y riesgo de operaciones superpuestas |

## Hallazgos detallados

### 1. Descarga y copias completas del PDF

`script.js` ejecuta `fetch(url_src)`, convierte la respuesta a `Blob`, después usa `FileReader.readAsArrayBuffer()` y finalmente construye un `Uint8Array` para PDF.js.

```text
HTTP -> Blob -> ArrayBuffer -> Uint8Array -> PDF.js
```

Al entregar un arreglo completo a `pdfjsLib.getDocument`, el visor pierde la posibilidad de aprovechar carga progresiva directamente desde una URL.

En el servidor, `Handler_image_wf.ashx.vb` invoca `ClassRaEnvioCorrespondencia.ReadFile`. Este método:

1. abre el archivo completo;
2. reserva un arreglo del tamaño total del PDF;
3. lee todo el contenido;
4. lo entrega mediante `Response.BinaryWrite`.

No se observó implementación de:

- solicitudes HTTP `Range`;
- `Accept-Ranges`;
- streaming directo del archivo;
- caché condicional;
- entrega explícita de `Content-Length`.

Por lo tanto, la primera página no puede mostrarse hasta completar la descarga y las conversiones previas.

### 2. Condición de carrera en la cola de renderizado

Los métodos implicados son:

- `PDFjsViewer.loadPages(...pages)`;
- `PDFjsViewer._loadingTask()`;
- `PDFjsViewer._renderPage(page, i)`.

`_loadingTask()` marca `_loading = true`, inicia `pdf.getPage(pagei)`, pero restablece `_loading = false` inmediatamente después de programar la promesa. Además, el primer `then` inicia `_renderPage()` sin retornar su promesa.

La secuencia efectiva es:

```text
inicia getPage
    |
    +--> libera _loading antes de terminar
    |
nuevo scroll puede iniciar otro trabajador
    |
getPage resuelve
    |
inicia render, pero la cadena no espera page.render(...).promise
```

`loadPages()` tampoco deduplica páginas ya en cola o actualmente renderizadas. Como resultado, una misma página puede cargarse o renderizarse más de una vez.

### 3. Recorrido completo del documento durante el scroll

`PDFjsViewer._visiblePages()` filtra el arreglo completo `this.pages`. Para cada página consulta posición y tamaño mediante operaciones jQuery como:

- `offset()`;
- `width()`;
- `height()`;
- `outerWidth()`;
- `outerHeight()`.

Estas lecturas pueden forzar recálculos de layout. La complejidad es lineal respecto al número total de páginas en cada evaluación relevante del scroll.

Adicionalmente, `_createSkeletons()` crea desde el inicio un elemento DOM para cada página, aunque solo una fracción sea visible.

### 4. Renderizado redundante durante zoom y rotación

`setZoom()` modifica las dimensiones DOM de todas las páginas y después invoca `_visiblePages(true)`. La condición que evalúa `estate_anotate` ejecuta la misma operación en ambas ramas.

No se conserva ni cancela el `RenderTask` devuelto por PDF.js. Varias acciones rápidas de zoom pueden dejar renderizados anteriores trabajando sobre canvas que pronto serán reemplazados.

El canvas usa una escala equivalente a:

```text
zoom actual * window.devicePixelRatio
```

En pantallas con `devicePixelRatio = 2`, el número de píxeles del canvas puede ser aproximadamente cuatro veces mayor que con relación 1.

### 5. Reescritura completa por cada grafo

`WebService_itext.service_anotate_pdf_image()` delega en `Class_ItexShare.add_anotate_image_pdf()`.

Para una sola estampa, el servidor:

1. resuelve la ruta del PDF;
2. prepara la imagen del grafo;
3. abre el documento completo con `PdfReader`;
4. crea un PDF temporal completo mediante `PdfStamper`;
5. incrusta la imagen en una página;
6. cierra y escribe el documento completo;
7. vuelve a abrir el PDF temporal;
8. extrae la página afectada mediante `PdfCopy`;
9. copia el PDF temporal sobre el original;
10. elimina el temporal.

El costo de guardar un grafo depende del tamaño completo del PDF y no únicamente de la página modificada.

### 6. Archivos temporales compartidos por usuario

Se utilizan nombres como:

- `{usuario}.pdf`;
- `{usuario}.png`;
- `{usuario}_1.png`;
- `_{nombre-documento}`.

Dos pestañas o solicitudes concurrentes del mismo usuario pueden competir por esos archivos. Esto puede producir esperas, errores de archivo en uso o reemplazos cruzados.

### 7. Dependencias duplicadas y carga inicial excesiva

`index.html` contiene 39 etiquetas `script`, de las cuales 14 referencian recursos externos.

Se verificaron las siguientes redundancias:

- varias versiones de jQuery UI;
- dos inclusiones de `signature_sdk.js`;
- dos inclusiones de `wgssStuSdk.js`;
- versiones diferentes de Popper;
- PDF.js, worker, sandbox y worker-entry incluidos como scripts;
- Font Awesome `all.js`, `brands.js` y `solid.js` simultáneamente;
- Bootstrap JavaScript 5.3.2 con CSS 5.1.3.

Los tres archivos JavaScript locales de Font Awesome suman aproximadamente **7,59 MB sin comprimir**. Los recursos Wacom únicos suman aproximadamente **536 KB** y se cargan aun cuando no se vaya a utilizar una tableta.

Fabric, jsPDF y Prettify permanecen incluidos, aunque el visor anterior basado en `pdfannotate.js` está comentado en `index.html`.

### 8. Serialización por sesión ASP.NET

`Handler_image_wf` implementa `IRequiresSessionState`, y los WebMethods relacionados con grafos declaran `EnableSession:=True`.

ASP.NET puede aplicar bloqueo exclusivo de sesión durante estas solicitudes. Una descarga o reescritura pesada puede retrasar otras peticiones del mismo usuario, aunque funcionalmente sean independientes.

### 9. El guardado retorna antes de finalizar

`save_anotate_firma()` configura `Image.onload`, inicia posteriormente el AJAX, pero retorna `"YES"` inmediatamente.

El llamador interpreta ese retorno como éxito y elimina la interfaz flotante del grafo antes de que el servidor confirme la operación.

No se observó:

- promesa retornada al llamador;
- indicador de progreso;
- prevención de doble clic;
- timeout explícito;
- cancelación;
- medición de duración por etapa.

### 10. Inconsistencia en el procesamiento de transparencia

`Class_transparent.TransparentAsync()` no ejecuta trabajo asíncrono.

Además, `ForEachPixel()` declara ciclos desde `Width - 1` y `Height - 1` hasta cero sin `Step -1`. En VB, esos ciclos normalmente no realizan iteraciones con el paso predeterminado positivo.

Actualmente esto representa principalmente una inconsistencia funcional. Corregir solamente el sentido del ciclo haría que se ejecutaran `GetPixel` y `SetPixel` por cada píxel, una estrategia que también sería lenta para imágenes grandes.

## Diagnóstico por momento de uso

### Al abrir el visor

Los factores dominantes son:

1. cantidad y duplicación de dependencias;
2. dependencia de varios CDN;
3. lectura completa del PDF en el servidor;
4. descarga completa antes de invocar PDF.js;
5. copias `Blob -> ArrayBuffer -> Uint8Array`;
6. creación de esqueletos para todas las páginas.

### Al desplazarse

Los factores dominantes son:

1. recorrido de todas las páginas;
2. lecturas repetidas de layout;
3. cola sin deduplicación;
4. bloqueo `_loading` liberado prematuramente;
5. renderizados no cancelados.

### Al ampliar o rotar

Los factores dominantes son:

1. actualización de dimensiones para todas las páginas;
2. recreación de canvas visibles y adyacentes;
3. costo cuadrático del número de píxeles respecto a la escala;
4. posibles renderizados obsoletos simultáneos.

### Al guardar un grafo

Los factores dominantes son:

1. procesamiento de la imagen;
2. lectura y escritura del PDF completo;
3. segunda apertura para extraer la página;
4. copia completa sobre el original;
5. auditoría dentro de la misma solicitud;
6. posible bloqueo de sesión;
7. nueva descarga y render de la página modificada.

## Orden recomendado de intervención

1. Corregir la cola de renderizado, retornar las promesas y deduplicar páginas.
2. Conservar y cancelar los `RenderTask` obsoletos de PDF.js.
3. Entregar una URL compatible con streaming/rangos directamente a PDF.js.
4. Evitar la lectura completa del PDF en `Byte()` dentro del handler.
5. Sustituir el recorrido global por observación de páginas visibles, por ejemplo mediante `IntersectionObserver`.
6. Eliminar dependencias duplicadas y cargar Wacom/Fabric/jsPDF solo cuando sean necesarios.
7. Convertir el guardado en una operación esperable, con progreso, timeout y exclusión de doble envío.
8. Usar temporales únicos por documento, pestaña u operación.
9. Medir y optimizar la reescritura de iTextSharp sin cambiar el resultado documental.
10. Revisar el procesamiento de transparencia con una operación de bitmap eficiente.

## Medición recomendada antes de implementar

La revisión estática demuestra los defectos estructurales, pero no cuantifica cuánto aporta cada uno en el ambiente real. Se recomienda medir al menos:

| Escenario | Métricas |
|---|---|
| Apertura de PDF de 5, 50 y 200 páginas | tiempo a primera página, descarga, memoria y tareas largas |
| Scroll continuo | FPS, tareas mayores de 50 ms, cantidad de renders por página |
| Tres acciones rápidas de zoom | renders iniciados, cancelados y terminados; memoria máxima |
| Guardado de un grafo | validación, procesamiento de imagen, PdfStamper, extracción, copia, log y refresco |
| Dos pestañas del mismo usuario | colisiones de temporales, esperas por sesión y consistencia del documento |

## Infraestructura E2E Workflow reutilizable

La revisión de `tools/e2e` confirmó que no es necesario construir de nuevo el inicio de sesión ni la selección de tareas. La medición reutiliza:

- `tools/e2e/tests/support/authenticated-workflow-session.cjs` para crear la sesión Web Forms autenticada;
- el contrato real de selección de tarea mediante `tip_event="seleccion_tarea_wf"` y `Hidden_id_tarea_selecionada`;
- el contrato real de selección documental mediante `tip_event="vis_doc_selecion_wf"`;
- el iframe oficial `ifrm_visor_` y la navegación a `pdfjs/pdf_anotate_view/index.html`;
- Playwright y Chromium ya instalados en `tools/e2e`;
- el patrón interactivo existente para solicitar secretos y autorizaciones sin persistirlos.

Se incorporó el runner `tools/e2e/scripts/run-workflow-pdf-viewer-performance-interactive.cjs`. Su primer alcance es deliberadamente no destructivo respecto del PDF: mide apertura, primera página, descarga, canvases, tareas largas, scroll y zoom, pero no invoca `service_anotate_pdf_image` ni guarda grafos.

Abrir el documento sí ejecuta el registro normal de visualización de la aplicación mediante `Registra_log_procesing_image`. Por esta razón el runner solicita autorización explícita para dicho efecto antes de iniciar.

### Perfil y ejecución

Copiar `tools/e2e/profiles/workflow-pdf-viewer-performance.profile.example.json` con extensión `.runtime.json` y configurar únicamente el ambiente, módulo, tarea, ordinal del documento y parámetros de medición. Los perfiles `.runtime.json` están excluidos de Git.

```powershell
npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance -- --profile profiles/workflow-pdf-viewer-performance.local.runtime.json
```

La evidencia saneada queda en `tools/e2e/artifacts/workflow-pdf-viewer-performance.json`. No contiene URL, query string, ruta física, credenciales, cookies, identificador de tarea ni identificador documental.

La prueba contractual, que no requiere ambiente ni credenciales, se ejecuta con:

```powershell
npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-performance:policy
```

La medición de guardado de grafos queda separada porque esa operación reescribe el PDF. Requerirá un documento descartable, autorización específica y comprobación antes/después; no debe mezclarse con la línea base de lectura.

## Línea base E2E obtenida

El 12 de octubre de 2026 se ejecutó satisfactoriamente la medición en navegador visible sobre un PDF pesado de Workflow. La corrida utilizó tres iteraciones, 12 pasos de desplazamiento y una ampliación de `1.25` a `1.5`. La evidencia saneada quedó en `tools/e2e/artifacts/workflow-pdf-viewer-performance.json`.

| Métrica | Resultado |
|---|---:|
| Páginas | 34 |
| Tamaño decodificado del PDF | 33.589.755 bytes |
| Primera carga hasta primer canvas | 2.902 ms |
| Recarga más rápida | 830 ms |
| Mediana hasta primer canvas | 878 ms |
| Máximo hasta primer canvas | 2.902 ms |
| Canvases simultáneos | 4 |
| Píxeles simultáneos de canvas | 3.130.752 |
| Tiempo del handler en primera carga | 177 ms |
| Tareas largas durante las tres cargas | 5 |
| Mayor tarea larga de carga | 618 ms |
| Scroll de 12 pasos | 3.902 ms |
| Tiempo bloqueado por tareas largas durante scroll | 3.548 ms |
| Tareas largas durante scroll | 11 |
| Zoom `1.25 -> 1.5` | 779 ms |
| Tiempo bloqueado por tarea larga durante zoom | 435 ms |
| Solicitudes de red fallidas | 0 |
| Rechazos de promesas no manejados | 0 |

### Interpretación de la línea base

- El handler entregó los 33,6 MB en 75–177 ms. Representó aproximadamente entre 6 % y 13 % del tiempo hasta el primer canvas, por lo que la red local y el handler no fueron el factor dominante de esta corrida.
- La primera apertura fue aproximadamente 3,3 veces más lenta que la mediana. Su `domInteractive` consumió 1.679 ms, frente a 92–98 ms en las recargas, lo que confirma el costo inicial de dependencias y compilación/ejecución JavaScript.
- Incluso con recursos calientes, cada primera página conservó una tarea larga de 550–563 ms. La mayor parte del tiempo residual está en procesamiento y renderizado del cliente.
- El scroll es el síntoma más severo: 3.548 de 3.902 ms, aproximadamente 91 %, estuvieron dentro de tareas largas. Esto confirma la degradación prevista por el recorrido global de páginas, las lecturas de layout y la cola de renderizado.
- El zoom también bloqueó 435 de 779 ms, aproximadamente 56 %, y confirma que la reconstrucción de canvases visibles ocurre en una operación costosa para el hilo principal.
- Las tres iteraciones transfirieron nuevamente el tamaño completo del PDF. La corrida no observó una reutilización que eliminara la transferencia del cuerpo en las recargas controladas.
- Se observó un error JavaScript por iteración y tres eventos `pageerror` en total, aunque el PDF renderizó y las interacciones terminaron. La evidencia actual no conserva el mensaje para evitar filtrar rutas o datos. Este error debe clasificarse con una categoría saneada antes de declarar la línea base libre de errores funcionales.

Esta prueba valida el recorrido y cuantifica la experiencia observada, pero no mide todavía persistencia de grafos, concurrencia entre pestañas ni consumo de memoria del proceso Chromium.

### Alcance local y proyección para servidor remoto

La línea base anterior se obtuvo contra el servidor local del mismo equipo. Es un resultado optimista que aísla principalmente el costo del navegador; no representa el tiempo esperable de un usuario conectado a un servidor remoto.

El visor actual ejecuta `fetch(url_src)`, espera el `Blob` completo, lo transforma en `ArrayBuffer`, crea un `Uint8Array` y solo entonces llama a PDF.js. Por tanto, el primer canvas no puede aparecer mientras no termine de transferirse el PDF completo. La latencia y el ancho de banda remoto se agregan directamente al tiempo de apertura.

Para el archivo medido, de 33.589.755 bytes, la transferencia teórica mínima sería aproximadamente:

| Ancho de banda efectivo | Solo transferencia del PDF | Apertura estimada sumando 0,8–2,9 s de cliente |
|---:|---:|---:|
| 10 Mbps | 26,9 s | 27,7–29,8 s |
| 20 Mbps | 13,4 s | 14,2–16,3 s |
| 50 Mbps | 5,4 s | 6,2–8,3 s |
| 100 Mbps | 2,7 s | 3,5–5,6 s |

Estas cifras son un límite optimista: no incluyen congestión, TLS, pérdida de paquetes, proxy, VPN, lectura de almacenamiento en el servidor, concurrencia, restricciones del enlace ni variabilidad del handler. En conexiones de 10–20 Mbps, un PDF similar puede tardar decenas de segundos antes de mostrar la primera página.

La medición local tampoco demostró reutilización del cuerpo: las tres iteraciones registraron nuevamente 33.589.755 bytes transferidos. En el ambiente remoto esto puede multiplicar el consumo de red cuando el usuario recarga, vuelve a abrir el documento o guarda un grafo y el visor refresca la página.

La solución estructural debe permitir que PDF.js consuma una URL HTTP con soporte real de `Range` y streaming, sin el ciclo obligatorio `fetch -> Blob -> FileReader -> Uint8Array`. Con rangos, la primera página podría renderizarse sin esperar los 33,6 MB completos. Esto debe verificarse con una segunda línea base ejecutada contra el servidor remoto y bajo perfiles de red controlados.

## Conclusión

La lentitud tiene tres focos principales:

1. **Apertura:** el PDF se materializa completo varias veces y la página carga dependencias excesivas y duplicadas.
2. **Interacción:** el visor revisa todas las páginas y su cola permite renderizados duplicados o concurrentes.
3. **Persistencia:** cada grafo provoca una reescritura completa, una segunda lectura para extraer la página y una copia final sobre el documento original.

La corrección debe abordarse por capas y con medición previa. Reemplazar completamente el visor no es una precondición: los defectos principales pueden aislarse y corregirse conservando el contrato actual con WebForms, PDF.js, el WebMethod y las tablas existentes.
