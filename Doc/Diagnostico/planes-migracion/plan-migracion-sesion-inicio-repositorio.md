# Plan de migración: vigilancia general de sesión en Inicio

## Objetivo

Incorporar la sonda de sesión de la página de Inicio para advertir al usuario y redirigirlo cuando la sesión haya caducado.

## Archivos del paquete

```text
session-status.ashx
js\session-status-watchdog.js
Defaul\WebFormInicioDocuarchiGestion.aspx
```

## Dependencia crítica

El script y el handler son inseparables. `session-status-watchdog.js` consulta `session-status.ashx`; el handler existe en `D:\temfile\Gestion`, pero no en el repositorio local.

El handler requiere sesión ASP.NET (`IRequiresSessionState`) y reconoce las claves:

```text
Id_Usuario_Workflow
GA_IDUSUARIOGESTION
RA_ID_USUARIO
ID_USUARIO_DOCUARCHI
```

## Implementación

1. Crear `session-status.ashx` en la raíz del repositorio con el contenido completo validado del precompilado.
2. Copiar `js\session-status-watchdog.js` al repositorio.
3. Cargarlo después de jQuery y de los scripts base en `Defaul\WebFormInicioDocuarchiGestion.aspx`:

```html
<script src="../js/session-status-watchdog.js?v=20260805-1min"></script>
```

4. Revisar la ruta `/GestorII/session-status.ashx` dentro del script. Solo conservarla si `GestorII` es el virtual directory definitivo; de lo contrario, usar una ruta relativa o una configuración de aplicación.
5. Confirmar que el handler solo responde a solicitudes AJAX con `X-Requested-With: XMLHttpRequest`.

## Validación

- Sesión válida: HTTP 200 con `{"sessionValid":true}`.
- Sesión expirada: HTTP 401 con `{"sessionExpired":true}`.
- Acceso directo al handler: HTTP 404.
- El watchdog muestra una sola capa de bloqueo y redirige una vez al login.
- Errores de red, HTTP 0 o 503 no expulsan al usuario.

## Reversión

Revertir juntos `session-status.ashx`, `js\session-status-watchdog.js` y su referencia en Inicio. No dejar el script cargado sin handler ni el handler expuesto sin consumidor aprobado.
