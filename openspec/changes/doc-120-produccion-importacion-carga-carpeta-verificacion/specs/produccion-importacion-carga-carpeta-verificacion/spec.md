<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## Purpose

Consolidar la verificación trazable de importación de carpetas sobre la interfaz definitiva, diferenciando evidencia histórica, pruebas focales, límites operativos y autorización de cada ejecución.

## ADDED Requirements

Reglas acordadas para planificación tras la exploración de rendimiento: [procesamiento independiente](../../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/09-procesamiento-independiente-archivos.md). A no depende de la disponibilidad física de B; la completitud se evalúa por resultados al cierre. Ante faltante conocido, la cola espera Continuar con los demás/Pausar para revisar. Los escenarios IND y las precisiones de creacion/pausa/CAP-08 se implementan localmente tras "implementa", con el contrato focal y script09 manual de3.171/3.197; las pruebas reales3.187/3.196 permanecen pendientes. Las guardas de autoridad, identidad y exclusión de escritor activo permanecen obligatorias.

### Requirement: RQ-01 — Base y vigencia por componente
DOC-120 SHALL cumplir la decisión D-01: Fijar el merge de DOC-119 como base funcional. Cada caso conserva SHA/huellas de sus componentes, prueba, ambiente y resultado originales; la identidad de huellas permite reutilización local, no una ejecución real nueva. Un cambio invalida únicamente los casos dependientes; el mapa explicita esa dependencia.

#### Scenario: Código vigente
- **WHEN** se contrasta una evidencia con la versión final
- **THEN** se comparan sus componentes y huellas, conservando SHA/runtime/ambiente de origen
- **AND** solo los casos dependientes de un cambio se revisan o reejecutan

#### Scenario: Cambio posterior
- **WHEN** cambia la interfaz o un componente después de una corrida
- **THEN** se registra el nuevo SHA y el impacto antes de aceptar la versión
- **AND** igualdad de huellas no se presenta como una sesión real nueva

### Requirement: RQ-02 — Matriz e índices únicos
DOC-120 SHALL cumplir la decisión D-02: Crear matriz-verificacion.md y resultados.md en Validacion, ausentes en la base inspeccionada. La matriz relaciona requisito/caso, componente/símbolo, prueba, SHA/huella, ambiente, evidencia, resultado, vigencia y límites. Resultados mantiene cronología y enlaza fuentes primarias sin sobrescribirlas.

#### Scenario: Índices con procedencia
- **WHEN** se incorpora un caso a la matriz
- **THEN** figuran requisito/caso/componente/prueba/SHA/ambiente/evidencia/resultado/vigencia/límite
- **AND** resultados enlaza el primario y preserva su cronología

#### Scenario: Clasificación y conteos
- **WHEN** se reutilizan resultados históricos o locales
- **THEN** se distinguen REAL_HISTORICO, LOCAL_HISTORICO_VIGENTE, LOCAL_DOC120, ESTATICO y NO_EJECUTADO
- **AND** no se suman suites superpuestas ni se etiqueta CI como E2E integral

### Requirement: RQ-03 — Consumo de EXP/ALM/COORD/SEG
DOC-120 SHALL cumplir la decisión D-03: Usar resultados efectivos y sus cortes finales. DOC-116 resolvió su ventana interna mediante variante de laboratorio; DOC-117 acredita ALM-01/02/03C con límites; DOC-118 distingue archivo administrativo de cierre técnico38/38; DOC-119 conserva SEG-01 real y SEG-02 con cobertura mixta aceptada. No sumar suites de cortes distintos ni reproducir efectos inciertos retenidos.

#### Scenario: Resoluciones posteriores
- **WHEN** un informe contiene bloqueos históricos y una resolución posterior
- **THEN** se identifica el corte efectivo con su fuente primaria y variante
- **AND** el archivo administrativo DOC-118 no sustituye su cierre técnico38/38

#### Scenario: Efecto incierto conservado
- **WHEN** un intento permanece EN_CURSO/público INCIERTO
- **THEN** se conserva con sus identidades y límites
- **AND** no se repite almacenamiento ni se borra o edita su checkpoint para obtener PASS

### Requirement: RQ-04 — Interfaz definitiva y regresión focal
DOC-120 SHALL cumplir la decisión D-04: Mapear UI01–05 al menú superior/contextual y asistente resultantes de DOC-119. Cubrir seleccionado/pulsado, homónimos, selección inválida, autoridad denegada/revocada, referencia/último lote/confirmación sin acuse, recarga/reapertura/UpdatePanel, árbol, foco y viewport. Reutilizar fuentes vigentes y ejecutar solo casos afectados o brechas documentadas.

#### Scenario: Consultar último lote sin referencia local ni perfil
- **WHEN** la cuenta abre el asistente sin referencia guardada y pulsa Consultar último lote
- **THEN** el botón está disponible sin elegir perfil y consulta la última importación confirmada propia de la empresa mediante ConsultarUltimoLote
- **AND** abrir el resultado conserva la selección previa y no confirma ni reanuda importaciones

#### Scenario: Consulta del último lote con identidad y destino autorizados
- **WHEN** se solicita la búsqueda sin enviar usuario, empresa, perfil o nivel
- **THEN** el servidor limita su SELECT al solicitante y empresa de sesión, ordena por lote descendente y devuelve como máximo un candidato
- **AND** antes de publicar su referencia revalida propietario, destino original, snapshot y permisos; la denegación no revela un lote ajeno

#### Scenario: Ausencia y fallo de búsqueda conservan la consulta manual
- **WHEN** no existen lotes propios o el servidor no responde a la búsqueda
- **THEN** informa ausencia o fallo según evidencia y mantiene Consultar importación por referencia, sin inventar IDs ni reutilizar otro contexto
- **AND** cerrar antes de la respuesta impide iniciar otra consulta; no hay envíos, creaciones o cambios de estado

#### Scenario: Destino y autoridad
- **WHEN** se valida acceso superior o contextual en la UI definitiva
- **THEN** se conserva seleccionado frente a pulsado, ruta autorizada y destino fijo
- **AND** nodos excluidos y autoridad denegada no provocan fallback

#### Scenario: Recuperación y avisos
- **WHEN** se consulta referencia/último lote o confirmación sin acuse tras recarga/postback
- **THEN** se conserva identidad, acceso equivalente, avisos, árbol/selección y foco
- **AND** no se reconfirma automáticamente ni se atribuye un caso sintético a IIS

