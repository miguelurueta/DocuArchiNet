# Base gráfica de importación por carpetas

## Bloqueo real no simulado en la maqueta — 2026-11-14

El video194059 muestra CREANDO/EN_CURSO sin confirmación, un caso distinto del intento INCIERTO de la demostración. La aplicación ahora conserva la operación bloqueada y un aviso visible al desplazar; **Revisar bloqueo** lleva al detalle sin peticiones, mientras Actualizar solo consulta. Verificar se ofrece únicamente bajo la autoridad del servidor y puede no resolver el intento. La maqueta mantiene sus límites offline; no se usa para habilitar replay ni inferir creación. Evidencia local79/79: ../Validacion/doc120-flujo-final-20261114.json. La causa y recuperación del lote39 requieren diagnóstico real autorizado, todavía no ejecutado.

## Fidelidad aplicada al asistente — 2026-11-13

El corte posterior a2642e604 traslada los estilos del HTML al asistente real con alcance CSS limitado. Consultar importación / último lote permanecen visibles; pasos circulares, formulario, filas de carpetas, estados, contadores, panel de acción y pie siguen la maqueta. Se comparan116 propiedades calculadas y capturas en390/768/1280px, además de71/71 pruebas locales. Fuente vigente: ../Validacion/doc120-fidelidad-maqueta-20261113.json; capturas y comparación en .tmp/doc120-fidelidad, excluidas de Git.

El HTML conserva los datos/controles DEMO. La aplicación conserva su marco modal, más perfiles, eliminación individual, incidencias/paginación y datos de sus servicios; esos controles operativos no se sustituyen por la simulación. La consulta secundaria conserva preparación en memoria y exige reselección propia; no se promete persistir Files al recargar. Los cortes siguientes mantienen su alcance histórico.


## Correcciones de compatibilidad implementadas2026-11-13

La aplicación ahora incorpora la organización de tres grupos en sus controles existentes. C01: la maqueta exige la carpeta confirmada en la página visible para índices/almacenamiento, igual que accionesRecurso. C02: descarga13 columnas con coma y escape/protección de fórmulas; nombre DEMO-importacion-N-archivos-pagina-P.csv y datos sintéticos. C03: conserva la página solicitada aunque quede vacía y declara capacidad simulada de página. Pruebas GUI03/GUI04 en tests/doc120-importacion-guiada.test.cjs.

El informe compatibilidad-maqueta-doc120-20261113.json conserva la auditoría original sobre f3bb9e16, registrada en0325a579 y **no describe el estado posterior** de sus fuentes modificadas. La resolución vigente y las huellas posteriores están en ../Validacion/doc120-asistente-guiado-20261113.json. Persisten los límites L04–L07: autenticación/persistencia/reconciliación/políticas simuladas no son prueba del servidor. Los apartados siguientes son antecedentes del prototipo y su auditoría inicial.


## Propuesta guiada DOC-120 — 2026-11-13

Abre [propuesta-importacion-guiada-doc120.html](propuesta-importacion-guiada-doc120.html) en el navegador. Es un único HTML con estilos, iconos y JavaScript incluidos, sin dependencias ni conexión a servicios. Representa la organización solicitada después de revisar el video seleccion-vista-plan; no modifica la aplicación desplegada ni sus reglas.

Rótulo de envío aclarado: **Archivo listo para enviar** y botón **Enviar archivo** cuando hay uno; **Archivos listos para enviar** y **Enviar N archivos** cuando hay varios. El texto explica el archivo o la cantidad que se enviará. Se conserva la función existente: solo los pendientes vinculados de la página visible, con el filtro aplicado.

La simplificación es de presentación: **Seleccionar → Revisar → Seguimiento**. La primera pantalla agrupa perfil y carpetas; filtros y configuración están plegados y la acción disponible se destaca en el pie. Se conserva la operativa construida: **Preparar revisión → Confirmar plan → Enviar pendientes de esta página → Crear expediente → Preparar índices → Almacenar archivo**. Cada acción es explícita; confirmar no recibe archivos, recibir no crea expedientes y preparar índices no almacena. Se retira el encadenamiento automático de la versión anterior de la maqueta. La unión/separación de PDF queda fuera.

