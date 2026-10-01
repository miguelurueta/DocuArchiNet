# Exploración consolidada — Contexto de radicado al adjuntar documentos

## Estado de esta guía

Este documento contiene la decisión técnica oficial para corregir la pérdida del radicado al adjuntar documentos desde Radicación Simplificada. Es la guía de entrada para el agente de implementación.

No contiene alternativas descartadas. La implementación debe respetar las fronteras, nombres y restricciones aquí consolidados y contrastarlos con las firmas reales antes de editar.

## Objetivo

Conseguir que `ADJUNTARADICACION` utilice como radicado autoritativo el `consecutivo_radicado` del registro seleccionado en `ra_rad_estados_modulo_radicacion`, incluso cuando el campo correspondiente de `DAT_ADIC_TAR<ruta>` no exista, sea `NULL` o esté vacío.

El mismo radicado autoritativo debe llegar, sin una segunda resolución, a:

- la consulta de plantilla;
- la construcción de índices del gabinete;
- el almacenamiento documental;
- la estructura devuelta a la interfaz.

## Frontera aprobada

Los siguientes archivos compartidos deben permanecer sin cambios:

```text
generic_control/FileUploadHandler_.ashx.vb
generic_control/FileUploadHandler.js
js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js
```

La infraestructura actual ya transporta para `ADJUNTARADICACION`:

```text
id_registro_estado_radicacion
radicado_radicacion
```

El handler ya identifica la rama, conserva el archivo temporal, invoca `UploadSaveFile(...)` con doce argumentos y mapea `stru_datos_image_lista` hacia la respuesta `uploadFiles`. La corrección comienza después de esa llamada y no requiere modificar el Controller compartido.

También deben permanecer intactos los recorridos de:

```text
GESTION_RESPUESTA
WORKFLOWSELECCION
PRODUCCION
WORKFLOWENLACE
ENLASE_RADICADO
integración SII
importación de sellos
```

## Diagnóstico confirmado

El recorrido defectuoso es:

```text
ADJUNTARADICACION
  -> UploadSaveFile(...)
  -> SolicitaDatosEstructuraEstadoRadicado(...)
  -> StruRegistroEstado.consecutivo_radicado contiene el valor correcto
  -> PreAlmacenaDocumentosRadicacion(...)
  -> SolicitaDatosCamposIndiceGabinete(...)
  -> SolicitaRadicadoTareaWorkflow(...)
  -> DAT_ADIC_TAR devuelve YES con Radicado=""
  -> SolicitaNombrePlantillaRadicado("")
  -> error antes de regresar al fallback tardío
```

El fallback actual de `PreAlmacenaDocumentosRadicacion(...)` se ejecuta después de la resolución histórica y no puede corregir un error ocurrido dentro de `SolicitaDatosCamposIndiceGabinete(...)`.

Adicionalmente, no es válido utilizar `radicado_radicacion` recibido del navegador cuando `consecutivo_radicado` está vacío. El dato del cliente solo sirve para detectar una pantalla desactualizada; nunca autoriza ni sustituye la identidad persistida.

## Fuente autoritativa

La única cadena de autoridad para esta operación es:

```text
IdRegistroEstado
        |
        v
ra_rad_estados_modulo_radicacion
        |
        v
consecutivo_radicado
        |
        +--> plantilla
        +--> índices
        +--> almacenamiento
        +--> respuesta
```

Para `ADJUNTARADICACION`, queda prohibido redescubrir o reemplazar el radicado desde:

- `DAT_ADIC_TAR<ruta>`;
- una variable de sesión;
- `RA_RADICADO_REGISTRO` como única fuente;
- `DG_RADICADO` como única fuente;
- texto, etiqueta o fila recibidos del navegador.

## Inventario de símbolos

### Símbolos existentes que se reutilizan

```text
FileUploadHandler_.ashx.vb / ProcessRequest
ClassAlmacenamiento.UploadSaveFile
ClassAlmacenamiento.AlmacenaDocumentosRadicacion
ClassAlmacenamiento.Almacenamiento
ClassDaGabinete.SolicitaDatosCamposIndiceGabinete
Class_DAT_ADIC_TAR.SolicitaRadicadoTareaWorkflow
stru_datos_image_lista
uploadFiles
```