#### Scenario: Revisión que supera un límite JSON comprobado
- **WHEN** el usuario prepara una selección cuyo cuerpo JSON excede el límite publicado por el servidor
- **THEN** el cliente detiene esa solicitud antes de enviarla, explica cantidad/tamaño/máximo y conserva selección/perfil, sin afirmar que se creó una revisión o un lote
- **AND** sin ese metadato solo HTTP413 o un error ASMX estructurado y reconocido justifican el aviso de tamaño; las respuestas desconocidas mantienen incertidumbre sin exponer Message/StackTrace ni atribuir automáticamente el fallo a la cantidad de PDF

#### Scenario: Primer expediente confirmado en nivel vacío
- **WHEN** una consulta posterior a la importación informa un expediente confirmado y el nivel original visible todavía no tiene contenedor de hijos
- **THEN** se agrega su relación al árbol mediante el insertor existente, con expansor, búsqueda y contador, una sola vez
- **AND** se conservan selección/expansión existentes; una rama ausente/oculta/ambigua o un fallo de inserción produce aviso sin repetir efectos, y Actualizar vuelve a intentar únicamente la proyección

#### Scenario: Etiqueta por código independiente del nombre informativo
- **WHEN** se crea y consulta un expediente importado cuyo código es distinto de su nombre
- **THEN** la creación guarda el código como ALEAS_EXPEDIENTE y el nombre como TEMA_EXPEDIENTE; la consulta conserva nombreExpediente y publica codigoExpediente solo desde el valor CODIGO confirmado no diferido de esa raíz
- **AND** el árbol y su búsqueda presentan el código como texto sin fallback al nombre/carpeta; relaciones con códigos iguales permanecen diferenciadas por sus IDs y actualizar una relación existente no repite efectos ni incrementa el contador
- **AND** una verificación de intento incierto con IDs conocidos conserva compatibilidad con el alias histórico igual al nombre confirmado, sin relajar código, clasificación, permisos o destino ni modificar evidencias anteriores

#### Scenario: Barra superior adaptable sin regresión funcional
- **WHEN** se ajusta la barra de Producción Documental y búsqueda a390/768/1024/1280/1554px y con texto aumentado
- **THEN** cada cabecera mantiene juntos icono, texto y flecha; los grupos y búsqueda permanecen accesibles sin desbordamiento horizontal y los desplegables largos pueden recorrerse dentro del panel
- **AND** se conservan las36 acciones estáticas, destinos, permisos, contexto, búsqueda por clic/Enter y restauración; el montaje dinámico no duplica acciones y las alturas del árbol/listado siguen a la barra
- **AND** la paridad local usa las funciones de enrutamiento originales con fronteras de negocio sintéticas y no se atribuye a una prueba IIS autenticada

#### Scenario: Recepción requerida antes de ofrecer creación
- **WHEN** una carpeta elegible no tiene ningún original recibido y utilizable para iniciar su expediente
- **THEN** la UI explica cómo vincular/enviar los pendientes y no ofrece Crear expediente hasta consultar la disponibilidad calculada por el servidor
- **AND** con al menos un PDF propio utilizable y datos de raíz válidos, otro faltante no bloquea crear para ese archivo; el servidor revalida autoridad, revisión y staging del original de referencia sin depender del filtro/página, y la completitud final conserva el inventario original

#### Scenario: Selección reunida sin perder controles
- **WHEN** el usuario abre el asistente reorganizado
- **THEN** elige perfil/versiones y carpetas en Seleccionar, conserva más perfiles, eliminación por archivo/carpeta y páginas del inventario; Revisar precede a confirmar
- **AND** Seguimiento conserva consulta por referencia/último lote/plan pendiente, CSV y paginación independiente
- **AND** el permiso nativo del selector se conserva con explicación previa accesible; Revisar muestra un resumen único y fichas con carpeta, código y nombre completos como texto, mantiene incidencias y datos previstos y ofrece la referencia del plan en detalles sin ejecutar efectos

#### Scenario: Seguimiento explícito con filtros y foco conservados
- **WHEN** se actualiza el estado o se usa una operación disponible
- **THEN** la presentación destaca el recurso y cada operación autorizada por accionesRecurso; el inicio explícito puede coordinar las fases confirmadas según la ampliación aprobada, conservando filtros aplicados/borradores, paneles y foco
- **AND** la actualización válida muestra fecha/contador, los fallos no anuncian éxito y el envío indica cantidad elegible de la página sin atribuir recepción a almacenamiento

#### Scenario: Maqueta coherente con el contrato de consulta
- **WHEN** filtros ocultan la carpeta confirmada, una página se queda vacía o se exporta una página filtrada
- **THEN** la maqueta bloquea índices/almacenamiento sin padre visible, conserva página vacía y usa CSV de trece columnas con coma/escape y protección de fórmulas
- **AND** sus datos y capacidad de página se identifican como simulados y no prueban autenticación ni recuperación real

#### Scenario: Fidelidad visual y consultas siempre visibles
- **WHEN** el usuario recorre selección, revisión o seguimiento en390/768/1280px o desplaza el contenido
- **THEN** la interfaz reproduce los controles y distribución de la maqueta y mantiene visibles Consultar importación, Consultar último lote y el pie de acciones, sin solaparlos
- **AND** la consulta secundaria conserva la preparación y permite volver; no toma archivos locales de esa preparación por coincidencias de idCliente, y relee un lote confirmado al regresar

#### Scenario: Recurso destacado sin efectos implícitos
- **WHEN** el usuario pulsa Ver acciones en una carpeta o archivo
- **THEN** el panel destaca ese recurso y un único botón derivado de sus acciones vigentes, con avance basado en datos confirmados
- **AND** la selección no envía, crea, almacena ni reconcilia, y los originales/estados reales sustituyen los datos simulados de la maqueta

#### Scenario: Retomar creación permanente con confirmación
- **WHEN** la consulta del servidor identifica una reserva de creación retomable en la página vigente
- **THEN** el módulo ofrece Retomar creación y pide confirmar la continuación y el levantamiento de la detención observada
- **AND** cancelar no solicita efectos y confirmar envía una sola fase con referencias/revisiones actuales, sin lotes ni recepciones nuevos

#### Scenario: Parada identificada antes de esperar la consulta posterior
- **WHEN** una operación de recepción, creación, índices o almacenamiento de la coordinación no obtiene confirmación, o su lectura posterior falla
- **THEN** la interfaz muestra inmediatamente la causa comprobada, fase y recurso con nombre/ruta relativa, posición en el inventario cuando se conoce y último avance confirmado; distingue espera de consulta y operación activa
- **AND** timeout conserva los segundos configurados del cliente, respuesta desconocida no inventa causa, acuse confirmado se conserva y consultar no repite ni encadena efectos

