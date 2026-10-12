# doblle-factor-interfaz-e2e Specification

## Purpose

Integrar una captura OTP accesible en el login Web Forms existente y validar de extremo a extremo que el segundo factor preserve seguridad, compatibilidad y navegación legacy.

## Requirements

### Requirement: RQ-01 Compatibilidad del login vigente

El sistema SHALL conservar el postback Web Forms como única entrada de credenciales y activar OTP solo ante `SECOND_FACTOR_REQUIRED` (D-01).

#### Scenario: Módulo con 2FA apagado
- **WHEN** las credenciales son válidas y 2FA está `0` o `NULL`
- **THEN** se completa la navegación vigente sin panel OTP

#### Scenario: Credenciales inválidas
- **WHEN** el postback devuelve otro error
- **THEN** se conserva el tratamiento vigente y no se muestra OTP

### Requirement: RQ-02 Panel OTP accesible

El sistema SHALL mostrar en `gestor.aspx` un panel accesible de seis dígitos y deshabilitar credenciales durante el pendiente (D-02, D-10).

#### Scenario: Pendiente visible
- **WHEN** el servidor confirma `RequiereCodigo=true` y `PENDIENTE`
- **THEN** muestra destino enmascarado, tiempo y acciones, y enfoca OTP

#### Scenario: Operación por teclado
- **WHEN** se opera sin ratón
- **THEN** label, foco, regiones vivas y botones tienen orden/nombres accesibles

### Requirement: RQ-03 Consumo mínimo del ASMX

El cliente SHALL usar los métodos existentes con Session; solo `Verificar` recibe `codigo As String` (D-03, D-04).

#### Scenario: Payload de verificación
- **WHEN** se valida OTP
- **THEN** el JSON contiene solo `codigo`

#### Scenario: Operaciones sin identidad
- **WHEN** se obtiene estado, reenvía o cancela
- **THEN** no se envían usuario, correo, challenge, módulo, empresa o ruta

### Requirement: RQ-04 Exclusión mutua y tiempo autoritativo

El cliente SHALL admitir una operación mutante en vuelo y mostrar tiempos desde el servidor sin autorizar por reloj local (D-05).

#### Scenario: Doble activación
- **WHEN** se hace doble click
- **THEN** sale una solicitud y los controles quedan deshabilitados

#### Scenario: Fin del contador
- **WHEN** llega a cero
- **THEN** consulta estado y no autentica ni reenvía por sí mismo

### Requirement: RQ-05 Restauración del pendiente

El sistema SHALL consultar `ObtenerEstado()` al cargar y después de postbacks parciales (D-06).

#### Scenario: Refresh pendiente
- **WHEN** se recarga durante un challenge vigente
- **THEN** el panel reaparece sin nuevas credenciales

#### Scenario: Sin pendiente
- **WHEN** recibe `NO_DISPONIBLE`
- **THEN** el login queda disponible sin error espurio

### Requirement: RQ-06 Finalización y ruta local

El cliente SHALL navegar solo después de completar y a una ruta local segura (D-07).

#### Scenario: OTP válido
- **WHEN** recibe éxito, `COMPLETADO` y ruta `~/` válida
- **THEN** navega una vez y existe autenticación emitida por servidor

#### Scenario: Ruta rechazada
- **WHEN** contiene esquema, host, traversal, barra inversa, query o fragmento
- **THEN** no navega y restaura login con error genérico

### Requirement: RQ-07 Fallos fail-closed

Los terminales SHALL limpiar OTP/contraseña, restaurar credenciales y no autenticar (D-08).

#### Scenario: OTP inválido y bloqueo
- **WHEN** un inválido queda pendiente
- **THEN** conserva panel; cuando servidor bloquea, vuelve al login sin cookie

#### Scenario: Expiración, cancelación o límite
- **WHEN** servidor devuelve terminal
- **THEN** exige reiniciar login y no navega

#### Scenario: Falla de transporte, correo o finalización
- **WHEN** no puede completar
- **THEN** nunca degrada a acceso sin segundo factor

### Requirement: RQ-08 Confidencialidad cliente

La UI SHALL evitar secretos e identificadores internos en DOM, URL, storage, consola y evidencia (D-09).

#### Scenario: Inspección de superficie
- **WHEN** el panel está pendiente, falla o completa
- **THEN** solo expone destino enmascarado y mensajes públicos

### Requirement: RQ-09 Pruebas focales

La entrega SHALL probar módulo cliente, markup, contrato y assets sin duplicar backend (D-11).

#### Scenario: Suite DOC-96
- **WHEN** se ejecuta
- **THEN** valida payload, estados, carrera cliente, ruta, accesibilidad y `.vbproj`

### Requirement: RQ-10 E2E real autorizada

La entrega SHALL reutilizar `tools/e2e` con autorización nueva, secretos efímeros y evidencia saneada (D-12).

#### Scenario: Módulo OFF y ON
- **WHEN** se ejecuta contra ambos perfiles
- **THEN** OFF conserva destino y ON completa solo tras OTP válido

#### Scenario: Control OFF temporal autorizado
- **WHEN** el ambiente no dispone de una cuenta válida de un módulo configurado OFF y se autoriza explícitamente el control temporal
- **THEN** cambia únicamente `RequiereSegundoFactor`, valida login legacy, restaura el snapshot ON aunque falle la E2E y confirma la restauración mediante `SELECT`

#### Scenario: Sin autorización o configuración
- **WHEN** falta ambiente, cuenta, buzón o datos autorizados
- **THEN** se detiene con código saneado y queda bloqueada la validación

#### Scenario: Matriz activa
- **WHEN** corren escenarios habilitados
- **THEN** refresh, inválido/bloqueo, expiración, reenvío, cancelación y concurrencia se observan sin sleeps largos

### Requirement: RQ-11 Inmutabilidad del backend DOC-95

DOC-96 SHALL reutilizar el contrato y SHALL NOT agregar datos, endpoints, DTOs o configuración (D-13).

#### Scenario: Revisión del diff
- **WHEN** se revisa la entrega
- **THEN** cambios funcionales se limitan a página, cliente, estilo y pruebas; una incompatibilidad reabre refinamiento
