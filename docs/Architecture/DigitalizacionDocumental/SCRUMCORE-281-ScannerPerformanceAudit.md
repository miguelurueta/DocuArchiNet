# Auditoría de rendimiento de escaneo (FE-05)

**Objetivo:** identificar la causa raíz del tiempo excesivo en captura y de la regresión percibida luego de cambios de blank detection, previousPagesForOperation y optimizaciones de rendimiento.

## 1) Evidencia de instrumentación activa

- `DynamsoftTwainClient.ts`
  - `initScanPipelinePerfRecord()` crea `scanStartedAt` y escribe estado global:
    - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:372`
  - `scan()` mide:
    - `AcquireImage` → `src/modules/.../DynamsoftTwainClient.ts:627`
    - `buildPagesFromBuffer` → `src/modules/.../DynamsoftTwainClient.ts:685`
    - `Blank Detection` → `src/modules/.../DynamsoftTwainClient.ts:721`
    - retorna `this.pages` en `:796`.
  - `DigitalizacionDocumentalWorkspace.tsx` registra el primer render y ranking:
    - `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:432`
  - `useDigitalizacionScanner.ts` llama a `client.scan(...)` y actualiza estado al resolver/fallar:
    - `src/modules/digitalizacion/hooks/useDigitalizacionScanner.ts:232`

## 2) Ranking de tiempo (valores reportados)

Tiempos observados (ms):

| Etapa | Duración |
|---|---:|
| AcquireImage | 33.3 s |
| Blank Detection | 15.9 s |
| buildPagesFromBuffer | 324 ms |
| Deskew | 1 ms |
| AutoCrop | 0 ms |
| AutoRotate | 0 ms |
| ReactFirstRender | Insignificante |

Total de etapas medidas (sin incluir `ReactFirstRender`): `49.524 s`.

Porcentaje sobre total medible:

| Etapa | % |
|---|---:|
| AcquireImage | 67.2 % |
| Blank Detection | 32.1 % |
| buildPagesFromBuffer | 0.7 % |
| Deskew | ~0 % |
| AutoCrop | ~0 % |
| AutoRotate | ~0 % |

## 3) Audit de AcquireImage

### 3.1 Capacidades negociadas en código antes de `AcquireImage()`

- `AcquireImage` se invoca con:
  - `PixelType`, `Resolution`, `IfFeederEnabled`, `IfDuplexEnabled`, `IfShowUI`, `IfDisableSourceAfterAcquire`, `IfAutoDiscardBlankpages`.
  - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:637`
- El tipo explícito del cliente también sólo incluye esas llaves:
  - `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.types.ts:189`

### 3.2 Capacidades no seteadas explícitamente en cliente

No existe ajuste de:
- `TransferMode`
- `Compression`
- `BufferType`
- `AutoFeed`
- `AutoScan`

No aparecen en `DynamsoftAcquireOptions` ni en el objeto de opciones construido.

### 3.3 ¿Puede separarse tiempo físico vs SDK en este flujo?

- `AcquireImage` cubre una promesa desde `dwt.OpenSource()` + `AcquireImage(...)` hasta callback de éxito (`:653`–`:672`).
- Esa sección incluye tiempo de UI de PaperStream, driver, movimiento físico de escaneo y retorno del buffer.
- En esta capa sólo se puede atribuir como `SDK/driver + scanner físico`; no hay medición interna por debajo de `dwt.AcquireImage` para separarlos.

### 3.4 ¿Hay buffering interno configurable aquí?

- No hay configuración explícita de buffering/caché al objeto de adquisición en este método.
- No hay propiedades de buffer adicionales en `acquireOptions`.

### 3.5 ¿Existe modo de transferencia más eficiente en este escenario desde este cliente?

- En el código actual, no hay control fino de transferencia más allá de las propiedades explícitas listadas.
- Para probar mejoras reales de `TransferMode`/`Compression`/`BufferType` se requiere revisar/añadir parámetros a `DynamsoftAcquireOptions` y validar compatibilidad con driver/modelo de scanner.

## 4) Auditoría de Blank Detection

### 4.1 Flujo implementado

