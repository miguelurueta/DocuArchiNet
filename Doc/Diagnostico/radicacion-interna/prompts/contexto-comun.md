# Contexto común de implementación de radicación interna

Esta serie prepara la implementación operativa de la capacidad explorada: radicar desde Producción o desde carga directa, representar usuarios internos o terceros y aplicar trámites sin seguimiento, con confirmación o con respuesta formal. Redactar los prompts no ejecuta la implementación ni activa configuración del ambiente.

La serie vigente tiene 6 prompts P01–P06. Las referencias RI00–RI11 del mapa y de las decisiones son subfases internas: P01 contiene RI00; P02 contiene RI01/RI02/RI03; P03 contiene RI04; P04 contiene RI05/RI06; P05 contiene RI07/RI08/RI09; P06 contiene RI10/RI11. Conservar su orden y criterios; no son doce prompts adicionales.

## Insumos y aprobaciones

Leer [diagnóstico](../diagnostico.md), [carga directa](../exploracion-carga-directa.md), [trámites con respuesta](../exploracion-tramites-con-respuesta.md), [mapa de funciones](../mapa-funciones-implementacion.md) y [guía](00-guia-ejecucion.md). La base revisada es `226d2000`; comprobar cambios posteriores antes de editar.

- D-RI-01 aprobada: usuario interno o tercero como remitente; el actor autenticado permanece como ejecutor y auditoría. El responsable Workflow es una identidad independiente.
- D-RI-02 aprobada: formulario compacto de `23464d6b`. Las variaciones solicitadas posteriormente están en `maqueta.html`: terceros, carga directa y tres modalidades de respuesta. Sus datos, límites y asociaciones son ficticios; no son una configuración aprobada del ambiente.
- D-RI-03 confirmada por el usuario el 2026-11-26: este módulo registra la radicación y crea/entrega el seguimiento cuando corresponde. La elaboración, envío, confirmación y cierre de respuestas pertenecen al módulo existente de respuestas. La aceptación de esta entrega termina en obligación disponible y legible por ese consumidor, sin trasladar su gestión a esta pantalla.
- D-RI-04 sustituida por D-RI-05: la aprobación anterior de adaptar el cargador compartido queda sin vigencia para esta implementación.
- D-RI-05 aprobada por el usuario el 2026-11-26: crear un control de carga nuevo e independiente para radicación interna, conservando la interfaz compacta aprobada. Propuesta: `js/radicacion/RadicacionInternaUpload.js`, clase `RadicacionInternaUpload`, con estado por instancia/operación y recepción temporal propia antes del radicado. No adaptar ni depender de `LoadFilePERSON` / `IniLoadPerson` ni modificar `generic_control/FileUploadHandler.js`, `FileUploadHandler_.ashx.vb` o sus eventos para habilitar este recorrido. Los consumidores actuales conservan su cargador.
- D-RI-06 incorporada por instrucción del usuario el 2026-11-26: tratar radicación, consecutivos, documentos, seguimiento y Workflow como núcleo crítico. Caracterizar primero el comportamiento y consumidores, conservar contratos y reglas, introducir adaptación de entrada y justificar cada modificación indispensable. Verificar concurrencia, fallos alrededor del commit y recuperación sin duplicados antes de aceptar la integración. La independencia de la UI/ASMX no elimina el riesgo de clases/tablas comunes ni autoriza una reescritura del motor.
- D-RI-07 exigida por el usuario el 2026-11-26: la función de almacenamiento es intocable. Se protege el archivo completo `workflow/ClassAlmacenamiento.vb` y las funciones existentes de preparación, metadatos y vinculación inventariadas en `almacenamiento-intocable-baseline.json`. Prohibido editarlas, extraerlas, agregarles sobrecargas, clonarlas, reprogramarlas indirectamente mediante helpers/configuración o sustituirlas por escritura directa de archivos/tablas definitivos. Se permite preparar datos y consultar/reconciliar desde un adaptador nuevo externo usando las firmas vigentes. Esta prohibición prevalece sobre las autorizaciones genéricas de cambio mínimo de D-RI-06 y sobre el texto anterior de los paquetes.
- Se conserva ASP.NET WebForms, VB.NET, .NET Framework 4.6.1, MySQL y los contratos vigentes. No migrar a ASP.NET Core/React ni trasladar esta tarea a otros repositorios por conveniencia.
- Esta serie no es DOC-127 ni la modernización del árbol. No alterar sus tareas, tickets o evidencia para alojar el nuevo trabajo.

