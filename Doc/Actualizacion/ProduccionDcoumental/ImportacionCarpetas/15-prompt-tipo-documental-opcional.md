# Prompt 15 — Tipo documental opcional por perfil de importación

## ROL ESPERADO

Actúa como arquitecto y desarrollador senior de ASP.NET WebForms, VB.NET, JavaScript y MySQL 5.1. Implementa el tipo documental opcional sobre la importación por carpetas existente, manteniendo sus controles de autorización, integridad, idempotencia y recuperación.

## OBJETIVO Y ENCARGO

Permitir que un perfil de importación autorice documentos sin tipología documental. Los perfiles anteriores mantienen la obligatoriedad actual; el nuevo comportamiento requiere una configuración explícita en una versión del perfil y queda fijado en el snapshot del plan y del lote.

La ausencia permitida de tipo documental es distinta de un código o identificador informado que resulte inválido, no autorizado o ambiguo. Estos últimos continúan produciendo errores. No asignar una tipología ficticia, seleccionar la primera disponible o convertir «sin coincidencia» en ausencia aceptada.

Este prompt describe implementación futura. Su creación no acredita funcionalidad implementada ni autoriza despliegues, reemplazo de DLL del IIS activo, migraciones o almacenamiento real de documentos.

## CONTEXTO Y DEPENDENCIAS

Leer `AGENTS.md`, [guía 00](00-guia-ejecucion-prompts.md), [exploración de este requisito](Exploracion/15-tipo-documental-opcional.md) y el diseño vigente en `Diseno/`. Verificar los componentes existentes de perfil, preparación, materialización, almacenamiento y recuperación antes de editarlos; no construir recorridos paralelos.

Esta mejora es independiente del [prompt 14 de fecha inicial](14-prompt-fecha-inicial-por-importacion.md) y del ejecutor opcional 13. Si la fecha inicial ya fue implementada, conservar su contrato y agregar regresión de coexistencia. No implementar esas capacidades por ejecutar este prompt.

Las decisiones explícitas de este prompt sobre tipología opcional actualizan exclusivamente ese punto de las exigencias históricas de los prompts 02/06/09/12 para perfiles que opten por la nueva política. Los perfiles anteriores conservan el comportamiento requerido. Refinar detalles internos con evidencia del repositorio y documentarlos; consultar al usuario solo ante una incompatibilidad que obligue a cambiar el alcance o una restricción vinculante.

## RESTRICCIONES CRÍTICAS

- Mantener WebForms, .NET Framework 4.6.1, VB.NET, ASMX, JavaScript y MySQL 5.1 existentes. Sin nuevas plataformas, sistemas de permisos, importadores o escritores documentales.
- **Almacenamiento intocable:** no modificar `workflow/ClassAlmacenamiento.vb` ni las funciones congeladas por D-RI-07. Leer el [diagnóstico de almacenamiento](../../../Diagnostico/radicacion-interna/diagnostico-almacenamiento-metadatos.md) y su baseline; ejecutar `node tools/diagnostico/verify-radicacion-storage-freeze.cjs` antes y después. No cambiar el verificador o baseline para autorizar diferencias.
- Se permite adaptar el comando y los puntos de llamada propios de `Infrastructure/ProduccionDocumental/ImportacionCarpetas/` que no estén congelados. Esto no autoriza editar el motor legacy ni funciones compartidas protegidas. Conservar firma y semántica del escritor.
- No cambiar SII, carga individual, radicación, creación de expedientes, retención, clasificación de serie/subserie ni documentos ya almacenados. No eliminar tipos documentales o modificar catálogos reales para probar.
- No confundir la tipología TRD (`TIPOLOGIA:TIPO_DOCUMENTAL`) con la clase documental `DOCUMENTO ELECTRONICO`, el formato PDF/DBT o el tipo de expediente. Estos datos y sus validaciones siguen siendo necesarios.
- Mantener las reglas de código, nombre y fecha inicial del expediente, índices requeridos del gabinete y barreras de integridad. No generalizar la aceptación de vacíos a otros destinos o fuentes.
- Mantener lectura y huellas de perfiles, planes, partes y lotes anteriores. No cambiar globalmente la serialización ni relajar el JSON cerrado, la comparación de publicación, revisiones, permisos o controles de identidad.
- No modificar esquemas ni configuraciones de gabinetes para volverlos compatibles de forma silenciosa. La compatibilidad debe comprobarse sin efectos documentales, antes de confirmar y crear expedientes.
- El diseño utiliza la configuración JSON existente; no presupone DDL. Si aparece una limitación que exige esquema nuevo, documentarla y refinar antes de introducir SQL incremental MySQL 5.1 para aplicación manual. No ejecutar SQL automáticamente.

