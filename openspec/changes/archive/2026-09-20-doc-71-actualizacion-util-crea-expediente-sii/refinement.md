<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-71-actualizacion-util-crea-expediente-sii

## Fuente y alcance

- Ticket: `DOC-71` — ACTUALIZACION-UTIL-CREA-EXPEDIENTE-SII
- Cambio OpenSpec: `doc-71-actualizacion-util-crea-expediente-sii`
- Fuente Jira: `specs/actualizacion-util-crea-expediente-sii/jira-context.md`
- Perfil: ASP.NET WebForms/VB.NET, .NET Framework 4.6.1 y MySQL 5.1.

## Contexto inspeccionado

- `MySqlImportExpedientConfigurationRepository.Obtener` asigna hoy `CreacionAutomaticaHabilitada` y `ExpedienteObligatorio` desde `header.CreateEnabled`.
- `ImportExpedientCoordinator.Resolver` devuelve `EXPEDIENT_CREATION_DISABLED` cuando `ExpedienteObligatorio=False`, antes del almacenamiento.
- `ImportServiceOrchestrator.Execute` exige un plan de expediente confirmado y solo completa efectos después de `ImportRelatedDocumentCoordinator.Procesar`.
- `EstadoEfectoExpedienteImportacion` no representa todavía `NoAplica`; los estados se persisten en `workflow_import_intent_item` y se proyectan mediante `ImportItemResultMapper`.
- `PhysicalImportExpedientRepository.Crear` conserva una comprobación tardía de `CreacionAutomaticaHabilitada` que debe mantenerse.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Introducir un modo autoritativo `SinExpediente`/`GestionarExpediente`; el cliente no lo controla. | `MySqlImportExpedientConfigurationRepository.Obtener` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | `CreateEnabled=0` permite almacenar y marca efectos de expediente `NoAplica`; no genera `EXPEDIENT_CREATION_DISABLED`. | `ImportExpedientCoordinator.Resolver`, `ImportServiceOrchestrator.Execute` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Agregar `NoAplica` como estado persistible y público, sin confundirlo con confirmado, en espera o fallido. | `EstadoEfectoExpedienteImportacion`, `MySqlImportIntentRepository`, `ImportItemResultMapper` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | En modo sin expediente se consulta el expediente SII para construir los índices; se omiten caché, búsqueda, creación y vínculo físicos, y se actualizan obligatoriamente todos los documentos del mismo `ENLASE` sin exigir `ID_EXPEDIENTE`. | `ImportExpedientCoordinator`, `ModernSiiExpedientSubjectResolver`, `ImportRelatedDocumentCoordinator`, `SiiDocumentIndexAdapter`, `LegacySiiDocumentIndexPhysicalGateway` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | La intención completa si almacenamiento e índices documentales están confirmados y los efectos exclusivos de expediente están confirmados o no aplican según el plan. | `ImportServiceOrchestrator`, `ImportIntentStateMachine` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | El modo forma parte del plan y fingerprint; un cambio 0↔1 invalida el preflight antes de persistir. | `ImportEffectPlanBuilder`, `ServicioPreflightImportacion`, `ServicioIntencionImportacion` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | La rama `GestionarExpediente` conserva secuencia, idempotencia, conflictos y comprobación tardía DOC-67. | `ImportExpedientCoordinator.ResolveOne`, `PhysicalImportExpedientRepository.Crear` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | La entrega exige pruebas automatizadas y E2E reales saneadas de ambas ramas, con gate restaurado. | `tests/importar-servicio-web-*.test.cjs`, `tools/e2e` | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | El backend determina el modo desde la configuración confiable. | `CreateEnabled=0` produce `SinExpediente`; `1`, `GestionarExpediente`. | No aceptar el modo del cliente. |
| RQ-02 | Un trámite en 0 almacena el sello y termina sin expediente. | No aparece `EXPEDIENT_CREATION_DISABLED`; efectos omitidos quedan `NoAplica`. | No activar trámites para superar pruebas. |
| RQ-03 | Consulta y persistencia diferencian `NoAplica`. | Reiniciar/releer conserva los estados. | MySQL 5.1 y valores históricos continúan parseando. |
| RQ-04 | Rama 0 consulta el expediente SII, tiene cero efectos físicos de expediente y actualiza NIT/cédula, razón social y matrícula disponibles en todos los documentos del `ENLASE`. | MERCANTIL/ESAL consultan por matrícula; RUP por proponente. El `UPDATE` identifica cada documento por `ID` y `ENLASE`, nunca por `ID_EXPEDIENTE`, y relee los valores. | Identidad consultable obligatoria; NIT/razón social opcionales; validar campos/tipos dinámicos. |
| RQ-05 | Item e intención completan sin reconciliación falsa. | Almacenamiento e índices documentales confirmados + efectos de expediente `NoAplica` ⇒ `Completada`. | No afirmar vínculo, caché o expediente inexistente. |
| RQ-06 | Cambios de configuración invalidan el contexto preparado. | Cambio 0↔1 rechaza antes de persistir. | Mantener fingerprint determinista DOC-70. |
| RQ-07 | Rama 1 conserva DOC-67. | Caché, búsqueda, creación, vínculo, índices y reconciliación siguen aprobando. | Evitar regresiones de idempotencia. |
| RQ-08 | Evidencia demuestra ambas ramas y restauración operacional. | E2E autorizada, SELECT y gate apagado. | Si falta recurso se documenta bloqueo. |

## Reglas de trazabilidad obligatorias

1. Cada decisión D-XX aparece en diseño, especificación y tareas.
2. No se modifica código legacy ni `AlmacenaDocumentoTareaWorkflow`.
3. La consulta tipada de expediente SII es obligatoria; no se incluye fallback con propietario ni reconstrucción histórica.
4. Las E2E reales requieren autorización explícita independiente.

## Resultado del refinamiento

- Estado: aprobado para implementación.
- Revisión: alcance contrastado con símbolos reales del repositorio el 2026-09-20.
