# Diseño 02 — Contratos de operación

Estado vigente: revisión DOC-112 del 2026-11-02 del contrato ya implementado en WebForms/ASMX/ASHX. Secciones 1/2/5 y referencias vigentes siguientes prevalecen sobre cortes locales anteriores; estos conservan fecha y procedencia. La [matriz final DOC-111](../Validacion/doc111-matriz-final-20261102.md) delimita evidencia real/local/no ejecutada. Esta revisión no ejecuta el producto.

## 1. Transporte, autoridad y respuesta vigentes

Entradas operativas: POST JSON en `webservice/WebServiceImportacionCarpetas.asmx/<operacion>` y POST multipart en `generic_control/ImportacionCarpetasUpload.ashx`. Ambos ya existen. JSON usa `{ "solicitud": { ... } }` y respuesta ASMX bajo `d`; el handler devuelve directamente el sobre lógico. Se reutilizan sesión y autenticación existentes. No se exponen adaptadores como endpoints.

`ImportacionCarpetasRequestGuard` comprueba contexto servidor con facultad de importar, usuario/empresa de sesión, POST y longitud positiva, tipo de contenido, vínculo del token a sesión/usuario/empresa y cabecera `X-Importacion-Carpetas-CSRF`. Comprueba Origin cuando está presente y rechaza `Sec-Fetch-Site: cross-site`. JSON se lee con UTF-8 estricto y esquema cerrado; multipart solo permite loteId, archivoId, claveIntento, sin duplicados, y un archivo. El cliente actual envía su parte como `file`; el guard exige un archivo pero no valida el nombre de esa parte como campo adicional. No se envían identidad autoritativa, permisos, SQL, ruta física o destino final desde el cliente.

La autoridad administrativa DOC-110 usa su propio contexto/guard; importar no exige ni concede administrar. Cada servicio recompone el destino por nivel fijado y autoriza recursos para su fase. La matriz final DOC-111 RQ-02 distingue operaciones reales autorizadas de revocación/combinaciones probadas localmente; no acredita revocación real en esas corridas.

`RespuestaPerfilImportacion` contiene exactamente ok, codigo, mensaje, operacionId, datos e incidencias. Las incidencias de ese sobre son `IncidenciaPerfilImportacion` (Campo, Codigo, Mensaje); las incidencias por raíz/archivo de preparación están en el resultado tipado, con su ámbito y correlación, no añadiendo campos al tipo del sobre. Ejemplo sintético, sin credenciales:

```json
{
  "ok": false,
  "codigo": "CONFLICTO_REVISION",
  "mensaje": "El plan o sus recursos cambiaron. Consulte su estado antes de continuar.",
  "operacionId": "11111111111111111111111111111111",
  "datos": null,
  "incidencias": []
}
```

IDs y bytes viajan como cadenas canónicas, revisiones/contadores acotados como enteros. Auditoría usa UTC; fechas documentales conservan calendario según contrato 05. Los DTO y modelos son la fuente de nombres y mayúsculas; no se normalizan arbitrariamente propiedades serializadas de modelos internos que forman parte de un resultado.

La función `WebServiceImportacionCarpetas.EstadoHttp`, compartida por el handler, aplica este mapeo. No se ofrece un mapeo HTTP distinto de la implementación:

| HTTP | codigo |
|---|---|
| 200 | ok=True. No implica por sí mismo documento incorporado. |
| 401 | SESION_REQUERIDA |
| 403 | ACCESO_DENEGADO, SOLICITUD_NO_VERIFICADA |
| 409 | CONFLICTO_REVISION, CODIGO_PERFIL_EXISTENTE, RESULTADO_INCIERTO, OPERACION_EN_CURSO, CONTENIDO_DISTINTO |
| 413 | LIMITE_EXCEDIDO |
| 415 | FORMATO_NO_SOPORTADO |
| 422 | ARCHIVO_RECHAZADO |
| 429 | RECEPCION_OCUPADA |
| 503 | CAPACIDAD_NO_DISPONIBLE, ESQUEMA_FUENTES_INCOMPLETO |
| 500 | ERROR_INTERNO |
| 400 | Otros códigos de error no enumerados por EstadoHttp. |

Se deshabilitan redirección Forms Authentication y errores IIS personalizados en la respuesta controlada; se indica no cachear. Una respuesta perdida exige consulta, no replay ciego. Los mensajes públicos no transportan SQL, credenciales, tokens, rutas ni evidencia privada del adaptador. El ASMX clasifica una excepción inesperada como ERROR_INTERNO con instrucción de consultar; recepción usa RESULTADO_INCIERTO cuando no puede comprobarla. Ninguna de esas etiquetas demuestra ausencia de efectos.

## 2. Operaciones y propietarios vigentes

Fuente: firmas de `WebServiceImportacionCarpetas.asmx.vb` y clases de `ImportacionCarpetasDtos.vb`. Todas devuelven RespuestaPerfilImportacion; la última columna identifica `datos`. La tabla enumera las doce operaciones operativas; los métodos administrativos DOC-110 de la misma fachada conservan su contrato separado.

| Operación / propietario | Solicitud y campos exactos | datos y efecto permitido |
|---|---|---|
| AbrirImportacion / 06 | AbrirImportacionSolicitud: nivelRef | AperturaImportacionResultado: destino/ruta/límites y página de publicaciones compatibles; sin efectos documentales. |
| ListarPerfilesImportacion / 06 | ListarPerfilesImportacionSolicitud: nivelRef, pagina, tamanoPagina | PaginaPerfilesImportacionOperativa; puedeHaberMas preserva paginación aunque se filtren incompatibles. |
| PrepararPlan / 06 | PrepararPlanImportacionSolicitud: nivelRef, versionId, raices, archivos | PreparacionImportacionResultado: planId, revision, planHash, expiraUtc, nivelRef, snapshot, resultado. Guarda plan; no recibe bytes ni crea expediente. |
| ConfirmarPlan / 07-B | ConfirmarPlanImportacionSolicitud: planId, revisionEsperada, planHashEsperado, claveConfirmacion | ConfirmacionImportacionResultado: loteId, planId, nivelRef, estado, revision, totalRaices, totalArchivos, totalBytes. Confirma inventario/snapshot, no ejecuta efectos documentales. |
| ConsultarConfirmacionLote / 11 | ConsultarConfirmacionImportacionSolicitud: planId | ConsultaConfirmacionImportacionResultado: planId, confirmada, loteId, nivelRef. Lectura para recuperar respuesta de confirmación. |
| ConsultarLote / 11 | ConsultarLoteImportacionSolicitud: loteId | EstadoLoteImportacionResultado: estado/revisión, política, totales, actividad/incertidumbre y acciones permitidas; sin avance. |
| ConsultarRecursosLote / 11 | ConsultarRecursosImportacionSolicitud: loteId heredado, tipo, pagina, tamanoPagina, raizId, estado, estadoRecepcion | PaginaConsultaImportacionResultado con revisionLectura, totalOriginal/Filtrado y elementos; solo lectura autorizada. |
| ExportarPaginaLote / 11 | ConsultarRecursosImportacionSolicitud: loteId heredado, tipo, pagina, tamanoPagina, raizId, estado, estadoRecepcion | ExportacionPaginaImportacionResultado: pagina, nombreArchivo, tipoContenido, contenido CSV protegido. No exportación total implícita ni descarga de originales. |
| AvanzarLote / 10 | AvanzarLoteImportacionSolicitud: loteId, recursoId, fase, revisionEsperada, claveOperacion | AvanceLoteImportacionResultado con fase/estado y IDs públicos conocidos. Una fase explícita: CREAR_EXPEDIENTE, MATERIALIZAR_VALORES o ALMACENAR. No encadena efectos. |
| SolicitarDetencion / 10 | DetenerLoteImportacionSolicitud: loteId, revisionEsperada | DetencionLoteImportacionResultado: loteId, estado, revision, detencionSolicitada. Sin clave adicional, rollback o aborto garantizado de llamada en vuelo. |
| ReconciliarRecurso / 10 | ReconciliarRecursoImportacionSolicitud: loteId, intentoId, revisionEsperada | ReconciliacionRecursoImportacionResultado: resuelta y proyección heredada de avance. Verifica/persiste evidencia de un intento, sin invocar otra creación/almacenamiento. |
| CerrarReservaSinInicio / 10 | ReconciliarRecursoImportacionSolicitud: loteId, intentoId, revisionEsperada | ReconciliacionRecursoImportacionResultado. Cierra solo reserva ALMACENAR sin inicio probado, bajo el bloqueo común con inicio; sin replay del escritor. |

Recepción / 07-B: el handler recibe `RecibirArchivoImportacionSolicitud` mediante tres campos multipart loteId, archivoId, claveIntento y un archivo; devuelve `RecepcionArchivoImportacionResultado` con loteId, archivoId, estadoRecepcion, estadoEfecto, bytesRecibidos, contenidoHash y revision. Los bytes se consumen por stream, no JSON; RECIBIDO no significa almacenado/incorporado.

Equivalencias de la propuesta histórica: Listar/ObtenerPerfil operativo se concreta en ListarPerfilesImportacion y el snapshot de preparación; no usar administración para seleccionar. ConsultarArchivos/Raices se concreta en ConsultarRecursosLote por tipo. ExportarResultados es ExportarPaginaLote. No hay ListarLotes público: se consulta un lote conocido y autorizado. Cancelación se concreta en SolicitarDetencion, sin endpoint CancelarLote. PlanRepositorio/OperacionRepositorio son capacidades del repositorio único, no implementaciones paralelas.

