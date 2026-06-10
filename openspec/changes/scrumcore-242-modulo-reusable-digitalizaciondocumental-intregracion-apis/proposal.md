## Why

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-INTREGRACION-APIS. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-242.
- Se formaliza una propuesta OpenSpec inicial derivada del ticket Jira.
- Se captura el resumen y la descripcion del ticket como punto de partida para refinement posterior.
- Se deja lista una base coherente para continuar con design, specs y tasks.

## Jira Details

> # PROMPT IMPLEMENTACION - Integracion APIs DigitalizacionDocumental
> # Fase FE-04 - Configuracion, lista chequeo, metadata, upload, crear y adjuntar
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ROL
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Actua como Arquitecto Frontend senior especialista en:
> 
> - React 19
> - TypeScript estricto
> - clientes API enterprise
> - AppResponses<T>
> - validacion contractual runtime
> - upload temporal por chunks
> - state orchestration
> - stale protection
> - retry seguro
> - AbortController
> - testing enterprise
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## OBJETIVO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Implementar la capa frontend de integracion API para `DigitalizacionDocumental`.
> 
> Debe cubrir:
> 
> - configuracion;
> - lista de chequeo;
> - metadata resolve;
> - upload temporal;
> - crear documento digitalizado;
> - validar adjuntar;
> - adjuntar digitalizacion PDF.
> 
> La implementacion debe ser:
> 
> - reusable;
> - desacoplada;
> - resiliente;
> - validada contractualmente;
> - protegida contra stale responses;
> - segura ante doble submit.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## CONTEXTO OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Contratos backend:
> 
> ```txt
> docs/Architecture/DigitalizacionDocumental/01-BE-Contratos-API-Digitalizacion.md
> docs/Architecture/DigitalizacionDocumental/PROMPT-BACKEND/
> ```
> 
> Cliente HTTP existente a revisar:
> 
> ```txt
> src/api/Clienteaxios.ts
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## SOURCE OF TRUTH OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> `DigitalizacionContext` es la unica fuente valida para:
> 
> - nombreGabinete;
> - modo;
> - idDocumentoDestino;
> - radicado;
> - workflow.
> 
> Los servicios NO deben:
> 
> - reconstruir contexto;
> - leer contexto desde URL;
> - leer contexto desde localStorage;
> - inferir contexto desde estado UI.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ENDPOINTS OBLIGATORIOS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> ```txt
> GET  /api/gestor-documental/digitalizacion/configuracion
> GET  /api/gestor-documental/digitalizacion/lista-chequeo
> POST /api/gestor-documental/digitalizacion/metadata/resolve
> POST /api/gestor-documental/digitalizacion/documentos
> GET  /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion/validacion
> POST /api/gestor-documental/documentos/{idDocumento}/adjuntar-digitalizacion
> ```
> 
> Upload temporal:
> 
> ```txt
> POST /api/gestor-documental/almacenamiento/upload-temporal/init
> PUT  /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/chunk/{chunkIndex}
> POST /api/gestor-documental/almacenamiento/upload-temporal/{rutaTemporalId}/{archivoTemporalId}/complete
> ```
> 
> Si el equipo decide usar upload temporal propio de digitalizacion, documentar la diferencia.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## UBICACION ESPERADA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> ```txt
> src/modules/digitalizacion/services/
> ├─ digitalizacionConfiguracion.api.ts
> ├─ digitalizacionListaChequeo.api.ts
> ├─ digitalizacionMetadata.api.ts
> ├─ digitalizacionDocumentos.api.ts
> ├─ adjuntarDigitalizacion.api.ts
> └─ digitalizacionUploadTemporal.api.ts
> 
> src/modules/digitalizacion/hooks/
> ├─ useDigitalizacionConfiguracion.ts
> ├─ useDigitalizacionListaChequeo.ts
> ├─ useDigitalizacionMetadataResolve.ts
> ├─ useCrearDocumentoDigitalizado.ts
> ├─ useAdjuntarDigitalizacion.ts
> └─ useUploadTemporalPdf.ts
> ```
> 
> Tipos:
> 
> ```txt
> src/modules/digitalizacion/types/digitalizacionApi.types.ts
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## REGLA APPRESPONSES OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Toda respuesta backend debe validarse.
> 
> Nunca asumir:
> 
> - `success = true`;
> - data existe;
> - ids existen;
> - payload completo;
> - `success` implicito;
> - `data` no null;
> - IDs presentes;
> - `RutaTemporalId` valido;
> - `ArchivoTemporalId` valido;
> 
> Toda respuesta debe validar:
> 
> - success;
> - data;
> - metadata requerida;
> - contrato minimo.
> 
> Si response invalida:
> 
> - abortar flujo;
> - emitir error funcional tipado;
> - no continuar pipeline.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## VALIDACION CONTRACTUAL RUNTIME OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Validaciones previas:
> 
> Contexto:
> 
> - contexto valido;
> - modo valido;
> - nombreGabinete valido;
> - radicado valido cuando aplique.
> 
> Adjuntar:
> 
> - `idDocumentoDestino > 0`.
> 
> PDF:
> 
> - PDF `File` existente;
> - `File` PDF;
> - tamano > 0.
> 
> Metadata:
> 
> - metadata requerida completa.
> 
> Validaciones posteriores:
> 
> - `success === true`;
> - `data != null`;
> - ids > 0;
> - strings requeridos no vacios;
> - pageCount valido.
> 
> Nunca asumir:
> 
> - data existe;
> - payload completo;
> - IDs validos.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## VALIDACION CONTRACTUAL POR ENDPOINT OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Configuracion:
> 
> - data existe.
> 
> Lista chequeo:
> 
> - items existen.
> 
> Metadata Resolve:
> 
> - metadata valida;
> - TRD valida cuando aplique.
> 
> Upload Init:
> 
> - RutaTemporalId;
> - ArchivoTemporalId.
> 
> Upload Complete:
> 
> - completado = true.
> 
> Crear Documento:
> 
> - IdDocumento > 0.
> 
> Adjuntar Documento:
> 
> - success = true;
> - contrato esperado.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## OWNERSHIP DE ESTADO OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Debe existir explicitamente:
> 
> ```txt
> Upload State
> ```
> 
> Responsable de:
> 
> - init;
> - chunks;
> - complete;
> - progreso;
> - errores;
> - cancelacion.
> 
> No mezclar Upload State con:
> 
> - estado UI;
> - metadata state;
> - scanner state.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## UPLOAD TEMPORAL OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Implementar upload por chunks con:
> 
> - init;
> - chunks;
> - progress;
> - complete;
> - cancelacion;
> - retry controlado;
> - AbortController;
> - stale protection.
> 
> PROHIBIDO:
> 
> - persistir URLs temporales;
> - continuar sin IDs validos;
> - continuar si complete falla.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## REGLA ANTI DOBLE SUBMIT OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Mientras exista una operacion activa:
> 
> - upload;
> - create;
> - attach.
> 
> NO permitir una segunda operacion concurrente.
> 
> Debe existir proteccion explicita.
> 
> Errores sugeridos:
> 
> ```txt
> UPLOAD_ALREADY_IN_PROGRESS
> CREATE_ALREADY_IN_PROGRESS
> ATTACH_ALREADY_IN_PROGRESS
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## IDEMPOTENCIA OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> El diseno debe quedar preparado para:
> 
> - reintentos;
> - recuperacion;
> - operaciones repetidas.
> 
> Sin crear:
> 
> - documentos duplicados;
> - adjuntos duplicados.
> 
> Aunque backend implemente la validacion final.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## STALE PROTECTION OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Si durante una operacion:
> 
> - cambia `context`;
> - se cierra modal;
> - se desmonta componente;
> - cambia documento destino;
> 
> Entonces:
> 
> - abortar requests pendientes si es posible;
> - ignorar responses stale;
> - NO llamar `onCompleted`;
> - NO contaminar nuevo contexto.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## MANEJO DE ERRORES OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Crear error tipado:
> 
> ```ts
> type DigitalizacionApiError = {
>   code: string;
>   message: string;
>   field?: string;
>   status?: "validation" | "conflict" | "error";
> };
> ```
> 
> Mapear:
> 
> - validaciones locales;
> - errores AppResponses;
> - errores network;
> - response parcial;
> - abort;
> - stale response.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ACCESIBILIDAD Y UX
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> - loading visible;
> - errores visibles;
> - progreso upload visible;
> - retry visible;
> - no perdida de contexto;
> - no doble submit accidental.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## PRUEBAS OBLIGATORIAS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Unitarias:
> 
> - configuracion OK;
> - lista chequeo OK;
> - metadata OK;
> - response parcial falla;
> - AppResponses success=false;
> - upload init invalido;
> - upload chunk falla;
> - upload complete falla;
> - create OK;
> - adjuntar OK;
> 
> Validacion contractual:
> 
> - data null;
> - ids invalidos;
> - payload parcial;
> - contexto invalido.
> 
> Anti doble submit:
> 
> - upload concurrente bloqueado;
> - create concurrente bloqueado;
> - attach concurrente bloqueado.
> 
> Stale Protection:
> 
> - contexto cambia;
> - modal cierra;
> - response ignorada.
> 
> Browser Interaction:
> 
> - retry upload;
> - cancelar upload;
> - cambio contexto;
> - cierre modal.
> 
> Integracion:
> 
> - create documento;
> - attach documento;
> - upload completo;
> - metadata resolve.
> 
> E2E:
> 
> - crear documento;
> - adjuntar documento;
> - upload exitoso;
> - upload fallido;
> - retry exitoso;
> - stale response ignorada.
> 
> QT / Calidad:
> 
> - sin errores build;
> - sin warnings TS/lint;
> - sin `any`;
> - sin memory leaks.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## DOCUMENTACION OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Ruta:
> 
> ```txt
> docs/Architecture/DigitalizacionDocumental/
> ```
> 
> Archivos obligatorios:
> 
> 1. `SCRUMCORE-[XX]-Arquitectura.md`
> 
> Debe incluir:
> 
> - arquitectura APIs;
> - source-of-truth;
> - upload lifecycle;
> - stale protection;
> - concurrencia.
> 
> 2. `SCRUMCORE-[XX]-Implementacion-Detallada.md`
> 
> Debe incluir:
> 
> - servicios;
> - hooks;
> - validaciones runtime;
> - ownership estados;
> - retry strategy.
> 
> 3. `SCRUM-[XX]-Integracion-BackEnd.md`
> 
> Debe incluir:
> 
> - matriz FE-BE;
> - contratos;
> - request;
> - response;
> - validaciones.
> 
> 4. `SCRUM-[XX]-Pruebas.md`
> 
> Debe incluir:
> 
> - unitarias;
> - integracion;
> - browser interaction;
> - E2E;
> - regresion;
> - cobertura.
> 
> 5. `SCRUM-[ID]-Metadata.md`
> 
> Debe incluir:
> 
> - ticket;
> - fecha;
> - version;
> - control cambios;
> - referencias cruzadas.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## INSTRUCCION FINAL
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Implementar la capa API frontend de `DigitalizacionDocumental` con validacion contractual runtime estricta, source-of-truth explicita, upload temporal seguro, ownership correcto de estados, stale protection, anti doble submit e idempotencia preparada para escenarios enterprise sin introducir regresiones.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: 04-FE, APIS, DIGITALIZACIONDOCUMENTAL, INTEGRACION, MODULO, REUSABLE

## Capabilities

### New Capabilities
- `modulo-reusable-digitalizaciondocumental-intregracion-apis`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Nueva propuesta inicial en `openspec/changes/<changeName>/proposal.md`.
- Impacto funcional pendiente de refinamiento en los siguientes artefactos OpenSpec.
