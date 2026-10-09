# DOC-92 — Persistencia transaccional del segundo factor

Implementa el almacenamiento central e inactivo de challenges 2FA. No activa el login, no publica endpoints y no envía correo.

## Componentes

- Modelos y puerto: `Modelo/Login/SegundoFactor/`.
- Implementación: `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb`.
- Esquema: `Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/`.
- Pruebas locales: `tests/login-second-factor-persistence.test.cjs`.
- Integración MySQL protegida: `tools/e2e/tests/login-second-factor-persistence.integration.test.cjs`.

La persistencia usa identidad canónica, no login. Por eso la lectura devuelve `SecondFactorStoredChallenge`; el login continúa en `PendingSecondFactorContext`. Las firmas legacy que exigirían reconstruir el login permanecen para compatibilidad, pero la implementación MySQL las rechaza explícitamente.

## Compatibilidad y reversa

Las nueve columnas son nullable para convivir con filas legacy. Solo `SchemaVersion=1` participa en el flujo nuevo. Se preservan índices existentes y no se hace backfill. `03-rollback.sql` revierte artefactos DOC-92, pero pierde atributos v1 y exige respaldo.
