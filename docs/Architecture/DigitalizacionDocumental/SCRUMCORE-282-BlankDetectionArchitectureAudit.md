# Auditoría de Arquitectura de Blank Detection (FE-06)

**Objetivo:** confirmar si la arquitectura actual de detección de páginas en blanco es correcta, si duplica capacidades de Dynamsoft y cuál es el origen técnico del costo de ~15.9 s observado en `Blank Detection`.

## Alcance y restricciones

- Sin cambios de comportamiento.
- Sin cambios de contratos.
- Sin refactors o optimizaciones.
- Solo evidencia de código y trazabilidad.

## 1) Flujo actual (código real)

### `scan()`

Ruta de ejecución actual:

- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:608`
  - `acquireOptions` armado con:
    - `IfShowUI`, `PixelType`, `Resolution`, `IfFeederEnabled`, `IfDuplexEnabled`, `IfDisableSourceAfterAcquire`, `IfAutoDiscardBlankpages`.
  - `dwt.OpenSource()`
  - `configureDynamsoftBlankPageDetection()` + `applyDynamsoftBlankPageDetection()`
  - `dwt.AcquireImage(...)`
- `buildPagesFromBuffer`:
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:697`
- `removeDetectedBlankPagesWithDynamsoft(...)`:
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:731`
- `applyAutomaticProcessing(...)`
- `resolveCaptureOperationPages(...)`
- `return [...this.pages]`
- `finally`:
  - `restoreDynamsoftBlankPageDetection()`

### Diagrama Mermaid (flujo técnico)

```mermaid
flowchart TD
  A[scan(options)] --> B[configureDynamsoftBlankPageDetection]
  B --> C[applyDynamsoftBlankPageDetection]
  C --> D[AcquireImage]
  D --> E[buildPagesFromBuffer]
  E --> F{removeBlankPages?}
  F -->|true| G[removeDetectedBlankPagesWithDynamsoft]
  G --> H[candidateIndexes]
  H --> I[IsBlankImageExpress per index]
  I --> J{¿candidato en blanco?}
  J -->|sí| K[load page + analyzeBlankPageCandidate]
  K --> L[new Image(src)]
  L --> M[createElement('canvas')]
  M --> N[drawImage + getImageData]
  N --> O[decisión heurística isBlank]
  O --> P[RemoveImage + rebuildPagesAfterBufferRemoval]
  K -->|no| Q[descarta candidato]
  J -->|no| Q
  G --> R[logBlankPageReinsertions]
  F -->|false| S[sin detección]
  G --> T[applyAutomaticProcessing]
  S --> T
  T --> U[resolveCaptureOperationPages]
  U --> V[return pages]
  V --> W[restoreDynamsoftBlankPageDetection]
```

## 2) Auditoría por método requerido

### A. `removeDetectedBlankPagesWithDynamsoft()`

- Método: `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1585`
- Comportamiento:
  - Construye índices `candidateIndexes` desde `startIndex` a `endIndex`.
  - Llama `IsBlankImageExpress.call(dwt, index)` para cada índice candidato.
  - Filtra `blankIndexes` según retorno nativo.
  - Re-analiza **solo candidatos** con `analyzeBlankPageCandidate(page)` usando `Promise.all`.
  - Remove/rebuild sobre páginas confirmadas.
- Salida de reinserciones: `logBlankPageReinsertions(...)` en `2015`.

### B. `analyzeBlankPageCandidate()`

- Método: `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1735`
- Comportamiento:
  - `imageUrl` = `page.imageUrl ?? page.thumbnailUrl`
  - `loadCandidates`: intenta `page.imageUrl` y `page.thumbnailUrl`.
  - `loadAnalysisImage(src)` con `new Image()` y `image.src = src`.
  - `canvas` desde `this.options.documentRef.createElement("canvas")`.
  - `context.drawImage(image, 0, 0, canvas.width, canvas.height)`.
  - `const pixels = context.getImageData(...).data`.
  - cálculo intensivo de contenido/contraste/transiciones y heurísticas.
  - retorna `isBlank`.

### C. `logBlankPageReinsertions()`

- Método: `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:2015`
- Operación:
  - Recibe `BlankPageRemovalResult`.
  - Si no hay resultado o detecciones vacías: `return`.
  - Itera `detected` y emite logs de reinserción si el `pageId` existe en estado actual.

### D. `buildPagesFromBuffer()`

- Método: `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1181`
- Origen de imagen:
 - `thumbnailUrl = normalizeImageUrl(dwt.GetImageURL?.(index, 160, 220))`
 - `imageUrl = normalizeImageUrl(dwt.GetImageURL?.(index, -1, -1))`
 - `normalizeImageUrl` solo valida string no vacía (`:261`).
- No crea `Blob` ni `Object URL` en este método.

## 3) Conteo de operaciones por página (reglas exactas del código)

Sea `P = páginas escaneadas` en el lote actual y `C = cantidad de candidatos detectados por `IsBlankImageExpress`.