Campos de manifiesto: cada raíz lleva seleccionId, idCliente, nombre; cada archivo lleva idCliente, raizIdCliente, nombre, rutaRelativa, bytes, ultimaModificacionLocal. El inventario confirmado se conserva incluso con rechazados. Paginación y límites se toman de los resultados/política servidor; no se anuncia soporte de fragmentos, subida masiva en una petición ni continuidad con navegador cerrado.

La propiedad técnica de persistencia sigue 07-A → 06 → 07-B: repositorio/esquema antes de cálculo/persistencia del plan y confirmación/recepción. 08/09 son puertos internos de efectos; 10 coordina y 11 consulta. Esta secuencia explica dependencias ya implementadas, no trabajo nuevo DOC-112.


## 3. Apertura desde Producción

`AbrirImportacion(nivelRef)` comprueba: identidad, ámbito derivado, existencia, elegibilidad del nivel y capacidad de crear/importar. Devuelve nivelRef/ruta, referencias de clasificación contextual si aplican, versiones de catálogo relevantes y permisos por acción. No crea niveles.

La ruta de display no identifica el nivel. Si no hay sello de revisión legacy, conservar en el plan una huella de la cadena de ancestros/ámbito relevante y reconsultar antes de efectos. No mantener un token duradero que reemplace revalidación de permisos. Otra pestaña puede navegar sin modificar el contexto del plan.

Precisión implementada en DOC-111: ResolverDestino autoriza el nivel antes de construir la ruta. El sello, esquema 1, incorpora empresa, identidad física de origen RA y la cadena ordenada de IDs/padres/nombres/propietarios/estados. Abrir/preparar/confirmar requiere en nivel compartido agregar_expediente y carga_archivo; crear usa agregar_expediente y recibir/almacenar usa carga_archivo. La propiedad del nivel conserva las capacidades del flujo existente. AutorizarRecursos reconstruye el destino y verifica gabinete DA, relación de identidad, ALMACENA_IMAGEN y las opciones documentales existentes. Estas operaciones son componentes internos: la fachada debe resolver autoridad vigente en cada llamada y aún no se declara integrada por existir estos métodos.

## 4. Preparación y canonización

### Referencia vigente de configuración y valores — DOC-112

SnapshotPerfilImportacionOperativa conserva PerfilId, VersionId, NumeroVersion, Huella, Configuracion y Reglas desde publicación autorizada, no un borrador mutable. Se serializa mediante `Services/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionJson.vb`: JSON cerrado, ASCII escapado, profundidad máxima 16 y máximo 262144 caracteres; rechaza miembros desconocidos, duplicados y tipos incompatibles. Ese máximo no equivale a bytes de upload ni a una promesa de volumen.

Se conservan [fuentes de expediente](05-contrato-fuentes-expediente.md) y [coincidencias/disponibilidad](06-coincidencias-disponibilidad.md). PREFLIGHT fija lo resoluble sin crear expediente. TRAS_EXPEDIENTE conserva fuente/tipo y propiedad pendiente explícitos para IDs/consecutivos aún inexistentes; no usa cero ficticio ni crea durante preview. `CoordinadorImportacionCarpetas.Valores.vb` resuelve pendientes desde identidad/valores de creación comprobados y persiste antes de almacenar mediante el parcial `.Valores.vb` del repositorio único. Si ValoresEfectivosFijados=True devuelve el checkpoint, incluso vacío, sin consultar otra vez proveedores. Las fechas generadas conservan su valor de creación; auditoría UTC no sustituye fecha documental. Una incompatibilidad posterior conserva expediente y bloquea el archivo, sin recrear.



### Corte local DOC-111, 2026-10-28

`AbrirImportacion` usa el dispatcher operativo del ASMX y devuelve destino/ruta/sello resueltos de nuevo, límites servidor y primera página de publicaciones compatibles. `ListarPerfilesImportacion` recibe únicamente `nivelRef`, `pagina` y `tamanoPagina` (1–50) y revalida el destino. `paginaPerfiles` declara página, tamaño, `puedeHaberMas` y publicaciones; `perfiles` conserva la lista inicial. `puedeHaberMas` se calcula sobre la página del origen antes de filtrar incompatibles: una página vacía filtrada no acredita fin del catálogo. No se publican totales ficticios.

Corrección del usuario, 2026-10-30: la composición no exige appSettings `Doc111.*`. Usa la capacidad de petición ASP.NET, el límite JSON y la vigencia de sesión existentes; las restricciones funcionales de formato/tamaño se toman del perfil publicado. No fija una cuota funcional acumulada del lote ni nuevos cupos PDF. La limpieza sigue deshabilitada sin política interna explícita. Los cortes históricos que exigen claves Doc111 de activación quedan sustituidos por esta decisión; IIS y Web.config no se modifican.

`ServicioPreparacionImportacion.CalcularPlan` calcula metadatos sin persistencia ni efectos documentales. Resuelve campos de raíz y archivo usando el motor existente, validación de catálogos, límite de código de 45 caracteres, extensiones/cuotas de perfil y dependencias tipadas. Los valores de proveedor resueltos conservan procedencia `proveedorId:versionId:propiedad`; una clave futura conserva dependencia sin consultar filas de negocio. No se anuncian bytes recibidos o formato validado por este cálculo.

`PrepararPlan` usa el dispatcher operativo, calcula el resultado y fija creación/vencimiento UTC a segundos según la política servidor. Delega `GuardarPlan` al repositorio único y devuelve su identidad/revisión/huella, snapshot y resultado persistidos. Puede guardar un plan bloqueado para revisión; no confirma el lote. Esquema o comparación no comprobables producen incidencia global `CAPACIDAD_NO_DISPONIBLE` y `PuedeConfirmar=False`. La evaluación integrada tiene pruebas locales por EXPEDIENTE/LOTE/parcial; la validación MySQL/HTTP real sigue pendiente.

La consulta preventiva está en `MySqlImportacionCarpetasRepository.ComprobarCoincidencias`: código, empresa y área, incluyendo todos los volúmenes. Comprueba metadatos del origen, representabilidad sin pérdida y duplicados dentro del inventario usando la collation de CODIGO_UNICO. Detectar un candidato bajo BLOQUEAR bloquea la raíz; varios candidatos son ambiguos. Un candidato bajo REUTILIZAR_VALIDADO devuelve `REUTILIZACION_NO_DISPONIBLE`: aún no se habilita reutilización ni se presenta una comparación de código como identidad completa. Véase contrato 06 para alcance y límites.

`ConfirmarPlan` recibe únicamente planId, revisión, huella y clave. El servicio resuelve la referencia persistida filtrada por usuario/empresa, reconstruye y autoriza destino/gabinete, carga el plan íntegro y distingue nueva confirmación de repetición. Para un plan nuevo revalida vigencia/publicación exacta, cuotas y coincidencias de unidades elegibles; el repositorio conserva el control transaccional final. Para un plan ya CONFIRMADO conserva autoridad/recursos actuales, pero no vuelve a aplicar vencimiento o disponibilidad del perfil como si fuera una operación nueva. La respuesta contiene loteId/planId/nivelRef, estado/revisión y totales originales, sin rutas físicas, clave ni hash de solicitud interno. Recepción y efectos documentales permanecen en sus fases posteriores.

Primera implementación: una solicitud completa de manifiesto, limitada por cantidad/bytes configurados y límites HTTP. Si el volumen aprobado requiere fragmentación, debe añadirse un contrato explícito de preparación por fragmentos con completitud; no implementarlo parcialmente ni anunciar una capacidad inexistente. La configuración de límites es requisito para habilitar la capacidad.

```json
{
  "nivelRef": "referencia-verificada-del-arbol",
  "versionId": "uid-publicado",
  "raices": [{"idCliente":"r1","nombre":"1246566"}],
  "archivos": [{
    "idCliente":"a1","raizIdCliente":"r1",
    "nombre":"1246566-125-OrdenCompra.pdf",
    "rutaRelativa":"1246566/1246566-125-OrdenCompra.pdf",
    "bytes":"1450020","ultimaModificacionLocal":"2026-10-19T10:00:00Z"
  }]
}
```

Comparar nombre con último segmento, raíz con primer segmento; aceptar únicamente rutas relativas normalizadas con `/`, rechazando absolutos, `..`, vacíos ambiguos y separadores peligrosos. Las raíces tienen identidad independiente de nombre. Canonización ordena por IDs estables, claves de objetos según esquema, representación estricta de números/cadenas y reglas de Unicode documentadas. No aplicar cambios destructivos al nombre original. Reordenar una tabla visual no cambia identidad; cambiar inventario/configuración genera nueva preparación/planId.

Precisión de persistencia DOC-111: la ruta usada para detectar colisiones y calcular ruta_hash se normaliza a Unicode NFC, sin convertir mayúsculas/minúsculas; el documento de origen conserva la ruta original. Se conservan correlaciones ASCII de hasta 64 caracteres (letras, dígitos, guion y guion bajo). En ResultadoPreparacionImportacion, los elementos usan Ambito=RAIZ sin ArchivoIdCliente, o Ambito=ARCHIVO con raíz/archivo existentes. Las fechas persistidas son UTC ISO con Z y precisión de segundos compatible con DATETIME de MySQL 5.1. Los valores diferidos permanecen sin inventar la identidad de un expediente.

