# Exploración: carga documental e importación masiva por carpetas

Fecha: 2026-10-19.

Estado: exploración arquitectónica; no constituye una especificación aprobada ni una implementación.

Consolidación de la revisión arquitectónica: las decisiones vigentes se coordinan en [la guía de prompts](../00-guia-ejecucion-prompts.md) y el [diagnóstico de integración](06-diagnostico-coherencia-prompts.md). Primera modalidad: dispositivo del usuario, una raíz seleccionada por expediente, ventana abierta desde un nivel existente de Producción, perfil independiente de autorregistro, SII intacto y ejecución acotada con estado persistido. El ejecutor desatendido es una evolución opcional. Integridad completa por defecto y parcial configurable; ambos ámbitos, expediente y lote, están aprobados como configuración del perfil.

Restricción obligatoria del usuario: la implementación SII existente no puede modificarse. Es un modelo de referencia. Solo se permite reutilizar componentes que puedan consumirse tal como están, sin alterar su comportamiento ni el de SII. Esta restricción prevalece sobre cualquier propuesta previa de extracción o generalización compartida.

Diagnóstico posterior de tablas: [01-diagnostico-reutilizacion-tablas.md](01-diagnostico-reutilizacion-tablas.md). Precisa la propuesta inicial: se recomienda configuración nueva de perfil/versión/reglas que referencie catálogos existentes. El autorregistro puede servir como plantilla de lectura, pero no debe adoptarse automáticamente como identidad del perfil o del expediente importado, porque tiene efectos funcionales propios.

Validación posterior en la base configurada: [03-validacion-bd-y-definicion-perfil.md](03-validacion-bd-y-definicion-perfil.md), con evidencia de metadatos en [02-evidencia-esquema-perfil.json](02-evidencia-esquema-perfil.json). Se utilizaron exclusivamente SELECT de esquema, sin leer registros documentales ni modificar SII. Las limitaciones de acceso a esquema expresadas en las etapas iniciales quedan precisadas por esta evidencia.

## 1. Objetivo y alcance

Referencia de funcionamiento del perfil: [04-diagrama-flujo-tecnico-perfil.md](04-diagrama-flujo-tecnico-perfil.md). Conserva en texto los recorridos de creación/publicación y selección/aplicación, con cajas, flechas y decisiones entre Cliente, Controller, Service, Repository y Respuesta.

Extender Producción Documental para importar una o varias carpetas, crear expedientes con el nombre de cada carpeta y almacenar sus documentos en un gabinete, incorporándolos al expediente correspondiente.

Los datos de clasificación y los demás valores predeterminados del expediente procederán de un perfil almacenado en tablas internas. El nombre de la carpeta aportará el nombre visible del expediente. A diferencia de SII, este origen no necesita consultar un servicio web externo para completar los datos del expediente.

La exploración incluye el flujo actual de carga desde dispositivos, los componentes reutilizables de Producción y de la integración SII, una propuesta de extensión y las condiciones para controlar la deuda técnica.

No se modificó código de aplicación, no se crearon tablas y no se ejecutaron cargas reales durante la exploración. Las conclusiones describen el código del repositorio, no una certificación del ambiente desplegado.

## 2. Conclusión de viabilidad

La extensión es viable. El origen interno de los datos no introduce deuda técnica por sí mismo: ya existe configuración de autorregistro que guarda fondo, instrumento, área, serie y subserie.

La separación recomendada es:

```text
Perfil interno + datos de carpeta
              ↓
Datos completos y validados del expediente
              ↓
Creación del expediente mediante operación existente
              ↓
Almacenamiento e incorporación de documentos
              ↓
Verificación y resultado persistido
```

La nueva operación de creación debe recibir datos completos y autoridad explícita, sin depender del origen de esos datos. La nueva integración tendrá adaptadores propios y podrá consumir capacidades existentes sin cambios cuando sus contratos y efectos sean compatibles. SII conserva su implementación, composición y recorrido actuales.

No se recomienda copiar la integración SII completa ni simular tareas Workflow, trámites o inscripciones para satisfacer sus parámetros. Tampoco ampliar el flujo mediante cambios sucesivos de variables de sesión por carpeta.

## 3. Contexto del módulo existente

El módulo es `Gestion/WebFormProducionDocumental.aspx`. La aplicación utiliza ASP.NET Web Forms, VB.NET y .NET Framework 4.6.1, con JavaScript, jQuery, servicios ASMX y un handler ASHX compartido.

La carga desde dispositivos utiliza la clase documental `DOCUMENTO ELECTRONICO`.

### Inventario funcional del módulo