1. ¿Cuántas veces se analiza cada página?
   - `buildPagesFromBuffer`: 1 vez por página para materializar metadatos.
   - `removeDetectedBlankPagesWithDynamsoft`: 1 llamada nativa `IsBlankImageExpress` por candidato en rango.
   - `analyzeBlankPageCandidate`: 1 llamada por cada candidato confirmado (`C`), no por cada página.

2. ¿Cuántas veces se crea un `canvas` por página?
   - `C` veces (solo por página candidata analizada).

3. ¿Cuántas veces se llama a `getImageData()`?
   - `C` veces (paralelo a `canvas` de candidatos).

4. ¿Cuántas imágenes se cargan mediante `new Image()`?
   - `C` veces (en `loadAnalysisImage` por candidato).

5. ¿Cuántos Blob URL / Object URL se crean?
   - 0 en la ruta de blank detection (ni en `analyzeBlankPageCandidate`, ni `loadAnalysisImage`, ni `buildPagesFromBuffer`).

6. ¿Cuántas conversiones Image → Canvas existen?
   - `C` conversiones (`drawImage`) para candidatos.

7. ¿Hay trabajo redundante?
   - Sí. `IfAutoDiscardBlankpages/IsBlankImageExpress` ya descarta candidato por SDK, pero luego esos candidatos pasan nuevamente por verificación manual de píxeles (Canvas), por lo tanto se repite costo CPU en CPU-bound path.

## 4) Auditoría Dynamsoft API nativa

### 4.1 API nativa existente

- El código usa explícitamente capacidades nativas:
  - `IfAutoDiscardBlankpages` en `acquireOptions` (`:651`, `:647` etc.)
  - `IsBlankImageExpress` en `removeDetectedBlankPagesWithDynamsoft` (`:1589`).
  - Configuración `BlankImageThreshold` y `BlankImageMaxStdDev` en `applyDynamsoftBlankPageDetection` (`:846`, `:850`, `:854`).

### 4.2 Qué hacen estas capacidades aquí

- `IfAutoDiscardBlankpages`:
  - habilitado por opción de escaneo cuando `removeBlankPages = true`.
  - asignado tanto en `acquireOptions` como en runtime del objeto DWT.
- `BlankImageThreshold`:
  - ajustado a `BLANK_PAGE_DYNAMSOFT_BLANK_IMAGE_THRESHOLD = 220` al escanear.
- `BlankImageMaxStdDev`:
  - ajustado a `BLANK_PAGE_DYNAMSOFT_BLANK_IMAGE_MAX_STDDEV = 28` al escanear.
- Estado anterior se guarda en `configureDynamsoftBlankPageDetection()` y se restaura en `finally`.

### 4.3 ¿Actualmente se está utilizando parcialmente?

- Sí. El pipeline usa nativamente el candidato (`IsBlankImageExpress`) pero **siempre** re-evalúa candidatos con Canvas en `analyzeBlankPageCandidate`.
- La ruta no está exclusivamente nativa.

### 4.4 ¿La implementación manual duplica capacidades?

- Sí: el candidato nativo se vuelve a procesar manualmente (píxel-a-píxel), que es exactamente la parte costosa.

### 4.5 ¿Puede evitarse el paso por Canvas?

- Sí, desde una arquitectura nativa de descarte completo: depender de `removeDetectedBlankPagesWithDynamsoft` + parámetros runtime para decisiones finales (salvo fallback cuando no exista `IsBlankImageExpress`).

## 5) Auditoría CORS / Canvas

### Origen de la imagen analizada

- `GetImageURL(...)` en `buildPageFromBuffer` (`:1221`-`:1223`) genera URLs de DWT.
- `analyzeBlankPageCandidate` usa esos URLs (`imageUrl`/`thumbnailUrl`), luego `new Image()` para cargar.

### Por qué aparece `Canvas has been tainted`

- `loadAnalysisImage()` no define `image.crossOrigin`.
- Cuando la URL no resuelve con CORS adecuado para lectura pixel, `drawImage` + `getImageData` puede quedar bloqueado por tainting.
- `analyzeBlankPageCandidate` captura excepción y devuelve `analysis-failed`, con logs `BLANK_PAGE_ANALYSIS_ERROR` (`:1996`).

### Comportamiento esperado

