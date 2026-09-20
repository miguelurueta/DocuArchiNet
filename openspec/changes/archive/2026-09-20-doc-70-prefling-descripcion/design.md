<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Context
El preflight actual valida contexto, selección y tipología y genera una huella incompleta. No consulta configuración de expediente, no describe efectos previstos y `CreateImportIntent` no revalida la huella. DOC-67 materializa los efectos posteriormente.

## Goals / Non-Goals
**Goals:** plan lógico por item, requisitos seguros, configuración local, `Executable`, fingerprint completo, revalidación, pureza y contrato aditivo.

**Non-Goals:** resolver/exponer `ExpedientId`, consultar SII, crear efectos, cambiar `ExecuteImportIntent`, definir la futura relación multi-expediente del proveedor o modificar frontend/legacy.

## Architecture
```text
PreflightImport -> gate/contexto -> ServicioPreflightImportacion
 -> IImportDocumentTypeResolver + IImportEffectConfigurationRepository (SELECT)
 -> ImportEffectPlanBuilder -> DTO + fingerprint
CreateImportIntent -> recomponer preflight -> comparar -> lock/idempotencia -> persistencia
```

## Decisions
### D-01 — Contrato aditivo
Agregar DTOs de plan/requisito por item y `Executable`; conservar campos actuales.
### D-02 — Destino exclusivamente lógico
Publicar tarea, tipología, `Single`/`Multiple`, expediente requerido y efectos previstos. Los IDs físicos solo existen después de ejecutar.
### D-03 — Configuración local autoritativa
Un puerto de solo lectura proyecta datos mínimos desde configuración existente mediante SELECT parametrizado.
### D-04 — Preflight puro
Builder, servicio y repositorio no dependen de cliente SII ni repositorios de mutación.
### D-05 — Fingerprint canónico
SHA-256 cubre usuario, tarea, ruta, trámite, proveedor, selección normalizada y configuración; las colecciones se ordenan.
### D-06 — Revalidación de intención
La creación usa el mismo preflight y exige resultado ejecutable, fingerprint y requisitos equivalentes antes del guard.
### D-07 — Errores cerrados
Códigos: `EFFECT_CONFIGURATION_UNAVAILABLE`, `DOCUMENT_TYPE_INVALID`, `EFFECT_DESTINATION_AMBIGUOUS`, `PREFLIGHT_UNAVAILABLE` y `PREFLIGHT_STALE`.
### D-08 — Verificación y rollback
Pruebas, build y E2E verifican pureza y compatibilidad. El gate apagado es rollback operativo.

## Risks / Trade-offs
- Un cambio entre preflight y creación se convierte en rechazo explícito.
- El plan no garantiza éxito físico: estados deben expresar previsión, no confirmación.
- Las lecturas locales adicionales evitan autoridad duplicada y tráfico SII.

## Migration Plan
1. Contratos/puertos. 2. Repositorio/builder. 3. Integrar preflight. 4. Revalidar creación. 5. Pruebas/documentación/E2E. No requiere migración de datos.
