# Auditoría de Capacidades Nativas de Dynamsoft Web TWAIN (FE-07)

**Ticket:** SCRUMCORE-283  
**Archivo:** `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-283-DynamsoftNativeCapabilitiesAudit.md`  
**Fecha:** 2026-07-01  
**Alcance:** Solo evidencia y propuesta, sin cambios de código.

## 1) Objetivo y método

- Objetivo:
  - Verificar si el flujo actual de Digitalización Documental utiliza de forma correcta las capacidades nativas de Dynamsoft para detección de páginas en blanco, acceso a buffer y exportación.
  - Detectar duplicación de trabajo entre SDK nativo y lógica propia.
  - Identificar posibles APIs nativas desaprovechadas con impacto de rendimiento.
- Método:
  - Instrumentación y medición existente: `SCRUMCORE-281` (tiempos reportados).
  - Revisión de código real en:
    - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
    - `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-282-BlankDetectionArchitectureAudit.md`
  - Revisión documental oficial de Dynamsoft.

---

## 2) Hallazgos en el flujo real (código actual)

### 2.1 Flujo de adquisición y post-proceso vigente

Ruta ejecutada en `scan()`:

1. `initialize()` (carga runtime/licencia, `runtime.Load()`, `waitForWebTwain`)  
   - `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts:421`
2. `scan()` valida opciones y estado  
   - `:608`
3. Construye `acquireOptions` y llama:
   - `dwt.OpenSource()`
   - `dwt.AcquireImage(acquireOptions, success, failure)`
   - `:639` a `:671`
4. `buildPagesFromBuffer(dwt, ...)`
   - `:697`
5. `removeDetectedBlankPagesWithDynamsoft(dwt, {startIndex, endIndex})`
   - `:731`
6. `removeDetectedBlankPages(...)` (fallback si no hay `IsBlankImageExpress`)
   - `:738`
7. `applyAutomaticProcessing(...)` (deskew, autocrop, autorotate)
   - `:760`
8. `resolveCaptureOperationPages(...)` y retorno de páginas
   - `:773`
9. `restoreDynamsoftBlankPageDetection()` en `finally`
   - `:798`, `:863`

### 2.2 Mermaid: arquitectura actual de flujo de blanco

```mermaid
flowchart TD
  A[scan(options)] --> B[configureDynamsoftBlankPageDetection]
  B --> C[OpenSource + applyDynamsoftBlankPageDetection]
  C --> D[AcquireImage]
  D --> E[buildPagesFromBuffer]
  E --> F{removeBlankPages}
  F -->|true| G[removeDetectedBlankPagesWithDynamsoft]
  G --> H[IsBlankImageExpress por página candidata]
  H --> I[analyzeBlankPageCandidate (fallback/confirmación)]
  I --> J[new Image(src)]
  J --> K[canvas 384x512]
  K --> L[getImageData()]
  L --> M[heurística propia de blank]
  M --> N[RemoveImage + rebuildPagesAfterBufferRemoval]
  F -->|false| O[sin detección de blanco]
  G --> P[applyAutomaticProcessing]
  P --> Q[resolveCaptureOperationPages]
  Q --> R[return pages]
  R --> S[restoreDynamsoftBlankPageDetection]
```

---

## 3) Auditoría del blank detection nativo (API del SDK)

### 3.1 APIs nativas disponibles y su uso en el cliente

- `IfAutoDiscardBlankpages`
  - Se usa en `acquireOptions` de `scan()` (valor de opción de UI: `options.removeBlankPages`)  
  - `DynamsoftTwainClient.ts:651`
  - Se vuelve a forzar en runtime antes de adquirir con `applyDynamsoftBlankPageDetection`  
  - `:846-848`
  - Se restaura al final con `restoreDynamsoftBlankPageDetection`  
  - `:859-878` y `:861-867`

- `BlankImageThreshold`
  - Se guarda (`configure...`) si existe: `:812-819`
  - Se setea en adquisición a `220` (`BLANK_PAGE_DYNAMSOFT_BLANK_IMAGE_THRESHOLD`)  
  - `:850-852`
  - Se restaura al final: `:885-895`

- `BlankImageMaxStdDev`
  - Se guarda (`configure...`): `:816-819`
  - Se setea en adquisición a `28` (`BLANK_PAGE_DYNAMSOFT_BLANK_IMAGE_MAX_STDDEV`)  
  - `:854-856`
  - Se restaura al final: `:902-913`

