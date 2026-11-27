# Contratos v1 de radicación interna

Estado: borrador técnico de P01, 2026-11-26. No hay aplicación, endpoints, tablas ni migración nuevos implementados por este documento. G01/G03 condicionan la apertura independiente y la incorporación; las demás puertas se refinan según [decisiones](decisiones.md). No se declara P01 aceptado mientras falte la alternativa documental necesaria.

## Alcance e invariantes

Se conservan Producción y la radicación actual; la capacidad nueva incorpora apertura directa, control de archivos independiente, remitente tipado y entrega de seguimiento. El almacenamiento y metadatos de la baseline son intocables. El módulo de respuestas conserva la gestión posterior. Antes de modificar el núcleo se ejecuta la [matriz de caracterización](matriz-caracterizacion.md); diseñar esa matriz no equivale a haberla ejecutado.

El cliente aporta intención y referencias. Actor, empresa, conexión, permisos, rutas físicas, tablas, columnas, reglas de plantilla y consecutivos proceden del servidor. El responsable Workflow no sustituye al remitente ni al actor. Una referencia visible no concede acceso; cada petición reautoriza operación y recursos.

## Tipos y límites de representación

| Valor | Transporte y regla v1 |
|---|---|
| `operacionId`, `archivoId`, `aperturaId`, `intentoId`, `revisionId`, `claveConfirmacion` | Cadenas opacas de 32 caracteres hexadecimales minúsculos, generadas con GUID sin separadores donde corresponda. Son correlación, no credenciales ni autorización. `aperturaId` y `claveConfirmacion` se conservan en el navegador durante su intención, sin reutilizarlos para otra operación. |
| `revisionOperacion`, `revisionArchivo`, `revisionManifiesto` | Cadenas decimales canónicas positivas. Propuesta interna UInt32; rechazar desbordamiento antes de incrementar. La revisión por archivo no se confunde con la estructural de la operación. |
| IDs de expediente/documento/plantilla/tipo/usuario | Cadenas decimales positivas sin signo, notación científica ni ceros de relleno. Comprobar el rango de la firma destino antes de convertir; el preparador con `Integer` impone un límite distinto de un campo `Long`. No corregirlo cambiando la función congelada. |
| Referencias de tarea/radicado | Tarea como cadena decimal según Long; radicado como referencia compuesta con plantilla e ID real del registro general, además del consecutivo visible. No autorizar por consecutivo solamente. |
| Fechas de formulario | ISO `YYYY-MM-DD` cuando el campo configurado sea DATE. El adaptador respeta tipo, cultura/formato esperado por los lectores vigentes y semántica original; no reemplaza todas las fechas por una sola. |
| Instantes operativos | UTC en transporte, con formato ISO inequívoco. Persistencia compatible con el esquema MySQL comprobado al implementar; no depender de precisión fraccional o tipos JSON nuevos. |
| Bytes y folios | Bytes como cadena decimal no negativa, comprobados contra stream/política real. Folios según campo/configuración y conteo legacy; no se deducen de bytes, cantidad de anexos o `lastModified`. |
| `configRevision`, hashes de recepción/revisión | SHA-256 canónico calculado por servidor sobre el contenido o snapshot definido. La huella del temporal no reemplaza las huellas del índice legacy ni acredita todos los derivados. |
| Campos dinámicos | Lista por `campoRef` emitido por el servidor, tipo y valor compatibles con la plantilla vigente. No aceptar nombre libre de tabla/columna ni utilizar el orden de propiedades JavaScript como índice de gabinete. |

## Entidades de intercambio

