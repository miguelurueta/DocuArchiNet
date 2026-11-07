# produccion-importacion-carpeta-perfil Specification

## Purpose

Administrar, validar, versionar y seleccionar perfiles de importación documental y sus fuentes autorizadas, preservando la carga individual y SII. Esta capacidad configura futuras importaciones sin ejecutar lectura de carpetas, lotes ni creación de expedientes.

## Requirements

### Requirement: Aceptación en instalación de empresa única
La aceptación de DOC-110 SHALL reflejar la confirmación explícita del usuario de que la instalación opera y operará con una sola empresa. Las pruebas reales entre empresas SHALL registrarse NO_APLICA, sin exigir otra empresa ni cuentas de otro ámbito. SHALL conservarse las defensas existentes de contexto servidor, autorización por recurso, facultades, sesión, CSRF y revisión esperada.

#### Scenario: No existe segunda empresa por alcance operativo
- **WHEN** se evalúan criterios de búsqueda, proveedores, fuentes o perfiles que requieren dos empresas
- **THEN** la parte de aceptación real entre empresas se registra NO_APLICA conforme a la decisión del usuario, sin atribuirle PASS; los demás criterios siguen requiriendo su propia evidencia.

### Requirement: RQ-03 Herencia de clasificación documental
El perfil SHALL permitir heredar ID, código configurado y nombre de serie/subserie mediante origen EXPEDIENTE y etapa TRAS_EXPEDIENTE. Los códigos SHALL proceder de Codigo_Arbitrario como texto, sin sustituirlos por identificadores o consecutivos. El ensayo SHALL mostrar una propuesta autorizada desde el perfil, no una confirmación de expediente. Contrato detallado en Diseno/14-herencia-clasificacion.md.

#### Scenario: Propuesta autorizada frente a datos del cliente
- **WHEN** se ensaya una regla de clasificación y el cliente envía valores distintos de serie/subserie
- **THEN** el servidor reemplaza esas propiedades por la proyección del catálogo autorizado del perfil y etiqueta el resultado PROPUESTA_PERFIL, sin modificar el expediente ni la publicación.

#### Scenario: Subserie ausente o clasificación pendiente
- **WHEN** no hay serie seleccionada o la serie seleccionada no tiene subserie configurada en el perfil
- **THEN** el primer caso queda diferido y el segundo es ausencia conocida; una regla obligatoria rechaza la ausencia y una opcional conserva null, sujeto a las restricciones del índice, sin producir cero artificial.

#### Scenario: Compatibilidad de códigos y publicaciones
- **WHEN** se configura un código de serie con ceros iniciales o se ensaya una versión publicada
- **THEN** el código conserva su representación textual y la publicación se evalúa desde su configuración persistida sin alterar su huella.

### Requirement: RQ-01 — Configuración aislada y compatibilidad
El sistema SHALL administrar perfiles sin invocar recepción, creación de expedientes, almacenamiento o incorporación y SHALL preservar SII y carga individual.

Trazabilidad: D-01, RQ-01; desarrollo en design.md.

#### Scenario: Ensayo sin efectos
- **WHEN** un usuario autorizado ensaya reglas con nombres y metadatos
- **THEN** se devuelven valores/incidencias sin leer archivos ni crear objetos documentales

#### Scenario: Esquema propio ausente
- **WHEN** el esquema de perfiles falta o es incompatible
- **THEN** solo la capacidad nueva informa indisponibilidad; Producción y carga individual permanecen operativas y no se ejecuta DDL

#### Scenario: UI integrada
- **WHEN** se abre y cierra repetidamente la administración después de UpdatePanel
- **THEN** se conservan foco, navegación y contratos existentes sin eventos o controles duplicados

### Requirement: RQ-02 — Versiones y concurrencia
El sistema SHALL guardar borradores con revisión y publicar atómicamente versiones inmutables, conservando historial.

Trazabilidad: D-02, RQ-02; desarrollo en design.md.

#### Scenario: Conflicto de edición
- **WHEN** dos solicitudes guardan el mismo borrador con igual revisión
- **THEN** una escritura válida incrementa revisión y la obsoleta se rechaza sin sobrescribir datos

#### Scenario: Publicación válida
- **WHEN** configuración completa y revisión vigente son autorizadas y publicadas
- **THEN** estado, configuración/reglas y referencia publicada pertenecen al mismo perfil y se confirman en transacción

