# DOC-110 AMP-01 — Componentes configurables

Estado: implementación parcial; no capacidad disponible todavía en el formulario.

## Entregado

- Diccionario y selección de orígenes: Diseno/10-catalogo-proveedores-tabla.md.
- SQL 04-proveedores-indices-mysql51.sql: dos tablas RA, INT UNSIGNED AUTO_INCREMENT, InnoDB, LONGTEXT y FK restrictiva. Revisión estática: sin JSON nativo, CHECK, DATETIME fraccional, utf8mb4 ni sintaxis posterior a 5.1. No ejecutado en MySQL; no acredita instalación.
- ProveedorIndicesModels.vb: contrato de configuración, propiedades y resultado; puerto de consulta independiente del transporte.
- MySqlProveedorDatosIndices.vb: conexión lógica inyectada, metadatos, identificadores validados, claves/filtro empresa parametrizados, tipos acotados, LIMIT 2, timeout de comando y errores saneados. Solo SELECT.
- MySqlProveedorIndicesRepository.vb: lectura parametrizada de versión activa por ámbito en RA; compilada, pendiente validación integrada del catálogo.
- Registro de compilación en vbproj y pruebas en el harness existente; no se modifican fuentes SII.

## Verificación ejecutada

`node --test tests/doc110-perfil-importacion-behavior.test.cjs`: PASS. Compila y ejecuta VB/C# .NET Framework, incluida regresión previa. Nuevo ProveedorIndicesBehaviorTests verifica dos tablas/claves diferentes con el mismo adaptador, clave 00125 conservada, resultado único/ausente/ambiguo, bloqueo previo a lectura de otro ámbito e identificador manipulado, metadatos incompatibles, alias de conexión ausente, timeout saneado, clave vacía/fecha imposible/NUL y serialización de propiedades. Transporte de consulta simulado: no es E2E ni acceso a datos reales.

`MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /p:OutputPath=.tmp/doc110-amp01-build/ /p:OutDir=.tmp/doc110-amp01-build/ /p:IntermediateOutputPath=.tmp/doc110-amp01-obj/ /clp:ErrorsOnly /nologo`: PASS.

## Pendiente

Prueba de repositorio/esquema, enlace de reglas con proveedor y compatibilidad de huellas, coordinación del servicio, catálogo HTTP, controles del editor y E2E autorizada. No se ha copiado la DLL a bin ni aplicado SQL. Los modelos nuevos están separados del modelo de reglas actual, que conserva su forma. Tareas oficiales abiertas reflejan estos pendientes; las pruebas de componentes no los cierran.
## Segunda etapa — reglas y coordinación (sustituye los pendientes de código correspondientes)

FuenteProveedorIndices y ClaveProveedorIndices agregadas al contrato cerrado de reglas. Proveedor es opcional con NullValueHandling.Ignore exclusivamente sobre la propiedad nueva: no cambia JSON de reglas previas. Fixture tests/Fixtures/ProduccionDocumental/doc110-perfil-antes-proveedor.json generado con la DLL de la etapa anterior antes de editar el modelo. Huella esperada c16eaf987e48e6897edfadd215b7803a97d484b02f1540e97257c58217f4f401; prueba de lectura/recalculo PASS, sin regenerar la huella al modificar el modelo.

ReglasProveedorIndices valida destino índice, IDs, propiedad lógica y clave con las fuentes originales; prohíbe proveedor anidado. ResolvedorReglasPerfilImportacion permanece puro: informa FUENTE_NO_RESUELTA si no hubo coordinación. CoordinadorProveedorIndices consulta una vez por proveedor/version/clave dentro del ensayo, reutiliza transformaciones existentes con copias y conserva FuenteTipo=PROVEEDOR. No guarda respuestas entre solicitudes. Dependencia de expediente sin valor permanece diferida sin ejecutar consulta.

ServicioPerfilImportacion integra autorización/referencia en guardar/publicar/seleccionar y validación DA después del ensayo. Composición ASMX registra alias RA y DA mediante conexiones servidor existentes; otros alias se rechazan hasta que se incorporen a composición administrada. No existe selección de prefijo arbitrario ni lectura de connectionStrings solicitada por cliente. No se cambia la consulta de las cuatro fuentes anteriores ni se exige catálogo de proveedores a perfiles que no usan esa fuente.

