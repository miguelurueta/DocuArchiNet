<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14,D-15,D-16 -->
## Context

DOC126 conecta la interfaz con DOC122–125 ya integrados; DOC125 terminó en main por PR137/138, con aceptación física5/5 de1000filas por modo y31/31tareas. La página todavía carece de host/cliente/vista Excel. El constructor del menú ya existe como utilidad sin dependencia de permisos de carpetas. El parser de perfiles exige otro envelope y no puede usarse como parser Excel. La apertura del servicio emite el token únicamente por POST JSON.

El prompt original se conserva íntegro en specs/importacion-excel-06-interfaz-menu/jira-context.md y en la exploración06. El valor inicial de10000del prompt no reduce la configuración vigente20000;10MB equivale a10000000bytes. Ver propuesta para objetivo funcional.

## Goals / Non-Goals

Entregar interacción y navegación sobre contratos existentes, con recuperación por ID y controles accesibles. La fase conserva transacciones y límites del backend. CSV, pausa por filas, cancelación garantizada al cerrar, migración entre gabinetes, omisión/upsert de duplicados y publicación de fuentes quedan fuera de esta fase.

## PRECONDICIONES DE RUTAS

Inventario contrastado con la carpeta principal antes de implementar. Las rutas nuevas se crearán en sus responsabilidades previstas; Api/Vista/CSS se registran como Content en GestionDocumental-Docuarchi.net.vbproj. Modelos/DTOs/servicios/repositorios/ASMX/receptor ya existen y se reutilizan; cualquier cambio de contrato exige justificar y revisar este diseño antes de implementarlo.

