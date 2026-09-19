<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Purpose
Permitir canje temporal, autorizado y de un solo uso de previews SII sin exponer ubicación, credenciales ni autoridad externa, con una descarga por descriptor.

## ADDED Requirements
### Requirement: RQ-01 Snapshot temporal compartido
El sistema SHALL conservar contenido autorizado en almacenamiento temporal compartido sin efectos documentales, conforme a D-01.
#### Scenario: Creación
- **WHEN** `GetPreview` valida contexto, tipo y tamaño
- **THEN** persiste un snapshot utilizable desde cualquier nodo sin cambiar estado de negocio

### Requirement: RQ-02 Descriptor opaco y ligado
El sistema SHALL emitir un descriptor aleatorio ligado a usuario, tarea, proveedor y recurso, conforme a D-02.
#### Scenario: Autoridad válida
- **WHEN** la misma sesión/tarea presenta el descriptor intacto y vigente
- **THEN** reconoce exclusivamente su snapshot
#### Scenario: Autoridad inválida
- **WHEN** está alterado o pertenece a otro contexto
- **THEN** rechaza sin revelar existencia ni campos internos

### Requirement: RQ-03 Semántica HEAD y GET
El sistema SHALL permitir HEAD no consumidor y una entrega GET, conforme a D-03.
#### Scenario: HEAD
- **WHEN** llega HEAD autorizado
- **THEN** devuelve headers sin contenido, consumo ni SII
#### Scenario: Carrera GET
- **WHEN** dos GET compiten
- **THEN** uno entrega y el otro recibe rechazo opaco

### Requirement: RQ-04 Política segura
El sistema SHALL validar tipo, tamaño y disposición desde autoridad servidor, conforme a D-04.
#### Scenario: Permitido
- **WHEN** cumple allowlist y límite
- **THEN** entrega headers exactos con no-store y nosniff
#### Scenario: Rechazado
- **WHEN** incumple tipo o tamaño
- **THEN** no transmite bytes como éxito

### Requirement: RQ-05 Descarga única
El sistema SHALL realizar máximo una descarga SII por descriptor, conforme a D-05.
#### Scenario: Recorrido completo
- **WHEN** se crea, inspecciona y consume
- **THEN** solo GetPreview descarga una vez y HEAD/GET llaman cero veces a SII

### Requirement: RQ-06 Expiración opaca
El sistema SHALL limpiar snapshots temporales y ocultar estado interno, conforme a D-06.
#### Scenario: No disponible
- **WHEN** falta, expiró, fue consumido o es ajeno
- **THEN** devuelve la misma respuesta pública sin diagnóstico interno

### Requirement: RQ-07 Gate y contexto
El sistema SHALL validar gate y contexto Workflow antes de descriptor o proveedor, conforme a D-07.
#### Scenario: Rechazo temprano
- **WHEN** gate está apagado o tarea cambió
- **THEN** rechaza antes de repositorio o llamada externa

### Requirement: RQ-08 Evidencia
El sistema SHALL conservar pruebas automatizadas y E2E saneada, conforme a D-08.
#### Scenario: Verificación
- **WHEN** se valida la entrega
- **THEN** suites, build, amenazas, conteo y E2E autorizada demuestran requisitos y gate restaurado
