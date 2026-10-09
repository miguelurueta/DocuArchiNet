<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
# Diseño técnico — DOC-92

## Evidencia inspeccionada

- DocuArchiCore `MiApp.Repository/Repositorio/Autenticacion/SecondFactorChallengeRepository.cs` usa `ra_auth_second_factor_challenge` para insertar, consultar, incrementar y consumir.
- `MiApp.Models/Models/Auntetnicacion/AuthSecondFactorChallengeDA.cs` declara exactamente diez propiedades persistidas.
- La inspección física versionada confirma InnoDB, `uq_challengeid` e `IX_ra_auth_sfc_authuserid`.
- La fundación local se encuentra en `Modelo/Login/SegundoFactor/`; las abstracciones ADO.NET están en `Infrastructure/Shared/Data/`.

## D-01 — Alcance inactivo

DOC-92 no expone endpoint, servicio web, UI, correo ni activación del login.

## D-02 — Contrato físico exacto, sin DDL

El repositorio usa únicamente:

| Columna | Uso DocuArchiNet |
| --- | --- |
| `Id` | PK administrada por MySQL; no se escribe explícitamente |
| `ChallengeId` | Identificador público único |
| `AuthUserId` | Identidad canónica `empresa:módulo:tipo:idInterno` |
| `Provider` | `EMAIL` |
| `CodeHash` | HMAC `v1:keyId:mac`, ligado a propósito, identidad, challenge y sesión |
| `ExpiresAtUtc` | Vencimiento UTC |
| `Consumed` | Invalidez/adquisición exclusiva |
| `Attempts` | Intentos fallidos, máximo cinco |
| `CreatedAtUtc` | Creación y referencia de cooldown |
| `AuthPayloadJson` | `NULL` en filas creadas por DocuArchiNet |

No se crean o alteran tablas, columnas, índices, procedimientos, eventos ni schedulers.

## D-03 — Índices existentes

Se depende de `uq_challengeid` y `IX_ra_auth_sfc_authuserid` sin modificarlos. La búsqueda y las mutaciones usan `ChallengeId` parametrizado.

## D-04 — Dependencias inyectadas

`MySqlSecondFactorChallengeRepository` recibe `IModuleConnectionFactory`, `IDataExecutor`, `ITransactionFactory`, `ContextoModulo` e `ISecondFactorClock`. No conoce contexto HTTP ni configuración de conexión.

## D-05 — Identidad y protección

La tabla no almacena login, por lo que la lectura retorna `SecondFactorStoredChallenge` con `CanonicalIdentity`. `GetForVerification` y `RegisterFailedAttempt` se conservan por compatibilidad de interfaz, pero lanzan `NotSupportedException`; se usan sus variantes de proyección. El enlace de sesión, propósito y `KeyId` están autenticados dentro de `CodeHash`, no en columnas adicionales.

## D-06 — Concurrencia

Las operaciones compuestas abren transacción, leen `FOR UPDATE` y actualizan con condiciones sobre `Consumed`, `Attempts` y `ExpiresAtUtc`. El quinto error deja `Attempts=5`. La adquisición correcta cambia `Consumed` de 0 a 1; dos verificaciones concurrentes producen un solo ganador.

## D-07 — Semántica derivada

- `Consumed=1` se proyecta como completado/no reutilizable.
- `Attempts>=5` se proyecta como bloqueado.
- `ExpiresAtUtc<=clock.UtcNow` se proyecta como expirado.
- En otro caso se proyecta como enviado/activo.
- El reenvío consume el anterior e inserta el nuevo en una transacción; el cooldown usa `CreatedAtUtc`. El límite de dos proviene del contexto de dominio/sesión porque la tabla no almacena el contador.

No se persisten `CREATED`, `FINALIZING`, `DELIVERY_FAILED`, `REVOKED` o `FINALIZATION_FAILED` como valores separados. Tras adquirir, un fallo de finalización no puede desconsumir el challenge y requiere reiniciar el login.

## D-08 — Sin migración ni limpieza automática

`Doc/Actualizacion/Login/Implementacion/DOC-92/Sql/README.md` deja constancia de que no aplica DDL ni rollback. El esquema actual carece de fecha terminal, por lo que DOC-92 no introduce una eliminación por retención que pudiera afectar filas de DocuArchiCore.

## D-09 — Verificación

Las pruebas locales cubren contrato exacto, parámetros, HMAC, payload nulo, intentos, consumo único y rollback del reemplazo. El harness MySQL crea la tabla exacta solamente en una base descartable `doc92_*` cuando existe autorización vigente; en otro caso queda omitido explícitamente.

## D-10 — Documentación comprobable contra código

La documentación DOC-92 mantiene un manifiesto explícito de documentos, diagramas, declaraciones, relaciones interfaz/implementación, firmas y propiedades. Mermaid valida sintaxis y Roslyn para Visual Basic valida estructuralmente archivos, tipos, pertenencia de métodos, parámetros, retornos y sobrecargas. Solo actores `EXT:` y conceptos `CONCEPT:` quedan excluidos de resolución contra código. La prueba se integra en el workflow existente y declara que la correspondencia estructural no demuestra por sí sola fidelidad conductual completa.

## Inventario

| Acción | Ruta |
| --- | --- |
| Modificar | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` |
| Modificar | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` |
| Crear | `Infrastructure/Repositories/Login/SegundoFactor/MySqlSecondFactorChallengeRepository.vb` |
| Modificar | `GestionDocumental-Docuarchi.net.vbproj` |
| Crear | `tests/LoginSecondFactorPersistenceBehaviorTests.cs` |
| Crear | `tests/login-second-factor-persistence.test.cjs` |
| Crear | `tools/e2e/tests/login-second-factor-persistence.integration.test.cjs` |
| Documentar | `Doc/Actualizacion/Login/Implementacion/DOC-92/` y `Doc/Tecnica/Opsxj/doc-92-segundo-factor-persistencia/` |
| Crear | `Doc/Actualizacion/Login/Implementacion/DOC-92/diagram-contract.json` y `Diagramas/*.mmd` |
| Crear | `tests/doc92-technical-documentation.test.cjs` |
| Modificar | `tools/validation/Doc72SourceValidator/Program.cs` y `.github/workflows/opsxj-validation.yml` |
