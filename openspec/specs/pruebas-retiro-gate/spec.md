# Pruebas, gate y transición legacy Specification

## Purpose

Demostrar de forma reproducible que la importación moderna opera bajo autorización completa, conserva el fallback legacy y puede revertirse sin ampliar el alcance de pruebas ni eliminar prematuramente controles existentes.

## Requirements

### Requirement: Validación frontend local
El sistema SHALL ofrecer una validación determinista, no autenticada y sin red para los contratos frontend de Importar Servicio Web. **Origen: D-01, RQ-01.**

#### Scenario: Contrato roto
- **WHEN** una suite focal detecta una regresión de arquitectura, gate, legacy o almacenamiento
- **THEN** el validador termina con código distinto de cero e identifica la suite fallida

### Requirement: Gate global para sesiones Workflow válidas
Cada endpoint moderno SHALL comprobar la bandera habilitada y una sesión Workflow válida antes de resolver dependencias o producir efectos. No SHALL aplicar listas adicionales de usuarios o grupos a una funcionalidad transversal. **Origen: D-02, RQ-02.**

#### Scenario: Acceso directo fuera del alcance
- **WHEN** un consumidor invoca directamente un endpoint con gate apagado o una sesión Workflow inválida
- **THEN** recibe un rechazo seguro y no se invoca proveedor, intención, almacenamiento ni reconciliación

#### Scenario: Usuario autenticado con gate activo
- **WHEN** cualquier usuario con una sesión Workflow válida accede al módulo y el gate está activo
- **THEN** la interfaz y los endpoints modernos están disponibles sin exigir pertenencia a listas de usuarios o grupos

### Requirement: Alternancia visual reversible
La vista SHALL mantener una sola entrada y un solo handler efectivo, ocultando inicialmente el árbol visual legacy bajo gate activo y preservándolo como fallback bajo gate apagado. **Origen: D-03, RQ-03.**

#### Scenario: Gate activo
- **WHEN** la página se representa con la experiencia moderna autorizada
- **THEN** solo la entrada moderna queda disponible y los controles legacy no originan una segunda ejecución

#### Scenario: Gate apagado
- **WHEN** la página se representa con el gate desactivado
- **THEN** el recorrido legacy conserva sus controles, postbacks y handlers vigentes

### Requirement: Ejecución única y proyección completa
La interfaz SHALL realizar una sola llamada `ExecuteImportIntent` por intención y proyectar una vez todos los documentos confirmados, sin progreso ficticio ni duplicados. **Origen: D-04, RQ-04.**

#### Scenario: Intención con múltiples documentos
- **WHEN** la ejecución permanece pendiente y luego retorna resultados confirmados
- **THEN** la UI muestra espera global, no inventa avance por elemento y actualiza cada documento una sola vez

### Requirement: Evidencia E2E gobernada
La validación E2E SHALL reutilizar exclusivamente la plataforma existente, requerir autorizaciones explícitas y producir evidencia saneada. **Origen: D-05, RQ-05.**

#### Scenario: Corrida autorizada finaliza o falla
- **WHEN** una prueba autenticada termina por cualquier ruta
- **THEN** el gate queda en `false`, usuarios y grupos quedan vacíos y los controles de consulta son solo `SELECT`

#### Scenario: Autorización ausente
- **WHEN** no existe autorización explícita para ambiente, cuenta o mutación aplicable
- **THEN** no se ejecuta la corrida real ni se sustituye con evidencia simulada

### Requirement: Inventario previo al retiro legacy
El sistema SHALL conservar un inventario verificable de referencias legacy y SHALL NOT eliminar controles o handlers dentro de DOC-79. **Origen: D-06, RQ-06.**

#### Scenario: Control todavía referenciado
- **WHEN** un control, postback, ASMX o handler conserva referencias o carece de evidencia suficiente
- **THEN** se clasifica como no removible y permanece disponible para rollback

### Requirement: Documentación canónica y saneada
La entrega SHALL documentarse exclusivamente en la ruta canónica DOC-79 bajo `Doc/Actualizacion`, sin secretos ni una carpeta `docs/` paralela. **Origen: D-07, RQ-07.**

#### Scenario: Registro de evidencia
- **WHEN** se registra una validación local o autorizada
- **THEN** el paquete consigna comando, resultado, limitaciones y restauración sin credenciales, cookies, tokens ni conexiones
