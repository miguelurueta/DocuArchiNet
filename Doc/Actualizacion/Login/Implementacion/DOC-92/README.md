# Implementación DOC-92

Se implementó la persistencia transaccional e inactiva del challenge 2FA: contrato compatible, proyección sin login, repositorio MySQL parametrizado, paquete SQL idempotente, pruebas locales y harness de integración protegido.

## Decisión corregida durante implementación

La tabla guarda `AuthUserId` canónico y no guarda login. `SegundoFactorIdentity` no puede rehidratarse sin inventar datos. Por ello `GetVerificationData` y `RegisterFailedAttemptData` retornan `SecondFactorStoredChallenge`. Las dos firmas antiguas que retornan `SegundoFactorChallenge` permanecen en el puerto, pero el repositorio MySQL lanza `NotSupportedException`; los consumidores nuevos no deben usarlas.

## Evidencia

- Build MSBuild: código 0.
- Pruebas DOC-91 + DOC-92: 6 aprobadas, 0 fallidas.
- Integración MySQL descartable: automatizada pero no ejecutada; faltó autorización vigente específica y se registró como `SKIP`.
- DDL/rollback/cleanup real: no ejecutados.

## Riesgo residual

La sintaxis y correspondencia estructural de SQL están verificadas localmente, pero la idempotencia y el ganador único requieren ejecución posterior en MySQL descartable autorizado. El rollback pierde los atributos v1 y exige respaldo. No existe atomicidad distribuida con el futuro finalizador de login.

Consulta [Sql/README.md](Sql/README.md) para el orden de despliegue y reversa.
