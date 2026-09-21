# Arquitectura

`WebServiceImportarServicioWebModern.GetPreview(GetPreviewRequestDto)` valida gate, sesión y tarea, resuelve el anexo mediante `SiiImportProvider.GetPreviewContentAsync` y realiza una única descarga en `SiiExternalImportProviderClient.GetPreviewContentAsync`. Los bytes se entregan a `ImportPreviewDescriptorService.Create`, que genera 32 bytes aleatorios, persiste solo su SHA-256 y devuelve el descriptor Base64URL.

`ImportarServicioWebPreview.ProcessRequest(HttpContext)` es la única frontera binaria. Reutiliza `WorkflowPreviewSessionContextGate`, construye la autoridad exclusivamente con sesión servidor y delega en `ImportPreviewContentService`. El handler no compone ni llama un cliente SII.

La tabla compartida permite que creación y canje ocurran en nodos diferentes. HEAD consulta metadatos; GET reclama atómicamente y luego transmite el BLOB por bloques de 64 KiB.

Fuentes: `webservice/WebServiceImportarServicioWebModern.asmx.vb`, `workflow/ImportarServicioWebPreview.ashx.vb`, `Infrastructure/Workflow/ImportarServicioWeb/Preview/*.vb`, `Infrastructure/Workflow/ImportarServicioWeb/Sii/*.vb`.