## COMPORTAMIENTO FUNCIONAL Y SEMÁNTICA

Agregar a la configuración del perfil **«Tipo documental: requerido / opcional»**. Mantener el selector en la administración del perfil, con versiones publicadas de solo lectura; no permitir que la solicitud de ejecución sobrescriba la política del snapshot.

| Política efectiva | Regla de tipología | Comportamiento |
|---|---|---|
| Requerido, incluido todo perfil anterior | No existe | No publicar ni preparar como perfil operativo. |
| Requerido | Existe, incluso con `Obligatoria=false` | Exigir un valor y una coincidencia válida por archivo, como actualmente. |
| Opcional | No existe | Admitir documentos sin tipo; no inventar regla o valor. |
| Opcional | Existe y `Obligatoria=false` | Admitir ausencia genuina de valor; validar normalmente cualquier valor informado. |
| Opcional | Existe y `Obligatoria=true` | La regla exige valor. La UI debe explicar esta exigencia y no anunciar ausencia permitida para esa regla. |

La opción «opcional» permite ausencia por archivo; un mismo lote puede contener documentos con y sin tipología. En revisión y resultados mostrar **«Sin tipo documental»**, distinguiéndolo de «pendiente de resolver» y de un error. Mantener avisos, detalle por campo, reglas colapsables, foco, acción visible de agregar regla y funcionamiento móvil.

El vacío aceptable es un valor ausente/vacío de una fuente evaluada correctamente. Segmento inexistente, extracción inválida, dependencia todavía pendiente, longitud excedida, código no encontrado, varias coincidencias, catálogo inaccesible o falta de permiso conservan sus errores. No agregar nuevos tipos de fuente ni consultas de proveedor para obtener tipologías.

## CONTRATO Y DISEÑO DE IMPLEMENTACIÓN

### Política y compatibilidad de documentos anteriores

Proponer `PoliticaTipoDocumental As String` opcional en `ConfiguracionPerfilImportacion`, con valores explícitos `REQUERIDO` y `OPCIONAL`. La ausencia de la propiedad significa `REQUERIDO`. Rechazar cadenas vacías y valores desconocidos.

Omitir la propiedad del JSON cuando sea `Nothing`, mediante configuración localizada en esa propiedad; no inicializar automáticamente todos los objetos históricos con una cadena nueva ni modificar globalmente el serializador. Conservar la representación y huella anteriores después de leer y volver a serializar. La UI no debe introducir una propiedad al abrir un perfil histórico sin cambios.

La política pertenece a la versión del perfil, por lo que participa en su huella cuando se configura y viaja en el snapshot existente. Un cambio de política requiere una versión conforme al ciclo vigente, sin reescribir planes o lotes anteriores. Resolver su valor efectivo de forma uniforme en los consumidores; no duplicar interpretaciones.

### Resolución y representación de ausencia

Propagar la política autorizada a los validadores y resolvedores internos que hoy reciben solamente reglas. Conservar un comportamiento requerido por defecto para consumidores antiguos; no usar una variable global o un parámetro HTTP como autoridad. Revisar guardado, publicación, ensayo, selección operativa, preflight clásico/segmentado, valores diferidos y correspondencia entre reglas y valores.

Fijar una única representación interna de ausencia antes de implementar y documentarla en el diseño. Recomendación: sin regla de tipología, no agregar un valor de tipología; con regla opcional presente, conservar su entrada con `Valor=Nothing`, `Diferido=false` y tipo canónico `REFERENCIA` para mantener correspondencia entre regla y valor. Preservar fuente y procedencia aplicables. Si se elige otra representación por compatibilidad comprobada, actualizar todos sus consumidores y pruebas antes de ejecutar efectos.

No aceptar un valor nulo por el solo hecho de tener `Obligatoria=false`: debe permitirlo también la política efectiva del perfil. Conservar el rechazo de tipologías duplicadas. Un valor presente sigue resolviéndose por ID cuando es `REFERENCIA` y por `codigo_documento` cuando es `TEXTO`; nunca interpretar un nombre como código o un texto numérico como ID implícito.

El catálogo evita buscar una tipología solamente ante una ausencia ya autorizada y validada. Una ausencia no ejecuta una búsqueda sin filtro. Una dependencia diferida continúa pendiente hasta resolverse y validarse; no convertirla anticipadamente en «sin tipo». Los errores auténticos no se suprimen con el modo parcial.

### Comando, adaptador y almacenamiento existente