Resultado incluye:

- `planId`, `revision`, `planHash`, `expiraUtc`, versión/configuración efectiva y nivel validado.
- Política de integridad y ámbito configurado: EXPEDIENTE o LOTE para modo completo, NO_APLICA para parcial; nunca un default implícito.
- `puedeConfirmar`, `erroresGlobales`, `unidadesElegibles`, `unidadesBloqueadas` y totales originales.
- Cada raíz: nombre visible propuesto, fuentes del código/fechas, estado y conflictos.
- Cada archivo: valores resueltos, valores diferidos con fuente/etapa/tipo, tipología e incidencias. No bytes ni supuesta validación de contenido.

En parcial, una raíz sin ningún archivo elegible no crea expediente vacío. Errores globales bloquean siempre. En completo por EXPEDIENTE, una raíz sin errores previos puede ser elegible aunque otra esté bloqueada; conservar todas las incidencias y el inventario. En completo por LOTE, cualquier error impeditivo de un archivo bloquea la confirmación global. Tras confirmar, ejecutar solo cuando todos los archivos de la unidad exigida estén recibidos y validados. `puedeConfirmar` no significa que los archivos ya estén recibidos.

## 5. Confirmación e idempotencia

Entrada JSON sintética para ConfirmarPlan (referencias ilustrativas, no ejecutar):

```json
{
  "solicitud": {
    "planId": "1001",
    "revisionEsperada": 1,
    "planHashEsperado": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
    "claveConfirmacion": "confirmacion-ejemplo-01"
  }
}
```

Los cuatro campos coinciden con ConfirmarPlanImportacionSolicitud. El hash esperado procede de PrepararPlan y no se calcula sobre una edición local de la vista; el algoritmo y la identidad estable ya están definidos en CanonizadorManifiestoImportacion y el repositorio. El servidor recupera el plan/snapshot, revalida autoridad/destino/disponibilidad y compara revisión/huella. Cambiar contenido requiere otra preparación.

La transacción de ConfirmarPlan del repositorio único confirma lote, raíces, archivos y plan con restricciones únicas por plan y por ámbito/solicitante/clave. Repetición idéntica autorizada devuelve el lote previo; mismo identificador o clave con contenido incompatible produce conflicto, no otro lote. Una revisión obsoleta se rechaza como CONFLICTO_REVISION salvo la repetición confirmada compatible que ya reconoce el repositorio. Si se pierde la respuesta, ConsultarConfirmacionLote consulta por planId antes de confirmar otra vez.

ConfirmacionImportacionResultado devuelve loteId, planId, nivelRef, estado, revision y totales originales; no incluye mapas de raíces/archivos. Estos se recuperan con ConsultarRecursosLote (idCliente/recursoId/raizId), sin reducir totales por rechazados. Confirmar no recibe bytes ni crea expedientes.

## 6. Recepción

Un archivo por solicitud. Referencias de lote/archivo y clave de intento via metadatos multipart; contenido como binario. Validar pertenencia antes de aceptar y durante transferencia aplicar cuotas concurrentes. No admitir ruta de escritura proporcionada por cliente.

Proceso: reclamar recepción → escribir parte bajo raíz controlada → comprobar tamaño/contenido permitidos y SHA-256 → cerrar archivo → convertirlo en staging completo → persistir referencia/huella/estado. El orden no crea transacción disco/DB: registrar evidencia y reconciliar crash en cada frontera. Si hay escritura completa sin registro, no procesar como documento hasta verificar y asociar autorizado; si fila dice RECIBIDO pero archivo falta, bloquear efectos y solicitar recuperación segura.

Salida `RECIBIDO` con bytes/huella significa únicamente staging validado. Reintento del mismo contenido recupera resultado; contenido diferente no sustituye silenciosamente un archivo confirmado. Nombre/tamaño/fecha de reselección no prueban identidad, aunque ayudan a correlacionar candidatos.

## 7. Puertos internos

### Staging implementado localmente, 2026-10-28

`AlmacenStagingImportacion` implementa IAlmacenStagingImportacion con una instancia vinculada a lote/archivo de servidor (copias independientes de sus metadatos). El constructor recibe raíz física de la aplicación, política de límites, lote, archivo y dos presupuestos PDF positivos explícitos: máximo de páginas y de objetos xref. No hay valores productivos predeterminados ni lectura de rutas desde DTO. La composición del servicio/handler y su configuración de ambiente siguen pendientes.

La raíz retenida es App_Data/ImportacionCarpetas. Las subrutas se generan con sello de destino, empresa, usuario, lote y archivo; la referencia opaca se deriva mediante SHA-256 del contexto y del intento/token de reclamación, tomando 128 bits. El token no se devuelve como stagingRef. Se comprueban contención y componentes sin reparse points antes de crear/abrir archivos. Nombre y ruta originales no determinan el nombre retenido. El helper de copias valida el nombre original exacto, incluidos dispositivos reservados, ADS y sufijos ambiguos de Windows; no renombra silenciosamente.

Recibir exige intento RECEPCION, pertenencia al recurso y estado RESERVADO/EN_CURSO. Crea una parte exclusiva, lee como máximo 4096 bytes por bloque y un byte adicional para detectar exceso, exige tamaño exacto y calcula SHA-256 de bytes reales. Comprueba cuota retenida del lote (incluye partes), disponibilidad de disco y límites servidor/perfil. Un bloqueo de archivo exclusivo serializa esta operación dentro del mismo almacenamiento físico del lote; no se presenta como reclamación MySQL o fencing documental. Contención antes de obtener el bloqueo devuelve RECEPCION_OCUPADA sin iniciar escritura. Un intento con parte/completo previo se rechaza antes de consumir el stream y exige consulta/reconciliación.

Formato caracterizado en este corte: **PDF 1.0–1.7**, sin cifrado ni reconstrucción reparadora de xref; cabecera y fin de archivo, parser iTextSharp existente en modo parcial, árbol de páginas y presupuestos de páginas/objetos. Extensión/MIME no bastan. Otros formatos, incluido TIFF/Multi-TIFF, devuelven FORMATO_NO_SOPORTADO. El receptor respeta además las extensiones y tamaño de la publicación. Esta comprobación estructural no declara validación antimalware ni almacenamiento/incorporación final.

Solo después de validar se mueve la parte a completo y se devuelve referencia/bytes/hash internos. El retorno no cambia ninguna fila o estado documental. Partes truncadas, excesivas o fallidas permanecen como evidencia privada y consumen cuota; no se borran para permitir otro intento. La retención/limpieza depende de su fase autorizada posterior.

Verificar reabre el completo del intento, comprueba tamaño, hash y formato; si existe fila RECIBIDO exige coincidencia con su referencia/bytes/hash. Un completo perdido o alterado deja RESULTADO_INCIERTO. Una parte aislada nunca se acepta como completo. La recuperación del servicio debe decidir qué registrar en MySQL; este componente no confirma la recepción por encontrar una fila.

CrearCopiaTrabajo exige recurso RECIBIDO con evidencia consistente, verifica el original y crea un directorio opaco nuevo con el nombre documental original. Devuelve una ruta interna solo al adaptador; consumir/borrar la copia deja el original retenido. Las copias se comprueban por tamaño/hash y disco disponible; la cuota de recepción cubre partes/completos retenidos, no representa una reserva global de disco ni una política de limpieza de copias. La coordinación y mantenimiento deberán controlar su ciclo de vida antes de habilitar efectos.

### Composición de recepción implementada localmente, 2026-10-28

`generic_control/ImportacionCarpetasUpload.ashx` recibe los tres campos multipart `loteId`, `archivoId`, `claveIntento` y un archivo. Usa IRequiresSessionState y el guard operativo existente antes de Form/Files; no usa el nombre, MIME o ruta cliente como destino. Devuelve el sobre JSON común con datos de recepción, sin stagingRef, token de reclamación o rutas. Los presupuestos PDF se configuran explícitamente en `Doc111.Staging.MaximoPaginasPdf` y `Doc111.Staging.MaximoObjetosPdf`; ausentes o no positivos dejan la capacidad indisponible. No se escriben valores predeterminados ni se modifican límites IIS.

`ServicioRecepcionImportacion.Recibir` carga la referencia persistida del lote filtrada por identidad/empresa, reconstruye destino y autoriza gabinete/carga vigentes, obtiene el archivo mediante archivo → raíz → lote y comprueba estado/revisión antes de staging. No exige perfil activo para un lote confirmado ni sustituye su snapshot. Vuelve a comprobar autoridad antes de registrar una recepción nueva. Una revocación durante la transferencia conserva el completo sin declararlo recibido.

El puerto de capacidad `IRecepcionArchivosImportacionRepository` lo implementa el repositorio operativo único. ReservarIntento admite ahora RECEPCION/ARCHIVO; las reclamaciones de efectos documentales siguen pendientes del coordinador. Bloquea lote y archivo durante una transacción breve, busca la huella de comando (contexto/recurso/clave), conserva el intento previo ante repetición y rechaza otra reclamación activa aunque sea antigua. Solo el token nuevo generado por servidor habilita escribir. No se mantiene transacción MySQL durante el streaming.