`AlmacenaDocumentosRadicacion(...)` y `ClassAlmacenamiento.Almacenamiento(...)` continúan siendo la preparación final y la escritura física. No se creará un segundo motor de almacenamiento.

### Símbolos nuevos aprobados

```text
ContextoAdjuntoRadicacion
ResultadoAdjuntoRadicacion

IContextoAdjuntoRadicacionRepository
  -> ObtenerAutorizado(...)

MySqlContextoAdjuntoRadicacionRepository
  -> ObtenerAutorizado(...)

ServicioAdjuntoRadicacion
  -> Adjuntar(...)

ClassAlmacenamiento.PreAlmacenaDocumentosRadicacionConContexto(...)

ClassDaGabinete.ConstruirDatosCamposIndiceGabineteConRadicado(...)
```

Los nombres anteriores son la convención oficial de esta corrección. Si una limitación real de compilación exige variar un nombre, la desviación debe quedar justificada en el paquete documental del ticket y conservar exactamente la misma responsabilidad.

## Contrato del contexto

`ContextoAdjuntoRadicacion` debe ser construido en servidor y exponer propiedades de solo lectura. Como mínimo contiene:

```text
IdRegistroEstado
Radicado
IdTareaWorkflow
IdTipoTramite
IdPlantilla
NombreGabinete, cuando pueda resolverse en la misma operación autorizada
```

El contexto no se almacena como autoridad en `Session`, variables estáticas ni estado de página. Cada POST resuelve su propio contexto para impedir mezcla entre archivos, pestañas o registros.

## Sobrecargas oficiales de UploadSaveFile

La firma actual con dos parámetros opcionales debe separarse en dos sobrecargas inequívocas.

### Sobrecarga legacy de diez argumentos

Conserva el contrato histórico empleado por los demás eventos:

```vb
Function UploadSaveFile(
    ByVal IdExpediente As Integer,
    ByVal IdTipoChek As Integer,
    ByVal DescripcionTipoDocumento As String,
    ByVal EstadoChekAdjuntoAnexo As Integer,
    ByVal EstadoChekRelacionado As Integer,
    ByVal NumeroDocRelacionado As Integer,
    ByVal FechaCarga As String,
    ByRef StruDatosImageLista As stru_datos_image_lista,
    ByRef IdTareaWorkflow As Long,
    ByRef Contador As String
) As String
```

### Sobrecarga exclusiva de doce argumentos

La llamada que ya existe en `FileUploadHandler_.ashx.vb` resuelve esta sobrecarga sin cambiar el handler:

```vb
Function UploadSaveFile(
    ByVal IdExpediente As Integer,
    ByVal IdTipoChek As Integer,
    ByVal DescripcionTipoDocumento As String,
    ByVal EstadoChekAdjuntoAnexo As Integer,
    ByVal EstadoChekRelacionado As Integer,
    ByVal NumeroDocRelacionado As Integer,
    ByVal FechaCarga As String,
    ByRef StruDatosImageLista As stru_datos_image_lista,
    ByRef IdTareaWorkflow As Long,
    ByRef Contador As String,
    ByVal IdRegistroEstadoRadicacion As Long,
    ByVal RadicadoRadicacion As String
) As String
```

Los dos argumentos adicionales son obligatorios y exclusivos de esta sobrecarga. No se conservarán como opcionales.

Resolución esperada:

```text
llamada de 10 argumentos
        -> UploadSaveFile legacy

llamada de 12 argumentos
        -> UploadSaveFile de Radicación Simplificada
        -> ServicioAdjuntoRadicacion.Adjuntar(...)
```

La sobrecarga de doce argumentos valida defensivamente que el evento de sesión continúe siendo `ADJUNTARADICACION`; una inconsistencia falla cerrada.

## Flujo técnico oficial