#### Scenario: Publicación inválida
- **WHEN** la versión contiene errores o revisión obsoleta
- **THEN** la publicación se rechaza sin alterar referencia ni versión anterior

#### Scenario: Nueva versión
- **WHEN** se crea un borrador desde una versión publicada autorizada
- **THEN** se asigna identidad/número único sin modificar la publicación original, incluso ante concurrencia

#### Scenario: Desactivación
- **WHEN** se desactiva un perfil con autorización y revisión válidas
- **THEN** el historial permanece consultable con permiso y el perfil deja de ofrecerse para nuevas selecciones

#### Scenario: Contrato de lotes anteriores
- **WHEN** se desactiva un perfil utilizado por un lote ya confirmado
- **THEN** el contrato preserva continuidad con snapshot original y permisos vigentes; el servicio del perfil no detiene el lote, y nuevas confirmaciones se rechazan en su entrega propietaria

#### Scenario: Permiso revocado independiente de disponibilidad
- **WHEN** un lote previo pierde autorización para ejecutar efectos
- **THEN** el coordinador debe detener nuevos efectos afectados en punto seguro, aunque la desactivación del perfil por sí sola no lo detenga; esta ejecución pertenece a la entrega operativa

#### Scenario: Respuesta perdida
- **WHEN** el cliente pierde la respuesta de una mutación
- **THEN** consulta el estado persistido antes de repetirla y no anuncia éxito ni reenvía ciegamente

### Requirement: RQ-03 — Reglas configurables y catálogos
El sistema SHALL validar reglas declarativas y referencias de catálogo y resolver metadatos de manera determinista, sin formato de nombre fijo.

Trazabilidad: D-03, RQ-03; desarrollo en design.md.

#### Scenario: Dos patrones
- **WHEN** se guardan y ensayan dos estructuras distintas de nombre
- **THEN** cada una produce los campos definidos por su propia configuración y conserva ceros iniciales compatibles

#### Scenario: Política de coincidencias versionada
- **WHEN** se crea configuración de perfil o se cambia su política de coincidencias
- **THEN** BLOQUEAR es el default persistido y visible, REUTILIZAR_VALIDADO es opción explícita y valores desconocidos impiden publicación; publicaciones anteriores conservan su política

#### Scenario: Límites de reutilización
- **WHEN** un perfil publicado configura REUTILIZAR_VALIDADO
- **THEN** conserva el contrato de coincidencia única, empresa/estructura/clasificación compatibles, estado y permisos vigentes; no autoriza omitir archivos ni modificar datos existentes para hacerlos coincidir, y DOC-110 no ejecuta reutilización durante publicación

#### Scenario: Destino inválido
- **WHEN** una regla usa un índice de otro gabinete o tipología desconocida
- **THEN** la publicación se rechaza con incidencia por campo sin modificar catálogos o equivalencias SII

#### Scenario: Asignación contradictoria
- **WHEN** dos fuentes asignan el mismo destino sin precedencia contractual única
- **THEN** la configuración se rechaza; no gana una fuente por orden accidental de UI

#### Scenario: Requeridos y conversiones
- **WHEN** una fuente omite un requerido o excede tipo/longitud/representación
- **THEN** el servicio emite incidencia específica y no publica esa configuración

#### Scenario: Valor futuro
- **WHEN** una fuente válida requiere el ID de expediente aún inexistente
- **THEN** devuelve fuente diferida tipada y nunca crea expediente ni fabrica ID para completar el ensayo

#### Scenario: Contenido ejecutable
- **WHEN** la configuración incluye SQL, nombres SQL libres o expresiones ejecutables
- **THEN** se rechaza sin ejecutarlos

#### Scenario: Fuente circular o ambigua de expediente
- **WHEN** un campo necesario para crear el expediente depende de su ID futuro o del nombre de uno de varios archivos sin contrato de reducción
- **THEN** se rechaza la configuración; no se crea un expediente de prueba ni se elige un archivo por orden

#### Scenario: Fechas documentales deterministas
- **WHEN** se resuelven fechas inicial/final conforme al contrato de fuentes
- **THEN** se usa formato explícito y calendario válido, se exige inicial para Conservación y se rechaza final anterior; la ausencia opcional no se reemplaza por la fecha actual

