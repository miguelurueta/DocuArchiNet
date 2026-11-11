# Carga de Excel en tablas de índices

Exploración del 10 de noviembre de 2026.

El apartado **Diseño técnico resuelto del 11 de noviembre de 2026** concreta las decisiones de implementación solicitadas por el usuario y es la referencia vigente para los detalles que antes figuraban como propuestas.

## Regla de entrega y revisión de SQL

Por instrucción del usuario, los prompts generan las modificaciones SQL como archivos nuevos en Doc/Actualizacion/Importacion/Sql/, con el orden, destino RA/DA, precondiciones y efectos previstos para su ejecución manual. El usuario ejecuta los archivos y confirma el resultado. Después el agente comprueba mediante consultas exclusivamente SELECT y registra evidencia saneada. Las correcciones de esquema, índices, datos de preparación o permisos siguen la misma secuencia mediante otro archivo.

El agente no aplica los archivos SQL ni ejecuta modificaciones de instalación o mantenimiento de forma directa o mediante tests, scripts de arranque o rutas indirectas de la aplicación. La instalación permanece PENDIENTE_EJECUCION_MANUAL hasta la confirmación del usuario. La verificación posterior debe acreditar lo aplicado antes de utilizarlo en las pruebas dependientes. El código funcional del cargador se desarrolla conforme al alcance acordado para su operación por el usuario.

## Propósito

Crear un módulo administrativo sencillo para cargar datos de Excel en tablas `imp_02_`. Estas tablas sirven como fuente para completar índices de documentos en la importación de carpetas. El módulo se ocupa de las filas y columnas de la tabla. Los PDF se reciben y almacenan mediante el flujo de carpetas existente.

La primera entrega admitirá archivos **XLS y XLSX**, según la decisión vigente del usuario del 11 de noviembre de 2026. CSV queda aplazado para una ampliación posterior.

## Lo que ya existe

El proyecto anterior, ubicado en `D:\imagenesda\desarrollo\copiaservercamara\Proyectos-General\preproyectoimport\preproyectoimport`, permite elegir una hoja de Excel, mapear columnas, crear una tabla de destino, agregar registros y vaciar los datos antes de volver a cargar. Genera nombres con el prefijo `imp_02_`. La migración de documentos es una operación aparte dentro de ese programa.

La clave primaria que genera el cargador anterior, `IMP_02_ID_CLAVE`, es un identificador autoincremental. La validación de unicidad de una columna de negocio, como `CONSECUTIVO` o `NUMERO_CONTRATO`, es el aporte propuesto. Las referencias del programa anterior son `ClassCargarDatos.vb`, líneas 91, 120 y 327, y `ClassPlantillas.vb`, línea 530.

## Relación con la importación de carpetas

El perfil consulta la tabla específica que tiene configurada su proveedor. La consulta compara la clave con una columna de la fuente autorizada. Una coincidencia aporta los valores de índices configurados, cero coincidencias indican ausencia y varias coincidencias producen un resultado ambiguo. El perfil puede obtener la clave del nombre del archivo, de la carpeta, de una propiedad del expediente o de un valor fijo. La regla vigente admite por defecto el prefijo literal `imp_02`, por lo que los nombres nuevos seguirían la convención `imp_02_<nombre>`. [Consulta de índices](../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorDatosIndices.vb) · [Restricción de nombres](../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/RestriccionTablasFuente.vb) · [Editor del perfil](../../../../js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js)

La unidad es **una fila por clave consultada**. Si la carpeta `CONTRATO_00123` contiene cinco PDF con el mismo contratista y número de contrato, una fila con clave `00123` puede alimentar los índices de los cinco. Si cada documento requiere valores propios, el perfil puede consultar una clave obtenida del nombre de cada archivo.

## Alcance del módulo

Una pantalla bajo **Importación → Cargar datos desde Excel…** permite elegir el archivo, la hoja, la tabla de destino y el modo de carga. La vista previa muestra los encabezados, una muestra de datos y las incidencias antes de confirmar.

| Modo | Comportamiento |
|---|---|
| **Crear tabla** | Crear una `imp_02_` con las columnas seleccionadas y cargar las filas del archivo. |
| **Agregar registros** | Insertar nuevas filas en una tabla existente después de validar la compatibilidad de las columnas. |
| **Reemplazar datos** | Validar el archivo completo y sustituir las filas conservando el nombre, las columnas y la estructura de la tabla. |

El operador configura la columna única. Si la tabla ya alimenta un perfil, la columna de búsqueda del proveedor es la opción predeterminada. Según la decisión aprobada el 11 de noviembre de 2026, esa clave tendrá validación por carga y una restricción permanente mediante un índice UNIQUE, que también permite acelerar su consulta. En una carga nueva o de reemplazo se revisan los duplicados del archivo. Al agregar, también se comprueba si las claves ya existen en el destino. Las incidencias indican qué filas requieren atención antes de cargar. Los códigos con ceros a la izquierda, como `00123`, se tratan como texto cuando ese es su tipo de clave.

## Condiciones para la carga

- Validar encabezados, tipos y columnas de destino. Generar los nombres de tabla y columnas a partir de identificadores válidos y parametrizar los valores de las filas.
- Confirmar la tabla afectada y el número de filas que se reemplazarán. Para tablas transaccionales, ejecutar el borrado y la inserción en una sola transacción una vez validado el archivo, de modo que un fallo durante la carga permita recuperar los datos anteriores.
- Habilitar una tabla nueva como fuente después de comprobar el resultado de la carga. La autorización se gestiona en el catálogo de fuentes vigente y el perfil usa el proveedor asociado. [Administración de fuentes](../../ProduccionDcoumental/ImportacionCarpetas/Diseno/11-administracion-fuentes-da.md)
- Mostrar el resultado con la tabla y el total de filas cargadas. El operador continúa con **Importar carpetas** para cargar los PDF y aplicar sus índices.

Las decisiones sobre la base de destino, el permiso del usuario y la cuenta de conexión quedaron aprobadas el 11 de noviembre de 2026 y se detallan al final de este documento. La autorización de consulta de una fuente y los permisos de escritura del cargador se gestionan como capacidades distintas.