**Exportar esta página** está disponible desde la confirmación y respeta el filtro. El envío incluye únicamente los pendientes vinculados de la página de archivos visible. Las carpetas se seleccionan por referencia mediante Ver acciones, sin crear otra de forma automática. La selección permite hasta tres carpetas de ejemplo; las consultas incluyen lotes ficticios mayores para mostrar paginación. Las tarjetas muestran únicamente inventario original (carpetas, archivos y tamaño), como el contrato actual; no anuncian un contador global nuevo de documentos almacenados.

Los controles construidos se representan en dos bloques independientes:

- **Carpetas del lote:** Estado de carpeta, Aplicar/Limpiar filtros de carpetas, tabla y paginación propia.
- **Archivos del lote:** Referencia de carpeta, Efecto documental y Recepción, Aplicar/Limpiar filtros de archivos, tabla y paginación propia. Los tres criterios se combinan; cambiar los controles no consulta hasta pulsar Aplicar.

Las opciones de estado coinciden con ImportacionCarpetasVista.js. Los filtros empiezan plegados, mantienen un indicador de filtros activos y se conservan al paginar; limpiar un bloque no borra los filtros del otro. Ambas páginas usan 25 filas, como el cliente construido. El CSV contiene solo la página de archivos y sus filtros, incluidos resultados vacíos.

**Consultas sin preparar otra importación:** el acceso Consultar importación abre Referencia del lote y permite consultar también el último lote. Prueba **101**, con27 carpetas en ARCHIVO-DEMO, para recorrer dos páginas; o **102**, con2 carpetas en OTRO-NIVEL-DEMO, para comprobar otro destino y estados bloqueados/cancelados. Una referencia no disponible conserva la consulta previa. Los lotes que confirmes en la demostración reciben referencias desde34 y también pueden consultarse sin volver a confirmar. Los datos se mantienen solo en memoria de este HTML; no se consulta el ambiente, ni se validan permisos reales.

**Verificar confirmación pendiente:** selecciona una carpeta, prepara la revisión y despliega Explorar otros estados de la maqueta. Simular confirmación sin respuesta muestra la referencia del plan y bloquea otra confirmación. Verificar confirmación pendiente consulta su resultado y abre el lote registrado, sin repetir confirmación, recepción ni creación.

**Actualizar estado** es el rótulo aclarado de la acción existente Consultar estado: actualiza el lote y las tablas en la misma vista. No navega a otra pantalla ni avanza fases. Al terminar muestra una confirmación visible junto al botón superior y en el pie fijo, con el lote y la hora, aunque los datos no hayan cambiado. Conserva filtros, páginas, desplazamiento y foco. **Consultar importación** sigue siendo el acceso separado para abrir un lote por referencia.

La recuperación representa **Actualizar estado**, **Reseleccionar archivo** pendiente y **Verificar intento** cuando la consulta identifica un intento incierto conocido. Una respuesta perdida bloquea nuevos efectos hasta consultar. La verificación recupera el resultado de ese intento y no dispara la fase siguiente. **Solicitar detención** conserva los avances y bloquea nuevas operaciones; no se presenta como Pausar/Continuar. Cerrar conserva la referencia y desvincula archivos pendientes; reabrir consulta esa referencia. Recargar realmente este HTML reinicia los datos ficticios. No se seleccionan, leen ni envían archivos reales.

Correspondencia con los servicios existentes, usados únicamente como referencia de esta simulación:

