<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05 -->
## 1. Contratos y fixtures

- [x] 1.1 [M] Agregar DTOs 1.1 y extender aditivamente capabilities/items. Área: DTOs y fixtures. Origen: D-01, RQ-01. Verificación: fixtures 1.0 siguen válidos.
- [x] 1.2 [S] Ampliar fixtures SII saneados con cero/una/múltiples inscripciones y opcionales. Área: `Tests/Fixtures/Workflow/ImportarServicioWeb/`. Origen: D-02, RQ-02. Verificación: sin secretos, URLs reales ni PII.

## 2. Catálogo

- [x] 2.1 [M] Crear puerto y `MySqlImportDocumentTypeCatalogRepository` en Radicación. Área: Modelo/Infrastructure. Origen: D-03, RQ-03. Verificación: `SELECT` parametrizado, ID TRD, `OBLIGATORIO`, orden y ambigüedad.
- [x] 2.2 [M] Integrar catálogo en `ResolveCapabilities` con contexto de servidor. Área: ASMX moderno. Origen: D-03, RQ-03. Verificación: contexto inválido no consulta repositorio/SII.

## 3. Presentación y estado

- [x] 3.1 [M] Extender `MapQuery` con metadatos `Code/Label/Value`. Área: mapper SII. Origen: D-02, RQ-02. Verificación: correlación por `ExternalKey`, límites y exclusiones.
- [x] 3.2 [M] Implementar `MySqlImportItemStatusRepository`. Área: repositorios modernos. Origen: D-04, RQ-04. Verificación: ausente, confirmado, parcial, fallido, incierto y duplicado.
- [x] 3.3 [M] Implementar `ImportItemPresentationService` y tabla de estados/acciones. Área: Services. Origen: D-04, RQ-04. Verificación: solo evidencia confirmada produce `Importado`.
- [x] 3.4 [M] Enriquecer localmente tras la única llamada de `QueryItems` y paginar compatible. Área: ASMX. Origen: D-05, RQ-05. Verificación: una llamada al proveedor y cero por fila/filtro/página.
- [x] 3.5 [S] Filtrar sellos SII por `tipoanexo=505` en listado y resolución física. Área: mapper/proveedor SII. Origen: D-02, RQ-02. Verificación: 518 no se lista y una clave fabricada se rechaza.

## 4. Validación local

- [x] 4.1 [S] Registrar nuevos VB exactamente una vez en el proyecto. Área: `.vbproj`. Origen: D-01, RQ-01. Verificación: estructura y orden válidos.
- [x] 4.2 [M] Crear/ejecutar pruebas `node:test` de presentación, catálogo y conteo. Área: `Tests/importar-servicio-web-*.test.cjs`. Origen: D-05, RQ-05. Cobertura: D-01,D-02,D-03,D-04,RQ-01,RQ-02,RQ-03,RQ-04. Verificación: suite real registrada.
- [x] 4.3 [M] Compilar .NET Framework 4.6.1. Área: `.vbproj`. Origen: D-01, RQ-01. Verificación: MSBuild código 0 y advertencias diferenciadas.

## 5. Documentación y E2E

- [x] 5.1 [M] Crear paquete técnico DOC-68 con inventario, contratos, SQL y diagramas. Área: `docs/Architecture/Workflow/ImportarServicioWeb/DOC-68-listado-enriquecido-catalogo-tipologias/`. Origen: D-01, RQ-01. Cobertura: D-02,D-03,D-04,D-05,RQ-02,RQ-03,RQ-04,RQ-05. Verificación: `00-Indice.md` a `07-Metadata.md` consistentes.
- [x] 5.2 [M] Extender E2E existente para listado/catálogo/conteo. Área: `tools/e2e`. Origen: D-05, RQ-05. Verificación: sin runner, login o configuración paralelos; controles `SELECT`.
- [x] 5.3 [L] Ejecutar E2E autorizada MERCANTIL/ESAL/RUP disponibles. Área: `tools/e2e`. Origen: D-05, RQ-05. Cobertura: D-02,D-03,D-04,RQ-02,RQ-03,RQ-04. Verificación: autorización, evidencia saneada y gate restaurado; sin autorización se bloquea.
- [x] 5.4 [S] Validar OpenSpec/OPSXJ sobre SHA final. Área: OpenSpec/.opsxj. Origen: D-01, RQ-01. Cobertura: D-02,D-03,D-04,D-05,RQ-02,RQ-03,RQ-04,RQ-05. Verificación: refinement, OpenSpec strict y opsxj pasan.
