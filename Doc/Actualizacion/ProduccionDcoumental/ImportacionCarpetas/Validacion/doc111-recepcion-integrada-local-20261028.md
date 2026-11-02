# DOC-111 — Recepción autorizada, repetición y reconciliación local

Fecha: 2026-10-28. Base HEAD `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios previos. Se implementan 8.6–8.10: progreso **71/130**. Este corte conecta el staging previo al servicio, repositorio único y handler; las transacciones MySQL y el pipeline HTTP reales continúan **NO_EJECUTADO**.

## Implementación

`ServicioRecepcionImportacion` ofrece la capacidad de recibir/reconciliar mediante un constructor específico y el puerto IRecepcionArchivosImportacionRepository. La confirmación del plan anterior conserva su composición. El puerto nuevo lo implementa MySqlImportacionCarpetasRepository; no hay otra familia de tablas o repositorios.

La recepción resuelve identidad vigente y referencia persistida del lote por usuario/empresa, reconstruye nivel/sello y autoriza gabinete/carga. Carga el archivo mediante archivo → raíz → lote, comprueba identidad, estado/revisión y bytes antes de crear staging. No usa nombre/MIME/ruta del cliente ni selecciona otro perfil: conserva el snapshot del lote confirmado. Revalida autoridad antes del registro final; una revocación durante la transferencia conserva evidencia sin declarar recepción confirmada.

El repositorio incorpora ObtenerContextoLote, ObtenerArchivo, ObtenerIntentoRecepcion, reserva de intentos RECEPCION, ConfirmarRecepcion y RegistrarFalloRecepcion. Los getters comprueban propiedad, ámbito/destino, documento de origen, identidad cliente/raíz, bytes y hash de ruta. Se comprueba la huella del snapshot del lote. La reserva admite solamente RECEPCION/ARCHIVO; las reclamaciones documentales y su tabla completa de transiciones siguen pendientes de 10.1–10.2.

La reserva usa una transacción breve con bloqueo de lote y archivo, revisión, estado y búsqueda de huella de comando. La huella incluye contexto/recurso/clave; el token y ejecutor se generan en servidor. Una solicitud repetida devuelve el intento previo; un intento activo o incierto no se sustituye por antigüedad. Solo quien obtiene su token nuevo inicia el stream. No mantiene transacción durante escritura o validación del PDF.

ConfirmarRecepcion comprueba recurso/token/estado y referencia/bytes/hash verificados; actualiza archivo RECIBIDO, intento CONFIRMADO/evidencia, evento y revisiones en la misma transacción. No actualiza estado_efecto a COMPLETADO ni crea relaciones documentales. Repetir la confirmación con la misma evidencia conserva el resultado. Registrar un fallo posterior no degrada una recepción ya confirmada.

El modelo de resultado del intento añade una propiedad Recepcion tipada y omitida cuando es nula, conservando documentos anteriores. El puerto de staging añade VerificarReenvio: verifica el completo retenido y compara el stream entrante por tamaño/SHA-256 sin escribir otra parte. Contenido distinto no sustituye archivo, evidencia o IDs.

La reconciliación interna verifica disco/fila/intento con autoridad vigente y puede registrar el completo que quedó antes de un fallo de metadatos. Una parte aislada, completo ausente o contenido alterado no autoriza nueva escritura ni devuelve éxito solo por existir una fila RECIBIDO. El completo producido o un fallo incierto retienen la reclamación; un rechazo demostrado antes de completo cierra el intento. Reintentar un rechazo exige otra clave explícita. RECEPCION_OCUPADA demuestra contención antes de iniciar escritura y cierra el intento sin dejar un escritor incierto.

El handler previsto ImportacionCarpetasUpload.ashx[.vb] usa IRequiresSessionState, autoridad/guard multipart previos a Form/Files y delegación al servicio. El sobre JSON no incluye stagingRef, token o rutas. Se incluyen Content/Compile del proyecto y fuente en el arnés existente. No hay login, runner, endpoint de efecto documental o configuración E2E nuevos.

Habilitar la composición productiva requiere las siete claves generales de límites y las dos claves positivas `Doc111.Staging.MaximoPaginasPdf` / `Doc111.Staging.MaximoObjetosPdf`. No se escribieron valores, permisos o Web.config. Con presupuestos ausentes la capacidad queda indisponible.

## Verificaciones locales

| Comando | Resultado / límite |
|---|---|
| `node --test tests/doc110-perfil-importacion-behavior.test.cjs` | PASS. Compila y ejecuta código VB real con .NET Framework y Option Compare Text del proyecto, conservando familias DOC-110 y DOC-111. |
| `msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /p:OutputPath=.tmp/doc111-recepcion-build/bin/ /p:IntermediateOutputPath=.tmp/doc111-recepcion-build/obj/ /nologo /verbosity:quiet /consoleLoggerParameters:ErrorsOnly /fileLogger /fileLoggerParameters:LogFile=.tmp/doc111-recepcion-build.log` | PASS, exit 0. Compilación completa real, salidas/intermedios separados; sin despliegue o sustitución de DLL del sitio. Advertencias legacy conservadas. |
| `node --test tests/doc111-importacion-carpetas-client.test.cjs tools/e2e/tests/doc88-production-document-upload-policy.test.cjs` | PASS: 15 pruebas locales de inventario/cliente y política de carga individual; sin escenario autenticado real. |
| `openspec.cmd validate doc-111-produccion-importacion-carga-carpeta --strict` | PASS estructural; no acredita transacciones/HTTP reales o aceptación del conjunto. |
| `git diff --check` | PASS, con aviso LF/CRLF del reporte OPSXJ previamente modificado. |

Pruebas del servicio: staging real sobre PDFs/streams sintéticos y **doble explícito de persistencia en memoria**. El doble reutiliza guards reales del repositorio y simula estado/errores; no acredita SQL, aislamiento, commit, rollback o carrera MySQL.

- Permiso de importar/gabinete revocado, nivel movido, lote/archivo ajeno, archivo ineligible o lote detenido: rechazo antes de leer el stream o crear staging.
- Recepción válida: referencia y contenido verificados, un intento y resultado RECIBIDO con estado_efecto NO_INICIADO.
- Reenvío idéntico: mismo contenido/resultado, sin nueva reserva, completo o commit simulado. Reenvío distinto: CONTENIDO_DISTINTO sin alterar evidencia previa.
- Completo ausente con fila recibida: incertidumbre y cero bytes del nuevo stream consumidos, sin replay.
- Fallo simulado antes del commit de metadatos: completo retenido e intento incierto; reconciliación registra la misma evidencia sin transferir otra vez.
- Fallo simulado de respuesta después del commit: conserva recepción positiva; reconciliación devuelve el mismo resultado, sin duplicación.
- Truncamiento: rechazo conocido, sin completo. La misma clave no reutiliza el intento terminal; otra clave explícita puede iniciar un intento nuevo bajo cuotas.
- Revocación durante el stream: no confirma recepción; conserva completo, que después se reconcilia con autoridad restaurada en el doble.
- Dos solicitudes locales del mismo comando: la primera se pausa dentro del stream; la segunda no escribe ni cambia el intento en vuelo. Una sola reserva y confirmación simuladas. El intento lleva fecha antigua para comprobar que su antigüedad no concede escritura.
- Contención física antes de iniciar escritura: cero bytes leídos, intento rechazado sin escritor incierto; otra clave explícita puede recuperar tras liberar el bloqueo.

Pruebas del handler: HttpContext/SimpleWorkerRequest sintéticos, guard multipart y servicio/staging reales con doble de persistencia. Se procesa y captura el JSON del handler sin servidor HTTP.

- Sesión ausente, permiso revocado, token ausente y origen ajeno: rechazo antes de leer multipart o invocar la fábrica del servicio.
- Campo extra de identidad: rechazo sin llamar al servicio.
- Nombre y MIME cliente discordantes: no deciden el destino/formato; se valida el PDF contra el archivo persistido esperado.
- Respuesta de éxito 200, JSON, redirect de formularios suprimido, separación RECIBIDO/NO_INICIADO; sin token, stagingRef o rutas en el cuerpo.
- Fábrica productiva sin presupuestos PDF: CAPACIDAD_NO_DISPONIBLE antes de crear repositorio/conexión.

Pruebas de registros/guards del repositorio: IDataRecord sintético para el lector real de archivo esperado; identidad de raíz, bytes o hash de ruta alterados dejan indisponibilidad. Los guards comprueban revisión, detención, reclamación incierta, fase, token, tamaño y forma de evidencia; confirmar una evidencia distinta para un recurso ya recibido produce conflicto. No se ejecutan sentencias SQL mediante estos casos.

## Pendientes reales

Se revisó que la actualización de archivo, intento y evento utiliza la transacción de Operar; se añadieron puntos internos de observación de fallo. **No se ejecutaron** commit/rollback real de recepción, fallo intermedio del evento, dos conexiones concurrentes, agotamiento real de disco o controles SELECT contra el ambiente. Esa validación requiere autorización específica de ambiente/cuentas/recursos, sin heredar el permiso de corridas anteriores de plan o publicación.

Tampoco se ejecutaron IIS/HTTP autenticado, emisión del token desde la UI, carga desde navegador, REC-01/02 o efectos documentales. La reconciliación es interna del servicio; su integración visual/consulta y las acciones del coordinador permanecen pendientes. La tabla completa de transiciones, reclamación documental, adaptadores legacy, seguimiento, interfaz y mantenimiento no se declaran implementados por completar recepción.

Las pruebas escriben solo archivos sintéticos en directorios temporales únicos, comprobados antes de limpiarlos. No se consultaron bases reales, aplicó DDL/DML, cambiaron cuentas/perfiles, guardaron secretos o alteraron carga individual/SII. Este corte acredita código y comportamiento local; no cierra DOC-111 ni declara terminada su validación real.
