## Why

IMPLEMENTACION-VER-DOCUMENTO-GESTION-CORRESPONDENCIA. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-227.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> PROMPT ARQUITECTÓNICO — Integración de AppDocumentViewerOrchestrator en DocumentosWorkbench
> Rol esperado
> Arquitecto frontend senior  (React 19, TypeScript estricto, AppTreeTable, AppVisorEmbedPdf, Dynamic UI, orchestration integration, testing enterprise)
> Objetivo
> Integrar AppDocumentViewerOrchestrator dentro de DocumentosWorkbench para:
> unificar visualización documental
> 
> soportar row_click y menu_action
> 
> resolver DocumentResolveRequest
> 
> cargar AppVisorEmbedPdf efectivamente
> 
> mantener estabilidad UX
> 
> Sin introducir duplicación de lógica ni romper selección múltiple/documento activo.
> IMPORTANTE
> DocumentosWorkbench NO aplica permisos del visor.
> Solo:
> obtiene DocumentResolveRequest
> 
> usa AppDocumentViewerOrchestrator
> 
> pasa resultado consolidado a AppVisorEmbedPdf.load()
> 
> AppVisorEmbedPdf.load() es responsable de:
> permisos
> 
> override por firma
> 
> policy efectiva del visor
> 
> Este ticket NO debe:
> modificar backend
> 
> cambiar endpoints
> 
> alterar permisos internos del visor
> 
> duplicar lógica resolve/firma
> 
> romper Dynamic UI
> 
> romper AppTreeTable
> 
> El objetivo es exclusivamente:
> integrar el núcleo reusable
> 
> conectar action/ver_documento
> 
> actualizar visor runtime
> 
> Dependencia
> AppDocumentViewerOrchestrator
> 
> AppTreeTable
> 
> AppVisorEmbedPdf
> 
> SCRUM-205
> 
> Dynamic UI
> 
> Contexto existente
> Actualmente:
> row_click puede abrir documento
> 
> menu_action puede abrir documento
> 
> el flujo puede divergir
> 
> DocumentResolveRequest proviene de action/ver_documento
> 
> Estado actual
> No existe una integración consolidada y desacoplada entre:
> Dynamic UI
> 
> AppTreeTable
> 
> action/ver_documento
> 
> AppDocumentViewerOrchestrator
> 
> AppVisorEmbedPdf
> 
> Ubicación esperada
> Workbench:src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx
> Hooks:src/modules/gestionCorrespondencia/hooks/*
> Adapters:src/modules/gestionCorrespondencia/adapters/*
> Tests:src/modules/gestionCorrespondencia/tests/*
> Restricciones obligatorias
> NO cambiar backendNO cambiar endpointsNO usar anyNO duplicar resolve/firmaNO romper selección múltipleNO romper documento activoNO tocar lógica interna AppVisorEmbedPdf
> Regla arquitectónica obligatoria
> DocumentosWorkbench debe actuar únicamente como consumidor del núcleo reusable AppDocumentViewerOrchestrator.
> Esto implica:
> DocumentosWorkbench obtiene DocumentResolveRequest
> 
> DocumentosWorkbench invoca visualizarDocumento()
> 
> DocumentosWorkbench NO implementa resolve/firma
> 
> row_click y menu_action convergen en misma integración
> 
> Regla de source of truth
> DocumentResolveRequest obtenido desde:POST /api/GestorDocumental/Documentos/ListaDocumentosRadicados/action
> es el único contrato canónico válido para invocar AppDocumentViewerOrchestrator.
> Contrato backend obligatorio
> Action/ver_documento request:
> TableId
> 
> ViewMode
> 
> ActionId
> 
> RowId
> 
> ParentRowId
> 
> NodeType
> 
> Payload.IdDocumento
> 
> Payload.NombreGabinete
> 
> IdTareaWorkflow
> 
> Radicado
> 
> Grafo
> 
> Response:{  "success": true,  "data": {    "DocumentResolveRequest": {      "NombreGabinete": string,      "IdDocumento": number    }  }}
> Contrato de integración obligatorio
> DocumentosWorkbench debe:
> resolver action/ver_documento
> 
> obtener DocumentResolveRequest
> 
> invocar:visualizarDocumento()
> 
> con:{  documentId,  nombreGabinete,  context: {    idTareaWorkflow,    radicado,    grafo  }}
> Contrato de salida visor
> DocumentosWorkbench debe mantener:
> activeFileUrl
> 
> activeRowId
> 
> document context:
> documentId
> 
> nombreGabinete
> 
> isPdf
> 
> isElectronicallySigned
> 
> firmaCheckStatus
> 
> y pasar al visor:
> fileUrl={activeFileUrl}
> Reglas de implementación obligatorias
> Convergencia handlers
> 
> Ambos:
> row_click
> 
> menu_action
> 
> deben converger en:
> misma función orquestadora local
> 
> Resolve canónico
> 
> SIEMPRE usar:DocumentResolveRequest
> Integración visor
> 
> actualizar documento activo consolidado
> 
> mantener documento previo en errores
> 
> no romper visor
> 
> Selección múltiple
> 
> NO alterarse
> 
> NO perderse
> 
> NO mezclarse con documento activo
> 
> Reglas de interacción
> click fila => visualizar
> 
> menu_action ver_documento => visualizar
> 
> selección múltiple intacta
> 
> documento activo estable
> 
> Accesibilidad y UX
> loading visual
> 
> errores visibles
> 
> no flicker visor
> 
> focus estable
> 
> no pérdida scroll/contexto
> 
> Reglas de performance
> evitar múltiples visualizaciones simultáneas
> 
> estabilidad visor
> 
> memoizar handlers
> 
> evitar re-render completo
> 
> Manejo de errores obligatorio
> Si falla action/ver_documento:
> NO llamar visualizarDocumento()
> 
> Si falla resolve/firma:
> mantener documento previo
> 
> no romper visor
> 
> Riesgos a evitar
> duplicación lógica
> 
> stale state
> 
> pérdida documento activo
> 
> race conditions
> 
> coupling a DTO backend
> 
> selección múltiple rota
> 
> Pruebas unitarias obligatorias
> row_click usa mismo flujo
> 
> menu_action usa mismo flujo
> 
> payload visualizarDocumento correcto
> 
> preserve documento previo
> 
> Pruebas de integración UI obligatorias
> row_click -> action -> visualizarDocumento
> 
> menu_action -> action -> visualizarDocumento
> 
> visor carga correctamente
> 
> selección múltiple intacta
> 
> Pruebas de interacción en navegador obligatorias
> click rápido múltiples documentos
> 
> estabilidad visor
> 
> foco estable
> 
> menú funciona correctamente
> 
> Pruebas E2E obligatorias
> row_click visualiza documento
> 
> menu_action visualiza documento
> 
> PDF firmado/no firmado
> 
> resolve error
> 
> firma error
> 
> selección múltiple intacta
> 
> Pruebas QT / calidad
> sin errores build
> 
> sin warnings TS/lint
> 
> sin errores consola
> 
> sin memory leaks
> 
> sin regresiones visuales
> 
> Criterios de aceptación
> row_click y menu_action convergen correctamente
> 
> DocumentResolveRequest es contrato canónico
> 
> AppDocumentViewerOrchestrator funciona correctamente
> 
> Visor carga efectivamente documento
> 
> Selección múltiple permanece estable
> 
> Documento activo permanece estable
> 
> Tests pasan correctamente
> 
> Documentación obligatoria
> Ruta:
> docs/modulos/gestioncorrespondencia/implenetacionverdocumento
> Archivos obligatorios:
> SCRUMCORE-[XX]-Arquitectura.md
> 
> SCRUMCORE-[XX]-Implementacion-Detallada.md
> 
> SCRUM-[XX]-Integracion-BackEnd.md
> 
> SCRUM-[XX]-Pruebas.md
> 
> SCRUM-[ID]-Metadata.md
> 
> Instrucción final
> Integrar DocumentosWorkbench con AppDocumentViewerOrchestrator usando DocumentResolveRequest como contrato canónico, garantizando una única orquestación de visualización documental, estabilidad del visor y ausencia de regresiones en Dynamic UI, selección múltiple y AppTreeTable/AppVisorEmbedPdf.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: CORRESPONDENCIA, DOCUMENTO, GESTION, IMPLEMENTACION, VER

## Capabilities

### New Capabilities
- `implementacion-ver-documento-gestion-correspondencia`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
