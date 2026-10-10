# doble-factor-contrato Specification

## Purpose

Define el comportamiento verificable de la fundación interna de autenticación de segundo factor para login, manteniéndola aislada e inactiva hasta una integración posterior explícita.

## Requirements

### Requirement: RQ-01 Fundación aislada

El sistema SHALL incorporar contratos 2FA únicamente en las rutas inventariadas y sin activarlos desde el login productivo. Esta requirement materializa D-01.

#### Scenario: Compilación sin activación

- **WHEN** se agregan los archivos y entradas `Compile` de DOC-91
- **THEN** el proyecto compila sin nuevas referencias desde `gestor.aspx`, `ClassGestorSesion`, `ClassCorreo`, Forms Authentication, SMTP, ASMX o repositorios MySQL

### Requirement: RQ-02 Modelo y política canónicos

El sistema SHALL representar propósito `LOGIN`, los nueve estados aprobados, identidad normalizada y política 5 intentos/60 segundos/2 reenvíos, EMAIL=1 y expiración de 1 a 10 minutos. Esta requirement materializa D-02.

#### Scenario: Configuración válida

- **WHEN** la configuración indica requerido=1, proveedor=1 y expiración entre 1 y 10
- **THEN** se construye una configuración activa con los límites vinculantes

#### Scenario: Configuración inválida o desactivada

- **WHEN** requerido es 0 o NULL
- **THEN** la configuración queda desactivada sin challenge
- **AND WHEN** requerido, proveedor o expiración queda fuera del contrato
- **THEN** el modelo la rechaza sin degradación silenciosa

### Requirement: RQ-03 OTP uniforme y tiempo comprobable

El sistema SHALL generar OTP de seis dígitos con RNG criptográfico y muestreo por rechazo, y SHALL consultar tiempo mediante un reloj inyectable. Esta requirement materializa D-03.

#### Scenario: Generación sin sesgo de módulo

- **WHEN** la fuente criptográfica entrega una muestra dentro del mayor múltiplo aceptable
- **THEN** el resultado contiene exactamente seis caracteres numéricos
- **AND WHEN** entrega una muestra fuera del límite
- **THEN** el generador la descarta y solicita otra muestra

#### Scenario: Expiración determinista

- **WHEN** un reloj controlado alcanza la expiración del challenge
- **THEN** el modelo lo considera expirado sin consultar `DateTime.Now`

### Requirement: RQ-04 Protección HMAC ligada al contexto

El sistema SHALL proteger códigos con HMAC-SHA256 en formato `v1:<keyId>:<base64mac>` usando propósito, challenge, identidad, vínculo de sesión y OTP, y SHALL comparar MAC de igual longitud mediante acumulación XOR. Esta requirement materializa D-04.

#### Scenario: Verificación válida

- **WHEN** código, contexto y llave coinciden con el valor protegido
- **THEN** la verificación retorna verdadero

#### Scenario: Manipulación o formato inválido

- **WHEN** cambia cualquier componente, versión, llave, longitud, Base64 o MAC
- **THEN** la verificación retorna falso sin aceptar prefijos ni lanzar detalles sensibles

### Requirement: RQ-05 Anillo de llaves externo y rotable

El sistema SHALL resolver la llave activa desde `LoginSecondFactorHmacActiveKeyId` y cada material desde `LoginSecondFactorHmacKey.<keyId>`, exigiendo Base64 de al menos 32 bytes. Esta requirement materializa D-05.

#### Scenario: Rotación durante challenge vigente

- **WHEN** un hash fue creado con una llave anterior aún configurada
- **THEN** puede verificarse por su `keyId`, mientras nuevas protecciones usan solo la llave activa

#### Scenario: Configuración insegura

- **WHEN** falta la llave, el identificador no existe, Base64 es inválido o el material tiene menos de 32 bytes
- **THEN** la operación falla de forma cerrada y no usa un valor por defecto

### Requirement: RQ-06 Contexto pendiente mínimo en Session

El sistema SHALL aislar `HttpSessionStateBase` en un adaptador que guarda solo IDs resueltos, login normalizado, destino enmascarado, challenge, nonce y tiempos. Esta requirement materializa D-06.

#### Scenario: Ciclo del contexto

- **WHEN** se guarda un contexto válido
- **THEN** puede recuperarse hasta expirar y `Clear` lo elimina

#### Scenario: Datos prohibidos

- **WHEN** se inspecciona el contrato persistido en Session
- **THEN** no contiene contraseña, OTP, correo completo, HMAC, llave, credenciales ni conexión

### Requirement: RQ-07 Respuesta pública sanitizada

El sistema SHALL definir DTO serializables que expongan solo éxito, código público, mensaje neutro, destino enmascarado y tiempos relativos. Esta requirement materializa D-07.

#### Scenario: Inspección de DTO

- **WHEN** se inspeccionan propiedades y una instancia serializada
- **THEN** no aparecen ID interno, login, correo completo, estado persistido, SQL, excepción o secreto

### Requirement: RQ-08 Evidencia reproducible y antirregresión

El sistema SHALL disponer de pruebas focales de los contratos anteriores, registrar los archivos en el proyecto y conservar evidencia sanitizada del build. Esta requirement materializa D-08.

#### Scenario: Validación de la fundación

- **WHEN** se ejecutan la suite focal, MSBuild Debug y la inspección estructural
- **THEN** terminan con código cero, todos los fuentes están registrados una vez y no existen cambios en archivos legacy prohibidos

#### Scenario: Herramienta no disponible

- **WHEN** MSBuild no está disponible en el ambiente
- **THEN** se registra el comando, el bloqueo y la comprobación alternativa sin declarar compilación exitosa

### Requirement: RQ-09 Documentación estructural verificable

El sistema SHALL mantener un inventario explícito de diagramas Mermaid y resolver sus referencias `CODE` mediante análisis Roslyn de Visual Basic. Esta requirement materializa D-09.

#### Scenario: Diagrama o símbolo inconsistente

- **WHEN** falta un diagrama requerido, Mermaid rechaza su sintaxis, una fuente no existe o una declaración/firma/DTO difiere del manifiesto
- **THEN** la prueba falla indicando archivo, símbolo y motivo

#### Scenario: Exclusiones controladas

- **WHEN** un participante no se resuelve contra el repositorio
- **THEN** solo se admite si está inventariado como `EXT:` o `CONCEPT:`

### Requirement: RQ-10 Finalización independiente de challenge

El contrato `ILegacyLoginFinalizer` SHALL recibir un `LegacyLoginFinalizationContext` que represente solamente la identidad y el módulo ya verificados.

#### Scenario: Finalización sin segundo factor

- **WHEN** el login legacy tiene segundo factor apagado
- **THEN** puede finalizar sin construir un `PendingSecondFactorContext`
- **AND** conserva el mismo resultado observable previo.

#### Scenario: Continuación OTP futura

- **WHEN** una implementación posterior verifique un challenge
- **THEN** podrá traducir la identidad verificada al mismo `LegacyLoginFinalizationContext`
- **AND** reutilizará el único finalizador sin duplicar lógica legacy.