| Entidad | Datos mínimos y propósito |
|---|---|
| `OperacionVista` | Operación, origen inmutable, revisiones, plantilla/configuración, campos visibles, remitente/destino, referencias documentales, capacidades y resultado actual. Excluir rutas físicas, cadenas de conexión, sesión completa y excepciones crudas. |
| `RemitenteSeleccion` | `tipo` = INTERNO o TERCERO, `referencia` y `catalogoRef` emitida/validada por el servidor cuando corresponda. La referencia numérica sin tipo/catálogo no es identidad suficiente. Alta/edición no se expone mientras G04 no la incluya. |
| `DestinoSeleccion` | IDs autorizados de trámite/flujo/actividad/grupo/usuario; nulabilidad según ruta real. El servidor invalida dependencias incompatibles y no elige silenciosamente la primera opción. |
| `ArchivoSeleccion` | Archivo propio, rol PRINCIPAL o ANEXO, nombre visible, bytes previstos y referencia de reserva. El cliente no aporta ruta temporal/final ni puede adjuntar un archivo de otra operación. |
| `RecepcionVista` | Archivo/revisión/intento, estado, bytes recibidos, huella del servidor y errores controlados. RECIBIDO significa recepción íntegra, no incorporación definitiva. |
| `RevisionPreparada` | `revisionId`, revisión estructural, versión de manifiesto, snapshot/hash de configuración y resumen exacto de datos/destino/remitente/recepciones. No concede permiso permanente. |
| `ResultadoRadicacion` | Estado de registro y de fases, radicado confirmado o ausencia/indeterminación explícita, referencias por rol y pendientes. Incluir `incertidumbreEn` cuando corresponda. La obligación pendiente no se presenta como respuesta elaborada. |

## Transporte y autoridad

Patrón de referencia: `ImportacionCarpetasUpload.ashx.vb`, `ImportacionCarpetasRequestGuard.vb` y `ImportacionCarpetasContextAdapter.vb`. Se reutilizan criterios de frontera y autorización; no se usa un lote de importación para radicar ni se modifica DOC-127.

La página autorizada emite un token efímero propio. Propuesta de cabecera: `X-Radicacion-Interna-CSRF`, vinculada a sesión/actor/empresa/contexto de backend. No aceptar un token de otro módulo ni conservarlo en logs, evidencia o localStorage. Verificar origen cuando venga informado, método, contenido, tamaño y forma de cada solicitud. No crear otro login ni recibir credenciales en DTOs.

En los postbacks WebForms que abren o confirman una operación, validar un campo de protección emitido por el servidor y vinculado al mismo contexto, además de la protección de formulario aplicable. No suponer que el postback lleva la cabecera AJAX ni que un campo oculto con actor/IDs constituye autorización. No cambiar `EnableEventValidation` sin resolver el ciclo dinámico caracterizado en P02.

Las nuevas operaciones ASMX usan JSON con argumento tipado `solicitud`; el resultado ASP.NET se recibe bajo `.d`. El handler multipart retorna el mismo sobre lógico sin `.d`; el cliente normaliza esa diferencia en su adaptador de transporte. No convertir un JSON serializado como cadena en un segundo protocolo ni aplicar estos retornos al servicio compartido.

Sobre lógico: `ok`, `codigo`, `mensaje`, `correlacionId`, `datos`, `erroresCampo`, `advertencias`. Un resultado conocido puede indicar registro con pendientes aunque HTTP sea exitoso; el estado de dominio es obligatorio. Sesión expirada: 401 sin redirección HTML; permiso general insuficiente: 403; recurso no disponible: respuesta uniforme que no revela pertenencia ajena. Conflicto de versión/configuración/intención: 409; datos inválidos: 422; tamaño/tipo no admitido: 413/415. No transformar un timeout en un rechazo confirmado ni reintentar escrituras automáticamente.

## Operaciones públicas mínimas propuestas

Los nombres nuevos son contratos de diseño, no WebMethods existentes. Los permisos nuevos se concretan en G03. Todas las operaciones, incluidas lecturas, reautorizan recursos; las consultas no crean radicado/documentos ni reparan fases.

