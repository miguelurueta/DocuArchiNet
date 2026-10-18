## ADDED Requirements

### Requirement: RQ-19 Grafo visible al insertarlo y acciones legibles

El visor SHALL insertar el grafo desde toolbar en la parte visible de la página activa sin desplazar la lectura. SHALL mantener acciones opacas y Guardar identificado con texto y accesible por teclado. SHALL conservar permisos, coordenadas explícitas y procesamiento del PDF. Origen: D-19.

#### Scenario: Lectura con scroll y zoom

- **WHEN** el usuario inserta un grafo desde la toolbar mientras lee una página desplazada o ampliada
- **THEN** el formulario se centra en la intersección visible de esa página y el viewport, sin cambiar scroll

#### Scenario: Imagen tardía y controles

- **WHEN** la imagen termina de cargar después de empezar a manipular el grafo
- **THEN** no se recentra el formulario y Guardar conserva texto y contraste sin heredar la transparencia de la imagen

### Requirement: RQ-18 Barra horizontal accesible en Producción

Producción SHALL reservar la cabecera del modal al dimensionar el iframe y mantener el borde inferior del visor dentro del área visible. SHALL conservar cierre, permisos y zoom existentes. Origen: D-18.

#### Scenario: Zoom con desbordamiento

- **WHEN** el PDF supera el ancho disponible
- **THEN** el desplazamiento horizontal alcanza el extremo derecho y su barra no queda recortada por la cabecera o cuerpo del modal

#### Scenario: Apertura y cambio de tamaño

- **WHEN** el modal se abre desde oculto o cambia la altura de la ventana
- **THEN** el cuerpo ocupa el espacio restante bajo la cabecera sin ocultar el cierre ni extender su iframe fuera del modal

### Requirement: RQ-16 Geometría del grafo consistente con vista previa

El guardado SHALL conservar el rectángulo de la imagen visualizada y su orientación respecto a la página, sin incorporar la barra ni forzar tamaño cuadrado. SHALL convertir todas las medidas con la escala del canvas y la rotación explícita del visor, considerando el origen del área visible PDF. Origen: D-16.

#### Scenario: Zoom y rotación

- **WHEN** se guarda un grafo rectangular con zoom reducido o ampliado y giro de 0, 90, 180 o 270 grados
- **THEN** sus cuatro esquinas coinciden al renderizar el PDF actualizado bajo la misma vista, sin cambiar transparencia ni duplicar la operación

#### Scenario: Rectángulo inválido

- **WHEN** las dimensiones son nulas, negativas o no finitas
- **THEN** se rechaza la geometría sin reemplazar el documento

### Requirement: RQ-15 Configuración explícita de fondo transparente

El visor SHALL conservar la elección confirmada de transparencia y su catálogo al reabrir parámetros, sin activación automática ni modificación del original del grafo. SHALL describir la tolerancia al blanco sin llamarla porcentaje. Origen: D-15.

#### Scenario: Actualización y reapertura

- **WHEN** se confirma una actualización de parámetros y se reabre el diálogo
- **THEN** aparecen los valores confirmados y un único conjunto de opciones; el guardado del grafo recibe esos mismos parámetros

#### Scenario: Fallo de actualización

- **WHEN** el servidor rechaza o no confirma la configuración
- **THEN** se conserva el último estado confirmado y el diálogo muestra un aviso saneado

### Requirement: RQ-14 Contador y herramientas coherentes durante la carga

El visor SHALL mostrar campos no vacíos desde el HTML inicial, conservar el total real conocido y bloquear herramientas mientras solo exista preview o haya error. SHALL conservar permisos y bloqueos individuales y usar la misma escala inicial en preview y visor principal sin exigir descarga completa. Origen: D-14.

#### Scenario: Área completa del botón de zoom

- **WHEN** el visor está listo y se pulsa el borde, el icono o se activa por teclado un botón de zoom
- **THEN** se ejecuta exactamente una acción de zoom; durante carga o error no se ejecuta ninguna

#### Scenario: Giro de la página en lectura

- **WHEN** se activa un botón de rotación mediante borde, icono o teclado con el visor listo
- **THEN** se ejecuta un giro y se posiciona el inicio de la misma página; ante giros consecutivos una inicialización obsoleta no reemplaza la última

#### Scenario: Preparación de descarga

- **WHEN** se solicita descarga autorizada del visor
- **THEN** se muestra espera accesible y se bloquean duplicados hasta entregar al navegador o informar error saneado, sin afirmar finalización del guardado local

#### Scenario: Retiro de confirmación de descarga

- **WHEN** el enlace se ha entregado al navegador
- **THEN** la confirmación se oculta después de cinco segundos; iniciar otra descarga invalida ese temporizador y mantiene visible la nueva espera hasta resolver la preparación

#### Scenario: Preview antes del PDF principal