ConfirmarRecepcion valida pertenencia/token/estado, tamaño previsto y forma de referencia/hash, registra archivo RECIBIDO, intento CONFIRMADO, evidencia y evento en una transacción. Conserva estado_efecto sin actualizarlo a COMPLETADO y aumenta revisiones. Un fallo de evento debe revertir el cambio asociado. La evidencia incluye RecepcionVerificadaImportacion dentro del resultado del intento, no SQL o rutas. El lector comprueba identidad cliente/raíz, documento de origen, bytes y hash de ruta; el snapshot del lote conserva su huella.

Un reenvío de archivo ya RECIBIDO usa `VerificarReenvio`: verifica primero el completo y compara SHA-256/tamaño del stream entrante sin escribir otra parte. Contenido distinto produce CONTENIDO_DISTINTO y no sustituye la recepción. Para el mismo comando en vuelo, solo puede verificar un completo existente; una parte o ausencia de archivo deja incertidumbre. La antigüedad o cierre del navegador no autoriza replay.

`ReconciliarRecepcion` es una operación interna del servicio (sin endpoint auxiliar de efectos). Con autoridad vigente verifica disco/fila/intento y puede confirmar el completo que quedó antes de un fallo de registro. Si la fila ya está recibida solo devuelve éxito tras verificar su completo y evidencia; si falta o está alterado devuelve RESULTADO_INCIERTO. No promueve partes, no fabrica prueba negativa para liberar intentos y no crea ni incorpora documentos.

Rechazos demostrados antes de tener completo cierran el intento como RECHAZADO; necesitan otra clave explícita tras revisar el fallo. Un completo producido o fallo no demostrable queda INCIERTO con reclamación retenida. Registrar un fallo posterior no degrada una confirmación ya persistida. RECEPCION_OCUPADA acredita que el intento no inició escritura y permite cerrarlo sin dejar un escritor incierto. El handler no repite ninguna mutación automáticamente.

HTTP: éxito 200; sesión 401; autoridad/token 403; conflicto/ocupación de intento/contenido distinto/incertidumbre 409; exceso 413; formato no soportado 415; contenido rechazado 422; contención previa a escritura 429; capacidad no configurada 503. JSON, no-cache y supresión del redirect de formularios se comprueban localmente; la validación IIS/HTTP y las transacciones MySQL reales siguen pendientes.

| Puerto propuesto | Contrato |
|---|---|
| PerfilConsulta / ResolucionCampos | Devuelve versión/configuración autorizada y valores tipados/resoluciones diferidas. No crea documentos. |
| ContextoProduccion | Valida identidad de nivel, permisos, ámbito y ruta actual. Reusable por apertura/confirmación/ejecución. |
| PlanRepositorio / OperacionRepositorio | Persistencia, CAS, reclamos e idempotencia. No reglas de nombres ni HTML. |
| ArchivoStaging | Recepción/referencia controlada, validación/huella y copia de trabajo. No camino arbitrario. |
| ExpedienteAdapter | Comando tipado, IDs reales/relación, rechazo conocido o resultado incierto. |
| DocumentoAdapter | Almacenar/verificar con campos finales, formato y contexto. Resultado distingue fase e IDs conocidos. |
| Reconciliacion | Consulta evidencia por operación; no inferir éxito solo por nombre/huella global. |

Los contratos no fuerzan clases concretas o interfaces del código SII. Resultado de adaptador: `Confirmado`, `RechazadoSinEfectos`, `ParcialConocido`, `Incierto`; incluye fase, IDs conocidos y evidencia saneada. Una excepción/retorno distinto de YES en legacy no demuestra ausencia de efecto.

### Reserva documental y señal de detención: implementación local 2026-10-28

El repositorio operativo existente incorpora `IControlLoteImportacionRepository`; su archivo parcial `.Control.vb` no constituye otro repositorio ni otra familia de tablas. `ReservarIntento` conserva RECEPCION y añade CREAR_EXPEDIENTE/RAIZ, ALMACENAR/ARCHIVO e INCORPORAR/ARCHIVO. VERIFICAR no se admite como nueva reclamación ejecutora: la reconciliación posterior conserva sus reglas específicas.

La transacción bloquea primero el lote autorizado, luego raíz y archivo cuando corresponde; comprueba revisión, estado e intento activo, y serializa inicialmente un único intento documental del lote. Intentos RESERVADO/EN_CURSO/INCIERTO o marcadores documentales pendientes impiden otra adquisición, sin usar antigüedad como permiso. Inserta intento RESERVADO, fija el marcador del recurso, revisiones y evento en la misma transacción. Para crear solo admite raíz PENDIENTE sin identidad previa; para almacenar exige recepción completa y raíz confirmada; para incorporar exige documento/expediente/gabinete conocidos coherentes con el snapshot y conserva esos IDs. No admite reintentos de rechazados sin un mecanismo de evidencia negativa implementado.

Un comando repetido devuelve el intento/token históricos y `EsNuevaReserva=False`; solo la adquisición nueva devuelve True. Ese indicador es transitorio y lleva JsonIgnore, no es evidencia durable ni parte de HTTP. Aun conservando el mismo token, la repetición no concede otra adquisición. Una respuesta perdida se consulta antes de cualquier ejecución. **Reservar no invoca legacy**: falta la fase de inicio/ejecución del coordinador con autoridad inmediata, barrera y adaptador compatible; el indicador no sustituye esas comprobaciones.

`CoordinadorImportacionCarpetas.SolicitarDetencion` reconstruye el destino fijado desde la identidad y referencia persistidas, comprueba autoridad actual y snapshot, y registra únicamente `detencion_solicitada`, revisión y evento. No cambia estado agregado, intentos, recepciones, IDs ni resultados. Una señal ya registrada se devuelve sin otra escritura, incluso con la revisión anterior; una revisión vencida antes de registrarla produce conflicto. Un lote terminal con revisión vigente se devuelve sin cambios. La respuesta tipada solo contiene lote, estado, revisión y señal; no afirma que una petición en vuelo terminó.

Estas capacidades permanecen internas sin endpoint nuevo ni invocación de adaptadores. 10.9 sigue parcial hasta implementar/persistir resultados de 10.7; 10.11 conserva las dependencias de barreras, ejecución y reconciliación. La caracterización de aislamiento mantiene deshabilitado almacenamiento y su función existente permanece intacta.

## 8. Consulta y recuperación

### Navegación DOC-119, 2026-11-12

Importar y consultar se alojan en el menú superior Importación y en el contextual de niveles, independientemente de administrar perfiles. El acceso superior a importar fija el nivel seleccionado; el contextual captura el nivel pulsado por formato/ID del nodo, sin fallback a la selección global. Fondo/expediente/documento no son destinos de importación. La consulta por referencia se aloja en el mismo asistente, admite último lote/verificación pendiente y conserva el destino devuelto por ConsultarLote aunque la selección visible sea distinta.

Las pistas de sessionStorage siguen aisladas por contexto de recuperación y contienen solo IDs; cambiar contexto destruye la instancia anterior y descarta pistas en memoria. No se añaden DTO, endpoints, roles o claves de activación. La administración mantiene sus guards y permisos separados. El menú compartido agrega solo metadatos de invocación/captura para las nuevas acciones; el primer argumento de callbacks legacy conserva su contrato.

La aceptación responsable de SEG-02 usa cobertura real/local explícita (`Validacion/doc119-aceptacion-cobertura-mixta-propuesta-20261112.md`), sin modificar PASS_READONLY_WITH_LIMITS del runtime. La guía preserva las limitaciones de inserción nueva/homónimos observadas solo localmente. La composición de limpieza permanece deshabilitada por decisión expresa; no cambia recepción/almacenamiento, estados o auditoría de negocio.

El contrato implementado usa páginas numeradas desde 1, conforme al puerto operativo existente, con orden ascendente por ID y límite configurado. Cada respuesta incluye revisión de lectura y totales originales/filtrados; páginas separadas tienen snapshots independientes. La propuesta inicial de cursor opaco queda como evolución futura. Consulta no ejecuta trabajo.

`accionesPermitidas` se calcula con estado/autoridad: continuar, detener, reseleccionar pendientes, solicitar verificación. La UI no decide habilitación solo por color/etiqueta. Reconciliar requiere permiso distinto de consultar; si recupera un ID, debe probar el vínculo operación–efecto y destino. Ajustes manuales de resultado exigen evidencia auditada, no un parámetro `marcarExitoso=true`.

### Lectura y exportación implementadas, 2026-10-28

El ASMX existente incorpora ConsultarLote, ConsultarRecursosLote y ExportarPaginaLote, todos POST JSON con sesión y guard operativo. Comparten sobre tipado, CSRF/origen, códigos HTTP y no-cache; las facultades administrativas conservan su dispatcher independiente. El servicio de consulta consume un puerto de solo lectura del repositorio único, implementado en el parcial `.Consulta.vb`.

ConsultarLote recibe loteId y reconstruye propietario/empresa/destino desde la referencia persistida y autoridad actual. Verifica snapshot y recursos mediante la acción CONSULTAR del adaptador existente. Se conserva de forma restrictiva el requisito actual de agregar expediente y cargar archivo sobre el nivel, además del permiso operativo y del gabinete; no se concede un nuevo rol de consulta ni se infiere acceso de administración. Un destino movido o un recurso no autorizado impide devolver resultados. No se consulta disponibilidad de la publicación para sustituir el snapshot del lote.