- `IsBlankImageExpress`
  - Se usa como etapa nativa candidata:
  - `removeDetectedBlankPagesWithDynamsoft`: `const isBlank = isBlankImageExpress.call(dwt, index)`
  - `:1590`, `:1616-1619`

- `IsBlankImage`
  - No se usa en este flujo.
  - No hay referencia en `DynamsoftTwainClient.ts`.

- `IsBlankImageAsync`
  - No se usa en este flujo.
  - No hay referencia en `DynamsoftTwainClient.ts`.

### 3.2 Diferencias entre APIs según documentación oficial

Documentación oficial:
- `IsBlankImage()` y `IsBlankImageExpress()` están relacionados con `BlankImageThreshold` y `BlankImageMaxStdDev`; `IsBlankImage()` es más precisa y más lenta; `IsBlankImageExpress()` es más rápida.  
  - Fuente: https://www.dynamsoft.com/web-twain/docs/info/api/WebTwain_Buffer.html líneas sobre `IsBlankImage()` e `IsBlankImageExpress()` (`L955`-`L1033` de la referencia).
- `IsBlankImageAsync()` usa algoritmo distinto y es **recomendada**.
  - Fuente: https://www.dynamsoft.com/web-twain/docs/info/api/WebTwain_Buffer.html (`L981-L987`, `L987`).
- `BlankImageThreshold` rango permitido `[0, 255]`, `default=128`.  
  - Fuente: https://www.dynamsoft.com/web-twain/docs/info/api/WebTwain_Acquire.html (líneas reportadas en resultados de referencia).
- `BlankImageMaxStdDev` y `BlankImageThreshold` no aplican a `IsBlankImageAsync()`.  
  - Fuente: https://www.dynamsoft.com/web-twain/docs/info/api/WebTwain_Buffer.html (`L901-L917`, `L920-L924`, `L983-L987`).

### 3.3 Diferencias y precisión (directa)

| API | Naturaleza | Precisión | Costo/latencia esperada | Relación con parámetros de configuración |
|---|---|---:|---:|---|
| `IsBlankImage()` | Sincrónica | Alta | Mayor que `Express` | Usa `BlankImageThreshold`, `BlankImageMaxStdDev` |
| `IsBlankImageExpress()` | Sincrónica | Menor | Menor que `IsBlankImage` | Usa `BlankImageThreshold`, `BlankImageMaxStdDev` |
| `IsBlankImageAsync()` | Async (Promise) | Equilibrio y recomendado por SDK | Depende de configuración interna de bloque | No usa `BlankImageThreshold` ni `BlankImageMaxStdDev` |
| `IfAutoDiscardBlankpages` | Capability/setting de source | Dependiente de soporte del driver | Muy bajo si el driver lo soporta | Opción de adquisición |

### 3.4 ¿La implementación actual recomienda usar nativamente?

- Sí se están usando APIs nativas:
  - `IfAutoDiscardBlankpages`, `BlankImageThreshold`, `BlankImageMaxStdDev`, `IsBlankImageExpress`.
- Pero **no se está usando la API recomendada para precisión y control moderno** (`IsBlankImageAsync()`).
- La salida de `IsBlankImageExpress()` vuelve a pasar por `analyzeBlankPageCandidate` (Canvas) para confirmar/reanalizar de nuevo, duplicando ruta de procesamiento.

### 3.5 ¿Se duplican capacidades?

Sí.

1) SDK nativo (candidato): `removeDetectedBlankPagesWithDynamsoft` usa `IsBlankImageExpress` por candidato.
2) Lógica propia: para candidatos confirmados ejecuta `analyzeBlankPageCandidate` con `new Image` + `canvas` + `getImageData()`.
- Archivo: `DynamsoftTwainClient.ts:1585` y `:1735-1820`.

Esto crea procesamiento duplicado (detección nativa + clasificación manual) sobre un subconjunto de páginas.

---

## 4) Auditoría de buffer e imagen en Dynamsoft

### 4.1 Formas de acceso a imágenes en la implementación

- `GetImageURL` (preview y `buildPagesFromBuffer`)
  - Usa `dwt.GetImageURL(index, 160, 220)`, `dwt.GetImageURL(index, -1, -1)`, fallback `dwt.GetImageURL(index)`, normalizados por `normalizeImageUrl`.  
  - `DynamsoftTwainClient.ts:1216-1224`
