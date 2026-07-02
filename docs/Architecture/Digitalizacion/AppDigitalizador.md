# Arquitectura y flujo operativo de AppDigitalizador

Este documento describe el funcionamiento real de **AppDigitalizador** y de su ruta de escaneo en la UI actual, incluyendo toolbars, herramientas, casos de uso, manejo de errores y puntos de corrección.

## 1) Entrada y montaje de la pantalla de digitalización

Ruta principal para Radicación en este repo:
- `src/modules/radicacion/components/CapDocument.tsx:24`
- `src/modules/radicacion/components/CapDocument.tsx:25`
- `src/modules/radicacion/components/CapDocument.tsx:26`
- `src/modules/radicacion/components/CapDocument.tsx:90`

`CapDocument`:
- Construye `scannerClient` vía `useAppDigitalizadorScannerClient` con licencia de entorno:
  - `src/modules/radicacion/components/CapDocument.tsx:26`
  - `src/modules/radicacion/components/CapDocument.tsx:27`
- Inyecta `DigitalizacionDocumentalWorkspace` directamente con:
  - `scannerClient`
  - `context`
  - `onCompleted`, `onError`
  - `toolbarHost` para render de toolbar por portal
  - `src/modules/radicacion/components/CapDocument.tsx:90`-`96`

Si se usa `AppDigitalizador` (componente contenedor):
- `src/app/Components/UI/AppDigitalizador/AppDigitalizador.tsx:16`
- Crea cliente con proveedor y licencia -> `useAppDigitalizadorScannerClient`
  - `src/app/Components/UI/AppDigitalizador/AppDigitalizador.tsx:30`-`35`
- Pasa ese cliente a `DigitalizacionDocumentalWorkspace`:
  - `src/app/Components/UI/AppDigitalizador/AppDigitalizador.tsx:77`-`85`

## 2) Provider y cliente de scanner

### AppDigitalizadorProvider
- `src/app/Components/UI/AppDigitalizador/AppDigitalizadorProvider.tsx:9`-`26`
- Expone por contexto:
  - `apiClient`
  - `dynamsoft`
  - `createScannerClient` (opcional override)

### Contexto por defecto
- `src/app/Components/UI/AppDigitalizador/AppDigitalizador.context.ts:5`-`11`
- `createScannerClient` por defecto -> `new DynamsoftTwainClient(options)`
  - `src/app/Components/UI/AppDigitalizador/AppDigitalizador.context.ts:5`-`7`

### Creación de cliente
- `src/app/Components/UI/AppDigitalizador/hooks/useAppDigitalizadorScannerClient.ts:9`-`39`
- Si viene `scannerClient` por props -> se usa directamente.
- Si no viene:
  - resuelve licencia en orden:
    - `licenciaDynamsoft`
    - `dynamsoft?.licenseKey`
    - `providerDynamsoft?.licenseKey`
  - arma `runtimeOptions`
  - llama `createScannerClient?.(runtimeOptions)`

## 3) Alerta de licencia y paso a workspace

- En `AppDigitalizador`, `missingLicense` se calcula cuando no hay scanner externo ni licencia:
  - `src/app/Components/UI/AppDigitalizador/AppDigitalizador.tsx:47`-`52`
- Mensaje de UI de licencia pendiente:
  - `src/app/Components/UI/AppDigitalizador/AppDigitalizador.tsx:70`-`74`

## 4) Punto de entrada del flujo de scanner

`DigitalizacionDocumentalWorkspace` es el núcleo UI + estado de scanner:
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:308`-`316`

Instancias el hook:
- `useDigitalizacionScanner({ client: scannerClient })`
  - `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:372`-`386`

Inicialización automática al montar el workspace:
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:434`-`438`
- Llama a `initialize()`.

## 5) Flujo de estado del hook `useDigitalizacionScanner`

Archivo:
- `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts`

