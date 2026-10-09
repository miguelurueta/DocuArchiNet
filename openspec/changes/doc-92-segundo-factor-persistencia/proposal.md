# Propuesta — DOC-92 Persistencia compatible del segundo factor

## Why

DocuArchiNet debe utilizar la misma tabla 2FA que DocuArchiCore sin modificar su esquema. La mejora se concentra en SQL transaccional y parametrizado sobre las columnas existentes, manteniendo la fundación inactiva hasta tareas posteriores.

## What Changes

- Reutilizar exactamente `docuarchi.ra_auth_second_factor_challenge`; no crear tablas, columnas ni índices.
- Implementar un repositorio MySQL con las abstracciones compartidas y operaciones atómicas para intentos, consumo, revocación, expiración y reemplazo.
- Guardar la identidad canónica en `AuthUserId`, el HMAC versionado en `CodeHash` y `AuthPayloadJson=NULL` para no persistir contexto sensible.
- Derivar la condición operativa desde `Consumed`, `Attempts` y `ExpiresAtUtc`, únicas señales disponibles en el contrato compartido.
- Probar compatibilidad estructural y concurrencia; MySQL real sigue sujeto a autorización vigente.
- Documentar la arquitectura, casos de uso e inventario realmente implementados y validar automáticamente diagramas Mermaid y firmas VB.NET mediante análisis Roslyn.

## Impact

- Modifica modelos/puerto y agrega `MySqlSecondFactorChallengeRepository`.
- No modifica base de datos, ASMX, UI, SMTP ni login productivo.
- La prueba documental y su integración CI no modifican lógica de aplicación.
- DocuArchiCore conserva lectura/escritura de sus filas porque el contrato físico permanece idéntico.

## Limitación aceptada

El esquema actual no permite persistir estados detallados, contador histórico de reenvíos ni fecha terminal. Esos valores no se simulan mediante columnas o payload sensible. La adquisición exclusiva se representa con `Consumed=1`; un fallo posterior obliga a reiniciar autenticación.