#### Scenario: Diagnóstico de parada legible y privado
- **WHEN** el recurso tiene nombre/ruta largos, hay una denegación o el servidor envía texto desconocido
- **THEN** el panel se adapta a390/768/1280px, usa texto seguro y un catálogo cerrado; los datos para soporte permanecen disponibles sin errores internos, rutas absolutas ni secretos
- **AND** la denegación no añade detalle de recurso desde la respuesta; cierre invalida lecturas tardías y no inicia otro archivo

#### Scenario: IND-UI-01 Faltante identificado y decisión pendiente
- **WHEN** no está disponible el File local de un pendiente o el servidor acredita un original no disponible antes de invocar el escritor
- **THEN** el asistente pausa antes del siguiente efecto y muestra archivo/ruta relativa, fase, causa comprobada, certeza y cantidades confirmadas/pendientes
- **AND** ofrece Continuar con los demás y Pausar para revisar sin elección automática, sin alert/confirm nativos y sin confundir falta de archivo con escritura incierta

#### Scenario: IND-UI-02 Continuación elegida sin descartar el faltante
- **WHEN** en A válido, B no disponible y C válido el usuario elige Continuar con los demás ante B
- **THEN** C puede procesarse una vez si cumple sus propias guardas, B queda sin completar e identificado en el mismo inventario y no se vuelve a elegir automáticamente en esa ejecución
- **AND** no se omite una denegación de autoridad, una identidad de expediente inválida ni la exclusión de un escritor todavía activo mediante esta decisión

#### Scenario: IND-UI-03 Pausa, cierre y generación de la decisión
- **WHEN** el usuario elige Pausar para revisar, cierra el aviso o el asistente, cambia de contexto o recarga antes de decidir
- **THEN** se conserva el avance confirmado y no se inicia el siguiente archivo; Escape y cierre mantienen la pausa y las respuestas tardías no continúan otra ejecución
- **AND** el diálogo conserva teclado/foco y vuelve al recurso pertinente; recargar no restaura una autorización para saltar o reintentar archivos

#### Scenario: IND-UI-04 Revisión y retoma de un pendiente individual
- **WHEN** el usuario revisa y vuelve a iniciar un pendiente después de una incidencia con cero invocación acreditada
- **THEN** se consulta el mismo lote y se reautoriza/revalida físicamente ese archivo antes de reservarlo; completados y resultados inciertos no se reenvían
- **AND** vincular un File local PENDIENTE no lo envía por sí solo; la ausencia del temporal de un RECIBIDO informa su límite real de recuperación y no inventa reposición mediante VerificarReenvio

#### Scenario: IND-UI-05 Resultado con pendientes y consultas conservadas
- **WHEN** termina la ejecución consentida con A y C confirmados y B sin procesar por decisión del usuario
- **THEN** muestra Importación con pendientes, los tres archivos siguen en el total original y B conserva referencia/fase/causa sin presentarse como completado
- **AND** progreso por acuses y recuento final son distinguibles; consulta, filtros, páginas, CSV y árbol mantienen identidad/selección y no ejecutan efectos implícitos

#### Scenario: CTL-01 Política de controles compartida
- **WHEN** la importación ejecuta una fase, aplica pausa, espera una transición o presenta incertidumbre
- **THEN** vista y controlador usan una política derivada de esos estados; bloquean edición/efectos paralelos y conservan ayuda/progreso y las acciones de consulta compatibles
- **AND** no se relajan permisos, acciones del servidor, revisiones o reservas por una habilitación visual

#### Scenario: CTL-02 Cierre voluntario sin abortar el acuse
- **WHEN** se pulsa X, Escape o Cerrar con una solicitud en curso
- **THEN** tras el consentimiento del lote confirmado se solicita una sola pausa/cierre, no se inicia la siguiente fase y la vista espera la solicitud actual sin abortarla antes de conservar referencias y cerrar
- **AND** una confirmación terminada durante la transición no inicia automáticamente la importación; cerrar una preparación sin confirmar conserva su consentimiento de descarte existente

#### Scenario: CTL-03 Una navegación después de pausar
- **WHEN** se solicita consultar otro lote, el último lote o volver a otra vista durante la ejecución
- **THEN** se indica Pausar y consultar, se espera la operación actual y se ejecuta una sola intención pendiente
- **AND** mientras se aplica pausa/transición los controles de navegación no agregan más acciones, y consultar nunca reanuda automáticamente

#### Scenario: CTL-04 Pausa de admisión en envío manual
- **WHEN** se solicita cerrar durante el envío manual de varios PDF
- **THEN** termina la solicitud iniciada, se conservan únicamente sus recepciones confirmadas y los archivos restantes no se envían
- **AND** el resumen informa la cantidad efectivamente recibida; pausa no equivale a cancelar la solicitud ni almacenar documentos

#### Scenario: CTL-05 Cierre con resultado sin confirmar
- **WHEN** la solicitud esperada falla o pierde su respuesta durante el cierre
- **THEN** se conservan lote/plan y el aviso público de consulta pendiente, sin afirmar guardado/cancelación del servidor ni repetir el efecto
- **AND** reabrir/consultar conserva los requisitos vigentes de recuperación e incertidumbre

#### Scenario: CTL-06 Desmontaje forzado y accesibilidad
- **WHEN** se destruye el componente por postback/desmontaje o se recorre el formulario con teclado/viewport estrecho
- **THEN** el desmontaje forzado invalida callbacks antiguos sin encadenar efectos, y la interfaz distingue ese caso del cierre voluntario
- **AND** foco, ayuda, espera visible, bloqueo de navegación y acceso al expediente completado se verifican localmente sin atribuirlo a otra prueba real

#### Scenario: CTL-07 Consentimiento antes del cierre de un lote pendiente
- **WHEN** se pulsa X, Escape o Cerrar en un lote confirmado no completado
- **THEN** se muestra un unico dialogo del modulo con referencia y consecuencias antes de solicitar pausa; seguir importando o Escape dentro del aviso lo cancelan sin modificar la ejecucion
- **AND** se explica que la importacion sigue activa mientras decide; aceptar espera la solicitud/lectura iniciada y no admite la siguiente fase antes de cerrar