- `normalizeImageUrl` simplemente valida string no vacío.  
  - `:261-262`
- No se usa `GetImagePartURL` en este módulo.
  - Sin aparición en el archivo.

- Exportación/serialización:
  - `ConvertToBlob()` para PDF (`generatePdf`)  
  - `:1110-1115`

- No se usa `ConvertToBase64Binary` en este módulo para el flujo principal de páginas.
- No se usa `SaveToBase64Binary` o `LoadImageFromBinary` en este flujo.

### 4.2 Recomendación por caso de uso (según capacidades del SDK)

- **Preview:** usar `GetImageURL` / viewer del SDK + buffer/IDs; ya está implementado.
- **Procesamiento interno:** si requiere inspección de pixels, mejor encauzar a APIs de blank detection del SDK en lugar de extracción de URL + HTML Canvas.
- **Exportación/PDF:** `ConvertToBlob` es la API nativa recomendada (y usada).
- **Análisis de documentos complejos:** `IsBlankImageAsync()` puede reemplazar pipeline manual pesado con parámetros de texto (`minBlockHeight`, `maxBlockHeight`) según necesidad de tolerancia.

### 4.3 ¿Existe acceso directo a píxeles sin Canvas?

En este flujo actual **no** existe acceso directo a píxeles vía API de Dynamsoft; se convierte cada imagen candidata a `new Image()` y se lee `canvas.getContext('2d').getImageData()`.
- `:2062-2069` (carga de imagen DOM)
- `:1801-1821` (canvas y getImageData)

---

## 5) Canvas / CORS / Origen de imágenes

### 5.1 ¿Por qué aparece `Canvas has been tainted by cross-origin data`?

1. `analyzeBlankPageCandidate` usa `new Image()` sin establecer `crossOrigin`.  
   - `:2062-2069`
2. La imagen se carga desde URL de `GetImageURL` proveniente del servicio de Dynamsoft/URL de servicio local; en contexto web puede implicar origen distinto al documento.  
   - El acceso de píxeles via Canvas entre orígenes sin política CORS explícita produce taint.
3. Configuración oficial de CORS:
   - `IfCheckCORS` por defecto `false` y cuando es `true` expone errores CORS más detallados en runtime/docs.  
   - https://www.dynamsoft.com/web-twain/docs/info/api/Dynamsoft_WebTwainEnv.html (`IfCheckCORS`, líneas alrededor de `678-680` en referencia abierta).
   - Para servicio local, documentación de servicio exige configurar `Access-Control-Allow-Origin` en `DSConfiguration.ini` y `IfCheckCORS=true` cuando aplique.  
   - https://www.dynamsoft.com/web-twain/docs/extended-usage/dynamsoft-service-configuration.html (líneas `242-244` en referencia).

Conclusión:
- El error es consistente con el punto donde el pipeline intenta leer píxeles desde una imagen proveniente de fuente no configurada para CORS.

### 5.2 ¿Puede eliminarse Canvas en esta capa?

Sí, en la parte de blank detection:
- La parte actual de análisis (`analyzeBlankPageCandidate`) no depende de API nativa de procesamiento de píxeles y puede reemplazarse por detección nativa (`IsBlankImageAsync`/`IsBlankImage`) sobre índice directo.
- El resto del flujo (`preview`, `ConvertToBlob`) ya vive en APIs de SDK.

---

## 6) Auditoría de capacidades no utilizadas (matriz)