## Revisión arquitectónica del 11 de noviembre de 2026

La revisión siguiente amplía el alcance explorado con lectura del proyecto antiguo, de los consumidores actuales y con consultas SELECT de metadatos mediante la conexión configurada del repositorio. El servidor consultado reporta MySQL `5.1.37-community`. En el esquema `docuarchi` se verificaron las tres tablas de configuración, dos claves foráneas y las estructuras de `imp_02_egresos` e `imp_02_contratos`. Las cinco tablas examinadas son InnoDB y usan `latin1_swedish_ci`. La consulta de triggers sobre esas cinco tablas devolvió cero resultados. La inspección acredita ese servidor y esas estructuras a la fecha indicada. La resolución de DA y RA de una sesión autenticada pertenece a la verificación de integración.

### Tablas existentes que participan

| Tabla observada en `docuarchi` | Estructura comprobada | Uso previsto |
|---|---|---|
| `imp01_xmlconexion` | 9 columnas, PK `ID_XMLCONEXION`, nombre de conexión único | Conservar la relación histórica de la plantilla con su conexión. Vincular explícitamente la conexión elegible con el destino autorizado de la sesión web. |
| `imp01_plantillaimp` | 10 columnas, PK `ID_PLANTILLAIMP`, `NOMBRE_PLANTILLA` único y FK hacia `imp01_xmlconexion` | Reutilizar la cabecera de la plantilla, el nombre de destino y las opciones históricas de carga. |
| `imp01_campos_plantilla` | 10 columnas, PK `ID_CAMPOS_PLANTILLA` y FK hacia `imp01_plantillaimp` | Reutilizar el mapeo entre encabezados de archivo y columnas de destino, sus tipos y las transformaciones configuradas. |
| `imp_02_egresos` | 6 columnas `varchar(250)` admitiendo NULL, sin índices declarados | Ejemplo real de tabla existente que puede recibir filas. |
| `imp_02_contratos` | 4 columnas `varchar(250)` admitiendo NULL, sin índices declarados | Segundo ejemplo real de destino compatible con el mecanismo de consulta. |

Las columnas observadas de `imp01_plantillaimp` son `ID_PLANTILLAIMP`, `IMP01_XMLCONEXION_ID_XMLCONEXION`, `NOMBRE_PLANTILLA`, `CONEXION_FUENTE`, `LIBRO_EXCEL`, `RUTA_ARCHIVO_FUENTE`, `ELIMINA_TABLA`, `ELIMINA_INSERTA_RECORD`, `INSERTA_RECORD` y `ESTADO_PLANTILLA`. `LIBRO_EXCEL` admite NULL. `RUTA_ARCHIVO_FUENTE` es obligatorio y tiene longitud 120.

Las columnas de `imp01_campos_plantilla` son `ID_CAMPOS_PLANTILLA`, `IMP01_PLANTILLAIMP_ID_PLANTILLAIMP`, `CAMPO_FUENTE`, `CAMPO_DESTINO`, `TIPO_CAMPO`, `REMPLAZA_COMA`, `REMPLAZA_PUNTO`, `OPCION_SPLITER`, `CARACTER_SPLITER` y `NUMERO_MATRIZ_EXPLITER`. Los nombres físicos, incluida su ortografía histórica, forman parte de la compatibilidad.

`imp01_xmlconexion` conserva metadatos y credenciales históricas. El acceso web debe resolver la conexión autorizada en el servidor, comprobar su correspondencia con la referencia de la plantilla y entregar al navegador identificadores y nombres descriptivos. La existencia de esa referencia histórica requiere comprobar el destino concreto antes de permitir una carga.

Las columnas reales de `imp_02_egresos` son `CODIGOTIPO`, `TIPO`, `CONSECUTIVO`, `FECHA`, `NIT` y `BENEFICIARIO`. Las de `imp_02_contratos` son `FECHA`, `NCONTRATO`, `NIT` y `NOMBRE`. Esto demuestra que el ID autoincremental generado por el creador antiguo es una variante de estructura. La carga de una tabla existente debe construir el INSERT con sus columnas efectivas y respetar tipos, valores por defecto y nulabilidad.

### Consumidores actuales y catálogos de integración

DocuArchi ya consulta las cabeceras y los campos de estas plantillas desde sus clases de Gestión. SII y el servicio de plantillas externas utilizan esos mapeos. Una edición de plantilla necesita comprobar sus consumidores antes de cambiar la relación entre encabezados y campos. [Cabeceras de plantilla](../../../../Gestion/Class_imp01_plantillaimp.vb) · [Mapeos](../../../../Gestion/Class_imp01_campos_plantilla.vb) · [Servicio de plantillas externas](../../../../webservice/WebServicePlantillaExterna.asmx.vb)

También se verificó la existencia física de `ra_integracion_fuente`, `ra_integracion_fuente_columna`, `ra_integracion_fuente_historial`, `ra_integracion_proveedor` y `ra_integracion_proveedor_version`. El cargador consume sus contratos mediante los servicios existentes para conocer el contexto de lectura. El alta de una tabla nueva se conecta con la administración vigente de fuentes y proveedores. Las conexiones lógicas RA y DA se resuelven por sesión, aunque los catálogos observados estén en un mismo esquema físico.

Las tablas `IMP02_PLANTILLA_MIGRACION`, `IMP02_REGISTRO_GENERAL_PLANTILLA` e `IMP_02_REGISTRO_DETALLE_MIGRACION` aparecen en el programa antiguo para migrar documentos entre gabinetes. Esa semántica permite identificarlas como componentes del recorrido de migración. El recorrido de carga tabular utiliza los tres catálogos `imp01_*` y su tabla de datos.

### Registro de ejecución

Los catálogos inspeccionados describen plantillas y columnas. Para consultar si una carga terminó después de perder la respuesta y reconocer una repetición de la misma petición, el diseño utiliza una tabla adicional `imp01_carga_ejecucion`, colocada en la misma base transaccional que el destino de datos.

