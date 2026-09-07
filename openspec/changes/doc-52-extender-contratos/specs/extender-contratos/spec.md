## Purpose

Define la preparación sin efectos y la identidad persistente e idempotente de importaciones modernas de uno o varios elementos externos.

## ADDED Requirements

### Requirement: RQ-01 Contrato completo de intención (D-01)
El sistema SHALL representar el contexto original, plan, requisitos, selección, estado, versión, fechas y correlación de una intención.

#### Scenario: Round-trip tipado
- **WHEN** una intención se guarda y recupera
- **THEN** conserva cabecera, requisitos y todos sus elementos sin depender de Session

### Requirement: RQ-02 Preflight sin efectos (D-02)
El sistema SHALL revalidar usuario, permiso, tarea, ruta, trámite, proveedor, selección, tipología y destino antes de crear una intención, sin realizar efectos.

#### Scenario: Preflight válido
- **WHEN** contexto y todos los elementos son válidos
- **THEN** devuelve un plan válido con requisitos y comandos y no escribe datos ni documentos

#### Scenario: Selección inválida
- **WHEN** falta un elemento, hay duplicado o no cumple un requisito
- **THEN** retorna un requisito/código seguro y no crea intención

### Requirement: RQ-03 Identidad y cardinalidad explícitas (D-03)
El sistema SHALL identificar cada elemento mediante proveedor, clave externa y tarea destino explícitos, usando el mismo contrato para uno o varios.

#### Scenario: Colección múltiple
- **WHEN** se previsualizan varios elementos
- **THEN** cada comando conserva su propio contexto sin tomar valores del primer elemento

### Requirement: RQ-04 Equivalencia canónica (D-04)
El sistema SHALL calcular una huella determinista de contexto, selección y requisitos autoritativos.

#### Scenario: Reordenamiento equivalente
- **WHEN** dos solicitudes contienen los mismos valores con elementos en distinto orden
- **THEN** producen la misma huella

#### Scenario: Contexto diferente
- **WHEN** cambia usuario, tarea, proveedor, destino o requisito
- **THEN** la huella es diferente

### Requirement: RQ-05 Reutilización o conflicto (D-05)
El sistema SHALL reutilizar una intención para la misma clave y huella y SHALL rechazar la misma clave con huella distinta.

#### Scenario: Repetición equivalente
- **WHEN** se repite una solicitud ya persistida con igual huella
- **THEN** retorna el mismo intentId con `Reused=True`

#### Scenario: Repetición incompatible
- **WHEN** se reutiliza la clave con huella distinta
- **THEN** retorna `IDEMPOTENCY_CONFLICT` sin modificar la intención existente

### Requirement: RQ-06 Unicidad concurrente (D-06)
El sistema SHALL garantizar en almacenamiento que dos solicitudes concurrentes equivalentes no creen dos intenciones ejecutables.

#### Scenario: Carrera de inserción
- **WHEN** dos procesos reservan simultáneamente la misma identidad idempotente
- **THEN** una sola fila gana y el otro proceso reutiliza o retorna conflicto según la huella

### Requirement: RQ-07 Persistencia parametrizada y reversible (D-07)
El sistema SHALL usar tablas modernas propias, transacciones locales y parámetros para todo valor funcional.

#### Scenario: Inspección SQL
- **WHEN** se revisan repositorio y DDL
- **THEN** no concatenan entradas y el paquete incluye precondiciones y rollback manual

### Requirement: RQ-08 Estado inicial auditable (D-08)
El sistema SHALL crear intención y elementos en estado `Creada` con versión, fechas UTC, operación y correlación saneadas.

#### Scenario: Creación exitosa
- **WHEN** se persiste una nueva intención
- **THEN** cabecera y elementos quedan completos en una transacción y sin payload sensible

### Requirement: RQ-09 Compatibilidad aislada (D-09)
El sistema SHALL validar DOC-52 sin red, base real, ejecución de migraciones, E2E autenticado o activación del gate.

#### Scenario: Regresión legacy
- **WHEN** se inspecciona el cambio y corre la suite focal
- **THEN** endpoints, cachés, Session y almacenamiento legacy permanecen intactos
