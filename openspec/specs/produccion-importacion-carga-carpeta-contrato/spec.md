# Producción — Contrato documental de importación por carpetas

## Purpose

Consolidar el diseño físico y los contratos observables de la importación por carpetas a partir de DOC-110/111 integrados, para que sus consumidores reutilicen identidades, fases y recuperación sin inventar otra implementación.

## Requirements

### Requirement: RQ-01 — Inventario y propietarios verificables

La entrega documental SHALL actualizar los cuatro diseños comunes y su inventario a partir de DOC-110/111 integrados; OpenSpec referencia esos contratos y no mantiene un segundo diseño físico.

Trazabilidad: D-01, RQ-01.

#### Scenario: Revisión del contrato 01
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de alcance documental y fuente única
- **THEN** Al cotejar un componente se identifica su ruta, propietario y estado real sin crear otra capa.

#### Scenario: Componente ya implementado
- **WHEN** la propuesta nombra una capa que ya existe en DOC-110/111
- **THEN** el inventario referencia su ruta y propietario y distingue actualización documental de desarrollo nuevo

### Requirement: RQ-02 — Diccionario físico reproducible

La entrega documental SHALL contrastar perfil/version/regla y las seis tablas operativas con repositorios y SQL existente, incluida migración de identidades de DOC-110; separar referencias legacy y catálogos de tablas propias.

Trazabilidad: D-02, RQ-02.

#### Scenario: Revisión del contrato 02
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de modelo físico observado y propiedad sql
- **THEN** Cada columna y clave documentada tiene tipo, nulabilidad, ámbito, índice y fuente; un script existente no acredita instalación en otro ambiente.

#### Scenario: Referencia legacy sin clave compatible
- **WHEN** el ID referenciado pertenece a una PK compuesta o difiere en tipo o collation
- **THEN** el diccionario explica la relación y su fuente sin proponer una FK incompatible ni aplicar una migración

### Requirement: RQ-03 — Identidad y repetición sin ambigüedad

La entrega documental SHALL distinguir seleccionId/idCliente de PK servidor; describir SHA-256, canonicalización y comparación del contenido completo para plan, ruta y fase, sin unicidad global del nombre visible.

Trazabilidad: D-03, RQ-03.

#### Scenario: Revisión del contrato 03
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de identidad, huellas e idempotencia
- **THEN** Misma clave/contenido devuelve el resultado previo; cambiar contenido, revisión o recurso no autoriza duplicar el efecto.

#### Scenario: Clave repetida con otro contenido
- **WHEN** una solicitud conserva su clave pero cambia contenido, recurso o revisión
- **THEN** el contrato identifica el conflicto y conserva el resultado previo sin autorizar otro efecto

### Requirement: RQ-04 — Matriz de autoridad por fase

La entrega documental SHALL mantener separados administrar e importar; resolver identidad/empresa en servidor y permisos de nivel/gabinete/acción, CSRF de JSON/multipart y errores seguros sin rutas, SQL o tokens.

Trazabilidad: D-04, RQ-04.

#### Scenario: Revisión del contrato 04
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de autoridad efectiva y transporte
- **THEN** Un importador no necesita administración; sesión/permiso/token inválidos impiden la operación afectada antes de sus efectos.

#### Scenario: Autorización revocada
- **WHEN** la sesión, permiso de destino o token requerido deja de ser válido antes de una fase
- **THEN** la ficha identifica el rechazo anterior al efecto y el error público sin exponer rutas, SQL o tokens

### Requirement: RQ-05 — Valores resolubles sin crear durante preview

La entrega documental SHALL documentar publicación inmutable, JSON ASCII versionado y acotado, valores PREFLIGHT/TRAS_EXPEDIENTE, IDs aún inexistentes, fechas generadas y valores efectivos ya fijados.

Trazabilidad: D-05, RQ-05.

#### Scenario: Revisión del contrato 05
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de snapshot y resolución por etapas
- **THEN** Un dato que requiere expediente conserva dependencia tipada; un checkpoint fijado, incluso vacío, no se recalcula por repetición.

#### Scenario: Valor todavía inexistente
- **WHEN** el preview necesita un ID futuro o encuentra un checkpoint ya fijado con cadena vacía
- **THEN** el primero conserva una dependencia tipada sin crear el expediente y el segundo permanece fijado sin recalcularse

### Requirement: RQ-06 — Cliente y servidor usan el mismo contrato

La entrega documental SHALL inventariar la fachada ASMX y el handler multipart actuales con entradas/salidas, propietario, revisión, errores y efectos. Documentar equivalencias de la guía sin inventar endpoints de adaptador.

Trazabilidad: D-06, RQ-06.

#### Scenario: Revisión del contrato 06
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de catálogo único de contratos públicos e internos
- **THEN** Cada operación tiene DTO y observable existentes; detención equivale a SolicitarDetencion y no promete cancelación destructiva o un CancelarLote inexistente.

#### Scenario: Nombre conceptual sin endpoint propio
- **WHEN** la guía solicita cancelar un lote pero la fachada expone SolicitarDetencion
- **THEN** se documenta la equivalencia y sus efectos permitidos sin inventar CancelarLote ni prometer rollback destructivo

