<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14 -->
## Purpose

Recibir libros Excel bajo una referencia privada identificada, validar el archivo completo y su correspondencia con esquema y claves del destino autorizado, y persistir una preparación verificable sin modificar datos de negocio.

## ADDED Requirements

### Requirement: RQ-01 Autoridad y transporte propios

El sistema SHALL cumplir D-01: Verificar en cada operación usuario activo, empresa, permiso exacto ADMINISTRAR_CARGAS_IMPORTACION=1, DA/módulo vigentes y CSRF propio. Extender guarda multipart sin debilitar POST JSON ni exigir permisos de perfiles/fuentes.

#### Scenario: Permiso propio y revocación

- **WHEN** un usuario tiene solo el permiso de cargas o pierde ese permiso durante el flujo
- **THEN** la operación usa autoridad RA vigente; funciona sin permiso de perfiles/fuentes o se deniega antes de abrir archivo/temporales

#### Scenario: Multipart forjado

- **WHEN** el origen/token/empresa es inválido, hay varios archivos o campos duplicados
- **THEN** la solicitud se rechaza sin publicar archivo ni insertar ejecución y la guarda JSON conserva su límite262144bytes

### Requirement: RQ-02 Archivo privado e inmutable

El sistema SHALL cumplir D-02: Contar bytes y SHA256 durante copia acotada a archivo .part exclusivo, validar XLS/XLSX real y publicar completo sin sobrescribir. Referencias/rutas generadas por servidor bajo almacenamiento protegido; RECIBIDA exige archivo completo verificable.

#### Scenario: Archivo completo verificado

- **WHEN** llega un único XLS/XLSX dentro del límite
- **THEN** se cuentan bytes/hash reales, se publica referencia privada inmutable y solo entonces se registra RECIBIDA

#### Scenario: Fallo durante recepción

- **WHEN** falla la copia/validación antes de publicar o falla el registro tras publicar
- **THEN** no existe RECIBIDA falsa; parcial propio puede descartarse tras fallo conocido, completo con resultado incierto se conserva para consulta

#### Scenario: Formato disfrazado y ruta hostil

- **WHEN** el nombre incluye traversal o extensión permitida con contenido ajeno
- **THEN** no se usa el nombre como ruta y el formato real inválido se rechaza

### Requirement: RQ-03 ID previo y recuperación del acuse

El sistema SHALL cumplir D-03: Emitir ID de carga UUID y referencia protegida antes del multipart, sin insertar metadatos ficticios. Repetición explícita del mismo ID/contenido reutiliza recepción propia; conflicto de contenido o escritor activo no se reintenta automáticamente.

#### Scenario: Acuse perdido

- **WHEN** se pierde la respuesta del upload después de terminar la recepción
- **THEN** el cliente conoce previamente el ID y consulta esa misma carga propia sin crear otra ni repetir por timeout

#### Scenario: Replay y escritor activo

- **WHEN** dos sesiones presentan el mismo ID con contenido igual o distinto
- **THEN** un escritor activo devuelve OPERACION_EN_CURSO; completo propio idéntico puede devolverse como repetición y contenido distinto produce conflicto

### Requirement: RQ-04 Destino y aislamiento de servidor

El sistema SHALL cumplir D-04: Resolver DA por recursos del módulo sin otra conexión/conector. Para tabla existente exigir política empresarial explícita de servidor; CONEXION exclusiva o COLUMNA con empresa+clave. No inferir exclusividad por prefijo/nombre de columna ni autoridad desde Excel.

#### Scenario: Política de tabla compartida

- **WHEN** la tabla autorizada tiene política COLUMNA
- **THEN** la columna empresarial es requerida, mapeada y coincide con empresa de sesión; temporales/consultas usan empresa+clave y no exponen otras empresas

#### Scenario: Política ausente o contradictoria

- **WHEN** un destino existente no tiene aislamiento de servidor verificable
- **THEN** se informa POLITICA_DESTINO_NO_DEFINIDA o incompatibilidad sin asumir exclusividad ni activar fuentes automáticamente

#### Scenario: Nuevo destino

- **WHEN** CREAR solicita un nombre imp_02_ ausente en DA autorizada
- **THEN** se prepara intención propia con estructura proyectada y ámbito empresarial; no se crea tabla ni se registra proveedor

### Requirement: RQ-05 Lectura completa y límites heredados

El sistema SHALL cumplir D-05: Usar ServicioLecturaExcel con límites positivos del servidor,10000 filas por defecto y20000 configuradas por solicitud del usuario2026-11-23, con10MB decimal, conservando coordenadas/tipos. Ninguna muestra o prefijo acredita validez; errores detienen publicación PREPARADA y mantienen RECIBIDA.