Al recibir la instrucción de ejecutar un paquete, refinar sus decisiones y aplicar el alcance autorizado. No pedir nuevamente permisos o acuerdos ya registrados. Resolver elecciones técnicas dentro del alcance con evidencia; consultar solamente las decisiones funcionales pendientes que afecten a esa parte. Su ausencia no impide avanzar con trabajo independiente. Una opción recomendada aquí no se transforma en aprobación por silencio.

## Requisitos de la capacidad

| Requisito | Resultado observable |
|---|---|
| RQ-RI-01 | Desde Producción se conserva principal/anexos, expediente, plantilla y autoridad; principal no se cuenta dos veces. |
| RQ-RI-02 | Entrada independiente autorizada permite recibir archivos reales, verificar su recepción y confirmar sin visitar Producción. |
| RQ-RI-03 | Remitente interno/tercero se resuelve con tipo e identidad; actor y responsable conservan su función. |
| RQ-RI-04 | Plantillas dinámicas preservan campos, tipos, catálogos, scripts, obligatoriedad, fechas y valores tras postback. |
| RQ-RI-05 | Trámite, flujo, actividad, grupo y responsable son coherentes y revalidados en servidor. |
| RQ-RI-06 | Modalidad de respuesta y vencimiento proceden de configuración autorizada; seguimiento tiene propietario correcto. |
| RQ-RI-07 | Una confirmación repetida o incierta no vuelve a crear el mismo radicado ni sus documentos/seguimiento. |
| RQ-RI-08 | Resultados distinguen rechazo, registro confirmado, preparación/asignación/notificación pendiente e incertidumbre. |
| RQ-RI-09 | Lecturas, uploads, cambios, descarga y recuperación verifican actor, ámbito, operación y recurso en cada petición. |
| RQ-RI-10 | Interfaz accesible y compacta muestra recepción real, validaciones, resumen y cierre correcto sin fingir éxito. |
| RQ-RI-11 | Se preservan consumidores de Producción, Radicación clásica/Simplificada y Workflow; no cambia indiscriminadamente su política. |
| RQ-RI-12 | Compilación, esquema, configuración, publicación y pruebas integradas acreditan un recorrido real autorizado. |

## Decisiones pendientes y paquetes afectados

| Puerta | Origen | Definición pendiente | Paquetes |
|---|---|---|---|
| G01 | Q-CD-01, Q-CD-06 | Expediente antes de confirmar y registros internos de Producción, o clasificación posterior con almacenamiento independiente. | RI00, RI02, RI06, RI07, RI10 |
| G02 | Q-RI-01/02 | Catálogo de terceros, ámbito y tipos admitidos. | RI04, RI10 |
| G03 | Q-RI-03, Q-CD-02 | Autoridad de representación, carga y radicación; acceso y plantillas permitidas. | RI02, RI04, RI05, RI10 |
| G04 | Q-RI-04 | Solo terceros existentes o creación/edición en este recorrido. Propuesta inicial: seleccionar existentes. | RI04, RI10 |
| G05 | Q-RI-05 | Datos del remitente en impresión, consultas y notificación; referencias de identidad que se entregan al consumidor existente. | RI04, RI08, RI09 |
| G06 | Q-CD-03 | Tipos, límites por archivo/total/cantidad, verificación de contenido y folios. Los valores de la maqueta no deciden esta política. | RI05, RI06, RI10 |
| G07 | Q-CD-04 | Orígenes separados o mezcla en una operación. Propuesta inicial: separados. | RI02, RI05, RI06, RI10 |
| G08 | Q-CD-05 | Conservación/reanudación del borrador, caducidad y recuperación. | RI02, RI05, RI08 |
| G09 | Exploración de respuesta; límite D-RI-03 | Identificar el consumidor vigente y su contrato de entrega. No es una decisión de construir aquí respuesta saliente o interna; configuración ausente/incompatible se registra como dependencia externa. | RI03, RI09, RI10 |
| G10 | Exploración de respuesta | Trámites elegibles y configuración por plantilla para registrar la obligación, comprobando compatibilidad con el consumidor sin rediseñar su homologación ni retirar los clásicos. | RI03, RI09 |
| G11 | Exploración de respuesta; límite D-RI-03 | Valor funcional de cero días y edición del plazo al radicar. Las políticas existentes de transferencia/cierre se conservan y solo se comprueba su compatibilidad. | RI03, RI09, RI10 |

