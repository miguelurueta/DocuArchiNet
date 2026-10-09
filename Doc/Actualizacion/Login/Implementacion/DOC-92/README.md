# Implementación DOC-92

DOC-92 implementa un repositorio transaccional e inactivo para segundo factor reutilizando exactamente `ra_auth_second_factor_challenge`. No crea ni altera tablas, columnas o índices; por ello la carpeta `Sql` contiene solamente la explicación de que no existe migración.

La tabla guarda la identidad canónica en `AuthUserId`, no el login. `GetVerificationData` y `RegisterFailedAttemptData` retornan `SecondFactorStoredChallenge`; las firmas heredadas que exigirían reconstruir un login inexistente lanzan `NotSupportedException` en este adaptador.

`Consumed`, `Attempts` y `ExpiresAtUtc` son las únicas señales persistidas para derivar disponibilidad y estado. `TryBeginFinalization` adquiere el challenge mediante el cambio atómico `Consumed=0 -> 1`. El esquema no representa estados intermedios: si el finalizador falla después de adquirirlo, el usuario debe reiniciar el login.

## Compatibilidad

La compatibilidad con DocuArchiCore es estructural y permite convivencia en la tabla. Las filas no son intercambiables entre aplicaciones debido a sus diferentes contratos de `CodeHash` y `AuthPayloadJson`; DocuArchiNet deja este último en `NULL`.

## Evidencia

Las seis pruebas locales DOC-91/DOC-92, el refinamiento OPSXJ y OpenSpec estricto terminaron en código 0. La integración MySQL quedó `SKIP` por ausencia de autorización vigente; no se atribuye aprobación real. Consulte `Doc/Tecnica/Opsxj/doc-92-segundo-factor-persistencia/05-PruebasEvidencia.md`.

No se ejecutó ni se requiere DDL sobre la base existente.
