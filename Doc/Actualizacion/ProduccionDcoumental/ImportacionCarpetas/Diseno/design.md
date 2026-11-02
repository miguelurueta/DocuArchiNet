# Inventario de implementación — DOC-111

Estado: propuesta de refinamiento, 2026-10-27; ninguna fila Nueva acredita código generado. Complementa 01-modelo-datos, 02-contratos-operacion y 03-estados-recuperacion. No sustituye sus contratos. Guía 00 fija propietarios; DOC-111 integra 04–12 y excluye 13.

Base inspeccionada: main 6313f91f, con DOC-110 en producción de código; no se presume esquema operativo instalado. Nombres de tablas propuestos permanecen los seis del modelo común hasta verificar nombres/tipos/collations en 07-A.

Los prefijos de las rutas de la tabla son relativos al repositorio:
M = Modelo/ProduccionDocumental/ImportacionCarpetas/;
D = DTOs/ProduccionDocumental/ImportacionCarpetas/;
S = Services/ProduccionDocumental/ImportacionCarpetas/;
I = Infrastructure/ProduccionDocumental/ImportacionCarpetas/;
R = Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/;
J = js/gestion/ImportacionCarpetas/.

| Archivo exacto (expandir prefijo) | Estado | Propietario | Responsabilidad y consumidor |
|---|---|---|---|
| M/ImportacionCarpetasModels.vb | Nuevo propuesto | 04/07-A | Contexto operativo, manifiesto, snapshots, estados/resultados; servicios/repositorio. |
| M/ImportacionCarpetasInterfaces.vb | Nuevo propuesto | 04/07-A | Puertos de perfil operativo, política, repositorio, staging y efectos; 06–11. |
| D/ImportacionCarpetasDtos.vb | Nuevo propuesto | 06/07/10/11 | Requests/sobre y páginas; ASMX/ASHX y cliente. |
| I/ImportacionCarpetasContextAdapter.vb | Nuevo propuesto | 06 | Identidad activa/empresa/facultad/recursos, sin administración implícita. |
| I/ImportacionCarpetasRequestGuard.vb | Nuevo propuesto | 06/07-B | JSON/multipart y token por propósito/contexto; fachadas operativas. |
| I/AdaptadorPerfilImportacionOperativa.vb | Nuevo propuesto | 06 | Lectura de publicación y valores con autoridad operativa; preparación/confirmación. |
| M/PerfilImportacionInterfaces.vb | Existente, extender preservando contratos | 02/06 | Puerto de lectura por propósito, sin otra configuración de perfil. |
| S/ServicioPerfilImportacion.vb | Existente, compatibilidad obligatoria | 02/06 | Preservar semántica ADMINISTRACION; no convertir PuedeAdministrar en atajo. |
| R/MySqlPerfilImportacionRepository.vb | Existente, extensión acotada | 02/06 | Reutilizar lectura autorizada mediante puerto operativo, manteniendo mutaciones administrativas. |
| I/PerfilImportacionCatalogAdapter.vb | Existente, evaluar composición/extensión | 02/06 | Reutilizar validación/consulta de catálogo sin exigir facultad administrativa al importador. |
| I/MySqlProveedorDatosIndices.vb | Existente, extensión acotada por propósito | 02/06 | Motor de consulta compatible; no elevar privilegios ni eludir política RA. |
| S/ResolvedorReglasPerfilImportacion.vb | Existente, consumir | 02/06/08/09 | Resolución pura y diferidos, códigos textuales; evitar duplicar reglas. |
| S/LimitesImportacionCarpetas.vb | Nuevo propuesto | 04/06/07 | Política servidor validada/inyección; sin cuota no se habilita capacidad. |
| S/CanonizadorManifiestoImportacion.vb | Nuevo propuesto | 06 | Canonización, identidades, completitud y límites; preparación/recepción. |
| S/ServicioPreparacionImportacion.vb | Nuevo propuesto | 06 | Abrir/preparar, contexto fijo, valores/elegibilidad/snapshot; fachada. |
| R/MySqlImportacionCarpetasRepository.vb | Nuevo propuesto único | 07-A | Plan/confirmación/recepción/intentos/eventos/consulta; 06–11. |
| S/ServicioRecepcionImportacion.vb | Nuevo propuesto | 07-B | Confirmar/recibir y reconciliar parte/fila; ASMX/ASHX. |
| I/AlmacenStagingImportacion.vb | Nuevo propuesto | 07-B | Stream/contención/hash/cuota, original retenido y copias; recepción/09. |
| I/AdaptadorExpedienteImportacion.vb | Implementado y probado localmente, 2026-10-29 | 08 | Fase de creación con comando explícito, inicio durable e incertidumbre; puerto estrecho ICreacionExpedienteImportacion, sin endpoint. |
| I/AdaptadorExpedienteImportacion.Legacy.vb | Binding compilado, sin invocación real | 08 | Composición con autoridad/catálogos/repositorio y llamada nombrada a Conservación; conserva IDs ByRef en Finally. |
| R/MySqlImportacionCarpetasRepository.ResultadoCreacion.vb | Implementado localmente, 2026-10-29 | 10 | Checkpoint de creación en intento/raíz/lote/evento, repetición exacta e incertidumbre; mismo repositorio y tablas. |
| S/CoordinadorImportacionCarpetas.Barreras.vb | Implementado localmente, 2026-10-29 | 10 | Evaluación pura de recepción por EXPEDIENTE/LOTE o parcial contra inventario confirmado, sin efectos. |
| S/CoordinadorImportacionCarpetas.Staging.vb | Implementado localmente, 2026-10-29 | 10 | Revalidación física y autoridad antes de una reserva de creación; conserva rechazados/inciertos y no invoca adaptadores. |
| S/CoordinadorImportacionCarpetas.Creacion.vb | Implementado localmente, 2026-10-29 | 10 | Una fase por raíz: recurso propio, idempotencia, preparación/reserva, relectura y creador con retención física; sin bucle documental o endpoint. |
| S/CoordinadorImportacionCarpetas.Documento.vb | Implementado localmente, 2026-10-30 | 10 | Una fase por archivo, histórico antes del disco, barreras compartidas, retención de originales y lectura del checkpoint; sin cierre de raíz/lote o endpoint. |
| S/CoordinadorImportacionCarpetas.Consolidacion.vb | Implementado localmente, 2026-10-30 | 10 | Cálculo puro de estados desde todos los originales y pruebas terminales, sin fuentes/efectos o reconciliación automática. |
| S/CoordinadorImportacionCarpetas.Reconciliacion.vb | Implementado localmente, 2026-10-30 | 10.8 | Reconciliación explícita por intento/revisión con autoridad vigente y evidencia conocida; sin nuevos efectos. |
| S/CoordinadorImportacionCarpetas.Operaciones.vb | Implementado localmente, 2026-10-30 | 10.11 | Fachada de una fase por recurso y proyección pública de reconciliación; sin encadenar efectos. |
| S/ServicioConsultaImportacion.Limpieza.vb | Implementado localmente, 2026-10-30 | 11.7 | Política explícita, revalidación y retiro de un original completo; consultas y diagnóstico no lo invocan. |
| I/AlmacenStagingImportacion.Limpieza.vb y ComposicionLimpiezaOriginalImportacion.vb | Implementados localmente, 2026-10-30 | 11.7 | Bloqueo/rutas/hash privados y comprobación final SELECT, sin alterar almacenamiento legacy o exponer endpoint. |
| R/MySqlImportacionCarpetasRepository.Limpieza.vb | Parcial del repositorio único, 2026-10-30 | 11.7 | Diario preparado/concluido en eventos existentes, vista completa bloqueada y recuperación sin borrar metadatos. |
| tools/e2e/tests/support/doc111-importacion-carpetas.cjs | Implementado localmente, 2026-10-30 | 13.4 | Capacidades y controles SELECT; sesión/transporte existentes, sin reenvío incierto. |
| tools/e2e/tests/doc111-importacion-carpetas.spec.cjs | Registrado y probado con dobles, 2026-10-30 | 13.5 | Un recorrido REC-01/02 por EXPEDIENTE, LOTE y PARCIAL; aceptación real pendiente. |
| tools/e2e/scripts/run-doc111-importacion-carpetas.cjs | Implementado localmente, 2026-10-30 | 13.6–13.7 | Selector del paquete existente, --check sin efectos, autorización TTY y resumen saneado. |
| J/ImportacionCarpetas.js, Api/Vista y WebFormProducionDocumental.js | Integración local, 2026-10-30 | 12.11 | Fases explícitas, detención/verificación sin replay y rama por relaciones confirmadas mediante helper existente, sin cambiar selección o duplicar nodos. |
| W/WebServiceImportacionCarpetas.Composicion.vb y ASMX existente | Implementados localmente, 2026-10-30 | 10.11 | Avance/detención/reconciliación con guard operativo y composiciones existentes; binding productivo compilado, HTTP sintético. |
| I/VerificadorReconciliacionImportacion.vb y ComposicionReconciliacionImportacion.vb | Implementados localmente, 2026-10-30 | 10.8 | Catálogos/lectores SELECT existentes, sin puertos de ejecución, staging o reserva. |
| R/MySqlImportacionCarpetasRepository.Reconciliacion.vb | Parcial del único repositorio, 2026-10-30 | 10.8 | Vista correlacionada, confirmación canónica CAS y consolidación en la misma transacción; replay sin eventos. |
| R/MySqlImportacionCarpetasRepository.Consolidacion.vb | Parcial del único repositorio, 2026-10-30 | 10 | Lectura acotada bajo bloqueo y consolidación CAS/eventos en transacción de checkpoint; conserva IDs, política, señal y totales. |
| I/ComposicionDocumentoImportacion.vb | Composición real compilada, 2026-10-30 | 10 | Mismos autoridad/repositorio/staging/límites y adaptador documental; sin modificar configuración o habilitar un worker. |
| I/ComposicionCreacionImportacion.vb | Composición compilada, sin ejecución real | 10 | Autoridad/repositorio/staging/adaptador existentes, mismas políticas servidor; no modifica configuración ni agrega rutas HTTP. |
| S/CoordinadorImportacionCarpetas.Valores.vb | Implementado localmente, 2026-10-30 | 06/10 | Materializa un archivo tras creación confirmada, resuelve solo pendientes y conserva valores previos; repetición sin proveedores. |
| R/MySqlImportacionCarpetasRepository.Valores.vb | Implementado localmente, 2026-10-30 | 07-A/10 | Vista propia comprobada por IDs y checkpoint CAS de valores/procedencia o rechazo; mismas tablas, sin escritura documental. |
| I/ComposicionPerfilImportacionOperativa.vb | Composición compartida compilada, 2026-10-30 | 06/10 | Misma construcción de catálogo/proveedor/política para selección, preparación y materialización; sin nuevas rutas/configuración. |
| R/MySqlImportacionCarpetasRepository.Barreras.vb | Implementado localmente, 2026-10-29 | 10 | Lectura acotada de metadatos de raíz/archivo bajo bloqueo común; consulta previa y revalidación antes de iniciar creación. |
| I/ComandoCreacionExpedienteImportacion.vb | Implementado localmente, 2026-10-29 | 08 | Composición inmutable desde lote/raíz/intento y valores resueltos; preparación interna para el adaptador, sin invocación legacy. |
| I/PerfilImportacionCatalogAdapter.Creacion.vb | Ampliación local del catálogo compartido, 2026-10-29 | 08 | Lectura de nombres por IDs confirmados y comprobación inversa bajo el alcance legacy; gabinete autorizado y responsable interno, sin efectos. |
| I/AdaptadorDocumentoImportacion.vb y .Legacy.vb | Implementado localmente/binding compilado, 2026-10-30 | 09 | Comando inmutable, reserva nueva, copia PDF consumible con original retenido, llamada nombrada única y checkpoint; sin endpoint ni carga real. |
| I/ComandoAlmacenamientoCarpetas.vb | Implementado localmente, 2026-10-30 | 09 | Archivo/intento/snapshot y valores fijados, compatibilidad Integer/texto y conflictos con campos automáticos/técnicos. |
| I/PerfilImportacionCatalogAdapter.Documento.vb | Ampliación del catálogo compartido, 2026-10-30 | 09 | Nombres físicos, clase electrónica/PDF y SELECT de documento/inventario/incorporación/nombres/índices; no incorpora otra vez. |
| R/MySqlImportacionCarpetasRepository.Documento.vb | Parcial del único repositorio, 2026-10-30 | 10 | Inicio durable y resultado documental en intento/archivo/lote/evento, idempotencia y preservación de incertidumbre; sin nuevas tablas. |
| S/CoordinadorImportacionCarpetas.vb | Nuevo propuesto | 10 | Reclamo, una fase, barreras, detención/reconciliación; fachada. |
| S/ServicioConsultaImportacion.vb | Nuevo propuesto | 11 | Lectura/páginas/CSV y diagnóstico/mantenimiento con política. |
| webservice/WebServiceImportacionCarpetas.asmx | Existente | 06–11 | Misma fachada; no servicio web paralelo. |
| webservice/WebServiceImportacionCarpetas.asmx.vb | Existente, ampliar | 06–11 | Dispatcher operativo separado del administrativo, DTO/errores; sin reglas documentales. |
| generic_control/ImportacionCarpetasUpload.ashx | Nuevo propuesto | 07-B | Entrada multipart propia, no handler compartido. |
| generic_control/ImportacionCarpetasUpload.ashx.vb | Nuevo propuesto | 07-B | Guard y delegación, sin coordinación documental. |
| Gestion/WebFormProducionDocumental.aspx | Existente, ampliar | 05 | Contenedor/entrada propios sin segundo árbol/formulario. |
| Gestion/WebFormProducionDocumental.aspx.vb | Existente, ampliar | 05/06 | Contexto de apertura sin redirigir lotes por Session. |
| js/gestion/WebFormProducionDocumental.js | Existente, ampliar acotadamente | 05/11 | Nuevo disparador/actualización rama; conservar ActivaCargaArchivos. |
| J/ImportacionCarpetas.js | Nuevo propuesto | 05/11 | Estado/ciclo de vida, preparación y lote; vista/transporte. |
| J/InventarioImportacion.js | Nuevo propuesto | 05 | Colección File/raíces/identidades; manifiesto y selección repetible. |
| J/ImportacionCarpetasApi.js | Nuevo propuesto | 05/07/10/11 | JSON/multipart, errores/progreso y reconciliación sin replay. |
| J/ImportacionCarpetasVista.js | Nuevo propuesto | 05/11 | Etapas, páginas, texto seguro y acciones permitidas. |
| J/ImportacionCarpetas.css | Nuevo propuesto | 05 | Estilos encapsulados, foco/adaptación. |
| Sql/02-carga-carpetas-mysql51.sql (bajo raíz funcional) | Nuevo propuesto | 07-A | DDL operativo/permiso revisado y manual; sin DDL runtime. |
| Sql/02-aplicacion-carga-carpetas.md (bajo raíz funcional) | Nuevo propuesto | 07-A | Pre/postcontroles SELECT, recuperación y límites de repetición. |
| tests/ImportacionCarpetasBehaviorTests.cs | Nuevo propuesto | Por capacidad | Modelo/servicio/guard, transiciones y fallos; arnés Framework existente. |
| tests/doc111-importacion-carpetas.test.cjs | Nuevo propuesto | 05/12 | Comportamiento cliente/contratos, reutilizando utilidades existentes. |
| tests/PerfilImportacionBehaviorTestRunner.cs | Existente, extensión compatible | 12 | Invocar pruebas nuevas sin sustituir familias DOC-110. |
| tools/e2e/tests/doc111-importacion-carpetas.spec.cjs | Nuevo propuesto | Capacidades/12 | REC agrupa casos especializados, sin suite duplicada. |
| tools/e2e/tests/support/doc111-importacion-carpetas.cjs | Nuevo propuesto | Capacidades | Drivers/datos sintéticos; consume sesión/utilidades existentes. |
| tools/e2e/scripts/run-doc111-importacion-carpetas.cjs | Nuevo propuesto | 12 | Selector de escenarios que delega consola/auth/reporteros; sin otro login/runner. |
| tools/e2e/package.json | Existente, ampliar | 12 | Comando focal; mismas dependencias/configuración/reporteros. |
| GestionDocumental-Docuarchi.net.vbproj | Existente, ampliar por fase | Cada entrega | Compile/Content de archivos efectivamente implementados, no incluir fixtures/secretos. |