La respuesta contiene estado persistido, revisión, destino visible, perfil/versión, política/totales originales, señal de detención e indicadores separados de intento activo e incertidumbre. Consultar no recalcula ni escribe el estado agregado. Las acciones anunciadas en esta entrega son únicamente CONSULTAR y EXPORTAR_PAGINA después de comprobar autoridad y estado válido. Los endpoints de avance/detención/reconciliación siguen pendientes de 10.11 y no se anuncian por una etiqueta de estado.

ConsultarRecursosLote recibe loteId, tipo RAICES/ARCHIVOS, pagina/tamanoPagina y filtros opcionales raizId/estado/estadoRecepcion. Estado se interpreta como estado de raíz o de efecto según tipo; estadoRecepcion solo corresponde a ARCHIVOS. Se rechazan valores fuera de los conjuntos cerrados, referencias no canónicas y desbordamiento del offset. SQL parametrizado con propiedad/empresa/nivel y orden por ID; transacción de lectura RepeatableRead para la revisión, cuenta y filas de cada página. No hay FOR UPDATE ni escritura de recursos. Sin filtros incluye rechazados, cancelados e inciertos. totalFiltrado describe la selección; totalOriginal conserva el inventario congelado.

La composición exige `Doc111.Consulta.MaximoPagina` entero positivo hasta el techo técnico 200. Falta/valor inválido devuelve CAPACIDAD_NO_DISPONIBLE antes de construir el repositorio de consulta; no hay valor habilitante por defecto ni edición automática de Web.config. Este techo acota el transporte, no fija una cuota funcional de archivos del lote. La política de capacidad general existente sigue requerida por el guard operativo.

La proyección pública enumera campos concretos: IDs/estados/nombre/bytes/revisión e indicador de intento, con IDs documentales conocidos. Omite staging, token, claves de confirmación, hashes internos, valores efectivos y mensajes/evidencia libres de adaptadores. Los códigos de resultado se proyectan mediante lista cerrada; uno desconocido se presenta como RESULTADO_REGISTRADO.

ExportarPaginaLote devuelve CSV de una página autorizada, sus metadatos de paginación y nombre generado en servidor. No recorre todo el lote en memoria ni presenta esa página como exportación completa. Revalida la misma autoridad/filtros; entrecomilla celdas, duplica comillas y protege prefijos de fórmula `=`, `+`, `-`, `@`, incluidos espacios/controles y marcas invisibles iniciales. Devuelve contenido tipado dentro del sobre JSON para que el cliente gestione la descarga. UI, MySQL/HTTP reales y mantenimiento/limpieza siguen pendientes.

### Cliente integrado localmente, 2026-10-28

Producción renderiza un contenedor operativo independiente del administrador DOC-110, con rutas de aplicación y token del propósito ImportacionCarpetas. Solo un nodo de nivel con referencia numérica canónica alimenta data-nivel; un nodo de expediente no se transforma silenciosamente en su padre. PreRender actualiza esa referencia tras el árbol/UpdatePanel. AbrirImportacion valida el destino antes de montar la ventana; después queda fijado aunque cambie la selección del árbol. El menú propio y el botón delegado no llaman ActivaCargaArchivos.

El cliente conserva el inventario y las identidades File originales. La vista usa las etapas Perfil/Carpetas/Revisión/Resultados, paginación acotada, textContent para contenido externo, foco contenido en el diálogo y estilos bajo doc111-importacion. PrepararPlan lleva manifiesto/versión/nivel; ConfirmarPlan lleva solo planId/revisión/huella/clave. Un cambio de perfil/inventario invalida la revisión; una respuesta antigua no la restablece. Incidencias y unidades elegibles se muestran por separado y PuedeConfirmar procede del servidor, también en parcial.

La API operativa usa X-Importacion-Carpetas-CSRF, JSON ASMX/ASHX y multipart propio. Nunca renueva automáticamente sesión/token ni repite una mutación por HTML, respuesta inválida, timeout o aborto. La cola admite una sola transferencia; un fallo impide iniciar las siguientes. La UI envía únicamente pendientes sin efecto de la página autorizada, enlazados por idCliente a un File con nombre/bytes correspondientes. El progreso describe bytes de la petición multipart enviados; solo el servidor confirma recepción/efecto. No se invocan fases documentales pendientes de 10.11.

ConsultarLote añade maximoTamanoPagina a su respuesta pública para que el cliente elija un tamaño dentro de la política explícita vigente. Resultados conservan totales originales, estados separados y exportación de la página actual. El Blob de descarga y su URL se liberan después de iniciar la descarga o al cerrar; no hay previews/binarios del lote en memoria.

El primer corte de interfaz conserva referencias en memoria y deja 12.10 pendiente. El corte de recuperación descrito a continuación incorpora persistencia acotada de referencias y reselección explícita; sus límites sustituyen esa condición inicial. Cerrar libera eventos, File y solicitudes cliente y no certifica cancelación del servidor. La actualización del árbol continúa pendiente de relaciones documentales verificadas en 12.11.

### Recuperación y reselección implementadas localmente, 2026-10-28

ConsultarConfirmacionLote recibe únicamente planId, mediante el guard operativo y la fachada de consulta existente. Reconstruye identidad/empresa/destino desde la referencia propia del plan y revalida recursos actuales; el repositorio consulta plan/lote en una transacción de lectura RepeatableRead, sin FOR UPDATE, actualización o confirmación. Un resultado positivo devuelve planId/loteId/nivelRef ya persistidos. Sin confirmación verificable devuelve confirmada=false, que mantiene el bloqueo: no es prueba negativa para repetir una mutación ni tomar control por antigüedad. Una combinación persistida incoherente queda indisponible. No exige que vuelva a estar publicada la versión del lote.

El contenedor incorpora un identificador opaco aleatorio de espacio de referencias, ligado en servidor a sesión, actor/empresa/login y origen RA. Se renueva cuando cambia ese contexto; no es un permiso ni token de API y no expone esos valores. El navegador usa sessionStorage bajo ese espacio y almacena solo versión del formato y los IDs canónicos del último lote y/o plan pendiente. La pista de plan se escribe antes de enviar ConfirmarPlan, cubriendo recarga o pérdida de respuesta. No se persisten token, credenciales, cookie, clave de confirmación, hash, manifiesto, nombre/ruta o File. Toda referencia, incluso manipulada, vuelve a autorizarse en servidor.

Tras recargar, Consultar último lote recupera el estado actual y su destino servidor. Verificar confirmación pendiente solo invoca la nueva consulta de lectura; un resultado positivo conserva el mismo lote y habilita su consulta, sin volver a emitir ConfirmarPlan. La ausencia/no visibilidad del resultado mantiene la referencia y permite otra consulta explícita. También se admite escribir un lote conocido para consultarlo bajo la misma autoridad. Si el navegador impide persistir referencias, se avisa para conservar el ID y usar la consulta manual; no se promete continuidad desatendida o acceso al disco tras cerrar el navegador.

Los DTO de archivos incorporan rutaRelativa original y ultimaModificacionLocal para distinguir y comprobar una reselección, sin exponer ubicación física privada. Cada botón elige explícitamente un recursoId/raizId pendiente sin efecto ni intento activo. Un selector de archivo individual vincula el File solo a ese recurso, conservando los IDs del lote. Se comprueban nombre/tamaño/fecha contra metadatos servidor y se revalida antes de enviar. Estos metadatos no prueban igualdad de contenido; la recepción mantiene validación de formato/bytes/hash y su protección idempotente. Recibidos, inciertos y recursos en vuelo no ofrecen sustitución. Dos nombres iguales entre raíces no se asocian automáticamente.

Cerrar o recargar libera los File reseleccionados; deben elegirse de nuevo cuando falten. La UI no cambia el inventario congelado ni elimina rechazados para poder continuar. En móvil los resultados se muestran como fichas para mantener legibles identidad, recepción, efecto y acción. La verificación local utiliza Chromium con recarga y sessionStorage nativos, páginas interceptadas y backend sintético; no acredita pipeline IIS, MySQL, autenticación o efectos documentales reales.

### Diagnóstico de retención interno, 2026-10-28

ServicioConsultaImportacion.DiagnosticarRetencion recibe una página ARCHIVOS y retención opcional de composición servidor, fuera del DTO cliente. Reutiliza autorización de propietario/empresa/destino/gabinete y el puerto de consulta ya implementado. No hay endpoint nuevo ni llamadas al staging/disco o a mutaciones del repositorio. diagnosticoEnSeco=True y eliminacionHabilitada=False son propiedades de solo lectura del resultado. No se crea una política de eliminación por disponer de una duración.

Se evalúa la retención desde actualizado_utc del lote, conservador respecto a sus últimas mutaciones registradas. Revisión diferente entre consulta de estado y página invalida todos los candidatos de esa página. Incertidumbre o actividad global también los invalida, aunque el filtro o el archivo individual parezcan completos. Sin retención positiva o con fecha inválida, futura o no calculable se conserva todo; antes del vencimiento también. Solo lote/archivo COMPLETADO con recepción RECIBIDO y datos coherentes de referencia temporal, bytes/hash e IDs de documento/gabinete/expediente puede señalarse como candidatoRevision. Este indicador no comprueba presencia física ni relaciones contra sistemas externos y no concede permiso para eliminar.

