# workflow-e2e-platform Specification

## Purpose

Proveer una plataforma E2E Workflow segura y declarativa para que los DOC reutilicen el mismo ciclo de ejecución, controles y evidencia sin duplicar infraestructura ni exponer secretos.

## Requirements

### Requirement: Escenarios y perfiles E2E declarativos

El sistema SHALL ejecutar únicamente escenarios E2E registrados y perfiles no sensibles que declaren ambiente, transporte, recurso, presupuesto y controles previamente aprobados.

#### Scenario: Perfil y escenario registrados

- **WHEN** una persona invoca un escenario registrado con un perfil válido
- **THEN** la plataforma resuelve su etapa, recurso, controles y expectativas sin que el perfil elija comandos ni scripts arbitrarios

#### Scenario: Perfil con contenido prohibido

- **WHEN** un perfil contiene secretos, SQL arbitrario, URL/cadena de conexión de base de datos o un campo no permitido
- **THEN** la plataforma rechaza la corrida antes de abrir navegador, conexión ODBC o sesión Workflow y no muestra el valor rechazado

### Requirement: Autorización y recursos por etapa

El sistema SHALL exigir las autorizaciones explícitas y el recurso apropiado para cada etapa E2E, con reserva y consumo para operaciones mutantes.

#### Scenario: Etapa protegida autorizada

- **WHEN** ambiente, cuenta, autorizaciones y recurso requeridos están aprobados para una etapa
- **THEN** la plataforma permite iniciar únicamente esa etapa con el recurso declarado

#### Scenario: Autorización o recurso ausente

- **WHEN** falta una autorización, una tarea requerida o un recurso descartable para una etapa protegida
- **THEN** la plataforma falla cerrada sin iniciar la etapa ni consumir recursos

### Requirement: Ciclo de ejecución seguro y reutilizable

El sistema SHALL reutilizar una sesión Workflow, controles de solo lectura y cierre común para las etapas anónima, lectura, preview, ejecución, concurrencia y bloqueo UI.

#### Scenario: Lectura o preview no mutante

- **WHEN** una etapa de lectura o preview se ejecuta sobre un recurso autorizado
- **THEN** la plataforma compara los controles antes y después y confirma que estado y auditoría no cambiaron

#### Scenario: Falla durante una etapa

- **WHEN** una etapa falla o supera su presupuesto
- **THEN** la plataforma bloquea las etapas posteriores y ejecuta el cierre de sesión, recursos y controles de integridad

### Requirement: Secretos, TLS y evidencia saneados

El sistema SHALL capturar secretos únicamente de forma efímera en TTY, validar TLS por defecto y producir salida y evidencia sin datos sensibles.

#### Scenario: Certificado local autofirmado autorizado

- **WHEN** una corrida local cuenta con autorización explícita para su certificado autofirmado
- **THEN** la excepción TLS se limita al proceso efímero de esa corrida y se aplica de forma consistente al navegador y al cliente HTTP utilizado por la etapa

#### Scenario: Salida, artefacto o cierre de corrida

- **WHEN** una corrida termina con éxito o error
- **THEN** secretos, cookies, tokens, cadenas de conexión, cuerpos HTTP y datos de negocio no se imprimen ni permanecen en evidencia o directorios temporales

### Requirement: Adaptadores de DOC y piloto de Notas compatibles

El sistema SHALL permitir que cada DOC aporte solo sus operaciones, payloads permitidos y expectativas, conservando los comandos E2E existentes durante la migración piloto de Notas.

#### Scenario: Adaptador de un nuevo DOC

- **WHEN** un DOC incorpora un escenario E2E mediante la plataforma
- **THEN** declara sus operaciones y expectativas sin implementar login, TTY, TLS, ODBC, saneamiento ni controles de cierre propios

#### Scenario: Lectura de Notas migrada

- **WHEN** se ejecuta el escenario piloto de lectura de Notas mediante la plataforma
- **THEN** conserva el comportamiento del comando `test:notes:read`, sus controles de solo lectura y su evidencia saneada