Cerrar estas decisiones al abrir el paquete que las consume; no exigirlas todas para empezar el refinamiento. Para G01, expediente previo permite estudiar la incorporación por los contratos vigentes. La clasificación posterior queda condicionada a demostrar una ruta ya compatible con el almacenamiento intocable: no se aprueba ni se promete viabilidad pasando cero o modificando el motor. Si requiere tocar una función/configuración protegida, ese recorrido no se habilita bajo el alcance actual; registrar la incompatibilidad y continuar con trabajo independiente.

## Arquitectura y contratos propuestos

### Protección del núcleo crítico, D-RI-06

Leer el [diagnóstico del WebService compartido](../diagnostico-webservice-radicacion.md) y actualizar el inventario de consumidores al ejecutar P01. El `WebServiceRadicacion.asmx` existente es transversal; el registro interno actual pasa por postback y `ClassRadicador.Registra_Radicacion_entrante_interna`. Mantener sus contratos, formatos y reglas de numeración, límites transaccionales, permisos, relaciones documentales, seguimiento y efectos Workflow.

Antes de modificar una función compartida, registrar en `implementacion/contratos-v1.md` su firma, consumidores, invariantes, efectos, conexiones/transacciones, comportamiento anterior y verificación de caracterización. P01 documenta esa línea base y los casos; el paquete funcional ejecuta las pruebas correspondientes antes del cambio y las compara después. Esta es una precondición técnica verificable, no un nuevo trámite de aprobación del trabajo ya autorizado. Una decisión funcional pendiente y la autorización del ambiente conservan sus límites propios.

La entrada nueva debe usar un adaptador/orden tipada y un contexto autorizado. No se presupone que un wrapper baste: las dependencias actuales de `Page`, controles, sesión y documentos de Producción pueden exigir extracción mínima. Cada cambio compartido debe indicar qué requisito lo hace indispensable, alternativas evaluadas, consumidores, evidencia previa/posterior y recuperación/reversión. No reescribir ni clonar el monolito ni alterar métodos generales, sus filtros o tipos para facilitar la nueva pantalla. Si la necesidad excede el alcance acordado, documentar el límite y mantener pendiente esa integración mientras continúa el trabajo independiente; no ocultar una migración transversal dentro de la UI.

La aceptación compara recorridos vigentes y nuevos: numeración y registros, rollback antes de commit, respuesta perdida después, dos peticiones concurrentes, incorporación documental, seguimiento único, Workflow y correo pendiente. Una respuesta incierta exige consultar efectos durables; no ejecutar de nuevo el registro completo. Un servicio separado no garantiza aislamiento de clases, bases o gabinetes. Compilar, mantener una firma o pasar pruebas estáticas tampoco demuestra equivalencia funcional ni ausencia de regresiones.

### Almacenamiento intocable y metadatos, D-RI-07

Leer el [diagnóstico de almacenamiento y metadatos](../diagnostico-almacenamiento-metadatos.md) y la [baseline protegida](../almacenamiento-intocable-baseline.json). El archivo central queda congelado íntegramente; en los otros archivos la baseline protege funciones concretas para permitir cambios de UI externos a ellas. El verificador compara con el commit de origen y normaliza CRLF/LF; no se modifica la baseline ni el verificador para permitir un cambio prohibido. El control estático acredita inmutabilidad de esas fuentes, no la corrección del almacenamiento ni su ejecución en el ambiente.

El adaptador externo entrega rutas temporales propias, contexto autenticado y valores válidos a las firmas actuales. Mantener por separado metadatos del formulario/radicado, inventario de Producción, índices de gabinete y XML de expediente; preservar tipos, dimensiones, orden `IDENTI`, relaciones y precedencias vigentes. No confundir remitente tercero con usuario de almacenamiento, folios del formulario con páginas detectadas, ni fecha documental con fechas de carga/incorporación. Identificar por separado fuente, copia y rótulo/certificado; un `ByRef` de imagen puede corresponder al último documento generado.