```text
+--------------------------------------------------------------+
| CLIENTE                                                      |
| FileUploadHandler.js                                         |
| SIN CAMBIOS                                                  |
|                                                              |
| Envía evento, archivo, tipología, ID del registro de estado |
| y radicado visible exclusivamente informativo.               |
+-----------------------------+--------------------------------+
                              |
                              v
+--------------------------------------------------------------+
| CONTROLLER                                                   |
| FileUploadHandler_.ashx.vb / ProcessRequest                  |
| SIN CAMBIOS                                                  |
|                                                              |
| Conserva la llamada actual de doce argumentos.               |
+-----------------------------+--------------------------------+
                              |
                              v
+--------------------------------------------------------------+
| ADAPTADOR DE ENTRADA                                         |
| UploadSaveFile, sobrecarga de doce argumentos                |
| NUEVA                                                        |
+-----------------------------+--------------------------------+
                              |
                              v
                    +-------------------------+
                    | ¿ID de estado > 0 y    |
                    | evento consistente?     |
                    +------------+------------+
                            sí   |   no
                                 |    +------> error funcional
                                 v
+--------------------------------------------------------------+
| SERVICE                                                      |
| ServicioAdjuntoRadicacion.Adjuntar                           |
| NUEVO                                                        |
+-----------------------------+--------------------------------+
                              |
                              v
+--------------------------------------------------------------+
| REPOSITORY                                                   |
| MySqlContextoAdjuntoRadicacionRepository.ObtenerAutorizado   |
| NUEVO                                                        |
|                                                              |
| SELECT parametrizado de ra_rad_estados_modulo_radicacion.    |
+-----------------------------+--------------------------------+
                              |
                              v
                    +-------------------------+
                    | ¿Registro existente y  |
                    | autorizado?             |
                    +------------+------------+
                            sí   |   no
                                 |    +------> error funcional
                                 v
                    +-------------------------+
                    | ¿consecutivo_radicado  |
                    | no está vacío?          |
                    +------------+------------+
                            sí   |   no
                                 |    +------> error funcional
                                 v
+--------------------------------------------------------------+
| MODELO                                                       |
| ContextoAdjuntoRadicacion inmutable                          |
| NUEVO                                                        |
+-----------------------------+--------------------------------+
                              |
                              v
                    +-------------------------+
                    | ¿El cliente informó un |
                    | radicado diferente?     |
                    +------------+------------+
                            no   |   sí
                                 |    +------> contexto desactualizado
                                 v
+--------------------------------------------------------------+
| PREPARACIÓN ESPECÍFICA                                      |
| PreAlmacenaDocumentosRadicacionConContexto                   |
| NUEVA                                                        |
|                                                              |
| Usa tarea, trámite, plantilla y radicado del contexto.       |
| No llama SolicitaRadicadoTareaWorkflow.                      |
+-----------------------------+--------------------------------+
                              |
                              v
                    +-------------------------+
                    | ¿Tipología obligatoria,|
                    | tarea y destino válidos? |
                    +------------+------------+
                            sí   |   no
                                 |    +------> error funcional
                                 v
+--------------------------------------------------------------+
| CONSTRUCTOR DE ÍNDICES                                      |
| ConstruirDatosCamposIndiceGabineteConRadicado                |
| NUEVO                                                        |
|                                                              |
| Recibe Contexto.Radicado como dato obligatorio.              |
| Consulta plantilla y construye los campos sin redescubrirlo. |
+-----------------------------+--------------------------------+
                              |
                              v
                    +-------------------------+
                    | ¿Plantilla, gabinete e  |
                    | índices compatibles?     |
                    +------------+------------+
                            sí   |   no
                                 |    +------> error funcional
                                 v
+--------------------------------------------------------------+
| ALMACENAMIENTO EXISTENTE                                     |
| AlmacenaDocumentosRadicacion                                 |
| SIN CAMBIO DE CONTRATO                                       |
+-----------------------------+--------------------------------+
                              |
                              v
+--------------------------------------------------------------+
| PERSISTENCIA EXISTENTE                                       |
| ClassAlmacenamiento.Almacenamiento                           |
| SIN CAMBIOS                                                  |
+-----------------------------+--------------------------------+
                              |
                              v
                    +-------------------------+
                    | ¿Resultado = "YES"?     |
                    +------------+------------+
                            sí   |   no
                                 |    +------> mensaje funcional seguro
                                 v
+--------------------------------------------------------------+
| RESULTADO                                                    |
| ResultadoAdjuntoRadicacion + stru_datos_image_lista          |
+-----------------------------+--------------------------------+
                              |
                              v
+--------------------------------------------------------------+
| CONTROLLER                                                   |
| FileUploadHandler_.ashx.vb                                   |
| SIN CAMBIOS                                                  |
|                                                              |
| Ejecuta el mapeo actual hacia uploadFiles.                   |
+-----------------------------+--------------------------------+
                              |
                              v
+--------------------------------------------------------------+
| CLIENTE                                                      |
| Inserta la fila mediante JavaScript sin GridView, DataBind,  |
| postback ni recarga parcial o completa.                      |
+--------------------------------------------------------------+
```