## Componentes que no se sustituyen

Verificación estructural DOC-111, 2026-10-28: el parcial `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Verificacion.vb` añade lectura por IDs conocidos y queda incluido en Compile/arnés. No es otro repositorio, no modifica legacy ni expone endpoint. Adelanta parte de 9.4 sin cerrarla ni invocar creación. La revisión de helpers y metadatos históricos se documenta en 15; controles SELECT en el documento SQL existente. Evidencia `Validacion/doc111-verificacion-expediente-local-20261028.md`; 9.3/9.4 abiertas, avance total 95/130.

Retención DOC-111, 2026-10-28: el servicio de consulta y sus DTO existentes incorporan diagnóstico paginado en seco de referencias registradas. No hay archivos productivos, repositorios, endpoints, configuración o permisos nuevos. El reloj es inyectable para pruebas; la duración opcional proviene de composición servidor. El puerto de consulta no dispone de borrado ni acceso a disco. Contrato 02 y `Validacion/doc111-retencion-diagnostico-local-20261028.md`; 11.7 permanece pendiente y almacenamiento intacto.

Recuperación DOC-111, 2026-10-28: se amplían el mismo puerto/repositorio/servicio/ASMX de consulta con ConsultarConfirmacionLote por plan, y el guard emite un espacio opaco para pistas del navegador ligado al contexto. El controlador existente persiste solo IDs, admite consulta por lote y vincula File reseleccionado a un recurso pendiente explícito. No se añaden tablas, archivos productivos, bibliotecas ni plataforma de pruebas. El archivo real de pruebas cliente es `tests/doc111-importacion-carpetas-client.test.cjs`, ampliado para 13.1–13.3. Contrato 02 y `Validacion/doc111-recuperacion-local-20261028.md`; 12.11 y efectos permanecen pendientes.

