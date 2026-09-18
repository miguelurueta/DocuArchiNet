# DOC-67 — Bitácora quirúrgica de diagnóstico E2E

## Propósito y reglas de evidencia

Este documento conserva los fallos reales encontrados durante la validación autenticada del escenario `import-sii-execution`, su causa, corrección y evidencia. Es parte permanente del paquete técnico DOC-67.

- No contiene contraseñas, cookies, tokens, cadenas de conexión ni valores personales.
- Los códigos corresponden a respuestas saneadas del backend o de la plataforma E2E.
- Las funciones legacy originales se conservan; las correcciones se realizan en adaptadores, repositorios y coordinación moderna.
- Cada corrida debe terminar con `WorkflowCentroTrabajoModernActive=false` y usuarios/grupos vacíos.
- Un avance a un error posterior confirma que la frontera anterior dejó de bloquear, pero no sustituye la regresión automatizada ni la postcondición física.

## Resumen de progresión

| Orden | Código observado | Frontera | Estado |
|---:|---|---|---|
| 1 | `E2E_RESOURCE_UNAVAILABLE` | Preparación del recurso | Corregido mediante estructura y datos de prueba requeridos |
| 2 | `IMPORT_E2E_QUERY_FAILED_SESSION_WORKFLOW_USER_UNAVAILABLE` | Contexto autenticado Workflow Registro | Corregido al usar usuario/tarea seleccionada y módulo correctos |
| 3 | `IMPORT_E2E_STORED_DOCUMENT_STATUS_NOT_AVAILABLE_EXPEDIENT_UNRESOLVED` | Persistencia/reconciliación | Diagnosticado; motivó completar la saga de expediente |
| 4 | `IMPORT_E2E_CREATE_FAILED_IMPORT_UNAVAILABLE` | Creación de intención | Corregido con diagnóstico seguro y resolución autoritativa de inscripción |
| 5 | `IMPORT_E2E_CREATE_FAILED_EXPEDIENT_CONFIGURATION_QUERY_FAILED` | Configuración de expediente | Refinado a códigos por consulta |
| 6 | `...EXPEDIENT_CONFIGURATION_HEADER_QUERY_FAILED` | Cabecera del trámite | Corregido usando conexión DocuArchi para `tipo_doc_entrante` |
| 7 | `...EXPEDIENT_DESTINATION_UNRESOLVED` | Plan de expediente | Refinado para propagar la causa real |
| 8 | `...EXPEDIENT_IDENTITY_CONFLICT` | Identidad física | Refinado en conflictos de gabinete, primaria y campos |
| 9 | `...EXPEDIENT_IDENTITY_EXPECTATION_UNRESOLVED` | Materialización de identidad | Corregido parcialmente integrando función de auto registro |
| 10 | `...EXPEDIENT_IDENTITY_EXPECTATION_UNRESOLVED_CODIGO_SERIE_TRD` | Gestión documental | Corregido integrando campos de gestión legacy |
| 11 | `...EXPEDIENT_IDENTITY_FIELD_MISSING` | Lectura de postcondición | Corregido mediante aliases de columnas históricas |
| 12 | `...RELATED_DOCUMENTS_NOT_CONFIRMED` | Documentos relacionados | Abierto: requiere granularidad por relación/caché/índices SQL/XML |
| 13 | `...RELATED_DOCUMENT_CABINET_INDEX_NOT_CONFIRMED` | Índices del gabinete | Abierto: relación y caché confirmadas; se refina escritura/postcheck |

## Hallazgos y correcciones

### 1. Esquema moderno incompleto

**Síntoma**

La inspección previa reportó tablas, columnas, índice y llave local faltantes:

- `workflow_import_inscription`
- `workflow_import_related_document`
- `workflow_import_document_link_cache`
- columnas de expediente y estados en `workflow_import_intent_item`
- índice `ix_import_item_inscription`
- FK local `fk_import_item_inscription`

**Decisión**

La persistencia moderna se mantiene en la base Workflow. No se crea una FK hacia catálogos ubicados en otra base de datos. La integridad entre módulos se valida por servicios/repositorios y postcondiciones, no mediante una restricción física cross-database.

**Artefacto**

`Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-67-creacion-vinculacion-expediente/Sql/manual-create-missing-schema-mysql51-without-procedures.sql`

El script es compatible con MySQL 5.1 y no crea procedimientos almacenados.

### 2. Radicado Workflow y código de barras SII confundidos

**Síntoma**

La creación de intención terminaba en `IMPORT_UNAVAILABLE` aunque la tarea y el recurso SII eran válidos.

**Causa**

`SiiImportInscriptionResolver` comparaba dos identidades independientes:

- radicado Workflow: identidad del trámite/tarea;
- código de barras SII: clave de consulta del proveedor.

**Corrección**

Se eliminó la igualdad incorrecta. Se conserva la regla válida: todos los elementos seleccionados para una intención deben pertenecer al mismo código de barras SII.

**Código seguro nuevo**

`SII_INSCRIPTION_BARCODE_CONFLICT`

### 3. Errores de creación ocultos por `IMPORT_UNAVAILABLE`

**Síntoma**

La frontera ASMX convertía fallos conocidos de infraestructura/configuración en un código genérico.

**Corrección**

Se añadieron fronteras y allowlists de códigos seguros en `CreateImportIntent`, el resolvedor SII y la persistencia de intención. No se exponen SQL, excepciones internas ni conexiones.

### 4. Conexiones de módulo incorrectas

**Síntoma**

`EXPEDIENT_CONFIGURATION_HEADER_QUERY_FAILED`.

**Causa**

La configuración está distribuida entre módulos:

- `tipo_doc_entrante`: conexión DocuArchi (`Dbase_Conction_Mysql_DA`);
- `ra_auto_campo_unico_expediente`: conexión Radicación (`Dbase_Conction_Mysql_RA`);
- `ra_dig_tipos_docum_lista_chequeo`: conexión Radicación.

El repositorio moderno consultaba todas las tablas usando una sola conexión.

**Corrección**

`MySqlImportExpedientConfigurationRepository` recibe fábricas separadas para DocuArchi y Radicación, usa consultas parametrizadas y propaga códigos por etapa:

