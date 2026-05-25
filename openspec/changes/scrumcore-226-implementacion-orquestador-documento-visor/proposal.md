## Why

IMPLEMENTACION-ORQUESTADOR-DOCUMENTO-VISOR. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-226.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> PROMPT ARQUITECTÓNICO — Núcleo reusable AppDocumentViewerOrchestrator (resolve + firma + estado visor)
> Rol esperado
> Arquitecto frontend senior  (React 19, TypeScript estricto, Clean Architecture, integración API enterprise, state orchestration, testing enterprise)
> Objetivo
> Crear un núcleo reusable de visualización documental llamado:
> AppDocumentViewerOrchestrator
> para que múltiples módulos puedan:
> Resolver visualización documental.
> 
> Resolver URL final del visor.
> 
> Detectar si el documento es PDF.
> 
> Consultar firma electrónica solo para PDF.
> 
> Consolidar estado documental runtime para AppVisorEmbedPdf.
> 
> Este ticket NO integra UI específica de módulos.El objetivo es exclusivamente crear la plataforma reusable.
> IMPORTANTE
> El orquestador NO conoce permisos del visor PDF.
> Responsabilidades del orquestador:
> resolve documental
> 
> resolve URL
> 
> consulta firma electrónica
> 
> consolidación estado documental runtime
> 
> NO:
> permisos UI
> 
> toolbar permissions
> 
> edición/anotaciones
> 
> Este ticket NO debe:
> modificar backend
> 
> cambiar endpoints
> 
> depender de DocumentosWorkbench
> 
> depender de AppTreeTable
> 
> incorporar lógica visual
> 
> persistir URLs temporales
> 
> invocar action/ver_documento
> 
> El núcleo reusable SOLO debe:
> recibir contexto documental ya resuelto
> 
> ejecutar resolve visualización
> 
> consultar firma
> 
> consolidar estado runtime
> 
> Dependencia
> AppVisorEmbedPdf
> 
> visualizacion/resolve
> 
> firma-electronica
> 
> Dynamic UI ecosystem
> 
> Contexto existente
> Actualmente múltiples módulos podrían necesitar:
> visualización documental
> 
> resolve URL
> 
> consulta firma
> 
> consolidación estado visor
> 
> No existe aún una plataforma reusable y desacoplada para este flujo.
> Estado actual
> La lógica de visualización documental podría terminar duplicándose entre módulos, generando:
> inconsistencias
> 
> race conditions
> 
> manejo desigual de errores
> 
> divergencia de comportamiento
> 
> Ubicación esperada
> Plataforma reusable:
> src/app/Components/UI/AppDocumentViewerOrchestrator/
> Archivos:
> src/app/Components/UI/AppDocumentViewerOrchestrator/├── AppDocumentViewerOrchestrator.types.ts├── AppDocumentViewerOrchestrator.service.ts├── AppDocumentViewerOrchestrator.adapter.ts├── useDocumentViewerOrchestrator.ts├── index.ts└── tests/
> Restricciones obligatorias
> NO cambiar backendNO cambiar endpointsNO usar anyNO depender de módulos específicosNO depender de action/ver_documentoNO persistir URLs temporalesNO introducir lógica visualNO introducir lógica de permisos
> Regla arquitectónica obligatoria
> El núcleo reusable debe encargarse exclusivamente de:
> resolve documental
> 
> selección URL
> 
> consulta firma
> 
> consolidación estado runtime
> 
> La obtención de:
> DocumentResolveRequest
> 
> action/ver_documento
> 
> metadata de fila
> 
> pertenece exclusivamente al módulo consumidor.
> Regla de source of truth
> El núcleo reusable recibe como contrato canónico:
> {  documentId: number,  nombreGabinete: string}
> No debe:
> inferir datos
> 
> reconstruir payloads
> 
> depender de rows DTO
> 
> Regla de concurrencia obligatoria
> El orquestador debe protegerse contra:
> race conditions
> 
> stale responses
> 
> múltiples visualizaciones concurrentes
> 
> Esto implica:
> cancelar requests previos
> 
> ignorar respuestas stale
> 
> preservar estabilidad del visor
> 
> Regla de estabilidad del visor
> Si falla:
> resolve
> 
> firma electrónica
> 
> NO debe perderse el documento previamente visible.
> Regla de seguridad obligatoria
> Las URLs:
> UrlTemporal
> 
> UrlTemporalAbsoluta
> 
> NO deben persistirse en:
> localStorage
> 
> sessionStorage
> 
> caches persistentes
> 
> Contrato backend obligatorio
> Resolve visualización
> 
> POST /api/gestor-documental/documentos/visualizacion/resolve
> Request:{  "NombreGabinete": string,  "IdDocumento": number}
> Response:{  "IdDocumento": number,  "NombreGabinete": string,  "FileName": string,  "ContentType": string,  "Origen": "ORIGINAL|TIF_TO_PDF",  "UrlTemporal": string,  "UrlTemporalAbsoluta": string | null,  "ExpiresAt": string}
> Firma electrónica
> 
> GET /api/gestor-documental/documentos/{idArchivo}/firma-electronica?nombreGabinete={nombreGabinete}
> Response:{  "IdArchivo": number,  "NombreGabinete": string,  "FirmadoElectronico": boolean,  "IdCertificado": number}
> Reglas contractuales críticas
> URL final:
> UrlTemporalAbsoluta
> 
> UrlTemporal
> 
> NO depender de:
> fileUrl legacy
> 
> url legacy
> 
> idArchivo firma:
> IdDocumento resuelto
> 
> Contrato de entrada obligatorio
> {  documentId: number,  nombreGabinete: string,  context?: {    idTareaWorkflow?: number,    radicado?: string,    grafo?: object  }}
> IMPORTANTE:context es opcional y solo para trazabilidad futura cross-módulo.
> Contrato de salida obligatorio
> {  documentId: number,  nombreGabinete: string,  fileUrl: string | null,  contentType: string | null,  isPdf: boolean,  isElectronicallySigned: boolean | null,  firmaCheckStatus:    | "not_required"    | "resolved"    | "failed",  resolveStatus:    | "idle"    | "loading"    | "resolved"    | "failed"    | "cancelled",  errors: string[]}
> Contrato del hook reusable
> useDocumentViewerOrchestrator()
> Debe exponer:
> visualizarDocumento()
> 
> documentoActivo
> 
> loading
> 
> error
> 
> reset
> 
> cancelCurrentRequest
> 
> Reglas de implementación obligatorias
> Resolve visualización
> 
> invocar resolve
> 
> resolver URL final
> 
> consolidar estado runtime
> 
> Firma electrónica
> 
> SOLO para PDF
> 
> NO bloquear visualización
> 
> Concurrencia
> 
> cancelar requests previos
> 
> ignorar stale responses
> 
> Consolidación
> 
> documentoActivo estable
> 
> no flicker
> 
> no pérdida documento previo
> 
> Reglas de interacción
> consumidores llaman visualizarDocumento()
> 
> consumidores NO implementan lógica resolve/firma
> 
> AppVisorEmbedPdf consume únicamente estado consolidado
> 
> Accesibilidad y UX
> loading perceptible
> 
> errores visibles
> 
> no flicker visor
> 
> focus estable
> 
> Reglas de performance
> evitar múltiples resolves simultáneos
> 
> memoizar handlers
> 
> estabilidad runtime
> 
> Manejo de errores obligatorio
> Si resolve falla:
> fileUrl = null
> 
> resolveStatus = failed
> 
> NO consultar firma
> 
> Si firma falla:
> mantener visualización
> 
> firmaCheckStatus = failed
> 
> isElectronicallySigned = null
> 
> Nunca lanzar excepciones no controladas.
> Riesgos a evitar
> race conditions
> 
> stale responses
> 
> pérdida documento activo
> 
> persistencia insegura URLs
> 
> duplicación lógica
> 
> coupling módulos
> 
> Pruebas unitarias obligatorias
> UrlTemporalAbsoluta prioridad
> 
> fallback UrlTemporal
> 
> PDF => consulta firma
> 
> no PDF => no consulta firma
> 
> firma falla => visor estable
> 
> stale responses ignoradas
> 
> Pruebas de integración UI obligatorias
> integración AppVisorEmbedPdf
> 
> loading
> 
> error
> 
> documentoActivo estable
> 
> Pruebas de interacción en navegador obligatorias
> clicks rápidos múltiples documentos
> 
> estabilidad visor
> 
> no flicker
> 
> cancelación requests
> 
> Pruebas E2E obligatorias
> PDF firmado
> 
> PDF no firmado
> 
> resolve error
> 
> firma error
> 
> cancelación concurrente
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
> Criterios de aceptación
> visualizacion/resolve funciona correctamente
> 
> Solo PDF consulta firma
> 
> Documento previo se mantiene en errores
> 
> URLs temporales no se persisten
> 
> Hook reusable desacoplado funcionando
> 
> Tests pasan correctamente
> 
> Documentación obligatoria
> Ruta:
> docs/Components/AppDocumentViewerOrchestrator/
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
> Implementar AppDocumentViewerOrchestrator como núcleo reusable enterprise de visualización documental desacoplado de módulos específicos, garantizando resolve documental, consulta de firma, consolidación de estado runtime y control robusto de concurrencia sin introducir regresiones ni persistencia insegura de URLs.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DOCUMENTO, IMPLEMENTACION, ORCHESTRATE, VISOR

## Capabilities

### New Capabilities
- `implementacion-orquestador-documento-visor`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