Interfaz DOC-111, 2026-10-28: se implementan `J/ImportacionCarpetasApi.js`, `J/ImportacionCarpetasVista.js`, `J/ImportacionCarpetas.css`, se amplía el controlador y se reutiliza el inventario previamente implementado. Se incluyen como Content. Producción añade RenderImportacionCarpetas/PreRender y un disparador de menú que delega en el módulo; ActivaCargaArchivos permanece intacto. El servicio de consulta expone su máximo de página ya configurado. Pruebas existentes de cliente se amplían con transporte simulado y Chromium local, sin runner/login/configuración nuevos. Evidencia `Validacion/doc111-interfaz-local-20261028.md`; 12.10/12.11 siguen pendientes y los writers documentales no se modifican.

Consulta DOC-111, 2026-10-28: se implementan `S/ServicioConsultaImportacion.vb` y el parcial `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Consulta.vb` del mismo repositorio. Se registran Compile y arnés Framework existente. El ASMX incorpora consulta de lote/recursos y exportación CSV de página mediante guard operativo. Acciones públicas de lectura conservadoras, política explícita de tamaño de página y proyección sin evidencia privada. Contrato 02 y `Validacion/doc111-consulta-exportacion-local-20261028.md`; no se implementan limpieza ni efectos documentales.

