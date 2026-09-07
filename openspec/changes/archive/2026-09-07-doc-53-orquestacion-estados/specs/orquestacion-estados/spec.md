## Purpose

Definir ejecución secuencial, persistente y consultable de intenciones, con fallos conservadores y aislamiento del almacenamiento legacy.

## ADDED Requirements

### Requirement: RQ-01 Ejecución autorizada (D-01)
El sistema SHALL revalidar contexto e intención antes de iniciar efectos.

#### Scenario: Contexto rechazado
- **WHEN** usuario, tarea, versión o intención no son válidos
- **THEN** Execute rechaza con código seguro y no invoca puertos mutadores

### Requirement: RQ-02 Secuencia versionada (D-02)
El sistema SHALL ejecutar un item a la vez y persistir transiciones con versión optimista.

#### Scenario: Colección múltiple
- **WHEN** existen varios items
- **THEN** el siguiente inicia solo después de clasificar el anterior

#### Scenario: Versión obsoleta
- **WHEN** el token no coincide
- **THEN** retorna conflicto sin repetir efectos

### Requirement: RQ-03 Transiciones auditadas (D-03)
El sistema SHALL aceptar solo transiciones declaradas y auditarlas sin secretos.

#### Scenario: Salto inválido
- **WHEN** la fase destino no está permitida
- **THEN** se rechaza y conserva el estado

### Requirement: RQ-04 Fases confirmadas (D-04)
El sistema SHALL conservar el orden expediente, índices, almacenamiento y caché, registrando la última fase confirmada.

#### Scenario: Fallo por fase
- **WHEN** un puerto falla antes o después de una fase
- **THEN** Get devuelve la clasificación correspondiente al último efecto demostrable

### Requirement: RQ-05 Storage aislado (D-05)
El sistema SHALL invocar `AlmacenaDocumentoTareaWorkflow` solo mediante el adaptador moderno sin modificar legacy.

#### Scenario: Caracterización
- **WHEN** se almacena un comando normalizado
- **THEN** el adaptador entrega los argumentos esperados y un resultado seguro

### Requirement: RQ-06 Incertidumbre protegida (D-06)
El sistema SHALL impedir nuevos efectos cuando no pueda demostrar la persistencia anterior.

#### Scenario: Respuesta incierta
- **WHEN** una mutación termina sin confirmación
- **THEN** queda `ResultadoIncierto`, no reintentable y requiere reconciliación

### Requirement: RQ-07 Detención cooperativa (D-07)
El sistema SHALL detenerse entre unidades sin revertir confirmados ni iniciar pendientes.

#### Scenario: Detención de colección
- **WHEN** se detiene después de un item confirmado
- **THEN** conserva ese resultado y marca pendientes como detenidos

### Requirement: RQ-08 Consulta v1 (D-08)
El sistema SHALL devolver fase, versión, `persistenceKnown`, retryable, error seguro y correlación persistidos.

#### Scenario: Consulta posterior
- **WHEN** Get consulta una ejecución parcial o terminal
- **THEN** responde sin depender de Session o navegador

### Requirement: RQ-09 Compatibilidad aditiva (D-09)
El sistema SHALL coexistir sin modificar ASMX, JavaScript, almacenamiento o consumidores legacy.

#### Scenario: Auditoría de regresión
- **WHEN** se revisa el diff y corre la suite focal
- **THEN** solo cambian componentes modernos, proyecto, pruebas y documentación canónica