| API SDK | Actualmente usada | Beneficio | Impacto esperado |
|---|---|---|---|
| `IsBlankImage()` | No | Mayor precisión sin aproximaciones heurísticas rápidas | Mejora precisión de detección si reemplaza pipeline manual; costo medio |
| `IsBlankImageExpress()` | Sí | Detección rápida en candidatos | Se usa como filtro inicial; costo moderado por página candidata |
| `IsBlankImageAsync()` | No | API recomendada (algoritmo propio recomendado por SDK) | Reduce riesgo de falsos positivos/negativos; evita lógica manual pesada |
| `IfAutoDiscardBlankpages` | Sí | Descarta en adquisición si el scanner lo soporta | Bajo costo y potencial gran mejora de velocidad |
| `BlankImageThreshold` | Sí | Ajusta sensibilidad de blank nativo | Riesgo de false positives/negativos si no calibrado |
| `BlankImageMaxStdDev` | Sí | Ajusta sensibilidad de ruido/degradación | Riesgo de sobre descarte si muy agresivo |
| `GetImageURL` | Sí | Construcción de previews y análisis propio | Adecuada para preview; no ideal para análisis intensivo |
| `GetImagePartURL` | No | Potencial para previews parciales | No usado |
| `ConvertToBlob` | Sí | Exportación PDF estable | Adecuado para output |
| `ConvertToBase64Binary` | No | Alternativa de exportación en string base64 | Sin beneficio en este flujo |
| `LoadImageFromBinary/Base64` | No | Reingesta de imagen procesada | Sin uso aquí |
| `IfCheckCORS` | No | Estabiliza errores CORS y diagnósticos | Potencial mejora de fiabilidad del análisis por URL |
| `IfAllowLocalCache` / `BufferMemoryLimit` | No | Escalabilidad en grandes lotes | Potencial mejora de rendimiento de buffer para volúmenes altos |

---

## 7) Pipeline recomendado por Dynamsoft (comparativa)

### 7.1 Arquitectura actual (resumen)

- Adquisición + configuración de flags `AcquireImage`.
- Build pages (`GetImageURL`).
- `IsBlankImageExpress` + validación manual `canvas` para candidatos.
- `RemoveImage` manual + rebuild.
- Auto procesamiento y retorno.

### 7.2 Arquitectura recomendada (alineada con capacidades nativas)

- Mantener `IfAutoDiscardBlankpages` + parámetros en adquisición cuando aplica.
- En páginas candidatas, usar API nativa recomendada (`IsBlankImageAsync`) o estrategia híbrida directa, evitando `new Image` + `canvas` salvo cuando sea estrictamente necesario.
- Ejecutar trabajo por página en flujo de eventos nativos (`OnPostTransferAsync`) para evitar bloquear retornos de adquisición (`on success callback`) en escenarios de alto volumen.
  - Documentación de eventos de adquisición indica que `OnPostTransferAsync` es la contraparte asíncrona de `OnPostTransfer`.

### 7.3 Diferencias técnicas clave

| Dimension | Actual | Recomendado |
|---|---|---|
| Detección blank | Nativa (`Express`) + heurística manual | Nativa consistente (`IsBlankImageAsync`) + fallback controlado |
| Riesgo CORS | Presente por `new Image()` + `getImageData` | Reduce riesgo al eliminar lectura de píxeles en Canvas |
| Rendimiento | Coste elevado en paso manual | Menor costo CPU en post-proceso, pipeline más lineal |
| Precisión | Mezcla de criterios de dos métodos | Un criterio único trazable y documentado |
| Mantenibilidad | Lógica propia compleja | Menos código de bajo nivel (canvas/píxeles) |

---

## 8) Rendimiento y carga (ranking de etapas)

Valores medidos en auditoría anterior:

| Etapa | Tiempo | Observación |
|---|---:|---|
| `AcquireImage` | ~33.3 s | Dominante del pipeline |
| `Blank Detection` | ~15.9 s | Segundo mayor costo |
| `buildPagesFromBuffer` | ~324 ms | Bajo |
| `Deskew` | ~1 ms | Bajo |
| `AutoCrop` | ~0 ms | Bajo |
| `AutoRotate` | ~0 ms | Bajo |
| `ReactFirstRender` | insignificante | Bajo |

Correlación:
- Aun cuando `Blank Detection` ejecuta una ruta nativa (`IsBlankImageExpress`) y luego manual, el costo de ~15.9 s coincide con la fase de análisis Canvas/`getImageData` y conversiones de imágenes candidatas.

---

## 9) Comparativa histórica (SCRUMCORE-279)

### 9.1 Antes de `5152957` (pre-SCRUMCORE-279)

- No existía integración con:
  - configuración/restaura de `BlankImageThreshold` y `BlankImageMaxStdDev`,
  - `IfAutoDiscardBlankpages` en pipeline de adquisición,
  - `removeDetectedBlankPagesWithDynamsoft(...)`.
- El flujo usaba `removeDetectedBlankPages(...)` (heurística manual sobre candidatos/ítems).

### 9.2 Cambios introducidos en `5152957`