#### Scenario: Responsable desde catálogo
- **WHEN** una regla constante o de carpeta determina responsable
- **THEN** se exige referencia autorizada o búsqueda de coincidencia única; una búsqueda fallida no se reemplaza por el usuario importador

#### Scenario: Código aportado compatible
- **WHEN** una regla de código de expediente produce texto con ceros iniciales o supera 45 caracteres
- **THEN** conserva los ceros y rechaza exceso bajo el contrato inicial sin truncar ni añadir sufijos; no confunde código administrativo del perfil con código de expediente

### Requirement: RQ-04 — Modo y ámbito de integridad
El sistema SHALL conservar modo y ámbito en la versión, con COMPLETO_OBLIGATORIO por defecto y selección explícita de EXPEDIENTE o LOTE al publicar.

Trazabilidad: D-04, RQ-04; desarrollo en design.md.

#### Scenario: Completo sin ámbito
- **WHEN** se intenta publicar completo sin elegir ámbito
- **THEN** la publicación se rechaza aunque el resto del perfil sea válido

#### Scenario: Ambos ámbitos
- **WHEN** se publican versiones completas con EXPEDIENTE y LOTE respectivamente
- **THEN** cada versión conserva su elección sin imponer un ámbito global

#### Scenario: Parcial explícito
- **WHEN** se configura PARCIAL_PERMITIDO
- **THEN** se normaliza ámbito a NO_APLICA y se conserva en versión; este ticket no ejecuta barreras documentales

### Requirement: RQ-05 — Autoridad y contexto de selección
El sistema SHALL obtener identidad/ámbito de autoridad servidor, exigir capacidades específicas separadas para administración e importación, autorizar cada operación y recurso, y proteger mutaciones HTTP; ningún dato cliente sustituye esos controles. DOC-110 implementa solo ADMINISTRAR_PERFILES_IMPORTACION, deshabilitada por defecto; IMPORTAR_CARPETAS pertenece a la entrega posterior.

Trazabilidad: D-05, RQ-05; desarrollo en design.md.

#### Scenario: Recurso ajeno
- **WHEN** un usuario consulta, edita, publica o desactiva un perfil ajeno sin permiso
- **THEN** se rechaza sin escritura ni filtración de configuración

#### Scenario: Sesión o protección inválida
- **WHEN** una mutación carece de sesión o protección CSRF válida
- **THEN** se rechaza antes de persistir, aunque la UI hubiera mostrado la acción

#### Scenario: Token ligado al contexto
- **WHEN** una solicitud a la fachada del perfil presenta token de otra sesión o de identidad/empresa anterior
- **THEN** el guard rechaza antes del servicio; un token válido tampoco sustituye autorización vigente del recurso

#### Scenario: Recuperación tras expiración
- **WHEN** la sesión expira o el transporte retorna HTML, timeout o respuesta no interpretable
- **THEN** el cliente libera el estado de espera, no declara éxito ni reenvía mutaciones automáticamente y obtiene contexto/estado autorizado antes de continuar

#### Scenario: Propósito administrativo de selección
- **WHEN** DOC-110 valida selección para revisar compatibilidad administrativa
- **THEN** exige permiso administrativo y nivel autorizado sin iniciar lote; propósito IMPORTACION permanece indisponible hasta integrar su permiso específico en la entrega operativa

#### Scenario: Revisión diferenciada al publicar
- **WHEN** PublicarVersion recibe revisión obsoleta de cabecera o de versión
- **THEN** rechaza sin sustituir referencia publicada ni alterar configuración confirmada

#### Scenario: Selección compatible
- **WHEN** un usuario autorizado elige versión publicada disponible compatible con el nivel recibido
- **THEN** se devuelve esa configuración conservando el mismo nivel y sin conceder permisos de crear/cargar

#### Scenario: Selección incompatible
- **WHEN** la versión es borrador, está indisponible o no es compatible con el nivel autorizado
- **THEN** se rechaza selección sin sustituir destino o versión silenciosamente

#### Scenario: Permiso administrativo específico
- **WHEN** el usuario intenta crear, editar, publicar o cambiar disponibilidad de un perfil
- **THEN** el servidor exige ADMINISTRAR_PERFILES_IMPORTACION y autorización del recurso/ámbito; IMPORTAR_CARPETAS, GA_MANAGER_CONFIGURACION o propiedad del nivel no lo sustituyen

