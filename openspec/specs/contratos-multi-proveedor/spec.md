# contratos-multi-proveedor Specification

## Purpose

Define el contrato compartido, el contexto autorizado y el registro explícito necesarios para integrar proveedores externos sin acoplar el núcleo a sesión, transporte o conceptos específicos de SII.

## Requirements

### Requirement: Contrato compartido versionado

El sistema SHALL publicar una única familia de DTO serializables con versión `v1` para `ResolveCapabilities`, `QueryItems`, `GetPreview`, `PreflightImport`, `CreateImportIntent`, `ExecuteImportIntent`, `GetImportIntent` y `ReconcileImportIntent`. El comando/resultado documental por elemento SHALL ser auxiliar y SHALL NOT sustituir una operación canónica. Requests y responses SHALL definir nulabilidad, `schemaVersion`, `operationId`, `providerId`, `externalKey`, `taskId`, `correlationId`, códigos seguros, autorización, idempotencia, concurrencia, timeout y compatibilidad aplicables, sin propiedades exclusivas de SII. **Origen: D-01, RQ-01.**

#### Scenario: Transporte y versión publicados

- **WHEN** un consumidor inspecciona cualquiera de las ocho operaciones
- **THEN** el contrato declara HTTPS `POST`, método ASMX moderno homónimo y versión `1.0`
- **AND** distingue campos requeridos de opcionales y define sus valores nulos
- **AND** los cambios incompatibles requieren una versión posterior

#### Scenario: Los fixtures representan el contrato v1

- **WHEN** la prueba contractual carga los ocho fixtures de `Tests/Fixtures/Workflow/ImportarServicioWeb/contracts-v1/`
- **THEN** cada documento declara `schemaVersion` igual a `1.0`
- **AND** coincide con la forma request o response correspondiente
- **AND** no contiene libro, registro, matrícula, acto, noticia, código de barras ni cachés SII

#### Scenario: Una evolución incompatible no redefine v1

- **WHEN** una operación necesita eliminar, reinterpretar o volver obligatorio un campo publicado
- **THEN** el cambio SHALL introducir una versión contractual posterior
- **AND** SHALL conservar la semántica observable de `v1`

#### Scenario: Preview mediado de solo lectura

- **WHEN** se solicita `GetPreview` con `taskId`, `providerId`, `externalKey` y `operationId` cuando aplique
- **THEN** el contrato retorna descriptor temporal o stream mediado con tipo, tamaño y disposición seguros
- **AND** no expone URL externa, token, ruta física ni respuesta cruda
- **AND** no cambia tarea, estado, intención, documento, expediente, índices, caché ni auditoría funcional

#### Scenario: Idempotencia concurrencia y timeout son explícitos

- **WHEN** se inspeccionan las operaciones mutadoras futuras
- **THEN** `CreateImportIntent` exige clave de idempotencia y `ExecuteImportIntent` token de versión
- **AND** el conflicto retorna `CONCURRENCY_CONFLICT`
- **AND** el timeout se declara por proveedor o capacidad y retorna `TIMEOUT` sin fijar un valor global en DOC-50

### Requirement: Contexto autorizado e inmutable

El sistema SHALL representar usuario autenticado, grupo, login, tarea, ruta, trámite, proveedor y permisos mediante `ContextoImportacionServicio`, construido antes de cualquier operación asíncrona. Sus valores SHALL ser de solo lectura después de la construcción y las clases nuevas SHALL permanecer independientes de `HttpContext` y `Session`. **Origen: D-02, RQ-02.**

#### Scenario: Cambio posterior de sesión no altera la operación

- **GIVEN** un contexto válido capturado para una tarea, ruta, trámite y proveedor
- **WHEN** el estado de sesión o la selección de otra pestaña cambia después de capturarlo
- **THEN** el servicio y el proveedor reciben los valores originales
- **AND** ninguna clase nueva vuelve a leer la sesión para sustituirlos

#### Scenario: Contexto incompleto es rechazado

- **WHEN** falta usuario, tarea, ruta, trámite o identidad de proveedor
- **THEN** la validación retorna un resultado seguro no exitoso
- **AND** no se invoca ningún proveedor

### Requirement: Registro explícito de proveedores

El sistema SHALL resolver `IExternalImportProvider` únicamente por una identidad canónica suministrada al registro. El registro SHALL rechazar proveedores nulos, identidades vacías e identidades duplicadas sin depender de mayúsculas. Una identidad desconocida SHALL producir un resultado tipado de proveedor no soportado y SHALL NOT usar SII como fallback. **Origen: D-03, RQ-03.**

#### Scenario: Proveedor registrado se resuelve por identidad

- **GIVEN** un proveedor registrado con identidad canónica `INTEGRACIONSII`
- **WHEN** se solicita esa identidad con diferencias permitidas de mayúsculas o espacios externos
- **THEN** se retorna exactamente la instancia registrada

#### Scenario: Proveedor desconocido falla cerrado

- **WHEN** se solicita una identidad no registrada
- **THEN** el resultado contiene `PROVIDER_NOT_SUPPORTED` o el código contractual equivalente
- **AND** no se retorna ni ejecuta el proveedor SII

#### Scenario: Identidad duplicada es inválida

- **WHEN** dos proveedores declaran la misma identidad canónica
- **THEN** la construcción del registro falla de forma determinista
- **AND** no conserva silenciosamente una de las instancias

### Requirement: Revalidación común de autorización