#### Scenario: CTL-08 Consentimiento accesible y aislado por sesion
- **WHEN** hay renders de progreso, clics repetidos o desmontaje mientras el aviso esta abierto
- **THEN** el foco permanece en el dialogo, Tab y Escape operan en el aviso y no aparecen consentimientos duplicados
- **AND** destruir descarta el consentimiento sin esperar ni ejecutar callbacks de la sesion anterior; no se repiten efectos por cancelar o reabrir

#### Scenario: CTL-09 Cierre completado y retoma explicita
- **WHEN** se cierra un lote completado o se acepta salir de un lote con pendientes o incertidumbre
- **THEN** completado no pregunta; el lote pendiente explica que se conserva lo confirmado y que consultar el lote permite revisar y continuar, pudiendo requerir originales locales
- **AND** el aviso distingue Pausar y cerrar de Cerrar y conservar lote cuando la ejecucion ya esta pausada; cerrar no solicita detencion de negocio ni confirma resultados inciertos

#### Scenario: ORI-01 Vincular originales desde el aviso local
- **WHEN** el lote consultado tiene PDF PENDIENTE sin File vinculado en esta ventana
- **THEN** el aviso distingue falta de seleccion local de temporal recibido ausente y ofrece vincular carpeta o PDF individual con pausa antes del selector
- **AND** seleccionar reutiliza validacion contextual existente, omite recibidos y no envia ni crea ni confirma otro lote; continuar sigue siendo explicito

#### Scenario: ORI-02 Recuperacion visible despues de pausar o cancelar
- **WHEN** el usuario pausa el aviso local o cancela el selector de originales
- **THEN** la vista conserva referencia y accion visible para vincular los originales del pendiente, incluso con su raiz fuera de pagina o filtros
- **AND** consultar no hereda File por coincidencia con otra preparacion y el consentimiento de cerrar sigue funcionando sin callbacks de sesiones antiguas

#### Scenario: ORI-03 Recuperar una carpeta con muchos pendientes
- **WHEN** un lote sintetico contiene10 de1000 PDF recibidos/guardados y se vincula la carpeta original
- **THEN** se vinculan solo990 pendientes sin enviar al seleccionar; Continuar procesa esos pendientes una vez y conserva los10 resultados anteriores
- **AND** faltantes/modificados/ambiguos se informan sin contar exito, y un RECIBIDO sin temporal no ofrece la reseleccion como restauracion de servidor

#### Scenario: MNS-01 Continuacion con mensaje vigente
- **WHEN** el usuario continua y la consulta inicial del lote aun espera respuesta
- **THEN** la franja informa que se esta consultando para continuar y no conserva pausa/vinculacion como estado actual
- **AND** solicitar pausa informa espera del acuse; solo al aplicarse muestra pausa, sin cambiar solicitud, resultado ni numero de operaciones

#### Scenario: MNS-02 Aviso local breve con detalle disponible
- **WHEN** falta un File local pendiente
- **THEN** el aviso muestra archivo, avance, conservacion de resultados y accion de vinculacion sin parrafos repetidos; causa, ruta, fase y referencias permanecen accesibles en un desplegable
- **AND** titulo, botones y foco caben o se alcanzan mediante scroll interno en pantallas estrechas o de poca altura; expandir detalle no envia ni consulta negocio

#### Scenario: MNS-03 Mensajes de fallo e incertidumbre preservados
- **WHEN** una solicitud o su lectura posterior falla durante pausa o continuacion
- **THEN** diagnostico, certeza y accion de consulta conservan prioridad sobre el mensaje informativo transitorio
- **AND** el aviso de temporal RECIBIDO ausente mantiene sus limites sin ofrecer reposicion local; texto/render no reintentan operaciones

### Requirement: RQ-05 — Integridad, incertidumbre y consumidores
Ampliación aprobada2026-11-17: D-04/D-05/D-09 autorizan el control ReanudarLote y la espera de respuesta según Diseno/espera-respuesta-reanudacion-lote-doc120.md. Sin nuevas tablas, escritores o efectos reales autorizados por la implementación local.
DOC-120 SHALL cumplir la decisión D-05: Relacionar EXPEDIENTE/LOTE y PARCIAL_PERMITIDO/NO_APLICA, valores diferidos y fecha estable, revocación, stop e incertidumbre con casos del arnés existente. Comprobar contrato del plugin en sus26 registros/22archivos y carga individual pertinente. Un callback sintético no demuestra la acción de negocio real de cada módulo.

#### Scenario: Recepción y almacenamiento esperan el acuse del servidor
- **WHEN** una recepción multipart o ALMACENAR permanece sin respuesta más de30 segundos
- **THEN** el cliente mantiene una sola solicitud sin abortarla por timeout genérico y espera el acuse real antes de contar éxito y consultar para la siguiente fase
- **AND** bytes100% no equivalen a recepción/guardado confirmado; pausa espera el acuse y cierre invalida respuestas tardías sin afirmar rollback

#### Scenario: Interfaz diferenciada por capacidad de lote
- **WHEN** se consulta un lote detenido, pausado localmente, activo, incierto o terminal
- **THEN** el asistente ofrece reanudar solo con la capacidad del servidor, continuar solo con avance autorizado y consultar/verificar cuando no hay permiso de ejecución
- **AND** no clasifica una detención sin actividad como OPERACION_EN_CURSO ni promete reanudación sobre servidor anterior

#### Scenario: Levantamiento atómico de detención sin efectos documentales
- **WHEN** el propietario autorizado solicita ReanudarLote para DETENIDO/señal1 con su revisión vigente y sin actividad/incertidumbre
- **THEN** el repositorio deriva el estado desde inventario/evidencia, levanta la señal e incrementa revisión con evento en una transacción
- **AND** conserva plan/snapshot/valores/identidades/confirmados y la consolidación ordinaria no levanta una detención

#### Scenario: Reanudación concurrente o revisión antigua
- **WHEN** dos solicitudes comparten revisión o una detención nueva precede a una solicitud vieja
- **THEN** solo una revisión válida puede levantar la señal y registrar el evento; el conflicto o fallo de transacción no deja cambios parciales
- **AND** actividad, incertidumbre, terminales, contexto ajeno o permisos revocados rechazan el control sin liberar intentos

#### Scenario: Continuación tras reanudación y respuesta perdida
- **WHEN** el usuario pulsa Reanudar importación
- **THEN** un acuse válido y lectura posterior coherente son requisitos antes de iniciar la cola existente sobre el mismo lote
- **AND** una respuesta perdida/inválida o consulta fallida impide encadenamiento y solo permite consultar con nueva decisión explícita, sin retry automático

