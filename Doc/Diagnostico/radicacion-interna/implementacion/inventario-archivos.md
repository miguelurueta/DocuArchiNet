# Inventario de archivos y responsabilidades para implementar radicación interna

Estado: propuesta técnica de P01, 2026-11-26; no son componentes ya implementados. Las rutas se concretan para evitar crear clases en capas incorrectas o duplicar los módulos existentes. No crear archivos vacíos por completar esta tabla. Los nombres pueden revisarse con evidencia, actualizando contratos/consumidores antes del código afectado.

## Funciones existentes y consumidores

El [mapa M01–M31](../mapa-funciones-implementacion.md) mantiene las funciones y subfases; esta tabla concreta el límite por archivo. Antes de editar, actualizar consumidores y ejecutar la caracterización correspondiente.

| Archivo existente | Función o intervención permitida | Consumidores y verificación primaria |
|---|---|---|
| `Gestion/WebFormProducionDocumental.aspx.vb` | `ButtonRadicar_Click`: capturar selección autorizada y abrir operación propia. | Producción → iframe; C01/C02/C07. |
| `Gestion/WebFormProducionDocumental.aspx` | Modal/iframe y cierre interno, sin reemplazar selección de otra operación. | Padre/iframe; C07/C17. |
| `radicador/WebFormRadicacionEntranteInterna.aspx` y `.aspx.vb` | Ciclo de controles dinámicos, apertura y adaptación del botón de confirmación; preservar campos/catálogos y estados. | Ambas entradas, UpdatePanel, campos condicionados; C03/C07/C17. |
| `Gestion/ClassGaProducionDocumental.vb` | Solo funciones no protegidas: generación, configuración, autorización y consultas necesarias según M07–M10. | Producción/interna y plantillas; C02/C03/C08. Las cuatro funciones de almacenamiento/metadatos protegidas no se editan. |
| `radicador/ClassRadicador.vb` | Selecciones/calendario y adaptación/extracción mínima del registro, justificada por D-RI-06. | Invariantes del núcleo y consumidores comprobados; C05/C06/C12/C13/C18. El código de almacenamiento no se copia a este archivo. |
| `radicador/Class_tipo_doc_entrante.vb` | Lectura canónica de modalidad con caracterización de consumidores; conservar lectores públicos o migrarlos explícitamente. | Clásica/Simplificada/interna; C05. |
| `radicador/Class_plantilla_validacion.vb`, `Class_remit_dest_interno.vb` | Referencias de catálogos/identidades; separar consultas de escrituras y no mezclar IDs de terceros con internos. | Remitente, campos de plantilla y relación; C04. |
| `workflow/ClassWorkflow.vb` y notificador en `ClassGaProducionDocumental.vb` | Invocación/correlación mínima justificada, sin nueva política de respuestas ni reenvío ciego. | Workflow/notificación; C14/C15. |
| `Gestion/Classgestionrespuesta.vb`, `radicador/Class_ra_respuesta_radicado.vb` y métodos de respuesta relacionados | Lectura/caracterización de compatibilidad; no implementar aquí elaboración, envío ni cierre. | Bandeja/propietario del módulo existente; C15. |
| `js/radicacion/WebFormRadicacionEntranteInterna.js` | Estado de UI, selección, invalidaciones y nuevo control; inicialización/destrucción idempotentes. | Ambos accesos, cambios tardíos y teclado; C07/C17. |
| `Defaul/ClassGestorSesion.vb` | Punto real de menú identificado: `Solicita_items_modulo_gestion`, línea 1605; nodo actual de Producción en 1911–1915. Definir acceso directo según G03, sin reutilizar su código de nodo ni conceder permisos por visibilidad. | Navegación inicial y sesión nueva; C02/C18. La selección general de menú también debe inventariarse antes de agregar un nodo. |
| `GestionDocumental-Docuarchi.net.vbproj` | Altas Compile/Content de lo implementado y comprobación de publicación. | Artefacto real, no archivos solo locales; C18. |

Los 10 archivos/24 implementaciones de [almacenamiento-intocable-baseline.json](../almacenamiento-intocable-baseline.json) conservan la protección de D-RI-07. También se preservan el cargador compartido, su handler y los contratos de `WebServiceRadicacion.asmx`; no se amplían por comodidad del módulo nuevo.

## Altas propuestas por responsabilidad