### Requirement: RQ-07 — Transiciones y totales consistentes

La entrega documental SHALL separar PLAN, RECEPCION, EFECTO, RAIZ, INTENTO y LOTE; contrastar transiciones cerradas y agregados con CoordinadorImportacionCarpetas sin equiparar bytes recibidos con incorporación.

Trazabilidad: D-07, RQ-07.

#### Scenario: Revisión del contrato 07
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de estados por dimensión y agregación
- **THEN** Una transferencia recibida no confirma un documento y un rechazado sigue en los totales originales.

#### Scenario: Recepción sin incorporación
- **WHEN** un archivo está recibido pero su efecto documental sigue pendiente
- **THEN** la tabla mantiene separadas RECEPCION y EFECTO y no lo cuenta como documento incorporado

### Requirement: RQ-08 — Tres políticas con una coordinación común

La entrega documental SHALL conservar COMPLETO_OBLIGATORIO con ámbito EXPEDIENTE o LOTE y PARCIAL_PERMITIDO/NO_APLICA; describir CAS/revisión/reclamaciones y barreras del mismo coordinador.

Trazabilidad: D-08, RQ-08.

#### Scenario: Revisión del contrato 08
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de barreras y exclusión del coordinador existente
- **THEN** EXPEDIENTE avanza la raíz lista, LOTE bloquea ambas si falta un archivo y PARCIAL avanza elegibles conservando rechazados.

#### Scenario: Una raíz lista y otra inválida
- **WHEN** el recorrido en papel presenta dos raíces con distinta elegibilidad
- **THEN** EXPEDIENTE permite la raíz lista, LOTE conserva la barrera común y PARCIAL permite elegibles manteniendo los rechazados en los totales

### Requirement: RQ-09 — Adaptadores caracterizados sin cambiar escritores compartidos

La entrega documental SHALL caracterizar creación y almacenamiento actuales: contexto temporal delimitado, copia consumible, nombre original, Archivo Gestión por nombre y prueba de IDs/relaciones/índices; preservar función legacy y SII.

Trazabilidad: D-09, RQ-09.

#### Scenario: Revisión del contrato 09
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de frontera legacy y efectos verificables
- **THEN** Almacenamiento ya incorporado no se incorpora otra vez; NOMBRE respeta visibilidad; código de formato con signo no se confunde con ID de recurso.

#### Scenario: Nombre no visible y código nativo con signo
- **WHEN** el adaptador usa un nombre oculto por el perfil o devuelve un código de formato negativo
- **THEN** la caracterización respeta la visibilidad y distingue código nativo de ID sin modificar el almacenamiento compartido

### Requirement: RQ-10 — Recuperación sin inventar causalidad

La entrega documental SHALL documentar incertidumbre entre SQL/disco/XML/respuesta, evidencia positiva/negativa y cierre de reserva sin inicio; un timeout o antigüedad no habilita replay.

Trazabilidad: D-10, RQ-10.

#### Scenario: Revisión del contrato 10
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de ventanas de fallo y reconciliación
- **THEN** Un resultado incierto conserva recursos; solo evidencia válida permite reconciliar o cerrar la reserva nunca iniciada bajo el mismo bloqueo de inicio.

#### Scenario: Caída después del escritor externo
- **WHEN** se pierde el checkpoint o la respuesta después de un posible efecto externo
- **THEN** la matriz conserva incertidumbre hasta evidencia válida y no usa timeout o lease como prueba de ausencia

### Requirement: RQ-11 — Límites y exclusiones explícitos

La entrega documental SHALL registrar límites derivados del perfil/transporte/JSON/sesión, disponibilidad de publicaciones y condiciones operativas de volumen/navegador/retención con responsable y efecto del bloqueo.

Trazabilidad: D-11, RQ-11.

#### Scenario: Revisión del contrato 11
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de capacidad, disponibilidad y retención
- **THEN** No se convierten 500 archivos/25 MB del prototipo en promesa; sin política de retención se conservan originales.

#### Scenario: Capacidad o retención sin decisión acreditada
- **WHEN** no existe evidencia suficiente de volumen, navegador o política de limpieza
- **THEN** se registra responsable y capacidad afectada, se conservan temporales sin política válida y no se publican cifras del prototipo como garantía

### Requirement: RQ-12 — Revisión documental reproducible

La entrega documental SHALL recorrer contratos en papel con dos raíces, barreras, diferidos, revisión y fallos; separar evidencia heredada de DOC-111 de la revisión DOC-112 y validar enlaces/D-RQ-tareas.

Trazabilidad: D-12, RQ-12.

#### Scenario: Revisión del contrato 12
- **WHEN** se contrasta el entregable documental con las fuentes vigentes de qa documental y trazabilidad de cierre
- **THEN** DOC-112 no ejecuta E2E/MSBuild/SQL ni acredita otra implementación al revisar contratos ya existentes.

#### Scenario: Evidencia heredada
- **WHEN** la revisión cita un PASS de DOC-111
- **THEN** el acta conserva su procedencia y alcance y registra E2E/MSBuild como NO_APLICA para DOC-112 sin atribuir una ejecución nueva
