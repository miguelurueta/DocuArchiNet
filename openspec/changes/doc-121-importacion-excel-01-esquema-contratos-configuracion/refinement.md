<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento DOC-121

## Fuente y alcance

Cambio `doc-121-importacion-excel-01-esquema-contratos-configuracion`, perfil `legacy-webforms-vb`. El usuario eligió implementar los prompts actuales y solicitó continuar la fase 01. Reiteró expresamente que ejecutará los archivos SQL. La aprobación refleja ese alcance y la revisión técnica; no acredita instalación ni aceptación de recorridos futuros.

## Contexto inspeccionado

- `Gestion/Class_imp01_plantillaimp.vb`, `Solicita_identificacion_plantilla_externa_x_nombe`: consulta ID por nombre desde DA.
- `Gestion/Class_imp01_campos_plantilla.vb`: nombres físicos CAMPO_DESTINO/CAMPO_FUENTE para SII. Consumidores: `webservice/WebServicePlantillaExterna.asmx.vb` e `Integracionccv/Class_Integracion_SII.vb`.
- `Infrastructure/Shared/Data/ModuleSessionConnectionStringResolver.vb`, `Resolve`: resolución lógica por sesión; `PerfilImportacionContextAdapter.vb`, `Resolver`: autoridad RA y permiso de perfiles que no debe exigirse al cargador.
- `MySqlProveedorDatosIndices.vb`, `ConsultarDatos`: clave parametrizada, filtro empresarial y LIMIT 2; `RestriccionTablasFuente.vb`, `Permite`: prefijo y conexiones autorizadas.
- Proyecto antiguo `preproyectoimport/ClassCargarDatos.vb`: hoja y PK técnica opcional IMP_02_ID_CLAVE. La clave de negocio del nuevo contrato es otra responsabilidad.
- Exploración del 11 de noviembre: antecedente de metadatos. La ampliación posterior de BENEFICIARIO debe conservarse.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Reutilizar catálogos y consumidores; entregar solo fase 01. | Class_imp01_plantillaimp.Solicita_identificacion_plantilla_externa_x_nombe; Class_imp01_campos_plantilla | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | SQL incremental manual y comprobaciones SELECT, sin aplicar instalación. | Prompt 01, flujo SQL manual; SQL históricos 01/06/07 de carpetas | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Una tabla operativa InnoDB en DA conserva identidad, archivo, configuración y resultado. | Exploración, esquema operativo; patrón MySqlProveedorIndicesRepository | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Permiso propio, cuenta vinculada por servidor y límites positivos 10000/10. | ModuleSessionConnectionStringResolver.Resolve; PerfilImportacionContextAdapter.Resolver | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Clave requerida y UNIQUE completo según aislamiento y metadatos reales. | MySqlProveedorDatosIndices.ConsultarDatos; RestriccionTablasFuente.Permite | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Contratos por ID; multipart y validación completa antes de confirmar. | PerfilImportacionRequestGuard.Validar como patrón; prompts 03/04 | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | DML y resultado atómicos, CREATE separado y consulta antes de repetir; sin pausa por fila. | Prompt 05, Confirmar/ConsultarResultado; exploración, creación recuperable | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Evidencia trazable y estado de instalación separado de la entrega contractual. | AGENTS.md; tools/e2e/AGENT-RUNBOOK.md; aceptación del prompt 01 | D-08 | RQ-08 | Origen: D-08, RQ-08 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Inventario y extensión aditiva. | Conservar nombres, orden histórico y consumidores SII. | Contrastar tipos actuales en preflight. |
| RQ-02 | SQL separado con destino y orden. | Preflight/postflight SELECT y cero aplicación automática. | DDL no se acredita por rollback DML. |
| RQ-03 | Registro de carga recuperable. | Estados y hashes coherentes, sin FK a catálogos o bases distintas. | NULL inicial no representa preparación válida. |
| RQ-04 | Autoridad y configuración explícitas. | Permiso exclusivo, límites y DA del ámbito. | Fuente legible no concede escritura. |
| RQ-05 | Una coincidencia por clave autorizada. | NOT NULL/UNIQUE de clave o empresa-clave completos. | No reducir tipos ni limpiar datos automáticamente. |
| RQ-06 | Requests y respuestas verificables. | Referencias/huellas y revisión completa sin credenciales. | Preparar conserva filas del destino. |
| RQ-07 | Una confirmación efectiva por ID. | Acuse perdido y rollback tienen recuperación distinta. | CREATE propio vacío se conserva y se consulta. |
| RQ-08 | Entrega con evidencia y límites. | OpenSpec/refinement y revisión estática; instalación no acreditada. | E2E corresponde a fases ejecutables. |

## Resultado del refinamiento

Revisión técnica conforme al alcance elegido por el usuario. Fuente Jira conservada. La generación de scripts no demuestra ejecución en MySQL. Auditoría: `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-121 --sync`.