| Área | Capacidades identificadas |
|---|---|
| Organización | Crear, editar y eliminar expedientes y niveles; cortar, pegar y limpiar selección. |
| Clasificación | Instrumento archivístico, series, subseries y tipologías. |
| Acceso | Compartir niveles, consultar y retirar permisos; permiso específico de carga. |
| Navegación | Árbol, búsquedas, restauración de consultas, paginación y ordenamiento. |
| Visibilidad | Ocultar niveles, listar ocultos y volver a mostrarlos. |
| Incorporación | Archivos locales y digitalización. |
| Documentos | Visualizar, descargar, renombrar, eliminar, copiar, pegar y gestionar metadatos. |
| Procesamiento | Radicación y actualización de índices por lote. |
| Colaboración | Compartir con usuarios internos, correo y solicitudes de firma electrónica. |
| Plantillas | Descargar plantillas para elaborar oficios. |
| Archivo | Archivar, desarchivar y ubicar expedientes; unidades contenedoras y de conservación. |
| Control | Estado del expediente, creación y consulta de índice. |
| Rótulos | Configuración, impresión y descarga. |

Este inventario procede de menús, eventos y servicios. La revisión profunda se concentró en la carga desde dispositivos y en los puntos de extensión para importación.

## 4. Recorrido actual de carga

```text
Cargar → Archivo
  → ActivaCargaArchivos
  → ServiceSolicitaCargarDocumentoExpediente
  → validación inicial de selección, estado y permiso
  → LoadFilePERSON / IniLoadPerson
  → consulta de configuración y tipologías
  → selección o arrastre de archivos
  → FormData por archivo
  → FileUploadHandler_.ashx
  → archivo temporal por usuario
  → UploadSaveFile, rama PRODUCCION
  → PreAlmacenaDocumentoProduccion
  → AlmacenamientoDocumentoProduccionDocumental
  → Almacenamiento
  → respuesta y actualización de lista
```

### Capacidades de carga identificadas

- Selección múltiple y arrastrar/soltar.
- Tabla con nombre, tamaño, advertencias y acciones por archivo.
- Tipologías de la subserie o serie del expediente.
- Sugerencia de tipología por coincidencias con el nombre del archivo.
- Fecha opcional por archivo, con validaciones JavaScript.
- Vista previa de imágenes y PDF; intento de vista Office mediante Google Viewer.
- Eliminación individual o total de pendientes.
- Guardado individual o secuencial por lote.
- Bloqueo JavaScript de doble activación específico de Producción.
- Configuración de extensiones y tamaño máximo.
- Preparación de clasificación, gabinete y campos heredados del expediente.
- Tratamiento especial de archivos `.TIF` multipágina.
- Persistencia, inventario e índice electrónico según configuración.
- Respuesta con identificadores de documento y registro, gabinete y datos visuales.
- Limpieza de temporales tras almacenamiento exitoso.

La digitalización abre una ruta distinta hacia `workflow/WebFormEscan.aspx`. También permanecen controles antiguos como `AjaxFileUpload_dowload`, con ramas `ESCANER` y `VISOR`; no deben confundirse con la carga actual `PRODUCCION`.

### Hallazgos que condicionan la extensión

| Hallazgo | Implicación |
|---|---|
| La autorización inicial no se vuelve a comprobar de manera equivalente en la rama de carga revisada. | Validar el destino y permiso al ejecutar cada operación, usando contexto persistido. No se realizó una prueba de acceso indebido. |
| El cliente controla tamaño y `accept`; la validación explícita de extensión en la preparación está comentada. | Aplicar en servidor la política específica del perfil antes de almacenar. La resolución posterior de extensión no equivale a esa política completa. |
| El temporal usa directamente `path & file.FileName`. | Normalizar nombres y utilizar referencias únicas por lote/archivo; comprobar contención de rutas. |
| Persistencia, limpieza y proyección pueden fallar en momentos distintos. | Separar resultado documental confirmado de fallos posteriores; reconciliar antes de reintentar. |
| Existen varias representaciones de la cola: `input.files`, caché, filas y arreglo de envío. | Usar identificadores estables; evitar resolver archivos solamente por nombre o índice de fila. |
| La obligatoriedad consulta `setioption_obliga_tipologia`, distinta de la opción declarada. | Corregir o sustituir esa validación al integrar el nuevo flujo. |
| Hay lecturas de `ex.mensaje` y ramas de timeout que no resuelven promesas. | Evitar heredar silenciosamente esos comportamientos. |
| El preview Office utiliza una URL `blob:` local con un visor remoto. | No tratar esa vista como capacidad funcional confirmada. |
| El almacenamiento usa sesión. | Adaptarlo a contexto explícito antes de ejecutarlo fuera de la petición web. |

La ruta antigua de VISOR contiene una llamada antivirus; eso no demuestra que la ruta actual de Producción disponga de esa protección.