Corte de control DOC-111, 2026-10-28: `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlImportacionCarpetasRepository.Control.vb` es parte de la misma clase/repositorio y se registra como Compile junto con el arnés existente. Añade reserva documental y señal de detención; el coordinador integra autorización de esta última. Son capacidades internas sin invocación legacy ni endpoint nuevo. Evidencia y límites en `Validacion/doc111-reserva-detencion-local-20261028.md`; 10.9 sigue parcial y almacenamiento intacto/deshabilitado.

Carga individual: ActivaCargaArchivos, IniLoadPerson y FileUploadHandler compartidos. SII, sus adaptadores/repositorios/contratos y tablas no se modifican. Crear un archivo por fase solo al implementar su responsabilidad; no crear stubs para completar el inventario.

Registrar_Expediente_Conservacion y AlmacenamientoDocumentoProduccionDocumental son candidatos a invocación acotada; esta lista no los certifica como adaptadores seguros. La caracterización previa puede impedir usarlos y debe registrar una operación inferior compatible antes de habilitar efectos.

No se inventaría host/proyecto 13. Mantener estados y contratos en documentos comunes; cualquier cambio se refleja en consumidores, tareas y pruebas. La selección de nombres propuesta aquí está sujeta a revisión del refinamiento antes de código.

Transiciones y aislamiento DOC-111, 2026-10-28: `S/CoordinadorImportacionCarpetas.vb` implementa exclusivamente la tabla cerrada de las seis dimensiones; el repositorio aplica su validación a recepción. No invoca adaptadores ni implementa todavía barreras o ejecución documental. El almacenamiento permanece deshabilitado por la ventana de guardado XML posterior al commit y la falta de exclusión comprobada con escritores externos; TIFF/Multi-TIFF se rechaza. Caracterización en 15, estados en 03 y evidencia en `Validacion/doc111-transiciones-aislamiento-20261028.md`.