#### Scenario: Permisos deshabilitados por defecto
- **WHEN** se incorpora la capacidad administrativa a la infraestructura existente
- **THEN** queda deshabilitada sin asignaciones automáticas; valor ausente, nulo o inválido no habilita operaciones

#### Scenario: Administración separada de ejecución
- **WHEN** un usuario solo tiene ADMINISTRAR_PERFILES_IMPORTACION
- **THEN** puede administrar recursos autorizados pero esa capacidad no habilita importación; la entrega posterior exige IMPORTAR_CARPETAS y permisos documentales aplicables

#### Scenario: Asignación mediante infraestructura existente
- **WHEN** se asigna o revoca la capacidad administrativa
- **THEN** se usa el mecanismo administrativo existente con su propia autorización y el editor de perfiles no permite autoasignarse privilegios; la comprobación servidor refleja la política de vigencia de permisos

#### Scenario: Empresa sin relación válida
- **WHEN** la resolución de empresa devuelve cero, relación ambigua o contexto incoherente
- **THEN** se rechaza la operación sin persistencia aunque la función legacy devuelva YES

#### Scenario: Origen documental del ámbito
- **WHEN** se resuelve el contexto para consultar o modificar perfiles
- **THEN** usuario, empresa y repositorio de perfiles usan el mismo origen documental RA autorizado; no se sustituye por la conexión sin prefijo ni se acepta empresa autoritativa del cliente

#### Scenario: Revocación vigente
- **WHEN** se revoca la capacidad administrativa y llega la siguiente solicitud protegida
- **THEN** se deniega aunque la sesión conserve el permiso anterior

#### Scenario: Esquema de permiso no aplicado
- **WHEN** la columna del permiso nuevo aún no existe
- **THEN** se deshabilitan perfiles sin romper login ni permisos anteriores

#### Scenario: Errores seguros
- **WHEN** ASMX produce rechazo, expiración o error inesperado
- **THEN** el cliente representa resultado contractual y no expone SQL, stack, cadenas de conexión o HTML interno

### Requirement: RQ-06 — SQL manual compatible
La entrega SHALL incluir DDL concreto MySQL 5.1 de configuración y procedimiento manual; la aplicación SHALL NOT ejecutar migraciones automáticamente.

Trazabilidad: D-06, RQ-06; desarrollo en design.md.

#### Scenario: Revisión de DDL
- **WHEN** se entrega Sql/01-perfil-importacion-mysql51.sql
- **THEN** contiene tablas/índices/relaciones propios, precondiciones, orden y controles SELECT sin alterar SII ni crear entidades operativas; incluye la extensión justificada de permisos administrativos con default 0, sin concederla masivamente

#### Scenario: Compatibilidad física
- **WHEN** se revisan tipos, collations e índices contra esquema autorizado
- **THEN** son compatibles con MySQL 5.1/InnoDB y referencias verificadas, sin JSON nativo, utf8mb4 o CHECK como garantía

#### Scenario: DDL parcial o previo
- **WHEN** existen objetos previos o la aplicación manual quedó incompleta
- **THEN** se detectan diferencias y se documenta recuperación sin afirmar idempotencia por IF NOT EXISTS ni rollback transaccional global

### Requirement: RQ-07 — Evidencia progresiva y E2E reutilizada
La entrega SHALL automatizar PER-01–03 sobre tools/e2e existente y distinguir evidencia real, simulada y no ejecutada.

Trazabilidad: D-07, RQ-07; desarrollo en design.md.

#### Scenario: PER-01
- **WHEN** en ambiente autorizado se crea perfil, se rechaza completo sin ámbito y se publica con EXPEDIENTE
- **THEN** recargar conserva configuración/reglas y versión; observación SELECT concuerda con UI/HTTP

#### Scenario: PER-02
- **WHEN** se publica otra versión con LOTE y se intenta publicar con cuenta restringida
- **THEN** primera versión permanece intacta y cuenta restringida no escribe

#### Scenario: PER-03
- **WHEN** se guardan y recargan reglas de dos formatos y se introduce requerido ausente o tipología inválida
- **THEN** los resultados persisten y la configuración inválida no publica

#### Scenario: Reutilización de infraestructura
- **WHEN** se incorporan las especificaciones del perfil
- **THEN** usan configuración, autenticación, runners y reporteros existentes; extensiones compatibles prueban consumidores y no crean login/.env/Playwright paralelos

