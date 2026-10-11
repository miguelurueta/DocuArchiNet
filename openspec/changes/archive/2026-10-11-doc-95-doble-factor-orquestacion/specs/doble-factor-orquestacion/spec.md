<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14 -->
# Especificación — Orquestación y ASMX de Login 2FA

## ADDED Requirements

### Requirement: RQ-01 Persistencia compatible sin DDL

**Origen:** D-01.

La solución MUST reutilizar `ra_auth_second_factor_challenge` y sus diez columnas actuales, mantener `AuthPayloadJson` en `NULL` y utilizar la transición atómica `Consumed=0 -> 1` como adquisición exclusiva. NO DEBE crear tablas, columnas, índices o procedimientos, ni afirmar que la tabla diferencia físicamente `FINALIZING`, `COMPLETED` y `FINALIZATION_FAILED`.

#### Scenario: Una sola verificación adquiere el challenge

- **WHEN** dos verificaciones correctas compiten por el mismo challenge vigente
- **THEN** exactamente una obtiene la transición de `Consumed=0` a `Consumed=1`
- **AND** la otra recibe un resultado público genérico sin ejecutar el finalizador

#### Scenario: La persistencia conserva el contrato compartido

- **WHEN** se crea o reemplaza un challenge
- **THEN** solo se escriben las columnas ya existentes
- **AND** `AuthPayloadJson` permanece `NULL`

### Requirement: RQ-02 Servicio de aplicación aislado

**Origen:** D-02.

`SecondFactorAuthenticationService` MUST coordinar `Start`, `GetStatus`, `Verify`, `Resend` y `Cancel` mediante interfaces del modelo. NO DEBE depender directamente de `HttpContext`, `Session`, `FormsAuthentication`, SQL, ODBC, `SmtpClient` ni clases concretas de infraestructura.

#### Scenario: El servicio se prueba sin infraestructura real

- **WHEN** se construye con dobles de sus puertos
- **THEN** los cinco casos de uso pueden ejecutarse sin servidor web, MySQL o SMTP

### Requirement: RQ-03 Inicio seguro después de validar credenciales

**Origen:** D-03.

La rama con 2FA activo MUST crear el challenge, enviar el OTP mediante `ISecondFactorEmailSender`, confirmar el envío y guardar el contexto pendiente solo después de que las credenciales hayan sido validadas. Un fallo DEBE consumir el challenge cuando corresponda, limpiar el contexto y no emitir cookie. Con 2FA desactivado se DEBE conservar el recorrido implementado por DOC-94.

#### Scenario: Inicio 2FA exitoso

- **WHEN** la configuración exige 2FA y la identidad tiene correo autorizado
- **THEN** se genera challenge, nonce y OTP nuevos
- **AND** se persiste el HMAC contextual, se envía el correo y se guarda el contexto pendiente
- **AND** la respuesta indica que el segundo factor es requerido

#### Scenario: Falla la entrega del OTP

- **WHEN** el envío o su confirmación falla
- **THEN** el challenge queda inutilizable, el contexto se limpia y no se emite cookie
- **AND** el cliente recibe un error controlado sin detalle SMTP

#### Scenario: Módulo con 2FA desactivado

- **WHEN** la configuración no exige segundo factor
- **THEN** no se invoca ninguna dependencia nueva de challenge o correo
- **AND** continúa el login legacy existente

### Requirement: RQ-04 Session como autoridad exclusiva

**Origen:** D-04.

El contexto pendiente MUST residir únicamente en Session e incluir los datos necesarios para reconstruir identidad, módulo y protección HMAC. Los endpoints NO DEBEN aceptar del cliente empresa, módulo, usuario, login, ID interno, challenge, nonce, correo ni URL de retorno.

#### Scenario: Sesión ausente o reemplazada

- **WHEN** se invoca un endpoint sin un contexto pendiente válido para la sesión actual
- **THEN** se limpia cualquier residuo y se devuelve una respuesta genérica
- **AND** no se consulta ni modifica un challenge indicado por el cliente

### Requirement: RQ-05 Validación, intentos y doble envío

**Origen:** D-05.