| Acción de la maqueta | Operación construida |
| --- | --- |
| Preparar revisión | PrepararPlan |
| Confirmar plan | ConfirmarPlan |
| Actualizar estado (misma acción de Consultar estado) | ConsultarLote / ConsultarRecursosLote |
| Consultar importación por referencia / Consultar último lote | ConsultarLote / ConsultarRecursosLote, con el destino original del lote |
| Verificar confirmación pendiente | ConsultarConfirmacionLote |
| Aplicar/Limpiar filtros y paginar carpetas o archivos | ConsultarRecursosLote, manteniendo tipo, filtros y página correspondientes |
| Enviar archivo / Enviar N archivos | Misma función de Enviar pendientes de esta página: recepción ASHX, un archivo por solicitud |
| Crear expediente | AvanzarLote / CREAR_EXPEDIENTE |
| Preparar índices | AvanzarLote / MATERIALIZAR_VALORES |
| Almacenar archivo | AvanzarLote / ALMACENAR |
| Verificar intento | ReconciliarRecurso, ligado al intento conocido |
| Solicitar detención | SolicitarDetencion |
| Exportar esta página | ExportarPaginaLote |

Referencias: [acciones y guardas del cliente](../../../../../js/gestion/ImportacionCarpetas/ImportacionCarpetas.js), [fachada de fases](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/CoordinadorImportacionCarpetas.Operaciones.vb) y [acciones permitidas por consulta](../../../../../Services/ProduccionDocumental/ImportacionCarpetas/ServicioConsultaImportacion.vb). La maqueta no invoca esos servicios ni introduce endpoints, persistencia o capacidades nuevas.

Verificación local de respuesta visible: **63 comprobaciones**. Incluye los54 controles de filtros/consultas/recuperación anteriores y9 comprobaciones de diferenciación de rótulos, confirmación visible en móvil, fecha de consulta, nueva respuesta por clic, conservación de vista/filtros/páginas, lectura sin efectos y foco de teclado. Cero errores y peticiones remotas. Capturas e informe excluidos de Git: `.tmp/doc120-propuesta-estado-visible/`. Los cortes de `.tmp/doc120-propuesta-filtros-consultas/`, `.tmp/doc120-propuesta-rotulo/`, `.tmp/doc120-propuesta-alineada/`, `.tmp/doc120-propuesta-simple/` y `.tmp/doc120-propuesta-guiada/` son antecedentes. Solo cambia la maqueta; no constituye una nueva prueba autenticada de la aplicación.

## Auditoría de compatibilidad — 2026-11-13

**Resultado: compatibilidad parcial; no equivalencia funcional total.** Corte auditado: `f3bb9e1698fc35fda60a0655e1cc43838492d915`. Se inventariaron42 manejadores (incluidos controles internos y de demostración) y6 controles de entrada. Los63 controles locales positivos siguen pasando, pero las8 comprobaciones focales adicionales reproducen3 grupos de divergencias y delimitan4 restricciones de simulación. No sumar esos conteos como una certificación integral. [Informe estructurado, huellas, inventario y reproducciones](compatibilidad-maqueta-doc120-20261113.json).

Esta auditoría no modifica el HTML ni la aplicación y no ejecuta autenticación, HTTP del ambiente, SQL o efectos documentales reales. No cierra DOC-120 ni renueva las pruebas históricas.