El resultado preserva totales original/filtrado, revisión, páginas, vencimiento calculado y motivos cerrados por archivo, sin devolver stagingRef, contenidoHash, rutas, mensajes del adaptador ni valores efectivos. candidatosEnPagina solo cuenta esa página. El diagnóstico cubre referencias registradas; no inventaría partes/copias huérfanas del disco. La eliminación de 11.7 permanece pendiente de política/facultad, verificación actual y reglas de reconciliación, con conservación de evidencia necesaria.

## 9. Errores y recuperación de resultados

El catálogo HTTP vigente está en sección 1 y se contrasta con EstadoHttp; el tratamiento por operación procede de los servicios. CONFLICTO_REVISION exige consultar estado/revisión; RESULTADO_INCIERTO conserva recursos hasta evidencia; OPERACION_EN_CURSO/RECEPCION_OCUPADA no autorizan un competidor. CAPACIDAD_NO_DISPONIBLE es indisponibilidad de esta capacidad, sin trasladarla a la carga individual existente. No usar los nombres conceptuales anteriores NO_AUTENTICADO, REVISION_OBSOLETA, IDEMPOTENCIA_CONFLICTO o ESQUEMA_NO_DISPONIBLE como códigos wire garantizados.

Errores globales de preflight bloquean confirmación; incidencias individuales se mantienen con correlación y se evalúan según la política fijada. Un HTTP 200 no sustituye examinar ok/datos/estadoEfecto. La matriz final DOC-111 registra serialización/sesión/operaciones ejercidas y casos locales; no convierte la revisión DOC-112 en otra prueba IIS/MySQL.

### Reconciliación vigente y alcance verificador — DOC-112

ReconciliarRecurso recibe loteId, intentoId y revisionEsperada, sin motivo libre, recursoId o payload de ejecución. Usa CoordinadorImportacionCarpetas.Reconciliacion y la composición verificadora con lectores de evidencia: no llama a los puertos de creación o almacenamiento. Puede confirmar un efecto ya ocurrido y consolidar el lote en el checkpoint; «completar» aquí no significa ejecutar otra fase externa. Evidencia incompleta conserva INCIERTO y los IDs conocidos.

CerrarReservaSinInicio usa el mismo DTO y composición; exige reserva ALMACENAR nunca iniciada con prueba válida y bloqueo común contra el inicio. Repetir el cierre no libera otra reclamación. La recuperación histórica del lote 2 ejerció cierre del intento 5 y reconciliación del intento 6 conservando documento 16204 e inventario 23293, sin volver a almacenar ese PDF. Véanse [matriz final RQ-09](../Validacion/doc111-matriz-final-20261102.md) y [verificación final del lote 2](../Validacion/doc111-lote2-verificacion-pass-20261102.md); sus límites distinguen la recuperación real de carreras/pérdida de respuesta probadas localmente.

## 10. Registro histórico de implementación por componente

Los cortes fechados siguientes conservan su alcance local al momento de redactarse. Frases «todavía pendiente», «sin invocación real» o «candidato» no invalidan la implementación y aceptaciones posteriores de la matriz final. Igual criterio aplica a los cortes locales conservados en secciones 4/7/8: no prometen capacidad adicional ni sustituyen las fichas vigentes.

### Creación interna implementada localmente, 2026-10-29

`ICreacionExpedienteImportacion.Crear(contexto, lote, raiz, intento)` recibe el lote persistido para conservar su snapshot/contexto y evita reconstruirlo a partir de parámetros sueltos. Es el puerto estrecho implementado por AdaptadorExpedienteImportacion; el puerto completo mantiene la reconciliación pendiente. No hay endpoint nuevo y el coordinador aún debe comprobar la barrera y persistir el resultado final de fase.

Antes de invocar, el adaptador valida comando/catálogos y autoridad actual. IniciarCreacionExpediente bloquea lote/raíz en el orden existente, contrasta revisión, snapshot y valores, y cambia el intento de RESERVADO a EN_CURSO por IDs/fase/token/huella, con resultado preliminar que conserva ciclo e intento, revisión de lote y evento en la misma transacción. Un UPDATE de cero filas no concede otro inicio. El commit ocurre antes de la llamada legacy; un fallo en esta frontera no habilita replay.

El binding real a Conservación usa argumentos nombrados: código manual resuelto, fechas, nombre en tema/alias visible, clasificación y gabinete comprobados, ciclo Archivo Gestión y nivel fijo. No crea contenedores físicos, parentesco con otro expediente, autorregistro o contexto Workflow/SII. Se usan contadores iniciales cero y volumen inicial uno, sin atribuir al parámetro omitido por el INSERT una capacidad de volumen distinta. Conserva IDs ByRef en Finally aun si hay excepción.

El resultado de esta fase solo se devuelve CONFIRMADO tras YES exacto, ambos IDs positivos y lectura de relación/empresa/creador/nivel/permiso, clasificación, código/nombre/alias/fechas, ciclo, gabinete, responsable y volumen. Cualquier fallo posterior al inicio conserva INCIERTO y los IDs parciales conocidos sin mensajes SQL/rutas o repetición. La evidencia de creación opcional en ResultadoEfectoImportacion contiene intento y ciclo ID/nombre; no se expone un token nuevo ni se simula persistencia final. Esas transiciones finales pertenecen a 10.7, todavía pendiente.

El nombre visible tiene límite de 120 de ALEAS_EXPEDIENTE en este recorrido: se informa en preflight y se revalida antes del efecto, sin truncar. El núcleo se prueba con dobles y el lector con SELECT sintéticos; el binding productivo se compila contra ClassGaExpediente real, sin ejecutarlo. No acredita XML físico, carrera/rollback MySQL real, barreras o aceptación integral.

### Checkpoint de creación implementado localmente, 2026-10-29

Checkpoint de creación posterior, 2026-10-29: RegistrarResultadoCreacion implementa el registro final de esta fase, sustituyendo el pendiente de persistencia de creación del corte anterior. Bloquea lote, raíz e intento en el orden común, compara IDs/token/huella y ciclo de inicio, y guarda intento/raíz/lote/evento atómicamente. Confirmado fija identidad y libera marcador de raíz; incierto conserva IDs parciales en evidencia, identidad de raíz no confirmada, marcador y PENDIENTE_VERIFICACION. No completa el lote ni altera totales o la señal de detención. Repetición canónica exacta no escribe ni duplica evento; resultados distintos, cambio de IDs o resolución de incertidumbre requieren la capacidad explícita de reconciliación aún pendiente.

El adaptador llama a ese registro después de cada salida del efecto. Un fallo o pérdida de respuesta conserva incertidumbre e IDs sin otra invocación legacy. El inicio y checkpoint usan el mismo repositorio/origen capturado, sin redirigir el guardado por cambios posteriores de Session. Códigos y mensajes se canonizan antes de guardar; no se persisten textos de error recibidos del escritor. Las fases de almacenamiento/incorporación y el coordinador no se declaran implementados por este checkpoint. La nueva regresión contra DDL comprueba el enlace de evidencia_doc del intento, corrigiendo el nombre erróneo resultado_doc usado inicialmente.

### Barrera de recepción previa a creación, 2026-10-29

ComprobarBarreraCreacion es una consulta interna de metadatos, no una reserva ni un permiso durable. Usa el plan confirmado/sellado del lote y la misma evaluación pura que vuelve a ejecutar IniciarCreacionExpediente bajo el bloqueo común de lote/raíz, antes de cambiar el intento. Un True anterior no permite omitir la comprobación actual; un False no cambia la raíz o el intento ni invoca un escritor. El futuro coordinador debe consultar antes de reservar para evitar dejar una reserva esperando una recepción todavía incompleta. Una reserva ya existente que no supera la revalidación conserva su marcador sin permiso de repetición.

La evaluación exige correspondencia de contexto/snapshot, cantidades/bytes originales e identidades de raíz/archivo por claves persistidas, sin inferir homónimos por nombre. EXPEDIENTE evalúa todos los archivos originales de la raíz; LOTE todos los originales del lote y sus raíces. Cada archivo requerido debe estar RECIBIDO, con bytes exactos/positivos, hash y stagingRef presentes en formato canónico y sin marcador activo o efecto incierto/en curso. En parcial/NO_APLICA basta un archivo elegible recibido, usando la misma decisión de elegibilidad del plan que la confirmación; no se descartan registros rechazados ni se recalculan totales.

Se leen solo columnas necesarias y un máximo de filas derivado del inventario confirmado más una fila para detectar exceso. El plan se carga mediante el lector que verifica su huella; no se relee una publicación nueva. La barrera es de metadatos de recepción: no abre el staging ni acredita que el archivo físico siga disponible/íntegro. Esa revalidación, coordinación integral y barreras de las fases posteriores siguen pendientes; no se habilita ejecución HTTP.

### Staging físico antes de reservar creación, 2026-10-29

AlmacenStagingImportacion.VerificarRetenido valida un RECIBIDO persistido por su referencia opaca, sin fabricar un intento de recepción ni crear una copia de trabajo. Comparte el verificador existente de contención/sin enlaces, bloqueo del staging, bytes exactos, SHA-256 y PDF. La autoridad y metadatos deben corresponder al lote/archivo autorizados. No sustituye el temporal o su evidencia positiva por un fallo posterior de lectura.