#### Scenario: Fronteras de archivo y filas

- **WHEN** el archivo tiene exactamente el máximo o lo excede en un byte/fila
- **THEN** se aceptan máximos inclusivos y se rechaza exceso sin publicar preparación parcial; límites se resuelven en servidor

#### Scenario: Lector inválido

- **WHEN** DOC123 informa tipo/charset/fórmula/archivo inválido o lectura incompleta
- **THEN** el resultado mantiene RECIBIDA con diagnóstico; no se simula comparación MySQL ni se publican filas válidas parciales

### Requirement: RQ-06 Índice único y esquema físico

El sistema SHALL cumplir D-06: En destinos existentes verificar InnoDB, tipos/longitudes/charset/nulabilidad actuales y UNIQUE de clave completa, o empresa+clave completa. Rechazar índice de prefijo/no único/con columnas adicionales; CREAR solo proyecta estructura y comprueba nombre disponible.

#### Scenario: Clave física válida

- **WHEN** el índice UNIQUE cubre íntegramente clave o los dos componentes empresariales requeridos
- **THEN** se acepta su capacidad física actual sin reducir longitudes ni rehacer índices

#### Scenario: Índice insuficiente

- **WHEN** el índice es de prefijo, no único, nullable o incluye otra columna que permite repetir la clave
- **THEN** la preparación devuelve ESQUEMA_INCOMPATIBLE con necesidad de adecuación manual

#### Scenario: Cambio concurrente de estructura

- **WHEN** cambia columna/collation/índice o aparece el nombre reservado para CREAR durante preparar
- **THEN** la relectura detecta diferencias y no publica PREPARADA

### Requirement: RQ-07 Duplicados con semántica MySQL

El sistema SHALL cumplir D-07: Comparar claves mediante temporales de la misma conexión con tipo, charset/collation y unsigned/precisión exactos. Distinguir duplicados internos y claves existentes al AGREGAR; REEMPLAZAR admite coincidencias del ámbito sustituido, CREAR exige unicidad interna.

#### Scenario: Igualdad del destino

- **WHEN** las claves difieren solo en mayúsculas/acentos/espacios o representación numérica según la columna
- **THEN** MySQL decide la igualdad con el tipo/collation de destino y se señalan todas las filas duplicadas observadas

#### Scenario: Tres modos

- **WHEN** el archivo es internamente único pero su clave ya existe en el ámbito autorizado
- **THEN** AGREGAR informa CLAVE_EXISTENTE; REEMPLAZAR permite esa coincidencia y CREAR solo procede con nombre ausente; ningún modo modifica datos

### Requirement: RQ-08 Snapshot efectivo y detección de cambios

El sistema SHALL cumplir D-08: Persistir documento efectivo canónico y hashes de archivo/configuración/esquema, revisión, hoja, modo, aislamiento y filas previstas. Mantener huella v1 de plantilla dentro del documento; relectura final de autoridad/plantilla/esquema invalida cambios observados.

#### Scenario: Editor antiguo y cambios de hoja

- **WHEN** cambia plantilla sin incrementar revisión o la intención efectiva de hoja/modo difiere
- **THEN** huella de plantilla y documento efectivo detectan el conflicto sin redefinir el hash v1 existente

#### Scenario: Archivo o política alterados

- **WHEN** el archivo retenido o esquema/aislamiento ya no coincide con su snapshot
- **THEN** la preparación se invalida y se exige una preparación explícita vigente

### Requirement: RQ-09 Estados y concurrencia por carga

El sistema SHALL cumplir D-09: Serializar recepción/preparación del mismo ID y publicar estado con transacción InnoDB y relectura propia; sin transacción abierta durante copia/parseo. RECIBIDA→PREPARADA solo tras validar. Fallo conocido no sobrescribe ganador y acuse incierto exige consultar.

#### Scenario: Publicación ganadora

- **WHEN** finalizan dos preparaciones del mismo ID o llega un estado ya confirmado por una fase posterior
- **THEN** bloqueo/relectura admiten un ganador y evitan sobrescribirlo; CONFIRMADA no se modifica

#### Scenario: Interrupción y reintento

- **WHEN** falla la validación, se pierde el acuse de commit o reinicia el proceso
- **THEN** fallo conocido deja RECIBIDA sin hashes preparados vigentes; resultado no conocido exige consulta del mismo ID, sin retomar por fila o vencimiento