### Estructura de estado
- `state` inicial: `devices`, `pages`, `selectedDeviceId`, `pdf`, `progress`, `error`, `status`
  - `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:35`-`43`

### initialize()
- `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:107`-`134`
- Secuencia:
  1. estado `initializing`
  2. `await client.initialize()`
  3. `await client.listDevices()`
  4. `status: ready`, `devices` actualizados o `error`.
- Si cae excepción: `handleError(...)` -> `status: error` con código `SCAN_FAILED`.
  - `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:131`-`133`

### selectDevice()
- `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:136`-`155`
- Llama `client.selectDevice(deviceId)` y guarda `selectedDeviceId`.

### scan()
- `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:157`-`196`
- Establece `status: scanning`, delega a `client.scan(...)`.
- En éxito: actualiza `pages`, limpia `pdf`, pone `status: ready`.
- En error: `handleError(..., "No fue posible completar el escaneo.")`.

### mutate pages
- `removePage` `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:198`-`216`
- `reorderPages` `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:220`-`243`
- `duplicatePage` `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:245`-`267`
- `rotatePage` `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:269`-`292`
- `deskewPage` `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:294`-`327`
- `cropPage` `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:329`-`349`
- `generatePdf` `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:367`-`406`
- `dispose` `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:408`-`414`

## 6) Client Dynamsoft (`DynamsoftTwainClient`)

### initialize()
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:377`-`436`
- Carga scripts CSS/JS -> valida runtime -> valida licencia -> configura container -> carga runtime -> `Load()`, espera `GetWebTwain`.
- Errores mapeables:
  - scripts/carga de CSS/runtime
  - licencia faltante -> `DYNAMSOFT_LICENSE_INVALID`

### listDevices()
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:438`-`474`
- Primer intento: SourceManager (`SourceCount`, `GetSourceNameItems`)
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:476`-`502`
- Fallback: `GetDevicesAsync`
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:445`-`450`

### selectDevice()
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:504`-`562`
- Lógica dual:
  - `SelectDeviceAsync` para devices modernos
  - `SelectSourceByIndex` para legado/source manager.

### scan()
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:564`-`712`
- Configura `AcquireImage`.
- Si `removeBlankPages`:
  - `IfAutoDiscardBlankpages` + aplicación de detección antes y después de operaciones automáticas.
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:605` y `640`-`669` y `696`-`699`.

### remove blank pages (Dynamsoft + fallback propio)
- Primario: `removeDetectedBlankPagesWithDynamsoft` (`IsBlankImageExpress` si existe)
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1416`-`1521`
- Fallback: `removeDetectedBlankPages` (análisis heurístico de imagen)
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1284`-`1414`
- Análisis de píxeles:
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1540`-`1800`

## 7) Toolbars y acciones (UI)

### Barra principal (toolgroup captura)
Definida en `toolbarElement`:
- archivo base: `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:1254`
- Sección principal:
  - `Captura`:
    - **Escanear / Nuevo documento** (`handlePrimaryCapture`) -> `handleScan` o `handleNewCapture` según estado de `hasPages`.
    - `src/...:1255`-`1318`, handlers `handleScan` (`498`-`500`) y `handleNewCapture` (`502`-`525`) y `handleAppendCapture` (`546`-`548`), `handleInsertCapture` (`535`-`553`), `handleReplaceCapture` (`527`-`533`)
  - `Salida`:
    - **Generar PDF** -> `handleGeneratePdf` (`824`-`830`)
- Estado de bloqueo:
  - si no hay dispositivo (`scanner.selectedDeviceId`) o error de contexto, carga, etc.

