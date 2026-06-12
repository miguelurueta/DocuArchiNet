# SCRUMCORE-249 - Auditoria tecnica de herramientas Dynamsoft

## Alcance

Este documento registra la auditoria tecnica solicitada para SCRUMCORE-249.

Restriccion del ticket: no implementar cambios funcionales, no modificar comportamiento y no cambiar configuracion de captura. El resultado es diagnostico tecnico sobre el estado real del modulo `src/modules/digitalizacion` y el componente corporativo `AppDigitalizador`.

## Entorno y componentes auditados

| Elemento | Evidencia | Estado |
| --- | --- | --- |
| SDK Dynamsoft | `DYNAMSOFT_SDK_VERSION = "19.3.2"` en `src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.constants.ts` | Configurado |
| Service esperado | `DYNAMSOFT_EXPECTED_SERVICE_VERSION = "1.9.3.1028"` | Alineado con el entorno validado previamente |
| TWAIN Module esperado | `DYNAMSOFT_EXPECTED_TWAIN_MODULE_VERSION = "19.3.2"` | Alineado con el entorno validado previamente |
| CSS DWT | `src/dynamsoft.webtwain.css`, `src/dynamsoft.webtwain.viewer.css` | Carga explicita |
| Contenedor DWT | `digitalizacion-dynamsoft-container` | Renderizado oculto por `DigitalizacionDocumentalWorkspace` |
| Cliente scanner | `DynamsoftTwainClient` | Adapter unico; React no usa `DWObject` directamente |
| Componente reutilizable | `AppDigitalizador` -> `DigitalizacionDocumentalWorkspace` | Disponible |

## Flujo actual auditado

```txt
AppDigitalizador
  -> useAppDigitalizadorScannerClient
  -> DynamsoftTwainClient
  -> DigitalizacionDocumentalWorkspace
  -> useDigitalizacionScanner
  -> initialize()
  -> listDevices()
  -> selectDevice()
  -> scan()
  -> AcquireImage()
  -> buildPageFromBuffer()
  -> thumbnails / preview
  -> generatePdf()
  -> ConvertToBlob(application/pdf)
```

### Flujo de inicializacion

1. `loadDynamsoftScripts()` carga JS y CSS desde CDN `dwt@19.3.2`.
2. `runtime.ProductKey` recibe la licencia inyectada.
3. `runtime.ResourcesPath` apunta al `dist` del SDK.
4. `runtime.Containers` registra `WebTwainId = "digitalizacion-documental-dwt"`.
5. `runtime.Load()` se ejecuta y luego `waitForWebTwain()` hace polling de `GetWebTwain(...)`.

Conclusion: la inicializacion moderna ya contiene las correcciones necesarias de SDK 19.3.2, CSS, ResourcesPath y espera controlada de instancia.

## Auditoria de seleccion de scanner

El flujo actual usa dos rutas:

| Ruta | Condicion | API usada | Estado |
| --- | --- | --- | --- |
| Source Manager legacy | `SourceCount > 0` | `OpenSourceManager()`, `GetSourceNameItems(index)`, `SelectSourceByIndex(index)` | Ruta preferida |
| DWT 19 modern | No hay fuentes legacy y `GetDevicesAsync` responde | `GetDevicesAsync(...)`, `SelectDeviceAsync(device)` | Fallback disponible |

Decision vigente: para scanners detectados por `SourceCount/GetSourceNameItems`, el adapter no envia objetos manuales a `SelectDeviceAsync`. Usa `SelectSourceByIndex(index)`, que es la ruta correcta para fuentes TWAIN legacy.

## Auditoria de configuracion AcquireImage

Configuracion actual enviada:

```ts
{
  IfShowUI: false,
  PixelType: colorModeToPixelType[options.colorMode ?? "color"],
  Resolution: options.resolutionDpi ?? DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
  IfFeederEnabled: true,
  IfDuplexEnabled: options.duplex ?? false,
  IfDisableSourceAfterAcquire: true,
}
```

Valores efectivos por defecto:

| Propiedad | Valor actual | Implicacion |
| --- | --- | --- |
| `IfShowUI` | `false` | No se muestra UI nativa del driver |
| `PixelType` | `2` si no se especifica modo | Color por defecto |
| `Resolution` | `200` DPI | Rango validado 75-600 |
| `IfFeederEnabled` | `true` | Usa ADF por defecto |
| `IfDuplexEnabled` | `false` si UI no envia `duplex` | Simplex por defecto |
| `IfDisableSourceAfterAcquire` | `true` | Cierra/deshabilita fuente despues de capturar |

### Causa raiz del duplex no activo

El Fujitsu fi-7160 soporta duplex, pero la UI actual llama:

```ts
scan({ deviceId: scanner.selectedDeviceId })
```

No envia `duplex: true`. Por contrato, `DynamsoftTwainClient.scan()` interpreta:

```ts
IfDuplexEnabled: options.duplex ?? false
```

Por tanto, el flujo actual escanea en simplex aunque el scanner fisico soporte duplex.

Punto exacto:

- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx`
  - `handleScan()` llama `scan({ deviceId })`.
- `src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts`
  - `scan()` construye `IfDuplexEnabled` desde `options.duplex ?? false`.

Correccion recomendada: agregar configuracion UI/controlada para modo simplex/duplex y enviar `duplex: true` cuando el usuario o contexto lo solicite. No debe activarse implicitamente sin decision de producto, porque cambia el comportamiento de captura.

## Auditoria de orientacion

### Dimensiones y orientacion capturadas

El adapter ya lee:

```ts
dwt.GetImageWidth(index)
dwt.GetImageHeight(index)
```

Y clasifica:

```txt
width > height  -> landscape
height > width  -> portrait
width == height -> square
sin datos       -> unknown
```

Tambien registra:

- `PAGE_DIMENSIONS`
- `PAGE_ORIENTATION`
- `ROTATION_STATE`
- `THUMBNAIL_DIMENSIONS`
- `PREVIEW_DIMENSIONS`

### Transformaciones actuales

| Capa | Transformacion | Estado |
| --- | --- | --- |
| Captura | No hay auto-rotate configurado en `AcquireImage` | No implementado |
| Adapter | Solo calcula orientacion; no rota automaticamente | Diagnostico |
| Rotacion manual | `dwt.Rotate(pageIndex, degrees, true)` | Implementado para boton Rotar |
| Miniatura | `GetImageURL(index, 160, 220)` | Implementado |
| Preview | `GetImageURL(index, -1, -1)` | Implementado |
| CSS miniatura | `aspect-ratio: 8.5 / 11`, `object-fit: contain` | Hoja vertical visual |
| CSS preview | `object-fit: contain` | No rota ni fuerza horizontal |

### Causa raiz probable para paginas horizontales

No hay `transform: rotate(...)` ni rotacion CSS en el workspace. Si una pagina aparece horizontal:

1. Si `PAGE_DIMENSIONS.width > height`, la imagen ya llega horizontal desde scanner/driver/Dynamsoft.
2. Si `PAGE_DIMENSIONS.width < height`, pero `THUMBNAIL_DIMENSIONS` o `PREVIEW_DIMENSIONS` reportan `width > height`, la transformacion ocurre en la URL generada por `GetImageURL(...)`.
3. Si todos los logs reportan portrait y aun se ve horizontal, habria que revisar layout externo o navegador, pero no se encontro transformacion en `DigitalizacionDocumentalWorkspace.module.css`.

Correccion recomendada: usar los logs reales con scanner fisico para decidir entre:

- configurar auto-rotate desde capacidades Dynamsoft/driver;
- exponer control de rotacion por lote;
- aplicar orientacion correctiva al generar miniatura/preview si la imagen llega horizontal pero el documento debe quedar vertical.

No se recomienda rotacion automatica ciega sin confirmar dimensiones reales y origen del giro.

## Auditoria de capacidades

### Escaneo

| Capacidad | Estado actual | Evidencia | Requiere desarrollo |
| --- | --- | --- | --- |
| Show Scanner UI | No expuesto | `IfShowUI: false` fijo | Si |
| Use ADF | Activo por defecto | `IfFeederEnabled: true` | No para uso basico |
| Duplex | Soportado por contrato, no expuesto en UI | `duplex?: boolean`, `IfDuplexEnabled` | Si |
| Flatbed | No expuesto | No hay bandera/control flatbed | Si |
| Color | Activo por defecto | `colorMode ?? "color"` -> `PixelType: 2` | No para default; si para UI |
| Gray | Soportado por contrato | `ScanColorMode = grayscale` -> `PixelType: 1` | Si, falta UI |
| B&W | Soportado por contrato | `ScanColorMode = blackWhite` -> `PixelType: 0` | Si, falta UI |
| Resolution | Soportado por contrato | `resolutionDpi`, default 200, rango 75-600 | Si, falta UI |
| Brightness | No implementado | No existe en tipos/opciones | Si |
| Contrast | No implementado | No existe en tipos/opciones | Si |

### Procesamiento

| Capacidad | Estado actual | Evidencia | Requiere desarrollo |
| --- | --- | --- | --- |
| Auto Rotate | No implementado | No hay configuracion en `AcquireImage` | Si |
| Deskew | No implementado | No hay API/tipo/flujo | Si |
| Auto Crop | No implementado | No hay API/tipo/flujo | Si |
| Blank Page Detection | No implementado | `removeBlankPages?: boolean` existe pero no se usa | Si |
| Blank Page Removal | No implementado | No hay llamada DWT asociada | Si |
| Border Removal | No implementado | No hay API/tipo/flujo | Si |

### Visualizacion

| Capacidad | Estado actual | Evidencia | Requiere desarrollo |
| --- | --- | --- | --- |
| Zoom In | No implementado | No hay control de zoom | Si |
| Zoom Out | No implementado | No hay control de zoom | Si |
| Fit Width | No implementado | CSS `object-fit: contain`, sin modo seleccionable | Si |
| Fit Page | Parcial visual | `object-fit: contain` | Si para control explicito |
| Rotate Left | No implementado | Boton rota 90 grados en un solo sentido | Si |
| Rotate Right | Implementado parcialmente | Boton `Rotar` llama `rotatePage(pageId, 90)` | Si para direccion explicita |

### Documentos

| Capacidad | Estado actual | Evidencia | Requiere desarrollo |
| --- | --- | --- | --- |
| Reordenar paginas | No implementado | No hay UI ni metodo | Si |
| Duplicar paginas | No implementado | No hay UI ni metodo | Si |
| Eliminar paginas | Implementado | `RemoveImage(index)` y boton Eliminar | No |
| Exportar PDF | Implementado | `ConvertToBlob(indices, "application/pdf")` | No |
| Exportar TIFF | No soportado por decision funcional | Salida final unica PDF | No recomendado |
| Exportar JPG | No soportado como salida final | Salida final unica PDF | No recomendado |
| Exportar PNG | No soportado como salida final | Salida final unica PDF | No recomendado |

## Capacidades reales Fujitsu fi-7160

Con base en el contexto validado del entorno:

- Scanner detectado: `PaperStream IP fi-7160 #2`.
- Scanner detectado alternativo: `WIATWAIN-fi-7160 #2`.
- El equipo fi-7160 soporta ADF y duplex a nivel de hardware.
- El flujo actual usa ADF (`IfFeederEnabled: true`).
- El flujo actual no activa duplex por defecto (`IfDuplexEnabled: false`).