- `EXPEDIENT_CONFIGURATION_HEADER_QUERY_FAILED`
- `EXPEDIENT_CONFIGURATION_IDENTITY_FIELDS_QUERY_FAILED`
- `EXPEDIENT_CONFIGURATION_SECONDARY_TYPES_QUERY_FAILED`
- `EXPEDIENT_CONFIGURATION_RADICACION_CONNECTION_FAILED`

### 5. Causa de destino de expediente colapsada

**Síntoma**

`EXPEDIENT_DESTINATION_UNRESOLVED` no permitía distinguir configuración, sujeto, identidad, caché o creación.

**Corrección**

`PlanExpedienteImportacion` conserva `Codigo`, y `ImportExpedientCoordinator`/`ImportServiceOrchestrator` propagan el código seguro de la frontera que falló.

### 6. Conflictos de identidad sin granularidad

**Síntoma**

`EXPEDIENT_IDENTITY_CONFLICT`.

**Corrección**

La verificación física diferencia:

- `EXPEDIENT_SNAPSHOT_UNAVAILABLE`
- `EXPEDIENT_CABINET_CONFLICT`
- `EXPEDIENT_PRIMARY_IDENTITY_CONFLICT`
- `EXPEDIENT_IDENTITY_EXPECTATION_UNRESOLVED`
- `EXPEDIENT_IDENTITY_FIELD_MISSING`
- `EXPEDIENT_IDENTITY_FIELD_CONFLICT`

Los nombres técnicos de campos pueden agregarse al código bajo una longitud y alfabeto seguros; nunca se agregan valores.

### 7. Valores de identidad no materializados

**Síntoma**

`EXPEDIENT_IDENTITY_EXPECTATION_UNRESOLVED`.

**Causa**

El repositorio cargaba los nombres de campos `estado_unico=1`, pero `CampoIdentidadExpedienteImportacion.Valor` permanecía vacío.

**Comportamiento legacy preservado**

1. `SolicitaDatosAutoRegistro` obtiene `funcion_servicio_datos`.
2. `SolicitaDatosFuncionAutoRegistro` recibe un `CIncripcionSII` autoritativo.
3. Para `ConsultarRadicado`, materializa `CODIGO_UNICO`, `NOMBRE_PERSONA_EXPEDIENTE` e `IDENTIFICACION_PERSONA_EXPEDIENTE`.

**Corrección**

`LegacySiiExpedientSubjectResolver` invoca esas funciones originales con datos obtenidos del SII. El cliente no puede proporcionar los valores de autoridad.

Los campos únicos opcionales pueden conservar vacío, como en legacy; únicamente un campo obligatorio vacío produce `EXPECTATION_UNRESOLVED`.

### 8. Campos de gestión documental omitidos

**Síntoma refinado**

`EXPEDIENT_IDENTITY_EXPECTATION_UNRESOLVED_CODIGO_SERIE_TRD`.

**Causa**

`SolicitaDatosFuncionAutoRegistro` solo llena datos del sujeto SII. El legacy ejecuta posteriormente `SolicitaDatosGestionCamposAutoRegistro` y asigna la configuración archivística.

**Corrección**

El adaptador moderno reutiliza la misma función y materializa:

| Origen | Campo de expediente |
|---|---|
| `IdArea` | `CODIGO_AREA_TRD` |
| nombre de área | `NOMBRE_AREA_TRD` |
| `IdSerie` | `CODIGO_SERIE_TRD` |
| nombre de serie | `NOMBRE_SERIE_TRD` |
| `IdSubSerie` | `CODIGO_SUB_SERIE_TRD` |
| nombre de subserie | `NOMBRE_SUBSERIE_TRD` |
| `IdFondo` | `ID_FONDO` |
| nombre de fondo | `NOMBRE_FONDO` |
| `IdInstrumento` | `id_instrumento` |
| unidad configurada | `ID_TIPO_UNIDAD_DOCUMENTAL`, `NOMBRE_TIPO_UNIDAD_DOCUMENTAL` |
| fecha formateada legacy | `FECHA_CREACION` |

La resolución de nombres sigue usando las clases legacy conservadas.

### 9. Alias entre columnas físicas y estructura legacy

**Síntoma**

`EXPEDIENT_IDENTITY_FIELD_MISSING` después de materializar correctamente `CODIGO_SERIE_TRD`.

**Causa**

`SolicitaDatosEstructuraExpediente` lee columnas reales, pero las proyecta con nombres históricos diferentes:

| Columna configurada/física | Miembro de `expediente_conservacion` |
|---|---|
| `NOMBRE_AREA_TRD` | `NOMBRE_AREA` |
| `CODIGO_SERIE_TRD` | `CODIGO_SERIE` |
| `NOMBRE_SERIE_TRD` | `NOMBRE_SERIE` |
| `CODIGO_SUB_SERIE_TRD` | `CODIGO_SUBSERIE` |
| `NOMBRE_SUBSERIE_TRD` | `NOMBRE_SUBSERIE` |

La copia por reflexión solo exponía el nombre del miembro y la verificación buscaba el nombre real configurado.

**Corrección**

`LegacyPhysicalExpedientGateway` agrega aliases explícitos al snapshot, manteniendo el diccionario case-insensitive y sin modificar la estructura legacy.

### 10. Documentos relacionados no confirmados

**Síntoma actual**

`IMPORT_E2E_EXECUTE_FAILED_RELATED_DOCUMENTS_NOT_CONFIRMED`.

**Hechos observados en el artefacto saneado**

Los siete controles registraron cambios:

- expediente;
- intención e items;
- relación documento–expediente;
- caché de vínculo;
- índice documental;
- auditoría de transición.

Esto demuestra que la corrida superó expediente y almacenamiento y avanzó dentro de `ImportRelatedDocumentCoordinator.ProcessOne`.

**Secuencia afectada**

```text
precheck/vínculo/postcheck
  → persistir estado de relación
  → registrar caché verificada
  → actualizar índices del gabinete
  → verificar índice electrónico SQL
  → verificar índice electrónico XML
  → reconciliar
```

**Limitación diagnóstica pendiente**

El coordinador devuelve un booleano y el orquestador colapsa cualquier fallo en `RELATED_DOCUMENTS_NOT_CONFIRMED`. Con la evidencia disponible, SQL/XML o reconciliación son candidatos probables, pero no debe afirmarse cuál sin leer el estado persistido o mejorar la propagación.

**Corrección siguiente requerida**

