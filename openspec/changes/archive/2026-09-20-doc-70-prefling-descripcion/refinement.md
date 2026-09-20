<!-- opsxj:refinement version=1 state=approved -->
# Refinamiento - doc-70-prefling-descripcion

## Fuente y alcance
- Ticket: `DOC-70` — PREFLING-DESCRIPCION.
- Perfil: ASP.NET WebForms/VB.NET, ASMX y MySQL 5.1.
- Alcance: enriquecer el único `PreflightImport` y revalidar su huella en `CreateImportIntent`; no cambiar la ejecución DOC-67.

## Contexto inspeccionado
- `ServicioPreflightImportacion.Preflight` valida contexto, selección y tipología, construye `Commands`, tres requisitos y una huella basada en usuario, tarea, proveedor y selección.
- `ServicioIntencionImportacion.Crear` valida forma e idempotencia, pero no recompone ni contrasta `ContextFingerprint`.
- `MySqlImportExpedientConfigurationRepository.Obtener` lee por SELECT parametrizado configuración de expediente, identidad y tipologías secundarias.
- `ImportExpedientCoordinator.Resolver` materializa después expedientes, caché y planificación física; nada de ello puede adelantarse al preflight.

## Decisiones aprobadas
| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Extender aditivamente la respuesta con `Executable` y un plan por item; conservar campos actuales. | `ImportarServicioWebDtos.vb`; `ServicioPreflightImportacion.Preflight` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Publicar destino lógico y clases de efectos; nunca ID físico, gabinete, tabla, SQL o ruta. | `ImportExpedientCoordinator`; `ImportExpedientPlan` | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Resolver configuración mediante un puerto local de solo lectura y SQL parametrizado. | `MySqlImportExpedientConfigurationRepository.Obtener` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Mantener preflight con cero mutaciones y cero clientes/llamadas SII. | `ServicioPreflightImportacion`; composición ASMX | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Calcular huella determinista sobre contexto, selección, tipología y configuración efectiva. | `ServicioPreflightImportacion.Fingerprint` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Recomponer preflight y rechazar autoridad obsoleta antes del lock o persistencia de intención. | `ServicioIntencionImportacion.Crear` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Usar códigos estables y mensajes sin detalles físicos para los fallos funcionales. | `ServicioPreflightImportacion.Fail`; `ErrorImportacionServicioDto` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Probar plan, pureza, fingerprint y E2E con SELECT/telemetría y gate restaurado. | `tests/importar-servicio-web-preflight.test.cjs`; `tools/e2e/` | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables
| ID | Resultado observable | Escenario | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | N elementos producen N planes y un `Executable` agregado. | Selección válida individual o múltiple. | Contrato aditivo. |
| RQ-02 | Efectos previstos sin identidad física. | Informa almacenamiento, expediente, vínculo, caché e índices. | No afirmar efectos confirmados. |
| RQ-03 | Configuración procede del servidor. | Ausencia/inconsistencia bloquea ejecución. | SQL parametrizado. |
| RQ-04 | Cero efectos y cero tráfico SII. | Éxito y rechazo permanecen de solo lectura. | Preserva DOC-67. |
| RQ-05 | Huella cubre toda autoridad usada. | Repetición estable; cambio autoritativo cambia o rechaza. | Evita plan obsoleto. |
| RQ-06 | Creación exige coherencia vigente. | Alteración se rechaza antes del lock. | Mantiene idempotencia. |
| RQ-07 | Fallos distinguibles y seguros. | Códigos estables sin infraestructura. | No filtra secretos. |
| RQ-08 | Evidencia integral y saneada. | Suites, build y E2E con gate restaurado. | E2E no reemplaza ramas locales. |

## Resultado del refinamiento
- Estado: aprobado.
- El modo múltiple solo describe previsión lógica; no resuelve IDs ni una relación SII aún no definida.