Limite de la auditoria local: no se ejecuto captura fisica en esta sesion. Las capacidades reportadas por el runtime deben confirmarse con los logs `SCANNER_CAPABILITIES`, `SCAN_CONFIGURATION` y `DUPLEX_CONFIGURATION` en `/__sandbox/app-digitalizador`.

## Restricciones actuales

1. La configuracion de escaneo no esta expuesta en UI.
2. `ScanOptions` soporta `duplex`, `colorMode` y `resolutionDpi`, pero el workspace solo envia `deviceId`.
3. `removeBlankPages` existe en el contrato pero no tiene implementacion.
4. Las capacidades avanzadas de DWT no estan modeladas en `DynamsoftWebTwainObject`.
5. Los logs diagnosticos actuales son utiles para auditoria, pero deberian convertirse en logger controlado o removerse antes de estabilizar produccion.

## Auditoria de toolbar y layout actual

### Toolbar actual

Archivo responsable:

- `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx`
- estilos en `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.module.css`

Acciones actuales:

| Accion | Componente/control | Estado de habilitacion actual | Observacion |
| --- | --- | --- | --- |
| Seleccionar scanner | `<select>` | Deshabilitado si `scanner.loading` o no hay dispositivos | Debe permanecer, pero podria moverse al panel de configuracion para limpiar toolbar |
| Escanear | `AppButton` | Deshabilitado si no hay scanner seleccionado, si carga o si contexto invalido | Accion primaria; debe permanecer en toolbar |
| Reintentar | `AppButton` | Deshabilitado si `scanner.loading` | Es accion de recuperacion; puede quedar secundaria o integrarse al estado de error |
| Limpiar | `AppButton` | Deshabilitado si `scanner.loading` | Accion secundaria; debe permanecer pero no competir con Escanear |
| Rotar | `AppButton` | Deshabilitado si no hay pagina seleccionada | Actualmente rota 90 grados en un solo sentido; conviene separar Rotar Izq/Der |
| Eliminar | `AppButton` | Deshabilitado si no hay pagina seleccionada | Debe permanecer cerca de acciones de pagina |
| Generar PDF | `AppButton` | Deshabilitado si no hay paginas o hay carga/contexto invalido | Accion primaria posterior a captura |