`Verify` MUST rechazar códigos que no tengan exactamente seis dígitos antes de consultar el repositorio, verificar identidad canónica y HMAC con el nonce de Session, registrar intentos fallidos y bloquear al quinto intento. La adquisición exclusiva DEBE ocurrir antes de llamar al finalizador legacy.

#### Scenario: Formato inválido

- **WHEN** el código es nulo, contiene caracteres no numéricos o no tiene seis dígitos
- **THEN** se rechaza sin acceder a persistencia

#### Scenario: Quinto intento incorrecto

- **WHEN** se registra el quinto código incorrecto
- **THEN** el challenge deja de ser elegible y el contexto pendiente se limpia

#### Scenario: Dos solicitudes correctas simultáneas

- **WHEN** dos solicitudes verifican correctamente el mismo OTP
- **THEN** una sola ejecuta `FinalizeLogin`

### Requirement: RQ-06 Finalización antes de cookie

**Origen:** D-06.

La autenticación MUST ejecutar `ILegacyLoginFinalizer.FinalizeLogin`, exigir la confirmación `Complete=True` y solo después emitir la cookie mediante `ISecondFactorAuthenticationTicketIssuer`. El éxito DEBE devolver exclusivamente `~/Defaul/WebFormInicioDocuarchiGestion.aspx`. Si cualquier paso falla, no se emite cookie, se limpia Session y el usuario debe reiniciar el login.

#### Scenario: Finalización completa

- **WHEN** la adquisición, el finalizador y `Complete` tienen éxito
- **THEN** se emite una única cookie de autenticación
- **AND** se devuelve la ruta local fija y se elimina el contexto pendiente

#### Scenario: Falla el finalizador o la confirmación

- **WHEN** falla `FinalizeLogin` o `Complete` no confirma el challenge consumido
- **THEN** no se emite cookie y el contexto se limpia
- **AND** no se promete compensación de efectos legacy ya ejecutados

### Requirement: RQ-07 Reenvío y cancelación acotados

**Origen:** D-07.

`Resend` MUST exigir 60 segundos de espera, permitir como máximo dos reenvíos, resolver nuevamente el correo desde repositorios autorizados y generar challenge, nonce y OTP nuevos. `Cancel` DEBE revocar el challenge activo y limpiar Session de forma idempotente. Ninguna operación DEBE autenticar al usuario.

#### Scenario: Reenvío elegible

- **WHEN** transcurrieron al menos 60 segundos y quedan reenvíos
- **THEN** el challenge anterior se revoca, el nuevo se crea atómicamente y se envía un OTP nuevo
- **AND** Session se reemplaza solo tras confirmar la entrega

#### Scenario: Cooldown o límite excedido

- **WHEN** no transcurrió el cooldown o ya hubo dos reenvíos
- **THEN** no se crea ni envía un challenge nuevo

#### Scenario: Cancelación repetida

- **WHEN** se cancela una o más veces
- **THEN** el challenge queda revocado, Session queda limpia y no se crea cookie

### Requirement: RQ-08 ASMX exclusivo y delgado

**Origen:** D-08.

`WebServiceLoginSegundoFactor` MUST ser `ScriptService`, habilitar Session y exponer exactamente `ObtenerEstado()`, `Verificar(codigo As String)`, `Reenviar()` y `Cancelar()` con respuesta JSON. Su code-behind solo DEBE aplicar gate, composición y mapeo; NO DEBE contener SQL, SMTP ni reglas de challenge.

#### Scenario: Contrato HTTP inspeccionado

- **WHEN** la prueba estructural analiza el ASMX y su code-behind
- **THEN** encuentra los cuatro métodos y sus firmas exactas
- **AND** cada método tiene `WebMethod(EnableSession:=True)` y `ScriptMethod(ResponseFormat:=Json)`

### Requirement: RQ-09 Respuesta saneada y ruta local

**Origen:** D-09.

Los DTO públicos MUST limitarse a éxito, código y mensaje controlados, estado público, destino enmascarado, segundos de expiración o cooldown y ruta local tras completar. NO DEBEN exponer challenge, IDs, login, correo completo, nonce, intentos internos, HMAC, excepciones, SQL o detalle SMTP, ni aceptar o reflejar `returnUrl`.

#### Scenario: Error sensible

- **WHEN** ocurre una falla de Session, persistencia, finalización o SMTP
- **THEN** la respuesta utiliza un código público estable y no revela el detalle interno