### Panel de miniaturas (izquierdo)
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:1375`-`1437`
- Cada thumbnail:
  - selección (`handleThumbnailClick`)
  - selección múltiple por checkbox (`handleTogglePageSelection`)
  - drag/drop reordenar (`handleThumbnailDragStart`, `handleThumbnailDragOver`, `handleThumbnailDrop`, `handleThumbnailDragEnd`)
  - acciones de selección completa / limpiar (`handleSelectAllPages`, `handleClearPageSelection`)

### Preview principal y toolbar de preview
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:1471`-`1709`
- Control de edición:
  - rotar izquierda/derecha -> `handleRotateSelected(270/90)` (`1490`-`1497`)
  - deskew -> `handleDeskewSelected` (`1502`-`1506`)
  - duplicar -> `handleDuplicateSelected` (`1510`-`1515`)
  - selección área -> toggle `areaSelectionEnabled`
  - limpiar -> `handleClear`
  - eliminar -> `handleRemoveSelected` (`1530`-`1537`)
  - crop actions sobre selección manual -> `handleApplyCropSelection`, `handleResetCropSelection`, `handleCancelCropSelection` (`1656`-`1691`)
- Control de visualización:
  - zoom in/out (`handleZoomIn`, `handleZoomOut`) (`1562`-`1570`)
  - fit width/fit page (`handleFitWidth`, `handleFitPage`) (`1574`-`1588`)
  - fullscreen (`handleTogglePreviewExpanded`) (`1589`-`1597`)
- Navegación:
  - `PageNavigatorFloating` -> `handleGoToFirstPage`, `handleGoToPreviousPage`, `handleGoToNextPage`, `handleGoToLastPage`, `handleGoToPage` (`800`-`822`, `785`-`806`).

### Overlay de progreso / errores
- Progreso:
  - `activeProgress` y `activeProgressLabel` (`1246`-`1254`)
  - `scanProgressOverlay` con botón cancelar -> `handleCancel` (`1720`-`1743`)
- Errores:
  - `state.validationError`, `scanner.error`, `operation.error` (`1350`-`1364`)

### Organizador de páginas (overlay)
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:1767`-`1805`
- Acciones:
  - rotar selección -> `handleRotateOrganizerSelection`
  - deskew selección -> `handleDeskewOrganizerSelection`
  - eliminar selección -> `handleRemoveOrganizerSelection`
  - cerrar -> `handleClosePageOrganizer`
- Selección por teclado no directo, selección por click/checkbox en grid y drag reorder ya maneja ids por selección visual.

### Barra de configuración (derecha)
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:1871`-`2023`
- Dispositivos:
  - `select` de dispositivo -> `selectDevice(scannerId)` (`1885`-`1890`)
- Configuración captura:
  - modo (`docuarchi`/`driver`) (`1925`-`1930`)
  - ADF/duplex (`1932`-`1942`)
  - **Eliminar páginas en blanco** -> `setRemoveBlankPages` (`1943`-`1950`)
  - deskew/autoCrop/autoRotate (`1951`-`1976`)
  - color y resolución (`1978`-`2006`)
  - driver mode -> botón **Configurar scanner** -> `handleScan` (`2010`-`2018`)

## 8) Casos de uso funcionales

### Caso de uso: escaneo normal (DocuArchi)
1. Usuario entra a CapDocument o AppDigitalizador.
2. `initialize()` carga runtime y dispositivos.
3. Selecciona scanner.
4. Clic en **Escanear** -> `scan()` -> opción por defecto `showScannerUi=false`.
5. Revisión `scanner.pages`, ajustes de imagen si aplica.
6. Generar PDF y submit.

### Caso de uso: usar modo PaperStream
1. En configuración: modo `driver`.
2. `handlePrimaryCapture` suele usar flujo con `handleScan`.
3. `scan` recibe `showScannerUi=true` y respeta `captureOperation` según botón usado.

### Caso de uso: reemplazar/insertar/append
1. Selección página + botón de acción.
2. `executeCapture` arma `captureOperation`:
   - `NEW` -> nueva captura
   - `REPLACE` -> página actual
   - `INSERT_BEFORE`, `INSERT_AFTER`
   - `APPEND` -> al final
   - `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:527`-`548`