### Requirement: RQ-10 Vista previa y consulta acotadas

El sistema SHALL cumplir D-10: Preparar devuelve hasta25 filas/25 incidencias con totales observados y truncamiento. ConsultarCargaExcel es lectura propia, nunca prepara/confirma. No devolver rutas, SQL, secretos o recursos ajenos; ninguna tabla destino se modifica.

#### Scenario: Muestra y totales

- **WHEN** se validan más de25 filas/incidencias o el lector supera su muestra1000
- **THEN** la respuesta acota muestras pero conserva totales observados y señala truncamiento/etapas no ejecutadas

#### Scenario: Consulta propia sin efectos

- **WHEN** se consulta ID propio, ajeno o una recepción cuyo registro es incierto
- **THEN** se devuelve estado/diagnóstico saneado propio, referencia no disponible para ajeno o resultado incierto; no se inserta, prepara ni confirma

### Requirement: RQ-11 Temporales, pool y recursos recuperables

El sistema SHALL cumplir D-11: Liberar temporales y lock y restaurar sql_mode de sesión en Finally; si limpieza no se acredita, descartar conexión del pool. No modificar modo global, cuenta o versión MySql.Data; un lock vencido por tiempo no autoriza tomar un escritor.

#### Scenario: Limpieza y reutilización

- **WHEN** termina o falla la comparación temporal
- **THEN** se liberan los recursos de esa operación y se restaura sql_mode; si falla limpieza la conexión no vuelve utilizable al pool

#### Scenario: Cancelación sin toma de escritor

- **WHEN** otro solicitante encuentra una operación todavía activa o una lease/timeout excedido
- **THEN** no borra staging ni toma posesión por reloj; se conserva evidencia y consulta estado

### Requirement: RQ-12 SQL manual y compatibilidad

El sistema SHALL cumplir D-12: Reutilizar las22 columnas de ejecución y scripts históricos sin reaplicarlos. Entregar preflight SELECT nuevo; diferencias permanentes requieren otro SQL manual y confirmación antes de SELECT posterior. No modificar EGRESOS/CONTRATOS ni consumidores SII/carpetas.

#### Scenario: Esquema ya instalado

- **WHEN** las22 columnas e índices de03 son compatibles en la DA real
- **THEN** no se reaplica instalación ni se cambia tabla de negocio; preflight de esta fase utiliza SELECT

#### Scenario: Diferencia de instalación

- **WHEN** la implementación descubre una columna/índice permanente faltante
- **THEN** se entrega SQL nuevo para ejecución manual con estado PENDIENTE_EJECUCION_MANUAL; el agente no lo aplica ni lo instala por test

### Requirement: RQ-13 Pruebas locales reutilizadas

El sistema SHALL cumplir D-13: Extender el ejecutable/harness DOC122/DOC110 y fixtures DOC123 con fallos entre filesystem/DB, rollback propio, modos, hashes, límites y concurrencia. Build aislado y evidencia distinguen dobles sintéticos de MySQL real.

#### Scenario: Fallos reproducibles

- **WHEN** los dobles provocan fallos de copia, publicación, commit, consulta o limpieza
- **THEN** las pruebas acreditan estados/recursos propios y resultado incierto sin presentarse como MySQL/E2E real

#### Scenario: Regresión y build

- **WHEN** se ejecuta el harness ampliado sobre fixtures sintéticos y build aislado
- **THEN** se conserva DOC122/DOC123/DOC110 y se registran resultados y SHA sin despliegue

### Requirement: RQ-14 E2E real de recepción y preparación

El sistema SHALL cumplir D-14: Extender tools/e2e existente para ASHX/ASMX reales, sin infraestructura paralela ni heredar autorización DOC122. Reutilizar las tablas consumidas y los controles existentes, conservando los destinos mediante SELECT y huellas completas. Acreditar con autorización específica ámbito/cuentas/archivos y ejecuciones de prueba: acceso, tres modos, dos sesiones y destino sin mutación; no exigir tablas nuevas o declarar descartables las existentes.

#### Scenario: Recorrido real autorizado

- **WHEN** se autoriza ambiente/cuentas/archivos y uso de los destinos existentes compatibles sin mutación
- **THEN** E2E reutilizada ejecuta recepción y preparación reales hasta RECIBIDA/PREPARADA y SELECT acredita destino sin mutación

#### Scenario: Concurrencia independiente y gate

- **WHEN** no hay autorización vigente o solo existe una sesión ASP.NET
- **THEN** no se ejecuta E2E real sin autorización y una sesión serializada no se presenta como dos escritores físicos
