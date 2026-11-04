<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento DOC-115 — propuesta concreta

## Fuente y alcance

Ticket DOC-115, prompt 07. Tecnología comprobada legacy-webforms-vb, .NET Framework 4.6.1 y MySQL 5.1. El usuario pidió refinar; no se registra aprobación del diseño ni autorización de ejecución real. Base origin/main 8bf61815f15357947d7d3308fefb5192626988a8. Se actualizan artefactos y se propone plan atómico; producto permanece sin cambios.

## Contexto inspeccionado

Guía 00, prompt 03/07, Diseno/01-modelo-datos.md, 02-contratos-operacion.md, 03-estados-recuperacion.md y design.md; SQL 02 y procedimiento; MySqlImportacionCarpetasRepository y parciales; DTOs/modelos/interfaces; ServicioRecepcionImportacion, límites, guard, handler y staging; harness ImportacionCarpetasBehaviorTests; package/config/runbook/support/scripts/fixtures E2E. DOC-111 tiene pruebas locales y REC integrados históricos; DOC-114 verifica planes. Ninguno acredita nuevas ejecuciones RECIB DOC-115.

## Decisiones propuestas para aprobación

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Reutilizar una sola fundación operativa en 07-A y 07-B | MySqlImportacionCarpetasRepository.vb; Sql/02-carga-carpetas-mysql51.sql | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Confirmar bajo exclusión transaccional y unicidad de ámbito | MySqlImportacionCarpetasRepository.vb: ConfirmarPlan, Repeticion | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Conservar la autoridad y el contrato multipart cerrado | ImportacionCarpetasRequestGuard.vb: LeerMultipart; ServicioRecepcionImportacion.ObtenerRecepcion | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Mantener staging privado y cuotas efectivas sin ampliar configuración global | AlmacenStagingImportacion.vb: Recibir, CopiarAcotado, BytesRetenidos; LimitesImportacionCarpetas.DesdeTransporte | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Recuperar por evidencia persistida y física sin transacción disco/BD ficticia | ServicioRecepcionImportacion.vb: Recibir, ReconciliarRecepcion; MySqlImportacionCarpetasRepository.RegistrarFalloRecepcion | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Mantener revisión y reclamación persistida como exclusión real | MySqlImportacionCarpetasRepository.vb: ReservarIntento, ConfirmarRecepcion; AlmacenStagingImportacion.BloquearLote | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Congelar inventario y barrera sin invocar efectos documentales | CoordinadorImportacionCarpetas.Barreras.vb; ServicioRecepcionImportacion.vb | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Extender exclusivamente la infraestructura de pruebas existente | tools/e2e/tests/support/authenticated-workflow-session.cjs; doc111-importacion-carpetas.cjs; doc114-plan-importacion.cjs | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Cerrar cada fase con evidencia y aprobación explícita del alcance | Validacion/doc111-matriz-final-20261102.md; tools/e2e/AGENT-RUNBOOK.md | D-09 | RQ-09 | Origen: D-09, RQ-09 |

## Requisitos verificables

| ID | Resultado observable | Criterio | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Inventario y esquema únicos | El mapa diferencia archivos existentes de propuestas y el SQL operativo referencia el perfil sin recrearlo. | Conservar contratos existentes; evidencia real separada por fase. |
| RQ-02 | Confirmación idempotente | Dos confirmadores del mismo plan conservan un lote; la misma clave con otro payload devuelve conflicto. | Conservar contratos existentes; evidencia real separada por fase. |
| RQ-03 | Autorización de recepción | Archivo/lote cruzado, campos de destino o estado y petición sin autoridad se rechazan antes de escribir staging. | Conservar contratos existentes; evidencia real separada por fase. |
| RQ-04 | Integridad física y límites | Solo bytes completos y PDF válido pasan a RECIBIDO; rutas de cliente nunca forman la ruta física y el reenvío no duplica cuota. | Conservar contratos existentes; evidencia real separada por fase. |
| RQ-05 | Recepción recuperable | Corte de red, completo sin commit, fila sin temporal y respuesta perdida conservan estados verificables sin sobrescribir contenido. | Conservar contratos existentes; evidencia real separada por fase. |
| RQ-06 | Concurrencia y conteos | Dos receptores no generan dos commits ni doble contabilidad; vencimiento o token nuevo no autoriza otra escritura incierta. | Conservar contratos existentes; evidencia real separada por fase. |
| RQ-07 | Separación recepción/documentos | EXPEDIENTE/LOTE/NO_APLICA permanecen congelados y recibir no crea expedientes ni documentos definitivos. | Conservar contratos existentes; evidencia real separada por fase. |
| RQ-08 | Validación por fase y capacidad | 07-A usa repositorios reales independientes de UI; RECIB-01/02/03 ejecutan 07-B con controles SELECT y evidencia saneada. | Conservar contratos existentes; evidencia real separada por fase. |
| RQ-09 | Entrega y recuperación operativa | Cada caso identifica SHA/ambiente/estado y los no ejecutados bloquean el cierre correspondiente; SQL y datos reales requieren autorización propia. | Conservar contratos existentes; evidencia real separada por fase. |

## Resultado del refinamiento

BORRADOR REFINADO PARA REVISIÓN. Nueve decisiones, nueve requisitos y 28 tareas atómicas propuestas en tasks.md. No se cambian checks a completados ni el marcador a approved por generar documentos. La única compuerta editorial abierta es aprobación del alcance. Las integraciones 07-A/RECIB requieren autorización específica de ambiente, cuentas, datos y presupuestos; responsable: equipo del ambiente. Este corte no autentica, no aplica SQL y no prepara planes.

## Revisión y avance local — 2026-11-03

El usuario solicitó «contiua con el flujo». Se ejecutó revisión técnica local y se completaron 1.1/1.2/1.3/3.2/4.1: mapa, contraste SQL, procedimiento manual, precisión contractual e inventario E2E. No se infiere autorización de ambiente ni aprobación editorial explícita a partir de esta revisión. El marcador permanece draft. Informe: Validacion/doc115-revision-local-20261103.md. La auditoría de contenido mantiene 14 comprobaciones PASS y una compuerta de aprobación abierta.

## Aprobación editorial posterior — 2026-11-03

El usuario indicó «aprobado» después de la revisión técnica. Se aprueban las decisiones y tareas refinadas para implementación local. Los párrafos de revisión anteriores conservan su estado histórico; esta aprobación resuelve la compuerta editorial. No autoriza por sí sola pruebas reales, SQL, cargas ni nuevos recursos de ambiente.

## Verificación posterior a implementación — 2026-11-04

07-A y RECIB-01/02/03 EJECUTADO_OK; completadas 28/28 tareas conforme al alcance aprobado. Diagnósticos y pruebas usan la infraestructura compartida; no hubo cambios de producto VB, proyecto, DDL, configuración ni SII. Los resultados fechados en Validacion/ separan evidencia local, real e intentos fallidos anteriores. Este estado no constituye publicación ni cierre Jira.
