# Arquitectura y diagramas

## Arquitectura implementada

La implementación es una biblioteca interna dentro del ensamblado WebForms existente. No hay recorrido `Cliente → Controller → Service → Repository → Respuesta` implementado en DOC-91. El recorrido real disponible es `consumidor interno futuro → contrato/interfaz → implementación de seguridad o Session → resultado/exception`.

| Capa real | Componentes | Dependencia permitida |
| --- | --- | --- |
| Dominio | modelos, estados, validaciones y puertos | `System`; sin WebForms, SMTP o SQL |
| DTO | `SegundoFactorEstadoDto`, `SegundoFactorResultadoDto` | tipos simples y DTO de estado |
| Seguridad | generador OTP, reloj, HMAC y proveedor de llaves | criptografía BCL, `ConfigurationManager`, puertos de dominio |
| Presentación | `SessionPendingSecondFactorContextStore` | `HttpSessionStateBase`, reloj y contexto pendiente |

## Diagramas obligatorios

### 1. Componentes y clases

Fuente: [01-componentes-clases.mmd](Diagramas/01-componentes-clases.mmd).

Representa las cinco relaciones interfaz–implementación existentes. No muestra implementación para `ISecondFactorConfigurationRepository`, `ISecondFactorChallengeRepository`, `ISecondFactorRecipientResolver`, `ISecondFactorEmailSender` ni `ILegacyLoginFinalizer` porque no existe en este ticket.

### 2. Secuencia de OTP y HMAC

Fuente: [02-secuencia-seguridad.mmd](Diagramas/02-secuencia-seguridad.mmd).

Traza `GenerateCode() As String`, `Protect(...) As String`, `Verify(...) As Boolean`, resolución de llave, rechazo de muestras RNG, validaciones, errores y rotación. Los participantes `RandomNumberGenerator` y `HMACSHA256` son `EXT:` porque pertenecen al BCL.

### 3. Validaciones y configuración

Fuente: [03-validaciones-configuracion.mmd](Diagramas/03-validaciones-configuracion.mmd).

Documenta únicamente las decisiones codificadas: activación 0/1/NULL, EMAIL=1, expiración 1..10, formato OTP, enmascarado, expiración del challenge y fallo cerrado de llaves.

### 4. Secuencia de Session

Fuente: [04-secuencia-session.mmd](Diagramas/04-secuencia-session.mmd).

Traza las firmas exactas de `Save`, `GetCurrent` y `Clear`, incluyendo contexto nulo, contexto expirado, limpieza y retorno `Nothing`. No representa autenticación ni creación de cookie porque esas operaciones no están implementadas.

## Trazabilidad

Cada diagrama contiene comentarios `Fuentes` y `Referencias CODE`. La prueba exige que todas las fuentes existan, que cada referencia esté declarada en el manifiesto, que los únicos símbolos no resolubles tengan prefijo `EXT:` o `CONCEPT:`, y que Mermaid acepte la sintaxis completa.
