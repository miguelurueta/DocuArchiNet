# Validación de base de datos y definición propuesta del perfil

Fecha: 2026-10-19.

## 1. Inspección realizada

El usuario autorizó continuar la exploración utilizando la conexión existente del repositorio. Se utilizó la conexión ODBC configurada en `Web.config`, sin copiar, imprimir ni guardar sus credenciales o cadena de conexión.

- Esquema predeterminado confirmado: `docuarchi`.
- Versión reportada por el servidor: `5.1.37-community`.
- Se inspeccionaron 37 tablas candidatas, 430 columnas y 56 referencias de clave foránea.
- Las 37 tablas inspeccionadas reportan motor InnoDB.
- La consulta de triggers no devolvió filas para esas tablas bajo la visibilidad de la cuenta utilizada; esto no certifica ausencia fuera de ese alcance o sin permisos suficientes.
- Únicamente se ejecutaron SELECT de metadatos en `information_schema`, `DATABASE()` y `VERSION()`.
- No se consultaron filas de expedientes, archivos, usuarios, perfiles operativos ni credenciales de servicios. No se modificaron esquema, registros ni SII.

Una consulta inicial enumeró la presencia de las tablas candidatas en los esquemas visibles. La inspección detallada y las conclusiones siguientes se limitaron al esquema predeterminado `docuarchi`; no deben trasladarse automáticamente a otras bases o ambientes.

La evidencia estructurada está en [02-evidencia-esquema-perfil.json](02-evidencia-esquema-perfil.json). Contiene nombres y tipos de columnas, índices y relaciones; no contiene valores de registros ni definiciones de conexión. Los nombres de columnas de credenciales son metadatos, no sus valores.

## 2. Hallazgos confirmados y precisión del diagnóstico anterior

### A. Autorregistro no constituye un perfil de importación completo

`ra_auto_registro_expediente` tiene tres columnas: identificador, nombre y función de obtención de datos. No contiene versión de perfil ni reglas de extracción de nombres.

`ra_auto_campos_gestion_expediente` tiene identificador, referencia a autorregistro y cinco campos de clasificación: `id_fondo`, `id_instrumento`, `d_area`, `id_serie`, `id_sub_serie`.

La referencia al autorregistro está indexada pero no es única en esta tabla. Por tanto, la estructura permite varias filas para un mismo autorregistro aunque el lector revisado tome la primera. No se comprobaron duplicados reales porque no se consultaron registros.

`ra_auto_campo_unico_expediente` contiene campo, obligatoriedad e identidad; tampoco define separadores, posiciones o conversiones por perfil.

En `expediente_archivo` existe una clave foránea real al autorregistro. El código previamente revisado bloquea la edición en dos rutas cuando ese identificador es distinto de cero. La trazabilidad del nuevo perfil debe mantenerse separada; no se asignará un autorregistro por conveniencia técnica.

### B. La configuración de servicio corresponde a conexión, no a clasificación

Se confirmó la existencia de `ra_ser_config_servicio`. Sus campos son identificador, referencia al servicio, URL, usuario y contraseña del servicio. No se leyeron sus valores.

La clase vacía del repositorio no permitía establecerlo antes. Ahora se descarta esta tabla como base del perfil documental: su responsabilidad es diferente.

### C. Existen equivalencias de tipologías con nombres distintos a los inferidos por las clases

En este esquema las tablas reales son:

- `ra_ser_tipos_documento_serviciointegracion`: servicio, nombre de tipología externa, código externo y alias.
- `ra_ser_tipo_doc_series_rel_ra_ser_tipos_serviciointegracion`: relación entre tipo externo y `tipo_doc_series`.

La primera tiene un índice único sobre `NombreTipologiaExterna`, sin incluir servicio ni versión de perfil. Esto puede impedir nombres iguales en configuraciones independientes. La relación tiene unicidad por pareja de tipos, pero no representa reglas por versión de perfil.

Pueden servir como referencia o fuente de equivalencias ya autorizadas cuando un caso concreto lo requiera. No se insertarán nuevas reglas del perfil ni se modificarán sus índices para adaptarlas a carpetas; SII permanece aislado.

### D. Catálogos y esquema del gabinete son reutilizables

Se verificaron `system1`, `detalle_gabienete`, clasificación, tipologías, tipos de expediente, niveles y permisos. `detalle_gabienete.id_gabinete` tiene clave foránea hacia `system1.id_gabinete`.

Las reglas nuevas pueden referenciar un campo índice real y comprobar su pertenencia al gabinete seleccionado. No hace falta duplicar la definición de los gabinetes ni sus campos.

La presencia de estas tablas en un mismo esquema permite estudiar referencias físicas para ese despliegue, pero no obliga a asumir la misma ubicación en todos los módulos o ambientes.

### E. Los límites físicos y funcionales difieren

- `expediente_archivo.CODIGO_UNICO`: `varchar(70)` en la base inspeccionada.
- La función actual de creación revisada rechaza códigos superiores a 45 caracteres.
- `ALEAS_EXPEDIENTE`: `varchar(120)`, coincidente con el límite del código revisado.
- La clave primaria de `expediente_archivo` es compuesta: `ID_EXPEDIENTE`, `CODIGO_UNICO`.

La validación del perfil debe cumplir tanto el esquema como la operación reutilizada; 70 caracteres físicos no autorizan a enviar 70 a una función que admite 45. La clave compuesta tampoco demuestra por sí sola unicidad global del nombre o código sin considerar los demás índices y reglas.

### F. Los perfiles administrativos son otro concepto

Se localizaron `adm_perfil`, `adm_perfil_permiso` y `adm_usuario_perfil`. Su estructura relaciona usuarios y permisos, por lo que no reemplaza el perfil documental.