#### Scenario: Validación sin autorización
- **WHEN** no hay autorización de ambiente/cuentas/datos descartables o SQL aplicado por el equipo
- **THEN** los escenarios reales quedan NO_EJECUTADO con causa; mocks y revisión estática no se registran como E2E aprobada

#### Scenario: Evidencia saneada
- **WHEN** se registra una corrida autorizada
- **THEN** incluye SHA, comandos y resultados por escenario sin credenciales/cookies/tokens/documentos reales sensibles; controles de BD son SELECT


### Requirement: RQ-08 — Identidad incremental y migración verificable
El sistema SHALL usar INT UNSIGNED AUTO_INCREMENT para las PK de perfil, versión y regla, referencias numéricas compatibles y transporte String decimal canónico positivo; la referencia publicada opcional SHALL admitir null. SHALL conservar autorización por ámbito y evidencia de la migración de JSON y huellas.

#### Scenario: Crear identidades desde MySQL
- **WHEN** se crea un perfil o una versión con reglas
- **THEN** el repositorio omite las PK en INSERT y recupera cada ID en la misma conexión/transacción
- **AND** regla_doc conserva el ReglaId incremental correspondiente sin cambiar valores documentales

#### Scenario: Migrar publicaciones existentes
- **WHEN** el equipo aplica manualmente la migración aprobada
- **THEN** se verifican las huellas originales antes de transformar ReglaId
- **AND** se conservan correspondencias, respaldo y huellas anteriores/nuevas, verificadas con el codec de producción

#### Scenario: Rechazar referencias no canónicas
- **WHEN** se recibe un GUID anterior, cero, negativo, ceros iniciales o un entero fuera de rango como identidad persistida
- **THEN** la operación se rechaza sin escribir datos

### Requirement: RQ-09 — Autocompletado autorizado de perfiles
El sistema SHALL sugerir perfiles por coincidencia parcial literal de código o nombre usando el ámbito autorizado y consultas acotadas; SHALL conservar el filtro exacto previo y la navegación del nivel documental.

#### Scenario: Seleccionar una sugerencia
- **WHEN** el usuario escribe al menos dos caracteres y selecciona una sugerencia
- **THEN** la lista se filtra por su código único dentro del ámbito sin abrir ni guardar el perfil
- **AND** el menú muestra código y nombre como texto seguro y permite navegación por teclado

#### Scenario: Limpiar o cambiar una búsqueda
- **WHEN** cambia el término o se limpia el filtro
- **THEN** no se muestran respuestas de términos anteriores y la limpieza vuelve a la primera página sin filtros

### Requirement: RQ-10 — Detalle guiado sin pérdida de edición
El sistema SHALL cargar catálogos automáticamente según dependencias y permisos, resolver etiquetas guardadas de forma acotada y conservar control explícito sobre cambios del borrador.

#### Scenario: Cambiar contexto de clasificación o gabinete
- **WHEN** cambia una selección padre
- **THEN** invalida descendientes, reinicia paginación y descarta respuestas anteriores
- **AND** carga únicamente opciones del contexto nuevo sin autoelegir un índice

#### Scenario: Consultar referencia guardada fuera de página
- **WHEN** una referencia guardada no está en la página inicial
- **THEN** consulta su etiqueta mediante filtro puntual autorizado incluso en publicado
- **AND** informa si falta sin sustituirla por otro valor

#### Scenario: Proteger cambios y recuperar estado
- **WHEN** hay cambios locales sin guardar
- **THEN** no permite publicar y requiere decisión antes de descartar edición al navegar o recuperar estado
- **AND** un resultado incierto no reenvía automáticamente una escritura

#### Scenario: Activar controles aplicables
- **WHEN** cambia fuente, destino, extracción, tipo o integridad
- **THEN** habilita únicamente campos aplicables y conserva las restricciones del resolvedor

### Requirement: RQ-11 — Proveedores versionados de datos para índices
El sistema SHALL permitir reglas de INDICE_GABINETE con fuente de proveedor registrado, versión explícita, clave configurable y propiedad declarada. SHALL separar consulta en servidor de resolución pura y conservar validaciones DA, fuentes previas y huellas publicadas. La primera entrega SHALL implementar un adaptador común de tablas/vistas configurables por proveedor; no SHALL modificar SII ni habilitar API sin proveedor implementado. Contrato D-08: Diseno/09-proveedores-datos-indices.md bajo la raíz funcional.

