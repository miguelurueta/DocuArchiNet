## Why

CREA-COMPONENTE-APPUPLOADDOCUMENTAL. Ver detalle funcional completo del ticket en la seccion Jira Details.

## What Changes

- Se genera automaticamente una propuesta OpenSpec basada en el issue SCRUMCORE-271.
- Se formaliza la propuesta OpenSpec para implementar AppAppuploaddocumental a partir del ticket Jira.
- Se define la capability `app-appuploaddocumental` como parte de la capa UI reutilizable.
- Se conserva el contexto funcional del ticket como base para los siguientes artefactos OpenSpec.

## Jira Details

> # PROMPT ARQUITECTONICO - AppUploadDocumental
> 
> ## Rol esperado
> 
> Arquitecto frontend senior
> 
> React 19, TypeScript estricto, componentes enterprise, integracion API documental, UX de cargas por lote, state orchestration, validacion contractual, accesibilidad, migracion legacy quirurgica.
> 
> ## Objetivo
> 
> Implementar `AppUploadDocumental` como componente especializado para carga documental donde:
> 
> - use `AppUpload` para seleccion de archivos;
> - use `AppUploadBatchView` como vista base reusable;
> - use `AppProgressBatch` para procesamiento secuencial;
> - use el cliente tecnico de almacenamiento nuevo para `init -> chunks -> complete -> almacenar`;
> - cargue configuracion de tamano, extensiones y reglas desde API o loader obligatorio;
> - cargue tipos documentales desde API o loader obligatorio;
> - permita multiples documentos;
> - permita tipologia independiente por documento;
> - sugiera tipologia por nombre de archivo;
> - permita fecha documental por archivo cuando el proceso lo requiera;
> - permita guardar individual y guardar todos;
> - emita resultados tipados para refrescar el modulo consumidor;
> - conserve las capacidades utiles de `FileUploadHandler.js` sin migrar su UI ni dependencias legacy.
> 
> ## IMPORTANTE
> 
> Este ticket NO debe:
> 
> - copiar el HTML legacy;
> - reimplementar `AppUpload`;
> - reimplementar `AppProgressBatch`;
> - crear una tabla DOM manual;
> - usar jQuery;
> - usar Bootstrap manual;
> - usar WebForms;
> - usar callbacks por string;
> - usar `.ashx`;
> - usar `XMLHttpRequest`;
> - usar `FormData` legacy para upload;
> - hardcodear extensiones y tamano como fuente final;
> - hardcodear lista de tipologias;
> - inventar endpoints de configuracion o tipologias;
> - modificar backend;
> - cambiar endpoints de almacenamiento;
> - introducir `any` nuevo.
> 
> Este ticket SI debe:
> 
> - construir la vista documental final;
> - componer `AppUploadBatchView`;
> - cargar reglas desde API/loader antes de habilitar seleccion;
> - validar archivos con reglas backend;
> - soportar `reject` y `queue-with-error`;
> - mantener metadata por archivo;
> - exigir tipologia cuando aplique;
> - validar fecha cuando aplique;
> - guardar un archivo individual;
> - guardar todos secuencialmente;
> - mapear cada archivo a un POST final individual;
> - usar `AppProgressBatch` para progreso global;
> - soportar cancelacion y retry;
> - emitir `onStored`, `onBatchComplete`, `onError`;
> - documentar matriz campo a campo frontend/backend.
> 
> ## Dependencias
> 
> - `AppUpload`.
> - `AppUploadBatchView`.
> - `AppProgressBatch`.
> - `AppButton`, `AppInputSelect`, `AppInput` o wrappers existentes.
> - `almacenamientoDocumentalUpload.service`.
> - `storageFile.utils`.
> - `tipoDocumentalSuggestion.utils`.
> - `clienteApi` solo dentro de servicios, no directamente desde el componente.
> 
> ## Contexto legacy funcional
> 
> El legacy `FileUploadHandler.js` aporta estas capacidades:
> 
> - seleccion multiple;
> - drag/drop;
> - validacion de extension;
> - validacion de tamano;
> - archivo invalido visible con error;
> - tabla de archivos;
> - preview;
> - eliminar archivo;
> - eliminar todos;
> - guardar archivo individual;
> - guardar todos;
> - tipologia por archivo;
> - tipologia obligatoria;
> - sugerencia de tipologia por nombre;
> - fecha por archivo;
> - extension efectiva por modo documental;
> - callbacks para refrescar interfaz.
> 
> La vista HTML legacy observada muestra:
> 
> - titulo `Adjunta documento`;
> - botones globales `Eliminar` y `Guardar`;
> - input multiple;
> - preview en iframe;
> - filas con nombre, tamano, acciones, select de tipologia y fecha;
> - contador de archivos.
> 
> La nueva implementacion debe conservar la semantica, no el markup.
> 
> ## Estado actual
> 
> Existe `AppUpload`, pero no existe:
> 
> - vista documental final;
> - adaptador de configuracion por API;
> - tipologia por archivo;
> - sugerencia moderna de tipologia;
> - fecha documental por archivo;
> - pipeline de almacenamiento documental por chunks;
> - integracion con progreso batch reusable.
> 
> ## Ubicacion esperada
> 
> Componente:
> 
> ```txt
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.tsx
> ```
> 
> Tipos:
> 
> ```txt
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.types.ts
> ```
> 
> Estilos:
> 
> ```txt
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.module.css
> ```
> 
> Hooks:
> 
> ```txt
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.ts
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.ts
> ```
> 
> Servicios:
> 
> ```txt
> src/modules/almacenamientoDocumental/services/uploadConfig.service.ts
> src/modules/almacenamientoDocumental/services/tipoDocumental.service.ts
> src/modules/almacenamientoDocumental/services/almacenamientoDocumentalUpload.service.ts
> src/modules/almacenamientoDocumental/services/uploadDocumentalInterfaceRegistration.mapper.ts
> ```
> 
> Utils:
> 
> ```txt
> src/modules/almacenamientoDocumental/utils/storageFile.utils.ts
> src/modules/almacenamientoDocumental/utils/tipoDocumentalSuggestion.utils.ts
> ```
> 
> Tests:
> 
> ```txt
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.test.tsx
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.test.ts
> src/modules/almacenamientoDocumental/utils/tipoDocumentalSuggestion.utils.test.ts
> ```
> 
> Export:
> 
> ```txt
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/index.ts
> ```
> 
> ## Estructura de archivos obligatoria
> 
> Crear o completar:
> 
> ```txt
> src/modules/almacenamientoDocumental/
> ├─ components/
> │  └─ AppUploadDocumental/
> │     ├─ AppUploadDocumental.tsx
> │     ├─ AppUploadDocumental.types.ts
> │     ├─ AppUploadDocumental.module.css
> │     ├─ AppUploadDocumental.test.tsx
> │     ├─ README.md
> │     ├─ index.ts
> │     └─ hooks/
> │        ├─ useAppUploadDocumentalActions.ts
> │        ├─ useAppUploadDocumentalState.ts
> │        └─ useAppUploadDocumentalState.test.ts
> ├─ services/
> │  ├─ almacenamientoDocumentalUpload.service.ts
> │  ├─ uploadConfig.service.ts
> │  ├─ tipoDocumental.service.ts
> │  └─ uploadDocumentalInterfaceRegistration.mapper.ts
> ├─ types/
> │  └─ almacenamientoDocumental.types.ts
> └─ utils/
>    ├─ storageFile.utils.ts
>    ├─ storageFile.utils.test.ts
>    ├─ tipoDocumentalSuggestion.utils.ts
>    └─ tipoDocumentalSuggestion.utils.test.ts
> ```
> 
> ## Referencias de arquitectura obligatorias
> 
> Leer y respetar:
> 
> ```txt
> docs/Architecture/AppUploadDocumental/AppUploadDocumental-Requisitos.md
> docs/Architecture/AppUploadDocumental/Legacy-Gap-Analysis.md
> docs/Architecture/AppUploadDocumental/Matriz-Migracion-Legacy-AppUploadDocumental-AppProgressBatch.md
> docs/Architecture/AppUploadDocumental/PROMPTS-CONSTRUCCION-AppUploadDocumental.md
> docs/Architecture/AppUploadDocumental/PROMPT-IMPLEMENTACION-AppUploadBatchView.md
> docs/Architecture/AppUploadDocumental/PROMPT-IMPLEMENTACION-upload-storage-client.md
> docs/Architecture/AppProgressBatch/PROMPT-IMPLEMENTACION-AppProgressBatch.md
> docs/Architecture/AppUploadDocumental/diagrams/
> docs/Architecture/AppUploadDocumental/legacy/
> ```
> 
> Backend:
> 
> ```txt
> D:\imagenesda\GestorDocumental\DocuArchiCore\DocuArchi.Api\Controllers\GestorDocumental\AlmacenamientoDocumental\AlmacenamientoDocumentalController.cs
> D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.DTOs\DTOs\GestorDocumental\AlmacenamientoDocumental\
> ```
> 
> ## Contrato frontend obligatorio
> 
> ```ts
> export type UploadDocumentalProcessKey = string;
> 
> export type UploadDocumentalContext = {
>   nombreGabinete: string;
>   idExpediente?: number;
>   idTipoExpediente?: number;
>   idUnidadConservacion?: number;
>   idClaseDocumento?: number;
>   idTareaWorkflow?: number;
>   idRutaWorkflow?: number;
>   idRespuesta?: number;
>   tipoAdjunta?: number;
>   estadoAdjunto?: number;
>   estadoRelacionado?: number;
>   numeroDocumentoRelacionado?: number;
>   idImagen?: number;
>   nameModulo?: string;
>   camposIndexacion?: Array<{
>     nombreCampo: string;
>     valor?: string;
>     esObligatorio?: boolean;
>   }>;
> };
> 
> export type UploadDocumentalConfig = {
>   accept: string;
>   allowedExtensions: string[];
>   maxSizeBytes: number;
>   multiple: boolean;
>   requiereTipologia: boolean;
>   requiereFechaCarga: boolean;
>   fechaCargaObligatoria?: boolean;
>   validationMode?: "reject" | "queue-with-error";
>   preferredChunkSizeBytes?: number;
> };
> 
> export type TipoDocumentalOption = {
>   idTipoDocumento: number;
>   nombreTipoDocumento: string;
> };
> 
> export type UploadDocumentalFileMetadata = {
>   idTipoDocumento?: number;
>   nombreTipoDocumento?: string;
>   numeroPaginas?: number;
>   fechaCarga?: string;
>   error?: string;
>   warning?: string;
>   suggestionConfidence?: number;
> };
> 
> export type AlmacenarDocumentoStoredResult = {
>   fileUid: string;
>   fileName: string;
>   idAlmacen: number;
>   idRegistroProduccionDocumental: number;
>   nombreArchivoFinal: string;
>   requestId: string;
>   metadata: UploadDocumentalFileMetadata;
>   interfaceRegistration?: UploadDocumentalInterfaceRegistration[];
>   rawBackendResult?: unknown;
> };
> 
> export type UploadDocumentalBatchSummary = {
>   total: number;
>   stored: number;
>   failed: number;
>   skipped: number;
>   cancelled: number;
>   results: AlmacenarDocumentoStoredResult[];
> };
> 
> export type AppUploadDocumentalProps = {
>   proceso: UploadDocumentalProcessKey;
>   context: UploadDocumentalContext;
>   title?: string;
>   open?: boolean;
>   embedded?: boolean;
>   tipologiaObligatoria?: boolean;
>   autoSuggestTipologia?: boolean;
>   requiereFechaCarga?: boolean;
>   fechaCargaObligatoria?: boolean;
>   allowSingleFileStore?: boolean;
>   validationMode?: "reject" | "queue-with-error";
>   modoDocumento?:
>     | "default"
>     | "adjunto-radicado"
>     | "relacionado-radicado"
>     | "formato-respuesta"
>     | "documento-libre-respuesta";
>   loadConfig: (input: {
>     proceso: UploadDocumentalProcessKey;
>     context: UploadDocumentalContext;
>     modoDocumento?: AppUploadDocumentalProps["modoDocumento"];
>   }) => Promise<UploadDocumentalConfig>;
>   loadTiposDocumentales: (input: {
>     proceso: UploadDocumentalProcessKey;
>     context: UploadDocumentalContext;
>   }) => Promise<TipoDocumentalOption[]>;
>   onStored?: (result: AlmacenarDocumentoStoredResult) => void;
>   onInterfaceRegistration?: (events: UploadDocumentalInterfaceRegistration[]) => void;
>   onBatchComplete?: (summary: UploadDocumentalBatchSummary) => void;
>   onError?: (error: unknown) => void;
>   onClose?: () => void;
> };
> ```
> 
> `loadConfig` y `loadTiposDocumentales` son obligatorios si no existe endpoint canonico confirmado en el repo. No inventar endpoints. Si existen servicios reales, los props pueden tener wrappers default documentados.
> 
> No usar `any`.
> 
> ## Contrato de retorno para registro en interfaz
> 
> El legacy actualiza pantallas distintas segun `funcion_name`. La nueva implementacion debe reemplazar ese dispatch global por un contrato discriminado.
> 
> Definir:
> 
> ```ts
> export type UploadDocumentalInterfaceRegistration =
>   | {
>       kind: "production-document-row";
>       idRegistro: number;
>       idImagen?: number;
>       nombreArchivo: string;
>       fecha?: string;
>       tipoDocumental?: string;
>       nombreGabinete?: string;
>       alias?: string;
>       estadoFirmaDigital?: string;
>       iconName?: string;
>     }
>   | {
>       kind: "related-document-row" | "workflow-document-row";
>       nombreGabinete?: string;
>       idImagen?: number;
>       radicado?: string;
>       tipoDocumental?: string;
>       nombreTipoDocumental?: string;
>       idTareaWorkflow?: number;
>       estadoFirmaDigital?: string;
>       iconName?: string;
>     }
>   | {
>       kind: "migration-preview";
>       url: string;
>       idRegistro?: number;
>     }
>   | {
>       kind: "page-counter";
>       contadorPaginas: number;
>     }
>   | {
>       kind: "traffic-light";
>       urlImagenSemaforo: string;
>     }
>   | {
>       kind: "dropdown-option";
>       text: string;
>       value: string | number;
>       target?: "respuesta" | "pqrs" | "anexo";
>     }
>   | {
>       kind: "document-version-row";
>       idImagen?: number;
>       idVersionDocumento?: number;
>       idRegistroVersion?: number;
>       tipoDocumento?: string;
>       estadoFirmaDigital?: string;
>       iconName?: string;
>       dbt?: number;
>       fechaRegistroVersion?: string;
>     }
>   | {
>       kind: "table-import-result";
>       rowTable: unknown;
>       fieldTable: unknown;
>       source: "rue-sii" | "virtual-sii";
>     }
>   | {
>       kind: "raw";
>       raw: unknown;
>     };
> ```
> 
> Reglas:
> 
> - no concatenar campos con `|`;
> - no llamar funciones globales legacy;
> - no actualizar DOM desde `AppUploadDocumental`;
> - mapear solo datos presentes en respuesta backend o contexto;
> - si el shape backend no permite normalizar, emitir `kind: "raw"` con dato opaco;
> - `onStored` debe incluir los eventos asociados al archivo;
> - `onInterfaceRegistration` puede emitirse como canal especializado para el modulo consumidor.
> 
> Equivalencias legacy:
> 
> ```txt
> insert_row_producion_documental
> -> production-document-row
> 
> insert_row_documento_relacionado
> -> related-document-row
> 
> InsertRowWorkflowSeleccion
> -> workflow-document-row
> 
> adjunta_migra_documento
> -> migration-preview
> 
> actualiza_contador_imagen
> -> page-counter
> 
> actualiza_semaforo_respuesta
> -> traffic-light
> 
> actualiza_drowp_respuesta / actualiza_drowp_pqrs
> -> dropdown-option
> 
> MIGRACION / WORKFLOW / RADICACION / DOCUARCHI / CORRESPO / adjunta_documeto_version_document
> -> document-version-row
> 
> adjunta_archivo_rue_sii / adjunta_archivo_virtual_sii
> -> table-import-result
> ```
> 
> ## Mapper de registro de interfaz obligatorio
> 
> Crear un mapper aislado y testeable:
> 
> ```txt
> src/modules/almacenamientoDocumental/services/uploadDocumentalInterfaceRegistration.mapper.ts
> ```
> 
> Responsabilidad:
> 
> - recibir respuesta normalizada de almacenamiento;
> - recibir `rawBackendResult`;
> - recibir `UploadDocumentalContext`;
> - recibir metadata del archivo;
> - recibir proceso y modo documental si aplica;
> - devolver `UploadDocumentalInterfaceRegistration[]`.
> 
> API esperada:
> 
> ```ts
> export type BuildInterfaceRegistrationInput = {
>   stored: AlmacenarDocumentoStoredResult;
>   rawBackendResult?: unknown;
>   context: UploadDocumentalContext;
>   metadata: UploadDocumentalFileMetadata;
>   proceso: UploadDocumentalProcessKey;
>   modoDocumento?: AppUploadDocumentalProps["modoDocumento"];
> };
> 
> export function buildUploadDocumentalInterfaceRegistration(
>   input: BuildInterfaceRegistrationInput
> ): UploadDocumentalInterfaceRegistration[];
> ```
> 
> Reglas:
> 
> - si puede mapear una variante conocida, devolver `kind` especifico;
> - si no puede mapear con seguridad, devolver `kind: "raw"` solo cuando exista dato util;
> - no lanzar error por campos opcionales faltantes;
> - lanzar o retornar error controlado solo ante shape corrupto que impida registrar una operacion exitosa;
> - no depender de DOM;
> - no depender de nombres de funciones globales;
> - no importar componentes React;
> - no usar `any`.
> 
> ## Regla de source-of-truth obligatoria
> 
> - Backend/config loader es fuente de verdad para extensiones, tamano y reglas.
> - Backend/tipologia loader es fuente de verdad para tipos documentales.
> - Estado React es fuente de verdad de la cola actual y metadata por archivo.
> - Backend almacenamiento es fuente de verdad del resultado persistido.
> 
> No asumir archivo almacenado hasta recibir exito valido de `POST /api/gestor-documental/almacenamiento`.
> 
> ## Regla de seleccion multiple y TRD obligatoria
> 
> La API final tiene `trd` a nivel request. Como el legacy permite tipologia por archivo:
> 
> ```txt
> Multiples archivos en UI
> -> procesamiento secuencial
> -> un POST final por archivo
> -> trd corresponde al archivo actual
> ```
> 
> No enviar multiples documentos con tipologias diferentes en un solo request final.
> 
> ## Flujo obligatorio completo
> 
> 1. Componente monta.
> 2. Valida `context.nombreGabinete`.
> 3. Ejecuta `loadConfig`.
> 4. Ejecuta `loadTiposDocumentales`.
> 5. Habilita seleccion de archivos.
> 6. Usuario selecciona o arrastra archivos.
> 7. Se normalizan extension, nombre, tamano y uid.
> 8. Se valida extension y tamano con config.
> 9. Segun `validationMode`, se rechaza o se encola con error.
> 10. Se crea metadata por archivo.
> 11. Si `autoSuggestTipologia`, se sugiere tipo documental.
> 12. Usuario ajusta tipologia y fecha por archivo.
> 13. Usuario ejecuta guardar individual o guardar todos.
> 14. Se valida metadata requerida.
> 15. Se inicia `AppProgressBatch`.
> 16. Por cada archivo valido: `init`.
> 17. Se suben chunks.
> 18. Se ejecuta `complete`.
> 19. Se ejecuta `POST /almacenamiento` con un documento.
> 20. Se normaliza el resultado para registro en interfaz usando `buildUploadDocumentalInterfaceRegistration`.
> 21. Se marca archivo `done`.
> 22. Se emite `onStored`.
> 23. Se emite `onInterfaceRegistration` si aplica.
> 24. Al finalizar lote, se emite `onBatchComplete`.
> 25. Si hay error, se marca archivo y se permite retry.
> 
> ## Validaciones obligatorias
> 
> Contexto:
> 
> - `nombreGabinete` requerido y no vacio.
> 
> Configuracion:
> 
> - `allowedExtensions` no vacio;
> - `maxSizeBytes` positivo;
> - `accept` derivado de extensiones;
> - `multiple` boolean.
> 
> Archivo:
> 
> - extension permitida;
> - tamano mayor a 0;
> - tamano menor o igual a `maxSizeBytes`;
> - no duplicado exacto si la politica del componente lo bloquea.
> 
> Tipologia:
> 
> - si requerida, `idTipoDocumento` valido;
> - `nombreTipoDocumento` consistente con la opcion seleccionada.
> 
> Fecha:
> 
> - formato `yyyy-MM-dd`;
> - fecha real;
> - ano no futuro;
> - si obligatoria, no vacia.
> 
> ## Sugerencia de tipologia obligatoria
> 
> Crear utilidad pura:
> 
> ```txt
> src/modules/almacenamientoDocumental/utils/tipoDocumentalSuggestion.utils.ts
> ```
> 
> Regla:
> 
> - normalizar nombre de archivo;
> - quitar extension;
> - convertir a mayusculas;
> - remover caracteres no alfanumericos;
> - tokenizar;
> - ignorar tokens menores al minimo configurable, default 4;
> - comparar contra nombre de cada tipo documental;
> - calcular score;
> - elegir mayor score si supera umbral;
> - permitir override manual.
> 
> La sugerencia nunca debe bloquear al usuario ni reemplazar una seleccion manual existente.
> 
> ## Fecha documental obligatoria
> 
> Cuando `requiereFechaCarga` sea verdadero:
> 
> - renderizar input de fecha por archivo dentro de `renderMetadata`;
> - validar antes de guardar;
> - mostrar error por archivo;
> - mapear a `camposIndexacion` u otro campo disponible segun contrato backend/consumidor.
> 
> No enviar fechas invalidas.
> 
> ## UI obligatoria
> 
> Usar `AppUploadBatchView` con especializacion documental:
> 
> ```txt
> Header:
>   Adjuntar documentos
>   resumen: total, pendientes, errores, almacenados
> 
> Toolbar:
>   agregar archivos
>   guardar todos
>   eliminar todos
> 
> Lista:
>   nombre
>   tamano
>   estado
>   acciones: ver, eliminar, guardar individual
>   metadata: tipologia, fecha
> 
> Preview:
>   archivo activo
>   PDF/image/fallback
> 
> Footer:
>   N archivo(s)
>   errores agregados
>   progreso batch cuando aplique
> ```
> 
> Reglas de diseno:
> 
> - no replicar tabla legacy literalmente;
> - mantener densidad operacional;
> - no usar hero, cards decorativas ni gradientes;
> - botones de accion por fila con iconos y tooltip/aria-label;
> - guardar todos como accion primaria;
> - guardar individual como accion secundaria;
> - errores visibles y accionables;
> - texto sin desbordes;
> - mobile con layout apilado.
> 
> ## Integracion con AppProgressBatch
> 
> `AppUploadDocumental` debe usar `AppProgressBatch` para guardar todos.
> 
> Cada item del batch representa un archivo.
> 
> `processItem` debe:
> 
> - validar metadata final del archivo;
> - ejecutar `uploadAndStoreOneDocument`;
> - actualizar fase visible;
> - retornar `success`, `warning`, `skipped` o `fatal-error` segun resultado.
> 
> Cancelacion:
> 
> - abortar request activo;
> - si hay `rutaTemporalId` y `archivoTemporalId`, intentar cancelacion backend;
> - marcar archivo como `cancelled`;
> - no marcar como almacenado.
> 
> ## Guardar individual
> 
> Cuando `allowSingleFileStore=true`:
> 
> - la fila muestra accion guardar;
> - solo procesa ese archivo;
> - respeta las mismas validaciones;
> - usa el mismo cliente tecnico;
> - actualiza el mismo estado;
> - emite `onStored`;
> - no dispara `onBatchComplete` salvo que se defina como politica explicita.
> 
> ## Manejo de errores obligatorio
> 
> Casos:
> 
> - config falla: deshabilitar seleccion y mostrar error funcional;
> - tipologias fallan: permitir reintentar carga de catalogo, no guardar si tipologia requerida;
> - archivo invalido: rechazar o encolar con error segun `validationMode`;
> - tipologia faltante: bloquear guardado de ese archivo;
> - fecha invalida: bloquear guardado de ese archivo;
> - init falla: archivo error, retry posible;
> - chunk falla: archivo error, retry posible;
> - complete falla: archivo error, retry posible;
> - almacenamiento final falla: archivo error, retry posible;
> - cancelacion: archivo cancelled;
> - respuesta backend invalida: error controlado y no continuar.
> 
> ## Politica anti-stale obligatoria
> 
> Si durante carga cambia:
> 
> - `proceso`;
> - `context.nombreGabinete`;
> - `modoDocumento`;
> - montaje/desmontaje del componente;
> 
> entonces:
> 
> - abortar operaciones cancelables;
> - ignorar resultados stale;
> - no contaminar nueva cola;
> - no emitir `onStored` para contexto obsoleto.
> 
> Usar `operationId` o token interno por corrida.
> 
> ## Performance
> 
> - no recalcular sugerencias para todos los archivos en cada render;
> - memoizar opciones de tipologia;
> - revocar object URLs;
> - evitar rerender completo de preview si solo cambia progreso de otro archivo;
> - procesar secuencialmente para evitar saturar red/backend;
> - no leer archivo completo para calcular hash salvo que el backend lo exija.
> 
> ## Seguridad
> 
> - no persistir `File` fuera del estado runtime;
> - no persistir object URLs;
> - no loguear contenido de archivos;
> - no exponer tokens;
> - no mostrar payloads sensibles completos en UI;
> - fail-safe: si no hay configuracion, no permitir upload.
> 
> ## Pruebas unitarias obligatorias
> 
> - carga config al montar;
> - carga tipologias al montar;
> - deshabilita seleccion si config falla;
> - aplica `accept` y `maxSize` desde config;
> - seleccion multiple cuando config lo permite;
> - rechaza extension invalida;
> - encola con error en `queue-with-error`;
> - crea metadata independiente por archivo;
> - sugiere tipologia por nombre;
> - no sobrescribe tipologia manual;
> - valida tipologia obligatoria;
> - valida fecha obligatoria;
> - valida fecha futura;
> - elimina archivo y metadata asociada;
> - limpia todos los archivos;
> - selecciona archivo activo y preview;
> - guardar individual procesa solo un archivo;
> - guardar todos procesa secuencialmente;
> - `onStored` se emite con metadata;
> - `onStored` incluye `interfaceRegistration` cuando el backend/contexto permite mapearlo;
> - `onInterfaceRegistration` emite eventos discriminados;
> - mapper de registro cubre variantes conocidas y fallback `raw`;
> - `onBatchComplete` resume resultados;
> - respuesta stale se ignora.
> 
> ## Pruebas de integracion obligatorias
> 
> - `AppUploadDocumental` + `AppUploadBatchView` renderizan la vista completa;
> - metadata documental aparece por fila;
> - cambio de tipologia actualiza solo el archivo correspondiente;
> - cambio de fecha actualiza solo el archivo correspondiente;
> - flujo `init -> chunks -> complete -> almacenar` por archivo;
> - multiples archivos generan multiples POST finales;
> - respuesta backend rica se transforma en eventos `UploadDocumentalInterfaceRegistration`;
> - error en un archivo no corrompe los demas;
> - cancelacion durante chunks aborta y marca estado.
> 
> ## Pruebas de navegador obligatorias
> 
> - seleccionar 5 archivos como en legacy;
> - ver contador correcto;
> - abrir preview de PDF;
> - cambiar tipologia por archivo;
> - ingresar fecha por archivo;
> - eliminar uno;
> - eliminar todos;
> - guardar individual;
> - guardar todos;
> - probar archivo invalido por extension;
> - probar archivo invalido por tamano;
> - probar retry tras error simulado.
> 
> ## Criterios de aceptacion
> 
> - `AppUploadDocumental` existe y renderiza la vista documental final.
> - La vista usa `AppUploadBatchView`, no HTML legacy.
> - La seleccion usa `AppUpload`.
> - Configuracion de tamano/tipos viene de API o loader obligatorio.
> - Tipologias vienen de API o loader obligatorio.
> - Cada archivo tiene tipologia independiente.
> - La sugerencia por nombre funciona y es sobreescribible.
> - Fecha por archivo funciona cuando aplica.
> - Guardar individual funciona.
> - Guardar todos funciona con `AppProgressBatch`.
> - Upload usa API nueva por chunks.
> - Registro final se hace una vez por archivo.
> - Retornos de interfaz se emiten como eventos tipados, no como callbacks string.
> - Mapper de retornos de interfaz probado de forma aislada.
> - Cancelacion y retry quedan soportados.
> - Sin `any` nuevo.
> - Sin jQuery, Bootstrap manual, WebForms ni `.ashx`.
> - Backend no fue modificado.
> 
> ## Documentacion obligatoria
> 
> Crear:
> 
> ```txt
> src/modules/almacenamientoDocumental/components/AppUploadDocumental/README.md
> ```
> 
> Debe incluir:
> 
> - objetivo;
> - props;
> - ejemplo embebido;
> - ejemplo modal;
> - loaders requeridos;
> - flujo de upload;
> - matriz campo frontend/backend;
> - politica de tipologia por archivo;
> - politica de fecha;
> - contrato de retorno para registro en interfaz;
> - politica de errores y retry;
> - limites conocidos.
> 
> Actualizar documentacion de arquitectura si se descubre:
> 
> - endpoint real de configuracion;
> - endpoint real de tipologias;
> - campos backend adicionales;
> - diferencia de contrato frente a DTOs revisados.
> 
> ## Entrega esperada
> 
> - Diff de componente, hooks, servicios, utils y tests.
> - Evidencia de tests ejecutados.
> - Matriz FE-BE campo a campo.
> - Resumen tecnico de:
>   - composicion visual;
>   - validacion por API;
>   - tipologia por archivo;
>   - flujo por chunks;
>   - progreso batch;
>   - cancelacion;
>   - retry;
>   - retornos de interfaz;
>   - mapper de registro visual.
> - Confirmacion explicita:
>   - backend no modificado;
>   - endpoints no modificados;
>   - `AppUpload` no reemplazado;
>   - `AppUploadBatchView` usado como base;
>   - `AppProgressBatch` usado para lotes;
>   - retornos de interfaz modelados como eventos tipados;
>   - mapper de retornos de interfaz implementado y probado;
>   - no queda dependencia runtime legacy.
> 
> ## Instruccion final
> 
> Implementar `AppUploadDocumental` como una especializacion documental completa sobre `AppUploadBatchView`, `AppUpload`, `AppProgressBatch` y el cliente nuevo de almacenamiento, cubriendo seleccion multiple, preview, tipologia por archivo, fecha por archivo, validacion desde API, guardar individual, guardar todos, upload por chunks, registro final individual, normalizacion de retornos para registro en interfaz, cancelacion, retry, callbacks tipados y una UX enterprise moderna sin migrar dependencias legacy.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium

## Capabilities

### New Capabilities
- `app-appuploaddocumental`: Componente reusable AppAppuploaddocumental para la capa UI compartida del proyecto.

### Modified Capabilities
- 

## Impact

- Nuevo componente compartido en `src/app/Components/UI/AppAppuploaddocumental/`.
- Posible integracion inicial en un modulo consumidor real del proyecto.
- Nuevas pruebas de comportamiento para el contrato reusable del componente.
