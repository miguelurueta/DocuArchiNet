<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Purpose
Permitir conocer antes de crear una intención el destino lógico, requisitos y efectos previstos, con autoridad local y sin mutaciones ni llamadas SII.

## ADDED Requirements
### Requirement: RQ-01 Plan por elemento
El sistema SHALL devolver un plan por `ClientItemId` y un indicador agregado `Executable`, conforme a D-01.
#### Scenario: Selección múltiple
- **WHEN** recibe varios elementos válidos
- **THEN** devuelve exactamente un plan por elemento y `Executable=true`

### Requirement: RQ-02 Efectos y destino lógico
El sistema SHALL describir tarea, tipología, modo, necesidad de expediente y efectos previstos sin identidad física, conforme a D-02.
#### Scenario: Plan público
- **WHEN** construye el plan
- **THEN** informa almacenamiento, expediente, vínculo, caché e índices sin `ExpedientId`, gabinete, tabla, SQL o ruta

### Requirement: RQ-03 Configuración autoritativa
El sistema SHALL resolver configuración desde fuentes locales confiables y parametrizadas, conforme a D-03.
#### Scenario: Configuración ausente
- **WHEN** falta configuración necesaria
- **THEN** `Executable=false` y un requisito seguro identifica la indisponibilidad

### Requirement: RQ-04 Pureza del preflight
El sistema SHALL ejecutar preflight sin mutaciones ni llamadas SII, conforme a D-04.
#### Scenario: Ausencia de efectos
- **WHEN** termina con éxito o rechazo
- **THEN** no cambia estados documentales y no solicita token, sello, preview o descarga

### Requirement: RQ-05 Huella determinista
El sistema SHALL cubrir en la huella todo contexto, selección y configuración usados, conforme a D-05.
#### Scenario: Repetición estable
- **WHEN** repite igual autoridad en distinto orden
- **THEN** devuelve la misma huella
#### Scenario: Autoridad modificada
- **WHEN** cambia tarea, tipología o configuración
- **THEN** devuelve otra huella o rechaza

### Requirement: RQ-06 Creación coherente
El sistema SHALL revalidar el plan antes de crear o reutilizar intención, conforme a D-06.
#### Scenario: Preflight obsoleto
- **WHEN** huella, selección o requisitos no coinciden
- **THEN** rechaza antes del lock o persistencia

### Requirement: RQ-07 Diagnóstico seguro
El sistema SHALL diferenciar fallos funcionales con códigos estables, conforme a D-07.
#### Scenario: Fallo cerrado
- **WHEN** configuración, tipología, destino o servicio fallan
- **THEN** no revela IDs, gabinete, SQL, rutas, secretos ni excepción

### Requirement: RQ-08 Evidencia integral
El sistema SHALL conservar evidencia automatizada y E2E saneada, conforme a D-08.
#### Scenario: Verificación
- **WHEN** valida DOC-70
- **THEN** suites, build, SELECT y telemetría demuestran contrato, cero SII y gate restaurado