## Responsabilidades por capa

### Controller existente

`FileUploadHandler_.ashx.vb` conserva:

- recepción multipart;
- validaciones generales del archivo;
- administración de la ruta temporal existente;
- llamada de doce argumentos;
- mapeo de `stru_datos_image_lista` a `uploadFiles`;
- serialización de la respuesta.

No incorpora consultas, autorización nueva ni composición adicional.

### Service nuevo

`ServicioAdjuntoRadicacion.Adjuntar(...)`:

- valida la solicitud de la operación;
- solicita al repository el contexto autorizado;
- compara el radicado informativo cuando fue enviado;
- rechaza cualquier discrepancia;
- coordina la preparación con contexto;
- traduce el retorno legacy a `ResultadoAdjuntoRadicacion`;
- no conoce controles Web Forms ni construye SQL.

### Repository nuevo

`MySqlContextoAdjuntoRadicacionRepository.ObtenerAutorizado(...)`:

- ejecuta una consulta parametrizada;
- obtiene el registro por `IdRegistroEstado`;
- comprueba pertenencia al usuario y contexto vigentes;
- obtiene tarea, trámite, plantilla y radicado;
- rechaza registros inexistentes, cruzados o con radicado vacío;
- materializa `ContextoAdjuntoRadicacion`;
- no recibe el radicado del navegador como filtro de autoridad.

### Adaptación legacy nueva

`PreAlmacenaDocumentosRadicacionConContexto(...)`:

- recibe el contexto completo;
- valida configuración de digitalización y tipología;
- confirma compatibilidad entre tarea, trámite, plantilla y gabinete;
- llama el constructor de índices con el radicado ya resuelto;
- reutiliza `AlmacenaDocumentosRadicacion(...)`;
- no consulta el registro de estado otra vez;
- no llama `SolicitaRadicadoTareaWorkflow(...)`.

### Constructor nuevo

`ConstruirDatosCamposIndiceGabineteConRadicado(...)`:

- exige un radicado no vacío;
- recibe la identidad de plantilla y gabinete validada;
- consulta las relaciones de campos existentes;
- asigna los datos de plantilla e índices;
- conserva el formato de fechas vigente;
- devuelve `CDcamposAsignaAlmacenamiento`;
- nunca sobrescribe el radicado recibido.

## Compatibilidad legacy

La ruta histórica permanece disponible:

```text
UploadSaveFile, sobrecarga de diez argumentos
  -> PreAlmacenaDocumentosRadicacion
  -> SolicitaDatosCamposIndiceGabinete
  -> SolicitaRadicadoTareaWorkflow
  -> DAT_ADIC_TAR
```

La nueva ruta queda aislada:

```text
UploadSaveFile, sobrecarga de doce argumentos
  -> ServicioAdjuntoRadicacion.Adjuntar
  -> MySqlContextoAdjuntoRadicacionRepository.ObtenerAutorizado
  -> ContextoAdjuntoRadicacion
  -> PreAlmacenaDocumentosRadicacionConContexto
  -> ConstruirDatosCamposIndiceGabineteConRadicado
  -> AlmacenaDocumentosRadicacion
```

`SolicitaDatosCamposIndiceGabinete(...)` y `SolicitaRadicadoTareaWorkflow(...)` conservan firma y semántica para sus consumidores actuales.

## Decisiones obligatorias de error

La operación falla antes del almacenamiento cuando:

- `IdRegistroEstadoRadicacion <= 0`;
- el evento de sesión no corresponde a `ADJUNTARADICACION`;
- el registro no existe;
- el registro no pertenece al contexto autorizado;
- `consecutivo_radicado` está vacío;
- el radicado informativo no vacío difiere del persistido;
- la tarea, trámite, plantilla o gabinete no son compatibles;
- la tipología obligatoria no fue seleccionada;
- no se pueden construir los índices;
- el almacenamiento no confirma `YES`.

Los errores devueltos al cliente son funcionales y no exponen SQL, rutas físicas, credenciales, cadenas de conexión ni excepciones técnicas.