### Agrupacion recomendada

| Grupo | Acciones | Razon |
| --- | --- | --- |
| Captura | Escanear | Inicio del flujo principal |
| Pagina seleccionada | Rotar Izq, Rotar Der, Eliminar | Operan sobre la pagina actual |
| Documento capturado | Limpiar, Generar PDF | Operan sobre todo el buffer/documento |
| Recuperacion | Reintentar | Mejor como accion contextual cuando hay error o sin scanners |

No se recomienda poner configuraciones avanzadas en toolbar. El toolbar debe quedar orientado a comandos, no a parametros.

### Toolbar recomendado

```txt
| Escanear | Rotar Izq | Rotar Der | Eliminar | Limpiar | Generar PDF |
```

Recomendacion visual:

- `Escanear`: boton primario, icono tipo scanner/camera.
- `Generar PDF`: boton primario o emphasized secundario cuando hay paginas.
- `Rotar Izq` / `Rotar Der`: botones icon-only con tooltip.
- `Eliminar`: boton icon-only con estado destructivo suave.
- `Limpiar`: boton secundario o ghost.
- `Reintentar`: no incluir como accion permanente; mostrarlo cuando `status = error` o `noScanner`.

## Reemplazo del panel Metadata por Configuracion de Escaneo

### Estado actual

El layout usa tres columnas:

```css
grid-template-columns: minmax(8rem, 0.72fr) minmax(14rem, 1.7fr) minmax(10rem, 0.92fr);
```

Distribucion actual:

```txt
Miniaturas | Preview PDF | Metadata
```

Problema:

- Metadata aporta poco durante captura.
- Consume la columna derecha completa.
- Reduce el area de preview.
- La configuracion de scanner no esta expuesta aunque el contrato ya soporta parte de ella.

### Panel derecho recomendado

Mantener columna derecha. No modal, no popup, no toolbar.

```txt
Configuracion de Escaneo

Scanner
[Fujitsu fi-7160 v]

Modo de captura
(*) DocuArchi
( ) Driver del Scanner

Si modo = DocuArchi:
[x] ADF
[x] Duplex
Color
[Color v]
Resolucion
[300 dpi v]

Si modo = Driver del Scanner:
Utilizar configuracion PaperStream
[Configurar Scanner]

Indicadores
ADF ✓  Duplex ✓  Color ✓  300 dpi
```

### Estrategia para reemplazar Metadata

1. Mover Metadata a un area colapsable, footer contextual o paso de confirmacion, no al centro de captura.
2. Sustituir la columna derecha por `ScannerSettingsPanel`.
3. Mantener el ancho actual de la columna derecha para no reestructurar todo el layout.
4. Pasar configuracion seleccionada a `scan(options)` sin modificar el adapter directamente en esta fase de diseno.

## Modo DocuArchi vs modo Driver Scanner

### Modo A: Configuracion administrada por DocuArchi

Usa opciones controladas por la aplicacion:

| Control UI | Opcion tecnica | Estado actual |
| --- | --- | --- |
| ADF | `IfFeederEnabled` | Actualmente fijo en `true` |
| Duplex | `IfDuplexEnabled` | Soportado por contrato, no enviado desde UI |
| Color | `PixelType` | Soportado por contrato, default color |
| Resolucion | `Resolution` | Soportado por contrato, default 200 |

