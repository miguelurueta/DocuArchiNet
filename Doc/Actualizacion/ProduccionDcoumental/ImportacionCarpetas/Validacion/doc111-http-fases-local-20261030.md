# DOC-111 — Fachada HTTP del coordinador

Fecha: 2026-10-30. HEAD base `b2e93b726f221868cf2b82d43200a8e568871117`, workspace con cambios locales previos. Tarea 10.11 completada localmente; 111/130. Sin commit ni despliegue.

## Contrato implementado

El ASMX existente añade tres WebMethods POST JSON con sesión, sin endpoints independientes de adaptadores:

| Método | Entrada cerrada | Alcance |
|---|---|---|
| `AvanzarLote` | `loteId`, `recursoId`, `fase`, `revisionEsperada`, `claveOperacion` | Una fase sobre un recurso persistido: CREAR_EXPEDIENTE sobre raíz; MATERIALIZAR_VALORES o ALMACENAR sobre archivo. |
| `SolicitarDetencion` | `loteId`, `revisionEsperada` | Señal de detención por el coordinador, sin cancelar el efecto en vuelo ni borrar resultados. |
| `ReconciliarRecurso` | `loteId`, `intentoId`, `revisionEsperada` | Verificación explícita del intento incierto conocido; nunca repite el escritor. |

Todos consumen el sobre `{ "solicitud": { ... } }` del cuerpo comprobado por el guard operativo. No aceptan autoridad, snapshot, ruta física, estado final impuesto ni IDs de efectos proporcionados por el cliente. Se conservan CSRF, origen, tipo/tamaño de cuerpo, rechazo de claves desconocidas/duplicadas, política servidor y validación de identidad vigente. El objeto entregado por el runtime ASMX no sustituye al cuerpo cerrado.

`CoordinadorImportacionCarpetas.Operaciones.vb` valida el vocabulario de fase con comparación binaria y delega exactamente una vez en el coordinador de la fase. No selecciona automáticamente otro recurso, encadena almacenamiento después de materializar ni inicia trabajo posterior autónomo. INCORPORAR no es una fase HTTP separada: el almacenamiento existente incorpora/verifica la relación dentro de su propio recorrido.

Para creación y almacenamiento la revisión corresponde a la raíz/archivo original del comando y se conserva al consultar su repetición con la misma clave. Materialización usa la revisión del archivo y su checkpoint de valores fijados; no crea un intento ni vuelve a consultar el proveedor en una repetición. Su campo `repeticion` se devuelve nulo, pues no se clasifica como nueva reserva. Detención y reconciliación usan revisión de lote. ConsultarLote/ConsultarRecursosLote siguen siendo las operaciones de lectura de revisión y estado agregado actuales.

La respuesta pública incluye fase/estado/código y los IDs conocidos, sin serializar `ResultadoEfectoImportacion`, valores/procedencia, evidencia de ciclo, tokens/hash de reclamación, staging o mensajes de excepciones. CONFIRMADO describe esa fase; materializar valores no acredita incorporación documental. INCIERTO conserva IDs públicos en `datos`, `ok=false`, código RESULTADO_INCIERTO y HTTP 409. Rechazo de valores devuelve ARCHIVO_RECHAZADO/422; los errores internos usan el mensaje saneado existente. No hay reenvío automático.

La página autorizada de recursos incorpora `intentoActivoId` cuando es una referencia numérica válida, sin su token, para recuperar una respuesta perdida. El ID no es una concesión de ejecución. Consultar sigue sin reconciliar ni modificar datos; las acciones anunciadas por la consulta continúan siendo conservadoras de lectura hasta integrar el cliente.

## Composición y autoridad

El parcial productivo `WebServiceImportacionCarpetas.Composicion.vb` selecciona las composiciones ya implementadas de creación, materialización, documento y reconciliación. Detención reutiliza el mismo repositorio, autoridad y conexiones de sesión, con comprobación del origen vigente. Las acciones internas DETENER_LOTE y RECONCILIAR se resuelven expresamente y exigen los permisos existentes de creación y carga del nivel para un usuario compartido, además de IMPORTAR_CARPETAS y gabinete/destino autorizados. No se agrega una columna de permisos ni se concede administración.

Las fases conservan sus propias barreras, autoridad inmediata, snapshot confirmado, idempotencia y comprobaciones del repositorio. La fachada no permite omitirlas. La función legacy de almacenamiento, creación legacy y consumidores SII/ImportarServicioWeb permanecen intactos. La exclusión adicional del XML frente a otras cargas continúa descartada por instrucción del usuario.

## Validación local

- PASS: arnés Framework `node --test tests/doc110-perfil-importacion-behavior.test.cjs`, incluyendo regresiones DOC-110/DOC-111 y contraste DDL de UPDATE de intentos.
- PASS: avance de creación/documento, IDs públicos y repetición durable sin otra reserva/writer; materialización aislada sin completar documento ni repetir proveedor; rechazo de valores con 422.
- PASS: detención, repetición sin otra señal y efecto posterior bloqueado; reconciliación positiva/repetida sin otro writer y evidencia insuficiente con 409 sin checkpoint.
- PASS: tres WebMethods con sesión; token ausente, GET y origen ajeno rechazados antes de componer una fase; cuerpo cerrado, fase desconocida/cambio de mayúsculas y revocación de IMPORTAR_CARPETAS.
- PASS: acciones DETENER_LOTE/RECONCILIAR rechazan permisos incompletos en nivel compartido; referencia numérica del intento activo recuperable mediante consulta autorizada, sin evidencia privada.
- PASS: compilación Debug completa en `.tmp/doc111-http-fases-build/`, sin reemplazar DLL del sitio; OpenSpec estricto y `git diff --check`.

Las pruebas invocan el ASMX con HttpContext sintético y coordinadores reales de las fixtures locales; PDF local y escritores/SQL/proveedores son dobles explícitos. El binding HTTP productivo se compila con el proyecto completo. En el arnés se sustituye solamente esa composición por `Doc111ComposicionHttpStub.vb`, que falla cerrado si no se inyecta un coordinador; no se registra en el vbproj productivo. Esto no acredita ejecución del binding legacy, IIS/ASMX autenticado, MySQL/rollback/carreras reales, XML ni almacenamiento/incorporación reales: **NO_EJECUTADO**.

La conexión del cliente a estas operaciones, actualización de la rama original y aceptación real permanecen pendientes. Esta evidencia no autoriza E2E ni efectos nuevos sobre un ambiente/cuenta.
