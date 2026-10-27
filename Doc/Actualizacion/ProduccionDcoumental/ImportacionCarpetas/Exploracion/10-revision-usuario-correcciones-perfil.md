# Revisión del usuario — Correcciones del perfil de importación

Fecha de apertura: 2026-10-20.

Bitácora acumulativa de las observaciones del usuario durante la revisión de DOC-110. Cada observación se registra con su decisión, alcance, evidencias y estado. Este documento no acredita ejecución de SQL, cambios de código ni aprobación de pruebas por el solo hecho de describirlos.

El usuario continuará revisando la funcionalidad por partes. Se agregarán las correcciones a este mismo archivo, sin convertir explicaciones o propuestas del agente en decisiones aprobadas.

## Registro de correcciones

| ID | Observación del usuario | Corrección requerida | Estado |
|---|---|---|---|
| COR-01 | `perfil_id` se genera como GUID; debe ser incremental. | Cambiar la identidad interna del perfil a `INT UNSIGNED AUTO_INCREMENT`, adecuando referencias, generación, validaciones y pruebas. | Registrada; pendiente de implementación y migración manual. |
| COR-02 | Las claves de esta implementación deben ser incrementales normales de la base, no GUID. | Extender la corrección a `version_id` y `regla_id`, con sus referencias y representaciones persistidas. | Registrada tras inspección de código; pendiente de migración e implementación. |

## COR-01 — Identificador incremental del perfil

### Situación encontrada