| Transporte/operación | Entrada | Resultado y efectos permitidos |
|---|---|---|
| Apertura desde Producción, llamada interna del code-behind | Selección real de principal/anexos y `aperturaId`, más contexto autenticado | N02 crea/devuelve la misma operación para la misma intención/selección autorizada. Excluir principal duplicado; no adoptar selección posterior de otra pestaña. No requiere WebMethod público adicional. |
| ASMX `AbrirCargaDirecta` | `aperturaId`; plantilla o expediente solo como intención opcional que el servidor valida | Operación de origen CARGA_DIRECTA. No exige visita previa al árbol. Resolver capacidades/configuración; recepción deshabilitada si falta permiso/política/recurso necesario. No escoger la alternativa de expediente por silencio. |
| ASMX `ConsultarOperacion` | Operación | Vista autorizada actual, sin mutación de dominio. |
| ASMX `ActualizarOperacion` | Operación, revisión esperada y cambios permitidos de campos/remitente/destino/contexto documental | Compare-and-swap; devuelve revisión nueva e invalida la revisión preparada. No admite cambiar actor/empresa/origen ni aportar SQL/rutas. |
| ASMX `ConsultarOpciones` / `BuscarRemitentes` | Operación, recurso/tipo de consulta y búsqueda/página acotada | Catálogo autorizado por plantilla/ámbito. No crear relaciones, terceros, expediente ni radicado. Catálogos de usuarios generales conservan sus contratos. |
| ASMX `PrepararArchivos` | Operación/revisión esperada y archivos/roles seleccionados | Reserva referencias propias y versiones por archivo; actualiza manifiesto estructural. No recibe archivos definitivos ni reserva consecutivo. Puede reservar varios anexos en una intención. |
| Handler `RadicacionInternaUpload.ashx` | Un binario y solicitud multipart con operación, archivo, revisión de archivo e intento | N06 verifica permiso, reserva activa y recepción; resultado temporal. Nombre/MIME/bytes declarados no son autoridad de contenido ni destino. |
| ASMX `ConsultarRecepcion` / `RetirarArchivo` | Referencias propias; retiro con versión esperada | Consulta sin efectos o retiro explícito del temporal no incorporado. Retiro invalida manifiesto/preparación; no borra definitivos. |
| ASMX `PrepararRevision` | Operación y revisión esperada | Revalida datos/configuración/recursos, todos los archivos y roles. Persiste snapshot y devuelve revisión preparada exacta; no registra el radicado. Es una preparación explícita, no una lectura que escriba inadvertidamente. |
| ASMX `ConfirmarOperacion` | Operación, revisión preparada y clave de confirmación | Reautoriza y reclama ejecución; devuelve rechazo previo, resultado confirmado, pendiente o incertidumbre. No recibe nuevos campos/binarios ni vuelve a guardar por doble clic. |
| ASMX `ConsultarResultado` / `ConsultarSeguimiento` | Operación o referencia autorizada según consumidor | Estado y referencias comprobadas; sin llamar al motor mutante. Resultado documental se entrega mediante acceso autorizado, nunca por ruta física suministrada por el navegador. |
| ASMX `RecuperarPendientes` | Operación y fase/revisión verificables | Mutación explícita según G08. Ejecuta solo fases cuyo efecto pueda determinarse; incertidumbre documental sin evidencia inequívoca no se reintenta. |
| ASMX `CancelarOperacion` | Operación/revisión esperada | Cancelación antes del registro y fuera de ejecución/efecto incierto. Conservar toda incorporación ya reconocida; cierre del modal no cancela ni revierte el dominio. |

Un control nuevo no depende de `LoadFilePERSON`/`IniLoadPerson`. Propuesta `RadicacionInternaUpload`: inicializar una vez por contenedor, seleccionar/sustituir principal, agregar/retirar anexos, recibir/consultar, notificar estado y destruir manejadores. Su destrucción aborta recursos de UI cuando sea necesario, pero no presume rollback del servidor. Cada evento/respuesta se contrasta con operación/archivo/versiones; una respuesta tardía no restaura un archivo retirado.

## Revisiones, concurrencia e intención

- La revisión de operación protege datos, configuración seleccionada y composición del manifiesto; la de archivo protege su reserva/recepción. El estado de progreso de un archivo no invalida por sí solo otros archivos válidos.
- Reservar/sustituir/retirar cambia el manifiesto y anula la preparación anterior. La recepción exige que el archivo/versiones sigan vigentes en el manifiesto actual; no se autoriza por el nombre original ni por una revisión global antigua.
- `PrepararRevision` captura las recepciones completas vigentes y sus versiones. Confirmar exige ese snapshot exacto y revalidación de permisos/configuración. Los cambios posteriores requieren nueva preparación.
- Misma apertura e idéntica selección/payload devuelve la operación conocida; misma clave con intención distinta produce conflicto. Mismo intento de recepción se consulta antes de reenviar cuando se pierde su respuesta.
- Misma confirmación devuelve el mismo resultado. Una clave nueva no habilita otro registro de una operación confirmada/en curso/incierta. La caducidad de una reclamación tampoco prueba ausencia de un efecto.

## Estados y límites de efectos

Estado visible candidato: BORRADOR, RECIBIENDO, LISTO_PARA_CONFIRMAR, CONFIRMANDO, REGISTRADO, REGISTRADO_CON_PENDIENTES, RECHAZADO_ANTES_DE_REGISTRAR, RESULTADO_INCIERTO y CANCELADO_ANTES_DE_REGISTRAR. Se registra por separado estado/evidencia de DOCUMENTOS_PREVIOS, NUCLEO, DOCUMENTOS_RADICADO, WORKFLOW y NOTIFICACION. No reducir todo el resultado a YES/NO.

