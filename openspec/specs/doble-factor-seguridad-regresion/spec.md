# doble-factor-seguridad-regresion Specification

## Purpose

TBD: definir el propósito estable de la capacidad después del archivado.

## Requirements

### Requirement: RQ-01 Trazabilidad del flujo real

La auditoría SHALL representar y verificar el flujo implementado desde `gestor.aspx` hasta finalizador, cookie y ruta local, sin composición, endpoint o contrato alternativo (D-01).

#### Scenario: Inventario resoluble
- **WHEN** se valida el inventario DOC-97
- **THEN** cada etapa referencia un archivo y símbolo real de DOC-91 a DOC-96
- **AND** una referencia inexistente falla con ruta y motivo

### Requirement: RQ-02 Matriz transversal de módulos

La suite SHALL cubrir DocuArchi, Gestor, Radicación y Workflow mediante el mismo contrato parametrizado, conservando diferencias legacy verificadas (D-02).

#### Scenario: Regla de estado por módulo
- **WHEN** se resuelve un principal de Gestor o Workflow
- **THEN** se aplica su condición legacy de estado
- **AND** DocuArchi y Radicación no reciben esa condición

#### Scenario: Configuración OFF o NULL
- **WHEN** cualquiera de los módulos tiene `RequiereSegundoFactor` en `0` o `NULL`
- **THEN** conserva finalización legacy sin challenge, SMTP ni panel OTP

#### Scenario: Configuración activa
- **WHEN** el módulo tiene configuración válida `1` y principal con correo
- **THEN** queda pendiente sin cookie, permisos, auditoría o navegación autenticada

### Requirement: RQ-03 Ciclo de challenge e aislamiento

La suite SHALL verificar expiración, intentos, reenvío, cancelación, replay, concurrencia y aislamiento mediante dobles deterministas (D-02, D-03).

#### Scenario: Límites y terminalidad
- **WHEN** ocurre el quinto intento, expira el challenge, se cancela o se consume
- **THEN** deja de ser elegible y no se emite cookie

#### Scenario: Concurrencia y replay
- **WHEN** dos verificaciones compiten o se reutiliza un OTP consumido
- **THEN** existe un solo ganador del finalizador y los demás fallan cerrados

#### Scenario: Aislamiento de contexto
- **WHEN** coinciden logins entre empresas o existen dos sesiones
- **THEN** empresa, módulo, usuario y binding de sesión impiden cruzar challenges

### Requirement: RQ-04 Confidencialidad y autoridad del servidor

La validación SHALL comprobar que OTP, HMAC, llaves, contraseñas, conexiones, correo completo e IDs internos no se exponen por DTO, DOM, URL, storage, log o evidencia, y que el cliente no selecciona identidad ni retorno (D-03).

#### Scenario: Contrato público mínimo
- **WHEN** se inspeccionan ASMX, DTO y JavaScript
- **THEN** solo `Verificar` acepta `codigo`
- **AND** estado, mensajes y ruta contienen solo datos públicos aprobados

#### Scenario: Persistencia mínima
- **WHEN** se crea o reemplaza un challenge
- **THEN** `AuthPayloadJson` permanece `NULL`
- **AND** no se persisten credenciales, conexión, OTP en claro ni contexto autenticado

### Requirement: RQ-05 Orden seguro y fallos cerrados

La autenticación SHALL emitir cookie y ruta local solo después de OTP correcto, adquisición exclusiva, finalización legacy exitosa y confirmación del challenge (D-03).

#### Scenario: Falla de dependencia
- **WHEN** configuración, proveedor, correo o finalización son inválidos o fallan
- **THEN** no se autentica y se devuelve código público saneado

#### Scenario: Comparación y llave HMAC
- **WHEN** se verifica un OTP
- **THEN** se usa protección contextual y comparación constante
- **AND** llave ausente o hash inválido falla cerrado sin contar como error del usuario

### Requirement: RQ-06 Corrección limitada a defectos demostrados

DOC-97 SHALL modificar lógica de producto únicamente cuando una prueba reproducible demuestre el defecto, preservando firmas y propietarios canónicos (D-04).

#### Scenario: Hallazgo funcional
- **WHEN** una prueba falla por comportamiento de producto
- **THEN** la corrección mínima cita prueba causal y deja verdes sus regresiones

#### Scenario: Cambio incompatible
- **WHEN** la corrección requiere DDL, nuevo contrato o cambio legacy
- **THEN** se detiene y documenta el bloqueo

### Requirement: RQ-07 Integración estable en CI

`opsxj-validation.yml` SHALL observar DOC-96/DOC-97 y ejecutar todas las pruebas 2FA locales estables, sin infraestructura real ni pipeline paralelo (D-05).

#### Scenario: Pull request 2FA
- **WHEN** cambia código, prueba o documentación 2FA
- **THEN** CI ejecuta contratos, comportamiento, documentación, políticas DOC-95/DOC-96 y matriz DOC-97
- **AND** informa el caso concreto al fallar

### Requirement: RQ-08 E2E real gobernada

La E2E DOC-97 SHALL reutilizar `tools/e2e`, requerir autorización específica, usar controles `SELECT`, mantener flags intactos y sanear salida (D-06).

#### Scenario: Precondición disponible
- **WHEN** ambiente, cuenta, buzón y configuración autorizados permiten el caso
- **THEN** se ejecuta sin imprimir secretos y se registra pasado o fallido

#### Scenario: Precondición ausente
- **WHEN** un caso exigiría cambiar un flag o no existe precondición autorizada
- **THEN** se registra bloqueado y no se fabrica cobertura

### Requirement: RQ-09 Evidencia y riesgos

La documentación SHALL registrar por caso evidencia local/E2E, comandos, códigos de salida, correcciones y pendientes, manteniendo riesgos residuales explícitos (D-07).

#### Scenario: Cierre de matriz
- **WHEN** se prepara la entrega DOC-97
- **THEN** cada celda figura pasada, fallida o bloqueada
- **AND** no se declara cobertura completa si existe un caso sin validar