Propagar códigos específicos desde cada frontera, como mínimo:

- `RELATED_DOCUMENT_PERSISTENCE_FAILED`
- `RELATED_DOCUMENT_RELATION_NOT_CONFIRMED`
- `RELATED_DOCUMENT_CACHE_NOT_CONFIRMED`
- `RELATED_DOCUMENT_CABINET_INDEX_NOT_CONFIRMED`
- `RELATED_DOCUMENT_SQL_INDEX_MISSING`
- `RELATED_DOCUMENT_XML_INDEX_MISSING`
- `RELATED_DOCUMENT_RECONCILIATION_UNCERTAIN`

La saga debe conservar efectos confirmados y continuar siendo idempotente; no se deben borrar documentos ni expedientes para reintentar.

**Corrección diagnóstica aplicada**

`PlanDocumentosRelacionadosImportacion` conserva ahora el código de la frontera fallida. `ImportRelatedDocumentCoordinator` distingue persistencia, inscripción, relación incierta/no confirmada, caché, índice de gabinete, índice SQL, índice XML y reconciliación. `ImportServiceOrchestrator` propaga ese código seguro en vez de colapsarlo siempre en `RELATED_DOCUMENTS_NOT_CONFIRMED`. La siguiente E2E determinará la postcondición física exacta.

### 11. Índice del gabinete no confirmado

**Síntoma refinado**

`IMPORT_E2E_EXECUTE_FAILED_RELATED_DOCUMENT_CABINET_INDEX_NOT_CONFIRMED`.

**Alcance confirmado**

El vínculo físico documento–expediente y la caché verificada terminaron antes del fallo. La detención ocurre al actualizar y releer `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA` en el gabinete DocuArchi.

**Defecto diagnóstico encontrado**

`SiiDocumentIndexAdapter.Actualizar` ignoraba el resultado booleano de `ISiiDocumentIndexPhysicalGateway.ActualizarCampos`. Después convertía rechazo de la función legacy o cualquier divergencia del postcheck en un único `DOCUMENT_INDEX_UPDATE_NOT_CONFIRMED`.

**Corrección aplicada**

- `False` de la escritura legacy produce `DOCUMENT_INDEX_UPDATE_REJECTED`.
- El postcheck compara individualmente los tres campos y produce `DOCUMENT_INDEX_FIELD_NOT_CONFIRMED_<CAMPO>` o `..._SNAPSHOT`.
- El coordinador propaga el código del adaptador sin incluir valores de índice.

La próxima corrida diferenciará rechazo físico frente a divergencia de un campo concreto.

### 12. Corrección arquitectónica: campos dinámicos por gabinete

**Aclaración funcional confirmada**

Los campos de índice no son universales ni están garantizados como `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA`. Cada gabinete define dinámicamente su estructura. La afirmación previa de que siempre debían actualizarse esos tres campos queda invalidada y no debe convertirse en contrato moderno.

**Fuentes legacy de autoridad**

- `DETALLE_GABIENETE`: estructura efectiva del gabinete (`CAMPO`, `TIPO`, visibilidad y estado).
- `Class_DETALLE_GABIENETE.SolicitaDetalleCamposGabinete`: lectura de los campos configurados.
- `Class_ra_ser_servicioIntegracion.SolicitaDatosCamposIndiceGabineteIntegracion`: resuelve los candidatos campo/valor de la integración.
- `Class_Integracion_SII.SolicitaDatosCamposIndiceGabineteSII`: materializa candidatos SII según contexto y gabinete.

**Regla corregida**

```text
campos efectivos
  = campos configurados y activos de DETALLE_GABIENETE
    ∩ campos/valores producidos por la integración SII
```

El backend debe:

1. cargar la estructura del gabinete desde servidor;
2. resolver los valores SII desde contexto confiable;
3. validar cada nombre como identificador seguro y como miembro de la estructura cargada;
4. construir el `UPDATE` solo con esos identificadores permitidos;
5. parametrizar todos los valores y las condiciones `ID`/expediente;
6. releer exactamente el mismo conjunto dinámico;
7. comparar cada postcondición sin registrar valores sensibles.

No se deben ejecutar `SELECT NITCEDULA,RAZONSOCIAL,MATRICULA` ni actualizaciones fijas para todos los gabinetes. La verificación SQL/XML del índice electrónico permanece independiente de los campos dinámicos del gabinete.

**Implementación aplicada**

`LegacySiiDocumentIndexPhysicalGateway` consulta `DETALLE_GABIENETE` por gabinete, forma un allowlist case-insensitive, intersecta los candidatos SII y genera dinámicamente `SET`/`SELECT`. Solo los identificadores provenientes del catálogo y validados por expresión segura entran al SQL; gabinete, valores, imagen y expediente se parametrizan cuando corresponde. El diccionario se reduce al conjunto efectivo para que `SiiDocumentIndexAdapter` relea y compare exactamente los mismos campos. Si la intersección queda vacía, falla cerrado con rechazo de actualización.

La función legacy fija permanece sin modificaciones para compatibilidad, pero la ruta moderna usa el gateway dinámico. Suite posterior: `303` aprobadas, `0` fallidas; compilación MSBuild: código `0`.

**Primera ejecución dinámica**

La primera corrida produjo `DOCUMENT_INDEX_UPDATE_UNKNOWN`: hubo una excepción dentro de la frontera dinámica, pero el adaptador todavía la colapsaba. Se añadieron límites seguros para diferenciar `DOCUMENT_INDEX_STRUCTURE_QUERY_FAILED`, `DOCUMENT_INDEX_DYNAMIC_UPDATE_FAILED`, `DOCUMENT_INDEX_DYNAMIC_READ_FAILED` y `DOCUMENT_INDEX_CONNECTION_FAILED`. Solo códigos con el patrón `DOCUMENT_INDEX_[A-Z0-9_]+` atraviesan la frontera; mensajes del proveedor permanecen ocultos.

**Segunda ejecución dinámica**

La corrida produjo `DOCUMENT_INDEX_DYNAMIC_UPDATE_FAILED`. Quedó demostrado que la conexión DocuArchi abrió, la consulta a `DETALLE_GABIENETE` terminó, la intersección dinámica no quedó vacía y la excepción ocurrió dentro de `IDataExecutor.ExecuteNonQuery`. La relectura/postcheck no llegó a ejecutarse. Las categorías pendientes de distinguir son permisos, tabla/columna, conversión o longitud/tipo de dato y restricción física. El siguiente refinamiento debe clasificar únicamente el número/categoría segura de `MySqlException`, sin publicar SQL, nombres no validados, valores ni mensajes del servidor.

