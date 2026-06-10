# Jira Context - SCRUMCORE-240

## Summary

MODULO-REUSABLE-DIGITALIZACIONDOCUMENTAL- DYNAMSOFT-ADAPTER-02FE

## Description

> # PROMPT IMPLEMENTACION - Dynamsoft Adapter DigitalizacionDocumental
> # Fase FE-02 - Infraestructura scanner PDF-only
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ROL
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Actua como Arquitecto Frontend senior especialista en:
> 
> - React 19
> - TypeScript estricto
> - integracion SDK browser
> - Dynamsoft Web TWAIN
> - adapters de infraestructura
> - runtime validation
> - state orchestration
> - manejo seguro de licencias/scripts
> - UX de dispositivos locales
> - testing enterprise
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## OBJETIVO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Implementar la capa de infraestructura que encapsula Dynamsoft para `DigitalizacionDocumental`.
> 
> El modulo React NO debe acceder directamente a:
> 
> ```txt
> DWObject
> ```
> 
> La salida funcional obligatoria del adapter sera:
> 
> ```txt
> PDF
> ```
> 
> No se permiten formatos finales:
> 
> - TIF;
> - JPG;
> - PNG;
> - BMP.
> 
> El adapter debe exponer una interfaz estable, reusable y desacoplada del SDK.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## CONTEXTO OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Legacy fuente:
> 
> ```txt
> Resources/online_demo_initpage.js
> Resources/online_demo_operation.js
> workflow/WebFormEscan.aspx
> js/workflow/WebFormEscan.js
> ```
> 
> Decision confirmada:
> 
> ```txt
> Existe licencia Dynamsoft.
> TIF/JPG/BMP quedan descartados como formato final.
> Salida unica: PDF.
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## UBICACION ESPERADA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> ```txt
> src/modules/digitalizacion/infrastructure/dynamsoft/
> ├─ DynamsoftTwainClient.ts
> ├─ loadDynamsoftScripts.ts
> ├─ dynamsoft.types.ts
> ├─ dynamsoft.errors.ts
> └─ dynamsoft.constants.ts
> ```
> 
> Hook consumidor:
> 
> ```txt
> src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts
> ```
> 
> Tests:
> 
> ```txt
> src/modules/digitalizacion/tests/*
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## RESTRICCIONES OBLIGATORIAS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> PROHIBIDO:
> 
> - usar `any`;
> - exponer `DWObject` fuera del adapter;
> - cargar scripts multiples veces;
> - dejar listeners vivos al desmontar;
> - generar TIF/JPG/BMP como salida final;
> - persistir licencia en logs;
> - exponer tokens/licencia en errores;
> - bloquear UI indefinidamente;
> - acceder a SDK desde UI.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## REGLA ARQUITECTONICA OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> `DigitalizacionDocumental` debe consumir exclusivamente:
> 
> ```txt
> DigitalizacionScannerClient
> ```
> 
> La UI NO puede:
> 
> - conocer `DWObject`;
> - conocer SDK internamente;
> - acceder a APIs Dynamsoft.
> 
> Toda interaccion debe pasar por el adapter.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## SOURCE OF TRUTH OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Las paginas capturadas son la unica fuente valida para generacion PDF.
> 
> PROHIBIDO generar PDF desde:
> 
> - miniaturas;
> - contadores UI;
> - estados visuales;
> - previews.
> 
> Solo:
> 
> ```txt
> ScanPage[]
> ```
> 
> puede alimentar `generatePdf()`.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## CONTRATO ADAPTER OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Definir interfaz:
> 
> ```ts
> export type ScannerDevice = {
>   id: string;
>   name: string;
> };
> 
> export type ScanPage = {
>   id: string;
>   index: number;
>   thumbnailUrl?: string;
> };
> 
> export type ScanOptions = {
>   deviceId: string;
>   resolutionDpi?: number;
>   colorMode?: "color" | "grayscale" | "blackWhite";
>   duplex?: boolean;
>   removeBlankPages?: boolean;
> };
> 
> export type PdfGenerationResult = {
>   file: File;
>   pageCount: number;
> };
> 
> export interface DigitalizacionScannerClient {
>   initialize(): Promise<void>;
>   listDevices(): Promise<ScannerDevice[]>;
>   selectDevice(deviceId: string): Promise<void>;
>   scan(options: ScanOptions): Promise<ScanPage[]>;
>   rotatePage(pageId: string, degrees: 90 | 180 | 270): Promise<void>;
>   removePage(pageId: string): Promise<void>;
>   clear(): Promise<void>;
>   generatePdf(fileName: string): Promise<PdfGenerationResult>;
>   dispose(): Promise<void>;
> }
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## VALIDACION CONTRACTUAL RUNTIME OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Validar explicitamente:
> 
> Scripts:
> 
> - Dynamsoft disponible;
> - scripts cargados;
> - licencia presente;
> - runtime service disponible.
> 
> Scanner:
> 
> - `deviceId` obligatorio;
> - scanner seleccionado;
> - scanner disponible.
> 
> ScanOptions:
> 
> - `resolutionDpi` valida;
> - `colorMode` valido;
> - `duplex` valido.
> 
> Generacion PDF:
> 
> - paginas > 0;
> - `pageCount > 0`;
> - `file != null`;
> - tamano > 0;
> - extension `.pdf`;
> - mime type `application/pdf`.
> 
> Nunca asumir:
> 
> - `deviceId` valido;
> - scanner disponible;
> - paginas existentes;
> - PDF generado correctamente.
> 
> Si falta informacion:
> 
> - abortar flujo;
> - error funcional controlado.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ERRORES FUNCIONALES TIPADOS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> ```txt
> DYNAMSOFT_SCRIPT_LOAD_FAILED
> DYNAMSOFT_RUNTIME_UNAVAILABLE
> DYNAMSOFT_LICENSE_INVALID
> SCANNER_NOT_SELECTED
> SCANNER_NOT_FOUND
> SCAN_IN_PROGRESS
> SCAN_CANCELLED
> SCAN_FAILED
> PDF_EMPTY
> PDF_GENERATION_FAILED
> INVALID_SCAN_OPTIONS
> INVALID_DEVICE_ID
> STALE_OPERATION_IGNORED
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## ANTI-STALE OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Si:
> 
> ```txt
> initialize()
> scan()
> generatePdf()
> ```
> 
> terminan despues de:
> 
> ```txt
> dispose()
> ```
> 
> Entonces:
> 
> - ignorar response;
> - no mutar estado;
> - no recrear listeners;
> - no recrear scanner state.
> 
> Nunca actualizar estado desmontado.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## CONCURRENCIA OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> No pueden coexistir:
> 
> 1. `scan()` + `scan()`;
> 2. `scan()` + `generatePdf()`;
> 3. `generatePdf()` + `generatePdf()`.
> 
> Si existe operacion activa:
> 
> - bloquear nueva operacion;
> - retornar error funcional.
> 
> Ejemplo:
> 
> ```txt
> SCAN_IN_PROGRESS
> ```
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## OWNERSHIP DE PAGINAS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Cada pagina debe poseer:
> 
> ```ts
> id: string;
> ```
> 
> estable y unico.
> 
> El adapter debe operar mediante:
> 
> ```txt
> pageId
> ```
> 
> NO mediante:
> 
> - indices visuales;
> - posiciones UI;
> - referencias DOM.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## HOOK OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Crear `useDigitalizacionScanner()` con:
> 
> - estado `idle | initializing | ready | scanning | generatingPdf | error`;
> - lista scanners;
> - scanner seleccionado;
> - paginas;
> - error funcional;
> - loading;
> - acciones:
>   - `initialize`;
>   - `selectDevice`;
>   - `scan`;
>   - `removePage`;
>   - `rotatePage`;
>   - `clear`;
>   - `generatePdf`;
>   - `dispose`.
> 
> Debe proteger stale updates al desmontar.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## UX OBLIGATORIA
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> El hook debe permitir que la UI muestre:
> 
> - no runtime service;
> - licencia invalida;
> - sin scanners;
> - scanner seleccionado;
> - escaneando;
> - generando PDF;
> - paginas capturadas;
> - error recuperable;
> - reintentar inicializacion.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## MANEJO DE ERRORES OBLIGATORIO
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Caso A: script falla
> 
> - bloquear scanner;
> - permitir retry.
> 
> Caso B: runtime no disponible
> 
> - mostrar error funcional.
> 
> Caso C: PDF vacio
> 
> - impedir `generatePdf()`.
> 
> Caso D: operacion stale
> 
> - ignorar resultado;
> - no mutar estado.
> 
> Caso E: scanner desconectado
> 
> - invalidar seleccion;
> - error recuperable.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## PRUEBAS OBLIGATORIAS
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Unitarias:
> 
> - initialize OK;
> - script falla;
> - runtime no disponible;
> - licencia invalida;
> - lista scanners vacia;
> - scanner seleccionado;
> - deviceId invalido;
> - scan bloqueado sin scanner;
> - scan OK agrega paginas;
> - removePage funciona;
> - rotatePage funciona;
> - clear funciona;
> - PDF sin paginas falla;
> - PDF valido retorna File;
> - mime PDF correcto;
> - dispose limpia estado/listeners;
> 
> Validacion contractual:
> 
> - deviceId requerido;
> - pageCount valido;
> - PDF valido;
> - scan options validas.
> 
> Concurrencia:
> 
> - scan concurrente bloqueado;
> - generatePdf concurrente bloqueado;
> - scan + generatePdf bloqueado.
> 
> Anti-stale:
> 
> - initialize stale ignorado;
> - scan stale ignorado;
> - generatePdf stale ignorado.
> 
> Integracion:
> 
> - hook usa adapter;
> - UI recibe estados correctos;
> - errores funcionales visibles.
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
> - arquitectura adapter;
> - Mermaid;
> - lifecycle;
> - errores;
> - source-of-truth;
> - concurrencia.
> 
> 2. `SCRUMCORE-[XX]-Implementacion-Detallada.md`
> 
> Debe incluir:
> 
> - adapter;
> - hook;
> - runtime validation;
> - anti-stale;
> - ownership de paginas.
> 
> 3. `SCRUM-[XX]-Integracion-BackEnd.md`
> 
> Debe incluir:
> 
> - licencia;
> - runtime;
> - SDK integration;
> - futuros contratos backend.
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
> - version;
> - fecha;
> - control cambios;
> - referencias cruzadas.
> 
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> ## INSTRUCCION FINAL
> ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
> 
> Implementar el adapter Dynamsoft de forma completamente aislada, tipada y PDF-only, garantizando validacion contractual runtime estricta, proteccion anti-stale, control de concurrencia, ownership correcto de paginas y una interfaz estable para `DigitalizacionDocumental` sin exponer nunca `DWObject` al resto del sistema.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: 02-FE, ADAPTER, DIGITALIZACIONDOCUMENTAL, DYNAMSOFT, MODULOS, REUSABLE