| Hallazgo | Prioridad | Diferencia y decisión necesaria |
| --- | --- | --- |
| C-01 | ALTA | **Acciones de archivo con padre fuera del resultado visible.** Después de recibir y crear, filtrar carpetas por PENDIENTE oculta el padre. La maqueta sigue ofreciendo Preparar índices y, luego, Almacenar archivo. accionesRecurso del cliente real devuelve[] en ambos casos. Reutilizar o reproducir la guarda completa de accionesRecurso, incluida la carpeta padre visible, su estado e intento activo. Ajustar la maqueta al cliente construido; no relajar las guardas del producto para igualar el ejemplo. |
| C-02 | MEDIA | **Formato CSV diferente.** La maqueta genera5 columnas separadas por punto y coma. ExportarPaginaLote genera13 columnas separadas por coma y aplica CeldaCsv. El alcance por página/filtro coincide; el contrato del archivo no. Reproducir las13 columnas, delimitador, escapado y protección de fórmulas del servicio si el HTML se usa como referencia del CSV. El prefijo DEMO puede conservarse para identificar datos ficticios. |
| C-03 | MEDIA | **Normalización automática de paginación y tamaño fijo.** pageOf cambia página2 a1 al quedar25 filas. El servicio conserva filtro.Pagina y LIMIT/OFFSET devuelve vacía la página2. El cliente real ajusta tamaño con Math.min(25,maximoTamanoPagina); la maqueta fija25. Conservar la página solicitada y mostrar vacío cuando corresponda; reiniciar únicamente donde el cliente lo hace (Aplicar/Limpiar). Tomar el tamaño de la política de consulta al integrar. |
| L-04 | MEDIA | **Recuperación real no representada.** Una recarga real elimina los nuevos lotes de la maqueta; Simular recarga conserva objetos en memoria. La aplicación usa lotes persistidos y pistas de IDs en sessionStorage o consulta manual con autorización vigente. Mantener el límite visible y no presentar Simular recarga como prueba de recuperación real. Conservar el número de lote y el comportamiento del producto al integrar. |
| L-05 | MEDIA | **Verificación siempre positiva en el ejemplo.** Verificar intento siempre termina confirmado en la maqueta; la reconciliación real puede devolver sin resolver. También faltan acuses inválidos, errores de transporte y confirmación todavía no verificable. Añadir estados de ejemplo sin resolución, manteniendo el bloqueo y la referencia del intento; no prometer que verificar siempre libera el flujo. |
| L-06 | MEDIA | **Perfiles, selección y cardinalidad limitados.** Un perfil Egresos fijo, hasta3 carpetas generadas y exactamente1 PDF por carpeta. No hay selección real, Más perfiles, quitar archivo individual, carpeta con varios PDF, integridad LOTE/PARCIAL ni incidencias reales. Antes de integrar el diseño, conservar catálogo y selector existentes y separar raíces de archivos en el modelo de vista; incluir ejemplos de varias páginas y varios PDF por expediente sin convertir3 en límite productivo. |
| L-07 | ALTA | **Autoridad, revisiones e integridad solo simuladas.** El HTML no aplica sesión/CSRF, propietario/empresa, accionesPermitidas, revisión, hash/vigencia del plan, intentos activos o disponibilidad autoritativa de creación. Sus banderas positivas no sustituyen esos contratos. Integrar como capa de presentación consumiendo los DTO y servicios actuales; conservar la reautorización y las barreras de servidor. No se requiere ni se autoriza ejecutar pruebas reales mediante este informe. |

En particular, C-01 es una diferencia de la maqueta respecto del cliente construido: no se recomienda modificar el producto para permitir una operación que su vista actual bloquea. C-02 conserva el alcance página/filtro, pero no el formato CSV. C-03 es una diferencia de página solicitada y tamaño, no una pérdida de la independencia de filtros.

### Matriz completa de manejadores

CORRESPONDENCIA significa que existe el flujo equivalente, no que se haya ejecutado el servicio real. PRESENTACION identifica organización visual sin endpoint nuevo. PARCIAL declara las limitaciones del ejemplo. SOLO_DEMO no es una operación del producto. NO_EXPUESTO no se cuenta como opción visible.

