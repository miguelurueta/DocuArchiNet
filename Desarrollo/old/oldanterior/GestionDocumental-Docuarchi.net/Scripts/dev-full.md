# dev-full.ps1

Script de desarrollo para la aplicacion legacy ASP.NET Framework/WebForms.

## Uso

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/dev-full.ps1
```

Parametros utiles:

- `-Configuration Debug|Release`: configuracion de MSBuild. Por defecto `Debug`.
- `-Url <url>`: reemplaza la URL configurada en el `.vbproj`.
- `-Retries <n>`: numero de intentos para validar el endpoint. Por defecto `30`.
- `-DelaySeconds <n>`: espera entre intentos. Por defecto `2`.
- `-SkipBuild`: valida IIS y el endpoint sin compilar.
- `-OpenBrowser`: abre la URL cuando el endpoint responde.
- `-MSBuildPath <path>`: usa una ruta explicita de MSBuild.

## Prerrequisitos Manuales

El script no crea ni modifica IIS automaticamente. La maquina debe tener:

- IIS habilitado y servicio `W3SVC` iniciado.
- Aplicacion virtual o sitio accesible en `https://localhost/GestionDocumental-Docuarchi.net`.
- Application Pool compatible con .NET CLR v4.0.
- Binding HTTPS/certificado local configurado si se usa la URL por defecto.
- Paquetes restaurados en `..\packages` o ensamblados disponibles en `bin`.
- DSN ODBC `MembershipUsers` y MySQL local configurados segun `Web.config` cuando la pagina los requiera.