Adaptar `ComandoAlmacenamientoCarpetas.TipologiaId` y `Componer` para admitir cero o una tipología según la política y la regla, preservando todas las demás barreras. Representar internamente la ausencia con `Nothing`, sin publicarla como ID `0` ni relajar `ReferenciaValida` o `EnteroLegacy` para todos los usos.

En el límite del adaptador propio, traducir únicamente la ausencia autorizada a los argumentos que el escritor ya contempla: `IdTipoLogia=0` y `DescripcionTipo=""`. Cuando existe tipo, mantener la conversión y validación positiva actuales. Esta traducción no añade una tipología ficticia al plan ni permite que el cliente envíe un ID cero como referencia válida.

Adaptar `PrepararDatosAlmacenamiento` y `ValidarDatos`: sin tipo no exigir consulta ni nombre de catálogo; con tipo mantener comprobación de coincidencia única y autorización. Mantener validaciones de gabinete, índices, expediente, clase y formato. Si existe un índice `TIPODOCUMENTO` requerido, no eludir su obligatoriedad con vacío.

### Compatibilidad física y comprobación posterior

El motor legacy contiene ramas que escriben `NULL` para el ID ausente y descripción vacía; el inventario puede recibir descripción `NULL` y el índice utiliza «NA». Esto es evidencia del código, no una garantía del esquema desplegado.

Para la nueva política opcional, antes de confirmar, comprobar mediante los puertos actuales o una extensión mínima de lectura las columnas efectivamente usadas por el escritor en el gabinete, `registro_producion_documental` y `ra_cert_indice_expediente`: tipos, nulabilidad, restricciones, campos visibles y obligatoriedad configurada de `TIPODOCUMENTO`. Los perfiles requeridos conservan sus comprobaciones actuales y no adquieren esta nueva exigencia de admitir ausencia. Consultas de metadatos parametrizadas y solo `SELECT`, sin escrituras de ensayo. No asumir que todos los gabinetes comparten esquema. Reutilizar el mecanismo de lecturas/sellos de preparación para detectar cambios relevantes entre revisión y confirmación.

Ante incompatibilidad, producir un error de configuración claro antes de crear expedientes o almacenar documentos. No modificar el gabinete para superar la comprobación. Un fallo al consultar metadatos no equivale a compatibilidad.

Adaptar `LeerAlmacenamiento`, comprobación final y recuperación V2 para verificar explícitamente el estado esperado sin tipología: ID `NULL` donde corresponde y las representaciones vacía/`NULL`/«NA» que realmente produce cada campo del escritor. Documentar la matriz por tabla y campo; no aceptar todas esas variantes indistintamente ni omitir la comprobación de tipo. Con tipología presente, continuar exigiendo el ID y los valores esperados; un ID nulo, distinto o de otro ámbito no es éxito.

Mantener atribución por identidades conocidas, huellas, vínculos de documento/inventario/expediente, fase, reclamación y diario existentes. La ausencia de tipo nunca constituye prueba de ausencia del documento. No repetir almacenamiento por una verificación incierta; conservar reconciliación e idempotencia.

## PRECONDICIONES DE RUTAS

Inventariar los archivos exactos antes de editar en `Diseno/design.md`, con responsabilidad, consumidores y condición existente/nuevo. Registrar explícitamente cualquier archivo VB nuevo en `GestionDocumental-Docuarchi.net.vbproj`. Las rutas siguientes son existentes y deben revisarse; no todas necesitan cambios.

