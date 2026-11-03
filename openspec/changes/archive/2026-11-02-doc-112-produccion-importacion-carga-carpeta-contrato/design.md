<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12 -->
# DOC-112 — Refinamiento del diseño físico y los contratos

## Contexto y alcance

El ticket corresponde al prompt 04 y produce documentación revisable. La base de referencia es main `40c5f55e`, con DOC-110 y DOC-111 integrados. El código operativo ya existe: el trabajo consiste en reconciliar sus contratos con los documentos comunes y distinguir requisitos, propuestas históricas y evidencia observada. No se reinicia DOC-111.

La instrucción del usuario de continuar autoriza aplicar el plan documental reducido. El resultado queda en revisión editorial del responsable; no constituye aprobación automática ni autoriza cambios de aplicación o BD.

## Fuente de verdad y entregables

- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/design.md`: Inventario exacto de archivos, propietario 02/04/05/06/07-A/07-B/08/09/10/11/12 y consumidores; estado existente/propuesto acreditado por fuente.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/01-modelo-datos.md`: Diccionario físico y relaciones: separar estructura lógica vigente, SQL de instalación/migración y metadatos de un ambiente concreto.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md`: Catálogo de operaciones, DTO, autoridad, revisión, errores, efectos y reintentos, con ejemplos sintéticos.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/03-estados-recuperacion.md`: Estados por dimensión, barreras, CAS, agregación y matriz de fallos/recuperación.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/04-decisiones-y-pendientes.md`: Decisiones confirmadas por evidencia, restricciones operativas, responsables, secuencia 07-A → 06 → 07-B y condiciones de salida.

OpenSpec referencia estos entregables; no replica sus tablas o payloads completos. Se actualizan sus secciones vigentes conservando los cortes históricos identificados.

## Decisiones refinadas

### D-01 — Alcance documental y fuente única

Actualizar los cuatro diseños comunes y su inventario a partir de DOC-110/111 integrados; OpenSpec referencia esos contratos y no mantiene un segundo diseño físico.

Resultado documental: Inventario y propietarios verificables. Verificación: Al cotejar un componente se identifica su ruta, propietario y estado real sin crear otra capa.

Evidencia a contrastar: Diseno/design.md; guía 00; archivo DOC-111 y matriz final. Origen: RQ-01.

### D-02 — Modelo físico observado y propiedad SQL

Contrastar perfil/version/regla y las seis tablas operativas con repositorios y SQL existente, incluida migración de identidades de DOC-110; separar referencias legacy y catálogos de tablas propias.

Resultado documental: Diccionario físico reproducible. Verificación: Cada columna y clave documentada tiene tipo, nulabilidad, ámbito, índice y fuente; un script existente no acredita instalación en otro ambiente.

Evidencia a contrastar: Sql/01-perfil-importacion-mysql51.sql; Sql/03-migrar-identificadores-perfil-mysql51.sql; Sql/02-carga-carpetas-mysql51.sql; MySqlImportacionCarpetasRepository.vb. Origen: RQ-02.

### D-03 — Identidad, huellas e idempotencia

Distinguir seleccionId/idCliente de PK servidor; describir SHA-256, canonicalización y comparación del contenido completo para plan, ruta y fase, sin unicidad global del nombre visible.

Resultado documental: Identidad y repetición sin ambigüedad. Verificación: Misma clave/contenido devuelve el resultado previo; cambiar contenido, revisión o recurso no autoriza duplicar el efecto.

Evidencia a contrastar: ImportacionCarpetasDtos.vb; MySqlImportacionCarpetasRepository.vb; PerfilImportacionJson.vb. Origen: RQ-03.

### D-04 — Autoridad efectiva y transporte

Mantener separados administrar e importar; resolver identidad/empresa en servidor y permisos de nivel/gabinete/acción, CSRF de JSON/multipart y errores seguros sin rutas, SQL o tokens.

Resultado documental: Matriz de autoridad por fase. Verificación: Un importador no necesita administración; sesión/permiso/token inválidos impiden la operación afectada antes de sus efectos.

Evidencia a contrastar: PerfilImportacionContextAdapter.vb; ImportacionCarpetasContextAdapter.vb; guards; ImportacionCarpetasUpload.ashx.vb. Origen: RQ-04.

### D-05 — Snapshot y resolución por etapas

Documentar publicación inmutable, JSON ASCII versionado y acotado, valores PREFLIGHT/TRAS_EXPEDIENTE, IDs aún inexistentes, fechas generadas y valores efectivos ya fijados.

Resultado documental: Valores resolubles sin crear durante preview. Verificación: Un dato que requiere expediente conserva dependencia tipada; un checkpoint fijado, incluso vacío, no se recalcula por repetición.

Evidencia a contrastar: PerfilImportacionModels.vb; ImportacionCarpetasModels.vb; PerfilImportacionJson.vb; CoordinadorImportacionCarpetas.Valores.vb. Origen: RQ-05.

### D-06 — Catálogo único de contratos públicos e internos

Inventariar la fachada ASMX y el handler multipart actuales con entradas/salidas, propietario, revisión, errores y efectos. Documentar equivalencias de la guía sin inventar endpoints de adaptador.

Resultado documental: Cliente y servidor usan el mismo contrato. Verificación: Cada operación tiene DTO y observable existentes; detención equivale a SolicitarDetencion y no promete cancelación destructiva o un CancelarLote inexistente.

Evidencia a contrastar: WebServiceImportacionCarpetas.asmx.vb; ImportacionCarpetasDtos.vb; ImportacionCarpetasInterfaces.vb; ImportacionCarpetasUpload.ashx.vb. Origen: RQ-06.

### D-07 — Estados por dimensión y agregación

Separar PLAN, RECEPCION, EFECTO, RAIZ, INTENTO y LOTE; contrastar transiciones cerradas y agregados con CoordinadorImportacionCarpetas sin equiparar bytes recibidos con incorporación.

Resultado documental: Transiciones y totales consistentes. Verificación: Una transferencia recibida no confirma un documento y un rechazado sigue en los totales originales.

Evidencia a contrastar: CoordinadorImportacionCarpetas.vb; CoordinadorImportacionCarpetas.Consolidacion.vb; ImportacionCarpetasModels.vb. Origen: RQ-07.

### D-08 — Barreras y exclusión del coordinador existente

Conservar COMPLETO_OBLIGATORIO con ámbito EXPEDIENTE o LOTE y PARCIAL_PERMITIDO/NO_APLICA; describir CAS/revisión/reclamaciones y barreras del mismo coordinador.

Resultado documental: Tres políticas con una coordinación común. Verificación: EXPEDIENTE avanza la raíz lista, LOTE bloquea ambas si falta un archivo y PARCIAL avanza elegibles conservando rechazados.

Evidencia a contrastar: CoordinadorImportacionCarpetas.Barreras.vb; MySqlImportacionCarpetasRepository.Barreras.vb; MySqlImportacionCarpetasRepository.Control.vb. Origen: RQ-08.

### D-09 — Frontera legacy y efectos verificables

Caracterizar creación y almacenamiento actuales: contexto temporal delimitado, copia consumible, nombre original, Archivo Gestión por nombre y prueba de IDs/relaciones/índices; preservar función legacy y SII.

Resultado documental: Adaptadores caracterizados sin cambiar escritores compartidos. Verificación: Almacenamiento ya incorporado no se incorpora otra vez; NOMBRE respeta visibilidad; código de formato con signo no se confunde con ID de recurso.

Evidencia a contrastar: AdaptadorExpedienteImportacion.Legacy.vb; AdaptadorDocumentoImportacion.vb; ComandoAlmacenamientoCarpetas.vb; workflow/ClassAlmacenamiento.vb. Origen: RQ-09.

### D-10 — Ventanas de fallo y reconciliación

Documentar incertidumbre entre SQL/disco/XML/respuesta, evidencia positiva/negativa y cierre de reserva sin inicio; un timeout o antigüedad no habilita replay.

Resultado documental: Recuperación sin inventar causalidad. Verificación: Un resultado incierto conserva recursos; solo evidencia válida permite reconciliar o cerrar la reserva nunca iniciada bajo el mismo bloqueo de inicio.

Evidencia a contrastar: CoordinadorImportacionCarpetas.Reconciliacion.vb; MySqlImportacionCarpetasRepository.Reconciliacion.vb; MySqlImportacionCarpetasRepository.Documento.vb. Origen: RQ-10.

### D-11 — Capacidad, disponibilidad y retención

Registrar límites derivados del perfil/transporte/JSON/sesión, disponibilidad de publicaciones y condiciones operativas de volumen/navegador/retención con responsable y efecto del bloqueo.

Resultado documental: Límites y exclusiones explícitos. Verificación: No se convierten 500 archivos/25 MB del prototipo en promesa; sin política de retención se conservan originales.

Evidencia a contrastar: LimitesImportacionCarpetas.vb; ServicioConsultaImportacion.Limpieza.vb; Diseno/04-decisiones-y-pendientes.md. Origen: RQ-11.

### D-12 — QA documental y trazabilidad de cierre

Recorrer contratos en papel con dos raíces, barreras, diferidos, revisión y fallos; separar evidencia heredada de DOC-111 de la revisión DOC-112 y validar enlaces/D-RQ-tareas.

Resultado documental: Revisión documental reproducible. Verificación: DOC-112 no ejecuta E2E/MSBuild/SQL ni acredita otra implementación al revisar contratos ya existentes.

Evidencia a contrastar: Diseno/01–04; tasks.md; refinement.md; Validacion/doc111-matriz-final-20261102.md. Origen: RQ-12.

## Hallazgos que motivan el plan

- El inventario común todavía presenta como nuevas varias rutas ya integradas por DOC-111; la tarea de DOC-112 debe distinguir existencia en código y despliegue.
- El modelo común conserva encabezados de propuesta y descripciones de identidades anteriores a la migración DOC-110; se contrastará SQL 01 más SQL 03 con el repositorio vigente. Las referencias legacy no se convierten en FK nuevas por inferencia.
- Los contratos y estados acumulan cortes locales que conservan frases de HTTP/ejecución por completar. Deben identificarse como históricos frente a la fachada y aceptaciones posteriores, sin borrar su procedencia.
- La fachada operativa ofrece AbrirImportacion, ListarPerfilesImportacion, PrepararPlan, ConfirmarPlan, ConsultarConfirmacionLote, ConsultarLote, ConsultarRecursosLote, ExportarPaginaLote, AvanzarLote, SolicitarDetencion, ReconciliarRecurso y CerrarReservaSinInicio. RecibirArchivo usa el handler propio. Limpieza y adaptadores no son endpoints públicos.
- Los DTO no aceptan usuario/empresa/snapshot autoritativo/SQL/ruta física desde el cliente. Los IDs de recursos viajan como cadenas canónicas; las revisiones como UInt32 y los bytes como cadenas. Se distinguirán de códigos de catálogo nativos con signo.

## Compatibilidad y exclusiones

No se modifican VB/JS, DTO, puertos, vbproj, SQL, permisos, configuración o consumidores SII. No se ejecutan consultas autenticadas, DDL, cargas, pruebas E2E o compilación para esta entrega documental. Se reutilizan evidencias existentes con fecha/ambiente/huella y límites explícitos. El aislamiento adicional del XML frente a otras cargas fue descartado en DOC-111: se documenta ese alcance sin reabrirlo como bloqueo. PDF es la capacidad caracterizada del adaptador; no se atribuye TIFF/Multi-TIFF. El host desatendido 13 y la limpieza real de recursos quedan fuera.

## Condiciones operativas registrables

Volumen objetivo, matriz de navegadores, retención operativa y presupuestos de tiempo requieren definición de operación para anunciar capacidad o activar mantenimiento. Estas condiciones no autorizan defaults del prototipo ni impiden describir límites configurables y rechazos del contrato existente. La publicación previa de perfiles no concede permisos sobre nivel/gabinete.

## Secuencia de trabajo y aceptación — sustituida por el contraste de brechas

Primero inventario/evidencia; después diccionario/identidad/autoridad; luego snapshot y catálogo de operaciones; finalmente transiciones/barreras/adaptadores/fallos y QA documental. La dependencia 07-A → 06 → 07-B se describe como propiedad de las entregas originales, sin reconstruir sus implementaciones.

El cierre exige coherencia de los cuatro documentos comunes, revisión con dos raíces en las tres políticas, entradas/salidas y errores sin campos inventados, enlaces y trazabilidad D-01–D-12/RQ-01–RQ-12. Una revisión estructural o el PASS histórico de DOC-111 no acredita ejecución nueva de DOC-112.

## Alcance depurado tras el contraste

[gap-analysis.md](gap-analysis.md) es el mapa del trabajo pendiente y sustituye la interpretación exhaustiva del plan anterior. Los D-01–D-12 y RQ-01–RQ-12 siguen como criterios, pero no ordenan rehacer todos los contratos ni volver a probar capacidades ya aceptadas. El plan vigente tiene doce correcciones: inventario, deltas del modelo, ejemplo de confirmación, transporte público, referencias de valores, equivalencias de operaciones, estado de coordinación, evidencia de barreras, aceptación de adaptadores, reconciliación, pendientes operativos y revisión final del delta.

Se preservan tablas, payloads y matrices que ya coinciden; cada cambio debe citar una diferencia observada. Las fuentes iniciales son evidencia histórica, los DTO/repositorios versionados fijan lo implementado y la matriz final DOC-111 delimita qué se probó realmente. No se exige repetir recorridos REC ni reescribir cada ficha. Una posible carencia funcional se registra y evalúa separadamente antes de proponer código.