Pruebas nuevas: JSON cerrado y round-trip de nueva regla; huella antigua fija; consulta compartida por dos campos y dos ejemplos; nuevo ensayo vuelve a leer; errores por campo; reglas originales no mutadas; clave diferida sin consulta; rechazo de propiedad inexistente/destino no índice/anidamiento; servicio guarda solo configuración válida sin leer negocio y valida los valores resueltos contra catálogo. Harness .NET Framework PASS y build completo PASS nuevamente.

Pendiente: verificación integrada del repositorio y versiones reales; catálogo de proveedores HTTP, editor, E2E y evidencia final. No se copió DLL a bin. SQL permanece sin aplicar y su ejecución corresponde exclusivamente al usuario, por confirmación expresa. No se ejecutaron comandos DDL ni escrituras de BD. Las pruebas descritas usan dobles explícitos.
## Tercera etapa — catálogo e interfaz (2026-10-22)

El usuario confirmó aplicación manual de SQL 04. SELECT de esquema y conteos confirma tablas InnoDB/PK incrementales, proveedor=0/version=0. Lectura real de Repository con filtro puntual inexistente devuelve colección vacía; Obtener rechaza versión inexistente. Evidencia doc110-amp01-esquema.json. No acredita lectura positiva de proveedor registrado, aislamiento multiempresa real ni consulta de datos de negocio. No se ejecutó DDL ni INSERT/UPDATE/DELETE.

ConsultarProveedores: DTO cerrado pagina/tamanoPagina/proveedorId/versionId, listado por contexto autorizado con filtro opcional exacto de ambas referencias, páginas máximas de 50, proyección de nombre/IDs/número/tipo de clave/propiedades lógicas. No entrega conexión, tabla ni columnas físicas. Usa Ejecutar/guard existente. Listado excluye desactivados; una referencia guardada ausente se conserva con diagnóstico en el editor, no se reemplaza.

Editor: origen Proveedor de datos solo para Índice DA; proveedor/version, propiedad y controles de extracción de clave. Respuestas acotadas, páginas y reintento contextual; carga puntual de versión fuera de página. Guardado conserva estructura de clave y etapas. Resultado del ensayo ya muestra códigos por campo para ausencia/ambigüedad/error y valores resueltos. Tablas vacías muestran ausencia de proveedores habilitados.

Comandos ejecutados: node --test tests/doc110-perfil-importacion-editor.test.cjs tests/doc110-perfil-importacion-client.test.cjs tests/doc110-perfil-importacion-behavior.test.cjs (16 PASS); MSBuild aislado con parámetros anteriores (PASS); npx.cmd playwright test tests/doc110-perfil-guided.spec.cjs --config=playwright.config.cjs desde tools/e2e (1 PASS). El recorrido Chromium reutiliza jQuery/editor y transporte simulado: seleccionar proveedor/version/propiedad, definir segmento y extensión, guardar y recuperar. No es aceptación ASMX/BD autenticada. Suite anónima ampliada con ConsultarProveedores, pendiente ejecución contra despliegue actualizado.

No se desplegó DLL ni se registró fuente real. Corrección posterior del usuario: el proveedor debe crearse/configurarse desde interfaz; no preparar registro manual por SQL. Los datos de un origen autorizado se necesitan solo para la aceptación real del formulario. No registrar fuentes inventadas para conseguir PASS. Cierre de 14.17 y evidencia global 14.19 pendientes.
## Corrección arquitectónica vigente — administración visual

El usuario rechazó el registro de proveedores por script. Las pruebas anteriores acreditan adaptador, selección de proveedor y ensayo, no su creación/edición/publicación desde UI. Contrato 10 y D-09/RQ-12 sustituyen esa limitación; tareas 14.20–14.31 abiertas. La base SQL 04 instalada requiere revisión del ciclo y migración incremental posterior; no se vuelve a crear. Ningún estado PASS anterior cierra esta nueva administración. Corrección documental, sin código ni SQL ejecutado.

## Ciclo administrativo — avance parcial

