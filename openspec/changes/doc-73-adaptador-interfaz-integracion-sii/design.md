<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
## Context

DOC-73 añade la primera presentación específica de proveedor sobre el núcleo DOC-72. El backend moderno ya valida gate/contexto y entrega capacidades, tipologías e items normalizados. La entrega es de consulta y listado; no modifica ASMX, almacenamiento ni SII legacy.

## Decisions

### D-01 — Registro canónico aislado

`importar-servicio-web-sii-adapter.js` expondrá `INTEGRACIONSII` y se registrará mediante el registro DOC-72, sin heurísticas ni fallback. Satisface RQ-01.

### D-02 — API moderna como única frontera remota

El adaptador consumirá `resolveCapabilities` y `queryItems`. Mapper y lista no usarán fetch, XHR, jQuery AJAX, ASMX legacy ni transporte SII. Satisface RQ-02.

### D-03 — Mapping contractual sin reinterpretación

El mapper validará la forma mínima y proyectará `ExternalItemDto`/`ImportItemMetadataDto`. Los metadatos conocidos alimentarán columnas SII; `ExternalKey` será opaco. Satisface RQ-03.

### D-04 — Modelo local de consulta

La lista conservará una instantánea para filtros y paginación. Cambiar filtro, tipología o página no invocará el adaptador; actualizar sí inicia otra consulta. Satisface RQ-04.

### D-05 — Estados y selección accesibles

La lista representará preparando, disponible, vacío, indisponible, respuesta inválida y no autorizado. Derivará importabilidad de `AllowedActions`/`ImportStatus`, excluirá no importables de selección masiva y usará nodos de texto. Satisface RQ-05.

### D-06 — Aislamiento, regresión y documentación

Las pruebas Node reutilizarán fixtures y transporte falso, comprobando ausencia de red, logs sensibles, parsing de claves y cambios legacy. Los módulos serán `<Content>`; la documentación canónica estará en `docs/modulos/workflow/importar-servicio-web/DOC-73-adaptador-sii-consulta-listado/`. Satisface RQ-06.

## Risks / Trade-offs

- Metadatos desconocidos se conservan sin crear columnas implícitas.
- La paginación local cubre solo la respuesta recibida; `ContinuationToken` queda contractual.
- La presentación visible nunca sustituye gate/autorización backend.
- El bootstrap debe ser idempotente ante postbacks parciales.

## Migration Plan

1. Implementar mapper, lista y adaptador con fixtures.
2. Integrar con el registro DOC-72 sin cambiar el núcleo.
3. Registrar assets bajo el gate existente.
4. Ejecutar suites focales y regresiones con gate apagado.
5. Rollback retirando assets; no hay migración de datos.

## Open Questions

- La habilitación productiva depende de que el contrato backend requerido por Jira esté publicado en el ambiente objetivo.