La vista interna VistaBarreraCreacionImportacion se obtiene del repositorio único bajo su bloqueo común y contiene plan sellado, raíz y metadatos del ámbito con revisión de archivo. No es DTO HTTP. ReservarCreacionConStaging comprueba la solicitud de creación y barrera, carga cada archivo completo por ID y exige que identidad/revisión/recepción coincidan con la vista y con nombre/ruta del manifiesto. Reautoriza, verifica físico y únicamente después llama al puerto de reserva una vez.

Completo exige verificar todos los archivos de la unidad. Parcial puede omitir de esta selección un archivo perdido/corrupto cuando hay otro elegible verificado: devuelve ArchivosVerificados y ArchivosInciertos como IDs, preservando el inventario y sus totales. Sin uno válido, con revisión cambiada o con fallo de autoridad/infraestructura/bloqueo no reserva. Un intento histórico devuelto por la reserva conserva EsNuevaReserva=False; releer un PDF no lo convierte en permiso de ejecutar.

Las listas son transitorias, no prueba durable ni permiso de usar una verificación anterior para un efecto futuro. El bloqueo de lectura se libera antes de la reserva; el coordinador completo debe conservar exclusión y revalidar según la fase. No se habilita un endpoint de ejecución ni se declara terminada 10.3/10.4–10.6. Persistencia de incidencias, coordinación integral y fases posteriores siguen pendientes.

### Una fase interna de creación coordinada, 2026-10-29

CoordinadorImportacionCarpetas.CrearExpediente recibe loteId/raizId, revisión original de raíz y clave de operación. Reconstruye identidad y nivel desde el recurso persistido; no acepta actor, destino o snapshot cliente. La huella estable incluye contexto/raíz/revisión/clave/fase, y el token se genera en servidor. Consultar un intento histórico por esa huella no reserva ni ejecuta y conserva incertidumbre sin inventar un resultado.

Para un comando nuevo prepara/verifica staging, reserva una vez y vuelve a cargar el lote/raíz, comprobando marcador, revisión y detención. El creador se compone con un callback obligatorio que revalida autoridad, vista y staging después del inicio durable. AlmacenStagingImportacion.ConservarVerificado mantiene el archivo abierto con FileShare.Read tras comprobar hash/PDF; no expone rutas o streams. El contenedor de retenciones conserva los archivos válidos de la unidad e intenta cerrar todos en salida/fallo. El adaptador mantiene esa retención durante la llamada, verificación y checkpoint y la libera en Finally. La comprobación previa de la sección anterior sigue siendo transitoria; esta retención cubre la ventana de ejecución de creación.

Una detención, revocación, cambio de revisión o staging perdido después de reservar no ejecuta legacy; el intento queda observable para revisión, sin otra adquisición automática. La fase solo llama al creador una vez y no continúa a almacenamiento, incorporación o materialización de diferidos. Tras devolver/recibir error consulta el checkpoint: un resultado confirmado persistido exige ambos IDs y la misma identidad en la raíz; de lo contrario informa incertidumbre y conserva los IDs observados válidos. No repite una creación para reparar una respuesta perdida.

ComposicionCreacionImportacion enlaza la fase con autoridad, repositorio, staging y binding existentes. Los presupuestos PDF se comparten con recepción mediante LimitesImportacionCarpetas.PresupuestoPdf, sin valores implícitos ni cambios de Web.config. La composición exige HttpContext.Current; no es un worker. No se agrega endpoint/acción ASMX y el cliente sigue sin ejecutar fases documentales. SQL/legacy reales, XML y fases restantes requieren sus validaciones posteriores.

### Materialización interna de valores, 2026-10-30

`ComposicionPerfilImportacionOperativa.CrearMaterializacion(http)` construye el coordinador con la autoridad, catálogo, proveedor/política y repositorio compartidos. `MaterializarArchivo(loteId, archivoId, revisionEsperada)` procesa un único documento recibido bajo un expediente confirmado. Revalida destino y recursos; la vista del repositorio comprueba expediente/relación/clasificación por IDs. No agrega una acción ASMX ni invoca almacenamiento.

Solo se resuelven y validan los campos diferidos. Los valores previos se conservan sin consultar sus fuentes ni reinterpretar IDs. El checkpoint del mismo repositorio compara la vista bajo bloqueo, fija valores y procedencia una vez e incrementa revisiones/evento. SQL NULL indica ausencia de checkpoint; `[]` es un resultado ya fijado. La repetición devuelve lo persistido sin proveedor, manteniendo autoridad vigente. Una incompatibilidad registra RECHAZADO/VALORES_INCOMPATIBLES y conserva identidad/totales; no permite nueva reserva de almacenamiento. Recuperación explícita y coordinación de las fases restantes siguen pendientes. Evidencia local `Validacion/doc111-valores-diferidos-local-20261030.md`.

### Almacenamiento documental interno, 2026-10-30

`AdaptadorDocumentoImportacion(http, staging)` compone la capacidad interna de almacenamiento con autoridad, catálogo y repositorio existentes. Recibe raíz/archivo/snapshot/intento de una reserva nueva ALMACENAR y utiliza los valores efectivos fijados. El binding llama a `AlmacenamientoDocumentoProduccionDocumental` con una copia PDF de nombre exacto y modo cero; conserva el original privado y no cambia selecciones de sesión. No se expone como nueva acción ASMX y no procesa el lote en un bucle.

La confirmación exige YES, IDs positivos de documento/inventario, nombre/gabinete devueltos coincidentes y comprobación SELECT de relaciones, nombres, tipo/formato e índices. El resultado añade `InventarioId` opcional para distinguirlo del ID de imagen. El inicio y resultado se persisten en el repositorio único por CAS. Un resultado incierto conserva los IDs conocidos y el intento activo, sin replay automático; la confirmación completa solo el archivo. Cierre de raíz/lote, coordinación HTTP y reconciliación siguen pendientes. Evidencia local `Validacion/doc111-adaptador-documento-local-20261030.md`; binding compilado y writer/SQL dobles, sin efectos reales.


### Coordinación interna de almacenamiento, 2026-10-30

`ComposicionDocumentoImportacion.Crear(http)` construye el coordinador para `AlmacenarArchivo(loteId, archivoId, revisionEsperada, claveOperacion)`. Procesa únicamente ALMACENAR para un archivo con expediente y valores efectivos ya confirmados. Obtiene el destino del lote, autoriza, consulta histórico por contexto/archivo/revisión original/clave/fase y comprueba la barrera de recepción antes de reservar. Un histórico evita disco, otra reserva y otro writer. Una composición documental rechaza ejecutar creación.

La fase comparte la evaluación y staging existentes. Completo/EXPEDIENTE o LOTE verifican y retienen todos los originales exigidos; parcial/NO_APLICA solo el objetivo elegible sin reducir inventario. El intento propio solo permite EN_CURSO para su archivo/ID exactos. Reserva e inicio vuelven a comprobar metadatos bajo bloqueo. Después de reservar revalida revisión, marcador y autoridad; una detención, revocación o pérdida de staging impide el escritor.

La respuesta interna devuelve lote/raíz/archivo/intento, repetición, resultado y referencias verificadas. Solo confirma con intento final y resultado de archivo COMPLETADO concordantes. Una respuesta perdida puede recuperarse leyendo ese checkpoint; de otro modo conserva incertidumbre/IDs sin replay. No completa raíz/lote ni agrega endpoint ASMX. Consolidación/reconciliación y exposición HTTP siguen pendientes; evidencia local `Validacion/doc111-fase-documento-local-20261030.md`.


### Consolidación interna de estados, 2026-10-30

Los checkpoints nuevos de creación/documento/valores y la detención llaman a la consolidación del mismo repositorio dentro de su conexión/transacción. Lee todos los originales y los intentos relevantes con límites derivados del plan; exige correlaciones y valores originales. COMPLETADA/COMPLETADO requiere pruebas terminales concordantes de creación y documentos, con recepción, IDs y valores fijados. Rechazados y pendientes no desaparecen de los totales; los mismos IDs de documento/inventario no pueden completar dos originales.

Incertidumbre tiene prioridad sobre actividad/detención. Una llamada activa impide declarar DETENIDO; completar el último efecto conserva la señal de detención. Un estado INCIERTO no sale por agregación. Solo se modifican estados/revisiones/fechas y eventos, sin borrar recursos ni cambiar identidad/snapshot. La repetición igual no genera SQL de actualización o eventos. La capacidad interna `ConsolidarLote(contexto, loteId, revisionEsperada)` no tiene endpoint. Evidencia local `Validacion/doc111-consolidacion-local-20261030.md`; transacciones/efectos reales pendientes de aceptación.

### Reconciliación interna de un intento (corte 2026-10-30)

`ReconciliarIntento(loteId, intentoId, revisionEsperada)` exige propietario/destino vigentes y una vista correlacionada con plan, valores y metadatos originales. Solo INCIERTO con IDs completos persistidos puede comprobarse mediante los lectores SELECT existentes. No invoca adaptadores de ejecución, reserva, proveedores ni staging. RESERVADO/EN_CURSO y vencimientos conservan su reclamación; evidencia insuficiente conserva INCIERTO. El ciclo Archivo Gestión debe coincidir con el ID registrado.