### Correcciones ya presentes y verificación realizada

Se confirmó en código la tolerancia a `radicado_radicacion` ausente, las validaciones de estructuras/configuración, la corrección de la fila de resultado de Producción, el bloqueo de doble activación y los códigos de error por etapa.

Se ejecutaron estas suites locales:

```text
tests/doc88-produccion-carga-almacenamiento.test.cjs
tests/doc88-produccion-carga-configuracion.test.cjs
tests/doc88-produccion-carga-preparacion.test.cjs
tests/doc88-produccion-carga-tipologias.test.cjs
tests/doc88-produccion-proyeccion.test.cjs
tests/doc88-produccion-upload-contract.test.cjs
```

Resultado: 14 pruebas aprobadas, 0 fallidas. Son verificaciones estáticas de contratos/código; no validan ejecución real de ASP.NET, navegador, base de datos ni almacenamiento físico.

## 5. Reutilización de configuración interna y SII

### Configuración existente

| Tabla | Responsabilidad observada |
|---|---|
| `ra_auto_registro_expediente` | Nombre del autorregistro y función que proporciona datos. |
| `ra_auto_campos_gestion_expediente` | Fondo, instrumento, área, serie y subserie. |
| `ra_auto_campo_unico_expediente` | Campos del expediente, obligatoriedad e identidad. |

Estas tablas constituyen un antecedente funcional. La decisión posterior es mantener clasificación propia en una versión del perfil nuevo que referencia catálogos existentes, sin identidad ni dependencia editable de autorregistro. No insertar configuración de carpetas en esas tablas ni activar sus efectos de negocio.

### Matriz de componentes

| Componente | Reutilización propuesta | Restricción actual |
|---|---|---|
| `Registrar_Expediente_Conservacion` | Creación, índice XML y relación con nivel. | Firma extensa y validaciones legacy; requiere un adaptador con datos completos. |
| `PreAlmacenaDocumentoProduccion` y `AlmacenamientoDocumentoProduccionDocumental` | Preparación y almacenamiento en gabinete con expediente. | Dependencias de sesión y resultados textuales/ByRef. |
| `ImportExpedientCoordinator` | Patrón buscar/verificar/crear y resolver conflictos. | Depende de inscripciones y resolución de sujeto SII. |
| `ImportServiceOrchestrator` | Coordinación, versiones, exclusión concurrente y estados persistidos. | Contexto y fases ligados a Workflow/SII. |
| `ImportIntentStateMachine` | Transiciones auditables y distinción de resultados inciertos. | Incluye estados específicos SII. |
| `DocumentExpedientRelationAdapter` | Verificar vínculo antes/después y detectar relaciones cruzadas/duplicadas. | Exige contexto de tarea. |
| Repositorios de intención y reconciliación | Patrón de persistencia y recuperación de resultados. | Esquema y consultas contienen datos de tarea, trámite e inscripción. |
| `LegacyImportDocumentStorageAdapter` | Referencia de adaptación a almacenamiento existente. | Invoca almacenamiento Workflow y exige `IdTareaWorkflow`; no es reutilizable directamente para carpetas. |

SII no es actualmente un motor genérico al que baste agregar un proveedor. La matriz anterior distingue operaciones potencialmente consumibles sin cambios de patrones que sirven únicamente como referencia. Toda adaptación se implementará en componentes nuevos de la importación por carpetas; no se adaptarán los componentes de SII.

## 6. Perfil de importación propuesto

Nombre provisional: `perfil_importacion_carpetas`. Es una propuesta conceptual, no un DDL aprobado.

| Grupo | Datos |
|---|---|
| Identificación | Nombre, versión, empresa, activo y permisos de uso. |
| Clasificación | Fondo/instrumento/área/serie/subserie seleccionados en la versión propia del perfil, referenciando catálogos existentes. |
| Expediente | Tipo, nivel padre, responsable, visibilidad, fechas y demás campos obligatorios. |
| Nombre | Mapeo del nombre de carpeta al campo visible del expediente. |
| Gabinete | Destino y mapeo de campos obligatorios compatibles con la clasificación; valores por documento extraídos del nombre original del archivo cuando el perfil lo indique. |
| Documentos | Clase documental, tipología predeterminada o reglas de asignación. |
| Carpetas | Interpretación de raíces, subcarpetas, vacías y archivos sueltos. |
| Coincidencias | Rechazo, continuación del mismo lote o incorporación a expediente existente verificado. |
| Límites | Extensiones, tamaño por archivo y límites del lote. |

Los campos variables pueden administrarse mediante una tabla de reglas relacionadas, con campos y fuentes permitidos. El perfil no debe ejecutar SQL ni funciones arbitrarias definidas por el usuario.

