# DOC-85 — Corrección del contexto de radicado al adjuntar documentos

## Estado

Implementación, validación local y E2E real aprobadas el 2026-10-01. La corrida autorizada ejecutó el escenario positivo y el rechazo por discrepancia informativa sobre un recurso descartable, mediante consultas de evidencia exclusivamente `SELECT`. DOC-85 queda técnicamente validado.

## Diagnóstico y punto de ruptura

En `ADJUNTARADICACION`, el registro seleccionado ya contenía `consecutivo_radicado`, pero el recorrido histórico volvía a buscarlo mediante `Class_DAT_ADIC_TAR.SolicitaRadicadoTareaWorkflow`. Cuando la tabla dinámica no tenía fila, la columna era `NULL` o estaba vacía, ese método retornaba `YES` con `RadicadoTarea = ""`. `SolicitaDatosCamposIndiceGabinete` consumía inmediatamente el vacío en `SolicitaNombrePlantillaRadicado("")`; el fallo ocurría antes del fallback tardío.

Mensaje observado: `Imposible encontrar el radicado () en registro general, imposible determinar la plantilla de radicación`.

## Evidencia legacy funcional

Se revisó en modo de solo lectura `D:\imagenesda\GestorDocumental\Desarrollo\GestionDocumental-Docuarchi.net\copia\GestionDocumental-Docuarchi.net`. La rama equivalente obtenía el consecutivo del registro de estado y lo entregaba explícitamente al almacenamiento antes de consultar plantilla e índices. DOC-85 recupera esa semántica sin copiar la implementación ni modificar el repositorio de referencia.

## Diseño aplicado

- `MySqlContextoAdjuntoRadicacionRepository.ObtenerAutorizado` consulta una vez `ra_rad_estados_modulo_radicacion`, parametriza estado y usuario, y falla de forma cerrada.
- `ContextoAdjuntoRadicacion` conserva de forma inmutable estado, radicado, tarea, trámite, plantilla y destino conocido.
- `ServicioAdjuntoRadicacion.Adjuntar` coordina la resolución, valida el radicado informativo y solo entonces permite preparar el almacenamiento.
- `ConstruirDatosCamposIndiceGabineteConRadicado` recibe el radicado obligatorio, lo valida antes de consultar la plantilla y nunca llama `SolicitaRadicadoTareaWorkflow`.
- `PreAlmacenaDocumentosRadicacionConContexto` valida tarea, gabinete y plantilla, y llama una sola vez al `AlmacenaDocumentosRadicacion` existente.
- `UploadSaveFile` queda dividido en una sobrecarga legacy de diez argumentos y otra de doce argumentos exclusiva de `ADJUNTARADICACION`.

No se creó otro motor de almacenamiento ni se modificaron `AlmacenaDocumentosRadicacion` o `Almacenamiento`.

## Flujo anterior

```text
[Cliente: Radicación Simplificada]
        |
        v
[FileUploadHandler_.ashx: 12 argumentos]
        |
        v
[UploadSaveFile]
        |
        v
[Registro de estado: consecutivo correcto]
        |
        v
[PreAlmacenaDocumentosRadicacion]
        |
        v
[SolicitaDatosCamposIndiceGabinete]
        |
        v
[SolicitaRadicadoTareaWorkflow / DAT_ADIC_TAR]
        |
        v
<¿DAT_ADIC_TAR tiene radicado?> -- no --> [Radicado vacío]
        |                                      |
       sí                                      v
        |                         [Consulta de plantilla con ""]
        v                                      |
[Consulta de plantilla]                        v
                                      [Error antes del fallback]
```

## Flujo corregido

```text
[Cliente: ID de estado + radicado informativo]
        |
        v
[FileUploadHandler_.ashx sin cambios]
        |
        v
[UploadSaveFile: sobrecarga de 12 argumentos]
        |
        v
[ServicioAdjuntoRadicacion]
        |
        v
[Repository: SELECT por estado + usuario]
        |
        v
<¿Existe, pertenece y tiene identidad completa?> -- no --> [Rechazo sin almacenar]
        |
       sí
        v
[ContextoAdjuntoRadicacion inmutable]
        |
        v
<¿Radicado informativo no vacío y diferente?> -- sí --> [Rechazo sin almacenar]
        |
       no
        v
[PreAlmacenaDocumentosRadicacionConContexto]
        |
        v
[ConstruirDatosCamposIndiceGabineteConRadicado]
        |
        v
[AlmacenaDocumentosRadicacion existente]
        |
        v
[Almacenamiento existente]
        |
        v
[stru_datos_image_lista -> uploadFiles]
        |
        v
[Inserción JavaScript de la fila, sin recarga]
```

## Contrato de entrada y validaciones

La llamada de doce argumentos exige `IdRegistroEstadoRadicacion > 0`. `RadicadoRadicacion` es informativo: vacío es válido, pero un valor no vacío diferente del autoritativo se rechaza. El servidor exige sesión válida, pertenencia al usuario de Radicación, consecutivo, tarea, trámite y plantilla válidos, configuración documental compatible, tipología cuando sea obligatoria, ruta y gabinete de la tarea.

Errores de identidad, autorización o discrepancia terminan antes del almacenamiento. Las excepciones de repository y servicio se traducen a mensajes funcionales sin exponer conexión ni SQL.

La composición productiva reutiliza la infraestructura transversal existente: `ModuleSessionConnectionStringResolver.Resolve(..., "RA_")` centraliza la configuración de sesión que también consume Workflow, `RadicacionModuleConnectionFactory` abre la conexión y `AdoNetDataExecutor` ejecuta la consulta. El repository solo acepta esas dependencias por constructor; no contiene una fábrica privada, no reconstruye credenciales y no depende de `HttpContext`.