**Regla de longitudes confirmada y aplicada**

Los campos homónimos pueden tener longitudes distintas entre gabinetes. La resolución efectiva cruza ahora `DETALLE_GABIENETE` con `INFORMATION_SCHEMA.COLUMNS` dentro de `DATABASE()`. Para `CHAR`, `VARCHAR` y familias `TEXT`, el valor se recorta al `CHARACTER_MAXIMUM_LENGTH` físico antes de parametrizarlo. Tipos numéricos, fecha y otros tipos no se recortan como texto: conservan su valor para que MySQL valide el tipo y el flujo falle cerrado ante incompatibilidad. El postcheck compara contra el valor ya ajustado, evitando falsos conflictos por truncamiento esperado.

**Clasificación MySQL aplicada**

Una segunda corrida siguió produciendo `DOCUMENT_INDEX_DYNAMIC_UPDATE_FAILED`, demostrando que la longitud no era la única causa. El gateway clasifica ahora exclusivamente `MySqlException.Number`: permisos (`1044`, `1045`, `1142`, `1227`), columna (`1054`), tabla (`1146`), tipo/conversión (`1264`, `1265`, `1366`, `1292`), longitud (`1406`), restricciones (`1062`, `1451`, `1452`) y concurrencia (`1205`, `1213`). Cualquier número no reconocido conserva `DOCUMENT_INDEX_DYNAMIC_UPDATE_FAILED`. Nunca se propaga `ex.Message`.

**Tipo incompatible confirmado**

La corrida siguiente produjo `DOCUMENT_INDEX_UPDATE_TYPE_INVALID`. Se confirmó que MySQL rechazó un valor por tipo/conversión. La resolución dinámica valida ahora cada valor contra `DATA_TYPE` antes del `UPDATE`: tipos numéricos mediante `Decimal.TryParse` invariant y tipos temporales mediante `DateTime.TryParse` invariant. Un rechazo previo produce `DOCUMENT_INDEX_VALUE_TYPE_INVALID_<CAMPO>`; solo se publica el nombre técnico validado, nunca el valor.

La validación se ejecuta fuera de la frontera que convierte fallos de consulta de estructura. Así, `DOCUMENT_INDEX_VALUE_TYPE_INVALID_<CAMPO>` llega intacto al borde ASMX, mientras una falla real al consultar `DETALLE_GABIENETE`/`INFORMATION_SCHEMA.COLUMNS` conserva `DOCUMENT_INDEX_STRUCTURE_QUERY_FAILED`. Ninguna rama inspecciona ni publica `MySqlException.Message`.

**Regla autoritativa de matrícula (aclaración funcional)**

La matrícula se almacena físicamente como entero positivo. Algunos registros SII llegan con letras o ceros iniciales; ninguno forma parte de la identidad. Se estableció una sola normalización para MERCANTIL, ESAL y RUP —en RUP se toma el proponente—: eliminar caracteres no numéricos, retirar todos los ceros de la izquierda y rechazar el resultado si no queda ningún dígito significativo. Ejemplo: `AB001230C` se convierte en `1230`; `A000` es inválido.

La regla se aplica antes de materializar los campos únicos del expediente y el mismo valor se conserva en `MatriculaNormalizada`. Se usa para localizar, crear mediante `AutoRegistraExpedienteTramite`, verificar la identidad física, registrar caché y actualizar el campo dinámico `MATRICULA`. Esto elimina la divergencia anterior en la que el almacenamiento extraía dígitos, pero la creación podía recibir la matrícula cruda.

**Campo dinámico `LIBRO` incompatible**

La corrida posterior se detuvo con `DOCUMENT_INDEX_VALUE_TYPE_INVALID_LIBRO`. La validación previa evitó ejecutar el `UPDATE`, pero el diagnóstico reveló una frontera funcional incorrecta: `LIBRO` no corresponde a la actualización posterior al vínculo. `LIBRO`, `INSCRIPCION`, descripción, fechas, acto, `ENLASE` y recibo se materializan al incorporar el documento. El adaptador posterior quedó restringido a `NITCEDULA`, `RAZONSOCIAL` y `MATRICULA`, intersectados con la estructura dinámica real del gabinete; los demás campos no se reescriben ni se normalizan en esta fase.

**Conflicto de versión aparente al finalizar**

La siguiente corrida llegó hasta la confirmación de documentos relacionados y devolvió `VERSION_CONFLICT`. No fue evidencia de una segunda ejecución concurrente: el orquestador intentaba persistir `CacheActualizado → Completada`, transición prohibida por la máquina de estados. La secuencia obligatoria es `CacheActualizado → Reconciliada → Completada`. Se corrigió el orquestador para persistir ambas transiciones, cada una con su propio token optimista; un conflicto real de token continúa fallando cerrado.

**Primera ejecución funcional completa y brecha de evidencia**

La corrida siguiente terminó sin mensaje de error. El artefacto saneado confirmó `success=true`, un documento procesado, códigos nulos para crear/ejecutar/consultar/reconciliar y cambios en los siete controles `SELECT`. El runner regresó al prompt sin texto porque actualmente solo publica el bloque `catch`; no imprime resumen de éxito.

La tarea E2E no se considera cerrada todavía: el reporte individual produjo tres aserciones aprobadas, seis bloqueadas y una fallida. Los bloqueos corresponden en parte a propiedades cubiertas por otros escenarios, pero las aserciones de efectos 7/8 carecen del arreglo `ExpedientEffects` aunque `ServicioReconciliacionImportacion` ya calcula cada `ImportItemExpedientEffectsDto`. La aserción 10 interpreta además como fallo el estado público `Disponible`, aceptando solamente fases internas `Reconciliada`/`Completada`. Debe completarse el contrato de evidencia y alinear el verificador con el estado público antes de declarar aprobadas las 18 aserciones.

**Matriz obligatoria por tipo de registro**

