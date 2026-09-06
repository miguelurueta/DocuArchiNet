## Context

DOC-51: SERVICIO-PROVEEDOR

## Jira Details

> # Prompt backend 02 — Clientes HTTP asíncronos y seguridad
> 
> Implementa la frontera HTTP externa sobre los contratos del Prompt backend 01. Conserva exactamente el formato real exigido por cada operación del proveedor.
> 
> Este cliente comunica backend con proveedores externos; no es el endpoint consumido directamente por el navegador. La operación moderna `GetPreview` será mediada por backend conforme al contrato compartido.
> 
> ## Objetivo
> 
> Reemplazar el transporte bloqueante del recorrido modernizado por clientes tipados, asíncronos, cancelables y seguros, sin alterar silenciosamente JSON, formulario, encabezados o serialización.
> 
> ## Rutas canónicas de implementación
> 
> ```txt
> Infrastructure/Workflow/ImportarServicioWeb/Http/
> ├── ExternalImportHttpTransport.vb
> ├── ExternalImportHttpResponseValidator.vb
> ├── ExternalImportHttpErrorMapper.vb
> └── ExternalImportHttpClientFactory.vb
> 
> Tests/
> ├── importar-servicio-web-http-contract.test.cjs
> ├── importar-servicio-web-http-security.test.cjs
> └── importar-servicio-web-http-cancellation.test.cjs
> 
> Tests/Fixtures/Workflow/ImportarServicioWeb/http-v1/
> ├── token-success.json
> ├── query-success.json
> ├── preview-success.json
> ├── invalid-response.json
> └── oversized-response.json
> ```
> 
> - Los puertos consumidos permanecen en `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`; no duplicarlos en infraestructura.
> - Los DTO públicos permanecen en `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`.
> - `ExternalImportHttpTransport.vb` encapsula únicamente transporte asíncrono común; no contiene reglas SII.
> - El cliente concreto SII se implementará posteriormente en `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiExternalImportProviderClient.vb` mediante Backend 06.
> - Agregar los `.vb` nuevos al `.vbproj` sin mover ni sustituir entradas existentes.
> 
> No crear transporte moderno en `Integracionccv/Class_ClassResfull.vb`, `webservice/`, `App_Code/` o `ServiciosIntegracion/`. Esos archivos legacy son solo referencia y permanecen sin cambios.
> 
> ## Ruta documental obligatoria
> 
> ```txt
> docs/Architecture/Workflow/ImportarServicioWeb/SCRUMCORE-000-clientes-http-asincronos-seguridad/
> ```
> 
> Sustituir `SCRUMCORE-000` por el ticket real. Crear allí el paquete canónico `00-Indice.md` a `07-Metadata.md` y `Diagramas/` definido en Backend 01. `03-ContratoUploadYMapping.md` documentará transporte, serialización, timeout, cancelación, límites y errores; no describe almacenamiento documental.
> 
> ## Investigación obligatoria
> 
> - Caracterizar con fixtures saneados los contratos de token, consulta, preview y descarga de SII.
> - Determinar timeout, tamaño máximo, tipos de contenido y política TLS/certificados por ambiente.
> - Verificar si ASMX puede propagar `Task`; si no, documentar y crear una frontera moderna paralela sin esperas bloqueantes.
> 
> ## Implementa
> 
> - Cliente equivalente a `IExternalImportProviderClient` con capacidades, consulta, preview y descarga asíncronos.
> - `CancellationToken` y timeout por operación, sin mutar configuración compartida durante una solicitud.
> - Validación de estado HTTP, tipo de contenido, tamaño y deserialización.
> - Soporte de preview mediado sin exponer URL externa, token, ruta física o respuesta cruda al frontend.
> - Traducción tipada de timeout, cancelación, indisponibilidad, respuesta inválida y acceso denegado.
> - Mensajes seguros y `correlationId`, sin registrar secretos ni payloads sensibles.
> - Servidor HTTP simulado para pruebas de token, respuesta válida, error, timeout, cancelación, contenido inválido y exceso de tamaño.
> 
> ## Restricciones
> 
> - Agregar los clientes modernos en paralelo; no sustituir ni modificar el transporte consumido por el recorrido vigente.
> - No modificar `AlmacenaDocumentoTareaWorkflow(...)` ni ninguna capa de almacenamiento documental.
> - No usar `.Result`, `.Wait()`, `GetAwaiter().GetResult()` ni `Task.Run` para adaptar I/O.
> - No instalar callbacks globales que acepten cualquier certificado.
> - No cambiar JSON por `application/x-www-form-urlencoded` ni viceversa sin evidencia contractual.
> - No ejecutar llamadas reales a SII.
> - No paralelizar elementos ni modernizar en esta entrega las capas síncronas de base de datos o PDF.
> 
> ## Aceptación
> 
> - No existe I/O HTTP bloqueante nuevo en el recorrido modernizado.
> - Cancelación y timeout producen resultados distintos y deterministas.
> - La configuración del cliente no se modifica concurrentemente.
> - Los logs y errores no contienen token, credenciales, URL sensible, ruta física ni respuesta cruda.
> - Las pruebas no utilizan red externa.
> 
> ## Trazabilidad
> 
> Exploración backend: secciones 7, 8, 17, 18 y 19; hallazgos B-04, B-05 y B-06; preguntas abiertas 1 y 9.
> 
> ## Correcciones opsxj:prompt-review
> 
> Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.
> 
> ## Rol esperado
> Definir el rol tecnico esperado para ejecutar el ticket.
> 
> ## Objetivo
> Describir el objetivo funcional y tecnico verificable.
> 
> ## Restricciones criticas
> - No introducir cambios fuera del alcance declarado.
> - No romper comportamiento existente ni contratos publicos.
> 
> ## Criterios de aceptacion
> - El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.
> 
> ## Contexto obligatorio
> Leer `Integracionccv/Class_ClassResfull.vb`, `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`, `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`, `GestionDocumental-Docuarchi.net.vbproj` y los fixtures contractuales de Backend 01. `Class_ClassResfull.vb` es referencia de contrato externo; no modificarlo.
> 
> ## Pruebas obligatorias
> Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.
> 
> ## Documentacion tecnica
> Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, con enlaces relativos y evidencia coherente con transporte, seguridad y pruebas implementadas.
> 
> ## Entregable final
> Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.
> 
> ## Requisitos positivos
> - Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
> - Mantener la integracion sobre los puntos de extension existentes del repo.
> - Dejar evidencia de pruebas y documentacion tecnica actualizada.
> 
> Agregar regla para [CONTRACT_DETAIL_REQUIRED]: Props, callbacks, eventos, request/response, payloads o tipos documentados.
> 
> Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.
> 
> Exigir pruebas unitarias/focales con Vitest o Testing Library segun el alcance.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. Las decisiones funcionales y tecnicas se completan durante `opsxj:refine`; no se inyectan politicas de otro perfil tecnologico.


## Risks / Trade-offs

- El refinamiento debe identificar compatibilidad, riesgos y limites del modulo afectado antes de iniciar cambios.

## Migration Plan

1. Completar y aprobar `refinement.md` antes de marcar tareas de implementacion.
2. Sincronizar cada decision con design, spec y tasks mediante `opsxj:refine --sync`.

## Open Questions

- TBD