Cada lote conservará una copia de la configuración efectiva y la versión del perfil. Los cambios posteriores no deben alterar el lote iniciado.

El acceso al perfil no concede automáticamente permiso para crear expedientes o escribir en un gabinete. La autorización se valida en servidor.

### Campos índice del gabinete obtenidos del nombre del archivo

Requisito indicado por el usuario: algunos campos índice del almacenamiento en gabinete pueden obtener su valor del nombre del archivo. El perfil debe soportar este origen por documento; no todos los valores del gabinete son constantes del perfil ni se heredan del expediente.

Las fuentes de metadatos quedan diferenciadas:

- Tabla interna del perfil: valores predeterminados y clasificación.
- Carpeta: nombre visible y otros valores expresamente configurados para el expediente.
- Nombre original del archivo: valores variables de los campos índice del gabinete.
- Expediente resuelto: campos heredados mediante relaciones existentes, cuando corresponda.

Ejemplo ilustrativo proporcionado por el usuario: `1246566-125-OrdenCompra`. No define un formato obligatorio ni un perfil aprobado. No se fijan tres partes, el guion, el orden de los datos ni campos de orden de compra en el diseño. La estructura física debe mantenerse simple y su interpretación configurable por perfil. En este ejemplo se trabaja con el nombre sin extensión.

```text
Nombre sin extensión: 1246566-125-OrdenCompra
Separador configurado: -

Parte 1 → número de orden: 1246566
Parte 2 → identificación del tipo documental: 125
Parte 3 → descripción del documento: OrdenCompra
```

Cada perfil definirá si utiliza el nombre completo o extrae partes, cuántas reglas necesita y qué campo recibe cada valor. La tipología solo se obtendrá del nombre cuando el perfil lo indique; puede proceder de otra fuente permitida. Los nombres físicos de columnas no se presuponen ni se fijarán en JavaScript.

| Configuración ilustrativa, no obligatoria | Valor del ejemplo |
|---|---|
| Fuente | Nombre original sin extensión. |
| Modo de lectura | Separador y posición, con posiciones visibles desde 1. |
| Separador | Guion `-`, editable por perfil. |
| Parte 1 | Campo índice de número de orden; conservar como texto si el esquema lo permite para no perder ceros iniciales. |
| Parte 2 | Identificador del tipo documental en el catálogo configurado y, si corresponde, su campo índice. |
| Parte 3 | Campo descriptivo del documento. |
| Descripción con guiones | Propuesta: opción «usar el resto del nombre desde esta posición»; en este caso, desde la parte 3. |
| Formato incorrecto | Mostrar el error antes de almacenar; no inventar ni sustituir silenciosamente valores obligatorios. |

La configuración se presentará como una tabla simple de reglas, destino y obligatoriedad, acompañada de una vista previa. No se exige un archivo auxiliar, JSON, XML ni expresiones regulares para interpretar este ejemplo. La cantidad de reglas, separador, posiciones, destinos y conversiones pueden cambiar entre perfiles sin modificar código ni imponer que todos los archivos adopten este nombre.

Cuando una regla mapee un valor a una tipología, deberá validarse contra el catálogo configurado: no se asume que coincida con un identificador de lista de chequeo, formato físico o clase documental. Si es un identificador interno, se comprobará su existencia y compatibilidad con serie/subserie; si es un código de origen, deberá existir una correspondencia explícita. Una descripción extraída del nombre no reemplaza por sí sola la descripción oficial de la tipología. Fondo, instrumento, área, serie y subserie siguen procediendo del perfil interno. Estas validaciones son generales y no dependen de los valores `125` ni `OrdenCompra` del ejemplo.

La tabla de reglas del perfil debería representar:

| Dato | Propósito |
|---|---|
| Campo destino | Campo real del gabinete o dato documental de destino permitido, validado contra su esquema o catálogo. |
| Fuente | Nombre original completo o nombre sin extensión. |
| Extracción | Valor completo, segmento por separador, rango de caracteres o patrón controlado. |
| Parámetros | Separador/posición o patrón y grupo; convenciones explícitas de posición. |
| Conversión | Texto, número o fecha con formato y cultura definidos según el campo destino. |
| Validación | Obligatoriedad, longitud, formato y restricciones del campo. |
| Ausencia o fallo | Bloqueo del documento o valor predeterminado explícito; nunca sustitución silenciosa. |
| Precedencia | Fuente autoritativa cuando también hay un valor fijo o heredado para el mismo campo. |

Como propuesta de interfaz sencilla se contemplan nombre completo y extracción por separador y posición, con opción de capturar el resto del nombre desde una posición configurable. Los métodos definitivos se definirán con los perfiles requeridos, sin convertir el ejemplo en una restricción. Si se requieren expresiones regulares, deben tener límites de entrada y tiempo de ejecución; no se ejecutarán scripts ni SQL proporcionados por el perfil.

