<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
# Especificación — Persistencia compatible del segundo factor

## ADDED Requirements

### Requirement: RQ-01 — Alcance inactivo (D-01)

El cambio SHALL agregar únicamente persistencia interna.

#### Scenario: despliegue aislado
- **WHEN** se despliega DOC-92
- **THEN** no cambia login, UI, ASMX, SMTP ni navegación.

### Requirement: RQ-02 — Tabla existente sin extensión (D-02)

El repositorio SHALL usar exclusivamente las diez columnas existentes y no SHALL requerir DDL.

#### Scenario: inserción compatible
- **WHEN** se crea un challenge
- **THEN** el `INSERT` menciona solo columnas existentes
- **AND** `AuthPayloadJson` queda `NULL`.

#### Scenario: revisión del paquete
- **WHEN** se inspeccionan archivos DOC-92
- **THEN** no existen scripts que creen o alteren tablas, columnas o índices.

### Requirement: RQ-03 — Índices preservados (D-03)

DOC-92 SHALL conservar `uq_challengeid` e `IX_ra_auth_sfc_authuserid` sin agregar índices.

#### Scenario: consulta y mutación
- **WHEN** se accede a un challenge
- **THEN** se usa `ChallengeId` parametrizado.

### Requirement: RQ-04 — Infraestructura desacoplada (D-04)

El repositorio SHALL usar las factorías compartidas y reloj inyectado, sin contexto HTTP.

#### Scenario: inspección estructural
- **WHEN** se inspecciona Infrastructure
- **THEN** no aparecen Session, HttpContext, `conect` ni cadenas de conexión.

### Requirement: RQ-05 — Identidad canónica y HMAC (D-05)

La identidad SHALL persistirse en `AuthUserId` y el HMAC versionado en `CodeHash`; login y vínculo de sesión no SHALL persistirse en claro.

#### Scenario: lectura verificable
- **WHEN** se obtiene un challenge activo
- **THEN** retorna identidad canónica y código protegido
- **AND** la verificación posterior liga el HMAC al vínculo de sesión recibido.

### Requirement: RQ-06 — Intentos y consumo atómicos (D-06)

Las mutaciones SHALL bloquear y actualizar condicionalmente usando las columnas existentes.

#### Scenario: quinto intento
- **GIVEN** cuatro intentos
- **WHEN** se registra un error
- **THEN** `Attempts` queda en cinco en un commit.

#### Scenario: doble verificación
- **WHEN** dos transacciones intentan adquirir el mismo challenge
- **THEN** exactamente una cambia `Consumed` de 0 a 1.

### Requirement: RQ-07 — Semántica representable (D-07)

La implementación SHALL derivar activo, bloqueado, expirado y consumido sin inventar estados físicos.

#### Scenario: reenvío
- **WHEN** el contexto permite reenviar después del cooldown
- **THEN** se consume el challenge anterior y se inserta el nuevo en una transacción.

#### Scenario: fallo después de adquirir
- **WHEN** falla la finalización después de `Consumed=1`
- **THEN** el challenge permanece inválido y el usuario debe reiniciar autenticación.

### Requirement: RQ-08 — Sin migración destructiva (D-08)

DOC-92 SHALL documentar que no aplica migración, rollback ni limpieza automática.

#### Scenario: despliegue
- **WHEN** se instala el binario
- **THEN** no se ejecuta SQL de esquema o mantenimiento.

### Requirement: RQ-09 — Evidencia honesta (D-09)

La implementación SHALL incluir pruebas locales y un harness MySQL descartable protegido.

#### Scenario: ausencia de autorización
- **WHEN** no existe autorización vigente
- **THEN** la integración real queda `SKIP`, no aprobada.

### Requirement: RQ-10 — Documentación estructural verificable (D-10)

DOC-92 SHALL documentar únicamente componentes implementados y SHALL mantener un inventario explícito de diagramas y símbolos verificables contra el código.

#### Scenario: validación documental completa
- **WHEN** se ejecuta la prueba documental
- **THEN** falla si falta un diagrama requerido o su sintaxis Mermaid es inválida
- **AND** falla si una clase, interfaz, relación, método, parámetro, retorno o propiedad no coincide estructuralmente con VB.NET
- **AND** excluye únicamente actores `EXT:` y conceptos `CONCEPT:` declarados.

#### Scenario: límite de la evidencia
- **WHEN** la prueba documental termina correctamente
- **THEN** se informa que acredita existencia y correspondencia estructural
- **AND** no se presenta como demostración completa del comportamiento o de todos los casos de uso.