| Información propuesta | Finalidad |
|---|---|
| ID de carga único | Identificar una ejecución durante preparación, confirmación y consulta. |
| Plantilla, ámbito, usuario, conexión lógica y tabla | Acreditar qué operación puede consultar o confirmar cada usuario. |
| Modo y configuración aplicada | Conservar hoja, formato Excel, campo único y transformaciones efectivamente utilizados. |
| Hash del archivo y de la configuración | Comprobar que la confirmación corresponde al archivo y opciones preparados. |
| Estado, filas confirmadas y fechas | Consultar el resultado persistido de la operación. |
| Código de incidencia | Dar un diagnóstico saneado cuando la carga falla. |

Esta tabla registra las operaciones de carga. Los tres catálogos históricos aportan las conexiones, plantillas y mapeos. El diseño final incorpora en la cabecera la columna única y una revisión de carga, y conserva en cada ejecución una copia de la configuración efectivamente utilizada.

### Recorrido de implementación

1. **Abrir el cargador.** Resolver usuario, empresa y facultad de escritura. Listar las plantillas elegibles con el destino que el servidor haya vinculado a la conexión autorizada. El prefijo `imp_02` filtra nombres, pero la facultad de cargar requiere autorización de escritura.
2. **Elegir o guardar la plantilla.** Reutilizar cabecera y mapeos por ID. Al guardar, escribir cabecera y campos en una transacción y leer las columnas por nombre. Las altas web utilizan en `RUTA_ARCHIVO_FUENTE` una referencia relativa generada por el servidor, descrita en el diseño final. La ruta del archivo recibido la determina el servidor.
3. **Recibir el archivo.** Guardarlo temporalmente bajo una referencia del servidor asociada al usuario y a la carga. Registrar su hash. Comprobar extensión, contenido reconocible y límites configurados de tamaño y filas, con valores iniciales aprobados de 10 MB por archivo y 10.000 filas de datos en la hoja seleccionada.
4. **Leer y preparar.** Obtener encabezados, aplicar mapeos y transformaciones explícitas, convertir tipos y revisar todas las filas. Mostrar una muestra y el total de incidencias, incluyendo números de fila y columna. El archivo completo debe resultar válido antes de reemplazar datos.
5. **Validar el destino.** Contrastar columnas, tipos, longitud, codificación, motor y claves con metadatos actuales. Preparar los identificadores de tabla y columnas mediante validación de nombres y los valores mediante parámetros SQL. Para una fuente compartida entre empresas, aplicar el ámbito a inserciones, comparación de claves y reemplazo de filas.
6. **Confirmar la carga.** Verificar otra vez autoridad, archivo, opciones y estructura. Serializar las cargas del módulo sobre la misma tabla. Abrir una transacción InnoDB y bloquear el registro de ejecución. Consultar un resultado ya confirmado cuando se repita su ID. Para una ejecución pendiente, comprobar conflictos, ejecutar la carga y guardar el resultado confirmado en la misma transacción.
7. **Consultar el resultado.** Devolver la tabla, modo y filas confirmadas. Una respuesta perdida se resuelve consultando el mismo ID de carga. El hash del archivo sirve para identificar su contenido, mientras el ID distingue una ejecución de otra carga posterior intencional del mismo archivo.

La interfaz puede ser una página o diálogo desde el menú Importación. Los componentes de producto necesarios son un receptor de archivos, operaciones de listar/guardar plantilla, preparar, confirmar y consultar resultado, un servicio de carga y su repositorio MySQL. La lectura de XLS y XLSX desemboca en la misma representación de filas y usa el mismo validador y escritor de datos.

### Operación de cada modo

**Agregar** inserta las filas validadas y vuelve a revisar las claves contra el destino en la confirmación. Según la decisión aprobada el 11 de noviembre de 2026, una clave repetida dentro del archivo o ya existente en el destino provoca el rechazo completo de la carga, mostrando las claves y filas involucradas. El operador corrige el archivo y vuelve a validarlo.