| Área | Rutas y puntos principales |
|---|---|
| Modelos y contratos | `Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionModels.vb`, interfaces de perfil y operación del mismo directorio; `DTOs/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionDtos.vb` y contratos de consulta aplicables. |
| Reglas y ensayo | `Services/ProduccionDocumental/ImportacionCarpetas/ValidadorPerfilImportacion.vb`, `ResolvedorReglasPerfilImportacion.vb`, `CoordinadorProveedorIndices.vb`, `ServicioPerfilImportacion.vb`, `PerfilImportacionJson.vb`. |
| Preparación y materialización | `ServicioPreparacionImportacion.vb`, `ServicioPreparacionSegmentadaImportacion.vb`, `ServicioRecepcionImportacion.vb` y `CoordinadorImportacionCarpetas.Valores.vb` en el directorio de servicios anterior. Revisar también consolidación y reconciliación. |
| Catálogos y esquema | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb`, `.Preparacion.vb`, `.Documento.vb` y `.RecuperacionV2.vb`. Conservar permisos y cachés/sellos de lectura. |
| Comando y llamada al escritor | `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ComandoAlmacenamientoCarpetas.vb`, `AdaptadorDocumentoImportacion.vb`, `AdaptadorDocumentoImportacion.Legacy.vb`. Solo contrato y adaptación propios; motor congelado sin cambios. |
| Persistencia y recuperación | `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/` para snapshots y diario documental existentes; `PersistenciaDocumentoV2Importacion.vb` en adaptadores y `CoordinadorImportacionCarpetas.RecuperacionV2.vb` en servicios. No duplicar repositorios. |
| Interfaz | `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js`, `ImportacionCarpetas.js`, clientes y estilos existentes; `Gestion/WebFormProducionDocumental.aspx` para versiones de recursos si corresponde. |
| Entradas y proyecto | `webservice/WebServiceImportacionCarpetas.asmx`, `webservice/WebServiceImportacionCarpetas.asmx.vb`, sus parciales y `GestionDocumental-Docuarchi.net.vbproj`. Conservar endpoints y autorización. |

## SECUENCIA DE TRABAJO

1. Caracterizar y fijar expectativas anteriores de serialización, huellas, regla obligatoria y lectura de planes/lotes, incluidos esquemas 1 y 2 y recuperación V2. Revisar soporte físico mediante metadatos sin efectos.
2. Implementar política, representación de ausencia, validación y ensayo; probar requerido/opcional y valores inválidos antes de adaptar ejecución.
3. Integrar preparación clásica/segmentada, materialización y revisión; bloquear incompatibilidades antes de confirmar y crear expedientes.
4. Adaptar comando y llamada al escritor, comprobación posterior y recuperación como una misma entrega coherente. No habilitar la nueva política mientras alguna de esas etapas conserve la exigencia anterior.
5. Completar UI, compilación, regresión y evidencia. Preparar publicación y retorno compatibles con lotes pendientes, sin desplegar por ejecutar este prompt.

## CRITERIOS DE ACEPTACIÓN Y PRUEBAS OBLIGATORIAS

| ID | Escenario | Resultado exigido |
|---|---|---|
| TD-01 | Perfil anterior sin propiedad nueva; planes, partes y lotes históricos. | Política efectiva requerida; mismas representaciones, huellas, exigencias y recuperación. No regenerar expectativas históricas para acomodar regresiones. |
| TD-02 | Crear borrador opcional sin regla de tipo, guardar, publicar y seleccionar; versión publicada de solo lectura. | Ciclo autorizado completo sin fecha o tipo ficticios; snapshot conserva política y no altera otra ejecución. |
| TD-03 | Matriz de política requerida/opcional, regla ausente/presente y `Obligatoria` verdadera/falsa. | Cumplir cada fila de la tabla funcional; vacío solo permitido bajo las condiciones explícitas. |
| TD-04 | Regla opcional con vacío o `Nothing`; lote mixto con tipos válidos y ausentes. | Ausencia canónica estable, revisión legible y correspondencia entre reglas y valores, sin consultar catálogo sin filtro. |
| TD-05 | Valor cero, ID ajeno, código desconocido, nombre usado como código o coincidencia múltiple. | Mantener errores; no transformar en ausencia aceptada ni elegir la primera tipología. |
| TD-06 | Extracción inválida, catálogo inaccesible, tipo duplicado o dependencia diferida. | Conservar error o estado pendiente según corresponda; no habilitar almacenamiento prematuramente. |
| TD-07 | Preparación clásica/segmentada, fragmentación, reconstrucción y guardado de valores efectivos. | Mismo significado y representación de ausencia; políticas completa/parcial y barreras actuales conservadas. |
| TD-08 | Gabinete compatible, columnas no compatibles, índice `TIPODOCUMENTO` requerido o cambio de configuración entre revisión y confirmación. | Solo permitir ausencia con compatibilidad demostrada; bloquear incompatibilidad antes de efectos; no cambiar esquema ni metadatos operativos. |
| TD-09 | Llamada al escritor mediante dobles, con y sin tipo. | Para ausencia autorizada, argumentos `0` y `""` solo en el adaptador; para presencia, ID positivo y nombre autorizado. Motor y baseline idénticos. |
| TD-10 | Verificación de documento/inventario/índice, con ausencia y presencia; resultados con ID incorrecto o vínculo ajeno. | Comprobar matriz de campos explícita; no tratar discrepancias como éxito ni relajar comprobaciones de identidad. |
| TD-11 | Pausa, reanudación, recarga, respuesta perdida, verificación final y recuperación V2. | Política y valores del snapshot conservados; no repetir efectos inciertos ni convertir ausencia de tipología en ausencia de documento. |
| TD-12 | Nueva versión cambia requerido/opcional mientras existe lote confirmado. | El lote anterior conserva su política; nuevas selecciones usan su versión; no sobrescribir snapshots, valores o documentos históricos. |
| TD-13 | Acceso sin permisos, solicitud de otro propietario/ámbito o JSON manipulado para forzar opcional. | Rechazos actuales conservados; ninguna política HTTP sustituye la del perfil autorizado. |
| TD-14 | UI con teclado, móvil, reglas largas y perfil publicado. | Selector y ayudas claros; «Sin tipo documental», error y pendiente distinguibles; colapsado, foco, agregar regla y solo lectura conservados. |
| TD-15 | Regresión de egresos, tipo documental requerido, carga individual y fecha inicial del prompt 14 si existe. | Comportamientos anteriores conservados; no se implementan otras capacidades para completar este caso. |
| TD-16 | Integración autorizada de almacenamiento y recuperación en gabinete compatible. | Persistencia real y comprobación concordantes con y sin tipo; sin duplicación y con almacenamiento congelado. Mantener pendiente si faltan ambiente, cuentas o autorización. |

Reutilizar arneses y pruebas existentes: `tests/doc110-perfil-importacion-behavior.test.cjs`, `tests/doc110-perfil-importacion-editor.test.cjs`, `tests/doc110-perfil-importacion-client.test.cjs`, `tests/doc117-almacenamiento.test.cjs`, `tests/ImportacionCarpetasRecuperacionV2BehaviorTests.cs`, `tests/doc120-preparacion-segmentada.test.cjs` y suites de navegador pertinentes. Ampliar pruebas de comportamiento; una búsqueda de cadenas no demuestra persistencia ni recuperación.

Para JavaScript ejecutar `node --check` sobre archivos cambiados. Para VB compilar con MSBuild y la configuración real de `GestionDocumental-Docuarchi.net.vbproj`; un arnés aislado no sustituye la compilación de la aplicación. No sustituir DLL del IIS activo para ejecutar comprobaciones locales.

La E2E es parte integral del mismo cambio: código, pruebas, validación autorizada y evidencia saneada forman la unidad de entrega. Reutilizar exclusivamente la infraestructura existente de `tools/e2e`: autenticación, configuración, validadores, reporteros y utilidades. No crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Cubrir autorización, lectura sin mutación, escrituras autorizadas, concurrencia y regresión según TD-01–TD-16.

Antes de E2E autenticado, carga o escritura real, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md` y obtener autorización explícita para ambiente, cuentas y datos descartables. Este prompt no la concede. Usar secretos efímeros; no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión. Consultas de control solo `SELECT`; evidencia saneada y datos de ejecución en ubicaciones excluidas.