El almacenamiento puede consumir temporales y usar `WF_RUTA_TEMPO_ADJUNTA`/`WF_RUTA_TEMPO_FINAL`. Preparar una copia de trabajo propia y demostrar aislamiento del contexto legacy desde fuera, sin modificar el motor, borrar otro borrador ni sobrescribir el contexto de otra operación. No afirmar que ese aislamiento ya existe ni resolverlo relajando permisos. Si la ruta requiere mutar una función protegida o no permite aislamiento seguro, registrar la dependencia y mantener pendiente esa integración.

Hay commits internos y efectos físicos/XML que no comparten una única transacción con el radicado. No repetir almacenamiento al recibir error/timeout sin consultar evidencia; un error posterior al commit no implica ausencia del documento. La correlación y reclamación se guardan en el estado nuevo de la operación, sin modificar el esquema/SQL del motor. Si no existe evidencia inequívoca de un efecto, conservar incertidumbre y atención controlada en lugar de prometer reconciliación automática o duplicar archivos.

Verificación obligatoria antes y después de cada paquete y en la entrega: `node tools/diagnostico/verify-radicacion-storage-freeze.cjs`. P04/P05/P06 exigen además comparación autorizada de contenido y metadatos, relaciones, índices, páginas y limpieza, respetando conversiones existentes como Multi-TIFF y el rótulo generado. El SHA-256 del temporal nuevo no sustituye las huellas del índice legacy ni prueba por sí solo equivalencia de todos los derivados.

Los nombres nuevos del mapa son **propuestas**, no componentes existentes ni firmas ya aprobadas. RI00 fija nombres, tipos, rutas y contrato v1; luego cada paquete actualiza esa versión sin romper consumidores. Usar límites tipados entre presentación, aplicación y persistencia. Session y controles WebForms pertenecen a sus adaptadores; la lógica nueva no recibe `Page`, `UpdatePanel` ni rutas arbitrarias del navegador.

Durante RI00 crear `implementacion/contratos-v1.md` con inventario de archivos/firmas, transportes, esquema y secuencias, y `implementacion/decisiones.md` con acuerdo, alternativa, fecha, paquetes afectados y estado de G01–G11. Registrar quién aporta la decisión sin inventar aprobaciones. Mantener esos archivos y la revisión del contrato durante las entregas; actualmente son salidas previstas, no documentos aprobados existentes.

La operación de radicación reúne origen, actor autenticado, ámbito, plantilla/configuración, remitente tipado, contexto documental, manifiesto versionado, datos, destino, reglas de respuesta y resultado. El cliente envía intención y referencias; servidor determina autoridad, rutas físicas, nombres de tablas, reglas y consecutivos. Validar formatos/rangos antes de convertir IDs a los tipos legacy; si se transportan como cadenas decimales, no convertirlos a `Number` de JavaScript sin comprobar precisión.

Operaciones públicas candidatas: abrir, consultar/configurar borrador, buscar remitentes/destinos, actualizar datos/selección, preparar confirmación, recibir/consultar/retirar archivo, confirmar, consultar resultado/seguimiento y cancelar. Publicar únicamente las necesarias. El upload binario necesita transporte propio; no reutilizar `ADJUNTARADICACION` ni `ADJUNTARADICACION_CLASICA` para un radicado inexistente. Conservar esos contratos.

D-RI-05 distingue el nuevo control N13 de los servicios de recepción N06/N07/N12. Su endpoint y resultado temporal pertenecen a la operación nueva; no usa callbacks, variables globales ni rutas de adjunto del cargador anterior. La selección de principal/anexos, progreso y errores se integra en el formulario aprobado. Los servicios de almacenamiento existentes se reutilizan detrás de adaptadores cuando sus contratos lo permiten; esto no autoriza modificar el cargador compartido ni convertir un evento de adjunto posterior en recepción previa. G01, G03, G06, G07 y G08 siguen en refinamiento: aprobar el control no fija expediente, permisos, límites, mezcla de orígenes ni caducidad.

El borrador y sus recepciones requieren referencias internas, revisión y almacenamiento durable suficiente para la política elegida. Antes de confirmar, revalidar actor, expediente/tipología cuando aplique, plantilla, remitente, destino y versión exacta del manifiesto. Cada archivo tiene rol explícito, referencia propia, estado y evidencia de recepción. El nombre del usuario o archivo no identifica una operación.