**Reemplazar datos** ejecuta DELETE e INSERT dentro de la misma transacción InnoDB después de validar el archivo. En una tabla exclusiva se reemplaza su contenido. En una tabla compartida se reemplazan las filas del ámbito autorizado. El resultado de ejecución se confirma junto con las filas. `TRUNCATE`, `DROP TABLE` y otras operaciones DDL tienen commits implícitos en MySQL, por lo que la implementación de reemplazo se apoya en DML transaccional. [Manual de MySQL](https://dev.mysql.com/doc/refman/8.0/en/implicit-commit.html)

**Crear tabla** ejecuta la creación estructural como una fase propia. Después carga las filas y confirma el resultado transaccional. Un fallo puede dejar una tabla creada y vacía, situación que debe quedar identificada para su recuperación. Antes de usarla como fuente se valida su esquema y el resultado de carga mediante los servicios existentes. Las tablas nuevas deben tener tipos que el proveedor soporte, como VARCHAR, enteros, DECIMAL y DATE. La creación necesita respetar la versión real de MySQL observada.

### Unicidad y compatibilidad de datos

La validación por carga detecta claves repetidas según el tipo y la comparación de la columna MySQL. `latin1_swedish_ci` obliga a considerar equivalencias de texto de esa collation. En una tabla compartida, la identidad puede ser el par empresa y clave. Se deben tratar explícitamente los valores vacíos o NULL cuando la columna se usa para buscar índices.

El usuario aprobó combinar la validación por carga con la garantía permanente mediante un índice UNIQUE compatible sobre la clave configurada. La restricción también rige para las herramientas antiguas que escriban en la misma tabla. Un bloqueo cooperativo del nuevo módulo serializa sus propias cargas, mientras el índice único protege contra claves duplicadas entre escritores concurrentes. Para tablas existentes se inspeccionan los datos y los índices antes de preparar la adecuación estructural.

La columna FECHA de los dos ejemplos reales es VARCHAR. El cargador respeta ese esquema al agregar o reemplazar. Una tabla nueva puede declarar DATE, siempre que el formato de entrada y el contrato del proveedor coincidan. El lector debe conservar el texto de claves como `00123` y revisar caracteres que el juego de caracteres de destino pueda representar.

### Lectores y comportamientos antiguos que requieren atención

El proyecto actual usa .NET Framework 4.6.1 y ya referencia FastExcel 3.0.13. Su envoltorio `Class_FastExcel` transforma valores eliminando caracteres como apóstrofes, barras y símbolos, y tiene consumidores de SII. El nuevo lector puede aprovechar la biblioteca donde resulte compatible, con una lectura que conserve los valores y siga el mapeo solicitado. La reutilización de ese envoltorio exige primero comprobar estos comportamientos y la selección de dependencias debe acreditar la lectura de XLS y XLSX. [Lector actual](../../../../FastExcel/Class_FastExcel.vb)

En `ClassCargarDatos.vb` del proyecto antiguo se encontraron los siguientes puntos concretos.

- Las conexiones Excel se configuran con `HDR=YES`, y el recorrido llama a `Read()` antes del `While Read()`. Esa secuencia omite la primera fila de datos.
- La preparación de valores elimina apóstrofes y todas las letras `n`, además de aplicar transformaciones de coma, punto y segmentos según la plantilla.
- El reemplazo borra filas antes de terminar la lectura y preparación del contenido. La propuesta prepara y valida primero.
- La inserción construye SQL por concatenación y ejecuta filas sucesivas. El nuevo escritor utiliza parámetros y una transacción que abarque las filas de la operación.
- `INSERTA_RECORD` se guarda en la plantilla, mientras el método de carga decide el borrado mediante las otras banderas y después inserta. El modo web debe ser explícito y validar combinaciones históricas antes de ejecutar.

### Verificación prevista y decisiones abiertas

La verificación debe cubrir cargas con plantillas y tablas históricas, archivos XLS y XLSX, primera y última fila de Excel, columnas vacías intermedias, claves con ceros iniciales, fechas, decimales, longitudes y compatibilidad del texto con la codificación de destino. También debe cubrir duplicados internos y existentes, comparación MySQL de claves, reemplazo fallido con recuperación de los datos anteriores, dos confirmaciones del mismo ID, pérdida de respuesta y cargas concurrentes. Para los consumidores compartidos, revisar la lectura de plantillas de SII y la resolución de índices del importador de carpetas, incluyendo valores diferidos y recuperación frente a una fuente actualizada.

La persistencia de la columna única y su correspondencia con el índice permanente quedan concretadas en el diseño técnico final de este documento. La implementación deberá acreditar las verificaciones descritas mediante compilación y pruebas de integración y capacidad.

### Perfil de usuario y conexión — decisión aprobada el 11 de noviembre de 2026

El usuario aprobó que el permiso del cargador se asigne en `remit_dest_interno_perfil_produccion`, relacionado con `remit_dest_interno`, mediante la columna `ADMINISTRAR_CARGAS_IMPORTACION`, siguiendo el patrón de las capacidades existentes, con valor inicial 0 y habilitación explícita. El servidor verifica la sesión, el usuario activo, su empresa y este permiso. El acceso al cargador se determina por su propia capacidad funcional. Las operaciones de administración de fuentes y proveedores conservan sus verificaciones específicas cuando el usuario las utilice.

El usuario aprobó utilizar la misma base DA que contiene las tablas históricas y los datos `imp_02_*`, abriendo una conexión propia para cada operación del cargador. El aislamiento de privilegios se realiza mediante una cuenta MySQL dedicada, configurada en el servidor y vinculada explícitamente al destino DA autorizado de la empresa. La selección de base y servidor procede del contexto autorizado y de configuración administrativa. La implementación concretará el aprovisionamiento y los privilegios sobre las tablas elegibles, la creación de tablas nuevas y la correspondencia de las conexiones históricas con ese destino.

Usar otra cuenta MySQL sobre la misma base permite acotar privilegios conservando el acceso a las tablas que ya consulta el proveedor de índices. El reemplazo de filas afecta a los mismos datos compartidos, por lo que ese aislamiento de cuenta debe acompañarse de la transacción y los controles de concurrencia descritos. Si se eligiera otra base de datos física, habría que registrar y resolver esa fuente en los proveedores actuales para que pudieran consultar los datos cargados allí.

El resolver actual obtiene las conexiones RA y DA de la configuración de módulos cargada en sesión. La capacidad nueva debe usar ese contexto empresarial como referencia al comprobar su destino. [Resolver de conexiones](../../../../Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb) · [Patrón actual de autorización](../../../../Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionContextAdapter.vb)

### Formatos de entrada — decisión aprobada el 11 de noviembre de 2026

La decisión vigente del usuario establece **XLS y XLSX desde la primera entrega**, con CSV aplazado para simplificar la configuración. Esta decisión sustituye la aprobación inicial que también incluía CSV. La selección del lector y sus dependencias debe acreditar ambos formatos Excel sobre el entorno .NET Framework 4.6.1 del proyecto. Los casos de verificación deben incluir archivos reales de cada formato, selección de hoja, encabezados, primera y última fila, celdas vacías intermedias y conservación de los valores de las claves. Ambos recorridos alimentan el mismo mapeo de plantilla y las mismas operaciones de crear, agregar y reemplazar datos.

### Unicidad e índice de consulta — decisión aprobada el 11 de noviembre de 2026

El usuario aprobó ambas medidas para el campo único, validación en cada carga y restricción permanente en la tabla. El índice UNIQUE sobre la clave proporciona la restricción y sirve para las búsquedas por igualdad del proveedor. La verificación de rendimiento debe revisar el plan de consulta con EXPLAIN sobre el servidor utilizado. [Índices y consultas en MySQL](https://dev.mysql.com/doc/refman/8.0/en/optimization-indexes.html)

- En tablas nuevas, incluir el índice único al crear la estructura. En tablas existentes, revisar si ya hay una clave o índice equivalente y comprobar los datos antes de crear el índice necesario.
- Revisar duplicados, valores NULL o vacíos, tipo, collation y longitud de clave. La adecuación de tablas existentes debe presentar sus incidencias para corrección y conservar los datos hasta resolverlas.
- Cubrir la clave completa y respetar los límites de índices del servidor MySQL 5.1 y del juego de caracteres del destino. Un índice de prefijo que cambie la identidad efectiva de los valores necesita revisión de compatibilidad.
- En fuentes exclusivas de una empresa, la unicidad corresponde a la clave configurada. En fuentes con aislamiento por columna empresarial, corresponde al par empresa y clave, alineado con el filtro del proveedor.
- Mantener la creación o adecuación del índice como fase estructural propia, por los commits implícitos del DDL. El reemplazo de filas conserva el índice y utiliza la transacción DML descrita.
- Verificar duplicados dentro del archivo, conflictos contra el destino en modo agregar y una carrera de inserciones sobre la misma clave, aplicando la política de rechazo completo aprobada y descrita a continuación.

### Tratamiento de duplicados — decisión aprobada el 11 de noviembre de 2026

El usuario aprobó rechazar por completo una carga con conflictos de clave, con validación previa y detalle de las filas afectadas. El comportamiento depende del modo elegido.

| Modo | Regla aprobada |
|---|---|
| **Crear tabla** | Exigir claves únicas dentro del archivo. Si se repite una clave, mostrar las filas involucradas y solicitar su corrección antes de cargar. |
| **Agregar registros** | Rechazar toda la carga cuando una clave se repita dentro del archivo o ya exista en el destino autorizado, aunque sus otros valores coincidan. |
| **Reemplazar datos** | Exigir claves únicas dentro del archivo. La coincidencia con las claves de los datos que serán sustituidos es válida. Confirmar el reemplazo del contenido autorizado mediante la transacción prevista. |

El diagnóstico identifica la fila, la clave y el origen del conflicto. Por ejemplo, puede indicar que la fila 27 contiene la clave `00123`, ya existente en el destino. Tras corregir el archivo, el operador vuelve a validarlo. Cuando su intención es sustituir el conjunto autorizado, utiliza el modo de reemplazo con su confirmación explícita.

Si una escritura concurrente introduce un conflicto después de validar el archivo, el índice UNIQUE provoca un error y la transacción revierte todas las escrituras de esa carga. La operación registra y presenta el conflicto. La omisión de filas y la actualización automática de registros por coincidencia de clave quedan fuera de los modos acordados para esta primera entrega.

### Volumen y tamaño configurables — decisión aprobada el 11 de noviembre de 2026

El usuario aprobó un máximo inicial de **10.000 filas de datos y 10 MB por archivo**, exigiendo que ambos límites sean configurables. Se administran como parámetros del servidor del módulo, con valores positivos y modificación independiente. Las filas corresponden a la hoja seleccionada y excluyen el encabezado. El tamaño corresponde al archivo Excel recibido, tanto XLS como XLSX. La unidad del tamaño debe quedar explícita en la configuración y en su presentación al usuario.

La interfaz muestra los límites vigentes y el servidor los valida al recibir y preparar cada carga, y comprueba su vigencia antes de escribir datos. La configuración del servidor determina los límites efectivos. Si un archivo excede alguno, la aplicación informa el valor permitido y el valor detectado antes de aplicar la carga.

Estos parámetros corresponden a cada carga. Una tabla puede acumular registros mediante cargas sucesivas dentro de los límites configurados. La configuración se coordina con los límites de solicitud de ASP.NET e IIS para permitir el envío del archivo y el contenido adicional de la petición.

La verificación de capacidad debe medir archivos XLS y XLSX con 1.000, 5.000 y 10.000 filas, tiempo de preparación y escritura, uso de memoria y comportamiento transaccional del reemplazo. Las pruebas funcionales incluyen los límites exactos, archivos que los exceden y cambios de configuración. Un aumento posterior se contrasta con mediciones del ambiente para acreditar su capacidad efectiva.

## Diseño técnico resuelto del 11 de noviembre de 2026

El usuario solicitó resolver los detalles pendientes como arquitecto. Las decisiones siguientes concretan el diseño sobre el alcance funcional aprobado. El lector, las extensiones de cabecera, la tabla de ejecución y las reglas técnicas se seleccionan en esta revisión. Su compilación, instalación y aceptación operativa corresponden a la implementación posterior.

### 1. Persistencia de la clave y compatibilidad de plantillas

La columna única se guarda en `imp01_plantillaimp`, ligada al destino de la plantilla. Se añaden dos columnas al final de la estructura histórica.

| Columna nueva | Tipo previsto | Uso |
|---|---|---|
| `CAMPO_UNICO` | `VARCHAR(64) CHARACTER SET ascii COLLATE ascii_bin NULL` | Nombre físico exacto de la columna clave. NULL identifica las plantillas históricas que aún necesitan completar esta configuración. |
| `REVISION_CARGA` | `INT UNSIGNED NOT NULL DEFAULT 1` | Detectar cambios concurrentes realizados por el nuevo editor. |

El campo único debe corresponder a un `CAMPO_DESTINO` de la plantilla y a una columna real del destino. Al preparar y confirmar se verifica que su índice UNIQUE cubra la clave completa. Para fuentes compartidas se verifica el índice compuesto de empresa y clave según la política vigente de la fuente.

El guardado de cabecera y campos se realiza en una transacción con revisión esperada. La preparación también conserva una huella de todos los campos históricos relevantes y del esquema del destino. Esto permite detectar una edición del importador antiguo que cambie datos de configuración sin incrementar `REVISION_CARGA`. En la confirmación se bloquea y relee la configuración por ID antes de escribir.

La asignación inicial de la clave incluye la adecuación estructural de su índice. Un cambio posterior de identidad de la tabla se trata como mantenimiento estructural con revisión de sus proveedores y consumidores. El formulario ordinario utiliza la clave configurada y comprueba su coherencia.

Para las plantillas históricas se conserva `RUTA_ARCHIVO_FUENTE` como metadato existente. En una plantilla creada desde web se guarda una referencia relativa generada, por ejemplo `web/<id-aleatorio>.xlsx`, dentro de la longitud 120. El módulo resuelve esa referencia en su almacenamiento privado. Cada ejecución conserva su propia referencia inmutable, independiente de la última carga de la plantilla. El nombre original se registra como dato descriptivo. Esta convención de altas web requiere verificar la visualización de esas plantillas en las herramientas antiguas.

`LIBRO_EXCEL` conserva el nombre de la hoja. Para las altas web, `CONEXION_FUENTE` registra `8.0 xls` o `12.0 xlsx` según el formato detectado. El modo se elige explícitamente para cada ejecución y queda registrado en ella. Al guardar una opción predeterminada de plantilla, agregar utiliza las banderas históricas 0/0/1 y reemplazar 0/1/0 para `ELIMINA_TABLA`/`ELIMINA_INSERTA_RECORD`/`INSERTA_RECORD`. Crear utiliza 0/0/1 y comprueba que el destino esté pendiente de creación.

### 2. Lector de XLS y XLSX

Se selecciona **ExcelDataReader 3.6.0 con su ensamblado net45**, compatible con el objetivo .NET Framework 4.6.1 del proyecto. Esa versión declara soporte para XLS y XLSX y ofrece lectura secuencial mediante `Read()` y selección de hojas mediante `NextResult()`. La selección queda fijada para evitar resolver accidentalmente un paquete con otro framework mínimo. [Proyecto oficial de la versión](https://raw.githubusercontent.com/ExcelDataReader/ExcelDataReader/v3.6.0/src/ExcelDataReader/ExcelDataReader.csproj) · [Documentación oficial](https://raw.githubusercontent.com/ExcelDataReader/ExcelDataReader/v3.6.0/README.md)

Se añade **ExcelNumberFormat 1.1.0**, que dispone de un ensamblado compatible con .NET Framework, para interpretar el formato visual de las celdas destinadas a texto, incluidos códigos numéricos presentados con ceros iniciales. El lector proporciona el formato de la celda y esta biblioteca lo aplica. [Paquete y frameworks](https://www.nuget.org/packages/ExcelNumberFormat/1.1.0)

La primera fila de la hoja seleccionada contiene los encabezados. Se recorre cada posición de columna, incluidas las celdas vacías intermedias. Las filas totalmente vacías se omiten, conservando los números físicos de fila en los diagnósticos. El límite de filas cuenta las filas de datos procesadas. La primera y la última fila de datos se incluyen.

Para destinos TEXTO se conserva el texto original y se aplica el formato Excel cuando la celda es numérica y su presentación aporta significado. ENTERO y DECIMAL se convierten desde el valor tipado y se contrastan con la capacidad real de la columna. DATE utiliza la fecha tipada o texto con formato `yyyy-MM-dd` o `dd/MM/yyyy`, con diagnóstico cuando la conversión falla. Una columna histórica VARCHAR que se llame FECHA se trata como texto.

Las reglas explícitas de coma, punto o segmento de `imp01_campos_plantilla` se aplican antes de validar el valor final. Se conserva el contenido que resulte de esas reglas. La validación de longitud, rango y charset reutiliza `PerfilImportacionCatalogAdapter.ValorCompatible`, completada con las verificaciones de nulabilidad y fecha del cargador. Los errores de precisión, codificación o valores de Excel se presentan por fila y columna.

### 3. Esquema operativo de ejecución

Se crea **una tabla `imp01_carga_ejecucion` en DA, con motor InnoDB**. Sus campos quedan agrupados así.

| Grupo | Campos previstos |
|---|---|
| Identidad | `id_carga CHAR(36)` PK, `id_plantilla INT UNSIGNED`, `ambito_ref VARCHAR(128)`, `usuario_ref VARCHAR(128)`. |
| Destino | `conexion_ref VARCHAR(64)`, `tabla_destino VARCHAR(64)`. |
| Archivo | `archivo_nombre VARCHAR(255)`, `archivo_ref VARCHAR(120)`, `archivo_bytes BIGINT UNSIGNED`, `archivo_hash BINARY(32)`. |
| Preparación | `modo VARCHAR(16)`, `revision_plantilla INT UNSIGNED`, `configuracion_doc LONGTEXT`, `configuracion_hash BINARY(32)`, `esquema_hash BINARY(32)`. |
| Resultado | `estado VARCHAR(32)`, `filas_previstas INT UNSIGNED`, `filas_confirmadas INT UNSIGNED`, `codigo_incidencia VARCHAR(64) NULL`. |
| Tiempo | `creado_utc DATETIME`, `actualizado_utc DATETIME`, `confirmado_utc DATETIME NULL`. |

Los identificadores y hashes usan almacenamiento ASCII o binario según corresponda. La configuración se serializa como JSON con escape ASCII dentro de LONGTEXT, compatible con MySQL 5.1. `id_plantilla` actúa como referencia lógica y el registro conserva el destino y la configuración aplicados. Se añaden índices por ámbito/usuario/fecha y por plantilla/fecha para consultar operaciones de forma paginada.

El registro comienza como `RECIBIDA`, pasa a `PREPARADA` al finalizar la validación y a `CONFIRMADA` dentro de la misma transacción que escribe los datos. Una incidencia conocida y una transacción revertida permiten registrar `FALLIDA`. Cuando el resultado del commit sea incierto, se conserva el ID y se consulta el registro antes de decidir una repetición. Una confirmación repetida del mismo ID y contenido devuelve el resultado persistido.

### 4. Conexión y privilegios concretos

La autorización funcional se comprueba en RA mediante el permiso aprobado del usuario y su empresa. El escritor usa la cuenta dedicada de DA. La configuración del servidor relaciona explícitamente el ámbito, el destino DA y el ID histórico de `imp01_xmlconexion`. Al abrir una operación se comprueba que la referencia histórica y la conexión dedicada correspondan al destino permitido. Las credenciales se mantienen en configuración protegida del servidor.

Para permitir altas de tablas desde el propio módulo, la cuenta dedicada recibe en **esa base DA** los privilegios `SELECT`, `INSERT`, `UPDATE`, `DELETE`, `CREATE`, `ALTER`, `INDEX` y `CREATE TEMPORARY TABLES`. La configuración de cuenta se restringe al host de la aplicación. Estos privilegios permiten operar sobre las tablas actuales y futuras de esa base. La aplicación limita cada operación a sus tablas de configuración y a los destinos de plantillas elegibles bajo `imp_02_`.

Esta elección requiere explicitar el alcance de aislamiento. MySQL 5.1 concede privilegios sobre una base o sobre nombres exactos de tabla. La cuenta dedicada y el control de prefijo de la aplicación aportan aislamiento de uso, mientras la concesión sobre DA abarca técnicamente sus tablas. Un límite físico por tabla exacta implicaría aprovisionar concesiones cuando aparezcan nuevos destinos. La solución elegida para mantener la creación desde el módulo utiliza la concesión sobre DA y validación de destinos en el servidor. [Privilegios en MySQL 5.1](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/sql-syntax.html#grant)

Los archivos SQL de instalación agregan `ADMINISTRAR_CARGAS_IMPORTACION TINYINT UNSIGNED NOT NULL DEFAULT 0` a `remit_dest_interno_perfil_produccion`, las dos columnas de cabecera y la tabla operativa. Los archivos de concesiones y su guía se entregan para ejecución manual del usuario. Después de su confirmación, la revisión mediante SELECT comprueba las estructuras y los permisos observables y registra las diferencias.

### 5. Clave física y preparación de tablas existentes

La clave seleccionada es requerida en la carga. Su adecuación estructural utiliza NOT NULL y un índice UNIQUE de columna completa. En fuentes compartidas se aplica al ámbito empresarial y la clave. MySQL permite varios NULL en un índice único cuando la columna admite NULL, razón por la que se comprueba también la nulabilidad. [Semántica de UNIQUE en MySQL 5.1](https://docs.oracle.com/cd/E19078-01/mysql/mysql-refman-5.1/sql-syntax.html#create-index)

Antes de habilitar una tabla existente se revisan índices equivalentes, claves vacías o NULL y duplicados según su tipo y collation. Los problemas encontrados se presentan para corrección explícita. La configuración de la clave y el índice quedan vinculados cuando la estructura resulta válida. La selección de una tabla histórica con datos inconsistentes produce un diagnóstico de adecuación estructural.

La creación y el ajuste de índices son una fase DDL propia. El reemplazo posterior conserva la estructura y sus índices. Para tablas nuevas se utiliza InnoDB, el charset/collation autorizado de DA, columnas VARCHAR(250), INT, DECIMAL(18,2) o DATE según los tipos elegidos, y se comprueba que la clave completa quepa en los límites del motor. Una tabla existente mantiene sus tipos reales. Se puede conservar su PK técnica cuando exista. La identidad de negocio queda determinada por el índice único configurado.

La comprobación previa de duplicados usa comparación MySQL con el mismo tipo y collation del destino, mediante una tabla temporal de validación de la conexión. Esto evita que una comparación de cadenas en JavaScript o .NET discrepe de la igualdad efectiva del índice.

### 6. Transacción, concurrencia y creación recuperable

La confirmación comprueba primero la autoridad vigente y la identidad de la petición. Si la ejecución ya está CONFIRMADA y coincide con su contenido registrado, devuelve el resultado histórico persistido. Para una ejecución pendiente adquiere un bloqueo cooperativo asociado a la base y tabla de destino, bloquea y relee el registro de ejecución y comprueba plantilla, esquema, límites y autoridad antes de escribir. La relectura reconoce una confirmación concurrente ya terminada. La conexión dedicada aplica modo SQL estricto de sesión. Los INSERT utilizan valores parametrizados y agrupaciones acotadas dentro de una única transacción, para controlar errores de tipo y tamaño de comandos.

Agregar compara de nuevo las claves contra el destino e inserta. Reemplazar ejecuta DELETE e INSERT dentro de la misma transacción y con el ámbito autorizado cuando corresponda. El registro pasa a CONFIRMADA junto con los datos. Un fallo revierte la operación completa. La restricción única protege frente a otro escritor que inserte la misma clave durante la confirmación.

Crear comprueba el nombre y ejecuta la fase estructural. Para recuperar una respuesta perdida del CREATE, la tabla nueva incorpora en su comentario el ID de carga. Una repetición verifica ese identificador y la huella de estructura antes de continuar. Si ya existe una tabla de otra procedencia, el modo crear devuelve conflicto de destino. Una carga fallida conserva el resultado estructural identificado y permite consultar su estado. Las tablas nuevas se incorporan a fuentes/proveedores mediante los servicios administrativos existentes después de confirmar sus datos.

### 7. Componentes previstos y configuración

| Componente nuevo | Responsabilidad |
|---|---|
| Página o diálogo de carga Excel en Gestión | Elegir plantilla, archivo, hoja, modo, revisar incidencias y confirmar. |
| Receptor ASHX | Recibir el archivo mediante multipart, comprobar sesión/permiso y asociarlo a su ID de carga. |
| Servicio ASMX | Listar y guardar plantillas, preparar, confirmar y consultar el resultado. |
| Servicio de carga | Coordinar lectura, reglas, autoridad, límites y estados de ejecución. |
| Repositorio MySQL | Guardar plantillas, inspeccionar estructura, validar claves y ejecutar la transacción. |
| Lector Excel | Adaptar XLS y XLSX a las mismas filas tipadas y numeradas. |

Se reutilizan los patrones existentes de sesión, respuestas saneadas y validación de solicitudes del repositorio. El receptor y las operaciones tienen su propio propósito de autorización. La administración de fuentes y proveedores utiliza sus permisos vigentes al invocarse esos servicios.

El acceso de menú debe funcionar cuando el usuario tenga exclusivamente `ADMINISTRAR_CARGAS_IMPORTACION`. Actualmente el menú se construye dentro de `PerfilImportacionEditor.js` y su host depende de la autorización de perfiles. La integración debe construir o reutilizar el menú desde la navegación común y añadir el acceso del cargador según su propio permiso. El patrón de `Class_remit_dest_interno_perfil_produccion` permite exponer un indicador de sesión para presentar el acceso. El servicio comprueba el permiso vigente en RA en cada petición, con usuario activo y empresa, mediante un contexto propio del cargador. [Asignación de permisos de sesión](../../../../Gestion/Class_remit_dest_interno_perfil_produccion.vb) · [Menú actual](../../../../js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js)

Los parámetros iniciales quedan como `MaxFilasDatos = 10000` y `MaxArchivoMB = 10`, independientes y configurables en el servidor. Para este módulo, 1 MB se define como 1.000.000 bytes. La interfaz muestra los valores efectivos. La recepción cuenta los bytes del archivo y el lector cuenta las filas de datos de la hoja elegida. La confirmación comprueba otra vez los límites vigentes. Una preparación que dejó de cumplirlos informa la incidencia antes de escribir.

### 8. Comprobaciones necesarias durante la implementación

La verificación de esta revisión abarca código, metadatos y frameworks declarados por las dependencias seleccionadas. La implementación debe acreditar la compilación y ejecución del lector en el proyecto real y los siguientes recorridos.

1. Leer XLS y XLSX con varias hojas, encabezados, primera/última fila, celdas vacías intermedias, fechas y códigos formateados con ceros.
2. Usar una plantilla histórica y crear una plantilla web, comprobando sus campos y los consumidores identificados de SII.
3. Crear un destino, agregar filas y reemplazar datos. Probar las reglas de duplicados y la comparación real de collation.
4. Introducir un fallo controlado de escritura para comprobar que el reemplazo conserva los datos anteriores al revertir la transacción.
5. Repetir una confirmación y perder su respuesta, verificando que el mismo ID devuelve las filas ya confirmadas. Revisar también la recuperación de la fase CREATE.
6. Confirmar simultáneamente cargas y comprobar el rechazo de una clave concurrente por el índice único.
7. Probar los valores exactos de filas/bytes, el exceso de cada límite y cambios de configuración. Medir 1.000, 5.000 y 10.000 filas para ambos formatos y revisar el plan de consulta de la clave con EXPLAIN.

Los casos reales de escritura se ejecutarán con el ambiente y las cuentas de prueba correspondientes, conservando las reglas del repositorio para credenciales y evidencia. Los resultados distinguirán las comprobaciones estáticas de las pruebas efectivamente realizadas.

## Secuencia de prompts para implementar

Los siete prompts se ejecutan en orden sobre el repositorio local y toman este documento como alcance. Cada entrega incorpora la verificación focal correspondiente a sus cambios y registra los resultados en el flujo del cambio. Los cambios SQL se entregan como archivos para ejecución manual del usuario y se revisan mediante SELECT después de su confirmación. Las pruebas funcionales de carga o E2E utilizan el ambiente y las cuentas autorizados y requieren la instalación manual previamente verificada.

Los [archivos de los siete prompts y sus informes de revisión](README.md) están disponibles en esta misma carpeta de exploración.

| Orden | Prompt de trabajo | Descripción y resultado esperado |
|---|---|---|
| **01** | **Formalizar el cambio y preparar esquema y contratos.** Crea el cambio OpenSpec correspondiente a esta exploración, con especificaciones y tareas ejecutables. Prepara el SQL del permiso, las dos columnas de cabecera, la tabla de ejecuciones y el procedimiento de adecuación del índice único. Define las solicitudes, respuestas, estados y parámetros de configuración. | Entrega el contrato del módulo, SQL revisable compatible con MySQL 5.1, diagnóstico de tablas existentes y guía de instalación de la cuenta dedicada y sus privilegios. Identifica las estructuras ya presentes antes de preparar cada cambio. |
| **02** | **Implementar autorización, conexión y administración de plantillas.** Comprueba el permiso de producción por usuario y empresa. Resuelve la cuenta dedicada sobre DA y su correspondencia con la conexión histórica. Implementa listado, consulta y guardado transaccional de cabecera y campos, con clave única, revisión y huella de configuración. | Entrega los servicios y repositorios de plantillas, autorización vigente en cada petición y detección de cambios concurrentes. Acredita lectura de plantillas históricas y compatibilidad con los consumidores identificados. |
| **03** | **Implementar lectura y validación de Excel.** Integra ExcelDataReader 3.6.0 net45 y ExcelNumberFormat 1.1.0. Lee XLS y XLSX por hoja, aplica los mapeos y transformaciones y valida tipos, longitudes, charset y límites configurados. Conserva posiciones de columnas y números de fila. | Entrega un lector común y diagnósticos por fila y columna. Acredita ambos formatos, primera y última fila, celdas vacías, fechas, decimales y códigos con ceros iniciales. |
| **04** | **Implementar recepción y preparación de cargas.** Recibe el archivo en almacenamiento privado, asigna ID y hash y registra su ejecución. Prepara la vista previa, valida estructura e índice y detecta duplicados con la comparación MySQL del destino. Aplica las reglas de crear, agregar y reemplazar y registra la configuración y las huellas preparadas. | Entrega las operaciones de recepción y preparación hasta el estado PREPARADA, con incidencias explicadas y comprobación completa antes de confirmar. Las pruebas focales cubren límites, duplicados y modificaciones posteriores a la preparación. |
| **05** | **Implementar confirmación transaccional y recuperación.** Revalida permiso, plantilla, archivo, estructura y límites. Implementa agregar y reemplazar con datos y resultado en una transacción, y crear con fase estructural identificable. Serializa las cargas sobre el mismo destino y resuelve confirmaciones repetidas por ID. | Entrega los tres modos, consulta de resultado, rollback, detección de conflictos concurrentes y recuperación de una respuesta perdida, incluidos los efectos de CREATE. |
| **06** | **Implementar la interfaz e integrar el menú.** Añade Cargar datos desde Excel al menú Importación según el permiso propio del cargador. Construye el recorrido de plantilla, archivo, hoja, modo, vista previa, incidencias, confirmación y resultado. Presenta límites vigentes y permite consultar el ID cuando una respuesta sea incierta. | Entrega el recorrido completo desde la interfaz habitual, usable con teclado y con un usuario que tenga exclusivamente el permiso del cargador. Reutiliza el menú y los servicios implementados en los pasos anteriores. |
| **07** | **Verificar integración, regresión y capacidad y cerrar la entrega.** Compila el proyecto y ejecuta la verificación integral en el ambiente autorizado. Cubre permisos, plantillas históricas, índice único, tres modos, recuperación, concurrencia y límites. Mide archivos de 1.000, 5.000 y 10.000 filas en XLS y XLSX y revisa EXPLAIN de la consulta de clave. | Entrega matriz de aceptación con SHA y ambiente, resultados efectivos, revisión de los consumidores de SII e importación de carpetas y guía operativa y de despliegue. Resuelve las brechas encontradas antes del cierre del cambio. |
