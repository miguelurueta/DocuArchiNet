# DOC-92 — Persistencia del segundo factor sobre esquema existente

- Ticket: DOC-92
- Cambio OpenSpec: doc-92-segundo-factor-persistencia
- Clasificacion: cross_cutting

## Objetivo

Incorporar el repositorio inactivo de challenges 2FA reutilizando sin alteraciones la tabla compartida `docuarchi.ra_auth_second_factor_challenge`. DOC-92 no activa el login, no envía correo, no publica endpoints y no modifica la lógica visual.

## Alcance y compatibilidad

- Modelos y puerto: `Modelo/Login/SegundoFactor/`.
- Repositorio: `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb`.
- Tabla reutilizada: `ra_auth_second_factor_challenge`, con sus diez columnas existentes.
- Pruebas: dobles locales y harness MySQL sobre base descartable protegida.

No se crean tablas, columnas, índices, procedimientos ni eventos. Tampoco existe migración, backfill, limpieza o rollback SQL en DOC-92.

La compatibilidad es física: DocuArchiNet puede convivir con las filas del repositorio nuevo en la misma tabla. No significa que una aplicación pueda consumir challenges emitidos por la otra, porque difieren el formato criptográfico de `CodeHash` y el uso de `AuthPayloadJson`. Cada aplicación valida únicamente challenges que ella misma emitió.

La identidad persistida es la clave canónica en `AuthUserId`; no se guarda ni se reconstruye el login. Los estados funcionales se derivan de `Consumed`, `Attempts` y `ExpiresAtUtc`.