#### Scenario: Medición sin alterar integridad
- **WHEN** se diagnostica el costo de comprobaciones repetidas en1/100/1.000 archivos
- **THEN** las etapas publican únicamente tiempos/categorías/referencias seguros y el laboratorio permanece separado de capacidad real
- **AND** una propuesta de optimización conserva originales/retenciones/permisos/revisiones y no cambia integridad completa a parcial por rendimiento

#### Scenario: Integridad y valores
- **WHEN** se evalúan escenarios completos/parciales, valores diferidos y fecha estable
- **THEN** cada regla tiene caso y evidencia de las barreras/revisiones existentes
- **AND** recepción y efecto permanecen separados y stop no promete rollback

#### Scenario: Consumidores y carga individual
- **WHEN** se consume la prueba del contextual compartido y la carga individual
- **THEN** se conserva recurso invocador, teclado/viewport y alcance probado por consumidor
- **AND** los26 contratos sintéticos no se anuncian como26 operaciones de negocio reales

#### Scenario: Creación sin resultado confirmado permanece visible
- **WHEN** Crear expediente no devuelve confirmación y la consulta conserva la carpeta CREANDO con un intento activo
- **THEN** el asistente muestra esa carpeta y la fase bloqueada, con aviso visible desde cualquier desplazamiento y referencias públicas de diagnóstico
- **AND** no ofrece otra creación ni reconciliación por tiempo, no anuncia progreso al actualizar y solo habilita Verificar intento cuando el contrato vigente lo permite

#### Scenario: Continuación y fin del recurso confirmado
- **WHEN** la creación, los valores o el almacenamiento quedan confirmados en consultas posteriores
- **THEN** el panel señala la fase coordinada bajo consentimiento vigente, la siguiente acción explícita al pausar o el resultado confirmado del archivo/lote
- **AND** una raíz completada conserva su expediente confirmado en el indicador; recepción sola no cuenta como documento guardado y filtros no relajan la autoridad del padre

#### Scenario: Fecha resuelta con longitud distinta del origen
- **WHEN** una regla válida convierte una fecha de origen corta a la fecha canónica yyyy-MM-dd
- **THEN** creación y validación de valores aceptan los diez caracteres canónicos sobre una copia de la regla
- **AND** la entrada original conserva su límite, las fechas inválidas/no canónicas siguen rechazadas y la regla, snapshot y reserva no cambian

#### Scenario: Reclamación de la misma reserva sin doble escritor
- **WHEN** se solicita retomar una creación reservada sin checkpoint de inicio
- **THEN** se validan autoridad, comando, catálogos, staging, revisiones, diario y ausencia de otras reclamaciones antes de rotar atómicamente el token del mismo intento
- **AND** solo un token puede pasar RESERVADO a EN_CURSO; se conservan recibo, valores, snapshot e identidad del intento y se rechaza al ejecutor anterior

#### Scenario: Detención o revisión concurrente
- **WHEN** cambia la detención o cualquiera de las revisiones entre la consulta y la reclamación
- **THEN** la reanudación se rechaza sin levantar la nueva detención ni iniciar efectos
- **AND** permisos revocados, destino cambiado o staging no comprobado también impiden mutar la reserva

#### Scenario: Intento ya iniciado o respuesta perdida
- **WHEN** un intento ya está EN_CURSO, INCIERTO o CONFIRMADO, o no se confirma la respuesta de reanudación
- **THEN** no se adquiere otra reclamación ni se repite el escritor
- **AND** se consulta el estado y la verificación de resultados inciertos mantiene sus guardas, sin deducir permiso de tiempo o antigüedad

#### Scenario: Inicio único y cola completa independiente de filtros
- **WHEN** el usuario revisa y pulsa Confirmar e importar
- **THEN** se confirma el plan una vez y solo tras lote y lectura válidos se coordinan las operaciones existentes con una mutación en vuelo y acuse/lectura posterior por fase
- **AND** el alcance elegible incluye todas las páginas autorizadas sin cambiar filtros visibles y respeta integridad EXPEDIENTE/LOTE/PARCIAL; File se vincula solo al contexto original o reselección validada
- **AND** la revisión debe permanecer coherente durante la enumeración; topes10000 recursos/consultas/efectos y400 páginas por tipo pausan antes de excederse, sin polling o retry automático

#### Scenario: Pausa, incertidumbre y recuperación explícita
- **WHEN** se pausa, se cierra, falta un original, cambia una revisión o falla una respuesta/lectura posterior
- **THEN** se pausa antes del siguiente efecto y se conservan resultados e identidades; ante un faltante con cero invocación acreditada, solo Continuar con los demás permite elegir otro archivo, mientras Pausar/cerrar o no decidir no avanza
- **AND** conflicto/autoridad o invocación posible sin resultado confirmado conservan sus guardas y consulta sin replay; las respuestas tardías de una generación cerrada no continúan la cola
- **AND** una respuesta perdida exige consulta sin replay ni reanudación automática; una lectura fallida tras acuse conserva su confirmación; Continuar, Retomar creación y Verificar intento mantienen consentimiento explícito
- **AND** consultar, exportar, recargar o recuperar una confirmación pendiente no inicia operaciones; el cierre y conteos proceden de resultados confirmados, con acceso por ID a los expedientes

#### Scenario: Código de negocio repetido sin exclusividad implícita
- **WHEN** una o varias raíces resuelven a un código ya existente o repetido dentro del inventario
- **THEN** preparación y confirmación no rechazan por esa coincidencia; una raíz nueva crea un expediente con ID propio, sin reutilizar ni alterar el anterior
- **AND** el delegado existente conserva un punto de extensión futuro, hoy limitado a validaciones técnicas; permisos, integridad, representabilidad e idempotencia del mismo intento permanecen obligatorios
- **AND** configuraciones anteriores BLOQUEAR/REUTILIZAR_VALIDADO siguen legibles sin mutar snapshots, sin bloqueo o reutilización implícitos; nuevas ediciones usan PERMITIR y un plan bloqueado requiere nueva revisión

Los escenarios CAP siguientes amplían RQ-05 con trazabilidad adicional RQ-04/RQ-07/RQ-09 según contrato08 aprobado para implementación local2026-11-16; no representan una corrida real.

