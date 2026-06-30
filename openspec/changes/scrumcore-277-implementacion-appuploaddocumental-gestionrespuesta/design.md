## Context

SCRUMCORE-277: IMPLEMENTACION-APPUPLOADDOCUMENTAL-GESTIONRESPUESTA

## Jira Details

> PROMPT ARQUITECTÓNICO OFICIAL - Integración AppUploadDocumental en Gestión Respuesta
> Rol esperado
>   Arquitecto frontend senior.
>   React 19, TypeScript estricto, integración API enterprise, StorageEngineV2, workflow documental, anexos de respuesta, contratos backend, carga por chunks, guards runtime, UX operacional,  accesibilidad, testing de componentes/servicios y arquitectura sostenible.
> Objetivo
>   Implementar la integración real de AppUploadDocumental dentro del módulo GestionRespuesta, en el tab Gestión, para permitir que el usuario cargue uno o varios archivos y los almacene  como documentos anexos asociados a una respuesta de radicado.
>   El flujo debe usar StorageEngineV2 en dos fases:
> 1. Cargar archivo a temporal
> 2. Consumir temporal para almacenar el anexo
> 
> Por cada archivo debe ejecutarse:
> 
> POST upload-temporal/init
> -> PUT chunk(s)
> -> GET status
> -> POST complete
> -> POST /api/gestor-documental/almacenamiento
> 
> Al finalizar exitosamente, el documento debe quedar:
> 
> - almacenado en gabinete documental;
> - relacionado en ra_anexos_respuesta;
> - indexado mediante CabinetIndexSeed con provider RADICACION;
> - visible tras refrescar el listado del tab Documentos;
> - disponible para abrirse desde el visor PDF embebido si el backend lo retorna en el listado documental.
> 
> ———
> 
> ## Fuente Contractual Oficial
> 
> No leer DTOs externos ni rutas locales de backend para este ticket.
> 
> La fuente de verdad contractual es el documento técnico:
> 
> Documento Técnico Frontend - Almacenamiento de Documento Anexo desde Workflow
> 
> La implementación debe respetar exactamente ese contrato.
> 
> Backend espera payloads en PascalCase.
> 
> Frontend puede usar tipos internos en camelCase, pero debe existir mapper explícito y testeado:
> 
> camelCase frontend
> -> PascalCase backend request
> -> PascalCase backend response
> -> camelCase frontend normalizado
> 
> ———
> 
> ## IMPORTANTE
> 
> Este ticket NO debe:
> 
> - reimplementar AppUploadDocumental;
> - reimplementar AppUploadBatchView;
> - reimplementar AppUpload;
> - reimplementar AppProgressBatch;
> - copiar HTML legacy;
> - usar jQuery;
> - usar Bootstrap manual;
> - usar WebForms;
> - usar .ashx;
> - usar XMLHttpRequest;
> - usar FormData legacy para upload;
> - llamar funciones globales legacy;
> - actualizar DOM manualmente;
> - inventar endpoints;
> - modificar backend;
> - introducir any nuevo;
> - ocultar errores de contrato;
> - asumir documento almacenado antes de respuesta exitosa del backend;
> - insertar manualmente filas en DocumentosWorkbench sin refrescar desde backend.
> 
> Este ticket SÍ debe:
> 
> - integrar AppUploadDocumental en el tab Gestión de GestionRespuesta;
> - crear adapter específico para GestionRespuesta;
> - construir request final con AnexoRespuesta;
> - construir CabinetIndexSeed;
> - soportar tipología documental por archivo;
> - hacer un POST /almacenamiento por archivo;
> - consultar status antes de complete;
> - validar ChunksPendientes vacío;
> - validar data.AnexoRespuesta.Created === true;
> - usar callbacks tipados;
> - refrescar documentos mediante provider compartido;
> - permitir que el tab Documentos refleje el nuevo documento;
> - preservar rawBackendResult;
> - implementar guards runtime;
> - manejar cancelación y retry;
> - documentar matriz campo a campo;
> - agregar pruebas unitarias e integración.
> 
> ———
> 
> ## Contexto Técnico Existente
> 
> Respetar y reutilizar:
> 
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/
> src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
> src/modules/almacenamientoDocumental/services/uploadDocumentalInterfaceRegistration.mapper.ts
> src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts
> src/modules/almacenamientoDocumental/utils/storageFile.utils.ts
> src/modules/almacenamientoDocumental/utils/tipoDocumentalSuggestion.utils.ts
> 
> Módulo consumidor:
> 
> src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
> src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
> src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
> src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx
> 
> La sección actual de adjuntos simples del tab Gestión debe evolucionar a una integración documental especializada.
> 
> ———
> 
> ## Arquitectura Esperada
> 
> GestionRespuesta
>   -> GestionRespuestaDocumentosProvider
>       -> Tab Gestión
>           -> GestionRespuestaMainTabContent
>               -> GestionRespuestaUploadDocumental
>                   -> AppUploadDocumental
>                       -> AppUploadBatchView
>                       -> AppUpload
>                       -> AppProgressBatch
>                   -> almacenamientoDocumentalUpload.service
>       -> Tab Documentos
>           -> DocumentosWorkbench
> 
> Responsabilidades:
> 
> GestionRespuestaUploadDocumental
> - adapter específico del módulo;
> - obtiene contexto workflow/respuesta;
> - provee loaders de config/tipologías;
> - transforma metadata por archivo en request backend;
> - maneja onStored/onError/onBatchComplete;
> - llama refreshDocumentos() cuando el anexo se crea correctamente.
> 
> AppUploadDocumental
> - experiencia reusable de carga documental;
> - cola de archivos;
> - tipología por archivo;
> - fecha si aplica;
> - guardar individual;
> - guardar todos;
> - retry/cancelación;
> - progreso por fase.
> 
> almacenamientoDocumentalUpload.service
> - cliente técnico;
> - init;
> - chunks;
> - status;
> - complete;
> - almacenamiento final;
> - guards runtime;
> - errores tipados.
> 
> GestionRespuestaDocumentosProvider
> - contexto compartido entre tabs;
> - expone refreshDocumentos();
> - expone documentosRefreshKey.
> 
> DocumentosWorkbench
> - escucha documentosRefreshKey;
> - recarga listado documental desde backend;
> - permite seleccionar el documento nuevo y verlo en visor PDF embebido cuando el backend lo retorna.
> 
> ———
> 
> ## Contrato Backend Obligatorio
> 
> Todos los requests usan JWT vía clienteApi.
> 
> El mismo token debe usarse durante todo el flujo:
> 
> init upload
> chunk upload
> status
> complete
> almacenamiento final
> 
> Endpoints:
> 
> POST /api/gestor-documental/almacenamiento/upload-temporal/init
> PUT /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
> GET /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/status
> POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
> POST /api/gestor-documental/almacenamiento
> 
> Reglas:
> 
> - chunkIndex inicia en 0;
> - chunks con Content-Type: application/octet-stream;
> - enviar X-Total-Chunks;
> - cada request de chunk envía solo bytes;
> - último chunk puede ser menor que ChunkSizeBytes;
> - consultar status antes de complete;
> - validar ChunksPendientes: [];
> - no reutilizar RutaTemporalId ni ArchivoTemporalId después de almacenamiento exitoso.
> 
> ———
> 
> ## Request Init
> 
> Backend espera PascalCase:
> 
> {
>   "NombreOriginal": "soporte-respuesta.pdf",
>   "TamanoBytes": 48216,
>   "Extension": ".pdf",
>   "HashSha256Esperado": null,
>   "NumeroChunks": 1
> }
> 
> Response:
> 
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "RutaTemporalId": "usr_141_...",
>     "ArchivoTemporalId": "af_89....pdf",
>     "ChunkSizeBytes": 10485760,
>     "Estado": "Initialized"
>   },
>   "errors": []
> }
> 
> Frontend conserva:
> 
> RutaTemporalId
> ArchivoTemporalId
> ChunkSizeBytes
> 
> ———
> 
> ## Request Final De Almacenamiento
> 
> Por cada archivo, construir un request final independiente.
> 
> Backend espera PascalCase:
> 
> {
>   "NombreGabinete": "CORRESPO",
>   "RutaTemporalId": "usr_141_...",
>   "NombreDocumento": "Anexo workflow respuesta 2600466700021",
>   "RequestId": "workflow-anexo-2600466700021-001",
>   "Documentos": [
>     {
>       "IdDocumento": "wf-anexo-001",
>       "ArchivoTemporalId": "af_89....pdf",
>       "NombreOriginal": "soporte-respuesta.pdf",
>       "Extension": ".pdf",
>       "NumeroPaginas": 1
>     }
>   ],
>   "Trd": {
>     "IdTipoDocumento": 43,
>     "NombreTipoDocumento": "Comprobante De Egreso"
>   },
>   "CabinetIndexSeed": {
>     "SourceModule": "RADICACION",
>     "ProviderKey": "RADICACION",
>     "Version": "1.0.0",
>     "Payload": {
>       "ModoResolucion": "RespuestaRadicado"
>     }
>   },
>   "AnexoRespuesta": {
>     "IdRespuestaRadicado": 672,
>     "NombreArchivo": "soporte-respuesta.pdf",
>     "TipoAdjunto": "respuesta",
>     "Observacion": "Anexo cargado desde workflow"
>   },
>   "NumeroPaginasDeclaradas": 1
> }
> 
> ———
> 
> ## Reglas De Tipología/TRD
> 
> La tipología documental se selecciona por archivo dentro de AppUploadDocumental.
> 
> Flujo:
> 
> loadTiposDocumentales()
> -> opciones visibles por archivo
> -> usuario selecciona tipología
> -> metadata del archivo guarda idTipoDocumento/nombreTipoDocumento
> -> adapter construye Trd.IdTipoDocumento/Trd.NombreTipoDocumento
> -> POST almacenamiento por archivo
> 
> No enviar múltiples archivos con tipologías distintas en un solo request final.
> 
> Regla obligatoria:
> 
> Múltiples archivos
> -> procesamiento secuencial
> -> un POST final por archivo
> -> Trd corresponde al archivo actual
> 
> ———
> 
> ## CabinetIndexSeed
> 
> Para este flujo usar:
> 
> {
>   "SourceModule": "RADICACION",
>   "ProviderKey": "RADICACION",
>   "Version": "1.0.0",
>   "Payload": {
>     "ModoResolucion": "RespuestaRadicado"
>   }
> }
> 
> Reglas:
> 
> - ProviderKey constante: RADICACION;
> - SourceModule constante: RADICACION;
> - ModoResolucion constante para este caso: RespuestaRadicado;
> - no duplicar datos que ya van en bloques comunes;
> - no enviar datos no conocidos.
> 
> ———
> 
> ## AnexoRespuesta
> 
> Construir desde contexto workflow/respuesta y archivo actual:
> 
> {
>   "IdRespuestaRadicado": 672,
>   "NombreArchivo": "soporte-respuesta.pdf",
>   "TipoAdjunto": "respuesta",
>   "Observacion": "Anexo cargado desde workflow"
> }
> 
> Reglas:
> 
> - IdRespuestaRadicado requerido;
> - NombreArchivo debe ser file.name;
> - no enviar ruta local;
> - no enviar C:\...;
> - no enviar subdirectorios;
> - TipoAdjunto para este caso: "respuesta";
> - si falta IdRespuestaRadicado, bloquear almacenamiento como anexo.
> 
> ———
> 
> ## Response Esperado
> 
> Backend responde PascalCase anidado:
> 
> {
>   "success": true,
>   "message": "OK",
>   "data": {
>     "Documento": {
>       "IdAlmacen": 9967,
>       "IdRegistroProduccionDocumental": 23040,
>       "NombreArchivoFinal": "DIG00009967.pdf"
>     },
>     "AnexoRespuesta": {
>       "IdAnexoRespuesta": 150,
>       "IdRespuestaRadicado": 672,
>       "IdAlmacen": 9967,
>       "NombreGabinete": "CORRESPO",
>       "NombreArchivo": "soporte-respuesta.pdf",
>       "Created": true
>     },
>     "Indice": {
>       "ProviderKey": "RADICACION",
>       "Resolved": true,
>       "SourceTrace": "ra_respuesta_radicado"
>     },
>     "Workflow": {
>       "LogInserted": true,
>       "IdTareaWorkflow": 933,
>       "IdRutaWorkflow": 9
>     }
>   },
>   "meta": {
>     "Status": "success",
>     "RequestId": "workflow-anexo-2600466700021-001",
>     "ResponseVersion": "1.1"
>   },
>   "errors": []
> }
> 
> Validaciones obligatorias:
> 
> success === true
> data.Documento.IdAlmacen válido
> data.Documento.IdRegistroProduccionDocumental válido
> data.Documento.NombreArchivoFinal no vacío
> data.AnexoRespuesta.Created === true
> meta.RequestId coincide o existe
> 
> Si success === false, no asumir que el anexo fue creado.
> 
> Mensaje de error funcional:
> 
> errors[0].UserMessage ?? message ?? "Error almacenando anexo"
> 
> ———
> 
> ## Tipos Frontend Obligatorios
> 
> Agregar o completar sin any:
> 
> export type AnexoRespuestaStorage = {
>   idRespuestaRadicado: number;
>   nombreArchivo: string;
>   tipoAdjunto: "respuesta" | string;
>   observacion?: string | null;
> };
> 
> export type CabinetIndexSeedStorage = {
>   sourceModule: "RADICACION" | string;
>   providerKey: "RADICACION" | string;
>   version: string;
>   payload: {
>     modoResolucion: "RespuestaRadicado" | string;
>     proveedorExterno?: string | null;
>     radicadoExterno?: string | null;
>     matriculaSII?: string | null;
>   };
> };
> 
> export type WorkflowAnexoStorageResult = {
>   documento: {
>     idAlmacen: number;
>     idRegistroProduccionDocumental: number;
>     nombreArchivoFinal: string;
>   };
>   anexoRespuesta: {
>     idAnexoRespuesta?: number | null;
>     idRespuestaRadicado: number;
>     idAlmacen: number;
>     nombreGabinete: string;
>     nombreArchivo: string;
>     created: boolean;
>   };
>   indice?: {
>     providerKey?: string | null;
>     resolved?: boolean | null;
>     sourceTrace?: string | null;
>   } | null;
>   workflow?: {
>     logInserted?: boolean | null;
>     idTareaWorkflow?: number | null;
>     idRutaWorkflow?: number | null;
>   } | null;
> };
> 
> ———
> 
> ## Ubicación Esperada
> 
> Crear:
> 
> src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental.tsx
> src/modules/gestionCorrespondencia/services/gestionRespuestaUploadDocumental.service.ts
> src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.ts
> src/modules/gestionCorrespondencia/adapters/gestionRespuestaUploadDocumental.mapper.test.ts
> 
> Actualizar:
> 
> src/modules/gestionCorrespondencia/components/gestionRespuestaMainTab/GestionRespuestaMainTabContent.tsx
> src/modules/gestionCorrespondencia/context/GestionRespuestaDocumentosContext.tsx
> src/modules/gestionCorrespondencia/components/documentosWorkbench/DocumentosWorkbench.tsx
> src/modules/almacenamientoDocumental/types/almacenamientoDocumental.types.ts
> src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
> src/modules/almacenamientoDocumental/services/uploadDocumentalInterfaceRegistration.mapper.ts
> 
> ———
> 
> ## Refresh Del Módulo
> 
> El refresh se centraliza en:
> 
> GestionRespuestaDocumentosProvider
> 
> Agregar si no existe:
> 
> documentosRefreshKey: number;
> refreshDocumentos: () => void;
> 
> Regla:
> 
> Cuando onStored confirme data.AnexoRespuesta.Created === true,
> GestionRespuestaUploadDocumental llama refreshDocumentos().
> 
> DocumentosWorkbench debe observar documentosRefreshKey:
> 
> documentosRefreshKey cambia
> -> DocumentosWorkbench recarga listado documental desde backend
> -> el documento subido desde Gestión aparece en el tab Documentos
> -> si el backend retorna los datos necesarios, puede abrirse en el visor PDF embebido
> 
> No insertar manualmente el documento en el listado como fuente principal. Backend es fuente de verdad.
> 
> ———
> 
> ## Flujo Funcional Completo
> 
> 1. Usuario entra a GestionRespuesta.
> 2. GestionRespuestaDocumentosProvider carga contexto.
> 3. Tab Gestión renderiza GestionRespuestaUploadDocumental.
> 4. Adapter carga config y tipologías.
> 5. Usuario selecciona archivos.
> 6. AppUploadDocumental valida extensión/tamaño.
> 7. Usuario selecciona tipología por archivo.
> 8. Usuario guarda uno o todos.
> 9. Por cada archivo:
>     - calcular chunks;
>     - init;
>     - subir chunks;
>     - consultar status;
>     - validar ChunksPendientes vacío;
>     - complete;
>     - construir request final PascalCase;
>     - POST /almacenamiento;
>     - validar response;
>     - emitir onStored.
> 
> 10. Si AnexoRespuesta.Created === true:
> 
> - llamar refreshDocumentos();
> - emitir eventos tipados;
> - marcar archivo como almacenado.
> 
> 11. Tab Documentos recarga listado.
> 12. Usuario puede seleccionar el documento nuevo y verlo en visor PDF si el backend lo expone.
> 
> ———
> 
> ## Manejo De Errores
> 
> - Config falla: bloquear selección.
> - Tipologías fallan: bloquear guardado si tipología es requerida.
> - Falta idRespuestaRadicado: bloquear almacenamiento como anexo.
> - Falta tipología: bloquear archivo.
> - Archivo inválido: rechazar o encolar con error según configuración.
> - Falla init: no subir chunks.
> - Falla chunk: no llamar status, complete ni almacenamiento final.
> - status con pendientes: no llamar complete.
> - Falla complete: no llamar almacenamiento final.
> - Falla almacenamiento final: no marcar como almacenado.
> - success === false: mostrar error funcional.
> - Cancelación con temporal creado: intentar DELETE upload-temporal.
> - Retry: generar nuevo RequestId.
> 
> ———
> 
> ## Pruebas Obligatorias
> 
> Unitarias:
> 
> - mapper construye request PascalCase final;
> - mapper construye AnexoRespuesta;
> - mapper construye CabinetIndexSeed;
> - mapper usa file.name sin ruta local;
> - mapper rechaza contexto sin idRespuestaRadicado;
> - tipología por archivo construye Trd;
> - RequestId es único por intento;
> - response anidado se normaliza correctamente;
> - AnexoRespuesta.Created === true confirma relación;
> - success === false usa errors[0].UserMessage.
> 
> Servicio:
> 
> - init envía PascalCase;
> - chunks usan bytes crudos;
> - chunks envían X-Total-Chunks;
> - status se consulta antes de complete;
> - no llama complete si ChunksPendientes no está vacío;
> - no llama almacenamiento final si falla complete;
> - almacenamiento final envía AnexoRespuesta y CabinetIndexSeed;
> - cancelación intenta DELETE si hay temporal.
> 
> Integración:
> 
> - GestionRespuestaUploadDocumental renderiza AppUploadDocumental;
> - carga config/tipologías;
> - selecciona archivo;
> - selecciona tipología;
> - guarda archivo;
> - ejecuta flujo completo;
> - emite onStored;
> - llama refreshDocumentos;
> - DocumentosWorkbench reacciona a documentosRefreshKey;
> - error en un archivo no corrompe otros;
> - retry funciona.
> 
> ———
> 
> ## Documentación Obligatoria
> 
> Crear:
> 
> docs/Architecture/AppUploadDocumental/SCRUMCORE-XXX-Integracion-GestionRespuesta-Anexos.md
> 
> Debe incluir:
> 
> - objetivo;
> - fuente contractual oficial;
> - flujo end-to-end;
> - matriz campo frontend/backend;
> - contrato AnexoRespuesta;
> - contrato CabinetIndexSeed;
> - tipología por archivo;
> - PascalCase vs camelCase;
> - estrategia de refresh;
> - relación con DocumentosWorkbench;
> - manejo de errores;
> - pruebas ejecutadas;
> - límites conocidos.
> 
> ———
> 
> ## Criterios De Aceptación
> 
> - GestionRespuesta usa AppUploadDocumental para anexos.
> - El flujo usa StorageEngineV2 por chunks.
> - Se consulta status antes de complete.
> - Se valida ChunksPendientes: [].
> - El request final usa PascalCase.
> - El request final incluye AnexoRespuesta.
> - El request final incluye CabinetIndexSeed.
> - La tipología se selecciona por archivo.
> - Se hace un POST /almacenamiento por archivo.
> - Se valida data.AnexoRespuesta.Created === true.
> - Se refresca el módulo vía GestionRespuestaDocumentosProvider.
> - El tab Documentos refleja el documento después del refresh.
> - No hay dependencia runtime legacy.
> - No se usa .ashx.
> - No se usa XHR.
> - No se usa jQuery.
> - No se usa FormData legacy.
> - No se modifica backend.
> - No se introduce any.
> - Hay pruebas unitarias/integración.
> - Hay documentación enterprise.
> 
> ———
> 
> ## Instrucción Final
> 
> Implementar de manera enterprise, escalable y sostenible la integración de AppUploadDocumental en GestionRespuesta para almacenar documentos anexos desde workflow, usando StorageEngineV2, upload
> temporal por chunks, status obligatorio, complete, almacenamiento final con AnexoRespuesta y CabinetIndexSeed, tipología por archivo, mapper PascalCase/camelCase, guards runtime, cancelación,
> retry, callbacks tipados, refresh centralizado vía provider, actualización del tab Documentos y documentación técnica completa.

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. TBD

## Risks / Trade-offs

- TBD

## Migration Plan

1. TBD

## Open Questions

- TBD