- **WHEN** se renderiza la primera página temporal sin metadatos del principal
- **THEN** se muestra `1 / …` y las herramientas permanecen bloqueadas

#### Scenario: Metadatos y render principal

- **WHEN** se conoce el total real y se renderiza una página del visor principal
- **THEN** el contador conserva el total y las herramientas quedan disponibles según sus permisos y bloqueos individuales

#### Scenario: Error después del preview

- **WHEN** falla la apertura principal después de mostrar la primera página temporal
- **THEN** el error permanece visible y ningún progreso o render tardío rehabilita herramientas

### Requirement: RQ-13 Primera página útil en PDF pesados

El sistema SHALL distinguir parcialidad nominal de una primera página útil. Un consumidor enriquecido SHALL NOT quedar `PASSED` únicamente porque use HTTP 206 o porque los bytes únicos antes del primer canvas sean inferiores al tamaño total. Origen: D-13.

#### Scenario: RANGE consume casi todo el documento

- **WHEN** la carga usa `RANGE` pero excede cualquiera de los presupuestos aplicables de tiempo, bytes únicos o solicitudes antes del primer canvas
- **THEN** la celda queda `FAILED` con categoría saneada y no se presenta como descarga parcial efectiva

#### Scenario: Presupuesto ausente

- **WHEN** un consumidor enriquecido se ejecuta sin presupuesto explícito para su perfil de red y caché
- **THEN** la celda queda `BLOCKED` antes de poder aprobar la regresión

#### Scenario: Derivado nominalmente linealizado pero ineficaz

- **WHEN** el derivado contiene `/Linearized` pero su sección inicial declarada excede el límite configurado o no puede validarse
- **THEN** el sistema no lo publica como representación `RANGE` apta y conserva el original sin modificación

#### Scenario: Corrección comprobada en Producción Documental

- **WHEN** se repite la E2E autorizada con el mismo PDF pesado de Producción Documental
- **THEN** el primer canvas cumple los presupuestos de tiempo, bytes y solicitudes en frío y caliente, conserva permisos y no realiza mutaciones

#### Scenario: Prioridad acotada de la primera página temporal

- **WHEN** existe una representación temporal de primera página
- **THEN** el arranque prioriza su render y oportunidad de pintura antes del principal durante un máximo de 3 segundos; error o timeout continúa con el principal, cancela el trabajo temporal y rechaza callbacks tardíos sin reiniciar el contador de un preview exitoso

### Requirement: RQ-01 Matriz completa de consumidores

El sistema SHALL mantener una matriz derivada de los tres constructores enriquecidos y de todos los consumidores directos conocidos de `Handler_image_wf.ashx`, indicando entrada, capacidad, mutación permitida y estado real. Origen: D-01.

#### Scenario: Consumidor no ejecutable

- **WHEN** un consumidor obligatorio no puede abrirse con la cuenta o datos autorizados
- **THEN** su fila queda `BLOCKED` con causa saneada y el resultado global no afirma cobertura total

#### Scenario: Consumidor ejecutado

- **WHEN** el recorrido alcanza su visor o descarga y termina sus verificaciones aplicables
- **THEN** la fila registra `PASSED` o `FAILED` sin inferir el resultado de otro módulo

### Requirement: RQ-02 Infraestructura E2E única

El sistema SHALL implementar DOC-105 dentro de `tools/e2e` reutilizando autenticación, selección, validadores y reglas de evidencia existentes. No SHALL crear login, configuración Playwright, proyecto ni `.env` paralelos. Origen: D-02.

#### Scenario: Inicio autenticado

- **WHEN** se lanza la matriz después de leer el runbook y autorizar ambiente, cuenta y datos
- **THEN** la sesión se crea con el helper común y ningún secreto se imprime o persiste

#### Scenario: Falta autorización

- **WHEN** falta una autorización exigida por el runbook
- **THEN** el runner se detiene antes de autenticar o mutar y emite solo un código seguro

### Requirement: RQ-03 Adaptadores por consumidor y medición común

El sistema SHALL separar la navegación específica de Workflow, Correspondencia, Producción, aprobación/externo y handler simple de la captura común de métricas. Origen: D-03.

#### Scenario: Visor enriquecido

- **WHEN** un adaptador abre un consumidor con PDF.js
- **THEN** el medidor común captura primer lienzo, `domInteractive`, tareas largas, scroll, zoom y respuestas del handler

#### Scenario: Handler simple

- **WHEN** un adaptador abre una descarga o preview sin PDF.js
- **THEN** se validan autorización, estado HTTP, política de transporte y bytes, mientras scroll/zoom quedan `NOT_APPLICABLE`

#### Scenario: Correspondencia de solo lectura