| Opción o manejador | Correspondencia construida | Resultado | Observación |
| --- | --- | --- | --- |
| Agregar carpeta de ejemplo (`add`) | agregar / selector webkitdirectory | PARCIAL | Genera un PDF por raíz y limita a3 ejemplos; no inventaría un directorio real. Véase L-06. |
| Quitar carpeta (`remove`) | quitar-raiz | CORRESPONDENCIA | Elimina la raíz y sus archivos de la selección sin confirmar. |
| Preparar revisión (`review`) | PrepararPlan | PARCIAL | La revisión real valida reglas, catálogos, coincidencias y límites; la maqueta siempre usa datos positivos. Véase L-06, L-07. |
| Cambiar selección (`back`) | etapa / invalidación de preparación | CORRESPONDENCIA | Disponible antes de confirmar; no permite editar el lote confirmado. |
| Confirmar plan (`confirm`) | ConfirmarPlan | PARCIAL | Semántica separada correcta; no reproduce hash, revisión, vencimiento ni validaciones servidor. Véase L-07. |
| Ver acciones de carpeta (`select-folder`) | Selección visual sobre referencia de raíz | PRESENTACION | Agrupa la acción contextual; no representa un endpoint nuevo. |
| Enviar archivo / Enviar N archivos (`send`) | enviarCola / recepción ASHX | PARCIAL | Respeta página y filtro, pero no transfiere bytes ni valida contenido, intento o progreso. Véase L-06, L-07. |
| Crear expediente (`create`) | AvanzarLote / CREAR_EXPEDIENTE | PARCIAL | La disponibilidad real exige recepcionListaParaCrear y ámbito completo; la maqueta equivale a un PDF por expediente. Véase L-06, L-07. |
| Preparar índices (`prepare`) | AvanzarLote / MATERIALIZAR_VALORES | DIVERGENCIA | La maqueta permite la acción con carpeta padre filtrada fuera de la página; el cliente real no. Véase C-01. |
| Almacenar archivo (`store`) | AvanzarLote / ALMACENAR | DIVERGENCIA | Misma diferencia de guarda de carpeta padre; el guardado real conserva intentos y evidencias. Véase C-01, L-07. |
| Reseleccionar archivo (`reselect`) | reseleccionar / validarReseleccion | PARCIAL | La maqueta vincula el ejemplo automáticamente; la implementación compara recurso, nombre, tamaño y fecha y luego valida contenido al recibir. Véase L-06. |
| Actualizar estado (`consult`) | ConsultarLote / ConsultarRecursosLote | CORRESPONDENCIA | Misma semántica de consulta; el rótulo, hora y confirmación visible son propuestas de presentación. |
| Verificar intento (`verify`) | ReconciliarRecurso | PARCIAL | La demostración siempre confirma; el resultado real también puede permanecer sin resolver. Véase L-05, L-07. |
| Manejador genérico antiguo (`filters`) | Sin botón visible en la versión actual | NO_EXPUESTO | Código residual; no contar como una opción visible ni como un filtro adicional. |
| Limpiar filtros (alias contextual) (`clear-filter`) | limpiar-resultados | CORRESPONDENCIA | Limpia únicamente los filtros de archivos. |
| Exportar esta página (`export`) | ExportarPaginaLote | DIVERGENCIA | Página y filtro correctos; formato diferente:5 columnas con punto y coma frente a13 con coma. Véase C-02. |
| Solicitar detención (`stop`) | SolicitarDetencion | CORRESPONDENCIA | Conserva efectos confirmados y no ofrece Pausar/Reanudar. El diálogo es una propuesta visual. Véase L-07. |
| Confirmar solicitud de detención (`confirm-stop`) | SolicitarDetencion | PRESENTACION | Confirmación del diálogo local; no es una operación distinta en servidor. |
| Cerrar asistente (`close`) | cerrar | PARCIAL | La maqueta siempre muestra diálogo; el cliente real lo exige en preparaciones o confirmaciones inciertas. Véase L-04. |
| Confirmar cierre (`confirm-close`) | cerrar / liberación de File y URLs | PARCIAL | Referencia simulada en memoria; no reproduce sesión, cancelación de transporte ni efecto en vuelo. Véase L-04. |
| Consultar importación de ejemplo (`reopen`) | Consulta del lote conocido | PARCIAL | Funciona dentro de esta página; una recarga real pierde el lote nuevo de la maqueta. Véase L-04. |
| Cerrar diálogo / Escape (`dismiss`) | Interacción local | PRESENTACION | No llama a servicios. |
| Cómo funciona (`help`) | Ayuda de presentación | PRESENTACION | No llama a servicios ni habilita fases nuevas. |
| Simular recarga (`reload-demo`) | Control exclusivo de demostración | SOLO_DEMO | No demuestra recuperación real con recarga del navegador. Véase L-04. |
| Simular respuesta perdida (`uncertain-demo`) | Control exclusivo de demostración | SOLO_DEMO | No se envía una operación real ni se pierde un acuse real. Véase L-05. |
| Simular intento incierto (`attempt-demo`) | Control exclusivo de demostración | SOLO_DEMO | Solo ejemplifica creación/almacenamiento y resolución positiva. Véase L-05. |
| Volver al inicio de la maqueta (`reset-demo`) | Control exclusivo de demostración | SOLO_DEMO | No existe una limpieza o rollback del lote productivo equivalente. |
| Reiniciar ejemplo (`confirm-reset`) | Control exclusivo de demostración | SOLO_DEMO | Borra solo el estado ficticio de esta página. |
| Aplicar filtros de carpetas (`apply-roots`) | ConsultarRecursosLote / RAICES | CORRESPONDENCIA | Opciones de Estado de carpeta exactas; aplicar reinicia su página y conserva archivos. |
| Aplicar filtros de archivos (`apply-files`) | ConsultarRecursosLote / ARCHIVOS | CORRESPONDENCIA | Referencia de carpeta, efecto y recepción se combinan; aplicación explícita e independiente. |
| Limpiar filtros de carpetas (`clear-roots`) | limpiar-raices | CORRESPONDENCIA | No borra filtros de archivos. |
| Limpiar filtros de archivos (`clear-files`) | limpiar-resultados | CORRESPONDENCIA | No borra filtros de carpetas. |
| Anterior/Siguiente de carpetas (`page-roots`) | ConsultarRecursosLote / página RAICES | DIVERGENCIA | El ajuste automático al máximo disponible difiere del contrato real cuando el total disminuye. Véase C-03. |
| Anterior/Siguiente de archivos (`page-files`) | ConsultarRecursosLote / página ARCHIVOS | DIVERGENCIA | Misma normalización automática; además usa25 fijos y no el máximo de página anunciado por el servidor. Véase C-03. |
| Mostrar carpeta seleccionada (`show-root`) | Navegación visual y consulta de página | PRESENTACION | Atajo visual; debe conservar filtro y referencia al integrarse. Véase C-03. |
| Mostrar archivo seleccionado (`show-file`) | Navegación visual y consulta de página | PRESENTACION | Atajo visual; debe respetar las guardas de padre y recurso del cliente real. Véase C-01, C-03. |
| Consultar importación (`open-query`) | Abrir consulta por referencia | CORRESPONDENCIA | Acceso sin preparar una nueva importación. Datos y autorización son ficticios. Véase L-07. |
| Consultar referencia / Enter (`query-reference`) | ConsultarLote / ConsultarRecursosLote | CORRESPONDENCIA | Destino original y rechazo sin fallback representados; no valida identidad o permisos reales. Véase L-07. |
| Consultar último lote (`last-query`) | Referencia conocida / consulta del lote | PARCIAL | La maqueta comienza con101 ficticio; el cliente usa la referencia del contexto de sesión. Véase L-04. |
| Importar carpetas desde consulta (`new-selection`) | Acceso existente a nueva importación | PRESENTACION | La maqueta usa un nivel fijo; la implementación debe conservar el nivel seleccionado/pulsado autorizado. Véase L-06, L-07. |
| Simular confirmación sin respuesta (`plan-demo`) | Control exclusivo de demostración | SOLO_DEMO | Crea un ejemplo local, no ejecuta ConfirmarPlan real. Véase L-07. |
| Verificar confirmación pendiente (`verify-plan`) | ConsultarConfirmacionLote | CORRESPONDENCIA | Representa la consulta sin repetir confirmación; no modela todas las respuestas negativas o inciertas. Véase L-05. |