SQL 05 incremental preparado (sin ejecutar). Lectores de perfiles filtran PUBLICADA; coordinador rechaza BORRADOR aunque esté activo. Prueba con ID conocido verifica rechazo antes de consulta al origen, PASS en harness. Build aislado PASS. No se ejecutó lectura del esquema nuevo porque su migración no fue aplicada por el usuario. Administración UI, mutaciones, política de fuentes y permisos pendientes; no desplegado. Se solicitó decisión explícita de permiso y alcance de fuentes antes de exponer metadatos.

SQL 05 aplicado manualmente por el usuario y verificado el 2026-10-22 mediante SELECT. Evidencia: Validacion/doc110-amp01-ciclo-esquema.json bajo raíz funcional. PASS: diez columnas del ciclo (tipo, nulabilidad y defaults; revision unsigned y hash binary(32)), índice proveedor_id/estado/numero, tablas InnoDB e IDs incrementales. Catálogo sin proveedores/versiones; Repository con filtro publicado devuelve vacío y rechaza referencia inexistente. No hubo DDL ni escrituras del agente. Supera el pendiente de aplicación del SQL 05; no acredita administración visual, publicaciones reales ni permisos. La decisión de 14.20 sobre permiso y alcance de fuentes continúa pendiente.
## Permiso y política de fuentes

Implementados gate específico y política de fuentes servidor, con endpoint de metadatos protegido. Pruebas nuevas dentro del harness existente PASS: misma fuente/empresa permite, columna ajena y otra empresa deniegan, política ausente deniega, permiso de perfil no permite descubrir, tabla ajena rechazada antes de abrir conexión, identificador manipulado rechazado. Build aislado PASS. SQL 06 entregado sin ejecutar; Web.config y asignaciones no modificados. Suite anónima ampliada con nuevo método pero no ejecutada contra despliegue actualizado. Administración visual y transacciones de ciclo aún pendientes.

### Administración visual — implementación local 2026-10-22

Se incorporaron modelos administrativos, MySqlAdministracionProveedorRepository, ServicioAdministracionProveedor, ValidadorAdministracionProveedor y nueve operaciones ASMX. El repositorio bloquea cabecera antes de versión y comprueba ambas revisiones dentro de la transacción. Publicación conserva huella SHA-256 del contrato guardado; validar estructura y ensayar son lecturas separadas. Las versiones publicadas no se editan. La concurrencia real entre conexiones queda pendiente de aceptación integrada.

ProveedorAdminEditor se integra en Producción Documental bajo permiso adicional. Permite alta, edición guiada, prueba por clave, publicación, nueva versión, historial y disponibilidad. Descarta respuestas antiguas incluso en secuencias RA→DA→RA, protege cambios pendientes y bloquea controles durante solicitudes. No admite SQL ni credenciales en el formulario. Reintentar opciones solo aparece ante error.

Verificación local: compilación aislada PASS; 20 pruebas Node (incluye harness .NET y cuatro pruebas nuevas de UI) PASS. Harness ampliado posteriormente con AdministracionProveedorBehaviorTests: PASS; comprueba permiso antes de repositorio, revisión obsoleta, publicación inmutable, validación/ensayo sin escritura y uso del contrato guardado frente al enviado por cliente. Dobles de repositorio y consulta: no acredita transacciones MySQL reales.

Instalación pendiente: SQL 06-permiso-proveedores-mysql51.sql aplicado manualmente por el usuario en RA, asignación del permiso mediante administración de usuarios y autorización de fuentes en la política del servidor. SQL 04/05 ya confirmados por el usuario; no repetirlos. No se ejecutó DDL, no se registraron proveedores por SQL, no se cambió Web.config ni SII y no se desplegó DLL. La aceptación autenticada, concurrencia real y entrega integral 14.17/14.30/14.31 siguen pendientes.

Administraci?n visual de proveedores (2026-10-22): `npx.cmd playwright test tests/doc110-perfil-guided.spec.cjs --config=playwright.config.cjs` desde tools/e2e: **2 PASS**. Incluye regresi?n del editor de perfiles y alta/configuraci?n/validaci?n/ensayo/publicaci?n/nueva versi?n de proveedor desde UI. Transporte simulado; no acredita ASMX, BD, permisos reales ni concurrencia. Se corrigi? el nombre accesible de selectores detectado en esta ejecuci?n. Las tareas de aceptaci?n real siguen abiertas; sin despliegue ni ejecuci?n de DDL.