| Ruta propuesta | Clase/contrato y operaciones | Consumidor / paquete |
|---|---|---|
| `Modelo/RadicacionInterna/OperacionRadicacionInterna.vb` | Modelo de operación, origen, revisiones y resultado con referencias tipadas. | Servicios y repositorio; P02. |
| `Modelo/RadicacionInterna/ContratosRadicacionInterna.vb` | Puertos mínimos para autoridad, operación, recepción, almacenamiento externo y registro. Sin `Page`, `UpdatePanel` ni SQL en el modelo. | N01–N11; P02 y ampliaciones justificadas. |
| `Modelo/RadicacionInterna/EstadosRadicacionInterna.vb` | Estados/códigos y transiciones válidas; distinguir registro y fases. | Servicios/transporte/UI; P02. |
| `DTOs/RadicacionInterna/ContratosOperacion.vb` | Apertura, actualización, consulta y capacidades. | ASMX/página; P02. |
| `DTOs/RadicacionInterna/ContratosConfiguracion.vb` | Configuración/campos/destinos y remitente tipado. | ASMX, página y catálogos; P02/P03. |
| `DTOs/RadicacionInterna/ContratosDocumentos.vb` | Reserva, solicitud de recepción, manifiesto y resultado por archivo. | Nuevo control/handler; P04. |
| `DTOs/RadicacionInterna/ContratosConfirmacion.vb` | Revisión preparada, confirmación, resultado y seguimiento consultable. | ASMX/UI; P05. |
| `DTOs/RadicacionInterna/RespuestaRadicacionInterna.vb` | Sobre lógico, errores de campo y advertencias sin secretos/rutas. | ASMX/handler; P02. |
| `Infrastructure/RadicacionInterna/RadicacionInternaContextAdapter.vb` | N01: `ResolverActorAutenticado`, `AutorizarOperacion`, `AutorizarRecursoDocumental`. | Cada caso de uso; P02. |
| `Infrastructure/RadicacionInterna/RadicacionInternaRequestGuard.vb` | Token efímero propio, origen/contenido/formato/límites; lectores JSON/multipart según contrato. | Transporte; P02/P04. |
| `Infrastructure/RadicacionInterna/ComposicionRadicacionInterna.vb` | Composición por petición de servicios/puertos y conexión autorizada. No capturar usuario en un singleton global. | Página, ASMX y handler; P02. |
| `Infrastructure/Repositories/RadicacionInterna/MySqlOperacionRadicacionRepository.vb` | N03: abrir/leer propio, CAS de revisión, archivos/reclamaciones, confirmación y fases. SQL parametrizado del estado nuevo. | N02/N06/N09/N10; P02–P05. |
| `Services/RadicacionInterna/ServicioOperacionRadicacionInterna.vb` | N02: `AbrirDesdeProduccion`, `AbrirCargaDirecta`, `ObtenerAutorizada`, `ActualizarBorrador`, `PrepararConfirmacion`, `CancelarOperacion`. | Página/ASMX y servicios; P02. `PrepararConfirmacion` coordina la revisión exacta, no registra. |
| `Services/RadicacionInterna/ServicioConfiguracionRadicacionInterna.vb` | N04: configuración, reglas de seguimiento, ruta y destinos autorizados. | Operación y página; P02. |
| `Services/RadicacionInterna/ServicioRemitenteRadicacionInterna.vb` | N05: `BuscarRemitentes`, `ResolverRemitenteAutorizado`; creación de relación solo cuando corresponda. | Revisión/confirmación; P03. |
| `Services/RadicacionInterna/ServicioRecepcionRadicacionInterna.vb` | N06: reserva, `RecibirArchivo`, `ConsultarRecepcion`, `RetirarArchivo`, `PrepararManifiesto`. | Handler/ASMX/revisión; P04. |
| `Infrastructure/RadicacionInterna/AlmacenTemporalRadicacionInterna.vb` | N07: reservar/escribir/verificar/retirar temporales propios y copia de trabajo; no almacenamiento definitivo paralelo. | Recepción/adapter; P04. |
| `Services/RadicacionInterna/ServicioDocumentosRadicacionInterna.vb` | N08: `IncorporarDocumentos`, `ConsultarIncorporacion`, `ReconciliarIncorporacion`. Orquestación/evidencia externa; sin alterar funciones congeladas. | Confirmación/finalización; P04/P05. |
| `Infrastructure/RadicacionInterna/AlmacenamientoRadicacionInternaAdapter.vb` | Adaptador externo a las firmas existentes: contexto compatible, copia temporal propia, llamada y lectura de referencias. Justificar viabilidad de sesión/efectos sin copiar motor/SQL definitivo. | N08; P04. |
| `Services/RadicacionInterna/ServicioConfirmacionRadicacionInterna.vb` | N09: `ConfirmarOperacion`, coordinación de `RegistrarNucleo`, `ConsultarResultadoConfirmacion`. No duplicar el monolito. | ASMX/página; P05. La ubicación mínima del núcleo y su transacción se concreta tras caracterización, preservando su firma pública legacy. |
| `Services/RadicacionInterna/ServicioFinalizacionRadicacionInterna.vb` | N10: `CompletarFasesPendientes`, `ConsultarResultado`, `RecuperarPendientes`. | Resultado/recuperación; P05. |
| `Services/RadicacionInterna/ServicioSeguimientoRadicacionInterna.vb` | N11: creación de obligación integrada en la transacción del núcleo y consulta autorizada. No endpoint público separado para crearla ni responderla. | N09/consumidor vigente; P05. |
| `webservice/WebServiceRadicacionInterna.asmx` y `.asmx.vb` | N12: métodos públicos tipados de [contratos v1](contratos-v1.md); transporte sin SQL/motor en la frontera. | UI; P02 y ampliaciones P03/P05. |
| `generic_control/RadicacionInternaUpload.ashx` y `.ashx.vb` | N12: un archivo por recepción multipart con operación/archivo/intento/versiones autorizados. | Control nuevo; P04. |
| `js/radicacion/RadicacionInternaUpload.js` | N13: clase independiente y transporte/eventos por instancia; no `LoadFilePERSON`, `IniLoadPerson` ni globals compartidas. | Formulario interno; P04/P06. |
| `Styles/radicacion-interna.css` | Estilos acotados a la página/control; conservar teclado, foco, errores, zoom y tamaños. | Página/control; P06. |
| `implementacion/sql/01-esquema-radicacion-interna.sql` | Ruta documental propuesta para migración futura de tablas propias, después de comprobar esquema/versión. No existe ni se aplica en P01. | Instalación autorizada P02/P06. |

