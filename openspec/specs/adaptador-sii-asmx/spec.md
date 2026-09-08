# adaptador-sii-asmx Specification

## Purpose

Definir la adaptación aislada de SII, el preview mediado y la compatibilidad ASMX reversible sobre los contratos comunes de importación.

## Requirements

### Requirement: RQ-01 Resolución explícita de SII (D-01)

El sistema SHALL resolver el adaptador SII únicamente para la identidad configurada `INTEGRACIONSII`.

#### Scenario: Proveedor desconocido

- **WHEN** se solicita una identidad distinta o no registrada
- **THEN** el sistema devuelve error explícito sin usar SII como fallback

### Requirement: RQ-02 Transporte compartido (D-02)

El sistema SHALL consumir SII mediante el transporte HTTP común con límites, cancelación y errores saneados.

#### Scenario: Respuesta externa inválida

- **WHEN** SII excede límites o entrega una respuesta inválida
- **THEN** el resultado contractual no expone token, payload sensible ni excepción interna

### Requirement: RQ-03 Contrato SII normalizado (D-03)

El sistema SHALL mapear capacidades e items SII a contratos comunes con una clave externa estable.

#### Scenario: Consulta exitosa

- **WHEN** SII devuelve inscripciones válidas
- **THEN** libro, registro, matrícula, acto, noticia y código de barras se proyectan sin contaminar el núcleo

### Requirement: RQ-04 Traducción legacy confinada (D-04)

El sistema SHALL confinar la traducción `YES`, `CTRL`, `CTRLRETURN` y `dato_lista` al adaptador de compatibilidad SII.

#### Scenario: Consumidor legacy

- **WHEN** un resultado estructurado debe regresar al formato histórico
- **THEN** el adaptador produce una única traducción determinista y el frontend no interpreta códigos

### Requirement: RQ-05 ASMX moderno delgado (D-05)

El sistema SHALL publicar una frontera ASMX paralela que solo valide contexto/gate, invoque servicios y serialice contratos.

#### Scenario: Invocación moderna

- **WHEN** llega una solicitud válida al ASMX moderno
- **THEN** este no ejecuta SQL, filesystem, reglas SII ni llamadas directas a `ClassAlmacenamiento`

### Requirement: RQ-06 Preview mediado seguro (D-06)

El sistema SHALL autorizar y mediar cada preview con expiración, tipo, tamaño, disposición y encabezados seguros.

#### Scenario: Preview no autorizado o vencido

- **WHEN** el contexto no autoriza el recurso o el descriptor expiró
- **THEN** se rechaza sin descargar ni revelar ruta física o secretos

### Requirement: RQ-07 Gate de servidor (D-07)

El sistema SHALL evaluar `WorkflowCentroTrabajoModernActive` en servidor antes de ejecutar rutas modernas.

#### Scenario: Gate apagado

- **WHEN** el gate está desactivado
- **THEN** responde `FEATURE_DISABLED` sin invocar proveedor, preview, orquestador o almacenamiento

### Requirement: RQ-08 Persistencia coordinada (D-08)

El sistema SHALL preparar el comando documental normalizado y delegar la persistencia exclusivamente al orquestador y puerto común.

#### Scenario: Importación SII confirmada

- **WHEN** un item autorizado se ejecuta
- **THEN** se conserva correlación e idempotencia sin modificar `AlmacenaDocumentoTareaWorkflow(...)`

### Requirement: RQ-09 Compatibilidad y evidencia (D-09)

El sistema SHALL preservar endpoints y consumidores vigentes y validar el comportamiento con fixtures sin red.

#### Scenario: Auditoría de regresión

- **WHEN** se revisan diff, pruebas y build
- **THEN** ASMX existentes, `Integracionccv`, almacenamiento y JavaScript legacy permanecen sin cambios

### Requirement: RQ-10 Resolución asíncrona aditiva (D-10)

El sistema SHALL resolver proveedores modernos mediante clientes asíncronos sin bloquear tareas ni modificar el registro síncrono existente.

#### Scenario: Resolución moderna SII

- **WHEN** la frontera moderna solicita `INTEGRACIONSII`
- **THEN** obtiene un cliente asíncrono registrado y puede usar `Await` de extremo a extremo