**No se certifica:** permisos reales, concurrencia, revocación, vencimientos, integridad con varios archivos, recuperación tras cerrar navegador, resolución de todos los intentos inciertos, contenido PDF, almacenamiento real, SII ni capacidad. La fuente de verdad sigue siendo el cliente y los servicios del repositorio. Los cambios propuestos de rótulo, tres grupos visuales, diálogos y mensaje de actualización requieren únicamente una adaptación de presentación cuando se implementen, preservando los contratos actuales.

Los apartados siguientes describen el prototipo anterior `importacion-carpetas.html`, conservado como antecedente.

Abre [importacion-carpetas.html](importacion-carpetas.html) directamente en un navegador de escritorio. Conserva los archivos HTML, CSS y JavaScript en este mismo directorio. No requiere instalación, servidor, conexión a internet ni autenticación.

## Qué representa

Propuesta visual basada en Producción Documental: navegación lateral por estructura, tonos azules/grises, tablas de documentos, acciones de carga y contexto del expediente/gabinete. Es una vista nueva de importación, no una copia exacta ni una modificación de la pantalla desplegada. La navegación lateral es contextual e ilustrativa.

El recorrido implementado visualmente es Árbol existente de Producción → Seleccionar nivel → Abrir ventana de importación → Perfil → Carpetas → Revisión → Resultados. Se puede seleccionar un directorio real para inventariar nombres, rutas relativas y tamaños; no se lee el contenido, no se sube y no se guarda información persistente. Los perfiles, permisos, catálogos, conflictos, IDs y resultados documentales son ficticios. La recarga descarta el estado.

