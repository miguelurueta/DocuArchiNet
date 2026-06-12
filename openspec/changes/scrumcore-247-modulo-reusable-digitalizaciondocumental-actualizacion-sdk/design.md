## Context

El adapter `DynamsoftTwainClient` usa el modelo clasico `Dynamsoft.DWT` y las siguientes APIs:

```txt
runtime.ProductKey
runtime.ResourcesPath
runtime.Containers
runtime.Load()
runtime.GetWebTwain()
runtime.Unload()
SourceCount
GetSourceNameItems()
SelectSourceByIndex()
OpenSource()
AcquireImage()
CloseSource()
Rotate()
RemoveImage()
RemoveAllImages()
ConvertToBlob("application/pdf")
```

Se inspecciono el paquete real `dwt@19.3.2` en jsDelivr. El paquete conserva los recursos requeridos:

```txt
https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dynamsoft.webtwain.min.js
https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/src/dynamsoft.webtwain.css
https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/src/dynamsoft.webtwain.viewer.css
https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dist/DynamicWebTWAINServiceSetup.msi
```

## Decisions

1. Usar `dwt@19.3.2` desde CDN para alinear con el servicio local `1.9.3.1028`.
2. Mantener `ResourcesPath` apuntando al directorio `dist` del paquete CDN.
3. Mantener inyeccion explicita de los dos CSS para evitar el error `-2804`.
4. No modificar `package.json`; no existe dependencia `dwt` instalada localmente.
5. Mantener el adapter callback-style actual porque las APIs usadas siguen presentes en `dwt@19.3.2`.

## Final Constants

```txt
DYNAMSOFT_SDK_VERSION = 19.3.2
DYNAMSOFT_EXPECTED_SERVICE_VERSION = 1.9.3.1028
DYNAMSOFT_EXPECTED_TWAIN_MODULE_VERSION = 19.3.2
DYNAMSOFT_DEFAULT_RESOURCES_PATH = https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist
DYNAMSOFT_DEFAULT_SCRIPT_SRC = https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dynamsoft.webtwain.min.js
DYNAMSOFT_SERVICE_INSTALLER_URL = https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dist/DynamicWebTWAINServiceSetup.msi
```

## Risks

- La licencia Dynamsoft debe cubrir la familia 19.x.
- La validacion completa requiere scanner fisico y servicio Windows activo.
- El mensaje de actualizacion debe desaparecer solo cuando navegador, SDK y servicio local queden alineados en runtime real.

## Manual Validation

Abrir:

```txt
/__sandbox/app-digitalizador
```

Validar:

- no aparece `Please update your document scanning service`;
- lista scanners;
- selecciona scanner;
- escanea;
- muestra miniaturas;
- muestra preview;
- genera PDF.