#### Scenario: CAP-01 Preparación automática por tamaño y cantidad
- **WHEN** se prepara una selección de1.000 PDF
- **THEN** los metadatos se segmentan automáticamente dentro de los límites efectivos, sin enviar contenido PDF ni añadir confirmaciones por segmento
- **AND** la revisión queda lista únicamente tras completar el inventario coherente

#### Scenario: CAP-02 Identidad del inventario independiente de segmentos
- **WHEN** una raíz ocupa varias partes o existen raíces homónimas y códigos repetidos
- **THEN** se conserva un plan/lote y un expediente por raíz elegible, con identidad por ID_EXPEDIENTE
- **AND** los segmentos no generan lotes o expedientes adicionales

#### Scenario: CAP-03 Partes idempotentes y acuse perdido
- **WHEN** se repite una parte o no se recibe su acuse
- **THEN** misma identidad/contenido conserva la parte y contenido distinto se rechaza
- **AND** una consulta acotada determina lo persistido sin crear otra preparación o repetir efectos de negocio

#### Scenario: CAP-04 Cierre incompleto o incompatible
- **WHEN** faltan partes, hay duplicados globales o cambia el contexto/publicación/sello relevante
- **THEN** no se sella ni confirma una revisión parcial o incompatible
- **AND** se informa la causa y la necesidad de continuar o preparar una nueva revisión según corresponda

#### Scenario: CAP-05 Equivalencia global de reglas e integridad
- **WHEN** cambia el orden de llegada o la frontera entre partes del mismo inventario
- **THEN** valores, incidencias, elegibilidad y hash canónicos son equivalentes
- **AND** EXPEDIENTE/LOTE/PARCIAL se aplican al inventario global y no a segmentos aislados

#### Scenario: CAP-06 Revisión y persistencia acotadas
- **WHEN** valores o incidencias de1.000 archivos superan el tamaño de un documento
- **THEN** se conservan en fragmentos y se consultan mediante resumen/páginas sin serializar el agregado por PerfilImportacionJson
- **AND** los límites y las huellas de perfiles/planes esquema1 permanecen intactos

#### Scenario: CAP-07 Confirmación única de plan segmentado
- **WHEN** se confirma o recupera una confirmación de plan esquema2
- **THEN** hash/revisión/autoridad vigentes y transacción generan o devuelven un lote con su inventario íntegro
- **AND** concurrencia o pérdida de acuse no publican inventario parcial ni otro lote

#### Scenario: CAP-08 Lectura focal antes del siguiente efecto
- **WHEN** termina una operación de la coordinación grande
- **THEN** acuse y lectura autoritativa del recurso/padre/barreras permiten continuar con hasta dos solicitudes de lectura por efecto ordinario
- **AND** detención, revocación, conflicto del objetivo o ejecución posiblemente activa impiden el siguiente efecto; una incidencia conocida de otro archivo solo se omite por decisión explícita y con guardas de ejecución vigentes, sin reenumerar todo el lote ni elevar topes

#### Scenario: CAP-09 Recuperación de originales por carpeta
- **WHEN** se reselecciona una carpeta para una raíz confirmada del lote
- **THEN** se vinculan los pendientes por identidad contextual, ruta y metadatos, informando faltantes/modificados/ambiguos sin enviar
- **AND** solo un nuevo consentimiento Continuar importación inicia la cola

#### Scenario: CAP-10 Pausa y recarga sin replay
- **WHEN** se pausa, cierra o recarga durante preparación/recepción/creación/valores/almacenamiento
- **THEN** se conservan los resultados confirmados y se invalida el consentimiento automático
- **AND** consulta y recuperación no repiten un efecto incierto ni agregan inventario al lote confirmado

#### Scenario: CAP-11 Privacidad de preparaciones y recursos focales
- **WHEN** otra cuenta o ámbito conoce IDs de preparación/parte/plan/recurso
- **THEN** se deniega lectura/completar/sellado/confirmación sin revelar existencia ajena
- **AND** autoridad y permisos se reconstruyen por solicitud

#### Scenario: CAP-12 Compatibilidad y capacidad verificadas por alcance
- **WHEN** se verifica la versión con preparación segmentada
- **THEN** lectores1/2, reversión compatible y UI390/768/1280 conservan sus controles y presupuesto local del contrato08
- **AND** simulación local no acredita capacidad IIS/SQL ni autoriza DDL, carga, SII o limpieza reales

#### Scenario: IND-01 Almacenamiento independiente por identidad de archivo
- **WHEN** se almacena A con B pendiente, rechazado o físicamente no disponible en el mismo lote, de la misma raíz o de otra
- **THEN** la operación de A comprueba pertenencia/contexto, recepción, valores, revisión y original propio sin cargar ni abrir el original de B
- **AND** COMPLETO_OBLIGATORIO EXPEDIENTE/LOTE y PARCIAL_PERMITIDO conservan inventario/snapshot y su evaluación final; A no requiere cambiar el perfil a parcial

#### Scenario: IND-02 Creación por raíz con un original utilizable
- **WHEN** una raíz elegible tiene datos válidos y al menos un PDF recibido y físicamente verificado, mientras otro archivo aún no está disponible
- **THEN** puede crear una sola vez su expediente confirmado para procesar el archivo disponible, sin exigir la recepción o comprobación de otros PDF
- **AND** no crea una raíz sin ningún original utilizable, no reutiliza expediente por nombre/código ni ignora un intento de creación incierto o una revisión/token diferentes

#### Scenario: IND-03 Reserva atómica y exclusión de ejecución
- **WHEN** dos sesiones intentan procesar A o existe un escritor todavía activo en el lote
- **THEN** las revisiones, tokens, estado y locks del repositorio permiten como máximo un escritor de A y mantienen la ejecución secuencial
- **AND** leer un ID o consentir continuar no concede una reclamación, no libera intentos ajenos y no ignora un marcador huérfano o una autorización sin retorno por tiempo transcurrido

#### Scenario: IND-04 Incidencia sin invocación y cierre seguro
- **WHEN** se comprueba falta, alteración o imposibilidad de lectura del original de A antes de invocarlo, incluso después de una reserva propia
- **THEN** se registra una incidencia focal sin IDs documentales y se conserva archivo/recepción/valores/historial; solo prueba durable de ausencia de autorización/invocación permite cerrar la reserva propia con CAS
- **AND** una incidencia negativa tipada permite una nueva revisión explícita del objetivo sin habilitar cualquier RECHAZADO/INCIERTO; una falla de registro mantiene incertidumbre y nunca afirma cierre no persistido

