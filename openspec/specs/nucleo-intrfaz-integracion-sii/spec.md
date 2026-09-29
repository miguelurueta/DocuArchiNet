# nucleo-intrfaz-integracion-sii Specification

## Purpose

Definir el núcleo frontend genérico oficial para consultar e importar documentos desde proveedores registrados, con contratos comprobables, errores seguros y una experiencia accesible.

## Requirements

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

#### Scenario: Ejecución programática de una intención
- **WHEN** un consumidor del núcleo invoca `execute(request)` una o más veces mientras la misma intención continúa en curso
- **THEN** el núcleo mantiene una espera global, realiza una sola llamada a `ExecuteImportIntent` y proyecta el resultado final

La selección y confirmación mediante controles visibles no forman parte de DOC-72; este cambio entrega la capacidad programática y su prueba aislada para que una integración posterior la consuma.

#### Scenario: Transición inválida
- **WHEN** se intenta un salto no permitido
- **THEN** rechaza la transición sin ejecutar mutaciones

### Requirement: RQ-04 Disponibilidad controlada por contexto (D-04)
El sistema SHALL ofrecer la entrada moderna oficial al usuario autorizado y evitar una segunda ejecución por controles legacy residuales.

#### Scenario: Contexto inválido
- **WHEN** la sesión no aporta un contexto Workflow válido
- **THEN** el núcleo moderno permanece inerte y no inicia una importación

#### Scenario: Contexto autorizado
- **WHEN** la sesión aporta un contexto Workflow válido
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
El sistema SHALL disponer de pruebas deterministas sin credenciales, red ni cambios de configuración de rollout.

#### Scenario: Suite local
- **WHEN** se ejecutan las pruebas de core, registro/UI, accesibilidad y contexto
- **THEN** validan resolución, estados, ejecución única, foco y compatibilidad legacy

#### Scenario: E2E no autorizado
- **WHEN** no existe autorización explícita
- **THEN** no se modifica configuración de rollout ni se ejecutan pruebas autenticadas o carga