- **WHEN** Gestión de Correspondencia abre un PDF con `anula_visor_espress = 1`
- **THEN** el adaptador acepta el iframe dirigido a `Handler_image_wf.ashx`, valida autorización y transporte, y marca canvas, overlay y scroll/zoom PDF.js como `NOT_APPLICABLE` sin exigir `UTIL_VISOR_EXPRESS`

### Requirement: RQ-04 Red remota y estado de caché reproducibles

El sistema SHALL ejecutar perfiles equivalentes a 10, 20, 50 y 100 Mbps con latencia declarada y distinguir muestras frías de calientes. Origen: D-04.

#### Scenario: Carga fría

- **WHEN** se ejecuta una muestra fría
- **THEN** se usa un contexto nuevo con la red configurada antes de abrir el documento

#### Scenario: Carga caliente

- **WHEN** se ejecuta una muestra caliente
- **THEN** se conserva el contexto y caché privada de la sesión autorizada y el resultado se etiqueta como caliente

### Requirement: RQ-05 Contabilidad correcta de transferencia parcial

El sistema SHALL calcular bytes únicos mediante unión de intervalos `Content-Range` y bytes físicos mediante la transferencia observada/sumada, sin equiparar ambos valores. Origen: D-05.

#### Scenario: RANGE verificable

- **WHEN** antes del primer lienzo hay respuestas `206`, longitud total conocida y unión de intervalos menor al total
- **THEN** el resultado puede declarar descarga parcial y reporta bytes únicos y físicos por separado

#### Scenario: STREAM completo

- **WHEN** la entrega usa `200` sin rangos
- **THEN** el resultado declara `STREAM`, no afirma descarga parcial y registra el costo real observado

#### Scenario: Rangos solapados

- **WHEN** dos respuestas cubren bytes repetidos
- **THEN** la unión cuenta esos bytes una vez y los bytes físicos conservan ambas transferencias

### Requirement: RQ-06 Evidencia saneada y resultado honesto

El sistema SHALL emitir evidencia sin identificadores, rutas, URLs, títulos, payloads, cookies, tokens, credenciales ni conexiones, y SHALL agregar resultados sin convertir bloqueos en éxitos. Origen: D-06.

#### Scenario: Campo sensible observado

- **WHEN** navegador o servidor exponen un valor sensible durante la prueba
- **THEN** la evidencia conserva solo categoría, conteo o booleano permitido

#### Scenario: Error JavaScript

- **WHEN** ocurre un error de página o consola no clasificado
- **THEN** la celda falla con una categoría saneada y el error no se oculta

### Requirement: RQ-07 Mutación única y controlada

El sistema SHALL permitir únicamente un guardado de grafo en Workflow sobre datos descartables, con autorización específica, exactamente un POST y consultas de control solo `SELECT` sobre `workflowdocument`. Origen: D-07.

#### Scenario: Guardado autorizado

- **WHEN** se confirma el guardado una vez
- **THEN** se observa un POST, respuesta exitosa e integridad permitida sin segunda solicitud

#### Scenario: Intento duplicado

- **WHEN** la interfaz o red intenta un segundo POST
- **THEN** la validación falla y no repite automáticamente la mutación

### Requirement: RQ-08 Presupuestos basados en evidencia comparable

El sistema SHALL derivar presupuestos finales separando local/remoto y frío/caliente con el mismo PDF, navegador, ambiente y perfil por comparación. Origen: D-08.

#### Scenario: Comparación válida

- **WHEN** existe línea base y resultado bajo condiciones equivalentes
- **THEN** se publican mediana de primer lienzo, `domInteractive`, tareas largas, bytes y máximos de scroll/zoom

#### Scenario: Regresión interactiva

- **WHEN** scroll alcanza 3.548 ms o zoom alcanza 435 ms de bloqueo, o el presupuesto derivado es excedido
- **THEN** la celda falla y se identifica la operación degradada

### Requirement: RQ-09 Despliegue y rollback reversibles

El sistema SHALL documentar orden de despliegue, observación y rollback de DOC-100 a DOC-104 sin `WorkflowCentroTrabajoModernActive` ni activación por usuario/grupo, y no SHALL activar producción sin autorización adicional. Origen: D-09.

#### Scenario: Ensayo de rollback

- **WHEN** se aplica el procedimiento en un ambiente autorizado
- **THEN** los archivos/configuración vuelven al estado anterior y un smoke test confirma el visor previo

#### Scenario: Señal de reversión

- **WHEN** aparece error JavaScript, permiso alterado, doble solicitud o degradación presupuestaria
- **THEN** el plan ordena detener promoción y ejecutar rollback

### Requirement: RQ-10 Corrección localizada y revalidada

El sistema SHALL limitar cualquier cambio productivo a una regresión reproducible, ejecutar pruebas focales y MSBuild de las capas afectadas, y repetir la celda que falló. Origen: D-10.

#### Scenario: Regresión encontrada

