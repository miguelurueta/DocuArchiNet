# Propuesta — DOC-92 Persistencia transaccional del segundo factor

## Why

La fundación de Login 2FA necesita una persistencia atómica antes de exponer servicios o interfaz. La tabla central existente conserva el challenge básico, pero no representa de forma verificable propósito, vínculo no reversible de sesión, estados de concurrencia, reenvíos ni versión del contrato. Resolverlo en una tarea aislada reduce el riesgo de activar parcialmente el segundo factor o afectar módulos actuales.

## What Changes

- Extender de forma aditiva e idempotente `docuarchi.ra_auth_second_factor_challenge`, preservando filas e índices legacy.
- Implementar `MySqlSecondFactorChallengeRepository` con las abstracciones de datos compartidas, SQL parametrizado, transacciones y bloqueo de fila.
- Extender de forma compatible el contrato de DOC-91 para lectura verificable, envío, reenvío, expiración y estados terminales.
- Versionar preflight, apply, postflight, rollback y limpieza manual con retención de 30 días.
- Probar transiciones, rechazo de legacy, quinto intento, reenvío y concurrencia; la integración MySQL real queda condicionada a autorización vigente.
- Documentar esquema, firmas, operación, evidencia y riesgos residuales.

## Capabilities

### New Capabilities

- `segundo-factor-persistencia`: almacenamiento central y transaccional de challenges 2FA sin activar todavía el flujo de autenticación.

### Modified Capabilities

- Ninguna capacidad funcional existente. Se conserva el comportamiento actual del login y de todos los módulos.

## Impact

- **Modelo/contrato:** `Modelo/Login/SegundoFactor/`.
- **Infraestructura:** nuevo repositorio en `Infrastructure/Repositories/Login/SegundoFactor/`.
- **Base de datos:** scripts versionados en `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/`; no se ejecutan sobre ambientes compartidos sin autorización explícita actual.
- **Proyecto:** alta explícita de fuentes en `GestionDocumental-Docuarchi.net.vbproj`.
- **Pruebas:** pruebas focales sin secretos ni datos reales; integración MySQL descartable bajo compuerta de autorización.
- **Fuera de alcance:** ASMX, Controller, Service, UI, SMTP, configuración funcional, Session/HttpContext y cambios en datos reales.

## Compatibilidad

Las extensiones de tabla permanecen nullable para filas legacy, mientras las filas nuevas se identifican con `SchemaVersion = 1` y cumplen invariantes desde Application. No se eliminan firmas de DOC-91 ni índices existentes. La implementación queda inactiva hasta que tareas posteriores conecten servicios e interfaz.
