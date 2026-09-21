## Why

LISTADO-TIPOLOGIAS. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue DOC-68.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # Prompt backend 09 — Listado enriquecido y catálogo de tipologías
> 
> Actúa como arquitecto y desarrollador senior de ASP.NET WebForms/VB.NET. Inspecciona primero la implementación cerrada de DOC-67 y crea un cambio OpenSpec nuevo; no reabras ni modifiques el cambio archivado.
> 
> Extiende de forma aditiva `ResolveCapabilities` y `QueryItems` para que el frontend pueda representar los documentos SII y seleccionar una tipología válida sin consultar directamente tablas, endpoints legacy ni el proveedor externo.
> 
> ## Objetivo
> 
> Publicar metadatos presentables, estado conocido y acciones permitidas por elemento, además del catálogo de tipologías documentales autorizado para la tarea y el trámite actuales.
> 
> ## Rutas canónicas de implementación
> 
> ```txt
> DTOs/Workflow/ImportarServicioWeb/
> └── ImportarServicioWebDtos.vb
> 
> Services/Workflow/ImportarServicioWeb/
> └── ImportItemPresentationService.vb
> 
> Infrastructure/Repositories/Workflow/ImportarServicioWeb/
> ├── MySqlImportDocumentTypeCatalogRepository.vb
> └── MySqlImportItemStatusRepository.vb
> 
> Infrastructure/Workflow/ImportarServicioWeb/Sii/
> └── SiiImportContractMapper.vb
> 
> Tests/
> ├── importar-servicio-web-query-presentation.test.cjs
> ├── importar-servicio-web-document-type-catalog.test.cjs
> └── importar-servicio-web-query-no-extra-sii-calls.test.cjs
> ```
> 
> - Extender DTOs existentes de manera compatible; no cambiar nombres ni semántica de campos publicados.
> - Los metadatos específicos se expresan como colección tipada `Code/Label/Value`, sin agregar libro, registro o acto al núcleo común.
> - El catálogo se obtiene desde repositorios modernos parametrizados y se proyecta en `ResolveCapabilitiesResponseDto` o en un DTO aditivo de la misma operación; no crear otro transporte si la operación existente puede resolverlo.
> 
> ## Ruta documental obligatoria
> 
> ```txt
> Doc/Actualizacion/workflow/ImportarServicioWeb/<TICKET>-listado-enriquecido-catalogo-tipologias/
> ```
> 
> Crear `00-Indice.md` a `07-Metadata.md`, `Diagramas/` y evidencia de contratos/consultas. Sustituir `<TICKET>` por el identificador real.
> 
> ## Investigación obligatoria
> 
> - Caracterizar los campos SII ya disponibles en la única respuesta de `consultarInformacionSello`.
> - Confirmar la fuente autoritativa de tipologías permitidas por tarea, trámite y lista de chequeo.
> - Definir cómo se determina `Disponible`, `Importado` o `ConNovedad` usando persistencia local y reconciliación, sin consultar nuevamente SII.
> - Medir y dejar prueba del número de llamadas externas durante `QueryItems`.
> 
> ## Implementa
> 
> - Metadatos presentables para libro, inscripción/registro, fecha, naturaleza/acto, noticia y referencia, saneados y sin payload externo crudo.
> - `ImportStatus` y acciones permitidas calculadas en servidor con valores versionados y documentados.
> - Catálogo de tipologías con `DocumentTypeId`, nombre y obligatoriedad, filtrado por el contexto autoritativo de la tarea.
> - Enriquecimiento local de los items después del único mapping de la respuesta SII.
> - Paginación y compatibilidad con los campos actuales de `ExternalItemDto`.
> 
> ## Restricciones
> 
> - Una llamada `QueryItems` no puede repetir `consultarInformacionSello` por inscripción, imagen, estado o tipología.
> - No consultar SII para resolver estado importado ni catálogo documental.
> - No devolver NIT, razón social, matrícula, tokens, URLs, rutas, XML/JSON crudo ni datos técnicos innecesarios.
> - No modificar endpoints legacy, `ClassAlmacenamiento`, funciones DOC-67 conservadas ni el flujo de ejecución.
> - Todo SQL nuevo es parametrizado y de solo lectura en esta entrega.
> 
> ## Aceptación
> 
> - Una respuesta externa con varias inscripciones produce una sola llamada SII y todos los items enriquecidos.
> - El frontend puede construir las columnas requeridas sin interpretar `ExternalKey` ni consultar endpoints legacy.
> - El catálogo contiene exclusivamente tipologías válidas para el trámite actual.
> - Los estados y acciones coinciden con intención/reconciliación persistidas y no habilitan duplicados.
> - Contratos 1.0 existentes continúan deserializando; la extensión queda versionada y cubierta por fixtures.
> 
> ## Trazabilidad
> 
> Brechas frontend: consulta/listado SII, metadatos completos, estado/acciones y catálogo de tipologías. Dependencias: backend 01, 03, 05, 06 y DOC-67 archivado.
> 
> ## Pruebas obligatorias
> 
> Usar `node:test`, fixtures compartidos, MSBuild .NET Framework y el readiness existente. No exigir TypeScript, Vitest ni red real. E2E autenticada solo con autorización y controles `SELECT`.
> 
> ## E2E real obligatoria
> 
> - Reutilizar `tools/e2e/scripts/run-workflow-e2e-platform.cjs`, `tools/e2e/tests/importar-servicio-web-modern.spec.cjs`, autenticación, perfiles, gate, saneamiento y evidencia existentes; no crear otro runner, login, `.env`, proyecto Playwright ni escenario paralelo.
> - Extender el escenario de consulta DOC-56/DOC-67 para una tarea descartable autorizada de cada registro disponible: MERCANTIL, ESAL y RUP.
> - Verificar en UI y respuesta que libro, inscripción/registro, fecha, acto/naturaleza, noticia/referencia, estado y acciones corresponden al mismo `ExternalKey` saneado.
> - Verificar que el catálogo mostrado coincide con las tipologías autorizadas obtenidas mediante consultas de control exclusivamente `SELECT`.
> - Instrumentar el adaptador E2E existente para demostrar una sola invocación funcional a `QueryItems`/`consultarInformacionSello` por consulta, sin repetirla por fila, filtro, estado o tipología.
> - Cubrir cero, uno y múltiples items; filtros Disponible/Importado/Con novedad; proveedor no autorizado; tarea cambiada y gate apagado.
> - La prueba es obligatoria para cierre, pero solo se ejecuta con autorización explícita de ambiente, cuenta y recurso. Debe restaurar el gate en `false` con usuarios/grupos vacíos y conservar evidencia saneada.
> 
> ## Entregable final
> 
> Entregar código, contratos, fixtures, pruebas, documentación, diagramas y evidencia explícita del conteo de llamadas SII.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: LISTADO, TIPOLOGIAS

## Capabilities

### New Capabilities
- `listado-tipologias`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.

