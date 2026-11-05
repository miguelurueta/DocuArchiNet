<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->

# Tareas atómicas DOC-116 — corte de implementación local

20/20 tareas completadas y escenarios aceptados con evidencia: EXP01/EXP03 reales PASS; EXP02 cliente y ventanas internas de laboratorio autorizadas PASS. DLL original restaurada y dos respaldos verificados. Entrega/publicación y revisión formal de cierre pendientes; intento 41 se conserva EN_CURSO sin recreación.

- [x] 1.1 [S] Fijar huellas y diferencias de las rutas locales de creación y herramientas. Área/archivos: `fuente local y manifiesto de evidencia`. Origen: D-01, RQ-01. Verificación: Ninguna diferencia de main sobrescribe la copia local.

- [x] 1.2 [S] Mapear contrato y replay de creación al arnés existente. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs CreationCommandCases/CreationAdapterCases/CoordinatedCreationCases`. Origen: D-02, RQ-02. Verificación: Cada invariante identifica prueba y procedencia; no se duplica escritor.

- [x] 1.3 [S] Clasificar negativos de catálogo y relación ya acreditados. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs CreationCatalogCases/KnownExpedientRelationCases/KnownExpedientClassificationCases`. Origen: D-02, RQ-02. Verificación: Homónimos, código, límites y nivel movido tienen referencia concreta o brecha explícita.

- [x] 1.4 [M] Diseñar el recorrido acotado EXP01 con selección real A→B. Área/archivos: `tools/e2e/tests/doc116-expedientes-importacion.spec.cjs (propuesta)`. Origen: D-03, RQ-03. Verificación: Las dos raíces se verifican en A; acciones documentales de almacenamiento quedan excluidas.

- [x] 1.5 [M] Separar las tres ventanas de fallo de EXP02 en la matriz. Área/archivos: `tools/e2e/tests/support/doc116-expedientes-importacion.cjs (propuesta); tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-04, RQ-04. Verificación: Descarte cliente, fallo local y fallo interno real no comparten un PASS.

- [x] 2.1 [M] Comprobar la viabilidad de la inyección interna real sin hook productivo. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.vb constructor Friend; Infrastructure/ProduccionDocumental/ImportacionCarpetas/AdaptadorExpedienteImportacion.Legacy.vb`. Origen: D-04, RQ-04. Verificación: Un procedimiento concreto autoriza solo el punto de fallo o declara dependencia que bloquea ese subcaso.