| Estado | Archivo | Responsabilidad |
| --- | --- | --- |
| Existente | `Gestion/WebFormProducionDocumental.aspx` | Página e inclusión ordenada de assets; editar |
| Existente | `Gestion/WebFormProducionDocumental.aspx.vb` | RenderPerfilImportacion/RenderImportacionCarpetas y PreRender; añadir host Excel propio |
| Existente | `Gestion/Class_remit_dest_interno_perfil_produccion.vb` | Permiso Excel ya cargado; preservar, sin usar la caché de sesión como autoridad |
| Existente | `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js` | asegurarMenu/rotularAccesoMenu compartidos y disponibles sin host de carpetas; reutilizar |
| Existente | `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js` | Sección administración y lifecycle; preservar |
| Existente | `js/gestion/ImportacionCarpetas/PerfilImportacionApi.js` | Patrón de transporte; su envelope no es el contrato Excel |
| Existente | `DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb` | Contratos de apertura, plantilla, recepción, preparación y confirmación; preservar |
| Existente | `Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb` | Plantilla/campos/incidencias/valores normalizados existentes; preservar |
| Existente | `webservice/WebServiceImportacionExcel.asmx` | Endpoint existente; reutilizar |
| Existente | `webservice/WebServiceImportacionExcel.asmx.vb` | Nueve métodos actuales y envelope ok/codigo/mensaje/operacionId/datos; reutilizar |
| Existente | `generic_control/ImportacionExcelUpload.ashx` | Recepción multipart actual; reutilizar |
| Existente | `generic_control/ImportacionExcelUpload.ashx.vb` | Validación previa del transporte/autoridad y receptor; preservar |
| Existente | `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelContextAdapter.vb` | Resolver y permiso vigente por RA; reutilizar |
| Existente | `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb` | Emitir sólo JSON POST; añadir referencia opaca de recuperación sin debilitar transportes |
| Existente | `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb` | Referencias autorizadas y límites del servidor; reutilizar |
| Existente | `Infrastructure/ProduccionDocumental/ImportacionExcel/ComposicionCargaExcel.vb` | Composición lazy y autoridad RA/DA actuales; preservar |
| Existente | `Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb` | Listar/Obtener/Guardar con revisión/huella, tipos y modo históricos; preservar |
| Existente | `Services/ProduccionDocumental/ImportacionExcel/ServicioRecepcionExcel.vb` | ID/referencia privada, validación completa y Hojas; preservar |
| Existente | `Services/ProduccionDocumental/ImportacionExcel/ServicioPreparacionExcel.vb` | PreparacionHashEsperada, muestra/incidencias25 y validación íntegra; preservar |
| Existente | `Services/ProduccionDocumental/ImportacionExcel/ServicioConfirmacionExcel.vb` | Resultado durable, consentimiento y recuperación histórica; preservar |
| Existente | `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb` | Persistencia de plantillas existente; preservar |
| Existente | `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlCargaExcelRepository.vb` | Persistencia/estados y bloqueos existentes; preservar |
| Existente | `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlConfirmacionExcelWriter.vb` | Transacción, UNIQUE y rollback DOC125; preservar |
| Existente | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasRequestGuard.vb` | Patrón de referencia opaca contextual, sin reutilizar autoridad de carpetas |
| Existente | `GestionDocumental-Docuarchi.net.vbproj` | Registrar tres nuevos Content, sin nuevo Compile previsto |
| Existente | `tests/ImportacionExcelBehaviorTests.cs` | Harness nativo DOC122 para recuperación contextual; ampliar |
| Existente | `tests/doc122-importacion-excel-behavior.test.cjs` | Compilación/harness existente; reutilizar |
| Existente | `tests/doc119-navigation.test.cjs` | Navegación browser local y permisos; ampliar |
| Existente | `tools/e2e/tests/importacion-excel.spec.cjs` | Suite real existente; añadir describe DOC126 |
| Existente | `tools/e2e/scripts/run-doc110-profile-session.cjs` | Consola, login y runner existente; añadir flags Excel UI |
| Existente | `tools/e2e/tests/support/authenticated-workflow-session.cjs` | Login existente; reutilizar |
| Existente | `tools/e2e/tests/support/doc110-admin-navigation.cjs` | Driver de navegación que observa una respuesta por click; ampliar |
| Existente | `tools/e2e/tests/support/doc125-excel-scope.cjs` | Ejemplo de validación cerrada de efectos; no heredar autorización |
| Existente | `tools/e2e/scripts/support/doc32-e2e-odbc.cjs` | Controles SELECT y huellas actuales; reutilizar |
| Existente | `tools/e2e/scripts/support/doc115-repository-integration.ps1` | Actor nativo ya existente para carrera; reutilizar sin siembra/limpieza |
| Existente | `tests/ImportacionExcelConfirmacionTests.cs` | Generador y actor externo DOC125; reutilizar |
| Existente | `tools/e2e/AGENT-RUNBOOK.md` | Autorización, secretos efímeros y límites; actualizar antes de ejecución |
| Existente | `Doc/Actualizacion/Importacion/Exploracion/06-interfaz-y-menu.md` | Prompt original inmutable |
| Existente | `Doc/Actualizacion/Importacion/Exploracion/carga-excel-csv-tablas-imp-02.md` | Diseño consolidado de las siete fases; antecedente |
| Existente | `Doc/Actualizacion/Importacion/Validacion/doc125-ejecucion-seguimiento-20261124.json` | Aceptación anterior conservada y límites físicos, sin reabrir ventanas |
| Nuevo previsto | `js/gestion/ImportacionExcel/ImportacionExcelApi.js` | Cliente Excel UMD/ES5 con jQuery actual; Content |
| Nuevo previsto | `js/gestion/ImportacionExcel/ImportacionExcelVista.js` | Controlador/vista accesible y estado; Content |
| Nuevo previsto | `Styles/ImportacionExcel.css` | Estilo acotado al host/modal Excel; Content |
| Nuevo previsto | `tests/doc126-importacion-excel-client.test.cjs` | Transporte local con dobles, sin servidor real |
| Nuevo previsto | `tests/doc126-importacion-excel-ui.test.cjs` | Browser local con transporte simulado, claramente identificado |
| Nuevo previsto | `tests/doc126-e2e-reuse.test.cjs` | Política del runner/scope; no autentica |
| Nuevo previsto | `tools/e2e/tests/support/doc126-excel-scope.cjs` | Scope propio de UI lectura/escritura |
| Nuevo previsto | `Doc/Actualizacion/Importacion/Sql/22-preflight-doc126.sql` | Diagnóstico SELECT revisable; sin instalación implícita |
| Nuevo | `Doc/Actualizacion/Importacion/Sql/23-fixtures-da-doc126.sql; 24-fuentes-e2e-ra-doc126.sql; 25-plantillas-e2e-da-doc126.sql` | Fixtures propios para aceptación UI; aplicación manual separada |
| Existente | `tools/e2e/scripts/query-doc32-odbc-fingerprint.ps1` | Reutilizar control SELECT con alias RA y validación optativa de huella del DSN |

## Decisions

### D-01 / RQ-01 — Acceso exclusivo y bootstrap autorizado

Añadir RenderImportacionExcel independiente de RenderPerfilImportacion y RenderImportacionCarpetas. Resolver la autoridad con ImportacionExcelContextAdapter y recursos actuales, sin exigir selección de nivel ni permisos de carpetas/perfiles/fuentes/proveedores. El host sólo expone rutas relativas de aplicación codificadas y referencia opaca. Al abrir, POST AbrirCargaExcel obtiene token y capacidad vigente; Emitir exige JSON POST y no se invoca desde el GET de la página. El permiso de sesión ya cargado no sustituye la consulta vigente del servidor.

Alternativa evaluada: Se reutiliza autoridad Excel; basarse sólo en GA_ADMINISTRAR_CARGAS_IMPORTACION o abrir el administrador de perfiles exigiría permisos ajenos o una caché obsoleta.

Evidencia: ImportacionExcelContextAdapter.Resolver; ImportacionExcelRequestGuard.Emitir/ValidarTransporte; RenderPerfilImportacion; AperturaImportacionExcel.

### D-02 / RQ-02 — Menú común y coexistencia

Reutilizar asegurarMenu/rotularAccesoMenu de ImportacionCarpetas.js: son funciones de DOM independientes del host y permiso de carpetas y el script se incluye siempre en la página. Insertar una sección propia de Excel en el único #doc110-navigation, conservando las secciones operativas y administrativas. El montaje identifica nodos propios, se repite de forma idempotente y destruye sólo listeners/nodos Excel. La disponibilidad de carpetas y administración mantiene sus autoridades actuales. Probar expresamente HTML sin hosts doc110-perfiles/doc111-importacion y coexistencia después de postback parcial.

Alternativa evaluada: La reutilización del constructor ya vigente conserva estilos y teclado del menú; crear otro menú o extraer el módulo completo añadiría una migración que esta fase no necesita.

Evidencia: ImportacionCarpetas.asegurarMenu/rotularAccesoMenu; PerfilImportacionEditor.montar; tests/doc119-navigation.test.cjs.

### D-03 / RQ-03 — Cliente del contrato Excel y transporte cerrado

Crear ImportacionExcelApi como cliente del ASMX y upload actuales. El envelope Excel permite .d y contiene ok/codigo/mensaje/operacionId/datos, sin exigir incidencias de nivel raíz como PerfilImportacionApi. Conservar datos de RespuestaPreparacionExcel incluso cuando ok=false para presentar validación fallida. Usar lista cerrada de operaciones, JSON {solicitud:...} salvo apertura {}, URL de aplicación del mismo origen, X-Importacion-Excel-CSRF y timeout30s. Multipart usa FormData con processData/contentType desactivados para dejar el boundary al navegador. Prohibir retries automáticos, renovación automática de autorización y logs de payload/token; un fallo ambiguo de mutación mantiene incertidumbre.

Alternativa evaluada: Se adapta el patrón de transporte existente sin usar directamente el parser de perfiles, que rechazaría respuestas Excel válidas y perdería las incidencias devueltas en datos.

Evidencia: RespuestaImportacionExcel/RespuestaPreparacionExcel; WebServiceImportacionExcel.Ejecutar; PerfilImportacionApi.envelope; ImportacionExcelRequestGuard.

### D-04 / RQ-04 — Administración de plantillas existente y CAS

Listar por conexión autorizada, término y páginas25; obtener detalle antes de editar o cargar. Implementar formulario de formato, tabla imp_02_, clave, hoja, modo, estado y matriz completa de campos admitidos (origen/destino/tipo, reemplazos y splitter con null preservado). Mantener IDs UInt32 como enteros seguros; no aplicar reglas de IDs string propias de los perfiles. GuardarPlantilla usa ID0/revisión0/huella vacía para alta y revisión/huella exactas para edición. El servidor conserva la autoridad sobre ruta, SQL, campos y compatibilidad histórica; el cliente no permite cadenas de conexión ni SQL. Tras guardar, usar el detalle devuelto y descartar cualquier preparación anterior. Conflicto exige recargar; una respuesta incierta no repite guardar/crear. Si se perdió el ID de alta, una selección explícita en la lista no acredita por sí sola qué alta terminó.

Alternativa evaluada: Se implementa el editor con el servicio DOC122 ya existente; no se reutiliza el editor de perfiles, cuyo modelo y permiso son distintos.

Evidencia: SolicitudGuardarPlantillaExcel; PlantillaExcel/CampoPlantillaExcel; ServicioPlantillasExcel.Listar/Obtener/Guardar/ModoHistorico.

### D-05 / RQ-05 — Recepción única, límites y hojas reales

La selección acepta un único XLS/XLSX compatible con el formato de la plantilla. Consultar límites efectivos de AperturaImportacionExcel; conservar20000filas configuradas y10MB decimales (10000000bytes), con default lector10000. El browser sólo anticipa extensión/tamaño; la detección de contenido, lectura completa y límites de filas corresponden al servidor. IniciarRecepcionExcel obtiene IdCarga/ReferenciaRecepcion; guardar el ID antes del upload. FormData incluye exactamente idCarga,referenciaRecepcion,conexionId,plantillaId y archivo. Usar Hojas devueltas por la recepción, proponer LIBRO_EXCEL si coincide y exigir selección válida. Ningún porcentaje de transferencia se presenta como porcentaje de importación. Ante recepción incierta, consultar el ID sin reupload automático.

Alternativa evaluada: Se conserva la validación completa DOC123/124; leer filas en JavaScript duplicaría la autoridad y aumentaría memoria sin acreditar la elegibilidad física.

Evidencia: InicioRecepcionExcel; ImportacionExcelRequestGuard.LeerMultipart; ServicioRecepcionExcel.Recibir; AperturaLimites.

### D-06 / RQ-06 — Intención y preparación vigentes

Ofrecer sólo el modo acreditado por ConsultarPlantilla: CREAR si el destino es creable, AGREGAR o REEMPLAZAR según los flags históricos; cambiar de modo exige una plantilla compatible o guardarla explícitamente, sin escribir configuración como efecto de un selector. Preparar envía ID, conexión, plantilla, revisión/huella, hoja y modo exactos; si se vuelve a preparar un estado PREPARADA usa PreparacionHashEsperada actual. Cambiar conexión/plantilla/archivo/hoja/modo/campos invalida de inmediato la confirmabilidad y cualquier consentimiento. Para cambiar archivo o plantilla iniciar otra referencia sólo por acción explícita, conservando la anterior. Confirmar requiere estado PREPARADA, ValidacionCompleta y LecturaCompleta, cero incidencias y hashes completos del resultado vigente.

Alternativa evaluada: Se conserva el CAS DOC124 y la intención de la plantilla; un selector de modo no puede modificar flags ni reapropiarse de snapshots publicados.

Evidencia: SolicitudPrepararExcel.PreparacionHashEsperada; ServicioPreparacionExcel.Preparar; ServicioPlantillasExcel.Obtener.

### D-07 / RQ-07 — Vista previa e incidencias acotadas

Mostrar únicamente la muestra devuelta (hasta25filas) e incidencias devueltas (hasta25), con TotalIncidencias e IncidenciasTruncadas para distinguir el total de lo mostrado. Numerar incidencias y conservar Fila/Columna/Hoja/CampoFuente/CampoDestino disponibles; no inventar ubicaciones ausentes. Renderizar texto con textContent, Texto sin perder ceros iniciales, NumeroCanonico como cadena sin Number/parseFloat, EsNulo distinto de vacío/cero y fechas sin cambiar de día por zona horaria. Admitir la serialización de fecha .NET vigente mediante parser cerrado sin eval. No descargar ni renderizar el libro completo en DOM ni insinuar que la muestra es toda la validación.

Alternativa evaluada: Se consume la muestra del servidor, que ya validó el libro completo; introducir paginación ficticia de incidencias ocultaría que sólo existe una muestra.

Evidencia: PreparacionCargaExcel; IncidenciaLecturaExcel/ValorNormalizadoExcel; ServicioPreparacionExcel.Take(25).

### D-08 / RQ-08 — Estado de interacción y respuestas tardías

Modelar vacío, cargando, con incidencias, preparado, confirmando, confirmado, resultado incierto y capacidad cerrada como estados de UI; no escribir esos nombres como nuevos estados DB. Un contador de generación y una intención inmutable vinculan cada callback al host/contexto/selección; descartar respuestas antiguas y destruir handlers al reemplazar host. Una sola petición de mutación por vista; bloquear dobles clicks de forma síncrona. Integrar DOMContentLoaded y PageRequestManager endRequest si existe, sin registrar varias veces. Cerrar/recargar durante una operación no cancela el servidor; conservar referencias y mostrar consulta al reabrir. Busy de Excel se acota a sus controles, conservando las capacidades de carpetas.

Alternativa evaluada: La generación de UI evita aplicar respuestas obsoletas sin suponer que abortar XHR revierte el trabajo del servidor.

Evidencia: PerfilImportacionEditor searchEpoch/viewEpoch; ImportacionCarpetas lifecycle; WebForms ScriptManager y contratos durables DOC125.

### D-09 / RQ-09 — Confirmación explícita y resultado durable

Confirmar toma los tres hashes y el ID de la preparación vigente, congelados antes de enviar. Para REEMPLAZAR, mostrar tabla autorizada, número de filas validadas y advertencia de sustitución del contenido autorizado; requerir aceptación separada por cada intención y enviar ConfirmacionReemplazo=true sólo tras ella. No inventar número de filas eliminadas ni inferir política CONEXION/COLUMNA por nombre de tabla. La aplicación del ámbito, unicidad y transacción permanece en DOC125. Mostrar CONFIRMADA sólo al acreditarlo el servidor con FilasConfirmadas/ConfirmadoUtc; mantener el resultado histórico sin afirmar que la tabla nunca cambió después. FALLIDA y código conocido no se confunden con incertidumbre ni se reintentan automáticamente.

Alternativa evaluada: Se conserva Confirmar como única autoridad transaccional; el cliente no transforma un timeout o una muestra correcta en éxito.

Evidencia: SolicitudConfirmarExcel; ServicioConfirmacionExcel.Identidad/Confirmar; EstadoCargaExcel; writer DOC125.

### D-10 / RQ-10 — Recuperación contextual sin secretos

Añadir ImportacionExcelRequestGuard.ReferenciaRecuperacion siguiendo el patrón existente de carpetas: referencia opaca aleatoria por vínculo servidor de sesión/login/usuario/empresa y recursos RA/DA/módulo. El vínculo usa referencias de host/base/módulo/proveedor, sin contraseñas ni cadenas de conexión. Es un namespace, no autoridad ni CSRF; no exponer identificadores de sesión o conexiones. El renderer la obtiene sin emitir token desde GET. sessionStorage guarda sólo IdCarga y ConexionId bajo ese namespace; memoria mantiene la intención y hashes mientras exista la vista. Guardar referencias antes de upload/confirmar; fallo de storage deja consulta manual por UUID y aviso para anotar el ID. La nueva UI envía X-Importacion-Excel-Contexto con esa referencia opaca; Emitir y ValidarToken contrastan la cabecera opcional antes de emitir/aceptar CSRF. Así un tab antiguo no adopta otra cuenta o módulo con referencias de su contexto anterior. Los consumidores DOC122–125 sin cabecera mantienen su contrato. Abrir siempre contexto/token vigente antes de consultar. ConsultarResultado/ConsultarCargaExcel no mutan. Tras recarga con sólo referencias, mostrar consulta y resultado, sin reconstruir automáticamente la preparación o confirmar a partir de configuración actual. PREPARADA observado durante incertidumbre no acredita rollback; FALLIDA conocida sólo permite un reintento explícito si la intención íntegra original sigue en memoria. CONFIRMADA se recupera aunque la plantilla/archivo/límites actuales ya no sirvan.

Alternativa evaluada: Persistir sólo referencias con espacio contextual evita guardar cookies, token, archivo o snapshots sensibles; consultar no se sustituye por una importación nueva.

Evidencia: ImportacionCarpetasRequestGuard.ReferenciaRecuperacion; ImportacionExcelRequestGuard.Vinculo; ServicioConfirmacionExcel.ConsultarResultado; composición lazy.

### D-11 / RQ-11 — Diálogo accesible y visibilidad

Un único diálogo Excel con role=dialog, aria-modal/labelledby, etiquetas explícitas, errores asociados, región role=status/aria-live y aria-busy acotado. Foco inicial en el primer control útil, Tab/Shift+Tab contenidos, Escape/cerrar con retorno al disparador o toggle vigente si el nodo fue reemplazado. Si otra ventana administrativa está abierta, pedir cerrarla mediante su acción existente antes de abrir Excel; conservar cambios sin guardar y no cerrar trabajo ajeno automáticamente. CSS acotado con max-height relativo al viewport, scroll interno, acciones visibles y scroll horizontal sólo en tablas. Verificar320x568,768x1024,1280x720 y zoom200%; el color o un spinner no son la única señal de estado. Durante actividad cerrar conserva el ID y los estados de consulta, sin ofrecer una pausa inexistente.

Alternativa evaluada: Se utiliza un diálogo propio con los patrones del repositorio y CSS local; no se cambia Bootstrap/jQuery ni se introduce un framework visual.

Evidencia: PerfilImportacionEditor dialog y estilos; tests/doc119-navigation.test.cjs; página Producción Documental.

### D-12 / RQ-12 — Información operativa y salida segura

Mensajes propios por códigos conocidos con texto sencillo; conservar datos estructurados para la UI sin imprimir operacionId, hashes, trazas, rutas, SQL, conexiones ni mensajes crudos de transporte en el producto. Mostrar tabla/clave/hoja/modo y límites porque permiten decidir, y el ID de carga porque permite consultar. Renderizar nombres/valores/incidencias mediante texto, con ubicaciones sólo si existen. Distinguir conflictos de revisión, duplicados, archivo/hoja inválidos, capacidad cerrada y resultado incierto con una acción concreta. No convertir RUTA_ARCHIVO_FUENTE histórica en selector ni mostrarla. No guardar/registrar payloads en consola, archivos, URL, localStorage o evidencias; screenshots/traces/videos reales desactivados por defecto para no retener sesiones/documentos.

Alternativa evaluada: Se usa el contrato saneado del módulo y un mapa de mensajes de UI; la interfaz no muestra detalles técnicos para diagnosticar al usuario.

Evidencia: WebServiceImportacionExcel.Ejecutar; RespuestaImportacionExcel; IncidenciaLecturaExcel; políticas E2E existentes.

### D-13 / RQ-13 — SQL manual y reutilización de instalación

La UI reutiliza permisos, tablas, plantillas y estados DOC121–125; no requiere migración de esquema por diseño. Entregar22-preflight-doc126.sql de sólo SELECT para acreditar instalación/cuentas/fixtures cuando se prepare la aceptación. Las correcciones o nuevos fixtures/permiso exclusivo que el diagnóstico justifique se entregan en archivos nuevos numerados posteriores, con destino RA/DA, IDs explícitos, precondiciones, efectos y SELECT posterior; mantener PENDIENTE_EJECUCION_MANUAL hasta confirmación específica. No reutilizar la delegación SQL de DOC125 como permiso para nuevos archivos ni reaplicar18–21. El scope de UI no instala, siembra, limpia o modifica permisos. Las creaciones/ediciones de plantilla y confirmaciones solicitadas por UI son funcionales autorizadas, no mecanismos de instalación indirecta.

Alternativa evaluada: Se comprueba y conserva la instalación vigente; crear otra conexión o ejecutar SQL desde tests confundiría instalación con aceptación funcional.

Evidencia: Sql/README.md; SQL18–21 y evidencia DOC125; permiso ADMINISTRAR_CARGAS_IMPORTACION ya instalado.

### D-14 / RQ-14 — Aceptación UI con E2E existentes

Ampliar importacion-excel.spec.cjs y el runner DOC110 con --excel-ui-read/--excel-ui-write mutuamente excluyentes y scope DOC126; reutilizar login, recursos, DOC32 SELECT, generador y actor nativo DOC125 y driver de navegación DOC110. La aceptación UI debe clickear menú, seleccionar plantilla/libro/hoja, preparar, consentir y confirmar desde controles reales; llamadas directas ASMX de DOC125 no acreditan esa UI. Separar tests browser locales con route.fulfill de ejecución real contra IIS. El scope liga ambiente/cuentas (propietario con sólo permiso del cargador y restringida), archivo/hash, DLL/assets, destinos/configuración y máximos de efectos. Las credenciales son nuevas y efímeras; las ventanas DOC125 están cerradas y no conceden autorización DOC126. Read sólo abre/lista/consulta; write cubre tres modos, duplicado/rollback, doble click, respuesta ignorada y carrera con actor existente. Reutilizar IDs confirmados anteriores sólo para consulta histórica; los positivos nuevos requieren fixtures explícitos, con CREAR ausente y configuración manual verificada. Conservar todos los datos anteriores, sus huellas completas y los nuevos resultados.

Alternativa evaluada: Se amplía la infraestructura existente con un scope de efectos nuevo; duplicar login/DSN o presentar mocks/IDs históricos como nuevas escrituras UI no acredita los recorridos solicitados.

Evidencia: importacion-excel.spec.cjs; excelProfilePlan; doc125-excel-scope.cjs; doc110-admin-navigation.cjs; DOC32 y DOC115.

### D-15 / RQ-15 — Compatibilidad y entrega verificable

Mantener net461, MySQL5.1.37, MySql.Data6.2.3, servicios y estados existentes; registrar Api.js/Vista.js/CSS como Content en vbproj. Reutilizar harness C# DOC122–125 y suites de navegación/carpetas afectadas, y añadir pruebas locales sólo para los nuevos comportamientos de transporte, UI, seguridad de montaje y scope. Build aislado y pruebas locales no despliegan ni acreditan E2E real. Actualizar cuatro documentos técnicos obligatorios, diseño de fase06, runbook y evidencia por SHA con límites. Rollback de assets y bin mediante respaldo/versión compatible, sin DROP ni borrado de datos/configuración/cargas. No modificar SII/AlmacenamientoDocumentoProduccionDocumental, CSV, permisos arbitrarios ni publicación automática de fuentes; las tablas elegibles conservan su compatibilidad como fuente de índices, sin crear una fuente RA desde el diálogo.

Alternativa evaluada: Se entrega una capa de interacción sobre el backend probado; sustituir infraestructura o añadir nuevas capacidades de importación alteraría las fases y sus contratos.

Evidencia: GestionDocumental-Docuarchi.net.vbproj; suites DOC110/119/120/122/124/125; docs de aceptación y rollback.

## Contratos de interacción

| Acción | Solicitud y resultado existentes |
| --- | --- |
| AbrirCargaExcel | POST {} sin CSRF inicial; devuelve token, conexiones autorizadas, MaxFilasDatos/MaxArchivoMB y PermiteNuevaCarga/CodigoCapacidad. |
| ListarPlantillas / ConsultarPlantilla | ConexionId/Termino/Pagina/TamanoPagina y ConexionId/PlantillaId; selección obtiene revisión, huella, clave, campos y modo vigentes. |
| GuardarPlantilla | SolicitudGuardarPlantillaExcel con CAS; nunca guardar como efecto lateral de seleccionar modo. |
| IniciarRecepcionExcel | ConexionId/PlantillaId/RevisionEsperada/HuellaEsperada; guardar IdCarga antes de subir. |
| Upload multipart | Exactamente cuatro campos más archivo; el browser genera boundary; el token va sólo en la cabecera propia. |
| Preparar | Identidad, revisión/huella, Hoja/Modo y PreparacionHashEsperada cuando corresponda; ok=false puede incluir datos útiles de incidencias. |
| Confirmar | ID, ConexionId, ArchivoHash/ConfiguracionHash/EsquemaHash y consentimiento; una solicitud por intención. |
| ConsultarResultado / ConsultarCargaExcel | ID/ConexionId propios; leer resultado sin preparar, reparar, escribir ni inferir rollback de un PREPARADA observado. |

## Estado de UI y continuidad

Vacío → cargando → con incidencias o preparado → confirmando → confirmado o resultado incierto. Es una máquina de interacción, no nuevos estados DB. PREPARADA/FALLIDA/CONFIRMADA se muestran según el servidor. Un error de lectura conserva el ID; no transforma desconocimiento en fallo conocido. La recarga recupera referencias para consulta, no un formulario confirmable con snapshots reconstruidos. Una edición de plantilla invalida una preparación aún visible. La consulta histórica no depende de listar/editar primero una plantilla hoy inválida.

## Risks / Trade-offs

- Menú compartido y postbacks parciales → prueba de montaje repetido y eliminación sólo de nodos propios; se conserva el constructor actual.
- Resultado de guardar plantilla nueva perdido sin ID devuelto → estado incierto, lista/selección explícitas; no retry ni deducción de éxito por nombre.
- Datos typed/fechas .NET → texto seguro, números canónicos como cadenas y fecha sin conversión a día local.
- sessionStorage bloqueado → aviso, referencia en memoria y consulta manual; no prometer recuperación automática tras recarga.
- Vista cerrada durante confirmación → servidor puede continuar; conservar ID, consultar y mantener cero reintentos.
- Muestra25 y total mayor → mostrar límite y total; no inventar descarga/paginación del resto.
- E2E heredada acredita backend, no UI → nuevos recorridos por controles bajo scope DOC126; no reusar CREAR anterior como destino vacío.
- Cuenta141 con permisos adicionales → no acredita acceso exclusivo; la matriz real requiere una cuenta/perfil que cumpla esa precondición, sin alterarlo desde tests.
- Capacidad física anterior1000 por modo → no extrapolar a20000; conservar configuración y medir únicamente el volumen autorizado.

## Migration Plan

1. Implementar y validar localmente las tareas trazadas; incluir Content y build aislado.
2. Entregar preflight22 SELECT; cualquier adecuación o fixture nuevo tendrá otro SQL y espera aplicación manual específica. No repetir18–21.
3. Preparar despliegue con respaldo y hashes de DLL/assets, y verificar recursos y scopes DOC126 antes de autenticar.
4. Ejecutar sólo las ventanas reales específicamente autorizadas, conservar resultados y comprobar huellas completas mediante SELECT.
5. Actualizar evidencia/gobierno por SHA. Revertir únicamente assets/bin compatibles si procede; conservar configuración y datos.

## Verificación del refinamiento

Revisión técnica de Codex del2026-11-24; no atribuye aprobación humana ni ejecución real DOC126. El inventario y las decisiones se contrastan con fuentes existentes. Las credenciales/instalación/ventanas DOC125 no se reabren por esta fase. La implementación y su aceptación se registrarán después, con evidencias distintas de esta revisión.

## Concreciones verificadas durante implementación

El ASMX conserva sus nueve métodos y DTOs. La cabecera contextual es opcional para consumidores anteriores y obligatoria en el cliente nuevo. El control DOC32 admite alias RA y, cuando el scope lo aporta, contrasta la huella del DSN con el resolver existente antes de abrirlo. El actor DOC125 acepta el scope DOC126 explícito sin heredar su autorización. SQL23–25 reutilizan la preparación administrativa conocida con nombres nuevos; los históricos quedan intactos. El ASPX invoca el renderer opcional por reflexión para que publicar Content antes de la DLL no rompa la página con el binario anterior; con la DLL compatible el permiso vigente es la única autoridad funcional. Los archivos nuevos de prueba y SQL no son Compile/Content de producto. La geometría del diálogo sigue el viewport visual y el zoom; teclado alcanza acciones dentro del scroll de su cuerpo.

### D-16 / RQ-16 — Crear plantilla desde Excel

Asistente aditivo sin plantilla: analizar XLS/XLSX en operación multipart propia, con autoridad/contexto/CSRF y límites actuales. Reutilizar LectorArchivoExcel y sus inspectores, con fila 1 y recorrido completo para proponer hasta 256 campos, tipos conservadores y muestra 25. Para tablas existentes se consultan sus tipos físicos mediante SELECT. No escribir archivos durables, plantillas o ejecuciones en el análisis. Guardar mediante el CAS actual y recibir sólo por acción explícita con el File retenido en memoria del navegador; contrastar hash antes de preparar. No introducir PlantillaId=0 en recepción, estados DB ni migración SQL. Cambiar archivo/hoja/contexto invalida propuestas; una recarga exige seleccionar nuevamente. Se conserva edición manual y el asistente se limita al alta de plantillas.

Alternativa evaluada: una recepción durable previa a la plantilla requiere caducidades, referencias y recuperación adicionales; se difiere para reducir regresión. El coste aceptado es enviar el archivo para análisis y luego por recepción habitual (y volver a analizar al cambiar hoja). No hay una segunda confirmación ni un reintento automático. Un fallo de análisis no es resultado durable incierto.

Inventario adicional antes de implementación:

| Estado | Archivo | Responsabilidad |
| --- | --- | --- |
| Nuevo | `Services/ProduccionDocumental/ImportacionExcel/ServicioAnalisisPlantillaExcel.vb` | Propuesta secuencial sin plantilla, con lector existente |
| Nuevo | `generic_control/ImportacionExcelAnalisis.ashx` | Entrada multipart independiente |
| Nuevo | `generic_control/ImportacionExcelAnalisis.ashx.vb` | Autoridad, guardas y consulta opcional de destino |
| Existente | `DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb` | Respuesta aditiva de análisis, sin cambiar DTOs previos |
| Existente | `tests/doc122-importacion-excel-behavior.test.cjs` | Compilar analizador y entrada en harness actual |
| Existente | `tests/ImportacionExcelLecturaTests.cs` | Reutilizar libros/generador y ampliar casos |

Api/Vista/host/CSS y proyecto ya inventariados reciben cambios acotados. No modificar la lógica de ServicioLecturaExcel, Recepcion, Preparacion, Confirmacion ni repositorios para este asistente. No atribuir aceptación física al transporte simulado; 8.8 requiere una ventana que incluya explícitamente el alta de plantilla.

## Preparación de aceptación del asistente

El runner incorpora --excel-ui-assistant con DOC126_ASSISTANT_E2E_AUTHORIZED independiente y scope exacto de una plantilla/dos campos/1000 filas/cero borrados. El bloqueo de Guardar permanece para lectura y matriz anterior. El helper de locks compartido conserva DOC125 por defecto y admite DOC126 sólo por parámetro explícito desde su adaptador. La DLL validada se publicó con respaldo, WSDL y rechazo anónimo; no equivale a aceptación autenticada ni cierra 8.8. Evidencia de despliegue y preparación en Validacion/doc126-despliegue-e2e-preparada-20261124.md/JSON.

## Corrección del video de creación — tarea 8.9

El video del usuario muestra análisis de 12759 filas y elección de NROORDEN, pero la tabla queda en el prefijo imp_02_; el guardado se detiene en el cliente con un mensaje genérico. Se propone un nombre editable desde el archivo al crear, normalizando acentos/separadores y acotando a 64 caracteres. Sólo se reemplaza el prefijo vacío o una sugerencia anterior sin editar; se preservan nombres manuales y modos agregar/reemplazar. Tabla incompleta y clave no mapeada tienen mensaje propio, aria-invalid y foco; el servidor conserva su validación y autoridad. Quitar queda en una línea. Assets doc126-3, sin cambios VB, contratos, SQL o DLL por esta corrección. Evidencia en Validacion/doc126-correccion-video-crear-plantilla-20261124.md/JSON.