#### Scenario: IND-05 Incertidumbre y checkpoints focales V2
- **WHEN** falta el acuse del escritor/checkpoint de A o la evidencia no permite determinar si fue invocado
- **THEN** conserva INCIERTO, identidad física, época, IDs observados y diario V2 sin replay ni conversión a archivo ausente sin efectos
- **AND** un resultado incierto de B solo deja avanzar otro archivo si la evidencia durable y guardas de exclusión prueban que su escritor terminó; una autorización posible sin retorno mantiene pausa/consulta aunque el usuario quiera continuar

#### Scenario: IND-06 Completitud por resultados sin inspección cruzada
- **WHEN** se confirma un documento, se actualiza progreso o se evalúa el cierre de raíz/lote
- **THEN** usa resultados y resúmenes coherentes del inventario original sin verificar físicamente otros PDF ni reconstruir todos sus JSON por cada fase individual
- **AND** solamente declara COMPLETADO si todos los documentos previstos están comprobados; pendientes, rechazos, incertidumbre y detención conservan significado e identidad del expediente sin mutar snapshots históricos

### Requirement: RQ-06 — INT-02 degradación acotada
DOC-120 SHALL cumplir la decisión D-06: Conservar el caso real histórico de ausencia temporal de ra_integracion_evento con restauración y carga individual, sin extrapolar a cada incompatibilidad. Contrastar los verificadores/esquema y el guard de indisponibilidad actuales. Exigir variante nueva solo si un requisito concreto y una diferencia reproducida la hacen necesaria; registrar NO_NECESARIA con justificación en los demás casos.

#### Scenario: Degradación histórica delimitada
- **WHEN** se reutiliza la ausencia de ra_integracion_evento
- **THEN** se conserva alcance de indisponibilidad/restauración/carga individual
- **AND** no se extrapola a toda incompatibilidad ni se reinstala SQL

#### Scenario: Necesidad de variante
- **WHEN** se encuentra una incompatibilidad exigida sin evidencia suficiente
- **THEN** se documenta reproducción, requisito, restauración y autorización para la variante concreta
- **AND** si no es necesaria consta NO_NECESARIA justificada sin modificar el esquema

### Requirement: RQ-07 — INT-03 capacidad con presupuesto definido
DOC-120 SHALL cumplir la decisión D-07: Antes de medir acordar objetivo, cantidad/distribución/tamaño de archivos y raíces, navegadores/versiones, concurrencia, duración, ambiente, recursos, disco temporal/libre y umbrales de tiempo/memoria. Incluir topes HTTP/SELECT/sesiones/bytes/efectos y criterio de parada. Sin esos acuerdos, INT-03 queda NO_EJECUTADO por falta de definición/autorización.

#### Scenario: Medición por etapas sin modificar el efecto
- **WHEN** una solicitud ALMACENAR autenticada y verificada solicita la captura optativa por cabecera
- **THEN** devuelve duraciones/conteos de categorías cerradas en Server-Timing y conserva el JSON, las comprobaciones, el retorno y las excepciones de negocio
- **AND** los acumuladores no cruzan solicitudes ni publican PDF, SQL, valores, conexiones o tokens; los subintervalos no se suman como etapas independientes

#### Scenario: Prueba real acotada a tres documentos existentes
- **WHEN** se ejecuta la ventana solicitada para tres archivos del lote50 con versión instrumentada aprobada y credenciales efímeras
- **THEN** solicita como máximo tres almacenamientos secuenciales, espera cada respuesta sin retry y comprueba los resultados y la conservación del inventario restante
- **AND** la falta de versión, autoridad, original listo, acuse o medición detiene sin inventar tiempos ni ejecutar otros efectos; la preparación local no satisface esta prueba real

#### Scenario: Contrato previo
- **WHEN** se propone medir INT-03
- **THEN** existen objetivo, corpus, navegadores/versiones, concurrencia, disco, umbrales y topes de ejecución revisados
- **AND** una medición solo afirma el presupuesto y ambiente observados

#### Scenario: Sin definición o autorización
- **WHEN** faltan objetivo, umbrales o autorización específica de capacidad
- **THEN** INT-03 consta NO_EJECUTADO con motivo, responsable e impacto operativo
- **AND** no se infiere capacidad máxima de páginas, lotes pequeños o pruebas locales

#### Scenario: IND-PERF-01 Costo focal independiente del tamaño del inventario
- **WHEN** se compara la misma operación individual con inventarios de1,100 y1000 archivos
- **THEN** las cargas/consultas operativas por ID, reconstrucciones de plan y verificaciones físicas de esa fase permanecen acotadas al objetivo y las visitas a otros originales son cero
- **AND** se miden enumeración, agregados y cierre por separado, incluyendo information_schema, autoridad, catálogos, V2 y consolidación; los SELECT del ejecutor externo no se presentan como el SQL interno total

#### Scenario: IND-PERF-02 Comparación real sin heredar la ventana anterior
- **WHEN** se pretende comprobar tiempos y decisión de continuar/pausar en el servidor real después del cambio
- **THEN** exige autorización nueva de instalación/ambiente/cuentas/recursos/efectos/topes y compara candidatos pendientes equivalentes con la misma política
- **AND** no repite documentos completados, no borra originales reales por inferencia ni reutiliza la ventana cerrada de tres archivos; sin ejecución conserva pendiente y evidencia local separada

### Requirement: RQ-08 — Autorización separada y herramientas existentes
DOC-120 SHALL cumplir la decisión D-08: La continuación autoriza refinamiento/inspección y preparación local. Antes de ejecutar cada ventana real exigir ambiente, cuentas, recursos, operaciones, presupuestos y recuperación revisados. Reutilizar autenticación, runners y queryRows existentes; controles RA/DA exclusivamente SELECT y secretos efímeros. Autorizaciones DOC110–119 no se heredan.

#### Scenario: Ventana real concreta
- **WHEN** se pretende autenticar, consultar BD, cargar o inyectar fallos
- **THEN** se exige autorización de ambiente/cuentas/recursos/operaciones/presupuestos/recuperación
- **AND** los controles RA/DA usan solo SELECT y las credenciales permanecen efímeras

#### Scenario: Resultado detenido
- **WHEN** una ventana real falla o consume su presupuesto
- **THEN** se conserva STOPPED y su consumo sin retry automático
- **AND** no se reutiliza una autorización anterior para ampliar efectos