## Recorrido recomendado

1. Seleccionar «Gestión de compras» o «Correspondencia» en el árbol de Producción. Pulsar «Importar carpetas»: se abre la ventana con ese destino de solo lectura. Elegir «Órdenes de trabajo» y consultar sus reglas. El nivel padre es independiente del gabinete de documentos.
2. Continuar a Carpetas y pulsar «Cargar ejemplo» para agregar dos raíces con cuatro documentos. También se puede usar «Agregar carpeta» repetidamente.
3. Revisar los documentos y abrir «Ver» para comprobar expediente, tipología e índices resueltos.
4. Cambiar el escenario a «Conflicto de expediente» para ver cómo se bloquea la confirmación.
5. Cambiar a «Respuesta perdida al almacenar», marcar la revisión y confirmar la simulación.
6. Consultar el resultado incierto y pulsar «Simular verificación del resultado». La demostración encuentra el documento; no lo vuelve a almacenar. Este resultado está programado para enseñar el flujo, no implementa reconciliación real.
7. Exportar el CSV de resultados ficticios o iniciar una nueva importación.

El perfil «Expedientes administrativos» muestra una segunda interpretación del nombre: descripción completa y tipología constante. Los separadores, campos y límites de la maqueta son datos de ejemplo, no una especificación rígida para producción.

La estructura resultante se muestra en Carpetas, Revisión y Resultados: cada raíz propone un expediente bajo el nivel recibido al abrir. No existe selector de estructura dentro de la ventana. Para otro destino se cierra, se selecciona otro nodo del árbol y se abre una nueva preparación. En la maqueta, esa nueva apertura descarta el inventario local anterior; no se eliminan efectos documentales porque todos son simulados. «Nueva importación» dentro de la ventana conserva el nivel de origen. El lote simulado conserva una copia de su destino; el cambio de perfil no modifica el nivel. En implementación se validarán perfiles compatibles y los permisos del contexto en servidor.

## Archivos y capturas

| Archivo | Función |
|---|---|
| [importacion-carpetas.html](importacion-carpetas.html) | Estructura, navegación de etapas, panel de destino y diálogo accesible. |
| [importacion-carpetas.css](importacion-carpetas.css) | Presentación autónoma adaptable a escritorio y móvil, sin CDN. |
| [importacion-carpetas.js](importacion-carpetas.js) | Estado local, selección de archivos, ejemplos, validación ilustrativa y ejecución simulada. |
| [vista-estructura.png](vista-estructura.png) | Selección en el árbol de Producción y acción para abrir la importación. |
| [vista-perfil.png](vista-perfil.png) | Primera etapa en escritorio. |
| [vista-revision.png](vista-revision.png) | Inventario, validación y confirmación. |
| [vista-resultados.png](vista-resultados.png) | Resultado parcial con operación por verificar. |
| [vista-movil.png](vista-movil.png) | Adaptación móvil y ejemplo de archivos homónimos. |

## Mapeo para implementar en ASP.NET

La política completa/parcial se detalló después de este prototipo: su simulación secuencial y bloqueo de todos los errores no implementan esa política. En la implementación prevalece la guía 00: recibir/validar todos los archivos de la unidad exigida antes de sus efectos, o procesar elegibles si el perfil permite parcial. Ambos ámbitos, por expediente y por lote, están aprobados como configuración del perfil; esta maqueta no implementa sus barreras reales. El HTML es base gráfica aprobada, no referencia ejecutable de atomicidad, idempotencia o reconciliación real.