Una ejecución MERCANTIL no representa por sí sola la entrega. La integración local ejecuta ahora la saga completa para MERCANTIL, ESAL y RUP, incluida su identidad canónica, creación/reutilización de expediente, almacenamiento, universo `ENLASE`, vínculo, caché, índice y XML. El cierre E2E requiere igualmente tres ejecuciones reales con los perfiles existentes `doc56-import-sii-execution-219887.runtime.json`, `doc56-import-sii-esal.runtime.json` y `doc56-import-sii-rup.runtime.json`; cada recurso debe ser descartable y autorizarse en su corrida. La tarea 6.4 permanece abierta hasta consolidar las tres evidencias y las 18 aserciones.

**ESAL 220573: creación automática deshabilitada y ruta de reutilización omitida**

La ejecución real de la tarea ESAL `220573` se detuvo con `IMPORT_E2E_EXECUTE_FAILED_EXPEDIENT_CREATION_DISABLED`. No fue un error de conexión, tipología documental ni consulta de configuración. `MySqlImportExpedientConfigurationRepository` leyó `tipo_doc_entrante.util_Estado_Crea_ExpedienteSII` con valor distinto de `1` y lo proyectó simultáneamente como `CreacionAutomaticaHabilitada=False` y `ExpedienteObligatorio=False`. Después, `ImportExpedientCoordinator.Resolver` devolvió `EXPEDIENT_CREATION_DISABLED` antes de invocar la búsqueda física o la caché.

La comparación con el comportamiento legacy confirmó una brecha: cuando la creación automática está inactiva, `GuardarConstanciaIncripcionSII` no abandona toda la operación; continúa por la ruta de vinculación con un expediente previamente registrado. Por tanto, la bandera controla la autorización para **crear**, pero no demuestra que el expediente sea opcional ni impide **buscar/reutilizar/vincular**. El modelo moderno no debe derivar `ExpedienteObligatorio` directamente de esa bandera y el coordinador debe intentar caché/búsqueda/verificación antes de decidir que no puede crear uno ausente.

La corrida alcanzó a modificar los controles de intención, item y estado de expediente; no cambió relación documental, caché de vínculo, índice ni auditoría de transición. El recurso fue liberado y el gate quedó restaurado con activo `false`, usuarios vacíos y grupos vacíos. No se alteró manualmente la configuración para forzar la prueba.

**ESAL 220573: sujeto SII no resuelto**

Después de habilitar administrativamente la creación automática, la siguiente ejecución avanzó hasta `LegacySiiExpedientSubjectResolver.Resolver` y se detuvo con `IMPORT_E2E_EXECUTE_FAILED_SII_SUBJECT_UNAVAILABLE`. El adaptador llamó `ClassConsultaExpedienteSII.SolicitaEstructuraExpedienteSII(matricula, proponente, gabinete, ByRef sujeto)` y recibió una respuesta distinta de `YES`; por ello no llegó a materializar NIT, razón social, matrícula normalizada ni campos únicos del expediente.

Para gabinete `ESAL`, el legacy transforma la matrícula eliminando todas las apariciones de `9000`, antepone `S0` y consulta el endpoint SII `consultarExpedienteMercantil`. Esa operación encadena: lectura de usuario técnico SII, resolución de URL, solicitud de token, POST al proveedor y deserialización/respuesta funcional. El adaptador moderno descarta el motivo textual devuelto por cualquiera de esas fronteras y lo reduce a `SII_SUBJECT_UNAVAILABLE`; el artefacto saneado no permite distinguir si falló configuración, autenticación técnica, transporte, formato o ausencia de la matrícula. Esta pérdida diagnóstica debe corregirse con subcódigos seguros y sin persistir secretos ni contenido sensible.

Los controles confirmaron cambios únicamente en intención, item y estado de expediente. No hubo cambios en relación documental, caché de vínculo, índice ni auditoría de transición. El recurso se liberó y el gate quedó restaurado con activo `false`, usuarios y grupos vacíos.

**Auditoría quirúrgica de `ConsultaExpedienteMercantilEsal`**

La función legacy conserva valor funcional, pero no satisface por sí sola una frontera misional robusta. La cadena activa es `SolicitaEstructuraExpedienteSII → ConsultaExpedienteMercantilEsal → solicita_usuario_validacion_sii → Solicita_url_nombrefuncion_restfull → Solicitar_token_general → GetResponse_POST → DesSerializacion_ConsultaExpedienteSIIMercantil`.

Hallazgos confirmados:

- La matrícula ESAL se transforma con `Matricula.Replace("9000", "")` y luego se antepone `S0`. `Replace` elimina cada aparición, no solamente un prefijo conocido, y puede cambiar una identidad válida.
- `ws_usuarioworkflowsii` se consulta sin `WHERE`, orden ni validación de unicidad; se utiliza arbitrariamente la primera fila. La clave técnica queda disponible como texto en memoria y se envía dentro del JSON de autenticación.
- Solo se resuelve la URL configurada para `solicitarToken`; después se concatena sobre esa misma base `consultarExpedienteMercantil`, creando un acoplamiento implícito entre rutas.
- La ruta HTTP síncrona activa usa `WebRequest`, no configura `Timeout` ni `ReadWriteTimeout`, no clasifica estado HTTP y devuelve texto de excepción como contrato funcional.
- `ServicePointManager.ServerCertificateValidationCallback` se reemplaza globalmente por `validarCertificado`, que siempre retorna `True`. Esto deshabilita la validación TLS para el proceso completo, no solamente para esta llamada, y permite intermediación no confiable.
- También se modifican globalmente `SecurityProtocol`, `Expect100Continue` y `DefaultConnectionLimit` en cada petición.
- El cuerpo JSON se codifica con ASCII, no UTF-8. Valores no ASCII se degradan.
- Streams y respuestas se cierran manualmente, sin bloques `Using`; una excepción intermedia puede dejar recursos sin liberar.
- La deserialización accede directamente a `HttpContext.Current.Session.Item("DG_CDCARACTERES")`; puede fallar cuando no existe sesión o cuando el flujo se ejecuta fuera de la interfaz legacy.
- La respuesta se considera exitosa atendiendo principalmente a `mensajeerror`; no existe validación fuerte del contrato, de campos obligatorios, de correspondencia entre matrícula solicitada y devuelta, ni de NIT/razón social antes de retornar `YES`.
- No hay política explícita de reintentos, backoff, circuit breaker, presupuesto de tiempo, correlación, métricas ni separación entre fallo transitorio, dato inexistente y contrato inválido.
- Todas las causas se transportan como cadenas. El adaptador moderno las colapsa finalmente en `SII_SUBJECT_UNAVAILABLE`, impidiendo operación y soporte precisos.