## Compatibilidad y no regresión

| Superficie | Garantía | Evidencia |
| --- | --- | --- |
| Handler compartido | Sin cambios | SHA-256 protegido por prueba |
| `FileUploadHandler.js` | Inserción nueva limitada a `evento_adjunta == "ADJUNTARADICACION"`; recorrido de versiones intacto en `else` | Prueba contractual y E2E real |
| JavaScript de Radicación Simplificada | Sin cambios | SHA-256 protegido por prueba |
| Siete llamadas legacy | Conservan 10 argumentos | Inventario sintáctico balanceado |
| Radicación Simplificada | Única llamada de 12 argumentos | Inventario y compilación |
| Gestión de respuestas, Workflow y Producción | Permanecen en sobrecarga legacy | Compilación y pruebas estructurales |
| Enlace, SII y sellos | Sin cambios de implementación | Matriz focal: 128 pruebas aprobadas |
| Bootstrap Table compartido | Contrato global conservado | Suite `bootstrap-table-global-contract` aprobada |
| Interfaz del adjunto | Conserva `uploadFiles` e inserción sin recarga | Prueba contractual local y E2E integrada |

## Archivos de implementación y pruebas

- `Modelo/RadicacionSimplificada/Adjuntos/*`
- `Infrastructure/Repositories/RadicacionSimplificada/Adjuntos/MySqlContextoAdjuntoRadicacionRepository.vb`
- `Services/RadicacionSimplificada/Adjuntos/ServicioAdjuntoRadicacion.vb`
- `Docuarchi/ClassDaGabinete.vb`
- `workflow/ClassAlmacenamiento.vb`
- `generic_control/FileUploadHandler.js` (proyección aislada de `ADJUNTARADICACION`)
- `Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb`
- `webservice/WorkflowPreviewSessionContextGate.vb` (delega su resolución de conexión a la infraestructura compartida)
- `GestionDocumental-Docuarchi.net.vbproj`
- `tests/radicacion-simple-attachment-*.test.cjs`
- `tests/Fixtures/RadicacionSimplificada/ServicioAdjuntoRadicacionHarness.vb`
- `tools/e2e/tests/radicacion-simple-attachment.spec.cjs`
- `tools/e2e/scripts/assert-radicacion-simple-attachment-config.cjs`
- `tools/e2e/scripts/run-radicacion-simple-attachment-interactive.cjs`
- `tools/e2e/profiles/radicacion-simple-attachment.profile.example.json`
- `tools/e2e/fixtures/doc85-constancia-inscripcion*.pdf`
- `tools/e2e/package.json`

## Evidencia local saneada

| Verificación | Resultado 2026-10-01 |
| --- | --- |
| Repository y servicio reales compilados con infraestructura inyectada | Aprobado: SQL/parametrización, negativos, discrepancia, informativo vacío, resolución única, inmutabilidad, aislamiento y composición con infraestructura compartida |
| Suite completa Node (incluye foco, Enlace/SII/sellos/Bootstrap/Workflow) | 712 aprobadas, 0 fallidas |
| E2E Playwright DOC-85 | 1 aprobada; escenarios positivo y negativo en 13,2 s (14,1 s total) |
| Compilación de la solución | Correcta, 0 errores, 1 advertencia |
| Fronteras compartidas | Handler y JavaScript del módulo sin cambios; proyección compartida aislada por evento y estable durante la corrida |

Comandos reproducibles:

```powershell
node --test tests/radicacion-simple-attachment-*.test.cjs tests/bootstrap-table-global-contract.test.cjs
msbuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m "/clp:ErrorsOnly;Summary"
node tools/e2e/node_modules/@playwright/test/cli.js test tools/e2e/tests/radicacion-simple-attachment.spec.cjs --list
git diff --check
```

## E2E real autorizada

La suite positiva verifica precondición `DAT_ADIC_TAR` vacía, POST real al handler, contrato `uploadFiles`, persistencia única, tipología, formato, icono, acciones e inserción sin navegación/postback. La negativa altera únicamente `radicado_radicacion` dentro del multipart, conserva el ID autoritativo y exige rechazo sin fila ni persistencia.

Comando autorizado:

```powershell
npm.cmd --prefix tools/e2e run test:radicacion-simple:attachment -- --profile <perfil-runtime-no-sensible.json>
```

Resultado aprobado el 2026-10-01. La evidencia saneada registró `DAT_ADIC_TAR` sin radicado, contrato `uploadFiles` conservado, inserción JavaScript sin recarga y rechazo de la discrepancia informativa. En la corrida final el conteo pasó de 3 a 4 tras el positivo y permaneció en 4 tras el negativo. Los documentos de intentos positivos anteriores permanecen en el recurso descartable: el runner no ejecuta limpieza mutante.

Las credenciales se capturaron de forma efímera y no se persistieron. Las consultas de control fueron `SELECT`. Las claves configurables de `WorkflowCentroTrabajoModernActive`, usuarios y grupos están ausentes de `Web.config`, por lo que el gate oficial no fue reintroducido ni activado.

## Reversa

La reversa se limita a retirar los componentes DOC-85, restaurar las dos firmas/métodos adaptados y retirar la bifurcación JavaScript exclusiva de `ADJUNTARADICACION`. No requiere migración de esquema ni cambios en el handler. Un documento creado por una E2E autorizada es un recurso descartable consumido y no se elimina mediante las consultas de evidencia.