El sistema SHALL revalidar mediante puertos inyectables el usuario autenticado, permiso vigente, tarea operable, coincidencia de ruta y trámite y habilitación del proveedor. Los identificadores del navegador SHALL ser referencias para localizar y comparar, nunca la fuente de autoridad. **Origen: D-04, RQ-04.**

#### Scenario: Permiso revocado detiene la consulta

- **GIVEN** un contexto previamente construido
- **WHEN** la fuente autorizada informa que el permiso ya no está vigente
- **THEN** el validador retorna un código seguro de autorización
- **AND** el proveedor no es invocado

#### Scenario: Tarea o ruta no coincide

- **WHEN** la tarea no está operable o su ruta o trámite no coincide con el contexto
- **THEN** la validación falla antes de consultar recursos externos
- **AND** no produce mutaciones ni revela detalles internos

### Requirement: Fachada contractual sin efectos

El sistema SHALL limitar `ServicioImportarServicioWeb` en DOC-50 a capacidades y consulta mediante contexto explícito, validación y proveedor registrado. El sistema SHALL NOT realizar preflight real, crear o ejecutar intenciones, persistir datos, escribir documentos, cambiar tareas, modificar ASMX ni invocar `AlmacenaDocumentoTareaWorkflow(...)`. **Origen: D-05, RQ-05.**

#### Scenario: Consulta contractual usa puertos y dobles

- **GIVEN** un contexto válido y un proveedor registrado de prueba
- **WHEN** se solicitan capacidades o elementos
- **THEN** la fachada valida el contexto y delega al proveedor explícito
- **AND** retorna un resultado contractual con correlación

#### Scenario: Operaciones futuras no producen efectos

- **WHEN** un consumidor inspecciona contratos de preflight, intención, ejecución o reconciliación
- **THEN** las formas v1 están disponibles para integración posterior
- **AND** DOC-50 no contiene implementaciones que ejecuten esos efectos

#### Scenario: Recorrido legacy permanece intacto

- **WHEN** se compara el diff de DOC-50
- **THEN** no existen cambios en firmas ASMX ni en `workflow/ClassAlmacenamiento.vb`
- **AND** `AlmacenaDocumentoTareaWorkflow(...)` conserva su implementación y consumidores

### Requirement: Fronteras y rutas canónicas

El sistema SHALL ubicar DTOs, modelos, interfaces y servicios únicamente en las rutas canónicas de ImportarServicioWeb y SHALL registrar cada archivo VB nuevo mediante `Compile Include` en `GestionDocumental-Docuarchi.net.vbproj`. El sistema SHALL NOT duplicar esos contratos en ASMX, `App_Code`, `ServiciosIntegracion`, `Integracionccv`, `workflow`, `Infrastructure` ni módulos Workflow ajenos. **Origen: D-06, RQ-06.**

#### Scenario: Estructura aditiva válida

- **WHEN** la prueba estructural inspecciona el repositorio y el proyecto VB
- **THEN** encuentra exactamente los seis archivos canónicos indicados por DOC-50
- **AND** cada archivo aparece una vez como `Compile Include`
- **AND** las entradas existentes permanecen sin mover ni reemplazar

#### Scenario: No hay definiciones duplicadas

- **WHEN** la prueba busca los DTOs e interfaces canónicos fuera de sus rutas permitidas
- **THEN** no encuentra una segunda definición con el mismo significado

### Requirement: Pruebas focales y evidencia segura

El sistema SHALL incluir pruebas focales CommonJS para contratos, registro y contexto, ejecutables sin sesión, red ni credenciales. La evidencia SHALL registrar comandos y resultados reales del build y pruebas disponibles. Una prueba E2E autenticada, carga o activación de gates SHALL requerir autorización explícita y no forma parte de DOC-50. **Origen: D-07, RQ-07.**

#### Scenario: Suites focales cubren reglas críticas

- **WHEN** se ejecutan las tres suites de DOC-50
- **THEN** validan fixtures y versionado contractual
- **AND** cubren proveedor conocido, desconocido y duplicado
- **AND** demuestran inmutabilidad y rechazo de contexto no autorizado

#### Scenario: Ausencia de E2E no se reporta como éxito

- **WHEN** no existe autorización para una corrida autenticada o de carga
- **THEN** la evidencia declara que no se ejecutó
- **AND** ningún gate es activado ni alterado

### Requirement: Paquete técnico único DOC-50

El sistema SHALL documentar la entrega exclusivamente en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-50-contratos-contexto-registro-multiproveedor/`, con `00-Indice.md` a `07-Metadata.md` y la carpeta `Diagramas/`. Los documentos SHALL usar enlaces relativos válidos y describir únicamente código, pruebas y limitaciones reales. **Origen: D-08, RQ-08.**

#### Scenario: Paquete completo y navegable

- **WHEN** se valida el paquete documental
- **THEN** existen los ocho documentos requeridos y el índice de diagramas
- **AND** todos los enlaces relativos resuelven dentro del paquete o hacia fuentes declaradas
- **AND** no existe un paquete paralelo `SCRUMCORE-000`

#### Scenario: Tabla de funciones demuestra no regresión

- **WHEN** se revisa `01-Arquitectura.md` o `02-FlujoIntegracion.md`
- **THEN** cada función creada o referenciada indica ruta, clase, parámetros, responsabilidad y condición nueva o existente
- **AND** `AlmacenaDocumentoTareaWorkflow(...)` figura como existente y no modificada