La confirmación canónica se persiste por CAS en el repositorio único, seguida de consolidación en la misma transacción. No admite IDs/valores sustituidos por el verificador. Se conserva detención y toda reclamación ajena; una respuesta perdida se resuelve leyendo, sin repetir el guardado ni el efecto. La repetición de una confirmación coherente no genera SQL/eventos. Los comandos de comprobación tienen marca inmutable y son rechazados por las rutas ordinarias de ejecución/checkpoint. Composición interna implementada; no hay endpoint hasta 10.11. Evidencia local `Validacion/doc111-reconciliacion-local-20261030.md`, con SELECT/SQL dobles y build completo, sin efectos reales.

### Fachada HTTP de fases (10.11, corte 2026-10-30)

El ASMX expone `AvanzarLote`, `SolicitarDetencion` y `ReconciliarRecurso` mediante el mismo dispatcher/guard operativo POST JSON con sesión. Avanzar recibe lote/recurso, revisión, clave y fase cerrada CREAR_EXPEDIENTE (raíz), MATERIALIZAR_VALORES o ALMACENAR (archivo). Ejecuta solo esa fase por el coordinador, sin encadenar otra ni exponer adaptadores. La revisión de avance pertenece al recurso; detención y reconciliación usan revisión de lote. Reconciliar recibe el `intentoId` persistido en lugar de una marca manual de éxito o IDs de efectos. La página autorizada publica la referencia numérica del intento activo para recuperación, sin token.

Las salidas son proyecciones públicas de fase/estado/código/IDs conocidos. Incertidumbre devuelve `ok=false`, HTTP 409 y datos conocidos; rechazo de valores devuelve 422. Materialización confirmada no acredita almacenamiento y su indicador de repetición es nulo porque no crea intento. El estado agregado/revisión actuales se consultan mediante las lecturas existentes, sin efecto implícito. Composición productiva separada y compilada con bindings legacy; pruebas HTTP sintéticas con coordinadores reales y dobles declarados. Detalle y ejemplos de entrada: `Validacion/doc111-http-fases-local-20261030.md`. Sin HTTP/IIS autenticado o efectos reales; conexión del cliente/árbol pendiente.

### Cliente de fases y rama original (12.11, corte 2026-10-30)

ConsultarLote incorpora CONSULTAR_RAICES y solicitudes SOLICITAR_AVANCE/DETENCION/VERIFICACION según autoridad, estado, actividad/incertidumbre y señal vigentes. No sustituyen las barreras del coordinador. La página de archivos publica `valoresEfectivosFijados`; la de raíces, `nombreExpediente` desde NOMBRE confirmado, solo con identidad y estado comprobados de raíz. La UI combina esas facultades con el recurso/padre consultados para ofrecer un único avance por clic. Tras mutación correcta consulta; tras respuesta incierta exige consulta explícita sin replay. Detención y reconciliación usan la revisión del lote y avance la del recurso.

La lectura de raíces actualiza únicamente una rama visible/cargada del árbol existente mediante el helper nativo, usando relación/nivel/expediente y nombre NOMBRE. Repeticiones no agregan duplicados ni cambian selección/expansión; rama ausente/contexto cambiado omiten solo el refresco. Evidencia local: `Validacion/doc111-cliente-rama-local-20261030.md`, Chromium/Framework y build completos, sin efectos reales. Las notas anteriores de acciones conservadoras de solo lectura quedan sustituidas por este corte.

### Limpieza interna del original (11.7, corte 2026-10-30)

`LimpiarOriginal(loteId, archivoId, revisionLoteEsperada, revisionArchivoEsperada)` no es un WebMethod. La política servidor requiere habilitación explícita y retención positiva, sin valores operativos inventados o cambios de configuración. El diagnóstico en seco sigue sin conceder permiso para eliminar. La operación exige inventario completo y checkpoints positivos, sin actividad/incertidumbre, retención desde la última fecha verificable de lote/checkpoints, autoridad vigente y SELECT actuales del documento/relaciones/valores efectivos.

Se reutilizan el repositorio/tablas y staging privados. Un evento PREPARAR_LIMPIEZA confirmado en una primera transacción liga inventario/política/recurso; otra transacción vuelve a comprobar todo antes de retirar únicamente el `.bin` exacto y registrar conclusión. No se borra historial o metadatos ni se promete atomicidad disco/MySQL. Respuesta/commit perdido conserva preparación e incertidumbre, sin reintento automático. Ausencia con preparación puede concluir como STAGING_AUSENTE_AUTORIZADO; ausencia sin diario se conserva como no verificable. Repetición concluida no abre disco ni crea otro evento. Fuente original alterada/retenida, política ampliada/deshabilitada o permiso revocado impiden borrar. Evidencia y límites: `Validacion/doc111-limpieza-local-20261030.md`, con PDF/SQL dobles y build completo, sin limpieza real.

### COR-03 — Búsqueda asistida de perfiles

ListarPerfiles acepta termino opcional (texto hasta 160 caracteres). Su predicado es coincidencia parcial literal en codigo OR nombre, siempre dentro del ámbito autorizado; codigo sigue siendo un filtro exacto independiente y nombre conserva su contrato anterior. Comparación según collation existente, sin prometer insensibilidad a mayúsculas/acentos. No se interpretan comodines ni expresiones SQL. El autocompletado pide página 1 de 10 resultados desde 2 caracteres y espera 300 ms. Muestra código — nombre; conserva perfilId en cada sugerencia y selecciona el código único del ámbito para filtrar la lista sin abrirla. Buscar aplica el término libre; Limpiar vuelve a la primera página sin filtros. Se reutiliza jQuery UI 1.8.22 y PerfilImportacionApi, misma autorización/CSRF; no hay endpoint ni tabla nuevos.

### COR-04 — Catálogos guiados y consulta puntual
ConsultarCatalogo agrega referencia opcional al DTO cerrado y la transmite al filtro autorizado existente del adaptador. El editor pide página 1/tamaño 1 para resolver el nombre de un ID guardado ausente de la página actual. Sin referencia conserva la paginación anterior. No sustituir valores faltantes ni omitir permisos RA/DA. Frontend invalida descendientes al cambiar padres, reinicia páginas y descarta respuestas de contextos/vistas anteriores; no hay dependencia Fondo → Instrumento. Índices se consultan automáticamente por gabinete. Publicar exige formulario guardado; Consultar estado se reserva a conflicto/resultado incierto y protege ediciones locales. Evidencia y límites: Validacion/doc110-cor04-detalle-guiado.md.


## Precisión DOC-115 — confirmación y recepción, 2026-11-03

Caracterización documental del DTO y servicio existentes; no añade endpoints ni cambia el contrato:

| Operación | Entrada cerrada | Respuesta datos | Autoridad y efecto permitido |
| --- | --- | --- | --- |
| ConfirmarPlan (ASMX POST) | planId cadena decimal, revisionEsperada UInt32, planHashEsperado hex64, claveConfirmacion ASCII 1–64 | loteId/planId/nivelRef cadenas; estado; revision/totalRaices/totalArchivos UInt32; totalBytes cadena decimal | Identidad/ámbito/nivel/sello persistidos; confirma solo inventario operativo. |
| ImportacionCarpetasUpload (ASHX POST multipart) | exactamente loteId, archivoId, claveIntento y un archivo | loteId/archivoId; estadoRecepcion/estadoEfecto; bytesRecibidos cadena decimal; contenidoHash hex64; revision UInt32 | Reautoriza recurso del lote; recibe staging, sin gabinete/expediente/nivel/estado arbitrarios. |

Ambas respuestas usan RespuestaPerfilImportacion y EstadoHttp vigentes. El repositorio calcula solicitud_hash de confirmación sobre planId/revision/huella. Misma clave con payload diferente es CONFLICTO_REVISION; un plan confirmado converge al lote conservando la clave original incluso con otra clave de confirmación. Un 1062, 1205 o 1213 de MySQL se convierte en CONFLICTO_REVISION: conservar evidencia, consultar estado y solo entonces repetir el mismo comando autorizado para verificar convergencia.

claveIntento identifica la solicitud de recepción; la identidad de contenido se prueba con bytes y SHA-256 del retenido. Reenvío íntegro de RECIBIDO devuelve evidencia previa; contenido distinto produce CONTENIDO_DISTINTO sin sustitución. Un intento RECHAZADO no se reutiliza ciegamente. Una incertidumbre sin contenido íntegro no habilita otro escritor por antigüedad.

ReconciliarRecepcion pertenece al servicio interno, no a una operación pública nueva. RECIBIDO no significa documento incorporado. Paths/stagingRef/token de reclamación no se publican. Errores de servidor anteriores al handler se registran como tales, sin atribuirles un envelope JSON de aplicación.

## Consolidación DOC-120 sobre la interfaz definitiva

Base8c0773d4; sin cambio de contratos de aplicación. [Matriz](../Validacion/matriz-verificacion.md) e [índice de resultados](../Validacion/resultados.md) distinguen la evidencia original y la regresión local focal. DOC120-SUP01 adapta únicamente el auxiliar de degradación al menú existente, sin DDL o nueva ejecución real. INT03 requiere contrato/autorización propios y la aceptación final conserva las limitaciones de UI autenticada, SII y limpieza.