Decisión arquitectónica: conservar la función original únicamente como fallback de compatibilidad y no modificarla directamente. Introducir un puerto moderno de consulta de sujeto con cliente HTTP aislado, TLS validado, timeout total, cancelación, UTF-8, contratos tipados, validación de identidad y códigos seguros. Aplicar reintentos limitados solo a fallos transitorios antes de producir efectos; proteger el proveedor con circuit breaker; registrar métricas y correlación sin secretos; y ejecutar comparación controlada contra el legacy antes de retirar el fallback. Ninguna dependencia externa puede garantizar cero fallos: la propiedad misional exigible es fallar de forma acotada, diagnosticable, reintentable e idempotente, sin corromper ni duplicar expedientes.

**Implementación moderna conservando el original**

Se implementó `ModernSiiExpedientSubjectResolver` como ruta primaria del ASMX moderno y se amplió `SiiExternalImportProviderClient` para consultar sujetos MERCANTIL/ESAL y RUP. La ruta reutiliza el transporte ya endurecido: TLS validado por el sistema, timeout/cancelación por solicitud, límite de respuesta, UTF-8, contrato tipado, telemetría y códigos normalizados. Para ESAL se elimina `9000` solamente como prefijo y se antepone una sola vez `S0`; no se usa el `Replace` global legacy.

`ClassConsultaExpedienteSII`, `ConsultaExpedienteMercantilEsal` y sus consumidores permanecen sin cambios. `LegacySiiExpedientSubjectResolver` se conserva como fallback gobernado por `ImportarServicioWebSiiSubjectLegacyFallback`, inicialmente activo para rollout controlado. Las pruebas focales terminaron `21/21` y MSBuild .NET Framework 4.6.1 finalizó con código `0`. La E2E ESAL debe repetirse para validar el proveedor real antes de retirar el fallback.

## Evidencia automatizada acumulada

- Suite DOC-67: `312` pruebas aprobadas, `0` fallidas, incluida la matriz integral MERCANTIL/ESAL/RUP y la consulta moderna de sujeto.
- Compilación `GestionDocumental-Docuarchi.net.vbproj`: código de salida `0` con .NET Framework 4.6.1.
- Gate verificado tras las corridas: activo `false`, usuarios y grupos vacíos.
- E2E final completa: pendiente; tarea OpenSpec `6.4` permanece abierta hasta aprobar las 18 aserciones.

## Archivos principales intervenidos

- `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportInscriptionResolver.vb`
- `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportExpedientConfigurationRepository.vb`
- `Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacySiiExpedientSubjectResolver.vb`
- `Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyPhysicalExpedientGateway.vb`
- `Infrastructure/Repositories/Workflow/ImportarServicioWeb/PhysicalImportExpedientRepository.vb`
- `Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb`
- `Services/Workflow/ImportarServicioWeb/ImportRelatedDocumentCoordinator.vb`
- `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`
- `webservice/WebServiceImportarServicioWebModern.asmx.vb`

## Regla de mantenimiento

Cada nuevo fallo E2E de DOC-67 debe agregarse aquí con: código, frontera, evidencia, causa confirmada o hipótesis explícita, cambio aplicado, prueba de regresión y estado del gate. Nunca se reemplaza la historia anterior; se añade una nueva entrada para conservar trazabilidad.
## Migración física 10.3 — avance 2026-09-16

- El contexto autenticado ahora captura y transporta explícitamente `GA_IDUSUARIOGESTION` y `GA_IDEMPRESA`; la infraestructura física no consulta `HttpContext.Session`.
- La composición productiva usa `ModernPhysicalExpedientGateway`.
- `AutoRegistraExpedienteTramite` se conserva con su firma para los consumidores legacy y delega en `RegistrarExpedienteTramiteConContexto`, que recibe usuario y empresa de gestión explícitos.
- La ruta moderna mantiene precheck y postcheck del repositorio, incluido el tratamiento de respuesta perdida como `ResultadoIncierto`.
- Verificación local: 15/15 pruebas focales y compilación .NET Framework exitosas. Las advertencias del proyecto son preexistentes.
- Cierre 10.3: el precheck usa todos los campos `estado_unico=1`, valida identificadores y parametriza valores; `GET_LOCK` serializa por identidad, una segunda lectura evita duplicados y la postconsulta recupera respuesta perdida. El snapshot exige además que exista un XML válido cuyo `identicacionexpediente` coincida con el expediente SQL. La función legacy y su firma pública permanecen disponibles.

## Migración de vínculo 10.4 — cierre 2026-09-16

- `ModernDocumentExpedientPhysicalGateway` recibe ruta, usuarios y empresa desde el contexto autenticado; no accede a `HttpContext.Session`.
- La lectura del gabinete valida el identificador dinámico y parametriza `IdImagen`, con límite de dos resultados para distinguir conflicto.
- Un `GET_LOCK` por gabinete/imagen serializa precheck, mutación y postcheck; una relación correcta se reutiliza, una cruzada o múltiple se rechaza y solo la ausencia permite escribir.
- El retorno `YES` nunca confirma el efecto: el adaptador relee la relación y el coordinador conserva verificaciones independientes de producción, vínculo, caché e índice SQL/XML.
- Evidencia local: 22/22 pruebas focales aprobadas y compilación .NET Framework finalizada con código 0.

## Corrección de invariancia legacy — 2026-09-16

- La suite integral detectó correctamente que la primera extracción había modificado `Gestion/ClassGaExpediente.vb`.
- Se restauró el blob canónico `2998523902ec2d455ac4b96a644297674d6d14b9` y se trasladaron ambos recorridos a `ModernExpedientMutationBridge`.
- El puente moderno no usa `HttpContext.Session`; recibe usuario, empresa, tarea y ruta explícitos. Los métodos `AutoRegistraExpedienteTramite` y `VinculaDocumentoExpediente` originales permanecen byte a byte intactos.
- Evidencia posterior: compilación exitosa, pruebas de huella aprobadas y suite completa `ImportarServicioWeb` con 320/320 casos aprobados.
# Auditoría posterior a ejecución ESAL 220573 — 2026-09-17

