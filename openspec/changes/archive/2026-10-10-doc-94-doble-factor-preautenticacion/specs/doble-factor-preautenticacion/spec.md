## Purpose

Define la preautenticación del login privado para decidir de forma segura entre conservar el recorrido legacy o detenerlo antes de autenticar cuando el módulo exige segundo factor.

## ADDED Requirements

Trazabilidad de decisiones del cambio: D-01, D-02, D-03, D-04, D-05, D-06, D-07, D-08, D-09, D-10, D-11.

### Requirement: RQ-01 Alcance interno de preautenticación

El sistema SHALL limitar DOC-94 a la decisión interna de preautenticación y SHALL NOT crear challenge, enviar correo, publicar servicios web ni modificar la interfaz OTP.

#### Scenario: Segundo factor activo

- **WHEN** las credenciales son válidas y la configuración efectiva es `1`
- **THEN** el sistema devuelve internamente `SECOND_FACTOR_REQUIRED`
- **AND** no persiste challenge, no envía correo y no expone datos al navegador.

### Requirement: RQ-02 Validación legacy preservada

El sistema SHALL reutilizar sin cambios funcionales las reglas de conexión, tipo de módulo y credenciales existentes.

#### Scenario: Estado por módulo

- **WHEN** se valida un usuario de cualquiera de los cuatro módulos
- **THEN** Gestor y Workflow conservan su validación de estado
- **AND** Radicación y DocuArchi no reciben una validación de estado nueva.

### Requirement: RQ-03 Finalizador legacy único

El sistema SHALL ejecutar sesiones, relaciones, permisos y auditoría mediante un único finalizador sin redirect.

#### Scenario: Finalización exitosa

- **WHEN** el finalizador recibe una identidad y módulo verificados
- **THEN** ejecuta una sola vez y en el orden legacy los efectos del módulo
- **AND** devuelve un resultado tipado sin establecer la cookie Forms Authentication.

#### Scenario: Wrapper legacy

- **WHEN** el servicio indica finalización inmediata y esta concluye correctamente
- **THEN** el wrapper ejecuta `FormsAuthentication.RedirectFromLoginPage`
- **AND** ningún otro componente redirige.

### Requirement: RQ-04 Contexto independiente del challenge

El sistema SHALL representar la finalización mediante `LegacyLoginFinalizationContext` y SHALL NOT exigir `PendingSecondFactorContext` para el recorrido sin 2FA.

#### Scenario: 2FA apagado

- **WHEN** la configuración sea `0` o `NULL`
- **THEN** el finalizador recibe empresa, módulo, tipo, ID interno y login normalizado
- **AND** no se fabrican challenge ID, nonce, expiración ni destino enmascarado.

### Requirement: RQ-05 Configuración central autoritativa

El sistema SHALL resolver la configuración desde `gestor_modulos` mediante la conexión ODBC central existente, parámetros y cardinalidad exacta.

#### Scenario: Selección válida

- **WHEN** empresa y módulo seleccionados corresponden a una única fila central
- **THEN** se devuelve un snapshot tipado con tipo, requisito 2FA, proveedor y expiración.

#### Scenario: Selección ambigua o inválida

- **WHEN** la consulta devuelve cero o múltiples filas, o valores no soportados
- **THEN** el sistema falla cerrado sin autenticar y entrega un error sanitizado.

### Requirement: RQ-06 Identidad autoritativa en cuatro módulos

El sistema SHALL resolver ID interno, login canónico y correo mediante un adaptador parametrizado específico para DocuArchi, Gestor, Radicación o Workflow.

#### Scenario: Identidad única

- **WHEN** el login validado identifica exactamente una fila en la base del módulo seleccionado
- **THEN** el adaptador devuelve la identidad canónica de esa fila.

#### Scenario: Identidad incompleta o ambigua con 2FA activo

- **WHEN** no existe una fila única, el ID no es válido o, con segundo factor activo, el correo está vacío
- **THEN** el sistema no finaliza ni emite cookie y devuelve un error controlado.

#### Scenario: Correo ausente con 2FA apagado

- **WHEN** existe una identidad única y válida, el correo está vacío y `RequiereSegundoFactor` es `0` o `NULL`
- **THEN** el correo no bloquea el finalizador legacy
- **AND** el recorrido conserva el destino previo.

#### Scenario: Login repetido entre empresas

- **WHEN** el mismo texto de login existe en dos empresas
- **THEN** solo se consulta el snapshot de conexión correspondiente a la empresa seleccionada y validada por el servidor.

### Requirement: RQ-07 Contexto especializado antes de identidad

El sistema SHALL permitir abrir la conexión del módulo antes de resolver el ID únicamente mediante `ContextoPreautenticacionModulo`.

#### Scenario: ID aún desconocido

- **WHEN** el contexto tiene módulo y login válidos e `IdUsuario = 0`
- **THEN** el contexto especializado es válido para resolver identidad
- **AND** `ContextoModulo.EsValido()` continúa rechazando el mismo contexto.

### Requirement: RQ-08 Decisión de segundo factor

El sistema SHALL finalizar inmediatamente con `0`/`NULL`, detenerse con `1` y fallar cerrado con cualquier otro valor.

#### Scenario: Configuración apagada

- **WHEN** `RequiereSegundoFactor` es `0` o `NULL`
- **THEN** se ejecuta exactamente una vez el finalizador y el wrapper conserva el destino legacy.

#### Scenario: Configuración activa

- **WHEN** `RequiereSegundoFactor` es `1`
- **THEN** se resuelve la identidad y se devuelve el resultado mínimo de preautenticación
- **AND** no se inicializan usuario autenticado, permisos, relaciones, auditoría ni cookie.

#### Scenario: Configuración inválida

- **WHEN** el valor no es `0`, `1` o `NULL`
- **THEN** el sistema falla cerrado sin efectos autenticados.

### Requirement: RQ-09 Protección de secretos y capas

El sistema SHALL descartar la contraseña después de la validación y SHALL mantener Session/HttpContext fuera de repositorios.

#### Scenario: Credenciales ya verificadas

- **WHEN** `ValidaUserAplicacion` termina
- **THEN** la contraseña local se limpia antes de consultar configuración o identidad.

#### Scenario: Acceso a datos

- **WHEN** un repositorio abre una conexión
- **THEN** recibe un contexto o snapshot tipado y no accede directamente a Session/HttpContext.

### Requirement: RQ-10 Cobertura de regresión

El sistema SHALL contar con pruebas deterministas para los cuatro módulos y todas las ramas de decisión sin requerir infraestructura real.

#### Scenario: Equivalencia apagada

- **WHEN** cada módulo se prueba con `0` y `NULL`
- **THEN** sesiones, relaciones, permisos, auditoría y destino observables coinciden con el recorrido caracterizado.

#### Scenario: Cero efectos activa

- **WHEN** cada módulo se prueba con configuración `1`
- **THEN** el finalizador, auditoría, cookie y sesiones autenticadas registran cero invocaciones.

### Requirement: RQ-11 Documentación trazable

El sistema SHALL documentar los contratos y flujos implementados y SHALL validar estructuralmente sus referencias.

#### Scenario: Validación documental

- **WHEN** se ejecuta la prueba de documentación
- **THEN** detecta diagramas faltantes, sintaxis inválida y símbolos o firmas inconsistentes
- **AND** declara que esta prueba no demuestra por sí sola fidelidad conductual completa.