- En `scan()`, si `options.removeBlankPages` es `true`, se ejecuta:
  - `removeDetectedBlankPagesWithDynamsoft(...)` y fallback a `removeDetectedBlankPages(...)` cuando native no existe.
  - `src/modules/.../DynamsoftTwainClient.ts:721`
- `removeDetectedBlankPagesWithDynamsoft` usa:
  - `IsBlankImageExpress.call(dwt, index)` para candidatos.
  - `src/modules/.../DynamsoftTwainClient.ts:1615`
- Luego **re-analiza cada candidato con `analyzeBlankPageCandidate(...)`**:
  - `src/modules/.../DynamsoftTwainClient.ts:1639`

### 4.2 Origen de imagen usado por análisis

- `buildPageFromBuffer()` guarda `imageUrl`/`thumbnailUrl` desde `dwt.GetImageURL(...)`.
  - `src/modules/.../DynamsoftTwainClient.ts:1221`
- `analyzeBlankPageCandidate()` intenta cargar:
  - primero `page.imageUrl`
  - luego `page.thumbnailUrl`
  - `src/modules/.../DynamsoftTwainClient.ts:1752`
- No hay conversión a `Blob`/`DataURL` propia ni control de CORS explícito.

### 4.3 ¿Por qué “canvas has been tainted”

- `loadAnalysisImage()` usa `new Image()` y asigna `src` sin `crossOrigin`.
  - `src/modules/.../DynamsoftTwainClient.ts:2062`
- En análisis se hace `context.drawImage(...)` seguido de `getImageData(...)`.
  - `src/modules/.../DynamsoftTwainClient.ts:1820`
  - `src/modules/.../DynamsoftTwainClient.ts:1821`
- Si el `imageUrl` remoto/externo no permite CORS, `getImageData()` provoca taint y `canvas` se vuelve no legible.
- El error queda atrapado y registrado como `BLANK_PAGE_ANALYSIS_ERROR`, no rompe inmediatamente el scan:
  - `src/modules/.../DynamsoftTwainClient.ts:1996`

### 4.4 ¿La API nativa evita Canvas?

- Sí existe uso nativo de `IsBlankImageExpress` (`removeDetectedBlankPagesWithDynamsoft`) y control de parámetros de runtime (`IfAutoDiscardBlankpages`, `BlankImageThreshold`, `BlankImageMaxStdDev`).
- Sin embargo, al estar en pipeline actual, aún entra a `analyzeBlankPageCandidate()` para páginas candidatas, por lo que Canvas sigue siendo camino activo.

### 4.5 ¿Se puede evitar Canvas completamente?

Con configuración nativa completa sí sería posible en escenarios con soporte real del driver, pero en estado actual no está habilitado de forma exclusiva porque la verificación adicional reintroduce Canvas.

### 4.6 Cantidad de veces que se analiza una página

- En el camino nativo:
  - cada candidato (`blankIndexes`) pasa por `analyzeBlankPageCandidate`.
- Sin API nativa:
  - se analiza **cada página** en `removeDetectedBlankPages`.
- Cada página candidata también se procesa para construir datos (`buildPagesFromBuffer`) y luego para detección; hay doble trabajo de pipeline (`image extraction + pixel inspect`) por misma página candidata.

## 5) Hotspots con responsable técnico

| Etapa | Archivo/Método | Archivo:línea | Causa técnica |
|---|---|---|---|
| AcquireImage | `DynamsoftTwainClient.scan` | `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:653` | Escaneo físico + callback de retorno del driver en `600 dpi` con UI de PaperStream. |
| buildPagesFromBuffer | `DynamsoftTwainClient.buildPagesFromBuffer/buildPageFromBuffer` | `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1181` | Construcción de páginas y `GetImageURL`; costo menor comparado. |
| Blank Detection | `DynamsoftTwainClient.scan` → `removeDetectedBlankPagesWithDynamsoft`/`analyzeBlankPageCandidate` | `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:731`, `1585`, `1639`, `1735` | Procesamiento adicional por candidato con `canvas` y `getImageData`; errores de CORS provocan manejo degradado por catch. |
| Deskew | `applyAutomaticProcessingFeature` → `Deskew` | `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1259` | No costo en escenario actual (`~0ms`). |
| AutoCrop | `applyAutomaticProcessingFeature` | `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1259` | No costó en escenarios reportados (`~0ms`). |
| AutoRotate | `applyAutomaticProcessingFeature` | `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:1259` | No costó en escenarios reportados (`~0ms`). |
| ReactFirstRender | `DigitalizacionDocumentalWorkspace` | `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx:432` | Medido como insignificante por evidencia del cliente. |

