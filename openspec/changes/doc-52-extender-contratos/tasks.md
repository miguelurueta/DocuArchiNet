# Tareas atómicas — DOC-52

## 1. Contratos y modelos

- [x] 1.1 [M] Extender DTO de preflight e intención con contexto, requisitos, elementos y resultado idempotente. Área/archivos: `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`. Origen: D-01, RQ-01. Verificación: prueba contractual confirma campos, defaults y serialización v1.
- [x] 1.2 [M] Modelar snapshot, elemento de intención, huella, estados y resultado de creación. Área/archivos: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebModels.vb`. Origen: D-03, RQ-03. Verificación: individual y múltiple usan las mismas clases y cada elemento conserva identidad/tarea.
- [x] 1.3 [M] Extender puertos de validación, repositorio y guard de concurrencia sin duplicarlos. Área/archivos: `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`. Origen: D-01, RQ-01. Verificación: interfaces compilan y no contienen SQL/web/session.

## 2. Preflight

- [x] 2.1 [M] Implementar revalidación autoritativa de contexto y selección. Área/archivos: `Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb`. Origen: D-02, RQ-02. Verificación: dobles cubren usuario, permiso, tarea, ruta, trámite, proveedor y selección inválidos.
- [x] 2.2 [M] Construir requisitos y comandos por elemento sin inferir contexto del primero. Área/archivos: `Services/Workflow/ImportarServicioWeb/ServicioPreflightImportacion.vb`. Origen: D-03, RQ-03. Verificación: fixture múltiple conserva destino/identidad individual.
- [ ] 2.3 [S] Demostrar que preflight no invoca repositorio, Session, caché ni almacenamiento. Área/archivos: `Tests/importar-servicio-web-preflight.test.cjs`. Origen: D-02, RQ-02. Verificación: prueba focal y auditoría fuente pasan.

## 3. Intención e idempotencia

- [x] 3.1 [M] Implementar serialización canónica ordenada y no ambigua del snapshot. Área/archivos: `Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb`. Origen: D-04, RQ-04. Verificación: reordenamiento equivalente produce la misma representación.
- [x] 3.2 [S] Calcular huella SHA-256 en minúsculas y cultura invariante. Área/archivos: `Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb`. Origen: D-04, RQ-04. Verificación: fixture equivalente coincide y cambio autoritativo difiere.
- [ ] 3.3 [M] Orquestar validación, guard, creación/reutilización/conflicto y mapeo DTO. Área/archivos: `Services/Workflow/ImportarServicioWeb/ServicioIntencionImportacion.vb`. Origen: D-05, RQ-05. Verificación: pruebas retornan mismo ID para equivalencia y `IDEMPOTENCY_CONFLICT` para diferencia.

## 4. Persistencia

- [ ] 4.1 [M] Crear script versionado de cabecera, requisitos, elementos, claves únicas e índices. Área/archivos: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-52-preflight-intencion-idempotencia/Sql/001-create-import-intents.sql`. Origen: D-07, RQ-07. Verificación: DDL no referencia tablas legacy y define unicidad idempotente.
- [ ] 4.2 [M] Implementar lectura y escritura agregada parametrizada. Área/archivos: `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb`. Origen: D-08, RQ-08. Verificación: dobles ADO.NET comprueban parámetros y round-trip completo.
- [ ] 4.3 [M] Implementar transacción para reserva, agregado, reutilización y conflicto ante duplicado. Área/archivos: `Infrastructure/Repositories/Workflow/ImportarServicioWeb/MySqlImportIntentRepository.vb`. Origen: D-05, RQ-05. Verificación: commit solo con agregado completo; rollback ante error.
- [ ] 4.4 [M] Implementar guard cooperativo con nombre hash y liberación garantizada. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/MySqlImportIntentConcurrencyGuard.vb`. Origen: D-06, RQ-06. Verificación: prueba simula adquisición, contención y dispose.
- [ ] 4.5 [S] Registrar cuatro archivos VB nuevos en el proyecto. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-07, RQ-07. Verificación: una entrada Compile por archivo sin mover existentes.

## 5. Pruebas

- [ ] 5.1 [S] Crear cuatro fixtures de intención saneados. Área/archivos: `Tests/Fixtures/Workflow/ImportarServicioWeb/intents-v1/*.json`. Origen: D-09, RQ-09. Verificación: JSON válido, schema v1 y sin datos reales.
- [ ] 5.2 [M] Probar preflight puro, cardinalidad y requisitos. Área/archivos: `Tests/importar-servicio-web-preflight.test.cjs`. Origen: D-02, RQ-02. Verificación: prueba focal pasa.
- [ ] 5.3 [M] Probar equivalencia, reutilización, conflicto y SQL parametrizado. Área/archivos: `Tests/importar-servicio-web-intent-idempotency.test.cjs`. Origen: D-05, RQ-05. Verificación: prueba focal pasa.
- [ ] 5.4 [M] Probar constraint, transacción y guard ante carreras. Área/archivos: `Tests/importar-servicio-web-intent-concurrency.test.cjs`. Origen: D-06, RQ-06. Verificación: prueba focal pasa.

## 6. Validación y documentación

- [ ] 6.1 [S] Documentar precondiciones, aplicación y rollback SQL manual. Área/archivos: paquete DOC-52 `Sql/README.md`, `002-rollback-import-intents.sql`. Origen: D-07, RQ-07. Verificación: orden de rollback y advertencia de autorización explícitos.
- [ ] 6.2 [M] Crear paquete canónico 00-07 y diagramas. Área/archivos: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-52-preflight-intencion-idempotencia/`. Origen: D-09, RQ-09. Verificación: enlaces relativos, modelo, transacción, concurrencia y evidencia completos.
- [ ] 6.3 [M] Ejecutar suite focal y build VB.NET. Área/archivos: tests DOC-50/51/52 y proyecto. Origen: D-09, RQ-09. Verificación: comandos y resultados registrados.
- [ ] 6.4 [S] Auditar ausencia de SQL concatenado y cambios legacy/ambientales. Área/archivos: diff DOC-52. Origen: D-07, RQ-07. Verificación: búsquedas y diff confirman parámetros, sin migración ejecutada, E2E/gate no aplicables.
- [ ] 6.5 [S] Validar OpenSpec y refinamiento OPSXJ. Área/archivos: cambio DOC-52. Origen: D-01, RQ-01. Verificación: validación estricta y `opsxj:refine DOC-52` pasan.