#### Scenario: Diagnóstico de prerrequisitos sin efectos
- **WHEN** se investiga una creación reservada sin checkpoint de inicio
- **THEN** las validaciones de comando y catálogo se reutilizan mediante lecturas acotadas y autorización propia, sin componer escritores
- **AND** una proyección local de la situación previa se identifica como contrafactual, no acredita sesión/transacción ni autoriza replay o liberación de la reserva

### Requirement: RQ-09 — Fuentes y decisiones protegidas

DOC-120 SHALL cumplir la decisión D-09: Conservar SII/Integracionccv, AlmacenamientoDocumentoProduccionDocumental, Web.config, semántica de recepción/efectos y PDF/rechazo TIFF. La excepción aprobada del contrato08 permite exclusivamente tres tablas aditivas de preparación, siete operaciones de metadatos/consulta en el ASMX existente y planes esquema2 con lectores compatibles1/2. Mantener decisión expresa SEG-03 de operar sin limpieza y datos retenidos. No añadir otros endpoints, jobs, cuotas, formatos o aislamiento XML adicional; aplicar DDL o desplegar requiere autorización propia.

Corrección adicional solicitada2026-11-17 por video170531: ConsultarUltimoLote es una lectura en el ASMX existente con solicitud vacía, sesión/CSRF, scope de propietario/empresa y reautorización del destino. No requiere cambios de esquema ni ejecuta fases de negocio.

Excepción de implementación aprobada2026-11-17: ReanudarLote es el único control adicional de esta ampliación en el ASMX existente; usa lote/revisión y tablas actuales, sin recibir PDF ni ejecutar efectos documentales. La medición se limita a fronteras del importador y no cambia el escritor compartido. Despliegue y prueba real conservan su autorización separada.

Excepción adicional autorizada2026-11-17: el ajuste del índice17 de Egresos puede ampliar CONTABIL.BENEFICIARIO/metadato legacy de60 a250 y publicar una versión nueva que cambie solo esa longitud. Debe verificar antes contexto/origen/capacidad/índice y después definición/contenido agregado/publicación anterior; no permite recortar datos, modificar el origen u otros campos ni iniciar importaciones.

#### Scenario: Capacidad del índice Egresos alineada sin pérdida de datos
- **WHEN** se ajusta el índice17 tras medir el origen y recibir autorización específica
- **THEN** columna y metadato coherentes preceden a la publicación de una versión nueva del perfil, conservando charset, null/default, contenido y publicación anterior
- **AND** los efectos parciales se conservan sin replay o reducción destructiva; una consola esperando credenciales no acredita aplicación ni publicación

#### Scenario: Fuentes y formatos
- **WHEN** se consolida o corrige una brecha de DOC-120
- **THEN** se preservan SII/almacenamiento compartido/configuración y PDF con rechazo TIFF, y el cambio aditivo de preparación se limita al contrato08 aprobado
- **AND** no se habilitan jobs/cuotas/limpieza/aislamiento XML adicional

#### Scenario: Decisión sin limpieza
- **WHEN** se consume SEG-03 de DOC-119
- **THEN** se conserva operación sin limpieza y datos retenidos
- **AND** la medición histórica de disco no se anuncia como actual ni como borrado real

#### Scenario: IND-COMP-01 Compatibilidad y fuentes protegidas
- **WHEN** se entrega procesamiento independiente con lectura focal versión2 e incidencias individuales
- **THEN** mantiene lectura focal1, planes1/2, evidencias/recuperación V1/V2 y escritores compartidos, con sesión/CSRF/origen/empresa/usuario/destino comprobados
- **AND** usa tablas/estados existentes salvo ampliación previamente documentada, no aplica DDL o despliegue por planificación y no activa limpieza/jobs, paralelismo de escritores, reposición automática de recibidos o rutas de archivos enviadas por el cliente

### Requirement: RQ-10 — Defectos reproducidos y criterio de cierre
DOC-120 SHALL cumplir la decisión D-10: Modificar producto solo ante una brecha reproducible vinculada a requisito, causa y prueba de regresión. Registrar cada no ejecutado con motivo, responsable e impacto operativo. Autoridad/destino/integridad/recuperación con fallo crítico bloquean salida. Límites necesitan aceptación expresa; archivo/CI/checklists no sustituyen aceptación integral.

#### Scenario: Brecha reproducible
- **WHEN** se detecta un defecto de autoridad, destino, integridad o recuperación
- **THEN** se registra causa/caso/corrección/regresión y se bloquea salida mientras sea crítico
- **AND** no se desarrolla producto por ausencia de un checklist genérico

#### Scenario: Aceptación y cierre
- **WHEN** se propone declarar lista la versión
- **THEN** la matriz identifica cada no ejecutado y su motivo, responsable e impacto; los límites tienen aceptación expresa
- **AND** archivo, CI y número de tareas no sustituyen aceptación operativa ni merge verificado

## Trazabilidad de capacidad — contrato08 aprobado para implementación local

Los41 escenarios anteriores y los12 escenarios CAP incorporados a RQ-05 forman53 escenarios normativos. La respuesta «continua» aprobó el [contrato08](../../../../../Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/08-preparacion-segmentada-capacidad.md) para implementación y pruebas locales. Esta matriz expresa su trazabilidad; no concede autorización de DDL, despliegue o ejecución real ni equivale a resultado PASS.

| Origen | Casos del contrato08 | Condición observable |
| --- | --- | --- |
| D-04/RQ-04 | CAP-01, CAP-06, CAP-09, CAP-10 | Preparación automática por partes, revisión acotada, progreso global y carpeta completa para recuperar pendientes con consentimiento |
| D-05/RQ-05 | CAP-02–CAP-10 | Un inventario lógico, partes idempotentes, cierre global, confirmación única y siguiente efecto solo con lectura focal coherente |
| D-07/RQ-07 | CAP-01, CAP-06, CAP-08, CAP-12 | Tamaños medidos por operación; <=6.045 lecturas para el recorrido ordinario simulado de1.000 archivos/una raíz, sin elevar el tope vigente |
| D-08/RQ-08 | CAP-11 | Autoridad por solicitud y privacidad de preparaciones/recursos; pruebas reales con ventana propia |
| D-09/RQ-09 | CAP-12 | Excepción aditiva de preparación/consulta aprobada para código local; configuración, escritores compartidos, formato y ausencia de limpieza preservados |
| D-01/RQ-01, D-10/RQ-10 | CAP-01–CAP-12 | Fuentes/huellas y resultados locales separados de capacidad/aceptación real |
