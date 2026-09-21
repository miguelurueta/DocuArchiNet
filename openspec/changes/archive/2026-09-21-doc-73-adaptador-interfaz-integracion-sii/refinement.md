<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-73-adaptador-interfaz-integracion-sii

## Fuente y alcance

- Ticket: `DOC-73` — ADAPTADOR-INTERFAZ-INTEGRACION-SII.
- Cambio: `doc-73-adaptador-interfaz-integracion-sii`.
- Perfil tecnológico: JavaScript ES5/UMD sobre ASP.NET WebForms, pruebas Node `node:test` y contrato ASMX ya publicado.
- Alcance: adaptador, mapper y lista SII de solo lectura; registro de assets y documentación del feature.

## Contexto inspeccionado

- `js/workflow/importar-servicio-web/importar-servicio-web-{api,core,provider-registry,ui}.js`: núcleo DOC-72 y transporte único.
- `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`: contratos de capacidades, items y metadatos.
- `webservice/WebServiceImportarServicioWebModern.asmx.vb`: `ResolveCapabilities` y `QueryItems`.
- `tests/Fixtures/Workflow/ImportarServicioWeb/contracts-v1/` y `sii-v1/`: fixtures deterministas.
- Referencia legacy de solo lectura: `WebService_integracion_sii.asmx.vb` y `Class_consultarInformacionSello.vb`; no se modifican.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Registrar solo la identidad canónica `INTEGRACIONSII`; ninguna identidad desconocida cae en SII. | `importar-servicio-web-provider-registry.js` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Consumir solo `resolveCapabilities` y `queryItems` mediante el cliente API DOC-72. | `importar-servicio-web-api.js`; endpoint moderno | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Mapear campos normalizados y metadatos explícitos; nunca descomponer `ExternalKey`. | `ExternalItemDto`; `ImportItemMetadataDto` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Mantener filtros, selección y paginación en memoria; solo actualizar repite `QueryItems`. | core DOC-72 y fixtures `sii-v1` | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Renderizar estados seguros y permitir selección solo a items importables. | `AllowedActions`; `ImportStatus` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Probar sin red/secretos/SII real y preservar legacy, almacenamiento, caché y auditoría. | fixtures y regresiones DOC-72 | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Solo `INTEGRACIONSII` resuelve al adaptador. | Registrado resuelve; ausente/desconocido falla cerrado. | Preserva ausencia de fallback. |
| RQ-02 | Una consulta usa capacidades y una llamada `QueryItems`. | Transporte falso observa ambas operaciones y ninguna legacy. | Backend conserva autorización. |
| RQ-03 | Items se presentan desde DTO normalizado. | Columnas provienen de propiedades/metadatos; clave opaca. | Evita acoplamiento SII. |
| RQ-04 | Filtros y páginas locales no consultan backend. | Interacciones mantienen contador; refrescar consulta una vez. | Evita carga duplicada. |
| RQ-05 | Cardinalidad, errores y selección son accesibles/seguros. | Importados/no importables quedan fuera de selección masiva. | No amplía permisos. |
| RQ-06 | Entrega determinista sin modificar legacy. | Suites con fixtures y antirregresión pasan sin red. | Rollback por retiro de assets. |

## Reglas de trazabilidad obligatorias

Cada decisión D-XX aparece en diseño, requisito RQ-XX y tareas con `Origen: D-XX, RQ-XX`.

## Resultado del refinamiento

- Estado: aprobado.
- La integración productiva permanece condicionada a los contratos backend publicados; DOC-73 no altera endpoints ni persistencia.