El servidor obtiene y conserva el nombre original asociado al archivo recibido. La extracción no utiliza el nombre temporal generado ni acepta los valores calculados por JavaScript como autoridad. El nombre es un dato no confiable: los campos destino se resuelven contra una lista permitida y los valores se validan y persisten con parámetros en los límites nuevos. Los adaptadores deben comprobar además las restricciones del almacenamiento legacy consumido sin cambios.

Antes de ejecutar, la vista previa mostrará archivo, regla, campo destino, valor obtenido y errores. Un campo obligatorio no resuelto impide almacenar ese documento; se detectará en la validación previa para evitar una incorporación parcial inesperada. La política del lote determinará si otras carpetas o archivos válidos pueden continuar.

El plan persistido conservará la versión de la regla, nombre original, valor extraído y valor final tipado. Los reintentos del mismo lote reutilizarán ese plan y comprobarán la identidad del archivo, sin recalcularlo con una versión nueva del perfil. Deben preservarse ceros iniciales cuando el campo sea textual y rechazarse fechas ambiguas sin un formato explícito.

La composición de valores debe producir una sola asignación final por campo. No se aplicará «el último valor gana» entre datos heredados, constantes y nombre del archivo: las colisiones se resuelven mediante una precedencia declarada o se reportan como conflicto. Si el almacenamiento actual sobrescribe un valor, el adaptador nuevo debe resolver esa incompatibilidad sin modificar SII ni sus dependencias compartidas.

Esta capacidad pertenece al proveedor de metadatos y a las reglas del perfil de la nueva integración. La obtención del contenido, creación de expedientes y garantías de ejecución mantienen sus responsabilidades.

Al configurar cada perfil se definirán sus reglas, campos reales y tipos, catálogos aplicables y política ante archivos inválidos. No hay un perfil de órdenes de compra aprobado ni una dependencia pendiente sobre el valor `125`: el nombre aportado solo ilustra la capacidad de configuración solicitada.

## 7. Interpretación de carpetas

Se proponen dos modalidades:

1. Carpetas seleccionadas: cada raíz añadida representa un expediente.
2. Carpeta contenedora: cada subcarpeta inmediata representa un expediente.

Ejemplo de carpeta contenedora:

```text
Importacion/
├── Contrato 001/
│   ├── Contrato.pdf
│   └── Anexos/
│       └── Identificacion.pdf
└── Contrato 002/
    └── Contrato.pdf
```

Modalidad inicial consolidada: seleccionar `Contrato 001` y `Contrato 002` como dos raíces para crear esos dos expedientes; `Importacion` es solo su contenedor físico en el ejemplo. Seleccionar únicamente `Importacion` no equivale a seleccionar sus hijas: esa otra estrategia no está incluida. Incorporar documentos de `Anexos` al primero solo si el perfil permite incluir subcarpetas; no crear niveles de Producción a partir de ellas.

El nombre visible no debe utilizarse como única identidad. Carpetas homónimas de raíces distintas no se fusionarán automáticamente. Se necesita un identificador de raíz y ruta relativa dentro del lote, además de una política explícita para coincidencias entre lotes.

La función actual limita código único a 45 caracteres y alias a 120. Debe comprobarse el campo visible apropiado y mostrar errores antes de ejecutar, sin truncamiento silencioso.

## 8. Flujo propuesto

1. Seleccionar un nivel en el árbol existente de Producción y abrir la ventana con ese contexto validado y de solo lectura; seleccionar perfil compatible y añadir carpetas.
2. Construir un inventario de carpetas y archivos con identificadores estables y rutas relativas.
3. Validar en servidor clasificación, campos, permisos, gabinete, tipologías y límites.
4. Mostrar los expedientes que se crearían, los documentos y los conflictos, sin crear todavía expedientes.
5. Registrar el lote y recibir archivos temporales mediante referencias únicas.
6. Resolver o crear el expediente de cada carpeta autorizada.
7. Almacenar cada documento e incorporar al expediente correspondiente.
8. Verificar documento, relación e índice aplicable.
9. Persistir resultados y ofrecer recuperación e informe final.

Si la operación de almacenamiento ya crea la relación y el índice, la fase posterior debe verificarlos. No se debe añadir otra incorporación que duplique esos efectos.

## 9. Persistencia operativa y recuperación

El perfil resuelve configuración; la ejecución necesita registros adicionales. Nombres conceptuales:

| Registro | Contenido |
|---|---|
| Lote | Usuario/empresa, origen, perfil efectivo, estado, fechas y totales. |
| Carpeta del lote | Raíz, ruta relativa, nombre, expediente resultante y estado. |
| Archivo del lote | Identificador, carpeta, ruta relativa, tamaño, huella, tipología, temporal, documento resultante y fases. |
| Eventos | Transiciones, errores, correlación y decisiones de recuperación. |