#### Scenario: Consultar una clave y asignar varias propiedades
- **WHEN** varias reglas autorizadas consultan el mismo proveedor/version/clave dentro de un ensayo
- **THEN** se comparte el resultado en ese contexto y cada propiedad se transforma y valida contra su índice
- **AND** una clave de texto conserva ceros iniciales y los datos consultados proceden del servidor

#### Scenario: Rechazar ausencias y ambigüedades
- **WHEN** la consulta retorna cero o varias coincidencias, o una propiedad requerida no tiene valor
- **THEN** se devuelve incidencia diferenciada por campo sin seleccionar arbitrariamente una fila ni inventar 0
- **AND** las reglas opcionales tampoco sustituyen silenciosamente una consulta fallida por un valor válido

#### Scenario: Rechazar origen no autorizado
- **WHEN** se solicita un proveedor de otro ámbito o una referencia desactivada para una nueva consulta
- **THEN** se rechaza antes de acceder al origen sin aceptar credenciales, SQL o conexiones enviados por el cliente

#### Scenario: Diferir clave dependiente del expediente
- **WHEN** la clave necesita una propiedad permitida del expediente que aún no existe
- **THEN** se declara dependencia TRAS_EXPEDIENTE y no se consulta con datos ficticios

#### Scenario: Preservar publicaciones existentes
- **WHEN** se incorpora la extensión o se cambia el contrato de un proveedor
- **THEN** los perfiles previos conservan lectura y huella y las publicaciones nuevas referencian una versión concreta
- **AND** no se reescribe una versión de proveedor utilizada para cambiar silenciosamente su significado

#### Scenario: Conservar valores confirmados para reintentos
- **WHEN** el motor consumidor reintenta almacenar un documento con valores aprobados del plan
- **THEN** usa esos valores sin consultar automáticamente otra vez el origen
- **AND** actualizar los valores exige revisión y confirmación del plan, conservando la revalidación de permisos y política de integridad
#### Scenario: Configurar fuentes diferentes sin cambiar código
- **WHEN** se registran dos proveedores con tablas/vistas, claves y columnas distintas dentro de sus conexiones autorizadas
- **THEN** el mismo adaptador consulta ambos usando sus versiones de configuración sin clases ni condiciones específicas por tabla
- **AND** el perfil selecciona proveedor/version y propiedad lógica sin almacenar SQL ni credenciales

#### Scenario: Validar identificadores y cambios de esquema
- **WHEN** una configuración referencia una tabla/columna no autorizada, una expresión SQL o metadatos incompatibles
- **THEN** se rechaza antes de consultar datos y se informa incompatibilidad sin cambiar automáticamente el mapeo
- **AND** los identificadores admitidos se validan y delimitan, mientras que claves y filtros usan parámetros

#### Scenario: Versionar un cambio de fuente
- **WHEN** se cambia la conexión lógica, tabla, clave, columnas o aislamiento de un proveedor
- **THEN** se genera una versión nueva sin alterar las referencias de los perfiles publicados
### Requirement: RQ-12 — Administración visual de proveedores
El sistema SHALL administrar proveedores desde interfaz dentro de DOC-110, con borrador editable, revisión, validación, publicación inmutable, historial y disponibilidad. SHALL permitir seleccionar conexión lógica autorizada, tabla/vista, clave, aislamiento empresarial y columnas mediante metadatos. SHALL reservar SQL manual para instalación/actualización de estructura y permisos; no SHALL exigir scripts de registro funcional. D-09 se desarrolla en Diseno/10-catalogo-proveedores-tabla.md.

#### Scenario: Crear y publicar desde interfaz
- **WHEN** el administrador autorizado configura un proveedor, valida su estructura y publica su borrador guardado
- **THEN** el servidor registra la versión publicada y la ofrece a perfiles del ámbito autorizado sin INSERT manual del usuario
- **AND** la consulta de prueba es de solo lectura y no publica ni guarda automáticamente

#### Scenario: Evitar autorización circular
- **WHEN** se solicitan conexiones, tablas o columnas para crear un proveedor
- **THEN** el servidor aplica la autorización de fuentes independientemente de la configuración propuesta por el cliente
- **AND** rechaza metadatos ajenos sin mostrar credenciales ni aceptar SQL libre