- Sí, es un comportamiento estándar de HTML Canvas: acceso de pixels restringido si el recurso no cumple CORS al entrar al pipeline de render/captura.
- Esto no implica fallo del scan completo, pero sí degrade calidad/tasa de confianza del análisis manual por páginas no procesables.

## 6) Auditoría de costes (con tiempos reportados en FE-05)

Tiempos medidos:

| Método/etapa | Duración | % sobre total medible |
|---|---:|---:|
| `AcquireImage` | 33.3 s | 67.2% |
| `Blank Detection` | 15.9 s | 32.1% |
| `buildPagesFromBuffer` | 324 ms | 0.7% |
| `Deskew` | 1 ms | 0.0% |
| `AutoCrop` | 0 ms | 0.0% |
| `AutoRotate` | 0 ms | 0.0% |
| `ReactFirstRender` | insignificante | insignificante |

Archivo responsable de ranking:
- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:432`

Conclusión de hotspot:
- `Blank Detection` está en 2° lugar y su costo se explica por la porción manual (Canvas/getImageData) sobre candidatos, no por el pipeline de escaneo físico.

## 7) Comparativa histórica (pre vs post SCRUMCORE-279, commit `5152957`)

- Implementación pre-`5152957` (anterior en `git show`):
  - `removeDetectedBlankPages()` analizaba candidatas/páginas con `analyzeBlankPageCandidate` en forma manual.
  - No había integración de `IfAutoDiscardBlankpages` ni config nativa de `BlankImage*`.
- Implementación introducida en `SCRUMCORE-279` (`5152957`):
  - Nuevas constantes de parámetros de blank detection nativos.
  - `configure/apply/restore` de configuración Dynamsoft runtime.
  - Inclusión de `IfAutoDiscardBlankpages` en opciones de acquire.
  - Nuevo flujo con `removeDetectedBlankPagesWithDynamsoft()` + fallback a manual.
- Beneficio aportado:
  - Primer filtro nativo de candidatos por SDK.
- Costo agregado:
  - Nueva etapa de configuración runtime + doble filtro (nativo + manual de candidatos).
- Riesgo introducido:
  - Dependencia de `IsBlankImageExpress` + inconsistencias cuando la salida manual y nativa no alinean en condiciones límite.

## 8) Causa raíz (de performance de Blank Detection)

- Variables/responsables críticos:
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1585`  
    `removeDetectedBlankPagesWithDynamsoft`
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1735`  
    `analyzeBlankPageCandidate`

Evidencia de causa:
- El flujo de descarte en blanco pasa por `Canvas` y `getImageData` para candidatos (`:1820`-`:1821`) tras pasar por filtro nativo (`IsBlankImageExpress` en `:1617`), causando costo de CPU adicional.
- No hay trazabilidad de soporte de `TransferMode/Compression/BufferType/AutoFeed/AutoScan` desde `scan()`; esas capacidades no están siendo configuradas ahí, por lo que el cuello aquí no viene de esa capa.

## 9) Recomendaciones técnicas futuras (sin implementación aún)

### Baja complejidad
- Eliminar la re-evaluación manual cuando `IsBlankImageExpress` ya marcó como candidato y no se requiere fallback por compatibilidad.
  - Beneficio: reducción directa de `Blank Detection`.
  - Riesgo: sensibilidad en casos de falsos positivos de API nativa.
  - Complejidad: baja.
  - Mejora estimada: 20% – 45%.

### Media complejidad
- Definir umbral operativo: si `analyzeBlankPageCandidate` falla por CORS o si el candidato está dentro de rango de confianza, confiar 100% en `IsBlankImageExpress` + métricas de telemetría.
  - Beneficio: elimina intentos de canvas en lotes problemáticos.
  - Riesgo: degradación de recall/precision según scanner.
  - Complejidad: media.
  - Mejora estimada: 15% – 35%.

### Alta complejidad
- Separar rutas de análisis:
  - 100% nativa donde el scanner la soporta.
  - Canvas únicamente como fallback controlado por feature-flag y sólo si hay evidencia de compatibilidad segura.
  - Beneficio: elimina trabajo duplicado sin perder cobertura.
  - Riesgo: mayor complejidad de producto/QA por matriz de hardware.
  - Complejidad: alta.
  - Mejora estimada: 30% – 60% del tiempo de `Blank Detection` en escenarios compatibles.

## Evidencia adicional de soporte oficial relevante

- `Dynamsoft` documenta `IsBlankImage`, `IsBlankImageExpress`, `BlankImageThreshold`, `BlankImageMaxStdDev` en buffer API.
- `Dynamsoft` documenta `GetImageURL`/`GetImagePartURL` para obtención de fuentes visuales.
- La capa de servicio recomienda revisar CORS de servicio cuando se consumen URLs y se realiza manipulación de imagen.