Principios:

- Una carpeta procesada no debe crear otro expediente al reanudar el mismo lote.
- Un archivo almacenado no debe volver a almacenarse porque falló la presentación o la incorporación posterior.
- Ante resultado incierto, consultar evidencia persistida antes de repetir efectos.
- Cancelar detiene pendientes; no elimina automáticamente documentos ya incorporados.
- No asumir una transacción global entre todos los archivos, base de datos e índice XML.
- La huella permite verificar integridad y coincidencias, pero no justifica deduplicación global: un mismo contenido puede pertenecer a distintos expedientes.
- Las operaciones concurrentes sobre un mismo expediente deben coordinarse, especialmente al modificar índices y orden documental.

## 10. Origen y ejecución prolongada

El origen de clasificación es la tabla interna del perfil. El alcance inicial consolidado usa carpetas del dispositivo. La alternativa servidor/red siguiente es referencia de evolución, no parte de la entrega actual.

| Origen de carpetas | Mecanismo |
|---|---|
| Computador del usuario | Selector de directorios y carga de archivos con ruta relativa. |
| Servidor o red | Enumeración de una ubicación autorizada/configurada mediante un adaptador de origen. |

En navegador, `webkitdirectory` y `webkitRelativePath` permiten recuperar archivos y su jerarquía relativa. Añadir varias raíces sucesivamente evita depender de que un selector permita seleccionarlas juntas. Una lista de archivos no representa por sí sola carpetas vacías.

`showDirectoryPicker` permite trabajar con un manejador de directorio, pero tiene disponibilidad limitada y requiere contexto seguro. La compatibilidad se deberá verificar contra los navegadores objetivo.

La primera entrega avanza mediante operaciones acotadas por petición y conserva estado persistido. Un ejecutor desatendido durable se contempla exclusivamente en el prompt opcional 13. El orquestador SII revisado procesa elementos dentro de `Execute`; eso no demuestra la existencia de una cola independiente de la petición web.

El almacenamiento que utiliza sesión debe recibir autoridad y contexto explícitos antes de ejecutarse fuera de una petición. Si los archivos aún están en el computador del usuario, el servidor no puede continuar recibiéndolos tras cerrar el navegador; la recuperación debe contemplar nueva selección o un mecanismo compatible de acceso autorizado. Los archivos ya recibidos sí pueden quedar disponibles para procesamiento posterior.

## 11. Control de deuda técnica

La solución conserva una arquitectura sostenible si:

- Separa obtención de datos, validación, creación e incorporación.
- Mantiene una única fuente de clasificación por perfil.
- Reutiliza operaciones de dominio a través de adaptadores, sin copiar grandes bloques SII.
- Evita tareas, trámites o inscripciones ficticias.
- Sustituye dependencias de sesión por parámetros explícitos en los límites que deban ejecutarse fuera de la petición.
- Versiona perfiles y conserva resultados por carpeta/archivo.
- Distingue persistencia documental de fallos posteriores y ofrece reintentos verificables.
- Define contratos propios para la nueva integración y reutiliza componentes existentes únicamente sin modificarlos, verificando que su consumo no altere SII.

La tabla interna es una fuente de datos natural. El riesgo de deuda está en acoplar esa fuente al funcionamiento específico de SII o en extender el handler compartido sin límites claros.

## 12. Alcance inicial recomendado y decisiones pendientes

Primera entrega propuesta:

- Perfil versionado y validación previa.
- Un expediente por carpeta destino y un gabinete por perfil.
- Nombre visible derivado de carpeta.
- Tipología por fuente configurada en el perfil, constante o extraída/resuelta según sus reglas.
- Vista previa de la importación sin efectos documentales.
- Recepción temporal, estado persistido por archivo y recuperación del lote.
- Verificación de incorporación e informe final.

Pendiente de decidir:

1. Parámetros operativos de integridad y pruebas de concurrencia; los dos ámbitos, expediente y lote, ya están aprobados como configurables en el perfil.
2. Opciones de tratamiento de subcarpetas que se habilitarán; la modalidad inicial es una raíz seleccionada por expediente y el selector no detecta vacías. No interpretar archivos sueltos sin regla explícita.
3. Campo visible del expediente y política de generación de código único.
4. Permisos concretos de administración/publicación/uso del perfil y compatibilidad con niveles existentes; clasificación propia sin autorregistro ya está decidida.
5. Reglas de tipología y fecha documental.
6. Política de coincidencias entre importaciones distintas.
7. Cantidad de archivos, tamaño máximo, volumen total y concurrencia objetivo.
8. Caracterización de adaptadores, ventanas de fallo y recepción/recuperación según volumen; el ejecutor desatendido no es requisito de la primera entrega.