SQL 06 confirmado aplicado por el usuario y contrastado mediante SELECT el 2026-10-22T19:24:14Z: ADMINISTRAR_PROVEEDORES_IMPORTACION existe como tinyint unsigned NOT NULL DEFAULT 0. Evidencia: Validacion/doc110-amp01-permiso-esquema.json. Cero filas tienen simult?neamente ADMINISTRAR_PROVEEDORES_IMPORTACION=1 y ADMINISTRAR_PERFILES_IMPORTACION=1. Web.config no tiene pol?tica Doc110.Proveedores.FuentesAutorizadas configurada. Supera el pendiente de aplicaci?n del script; no acredita asignaci?n del permiso ni autorizaci?n de fuentes. La aceptaci?n real sigue pendiente de esas configuraciones y del c?digo disponible en el ambiente. No se ejecutaron DDL, asignaciones ni despliegue.


## AMP-01: fuente exclusiva de empresa por conexion (2026-10-22)

Decision del usuario: los modulos pertenecen a una sola empresa y su propia base; DA.imp_02_egresos no mezcla organizaciones. Conservar tabla y datos sin agregar EMPRESA. La clave es CONSECUTIVO (texto); CODIGOTIPO, TIPO, CONSECUTIVO, FECHA, NIT y BENEFICIARIO son varchar(250). No asumir unicidad: LIMIT 2 y resultado AMBIGUO permanecen.

Se incorpora ExclusivaEmpresa (booleano, default false) en FuenteProveedorAutorizada, exclusivamente en la politica del servidor. Su identidad sigue siendo AmbitoRef + ConexionRef + Tabla. Solo una entrada autorizada para el ambito actual con ExclusivaEmpresa=true permite ColumnaEmpresa vacia. El cliente no puede declarar esa autorizacion: el contrato del proveedor no incorpora el booleano. La conexion sigue resuelta en servidor desde la sesion RA/DA. La administracion valida esta condicion al validar/publicar y el adaptador la comprueba antes de abrir conexiones. Si existe columna, conserva el filtro parametrizado por ambito, incluso en fuente exclusiva.

Sin migracion SQL: empresa_columna VARCHAR(64) NOT NULL admite cadena vacia; el repositorio ya la guarda y recupera. No cambiar scripts 04/05/06, ni tabla fuente, ni SII. Publicaciones existentes conservan contrato y huella. Una politica sin el atributo mantiene la exigencia anterior de columna.

Configuracion propuesta para la fuente (pendiente del AmbitoRef real de la empresa; no copiar el marcador como ID):
```json
[{"AmbitoRef":"<ID_EMPRESA_REAL>","ConexionRef":"DA","Tabla":"imp_02_egresos","ExclusivaEmpresa":true,"Columnas":["CODIGOTIPO","TIPO","CONSECUTIVO","FECHA","NIT","BENEFICIARIO"]}]
```
La configuracion corresponde a appSettings Doc110.Proveedores.FuentesAutorizadas. No se modifico Web.config ni se creo un proveedor en BD: el alta funcional se realiza por interfaz una vez autorizado el origen y disponible el permiso.

FECHA se obtiene como texto, por ejemplo 23/01/2006 0:00:00. El formato identificado es dd/MM/yyyy H:mm:ss; su conversion a fecha requiere ampliar el conjunto permitido de formatos del resolvedor y del editor con pruebas. Esta entrega de aislamiento no afirma haber implementado esa conversion.

Verificacion: build aislado PASS; node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc110-proveedor-admin-editor.test.cjs: 5 PASS. Casos nuevos: autorizacion exclusiva explicita, default denegado, ambito ajeno, copia de politica sin mutacion, consulta sin parametro empresa, ceros iniciales, duplicados ambiguos y filtro anterior preservado. Pruebas con dobles; sin DDL, consultas de negocio reales ni despliegue.


Evidencia vigente 2026-10-23: ver Validacion/doc110-amp01-evidencia-consolidada-20261023.md. Build PASS, 20 pruebas locales PASS y 2 Chromium con transporte simulado PASS. SELECT DA: 25153 filas, 2 claves vacias y 738 grupos CONSECUTIVO duplicados. No se modifica la clave ni se elige una fila arbitraria: requiere decision sobre ambiguedad. Permisos conjuntos=0 y politica de fuentes sin configurar. Aceptacion real pendiente, sin escrituras ni despliegue.