Actualización DOC-111, 2026-10-28: `ServicioPreparacionImportacion.vb` implementa apertura, cálculo y persistencia del plan; `ServicioRecepcionImportacion.vb` incorpora confirmación revalidada (recepción de bytes aún pendiente). `MySqlImportacionCarpetasRepository.vb` mantiene un único repositorio y añade lectura preventiva de candidatos y referencia autorizada del plan. El ASMX existente incorpora PrepararPlan/ConfirmarPlan a su dispatcher operativo. Los componentes VB están registrados en Compile. Contratos vigentes en 02/06 y evidencia en `Validacion/doc111-plan-confirmacion-20261028.md`; no se añaden tablas, runners ni endpoints de adaptadores documentales.

Staging DOC-111, mismo día: `I/AlmacenStagingImportacion.vb` implementa el puerto interno de recepción/verificación/copia con recursos de servidor, escritura acotada, PDF validado con iTextSharp existente y original retenido independiente. Se registra Compile y se prueba con streams/archivos sintéticos en el arnés Framework previo. No se integra aún al servicio o al handler multipart ni confirma filas de recepción. Evidencia: `Validacion/doc111-staging-privado-20261028.md`.

Recepción DOC-111, corte posterior del mismo día: se integra staging al servicio mediante IRecepcionArchivosImportacionRepository, implementado en el repositorio operativo existente. Se añaden lectura autorizada archivo/lote, reserva de intentos RECEPCION, confirmación transaccional, reenvío y reconciliación interna. El handler previsto `generic_control/ImportacionCarpetasUpload.ashx[.vb]` se incluye como Content/Compile y delega guard/servicio. El resultado del intento incorpora evidencia tipada de recepción. No se habilitan reclamaciones documentales, otros repositorios, migraciones o configuración automática. Evidencia local: `Validacion/doc111-recepcion-integrada-local-20261028.md`; MySQL/HTTP reales pendientes.