Los nombres de métodos son responsabilidades internas; no construir un endpoint por cada método privado ni capas adicionales sin necesidad. D-RI-07 impide resolver incompatibilidades agregando sobrecargas al almacenamiento.

## Unidades de trabajo y verificación

| Unidad | Rutas principales | Cierre verificable |
|---|---|---|
| P02 / RI01 | Markup/code-behind/JS internos y funciones no protegidas del generador | C03/C07/C17: una instancia de dependencias, valores/eventos conservados y cierre correcto. |
| P02 / RI02 | N01/N02/N03/N12, apertura de Producción y menú identificado | C01/C02/C07/C12: contexto propio, CAS, autoridad y dos pestañas; migración comprobada antes del ambiente. |
| P02 / RI03 | N04 y lectores de configuración/destino no congelados | C03/C05/C06: tipos/campos, modalidades y calendario con consumidores caracterizados. |
| P03 / RI04 | N05 y catálogos autorizados | C04: identidad tipada y consulta sin altas incidentales; G02–G05 refinadas donde aplique. |
| P04 / RI05 | Control, request guard, handler, N06/N07 | C07/C11/C17: recepción íntegra, cancelación/respuestas tardías y aislamiento; G03/G06/G07/G08. |
| P04 / RI06 | N08/adaptador externo, funciones legacy solo por llamada | C08–C11/C13: baseline intacta, referencias y metadatos trazables; alternativa G01 compatible. |
| P05 / RI07 | N09 y núcleo mínimo no protegido por D-RI-07 | C12/C13: un registro/obligación y asociación durable, antes/después del commit, sin atomicidad documental ficticia. |
| P05 / RI08–RI09 | N10/N11 y consumidores existentes | C13–C16: efectos reconocidos no repetidos, pendientes explícitos y seguimiento disponible sin gestionar respuesta. |
| P06 / RI10–RI11 | Página/JS/control/estilos/proyecto y publicación | C01–C18 en el alcance acordado, evidencia autorizada, baseline y artefacto publicado coherentes. |

Las pruebas de ambiente requieren autorización específica. Los tests estáticos actuales sirven como línea base, no como sustituto de transacción, almacenamiento y UI reales. La [matriz](matriz-caracterizacion.md) define resultados de dominio; las rutas finales de las pruebas funcionales se concretan al implementar cada unidad reutilizando la infraestructura existente y sin otro login/arnés paralelo.
