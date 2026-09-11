<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Context

DOC-56 consolida la validación del backend de importación de B01–B06. El E2E reveló además brechas productivas en el contrato SII y en la identidad de tipología; se corrigen sin ampliar autorización operativa.

## Goals / Non-Goals

**Goals**
- Validar contratos, seguridad, compatibilidad, idempotencia, estados y reconciliación.
- Proteger fronteras legacy y demostrar ausencia de efectos con el gate apagado.
- Preparar validación local y E2E en rutas canónicas.
- Corregir la brecha productiva revelada por el E2E entre las rutas REST supuestas por DOC-55 y el contrato SII real por radicado/anexo.

**Non-Goals**
- Ejecutar E2E real, carga, SII o mutaciones de ambiente.
- Modificar `ClassAlmacenamiento.vb`, JavaScript o ASMX históricos.
- Crear arneses, fixtures, login, Playwright o secretos paralelos.

## Decisions

### D-01 — Activos existentes
Las suites integrales viven en `Tests/` y reutilizan tests y fixtures B01–B06 sin copiarlos.

La documentación técnica se deriva de los símbolos, contratos, consultas y tablas implementados; no se acepta un resumen conceptual como sustituto. Los diagramas se segmentan por frontera, consulta, intención, ejecución, estados, reconciliación y persistencia cuando un único gráfico no pueda conservar todas las llamadas y decisiones.

### D-02 — Integración local determinista
Transporte, concurrencia, fallos y persistencia usan datos locales controlados, sin red, autenticación ni secretos.

### D-03 — Invariantes legacy
La suite caracteriza que `JSProgresBar` no inicia efectos, el adaptador concentra la única llamada nueva a `AlmacenaDocumentoTareaWorkflow(...)` y `ClassAlmacenamiento.vb` no cambia.

### D-04 — Contratos y gate ASMX
Las ocho operaciones usan fixtures compartidos. Con el gate apagado responden `FEATURE_DISABLED` antes de ejecutar dependencias; las rutas legacy permanecen.

### D-05 — Validador único
`tools/validation/Verify-ImportarServicioWebModern.ps1` ejecuta las suites locales, resume y conserva el código de salida.

### D-06 — E2E no autorizado
La especificación usa `tools/e2e/tests/` y soporte existente. Crear la prueba no autoriza ejecutarla; evidencia local y autorizada se separan.

### D-07 — Composición productiva de las ocho operaciones
El ASMX publicará `PreflightImport`, `CreateImportIntent`, `ExecuteImportIntent`, `GetImportIntent` y `ReconcileImportIntent`. La autorización reutiliza exclusivamente `ADJUNTAR_IMAGENES_PREDETERMINADA` (índice 13), que es el único permiso comprobado por el flujo legacy de importación SII; `UTIL_SII_GETION_TAREA` se carga en sesión pero no autoriza esta operación. Gabinete, ruta y clase `DOCUMENTO ELECTRONICO` se resuelven en servidor. El frontend aporta el radicado y la tipología seleccionada como pareja inmutable `DocumentTypeId`/`DocumentTypeName`; `DocumentTypeId` significa exclusivamente `tipo_doc_series.Id_Tipo_Doc_Series` y no aporta metadatos de infraestructura.

Antes de aceptar el preflight y nuevamente antes de almacenar, el backend consulta Radicación y traduce `(IdTramite, DocumentTypeId)` a un único `ra_dig_tipos_docum_lista_chequeo.ID_TIPO_DOCUMENTAL_CHEQUEO`. También compara el nombre canónico con `DocumentTypeName`. Cero coincidencias, más de una o un nombre discordante fallan cerrados. Sólo el ID contextual resultante se entrega como `IdTipoListaChequeo` a `AlmacenaDocumentoTareaWorkflow`; ese ID nunca cruza el contrato público.

### D-08 — Contrato SII productivo por código de barras y anexo

La ruta moderna no usará los endpoints no demostrados `items`, `preview` ni `resource`. `QueryItems` ejecutará el contrato productivo de token y `consultarInformacionSello` usando `codigoBarras`, mapeará cada elemento de `inscripciones[].imagenes[]` y publicará una identidad opaca basada en código de barras, libro, registro e `idanexo`, nunca en URL. Preview y descarga volverán a consultar el sello y resolver el anexo del lado servidor, y descargarán exclusivamente la URL HTTP o HTTPS entregada por SII cuyo host esté en la allowlist versionada; no invocarán los ASMX legacy ni aceptarán URL desde el navegador. Una inscripción con varias imágenes produce un item por imagen y la ausencia de `inscripciones` o `imagenes` produce un resultado vacío explícito, no éxito ficticio.

La evidencia real autorizada identificó `repositoriosii.s3.amazonaws.com` como hostname de anexos SII. Se autoriza únicamente ese bucket junto con `confecamaras.co` y sus subdominios; no se autoriza de forma amplia `amazonaws.com`.

El almacenamiento conserva la semántica legacy de `PreAlmacenaConstanciaIsncripcionsSII`: el código de barras usado para consultar SII se separa del recibo SII resuelto desde la tarea. `CODBARRAS` recibe el primero; `ENLASE`, `RECIBOCAJA` y el radicado documental reciben el segundo. `LIBRO` elimina el prefijo registral, RUP usa `proponente` como matrícula y los nombres de fecha/descripción varían por gabinete. Los metadatos comunes de archivo, usuario, expediente, TRD y clase documental continúan siendo resueltos por `AlmacenaDocumentoTareaWorkflow`.

La bitácora DocuArchi conserva además snapshots opcionales de `TaskId`, `Radicado`, `CodigoBarras` y `ReferenciaProveedor`. No se crea FK hacia Workflow y ningún campo de negocio es obligatorio, de modo que proveedores sin código de barras pueden usar únicamente su referencia genérica. Para SII, consulta y preview registran la tarea y el código de barras derivados del request/contexto validado; la ejecución agrega radicado, intención e item persistidos.

## Risks / Trade-offs

- La caracterización de fuente puede requerir ajuste ante refactors legítimos; protege deliberadamente invariantes legacy.
- Los dobles locales no demuestran disponibilidad real de SII.
- `origin/main` de DOC-55 contiene rutas REST genéricas no respaldadas por el contrato legacy real; hasta completar D-08 la ruta moderna no es apta para producción aunque compile y pase fixtures.
- El cierre E2E queda pendiente hasta autorización explícita.
- Los identificadores de lista de chequeo son contextuales y pueden coincidir numéricamente con IDs TRD distintos; pasarlos sin traducción almacena una tipología incorrecta aunque la operación legacy responda éxito.

## Migration Plan

1. Agregar suites y validador sin tocar producción.
2. Preparar el E2E en `tools/e2e`, sin ejecutarlo.
3. Sustituir dentro del adaptador SII las rutas supuestas por token, `consultarInformacionSello`, recorrido de `inscripciones[].imagenes[]` y descarga segura por URL resuelta en servidor.
4. Crear documentación y evidencia saneada bajo `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-56-pruebas-backend-evidencia/`.
5. Ejecutar validación local y E2E autorizado manteniendo el gate seguro.

## Open Questions

- La ejecución E2E queda pendiente de autorización explícita para ambiente, cuentas y datos descartables.
