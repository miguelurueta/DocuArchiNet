# Casos de uso implementados

## UC-01 — Login con segundo factor apagado

| Campo | Detalle |
| --- | --- |
| Actor | Usuario de DocuArchi, Gestor, Radicación o Workflow. |
| Precondiciones | Empresa/módulo válidos, conexiones preparadas y credenciales aceptadas por `ValidaUserAplicacion`. |
| Flujo principal | Resolver módulo/configuración; resolver ID/login sin exigir correo; crear `LegacyLoginFinalizationContext`; ejecutar `FinalizeLogin`; redirigir si `RedirectRequired=True`. |
| Alternativas | `0` y `NULL` tienen el mismo resultado. El correo ausente no bloquea esta rama. Las salidas tempranas históricas conservan `RedirectRequired=False`. |
| Errores | Módulo o identidad inexistentes, duplicados o con ID inválido fallan cerrados. |
| Resultado | Mismos efectos legacy y retorno `YES`. |
| Endpoint / Service / Repository | No aplica / `SecondFactorPreAuthenticationService.Execute` / repositorio central y adaptador del módulo. |

## UC-02 — Preautenticación con segundo factor activo

| Campo | Detalle |
| --- | --- |
| Actor | Usuario con credenciales válidas. |
| Precondiciones | Fila única de módulo con `RequiereSegundoFactor=1`, proveedor EMAIL=1 y expiración de 1 a 10 minutos. |
| Flujo principal | Resolver configuración, identidad y correo válido; devolver `SECOND_FACTOR_REQUIRED`. |
| Alternativas | No aplica. |
| Errores | Proveedor/expiración inválidos producen `PREAUTHENTICATION_FAILED`; correo ausente produce `SECOND_FACTOR_EMAIL_UNAVAILABLE`. |
| Resultado | No se llama al finalizador; no hay cookie, auditoría, permisos ni sesiones autenticadas. |
| Endpoint / Service / Repository | No aplica / `SecondFactorPreAuthenticationService.Execute` / repositorio central y adaptador del módulo. |

## UC-03 — Rechazar selección central inválida

| Campo | Detalle |
| --- | --- |
| Actor | Usuario en postback privado. |
| Precondiciones | Credenciales legacy válidas. |
| Flujo principal | Consultar empresa y módulo mediante parámetros ODBC. |
| Alternativas | Cero filas devuelve `LOGIN_MODULE_NOT_FOUND`; varias filas o configuración inválida devuelven error sanitizado. |
| Errores | No se propaga detalle SQL al resultado público. |
| Resultado | Login no finalizado. |
| Endpoint / Service / Repository | No aplica / servicio de preautenticación / `OdbcSecondFactorLoginModuleRepository.Resolve`. |

## UC-04 — Resolver identidad por módulo

| Campo | Detalle |
| --- | --- |
| Actor | Servicio de preautenticación. |
| Precondiciones | Tipo de módulo resuelto centralmente y snapshot de conexión preparado por Presentation. |
| Flujo principal | Seleccionar exactamente uno de cuatro repositorios; consultar por `@login`; proyectar ID, login canónico y correo. |
| Alternativas | El mismo login en otra empresa usa otro snapshot y no comparte consulta. |
| Errores | Cero o múltiples filas e ID no positivo impiden cualquier recorrido. El correo vacío impide únicamente la rama con 2FA activo. |
| Resultado | `SecondFactorPrincipal`; contiene destino enmascarado cuando existe correo válido. |
| Endpoint / Service / Repository | No aplica / resolver de repositorios / adaptador MySQL por módulo. |

## UC-05 — Credenciales inválidas

| Campo | Detalle |
| --- | --- |
| Actor | Usuario del login privado. |
| Precondiciones | Postback recibido. |
| Flujo principal | `ValidaUserAplicacion` devuelve un valor distinto de `YES`; el wrapper retorna antes de crear el servicio. |
| Alternativas | Se conservan mensajes y validaciones legacy por módulo. |
| Errores | No aplica. |
| Resultado | No consulta configuración 2FA ni identidad y no autentica. |
| Endpoint / Service / Repository | `gestor.aspx` postback / `InicioAplicacionWebGestorDocumental` / No aplica. |
