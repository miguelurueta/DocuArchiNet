# Contexto técnico: selección y carga de carpetas

Fecha: 2026-10-19. Revisión estática para redactar el prompt; no se implementó ni se ejecutó una carga.

## Diagnóstico

La selección e inventario son principalmente JavaScript. La importación completa requiere además coordinación persistida en servidor: autorización, resolución del perfil, creación de expedientes, almacenamiento, incorporación y recuperación. No es suficiente agregar un atributo de selección de directorios al control actual.

## Evidencia del repositorio

| Ruta y símbolo | Hallazgo | Decisión |
|---|---|---|
| `GestionDocumental-Docuarchi.net.vbproj` | Framework objetivo 4.6.1. | Respetar WebForms/VB.NET y las dependencias existentes. |
| `Gestion/WebFormProducionDocumental.aspx`, `js/gestion/WebFormProducionDocumental.js`, `ActivaCargaArchivos` | Activa `IniLoadPerson` con un `IdRespuestaIdExpediente` único. | Agregar una entrada propia de importación; mantener la carga individual. |
| `webservice/WebServiceProducion.asmx.vb`, `ServiceSolicitaCargarDocumentoExpediente` | Consulta selección y permiso actuales mediante `ClassGaProducionDocumental`. | Referencia de contrato y autorización; no es un preflight de varias carpetas. |
| `generic_control/FileUploadHandler.js`, `LoadFilePERSON`, `_SolicitaItemTabla` | Obtiene `File` mediante `InputFile.files` y `find(file => file.name === NombreBuscado)`; construye FormData por archivo. | No usar ese mecanismo como identidad de archivos homónimos. Nueva cola con raíces e IDs estables. |
| Mismo archivo, `_PreEnvioArchivosServidor`, `_EnviaArchivoServidor` | Usa `JSProgresBarBoot`, envía al ASHX y diferencia confirmación incierta en Producción. | Reutilizar presentación/progreso solo si no impone estado global ni semántica incompatible. |
| `generic_control/FileUploadHandler_.ashx.vb`, `ProcessRequest` | `IRequiresSessionState`, `SaveAs(path & file.FileName)`, asignación de variables de sesión, rama PRODUCCION con `UploadSaveFile`. | No enviar rutas relativas como nombres físicos ni convertir el handler compartido en coordinador de lotes. |
| `Gestion/ClassGaExpediente.vb`, `Registrar_Expediente_Conservacion` | Operación existente con validaciones, salida de identificadores y efectos documentales. | Candidata a invocación mediante adaptador nuevo; comprobar parámetros, autoridad y recuperación tras fallo parcial. |
| `Gestion/ClassGaProducionDocumental.vb`, `Agregar_expediente_produccion_carpeta_a_la_estructura` | Consume la operación anterior y controles de interfaz; usa valores predeterminados del flujo interactivo. | Referencia de composición, no servicio listo para invocar desde una importación. |
| `workflow/ClassAlmacenamiento.vb`, `PreAlmacenaDocumentoProduccion` | Resuelve gabinete del expediente y campos heredados de relaciones compartidas. | No modificar relaciones globales para aplicar un perfil. |
| Mismo archivo, `AlmacenamientoDocumentoProduccionDocumental` | Recibe `List(Of CDcamposAsignaAlmacenamiento)`, gabinete y expediente explícitos. Usa sesión para usuario/empresa, temporales y limpieza; contempla Multi-TIFF. | Punto candidato para valores por archivo, condicionado a verificar dependencias y efectos. No constituye un servicio autónomo confirmado. |
| `Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb` | Rechaza `IdTareaWorkflow <= 0` e invoca almacenamiento Workflow. | Referencia únicamente. No utilizar tareas ficticias ni modificar SII. |

Las rutas de la tabla son relativas a la raíz del repositorio. La revisión no certifica comportamiento desplegado ni atomicidad del almacenamiento. Las pruebas DOC-88 citadas en exploraciones anteriores son verificaciones estáticas; en esta revisión no se volvieron a ejecutar.

## Selección desde navegador