3. `handleCapture` -> `scan({ captureOperation, ... })`.
4. Cliente resuelve mezcla en `resolveCaptureOperationPages`:
   - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1231`-`1282`.

### Caso de uso: eliminar hoja en blanco
1. Usuario activa configuración `removeBlankPages`:
   - `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:1943`-`1950`.
2. Al escanear, cliente setea `IfAutoDiscardBlankpages` + umbrales + `removeDetected...`:
   - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:605`-`612`
   - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:744`-`766`
3. Aplicación de eliminación:
   - nativo si hay `IsBlankImageExpress`:
     - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1416`-`1521`
   - fallback por análisis de imagen:
     - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1284`-`1414`
4. Si falla o no detecta, sigue con resultado de páginas escaneadas.

## 9) Puntos de error y correcciones (posibles)

### A) Estado sin cliente / fallback incorrecto
- Riesgo: si se pasa `unavailableScannerClient`, muchas operaciones fallan por excepciones de contrato.
- Implementación actual:
  - `unavailableScannerClient` devuelve `[]` en `listDevices` y lanza errores en `scan`/`generatePdf`.
  - `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/digitalizacionWorkspace.helpers.ts:7`-`31`.
- Corrección sugerida: mantener cliente inyectado desde provider y evitar fallback silencioso para flows productivos.

### B) Licencia faltante
- Error temprano visible en UI + potencial bloqueo de runtime al `initialize`:
  - App: `missingLicense` warning (`47`-`74`).
  - Cliente: `DYNAMSOFT_LICENSE_INVALID` al `initialize` (`395`-`400`).
- Corrección sugerida:
  - asegurar que `licenciaDynamsoft` o `dynamsoft.licenseKey` y/o provider estén siempre definidos en entrypoints.

### C) Selección de dispositivo inválida
- `selectDevice` valida índices y `assertValidDeviceId`.
- Errores:
  - `INVALID_DEVICE_ID`, `SCANNER_NOT_FOUND`.
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:155`-`161`, `517`-`520`.
- Corrección sugerida:
  - bloquear doble click en UI cuando `disabled`.

### D) Riesgo de pérdida de página en blanco por heurística
- Hay logs diagnósticos de `blank` en múltiples etapas:
  - `BLANK_PAGE_DETECTED`, `BLANK_PAGE_REMOVED`, `BLANK_PAGE_FINAL_STATE`.
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1416`-`1800`.
- Corrección sugerida:
  - ajustar thresholds en constantes `BLANK_PAGE_*` si se observan falsos positivos.
  - validar `removeBlankPages` desactivada por defecto en flujos con documentos de baja tinta.

### E) Riesgo de estado stale/concurrente
- Cliente usa `activeOperation` + generation para proteger operaciones concurrentes.
- `assertNoActiveOperation`, `ensureNotStale` (`1910`-`1926`, `1919`-`1926`).
- Corrección sugerida:
  - mantener un único scan por vez desde UI (ya desactivado por `disabled={scanner.loading}` en acciones críticas).

## 10) Ruta resumida de dependencia (visión rápida)

`CapDocument`
  -> `useAppDigitalizadorScannerClient`
  -> `DigitalizacionDocumentalWorkspace`
  -> `useDigitalizacionScanner`
  -> `scannerClient` (`DynamsoftTwainClient`)
  -> APIs Dynamsoft: initialize/listDevices/selectDevice/scan/...

## 11) Archivos clave de referencia

- `src/modules/radicacion/components/CapDocument.tsx`
- `src/app/Components/UI/AppDigitalizador/AppDigitalizador.tsx`
- `src/app/Components/UI/AppDigitalizador/AppDigitalizadorProvider.tsx`
- `src/app/Components/UI/AppDigitalizador/AppDigitalizador.context.ts`
- `src/app/Components/UI/AppDigitalizador/hooks/useAppDigitalizadorScannerClient.ts`
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx`
- `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts`
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/digitalizacionWorkspace.helpers.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
- `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts`