No se establece todavía una estimación de tiempos o capacidad máxima: requiere resolver esos puntos y caracterizar las dependencias de almacenamiento.

## 13. Extensión como servicio de integración documental

### Decisión arquitectónica propuesta

La importación por carpetas debe plantearse como una capacidad de un servicio de integración documental. Una fuente local también requiere integración: identificar elementos, obtener contenido y metadatos, convertirlos al modelo documental, validar y confirmar sus efectos.

Servicio de integración no implica crear un microservicio ni consumir HTTP para consultar una tabla interna. Inicialmente puede ser un servicio de aplicación dentro de la solución ASP.NET, con contratos que permitan separar el ejecutor si el volumen lo exige. Producción Documental actuaría como interfaz de esta capacidad.

### Separación de responsabilidades

| Pieza propuesta | Responsabilidad | Ejemplos |
|---|---|---|
| Conector de origen | Enumerar recursos y obtener su contenido mediante referencias controladas. | Archivos recibidos del navegador o directorio autorizado del servidor. SII se mantiene fuera de esta nueva composición. |
| Proveedor de metadatos | Resolver los datos necesarios sin producir efectos documentales. | Perfil de tabla interna, metadatos de API, manifiesto. |
| Perfil versionado | Declarar clasificación, gabinete, mapeos y políticas permitidas. | Una configuración por proceso documental. |
| Estrategia de importación | Interpretar agrupaciones, identidades y reglas específicas que no sean simples valores configurables. | Un expediente por carpeta u otras estrategias futuras de la nueva integración. La resolución SII es solo referencia. |
| Plan normalizado | Expresar expedientes, documentos, destinos, metadatos y efectos previstos sin depender del transporte. | Plan validado que se conserva con el lote. |
| Servicio común | Autorizar, coordinar fases, persistir avances, controlar concurrencia, reconciliar y reportar. | Ejecución recuperable por carpeta y documento. |
| Adaptadores de destino | Crear expedientes, almacenar y verificar incorporación usando operaciones existentes. | Puentes a Producción y al almacenamiento documental. |

Para carpetas locales, el origen de contenido y el de metadatos son diferentes: los archivos vienen del dispositivo o servidor; la clasificación viene de la tabla interna. No deben fusionarse en un conector monolítico.

### Particularidades de cada importación

Se deben resolver mediante configuración cuando sean valores o reglas simples: gabinete, clasificación, campo de nombre, tipología predeterminada, patrón de asignación y tratamiento de subcarpetas.

Cuando cambie la interpretación del negocio, se utilizará una estrategia registrada y probada: identidad de expediente, agrupación de documentos o resolución de una inscripción. No se recomienda un motor de scripts arbitrarios ni agregar ramas por cliente/proveedor en el orquestador común.

Las estrategias producen un plan; no pueden omitir autorización, validación de contenido, control de duplicación de efectos o auditoría. Las operaciones externas irreversibles que un proveedor requiera deben modelarse como pasos específicos con resultado persistido, no como efectos ocultos del conector de lectura.

### Evidencia y límites de reutilización

El repositorio contiene `RegistroProveedoresImportacion`, `RegistroClientesProveedoresImportacion`, `IExternalImportProvider`, `IExternalImportProviderClient` e `IImportExecutionStep`. Son puntos de partida reales para la composición por capacidades.

Sin embargo:

- `ContextoImportacionServicio` contiene tarea, ruta y trámite Workflow.
- `DownloadImportExecutionStep` realiza un `TryCast` a `SiiImportProvider` para obtener metadatos SII.
- `PrepareImportExecutionStep` consulta `MetadatosSii.Formato` y materializa el contenido en un archivo temporal.
- `DownloadAsync` devuelve `Byte()`, por lo que el contrato actual materializa el documento en memoria.
- Los planes y repositorios contienen identidad de inscripción y destino de tarea.

Por tanto, la fuente local puede incorporarse al concepto de integración, pero no debe agregarse al recorrido SII existente. Se definirán contratos y una composición propios. Los contratos SII acoplados a Workflow se tomarán como referencia, sin extraerlos, generalizarlos, renombrarlos ni modificar sus consumidores.

Para cargas grandes, el contrato nuevo de contenido debería ofrecer una referencia temporal controlada o flujo de lectura, con tamaño, huella y ciclo de vida definidos. No debería exigir cargar todos los documentos completos en memoria.

### Superficie funcional sugerida

Operaciones conceptuales del servicio: preparar inventario, validar plan, recibir contenido cuando corresponda, iniciar ejecución, consultar estado, solicitar detención, reconciliar y reanudar operaciones seguras. No son endpoints implementados.