#### Scenario: Rechazar borradores en perfiles
- **WHEN** un cliente intenta seleccionar o ensayar una regla que referencia una versión BORRADOR
- **THEN** el servidor la rechaza aunque conozca su ID y el proveedor esté activo

#### Scenario: Preservar versión y revisión
- **WHEN** se cambia una configuración publicada o se guarda con una revisión antigua
- **THEN** la publicación existente permanece intacta y la edición exige otro borrador o produce conflicto de revisión respectivamente

#### Scenario: Actualizar instalación existente
- **WHEN** se entrega el esquema del ciclo administrativo sobre las tablas instaladas por SQL 04
- **THEN** se entrega una migración incremental MySQL 5.1 para aplicación manual del usuario, sin recrear tablas ni publicar automáticamente registros previos

### Requirement: Fuente exclusiva de empresa autorizada por servidor
El proveedor SHALL permitir omitir columna de empresa solamente cuando la politica servidor autorice ExclusivaEmpresa=true para el ambito, conexion y tabla actuales. SHALL preservar filtro por columna cuando esta exista, y rechazar omision sin autorizacion antes de consultar. No requiere alterar tabla de negocio.

#### Scenario: Tabla exclusiva sin columna de empresa
- **WHEN** la fuente esta autorizada como exclusiva para la empresa actual y se consulta por CONSECUTIVO
- **THEN** se usa la conexion del modulo resuelta en servidor y la clave parametrizada, sin columna artificial de empresa; multiples filas producen AMBIGUO.

#### Scenario: Omision sin autorizacion o ambito ajeno
- **WHEN** un proveedor omite empresa y la fuente no tiene autorizacion exclusiva para el ambito actual
- **THEN** la consulta es denegada antes de abrir conexion.


### Requirement: RQ-12 ampliacion AMP-02 - Administracion persistente de fuentes
El sistema SHALL administrar autorizaciones de fuentes en RA mediante interfaz y facultad ADMINISTRAR_FUENTES_IMPORTACION adicional a perfiles. SHALL separar esa facultad de administrar proveedores, conservar identidad por empresa/conexion/tabla, historial transaccional y revision esperada. La politica SHALL proceder solo de RA tras la migracion, sin fallback a Web.config. El contrato detallado y diccionario estan en Diseno/11-administracion-fuentes-da.md.

#### Scenario: Alta inicial de fuente DA
- **WHEN** un administrador de fuentes solicita candidatos para su conexion DA de sesion
- **THEN** puede descubrir tablas/vistas y columnas de esa base sin leer filas de negocio ni depender de una autorizacion previamente registrada; un usuario sin esa facultad no puede usar este descubrimiento.

#### Scenario: Autorizacion con aislamiento por columna
- **WHEN** se utiliza una fuente de modalidad COLUMNA
- **THEN** el proveedor debe usar la columna empresarial exacta de la autorizacion; otra columna permitida no sustituye ese filtro.

#### Scenario: Revision e historial atomicos
- **WHEN** se modifica o revoca una fuente con revision vigente
- **THEN** cabecera, columnas e historial resultante se confirman en una transaccion; una revision obsoleta o fallo al guardar historial no deja cambios parciales.

#### Scenario: Revocacion y falla de lectura
- **WHEN** una nueva peticion consulta una fuente revocada o no puede leer la politica RA
- **THEN** se deniega el uso sin recurrir al archivo y sin modificar proveedores publicados; consultas ya iniciadas no se cancelan retroactivamente.


### Requirement: RQ-12 limite de tablas imp_02
El servidor SHALL limitar descubrimiento, autorizacion y uso de fuentes al prefijo literal imp_02 o a nombres exactos adicionales configurados por conexion en el servidor. El cliente no SHALL ampliar este limite; la autorizacion persistida en RA no SHALL eludirlo.

#### Scenario: Solicitud directa de tabla ajena
- **WHEN** un administrador solicita columnas de una tabla fuera del prefijo y sin excepcion
- **THEN** el servidor rechaza antes de conectar al origen, aunque el usuario pueda administrar fuentes.

#### Scenario: Excepcion exacta por conexion
- **WHEN** el servidor configura una tabla adicional para DA
- **THEN** solo ese nombre en DA supera el limite; otra conexion o un nombre parecido no se habilita, y el uso sigue requiriendo autorizacion RA activa.
