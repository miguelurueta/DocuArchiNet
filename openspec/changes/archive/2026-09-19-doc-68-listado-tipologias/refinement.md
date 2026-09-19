<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-68-listado-tipologias

## Fuente y alcance

- Ticket: `DOC-68` — LISTADO-TIPOLOGIAS.
- Cambio: `doc-68-listado-tipologias`.
- Fuente normativa: `specs/listado-tipologias/jira-context.md` y `Doc/Actualizacion/workflow/ImportarServicioWeb/PromptBackend/09-listado-enriquecido-catalogo-tipologias.md`.
- Dependencia cerrada: DOC-67, sin reabrir ni modificar su cambio archivado.
- Perfil: ASP.NET WebForms/ASMX, VB.NET, .NET Framework 4.6.1, MySQL y pruebas `node:test`.

## Contexto inspeccionado

- `webservice/WebServiceImportarServicioWebModern.asmx.vb`: `ResolveCapabilities` y `QueryItems` resuelven contexto de servidor; `QueryItems` devuelve actualmente el resultado básico del proveedor.
- `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`: `ExternalItemDto` solo publica clave, nombre, contenido, longitud y preview; capabilities no publica catálogo.
- `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb`: `MapQuery` aplana imágenes y ya tiene libro, registro, fecha, acto/nombre de acto e imagen en una única respuesta.
- `SiiExternalImportProviderClient.vb`: una consulta ejecuta token + `consultarInformacionSello`; el enriquecimiento no debe invocar preview ni almacenamiento.
- `MySqlImportDocumentTypeResolver.vb`: valida una tipología con SQL parametrizado sobre `ra_dig_tipos_docum_lista_chequeo` y `tipo_doc_series`, por `contexto.IdTramite` y conexión Radicación.
- `radicador/ra_dig_tipos_docum_lista_chequeo.vb`: confirma `OBLIGATORIO`, `ORDEN_LISTA`, `ID_TIPO_DOCUMENTAL_CHEQUEO` y `tipo_doc_series_Id_Tipo_Doc_Series`; el contrato moderno usa el ID TRD, no el ID de checklist.
- `MySqlImportIntentRepository.vb` y `MySqlImportReconciliationRepository.vb`: contienen tarea, proveedor, clave externa, estado y documento confirmado para resolver estado sin SII.
- `Tests/Fixtures/Workflow/ImportarServicioWeb/sii-v1/query-provider-response.json`: fixture saneado con los campos disponibles.
- `tools/e2e`: runner, autenticación, gate y evidencia existentes que deben reutilizarse.

## Compatibilidad preservada

- Extensión aditiva; no renombrar ni cambiar campos 1.0.
- `ExternalKey` permanece opaco para el frontend.
- No modificar endpoints legacy, `ClassAlmacenamiento`, DOC-67 ni su orquestación.
- SQL nuevo exclusivamente `SELECT` parametrizado con conexiones del servidor.
- Gate apagado conserva legacy y no ejecuta llamadas modernas.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Extender capabilities con catálogo e item con metadatos/estado/acciones; subcontratos nuevos 1.1 y envelope 1.0 compatible. | `ImportarServicioWebDtos.vb`; patrón `ImportItemExpedientEffectsDto` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Mapear metadatos `Code/Label/Value` desde la única respuesta ya descargada, saneados y sin campos SII en el núcleo. | `SiiImportContractMapper.MapQuery`; fixture SII | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Obtener catálogo por `IdTramite` desde Radicación con ID TRD, descripción, `OBLIGATORIO` y orden. | `MySqlImportDocumentTypeResolver`; clase legacy de checklist | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Calcular `Disponible`, `Importado` o `ConNovedad` y acciones desde persistencia moderna por tarea+proveedor+clave. | tablas/repo de intención y reconciliación | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Enriquecer tras una sola llamada funcional a `QueryItems`; catálogo, estado, filtros y paginación son locales; E2E reutilizada. | endpoint, cliente SII y `tools/e2e` | D-05 | RQ-05 | Origen: D-05, RQ-05 |

## Requisitos verificables

| ID | Resultado observable | Escenario o aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Contratos aditivos deserializables por clientes 1.0. | Fixtures viejos conservan campos; nuevos reciben presentación 1.1. | Cambiar campos rompe ASMX. |
| RQ-02 | Cada item presenta campos disponibles sin payload crudo ni datos prohibidos. | Varias inscripciones correlacionan metadatos con su `ExternalKey`. | Campos opcionales admiten ausencia y límites. |
| RQ-03 | Catálogo solo contiene tipologías válidas del trámite. | SQL parametrizado devuelve ID TRD, nombre, obligatoriedad y orden; no expone ID de checklist. | Mapeo ambiguo falla seguro. |
| RQ-04 | Estado/acciones impiden duplicados desde evidencia local. | Confirmado es `Importado`; inconsistente `ConNovedad`; ausente `Disponible`. | Parcial no anuncia éxito. |
| RQ-05 | Una consulta de listado realiza una sola `consultarInformacionSello`. | Cero/uno/múltiples items y filtros no incrementan llamadas SII. | Token+consulta es una secuencia, nunca por fila. |

## Resultado del refinamiento

- Estado: `approved`.
- D-01 a D-05 están sincronizadas con diseño, RQ-01 a RQ-05 y tareas atómicas.
- Próximo paso: ejecutar `opsxj:refine -- DOC-68 --sync` y no implementar hasta que la compuerta pase.
