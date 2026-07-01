## 1. Refinement

- [x] 1.1 Consolidar alcance final desde Jira, SCRUM-250 y contexto de codigo existente.
- [x] 1.2 Ajustar proposal/design/spec/tasks con decisiones, riesgos y criterios verificables.

## 2. Contract And Storage Client

- [x] 2.1 Agregar tipos workflow-anexo sin `any`: `AnexoRespuestaStorage`, `CabinetIndexSeedStorage`, `WorkflowAnexoStorageResult` y DTOs backend PascalCase.
- [x] 2.2 Implementar mapper camelCase frontend -> PascalCase backend para init/final storage cuando aplique a SCRUMCORE-277.
- [x] 2.3 Normalizar response PascalCase anidado -> resultado frontend camelCase preservando `rawBackendResult`.
- [x] 2.4 Validar `success === true`, documento valido y `AnexoRespuesta.Created === true` para confirmar anexo.
- [x] 2.5 Integrar `GET status` antes de `complete` y bloquear `complete` si `ChunksPendientes` no esta vacio.
- [x] 2.6 Mantener compatibilidad con el flujo generico existente de almacenamiento documental.

## 3. GestionRespuesta Adapter

- [x] 3.1 Crear `GestionRespuestaUploadDocumental.tsx` como adapter de modulo sobre `AppUploadDocumental`.
- [x] 3.2 Crear `gestionRespuestaUploadDocumental.mapper.ts` para construir `AnexoRespuesta`, `CabinetIndexSeed`, `Trd`, `Documentos` y `RequestId`.
- [x] 3.3 Crear `gestionRespuestaUploadDocumental.service.ts` si se requiere un wrapper especializado para el flujo anexo respuesta.
- [x] 3.4 Validar `nombreGabinete`, `idRespuestaRadicado`, tipologia por archivo y nombre de archivo sin ruta local.
- [x] 3.5 Reemplazar la seccion de adjuntos simple en `GestionRespuestaMainTabContent.tsx` por el adapter documental.

## 4. Cross-Tab Refresh

- [x] 4.1 Extender `GestionRespuestaDocumentosContext.tsx` con `documentosRefreshKey` y `refreshDocumentos()`.
- [x] 4.2 Actualizar `useGestionRespuestaDocumentos.ts` para exponer los nuevos campos con fallbacks seguros.
- [x] 4.3 Hacer que `GestionRespuestaUploadDocumental` llame `refreshDocumentos()` solo cuando el anexo quede confirmado.
- [x] 4.4 Hacer que `DocumentosWorkbench` recargue el listado cuando cambie `documentosRefreshKey`.
- [x] 4.5 Verificar que el listado viene de backend y no de insercion manual local.

## 5. UX, Error Handling, Cancel And Retry

- [x] 5.1 Configurar `AppUploadDocumental` para seleccion multiple, guardar individual, guardar todos, tipologia requerida y errores por archivo.
- [x] 5.2 Mostrar estados funcionales para contexto incompleto, carga de config/tipologias, error de almacenamiento y retry.
- [x] 5.3 Asegurar que cancelacion con temporal creado intenta `DELETE upload-temporal`.
- [x] 5.4 Asegurar que retry genera nuevo `RequestId` y no reutiliza temporales.
- [x] 5.5 Evitar logs/persistencia de bytes, tokens, rutas locales o payload sensible.

## 6. Tests

- [x] 6.1 Unit tests del mapper: PascalCase final, `AnexoRespuesta`, `CabinetIndexSeed`, `Trd`, nombre sin ruta local e `idRespuestaRadicado` invalido.
- [x] 6.2 Unit tests de response: nested response valido, `Created !== true`, `success === false`, error funcional desde `UserMessage`.
- [x] 6.3 Tests del storage service: init/chunk/status/complete/store order, `X-Total-Chunks`, bytes crudos, bloqueo por pendientes, cancelacion.
- [x] 6.4 Tests de adapter/componente: renderiza `AppUploadDocumental`, carga loaders, selecciona tipologia, guarda archivo, emite callbacks.
- [x] 6.5 Tests de refresh: `refreshDocumentos()` se llama al confirmar anexo y `DocumentosWorkbench` reacciona al key.
- [x] 6.6 Ejecutar suites afectadas y registrar evidencia.

## 7. Documentation And Close

- [x] 7.1 Crear `docs/Architecture/AppUploadDocumental/SCRUMCORE-277-Integracion-GestionRespuesta-Anexos.md`.
- [x] 7.2 Documentar flujo end-to-end, matriz campo FE/BE, PascalCase/camelCase, tipologia por archivo, refresh del Workbench, modal, cola de archivos, visor PDF bajo demanda, errores UX y ajustes visuales enterprise.
- [x] 7.3 Confirmar explicitamente: backend no modificado, endpoints no modificados, sin `.ashx`, sin XHR, sin jQuery, sin FormData legacy y sin `any` nuevo.
- [x] 7.4 Ejecutar `openspec validate scrumcore-277-implementacion-appuploaddocumental-gestionrespuesta --strict`.
- [ ] 7.5 Preparar commit/push/PR cuando la implementacion y verificacion esten completas.