Una operación rechazada antes del radicado puede conservar documentos incorporados durante su preparación. Ese caso no se convierte en borrador vacío ni autoriza borrarlos. Un estado incierto indica la fase afectada y no anuncia un radicado confirmado sin evidencia. La consulta de resultado distingue esos ejes.

Secuencia condicional A, todavía pendiente de G01: recepción propia → revisión → reclamación → incorporación previa con firmas vigentes y expediente autorizado → referencias reales de Producción → núcleo y vínculo de operación → copias/rótulo/relaciones posteriores según gabinete → Workflow/notificación → entrega del seguimiento. La variante B no se considera satisfecha pasando cero.

`RegistrarNucleo` y la asociación del resultado a la operación deben compartir la transacción y conexión del núcleo RA si la implementación demuestra esa compatibilidad. No ejecutar un INSERT de operación confirmado en otra conexión y llamarlo atómico. Las funciones de almacenamiento tienen commits propios y permanecen intactas: quedan fuera de esa garantía global.

Si falla la escritura física/XML después de un commit, conservar el estado incierto y consultar evidencias con la API existente. La entrada nueva no añade columnas al motor, no predice IDs ni repite el método monolítico para recuperar. Metadatos de formulario, inventario, gabinete e índice se conservan como capas distintas, con referencias de fuente/copia/rótulo separadas.

## Persistencia propia propuesta, sin DDL aplicado

Inventario de referencia: `MySqlImportacionCarpetasRepository.vb` usa parámetros, revisión, snapshots y reclamaciones; `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Sql/01-perfil-importacion-mysql51.sql` muestra el patrón local de scripts versionados. No implica que el ambiente de esta capacidad esté inspeccionado o tenga esas mismas tablas/versión.

| Tabla nueva propuesta | Datos y restricciones lógicas |
|---|---|
| `ra_ri_operacion` | ID opaco, namespace backend/ámbito/actor, apertura/payload hash, origen, revisiones, snapshots, estado, referencia preparada y resultado. Unicidad de apertura por contexto; reclamación/versiones y enlace durable al radicado. |
| `ra_ri_archivo` | Operación/archivo, rol, versiones, reserva, nombre visible, bytes previstos/recibidos, estado de recepción/efecto, hash y stagingRef interno. Unicidad operación/archivo y reclamación de intento; no almacenar rutas aportadas por cliente. |
| `ra_ri_confirmacion` | Operación, clave de confirmación, revisión/snapshot hash, reclamación y resultado. Unicidad operación/clave; la fila de operación impide más de un núcleo confirmado o ejecución concurrente. Rechazo previo sin efectos puede revisarse con nueva preparación; incertidumbre no. |
| `ra_ri_fase` | Operación/fase/recurso, estado, reclamación, intento, evidencia y referencias de documento/tarea/notificación. Unicidad de la fase por recurso; vencimiento no implica permiso de repetir un efecto. |

Los snapshots requieren versión de esquema, lectura validada y límites; usar TEXT/LONGTEXT o formato compatible comprobado, no depender de un tipo JSON inexistente en una versión antigua. No persistir secretos, tokens ni cadenas de conexión. Las tablas nuevas no son tablas de almacenamiento definitivo y no reemplazan el inventario/cabinete/índice.

P02 verificará antes de generar/aplicar migración la versión, motores transaccionales, índices, esquema y conexión autorizada. El tipo final de PK/FK, cotas y orden de migración se contrasta con el esquema real. Propuesta de script futuro: `implementacion/sql/01-esquema-radicacion-interna.sql`; no existe ni se aplica como parte de P01. No modificar tablas/columnas/configuración del almacenamiento protegido para correlacionar el nuevo flujo.

## Cierre de este contrato

La estructura del intercambio y sus invariantes sirven para avanzar en trabajo independiente. La alternativa documental y las capacidades nuevas no se congelan como operativas antes del acuerdo/evidencia necesarios. P01 queda en refinamiento hasta cerrar G01 y concretar sus consecuencias; P02–P06 conservarán las otras puertas y la baseline D-RI-07. Ver [inventario](inventario-archivos.md) y [resultado de P01](P01-resultado.md).
