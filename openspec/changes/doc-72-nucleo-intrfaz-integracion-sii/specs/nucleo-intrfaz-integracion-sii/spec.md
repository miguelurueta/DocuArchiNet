<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## ADDED Requirements

### Requirement: RQ-01 Separación del núcleo y API (D-01)
El sistema SHALL limitar el acceso ASMX a `importar-servicio-web-api.js`.
#### Scenario: Transporte inyectable
- **WHEN** el core usa un cliente falso local
- **THEN** completa el flujo sin red y la UI no realiza AJAX directo

### Requirement: RQ-02 Registro seguro de proveedores (D-02)
El sistema SHALL resolver adaptadores por identidad canónica y capacidades explícitas, sin fallback a SII.
#### Scenario: Proveedor registrado
- **WHEN** se solicita una identidad registrada
- **THEN** devuelve su adaptador y capacidades
#### Scenario: Proveedor inválido
- **WHEN** el proveedor está ausente, no migrado o desconocido
- **THEN** devuelve el error seguro correspondiente sin consultar SII

### Requirement: RQ-03 Orquestación cerrada y ejecución única (D-03)
El sistema SHALL aplicar transiciones válidas e invocar `ExecuteImportIntent` una vez por intención.
#### Scenario: Selección múltiple
- **WHEN** se confirman uno o varios elementos
- **THEN** muestra espera global, realiza una llamada y proyecta el resultado final
#### Scenario: Transición inválida
- **WHEN** se intenta un salto no permitido
- **THEN** rechaza la transición sin ejecutar mutaciones

### Requirement: RQ-04 Coexistencia controlada por gate (D-04)
El sistema SHALL preservar legacy con gate apagado y ofrecer una entrada moderna al usuario autorizado.
#### Scenario: Gate apagado
- **WHEN** `WorkflowCentroTrabajoModernActive` es `false`
- **THEN** `btnloadservice` conserva su recorrido y el núcleo moderno permanece inerte
#### Scenario: Gate autorizado
- **WHEN** el gate está activo para el usuario o grupo permitido
- **THEN** `ctw-document-action-service` abre el modal sin duplicar entradas visibles

### Requirement: RQ-05 Modal accesible (D-05)
El sistema SHALL ofrecer diálogo nombrado, teclado, foco inicial/restaurado y anuncios de estado.
#### Scenario: Apertura y cierre
- **WHEN** se abre y cierra el modal
- **THEN** el foco entra al diálogo y vuelve al disparador
#### Scenario: Cambio de estado
- **WHEN** cambia el estado
- **THEN** `aria-live` lo anuncia sin progreso porcentual ficticio

### Requirement: RQ-06 Verificación aislada y segura (D-06)
El sistema SHALL disponer de pruebas deterministas sin credenciales, red ni activación del gate real.
#### Scenario: Suite local
- **WHEN** se ejecutan las pruebas de core, registro/UI, accesibilidad y gate
- **THEN** validan resolución, estados, ejecución única, foco y compatibilidad legacy
#### Scenario: E2E no autorizado
- **WHEN** no existe autorización explícita
- **THEN** no se activa el gate ni se ejecutan pruebas autenticadas o carga