Commit `5152957` añadió (aprox. +356/369 líneas):
- Configuración runtime de blank detection en `configure/apply/restoreDynamsoftBlankPageDetection`.
- Inserción de bandera `IfAutoDiscardBlankpages` en `acquireOptions` + try/finally de restauración.
- Nueva ruta `removeDetectedBlankPagesWithDynamsoft` y fallback al método manual anterior.
- Expansión heurística manual de `analyzeBlankPageCandidate`.

### 9.3 Impacto comparado

- Beneficio: intenta usar capacidades nativas sin perder fallback manual.
- Riesgo/costo agregado: duplicación de evaluación (nativa + heurística manual) sobre candidatos.
- Riesgo funcional: discrepancias entre criterio nativo y heurístico propio (falsos positivos/negativos).

---

## 10) Propuestas (sin implementación)

### 10.1 Clasificación de propuestas

#### Baja complejidad
- **Habilitar `crossOrigin` explícito en la carga de imágenes DOM**  
  - Beneficio: reduce riesgo de error taint en análisis manual.  
  - Riesgo: dependencia de política CORS del origen y de `GetImageURL`.  
  - Mejora estimada: 5-15% en estabilidad de análisis manual.
- **Documentar y parametrizar el tipo de blank detection activo**  
  - Beneficio: trazabilidad operativa y diagnóstico.
  - Riesgo: nulo.
  - Mejora estimada: 0% tiempo, +10-20% operabilidad.

#### Media complejidad
- **Migrar confirmación de blanco a `IsBlankImageAsync()` con parámetros de bloque**  
  - Beneficio: elimina reanálisis manual por `canvas/getImageData`, alinea con API recomendada.
  - Riesgo: ajuste de umbrales (`minBlockHeight`/`maxBlockHeight`) y validación de casos de texto tenue.
  - Mejora estimada: 20-45% en `Blank Detection` para lotes con muchas candidatas.
  - Mantenimiento: menor lógica custom.
  - Compatibilidad: alta si SDK `v18.4+` (docs indican disponibilidad).

#### Alta complejidad
- **Refactor del post-proceso para pipeline de evento asíncrono por transferencia (`OnPostTransferAsync`)**  
  - Beneficio: disminuye riesgo de bloqueo en callback de adquisición y ordena el flujo por página.
  - Riesgo: migración de estado, sincronización con UI y capture operations.
  - Mejora estimada: mejora de UX y estabilidad en alto volumen; impacto variable en tiempo total.
  - Compatibilidad: requiere compatibilidad de eventos asíncronos en versión objetivo.

---

## 11) Riesgos y recomendaciones finales

### Riesgos actuales detectados
- Duplicación de lógica (nativa + manual) en detección.
- Posible contaminación de estado cuando la lógica manual cae por `analysis-failed`.
- Dependencia de URLs del servicio y carga de imágenes DOM para análisis de pixeles (CORS).

### Recomendación principal (no ejecutada)
- Sustituir la etapa manual de `analyzeBlankPageCandidate` por detección nativa de alto nivel (`IsBlankImageAsync`) o reducir al mínimo absoluto la verificación por Canvas.

---

## 12) Referencias

- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`  
  - https://github.com/DocuArchiCore.react/blob/main/src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts (línea base local)
- Dynamsoft Web TWAIN API Reference - Buffer:
  - https://www.dynamsoft.com/web-twain/docs/info/api/WebTwain_Buffer.html
- Dynamsoft Web TWAIN API Reference - Acquire:
  - https://www.dynamsoft.com/web-twain/docs/info/api/WebTwain_Acquire.html
- FAQ: remove blank page automatically:
  - https://www.dynamsoft.com/web-twain/docs/faq/remove-blank-page-automatically.html
- Dynamsoft Web TWAIN Env (`IfCheckCORS`):
  - https://www.dynamsoft.com/web-twain/docs/info/api/Dynamsoft_WebTwainEnv.html
- Dynamsoft Service config / CORS:
  - https://www.dynamsoft.com/web-twain/docs/extended-usage/dynamsoft-service-configuration.html
- FAQ rendimiento (`OnPostTransferAsync`):
  - https://www.dynamsoft.com/web-twain/docs/faq/document-scanning-slow-than-native.html
- Buffer caching docs:
  - https://www.dynamsoft.com/web-twain/docs/indepth/features/buffer.html