- **WHEN** una celda falla por código productivo
- **THEN** se documenta la causa, se cambia solo el límite responsable y se revalida prueba, compilación y celda

#### Scenario: Sin regresión productiva

- **WHEN** las políticas y matriz pasan sin defecto del producto
- **THEN** DOC-105 no modifica almacenamiento, handler, constructores ni visor preventivamente

### Requirement: RQ-11 Descarga parcial para PDF grandes sin cambiar almacenamiento

El sistema SHALL usar `RANGE` directamente solo para PDF linealizados de al menos 1 MiB. Para un PDF válido grande no linealizado MAY preparar una copia linealizada temporal, globalmente configurada y cacheada; SHALL conservar `STREAM` del original mientras el derivado no exista o ante desactivación, timeout, salida inválida o error. La preparación SHALL ocurrir en segundo plano, con una sola generación concurrente por versión del archivo, y no SHALL bloquear la primera visualización. `doc_ref` SHALL referir la copia únicamente para lectura, mientras las operaciones de negocio SHALL conservar la ruta original. El sistema no SHALL reescribir ni sustituir el documento almacenado. Origen: D-11.

#### Scenario: PDF grande no linealizado

- **WHEN** un PDF válido de al menos 1 MiB no contiene `/Linearized`
- **THEN** el visor solicita HTTP 206 con carga automática deshabilitada y puede renderizar la primera página con una unión de bytes menor al total

#### Scenario: Fallback compatible

- **WHEN** el archivo es pequeño, no tiene una cabecera PDF válida o no puede inspeccionarse
- **THEN** la estrategia devuelve `STREAM` sin modificar el archivo ni revelar su ruta

#### Scenario: Validación remota obligatoria

- **WHEN** se repite la celda `remote-100` con el mismo PDF de la línea base
- **THEN** la aceptación exige modo `RANGE`, respuestas 206, bytes únicos iniciales menores al total y mejora frente a la muestra `STREAM`; si no se cumplen, el cambio productivo no se aprueba

### Requirement: RQ-17 Modal estable durante inserción del grafo

El visor SHALL revelar el grafo desplazando solamente el contenedor documental, sin desplazar el modal anfitrión ni ocultar su cierre. SHALL conservar permisos y geometría de guardado. Origen: D-17.

#### Scenario: Inserción fuera del área visible

- **WHEN** se inserta un grafo fuera del área visible, incluso repetidamente dentro de un iframe
- **THEN** el desplazamiento necesario ocurre solo dentro del visor y la cabecera del modal conserva su posición

#### Scenario: Grafo visible

- **WHEN** el grafo ya está completamente visible
- **THEN** la comprobación no desplaza el visor ni sus ancestros

### Requirement: RQ-20 Preparación de impresión sin duplicados

El visor SHALL mostrar preparación accesible desde el primer clic sobre el botón completo y bloquear duplicados hasta notificación de cierre del diálogo, fallback de navegador incompatible o error. onLoadingEnd no SHALL liberar el control. SHALL conservar permisos y operación existente y sanear errores sin exponer respuestas. SHALL retirar el aviso al finalizar sin afirmar impresión física ni mejoras de latencia. Origen: D-20.

#### Scenario: PDF pesado en preparación

- **WHEN** el servidor responde o PrintJS termina su carga pero el navegador sigue preparando el diálogo
- **THEN** continúa el aviso y un segundo clic no genera otra solicitud

#### Scenario: Entrega o fallo

- **WHEN** PrintJS notifica cierre del diálogo (incluida cancelación), fallback incompatible o error, incluso después de onLoadingEnd
- **THEN** el control se libera y el aviso se retira o muestra un error saneado, dando prioridad al error

### Requirement: RQ-12 Retroalimentación visible de carga

El visor SHALL mostrar un estado accesible de carga desde el HTML inicial hasta que el primer canvas sea renderizado. SHALL usar el progreso real de PDF.js cuando exista un total conocido, SHALL usar estado indeterminado cuando no exista y SHALL mostrar un error estable si la apertura falla. No SHALL simular porcentajes ni solicitar el PDF completo mediante `getData()` durante el arranque. Origen: D-12.

#### Scenario: Progreso determinado

- **WHEN** PDF.js informa bytes cargados y total positivo
- **THEN** el visor presenta un porcentaje acotado entre 0 y 100 y mantiene `aria-busy=true`

#### Scenario: Primer canvas visible

- **WHEN** una página termina de renderizarse y su canvas se inserta en el visor
- **THEN** el overlay se oculta, se elimina el estado ocupado y quedan disponibles scroll y navegación sin una espera artificial

#### Scenario: Total desconocido o error

- **WHEN** no existe total conocido o la apertura falla
- **THEN** el visor usa progreso indeterminado o muestra el mensaje de error, sin inventar porcentaje ni dejar una pantalla vacía
