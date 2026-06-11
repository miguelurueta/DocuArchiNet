# SCRUMCORE-239 Legacy Traceability

## Alcance

Este documento registra la trazabilidad funcional usada por `SCRUMCORE-245` para corregir la migracion de `DigitalizacionDocumental` desde el legacy.

Archivos leidos en esta sesion:

- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\online_demo_initpage.js`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\online_demo_operation.js`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\WebFormEscan.js`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\WebFormEscan.aspx`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\WebFormEscan.aspx.vb.txt`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\Webform_save_digital_image.aspx`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\Webform_save_digital_image.aspx.vb.txt`

Rutas legacy no accesibles desde esta sesion:

- `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\WebFormEscan.aspx`
- `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\WebFormEscan.aspx.vb`
- `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\Webform_save_digital_image.aspx`
- `D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\workflow\Webform_save_digital_image.aspx.vb`

Impacto: se usaron las copias locales disponibles en `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner`. Si esas copias no corresponden exactamente al ultimo legacy productivo, la equivalencia debe confirmarse con el repositorio legacy original.

## Matriz de trazabilidad

| Archivo legacy | Funcion/metodo/evento | Responsabilidad | Regla funcional | Nueva ubicacion React/API | Estado | Evidencia |
| --- | --- | --- | --- | --- | --- | --- |
| `online_demo_initpage.js` | `pageonload` | Inicializar UI de scanner y combo de dispositivos. | El digitalizador debe preparar controles y fuentes antes de capturar. | `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts`; `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`; `DigitalizacionDocumentalWorkspace` | migrado | `pageonload` llama `initCustomScan` y prepara `source`; React inicializa con `initialize()` y lista dispositivos. |
| `online_demo_initpage.js` | `Dynamsoft_OnReady` | Obtener `DWObject`, configurar viewer, listar scanners/webcam y registrar eventos. | La UI no debe operar hasta que Dynamsoft este listo y existan fuentes. | `DynamsoftTwainClient`; `loadDynamsoftScripts`; `useDigitalizacionScanner` | migrado | El adapter encapsula Dynamsoft; la UI solo consume `DigitalizacionScannerClient`. |
| `online_demo_initpage.js` | `SaveScanSettings` / `LoadScanSettings` | Persistir configuracion de scanner y preferencias de interfaz. | Cambios de ADF, duplex, pagina en blanco, deskew, bordes y vista pueden requerir persistencia. | `GET /api/gestor-documental/digitalizacion/configuracion`; pendiente API de actualizacion de preferencias si negocio la exige. | pendiente backend | Legacy usa cookies, globals y `Service_actualiza_estructura_interface_digitalizacion`. |
| `WebFormEscan.js` | `Service_solicita_estructura_interface_digitalizacion` | Consultar configuracion de digitalizacion por radicado/contexto. | Configuracion define lista obligatoria, formato, resolucion, tonalidad y opciones de scanner. | `getDigitalizacionConfiguracion`; `useDigitalizacionConfiguracion` | reemplazado por API | Endpoint moderno: `/api/gestor-documental/digitalizacion/configuracion`. |
| `WebFormEscan.js` | `Configura_iterface_digitalizacion` | Mostrar/ocultar formatos y tonalidades permitidas. | La salida moderna descarta TIF/JPG/BMP como formato final y fuerza PDF. | `DigitalizacionDocumentalWorkspace`; `DynamsoftTwainClient.generatePdf` | descartado por decision | SCRUMCORE-239 decide PDF-only; no se conserva seleccion final TIF/JPG/BMP. |
| `online_demo_operation.js` | `source_onchange` | Seleccionar fuente TWAIN/webcam. | El usuario debe seleccionar scanner antes de capturar. | `DigitalizacionDocumentalWorkspace` select `Scanner`; `useDigitalizacionScanner.selectDevice` | migrado | Workspace lista `scanner.devices` y llama `selectDevice`. |
| `online_demo_operation.js` | `acquireImage` y llamadas a `DWObject.AcquireImage` | Capturar paginas desde scanner. | Captura puede crear nuevo lote, agregar, insertar o reemplazar paginas. | `useDigitalizacionScanner.scan`; `DynamsoftTwainClient.scan` | migrado parcial | React cubre captura de paginas; insertar/reemplazar pagina individual queda pendiente si se confirma como requisito moderno. |
| `online_demo_operation.js` | `btnRotateRight_onclick`, `btnRotateLeft_onclick`, `btnRotate180_onclick`, `ini_rotate_paginas_miniaturas` | Rotar paginas capturadas. | El usuario puede corregir orientacion antes de generar PDF. | `DigitalizacionDocumentalWorkspace`; `useDigitalizacionScanner.rotatePage` | migrado parcial | Workspace ofrece rotacion basica de 90 grados sobre pagina seleccionada. |
| `online_demo_operation.js` | `btnRemoveCurrentImage_onclick`, `btnRemoveAllImages_onclick` | Eliminar paginas o limpiar buffer. | El usuario puede descartar paginas antes de guardar. | `DigitalizacionDocumentalWorkspace`; `useDigitalizacionScanner.removePage`; `useDigitalizacionScanner.clear` | migrado | Workspace ofrece eliminar pagina seleccionada y limpiar lote. |
| `online_demo_operation.js` | `btnCrop_onclick`, area selection events | Recortar area seleccionada. | Recorte existe en legacy, pero no fue parte del minimo frontend implementado en SCRUMCORE-240/241/242. | Pendiente en adapter/UI si negocio lo exige. | pendiente frontend | Legacy usa `DWObject.Crop` y eventos `OnImageAreaSelected`. |
| `online_demo_operation.js` | `des_kew`, blank page checks | Deskew y descarte de paginas en blanco. | Correcciones automaticas dependen de configuracion de digitalizacion. | Pendiente en adapter/API de configuracion. | pendiente frontend | Legacy usa `IfAutoDiscardBlankpages`, `BlankImageMaxStdDev` e `IsBlankImageExpress`. |
| `online_demo_operation.js` | `Gurdar_documento_htpp_server` / `docu_save_html` | Exportar imagenes y subir al servidor WebForms. | El PDF generado se subia a `Webform_save_digital_image.aspx`; legacy tambien permitia TIF/JPG/BMP. | `uploadPdfTemporal`; `digitalizacionUploadTemporal.api.ts` | reemplazado por API | Moderno usa upload temporal por chunks y salida PDF-only. |
| `online_demo_operation.js` | `OnHttpUploadSuccess` / `OnHttpUploadFailure` | Manejo de exito/error de upload HTTP. | Errores deben ser controlados y visibles. | `DigitalizacionApiContractError`; hooks de operacion API; `DigitalizacionDocumentalWorkspace` alertas | migrado | Workspace muestra errores de scanner, contexto y API con `role="alert"`. |
| `Webform_save_digital_image.aspx.vb.txt` | `Page_Load` | Crear/limpiar ruta temporal de `Session`, guardar `RemoteFile`. | Upload temporal legacy depende de `WF_RUTA_TEMPO_ESCANER` y `Session`. | Upload temporal moderno `/api/gestor-documental/almacenamiento/upload-temporal/...` | reemplazado por API | Moderno no usa `Session` como contrato funcional. |
| `WebFormEscan.aspx` | Includes scripts y toolbar WebForms/jQuery | Pantalla acoplada con Bootstrap, jQuery, AjaxControlToolkit y controles ASP.NET. | La nueva experiencia no debe reutilizar WebForms ni jQuery. | `DigitalizacionDocumentalWorkspace` y `DigitalizacionDocumentalModal` | migrado | Workspace React reemplaza toolbar, miniaturas, preview y metadata. |
| `WebFormEscan.aspx.vb.txt` | `Page_Load` | Resolver modo por `Session.Item("DG_TIPODIGITALIZACION")`. | Modos legacy: tramite, adjunto workflow, produccion, migracion, tramite simple, reemplazo version. | `DigitalizacionContext.modo`; APIs de configuracion/metadata/documentos | migrado parcial | Contrato moderno cubre `crear` y `adjuntar`; migracion/reemplazo version quedan fuera o en APIs separadas. |
| `WebFormEscan.aspx.vb.txt` | `Button_guardar_documento_Click` / `Button_guardar_popup_Click` | Validar lista de chequeo/tipologia y preparar guardado. | Lista de chequeo puede ser obligatoria y unica por contexto. | `resolveDigitalizacionMetadata`; `crearDocumentoDigitalizado` | reemplazado por API | Backend moderno debe ser fuente de verdad para metadata y unicidad. |
| `WebFormEscan.aspx.vb.txt` | `Button_adjuntar_Click` | Validar si un documento seleccionado puede recibir digitalizacion. | Adjuntar requiere seleccion destino valida. | `validarAdjuntarDigitalizacion` | reemplazado por API | Endpoint moderno: `/api/gestor-documental/documentos/{id}/adjuntar-digitalizacion/validacion`. |
| `WebFormEscan.aspx.vb.txt` | `ClassWorkflowDigitalizacion.Valida_adjuntar_documento_digitalizado` | Validar restricciones de adjuntar. | Bloquear documento firmado, bloqueado, radicado no modificable o no PDF. | Backend `AdjuntarDigitalizacionController` propuesto | pendiente backend | El frontend ya llama validacion; la regla final debe ejecutarse en backend. |
| `WebFormEscan.aspx.vb.txt` | `ClassAñadirDocumento.Añade_documento_digitalizado` | Adjuntar paginas digitalizadas a documento existente. | Adjuntar modifica PDF existente, no crea documento nuevo. | `adjuntarDigitalizacion`; backend merge PDF | pendiente backend | Frontend tiene contrato; merge atomico depende de API backend. |

## Decisiones de migracion

- `online_demo_initpage.js` queda reemplazado por el adapter `DynamsoftTwainClient`, el loader `loadDynamsoftScripts` y el hook `useDigitalizacionScanner`.
- `online_demo_operation.js` queda dividido en operaciones React/API: scanner, edicion basica, PDF-only y upload temporal moderno.
- `WebFormEscan.js` queda reemplazado por servicios API de configuracion, lista de chequeo y metadata.
- `WebFormEscan.aspx` queda reemplazado por componentes React. No se migra jQuery, AjaxControlToolkit ni controles ASP.NET.
- `WebFormEscan.aspx.vb` queda reemplazado por contrato `DigitalizacionContext` y APIs backend. No se migra `Session`.
- `Webform_save_digital_image.aspx(.vb)` queda reemplazado por upload temporal moderno.
- TIF/JPG/BMP se descartan como salida final por decision de SCRUMCORE-239; solo PDF sigue vigente.

## Gap analysis SCRUMCORE-239 a SCRUMCORE-245

| Capacidad | Estado actual | Ubicacion |
| --- | --- | --- |
| Adapter React para Dynamsoft | Implementado | `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts` |
| Carga de scripts/runtime | Implementado | `src/modules/digitalizacion/infrastructure/dynamsoft/loadDynamsoftScripts.ts` |
| Inicializacion y listado de scanners | Implementado | `useDigitalizacionScanner`; `DigitalizacionDocumentalWorkspace` |
| Seleccion y captura | Implementado | `DigitalizacionDocumentalWorkspace` |
| Miniaturas y preview | Implementado | `DigitalizacionDocumentalWorkspace` |
| Rotacion basica y eliminacion | Implementado | `DigitalizacionDocumentalWorkspace` |
| PDF-only | Implementado | `DynamsoftTwainClient.generatePdf`; servicios validan PDF |
| Upload temporal moderno | Implementado frontend | `digitalizacionUploadTemporal.api.ts` |
| Resolver metadata/lista chequeo | Implementado frontend contra API | `digitalizacionMetadata.api.ts`; `digitalizacionListaChequeo.api.ts` |
| Crear documento digitalizado | Implementado frontend contra API | `digitalizacionDocumentos.api.ts` |
| Adjuntar digitalizacion | Implementado frontend contra API | `adjuntarDigitalizacion.api.ts` |
| Validar firmado/bloqueo/radicado | Pendiente backend | API propuesta; frontend consume validacion |
| Workspace inline sin modal | Implementado en SCRUMCORE-245 | `DigitalizacionDocumentalWorkspace` |
| Modal reutilizable | Conservado | `DigitalizacionDocumentalModal` como wrapper de `AppModal` |

## Componente corporativo final

Para embebido directo en `CapDocument`, `Correspondencia`, `Workflow`, `Ventanilla` o `Archivo Central`, usar:

```tsx
import { DigitalizacionDocumentalWorkspace } from "src/modules/digitalizacion";
```

El wrapper modal sigue disponible para flujos overlay:

```tsx
import { DigitalizacionDocumentalModal } from "src/modules/digitalizacion";
```