## 6) Comparación de cambios que pueden haber roto flujo

- Cambio más relevante asociado a la regresión de rendimiento de post-proceso:
  - `SCRUMCORE-279` (`5152957`) introdujo y estructuró:
    - `IfAutoDiscardBlankpages` y ajuste de parámetros de blank detection (`configureDynamsoftBlankPageDetection`, `applyDynamsoftBlankPageDetection`)
    - `removeDetectedBlankPagesWithDynamsoft`
    - llamadas a `AnalyzeBlankPageCandidate` por candidatos.
  - Afecta directamente el stage `Blank Detection` con costo de CPU.
- `51e7f9c` (`Improve digitalization scan rendering performance`) cambió `buildPagesFromBuffer` para reutilización en APPEND y agregó trazabilidad de `ReactFirstRender`/ranking.
  - Esto no explica por sí solo el cuello de botella de `15.9 s` en blank detection.
- `previousPagesForOperation` afecta estado/selección de páginas reconstruidas post-scan (`DynamsoftTwainClient.ts:762`), pero no añade el mayor costo de cómputo observado.

## 7) Causa raíz (base en evidencia)

La regresión funcional/performance observada se explica por un **cambio de costo en etapa de post-proceso**:
- `AcquireImage` consume ~`33.3s` (acumulación escáner físico + transferencia/runtime nativo).
- `Blank Detection` consume ~`15.9s` y pasa por pipeline de Canvas para candidatos pese a existir API nativa, con errores de CORS recurrentes por `getImageData`.
- Esto mantiene el flujo correcto pero aumenta drásticamente el tiempo total percibido en la sesión de escaneo con opciones altas (`600 dpi`, `duplex/ADF`, `removeBlankPages`, `deskew/autoCrop/autoRotate`).

Variable responsable principal:
- `page.imageUrl`/`page.thumbnailUrl` + `analyzeBlankPageCandidate` + `getImageData()` en la cadena de `removeDetectedBlankPagesWithDynamsoft`/fallback cuando activado `removeBlankPages`.

Archivo/método crítico:
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
  - `scan` (`721`, `740`)
  - `removeDetectedBlankPagesWithDynamsoft` (`1585`)
  - `analyzeBlankPageCandidate` (`1735`–`1810` y `1996`)

## 8) Propuestas (sin implementar)

1. **Eliminación de la validación canvas cuando `IsBlankImageExpress` retorna candidato positivo**
   - Riesgo: posible cambio de precisión de descarte en casos límite.
   - Complejidad: baja.
   - Beneficio esperado: reducción de `Blank Detection` (15.9s).
   - Mejora estimada: 20–45% en etapa.

2. **Forzar ruta nativa exclusivamente con `removeBlankPages` cuando `removeDetectedBlankPagesWithDynamsoft` retorna con confianza**
   - Riesgo: diferencias de comportamiento según driver (dispositivos con soporte parcial).
   - Complejidad: media.
   - Beneficio esperado: menor trabajo Canvas.
   - Mejora estimada: 15–35% en escenarios con soporte nativo.

3. **Reclasificar análisis por origen de imagen (usar únicamente thumbnail con control de caché)**
   - Riesgo: mayor falso negativo/positivo en páginas con texto muy tenue.
   - Complejidad: media.
   - Beneficio esperado: menos costo de decodificación y menos taint.
   - Mejora estimada: 5–20%.

4. **Auditar soporte `transfermode/compression` en `DynamsoftAcquireOptions` del runtime real del scanner**
   - Riesgo: incompatibilidad con ciertos scanners; posibles cambios de output.
   - Complejidad: alta (requiere pruebas por modelo y contrato).
   - Beneficio esperado: reducción de tiempo físico de `AcquireImage`.
   - Mejora estimada: variable; potencial 10–40% de la etapa `AcquireImage`.