### Requirement: RQ-10 Composición compatible

**Origen:** D-10.

La composición WebForms MUST reutilizar los puertos, repositorios, protección HMAC, resolución de conexión y SMTP implementados por DOC-91 a DOC-94. NO DEBE modificar `ClassCorreo`, recuperación de contraseña, otros ASMX, configuración productiva ni datos reales.

#### Scenario: Construcción del grafo 2FA

- **WHEN** la factoría compone el servicio para una Session válida
- **THEN** utiliza las implementaciones existentes y mantiene los secretos dentro de sus adaptadores

### Requirement: RQ-11 Evidencia local y E2E autorizada

**Origen:** D-11.

La entrega MUST incluir pruebas locales de los flujos, errores y concurrencia, regresión DOC-91 a DOC-94, validación OpenSpec, validación documental y compilación. Una E2E real DEBE usar `tools/e2e` y su TTY únicamente con autorización nueva y explícita para DOC-95; sin ella el bloqueo DEBE reportarse y no sustituirse por evidencia simulada.

#### Scenario: No existe autorización vigente

- **WHEN** se llega a la tarea E2E sin autorización específica
- **THEN** no se ejecuta login, correo, escritura de challenge ni carga real
- **AND** el cambio no se declara completamente cerrado por E2E

### Requirement: RQ-12 Documentación, no regresión y rollback

**Origen:** D-12.

La documentación MUST corresponder al código implementado, distinguir estados lógicos de señales físicas, registrar efectos no compensables y mantener trazabilidad de endpoints, clases y firmas. El rollback DEBE consistir en desactivar 2FA mediante la configuración existente y revertir el artefacto, sin SQL de reversión.

#### Scenario: Validación estructural documental

- **WHEN** se ejecuta la prueba documental
- **THEN** valida los diagramas requeridos y los símbolos referenciados contra el código
- **AND** aclara que esta prueba no demuestra por sí sola fidelidad conductual completa

### Requirement: RQ-13 Observabilidad saneada del inicio

**Origen:** D-13.

La preautenticación y `Start` MUST compartir una correlación aleatoria y registrar sus límites mediante etapas y resultados cerrados. El puerto diagnóstico NO DEBE aceptar ni emitir login, IDs internos, correo, OTP, challenge, nonce, HMAC, SQL, conexión, contraseña o información de excepción. Una falla del diagnóstico NO DEBE cambiar el resultado del login. El despliegue MUST verificar la captura del listener y una corrida posterior a reciclado antes de declarar resuelto el riesgo transitorio.

#### Scenario: Falla antes de insertar el challenge

- **WHEN** una excepción interrumpe el inicio después de resolver el principal y antes de crear el challenge
- **THEN** las señales de una misma correlación identifican la última etapa alcanzada y la etapa fallida
- **AND** la respuesta pública permanece genérica y no expone el detalle interno

#### Scenario: Falla el destino diagnóstico

- **WHEN** el sink diagnóstico lanza una excepción
- **THEN** el flujo conserva el mismo resultado funcional que tendría sin observabilidad

### Requirement: RQ-14 Errores identificables y saneados

**Origen:** D-14.

El resultado HTTP MUST enviar únicamente un código de catálogo y una descripción segura. Los códigos internos desconocidos MUST colapsarse a `SECOND_FACTOR_UNAVAILABLE`. `Verify`, `Resend` y `Cancel` MUST registrar correlación, etapa y resultado cerrados sin texto libre sensible. Una llave HMAC ausente o un valor protegido inválido MUST tratarse como falla interna y NO DEBE incrementar los intentos del usuario.

#### Scenario: Error interno desconocido

- **WHEN** Presentation recibe un código fuera del catálogo aprobado
- **THEN** responde `SECOND_FACTOR_UNAVAILABLE` con descripción genérica
- **AND** no refleja el código ni el mensaje interno

#### Scenario: Llave de verificación ausente

- **WHEN** el challenge referencia una llave HMAC inexistente o su valor protegido es inválido
- **THEN** la verificación falla cerrada sin emitir cookie
- **AND** no incrementa el contador de intentos del usuario
- **AND** el diagnóstico identifica la etapa `CODE_VERIFICATION`