## Archivos previstos de implementación

```text
workflow/ClassAlmacenamiento.vb
Docuarchi/ClassDaGabinete.vb

Modelo/RadicacionSimplificada/Adjuntos/
  ContextoAdjuntoRadicacion.vb
  ResultadoAdjuntoRadicacion.vb
  IContextoAdjuntoRadicacionRepository.vb

Services/RadicacionSimplificada/Adjuntos/
  ServicioAdjuntoRadicacion.vb

Infrastructure/Repositories/RadicacionSimplificada/Adjuntos/
  MySqlContextoAdjuntoRadicacionRepository.vb

GestionDocumental-Docuarchi.net.vbproj
tests/

tools/e2e/tests/
  radicacion-simple-attachment.spec.cjs

tools/e2e/scripts/
  assert-radicacion-simple-attachment-config.cjs
  run-radicacion-simple-attachment-interactive.cjs

tools/e2e/profiles/
  radicacion-simple-attachment.profile.example.json

tools/e2e/package.json
```

No se autoriza agregar cambios a `FileUploadHandler_.ashx.vb`, `FileUploadHandler.js` ni `Web_form_radicacion_simpilificada.js` para esta corrección.

## Pruebas obligatorias

### Integridad del Controller compartido

- Proteger la huella aprobada de `generic_control/FileUploadHandler_.ashx.vb` o caracterizar estructuralmente sus ramas.
- Fallar si se modifica la llamada de diez argumentos de cualquier evento legacy.
- Confirmar que la llamada existente de doce argumentos resuelve la sobrecarga exclusiva.

### Resolución autoritativa

- Registro válido y `DAT_ADIC_TAR` vacío: almacena con `consecutivo_radicado`.
- Registro inexistente: rechaza sin almacenamiento.
- Registro no autorizado: rechaza sin almacenamiento.
- `consecutivo_radicado` vacío: rechaza y no usa el navegador como fallback.
- Radicado informativo distinto: rechaza por contexto desactualizado.
- Radicado informativo vacío: continúa exclusivamente con el valor autoritativo.

### Construcción y almacenamiento

- La plantilla se consulta con `ContextoAdjuntoRadicacion.Radicado`.
- Los índices usan el mismo radicado.
- `PreAlmacenaDocumentosRadicacionConContexto` no referencia `SolicitaRadicadoTareaWorkflow`.
- La nueva ruta llama una sola vez `AlmacenaDocumentosRadicacion`.
- La respuesta conserva todos los campos actuales de `stru_datos_image_lista` y `uploadFiles`.
- Varios archivos resuelven contextos aislados y no mezclan rutas, registros, tareas o tipologías.

### No regresión

- `GESTION_RESPUESTA` conserva llamada y comportamiento.
- `WORKFLOWSELECCION` conserva llamada y comportamiento.
- `PRODUCCION` conserva llamada y comportamiento.
- `WORKFLOWENLACE` conserva llamada y comportamiento.
- ENLASE, SII e importación de sellos no referencian el contexto nuevo.
- Ejecutar `tests/bootstrap-table-global-contract.test.cjs`.
- Ejecutar las pruebas focales nuevas y la suite local completa aplicable.
- Compilar `GestionDocumental-Docuarchi.net.vbproj` con MSBuild.
- Ejecutar `git diff --check`.

Las pruebas locales son deterministas y no requieren red, autenticación real ni escrituras en bases de datos externas.

### E2E real obligatoria

La corrección requiere una prueba E2E real integrada al arnes oficial `tools/e2e`. Debe utilizar Playwright sobre la interfaz de Radicación Simplificada y reutilizar `tools/e2e/tests/support/authenticated-workflow-session.cjs`; no puede crear login, `.env`, cookies persistidas, proyecto Playwright ni transporte paralelos.

El comando oficial nuevo es:

```text
npm.cmd --prefix tools/e2e run test:radicacion-simple:attachment
```

El comando ejecuta un iniciador interactivo y un validador que fallan antes de abrir el navegador cuando falte cualquiera de estas autorizaciones:

- ambiente de pruebas;
- cuenta autenticada;
- registro de estado y tarea descartables;
- carga documental real;
- fixture y tipología autorizados;
- consultas de control con cuenta MySQL/ODBC de solo lectura.

