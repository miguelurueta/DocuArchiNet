## Why

LISTA-PREVIEW-INCRIPCIONES. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-69.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # Prompt backend 10 — Stream y descarga segura de preview
> 
> Actúa como arquitecto y desarrollador senior de ASP.NET WebForms/VB.NET. Parte del descriptor seguro implementado en DOC-67 y crea un cambio OpenSpec nuevo.
> 
> Completa el recorrido de `GetPreview`: el descriptor publicado no es contenido ni URL y debe poder canjearse mediante una frontera HTTP segura, temporal y autorizada.
> 
> ## Objetivo
> 
> Permitir visualización inline o descarga temporal de un recurso SII sin exponer URL externa, token, ruta física, credenciales ni respuesta cruda, y sin descargar el mismo recurso más de una vez por consumo.
> 
> ## Rutas canónicas de implementación
> 
> ```txt
> Infrastructure/Workflow/ImportarServicioWeb/Preview/
> ├── ImportPreviewDescriptorService.vb
> ├── ImportPreviewDescriptorRepository.vb
> └── ImportPreviewContentService.vb
> 
> workflow/
> ├── ImportarServicioWebPreview.ashx
> └── ImportarServicioWebPreview.ashx.vb
> 
> Tests/
> ├── importar-servicio-web-preview-content.test.cjs
> ├── importar-servicio-web-preview-authorization.test.cjs
> └── importar-servicio-web-preview-single-fetch.test.cjs
> ```
> 
> - `GetPreview` sigue devolviendo metadatos y un `DescriptorId` opaco.
> - El handler es la única frontera que entrega bytes y debe reutilizar autenticación/contexto Workflow.
> - Registrar archivos nuevos en el `.vbproj`; no crear un segundo cliente SII ni servir contenido desde JavaScript.
> 
> ## Ruta documental obligatoria
> 
> ```txt
> docs/Architecture/Workflow/ImportarServicioWeb/<TICKET>-preview-stream-descarga-segura/
> ```
> 
> Crear paquete técnico canónico, diagramas de secuencia y amenazas, contrato HTTP, expiración, límites y evidencia saneada.
> 
> ## Investigación obligatoria
> 
> - Determinar si el proveedor entrega descriptor resoluble o exige una descarga posterior.
> - Confirmar tamaño máximo, tipos permitidos, disposición, expiración y comportamiento en granja web.
> - Definir persistencia temporal o token firmado sin depender de memoria local del proceso cuando haya más de un nodo.
> - Medir llamadas externas de metadata y contenido y documentar cuándo son inevitables.
> 
> ## Implementa
> 
> - Descriptor opaco ligado a usuario, tarea, proveedor, `ExternalKey`, content type, tamaño, disposición y expiración.
> - Canje autorizado mediante `GET`/`HEAD` en handler; `HEAD` no descarga el recurso externo.
> - Streaming por bloques, límites estrictos, cancelación por desconexión y headers `Content-Type`, `Content-Length`, `Content-Disposition`, `Cache-Control: no-store`, `X-Content-Type-Options: nosniff` y política de framing compatible con el visor.
> - Disposición inline solo para allowlist; el resto se descarga como attachment.
> - Resultado uniforme para descriptor ausente, vencido o ajeno, sin revelar existencia.
> - Reutilización controlada o consumo único según el contrato aprobado, sin repetir descarga SII por reintentos del mismo request.
> 
> ## Restricciones
> 
> - Nunca aceptar URL, ruta, token externo, content type o tamaño enviados por el navegador como autoridad.
> - No devolver bytes en JSON/base64 desde ASMX.
> - No persistir contenido temporal en una ruta pública ni registrar descriptor, token o datos personales.
> - No desactivar validación TLS ni usar callbacks permisivos.
> - No modificar visores, endpoints legacy, `ClassAlmacenamiento` o el flujo de ejecución DOC-67.
> 
> ## Aceptación
> 
> - Un descriptor válido permite exactamente el contenido autorizado para la misma sesión/tarea.
> - Descriptor vencido, alterado, de otra tarea o de otro usuario devuelve rechazo opaco.
> - Archivo sobredimensionado o tipo no permitido nunca se transmite parcialmente como éxito.
> - Las pruebas demuestran como máximo una descarga externa por consumo y cero llamadas externas para `HEAD`.
> - `GetPreview` continúa siendo de solo lectura y no cambia tarea, intención, documento, expediente, caché, índice ni auditoría funcional.
> 
> ## Trazabilidad
> 
> Brecha frontend: consumo seguro de `DescriptorId`, vista inline y descarga temporal. Dependencias: backend 02, 06, DOC-67 y contrato compartido.
> 
> ## Pruebas obligatorias
> 
> Usar servidor HTTP local simulado, `node:test`, MSBuild y pruebas de seguridad/rechazo. No ejecutar llamadas SII reales ni E2E autenticada sin autorización.
> 
> ## E2E real obligatoria
> 
> - Reutilizar exclusivamente `tools/e2e`, su prueba `importar-servicio-web-modern.spec.cjs`, perfiles, autenticación, gate, certificado local, evidencias y restauración; no crear infraestructura paralela.
> - Extender el escenario de preview existente: consultar un item real autorizado, obtener `DescriptorId` y consumirlo por el handler de streaming en la misma sesión/tarea.
> - Confirmar contenido no vacío, `Content-Type`, `Content-Length`, disposición inline/attachment, `Cache-Control: no-store`, `X-Content-Type-Options: nosniff` y expiración.
> - Ejecutar controles negativos reales sin mutación: descriptor alterado, vencido, reutilizado cuando sea de un solo uso, otra tarea/sesión y tipo no visualizable.
> - Confirmar mediante la telemetría saneada existente que un consumo produce como máximo una descarga externa y que `HEAD` no descarga el contenido SII.
> - Auditar exclusivamente con `SELECT` que preview/descarga no cambia tarea, intención, documento, expediente, relación, caché, índices ni auditoría funcional.
> - La corrida requiere autorización explícita para ambiente/cuenta/recurso, no imprime secretos y termina con el gate apagado y sin alcance.
> 
> ## Entregable final
> 
> Entregar handler, servicios, contratos, pruebas, modelo de amenazas, documentación y evidencia del conteo de llamadas externas.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: INSCRIPCIONES, LISTADO, PREVIEW

## Capabilities

### New Capabilities
- `lista-preview-incripciones`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