La API `webkitdirectory` devuelve archivos con `webkitRelativePath`; permite construir el inventario conservando su ubicación relativa. Ver documentación primaria de [HTMLInputElement.webkitdirectory](https://developer.mozilla.org/en-US/docs/Web/API/HTMLInputElement/webkitdirectory) y [File.webkitRelativePath](https://developer.mozilla.org/en-US/docs/Web/API/File/webkitRelativePath).

Propuesta inicial: botón «Agregar carpeta» repetible, una raíz seleccionada representa un expediente, con tratamiento explícito de subcarpetas según perfil. No depender de que el selector nativo admita varias raíces en una sola apertura. Detectar capacidad y validar en los navegadores objetivo.

Consecuencias para el diseño:

- La lista de archivos no permite inferir carpetas vacías. No prometer su importación con ese mecanismo; informar la limitación y no fabricar expedientes vacíos.
- Dos selecciones pueden tener el mismo nombre de raíz. Asignar IDs de selección distintos y reportar coincidencias documentales; no fusionarlas por nombre.
- Conservar referencias `File` en memoria. No leer todo como base64 ni crear todas las vistas previas a la vez.
- Al recargar, el estado del servidor puede recuperarse, pero los archivos locales pendientes requieren nueva selección y comprobación de correspondencia. No prometer acceso persistente al disco.
- La ruta relativa es metadato no confiable, no una ruta física del servidor. Rechazar recorridos fuera de raíz y usar nombres temporales generados.

## Decisiones para el prompt

Hallazgo adicional de integración: `AlmacenamientoDocumentoProduccionDocumental` obtiene nombre de `FileInfo(RutaArchivoAlmacenar).Name` y elimina archivos de `MatrizDocumentosFinal` tras persistir. La referencia de staging recuperable no puede entregarse como único original consumible ni usar un nombre opaco suponiendo conservación automática del nombre visible. El prompt 09 exige caracterizar copia de trabajo, nombre/extensión y efectos de limpieza. El diagnóstico 06 detalla estas incompatibilidades y las condiciones de reutilización.

### Ubicación obligatoria en la estructura de Producción

El usuario confirmó que las carpetas deben generar expedientes en una estructura específica de Producción Documental. Debe resolverse un nivel padre existente y autorizado antes de confirmar, independiente del gabinete documental y de la clasificación. `Agregar_expediente_produccion_carpeta_a_la_estructura` recibe `id_nivel_padre`, consulta permisos de nivel y lo transmite a `Registrar_Expediente_Conservacion`; la creación devuelve un identificador de relación. Es evidencia de que crear el expediente y vincularlo a la estructura son requisitos del recorrido.

Precisión posterior del usuario: la exploración y creación de estructuras ya existen. El usuario selecciona el nivel en el árbol de Producción y desde allí abre la ventana de importación. Esta decisión sustituye la propuesta de volver a seleccionar el destino dentro del asistente. La ventana recibe ID/ámbito del nivel y muestra su ruta como solo lectura; el perfil aporta datos compatibles sin cambiar el destino. Todos los expedientes del lote usan ese contexto; no se crean niveles a partir de las subcarpetas. El servidor debe persistir ID/ámbito, comprobar estado y permisos y verificar la relación resultante. Para otro destino se cierra la ventana y se abre una nueva preparación desde otro nivel; los lotes confirmados no cambian con la navegación ni con una selección mutable de sesión.

El [prompt de carga por carpetas](../03-prompt-implementacion-carga-carpetas.md) cubrirá la operación completa por fases comprobables. Depende de la implementación real del perfil; no debe sustituirla con constantes. La modalidad inicial propuesta debe seleccionarse explícitamente en el perfil; otras estrategias no implementadas se rechazan con un mensaje claro.

La ejecución será persistida y acotada por operación. No se propone un bucle largo dentro de una sola petición ni una tarea ASP.NET en memoria que prometa sobrevivir al reciclado. La primera versión puede avanzar mediante peticiones del cliente y reanudarse; cerrar el navegador detiene el avance y exige consultar/reconciliar operaciones en vuelo. Una ejecución desatendida requiere un ejecutor durable, fuera del alcance inicial.

Las tablas operativas serán propias, adicionales a perfil/versión/reglas. El prompt exigirá un SQL concreto para MySQL 5.1, aplicado manualmente por el equipo del usuario. SII permanece intacto.
