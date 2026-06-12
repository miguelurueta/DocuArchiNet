# SCRUMCORE-247 Retorno a Dynamsoft 19.3.2

## Contexto

Se retorna a la configuracion alineada con el equipo local:

```txt
SDK frontend: dwt@19.3.2
Dynamsoft Service: 1.9.3.1028
TWAIN Module: 19.3.2.0306
```

Motivo:

```txt
La licencia anterior no cubria familia 19.x.
La nueva licencia ya cubre Dynamic Web TWAIN 19.3.
```

## Cambio aplicado

Archivo:

```txt
src/modules/digitalizacion/infrastructure/dynamsoft/dynamsoft.constants.ts
```

Valores restaurados:

```txt
DYNAMSOFT_SDK_VERSION = "19.3.2"
DYNAMSOFT_DEFAULT_RESOURCES_PATH = https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist
DYNAMSOFT_DEFAULT_SCRIPT_SRC = https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dynamsoft.webtwain.min.js
DYNAMSOFT_SERVICE_INSTALLER_URL = https://cdn.jsdelivr.net/npm/dwt@19.3.2/dist/dist/DynamicWebTWAINServiceSetup.msi
```

## Correcciones conservadas

```txt
ContainerId
CSS loading
ResourcesPath
DynamsoftTwainClient
AppDigitalizador
DigitalizacionDocumentalWorkspace
Sandbox
Tests
```

## Referencias residuales a 18.5.0

No queda referencia activa a `18.5.0` en codigo runtime despues del retorno.

Las referencias restantes estan en documentos historicos:

```txt
docs/Architecture/DigitalizacionDocumental/SCRUMCORE-249-Dynamsoft-1850.md
docs/Architecture/DigitalizacionDocumental/SCRUMCORE-248-Dynamsoft-1920.md
openspec/changes/scrumcore-247-modulo-reusable-digitalizaciondocumental-actualizacion-sdk/*
```

## Checklist manual

Abrir:

```txt
/__sandbox/app-digitalizador
```

Validar:

```txt
1. No aparece error -2539.
2. No aparece "Failed to connect to the service".
3. Dynamsoft inicializa.
4. Lista scanners fisicos.
5. Permite seleccionar scanner.
6. Permite escanear.
7. Genera miniaturas.
8. Genera PDF.
```

Si aparece un nuevo error, registrar:

```txt
codigo
mensaje
punto exacto del flujo
```