Podrían participar en autorización tras revisar su integración efectiva con la aplicación; la existencia del esquema no demuestra que ya controle la nueva capacidad. No se crearon permisos ni se cambiaron asignaciones.

## 3. Decisión recomendada

Mantener tres entidades nuevas para configuración:

1. Perfil: identidad estable y disponibilidad.
2. Versión: datos documentales y políticas publicadas.
3. Regla: asignaciones configurables por versión.

Reutilizar los catálogos existentes mediante identificadores. Una copia de valores seleccionados en la versión del perfil representa su configuración efectiva, no un nuevo catálogo editable de fondos, series o gabinetes.

No utilizar tablas operativas de SII. Su revisión anterior fue sobre scripts del repositorio; esta inspección no certifica el esquema Workflow de otro catálogo.

La versión de servidor detectada obliga a validar el DDL y las consultas contra ese entorno antes de implementarlos. Se prioriza un modelo relacional con columnas explícitas; no se presupone disponibilidad de características SQL de versiones más recientes. No se propone actualizar el motor ni modificar SII como parte de este trabajo.

## 4. Contenido propuesto del perfil

Esta es una propuesta para revisión funcional; no se ha publicado un perfil ni aprobado un DDL.

| Grupo | Dato | Regla de publicación propuesta |
|---|---|---|
| Identificación | Código estable, nombre y ámbito | Obligatorios; nombre comprensible y código único dentro del ámbito definido. |
| Identificación | Descripción | Recomendada para explicar el uso. |
| Estado | Borrador/publicado/inactivo y versión | Un borrador puede estar incompleto; solo una versión validada puede iniciar lotes. |
| Clasificación | Fondo, instrumento, área, serie y subserie | Resolver los valores exigidos por el proceso y validar su coherencia. Subserie solo cuando corresponda; no fabricar una para completar un formulario. |
| Expediente | Tipo de expediente y tipo de unidad documental | Deben quedar resueltos antes de crear; seleccionar catálogos existentes. |
| Expediente | Nivel de Producción | Precisión funcional posterior: recibir el nivel seleccionado en el árbol existente al abrir la importación, validar compatibilidad con el perfil y conservarlo explícitamente; no reseleccionarlo dentro de la ventana ni tomar una selección mutable de sesión. |
| Expediente | Responsable, fecha inicial y otros campos | Deben tener una fuente definida si las reglas actuales los requieren. No inventar fechas ni responsables por omisión. |
| Destino | Gabinete | Obligatorio y compatible con clasificación, usuario y expediente. |
| Carpetas | Estrategia de agrupación | Alcance posterior: una raíz seleccionada por expediente bajo el nivel de apertura. Publicar solo opciones de subcarpetas soportadas y explícitas. |
| Nombre | Regla de nombre visible y política de código | Distinguir nombre visible de identidad; validar límites físicos y funcionales. |
| Índices | Asignaciones | Una fuente final por campo requerido; reglas variables solo cuando sean necesarias. |
| Tipología | Fuente o regla de resolución | Debe resolver un tipo documental válido por documento; no exige que aparezca en el nombre. |
| Coincidencias | Política entre lotes distintos | Propuesta inicial: reportar conflicto y exigir decisión explícita; sin fusión automática por nombre. |
| Límites | Extensiones/tamaño | Respetar configuración general aplicable; el perfil puede ser más restrictivo. |

Los identificadores y longitudes definitivos se fijarán al diseñar el contrato y el DDL. No se mantiene una referencia editable a clasificación SII: si se utiliza como plantilla de lectura, se conserva procedencia y se publican los valores efectivos en la nueva versión.

## 5. Reglas de publicación y uso

- Crear y guardar borradores no debe requerir tener todos los campos completos.
- Publicar exige configuración coherente, destinos existentes y fuentes resueltas para los campos obligatorios.
- Seleccionar una versión no crea expedientes ni transfiere archivos.
- La revisión previa valida los valores particulares de las carpetas/archivos contra esa versión.
- Un lote confirmado conserva versión y valores efectivos; no cambia si se modifica posteriormente el perfil.
- Si cambia el esquema de un gabinete o la autorización, la ejecución debe volver a validar esas condiciones externas antes de producir efectos.
- El mismo lote se recupera por identidad persistida; no se considera una nueva importación coincidente.
- Ninguna regla de nombres fija guion, tres posiciones o campos del ejemplo de órdenes de compra.

## 6. Pendientes que la inspección de esquema no resuelve

- Catálogos y combinaciones concretos que utilizará cada organización: no se consultaron datos de negocio.
- Política de carpetas y subcarpetas, vacías y archivos sueltos.
- Editabilidad posterior del expediente y decisión de código único.
- Posible evolución a servidor/red fuera del alcance inicial desde dispositivo.
- Permisos efectivos de publicación/selección y su conexión con el modelo administrativo existente.
- Volumen objetivo y mecanismo de ejecución/recuperación.

Actualización de alcance: el origen inicial es dispositivo, la raíz seleccionada representa un expediente y el nivel procede del árbol existente. Esos puntos ya no son decisiones abiertas. Continúan pendientes las opciones concretas de subcarpetas, identidad/fechas/edición y parámetros operativos; el procesamiento desatendido es opcional. Ambos ámbitos de integridad completa (expediente/lote) están aprobados como configurables en el perfil. Ver diagnóstico 06 para riesgos de integración que los metadatos de base no resuelven.

La reutilización estructural ya tiene evidencia de base de datos; estas decisiones siguen siendo funcionales y no se pueden deducir únicamente de columnas y claves.
