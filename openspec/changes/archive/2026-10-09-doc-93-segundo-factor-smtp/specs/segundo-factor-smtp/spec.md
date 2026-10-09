<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## Purpose

Proveer infraestructura SMTP interna, tipada y fail-closed para entregar OTP reutilizando la configuración existente sin alterar el correo legacy.

## ADDED Requirements

### Requirement: RQ-01 — Infraestructura inactiva (D-01)
DOC-93 SHALL agregar infraestructura interna sin activar login, UI, ASMX, Session ni challenges.

#### Scenario: despliegue aislado
- **WHEN** se despliega DOC-93
- **THEN** los recorridos existentes conservan su comportamiento y no se expone una ruta nueva.

### Requirement: RQ-02 — Compatibilidad del puerto (D-02)
La implementación SHALL conservar `ISecondFactorEmailSender.Send` y los contratos DOC-91.

#### Scenario: consumidor heredado
- **WHEN** un consumidor invoca el puerto existente
- **THEN** recibe `SecondFactorDeliveryResult` sin conocer `System.Net.Mail`.

### Requirement: RQ-03 — Configuración y resultado tipados (D-03)
La infraestructura SHALL usar modelos inmutables y estados `Submitted`, `Disabled`, `InvalidConfiguration`, `AmbiguousConfiguration` y `Failed`.

#### Scenario: resultado público
- **WHEN** termina resolución o envío
- **THEN** el resultado contiene solo estado y código público sanitizado.

### Requirement: RQ-04 — Conexión reutilizada y SQL explícito (D-04)
El repositorio SHALL usar fábrica y ejecutor compartidos con contexto inyectado.

#### Scenario: lectura de configuración
- **WHEN** se resuelve SMTP
- **THEN** se seleccionan columnas explícitas con `ESTADO_ENVIO=@enabled`
- **AND** no se usa `SELECT *`, `conect`, Session, `HttpContext` ni conexión paralela.

### Requirement: RQ-05 — Cardinalidad estricta (D-05)
El repositorio SHALL exigir exactamente una fila activa.

#### Scenario: configuración ausente
- **WHEN** hay cero filas
- **THEN** retorna `Disabled` y no crea cliente SMTP.

#### Scenario: configuración ambigua
- **WHEN** hay más de una fila
- **THEN** retorna `AmbiguousConfiguration` sin elegir ninguna.

### Requirement: RQ-06 — Validación previa a red (D-06)
La configuración SHALL validar host, puerto, remitente, banderas, credenciales y tiempo antes de abrir recursos SMTP.

#### Scenario: campo inválido
- **WHEN** un campo requerido es nulo, vacío o fuera de rango
- **THEN** retorna `InvalidConfiguration` y no abre SMTP.

### Requirement: RQ-07 — Timeout compatible y acotado (D-07)
El timeout SHALL conservar `SMTP_TIEMPO * 100000` con aritmética comprobada y máximo 120000 ms.

#### Scenario: overflow
- **WHEN** la multiplicación excede `Int32` o el valor no es positivo
- **THEN** la configuración es inválida.

#### Scenario: dominio legacy
- **WHEN** existe `DOMINIO_SMTP`
- **THEN** se lee pero no se aplica a `NetworkCredential`.

### Requirement: RQ-08 — Transporte OTP determinístico (D-08)
El transporte SHALL enviar sincrónicamente un OTP mínimo y liberar sus recursos.

#### Scenario: envío válido
- **WHEN** la configuración es única y válida
- **THEN** SSL y credenciales reflejan la tabla
- **AND** el cuerpo solo contiene código, expiración y advertencias aprobadas
- **AND** retorna `Submitted` si el cliente acepta el envío.

#### Scenario: excepción de envío
- **WHEN** SMTP lanza una excepción
- **THEN** los recursos se liberan y retorna `Failed`.

### Requirement: RQ-09 — Sanitización estricta (D-09)
La implementación MUST NOT exponer secretos o detalles internos.

#### Scenario: fallo técnico
- **WHEN** ocurre error de repositorio o SMTP
- **THEN** el resultado no contiene contraseña, OTP, destinatario completo, cuerpo, conexión, excepción ni texto del servidor.

### Requirement: RQ-10 — Evidencia sin red y no regresión (D-10)
DOC-93 SHALL demostrar comportamiento con dobles y preservar correo legacy.

#### Scenario: ejecución local
- **WHEN** se ejecutan pruebas DOC-93 y regresiones DOC-91/DOC-92
- **THEN** se validan cardinalidad, transporte, disposición y sanitización sin red
- **AND** `ClassCorreo.vb` y sus llamadores no presentan diff funcional.

#### Scenario: integración real no autorizada
- **WHEN** no existe autorización vigente para ambiente, cuenta y buzón
- **THEN** no se envía correo ni se reporta éxito integrado.