- `Sql/01-perfil-importacion-mysql51.sql` define `ra_integracion_perfil.perfil_id` como `CHAR(32)` ASCII y clave primaria.
- `ra_integracion_perfil_version.perfil_id` utiliza el mismo tipo y una FK al perfil.
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb` genera el identificador mediante `Guid.NewGuid().ToString("N")`.
- El servicio valida el identificador del perfil con la misma regla de GUID utilizada para versiones.
- El diseño inicial documentó GUID; el usuario corrige esa decisión en esta revisión. No se identificó una necesidad actual de generación distribuida de perfiles que exija ese formato.
- Existen perfiles y versiones persistidos por las pruebas E2E. No deben perderse sus relaciones al cambiar el identificador.

### Decisión requerida por el usuario

La clave interna `perfil_id` debe ser un entero autoincremental generado por MySQL. Definición técnica propuesta para materializar esa corrección:

```sql
perfil_id INT UNSIGNED NOT NULL AUTO_INCREMENT
```

El código funcional del perfil permanece separado del identificador interno y mantiene su unicidad dentro de la empresa. El cambio de tipo no modifica el ámbito empresarial, los permisos ni las políticas documentales.

### Aclaración sobre ordenamiento

El autoincremento asigna valores crecientes a la identidad; no garantiza una secuencia sin saltos ni el orden de salida de una consulta. En InnoDB, la clave primaria organiza el índice agrupado, pero las consultas requieren `ORDER BY` para garantizar su orden de presentación.

El listado actual usa `ORDER BY codigo,perfil_id`. Cambiar el identificador no cambia ese criterio. El orden visual deseado queda pendiente de precisar durante la revisión; no se considera aprobado cambiarlo a orden por ID. Tampoco se propone un consecutivo independiente por empresa ni utilizar este ID como numeración de expedientes.

### Impacto de implementación

| Área | Trabajo necesario |
|---|---|
| Tabla de perfiles | Clave primaria entera autoincremental e índices coherentes. |
| Tabla de versiones | Referencia `perfil_id` del mismo tipo; preservar FK, índice de versiones y pertenencia al perfil. |
| Migración de datos | Establecer correspondencia entre GUID anterior e ID nuevo y trasladar todas las referencias verificadas. No convertir directamente el texto GUID a entero ni eliminar datos por considerarlos de prueba. |
| Repositorio | Omitir el ID en la inserción, recuperar el generado en la misma conexión/transacción y usarlo para crear la primera versión. No usar `MAX(id)+1`. |
| Servicio, modelos y DTO | Aplicar identificadores numéricos a perfil, versión y regla, conforme a COR-02; revisar consumidores, parámetros y referencias opcionales. |
| JavaScript | Revisar transporte y comparación del ID para evitar conversiones o contratos inconsistentes. |
| Verificación de esquema | Comprobar el tipo nuevo, autoincremento, índices y FK conservadas. |
| Pruebas | Repetir creación, recarga, revisión obsoleta, publicación, versionado, historial, disponibilidad y autorización por empresa. |
| Documentación y prompts | Actualizar diccionario, contratos y referencias a identidad GUID del perfil. |

### Entrega SQL y límites

- Entregar un nuevo archivo de migración compatible con MySQL 5.1 para aplicación manual por el usuario, separado del script inicial ya aplicado.
- Actualizar también el script de instalación para bases nuevas; no indicar al usuario que reejecute el CREATE inicial sobre tablas existentes.
- Revisar dependencias y preparar una migración que preserve los datos. El DDL de MySQL 5.1 no debe presentarse como reversible mediante un simple rollback transaccional.
- Alcance inicial: `perfil_id`. La instrucción posterior del usuario amplía el alcance a `version_id` y `regla_id`; véase COR-02. Queda superada la propuesta inicial de conservar GUID en esas claves.
- Mantener SII sin modificaciones.

### Estado al registrar

Solo documentación. No se cambió el código, el SQL existente ni la base por esta corrección. La migración todavía no está preparada ni ejecutada. Las pruebas aprobadas hasta ahora corresponden al diseño anterior con GUID y no validan el futuro cambio a autoincremento.

## Próximas observaciones

Agregar COR-03 y siguientes conforme el usuario avance. Cada entrada debe identificar el comportamiento observado, la corrección solicitada, las decisiones pendientes, los archivos afectados y la evidencia de implementación cuando exista.

## COR-02 — Inventario de identidades GUID a sustituir

Instrucción del usuario: «deben incrementales normales de la base de datos». Inspección de la implementación DOC-110 y de sus contratos comunes; no se modifican las claves legacy ni SII. Evidencias de línea correspondientes al estado inspeccionado, susceptibles de moverse con cambios posteriores.

### Seis columnas afectadas en las tres tablas implementadas

| Tabla | Columna | Función actual | Corrección |
|---|---|---|---|
| `ra_integracion_perfil` | `perfil_id` | PK `CHAR(32)`; GUID generado por aplicación. | Entero `AUTO_INCREMENT`, PK. |
| `ra_integracion_perfil` | `version_publicada_id` | Referencia nullable a la versión publicada; validación lógica, sin FK física. | Entero nullable del mismo tipo que `version_id`; no autoincremental. |
| `ra_integracion_perfil_version` | `version_id` | PK `CHAR(32)`; GUID generado por aplicación. | Entero `AUTO_INCREMENT`, PK. |
| `ra_integracion_perfil_version` | `perfil_id` | FK física al perfil. | Entero del mismo tipo que la PK del perfil; no autoincremental. |
| `ra_integracion_perfil_regla` | `regla_id` | PK `CHAR(32)`; GUID generado por aplicación. | Entero `AUTO_INCREMENT`, PK. |
| `ra_integracion_perfil_regla` | `version_id` | FK física a la versión. | Entero del mismo tipo que la PK de versión; no autoincremental. |

Propuesta de tipo para las tres identidades: `INT UNSIGNED`. Cada tabla genera su propia secuencia. Solo las tres PK llevan AUTO_INCREMENT; las referencias reciben el ID generado en su tabla padre. `numero` de versión, `revision` y `orden` de regla ya son enteros y tienen funciones diferentes: no se convierten en identidades autoincrementales.

### Puntos concretos de generación y dependencia

Rutas relativas a la raíz del repositorio:

| Archivo / ubicación | Hallazgo e impacto |
|---|---|
| `Sql/01-perfil-importacion-mysql51.sql`, bajo la raíz funcional, líneas 2, 8, 20, 21, 48, 49 | Seis definiciones `CHAR(32)`; modificar instalación nueva y entregar migración separada para la base existente. |
| `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlPerfilImportacionRepository.vb:178–179` | Genera GUID de perfil y primera versión antes de insertar. Obtener ambos IDs de sus inserciones respectivas dentro de la transacción. |
| Mismo repositorio, línea 235 | Nueva versión recibe otro GUID. Cambiar generación sin alterar `numero` ni el control de concurrencia. |
| Mismo repositorio, líneas 311–312 | Cada regla recibe GUID antes de insertarse. Revisar recuperación del ID generado y su serialización. |
| Mismo repositorio, líneas 84–96 | Validador de esquema exige `char`, longitud 32 y collation ASCII para esas claves/referencias. Ajustar tipos y comprobar autoincremento de PK. |
| Mismo repositorio, líneas 276–328 | Selección de versión, lectura de reglas, parámetros y materialización de modelos dependen de IDs textuales. Revisar en conjunto. |
| `Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionModels.vb:13–26,71` | `PerfilId`, `VersionId`, `VersionPublicadaId`, `ReglaId` declarados String. Revisar tipo y nulabilidad de cada propiedad. |
| `Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionInterfaces.vb` | Firmas del repositorio reciben identificadores String; ajustar junto a implementación y dobles de pruebas. |
| `DTOs/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionDtos.vb` | `perfilId`, `versionId`, `versionOrigenId` transportados como String. Definir contrato numérico consistente con cliente y codec cerrado. |
| `Services/ProduccionDocumental/ImportacionCarpetas/ServicioPerfilImportacion.vb:125` | `ExigirId` exige exactamente 32 caracteres hexadecimales; reemplazar por validación del identificador incremental. |
| `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js:289–305` | Conserva ReglaId y transmite identificadores de perfil/versión. Revisar comparación, nulabilidad y payload sin aplicar cambios a referencias legacy indiscriminadamente. |
| `tests/PerfilImportacionServiceBehaviorTests.cs:28–29` | Fixtures fabrican identidades de 32 caracteres. Sustituir y probar entradas numéricas inválidas, inexistentes y ajenas. |
| `tools/e2e/tests/support/doc110-*-lifecycle.cjs` y suite completa | Reciben IDs del servidor; repetir las corridas después de la migración. No asumir que las corridas GUID validan el nuevo contrato. |

### Hallazgo de migración: JSON y huella de publicación

`InsertarReglas` serializa la regla completa en `regla_doc`, incluyendo `ReglaId`. Al leer, el repositorio sustituye ese dato con el ID de la columna. Por tanto, cambiar solo la PK dejaría una identidad antigua dentro del JSON y otra en memoria.

`PerfilImportacionJson.Huella` calcula SHA-256 sobre configuración y reglas completas ordenadas. Las reglas incluyen ReglaId: renumerarlas altera la entrada de la huella, aunque sus valores documentales permanezcan iguales. La migración debe definir y verificar explícitamente el tratamiento de `regla_doc` y `config_hash`, preservando evidencia de correspondencias y coherencia de las versiones publicadas. No recalcular ni ignorar huellas silenciosamente ni manipular JSON con reemplazos globales sin validar su estructura. MySQL 5.1 no dispone de las funciones JSON de versiones modernas.

Al guardar un borrador, el repositorio elimina y reinserta sus reglas. Con AUTO_INCREMENT consumirá IDs nuevos por guardado, sin continuidad garantizada. Ese comportamiento debe documentarse y probarse; no usar el ID de regla como posición o consecutivo funcional.

### Valores que no son PK incrementales

- `webservice/WebServiceImportacionCarpetas.asmx.vb:62`: genera GUID para `operacionId`, correlación de una respuesta HTTP; no corresponde a una fila ni FK de estas tablas. No convertirlo automáticamente en una secuencia de base de datos por esta corrección.
- Token CSRF: aleatorio criptográfico, no GUID de identidad documental. No sustituir por consecutivo.
- `config_hash` y `HuellaOrigenGabinete`: huellas SHA-256; no identificadores autoincrementales. Revisar la primera por la migración de su contenido, no cambiar su naturaleza.
- Códigos E2E: sufijos aleatorios para evitar colisiones de códigos funcionales de pruebas; no sustituyen las PK.

### Propagación a diseño posterior, todavía no implementado en DOC-110

El modelo común `Diseno/01-modelo-datos.md` define UID para `plan_id`, `lote_id`, `raiz_id`, `archivo_id`, `intento_id` y `evento_id`, además de sus referencias. Es una propagación del mismo diseño que debe corregirse en los documentos antes de implementar esas entregas. No afirmar que ya existen estas tablas por aparecer en el diseño. `staging_ref` y `token_reclamacion` tienen funciones de referencia opaca/control de ejecución y requieren tratamiento distinto de las PK.

Fuentes documentales a sincronizar: modelo común 01, contratos 02, decisión T02 en 04, contrato HTTP 07 y D-06 de `openspec/changes/doc-110-produccion-importacion-carpeta-perfil/design.md`. Revisar también los prompts que remiten a esos contratos, aunque no contengan literalmente GUID.

### Estado

Inventario registrado. No se ha aplicado DDL ni cambiado el comportamiento del código por esta revisión. La corrección comprende tres PK y tres referencias del perfil, sus JSON/huellas dependientes y contratos. SQL se entregará para ejecución manual; conservar datos, pertenencias, historial y aislamiento empresarial.

## Tareas de corrección COR-01 / COR-02

Estas tareas forman parte de la corrección de DOC-110 y de esta bitácora. La decisión vigente es sustituir las tres PK GUID por identificadores incrementales generados por MySQL. No se mantiene la propuesta inicial de corregir solo el perfil. Todas las tareas siguientes están pendientes; registrarlas no significa que se hayan ejecutado.

Las rutas SQL y de documentación funcional se expresan desde `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/`. Las demás parten de la raíz del repositorio.

### Contrato y preparación de datos

- [ ] T-COR-01 — Actualizar el contrato de identidad en `Diseno/01-modelo-datos.md`, `Diseno/02-contratos-operacion.md`, T02 de `Diseno/04-decisiones-y-pendientes.md`, `Diseno/07-contrato-http-seguridad-perfil.md` y `openspec/changes/doc-110-produccion-importacion-carpeta-perfil/design.md`. Definir las tres PK como `INT UNSIGNED AUTO_INCREMENT` y sus referencias como `INT UNSIGNED`, respetando nulabilidad. Precisar el tipo JSON y su correspondencia VB/JavaScript; no dejar validadores que exijan 32 caracteres. Verificación: un único contrato para las seis columnas y sus consumidores; tokens y huellas quedan distinguidos de las claves.

- [ ] T-COR-02 — Preparar el inventario de migración de RA mediante SELECT: cantidades, PK/FK, referencias lógicas a versión publicada, identidades presentes en `regla_doc` y huellas publicadas. Localizar consumidores adicionales antes de cambiar IDs. Diseñar correspondencias separadas perfil/versión/regla. Verificación: todas las relaciones tienen destino y el plan preserva filas, empresa, estados, revisiones e historial; no asumir que todos los datos se pueden descartar.

### Archivo SQL inicial y migración manual

- [ ] T-COR-03 — **Actualizar `Sql/01-perfil-importacion-mysql51.sql`** para instalaciones nuevas. Cambiar las tres PK y las tres referencias indicadas en COR-02; mantener FK, unicidades y el índice de listado. Solo las PK generan AUTO_INCREMENT. Conservar compatibilidad MySQL 5.1 y el alcance RA; no modificar permisos SII ni convertir el script inicial en un migrador de una base existente. Verificación: revisión de DDL y prueba de instalación en un esquema de prueba autorizado.

- [ ] T-COR-04 — **Crear `Sql/03-migrar-identificadores-perfil-mysql51.sql`** para la instalación ya existente. Preparar una migración basada en correspondencias, sin conversión directa GUID→entero, pérdida de filas ni reinicio de tablas. Trasladar perfil→versión, versión→regla y versión publicada de cabecera; reconstruir las restricciones e índices afectados y dejar los siguientes IDs disponibles correctamente. Las tablas auxiliares nuevas, si son necesarias, deben llevar prefijo `ra_`. Verificación: mismo número de registros y relaciones válidas antes/después, sin huérfanos ni colisiones. La aplicación corresponde al usuario, no al agente.

- [ ] T-COR-05 — Resolver la migración de `regla_doc` y `config_hash` antes de finalizar SQL 03. Definir cómo se conserva la evidencia de la huella anterior y cómo se obtiene/verifica la representación nueva sin modificar las reglas documentales publicadas. MySQL 5.1 no ofrece las funciones JSON de versiones modernas: no redactar un SQL ficticiamente autosuficiente ni reemplazos textuales indiscriminados. Si se necesita una herramienta auxiliar, documentar y entregar su ejecución manual coordinada con SQL 03. Verificación: ID de columna y JSON coherentes, y huella de cada publicación verificable con el contrato resultante.

- [ ] T-COR-06 — Actualizar `Sql/01-aplicacion-perfil-importacion.md` y documentar el procedimiento de SQL 03: destino RA, respaldo, detención de escrituras durante migración, orden de etapas, recuperación ante aplicación parcial y compatibilidad entre versión de código y esquema. Mantener instrucciones/controles operativos separados del SQL de cambios, conforme a la preferencia del usuario. Verificación: no se indica reejecutar SQL 01 sobre las tablas existentes ni se promete rollback transaccional del DDL.

### Actualización del código

- [ ] T-COR-07 — **Corregir `MySqlPerfilImportacionRepository.vb`**: retirar generación GUID de perfiles, versiones y reglas; omitir las PK en INSERT y recuperar los IDs asignados en la misma conexión/transacción. Utilizar esos IDs en las relaciones, lecturas y respuestas; no usar `MAX(id)+1`. Revisar copia de reglas al crear versión y borrado/reinserción al guardar borrador, manteniendo control de revisión y orden de bloqueos. Verificación: creación y versionado producen IDs asignados por la BD y referencias correctas bajo solicitudes concurrentes.

- [ ] T-COR-08 — **Actualizar modelos, interfaces, DTO y servicio**: `PerfilImportacionModels.vb`, `PerfilImportacionInterfaces.vb`, `PerfilImportacionDtos.vb`, `ServicioPerfilImportacion.vb` y composición de `WebServiceImportacionCarpetas.asmx.vb`. Eliminar exigencia GUID de claves persistidas; validar rango positivo, referencias opcionales y pertenencia al perfil/empresa. Revisar codec `PerfilImportacionJson.vb` y huella según T-COR-05. Verificación: IDs cero, negativos, fuera de rango, antiguos GUID y referencias ajenas se rechazan de forma controlada sin escritura.

- [ ] T-COR-09 — **Actualizar el verificador de esquema** del repositorio: tipo y nulabilidad de las seis columnas, AUTO_INCREMENT en exactamente las tres PK, índices y FK. Conservar consultas acotadas para MySQL 5.1 y rechazo seguro de esquema incompatible. Verificación: esquema nuevo aceptado, esquema GUID antiguo no utilizado por código incompatible, sin migraciones automáticas ni bloqueo de la carga individual.

- [ ] T-COR-10 — **Actualizar JavaScript y contratos de cliente**: `PerfilImportacionEditor.js`, consumidores de `PerfilImportacionApi.js` y payloads E2E. Preservar identificadores devueltos por servidor, distinguir referencia ausente de cero y revisar ReglaId en edición/clonado. Verificación: creación, recarga, selección de versión y guardado funcionan con el contrato nuevo sin conversiones ambiguas ni cambio de nivel documental.

### Verificación y cierre de la corrección

- [ ] T-COR-11 — Actualizar fixtures y pruebas de servicio/repositorio/cliente que usan GUID, y ejecutar compilación MSBuild y pruebas locales pertinentes. Añadir comprobaciones de generación del ID por BD y correspondencias de la migración. Verificación: evidencia de comandos/resultados, sin presentar pruebas con dobles como integración real.

- [ ] T-COR-12 — Tras la aplicación manual confirmada y con el ambiente/cuentas autorizados, repetir E2E usando `tools/e2e`: sesión, borrador, publicación EXPEDIENTE, segunda versión LOTE, inmutabilidad, historial y disponibilidad. Añadir concurrencia entre sesiones independientes y controles SELECT para las nuevas identidades/relaciones. Verificación: no basta repetir el caso con una sola sesión; registrar resultados saneados y pendientes reales.

- [ ] T-COR-13 — Sincronizar prompts y artefactos OpenSpec que remiten al contrato de identidad, además de la evidencia de implementación. Corregir el uso de UID para PK de las futuras entregas documentadas sin implementarlas dentro de DOC-110. Verificación: ninguna instrucción vigente exige GUID para las tres PK corregidas; las evidencias anteriores permanecen identificadas como corridas del esquema anterior.

### Orden de aplicación y criterio de cierre

Preparar contrato e inventario → resolver migración de JSON/huellas → entregar SQL y código coordinados → validar en entorno de prueba → aplicación manual del usuario → E2E y evidencia final. No ejecutar el código de IDs nuevos sobre el esquema GUID ni migrar la base activa mientras los escritores antiguos siguen operando.

COR-01/COR-02 solo se cierran cuando el código y ambos archivos SQL estén corregidos, la migración aplicada haya sido confirmada, las relaciones/publicaciones sean coherentes y las pruebas necesarias estén aprobadas. No cambiar su estado por haber completado únicamente este plan.

## Avance de corrección — 2026-10-20
Código corregido para IDs generados por MySQL en perfil, versión y regla; esquema inicial actualizado y migración conservadora preparada en SQL 03/04 más conversor JSON/huellas manual. Contrato vigente: Diseno/08-migracion-identidad-incremental.md. Estado oficial en tareas 10.1–10.20 de DOC-110. No se ejecutó migración ni se desplegó bin sobre el sitio GUID; pendientes comprobación real MySQL 5.1, E2E incremental y concurrencia/aislamiento entre sesiones.

## COR-03 — Búsqueda de perfiles sin autocompletado

Hallazgo del usuario durante la revisión funcional por partes. Confirmado en código: PerfilImportacionEditor.js, función list(), crea un input «Buscar por código exacto» y solo consulta al pulsar Buscar; no registra input/source de autocompletado. ListarPerfiles recibe codigo y paginación. No es un fallo de carga de sugerencias: la funcionalidad no está implementada.

MySqlPerfilImportacionRepository.Listar aplica codigo=@codigo y ofrece por separado búsqueda parcial de nombre con LOCATE(@nombre,nombre)>0. Ambos filtros se combinan con AND: enviar el mismo término a codigo y nombre no implementa una búsqueda por cualquiera de ellos. El listado respeta ambito_ref y limita páginas a 50 elementos. No descargar todos los perfiles ni usar solamente los 25 ya visibles como universo de sugerencias.

Reutilización disponible: WebFormProducionDocumental.aspx ya carga js/ui/jquery-ui-1.8.22.custom.js y jquery-ui.css; WebFormProducionDocumental.js contiene usos de autocomplete. Reutilizar el widget y el transporte PerfilImportacionApi con identidad/CSRF actuales. Verificar la API de la versión 1.8.22; no copiar llamadas de versiones nuevas como autocomplete("instance") sin comprobar compatibilidad.

Comportamiento propuesto: sugerir perfiles autorizados por código o nombre mientras se escribe; mostrar «código — nombre», conservar el ID real y permitir seleccionar para filtrar la lista. Mantener Buscar y limpieza del filtro. Consultas acotadas (por ejemplo 10 sugerencias), espera breve entre pulsaciones, descarte de respuestas antiguas, render de texto seguro y navegación por teclado. No cambiar el nivel documental ni abrir/guardar automáticamente un perfil al navegar sugerencias.

Estado: diagnóstico registrado; implementación pendiente como tareas oficiales de DOC-110, sección 11. No requiere tablas nuevas ni cambios SQL de esquema. El despliegue principal corresponde al usuario.

COR-03 actualizado por autorización expresa: implementado en código y DLL local. Búsqueda parcial literal por código/nombre, sugerencias desde 2 caracteres, teclado/limpieza y descarte de respuestas antiguas; sin SQL de esquema. Evidencia y límites en Validacion/doc110-cor03-autocomplete.md; tareas oficiales 11.1/11.2 completas, 11.3 pendiente de aceptación HTTP autenticada.

## COR-04 — Detalle guiado por interacción, sin cargas manuales redundantes

Revisión solicitada por el usuario. Alcance de esta entrada: diagnóstico del código, propuesta y tareas oficiales; no modifica todavía la interfaz. No se consultó ni cambió la base de datos.

### Diagnóstico comprobado

| Hallazgo | Evidencia | Consecuencia | Corrección propuesta |
|---|---|---|---|
| Ocho botones Cargar opciones, luego Más opciones/Recargar opciones | PerfilImportacionEditor.js catalog() y ocho invocaciones en render() | El usuario debe pedir catálogos necesarios para editar | Carga inicial de catálogos independientes; carga dependiente al seleccionar padre; paginación dentro del selector o Más resultados contextual, nunca eliminar acceso a páginas posteriores. |
| Campos dependientes habilitados sin contexto | catalog() solo vacía opciones al cambiar padre | Área/serie/subserie parecen utilizables aunque no hay selección previa | Estados esperando selección, cargando, disponible, vacío y error; habilitar cuando corresponda. |
| Cursor de paginación no se reinicia al cambiar el padre | page es local a catalog(); el cambio de padre vacía controles hijos pero no ese cursor | Puede pedir página 2 de otro instrumento/área y omitir la primera | Invalidar caché y cursor por contexto; reiniciar en página 1. |
| Respuestas de catálogos no están ligadas al contexto vigente | request() usa busy global; selects siguen editables | Cambiar padre durante petición puede aplicar resultados viejos; llamar varias cargas con request() descartaría todas salvo la primera | Control de solicitudes de catálogo por campo/época y contexto, separado del bloqueo de mutaciones; descartar respuestas antiguas y evitar tormenta de solicitudes. |
| Selecciones guardadas muestran ID sin descripción inicialmente | opciones Seleccionado: ID; texto solo mejora si ID aparece en página consultada | En páginas posteriores sigue el ID; publicado tiene fieldset deshabilitado y tampoco permite cargar etiquetas | Resolver etiquetas de referencias guardadas con consulta autorizada acotada; lectura también en publicado, sin editar configuración ni elegir sustitutos. |
| Índices de gabinete se piden con otro botón | Consultar índices del gabinete + indexPage | Añadir reglas exige una operación técnica manual | Cargar índices automáticamente con gabinete válido; su ausencia y sus errores deben ser visibles. |
| Cambio de gabinete no limpia los destinos visibles de reglas | change solo indices=[]; row.refreshIndices preserva ref aunque no exista | Puede conservar Campo guardado de otro gabinete, incluso si IDs aparentan coincidir | Invalidar destinos de índice al cambiar gabinete explícitamente y avisar; al abrir conservar valores para diagnosticar incompatibilidad, sin borrarlos silenciosamente. |
| Consultar estado está siempre visible y reemplaza edición | data-doc110=recargar llama load/render; sin dirty tracking | Puede perder cambios pendientes; activar/desactivar o editar cabecera también termina en render | Acción de recuperación contextual tras conflicto/resultado incierto, con decisión explícita sobre cambios pendientes. Proteger también navegación/cierre y otras acciones que reconstruyen formulario. |
| Publicar permite cambios locales sin guardar | PublicarVersion envía IDs/revisiones; Service.Publicar carga BD | Publica lo guardado aunque la pantalla muestre otra configuración | Deshabilitar publicación mientras haya cambios pendientes e indicar Guardar primero; no introducir guardado automático implícito. |
| Reglas muestran todos los controles simultáneamente | addRule: constante/herencia/extracción/separador/fecha/extensión siempre habilitados | Permite combinaciones que el resolvedor rechaza | Activación según destino, fuente, extracción y tipo; normalización explícita del payload y pruebas con el resolvedor, sin desactivar validaciones servidor. |
| Integridad parcial deja Ámbito editable | change fija NO_APLICA, pero no deshabilita selector; configuration() fuerza NO_APLICA | La pantalla puede representar una selección que nunca se guarda | Parcial: mostrar No aplica y bloquear campo; completo: habilitar expediente/lote con elección válida explícita. |

### Dependencias reales, sin suponer relaciones de catálogos

Fuente: PerfilImportacionCatalogAdapter.Consultar y Campos.

- Al abrir: FONDO, INSTRUMENTO, TIPO_EXPEDIENTE, UNIDAD_DOCUMENTAL y GABINETE son consultas independientes para este adaptador. Los permisos RA/DA se conservan. No inventar una dependencia Fondo → Instrumento: la consulta actual de instrumento no filtra por fondo.
- INSTRUMENTO → AREA; INSTRUMENTO + AREA → SERIE; INSTRUMENTO + AREA + SERIE → SUBSERIE. Subserie es opcional; catálogo vacío no es un error de transporte.
- GABINETE → INDICE, consultado en DA con permiso de almacenamiento. INDICE devuelve metadatos de tipo/longitud/requerido y tiene límite de 128 campos en servidor; su paginación actual es 50.
- TIPOLOGIA depende de instrumento, serie y subserie en el adaptador, pero el editor actual no ofrece selector general de tipologías: una regla puede resolverla desde un nombre de archivo. No sustituir todas esas reglas por una constante elegida.
- RESPONSABLE existe como catálogo, pero la regla también puede resolverlo desde otra fuente; no ampliar el ticket a cambiar el modelo declarativo.

### Interacción de reglas

CONSTANTE habilita valor constante; EXPEDIENTE habilita propiedad heredada y respeta las restricciones de destino del resolvedor; CARPETA/ARCHIVO habilitan extracción. SEGMENTO habilita separador y posición únicamente para carpeta/archivo; COMPLETO los desactiva. Quitar extensión solo aplica a ARCHIVO. Formato de fecha se habilita para FECHA y debe respetar yyyy-MM-dd para constante/herencia cuando lo exige el resolvedor. No permitir ARCHIVO/EXPEDIENTE como fuente de CAMPO_EXPEDIENTE, combinación rechazada por FUENTE_CIRCULAR_O_AMBIGUA. Tipo/longitud del índice se presentan desde sus metadatos y se validan en servidor; no aceptar NO_SOPORTADO como tipo editable.

### Comportamiento objetivo

Abrir perfil → recuperar valores guardados → resolver etiquetas/catálogos aplicables → habilitar campos con contexto válido. Cambiar instrumento/área/serie → invalidar únicamente descendientes → cargar el siguiente catálogo. Cambiar gabinete → invalidar referencias de índice y cargar las nuevas opciones. No autoelegir el primer elemento ni guardar/publicar por un cambio de selección.

Retirar Cargar opciones y Recargar opciones como acciones permanentes. Mantener Reintentar solo en el campo cuyo catálogo falló y acceso a más resultados solo cuando exista paginación. Consultar estado no se elimina del mecanismo de recuperación: se vuelve contextual y protege cambios locales. Un timeout de escritura no autoriza reintento automático.

### Pruebas y alcance

El recorrido de publicación E2E actual guarda buena parte de la configuración por API y usa recargar para verla; no demuestra un formulario guiado completo. COR-04 requiere un recorrido que seleccione campos desde UI y pruebe dependencias/páginas/respuestas tardías, además de los casos de guardado/publicación existentes. Reutilizar tools/e2e y autenticación vigente. Eliminar el selector recargar obliga a actualizar ese recorrido con el comportamiento real, no simular éxito.

No requiere nuevas tablas ni cambio SQL de esquema. Puede requerir ampliar ConsultarCatalogo para resolver referencias guardadas autorizadas sin recorrer todo un catálogo; concretar contrato y límites antes de implementarlo. No usar caches compartidas entre usuarios/empresas. Estado oficial: sección 12 de tasks.md, pendiente de implementación.

COR-04 implementado por autorización expresa. Cada hallazgo tiene tarea individual oficial 12.7–12.17; ejecución y verificaciones completas junto con 12.1–12.6. Evidencia en Validacion/doc110-cor04-detalle-guiado.md: configuración guiada, protección de cambios, catálogos e índices automáticos, prueba autenticada UI PASS. Sustituye el estado pendiente de la entrada diagnóstica anterior; no cambia el SQL ni el servidor principal.

## COR-05 — Índices del gabinete mostrados como 0

Revisión del usuario: en el selector de índices solo aparece correctamente Nombre beneficiario. Confirmado mediante SELECT de metadatos DA de CONTABIL: 21 campos visibles, 20 con ALEAS_CAMPO=0 y uno con alias descriptivo. Campos usaba el alias cuando no estaba vacío, por lo que presentaba literalmente el marcador legacy 0. Es un defecto de etiqueta del catálogo; esta consulta no demuestra valores cero en documentos almacenados.

Corrección en PerfilImportacionCatalogAdapter.Campos: cuando el alias es NULL, vacío, espacios o 0, mostrar CAMPO; conservar el alias descriptivo. IDs, tipos, longitudes, permisos y reglas guardadas conservan su comportamiento. No requiere SQL ni modificación de SII.

Tareas oficiales: 13.1–13.3 de DOC-110, completadas. Harness de comportamiento compilado PASS, compilación aislada PASS y contraste de 21 etiquetas del adaptador real con metadatos DA PASS (20 nombres de campo recuperados, un alias conservado, cero etiquetas 0). Evidencia: Validacion/doc110-cor05-indices.json. La validación real usa SELECT y el adaptador; no es una nueva E2E HTTP autenticada. DLL local actualizada con respaldo; servidor principal a cargo del usuario.