| Elemento visual o función | Integración real requerida |
|---|---|
| Selector `profile` y panel de destino | Consultar perfiles publicados autorizados y su versión desde el servidor. Reemplazar el objeto `profiles` de ejemplo. |
| Acción `open-import` desde el árbol existente | Recibir ID/ámbito del nivel seleccionado, verificar elegibilidad y permisos, abrir la ventana con destino de solo lectura y conservarlo en el plan. Verificar la relación expediente–nivel después de crear. No implementar otro selector ni creador de estructuras. |
| Colección de raíces/archivos y `selectedFolders` | Conservar IDs de selección y referencia `File`; enviar manifiesto validado y obtener IDs persistidos. No identificar únicamente por nombre. |
| `validate`, reglas y detalle | Sustituir validación ilustrativa por preflight del servicio. El cliente puede anticipar errores, pero no autorizar ni decidir el destino final. |
| Confirmación | Enviar identidad/revisión de un plan completo asociado al usuario. Confirmar de manera idempotente en servidor. |
| `startSimulation`, `tick`, `markDone` | Sustituir completamente por recepción segura, ejecución acotada y consulta de estado persistido. Los temporizadores actuales solo animan la maqueta. |
| Pausa, cancelación y verificación | Implementar contratos de pendientes/operaciones en vuelo y reconciliación real; no suponer cancelación de efectos por detener JavaScript. |
| IDs y progreso | Mostrar identificadores reales y distinguir bytes recibidos de efectos documentales confirmados. |
| Exportación | Generar a partir de resultados autorizados y persistidos; mantener protección frente a fórmulas en CSV. |

Integrar en la arquitectura WebForms/ASMX/ASHX existente según el prompt, evitando copiar el documento completo dentro de una página con otro formulario. Adaptar selectores si WebForms genera ClientID, usar botones que no provoquen postbacks accidentales y asegurar inicialización única o limpieza de eventos tras actualizaciones parciales. Encapsular estilos bajo el contenedor del módulo antes de incorporarlos a la aplicación: esta maqueta usa selectores globales porque es autónoma. Evaluar compatibilidad del diálogo nativo con los navegadores objetivo o adaptarlo al modal existente.

La interfaz no decide permisos, clasificación, coincidencias, tablas ni rutas físicas. Reutilizar operaciones documentales mediante adaptadores compatibles; mantener SII intacto. La implementación de tablas se entrega en SQL MySQL 5.1 para ejecución manual del equipo, conforme al [prompt de carga](../03-prompt-implementacion-carga-carpetas.md).

## Verificación realizada

Se ejecutó una prueba local con Chromium headless/Playwright sobre el archivo HTML, sin autenticación ni conexión al sistema. Comprobó navegación, ejemplo, consulta de detalle, conflicto que bloquea confirmación, resultado incierto y verificación simulada, pausa/reanudación, selección real de una carpeta de prueba, archivos homónimos en subcarpetas, rechazo de extensión, cancelación y descarga CSV. Se comprobó ausencia de errores JavaScript y tráfico HTTP; a 390 px no hubo desbordamiento horizontal de página. Se revisaron visualmente las capturas de escritorio y móvil.

Esto no certifica integración ASP.NET, permisos, carga real, acceso a base de datos, rendimiento masivo ni compatibilidad de selección de carpetas en otros navegadores/dispositivos. El selector no representa carpetas vacías y la maqueta limita el inventario a 500 archivos. No hay lectura por arrastre, recuperación persistente ni carga desatendida.

Tras la precisión del flujo de entrada se repitió la prueba: la acción de importar permanece deshabilitada sin nivel seleccionado; abre un diálogo desde el árbol; no hay segundo selector de nivel; cambiar de perfil conserva el destino; cerrar y abrir desde otro nodo recibe el nuevo contexto. Se regeneraron las capturas.