Respetar los gates de seguridad existentes sin habilitarlos arbitrariamente para probar. No introducir flags o listas de usuarios/grupos para activar la experiencia moderna. No cerrar sin la validación requerida; registrar bloqueo explícito cuando falte ambiente o autorización. Mocks y simulaciones son evidencia local complementaria y no sustituyen la integración real pendiente.

## PUBLICACIÓN Y RETORNO

Publicar backend y recursos cliente compatibles antes de permitir perfiles opcionales. Versionar recursos según el mecanismo vigente. Un backend antiguo puede rechazar la propiedad nueva o un lote sin tipología: documentar cómo preservar lectura y recuperación de esos lotes antes de un retorno. No quitar soporte mientras existan preparaciones/lotes afectados pendientes ni convertir documentos sin tipo en tipos ficticios para permitir una reversión.

## DOCUMENTACIÓN TÉCNICA

Actualizar la exploración, inventario y diseño vigente con contrato final, matriz de política/regla, representación de ausencia y matriz de persistencia por tabla/campo. Agregar tabla de funciones creadas/modificadas: función, ruta, ubicación, parámetros y responsabilidad/retorno. Documentar gabinetes compatibles, errores de configuración, recuperación, publicación y retorno en las guías existentes; evidencia saneada en `Validacion/`, datos de ejecución en ubicaciones excluidas. No duplicar documentación del módulo.

## ENTREGABLE FINAL

Entregar archivos cambiados, decisiones refinadas, contrato final y evidencia por TD-01–TD-16 con comandos, resultados y limitaciones. Separar `EJECUTADO_OK`, `FALLIDO` y `NO_EJECUTADO` con motivo. Informar explícitamente si solo se verificó el código o también un esquema desplegado y persistencia real.

Registrar cambios propios en la carpeta principal según `AGENTS.md`, comunicar sincronización con el remoto y conservar avances ajenos. No declarar la capacidad operativa de extremo a extremo mientras falten compilación, demostración de compatibilidad física o integración requerida. El escritor congelado debe permanecer idéntico.
