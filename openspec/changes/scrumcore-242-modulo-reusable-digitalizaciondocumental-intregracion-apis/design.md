## Context

SCRUMCORE-242: MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL-INTREGRACION-APIS

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

## Goals / Non-Goals

**Goals**
- Refinar alcance tecnico usando el contexto completo de Jira.
- Definir decisiones arquitectonicas, riesgos y plan de migracion.

**Non-Goals**
- Cambios fuera del alcance descrito por el ticket.

## Decisions

1. La fase implementa una capa API reutilizable bajo `src/modules/digitalizacion/services` y `src/modules/digitalizacion/hooks`; el modal no se acopla directamente al backend hasta definir el contrato final de submit/persistencia.
2. Las respuestas backend se validan con `unwrapAppResponse`, aceptando `success/data` y `Success/Data` para tolerar casing backend sin relajar el contrato minimo.
3. Los errores funcionales se normalizan como `DigitalizacionApiError` y se lanzan con `DigitalizacionApiContractError` cuando hay respuesta invalida, data null, IDs invalidos o validacion local fallida.
4. El upload temporal usa `/api/gestor-documental/almacenamiento/upload-temporal` y chunks con `PUT` binario. No persiste URLs temporales ni continua sin `rutaTemporalId` y `archivoTemporalId`.
5. La proteccion anti doble submit y stale responses vive en hooks de operacion con `AbortController`, generation refs y loading refs.
6. `DigitalizacionContext` sigue siendo source-of-truth: los servicios reciben request DTO explicitos y no leen URL, localStorage ni estado UI.

## Risks / Trade-offs

- La integracion de boton primario del modal queda para una fase de orquestacion UI/BE porque falta confirmar metadata final, idempotency key y respuesta de persistencia productiva.
- El endpoint de chunk puede devolver `AppResponses` o solo 2xx; el servicio valida envelope cuando existe `success` y confia en el status HTTP cuando no hay body contractual.
- `npm run build` sigue bloqueado por un error preexistente fuera del modulo en `AppEditorToolbar.tsx`.

## Migration Plan

1. Conectar `useUploadTemporalPdf` con el PDF generado por `useDigitalizacionScanner`.
2. En modo `crear`, usar `useCrearDocumentoDigitalizado` despues del upload temporal.
3. En modo `adjuntar`, ejecutar `useAdjuntarDigitalizacion().validar` antes de `adjuntar`.
4. Integrar metadata resuelta con `useDigitalizacionMetadataResolve` cuando la UI exponga tipologia/lista chequeo completa.
5. Mantener `requestId` estable por intento de submit para preparar idempotencia backend.

## Open Questions

- Confirmar si el backend devolvera envelope AppResponses en cada chunk o solo status HTTP 2xx.
- Confirmar si `RequestId` sera obligatorio para create/attach.
- Confirmar si el submit final debe vivir dentro de `DigitalizacionDocumentalModal` o en un contenedor externo del dominio.
