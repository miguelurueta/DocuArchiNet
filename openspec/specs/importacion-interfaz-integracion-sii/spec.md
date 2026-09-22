# importacion-interfaz-integracion-sii Specification

## Purpose

Preparar de forma segura una importación SII individual o múltiple antes de cualquier escritura, con catálogo, requisitos y plan confirmados por backend.

## Requirements

### Requirement: Colección única de preparación
El sistema SHALL usar el mismo contrato para uno o varios elementos. Origen: D-01, RQ-01.

#### Scenario: Individual
- **WHEN** se prepara una fila
- **THEN** la colección contiene exactamente ese elemento

#### Scenario: Múltiple
- **WHEN** se preparan varias filas
- **THEN** una colección contiene exactamente la selección explícita

### Requirement: Preflight obligatorio
El sistema SHALL impedir crear intención hasta completar datos y recibir preflight ejecutable. Origen: D-02, RQ-02.

#### Scenario: Datos incompletos
- **WHEN** falta tipología o `Executable` no es verdadero
- **THEN** confirmar queda deshabilitado y no se crea intención

### Requirement: Autoridad de backend
El sistema SHALL representar solo catálogo, requisitos, huella y plan confirmados por backend. Origen: D-03, RQ-03.

#### Scenario: Resumen confirmado
- **WHEN** se presenta el plan
- **THEN** muestra tarea, tipología, requisitos y efectos previstos
- **AND** no muestra `ExpedientId` ni efectos consumados

### Requirement: Intención única e idempotente
El sistema SHALL crear una sola intención para toda la colección mediante el API existente y no ejecutarla. Origen: D-04, RQ-04.

#### Scenario: Confirmación concurrente
- **WHEN** doble clic o rerender ocurren durante la creación
- **THEN** se comparte la operación y no se crea una intención por elemento

### Requirement: Popup accesible sin mutación
El sistema SHALL conservar contexto y restaurar foco al cerrar. Origen: D-05, RQ-05.

#### Scenario: Cancelar
- **WHEN** se cancela o cierra la preparación
- **THEN** no hay mutación y se restauran selección, filtros, scroll y foco

#### Scenario: Guardar todas pasivo
- **WHEN** no se inició preparación múltiple explícita
- **THEN** `Guardar todas` no inicializa contexto ni preflight

### Requirement: Fallo cerrado
El sistema SHALL bloquear cuando B03, B09, B11 o sus respuestas no sean utilizables. Origen: D-06, RQ-06.

#### Scenario: Dependencia ausente
- **WHEN** catálogo, preflight o plan no están disponibles
- **THEN** se bloquea sin fabricar datos ni filtrar detalles internos

#### Scenario: Preflight obsoleto
- **WHEN** backend responde `PREFLIGHT_STALE`
- **THEN** se exige preparar nuevamente sin reutilizar la huella
