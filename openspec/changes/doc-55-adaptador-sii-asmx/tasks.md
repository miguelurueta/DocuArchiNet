<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->
## 1. Investigación y contratos

- [x] 1.1 [S] Inventariar firmas ASMX, consumidores y códigos legacy que DOC-55 debe preservar. Área/archivos: `webservice/WebService_integracion_sii.asmx.vb`, `webservice/WebServiceGaExpediente.asmx.vb`, `Integracionccv/`, `js/java_general/JSProgresBar.js`. Origen: D-04, RQ-04. Verificación: tabla de consumidores y traducciones incorporada a la documentación canónica.
- [x] 1.2 [M] Caracterizar respuestas saneadas de token, consulta, recurso y constancia SII y fijar la clave externa estable. Área/archivos: `Tests/Fixtures/Workflow/ImportarServicioWeb/sii-v1/`. Origen: D-02, RQ-02. Verificación: seis fixtures válidos sin secretos ni datos reales.

## 2. Adaptador SII

- [x] 2.1 [S] Registrar `INTEGRACIONSII` como cliente asíncrono exacto sin fallback, preservando el registro síncrono. Área/archivos: `Services/Workflow/ImportarServicioWeb/RegistroClientesProveedoresImportacion.vb`, `Modelo/Workflow/ImportarServicioWeb/`, `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-10, RQ-10. Verificación: prueba focal resuelve SII, rechaza desconocidos y prohíbe bloqueo síncrono.
- [x] 2.2 [M] Implementar el cliente externo SII sobre el transporte común. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb`. Origen: D-02, RQ-02. Verificación: prueba con transporte falso cubre éxito, límite, cancelación y error saneado.
- [x] 2.3 [M] Mapear capacidades e items SII al contrato común. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportContractMapper.vb`. Origen: D-03, RQ-03. Verificación: fixtures producen campos normalizados y clave externa estable.
- [x] 2.4 [M] Implementar `ResolveCapabilities`, `QueryItems` y preparación documental del proveedor. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb`. Origen: D-01, RQ-01. Verificación: prueba del proveedor cubre las operaciones y ausencia de fallback.
- [x] 2.5 [M] Implementar traducción bidireccional legacy localizada. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiLegacyResultAdapter.vb`. Origen: D-04, RQ-04. Verificación: matriz `YES`/`CTRL`/`CTRLRETURN`/`dato_lista` queda cubierta sin símbolos legacy en DTO comunes.

## 3. Preview y frontera ASMX

- [ ] 3.1 [M] Construir la respuesta de preview con metadatos y encabezados seguros. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiPreviewResponseFactory.vb`. Origen: D-06, RQ-06. Verificación: pruebas rechazan vencimiento, tipo/tamaño inválido y contexto ajeno.
- [ ] 3.2 [S] Crear el descriptor del ASMX moderno y registrarlo en el proyecto. Área/archivos: `webservice/WebServiceImportarServicioWebModern.asmx`, `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-05, RQ-05. Verificación: descriptor referencia la clase code-behind correcta y compila.
- [ ] 3.3 [M] Implementar la frontera ASMX delgada para capacidades, consulta y preview. Área/archivos: `webservice/WebServiceImportarServicioWebModern.asmx.vb`. Origen: D-05, RQ-05. Verificación: prueba estructural confirma delegación y ausencia de SQL/filesystem/`ClassAlmacenamiento`.
- [ ] 3.4 [S] Aplicar el gate de servidor antes de cualquier dependencia moderna. Área/archivos: `webservice/WebServiceImportarServicioWebModern.asmx.vb`. Origen: D-07, RQ-07. Verificación: gate apagado retorna `FEATURE_DISABLED` y los dobles registran cero llamadas.

## 4. Orquestación y compatibilidad

- [x] 4.1 [M] Generar el comando documental SII normalizado para el orquestador. Área/archivos: `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb`, `SiiImportContractMapper.vb`. Origen: D-08, RQ-08. Verificación: prueba prueba correlación, tarea, expediente, tipo documental e identidad externa.
- [ ] 4.2 [S] Verificar que la ejecución usa exclusivamente el puerto moderno de almacenamiento. Área/archivos: `Services/Workflow/ImportarServicioWeb/ImportServiceOrchestrator.vb`, `Infrastructure/Workflow/ImportarServicioWeb/Storage/LegacyImportDocumentStorageAdapter.vb`. Origen: D-08, RQ-08. Verificación: prueba focal demuestra una sola invocación idempotente.
- [ ] 4.3 [S] Registrar todos los archivos nuevos sin alterar inclusiones legacy. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-09, RQ-09. Verificación: build resuelve cada clase y ASMX nuevo.

## 5. Pruebas y evidencia

- [x] 5.1 [M] Cubrir resolución, consulta y preparación del proveedor SII. Área/archivos: `Tests/importar-servicio-web-sii-provider.test.cjs`. Origen: D-01, RQ-01. Verificación: suite focal aprobada sin red.
- [x] 5.2 [M] Cubrir mapping contractual contra fixtures saneados. Área/archivos: `Tests/importar-servicio-web-sii-contract-mapping.test.cjs`, `Tests/Fixtures/Workflow/ImportarServicioWeb/sii-v1/`. Origen: D-03, RQ-03. Verificación: todos los fixtures normalizan determinísticamente.
- [x] 5.3 [M] Cubrir compatibilidad legacy y ausencia de contaminación contractual. Área/archivos: `Tests/importar-servicio-web-sii-legacy-compatibility.test.cjs`. Origen: D-04, RQ-04. Verificación: tabla de códigos cubierta y búsqueda estructural limitada al adaptador.
- [ ] 5.4 [M] Cubrir preview, autorización, gate y mediación ASMX. Área/archivos: `Tests/importar-servicio-web-preview-mediation.test.cjs`. Origen: D-06, RQ-06. Verificación: escenarios seguro, vencido, ajeno, sobredimensionado y gate apagado aprobados.
- [ ] 5.5 [S] Ejecutar todas las pruebas focales de ImportarServicioWeb. Área/archivos: `Tests/importar-servicio-web-*.test.cjs`. Origen: D-09, RQ-09. Verificación: comando y conteo final registrados como evidencia unit.
- [ ] 5.6 [M] Compilar el proyecto VB.NET y registrar advertencias heredadas. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj`. Origen: D-09, RQ-09. Verificación: MSBuild finaliza con código 0.
- [ ] 5.7 [S] Confirmar por diff que las superficies legacy protegidas no cambiaron. Área/archivos: `webservice/WebService_integracion_sii.asmx*`, `webservice/WebServiceGaExpediente.asmx*`, `Integracionccv/`, `ServiciosIntegracion/`, `workflow/ClassAlmacenamiento.vb`, `js/java_general/JSProgresBar.js`. Origen: D-09, RQ-09. Verificación: `git diff --exit-code` focal no reporta cambios.

## 6. Documentación y cierre

- [ ] 6.1 [M] Crear el paquete técnico canónico 00-07 y diagramas. Área/archivos: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-55-adaptador-sii-compatibilidad-asmx/`. Origen: D-09, RQ-09. Verificación: índice enlaza contratos, matriz legacy/moderno, preview, gate, compatibilidad y evidencia.
- [ ] 6.2 [S] Documentar rollback y comprobar que el gate queda desactivado. Área/archivos: paquete técnico canónico y configuración aplicable. Origen: D-07, RQ-07. Verificación: evidencia manual registra gate false y listas de usuarios/grupos vacías.
- [ ] 6.3 [S] Validar OpenSpec y gobierno OPSXJ. Área/archivos: `openspec/changes/doc-55-adaptador-sii-asmx/`, `.opsxj/evidence/DOC-55.json`. Origen: D-09, RQ-09. Verificación: validación strict y `opsxj:validate` finalizan PASS.