Impacto tecnico:

- Extender estado React con `scanSettings`.
- Enviar `scan({ deviceId, duplex, colorMode, resolutionDpi })`.
- Validar defaults por modulo o por usuario.
- Persistir preferencias si producto lo decide.

### Modo B: Configuracion administrada por driver Fujitsu

Usa ventana nativa PaperStream IP mediante:

```ts
IfShowUI: true
```

Viabilidad:

- Es viable conceptualmente con DWT/TWAIN porque `AcquireImage` soporta `IfShowUI`.
- Requiere exponer un modo de captura que no fuerce todos los parametros desde DocuArchi.
- Debe validarse en equipo fisico porque la experiencia depende del driver PaperStream instalado.

Impacto tecnico:

- Agregar `scanMode: "docuarchi" | "driver"` al contrato interno de escaneo.
- Si `driver`, enviar `IfShowUI: true` y evitar sobrescribir configuraciones que el driver administra.
- Definir si `Configurar Scanner` abre la UI del driver antes de capturar o captura directamente con UI visible.
- Asegurar que el cierre de fuente y manejo de errores funcione igual que en modo actual.

Riesgos:

- La UI nativa puede bloquear el flujo hasta que el usuario confirme/cancele.
- La configuracion queda fuera de control de DocuArchi, por lo que auditoria y reproduccion pueden ser menos deterministicas.
- Algunas opciones PaperStream pueden persistir fuera de la aplicacion.

## Layout recomendado

Distribucion objetivo:

```txt
| Miniaturas | Preview PDF                         | Configuracion |
|            | Area principal de trabajo             | Escaneo       |
```

Recomendaciones:

1. Mantener miniaturas en columna izquierda.
2. Mantener configuracion en columna derecha compacta.
3. Dar mayor peso al preview central.
4. Mantener scroll independiente en miniaturas.
5. Hacer el panel derecho colapsable solo como mejora posterior, no como requisito inicial.

Propuesta de grid:

```css
grid-template-columns:
  minmax(8rem, 0.62fr)
  minmax(18rem, 2.25fr)
  minmax(10rem, 0.82fr);
```

Esta propuesta aumenta el protagonismo del preview sin eliminar el lateral derecho.

## Mockup textual propuesto

```txt
Digitalizador documental                         Workflow

Gabinete: ... | Radicado: ... | Documento destino: ...

[Escanear] [↺] [↻] [Eliminar] [Limpiar] [Generar PDF]

┌──────────────┬──────────────────────────────┬──────────────────────┐
│ Miniaturas   │ Preview PDF                  │ Configuracion        │
│              │                              │ de Escaneo           │
│ ┌────────┐   │                              │                      │
│ │ pagina │   │        hoja escaneada        │ Scanner              │
│ │        │   │                              │ [Fujitsu fi-7160 v]  │
│ └────────┘   │                              │                      │
│ Pagina 1     │                              │ Modo de captura      │
│              │                              │ (*) DocuArchi        │
│ ┌────────┐   │                              │ ( ) Driver Scanner   │
│ │ pagina │   │                              │                      │
│ └────────┘   │                              │ [x] ADF              │
│ Pagina 2     │                              │ [x] Duplex           │
│              │                              │ Color [Color v]      │
│              │                              │ DPI   [300 v]        │
└──────────────┴──────────────────────────────┴──────────────────────┘
```

## Impacto tecnico por cambio

| Cambio | Impacto | Riesgo | Orden |
| --- | --- | --- | --- |
| Reordenar toolbar | Bajo | Bajo | 1 |
| Separar rotar izq/der | Bajo/medio | Requiere soporte de grados 270 o direccion explicita | 2 |
| Reemplazar Metadata por Configuracion | Medio | Puede afectar flujos que necesitan metadata visible | 3 |
| Estado `scanSettings` | Medio | Requiere defaults claros | 4 |
| Enviar duplex/color/DPI desde UI | Medio | Cambia resultado de captura | 5 |
| Modo driver `IfShowUI=true` | Medio/alto | Depende de PaperStream y UX nativa | 6 |
| Persistir preferencias | Medio | Requiere decision por usuario/modulo | 7 |
| Panel colapsable | Bajo/medio | Puede ocultar configuracion critica | 8 |