Una carpeta del computador del usuario no es una ruta que ASP.NET pueda leer directamente. El conector local del servidor debe operar sobre archivos efectivamente recibidos y vinculados al lote autorizado. Una carpeta del servidor es otro conector, con ubicaciones permitidas. Esta distinción debe mantenerse aunque ambos usen el mismo perfil y estrategia.

### Evolución recomendada

1. Implementar la capacidad de carpetas con contratos mínimos propios de origen, metadatos, plan y destino, evitando dependencias ficticias de Workflow.
2. Reutilizar mediante adaptadores nuevos solo las operaciones documentales existentes que puedan consumirse sin cambios y con efectos compatibles.
3. Implementar estado y reconciliación propios tomando SII como referencia cuando los componentes existentes no sean consumibles directamente. No extraer ni refactorizar código de SII.
4. Añadir futuras importaciones a la nueva composición mediante conectores, perfiles o estrategias propios, según qué responsabilidad cambie.

El servicio común propuesto será común a las nuevas importaciones, no una sustitución del servicio SII. No promete ausencia total de deuda: admite una separación deliberada para preservar SII, evitando copiar su implementación completa o acoplar el nuevo recorrido a sus particularidades.

### Límite obligatorio de aislamiento respecto de SII

- No modificar código, contratos, endpoints, registro de proveedores, composición, configuración, esquema ni reglas de ejecución de SII.
- No reutilizar sus tablas operativas para registrar lotes de carpetas ni introducir datos ficticios de tarea, trámite o inscripción.
- No modificar un componente compartido consumido por SII para satisfacer la nueva importación. Si requiere cambios, crear una implementación o adaptador nuevo fuera de ese recorrido.
- Evaluar la reutilización por contrato y efectos: poder llamar una función no garantiza que sea adecuada si depende de sesión, configuración mutable o escrituras específicas de SII.
- Mantener el almacenamiento e incorporación propios del negocio documental cuando puedan invocarse sin cambios; documentar qué se reutiliza realmente y qué se toma solo como referencia.
- Revisar los cambios y ejecutar verificaciones pertinentes para demostrar que la nueva composición no altera SII. Las pruebas no autorizan refactorizaciones que incumplan esta restricción.

## 14. Referencias del repositorio

Rutas relativas a la raíz del repositorio:

- `Gestion/WebFormProducionDocumental.aspx`: menús y controles.
- `Gestion/WebFormProducionDocumental.aspx.vb`: eventos, digitalización y controles antiguos.
- `js/gestion/WebFormProducionDocumental.js`: activación de carga.
- `generic_control/FileUploadHandler.js`: `LoadFilePERSON`, preparación, envío y proyección.
- `generic_control/FileUploadHandler_.ashx.vb`: recepción y despacho compartido.
- `js/java_general/JSProgresBar.js`: ejecución secuencial de la interfaz.
- `workflow/ClassAlmacenamiento.vb`: almacenamiento de Producción y genérico.
- `Gestion/ClassGaExpediente.vb`: creación e incorporación de expedientes.
- `Gestion/Class_ra_auto_registro_expediente.vb`: configuración y origen de datos de autorregistro.
- `Gestion/Class_ra_auto_campos_gestion_expediente.vb`: clasificación interna.
- `Gestion/Class_ra_auto_campo_unico_expediente.vb`: campos, obligatoriedad e identidad.
- `Services/Workflow/ImportarServicioWeb/ImportExpedientCoordinator.vb`.
- `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`.
- `Services/Workflow/ImportarServicioWeb/ImportIntentStateMachine.vb`.
- `Services/Workflow/ImportarServicioWeb/ValidadorContextoImportacion.vb`.
- `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportExpedientConfigurationRepository.vb`.
- `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb`.
- `Infrastructure/Workflow/ImportarServicioWeb/Expedients/ModernExpedientMutationBridge.vb`.
- `Infrastructure/Workflow/ImportarServicioWeb/Expedients/LegacyPhysicalExpedientGateway.vb`: contiene `ModernPhysicalExpedientGateway`.
- `Infrastructure/Workflow/ImportarServicioWeb/Expedients/DocumentExpedientRelationAdapter.vb`.
- `Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb`.

Documentación de APIs de navegador consultada:

- [MDN: webkitdirectory](https://developer.mozilla.org/en-US/docs/Web/API/HTMLInputElement/webkitdirectory).
- [MDN: webkitRelativePath](https://developer.mozilla.org/en-US/docs/Web/API/File/webkitRelativePath).
- [MDN: showDirectoryPicker](https://developer.mozilla.org/en-US/docs/Web/API/Window/showDirectoryPicker).
