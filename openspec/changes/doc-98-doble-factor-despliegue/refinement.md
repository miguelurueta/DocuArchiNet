<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-98-doble-factor-despliegue

## Fuente y alcance

- Ticket: `DOC-98` — DOBLE-FACTOR-DESPLIEGUE.
- Perfil: Web Forms/VB.NET 4.6.1, MySQL y Node/CJS.
- Alcance: documentación, SQL operativo guardado, pruebas, CI, piloto y activación global autorizada por oleadas.
- Exclusiones: mutaciones sin autorización, secretos, DDL, scheduler, cambios de producto y bypass por usuario.

## Contexto inspeccionado

- `Web.config`: appSettings externo y Session InProc de 10 minutos.
- `AppSettingsSecondFactorKeyProvider.vb`: nombres, Base64 y mínimo 32 bytes.
- `MySqlSecondFactorChallengeRepository.vb`: señales físicas y `v1:keyId:mac`.
- sinks diagnósticos: catálogo sanitizado correlation/stage/outcome.
- `.vbproj`: registro de fuentes/recursos DOC-91..96.
- `DOC-92/Sql/README.md`: ausencia comprobada de migración.
- matrices DOC-94, DOC-96 y DOC-97: OFF/NULL, piloto y bloqueos.

## Decisiones aprobadas

| ID | Decisión | Evidencia | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Solo operación/validación; producto intacto. | `.vbproj`, DOC-91..97 | D-01 | RQ-01 | 1.1, 3.1, 4.2 |
| D-02 | Preflight; no se requiere DDL. | `DOC-92/Sql/README.md` | D-02 | RQ-02 | 1.2, 3.1 |
| D-03 | Llaves externas, Session y SMTP sin revelar. | key provider, `Web.config`, SMTP repo | D-03 | RQ-03 | 1.3, 3.1 |
| D-04 | Secuencia release y abortos. | runbook DOC-31, DOC-97 | D-04 | RQ-04 | 2.1, 3.3, 3.4, 4.1 |
| D-05 | Una fila central con snapshot/cardinalidad. | módulo repo/configuración | D-05 | RQ-05 | 2.2, 3.2, 3.4 |
| D-06 | Métricas físicas más diagnósticos. | challenge repo y sinks | D-06 | RQ-06 | 2.3, 3.2, 4.1 |
| D-07 | Rotación compatible con keyId persistido. | key provider/protector | D-07 | RQ-07 | 2.4, 4.1 |
| D-08 | Limpieza manual >30 días. | tabla sin scheduler | D-08 | RQ-08 | 2.5, 3.2, 4.1 |
| D-09 | OFF primero, datos preservados, paquete previo. | OFF/NULL y Session InProc | D-09 | RQ-09 | 2.6, 3.4, 4.1, 4.2 |
| D-10 | Tras el piloto, activar todas las filas autorizadas transversalmente por módulo, sin selector por usuario. | catálogo central y repositorios de principal en resolución de login | D-10 | RQ-10 | 2.7, 2.8, 3.5, 3.6, 4.1, 4.2 |

## Requisitos verificables

| ID | Resultado | Criterio | Compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Sin cambios productivos. | Inventario único y diff limitado. | Protege Login/correo. |
| RQ-02 | Preflight o NO-GO. | Solo SELECT; ausencias detienen. | Evita DDL duplicado. |
| RQ-03 | Configuración sin secretos. | Estado/cardinalidad solamente. | Fallo cerrado. |
| RQ-04 | Fases abortables. | Nada mutante tras puerta fallida. | Evita despliegue parcial. |
| RQ-05 | Piloto de una fila. | Snapshot y affected_rows=1. | Aísla módulos/esquemas. |
| RQ-06 | Métricas fieles/saneadas. | Agregados más stage/outcome. | No inventa estados. |
| RQ-07 | Challenges verificables al rotar. | Retiro tras TTL+margen y cero elegibles. | Evita rechazar OTP vigente. |
| RQ-08 | Limpieza gobernada. | Preview, >30 días, autorización. | Preserva evidencia. |
| RQ-09 | Rollback legacy sin borrar. | OFF, expiración y paquete previo. | Conserva datos. |
| RQ-10 | Todos los módulos quedan bajo 2FA transversal, sin activación por usuario. | Oleadas acotadas, diagnóstico agregado y verificación global. | Evita mecanismos paralelos y cambios fuera de alcance. |

## Resultado del refinamiento

- Estado aprobado para implementación operativa, piloto y activación global controlada.
- Ningún ambiente fue consultado o modificado.
- Toda prueba o activación real queda bloqueada hasta recibir autorización vigente para el ambiente y la ventana concretos.