La corrida sobre la tarea descartable `220573` fue marcada como consumida por la plataforma, pero el reporte resumido fue sobrescrito por un reintento bloqueado con `E2E_RESOURCE_CONSUMED`. Se reconstruyó la evidencia exclusivamente mediante `SELECT` sobre la intención `269730d670964950b09ce6d300b8a024`.

Resultado físico confirmado:

- intención e item en `Completada`;
- un item almacenado y con expediente;
- inscripción y caché de expediente confirmadas;
- dos imágenes relacionadas, sin duplicados;
- destino, relación, caché, índice de gabinete, índice SQL, XML y reconciliación confirmados para ambas imágenes;
- caché documental consistente con el universo relacionado;
- sin códigos de error;
- transiciones `Reconciliada` y `Completada` presentes.

Inconsistencia detectada:

- `workflow_import_intent_item.storage_status = Confirmado`;
- `workflow_import_intent_item.cache_status = Confirmado`;
- `workflow_import_intent_item.relation_status` no quedó en `Confirmado`;
- `workflow_import_intent_item.index_status` no quedó en `Confirmado`.

Causa localizada: `ImportServiceOrchestrator.Execute` recibe un `PlanDocumentosRelacionadosImportacion` completamente confirmado, pero avanza inmediatamente cada item a `Reconciliada` y `Completada` sin proyectar al agregado del item los estados confirmados de relación e índice. `MySqlImportIntentRepository.ActualizarTransicion` persiste correctamente el estado en memoria, por lo que conserva los valores anteriores. La E2E valida los efectos físicos DTO de documentos relacionados, pero no exige actualmente la coherencia de las columnas agregadas del item.

Veredicto: la mutación física fue exitosa, pero la prueba no puede aceptarse como cierre DOC-67 hasta corregir la proyección/persistencia del agregado y añadir una aserción previa a E2E que impida completar con estos estados sin confirmar.

Corrección aplicada el 2026-09-17: el orquestador proyecta `EstadoRelacion`, `EstadoIndice` y `EstadoCache = Confirmado` únicamente después de recibir el plan físico completo confirmado y antes de persistir la transición `Reconciliada`. La consulta de control E2E de items incluye ahora expediente y los cuatro estados agregados. Se añadió una regresión focal y una condición al readiness general. Evidencia local: 327 pruebas aprobadas, 0 fallidas y compilación VB.NET correcta.

## Conflicto falso de caché ESAL 220572 — 2026-09-17

La matrícula normalizada `508663` del gabinete ESAL tenía una única fila de caché: expediente `578`, radicado histórico `S002194199`. La nueva operación usaba el radicado `S002194198`. No existían filas ni expedientes duplicados. `MySqlImportExpedientCacheRepository.SameDestination` comparaba tanto expediente como radicado y produjo `EXPEDIENT_CACHE_CONFLICT` pese a que la identidad física apuntaba al mismo expediente.

Regla corregida: la caché `(Matrícula normalizada, gabinete)` determina el expediente; un radicado nuevo no cambia esa identidad. Si el expediente coincide se reutiliza, se conserva la fila histórica y `EnsureRadicadoRelation` agrega idempotentemente `(expediente, nuevo radicado)` en `ra_relacion_radicado_externo_expediente`. Solo un expediente distinto o una identidad duplicada produce conflicto. Si no existe caché ni expediente físico, el flujo crea y postverifica exactamente un expediente.
# 2026-09-17 — Falso negativo del verificador tras ejecución ESAL correcta

La ejecución autorizada de la tarea ESAL `220572` terminó sin códigos
funcionales en creación, ejecución, consulta o reconciliación. La auditoría
autoritativa exclusivamente `SELECT` confirmó un item almacenado y completado,
dos documentos relacionados únicos, expediente, inscripción, relación, caché,
índices SQL/XML, transición de reconciliación y ausencia de errores
(`DOC67_EVIDENCE_VERDICT=PASSED`).

El artefacto E2E había dejado `DOC67-E2E-07/08` bloqueadas y
`DOC67-E2E-10` fallida por tres defectos del verificador, no del proceso físico:

1. buscaba `ExpedientEffects`, pero los envelopes de ejecución, consulta y
   reconciliación no publicaban la colección 1.1 ya modelada;
2. esperaba el estado interno `Reconciliada|Completada`, mientras el contrato
   público agrega items confirmados como `Completado`;
3. comparaba la cantidad de efectos contra `sampleSize` (items seleccionados),
   aunque un item puede descubrir varios documentos por `ENLASE`.

La corrección aditiva publica y proyecta todos los `ExpedientEffects`, acepta
`Completado` y valida que el universo de efectos no esté vacío y que todos sus
elementos cumplan la postcondición, sin igualarlo a `sampleSize`. Una prueba
focal reproduce un item y dos documentos relacionados. Evidencia local posterior:
332 pruebas `ImportarServicioWeb` aprobadas, 0 fallidas; compilación VB.NET con
0 errores. La tarea `220572` permanece consumida y no debe reutilizarse para
otra ejecución mutadora.

## 2026-09-17 — Reejecución ESAL posterior a la corrección

Una nueva muestra descartable ESAL confirmó la corrección de extremo a extremo.
La plataforma terminó con `success=true`, sin códigos funcionales en creación,
ejecución, consulta o reconciliación. Las aserciones `DOC67-E2E-07`, `08`, `09`
y `10` quedaron aprobadas, incluido el universo de efectos publicado por el DTO.

La auditoría posterior, ejecutada exclusivamente mediante `SELECT`, confirmó:

- un item almacenado, asociado a expediente y completado;
- dos documentos relacionados únicos;
- almacenamiento, relación, índice y caché del item confirmados;
- inscripción y expediente confirmados;
- universo relacionado, caché documental e índices SQL/XML reconciliados;
- transición de reconciliación presente y cero errores funcionales;
- veredicto autoritativo `DOC67_EVIDENCE_VERDICT=PASSED`.

El gate terminó en `false`, con usuarios y grupos vacíos, y el recurso quedó
marcado como consumido. Esta evidencia cierra la muestra ESAL; MERCANTIL y RUP
siguen pendientes para completar la matriz E2E de DOC-67.

## 2026-09-17 — Ejecución MERCANTIL confirmada