- [x] 2.2 [S] Precisar la concesión efectiva y no propiedad exigidas por EXP03. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/ImportacionCarpetasContextAdapter.vb ResolverDestino`. Origen: D-05, RQ-05. Verificación: Preflight SELECT descarta propietario y permisos de nivel inexistentes/ambiguos.

- [x] 2.3 [M] Especificar revocación y restauración exacta del permiso de nivel. Área/archivos: `tools/e2e/scripts/support/doc110-permission-db.ps1 y tools/e2e/tests/support/doc110-permission-revocation.cjs`. Origen: D-05, RQ-05. Verificación: Contrato cerrado identifica fila/columna, valor previo, diario, restauración y readback sin usar cuenta de control para escribir.

- [x] 2.4 [M] Agregar soporte de expedientes con presupuestos y controles SELECT. Área/archivos: `tools/e2e/tests/support/doc116-expedientes-importacion.cjs (propuesta)`. Origen: D-06, RQ-06. Verificación: Operaciones/documentos prohibidos se rechazan; referencias se conservan antes de comprobar respuesta.

- [x] 2.5 [S] Agregar fixtures sintéticas de escenarios y alcance. Área/archivos: `tools/e2e/fixtures/doc116-expedientes-cases.json (propuesta)`. Origen: D-06, RQ-06. Verificación: No contienen secretos/IDs asumidos y cada presupuesto delimita plan/lote/recepción/creación.

- [x] 3.1 [S] Registrar selector delgado en el paquete E2E existente. Área/archivos: `tools/e2e/scripts/run-doc116-expedientes-importacion.cjs (propuesta); tools/e2e/package.json`. Origen: D-06, RQ-06. Verificación: --check valida recursos sin login ni efectos y delega autenticación/consola compartidas.

- [x] 3.2 [M] Completar aserciones EXP01 de destino y replay. Área/archivos: `tools/e2e/tests/doc116-expedientes-importacion.spec.cjs (propuesta)`. Origen: D-03, RQ-03. Verificación: SELECT demuestra IDs/relaciones en A, B sin efectos y repetición sin expediente adicional.

- [x] 3.3 [M] Completar aserciones EXP02 por punto de observación. Área/archivos: `tools/e2e/tests/doc116-expedientes-importacion.spec.cjs (propuesta); tests/ImportacionCarpetasBehaviorTests.cs`. Origen: D-04, RQ-04. Verificación: Un caso no viable se informa NO_EJECUTADO; incertidumbre nunca habilita otra creación.

- [x] 3.4 [M] Completar EXP03 con restauración comprobada. Área/archivos: `tools/e2e/tests/doc116-expedientes-importacion.spec.cjs (propuesta); soporte de permiso acotado`. Origen: D-05, RQ-05. Verificación: Creación rechazada y cero nuevas relaciones; valor original y diario recuperados incluso ante fallo.

- [x] 3.5 [M] Validar regresión local de contratos y consumidores de soporte. Área/archivos: `tests/ImportacionCarpetasBehaviorTests.cs; tests/doc110-perfil-importacion-behavior.test.cjs; suites de los helpers modificados`. Origen: D-06, RQ-06. Verificación: Arnès Framework y contratos pertinentes pasan; si solo cambia documentación no se ejecuta integración.

- [x] 4.1 [S] Preparar recursos y autorización específica por escenario. Área/archivos: `tools/e2e/fixtures/doc116-expedientes-cases.json (propuesta); herramientas interactivas existentes`. Origen: D-07, RQ-07. Verificación: Cuenta/ambiente/niveles/publicación/presupuesto/fallo/permiso autorizados antes de cualquier efecto real.

- [x] 4.2 [M] Ejecutar EXP01 autorizado y conservar evidencia. Área/archivos: `tools/e2e/tests/doc116-expedientes-importacion.spec.cjs (propuesta); Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-exp01-resultados.md (propuesta)`. Origen: D-03, RQ-03. Verificación: A→B y dos raíces en A acreditados con lectura independiente; no almacenamiento definitivo.

- [x] 4.3 [M] Ejecutar únicamente variantes viables y autorizadas de EXP02. Área/archivos: `tools/e2e/tests/doc116-expedientes-importacion.spec.cjs (propuesta); Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-exp02-resultados.md (propuesta)`. Origen: D-04, RQ-04. Verificación: Se identifica punto de fallo exacto; no se repite efecto incierto ni se atribuye cobertura no ejecutada.

- [x] 4.4 [M] Ejecutar EXP03 autorizado con recuperación verificada. Área/archivos: `tools/e2e/tests/doc116-expedientes-importacion.spec.cjs (propuesta); Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-exp03-resultados.md (propuesta)`. Origen: D-05, RQ-05. Verificación: Permiso de nivel efectivo retirado/restaurado y cero nuevas creaciones demostrados por SELECT.

- [x] 4.5 [S] Consolidar matriz, dossier y validación de entrega. Área/archivos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-expedientes-matriz.md (propuesta); Doc/Tecnica/Opsxj/doc-116-produccion-importacion-carga-carpeta-expediente`. Origen: D-07, RQ-07. Verificación: Cada criterio tiene fuente/SHA/estado/comando; OpenSpec y gobernanza distinguen límites y bloqueos.

Evidencia 4.2: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-exp01-resultados.md`. EXP01 no autoriza recursos ni revocaciones de los otros escenarios.

Evidencia 4.3: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-exp02-resultados.md`. Solo variante cliente viable/autorizada ejecutada; fallo interno explícitamente NO_EJECUTADO y pendiente de composición autenticada.

Evidencia 4.1/4.4: autorizaciones específicas EXP01/EXP02-cliente/EXP03 y `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-exp03-resultados.md`. EXP02 interno no compuesto no autoriza efectos ni se marca ejecutado. Diario de permiso 629 RESTORED; sin repetición del escenario.

Evidencia 4.5: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-verificacion-final-20261105.md` y JSON del mismo nombre; matriz/dossier consolidados. Hallazgo crítico DOC116-VERIFY-01 impide aceptación global/archivo/Jira Listo hasta resolver la variante interna de EXP02 o revisar explícitamente el alcance aprobado.

Resolución del hallazgo crítico DOC116-VERIFY-01: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc116-exp02-interno-resultados.md`. La instrumentación fue exclusiva de laboratorio y se retiró restaurando el DLL original; no se afirma pérdida de paquetes de red ni reconciliación del intento 41.
