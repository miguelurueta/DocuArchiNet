# adaptador-interfaz-integracion-sii Specification

## Purpose

Proveer consulta y presentación SII segura sobre el contrato moderno común, sin exponer detalles del proveedor al núcleo ni producir mutaciones.

## Requirements

### Requirement: RQ-01 Resolución exclusiva de INTEGRACIONSII (D-01)
El sistema SHALL registrar el adaptador únicamente para `INTEGRACIONSII`, sin fallback.

#### Scenario: Identidad canónica
- **WHEN** el registro resuelve `INTEGRACIONSII` con variaciones de mayúsculas o espacios
- **THEN** devuelve el adaptador SII y sus capacidades

#### Scenario: Proveedor no soportado
- **WHEN** la identidad está ausente o no corresponde a `INTEGRACIONSII`
- **THEN** falla cerrado sin consultar SII

### Requirement: RQ-02 Consulta por API moderna (D-02)
El sistema SHALL obtener capacidades e items exclusivamente mediante el cliente API moderno.

#### Scenario: Consulta única
- **WHEN** se inicia una consulta válida
- **THEN** invoca `ResolveCapabilities` y una sola vez `QueryItems`, sin AJAX directo ni endpoints legacy

### Requirement: RQ-03 Mapping normalizado (D-03)
El sistema SHALL proyectar identidad, título, contenido, estado, acciones y metadatos del contrato, tratando `ExternalKey` como opaco.

#### Scenario: Columnas SII
- **WHEN** un item contiene libro, inscripción, fecha, naturaleza o acto, noticia y referencia
- **THEN** muestra esos metadatos sin reconstruirlos desde `ExternalKey`

#### Scenario: Respuesta inválida
- **WHEN** falta la forma contractual mínima
- **THEN** muestra error seguro y no habilita selección

### Requirement: RQ-04 Interacción local (D-04)
El sistema SHALL aplicar filtros y paginación sobre la instantánea recibida sin repetir consultas remotas.

#### Scenario: Filtro o página
- **WHEN** cambia Todos, Disponibles, Importados, Con novedad, tipología o página
- **THEN** cambia la proyección local y el contador backend no aumenta

#### Scenario: Actualización explícita
- **WHEN** el usuario solicita actualizar
- **THEN** realiza exactamente una nueva consulta y reemplaza la instantánea

### Requirement: RQ-05 Estados y selección segura (D-05)
El sistema SHALL representar estados accesibles y permitir selección solo de elementos importables.

#### Scenario: Cardinalidad
- **WHEN** llegan cero, uno o múltiples elementos
- **THEN** presenta vacío o disponible con filas y conteo coherentes

#### Scenario: Exclusión de importados
- **WHEN** un item está importado o carece de acción de importación
- **THEN** queda deshabilitado y fuera de la selección masiva

#### Scenario: Error seguro
- **WHEN** backend informa indisponible o no autorizado
- **THEN** conserva contexto de tarea sin datos técnicos sensibles

### Requirement: RQ-06 Verificación sin efectos (D-06)
El sistema SHALL validarse con fixtures deterministas, sin red real, secretos ni mutaciones legacy.

#### Scenario: Suite aislada
- **WHEN** se prueban adaptador, mapper y lista
- **THEN** valida mapping, consulta única, filtros y selección sin contactar SII

#### Scenario: Antirregresión
- **WHEN** se inspeccionan superficies prohibidas y gate apagado
- **THEN** permanecen sin acoplamiento, logs sensibles ni cambios al recorrido legacy
