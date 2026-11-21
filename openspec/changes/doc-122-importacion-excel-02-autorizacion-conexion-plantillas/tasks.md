<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
# Tareas DOC-122

- [x] 1.1 [S] Concretar diseño y trazabilidad. Área/archivos: `openspec/changes/doc-122-importacion-excel-02-autorizacion-conexion-plantillas/`. Origen: D-07, RQ-07. Verificación: OpenSpec estricto.
- [x] 1.2 [M] Añadir modelos y DTO. Área/archivos: `Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb; DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb`. Origen: D-03, RQ-03. Verificación: serialización tipada sin secretos.
- [x] 1.3 [M] Validar autoridad vigente en RA. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelContextAdapter.vb`. Origen: D-01, RQ-01. Verificación: inactivo, revocado y empresa ajena rechazados.
- [x] 1.4 [S] Integrar indicador de presentación. Área/archivos: `Gestion/Class_remit_dest_interno_perfil_produccion.vb`. Origen: D-01, RQ-01. Verificación: default0 y permiso exacto1.
- [x] 1.5 [M] Resolver conexión DA del módulo reutilizando el resolver compartido. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb`. Origen: D-02, RQ-02. Verificación: vínculos manipulados rechazados.
- [x] 1.6 [M] Implementar reglas y huella canónica. Área/archivos: `Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb`. Origen: D-05, RQ-05. Verificación: vectores, flags y rutas históricas.
- [x] 1.7 [M] Consultar plantillas paginadas. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb`. Origen: D-03, RQ-03. Verificación: filtro por conexión y paginación.
- [x] 1.8 [M] Guardar cabecera y mapeos atómicamente. Área/archivos: `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb`. Origen: D-04, RQ-04. Verificación: rollback y un ganador por revisión.
- [x] 1.9 [M] Añadir guardia de solicitud Excel. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb`. Origen: D-06, RQ-06. Verificación: origen, token y contexto forjados rechazados.
- [x] 1.10 [M] Componer ASMX y registrar proyecto. Área/archivos: `webservice/WebServiceImportacionExcel.asmx.vb; GestionDocumental-Docuarchi.net.vbproj`. Origen: D-06, RQ-06. Verificación: build aislado y reflexión SOAP.
- [x] 1.11 [M] Ejecutar pruebas nativas y regresión. Área/archivos: `tests/ImportacionExcelBehaviorTests.cs`. Origen: D-07, RQ-07. Verificación: suite sintética PASS.
- [x] 1.12 [M] Preparar E2E reutilizando autenticación. Área/archivos: `tools/e2e/tests/importacion-excel.spec.cjs`. Origen: D-07, RQ-07. Verificación: descubrimiento sin autenticación y guardas explícitas.
- [x] 1.13 [S] Documentar configuración y operación. Área/archivos: `Doc/Actualizacion/Importacion/Diseno/02-autorizacion-plantillas-doc122.md`. Origen: D-02, RQ-02. Verificación: guía sin secretos ni aplicación SQL.
- [ ] 1.14 [M] Acreditar integración real autorizada. Área/archivos: `Doc/Actualizacion/Importacion/Validacion`. Origen: D-07, RQ-07. Verificación: enrutamiento DA del módulo y E2E de plantilla descartable verificados.