## Orden recomendado de implementacion

1. Crear `ScannerSettingsPanel` visual sin conectar comportamiento.
2. Reubicar selector de scanner al panel derecho.
3. Simplificar toolbar con acciones de comando.
4. Introducir estado local `scanSettings` con defaults actuales para no cambiar comportamiento.
5. Conectar `colorMode` y `resolutionDpi`.
6. Conectar `duplex` de forma explicita, inicialmente apagado por default.
7. Agregar modo driver con `IfShowUI=true` tras prueba fisica PaperStream.
8. Evaluar persistencia de preferencias y panel colapsable.

## Recomendaciones tecnicas

1. Crear un panel de configuracion de escaneo con:
   - modo simplex/duplex;
   - ADF/flatbed si el driver lo permite;
   - color/grayscale/B&W;
   - DPI;
   - show driver UI para diagnostico avanzado.
2. Confirmar capacidades reales del Fujitsu fi-7160 con `SCANNER_CAPABILITIES` en equipo fisico.
3. No activar duplex por defecto sin decision funcional: cambia el resultado esperado y puede duplicar paginas.
4. Implementar auto-rotate solo si la evidencia confirma que las imagenes nacen horizontales desde DWT/driver.
5. Convertir `removeBlankPages` en funcionalidad real o retirarlo del contrato para evitar falsa promesa.
6. Mantener PDF como unica salida documental final; JPG/PNG/TIFF solo podrian considerarse como herramientas internas de preview/export diagnostico, no como almacenamiento final.

## Validacion recomendada con scanner fisico

1. Abrir `/__sandbox/app-digitalizador`.
2. Seleccionar `PaperStream IP fi-7160 #2`.
3. Escanear hoja vertical de una cara.
4. Registrar:
   - `PAGE_DIMENSIONS`
   - `THUMBNAIL_DIMENSIONS`
   - `PREVIEW_DIMENSIONS`
   - `ROTATION_STATE`
5. Escanear hoja doble cara con configuracion actual.
6. Confirmar que `SCAN_CONFIGURATION.IfDuplexEnabled` sea `false`.
7. Confirmar cantidad de paginas capturadas.
8. Repetir cuando exista UI/configuracion duplex.

## Implementacion aplicada

Despues de esta auditoria, el usuario autorizo implementar el rediseno recomendado para SCRUMCORE-249.

Cambios aplicados:

1. `DigitalizacionDocumentalWorkspace` mantiene el componente inline reutilizable.
2. El toolbar superior queda como barra de comandos:
   - Escanear;
   - Rotar izq;
   - Rotar der;
   - Eliminar;
   - Limpiar;
   - Generar PDF.
3. La seleccion de scanner se movio al panel derecho.
4. El panel derecho Metadata fue reemplazado por Configuracion de Escaneo.
5. El modo DocuArchi expone ADF, Duplex, Color y Resolucion.
6. El modo Driver Scanner usa `IfShowUI: true` para abrir la UI nativa PaperStream durante la captura.
7. `DynamsoftTwainClient.scan()` ahora recibe:
   - `IfShowUI`;
   - `IfFeederEnabled`;
   - `IfDuplexEnabled`;
   - `Resolution`;
   - `PixelType`.
8. El preview PDF gana mayor proporcion horizontal en la grilla principal.

Validacion manual pendiente:

1. Confirmar fisicamente duplex con Fujitsu fi-7160.
2. Confirmar que PaperStream IP abre correctamente en modo Driver Scanner.
3. Confirmar que color/gris/B/N y resoluciones disponibles son aceptadas por el driver instalado.

## Conclusion

El modulo actual esta funcional para captura basica, miniaturas, preview, rotacion manual, eliminacion y PDF. SCRUMCORE-249 expone ahora configuracion controlada para ADF, duplex, color, resolucion y modo Driver Scanner. Las capacidades mas avanzadas de Dynamsoft, como deskew, auto-crop, auto-rotate y blank-page removal, siguen pendientes de diseño funcional y validacion fisica. La orientacion debe decidirse con las dimensiones reales registradas; no hay evidencia de rotacion CSS en el workspace.
