# Jira Context - SCRUMCORE-284

## Summary

IMPLEMENTACION-TIPOLOGIAS-DOCUMENTALES-GESTIONCORRESPONDENCIA

## Description

> PROMPT ARQUITECTONICO - Tipologias documentales por adjunto en Gestion Correspondencia
> Rol esperado
>   Arquitecto frontend senior.
>   React 19, TypeScript estricto, Clean Architecture, componentes enterprise, integracion API documental, gestion de adjuntos por lote, metadata por archivo,  accesibilidad, testing y migracion quirurgica sin romper flujos existentes.
> Objetivo
>   Implementar en src/modules/gestionCorrespondencia la carga de tipologias documentales para adjuntos de Gestion Respuesta, usando contexto WORKFLOW, y permitir que  cada archivo adjunto tenga su propia seleccion de tipo documental/lista de chequeo.
>   La solucion debe:
> cargar tipologias al iniciar el componente/contenedor cuando existan idTareaWf e idRutaWf;
> 
> consumir GET /api/gestor-documental/tipologias-documentales;
> 
> enviar Contexto=WORKFLOW, IdTareaWf e IdRutaWf;
> 
> NO resolver IdTipoTramite en frontend;
> 
> reutilizar AppUploadBatchView para la vista de archivos por lote;
> 
> reutilizar AppInputSelect para el dropdown de tipologias;
> 
> guardar la lista de tipologias una sola vez por contexto workflow;
> 
> guardar la seleccion de tipologia de forma independiente por archivo;
> 
> mantener la logica documental en gestionCorrespondencia, no en componentes shared.
> 
> Contexto backend confirmado
>   Endpoint:
> GET /api/gestor-documental/tipologias-documentales
> 
> Query requerida para este caso:
> 
> {
>   Contexto: "WORKFLOW",
>   IdTareaWf: number,
>   IdRutaWf: number
> }
> 
> Respuesta esperada:
> 
> {
>   success: boolean;
>   message: string;
>   data: Array<{
>     Id: number;
>     Descripcion: string;
>   }>;
>   meta?: {
>     Status?: "success" | "empty" | "error";
>     RequestId?: string;
>     Total?: number;
>   };
>   errors?: unknown[];
> }
> 
> Reglas:
> 
> - Id es el identificador canonico publico: tipo_doc_series.Id_Tipo_Doc_Series.
> - Descripcion es el texto visible.
> - Si existe idTipoTramite, podria enviarse, pero para este caso NO se debe usar; SCRUM-304 permite resolverlo desde IdTareaWf + IdRutaWf.
> - No enviar IdTipoTramite=0.
> - No llamar otra API para resolver IdTipoTramite.
> 
> Referencias backend/documentacion:
> 
> D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\GestorDocumental\TipologiasDocumentales\SolicitaListaTipologiasDocumentalesController.cs
> 
> D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchiCore\Docs\GestorDocumental\TipologiasDocumentales\Core\SCRUM-304-resolver-contexto-tipologias-externas\07-
> Documento-Tecnico-Frontend.md
> 
> ## Contexto frontend actual
> 
> Archivos principales:
> 
> src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
> src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
> src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
> 
> Actualmente GestionRespuestaMainTabContent usa AppUpload directamente para adjuntos. Para soportar metadata por archivo, migrar esa zona a AppUploadBatchView.
> 
> Componentes shared existentes que deben reutilizarse:
> 
> src/app/Components/UI/AppUploadBatchView
> src/app/Components/UI/AppInputSelect
> src/app/Components/UI/AppButton
> 
> ## Precondicion esperada
> 
> El inicio de Gestion Correspondencia sera actualizado para entregar idRutaWf.
> 
> Propagar ese dato asi:
> 
> GestionRespuesta
>   -> GestionRespuestaDocumentosProvider
>   -> GestionRespuestaMainTabContent
> 
> Contrato esperado minimo:
> 
> type GestionRespuestaProps = {
>   idTareaWf?: number;
>   idRutaWf?: number;
>   radicado?: string;
>   idRespuestaRadicado?: string | number;
> };
> 
> El provider debe exponer ambos:
> 
> type GestionRespuestaDocumentosState = {
>   idTareaWf?: number;
>   idRutaWf?: number;
>   // existente...
> };
> 
> ## Ubicacion esperada
> 
> Servicios:
> 
> src/modules/gestionCorrespondencia/services/tipologiasDocumentalesWorkflow.service.ts
> 
> Hook:
> 
> src/modules/gestionCorrespondencia/hooks/useTipologiasDocumentalesWorkflow.ts
> 
> Tipos:
> 
> src/modules/gestionCorrespondencia/types/tipologiasDocumentalesWorkflow.types.ts
> 
> Integracion UI:
> 
> src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
> src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
> src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
> 
> Tests:
> 
> src/modules/gestionCorrespondencia/tests/tipologiasDocumentalesWorkflow.service.test.ts
> src/modules/gestionCorrespondencia/tests/useTipologiasDocumentalesWorkflow.test.tsx
> src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx
> 
> ## Contratos TypeScript obligatorios
> 
> Crear tipos sin any:
> 
> export type TipologiaDocumentalWorkflowQuery = {
>   idTareaWf: number;
>   idRutaWf: number;
> };
> 
> export type TipologiaDocumentalWorkflowDto = {
>   Id: number;
>   Descripcion: string;
> };
> 
> export type TipologiaDocumentalWorkflowOption = {
>   value: number;
>   label: string;
>   idTipoDocumento: number;
>   nombreTipoDocumento: string;
> };
> 
> export type TipologiasDocumentalesWorkflowResponse = {
>   success: boolean;
>   message: string;
>   data: TipologiaDocumentalWorkflowDto[];
>   meta?: {
>     Status?: string;
>     RequestId?: string;
>     Total?: number;
>   };
>   errors?: unknown[];
> };
> 
> export type GestionRespuestaAdjuntoMetadata = {
>   idTipoDocumento?: number;
>   nombreTipoDocumento?: string;
>   tipologiaError?: string;
> };
> 
> export type GestionRespuestaAdjuntoItem =
>   AppUploadBatchFileItem<GestionRespuestaAdjuntoMetadata>;
> 
> ## Servicio obligatorio
> 
> Implementar servicio usando clienteApi, no fetch directo:
> 
> getTipologiasDocumentalesWorkflow(
>   query: TipologiaDocumentalWorkflowQuery,
>   options?: { signal?: AbortSignal }
> ): Promise<TipologiaDocumentalWorkflowOption[]>
> 
> Reglas del servicio:
> 
> - validar que idTareaWf > 0;
> - validar que idRutaWf > 0;
> - llamar:
> 
>   /api/gestor-documental/tipologias-documentales
> 
> - params:
> 
>   {
>     Contexto: "WORKFLOW",
>     IdTareaWf: query.idTareaWf,
>     IdRutaWf: query.idRutaWf
>   }
> 
> - aceptar respuestas con success=true y data=[];
> - mapear Id/Descripcion a opciones de AppInputSelect;
> - si success=false, lanzar error funcional normalizado;
> - no loguear payloads sensibles;
> - no introducir any.
> 
> ## Hook obligatorio
> 
> Implementar:
> 
> useTipologiasDocumentalesWorkflow({
>   idTareaWf,
>   idRutaWf,
>   enabled = true,
> })
> 
> Debe exponer:
> 
> {
>   options: TipologiaDocumentalWorkflowOption[];
>   loading: boolean;
>   error?: string;
>   empty: boolean;
>   reload: () => Promise<void>;
> }
> 
> Reglas:
> 
> - cargar automaticamente solo si enabled=true, idTareaWf > 0 e idRutaWf > 0;
> - usar AbortController;
> - ignorar respuestas stale si cambia tarea/ruta;
> - no llamar la API si faltan parametros;
> - conservar opciones vacias con empty=true cuando backend retorna success=true y sin datos;
> - permitir retry con reload;
> - no duplicar llamadas por render.
> 
> ## Integracion UI obligatoria
> 
> Reemplazar el uso directo de AppUpload en adjuntos por AppUploadBatchView.
> 
> Usar renderMetadata para renderizar la tipologia por archivo:
> 
> <AppUploadBatchView<GestionRespuestaAdjuntoMetadata>
>   title="Adjuntos"
>   files={files}
>   selectedUid={selectedUid}
>   loading={tipologiasLoading}
>   onFilesSelected={handleFilesSelected}
>   onSelectFile={setSelectedUid}
>   onRemoveFile={handleRemoveFile}
>   onClearAll={handleClearAll}
>   canSaveAll={false}
>   canSaveOne={false}
>   renderMetadata={({ item, disabled }) => (
>     <AppInputSelect<number>
>       value={item.metadata?.idTipoDocumento}
>       options={tipologiaOptions}
>       loading={tipologiasLoading}
>       disabled={disabled || tipologiasLoading || Boolean(tipologiasError)}
>       placeholder="Tipo documental"
>       searchable
>       allowClear
>       size="sm"
>       state={item.metadata?.tipologiaError ? "error" : "default"}
>       helperText={item.metadata?.tipologiaError}
>       aria-label={`Tipo documental de ${item.name}`}
>       onChange={(value, option) => updateFileTipologia(item.uid, value, option)}
>     />
>   )}
> />
> 
> Reglas:
> 
> - AppUploadBatchView NO debe modificarse para conocer tipologias.
> - AppInputSelect debe ser el componente de dropdown.
> - La lista de opciones viene del hook, no de constantes hardcodeadas.
> - Cada archivo guarda su seleccion en metadata.
> - Cambiar tipologia de un archivo no debe cambiar la de otros.
> - Si la lista falla y la tipologia es obligatoria, bloquear envio/avance.
> - Si data=[], mostrar estado claro: “Sin tipologias disponibles”.
> 
> ## Estado de archivos
> 
> Al seleccionar archivos:
> 
> - crear un GestionRespuestaAdjuntoItem por cada File;
> - generar uid estable;
> - llenar:
> 
>   {
>     uid,
>     file,
>     name,
>     size,
>     extension,
>     state: "ready",
>     metadata: {}
>   }
> 
> - preservar archivo activo si sigue existiendo;
> - limpiar metadata al eliminar archivo;
> - limpiar todo en onClearAll.
> 
> ## Validacion de avance
> 
> canAdvanceToSend debe considerar:
> 
> - existe al menos un archivo;
> - si tipologia es requerida, todos los archivos deben tener metadata.idTipoDocumento;
> - no hay error de carga de tipologias;
> - no esta cargando tipologias;
> - existe idTareaWf e idRutaWf validos.
> 
> Si falta tipologia:
> 
> - marcar error por archivo en metadata.tipologiaError;
> - no abrir GestionDocumentoModal.
> 
> ## Restricciones obligatorias
> 
> NO hacer:
> 
> - no modificar backend;
> - no inventar endpoints;
> - no crear otro select si existe AppInputSelect;
> - no consumir clienteApi desde componentes React;
> - no meter logica de tipologias dentro de AppUploadBatchView;
> - no usar any;
> - no usar jQuery, Bootstrap manual, HTML por strings ni callbacks globales;
> - no hardcodear tipologias;
> - no enviar IdTipoTramite=0;
> - no resolver IdTipoTramite en frontend.
> 
> SI hacer:
> 
> - crear servicio tipado;
> - crear hook tipado;
> - usar AppUploadBatchView;
> - usar AppInputSelect;
> - cargar tipologias una vez por idTareaWf + idRutaWf;
> - guardar seleccion por archivo;
> - manejar loading, empty, error y retry;
> - cubrir pruebas de servicio, hook e UI;
> - actualizar tests existentes de Gestion Respuesta si cambian mocks.
> 
> ## Accesibilidad
> 
> - el select por fila debe tener aria-label con el nombre del archivo;
> - errores de tipologia deben mostrarse como helper text;
> - botones de eliminar/ver deben conservar aria-label;
> - no dejar controles activos si falta catalogo obligatorio;
> - no depender solo de color para errores.
> 
> ## Pruebas obligatorias
> 
> Servicio:
> 
> - construye request con Contexto=WORKFLOW, IdTareaWf, IdRutaWf;
> - no envia IdTipoTramite;
> - normaliza Id/Descripcion;
> - retorna lista vacia cuando success=true y data=[];
> - lanza error funcional cuando success=false;
> - respeta AbortSignal.
> 
> Hook:
> 
> - no llama API si falta idTareaWf;
> - no llama API si falta idRutaWf;
> - carga opciones con parametros validos;
> - expone loading;
> - expone empty;
> - expone error;
> - reload reintenta;
> - ignora respuestas stale cuando cambia tarea/ruta.
> 
> UI:
> 
> - renderiza AppUploadBatchView en adjuntos;
> - renderiza un AppInputSelect por archivo;
> - seleccion de tipologia actualiza solo ese archivo;
> - eliminar archivo elimina su metadata;
> - “Enviar” no abre modal si falta tipologia requerida;
> - “Enviar” abre modal cuando todos los archivos tienen tipologia;
> - muestra error si falla catalogo;
> - muestra “Sin tipologias disponibles” cuando catalogo esta vacio.
> 
> ## Criterios de aceptacion
> 
> - Gestion Correspondencia recibe y propaga idRutaWf.
> - El modulo carga tipologias con Contexto=WORKFLOW, IdTareaWf, IdRutaWf.
> - No se usa IdTipoTramite en frontend para este flujo.
> - La lista de tipologias se carga una vez por contexto.
> - Cada archivo tiene seleccion independiente.
> - La UI de adjuntos usa AppUploadBatchView.
> - El dropdown usa AppInputSelect.
> - No hay cambios en backend.
> - No hay any nuevo.
> - Tests focales pasan.
> 
> ## Documentacion esperada
> 
> Crear o actualizar documentacion en:
> 
> docs/Architecture/GestionCorrrespondecia/
> 
> Sugerido:
> 
> docs/Architecture/GestionCorrrespondecia/17-FE-Tipologias-Documentales-Adjuntos-Workflow.md
> 
> Debe incluir:
> 
> - problema;
> - endpoint consumido;
> - parametros enviados;
> - razon para usar WORKFLOW + idTareaWf + idRutaWf;
> - razon para no resolver idTipoTramite en frontend;
> - composicion AppUploadBatchView + AppInputSelect;
> - flujo de metadata por archivo;
> - manejo de error/empty/loading;
> - pruebas realizadas.
> 
> ## Instruccion final
> 
> Implementar tipologias documentales por adjunto en Gestion Correspondencia como una especializacion de modulo: cargar una sola vez el catalogo desde GET /api/gestor-
> documental/tipologias-documentales con Contexto=WORKFLOW, IdTareaWf e IdRutaWf; renderizar la cola de archivos con AppUploadBatchView; renderizar el selector de
> tipologia por archivo con AppInputSelect; guardar la seleccion en metadata independiente por archivo; validar antes de abrir el flujo de envio; y cubrir servicio, hook
> e integracion UI con pruebas, sin modificar backend ni acoplar componentes shared al dominio documental.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: DOCUMENTALES, GESTIONCORRESPONDENCIA, IMPLEMENTACION, TIPOLOGIAS