El perfil de ejemplo contiene solamente configuración no sensible. Las cuentas y contraseñas se capturan de forma efímera y oculta; no se imprimen, guardan en `.env`, incorporan al perfil ni persisten con `setx`.

La E2E positiva recorre la operación real:

```text
sesión autenticada
  -> Radicación Simplificada
  -> selección del registro descartable autorizado
  -> modal/control real de adjuntos
  -> selección de fixture y tipología
  -> POST real a FileUploadHandler_.ashx.vb
  -> almacenamiento físico y documental
  -> respuesta uploadFiles
  -> inserción de la fila mediante JavaScript
```

Precondiciones y comprobaciones obligatorias:

- `ra_rad_estados_modulo_radicacion.consecutivo_radicado` contiene el radicado esperado;
- el campo correspondiente de `DAT_ADIC_TAR` está ausente, `NULL` o vacío para reproducir el defecto corregido;
- la fila mostrada conserva tipología, formato, icono y acciones;
- no ocurre navegación, postback, `DataBind`, recarga parcial ni recarga completa;
- una consulta `SELECT` posterior confirma exactamente una persistencia documental nueva con el radicado autoritativo;
- tarea, registro de estado, plantilla y gabinete permanecen correlacionados.

La E2E negativa reutiliza una sesión real y altera solo `radicado_radicacion` en el multipart para enviarlo diferente del valor persistido. Debe recibir rechazo funcional, no insertar una fila y conservar el conteo documental antes/después. No se agrega un endpoint, modo de prueba ni bypass al producto.

Todas las consultas de evidencia son `SELECT` parametrizados. La cuenta de control no puede ejecutar mutaciones, DDL ni procedimientos de limpieza. La evidencia guarda únicamente códigos, conteos, identificadores saneados, latencias y huellas; nunca contenido documental, credenciales, cookies, tokens, cadenas de conexión ni cuerpos completos.

Antes de ejecutar se leen `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`. La corrida no activa ni modifica `WorkflowCentroTrabajoModernActive`; al inicio y en `finally` confirma `false` y listas de usuarios/grupos vacías. Si falta autorización o recurso descartable, la E2E queda bloqueada operacionalmente: no se sustituye por mocks y el ticket no puede declararse cerrado.

## Criterios de aceptación

- `FileUploadHandler_.ashx.vb` permanece sin cambios.
- Las llamadas legacy resuelven la sobrecarga de diez argumentos.
- `ADJUNTARADICACION` resuelve la sobrecarga de doce argumentos.
- El repository obtiene una sola vez el contexto desde el registro de estado.
- El radicado del navegador nunca sustituye un `consecutivo_radicado` ausente.
- La nueva ruta no consulta `DAT_ADIC_TAR` para resolver el radicado.
- Plantilla, índices, almacenamiento y respuesta utilizan el mismo valor autoritativo.
- `AlmacenaDocumentosRadicacion(...)` y `Almacenamiento(...)` se reutilizan sin crear un motor paralelo.
- Los demás eventos y módulos conservan sus contratos.
- La interfaz agrega el documento mediante JavaScript sin recargar GridView ni ejecutar postback.
- Existe una E2E real integrada a `tools/e2e`, no un harness paralelo.
- La E2E positiva confirma almacenamiento con `DAT_ADIC_TAR` vacío y persistencia única bajo el radicado autoritativo.
- La E2E negativa confirma rechazo ante discrepancia del radicado informativo sin persistencia adicional.
- La E2E confirma la inserción JavaScript sin recarga y conserva evidencia saneada mediante consultas `SELECT`.

## Criterio de cierre

La corrección no se considera terminada solo porque desaparezca el mensaje visual. Debe existir evidencia automatizada de que:

```text
registro de estado
  -> radicado autoritativo
  -> plantilla
  -> índices
  -> almacenamiento
  -> respuesta
```

usa una sola identidad, y de que el Controller compartido y todos los recorridos distintos de `ADJUNTARADICACION` permanecen intactos.

El cierre exige además una corrida E2E real aprobada con ambiente, cuenta y recurso descartable expresamente autorizados. Si esas condiciones no están disponibles, se documenta el bloqueo y el cambio permanece sin cierre, aunque todas las pruebas locales hayan aprobado.