La primera corrida MERCANTIL se detuvo correctamente antes del almacenamiento
porque la creación de expediente estaba deshabilitada para el trámite. La
plataforma liberó el recurso y restauró el gate. Después de que el responsable
habilitó la configuración, se reejecutó la misma muestra descartable.

La segunda corrida terminó con `success=true`, sin códigos funcionales en
creación, ejecución, consulta o reconciliación. Las aserciones de relación,
caché, índices, fase final y reconciliación quedaron aprobadas. La auditoría
posterior exclusivamente `SELECT` confirmó un item almacenado y completado, dos
documentos relacionados únicos, expediente e inscripción confirmados, todos los
efectos reconciliados, caché consistente, cero errores y
`DOC67_EVIDENCE_VERDICT=PASSED`.

El recurso terminó consumido y el gate quedó en `false`, con alcance vacío. La
muestra MERCANTIL queda cerrada; solo RUP continúa pendiente en la matriz E2E.

## 2026-09-17 — Ejecución RUP confirmada

La muestra descartable RUP usó la tipología autorizada resuelta desde la lista
de chequeo del trámite (`Constancia De Inscripción`). La corrida terminó con
`success=true`, sin códigos funcionales en creación, ejecución, consulta o
reconciliación. Las aserciones de relación, caché, índices, fase final y
reconciliación quedaron aprobadas.

La auditoría posterior exclusivamente `SELECT` confirmó un item almacenado y
completado, tres documentos relacionados únicos, expediente e inscripción
confirmados, todos los efectos reconciliados, caché consistente, transición de
reconciliación, cero errores y `DOC67_EVIDENCE_VERDICT=PASSED`. El recurso
terminó consumido y el gate quedó en `false`, con usuarios y grupos vacíos.

Con esta evidencia queda completa la matriz de ejecución normal por tipo de
registro: ESAL, MERCANTIL y RUP. Posteriormente también se aprobaron concurrencia
y reintento controlado. La tarea OpenSpec 6.4 continúa abierta por recovery y por
la evidencia real multi-expediente condicionada al contrato del proveedor.
## Cobertura multi-expediente previa a retry/recovery/concurrencia — 2026-09-17

Se identificó que el perfil histórico `doc56-import-sii-multidocument.profile.example.json`
solo exigía `sampleSize=3`. Esa condición demuestra varios documentos, pero no demuestra
que la intención afecte varios expedientes: los documentos podrían terminar en una sola
inscripción y un único destino.

Se añadió al contrato del perfil `minimumExpedientCount=2`. Cuando se informa, el adaptador
exige en la evidencia reconciliada:

- al menos dos `ExpedientId` positivos y distintos;
- al menos dos `InscriptionKey` no vacíos y distintos;
- efectos de relación, caché e índices confirmados por el flujo normal;
- tres documentos persistidos para el perfil de referencia actual.

La aserción `DOC67-E2E-01` deja de aprobarse por el solo hecho de que `sampleSize > 1` y
únicamente se aprueba cuando el universo reconciliado confirma los destinos e inscripciones
distintos. Los fallos seguros son
`IMPORT_E2E_MULTIEXPEDIENT_EFFECTS_MISSING`,
`IMPORT_E2E_MULTIEXPEDIENT_DESTINATIONS_UNCONFIRMED` e
`IMPORT_E2E_MULTIEXPEDIENT_INSCRIPTIONS_UNCONFIRMED`.

La prueba focal aprueba un universo con dos inscripciones/destinos y rechaza dos efectos
que apuntan al mismo expediente; resultado: 12/12. La E2E real queda pendiente de un recurso
descartable no consumido que realmente contenga varias inscripciones. No debe reutilizarse la
tarea `219887` como evidencia porque ya fue consumida y su perfil no garantiza el universo.

## Reintento controlado y expectativa idempotente — 2026-09-17

La tarea descartable `220562` creó la intención
`beab06593c044bcda92b1d33ff7db62c`. La detención solicitada quedó persistida después de
resolver y confirmar el expediente, pero el orquestador intentó construir el universo de
documentos relacionados antes de retornar y falló con
`RELATED_DOCUMENTS_WAITING_FOR_STORAGE`. La causa era el orden de la saga: una detención es
un checkpoint recuperable y los relacionados solo pueden ejecutarse después del
almacenamiento.

Se agregó un retorno temprano cuando `DetencionSolicitada=True`, conservando la versión y
los items detenidos. También se endureció el verificador para aceptar explícitamente
`EXECUTION_STOPPED` solo si `Retryable=True`, `PersistenceKnown=True` y no existe
`DocumentId`. Las pruebas focales demostraron tanto la preparación como la reanudación de
una intención persistida.

El reintento real confirmó el almacenamiento de un único documento y una segunda ejecución
con el token obsoleto devolvió `VERSION_CONFLICT`. Las aserciones `DOC67-E2E-11/12/13`
quedaron aprobadas. El control `import-expedient-state` no cambió porque el expediente ya
estaba confirmado; exigir su modificación contradecía la idempotencia. La política fue
corregida para exigir esa huella sin cambios y cambios en los otros seis controles.

La auditoría autoritativa posterior, exclusivamente `SELECT`, confirmó un item completo,
dos relacionados únicos, todos los estados de almacenamiento, relación, caché e índices
confirmados, reconciliación presente, cero errores y
`DOC67_EVIDENCE_VERDICT=PASSED`. El gate final quedó en `false`, con usuarios y grupos
vacíos, y las páginas legacy no fueron modificadas.

## Recovery autoritativo de solo lectura — 2026-09-17

Sobre la misma intención completada se ejecutó `import-sii-recovery`. El adaptador invocó
únicamente `GetImportIntent`; no creó intención, no ejecutó la saga y no reconcilió otra
vez. La respuesta confirmó un documento disponible y el control de integridad posterior
demostró que las siete huellas `SELECT` permanecieron idénticas.

El artefacto saneado registró `success=true`, código `get=null`, conteo `1` y las
aserciones `DOC67-E2E-16/17/18` en estado `passed`. El gate fue restaurado a `false`, con
alcance vacío. Con esta corrida quedan cubiertos ejecución, retry, concurrencia y recovery;
la única evidencia E2E aún pendiente es el caso real multi-expediente sujeto a la respuesta
del proveedor SII.