Estados de negocio propuestos: BORRADOR, RECIBIENDO, LISTO_PARA_CONFIRMAR, CONFIRMANDO, REGISTRADO, REGISTRADO_CON_PENDIENTES, RECHAZADO_ANTES_DE_REGISTRAR, RESULTADO_INCIERTO y CANCELADO_ANTES_DE_REGISTRAR. Registrar por separado las fases documental, Workflow, respuesta y notificación. RI00 concreta transiciones y códigos públicos; un HTTP 200 o un texto `YES` no demuestra todas las fases.

La idempotencia necesita una relación durable de operación/clave/versión con el radicado y una reclamación de ejecución atómica. La inserción del radicado, su obligación de respuesta y la confirmación de la operación deben compartir transacción cuando pertenezcan al mismo recurso transaccional. Las conexiones/gabinetes/archivos externos se tratan con fases y reconciliación. No envolver la llamada monolítica actual y asumir que se ha vuelto atómica.

El código actual usa `SELECT ... FOR UPDATE` al reservar consecutivos. Preservar ámbitos, formato, actualización y reglas de código general/corto/simple al extraer el núcleo. Ante resultado incierto, consultar evidencia durable antes de reintentar. La recuperación no llama otra vez al registro completo ni repite correo/Workflow sin verificar su efecto.

## Recorrido de integración

Desde Producción: selección explícita → apertura autorizada con referencias propias → formulario/plantilla → revisión → confirmación → resultado y actualización del padre.

Desde carga directa: acceso autorizado → borrador → recepción real de principal/anexos → datos y clasificación acordada → revisión del manifiesto → incorporación controlada → registro común → seguimiento/Workflow/notificación → consulta y recuperación. La obligación se entrega al módulo existente de respuestas; el recorrido de este módulo no incluye responderla.

La nueva entrada usa el mismo comportamiento de aplicación. No requiere visitar Producción, aunque G01 puede conservar sus registros internos. Los datos locales de la maqueta y sus temporizadores no pasan a producción. El seguimiento pendiente no se presenta como una respuesta ya elaborada. Cerrar un modal después del commit no cancela el registro.

## Fronteras de código

Rutas propuestas, a concretar en RI00: `Modelo/RadicacionInterna/` para reglas/puertos, `DTOs/RadicacionInterna/` para contratos, `Services/RadicacionInterna/` para casos de uso, `Infrastructure/Repositories/RadicacionInterna/` para SQL parametrizado y transacciones, e `Infrastructure/RadicacionInterna/` para Session/IO/adaptadores legacy. Transportes: `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb`, `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb`. Cliente: página existente, `js/radicacion/WebFormRadicacionEntranteInterna.js`, recursos nuevos justificados bajo `js/radicacion/` y `Styles/radicacion-interna.css`.

No crear capas vacías ni introducir un ORM o dependencia por llenar esta lista. Registrar archivos Compile/Content en `GestionDocumental-Docuarchi.net.vbproj` y verificar su publicación. El esquema durable necesita migración versionada en la ubicación acordada con el patrón real del repositorio; no modificar bases reales al generar el script ni tratar una tabla propuesta como existente.

## Verificación y entrega

Cada paquete incorpora pruebas de comportamiento de sus subfases, compilación MSBuild cuando cambie VB.NET/proyecto y su integración autorizada cuando corresponda. Las pruebas estáticas y la maqueta complementan; no sustituyen recepción, transacción, descarga ni entrega real del seguimiento al consumidor. La evidencia futura se registra bajo `implementacion/Pxx-resultado.md`, distinguiendo las subfases RI; esos archivos todavía no existen por redactar la serie.

Reutilizar `tools/e2e`, autenticación/configuración y runbook del repositorio. No crear login ni proyecto Playwright paralelo. Sin autorización de ambiente, cuentas y datos descartables, continuar pruebas aisladas y registrar pendiente la aceptación integrada afectada; no fingirla completada. No introducir `WorkflowCentroTrabajoModernActive` ni listas de usuarios/grupos como activación. Documentar cambios, comandos, resultados, límites y reversión aplicable; conservar avances ajenos en Git.
